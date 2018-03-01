/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MembershipManager.cs
 *  Namespace:  Ewav.Membership    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using Ewav.ViewModels.Membership;
using Ewav.ViewModels;
using System.Windows.Browser;
using Ewav.DTO;    

namespace Ewav.Membership
{
    /// <summary>
    /// The MembershipManager manager is responsible for all Membership related coordihation.  Many things that 
    /// would typically fe found in ViewModel type code maybe found here instead for classes in the 
    /// Membership namespace     
    /// </summary>
    public class MembershipManager
    {
        public EventHandler UserAuthenticatedFromEwav;
        public EventHandler UserNotAuthenticatedFromEwav;
        public EventHandler UserLoadedWithNoDatasource;
        /// <summary>
        /// 
        /// </summary>
        private UserViewModel userViewModel;

        /// <summary>
        /// Authenticates the and load user.
        /// </summary>
        /// <param name="toString">To string.</param>
        public void AuthenticateAndLoadUser( string  userName    )    
        {
          

            UserViewModel uvm = new UserViewModel();
            uvm.UserLoaded += new EventHandler(uvm_UserLoaded);
            uvm.UserNotAuthenticated += new EventHandler(uvm_UserNotAuthenticated);
            uvm.LoadUser(userName);     


        }

        void uvm_UserNotAuthenticated(object sender, EventArgs e)
        {
            UserNotAuthenticatedFromEwav(sender, e);
        }

        void uvm_UserLoaded(object sender, EventArgs e)
        {
            UserDTO resultDTO;

            resultDTO = sender as UserDTO;


            ApplicationViewModel.Instance.LoggedInUser.UserDto = resultDTO;

            if (ApplicationViewModel.Instance.LoggedInUser.UserDto.DatasourceCount == 0)
            {
                UserLoadedWithNoDatasource(sender, e);
            }

            UserAuthenticatedFromEwav(sender, e);
        }

        public delegate void UserAuthenticatedHandler(UserDTO authenticatedUserDto);
        public event UserAuthenticatedHandler UserAuthenticated;
        public event EventHandler AuthenticationAndLoadcomplete;


        /// <summary>
        ///   
        /// </summary>
        public event EventHandler UserNotAuthenticated;
        /// <summary>
        /// Authenticates the specified user dto.
        /// </summary>
        /// <param name="userDto">The user dto.</param>
        /// <returns></returns>     
        public event EventHandler UserLoaded = delegate { };

        public void AuthenticateUser(UserDTO userDto)
        {


            userViewModel = new UserViewModel();
            userViewModel.UserAuthenticated += new EventHandler(userViewModel_UserAuthenticated);
            userViewModel.Authenticate(userDto);

        }

        public void LoadUser(string UserName)
        {
            userViewModel = new UserViewModel();
            userViewModel.UserLoaded += new EventHandler(userViewModel_UserLoaded);
            userViewModel.UserNotAuthenticated += new EventHandler(userViewModel_UserNotAuthenticated);
            userViewModel.LoadUser(UserName);

        }

        void userViewModel_UserNotAuthenticated(object sender, EventArgs e)
        {
            HtmlPage.Window.Navigate(new Uri("Error.aspx?AdminEmail=" + sender.ToString(), UriKind.Relative));
        }

        /// <summary>
        /// Handles the UserAuthenticated event of the userViewModel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        void userViewModel_UserAuthenticated(object sender, EventArgs e)
        {

            //  TODO  create a heter test or a non-authenticated user      
            if (userViewModel.AuthenticatedUser.PasswordHash.ToLower() == "false")
            {
                UserNotAuthenticated(this, new EventArgs());
            }
            else
            {
                UserAuthenticated(userViewModel.AuthenticatedUser);
            }
        }

        void userViewModel_UserLoaded(object sender, EventArgs e)
        {
            // Set the user    
            ApplicationViewModel.Instance.LoggedInUser.UserDto = userViewModel.LoadedUser;
            UserLoaded(this, new EventArgs());

        }

    }
}