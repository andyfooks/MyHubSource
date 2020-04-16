using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJG.VirtualTrainer.Helper.General
{
    public static class DirectoryHelper
    {
        /// <summary>
        ///  Validation includes, access to path, creating a folder, creating a file, deleting a file and deleting a directory.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="tempFolderName"></param>
        /// <param name="returnMessage"></param>
        /// <returns></returns>
        public static bool ValidateDirectoryAccess(string path, string tempFolderName, out string returnMessage)
        {
            returnMessage = string.Empty;

            try
            {
                string projectName = tempFolderName;
                string fileContentText = string.Format("Validation Test For {0}.", projectName);
                string tempFileName = string.Format("{0}.txt", projectName);
                string fullDirectoryPath = System.IO.Path.Combine(path, tempFolderName);
                string fullFilePath = System.IO.Path.Combine(fullDirectoryPath, tempFileName);

                // does the path exist?
                if (System.IO.Directory.Exists(path))
                {
                    // Create a directory
                    try
                    {
                        System.IO.Directory.CreateDirectory(fullDirectoryPath);
                    }
                    catch (Exception ex)
                    {
                        returnMessage = string.Format("Unable to create a folder at location: {0}. Error: {1}", fullDirectoryPath, ex.Message);
                        return false;
                    }
                    // Now try to save a file at the location
                    try
                    {
                        System.IO.File.WriteAllText(fullFilePath, fileContentText);
                    }
                    catch (Exception ex)
                    {
                        returnMessage = string.Format("Unable to create a file at location: {0}. Error: {1}", fullFilePath, ex.Message);
                        return false;
                    }
                    // Now try to delete the file.
                    try
                    {
                        System.IO.File.Delete(fullFilePath);
                    }
                    catch (Exception ex)
                    {
                        returnMessage = string.Format("Unable to delete a file at location: {0}. Error: {1}", fullFilePath, ex.Message);
                        return false;
                    }
                    // Now try to delete the Folder
                    try
                    {
                        System.IO.Directory.Delete(fullDirectoryPath);
                    }
                    catch (Exception ex)
                    {
                        returnMessage = string.Format("Unable to delete a folder at location: {0}. Error: {1}", fullDirectoryPath, ex.Message);
                        return false;
                    }
                }
                else
                {
                    returnMessage = string.Format("The Specfied path does not exist or is not accessible: {0}.", path);
                    return false;
                }
            }
            catch (Exception ex)
            {
                returnMessage = string.Format("Unexpected Error {0}", ex.Message);
                return false;
            }
            return true;
        }

        public static string GetSystemSaveLocationForDocument(string FileName)
        {
            System.Reflection.Assembly ass = System.Reflection.Assembly.GetExecutingAssembly();
            string fullProcessPath = ass.Location;
            string currentDir = Path.GetDirectoryName(fullProcessPath);

            return Path.Combine(currentDir, FileName);
        }

        //public static string SaveDocLocal(string docName, string content)
        //{
        //    System.Reflection.Assembly ass = System.Reflection.Assembly.GetExecutingAssembly();
        //    string fullProcessPath = ass.Location;
        //    string currentDir = Path.GetDirectoryName(fullProcessPath);
        //    string documentFullPath = string.Format(@"{0}\{1}.xlsx", currentDir.TrimEnd('\\'), Guid.NewGuid().ToString());

        //    FileInfo newFile = new FileInfo(documentFullPath);
        //    if (newFile.Exists)
        //    {
        //        newFile.Delete();  // ensures we create a new workbook
        //        newFile = new FileInfo(documentFullPath);
        //    }

        //    return documentFullPath;
        //}
    }
}
