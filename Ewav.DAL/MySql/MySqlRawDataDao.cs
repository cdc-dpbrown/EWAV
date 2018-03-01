/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MySqlRawDataDao.cs
 *  Namespace:  Ewav.DAL.MySqlLayer    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Data;
using System.Linq;
using Ewav.DAL.Interfaces;
using Ewav.DTO;
using MySql.Data.MySqlClient;

namespace Ewav.DAL.MySqlLayer
{
    public class MySqlRawDataDao : IRawDataDao
    {
        readonly string ConnectionString;
        readonly string DataViewName;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlRawDataDao" /> class.
        /// </summary>
        /// <param name="metaDataConnectionString">The meta data connection string.</param>
        /// <param name="metaDataViewName">Name of the meta data view.</param>
        public MySqlRawDataDao(string metaDataConnectionString, string metaDataViewName)
        {
            this.ConnectionString = metaDataConnectionString;
            this.DataViewName = metaDataViewName;
        }

        /// <summary>
        /// Gets the columns for datasource.
        /// </summary>
        /// <param name="dr">The dr.</param>
        /// <returns></returns>
        public DataTable GetColumnsForDatasource(DataRow[] dr)
        {
            string tableName = dr[0]["DatabaseObject"].ToString();
            string extDataConnectionString = Utilities.CreateConnectionString(DataBaseTypeEnum.MySQL, dr);

            string queryString;

            if (tableName.SQLTest())
            {
                queryString =
                    string.Format("SELECT *  FROM     ( {0}  )  as  f1    ", tableName);
            }
            else
            {
                queryString =
                    string.Format("SELECT *  FROM {0} ", tableName);
            }

            DataTable dt = new DataTable();
         

            MySqlConnection conn = new MySqlConnection(extDataConnectionString);

            // Create the Command and Parameter objects.
            MySqlCommand command = new MySqlCommand(queryString, conn);

            // create a new data adapter based on the specified query.
            MySqlDataAdapter da = new MySqlDataAdapter();
            //set the SelectCommand of the adapter
            da.SelectCommand = command;
            // create a new DataTable
            DataTable dtGet = new DataTable();
            //fill the DataTable
            da.Fill(dtGet);
            //return the DataTable

            dt = dtGet;

            return dt;
        }


        /// <summary>
        /// Gets the record  count.
        /// </summary>
        /// <param name="connStr">The conn STR.</param>
        /// <param name="TableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <returns></returns>
        public string GetRecordsCount(string connStr, string TableName, string whereClause)
        {
            DataSet ds;
            string returnString;

            if (whereClause.Length > 0)
            {

                whereClause = whereClause.Replace("= missing", " is NULL ").Replace("#", "'");

            }

            ds = MySqlUtilities.ExecuteSimpleDatsetFromStoredProc(connStr, "usp_read_records_count",
              TableName, TableName.SQLTest().ToString(), whereClause);

            returnString = string.Format("{0}", ds.Tables[0].Rows[0][0].ToString());

            ds = MySqlUtilities.ExecuteSimpleDatsetFromStoredProc(connStr, "usp_read_records_count",
              TableName, TableName.SQLTest().ToString(), " ");

            returnString = string.Format("{0}, {1} ", returnString, ds.Tables[0].Rows[0][0].ToString());


            return returnString;
        }

        /// <summary>
        /// Gets the records count.
        /// </summary>
        /// <param name="datasourceName">Name of the datasource.</param>
        /// <param name="TableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <returns></returns>
        public string xGetRecordsCount(string connStr, string TableName, string whereClause)
        {
            // Provide the query string with a parameter placeholder.
            string queryString;

            if (TableName.SQLTest())
            {
                queryString =
                    string.Format("SELECT *  FROM     ( {0}  )  as  f1    ", TableName);
            }
            else
            {
                queryString =
                    string.Format("SELECT *  FROM {0} ", TableName);
            }

            DataTable dtGet = this.getDataTable(queryString, connStr);

            string returnString = string.Format("{0},", dtGet.Rows.Count);

            if (TableName.SQLTest())
            {
                queryString =
                    string.Format("SELECT *  FROM     ( {0}  )  as  f1    ", TableName);
            }
            else
            {
                queryString =
                    string.Format("SELECT *  FROM {0} ", TableName);
            }

            whereClause = parseForMySql(whereClause);

            if (whereClause.Length > 0)
            {
                queryString = string.Format("{0} WHERE {1}", queryString, whereClause.Replace("= missing", " is NULL ").Replace("#", "'"));
            }

            dtGet = this.getDataTable(queryString, connStr);

            returnString = string.Format("{0}{1}", returnString, dtGet.Rows.Count.ToString());

            return returnString;
        }

