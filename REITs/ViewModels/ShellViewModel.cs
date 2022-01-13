using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using REITs.Infrastructure.Events;
using System.Windows;

namespace REITs.ViewModels
{
	public class ShellViewModel : BindableBase
	{
		#region Properties

		public string ShellTitle { get { return Properties.Resources.ShellTitle; } }
		public string BannerTitle { get { return Properties.Resources.BannerTitle; } }

		private Visibility _topMenuVisibility = Visibility.Visible;

		public Visibility TopMenuVisibility
		{
			get { return _topMenuVisibility; }
			set { SetProperty(ref _topMenuVisibility, value); }
		}

		#endregion Properties

		#region Variables

		private IEventAggregator _eventAggregator;
		private IRegionManager _regionManager;

		#endregion Variables

		#region Commands

		public DelegateCommand TopMenuVisibilityCommand { get; private set; }

		#endregion Commands

		#region Constructor

		public ShellViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
		{
			_eventAggregator = eventAggregator;
			_regionManager = regionManager;

			_eventAggregator.GetEvent<MenuViewRequestEvent>().Subscribe(OnMenuViewRequestEvent);
			_eventAggregator.GetEvent<TopMenuVisibilityCollapseEvent>().Subscribe(HandleTopMenuVisibilityCollapseEvent);

			InitialiseCommands();
		}

		#endregion Constructor

		#region Private Methods

		private void InitialiseCommands()
		{
			TopMenuVisibilityCommand = new DelegateCommand(HandleTopMenuVisibilityEvent);
		}

		private void OnMenuViewRequestEvent(string name)
		{
			IRegion region = _regionManager.Regions["ContentRegion"];
			_regionManager.RequestNavigate("ContentRegion", name);
		}

		private void HandleTopMenuVisibilityCollapseEvent()
		{
			TopMenuVisibility = Visibility.Collapsed;
		}

		private void HandleTopMenuVisibilityEvent()
		{
			TopMenuVisibility = (TopMenuVisibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
		}

		#endregion Private Methods
	}
}