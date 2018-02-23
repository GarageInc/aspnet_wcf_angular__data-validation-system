using LSports.Framework.Models.CustomClasses;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using log4net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using LSports.DVS.Framework.DataAccess.Repositories;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Services;
using LSports.Services.Interfaces;

namespace LSports.Scheduler.Services
{
	/// <summary>
	/// Service for downloading and parsing xml-files
	/// </summary>
    public class XmlDownloadService
    {
        private readonly XmlParser _parser = new XmlParser();
        private readonly XMLDownloader _downloader = new XMLDownloader();
		
        private readonly IProductTreeRepository _productTreeRepository = new ProductTreeRepository();

        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /*
        public string CompressFile(string fileData, int productId)
        {
            return XMLDownloader.CompressGzipFile(fileData, productId);
        }

        public string DeCompressFile(string path)
        {
            return XMLDownloader.DecompressGzipFile(path);
        }
        */

        public void CheckTmpFilesDir(int productId)
        {
            _downloader.CheckTmpFilesDir(productId);
        }

        public string DownloadFile(Product product, string gateway)
        {
            _log.Info(string.Format("Start downloading from {0}", gateway));
			
            var filePath = _downloader.Download(product.Id, gateway);
            
            return filePath;
        }

        protected bool isProductTreeUpdating = false;
        protected object locker = new Object();

        Random r = new Random();

        public bool ProcessEventMessageAndParse(ArrivalMessage message, string xmlMessage, bool enableInsertingArrMessages, ref ConcurrentBag<Event> @events)
        {
            isProductTreeUpdating = false;

            bool isContainEvents = false;
            
            // 1 step: refresh product tree
            TreeNode newTree = null;

            var isRandom = r.Next(0, 100) < 3;

            if (isRandom)
            {
                newTree = new TreeNode {text = "xml"};
            }

            if (!string.IsNullOrEmpty(xmlMessage))
            {
                var eventNames = new List<string>();
                var sportNames = new List<string>();
                var leagueNames = new List<string>();
                var locationNames = new List<string>();
                var statusNames = new List<string>();
                var marketNames = new List<string>();
                var providerNames = new List<string>();
                var betNames = new List<string>();

                using (XmlReader myTextReader = XmlReader.Create(new StringReader(xmlMessage)))
                {
                    var headerXml = "";
                
                    var xmlDoc = new XmlDocument();
                    var xmlText = "";
                    while (myTextReader.EOF == false)
                    {
                        if (myTextReader.NodeType == XmlNodeType.Element)
                        {
                            if (myTextReader.LocalName == "Event")
                            {
                                isContainEvents = true;

                                xmlText = myTextReader.ReadOuterXml();

                                var newEvent = _parser.LoadEvent(enableInsertingArrMessages, message.ProductId, headerXml, xmlText, eventNames,
                                     sportNames, leagueNames, locationNames, statusNames, marketNames, providerNames,
                                   betNames);

                                if (isRandom && !isProductTreeUpdating)
                                {
                                    xmlDoc.LoadXml(newEvent.XmlTextExample.XPathSelectElement("/xml/Event").ToString());
                                    _parser.ParseToTreeNode(xmlDoc.DocumentElement, newTree);
                                }

                                @events.Add(newEvent);
                            }
                            else if (isRandom && myTextReader.LocalName == "Header")
                            {
                                headerXml = myTextReader.ReadOuterXml();

                                if (!isProductTreeUpdating && !string.IsNullOrEmpty(headerXml))
                                {
                                    xmlDoc.LoadXml(headerXml);

                                    _parser.ParseToTreeNode(xmlDoc.DocumentElement, newTree);
                                }
                            }
                            else
                            {
                                myTextReader.Read();
                            }
                        }
                        else
                        {
                            myTextReader.Read();
                        }
                    }
                };

                if (isContainEvents)
                {
                    message.SetFields(!enableInsertingArrMessages, eventNames, sportNames, leagueNames, locationNames, statusNames, marketNames, providerNames, betNames);
                }
            }

            if (isContainEvents && !isProductTreeUpdating && isRandom)
            {
                ProcessProductTree(message.ProductId, newTree);
            }

            isProductTreeUpdating = true;

            return isContainEvents;
        }

