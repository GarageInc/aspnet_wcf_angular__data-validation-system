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
	/// Simple http-listener for getting http-request. 
	/// This class must be inherited.
	/// </summary>
	public class BaseHttpServer
    {
        protected ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected Thread _serverThread;

        protected HttpListener _listener;
        protected string _path;
        protected int _port;
        protected int productId;

        public int Port
        {
            get { return _port; }
            protected set { }
        }
        
        public void Stop()
        {
            _serverThread.Abort();
            _listener.Stop();
        }

        protected void Listen()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://*:" + _port.ToString() + "/");
            _listener.Start();

            while (_listener.IsListening)
            {
                try
                {
                    HttpListenerContext context = _listener.GetContext();
                    Process(context);
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
            }
        }

        protected virtual void Process(HttpListenerContext context)
        {
            throw new NotImplementedException();
        }


        protected virtual void Initialize(string path, int port)
        {
            this._port = port;
            this._path = path;

            _serverThread = new Thread(this.Listen);
            _serverThread.Start();
        }
    }
}
