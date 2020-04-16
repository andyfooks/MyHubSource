using AJG.VirtualTrainer.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using VirtualTrainer;
using VirtualTrainer.Interfaces;
using VirtualTrainer.Models.MyHub;

namespace AJG.VirtualTrainer.Services
{
    public class MyHubService : BaseService, IDisposable
    {
        public MyHubService() : base()
        {
        }

        public MyHubService(IUnitOfWork uow) : base(uow)
        {
        }

        #region [ Mobile Phones ]        

        public List<MyHubMobilePhoneSummaryDTO> GetMobilePhoneTotalsByUserId(List<string> users, List<DateTime> ReportDates)
        {
            // Get all the users results
            var mobilePhoneDetails = (from c in _unitOfWork.GetRepository<PhoneSummaryActivity>().GetAllNoTrack()
                                      where users.Contains(c.EmployeeID)
                                      && ReportDates.Contains(c.ReportDate)
                                      group c by c.EmployeeID into g
                                      select new MyHubMobilePhoneSummaryDTO
                                      {
                                          TotalData = g.Sum(dv => (long?)dv.DataVolumeKB / 1000) ?? 0,
                                          TotalDuration = g.Sum(dv => (int?)dv.TotalDuration / 60) ?? 0,
                                          TotalUsageCharges = g.Sum(dv => (decimal?)dv.TotalUsageChargesInc / 100) ?? 0,
                                          LineRental = g.Sum(dv => (decimal?)dv.LineRentalInc / 100) ?? 0,
                                          TotalCost = g.Sum(dv => (decimal?)dv.TotalCost / 100) ?? 0,
                                          UserName = g.Key
                                      }).ToList();

            return mobilePhoneDetails;
        }
        public MyHubMobilePhoneSummaryDTO GetMobilePhoneTotal(List<string> users, List<DateTime> ReportDates)
        {
            // Get the report 
            var mobilePhoneDetails = GetMobilePhoneTotalByMonth(users, ReportDates);

            // Now flatten monthly results into one MyHubMobilePhoneSummaryDTO.
            var returnDetails = (from c in mobilePhoneDetails
                                 group c by c.UserName into g
                                 select new MyHubMobilePhoneSummaryDTO
                                 {
                                     TotalData = g.Sum(dv => (long?)dv.TotalData) ?? 0,
                                     TotalDuration = g.Sum(dv => (int?)dv.TotalDuration) ?? 0,
                                     TotalUsageCharges = g.Sum(dv => (decimal?)dv.TotalUsageCharges) ?? 0,
                                     LineRental = g.Sum(dv => (decimal?)dv.LineRental) ?? 0,
                                     TotalCost = g.Sum(dv => (decimal?)dv.TotalCost) ?? 0
                                 }).FirstOrDefault();

            return returnDetails;
        }
        
        public List<MyHubMobilePhoneSummaryDTO> GetMobilePhoneTotalByMonth(List<string> users, List<DateTime>ReportDates)
        {
            var mobilePhoneDetails = (from c in _unitOfWork.GetRepository<PhoneSummaryActivity>().GetAllNoTrack()
                                      where users.Contains(c.EmployeeID)
                                      && ReportDates.Contains(c.ReportDate)
                                      group c by c.ReportDate into g
                                      select new MyHubMobilePhoneSummaryDTO
                                      {
                                          TotalData = g.Sum(dv => (long?)dv.DataVolumeKB / 1000) ?? 0,
                                          TotalDuration = g.Sum(dv => (int?)dv.TotalDuration / 60) ?? 0,
                                          TotalUsageCharges = g.Sum(dv => (decimal?)dv.TotalUsageChargesInc / 100) ?? 0,
                                          LineRental = g.Sum(dv => (decimal?)dv.LineRentalInc / 100) ?? 0,
                                          TotalCost = g.Sum(dv => (decimal?)dv.TotalCost / 100) ?? 0,
                                          //TotalCost = (g.Sum(dv => (decimal?)dv.LineRentalInc / 100) ?? 0) + (g.Sum(dv => (decimal?)dv.TotalUsageChargesInc / 100) ?? 0),
                                          ReportDate = g.Key
                                      }).ToList();

            // Make sure there is a phone details dto for each of the passed in months even if no data retrieved from DB - to make the graph render data correctly.
            foreach(var dt in ReportDates)
            {
                if(!mobilePhoneDetails.Where(a=>a.ReportDate == dt).Any())
                {
                    mobilePhoneDetails.Add(new MyHubMobilePhoneSummaryDTO()
                    {
                        ReportDate = dt
                    });
                }
            }

            mobilePhoneDetails = mobilePhoneDetails.OrderBy(d => d.ReportDate).ToList();
            return mobilePhoneDetails;
        }

