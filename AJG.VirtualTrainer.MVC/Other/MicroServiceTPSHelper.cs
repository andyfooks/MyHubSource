using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VirtualTrainer;

namespace AJG.VirtualTrainer.MVC
{
    public class MicroServiceTPSHelper
    {
        private bool GetFTPFileLastUpdatedDateFromLocalFile(string filePath, out DateTime lastUpdated)
        {
            lastUpdated = DateTime.Now;
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    string dateFromFile = System.IO.File.ReadAllText(filePath);
                    if (DateTime.TryParse(dateFromFile, out lastUpdated))
                    {
                        return true;
                    }
                    else
                    {
                        //Console.WriteLine(string.Format("File content could not be parsed to a dateTime: {0}", filePath));
                    }
                }
                else
                {
                    //Console.WriteLine(string.Format("File Not found: {0}", filePath));
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(string.Format("Error reading in file: {0}", filePath));
            }

            return false;
        }
    }
}