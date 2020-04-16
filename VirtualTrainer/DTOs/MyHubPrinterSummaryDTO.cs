using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public class MyHubPrinterSummaryDTO
    {
        public int BWPagesPrinted { get; set; }
        public int ColourPagesPrinted { get; set; }
        public int BWCostPerPage { get; set; }
        public int ColourCostPerPage { get; set; }
        public int totalPeople { get; set; }
        public string UserName { get; set; }

        public decimal BWTotalCost
        {
            get { return (decimal)(BWPagesPrinted * BWCostPerPage) / 100; }
        }
        public decimal ColourTotalCost
        {
            get { return (decimal)(ColourPagesPrinted * ColourCostPerPage) / 100; }
        }
        public decimal TotalCost
        {
            get { return (BWTotalCost + ColourTotalCost); }
        }
        public int TotalPages
        {
            get { return BWPagesPrinted + ColourPagesPrinted; }
        }
        public DateTime? ReportDate { get; set; }
        public MyHubPrinterSummaryDTO()
        {
            BWPagesPrinted = 0;
            ColourPagesPrinted = 0;
            BWCostPerPage = 0;
            totalPeople = 0;
        }
        public void SetPrinterCostPerPage(int BWCost, int ColourCost)
        {
            this.BWCostPerPage = BWCost;
            this.ColourCostPerPage = ColourCost;
        }
    }
    public class MyHierachyPrinterSummaryDTO
    {
        public MyHubPrinterSummaryDTO User { get; set; }
        public MyHubPrinterSummaryDTO Team { get; set; }
        public MyHubPrinterSummaryDTO TeamMates { get; set; }
        public List<MyHubPrinterSummaryDTO> TeamMatesPrinterSummaryDTOs { get; set; }
        public List<MyHubPrinterSummaryDTO> TeamPrinterSummaryDTOs { get; set; }
        public MyHubPrinterSummaryDTO AllPeopleBelowUser { get; set; }

        #region [ Team mates ]

        public List<int> TeamMatesColourList
        {
            get
            {
                return TeamMatesPrinterSummaryDTOs.Select(s => s.ColourPagesPrinted).ToList();
            }
        }
        public List<int> TeamMatesBandPagesList
        {
            get
            {
                return TeamMatesPrinterSummaryDTOs.Select(s => s.BWPagesPrinted).ToList();
            }
        }
        public List<decimal> TeamMatesBandWCostList
        {
            get
            {
                return TeamMatesPrinterSummaryDTOs.Select(s => s.BWTotalCost).ToList();
            }
        }
        public List<decimal> TeamMatesColourCostList
        {
            get
            {
                return TeamMatesPrinterSummaryDTOs.Select(s => s.ColourTotalCost).ToList();
            }
        }
        public List<decimal> TeamMatesTotlaCostList
        {
            get
            {
                return TeamMatesPrinterSummaryDTOs.Select(s => s.TotalCost).ToList();
            }
        }
        public List<int> TeamMatesTotlaPagesList
        {
            get
            {
                return TeamMatesPrinterSummaryDTOs.Select(s => s.TotalPages).ToList();
            }
        }
        public List<string> TeamMatesUserNamesList
        {
            get
            {
                return TeamMatesPrinterSummaryDTOs.Select(s => s.UserName).ToList();
            }
        }

        #endregion

        #region [ Team L1 ]

        public List<int> TeamMembersColourList
        {
            get
            {
                return TeamPrinterSummaryDTOs.Select(s => s.ColourPagesPrinted).ToList();               
            }
        }
        public List<int> TeamMembersBandPagesList
        {
            get
            {
                return TeamPrinterSummaryDTOs.Select(s => s.BWPagesPrinted).ToList();
            }
        }
        public List<decimal> TeamMembersBandWCostList
        {
            get
            {
                return TeamPrinterSummaryDTOs.Select(s => s.BWTotalCost).ToList();
            }
        }
        public List<decimal> TeamMembersColourCostList
        {
            get
            {
                return TeamPrinterSummaryDTOs.Select(s => s.ColourTotalCost).ToList();
            }
        }
        public List<decimal> TeamMembersTotlaCostList
        {
            get
            {
                return TeamPrinterSummaryDTOs.Select(s => s.TotalCost).ToList();
            }
        }
        public List<int> TeamMembersTotlaPagesList
        {
            get
            {
                return TeamPrinterSummaryDTOs.Select(s => s.TotalPages).ToList();
            }
        }
        public List<string> TeamUSerNamesList
        {
            get
            {
                return TeamPrinterSummaryDTOs.Select(s => s.UserName).ToList();
            }
        }

        #endregion

        public MyHierachyPrinterSummaryDTO()
        {
            User = new MyHubPrinterSummaryDTO();
            Team = new MyHubPrinterSummaryDTO();
            TeamPrinterSummaryDTOs = new List<MyHubPrinterSummaryDTO>();
            AllPeopleBelowUser = new MyHubPrinterSummaryDTO();
        }
    }
}
