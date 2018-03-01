/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       AdminMenuView.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Windows.Controls;
using Ewav.ViewModels;

using Ewav.BAL;
using System.Windows;
using System.Xml.Linq;
using Ewav.Client.Application;

namespace Ewav
{
    public partial class AdminMenuView : UserControl
    {
        public ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;
        public event EventHandler AdminViewClickEvent;
            
        public AdminMenuView()
        {
            this.InitializeComponent();
            this.Loaded += new System.Windows.RoutedEventHandler(AppMenuView_Loaded);
            applicationViewModel.LoggedInUser.UserChanged += new EventHandler(LoggedInUser_UserChanged);
        }

        private void ManageUsers_Click(object sender, RoutedEventArgs e)
        {
            AdminViewClickEvent(sender, new EventArgs());
        }

        private void ManageOrg_Click(object sender, RoutedEventArgs e)
        {
            AdminViewClickEvent(sender, new EventArgs());
        }


        private void ManageDataSources_Click(object sender, RoutedEventArgs e)
        {
            AdminViewClickEvent(sender, new EventArgs());
        }

        void LoggedInUser_UserChanged(object sender, EventArgs e)
        {


            if (applicationViewModel.LoggedInUser.HighestRolesEnum == Membership.RolesEnum.SuperAdministrator)
            {
                ManageOrg.Visibility = System.Windows.Visibility.Visible;
                ManageUsers.Visibility = System.Windows.Visibility.Visible;
                ManageDataSources.Visibility = System.Windows.Visibility.Visible;
                CopyDashboard.Visibility = System.Windows.Visibility.Visible;        

            }
            if (applicationViewModel.LoggedInUser.HighestRolesEnum == Membership.RolesEnum.Administrator)
            {


                ManageUsers.Visibility = System.Windows.Visibility.Visible;
                ManageDataSources.Visibility = System.Windows.Visibility.Visible;
                CopyDashboard.Visibility = System.Windows.Visibility.Visible;        



            }

        }
        void AppMenuView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {


                tbVersion.Text = "Version " + applicationViewModel.AssemblyVersion;


                if (applicationViewModel.DemoMode)
                {
                    AppNameText2.Text = "Epi Info Cloud Data Analytics Administration (Demo Mode)";
                }
                else
                {
                    AppNameText2.Text = "Epi Info Cloud Data Analytics Administration ";
                }




                ManageOrg.Visibility = System.Windows.Visibility.Collapsed;
                ManageUsers.Visibility = System.Windows.Visibility.Collapsed;
                ManageDataSources.Visibility = System.Windows.Visibility.Collapsed;
                CopyDashboard.Visibility = System.Windows.Visibility.Collapsed;    


                AppMenuViewModel appMenuViewModel = (AppMenuViewModel)this.DataContext;


            }
            catch (System.Exception ex)
            {
                throw new Exception("aa");
            }
        }

        private void CopyDashboard_Click(object sender, RoutedEventArgs e)
        {


            AdminViewClickEvent(sender, new EventArgs());




        }


    
    }

}