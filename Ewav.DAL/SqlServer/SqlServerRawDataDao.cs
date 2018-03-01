/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       SqlServerRawDataDao.cs
 *  Namespace:  Ewav.DAL.SqlServer    
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
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Ewav.DTO;

// -----------------------------------------------------------------------
// <copyright file="$safeitemrootname$.cs" company="$registeredorganization$">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
namespace Ewav.DAL.SqlServer
{
    public class SqlServerRawDataDao : IRawDataDao
    {
        private readonly string MetaDataConnectionString;
        private readonly string MetaDataViewName;
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerRawDataDao" /> class.
        /// </summary>
        /// <param name="ConnectionString">The meta data connection string.</param>
        /// <param name="MetaDataViewName">Name of the meta data view.</param>
        public SqlServerRawDataDao(string MetaDataConnectionString, string MetaDataViewName)
        {
            // TODO: Complete member initialization
            this.MetaDataConnectionString = MetaDataConnectionString;
            this.MetaDataViewName = MetaDataViewName;
        }

        /// <summary>
        /// Gets the columns for datasource.
        /// </summary>
        /// <param name="dr">The dr.</param>
        /// <returns></returns>
        public DataTable GetColumnsForDatasource(DataRow[] dr)
        {
            Security.Cryptography cy = new Security.Cryptography();

            string tableName = cy.Decrypt(dr[0]["DatabaseObject"].ToString());
            string externalConnectionString = Utilities.CreateConnectionString(DataBaseTypeEnum.SQLServer, dr);

            string query = "";

            //  if (tableName.ToLower().Contains("select"))



            if (tableName.SQLTest())
            {
                query = string.Format("SELECT TOP 1 * FROM  ( {0}  ) as table1 ", tableName);
            }
            else
            {
                query = string.Format("SELECT TOP 1 * FROM {0}", tableName);
            }

            DataTable dt = new DataTable();

            SqlDatabase sqd = new SqlDatabase(externalConnectionString);

            //dt = sqd.ExecuteDataSet("usp_read_columns_for_datasource", tableName, tableName.SQLTest()).Tables[0];
            dt = sqd.ExecuteDataSet(CommandType.Text, query).Tables[0];

            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlTableName"></param>
        /// <returns></returns>
        public int CountRecords(string connStr, string sqlOrTableName)
        {
            SqlDatabase db = new SqlDatabase(connStr);    

            string sqlQuery = "";

            if (sqlOrTableName.LastIndexOf(" ") > 0)
            {
                sqlQuery = "Select Count(*) from (" + sqlOrTableName + ") as F1";
            }
            else
            {
                sqlQuery = "Select Count(*) from " + sqlOrTableName + "";
            }


            return Convert.ToInt32(db.ExecuteScalar(CommandType.Text, sqlQuery));

        }

        /// <summary>
        /// Gets the records count.
        /// </summary>
        /// <param name="connStr">The conn STR.</param>
        /// <param name="TableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <returns></returns>
        public string GetRecordsCount(string connStr, string TableName, string whereClause)
        {

            SqlDatabase sqd = new SqlDatabase(connStr);
    
            DataTable dtGet;
            string returnString;       

            if (whereClause.Length > 0)
            {
   
                whereClause = whereClause.Replace("= missing", " is NULL ").Replace("#", "'");
      
            }

            dtGet = sqd.ExecuteDataSet("usp_read_records_count", TableName, TableName.SQLTest(), whereClause).Tables[0];
     
            returnString = string.Format("{0}", dtGet.Rows[0][0].ToString());
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
            SqlDatabase sqd = new SqlDatabase(connStr);

            // Provide the query string with a parameter placeholder.

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

            //DataTable dtGet = sqd.ExecuteDataSet("usp_read_records", tableName, tableName.SQLTest()).Tables[0];

            DataTable dtGet = sqd.ExecuteDataSet(CommandType.Text, queryString).Tables[0];

            return dtGet;
        }

        /// <summary>
        /// Gets the table2.
        /// </summary>
        /// <param name="Datasoucename">The datasoucename.</param>
        /// <param name="connStr">The conn STR.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public DataTable GetTopTwoTable(string Datasoucename, string connStr, string tableName)
        {
            SqlDatabase sqd = new SqlDatabase(connStr);

            string queryString;

            if (tableName.SQLTest())
            {
                queryString =
                    string.Format("SELECT TOP 1 * FROM     ( {0}  )  as  f1    ", tableName);
            }
            else
            {
                queryString =
                    string.Format("SELECT TOP 1 * FROM {0} ", tableName);
            }

            //DataTable dtGet = sqd.ExecuteDataSet("usp_read_columns_for_datasource", tableName, tableName.SQLTest()).Tables[0];

            DataTable dtGet = sqd.ExecuteDataSet(CommandType.Text, queryString).Tables[0];

            return dtGet;
        }

        /// <summary>
        /// Gets the records count.
        /// </summary>
        /// <param name="TableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <returns></returns>
        public string GetTotalRecordsCount(string connStr, string TableName)
        {
            //    DatasourceDao dsDao = new DatasourceDao();
            SqlDatabase sqd = new SqlDatabase(connStr);

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

            DataTable dtGet = sqd.ExecuteDataSet(CommandType.Text, queryString).Tables[0];
            //DataTable dt = sqd.ExecuteDataSet("usp_read_records_count", TableName, 0, "").Tables[0];
            string returnString = string.Format("{0}", dtGet.Rows.Count);

            return returnString;
        }
    }
}