    #region License
/*
Copyright (c) 2005-2011, CellAO Team

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
using System.Text;
using AO.Core.Config;
using System.Data;
using System.IO;
using System.Reflection;
using System.Linq;
//SQL Using Area...
using System.Data.Odbc;
using System.Data.SqlClient;
using Npgsql;
using NpgsqlTypes;
using MySql.Data.MySqlClient;
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
        private string ConnectionString_MySQL = ConfigReadWrite.Instance.CurrentConfig.MysqlConnection;
        private string ConnectionString_MSSQL = ConfigReadWrite.Instance.CurrentConfig.MsSqlConnection;
        private string ConnectionString_PostGreSQL = ConfigReadWrite.Instance.CurrentConfig.PostgreConnection;

        #endregion

        #region Sql Count System
        /// <summary>
        /// Read Data into a DataTable object
        /// </summary>
        /// <param name="SqlQuery">Insert SQL Query here</param>
        /// <returns></returns>
        public Int32 SqlCount(string SqlQuery)
        {
            DataTable dt = ReadDT(SqlQuery);
            try
            {
                return (Int32)(Int64)dt.Rows[0][0];
            }
            catch
            {
                try
                {
                    return (Int32)dt.Rows[0][0];
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
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog == true)
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
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog == true)
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
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog == true)
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
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog == true)
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
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog == true)
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
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog == true)
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
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog == true)
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
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog == true)
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
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog == true)
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
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog == true)
                    {
                        MySqlLogger(me, SqlQuery);
                    }
                    else { 
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
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog == true)
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
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog == true)
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
        /// <param name="SqlQuery">Insert SQL Query here</param>
        /// <returns></returns>
        public DataTable ReadDT(string SqlQuery)
        {
            DataSet ds = new DataSet();
            try
            {
                if (ismysql)
                {
                    mcc = new MySqlConnection(ConnectionString_MySQL);
                    MySqlDataAdapter mda = new MySqlDataAdapter(SqlQuery, mcc);
                    mda.Fill(ds);
                }
                if (ismssql)
                {
                    sqlcc = new SqlConnection(ConnectionString_MSSQL);
                    SqlDataAdapter mda = new SqlDataAdapter(SqlQuery, sqlcc);
                    mda.Fill(ds);
                }
                if (isnpgsql)
                {
                    npgcc = new NpgsqlConnection(ConnectionString_PostGreSQL);
                    NpgsqlDataAdapter mda = new NpgsqlDataAdapter(SqlQuery, npgcc);
                    mda.Fill(ds);
                }
            }
            catch (MySqlException me)
            {
                if (ConfigReadWrite.Instance.CurrentConfig.SqlLog == true)
                {
                    MySqlLogger(me, SqlQuery);
                }
                else
                {
                    throw me;
                }
            }
            catch (SqlException me)
            {
                if (ConfigReadWrite.Instance.CurrentConfig.SqlLog == true)
                {
                    MsSqlLogger(me, SqlQuery);
                }
                else
                {
                    throw me;
                }
            }
            catch (NpgsqlException me)
            {
                if (ConfigReadWrite.Instance.CurrentConfig.SqlLog == true)
                {
                    PostgressqlLogger(me, SqlQuery);
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
            if (t.Exists == true)
            {
                TextWriter tex = new StreamWriter(t.OpenWrite());
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
            if (t.Exists == true)
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
            if (t.Exists == true)
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
                try
                {
                    mcc.Close();
                }
                catch { }
                try
                {
                    myreader.Close();
                }
                catch { }
            }
            if (ismssql)
            {
                try
                {
                    sqlcc.Close();
                }
                catch { }
                try
                {
                    sqlreader.Close();
                }
                catch { }
            }
            if (isnpgsql)
            {
                try
                {
                    npgcc.Close();
                }
                catch { }
                try
                {
                    npgreader.Close();
                }
                catch { }
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
            try
            {
                mcom.Dispose();
            }
            catch { }
            try
            {
                myreader.Dispose();
            }
            catch { }
            if (ismysql)
            {
                try
                {
                    mcc.Close();
                    mcc.Dispose();
                }
                catch { }
            }
            if (ismssql)
            {
                try
                {
                    sqlcc.Close();
                    sqlcc.Dispose();
                }
                catch { }
            }
            if (isnpgsql)
            {
                try
                {
                    npgcc.Close();
                    npgcc.Dispose();
                }
                catch { }
            }
        }
        #endregion
    }
}
