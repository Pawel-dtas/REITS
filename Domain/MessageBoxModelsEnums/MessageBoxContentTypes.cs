using System.ComponentModel;

namespace Domain.MessageBoxModelsEnums
{
	public enum MessageBoxContentTypes
	{
		[Description("Xml Export Successful")]
		XMLExportSuccessful,

		[Description("Xml Export Unsuccessful")]
		XMLExportUnsuccessful,

		[Description("Save Successful")]
		SaveSuccessful,

		[Description("Save Unsuccessful")]
		SaveUnsuccessful,

		[Description("Save User Successful")]
		SaveUserSuccessful,

		[Description("Save User Unsuccessful")]
		SaveUserUnsuccessful,

		[Description("Edit User Successful")]
		EditUserSuccessful,

		[Description("Edit User Unsuccessful")]
		EditUserUnsuccessful,

		[Description("Import Successful")]
		ImportSuccessful,

		[Description("Import Unsuccessful")]
		ImportUnsuccessful,

		[Description("Delete Confirmation")]
		DeleteConfirmation,

		[Description("Delete Successful")]
		DeleteSuccessful,

		[Description("Delete User Confirmation")]
		DeleteUserConfirmation,

		[Description("Delete User Successful")]
		DeleteUserSuccessful,

		[Description("Delete User Unsuccessful")]
		DeleteUserUnsuccessful,

		[Description("Delete Unsuccessful")]
		DeleteUnsuccessful,

		[Description("Export Successful")]
		ExportSuccessful,

		[Description("Export to CSV?")]
		CopyToClipboard,

		[Description("Export Unsuccessful")]
		ExportUnsuccessful,

		[Description("D1 Change Confirmation")]
		D1_ChangeConfirmation,

		[Description("A2 Change Confirmation")]
		A2_ChangeConfirmation,

		[Description("New Ruling")]
		NewRuling,

		[Description("New Ruling")]
		XMLCreationError,

		[Description("Name already exists")]
		NameAlreadyExists,

		[Description("Reference already exists")]
		ReferenceAlreadyExists,

		[Description("Address already exists")]
		AddressAlreadyExists,

		[Description("Business Activity already exists")]
		BusinessActivityAlreadyExists,

		[Description("Group Activity already exists")]
		GroupAlreadyExists,

		[Description("Country already exists")]
		CountryAlreadyExists,

		[Description("Method already exists")]
		MethodAlreadyExists,

		[Description("Summary already exists")]
		SummaryAlreadyExists,

		[Description("Criteria already exists")]
		CriteriaAlreadyExists,

		[Description("Arrangement Type already exists")]
		ArrangementTypeAlreadyExists,

		[Description("EU Ruling Number already exists")]
		EURulingNumberAlreadyExists,

		[Description("Case Reference already exists")]
		CaseReferenceAlreadyExists,

		[Description("This User already exists")]
		UserAlreadyExists,

		[Description("No results to display")]
		NoSearchResults,

		[Description("Help File Error")]
		HelpFileError,

		[Description("Validated records")]
		ValidatedRecords,

		[Description("Excel validation error")]
		ExcelValidationError,

		[Description("XML validation error")]
		XMLValidationError,

		[Description("Converted to XML")]
		RecordsConvertedToXML,

		[Description("Multiple Excel Files Selected")]
		MultipleExcelBulkFilesSelected,

		[Description("PDF Created Sucessfully")]
		PDFExportSuccessful,

		[Description("PDF Created not created")]
		PDFExportUnsuccessful,

		[Description("Print successful")]
		PrintSuccessful,

		[Description("print problem")]
		PrintUnsuccessful,

		[Description("Incompatible XML File")]
		IncompatibleXMLFileType,

		[Description("Invalid Netowrk Path")]
		InvalidNetworkPath,

		[Description("Search Options Reset Warning")]
		SearchOptionsResetWarning,

		[Description("No Customer(s) Selected")]
		NoCustomersSelected,

		[Description("Delete FS Reecord")]
		DeleteFSRecordCheck,

		[Description("Generic Success")]
		Success,

		[Description("Generic Failure")]
		Failure
	}
}