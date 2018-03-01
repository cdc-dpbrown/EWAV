/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       DataSources.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Windows;
using System.Windows.Controls;
using Ewav.DTO;
using Ewav.ExtensionMethods;
using Ewav.Membership;
using Ewav.ViewModels;
using Ewav.ViewModels.Membership;
using System.Collections.Generic;

namespace Ewav
{
    /// <summary>
    /// Datasource class
    /// </summary>
    public partial class DataSources : UserControl
    {
        public ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;
        AdminDatasourceViewModel adminDSVM = null;
        UserViewModel uvm = null;
        AddEditDataSources addEdit = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="DataSources" /> class.
        /// </summary>
        public DataSources()
        {
            this.InitializeComponent();

            uvm = new UserViewModel();
            uvm.ReadAllOrgsForUser(User.Instance.UserDto.UserID);
            uvm.ReadAllOrgsForUserComplete += new UserViewModel.ReadAllOrgsForUserCompleteHandler(uvm_ReadAllOrgsForUserComplete);

            applicationViewModel.LoggedInUser.SelectedOrganizationChanged += new EventHandler(LoggedInUser_SelectedOrganizationChanged);
        }

        /// <summary>
        /// Adds the edit_ CUD datasource completed event.
        /// </summary>
        /// <param name="o">The o.</param>
        void addEdit_CUDDatasourceCompletedEvent(object o)
        {
            // BindGrid(o);
            dgDataSource.Refresh(o);

            spMsg.Visibility = System.Windows.Visibility.Visible;

            if (addEdit.Mode == ModeEnum.Add)
            {
                tbErrMsg.Text = "Datasource " + addEdit.tbDSName.Text + " has been added. ";
            }
            else
            {
                tbErrMsg.Text = "Datasource " + addEdit.tbDSName.Text + " has been updated.";
            }
            waitCursor.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Admins the V m_ read completed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void adminVM_ReadCompletedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {


            var dto = sender as List<DatasourceDto>;

            if (applicationViewModel.DemoMode)
            {

                foreach (DatasourceDto item in dto)
                {

                    item.Connection.ServerName = "<server name withheld> ";


                }

            }

            dgDataSource.Refresh(dto);

        }

        /// <summary>
        /// Handles the Click event of the Add control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void Add_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            addEdit = new AddEditDataSources();
            addEdit.Mode = ModeEnum.Add;
            //addEdit.SelectedDatasourceDto = new DatasourceDto();
            addEdit.OrganizationId = ((OrganizationDto)cmbOrgName.SelectedItem).Id;
            addEdit.CUDDatasourceCompletedEvent += new Client.Application.CUDDatasourceCompletedEventHandler(addEdit_CUDDatasourceCompletedEvent);
            addEdit.Closed += new EventHandler(addEdit_Closed);
            addEdit.Show();
        }

        void addEdit_Closed(object sender, EventArgs e)
        {
            if (addEdit.DialogResult == true)
            {
                waitCursor.Visibility = System.Windows.Visibility.Visible;
            }

        }

        /// <summary>
        /// Handles the DropDownClosed event of the cmbOrgName control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void cmbOrgName_DropDownClosed(object sender, EventArgs e)
        {
            OrganizationDto oDto = ((ComboBox)sender).SelectedItem as OrganizationDto;
            // the current org for this user is now oDto.OrganizationId     
            applicationViewModel.LoggedInUser.SetOrganizationId(oDto.Id);
        }

        /// <summary>
        /// Handles the Click event of the Delete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
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
                var dsdto = ctl.DataContext as DatasourceDto;
                if (null != dsdto)
                {
                    EditDatasource(dsdto);
                }
            }
        }

        /// <summary>
        /// Edits the selected user.
        /// </summary>
        /// <param name="userDto">The user dto.</param>
        private void EditDatasource(DatasourceDto dsdto)
        {
            addEdit = new AddEditDataSources();

            if (applicationViewModel.DemoMode)
            {
                dsdto.Connection.ServerName = "<server name withheld> ";
            }


            //addEdit.UserMode = ClientCommon.UserOperationModeEnum.Edit;
            addEdit.Title = "Edit Data Source";
            addEdit.SelectedDatasourceDto = dsdto;
            addEdit.OrganizationId = ((OrganizationDto)cmbOrgName.SelectedItem).Id;
            //addEdit.UserDto = dsdto;
            //addEdit.CurrentUserActionEnum = Organization.UserActionEnum.EditExistOrg | Organization.UserActionEnum.EditUserOfExistOrg;
            addEdit.Mode = ModeEnum.Edit;
            addEdit.CUDDatasourceCompletedEvent += new Client.Application.CUDDatasourceCompletedEventHandler(addEdit_CUDDatasourceCompletedEvent);
            //addEdit.Mode = AddEditUsers.ModeType.Edit;
            addEdit.Closed += new EventHandler(addEdit_Closed);
            addEdit.Show();
        }

        /// <summary>
        /// Handles the SelectedOrganizationChanged event of the LoggedInUser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        void LoggedInUser_SelectedOrganizationChanged(object sender, EventArgs e)
        {
            //read all the users in my organization including superadmin, admin and analysts.           
            int orgId = Convert.ToInt32(sender); //((OrganizationDto)cmbOrgName.SelectedItem).Id;
            adminDSVM.Read(orgId);
            spMsg.Visibility = System.Windows.Visibility.Collapsed;
            //  NOTE:  
            //  A listener has already been wired to this event so it does not have to be wired again ( memeory leak  )     
            //  uvm.ReadAllUsersLoaded += new EventHandler(uvm_ReadAllUsersLoaded);
        }

        /// <summary>
        /// Uvm_s the read all orgs for user complete.
        /// </summary>
        /// <param name="userOrganizationDtoList">The user organization dto list.</param>
        /// <param name="ex">The ex.</param>
        void uvm_ReadAllOrgsForUserComplete(System.Collections.Generic.List<OrganizationDto> userOrganizationDtoList, Exception ex)
        {
            cmbOrgName.ItemsSource = userOrganizationDtoList;
            cmbOrgName.DisplayMemberPath = "Name";
            cmbOrgName.SelectedValuePath = "Id";
            cmbOrgName.SelectedIndex = 0;

            // After we get all orgs for the user get all datasources  for this org     
            adminDSVM = new AdminDatasourceViewModel();
            int orgId = ((OrganizationDto)cmbOrgName.SelectedItem).Id;
            adminDSVM.Read(orgId);
            adminDSVM.ReadCompletedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(adminVM_ReadCompletedEvent);

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
    }
}