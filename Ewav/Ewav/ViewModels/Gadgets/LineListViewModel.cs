/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       LineListViewModel.cs
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
using Ewav.Web.Services;
using System.Collections.Generic;
using Ewav.Web.EpiDashboard;
using CommonLibrary;

namespace Ewav.ViewModels.Gadgets
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class LineListViewModel : ViewModelBase<LineListViewModel>
    {
        #region Initialization and Cleanup

        // TODO: Add a member for IXxxServiceAgent
        private ILineListControlServiceAgent serviceAgent;

        // Default ctor
        public LineListViewModel() {
            serviceAgent = new LineListControlServiceAgent();
        }

        // TODO: ctor that accepts IXxxServiceAgent
        public LineListViewModel(ILineListControlServiceAgent serviceAgent)
        {
            this.serviceAgent = serviceAgent;
        }

        #endregion

        #region Notifications

        // TODO: Add events to notify the view or obtain data from the view
        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;
        public event EventHandler<NotificationEventArgs<Exception>> TableLoadedEvent;
        #endregion

        #region Properties

        // TODO: Add properties using the mvvmprop code snippet

        public List<DatatableBag> ListOfTables { get; set; }

        #endregion

        #region Methods

        // TODO: Add methods that will be called by the view

        public void GetLineList(GadgetParameters gp) 
        {
            this.serviceAgent.GenerateLineList(gp, GetLineListCompleted);
        }

        #endregion

        #region Completion Callbacks

        // TODO: Optionally add callback methods for async calls to the service agent

        private void GetLineListCompleted(List<DatatableBag> result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                this.ListOfTables = result;
                if (this.ListOfTables.Count == 0)
                {
                    Exception exp = new Exception(SharedStrings.NO_RECORDS_SELECTED.ToString());

                    this.Notify(ErrorNotice, new NotificationEventArgs<Exception>("", exp));
                }
                else
                {
                    Notify(TableLoadedEvent, new NotificationEventArgs<Exception>());
                }
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

        #endregion
    }
}