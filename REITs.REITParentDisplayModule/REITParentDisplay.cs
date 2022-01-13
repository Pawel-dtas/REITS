using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using REITs.REITParentDisplayModule.Views;

namespace REITs.REITParentDisplayModule
{
    public class REITParentDisplay : IModule
    {
        private IUnityContainer _unityContainer;
        private IRegionManager _regionManager;

        public REITParentDisplay(IRegionManager regionManager, IUnityContainer unityContainer)
        {
            _regionManager = regionManager;
            _unityContainer = unityContainer;
        }

        public void Initialize()
        {
            _unityContainer.RegisterTypeForNavigation<REITParentView>();
        }
    }
}