using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using REITs.ReportsModule.Views;

namespace REITs.ReportsModule
{
    public class Reports : IModule
    {
        private IUnityContainer _unityContainer;
        private IRegionManager _regionManager;

        public Reports(RegionManager regionManager, IUnityContainer unityContainer)
        {
            _regionManager = regionManager;
            _unityContainer = unityContainer;
        }

        public void Initialize()
        {
            _unityContainer.RegisterTypeForNavigation<ReportsView>();
        }
    }
}