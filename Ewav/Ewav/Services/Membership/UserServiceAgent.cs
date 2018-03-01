/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       UserServiceAgent.cs
 *  Namespace:  Ewav.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.ServiceModel.DomainServices.Client;
using Ewav.Web.Services;
using System.Collections.Generic;
using System.Linq;
using Ewav.DTO;
using Ewav.ViewModels;



namespace Ewav.Services
{
    public class UserServiceAgent : IUserServiceAgent
    {
        #region Variables
        UserDomainContext userCtx;
        private Action<UserDTO, Exception> _completed;
        private Action<bool, Exception> _addCompleted;
        private Action<bool, Exception> _delCompleted;
        private Action<bool, Exception> _updateCompleted;
        private Action<List<UserDTO>, Exception> _readCompleted;
        private Action<bool, Exception> _forgotPwdCompleted;
        private Action<PasswordRulesDTO, Exception> _rulesReadCompleted;
        private Action<List<DTO.DatasourceDto>, Exception> _readDSCompleted;
        private Action<List<string>, Exception> _readUserNamesCompleted;
        private Action<UserDTO, Exception> _readUserByNameCompleted;
        #endregion

        #region Constructor
        public UserServiceAgent()
        {


        }
        #endregion

        #region Helper Method

        #endregion

        #region Completion Callbacks

        /// <summary>
        /// Reads all orgs for user.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="readAllOrgsForUserCompleted">The read all orgs for user completed.</param>
        public void ReadAllOrgsForUser(int userid, Action<List<OrganizationDto>, Exception> readAllOrgsForUserCompleted)
        {
            userCtx = new UserDomainContext();

            userCtx.ReadAllOrgsForUser(userid, result =>
            {
                Exception ex = null;
                if (result.HasError)
                {
                    result.MarkErrorAsHandled();
                    ex = result.Error;
                }

                List<OrganizationDto> returnedUserOrganizationDTO = ((InvokeOperation<List<OrganizationDto>>)result).Value;
                readAllOrgsForUserCompleted(returnedUserOrganizationDTO, ex);
            }, null);

        }

        void userResults_Completed(object sender, EventArgs e)
        {

            InvokeOperation<UserDTO> result = (InvokeOperation<UserDTO>)sender;
            Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                //throw new GadgetException(result.Error.Message);
                ex = result.Error;
            }
            else
            {
                UserDTO returnedUserDTO = ((InvokeOperation<UserDTO>)sender).Value;
                _completed(returnedUserDTO, ex);
            }
        }

        #endregion

        /// <summary>
        /// Used to load user dto at login  
        /// </summary>
        /// <param name="userDTO"></param>
        /// <param name="completed"></param>
        public void LoadUser(string UserName, Action<UserDTO, Exception> completed)
        {
            //   _completed = completed;
            userCtx = new UserDomainContext();

            userCtx.LoadUser(UserName, result =>
            {
                //    InvokeOperation<UserDTO> result = (InvokeOperation<UserDTO>)sender;
                Exception ex = null;
                if (result.HasError)
                {
                    result.MarkErrorAsHandled();
                    ex = result.Error;
                    throw new GadgetException(result.Error.Message);
                    
                }

                UserDTO returnedUserDTO = ((InvokeOperation<UserDTO>)result).Value;
                completed(returnedUserDTO, ex);
            }, null);
        }

        /// <summary>
        /// Used to load user dto at login  
        /// </summary>
        /// <param name="userDTO"></param>
        /// <param name="completed"></param>
        public void LoadUserFromActivedirectory(string EmailAddress, Action<UserDTO, Exception> completed)
        {
            //   _completed = completed;
            userCtx = new UserDomainContext();

            userCtx.LoadUserFromActivedirectory(ApplicationViewModel.Instance.CurrentUserDomain, EmailAddress, result =>
            {
                //    InvokeOperation<UserDTO> result = (InvokeOperation<UserDTO>)sender;
                Exception ex = null;
                if (result.HasError)
                {
                    result.MarkErrorAsHandled();
                    ex = result.Error;
                    throw new GadgetException(result.Error.Message);

                }

                UserDTO returnedUserDTO = ((InvokeOperation<UserDTO>)result).Value;
                completed(returnedUserDTO, ex);
            }, null);
        }


