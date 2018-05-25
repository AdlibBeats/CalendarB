using CalendarB.Controls.RTDCalendarView;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CalendarB.Controls
{
    public sealed class RTDCalendarDatePicker : Control
    {
        private Button _loadingButton;
        private ProgressRing _loadingProgress;
        private RTDCalendarView.RTDCalendarView _calendarView;
        private Flyout _rootFlyout;
        private Image _calendarIcon;
        private TextBlock _dateText;
        private Grid _flyoutBorder;

        public event EventHandler CalendarClosed;

        private bool _isTemplateLoaded;

        public RTDCalendarDatePicker()
        {
            DefaultStyleKey = typeof(RTDCalendarDatePicker);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_loadingButton != null)
            {
                _loadingButton.Tapped -= loadingButton_Tapped;
            }

            if (_calendarView != null)
            {
                _calendarView.Initialized -= OnCalendarViewInitialized;
                _calendarView.SelectionChanged -= ProCalendar_SelectionChanged;
                _calendarView.UnselectionChanged -= ProCalendar_UnselectionChanged;
            }

            _calendarIcon = GetTemplateChild("CalendarIcon") as Image;
            _loadingProgress = GetTemplateChild("LoadingProgress") as ProgressRing;
            _rootFlyout = GetTemplateChild("RootFlyout") as Flyout;
            _flyoutBorder = GetTemplateChild("FlyoutBorder") as Grid;
            _loadingButton = GetTemplateChild("LoadingButton") as Button;
            _dateText = GetTemplateChild("DateText") as TextBlock;
            _calendarView = GetTemplateChild("RTDCalendarView") as RTDCalendarView.RTDCalendarView;

            if (_loadingButton != null)
            {
                _loadingButton.Tapped += loadingButton_Tapped;
            }

            if (_calendarView != null)
            {
                _calendarView.Initialized += OnCalendarViewInitialized;
                _calendarView.SelectionChanged += ProCalendar_SelectionChanged;
                _calendarView.UnselectionChanged += ProCalendar_UnselectionChanged;

                _calendarView.EnableDates = EnableDates;
            }

            _isTemplateLoaded = (_calendarIcon != null) && (_loadingProgress != null)
                && (_rootFlyout != null) && (_flyoutBorder != null) && (_loadingButton != null)
                && (_dateText != null) && (_calendarView != null);

            SetValue(SelectedDateProperty, SelectedDate);
            UpdateProgressRing();
        }

        private void OnCalendarViewInitialized(object sender, RoutedEventArgs e)
        {
            _calendarView.SetSelectedDate(SelectedDate);

            UpdateBlackSelectionMode();
            UpdateOldDateTime();
        }

        private void loadingButton_Tapped(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    _rootFlyout.ShowAt(_flyoutBorder);
                });
            });
        }

        private async void ProCalendar_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedItem = sender as RTDCalendarViewToggleButton;
            if (selectedItem != null)
            {
                _dateText.Text = $"{selectedItem.DateTime.Day}/{selectedItem.DateTime.Month}/{selectedItem.DateTime.Year}";
                SelectedDate = selectedItem.DateTime;
            }

            await Task.Run(async () =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    _rootFlyout.Hide();
                    CalendarClosed?.Invoke(this, EventArgs.Empty);
                });
            });
        }

        private void ProCalendar_UnselectionChanged(object sender, RoutedEventArgs e)
        {
            _dateText.Text = "Выберите дату";
            SelectedDate = null;
        }

        public List<DateTime> EnableDates
        {
            get => (List<DateTime>)GetValue(EnableDatesProperty);
            set => SetValue(EnableDatesProperty, value);
        }

        public static readonly DependencyProperty EnableDatesProperty =
            DependencyProperty.Register(nameof(EnableDates), typeof(List<DateTime>), typeof(RTDCalendarDatePicker), new PropertyMetadata(new List<DateTime>(), OnEnableDatesChanged));

        private static void OnEnableDatesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (RTDCalendarDatePicker)d;
            if (self._calendarView == null)
                return;

            self._calendarView.EnableDates = e.NewValue as List<DateTime>;
        }

        public bool IsReady
        {
            get => (bool)GetValue(IsReadyProperty);
            set => SetValue(IsReadyProperty, value);
        }

        public static readonly DependencyProperty IsReadyProperty =
            DependencyProperty.Register(nameof(IsReady), typeof(bool), typeof(RTDCalendarDatePicker), new PropertyMetadata(true, OnIsReadyChanged));

        private static void OnIsReadyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (RTDCalendarDatePicker)d;
            self.UpdateProgressRing();
        }

        private void UpdateProgressRing()
        {
            if (_loadingProgress == null || _calendarIcon == null)
                return;

            if (IsReady)
            {
                _loadingProgress.IsActive = false;
                _calendarIcon.Visibility = Visibility.Visible;
                IsEnabled = true;
            }
            else
            {
                _loadingProgress.IsActive = true;
                _calendarIcon.Visibility = Visibility.Collapsed;
                IsEnabled = false;
            }
        }

        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register(nameof(SelectedDate), typeof(DateTime?), typeof(RTDCalendarDatePicker), new PropertyMetadata(null, OnSelectedDateChanged));

        public DateTime OldDateTime
        {
            get => (DateTime)GetValue(RedDateTimeProperty);
            set => SetValue(RedDateTimeProperty, value);
        }

        public static readonly DependencyProperty RedDateTimeProperty =
            DependencyProperty.Register(nameof(OldDateTime), typeof(DateTime), typeof(RTDCalendarDatePicker),
                new PropertyMetadata(default(DateTime), (d, e) => ((RTDCalendarDatePicker)d).UpdateOldDateTime()));

        public bool IsBlackSelectionMode
        {
            get => (bool)GetValue(IsBlackSelectionModeProperty);
            set => SetValue(IsBlackSelectionModeProperty, value);
        }

        public static readonly DependencyProperty IsBlackSelectionModeProperty =
            DependencyProperty.Register(nameof(IsBlackSelectionMode), typeof(bool), typeof(RTDCalendarDatePicker),
                new PropertyMetadata(false, (d, e) => ((RTDCalendarDatePicker)d).UpdateOldDateTime()));

        private void UpdateOldDateTime()
        {
            if (_calendarView == null) return;
            _calendarView.OldDateTime = OldDateTime;
        }

        private void UpdateBlackSelectionMode()
        {
            if (_calendarView == null) return;
            _calendarView.IsBlackSelectionMode = IsBlackSelectionMode;
        }

        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            var self = (RTDCalendarDatePicker)d;
            var dateTime = e.NewValue as DateTime?;

            if (!self._isTemplateLoaded || self.EnableDates == null)
                return;

            if (!self.EnableDates.Exists(x => x.Equals(dateTime)))
            {
                self.SetValue(SelectedDateProperty, null);
                self._dateText.Text = "Выберите дату";
                return;
            }

            self._dateText.Text = dateTime != null ? $"{dateTime.Value.Day}/{dateTime.Value.Month}/{dateTime.Value.Year}" : "Выберите дату";
        }
    }
}
