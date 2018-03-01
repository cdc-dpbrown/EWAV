/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MySqlDatasourceDao.cs
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
using Ewav.Security;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Ewav.DAL.MySqlLayer
{
    public class MySqlDatasourceDao : IAdminDatasourceDao
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName { get; set; }

        /// <summary>
        /// Copies the saved canvas to different datasource
        /// </summary>
        /// <param name="OldCanvasName"></param>
        /// <param name="NewCanvasName"></param>
        /// <param name="OldDatasourceId"></param>
        /// <param name="NewDatasourceId"></param>
        /// <returns></returns>
        public bool CopyDashboard(string OldCanvasName, string NewCanvasName,  string NewDatasourceName)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the datasource.
        /// </summary>
        /// <param name="datasourceId">The datasource id.</param>
        /// <returns></returns>
        public DataSet GetDatasource(int datasourceId)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds the datasource.
        /// </summary>
        /// <param name="dsDto">The ds dto.</param>
        /// <returns></returns>
        public bool AddDatasource(DTO.DatasourceDto dsDto)
        {
            MySqlConnection mySqlConn = new MySqlConnection(this.ConnectionString);

            mySqlConn.Open();

            MySqlCommand addDSCommand = mySqlConn.CreateCommand();
            MySqlCommand addUserDSCommand = mySqlConn.CreateCommand();

            int dsId = -1;

            addDSCommand.CommandType = System.Data.CommandType.StoredProcedure;

            addDSCommand.CommandText = "usp_add_datasource"; //TBD

            Cryptography cy = new Cryptography();

            addDSCommand.Parameters.Add("DatasourceNameArg", dsDto.DatasourceName);
            addDSCommand.Parameters.Add("OrganizationId", dsDto.OrganizationId);
            addDSCommand.Parameters.Add("DatasourceServerName", cy.Encrypt(dsDto.Connection.ServerName));
            addDSCommand.Parameters.Add("DatabaseType", dsDto.Connection.DatabaseType.ToString());
            addDSCommand.Parameters.Add("InitialCatalog", cy.Encrypt(dsDto.Connection.DatabaseName));
            addDSCommand.Parameters.Add("PersistSecurityInfo", dsDto.Connection.PersistSecurityInfo.ToString());
            addDSCommand.Parameters.Add("DatabaseUserID", cy.Encrypt(dsDto.Connection.UserId));
            addDSCommand.Parameters.Add("Password", cy.Encrypt(dsDto.Connection.Password));
            addDSCommand.Parameters.Add("DatabaseObject", cy.Encrypt(dsDto.Connection.DatabaseObject));
            addDSCommand.Parameters.Add("SQLQuery", dsDto.SQLQuery());

            addDSCommand.Parameters.Add("active", dsDto.IsActive);

            addDSCommand.Parameters.Add("@DatasourceUser", SqlDbType.Structured);
            addDSCommand.Parameters["@DatasourceUser"].Direction = ParameterDirection.Input;

            addDSCommand.Parameters.Add("portnumber", cy.Encrypt(dsDto.Connection.PortNumber));

            try
            {
                string assUsers = "";
                string datasourceID = "";

                foreach (Ewav.DTO.UserDTO item in dsDto.AssociatedUsers)
                {
                    assUsers += string.Format("{0},", item.UserID.ToString());
                    datasourceID += string.Format("{0},", dsDto.DatasourceId.ToString());
                }


                addDSCommand.Parameters.Add("datasource_ids", datasourceID);
                addDSCommand.Parameters.Add("user_ids", assUsers);



            }
            catch (Exception  ex  ) 
            {
                throw new Exception(ex.Message);
            }

            try
            {
                dsId = Convert.ToInt32(addDSCommand.ExecuteScalar());
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Gets the columns for the table or view for a datasource.
        /// </summary>
        /// <returns></returns>
        public DataTable GetColumnsForDatasource()
        {
            DataSet ds = null;
            //  SqlDatabase db = new SqlDatabase(ConnectionString);
            try
            {
                ds = MySql.Data.MySqlClient.MySqlHelper.ExecuteDataset(ConnectionString,
                    string.Format("select   *  from {0} limit 1 ", TableName));
            }
            catch (MySql.Data.MySqlClient.MySqlException sqlEx)
            {
                Exception duplicateException = new Exception(string.Format("SQL Exception - {0}", sqlEx.Message));
                throw duplicateException;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ds.Tables[0];
        }

        /// <summary>
        /// Gets the connection string from DB.
        /// </summary>
        /// <param name="DsName">Name of the ds.</param>
        /// <param name="ConnectionString">The connection string.</param>
        /// <returns></returns>
        public string GetConnectionStringFromDB(string DsName, string ConnectionString)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads the users that are part of a organization
        /// </summary>
        /// <param name="orgId">The org id.</param>
        /// <returns></returns>
        public System.Data.DataSet ReadAssociatedUsers(int orgId)
        {
            DataSet ds = null;

            try
            {
                //ds = MySqlUtilities.ExecuteSimpleDatsetFromStoredProc(connectionString, "usp_read_user",
                //         new string[] { orgId.ToString(), "-1", "null", "-1", dsID.ToString() });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ds;
        }


        /// <summary>
        /// Reads the users that are part of a organization and have access to a particular datasource
        /// </summary>
        /// <param name="dsID">The datasource ID.</param>
        /// <param name="orgId">The organization id.</param>
        /// <returns></returns>
        public DataSet ReadAssociatedUsersForDatasource(int dsID, int orgId)
        {
            DataSet ds = null;

            try
            {
                ds = MySqlUtilities.ExecuteSimpleDatsetFromStoredProc(ConnectionString, "usp_read_users_for_datasource",
                    orgId.ToString(), dsID.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ds;
        }

        public System.Data.DataSet ReadDatasource(int orgId)
        {
            DataSet ds = null;

            try
            {
                ds = MySqlUtilities.ExecuteSimpleDatsetFromStoredProc(ConnectionString, "usp_read_datasource",
                    new string[] { orgId.ToString(), "-1" });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ds;
        }

        public bool RemoveDatasource(int dsId)
        {
            return false;
        }

        public bool TestData(string sqlOrTableName)
        {
            try
            {
                MySql.Data.MySqlClient.MySqlHelper.ExecuteScalar(this.ConnectionString, string.Format("Select Count(*) from {0}", sqlOrTableName));
                int i = Convert.ToInt32(MySql.Data.MySqlClient.MySqlHelper.ExecuteScalar(this.ConnectionString, string.Format("Select Count(*) from {0}", sqlOrTableName)));

                if (i <= 0)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
                //throw;
            }
            return true;
        }

        public bool TestDBConnection(string connStr)
        {
            MySql.Data.MySqlClient.MySqlConnection sqlConn = new MySql.Data.MySqlClient.MySqlConnection(this.ConnectionString);

            try
            {
                sqlConn.Open();
                sqlConn.Close();
                return true;
            }
            catch (Exception)
            {
                sqlConn.Close();
                sqlConn = null;
                return false;
            }
        }

        public bool UpdateDatasource(DTO.DatasourceDto dsDto)
        {
            MySqlConnection mySqlConn = new MySqlConnection(this.ConnectionString);

            mySqlConn.Open();

            MySqlCommand addDSCommand = mySqlConn.CreateCommand();
            MySqlCommand addUserDSCommand = mySqlConn.CreateCommand();

            addDSCommand.CommandType = System.Data.CommandType.StoredProcedure;

            addDSCommand.CommandText = "usp_update_datasource";

            Cryptography cy = new Cryptography();

            addDSCommand.Parameters.Add("DatasourceNameArg", dsDto.DatasourceName);
            addDSCommand.Parameters.Add("DatabaseType", dsDto.Connection.DatabaseType.ToString());
            addDSCommand.Parameters.Add("PersistSecurityInfo", dsDto.Connection.PersistSecurityInfo.ToString());
            addDSCommand.Parameters.Add("InitialCatalog", cy.Encrypt(dsDto.Connection.DatabaseName));
            addDSCommand.Parameters.Add("DatasourceServerName", cy.Encrypt(dsDto.Connection.ServerName));
            addDSCommand.Parameters.Add("DatabaseUserID", cy.Encrypt(dsDto.Connection.UserId));
            addDSCommand.Parameters.Add("Password", cy.Encrypt(dsDto.Connection.Password));
            addDSCommand.Parameters.Add("DatabaseObject", cy.Encrypt(dsDto.Connection.DatabaseObject));
            addDSCommand.Parameters.Add("DatasourceID", dsDto.DatasourceId);

            addDSCommand.Parameters.Add("active", dsDto.IsActive);

            try
            {
                string assUsers = "";
                string datasourceID = "";

                foreach (Ewav.DTO.UserDTO item in dsDto.AssociatedUsers)
                {
                    assUsers += string.Format("{0},", item.UserID.ToString());
                    datasourceID += string.Format("{0},", dsDto.DatasourceId.ToString());
                }
            }
            catch (Exception)
            {
            }

            try
            {
                int dsId = Convert.ToInt32(addDSCommand.ExecuteScalar());
            }
            catch (Exception)
            {
                return false;
            }

            return true;

            return true;
        }


        public object ReadEWEDatasourceFormId(DTO.EWEDatasourceDto EWEDsDto)
        {
            
            throw new NotImplementedException("Functionality is not supported for underlying database type.");
        }

        public DataSet ReadAllDatasourceUsers()
        {
            throw new NotImplementedException();
        }


        //public List<int> ReadEwavDatasource(Guid DatasourceId)
        //{
        //    throw new NotImplementedException("Functionality is not supported for underlying database type.");
        //}
    }
}