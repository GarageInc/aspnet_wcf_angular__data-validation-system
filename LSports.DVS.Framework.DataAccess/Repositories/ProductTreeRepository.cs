using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using LSports.DVS.Framework.DataAccess.Models;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;

namespace LSports.DVS.Framework.DataAccess.Repositories
{
    public class ProductTreeRepository : IProductTreeRepository
    {

        protected Object thisLock = new Object();

        public ProductTree GetItemByProductId(int productId)
        {
            lock (thisLock)
            {
                using (var model = new gb_dvsstagingEntities())
                {
                    var q = (from tree in model.dvs_producttree
                             where tree.IsActive
                             where tree.ProductId == productId
                             select new ProductTree
                             {
                                 Id = tree.Id,
                                 XmlText = tree.XmlText,
                                 ProductId = tree.ProductId
                             }).FirstOrDefault();

                    if (q != null && !String.IsNullOrEmpty(q.XmlText))
                    {
                        using (var textReader = new StringReader(q.XmlText))
                        {
                            var resultNode = (TreeNode)new XmlSerializer(typeof(TreeNode)).Deserialize(textReader);
                            q.Tree = new TreeCore { core = new TreeData { data = resultNode } };
                        }
                    }

                    return q;
                }
            }
        }

        public void Insert(int productId, TreeNode treeNode)
        {
            lock (thisLock)
            {
                string xmlText;

                using (var textWriter = new StringWriter())
                {
                    new XmlSerializer(typeof(TreeNode)).Serialize(textWriter, treeNode);

                    xmlText = textWriter.ToString();
                }

                using (var model = new gb_dvsstagingEntities())
                {
                    var productTree = new dvs_producttree
                    {
                        ProductId = productId,
                        XmlText = xmlText,
                        CreatedBy = "Admin",
                        IsActive = true
                    };
                    model.dvs_producttree.Add(productTree);
                    model.SaveChanges();
                }
            }
        }

        public void Update(ProductTree productTree)
        {
            lock (thisLock)
            {
                string xmlText;

                using (var textWriter = new StringWriter())
                {
                    new XmlSerializer(typeof(TreeNode)).Serialize(textWriter, productTree.Tree.core.data);

                    xmlText = textWriter.ToString();
                }

                using (var model = new gb_dvsstagingEntities())
                {
                    var productTreeModel = model.dvs_producttree.Find(productTree.Id);

                    productTreeModel.ProductId = productTree.ProductId;
                    productTreeModel.XmlText = xmlText;
                    productTreeModel.UpdatedOn = DateTime.UtcNow;

                    model.Entry(productTreeModel).State = System.Data.Entity.EntityState.Modified;

                    model.SaveChanges();
                }
            }
        }
    }
}