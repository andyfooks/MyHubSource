using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AJG.VirtualTrainer.Helper.Exchange;
using AJG.VirtualTrainer.Helper.Encryption;
using System.Collections.Generic;
using AJG.VirtualTrainer.Services;
using System.Data.Entity;
using System.Linq;
using System.Xml.Linq;
using VirtualTrainer;
using System.Data.OleDb;
using System.Data;

namespace VTTests
{
    [TestClass]
    public class AJGVTHelper
    {
        [TestMethod]
        public void TestOLEDB()
        {
            string sConnection = null;
            OleDbCommand oleExcelCommand = default(OleDbCommand);
            OleDbDataReader oleExcelReader = default(OleDbDataReader);
            OleDbConnection oleExcelConnection = default(OleDbConnection);
            
            System.IO.DirectoryInfo directoryName  = System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory);
            string fullname = directoryName.Parent.FullName;
            // HDR = Yes ensures that the top row is used as row header - for queries!
            sConnection = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"", string.Format("{0}\\Test.xlsx", fullname));

            oleExcelConnection = new OleDbConnection(sConnection);
            oleExcelConnection.Open();

            oleExcelCommand = oleExcelConnection.CreateCommand();
            oleExcelCommand.CommandText = "Select P.Name, A.Address From [People$] as P Left Join [Address$] as A on P.ID = A.UserID";// where P.ID = 1";
            oleExcelCommand.CommandType = CommandType.Text;
            oleExcelReader = oleExcelCommand.ExecuteReader();
            int nOutputRow = 0;

            while (oleExcelReader.Read())
            {
                int count = oleExcelReader.FieldCount;
                string displaytext = string.Empty;
                for (int i = 0; i < oleExcelReader.FieldCount; i++)
                {
                    displaytext = string.Format("{0}, {1}", displaytext, oleExcelReader[i].ToString());
                    Type t = oleExcelReader[0].GetType();
                    // Get fields and use VT mappings to map return fields to Breach object.
                    string a = oleExcelReader["Name"].ToString();
                    string b = oleExcelReader["Address"].ToString();
                }
               // Console.WriteLine(displaytext);
            }
            oleExcelReader.Close();

