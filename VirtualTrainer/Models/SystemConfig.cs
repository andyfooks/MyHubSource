using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace VirtualTrainer
{
    public class SystemConfig : VirtualTrainerBase
    {
        public int Id { get; set; }
        [Required]
        public string GroupName { get; set; }
        [Required]
        public string Key { get; set; }
        public string value { get; set; }    
        public DateTime TimeStamp { get; set; }  
        
        public enum ConfigKeys
        { 
            ADHierachy,
            TechTimeSheetDropDownVals,
            TechTimeSheetMonthData,
            TechTimeSheetUserTemplate,
            LocationFieldRequired,
            TimeSheetApprovalRequired         
        }  
    }
}
