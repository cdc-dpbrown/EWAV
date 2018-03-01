/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       Users.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Ewav.DTO;
using Ewav.ExtensionMethods;
using Ewav.Membership;
using Ewav.ViewModels;
using Ewav.ViewModels.Membership;

namespace Ewav
{
    /// <summary>
    /// Users Class
    /// </summary>
    public partial class Users : UserControl
    {
        public ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;

        UserViewModel uvm = null;
        AddEditUser user = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="Users" /> class.
        /// </summary>
        public Users()
        {
            this.InitializeComponent();
            uvm = new UserViewModel();

            uvm.ReadAllOrgsForUser(User.Instance.UserDto.UserID);
            uvm.ReadAllOrgsForUserComplete += new UserViewModel.ReadAllOrgsForUserCompleteHandler(uvm_ReadAllOrgsForUserComplete);

            //uvm.ReadUserNamesFromEwav();
            //uvm.ReadUserNamesLoaded += new EventHandler(uvm_ReadUserNamesLoaded);
            applicationViewModel.ReadUserNames();

            applicationViewModel.LoggedInUser.SelectedOrganizationChanged += new EventHandler(LoggedInUser_SelectedOrganizationChanged);
        }

        //public List<string> UserNames { get; set; }
        /// <summary>
        /// Handles the Click event of the btnAddNewUser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void btnAddNewUser_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            user = new AddEditUser();
            user.Mode = AddEditUser.ModeType.Add;
            user.SelectedOrg = (OrganizationDto)cmbOrgName.SelectedItem;
            user.CUDUserCompletedEvent += new Client.Application.CUDUserCompletedEventHandler(CUDUserCompletedEvent);
            user.CUDUserFailedEvent += new EventHandler(user_CUDUserFailedEvent);
            user.Closed += new EventHandler(user_Closed);
            user.Show();
        }

        void user_Closed(object sender, EventArgs e)
        {
            if (user.DialogResult == true)
            {
               waitCursor.Visibility = System.Windows.Visibility.Visible; 
            }
            
        }

        private void cmbOrgName_DropDownClosed(object sender, EventArgs e)
        {
            OrganizationDto oDto = ((ComboBox)sender).SelectedItem as OrganizationDto;
            // the current org for this user is now oDto.OrganizationId     
            applicationViewModel.LoggedInUser.SetOrganizationId(oDto.Id);
        }

        /// <summary>
        /// completed event.
        /// </summary>
        /// <param name="o">The o.</param>
        void CUDUserCompletedEvent(object o)
        {
            dgUsers.Refresh(o);
            applicationViewModel.ReadUserNames();
            spMsg.Visibility = System.Windows.Visibility.Visible;
            if (user.Mode == AddEditUser.ModeType.Add)
            {
                errMsg.Text = "User " + user.tbFirstName.Text + " " + user.tbLastName.Text + " has been added.";

            }
            else
            {
                errMsg.Text = "User " + user.tbFirstName.Text + " " + user.tbLastName.Text + " has been updated.";
            }
            waitCursor.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Handles the Click event of the Delete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxShow = MessageBox.Show("Are you sure you want to delete the user", "Warning", MessageBoxButton.OKCancel);

            if (messageBoxShow == MessageBoxResult.OK)
            {
                var ctl = e.OriginalSource as Button;
                if (null != ctl)
                {
                    var userDto = ctl.DataContext as UserDTO;
                    if (null != userDto)
                    {
                        DeleteSelectedUser(userDto);
                    }
                }
            }
        }

        /// <summary>
        /// Deletes the selected user.
        /// </summary>
        /// <param name="userDto">The user dto.</param>
        private void DeleteSelectedUser(UserDTO userDto)
        {
            uvm.DeleteUser(userDto.UserID);
            uvm.DeleteUserLoaded += new EventHandler(uvm_DeleteUserLoaded);
        }

        /// <summary>
        /// Handles the Click event of the Edit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var ctl = e.OriginalSource as Button;
            if (null != ctl)
            {
                var userDto = ctl.DataContext as UserDTO;
                if (null != userDto)
                {
                    EditSelectedUser(userDto);
                }
            }
        }

        /// <summary>
        /// Edits the selected user.
        /// </summary>
        /// <param name="userDto">The user dto.</param>
        private void EditSelectedUser(UserDTO userDto)
        {
            user = new AddEditUser();
            user.Mode = AddEditUser.ModeType.Edit;
            user.SelectedOrg = (OrganizationDto)cmbOrgName.SelectedItem;
            user.SelectedUserDto = userDto;
            user.CUDUserCompletedEvent += new Client.Application.CUDUserCompletedEventHandler(CUDUserCompletedEvent);
            user.CUDUserFailedEvent += new EventHandler(user_CUDUserFailedEvent);
            user.Closed += new EventHandler(user_Closed);
            user.Show();
        }

        void user_CUDUserFailedEvent(object sender, EventArgs e)
        {
            waitCursor.Visibility = System.Windows.Visibility.Collapsed;
        }

        void LoggedInUser_SelectedOrganizationChanged(object sender, EventArgs e)
        {
            //read all the users in my organization including superadmin, admin and analysts.
            //int orgId = ((OrganizationDto)cmbOrgName.SelectedItem).Id;
            int orgId = Convert.ToInt32(sender);
            uvm.ReadUser(-1, orgId);
            spMsg.Visibility = System.Windows.Visibility.Collapsed;
            //  NOTE:  
            //  A listener has already been wired to this event so it does not have to be wired again ( memeory leak  )     
            //  uvm.ReadAllUsersLoaded += new EventHandler( );
        }

        /// <summary>
        /// Refreshes the data grid.
        /// </summary>


        /// <summary>
        /// Handles the DeleteUserLoaded event of the uvm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        void uvm_DeleteUserLoaded(object sender, EventArgs e)
        {
            if ((bool)sender)
            {
                errMsg.Text = "User deleted successfully";
                MessageBox.Show("User deleted successfully");
            }
            else
            {
                errMsg.Text = "Delete User Failed.";

                spMsg.Visibility = System.Windows.Visibility.Visible;
            }
        }

        void uvm_ReadAllOrgsForUserComplete(List<OrganizationDto> userOrganizationDtoList, Exception ex)
        {

            cmbOrgName.ItemsSource = userOrganizationDtoList;
            cmbOrgName.DisplayMemberPath = "Name";
            cmbOrgName.SelectedValuePath = "Id";

            cmbOrgName.SelectedIndex = 0;

            int orgId = ((OrganizationDto)cmbOrgName.SelectedItem).Id;

            // after we get all orgs for the user get all users for this org     
            uvm.ReadUser(-1, orgId); //read all the users in my organization including superadmin, admin and analysts.
            uvm.ReadAllUsersLoaded += new EventHandler(uvm_ReadAllUsersLoaded);

            spMsg.Visibility = System.Windows.Visibility.Collapsed;

            // set the dropdown to the right org  
            foreach (object item in cmbOrgName.Items)
            {
                ComboBoxItem ciForType = item as ComboBoxItem;

                OrganizationDto oDto = item as OrganizationDto;
                if (oDto.Id == orgId)
                {
                    cmbOrgName.SelectedItem = item;
                }
            }
        }

        /// <summary>
        /// Handles the ReadAllUsersLoaded event of the uvm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        void uvm_ReadAllUsersLoaded(object sender, EventArgs e)
        {
            //BindGrid(uvm.Users);    //  Extension method 
            dgUsers.Refresh(sender);
        }



        //void uvm_ReadUserNamesLoaded(object sender, EventArgs e)
        //{
        //    UserNames = (List<string>)sender;
        //}
    }
}