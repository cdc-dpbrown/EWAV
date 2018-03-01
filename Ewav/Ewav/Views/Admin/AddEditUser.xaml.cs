/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       AddEditUser.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Ewav.Client.Application;
using Ewav.ExtensionMethods;
using Ewav.Membership;
using Ewav.ViewModels;
using Ewav.ViewModels.Membership;
using Ewav.DTO;
using Ewav.Views.Dialogs;

namespace Ewav
{
    /// <summary>
    /// Child window to Add/Edit User
    /// </summary>
    public partial class AddEditUser : ChildWindow
    {
        public event CUDUserCompletedEventHandler CUDUserCompletedEvent;
        public event EventHandler CUDUserFailedEvent;
        AdminDatasourceViewModel advm = null;
        ObservableCollection<DTO.DatasourceDto> availableList;
        //private OrganizationDto organizationDto;
        ObservableCollection<DTO.DatasourceDto> selectedList;
        private UserDTO userDtoCopy;
        public int SelectedOrgId = -1;
        UserViewModel uvm = null;
        public OrganizationDto SelectedOrg { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="AddEditUser" /> class.
        /// </summary>
        public AddEditUser()
        {
            InitializeComponent();
            uvm = new UserViewModel();
            advm = new AdminDatasourceViewModel();
            //dvm.ReadAllDatasourcesInMyOrg(User.Instance.UserDto.OrganizationID);
            //dvm.MyDatasourcesReadLoaded += new EventHandler(dvm_MyDatasourcesReadLoaded);
            selectedList = new ObservableCollection<DTO.DatasourceDto>();
            availableList = new ObservableCollection<DTO.DatasourceDto>();
            //lbxAvailable.SelectionChanged += new SelectionChangedEventHandler(lbxAvailable_SelectionChanged);
            autoEmail.ItemsSource = null;
            autoEmail.ItemsSource = ApplicationViewModel.Instance.UserNames;
            this.Loaded += new RoutedEventHandler(AddEditUser2_Loaded);
        }

        /// <summary>
        /// Requests the read.
        /// </summary>
        private void RequestRead()
        {
            int orgId = this.SelectedOrg.Id;
            uvm.ReadUser(-1, orgId);
            uvm.ReadAllUsersLoaded += new EventHandler(uvm_ReadAllUsersLoaded);
        }

        /// <summary>
        /// Handles the ReadAllUsersLoaded event of the uvm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        void uvm_ReadAllUsersLoaded(object sender, EventArgs e)
        {
            BindGrid(sender);
        }

        /// <summary>
        /// Binds the grid.
        /// </summary>
        /// <param name="o">The o.</param>
        private void BindGrid(object o)
        {
            CUDUserCompletedEvent(o);
        }


        /// <summary>
        /// Mode Type Enumeration
        /// </summary>
        public enum ModeType
        {
            Add = 0,
            Edit
        }
        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        /// <value>
        /// The mode.
        /// </value>
        public ModeType Mode { get; set; }
        public UserDTO SelectedUserDto { get; set; }
        public List<string> UserNames { get; set; }
        /// <summary>
        /// Binds the list boxes.
        /// </summary>
        public void BindListBoxes()
        {
            lbxAvailable.ItemsSource = null;
            lbxAvailable.ItemsSource = availableList;
            lbxAvailable.DisplayMemberPath = "DatasourceName";
            lbxAvailable.SelectedValuePath = "DatasourceId";

            lbxSelected.ItemsSource = null;
            lbxSelected.ItemsSource = selectedList;
            lbxSelected.DisplayMemberPath = "DatasourceName";
            lbxSelected.SelectedValuePath = "DatasourceId";
        }

        /// <summary>
        /// Handles the Loaded event of the AddEditUser2 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        void AddEditUser2_Loaded(object sender, RoutedEventArgs e)
        {
            OrgName.Text = SelectedOrg.Name;

            if (ApplicationViewModel.Instance.AuthenticationMode.ToString().ToLower() == "windows")
            {
                //tnSearch.Visibility = System.Windows.Visibility.Visible;
                grdUserId.Visibility = System.Windows.Visibility.Visible;
                grdEmail.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                grdUserId.Visibility = System.Windows.Visibility.Collapsed;
                grdEmail.Visibility = System.Windows.Visibility.Visible;
                autoEmail.DropDownClosed += new RoutedPropertyChangedEventHandler<bool>(autoEmail_DropDownClosed);
            }

            if (this.SelectedUserDto != null)
            {
                if (this.SelectedUserDto.UserID == ApplicationViewModel.Instance.LoggedInUser.UserDto.UserID 
                    || this.SelectedUserDto.UserRoleInOrganization.ToLower() == "superadministrator" )
                {
                    cmbActive.IsEnabled = false;
                    cmbRole.IsEnabled = false;
                    autoEmail.IsEnabled = false;
                    tbEmailAddress.IsEnabled = false;
                }
                uvm.ReadAssociatedDatasources(this.SelectedUserDto.UserID, this.SelectedOrg.Id);
                uvm.ReadAssociatedDSLoaded += new EventHandler(uvm_ReadAssociatedDSLoaded);
                autoEmail.IsEnabled = false;
                this.Title = "Edit User";
            }
            else
            {
                ReadAllDatasources();
                this.Title = "Add User";
            }

        }

        /// <summary>
        /// Advm_s the read completed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void advm_ReadCompletedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            List<DTO.DatasourceDto> users = (List<DTO.DatasourceDto>)sender;
            availableList = new ObservableCollection<DTO.DatasourceDto>(users);
            ReadRestOfAvailableDatasources(selectedList);
            BindListBoxes();
        }

