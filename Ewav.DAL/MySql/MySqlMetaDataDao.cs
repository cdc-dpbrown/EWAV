/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MySqlMetaDataDao.cs
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
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using MySql.Data.MySqlClient;

namespace Ewav.DAL.MySqlLayer
{
    public class MySqlMetaDataDao : IMetaDataDao
    {
        //private DataTable dtAllDataSources;
        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlMetaDataDao" /> class.
        /// </summary>
        /// <param name="ConnectionString">The meta data connection string.</param>
        /// <param name="MetaDataViewName">Name of the meta data view.</param>
        public MySqlMetaDataDao(string MetaDataConnectionString, string MetaDataViewName)
        {
            // TODO: Complete member initialization
            this.MetaDataConnectionString = MetaDataConnectionString;
            this.MetaDataViewName = MetaDataViewName;
        }

        /// <summary>
        /// Gets or sets the ewav data connection string.
        /// </summary>
        /// <value>The ewav data connection string.</value>
        public string EwavDataConnectionString
        {
            get
            {
                // TODO: Implement this property getter
                throw new NotImplementedException();
            }
            set
            {
                // TODO: Implement this property setter
                throw new NotImplementedException();
            }
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
        /// <param name="userName"></param>
        /// <returns>A data table with one row of meta data for each data source</returns>
        public DataTable GetAllDataSources(string userName)
        {
            DataTable dtGet = null;

            try
            {


                string queryString = string.Format("call  usp_read_all_datasources ( '{0}' , '{1}'  ) ", userName, this.MetaDataViewName);

                DataSet ds = MySqlHelper.ExecuteDataset(new MySqlConnection(MetaDataConnectionString), queryString);
                dtGet = ds.Tables[0];
            }
            catch (Exception)
            {
            }

            return dtGet;
        }

        /// <summary>
        /// Gets all available data sources
        /// </summary>
        /// <returns>A data table with one row of meta data for each data source</returns>
        public DataTable GetAllDataSources()
        {
            string queryString =
                string.Format("SELECT * FROM {0} where active = 1  ", this.MetaDataViewName);

            MySqlConnection conn = new MySqlConnection(this.MetaDataConnectionString);

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

            //       this.dtAllDataSources = dtGet;

            return dtGet;
        }

        /// <summary>
        /// Gets the columns for datasource.
        /// </summary>
        /// <param name="dataSourceName"></param>
        /// <returns></returns>
        public DataTable GetColumnsForDatasource(string dataSourceName)
        {
            string externalConnectionString = this.MetaDataConnectionString;

            SqlDatabase sqd = new SqlDatabase(externalConnectionString);

            string query = string.Format("SELECT TOP 1 * FROM {0}", this.MetaDataViewName);
            DataTable dt = sqd.ExecuteDataSet(CommandType.Text, query).Tables[0];

            return dt;
        }

        /// <summary>
        /// Gets the type of the database.
        /// </summary>
        /// <param name="dataSourceName">Name of the data source.</param>
        /// <returns></returns>
        public DataBaseTypeEnum GetDatabaseType(string dataSourceName)
        {
            string queryString =
                string.Format("call  usp_read_database_type ('{0}' ) ", dataSourceName);
            // string.Format("SELECT * FROM {0} WHERE DataSourceName =  '{1}' ", this.MetaDataViewName, dataSourceName);

            MySqlConnection conn = new MySqlConnection(this.MetaDataConnectionString);

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

            return ((DataBaseTypeEnum)Enum.Parse(typeof(DataBaseTypeEnum),
                dtGet.Rows[0]["DatabaseType"].ToString()));
            //    dtGet.Rows[0][ "DataSourceName"    ].ToString()    ;
        }


        /// <summary>
        /// Gets the external connection string to retrieve data for analysis.  Not to je confused with the
        /// metadata Connection string which is used to connect to the Ewav database
        /// </summary>
        /// <param name="dataSourceName">Name of the data source.</param>
        /// <returns></returns>
        public string GetExternalConnectionString(string dataSourceName)
        {
            //string queryString =
            //    string.Format("SELECT *  FROM {0} WHERE DatasourceName =  '{1}'", this.MetaDataViewName, dataSourceName);    
            string queryString = string.Format("Call usp_read_external_connec_str ('{0}', '{1}') ", this.MetaDataViewName, dataSourceName);

            MySqlConnection conn = new MySqlConnection(this.MetaDataConnectionString);

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

            //string extConnectionString =
            //    string.Format("Data Source={0};Initial Catalog={1};Persist Security Info={2};User ID={3};Password={4}", dtGet.Rows[0]["DatasourceServerName"].ToString(), dtGet.Rows[0]["InitialCatalog"], dtGet.Rows[0]["PersistSecurityInfo"], dtGet.Rows[0]["DatabaseUserID"], dtGet.Rows[0]["Password"]);                  //     shorterd01"   

            string extConnectionString = " ";

            string datasourceType = dtGet.Rows[0]["DatabaseType"].ToString();

            DataBaseTypeEnum dataBaseTypeEnum = (DataBaseTypeEnum)Enum.Parse(typeof(DataBaseTypeEnum), datasourceType);
            extConnectionString = Utilities.CreateConnectionString(dataBaseTypeEnum, new DataRow[] { dtGet.Rows[0] });

            return extConnectionString;
        }

        /// <summary>
        /// Gets the external connection string.
        /// </summary>
        /// <param name="dataSourceName">Name of the data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="databaseType">Type of the database.</param>
        /// <returns></returns>
        public string GetExternalConnectionString(out string tableName, out DataBaseTypeEnum databaseType)
        {
            // TODO: Implement this method
            tableName = null;
            databaseType = default(DataBaseTypeEnum);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the external connection string.
        /// </summary>
        /// <param name="dataSourceName">Name of the data source.</param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string GetExternalConnectionString(string dataSourceName, out string tableName)
        {
            // TODO: Implement this method
            tableName = null;
            throw new NotImplementedException();
        }

        public void SaveCanvas(CanvasDto canvasDto)
        {
            throw new NotImplementedException();
        }


        public string GetDatabaseObject(string datasourceName)
        {
            string queryString = string.Format("Call usp_read_external_connec_str ('{0}', '{1}') ", this.MetaDataViewName, datasourceName);

            MySqlConnection conn = new MySqlConnection(this.MetaDataConnectionString);

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

            return dtGet.Rows[0]["DatabaseObject"].ToString();
        }
    }
}