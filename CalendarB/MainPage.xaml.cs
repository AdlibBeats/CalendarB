using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace CalendarB
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

	        var dates = new List<DateTime>();
	        for (int month = 1; month <= 12; month++)
	        for (int day = 1; day <= DateTime.DaysInMonth(DateTime.Now.Year, month); day++)
		        dates.Add(new DateTime(DateTime.Now.Year, month, day));

	        ProductCalendarView.EnableDates = dates.Where(x => x.Day % 2 == 0).ToList();
	        ServiceCalendarView.EnableDates = dates.Where(x => x.Day % 2 == 0).ToList();
		}
    }
}
