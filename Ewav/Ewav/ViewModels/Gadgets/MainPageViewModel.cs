/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MainPageViewModel.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using SimpleMvvmToolkit;

// Toolkit extension methods
namespace Ewav
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class MainPageViewModel : ViewModelBase<MainPageViewModel>
    {
        public MainPageViewModel()
        {
            // Register to get notified when a message arrives
            // at the MessageBus using the navigation message token
            this.RegisterToReceiveMessages(MessageTokens.Navigation, OnNavigationRequested);
        }

        #region Notifications

        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;

        #endregion

        #region Properties

        // Use the mvvmprop code snippet to add a SelectedPage property of type Uri
        private Uri selectedPage;
        public Uri SelectedPage
        {
            get
            {
                return this.selectedPage;
            }
            set
            {
                this.selectedPage = value;
                this.NotifyPropertyChanged(m => m.SelectedPage);
            }
        }

        #endregion

        #region Methods

        // Add Navigate method with string parameter
        // Create relative uri and set SelectedPage property to it
        private void Navigate(string pageName)
        {
            Uri pageUri = new Uri(pageName, UriKind.Relative);
            this.SelectedPage = pageUri;
        }

        // Create a callback method (EventHandler<NotificationEventArgs)
        // in which you call Navigate, passing e.Message
        public void OnNavigationRequested(object sender, NotificationEventArgs e)
        {
            this.Navigate(e.Message);
        }

        #endregion

        #region Commands

        // Add a command using the mvvmcommand snippet that calls Navigate
        private DelegateCommand<string> navigateCommand;
        public DelegateCommand<string> NavigateCommand
        {
            get
            {
                if (this.navigateCommand == null)
                {
                    this.navigateCommand =
                        new DelegateCommand<string>(Navigate);
                }
                return this.navigateCommand;
            }
            private set
            {
                this.navigateCommand = value;
            }
        }

        #endregion Commands

        #region Helpers

        // Helper method to notify View of an error
        private void NotifyError(string message, Exception error)
        {
            // Notify view of an error
            this.Notify(ErrorNotice, new NotificationEventArgs<Exception>(message, error));
        }
        #endregion
    }
}