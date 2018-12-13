using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BoardVisualizer.util
{
	public abstract class BindableBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
		{
			if (!Equals(storage, value))
			{
				storage = value;
				var handler = PropertyChanged;
				if (handler != null)
					handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}