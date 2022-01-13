using Prism.Regions;
using REITs.Infrastructure.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace REITs.Infrastructure
{
    public static class ViewManagementExtensions
    {
        public static IEnumerable<object> FindViews(this IRegion region, string viewName)
        {
            return from view in region.Views
                   from attribute in Attribute.GetCustomAttributes(view.GetType())
                   where attribute is ViewName && ((ViewName)attribute).Name == viewName
                   select view;
        }
    }

    public static class IRegionManagerExtensions
    {
        public static void ActivateOrRequestNavigate(this IRegionManager regionManager, string regionName, string viewName, NavigationParameters parameters)
        {
            IRegion region = regionManager.Regions[regionName];
            object view = region.FindViews(viewName).FirstOrDefault();
            if (view != null)
                region.Activate(view);
            else
                regionManager.RequestNavigate(regionName, new Uri(
                    parameters != null ? viewName + parameters.ToString() : viewName, UriKind.Relative));
        }

        public static void RemoveAndRequestNavigate(this IRegionManager regionManager, string regionName, string viewName, NavigationParameters parameters)
        {
            IRegion region = regionManager.Regions[regionName];

            foreach (object view in region.FindViews(viewName))
                region.Remove(view);

            regionManager.RequestNavigate(regionName, new Uri(
                parameters != null ? viewName + parameters.ToString() : viewName, UriKind.Relative));
        }

        public static void RemoveViewAndGoBack(this IRegionManager regionManager, string regionName, string viewName)
        {
            IRegion region = regionManager.Regions[regionName];
            object view = region.FindViews(viewName).FirstOrDefault();
            region.Remove(view);
            view = region.Views.FirstOrDefault();
            if (view != null)
                region.Activate(view);
        }

        public static void RemoveView(this IRegionManager regionManager, string regionName, string viewName)
        {
            IRegion region = regionManager.Regions[regionName];
            object view = region.FindViews(viewName).FirstOrDefault();
            region.Remove(view);
        }

        public static bool IsViewCurrentView(this IRegionManager regionManager, string regionName, string viewName)
        {
            bool tempbool = false;
            if (!string.IsNullOrEmpty(regionName) && !string.IsNullOrEmpty(viewName))
            {
                IRegion region = regionManager.Regions[regionName];
                object view = region.ActiveViews.FirstOrDefault();
                if (view.ToString().Contains(viewName))
                    tempbool = true;
            }
            return tempbool;
        }
    }
}