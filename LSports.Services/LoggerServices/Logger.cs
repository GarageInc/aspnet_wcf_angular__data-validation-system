using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace LSports.Services.LoggerServices
{
    public class InfoException : Exception
    {
        public InfoException()
        {
        }

        public InfoException(string message)
            : base(message)
        {
        }

        public InfoException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }

    public class Logger
    {
        protected static Logger instance = null;

        private readonly ILog log4net = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static Logger GetInstance()
        {
            if (instance == null)
            {
                instance = new Logger();
            }

            return instance;
        }

        public void Info(string message)
        {
            log4net.Info(message);
        }
        
        public void Debug(string message)
        {
            log4net.Debug(message);
        }

        public void Error(string message)
        {
            log4net.Error(message);
        }
    }
}
