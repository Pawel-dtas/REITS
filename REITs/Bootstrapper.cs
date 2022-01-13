using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Unity;
using REITs.AppInfoModule;
using REITs.DataLayer.Services;
using REITs.ImportModule;
using REITs.MenuModule;
using REITs.REITDisplayModule;
using REITs.REITParentDisplayModule;
using REITs.ReportsModule;
using REITs.SearchModule;
using REITs.UserModule;
using REITs.Views;
using System.Windows;

namespace REITs
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            //System.Windows.Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            var catalog = (ModuleCatalog)ModuleCatalog;
            catalog.AddModule(typeof(AppInfo));
            catalog.AddModule(typeof(Menu));
            catalog.AddModule(typeof(REITDisplay));
            catalog.AddModule(typeof(REITParentDisplay));
            catalog.AddModule(typeof(Search));
            catalog.AddModule(typeof(Users));
            catalog.AddModule(typeof(Import));
            catalog.AddModule(typeof(Reports));
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            ViewModelLocationProvider.SetDefaultViewModelFactory(type => Container.Resolve(type));

            Container.RegisterType(typeof(object), typeof(Shell), "Shell");

            Container.RegisterType<IREITDataService, REITDataService>();
            Container.RegisterType<IUserDataService, UserDataService>();
            Container.RegisterType<ISearchService, SearchService>();
            Container.RegisterType<IReportService, ReportService>();

            System.Windows.Application.Current.Resources.Add("IoC", this.Container);
        }
    }
}