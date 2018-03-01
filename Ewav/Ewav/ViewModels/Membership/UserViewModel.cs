/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       UserViewModel.cs
 *  Namespace:  Ewav.ViewModels.Membership    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using SimpleMvvmToolkit;
using SimpleMvvmToolkit.ModelExtensions;

using System.Collections.Generic;
using Ewav.Services;

using Ewav.DTO;

namespace Ewav.ViewModels.Membership
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class UserViewModel : ViewModelBase<UserViewModel>
    {
        #region Initialization and Cleanup

        // TODO: Add a member for IXxxServiceAgent
        private IUserServiceAgent serviceAgent;

        private UserDTO authenticatedUser;

        private UserDTO loadedUser;

        // Default ctor
        public UserViewModel()
        {
        }

        // TODO: ctor that accepts IXxxServiceAgent
        public UserViewModel(IUserServiceAgent serviceAgent)
        {
            this.serviceAgent = serviceAgent;
        }

        #endregion

        #region Notifications

        public PasswordRulesDTO PasswordRules { get; set; }

        public List<UserDTO> Users { get; set; }

        /// <summary>
        /// Gets or sets the loaded user.
        /// </summary>
        /// <value>The loaded user.</value>
        public UserDTO LoadedUser
        {
            get
            {
                return this.loadedUser;
            }
            set
            {
                this.loadedUser = value;
            }
        }

        /// <summary>
        /// Loads the user.
        /// </summary>
        /// <param name="userDto">The user dto.</param>
        public void LoadUser(string UserName)
        {

            UserServiceAgent usa = new UserServiceAgent();
            usa.LoadUser(UserName, UserLoadedComplete);

        }

        /// <summary>
        /// Loads the user from activedirectory.
        /// </summary>
        /// <param name="EmailAddress">The email address.</param>
        public void LoadUserFromActivedirectory(string EmailAddress)
        {
            UserServiceAgent usa = new UserServiceAgent();
            usa.LoadUserFromActivedirectory(EmailAddress, UserFromADLoadedComplete);
        }

        /// <summary>
        /// Reads all orgs for user.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public void ReadAllOrgsForUser(int userID)
        {
            UserServiceAgent usa = new UserServiceAgent();
            usa.ReadAllOrgsForUser(userID, ReadAllOrgsForUserCompleted);
        }

        /// <summary>
        /// Reads all orgs for user completed.
        /// </summary>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        private void ReadAllOrgsForUserCompleted(List<OrganizationDto> userOrganizationsList, Exception e)
        {


            ReadAllOrgsForUserComplete(userOrganizationsList, null);

        }

        /// <summary>
        /// Gets or sets the authenticate resuts.
        /// </summary>
        /// <value>The authenticate resuts.</value>
        public UserDTO AuthenticatedUser
        {
            get
            {
                return this.authenticatedUser;
            }
            set
            {
                this.authenticatedUser = value;
            }
        }

        //public List<string> UserNames { get; set; }

        // TODO: Add events to notify the view or obtain data from the view
        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;
        public delegate void ReadAllOrgsForUserCompleteHandler(List<OrganizationDto> UserOrganizationDtoList, Exception ex);
        public event ReadAllOrgsForUserCompleteHandler ReadAllOrgsForUserComplete;



        /// <summary>
        /// fired when a user is loaded     
        /// </summary>    
        public event EventHandler UserLoaded;
        public event EventHandler UserNotAuthenticated;
        public event EventHandler UserLoadedFromAD;
        public event EventHandler UserAuthenticated;
        public event EventHandler UserUpdated;
        public event EventHandler UserAdded;
        public event EventHandler UserAddedFailed;
        public event EventHandler ForgotPasswordLoaded;
        public event EventHandler ForgotPasswordFailed;
        public event EventHandler PasswordRulesReadLoaded;
        public event EventHandler ReadAllUsersLoaded;
        public event EventHandler DeleteUserLoaded;
        public event EventHandler ReadAssociatedDSLoaded;
        public event EventHandler ReadUserNamesLoaded;
        public event EventHandler ReadUserByNameLoaded;
        #endregion

        #region Properties

        // TODO: Add properties using the mvvmprop code snippet


        #endregion

        #region Methods

        public void DeleteUser(int userid)
        {
            UserServiceAgent usa = new UserServiceAgent();
            usa.DeleteUser(userid, DeleteUserCompleted);
        }

        public void ReadUser(int roleid, int orgId)
        {
            UserServiceAgent usa = new UserServiceAgent();
            usa.ReadUser(roleid, orgId, ReadUserCompleted);

            //ReadUserNamesFromEwav();
        }

        public void ReadPasswordRules()
        {
            UserServiceAgent usa = new UserServiceAgent();
            usa.ReadPasswordRules(PasswordRulesReadCompleted);
        }



        /// <summary>
        /// Forgot password
        /// </summary>
        /// <param name="email"></param>
        public void ForgotPassword(string email)
        {
            UserServiceAgent usa = new UserServiceAgent();
            usa.ForgotPassword(email, ForgotPasswordCompleted);
        }

        /// <summary>
        /// Authenticates the specified user dto.
        /// </summary>
        /// <param name="userDto">The user dto.</param>
        public void Authenticate(UserDTO userDto)
        {

            UserServiceAgent usa = new UserServiceAgent();
            usa.GetUserForAuthentication(userDto, UserAuthenticatedComplete);


        }


        public void UpdateUser(UserOrganizationDto dto)
        {
            UserServiceAgent usa = new UserServiceAgent();
            usa.UpdateUser(dto, UpdateUserCompleted);
        }

        public void AddUser(UserOrganizationDto dto)
        {
            UserServiceAgent usa = new UserServiceAgent();
            usa.AddUser(dto, AddUserCompleted);
        }

        public void ReadAssociatedDatasources(int UserId, int OrganizationId)
        {
            UserServiceAgent usa = new UserServiceAgent();
            usa.ReadAssociatedDatasources(UserId, OrganizationId, ReadAssociatedDatasourcesCompleted);
        }

        public void ReadUserNamesFromEwav()
        {
            UserServiceAgent usa = new UserServiceAgent();
            usa.ReadUserNamesFromEwav(ReadUserNamesFromEwavCompleted);
        }

        public void ReadUserByUserName(string UserName, int OrganizationId)
        {
            UserServiceAgent usa = new UserServiceAgent();
            usa.ReadUserByUserName(UserName, OrganizationId, ReadUserByNameCompleted);
        }

        #endregion

        #region Completion Callbacks

        private void ReadUserByNameCompleted(UserDTO User, Exception ex)
        {
            ReadUserByNameLoaded(User, new EventArgs());
        }

        private void ReadUserNamesFromEwavCompleted(List<string> UserNames, Exception ex)
        {
            ReadUserNamesLoaded(UserNames, new EventArgs());
            //this.UserNames = UserNames;
        }


        private void ReadAssociatedDatasourcesCompleted(List<DTO.DatasourceDto> Datasources, Exception ex)
        {
            ReadAssociatedDSLoaded(Datasources, new EventArgs());
        }

        private void DeleteUserCompleted(bool result, Exception ex)
        {
            DeleteUserLoaded(result, new EventArgs());

        }

        private void ReadUserCompleted(List<UserDTO> users, Exception ex)
        {
            this.Users = users;
            ReadAllUsersLoaded(this.Users, new EventArgs());
        }

        private void AddUserCompleted(bool result, Exception ex)
        {
            if (result)
            {
                UserAdded(ex, new EventArgs());
            }
            else
            {
                UserAddedFailed(result, new EventArgs());
            }
        }

        private void PasswordRulesReadCompleted(PasswordRulesDTO passwordDTO, Exception ex)
        {
            PasswordRules = passwordDTO;
            PasswordRulesReadLoaded(this, new EventArgs());
        }


        // TODO: Optionally add callback methods for async calls to the service agent
        private void UserAuthenticatedComplete(UserDTO userDTO, Exception ex)
        {

            this.AuthenticatedUser = userDTO;
            UserIsAuthenticated = true;
            UserAuthenticated(this, new EventArgs());

        }

        private void UserFromADLoadedComplete(UserDTO userDTO, Exception ex)
        {
            this.LoadedUser = userDTO;
            UserLoadedFromAD(userDTO, new EventArgs());
        }

        private void UserLoadedComplete(UserDTO userDTO, Exception ex)
        {
            this.LoadedUser = userDTO;
            if (userDTO.UserID != -1)
            {
                UserLoaded(userDTO, new EventArgs());
            }
            else
            {
                UserNotAuthenticated(userDTO.Email, new EventArgs());
            }

        }

        private void UpdateUserCompleted(bool result, Exception ex)
        {

            UserUpdated(ex, new EventArgs());
        }

        // TODO: Optionally add callback methods for async calls to the service agent
        private void ForgotPasswordCompleted(bool result, Exception ex)
        {


            if (ex == null && result == true)
            {
                ForgotPasswordLoaded(this, new EventArgs());
            }
            else
            {
                ForgotPasswordFailed(this, new EventArgs());
            }


        }
        #endregion

        #region Helpers

        // Helper method to notify View of an error
        private void NotifyError(string message, Exception error)
        {
            // Notify view of an error
            Notify(ErrorNotice, new NotificationEventArgs<Exception>(message, error));
        }

        #endregion

        public bool UserIsAuthenticated { get; set; }
    }

}