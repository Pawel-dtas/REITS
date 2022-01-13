using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using REITs.ImportModule.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REITs.ImportModule
{
    public class Import : IModule
    {
        IUnityContainer _unityContainer;
        IRegionManager _regionManager;

        public Import(RegionManager regionManager, IUnityContainer unityContainer)
        {
            _regionManager = regionManager;
            _unityContainer = unityContainer;
        }
        public void Initialize()
        {
            _unityContainer.RegisterTypeForNavigation<ImportView>();
        }
    }    
}

