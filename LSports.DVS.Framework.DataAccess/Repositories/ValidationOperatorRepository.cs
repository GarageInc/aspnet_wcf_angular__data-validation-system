using System.Collections.Generic;
using System.Linq;
using LSports.DVS.Framework.DataAccess.Models;
using LSports.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;

namespace LSports.DVS.Framework.DataAccess.Repositories
{
    public class ValidationOperatorRepository : IValidationOperatorRepository
    {
        protected gb_dvsstagingEntities Entities = new gb_dvsstagingEntities();

        public IList<ValidationOperator> List()
        {
            using (Entities)
            {
                var q = Entities.val_operator.Where(rec => rec.IsActive).Select(rec => new ValidationOperator()
                {
                    Id = rec.Id,
                    Name = rec.Name
                }).OrderBy(rec => rec.Name);

                return q.ToList();
            }
        }
    }
}
