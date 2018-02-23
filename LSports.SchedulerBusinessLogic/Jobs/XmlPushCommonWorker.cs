
namespace LSports.Scheduler.Jobs
{
    using log4net;
    using LSports.DVS.Framework.DataAccess.Repositories;
    using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
    using LSports.Framework.Models.CustomClasses;
    using LSports.Framework.Models.Enums;
    using LSports.Scheduler.Services;
    using Quartz;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using LSports.Scheduler.Jobs.@base;
    using LSports.Services;
    using LSports.Services.Interfaces;
    using Microsoft.Ajax.Utilities;
    using System.Collections.Concurrent;

	/// <summary>
	/// Main service for processing PUSH-messages
	/// </summary>
    [DisallowConcurrentExecution]
    public class XmlPushCommonWorker : XmlCommonWorker
    {
        protected XmlDownloadService _downloadService = new XmlDownloadService();
        protected IArrivalMessageService _arrivalMessageService = new ArrivalMessageService();
        protected IArrivalMessageRepository _arrivalMessageRepository = new ArrivalMessageRepository();

        protected readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Max count of events, which we can take from queue for one processing(memory limit)
		/// </summary>
        public static int MAX_DEQUE_COUNT = 6000;

		/// <summary>
		/// Initialization events store with lockers
		/// </summary>
        public void InitDictionary()
        {
            sportsEventsStores = new ConcurrentDictionary<string, EventsStoreAndLocker>();

			// used ConcurrentDictionary !
            foreach (var checkingProductId in checkingProductIds)
            {
                sportsEventsStores.AddOrUpdate(checkingProductId.ToString(), new EventsStoreAndLocker(),(key, oldValue)=> new EventsStoreAndLocker());
            }
        }

        public override void Run()
        {
            try
            {
				// We do't all arrival messages for product #5, because it has thousands of identical messages. 
				// We insert only one common message
                var SpecialProductId = 5;// special conditions for product 5

				// if 'true' - we check all push-products
                checkingProductIds = productRepository.GetListByType(true).Select(x => x.Id).ToList();

                InitDictionary();

                var validationRules = GetValidationRules();
                var validationSettings = GetValidationSettingsForProducts(checkingProductIds);

				// mapping val.rules to val.settings(optimization for validation)
				SetRulesForSettings(validationSettings, validationRules);

                var productsTasks = new List<Task>();

				// one product - one parallel task
                foreach (var checkingProductId in checkingProductIds)
                {
                    var queue = ProductsEventsConcurencyQueue.GetInstance().Storage.store[checkingProductId];
                    var productSettings = validationSettings.Where(x => x.ProductId == checkingProductId).ToList();

                    var count = Math.Min(queue.Count, MAX_DEQUE_COUNT);

                    if (count > 0)
                    {
                        var task = Task.Factory.StartNew(() =>
                        {
                            var messages = new List<string>();

							// extraction from queue
                            for (var i = 0; i < count; i++)
                            {
                                var message = "";

                                if (queue.TryDequeue(out message))
                                {
                                    messages.Add(message);
                                } // pass
                            }

							// processing all received messages from queue(by limit for each product)
                            ProcessArrivalMessagesForProduct(checkingProductId, messages, productSettings, count,
                                checkingProductId != SpecialProductId);
                        });

                        productsTasks.Add(task);
                    }
                }

                if (productsTasks.Count > 0)
                {
                    Task.WaitAll(productsTasks.ToArray());
                }
            }
            catch (Exception e)
            {
                var errorMessage = GetExceptionInfo(e);

                _log.Error(errorMessage);
            }
        }


