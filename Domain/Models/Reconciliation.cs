using System;

namespace REITs.Domain.Models
{
    public class Reconciliation
    {
        public Guid Id { get; set; }
        public string ReconciliationType { get; set; }
        public string ReconciliationName { get; set; }
        public double ReconciliationAmount { get; set; }
        public Guid REITId { get; set; } //Foreign Key
        //public virtual REIT REIT { get; set; } //It's parent REIT
    }
}