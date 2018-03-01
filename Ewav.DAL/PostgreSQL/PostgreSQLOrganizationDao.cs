/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       PostgreSQLOrganizationDao.cs
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
using Ewav.DTO;
using Npgsql;
using System.Data;

namespace Ewav.DAL.PostgreSQL
{
    public class PostgreSQLOrganizationDao : IOrganizationDao
    {

        public string ConnectionString { get; set; }
        public string TableName { get; set; }

        /// <summary>
        /// Adds new org and Admin User object
        /// </summary>
        /// <param name="dto"></param>
        public int AddOrganization(UserOrganizationDto userOrganizationDto)
        {
            OrganizationDto organizationDto = userOrganizationDto.Organization;

            UserDTO userDto = userOrganizationDto.User;

            if (userOrganizationDto.User == null)
            {
                throw new Exception("An organization cannot be added with zero users");
            }

            int organizationID = -1;
            PostgreSQLDB db = new PostgreSQLDB(ConnectionString);

            NpgsqlCommand Command = new NpgsqlCommand();
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "add_organization";

            NpgsqlParameter parameter = new NpgsqlParameter("orgname", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = organizationDto.Name;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("orgdescription", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = organizationDto.Description;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("Usid", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = userDto.UserID;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("UserNm", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = userDto.UserName;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("FirstNm", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = userDto.FirstName;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("LastNm", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = userDto.LastName;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("EmailAdd", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = userDto.Email;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("phonenbr", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = userDto.Phone;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("PwdHash", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = userDto.PasswordHash;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("IsExistingUser", NpgsqlTypes.NpgsqlDbType.Boolean);
            parameter.Value = userDto.IsExistingUser;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            

            if (userDto.IsExistingUser)
            {
                parameter = new NpgsqlParameter("ResetPwd", NpgsqlTypes.NpgsqlDbType.Boolean);
                parameter.Value = userDto.ShouldResetPassword;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);
            }
            else
            {
                parameter = new NpgsqlParameter("ResetPwd", NpgsqlTypes.NpgsqlDbType.Boolean);
                parameter.Value = true;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);
            }

            parameter = new NpgsqlParameter("RId", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = userOrganizationDto.RoleId;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("IsActive", NpgsqlTypes.NpgsqlDbType.Boolean);
            parameter.Value = userOrganizationDto.Active;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

                try
                {
                    db.ExecuteNonQuery(Command);
                    organizationID = 1000; //success
                }
                catch (Exception Ex)
                {

                    throw new Exception(Ex.Message);
                }


            return organizationID;
        }

        /// <summary>
        /// does a soft delete on the organization
        /// </summary>
        /// <param name="organzationId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool RemoveOrganization(int organzationId)
        {
            PostgreSQLDB db = new PostgreSQLDB(ConnectionString);

            NpgsqlCommand Command = new NpgsqlCommand();

            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "remove_organization";

            NpgsqlParameter parameter = new NpgsqlParameter("orgid", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = organzationId;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);


            try
            {
                db.ExecuteScalar(Command);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        /// <summary>
        /// reads all the organization
        /// there is no apparent reason for the user to read his organization only. 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public DataSet ReadAllOrganizations()
        {

            PostgreSQLDB db = new PostgreSQLDB(ConnectionString);

            NpgsqlCommand Command = new NpgsqlCommand();

            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "read_all_organization_tallys";

            DataSet ds = null;
            string query = string.Empty;
            try
            {
                ds = db.ExecuteDataSet(Command);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ds;
        }


        /// <summary>
        /// Reads the specified organization ID.
        /// </summary>
        /// <param name="organizationID">The organization ID.</param>
        /// <returns></returns>
        public DataSet ReadOrganization(int organizationID)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }


        /// <summary>
        /// Updates the organization Info, updates the name
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public bool UpdateOrganization(OrganizationDto dto)
        {

            PostgreSQLDB db = new PostgreSQLDB(ConnectionString);

            NpgsqlCommand Command = new NpgsqlCommand();

            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "update_organization";

            NpgsqlParameter parameter = new NpgsqlParameter("orgid", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = dto.Id;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("orgname", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = dto.Name;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("isactive", NpgsqlTypes.NpgsqlDbType.Boolean);
            parameter.Value = dto.Active;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);
            
            try
            {
                db.ExecuteNonQuery(Command);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }
    }
}