/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       IUserServiceAgent.cs
 *  Namespace:  Ewav.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using Ewav.DTO;

namespace Ewav.Services
{
    public interface IUserServiceAgent
    {
        void GetUserForAuthentication(UserDTO userDTO, Action<UserDTO, Exception> completed);
        void AddUser(UserOrganizationDto dto, Action<bool, Exception> completed);
        void UpdateUser(UserOrganizationDto dto, Action<bool, Exception> completed);
        void DeleteUser(int userId, Action<bool, Exception> completed);
        void ReadUser(int roleid, int orgnizationId, Action<List<UserDTO>, Exception> completed);
        void ForgotPassword(string email, Action<bool, Exception> completed);
        void ReadPasswordRules(Action<PasswordRulesDTO, Exception> completed);
        void ReadAssociatedDatasources(int UserId, int OrganizationId, Action<List<DatasourceDto>, Exception> completed);
        void ReadUserNamesFromEwav(Action<List<string>, Exception> completed);
        void ReadUserByUserName(string UserName, int OrganizationId, Action<UserDTO, Exception> completed);
    }





}