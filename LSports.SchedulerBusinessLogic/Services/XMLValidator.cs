

using LSports.DVS.Framework.DataAccess.Models;

namespace LSports.Scheduler.Services
{
    using LSports.DVS.Framework.DataAccess.Repositories;
    using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
    using LSports.Framework.Models.CustomClasses;
    using LSports.Framework.Models.Enums;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Xml;
    using WebGrease.Css.Extensions;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using System.Collections;
    using System.Xml.XPath;

    using System.Collections.Concurrent;
    using System.Configuration;
    using System.Reflection;
    using LSports.DVS.Framework.DataAccess.CustomClasses;
    using Slack.Webhooks;

    using static System.String;
    using log4net;

    public class XmlValidator
    {
        private readonly IValidationResultRepository _validationResultsRepository;

        public XmlValidator(IValidationResultRepository validationResultsRepo)
        {
            _validationResultsRepository = validationResultsRepo;
        }

        protected readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public XmlValidator()
            : this(new ValidationResultRepository())
        {
        }

        public void ValidateAll(int productId)
        {
        }

        // did't optomized
        public void SendPostMessages(IList<ValidationSetting> validationSettings, string validationHashIndex)
        {
            var slackEnabledSettings = validationSettings.Where(x => x.IsSlackEnabled && !string.IsNullOrEmpty(x.SlackChannel)).ToList();

            if (slackEnabledSettings.Count > 0)
            {
                var newValidationResults = _validationResultsRepository.GetLastValidationResults(validationHashIndex, slackEnabledSettings);

                var slackDefaultChannel = ConfigurationManager.AppSettings[SchedulerConfig.SlackDefaultChannelAlias];

                string webHook = ConfigurationManager.AppSettings[SchedulerConfig.SlackWebhookAlias];
                string dvsHost = ConfigurationManager.AppSettings[SchedulerConfig.DvsHostAlias];

                var slackClient = new Slack.Webhooks.SlackClient(webHook);

                foreach (var slackEnabledSetting in slackEnabledSettings)
                {
                    var settingResults = newValidationResults.Where(x => x.ValidationSettingId == slackEnabledSetting.Id).ToList();

                    var channel = slackEnabledSetting.SlackChannel;

                    if (string.IsNullOrEmpty(channel))
                    {
                        channel = slackDefaultChannel;
                    }

                    if (settingResults.Count > 5)
                    {
                        var slackMessage = new SlackMessage
                        {
                            Channel = channel,
                            Text = $"New alerts: product #{slackEnabledSetting.ProductId}, setting #{slackEnabledSetting.Id} '{slackEnabledSetting.Name}', count: {settingResults.Count} for event(s): {string.Join(", ", settingResults.Select(x=>x.EventId).Distinct())}",
                            IconEmoji = Emoji.Bug,
                            Username = "DVS-bot"
                        };

                        slackClient.PostAsync(slackMessage);
                    }
                    else
                    {
                        foreach (var result in settingResults)
                        {
                            var message = $"New alert: product #{result.ProductId}, setting #{slackEnabledSetting.Id} '{slackEnabledSetting.Name}'";

                            if (!string.IsNullOrEmpty(result.Market))
                            {
                                message += $"'({result.Market}";

                                if (!string.IsNullOrEmpty(result.Provider))
                                {
                                    message += $",{result.Provider})'";
                                }
                                else
                                {
                                    message += ")'";
                                }
                            }

                            message += " see " + dvsHost + "/ValidationResults/ForProduct/" + result.ProductId + "#" +
                                       slackEnabledSetting.Id + "_" + result.Id;

                            var slackMessage = new SlackMessage
                            {
                                Channel = channel,
                                Text = message,
                                IconEmoji = Emoji.Bug,
                                Username = "DVS-bot"
                            };

                            slackClient.PostAsync(slackMessage);
                        }
                    }
                   
                }
            }
        }

        public void ValidateEventsForProduct(IList<Event> events, IList<ValidationSetting> validationSettings)
        {
            var validationHashIndex = Guid.NewGuid().ToString("N");


            ValidateEventsCommonParallel(events, validationSettings, validationHashIndex);

            SendPostMessages(validationSettings, validationHashIndex);
        }

        public string GetNodeIdentifier(XElement node, ref bool isFalseIdentificator)
        {
            isFalseIdentificator = false;

            if (node == null)
            {
                return "";
            }

            var namedItem = node.Attribute("id");

            if (namedItem == null)
            {
                namedItem = node.Attribute("Id");
            } // pass
            if (namedItem == null)
            {
                namedItem = node.Attribute("Number");
            } // pass
            if (namedItem == null)
            {
                namedItem = node.Attribute("Name");
            } // pass
            if (namedItem == null)
            {
                namedItem = node.Attribute("name");
            } // pass

            if (namedItem == null && node.HasAttributes)
            {
                namedItem = node.Attributes().First();
            } // pass

            if (namedItem != null)
            {
                return namedItem.Name + "=\"" + namedItem.Value + "\"";
            }
            else
            {
                if (node.HasAttributes)
                {
                    StringBuilder identifier = new StringBuilder();

                    foreach (var xAttribute in node.Attributes())
                    {
                        identifier.Append(xAttribute.Value);
                    }

                    var sId = identifier.ToString();

                    if (sId != "")
                    {
                        isFalseIdentificator = true;
                    }

                    return sId;
                }
                else
                {
                    return "";
                }
            }
        }
        
        public void ValidateEventsCommonParallel(IList<Event> events, IList<ValidationSetting> validationSettings, string validationHashIndex )
        {
            _log.Info("Events for validation: " + events.Count + " / " + validationSettings.Count);

            var tasks = new List<Task>();

            // var results = ValidateEventsCommon(events, validationSettings);
            
            var valResultsBug = new ConcurrentBag<ValidationResultsHashModel>();
            var takingCount = 100;

            for (var i = 0; i <= events.Count / takingCount; i++)
            {
                var validateEvents = events.Skip(i * takingCount).Take(takingCount).ToList();

                if (validateEvents.Count > 0)
                {
                    var task = Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            _validationResultsRepository.DisableOldValidationResults(validateEvents);
                            
                            var newResults = ValidateEventsCommon(validateEvents, validationSettings);
                            
                            if (newResults.Count > 0)
                            {
                                foreach (var result in newResults)
                                {
                                    valResultsBug.Add(result);
                                }

                                _log.Info("Added validation results to repository: " + newResults.Count);
                            }// else pass
                        }
                        catch (Exception e)
                        {
                            _log.Error( e.Message + " " + e.StackTrace );
                        }
                    });

