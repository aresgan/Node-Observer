using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;

namespace NodeObserver
{
    class Database
    {

        private SqlCeConnection conn = null;
        private SqlCeCommand cmd = null;

        public Database() {
            conn = new SqlCeConnection("Data Source = subject101.sdf; Password =''");
        }

        public Database(string dbString)
        {
            conn = new SqlCeConnection(dbString);
        }

        public void openDBConnection() {
            try
            {
                //conn = new SqlCeConnection("Data Source = subject101.sdf; Password =''");
                conn.Open();
                cmd = conn.CreateCommand();
            }
            catch (SqlCeException se)
            {
                System.Console.WriteLine("Error : " + se.Message.ToString());
            }
        }
        

        public void insertRecord(string day, string date, string time, string ip, string mac, string hostname, string status)
        {
            try
            {
                cmd.CommandText = "INSERT INTO Record (Day,Date,Time,IP,MAC,Hostname,status) Values ('" + day + "','" + date + "','" + time + "','" + ip + "','" + mac + "','" + hostname + "','" + status + "')";
                cmd.ExecuteNonQuery();
            }
            catch (SqlCeException se)
            {
                System.Console.WriteLine("Error : " + se.Message.ToString());
            }
        }

        public int GetLargestID()
        {
            int value = 0;
            try
            {
                cmd.CommandText = "SELECT MAX(ID) AS Expr1 FROM Record";

                string x = cmd.ExecuteScalar().ToString();
                if (x != "")
                {
                    value = int.Parse(x);
                }
                else
                {
                    value = 0;
                }
            }
            catch (SqlCeException se) {
                System.Console.WriteLine("Error : " + se.Message.ToString());
            }
            return value;
        }

        public void closeDBConnection() {
            try
            {
                cmd.Dispose();
                conn.Close();
            }
            catch (SqlCeException se)
            {
                System.Console.WriteLine("Error : " + se.Message.ToString());
            }
            finally {
                cmd.Dispose();
                conn.Close();
            }
        
        }

        public void DeleteAllRecord()
        {
            try
            {
               // conn = new SqlCeConnection("Data Source = subject101.sdf; Password =''");
                conn.Open();

                SqlCeCommand cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE Record";

                cmd.ExecuteNonQuery();
                cmd.Dispose();
                conn.Close();
            }
            finally
            {
                conn.Close();
            }

        }

        //public int GetLargestID() {

        //    int value = 0;
        //    try
        //    {
        //        //conn = new SqlCeConnection("Data Source = subject101.sdf; Password =''");
        //        conn.Open();

        //        SqlCeCommand cmd = conn.CreateCommand();
        //        cmd.CommandText = "SELECT MAX(ID) AS Expr1 FROM Record";

        //        string x = cmd.ExecuteScalar().ToString();
        //        if (x != "")
        //        {
        //            value = int.Parse(x);
        //        }
        //        else
        //        {
        //            value = 0;
        //        }
        //        cmd.Dispose();
        //        conn.Close();
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }
        //    return value;
        
        //}
        




    }
}
