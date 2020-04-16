using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace AJG.VirtualTrainer.Console
{
    public class WebMethodsToCallSection : ConfigurationSection
    {
        /// <summary>
        /// The name of this section in the app.config.
        /// </summary>
        public const string SectionName = "WebMethodsToCallSection";

        private const string CollectionName = "Urls";

        [ConfigurationProperty(CollectionName)]
        [ConfigurationCollection(typeof(UrlCollection), AddItemName = "add")]
        public UrlCollection Urls { get { return (UrlCollection)base[CollectionName]; } }
    }
    public class UrlCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new UrlElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((UrlElement)element).Name;
        }
    }

    public class UrlElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }
        [ConfigurationProperty("TargetEnvironment", IsRequired = true)]
        public string TargetEnvironment
        {
            get { return (string)this["TargetEnvironment"]; }
            set { this["TargetEnvironment"] = value; }
        }
        [ConfigurationProperty("url", IsRequired = true)]
        public string Url
        {
            get { return (string)this["url"]; }
            set { this["url"] = value; }
        }
    }
    /// <summary>
    /// 
    /// </summary>
	public class WebMethodToCall
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string TargetEnvironment { get; set; }

        public bool IsValid { get { return !string.IsNullOrWhiteSpace(Name); } }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class WebMethodsToCall
    {
        /// <summary>
        /// A list of known Connection Environments.
        /// </summary>
        public static IEnumerable<WebMethodToCall> WebMethodList { get { foreach (var webMethod in _webMethodList) { yield return webMethod; } } }
        private static readonly List<WebMethodToCall> _webMethodList = new List<WebMethodToCall>();

        /// <summary>
        /// Constructor.
        /// </summary>
        static WebMethodsToCall()
        {
            var customSection = ConfigurationManager.GetSection(WebMethodsToCallSection.SectionName) as WebMethodsToCallSection;
            if (customSection != null)
            {
                foreach (UrlElement element in customSection.Urls)
                {
                    var returnValue = new WebMethodToCall() { Name = element.Name, Url = element.Url, TargetEnvironment = element.TargetEnvironment };
                    AddWebMethod(returnValue);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webMethod"></param>
        public static void AddWebMethod(WebMethodToCall webMethod)
        {
            if (webMethod == null || !webMethod.IsValid)
                return;

            if (!_webMethodList.Contains(webMethod))
                _webMethodList.Add(webMethod);
        }
    }
}
