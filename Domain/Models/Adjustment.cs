using System;

namespace REITs.Domain.Models
{
    public class Adjustment
    {
        public Guid Id { get; set; }
        public string AdjustmentCategory { get; set; }
        public string AdjustmentType { get; set; }
        public string AdjustmentName { get; set; }
        public double AdjustmentAmount { get; set; }

        public Guid EntityId { get; set; } //Foreign Key
        public virtual Entity Entity { get; set; } //It's parent Entity
    }
}