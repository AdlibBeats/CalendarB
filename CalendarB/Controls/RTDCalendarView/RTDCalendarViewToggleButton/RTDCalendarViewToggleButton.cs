using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace CalendarB.Controls.RTDCalendarView
{
	public class RTDCalendarViewToggleButton : ContentControl
	{
		#region Public Events

		public event RoutedEventHandler Selected;
		public event RoutedEventHandler Unselected;

		#endregion

		#region Public Cotr
		public RTDCalendarViewToggleButton()
		{
			DefaultStyleKey = typeof(RTDCalendarViewToggleButton);

		
			PointerPressed += OnPointerPressed;
			PointerReleased += OnPointerReleased;
			PointerEntered += OnPointerEntered;
			PointerExited += OnPointerExited;
		}
		#endregion

		#region Protected OnApplyTemplate
		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (IsDisabled)
				UpdateIsDisable();
			else if (IsBlackout)
				UpdateIsBlackout();
			else
				UpdateIsSelected();

			UpdateIsToday();
			UpdateIsWeekend();
		}
		#endregion

		#region Private Handlers
		private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
		{
			VisualStateManager.GoToState(this, IsSelected ? "CheckedPressed" : "Pressed", true);

			IsSelected = !IsSelected;

			if (IsSelected)
				Selected?.Invoke(this, null);
			else
				Unselected?.Invoke(this, null);
		}

		private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
		{
			VisualStateManager.GoToState(this, IsSelected ? "CheckedPointerOver" : "PointerOver", true);
		}

		private void OnPointerExited(object sender, PointerRoutedEventArgs e)
		{
			VisualStateManager.GoToState(this, IsSelected ? "CheckedNormal" : "Normal", true);
		}

		private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
		{
			VisualStateManager.GoToState(this, IsSelected ? "CheckedNormal" : "Normal", true);
		}
		#endregion

	

		#region Private Updating Methods
		private void UpdateIsSelected()
		{
			IsEnabled = true;

			VisualStateManager.GoToState(this, IsSelected ? "CheckedNormal" : "Normal", true);
		}

		private void UpdateIsBlackout()
		{
			IsEnabled = !IsBlackout;

			VisualStateManager.GoToState(this, IsSelected ? "CheckedBlackouted" : "Blackouted", true);
		}

		private void UpdateIsDisable()
		{
			IsEnabled = !IsDisabled;

			VisualStateManager.GoToState(this, IsSelected ? "CheckedDisabled" : "Disabled", true);
		}

        private void UpdateIsHidden()
        {
            IsEnabled = !IsHidden;

            VisualStateManager.GoToState(this, IsHidden ? "CheckedDisabled" : "Disabled", true);
        }

		private void UpdateIsWeekend()
		{
			VisualStateManager.GoToState(this, IsWeekend ? "IsWeekendTrue" : "IsWeekendFalse", true);
		}

		private void UpdateIsToday()
		{
			VisualStateManager.GoToState(this, IsToday ? "IsTodayTrue" : "IsTodayFalse", true);
		}

		private void UpdateDateTime()
		{
			Content = DateTime.Day;
		}
		#endregion

		#region Public Dependency Properties
		public bool IsSelected
		{
			get => (bool)GetValue(IsSelectedProperty);
			set => SetValue(IsSelectedProperty, value);
		}

		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(false, (d, e) => ((RTDCalendarViewToggleButton)d).UpdateIsSelected()));

		public bool IsBlackout
		{
			get => (bool)GetValue(IsBlackoutProperty);
			set => SetValue(IsBlackoutProperty, value);
		}

		public static readonly DependencyProperty IsBlackoutProperty =
			DependencyProperty.Register(nameof(IsBlackout), typeof(bool), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(true, (d, e) => ((RTDCalendarViewToggleButton)d).UpdateIsBlackout()));

		public bool IsDisabled
		{
			get => (bool)GetValue(IsDisabledProperty);
			set => SetValue(IsDisabledProperty, value);
		}

		public static readonly DependencyProperty IsDisabledProperty =
			DependencyProperty.Register(nameof(IsDisabled), typeof(bool), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(false, (d, e) => ((RTDCalendarViewToggleButton)d).UpdateIsDisable()));

        public bool IsHidden
        {
            get => (bool)GetValue(IsHiddenProperty);
            set => SetValue(IsHiddenProperty, value);
        }
        
        public static readonly DependencyProperty IsHiddenProperty =
            DependencyProperty.Register(nameof(IsHidden), typeof(bool), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(false, (d, e) => ((RTDCalendarViewToggleButton)d).UpdateIsHidden()));

        public bool IsWeekend
		{
			get => (bool)GetValue(IsWeekendProperty);
			set => SetValue(IsWeekendProperty, value);
		}

		public static readonly DependencyProperty IsWeekendProperty =
			DependencyProperty.Register(nameof(IsWeekend), typeof(bool), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(false, (d, e) => ((RTDCalendarViewToggleButton)d).UpdateIsWeekend()));

		public bool IsToday
		{
			get => (bool)GetValue(IsTodayProperty);
			set => SetValue(IsTodayProperty, value);
		}

		public static readonly DependencyProperty IsTodayProperty =
			DependencyProperty.Register(nameof(IsToday), typeof(bool), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(false, (d, e) => ((RTDCalendarViewToggleButton)d).UpdateIsToday()));

		public DateTime DateTime
		{
			get => (DateTime)GetValue(DateTimeProperty);
			set => SetValue(DateTimeProperty, value);
		}

		public static readonly DependencyProperty DateTimeProperty =
			DependencyProperty.Register(nameof(DateTime), typeof(DateTime), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(DateTime.Now, (d, e) => ((RTDCalendarViewToggleButton)d).UpdateDateTime()));
		#endregion
	}
}
