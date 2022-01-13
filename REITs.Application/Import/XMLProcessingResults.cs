using REITs.Domain.Enums;
using System;
using System.Windows.Media;

namespace REITs.Application
{
	public class XMLProcessingResult
	{
		public string FileName { get; set; }

		public string ValidationMessage { get; set; }

		public ImportXMLStatusTypes XMLStatus { get; set; }

		public ImportCompanyStatusTypes ImportCompanyStatus { get; set; }

		public int Version { get; set; }

		public Guid ParentCompanyGuid { get; set; }

		public SolidColorBrush BackgroundColor { get; set; }

		public XMLProcessingResult()
		{
			ValidationMessage = "...";
			XMLStatus = ImportXMLStatusTypes.Ready;
			BackgroundColor = new SolidColorBrush(Colors.LightYellow);
		}
	}
}