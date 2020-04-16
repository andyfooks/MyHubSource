using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using AJG.VirtualTrainer.Helper;
using System.IO;

namespace VTTests
{
    [TestClass]
    public class UnitTest_AsposeHelper
    {
        [TestMethod]
        public void TestPDFtoWordCOnversion()
        {
            try
            {
                string dir = @"C:\Users\afooks\Documents\AAAA\";
                string pdfFileName = "AddTextToPDFDoc.pdf";
                AsposeHelper h = new AsposeHelper();
                h.ConvertPDFtoWord(dir, pdfFileName);
            }
            catch (Exception ex)
            {
                int a = 1;
            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            AsposeHelper h = new AsposeHelper();

            string filePath = string.Format(@"C:\Users\a-ajfooks\Documents\AJG\HelloWorld{0}.pdf", Guid.NewGuid());
            string headerTxt = @"<div style='width:700px; padding: 0'>
              <table style='width:100%;'>
                   <tr>
                       <td>
                           <div style='text-align:left;'>
                                <img height='80' src='C:\Users\a-ajfooks\Source\Repos\virtualtrainer\AJG.VirtualTrainer.MVC\Views\RazorEmailTemplates\LeisureStatements\Assets\AJGLogo.png' alt='someimage' />
                               </div>
                           </td>
                           <td>
                               <div style='text-align:right;'>
                                    <img height='80' src='C:\Users\a-ajfooks\Source\Repos\virtualtrainer\AJG.VirtualTrainer.MVC\Views\RazorEmailTemplates\LeisureStatements\Assets\holidayAndHomesLogo.png' alt='someimage' />
                                   </div>
                               </td> 
                           </tr>
                       </table>
                   </div>";
            string footerTxt = @"<div style='width:700px; margin-left:100px'>
                                    <table style='font-size:0.3em;'>
                                        <tr>
                                            <td width='200px'>
                                                Arthur J. Gallagher Insurance Brokers Ltd
                                            </td>
                                            <td width='200px'></td>
                                            <td width='200px' style='text-align:right;'>
                                                Reg. Office: Spectrum Building, 7th Floor, 55 Blythswood Street, Glasgow, G2 7AT
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Is authorised and regulated by</td>
                                            <td></td>
                                            <td style='text-align:right;'>T: 0141 285 3300 F: 0870 191 6766 W: www.ajginternational.com</td>
                                        </tr>
                                        <tr>
                                            <td>Financial Conduct Authority</td>
                                            <td></td>
                                            <td style='text-align:right;'>Registered in Scotland.  Registration No. SC108909</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td></td>
                                            <td style='text-align:right;'>VAT Registration No: 973 0169 15</td>
                                        </tr>
                                        <tr>
                                            <td>The Financial Conduct Authority</td>
                                            <td></td>
                                            <td style='text-align:right;'></td>
                                        </tr>
                                        <tr>
                                            <td>Regulate all types of product that we provide.</td>
                                            <td></td>
                                            <td style='text-align:right;'></td>
                                        </tr>
                                    </table>
                                </div>";

            string html = @"<div style='color:red;'>Hello World <h1>Hello Bigger</h1></div>";

            for (int i = 0; i < 5; i++)
            {
                html += html;
            }

            h.SaveHtmlToPDFFile(html, filePath, null, null, 100, 100, 20, 20);
            h.AddHeadersFooters(filePath, AsposeHelper.HeaderFooterEnum.header, 10, 0, 50, 0, headerTxt);
            h.AddHeadersFooters(filePath, AsposeHelper.HeaderFooterEnum.footer, 10, 0, 50, 0, footerTxt);
        }
    }
}
