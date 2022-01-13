using System;

namespace REITs.Domain.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DataExportable : Attribute
    {
        public bool Exportable { get; private set; }

        public DataExportable(bool exportable)
        {
            Exportable = exportable;
        }
    }
}
