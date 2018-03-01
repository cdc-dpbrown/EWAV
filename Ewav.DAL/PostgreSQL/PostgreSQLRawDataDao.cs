/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       PostgreSQLRawDataDao.cs
 *  Namespace:  Ewav.DAL.PostgreSQL    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ewav.DAL.Interfaces;
using System.Data;
using Ewav.DTO;
using Npgsql;

namespace Ewav.DAL.PostgreSQL
{
    class PostgreSQLRawDataDao : IRawDataDao
    {
        private readonly string MetaDataConnectionString;
        private readonly string MetaDataViewName;

        public PostgreSQLRawDataDao()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerRawDataDao" /> class.
        /// </summary>
        /// <param name="MetaDataConnectionString">The meta data connection string.</param>
        /// <param name="MetaDataViewName">Name of the meta data view.</param>
        public PostgreSQLRawDataDao(string MetaDataConnectionString, string MetaDataViewName)
        {
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
            string externalConnectionString = Utilities.CreateConnectionString(DataBaseTypeEnum.PostgreSQL, dr);

            string query = "";

            if (tableName.SQLTest())
            {
                query = string.Format("SELECT * FROM  ( {0}  ) as table1 limit 1; ", tableName);
            }
            else
            {
                query = string.Format("SELECT * FROM {0} limit 1;", tableName);
            }

            DataTable dt = new DataTable();

            PostgreSQLDB pstdb = new PostgreSQLDB(externalConnectionString);

            NpgsqlCommand Command = new NpgsqlCommand();

            Command.CommandText = query;
            Command.CommandType = CommandType.Text;


            dt = pstdb.ExecuteDataSet(Command).Tables[0];

            return dt;
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
            PostgreSQLDB pdb = new PostgreSQLDB(connStr);

            if (whereClause.Length > 0)
            {
                whereClause = whereClause.Replace("= missing", " is NULL ").Replace("#", "'");
            }

            NpgsqlCommand Command = new NpgsqlCommand();
            Command.CommandType = CommandType.StoredProcedure;

            NpgsqlParameter parameter = new NpgsqlParameter("TableName", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = TableName;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("Test", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = TableName.SQLTest();
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("whereClause", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = whereClause;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            DataTable dtGet;
            string returnString;                           


            dtGet = pdb.ExecuteDataSet(Command).Tables[0];
            returnString = string.Format("{0}", dtGet.Rows[0][0].ToString());
            return returnString;
        }

        /// <summary>
        /// Gets the total records.
        /// </summary>
        /// <param name="externalDatasourceRow">The external datasource row.</param>
        /// <returns></returns>
        public long GetRecordsCount()
        {
            string externalConnectionString = this.MetaDataConnectionString;
            
            PostgreSQLDB pdb = new PostgreSQLDB(externalConnectionString);

            NpgsqlCommand Command = new NpgsqlCommand();
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "read_records_count";

            NpgsqlParameter parameter = new NpgsqlParameter("count", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = "";
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("metadataviewname", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = this.MetaDataViewName;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("Test", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = 0;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("whereClause", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = "";
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            DataTable dt = pdb.ExecuteDataSet(Command).Tables[0];

            return long.Parse(dt.Rows[0]["Count"].ToString());
        }

        /// <summary>
        /// Gets the table.
        /// </summary>
        /// <param name="Datasoucename">The datasoucename.</param>
        /// <param name="connStr">The conn STR.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public DataTable GetTable(string Datasoucename, string connStr, string tableName)
        {

            PostgreSQLDB pdb = new PostgreSQLDB(connStr);

            NpgsqlCommand Command = new NpgsqlCommand();
            Command.CommandType = CommandType.Text;

            if (tableName.SQLTest())
            {
                Command.CommandText =
                    string.Format("SELECT *  FROM     ( {0}  )  as  f1    ", tableName);
            }
            else
            {
                Command.CommandText =
                    string.Format("SELECT *  FROM {0} ", tableName);
            }

            
            NpgsqlParameter parameter = new NpgsqlParameter("tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = tableName;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);


            DataTable dtGet = pdb.ExecuteDataSet(Command).Tables[0];

            return dtGet;
        }

        /// <summary>
        /// Gets the table.
        /// </summary>
        /// <param name="Datasoucename">The datasoucename.</param>
        /// <param name="connStr">The conn STR.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public DataTable GetTopTwoTable(string Datasoucename, string connStr, string tableName)
        {
            PostgreSQLDB pdb = new PostgreSQLDB(connStr);

            NpgsqlCommand Command = new NpgsqlCommand();
            Command.CommandType = CommandType.Text;

            if (tableName.SQLTest())
            {
                Command.CommandText =
                    string.Format("SELECT * FROM     ( {0}  )  as  f1 limit 1; ", tableName);
            }
            else
            {
                Command.CommandText =
                    string.Format("SELECT * FROM {0} limit 1; ", tableName);
            }


            NpgsqlParameter parameter = new NpgsqlParameter("tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = tableName;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);


            DataTable dtGet = pdb.ExecuteDataSet(Command).Tables[0];
            
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
            PostgreSQLDB pdb = new PostgreSQLDB(connStr);

            NpgsqlCommand Command = new NpgsqlCommand();
            Command.CommandType = CommandType.Text;

            if (TableName.SQLTest())
            {
                Command.CommandText =
                    string.Format(@"SELECT *  FROM      ""{0}""    as  f1;    ", TableName);
            }
            else
            {
                Command.CommandText =
                    string.Format(@"SELECT *  FROM ""{0}""; ", TableName);
            }
            
            DataTable dtGet = pdb.ExecuteDataSet(Command).Tables[0];

            string returnString = string.Format("{0}", dtGet.Rows.Count);

            return returnString;
        }

        public int CountRecords(string connStr, string sqlOrTableName)
        {
            throw new NotImplementedException();
        }
    }
}