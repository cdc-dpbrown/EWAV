/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       PostgreSQLDatasourceDao.cs
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
using Npgsql;
using System.Data;
using Ewav.Security;

namespace Ewav.DAL.PostgreSQL
{
    public class PostgreSQLDatasourceDao : IAdminDatasourceDao
    {

        Cryptography cy = new Cryptography();

        private string tableName;
        /// <summary>
        /// Copies the saved canvas to different datasource
        /// </summary>
        /// <param name="OldCanvasName"></param>
        /// <param name="NewCanvasName"></param>
        /// <param name="OldDatasourceId"></param>
        /// <param name="NewDatasourceId"></param>
        /// <returns></returns>
        public bool CopyDashboard(string OldCanvasName, string NewCanvasName, string NewDatasourceName)
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
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        public string TableName
        {
            get
            {
                return this.tableName;
            }
            set
            {
                this.tableName = value;
            }
        }



        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string ConnectionString
        {
            get;
            set;
        }
        

        /// <summary>
        /// Gets or sets the organization id.
        /// </summary>
        /// <value>
        /// The organization id.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public int OrganizationId
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        /// <summary>
        /// Tests the DB connection.
        /// </summary>
        /// <param name="connStr">The conn STR.</param>
        /// <returns></returns>
        public bool TestDBConnection(string connStr)
        {
            PostgreSQLDB postDb = new PostgreSQLDB(connStr); 
            try
            {
                postDb.OpenConnection();
                postDb.CloseConnection();
                return true;
            }
            catch (Exception ex)
            {
                postDb.CloseConnection();
                postDb = null;
                return false;
            }
        }

