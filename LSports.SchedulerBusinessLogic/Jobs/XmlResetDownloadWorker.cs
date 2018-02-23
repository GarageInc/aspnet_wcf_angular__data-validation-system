
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace LSports.Scheduler.Jobs
{
    using LSports.Framework.Models.Enums;
    using Quartz;
    using System;
    using System.Net;

	/// <summary>
	/// Each N seconds this service is started. And this downloader download files with update for each sport(by sportId). And then create not processed ArrivalMessages.
	/// </summary>
    [DisallowConcurrentExecution]
    public class XmlResetDownloadWorker : XmlDownloadWorker
    {
        public override void Run()
        {
            var startedOn = DateTime.UtcNow;

            int currentProductId = 1;

            try
            {
				// Select all pull-product(because param === false)
                var products = _productRepository.GetListByType(false);

                foreach (var product in products)
                {
					// Getting all not processed messages for current product
                    var notProccessedMessages = GetLastArrivalMessagesNotProcessed(product.Id);

					// Do't start downloading, if limit of not processed message more, than below.
                    if (notProccessedMessages.Count > Sports.sports.Count / 2)
                    {
						// because processing is slower, than downloading(it can call OutOfMemoryException)
                        continue;
                    }

                    currentProductId = product.Id;
                    var commonSize = 0.0;

                    try
                    {
                        var tasks = new List<Task>();

						// Parsing timestamp interval from web.config
                        var date = DateTime.UtcNow;
                        var resetDownloaderTimeStamp = 0;

                        var parsed = int.TryParse(ConfigurationManager.AppSettings[SchedulerConfig.ResetDownloaderTimeStampAlias], out resetDownloaderTimeStamp);
                        if (parsed)
                        {
                            var timespanSeconds = new TimeSpan(0, 0, 0, resetDownloaderTimeStamp);
                            date = date.Add(timespanSeconds);
                        }

						// one sport - one parallel task
                        foreach (var sport in Sports.sports)
                        {
                            var downloadTask = Task.Factory.StartNew(() =>
                            {
                                try
                                {
                                    var gateway = product.GatewayAPI + "&timestamp=" + ConvertToUnixTimestamp(date) 
                                                + "&sports=" + sport.Id;

                                    var pathToFile = downloadService.DownloadFile(product, gateway);

									// If xml-file contains only keep-alive messages - we will't process it(deleting from harddrive and memory)
                                    var isContainsEvents = downloadService.IsContainsEvents(product.Id, pathToFile);

                                    if (isContainsEvents)
                                    {
                                        commonSize += (new System.IO.FileInfo(pathToFile)).Length;

                                        var message = CreateArrivalMessage(product.Id, pathToFile, gateway, sport.Id.ToString());

                                        _arrivalMessageRepository.Insert(message);
                                    }
                                }
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

                        Task.WaitAll(tasks.ToArray());

                    }
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
        }


    }
}