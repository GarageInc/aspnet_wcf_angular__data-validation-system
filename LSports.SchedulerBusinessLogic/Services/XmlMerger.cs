using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using LSports.DVS.Framework.DataAccess.Repositories;
using WebGrease.Css.Extensions;

namespace LSports.Scheduler.Services
{
    public class XmlMerger
    {
        private readonly IEventHistoryRepository _eventHistoryRepository;
        private readonly IEventRepository _eventRepository;

        public static string DvsHistoryValues = "DvsHistoryValues";
        public static string DvsHistoryAttributes = "DvsHistoryAttributes";
        public static string DvsHistoryOldValue = "DvsOld";
        public static string DvsHistoryAddValue = "DvsAdd";
        public static string DvsLastupdateAttribbute = "DvsLastUpdate";

        public static string XmlLastupdateAttribbute_v1 = "LastUpdate";
        public static string XmlLastupdateAttribbute_v2 = "lastUpdate";

        public XmlMerger() : this(new EventHistoryRepository(), new EventRepository())
        {
        }

        public XmlMerger(
            IEventHistoryRepository eventHistoryRepo,
            IEventRepository eventRepo)
        {
            _eventHistoryRepository = eventHistoryRepo;
            _eventRepository = eventRepo;
        }

        public void MergeByHistories()
        {
            var historiesCount = _eventHistoryRepository.GetNotMergedCount();

            var counter = 300;

            var baseEventXml = new XmlDocument();
            var historyXml = new XmlDocument();

            for (var i = 0; i <= historiesCount / counter; i++)
            {
                throw new Exception();
                /*
                var notMerged = new List<Event>(); //_eventHistoryRepository.GetGroupsNotMergedByEvent(counter);

                if (!notMerged.Any())
                {
                    break;
                }// else pass

                foreach(var group in notMerged)
                {
                    baseEventXml.LoadXml(group.BaseEventXmlText);

                    foreach(var history in group.EventHistories)
                    {
                        historyXml.LoadXml(history.XmlText);

                        MergeWithOldChanges(baseEventXml.ChildNodes, historyXml.ChildNodes, baseEventXml, historyXml, "");
                    }

                    group.BaseEventXmlText = baseEventXml.OuterXml;
                }

                _eventHistoryRepository.SaveHistoriesGroups(notMerged);*/
            }
        }

        public List<Event> GetEventsByEventIds(List<long> oldEventsIds, int productId)
        {
            return _eventRepository.GetEventsByEventIds(oldEventsIds, productId);
        }
        
        public List<Event> MergeByEvents(List<Event> newEvents, int productId)
        {
            //var oldEvents = GetEventsByEventIds(newEvents, productId);

            //var oldEventXml = new XmlDocument();
            //var newEventXml = new XmlDocument();

            foreach (var newEvent in newEvents)
            {
                if (newEvent.XmlTextExample != null)
                {
                    /*
                    var oldEvent = oldEvents.FirstOrDefault(x => x.EventId == newEvent.EventId);

                    if (oldEvent != null && oldEvent.XmlTextExample != null)
                    {

                        MergeWithOldChanges(oldEvent.XmlTextExample.ChildNodes, newEvent.XmlTextExample.ChildNodes, oldEvent.XmlTextExample, newEvent.XmlTextExample, "");

                        newEvent.XmlTextExample = oldEvent.XmlTextExample;

                        newEvent.ProductId = productId;

                        newEvent.LastUpdate = oldEvent.LastUpdate;
                        newEvent.CreatedOn = oldEvent.CreatedOn;
                        newEvent.UpdatedOn = DateTime.UtcNow;
                    }
                    */
                }
            }

            return newEvents;
        }


