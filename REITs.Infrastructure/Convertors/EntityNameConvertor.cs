using REITs.Domain.Models;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace REITs.Infrastructure.Convertors
{
    public class EntityNameIdConvertor : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string entityNameValue = "Test Value";

            if (values[0] != DependencyProperty.UnsetValue)
            {
                Guid EntityId = (values[0] != null) ? (Guid)values[0] : Guid.Empty;

                ObservableCollection<Entity> tempEntityList = (ObservableCollection<Entity>)values[1];

                if (EntityId != null)
                    if (tempEntityList != null)
                        entityNameValue = tempEntityList.Where(x => x.Id == EntityId).Select(x => x.EntityName).FirstOrDefault();
            }

            return entityNameValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}