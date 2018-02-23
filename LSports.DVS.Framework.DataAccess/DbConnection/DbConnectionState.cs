using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSports.DVS.Framework.DataAccess.DbConnection
{
    class DbConnectionState
    {
        protected static DbConnection instance;

        protected DbConnectionState()
        {
        }

        public static DbConnection getInstance()
        {
            if (instance == null)
                instance = new DbConnection();

            return instance;
        }
    }
}
