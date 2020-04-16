using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AJG.VirtualTrainer.Helper;
using System.Linq;
using System.Collections.Generic;

namespace VTTests
{
    [TestClass]
    public class UnitTest_BankHolidayHelper
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Succeeded for all years from 2013 to 2020.
            // Failed for 2012 due to Queens Jubilee - special cases!!!            
            UKBankHolidayCalculator helper = new UKBankHolidayCalculator(DateTime.Now);
            var bhDates = helper.GetBankHolidays().OrderBy(o => o.Date).ToList();

            foreach (var date in bhDates)
            {
                Console.WriteLine(date.ToString("dd MMM"));
            }

            bhDates = bhDates.Select(d => d.AddDays(1)).ToList();
            helper = new UKBankHolidayCalculator(bhDates);            

            foreach (var date in bhDates.OrderBy(o => o.Date).ToList())
            {
                Console.WriteLine(date.ToString("dd MMM"));
            }
            
            Assert.IsTrue(helper.IsBankHoliday(bhDates[0]));
            Assert.IsFalse(helper.IsBankHoliday(bhDates[0].AddDays(1)));

            Assert.IsFalse(helper.IsWorkingDay(bhDates[0]));
            Assert.IsTrue(helper.IsWorkingDay(bhDates[0].AddDays(1)));
            
           
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestPassingWrongYearToFunction()
        {
            UKBankHolidayCalculator helper = new UKBankHolidayCalculator(DateTime.Now);
            var bhDates = helper.GetBankHolidays().OrderBy(o => o.Date).ToList();
            helper.IsWorkingDay(bhDates[0].AddYears(1));
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestPassingnullYearToConstructor()
        {
            UKBankHolidayCalculator helper = new UKBankHolidayCalculator(null);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestPassingNullYearsListToConstructor()
        {
            UKBankHolidayCalculator helper = new UKBankHolidayCalculator(new List<DateTime>());
        }
    }
}
