/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       AddEditOrganization.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.Windows;
using System.Windows.Controls;
using Ewav.ExtensionMethods;

using Ewav.ViewModels;
using Ewav.ViewModels.Membership;
using System;
using SimpleMvvmToolkit;
using Ewav.DTO;
using System.Linq;
using Ewav.Views.Dialogs;


namespace Ewav
{
    public partial class AddEditOrganization : ChildWindow
    {
        private Organizations.AddEditMode addEditMode;
        private int selectedOrgID;


        public AddEditOrganization()
        {
            InitializeComponent();
            autoEmail.ItemsSource = null;
            autoEmail.ItemsSource = ApplicationViewModel.Instance.UserNames;
            Loaded += new System.Windows.RoutedEventHandler(AddEditOrganization_Loaded);
        }

        /// <summary>
        /// Gets or sets the selected org ID.
        /// </summary>
        /// <value>The selected org ID.</value>
        public int SelectedOrgID
        {
            get
            {
                return this.selectedOrgID;
            }
            set
            {
                this.selectedOrgID = value;
            }
        }

        void AddEditOrganization_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (addEditMode == Organizations.AddEditMode.Add)
            {
                spAddAdmin.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                spAddAdmin.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (ApplicationViewModel.Instance.AuthenticationMode.ToString().ToLower() == "windows")
            {
                grdWindows.Visibility = System.Windows.Visibility.Visible;
                grdForms.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                grdWindows.Visibility = System.Windows.Visibility.Collapsed;
                grdForms.Visibility = System.Windows.Visibility.Visible;
            }
        }

        /// <summary>
        /// Gets or sets the add edit mode.
        /// </summary>
        /// <value>The add edit mode.</value>
        public Organizations.AddEditMode AddEditMode
        {
            get
            {
                return this.addEditMode;
            }
            set
            {
                this.addEditMode = value;
            }
        }



        private void btnCancelOrgDetails_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DialogResult = false;
        }


        UserDTO userDto;

        void uvm_ReadUserByNameLoaded(object sender, EventArgs e)
        {
            if (((UserDTO)sender).FirstName == null)
            {
               // MessageBox.Show("User doesn't exists in our database.");
            }
            else
            {
                userDto = (UserDTO)sender;
                tbFirstName.Text = userDto.FirstName;
                tbLastName.Text = userDto.LastName;
                tbPhone.Text = userDto.Phone;
            }

        }



        UserViewModel uvm;

