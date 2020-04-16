using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public class BreachLogDTO
    {
        public int Id { get; set; }
        public string RuleName { get; set; }
        public string RuleConfigurationName { get; set; }
        public string UserName { get; set; }
        public string OfficeName { get; set; }
        public string TeamName { get; set; }
        public bool IsArchived { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime TimeStampDateOnly
        {
            get { return this.TimeStamp.Date; }
        }
        public string ContextRef { get; set; }
        public string BreachDisplayText { get; set; }
        public string ContextRefType { get; set; }
        public string BreachDisplayAlternateText { get; set; }
        public string RuleBreachFieldOne { get; set; }
        public string RuleBreachFieldTwo { get; set; }
        public string RuleBreachFieldThree { get; set; }
        public string RuleBreachFieldFour { get; set; }
        public string RuleBreachFieldFive { get; set; }
        public string RuleBreachFieldSix { get; set; }
        public string RuleBreachFieldSeven { get; set; }
        public string RuleBreachFieldEight { get; set; }
        public string RuleBreachFieldNine { get; set; }
        public string RuleBreachFieldTen { get; set; }
        public string RuleBreachFieldEleven { get; set; }
        public string RuleBreachFieldTwelve { get; set; }
        public string RuleBreachFieldThirteen { get; set; }
        public string RuleBreachFieldFourteen { get; set; }
        public string RuleBreachFieldFifteen { get; set; }
        public string RuleBreachFieldSixteen { get; set; }
        public string RuleBreachFieldSeventeen { get; set; }
        public string RuleBreachFieldEighteen { get; set; }
        public string RuleBreachFieldNineteen { get; set; }
        public string RuleBreachFieldTwenty { get; set; }
    }
}
