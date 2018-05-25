using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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

            calendar.EnableDates = dates.Where(x => x.Day % 2 == 0).ToList();

            calendar.IsBlackSelectionMode = true;
            calendar.RedDateTime = new DateTime(2018, 6, 12);
        }
    }
}
