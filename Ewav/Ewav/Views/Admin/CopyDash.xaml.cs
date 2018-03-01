using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Ewav.ExtensionMethods;
using Ewav.Web.Services;
using Ewav.ViewModels;
using Ewav.Views.Dialogs;

namespace Ewav
{
    public partial class CopyDash : UserControl
    {

        Ewav.ViewModels.ApplicationViewModel applicationViewModel = Ewav.ViewModels.ApplicationViewModel.Instance;
        AppMenuViewModel appMenuViewModel = null;
        public CopyDash()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(CopyDash_Loaded);
        }

        void CopyDash_Loaded(object sender, RoutedEventArgs e)
        {

            applicationViewModel.CanvasDtoListForLoggedUser = new System.Collections.Generic.List<CanvasDto>();
            applicationViewModel.CanvasListLoadedEvent -= new Client.Application.CanvasListLoadedEventHandler(applicationViewModel_CanvasListLoadedEvent);
            applicationViewModel.CanvasListLoadedEvent += new Client.Application.CanvasListLoadedEventHandler(applicationViewModel_CanvasListLoadedEvent);

            applicationViewModel.LoadCanvasUserList(applicationViewModel.LoggedInUser.UserDto.UserID);

            appMenuViewModel = new AppMenuViewModel();
            appMenuViewModel.GetDatasourcesAsIEnumerable2();
            appMenuViewModel.DatasourcesLoadedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(appMenuViewModel_DatasourcesLoadedEvent);

            waitCursor.Visibility = System.Windows.Visibility.Visible;

        }

        void applicationViewModel_CanvasListLoadedEvent(object o)
        {

            DatatableBag results = (DatatableBag)o;
            if (applicationViewModel.CanvasDtoListForLoggedUser.Count == 0)
            {
                spMsg.Visibility = System.Windows.Visibility.Visible;
                //   tbSaveError.Text = "You do not have any saved dashboards.";

            }
            else
            {

                spMsg.Visibility = System.Windows.Visibility.Collapsed;
                cmbSavedDash.ItemsSource = null;
                cmbSavedDash.ItemsSource = new DashboardInfoCollection(applicationViewModel.CanvasDtoListForLoggedUser);
                cmbSavedDash.SelectedIndex = -1;

            }
            waitCursor.Visibility = System.Windows.Visibility.Collapsed;


        }

        /// <summary>
        /// Apps the menu view model_ datasources loaded event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void appMenuViewModel_DatasourcesLoadedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            appMenuViewModel = (AppMenuViewModel)sender;

            ApplicationViewModel.Instance.EwavDatasources = appMenuViewModel.Datasources2;

            if (appMenuViewModel.Datasources2.Count > 1)
            {
                this.cmbDataSource.ItemsSource = appMenuViewModel.Datasources2;
                this.cmbDataSource.SelectedValue = "DatasourceID";
                this.cmbDataSource.DisplayMemberPath = "DatasourceNoCamelName";

                this.cmbDataSource.SelectedIndex = -1;
            }
            else
            {
                spMsg.Visibility = System.Windows.Visibility.Visible;
                errMsg.Text = "There are no data sources assigned to you. Please contact the Adminstrator for your organization.";
                btnCopyDash.IsEnabled = false;
            }
            waitCursor.Visibility = System.Windows.Visibility.Collapsed;

        }



        private void cmbSavedDash_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ValidateForm();
        }



        private void ValidateForm()
        {
            if (cmbSavedDash.SelectedIndex > -1 &&
                cmbDataSource.SelectedIndex > -1 &&
                txtCanvasName.Text.Length > 0)
            {
                btnCopyDash.IsEnabled = true;
            }
            else
            {
                btnCopyDash.IsEnabled = false;
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            cmbDataSource.SelectedIndex = -1;
            cmbSavedDash.SelectedIndex = -1;
            txtCanvasName.Text = "";
            spMsg.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void txtCanvasName_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateForm();
        }

        private void btnCopyDash_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationViewModel.Instance.DemoMode)
            {
                DemoMode dm = new DemoMode();
                dm.Show();
                return;
            }


            AdminDatasourceViewModel AdminDSVM = new AdminDatasourceViewModel();
            string OldCanvasName = cmbSavedDash.SelectedValue.ToString();
            //int CanvasId = ((CanvasDto)cmbSavedDash.SelectedValue).DatasourceID;
            string OldDatasourceName = ((DashboardInfo)cmbSavedDash.SelectedItem).DataSource.ToString();

            //int OldDatasourceId = applicationViewModel.EwavDatasources.First(ds => ds.DatasourceName == currentDsName).DatasourceID;

            string NewCanvasName = txtCanvasName.Text;
            //int UserId = applicationViewModel.LoggedInUser.UserDto.UserID;
            //int NewDatasourceId = ((Ewav.BAL.EwavDatasourceDto)(cmbDataSource.SelectedValue)).DatasourceID;

            string NewDatasourceName = ((Ewav.BAL.EwavDatasourceDto)(cmbDataSource.SelectedValue)).DatasourceName;

            AdminDSVM.CopyDashboard(OldCanvasName, NewCanvasName, OldDatasourceName, NewDatasourceName);
            AdminDSVM.CopyDashboardCompletedEvent -= new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(AdminDSVM_CopyDashboardCompletedEvent);
            AdminDSVM.CopyDashboardCompletedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(AdminDSVM_CopyDashboardCompletedEvent);
            AdminDSVM.ErrorNotice -= new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(AdminDSVM_ErrorNotice);
            AdminDSVM.ErrorNotice += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(AdminDSVM_ErrorNotice);
        }

        void AdminDSVM_ErrorNotice(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            spMsg.Visibility = System.Windows.Visibility.Visible;
            imgSMsg.Visibility = System.Windows.Visibility.Collapsed;
            imgerrMsg.Visibility = System.Windows.Visibility.Visible;
            errMsg.Visibility = System.Windows.Visibility.Visible;
            errMsg.Text = (e.Data).Message;
            spMsg.Background = new SolidColorBrush(Color.FromArgb(255, 241, 202, 194)); //pink BK to use in case of error
            errMsg.Foreground = new SolidColorBrush(Color.FromArgb(255, 96, 25, 25)); //dark pink text color to use in case of error.
        }

        void AdminDSVM_CopyDashboardCompletedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            if (sender.ToString().Equals("success"))
            {
                spMsg.Visibility = System.Windows.Visibility.Visible;
                imgerrMsg.Visibility = System.Windows.Visibility.Collapsed;
                imgSMsg.Visibility = System.Windows.Visibility.Visible;
                errMsg.Visibility = System.Windows.Visibility.Visible;
                errMsg.Text = "Dashboard has been copied.";
                spMsg.Background = new SolidColorBrush(Color.FromArgb(255, 220, 236, 187)); //Green BK to use in case of success.
                errMsg.Foreground = new SolidColorBrush(Color.FromArgb(255, 68, 88, 24)); //dark Green text color to use in case of success.
            }
            else
            {
                spMsg.Visibility = System.Windows.Visibility.Visible;
                imgSMsg.Visibility = System.Windows.Visibility.Collapsed;
                imgerrMsg.Visibility = System.Windows.Visibility.Visible;
                errMsg.Visibility = System.Windows.Visibility.Visible;
                errMsg.Text = "Dashboard could not be copied. " + sender.ToString();
                spMsg.Background = new SolidColorBrush(Color.FromArgb(255, 241, 202, 194)); //pink BK to use in case of error
                errMsg.Foreground = new SolidColorBrush(Color.FromArgb(255, 96, 25, 25)); //dark pink text color to use in case of error.
            }
        }


    }
}
