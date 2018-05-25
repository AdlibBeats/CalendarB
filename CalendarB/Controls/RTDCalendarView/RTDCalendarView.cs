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
        public event RoutedEventHandler Initialized;

        public RTDCalendarView()
        {
            DefaultStyleKey = typeof(RTDCalendarView);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            UpdateDaysOfWeekContent();
            UpdateContentTemplateRoot();

            UpdateNavigationButtons("PreviousButtonVertical");
            UpdateNavigationButtons("NextButtonVertical");
        }

        private void UpdateDaysOfWeekContent()
        {
            DaysOfWeekContent = GetTemplateChild("DaysOfWeekContent") as AdaptiveGridView;
            if (DaysOfWeekContent == null)
                return;

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

        public List<DateTime> EnableDates
        {
            get => (List<DateTime>)GetValue(EnableDatesProperty);
            set => SetValue(EnableDatesProperty, value);
        }

        public static readonly DependencyProperty EnableDatesProperty =
            DependencyProperty.Register(nameof(EnableDates), typeof(List<DateTime>), typeof(RTDCalendarView), new PropertyMetadata(new List<DateTime>(), OnEnableDatesChanged));

        private static void OnEnableDatesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var calendarView = (RTDCalendarView)d;
            calendarView.UpdateContentTemplateRoot();
        }

        private void UpdateContentTemplateRoot()
        {
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
                adaptiveGridView.SelectionChanged -= AdaptiveGridView_SelectionChanged;
                adaptiveGridView.SelectionChanged += AdaptiveGridView_SelectionChanged;
                ContentTemplateRoot.Items.Add(adaptiveGridView);
            }

            ContentTemplateRoot.Loaded -= ContentTemplateRoot_Loaded;
            ContentTemplateRoot.Loaded += ContentTemplateRoot_Loaded;

            ContentTemplateRoot.SelectionChanged -= ContentTemplateRoot_SelectionChanged;
            ContentTemplateRoot.SelectionChanged += ContentTemplateRoot_SelectionChanged;
        }

        private void UpdateNavigationButtons(string childName)
        {
            var navigationButton = GetTemplateChild(childName) as ButtonBase;
            if (navigationButton == null) return;

            if (ContentTemplateRoot == null) return;

            int navigationIndex = 1;
            bool isSelectedIndex = false;

            switch (childName)
            {
                case "PreviousButtonVertical":
                    navigationIndex = -1;
                    isSelectedIndex = ContentTemplateRoot.SelectedIndex > 0;
                    break;
                case "NextButtonVertical":
                    navigationIndex = 1;
                    isSelectedIndex = ContentTemplateRoot.Items.Count - 1 > ContentTemplateRoot.SelectedIndex;
                    break;
            }

            navigationButton.Click -= OnNavigationButtonClick;
            navigationButton.Click += OnNavigationButtonClick;

            void OnNavigationButtonClick(object sender, RoutedEventArgs e)
            {
                if (isSelectedIndex)
                    ContentTemplateRoot.SelectedIndex += navigationIndex;
            }
        }

        private void ContentTemplateRoot_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsContentTemplateRootLoaded)
                OnLoadingUpdateChildren();

            IsContentTemplateRootLoaded = true;
            Initialized?.Invoke(this, new RoutedEventArgs());

            if (SelectedItem != null)
                LoadSelectedChildren(i => i.IsSelected);
            else
                LoadSelectedChildren(i => i.IsToday);
        }

        public void OnLoadingUpdateChildren()
        {
            if (ContentTemplateRoot?.ItemsPanelRoot == null)
                return;

            for (int i = 0; i < ContentTemplateRoot.ItemsPanelRoot.Children.Count; i++)
            {
                var selectorItem = ContentTemplateRoot.ItemsPanelRoot.Children.ElementAtOrDefault(i) as SelectorItem;
                if (selectorItem == null)
                    continue;

                var adaptiveGridView = selectorItem.ContentTemplateRoot as AdaptiveGridView;
                if (adaptiveGridView == null)
                    continue;

                adaptiveGridView.SelectionChanged -= AdaptiveGridView_SelectionChanged;
                adaptiveGridView.SelectionChanged += AdaptiveGridView_SelectionChanged;
            }
        }

        private int _contentTemplateRoot_CurrentIndex = 0;
        private void ContentTemplateRoot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ContentTemplateRoot.SelectedIndex > -1)
            {
                OnScrollingUpdateChildren();

                _contentTemplateRoot_CurrentIndex =
                    ContentTemplateRoot.SelectedIndex;
            }
        }

        private void OnScrollingUpdateChildren()
        {
            int index = _contentTemplateRoot_CurrentIndex < ContentTemplateRoot.SelectedIndex ? -1 : 1;

            var itemsPanelRoot = GetItemsPanelRootFromIndex(ContentTemplateRoot.SelectedIndex + index);
            if (itemsPanelRoot == null)
                return;

            var currentItemsPanelRoot = GetItemsPanelRootFromIndex(ContentTemplateRoot.SelectedIndex);

            for (int i = 0; i < itemsPanelRoot.Children.Count; i++)
            {
                var proCalendarToggleButton = itemsPanelRoot.Children.ElementAtOrDefault(i) as RTDCalendarViewToggleButton;

                ScrollingUpdateChildren(currentItemsPanelRoot, proCalendarToggleButton);
            }
        }

        private void ScrollingUpdateChildren(Panel currentItemsPanelRoot, RTDCalendarViewToggleButton proCalendarToggleButton)
        {
            if (currentItemsPanelRoot == null || proCalendarToggleButton == null)
                return;

            var currentProCalendarToggleButton = currentItemsPanelRoot.Children.OfType<RTDCalendarViewToggleButton>()
                .FirstOrDefault(j => j.DateTime.Date == proCalendarToggleButton.DateTime.Date);

            if (currentProCalendarToggleButton == null)
                return;

            if (proCalendarToggleButton.IsSelected && proCalendarToggleButton.IsToday)
            {
                currentProCalendarToggleButton.IsSelected = true;
                proCalendarToggleButton.IsSelected = false;

                currentProCalendarToggleButton.IsToday = true;
                proCalendarToggleButton.IsToday = false;
            }
            else if (proCalendarToggleButton.IsSelected)
            {
                currentProCalendarToggleButton.IsSelected = true;
                proCalendarToggleButton.IsSelected = false;
            }
            else if (proCalendarToggleButton.IsToday)
            {
                currentProCalendarToggleButton.IsToday = true;
                proCalendarToggleButton.IsToday = false;
            }
            else if (proCalendarToggleButton.IsDisabled)
            {
                currentProCalendarToggleButton.IsDisabled = true;
                proCalendarToggleButton.IsDisabled = false;
            }
            else if (proCalendarToggleButton.IsBlackout)
            {
                currentProCalendarToggleButton.IsBlackout = true;
                proCalendarToggleButton.IsBlackout = false;
            }
        }

        private void LoadSelectedChildren(Predicate<RTDCalendarViewToggleButton> func)
        {
            for (int i = 0; i < ContentTemplateRoot.ItemsPanelRoot.Children.Count; i++)
            {
                var itemsPanelRoot = GetItemsPanelRootFromIndex(i);
                if (itemsPanelRoot == null) continue;

                for (int j = 0; j < itemsPanelRoot.Children.Count; j++)
                {
                    var proCalendarToggleButton = itemsPanelRoot.Children.ElementAtOrDefault(j) as RTDCalendarViewToggleButton;
                    if (proCalendarToggleButton == null) continue;

                    if (func.Invoke(proCalendarToggleButton))
                    {
                        ContentTemplateRoot.SelectedIndex = i;
                        if (proCalendarToggleButton.DateTime.Day > 20) return;
                    }

                }
            }
            OnLoadingUpdateChildren();
        }

        private void AdaptiveGridView_SelectionChanged(object sender, RoutedEventArgs e)
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
            }
            else
            {
                UnselectionChanged?.Invoke(SelectedItem, null);
                Debug.WriteLine("Unselected: " + SelectedItem.DateTime);

                SelectedItem = null;
            }
        }

        private void OnSelectedChangedUpdateChildren()
        {
            for (int i = 0; i < ContentTemplateRoot.ItemsPanelRoot.Children.Count; i++)
            {
                var itemsPanelRoot = GetItemsPanelRootFromIndex(i);
                if (itemsPanelRoot == null)
                    continue;

                for (int j = 0; j < itemsPanelRoot.Children.Count; j++)
                {
                    var proCalendarToggleButton = itemsPanelRoot.Children.ElementAtOrDefault(j) as RTDCalendarViewToggleButton;
                    if (proCalendarToggleButton == null)
                        continue;

                    UpdateFromSelectionMode(i, proCalendarToggleButton);
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

        private Panel GetItemsPanelRootFromIndex(int index)
        {
            if (ContentTemplateRoot.ItemsPanelRoot == null)
                return null;

            var selectorItem = ContentTemplateRoot.ItemsPanelRoot.Children.ElementAtOrDefault(index) as SelectorItem;
            if (selectorItem == null)
                return null;

            var adaptiveGridView = selectorItem.ContentTemplateRoot as AdaptiveGridView;
            if (adaptiveGridView == null)
                return null;

            var itemsPanelRoot = adaptiveGridView.ItemsPanelRoot as Panel;
            if (itemsPanelRoot == null)
                return null;

            return itemsPanelRoot;
        }

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
                new PropertyMetadata(false));

        public void SetBlackSelectionMode(bool isBlackSelectionMode)
        {
            IsBlackSelectionMode = isBlackSelectionMode;

            var monthPages = ContentTemplateRoot?.ItemsPanelRoot?.Children;
            if (monthPages == null) return;

            foreach (FlipViewItem monthPage in monthPages)
            {
                var month = (AdaptiveGridView)monthPage.ContentTemplateRoot;
                foreach (var day in month.Items)
                    ((RTDCalendarViewToggleButton)day).IsBlackSelectionMode = isBlackSelectionMode;
            }
        }

        public DateTime RedDateTime
        {
            get => (DateTime)GetValue(RedDateTimeProperty);
            set => SetValue(RedDateTimeProperty, value);
        }

        public static readonly DependencyProperty RedDateTimeProperty =
            DependencyProperty.Register(nameof(RedDateTime), typeof(DateTime), typeof(RTDCalendarView),
                new PropertyMetadata(default(DateTime)));

        public void SetRedDateTime(DateTime? oldDateTime)
        {
            RedDateTime = oldDateTime.HasValue ? oldDateTime.Value : default(DateTime);

            var monthPages = ContentTemplateRoot?.ItemsPanelRoot?.Children;
            if (monthPages == null) return;

            foreach (FlipViewItem monthPage in monthPages)
            {
                var month = (AdaptiveGridView)monthPage.ContentTemplateRoot;
                foreach (var day in month.Items)
                {
                    var toggleButton = (RTDCalendarViewToggleButton)day;
                        toggleButton.IsBlackSelectionMode = IsBlackSelectionMode;
                        toggleButton.OldDateTime = toggleButton.DateTime != RedDateTime ? default(DateTime) : RedDateTime;
                    
                }
            }
        }

        public void SetSelectedDate(DateTime? dateTime)
        {
            var monthPages = ContentTemplateRoot?.ItemsPanelRoot?.Children;
            if (monthPages == null) return;

            foreach (FlipViewItem monthPage in monthPages)
            {
                var month = (AdaptiveGridView)monthPage.ContentTemplateRoot;
                foreach (var day in month.Items)
                {
                    var toggleButton = (RTDCalendarViewToggleButton)day;
                    if (!dateTime.HasValue)
                        toggleButton.IsSelected = false;
                    else
                        toggleButton.IsSelected = toggleButton.DateTime == dateTime.Value;
                }
            }
        }
    }
}
