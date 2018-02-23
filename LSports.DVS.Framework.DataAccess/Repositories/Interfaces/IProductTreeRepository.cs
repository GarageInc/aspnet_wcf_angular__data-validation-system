using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LSports.Framework.Models.CustomClasses;

namespace LSports.DVS.Framework.DataAccess.Repositories.Interfaces
{
    public interface IProductTreeRepository
    {
        void Insert(int productId, TreeNode treeNode);

        void Update(ProductTree productTree);

        ProductTree GetItemByProductId(int productId);
    }
}
