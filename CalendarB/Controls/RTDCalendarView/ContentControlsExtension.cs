using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CalendarB.Controls.RTDCalendarView
{
	public static class ContentControlsExtension
	{
		public static ContentControl GetDefaultStyle(this ContentControl value, string content = null) => new ContentControl
		{
			Content = content,
			FontSize = 12,
            Foreground = new SolidColorBrush(Color.FromArgb(255, 114, 114, 114)),
            FontWeight = FontWeights.Medium,
			VerticalAlignment = VerticalAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalContentAlignment = VerticalAlignment.Center,
			HorizontalContentAlignment = HorizontalAlignment.Center
		};
	}
}
