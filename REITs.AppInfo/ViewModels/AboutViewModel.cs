using REITs.Domain.MenuModels;
using REITs.Infrastructure;

namespace REITs.AppInfoModule.ViewModels
{
    public class AboutViewModel
    {
        public AboutViewModel()
        {
            PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("Search", true));
        }
    }
}
