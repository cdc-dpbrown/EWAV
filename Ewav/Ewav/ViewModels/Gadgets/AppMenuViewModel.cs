/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       AppMenuViewModel.cs
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
using Ewav.BAL;
using Ewav.DTO;
using Ewav.ExtensionMethods;
using Ewav.Services;
using SimpleMvvmToolkit;
using Ewav.Web.Services;

// Toolkit namespace

// Toolkit extension methods
namespace Ewav.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.      
    /// </summary>
    public class AppMenuViewModel : ViewModelBase<AppMenuViewModel>
    {
        #region Initialization and Cleanup
        // TODO: Add a member for IXxxServiceAgent
        private IDatasourceServiceAgent serviceAgent;

        private string selectedDatasourceName;

        /// <summary>
        /// Gets or sets the datasources2.
        /// </summary>
        /// <value>The datasources2.</value>
        public List<EwavDatasourceDto> Datasources2
        {
            get
            {
                return this.datasources2;
            }
            set
            {
                this.datasources2 = value;
            }
        }

        public string SelectedDatasourceName
        {
            get
            {
                return this.selectedDatasourceName;
            }
            set
            {
                this.selectedDatasourceName = value;
            }
        }

        /// <summary>
        /// Not sure if this should be exposed to the vw but will do anyway for now   
        /// </summary>
        public IDatasourceServiceAgent ServiceAgent
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
        public AppMenuViewModel()
        {
            this.serviceAgent = new DatasourceServiceAgent();
        }

        // TODO: ctor that accepts IXxxServiceAgent
        public AppMenuViewModel(IDatasourceServiceAgent serviceAgent)
        {
            this.serviceAgent = serviceAgent;
        }

        #endregion

        #region Notifications

        // TODO: Add events to notify the view or obtain data from the view
        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;
        public event EventHandler<NotificationEventArgs<Exception>> DatasourcesLoadedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> RecordcountRecievedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> FilterStringRecieved;

        #endregion

        #region Properties

        private List<EwavDatasourceDto> datasources2;
   
        private string recordCountString;

        public string RecordCountString
        {
            get { return recordCountString; }
            set { recordCountString = value; }
        }

        private string filterString;

        public string FilterString
        {
          get { return filterString; }
          set { filterString = value; }
        }

        public string TestDatasources
        {
            get
            {
                return "A test";
            }
        }


        #endregion

        #region Methods

        public void GetRecordCount(List<EwavDataFilterCondition> filterList, List<EwavRule_Base> rules, string tableName, string dsName) 
        {
            this.ServiceAgent.GetRecordCount(filterList,rules, tableName, dsName, GetRecordsCountCompleted);
        }

        public void GetRecordCount(List<EwavRule_Base> rules, string s, string tableName, string dsName)
        {
          this.ServiceAgent.GetRecordCount(rules,s, tableName, dsName, GetRecordsCountCompleted);
        }
      /// <summary>
      /// Gets Filter String.
      /// </summary>
      /// <param name="filterList"></param>
      /// <param name="dsName"></param>
        public void GetFilterString(List<EwavDataFilterCondition> filterList, List<EwavRule_Base> rules, string tableName, string dsName) 
        {
          this.ServiceAgent.ReadFilterString(filterList,rules,tableName, dsName,GetFilterStringCompleted);
        }


        public void GetDatasourcesAsIEnumerable2()
        {
            //if (ApplicationViewModel.Instance.DatasourceList.Length == 0)
            //{
            this.ServiceAgent.GetDatasources2(GetDatasourcesCompleted2);
            //}
            //else
            //{
            //    PopulateDatasouceListFromApplicationViewModel();
            //}
        }


     
        //public void GetDatasoourceRecordCount(string DaasourceName)    //   7777 a
        //{
        //    this.ServiceAgent.DatasetRecordCount(DaasourceName, GetRecordCountCompleted);
        //}

        #endregion

        #region Completion Callbacks
        private void GetRecordsCountCompleted(string result, Exception error)
        {
            // Notify view of an error
            if (error != null)
            {
                this.Notify(this.ErrorNotice, new NotificationEventArgs<Exception>("There was an error retrieving record count.", error));
            }
            else
            {
                RecordCountString = result;
                this.Notify(this.RecordcountRecievedEvent, new NotificationEventArgs<Exception>());
            }
        }

        private void GetFilterStringCompleted(string result, Exception error)
        {
          // Notify view of an error
          if (error != null)
          {
            this.NotifyError("There was an error retrieving record count.", error);
          }
          else
          {
            FilterString = result;
            this.Notify(this.FilterStringRecieved, new NotificationEventArgs<Exception>());
          }
        }

        private void GetDatasourcesCompleted2(IEnumerable<EwavDatasourceDto> result, Exception error)
        {
            // Notify view of an error
            try
            {
                if (error != null)
                {
                    this.Notify(this.ErrorNotice, new NotificationEventArgs<Exception>("There was an error retrieving datasources"
                        , error));
                }
                else
                {
                    this.Datasources2 = new List<EwavDatasourceDto>(result);

                   

                    foreach (EwavDatasourceDto item in Datasources2)
                    {
                        //    item.DatasourceName = ExtensionMethods.Extensions.FromCamelCase(item.DatasourceName);
                        //    item.DatasourceName = item.DatasourceName.FromCamelCase();
                        item.DatasourceNoCamelName = item.DatasourceName.FromCamelCase();       
                    }
                    //   Datasources2.Insert(0, new EwavDatasourceDto() { DatasourceName = "Select..." });

                    //  applicationViewModel.   = this.Datasources;


                    DatasourcesLoadedEvent(this, new NotificationEventArgs<Exception>());    

                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message + " ==== " + ex.StackTrace);    
            }
            finally
            {

            }
        }

        #endregion

        #region Helpers

        // Helper method to notify View of an error
        private void NotifyError(string message, Exception error)
        {
            // Notify view of an error
            this.Notify(this.ErrorNotice, new NotificationEventArgs<Exception>(message, error));
        }

        #endregion

        private long selectedDatasetRecordCount;

        public long SelectedDatasetRecordCount
        {
            get
            {
                return this.selectedDatasetRecordCount;
            }
            set
            {
                this.selectedDatasetRecordCount = value;
            }
        }
    }
}