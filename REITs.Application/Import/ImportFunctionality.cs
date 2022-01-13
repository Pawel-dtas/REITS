using Domain;
using Domain.Enums;
using Domain.Models;
using Prism.Mvvm;
using REITs.DataLayer.Services;
using REITs.Domain.Enums;
using REITs.Domain.Models;
using REITs.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;

namespace REITs.Application
{
	public class ImportFunctionality : BindableBase, IDisposable
	{
		#region Properties

		public List<XMLProcessingResult> XMLProcessingResults { get; set; }

		private static XmlSchemaSet CompiledSchemaSetCached { get; set; }

		#endregion Properties

		#region Variables

		private IREITDataService _dataService = PrismHelpers.ResolveService<IREITDataService>();

		private List<string> Errors = new List<string>();

		private string currentXMLFileName = string.Empty;

		private XDocument currentXDoc;

		#endregion Variables

		#region Constructor

		public ImportFunctionality()
		{ }

		#endregion Constructor

		#region Public Methods

		public void ValidateXMLFiles()
		{
			foreach (var xmlResult in XMLProcessingResults)
			{
				var tempImportStatus = ImportXMLStatusTypes.NotSet;
				var tempImportCompanyStatus = ImportCompanyStatusTypes.NotSet;

				currentXMLFileName = xmlResult.FileName;

				try
				{
					currentXDoc = LoadXMLDoc(currentXMLFileName);

					var xmlIsValid = ValidateXML(currentXDoc, GetSchemaSpec());

					tempImportStatus = (xmlIsValid) ? ImportXMLStatusTypes.Validated : ImportXMLStatusTypes.Errors;

					int currentVersion = GetExistingImportedXMLVersionNum(currentXDoc);

					if (currentVersion > 0)
					{
						tempImportStatus = (xmlIsValid) ? ImportXMLStatusTypes.ValidatedAndExists : ImportXMLStatusTypes.ErrorsAndExists;

						xmlResult.Version = currentVersion;

						var r1 = CheckForExactMatchOnSubmissionDate(currentXDoc);
						var r2 = GetXMLModifiedDate(currentXMLFileName);

						if (r1 == r2)
							tempImportStatus = ImportXMLStatusTypes.ValidatedAndExistsExactMatch;
					}

					var tempParent = CheckForExistingREITParent(currentXDoc);

					if (tempParent != null)
					{
						xmlResult.ParentCompanyGuid = tempParent.Id;
						tempImportCompanyStatus = ImportCompanyStatusTypes.Exists;
					}
					else
					{
						tempImportCompanyStatus = ImportCompanyStatusTypes.NoMatch;
					}

					// update xmlResult //
					xmlResult.ValidationMessage = GetValidationErrorMessagesAsString();
					xmlResult.XMLStatus = tempImportStatus;
					xmlResult.ImportCompanyStatus = tempImportCompanyStatus;
				}
				catch (XmlSchemaException ex) { Debug.Print(ex.ToString()); }
			}

			currentXMLFileName = String.Empty;
		}

		public void ImportXMLFiles(object sender)
		{
			int filesToProcess = XMLProcessingResults.Where(r => r.XMLStatus == ImportXMLStatusTypes.Validated
															|| r.XMLStatus == ImportXMLStatusTypes.ValidatedAndExists).Count();

			int processedFiles = 0;

			if (filesToProcess > 0)
			{
				foreach (var currentXPR in XMLProcessingResults.Where(r => r.XMLStatus == ImportXMLStatusTypes.Validated
																									|| r.XMLStatus == ImportXMLStatusTypes.ValidatedAndExists))
				{
					(sender as BackgroundWorker).ReportProgress(processedFiles, currentXPR.FileName);

					REIT tempREIT = GetXMLData(currentXPR.FileName);

					currentXPR.ImportCompanyStatus = ImportCompanyStatusTypes.NotSet;

					if (tempREIT != null)
					{
						currentXPR.ImportCompanyStatus = (currentXPR.ParentCompanyGuid == Guid.Empty) ? ImportCompanyStatusTypes.NoMatch : ImportCompanyStatusTypes.Exists;

						if (currentXPR.ParentCompanyGuid != Guid.Empty)
						{
							tempREIT.ParentId = currentXPR.ParentCompanyGuid;
							tempREIT.REITTotal.Add(CreateREITTotals(tempREIT));
							tempREIT.XMLVersion = currentXPR.Version + 1;

							currentXPR.XMLStatus = (SaveImportedRecords(tempREIT)) ? ImportXMLStatusTypes.ImportedOK : ImportXMLStatusTypes.ImportError;

							processedFiles++;

							(sender as BackgroundWorker).ReportProgress(processedFiles, currentXPR.FileName);
						}
					}
				}
			}
		}

		#endregion Public Methods

		#region Private Methods

		private XDocument LoadXMLDoc(string xmlFilename)
		{
			var xmlData = XMLExtensions.LoadAndFormatXML(xmlFilename);

			var xd = XDocument.Parse(xmlData, LoadOptions.SetLineInfo);
			xd.Declaration = null;

			xd.Descendants()
				   .Attributes()
				   .Where(x => x.IsNamespaceDeclaration)
				   .Remove();

			foreach (var elem in xd.Descendants())
				elem.Name = elem.Name.LocalName;

			return xd;
		}

		private REIT GetXMLData(string xmlFilePath)
		{
			REIT tempREIT = new REIT();

			try
			{
				XDocument xmlDoc = LoadXMLDoc(xmlFilePath);
				tempREIT = ParseToREIT(xmlDoc);

				tempREIT.XMLDateSubmitted = GetXMLModifiedDate(xmlFilePath);
			}
			catch { tempREIT = null; }

			return tempREIT;
		}

