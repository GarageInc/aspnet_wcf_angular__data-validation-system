using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LSports.Framework.Models.CustomClasses
{
    public class Event
    {
        public string GetLockerKey()
        {
            return SportId + "-" + LeagueId + "-" + LocationId + "-" + HomeTeamId + "-" + AwayTeamId;
        }

        public int Id { get; set; }
        public long EventId { get; set; }
        public string EventName { get; set; }
        public string StartDate { get; set; }

        public long? SportId { get; set; }
        public string SportName { get; set; }

        public long? LeagueId { get; set; }
        public string LeagueName { get; set; }

        public long? MarketId { get; set; }
        public string MarketName { get; set; }
        public List<string> MarketNames { get; set; }

        public long? ProviderId { get; set; }
        public string ProviderName { get; set; }
        public List<string> ProviderNames { get; set; }

        public long? LocationId { get; set; }
        public string LocationName { get; set; }

        public string Status { get; set; }
        public DateTime LastUpdate = DateTime.UtcNow;

        public long? HomeTeamId { get; set; }
        public string HomeTeamName { get; set; }

        public long? AwayTeamId { get; set; }
        public string AwayTeamName { get; set; }
        
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public int ProductId { get; set; }

        protected string FileDir => $"{ConfigurationManager.AppSettings["BasePath"]}/EventsStorage/{ProductId}";

        protected string PathToXmlFile => $"{FileDir}/{EventId}.gzip";

        protected XDocument Doc { get; set; }

        public XDocument XmlTextExample
        {
            get
            {
                if (File.Exists(PathToXmlFile) && Doc.Root == null)
                {
                    var fileText = File.ReadAllText(PathToXmlFile);

                    try
                    {
                        if (!string.IsNullOrEmpty(fileText))
                        {
                            var fileData = new StringBuilder();

                            using (var fInStream = new FileStream(PathToXmlFile, FileMode.Open, FileAccess.Read))
                            {
                                using (var zipStream = new GZipStream(fInStream, CompressionMode.Decompress))
                                {
                                    var tempBytes = new byte[4096];
                                    int i;
                                    while ((i = zipStream.Read(tempBytes, 0, tempBytes.Length)) != 0)
                                        fileData.Append(Encoding.UTF8.GetString(tempBytes, 0, i));
                                }
                            }

                            var data = fileData.ToString();

                            return Doc = XDocument.Parse(data);
                        }
                    }
                    catch (Exception e)
                    {
                        // pass
                    }
                }

                return Doc;
            }

            set
            {
                Doc = value;

                /*
                var xmlText = value.OuterXml;

                if (string.IsNullOrEmpty(xmlText))
                {
                    throw new Exception("1-------------------------------------------------------------");
                }

                if (string.IsNullOrEmpty(Doc.OuterXml))
                {

                    throw new Exception("3-------------------------------------------------------------");
                }

                if (Doc.DocumentElement == null)
                {
                    throw new Exception("2-----------------" + value.OuterXml);
                }
                */
            }
        }

        public void Save()
        {
            if (!System.IO.Directory.Exists(FileDir))
            {
                System.IO.Directory.CreateDirectory(FileDir);
            }// pass

            var fileData = Doc.ToString();

            byte[] buffer = Encoding.UTF8.GetBytes(fileData);

            using (FileStream fStream = new FileStream(PathToXmlFile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (GZipStream zipStream = new GZipStream(fStream, CompressionMode.Compress))
                {
                    zipStream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        public Event()
        {
            Doc = new XDocument();
        }
    }
}
