using System;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

public static class XMLExtensions
{
	// GET XML ELE VALUES
	public static string GetValue(this XElement xmlPath)
	{
		var returnValue = string.Empty;

		try
		{
			if (xmlPath != null)
				returnValue = xmlPath?.Value?.Trim();
		}
		catch { }

		return returnValue;
	}

	public static string GetValue(this XmlNode xmlNode)
	{
		var returnValue = "0";

		try
		{
			if (xmlNode != null)
				if (xmlNode.InnerText.Length > 0)
					returnValue = xmlNode?.InnerText?.Trim();
		}
		catch { }

		return returnValue;
	}

	public static double GetValueAsDouble(this XmlNode xmlNode)
	{
		double returnValue = 0.0;

		var value = GetValue(xmlNode);

		returnValue = Convert.ToDouble(value);

		return returnValue;
	}

	public static bool GetValueAsBoolean(this XmlNode xmlNode)
	{
		bool returnValue = false;

		var value = GetValue(xmlNode);

		returnValue = Convert.ToBoolean(value);

		return returnValue;
	}

	public static DateTime? GetValueAsDateTime(this XElement xmlPath)
	{
		DateTime? tempDate = null;

		try
		{
			tempDate = DateTime.ParseExact(string.Format("{0} 00:00:00", xmlPath.Value), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
		}
		catch { }

		return tempDate;
	}

	public static string LoadAndFormatXML(string fileName)
	{
		var xDoc = XDocument.Load(fileName, LoadOptions.SetLineInfo);

		var xml = xDoc.ToString();

		var stringBuilder = new StringBuilder();
		var element = XElement.Parse(xml);

		var settings = new XmlWriterSettings();
		settings.OmitXmlDeclaration = true;
		settings.Indent = true;
		settings.NewLineOnAttributes = true;

		using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
		{
			element.Save(xmlWriter);
		}

		return stringBuilder.ToString();
	}
}