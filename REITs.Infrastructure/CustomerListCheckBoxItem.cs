using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace REITs.Infrastructure
{
	public class CustomerListCheckBoxItem : INotifyPropertyChanged
	{
		private Guid _guid;

		public Guid Guid
		{
			get { return _guid; }
			set
			{
				_guid = value;
				OnPropertyChanged();
			}
		}

		private string _name;

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged();
			}
		}

		private int _sectorsFlag;

		public int SectorsFlag
		{
			get { return _sectorsFlag; }
			set
			{
				_sectorsFlag = value;
				OnPropertyChanged();
			}
		}

		private bool _isSelected;

		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				OnPropertyChanged();
			}
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string name = "")
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(name));
		}

		#endregion INotifyPropertyChanged
	}
}