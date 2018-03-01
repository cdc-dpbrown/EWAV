/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MeansViewModel.cs
 *  Namespace:  Ewav.ViewModels    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using Ewav.Web.EpiDashboard;
using Ewav.BAL;
using Ewav.Services;
using Ewav.Web.Services;
using SimpleMvvmToolkit;
using CommonLibrary;


namespace Ewav.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class MeansViewModel : ViewModelBase<MeansViewModel>
    {
        #region Initialization and Cleanup
        // TODO: Add a member for IXxxServiceAgent
        private IMeansControlServiceAgent serviceAgent;

        /*
        * a
        * aa
        * aaaa
        * a
        * aa
        */

        public IMeansControlServiceAgent ServiceAgent
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

        // Default ctor
        public MeansViewModel()
        {
            this.serviceAgent = new MeansControlServiceAgent();
        }

        // TODO: ctor that accepts IXxxServiceAgent
        public MeansViewModel(IMeansControlServiceAgent serviceAgent)
        {
            this.serviceAgent = serviceAgent;
        }

        #endregion

        #region Notifications

        // TODO: Add events to notify the view or obtain data from the view
        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;
        public event EventHandler<NotificationEventArgs<Exception>> ColumnsLoadedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> FrequencyTableLoadedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> FreqAndCrossTableLoadedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> MeansCrossTabDataLoadedEvent;

        #endregion

        #region Properties

        private List<CrossTabResponseObjectDto> crossTabTable;

        public List<CrossTabResponseObjectDto> CrossTabTable
        {
            get
            {
                return this.crossTabTable;
            }
            set
            {
                this.crossTabTable = value;
            }
        }

        private List<FrequencyResultData> frequencyTable;

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

        private FrequencyAndCrossTable freqAndCrossTable;

        public FrequencyAndCrossTable FreqAndCrossTable
        {
            get
            {
                return this.freqAndCrossTable;
            }
            set
            {
                this.freqAndCrossTable = value;
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

        // TODO: Add methods that will be called by the view

        #endregion

        #region Completion Callbacks

        // TODO: Optionally add callback methods for async calls to the service agent

        #endregion

        #region Helpers

        // Helper method to notify View of an error
        private void NotifyError(string message, Exception error)
        {
            // Notify view of an error
            this.Notify(ErrorNotice, new NotificationEventArgs<Exception>(message, error));
        }

        #endregion

        #region Domain service Calls

        //public void GetCrossTabResults(GadgetParameters gadgetParameters)
        //{
        //    this.GetFrequencyData(gadgetParameters);
        //    this.ServiceAgent.GetCrossTabResults(gadgetParameters, MeansCrossTabDataCompleted);
        //}

        //public void GetColumns(string DataSourceName, string TableName)
        //{
        //    this.ServiceAgent.GetColumns(DataSourceName, TableName, GetColumnsCompleted);
        //}

        public void GetFrequencyData(GadgetParameters gadgetParameters)
        {
            this.ServiceAgent.GetFrequencyResults(gadgetParameters, GetFrequencyTableCompleted);
        }

        public void GetCrossTabAndFreqResults(GadgetParameters gadgetParameters)
        {
            this.ServiceAgent.GetCrossTableAndFreq(gadgetParameters, GetCrossTabAndFreqCompleted);
        }

        #endregion

        #region completed Events

        //private void MeansCrossTabDataCompleted(List<CrossTabResponseObjectDto> result, Exception e)
        //{
        //    if (e != null)
        //    {
        //        this.NotifyError("There is an error", e);
        //    }
        //    else
        //    {
        //        this.crossTabTable = result;
        //        this.Notify(MeansCrossTabDataLoadedEvent, new NotificationEventArgs<Exception>());
        //    }
        //}

        private void GetCrossTabAndFreqCompleted(FrequencyAndCrossTable result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.FreqAndCrossTable = result;
                if (this.FreqAndCrossTable != null)
                {
                    this.Notify(FreqAndCrossTableLoadedEvent, new NotificationEventArgs<Exception>());
                }
                else
                {
                    this.Notify(ErrorNotice, new NotificationEventArgs<Exception>(SharedStrings.NO_RECORDS_SELECTED));
                }
                
            }
        }

        private void GetFrequencyTableCompleted(List<FrequencyResultData> result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                this.frequencyTable = result;
                this.Notify(FrequencyTableLoadedEvent, new NotificationEventArgs<Exception>());
            }
        }

        private void GetColumnsCompleted(List<EwavColumn> result, Exception e)
        {
            if (e != null)
            {
                this.NotifyError("There is an error", e);
            }
            else
            {
                this.Columns = new List<EwavColumn>(result);
                this.Notify(ColumnsLoadedEvent, new NotificationEventArgs<Exception>());
            }
        }
        #endregion
    }
}