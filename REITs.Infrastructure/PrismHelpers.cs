using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Modularity;
using Prism.Regions;
using System.ComponentModel;
using System.Windows;

namespace REITs.Infrastructure
{
    public static class PrismHelpers
    {
        public static IUnityContainer GetUnityContainer()
        {
            IUnityContainer _container = (UnityContainer)System.Windows.Application.Current.Resources["IoC"];

            return _container;
        }

        public static IEventAggregator GetEventAggregator()
        {
            return GetUnityContainer().Resolve<IEventAggregator>();
        }

        public static IRegionManager GetRegionManager()
        {
            return GetUnityContainer().Resolve<IRegionManager>();
        }

        public static IModuleCatalog GetModuleCatalog()
        {
            return GetUnityContainer().Resolve<IModuleCatalog>();
        }

        public static IModuleManager GetModuleManager()
        {
            return GetUnityContainer().Resolve<IModuleManager>();
        }

        public static T ResolveService<T>()
        {
            return GetUnityContainer().Resolve<T>();
        }

        public static bool CheckForDesignMode()
        {
            return (bool)(DesignerProperties.GetIsInDesignMode(new DependencyObject()));
        }
    }   
}
