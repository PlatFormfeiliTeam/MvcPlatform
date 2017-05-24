using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using System.Data; 
using System.Reflection;

namespace MvcPlatform.Common
{
    public class DBMgr
    {
        private static readonly string ConnectionString = ConfigurationManager.AppSettings["strconn"];

        public static DataSet GetDataSet(string sql)
        {
            DataSet ds = new DataSet();
            OracleConnection orclCon = null;
            try
            {
                using (orclCon = new OracleConnection(ConnectionString))
                {
                    DbCommand oc = orclCon.CreateCommand();
                    oc.CommandText = sql;
                    if (orclCon.State.ToString().Equals("Open"))
                    {
                        orclCon.Close();
                    }
                    orclCon.Open();
                    DbDataAdapter adapter = new OracleDataAdapter();
                    adapter.SelectCommand = oc;
                    adapter.Fill(ds);
                }
            }
            catch (Exception e)
            {
                //log.Error(e.Message + e.StackTrace);
            }
            finally
            {
                orclCon.Close();
            }
            return ds;
        }

        public static DataTable GetDataTable(string sql)
        {
            DataSet ds = new DataSet();
            OracleConnection orclCon = null;
            try
            {
                using (orclCon = new OracleConnection(ConnectionString))
                {
                    DbCommand oc = orclCon.CreateCommand();
                    oc.CommandText = sql;
                    if (orclCon.State.ToString().Equals("Open"))
                    {
                        orclCon.Close();
                    }
                    orclCon.Open();
                    DbDataAdapter adapter = new OracleDataAdapter();
                    adapter.SelectCommand = oc;
                    adapter.Fill(ds);
                }
            }
            catch (Exception e)
            {
                //log.Error(e.Message + e.StackTrace);
            }
            finally
            {
                orclCon.Close();
            }
            return ds.Tables[0];
        }

        public static int ExecuteNonQuery(string sql)
        {
            int retcount = -1;
            OracleConnection orclCon = null;
            using (orclCon = new OracleConnection(ConnectionString))
            {
                OracleCommand oc = new OracleCommand(sql, orclCon);                
                if (orclCon.State.ToString().Equals("Open"))
                {
                    orclCon.Close();
                }
                orclCon.Open();
                retcount = oc.ExecuteNonQuery();
                oc.Parameters.Clear();
            }
            orclCon.Close();
            return retcount;
        }

        public static int ExecuteNonQueryParm(string sql, OracleParameter[] parms)
        {
            int retcount = -1;
            OracleConnection orclCon = null;
            using (orclCon = new OracleConnection(ConnectionString))
            {
                OracleCommand oc = new OracleCommand();
                oc.Connection = orclCon;
                if (orclCon.State.ToString().Equals("Open"))
                {
                    orclCon.Close();
                }
                orclCon.Open();

                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = sql;
                oc.Parameters.AddRange(parms);

                retcount = oc.ExecuteNonQuery();
                oc.Parameters.Clear();
                oc.Dispose();
            }
            orclCon.Close();
            return retcount;
        }

        public static DataTable GetDataTableParm(string sql, OracleParameter[] parms)
        {
            DataSet ds = new DataSet();
            try
            {
                using (OracleConnection orclCon = new OracleConnection(ConnectionString))
                {
                    DbCommand oc = orclCon.CreateCommand();
                    oc.Connection = orclCon;
                    if (orclCon.State.ToString().Equals("Open"))
                    {
                        orclCon.Close();
                    }
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = sql;
                    oc.Parameters.AddRange(parms);
                    orclCon.Open();
                    DbDataAdapter adapter = new OracleDataAdapter();
                    adapter.SelectCommand = oc;
                    adapter.Fill(ds);
                }
            }
            catch (Exception e)
            {
                //log.Error(e.Message + e.StackTrace);
            }

            DataTable dt = null;
            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }
            return dt;
        }

    }
}