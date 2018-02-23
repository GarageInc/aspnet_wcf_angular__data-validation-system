

using System.Collections.Concurrent;
using System.Configuration;

namespace LSports.Scheduler.Jobs.@base
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
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Amib.Threading;
    using LSports.Framework.DataAccess.Repositories.Interfaces;
	/// <summary>
	/// We use SmartThreadPull for processing arrival messages. And it is class for this thread data.
	/// </summary>
    public class ThreadInfo
    {
        public ArrivalMessage Message { get; set; }
        public IList<ValidationSetting> ValidationSettings { get; set; }
    }

    [DisallowConcurrentExecution]
    public class XmlCommonWorker : XmlWorker
    {

		// Stores for each kind of sports
        protected ConcurrentDictionary<string, EventsStoreAndLocker> sportsEventsStores;
        protected Dictionary<string, Object> sportsLockers = new Dictionary<string, object>();

		// Store for ArrivalMessages
        protected MessagesStore messagesStore;

		// Heplers for processing events
        protected readonly XmlParser _parser = new XmlParser();
        protected readonly XmlMerger _merger = new XmlMerger();
        protected readonly XmlDownloadService _downloadService = new XmlDownloadService();

		// Repositories for CRUD operations
        protected readonly IArrivalMessageRepository _arrivalMessagesRepository = new ArrivalMessageRepository();
        protected readonly IEventRepository _eventRepository = new EventRepository();
        protected readonly ISelectionTreeItemRepository _selectionTreeItemRepository = new SelectionTreeItemRepository();
        private readonly IValidationSettingRepository _validationSettingsRepository = new ValidationSettingRepository();
        private readonly IValidationRuleRepository _validationRuleRepository = new ValidationRuleRepository();

		// Common logger
        protected readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected readonly IProductRepository productRepository = new ProductRepository();

		// List of checking products
        public List<int> checkingProductIds;

		// Common locker for synchronization of threads
        public object locker = new Object();

		// Initialization of events stores and sport-lockers
        public virtual void InitDictionary(IList<ArrivalMessage> messages )
        {
            sportsEventsStores = new ConcurrentDictionary<string, EventsStoreAndLocker>();

            foreach(var message in messages)
            {
                if (!sportsEventsStores.ContainsKey(message.GroupId))
                {
                    sportsEventsStores.AddOrUpdate(message.GroupId, new EventsStoreAndLocker(), (key, oldValue) => new EventsStoreAndLocker());
                }

                if (!sportsLockers.ContainsKey(message.GroupId))
                {
                    sportsLockers.Add(message.GroupId, new Object());
                }
            }
        }

		/// <summary>
		/// Mapping val.rules to val.settings. Also we make some optimizations for validations.
		/// It is checking of related val.rules and val.rules, which are validating all nodes from xml(using in Setting.Expression)
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="rules"></param>
        public void SetRulesForSettings(IList<ValidationSetting> settings, IList<ValidationRule> rules )
        {
            foreach (var validationSetting in settings)
            {
                validationSetting.ValidationRules =
                    rules.Where(x => x.ValidationSettingId == validationSetting.Id).ToList();

                validationSetting.IsContainsRuleForAllNodes = validationSetting.ValidationRules.Any(x => x.IsForAllNodes);

                var containsRelated = false;

                for (var i = 0; i < validationSetting.ValidationRules.Count && !containsRelated; i++)
                {
                    var rule = validationSetting.ValidationRules[i];

                    containsRelated = validationSetting.ValidationRules
                        .Any(
                            x =>
                                x.PathToValueParentNode != null 
                                && x.PathToValueParentNode == rule.PathToPropertyParentNode // checking one node
                                && rule.Id != x.Id);// is not one rule

                    containsRelated = containsRelated || validationSetting.ValidationRules
                        .Any(
                            x =>
                                x.PathToPropertyParentNode != null 
                                && x.PathToPropertyParentNode == rule.PathToValueParentNode
                                && rule.Id != x.Id);

                }

                validationSetting.IsContainsRelatedRules = containsRelated;
            }
        }

		// Entry-point for processing messages
        public virtual void ProcessArrivalMessages()
        {
            try
            {
				// Getting all necessary info and initialization stores before processing
                var validationSettings = GetValidationSettingsForProducts(checkingProductIds);
                var validationRules = GetValidationRules();

                SetRulesForSettings(validationSettings, validationRules);

                lock (locker)
                {
                    messagesStore = new MessagesStore();

					var messages = _arrivalMessagesRepository.GetNotProcessedMessages(checkingProductIds, null)
                        .OrderBy(t => t.CreatedOn).ToList();

                    messagesStore.Messages = messages;

					InitDictionary(messages);
                }

                SmartThreadPool smartThreadPool = new SmartThreadPool();

				// Max count threads, which can work together in one time
                var threadpoolMaxCountOfThreads = int.Parse(ConfigurationManager.AppSettings[SchedulerConfig.NumberOfSimultaneouslyProcessedMessagesAlias]);
                smartThreadPool.MaxThreads = (int)Math.Min(threadpoolMaxCountOfThreads, messagesStore.Messages.Count);

				// Parallel processing by using SmartThreadPool
                foreach (ArrivalMessage message in messagesStore.Messages)
                {
                    smartThreadPool.QueueWorkItem(new WorkItemCallback(this.callback), new ThreadInfo
                    {
                        Message = message,
                        ValidationSettings = validationSettings.Where(x => x.ProductId == message.ProductId).ToList(),
                    });
                }

                smartThreadPool.WaitForIdle();

                smartThreadPool.Shutdown();

				// Then we delete all files, if their count more than limit(another way it can call OutOfMemoryException)
	            foreach (var checkingProductId in checkingProductIds)
				{
					_downloadService.CheckTmpFilesDir(checkingProductId);
				}
            }
            catch (Exception er)
            {
                var errorMessage = GetExceptionInfo(er);

                _log.Error(errorMessage);
            }
        }

        public object callback(object a)
        {
            var info = a as ThreadInfo;

            var startedOn = DateTime.UtcNow;

            try
            {
				// It this file exist - DVS process it(it can't exist, if system was crashed before marking "Processed" for arrival message)
                if (File.Exists(info.Message.PathToXmlFile))
                {
                    ProcessPullMessage(info.Message, info.ValidationSettings);
                    File.Delete(info.Message.PathToXmlFile);
                }
                else
                {
                    setAsProcessedWithLock(info.Message);
                }

                var dateUpdateArrivalMessages = DateTime.UtcNow;

                _arrivalMessagesRepository.Update(info.Message);

                CreateSuccessSchedulerHistory(dateUpdateArrivalMessages, info.Message.ProductId,
                    SchedulerTypes.UpdatingArrivalMessages, "");
            }
            catch (Exception e)
            {
                var errorMessage = GetExceptionInfo(e);

                _log.Error(errorMessage);
                CreateFailedSchedulerHistory(startedOn, info.Message.ProductId, SchedulerTypes.ProcessingArrivalMessage,
                    errorMessage);
            }
            finally
            {
                if (File.Exists(info.Message.PathToXmlFile))
                {
                    File.Delete(info.Message.PathToXmlFile);
                }
            }

            return true;
        }

		/// <summary>
		/// One message - one processing.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="validationSettings"></param>
        protected void ProcessPullMessage(ArrivalMessage message, IList<ValidationSetting> validationSettings)
        {
            DateTime startedOn = DateTime.UtcNow;

            try
            {
                var startDate = DateTime.UtcNow;

				// We get all parsed events from big-xml file(message)
                var messageEvents = _downloadService.ProcessPullArrivalMessage(message);

                var messageText = "Count: " + messageEvents.Count + " events";

                CreateSuccessSchedulerHistory(startDate, message.ProductId, SchedulerTypes.ParseXmlFile, messageText);

                if (messageEvents.Count > 0)
                {

                    //var startValidateEvents = DateTime.UtcNow;

                    //XmlValidator _validator = new XmlValidator();
                    //_validator.ValidateEventsForProduct(messageEvents, validationSettings);
                    //CreateSuccessSchedulerHistory(startValidateEvents, message.ProductId, SchedulerTypes.EventsValidator, messageEvents.Count + " events");

					// Then by locking each kind of sport we process all all messages
                    lock (sportsLockers[message.GroupId])
                    {
                        var startMergeEvents = DateTime.UtcNow;

						// distinct by EventId(for getting old events from storage)
                        var messageEventsIds = messageEvents.Select(x => x.EventId)
                            .Distinct();

						// all current processed events
                        var savedEventsIds = sportsEventsStores[message.GroupId].GetEvents()
                            .Select(x => x.EventId)
                            .Distinct();

						// and then we get all new events(which are not exist in storage)
                        var notSavedEventsIds = messageEventsIds.Except(savedEventsIds)
                            .ToList();

						// Then we add merge events as not-repeated(because one big xml-file contains only unique events)
                        sportsEventsStores[message.GroupId].AddNotRepeated(_merger.GetEventsByEventIds(notSavedEventsIds, message.ProductId), messageEvents);

						CreateSuccessSchedulerHistory(startMergeEvents, message.ProductId, SchedulerTypes.Merger, messageText);

						// Can we process this xml-file?(Is it the last xml-file for selected kind of sport?)
                        bool canProcess = setAsProcessedWithLock(message);

                        if (canProcess)
                        {
                            var processedEvents = sportsEventsStores[message.GroupId].GetEvents();

                            if (processedEvents.Count > 0)
                            {
								// Validation,saving of events
                                StartProcessingTasks(startDate, processedEvents, message.ProductId, validationSettings);

								// Clearing from memory
								sportsEventsStores[message.GroupId].Clear();

                                System.GC.Collect();
                            }
                        }
                    }
                }
                else
                {
                    setAsProcessedWithLock(message);
                }

                CreateSuccessSchedulerHistory(startedOn, message.ProductId, SchedulerTypes.ProcessingArrivalMessage, "");
            }
            catch (Exception e)
            {
                var errorMessage = GetExceptionInfo(e);
                _log.Error(errorMessage);
                CreateFailedSchedulerHistory(startedOn, message.ProductId, SchedulerTypes.ProcessingArrivalMessage, errorMessage);
            }
        }

		/// <summary>
		/// Setting as processed current arrival message. 
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
        public bool setAsProcessedWithLock(ArrivalMessage message)
        {
            lock (locker)
            {
                messagesStore.SetAsProcessed(message.Id);

				// Is all messages processed for this sportId?
                return !messagesStore.IsContainsNotProcessedForGroup(message.GroupId);
            }
        }

		/// <summary>
		/// Common processing events, which were accumulated for one kind of sport
		/// </summary>
		/// <param name="startedOn"></param>
		/// <param name="events"></param>
		/// <param name="productId"></param>
		/// <param name="validationSettings"></param>
        public void StartProcessingTasks(DateTime startedOn, List<Event> events, int productId, IList<ValidationSetting> validationSettings)
        {
            var logMessage = "Common: " + events.Count;
            
            var processingTasks = new List<Task>();

			// 1 task: saving events in storages(xmls are saved on harddrive and meta-info is saved in database)
            var taskOfInserting = Task.Factory.StartNew(() =>
            {
	            try
	            {
					var startInsertEvents = DateTime.UtcNow;
					foreach (var processedEvent in events)
					{
						processedEvent.Save();
					}
					_eventRepository.BulkInsert(events);
					CreateSuccessSchedulerHistory(startInsertEvents, productId, SchedulerTypes.EventsCreator, logMessage);
				}
	            catch (Exception e)
				{
					var errorMessage = GetExceptionInfo(e);
					_log.Error(errorMessage);
					CreateFailedSchedulerHistory(startedOn, productId, SchedulerTypes.EventsCreator, errorMessage);
				}
            });
            processingTasks.Add(taskOfInserting);

			// 2 task: creationg selection tree item(for easy seraching in client-side)
            var taskOfSelectionTreeItem = Task.Factory.StartNew(() => {
				try
				{
					var startSelectionTreeItemCreating = DateTime.UtcNow;
					_selectionTreeItemRepository.BulkInsert(events, productId);
					CreateSuccessSchedulerHistory(startSelectionTreeItemCreating, productId, SchedulerTypes.SelectionTreeItemCreator, logMessage);
				}
				catch (Exception e)
				{
					var errorMessage = GetExceptionInfo(e);
					_log.Error(errorMessage);
					CreateFailedSchedulerHistory(startedOn, productId, SchedulerTypes.SelectionTreeItemCreator, errorMessage);
				}
            });
            processingTasks.Add(taskOfSelectionTreeItem);

            if (validationSettings.Count > 0)
            {
				// 3 task: validation of events
                var taskOfValidation = Task.Factory.StartNew(() =>
				{
					try
					{
						var startValidateEvents = DateTime.UtcNow;

						XmlValidator _validator = new XmlValidator();
						_validator.ValidateEventsForProduct(events, validationSettings);

						CreateSuccessSchedulerHistory(startValidateEvents, productId, SchedulerTypes.EventsValidator, logMessage);
					}
					catch (Exception e)
					{
						var errorMessage = GetExceptionInfo(e);
						_log.Error(errorMessage);
						CreateFailedSchedulerHistory(startedOn, productId, SchedulerTypes.EventsValidator, errorMessage);
					}
                });
                processingTasks.Add(taskOfValidation);
            }

			//4 task: updating datetime of current validation for selected validation settings
            var taskOfUpdatingLastValidateAtForSettings = Task.Factory.StartNew(() =>
			{
				try
				{
					UpdateLastValidateAtFor(validationSettings.Select(x => x.Id).ToList());
				}
				catch (Exception e)
				{
					var errorMessage = GetExceptionInfo(e);
					_log.Error(errorMessage);
					CreateFailedSchedulerHistory(startedOn, productId, SchedulerTypes.UpdatingLastValidateAt, errorMessage);
				}
            });
            processingTasks.Add(taskOfUpdatingLastValidateAtForSettings);

            Task.WaitAll(processingTasks.ToArray());

            _log.Info(String.Format("processed"));

            CreateSuccessSchedulerHistory(startedOn, productId, SchedulerTypes.ProcessingAllArrivalMessages, logMessage);
        }

		/// <summary>
		/// Get all validation settings for list of products
		/// </summary>
		/// <param name="productsIds"></param>
		/// <returns></returns>
        public IList<ValidationSetting> GetValidationSettingsForProducts(List<int> productsIds)
        {
            return _validationSettingsRepository.GetListForProducts(productsIds);
        }

		/// <summary>
		/// Get full list of val.rules
		/// </summary>
		/// <returns></returns>
        public IList<ValidationRule> GetValidationRules()
        {
            return _validationRuleRepository.List();
        }

        public void UpdateLastValidateAtFor(List<int> settingsIds)
        {
            _validationSettingsRepository.UpdateLastValidateAtFor(settingsIds);
        }
    }
}