        private void autoEmail_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (autoEmail.SelectedItem != null && 
                ApplicationViewModel.Instance.UserNames.Contains(autoEmail.SelectedItem.ToString()))
            {
                //btnSearch_Click(sender, e);
                uvm = new UserViewModel();
                uvm.ReadUserByUserName(autoEmail.Text, -1);
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




        OrganizationViewModel organizationViewModel;
        UserDTO newAdminDto;
        OrganizationDto newOrganizationDto;
        UserOrganizationDto userOrgDto;
        OrganizationDto modelODto;

        /// <summary>
        /// Handles the Click event of the btnSaveOrgDetails control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void btnSaveOrgDetails_Click(object sender, RoutedEventArgs e)
        {

            if (ApplicationViewModel.Instance.DemoMode)
            {
                DemoMode dm = new DemoMode();
                dm.Show();
                return;
            }

            if (addEditMode == Organizations.AddEditMode.Edit)
            {
                if (ValidateForm())
                {
                    organizationViewModel = new OrganizationViewModel();

                    organizationViewModel.DtoUpdated += new EventHandler<NotificationEventArgs<Exception>>(organizationViewModel_DtoUpdated);
                    //   OrganizationDto modelODto = organizationViewModel.OrganizationDtoList.Single(x => x.Id ==  this.selectedOrgID);
                    modelODto = this.DataContext as OrganizationDto;
                    modelODto.Name = tbOrganizationName.Text;
                    modelODto.Active = (((ComboBoxItem)cboActive.SelectedItem).Content.ToString() == "Yes") ? true : false;    
                    organizationViewModel.Update(modelODto);
                    //Code to show success message on edit organization
                    //SucessMsg.Text = "Organization " + tbOrganizationName.Text + " has been updated.";
                    //imgSMsg.Visibility = System.Windows.Visibility.Visible;
                    //spMsg.Visibility = System.Windows.Visibility.Visible;

                }
            }
            else
            {
                if (ValidateForm())
                {
                    //string s = ((ComboBoxItem)cboActive.SelectedItem).Content.ToString();
                    bool IsActive = false;

                    if (((ComboBoxItem)cboActive.SelectedItem).Content.ToString() == "Yes")
                    {
                        IsActive = true;
                    }

                    userOrgDto = new UserOrganizationDto();

                    newOrganizationDto = new OrganizationDto()
                    {
                        Active = true,
                        Id = -1,
                        Name = tbOrganizationName.Text,
                        AdminCount = 0,
                        AnalystCount = 0,
                        DatasourceCount = 0,
                        DatasourcesCount = 0,
                        Description = null,
                        SuperAdminCount = 0,
                        TotalUserCount = 0
                    };

                    if (userDto != null)
                    {
                        newAdminDto = userDto;
                    }
                    else
                    {
                        newAdminDto = new UserDTO()
                        {
                            FirstName = tbFirstName.Text,
                            LastName = tbLastName.Text,
                            //OrganizationID = -1,
                            Phone = tbPhone.Text,
                            UserID = -1,
                            //UserName = autoEmail.Text, //"default user name",
                            //Email = autoEmail.Text,
                            //RoleValue = 2,
                            IsActive = true,
                            IsExistingUser = false
                        };
                    }

                    //newOrganizationDto.AdminList = new List<UserDTO>();
                    //newOrganizationDto.AdminList.Add(newAdminDto);

                    if (ApplicationViewModel.Instance.AuthenticationMode.ToString().ToLower() == "windows")
                    {
                        newAdminDto.Email = tbEmailAddress.Text.ToString().ToLower();
                        newAdminDto.UserName = tbUserId.Text.ToString().ToLower();
                    }
                    else
                    {
                        newAdminDto.Email = autoEmail.Text.ToString().ToLower();
                        newAdminDto.UserName = autoEmail.Text;
                    }


                    userOrgDto.Organization = newOrganizationDto;
                    userOrgDto.User = newAdminDto;
                    userOrgDto.RoleId = 2;
                    userOrgDto.Active = IsActive;

                    organizationViewModel = new OrganizationViewModel();
                    organizationViewModel.DtoAdded += new EventHandler<NotificationEventArgs<Exception>>(organizationViewModel_DtoAdded);
                    organizationViewModel.Add(userOrgDto);

                    //Code to show success message on add organization
                    //SucessMsg.Text = "Organization " + tbOrganizationName.Text + " has been added.";
                    //imgSMsg.Visibility = System.Windows.Visibility.Visible;
                    //spMsg.Visibility = System.Windows.Visibility.Visible;
                    //spMsg.Background = new SolidColorBrush(Color.FromArgb(255, 241, 202, 194)); //pink BK to use in case of error
                    //errMsg.Foreground = new SolidColorBrush(Color.FromArgb(255, 96, 25, 25)); //dark pink text color to use in case of error.
                    //imgerrMsg.Visibility = System.Windows.Visibility.Collapsed; // error icon
                }
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
        /// Handles the DtoAdded event of the organizationViewModel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SimpleMvvmToolkit.NotificationEventArgs&lt;System.Exception&gt;" /> instance containing the event data.</param>
        void organizationViewModel_DtoAdded(object sender, NotificationEventArgs<Exception> e)
        {
            //organizationViewModel = new OrganizationViewModel();
            //organizationViewModel.DtoRead += new EventHandler<NotificationEventArgs<Exception>>(organizationViewModel_DtoRead);
            //organizationViewModel.ReadAll();    

            this.DialogResult = true;
        }


        /// <summary>
        /// Organizations the view model_ dto updated.
        /// </summary>
        /// <param name="e">The e.</param>
        void organizationViewModel_DtoUpdated(object sender, NotificationEventArgs<Exception> e)
        {
            organizationViewModel.ReadAll();
            DialogResult = true;    

        }


        /// <summary>
        /// Validates the form.
        /// </summary>
        /// <returns></returns>
        private bool ValidateForm()
        {
            //return true;

            tbOrganizationName.ClearValidationError();

            bool isFormValid = true;
            spMsg.Visibility = System.Windows.Visibility.Collapsed;

            if (!tbOrganizationName.Text.IsAlphaNumericValid() || tbOrganizationName.Text == "")
            {
                tbOrganizationName.SetValidation("Please enter the organization name");
                tbOrganizationName.RaiseValidationError();
                isFormValid = false;
                spMsg.Visibility = System.Windows.Visibility.Visible;
                ErrMsg.Text = "Please correct the following errors.";
            }

            if (addEditMode == Ewav.Organizations.AddEditMode.Add)
            {
                tbFirstName.ClearValidationError();
                tbLastName.ClearValidationError();
                tbPhone.ClearValidationError();
                autoEmail.ClearValidationError();
                autoEmail.ClearValidationError();


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
            }


            return isFormValid;
        }

        private void btnSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                btnSearch_Click(sender, e);
            }
        }

    }
}