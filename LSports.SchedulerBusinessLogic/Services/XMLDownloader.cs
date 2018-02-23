using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace LSports.Scheduler.Services
{
	/// <summary>
	/// Helper. This service work with folders of events
	/// </summary>
    public class XMLDownloader
    {
        public static string FilesFolder = ConfigurationManager.AppSettings["BasePath"] + "/DownloadedEvents";

        public static string ZIP_EXTENSION = ".gzip";

		/// <summary>
		/// Delete all files, if their count more than limit
		/// </summary>
		/// <param name="productId"></param>
        public void CheckTmpFilesDir(int productId)
        {
            var productFolder = FilesFolder + "/" + productId + "/";

            if (Directory.Exists(productFolder))
            {
                var myDir = new DirectoryInfo(productFolder);
                var files = myDir.GetFiles();

                if (files.Length > Sports.sports.Count * 3)// 3 - max count. dog-nail.
                {
                    foreach (var fileInfo in files)
                    {
                        try
                        {
                            fileInfo.Delete();
                        }
                        catch (Exception er)
                        {
                            
                        }
                    }
                }
            }
        }

		/// <summary>
		/// Download xml with events for selected product
		/// And saving in harddrive
		/// </summary>
		/// <param name="productId"></param>
		/// <param name="gateway"></param>
		/// <returns></returns>
        public string Download(int productId, string gateway)
        {
            var client = new GZipWebClient();

            var filePath = GetFileName(productId);
            client.DownloadFile(gateway, filePath);

            return filePath;
        }

		/// <summary>
		/// Not used now. For compressing data in file.gzip
		/// </summary>
		/// <param name="data"></param>
		/// <param name="productId"></param>
		/// <returns></returns>
        public static string CompressGzipFile(string data, int productId)
        {
            var newGzipPath = XMLDownloader.GetFileName(productId) + ZIP_EXTENSION;

            byte[] buffer = Encoding.UTF8.GetBytes(data);

            using (FileStream fStream = new FileStream(newGzipPath, FileMode.Create, FileAccess.Write))
            {
                using (GZipStream zipStream = new GZipStream(fStream, CompressionMode.Compress))
                {
                    zipStream.Write(buffer, 0, buffer.Length);
                }
            }

            return newGzipPath;
        }

		/// <summary>
		/// Decompressing data from file.gzip by path
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
        public static string DecompressGzipFile(string path)
        {
            var fileData = new StringBuilder();

            using (var fInStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (var zipStream = new GZipStream(fInStream, CompressionMode.Decompress))
                {
                    var tempBytes = new byte[4096];
                    int i;
                    while ((i = zipStream.Read(tempBytes, 0, tempBytes.Length)) != 0)
                        fileData.Append(Encoding.UTF8.GetString(tempBytes, 0, i));
                }
            }

            return fileData.ToString();
        }

		/// <summary>
		/// Generating unique filename for selected product
		/// </summary>
		/// <param name="productId"></param>
		/// <returns></returns>
        public static string GetFileName(int productId)
        {
            var path = $"{FilesFolder}/{productId}";

            bool exists = System.IO.Directory.Exists(path);

            if ( !exists)
                System.IO.Directory.CreateDirectory(path);

            return $"{path}/{DateTime.UtcNow.Ticks}{Guid.NewGuid()}";
        }

    }

    public class GZipWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);

            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            return request;
        }
    }
}