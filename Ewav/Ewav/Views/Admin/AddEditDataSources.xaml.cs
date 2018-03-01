/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       AddEditDataSources.xaml.cs
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
using System.Windows.Media;
using Ewav.ViewModels;
//using Ewav.Views.Admin;
using Ewav.Membership;
using Ewav.DTO;
using System.Collections.ObjectModel;
using Ewav.Client.Application;
using Ewav.BAL;
using Ewav.ExtensionMethods;
using Ewav.Views.Admin;
using Ewav.Views.Dialogs;

namespace Ewav
{
    /// <summary>
    /// Child Window to Add Edit Datasource
    /// </summary>
    public partial class AddEditDataSources : ChildWindow//UserControl
    {
        AdminDatasourceViewModel adminVM = null;

        Connection con = null;

        DatasourceDto localDto = null;

        ObservableCollection<UserDTO> availableList;
        ObservableCollection<UserDTO> selectedList;
        ObservableCollection<UserDTO> OriginalList;

        public int OrganizationId { get; set; }
        public event CUDDatasourceCompletedEventHandler CUDDatasourceCompletedEvent;

        public DatasourceDto SelectedDatasourceDto { get; set; }

        //public List<UserDTO> AssociatedUsers { get; set; }

        public ModeEnum Mode { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddEditDataSources" /> class.
        /// </summary>
        public AddEditDataSources()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(AddEditDataSources_Loaded);
            selectedList = new ObservableCollection<UserDTO>();
            availableList = new ObservableCollection<UserDTO>();
            OriginalList = new ObservableCollection<UserDTO>();
        }

        /// <summary>
        /// Handles the Loaded event of the AddEditDataSources control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        void AddEditDataSources_Loaded(object sender, RoutedEventArgs e)
        {
            DataBaseTypeEnum dbenum = DataBaseTypeEnum.MySQL;
            cmbDBType.ItemsSource = System.Enum.GetValues(dbenum.GetType());

            if (SelectedDatasourceDto != null)
            {
                adminVM = new AdminDatasourceViewModel();
                adminVM.ReadAssociatedUsers(this.SelectedDatasourceDto.DatasourceId, this.SelectedDatasourceDto.OrganizationId);
                adminVM.ReadAssociatedUsersCompletedEvent -= new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(adminVM_ReadAssociatedUsersCompletedEvent);
                adminVM.ReadAssociatedUsersCompletedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(adminVM_ReadAssociatedUsersCompletedEvent);
                PopulatePage();

            }
            else
            {
                //SelectedDatasourceDto = new DatasourceDto();
                adminVM = new AdminDatasourceViewModel();
                ReadAllUsers();
            }


        }

        /// <summary>
        /// Reads all users.
        /// </summary>
        private void ReadAllUsers()
        {
            adminVM.ReadUsers(OrganizationId);
            adminVM.ReadUsersCompletedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(adminVM_ReadUsersCompletedEvent);
        }

        /// <summary>
        /// Admins the V m_ read associated users completed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void adminVM_ReadAssociatedUsersCompletedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            List<UserDTO> Users = (List<UserDTO>)sender;

            selectedList = new ObservableCollection<UserDTO>(Users.Where(t1 => t1.UserID != User.Instance.UserDto.UserID && t1.UserRoleInOrganization.ToLower() == "analyst"));


            ReadAllUsers();

        }

