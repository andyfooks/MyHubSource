using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Aspose.Pdf;
using Aspose.Pdf.Generator;
using Aspose.Pdf.Text;
using Microsoft.Win32.SafeHandles;
using System.Xml;

namespace AJG.VirtualTrainer.Helper
{
    public class AsposeHelper
    {
        public enum HeaderFooterEnum
        {
            header = 1,
            footer = 2
        }

        public AsposeHelper()
        {
             Aspose.Pdf.License lic = new Aspose.Pdf.License();
             lic.SetLicense("Aspose.Pdf.lic");
        }

        #region [ Header Footer ]

        public void AddHeadersFooters(string fileFullPath, HeaderFooterEnum headerFooter,
            int? marginTop, int? marginRight, int? marginBottom, int? marginLeft,
            string headerFooterHtml = "")
        {
            Document doc = new Document(fileFullPath);
            
            Aspose.Pdf.HeaderFooter pdfheaderFooter = new Aspose.Pdf.HeaderFooter();

            HtmlFragment headerFooterHtmlFragment = new HtmlFragment(headerFooterHtml);

            // Set margins
            pdfheaderFooter.Margin = new Aspose.Pdf.MarginInfo()
            {
                Top = marginTop.GetValueOrDefault(),
                Bottom = marginBottom.GetValueOrDefault(),
                Left = marginLeft.GetValueOrDefault(),
                Right = marginRight.GetValueOrDefault(),
            };

            pdfheaderFooter.Paragraphs.Add(headerFooterHtmlFragment);

            for (int cnt = 1; cnt <= doc.Pages.Count; cnt++)
            {
                if (!string.IsNullOrEmpty(headerFooterHtml))
                {
                    switch(headerFooter)
                    {
                        case HeaderFooterEnum.footer:
                            doc.Pages[cnt].Footer = pdfheaderFooter;
                            break;
                        case HeaderFooterEnum.header:
                            doc.Pages[cnt].Header = pdfheaderFooter;
                            break;
                    }
                }
            }

            doc.Save(fileFullPath);
        }

        #endregion

        #region [ Convert PDF to Word ]

        public void ConvertPDFtoWord(string dir, string pdfFileName)
        {
            var pdf = new Aspose.Pdf.Document(dir + pdfFileName);
            // save in different formats
            pdf.Save(dir + "output.docx", Aspose.Pdf.SaveFormat.DocX);
            //pdf.Save(dir + "output.pptx", Aspose.Pdf.SaveFormat.Pptx);
            //pdf.Save(dir + "output.html", Aspose.Pdf.SaveFormat.Html);
        }

        #endregion

        #region [ Save html content to pdf ]

        public void SaveHtmlToPDFFile(string pdfcontent, string filePath)
        {
            SaveHtmlToPDFFile(pdfcontent, filePath, null, null);
        }
        public void SaveHtmlToPDFFile(string pdfcontent, string filePath, int? pageHeight, int? pageWidth)
        {
            //Create pdf document
            SaveHtmlToPDFFile(pdfcontent, filePath, pageHeight, pageWidth, null, null, null, null);
        }
        public void SaveHtmlToPDFFile(string pdfcontent, string filePath, 
            int? pageHeight, int? pageWidth, 
            int? marginTop, int? marginRight, int? marginBottom, int? marginLeft)
        {
            //Create pdf document
            Aspose.Pdf.Generator.Pdf pdf = new Aspose.Pdf.Generator.Pdf();

            // Set Page Dimensions
            if (pageWidth != null && pageWidth > 0)
            {
                pdf.PageSetup.PageWidth = pageWidth.GetValueOrDefault();
            }
            if (pageHeight != null && pageHeight > 0)
            {
                pdf.PageSetup.PageHeight = pageHeight.GetValueOrDefault();
            }

            int parsedMarginInt = 0;

            // Set Page margins
            if (marginTop != null && int.TryParse(marginTop.ToString(), out parsedMarginInt))
            {
                pdf.PageSetup.Margin.Top = marginTop.GetValueOrDefault();
            }
            if (marginBottom != null && int.TryParse(marginBottom.ToString(), out parsedMarginInt))
            {
                pdf.PageSetup.Margin.Bottom = marginBottom.GetValueOrDefault();
            }
            if (marginLeft != null && int.TryParse(marginLeft.ToString(), out parsedMarginInt))
            {
                pdf.PageSetup.Margin.Left = marginLeft.GetValueOrDefault();
            }
            if (marginTop != null && int.TryParse(marginTop.ToString(), out parsedMarginInt))
            {
                pdf.PageSetup.Margin.Right = marginRight.GetValueOrDefault();
            }

            pdf.BindHTML(pdfcontent);

            //Save the document
            pdf.Save(filePath);
        }

        #endregion

        #region [ Aspose Templates ]

        /// <summary>
        /// This will load the Aspose PDF xml template document, find the first "Section" (there should be only 1) and then loop through the passed in fieldNameValuePairs
        /// finding the pdf template element with an id that matches the key and sets the value to the value of the keyValuePair.
        /// </summary>
        /// <param name="xmlTemplatePath">Path to an aspose.pdf xml template: this must meet the syntax as defined by the Aspose.PDF xsd document.</param>
        /// <param name="fieldNameValuePairs">a list of field value pairs where the key is the name of an id of an element in the xml template and the value of the keyvaluepair is the value.</param>
        /// <returns>The PDF as a memory stream, which then can be saved wherever.</returns>
        public MemoryStream PopulateAsposeXmlTemplate(string xmlTemplatePath, List<KeyValuePair<string, string>> fieldNameValuePairs)
        {
            if (File.Exists(xmlTemplatePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlTemplatePath);
                return PopulateAsposeXmlTemplate(doc, fieldNameValuePairs);
            }
            else
            {
                throw new FileNotFoundException(string.Format("The Aspose.PDF xml Template does not exist at: {0}", xmlTemplatePath));
            }
        }

        /// <summary>
        /// This will use the passed xmldocument pdf xml template, find the first "Section" (there should be only 1) and then loop through the passed in fieldNameValuePairs
        /// finding the pdf template element with an id that matches the key and sets the value to the value of the keyValuePair.
        /// </summary>
        /// <param name="xmlTemplate">XmlDocument</param>
        /// <param name="fieldNameValuePairs"></param>
        /// <returns></returns>
        public MemoryStream PopulateAsposeXmlTemplate(XmlDocument xmlTemplate, List<KeyValuePair<string, string>> fieldNameValuePairs)
        {
            Aspose.Pdf.Generator.Pdf pdf1 = new Aspose.Pdf.Generator.Pdf();
            pdf1.BindXML(xmlTemplate, null);

            //Get the first Section from the PDF document
            Aspose.Pdf.Generator.Section sec1 = pdf1.Sections[0];

            foreach (KeyValuePair<string, string> fieldinfo in fieldNameValuePairs)
            {
                Aspose.Pdf.Generator.Text NameField = sec1.GetObjectByID(fieldinfo.Key) as Aspose.Pdf.Generator.Text;
                NameField.Segments.Add(fieldinfo.Value);
            }

            MemoryStream mStream = new MemoryStream();
            pdf1.Save(mStream);
            mStream.Position = 0;
            return mStream;
        }

        #endregion
    }
}
