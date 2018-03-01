/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       Organizations.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Ewav.ExtensionMethods;
using Ewav.ViewModels;
using Ewav.ViewModels.Membership;
using SimpleMvvmToolkit;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Ewav.DTO;


namespace Ewav
{
    /// <summary>
    /// Organization Class
    /// </summary>
    public partial class Organizations : UserControl
    {
        AddEditMode addEditMode;
        ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;
        UserConrolUIMode currentUserConrolUIMode;
        UserDTO newAdminDto, userDto;
        OrganizationDto newOrganizationDto;
        UserOrganizationDto userOrgDto;
        OrganizationViewModel organizationViewModel;

        //OrganizationDto selectedOrganizationDto;
        int selectedOrgID;
        /// <summary>
        /// Initializes a new instance of the <see cref="Organizations" /> class.
        /// </summary>
        public Organizations()
        {
            this.InitializeComponent();
            spMsg.Visibility = System.Windows.Visibility.Collapsed;

            applicationViewModel.ReadUserNames();

            this.Loaded += new System.Windows.RoutedEventHandler(Organization_Loaded);
        }

        public enum AddEditMode
        {
            Add,
            Edit
        }

        /// <summary>
        /// Controls mode for this user control     
        /// </summary>
        enum UserConrolUIMode
        {
            Edit,
            ShowList,
            Add
        }
        /// <summary>
        /// Handles the Click event of the btnAddOrg control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnAddOrg_Click(object sender, RoutedEventArgs e)
        {
            addEditMode = AddEditMode.Add;
            spUser.Visibility = System.Windows.Visibility.Visible;
            tbFirstName.IsEnabled = true;
            autoEmail.Text = "";
            autoEmail.IsEnabled = true;
            tbFirstName.Text = "";
            tbLastName.IsEnabled = true;
            tbLastName.Text = "";
            tbPhone.IsEnabled = true;
            tbPhone.Text = "";

            autoEmail.ItemsSource = null;
            autoEmail.ItemsSource = applicationViewModel.UserNames;

            //  SwitchMode(UserConrolUIMode.Add);    

            AddEditOrganization aeo = new AddEditOrganization();
            aeo.Title = "Add Organization";
            aeo.AddEditMode = addEditMode;
            aeo.cboActive.SelectedIndex = 0;
            aeo.Closed += new EventHandler(aeo_Closed);
            aeo.Show();

        }

