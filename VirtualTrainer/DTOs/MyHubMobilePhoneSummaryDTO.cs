using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public class MyHubMobilePhoneSummaryDTO
    {
        public int NumberOfCalls { get; set; }
        public int TotalDuration { get; set; }
        public long TotalData { get; set; }
        public decimal LineRental { get; set; }
        public decimal TotalUsageCharges { get; set; }
        public decimal TotalCost { get; set; }
        public string UserName { get; set; }
        public DateTime? ReportDate { get; set; }
        public MyHubMobilePhoneSummaryDTO()
        {
            NumberOfCalls = 0;
            TotalDuration = 0;
            TotalData = 0;
            LineRental = 0;
            TotalUsageCharges = 0;
        }
    }
    public class MyHierachyMobilePhoneSummaryDTO
    {
        public MyHubMobilePhoneSummaryDTO User { get; set; }
        public MyHubMobilePhoneSummaryDTO Team { get; set; }
        public MyHubMobilePhoneSummaryDTO TeamMates { get; set; }
        public List<MyHubMobilePhoneSummaryDTO> TeamMatesPrinterSummaryDTOs { get; set; }
        public List<MyHubMobilePhoneSummaryDTO> TeamPrinterSummaryDTOs { get; set; }
        public MyHubMobilePhoneSummaryDTO AllPeopleBelowUser { get; set; }
        public List<MyHubMobilePhoneSummaryDTO> UserSummaryDTOAllMonths { get; set; }

        //#region [ Team mates ]

        //public List<int> TeamMatesColourList
        //{
        //    get
        //    {
        //        return TeamMatesPrinterSummaryDTOs.Select(s => s.ColourPagesPrinted).ToList();
        //    }
        //}
        //public List<int> TeamMatesBandPagesList
        //{
        //    get
        //    {
        //        return TeamMatesPrinterSummaryDTOs.Select(s => s.BWPagesPrinted).ToList();
        //    }
        //}
        //public List<decimal> TeamMatesBandWCostList
        //{
        //    get
        //    {
        //        return TeamMatesPrinterSummaryDTOs.Select(s => s.BWTotalCost).ToList();
        //    }
        //}
        //public List<decimal> TeamMatesColourCostList
        //{
        //    get
        //    {
        //        return TeamMatesPrinterSummaryDTOs.Select(s => s.ColourTotalCost).ToList();
        //    }
        //}
        //public List<decimal> TeamMatesTotlaCostList
        //{
        //    get
        //    {
        //        return TeamMatesPrinterSummaryDTOs.Select(s => s.TotalCost).ToList();
        //    }
        //}
        //public List<int> TeamMatesTotlaPagesList
        //{
        //    get
        //    {
        //        return TeamMatesPrinterSummaryDTOs.Select(s => s.TotalPages).ToList();
        //    }
        //}
        //public List<string> TeamMatesUserNamesList
        //{
        //    get
        //    {
        //        return TeamMatesPrinterSummaryDTOs.Select(s => s.UserName).ToList();
        //    }
        //}

        //#endregion

        //#region [ Team L1 ]

        //public List<int> TeamMembersColourList
        //{
        //    get
        //    {
        //        return TeamPrinterSummaryDTOs.Select(s => s.ColourPagesPrinted).ToList();               
        //    }
        //}
        //public List<int> TeamMembersBandPagesList
        //{
        //    get
        //    {
        //        return TeamPrinterSummaryDTOs.Select(s => s.BWPagesPrinted).ToList();
        //    }
        //}
        //public List<decimal> TeamMembersBandWCostList
        //{
        //    get
        //    {
        //        return TeamPrinterSummaryDTOs.Select(s => s.BWTotalCost).ToList();
        //    }
        //}
        //public List<decimal> TeamMembersColourCostList
        //{
        //    get
        //    {
        //        return TeamPrinterSummaryDTOs.Select(s => s.ColourTotalCost).ToList();
        //    }
        //}
        //public List<decimal> TeamMembersTotlaCostList
        //{
        //    get
        //    {
        //        return TeamPrinterSummaryDTOs.Select(s => s.TotalCost).ToList();
        //    }
        //}
        //public List<int> TeamMembersTotlaPagesList
        //{
        //    get
        //    {
        //        return TeamPrinterSummaryDTOs.Select(s => s.TotalPages).ToList();
        //    }
        //}
        //public List<string> TeamUSerNamesList
        //{
        //    get
        //    {
        //        return TeamPrinterSummaryDTOs.Select(s => s.UserName).ToList();
        //    }
        //}

        //#endregion

        public MyHierachyMobilePhoneSummaryDTO()
        {
            User = new MyHubMobilePhoneSummaryDTO();
            Team = new MyHubMobilePhoneSummaryDTO();
            TeamPrinterSummaryDTOs = new List<MyHubMobilePhoneSummaryDTO>();
            AllPeopleBelowUser = new MyHubMobilePhoneSummaryDTO();
        }
    }
}
