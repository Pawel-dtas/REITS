using Prism.Events;

namespace REITs.Infrastructure.Events
{
	public class MenuViewRequestEvent : PubSubEvent<string>
	{ }

	public class TopMenuVisibilityCollapseEvent : PubSubEvent
	{ }
}