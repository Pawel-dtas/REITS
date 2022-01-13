using Domain.Models;
using System;
using System.Collections.Generic;

namespace REITs.Domain.Models
{
    public class REIT
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string REITName { get; set; }
        public string PrincipalUTR { get; set; }
        public DateTime AccountPeriodEnd { get; set; }
        public DateTime PreviousAccountPeriodEnd { get; set; }
        public string REITNotes { get; set; }
        public DateTime XMLDateSubmitted { get; set; }
        public int XMLVersion { get; set; }

        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual List<Entity> Entities { get; set; } //It's collection of Entities

        public virtual List<Reconciliation> Reconciliations { get; set; } //It's collection of Entities

        public virtual List<REITTotals> REITTotal { get; set; }

        public REIT()
        {
            Entities = new List<Entity>();
            Reconciliations = new List<Reconciliation>();
            REITTotal = new List<REITTotals>();
        }
    }
}