        /// <summary>
        /// Populates the page.
        /// </summary>
        private void PopulatePage()
        {
            if (this.SelectedDatasourceDto.Connection != null)
            {
                chkEpiForm.IsChecked = (bool)this.SelectedDatasourceDto.IsEpiInfoForm;
                tbDSName.Text = this.SelectedDatasourceDto.DatasourceName;
                tbServerName.Text = this.SelectedDatasourceDto.Connection.ServerName;
                tbDbName.Text = this.SelectedDatasourceDto.Connection.DatabaseName;
                tbUserId.Text = this.SelectedDatasourceDto.Connection.UserId;
                tbPassword.Password = this.SelectedDatasourceDto.Connection.Password;
                //tbTableName.Text = this.SelectedDatasourceDto.Connection.DatabaseObject;
                tbPort.Text = this.SelectedDatasourceDto.Connection.PortNumber.ToString();


                if (this.SelectedDatasourceDto.IsActive)
                {
                    radEnable.IsChecked = true;
                }
                else
                {
                    radDisable.IsChecked = true;
                }

                if (this.SelectedDatasourceDto.Connection.DatabaseObject.LastIndexOf(" ") <= 0)
                {
                    radTableNm.IsChecked = true;
                    tbTableName.Text = this.SelectedDatasourceDto.Connection.DatabaseObject;
                }
                else
                {
                    radSql.IsChecked = true;
                    tbScript.Text = this.SelectedDatasourceDto.Connection.DatabaseObject;
                }

                //if (this.SelectedDatasourceDto.Connection.DatabaseType == DataBaseTypeEnum.SQLServer)
                //{
                //    cmbDBType.SelectedItem = DataBaseTypeEnum.SQLServer
                //}
                //else
                //{
                //    radMySql.IsChecked = true;
                //}

                switch (this.SelectedDatasourceDto.Connection.DatabaseType)
                {
                    case DataBaseTypeEnum.MySQL:
                        cmbDBType.SelectedItem = DataBaseTypeEnum.MySQL;
                        chkEpiForm.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case DataBaseTypeEnum.PostgreSQL:
                        cmbDBType.SelectedItem = DataBaseTypeEnum.PostgreSQL;
                        chkEpiForm.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case DataBaseTypeEnum.SQLServer:
                        cmbDBType.SelectedItem = DataBaseTypeEnum.SQLServer;
                        chkEpiForm.Visibility = System.Windows.Visibility.Visible;
                        break;
                    default:
                        chkEpiForm.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                }
            }


        }

        /// <summary>
        /// Reads the rest of available users.
        /// </summary>
        /// <param name="selectedList">The selected list.</param>
        private void ReadRestOfAvailableUsers(ObservableCollection<UserDTO> selectedList)
        {
            availableList = new ObservableCollection<UserDTO>(availableList.Where(t => selectedList.All(t1 => t.UserID != t1.UserID)));
        }

