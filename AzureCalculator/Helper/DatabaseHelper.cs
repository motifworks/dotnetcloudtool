using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;

namespace AzureCalculator.Helper
{
    public class DatabaseHelper
    {
        public static int MaxRetryCount = 3;
        public static string ConnectionString = @"Data Source=zw6gy4b0ha.database.windows.net;Initial Catalog=spdrive;User ID=motifindia@zw6gy4b0ha;Password=Motif123;Encrypt=true;Trusted_Connection=false;";

        public static List<T> Get<T>(String query, Object data)
        {
            SqlConnection conn = null;
            try
            {
                conn = CreateConnection();

                return conn.Query<T>(query, data).ToList();
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public static SqlConnection CreateConnection()
        {
            SqlConnection conn = null;
            int retryCount = 0;
            while (retryCount < MaxRetryCount)
            {
                try
                {
                    conn = new SqlConnection(ConnectionString);
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    break;
                }
                catch
                {
                    retryCount++;
                }
            }
            
            return conn;
        }
    }
}