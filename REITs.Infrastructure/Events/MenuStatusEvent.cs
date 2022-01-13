using Prism.Events;
using REITs.Domain.MenuModels;

namespace REITs.Infrastructure.Events
{
    public class MenuStatusEvent : PubSubEvent<MenuStatusProperties>
    {
    }
}
