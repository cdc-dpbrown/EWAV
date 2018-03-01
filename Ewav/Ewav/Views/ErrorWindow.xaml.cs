/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ErrorWindow.xaml.cs
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
using Ewav.ViewModels.Gadgets;
using System.Configuration;
using Ewav.ViewModels;

namespace Ewav
{
    public partial class ErrorWindow : ChildWindow
    {
        public ErrorWindow(Exception e)
        {
            ErrorMessageViewModel emv = new ErrorMessageViewModel();
            InitializeComponent();
            if (e != null)
            {
                ErrorTextBox.Text = e.Message + Environment.NewLine + Environment.NewLine + e.StackTrace;

                if (ApplicationViewModel.Instance.SendEmailOnException)
                {
                    emv.EmailErrorMessage(ErrorTextBox.Text);
                }

                //emv.ErrorNotice += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(emv_ErrorNotice);
                //emv.SendEmailMessageCompleted += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(emv_SendEmailMessageCompleted);

                if (ApplicationViewModel.Instance.EnableExceptionDetail)
                {
                    ContentStackPanel.Visibility = System.Windows.Visibility.Visible;
                }
            }


        }


        public ErrorWindow(Uri uri)
        {
            InitializeComponent();
            if (uri != null)
            {
                ErrorTextBox.Text = "Page not found: \"" + uri.ToString() + "\"";
            }

            if (ApplicationViewModel.Instance.EnableExceptionDetail)
            {
                ContentStackPanel.Visibility = System.Windows.Visibility.Visible;
            }

        }

        public ErrorWindow(string message, string details)
        {
            InitializeComponent();
            ErrorTextBox.Text = message + Environment.NewLine + Environment.NewLine + details;


            ContentStackPanel.Visibility = System.Windows.Visibility.Visible;

        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}