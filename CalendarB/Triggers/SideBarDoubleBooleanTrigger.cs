namespace CalendarB.Triggers
{
	public class SideBarDoubleBooleanTrigger : TriggerBase
	{
		private bool _isFirstActive;
		public bool IsFirstActive
		{
			get => _isFirstActive;
			set => SetProperty(ref _isFirstActive, value, value && IsSecondActive);
		}

		private bool _isSecondActive;
		public bool IsSecondActive
		{
			get => _isSecondActive;
			set => SetProperty(ref _isSecondActive, value, value && IsFirstActive);
		}
	}
}
