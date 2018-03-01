/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ResetDash.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Ewav.ViewModels;

namespace Ewav
{
    public partial class ResetDash : ChildWindow
    {
        ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;
        public ResetDash()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            //  JUst clears gadget leaves filters ahd vars intact     
            applicationViewModel.ResetDashBoard();

            //applicationViewModel.IsReset = true;
            //applicationViewModel.CleanupDashboard();
            //applicationViewModel.IsReset = false;
            
            //applicationViewModel.CurrentCanvasId = -1;
            this.DialogResult = true;
        }
    }
}