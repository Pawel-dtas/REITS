using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REITs.Domain.Models
{
    public class SystemAdmin
    {
        public Guid Id { get; set; }
        public string AdminType { get; set; }
        public string Description { get; set; }
        public string Position { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

    }
}
