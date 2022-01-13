using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REITs.Domain.Models
{
    public class AdjustmentType
    {
        public Guid Id { get; set; }
        public string AdjustmentCategory { get; set; }
        public string AdjustmentName { get; set; }
    }
}
