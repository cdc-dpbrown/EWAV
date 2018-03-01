/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       LogisticRegressionViewModel.cs
 *  Namespace:  Ewav.ViewModels    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
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
    public class LogisticRegressionViewModel : ViewModelBase<LogisticRegressionViewModel>
    {
        #region Initialization and Cleanup
        // TODO: Add a member for IXxxServiceAgent
        private ILogisticRegressionServiceAgent serviceAgent;

        public ILogisticRegressionServiceAgent ServiceAgent
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
        public LogisticRegressionViewModel()
        {
        }

        // TODO: ctor that accepts IXxxServiceAgent
       
        public LogisticRegressionViewModel(ILogisticRegressionServiceAgent logisticRegressionServiceAgent)
        {
            // TODO: Complete member initialization
            this.serviceAgent = logisticRegressionServiceAgent;
        }

        #endregion

        #region Notifications

        // TODO: Add events to notify the view or obtain data from the view
        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;
        // public event EventHandler<NotificationEventArgs<Exception>> TableLoadedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> ColumnsLoadedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> RegressTableLoadedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> RegressResultsLoadedEvent;

        #endregion

        #region Properties

        // TODO: Add properties using the mvvmprop code snippet

        private LogRegressionResults regressionResults;

        public LogRegressionResults RegressionResults
        {
            get
            {
                return this.regressionResults;
            }
            set
            {
                this.regressionResults = value;
            }
        }

        #endregion

        #region Methods

        // TODO: Add methods that will be called by the view
        /// <summary>
        /// Completion for frequency Table
        /// </summary>
        /// <param name="result"></param>
        /// <param name="e"></param>
        private void GetRegressionResultsCompleted(LogRegressionResults result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                this.RegressionResults = result;
                this.Notify(RegressResultsLoadedEvent, new NotificationEventArgs<Exception>());
            }
        }

        public void GetRegressionResults(GadgetParameters gadgetOptions, List<string> columnNames, List<DictionaryDTO> inputDtoList, string customFilter = "") 
        {
            this.ServiceAgent.GetRegressionResults( gadgetOptions, columnNames, customFilter, inputDtoList, GetRegressionResultsCompleted);
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
            this.Notify(ErrorNotice, new NotificationEventArgs<Exception>(message, error));
        }

        public void GenerateTable(string DataSourceName, string TableName, List<string> columnNames, string customFilter)
        {
            this.ServiceAgent.GenerateTable(DataSourceName, TableName, columnNames, customFilter, GenerateTableCompleted);
        }

        private List<ListOfStringClass> regressTable;
        public List<ListOfStringClass> RegressTable
        {
            get
            {
                return this.regressTable;
            }
            set
            {
                this.regressTable = value;
            }
        }
        /// <summary>
        /// Completion for frequency Table
        /// </summary>
        /// <param name="result"></param>
        /// <param name="e"></param>
        private void GenerateTableCompleted(List<ListOfStringClass> result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                if (this.RegressTable == null)
                {
                    Exception exp = new Exception(SharedStrings.NO_RECORDS_SELECTED.ToString());

                    this.Notify(ErrorNotice, new NotificationEventArgs<Exception>("", exp));
                }
                else
                {

                    this.RegressTable = result;
                    this.Notify(RegressTableLoadedEvent, new NotificationEventArgs<Exception>());
                }
            }
        }

        /// <summary>
        /// Call to serviceAgent Method
        /// </summary>
        /// <param name="DataSourceName"></param>
        /// <param name="TableName"></param>
        //public void GetColumns(string DataSourceName, string TableName)
        //{
        //    this.ServiceAgent.GetColumns(DataSourceName, TableName, GetColumnsCompleted);
        //}

        //void ColumnResults_Completed(object sender, EventArgs e)
        //{
        //    InvokeOperation<List<EwavColumn>> result = (InvokeOperation<List<EwavColumn>>)sender;
        //    if (result.HasError)
        //    {

        //    }
        //    else
        //    {
        //        List<EwavColumn> returnedData = ((InvokeOperation<List<EwavColumn>>)sender).Value;
        //        _completed(returnedData, null);
        //    }
        //}

        private List<EwavColumn> columns;
        //private ILogisticRegressionServiceAgent logisticRegressionServiceAgent;

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

        /// <summary>
        /// Completion for columns
        /// </summary>
        /// <param name="result"></param>
        /// <param name="e"></param>

        private void GetColumnsCompleted(List<EwavColumn> result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
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