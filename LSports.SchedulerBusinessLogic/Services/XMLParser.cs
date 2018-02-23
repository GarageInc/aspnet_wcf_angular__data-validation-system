using LSports.DVS.Framework.DataAccess.Repositories;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace LSports.Scheduler.Services
{
    public class XmlParser
    {
        public void RecursivelyParseXml(XmlNodeList list, TreeNode treeNode)
        {
            foreach (XmlNode childNode in list)
            {
                ParseToTreeNode(childNode, treeNode);
            }
        }

        public void ParseToTreeNode(XmlNode childNode, TreeNode treeNode)
        {
            if (childNode is XmlDeclaration)
                return;

            if (treeNode.children == null)
                treeNode.children = new List<TreeNode>();

            var currentTreeNode = new TreeNode
            {
                text = childNode.Name == "#text" ? "Value" : childNode.Name
            };

            if (childNode.HasChildNodes && childNode.ChildNodes.Count == 1 && childNode.FirstChild.Name == "#text")
            {
                if (treeNode.children.All(ch => ch.text != currentTreeNode.text))
                    treeNode.children.Add(currentTreeNode);
                if (childNode.Attributes != null && childNode.Attributes.Count == 0 && treeNode.children.All(ch => ch.text != "Value"))
                    return;
                else
                {
                    currentTreeNode.children = new List<TreeNode>
                    {
                        new TreeNode
                        {
                            text = "Value"
                        }
                    };
                }
            }

            if (treeNode.children.All(ch => ch.text != currentTreeNode.text))
                treeNode.children.Add(currentTreeNode);
            else
                currentTreeNode = treeNode.children.FirstOrDefault(ch => ch.text == currentTreeNode.text);

            if (childNode.Attributes != null)
                foreach (XmlAttribute attribute in childNode.Attributes)
                {
                    var childTreeNode = new TreeNode
                    {
                        text = attribute.Name
                    };
                    if (currentTreeNode != null && currentTreeNode.children == null)
                        currentTreeNode.children = new List<TreeNode>();
                    if (currentTreeNode != null && currentTreeNode.children.All(ch => ch.text != childTreeNode.text))
                        currentTreeNode.children.Add(childTreeNode);
                }

            if (childNode.HasChildNodes && (!(childNode.ChildNodes.Count == 1 && childNode.FirstChild.Name == "#text")))
                RecursivelyParseXml(childNode.ChildNodes, currentTreeNode);
        }

        public long GetNodeId(XElement node)
        {
            if (node.HasAttributes)
            {
                var idAttr = node.Attribute("Id");

                if (idAttr != null)
                {
                    return long.Parse(idAttr.Value);
                }
            }

            return 0;
        }

        public Event LoadEvent(bool enableInsertingArrMessages, int productId, string headerXml, string xmlText, 
            List<string> eventNames, List<string> sportNames, 
            List<string> leagueNames, List<string> locationNames, 
            List<string> statusNames, List<string> marketNames,
            List<string> providerNames, List<string> betNames)
        {
            var @event = new Event
            {
                ProductId = productId,
                CreatedBy = "Admin",
                CreatedOn = DateTime.UtcNow,
                UpdatedBy = "Admin",
                UpdatedOn = DateTime.UtcNow
            };
            
            var eventNode = XDocument.Parse(xmlText).Root;// @event.XmlTextExample.XPathSelectElement("/xml/Event");

            if (eventNode.HasElements)
            {
                foreach (var node in eventNode.Elements())
                {
                    switch (node.Name.LocalName)
                    {
                        case "EventID":
                            long value;
                            long.TryParse(node.Value, out value);
                            @event.EventId = value;
                            eventNames.Add(value.ToString());
                            break;
                        case "StartDate":
                            @event.StartDate = node.Value.Replace("'", "");
                            break;
                        case "SportID":
                            long.TryParse(node.Value, out value);
                            @event.SportId = value;
                            if (node.HasAttributes)
                            {
                                var nodeName = node.Attribute("Name");

                                if (nodeName != null)
                                {
                                    @event.SportName = node.Attribute("Name").Value.Replace("'", "");
                                        sportNames.Add(@event.SportId.ToString());
                                }
                            }
                            break;
                        case "LeagueID":
                            long.TryParse(node.Value, out value);
                            @event.LeagueId = value;
                            if (node.HasAttributes )
                            {
                                var nodeName = node.Attribute("Name");

                                if (nodeName != null)
                                {
                                    @event.LeagueName = nodeName.Value.Replace("'", "");
                                        leagueNames.Add(@event.LeagueId.ToString());
                                }
                            }
                            break;
                        case "LocationID":
                            long.TryParse(node.Value, out value);
                            @event.LocationId = value;
                            if (node.HasAttributes )
                            {
                                var nodeName = node.Attribute("Name");

                                if (nodeName != null)
                                {
                                    @event.LocationName = node.Attribute("Name").Value.Replace("'", "");
                                        locationNames.Add(@event.LocationId.ToString());
                                }
                            }
                            break;
                        case "Status":
                            @event.Status = node.Value.Replace("'", "");
                                statusNames.Add(@event.Status);
                            break;
                        case "LastUpdate":
                            DateTime.TryParse(node.Value, out@event.LastUpdate);
                            break;
                        case "HomeTeam":
                            if (node.HasAttributes )
                            {
                                var nodeId = node.Attribute("ID");
                                if (nodeId != null)
                                {
                                    long.TryParse(nodeId.Value, out value);
                                    @event.HomeTeamId = value;

                                    if (node.HasAttributes)
                                    {
                                        var nodeName = node.Attribute("Name") ;
                                        if (nodeName != null)
                                        {
                                            @event.HomeTeamName = nodeName.Value.Replace("'", "");
                                        }
                                    }
                                }
                                else
                                {
                                    @event.HomeTeamId = GetNodeId(node); ;
                                    @event.HomeTeamName = node.Value.Replace("'", "");
                                }
                            }
                            break;
                        case "AwayTeam":
                            if (node.HasAttributes)
                            {
                                var nodeId = node.Attribute("ID");
                                if (nodeId != null)
                                {
                                    long.TryParse(nodeId.Value, out value);
                                    @event.AwayTeamId = value;

                                    if (node.HasAttributes)
                                    {
                                        var nodeName = node.Attribute("Name");
                                        if (nodeName != null)
                                        {
                                            @event.AwayTeamName = nodeName.Value.Replace("'", "");
                                        }
                                    }
                                }
                                else
                                {
                                    @event.AwayTeamId = GetNodeId(node);
                                    @event.AwayTeamName = node.Value.Replace("'", "");
                                }
                            }
                            break;
                    }

                    switch (node.Name.LocalName)
                    {
                        case "Sport":
                            @event.SportId = GetNodeId(node);
                            @event.SportName = node.Value.Replace("'", "");
                                sportNames.Add(@event.SportId.ToString());
                            break;

                        case "League":
                            @event.LeagueId = GetNodeId(node);
                            @event.LeagueName = node.Value.Replace("'", "");
                            leagueNames.Add(@event.LeagueId.ToString());
                            break;

                        case "Location":
                            @event.LocationId = GetNodeId(node);
                            @event.LocationName = node.Value.Replace("'", "");
                                locationNames.Add(@event.LocationId.ToString());
                            break;

                    }
                }
                // childDocument.LoadXml(childNode.OuterXml);

                if (eventNode.XPathSelectElement($"/Event/Outcomes")!=null)
                {
                    var marketIterator = eventNode.XPathSelectElements($"/Event/Outcomes/Outcome").Select(x => x.Attribute("name")?.Value);
                    var providerIterator = eventNode.XPathSelectElements($"/Event/Outcomes/Outcome/Bookmaker").Select(x => x.Attribute("name")?.Value);
                    var betIterator = eventNode.XPathSelectElements($"/Event/Outcomes/Outcome/Bookmaker/Odds").Select(x => x.Attribute("bet")?.Value);

                    foreach (var attr in marketIterator)
                    {
                        if (attr != null)
                            marketNames.Add(attr.Replace("'", ""));
                    }

                    foreach (var attr in providerIterator)
                    {
                        if (attr != null)
                            providerNames.Add(attr.Replace("'", ""));
                    }

                    foreach (var attr in betIterator)
                    {
                        if (attr != null)
                            betNames.Add(attr.Replace("'", ""));
                    }

                    @event.MarketNames = new List<string>();
                    @event.MarketNames.AddRange(marketNames);

                    @event.ProviderNames = new List<string>();
                    @event.ProviderNames.AddRange(providerNames);
                }

                var currentPriceNodes = eventNode.XPathSelectElements($"/Event/Outcomes/Outcome/Bookmaker/Odds");

                if (currentPriceNodes != null)
                {
                    foreach (var currentPriceNode in currentPriceNodes)
                    {
                        if (currentPriceNode.Attribute("currentPriceProbability") == null)
                        {
                            var currentPrice = currentPriceNode.Attribute("currentPrice");

                            if (currentPrice != null)
                            {
                                double value = 1;

                                var parsed = double.TryParse(currentPrice.Value, out value);

                                if (parsed)
                                {
                                    currentPriceNode.Add(new XAttribute("currentPriceProbability", Math.Round(100.0 * (1.0 / value), 3)));
                                }
                            }
                        }
                    }
                }

                var scoresNodes = eventNode.XPathSelectElements($"/Event/Scores/Score");
                if (scoresNodes != null)
                {
                    foreach (var score in scoresNodes)
                    {
                        if (score.Attribute("totalScore") == null && score.Attribute("TotalScore") == null)
                        {
                            var homeScore = score.Attribute("homeScore");
                            var awayScore = score.Attribute("awayScore");
                            if (homeScore != null && awayScore != null)
                            {
                                int homeScoreValue = 0;
                                int awayScoreValue = 0;

                                var parsed = int.TryParse(homeScore.Value, out homeScoreValue);
                                parsed = parsed && int.TryParse(awayScore.Value, out awayScoreValue);

                                if (parsed)
                                {
                                    var newAttr = new XAttribute("totalScore", (homeScoreValue + awayScoreValue).ToString());
                                    score.Add(newAttr);
                                }// pass

                                continue;
                            }

                            homeScore = score.Attribute("HomeScore");
                            awayScore = score.Attribute("AwayScore");
                            if (homeScore != null && awayScore != null)
                            {
                                int homeScoreValue = 0;
                                int awayScoreValue = 0;

                                var parsed = int.TryParse(homeScore.Value, out homeScoreValue);
                                parsed = parsed && int.TryParse(awayScore.Value, out awayScoreValue);

                                if (parsed)
                                {
                                    var newAttr = new XAttribute("TotalScore", (homeScoreValue + awayScoreValue).ToString());
                                    score.Add(newAttr);
                                }// pass

                                continue;
                            }
                        }
                    }
                }

                @event.XmlTextExample = XDocument.Parse("<xml>" + headerXml + eventNode.ToString() + "</xml>");//.Replace("'", "") ;// IMPORTANT!

                if (@event.EventId == 0)
                {
                    @event.EventId = GetNodeId(@event.XmlTextExample.XPathSelectElement("/xml/Event"));
                }

                if (@event.EventId > 0)
                {
                    eventNames.Add(@event.EventId.ToString());
                }

                //events.Add(@event);
            }// else pass

            return @event;
        }

        public static string GetNodeHistoryLastValue(XObject node)
        {
            if (node is XElement)
            {
                var text = ((node as XElement).FirstNode as XText).Value;
                return text;
                var elements = ((XElement) node).Elements();
                foreach (var xElement in elements)
                {

                    if (!XmlMerger.IsHistoryNode(xElement.Name.LocalName))
                    {
                        return (xElement).Value;
                    }
                }

                return (node as XElement).Value;
            } else if (node is XText)
            {
                return (node as XText).Value;
            }
            else
            {
                return (node as XAttribute).Value;
            }

            // var hasHistory = node.ChildNodes.Cast<XmlNode>().Any(XmlMerger.IsHistoryNode);

            // return hasHistory ? node.FirstChild.InnerText : node.InnerText;
        }
    }
}