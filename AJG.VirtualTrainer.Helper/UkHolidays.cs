using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJG.VirtualTrainer.Helper
{
    public class UKBankHolidayCalculator
    {
        #region [ Private ]

        private List<DateTime> bankHolidays = new List<DateTime>();
        private DateTime constructorCalculationYear = new DateTime();
                
        #endregion

        #region [ Constructors ]

        public UKBankHolidayCalculator(List<DateTime> bankHolidays)
        {
            // All dates must be for the smae year.
            if (bankHolidays != null && bankHolidays.Count != 0 && bankHolidays.GroupBy(b=>b.Year).Count() == 1)
            {
                this.bankHolidays = bankHolidays;
                this.constructorCalculationYear = this.bankHolidays[0];
            }
            else
            {
                throw new ArgumentOutOfRangeException("bankHolidays", "Value must not be null, must have at least one date and all dates should be for the same year.");
            }
        }

        public UKBankHolidayCalculator(DateTime calculationYear)
        {
            if (calculationYear != null)
            {
                this.constructorCalculationYear = calculationYear;
                this.bankHolidays = GetBankHolidays();
            }
            else
            {
                throw new ArgumentOutOfRangeException("","");
            }
        }

        #endregion

        public bool IsWeekend(DateTime currentDateTime)
        {
            if (currentDateTime.Year == this.bankHolidays[0].Year)
            {
                return currentDateTime.DayOfWeek == DayOfWeek.Saturday || currentDateTime.DayOfWeek == DayOfWeek.Sunday ? true : false;
            }
            else
            {
                throw new ArgumentOutOfRangeException("currentDateTime", "The passed year must match the year of the date passed in to the contructor.");
            }
        }
        public bool IsBankHoliday(DateTime currentDateTime)
        {
            if (currentDateTime.Year == this.bankHolidays[0].Year)
            {
                return GetBankHolidays().Where(d => d.Date == currentDateTime.Date).Any();
            }
            else
            {
                throw new ArgumentOutOfRangeException("currentDateTime", "The passed year must match the year of the date passed in to the contructor.");
            }
        }
        public bool IsWorkingDay(DateTime currentDateTime)
        {
            if (currentDateTime.Year == this.bankHolidays[0].Year)
            {
                if (!IsWorkingDay(currentDateTime.DayOfWeek))
                {
                    return false;
                }
                if(this.bankHolidays.Where(s=>s.Date == currentDateTime.Date).Any())
                {
                    return false;
                }
                return true;
            }
            else
            {
                throw new ArgumentOutOfRangeException("currentDateTime", "The passed year must match the year of the date passed in to the contructor.");
            }            
        }
        public List<DateTime> GetBankHolidays()
        {
            if (this.bankHolidays == null || this.bankHolidays.Count == 0)
            {
                this.bankHolidays = new List<DateTime>();

                bankHolidays.Add(GetJanBankHolidayMonday(this.constructorCalculationYear)); //1
                bankHolidays.AddRange(GetEasterBankHolidays(this.constructorCalculationYear));//2
                bankHolidays.AddRange(GetMayBankHolidayMondays(this.constructorCalculationYear));//2
                bankHolidays.Add(GetAugustBankHolidayMonday(this.constructorCalculationYear)); //1
                bankHolidays.AddRange(GetDecemberBankHolidays(this.constructorCalculationYear)); //2
            }
            return this.bankHolidays;
        }        
        private DateTime GetJanBankHolidayMonday(DateTime currentDateTime)
        {
            // January bank holiday falls on the first working day after New Year's day,
            // which is usually January 1st itself.
            DateTime newYearsDay = new DateTime(currentDateTime.Year, 01, 01);
            DateTime bankHoliday = newYearsDay;
            while (IsWorkingDay(bankHoliday.DayOfWeek) == false)
            {
                bankHoliday = bankHoliday.AddDays(1);
            }
            return bankHoliday;
        }
        private List<DateTime> GetEasterBankHolidays(DateTime currentDateTime)
        {
            List<DateTime> returnDates = new List<DateTime>();
            DateTime easterSunday = GetEasterSunday(currentDateTime.Year);
            // Easter monday
            returnDates.Add(easterSunday.AddDays(1));
            // Good friday
            returnDates.Add(easterSunday.AddDays(-2));

            return returnDates;
        }
        public DateTime GetEasterSunday(int year)
        {
            // From http://stackoverflow.com/a/2510411/21574
            int day = 0;
            int month = 0;

            int g = year % 19;
            int c = year / 100;
            int h = (c - (int)(c / 4) - (int)((8 * c + 13) / 25) + 19 * g + 15) % 30;
            int i = h - (int)(h / 28) * (1 - (int)(h / 28) * (int)(29 / (h + 1)) * (int)((21 - g) / 11));

            day = i - ((year + (int)(year / 4) + i + 2 - c + (int)(c / 4)) % 7) + 28;
            month = 3;

            if (day > 31)
            {
                month++;
                day -= 31;
            }

            return new DateTime(year, month, day);
        }
        private List<DateTime> GetMayBankHolidayMondays(DateTime currentDateTime)
        {
            List<DateTime> returnDates = new List<DateTime>();

            // The first Monday of May is a bank holiday (May day)
            DateTime firstMayBankHoliday = new DateTime(currentDateTime.Year, 05, 01);
            while (firstMayBankHoliday.DayOfWeek != DayOfWeek.Monday)
            {
                firstMayBankHoliday = firstMayBankHoliday.AddDays(1);
            }
            returnDates.Add(firstMayBankHoliday);

            // The last Monday of May is a bank holiday (Spring bank holiday)
            DateTime lastMayBankHoliday = new DateTime(currentDateTime.Year, 05, 31);
            while (lastMayBankHoliday.DayOfWeek != DayOfWeek.Monday)
            {
                lastMayBankHoliday = lastMayBankHoliday.AddDays(-1);
            }
            returnDates.Add(lastMayBankHoliday);

            return returnDates;
        }
        private DateTime GetAugustBankHolidayMonday(DateTime currentDateTime)
        {
            DateTime augustBankHoliday = new DateTime(currentDateTime.Year, 08, 31);
            while (augustBankHoliday.DayOfWeek != DayOfWeek.Monday)
            {
                augustBankHoliday = augustBankHoliday.AddDays(-1);
            }
            return augustBankHoliday;
        }

        private List<DateTime> GetDecemberBankHolidays(DateTime currentDateTime)
        {
            List<DateTime> returnDates = new List<DateTime>();
            
            // CD = 25. if sat then BH = 24th, if sunday then BH = tuesday (because Boxing day will be on the monday)
            DateTime ChristmsDay = new DateTime(currentDateTime.Year, 12, 25);
            if (ChristmsDay.DayOfWeek == DayOfWeek.Friday)
            {                
                returnDates.Add(ChristmsDay);
                // Boxing day will fall on Saturday and so will carry over to Monday
                returnDates.Add(ChristmsDay.AddDays(3));
            }
            else if(ChristmsDay.DayOfWeek == DayOfWeek.Saturday)
            {
                returnDates.Add(ChristmsDay.AddDays(-1));
                // Boxing day must be sunday then so will carry over to Monday
                returnDates.Add(ChristmsDay.AddDays(2));
            }
            else if(ChristmsDay.DayOfWeek == DayOfWeek.Sunday)
            {                 
                returnDates.Add(ChristmsDay.AddDays(2));
                // Boxing day will fall on Monday
                returnDates.Add(ChristmsDay.AddDays(1));
            }
            else
            {
                returnDates.Add(ChristmsDay);
                // Boxing day is the next day
                returnDates.Add(ChristmsDay.AddDays(1));
            }

            return returnDates;
        }

        public bool IsWorkingDay(DayOfWeek dayOfWeek)
        {
            if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
                return false;

            return true;
        }        
    }
}