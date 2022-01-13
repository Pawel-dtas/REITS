using System;
using System.ComponentModel;

namespace REITs.Domain.Models
{
    public class REITParentReviewFS : BaseEntity, INotifyPropertyChanged
    {
        public Guid ParentId { get; set; }

        public DateTime FSAPEYear { get; set; }

        // KEY DATES
        public DateTime? FSDue { get; set; }

        public DateTime? FSRecDate { get; set; }

        //Docs Recieved
        public DateTime? PIDDueDate { get; set; }

        public DateTime? PIDRecDate { get; set; }

        //Comments
        public string Comments { get; set; }

        public REITParentReviewFS()
        { }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}