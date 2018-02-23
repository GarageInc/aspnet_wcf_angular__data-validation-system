using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LSports.Framework.Models.CustomClasses;
using System;

namespace LSports.DVS.Framework.DataAccess.Repositories.Interfaces
{
    public interface ISchedulerHistoryRepository
    {
        void Insert(SchedulerHistory history);

        Dictionary<string, List<SchedulerHistoryData>> GetHistoryByProductId(int productId, DateTime from, DateTime to);

        IList<Object> GetHistoryOfValidation(DateTime dateFrom, DateTime dateTo);
    }
}
