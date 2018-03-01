/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       LogIn.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.Windows;
using System.Windows.Controls;
using Ewav.Membership;
using Ewav.ViewModels;
using Ewav.ExtensionMethods;
using Ewav.DTO;
using Ewav.Views.Dialogs;


namespace Ewav
{
    public partial class LogIn : ChildWindow
    {
        ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;

        MembershipManager mm;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogIn" /> class.
        /// </summary>
        public LogIn()
        {

            InitializeComponent();
            Loaded += LogIn_Loaded;
      

        }

        void LogIn_Loaded(object sender, RoutedEventArgs e)
        {

            txtServerClient.Text = "V " + applicationViewModel.ClientAssemblyVersion;

            if (applicationViewModel.DemoMode)
            {
                tbEmail.Text = applicationViewModel.DemoModeUser;
                tbPassword.Password = applicationViewModel.DemoModePassword;

                //  AuthenticateUser();
            }
       

        }

        /// <summary>
        /// Handles the Click event of the OKButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            AuthenticateUser();
        }

        /// <summary>
        /// Handles the Click event of the CancelButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }


        /// <summary>
        /// Authenticates the user.
        /// </summary>
        private void AuthenticateUser()
        {

            if (tbEmail.Text.IsEmailValid())
            {
                mm = new MembershipManager();

                UserDTO possibleUser = new UserDTO();

                possibleUser.UserName = tbEmail.Text.ToLower();

                PasswordHasher ph = new PasswordHasher(applicationViewModel.KeyForUserPasswordSalt);
                string salt = ph.CreateSalt(possibleUser.UserName);
                possibleUser.PasswordHash = ph.HashPassword(salt, tbPassword.Password);

                mm.AuthenticateUser(possibleUser);
                mm.UserAuthenticated += new MembershipManager.UserAuthenticatedHandler(mm_UserAuthenticated);
                mm.UserNotAuthenticated += new System.EventHandler(mm_UserNotAuthenticated);
            }
            else
            {
                spMsg.Visibility = System.Windows.Visibility.Visible;
                tbPassword.Password = "";
            }


        }


        /// <summary>
        /// Handles the UserNotAuthenticated event of the mm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        void mm_UserNotAuthenticated(object sender, System.EventArgs e)
        {

            mm.UserNotAuthenticated -= new System.EventHandler(mm_UserNotAuthenticated);

            if (spMsg.Visibility != System.Windows.Visibility.Visible)
            {
                spMsg.Visibility = System.Windows.Visibility.Visible;
                tbPassword.Password = "";
            }
            else
            {
            }

        }


        /// <summary>
        /// MM_s the user authenticated.
        /// </summary>
        /// <param name="userDto">The user dto.</param>
        void mm_UserAuthenticated(UserDTO userDto)
        {
#if   DEBUG
            //AppMenuView ap = this.Parent;
            //DashboardMainPage dp = ap.Parent;
            //dp.UserStats.ItemsSource = applicationViewModel.LoggedInUser;
            // ((DashboardMainPage)(AppMenuView(this.Parent))).UserStats.ItemsSource = applicationViewModel.LoggedInUser;
            //  ((DashboardMainPage)(AppMenuView(this.Parent))).UserStats.ItemsSource = applicationViewModel.LoggedInUser;  
#endif

            mm.UserAuthenticated -= new MembershipManager.UserAuthenticatedHandler(mm_UserAuthenticated);

            //  bool UserShouldResetPassword = (bool)sender;

            UserDTO dto = userDto as UserDTO;

            if (dto.ShouldResetPassword)
            {
                ResetPwd resetpwd = new ResetPwd();
                resetpwd.Show();
            }

            //  Reset passwod if neccessary but always load the authenticated user      
            mm = new MembershipManager();


            mm.LoadUser(dto.UserName);
            mm.UserLoaded += new System.EventHandler(mm_UserLoaded);
            mm.UserNotAuthenticated += new System.EventHandler(mm_UserNotAuthenticated);

        }

        /// <summary>
        /// Handles the UserLoaded event of the mm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        void mm_UserLoaded(object sender, System.EventArgs e)
        {

            DialogResult = true;
        }

        /// <summary>
        /// Handles the Click event of the HyperlinkButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void HyperlinkButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ApplicationViewModel.Instance.DemoMode)
            {
                DemoMode dm = new DemoMode();
                dm.Show();
                return;
            }


            // TODO: Add event handler implementation here.
            this.DialogResult = false;
            ForgotPwd fp = new ForgotPwd();
            fp.Show();
        }

        /// <summary>
        /// Handles the KeyDown event of the tbPassword control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs" /> instance containing the event data.</param>
        private void tbPassword_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                AuthenticateUser();
            }
        }
    }
}