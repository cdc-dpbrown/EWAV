/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       PostgreSQLUserDao.cs
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
    public class PostgreSQLUserDao : IUserDao
    {
        public string ConnectionString { get; set; }
        public string TableName { get; set; }

        /// <summary>
        /// Reads the users for organization id.
        /// </summary>
        /// <param name="organizationId">The organization id.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public DataTable ReadAdminsForOrganizationId(int organizationId)
        {
            PostgreSQLDB db = new PostgreSQLDB(ConnectionString);

            try
            {
                NpgsqlCommand Command = new NpgsqlCommand();
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "read_admins";

                NpgsqlParameter parameter = new NpgsqlParameter("orgid", NpgsqlTypes.NpgsqlDbType.Integer);
                parameter.Value = organizationId;
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
        /// Gets the user for authentication.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="passwordHash">The password hash.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public DataTable GetUserForAuthentication(string userName, string passwordHash)
        {
            PostgreSQLDB db = new PostgreSQLDB(ConnectionString);

            try
            {
                NpgsqlCommand Command = new NpgsqlCommand();
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "read_user_for_authentication";

                NpgsqlParameter parameter = new NpgsqlParameter("uname", NpgsqlTypes.NpgsqlDbType.Varchar);
                parameter.Value = userName;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                parameter = new NpgsqlParameter("pwdhash", NpgsqlTypes.NpgsqlDbType.Varchar);
                parameter.Value = passwordHash;
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
        /// Loads the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public DataTable LoadUser(string userName)
        {
            PostgreSQLDB db = new PostgreSQLDB(ConnectionString);
            try
            {
                NpgsqlCommand Command = new NpgsqlCommand();
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "load_user";

                NpgsqlParameter parameter = new NpgsqlParameter("uname", NpgsqlTypes.NpgsqlDbType.Varchar);
                parameter.Value = userName;
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
        /// Adds the user.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public bool AddUser(UserOrganizationDto dto)
        {
            PostgreSQLDB db = new PostgreSQLDB(ConnectionString);

            UserDTO User = dto.User;

            NpgsqlCommand addUserCommand = new NpgsqlCommand();

            addUserCommand.CommandType = CommandType.StoredProcedure;
            addUserCommand.CommandText = "add_user";

            NpgsqlParameter parameter = new NpgsqlParameter("uname", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = User.UserName;
            parameter.Direction = ParameterDirection.Input;
            addUserCommand.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("fname", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = User.FirstName;
            parameter.Direction = ParameterDirection.Input;
            addUserCommand.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("lname", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = User.LastName;
            parameter.Direction = ParameterDirection.Input;
            addUserCommand.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("emailadd", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = User.Email;
            parameter.Direction = ParameterDirection.Input;
            addUserCommand.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("pnumber", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = User.Phone;
            parameter.Direction = ParameterDirection.Input;
            addUserCommand.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("phash", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = User.PasswordHash;
            parameter.Direction = ParameterDirection.Input;
            addUserCommand.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("rpassword", NpgsqlTypes.NpgsqlDbType.Boolean);
            parameter.Value = User.ShouldResetPassword;
            parameter.Direction = ParameterDirection.Input;
            addUserCommand.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("isexisting", NpgsqlTypes.NpgsqlDbType.Boolean);
            parameter.Value = User.IsExistingUser;
            parameter.Direction = ParameterDirection.Input;
            addUserCommand.Parameters.Add(parameter);

            if (User.IsExistingUser)
            {
                parameter = new NpgsqlParameter("uid", NpgsqlTypes.NpgsqlDbType.Integer);
                parameter.Value = User.UserID;
                parameter.Direction = ParameterDirection.Input;
                addUserCommand.Parameters.Add(parameter);
                //addUserCommand.Parameters.Add("UsrId", User.UserID);
            }
            else
            {
                parameter = new NpgsqlParameter("uid", NpgsqlTypes.NpgsqlDbType.Integer);
                parameter.Value = -1;
                parameter.Direction = ParameterDirection.Input;
                addUserCommand.Parameters.Add(parameter);
                //addUserCommand.Parameters.Add("UsrId", -1);
            }



            parameter = new NpgsqlParameter("orgid", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = dto.Organization.Id;
            parameter.Direction = ParameterDirection.Input;
            addUserCommand.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("rid", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = dto.RoleId;
            parameter.Direction = ParameterDirection.Input;
            addUserCommand.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("isactive", NpgsqlTypes.NpgsqlDbType.Boolean);
            parameter.Value = dto.Active;
            parameter.Direction = ParameterDirection.Input;
            addUserCommand.Parameters.Add(parameter);



            StringBuilder DSIds = new StringBuilder();

            foreach (DatasourceDto item in User.DatasourceList)
            {
                DSIds.Append(item.DatasourceId);
                DSIds.Append(",");
            }

            parameter = new NpgsqlParameter("dsids", NpgsqlTypes.NpgsqlDbType.Varchar);
            if (DSIds.ToString().Contains(","))
            {
                parameter.Value = DSIds.ToString().Substring(0, DSIds.ToString().Length - 1);
            }
            else
            {
                parameter.Value = "";
            }

            parameter.Direction = ParameterDirection.Input;
            addUserCommand.Parameters.Add(parameter);

            try
            {
                db.ExecuteNonQuery(addUserCommand);
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
                 return false;
            }


            return true;
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public bool UpdateUser(UserOrganizationDto dto)
        {
            PostgreSQLDB db = new PostgreSQLDB(ConnectionString);

            UserDTO User = dto.User;

            NpgsqlCommand Command = new NpgsqlCommand();

            NpgsqlParameter parameter = null;


            if (User.UserEditType == UserEditType.EditingUserInfo)
            {
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "update_user";

                parameter = new NpgsqlParameter("fname", NpgsqlTypes.NpgsqlDbType.Varchar);
                parameter.Value = User.FirstName;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                parameter = new NpgsqlParameter("lname", NpgsqlTypes.NpgsqlDbType.Varchar);
                parameter.Value = User.LastName;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                parameter = new NpgsqlParameter("emailadd", NpgsqlTypes.NpgsqlDbType.Varchar);
                parameter.Value = User.Email;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                parameter = new NpgsqlParameter("pnumber", NpgsqlTypes.NpgsqlDbType.Varchar);
                parameter.Value = User.Phone;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                parameter = new NpgsqlParameter("usid", NpgsqlTypes.NpgsqlDbType.Integer);
                parameter.Value = User.UserID;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);
                //addUserCommand.Parameters.Add("UsrId", User.UserID);

                parameter = new NpgsqlParameter("orid", NpgsqlTypes.NpgsqlDbType.Integer);
                parameter.Value = dto.Organization.Id;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                parameter = new NpgsqlParameter("active", NpgsqlTypes.NpgsqlDbType.Boolean);
                parameter.Value = dto.Active;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                parameter = new NpgsqlParameter("roleid", NpgsqlTypes.NpgsqlDbType.Integer);
                parameter.Value = dto.RoleId;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);



                StringBuilder DSIds = new StringBuilder();

                foreach (DatasourceDto item in User.DatasourceList)
                {
                    DSIds.Append(item.DatasourceId);
                    DSIds.Append(",");
                }

                parameter = new NpgsqlParameter("dsids", NpgsqlTypes.NpgsqlDbType.Varchar);
                if (DSIds.ToString().Contains(","))
                {
                    parameter.Value = DSIds.ToString().Substring(0, DSIds.ToString().Length - 1);
                }
                else
                {
                    parameter.Value = "";
                }
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);
            }
            else
            {
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "update_password";

                parameter = new NpgsqlParameter("uid", NpgsqlTypes.NpgsqlDbType.Integer);
                parameter.Value = User.UserID;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                parameter = new NpgsqlParameter("hpassword", NpgsqlTypes.NpgsqlDbType.Varchar);
                parameter.Value = User.PasswordHash;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);
            }
            try
            {
                db.ExecuteNonQuery(Command);
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Removes the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public bool RemoveUser(int userId)
        {
            PostgreSQLDB db = new PostgreSQLDB(ConnectionString);

            int flag = 0;
            try
            {
                flag = Convert.ToInt32(db.ExecuteScalar(null));
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

        /// <summary>
        /// Reads the user.
        /// </summary>
        /// <param name="roleid">The roleid.</param>
        /// <param name="organizationId">The organization id.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public DataSet ReadUser(int roleid, int organizationId)
        {
            PostgreSQLDB db = new PostgreSQLDB(ConnectionString);
            try
            {
                NpgsqlCommand Command = new NpgsqlCommand();
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "read_user";

                NpgsqlParameter parameter = new NpgsqlParameter("orgid", NpgsqlTypes.NpgsqlDbType.Integer);
                parameter.Value = organizationId;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                parameter = new NpgsqlParameter("uid", NpgsqlTypes.NpgsqlDbType.Integer);
                parameter.Value = -1;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                parameter = new NpgsqlParameter("email", NpgsqlTypes.NpgsqlDbType.Varchar);
                parameter.Value = "";
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                parameter = new NpgsqlParameter("rid", NpgsqlTypes.NpgsqlDbType.Integer);
                parameter.Value = roleid;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                DataSet ds = db.ExecuteDataSet(Command);

                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public bool ForgotPasswod(string email, string hashedPwd)
        {
            PostgreSQLDB db = new PostgreSQLDB(ConnectionString);
            DataSet ds;
            try
            {
                NpgsqlCommand Command = new NpgsqlCommand();
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "read_user";

                NpgsqlParameter parameter = new NpgsqlParameter("orgid", NpgsqlTypes.NpgsqlDbType.Integer);
                parameter.Value = -1;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                parameter = new NpgsqlParameter("uid", NpgsqlTypes.NpgsqlDbType.Integer);
                parameter.Value = -1;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                parameter = new NpgsqlParameter("email", NpgsqlTypes.NpgsqlDbType.Varchar);
                parameter.Value = email;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                parameter = new NpgsqlParameter("rid", NpgsqlTypes.NpgsqlDbType.Integer);
                parameter.Value = -1;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                ds = db.ExecuteDataSet(Command);

                //return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


            if (ds.Tables[0].Rows.Count > 0)
            {
                NpgsqlCommand Command = new NpgsqlCommand();
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "forgot_password";


                NpgsqlParameter parameter = new NpgsqlParameter("emailadd", NpgsqlTypes.NpgsqlDbType.Varchar);
                parameter.Value = ds.Tables[0].Rows[0]["EMAILADDRESS"];
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                parameter = new NpgsqlParameter("hpassword", NpgsqlTypes.NpgsqlDbType.Varchar);
                parameter.Value = hashedPwd;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);
                try
                {
                    db.ExecuteNonQuery(Command);
                }
                catch (Exception ex)
                {

                    //throw new Exception(ex.Message);
                    return false;
                }
                return true;
            }
            return false;
        }


        /// <summary>
        /// Reads the associated datasources.
        /// </summary>
        /// <param name="UserId">The user id.</param>
        /// <param name="OrganizationId">The organization id.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public DataSet ReadAssociatedDatasources(int UserId, int OrganizationId)
        {
            PostgreSQLDB db = new PostgreSQLDB(ConnectionString);
            try
            {
                NpgsqlCommand Command = new NpgsqlCommand();
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "read_datasource";

                NpgsqlParameter parameter = new NpgsqlParameter("orgid", NpgsqlTypes.NpgsqlDbType.Integer);
                parameter.Value = OrganizationId;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                parameter = new NpgsqlParameter("uid", NpgsqlTypes.NpgsqlDbType.Integer);
                parameter.Value = UserId;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);


                DataSet ds = db.ExecuteDataSet(Command);

                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// Reads the user names.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public DataSet ReadUserNames()
        {
            PostgreSQLDB db = new PostgreSQLDB(ConnectionString);
            try
            {
                NpgsqlCommand Command = new NpgsqlCommand();
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "read_usernames";


                DataSet ds = db.ExecuteDataSet(Command);

                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// Reads the name of the user by user.
        /// </summary>
        /// <param name="UserName">Name of the user.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public DataSet ReadUserByUserName(string UserName)
        {
            PostgreSQLDB db = new PostgreSQLDB(ConnectionString);
            try
            {
                NpgsqlCommand Command = new NpgsqlCommand();
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "read_by_username";

                NpgsqlParameter parameter = new NpgsqlParameter("orgid", NpgsqlTypes.NpgsqlDbType.Varchar);
                parameter.Value = UserName;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);


                DataSet ds = db.ExecuteDataSet(Command);

                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Reads all orgs for user.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public DataTable ReadAllOrgsForUser(int userID)
        {
            PostgreSQLDB db = new PostgreSQLDB(ConnectionString);
            try
            {
                NpgsqlCommand Command = new NpgsqlCommand();
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "read_all_organization_for_user";

                NpgsqlParameter parameter = new NpgsqlParameter("uid", NpgsqlTypes.NpgsqlDbType.Integer);
                parameter.Value = userID;
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
        /// Reads the super admin from ewav.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public string ReadSuperAdminFromEwav()
        {
            PostgreSQLDB db = new PostgreSQLDB(ConnectionString);
            try
            {
                NpgsqlCommand Command = new NpgsqlCommand();
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "read_super_admin_from_ewav";


                DataSet ds = db.ExecuteDataSet(Command);

                return ds.Tables[0].Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DataSet GetUser(int userid)
        {
            throw new NotImplementedException();
        }
    }
}