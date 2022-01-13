using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using REITs.UserModule.Views;

namespace REITs.UserModule
{
    public class Users : IModule
    {
        IUnityContainer _unityContainer;
        IRegionManager _regionManager;

        public Users(RegionManager regionManager, IUnityContainer unityContainer)
        {
            _regionManager = regionManager;
            _unityContainer = unityContainer;
        }

        public void Initialize()
        {
            _unityContainer.RegisterTypeForNavigation<UsersView>();
        }
    }
}
