using System;
using System.Collections.Generic;
using LSports.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;
using LSports.DVS.Framework.DataAccess.Models;
using System.Linq;

namespace LSports.Framework.DataAccess.Repositories
{
    public class PriorityRepository : IPriorityRepository
    {

        public IList<Priority> List()
        {
            using (gb_dvsstagingEntities entities = new gb_dvsstagingEntities())
            {
                var q = entities.val_priority.Select(rec => new Priority()
                {
                    Id = rec.Id,
                    Name = rec.Name
                }).OrderBy(rec => rec.Id);

                return q.ToList();
            }
        }
    }
}
