using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSports.Framework.Models.Extensions;
using MySql.Data.MySqlClient;

namespace LSports.DVS.Framework.DataAccess.DbConnection
{
    public class DbConnection : IDisposable
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        //Constructor
        public DbConnection()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["gb_dvsstagingEntities"].ConnectionString;
            var startIndex = connectionString.IndexOf('"');
            var lastIndex = connectionString.LastIndexOf('"');
            connection = new MySqlConnection(connectionString.Substring(startIndex + 1, lastIndex - startIndex - 1));
            //connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["gb_dvsstagingEntities"].ToString());
        }

        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                if(connection.State == ConnectionState.Closed) {
                    connection.Open();
                }

                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        break;

                    case 1045:
                        break;
                }
                return false;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                return false;
            }
        }

        //Insert statement
        public void Execute(string query)
        {
            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);

                cmd.CommandTimeout = 1000;

                //ExecuteList command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
        }

        public void BulkExecute(string query)
        {
            //open connection
            if (this.OpenConnection() == true)
            {
                Execute(query);

                //close connection
                this.CloseConnection();
            }
        }

        //Bulk Insert statement
        public void BulkExecuteList(List<string> queries)
        {
            //open connection
            if (this.OpenConnection() == true)
            {
                var queriesToWork = queries.Split();

                foreach (var query in queriesToWork)
                {
                    ExecuteList(query);
                }

                //close connection
                this.CloseConnection();
            }
        }

        public void ExecuteList(List<string> query)
        {
            Execute(string.Join(";", query));
        }

        //Update statement
        public void Update()
        {
        }

        //Delete statement
        public void Delete()
        {
        }

        //Select statement
        public List<string>[] Select()
        {
            throw new NotImplementedException();
        }

        //Count statement
        public int Count()
        {
            throw new NotImplementedException();
        }

        //Backup
        public void Backup()
        {
        }

        //Restore
        public void Restore()
        {
        }

        public void Dispose()
        {
            connection.Close();
            connection.Dispose();
        }
    }
}
