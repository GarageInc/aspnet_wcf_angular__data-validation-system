using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSports.Framework.Models.CustomClasses;
using LSports.Services.ViewModels;
using System;
using LSports.DVS.Framework.DataAccess.Models;

namespace LSports.Services.Interfaces
{
    public interface IArrivalMessageService
    {
        List<ArrivalMessage> GetLastArrivalMessageNotProcessed(int productId);

        IList<ArrivalMessageViewModel> GetArrivalMessages(int productId, FilterObject filter, int offset, int count);
        int GetCountOfArrivalMessages(int productId, FilterObject filter);

        ArrivalMessage CreateArrivalMessage(int productId, string outerXml, string pathToFile, string sportId);

        void BulkInsert(List<ArrivalMessage> messages, string groupId);
    }
}
