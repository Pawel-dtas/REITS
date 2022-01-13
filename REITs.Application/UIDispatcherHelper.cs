using System;
using System.Windows.Threading;

public class UIDispatcherHelper
{
	public static UIDispatcherHelper UIDispatcher { get { return _instance; } }

	private static readonly UIDispatcherHelper _instance = new UIDispatcherHelper();

	public Dispatcher Instance { get; private set; }

	private UIDispatcherHelper()
	{
		if (UIDispatcher == null)
			Instance = Dispatcher.CurrentDispatcher;
		;
	}

	public void InvokeOnUi(Action action)
	{
		if (action == null)
			throw new ArgumentNullException("InvokeOnUI action NULL");

		Instance.Invoke(action, DispatcherPriority.Background);
	}

	public void InvokeOnUi(Action action, DispatcherPriority priority)
	{
		if (action == null)
			throw new ArgumentNullException("InvokeOnUI action NULL");

		Instance.Invoke(action);
	}

	public DispatcherOperation InvokeOnUiAsync(Action action, DispatcherPriority priority)
	{
		if (action == null)
			throw new ArgumentNullException("InvokeOnUIAsync action NULL");

		return Instance.InvokeAsync(action, priority);
	}
}