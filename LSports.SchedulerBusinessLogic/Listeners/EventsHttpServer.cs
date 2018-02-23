using System;
using System.Collections.Generic;
using System.Configuration;
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
	/// Simple http-server, which is used for DVS-site.
	/// We saved data(xmls) and BL in different servers. 
	/// If user want to get xml, it should send request with eventId. And this server will return xml-event(which is saved in harddrive).
	/// </summary>
    public class EventsHttpServer : BaseHttpServer
    {
        private string _basePath;

        public EventsHttpServer(string path, int port)
        {
            this.Initialize(path, port);

			// Path to folder with saved events
            _basePath = ConfigurationManager.AppSettings["BasePath"];
        }
		
		/// <summary>
		/// Return xml for event by product
		/// </summary>
		/// <param name="context"></param>
        protected override  void Process(HttpListenerContext context)
        {
            try
            {
                log.Info("start downloading from push-service");

                var eventId = context.Request.QueryString["eventId"];
                var productId = context.Request.QueryString["productId"];

                var xmlMessage = File.ReadAllBytes($"{_basePath}/EventsStorage/{productId}/{eventId}.gzip");

                context.Response.OutputStream.Write(xmlMessage,0,xmlMessage.Length);

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.Close();
            }
            catch (Exception e)
            {
                var message = $"{e.Message}; {e.StackTrace}";

                var messageData = Encoding.UTF8.GetBytes(message); 

                context.Response.OutputStream.Write(messageData, 0, messageData.Length);

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.Close();

                log.Error(message);
            }
        }

        protected void Initialize(string path, int port)
        {
            this._port = port;

            _serverThread = new Thread(this.Listen);
            _serverThread.Start();
        }

    }
}
