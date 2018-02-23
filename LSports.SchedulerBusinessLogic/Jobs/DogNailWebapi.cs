using log4net;
using LSports.DVS.Framework.DataAccess.Repositories;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;
using LSports.Framework.Models.Enums;
using LSports.Scheduler.Services;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml;
using LSports.Scheduler.Jobs.@base;

namespace LSports.Scheduler.Jobs
{
	// Dog-nail for IIS.
	/// <summary>
	/// Because IIS each N seconds killed app, we always send http-request to web-api project. If ISS will kill app, then all data will be cleared from memory. 
	/// It is bad.
	/// And this code work from EventsService
	/// </summary>
    [DisallowConcurrentExecution]
    public class DogNailWebapi : XmlWorker
    {
        protected readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        override public void Run()
        {
            _log.Info("Start sending request to WebApi2");

			// Sending test-request
            try
            {
                var pathToDognailUrl = ConfigurationManager.AppSettings[SchedulerConfig.EventsServiceWebApiUrlAlias];
                
                WebRequest wrGETURL = WebRequest.Create(pathToDognailUrl);

                wrGETURL.Method = WebRequestMethods.Http.Get;   
                             
                MemoryStream ms = new MemoryStream();
                wrGETURL.GetResponse().GetResponseStream().CopyTo(ms);

                var data = ms.ToArray();

                var message = Encoding.UTF8.GetString(data, 0, data.Length);

                _log.Info(message);

                _log.Info("Success with receiving message!");
            }
            catch (Exception exception)
            {
                var message = exception.Message + " ; " + exception.StackTrace;

                if (exception.InnerException != null)
                {
                    var innerException = exception.InnerException;
                    var innerExceptionMessage = innerException.Message + " ; " + innerException.StackTrace;
                    message = $"{message}; {innerExceptionMessage}";
                }

                _log.Error(message);
            }
            
        }
    }
}