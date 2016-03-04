using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using libZip = ICSharpCode.SharpZipLib.Zip;

namespace gzipper
{
    class Program
    {
        static void Main(string[] args)
        {
            string zipPath = @"c:\zipped.zip";
            string easyPath = @"c:\unzipped\easy";
            string safiresPath = @"c:\unzipped\safires";

            EasyExtract(zipPath, easyPath);
            ExtractSync(zipPath, safiresPath);
        }

        public static bool EasyExtract(string file, string extractLocation)
        {
            try
            {
                ZipFile.ExtractToDirectory(file, extractLocation);
            }
            catch (Exception ex)
            {
                Console.Error.Write(ex);
                return false;
            }
            
            return true;
        }

        //This is the code we used in Safires to extract the database
        //sync software, which was downloaded as a .zip file
        public static bool ExtractSync(string file, string extractLocation)
        {
            //Copied straight from Safire Desktop, file paths and zip file data changed.
            var zipFile = File.ReadAllBytes(file);

            libZip.ZipFile zf = null;

            try
            {
                zf = new libZip.ZipFile(new MemoryStream(zipFile));
                foreach (libZip.ZipEntry zipEntry in zf)
                {
                    if (!zipEntry.IsFile)
                        continue;
                    var entryFileName = zipEntry.Name;
                    var buffer = new byte[4096];
                    var zipStream = zf.GetInputStream(zipEntry);

                    var fullzipToPath = Path.Combine(extractLocation, entryFileName);
                    var directoryName = Path.GetDirectoryName(fullzipToPath);

                    if (!string.IsNullOrEmpty(directoryName))
                        Directory.CreateDirectory(directoryName);

                    using (var streamWriter = File.Create(fullzipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error extracting symmetric archive", ex);
                return false;
            }
            finally
            {
                if (zf != null)
                {
                    zf.IsStreamOwner = true;
                    zf.Close();
                }

            }

            return true;
        }
    }
}