                    tasks.Add(task);
                }
            }
            
            Task.WaitAll(tasks.ToArray());

            _validationResultsRepository.BulkInsertToPull(valResultsBug.ToList(), validationHashIndex);

        }

        public List<ValidationResultsHashModel> ValidateEventsCommon(IList<Event> events, IList<ValidationSetting> validationSettings)
        {
            //validationSettings = validationSettings.Where(x => x.Id == 88).ToList();

            var resultsForSaving = new List<ValidationResultsHashModel>();

            var currentNodesFoundedCount = new Dictionary<int, Dictionary<string, int>>();

            foreach (var validationSetting in validationSettings)
            {
                foreach (var validationSettingValidationRule in validationSetting.ValidationRules)
                {
                    if (currentNodesFoundedCount.ContainsKey(validationSettingValidationRule.Id) == false)
                    {
                        currentNodesFoundedCount.Add(validationSettingValidationRule.Id*1, new Dictionary<string, int>());
                    }
                }
            }

            foreach (var @event in events)
            {
                XDocument xmlDocument = @event.XmlTextExample;

                foreach (var setting in validationSettings)
                {
                    var isEmptySettingExpression = IsNullOrEmpty(setting.Expression);

                    var ruleResults = new List<RuleResultDTO>();

                    bool isFoundedForAllRules = true;

                    var canAggregateResults = true;

                    foreach (var validationRule in setting.ValidationRules)
                    {
                        currentNodesFoundedCount[validationRule.Id] = new Dictionary<string, int>();

                        var validatedNodesForNewRule = new List<XElement>();

                        if (isEmptySettingExpression && setting.IsContainsRelatedRules)
                        {
                            var relatedRulesFromPropertyToValue = setting.ValidationRules
                                .Where(x => x.PathToValueParentNode != null 
                                            && x.PathToValueParentNode == validationRule.PathToPropertyParentNode
                                            && x.Id != validationRule.Id)
                                .ToList();


                            if (relatedRulesFromPropertyToValue.Count > 0)
                            {
                                var relatedRulesIds = relatedRulesFromPropertyToValue
                                    .Select(x => x.Id)
                                    .ToArray();

                                var relatedForRuleIssues = ruleResults
                                    .Where(x => relatedRulesIds.Contains(x.Rule.Id) && x.IsIssue)
                                    .ToList();

                                if (relatedForRuleIssues.Count > 0)
                                {
                                    validatedNodesForNewRule = relatedForRuleIssues
                                        .Where(x => x.ParentValidatedXElement != null)
                                        .Select(x => x.ParentValidatedXElement)
                                        .ToList();
                                }
                            }
                        }

                        var xPathArray = validationRule.PropertyXPathForValidator;

                        var length = xPathArray.Length;

                        string parentNodeName = "";
                        string nodeName = xPathArray[length - 1];

                        if (length > 2)
                        {
                            parentNodeName = xPathArray[length - 2];
                        }

                        List<XElement> nodesToValidate;
                        if (validatedNodesForNewRule.Count > 0)
                        {
                            nodesToValidate = validatedNodesForNewRule;
                        }
                        else
                        {
                            xPathArray = length > 2 ? xPathArray.Take(length - 1).ToArray() : xPathArray;

                            var pathToParentNode = Join("/", xPathArray);

                            nodesToValidate = xmlDocument.XPathSelectElements(pathToParentNode).ToList();
                        }

                        var nodesExists = nodesToValidate != null
                                          && nodesToValidate.Count > 0;

                        if (nodesExists)
                        {
                            switch ((ValidationOperatorId)validationRule.OperatorId)
                            {
                                case ValidationOperatorId.PercentOfChangesMoreThan:
                                {
                                    ValidatePercentOfChangesMoreThan(nodesToValidate, validationRule, nodeName, ruleResults, @event, parentNodeName, currentNodesFoundedCount);
                                    break;
                                }
                                case ValidationOperatorId.DifferenceBetweenTheLastEquals:
                                {
                                    //currentNodesFoundedCount[validationRule.Id] = nodesToValidate.Count;
                                    ValidateDifferenceBetweenTheLast(nodesToValidate, validationRule, nodeName,
                                    ruleResults, @event, parentNodeName, currentNodesFoundedCount);
                                    break;
                                }
                                case ValidationOperatorId.DifferenceBetweenTheLastLessThan:
                                {
                                    //currentNodesFoundedCount[validationRule.Id] = nodesToValidate.Count;
                                    ValidateDifferenceBetweenTheLast(nodesToValidate, validationRule, nodeName,
                                    ruleResults, @event, parentNodeName, currentNodesFoundedCount);
                                    break;
                                }
                                case ValidationOperatorId.DifferenceBetweenTheLastMoreThan:
                                {
                                    //currentNodesFoundedCount[validationRule.Id] = nodesToValidate.Count;
                                    ValidateDifferenceBetweenTheLast(nodesToValidate, validationRule, nodeName,
                                    ruleResults, @event, parentNodeName, currentNodesFoundedCount);
                                    break;
                                }
                                default:
                                {
                                    //currentNodesFoundedCount[validationRule.Id] = nodesToValidate.Count;

                                    ValidateEachNodes(nodesToValidate, setting, validationRule, xmlDocument, nodeName,
                                        ruleResults, @event, parentNodeName, currentNodesFoundedCount, isEmptySettingExpression, ref isFoundedForAllRules);
                                    break;
                                }
                            };
                        }
                        else
                        {
                            if (validationRule.ParameterId.Value == (int) ValidationParameterId.Value
                                && (ValidationOperatorId) validationRule.OperatorId == ValidationOperatorId.NotExist)
                            {
                                _validationResultsRepository.NewRuleResultFrom(@event,
                                    null,
                                    null,
                                    validationRule, true,
                                    new PointToHighline(), parentNodeName, currentNodesFoundedCount, ref ruleResults);
                            }
                            else
                            {
                                isFoundedForAllRules = false;
                                // SetFalseFoundedForAllRules(ref isFoundedForAllRules, null, @event, ruleResults, validationRule, parentNodeName, currentNodesFoundedCount );
                            }// pass
                        }

                        if (!isFoundedForAllRules && isEmptySettingExpression)
                        {
                            canAggregateResults = false;
                            break;
                        }
                    }

                    if (canAggregateResults && ruleResults.Count > 0)
                    {
                        resultsForSaving.AddRange(GetAggregatedValidationResults(ruleResults, setting, currentNodesFoundedCount, @event, isFoundedForAllRules, isEmptySettingExpression));
                    }
                }
            }

            return resultsForSaving;
        }

        public void SetFalseFoundedForAllRules(ref bool isFoundedForAllRules, XElement checkingNode, Event @event, List<RuleResultDTO> ruleResults, ValidationRule validationRule, string parentNodeName, Dictionary<int,Dictionary<string, int>> currentNodeFoundedCount)
        {
            isFoundedForAllRules = isFoundedForAllRules && false;
        }

        public void ValidateDifferenceBetweenTheLast(List<XElement> nodesToValidate,
            ValidationRule validationRule,
            string nodeName, List<RuleResultDTO> ruleResults, Event @event, string parentNodeName, Dictionary<int, Dictionary<string, int>> currentNodeFoundedCount)
        {
            var parsedValue = 0.0;
            var isPArsedValue = double.TryParse(validationRule.Value, out parsedValue);

            if (isPArsedValue)
            {
                foreach (var xElement in nodesToValidate)
                {
                    var nodeOrAttr = GetChildNodeByName(xElement, nodeName);

                    if (nodeOrAttr != null)
                    {
                        var isAttribute = nodeOrAttr is XAttribute;
                        var historiesNodeName = isAttribute ? XmlMerger.DvsHistoryAttributes : XmlMerger.DvsHistoryValues;

                        XElement withHistoryNode = isAttribute ? nodeOrAttr.Parent : (XElement)nodeOrAttr;

                        if (withHistoryNode != null)
                        {
                            var historyNode = withHistoryNode.XPathSelectElement(historiesNodeName);

                            var oldNode = historyNode
                                ?.Elements(XmlMerger.DvsHistoryOldValue)
                                ?.LastOrDefault(x => x.Attribute(nodeName) != null);// only last!

                            if (oldNode != null && oldNode.HasAttributes)
                            {

                                var xAttribute = oldNode.Attribute(nodeName);

                                if (xAttribute != null)
                                {
                                    var isIssue = false;

                                    var value = xAttribute.Value;
                                    var checkingValue = XmlParser.GetNodeHistoryLastValue(nodeOrAttr);

                                    double parsedCheckingValue = 0.0;
                                    var parsed = double.TryParse(checkingValue, out parsedCheckingValue);

                                    if (parsed)
                                    {
                                        double oldValue = 0.0;
                                        parsed = double.TryParse(value, out oldValue);
                                        if (parsed)
                                        {
                                            isIssue = ValidateDifferenceBetweenNumbers(parsedCheckingValue, oldValue, parsedValue, validationRule.OperatorId);
                                        }
                                    }

                                    _validationResultsRepository.NewRuleResultFrom(@event,
                                        nodeOrAttr, xElement, validationRule, isIssue,
                                        GetNodeToHihgline(nodeOrAttr, checkingValue), parentNodeName,
                                        currentNodeFoundedCount, ref ruleResults);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void ValidatePercentOfChangesMoreThan(List<XElement> nodesToValidate,
            ValidationRule validationRule,
            string nodeName, List<RuleResultDTO> ruleResults, Event @event, string parentNodeName,
            Dictionary<int, Dictionary<string, int>> currentNodeFoundedCount)
        {
            var currentResults = new List<RuleResultDTO>();

            foreach (var xElement in nodesToValidate)
            {
                var nodeOrAttr = GetChildNodeByName(xElement, nodeName);

                var isAttribute = nodeOrAttr is XAttribute;
                var historiesNodeName = isAttribute ? XmlMerger.DvsHistoryAttributes : XmlMerger.DvsHistoryValues;

                XElement withHistoryNode = isAttribute ? nodeOrAttr.Parent : (XElement)nodeOrAttr;

                var historyNode = withHistoryNode.XPathSelectElement(historiesNodeName);

                var oldNode = historyNode
                    ?.Elements(XmlMerger.DvsHistoryOldValue)
                    ?.LastOrDefault(x => x.Attribute(nodeName) != null);// only last!

                if (oldNode != null && oldNode.HasAttributes)
                {
                    var attr = oldNode.Attribute(nodeName);
                    if (attr != null)
                    {
                        var checkingValue = XmlParser.GetNodeHistoryLastValue(nodeOrAttr);
                        _validationResultsRepository.NewRuleResultFrom(@event,
                                nodeOrAttr, xElement, validationRule, attr.Value != checkingValue,
                                GetNodeToHihgline(nodeOrAttr, checkingValue), parentNodeName, currentNodeFoundedCount, ref currentResults);
                    }
                }
            }

            if (currentResults.Count > 0)
            {
                var valueInPercent = 0.0;

                double.TryParse(validationRule.Value, out valueInPercent);

                var groupedByMarket = currentResults.GroupBy(x => x.Market);
                foreach (var groupByMarket in groupedByMarket)
                {
                    var groupedByProvider = groupByMarket.GroupBy(x => x.Provider);

                    foreach (var groupByProvider in groupedByProvider)
                    {
                        var totalCount = groupByProvider.Count();

                        if (totalCount > 0)
                        {
                            var issues = groupByProvider.Where(x => x.IsIssue).ToArray();

                            var percent = 1.0 * 100 * (issues.Length / totalCount);
                            if (percent > valueInPercent)
                            {
                                ruleResults.AddRange(issues);
                            }
                        }
                    }
                }
            }
        }

        public void ValidateEachNodes(List<XElement> nodesToValidate,
            ValidationSetting setting,
            ValidationRule validationRule,
            XDocument xmlDocument,
            string nodeName,  List<RuleResultDTO> ruleResults, Event @event, string parentNodeName,
            Dictionary<int, Dictionary<string, int>> currentNodeFoundedCount,
            bool isEmptySettingExpression,
            ref bool isFoundedForAllRules)
        {
            foreach (var checkingNode in nodesToValidate)
            {
                var nodeOrAttr = GetChildNodeByName(checkingNode, nodeName);

                var checkingValue = "";

                if (nodeOrAttr != null)
                {
                    checkingValue = XmlParser.GetNodeHistoryLastValue(nodeOrAttr);
                }

                if (validationRule.ParameterId.Value == (int)ValidationParameterId.Value)
                {
                    switch ((ValidationOperatorId)validationRule.OperatorId)
                    {
                        case ValidationOperatorId.Exists:
                            _validationResultsRepository.NewRuleResultFrom(@event,
                                nodeOrAttr, checkingNode,
                                validationRule, nodeOrAttr != null,
                                GetNodeToHihgline(nodeOrAttr, checkingValue), parentNodeName, currentNodeFoundedCount, ref ruleResults);
                            continue;

                        case ValidationOperatorId.NotExist:

                            var isFalseIdentificator = false;
                            var identifier = GetNodeIdentifier(checkingNode, ref isFalseIdentificator);

                            _validationResultsRepository.NewRuleResultFrom(@event,
                                    checkingNode, checkingNode,
                                    validationRule, nodeOrAttr == null, new PointToHighline
                                    {
                                        ParentNodeName = parentNodeName,
                                        ParentNodeIdentifier = identifier,
                                    // ParentNodeName = parentNodeName,
                                    Point = identifier,
                                        isFalseIdentificator = isFalseIdentificator
                                    // ParentNodeIdentifier = pointToHighline
                                }, parentNodeName, currentNodeFoundedCount, ref ruleResults);
                            continue;
                    }
                } // pass

                if (nodeOrAttr != null)
                {
                    bool isIssuesCreated = false;
                    bool isValidationIssue = true;

                    switch ((ValidationParameterId)validationRule.ParameterId.Value)
                    {
                        case ValidationParameterId.Value:

                            ValidateValue(nodeOrAttr, checkingNode, checkingValue, parentNodeName, setting, @event,
                                validationRule, xmlDocument, currentNodeFoundedCount, isEmptySettingExpression, ref ruleResults, ref isFoundedForAllRules);

                            isIssuesCreated = true;
                            break;

                        case ValidationParameterId.Length:
                            isValidationIssue = ValidateLength(checkingValue.Length, validationRule);
                            break;

                        case ValidationParameterId.DataType:
                            isValidationIssue = ValidateType(checkingValue, validationRule);
                            break;

                        case ValidationParameterId.NumberOfChanges:
                            isValidationIssue = ValidateNumberOfChanges(nodeName, nodeOrAttr,
                                nodeOrAttr is XAttribute, validationRule);
                            break;

                        case ValidationParameterId.NumberOfDistinctChanges:
                            isValidationIssue = ValidateNumberOfDistinctChanges(nodeName, nodeOrAttr,
                                nodeOrAttr is XAttribute, validationRule);
                            break;

                        default:
                            throw new NotImplementedException(
                                "Not realized validation parameter ValidateEventErrors by Id : " +
                                validationRule.ParameterId.Value);
                    }

                    if (!isIssuesCreated)
                    {
                        _validationResultsRepository.NewRuleResultFrom(@event,
                            nodeOrAttr, checkingNode, validationRule, isValidationIssue,
                            GetNodeToHihgline(nodeOrAttr, checkingValue), parentNodeName, currentNodeFoundedCount, ref ruleResults);
                    }
                }
                else
                {
                    isFoundedForAllRules = false;
                    // SetFalseFoundedForAllRules(ref isFoundedForAllRules, checkingNode, @event, ruleResults, validationRule, parentNodeName, currentNodeFoundedCount);
                }
            }
        }

        public PointToHighline GetNodeToHihgline(XObject checkingNode, string checkingValue)
        {
            if (checkingNode == null)
            {
                return new PointToHighline()
                {
                    ParentNodeIdentifier = "",
                    ParentNodeName = "",
                    isFalseIdentificator = false,
                    Point = ""
                };
            }

            var point = new PointToHighline();
            
            XElement ownerElement = null;
            var isAttribute = checkingNode is XAttribute;

            XElement parentNode = null;

            if (isAttribute)
            {
                ownerElement = checkingNode.Parent;
                parentNode = ownerElement;
            }
            else
            {
                if (checkingNode is XText)
                {
                    parentNode = (checkingNode as XText).Parent;
                }
                else
                {
                    parentNode = (checkingNode as XElement);//.Parent;
                }
            }
            
            var identifier = "";
            
            if ( parentNode.HasAttributes)
            {
                var isFalseIdentificator = false;

                identifier = GetNodeIdentifier(parentNode, ref isFalseIdentificator);
                point.isFalseIdentificator = isFalseIdentificator;
            }

            point.ParentNodeIdentifier = identifier;

            if (isAttribute)
            {
                point.ParentNodeName = ownerElement.Name.LocalName;

                point.Point = (checkingNode as XAttribute).Name.LocalName + "=" + '"' + checkingValue + '"';
            }
            else
            {
                point.ParentNodeName = parentNode.Name.LocalName;

                point.Point = checkingValue;
            }

            return point;
        }

        public int CountIssuesEqualsByRuleId(ArrayList rulesResults, int ruleId)
        {
            var counter = 0;
            for (var i = 0; i < rulesResults.Count; i++)
            {
                var ruleResult = rulesResults[i] as RuleResultDTO;
                if (ruleResult.IsIssue && ruleResult.Rule.Id == ruleId)
                {
                    counter++;
                }
            }

            return counter;
        }

        public bool AnyIssueEqualsByRuleId(ArrayList rulesResults, int ruleId)
        {
            for (var i = 0; i < rulesResults.Count; i++)
            {
                var ruleResult = rulesResults[i] as RuleResultDTO;
                if (ruleResult.IsIssue && ruleResult.Rule.Id == ruleId)
                {
                    return true;
                }
            }

            return false;
        }
        

        public bool IsResolveExpression(bool isFoundedForAllRules, ArrayList rulesResults,
            ValidationSetting setting, 
            Dictionary<int, Dictionary<string, int>> currentNodesFoundedCount, 
            bool isEmptySettingExpression)
        {
            if (isEmptySettingExpression)
            {
                if (setting.IsContainsRuleForAllNodes)
                {
                    if (!isFoundedForAllRules)
                    {
                        return false;
                    }
                    else
                    {
                        foreach (var validationRule in setting.ValidationRules)
                        {
                            var isResolve = true;

                            if (validationRule.IsForAllNodes)
                            {
                                var firstWithMarketProvider = rulesResults.Cast<RuleResultDTO>()
                                    .FirstOrDefault(x => x.Rule.Id == validationRule.Id);

                                var countOfAllNodes = 0;

                                var marketProviderKey = firstWithMarketProvider != null ?
                                    firstWithMarketProvider.Market + "-" + firstWithMarketProvider.Provider
                                    : "-";

                                if (currentNodesFoundedCount[validationRule.Id].ContainsKey(marketProviderKey))
                                {
                                    countOfAllNodes = currentNodesFoundedCount[validationRule.Id][marketProviderKey];
                                };

                                var count = CountIssuesEqualsByRuleId(rulesResults, validationRule.Id);
                                bool isNotExistRule = (ValidationOperatorId)validationRule.OperatorId ==
                                                      ValidationOperatorId.NotExist;

                                if (!((countOfAllNodes == 0 && isNotExistRule) || (countOfAllNodes != 0 && countOfAllNodes == count)))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                isResolve = AnyIssueEqualsByRuleId(rulesResults, validationRule.Id);//rulesResults.Any(x => x.IsIssue && x.RuleId == validationRule.Id);
                            }

                            if (!isResolve)
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                }
                else
                {
                    if (isFoundedForAllRules)
                    {
                        var ruleResultsCasted = rulesResults.Cast<RuleResultDTO>().Where(x=>x.IsIssue);
                        foreach (var settingValidationRule in setting.ValidationRules)
                        {
                            if (!ruleResultsCasted.Any(x => x.Rule.Id == settingValidationRule.Id))
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                var copy = new StringBuilder(setting.Expression);

                foreach (var validationRule in setting.ValidationRules)
                {
                    bool isNotExistRule = (ValidationOperatorId)validationRule.OperatorId ==
                                          ValidationOperatorId.NotExist;

                    var firstWithMarketProvider = rulesResults.Cast<RuleResultDTO>().FirstOrDefault(x => x.Rule.Id == validationRule.Id);

                    var countOfAllNodes = 0;

                    var marketProviderKey = firstWithMarketProvider != null ? 
                        firstWithMarketProvider.Market + "-" + firstWithMarketProvider.Provider
                        : "-";

                    if (currentNodesFoundedCount[validationRule.Id].ContainsKey(marketProviderKey))
                    {
                        countOfAllNodes = currentNodesFoundedCount[validationRule.Id][marketProviderKey];
                    };

                    if (validationRule.IsForAllNodes == true)
                    {
                        var count = CountIssuesEqualsByRuleId(rulesResults, validationRule.Id);

                        if ((countOfAllNodes == 0 && isNotExistRule) || (countOfAllNodes != 0 && countOfAllNodes == count))
                        {
                            copy.Replace(validationRule.RuleName, "1");
                        }
                        else
                        {
                            copy.Replace(validationRule.RuleName, "0");
                        }
                    }
                    else
                    {
                        var isContains = true;

                        if (isNotExistRule)
                        {
                            if (countOfAllNodes == 0)
                            {
                                isContains = false;
                            }
                            else
                            {
                                isContains = AnyIssueEqualsByRuleId(rulesResults, validationRule.Id);
                            }
                        }
                        else
                        {
                            isContains = AnyIssueEqualsByRuleId(rulesResults, validationRule.Id);
                        }

                        copy = isContains
                            ? copy.Replace(validationRule.RuleName, "1")
                            : copy.Replace(validationRule.RuleName, "0");
                    }
                }

                var result = new BooleanEvaluator().Evaluate( copy);
                /*
                var @event = (rulesResults[0] as RuleResultDTO).Event;
                if (setting.Id == 88 && @event.EventId == 62313194)
                {
                    var xmlText = @event.XmlTextExample;

                    xmlText.Save("C:/test/" + @event.EventId + "_" + Guid.NewGuid().ToString());
                }
                */
                return result;
            }
        }

        public bool IsEqualsToEventsLastUpdate(string eventlastUpdate, XmlNode node)
        {
            DateTime eventLastUpdate;

            var isParsed = DateTime.TryParse(eventlastUpdate, out eventLastUpdate);

            if (isParsed && node.Attributes != null && (node.Attributes.GetNamedItem(XmlMerger.XmlLastupdateAttribbute_v1) != null || node.Attributes.GetNamedItem(XmlMerger.XmlLastupdateAttribbute_v2) != null))
            {
                var lastUpdate_v1 = node.Attributes.GetNamedItem(XmlMerger.XmlLastupdateAttribbute_v1);

                var nodeLastUpdateString = "";

                if (lastUpdate_v1 == null)
                {
                    var lastUpdate_v2 = node.Attributes.GetNamedItem(XmlMerger.XmlLastupdateAttribbute_v2);

                    nodeLastUpdateString = lastUpdate_v2.Value;
                }
                else
                {
                    nodeLastUpdateString = lastUpdate_v1.Value;
                }

                DateTime nodeLastUpdate;

                isParsed = DateTime.TryParse(nodeLastUpdateString, out nodeLastUpdate);

                if (isParsed)
                {
                    if (eventLastUpdate == nodeLastUpdate)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        public XObject GetChildNodeByName(XObject checkingNode, string nodeName)
        {
            XAttribute nodeOrAttr = null;
            var isXElement = checkingNode is XElement;

            if ( isXElement)
            {
                nodeOrAttr = ((XElement) checkingNode).Attribute(nodeName);

                if (nodeOrAttr == null && nodeName == "Value")
                {
                    var xText = (checkingNode as XElement).FirstNode;
                    if (xText is XText)
                    {
                        return (xText as XText);
                    }
                } else if (nodeOrAttr != null)
                {
                    return nodeOrAttr;
                }
            }

            XElement xElement = null;

            if (isXElement)
            {
                xElement = ((XElement) checkingNode).XPathSelectElement(nodeName);
            }


            if (xElement == null)
            {
                var nodes = (checkingNode as XElement).Nodes();
                foreach (var element in nodes)
                {
                    if (element.NodeType == XmlNodeType.Text)
                    {
                        return element;
                    } else if(element is XElement && (element as XElement).Name.LocalName == nodeName)
                    {
                        return element;
                    }
                }

                return null;
            }
            else
            {
                return xElement;
            }
        }

        public List<ValidationResultsHashModel> GetAggregatedValidationResults(
            List<RuleResultDTO> rulesResults,
            ValidationSetting setting,
            Dictionary<int, Dictionary<string, int>> currentNodesFoundedCount,
            Event @event,
            bool isFoundedForAllRules,
            bool isEmptySettingExpression)
        {
            var results = new List<ValidationResultsHashModel>();

            var withMarketDto = new List<RuleResultDTO>();
            var withoutMarketDto = new ArrayList();

            foreach (var x in rulesResults)
            {
                if (!IsNullOrEmpty(x.Market))
                {
                    withMarketDto.Add(x);
                }
                else
                {
                    withoutMarketDto.Add(x);
                }
            }

            if (withoutMarketDto.Count > 1)
            {
                var newWithoutMarketAndProviderDto = new ArrayList(15);

                var rootEventNodeName = "Event";

                newWithoutMarketAndProviderDto.AddRange(withoutMarketDto.Cast<RuleResultDTO>().Where(x => x.ParentNodeName == rootEventNodeName).ToList());

                var notForEventNode = withoutMarketDto.Cast<RuleResultDTO>().Where(x => x.ParentNodeName != rootEventNodeName);
                var groupedByParent = notForEventNode.GroupBy(x => x.ParentNodeName);

                foreach (var group in groupedByParent)
                {
                    var groupedByParentNodeIdentifier =
                        group.GroupBy(x => x.PointToHighline.ParentNodeNameAndParentNodeIdentifier);

                    var byParentNodeIdentifier = groupedByParentNodeIdentifier as IGrouping<string, RuleResultDTO>[] ?? groupedByParentNodeIdentifier.ToArray();
                    var maxCountInGroup = byParentNodeIdentifier.Max(y => y.Count());

                    byParentNodeIdentifier
                        .Where(x => x.Count() == maxCountInGroup)
                        .ForEach(x => newWithoutMarketAndProviderDto.AddRange(x.ToList()));
                }

                withoutMarketDto = newWithoutMarketAndProviderDto;
            }


            if (withMarketDto.Count > 0)
            {
                var marketGroups = withMarketDto.GroupBy(x => x.Market);

                foreach (var marketGroup in marketGroups)
                {
                    var marketIssues = marketGroup.Where(x => x.IsMarket).ToList();

                    var providerGroups = marketGroup.Where(x => x.IsMarket == false).GroupBy(x => x.Provider);

                    var arrayProvidersGroups = providerGroups as IGrouping<string, RuleResultDTO>[] ?? providerGroups.ToArray();

                    if (arrayProvidersGroups.Length > 0)
                    {
                        foreach (var providerGroup in arrayProvidersGroups)
                        {
                            var providerGroupList = providerGroup.Where(x => x.IsProvider == false).ToArray();
                            var providerIssues = providerGroup.Where(x => x.IsProvider).ToArray();

                            if (marketIssues.Count >0)
                            {
                                foreach (var ruleResultDto in marketIssues)
                                {
                                    ruleResultDto.Provider = providerGroup.Key;
                                }
                            }

                            if (providerGroupList.Length > 0)
                            {
                                RuleResultDTO[] withoutIsForAllNodes = null;
                                if (setting.IsContainsRuleForAllNodes)
                                {
                                    withoutIsForAllNodes = providerGroupList.Where(x => x.Rule.IsForAllNodes).ToArray();
                                    providerGroupList =
                                        providerGroupList.Where(x => x.Rule.IsForAllNodes == false).ToArray();
                                }

                                var checkingList = new ArrayList(20);

                                var groupedByParentNode = providerGroupList.GroupBy(x => x.PointToHighline.ParentNodeNameAndParentNodeIdentifier);
                                var byParentNodeIdentifier = groupedByParentNode as IGrouping<string, RuleResultDTO>[] ?? groupedByParentNode.ToArray();

                                if (byParentNodeIdentifier.Length > 0)
                                {
                                    var maxCountPointsForHighLine = byParentNodeIdentifier.Max(y => y.Count());
                                    groupedByParentNode = byParentNodeIdentifier.Where(x => x.Count() == maxCountPointsForHighLine);
                                };

                                if (!groupedByParentNode.Any())
                                {
                                    var newList = withoutIsForAllNodes==null ? new ArrayList(20) : new ArrayList(withoutIsForAllNodes);

                                    if (marketIssues.Count > 0)
                                    {
                                        newList.AddRange(marketIssues);
                                    }// pass

                                    if (providerIssues != null)
                                    {
                                        newList.AddRange(providerIssues);
                                    }// pass

                                    if (withoutMarketDto.Count > 0)
                                    {
                                        newList.AddRange(withoutMarketDto);
                                    }

                                    if (IsResolveExpression(isFoundedForAllRules, newList, setting, currentNodesFoundedCount, isEmptySettingExpression))
                                    {
                                        checkingList.AddRange(withoutIsForAllNodes);
                                    }
                                }
                                else
                                {

                                    foreach (var groupByParentNode in groupedByParentNode)
                                    {
                                        var newList = new ArrayList(groupByParentNode.ToList());

                                        if (marketIssues.Count > 0)
                                        {
                                            newList.AddRange(marketIssues);
                                        }// pass

                                        if (providerIssues != null && providerIssues.Length > 0)
                                        {
                                            newList.AddRange(providerIssues);
                                        }// pass


                                        if (withoutMarketDto.Count > 0)
                                        {
                                            newList.AddRange(withoutMarketDto);
                                        }

                                        if (withoutIsForAllNodes != null)
                                        {
                                            newList.AddRange(withoutIsForAllNodes);
                                        }

                                        if (IsResolveExpression(isFoundedForAllRules, newList, setting, currentNodesFoundedCount, isEmptySettingExpression))
                                        {
                                            checkingList.AddRange(groupByParentNode.ToList());
                                        }
                                    }

                                }

                                if (checkingList.Count > 0)
                                {

                                    if (marketIssues.Count > 0)
                                    {
                                        checkingList.AddRange(marketIssues);
                                    }// pass

                                    if (providerIssues != null)
                                    {
                                        checkingList.AddRange(providerIssues);
                                    }// pass

                                    if (withoutMarketDto.Count > 0)
                                    {
                                        checkingList.AddRange(withoutMarketDto);
                                    }// pass

                                    if (withoutIsForAllNodes != null)
                                    {
                                        checkingList.AddRange(withoutIsForAllNodes);
                                    }

                                    results.Add(_validationResultsRepository.NewRepositoryValidationResultFrom(checkingList, setting, @event, marketGroup.Key, providerGroup.Key)); // .Where(x => x.IsIssue).DistinctBy(x => x.Id).ToList()
                                }
                            }
                        }
                    }
                    else
                    {
                        var checkingList = new ArrayList(marketGroup.ToList());

                        if (withoutMarketDto.Count > 0)
                        {
                            checkingList.AddRange(withoutMarketDto);
                        }// pass

                        if (IsResolveExpression(isFoundedForAllRules, checkingList, setting, currentNodesFoundedCount, isEmptySettingExpression))
                        {
                            results.Add(_validationResultsRepository.NewRepositoryValidationResultFrom(checkingList, setting, @event, marketGroup.Key, ""));
                        }
                    }
                }
            }
            else
            {
                if (IsResolveExpression(isFoundedForAllRules,  withoutMarketDto, setting, currentNodesFoundedCount, isEmptySettingExpression))
                {
                    results.Add(_validationResultsRepository.NewRepositoryValidationResultFrom(withoutMarketDto, setting, @event, "", "")); 
                }
            }

            return results;
        }
        
        protected void ValidateValue(XObject checkingNode, XElement parentNode,
            string checkingValue,
            string parentNodeName,
            ValidationSetting setting,
            Event @event,
            ValidationRule validationRule,
            XDocument xmlDocument,
            Dictionary<int, Dictionary<string, int>> currentNodeFoundedCount,
            bool isEmptySettingExpression,
            ref List<RuleResultDTO> ruleResults,
            ref bool isFoundedForAllRules)
        {
            switch ((ValidationOperatorId)validationRule.OperatorId)
            {
                case ValidationOperatorId.Empty:
                    _validationResultsRepository.NewRuleResultFrom(@event, checkingNode, parentNode, 
                        validationRule, IsNullOrEmpty(checkingValue),
                        GetNodeToHihgline(checkingNode, checkingValue), parentNodeName, currentNodeFoundedCount, ref ruleResults);
                    return;
                case ValidationOperatorId.NotEmpty:
                    _validationResultsRepository.NewRuleResultFrom(@event, checkingNode, parentNode, 
                        validationRule, !IsNullOrEmpty(checkingValue),
                        GetNodeToHihgline(checkingNode, checkingValue), parentNodeName, currentNodeFoundedCount, ref ruleResults);
                    return;
            }

            bool isBySingleValue = !(validationRule.ValueXPathForValidator.Length > 0 && validationRule.ValueXPathForValidator[0] == "xml");
            var comparingValue = "";

            if (isBySingleValue)
            {
                comparingValue = validationRule.Value;

                if (validationRule.IsTime != null && (bool)validationRule.IsTime)
                {
                    var borderDate = GetBorderDateTime(validationRule);

                    DateTime nodeDate;

                    var isParsed = DateTime.TryParse(checkingValue, out nodeDate);

                    ValidationOperatorId operatorId = (ValidationOperatorId)validationRule.OperatorId;

                    var isIssue = isParsed && ValidateValueDates(borderDate, nodeDate, operatorId);

                    _validationResultsRepository.NewRuleResultFrom(@event, checkingNode, parentNode,
                        validationRule, isIssue,
                        GetNodeToHihgline(checkingNode, checkingValue), parentNodeName, currentNodeFoundedCount, ref ruleResults);
                }
                else
                {
                    var isValidationIssue = ValidateValueHelper(checkingValue, comparingValue, validationRule);

                    _validationResultsRepository.NewRuleResultFrom(@event, checkingNode, parentNode, validationRule, isValidationIssue,
                        GetNodeToHihgline(checkingNode, checkingValue), parentNodeName, currentNodeFoundedCount, ref ruleResults);
                }
            }
            else
            {
                var xPathArray = validationRule.ValueXPathForValidator;

                var length = xPathArray.Length;
                if (length > 1)
                {
                    string comparingNodeParentName = "";
                    string comparingNodeName = xPathArray[length - 1];

                    if (length > 1)
                    {
                        comparingNodeParentName = xPathArray[length - 2];
                    }
                    
                    List<XElement> nodesToValidate = null;
                    
                    var isFoundedRelatedAndYetValidated = false;
                    List<RuleResultDTO> relatedResults = null;
                     
                    if (isEmptySettingExpression && setting.IsContainsRelatedRules)
                    {
                        var relatedRulesFromValueToProperty = setting.ValidationRules
                                        .Where(x => x.PathToPropertyParentNode != null 
                                                    && x.PathToPropertyParentNode == validationRule.PathToValueParentNode
                                                    && x.Id != validationRule.Id)
                                        .ToList();

                        if (relatedRulesFromValueToProperty.Count > 0)
                        {
                            var relatedRulesIds = relatedRulesFromValueToProperty
                                .Select(x => x.Id)
                                .ToArray();

                            var relatedForRuleIssues = ruleResults
                                .Where(x => x.IsIssue && relatedRulesIds.Contains(x.Rule.Id) && x.ParentValidatedXElement != null)
                                .ToList();

                            if (relatedForRuleIssues.Count > 0)
                            {
                                relatedResults = relatedForRuleIssues;

                                nodesToValidate = relatedResults
                                    .Select(x => x.ParentValidatedXElement)
                                    .ToList();

                                isFoundedRelatedAndYetValidated = true;
                            }
                        }
                    }

                    if (!isFoundedRelatedAndYetValidated)
                    {
                        if (parentNodeName == comparingNodeParentName)
                        {
                            var gettingNode = checkingNode is XAttribute ? ((XAttribute)checkingNode).Parent : checkingNode as XElement;

                            nodesToValidate = new List<XElement> { gettingNode };
                        }
                        else
                        {
                            nodesToValidate = new List<XElement>(xmlDocument.XPathSelectElements(Join("/", xPathArray.Take(length - 1)))); ;
                        }
                    }

                    List<RuleResultDTO> currentResults = new List<RuleResultDTO>();
                    if (nodesToValidate.Count > 0)
                    {
                        foreach (var node in nodesToValidate)
                        {
                            var nodeOrAttr = GetChildNodeByName(node, comparingNodeName);

                            if (nodeOrAttr != null)
                            {
                                comparingValue = XmlParser.GetNodeHistoryLastValue(nodeOrAttr);

                                bool isValidationIssue = ValidateValueHelper(checkingValue, comparingValue, validationRule);

                                _validationResultsRepository.NewRuleResultFrom(@event, checkingNode, node, 
                                    validationRule, isValidationIssue,
                                    GetNodeToHihgline(checkingNode, checkingValue), parentNodeName, currentNodeFoundedCount, ref currentResults);
                            }
                        } // else pass

                        if (isFoundedRelatedAndYetValidated && currentResults.Count > 0 )
                        {
                            var currentResultsNodes = currentResults
                                .Select(x => x.ParentValidatedXElement)
                                .ToArray();

                            var forDeleting = relatedResults
                                .Where(x => !currentResultsNodes.Contains(x.ParentValidatedXElement));

                            foreach (var ruleResultDto in forDeleting)
                            {
                                ruleResults.Remove(ruleResultDto);
                            }
                        }

                        ruleResults.AddRange(currentResults);
                    }
                    else
                    {
                        isFoundedForAllRules = false;
                    }// else pass
                }// else pass

            }
        }

        protected bool ValidateValueHelper(string checkingValue, string valueToCompare, ValidationRule validationRule)
        {
            double doubleCheckingValue = 0;
            double doubleValueToCompare = 0;

            ValidationOperatorId operatorId = (ValidationOperatorId)validationRule.OperatorId;

            if (operatorId == ValidationOperatorId.StartsWith
                || operatorId == ValidationOperatorId.EndsWith
                || operatorId == ValidationOperatorId.NotEndingWith
                || operatorId == ValidationOperatorId.NotStartingWith
                || operatorId == ValidationOperatorId.NotContains
                || operatorId == ValidationOperatorId.Contains
                || operatorId == ValidationOperatorId.Regex
                || operatorId == ValidationOperatorId.Contains
                || operatorId == ValidationOperatorId.Contains)
            {
                return ValidateValueStrings(checkingValue, valueToCompare, operatorId);
            }


            var isNumberСheckingValue = double.TryParse(checkingValue, out doubleCheckingValue);
            var isNumberComparingValue = double.TryParse(valueToCompare, out doubleValueToCompare);

            if (isNumberСheckingValue && isNumberComparingValue)
            {
                return ValidateValueNumbers(doubleCheckingValue, doubleValueToCompare, validationRule, operatorId);
            }
            else
            {
                if (!isNumberComparingValue && isNumberСheckingValue || isNumberComparingValue && !isNumberСheckingValue)
				{
					return ValidateValueStrings(checkingValue, valueToCompare, operatorId);
				}
                else
                {
                    DateTime dateTimeCheckingValue;
                    DateTime dateTimeValueToCompare = DateTime.UtcNow;

                    var isDateCheckingValue = DateTime.TryParse(checkingValue, out dateTimeCheckingValue);
                    var isDateComparingValue = DateTime.TryParse(valueToCompare, out dateTimeValueToCompare);

                    if (isDateCheckingValue && isDateComparingValue)
                    {
                        return ValidateValueDates(dateTimeValueToCompare, dateTimeCheckingValue, operatorId);
                    }
                    else
                    {
                        return ValidateValueStrings(checkingValue, valueToCompare, operatorId);
                    }
                }
            }
        }

        protected bool ValidateValueStrings(string checkingValue, string valueToCompare, ValidationOperatorId operatorId)
        {
            switch (operatorId)
            {
                case ValidationOperatorId.GreaterThanOrEqualTo:
                    return CompareOrdinal(checkingValue, valueToCompare) >= 0;
                case ValidationOperatorId.GreaterThan:
                    return CompareOrdinal(checkingValue, valueToCompare) > 0;
                case ValidationOperatorId.LowerThanOrEqualTo:
                    return CompareOrdinal(checkingValue, valueToCompare) <= 0;
                case ValidationOperatorId.LowerThan:
                    return CompareOrdinal(checkingValue, valueToCompare) < 0;
                case ValidationOperatorId.StartsWith:
                    return checkingValue.StartsWith(valueToCompare);
                case ValidationOperatorId.NotStartingWith:
                    return !checkingValue.StartsWith(valueToCompare);
                case ValidationOperatorId.EndsWith:
                    return checkingValue.EndsWith(valueToCompare);
                case ValidationOperatorId.NotEndingWith:
                    return !checkingValue.EndsWith(valueToCompare);
                case ValidationOperatorId.Contains:
                    return checkingValue.Contains(valueToCompare);
                case ValidationOperatorId.NotContains:
                    return !checkingValue.Contains(valueToCompare);
                case ValidationOperatorId.Regex:
                    return Regex.IsMatch(checkingValue, valueToCompare, RegexOptions.CultureInvariant);
                case ValidationOperatorId.Equals:
                    return checkingValue == valueToCompare;
                case ValidationOperatorId.NotEqual:
                    return checkingValue != valueToCompare;
                case ValidationOperatorId.Between:
                    break;
                case ValidationOperatorId.NotBetween:
                    break;
            }

            return false;
        }


        protected bool ValidateValueDates(DateTime checkingValue, DateTime comparingValue, ValidationOperatorId operatorId)
        {
            switch (operatorId)
            {
                case ValidationOperatorId.GreaterThanOrEqualTo:
                    return checkingValue >= comparingValue;
                case ValidationOperatorId.GreaterThan:
                    return checkingValue > comparingValue;
                case ValidationOperatorId.LowerThanOrEqualTo:
                    return checkingValue <= comparingValue;
                case ValidationOperatorId.LowerThan:
                    return checkingValue < comparingValue;
                case ValidationOperatorId.Equals:
                    return checkingValue == comparingValue;
                case ValidationOperatorId.NotEqual:
                    return checkingValue != comparingValue;
            }

            return false;
        }

        protected bool ValidateDifferenceBetweenNumbers(double doubleCheckingValue, double doubleValueToCompare, double value, int? operatorId)
        {
            switch ((ValidationOperatorId)operatorId)
            {
                case ValidationOperatorId.DifferenceBetweenTheLastLessThan:
                    return Math.Abs(doubleCheckingValue - doubleValueToCompare) < value;
                case ValidationOperatorId.DifferenceBetweenTheLastMoreThan:
                    return Math.Abs(doubleCheckingValue - doubleValueToCompare) > value;
                case ValidationOperatorId.DifferenceBetweenTheLastEquals:
                    return Math.Abs(doubleCheckingValue - doubleValueToCompare - value) < EPSILON;
            }

            return false;
        }

        protected static double EPSILON = 0.001;

        protected bool ValidateValueNumbers(double doubleCheckingValue, double doubleValueToCompare, ValidationRule validationRule, ValidationOperatorId operatorId)
        {
            double left, right;
            switch (operatorId)
            {
                case ValidationOperatorId.GreaterThanOrEqualTo:
                    return doubleCheckingValue >= doubleValueToCompare;
                case ValidationOperatorId.GreaterThan:
                    return doubleCheckingValue > doubleValueToCompare;
                case ValidationOperatorId.LowerThanOrEqualTo:
                    return doubleCheckingValue <= doubleValueToCompare;
                case ValidationOperatorId.LowerThan:
                    return doubleCheckingValue < doubleValueToCompare;
                case ValidationOperatorId.Equals:
                    return Math.Abs(doubleCheckingValue - doubleValueToCompare) < EPSILON;
                case ValidationOperatorId.NotEqual:
                    return Math.Abs(doubleCheckingValue - doubleValueToCompare) > EPSILON;
                case ValidationOperatorId.Between:
                    var between = validationRule.Value.Split('|');
                    double.TryParse(between[0], out left);
                    double.TryParse(between[0], out right);
                    return doubleCheckingValue >= left && doubleCheckingValue <= right;
                case ValidationOperatorId.NotBetween:
                    between = validationRule.Value.Split('|');
                    double.TryParse(between[0], out left);
                    double.TryParse(between[0], out right);
                    return doubleCheckingValue < left || doubleCheckingValue > right;
                case ValidationOperatorId.Contains:
                case ValidationOperatorId.NotContains:
                case ValidationOperatorId.StartsWith:
                case ValidationOperatorId.NotStartingWith:
                case ValidationOperatorId.EndsWith:
                case ValidationOperatorId.NotEndingWith:
                case ValidationOperatorId.Regex:
                    break;
            }

            return false;
        }

        protected bool ValidateLength(int length, ValidationRule validationRule)
        {
            if (validationRule?.OperatorId == null)
                return true;

            ValidationOperatorId validationOperatorId =
                                    (ValidationOperatorId)validationRule.OperatorId.Value;

            double valueToCompare;
            double.TryParse(validationRule.Value, out valueToCompare);

            double left;
            double right;

            switch (validationOperatorId)
            {
                case ValidationOperatorId.GreaterThanOrEqualTo:
                    return length >= valueToCompare;
                case ValidationOperatorId.GreaterThan:
                    return length > valueToCompare;
                case ValidationOperatorId.LowerThanOrEqualTo:
                    return length <= valueToCompare;
                case ValidationOperatorId.LowerThan:
                    return length < valueToCompare;
                case ValidationOperatorId.Equals:
                    return length == valueToCompare;
                case ValidationOperatorId.NotEqual:
                    return length != valueToCompare;
                case ValidationOperatorId.Between:
                    var between = validationRule.Value.Split('|');
                    double.TryParse(between[0], out left);
                    double.TryParse(between[0], out right);
                    return length >= left && length <= right;
                case ValidationOperatorId.NotBetween:
                    between = validationRule.Value.Split('|');
                    double.TryParse(between[0], out left);
                    double.TryParse(between[0], out right);
                    return length < left || length > right;
                case ValidationOperatorId.Contains:
                    break;
                case ValidationOperatorId.NotContains:
                    break;
                case ValidationOperatorId.StartsWith:
                    break;
                case ValidationOperatorId.NotStartingWith:
                    break;
                case ValidationOperatorId.EndsWith:
                    break;
                case ValidationOperatorId.NotEndingWith:
                    break;
                case ValidationOperatorId.Regex:
                    break;
            }

            return false;
        }

        protected bool ValidateType(string value, ValidationRule validationRule)
        {
            if (validationRule?.OperatorId == null)
                return true;

            var shouldBeEqual = validationRule.OperatorId.Value == (int)ValidationOperatorId.Equals;

            DatatypeId dataType = (DatatypeId)validationRule.DataTypeId;

            bool result;
            switch (dataType)
            {
                case DatatypeId.Int:
                    long longValue;
                    result = long.TryParse(value, out longValue);
                    return result == shouldBeEqual;

                case DatatypeId.Datetime:
                    DateTime DateTimeValue;
                    result = value.Contains(":") && DateTime.TryParse(value, out DateTimeValue);
                    return result == shouldBeEqual;

                case DatatypeId.Decimal:
                    decimal decimalValue;
                    result = decimal.TryParse(value, out decimalValue);
                    return result == shouldBeEqual;

                case DatatypeId.Varchar:
                    return shouldBeEqual;

                case DatatypeId.Boolean:
                    bool booleanValue;
                    result = bool.TryParse(value, out booleanValue);
                    return result == shouldBeEqual;
            }

            return false;
        }

        protected DateTime GetBorderDateTime(ValidationRule rule)
        {
            var dateTime = DateTime.UtcNow;

            int seconds = 0;

            int.TryParse(rule.Value, out seconds);

            dateTime = dateTime.AddSeconds((-1) * seconds);

            return dateTime;
        }

        protected bool ValidateNumberOfChanges(string name, XObject node, bool isAttribute, ValidationRule validationRule)
        {
            var historiesNodeName = isAttribute ? XmlMerger.DvsHistoryAttributes : XmlMerger.DvsHistoryValues;

            var countOfChanges = 0;

            XElement withHistoryNode = node is XElement ? (XElement) node : node.Parent;

            var historyNode = withHistoryNode.XPathSelectElement(historiesNodeName);
            var oldNodes = historyNode?.XPathSelectElements(XmlMerger.DvsHistoryOldValue);

            if (oldNodes != null)
            {
                var borderDateTime = GetBorderDateTime(validationRule);

                foreach (var attrNode in oldNodes)
                {
                    if (attrNode.HasAttributes
                        && (attrNode.Attribute(name) != null || !isAttribute))
                    {
                        XAttribute attr = null;
                        if (attrNode.Attribute(XmlMerger.DvsLastupdateAttribbute) != null)
                        {
                            attr = attrNode.Attribute(XmlMerger.DvsLastupdateAttribbute);
                        } else if (attrNode.Attribute(XmlMerger.XmlLastupdateAttribbute_v1) != null)
                        {
                            attr = attrNode.Attribute(XmlMerger.XmlLastupdateAttribbute_v1);
                        }
                        else if (attrNode.Attribute(XmlMerger.XmlLastupdateAttribbute_v2) != null)
                        {
                            attr = attrNode.Attribute(XmlMerger.XmlLastupdateAttribbute_v2);
                        }
                        if (attr != null)
                        {
                            DateTime changeDateTime;

                            var isParsed = DateTime.TryParse(attr.Value, out changeDateTime);

                            if (isParsed && changeDateTime > borderDateTime)
                            {
                                countOfChanges++;
                            }// pass
                        }
                    }// pass
                }
            }

            return NumberOfChangesHelper(countOfChanges, validationRule.NumberOfChanges, validationRule);
        }

        protected bool ValidateNumberOfDistinctChanges(string name, XObject node, bool isAttribute, ValidationRule validationRule)
        {
            var historiesNodeName = isAttribute ? XmlMerger.DvsHistoryAttributes : XmlMerger.DvsHistoryValues;

            List<string> changes = new List<string>();

            XElement withHistoryNode = node is XElement ? node as XElement : node.Parent;

            var historyNode = withHistoryNode.XPathSelectElement(historiesNodeName);

            if (historyNode != null)
            {
                var oldNodes = historyNode.XPathSelectElements(XmlMerger.DvsHistoryOldValue);

                var borderDateTime = GetBorderDateTime(validationRule);

                foreach (var attrNode in oldNodes)
                {
                    if (attrNode.HasAttributes 
                        && (attrNode.Attribute(name) != null || !isAttribute))
                    {
                        XAttribute attr = null;
                        if (attrNode.Attribute(XmlMerger.DvsLastupdateAttribbute) != null)
                        {
                            attr = attrNode.Attribute(XmlMerger.DvsLastupdateAttribbute);
                        }
                        else if (attrNode.Attribute(XmlMerger.XmlLastupdateAttribbute_v1) != null)
                        {
                            attr = attrNode.Attribute(XmlMerger.XmlLastupdateAttribbute_v1);
                        }
                        else if (attrNode.Attribute(XmlMerger.XmlLastupdateAttribbute_v2) != null)
                        {
                            attr = attrNode.Attribute(XmlMerger.XmlLastupdateAttribbute_v2);
                        }
                        if (attr != null)
                        {
                            DateTime changeDateTime;

                            var isParsed = DateTime.TryParse(attr.Value, out changeDateTime);

                            if (isParsed && changeDateTime > borderDateTime)
                            {
                                var value = attr.Value;

                                if (changes.All(x => x != value))
                                {
                                    changes.Add(value);
                                }
                            }// pass
                        }
                    }
                }
            }

            return NumberOfChangesHelper(changes.Count, validationRule.NumberOfChanges, validationRule);
        }

        protected bool NumberOfChangesHelper(int validationValue, int? numberOfChanges, ValidationRule validationRule)
        {
            ValidationOperatorId validationOperatorId = (ValidationOperatorId)validationRule.OperatorId;

            double left, right;
            switch (validationOperatorId)
            {
                case ValidationOperatorId.GreaterThanOrEqualTo:
                    return validationValue >= numberOfChanges;
                case ValidationOperatorId.GreaterThan:
                    return validationValue > numberOfChanges;
                case ValidationOperatorId.LowerThanOrEqualTo:
                    return validationValue <= numberOfChanges;
                case ValidationOperatorId.LowerThan:
                    return validationValue < numberOfChanges;
                case ValidationOperatorId.Equals:
                    return validationValue == numberOfChanges;
                case ValidationOperatorId.NotEqual:
                    return validationValue != numberOfChanges;
                case ValidationOperatorId.Between:
                    var between = validationRule.Value.Split('|');
                    double.TryParse(between[0], out left);
                    double.TryParse(between[0], out right);
                    return validationValue >= left && validationValue <= right;
                case ValidationOperatorId.NotBetween:
                    between = validationRule.Value.Split('|');
                    double.TryParse(between[0], out left);
                    double.TryParse(between[0], out right);
                    return validationValue < left || validationValue > right;
                case ValidationOperatorId.Contains:
                case ValidationOperatorId.NotContains:
                case ValidationOperatorId.StartsWith:
                case ValidationOperatorId.NotStartingWith:
                case ValidationOperatorId.EndsWith:
                case ValidationOperatorId.NotEndingWith:
                case ValidationOperatorId.Regex:
                    break;
            }

            return false;
        }
    }

    class BooleanEvaluator
    {
        public bool Evaluate( StringBuilder expression)
        {
            //expression = expression.Replace("False", "0");
            //expression = expression.Replace("True", "1");
            expression = expression.Replace(" ", "");

            String temp;
            do
            {
                temp = expression.ToString();

                if (temp.Contains("(0)"))
                    expression = expression.Replace("(0)", "0");
                if (temp.Contains("(1)"))
                    expression = expression.Replace("(1)", "1");

                if (temp.Contains("AND"))
                {
                    if (temp.Contains("0AND"))
                    {
                        expression = expression.Replace("0AND0", "0");
                        expression = expression.Replace("0AND1", "0");
                    }
                    if (temp.Contains("1AND"))
                    {
                        expression = expression.Replace("1AND0", "0");
                        expression = expression.Replace("1AND1", "1");
                    }
                }

                if (temp.Contains("OR"))
                {
                    if (temp.Contains("0OR"))
                    {
                        expression = expression.Replace("0OR0", "0");
                        expression = expression.Replace("0OR1", "1");
                    }
                    if (temp.Contains("1OR"))
                    {
                        expression = expression.Replace("1OR0", "1");
                        expression = expression.Replace("1OR1", "1");
                    }
                }
            }
            while (temp != expression.ToString());

            if (temp.Length==1 && temp == "1")
                return true;

            return false;
            throw new ArgumentException("expression");
        }
    }
}