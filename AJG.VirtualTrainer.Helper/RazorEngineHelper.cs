using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJG.VirtualTrainer.Helper.RazorEngine
{
    public class RazorEngineHelper
    {
        public bool GetContentFromTemplate(string templateName, string templateFullPath, string razorModel, bool returnInputIfTemplateNotFound, out string htmlOutput)
        {
            htmlOutput = string.Empty;
            bool success = false;

            if (Engine.Razor.IsTemplateCached(templateName, razorModel.GetType()))
            {
                htmlOutput = Engine.Razor.Run(templateName, razorModel.GetType(), razorModel, null);
                success = true;
            }
            else
            {
                //string filePath = string.Format("{0}\\{1}", templateFullPath, templateName);
                if (System.IO.File.Exists(templateFullPath))
                {
                    string emailRazorTemplate = System.IO.File.ReadAllText(templateFullPath);
                    htmlOutput = Engine.Razor.RunCompile(emailRazorTemplate, templateName, razorModel.GetType(), razorModel, null);
                    success = true;
                }
                else
                {
                    if (returnInputIfTemplateNotFound)
                    {
                        htmlOutput = templateName;
                    }
                    else
                    {
                        htmlOutput = string.Format("Template: '{0}' Could not be found at: {1}. ", templateName, templateFullPath);
                    }
                }
            }

            htmlOutput = htmlOutput.Replace("&lt;", "<").Replace("&gt;", ">");
            return success;
        }
    }
}
