using System.Diagnostics;
using System.Reflection;

namespace REITs.Infrastructure
{
	public class ObjectChangeEventing
	{
		private object CallingClass;

		public ObjectChangeEventing(object _callingClass)
		{
			CallingClass = _callingClass;
		}

		public void ObjectPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			// Call ObjectChanged
			InvokeMethod(string.Format("Object{0}Changed", sender.GetType().Name));

			// Call PropertyChanged
			InvokeMethod(string.Format("Property{0}Changed", e.PropertyName));
		}

		private void InvokeMethod(string methodName)
		{
			try
			{
				MethodInfo mi = CallingClass.GetType().GetMethod(methodName);
				if (mi != null)
					mi.Invoke(CallingClass, null);

				Debug.Print(string.Format("Attempt to call method: {0}, {1}", methodName, (mi != null) ? "Succeeded" : "Failed"));
			}
			catch { }
		}
	}
}