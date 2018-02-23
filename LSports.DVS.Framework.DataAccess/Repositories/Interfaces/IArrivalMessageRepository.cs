using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LSports.DVS.Framework.DataAccess.Models;
using LSports.Framework.Models.CustomClasses;

namespace LSports.DVS.Framework.DataAccess.Repositories.Interfaces
{
    public interface IArrivalMessageRepository
    {
        int Insert(ArrivalMessage arrivalMessage);
        void BulkInsert(List<ArrivalMessage> messages, string groupId);

        void Update(ArrivalMessage arrivalMessage);
        void BulkUpdate(IList<ArrivalMessage> arrivalMessages);

        List<ArrivalMessage> GetLastArrivalMessageDateForProduct(int productId);

        IList<ArrivalMessage> GetFilteredItemsByProductId(int productId, FilterObject filter, int offset, int count);

        Dictionary<string, ArrivalMessage> GetChartDataForProduct(int productId, FilterObject filter);

        int GetCountOfFilteredItemsByProductId(int productId, FilterObject filter);
        IList<ArrivalMessage> GetNotProcessedMessages(List<int> productIds, int? limitCount);
    }
}
