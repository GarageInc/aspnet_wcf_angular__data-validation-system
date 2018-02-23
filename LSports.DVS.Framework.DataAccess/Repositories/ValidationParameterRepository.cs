using System.Collections.Generic;
using System.Linq;
using LSports.DVS.Framework.DataAccess.Models;
using LSports.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;

namespace LSports.DVS.Framework.DataAccess.Repositories
{
    public class ValidationParameterRepository : IValidationParameterRepository
    {
        protected gb_dvsstagingEntities Entities = new gb_dvsstagingEntities();

        public IList<ValidationParameter> List()
        {
            using (Entities)
            {
                var q = Entities.val_parameter.Where(rec => rec.IsActive).Select(rec => new ValidationParameter()
                {
                    Id = rec.Id,
                    Name = rec.Name
                }).OrderBy(rec => rec.Name);

                return q.ToList();
            }
        }
    }
}