        /// <summary>
        /// Handles the Click event of the btnAddSource control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnAddSource_Click(object sender, RoutedEventArgs e)
        {
            var item = (DTO.DatasourceDto)lbxAvailable.SelectedItem;
            if (item != null)
            {
                selectedList.Add(item);
                availableList.Remove(item);
            }
        }

        /// <summary>
        /// Handles the Click event of the btnFinish control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void btnFinish_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ApplicationViewModel.Instance.DemoMode)
            {
                DemoMode dm = new DemoMode();
                dm.Show();
                return;
            }


            Storyboard2.Begin();
            tbStep1.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            rectStep1.Fill = new SolidColorBrush(Color.FromArgb(255, 38, 198, 48));
            tbStep2.Foreground = new SolidColorBrush(Color.FromArgb(255, 122, 122, 122));
            rectStep2.Fill = new SolidColorBrush(Color.FromArgb(255, 122, 122, 122));

            //userDto = userDtoCopy; 
            //this.SelectedUserDto = new UserDTO();
            if (this.SelectedUserDto == null)
            {
                this.SelectedUserDto = new UserDTO();
            }
            this.SelectedUserDto.FirstName = tbFirstName.Text;
            this.SelectedUserDto.LastName = tbLastName.Text;
            if (ApplicationViewModel.Instance.AuthenticationMode.ToString().ToLower() == "windows")
            {
                this.SelectedUserDto.Email = tbEmailAddress.Text.ToString().ToLower();
                this.SelectedUserDto.UserName = tbUserID.Text.ToString().ToLower();
            }
            else
            {
                this.SelectedUserDto.Email = autoEmail.Text.ToString().ToLower();
                this.SelectedUserDto.UserName = autoEmail.Text.ToLower();
            }

            this.SelectedUserDto.Phone = tbPhone.Text;
            //this.SelectedUserDto.RoleValue = Convert.ToInt32(Enum.Parse(typeof(RolesEnum), ((ComboBoxItem)cmbRole.SelectedValue).Content.ToString(), false));
            this.SelectedUserDto.UserRoleInOrganization = ((ComboBoxItem)cmbRole.SelectedItem).Content.ToString(); // cmbRole.SelectedValue.ToString();
            
            //this.SelectedUserDto.OrganizationID = User.Instance.UserDto.OrganizationID;
            this.SelectedUserDto.DatasourceList = ReadDatasourceList(this.SelectedUserDto.UserRoleInOrganization);

            UserOrganizationDto dto = new UserOrganizationDto();


            dto.Organization = new OrganizationDto();

            dto.Organization.Active = this.SelectedOrg.Active;
            dto.Organization.AdminCount = this.SelectedOrg.AdminCount;
            dto.Organization.AnalystCount = this.SelectedOrg.AnalystCount;
            dto.Organization.DatasourceCount = this.SelectedOrg.DatasourceCount;
            dto.Organization.DatasourcesCount = this.SelectedOrg.DatasourcesCount;
            dto.Organization.Description = "";
            dto.Organization.Id = this.SelectedOrg.Id;
            dto.Organization.Name = this.SelectedOrg.Name;
            dto.Organization.SuperAdminCount = this.SelectedOrg.SuperAdminCount;
            dto.Organization.TotalUserCount = this.SelectedOrg.TotalUserCount;


