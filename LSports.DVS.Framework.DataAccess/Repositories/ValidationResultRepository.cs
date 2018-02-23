using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using LSports.DVS.Framework.DataAccess.Models;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using LSports.DVS.Framework.DataAccess.CustomClasses;
using LSports.Framework.Models.Extensions;
using Microsoft.Ajax.Utilities;

namespace LSports.DVS.Framework.DataAccess.Repositories
{
    public class ValidationResultRepository : IValidationResultRepository
    {
        private const int ConnectionTimeout = 300;
        private const int InsertCount = 100;

        protected Object thisLock = new Object();
        // protected Object thisPushLock = new Object();

        public List<ValidationResult> GetLastValidationResults(string validationHashIndex, IList<ValidationSetting> settings )
        {
            using (var entities = new gb_dvsstagingEntities())
            {
                entities.Configuration.AutoDetectChangesEnabled = false;
                entities.Configuration.ValidateOnSaveEnabled = false;
                entities.Database.CommandTimeout = ConnectionTimeout;

                var settingsIds = settings.Select(x => x.Id).ToArray();
                var newResults =
                    entities.val_validationresult
                    .Where(x => x.HashValidationIndex == validationHashIndex)
                    .Where(x => x.IsActive)
                    .Where(x=> settingsIds.Contains(x.ValidationSettingId))
                        .Select(x => new ValidationResult()
                        {
                            Id = x.Id,
                            EventId = x.EventId,
                            Market = x.Market,
                            Provider = x.Provider,
                            ProductId = x.ProductId,
                            ValidationSettingId = x.ValidationSettingId
                        });

               return newResults.ToList();
            }
        }

        public void SetDisabledBySetting(int settingId)
        {
            using (var entities = new gb_dvsstagingEntities())
            {
                var query = entities.val_validationresult
                    .Where(x => x.ValidationSettingId == settingId);

                foreach (var valValidationresult in query)
                {
                    valValidationresult.IsActive = false;
                }

                entities.SaveChanges();
            }
        }

        /*
        public void InsertValidationResults(List<val_validationresult> validationResults)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                model.Configuration.AutoDetectChangesEnabled = false;
                model.Configuration.ValidateOnSaveEnabled = false;
                model.Database.CommandTimeout = ConnectionTimeout;
                
                /*
                var savingEventIds = validationResults.Select(x => x.EventId).Distinct().ToArray();
                var validationSettingIds = validationResults.Select(x => x.ValidationSettingId).Distinct().ToArray();
                var markets = validationResults.Select(x => x.Market).Distinct().ToArray();
                var providers = validationResults.Select(x => x.Provider).Distinct().ToArray();
                

                var dbEventIds = model.val_validationresult.AsNoTracking().Where(x => x.IsActive
                                                                                    && validationResults.Any(y=> y.EventId == x.EventId
                                                                                        && y.ValidationSettingId == x.ValidationSettingId
                                                                                        && y.Market == x.Market
                                                                                        && y.Provider == x.Provider))
                                                                        .Select(x => x.EventId)
                                                                        .ToArray();

                var insertingValidationResults = validationResults
                    .Where(x => !dbEventIds.Contains(x.EventId));

                model.val_validationresult.AddRange(insertingValidationResults);

                model.SaveChanges();
            }
        }
        */

        public List<int> DisableForSettingEvent(int productId, int settingId, int eventId)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                var results = model.val_validationresult
                    .Where(x => x.ProductId == productId)
                    .Where(x => x.ValidationSettingId == settingId && x.EventId == eventId);

                foreach (var valValidationresult in results)
                {
                    valValidationresult.IsActive = false;
                }

                model.SaveChanges();

                var ids = results.Select(x => x.Id).ToList();

