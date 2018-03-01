/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       PostgreSQLMetaDataDao.cs
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
using Npgsql;
using Ewav.DTO;

namespace Ewav.DAL.PostgreSQL
{
    public class PostgreSQLMetaDataDao : IMetaDataDao
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerMetaDataDao" /> class.
        /// </summary>
        /// <param name="MetaDataConnectionString">The meta data connection string.</param>
        /// <param name="MetaDataViewName">Name of the meta data view.</param>
        public PostgreSQLMetaDataDao(string MetaDataConnectionString, string MetaDataViewName)
        {
            this.MetaDataConnectionString = MetaDataConnectionString;
            this.MetaDataViewName = MetaDataViewName;
        }

        public PostgreSQLMetaDataDao()
        {
        }

        /// <summary>
        /// Gets or sets the meta data connection string.
        /// </summary>
        /// <value>The meta data connection string.</value>
        public string MetaDataConnectionString { get; set; }
        /// <summary>
        /// Gets or sets the name of the meta data view.
        /// </summary>
        /// <value>The name of the meta data view.</value>
        public string MetaDataViewName { get; set; }
        /// <summary>
        /// Gets all available data sources
        /// </summary>
        /// <returns>A data table with one row of meta data for each data source</returns>
        public DataTable GetAllDataSources(string userName)
        {
            PostgreSQLDB db = new PostgreSQLDB(this.MetaDataConnectionString);
            try
            {
                NpgsqlCommand Command = new NpgsqlCommand();
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "read_all_datasources";

                NpgsqlParameter parameter = new NpgsqlParameter("uname", NpgsqlTypes.NpgsqlDbType.Varchar);
                parameter.Value = userName;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                parameter = new NpgsqlParameter("dbobject", NpgsqlTypes.NpgsqlDbType.Varchar);
                parameter.Value = this.MetaDataViewName;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                DataSet ds = db.ExecuteDataSet(Command);

                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Gets the columns for datasource.
        /// </summary>
        /// <param name="dataSourceName"></param>
        /// <returns></returns>
        public DataTable GetColumnsForDatasource(string dataSourceName)
        {

            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the type of the database.
        /// </summary>
        /// <param name="dataSourceName">Name of the data source.</param>
        /// <returns></returns>
        public DataBaseTypeEnum GetDatabaseType(string dataSourceName)
        {
            PostgreSQLDB ewavDB = new PostgreSQLDB(this.MetaDataConnectionString);

            try
            {
                NpgsqlCommand Command = new NpgsqlCommand();
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "read_database_type";

                NpgsqlParameter parameter = new NpgsqlParameter("dsname", NpgsqlTypes.NpgsqlDbType.Varchar);
                parameter.Value = dataSourceName;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                parameter = new NpgsqlParameter("dbobject", NpgsqlTypes.NpgsqlDbType.Varchar);
                parameter.Value = this.MetaDataViewName;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                DataTable dtGet = ewavDB.ExecuteDataSet(Command).Tables[0];

                return ((DataBaseTypeEnum)Enum.Parse(typeof(DataBaseTypeEnum),
               dtGet.Rows[0]["DatabaseType"].ToString()));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        /// <summary>
        /// Gets the external connection string.
        /// </summary>
        /// <param name="dataSourceName">Name of the data source.</param>
        /// <returns></returns>
        public string GetExternalConnectionString(string dataSourceName)
        {
            DataTable dtGet;
            PostgreSQLDB ewavDB = new PostgreSQLDB(this.MetaDataConnectionString);
            NpgsqlCommand Command = new NpgsqlCommand();
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "read_external_connec_str";

            NpgsqlParameter parameter = new NpgsqlParameter("dsname", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = dataSourceName;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("dbobject", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = this.MetaDataViewName;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);
            try
            {
                dtGet = ewavDB.ExecuteDataSet(Command).Tables[0];
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

            DataBaseTypeEnum dataBaseTypeEnum = ((DataBaseTypeEnum)Enum.Parse(typeof(DataBaseTypeEnum),
             dtGet.Rows[0]["DatabaseType"].ToString()));

            string extConnectionString = " ";

            extConnectionString = Utilities.CreateConnectionString(dataBaseTypeEnum, new DataRow[] { dtGet.Rows[0] });

            return extConnectionString;
        }

        /// <summary>
        /// Gets the external connection string.
        /// </summary>
        /// <param name="dataSourceName">Name of the data source.</param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string GetExternalConnectionString(string dataSourceName, out string tableName)  //     out    DataBaseTypeEnum databaseType)
        {
            DataTable dtGet;

            PostgreSQLDB ewavDB = new PostgreSQLDB(this.MetaDataConnectionString);
            NpgsqlCommand Command = new NpgsqlCommand();
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "read_external_connec_str";

            NpgsqlParameter parameter = new NpgsqlParameter("dsname", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = dataSourceName;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("dbobject", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = this.MetaDataViewName;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);
            try
            {
                dtGet = ewavDB.ExecuteDataSet(Command).Tables[0];
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

            string extConnectionString = " ";

            extConnectionString = Utilities.CreateConnectionString(DataBaseTypeEnum.PostgreSQL,
                new DataRow[] { dtGet.Rows[0] });

            Ewav.Security.Cryptography cy = new Security.Cryptography();
            tableName = cy.Decrypt(dtGet.Rows[0]["DatabaseObject"].ToString());


            return extConnectionString;
        }

        public void SaveCanvas(CanvasDto canvasDto)
        {
            throw new NotImplementedException();
        }


        public string GetDatabaseObject(string dataSourceName)
        {
            DataTable dtGet;
            PostgreSQLDB ewavDB = new PostgreSQLDB(this.MetaDataConnectionString);
            NpgsqlCommand Command = new NpgsqlCommand();
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "read_external_connec_str";

            NpgsqlParameter parameter = new NpgsqlParameter("dsname", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = dataSourceName;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("dbobject", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = this.MetaDataViewName;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);
            try
            {
                dtGet = ewavDB.ExecuteDataSet(Command).Tables[0];
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

            return dtGet.Rows[0]["DatabaseObject"].ToString();
        }
    }
}