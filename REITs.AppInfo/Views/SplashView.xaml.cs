using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace REITs.AppInfoModule.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SplashView : Window, ISplashScreen
    {
        private Thread loadingThread;
        private Storyboard Showboard;
        private Storyboard HideBoard;

        private delegate void ShowDelegate(string txt);

        private delegate void HideDelegate();

        private ShowDelegate showDelegate;
        private HideDelegate hideDelegate;

        public SplashView()
        {
            InitializeComponent();
            showDelegate = new ShowDelegate(this.ShowText);
            hideDelegate = new HideDelegate(this.HideText);

            Showboard = this.Resources["showStoryBoard"] as Storyboard;
            HideBoard = this.Resources["hideStoryBoard"] as Storyboard;
        }

        private void ShowText(string txt)
        {
            txtLoading.Text = txt;
            BeginStoryboard(Showboard);
        }

        private void HideText()
        {
            BeginStoryboard(HideBoard);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadingThread = new Thread(Load);
            loadingThread.Start();
        }

        private void Load()
        {
            Thread.Sleep(500);
            this.Dispatcher.Invoke(showDelegate, "Setting up environment");
            Thread.Sleep(2000);
            this.Dispatcher.Invoke(hideDelegate);

            Thread.Sleep(500);
            this.Dispatcher.Invoke(showDelegate, "Loading Data");
            Thread.Sleep(2000);
            this.Dispatcher.Invoke(hideDelegate);

            Thread.Sleep(500);
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate ()
            { Close(); });
        }

        public void AddMessage(string message)
        {
            //Dispatcher.Invoke((Action)delegate ()
            //{
            //    this.UpdateMessageTextBox.Content = message;
            //});
        }

        public void LoadCompleted()
        {
            Dispatcher.InvokeShutdown();
        }
    }

    public interface ISplashScreen
    {
        void LoadCompleted();
    }
}