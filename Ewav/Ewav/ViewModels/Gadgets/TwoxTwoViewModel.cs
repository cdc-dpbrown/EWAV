/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       TwoxTwoViewModel.cs
 *  Namespace:  Ewav.ViewModels    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Ewav.Web.EpiDashboard;
using Ewav.BAL;
using Ewav.Services;
using Ewav.Web.Services;
using SimpleMvvmToolkit;
using CommonLibrary;
using System.Text;


// Toolkit extension methods
namespace Ewav.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class TwoxTwoViewModel : ViewModelBase<TwoxTwoViewModel>
    {
        private ObservableCollection<String> _stringCollection = new ObservableCollection<String>();
        public ObservableCollection<string> StringCollection
        {
            get
            {
                return _stringCollection;
            }
            set
            {
                _stringCollection = value;
            }
        }

        private ObservableCollection<TwoxTwoAndMxNResultsSet> _observableResultSets = new ObservableCollection<TwoxTwoAndMxNResultsSet>();
        public ObservableCollection<TwoxTwoAndMxNResultsSet> ObservableResultSets
        {
            get
            {
                return _observableResultSets;
            }
            set
            {
                _observableResultSets = value;
            }
        }

        private ObservableCollection<DatatableBag> _datatableBagResultSet = new ObservableCollection<DatatableBag>();
        public ObservableCollection<DatatableBag> DatatableBagResultSet
        {
            get
            {
                return _datatableBagResultSet;
            }
            set
            {
                _datatableBagResultSet = value;
            }
        }

        private String simpleTag = "Simple_Tag";

        #region Initialization and Cleanup
        public ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;

        private TwoxTwoControlServiceAgent serviceAgent;

        private TwoxTwoAndMxNResultsSet twoxTwoAndMxNResultsSet;

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

        public TwoxTwoControlServiceAgent ServiceAgent
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
        public TwoxTwoViewModel()
        {
            this.serviceAgent = new TwoxTwoControlServiceAgent();
        }

        // TODO: ctor that accepts IXxxServiceAgent
        public TwoxTwoViewModel(ITwoxTwoControlServiceAgent serviceAgent)
        {
            this.serviceAgent = serviceAgent as TwoxTwoControlServiceAgent;
        }

        #endregion

        #region Notifications

        // TODO: Add events to notify the view or obtain data from the view
        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;
        public event EventHandler<NotificationEventArgs<Exception>> ColumnsLoadedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> FrequencyTableLoadedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> ColumnsMetaDataLoadedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> MeansCrossTabDataLoadedEvent;

        public event EventHandler<NotificationEventArgs<Exception>> SetupGadgetEvent;

        #endregion

        #region Properties

        #endregion

        #region Methods

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

        public string SimpleTag
        {
            get
            {
                return simpleTag;
            }

            set
            {
                simpleTag = value;
            }
        }

        public void GetCrossTabResults(string DataSourceName, string TableName, GadgetParameters gadgetParameters)
        {
            this.ServiceAgent.GetCrossTabResults(DataSourceName, TableName, gadgetParameters, MeansCrossTabDataCompleted);
        }

        public void GetColumns(string DataSourceName, string TableName)
        {
            this.ServiceAgent.GetColumns(DataSourceName, TableName, GetColumnsCompleted);
        }

        private void Get2x2FilteredColumns()
        {
            this.Notify(ColumnsLoadedEvent, new NotificationEventArgs<Exception>());
        }

        public void SetupGadget(GadgetParameters gadgetParameters)
        {
            this.ObservableResultSets.Clear();
            foreach (string mainVariableName in gadgetParameters.MainVariableNames)
            {
                this.ServiceAgent.SetupGadget(gadgetParameters, SetupGadgetCompleted);
            }
        }

        private void SetupGadgetCompleted(TwoxTwoAndMxNResultsSet resulSet, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
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

                    this.Notify(ErrorNotice, new NotificationEventArgs<Exception>("",exp));
                }
                else
                {
                    this.TwoxTwoAndMxNResultsSet = resulSet;
                    this.DatatableBagResultSet = new ObservableCollection<DatatableBag>(resulSet.DatatableBagArray);

                    this.ObservableResultSets.Add(resulSet); 
                    this.Notify(SetupGadgetEvent, new NotificationEventArgs<Exception>());        
                }
                
            }
            SimpleTag = "SetupGadgetCompleted";
        }

        private void GetColumnsCompleted(List<EwavColumn> result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                this.AllColumns = new List<EwavColumn>(result);
                this.Notify(ColumnsLoadedEvent, new NotificationEventArgs<Exception>());
            }
        }

        public void GetFrequencyData(string DataSourceName, string TableName, GadgetParameters gadgetParameters)
        {
            this.ServiceAgent.GetFrequencyResults(DataSourceName, TableName, gadgetParameters,
                applicationViewModel.EwavDatafilters,
                GetFrequencyTableCompleted);
        }

        private void MeansCrossTabDataCompleted(List<CrossTabResponseObjectDto> result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                this.crossTabTable = result;
                this.Notify(MeansCrossTabDataLoadedEvent, new NotificationEventArgs<Exception>());
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

        internal string checkType(string p)
        {
            return "";
        }
    }
}