/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       CombinedFrequencyViewModel.cs
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
using Ewav.Web.EpiDashboard;
using Ewav.Web.Services;

namespace Ewav.ViewModels.Gadgets
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class CombinedFrequencyViewModel : ViewModelBase<CombinedFrequencyViewModel>
    {
        #region Initialization and Cleanup

        // TODO: Add a member for IXxxServiceAgent
        private ICombinedFrequencyServiceAgent serviceAgent;

        private DatatableBag frequencyTable;

        public DatatableBag FrequencyTable
        {
            get { return frequencyTable; }
            set { frequencyTable = value; }
        }


        // Default ctor
        public CombinedFrequencyViewModel() {
            this.serviceAgent = new CombinedFrequencyServiceAgent();
        }

        // TODO: ctor that accepts IXxxServiceAgent
        public CombinedFrequencyViewModel(ICombinedFrequencyServiceAgent serviceAgent)
        {
            this.serviceAgent = serviceAgent;
        }

        #endregion

        #region Notifications

        // TODO: Add events to notify the view or obtain data from the view
        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;
        public event EventHandler<NotificationEventArgs<Exception>> FrequencyTableLoadedEvent;
        #endregion

        #region Properties

        // TODO: Add properties using the mvvmprop code snippet

        #endregion

        #region Methods

        public void GetCombinedFrequency(EwavCombinedFrequencyGadgetParameters combinedParameters, string groupVar, GadgetParameters gp)
        {
            this.serviceAgent.GetCombinedFrequencyResults( combinedParameters, groupVar, gp, 
                ApplicationViewModel.Instance.EwavDatafilters, 
                ApplicationViewModel.Instance.EwavDefinedVariables, 
                ApplicationViewModel.Instance.FilterString, 
                GetCombinedFrequencyCompleted);
        }

        #endregion

        #region Completion Callbacks

        /// <summary>
        /// Completion for frequency Table
        /// </summary>
        /// <param name="result"></param>
        /// <param name="e"></param>
        private void GetCombinedFrequencyCompleted(DatatableBag result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.FrequencyTable = result;
                //if (frequencyTable.Count < applicationViewModel.EwavSelectedDatasource.TotalRecords)
                //{
                if (FrequencyTable.FieldsList.Fields.Count == 0)
                {
                    this.Notify(ErrorNotice, new NotificationEventArgs<Exception>("No Data Selected", e));// new NotificationEventArgs<Exception>());
                }
                else
                {
                    this.Notify(FrequencyTableLoadedEvent, new NotificationEventArgs<Exception>());
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