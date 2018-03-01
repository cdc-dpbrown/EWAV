/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       DeleteDash.xaml.cs
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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Ewav.ViewModels;
using Ewav.Views.Dialogs;

namespace Ewav
{
    public partial class DeleteDash : ChildWindow
    {
        ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteDash" /> class.
        /// </summary>
        public DeleteDash()
        {
            InitializeComponent();
            applicationViewModel.DeleteCanvasCompletedEvent += new Client.Application.DeleteCanvasCompletedEventHandler(applicationViewModel_DeleteCanvasCompletedEvent);
            if (applicationViewModel.IsCurrentCanvasShared)
            {
                msgDescription.Text = "You have shared this dashboard with other users. Are you sure you want to delete it?";
            }
            canvasName.Text = applicationViewModel.SelectedCanvasName;
        }


        /// <summary>
        /// Applications the view model_ delete canvas completed event.
        /// </summary>
        /// <param name="o">The o.</param>
        void applicationViewModel_DeleteCanvasCompletedEvent(object o)
        {
            applicationViewModel.CleanupDashboard();
            applicationViewModel.IsCurrentCanvasShared = false;
            applicationViewModel.CurrentCanvasId = -1;
            msgDescription.Text = "Dashboard " + canvasName.Text + " has been deleted. Dashboard will be reset.";
            applicationViewModel.SelectedCanvasName = "";
            deleteSp.Visibility = System.Windows.Visibility.Collapsed;
            spMsg_Success.Visibility = System.Windows.Visibility.Collapsed;
            continueSp.Visibility = System.Windows.Visibility.Visible;
            canvasName.Text = applicationViewModel.SelectedCanvasName;
        }



        /// <summary>
        /// Handles the Click event of the OKButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationViewModel.Instance.DemoMode)
            {
                DemoMode dm = new DemoMode();
                dm.Show();
                return;
            }



            applicationViewModel.DeleteCanvas(applicationViewModel.CurrentCanvasId);
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
        /// Handles the Click event of the btnContinue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}