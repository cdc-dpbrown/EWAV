/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       SqlServerDatasourceDao.cs
 *  Namespace:  Ewav.DAL.SqlServer    
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
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using Microsoft.SqlServer.Server;
using Ewav.Security;


namespace Ewav.DAL.SqlServer
{
    public class SqlServerDatasourceDao : IAdminDatasourceDao
    {
        /// <summary>
        /// Gets or sets the P table name1.
        /// </summary>
        /// <value>The P table name1.</value>
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



        public bool TestDBConnection(string connStr)
        {
            //SqlDatabase db = new SqlDatabase(connStr);
            SqlConnection sqlConn = new SqlConnection(this.ConnectionString);
            try
            {
                sqlConn.Open();
                sqlConn.Close();
                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                sqlConn = null;
                return false;
            }
        }

      
        public bool TestData(string sqlOrTableName)
        {
            SqlDatabase db = new SqlDatabase(this.ConnectionString);

            string sqlQuery = "";

            if (sqlOrTableName.LastIndexOf(" ") > 0)
            {
                sqlQuery = "Select Count(*) from (" + sqlOrTableName + ") as F1";
            }
            else
            {
                sqlQuery = "Select Count(*) from " + sqlOrTableName + "";
            }

            try
            {

                int i = Convert.ToInt32(db.ExecuteScalar(CommandType.Text, sqlQuery));

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

        /// <summary>
        /// Adds the datasource.
        /// </summary>
        /// <param name="dsDto">The ds dto.</param>
        /// <returns></returns>
        public bool AddDatasource(DTO.DatasourceDto dsDto)
        {


            SqlConnection sqlConn = new SqlConnection(this.ConnectionString);

            sqlConn.Open();

            SqlCommand addDSCommand = sqlConn.CreateCommand();
            SqlCommand addUserDSCommand = sqlConn.CreateCommand();

            int dsId = -1;

            addDSCommand.CommandType = System.Data.CommandType.StoredProcedure;

            addDSCommand.CommandText = "usp_add_datasource";

            object Formobj = ReadEWEDatasourceFormId(new DTO.EWEDatasourceDto() { DatabaseName = dsDto.Connection.DatabaseName });



            Cryptography cy = new Cryptography();

            addDSCommand.Parameters.Add("EIWSSurveyId", (Formobj != null) ? Formobj : DBNull.Value);
            addDSCommand.Parameters.Add("DatasourceName", dsDto.DatasourceName);
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

            addDSCommand.Parameters.Add("pnumber", cy.Encrypt(dsDto.Connection.PortNumber));

            addDSCommand.Parameters.Add("@DatasourceUser", SqlDbType.Structured);
            addDSCommand.Parameters["@DatasourceUser"].Direction = ParameterDirection.Input;
            addDSCommand.Parameters["@DatasourceUser"].TypeName = "DatasourceUserTableType";

            List<SqlDataRecord> sqlDrList = new List<SqlDataRecord>();

            SqlDataRecord sqdr;


            try
            {


                foreach (Ewav.DTO.UserDTO item in dsDto.AssociatedUsers)
                {
                    sqdr = new SqlDataRecord(new SqlMetaData[] 
                    { new SqlMetaData("DatasourceID", SqlDbType.Int ), 
                       new SqlMetaData("UserID", SqlDbType.Int)      
                    });

                    // Set the record fields.
                    sqdr.SetInt32(0, 0);
                    sqdr.SetInt32(1, item.UserID);

                    sqlDrList.Add(sqdr);
                }

                //// Also add the creator    
                //sqdr = new SqlDataRecord(new SqlMetaData[] 
                //    { new SqlMetaData("DatasourceID", SqlDbType.Int ), 
                //       new SqlMetaData("UserID", SqlDbType.Int)      
                //    });

                //sqdr.SetInt32(0, 0);
                //sqdr.SetInt32(1, dsDto.CreatorID);
                //sqlDrList.Add(sqdr);


                if (dsDto.AssociatedUsers.Count == 0)
                    addDSCommand.Parameters["@DatasourceUser"].Value = null;
                else
                    addDSCommand.Parameters["@DatasourceUser"].Value = sqlDrList;

            }
            catch (Exception e)
            {

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

        public bool UpdateDatasource(DTO.DatasourceDto dsDto)
        {
            SqlConnection sqlConn = new SqlConnection(this.ConnectionString);

            sqlConn.Open();

            object Formobj = null;

            if (dsDto.IsEpiInfoForm)
            {
                Formobj = ReadEWEDatasourceFormId(new DTO.EWEDatasourceDto() { DatabaseName = dsDto.Connection.DatabaseName });
            }


            SqlCommand addDSCommand = sqlConn.CreateCommand();
            SqlCommand addUserDSCommand = sqlConn.CreateCommand();

            addDSCommand.CommandType = System.Data.CommandType.StoredProcedure;

            addDSCommand.CommandText = "usp_update_datasource";

            Cryptography cy = new Cryptography();
            addDSCommand.Parameters.Add("EIWSSurveyId", (Formobj != null) ? Formobj : DBNull.Value);
            addDSCommand.Parameters.Add("DatasourceName", dsDto.DatasourceName);
            addDSCommand.Parameters.Add("DatabaseType", dsDto.Connection.DatabaseType.ToString());
            addDSCommand.Parameters.Add("PersistSecurityInfo", dsDto.Connection.PersistSecurityInfo.ToString());
            addDSCommand.Parameters.Add("InitialCatalog", cy.Encrypt(dsDto.Connection.DatabaseName));
            addDSCommand.Parameters.Add("DatasourceServerName", cy.Encrypt(dsDto.Connection.ServerName));
            addDSCommand.Parameters.Add("DatabaseUserID", cy.Encrypt(dsDto.Connection.UserId));
            addDSCommand.Parameters.Add("Password", cy.Encrypt(dsDto.Connection.Password));
            addDSCommand.Parameters.Add("DatabaseObject", cy.Encrypt(dsDto.Connection.DatabaseObject));
            addDSCommand.Parameters.Add("DatasourceID", dsDto.DatasourceId);

            addDSCommand.Parameters.Add("active", dsDto.IsActive);

            addDSCommand.Parameters.Add("pnumber", cy.Encrypt(dsDto.Connection.PortNumber));
            addDSCommand.Parameters.Add("@DatasourceUser", SqlDbType.Structured);
            addDSCommand.Parameters["@DatasourceUser"].Direction = ParameterDirection.Input;
            addDSCommand.Parameters["@DatasourceUser"].TypeName = "DatasourceUserTableType";


            List<SqlDataRecord> sqlDrList = new List<SqlDataRecord>();

            SqlDataRecord sqdr;

            try
            {


                foreach (Ewav.DTO.UserDTO item in dsDto.AssociatedUsers)
                {
                    sqdr = new SqlDataRecord(new SqlMetaData[] 
                    { new SqlMetaData("DatasourceID", SqlDbType.Int ), 
                       new SqlMetaData("UserID", SqlDbType.Int)      
                    });

                    // Set the record fields.
                    sqdr.SetInt32(0, dsDto.DatasourceId);
                    sqdr.SetInt32(1, item.UserID);

                    sqlDrList.Add(sqdr);
                }

                //// Also add the creator    
                //sqdr = new SqlDataRecord(new SqlMetaData[] 
                //    { new SqlMetaData("DatasourceID", SqlDbType.Int ), 
                //       new SqlMetaData("UserID", SqlDbType.Int)      
                //    });

                //sqdr.SetInt32(0, dsDto.DatasourceId);
                //sqdr.SetInt32(1, dsDto.CreatorID);
                //sqlDrList.Add(sqdr);


                if (dsDto.AssociatedUsers.Count == 0)
                    addDSCommand.Parameters["@DatasourceUser"].Value = null;
                else
                    addDSCommand.Parameters["@DatasourceUser"].Value = sqlDrList;

            }
            catch (Exception e)
            {

            }


            try
            {
                addDSCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //throw ex;
                return false;
            }


            return true;
        }

        public bool RemoveDatasource(int dsId)
        {
            return false;
        }

        public DataTable GetColumnsForDatasource()
        {
            DataSet ds = null;
            SqlDatabase db = new SqlDatabase(ConnectionString);
            string sqlQuery = "";

            if (TableName.LastIndexOf(" ") > 0)
            {
                sqlQuery = "select top 1  *  from (" + TableName + ") as F1";
            }
            else
            {
                sqlQuery = "select top 1  *  from " + TableName;
            }

            try
            {
                ds = db.ExecuteDataSet(CommandType.Text, sqlQuery);
            }
            catch (SqlException sqlEx)
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



        public DataSet GetDatasource(int datasourceId)
        {
            DataSet ds = null;
            SqlDatabase db = new SqlDatabase(ConnectionString);
            try
            {
                ds = db.ExecuteDataSet("usp_get_datasource", datasourceId);
            }
            catch (SqlException sqlEx)
            {
                Exception duplicateException = new Exception(string.Format("SQL Exception - {0}", sqlEx.Message));
                throw duplicateException;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ds;
        }

        public DataSet ReadDatasource(int orgId)
        {
            DataSet ds = null;
            SqlDatabase db = new SqlDatabase(ConnectionString);
            try
            {
                ds = db.ExecuteDataSet("usp_read_datasource", orgId, -1);//, UserId TBD
            }
            catch (SqlException sqlEx)
            {
                Exception duplicateException = new Exception(string.Format("SQL Exception - {0}", sqlEx.Message));
                throw duplicateException;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ds;
        }


        public DataSet ReadAssociatedUsers(int orgId)
        {
            DataSet ds = null;
            SqlDatabase db = new SqlDatabase(ConnectionString);
            try
            {
                ds = db.ExecuteDataSet("usp_read_user", orgId, -1, "", -1);
            }
            catch (SqlException sqlEx)
            {
                Exception duplicateException = new Exception(string.Format("SQL Exception - {0}", sqlEx.Message));
                throw duplicateException;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ds;
        }

        public DataSet ReadAssociatedUsersForDatasource(int dsId, int orgId)
        {
            DataSet ds = null;
            SqlDatabase db = new SqlDatabase(ConnectionString);
            try
            {
                ds = db.ExecuteDataSet("usp_read_users_for_datasource", orgId, dsId);

            }
            catch (SqlException sqlEx)
            {
                Exception duplicateException = new Exception(string.Format("SQL Exception - {0}", sqlEx.Message));
                throw duplicateException;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ds;
        }


        public string ConnectionString
        {
            get;
            set;
        }

        //public string TableName
        //{
        //    get;
        //    set;
        //}


        //public int OrganizationId
        //{
        //    get;
        //    set;
        //}




        private string tableName;



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

        Cryptography cy = new Cryptography();


        public string GetConnectionStringFromDB(string DsName, string ConnectionString)
        {
            DataSet ds = null;
            SqlDatabase db = new SqlDatabase(ConnectionString);
            try
            {
                ds = db.ExecuteDataSet(CommandType.Text, "Select TOP 1 * From Datasource where DatasourceName     = '" +
                           DsName + "'");
            }
            catch (SqlException sqlEx)
            {
                Exception duplicateException = new Exception(string.Format("SQL Exception - {0}", sqlEx.Message));
                throw duplicateException;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            //return
            //string.Format("Data Source={0};Initial Catalog={1};Persist Security Info={2};User ID={3};Password={4}", ds.Tables[0].Rows[0]["DatasourceServerName"].ToString(), ds.Tables[0].Rows[0]["InitialCatalog"], ds.Tables[0].Rows[0]["PersistSecurityInfo"], ds.Tables[0].Rows[0]["DatabaseUserID"], ds.Tables[0].Rows[0]["Password"]);
            return Utilities.CreateConnectionString(DTO.DataBaseTypeEnum.SQLServer, new DataRow[] { ds.Tables[0].Rows[0] });

            //DataRow[] dr;
            //string qstr = Utilities.createConnectionString(DTO.DataBaseTypeEnum.SQLServer, dr);

            //return "
        }


        public bool CopyDashboard(string OldCanvasName, string NewCanvasName, string NewDatasourceName)
        {
            SqlDatabase db = new SqlDatabase(ConnectionString);
            SqlCommand Command = new SqlCommand();
            Command.CommandType = CommandType.StoredProcedure;
            Command.Parameters.AddWithValue("oldCanvasName", OldCanvasName);
            Command.Parameters.AddWithValue("newCanvasName", NewCanvasName);
            Command.Parameters.AddWithValue("newDatasourceName", NewDatasourceName);
            try
            {
                Command.CommandText = "usp_copy_canvas";
                int noOfRows = db.ExecuteNonQuery(Command);

                if (noOfRows < 0)
                {
                    return false;
                }

                return true;
            }
            catch (SqlException sqlEx)
            {
                Exception duplicateException = new Exception(string.Format("SQL Exception - {0}", sqlEx.Message));
                throw duplicateException;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


        public object ReadEWEDatasourceFormId(DTO.EWEDatasourceDto EWEDsDto)
        {
            SqlDatabase db = new SqlDatabase(this.ConnectionString);

            string sqlQuery = "SELECT SurveyId from EIDatasource where InitialCatalog = '" + EWEDsDto.DatabaseName + "'";



            try
            {
                object formobj = db.ExecuteScalar(CommandType.Text, sqlQuery);
                // string FormId = (formobj != null) ? formobj.ToString() : string.Empty;

                return formobj;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            // return string.Empty;
        }

        public DataSet ReadAllDatasourceUsers()
        {

            SqlDatabase db = new SqlDatabase(this.ConnectionString);

            string sqlQuery = "SELECT * from DatasourceUser";


            try
            {
                DataSet formobj = db.ExecuteDataSet(CommandType.Text, sqlQuery);

                return formobj;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


        //public List<int> ReadEwavDatasource(Guid DatasourceId)
        //{
        //    SqlDatabase db = new SqlDatabase(this.ConnectionString);

        //    string sqlQuery = "SELECT DatasourceId from Datasource where EIWSSurveyId = '" + DatasourceId + "'";



        //    try
        //    {
        //        object formobj = db.ExecuteScalar(CommandType.StoredProcedure, "");
        //        // string FormId = (formobj != null) ? formobj.ToString() : string.Empty;

        //        return Convert.ToInt16(formobj);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //    // return string.Empty;
        //}
    }
}