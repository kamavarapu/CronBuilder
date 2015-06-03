using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Quartz;

namespace CronBuilder
{
    public class CronBuilder
    {
        private int _hour;
        private int _minute;
        private string _cronExpression;
        private IntervalUnit _intervalUnit;
        private int _repeat;
        private IEnumerable<DayOfWeek> _days;
        private int? _dateOfMonth;
        private WeekOfMonth _weekOfMonth;
        private DayOfWeek _dayOfMonth;
        private Month _month;

        public CronBuilder()
        {
        }

        public CronBuilder OccurringAt(int hour, int minute)
        {
            Contract.Assert(hour >= 0 && hour <= 23);
            Contract.Assert(minute >= 0 && minute <= 59);

            _hour = hour;
            _minute = minute;

            return this;
        }

        public CronBuilder WithInterval(IntervalUnit intervalUnit)
        {
            Contract.Assert(intervalUnit == IntervalUnit.Day ||
                            intervalUnit == IntervalUnit.Week ||
                            intervalUnit == IntervalUnit.Month ||
                            intervalUnit == IntervalUnit.Year);

            _intervalUnit = intervalUnit;
            return this;
        }

        public CronBuilder Daily()
        {
            _intervalUnit = IntervalUnit.Day;
            return this;
        }

        public CronBuilder Weekly()
        {
            _intervalUnit = IntervalUnit.Week;
            return this;
        }

        public CronBuilder Monthly()
        {
            _intervalUnit = IntervalUnit.Month;
            return this;
        }

        public CronBuilder Yearly()
        {
            _intervalUnit = IntervalUnit.Year;
            return this;
        }

        public CronBuilder RepeatEvery(int repeat)
        {
            Contract.Assert(_intervalUnit == IntervalUnit.Day ||
                            _intervalUnit == IntervalUnit.Week ||
                            _intervalUnit == IntervalUnit.Month);
            Contract.Assert(repeat > 0);

            _repeat = repeat;
            return this;
        }

        public CronBuilder WeekdaysOnly()
        {
            Contract.Assert(_intervalUnit == IntervalUnit.Day);

            _days = new List<DayOfWeek>
                    {
                        DayOfWeek.Monday, 
                        DayOfWeek.Tuesday, 
                        DayOfWeek.Wednesday, 
                        DayOfWeek.Thursday, 
                        DayOfWeek.Friday
                    };
            return this;
        }

        public CronBuilder OnDays(IEnumerable<DayOfWeek> days)
        {
            Contract.Assert(_intervalUnit == IntervalUnit.Day || _intervalUnit == IntervalUnit.Week);

            _days = days;
            return this;
        }

        public CronBuilder RepeatMonthlyBy(int dateOfMonth)
        {
            Contract.Assert(_intervalUnit == IntervalUnit.Month);

            _dateOfMonth = dateOfMonth;
            return this;
        }

        public CronBuilder RepeatMonthlyBy(WeekOfMonth week, DayOfWeek day)
        {
            Contract.Assert(_intervalUnit == IntervalUnit.Month);

            _weekOfMonth = week;
            _dayOfMonth = day;
            return this;
        }

        public CronBuilder RepeatYearlyBy(Month month, int date)
        {
            Contract.Assert(_intervalUnit == IntervalUnit.Year);

            _month = month;
            _dateOfMonth = date;
            return this;
        }

        public CronBuilder RepeatYearlyBy(WeekOfMonth week, DayOfWeek day, Month month)
        {
            Contract.Assert(_intervalUnit == IntervalUnit.Year);

            _weekOfMonth = week;
            _dayOfMonth = day;
            _month = month;
            return this;
        }

        public string GetCronExpression()
        {
            Contract.Assert(_intervalUnit == IntervalUnit.Day ||
                            _intervalUnit == IntervalUnit.Week ||
                            _intervalUnit == IntervalUnit.Month || 
                            _intervalUnit == IntervalUnit.Year);

            switch (_intervalUnit)
            {
                case IntervalUnit.Day:
                    Contract.Assert(_repeat > 0);
                    Contract.Assert(_hour >= 0 && _hour <= 23);
                    Contract.Assert(_minute >= 0 && _minute <= 59);

                    _cronExpression = string.Format("0 {0} {1} 1/{2} * ? *", _minute, _hour, _repeat);

                    break;
                case IntervalUnit.Week:
                    break;
                case IntervalUnit.Month:
                    break;
                case IntervalUnit.Year:
                    break;

                default:
                    throw new Exception(string.Format("Interval {0} is not supported.", _intervalUnit));
            }

            return _cronExpression;
        }

        public IEnumerable<DateTime> OccurancesBetween(DateTime startDateTime, DateTime endDateTime)
        {
            Contract.Assert(_intervalUnit == IntervalUnit.Day ||
                            _intervalUnit == IntervalUnit.Week ||
                            _intervalUnit == IntervalUnit.Month ||
                            _intervalUnit == IntervalUnit.Year);
            Contract.Assert(!string.IsNullOrWhiteSpace(_cronExpression));
            Contract.Assert(startDateTime < endDateTime);

            var occurances = new List<DateTime>();
            var tempStart = startDateTime;
            var cronExpression = new CronExpression(_cronExpression);

            switch (_intervalUnit)
            {
                case IntervalUnit.Day:
                    for (var i = 0; i < endDateTime.Subtract(startDateTime).Days; i++)
                    {
                        var nextOccurance = cronExpression.GetNextValidTimeAfter(tempStart);

                        if (!nextOccurance.HasValue) throw new Exception("nextOccuranceStart is null");

                        if (nextOccurance > endDateTime)
                            break;

                        occurances.Add(nextOccurance.Value.LocalDateTime);

                        tempStart = nextOccurance.Value.LocalDateTime;
                    }

                    break;
                case IntervalUnit.Week:
                    break;
                case IntervalUnit.Month:
                    break;
                case IntervalUnit.Year:
                    break;

                default:
                    throw new Exception(string.Format("Interval {0} is not supported.", _intervalUnit));
            }

            return occurances;
        }

        public DateTime GetNextValidTimeAfter(DateTime startDateTime)
        {
            Contract.Assert(!string.IsNullOrWhiteSpace(_cronExpression));
            var cronExpression = new CronExpression(_cronExpression);
            var nextOccurance = cronExpression.GetNextValidTimeAfter(startDateTime);

            if (!nextOccurance.HasValue)
                throw new Exception("nextOccuranceStart is null");

            return nextOccurance.Value.LocalDateTime;
        }
    }

    public enum WeekOfMonth
    {
        FIRST = 1,
        SECOND = 2,
        THIRD = 3,
        FOURTH = 4,
        LAST = 5
    }

    public enum Month
    {
        JAN = 1,
        FEB = 2,
        MAR = 3,
        APR = 4,
        MAY = 5,
        JUN = 6,
        JUL = 7,
        AUG = 8,
        SEP = 9,
        OCT = 10,
        NOV = 11,
        DEC = 12
    }
}
