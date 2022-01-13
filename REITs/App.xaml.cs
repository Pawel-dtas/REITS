using Domain;
using Domain.Models;
using Hmrc.BDApp.WorkFlow.FileStorage;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using Prism.Regions;
using REITs.DataLayer.Services;
using REITs.Domain.Enums;
using REITs.Infrastructure;
using REITs.Infrastructure.Events;
using System.Diagnostics;
using System.Windows;

namespace REITs
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : System.Windows.Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			if (VerifyInstance() == false)
			{
				System.Windows.MessageBox.Show("There is already an instance of the REITs Application open.", "INSTANCE: REITs", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				Process.GetCurrentProcess().Kill();
			}

			base.OnStartup(e);

			Bootstrapper bootstrapper = new Bootstrapper();
			bootstrapper.Run();

			if (GetUserSessionInfo() != true)
			{
				MessageBox.Show("You are not authorised to use this system.", "STARTUP: Not Authorised");
				MainWindow.Close();
			}
			else
            {

				RegisterMenus();

                StaticUserList.PopulateListOfSystemUsers();

                System.Windows.Application.Current.MainWindow.Show();

                StorageFunctionality.Instance.Setup();

            }
		}

		private static bool VerifyInstance()
		{
			return (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).GetUpperBound(0) > 0) ? false : true;
		}

		private bool GetUserSessionInfo()
		{
            var currentUser = PrismHelpers.ResolveService<IUserDataService>().GetSystemUser(System.Environment.UserName);
            //var currentUser = PrismHelpers.ResolveService<IUserDataService>().GetSystemUser("7209233");

            var result = (currentUser != null) ? currentUser.IsActive : false;

			if (result)
			{
				UserSecurityDetails.PINumber = currentUser.PINumber;
				UserSecurityDetails.AccessLevel = currentUser.AccessLevel.GetEnumFromString<AccessLevels>();
				UserSecurityDetails.FullNameAndPINumber = currentUser.FullNameAndPINumber;
			}

			return result;
		}

		private void RegisterMenus()
        {

				var eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
				var RegionManager = ServiceLocator.Current.GetInstance<IRegionManager>();

				RegionManager.RegisterViewWithRegion(Infrastructure.RegionNames.TopMenuRegion, typeof(MenuModule.Views.TopMenuView));

				eventAggregator.GetEvent<MenuViewRequestEvent>().Publish("SearchView");
		}
	}
}