using Microsoft.Practices.Unity;
using Prism.Regions;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace REITs.Infrastructure
{
    public static class UIHelper
    {
        // ViewModel Methods
        public static void PushModelToView(object sourceModel, object targetViewModel)
        {
            CopyPropertiesFromSourceToDest(sourceModel, targetViewModel);
        }

        public static void PushViewToModel(object sourceViewModel, object targetModel)
        {
            CopyPropertiesFromSourceToDest(sourceViewModel, targetModel);
        }

        public static void CopyObject(object sourceObject, object targetObject)
        {
            CopyPropertiesFromSourceToDest(sourceObject, targetObject);
        }

        private static void CopyPropertiesFromSourceToDest(object source, object target)
        {
            Type sourceType = source.GetType();

            foreach (PropertyInfo propInfo in target.GetType().GetProperties())
            {
                try
                {
                    if (sourceType.GetProperty(propInfo.Name) != null)
                    {
                        var propValue = sourceType.GetProperty(propInfo.Name).GetValue(source, null);

                        var t = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
                        var safeValue = (propValue == null) ? null : Convert.ChangeType(propValue, t);
                        propInfo.SetValue(target, safeValue);

                        Debug.Print("Set property " + propInfo.Name + " - " + ((safeValue == null) ? "null" : safeValue.ToString()));
                    }
                }
                catch (Exception ex) { Debug.Print("Property not set: " + propInfo.Name + " - " + ex.Message.ToString()); }
            }
        }

        public static bool CheckForRecordChangedAgainstDomainModel<T>(PropertyChangedEventArgs propertyChanged)
        {
            bool recordChanged = false;

            // check if the property that changed is in the domain model for database update

            if (typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Any(p => p.Name == propertyChanged.PropertyName))
            {
                recordChanged = true;

                Debug.Print("Set RecordChanged: " + recordChanged.ToString() + " for: " + propertyChanged.PropertyName.ToString());
            }
            else
            {
                Debug.Print("Checked RecordChanged: NOT-DOMAIN Property: " + propertyChanged.PropertyName.ToString());
            }

            return recordChanged;
        }

        // UI Methods
        public static DependencyObject FindChild(DependencyObject parent, Func<DependencyObject, bool> predicate)
        {
            if (parent == null)
                return null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (predicate(child))
                {
                    return child;
                }
                else
                {
                    var foundChild = FindChild(child, predicate);
                    if (foundChild != null)
                        return foundChild;
                }
            }

            return null;
        }

        public static class FocusHelper
        {
            public static void Focus(UIElement element)
            {
                ThreadPool.QueueUserWorkItem(delegate (Object theElement)
                {
                    UIElement elem = (UIElement)theElement;
                    elem.Dispatcher.Invoke(DispatcherPriority.Normal,
                        (Action)delegate ()
                        {
                            elem.Focus();
                            Keyboard.Focus(elem);
                        });
                }, element);
            }
        }

        public static void LoadViewIntoRegion<T>(string regionName)
        {
            IRegion region = PrismHelpers.GetRegionManager().Regions[regionName];
            IUnityContainer _container = PrismHelpers.GetUnityContainer();

            var view = _container.Resolve<T>();

            region.Add(view, typeof(T).FullName);
            region.Activate(view);
        }

        public static void ClearRegion(string regionName)
        {
            IRegion region = PrismHelpers.GetRegionManager().Regions[regionName];
            region.RemoveAll();
        }

        public static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            // get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null)
                return null;

            // check if the parent matches the type we’re looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                // use recursion to proceed with next level
                return FindVisualParent<T>(parentObject);
            }
        }

        // Allow UI Updates
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void AllowUIToUpdate()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        private static object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;

            return null;
        }

        public static class DefaultBrush
        {
            public static SolidColorBrush Transparent = new SolidColorBrush(System.Windows.Media.Colors.Transparent);
            public static SolidColorBrush White = new SolidColorBrush(System.Windows.Media.Colors.White);

            public static SolidColorBrush Green = new SolidColorBrush(System.Windows.Media.Colors.LightGreen);
            public static SolidColorBrush Orange = new SolidColorBrush(System.Windows.Media.Colors.Orange);
            public static SolidColorBrush Red = new SolidColorBrush(System.Windows.Media.Colors.Red);

            public static SolidColorBrush Yellow = new SolidColorBrush(System.Windows.Media.Colors.LightYellow);
        }
    }
}