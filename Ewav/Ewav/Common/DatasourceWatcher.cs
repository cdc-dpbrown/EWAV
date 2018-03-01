/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       DatasourceWatcher.cs
 *  Namespace:  Ewav.Common    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using Ewav.ViewModels;

namespace Ewav.Common
{
    public class DatasourceWatcher
    {
        //     ** Singleton  **
        public static readonly DatasourceWatcher Instance = new DatasourceWatcher();

        public readonly AppMenuViewModel appMenuViewModel = new AppMenuViewModel();

        /// <summary>
        /// Occurs when [datasource watcher reset notification event].
        /// </summary>
        public event EventHandler DatasourceWatcherEvent;

        public event EventHandler RefreshEvent;

        /// <summary>
        /// Gets or sets the current application record count.
        /// </summary>
        /// <value>The current application record count.</value>
        public long CurrentApplicationRecordCount { get; set; }

        /// <summary>
        /// Gets or sets the current datasource record count.
        /// </summary>
        /// <value>The current datasource record count.</value>
        public long CurrentDatasourceRecordCount { get; set; }

        /// <summary>
        /// Gets or sets the current record count.
        /// </summary>
        /// <value>The current record count.</value>
        public long CurrentRecordCount { get; set; }

        /// <summary>
        /// Gets or sets the new datasource record count.
        /// </summary>
        /// <value>The new datasource record count.</value>
        public long NewDatasourceRecordCount { get; set; }

        /// <summary>
        /// Gets or sets the record count difference.
        /// </summary>
        /// <value>The record count difference.</value>
        public long RecordCountDifference { get; set; }

        /// <summary>
        /// Gets or sets the reload if changed.
        /// </summary>
        /// <value>The reload if changed.</value>
        public bool ReloadIfChanged { get; set; }

        /// <summary>
        /// WARNING – This constructor cannot be public.  
        /// Making the constructor of this singleton class public will rip a hole in space-time, 
        /// surely destroying us all. -DS                               
        /// </summary>
        /// 
        private DatasourceWatcher()
        {
            RefreshTimer.Instance.RefreshTimerFiredEvent += new RefreshTimer.RefreshTimerFiredEventHandler(RefreshTimer_RefreshTimerFiredEvent);
            ApplicationViewModel.Instance.DatasourceChangedEvent += new Client.Application.DatasourceChangedEventHandler(ApplicationViewModel_DatasourceChangedEvent);

            this.ReloadIfChanged = false;
        }

        public void ManualReload()
        {


            this.CurrentApplicationRecordCount = this.CurrentDatasourceRecordCount;

            this.RecordCountDifference = 0;

            // amd notify    
            this.DatasourceWatcherEvent(this, new EventArgs());

            //     reload 
            ApplicationViewModel.Instance.ReloadGadgets();

            RefreshEvent(this, new EventArgs());





        }

        void ApplicationViewModel_DatasourceChangedEvent(object o, Client.Application.DatasourceChangedEventArgs e)
        {
            //  reset stuff    
            this.CurrentDatasourceRecordCount = ApplicationViewModel.Instance.EwavDatasources[e.DataSource].TotalRecords;
            this.CurrentApplicationRecordCount = this.CurrentDatasourceRecordCount;
            this.RecordCountDifference = 0;

            this.DatasourceWatcherEvent(this, new EventArgs());
        }

        void appMenuViewModel_RecordcountRecievedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            // get the new count    
            this.CurrentDatasourceRecordCount = Convert.ToInt64(this.appMenuViewModel.RecordCountString.Substring(0, this.appMenuViewModel.RecordCountString.IndexOf(",")));

            if (this.CurrentDatasourceRecordCount != this.CurrentApplicationRecordCount)
            {
                // if the DatasourceRecordCount has changed calculate differnece          
                this.RecordCountDifference = Math.Abs(this.CurrentDatasourceRecordCount - this.CurrentApplicationRecordCount);

                //  if auto-reload   
                if (this.ReloadIfChanged)
                {


                    this.CurrentApplicationRecordCount = this.CurrentDatasourceRecordCount;

                    this.RecordCountDifference = 0;

                    //     reload 
                    ApplicationViewModel.Instance.ReloadGadgets();

                    RefreshEvent(this, new EventArgs());

                }

                // amd notify    
                this.DatasourceWatcherEvent(this, new EventArgs());
            }
        }

        void RefreshTimer_RefreshTimerFiredEvent(object o)
        {
            ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;

            this.appMenuViewModel.RecordcountRecievedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(appMenuViewModel_RecordcountRecievedEvent);

            if (applicationViewModel.UseAdvancedFilter)
            {
                this.appMenuViewModel.GetRecordCount(applicationViewModel.EwavDefinedVariables,
                    applicationViewModel.AdvancedDataFilterString,
                    applicationViewModel.EwavSelectedDatasource.TableName,
                    applicationViewModel.EwavSelectedDatasource.DatasourceName);
            }
            else
            {
                this.appMenuViewModel.GetRecordCount(applicationViewModel.EwavDatafilters,
                    applicationViewModel.EwavDefinedVariables,
                    applicationViewModel.EwavSelectedDatasource.TableName,
                    applicationViewModel.EwavSelectedDatasource.DatasourceName);
            }
        }
    }
}