using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace AJG.VirtualTrainer.Helper.General
{
    public class MailAddressDetails
    {
        public string Address { get; set; }
        public string DisplayName { get; set; }
        public string Host { get; set; }
        public string user { get; set; }
    }
    public static class GeneralHelper
    {
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
        /// <summary>
        /// Combine 2 or more path section together to create a well formed url string.
        /// </summary>
        /// <param name="pathParts">zero or more strings to be combined into a single forward slash seperated string.</param>
        /// <returns>single string of the pathParts seperated by a Forward slash</returns>
        public static string CombinePaths(params string[] pathParts)
        {
            string combinedPath = string.Empty;
            foreach (string pathPart in pathParts)
            {
                if (!string.IsNullOrEmpty(pathPart))
                {
                    combinedPath = string.IsNullOrEmpty(combinedPath) ? pathPart.Trim('/') : string.Format("{0}/{1}", combinedPath.Trim('/'), pathPart.Trim('/'));
                }
            }
            return combinedPath;
        }
        public class EmailMetadataFields : IDisposable
        {
            public void Dispose()
            {
                if (this.MessageStream != null)
                {
                    this.MessageStream.Dispose();
                }
                foreach (Attachment attach in this.Attachemnts)
                {
                    if (attach.FileStream != null)
                    {
                        attach.FileStream.Dispose();
                    }
                }
            }

            private List<GeneralHelper.Attachment> attachemnts = new List<GeneralHelper.Attachment>();
            public List<GeneralHelper.Attachment> Attachemnts
            {
                get { return this.attachemnts; }
                set { this.attachemnts = value; }
            }
            public string MessageConversationID { get; set; }
            public DateTime MessageRecievedDate { get; set; }
            public string MessageID { get; set; }
            public string MessageCC { get; set; }
            public string MessageFrom { get; set; }
            public string MessageTo { get; set; }
            public string MessageSubject { get; set; }
            public byte[] Message { get; set; }
            public bool MessageHasAttachments { get; set; }
            public string DisplayToValue { get; set; }
            public List<string> ToAddressesFromHeader { get; set; }
            public string TargetTeam { get; set; }
            public Stream MessageStream { get; set; }
            public string MessageBody { get; set; }
        }
        public class Attachment
        {
            public string Name { get; set; }
            public Stream FileStream { get; set; }
            public string contenttype { get; set; }
            public bool isInline { get; set; }
        }
    }
}
