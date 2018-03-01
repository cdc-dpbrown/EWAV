/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       XYChartViewModel.cs
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
using Ewav.BAL;
using Ewav.Services;
using Ewav.Web.Services;
using SimpleMvvmToolkit;

// Toolkit extension methods
using SimpleMvvmToolkit.ModelExtensions;
using System.Collections.Generic;
using Ewav.Web.EpiDashboard;
using CommonLibrary;

namespace Ewav.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class XYChartViewModel : ViewModelBase<XYChartViewModel>
    {
        #region Initialization and Cleanup

        // TODO: Add a member for IXxxServiceAgent
        private IXYChartServiceAgent serviceAgent;

        // Default ctor
        public XYChartViewModel() { }

        private List<FrequencyResultData> frequencyTable;

        // TODO: ctor that accepts IXxxServiceAgent
        public XYChartViewModel(IXYChartServiceAgent serviceAgent)
        {
            this.serviceAgent = serviceAgent;
        }

        #endregion

        #region Notifications

        /// <summary>
        /// Gets or sets the frequency table.
        /// </summary>
        /// <value>The frequency table.</value>
        public List<FrequencyResultData> FrequencyTable
        {
            get
            {
                return this.frequencyTable;
            }
            set
            {
                this.frequencyTable = value;
            }
        }

        // TODO: Add events to notify the view or obtain data from the view
        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;
        public event EventHandler<NotificationEventArgs<Exception>> FrequencyTableLoadedEvent;

        #endregion

        #region Properties

        public IXYChartServiceAgent ServiceAgent
        {
            get
            {
                return this.serviceAgent;
            }
            set
            {
                this.serviceAgent = value;
            }
        }

        private List<EwavColumn> columns;

        public List<EwavColumn> Columns
        {
            get
            {
                return this.columns;
            }
            set
            {
                this.columns = value;
            }
        }


        #endregion

        #region Methods

        public void GetFrequencyData(GadgetParameters gadgetParameters)
        {
            this.ServiceAgent.GetFrequencyResults(gadgetParameters, GetFrequencyTableCompleted);
        }

        private void GetFrequencyTableCompleted(List<FrequencyResultData> result, Exception e)
        {
            if (e != null)
            {
                this.Notify(ErrorNotice, new NotificationEventArgs<Exception>("", e));
            }
            else
            {
                if (result == null || result.Count == 0)
                {
                    Exception exp = new Exception(SharedStrings.NO_RECORDS_SELECTED.ToString());

                    this.Notify(ErrorNotice, new NotificationEventArgs<Exception>("", exp));
                }
                else
                {
                    this.frequencyTable = result;
                    this.Notify(FrequencyTableLoadedEvent, new NotificationEventArgs<Exception>());
                }
            }
        }

        #endregion

        #region Completion Callbacks

        // TODO: Optionally add callback methods for async calls to the service agent

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