        /// <summary>
        /// Handles the Click event of the btnAddSource control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnAddSource_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Handles the Click event of the btnCancelOrgDetails control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnCancelOrgDetails_Click(object sender, RoutedEventArgs e)
        {
            spOrganizationList.Visibility = System.Windows.Visibility.Visible;
            spOrganizationEdit.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Handles the Click event of the btnCancelUserDetails control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnCancelUserDetails_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Handles the Click event of the btnEditOrg control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void btnEditOrg_Click(object sender, RoutedEventArgs e)
        {
            var ctl = e.OriginalSource as Button;
            if (null != ctl)
            {
                var orgDto = ctl.DataContext as OrganizationDto;
                if (null != orgDto)
                {
                    selectedOrgID = orgDto.Id;
                    addEditMode = AddEditMode.Edit;
                    OrganizationDto modelODto = organizationViewModel.OrganizationDtoList.Single(x => x.Id == selectedOrgID);
                    EditSelectedOrg(ref modelODto);
                }
            }
        }

        private void btnRemoveSource_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Handles the Click event of the btnSaveOrgDetails control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void btnSaveOrgDetails_Click(object sender, RoutedEventArgs e)
        {
            if (addEditMode == AddEditMode.Edit)
            {
                if (ValidateForm())
                {
                    organizationViewModel.DtoUpdated += new EventHandler<NotificationEventArgs<Exception>>(organizationViewModel_DtoUpdated);
                    OrganizationDto modelODto = organizationViewModel.OrganizationDtoList.Single(x => x.Id == selectedOrgID);
                    modelODto.Name = tbOrganizationName.Text;
                    modelODto.Active = (((ComboBoxItem)cboActive.SelectedItem).Content.ToString() == "Yes") ? true : false;
                    organizationViewModel.Update(modelODto);
                    //Code to show success message on edit organization
                    SucessMsg.Text = "Organization " + tbOrganizationName.Text + " has been updated.";
                    imgSMsg.Visibility = System.Windows.Visibility.Visible;
                    spMsg.Visibility = System.Windows.Visibility.Visible;
                    //spMsg.Background = new SolidColorBrush(Color.FromArgb(255, 241, 202, 194)); //pink BK to use in case of error
                    //errMsg.Foreground = new SolidColorBrush(Color.FromArgb(255, 96, 25, 25)); //dark pink text color to use in case of error.
                    //imgerrMsg.Visibility = System.Windows.Visibility.Collapsed; // error icon
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
                    SucessMsg.Text = "Organization " + tbOrganizationName.Text + " has been added.";
                    imgSMsg.Visibility = System.Windows.Visibility.Visible;
                    spMsg.Visibility = System.Windows.Visibility.Visible;
                    //spMsg.Background = new SolidColorBrush(Color.FromArgb(255, 241, 202, 194)); //pink BK to use in case of error
                    //errMsg.Foreground = new SolidColorBrush(Color.FromArgb(255, 96, 25, 25)); //dark pink text color to use in case of error.
                    //imgerrMsg.Visibility = System.Windows.Visibility.Collapsed; // error icon
                }
            }
        }


        //private void saveEdits()
        //{
        //    if (addEditMode == AddEditMode.Edit)
        //    {
        //        if (ValidateForm())
        //        {
        //            organizationViewModel.DtoUpdated += new EventHandler<NotificationEventArgs<Exception>>(organizationViewModel_DtoUpdated);
        //            OrganizationDto modelODto = organizationViewModel.OrganizationDtoList.Single(x => x.Id == selectedOrgID);
        //            modelODto.Name = tbOrganizationName.Text;
        //            modelODto.Active = (((ComboBoxItem)cboActive.SelectedItem).Content.ToString() == "Yes") ? true : false;
        //            organizationViewModel.Update(modelODto);
        //            //Code to show success message on edit organization
        //            SucessMsg.Text = "Organization " + tbOrganizationName.Text + " has been updated.";
        //            imgSMsg.Visibility = System.Windows.Visibility.Visible;
        //            spMsg.Visibility = System.Windows.Visibility.Visible;
        //            //spMsg.Background = new SolidColorBrush(Color.FromArgb(255, 241, 202, 194)); //pink BK to use in case of error
        //            //errMsg.Foreground = new SolidColorBrush(Color.FromArgb(255, 96, 25, 25)); //dark pink text color to use in case of error.
        //            //imgerrMsg.Visibility = System.Windows.Visibility.Collapsed; // error icon
        //        }
        //    }
        //    else
        //    {
        //        if (ValidateForm())
        //        {
        //            //string s = ((ComboBoxItem)cboActive.SelectedItem).Content.ToString();
        //            bool IsActive = false;

        //            if (((ComboBoxItem)cboActive.SelectedItem).Content.ToString() == "Yes")
        //            {
        //                IsActive = true;
        //            }

        //            userOrgDto = new UserOrganizationDto();

        //            newOrganizationDto = new OrganizationDto()
        //            {
        //                Active = true,
        //                Id = -1,
        //                Name = tbOrganizationName.Text,
        //                AdminCount = 0,
        //                AnalystCount = 0,
        //                DatasourceCount = 0,
        //                DatasourcesCount = 0,
        //                Description = null,
        //                SuperAdminCount = 0,
        //                TotalUserCount = 0
        //            };

        //            if (userDto != null)
        //            {
        //                newAdminDto = userDto;
        //            }
        //            else
        //            {
        //                newAdminDto = new UserDTO()
        //                {
        //                    FirstName = tbFirstName.Text,
        //                    LastName = tbLastName.Text,
        //                    //OrganizationID = -1,
        //                    Phone = tbPhone.Text,
        //                    UserID = -1,
        //                    //UserName = autoEmail.Text, //"default user name",
        //                    //Email = autoEmail.Text,
        //                    //RoleValue = 2,
        //                    IsActive = true,
        //                    IsExistingUser = false
        //                };
        //            }

        //            //newOrganizationDto.AdminList = new List<UserDTO>();
        //            //newOrganizationDto.AdminList.Add(newAdminDto);

        //            if (ApplicationViewModel.Instance.AuthenticationMode.ToString().ToLower() == "windows")
        //            {
        //                newAdminDto.Email = tbEmailAddress.Text.ToString().ToLower();
        //                newAdminDto.UserName = tbUserId.Text.ToString().ToLower();
        //            }
        //            else
        //            {
        //                newAdminDto.Email = autoEmail.Text.ToString().ToLower();
        //                newAdminDto.UserName = autoEmail.Text;
        //            }


        //            userOrgDto.Organization = newOrganizationDto;
        //            userOrgDto.User = newAdminDto;
        //            userOrgDto.RoleId = 2;
        //            userOrgDto.Active = IsActive;

        //            organizationViewModel = new OrganizationViewModel();
        //            organizationViewModel.DtoAdded += new EventHandler<NotificationEventArgs<Exception>>(organizationViewModel_DtoAdded);
        //            organizationViewModel.Add(userOrgDto);

        //            //Code to show success message on add organization
        //            SucessMsg.Text = "Organization " + tbOrganizationName.Text + " has been added.";
        //            imgSMsg.Visibility = System.Windows.Visibility.Visible;
        //            spMsg.Visibility = System.Windows.Visibility.Visible;
        //            //spMsg.Background = new SolidColorBrush(Color.FromArgb(255, 241, 202, 194)); //pink BK to use in case of error
        //            //errMsg.Foreground = new SolidColorBrush(Color.FromArgb(255, 96, 25, 25)); //dark pink text color to use in case of error.
        //            //imgerrMsg.Visibility = System.Windows.Visibility.Collapsed; // error icon
        //        }
        //    }

        //}
        private void btnSaveUserDetails_Click(object sender, RoutedEventArgs e)
        {
        }

        private void cmbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        /// <summary>
        /// Edits the selected org.
        /// </summary>
        /// <param name="orgDto">The org dto.</param>
        private void EditSelectedOrg(ref OrganizationDto modelODto)
        {
            AddEditOrganization aeo = new AddEditOrganization();
            aeo.DataContext = modelODto;
            aeo.tbOrganizationName.DataContext = aeo.DataContext;
            aeo.cboActive.SelectedIndex = (modelODto.Active) ? 0 : 1;          //                 GetComboIndex(cboActive, modelODto.Active);
            aeo.AddEditMode = addEditMode;
            aeo.SelectedOrgID = selectedOrgID;
            aeo.Closed += new EventHandler(aeo_Closed);
            aeo.Show();
        }


        /// <summary>
        /// Handles the Closed event of the aeo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>    
        void aeo_Closed(object sender, EventArgs e)
        {
            AddEditOrganization aeo = (AddEditOrganization)sender as AddEditOrganization;  

            if (aeo.DialogResult == true)
            {
                aeo.Closed -= new EventHandler(aeo_Closed);

                organizationViewModel = new OrganizationViewModel();
                organizationViewModel.DtoRead += new EventHandler<NotificationEventArgs<Exception>>(organizationViewModel_DtoRead);
                organizationViewModel.ReadAll();
                spMsg.Visibility = System.Windows.Visibility.Visible;
                if (addEditMode == AddEditMode.Add)
                {
                    SucessMsg.Text = "Organization " + aeo.tbOrganizationName.Text + " has been added.";
                }
                else
                {
                    SucessMsg.Text = "Organization " + aeo.tbOrganizationName.Text + " has been updated.";
                }
                waitCursor.Visibility = System.Windows.Visibility.Visible;
            }

        }

        /// <summary>
        /// Edits the selected org.
        /// </summary>
        /// <param name="orgDto">The org dto.</param>
        private void EditSelectedOrg1(ref OrganizationDto modelODto)
        {
            //  tbOrganizationName.Text = modelODto.Name;
            cboActive.SelectedIndex = GetComboIndex(cboActive, modelODto.Active);
            //tbFirstName.Text = modelODto.AdminList[0].FirstName;
            //tbLastName.Text = modelODto.AdminList[0].LastName;
            //autoEmail.Text = modelODto.AdminList[0].Email;

            tbOrganizationName.DataContext = modelODto;
            cboActive.SelectedIndex = GetComboIndex(cboActive, modelODto.Active);
            //tbFirstName.DataContext = modelODto.AdminList[0];
            //tbLastName.DataContext = modelODto.AdminList[0];
            //autoEmail.DataContext = modelODto.AdminList[0];
            spUser.Visibility = System.Windows.Visibility.Collapsed;
            SwitchMode(UserConrolUIMode.Edit);
        }

        /// <summary>
        /// Gets the index of the combo.
        /// </summary>
        /// <param name="cboActive">The cbo active.</param>
        /// <param name="active">The active.</param>
        /// <returns></returns>

        //private int GetComboIndex(ComboBox cbo, string content)
        //{
        //    for (int i = 0; i < cbo.Items.Count; i++)
        //    {
        //        if (cbo.Items[i].Content.ToString() == content)
        //        {
        //            return i;  
        //        }
        //    }

        //    throw new Exception();     
        //}
        private int GetComboIndex(ComboBox cbo, object searchContent)
        {
            string type = searchContent.GetType().ToString();
            switch (type)
            {
                case "System.Boolean":

                    //  List<ComboBoxItem> itemList = cbo.Items as List<ComboBoxItem>;
                    ItemCollection itemList = cbo.Items;
                    //  if the content of the cxo is  String      
                    ComboBoxItem ciForType = itemList[0] as ComboBoxItem;

                    if (ciForType.Content.GetType().ToString() == "System.String")
                    {
                        string actualContentSearch;
                        for (int i = 0; i < cbo.Items.Count; i++)
                        {
                            if ((bool)searchContent == true)
                            {
                                actualContentSearch = "Yes";
                            }
                            else
                            {
                                actualContentSearch = "No";
                            }
                            ComboBoxItem ci = itemList[i] as ComboBoxItem;

                            if (ci.Content.ToString() == actualContentSearch)
                            {
                                return i;
                            }
                        }
                    }

                    break;
                default:
                    throw new Exception();
            }

            return 1;
        }

        /// <summary>
        /// Handles the Loaded event of the Organization control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        void Organization_Loaded(object sender, RoutedEventArgs e)
        {
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

            organizationViewModel = new OrganizationViewModel();
            organizationViewModel.DtoRead += new EventHandler<NotificationEventArgs<Exception>>(organizationViewModel_DtoRead);
            organizationViewModel.ReadAll();

            //dgOrg.ItemsSource = null;
            //dgOrg.ItemsSource = organizationViewModel.OrganizationDtoList;
            SwitchMode(UserConrolUIMode.ShowList);
        }

        /// <summary>
        /// Handles the DtoAdded event of the organizationViewModel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SimpleMvvmToolkit.NotificationEventArgs&lt;System.Exception&gt;" /> instance containing the event data.</param>
        void organizationViewModel_DtoAdded(object sender, NotificationEventArgs<Exception> e)
        {
            organizationViewModel = new OrganizationViewModel();
            organizationViewModel.DtoRead += new EventHandler<NotificationEventArgs<Exception>>(organizationViewModel_DtoRead);
            organizationViewModel.ReadAll();
        }

        /// <summary>
        /// Organizations the view model_ dto read.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void organizationViewModel_DtoRead(object sender, NotificationEventArgs<Exception> e)
        {
            dgOrg.ItemsSource = null;
            dgOrg.ItemsSource = organizationViewModel.OrganizationDtoList;

            SwitchMode(UserConrolUIMode.ShowList);
            waitCursor.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Organizations the view model_ dto updated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void organizationViewModel_DtoUpdated(object sender, NotificationEventArgs<Exception> e)
        {
            organizationViewModel.ReadAll();
        }

        /// <summary>
        /// Switches the UI mode.
        /// </summary>
        /// <param name="ucm">The ucm.</param>
        void SwitchMode(UserConrolUIMode ucm)
        {
            this.currentUserConrolUIMode = ucm;

            switch (ucm)
            {
                case UserConrolUIMode.Edit:

                    spOrganizationList.Visibility = System.Windows.Visibility.Collapsed;
                    spOrganizationEdit.Visibility = System.Windows.Visibility.Visible;

                    break;
                case UserConrolUIMode.ShowList:

                    spOrganizationList.Visibility = System.Windows.Visibility.Visible;
                    spOrganizationEdit.Visibility = System.Windows.Visibility.Collapsed;
                    //  refresh orgs?  

                    break;
                case UserConrolUIMode.Add:

                    spOrganizationList.Visibility = System.Windows.Visibility.Collapsed;
                    spOrganizationEdit.Visibility = System.Windows.Visibility.Visible;
                    tbOrganizationName.Text = "";
                    cboActive.SelectedIndex = 0;

                    break;
                default:
                    throw new Exception();
            }
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

            if (!tbOrganizationName.Text.IsAlphaNumericValid() || tbOrganizationName.Text == "")
            {
                tbOrganizationName.SetValidation("Please enter the organization name");
                tbOrganizationName.RaiseValidationError();
                isFormValid = false;
            }

            if (addEditMode == AddEditMode.Add)
            {
                tbFirstName.ClearValidationError();
                tbLastName.ClearValidationError();
                tbPhone.ClearValidationError();
                autoEmail.ClearValidationError();
                tbEmailAddress.ClearValidationError();


                if (!tbFirstName.Text.IsTextValid() || tbFirstName.Text == "")
                {
                    tbFirstName.SetValidation("Please enter your First name");
                    tbFirstName.RaiseValidationError();
                    isFormValid = false;
                }
                if (!tbLastName.Text.IsTextValid() || tbLastName.Text == "")
                {
                    tbLastName.SetValidation("Please enter your Last Name");
                    tbLastName.RaiseValidationError();
                    isFormValid = false;
                }
                if (!tbPhone.Text.IsPhoneNumberValid() || tbPhone.Text == "")
                {
                    tbPhone.SetValidation("Please enter your phone number (xxx)xxx-xxxx");
                    tbPhone.RaiseValidationError();
                    isFormValid = false;
                }

                if (ApplicationViewModel.Instance.AuthenticationMode.ToString().ToLower() == "windows")
                {
                    if (!tbEmailAddress.Text.IsEmailValid() || tbEmailAddress.Text == "")
                    {
                        tbEmailAddress.SetValidation("Please enter a valid e-mail address");
                        tbEmailAddress.RaiseValidationError();
                        isFormValid = false;
                        spMsg.Visibility = System.Windows.Visibility.Visible;
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
                    }
                }
            }


            return isFormValid;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            UserViewModel uvm = null;
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
                tbUserId.Text = (User.UserName == null) ? "" : User.UserName;

            }

            else
            {
                MessageBox.Show("User not found. Please try again.");
            }
        }

        void uvm_ReadUserByNameLoaded(object sender, EventArgs e)
        {
            if (((UserDTO)sender).FirstName == null)
            {
                MessageBox.Show("User doesn't exists in our database.");
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
            if (autoEmail.SelectedItem != null && ApplicationViewModel.Instance.UserNames.Contains(autoEmail.SelectedItem.ToString()))
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
    }
}