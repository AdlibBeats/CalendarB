using System;
using System.Collections.Generic;

namespace CalendarB.Controls.RTDCalendarView
{
	public sealed class CalendarDays<T> : BaseCalendarDays<T> where T : RTDCalendarViewToggleButton, new()
	{
		public int ContentDaysCapacity => 42;

		public CalendarDays() : this(new T()) { }

		public CalendarDays(T dateTimeModel, params DateTime[] blackoutDays) : base(dateTimeModel, blackoutDays)
		{
			Initialize();
		}

		private void Initialize()
		{
			Days = new List<T>();

			int dayOfWeek = (int)CurrentDay.DateTime.DayOfWeek;

			var count = (dayOfWeek == 0) ? 6 : dayOfWeek - 1;
			if (count != 0)
				AddRemainingDates(count, CurrentDay.DateTime.AddDays(-count));

			foreach (var dateTimeModel in BaseDays)
				Days.Add(dateTimeModel);

			count = ContentDaysCapacity - Days.Count;
			AddRemainingDates(count, CurrentDay.DateTime.AddMonths(1));
		}

		private void AddRemainingDates(int daysCount, DateTime remainingDateTime)
		{
			for (int i = 0; i < daysCount; i++)
			{
                var dateTimeModel = new T
                {
                    DateTime = remainingDateTime,
                    //IsWeekend = GetIsWeekend(remainingDateTime),
                    IsBlackout = GetIsBlackout(remainingDateTime),
                    IsSelected = CurrentDay.IsSelected,
                    //IsBlackSelected = CurrentDay.IsBlackSelected,
                    IsBlackSelectionMode = CurrentDay.IsBlackSelectionMode,
					IsDisabled = CurrentDay.IsDisabled,
                    IsHidden = true,
					IsToday = GetIsToday(remainingDateTime)
				};

				Days.Add(dateTimeModel);
				remainingDateTime = remainingDateTime.AddDays(1);
			}
		}

		public IList<T> Days { get; set; }
	}
}