		private REIT ParseToREIT(XDocument xmlDoc)
		{
			REIT importedREIT = new REIT();

			XElement root = xmlDoc.Root;

			importedREIT.REITName = root.Element("ReitName").GetValue();
			importedREIT.PrincipalUTR = root.Element("ReitUTR").GetValue();
			importedREIT.AccountPeriodEnd = (DateTime)root.Element("ReitAPE").GetValueAsDateTime();
			importedREIT.PreviousAccountPeriodEnd = (DateTime)root.Element("ReitAPE").GetValueAsDateTime();
			importedREIT.REITNotes = root.Element("ReitNotes").GetValue();

			importedREIT.DateCreated = DateTime.Now;
            importedREIT.CreatedBy = System.Environment.UserName;
            //importedREIT.CreatedBy = "7209233";

			var xmlDocument = ToXmlDocument(xmlDoc);

			foreach (XmlNode xnRecon in xmlDocument.SelectNodes("/Reit/ReitReconciliations/Reconciliation"))
			{
				importedREIT.Reconciliations.Add(new Reconciliation()
				{
					ReconciliationType = xnRecon["ReconciliationType"].GetValue(),
					ReconciliationName = xnRecon["ReconciliationName"].GetValue(),
					ReconciliationAmount = xnRecon["ReconciliationAmount"].GetValueAsDouble()
				});
			}

			foreach (XmlNode xnEntity in xmlDocument.SelectNodes("/Reit/Entities/Entity"))
			{
				var entity = new Entity()
				{
					EntityName = xnEntity["EntityName"].GetValue(),
					EntityType = xnEntity["EntityType"].GetValue(),
					EntityUTR = xnEntity["EntityUTR"].GetValue(),
					InterestPercentage = xnEntity["EntityPercentage"].GetValueAsDouble(),
					Jurisdiction = xnEntity["EntityJurisdiction"].GetValue(),
					TaxExempt = xnEntity["EntityTaxExempt"].GetValueAsBoolean(),
					CustomerReference = xnEntity["EntityCustomerReference"].GetValue()
				};

				foreach (XmlNode xnAdj in xnEntity.SelectNodes("EntityAdjustments/EntityAdjustment"))
				{
					entity.Adjustments.Add(new Adjustment()
					{
						AdjustmentCategory = xnAdj["AdjustmentInputType"].GetValue(),
						AdjustmentType = xnAdj["AdjustmentType"].GetValue(),
						AdjustmentName = xnAdj["AdjustmentName"].GetValue(),
						AdjustmentAmount = xnAdj["AdjustmentAmount"].GetValueAsDouble()
					});
				}

				importedREIT.Entities.Add(entity);
			}

			return importedREIT;
		}

		private int GetExistingImportedXMLVersionNum(XDocument xmlDoc)
		{
			var root = xmlDoc.Root;

			var REITName = root.XPathSelectElement("ReitName").GetValue();
			var PrincipalUTR = root.XPathSelectElement("ReitUTR").GetValue();
			var AccountPeriodEnd = (DateTime)root.XPathSelectElement("ReitAPE").GetValueAsDateTime();

			return _dataService.CheckIfREITExists(PrincipalUTR, REITName, AccountPeriodEnd);
		}

		private DateTime CheckForExactMatchOnSubmissionDate(XDocument xmlDoc)
		{
			var root = xmlDoc.Root;

			var REITName = root.XPathSelectElement("ReitName").GetValue();
			var PrincipalUTR = root.XPathSelectElement("ReitUTR").GetValue();
			var AccountPeriodEnd = (DateTime)root.XPathSelectElement("ReitAPE").GetValueAsDateTime();

			return _dataService.CheckIfREITExistsExactMatch(PrincipalUTR, REITName, AccountPeriodEnd); ;
		}

		private DateTime GetXMLModifiedDate(string fileName)
		{
			return DateTime.Parse(File.GetLastWriteTime(fileName).ToString());
		}

		private REITParent CheckForExistingREITParent(XDocument xmlDoc)
		{
			var root = xmlDoc.Root;

			var REITParentName = root.Element("ReitName").GetValue();
			var PrincipalUTR = root.Element("ReitUTR").GetValue();

			return _dataService.CheckIfREITParentExists(PrincipalUTR, REITParentName);
		}

		public Guid CreateImportedCompanyRecord(REIT ImportedReit)
		{
			var success = Guid.Empty;

			try
			{
				var newREITParent = new REITParent()
				{
					PrincipalCustomerName = ImportedReit.REITName,
					PrincipalUTR = ImportedReit.PrincipalUTR,
					APEDate = ImportedReit.AccountPeriodEnd
				};

				_dataService.SaveREITParent(newREITParent);

				success = newREITParent.Id;
			}
			catch { }

			return success;
		}

		// DJC - 15-11-2019
		private void ParseXSD()
		{
			try
			{
				XmlSchema schema;
				using (var reader = new StreamReader(GetXSDFilePath()))
				{
					schema = XmlSchema.Read(reader, null);
				}

				XmlSchemaSet schemaSet = new XmlSchemaSet();
				schemaSet.Add(schema);
				schemaSet.Compile();

				schema = schemaSet.Schemas().Cast<XmlSchema>().First();

				foreach (XmlSchemaElement element in schema.Items)
				{
					XmlSchemaSimpleType simpleType = element.ElementSchemaType as XmlSchemaSimpleType;

					if (simpleType != null)
					{
						var restriction = (XmlSchemaSimpleTypeRestriction)simpleType.Content;

						var enumFacets = restriction.Facets.OfType<XmlSchemaEnumerationFacet>();

						if (enumFacets.Count() > 0)
						{
							Debug.WriteLine(element.Name);
							enumFacets.ToList().ForEach(p => Debug.WriteLine(string.Format("++ {0}", p.Value.ToString())));
						}
					}
				}
			}
			catch (Exception ex) { Debug.Print(ex.Message.ToString()); }
		}

