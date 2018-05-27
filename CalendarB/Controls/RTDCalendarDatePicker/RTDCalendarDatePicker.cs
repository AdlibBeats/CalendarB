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

        public RTDCalendarDatePicker()
        {
            DefaultStyleKey = typeof(RTDCalendarDatePicker);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_loadingButton != null)
                _loadingButton.Tapped -= loadingButton_Tapped;

            if (_calendarView != null)
            {
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
                _loadingButton.Tapped += loadingButton_Tapped;

            if (_calendarView != null)
            {
                _calendarView.SelectionChanged += ProCalendar_SelectionChanged;
                _calendarView.UnselectionChanged += ProCalendar_UnselectionChanged;

                _calendarView.EnableDates = EnableDates;
            }

            SetValue(SelectedDateProperty, SelectedDate);
            UpdateProgressRing();
        }

        private void loadingButton_Tapped(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
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

        private void UpdateEnableDates()
        {
            if (_calendarView == null) return;
            _calendarView.EnableDates = EnableDates;
        }

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

        private void UpdateSelectedDate()
        {
            if (_dateText == null || EnableDates == null) return;
            if (!EnableDates.Exists(x => x.Equals(SelectedDate)))
            {
                SetValue(SelectedDateProperty, null);
                _dateText.Text = "Выберите дату";
                return;
            }

            _dateText.Text = SelectedDate != null ? $"{SelectedDate.Value.Day}/{SelectedDate.Value.Month}/{SelectedDate.Value.Year}" : "Выберите дату";
        }

        private void UpdateProgressRing()
        {
            if (_loadingProgress == null || _calendarIcon == null) return;

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

        public List<DateTime> EnableDates
        {
            get => (List<DateTime>)GetValue(EnableDatesProperty);
            set => SetValue(EnableDatesProperty, value);
        }

        public static readonly DependencyProperty EnableDatesProperty =
            DependencyProperty.Register(nameof(EnableDates), typeof(List<DateTime>), typeof(RTDCalendarDatePicker),
                new PropertyMetadata(new List<DateTime>(), (d, e) => ((RTDCalendarDatePicker)d).UpdateEnableDates()));

        public bool IsReady
        {
            get => (bool)GetValue(IsReadyProperty);
            set => SetValue(IsReadyProperty, value);
        }

        public static readonly DependencyProperty IsReadyProperty =
            DependencyProperty.Register(nameof(IsReady), typeof(bool), typeof(RTDCalendarDatePicker),
                new PropertyMetadata(true, (d, e) => ((RTDCalendarDatePicker)d).UpdateProgressRing()));

        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register(nameof(SelectedDate), typeof(DateTime?), typeof(RTDCalendarDatePicker),
                new PropertyMetadata(null, (d, e) => ((RTDCalendarDatePicker)d).UpdateSelectedDate()));

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
                new PropertyMetadata(false, (d, e) => ((RTDCalendarDatePicker)d).UpdateBlackSelectionMode()));
    }
}