            dto.RoleId = Convert.ToInt32(Enum.Parse(typeof(RolesEnum), ((ComboBoxItem)cmbRole.SelectedValue).Content.ToString(), false));
            dto.Active = (cmbActive.SelectionBoxItem.ToString() == "Yes") ? true : false;


            switch (Mode)
            {
                case ModeType.Add:
                    dto.User = this.SelectedUserDto;
                    uvm.AddUser(dto);
                    uvm.UserAdded += new EventHandler(uvm_UserAdded);
                    uvm.UserAddedFailed += new EventHandler(uvm_UserAddedFailed);
                    break;
                case ModeType.Edit:
                    this.SelectedUserDto.UserID = this.SelectedUserDto.UserID;
                    this.SelectedUserDto.PasswordHash = this.SelectedUserDto.PasswordHash;
                    this.SelectedUserDto.UserEditType = UserEditType.EditingUserInfo;
                    dto.User = this.SelectedUserDto;
                    uvm.UpdateUser(dto);
                    uvm.UserUpdated += new EventHandler(uvm_UserUpdated);
                    break;
                default:
                    break;           
            }

            btnFinish.IsEnabled = false;
            this.DialogResult = true;
        }

        void uvm_UserAddedFailed(object sender, EventArgs e)
        {
            CUDUserFailedEvent(sender, e);
            MessageBox.Show("User already exists for this organization. Add failed");
        }

        /// <summary>
        /// Handles the Click event of the btnRemoveSource control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnRemoveSource_Click(object sender, RoutedEventArgs e)
        {
            var item = (DTO.DatasourceDto)lbxSelected.SelectedItem;
            if (item != null)
            {
                availableList.Add(item);
                selectedList.Remove(item);
            }
        }

