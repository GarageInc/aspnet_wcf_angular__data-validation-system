using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LSports.DVS.Framework.DataAccess.Repositories;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Scheduler;
using LSports.Scheduler.Services;
using log4net;
using LSports.Framework.Models.CustomClasses;
using LSports.Services;
using LSports.Services.Interfaces;

namespace LSports.Scheduler.Listeners
{
	/// <summary>
	/// Not used now. It service was created for WCF-service, which can listen incoming request with xml-events
	/// </summary>
	public class PushHttpServer : BaseHttpServer
	{
		protected ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public int productId;

		public PushHttpServer(string path, int port, int productId)
		{
			this.Initialize(path, port, productId);
		}

        protected override void Process(HttpListenerContext context)
		{
		    try
		    {
		        log.Info("start downloading from push-service");
                
		        using (StreamReader reader = new StreamReader(context.Request.InputStream))
		        {
                    string xmlMessage = reader.ReadToEnd();

                    reader.Close();

                    context.Response.StatusCode = (int)HttpStatusCode.OK;

                    context.Response.Close();

                    //if (xmlMessage.Contains("<Event"))
                    {
                        if (xmlMessage.StartsWith("data="))
                        {
                            xmlMessage = xmlMessage.Substring(5, xmlMessage.Length - 5);
                        }

                        ProductsEventsConcurencyQueue.GetInstance().Storage.store[productId].Enqueue(xmlMessage);
                    }
                }
            }
		    catch (Exception e)
		    {
		        var message = String.Format("{0}; {1}", e.Message, e.StackTrace);

		        log.Error(message);

                context.Response.StatusCode = (int)HttpStatusCode.OK;

                context.Response.Close();
            }
		}

		protected void Initialize(string path, int port, int productId)
		{
			this.productId = productId;

			base.Initialize(path, port);
		}

	}
}
