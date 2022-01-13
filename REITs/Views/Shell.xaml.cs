using REITs.AppInfoModule.Views;
using System.Windows;

namespace REITs.Views
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window
    {
        public Shell()
        {
            InitializeComponent();

            SplashView splashView = new SplashView();
            splashView.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool shouldCancel = true;

            //if (Debugger.IsAttached) return; // enable fast closing when debugging

            try
            {
                MessageBoxResult result = MessageBox.Show("You have chosen to close the entire application.\n\nThe application and any open windows will be closed.\n\nAre you sure you want to continue?",
                                                            "Close entire application?",
                                                            MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                if (result == MessageBoxResult.Yes)
                {
                    foreach (Window window in System.Windows.Application.Current.Windows)
                    {
                        //if (window.Name.StartsWith("REIT_"))
                        window.Close();
                    }

                    //App.Current.MainWindow.Close();

                    shouldCancel = false;
                }

                e.Cancel = shouldCancel;
            }
            catch { }
        }
    }
}