        /// <summary>
        /// Gets the table2.
        /// </summary>
        /// <param name="Datasoucename">The datasoucename.</param>
        /// <param name="connStr">The conn STR.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public DataTable GetTable(string Datasoucename, string connStr, string tableName)
        {
            string queryString;

            if (tableName.SQLTest())
            {
                queryString =
                    string.Format("SELECT *  FROM     ( {0}  )  as  f1    ", tableName);
            }
            else
            {
                queryString =
                    string.Format("SELECT *  FROM {0} ", tableName);
            }

            string extDataConnectionString = connStr;

            DataTable dt = new DataTable();

            MySqlConnection conn = new MySqlConnection(extDataConnectionString);

            // Create the Command and Parameter objects.
            MySqlCommand command = new MySqlCommand(queryString, conn);

            // create a new data adapter based on the specified query.
            MySqlDataAdapter da = new MySqlDataAdapter();
            //set the SelectCommand of the adapter
            da.SelectCommand = command;
            // create a new DataTable
            DataTable dtGet = new DataTable();
            //fill the DataTable
            da.Fill(dtGet);
            //return the DataTable

            dt = dtGet;

            return dt;
        }

        /// <summary>
        /// Gets the top two table.
        /// </summary>
        /// <param name="Datasoucename">The datasoucename.</param>
        /// <param name="connStr">The conn STR.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public DataTable GetTopTwoTable(string Datasoucename, string connStr, string tableName)
        {
            string queryString;

            if (tableName.SQLTest())
            {
                queryString =
                    string.Format("SELECT *  FROM     ( {0}  )  as  f1    ", tableName);
            }
            else
            {
                queryString =
                    string.Format("SELECT *  FROM {0} ", tableName);
            }

            DataTable dt;

            dt = this.getDataTable(queryString, connStr);

            return dt;
        }

        /// <summary>
        /// Gets the total records count.
        /// </summary>
        /// <param name="externalConnStr">The external conn STR.</param>
        /// <param name="datasourceName">Name of the datasource.</param>
        /// <returns></returns>
        public string GetTotalRecordsCount(string externalConnStr, string tableName)
        {
            string queryString;

            if (tableName.SQLTest())
            {
                queryString =
                    string.Format("SELECT *  FROM     ( {0}  )  as  f1    ", tableName);
            }
            else
            {
                queryString =
                    string.Format("SELECT *  FROM {0} ", tableName);
            }

            DataTable dt;

            dt = this.getDataTable(queryString, externalConnStr);

            return dt.Rows.Count.ToString();
        }

        /// <summary>
        /// Gets the data table.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <param name="datasoucename">The datasoucename.</param>
        /// <param name="connStr">The conn STR.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        private DataTable getDataTable(string queryString, string connStr)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            // Create the Command and Parameter objects.
            MySqlCommand command = new MySqlCommand(queryString, conn);

            // create a new data adapter based on the specified query.
            MySqlDataAdapter da = new MySqlDataAdapter();
            //set the SelectCommand of the adapter
            da.SelectCommand = command;
            // create a new DataTable
            DataTable dtGet = new DataTable();
            //fill the DataTable
            da.Fill(dtGet);
            //return the DataTable

            return dtGet;
        }

        /// <summary>
        /// Parses for my SQL.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        /// <returns></returns>
        private string parseForMySql(string sqlStatement)
        {
            // replace 'identity' with 'auto_increment'
            sqlStatement = sqlStatement.Replace(" identity (1,1) ", " auto_increment ");

            // Replace escape chars
            sqlStatement = sqlStatement.Replace(CharLiterals.LEFT_SQUARE_BRACKET, CharLiterals.BACK_TICK);
            sqlStatement = sqlStatement.Replace(CharLiterals.RIGHT_SQUARE_BRACKET, CharLiterals.BACK_TICK);

            // Correct syntax for default
            sqlStatement = sqlStatement.Replace("default 1", "default '1'");

            // End the statement with a semicolon
            if (!sqlStatement.EndsWith(";"))
            {
            }
            {
                sqlStatement += ";";
            }

            return sqlStatement;
        }

        public int CountRecords(string connStr, string sqlOrTableName)
        {
            throw new NotImplementedException();
        }
    }
}