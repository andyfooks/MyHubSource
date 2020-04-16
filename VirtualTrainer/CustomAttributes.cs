using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    [AttributeUsage(AttributeTargets.Property| AttributeTargets.Class, AllowMultiple = false, Inherited=false)]
    public class IsRuleConfigurationParticipant : Attribute
    {

    }
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class DateFormatAttribute : System.Attribute
    {
        private string stringFormat;
        public string StringFormat
        {
            get { return this.stringFormat; }
        }
        public DateFormatAttribute(string stringFormat)
        {
            this.stringFormat = stringFormat;
        }
    }

}
