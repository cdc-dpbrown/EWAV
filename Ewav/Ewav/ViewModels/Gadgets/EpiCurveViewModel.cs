/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       EpiCurveViewModel.cs
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
using System.Collections.Generic;
using Ewav.Web.Services;
using Ewav.BAL;
using Ewav.Web.EpiDashboard;
using Ewav.Client.Application;
using CommonLibrary;

namespace Ewav.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class EpiCurveViewModel : ViewModelBase<EpiCurveViewModel>
    {
        #region Initialization and Cleanup

        // TODO: Add a member for IXxxServiceAgent
        private IEpiCurveServiceAgent serviceAgent;

        public ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;


        /// <summary>
        /// When the data source name is chahged the name is made avail to the application  
        /// </summary>
        public event DatasourceChangedEventHandler DatasourceChangedEvent;

        public IEpiCurveServiceAgent ServiceAgent
        {
            get { return serviceAgent; }
            set { serviceAgent = value; }
        }

        // Default ctor
        public EpiCurveViewModel()
        {
            //applicationViewModel.DatasourceChangedEvent += new DatasourceChangedEventHandler(applicationViewModel_DatasourceChangedEvent);
            serviceAgent = new EpiCurveServiceAgent();
        }

        //void applicationViewModel_DatasourceChangedEvent(object o, DatasourceChangedEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        // TODO: ctor that accepts IXxxServiceAgent
        public EpiCurveViewModel(IEpiCurveServiceAgent serviceAgent)
        {
            this.serviceAgent = serviceAgent;
        }

        #endregion

        #region Notifications

        // TODO: Add events to notify the view or obtain data from the view
        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;
        public event EventHandler<NotificationEventArgs<Exception>> ColumnsLoadedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> EpiCurveTableLoadedEvent;

        #endregion

        #region Properties

        private List<EwavColumn> columns;

        public List<EwavColumn> Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        private DatatableBag dataTableBag;

        public DatatableBag DataTableBag
        {
            get { return dataTableBag; }
            set { dataTableBag = value; }
        }

        // TODO: Add properties using the mvvmprop code snippet

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
            Notify(ErrorNotice, new NotificationEventArgs<Exception>(message, error));
        }

        private void GetEpiCurveResultsCompleted(DatatableBag result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                this.DataTableBag = result;
                if (DataTableBag == null || DataTableBag.ColumnNameList.Count == 0)
                {
                    Exception exp = new Exception(SharedStrings.NO_RECORDS_SELECTED.ToString());

                    this.Notify(ErrorNotice, new NotificationEventArgs<Exception>("", exp));

                    //Notify(ErrorNotice, new NotificationEventArgs<Exception>(SharedStrings.NO_RECORDS_SELECTED));
                }
                else
                {
                    Notify(EpiCurveTableLoadedEvent, new NotificationEventArgs<Exception>());
                }

            }
        }

        /// <summary>
        /// Completion for allColumns
        /// </summary>
        /// <param name="result"></param>
        /// <param name="e"></param>

        private void GetColumnsCompleted(List<EwavColumn> result, Exception e)
        {
            if (e != null)
            {
                NotifyError("There is an error", e);
            }
            else
            {
                this.Columns = new List<EwavColumn>(result);
                Notify(ColumnsLoadedEvent, new NotificationEventArgs<Exception>());
            }
        }

        /// <summary>
        /// Call to serviceAgent Method
        /// </summary>
        /// <param name="DataSourceName"></param>
        /// <param name="TableName"></param>
        public void GetColumns(string DataSourceName, string TableName)
        {

            ServiceAgent.GetColumns(DataSourceName, TableName, GetColumnsCompleted);
        }

        public void GetEpiCurve(GadgetParameters gadgetParameters, bool byEpiWeek, string dateVar, string caseStatusVar)
        {
            ServiceAgent.GetEpiCurveResults(gadgetParameters, byEpiWeek, dateVar, caseStatusVar, GetEpiCurveResultsCompleted);
        }

        #endregion
    }
}