        #endregion

        #region [ Printer ]

        public List<MyHubPrinterSummaryDTO> GetPrinterTotalsByUserId(List<string> users, List<DateTime> ReportDates, int take = 10)
        {
            int BWCostPerPage = (int)Int32.Parse(ConfigurationHelper.Get(AppSettingsList.PrintCostPerPage_BlackandWhite));
            int ColourCostPerPage = (int)Int32.Parse(ConfigurationHelper.Get(AppSettingsList.PrintCostPerPage_Colour));

            // Get all the users results
            var returnDetails = (from c in _unitOfWork.GetRepository<PrinterSummaryActivity>().GetAllNoTrack()
                                      where users.Contains(c.AccountName)
                                      && ReportDates.Contains(c.ReportDate)
                                      group c by c.AccountName into g
                                      select new MyHubPrinterSummaryDTO
                                      {
                                          BWPagesPrinted = g.Sum(s => (int?)s.BWPages) ?? 0,
                                          ColourPagesPrinted = g.Sum(s => (int?)s.ColourPages) ?? 0,
                                          BWCostPerPage = BWCostPerPage,
                                          ColourCostPerPage = ColourCostPerPage,
                                          UserName = g.Key
                                      }).ToList();

            return returnDetails;
        }
        public MyHubPrinterSummaryDTO GetPrinterTotal(List<string> users, List<DateTime> ReportDates)
        {
            int BWCostPerPage = (int)Int32.Parse(ConfigurationHelper.Get(AppSettingsList.PrintCostPerPage_BlackandWhite));
            int ColourCostPerPage = (int)Int32.Parse(ConfigurationHelper.Get(AppSettingsList.PrintCostPerPage_Colour));

            // Get the report 
            var PrinterDetails = GetPrinterTotalByMonth(users, ReportDates);

            // Now flatten monthly results into one MyHubMobilePhoneSummaryDTO.
            var returnDetails = (from c in PrinterDetails
                                 group c by c.UserName into g
                                 select new MyHubPrinterSummaryDTO
                                 {
                                     BWPagesPrinted = g.Sum(s => (int?)s.BWPagesPrinted) ?? 0,
                                     ColourPagesPrinted = g.Sum(s => (int?)s.ColourPagesPrinted) ?? 0,
                                     BWCostPerPage = BWCostPerPage,
                                     ColourCostPerPage = ColourCostPerPage,
                                 }).FirstOrDefault();

            return returnDetails;
        }

        public List<MyHubPrinterSummaryDTO> GetPrinterTotalByMonth(List<string> users, List<DateTime> ReportDates)
        {
            int BWCostPerPage = (int)Int32.Parse(ConfigurationHelper.Get(AppSettingsList.PrintCostPerPage_BlackandWhite));
            int ColourCostPerPage = (int)Int32.Parse(ConfigurationHelper.Get(AppSettingsList.PrintCostPerPage_Colour));

            var printerDetails = (from c in _unitOfWork.GetRepository<PrinterSummaryActivity>().GetAllNoTrack()
                                      where users.Contains(c.AccountName)
                                      && ReportDates.Contains(c.ReportDate)
                                      group c by c.ReportDate into g
                                      select new MyHubPrinterSummaryDTO
                                      {
                                          BWPagesPrinted = g.Sum(s=>(int?)s.BWPages) ?? 0,
                                          ColourPagesPrinted = g.Sum(s => (int?)s.ColourPages) ?? 0,
                                          BWCostPerPage = BWCostPerPage,
                                          ColourCostPerPage = ColourCostPerPage,
                                          ReportDate = g.Key
                                      }).ToList();

            // Make sure there is a dto for each of the passed in months even if no data retrieved from DB - to make the graph render data correctly.
            foreach (var dt in ReportDates)
            {
                if (!printerDetails.Where(a => a.ReportDate == dt).Any())
                {
                    printerDetails.Add(new MyHubPrinterSummaryDTO()
                    {
                        ReportDate = dt
                    });
                }
            }

            printerDetails = printerDetails.OrderBy(d => d.ReportDate).ToList();
            return printerDetails;
        }

        #endregion       
    }
}