        /// <summary>
        /// Handles the Click event of the btnSearch control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            uvm = new UserViewModel();
            uvm.LoadUserFromActivedirectory(tbEmailAddress.Text);
            uvm.UserLoadedFromAD += new EventHandler(uvm_UserLoadedFromAD);
        }

        void uvm_UserLoadedFromAD(object sender, EventArgs e)
        {
            UserDTO User = (UserDTO)sender;
            if (User != null)
            {
                tbFirstName.Text = (User.FirstName == null) ? "" : User.FirstName;
                tbLastName.Text = (User.LastName == null) ? "" : User.LastName;
                tbPhone.Text = (User.Phone == null) ? "" : User.Phone; 
                tbUserID.Text = (User.UserName == null) ? "" : User.UserName;
                spMsg.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                spMsg.Visibility = System.Windows.Visibility.Visible;
                ErrMsg.Text = "User not found. Please try again.";
            }
        }


        /// <summary>
        /// Handles the Click event of the btnStep1Next control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void btnStep1Next_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ValidateForm())
            {
                Storyboard1.Begin();
                tbStep1.Foreground = new SolidColorBrush(Color.FromArgb(255, 122, 122, 122));
                rectStep1.Fill = new SolidColorBrush(Color.FromArgb(255, 122, 122, 122));
                tbStep2.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                rectStep2.Fill = new SolidColorBrush(Color.FromArgb(255, 38, 198, 48));

                HideShowDatasourceLists();
            }



        }

        /// <summary>
        /// Handles the Click event of the Cancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            //autoEmail.
        }

        /// <summary>
        /// Hides the show datasource lists.
        /// </summary>
        private void HideShowDatasourceLists()
        {
            if (((ComboBoxItem)cmbRole.SelectedItem).Content.ToString().ToLower() == "analyst"  )        
            {
                spDatasource.Visibility = System.Windows.Visibility.Visible;
                txtAdminDS.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                spDatasource.Visibility = System.Windows.Visibility.Collapsed;
                txtAdminDS.Visibility = System.Windows.Visibility.Visible;
            }
        }

        /// <summary>
        /// Populates the form.
        /// </summary>
        private void PopulateForm()
        {
            if (this.SelectedUserDto.FirstName != null)
            {
                tbFirstName.Text = this.SelectedUserDto.FirstName;
                tbLastName.Text = this.SelectedUserDto.LastName;
                tbPhone.Text = this.SelectedUserDto.Phone;

                if (ApplicationViewModel.Instance.AuthenticationMode.ToString().ToLower() == "windows")
                {
                    tbEmailAddress.Text = this.SelectedUserDto.Email;
                }
                else
                {
                    autoEmail.Text = this.SelectedUserDto.UserName;
                }



                if (this.SelectedUserDto.UserRoleInOrganization != null &&
                    this.SelectedUserDto.UserRoleInOrganization.ToLower() == "user")
                {
                    cmbRole.SelectedIndex = 1;
                }
                else ///if (User.RoleText.ToLower() == "administrator")
                {
                    cmbRole.SelectedIndex = 0;
                    // spRole.Visibility = System.Windows.Visibility.Collapsed;
                }

                if (this.SelectedUserDto.IsActive)
                {
                    cmbActive.SelectedIndex = 0;
                }
                else
                {
                    cmbActive.SelectedIndex = 1;
                }
            }
            //else
            //{
            //    MessageBox.Show("User doesn't exists in our database.");
            //}

            BindListBoxes();
        }

        /// <summary>
        /// Reads all datasources.
        /// </summary>
        private void ReadAllDatasources()
        {
            advm.Read(this.SelectedOrg.Id);
            advm.ReadCompletedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(advm_ReadCompletedEvent);
        }

        /// <summary>
        /// Reads the datasource list.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <returns></returns>
        private List<DTO.DatasourceDto> ReadDatasourceList(string UserRoleInOrganization)
        {
            List<DTO.DatasourceDto> combinedList = new List<DTO.DatasourceDto>();
            if (UserRoleInOrganization == "Administrator") //Admin - Read all the datasources.
            {
                combinedList.AddRange(availableList.ToList());
                combinedList.AddRange(selectedList.ToList());
                return combinedList;
            }
            else
            {
                return selectedList.ToList();
            }
        }

        /// <summary>
        /// Reads the rest of available Datasources.
        /// </summary>
        /// <param name="selectedList">The selected list.</param>
        private void ReadRestOfAvailableDatasources(ObservableCollection<DTO.DatasourceDto> selectedList)
        {
            availableList = new ObservableCollection<DTO.DatasourceDto>(availableList.Where(t => selectedList.All(t1 => t.DatasourceId != t1.DatasourceId)));
        }

        /// <summary>
        /// Handles the ReadAssociatedDSLoaded event of the uvm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        void uvm_ReadAssociatedDSLoaded(object sender, EventArgs e)
        {
            List<DTO.DatasourceDto> datasources = (List<DTO.DatasourceDto>)sender;

            selectedList = new ObservableCollection<DTO.DatasourceDto>(datasources);


            if (ApplicationViewModel.Instance.LoggedInUser.RolesEnum == Membership.RolesEnum.SuperAdministrator)
            {
            }
            if (Mode == ModeType.Edit)
            {
                userDtoCopy = new UserDTO()
                {
                    Email = this.SelectedUserDto.Email,
                    FirstName = this.SelectedUserDto.FirstName,
                    LastName = this.SelectedUserDto.LastName,
                    //OrganizationID = this.SelectedUserDto.OrganizationID,
                    PasswordHash = this.SelectedUserDto.PasswordHash,
                    Phone = this.SelectedUserDto.Phone,
                    // RoleValue = this.SelectedUserDto.RoleValue,
                    UserRoleInOrganization = this.SelectedUserDto.UserRoleInOrganization,
                    ShouldResetPassword = this.SelectedUserDto.ShouldResetPassword,
                    UserID = this.SelectedUserDto.UserID,
                    UserName = this.SelectedUserDto.UserName,
                    // RoleText = this.SelectedUserDto.RoleText,
                    IsActive = this.SelectedUserDto.IsActive
                };
            }

            if (userDtoCopy != null)
            {
                if (userDtoCopy.UserRoleInOrganization.ToLower() == "user")
                {
                    cmbRole.SelectedIndex = 1;
                }
                else if (userDtoCopy.UserRoleInOrganization.ToLower() == "administrator")
                {
                    cmbRole.SelectedIndex = 0;
                }

                if (this.SelectedUserDto.IsActive)
                {
                    cmbActive.SelectedIndex = 0;
                }
                else
                {
                    cmbActive.SelectedIndex = 1;
                }
            }

            ReadAllDatasources();

            this.DataContext = userDtoCopy;
        }

        /// <summary>
        /// Handles the ReadUserByNameLoaded event of the uvm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        void uvm_ReadUserByNameLoaded(object sender, EventArgs e)
        {
            this.SelectedUserDto = (UserDTO)sender;
            PopulateForm();
        }

        /// <summary>
        /// Handles the UserAdded event of the uvm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        void uvm_UserAdded(object sender, EventArgs e)
        {
            btnFinish.IsEnabled = true;
            RequestRead();
        }

        /// <summary>
        /// Handles the UserUpdated event of the uvm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        void uvm_UserUpdated(object sender, EventArgs e)
        {
            btnFinish.IsEnabled = true;
            RequestRead();
        }

        /// <summary>
        /// Validates the form.
        /// </summary>
        /// <returns></returns>
        private bool ValidateForm()
        {
            //return true;

            tbFirstName.ClearValidationError();
            tbLastName.ClearValidationError();
            tbPhone.ClearValidationError();
            autoEmail.ClearValidationError();
            cmbRole.ClearValidationError();
            tbEmailAddress.ClearValidationError();


            bool isFormValid = true;
            spMsg.Visibility = System.Windows.Visibility.Collapsed;

            if (!tbFirstName.Text.IsTextValid() || tbFirstName.Text == "")
            {
                tbFirstName.SetValidation("Please enter your First name");
                tbFirstName.RaiseValidationError();
                isFormValid = false;
                spMsg.Visibility = System.Windows.Visibility.Visible;
                ErrMsg.Text = "Please correct the following errors.";
            }
            if (!tbLastName.Text.IsTextValid() || tbLastName.Text == "")
            {
                tbLastName.SetValidation("Please enter your Last Name");
                tbLastName.RaiseValidationError();
                isFormValid = false;
                spMsg.Visibility = System.Windows.Visibility.Visible;
                ErrMsg.Text = "Please correct the following errors.";
            }
            //if (!tbPhone.Text.IsPhoneNumberValid() || tbPhone.Text == "")
            //{
            //    tbPhone.SetValidation("Please enter your phone number xxx-xxx-xxxx");
            //    tbPhone.RaiseValidationError();
            //    isFormValid = false;
            //    spMsg.Visibility = System.Windows.Visibility.Visible;
            //    ErrMsg.Text = "Please correct the following errors.";
            //}
            if (ApplicationViewModel.Instance.AuthenticationMode.ToString().ToLower() == "windows")
            {
                if (!tbEmailAddress.Text.IsEmailValid() || tbEmailAddress.Text == "")
                {
                    tbEmailAddress.SetValidation("Please enter a valid e-mail address");
                    tbEmailAddress.RaiseValidationError();
                    isFormValid = false;
                    spMsg.Visibility = System.Windows.Visibility.Visible;
                    ErrMsg.Text = "Please correct the following errors.";
                }
            }
            else
            {
                if (!autoEmail.Text.IsEmailValid() || autoEmail.Text == "")
                {
                    autoEmail.SetValidation("Please enter a valid e-mail address");
                    autoEmail.RaiseValidationError();
                    isFormValid = false;
                    spMsg.Visibility = System.Windows.Visibility.Visible;
                    ErrMsg.Text = "Please correct the following errors.";
                }
            }


            if (cmbRole.SelectedIndex == -1)
            {
                cmbRole.SetValidation("Please select a role.");
                cmbRole.RaiseValidationError();
                isFormValid = false;
                spMsg.Visibility = System.Windows.Visibility.Visible;
                ErrMsg.Text = "Please correct the following errors.";
            }

            //if (isFormValid)
            //{
            //    HtmlPage.Window.Alert("form was successfully submitted!");
            //    ResetForm();
            //}

            return isFormValid;
        }

        /// <summary>
        /// Handles the Click event of the btnCancel3 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnCancel3_Click(object sender, RoutedEventArgs e)
        {
            Storyboard2.Begin();
        }

        /// <summary>
        /// Autoes the email_ drop down closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void autoEmail_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {



            if (Mode == ModeType.Add)
            {
                if (autoEmail.SelectedItem != null && ApplicationViewModel.Instance.UserNames.Contains(autoEmail.SelectedItem.ToString()))
                {
                    uvm = new UserViewModel();
                    uvm.ReadUserByUserName(autoEmail.Text, SelectedOrgId);
                    uvm.ReadUserByNameLoaded += new EventHandler(uvm_ReadUserByNameLoaded);

                    tbFirstName.IsEnabled = false;
                    tbLastName.IsEnabled = false;
                    tbPhone.IsEnabled = false;
                }
                else
                {
                    tbFirstName.IsEnabled = true;
                    tbFirstName.Text = "";
                    tbLastName.IsEnabled = true;
                    tbLastName.Text = "";
                    tbPhone.IsEnabled = true;
                    tbPhone.Text = "";
                }
            }

        }

        private void tbEmailAddress_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                btnSearch_Click(sender, e);
            }
        }
    }
}