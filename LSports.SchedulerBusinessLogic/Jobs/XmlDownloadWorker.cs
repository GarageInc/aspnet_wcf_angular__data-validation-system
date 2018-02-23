using log4net;
using LSports.DVS.Framework.DataAccess.Repositories;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.Enums;
using LSports.Scheduler.Services;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using LSports.DVS.Framework.DataAccess.Models;
using LSports.Framework.Models.CustomClasses;
using LSports.Scheduler.Jobs.@base;
using LSports.Services;
using LSports.Services.Interfaces;

namespace LSports.Scheduler.Jobs
{
	// Downloader for pull-service
    [DisallowConcurrentExecution]
    public class XmlDownloadWorker : XmlWorker
    {
        protected XmlDownloadService downloadService = new XmlDownloadService();

        protected readonly IArrivalMessageService _arrivalMessageService = new ArrivalMessageService();

        protected readonly IProductRepository _productRepository = new ProductRepository();

        protected readonly IArrivalMessageRepository _arrivalMessageRepository = new ArrivalMessageRepository();

        protected Sports Sports = new Sports();

        protected readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public override void Run()
        {
			// datetime variable for logging
            var startedOn = DateTime.UtcNow;

            int currentProductId = 1;
            int? currentArrivalMessageId = null;

            try
            {
				// false - pull-products, if true - only push-products
                var products = _productRepository.GetListByType(false);

				// for each product in pull-products(now only one product)
                foreach (var product in products)
                {
                    var messages = new List<ArrivalMessage>();

                    var notProccessedMessages = GetLastArrivalMessagesNotProcessed(product.Id);

					// if we have a lot of not processed messages we stop downloading. 
					// because downloading always works faster than message processing.
					if (notProccessedMessages.Count > Sports.sports.Count/2)
                    {
                        continue;
                    }

                    currentProductId = product.Id;

					// one file - on task
                    var tasks = new List<Task>();
					// variable for logging common file size
                    var commonSize = 0L;

					// timestamp variable
                    var downloadFrom = notProccessedMessages.FirstOrDefault()?.CreatedOn ?? DateTime.UtcNow.AddDays(-1);

                    foreach (var sport in Sports.sports)
                    {
                        var downloadTask = Task.Factory.StartNew(() =>
                        {
                            try
                            {
								// building gateway url with sportid and timestamp
                                var gateway = product.GatewayAPI + "&timestamp=" + ConvertToUnixTimestamp(downloadFrom) +
                                              "&sports=" + sport.Id;

                                var filePath = downloadService.DownloadFile(product, gateway);
								
                                var isContainsEvents = downloadService.IsContainsEvents(product.Id, filePath);
								// if downloaded file contains events...
                                if (isContainsEvents)
                                {
                                    commonSize += (new System.IO.FileInfo(filePath)).Length;

									// new not processed arrival message
                                    var message = CreateArrivalMessage(product.Id, filePath, gateway,
                                        sport.Id.ToString());

                                    _arrivalMessageRepository.Insert(message);

                                    messages.Add(message);
                                }
                            }
							// else logging error
                            catch (WebException webError)
                            {
                                var httpResponse = (HttpWebResponse)webError.Response;
                                var message = "";

                                if (httpResponse != null)
                                {
                                    message =
                                        $"{webError.Message}; {httpResponse.StatusDescription}; {GetExceptionInfo(webError)}";
                                }
                                else
                                {
                                    message = $"{webError.Message}";
                                }

                                _log.Error(message);
                                CreateFailedSchedulerHistory(startedOn, currentProductId, SchedulerTypes.Downloader, message);
                            }
                            catch (Exception e)
                            {
                                var message = GetExceptionInfo(e);

                                _log.Error(message);
                                CreateFailedSchedulerHistory(startedOn, currentProductId, SchedulerTypes.Downloader, message);
                            }
                        });

                        tasks.Add(downloadTask);
                    }

					// waiting of all downloaders
                    Task.WaitAll(tasks.ToArray());

					// logging common result
                    _log.Info("Saved: " + messages.Count + " messages");
                    var fileSizeString = $"Size: {Math.Round((double)commonSize / 1024/1024, 2)} MB";
                    CreateSuccessSchedulerHistory(startedOn, currentProductId, SchedulerTypes.Downloader, fileSizeString);
                }
            }
            catch (WebException webError)
            {
                var httpResponse = (HttpWebResponse)webError.Response;

                var message = "";

                if (httpResponse != null)
                {
                    message = $"{webError.Message}; {httpResponse.StatusDescription}; {GetExceptionInfo(webError)}";
                }
                else
                {
                    message = $"{webError.Message}";
                }

                _log.Error(message);
                CreateFailedSchedulerHistory(startedOn, currentProductId, SchedulerTypes.Downloader, message);
            }
            catch (Exception e)
            {
                var message = GetExceptionInfo(e);

                _log.Error(message);
                CreateFailedSchedulerHistory(startedOn, currentProductId, SchedulerTypes.Downloader, message);
            }
            finally
            {
//                _log.Info(String.Format("End downloading"));
            }
        }


		// creating new arrival message in database
        public ArrivalMessage CreateArrivalMessage(int productId, string pathToFile, string gateway, string sportId)
        {
            // 2 step: create arrival message with xml
            return _arrivalMessageService.CreateArrivalMessage(productId, pathToFile, gateway, sportId);// xml a very big: out of memory exception
        }


		/// <summary>
		/// Getting last not processed arrival message
		/// </summary>
		/// <param name="productId"></param>
		/// <returns></returns>
        public List<ArrivalMessage> GetLastArrivalMessagesNotProcessed(int productId)
        {
            return _arrivalMessageService.GetLastArrivalMessageNotProcessed(productId);
        }

		/// <summary>
		/// Converting to unix timestamp from selected system DateTime
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
        public static long ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return (long)diff.TotalSeconds;
        }
    }
}