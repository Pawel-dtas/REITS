using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using REITs.REITDisplayModule.Views;

namespace REITs.REITDisplayModule
{
    public class REITDisplay : IModule
    {
        private IUnityContainer _unityContainer;
        private IRegionManager _regionManager;

        public REITDisplay(IRegionManager regionManager, IUnityContainer unityContainer)
        {
            _regionManager = regionManager;
            _unityContainer = unityContainer;
        }

        public void Initialize()
        {
            _unityContainer.RegisterTypeForNavigation<REITView>();
        }
    }
}