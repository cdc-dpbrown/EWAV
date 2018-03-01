/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ScatterViewModel.cs
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

namespace Ewav.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class ScatterViewModel : ViewModelBase<ScatterViewModel>
    {
        #region Initialization and Cleanup
        // TODO: Add a member for IXxxServiceAgent
        private IScatterControlServiceAgent serviceAgent;

        public IScatterControlServiceAgent ServiceAgent
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

        private ScatterDataDTO regressionResults;

        public ScatterDataDTO RegressionResults
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

        // Default ctor
        public ScatterViewModel()
        {
        }

        // TODO: ctor that accepts IXxxServiceAgent
        public ScatterViewModel(IScatterControlServiceAgent serviceAgent)
        {
            this.serviceAgent = serviceAgent;
        }

        #endregion

        #region Notifications

        // TODO: Add events to notify the view or obtain data from the view
        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;
        public event EventHandler<NotificationEventArgs<Exception>> ColumnsLoadedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> RegressTableLoadedEvent;

        #endregion

        #region Properties

        // TODO: Add properties using the mvvmprop code snippet

        #endregion

        #region Methods

        /// <summary>
        /// Call to service agent.
        /// </summary>
        /// <param name="DataSourceName"></param>
        /// <param name="TableName"></param>
        /// <param name="gadgetOptions"></param>
        /// <param name="columnNames"></param>
        /// <param name="inputDtoList"></param>
        //public void GetRegressionResults(string DataSourceName, string TableName, GadgetParameters gadgetOptions, List<string> columnNames, List<DictionaryDTO> inputDtoList)
        //{
        //    this.ServiceAgent.GetRegressionResults(DataSourceName, TableName, gadgetOptions, columnNames, inputDtoList, GetRegressionResultsCompleted);
        //}

        public void GenerateTable(GadgetParameters gadgetOptions)
        {
            this.ServiceAgent.GenerateTable(gadgetOptions, ApplicationViewModel.Instance.EwavDatafilters, ApplicationViewModel.Instance.AdvancedDataFilterString, GetTableCompleted);
        }

        /// <summary>
        /// Call to serviceAgent Method
        /// </summary>
        /// <param name="DataSourceName"></param>
        /// <param name="TableName"></param>
        public void GetColumns(string DataSourceName, string TableName)
        {
            this.ServiceAgent.GetColumns(DataSourceName, TableName, GetColumnsCompleted);
        }

        #endregion

        // Helper method to notify View of an error
        private void NotifyError(string message, Exception error)
        {
            // Notify view of an error
            this.Notify(ErrorNotice, new NotificationEventArgs<Exception>(message, error));
        }

        #region Completion
        /// <summary>
        /// Completion for regresion results Table
        /// </summary>
        /// <param name="result"></param>
        /// <param name="e"></param>
        private void GetTableCompleted(ScatterDataDTO result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                this.RegressionResults = result;
                this.Notify(RegressTableLoadedEvent, new NotificationEventArgs<Exception>());
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