		/// <summary>
		/// Processing message for one product
		/// </summary>
		/// <param name="checkingProductId"></param>
		/// <param name="xmlMessages"></param>
		/// <param name="productSettings"></param>
		/// <param name="messagesCount"></param>
		/// <param name="enableInsertingArrMessages"></param>
		public void ProcessArrivalMessagesForProduct(int checkingProductId, List<string> xmlMessages, IList<ValidationSetting> productSettings, int messagesCount, bool enableInsertingArrMessages)
        {
            var startedAt = DateTime.UtcNow;

            _log.Info("Count of messages: " + xmlMessages.Count);

            try
            {
                var allArrivalMessages = new ConcurrentBag<ArrivalMessage>();
                var allEvents = new ConcurrentBag<Event>();

                var processTasks = new List<Task>();

                var counter = 10;
                var step = messagesCount / counter;

				// First: we parse all xml-messages and build sometimes xml-tree(as update)
				// also it is parallel process by each N messages(10 tasks for all messages)
                for (var i = 0; i < counter; i++)
                {
                    var index = i * 1;

                    var processingMessages = xmlMessages.Skip(index * step).Take(step).ToList();

                    var task = Task.Factory.StartNew(() =>
                    {
                        foreach (var xmlMessage in processingMessages)
                        {
                            var message = _arrivalMessageService.CreateArrivalMessage(checkingProductId, "", "", "");

							// Parsing and checking: sometimes xml contains only keep-alive messages
                            if (_downloadService.ProcessEventMessageAndParse(message, xmlMessage, enableInsertingArrMessages, ref allEvents))
                            {
                                message.IsProcessed = true;

                                allArrivalMessages.Add(message);
                            } // pass
                        }
                    });

                    processTasks.Add(task);
                }

                Task.WaitAll(processTasks.ToArray());

				// Now we get all events
                var events = allEvents.ToList();

                var eventsCount = events.Count;
                CreateSuccessSchedulerHistory(startedAt, checkingProductId, SchedulerTypes.ParseXmlFile, "Count: " + eventsCount + " events");

                _log.Info("Processing: " + eventsCount + " events; unique: " + events.DistinctBy(x => x.EventId).Count());

                if (eventsCount > 0)
                {
                    var startedOn = DateTime.UtcNow;

					// Then we also parallel all processing(inserting arrival messages and etc)
                    var waitingTasks = new List<Task>();

					//1 task, if it is push-product, but not product #5
                    if (enableInsertingArrMessages)
                    {
						// buld insert the packet of messages
                        var taskUpdateArrivalMessages = Task.Factory.StartNew(() =>
                        {
                            _arrivalMessageService.BulkInsert(allArrivalMessages.ToList(), "");
                        });
                        waitingTasks.Add(taskUpdateArrivalMessages);
                    }
                    
					//2 task
                    var taskMergeEvents = Task.Factory.StartNew(() =>
                    {
						var startMergeEvents = DateTime.UtcNow;

						// Ae get all old events
                        var oldEvents = _merger.GetEventsByEventIds(events.Select(x=>x.EventId).ToList(), checkingProductId);

						// And we add them all as repeated, because there are a lot of not-unique events. It is such optimization.
						sportsEventsStores[checkingProductId.ToString()].AddRepeated(oldEvents, events);

                        CreateSuccessSchedulerHistory(startMergeEvents, checkingProductId, SchedulerTypes.Merger, "Merged: " + oldEvents.Count + events.Count);
                    });

					// if it is't product #5 we create common arrival message
                    if (!enableInsertingArrMessages)
                    {
                        var taskInseringCommonArrMessage = Task.Factory.StartNew(() =>
                        {
                            var commonArrivalMessage = new ArrivalMessage
                            {
                                ProductId = checkingProductId,
                                IsProcessed = true,
                                Url = "Count of processed arrival messages: " + messagesCount,
                            };

                            var bets = allArrivalMessages.SelectMany(x => x.Bets ?? new List<string>()).ToList();
                            var messagesEvents = allArrivalMessages.SelectMany(x => x.Events ?? new List<string>()).ToList();
                            var leagues = allArrivalMessages.SelectMany(x => x.Leagues ?? new List<string>()).ToList();
                            var markets = allArrivalMessages.SelectMany(x => x.Markets ?? new List<string>()).ToList();
                            var sports = allArrivalMessages.SelectMany(x => x.Sports ?? new List<string>()).ToList();
                            var locations =
                                allArrivalMessages.SelectMany(x => x.Locations ?? new List<string>()).ToList();
                            var providers =
                                allArrivalMessages.SelectMany(x => x.Providers ?? new List<string>()).ToList();
                            var statuses = allArrivalMessages.SelectMany(x => x.Statuses ?? new List<string>()).ToList();

                            commonArrivalMessage.SetFields(true, messagesEvents, sports, leagues, locations, statuses, markets,
                                providers, bets);

                            _arrivalMessageRepository.Insert(commonArrivalMessage);
                        });

                        waitingTasks.Add(taskInseringCommonArrMessage);
                    }

                    _log.Info(String.Format("merged"));

					// We can't validate messages, while not merge all of them(for NumberOfChanges and DistinctNumberOfChanges)
                    taskMergeEvents.Wait();

					// Then we get all events for processing
                    var eventsForProcessing = sportsEventsStores[checkingProductId.ToString()].GetEvents();

					// And...cal saving dropdowns, validation, saving events!
                    StartProcessingTasks(startedOn, eventsForProcessing, checkingProductId, productSettings);

					// Clearing from memory.
                    sportsEventsStores[checkingProductId.ToString()].Clear();

                    Task.WaitAll(waitingTasks.ToArray());

                    _log.Info(String.Format("end processing-creating-merging-validating"));
                }
            }
            catch (Exception e)
            {
                var errorMessage = GetExceptionInfo(e);

                _log.Error(errorMessage);

                CreateFailedSchedulerHistory(startedAt, checkingProductId, SchedulerTypes.ProcessingAllArrivalMessages,
                    errorMessage);
            }
        }

		/// <summary>
		/// Not used. It is used as optimization for first validation withon history-values
		/// </summary>
		/// <param name="productSettings"></param>
		/// <returns></returns>
        public List<ValidationSetting> GetSettingsWithNumberOfChanges(IList<ValidationSetting> productSettings)
        {
            return productSettings
                        .Where(setting => setting.ValidationRules
                                            .Any(rule => rule.ParameterId == (int)ValidationParameterId.NumberOfChanges
                                                    || rule.ParameterId == (int)ValidationParameterId.NumberOfDistinctChanges)).ToList();
        }

		/// <summary>
		/// Not used. It is used as optimization for first validation withon history-values
		/// </summary>
		/// <param name="productSettings"></param>
		/// <returns></returns>
		public List<ValidationSetting> GetSettingsWithoutNumberOfChanges(IList<ValidationSetting> productSettings)
        {
            return productSettings
                        .Where(setting => !setting.ValidationRules
                                            .Any(rule => rule.ParameterId == (int)ValidationParameterId.NumberOfChanges
                                                    || rule.ParameterId == (int)ValidationParameterId.NumberOfDistinctChanges)).ToList();
        }
    }
}