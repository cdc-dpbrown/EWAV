/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       BinomialStatCalcViewModel.cs
 *  Namespace:  Ewav.ViewModels    
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
using Ewav.Web.Services;

namespace Ewav.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class BinomialStatCalcViewModel : ViewModelBase<BinomialStatCalcViewModel>
    {
        #region Initialization and Cleanup

        // TODO: Add a member for IXxxServiceAgent
        private IBinomialServiceAgent serviceAgent;

        // Default ctor
        public BinomialStatCalcViewModel() {
            serviceAgent = new BinomialServiceAgent();
        }

        // TODO: ctor that accepts IXxxServiceAgent
        public BinomialStatCalcViewModel(IBinomialServiceAgent serviceAgent)
        {
            this.serviceAgent = serviceAgent;
        }

        #endregion

        #region Notifications

        // TODO: Add events to notify the view or obtain data from the view
        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;
        public event EventHandler<NotificationEventArgs<Exception>> StatCalcLoadedEvent;

        #endregion

        #region Properties

        // TODO: Add properties using the mvvmprop code snippet

        private BinomialStatCalcDTO dataBag;

        public BinomialStatCalcDTO DataBag
        {
            get { return dataBag; }
            set { dataBag = value; }
        }

        #endregion

        #region Methods

        // TODO: Add methods that will be called by the view

        #endregion

        #region Completion Callbacks

        // TODO: Optionally add callback methods for async calls to the service agent
        /// <summary>
        /// Completion for allColumns
        /// </summary>
        /// <param name="result"></param>
        /// <param name="e"></param>

        private void GetBinomialStatCalcCompleted(BinomialStatCalcDTO result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                this.DataBag = result;
                Notify(StatCalcLoadedEvent, new NotificationEventArgs<Exception>());
            }
        }
        #endregion

        #region Helpers

        // Helper method to notify View of an error
        private void NotifyError(string message, Exception error)
        {
            // Notify view of an error
            Notify(ErrorNotice, new NotificationEventArgs<Exception>(message, error));
        }

        public void GetBinomialStatCalc(string txtNumerator, string txtObserved, string txtExpected) 
        {
            serviceAgent.GetBinomialStatCalc(txtNumerator, txtObserved, txtExpected, GetBinomialStatCalcCompleted);
        }

        #endregion
    }
}