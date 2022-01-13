using Domain.Enums;
using Domain.Models;

namespace REITs.DataLayer.Services
{
	internal class ReportServiceSQLCommands : IReportServiceSQLCommands
	{
		public string Submissions(ReportCriteria rc)
		{
			string tempSql = string.Format(@"SELECT PAR.[PrincipalCustomerName] AS [Principal Customer Name]
											  ,PAR.[PrincipalUTR] AS [Principal UTR]
											  ,PAR.[TaxExemptUTR] AS [Tax Exempt UTR]
											  ,PAR.[ConversionDate] AS [Conversion]
											  ,PAR.[LeftRegime] AS [Ceased]
											  ,FORMAT(PAR.[APEDate], 'd, MMMM') AS [Principal APE]
											  ,REIT.[AccountPeriodEnd] AS [REIT APE]
											  ,REIT.[XMLVersion] AS [Version]
											  ,FS.[PIDRecDate] AS [PID Received]
											  ,REIT.[XMLDateSubmitted] AS [XML Submitted]

												FROM [REITS_111099].[REIT].[REITParents] PAR

												LEFT OUTER JOIN REITS_111099.[REIT].[REITs] REIT ON REIT.ParentId = PAR.Id
												AND REIT.[AccountPeriodEnd] >= '{1}' AND REIT.[AccountPeriodEnd] <= '{2}'

												LEFT JOIN [REITS_111099].[REIT].[REITParentReviewFS] FS ON FS.ParentId = REIT.ParentId
												AND FS.[FSDue] = REIT.[AccountPeriodEnd]

												WHERE MONTH(PAR.[APEDate]) >= DATEPART(MONTH, '{1}') AND MONTH(PAR.[APEDate]) <= DATEPART(MONTH, '{2}')
												AND REIT.[ParentId] IN ({0})
												AND REIT.XMLVersion = (SELECT MAX(XMLVersion) FROM [REITS_111099].[REIT].[REITs] RTV WHERE REIT.[ParentId] = RTV.[ParentId])

												ORDER BY PAR.[PrincipalCustomerName]
														,PAR.[PrincipalUTR]
														,REIT.[XMLVersion] DESC
														,REIT.[XMLDateSubmitted]", rc.ChosenCompanyGuids, rc.StartDate, rc.EndDate);

			if (string.IsNullOrEmpty(rc.ChosenCompanyGuids))
				tempSql = tempSql.Replace("AND REIT.[ParentId] IN ()", "");

			if (rc.Version == VersionTypes.All.ToString())
				tempSql = tempSql.Replace("AND REIT.XMLVersion = (SELECT MAX(XMLVersion) FROM [REITS_111099].[REIT].[REITs] RTV WHERE REIT.[ParentId] = RTV.[ParentId])", "");

			return tempSql;
		}

		public string ExpectedSubmissions(ReportCriteria rc)
		{
			string tempSql = string.Format(@"SELECT DISTINCT PAR.[PrincipalCustomerName] AS [Principal Customer Name]
											  ,PAR.[PrincipalUTR] AS [Principal UTR]
											  ,PAR.[TaxExemptUTR] AS [Tax Exempt UTR]
											  ,PAR.[ConversionDate] AS [Conversion]
											  ,PAR.[LeftRegime] AS [Ceased]
											  ,FORMAT(PAR.[APEDate], 'd, MMMM') AS [Principal APE]
											  ,REIT.[AccountPeriodEnd] AS [REIT APE]
											  ,(SELECT MAX(XMLVersion) FROM [REITS_111099].[REIT].[REITs] RTV WHERE REIT.[ParentId] = RTV.[ParentId]) AS [Version]
											  ,FS.[PIDRecDate] AS [PID Received]
											  ,REIT.[XMLDateSubmitted] AS [XML Submitted]

												FROM [REITS_111099].[REIT].[REITParents] PAR

												LEFT JOIN REITS_111099.[REIT].[REITs] REIT ON REIT.ParentId = PAR.Id
												AND REIT.[AccountPeriodEnd] >= '{1}' AND REIT.[AccountPeriodEnd] <= '{2}'

												LEFT JOIN [REITS_111099].[REIT].[REITParentReviewFS] FS ON FS.ParentId = REIT.ParentId
												AND FS.[FSDue] = REIT.[AccountPeriodEnd]

												WHERE MONTH(PAR.[APEDate]) >= DATEPART(MONTH, '{1}') AND MONTH(PAR.[APEDate]) <= DATEPART(MONTH, '{2}')
												AND REIT.[ParentId] IN ({0})

												ORDER BY PAR.[PrincipalCustomerName]
														,PAR.[PrincipalUTR]
														,REIT.[XMLDateSubmitted]", rc.ChosenCompanyGuids, rc.StartDate, rc.EndDate);

			if (string.IsNullOrEmpty(rc.ChosenCompanyGuids))
				tempSql = tempSql.Replace("AND REIT.[ParentId] IN ()", "");

			if (rc.Version == VersionTypes.All.ToString())
				tempSql = tempSql.Replace("AND REIT.XMLVersion = (SELECT MAX(XMLVersion) FROM [REITS_111099].[REIT].[REITs] RTV WHERE REIT.[ParentId] = RTV.[ParentId])", "");

			return tempSql;
		}

		public string BRRSchedule(ReportCriteria rc)
		{
			string tempSql = string.Format(@"SELECT DISTINCT PAR.[PrincipalCustomerName] AS [Principal Customer Name]
											  ,PAR.[PrincipalUTR] AS [Principal UTR]
											  ,PAR.[TaxExemptUTR] AS [Tax Exempt UTR]

											  ,FORMAT(PAR.[APEDate], 'd, MMMM') AS [Principal APE]
												,[NextBRRDate] AS [Next BRR Date]

												FROM [REITS_111099].[REIT].[REITParents] PAR

												WHERE MONTH(PAR.[NextBRRDate]) >= DATEPART(MONTH, '{1}') AND MONTH(PAR.[NextBRRDate]) <= DATEPART(MONTH, '{2}')
												AND YEAR(PAR.[NextBRRDate]) >= DATEPART(YEAR, '{1}') AND YEAR(PAR.[NextBRRDate]) <= DATEPART(YEAR, '{2}')

												AND PAR.[Id] IN ({0})

												ORDER BY PAR.[NextBRRDate]
														,PAR.[PrincipalCustomerName]
														,PAR.[PrincipalUTR]", rc.ChosenCompanyGuids, rc.StartDate, rc.EndDate);

			if (string.IsNullOrEmpty(rc.ChosenCompanyGuids))
				tempSql = tempSql.Replace("AND PAR.[Id] IN ()", "");

			return tempSql;
		}

		public string AnalysisOfAdjustments(ReportCriteria rc)
		{
			string tempSql = string.Format(@"SELECT REIT.[REITName] AS [REIT Name]
											,REIT.[PrincipalUTR] AS [Principal UTR]
											,ENT.[EntityName] As [Entity Name], ENT.[EntityUTR] AS [Entity UTR]
											,ADJ.[AdjustmentCategory] AS [Adjustment Category]
											,LTRIM(REPLACE(ADJ.[AdjustmentType], ADJ.[AdjustmentCategory], '')) AS [Adjustment Type]
											,ADJ.[AdjustmentName] AS [Adjustment Name]
											,SUM(ADJ.[AdjustmentAmount]) AS [Amount]
											,REIT.[AccountPeriodEnd] AS [REIT APE]
											,REIT.[XMLVersion] AS [Version]
											,REIT.[XMLDateSubmitted] AS [XML Submitted]

											  FROM [REITS_111099].[REIT].[Adjustments] ADJ

											  LEFT JOIN [REITS_111099].[REIT].[Entities] ENT ON ENT.Id = ADJ.EntityId
											  LEFT JOIN [REITS_111099].[REIT].[REITs] REIT ON REIT.id = ENT.REITId

											  WHERE REIT.[AccountPeriodEnd] BETWEEN '{1}' AND '{2}'
											  AND REIT.[ParentId] IN ({0})
											  AND AdjustmentCategory LIKE '%{3}'
											  AND AdjustmentType LIKE '{3}%{4}'
											  AND AdjustmentName LIKE '{5}%'
											  AND REIT.XMLVersion = (SELECT MAX(XMLVersion) FROM [REITS_111099].[REIT].[REITs] RTV WHERE REIT.[ParentId] = RTV.[ParentId])

											  GROUP BY
													REIT.[REITName]
													,REIT.[PrincipalUTR]
													,REIT.[AccountPeriodEnd]
													,REIT.[XMLVersion]
													,REIT.[XMLDateSubmitted]
													,ENT.[EntityName], ENT.[EntityUTR]
													,ADJ.[AdjustmentCategory]
													,ADJ.[AdjustmentType]
													,ADJ.[AdjustmentName]

											ORDER BY REIT.REITName ASC
													,ENT.[EntityName], ENT.[EntityUTR]
													,ADJ.[AdjustmentCategory]
													,ADJ.[AdjustmentType]
													,ADJ.[AdjustmentName]
													,[Amount] DESC
													,REIT.[AccountPeriodEnd] DESC
													,REIT.[XMLVersion] DESC
													,REIT.[XMLDateSubmitted]", rc.ChosenCompanyGuids, rc.StartDate, rc.EndDate, rc.AdjustmentCategory, rc.AnalysisType, rc.AnalysisName);

			if (string.IsNullOrEmpty(rc.ChosenCompanyGuids))
				tempSql = tempSql.Replace("AND REIT.[ParentId] IN ()", "");

			if (!rc.EntityLevel)
			{
				tempSql = tempSql.Replace(",ENT.[EntityName] As [Entity Name], ENT.[EntityUTR] AS [Entity UTR]", "");
				tempSql = tempSql.Replace(",ENT.[EntityName], ENT.[EntityUTR]", "");
			}

			if (rc.Version == VersionTypes.All.ToString())
				tempSql = tempSql.Replace("AND REIT.XMLVersion = (SELECT MAX(XMLVersion) FROM [REITS_111099].[REIT].[REITs] RTV WHERE REIT.[ParentId] = RTV.[ParentId])", "");

			return tempSql;
		}

		public string AnalysisOfReconciliations(ReportCriteria rc)
		{
			string tempSql = string.Format(@"SELECT REIT.[REITName] AS [REIT Name]
											,REIT.[PrincipalUTR] AS [Principal UTR]
											,REC.[ReconciliationType] AS [Rec Type]
											,REC.[ReconciliationName] AS [Rec Name]
											,SUM(REC.[ReconciliationAmount]) AS [Amount]
											,REIT.[AccountPeriodEnd] AS [REIT APE]
											,REIT.[XMLVersion] AS [Version]
											,REIT.[XMLDateSubmitted] AS [XML Submitted]
											  FROM [REITS_111099].[REIT].[Reconciliations] REC

											  LEFT JOIN [REITS_111099].[REIT].[REITs] REIT ON REIT.id = REC.REITId

											  WHERE REIT.[AccountPeriodEnd] BETWEEN '{1}' AND '{2}'
											  AND REIT.[ParentId] IN ({0})
											  AND ReconciliationType LIKE '%{3}'
											  AND ReconciliationName LIKE '{4}%'

											  AND REIT.XMLVersion = (SELECT MAX(XMLVersion) FROM [REITS_111099].[REIT].[REITs] RTV WHERE REIT.[ParentId] = RTV.[ParentId])

											  GROUP BY
													REIT.[REITName]
													,REIT.[PrincipalUTR]
													,REIT.[AccountPeriodEnd]
													,REIT.[XMLVersion]
													,REIT.[XMLDateSubmitted]
													,REC.[ReconciliationType]
													,REC.[ReconciliationName]

											ORDER BY REIT.REITName ASC
													,Amount DESC
													,REIT.[AccountPeriodEnd] DESC
													,REIT.[XMLVersion] DESC
													,REIT.[XMLDateSubmitted]", rc.ChosenCompanyGuids, rc.StartDate, rc.EndDate, rc.AnalysisType, rc.AnalysisName);

			if (string.IsNullOrEmpty(rc.ChosenCompanyGuids))
				tempSql = tempSql.Replace("AND REIT.ParentId IN ()", "");

			if (rc.Version == VersionTypes.All.ToString())
				tempSql = tempSql.Replace("AND REIT.XMLVersion = (SELECT MAX(XMLVersion) FROM [REITS_111099].[REIT].[REITs] RTV WHERE REIT.[ParentId] = RTV.[ParentId])", "");

			return tempSql;
		}

		public string Notes(ReportCriteria rc)
		{
			string tempSql = string.Format(@"SELECT REIT.[REITName] As [REIT Name]
							,REIT.[PrincipalUTR] AS [Principal UTR]
							,REIT.[AccountPeriodEnd] AS [REIT APE]
							,REIT.REITNotes AS [Notes]
							,REIT.[XMLVersion] AS [Version]
							,REIT.[XMLDateSubmitted] AS [XML Submitted]

							FROM [REITS_111099].[REIT].[REITParents] PAR

							LEFT OUTER JOIN REITS_111099.[REIT].[REITs] REIT ON REIT.ParentId = PAR.Id
							AND REIT.[AccountPeriodEnd] >= '{1}' AND REIT.[AccountPeriodEnd] <= '{2}'

							WHERE MONTH(PAR.[APEDate]) >= DATEPART(MONTH, '{1}') AND MONTH(PAR.[APEDate]) <= DATEPART(MONTH, '{2}')
							AND REIT.[ParentId] IN ({0})
							AND REIT.XMLVersion = (SELECT MAX(XMLVersion) FROM [REITS_111099].[REIT].[REITs] RTV WHERE REIT.[ParentId] = RTV.[ParentId])

							AND LOWER(REIT.[REITNotes]) LIKE LOWER('%{3}%')

							ORDER BY REIT.[REITName]
									,REIT.[PrincipalUTR]
									,REIT.[XMLVersion] DESC
									,REIT.[XMLDateSubmitted]", rc.ChosenCompanyGuids, rc.StartDate, rc.EndDate, rc.NoteSearchWord);

			if (string.IsNullOrEmpty(rc.ChosenCompanyGuids))
				tempSql = tempSql.Replace("AND REIT.[ParentId] IN ()", "");

			if (rc.Version == VersionTypes.All.ToString())
				tempSql = tempSql.Replace("AND REIT.XMLVersion = (SELECT MAX(XMLVersion) FROM [REITS_111099].[REIT].[REITs] RTV WHERE REIT.[ParentId] = RTV.[ParentId])", "");

			return tempSql;
		}

		public string Tests(ReportCriteria rc)      // TODO: DJC
		{
			string tempSql = string.Format(@"SELECT PAR.[PrincipalCustomerName] as [Principal Customer Name]
											  ,PAR.[PrincipalUTR] as [P UTR]
											  ,FORMAT(PAR.[APEDate], 'd, MMM') as [P APE]

											  ,FORMAT(REIT.[AccountPeriodEnd], 'd, MMM, yyyy') as [REIT APE]
											  ,REIT.[XMLVersion] as [Vx]

												,TOT.PropCombinedRevenueLessCostOfSales
														+ TOT.PropCombinedPIDs
														+ TOT.PropCombinedOtherIncomeOrExpenseAmount
														+ TOT.PropCombinedNetFinanceCosts
														+ TOT.PropCombinedOtherAdjustmentsAmount as [PRB Profit]

												,TOT.ResCombinedRevenueLessCostOfSales
														+ TOT.ResCombinedBeneficialInterestsIncome
														+ TOT.ResCombinedOtherIncomeOrExpenseAmount
														+ TOT.ResCombinedNetFinanceCosts
														+ TOT.ResCombinedOtherAdjustmentsAmount as [RES Profit]

												,ROUND(TOT.[IncomeTestPercentage], 2) as [Income %]
												,TOT.[IncomeTestResult] as [I TR]

												,TOT.PropCombinedNonCurrentAssets
														+ TOT.PropCombinedCurrentAssets as [PRB Assets]

												,TOT.ResCombinedNonCurrentAssets
														+ TOT.ResCombinedCurrentAssets as [RES Assets]

												,ROUND(TOT.AssetTestPercentage, 2) as [Asset %]
												,TOT.AssetTestResult as [A TR]

												,TOT.[TaxExCombinedUKPRBProfits] as [TE Profits]
												,TOT.[PropCombinedNetFinanceCosts] as [PRB Fin Costs]

												,ROUND(TOT.InterestCoverRatioTestPercentage, 2) as [Int Cov %]
												,TOT.InterestCoverRatioTestResult as [IC TR]

												,CASE WHEN TOT.InterestCoverRatioTestResult = 'Fail'
													THEN CONVERT(nvarchar, (-1.25 * TOT.[PropCombinedNetFinanceCosts]) - TOT.[TaxExCombinedUKPRBProfits])
													ELSE 'n/a'
												END as [S543 Addtl Profits]

												FROM [REITS_111099].[REIT].[REITParents] PAR

												LEFT OUTER JOIN REITS_111099.[REIT].[REITs] REIT ON REIT.ParentId = PAR.Id
												AND REIT.[AccountPeriodEnd] >= '{1}' AND REIT.[AccountPeriodEnd] <= '{2}'

												LEFT JOIN [REITS_111099].[REIT].[REITTotals] TOT ON TOT.REITId = REIT.Id

												WHERE MONTH(PAR.[APEDate]) >= DATEPART(MONTH, '{1}') AND MONTH(PAR.[APEDate]) <= DATEPART(MONTH, '{2}')
												AND REIT.[ParentId] IN ({0})
												AND REIT.XMLVersion = (SELECT MAX(XMLVersion) FROM [REITS_111099].[REIT].[REITs] RTV WHERE REIT.[ParentId] = RTV.[ParentId])

												ORDER BY PAR.[PrincipalCustomerName]
														,PAR.[PrincipalUTR]
														,REIT.[XMLVersion] DESC
														,REIT.[XMLDateSubmitted]", rc.ChosenCompanyGuids, rc.StartDate, rc.EndDate);

			if (string.IsNullOrEmpty(rc.ChosenCompanyGuids))
				tempSql = tempSql.Replace("AND REIT.[ParentId] IN ()", "");

			if (rc.Version == VersionTypes.All.ToString())
				tempSql = tempSql.Replace("AND REIT.XMLVersion = (SELECT MAX(XMLVersion) FROM [REITS_111099].[REIT].[REITs] RTV WHERE REIT.[ParentId] = RTV.[ParentId])", "");

			return tempSql;
		}

		public string Summary(ReportCriteria rc)    // TODO: DJC
		{
			string tempSql = string.Format(@"WITH [TotalsData] AS
												(
													SELECT

													RP.[PrincipalCustomerName] as [REIT Parent],
													RT.[AccountPeriodEnd] as [APE],
													RT.[XMLVersion] as [Vn],

													TOT.PropUKCurrentAssets, TOT.PropNonUKCurrentAssets, TOT.PropCombinedCurrentAssets,
													TOT.PropUKNonCurrentAssets, TOT.PropNonUKNonCurrentAssets, TOT.PropCombinedNonCurrentAssets,
													TOT.PropUKNetFinanceCosts, TOT.PropNonUKNetFinanceCosts, TOT.PropCombinedNetFinanceCosts,
													TOT.PropUKOtherAdjustmentsAmount, TOT.PropNonUKOtherAdjustmentsAmount, TOT.PropCombinedOtherAdjustmentsAmount,
													TOT.PropUKOtherIncomeOrExpenseAmount, TOT.PropNonUKOtherIncomeOrExpenseAmount, TOT.PropCombinedOtherIncomeOrExpenseAmount,
													TOT.PropUKRevenueLessCostOfSales, TOT.PropNonUKRevenueLessCostOfSales, TOT.PropCombinedRevenueLessCostOfSales,
													TOT.PropUKPIDs, TOT.PropNonUKPIDs, TOT.PropCombinedPIDs,
													TOT.ResUKCurrentAssets, TOT.ResNonUKCurrentAssets, TOT.ResCombinedCurrentAssets,
													TOT.ResUKNonCurrentAssets, TOT.ResNonUKNonCurrentAssets, TOT.ResCombinedNonCurrentAssets,
													TOT.ResUKNetFinanceCosts, TOT.ResNonUKNetFinanceCosts, TOT.ResCombinedNetFinanceCosts,
													TOT.ResUKOtherAdjustmentsAmount, TOT.ResNonUKOtherAdjustmentsAmount, TOT.ResCombinedOtherAdjustmentsAmount,
													TOT.ResUKOtherIncomeOrExpenseAmount, TOT.ResNonUKOtherIncomeOrExpenseAmount, TOT.ResCombinedOtherIncomeOrExpenseAmount,
													TOT.ResUKRevenueLessCostOfSales, TOT.ResNonUKRevenueLessCostOfSales, TOT.ResCombinedRevenueLessCostOfSales,
													TOT.ResUKBeneficialInterestsIncome, TOT.ResNonUKBeneficialInterestsIncome, TOT.ResCombinedBeneficialInterestsIncome,
													TOT.TaxExUKUKPRBProfits, TOT.TaxExNonUKUKPRBProfits, TOT.TaxExCombinedUKPRBProfits,
													TOT.TaxExUKProfitsExREITSInvProfits, TOT.TaxExNonUKProfitsExREITSInvProfits, TOT.TaxExCombinedProfitsExREITSInvProfits,
													TOT.TaxExUKREITSInvProfits, TOT.TaxExNonUKREITSInvProfits, TOT.TaxExCombinedREITSInvProfits,
													TOT.TaxExUKUKPropertyBroughtFwd, TOT.TaxExNonUKUKPropertyBroughtFwd, TOT.TaxExCombinedUKPropertyBroughtFwd

													FROM [REITS_111099].[REIT].[REITParents] as RP
													LEFT JOIN [REITS_111099].[REIT].[REITs] as RT on RT.ParentId = RP.Id
													LEFT JOIN [REITS_111099].[REIT].[REITTotals] as TOT on TOT.REITId = RT.Id

													WHERE AccountPeriodEnd IS NOT NULL
													AND RT.[AccountPeriodEnd] >= '{1}' AND RT.[AccountPeriodEnd] <= '{2}'
													AND MONTH(RP.[APEDate]) >= DATEPART(MONTH, '{1}') AND MONTH(RP.[APEDate]) <= DATEPART(MONTH, '{2}')

													AND RT.[ParentId] IN ({0})
													AND RT.XMLVersion = (SELECT MAX(XMLVersion) FROM [REITS_111099].[REIT].[REITs] RTV WHERE RT.[ParentId] = RTV.[ParentId])
												)

			SELECT [REIT Parent], [APE], [Vn] ,RESULTS.[Type], RESULTS.[SubType], RESULTS.[UK], RESULTS.[NonUK], RESULTS.[Combined]
			FROM [TotalsData]

			CROSS APPLY
			(
				VALUES

					('Property', '1a) Current Assets', PropUKCurrentAssets, PropNonUKCurrentAssets, PropCombinedCurrentAssets),
					('Property', '1b) NonCurrent Assets', PropUKNonCurrentAssets, PropNonUKNonCurrentAssets, PropCombinedNonCurrentAssets),
					('Property', '1c)                                                                Assets Subtotal', PropUKNonCurrentAssets
																														+ PropUKCurrentAssets, PropNonUKNonCurrentAssets
																																				+ PropNonUKCurrentAssets, PropCombinedNonCurrentAssets
																																											+ PropCombinedCurrentAssets),
					('Property + Residual', '1d)                                                        Group Assets Total', PropUKCurrentAssets
																															+ PropUKNonCurrentAssets
																															+ ResUKCurrentAssets
																															+ ResUKNonCurrentAssets,
																																					PropNonUKCurrentAssets
																																					+ PropNonUKNonCurrentAssets
																																					+ ResNonUKCurrentAssets
																																					+ ResNonUKNonCurrentAssets,
																																												PropCombinedCurrentAssets
																																												+ PropCombinedNonCurrentAssets
																																												+ ResCombinedCurrentAssets
																																												+ ResCombinedNonCurrentAssets),
					('Property', '2) Net Finance Costs', PropUKNetFinanceCosts,PropNonUKNetFinanceCosts,PropCombinedNetFinanceCosts),
					('Property', '3) Other Adjustments Amount', PropUKOtherAdjustmentsAmount,PropNonUKOtherAdjustmentsAmount,PropCombinedOtherAdjustmentsAmount),
					('Property', '4) Other Income Or Expense Amount', PropUKOtherIncomeOrExpenseAmount,PropNonUKOtherIncomeOrExpenseAmount,PropCombinedOtherIncomeOrExpenseAmount),
					('Property', '5) Revenue Less CostOfSales', PropUKRevenueLessCostOfSales,PropNonUKRevenueLessCostOfSales,PropCombinedRevenueLessCostOfSales),
					('Property', '6) PIDs', PropUKPIDs,PropNonUKPIDs,PropCombinedPIDs),

					('Residual', '1a) Current Assets', ResUKCurrentAssets, ResNonUKCurrentAssets, ResCombinedCurrentAssets),
					('Residual', '1b) NonCurrent Assets', ResUKNonCurrentAssets, ResNonUKNonCurrentAssets, ResCombinedNonCurrentAssets),

					('Residual', '1c)                                                                Assets Subtotal', ResUKCurrentAssets
																														+ ResUKNonCurrentAssets, ResNonUKCurrentAssets
																																				+ ResNonUKNonCurrentAssets, ResCombinedCurrentAssets
																																											+ ResCombinedNonCurrentAssets),
					('Residual + Property', '1d)                                                        Group Assets Total', PropUKCurrentAssets
																															+ PropUKNonCurrentAssets
																															+ ResUKCurrentAssets
																															+ ResUKNonCurrentAssets,
																																					PropNonUKCurrentAssets
																																					+ PropNonUKNonCurrentAssets
																																					+ ResNonUKCurrentAssets
																																					+ ResNonUKNonCurrentAssets,
																																												PropCombinedCurrentAssets
																																												+ PropCombinedNonCurrentAssets
																																												+ ResCombinedCurrentAssets
																																												+ ResCombinedNonCurrentAssets),
					('Residual', '2) Net Finance Costs', ResUKNetFinanceCosts, ResNonUKNetFinanceCosts, ResCombinedNetFinanceCosts),
					('Residual', '3) Other Adjustments Amount', ResUKOtherAdjustmentsAmount, ResNonUKOtherAdjustmentsAmount, ResCombinedOtherAdjustmentsAmount),
					('Residual', '4) Other Income Or Expense Amount', ResUKOtherIncomeOrExpenseAmount, ResNonUKOtherIncomeOrExpenseAmount, ResCombinedOtherIncomeOrExpenseAmount),
					('Residual', '5) Revenue Less Cost Of Sales', ResUKRevenueLessCostOfSales, ResNonUKRevenueLessCostOfSales, ResCombinedRevenueLessCostOfSales),
					('Residual', '6) Beneficial Interests Income', ResUKBeneficialInterestsIncome, ResNonUKBeneficialInterestsIncome, ResCombinedBeneficialInterestsIncome),

					('TaxEx', 'a) UK PRB Profits', TaxExUKUKPRBProfits, TaxExNonUKUKPRBProfits, TaxExCombinedUKPRBProfits),
					('TaxEx', 'b) Profits ExREITS Inv Profits', TaxExUKProfitsExREITSInvProfits, TaxExNonUKProfitsExREITSInvProfits, TaxExCombinedProfitsExREITSInvProfits),
					('TaxEx', 'c) REITS Inv Profits', TaxExUKREITSInvProfits, TaxExNonUKREITSInvProfits, TaxExCombinedREITSInvProfits),
					('TaxEx', 'd) UK Property BroughtFwd', TaxExUKUKPropertyBroughtFwd, TaxExNonUKUKPropertyBroughtFwd, TaxExCombinedUKPropertyBroughtFwd)

			) RESULTS ([Type], [SubType], [UK], [NonUK], [Combined])

			ORDER BY [REIT Parent] ASC, [APE] DESC, [Type] ASC, [SubType] ASC", rc.ChosenCompanyGuids, rc.StartDate, rc.EndDate);

			if (string.IsNullOrEmpty(rc.ChosenCompanyGuids))
				tempSql = tempSql.Replace("AND RT.[ParentId] IN ()", "");

			if (rc.Version == VersionTypes.All.ToString())
				tempSql = tempSql.Replace("AND RT.XMLVersion = (SELECT MAX(XMLVersion) FROM [REITS_111099].[REIT].[REITs] RTV WHERE RT.[ParentId] = RTV.[ParentId])", string.Empty);

			return tempSql;
		}
	}
}