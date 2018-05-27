using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace CalendarB.UserControls
{
    public sealed partial class CalendarOnContentDialog : UserControl
    {
        public CalendarOnContentDialog()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                SetEnableDates();
                SetSelectionMode();
            };
        }

        /// <summary>
        /// FirstStep
        /// </summary>
        private void SetEnableDates()
        {
            //Test
            var dates = new List<DateTime>();
            for (int month = 1; month <= 12; month++)
                for (int day = 1; day <= DateTime.DaysInMonth(DateTime.Now.Year, month); day++)
                    dates.Add(new DateTime(DateTime.Now.Year, month, day));

            calendar.EnableDates = dates.Where(x => x.Day % 2 == 0).ToList();
        }

        /// <summary>
        /// SecondStep
        /// </summary>
        private void SetSelectionMode()
        {
            //Test
            //calendar.IsBlackSelectionMode = true;

            //если указывается дата, то календарь автоматически переключается в BlackSelectionMode.
            calendar.OldDateTime = new DateTime(2018, 5, 24);

            //Reset BlackSelectionMode
            //calendar.OldDateTime = default(DateTime);
        }
    }
}
