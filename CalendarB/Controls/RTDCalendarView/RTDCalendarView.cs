using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace CalendarB.Controls.RTDCalendarView
{
	public enum CalendarViewSelectionMode
	{
		None,
		Single,
		Multiple,
		Extended
	}

    public sealed class RTDCalendarView : Control
    {
        public event RoutedEventHandler SelectionChanged;
        public event RoutedEventHandler UnselectionChanged;

        public RTDCalendarView()
        {
            DefaultStyleKey = typeof(RTDCalendarView);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            UpdateDaysOfWeekContent();
            UpdateContentTemplateRoot();

            UpdateNavigationButtons("PreviousButtonVertical", -1, i => i.SelectedIndex > 0);
            UpdateNavigationButtons("NextButtonVertical", 1, i => i.Items.Count - 1 > i.SelectedIndex);

            UpdateBlackSelectionMode();
            UpdateOldDateTime();
        }

        private void UpdateNavigationButtons(string childName, int navigatedIndex, Predicate<Selector> func)
        {
            var navigationButton = GetTemplateChild(childName) as ButtonBase;
            if (navigationButton == null) return;

            navigationButton.Click -= OnNavigationButtonClick;
            navigationButton.Click += OnNavigationButtonClick;

            void OnNavigationButtonClick(object sender, RoutedEventArgs e)
            {
                if (func.Invoke(ContentTemplateRoot))
                    ContentTemplateRoot.SelectedIndex += navigatedIndex;
            }
        }

        private void UpdateDaysOfWeekContent()
        {
            DaysOfWeekContent = GetTemplateChild("DaysOfWeekContent") as AdaptiveGridView;
            if (DaysOfWeekContent == null) return;

            DaysOfWeekContent.Clear();

            var contentDays = new List<ContentControl>
            {
                new ContentControl().GetDefaultStyle("Пн"),
                new ContentControl().GetDefaultStyle("Вт"),
                new ContentControl().GetDefaultStyle("Ср"),
                new ContentControl().GetDefaultStyle("Чт"),
                new ContentControl().GetDefaultStyle("Пт"),
                new ContentControl().GetDefaultStyle("Сб"),
                new ContentControl().GetDefaultStyle("Вс"),
            };

            DaysOfWeekContent.Items = contentDays;
        }

        private void UpdateContentTemplateRoot()
        {
            if (ContentTemplateRoot != null)
                ContentTemplateRoot.Loaded -= OnContentTemplateRootLoaded;

            ContentTemplateRoot = GetTemplateChild("ContentFlipView") as Selector;
            if (ContentTemplateRoot == null) return;
            
            ContentTemplateRoot.Items.Clear();

            var listDates = new CalendarMonths(EnableDates).Months;
            foreach (var item in listDates)
            {
                var adaptiveGridView = new AdaptiveGridView
                {
                    RowsCount = 6,
                    ColumnsCount = 7,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Items = item.Days
                };

                //adaptiveGridView.UpdateItems();+
                adaptiveGridView.SelectionChanged += OnAdaptiveGridViewSelectionChanged;
                ContentTemplateRoot.Items.Add(adaptiveGridView);
            }
            ContentTemplateRoot.Loaded += OnContentTemplateRootLoaded;
        }

        private void OnContentTemplateRootLoaded(object sender, RoutedEventArgs e)
        {
            if (!IsContentTemplateRootLoaded)
                OnLoadingUpdateChildren();

            IsContentTemplateRootLoaded = true;

            if (SelectedItem != null)
                LoadSelectedChildren(i => i.IsSelected);
            else
                LoadSelectedChildren(i => i.IsToday);
        }

        public void OnLoadingUpdateChildren()
        {
            if (ContentTemplateRoot == null) return;

            foreach (var uiElement in ContentTemplateRoot.Items)
            {
                if (!(uiElement is AdaptiveGridView adaptiveGridView)) continue;
                adaptiveGridView.SelectionChanged += OnAdaptiveGridViewSelectionChanged;
            }
        }

        private void LoadSelectedChildren(Predicate<RTDCalendarViewToggleButton> func)
        {
            int index = 0;
            foreach (var uiElement in ContentTemplateRoot.Items)
            {
                if (!(uiElement is AdaptiveGridView adaptiveGridView)) continue;
                foreach (var frameworkElement in adaptiveGridView.Items)
                {
                    if (!(frameworkElement is RTDCalendarViewToggleButton proCalendarToggleButton)) continue;
                    if (func.Invoke(proCalendarToggleButton))
                    {
                        ContentTemplateRoot.SelectedIndex = index;
                        if (proCalendarToggleButton.DateTime.Day > 20) return;
                    }
                }
                index++;
            }

            OnLoadingUpdateChildren();
        }

        private void OnAdaptiveGridViewSelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedItem = sender as RTDCalendarViewToggleButton;
            if (selectedItem == null) return;

            SelectedItem = selectedItem;

            OnSelectedChangedUpdateChildren();
            OnSelectedChangedUpdateEvents();
        }

        private void OnSelectedChangedUpdateEvents()
        {
            if (SelectedItem.IsSelected)
            {
                SelectionChanged?.Invoke(SelectedItem, null);
                Debug.WriteLine("Selected: " + SelectedItem.DateTime);
	            SelectedDate = SelectedItem.DateTime;
            }
            else
            {
                UnselectionChanged?.Invoke(SelectedItem, null);
                Debug.WriteLine("Unselected: " + SelectedItem.DateTime);
	            SelectedDate = null;
				SelectedItem = null;
            }
        }

        private void OnSelectedChangedUpdateChildren()
        {
            int index = 0;
            foreach (var uiElement in ContentTemplateRoot.Items)
            {
                if (!(uiElement is AdaptiveGridView adaptiveGridView)) continue;
                foreach (var framework in adaptiveGridView.Items)
                {
                    if (!(framework is RTDCalendarViewToggleButton proCalendarToggleButton)) continue;
                    UpdateFromSelectionMode(index, proCalendarToggleButton);
                }
                index++;
            }
        }

        private void UpdateBlackSelectionMode()
        {
            if (ContentTemplateRoot == null) return;

            foreach (var uiElement in ContentTemplateRoot.Items)
            {
                if (!(uiElement is AdaptiveGridView adaptiveGridView)) continue;
                foreach (var frameworkElement in adaptiveGridView.Items)
                {
                    if (!(frameworkElement is RTDCalendarViewToggleButton proCalendarToggleButton)) continue;
                    proCalendarToggleButton.IsBlackSelectionMode = IsBlackSelectionMode;
                }
            }
        }

        private void UpdateOldDateTime()
        {
            if (ContentTemplateRoot == null) return;

            foreach (var uiElement in ContentTemplateRoot.Items)
            {
                if (!(uiElement is AdaptiveGridView adaptiveGridView))
	                continue;

                foreach (var frameworkElement in adaptiveGridView.Items)
                {
                    if (!(frameworkElement is RTDCalendarViewToggleButton proCalendarToggleButton))
	                    continue;

                    proCalendarToggleButton.OldDateTime = proCalendarToggleButton.DateTime != OldDateTime ? default(DateTime) : OldDateTime;
                }
            }
        }

        private void UpdateSelectedDate()
        {
            if (ContentTemplateRoot == null) return;

            foreach (var uiElement in ContentTemplateRoot.Items)
            {
                if (!(uiElement is AdaptiveGridView adaptiveGridView)) continue;
                foreach (var frameworkElement in adaptiveGridView.Items)
                {
                    if (!(frameworkElement is RTDCalendarViewToggleButton proCalendarToggleButton)) continue;

                    if (!SelectedDate.HasValue)
                        proCalendarToggleButton.IsSelected = false;
                    else
                        proCalendarToggleButton.IsSelected = proCalendarToggleButton.DateTime == SelectedDate.Value;
                }
            }
        }

        private void UpdateFromSelectionMode(int index, RTDCalendarViewToggleButton proCalendarToggleButton)
        {
            switch (SelectionMode)
            {
                case CalendarViewSelectionMode.None:
                    {
                        UpdateNoneMode(index, proCalendarToggleButton);
                        break;
                    }
                case CalendarViewSelectionMode.Single:
                    {
                        UpdateSingleMode(index, proCalendarToggleButton);
                        break;
                    }
                case CalendarViewSelectionMode.Multiple:
                    {
                        UpdateMultipleMode(index, proCalendarToggleButton);
                        break;
                    }
                case CalendarViewSelectionMode.Extended:
                    {
                        UpdateExtendedMode(index, proCalendarToggleButton);
                        break;
                    }
            }
        }

        private void UpdateNoneMode(int index, RTDCalendarViewToggleButton proCalendarToggleButton)
        {
            //TODO: UpdateNoneMode();
        }

        private void UpdateSingleMode(int index, RTDCalendarViewToggleButton proCalendarToggleButton)
        {
            if (SelectedItem.IsSelected && ContentTemplateRoot.SelectedIndex == index)
                proCalendarToggleButton.IsSelected = SelectedItem.DateTime.Date == proCalendarToggleButton.DateTime.Date;
            else
                proCalendarToggleButton.IsSelected = false;
        }

        private void UpdateMultipleMode(int index, RTDCalendarViewToggleButton proCalendarToggleButton)
        {
            //TODO: UpdateMultipleMode();
        }

        private void UpdateExtendedMode(int index, RTDCalendarViewToggleButton proCalendarToggleButton)
        {
            //TODO: UpdateExtendedMode();
        }

        public List<DateTime> EnableDates
        {
            get => (List<DateTime>)GetValue(EnableDatesProperty);
            set => SetValue(EnableDatesProperty, value);
        }

        public static readonly DependencyProperty EnableDatesProperty =
            DependencyProperty.Register(nameof(EnableDates), typeof(List<DateTime>), typeof(RTDCalendarView),
                new PropertyMetadata(new List<DateTime>(), (d, e) => ((RTDCalendarView)d).UpdateContentTemplateRoot()));

        public Selector ContentTemplateRoot
        {
            get => (Selector)GetValue(ContentTemplateRootProperty);
            private set => SetValue(ContentTemplateRootProperty, value);
        }

        public static readonly DependencyProperty ContentTemplateRootProperty =
            DependencyProperty.Register(nameof(ContentTemplateRoot), typeof(Selector), typeof(RTDCalendarView),
                new PropertyMetadata(default(Selector)));

        public bool IsContentTemplateRootLoaded
        {
            get => (bool)GetValue(IsContentTemplateRootLoadedProperty);
            private set => SetValue(IsContentTemplateRootLoadedProperty, value);
        }

        public static readonly DependencyProperty IsContentTemplateRootLoadedProperty =
            DependencyProperty.Register(nameof(IsContentTemplateRootLoaded), typeof(bool), typeof(RTDCalendarView),
                new PropertyMetadata(false));

        public AdaptiveGridView DaysOfWeekContent
        {
            get => (AdaptiveGridView)GetValue(DaysOfWeekContentProperty);
            private set => SetValue(DaysOfWeekContentProperty, value);
        }

        public static readonly DependencyProperty DaysOfWeekContentProperty =
            DependencyProperty.Register(nameof(DaysOfWeekContent), typeof(AdaptiveGridView), typeof(RTDCalendarView),
                new PropertyMetadata(default(AdaptiveGridView)));

        public RTDCalendarViewToggleButton SelectedItem
        {
            get => (RTDCalendarViewToggleButton)GetValue(SelectedItemProperty);
            private set => SetValue(SelectedItemProperty, value);
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(RTDCalendarViewToggleButton), typeof(RTDCalendarView),
                new PropertyMetadata(default(RTDCalendarViewToggleButton)));

        public CalendarViewSelectionMode SelectionMode
        {
            get => (CalendarViewSelectionMode)GetValue(SelectionModeProperty);
            set => SetValue(SelectionModeProperty, value);
        }

        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(nameof(SelectionMode), typeof(CalendarViewSelectionMode), typeof(RTDCalendarView),
                new PropertyMetadata(CalendarViewSelectionMode.Single));

        public bool IsBlackSelectionMode
        {
            get => (bool)GetValue(IsBlackSelectionModeProperty);
            set => SetValue(IsBlackSelectionModeProperty, value);
        }

        public static readonly DependencyProperty IsBlackSelectionModeProperty =
            DependencyProperty.Register(nameof(IsBlackSelectionMode), typeof(bool), typeof(RTDCalendarView),
                new PropertyMetadata(false, (d, e) => ((RTDCalendarView)d).UpdateBlackSelectionMode()));

        public DateTime OldDateTime
        {
            get => (DateTime)GetValue(OldDateTimeProperty);
            set => SetValue(OldDateTimeProperty, value);
        }

        public static readonly DependencyProperty OldDateTimeProperty =
            DependencyProperty.Register(nameof(OldDateTime), typeof(DateTime), typeof(RTDCalendarView),
                new PropertyMetadata(default(DateTime), (d, e) => ((RTDCalendarView)d).UpdateOldDateTime()));

        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }
        
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register(nameof(SelectedDate), typeof(DateTime?), typeof(RTDCalendarView),
                new PropertyMetadata(null, (d, e) => ((RTDCalendarView)d).UpdateSelectedDate()));
    }
}
