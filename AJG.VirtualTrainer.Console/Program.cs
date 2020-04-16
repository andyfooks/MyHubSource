using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VirtualTrainer;

namespace AJG.VirtualTrainer.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string Environment = ConfigurationManager.AppSettings["Environment"];
            var urlList = WebMethodsToCall.WebMethodList.Where(w => w.TargetEnvironment.ToLower() == Environment.ToLower()).Select(s => s.Url).ToList();

            foreach(var url in urlList)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.UseDefaultCredentials = true;
                request.Timeout = -1;

                try
                {
                    WebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream ReceiveStream = response.GetResponseStream();

                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

                    // Pipe the stream to a higher level stream reader with the required encoding format. 
                    StreamReader readStream = new StreamReader(ReceiveStream, encode);
                    Char[] read = new Char[256];

                    // Read 256 charcters at a time.    
                    int count = readStream.Read(read, 0, 256);

                    while (count > 0)
                    {
                        // Dump the 256 characters on a string and display the string onto the console.
                        String str = new String(read, 0, count);
                        count = readStream.Read(read, 0, 256);
                    }

                    // Release the resources of stream object.
                    readStream.Close();

                    // Release the resources of response object.
                    response.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }       

            //List<ExecuteEnums> executeSwitches = new List<ExecuteEnums>();
            //foreach (string arg in args)
            //{
            //    ExecuteEnums executeEnum = ExecuteEnums.ExecuteEscalationsFramework;
            //    bool parsedSucessfully = Enum.TryParse<ExecuteEnums>(arg, true, out executeEnum);

            //    if (parsedSucessfully)
            //    {
            //        if (!executeSwitches.Contains(executeEnum))
            //        {
            //            executeSwitches.Add(executeEnum);
            //        }
            //    }
            //}

            //using (var ctx = new VirtualTrainerContext())
            //{
            //    foreach (Project project in ctx.Project.ToList())
            //    {
            //        if (executeSwitches.Contains(ExecuteEnums.ImportDataFromActuris))
            //        {
            //            project.ExecuteAllActurisBusinessStructureConfigurations(ctx);
            //        }
            //        if (executeSwitches.Contains(ExecuteEnums.ExecuteRules))
            //        {
            //            project.ExecuteAllRules(ctx, true);
            //        }
            //        if (executeSwitches.Contains(ExecuteEnums.ExecuteEscalationsFramework))
            //        {
            //            project.ExecuteEscalationsFramework(ctx, true);
            //        }
            //    }
            //}
        }
    }
    public enum ExecuteEnums
    {
        ImportDataFromActuris,
        ExecuteRules,
        ExecuteEscalationsFramework
    }
}