        public void MergeWithOldChanges(IEnumerable<XElement> a, IEnumerable<XElement> b, XElement parentDocA, XElement parentDocB, string lastUpdate)
        {
            if (String.IsNullOrEmpty(lastUpdate))
            {
                foreach (var nodeB in b)
                {
                    if (nodeB.Name.LocalName == XmlLastupdateAttribbute_v1 || nodeB.Name.LocalName == XmlLastupdateAttribbute_v2)
                    {
                        lastUpdate = (nodeB.FirstNode as XText).Value;
                    }
                }
            }// pass

            foreach (var nodeB in b)
            {
                if (nodeB.Name.LocalName == "Header" )
                {
                    continue;
                }

                bool isContains = false;

                if (nodeB.Name.LocalName == XmlLastupdateAttribbute_v1)
                {
                    continue;
                }

                foreach (var nodeA in a)
                {
                    bool canMerge = !IsHistoryNode(nodeB.Name.LocalName);


                    if (canMerge && nodeB.Name.LocalName == nodeA.Name.LocalName)
                    {
                        if (nodeA.HasAttributes && nodeB.HasAttributes )
                        {
                            var nodeName = nodeB.Name.LocalName;

                            if (nodeName == "Score")
                            {
                                canMerge = canMerge && IsIdenticalScores(nodeA, nodeB);
                            }
                            else if (nodeName == "Scorer")
                            {
                                canMerge = canMerge && IsIdenticalScorers(nodeA, nodeB);

                                isContains = isContains || XmlMerger.IsEqualsByAttributes(nodeA, nodeB);
                            }
                            else if (nodeName == "Card")
                            {
                                canMerge = canMerge && IsIdenticalCards(nodeA, nodeB);

                                isContains = isContains || XmlMerger.IsEqualsByAttributes(nodeA, nodeB);
                            }
                            else if (nodeName == "Participant")
                            {
                                canMerge = canMerge && IsIdenticalParticipants(nodeA, nodeB);
                            }
                            else if (nodeName == "Runner")
                            {
                                canMerge = canMerge && IsIdenticalRunners(nodeA, nodeB);
                            }
                            else if (nodeName == "Player")
                            {
                                canMerge = canMerge && IsIdenticalPlayers(nodeA, nodeB);
                            }
                            else if (nodeName == "Statistic")
                            {
                                canMerge = canMerge && IsIdenticalStatistics(nodeA, nodeB);
                            }
                            else if (nodeName == "Outcome" || nodeName == "Bookmaker" || nodeName == "Odds")
                            {
                                canMerge = canMerge && EqualsById(nodeA, nodeB);
                            } else if (nodeName == "Odd")
                            {
                                canMerge = canMerge && EqualsBetLine(nodeA, nodeB);
                            }// else pass
                        }// else pass
                    }
                    else
                    {
                        canMerge = false;
                    }

                    if (canMerge)
                    {
                        isContains = true;

                        if (nodeB.HasElements)
                        {
                            MergeWithOldChanges(nodeA.Elements(), nodeB.Elements(), nodeA, nodeB, lastUpdate);
                        }
                        else
                        {
                            if (nodeB.FirstNode is XText && nodeA.FirstNode is XText && (nodeB.FirstNode as XText).Value == (nodeA.FirstNode as XText).Value)
                            {
                                // pass, not changed
                            }
                            else if(nodeB.FirstNode is XText && nodeA.FirstNode is XText)
                            {
                                var historyInnerTextNodes = nodeA.XPathSelectElement(DvsHistoryValues);

                                XElement historyInnerTextNode;

                                if (historyInnerTextNodes == null)
                                {
                                    historyInnerTextNode = new XElement(DvsHistoryValues);
                                    nodeA.Add(historyInnerTextNode);
                                }
                                else
                                {
                                    historyInnerTextNode = historyInnerTextNodes;
                                }

                                var oldInnerTextNode = new XElement(DvsHistoryOldValue, (nodeA.FirstNode as XText).Value);

                                // copy
                                if (nodeA.HasAttributes)
                                {
                                    foreach (var attrA in nodeA.Attributes())
                                    {
                                        var newAttr = new XAttribute(attrA.Name.LocalName, attrA.Value);
                                        oldInnerTextNode.Add(newAttr);
                                    }
                                } // pass

                                // add timeframe attribute

                                if (string.IsNullOrEmpty(lastUpdate))
                                {
                                    lastUpdate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff");
                                }

                                var attrTime = new XAttribute(DvsLastupdateAttribbute, lastUpdate);
                                oldInnerTextNode.Add(attrTime);

                                historyInnerTextNode.Add(oldInnerTextNode);

                                
                                (nodeA.FirstNode as XText).Value = (nodeB.FirstNode as XText).Value;
                                //nodeA.Add(new XText(nodeB.Value));
                            }
                            
                        }

                        if (nodeA.HasAttributes && nodeB.HasAttributes)
                        {
                            MergeNodesAttributes(nodeA, nodeB, lastUpdate);
                        } // else pass
                    }
                    else
                    {
                        // pass
                    }
                }

                if ( !isContains)
                {
                    // var newNode = parentDocA.OwnerDocument.ImportNode(nodeB, true);

                    // var result = ReflectiveEquals(nodeB, newNode);

                    parentDocA.Add(nodeB);
                }// else pass
            }
        }

