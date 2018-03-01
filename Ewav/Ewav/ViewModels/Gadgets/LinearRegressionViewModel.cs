/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       LinearRegressionViewModel.cs
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


// Toolkit extension methods
namespace Ewav.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class LinearRegressionViewModel : ViewModelBase<LinearRegressionViewModel>
    {
        #region Initialization and Cleanup
        // TODO: Add a member for IXxxServiceAgent
        private ILinearRegressionServiceAgent serviceAgent;

        public ILinearRegressionServiceAgent ServiceAgent
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

        private LinRegressionResults regressionResults;

        public LinRegressionResults RegressionResults
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
        public LinearRegressionViewModel()
        {
        }

        // TODO: ctor that accepts IXxxServiceAgent
        public LinearRegressionViewModel(ILinearRegressionServiceAgent serviceAgent)
        {
            this.serviceAgent = serviceAgent;
        }

        #endregion

        #region Notifications

        // TODO: Add events to notify the view or obtain data from the view
        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;
        public event EventHandler<NotificationEventArgs<Exception>> ColumnsLoadedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> RegressResultsLoadedEvent;

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
        public void GetRegressionResults(  GadgetParameters gadgetOptions, List<string> columnNames, List<DictionaryDTO> inputDtoList)
        {
            this.ServiceAgent.GetRegressionResults(  gadgetOptions, columnNames, inputDtoList, GetRegressionResultsCompleted);
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
        private void GetRegressionResultsCompleted(LinRegressionResults result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                this.RegressionResults = result;
                if (this.RegressionResults == null)
                {
                    Exception exp = new Exception(SharedStrings.NO_RECORDS_SELECTED.ToString());

                    this.Notify(ErrorNotice, new NotificationEventArgs<Exception>("", exp));
                }
                else { 
                this.Notify(RegressResultsLoadedEvent, new NotificationEventArgs<Exception>());
                }
            }
        }

        /// <summary>
        /// Completion for columns
        /// </summary>
        /// <param name="result"></param>
        /// <param name="e"></param>

        //private void GetColumnsCompleted(List<EwavColumn> result, Exception e)
        //{
        //    if (e != null)
        //    {
        //        this.NotifyError("There is an error", e);
        //    }
        //    else
        //    {
        //        this.Columns = new List<EwavColumn>(result);
        //        this.Notify(ColumnsLoadedEvent, new NotificationEventArgs<Exception>());
        //    }
        //}
        #endregion
    }
}