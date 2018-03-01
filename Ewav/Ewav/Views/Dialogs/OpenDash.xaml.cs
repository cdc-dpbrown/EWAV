/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       OpenDash.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Ewav.ExtensionMethods;
using Ewav.ViewModels;
using Ewav.Web.Services;
using Ewav.BAL;
using System.Collections.Generic;

namespace Ewav
{

    public partial class OpenDash : ChildWindow
    {
        ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;
        DatatableBag results;

        public List<EwavDatasourceDto> Datasources { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenDash" /> class.
        /// </summary>
        public OpenDash()
        {
            InitializeComponent();

            applicationViewModel.CanvasListLoadedEvent -= new Client.Application.CanvasListLoadedEventHandler(applicationViewModel_CanvasListLoadedEvent);
            applicationViewModel.CanvasListLoadedEvent += new Client.Application.CanvasListLoadedEventHandler(applicationViewModel_CanvasListLoadedEvent);
            applicationViewModel.CanvasDtoListForLoggedUser = new System.Collections.Generic.List<CanvasDto>();
            int userId = -1;
            userId = Convert.ToInt32(applicationViewModel.LoggedInUser.UserDto.UserID.ToString());

            //datasourceId = Convert.ToInt32(applicationViewModel.LoggedInUser.UserDto.DatasourceID.ToString());

            applicationViewModel.LoadCanvasUserList(userId);
            waitCursor.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Applications the view model_ canvas list loaded event.
        /// </summary>
        /// <param name="o">The o.</param>
        void applicationViewModel_CanvasListLoadedEvent(object o)
        {
            results = (DatatableBag)o;
            if (applicationViewModel.CanvasDtoListForLoggedUser.Count == 0)
            {
                spMsg.Visibility = System.Windows.Visibility.Visible;
                tbSaveError.Text = "You do not have any saved dashboards.";
                //btnOpenDB.IsEnabled = false;
            }
            else
            {
                spMsg.Visibility = System.Windows.Visibility.Collapsed;
                cb1.ItemsSource = null;
                cb1.ItemsSource = new DashboardInfoCollection(applicationViewModel.CanvasDtoListForLoggedUser);
                //btnOpenDB.IsEnabled = true;
            }
            waitCursor.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Handles the Click event of the btnBegin control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void btnBegin_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            this.DialogResult = false;
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// Handles the Click event of the btnOpen control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void btnOpen_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            int selectedIndex = -1;

            if (ApplicationViewModel.Instance.CurrentCanvasId > 0)
            {
                MessageBoxResult result = MessageBox.Show("You are about to open another dashboard. Any unsaved changes will be lost. Would you like to continue?", "Warning", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.Cancel)
                {
                    this.DialogResult = false;
                    return;
                }
            }

            for (int i = 0; i < applicationViewModel.EwavDatasources.Count; i++)
            {
                if (ApplicationViewModel.Instance.EwvSelectedDatasourceNameCandidate ==
                    applicationViewModel.EwavDatasources[i].DatasourceName)
                {
                    //  ApplicationViewModel.Instance.EwavSelectedDatasource = applicationViewModel.EwavDatasources[i];
                    ApplicationViewModel.Instance.EwavDatasourceSelectedIndex = i;
                    //  applicationViewModel.ColumnsLoadedEvent += new Client.Application.ColumnsLoadedEventEventHandler(applicationViewModel_ColumnsLoadedEvent);
                    waitCursor.Visibility = System.Windows.Visibility.Visible;
                    applicationViewModel.DatasourceChangedEvent -= new Client.Application.DatasourceChangedEventHandler(applicationViewModel_DatasourceChangedEvent);
                    applicationViewModel.DatasourceChangedEvent += new Client.Application.DatasourceChangedEventHandler(applicationViewModel_DatasourceChangedEvent);
                    selectedIndex = i;
                    break;
                }
            }

            if (selectedIndex < 0) //replaced <= with <
            {
                WarningWindow error = new WarningWindow("You do not have access to this data source. Please contact the owner of the dashboard to request access.", "");
                error.Show();
                //ErrorWindow Err = new ErrorWindow("You don't have access to their datasource. Contact System Adminstrator.", "");
                //Err.Show();
                return;
            }






        }

        void applicationViewModel_DatasourceChangedEvent(object o, Client.Application.DatasourceChangedEventArgs e)
        {


            applicationViewModel.DatasourceChangedEvent -= new Client.Application.DatasourceChangedEventHandler(applicationViewModel_DatasourceChangedEvent);


            int canvasId = SearchCanvasId(applicationViewModel.SelectedCanvasName);

            if (canvasId != -1)
            {
                applicationViewModel.LoadCanvas(canvasId);
            }
            waitCursor.Visibility = System.Windows.Visibility.Collapsed;
            this.DialogResult = true;
        }

        void applicationViewModel_ColumnsLoadedEvent(object o, Client.Application.ColumnsLoadedEventEventArgs e)
        {


        }

        /// <summary>
        /// Searches the canvas id.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        private int SearchCanvasId(string p)
        {
            for (int i = 0; i < applicationViewModel.CanvasDtoListForLoggedUser.Count; i++)
            {
                if (p == applicationViewModel.CanvasDtoListForLoggedUser[i].CanvasName)
                {
                    return Convert.ToInt32(applicationViewModel.CanvasDtoListForLoggedUser[i].CanvasId.ToString());
                }
            }
            return -1;
        }

        private void cb1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                btnOpenDB.IsEnabled = true;
        }
    }
}