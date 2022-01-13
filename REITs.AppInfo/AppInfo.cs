using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Unity;
using Prism.Regions;
using REITs.AppInfoModule.Views;

namespace REITs.AppInfoModule
{
    public class AppInfo:IModule
    {
        IUnityContainer _unityContainer;
        IRegionManager _regionManager;

        public AppInfo(RegionManager regionManager, IUnityContainer unityContainer)
        {
            _regionManager = regionManager;
            _unityContainer = unityContainer;
        }

        public void Initialize()
        {
            _unityContainer.RegisterTypeForNavigation<HomeView>();
            _unityContainer.RegisterTypeForNavigation<AboutView>();
            
        }
    }
}