        public void GetUserForAuthentication(UserDTO userDTO, Action<UserDTO, Exception> completed)
        {
            //   _completed = completed;
            userCtx = new UserDomainContext();

            userCtx.GetUserForAuthentication(userDTO, result =>
            {
                //    InvokeOperation<UserDTO> result = (InvokeOperation<UserDTO>)sender;
                Exception ex = null;
                if (result.HasError)
                {
                    result.MarkErrorAsHandled();
                    //throw new GadgetException(result.Error.Message);
                    ex = result.Error;
                }

                UserDTO returnedUserDTO = ((InvokeOperation<UserDTO>)result).Value;
                completed(returnedUserDTO, ex);
            }, null);
        }


        public void AddUser(UserOrganizationDto dto, Action<bool, Exception> completed)
        {
            _addCompleted = completed;
            userCtx = new UserDomainContext();

            InvokeOperation<bool> addResults = userCtx.GenerateUser(dto);
            addResults.Completed += new EventHandler(addResults_Completed);
        }

        void addResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<bool> result =
                (InvokeOperation<bool>)sender;
            Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                //throw new GadgetException(result.Error.Message);
                ex = result.Error;
            }
            //else
            //{
            bool returnedData = ((InvokeOperation<bool>)sender).Value;
            _addCompleted(returnedData, ex);
            //}
        }

        public void UpdateUser(UserOrganizationDto dto, Action<bool, Exception> completed)
        {
            _updateCompleted = completed;
            userCtx = new UserDomainContext();

            InvokeOperation<bool> updateResults = userCtx.EditUser(dto);
            updateResults.Completed += new EventHandler(updateResults_Completed);
        }

        void updateResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<bool> result =
                (InvokeOperation<bool>)sender;
            Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                //throw new GadgetException(result.Error.Message);
                ex = result.Error;
            }
            //else
            //{
            bool returnedData = ((InvokeOperation<bool>)sender).Value;
            _updateCompleted(returnedData, ex);
            //}
        }

        public void DeleteUser(int userId, Action<bool, Exception> completed)
        {
            _delCompleted = completed;
            userCtx = new UserDomainContext();

            InvokeOperation<bool> deleteResults = userCtx.RemoveUser(userId);
            deleteResults.Completed += new EventHandler(deleteResults_Completed);
        }

        void deleteResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<bool> result =
                (InvokeOperation<bool>)sender;
            Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                //throw new GadgetException(result.Error.Message);
                ex = result.Error;
            }
            //else
            //{
            bool returnedData = ((InvokeOperation<bool>)sender).Value;
            _delCompleted(returnedData, ex);
            //}
        }

        public void ReadUser(int roleid, int orgnizationId, Action<System.Collections.Generic.List<UserDTO>, Exception> completed)
        {
            _readCompleted = completed;
            userCtx = new UserDomainContext();

            InvokeOperation<List<UserDTO>> readResults = userCtx.ReadUser(roleid, orgnizationId);
            readResults.Completed += new EventHandler(readResults_Completed);
        }

        void readResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<List<UserDTO>> result =
               (InvokeOperation<List<UserDTO>>)sender;
            Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                //throw new GadgetException(result.Error.Message);
                ex = result.Error;
            }
            //else
            //{
            List<UserDTO> returnedData = ((InvokeOperation<List<UserDTO>>)sender).Value;
            _readCompleted(returnedData, ex);
            //}
        }


        public void ForgotPassword(string email, Action<bool, Exception> completed)
        {
            _forgotPwdCompleted = completed;
            userCtx = new UserDomainContext();

            InvokeOperation<bool> forgotPwdResults = userCtx.ForgotPassword(email);
            forgotPwdResults.Completed += new EventHandler(forgotPwdResults_Completed);
        }

        void forgotPwdResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<bool> result =
               (InvokeOperation<bool>)sender;
            Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                //throw new GadgetException(result.Error.Message);
                ex = result.Error;
            }
            //else
            //{
            bool returnedData = ((InvokeOperation<bool>)sender).Value;
            _forgotPwdCompleted(returnedData, ex);
            //}
        }


        public void ReadPasswordRules(Action<PasswordRulesDTO, Exception> completed)
        {
            _rulesReadCompleted = completed;
            userCtx = new UserDomainContext();

            InvokeOperation<PasswordRulesDTO> readPassRulesResults = userCtx.ReadPasswordRules();
            readPassRulesResults.Completed += new EventHandler(readPassRulesResults_Completed);
        }

        void readPassRulesResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<PasswordRulesDTO> result =
                (InvokeOperation<PasswordRulesDTO>)sender;
            Exception ex = null;

            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                //throw new GadgetException(result.Error.Message);
                ex = result.Error;
            }
            //else
            //{
            PasswordRulesDTO returnedData = ((InvokeOperation<PasswordRulesDTO>)sender).Value;
            _rulesReadCompleted(returnedData, ex);
            //}
        }




        public void ReadAssociatedDatasources(int UserId, int OrganizationId, Action<List<DTO.DatasourceDto>, Exception> completed)
        {
            _readDSCompleted = completed;
            userCtx = new UserDomainContext();

            InvokeOperation<List<DTO.DatasourceDto>> readAssociatedDSResults = userCtx.ReadAssociatedDatasources(UserId, OrganizationId);
            readAssociatedDSResults.Completed += new EventHandler(readAssociatedDSResults_Completed);
        }

        void readAssociatedDSResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<List<DTO.DatasourceDto>> result =
                (InvokeOperation<List<DTO.DatasourceDto>>)sender;
            Exception ex = null;

            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                //throw new GadgetException(result.Error.Message);
                ex = result.Error;
            }
            //else
            //{
            List<DTO.DatasourceDto> returnedData = ((InvokeOperation<List<DTO.DatasourceDto>>)sender).Value;
            _readDSCompleted(returnedData, ex);
            //}
        }

        public void ReadUserNamesFromEwav(Action<List<string>, Exception> completed)
        {
            _readUserNamesCompleted = completed;
            userCtx = new UserDomainContext();

            InvokeOperation<IEnumerable<string>> readUserNamesResults = userCtx.ReadUserNamesFromEwav();
            readUserNamesResults.Completed += new EventHandler(readUserNamesResults_Completed);
        }

        void readUserNamesResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<IEnumerable<string>> result =
                 (InvokeOperation<IEnumerable<string>>)sender;
            Exception ex = null;

            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                //throw new GadgetException(result.Error.Message);
                ex = result.Error;
            }
            //else
            //{
            IEnumerable<string> returnedData = ((InvokeOperation<IEnumerable<string>>)sender).Value;
            _readUserNamesCompleted(returnedData.ToList(), ex);
            //}
        }


        public void ReadUserByUserName(string UserName, int OrganizationId, Action<UserDTO, Exception> completed)
        {
            _readUserByNameCompleted = completed;
            userCtx = new UserDomainContext();

            InvokeOperation<UserDTO> readUserResult = userCtx.ReadUserByUserName(UserName, OrganizationId);
            readUserResult.Completed += new EventHandler(readUserResult_Completed);
        }

        void readUserResult_Completed(object sender, EventArgs e)
        {
            InvokeOperation<UserDTO> result =
                 (InvokeOperation<UserDTO>)sender;
            Exception ex = null;

            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                //throw new GadgetException(result.Error.Message);
                ex = result.Error;
            }
            //else
            //{
            UserDTO returnedData = ((InvokeOperation<UserDTO>)sender).Value;
            _readUserByNameCompleted(returnedData, ex);
            //}
        }
    }
}