/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ErrorMessageViewModel.cs
 *  Namespace:  Ewav.ViewModels.Gadgets    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Windows;
using System.Threading;
using System.Collections.ObjectModel;

// Toolkit namespace
using SimpleMvvmToolkit;

// Toolkit extension methods
using SimpleMvvmToolkit.ModelExtensions;
using Ewav.Services;

namespace Ewav.ViewModels.Gadgets
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class ErrorMessageViewModel : ViewModelBase<ErrorMessageViewModel>
    {
        #region Initialization and Cleanup

        // TODO: Add a member for IXxxServiceAgent
        private IErrorMessageServiceAgent serviceAgent;

        // Default ctor
        public ErrorMessageViewModel() {
            this.serviceAgent = new ErrorMessageServiceAgent();
        }

        // TODO: ctor that accepts IXxxServiceAgent
        public ErrorMessageViewModel(IErrorMessageServiceAgent serviceAgent)
        {
            this.serviceAgent = serviceAgent;
        }

        #endregion

        #region Notifications

        // TODO: Add events to notify the view or obtain data from the view
        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;
        public event EventHandler<NotificationEventArgs<Exception>> SendEmailMessageCompleted;

        #endregion

        #region Properties

        // TODO: Add properties using the mvvmprop code snippet

        #endregion

        #region Methods

        public void EmailErrorMessage(string Message) 
        {
            serviceAgent.EmailErrorMessage(Message, GetFrequencyTableCompleted);
        }

        #endregion

        #region Completion Callbacks

        private void GetFrequencyTableCompleted(bool result, Exception e)
        {
            //if (result)
            //{
            //    SendEmailMessageCompleted(result, null);
            //}
            //else
            //{
            //    ErrorNotice(result, new NotificationEventArgs<Exception>("Sending Email failed.", e));
            //}

        }
        
        #endregion

        #region Helpers

        // Helper method to notify View of an error
        private void NotifyError(string message, Exception error)
        {
            // Notify view of an error
            Notify(ErrorNotice, new NotificationEventArgs<Exception>(message, error));
        }

        #endregion
    }
}