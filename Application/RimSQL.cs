using System;
using System.Data;
using System.Data.SQLite;

namespace FTBAPISERVER.Application
{
    public class RimSQL
    {
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter DB;
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();

        public RimSQL()
        {
        }

        private void SetConnection()
        {
            sql_con = new SQLiteConnection("Data Source=|DataDirectory|FTBToken.db;Version=3;Compress=False;synchronous=OFF;");
            //("Data Source=Rimkus.db;Version=3;New=False;Compress=True;");
        }

        public string Doscalar(string sqlstring)
        {
            //cmd.CommandText = "SELECT COUNT(*) FROM dbo.region";
            //Int32 count = (Int32)cmd.ExecuteScalar();
            try
            {
                SetConnection();
                sql_con.Open();
                sql_cmd = sql_con.CreateCommand();
                sql_cmd.CommandText = sqlstring;
                var result = sql_cmd.ExecuteScalar();
                sql_con.Close();
                if (result == null)
                { return ""; }
                else
                { return result.ToString(); };
            }
            catch
            {
                if (sql_con.State == ConnectionState.Open)
                    sql_con.Close();

                return "";
            }
        }

        public DataSet DbDataSet(string sqlstring)
        {
            try
            {
                SetConnection();
                DataSet ds;
                sql_con.Open();
                sql_cmd = sql_con.CreateCommand();
                sql_cmd.CommandText = sqlstring;
                var result = sql_cmd.ExecuteReader();//    .ExecuteScalar();
                DataTable dt = new DataTable();
                dt.Load(result);
                ds = new DataSet();
                ds.Tables.Add(dt);
                sql_con.Close();
                if (result != null)
                { return ds; }
                else
                { return new DataSet(); };
            }
            catch
            {
                if (sql_con.State == ConnectionState.Open)
                    sql_con.Close();

                return new DataSet();
            }

        }
        public void ExecuteNonQuery(string txtQuery)
        {
            try
            {
                SetConnection();
                sql_con.Open();
                sql_cmd = sql_con.CreateCommand();
                sql_cmd.CommandText = txtQuery;
                sql_cmd.ExecuteNonQuery();
                sql_con.Close();
            }
            catch (Exception exx)
            {
                if (sql_con.State == ConnectionState.Open)
                    sql_con.Close(); 
                
            }
        }
    }
}