using System;
using System.ComponentModel;

namespace REITs.Domain.Models
{
    public class REITParentReviewRFS : BaseEntity, INotifyPropertyChanged
    {
        public Guid ParentId { get; set; }

        // KEY DATES
        public DateTime RFSAPEYear { get; set; }

        public DateTime? FSReviewedAPEDate { get; set; }

        public string RiskStatus { get; set; }

        public string OnBRRTT { get; set; }
        public DateTime? InternalBRRDueDate { get; set; }

        public DateTime? RFSReviewedDate { get; set; }

        public DateTime? RAPlanMeetDate { get; set; }

        public DateTime? ReviewedDate { get; set; }

        public DateTime? NextReviewDate { get; set; }

        //Comments
        public string Comments { get; set; }

        public REITParentReviewRFS()
        { }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}