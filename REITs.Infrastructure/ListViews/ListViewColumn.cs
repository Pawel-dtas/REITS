using System.Collections.Generic;

namespace REITs.Infrastructure
{
	public class ColumnConfig
	{
		public IEnumerable<Column> Columns { get; set; }
	}

	public class Column
	{
		public string RecordId { get; set; }
		public string Header { get; set; }
		public string DataField { get; set; }
		public int Width { get; set; }
		public bool IsDate { get; set; }
		public bool IsAPE { get; set; }

		public bool IsYear { get; set; }

		public bool IsMonthYear { get; set; }

		public bool IsDateTime { get; set; }

		public bool IsWrapped { get; set; }

		public bool IsEnumFlag { get; set; }
	}
}