        /// <summary>
        /// Tests the data.
        /// </summary>
        /// <param name="sqlOrTableName">Name of the SQL or table.</param>
        /// <returns></returns>
        public bool TestData(string sqlOrTableName)
        {
            PostgreSQLDB postDb = new PostgreSQLDB(this.ConnectionString);
            try
            {
                NpgsqlCommand Command = new NpgsqlCommand();
                Command.CommandType = CommandType.Text;
                

                if (sqlOrTableName.LastIndexOf(" ") > 0)
                {
                    Command.CommandText = string.Format(@"SELECT COUNT(*)  FROM      ({0})    as  f1;    ", sqlOrTableName);
                }
                else
                {
                    Command.CommandText = string.Format(@"SELECT COUNT(*)  FROM      ""{0}""    as  f1;    ", sqlOrTableName);
                }
                int i = Convert.ToInt32(postDb.ExecuteScalar(Command));

                if (i <= 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Adds the datasource.
        /// </summary>
        /// <param name="dsDto">The ds dto.</param>
        /// <returns></returns>
        public bool AddDatasource(DTO.DatasourceDto dsDto)
        {
            PostgreSQLDB postDb = new PostgreSQLDB(this.ConnectionString);



            NpgsqlCommand Command = new NpgsqlCommand();

            int dsId = -1;

            Command.CommandType = System.Data.CommandType.StoredProcedure;

            Command.CommandText = "add_datasource";


            Cryptography cy = new Cryptography();

            NpgsqlParameter parameter = new NpgsqlParameter("dsname", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = dsDto.DatasourceName;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("orgid", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = dsDto.OrganizationId;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("dsservername", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = cy.Encrypt(dsDto.Connection.ServerName);
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("dbtype", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = dsDto.Connection.DatabaseType.ToString();
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("icatalog", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = cy.Encrypt(dsDto.Connection.DatabaseName);
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("psinfo", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = dsDto.Connection.PersistSecurityInfo.ToString();
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("dbuid", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = cy.Encrypt(dsDto.Connection.UserId);
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("pwd", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = cy.Encrypt(dsDto.Connection.Password);
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("dbobject", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = cy.Encrypt(dsDto.Connection.DatabaseObject);
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("sqlqry", NpgsqlTypes.NpgsqlDbType.Boolean);
            parameter.Value = dsDto.SQLQuery();
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("isactive", NpgsqlTypes.NpgsqlDbType.Boolean);
            parameter.Value = dsDto.IsActive;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            StringBuilder sb = new StringBuilder();
            try
            {

                foreach (Ewav.DTO.UserDTO item in dsDto.AssociatedUsers)
                {
                    sb.Append(item.UserID);
                    sb.Append(",");
                }


            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            parameter = new NpgsqlParameter("userids", NpgsqlTypes.NpgsqlDbType.Varchar); 
            if (sb.ToString().Contains(","))
            {
                parameter.Value = sb.ToString().Substring(0, sb.ToString().Length - 1);
            }
            else
            {
                parameter.Value = "";
            }
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);



            parameter = new NpgsqlParameter("pnumber", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = cy.Encrypt(dsDto.Connection.PortNumber.ToString());
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);


            try
            {
                dsId = Convert.ToInt32(postDb.ExecuteScalar(Command));
            }
            catch (Exception)
            {

                return false;
            }

            return true;
        }

        /// <summary>
        /// Updates the datasource.
        /// </summary>
        /// <param name="dsDto">The ds dto.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public bool UpdateDatasource(DTO.DatasourceDto dsDto)
        {
            PostgreSQLDB postDb = new PostgreSQLDB(this.ConnectionString);

            NpgsqlCommand Command = new NpgsqlCommand();

            Command.CommandType = System.Data.CommandType.StoredProcedure;

            Command.CommandText = "update_datasource";


            Cryptography cy = new Cryptography();

            NpgsqlParameter parameter = new NpgsqlParameter("dsname", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = dsDto.DatasourceName;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("dbtype", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = dsDto.Connection.DatabaseType.ToString();
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("psinfo", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = dsDto.Connection.PersistSecurityInfo.ToString();
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("icatalog", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = cy.Encrypt(dsDto.Connection.DatabaseName);
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("dsservername", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = cy.Encrypt(dsDto.Connection.ServerName);
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("dbuid", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = cy.Encrypt(dsDto.Connection.UserId);
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("pwd", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = cy.Encrypt(dsDto.Connection.Password);
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("dbobject", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = cy.Encrypt(dsDto.Connection.DatabaseObject);
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("dsid", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = dsDto.DatasourceId;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("isactive", NpgsqlTypes.NpgsqlDbType.Boolean);
            parameter.Value = dsDto.IsActive;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            StringBuilder sb = new StringBuilder();
            try
            {

                foreach (Ewav.DTO.UserDTO item in dsDto.AssociatedUsers)
                {
                    sb.Append(item.UserID);
                    sb.Append(",");
                }


            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            parameter = new NpgsqlParameter("userids", NpgsqlTypes.NpgsqlDbType.Varchar);
            if (sb.ToString().Contains(","))
            {
                parameter.Value = sb.ToString().Substring(0, sb.ToString().Length - 1);
            }
            else
            {
                parameter.Value = "";
            }
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);


            parameter = new NpgsqlParameter("pnumber", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = cy.Encrypt(dsDto.Connection.PortNumber.ToString());
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            try
            {
                postDb.ExecuteNonQuery(Command);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //return false;
            }
            return true;
        }

        public bool RemoveDatasource(int dsId)
        {
            return false;
        }

        /// <summary>
        /// Gets the columns for datasource.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public DataTable GetColumnsForDatasource()
        {
            DataSet ds = null;
            PostgreSQLDB db = new PostgreSQLDB(ConnectionString);

            NpgsqlCommand Command = new NpgsqlCommand();
            Command.CommandType = CommandType.Text;
            if (TableName.LastIndexOf(" ") > 0)
            {
                Command.CommandText = "select *  from (" + TableName + ") as f1 limit 1;";
            }
            else
            {
                Command.CommandText = "select *  from " + TableName + " limit 1;";
            }

            try
            {
                ds = db.ExecuteDataSet(Command);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ds.Tables[0];


        }
        /// <summary>
        /// Reads the datasource.
        /// </summary>
        /// <param name="orgId">The org id.</param>
        /// <returns></returns>
        public DataSet ReadDatasource(int orgId)
        {

            PostgreSQLDB db = new PostgreSQLDB(this.ConnectionString);
            NpgsqlCommand Command = new NpgsqlCommand();

            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "read_datasource";

            NpgsqlParameter parameter = new NpgsqlParameter("orgid", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = orgId;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            DataSet ds = db.ExecuteDataSet(Command);

            return ds;
        }


        /// <summary>
        /// Reads the associated users.
        /// </summary>
        /// <param name="orgId">The org id.</param>
        /// <returns></returns>
        public DataSet ReadAssociatedUsers(int orgId)
        {
            PostgreSQLDB db = new PostgreSQLDB(this.ConnectionString);
            NpgsqlCommand Command = new NpgsqlCommand();

            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "read_user";

            NpgsqlParameter parameter = new NpgsqlParameter("orgid", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = orgId;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("userid", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = -1;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("email", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = "";
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("roleid", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = -1;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            DataSet ds = db.ExecuteDataSet(Command);

            return ds;
        }

        /// <summary>
        /// Reads the associated users for datasource.
        /// </summary>
        /// <param name="dsId">The ds id.</param>
        /// <param name="orgId">The org id.</param>
        /// <returns></returns>
        public DataSet ReadAssociatedUsersForDatasource(int dsId, int orgId)
        {
            PostgreSQLDB db = new PostgreSQLDB(this.ConnectionString);
            NpgsqlCommand Command = new NpgsqlCommand();

            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "read_users_for_datasource";

            NpgsqlParameter parameter = new NpgsqlParameter("orgid", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = orgId;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("dsid", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = dsId;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);


            DataSet ds = db.ExecuteDataSet(Command);

            return ds;
        }



        /// <summary>
        /// Gets the connection string from DB.
        /// </summary>
        /// <param name="DsName">Name of the ds.</param>
        /// <param name="ConnectionString">The connection string.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public string GetConnectionStringFromDB(string DsName, string ConnectionString)
        {
            DataSet ds = null;
            PostgreSQLDB db = new PostgreSQLDB(ConnectionString);

            NpgsqlCommand Command = new NpgsqlCommand();
            Command.CommandType = CommandType.Text;
            Command.CommandText = "Select  * From Datasource where DatasourceName     = '" +
                           DsName + "' Limit 1;";

            try
            {
                ds = db.ExecuteDataSet(Command);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
            return Utilities.CreateConnectionString(DTO.DataBaseTypeEnum.PostgreSQL, new DataRow[] { ds.Tables[0].Rows[0] });
        }


        public object ReadEWEDatasourceFormId(DTO.EWEDatasourceDto EWEDsDto)
        {
            throw new NotImplementedException("Functionality is not supported for underlying database.");
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