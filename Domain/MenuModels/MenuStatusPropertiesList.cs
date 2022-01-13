using Prism.Events;
using System;

namespace REITs.Domain.MenuModels
{
    public class MenuItemEnabledEvent : PubSubEvent<MenuItemEnabledEventPayload>
    { }

    public class MenuItemEnabledEventPayload
    {
        public string MenuName { get; set; }
        public bool MenuEnabled { get; set; }

        public MenuItemEnabledEventPayload(string menuName, bool menuEnabled)
        {
            MenuName = menuName;
            MenuEnabled = menuEnabled;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", MenuName, MenuEnabled.ToString());
        }
    }

    public class MenuItemClickedEvent : PubSubEvent<MenuItemClickedEventPayload>
    { }

    public class MenuItemClickedEventPayload
    {
        public string MenuName { get; set; }

        public Guid XMLGuid { get; set; }

        public MenuItemClickedEventPayload(string menuName, Guid xmlGuid)
        {
            MenuName = menuName;
            XMLGuid = xmlGuid;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", MenuName, XMLGuid.ToString());
        }
    }
}