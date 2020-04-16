using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AJG.VirtualTrainer.Helper.FTP
{
    public class FTPHelper
    {
        private ConnectionInfo connectionInfo;

        public FTPHelper(string host, string username, string password)
        {
            connectionInfo = new ConnectionInfo(host, username,
                                        new PasswordAuthenticationMethod(username, password),
                                        new PrivateKeyAuthenticationMethod("rsa.key"));
        }

        public bool FTPFileExists(string downloadFilePath)
        {
            try
            {
                using (var client = new SftpClient(connectionInfo))
                {
                    client.Connect();
                    return client.Exists(downloadFilePath);
                }
            }
            catch (Exception ex)
            {
                // Log this error.
                throw ex;
            }
        }

        public bool DownloadDocument(string downloadFilePath, string downloadFileDestinationPath)
        {
            try {
                using (var client = new SftpClient(connectionInfo))
                {
                    client.Connect();
                    var fileAttributes = client.GetAttributes(downloadFilePath);
                    using (Stream stream = new FileStream(downloadFileDestinationPath, FileMode.Create))
                    {
                        client.DownloadFile(downloadFilePath, stream);
                    }
                }
                return File.Exists(downloadFileDestinationPath) ? true : false;
            }
            catch (Exception ex)
            {
                // Log this error.
                throw ex;
            }
        }
        public string GetLastWriteTime(string downloadFilePath)
        {
            try
            {
                using (var client = new SftpClient(connectionInfo))
                {
                    client.Connect();
                    DateTime dt = client.GetLastWriteTime(downloadFilePath);
                    return dt.ToString();
                }
            }
            catch (Exception ex)
            {
                // Log Exception - if want to.
                throw ex;
            }
        }
    }
}
