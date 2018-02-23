using System;
using System.Collections.Generic;
using LSports.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;

namespace LSports.Framework.DataAccess.Repositories
{
    public class IconRepository : IIconRepository
    {
        public IList<_Icon> GetList()
        {
            throw new NotImplementedException();
                //using (var model = new gb_dvsstagingEntities())
                //{
                //    var q = model.tic_Icons.Select(rec => new _Icon()
                //    {
                //        Id = rec.Id,
                //        Name = rec.Name,
                //        Icon = rec.Icon
                //    }).OrderBy(rec=>rec.Name);

                //    return q.ToList();
                //}
        }
    }
}
