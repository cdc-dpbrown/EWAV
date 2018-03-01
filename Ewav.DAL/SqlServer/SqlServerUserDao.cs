/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       SqlServerUserDao.cs
 *  Namespace:  Ewav.DAL.SqlServer    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
namespace Ewav.DAL.SqlServer
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using Ewav.DAL.Interfaces;
    using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
    using Ewav.DTO;
    using System.Collections.Generic;
    using Microsoft.SqlServer.Server;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SqlServerUserDao : IUserDao
    {
        public string ConnectionString { get; set; }
        public string TableName { get; set; }
        /// <summary>
        /// Reads the users for organization id.
        /// </summary>
        /// <param name="organizationId">The organization id.</param>
        /// <returns></returns>
        public DataTable ReadAdminsForOrganizationId(int organizationId)
        {

            //SqlDatabase db = new SqlDatabase(ConnectionString);
            //string query = string.Format(
            //    "SELECT  *   FROM  [User]        WHERE    (  organizationId =  {0}  and RoleId  =  2    )  ", organizationId);

            //try
            //{
            //    DataSet ds = db.ExecuteDataSet(CommandType.Text, query);
            //    return ds.Tables[0];
            //}
            //catch (SqlException sqlEx)
            //{
            //    Exception duplicateException = new Exception(string.Format("SQL Exception - {0}", sqlEx.Message));
            //    throw duplicateException;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}

            DataSet ds = null;
            SqlDatabase db = new SqlDatabase(ConnectionString);
            try
            {
                ds = db.ExecuteDataSet("usp_read_admins", organizationId);
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

        public DataTable GetUserForAuthentication(string userName, string passwordHash)
        {
            SqlDatabase db = new SqlDatabase(ConnectionString);
            //string query = string.Format(
            //    "SELECT  *   FROM  [User] WHERE    ( UserName = '{0}'  ) AND    ( PasswordHash =  '{1}'    )  ", userName, passwordHash);

            Guid UGuid;
            if (Guid.TryParse(userName, out UGuid))
            {
                try
                {
                    DataSet ds = db.ExecuteDataSet("usp_read_user_bypass_authentication", UGuid);
                    return ds.Tables[0];
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
            else
            {
                try
                {
                    DataSet ds = db.ExecuteDataSet("usp_read_user_for_authentication", userName, passwordHash);
                    return ds.Tables[0];
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
        }


        public DataTable LoadUser(string userName)
        {
            SqlDatabase db = new SqlDatabase(ConnectionString);
            //string query = string.Format(
            //    "SELECT  UserId   FROM   [User]  WHERE    ( UserName = '{0}'  )  ", userName);
            //DataSet ds = db.ExecuteDataSet(CommandType.Text, query);

            //if (ds.Tables[0].Rows.Count > 0)
            //{
            //    string UserId = ds.Tables[0].Rows[0]["UserId"].ToString();
            //    query = "Select *, (Select Count(*) from DatasourceUser where DatasourceUser.UserId = " + UserId + ") as DatasourceCount from vwUserOrganizationUser where vwUserOrganizationUser.UserId = " + UserId + "  and Active = 'True' and IsorgActive = 'True' ";
            //}
            DataSet ds = null;
            try
            {
                ds = db.ExecuteDataSet("usp_load_user", userName);
                return ds.Tables[0];
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


        public bool AddUser(UserOrganizationDto dto)
        {
            SqlDatabase db = new SqlDatabase(ConnectionString);

            UserDTO User = dto.User;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                SqlCommand addUserCommand = connection.CreateCommand();

                addUserCommand.CommandType = CommandType.StoredProcedure;
                addUserCommand.CommandText = "usp_add_user";

                addUserCommand.Parameters.Add("UserName", User.UserName);
                addUserCommand.Parameters.Add("FirstName", User.FirstName);
                addUserCommand.Parameters.Add("LastName", User.LastName);
                addUserCommand.Parameters.Add("EmailAddress", User.Email);
                addUserCommand.Parameters.Add("PhoneNumber", User.Phone);
                addUserCommand.Parameters.Add("PasswordHash", User.PasswordHash);
                addUserCommand.Parameters.Add("ResetPassword", User.ShouldResetPassword);

                if (User.IsExistingUser)
                {
                    addUserCommand.Parameters.Add("UsrId", User.UserID);
                }
                else
                {
                    addUserCommand.Parameters.Add("UsrId", -1);
                }

                addUserCommand.Parameters.Add("OrganizationId", dto.Organization.Id);
                addUserCommand.Parameters.Add("RoleId", dto.RoleId);
                addUserCommand.Parameters.Add("Active", dto.Active);

                addUserCommand.Parameters.Add("UGuid", Guid.NewGuid());

                addUserCommand.Parameters.Add("@DatasourceUser", SqlDbType.Structured);
                addUserCommand.Parameters["@DatasourceUser"].Direction = ParameterDirection.Input;
                addUserCommand.Parameters["@DatasourceUser"].TypeName = "DatasourceUserTableType";

                List<SqlDataRecord> sqlDrList = new List<SqlDataRecord>();

                SqlDataRecord sqdr;




                foreach (DatasourceDto item in User.DatasourceList)
                {
                    sqdr = new SqlDataRecord(new SqlMetaData[] 
                    { new SqlMetaData("DatasourceID", SqlDbType.Int ), 
                       new SqlMetaData("UserID", SqlDbType.Int)      
                    });


                    // Set the record fields.
                    sqdr.SetInt32(0, item.DatasourceId);
                    sqdr.SetInt32(1, 0);

                    sqlDrList.Add(sqdr);
                }


                if (User.DatasourceList.Count == 0)
                    addUserCommand.Parameters["@DatasourceUser"].Value = null;
                else
                    addUserCommand.Parameters["@DatasourceUser"].Value = sqlDrList;

                try
                {
                    addUserCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                    return false;
                }

                //try
                //{
                //    if (!User.IsExistingUser)
                //    {
                //        addUserCommand.CommandType = CommandType.StoredProcedure;
                //        addUserCommand.CommandText = "usp_add_user";//TBD
                //        addUserCommand.Parameters.Add("UserName", User.UserName);
                //        addUserCommand.Parameters.Add("FirstName", User.FirstName);
                //        addUserCommand.Parameters.Add("LastName", User.LastName);
                //        //addUserCommand.Parameters.Add("OrganizationId", dto.OrganizationID);
                //        //addUserCommand.Parameters.Add("RoleID", dto.RoleValue);
                //        addUserCommand.Parameters.Add("EmailAddress", User.Email);
                //        addUserCommand.Parameters.Add("PhoneNumber", User.Phone);
                //        addUserCommand.Parameters.Add("PasswordHash", User.PasswordHash);
                //        addUserCommand.Parameters.Add("ResetPassword", true);
                //        //addUserCommand.Parameters.Add("Active", dto.IsActive);
                //        addUserCommand.Parameters.Add(new SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int) { Direction = ParameterDirection.Output });

                //        addUserCommand.ExecuteNonQuery();

                //        User.UserID = (int)addUserCommand.Parameters["@RETURN_VALUE"].Value;

                //        if (User.UserID < 0)
                //        {
                //            return false;
                //        }
                //    }



                //    //if (flag > -1)
                //    //{
                //    for (int i = 0; i < User.DatasourceList.Count; i++)
                //    {
                //        addDScommand = connection.CreateCommand();
                //        //addDScommand.Transaction = sqlTran;
                //        try
                //        {
                //            //flag = Convert.ToInt32(db.ExecuteScalar("usp_add_datasource", dto.UserID, dto.DatasourceList[i]));
                //            //if (flag < 0)
                //            //{
                //            //    return false;
                //            //}

                //            addDScommand.CommandType = CommandType.StoredProcedure;
                //            addDScommand.CommandText = "usp_add_user_datasource";
                //            addDScommand.Parameters.Add("UserName", User.UserName);
                //            addDScommand.Parameters.Add("DatasourceName", User.DatasourceList[i].DatasourceName);
                //            //addDScommand.Parameters.Add(new SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int) { Direction = ParameterDirection.Output });
                //            addDScommand.ExecuteNonQuery();

                //            //flag = (int)addDScommand.Parameters["@RETURN_VALUE"].Value;

                //        }
                //        catch (SqlException sqlEx)
                //        {
                //            // sqlTran.Rollback();
                //            Exception duplicateException = new Exception(string.Format("SQL Exception - {0}", sqlEx.Message));
                //            throw duplicateException;
                //        }
                //        catch (Exception ex)
                //        {
                //            //sqlTran.Rollback();
                //            throw new Exception(ex.Message);
                //        }
                //    }

                //    //if (flag > -1)
                //    //{
                //    //    // sqlTran.Commit();
                //    //    return true;
                //    //}

                //    //}

                //    addUserCommand = connection.CreateCommand();
                //    // addUserCommand.Transaction = sqlTran;

                //    //updateUserCommand.CommandType = CommandType.StoredProcedure;
                //    //updateUserCommand.CommandText = "usp_update_user";
                //    //updateUserCommand.Parameters.Add(new SqlParameter("@FirstName", User.FirstName));
                //    //updateUserCommand.Parameters.Add(new SqlParameter("@LastName", User.LastName));
                //    //updateUserCommand.Parameters.Add(new SqlParameter("@OrganizationID", organizationID));
                //    //updateUserCommand.Parameters.Add(new SqlParameter("@RoleID", User.RoleValue));
                //    //updateUserCommand.Parameters.Add(new SqlParameter("@EmailAddress", User.Email));
                //    //updateUserCommand.Parameters.Add(new SqlParameter("@PhoneNumber", User.Phone));
                //    //updateUserCommand.Parameters.Add(new SqlParameter("@UserId", User.UserID));
                //    //updateUserCommand.Parameters.Add(new SqlParameter("@Active", User.IsActive));

                //    //updateUserCommand.Parameters.Add(new SqlParameter("@RETURN_VALUE", SqlDbType.Int) { Direction = ParameterDirection.Output });

                //    addUserCommand.CommandType = CommandType.StoredProcedure;
                //    addUserCommand.CommandText = "usp_add_user_organization";
                //    addUserCommand.Parameters.Add(new SqlParameter("@UserId", User.UserID));
                //    addUserCommand.Parameters.Add(new SqlParameter("@OrganizationID", dto.Organization.Id));
                //    addUserCommand.Parameters.Add(new SqlParameter("@RoleID", dto.RoleId));
                //    addUserCommand.Parameters.Add(new SqlParameter("@Active", dto.Active));

                //    //  SqlParameter retvalUser = new SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int);
                //    //  retvalUser.Direction = System.Data.ParameterDirection.ReturnValue;
                //    //  addUserCommand.Parameters.Add(retvalUser);

                //    addUserCommand.ExecuteNonQuery();
                //}
                //catch (Exception e)
                //{
                //    //  sqlTran.Rollback();
                //    throw new Exception(e.Message);
                //}
            }
            return true;
            //return false;
        }

        public bool UpdateUser(UserOrganizationDto dto)
        {
            SqlDatabase db = new SqlDatabase(ConnectionString);
            //SqlTransaction sqlTran = null;
            //int flag = -1;
            UserDTO User = dto.User;
            OrganizationDto Organization = dto.Organization;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand updUsercommand = connection.CreateCommand();

                if (User.UserEditType == UserEditType.EditingUserInfo)
                {
                    updUsercommand.CommandType = CommandType.StoredProcedure;
                    updUsercommand.CommandText = "usp_update_user";
                    updUsercommand.Parameters.Add("FirstName", User.FirstName);
                    updUsercommand.Parameters.Add("LastName", User.LastName);
                    updUsercommand.Parameters.Add("EmailAddress", User.Email);
                    updUsercommand.Parameters.Add("PhoneNumber", User.Phone);
                    updUsercommand.Parameters.Add("UserId", User.UserID);
                    updUsercommand.Parameters.Add("OrganizationId", dto.Organization.Id);
                    updUsercommand.Parameters.Add("IsUserOrgActive", dto.Active);
                    updUsercommand.Parameters.Add("RoleId", dto.RoleId);

                    updUsercommand.Parameters.Add("@DatasourceUser", SqlDbType.Structured);
                    updUsercommand.Parameters["@DatasourceUser"].Direction = ParameterDirection.Input;
                    updUsercommand.Parameters["@DatasourceUser"].TypeName = "DatasourceUserTableType";

                    List<SqlDataRecord> sqlDrList = new List<SqlDataRecord>();

                    SqlDataRecord sqdr;




                    foreach (DatasourceDto item in User.DatasourceList)
                    {
                        sqdr = new SqlDataRecord(new SqlMetaData[] 
                    { new SqlMetaData("DatasourceID", SqlDbType.Int ), 
                       new SqlMetaData("UserID", SqlDbType.Int)      
                    });


                        // Set the record fields.
                        sqdr.SetInt32(0, item.DatasourceId);
                        sqdr.SetInt32(1, User.UserID);

                        sqlDrList.Add(sqdr);
                    }


                    if (User.DatasourceList.Count == 0)
                        updUsercommand.Parameters["@DatasourceUser"].Value = null;
                    else
                        updUsercommand.Parameters["@DatasourceUser"].Value = sqlDrList;

                }
                else
                {


                    updUsercommand.CommandType = CommandType.StoredProcedure;
                    updUsercommand.CommandText = "usp_update_password";
                    updUsercommand.Parameters.Add("UserId", User.UserID);
                    updUsercommand.Parameters.Add("HashedPassword", User.PasswordHash);

                }
                try
                {
                    updUsercommand.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }

                return true;
            }

        }

        public bool RemoveUser(int userId)
        {
            SqlDatabase db = new SqlDatabase(ConnectionString);
            int flag = 0;
            try
            {
                flag = Convert.ToInt32(db.ExecuteScalar("usp_delete_user", userId));
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
            if (flag == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public DataSet GetUser(int userid)
        {
            DataSet ds = null;
            SqlDatabase db = new SqlDatabase(ConnectionString);
            try
            {

                ds = db.ExecuteDataSet(CommandType.Text, "select * from  [user]  where  userid = " + userid);


            }
            catch (Exception ex)
            {


            }

            return ds;

        }

        public DataSet ReadUser(int roleid, int organizationId)
        {
            DataSet ds = null;
            SqlDatabase db = new SqlDatabase(ConnectionString);


            try
            {
                ds = db.ExecuteDataSet("usp_read_user", organizationId, -1, " ", roleid);
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


        public bool ForgotPasswod(string email, string hashedPwd)
        {
            SqlDatabase db = new SqlDatabase(ConnectionString);
            DataSet ds = null;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();


                SqlCommand readUserComman = connection.CreateCommand();
                SqlCommand updateUserCommand = connection.CreateCommand();

                //readUserComman.Transaction = sqlTran;
                //updateUserCommand.Transaction = sqlTran;

                try
                {
                    readUserComman.CommandType = CommandType.StoredProcedure;
                    readUserComman.CommandText = "usp_read_user";
                    readUserComman.Parameters.Add("orgid", -1);
                    readUserComman.Parameters.Add("userid", -1);
                    readUserComman.Parameters.Add("email", email);
                    readUserComman.Parameters.Add("roleid", -1);
                    //readUserComman.Parameters.Add("datasourceid", -1);
                    //readUserComman.Parameters.Add(new SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int) { Direction = ParameterDirection.ReturnValue });

                    //ds = readUserComman.execute

                    ds = db.ExecuteDataSet(readUserComman);//, sqlTran);

                    //flag = (int)addUserCommand.Parameters["@RETURN_VALUE"].Value;

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        updateUserCommand = connection.CreateCommand();
                        //updateUserCommand.Transaction = sqlTran;
                        try
                        {
                            //flag = Convert.ToInt32(db.ExecuteScalar("usp_update_user", dto.UserID, dto.DatasourceList[i]));
                            //if (flag < 0)
                            //{
                            //    return false;
                            //}

                            updateUserCommand.CommandType = CommandType.StoredProcedure;
                            updateUserCommand.CommandText = "usp_forgot_password";
                            //updateUserCommand.Parameters.Add("FirstName", ds.Tables[0].Rows[0]["FIRSTNAME"]);
                            //updateUserCommand.Parameters.Add("LastName", ds.Tables[0].Rows[0]["LASTNAME"]);
                            //updateUserCommand.Parameters.Add("ORGANIZATIONID", ds.Tables[0].Rows[0]["ORGANIZATIONID"]);
                            //updateUserCommand.Parameters.Add("ROLEID", ds.Tables[0].Rows[0]["ROLEID"]);
                            updateUserCommand.Parameters.Add("EmailAddress", ds.Tables[0].Rows[0]["EMAILADDRESS"]);
                            //updateUserCommand.Parameters.Add("PHONENUMBER", ds.Tables[0].Rows[0]["PHONENUMBER"]);
                            updateUserCommand.Parameters.Add("HashedPassword", hashedPwd);
                            //updateUserCommand.Parameters.Add("ResetPassword", true);
                            //updateUserCommand.Parameters.Add("Original_UserID", ds.Tables[0].Rows[0]["USERID"]);
                            //updateUserCommand.Parameters.Add(new SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int) { Direction = ParameterDirection.Output });
                            updateUserCommand.ExecuteNonQuery();

                            //flag = (int)updateUserCommand.Parameters["@RETURN_VALUE"].Value;

                            //if (flag > -1)
                            //{

                            //}

                        }
                        catch (SqlException sqlEx)
                        {
                            Exception duplicateException = new Exception(string.Format("SQL Exception - {0}", sqlEx.Message));
                            // sqlTran.Rollback();
                            throw duplicateException;
                        }
                        catch (Exception ex)
                        {
                            //sqlTran.Rollback();
                            throw new Exception(ex.Message);
                        }

                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    //sqlTran.Rollback();
                    throw new Exception(e.Message);
                }


                //try
                //{
                //    ds = db.ExecuteDataSet("usp_read_user", -1, -1, email);
                //}
                //catch (SqlException sqlEx)
                //{
                //    Exception duplicateException = new Exception(string.Format("SQL Exception - {0}", sqlEx.Message));
                //    throw duplicateException;
                //}
                //catch (Exception ex)
                //{
                //    throw new Exception(ex.Message);
                //}

                //if (ds.Tables[0].Rows.Count < 1)
                //{
                //    throw new Exception("User doesn't exists in the datatbase. Please check with your Administrator.");
                //}

                //try
                //{
                //    db.ExecuteScalar("usp_update_user", ds.Tables[0].Rows[0]["FIRSTNAME"], ds.Tables[0].Rows[0]["LASTNAME"], ds.Tables[0].Rows[0]["ORGANIZATIONID"], ds.Tables[0].Rows[0]["ROLEID"], ds.Tables[0].Rows[0]["EMAILADDRESS"], ds.Tables[0].Rows[0]["PHONENUMBER"], hashedPwd, true, ds.Tables[0].Rows[0]["USERID"]);
                //}
                //catch (SqlException sqlEx)
                //{
                //    Exception duplicateException = new Exception(string.Format("SQL Exception - {0}", sqlEx.Message));
                //    throw duplicateException;
                //}
                //catch (Exception ex)
                //{
                //    throw new Exception(ex.Message);
                //}

                //sqlTran.Commit();
                return true;

            }
        }


        public DataSet ReadAssociatedDatasources(int UserId, int OrganizationId)
        {
            DataSet ds = null;
            SqlDatabase db = new SqlDatabase(ConnectionString);
            try
            {
                ds = db.ExecuteDataSet("usp_read_datasource", OrganizationId, UserId);//TBD
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


        public DataSet ReadUserNames()
        {
            DataSet ds = null;

            SqlDatabase db = new SqlDatabase(ConnectionString);
            try
            {
                ds = db.ExecuteDataSet("usp_read_usernames");
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


        public DataSet ReadUserByUserName(string UserName)
        {
            DataSet ds = null;

            SqlDatabase db = new SqlDatabase(ConnectionString);
            try
            {
                //ds = db.ExecuteDataSet(CommandType.Text, "select * from [User] left join UserOrganization on [User].UserId = UserOrganization.UserId Where UserName = '" + UserName + "'");
                ds = db.ExecuteDataSet("usp_read_by_username", UserName);
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

        public DataTable ReadAllOrgsForUser(int userID)
        {

            DataSet ds = null;

            SqlDatabase db = new SqlDatabase(ConnectionString);
            try
            {
                //ds = db.ExecuteDataSet(CommandType.Text, "Select * From   vwOrgsforUser  Where RoleId > 1 AND UserId  =  " + userID + " AND Active = 1 AND IsOrgActive = 1");
                ds = db.ExecuteDataSet("usp_read_all_organization_for_user", userID);
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


        /// <summary>
        /// Reads the super admin from ewav.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public string ReadSuperAdminFromEwav()
        {
            DataSet ds = null;

            SqlDatabase db = new SqlDatabase(ConnectionString);
            try
            {
                //ds = db.ExecuteDataSet(CommandType.Text, "Select TOP 1 EmailAddress From vwUserOrganizationuser Where RoleID = 4 ");
                ds = db.ExecuteDataSet("usp_read_super_admin_from_ewav");
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

            return ds.Tables[0].Rows[0][0].ToString();
        }
    }
}