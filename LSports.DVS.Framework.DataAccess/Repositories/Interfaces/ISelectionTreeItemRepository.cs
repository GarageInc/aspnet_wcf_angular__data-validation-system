using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSports.Framework.Models.CustomClasses;

namespace LSports.DVS.Framework.DataAccess.Repositories.Interfaces
{
    public interface ISelectionTreeItemRepository
    {
        IList<SelectionTreeItem> GetListByTypeId(int typeId);
        void Insert(Event @event);

        void BulkInsert(IList<Event> events, int? productId);
    }
}
