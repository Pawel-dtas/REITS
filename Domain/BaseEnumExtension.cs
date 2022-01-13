using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Domain
{
    public static class BaseEnumExtension
    {
        #region Public Methods

        public static string GetDescriptionFromEnum(this Enum value)
        {
            DescriptionAttribute attribute = new DescriptionAttribute();
            if (value != null)
            {
                attribute = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).SingleOrDefault() as DescriptionAttribute;
            }
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static T GetEnumFromString<T>(this string value)
        {
            T result = default(T);

            if (value == null)
            {
                result = default(T);
            }
            else
            {
                if (Enum.IsDefined(typeof(T), value))
                {
                    result = (T)Enum.Parse(typeof(T), value, true);
                }
                else
                {
                    string[] enumNames = Enum.GetNames(typeof(T));

                    foreach (string enumName in enumNames)
                    {
                        object e = Enum.Parse(typeof(T), enumName);
                        if (value.ToUpper() == GetDescriptionFromEnum((Enum)e).ToUpper())
                        {
                            result = (T)e;
                        }
                    }
                }
            }

            return result;
        }

        public static bool EnumToBoolean(this string str)
        {
            return str.ToLower() == "yes";
        }

        public static IList ToList(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            ArrayList list = new ArrayList();
            Array enumValues = Enum.GetValues(type);

            foreach (Enum value in enumValues)
            {
                if (GetDescriptionFromEnum(value) != string.Empty)
                {
                    list.Add(GetDescriptionFromEnum(value));
                }
            }

            return list;
        }

        public static List<string> GetEnumValuesAndDescription<T>()
        {
            Type enumType = typeof(T);
            List<string> displayList = new List<string>();

            if (enumType.BaseType != typeof(Enum))
            {
                throw new ArgumentException("T is not System.Enum");
            }

            foreach (var e in Enum.GetValues(typeof(T)))
            {
                var fi = e.GetType().GetField(e.ToString());
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                displayList.Add(e.ToString() + " - " + attributes[0].Description);
            }
            return displayList;
        }

        public static string GetEnumValueAndDescription<T>(this Enum value)
        {
            DescriptionAttribute attribute = new DescriptionAttribute();
            if (value != null)
            {
                attribute = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).SingleOrDefault() as DescriptionAttribute;
            }
            return attribute == null ? value.ToString() : value.ToString() + " - " + attribute.Description;
        }

        public static List<string> GetEnumDescriptions<T>()
        {
            Type enumType = typeof(T);
            List<string> displayList = new List<string>();

            if (enumType.BaseType != typeof(Enum))
            {
                throw new ArgumentException("T is not System.Enum");
            }

            foreach (var e in Enum.GetValues(typeof(T)))
            {
                var fi = e.GetType().GetField(e.ToString());
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                displayList.Add(attributes[0].Description);
            }
            return displayList;
        }

        #endregion Public Methods
    }
}