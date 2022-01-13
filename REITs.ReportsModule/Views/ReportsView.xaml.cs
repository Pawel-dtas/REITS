using REITs.Infrastructure.CustomAttributes;
using System.Windows.Controls;

namespace REITs.ReportsModule.Views
{
    [ViewName("ReportsView")]
    public partial class ReportsView : UserControl
    {
        public ReportsView()
        {
            InitializeComponent();
        }

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd/MM/yyyy";
        }
    }
}