        public List<Event> ProcessPullArrivalMessage(ArrivalMessage message)
        {
            isProductTreeUpdating = false;

            bool isContainEvents = false;

            TreeNode newTree = null;

            newTree = new TreeNode { text = "xml" };

            var messageEvents = new List<Event>();

            var eventNames = new List<string>();
            var sportNames = new List<string>();
            var leagueNames = new List<string>();
            var locationNames = new List<string>();
            var statusNames = new List<string>();
            var marketNames = new List<string>();
            var providerNames = new List<string>();
            var betNames = new List<string>();

            var filePath = message.PathToXmlFile;

            // var fileData = DeCompressFile(filePath);

            using (XmlTextReader myTextReader = new XmlTextReader(filePath))
            {
                myTextReader.WhitespaceHandling = WhitespaceHandling.None;

                var headerXml = "";
                var xmlDoc = new XmlDocument();
                var xmlText = "";
                while (myTextReader.EOF == false)
                {
                    if (myTextReader.NodeType == XmlNodeType.Element)
                    {
                        if (myTextReader.LocalName == "Event")
                        {
                            isContainEvents = true;

                            xmlText = myTextReader.ReadOuterXml();

                            var newEvent = _parser.LoadEvent(true, message.ProductId, headerXml, xmlText, eventNames,
                                    sportNames, leagueNames, locationNames, statusNames, marketNames, providerNames,
                                betNames);
                                
                            if (!isProductTreeUpdating)
                            {
                                xmlDoc.LoadXml(newEvent.XmlTextExample.XPathSelectElement("/xml/Event").ToString());
                                _parser.ParseToTreeNode(xmlDoc.DocumentElement, newTree);
                            }

                            messageEvents.Add(newEvent);
                        }
                        else if (myTextReader.LocalName == "Header")
                        {
                            headerXml = myTextReader.ReadOuterXml();

                            if (!isProductTreeUpdating && !string.IsNullOrEmpty(headerXml))
                            {
                                xmlDoc.LoadXml(headerXml);

                                _parser.ParseToTreeNode(xmlDoc.DocumentElement, newTree);
                            }
                        }
                        else
                        {
                            myTextReader.Read();
                        }
                    }
                    else
                    {
                        myTextReader.Read();
                    }
                }
            }

            if (isContainEvents && !isProductTreeUpdating)
            {
                ProcessProductTree(message.ProductId, newTree);
            }

            if (isContainEvents)
            {
                message.SetFields(true, eventNames, sportNames, leagueNames, locationNames, statusNames, marketNames, providerNames, betNames);
            }

            isProductTreeUpdating = true;

            return messageEvents;
        }

        public bool IsContainsEvents(int productId, string filePath)
        {
            using (XmlTextReader myTextReader = new XmlTextReader(filePath))
            {
                myTextReader.WhitespaceHandling = WhitespaceHandling.None;

                while (myTextReader.EOF == false)
                {
                    if (myTextReader.NodeType == XmlNodeType.Element)
                    {
                        if (myTextReader.LocalName == "Event")
                        {
                            return true;
                        }
                        else
                        {
                            myTextReader.Read();
                        }
                    }
                    else
                    {
                        myTextReader.Read();
                    }
                }
            }

            return false;
        }

        public void ProcessProductTree(int productId, TreeNode newTree)
        {
            var productTree = _productTreeRepository.GetItemByProductId(productId);

            if (productTree == null)
            {
                //_log.Info(string.Format("insert tree of product {0}", productId));
                _productTreeRepository.Insert(productId, newTree);
            }
            else
            {
                //_log.Info(string.Format("refresh tree of product {0}", productId));
                TreeNode oldTree = productTree.Tree.core.data;

                MergeOldTreeByNew(newTree, oldTree);

                productTree.Tree.core.data = oldTree;
                _productTreeRepository.Update(productTree);
            }

        }

        public void MergeOldTreeByNew(TreeNode newNode, TreeNode oldNode)
        {
            List<TreeNode> addingNodes = new List<TreeNode>();

            if (!oldNode.children.Any())
            {
                addingNodes = newNode.children;
            }
            else if (oldNode.children.Any() && newNode.children != null && newNode.children.Any())
            {
                foreach (var nodeFrom in newNode.children)
                {
                    var isContain = false;

                    foreach (var nodeTo in oldNode.children)
                    {
                        if (nodeFrom.text == nodeTo.text)
                        {
                            isContain = true;
                            MergeOldTreeByNew(nodeFrom, nodeTo);
                        }// pass
                    }

                    if (!isContain)
                    {
                        addingNodes.Add(nodeFrom);
                    }// pass
                }
            }// pass, oldNode.count can't be < 0

            if (addingNodes != null && addingNodes.Any())
            {
                oldNode.children.AddRange(addingNodes);
            }
        }

    }
}