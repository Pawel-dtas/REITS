using System;
using System.Collections.Generic;

namespace REITs.Domain.Models
{
    public class Entity
    {
        public Guid Id { get; set; }
        public string EntityName { get; set; }
        public string EntityType { get; set; }
        public string EntityUTR { get; set; }
        public double InterestPercentage { get; set; }
        public string Jurisdiction { get; set; }
        public bool TaxExempt { get; set; }
        public string CustomerReference { get; set; }

        public virtual List<Adjustment> Adjustments { get; set; } //It's collection of Adjustments

        public Guid REITId { get; set; } //Foreign Key

        public virtual REIT REIT { get; set; } //It's parent REIT

        public Entity()
        {
            Adjustments = new List<Adjustment>();
        }
    }
}