            oleExcelConnection.Close();
        }
        [TestMethod]
        public void TestXpath()
        {
            try
            {
                string a = (@"<html xmlns:v='urn: schemas - microsoft - com:vml' xmlns:o='urn: schemas - microsoft - com:office: office' xmlns:w='urn: schemas - microsoft - com:office: word' xmlns:m='http://schemas.microsoft.com/office/2004/12/omml' xmlns='http://www.w3.org/TR/REC-html40'> <head>  <!--[if !mso]><style>v\:* {behavior:url(#default#VML);} o\:* {behavior:url(#default#VML);} w\:* {behavior:url(#default#VML);} .shape {behavior:url(#default#VML);} </style><![endif]--><style><!-- /* Font Definitions */ @font-face {font-family:Calibri; panose-1:2 15 5 2 2 2 4 3 2 4;} @font-face {font-family:Tahoma; panose-1:2 11 6 4 3 5 4 4 2 4;} /* Style Definitions */ p.MsoNormal, li.MsoNormal, div.MsoNormal {margin:0cm; margin-bottom:.0001pt; font-size:11.0pt; font-family:'Calibri','sans-serif'; mso-fareast-language:EN-US;} a:link, span.MsoHyperlink {mso-style-priority:99; color:blue; text-decoration:underline;} a:visited, span.MsoHyperlinkFollowed {mso-style-priority:99; color:purple; text-decoration:underline;} p.MsoAcetate, li.MsoAcetate, div.MsoAcetate {mso-style-priority:99; mso-style-link:'Balloon Text Char'; margin:0cm; margin-bottom:.0001pt; font-size:8.0pt; font-family:'Tahoma','sans-serif'; mso-fareast-language:EN-US;} span.BalloonTextChar {mso-style-name:'Balloon Text Char'; mso-style-priority:99; mso-style-link:'Balloon Text'; font-family:'Tahoma','sans-serif';} span.EmailStyle19 {mso-style-type:personal; font-family:'Calibri','sans-serif'; color:windowtext;} span.EmailStyle20 {mso-style-type:personal-reply; font-family:'Calibri','sans-serif'; color:#1F497D;} .MsoChpDefault {mso-style-type:export-only; font-size:10.0pt;} @page WordSection1 {size:612.0pt 792.0pt; margin:72.0pt 72.0pt 72.0pt 72.0pt;} div.WordSection1 {page:WordSection1;} --></style><!--[if gte mso 9]><xml> <o:shapedefaults v:ext='edit' spidmax='1026' /> </xml><![endif]--><!--[if gte mso 9]><xml> <o:shapelayout v:ext='edit'> <o:idmap v:ext='edit' data='1' /> </o:shapelayout></xml><![endif]--> </head> <body lang='EN-GB' link='blue' vlink='purple'> <div class='WordSection1'> <p class='MsoNormal'><span style='color:#1F497D'>OK – sounds reasonable<o:p></o:p></span></p> <p class='MsoNormal'><span style='color:#1F497D'><o:p></o:p></span></p> <div> <div style='border:none;border-top:solid #B5C4DF 1.0pt;padding:3.0pt 0cm 0cm 0cm'> <p class='MsoNormal'><b><span lang='EN-US' style='font-size:10.0pt;font-family:&quot;Tahoma&quot;,&quot;sans-serif&quot;;mso-fareast-language:EN-GB'>From:</span></b><span lang='EN-US' style='font-size:10.0pt;font-family:&quot;Tahoma&quot;,&quot;sans-serif&quot;;mso-fareast-language:EN-GB'> Andy Fooks <br/> <b>Sent:</b> 23 May 2017 10:46<br/> <b>To:</b> Adrian Wyatt<br/> <b>Subject:</b> Acturis Import<o:p></o:p></span></p> </div> </div> <p class='MsoNormal'><o:p>&nbsp;</o:p></p> <p class='MsoNormal'>Hi Adrian,<o:p></o:p></p> <p class='MsoNormal'><o:p>&nbsp;</o:p></p> <p class='MsoNormal'>We agreed a while ago that we would trash Acturis data in production and start afresh.<o:p></o:p></p> <p class='MsoNormal'><o:p>&nbsp;</o:p></p> <p class='MsoNormal'>I am just trashing the Acturis data in UAT and then ill run an import. This will be complete around 2 or 3 today.<o:p></o:p></p> <p class='MsoNormal'><o:p>&nbsp;</o:p></p> <p class='MsoNormal'>I know you and Jamie are concerned about certain changes introduces by the 7.3 upgrade, so we can evaluate this in UAT first and see if there are any issues and look at fixing them before moving on to Prod.<o:p></o:p></p> <p class='MsoNormal'><o:p>&nbsp;</o:p></p> <p class='MsoNormal'>What do you think?<o:p></o:p></p> <p class='MsoNormal'><o:p>&nbsp;</o:p></p> <p class='MsoNormal'>Regards,<o:p></o:p></p> <p class='MsoNormal'><o:p>&nbsp;</o:p></p> <p class='MsoNormal'><span style='font-family:&quot;Times New Roman&quot;,&quot;serif&quot;;color:#1F497D;mso-fareast-language:EN-GB'>Andy Fooks<o:p></o:p></span></p> <p class='MsoNormal'><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,&quot;sans-serif&quot;;color:#1F497D;mso-fareast-language:EN-GB'>Developer<o:p></o:p></span></p> <p class='MsoNormal'><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,&quot;sans-serif&quot;;color:#1F497D;mso-fareast-language:EN-GB'>Arthur J. Gallagher UK Group<o:p></o:p></span></p> <p class='MsoNormal'><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,&quot;sans-serif&quot;;color:#1F497D;mso-fareast-language:EN-GB'><o:p>&nbsp;</o:p></span></p> <p class='MsoNormal'><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,&quot;sans-serif&quot;;color:#1F497D;mso-fareast-language:EN-GB'><img width='244' height='42' id='Picture_x0020_4' src='cid:image001.gif@01D2D3B4.1389D5E0' alt='AJG_logo_for_email'><o:p></o:p></span></p> <p class='MsoNormal'><span style='color:#1F497D;mso-fareast-language:EN-GB'><o:p>&nbsp;</o:p></span></p> <p class='MsoNormal'><span style='font-size:10.0pt;color:black;mso-fareast-language:EN-GB'>100 Holdenhurst Road | Bournemouth | BH8 8AQ<o:p></o:p></span></p> <p class='MsoNormal'><span style='color:#1F497D;mso-fareast-language:EN-GB'>Mobile: &#43;44 07975795996<o:p></o:p></span></p> <p class='MsoNormal'><span style='mso-fareast-language:EN-GB'><a href='mailto:andy_fooks@ajg.com'>andy_fooks@ajg.com</a><span style='color:#1F497D'><o:p></o:p></span></span></p> <p class='MsoNormal'><span style='mso-fareast-language:EN-GB'><a href='http://www.ajginternational.com/'>www.ajginternational.com</a><span style='color:#1F497D'> <o:p></o:p></span></span></p> <p class='MsoNormal'><span style='color:black;mso-fareast-language:EN-GB'><o:p>&nbsp;</o:p></span></p> <p class='MsoNormal'><span style='color:black;mso-fareast-language:EN-GB'><img border='0' width='130' height='72' id='Picture_x0020_3' src='cid:image002.png@01D2D3B4.1389D5E0' alt='cid:image002.png@01D2A3BB.09202880'></span><span style='mso-fareast-language:EN-GB'><o:p></o:p></span></p> <p class='MsoNormal'><o:p>&nbsp;</o:p></p> </div> </body> </html>");
                XDocument doc = XDocument.Parse(a);
                var cats = from node in doc.Descendants("body")
                           select node.Attribute("body");
            }
            catch (Exception ex)
            {

            }
        }

        [TestMethod]
        public void testEnumNameFromInt()
        {
            AJGExchangeDeleteMode e = (AJGExchangeDeleteMode)1;
            string anme = e.ToString();
        }
        [TestMethod]
        public void TestRuleConfig()
        {
        //    using (VirtualTrainer.VirtualTrainerContext ctx = new VirtualTrainer.VirtualTrainerContext())
        //    {
        //        try
        //        {
        //            ExchangeEmailRuleConfig c = ctx.RuleConfigsExchange.FirstOrDefault();
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    }
        }

        [TestMethod]
        public void GetEmailMessageAndAttachementsTwo()
        {
            ExchangeHelper eh = new ExchangeHelper("andy_fooks@ajg.com", "afooks", "1150_LouiseF", "emea");
            //ExchangeHelper eh = new ExchangeHelper("xis-reports@ajg.com", "classresponses", "classresponses", "emea");

            // set up the date fields - there are 2, 1 with greater than one with less than.
            // These are connected with an AND operator
            List<AJGSearchFilter> dates = new List<AJGSearchFilter>();
            dates.Add(new AJGSearchFilterDate(DateTime.Now.AddDays(-2), true, AJGExchangeMessageSearchFilter.IsLessThanOrEqualTo, AJGEmailPropertyDefinition.DateTimeReceived));
            dates.Add(new AJGSearchFilterDate(DateTime.Now.AddDays(-3), true, AJGExchangeMessageSearchFilter.IsGreaterThanOrEqualTo, AJGEmailPropertyDefinition.DateTimeReceived));
            AJGSearchFilter searchFilter = new AJGSearchFilter(AJGExchangeLogicalOperator.And, dates);

            // Set up a couple other filters with an AND operator too
            List<AJGSearchFilter> subjectAndSender = new List<AJGSearchFilter>();
            subjectAndSender.Add(new AJGSearchFilterString("christie", AJGExchangeMessageSearchFilter.ContainsSubString, AJGEmailPropertyDefinition.Sender));
            subjectAndSender.Add(new AJGSearchFilterString("large number", AJGExchangeMessageSearchFilter.ContainsSubString, AJGEmailPropertyDefinition.Subject));
            AJGSearchFilter subjectAndSenderSearchFilter = new AJGSearchFilter(AJGExchangeLogicalOperator.And, subjectAndSender);

            // Now conbine the filters above with an AND/OR operator.
            List<AJGSearchFilter> requestFitlers = new List<AJGSearchFilter>();
            requestFitlers.Add(searchFilter);
            requestFitlers.Add(subjectAndSenderSearchFilter);
            AJGSearchFilter requestSearchFilter = new AJGSearchFilter(AJGExchangeLogicalOperator.And, requestFitlers);

            // Retrieve the emails.
            List<AJGEmailMessage> b = eh.GetEmailMessagesAndAttachementsFromInbox(requestSearchFilter, AJGExchangeDeleteMode.None);
        }
        [TestMethod]
        public void TestEncryption()
        {
            AdminService annn = new AdminService();
            string EncryptionKey = annn.GetEncryptionKey();
            string m = EncryptionHelper.Encrypt("MYPassword", EncryptionKey);
            m = EncryptionHelper.Decrypt(m, EncryptionKey);
        }
    }
}