		private REITTotals CreateREITTotals(REIT importedREIT)
		{
			REITTotals tempREITTotal = new REITTotals();

			// IDENTIFY UK AND NON UK ENTITIES

			List<Entity> uKEntities = (importedREIT.Entities).Where(a => a.Jurisdiction == Jurisdictions.UnitedKingdom.GetDescriptionFromEnum()).ToList();
			List<Entity> nonUKEntities = (importedREIT.Entities).Where(a => a.Jurisdiction != Jurisdictions.UnitedKingdom.GetDescriptionFromEnum()).ToList();

			foreach (Entity entity in uKEntities)
			{
				foreach (var adjustment in entity.Adjustments)
				{
					#region UK-Property

					if (adjustment.AdjustmentCategory.Trim() == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentCategories.PropertyRentalBusiness))
					{
						//IAS - Profits/Expenses
						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.PropertyRentalBusinessIASProfitsExpenses))
						{
							switch (BaseEnumExtension.GetEnumFromString<PropertyAdjustmentNames>(adjustment.AdjustmentName))
							{
								case PropertyAdjustmentNames.RevenueLessCostOfSales:
									tempREITTotal.PropUKRevenueLessCostOfSales = tempREITTotal.PropUKRevenueLessCostOfSales + adjustment.AdjustmentAmount;
									break;

								case PropertyAdjustmentNames.PIDsFromOtherUKREITs:
									tempREITTotal.PropUKPIDs = tempREITTotal.PropUKPIDs + adjustment.AdjustmentAmount;
									break;

								default:
									break;
							}
						}

						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.PropertyRentalBusinessOtherIncomeOrExpense))
						{
							//Other Income or Expense
							List<string> otherIncomeOrExpenseOptions = BaseEnumExtension.GetEnumDescriptions<PropertyOtherIncomeOrExpenseNames>();

							if (otherIncomeOrExpenseOptions.Contains(adjustment.AdjustmentName) == true)
								tempREITTotal.PropUKOtherIncomeOrExpenseAmount = tempREITTotal.PropUKOtherIncomeOrExpenseAmount + adjustment.AdjustmentAmount;
						}

						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.PropertyRentalBusinessNetFinanceCosts))
						{
							if (adjustment.AdjustmentName == BaseEnumExtension.GetDescriptionFromEnum(PropertyAdjustmentNames.NetFinanceCostsExternal))
								tempREITTotal.PropUKNetFinanceCosts = tempREITTotal.PropUKNetFinanceCosts + adjustment.AdjustmentAmount;
						}

						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.PropertyRentalBusinessOtherAdjustments))
						{
							//Other Adjustments
							List<string> otherAdjustmentOptions = BaseEnumExtension.GetEnumDescriptions<PropertyOtherAdjustmentNames>();

							if (otherAdjustmentOptions.Contains(adjustment.AdjustmentName) == true)
								tempREITTotal.PropUKOtherAdjustmentsAmount = tempREITTotal.PropUKOtherAdjustmentsAmount + adjustment.AdjustmentAmount;
						}

						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.PropertyRentalBusinessIASAssets))
						{
							//Assets
							switch (BaseEnumExtension.GetEnumFromString<PropertyAdjustmentNames>(adjustment.AdjustmentName))
							{
								case PropertyAdjustmentNames.NonCurrentAssets:
									tempREITTotal.PropUKNonCurrentAssets = tempREITTotal.PropUKNonCurrentAssets + adjustment.AdjustmentAmount;
									break;

								case PropertyAdjustmentNames.CurrentAssets:
									tempREITTotal.PropUKCurrentAssets = tempREITTotal.PropUKCurrentAssets + adjustment.AdjustmentAmount;
									break;

								default:
									break;
							}
						}
					}

					#endregion UK-Property

					#region UK-Residual

					if (adjustment.AdjustmentCategory.Trim() == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentCategories.ResidualIncome))
					{
						//IAS - Profits/Expenses
						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.ResidualIncomeIASProfitsExpenses))
						{
							switch (BaseEnumExtension.GetEnumFromString<ResidualAdjustmentNames>(adjustment.AdjustmentName))
							{
								case ResidualAdjustmentNames.RevenueLessCostOfSales:
									tempREITTotal.ResUKRevenueLessCostOfSales = tempREITTotal.ResUKRevenueLessCostOfSales + adjustment.AdjustmentAmount;
									break;

								case ResidualAdjustmentNames.BeneficialInterestsIncome:
									tempREITTotal.ResUKBeneficialInterestsIncome = tempREITTotal.ResUKBeneficialInterestsIncome + adjustment.AdjustmentAmount;
									break;

								default:
									break;
							}
						}
						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.ResidualIncomeOtherIncomeOrExpense))
						{
							//Other Income or Expense
							List<string> otherIncomeOrExpenseOptions = BaseEnumExtension.GetEnumDescriptions<ResidualOtherIncomeOrExpenseNames>();

							if (otherIncomeOrExpenseOptions.Contains(adjustment.AdjustmentName) == true)
								tempREITTotal.ResUKOtherIncomeOrExpenseAmount = tempREITTotal.ResUKOtherIncomeOrExpenseAmount + adjustment.AdjustmentAmount;
						}

						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.ResidualIncomeNetFinanceCosts))
						{
							if (adjustment.AdjustmentName == BaseEnumExtension.GetDescriptionFromEnum(ResidualAdjustmentNames.NetFinanceCostsResidual))
								tempREITTotal.ResUKNetFinanceCosts = tempREITTotal.ResUKNetFinanceCosts + adjustment.AdjustmentAmount;
						}

						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.ResidualIncomeOtherAdjustments))
						{
							//Other Adjustments
							List<string> otherAdjustmentOptions = BaseEnumExtension.GetEnumDescriptions<ResidualOtherAdjustments>();

							 if (otherAdjustmentOptions.Contains(adjustment.AdjustmentName) == true)
								tempREITTotal.ResUKOtherAdjustmentsAmount = tempREITTotal.ResUKOtherAdjustmentsAmount + adjustment.AdjustmentAmount;
						}

						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.ResidualIncome_IAS_Assets))
						{
							switch (BaseEnumExtension.GetEnumFromString<ResidualAdjustmentNames>(adjustment.AdjustmentName))
							{
								case ResidualAdjustmentNames.NonCurrentAssets:
									tempREITTotal.ResUKNonCurrentAssets = tempREITTotal.ResUKNonCurrentAssets + adjustment.AdjustmentAmount;
									break;

								case ResidualAdjustmentNames.CurrentAssets:
									tempREITTotal.ResUKCurrentAssets = tempREITTotal.ResUKCurrentAssets + adjustment.AdjustmentAmount;
									break;

								default:
									break;
							}
						}
					}

					#endregion UK-Residual

					#region UK-TaxExmpt

					if (adjustment.AdjustmentCategory.Trim() == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentCategories.TaxExempt))
					{
						//PRB Profits Before Tax
						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.TaxExemptUKPRBProfits))
						{
							switch (BaseEnumExtension.GetEnumFromString<TaxExemptAdjustmentNames>(adjustment.AdjustmentName))
							{
								case TaxExemptAdjustmentNames.PRBProfitsBeforeTax:
									tempREITTotal.TaxExUKPRBProfitsBeforeTax = tempREITTotal.TaxExUKPRBProfitsBeforeTax + adjustment.AdjustmentAmount;
									break;

								case TaxExemptAdjustmentNames.PRBCostsReceivable:
									tempREITTotal.TaxExUKPRBIntAndFCsReceivable = tempREITTotal.TaxExUKPRBIntAndFCsReceivable + adjustment.AdjustmentAmount;
									break;

								case TaxExemptAdjustmentNames.PRBCostsPayable:
									tempREITTotal.TaxExUKPRBIntAndFCsPayable = tempREITTotal.TaxExUKPRBIntAndFCsPayable + adjustment.AdjustmentAmount;
									break;

								case TaxExemptAdjustmentNames.PRBFairMovementOfHedgingDerivatives:
									tempREITTotal.TaxExUKPRBHedgingDerivatives = tempREITTotal.TaxExUKPRBHedgingDerivatives + adjustment.AdjustmentAmount;
									break;

								case TaxExemptAdjustmentNames.ResidualIncome:
									tempREITTotal.TaxExUKPRBResidualIncome = tempREITTotal.TaxExUKPRBResidualIncome + adjustment.AdjustmentAmount;
									break;

								case TaxExemptAdjustmentNames.ResidualExpense:
									tempREITTotal.TaxExUKPRBResidualExpenses = tempREITTotal.TaxExUKPRBResidualExpenses + adjustment.AdjustmentAmount;
									break;

								default:
									break;
							}
						}
						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.TaxExemptAdjustmentsToExcludeOtherTaxDisallowableExpenditureInAccountingPBT))
						{
							List<string> taxExemptAdjustmentToExcludeDisallowableExpenditureOptions = BaseEnumExtension.GetEnumDescriptions<TaxExemptAdjustmentsToExcludeNames>();

							if (taxExemptAdjustmentToExcludeDisallowableExpenditureOptions.Contains(adjustment.AdjustmentName) == true)
								tempREITTotal.TaxExUKPBTAdjustments = tempREITTotal.TaxExUKPBTAdjustments + adjustment.AdjustmentAmount;
						}

						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.TaxExemptTaxExemptProfits))
						{
							switch (BaseEnumExtension.GetEnumFromString<TaxExemptAdjustmentNames>(adjustment.AdjustmentName))
							{
								case TaxExemptAdjustmentNames.CostsReceivable:
									tempREITTotal.TaxExUKIntAndFCsReceivable = tempREITTotal.TaxExUKIntAndFCsReceivable + adjustment.AdjustmentAmount;
									break;

								case TaxExemptAdjustmentNames.CostsPayable:
									tempREITTotal.TaxExUKIntAndFCsPayable = tempREITTotal.TaxExUKIntAndFCsPayable + adjustment.AdjustmentAmount;
									break;

								case TaxExemptAdjustmentNames.FairMovementOfHedgingDerivatives:
									tempREITTotal.TaxExUKHedgingDerivatives = tempREITTotal.TaxExUKHedgingDerivatives + adjustment.AdjustmentAmount;
									break;

								case TaxExemptAdjustmentNames.CapitalAllowancesInFull:
									tempREITTotal.TaxExUKCapitalAllowances = tempREITTotal.TaxExUKCapitalAllowances + adjustment.AdjustmentAmount;
									break;

								case TaxExemptAdjustmentNames.PropertyLossesBroughtForward:
									tempREITTotal.TaxExUKUKPropertyBroughtFwd = tempREITTotal.TaxExUKUKPropertyBroughtFwd + adjustment.AdjustmentAmount;
									break;

								default:
									break;
							}
						}
						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.TaxExemptOtherTaxAdjustments))
						{
							List<string> taxExemptOtherAdjustmentOptions = BaseEnumExtension.GetEnumDescriptions<TaxExemptOtherAdjustmentNames>();

							if (taxExemptOtherAdjustmentOptions.Contains(adjustment.AdjustmentName) == true)
								tempREITTotal.TaxExUKOtherTaxAdjustments = tempREITTotal.TaxExUKOtherTaxAdjustments + adjustment.AdjustmentAmount;
						}
					}

					#endregion UK-TaxExmpt
				}

				tempREITTotal.TaxExUKREITSInvProfits = tempREITTotal.PropUKPIDs;

				tempREITTotal.TaxExUKPRBFinanceCosts = tempREITTotal.TaxExUKPRBIntAndFCsReceivable + tempREITTotal.TaxExUKPRBIntAndFCsPayable +
													   tempREITTotal.TaxExUKPRBHedgingDerivatives;

				tempREITTotal.TaxExUKUKPRBProfits = tempREITTotal.TaxExUKPRBProfitsBeforeTax + tempREITTotal.TaxExUKPRBFinanceCosts +
													tempREITTotal.TaxExUKPRBResidualIncome + tempREITTotal.TaxExUKPRBResidualExpenses +
													tempREITTotal.TaxExUKPBTAdjustments;

				tempREITTotal.TaxExUKOtherClaims = tempREITTotal.TaxExUKCapitalAllowances + tempREITTotal.TaxExUKOtherTaxAdjustments;

				tempREITTotal.TaxExUKProfitsExREITSInvProfits = tempREITTotal.TaxExUKUKPRBProfits + tempREITTotal.TaxExUKIntAndFCsPayable +
																tempREITTotal.TaxExUKIntAndFCsReceivable + tempREITTotal.TaxExUKHedgingDerivatives +
																tempREITTotal.TaxExUKOtherClaims + tempREITTotal.TaxExUKUKPropertyBroughtFwd;
			}

			foreach (Entity entity in nonUKEntities)
			{
				foreach (Adjustment adjustment in entity.Adjustments)
				{
					#region NonUK-Property

					if (adjustment.AdjustmentCategory.Trim() == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentCategories.PropertyRentalBusiness))
					{
						//IAS - Profits/Expenses

						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.PropertyRentalBusinessIASProfitsExpenses))
						{
							switch (BaseEnumExtension.GetEnumFromString<PropertyAdjustmentNames>(adjustment.AdjustmentName))
							{
								case PropertyAdjustmentNames.RevenueLessCostOfSales:
									tempREITTotal.PropNonUKRevenueLessCostOfSales = tempREITTotal.PropNonUKRevenueLessCostOfSales + adjustment.AdjustmentAmount;
									break;

								case PropertyAdjustmentNames.PIDsFromOtherUKREITs:
									tempREITTotal.PropNonUKPIDs = tempREITTotal.PropNonUKPIDs + adjustment.AdjustmentAmount;
									break;

								default:
									break;
							}
						}

						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.PropertyRentalBusinessOtherIncomeOrExpense))
						{
							//Other Income or Expense
							List<string> otherIncomeOrExpenseOptions = BaseEnumExtension.GetEnumDescriptions<PropertyOtherIncomeOrExpenseNames>();

							if (otherIncomeOrExpenseOptions.Contains(adjustment.AdjustmentName) == true)
								tempREITTotal.PropNonUKOtherIncomeOrExpenseAmount = tempREITTotal.PropNonUKOtherIncomeOrExpenseAmount + adjustment.AdjustmentAmount;
						}

						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.PropertyRentalBusinessNetFinanceCosts))
						{
							if (adjustment.AdjustmentName == BaseEnumExtension.GetDescriptionFromEnum(PropertyAdjustmentNames.NetFinanceCostsExternal))
								tempREITTotal.PropNonUKNetFinanceCosts = tempREITTotal.PropNonUKNetFinanceCosts + adjustment.AdjustmentAmount;
						}

						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.PropertyRentalBusinessOtherAdjustments))
						{
							//Other Adjustments
							List<string> otherAdjustmentOptions = BaseEnumExtension.GetEnumDescriptions<PropertyOtherAdjustmentNames>();
							if (otherAdjustmentOptions.Contains(adjustment.AdjustmentName) == true)
								tempREITTotal.PropNonUKOtherAdjustmentsAmount = tempREITTotal.PropNonUKOtherAdjustmentsAmount + adjustment.AdjustmentAmount;
						}

						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.PropertyRentalBusinessIASAssets))
						{
							switch (BaseEnumExtension.GetEnumFromString<PropertyAdjustmentNames>(adjustment.AdjustmentName))
							{
								case PropertyAdjustmentNames.NonCurrentAssets:
									tempREITTotal.PropNonUKNonCurrentAssets = tempREITTotal.PropNonUKNonCurrentAssets + adjustment.AdjustmentAmount;
									break;

								case PropertyAdjustmentNames.CurrentAssets:
									tempREITTotal.PropNonUKCurrentAssets = tempREITTotal.PropNonUKCurrentAssets + adjustment.AdjustmentAmount;
									break;

								default:
									break;
							}
						}
					}

					#endregion NonUK-Property

					#region NonUK-Residual

					if (adjustment.AdjustmentCategory.Trim() == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentCategories.ResidualIncome))
					{
						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.ResidualIncomeIASProfitsExpenses))
						{
							switch (BaseEnumExtension.GetEnumFromString<ResidualAdjustmentNames>(adjustment.AdjustmentName))
							{
								case ResidualAdjustmentNames.RevenueLessCostOfSales:
									tempREITTotal.ResNonUKRevenueLessCostOfSales = tempREITTotal.ResNonUKRevenueLessCostOfSales + adjustment.AdjustmentAmount;
									break;

								case ResidualAdjustmentNames.BeneficialInterestsIncome:
									tempREITTotal.ResNonUKBeneficialInterestsIncome = tempREITTotal.ResNonUKBeneficialInterestsIncome + adjustment.AdjustmentAmount;
									break;

								default:
									break;
							}
						}

						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.ResidualIncomeOtherIncomeOrExpense))
						{
							//Other Income or Expense
							List<string> otherIncomeOrExpenseOptions = BaseEnumExtension.GetEnumDescriptions<ResidualOtherIncomeOrExpenseNames>();

							if (otherIncomeOrExpenseOptions.Contains(adjustment.AdjustmentName) == true)
								tempREITTotal.ResNonUKOtherIncomeOrExpenseAmount = tempREITTotal.ResNonUKOtherIncomeOrExpenseAmount + adjustment.AdjustmentAmount;
						}

						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.ResidualIncomeNetFinanceCosts))
						{
							if (adjustment.AdjustmentName == BaseEnumExtension.GetDescriptionFromEnum(ResidualAdjustmentNames.NetFinanceCostsResidual))
								tempREITTotal.ResNonUKNetFinanceCosts = tempREITTotal.ResNonUKNetFinanceCosts + adjustment.AdjustmentAmount;
						}

						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.ResidualIncomeOtherAdjustments))
						{
							//Other Adjustments
							List<string> otherAdjustmentOptions = BaseEnumExtension.GetEnumDescriptions<ResidualOtherAdjustments>();
							if (otherAdjustmentOptions.Contains(adjustment.AdjustmentName) == true)
								tempREITTotal.ResNonUKOtherAdjustmentsAmount = tempREITTotal.ResNonUKOtherAdjustmentsAmount + adjustment.AdjustmentAmount;
						}
						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.ResidualIncome_IAS_Assets))
						{
							switch (BaseEnumExtension.GetEnumFromString<ResidualAdjustmentNames>(adjustment.AdjustmentName))
							{
								case ResidualAdjustmentNames.NonCurrentAssets:
									tempREITTotal.ResNonUKNonCurrentAssets = tempREITTotal.ResNonUKNonCurrentAssets + adjustment.AdjustmentAmount;
									break;

								case ResidualAdjustmentNames.CurrentAssets:
									tempREITTotal.ResNonUKCurrentAssets = tempREITTotal.ResNonUKCurrentAssets + adjustment.AdjustmentAmount;
									break;

								default:
									break;
							}
						}
					}

					#endregion NonUK-Residual

					#region NonUK-TaxExempt

					if (adjustment.AdjustmentCategory.Trim() == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentCategories.TaxExempt))
					{
						//PRB Profits Before Tax
						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.TaxExemptUKPRBProfits))
						{
							switch (BaseEnumExtension.GetEnumFromString<TaxExemptAdjustmentNames>(adjustment.AdjustmentName))
							{
								case TaxExemptAdjustmentNames.PRBProfitsBeforeTax:
									tempREITTotal.TaxExNonUKPRBProfitsBeforeTax = tempREITTotal.TaxExNonUKPRBProfitsBeforeTax + adjustment.AdjustmentAmount;
									break;

								case TaxExemptAdjustmentNames.PRBCostsReceivable:
									tempREITTotal.TaxExNonUKPRBIntAndFCsReceivable = tempREITTotal.TaxExNonUKPRBIntAndFCsReceivable + adjustment.AdjustmentAmount;
									break;

								case TaxExemptAdjustmentNames.PRBCostsPayable:
									tempREITTotal.TaxExNonUKPRBIntAndFCsPayable = tempREITTotal.TaxExNonUKPRBIntAndFCsPayable + adjustment.AdjustmentAmount;
									break;

								case TaxExemptAdjustmentNames.PRBFairMovementOfHedgingDerivatives:
									tempREITTotal.TaxExNonUKPRBHedgingDerivatives = tempREITTotal.TaxExNonUKPRBHedgingDerivatives + adjustment.AdjustmentAmount;
									break;

								case TaxExemptAdjustmentNames.ResidualIncome:
									tempREITTotal.TaxExNonUKPRBResidualIncome = tempREITTotal.TaxExNonUKPRBResidualIncome + adjustment.AdjustmentAmount;
									break;

								case TaxExemptAdjustmentNames.ResidualExpense:
									tempREITTotal.TaxExNonUKPRBResidualExpenses = tempREITTotal.TaxExNonUKPRBResidualExpenses + adjustment.AdjustmentAmount;
									break;

								default:
									break;
							}
						}
						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.TaxExemptAdjustmentsToExcludeOtherTaxDisallowableExpenditureInAccountingPBT))
						{
							List<string> taxExemptAdjustmentToExcludeDisallowableExpenditureOptions = BaseEnumExtension.GetEnumDescriptions<TaxExemptAdjustmentsToExcludeNames>();

							if (taxExemptAdjustmentToExcludeDisallowableExpenditureOptions.Contains(adjustment.AdjustmentName) == true)
								tempREITTotal.TaxExNonUKPBTAdjustments = tempREITTotal.TaxExNonUKPBTAdjustments + adjustment.AdjustmentAmount;
						}

						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.TaxExemptTaxExemptProfits))
						{
							switch (BaseEnumExtension.GetEnumFromString<TaxExemptAdjustmentNames>(adjustment.AdjustmentName))
							{
								case TaxExemptAdjustmentNames.CostsReceivable:
									tempREITTotal.TaxExNonUKIntAndFCsReceivable = tempREITTotal.TaxExNonUKIntAndFCsReceivable + adjustment.AdjustmentAmount;
									break;

								case TaxExemptAdjustmentNames.CostsPayable:
									tempREITTotal.TaxExNonUKIntAndFCsPayable = tempREITTotal.TaxExNonUKIntAndFCsPayable + adjustment.AdjustmentAmount;
									break;

								case TaxExemptAdjustmentNames.FairMovementOfHedgingDerivatives:
									tempREITTotal.TaxExNonUKHedgingDerivatives = tempREITTotal.TaxExNonUKHedgingDerivatives + adjustment.AdjustmentAmount;
									break;

								case TaxExemptAdjustmentNames.CapitalAllowancesInFull:
									tempREITTotal.TaxExNonUKCapitalAllowances = tempREITTotal.TaxExNonUKCapitalAllowances + adjustment.AdjustmentAmount;
									break;

								case TaxExemptAdjustmentNames.PropertyLossesBroughtForward:
									tempREITTotal.TaxExNonUKUKPropertyBroughtFwd = tempREITTotal.TaxExNonUKUKPropertyBroughtFwd + adjustment.AdjustmentAmount;
									break;

								default:
									break;
							}
						}

						if (adjustment.AdjustmentType == BaseEnumExtension.GetDescriptionFromEnum(AdjustmentTypes.TaxExemptOtherTaxAdjustments))
						{
							List<string> taxExemptOtherAdjustmentOptions = BaseEnumExtension.GetEnumDescriptions<TaxExemptOtherAdjustmentNames>();

							if (taxExemptOtherAdjustmentOptions.Contains(adjustment.AdjustmentName) == true)
								tempREITTotal.TaxExNonUKOtherTaxAdjustments = tempREITTotal.TaxExNonUKOtherTaxAdjustments + adjustment.AdjustmentAmount;
						}
					}

					#endregion NonUK-TaxExempt

					tempREITTotal.TaxExNonUKREITSInvProfits = tempREITTotal.PropNonUKPIDs;
				}

				tempREITTotal.TaxExNonUKPRBFinanceCosts = tempREITTotal.TaxExNonUKPRBIntAndFCsReceivable + tempREITTotal.TaxExNonUKPRBIntAndFCsPayable +
														  tempREITTotal.TaxExNonUKPRBHedgingDerivatives;

				tempREITTotal.TaxExNonUKUKPRBProfits = tempREITTotal.TaxExNonUKPRBProfitsBeforeTax + tempREITTotal.TaxExNonUKPRBFinanceCosts +
													   tempREITTotal.TaxExNonUKPRBResidualIncome + tempREITTotal.TaxExNonUKPRBResidualExpenses +
													   tempREITTotal.TaxExNonUKPBTAdjustments;

				tempREITTotal.TaxExNonUKOtherClaims = tempREITTotal.TaxExNonUKCapitalAllowances + tempREITTotal.TaxExNonUKOtherTaxAdjustments;

				tempREITTotal.TaxExNonUKProfitsExREITSInvProfits = tempREITTotal.TaxExNonUKUKPRBProfits + tempREITTotal.TaxExNonUKIntAndFCsPayable +
																   tempREITTotal.TaxExNonUKIntAndFCsReceivable + tempREITTotal.TaxExNonUKHedgingDerivatives +
																   tempREITTotal.TaxExNonUKOtherClaims + tempREITTotal.TaxExNonUKUKPropertyBroughtFwd;
			}

			//Combined Property Data
			tempREITTotal.PropCombinedRevenueLessCostOfSales = tempREITTotal.PropUKRevenueLessCostOfSales + tempREITTotal.PropNonUKRevenueLessCostOfSales;
			tempREITTotal.PropCombinedPIDs = tempREITTotal.PropUKPIDs + tempREITTotal.PropNonUKPIDs;
			tempREITTotal.PropCombinedOtherIncomeOrExpenseAmount = tempREITTotal.PropUKOtherIncomeOrExpenseAmount + tempREITTotal.PropNonUKOtherIncomeOrExpenseAmount;
			tempREITTotal.PropCombinedNetFinanceCosts = tempREITTotal.PropUKNetFinanceCosts + tempREITTotal.PropNonUKNetFinanceCosts;
			tempREITTotal.PropCombinedOtherAdjustmentsAmount = tempREITTotal.PropUKOtherAdjustmentsAmount + tempREITTotal.PropNonUKOtherAdjustmentsAmount;
			tempREITTotal.PropCombinedNonCurrentAssets = tempREITTotal.PropUKNonCurrentAssets + tempREITTotal.PropNonUKNonCurrentAssets;
			tempREITTotal.PropCombinedCurrentAssets = tempREITTotal.PropUKCurrentAssets + tempREITTotal.PropNonUKCurrentAssets;

			//Combined Residual Data
			tempREITTotal.ResCombinedRevenueLessCostOfSales = tempREITTotal.ResUKRevenueLessCostOfSales + tempREITTotal.ResNonUKRevenueLessCostOfSales;
			tempREITTotal.ResCombinedBeneficialInterestsIncome = tempREITTotal.ResUKBeneficialInterestsIncome + tempREITTotal.ResNonUKBeneficialInterestsIncome;
			tempREITTotal.ResCombinedOtherIncomeOrExpenseAmount = tempREITTotal.ResUKOtherIncomeOrExpenseAmount + tempREITTotal.ResNonUKOtherIncomeOrExpenseAmount;
			tempREITTotal.ResCombinedNetFinanceCosts = tempREITTotal.ResUKNetFinanceCosts + tempREITTotal.ResNonUKNetFinanceCosts;

			tempREITTotal.ResCombinedOtherAdjustmentsAmount = tempREITTotal.ResUKOtherAdjustmentsAmount + tempREITTotal.ResNonUKOtherAdjustmentsAmount;
			tempREITTotal.ResCombinedNonCurrentAssets = tempREITTotal.ResUKNonCurrentAssets + tempREITTotal.ResNonUKNonCurrentAssets;
			tempREITTotal.ResCombinedCurrentAssets = tempREITTotal.ResUKCurrentAssets + tempREITTotal.ResNonUKCurrentAssets;

			//Combined Tax Exempt Data
			tempREITTotal.TaxExCombinedPRBProfitsBeforeTax = tempREITTotal.TaxExUKPRBProfitsBeforeTax + tempREITTotal.TaxExNonUKPRBProfitsBeforeTax;
			tempREITTotal.TaxExCombinedPRBIntAndFCsReceivable = tempREITTotal.TaxExUKPRBIntAndFCsReceivable + tempREITTotal.TaxExNonUKPRBIntAndFCsReceivable;
			tempREITTotal.TaxExCombinedPRBIntAndFCsPayable = tempREITTotal.TaxExUKPRBIntAndFCsPayable + tempREITTotal.TaxExNonUKPRBIntAndFCsPayable;
			tempREITTotal.TaxExCombinedPRBHedgingDerivatives = tempREITTotal.TaxExUKPRBHedgingDerivatives + tempREITTotal.TaxExNonUKPRBHedgingDerivatives;
			tempREITTotal.TaxExCombinedPRBFinanceCosts = tempREITTotal.TaxExCombinedPRBIntAndFCsReceivable + tempREITTotal.TaxExCombinedPRBIntAndFCsPayable + tempREITTotal.TaxExCombinedPRBHedgingDerivatives;

			tempREITTotal.TaxExCombinedPRBResidualIncome = tempREITTotal.TaxExUKPRBResidualIncome + tempREITTotal.TaxExNonUKPRBResidualIncome;
			tempREITTotal.TaxExCombinedPRBResidualExpenses = tempREITTotal.TaxExUKPRBResidualExpenses + tempREITTotal.TaxExNonUKPRBResidualExpenses;

			tempREITTotal.TaxExCombinedPBTAdjustments = tempREITTotal.TaxExUKPBTAdjustments + tempREITTotal.TaxExNonUKPBTAdjustments;
			tempREITTotal.TaxExCombinedUKPRBProfits = tempREITTotal.TaxExUKUKPRBProfits + tempREITTotal.TaxExNonUKUKPRBProfits;
			tempREITTotal.TaxExCombinedIntAndFCsReceivable = tempREITTotal.TaxExUKIntAndFCsReceivable + tempREITTotal.TaxExNonUKIntAndFCsReceivable;
			tempREITTotal.TaxExCombinedIntAndFCsPayable = tempREITTotal.TaxExUKIntAndFCsPayable + tempREITTotal.TaxExNonUKIntAndFCsPayable;
			tempREITTotal.TaxExCombinedHedgingDerivatives = tempREITTotal.TaxExUKHedgingDerivatives + tempREITTotal.TaxExNonUKHedgingDerivatives;
			tempREITTotal.TaxExCombinedCapitalAllowances = tempREITTotal.TaxExUKCapitalAllowances + tempREITTotal.TaxExNonUKCapitalAllowances;
			tempREITTotal.TaxExCombinedOtherTaxAdjustments = tempREITTotal.TaxExUKOtherTaxAdjustments + tempREITTotal.TaxExNonUKOtherTaxAdjustments;
			tempREITTotal.TaxExCombinedUKPropertyBroughtFwd = tempREITTotal.TaxExUKUKPropertyBroughtFwd + tempREITTotal.TaxExNonUKUKPropertyBroughtFwd;
			tempREITTotal.TaxExCombinedOtherClaims = tempREITTotal.TaxExCombinedCapitalAllowances + tempREITTotal.TaxExCombinedOtherTaxAdjustments;
			tempREITTotal.TaxExCombinedProfitsExREITSInvProfits = tempREITTotal.TaxExUKProfitsExREITSInvProfits + tempREITTotal.TaxExNonUKProfitsExREITSInvProfits;
			tempREITTotal.TaxExCombinedREITSInvProfits = tempREITTotal.TaxExUKREITSInvProfits + tempREITTotal.TaxExNonUKREITSInvProfits;

			// tests

			bool resTestGtrZero = (tempREITTotal.ResCombinedOtherAdjustmentsAmount
								+ tempREITTotal.ResCombinedNetFinanceCosts
								+ tempREITTotal.ResCombinedRevenueLessCostOfSales
								+ tempREITTotal.ResCombinedBeneficialInterestsIncome
								+ tempREITTotal.ResCombinedOtherIncomeOrExpenseAmount) > 0;

			bool prbTestGtrZero = (tempREITTotal.PropCombinedOtherAdjustmentsAmount
									+ tempREITTotal.PropCombinedNetFinanceCosts
									+ tempREITTotal.PropCombinedRevenueLessCostOfSales
									+ tempREITTotal.PropCombinedPIDs
									+ tempREITTotal.PropCombinedOtherIncomeOrExpenseAmount) > 0;

			if (resTestGtrZero == false)
				tempREITTotal.IncomeTestPercentage = 100.00;
			else if (prbTestGtrZero == false)
				tempREITTotal.IncomeTestPercentage = 0.00;
			else
				tempREITTotal.IncomeTestPercentage = Math.Round(((tempREITTotal.PropCombinedOtherAdjustmentsAmount
														+ tempREITTotal.PropCombinedNetFinanceCosts
														+ tempREITTotal.PropCombinedRevenueLessCostOfSales
														+ tempREITTotal.PropCombinedPIDs
														+ tempREITTotal.PropCombinedOtherIncomeOrExpenseAmount)
														/
														(tempREITTotal.PropCombinedOtherAdjustmentsAmount
														+ tempREITTotal.PropCombinedNetFinanceCosts
														+ tempREITTotal.PropCombinedRevenueLessCostOfSales
														+ tempREITTotal.PropCombinedPIDs
														+ tempREITTotal.PropCombinedOtherIncomeOrExpenseAmount
														+ tempREITTotal.ResCombinedOtherAdjustmentsAmount
														+ tempREITTotal.ResCombinedNetFinanceCosts
														+ tempREITTotal.ResCombinedRevenueLessCostOfSales
														+ tempREITTotal.ResCombinedBeneficialInterestsIncome
														+ tempREITTotal.ResCombinedOtherIncomeOrExpenseAmount)) * 100, 2, MidpointRounding.ToEven);

			//tempREITTotal.IncomeTestResult = (tempREITTotal.IncomeTestPercentage >= 75) ? "Pass" : "Fail";
			//added by Pawel
			if (tempREITTotal.IncomeTestPercentage >= 80)
            {
				tempREITTotal.IncomeTestResult = "Pass";
			}
            else if (tempREITTotal.IncomeTestPercentage >= 75 && tempREITTotal.IncomeTestPercentage < 80)
            {
				tempREITTotal.IncomeTestResult = "Gateway";
			}
            else
            {
				tempREITTotal.IncomeTestResult = "Fail";
			}


				bool resAssTestGtrZero = (tempREITTotal.ResCombinedCurrentAssets
										+ tempREITTotal.ResCombinedNonCurrentAssets) > 0;

			bool prbAssTestGtrZero = (tempREITTotal.PropCombinedNonCurrentAssets
										+ tempREITTotal.PropCombinedCurrentAssets) > 0;

			if (resAssTestGtrZero == false)
				tempREITTotal.AssetTestPercentage = 100.00;
			else if (prbAssTestGtrZero == false)
				tempREITTotal.AssetTestPercentage = 0.00;
			else
				tempREITTotal.AssetTestPercentage = Math.Round(((tempREITTotal.PropCombinedNonCurrentAssets
													+ tempREITTotal.PropCombinedCurrentAssets)
													/
													((tempREITTotal.PropCombinedNonCurrentAssets
													+ tempREITTotal.PropCombinedCurrentAssets)
													+ (tempREITTotal.ResCombinedCurrentAssets
													+ tempREITTotal.ResCombinedNonCurrentAssets))
													) * 100, 2, MidpointRounding.ToEven);

			//tempREITTotal.AssetTestResult = (tempREITTotal.AssetTestPercentage >= 75) ? "Pass" : "Fail";
			//added by Pawel
			if (tempREITTotal.AssetTestPercentage >= 80)
			{
				tempREITTotal.AssetTestResult = "Pass";
			}
			else if (tempREITTotal.AssetTestPercentage >= 75 && tempREITTotal.IncomeTestPercentage < 80)
			{
				tempREITTotal.AssetTestResult = "Gateway";
			}
			else
			{
				tempREITTotal.AssetTestResult = "Fail";
			}

			tempREITTotal.InterestCoverRatioTestPercentage = (tempREITTotal.TaxExCombinedUKPRBProfits > 0 && tempREITTotal.PropCombinedNetFinanceCosts < 0) ? tempREITTotal.TaxExCombinedUKPRBProfits / -tempREITTotal.PropCombinedNetFinanceCosts : 0;

			tempREITTotal.InterestCoverRatioTestResult = (tempREITTotal.InterestCoverRatioTestPercentage >= 1.25 || tempREITTotal.PropCombinedNetFinanceCosts == 0) ? "Pass" : "Fail";

			// PID Dist figures/results
			tempREITTotal.PIDDistribution90Amount = (tempREITTotal.TaxExCombinedProfitsExREITSInvProfits > 0) ? (tempREITTotal.TaxExCombinedProfitsExREITSInvProfits * 90) / 100 : 0;

			tempREITTotal.PIDDistribution100Amount = (tempREITTotal.TaxExCombinedREITSInvProfits > 0) ? tempREITTotal.TaxExCombinedREITSInvProfits : 0;
			tempREITTotal.PaidDividendScheduleConfirmed = "No";

			if (importedREIT.Reconciliations.Count > 0)
			{
				var PBTReconciliations = new List<Reconciliation>();
				var AssetReconciliations = new List<Reconciliation>();

				PBTReconciliations = importedREIT.Reconciliations.Where(a => a.ReconciliationType == BaseEnumExtension.GetDescriptionFromEnum(SummaryReconciliationTypes.PBTReconciliation)).ToList();
				AssetReconciliations = importedREIT.Reconciliations.Where(a => a.ReconciliationType == BaseEnumExtension.GetDescriptionFromEnum(SummaryReconciliationTypes.AssetReconciliation)).ToList();

				foreach (Reconciliation reconciliation in PBTReconciliations)
					tempREITTotal.PBTReconsToAuditedFinancialStatement = tempREITTotal.PBTReconsToAuditedFinancialStatement + reconciliation.ReconciliationAmount;

				foreach (Reconciliation reconciliation in AssetReconciliations)
					tempREITTotal.ReconsToAuditedFinancialStatement = tempREITTotal.ReconsToAuditedFinancialStatement + reconciliation.ReconciliationAmount;
			}

			tempREITTotal.REITId = importedREIT.Id;

			return tempREITTotal;
		}

		private bool SaveImportedRecords(REIT importedReit)
		{
			var success = false;

			try
			{
                importedReit.CreatedBy = System.Environment.UserName;
                //importedReit.CreatedBy = "7209233";
                importedReit.DateCreated = DateTime.Now;

				success = _dataService.SaveREIT(importedReit);
			}
			catch { }

			return success;
		}

		private string GetXSDFilePath()
		{
			var xsdFileName = string.Empty;

			//var appPath = @"\\c\s\caf1\BDApp Installation Files\111099\XSD";
			var appPath = @"C:\BDApp Installation Files\111099\XSD";

			try
			{
				xsdFileName = ConfigurationManager.AppSettings["xmlValidationFile"].ToString();
			}
			catch { throw new Exception("No [xmlValidationFile] setting"); }

			if (Debugger.IsAttached)
				appPath = @"C:\LaunchLocations\REITS\XSD";

			return Path.Combine(appPath, xsdFileName);
		}

		// VALIDATIONS

		public bool ValidateXML(XDocument xmlSource, XmlSchemaSet schemaSet)
		{
			Errors.Clear();

			if (schemaSet == null)
				throw new ArgumentNullException("In ValidateAgainstSchema: No schema loaded.");

			var settings = new XmlReaderSettings();
			settings.CloseInput = true;
			settings.ValidationType = ValidationType.Schema;
			settings.ValidationEventHandler += new ValidationEventHandler(HandleValidationError);
			settings.Schemas.Add(schemaSet);
			settings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings | XmlSchemaValidationFlags.ProcessSchemaLocation;

			using (StringReader srStringReader = new StringReader(xmlSource.ToString()))
			{
				using (XmlReader validatingReader = XmlReader.Create(srStringReader, settings))
				{
					while (validatingReader.Read())
					{ }
				}
			}

			return Errors.Count == 0;
		}

		private void HandleValidationError(object sender, ValidationEventArgs e)
		{
			var ex = (e.Exception as XmlSchemaValidationException);

			var entityNameHelp = EntityNameForLineInfo(ex.LineNumber);

			var errorMessage = $"Line: {ex.LineNumber} - {entityNameHelp} - {e.Message} - {e.Severity}";

			Errors.Add(errorMessage);

			Debug.Print(errorMessage);
		}

		private string EntityNameForLineInfo(int ln)
		{
			var result = string.Empty;

			if (currentXDoc == null)
				return string.Empty;

			var query =
				from ele in currentXDoc.Descendants()
				let lineInfo = (IXmlLineInfo)ele
				where lineInfo.LineNumber == ln
				select ele;

			if (query != null)
				result = GetEntityNameForParentOf(query.FirstOrDefault());

			return result;
		}

		private string GetEntityNameForParentOf(XElement ele)
		{
			var result = string.Empty;

			if (ele == null)
				return result;

			var parent = ele.Parent;

			if (parent.Name.LocalName == "Entity")
				result = GetNameFor(parent);

			if (parent.Name.LocalName == "EntityAdjustment")
			{
				parent = parent.Parent.Parent;

				result = GetNameFor(parent);
			}

			return result;
		}

		private string GetNameFor(XElement ele)
		{
			var entityName = ele.Descendants()
									   .Where(x => x.Name.LocalName.Contains("EntityName"))
									   .Select(p => p.Value).ToList();

			return string.Join(Environment.NewLine, entityName.ToArray());
		}

		private XmlSchemaSet GetSchemaSpec()
		{
			var defaultSchemaPath = GetXSDFilePath();  // TO REDO Method

			if (CompiledSchemaSetCached == null)
			{
				var reitSchemaSet = new XmlSchemaSet();
				reitSchemaSet.Add(null, defaultSchemaPath);
				reitSchemaSet.Compile();

				CompiledSchemaSetCached = reitSchemaSet;
			}

			return CompiledSchemaSetCached;
		}

		private string GetValidationErrorMessagesAsString()
		{
			var errorMessage = "No Validation Error";

			if (Errors?.Count > 0)
				errorMessage = string.Join(Environment.NewLine, Errors);

			return errorMessage;
		}

		// XML FORMATS
		private XmlDocument ToXmlDocument(XDocument xDocument)
		{
			var xmlDocument = new XmlDocument();

			using (var xmlReader = xDocument.CreateReader())
			{
				xmlDocument.Load(xmlReader);
			}

			return xmlDocument;
		}

		private XDocument ReplaceNameSpace(XDocument xDoc)
		{
			var originalXML = xDoc.ToString().Replace("xmlns:reit", "xmlns");

			return XDocument.Parse(originalXML);
		}

		#endregion Private Methods

		#region Disposable

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Debug.Print("DISPOSAL: Disposed ImportFunctions");
			}
		}

		#endregion Disposable
	}

	public static class DocumentExtensions
	{
		public static XmlDocument ToXmlDocument(this XDocument xDocument)
		{
			var xmlDocument = new XmlDocument();
			using (var xmlReader = xDocument.CreateReader())
			{
				xmlDocument.Load(xmlReader);
			}
			return xmlDocument;
		}

		public static XDocument ToXDocument(this XmlDocument xmlDocument)
		{
			using (var nodeReader = new XmlNodeReader(xmlDocument))
			{
				nodeReader.MoveToContent();
				return XDocument.Load(nodeReader);
			}
		}
	}
}