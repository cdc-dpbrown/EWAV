/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       IUserDao.cs
 *  Namespace:  Ewav.DAL.Interfaces    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace Ewav.DAL.Interfaces
{
    using System;
    using System.Data;
    using System.Collections.Generic;
    using Ewav.DTO;

    public interface IUserDao
    {
        DataSet GetUser(int userid);

        /// <summary>
        /// Reads all orgs for user.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <returns></returns>
        DataTable ReadAllOrgsForUser(int userID);

        /// <summary>
        /// Reads the users for organization id.
        /// </summary>
        /// <param name="organizationId">The organization id.</param>
        /// <returns></returns>
        DataTable ReadAdminsForOrganizationId(int organizationId);

        string ConnectionString { get; set; }
        string TableName { get; set; }
        DataTable GetUserForAuthentication(string userID, string passwordHash);
        DataTable LoadUser(string userID);
        bool AddUser(UserOrganizationDto dto);
        bool UpdateUser(UserOrganizationDto dto);
        bool RemoveUser(int userId);
        DataSet ReadUser(int roleid, int organizationId);
        bool ForgotPasswod(string email, string hashedPwd);
        DataSet ReadAssociatedDatasources(int UserId, int OrganizationId);
        DataSet ReadUserNames();
        DataSet ReadUserByUserName(string UserName);
        string ReadSuperAdminFromEwav();
    }
}