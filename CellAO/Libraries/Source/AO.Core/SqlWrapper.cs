#region License
/*
Copyright (c) 2005-2012, CellAO Team

All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

#region Using...
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using AO.Core.Config;
using MySql.Data.MySqlClient;
using Npgsql;
//SQL Using Area...
//end SQL Using Area...
#endregion

namespace AO.Core
{
    /// <summary>
    /// MySQL wrapper for CellAO database
    /// </summary>
    public class SqlWrapper : IDisposable
    {
        /// <summary>
        /// Sqltye is set in the config.xml and can be 'MySql', 'MsSql' or 'PostgreSQL' as for now those are the 3 database types supported by cellao at this time.
        /// </summary>
        public string Sqltype = ConfigReadWrite.Instance.CurrentConfig.SQLType;

        /// <summary>
        /// shortcuts for checking sql type
        /// </summary>
        public Boolean ismysql;

        /// <summary>
        /// shortcuts for checking sql type
        /// </summary>
        public Boolean ismssql;

        /// <summary>
        /// shortcuts for checking sql type
        /// </summary>
        public Boolean isnpgsql;

        //Text file for Sql Errors

        #region Sql Setup...

        #region Mysql Area
        /// <summary>
        /// Opens a Public MySql Datareader named 'myreader' to be used in any class that has AO.Core in the using
        /// </summary>
        public MySqlDataReader myreader;

        /// <summary>
        /// Opens a Public Mysql Connection named 'mcc' to be used in any class that has AO.Core in the using.
        /// </summary>
        public MySqlConnection mcc;

        /// <summary>
        /// Opens a Public Mysql Command named 'mcom' to be used in any class that has AO.Core in using.
        /// </summary>
        public MySqlCommand mcom;
        #endregion

        #region MsSql Area
        /// <summary>
        /// Opens a Public MsSql Datareader named 'sqlreader' to be used in any class that has AO.Core in the using.
        /// </summary>
        public SqlDataReader sqlreader;

        /// <summary>
        /// Opens a Public MsSql Connection named 'sqlcc' to be used in any class that has AO.Core in the using.
        /// </summary>
        public SqlConnection sqlcc;

        /// <summary>
        /// Opens a Public MsSql Command named 'sqlcom' to be used in any class that has AO.Core in the using.
        /// </summary>
        public SqlCommand sqlcom;
        #endregion

        #region PostgreSQL Area
        /// <summary>
        /// Opens a public PostgreSQL Datareader named 'npgreader' to be used in any class that has AO.Core in the using.
        /// </summary>
        public NpgsqlDataReader npgreader;

        /// <summary>
        /// Opens a public PostgreSQL Connection named 'npgcc' to be used in any class that has AO.Core in the using.
        /// </summary>
        public NpgsqlConnection npgcc;

        /// <summary>
        /// Opens a Public PostgreSQL Command named ' npgcom' to be used in any class that has AO.Core in the using.
        /// </summary>
        public NpgsqlCommand npgcom;
        #endregion

        #endregion

        /// <summary>
        /// setting ismysql, ismssql and isnpgsql shortcut variables, so no more typos should occur in strings to check
        /// </summary>
        public SqlWrapper()
        {
            // determine which SQL engine we use
            ismysql = (Sqltype == "MySql");
            ismssql = (Sqltype == "MsSql");
            isnpgsql = (Sqltype == "PostgreSQL");
        }

        #region Connection String Setup
        //set Connect String..

        /// <summary>
        /// only needed once to read this
        /// </summary>
        private readonly string ConnectionString_MySQL = ConfigReadWrite.Instance.CurrentConfig.MysqlConnection;

        private readonly string ConnectionString_MSSQL = ConfigReadWrite.Instance.CurrentConfig.MsSqlConnection;
        private readonly string ConnectionString_PostGreSQL = ConfigReadWrite.Instance.CurrentConfig.PostgreConnection;
        #endregion

        #region Sql Count System
        /// <summary>
        /// Read Data into a DataTable object
        /// </summary>
        /// <param name="SqlQuery">Insert SQL Query here</param>
        /// <returns></returns>
        public Int32 SqlCount(string SqlQuery)
        {
            DataTable dt = this.ReadDatatable(SqlQuery);
            try
            {
                return (Int32) (Int64) dt.Rows[0][0];
            }
            catch
            {
                try
                {
                    return (Int32) dt.Rows[0][0];
                }
                catch
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }
        #endregion

        #region Sql Read System...
        /// <summary>
        /// Reads SQL Table. 
        /// Be sure to call sqlclose() after done reading!
        /// </summary>
        /// <param name="SqlQuery">Reads data from SQL DB, SqlQuery =  string SqlQuery = "SELECT * FROM `table` WHERE ID = "+ "'"+charID+"'";</param>
        public void SqlRead(string SqlQuery)
        {
            #region Mysql
            //MysqlRead: Create a SqlQuery to send to this wrapper for Reading the DB
            if (ismysql)
            {
                try
                {
                    mcom = new MySqlCommand(SqlQuery);
                    mcc = new MySqlConnection(ConnectionString_MySQL);
                    mcom.Connection = mcc;
                    mcom.CommandTimeout = 10000;
                    mcc.Open();
                    myreader = mcom.ExecuteReader();
                }
                catch (MySqlException me)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        MySqlLogger(me, SqlQuery);
                    }
                    else
                    {
                        throw me;
                    }
                }
            }
                #endregion
                #region MSSQL
            else if (ismssql)
            {
                try
                {
                    sqlcc = new SqlConnection(ConnectionString_MSSQL);
                    sqlcom = new SqlCommand(SqlQuery);
                    sqlcom.Connection = sqlcc;
                    sqlcom.CommandTimeout = 10000;
                    if (sqlcc.State == 0)
                    {
                        sqlcc.Open();
                    }
                    sqlreader = sqlcom.ExecuteReader();
                }
                catch (SqlException se)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        MsSqlLogger(se, SqlQuery);
                    }
                    else
                    {
                        throw se;
                    }
                }
            }
                #endregion
                #region Postgresql
            else if (isnpgsql)
            {
                try
                {
                    npgcc = new NpgsqlConnection(ConnectionString_PostGreSQL);
                    npgcom = new NpgsqlCommand(SqlQuery);
                    npgcom.Connection = npgcc;
                    npgcom.CommandTimeout = 10000;
                    if (npgcc.State == 0)
                    {
                        npgcc.Open();
                    }
                    npgreader = npgcom.ExecuteReader();
                }
                catch (NpgsqlException ne)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        PostgressqlLogger(ne, SqlQuery);
                    }
                    else
                    {
                        throw ne;
                    }
                }
            }
            #endregion
        }
        #endregion

        #region Sql Update System...
        /// <summary>
        /// Updates Data in SQL Table
        /// </summary>
        /// <param name="SqlQuery">Updates data in SQL DB, SqlQuery = string SqlQuery = "UPDATE `table` SET `collum` = "+data+" WHERE ID = "'"+charID+"'";</param>
        public int SqlUpdate(string SqlQuery)
        {
            #region Mysql
            if (ismysql)
            {
                try
                {
                    mcc = new MySqlConnection(ConnectionString_MySQL);
                    mcom = new MySqlCommand(SqlQuery);
                    mcom.Connection = mcc;
                    mcom.CommandTimeout = 10000;
                    if (mcc.State == 0)
                    {
                        mcc.Open();
                    }
                    return mcom.ExecuteNonQuery();
                }
                catch (MySqlException me)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        MySqlLogger(me, SqlQuery);
                    }
                    else
                    {
                        throw me;
                    }
                }
            }
                #endregion
                #region MSSQL
            else if (ismssql)
            {
                try
                {
                    sqlcc = new SqlConnection(ConnectionString_MSSQL);
                    sqlcom = new SqlCommand(SqlQuery);
                    sqlcom.Connection = sqlcc;
                    sqlcom.CommandTimeout = 10000;
                    if (sqlcc.State == 0)
                    {
                        sqlcc.Open();
                    }
                    return sqlcom.ExecuteNonQuery();
                }
                catch (SqlException se)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        MsSqlLogger(se, SqlQuery);
                    }
                    else
                    {
                        throw se;
                    }
                }
            }
                #endregion
                #region Postgresql
            else if (isnpgsql)
            {
                try
                {
                    npgcc = new NpgsqlConnection(ConnectionString_PostGreSQL);
                    npgcom = new NpgsqlCommand(SqlQuery);
                    npgcom.Connection = npgcc;
                    npgcom.CommandTimeout = 10000;
                    if (npgcc.State == 0)
                    {
                        npgcc.Open();
                    }
                    return npgcom.ExecuteNonQuery();
                }
                catch (NpgsqlException ne)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        PostgressqlLogger(ne, SqlQuery);
                    }
                    else
                    {
                        throw ne;
                    }
                }
            }
            #endregion

            sqlclose();
            return 0;
        }
        #endregion

        #region Sql Insert System...
        /// <summary>
        /// Inserts data to SQL DB
        /// </summary>
        /// <param name="SqlQuery">Insert data into the SQL db, SqlQuery = INSERT INTO `dbname`  VALUES (`item1_value`, `item2_value`, `etc`)  </param>
        public void SqlInsert(string SqlQuery)
        {
            #region Mysql
            if (ismysql)
            {
                try
                {
                    mcc = new MySqlConnection(ConnectionString_MySQL);
                    mcom = new MySqlCommand(SqlQuery);
                    mcom.Connection = mcc;
                    mcom.CommandTimeout = 10000;
                    if (mcc.State == 0)
                    {
                        mcc.Open();
                    }
                    mcom.ExecuteNonQuery();
                }
                catch (MySqlException me)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        MySqlLogger(me, SqlQuery);
                    }
                    else
                    {
                        throw me;
                    }
                }
            }
                #endregion
                #region MSSQL
            else if (ismssql)
            {
                try
                {
                    sqlcc = new SqlConnection(ConnectionString_MSSQL);
                    sqlcom = new SqlCommand(SqlQuery);
                    sqlcom.Connection = sqlcc;
                    sqlcom.CommandTimeout = 10000;
                    if (sqlcc.State == 0)
                    {
                        sqlcc.Open();
                    }
                    sqlcom.ExecuteNonQuery();
                }
                catch (SqlException se)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        MsSqlLogger(se, SqlQuery);
                    }
                    else
                    {
                        throw se;
                    }
                }
            }
                #endregion
                #region Postgresql
            else if (isnpgsql)
            {
                try
                {
                    npgcc = new NpgsqlConnection(ConnectionString_PostGreSQL);
                    npgcom = new NpgsqlCommand(SqlQuery);
                    npgcom.Connection = npgcc;
                    npgcom.CommandTimeout = 10000;
                    if (npgcc.State == 0)
                    {
                        npgcc.Open();
                    }
                    npgcom.ExecuteNonQuery();
                }
                catch (NpgsqlException ne)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        PostgressqlLogger(ne, SqlQuery);
                    }
                    else
                    {
                        throw ne;
                    }
                }
            }
            #endregion

            sqlclose();
        }
        #endregion

        #region Sql Delete System...
        /// <summary>
        /// Deletes data from the SQL Table
        /// </summary>
        /// <param name="SqlQuery">SQL Query to execute DELETE from SQL DB, SqlQuery = DELETE FROM `database` WHERE (`Field` = {value})</param>
        public void SqlDelete(string SqlQuery)
        {
            #region Mysql
            if (ismysql)
            {
                try
                {
                    mcc = new MySqlConnection(ConnectionString_MySQL);
                    mcom = new MySqlCommand(SqlQuery);
                    mcom.Connection = mcc;
                    mcom.CommandTimeout = 10000;
                    if (mcc.State == 0)
                    {
                        mcc.Open();
                    }
                    mcom.ExecuteNonQuery();
                }
                catch (MySqlException me)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        MySqlLogger(me, SqlQuery);
                    }
                    else
                    {
                        throw me;
                    }
                }
            }
                #endregion
                #region MSSQL
            else if (ismssql)
            {
                try
                {
                    sqlcc = new SqlConnection(ConnectionString_MSSQL);
                    sqlcom = new SqlCommand(SqlQuery);
                    sqlcom.Connection = sqlcc;
                    sqlcom.CommandTimeout = 10000;
                    if (sqlcc.State == 0)
                    {
                        sqlcc.Open();
                    }
                    sqlcom.ExecuteNonQuery();
                }
                catch (SqlException se)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        MsSqlLogger(se, SqlQuery);
                    }
                    else
                    {
                        throw se;
                    }
                }
            }
                #endregion
                #region Postgresql
            else if (isnpgsql)
            {
                try
                {
                    npgcc = new NpgsqlConnection(ConnectionString_PostGreSQL);
                    npgcom = new NpgsqlCommand(SqlQuery);
                    npgcom.Connection = npgcc;
                    npgcom.CommandTimeout = 10000;
                    if (npgcc.State == 0)
                    {
                        npgcc.Open();
                    }
                    npgcom.ExecuteNonQuery();
                }
                catch (NpgsqlException ne)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        PostgressqlLogger(ne, SqlQuery);
                    }
                    else
                    {
                        throw ne;
                    }
                }
            }
            #endregion

            sqlclose();
        }
        #endregion

        #region SQL read into DataTable
        /// <summary>
        /// Read Data into a DataTable object
        /// </summary>
        /// <param name="sqlQuery">Insert SQL Query here</param>
        /// <returns></returns>
        public DataTable ReadDatatable(string sqlQuery)
        {
            DataSet ds = new DataSet();
            try
            {
                if (ismysql)
                {
                    mcc = new MySqlConnection(ConnectionString_MySQL);
                    MySqlDataAdapter mda = new MySqlDataAdapter(sqlQuery, mcc);
                    mda.Fill(ds);
                }
                if (ismssql)
                {
                    sqlcc = new SqlConnection(ConnectionString_MSSQL);
                    SqlDataAdapter mda = new SqlDataAdapter(sqlQuery, sqlcc);
                    mda.Fill(ds);
                }
                if (isnpgsql)
                {
                    npgcc = new NpgsqlConnection(ConnectionString_PostGreSQL);
                    NpgsqlDataAdapter mda = new NpgsqlDataAdapter(sqlQuery, npgcc);
                    mda.Fill(ds);
                }
            }
            catch (MySqlException me)
            {
                if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                {
                    MySqlLogger(me, sqlQuery);
                }
                else
                {
                    throw me;
                }
            }
            catch (SqlException me)
            {
                if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                {
                    MsSqlLogger(me, sqlQuery);
                }
                else
                {
                    throw me;
                }
            }
            catch (NpgsqlException me)
            {
                if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                {
                    PostgressqlLogger(me, sqlQuery);
                }
                else
                {
                    throw me;
                }
            }
            sqlclose();
            if (ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;
            }
        }
        #endregion

        //Loggers

        #region MysqlLogger
        private void MySqlLogger(MySqlException me, string SqlQuery)
        {
            FileInfo t = new FileInfo("SqlError.log");
            if (t.Exists)
            {
                TextWriter tex = t.AppendText();
                tex.WriteLine("Date/Time: " + DateTime.Now.ToString());
                tex.WriteLine(" ");
                tex.WriteLine("Sql String: " + SqlQuery);
                tex.WriteLine(" ");
                tex.WriteLine("Sql Error: ");
                tex.WriteLine(me);
                tex.Write(tex.NewLine);
                tex.Flush();
                tex.Close();
                tex = null;
                t = null;
            }
            else
            {
                StreamWriter sw = t.CreateText();
                sw.WriteLine("Date/Time: " + DateTime.Now.ToString());
                sw.WriteLine(" ");
                sw.WriteLine("Sql String: " + SqlQuery);
                sw.WriteLine(" ");
                sw.WriteLine("Sql Error: ");
                sw.WriteLine(me);
                sw.Write(sw.NewLine);
                sw.Flush();
                sw.Close();
                sw = null;
                t = null;
            }
        }
        #endregion

        #region MSSQLLogger
        private void MsSqlLogger(SqlException se, string SqlQuery)
        {
            FileInfo t = new FileInfo("SqlError.log");
            if (t.Exists)
            {
                TextWriter tex = new StreamWriter(t.OpenWrite());
                tex.WriteLine("Date/Time: " + DateTime.Now.ToString());
                tex.WriteLine(" ");
                tex.WriteLine("Sql String: " + SqlQuery);
                tex.WriteLine(" ");
                tex.WriteLine("Sql Error: ");
                tex.WriteLine(se);
                tex.Write(tex.NewLine);
                tex.Flush();
                tex.Close();
                tex = null;
                t = null;
            }
            else
            {
                StreamWriter sw = t.CreateText();
                sw.WriteLine("Date/Time: " + DateTime.Now.ToString());
                sw.WriteLine(" ");
                sw.WriteLine("Sql String: " + SqlQuery);
                sw.WriteLine(" ");
                sw.WriteLine("Sql Error: ");
                sw.WriteLine(se);
                sw.Write(sw.NewLine);
                sw.Flush();
                sw.Close();
                sw = null;
                t = null;
            }
        }
        #endregion

        #region PostgresqlLogger
        private void PostgressqlLogger(NpgsqlException ne, string SqlQuery)
        {
            FileInfo t = new FileInfo("SqlError.log");
            if (t.Exists)
            {
                TextWriter tex = new StreamWriter(t.OpenWrite());
                tex.WriteLine("Date/Time: " + DateTime.Now.ToString());
                tex.WriteLine(" ");
                tex.WriteLine("Sql String: " + SqlQuery);
                tex.WriteLine(" ");
                tex.WriteLine("Sql Error: ");
                tex.WriteLine(ne);
                tex.Write(tex.NewLine);
                tex.Flush();
                tex.Close();
                tex = null;
                t = null;
            }
            else
            {
                StreamWriter sw = t.CreateText();
                sw.WriteLine("Date/Time: " + DateTime.Now.ToString());
                sw.WriteLine(" ");
                sw.WriteLine("Sql String: " + SqlQuery);
                sw.WriteLine(" ");
                sw.WriteLine("Sql Error: ");
                sw.WriteLine(ne);
                sw.Write(sw.NewLine);
                sw.Flush();
                sw.Close();
                sw = null;
                t = null;
            }
        }
        #endregion

        #region SQL closer
        /// <summary>
        /// sqlclose
        /// </summary>
        public void sqlclose()
        {
            if (ismysql)
            {
                if (mcc != null)
                {
                    mcc.Close();
                }
            }
            if (ismssql)
            {
                try
                {
                    sqlcc.Close();
                }
                catch
                {
                }
                try
                {
                    sqlreader.Close();
                }
                catch
                {
                }
            }
            if (isnpgsql)
            {
                try
                {
                    npgcc.Close();
                }
                catch
                {
                }
                try
                {
                    npgreader.Close();
                }
                catch
                {
                }
            }
            Dispose();
        }
        #endregion

        #region SQL connection/reader/command disposer
        /// <summary>
        /// Disposer
        /// </summary>
        public void Dispose()
        {
            if (mcom != null)
            {
                mcom.Dispose();
            }
            if (myreader != null)
            {
                myreader.Dispose();
            }
            if (ismysql)
            {
                try
                {
                    mcc.Close();
                    mcc.Dispose();
                }
                catch
                {
                }
            }
            if (ismssql)
            {
                try
                {
                    sqlcc.Close();
                    sqlcc.Dispose();
                }
                catch
                {
                }
            }
            if (isnpgsql)
            {
                try
                {
                    npgcc.Close();
                    npgcc.Dispose();
                }
                catch
                {
                }
            }
        }
        #endregion

        #region SQL Checks
        /// <summary>
        /// Tests SQL Connection/Database setup/Rights to create tables
        /// </summary>
        /// <returns>DBCheckCodes</returns>
        public DBCheckCodes SQLCheck()
        {
            string dbname = GetDBName();
            if (ismysql)
            {
                try
                {
                    mcc = new MySqlConnection(ConnectionString_MySQL);
                    mcom = new MySqlCommand("Use " + dbname);
                    mcom.Connection = mcc;
                    mcom.CommandTimeout = 10000;
                    if (mcc.State == 0)
                    {
                        mcc.Open();
                    }
                    // Test if database can be used
                    mcom.ExecuteNonQuery();

                    // Test if table can be created
                    mcom.CommandText = "CREATE TABLE " + dbname + ".TEMPDBTEMP (t int);";
                    mcom.ExecuteNonQuery();

                    // Drop the table again
                    mcom.CommandText = "DROP TABLE " + dbname + ".TEMPDBTEMP";
                    mcom.ExecuteNonQuery();
                }
                catch (MySqlException myex)
                {
                    lasterrorcode = myex.Number;
                    lasterrormessage = myex.Message;
                    switch (myex.Number)
                    {
                        case 1044:
                            return DBCheckCodes.DBC_NoRightsToAccessDatabase;
                        case 1049:
                            return DBCheckCodes.DBC_DatabaseDoesNotExist;
                        case 1142:
                            return DBCheckCodes.DBC_NotEnoughRightsForTableAction;
                        default:
                            return DBCheckCodes.DBC_Somethingwentwrong;
                    }
                }
            }
            if (ismssql)
            {
                try
                {
                    sqlcc = new SqlConnection(ConnectionString_MSSQL);
                    sqlcom = new SqlCommand("use " + dbname);
                    sqlcom.Connection = sqlcc;
                    sqlcom.CommandTimeout = 10000;
                    if (sqlcc.State == 0)
                    {
                        sqlcc.Open();
                    }
                    sqlcom.ExecuteNonQuery();

                    sqlcom.CommandText = "CREATE TABLE " + dbname + ".TEMPDBTEMP (`c` INTEGER )";
                    sqlcom.ExecuteNonQuery();

                    sqlcom.CommandText = "DROP TABLE " + dbname + ".TEMPDBTEMP";
                    sqlcom.ExecuteNonQuery();
                }
                catch (SqlException se)
                {
                    lasterrormessage = se.Message;
                    lasterrorcode = se.Number;
                    return DBCheckCodes.DBC_Somethingwentwrong;
                }
            }
            if (isnpgsql)
            {
                try
                {
                    npgcc = new NpgsqlConnection(ConnectionString_PostGreSQL);

                    npgcom = new NpgsqlCommand("CREATE TABLE " + dbname + ".TEMPDBTEMP (c integer)");
                    npgcom.Connection = npgcc;
                    npgcom.CommandTimeout = 10000;
                    if (npgcc.State == 0)
                    {
                        npgcc.Open();
                    }
                    npgcom.ExecuteNonQuery();

                    npgcom.CommandText = "DROP TABLE " + dbname + ".TEMPDBTEMP";
                    npgcom.ExecuteNonQuery();
                }
                catch (NpgsqlException ne)
                {
                    lasterrorcode = ne.ErrorCode;
                    lasterrormessage = ne.Message;
                    return DBCheckCodes.DBC_Somethingwentwrong;
                }
            }
            return DBCheckCodes.DBC_ok;
        }


        /// <summary>
        /// Our Database Check Codes
        /// </summary>
        public enum DBCheckCodes
        {
            /// <summary>
            /// All fine
            /// </summary>
            DBC_ok,

            /// <summary>
            /// Database does not exists
            /// </summary>
            DBC_DatabaseDoesNotExist,

            /// <summary>
            /// No rights to use the database
            /// </summary>
            DBC_NoRightsToAccessDatabase,

            /// <summary>
            /// User has no rights to create a table
            /// </summary>
            DBC_NotEnoughRightsForTableAction,

            /// <summary>
            /// All other errors
            /// </summary>
            DBC_Somethingwentwrong
        };

        /// <summary>
        /// Last error message from failed sql query
        /// </summary>
        public string lasterrormessage = "";

        /// <summary>
        /// Last error code from failed sql query
        /// </summary>
        public int lasterrorcode;
        #endregion

        #region GetDBName (extract database name from configuration)
        private string GetDBName()
        {
            string dbn = "";
            if (ismysql)
            {
                MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(ConnectionString_MySQL);
                dbn = builder.Database;
            }
            if (ismssql)
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString_MSSQL);
                dbn = builder.DataSource;
            }
            if (isnpgsql)
            {
                NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(ConnectionString_PostGreSQL);
                dbn = builder.Database;
            }
            return dbn;
        }
        #endregion

        #region Check Database
        /// <summary>
        /// Check our tables and create/fill them if they don't exist
        /// </summary>
        public void CheckDBs()
        {
            SqlWrapper ms = new SqlWrapper();
            List<string> tablelist = new List<string>();
            List<string> tabletodo = new List<string>();
            bool allok = true;
            // ToDo: check if database exists and create it if not (parsing the connection string)
            if (ismssql)
            {
                ms.SqlRead("SELECT table_name FROM INFORMATION_SCHEMA.TABLES;");
            }
            else if (isnpgsql)
            {
                ms.SqlRead("SELECT table_name FROM information_schema.tables;");
            }
            else if (ismysql)
            {
                ms.SqlRead("show Tables");
            }
            if (ms.myreader.HasRows)
            {
                while (ms.myreader.Read())
                {
                    tablelist.Add(ms.myreader.GetString(0));
                }
            }
            else
            {
                allok = false;
            }
            ms.sqlclose();

            string[] sqlfiles = Directory.GetFiles("SQLTables");
            bool isin;
            foreach (string s in sqlfiles)
            {
                isin = false;
                foreach (string table in tablelist)
                {
                    if (s.ToLower() == Path.Combine("SQLTables", table + ".sql").ToLower())
                    {
                        isin = true;
                        break;
                    }
                }
                if (!isin)
                {
                    tabletodo.Add(s);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Table " + s + " doesn't exist.");
                    allok = false;
                }
            }

            if (!allok)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("SQL Tables are not complete. Should they be created? (Y/N) ");
                string answer = Console.ReadLine();
                string sqlquery;
                if (answer.ToLower() == "y")
                {
                    foreach (string todo in tabletodo)
                    {
                        long filesize = new FileInfo(todo).Length;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("Table " + todo.PadRight(67) + "[  0%]");

                        if (filesize > 10000)
                        {
                            string[] queries = File.ReadAllLines(todo);
                            int c = 0;
                            sqlquery = "";
                            string lastpercent = "0";
                            while (c < queries.Length)
                            {
                                if (queries[c].IndexOf("INSERT INTO") == -1)
                                {
                                    sqlquery += queries[c] + "\n";
                                }
                                else
                                {
                                    c--;
                                    break;
                                }
                                c++;
                            }
                            ms.SqlInsert(sqlquery);
                            c++;
                            string buf1 = "";
                            while (c < queries.Length)
                            {
                                if (queries[c].ToLower().Substring(0, 11) == "insert into")
                                    break;
                                c++;
                            }

                            if (c < queries.Length)
                            {
                                buf1 = queries[c].Substring(0, queries[c].ToLower().IndexOf("values"));
                                buf1 = buf1 + "VALUES ";
                                StringBuilder Buffer = new StringBuilder(0, 1*1024*1024);
                                while (c < queries.Length)
                                {
                                    if (Buffer.Length == 0)
                                    {
                                        Buffer.Append(buf1);
                                    }
                                    string part = "";
                                    while (c < queries.Length)
                                    {
                                        if (queries[c].Trim() != "")
                                        {
                                            part = queries[c].Substring(queries[c].ToLower().IndexOf("values"));
                                            part = part.Substring(part.IndexOf("(")); // from '(' to end
                                            part = part.Substring(0, part.Length - 1); // Remove ';'
                                            if (Buffer.Length + 1 + part.Length > 1024*1000)
                                            {
                                                Buffer.Remove(Buffer.Length - 2, 2);
                                                Buffer.Append(";");
                                                ms.SqlInsert(Buffer.ToString());
                                                Buffer.Clear();
                                                Buffer.Append(buf1);
                                                string lp2 =
                                                    Convert.ToInt32(Math.Floor((double) c/queries.Length*100)).ToString();
                                                if (lp2 != lastpercent)
                                                {
                                                    Console.Write("\rTable " + todo.PadRight(67) + "[" + lp2.PadLeft(3) +
                                                                  "%]");
                                                    lastpercent = lp2;
                                                }
                                            }
                                            Buffer.Append(part + ", ");
                                        }
                                        c++;
                                    }

                                    Buffer.Remove(Buffer.Length - 2, 2);
                                    Buffer.Append(";");
                                    ms.SqlInsert(Buffer.ToString());
                                    Buffer.Clear();
                                    string lp = Convert.ToInt32(Math.Floor((double) c/queries.Length*100)).ToString();
                                    if (lp != lastpercent)
                                    {
                                        Console.Write("\rTable " + todo.PadRight(67) + "[" + lp.PadLeft(3) + "%]");
                                        lastpercent = lp;
                                    }
                                }
                            }
                            else
                            {
                                Console.Write("\rTable " + todo.PadRight(67) + "[100%]");
                            }
                        }
                        else
                        {
                            sqlquery = File.ReadAllText(todo);
                            ms.SqlInsert(sqlquery);
                            Console.Write("\rTable " + todo.PadRight(67) + "[100%]");
                        }
                        Console.WriteLine();
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Database is fine.");
        }
        #endregion
    }
}