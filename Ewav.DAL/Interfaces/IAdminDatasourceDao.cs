/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       IAdminDatasourceDao.cs
 *  Namespace:  Ewav.DAL.Interfaces    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Ewav.DTO;
using System.Data;
// using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Ewav.DAL.Interfaces
{
    public interface IAdminDatasourceDao
    {
        DataSet GetDatasource(int datasourceId);

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        string TableName { get; set; }

        /// <summary>
        /// Tests the DB connection.
        /// </summary>
        /// <param name="connStr">The conn STR.</param>
        /// <returns></returns>
        bool TestDBConnection(string connStr);

        /// <summary>
        /// Tests the data.
        /// </summary>
        /// <param name="sqlOrTableName">Name of the SQL or table.</param>
        /// <returns></returns>
        bool TestData(string sqlOrTableName);

        /// <summary>
        /// Adds the datasource.
        /// </summary>
        /// <param name="dsDto">The ds dto.</param>
        /// <returns></returns>
        bool AddDatasource(DatasourceDto dsDto);

        /// <summary>
        /// Updates the datasource.
        /// </summary>
        /// <param name="dsDto">The ds dto.</param>
        /// <returns></returns>
        bool UpdateDatasource(DatasourceDto dsDto);

        /// <summary>
        /// Removes the datasource.
        /// </summary>
        /// <param name="dsId">The ds id.</param>
        /// <returns></returns>
        bool RemoveDatasource(int dsId);

        /// <summary>
        /// Reads the datasource.
        /// </summary>
        /// <param name="orgId">The org id.</param>
        /// <returns></returns>
        DataSet ReadDatasource(int orgId);

        /// <summary>
        /// Reads the users that are part of a organization    
        /// </summary>
        /// <param name="orgId">The org id.</param>
        /// <returns></returns>
        DataSet ReadAssociatedUsers(int orgId);

        /// <summary>
        ///  Reads the users that are part of a organization and have access to a particular datasource  
        /// </summary>
        /// <param name="dsID">The datasource ID.</param>
        /// <param name="orgId">The organization id.</param>
        /// <returns></returns>
        DataSet ReadAssociatedUsersForDatasource(int dsID, int orgId);

        /// <summary>
        /// Gets the columns for the table or view for a datasource.
        /// </summary>
        /// <returns></returns>
        DataTable GetColumnsForDatasource();

        /// <summary>
        /// Gets the connection string from DB.
        /// </summary>
        /// <param name="DsName">Name of the ds.</param>
        /// <param name="ConnectionString">The connection string.</param>
        /// <returns></returns>
        string GetConnectionStringFromDB(string DsName, string ConnectionString);


        /// <summary>
        ///  Copies the dashboard.
        /// </summary>
        /// <param name="OldCanvasName"></param>
        /// <param name="NewCanvasName"></param>
        /// <param name="NewDatasourceName"></param>
        /// <returns></returns>
        bool CopyDashboard(string OldCanvasName, string NewCanvasName, string NewDatasourceName);

        /// <summary>
        /// Used to read EWEDatasource Form id if present
        /// </summary>
        /// <param name="EWEDsDto"></param>
        /// <returns></returns>
        object ReadEWEDatasourceFormId(EWEDatasourceDto EWEDsDto);

        /// <summary>
        /// Read ewav datasource associated to formid/datasourceid
        /// </summary>
        /// <param name="DatasourceId"></param>
        /// <returns></returns>
        //List<int> ReadEwavDatasource(Guid DatasourceId, int UserId);
        //This functionality is replaced by similiar functionality in Canvas


        DataSet ReadAllDatasourceUsers();
    }
}