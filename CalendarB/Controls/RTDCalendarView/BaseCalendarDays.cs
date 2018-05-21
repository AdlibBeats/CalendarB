using System;
using System.Collections.Generic;
using System.Linq;

namespace CalendarB.Controls.RTDCalendarView
{
	public abstract class BaseCalendarDays<T> where T : RTDCalendarViewToggleButton, new()
	{
		protected BaseCalendarDays(T currentDay, params DateTime[] blackoutDays)
		{
			CurrentDay = currentDay;

			if (blackoutDays != null && blackoutDays.Any())
				BlackoutDays = new List<DateTime>(blackoutDays);

			Initialize();
		}

		private void Initialize()
		{
			BaseDays = new List<T>();

			int countDays = DateTime.DaysInMonth(CurrentDay.DateTime.Year, CurrentDay.DateTime.Month);
			for (int day = 1; day <= countDays; day++)
			{
				var dateTime = new DateTime(CurrentDay.DateTime.Year, CurrentDay.DateTime.Month, day);

				var dateTimeModel = new T()
				{
					DateTime = dateTime,
					IsWeekend = GetIsWeekend(dateTime),
					IsBlackout = GetIsBlackout(dateTime),
					IsSelected = CurrentDay.IsSelected,
					IsDisabled = CurrentDay.IsDisabled,
					IsToday = GetIsToday(dateTime)
				};

				BaseDays.Add(dateTimeModel);
			}
		}

		public virtual bool GetIsBlackout(DateTime dateTime)
		{
			if (BlackoutDays == null || !BlackoutDays.Any())
				return true;

			var blackoutDay =
				BlackoutDays.FirstOrDefault(i =>
					i.Year == dateTime.Year &&
					i.Month == dateTime.Month &&
					i.Day == dateTime.Day);

			return blackoutDay.Year == 0001;
		}

		public virtual bool GetIsWeekend(DateTime dateTime) =>
			dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday;

		public virtual bool GetIsToday(DateTime dateTime) =>
			dateTime.Year == DateTime.Now.Year &&
			dateTime.Month == DateTime.Now.Month &&
			dateTime.Day == DateTime.Now.Day;

		public IEnumerable<DateTime> BlackoutDays { get; set; }

		public T CurrentDay { get; set; }

		public IList<T> BaseDays { get; set; }
	}
}
