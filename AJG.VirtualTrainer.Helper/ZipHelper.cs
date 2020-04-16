using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJG.VirtualTrainer.Helper.ZipHelper
{
    public class ZipHelper
    {
        public static void ExtractAll(string zipPath, string extractDirectoryPath)
        {
            using (ZipArchive za = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry entry in za.Entries)
                {
                    string extractFullPath = Path.Combine(extractDirectoryPath, entry.Name);
                    if (File.Exists(extractFullPath))
                    {
                        File.Delete(extractFullPath);
                    }
                    entry.ExtractToFile(extractFullPath);
                }
            }
        }
    }
}
