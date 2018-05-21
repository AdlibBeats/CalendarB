using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CalendarB.Controls.RTDCalendarView
{
	public static class ContentControlsExtension
	{
		public static ContentControl GetDefaultStyle(this ContentControl value, string content = null) => new ContentControl
		{
			Content = content,
			FontSize = 16,
			VerticalAlignment = VerticalAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalContentAlignment = VerticalAlignment.Center,
			HorizontalContentAlignment = HorizontalAlignment.Center
		};
	}
}
