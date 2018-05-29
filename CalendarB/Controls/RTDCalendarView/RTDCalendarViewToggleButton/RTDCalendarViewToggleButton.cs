using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CalendarB.Controls.RTDCalendarView
{
	public class RTDCalendarViewToggleButton : ContentControl
	{
        private Grid _root;

        public event RoutedEventHandler Selected;
		public event RoutedEventHandler Unselected;

        public RTDCalendarViewToggleButton()
        {
            DefaultStyleKey = typeof(RTDCalendarViewToggleButton);

            PointerPressed += (s, e) =>
            {
                IsSelected = !IsSelected;

                if (IsSelected)
                    Selected?.Invoke(this, new RoutedEventArgs());
                else
                    Unselected?.Invoke(this, new RoutedEventArgs());
            };

            SizeChanged += (s, e) => UpdateCornerRadius();
        }

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

            _root = GetTemplateChild("Root") as Grid;

            UpdateCornerRadius();
            UpdateSelectedState();
        }

        private void UpdateCornerRadius()
        {
            if (_root == null) return;
            if (IsCornerRadiusVisible)
                _root.CornerRadius =
                    CornerRadius.Equals(default(CornerRadius)) ? new CornerRadius(ActualHeight / 2) : CornerRadius;
            else
                _root.CornerRadius = default(CornerRadius);
        }

        private void UpdateDateTime() =>
            Content = DateTime.Day;

        private void UpdateSelectedState()
        {
            if (IsSelected && OldDateTime != default(DateTime))
                VisualStateManager.GoToState(this, "SelectedOldDateTimeState", true);
            else if (IsSelected)
                VisualStateManager.GoToState(this, IsBlackSelectionMode ? "BlackSelectedState" : "SelectedState", true);
            else if (OldDateTime != default(DateTime))
            {
                //IsBlackSelectionMode = false;
                VisualStateManager.GoToState(this, "OldDateTimeState", true);
            }
            else
                VisualStateManager.GoToState(this, "UnselectedState", true);
        }

        public bool IsBlackSelectionMode
        {
            get => (bool)GetValue(IsBlackSelectionModeProperty);
            set => SetValue(IsBlackSelectionModeProperty, value);
        }

        public static readonly DependencyProperty IsBlackSelectionModeProperty =
            DependencyProperty.Register(nameof(IsBlackSelectionMode), typeof(bool), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(false));

        public bool IsSelected
		{
			get => (bool)GetValue(IsSelectedProperty);
			set => SetValue(IsSelectedProperty, value);
		}

		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(false, (d, e) => ((RTDCalendarViewToggleButton)d).UpdateSelectedState()));

        public bool IsBlackout
		{
			get => (bool)GetValue(IsBlackoutProperty);
			set => SetValue(IsBlackoutProperty, value);
		}

		public static readonly DependencyProperty IsBlackoutProperty =
			DependencyProperty.Register(nameof(IsBlackout), typeof(bool), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(false, (d, e) =>
                ((RTDCalendarViewToggleButton)d).IsEnabled =
                    !((RTDCalendarViewToggleButton)d).IsBlackout));

		public bool IsDisabled
		{
			get => (bool)GetValue(IsDisabledProperty);
			set => SetValue(IsDisabledProperty, value);
		}

		public static readonly DependencyProperty IsDisabledProperty =
			DependencyProperty.Register(nameof(IsDisabled), typeof(bool), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(false, (d, e) =>
                ((RTDCalendarViewToggleButton)d).IsEnabled =
                    !((RTDCalendarViewToggleButton)d).IsDisabled));

        public bool IsHidden
        {
            get => (bool)GetValue(IsHiddenProperty);
            set => SetValue(IsHiddenProperty, value);
        }
        
        public static readonly DependencyProperty IsHiddenProperty =
            DependencyProperty.Register(nameof(IsHidden), typeof(bool), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(false, (d, e) =>
                ((RTDCalendarViewToggleButton)d).IsEnabled =
                    !((RTDCalendarViewToggleButton)d).IsHidden));

		public bool IsToday
		{
			get => (bool)GetValue(IsTodayProperty);
			set => SetValue(IsTodayProperty, value);
		}

		public static readonly DependencyProperty IsTodayProperty =
			DependencyProperty.Register(nameof(IsToday), typeof(bool), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(false));

        public DateTime OldDateTime
        {
            get => (DateTime)GetValue(OldDateTimeProperty);
            set => SetValue(OldDateTimeProperty, value);
        }

        public static readonly DependencyProperty OldDateTimeProperty =
            DependencyProperty.Register(nameof(OldDateTime), typeof(DateTime), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(default(DateTime), (d, e) => ((RTDCalendarViewToggleButton)d).UpdateSelectedState()));

        public DateTime DateTime
		{
			get => (DateTime)GetValue(DateTimeProperty);
			set => SetValue(DateTimeProperty, value);
		}

		public static readonly DependencyProperty DateTimeProperty =
			DependencyProperty.Register(nameof(DateTime), typeof(DateTime), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(DateTime.Now, (d, e) => ((RTDCalendarViewToggleButton)d).UpdateDateTime()));

        public Brush TodayForeground
        {
            get => (Brush)GetValue(TodayForegroundProperty);
            set => SetValue(TodayForegroundProperty, value);
        }

        public static readonly DependencyProperty TodayForegroundProperty =
            DependencyProperty.Register(nameof(TodayForeground), typeof(Brush), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 237, 28, 36))));

        public Brush BlackSelectedForeground
        {
            get => (Brush)GetValue(BlackSelectedForegroundProperty);
            set => SetValue(BlackSelectedForegroundProperty, value);
        }

        public static readonly DependencyProperty BlackSelectedForegroundProperty =
            DependencyProperty.Register(nameof(BlackSelectedForeground), typeof(Brush), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 44, 50, 66))));

        public Brush BlackSelectedBackground
        {
            get => (Brush)GetValue(BlackSelectedBackgroundProperty);
            set => SetValue(BlackSelectedBackgroundProperty, value);
        }

        public static readonly DependencyProperty BlackSelectedBackgroundProperty =
            DependencyProperty.Register(nameof(BlackSelectedBackground), typeof(Brush), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public Brush BlackSelectedBorderBrush
        {
            get => (Brush)GetValue(BlackSelectedBorderBrushProperty);
            set => SetValue(BlackSelectedBorderBrushProperty, value);
        }

        public static readonly DependencyProperty BlackSelectedBorderBrushProperty =
            DependencyProperty.Register(nameof(BlackSelectedBorderBrush), typeof(Brush), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 44, 50, 66))));

        public Thickness BlackSelectedBorderThickness
        {
            get => (Thickness)GetValue(BlackSelectedBorderThicknessProperty);
            set => SetValue(BlackSelectedBorderThicknessProperty, value);
        }

        public static readonly DependencyProperty BlackSelectedBorderThicknessProperty =
            DependencyProperty.Register(nameof(BlackSelectedBorderThickness), typeof(Thickness), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(new Thickness(2d)));

        public Brush SelectedForeground
        {
            get => (Brush)GetValue(SelectedForegroundProperty);
            set => SetValue(SelectedForegroundProperty, value);
        }

        public static readonly DependencyProperty SelectedForegroundProperty =
            DependencyProperty.Register(nameof(SelectedForeground), typeof(Brush), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 255, 255, 255))));

        public Brush SelectedBackground
        {
            get => (Brush)GetValue(SelectedBackgroundProperty);
            set => SetValue(SelectedBackgroundProperty, value);
        }

        public static readonly DependencyProperty SelectedBackgroundProperty =
            DependencyProperty.Register(nameof(SelectedBackground), typeof(Brush), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 237, 28, 36))));

        public Brush SelectedBorderBrush
        {
            get => (Brush)GetValue(SelectedBorderBrushProperty);
            set => SetValue(SelectedBorderBrushProperty, value);
        }

        public static readonly DependencyProperty SelectedBorderBrushProperty =
            DependencyProperty.Register(nameof(SelectedBorderBrush), typeof(Brush), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 237, 28, 36))));

        public Thickness SelectedBorderThickness
        {
            get => (Thickness)GetValue(SelectedBorderThicknessProperty);
            set => SetValue(SelectedBorderThicknessProperty, value);
        }

        public static readonly DependencyProperty SelectedBorderThicknessProperty =
            DependencyProperty.Register(nameof(SelectedBorderThickness), typeof(Thickness), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(new Thickness(0d)));

        public Brush BlackoutForeground
        {
            get => (Brush)GetValue(BlackoutForegroundProperty);
            set => SetValue(BlackoutForegroundProperty, value);
        }

        public static readonly DependencyProperty BlackoutForegroundProperty =
            DependencyProperty.Register(nameof(BlackoutForeground), typeof(Brush), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 189, 189, 189))));

        public bool IsCornerRadiusVisible
        {
            get => (bool)GetValue(IsCornerRadiusVisibleProperty);
            set => SetValue(IsCornerRadiusVisibleProperty, value);
        }

        public static readonly DependencyProperty IsCornerRadiusVisibleProperty =
            DependencyProperty.Register(nameof(IsCornerRadiusVisible), typeof(bool), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(false, (d, e) => ((RTDCalendarViewToggleButton)d).UpdateCornerRadius()));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(default(CornerRadius), (d, e) => ((RTDCalendarViewToggleButton)d).UpdateCornerRadius()));
    }
}
