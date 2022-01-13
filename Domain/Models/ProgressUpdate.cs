using System.Windows;

namespace REITs.Domain.Models
{
    public class ProgressUpdate
    {
        public Visibility ProgressBarVisibility { get; set; }

        public double ProgressBarValue { get; set; }

        public string CurrentFile { get; set; }

        public string CurrentFileToDisplay { get; set; }
    }
}