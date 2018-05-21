using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// ReSharper disable once CheckNamespace
namespace CalendarB.Controls.RTDCalendarView
{
	public sealed class AdaptiveGridView : Control
	{
		public event RoutedEventHandler SelectionChanged;

		private int _currentColumn;
		private int _currentRow;

		public AdaptiveGridView()
		{
			DefaultStyleKey = typeof(AdaptiveGridView);
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			ItemsPanelRoot = GetTemplateChild("ItemsPanelRoot") as Grid;

			UpdateColumnsCount();
			UpdateRowsCount();
			UpdateItems();
			UpdateItemWidth();
			UpdateItemHeigh();
		}

		#region Private Updating Methods
		private void UpdateColumnsCount()
		{
			if (ItemsPanelRoot == null)
				return;

			ItemsPanelRoot.ColumnDefinitions.Clear();

			for (var column = 0; column < ColumnsCount; column++)
			{
				ItemsPanelRoot.ColumnDefinitions.Add(new ColumnDefinition
				{ Width = GridLength.Auto});
			}
		}

		private void UpdateRowsCount()
		{
			if (ItemsPanelRoot == null)
				return;

			ItemsPanelRoot.RowDefinitions.Clear();

			for (var row = 0; row < RowsCount; row++)
			{
				ItemsPanelRoot.RowDefinitions.Add(new RowDefinition
				{ Height = GridLength.Auto });
			}
		}

		private void UpdateItems()
		{
			if (ItemsPanelRoot == null)
				return;

			ItemsPanelRoot.Children.Clear();

			if (Items == null)
				return;

			foreach (var item in Items)
				Add(item);
		}

		private void UpdateItemWidth()
		{
			if (ItemsPanelRoot?.Children == null)
				return;

			foreach (var child in ItemsPanelRoot.Children.OfType<FrameworkElement>())
				child.Width = ItemWidth;
		}

		private void UpdateItemHeigh()
		{
			if (ItemsPanelRoot?.Children == null)
				return;

			foreach (FrameworkElement child in ItemsPanelRoot.Children.OfType<FrameworkElement>())
				child.Height = ItemHeight;
		}
		#endregion

		#region Public Dependency Properties
		public IEnumerable<FrameworkElement> Items
		{
			get => (IEnumerable<FrameworkElement>)GetValue(ItemsProperty);
			set => SetValue(ItemsProperty, value);
		}

		public static readonly DependencyProperty ItemsProperty =
			DependencyProperty.Register(nameof(Items), typeof(IEnumerable<FrameworkElement>), typeof(AdaptiveGridView), new PropertyMetadata(null, OnItemsChanged));

		private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var adaptiveGridView = (AdaptiveGridView) d;

			adaptiveGridView?.UpdateItems();
		}

		public Grid ItemsPanelRoot
		{
			get => (Grid)GetValue(ItemsPanelRootProperty);
			private set => SetValue(ItemsPanelRootProperty, value);
		}

		public static readonly DependencyProperty ItemsPanelRootProperty =
			DependencyProperty.Register(nameof(ItemsPanelRoot), typeof(Grid), typeof(AdaptiveGridView), new PropertyMetadata(null));

		public int RowsCount
		{
			get => (int)GetValue(RowsCountProperty);
			set => SetValue(RowsCountProperty, value);
		}

		public static readonly DependencyProperty RowsCountProperty =
			DependencyProperty.RegisterAttached(nameof(RowsCount), typeof(int), typeof(AdaptiveGridView), new PropertyMetadata(0, OnRowsCountChanged));

		private static void OnRowsCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var adaptiveGridView = (AdaptiveGridView) d;

			adaptiveGridView?.UpdateRowsCount();
		}

		public int ColumnsCount
		{
			get => (int)GetValue(ColumnsCountProperty);
			set => SetValue(ColumnsCountProperty, value);
		}

		public static readonly DependencyProperty ColumnsCountProperty =
			DependencyProperty.RegisterAttached(nameof(ColumnsCount), typeof(int), typeof(AdaptiveGridView), new PropertyMetadata(0, OnColumnsCountChanged));

		private static void OnColumnsCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var adaptiveGridView = (AdaptiveGridView) d;

			adaptiveGridView?.UpdateColumnsCount();
		}

		public double ItemWidth
		{
			get => (double)GetValue(ItemWidthProperty);
			set => SetValue(ItemWidthProperty, value);
		}

		public static readonly DependencyProperty ItemWidthProperty =
			DependencyProperty.RegisterAttached(nameof(ItemWidth), typeof(double), typeof(AdaptiveGridView), new PropertyMetadata(36d, OnItemWidthChanged));

		private static void OnItemWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var adaptiveGridView = (AdaptiveGridView) d;

			adaptiveGridView.UpdateItemWidth();
		}

		public double ItemHeight
		{
			get => (double)GetValue(ItemHeightProperty);
			set => SetValue(ItemHeightProperty, value);
		}

		public static readonly DependencyProperty ItemHeightProperty =
			DependencyProperty.RegisterAttached(nameof(ItemHeight), typeof(double), typeof(AdaptiveGridView), new PropertyMetadata(36d, OnItemHeightChanged));

		private static void OnItemHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var adaptiveGridView = (AdaptiveGridView) d;

			adaptiveGridView?.UpdateItemHeigh();
		}
		#endregion

		#region Public IList Methods & Properties

		private void Add(FrameworkElement item)
		{
			if (_currentRow == RowsCount)
				return;

			if (item == null)
				return;
			if (ItemsPanelRoot?.Children == null)
				return;

			item.Width = ItemWidth;
			item.Height = ItemHeight;

			if (item is RTDCalendarViewToggleButton proCalendarToggleButton)
			{
				proCalendarToggleButton.Selected -= OnSelected;
				proCalendarToggleButton.Selected += OnSelected;

				proCalendarToggleButton.Unselected -= OnSelected;
				proCalendarToggleButton.Unselected += OnSelected;

				void OnSelected(object sender, RoutedEventArgs e) =>
					SelectionChanged?.Invoke(sender, null);
			}

			Grid.SetColumn(item, _currentColumn);
			Grid.SetRow(item, _currentRow);

			_currentColumn++;
			if (_currentColumn == ColumnsCount)
			{
				_currentColumn = 0;
				_currentRow++;
			}

			ItemsPanelRoot.Children.Add(item);
		}

		public int Count => ItemsPanelRoot?.Children?.Count ?? -1;


		public void Clear()
		{
			ItemsPanelRoot?.Children?.Clear();
		}

	
	
		#endregion
	}
}
