/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       SqlServerMetaDataDao.cs
 *  Namespace:  Ewav.DAL.SqlServer    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Ewav.DAL.Interfaces;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Ewav.DTO;

namespace Ewav.DAL.SqlServer
{
    public class SqlServerMetaDataDao : IMetaDataDao
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerMetaDataDao" /> class.
        /// </summary>
        /// <param name="ConnectionString">The meta data connection string.</param>
        /// <param name="MetaDataViewName">Name of the meta data view.</param>
        public SqlServerMetaDataDao(string MetaDataConnectionString, string MetaDataViewName)
        {
            this.MetaDataConnectionString = MetaDataConnectionString;
            this.MetaDataViewName = MetaDataViewName;
        }

        public SqlServerMetaDataDao()
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
            //string queryString =
            //    string.Format("SELECT * FROM {0} WHERE active =  'True' And   username =  '{1}'       ", this.MetaDataViewName, userName);

            string queryString;

            //if (userName == "*")
            //{
            //    queryString =
            //        string.Format("SELECT * FROM {0} Left join vwUserOrganizationUser On vwUserDatasource.UserID = vwUserOrganizationUser.UserID and vwUserDatasource.OrganizationId =  vwUserOrganizationUser.OrganizationId   WHERE IsDatasourceactive =  'True'  and Active = 'True' And IsOrgActive ='True'", this.MetaDataViewName);
            //}
            //else
            //{
            //    queryString =
            //       string.Format("SELECT * FROM {0} Left join vwUserOrganizationUser On vwUserDatasource.UserID = vwUserOrganizationUser.UserID and vwUserDatasource.OrganizationId =  vwUserOrganizationUser.OrganizationId   WHERE IsDatasourceactive =  'True' And   vwUserOrganizationUser.username =  '{1}' and Active = 'True' And IsOrgActive ='True'", this.MetaDataViewName, userName);

            //}



            SqlConnection conn = new SqlConnection(this.MetaDataConnectionString);

            // Create the Command and Parameter objects.
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "usp_read_all_datasources";
            command.Parameters.Add(new SqlParameter("UserName", userName));
            command.Parameters.Add(new SqlParameter("DatabaseObject", this.MetaDataViewName));
            command.Connection = conn;
            // create a new data adapter based on the specified query.
            SqlDataAdapter da = new SqlDataAdapter();
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
            //string queryString =
            //    string.Format("SELECT * FROM {0} WHERE DataSourceName =  '{1}' ", this.MetaDataViewName, dataSourceName);

            //SqlConnection conn = new SqlConnection(this.ConnectionString);
            SqlDatabase ewavDB = new SqlDatabase(this.MetaDataConnectionString);

            // Create the Command and Parameter objects.
            //SqlCommand command = new SqlCommand();

            //command.Connection = conn;
            //command.CommandText = "usp_read_database_type";
            //command.Parameters.Add(new SqlParameter("DatabaseObject", dataSourceName));
            //command.Parameters.Add(new SqlParameter("TableName", this.MetaDataViewName));

            //// create a new data adapter based on the specified query.
            //SqlDataAdapter da = new SqlDataAdapter(command);
            ////set the SelectCommand of the adapter
            ////da.SelectCommand = command;
            //// create a new DataTable
            //DataTable dtGet = new DataTable();
            ////fill the DataTable
            //da.Fill(dtGet);
            DataTable dtGet = ewavDB.ExecuteDataSet("usp_read_database_type", dataSourceName, this.MetaDataViewName).Tables[0];

            return ((DataBaseTypeEnum)Enum.Parse(typeof(DataBaseTypeEnum),
                dtGet.Rows[0]["DatabaseType"].ToString()));
            dtGet.Rows[0]["DataSourceName"].ToString();


        }

        /// <summary>
        /// Gets the external connection string to retrieve data for analysis.  Not to je confused with the 
        /// metadata Connection string which is used to connect to the Ewav database    
        /// </summary>
        /// <param name="dataSourceName">Name of the data source.</param>
        /// <returns></returns>
        public string GetExternalConnectionString(string dataSourceName)
        {
            SqlDatabase ewavDB = new SqlDatabase(this.MetaDataConnectionString);

            DataTable dtExternal = ewavDB.ExecuteDataSet("usp_read_external_connec_str", this.MetaDataViewName, dataSourceName).Tables[0];

            DataBaseTypeEnum dataBaseTypeEnum = ((DataBaseTypeEnum)Enum.Parse(typeof(DataBaseTypeEnum),
                dtExternal.Rows[0]["DatabaseType"].ToString()));

            string extConnectionString = " ";

            extConnectionString = Utilities.CreateConnectionString(dataBaseTypeEnum, new DataRow[] { dtExternal.Rows[0] });

            return extConnectionString;
        }

        /// <summary>
        /// Gets the database object for EwavLite  
        /// </summary>
        /// <param name="dataSourceName">Name of the data source.</param>
        /// <returns></returns>
        public string GetDatabaseObject(string dataSourceName)
        {
            SqlDatabase ewavDB = new SqlDatabase(this.MetaDataConnectionString);

            DataTable dtExternal = ewavDB.ExecuteDataSet("usp_read_external_connec_str", this.MetaDataViewName, dataSourceName).Tables[0];

            return dtExternal.Rows[0]["DatabaseObject"].ToString();
        }

        /// <summary>
        /// Gets the external connection string.
        /// </summary>
        /// <param name="dataSourceName">Name of the data source.</param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string GetExternalConnectionString(string dataSourceName, out string tableName)  //     out    DataBaseTypeEnum databaseType)
        {
            //SqlDatabase ewavDB = new SqlDatabase(this.MetaDataConnectionString);

            ////string queryString =
            ////    string.Format("SELECT *  FROM {0} WHERE DatasourceName =  '{1}'", this.MetaDataViewName, dataSourceName);

            ////DataTable dtGet = ewavDB.ExecuteDataSet(CommandType.Text, queryString).Tables[0];
            //DataTable dtGet = ewavDB.ExecuteDataSet("usp_read_external_connec_str", this.MetaDataViewName, dataSourceName).Tables[0];

            ////  string databaseType = dtGet.Rows[0]["DatabaseType"].ToString();

            ////DataBaseTypeEnum dataBaseTypeEnum = ((DataBaseTypeEnum)Enum.Parse(typeof(DataBaseTypeEnum),
            ////    dtGet.Rows[0]["DatabaseType"].ToString()));

            //string extConnectionString = " ";

            //extConnectionString = Utilities.CreateConnectionString(DataBaseTypeEnum.SQLServer,
            //    new DataRow[] { dtGet.Rows[0] });
            ////  string.Format("Data Source={0};Initial Catalog={1};Persist Security Info={2};User ID={3};Password={4}", dtGet.Rows[0]["DatasourceServerName"].ToString(), dtGet.Rows[0]["InitialCatalog"], dtGet.Rows[0]["PersistSecurityInfo"], dtGet.Rows[0]["DatabaseUserID"], dtGet.Rows[0]["Password"]);                  //     shorterd01"   

            //Ewav.Security.Cryptography cy = new Security.Cryptography();
            //tableName = cy.Decrypt(dtGet.Rows[0]["DatabaseObject"].ToString());


            //return extConnectionString;    


            throw new NotImplementedException();

        }

        public void SaveCanvas(CanvasDto canvasDto)
        {
            throw new NotImplementedException();
        }
    }
}