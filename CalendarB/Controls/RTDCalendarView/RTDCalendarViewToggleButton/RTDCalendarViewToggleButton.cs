using System;
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

        public bool IsBlackSelected
        {
            get => (bool)GetValue(IsBlackSelectedProperty);
            set => SetValue(IsBlackSelectedProperty, value);
        }

        public static readonly DependencyProperty IsBlackSelectedProperty =
            DependencyProperty.Register(nameof(IsBlackSelected), typeof(bool), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(false));

        //public bool IsNotSelected => !IsSelected;

        public bool IsSelected
		{
			get => (bool)GetValue(IsRedSelectedProperty);
			set => SetValue(IsRedSelectedProperty, value);
		}

		public static readonly DependencyProperty IsRedSelectedProperty =
			DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(false));

        public bool IsBlackout
		{
			get => (bool)GetValue(IsBlackoutProperty);
			set => SetValue(IsBlackoutProperty, value);
		}

		public static readonly DependencyProperty IsBlackoutProperty =
			DependencyProperty.Register(nameof(IsBlackout), typeof(bool), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(false, (d, e) => ((RTDCalendarViewToggleButton)d).IsEnabled = !((RTDCalendarViewToggleButton)d).IsBlackout));

		public bool IsDisabled
		{
			get => (bool)GetValue(IsDisabledProperty);
			set => SetValue(IsDisabledProperty, value);
		}

		public static readonly DependencyProperty IsDisabledProperty =
			DependencyProperty.Register(nameof(IsDisabled), typeof(bool), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(false, (d, e) => ((RTDCalendarViewToggleButton)d).IsEnabled = !((RTDCalendarViewToggleButton)d).IsDisabled));

        public bool IsHidden
        {
            get => (bool)GetValue(IsHiddenProperty);
            set => SetValue(IsHiddenProperty, value);
        }
        
        public static readonly DependencyProperty IsHiddenProperty =
            DependencyProperty.Register(nameof(IsHidden), typeof(bool), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(false, (d, e) => ((RTDCalendarViewToggleButton)d).IsEnabled = !((RTDCalendarViewToggleButton)d).IsHidden));

		public bool IsToday
		{
			get => (bool)GetValue(IsTodayProperty);
			set => SetValue(IsTodayProperty, value);
		}

		public static readonly DependencyProperty IsTodayProperty =
			DependencyProperty.Register(nameof(IsToday), typeof(bool), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(false));

		public DateTime DateTime
		{
			get => (DateTime)GetValue(DateTimeProperty);
			set => SetValue(DateTimeProperty, value);
		}

		public static readonly DependencyProperty DateTimeProperty =
			DependencyProperty.Register(nameof(DateTime), typeof(DateTime), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(DateTime.Now, (d, e) => ((RTDCalendarViewToggleButton)d).Content = ((RTDCalendarViewToggleButton)d).DateTime.Day));

        public Brush TodayForeground
        {
            get => (Brush)GetValue(TodayForegroundProperty);
            set => SetValue(TodayForegroundProperty, value);
        }

        public static readonly DependencyProperty TodayForegroundProperty =
            DependencyProperty.Register(nameof(TodayForeground), typeof(Brush), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(default(Brush)));

        public Brush RedSelectedForeground
        {
            get => (Brush)GetValue(RedSelectedForegroundProperty);
            set => SetValue(RedSelectedForegroundProperty, value);
        }

        public static readonly DependencyProperty RedSelectedForegroundProperty =
            DependencyProperty.Register(nameof(RedSelectedForeground), typeof(Brush), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(default(Brush)));

        public Brush RedSelectedBackground
        {
            get => (Brush)GetValue(RedSelectedBackgroundProperty);
            set => SetValue(RedSelectedBackgroundProperty, value);
        }

        public static readonly DependencyProperty RedSelectedBackgroundProperty =
            DependencyProperty.Register(nameof(RedSelectedBackground), typeof(Brush), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(default(Brush)));

        public Brush BlackSelectedBorderBrush
        {
            get => (Brush)GetValue(BlackSelectedBorderBrushProperty);
            set => SetValue(BlackSelectedBorderBrushProperty, value);
        }

        public static readonly DependencyProperty BlackSelectedBorderBrushProperty =
            DependencyProperty.Register(nameof(BlackSelectedBorderBrush), typeof(Brush), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(default(Brush)));

        public Brush BlackoutForeground
        {
            get => (Brush)GetValue(BlackoutForegroundProperty);
            set => SetValue(BlackoutForegroundProperty, value);
        }

        public static readonly DependencyProperty BlackoutForegroundProperty =
            DependencyProperty.Register(nameof(BlackoutForeground), typeof(Brush), typeof(RTDCalendarViewToggleButton),
                new PropertyMetadata(default(Brush)));

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
