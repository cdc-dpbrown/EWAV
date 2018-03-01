/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MxNViewModel.cs
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
using System.Text;
using CommonLibrary;


// Toolkit extension methods
namespace Ewav.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class MxNViewModel : ViewModelBase<MxNViewModel>
    {
        #region Initialization and Cleanup
        private ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;

        // TODO: Add a member for IXxxServiceAgent
        private Services.MxNControlServiceAgent serviceAgent;

        private TwoxTwoAndMxNResultsSet twoxTwoAndMxNResultsSet;

        /// <summary>
        /// Gets the frequency data.
        /// </summary>
        /// <param name="selectedDatasourceName">Name of the selected datasource.</param>
        /// <param name="selectedDatasourceTableName">Name of the selected datasource table.</param>
        /// <param name="gadgetParameters">The gadget parameters.</param>
        //public void GetFrequencyData(string selectedDatasourceName, string selectedDatasourceTableName,
        //    EpiDashboard.GadgetParameters gadgetParameters)
        //{
        //    this.ServiceAgent.GetFrequencyResults(
        //        gadgetParameters,   applicationViewModel.EwavDatafilters,  
        //        GetFrequencyTableCompleted);
        //}

        /// <summary>
        /// Gets or sets the service agent.
        /// </summary>
        /// <value>The service agent.</value>
        public Services.MxNControlServiceAgent ServiceAgent
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

        public void SetupGadget(GadgetParameters gadgetParameters)
        {
            this.ServiceAgent.SetupGadget(gadgetParameters, SetupGadgetCompleted);
        }

        /// <summary>
        /// Gets or sets the twox two results set.
        /// </summary>
        /// <value>The twox two results set.</value>
        public TwoxTwoAndMxNResultsSet TwoxTwoAndMxNResultsSet
        {
            get
            {
                return this.twoxTwoAndMxNResultsSet;
            }
            set
            {
                this.twoxTwoAndMxNResultsSet = value;
            }
        }

        private void SetupGadgetCompleted(TwoxTwoAndMxNResultsSet resulSet, Exception e)
        {
            if (resulSet == null)
            {
                Exception exp = new Exception(SharedStrings.NO_RECORDS_SELECTED);
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", exp);
                this.Notify(ErrorNotice, notification);
            }
            else if (resulSet.Errors.Count > 0)
            {
                StringBuilder errorString = new StringBuilder();
                for (int i = 0; i < resulSet.Errors.Count; i++)
                {
                    errorString.Append(" " + resulSet.Errors[i].VarName);
                }

                Exception exp = new Exception(errorString.ToString());

                this.Notify(ErrorNotice, new NotificationEventArgs<Exception>("", exp));
            }
            else if (resulSet.FreqResultsDescriptiveStatistics == null)
            {
                Exception exp = new Exception(SharedStrings.NO_RECORDS_SELECTED.ToString());

                this.Notify(ErrorNotice, new NotificationEventArgs<Exception>("", exp));
            }
            else
            {
                this.TwoxTwoAndMxNResultsSet = resulSet;
                this.Notify(SetupGadgetEvent, new NotificationEventArgs<Exception>());
            }

        }

        #endregion

        // Default ctor
        public MxNViewModel()
        {
        }

        //   TODO: ctor that accepts IXxxServiceAgent
        public MxNViewModel(MxNControlServiceAgent serviceAgent)
        {
            this.serviceAgent = serviceAgent;
        }

        #region Notifications

        //public void GetColumns(string DataSourceName, string TableName)
        //{
        //    //if (this.applicationViewModel.AllColumns == null)
        //    //{
        //        this.ServiceAgent.GetColumns(DataSourceName, TableName, GetColumnsCompleted);
        //    //}
        //    //else
        //    //{
        //    //    this.Get2x2FilteredColumns();
        //    //}
        //}

        private void Get2x2FilteredColumns()
        {
           //   this.allColumns = this.applicationViewModel.AllColumns;

            this.Notify(ColumnsLoadedEvent, new NotificationEventArgs<Exception>());
        }

        // TODO: Add events to notify the view or obtain data from the view
        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;
        public event EventHandler<NotificationEventArgs<Exception>> ColumnsLoadedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> FrequencyTableLoadedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> ColumnsMetaDataLoadedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> MeansCrossTabDataLoadedEvent;

        public event EventHandler<NotificationEventArgs<Exception>> SetupGadgetEvent;

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

        private List<EwavColumn> allColumns;

        public List<EwavColumn> AllColumns
        {
            get
            {
                return this.allColumns;
            }
            set
            {
                this.allColumns = value;
            }
        }

        #endregion

        #region Methods

        // TODO: Add methods that will be called by the view

        #endregion

        #region Completion Callbacks

        // TODO: Optionally add callback methods for async calls to the service agent

        //private void GetColumnsCompleted(List<EwavColumn> result, Exception e)
        //{
        //    if (e != null)
        //    {
        //        this.NotifyError("There is an error", e);
        //    }
        //    else
        //    {
        //        this.AllColumns = new List<EwavColumn>(result);
        //   //        this.applicationViewModel.AllColumns = this.AllColumns;
        //        this.Notify(ColumnsLoadedEvent, new NotificationEventArgs<Exception>());
        //    }
        //}

        //private void GetFrequencyTableCompleted(List<FrequencyResultData> result, Exception e)
        //{
        //    if (e != null)
        //    {
        //        this.NotifyError("There is an error", e);
        //    }
        //    else
        //    {
        //        this.frequencyTable = result;
        //        this.Notify(FrequencyTableLoadedEvent, new NotificationEventArgs<Exception>());
        //    }
        //}

        #endregion

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