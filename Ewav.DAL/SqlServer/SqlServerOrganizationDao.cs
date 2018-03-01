/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       SqlServerOrganizationDao.cs
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
using Ewav.Security;

namespace Ewav.DAL.SqlServer
{
    public class SqlServerOrganizationDao : IOrganizationDao
    {

        /// <summary>
        /// Adds new org and Admin User object
        /// </summary>
        /// <param name="dto"></param>
        public int AddOrganization(UserOrganizationDto userOrganizationDto)
        {
            OrganizationDto organizationDto = userOrganizationDto.Organization;
            Cryptography Cryptography = new Security.Cryptography();
            UserDTO userDto = userOrganizationDto.User;

            if (userOrganizationDto.User == null)
            {
                throw new Exception("An organization cannot be added with zero users");
            }

            int organizationID = -1;
            SqlDatabase db = new SqlDatabase(ConnectionString);

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {

                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "usp_add_organization";
                command.Parameters.Add(new SqlParameter("@OrganizationName", organizationDto.Name));
                command.Parameters.Add(new SqlParameter("@OrganizationDescription", ""));
                command.Parameters.Add(new SqlParameter("@OrganizationKey", Cryptography.Encrypt(Guid.NewGuid().ToString())));
                command.Parameters.Add(new SqlParameter("@UserId", userDto.UserID));
                command.Parameters.Add(new SqlParameter("@UserNm", userDto.UserName));
                command.Parameters.Add(new SqlParameter("@FirstNm", userDto.FirstName));
                command.Parameters.Add(new SqlParameter("@LastNm", userDto.LastName));
                command.Parameters.Add(new SqlParameter("@EmailAdd", userDto.Email));
                command.Parameters.Add(new SqlParameter("@PhoneNbr", userDto.Phone));
                command.Parameters.Add(new SqlParameter("@PwdHash", userDto.PasswordHash));
                command.Parameters.Add(new SqlParameter("@IsExistingUser", userDto.IsExistingUser));

                if (userDto.IsExistingUser)
                {
                    command.Parameters.Add(new SqlParameter("@ResetPwd", userDto.ShouldResetPassword));
                }
                else
                {
                    command.Parameters.Add(new SqlParameter("@ResetPwd", true));
                }


                command.Parameters.Add(new SqlParameter("@RoleId", userOrganizationDto.RoleId));
                command.Parameters.Add(new SqlParameter("@IsActive", userOrganizationDto.Active));

                try
                {
                    db.ExecuteNonQuery(command);
                    organizationID = 1000; //success
                }
                catch (Exception Ex)
                {

                    throw new Exception(Ex.Message);
                }

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
            SqlDatabase db = new SqlDatabase(ConnectionString);
            string query = string.Empty;
            try
            {
                db.ExecuteScalar("usp_remove_organization", organzationId);
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
            SqlDatabase db = new SqlDatabase(ConnectionString);
            DataSet ds = null;
            string query = string.Empty;
            try
            {



                ds = db.ExecuteDataSet("usp_read_all_organization_tallys");


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


        public string ConnectionString { get; set; }
        public string TableName { get; set; }
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
            SqlDatabase db = new SqlDatabase(ConnectionString);
            string query = string.Empty;
            try
            {


                db.ExecuteScalar("usp_update_organization", dto.Id, dto.Name, dto.Active);

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

            return false;
        }
    }
}