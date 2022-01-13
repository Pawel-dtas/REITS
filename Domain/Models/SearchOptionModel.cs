using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REITs.Domain.Models
{
    public class SearchOptionModel
    {       
        public string SearchType { get; set; }
        public string SearchName { get; set; }       
        public string SearchUTR { get; set; }
        public string SearchCustomerReference { get; set; }

        public DateTime? SearchAPEFrom { get; set; }
        public DateTime? SearchAPETo { get; set; }
        public DateTime? SearchPAPEFrom { get; set; }
        public DateTime? SearchPAPETo { get; set; }

    }
}
