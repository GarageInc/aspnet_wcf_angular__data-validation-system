using Common.Logging;
using LSports.DVS.Framework.DataAccess.Repositories;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;
using LSports.Framework.Models.Enums;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace LSports.Scheduler.Jobs.@base
{

    [DisallowConcurrentExecution]
    public class XmlWorker : IJob
    {
        protected ISchedulerHistoryRepository _shedulerHistoryRepository = new SchedulerHistoryRepository();

        public void Execute(IJobExecutionContext context)
        {
            Run();
        }

		// Must be realized in childs
        public virtual void Run()
        {
            throw new NotImplementedException();
        }

		/// <summary>
		/// Creating reports for each type of action(Validation,Merge,Building xml-tree and etc)
		/// </summary>
		/// <param name="startedOn"></param>
		/// <param name="productId"></param>
		/// <param name="type"></param>
		/// <param name="additionalInfo"></param>
        public void CreateSuccessSchedulerHistory(DateTime startedOn, int? productId, SchedulerTypes type, string additionalInfo)
        {
            _shedulerHistoryRepository.Insert(new SchedulerHistory
            {
                ArrivalMessageId = null,
                CreatedBy = "Admin",
                FinishedOn = DateTime.UtcNow,
                CreatedOn = DateTime.UtcNow,
                IsActive = true,
                Type = (int)type,
                ProductId = productId,
                StartedOn = startedOn,

                AdditionalInfo = additionalInfo,

                IsByError = false,
                ErrorMessage = ""
            });
        }

        public void CreateFailedSchedulerHistory(DateTime startedOn, int productId, SchedulerTypes type, string errorMessage, string additionalInfo = "")
        {
            _shedulerHistoryRepository.Insert(new SchedulerHistory
            {
                ArrivalMessageId = null,
                CreatedBy = "Admin",
                FinishedOn = DateTime.UtcNow,
                CreatedOn = DateTime.UtcNow,
                IsActive = true,

                AdditionalInfo = additionalInfo,

                Type = (int)type,
                ProductId = productId,
                StartedOn = startedOn,

                IsByError = true,
                ErrorMessage = errorMessage
            });
        }

		/// <summary>
		/// Getting exception info from parent and child.
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public string GetExceptionInfo(Exception e)
        {
            var message = string.Format("\n{0}; {1}", e?.Message, e?.StackTrace);

            if (e?.InnerException != null)
            {
                return string.Format("\n{0}; {1} {2}", message, e?.InnerException.Message, e?.InnerException.StackTrace);
            }
            else
            {
                return message;
            }
        }
    }


    

}