using System;
using System.Collections.Generic;
using System.Linq;

namespace CalendarB.Controls.RTDCalendarView
{
	public enum CalendarMonthsLoadingType
	{
		LoadingYears,
		LoadingMonths,
		LoadingDays
	}

	public sealed class CalendarMonths
	{
		public DateTime Min { get; set; }
		public DateTime Max { get; set; }
		public CalendarMonthsLoadingType ProListDatesLoadingType { get; set; }
		public CalendarMonths(List<DateTime> blackoutDays) : this(DateTime.Now, DateTime.Now.AddMonths(3), CalendarMonthsLoadingType.LoadingMonths, blackoutDays)
		{

		}

		public CalendarMonths(DateTime min, DateTime max, CalendarMonthsLoadingType proListDatesLoadingType, List<DateTime> blackoutDays)
		{
			BlackoutDays = blackoutDays;
			ProListDatesLoadingType = proListDatesLoadingType;
			Min = min;
			Max = max;

			Initialize();
		}

		private void Initialize()
		{
			Months = new List<CalendarDays<RTDCalendarViewToggleButton>>();

			switch (ProListDatesLoadingType)
			{
				case CalendarMonthsLoadingType.LoadingYears:
					{
						LoadYears();
						break;
					}
				case CalendarMonthsLoadingType.LoadingMonths:
					{
						LoadMonths();
						break;
					}
				case CalendarMonthsLoadingType.LoadingDays:
					{
						LoadDays();
						break;
					}
			}
		}

		private void LoadYears()
		{
			for (DateTime i = Min; i <= Max;)
			{
				for (int j = 1; j <= 12; j++)
				{
					var dateTime = new DateTime(i.Year, j, 1);

					var dateTimeModel = new RTDCalendarViewToggleButton()
					{
						DateTime = dateTime,
						IsBlackout = false,
						IsSelected = false,
                        IsBlackSelectionMode = false,
						IsDisabled = false,
						IsToday = false
					};

					Months.Add(new CalendarDays<RTDCalendarViewToggleButton>(dateTimeModel));
				}
				i = i.AddYears(1);
			}
		}

		private void LoadMonths()
		{
			if (BlackoutDays == null)
				return;

			for (DateTime i = Min; i <= Max;)
			{
				var dateTime = new DateTime(i.Year, i.Month, 1);

				var dateTimeModel = new RTDCalendarViewToggleButton()
				{
					DateTime = dateTime,
					IsBlackout = true,
					IsSelected = false,
                    IsBlackSelectionMode = false,
					IsDisabled = false,
					IsToday = false
				};

				Months.Add(new CalendarDays<RTDCalendarViewToggleButton>(dateTimeModel, BlackoutDays.ToArray()));
				i = i.AddMonths(1);
			}
		}

		private void LoadDays()
		{
			//TODO: 
		}

		public IEnumerable<DateTime> BlackoutDays { get; set; }

		public IList<CalendarDays<RTDCalendarViewToggleButton>> Months { get; set; }
	}
}