                return ids;
            }
        }

        public ValidationResult NewValidationResultFromRepo(val_validationresult result)
        {
            return new ValidationResult
            {
                Id = result.Id,
                ProductId = result.ProductId,
                EventId = result.EventId,
                EventName = result.EventName,
                ValidationSettingId = result.ValidationSettingId,
                LeagueName = result.LeagueName,
                LeagueId = result.LeagueId,
                LocationName = result.LocationName,
                LocationId = result.LocationId,
                Market = result.Market,
                Provider = result.Provider,
                SportName = result.SportName,
                SportId = result.SportId,
                Status = result.Status,
                LastUpdate = result.LastUpdate,
                UpdatedOn = result.UpdatedOn,
                XmlMessage = "",
                PointsToHighline = result.PointsToHighline,
                Counter = result.Counter,
                IsActive = result.IsActive,
                CreatedOn = result.CreatedOn
            };
        }


        public IList<ValidationResult> GetValidationResultsByBorderDate(DateTime date)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                var results = model.val_validationresult.AsNoTracking()
                    .Where(x => x.CreatedOn > date)
                    .Select(NewValidationResultFromRepo)
                    .ToList();

                return results;
            }
        }

        public ValidationResult Get(int resultId)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                var result = model.val_validationresult.AsNoTracking().FirstOrDefault(x=>x.Id == resultId);

                return NewValidationResultFromRepo(result);
            }
        }

        public void DisableOldValidationResults(IList<Event> events )
        {
            // var updatedOn = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff");

            //var locker = productId == 1 ? this.thisPullLock : thisPushLock;


            // lock (this.thisLock)
            {
                using (var dbConnection = new DbConnection.DbConnection())
                {
                    // var eventsHashes = events.OrderBy(x=>x.EventId).Select(x => x.EventId.ToString() + "-" + productId.ToString()).ToList();

                    var queries = new List<string>();

                    foreach (var @event in events)
                    {
                        queries.Add("UPDATE val_validationresult SET IsActive = b'0'" +
                                //", LastUpdate = '" + updatedOn + "'" +
                                //", UpdatedOn = '" + updatedOn + "'" +
                                ", HashIndexByEventProduct = '" + ValidationResultsHashModel.GetEventProductHash(@event.EventId, @event.ProductId, false) + "'" +
                                " WHERE HashIndexByEventProduct = '" + ValidationResultsHashModel.GetEventProductHash(@event.EventId, @event.ProductId, true) + "'");
                    }

                    /*
                    var query = "UPDATE IGNORE val_validationresult SET IsActive = b'0'" +
                                ", LastUpdate = '" + updatedOn + "'" +
                                ", UpdatedOn = '" + updatedOn + "'" + 
                                " WHERE HashIndexByEventProduct IN (" + string.Join(",", eventsHashes) + ")"
                                + " AND  IsActive=b'1';";

                    * */
                    dbConnection.ExecuteList(queries);
                }
            }
        }

        protected void BulkInsertToTable(List<ValidationResultsHashModel> validationResults, string tableName, string hashValidationIndex)
        {
            using (var dbConnection = new DbConnection.DbConnection())
            {
                var format = "yyyy-MM-ddTHH:mm:ss.fff";
                var updatedOn = DateTime.UtcNow.ToString(format);

                /*
                var queriesForDisable = new List<string>();

                foreach (var validationResult in validationResults)
                {
                    if (validationResult.EventId <= 0 || validationResult.ValidationSettingId <= 0)
                        continue;

                    var hash = validationResult.EventId.ToString() + " " + validationResult.ValidationSettingId.ToString();

                    queriesForDisable.Add(new StringBuilder("UPDATE IGNORE val_validationresult SET IsActive = b'0', LastUpdate = '" + updatedOn + "', UpdatedOn = '" + updatedOn +
                                          "' WHERE HashByEventSetting = '" + hash + "'"
                                          + " AND  IsActive=b'1'").ToString());
                }

                dbConnection.BulkExecuteList(queriesForDisable);
                */
                
                for (int i = 0; i <= validationResults.Count() / InsertCount; i++)
                {
                    var subResults = validationResults.Skip(i * InsertCount).Take(InsertCount).ToList();

                    if (subResults.Any())
                    {

                        var query =
                            new StringBuilder(
                                "INSERT DELAYED INTO " + tableName + " (ValidationSettingId, EventId, EventName, SportName, LeagueName, " +
                                "LocationName, Market, Provider, Status, LastUpdate, ProductId, CreatedOn, CreatedBy, IsActive, " +
                                "UpdatedOn, UpdatedBy, LeagueId, SportId, LocationId, PointsToHighline, HashIndexByEventProduct, HashUniqueIndex, " +
                                "Counter, HashValidationIndex) VALUES ");

                        var counter = 0;

                        for (var resI = 0; resI < subResults.Count; resI++)
                        {
                            var model = subResults[resI];
                            var validationResult = model.Result;

                            query.Append("(");

                            if (validationResult.PointsToHighline.Contains("'"))
                            {
                                validationResult.PointsToHighline = validationResult.PointsToHighline.Replace("'", "\\'");
                            }

                            query.Append(validationResult.ValidationSettingId);
                            query.Append("," + validationResult.EventId);
                            query.Append(",'" + validationResult.EventName + "'");
                            query.Append(",'" + validationResult.SportName + "'");
                            query.Append(",'" + validationResult.LeagueName + "'");
                            query.Append(",'" + validationResult.LocationName + "'");
                            query.Append(",'" + validationResult.Market + "'");
                            query.Append(",'" + validationResult.Provider + "'");
                            query.Append(",'" + validationResult.Status + "'");
                            query.Append(",'" + validationResult.LastUpdate.ToString(format) + "'");
                            // query.Append(",'" + validationResult.XmlMessage + "'");
                            query.Append(",'" + validationResult.ProductId + "'");
                            query.Append(",'" + validationResult.CreatedOn.ToString(format) + "'");
                            query.Append(",'" + validationResult.CreatedBy + "'");
                            query.Append(", b'1'");
                            query.Append(",'" + updatedOn + "'");
                            query.Append(",'" + validationResult.UpdatedBy + "'");
                            query.Append(",'" + validationResult.LeagueId + "'");
                            query.Append(",'" + validationResult.SportId + "'");
                            query.Append(",'" + validationResult.LocationId + "'");
                            query.Append(",'" + validationResult.PointsToHighline + "'");
                            query.Append(",'" + model.IndexEventProductHash + "'");
                            query.Append(",'" + model.UniqueHash + "'");
                            query.Append("," + 1);
                            query.Append(",'" + hashValidationIndex + "'");
                            query.Append(")");

                            if (counter != subResults.Count - 1 )
                            {
                                query.Append(",");
                            }

                            subResults[resI] = null;

                            counter++;
                        }

                        query.Append(" ON DUPLICATE KEY UPDATE");
                        query.Append(" Status = VALUES(Status)");
                        query.Append(", LastUpdate = VALUES(LastUpdate)");
                        query.Append(", UpdatedOn = VALUES(UpdatedOn)");
                        query.Append(", Market = VALUES(Market)");
                        query.Append(", Provider = VALUES(Provider)");
                        query.Append(", IsActive = b'1'");
                        //                    query.Append(", UpdatedOn = VALUES(UpdatedOn)");
                        query.Append(", HashIndexByEventProduct = VALUES(HashIndexByEventProduct)");
                        query.Append(", PointsToHighline = VALUES(PointsToHighline)");
                        query.Append(", Counter = Counter + 1");
                        query.Append(";");

                        dbConnection.BulkExecute(query.ToString());
                    }
                }
            }
        }

        public void BulkInsertToPull(List<ValidationResultsHashModel> validationResults, string hashValidationIndex)
        {
            // lock (this.thisLock)
            {
                // DisablePreviousErrors(validationResults);
                // InsertValidationResults(validationResults);

                //SHA256Managed hashstring = new SHA256Managed();

                BulkInsertToTable(validationResults, "val_validationresult", hashValidationIndex);
            }
        }

        public void BulkInsertToPush(List<ValidationResultsHashModel> validationResults, string hashValidationIndex)
        {
            // lock (this.thisLock)
            {
                // DisablePreviousErrors(validationResults);
                // InsertValidationResults(validationResults);

                BulkInsertToTable(validationResults, "val_validationresult", hashValidationIndex);
                
                //BulkInsertToTable(validationResults, "val_validationresult_push");
            }
        }

        /*
        public void DisablePreviousErrors(List<val_validationresult> validationResults)
        {
            using (var dbConnection = new DbConnection.DbConnection())
            {
                var updatedOn = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff");
                var queriesForDisable = new List<string>();

                foreach (var validationresult in validationResults)
                {
                    if (validationresult.EventId <= 0 || validationresult.ValidationSettingId <= 0)
                        continue;


                    queriesForDisable.Add(new StringBuilder("UPDATE val_validationresult SET IsActive = b'0', LastUpdate = '" + updatedOn + "', UpdatedOn = '" + updatedOn +
                                          "' WHERE EventId = " + validationresult.EventId 
                                          + " AND ValidationSettingId = " + validationresult.ValidationSettingId 
                                          + " AND  IsActive=b'1'").ToString());
                }

                dbConnection.BulkExecuteList(queriesForDisable);

                /*
                var queriesForEnable = new List<string>();
                foreach (var validationresult in validationResults)
                {

                    if (validationresult.EventId <= 0 )
                        continue;

                    queriesForEnable.Add(new StringBuilder("UPDATE val_validationresult "
                                            +"SET IsActive = b'1', LastUpdate = '" + updatedOn 
                                            + "', UpdatedOn = '" + updatedOn 
                                            + "', PointsToHighline = '" + validationresult.PointsToHighline
                                            + "' WHERE EventId = " + validationresult.EventId
                                          + " AND ValidationSettingId = " + validationresult.ValidationSettingId
                                          + " AND Market = '" + validationresult.Market+"'"
                                          + " AND Provider = '" + validationresult.Provider + "'"
                                          + "AND IsActive = b'0'"
                                          ).ToString());
                }

                dbConnection.BulkExecuteList(queriesForEnable);
            }
        }
        */

        private static  val_validationresult NewRepositoryValidationResultsFrom(ValidationResult result)
        {
            return new val_validationresult
            {
                Id = result.Id,
                EventId = result.EventId,
                Market = result.Market,
                Status = result.Status,
                ProductId = result.ProductId,
                Provider = result.Provider,
                LeagueName = result.LeagueName,
                LeagueId = result.LeagueId,
                SportId = result.SportId,
                LocationId = result.LocationId,
                EventName = result.EventName,
                LastUpdate = result.LastUpdate,
                LocationName = result.LocationName,
                SportName = result.SportName,
                ValidationSettingId = result.ValidationSettingId,
                // XmlMessage = result.XmlMessage,
                CreatedBy = result.CreatedBy,
                UpdatedBy = result.UpdatedBy,
                CreatedOn = result.CreatedOn,
                UpdatedOn = result.UpdatedOn,
                IsActive = true
            };
        }
        
        public ValidationResultsHashModel NewRepositoryValidationResultFrom(ArrayList results, ValidationSetting setting, Event @event, string market, string provider)
        {
            var ruleResults = results.Cast<RuleResultDTO>().ToArray();

            ruleResults = ruleResults.DistinctBy(x => x.PointToHighline).ToArray();

            var pointToHighline = "";

            if (setting.IsContainsRuleForAllNodes)
            {
                pointToHighline = string.Join("|",
                    ruleResults.Where(x => x.IsIssue && x.Rule.IsForAllNodes == false).Select(x => x.PointToHighline.ToString()).Distinct());

                var groupsByNodeName = ruleResults.Where(x=>x.IsIssue && x.Rule.IsForAllNodes).GroupBy(x => x.ParentNodeName);

                foreach (var groupByNodeName in groupsByNodeName)
                {
                    var groupsByRuleId = groupByNodeName.GroupBy(x => x.Rule.Id).ToArray();

                    var maxCount = groupsByRuleId.Max(x => x.Count());

                    foreach (var grouping in groupsByRuleId)
                    {
                        if (grouping.Count() == maxCount)
                        {
                            foreach (var ruleResultDto in grouping)
                            {
                                pointToHighline += "|" + ruleResultDto.PointToHighline.ToString();
                            }
                        }
                    }
                }
            }
            else
            {
                pointToHighline = string.Join("|",
                    ruleResults.Where(x => x.IsIssue).Select(x => x.PointToHighline.ToString()).Distinct());
            }

            string markets = "";
            string providers = "";

            bool isContainsSingleProvider = true;
            bool isContainsSingleMarket = true;

            var resultWithMarket = ruleResults.FirstOrDefault(x => x.IsMarket || x.Market != "");

            if (resultWithMarket != null)
            {
                var resultWithProvider = ruleResults.FirstOrDefault(x => x.IsProvider || x.Provider != "");
                if (resultWithProvider != null)
                {
                    markets = market;
                    providers = provider;
                }
                else
                {
                    isContainsSingleProvider = false;

                    markets = market;

                    // select all providers
                    var bookmakers = resultWithMarket.ParentXElement.XPathSelectElements(BookmakerName);
                    var withAttributes = bookmakers.Where(x => x.HasAttributes);

                    List<string> names = new List<string>();

                    foreach (var withAttribute in withAttributes)
                    {
                        var nameAttr = withAttribute.Attribute("name");
                        if (nameAttr != null)
                        {
                            names.Add(nameAttr.Value);
                        }
                    }

                    if (names.Count > 0)
                    {
                        providers = string.Join(", ", names);
                    }
                }
            }
            else
            {
                isContainsSingleMarket = false;

                /*
                // select all markets and providers
                if (@event.MarketNames != null)
                {
                    markets = string.Join(",", @event.MarketNames.Distinct());
                }
                if (@event.ProviderNames != null)
                {
                    providers = string.Join(",", @event.ProviderNames.Distinct());
                }
                */
            }

            var result = new val_validationresult
            {
                EventId = (long) @event.EventId,
                Status = @event.Status,
                LeagueName = @event.LeagueName,
                EventName = @event.BuildEventName(),
                LastUpdate = @event.LastUpdate,
                LocationName = @event.LocationName,
                SportName = @event.SportName,
                CreatedBy = "Admin",
                UpdatedBy = "Admin",
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                ProductId = (int) setting.ProductId,
                Market = markets,
                Provider = providers,
                ValidationSettingId = setting.Id,
                PointsToHighline = pointToHighline,
                IsActive = true
            };
            
            // XmlMessage = string.Join("|", resultsWithoutMarketAndProviderDTO.Select(x => x.OuterXml)),

            if (@event.LeagueId != null)
                result.LeagueId = (long)@event.LeagueId;
            if (@event.SportId != null)
                result.SportId = (long)@event.SportId;
            if (@event.LocationId != null)
                result.LocationId = (long)@event.LocationId;

            return new ValidationResultsHashModel
            {
                ContainsSingleMarket = isContainsSingleMarket,
                ContainsSingleProvider = isContainsSingleProvider,
                Result = result
            };
        }
        
        public val_validationresult NewRepositoryValidationResultFrom(RuleResultDTO ruleResult, ValidationSetting setting)
        {
            return new val_validationresult
            {
                EventId = (long)ruleResult.Event.EventId,
                Market = ruleResult.Market,
                Provider = ruleResult.Provider,
                Status = ruleResult.Event.Status,
                ProductId = (int)setting.ProductId,
                LeagueName = ruleResult.Event.LeagueName,
                LeagueId = (long)ruleResult.Event.LeagueId,
                SportId = (long)ruleResult.Event.SportId,
                LocationId = (long)ruleResult.Event.LocationId,
                EventName = ruleResult.Event.BuildEventName(),
                LastUpdate = ruleResult.Event.LastUpdate,
                LocationName = ruleResult.Event.LocationName,
                SportName = ruleResult.Event.SportName,
                ValidationSettingId = setting.Id,
                //XmlMessage = ruleResult.OuterXml,
                CreatedBy = "Admin",
                UpdatedBy = "Admin",
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                PointsToHighline = ruleResult.PointToHighline.ToString(),
                IsActive = true
            };
        }

        public static string OutcomeName = "Outcome";
        public static string BookmakerName = "Bookmaker";
        public static string OddsName = "Odds";

        public void NewRuleResultFrom(Event @event, XObject node, XElement validatedNode,
            ValidationRule rule, bool isIssue, 
            PointToHighline pointToHighline, string parentNodeName,
            Dictionary<int, Dictionary<string, int>> currentNodesFoundedCount,
            ref List<RuleResultDTO> ruleResults)
        {
            bool isMarket = false;
            bool isProvider = false;

            string providerName = "";
            string marketName = "";

            var isAttr = node is XAttribute;

            XElement ownerElement = null;


            if (isAttr || (node is XElement && (node as XElement).Name == "Odds"))
            {
                if (isAttr)
                {
                    ownerElement = (node as XAttribute).Parent;
                }
                else
                {
                    ownerElement = node as XElement;
                }

                if (ownerElement.Name == OddsName || ownerElement.Name == BookmakerName || ownerElement.Name == OutcomeName)
                {
                    if (ownerElement.Name == OddsName)
                    {
                        var parentNode = ownerElement.Parent;

                        if ( parentNode != null && parentNode.Name == BookmakerName)
                        {
                            var providerAttrName = parentNode.Attribute("name");
                            if (providerAttrName != null)
                            {
                                providerName = providerAttrName.Value;
                            } // else pass

                            parentNode = parentNode.Parent;
                            if (parentNode != null && parentNode.Name == OutcomeName)
                            {
                                var marketNameAttr = parentNode.Attribute("name");
                                if (marketNameAttr != null)
                                {
                                    marketName = marketNameAttr.Value;
                                } // else pass
                            }// else pass
                        } else if (parentNode != null && parentNode.Name == OutcomeName)
                        {
                            var marketNameAttr = parentNode.Attribute("name");
                            if (marketNameAttr != null)
                            {
                                marketName = marketNameAttr.Value;
                            } // else pass
                        } // else pass
                    } else if (ownerElement.Name == BookmakerName)
                    {
                        isProvider = true;

                        var providerAttrName = ownerElement.Attribute("name");
                        if (providerAttrName != null)
                        {
                            providerName = providerAttrName.Value;
                        } // else pass

                        var parentNode = ownerElement.Parent;
                        if (parentNode != null && parentNode.Name == OutcomeName)
                        {
                            var marketNameAttr = parentNode.Attribute("name");
                            if (marketNameAttr != null)
                            {
                                marketName = marketNameAttr.Value;
                            } // else pass
                        }// else pass
                    } else if (ownerElement.Name == OutcomeName)
                    {
                        isMarket = true;

                        var marketNameAttr = ownerElement.Attribute("name");
                        if (marketNameAttr != null)
                        {
                            marketName = marketNameAttr.Value;
                        } // else pass
                    }// pass

                }// pass
            }// pass
            
            var keyValue = marketName + "-" + providerName;

            if (currentNodesFoundedCount[rule.Id].ContainsKey(keyValue))
            {
                currentNodesFoundedCount[rule.Id][keyValue] += 1;
            }
            else
            {
                currentNodesFoundedCount[rule.Id].Add(keyValue, 1);
            }

            if (isIssue)
            {
                ruleResults.Add(new RuleResultDTO
                {
                    Provider = providerName,
                    Market = marketName,
                    IsIssue = isIssue,
                    //OuterXml = xml,
                    PointToHighline = pointToHighline,
                    Rule = rule,
                    Event = @event,
                    ParentNodeName = parentNodeName,
                    IsMarket = isMarket,
                    IsProvider = isProvider,
                    ParentXElement = ownerElement,
                    ParentValidatedXElement = validatedNode
                });
            }
        }


        public int GetNumberOfResultsByProductId(int productId)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                //if(productId == 1)
                    return model.val_validationresult.Count(res => res.ProductId == productId && res.IsActive);
                //else
                //    return model.val_validationresult_push.Count(res => res.ProductId == productId && res.IsActive);
            }
        }

        public Dictionary<string, int> GetHistoricalDataByProductIdAndTimeFrame(int productId, DateTime from, DateTime to)
        {
            from = from.Date + new TimeSpan(0, 0, 0);
            to = to.Date + new TimeSpan(23, 59, 59);

            using (var model = new gb_dvsstagingEntities())
            {
                model.Configuration.AutoDetectChangesEnabled = false;
                model.Configuration.ValidateOnSaveEnabled = false;
                model.Database.CommandTimeout = ConnectionTimeout;

                List<DateTime> creations = null;

                creations = model.val_validationresult
                    .AsNoTracking()
                    .Where(res => res.IsActive && res.ProductId == productId && res.UpdatedOn >= from && res.UpdatedOn <= to)
                    .Select(x => x.UpdatedOn)
                    .ToList();

                var result = new Dictionary<string, int>();

                for (; to.CompareTo(from) >= 0; from = from.AddDays(1.0))
                {
                    result.Add(
                        from.ToString("yyyy-MM-dd"),
                        creations
                            .Count(creation => creation.Date.Equals(from.Date)));
                }

                return result;
            }
        }

        public int GetNumberOfResultsBySettingId(int productId, int settingId)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                //if (productId == 1)
                //{
                    return model.val_validationresult.Count(res => res.IsActive && res.ValidationSettingId == settingId);
                //}
                //else
                //{
                //    return model.val_validationresult_push.Count(res => res.IsActive && res.ValidationSettingId == settingId);
                //}
            }
        }

                

        public IList<ValidationResult> GetResultsBySettingId(int settingId, int productId, int offset, int count, FilterObject filter, bool loadInactive)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                model.Database.CommandTimeout = 300;

                if (filter.Event == null)
                    filter.Event = new FilterValue();
                if (filter.Country == null)
                    filter.Country = new FilterValue();
                if (filter.EventStatus == null)
                    filter.EventStatus = new FilterValue();
                if (filter.League == null)
                    filter.League = new FilterValue();
                if (filter.Market == null)
                    filter.Market = new FilterValue();
                if (filter.Provider == null)
                    filter.Provider = new FilterValue();
                if (filter.Sport == null)
                    filter.Sport = new FilterValue();
                
                return model.val_validationresult.AsNoTracking()
                    .Where(result => result.ValidationSettingId == settingId && result.ProductId == productId)
                    .Where(result => (!filter.Event.ExternalId.HasValue) || (result.EventId == filter.Event.ExternalId))
                    .Where(result => (!filter.Country.ExternalId.HasValue) || (result.LocationId == filter.Country.ExternalId))
                    .Where(result => string.IsNullOrEmpty(filter.EventStatus.Name) || (result.Status == filter.EventStatus.Name))
                    .Where(result => (!filter.League.ExternalId.HasValue) || (result.LeagueId == filter.League.ExternalId))
                    .Where(result => string.IsNullOrEmpty(filter.Market.Name) || (string.IsNullOrEmpty(result.Market) || result.Market.Contains(filter.Market.Name)))
                    .Where(result => string.IsNullOrEmpty(filter.Provider.Name) || (string.IsNullOrEmpty(result.Provider) || result.Provider.Contains(filter.Provider.Name)))
                    .Where(result => (!filter.Sport.ExternalId.HasValue) || (result.SportId == filter.Sport.ExternalId))
                    .Where(result => (filter.StartDate == null) || (result.UpdatedOn >= filter.StartDate))
                    .Where(result => (filter.EndDate == null) || (result.UpdatedOn <= filter.EndDate))
                    .Where(result => result.IsActive == !loadInactive)
                    /*.Join(model.val_validationsetting, valResult => valResult.ValidationSettingId, valRule => valRule.Id, (a, b) => new {
                        ValidationResult = a,
                        ValidationSetting = b
                    })*/
                    .OrderByDescending(x => x.UpdatedOn)
                    .Select(a => new ValidationResult
                    {
                        Id = a.Id,
                        ProductId = a.ProductId,
                        EventId = a.EventId,
                        EventName = a.EventName,
                        ValidationSettingId = a.ValidationSettingId,
                        LeagueName = a.LeagueName,
                        LeagueId = a.LeagueId,
                        LocationName = a.LocationName,
                        LocationId = a.LocationId,
                        Market = a.Market,
                        Provider = a.Provider,
                        SportName = a.SportName,
                        SportId = a.SportId,
                        Status = a.Status,
                        LastUpdate = a.LastUpdate,
                        UpdatedOn = a.UpdatedOn,
                        XmlMessage = "",
                        PointsToHighline = a.PointsToHighline,
                        IsActive = a.IsActive,
                        /*ValidationSetting = new ValidationSetting()
                        {
                            Id = a.ValidationSetting.Id,

                            Name = a.ValidationSetting.Name,
                            ProductId = a.ValidationSetting.ProductId,
                            PriorityId = a.ValidationSetting.PriorityId,
                            Description = a.ValidationSetting.Description,
                            Expression = a.ValidationSetting.Expression
                        },*/
                        Counter = a.Counter
                    })
                    .Skip(offset)
                    .Take(count)
                    .ToList();
            }
        }

        public ValidationResultsStatistic GetFilteredResultsByProductId(int? productId, FilterObject filter, int settingId)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                model.Database.CommandTimeout = 300;

                var q = model.val_validationresult.AsNoTracking()
                    .Where(result=> result.ValidationSettingId == settingId)
                    .Where(result =>
                    (!productId.HasValue || result.ProductId == productId)
                    && (!filter.Event.ExternalId.HasValue || result.EventId == filter.Event.ExternalId)
                    && (!filter.Country.ExternalId.HasValue || result.LocationId == filter.Country.ExternalId)
                    && (string.IsNullOrEmpty(filter.EventStatus.Name) || result.Status == filter.EventStatus.Name)
                    && (!filter.League.ExternalId.HasValue || result.LeagueId == filter.League.ExternalId)
                    &&
                    (string.IsNullOrEmpty(filter.Market.Name) || string.IsNullOrEmpty(result.Market) ||
                     result.Market.Contains(filter.Market.Name))
                    &&
                    (string.IsNullOrEmpty(filter.Provider.Name) || string.IsNullOrEmpty(result.Provider) ||
                     result.Provider.Contains(filter.Provider.Name))
                    && (!filter.Sport.ExternalId.HasValue || result.SportId == filter.Sport.ExternalId)
                    && (filter.StartDate == null || result.UpdatedOn >= filter.StartDate)
                    && (filter.EndDate == null || result.UpdatedOn <= filter.EndDate))
                    .Select(x=>new SubStatistic()
                    {
                        IsActive = x.IsActive,
                        LastTimeShown = x.UpdatedOn
                    })
                    .ToList();

                var statistic = new ValidationResultsStatistic();

                statistic.TotalIssues = q.Count;
                statistic.OpenIssues = q.Count(x => x.IsActive);

                if (statistic.OpenIssues > 0)
                {
                    statistic.LastTimeShown = q.Max(x => x.LastTimeShown);
                }

                return statistic;
            }
        }

        /*
        public IList<ValidationResult> GetFilteredResultsBySettingId(int settingId, FilterObject filter)
        {
            return GetFilteredResults(filter)
                .Where(x => x.ValidationSettingId == settingId)
                .ToList();
        }
        */
        

        delegate List<FilterValue> GetFilterValues();
        delegate List<dvs_selectiontreeitem> GetSelectionTreeItems();

        public DropDownValues GetDropDownValues(int? productId, int filterType, FilterValue selectedFilterValue)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                GetSelectionTreeItems getSelectionTreeItems = delegate()
                {
                    var filtered =
                        model.dvs_selectiontreeitem.Where(
                            x => x.IsActive
                                 && x.TypeId == filterType
                                 && x.ParentId == selectedFilterValue.ExternalId);

                    if (productId.HasValue)
                    {
                        filtered = filtered.Where(x => x.ProductId == productId);
                    }

                    return filtered.ToList();
                };

                GetFilterValues getFilterValues = delegate()
                {
                    var filtered = getSelectionTreeItems();

                    var items = new List<FilterValue>();
                    foreach (var filter in filtered)
                    {
                        items.AddRange(filter.Name.Split(',').Select(m => new FilterValue {Name = m}).ToList());
                    }
                    
                    return items.OrderBy(x=>x.Name).ToList();
                };

                var q = model.dvs_event.Where(x => x.IsActive);
                if (productId.HasValue)
                {
                    q = q.Where(x => x.ProductId == productId);
                }

                switch (filterType)
                {
                    case 1:
                        var sports = q
                                .Select(x => new FilterValue
                                {
                                    ExternalId = x.SportId,
                                    Name = x.SportName
                                })
                                .Distinct()
                                .OrderBy(x=>x.Name)
                                .ToList();
                        return new DropDownValues
                        {
                            Sports = sports
                        };
                    case 2:
                        var locations = q.Where(x => selectedFilterValue.ExternalId == x.SportId)
                                .Select(x => new FilterValue
                                {
                                    ExternalId = x.LocationId,
                                    Name = x.LocationName
                                })
                                .Distinct()
                                .OrderBy(x => x.Name)
                                .ToList();
                        return new DropDownValues
                         {
                             Countries = locations
                        };
                    case 3:
                        var leagues = q.Where(x => selectedFilterValue.ExternalId == x.LocationId)
                                .Select(x => new FilterValue
                                {
                                    ExternalId = x.LeagueId,
                                    Name = x.LeagueName
                                })
                                .Distinct()
                                .OrderBy(x => x.Name)
                                .ToList();
                        return new DropDownValues
                        {
                            Leagues = leagues
                        };
                    case 4:
                        var eventNames = q.Where(x =>selectedFilterValue.ExternalId == x.LeagueId)
                                .Select(x => new FilterValue
                                {
                                    Name = x.EventName,
                                    ExternalId = x.EventId,
                                    Id = x.EventId
                                })
                                .OrderBy(x => x.Name)
                                .ToList();
                        return new DropDownValues
                        {
                            Events = eventNames
                        };
                    case 5:
                        return new DropDownValues
                        {
                            Markets = getFilterValues()
                        };
                    case 6:
                        return new DropDownValues
                        {
                            Providers = getFilterValues()
                        };
                    case 7:
                        var filtered = getSelectionTreeItems();
                        
                        return new DropDownValues
                        {
                            Statuses = filtered.Select(x => new FilterValue { Name = x.Name}).ToList()
                        };
                    default:
                        return new DropDownValues
                        {
                        };
                }
            }
        }
    }
}