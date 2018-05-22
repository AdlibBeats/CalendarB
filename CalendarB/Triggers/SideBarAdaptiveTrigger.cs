using Windows.UI.Xaml;

namespace CalendarB.Triggers
{
	public class SideBarAdaptiveTrigger : TriggerBase
	{
		private double _windowWidth = Window.Current.Bounds.Width;

		public SideBarAdaptiveTrigger() =>
			Window.Current.SizeChanged += (s, e) =>
			{
				_windowWidth = e.Size.Width;
				SetActive(IsActive && _windowWidth > MinWindowWidth);
			};

		private bool _isActive;
		public bool IsActive
		{
			get => _isActive;
			set => SetProperty(ref _isActive, value, value && _windowWidth > MinWindowWidth);
		}

		private double _minWindowWidth;
		public double MinWindowWidth
		{
			get => _minWindowWidth;
			set => SetProperty(ref _minWindowWidth, value, IsActive && _windowWidth > value);
		}
	}
}
