using System;
using Windows.UI.Xaml.Data;

namespace CalendarB.Controls.RTDCalendarView
{
	public class RTDCalendarViewDateTimeToMonth : IValueConverter
	{
        public object Convert(object value, Type targetType, object parameter, string language) =>
            ((DateTime)value).ToString("Y");

		public object ConvertBack(object value, Type targetType, object parameter, string language) =>
			string.IsNullOrEmpty(value as string) ? DateTime.MinValue : DateTime.Parse((string) value);
	}
}
