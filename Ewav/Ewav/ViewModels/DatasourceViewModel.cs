/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       DatasourceViewModel.cs
 *  Namespace:  Ewav.ViewModels    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using Ewav.BAL;
using Ewav.Services;
using SimpleMvvmToolkit;
using System.Collections.Generic;


// Toolkit extension methods
namespace Ewav.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class DatasourceViewModel : ViewModelBase<DatasourceViewModel>
    {
        #region Initialization and Cleanup
        // TODO: Add a member for IXxxServiceAgent
        private IDatasourceServiceAgent serviceAgent;

        // Default ctor
        public DatasourceViewModel()
        {
        }

        // TODO: ctor that accepts IXxxServiceAgent
        public DatasourceViewModel(IDatasourceServiceAgent serviceAgent)
        {
            this.serviceAgent = serviceAgent;
        }

        #endregion

        #region Notifications

        /// <summary>
        /// Gets the total record count.
        /// </summary>
        /// <param name="datasourceName">Name of the datasource.</param>
        public void GetTotalRecordCount(string dataSourceName)
        {

            DatasourceServiceAgent dsa = new DatasourceServiceAgent();

            dsa.DatasetRecordCount(dataSourceName, DatasetRecordCountCompleted);     

        }

        // TODO: Add events to notify the view or obtain data from the view
        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;
        public event EventHandler MyDatasourcesReadLoaded;
        public event EventHandler ColumnsForDatasourceLoaded;
        public event EventHandler RecordsCounted;


        public List<DTO.DatasourceDto> ListOfDSNamesForMyOrg { get; set; }

        #endregion

        #region Properties

        // TODO: Add properties using the mvvmprop code snippet
        private int filteredRecordCount;
        public int FilteredRecordCount
        {
            get
            {
                return this.filteredRecordCount;
            }
            set
            {
                this.filteredRecordCount = value;
                this.NotifyPropertyChanged(m => m.FilteredRecordCount);
            }
        }

        private int totalRecordCount;
        public int TotalRecordCount
        {
            get
            {
                return this.totalRecordCount;
            }
            set
            {
                this.totalRecordCount = value;
                this.NotifyPropertyChanged(m => m.TotalRecordCount);
            }
        }
        #endregion

        #region Methods

        // TODO: Add methods that will be called by the view

        //public void ReadAllDatasourcesInMyOrg(int orgId) 
        //{
        //    DatasourceServiceAgent dsa = new DatasourceServiceAgent();
        //    dsa.ReadAllDatasourcesInMyOrg(orgId, ReadAllDatasourcesInMyOrgCompleted);
        //      dataSourceName
        //}

        public void GetColumnsForDatasource(string dataSourceName)
        {
            DatasourceServiceAgent dsa = new DatasourceServiceAgent();

            dsa.GetColumnsForDatasource(dataSourceName, GetColumnsForDatasourceCompleted);
        }

        #endregion

        #region Completion Callbacks


        private void DatasetRecordCountCompleted(long count, Exception ex)
        {
            NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", ex);
            if (ex == null)
            {
                RecordsCounted(count, null);       
            }
            else
            {
                this.Notify(ErrorNotice, notification);
            }


        }



        private void GetColumnsForDatasourceCompleted(List<EwavColumn> allColumns, Exception ex)
        {
            NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", ex);
            if (ex == null)
            {
                //AddCompletedEvent(result, new NotificationEventArgs<Exception>());
                //this.Notify(AddCompletedEvent,notification);
                ColumnsForDatasourceLoaded(allColumns, null);
            }
            else
            {
                this.Notify(ErrorNotice, notification);
            }


        }


        private void ReadAllDatasourcesInMyOrgCompleted(List<DTO.DatasourceDto> listOfNames, Exception ex)
        {
            ListOfDSNamesForMyOrg = listOfNames;

            MyDatasourcesReadLoaded(this, new EventArgs());
        }

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
    }
}