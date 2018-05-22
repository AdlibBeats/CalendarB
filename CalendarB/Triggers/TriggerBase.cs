using Windows.UI.Xaml;

namespace CalendarB.Triggers
{
	public class TriggerBase : StateTriggerBase
	{
		public void SetProperty<T>(ref T storage, T value, bool isActive)
		{
			storage = value;
			SetActive(isActive);
		}
	}
}