        /// <summary>
        /// Admins the V m_ read users completed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void adminVM_ReadUsersCompletedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            if (sender != null)
            {
                OriginalList = new ObservableCollection<UserDTO>((List<UserDTO>)sender);

                availableList = new ObservableCollection<UserDTO>(OriginalList.Where(t1 => t1.UserID != User.Instance.UserDto.UserID && t1.UserRoleInOrganization.ToLower() == "analyst")); //take the logged in user and admins out the list

                if (SelectedDatasourceDto != null)
                {
                    ReadRestOfAvailableUsers(selectedList);
                }

                BindListBoxes();
            }
            else
            {
                btnStep1Next.IsEnabled = false;
            }
        }

        /// <summary>
        /// Binds the list boxes.
        /// </summary>
        private void BindListBoxes()
        {
            lbxAvailable.ItemsSource = null;
            lbxAvailable.ItemsSource = availableList;// this.AssociatedUsers;
            lbxAvailable.DisplayMemberPath = "FullName";
            lbxAvailable.SelectedValuePath = "UserId";

            lbxSelected.ItemsSource = null;
            lbxSelected.ItemsSource = selectedList;
            lbxSelected.DisplayMemberPath = "FullName";
            lbxSelected.SelectedValuePath = "UserId";
        }

        /// <summary>
        /// Handles the Click event of the btnTestConn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void btnTestConn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ApplicationViewModel.Instance.DemoMode)
            {
                MessageBox.Show("Connection is successful");
                btnStep1Next.IsEnabled = true;
                localDto = new DatasourceDto();
                localDto.DatasourceName = tbDSName.Text;

                return;
            }

            if (ValidateStep1())
            {
                //string connStr = CreateConnectionString();
                con = new Connection();
                con.ServerName = tbServerName.Text;
                con.DatabaseName = tbDbName.Text;
                con.UserId = tbUserId.Text;
                con.Password = tbPassword.Password;
                con.PortNumber = tbPort.Text;

                //if (radMySql.IsChecked == true)
                //{
                //    con.DatabaseType = DataBaseTypeEnum.MySQL;
                //}
                //else
                //{
                //    con.DatabaseType = DataBaseTypeEnum.SQLServer;
                //}

                switch (cmbDBType.SelectedItem.ToString())
                {
                    case "MySQL":
                        con.DatabaseType = DataBaseTypeEnum.MySQL;
                        break;
                    case "PostgreSQL":
                        con.DatabaseType = DataBaseTypeEnum.PostgreSQL;
                        break;
                    case "SQLServer":
                        con.DatabaseType = DataBaseTypeEnum.SQLServer;
                        break;
                    default:
                        break;
                }

                btnTestConn.IsEnabled = false;
                adminVM.TestConnection(con);
                adminVM.TestConnectionCompletedEvent -= new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(adminVM_TestConnectionCompletedEvent);
                adminVM.TestConnectionCompletedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(adminVM_TestConnectionCompletedEvent);
            }


        }

        /// <summary>
        /// Validates the step1.
        /// </summary>
        /// <returns></returns>
        private bool ValidateStep1()
        {
            tbDSName.ClearValidationError();
            tbServerName.ClearValidationError();
            tbDbName.ClearValidationError();
            tbUserId.ClearValidationError();
            tbPassword.ClearValidationError();
            //tbPort.ClearValidationError();

            bool isFormValid = true;

            if (tbDSName.Text.ToString().Trim() == "")//!tbDSName.Text.IsAlphaNumericValid() ||
            {
                tbDSName.SetValidation("Enter correct data source Name.");
                tbDSName.RaiseValidationError();
                isFormValid = false;
            }

            if (tbServerName.Text.ToString().Trim() == "")//!tbServerName.Text.IsAlphaNumericValid() || 
            {
                tbServerName.SetValidation("Enter correct Server Name.");
                tbServerName.RaiseValidationError();
                isFormValid = false;
            }
            if (tbDbName.Text.ToString().Trim() == "")//!tbDbName.Text.IsAlphaNumericValid() || 
            {
                tbDbName.SetValidation("Enter correct Database name.");
                tbDbName.RaiseValidationError();
                isFormValid = false;
            }
            if (tbUserId.Text.ToString().Trim() == "")//!tbUserId.Text.IsAlphaNumericValid() || 
            {
                tbUserId.SetValidation("Enter correct User name.");
                tbUserId.RaiseValidationError();
                isFormValid = false;
            }
            if (tbPassword.Password.ToString().Trim() == "")//!tbPassword.Password.IsAlphaNumericValid() || 
            {
                tbPassword.SetValidation("Enter correct Password.");
                tbPassword.RaiseValidationError();
                isFormValid = false;
            }

            //if (tbPort.Text.ToString().Trim() == "")
            //{
            //    tbPort.SetValidation("Enter correct Port number.");
            //    tbPort.RaiseValidationError();
            //    isFormValid = false;
            //}
            return isFormValid;
        }

        /// <summary>
        /// Vaildates the step2.
        /// </summary>
        /// <returns></returns>
        private bool VaildateStep2()
        {
            tbTableName.ClearValidationError();
            //if (tbTableName.Text == "")//!tbTableName.Text.IsAlphaNumericValid() || 
            //{
            //    tbTableName.SetValidation("Enter correct Table name.");
            //    tbTableName.RaiseValidationError();
            //    return false;
            //}


            if (radTableNm.IsChecked == true && (tbTableName.Text.LastIndexOf(" ") > 0 || tbTableName.Text == ""))
            {
                tbTableName.SetValidation("Enter correct table name.");
                tbTableName.RaiseValidationError();
                return false;
            }

            if (radSql.IsChecked == true && (tbScript.Text.LastIndexOf(" ") < 0 || tbScript.Text == ""))
            {
                tbScript.SetValidation("Enter correct SQL script.");
                tbScript.RaiseValidationError();
                return false;
            }




            return true;
        }


        /// <summary>
        /// Admins the V m_ test connection completed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void adminVM_TestConnectionCompletedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            btnTestConn.IsEnabled = true;
            //if (e.Message.ToString().ToLower() == "true")
            if ((bool)sender)
            {
                MessageBox.Show("Connection is successful");
                btnStep1Next.IsEnabled = true;
                localDto = new DatasourceDto();
                localDto.DatasourceName = tbDSName.Text;


                //if (rad)
                //{

                //}

                //localDto.DatasourceConnectionString = con.GetConnectionString();
                //localDto.DatasourceType = con.DatabaseType;
                //localDto.DatabaseName = con.DatabaseName;
                //localDto.ServerName = con.ServerName;
                Connection newCon = con;
                localDto.Connection = newCon;
            }
            else
            {
                btnStep1Next.IsEnabled = false;
                MessageBox.Show("Test Connection has failed.");
            }
        }


        /// <summary>
        /// Handles the Click event of the btnTestData control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnTestData_Click(object sender, RoutedEventArgs e)
        {

            if (ApplicationViewModel.Instance.DemoMode)
            {
                MessageBox.Show("Successfully Tested data");
                btnStep2Next.IsEnabled = true;

                return;
            }

            if (VaildateStep2())
            {
                if (radTableNm.IsChecked == true)
                {
                    con.DatabaseObject = tbTableName.Text.Trim();
                }
                else if (radSql.IsChecked == true)
                {
                    con.DatabaseObject = tbScript.Text.Trim();
                }


                btnTestData.IsEnabled = false;
                adminVM.TestData(con);
                adminVM.TestDataCompletedEvent -= new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(adminVM_TestDataCompletedEvent);
                adminVM.TestDataCompletedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(adminVM_TestDataCompletedEvent);
            }
        }

        /// <summary>
        /// Admins the V m_ test data completed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void adminVM_TestDataCompletedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            btnTestData.IsEnabled = true;
            if (((bool)sender))
            {
                MessageBox.Show("Successfully Tested data");
                btnStep2Next.IsEnabled = true;
            }
            else
            {
                btnStep2Next.IsEnabled = false;
                MessageBox.Show("Test Data has failed.");
            }
        }


        /// <summary>
        /// Handles the Click event of the btnStep1Next control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void btnStep1Next_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DataBaseTypeEnum dbTypeSelected = (DataBaseTypeEnum)(Enum.Parse(typeof(DataBaseTypeEnum), cmbDBType.SelectedItem.ToString(), true));
            if (ApplicationViewModel.Instance.IsEpiWebIntegrationEnabled &&
                dbTypeSelected == DataBaseTypeEnum.SQLServer &&
                (bool)chkEpiForm.IsChecked)//Only if integration is enabled. && for SQLServer 
            {
                localDto.IsEpiInfoForm = true;
                EWEDatasourceDto EWEDsDTO = new EWEDatasourceDto();
                EWEDsDTO.DatabaseName = localDto.Connection.DatabaseName;

                adminVM.ReadEWEDatasourceFormId(EWEDsDTO);
                adminVM.ReadEweFormIdCompletedEvent -= new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(adminVM_ReadEweFormIdCompletedEvent);
                adminVM.ReadEweFormIdCompletedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(adminVM_ReadEweFormIdCompletedEvent);


            }
            else
            {
                localDto.IsEpiInfoForm = false;
                NavigateToStep2();
            }


        }

        void adminVM_ReadEweFormIdCompletedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {

            if (sender == null || sender.ToString() == string.Empty)
            {
                MessageBox.Show("Epi Info Form project couldnt be found in the system. Contact your system administrator");
                return;
            }
            else
            {

                NavigateToStep2();
            }
        }

        private void NavigateToStep2()
        {
            Storyboard1.Begin();
            //spSource.Visibility = System.Windows.Visibility.Collapsed;
            // spTable.Visibility = System.Windows.Visibility.Visible;
            // spAssocUser.Visibility = System.Windows.Visibility.Collapsed;
            tbStep1.Foreground = new SolidColorBrush(Color.FromArgb(255, 122, 122, 122));
            rectStep1.Fill = new SolidColorBrush(Color.FromArgb(255, 122, 122, 122));
            tbStep2.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            rectStep2.Fill = new SolidColorBrush(Color.FromArgb(255, 38, 198, 48));
        }

        /// <summary>
        /// Handles the Click event of the btnStep2Next control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void btnStep2Next_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Storyboard2.Begin();
            //spSource.Visibility = System.Windows.Visibility.Collapsed;
            //spTable.Visibility = System.Windows.Visibility.Collapsed;
            //spAssocUser.Visibility = System.Windows.Visibility.Visible;
            tbStep1.Foreground = new SolidColorBrush(Color.FromArgb(255, 122, 122, 122));
            rectStep1.Fill = new SolidColorBrush(Color.FromArgb(255, 122, 122, 122));
            tbStep2.Foreground = new SolidColorBrush(Color.FromArgb(255, 122, 122, 122));
            rectStep2.Fill = new SolidColorBrush(Color.FromArgb(255, 122, 122, 122));
            tbStep3.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            rectStep3.Fill = new SolidColorBrush(Color.FromArgb(255, 38, 198, 48));

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
                //MessageBox.Show("New Datasources cannot be added in DEMO mode.");
                return;
            }

            localDto.OrganizationId = OrganizationId;
            localDto.DatasourceName = tbDSName.Text;
            localDto.AssociatedUsers = new List<UserDTO>();
            List<UserDTO> selectList = selectedList.ToList<UserDTO>();
            List<UserDTO> adminList = new ObservableCollection<UserDTO>(OriginalList.Where(t1 => t1.UserRoleInOrganization.ToLower() == "administrator" || t1.UserRoleInOrganization.ToLower() == "superadministrator")).ToList<UserDTO>();

            selectList.AddRange(adminList);

            //localDto.AssociatedUsers.AddRange(selectList);

            localDto.AssociatedUsers = selectedList.ToList<UserDTO>();

            if (radEnable.IsChecked == true)
            {
                localDto.IsActive = true;
            }
            else
            {
                localDto.IsActive = false;
            }

            FinishProcessing();
        }

        /// <summary>
        /// Finishes the processing.
        /// </summary>
        private void FinishProcessing()
        {
            if (this.SelectedDatasourceDto != null)
            {
                localDto.DatasourceId = this.SelectedDatasourceDto.DatasourceId;
                //localDto.CreatorID = this.SelectedDatasourceDto.CreatorID;
                adminVM.Update(localDto);
                adminVM.UpdateCompletedEvent -= new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(adminVM_UpdateCompletedEvent);
                adminVM.UpdateCompletedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(adminVM_UpdateCompletedEvent);
            }
            else
            {
                adminVM.Add(localDto);
                adminVM.AddCompletedEvent -= new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(adminVM_AddCompletedEvent);
                adminVM.AddCompletedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(adminVM_AddCompletedEvent);
            }

        }

        /// <summary>
        /// Admins the V m_ update completed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void adminVM_UpdateCompletedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            HideShowErrMessage(sender);

            RequestRead();
        }

        /// <summary>
        /// Admins the V m_ add completed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void adminVM_AddCompletedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            HideShowErrMessage(sender);
            if ((bool)sender)
            {
                RequestRead();
                this.DialogResult = true;
            }
            //else
            //{
            //    this.DialogResult = false;
            //}

        }

        /// <summary>
        /// Hides the show err message.
        /// </summary>
        /// <param name="sender">The sender.</param>
        private void HideShowErrMessage(object sender)
        {
            if ((bool)sender)
            {
                spMsg.Visibility = System.Windows.Visibility.Collapsed;
                this.DialogResult = true;
            }
            else
            {
                errMsg.Text = "Please enter a different name for the data source.";
                spMsg.Visibility = System.Windows.Visibility.Visible;
            }
        }

        /// <summary>
        /// Requests the read.
        /// </summary>
        private void RequestRead()
        {
            adminVM.Read(OrganizationId);
            adminVM.ReadCompletedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(adminVM_ReadCompletedEvent);
        }

        /// <summary>
        /// Admins the V m_ read completed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void adminVM_ReadCompletedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            BindGrid(sender);
        }

        /// <summary>
        /// Binds the grid.
        /// </summary>
        /// <param name="sender">The sender.</param>
        private void BindGrid(object sender)
        {
            CUDDatasourceCompletedEvent(sender);
        }


        /// <summary>
        /// Handles the Click event of the btnAddSource control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnAddSource_Click(object sender, RoutedEventArgs e)
        {
            UserDTO item = (UserDTO)lbxAvailable.SelectedItem;
            if (item != null)
            {
                selectedList.Add(item);
                availableList.Remove(item);
            }
            BindListBoxes();
        }

        /// <summary>
        /// Handles the Click event of the btnRemoveSource control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnRemoveSource_Click(object sender, RoutedEventArgs e)
        {
            UserDTO item = (UserDTO)lbxSelected.SelectedItem;
            if (item != null)
            {
                availableList.Add(item);
                selectedList.Remove(item);
            }
            BindListBoxes();
        }

        /// <summary>
        /// Handles the Click event of the btnCancel2 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnCancel2_Click(object sender, RoutedEventArgs e)
        {

            //spSource.Visibility = System.Windows.Visibility.Visible;
            spTable.Visibility = System.Windows.Visibility.Collapsed;
            spAssocUser.Visibility = System.Windows.Visibility.Collapsed;
            tbStep1.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            rectStep1.Fill = new SolidColorBrush(Color.FromArgb(255, 38, 198, 48));
            tbStep2.Foreground = new SolidColorBrush(Color.FromArgb(255, 122, 122, 122));
            rectStep2.Fill = new SolidColorBrush(Color.FromArgb(255, 122, 122, 122));
            tbStep3.Foreground = new SolidColorBrush(Color.FromArgb(255, 122, 122, 122));
            rectStep3.Fill = new SolidColorBrush(Color.FromArgb(255, 122, 122, 122));
            Storyboard4.Begin();

        }

        /// <summary>
        /// Handles the Click event of the btnCancel3 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnCancel3_Click(object sender, RoutedEventArgs e)
        {
            spSource.Visibility = System.Windows.Visibility.Collapsed;
            // spTable.Visibility = System.Windows.Visibility.Visible;
            spAssocUser.Visibility = System.Windows.Visibility.Collapsed;
            tbStep1.Foreground = new SolidColorBrush(Color.FromArgb(255, 122, 122, 122));
            rectStep1.Fill = new SolidColorBrush(Color.FromArgb(255, 122, 122, 122));
            tbStep2.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            rectStep2.Fill = new SolidColorBrush(Color.FromArgb(255, 38, 198, 48));
            Storyboard5.Begin();
        }

        /// <summary>
        /// Handles the Click event of the btnCancel1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void btnCancel1_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DialogResult = false;
        }


        private void cmbDBType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectedDatasourceDto != null && this.SelectedDatasourceDto.Connection != null)
            {
                tbPort.Text = this.SelectedDatasourceDto.Connection.PortNumber.ToString();
            }
            else
            {


                DataBaseTypeEnum dbTypeSelected = (DataBaseTypeEnum)(Enum.Parse(typeof(DataBaseTypeEnum), cmbDBType.SelectedItem.ToString(), true));

                switch (dbTypeSelected)
                {
                    case DataBaseTypeEnum.MySQL:
                        tbPort.Text = DatabaseDefaultPorts.MySQL;
                        chkEpiForm.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case DataBaseTypeEnum.PostgreSQL:
                        tbPort.Text = DatabaseDefaultPorts.PostgreSQL;
                        chkEpiForm.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case DataBaseTypeEnum.SQLServer:
                        tbPort.Text = DatabaseDefaultPorts.SQLServer;
                        if (ApplicationViewModel.Instance.IsEpiWebIntegrationEnabled)
                        {
                            chkEpiForm.Visibility = System.Windows.Visibility.Visible;
                        }

                        break;
                    default:
                        chkEpiForm.Visibility = System.Windows.Visibility.Collapsed;
                        break;

                }
            }
        }

        private void radTableNm_Checked(object sender, RoutedEventArgs e)
        {
            tbTableName.Visibility = System.Windows.Visibility.Visible;
            tbScript.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void radSql_Checked(object sender, RoutedEventArgs e)
        {
            tbTableName.Visibility = System.Windows.Visibility.Collapsed;
            tbScript.Visibility = System.Windows.Visibility.Visible;
        }

    }

    /// <summary>
    /// Enumeration for Mode
    /// </summary>
    public enum ModeEnum
    {
        Add = 0,
        Edit
    }
}