        public bool ReflectiveEquals(object first, object second)
        {
            if (first == null && second == null)
            {
                return true;
            }
            if (first == null || second == null)
            {
                return false;
            }
            Type firstType = first.GetType();
            if (second.GetType() != firstType)
            {
                return false; // Or throw an exception
            }
            // This will only use public properties. Is that enough?
            foreach (PropertyInfo propertyInfo in firstType.GetProperties())
            {
                if (propertyInfo.CanRead)
                {
                    object firstValue = propertyInfo.GetValue(first, null);
                    object secondValue = propertyInfo.GetValue(second, null);
                    if (!object.Equals(firstValue, secondValue))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool EqualsBetLine(XElement nodeA, XElement nodeB)
        {
            var result = nodeB.Attribute("Bet") != null && nodeA.Attribute("Bet") != null
                        && nodeB.Attribute("Bet").Value == nodeA.Attribute("Bet").Value
                        && nodeB.Attribute("Line") != null && nodeA.Attribute("Line") != null
                        && nodeB.Attribute("Line").Value == nodeA.Attribute("Line").Value;

            return result;
        }

        public bool EqualsById(XElement nodeA, XElement nodeB)
        {
            var result = false;

            if (nodeB.HasAttributes && nodeA.HasAttributes)
            {
                if (nodeB.Attribute("id") != null && nodeA.Attribute("id") != null)
                {
                    result = nodeB.Attribute("id").Value == nodeA.Attribute("id").Value;
                }
                else if (nodeB.Attribute("ID") != null && nodeA.Attribute("ID") != null)
                {
                    result = nodeB.Attribute("ID").Value == nodeA.Attribute("ID").Value;
                }
                else if (nodeB.Attribute("Id") != null && nodeA.Attribute("Id") != null)
                {
                    result = nodeB.Attribute("Id").Value == nodeA.Attribute("Id").Value;
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

        public static bool IsEqualsByAttributes(XElement nodeA, XElement nodeB)
        {

            if (nodeA.HasAttributes && nodeB.HasAttributes)
            {
                return nodeA.Attributes()
                    .All(attrA => !nodeB.Attributes().Any(attrB => attrB.Name.LocalName == attrA.Name.LocalName && attrA.Value != attrB.Value));
            }
            else if (!nodeA.HasAttributes && nodeB.HasAttributes
              || nodeA.HasAttributes && !nodeB.HasAttributes)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsIdenticalScores(XElement nodeA, XElement nodeB)
        {
            var periodB = nodeB.Attribute("period");

            if (periodB != null)
            {
                var periodA = nodeA.Attribute("period");

                return periodA != null
                       && periodB.Value == periodA.Value;
            }
            else
            {
                periodB = nodeB.Attribute("Period");

                if (periodB != null)
                {
                    var periodA = nodeA.Attribute("Period");

                    return periodA != null
                            && periodB.Value == periodA.Value;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsIdenticalParticipants(XElement nodeA, XElement nodeB)
        {
            var NumberA = nodeA.Attribute("Number");
            var NumberB = nodeB.Attribute("Number");

            if (NumberA != null && NumberB != null
                && NumberA.Value == NumberB.Value)
            {
                var NameA = nodeA.Attribute("Name");
                var NameB = nodeB.Attribute("Name");

                return NameA != null && NameB != null
                       && NameA.Value == NameB.Value;
            }
            else
            {
                return false;
            }
        }

        public bool IsIdenticalPlayers(XElement nodeA, XElement nodeB)
        {
            var TeamA = nodeA.Attribute("Team");
            var TeamB = nodeB.Attribute("Team");

            if (TeamA != null && TeamB != null
                        && TeamA.Value == TeamB.Value)
            {
                var PlayerNameA = nodeA.Attribute("PlayerName");
                var PlayerNameB = nodeB.Attribute("PlayerName");

                return PlayerNameB != null && PlayerNameA != null
                       && PlayerNameB.Value == PlayerNameA.Value;
            }
            else
            {
                return false;
            }
        }


        public bool IsIdenticalStatistics(XElement nodeA, XElement nodeB)
        {
            var NameA = nodeA.Attribute("Name");
            var NameB = nodeB.Attribute("Name");

            var result = NameA != null && NameB != null
                         && NameA.Value == NameB.Value;

            return result;
        }
        public bool IsIdenticalRunners(XElement nodeA, XElement nodeB)
        {
            var ProgramNumberA = nodeA.Attribute("ProgramNumber");
            var ProgramNumberB = nodeB.Attribute("ProgramNumber");

            return ProgramNumberA != null && ProgramNumberB != null
                         && ProgramNumberA.Value == ProgramNumberB.Value;

        }

        public bool IsIdenticalScorers(XElement nodeA, XElement nodeB)
        {
            var nameA = nodeA.Attribute("name");
            var nameB = nodeB.Attribute("name");

            if (nameA != null && nameB != null
                        && nameA.Value == nameB.Value)
            {
                var timeA = nodeA.Attribute("time");
                var timeB = nodeB.Attribute("time");

                return timeB != null && timeA != null
                       && timeB.Value == timeA.Value;
            }
            else
            {
                return false;
            }
        }

        public bool IsIdenticalCards(XElement nodeA, XElement nodeB)
        {
            var nameA = nodeA.Attribute("time");
            var nameB = nodeB.Attribute("time");

            return nameA != null && nameB != null
                         && nameA.Value == nameB.Value;
        }

        public void MergeNodesAttributes(XElement nodeA, XElement nodeB, string lastUpdate)
        {
            var attributeLastUpdate = lastUpdate;

            var addDvsLastUpdate = true;

            if (nodeB.HasAttributes)
            {
                foreach (var attrB in nodeB.Attributes())
                {
                    if (attrB.Name.LocalName == XmlLastupdateAttribbute_v1 || attrB.Name.LocalName == XmlLastupdateAttribbute_v2)
                    {
                        addDvsLastUpdate = false;
                        attributeLastUpdate = attrB.Value;
                        break;
                    }
                }
            }

            List<XAttribute> changedAttributes = new List<XAttribute>();
            List<XAttribute> newAttributes = new List<XAttribute>();

            if (nodeB.HasAttributes)
            {
                foreach (var attrB in nodeB.Attributes())
                {
                    if (attrB.Name.LocalName == "MsgGuid" || attrB.Name.LocalName == "MsgID")
                    {
                        continue;
                    }

                    bool isConstains = false;

                    if (nodeA.HasAttributes)
                    {
                        foreach (var attrA in nodeA.Attributes())
                        {
                            if (attrA.Name.LocalName == attrB.Name.LocalName)
                            {
                                isConstains = true;

                                if (attrA.Value != attrB.Value)
                                {
                                    changedAttributes.Add(new XAttribute(attrA.Name.LocalName, attrA.Value));

                                    attrA.Value = attrB.Value;
                                }// else pass
                            }// else pass
                        }

                        if (!isConstains)
                        {
                            newAttributes.Add(new XAttribute(attrB.Name.LocalName, attrB.Value));

                            var attrInParent = new XAttribute(attrB.Name.LocalName, attrB.Value);

                            nodeA.Add(attrInParent);
                        }
                    }
                }
            }


            if (changedAttributes.Count > 0)
            {
                var first = changedAttributes.First();
                if (changedAttributes.Count == 1 && (first.Name.LocalName == XmlLastupdateAttribbute_v1 || first.Name.LocalName == XmlLastupdateAttribbute_v2))
                {
                    // pass
                }
                else
                {
                    AddHistory(nodeA, DvsHistoryOldValue, attributeLastUpdate, addDvsLastUpdate, changedAttributes);
                }
            }// pass

            if (newAttributes.Count > 0)
            {
                AddHistory(nodeA, DvsHistoryAddValue, attributeLastUpdate, true, newAttributes);
            }// pass
        }

        public void AddHistory(XElement nodeA, string historyName, string attributeLastUpdate, bool addHistoryLastUpdateAttr, List<XAttribute> attributes)
        {
            var historyAttribute = nodeA.XPathSelectElement(DvsHistoryAttributes);
            XElement historyNode;

            if (historyAttribute == null)
            {
                historyNode = new XElement(DvsHistoryAttributes);
                nodeA.Add(historyNode);
            }
            else
            {
                historyNode = historyAttribute;
            }

            var oldInnerTextNode = new XElement(historyName); 

            if (addHistoryLastUpdateAttr)
            {
                if (string.IsNullOrEmpty(attributeLastUpdate))
                {
                    attributeLastUpdate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff");
                }

                attributes.Add(new XAttribute(DvsLastupdateAttribbute, attributeLastUpdate));
            }

            foreach (var a in attributes)
            {
                oldInnerTextNode.Add(a);
            }

            historyNode.Add(oldInnerTextNode);
        }

        public static bool IsHistoryNode(string name)
        {
            return (name == DvsHistoryValues || name == DvsHistoryAttributes);
        }

    }
}