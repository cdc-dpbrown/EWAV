/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MySqlOrganizationDao.cs
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
using MySql.Data.MySqlClient;

namespace Ewav.DAL.MySqlLayer
{
    public class MySqlOrganizationDao : IOrganizationDao
    {
        public string ConnectionString { get; set; }

        public string TableName { get; set; }

        /// <summary>
        /// Adds the specified organization dto.
        /// </summary>
        /// <param name="organizationDto">The organization dto.</param>
        /// <param name="userDTO">The user DTO.</param>
        /// <returns></returns>
        public int AddOrganization(UserOrganizationDto userOrganizationDto)
        {
            MySqlConnection conn = new MySqlConnection(ConnectionString);
            conn.Open();

            OrganizationDto organizationDto = userOrganizationDto.Organization;
            UserDTO userDto = userOrganizationDto.User;

            MySqlCommand myCommand = new MySqlCommand();
            myCommand.Connection = conn;
            myCommand.CommandText = "usp_add_organization";
            myCommand.CommandType = CommandType.StoredProcedure;

            myCommand.Parameters.Add(new MySqlParameter("OrganizationName", organizationDto.Name));
            myCommand.Parameters.Add(new MySqlParameter("OrganizationDescription", organizationDto.Description));
            myCommand.Parameters.Add(new MySqlParameter("IsActive", userOrganizationDto.Active));

            myCommand.Parameters.Add(new MySqlParameter("UserId", userDto.UserID.ToString()));
            myCommand.Parameters.Add(new MySqlParameter("UserNm", userDto.UserName));
            myCommand.Parameters.Add(new MySqlParameter("FirstNm", userDto.FirstName));
            myCommand.Parameters.Add(new MySqlParameter("LastNm", userDto.LastName));
            myCommand.Parameters.Add(new MySqlParameter("EmailAdd", userDto.Email));
            myCommand.Parameters.Add(new MySqlParameter("PhoneNbr", userDto.Phone));
            myCommand.Parameters.Add(new MySqlParameter("PwdHash", userDto.PasswordHash));
            myCommand.Parameters.Add(new MySqlParameter("IsExistingUser", userDto.IsExistingUser));

            myCommand.Parameters.Add(new MySqlParameter("ResetPwd", userDto.ShouldResetPassword));
            myCommand.Parameters.Add(new MySqlParameter("RoleId", userOrganizationDto.RoleId));

            MySqlParameter p1 = new MySqlParameter();
            p1.ParameterName = "OrgId";
            p1.Direction = ParameterDirection.Output;

            myCommand.Parameters.Add(p1);

            myCommand.ExecuteNonQuery();

            //for (int i = 0; i < userOrganizationDto.User.Count; i++)
            //{
            try
            {
                myCommand = new MySqlCommand();
                myCommand.Connection = conn;
                myCommand.CommandText = "usp_add_user";
                myCommand.CommandType = CommandType.StoredProcedure;

                myCommand.Parameters.Add(new MySqlParameter("FirstName", userOrganizationDto.User.FirstName));
                myCommand.Parameters.Add(new MySqlParameter("LastName", userOrganizationDto.User.LastName));
                myCommand.Parameters.Add(new MySqlParameter("Phone", userOrganizationDto.User.Phone));
                myCommand.Parameters.Add(new MySqlParameter("Email", userOrganizationDto.User.Email));
                myCommand.Parameters.Add(new MySqlParameter("Role", 2));

                myCommand.ExecuteNonQuery();
            }
            catch (MySqlException sqlEx)
            {
                Exception duplicateException = new Exception(string.Format("SQL Exception - {0}", sqlEx.Message));
                throw duplicateException;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            //}

            return Int32.Parse(p1.Value.ToString());
        }

        public DataSet ReadAllOrganizations()
        {
            DataSet ds = null;

            ds = MySqlUtilities.ExecuteSimpleDatsetFromStoredProc(ConnectionString, " usp_read_all_organization_tallys ");

            return ds;
        }

        public DataSet ReadOrganization(int organizationID)
        {
            throw new NotImplementedException();
        }

        public bool RemoveOrganization(int organzationId)
        {
            try
            {
                // Query string    
                string query = string.Empty;

                //  Connection      
                MySqlConnection conn = new MySqlConnection(ConnectionString);
                conn.Open();

                MySqlCommand myCommand = new MySqlCommand();
                myCommand.Connection = conn;
                myCommand.CommandText = "usp_remove_organization";
                myCommand.CommandType = CommandType.StoredProcedure;

                myCommand.Parameters.Add(new MySqlParameter("OrganzationId", organzationId));

                MySqlParameter p1 = new MySqlParameter();
                p1.ParameterName = "Status";
                p1.Direction = ParameterDirection.Output;

                myCommand.Parameters.Add(p1);

                myCommand.ExecuteNonQuery();

                return Boolean.Parse(p1.Value.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool UpdateOrganization(OrganizationDto dto)
        {
            try
            {
                MySqlHelper.ExecuteScalar(ConnectionString, string.Format("call usp_update_organization ({0}, '{1}', {2} )",
                    dto.Id.ToString(), dto.Name.ToString(), dto.Active ? "1" : "0"));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}