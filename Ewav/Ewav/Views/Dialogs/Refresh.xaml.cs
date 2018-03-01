/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       Refresh.xaml.cs
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
using Ewav.Common;

namespace Ewav
{
    public partial class RefreshDash : ChildWindow
    {
        public RefreshDash()
        {
            this.InitializeComponent();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            DatasourceWatcher.Instance.ManualReload();        

            this.DialogResult = true;      
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            int secs = 0;

            switch (this.cmbInterval.SelectedIndex)
            {
                case 0:
                    secs = 10;
                    break;
                case 1:
                    secs = 30;
                    break;
                case 2:
                    secs = 60;
                    break;
                case 3:
                    secs = 120;
                    break;
                case 4:
                    secs = 180;
                    break;
                case 5:
                    secs = 240;
                    break;
                case 6:
                    secs = 300;
                    break;
                case 7:
                    secs = 1800;
                    break;
                case 8:
                    secs = 3600;
                    break;

            }

            RefreshTimer.Instance.Seconds = secs;

            RefreshTimer.Instance.Start();    
              
            if (this.cbAutoreferesh.IsChecked == true)
            {
                //    RefreshTimer.Instance.Start();        
                DatasourceWatcher.Instance.ReloadIfChanged = true;
            }
            else
            {
                //   RefreshTimer.Instance.Stop();    
                DatasourceWatcher.Instance.ReloadIfChanged = false;
            }

            this.DialogResult = true;      
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}