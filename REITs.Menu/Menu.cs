using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REITs.MenuModule
{
    public class Menu:IModule
    {
        IUnityContainer _unityContainer;
        IRegionManager _regionManager;

        public Menu(RegionManager regionManager, IUnityContainer unityContainer)
        {
            _regionManager = regionManager;
            _unityContainer = unityContainer;
        }

        public void Initialize()
        {

        }
    }
}
