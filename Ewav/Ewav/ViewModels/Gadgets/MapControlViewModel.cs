/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MapControlViewModel.cs
 *  Namespace:  Ewav.ViewModels.Gadgets    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Windows;
using System.Threading;
using System.Collections.ObjectModel;

// Toolkit namespace
using Ewav.DTO;
using SimpleMvvmToolkit;
// Toolkit extension methods
using SimpleMvvmToolkit.ModelExtensions;

using Ewav.Services;
using Ewav.Web.EpiDashboard;
using CommonLibrary;

namespace Ewav.ViewModels.Gadgets
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class MapControlViewModel : ViewModelBase<MapControlViewModel>
    {
        #region Initialization and Cleanup

        // TODO: Add a member for IXxxServiceAgent
        private IMapControlControlServiceAgent serviceAgent;

        private List<PointDTOCollection> resultCollection;

        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;
        public event EventHandler<NotificationEventArgs<Exception>> MapDataLoadedEvent;



        // Default ctor
        public MapControlViewModel()
        {
        }

        // TODO: ctor that accepts IXxxServiceAgent
        public MapControlViewModel(IMapControlControlServiceAgent serviceAgent)
        {
            this.serviceAgent = serviceAgent;
        }

        #endregion

        #region Notifications

        /// <summary>
        /// Gets or sets the result collection.
        /// </summary>
        /// <value>The result collection.</value>
        public List<PointDTOCollection> ResultCollection
        {
            get
            {
                return this.resultCollection;
            }
            set
            {
                this.resultCollection = value;
            }
        }

        /// <summary>
        /// Loads the map data.
        /// </summary>
        public void LoadMapData(GadgetParameters gp)
        {
            try
            {
          //       MapControlServiceAgent mcsa = new MapControlServiceAgent();
                serviceAgent.LoadMapData(gp, LoadMapDataCompleted);
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Loads the map data completed.
        /// </summary>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        private void LoadMapDataCompleted(List<PointDTOCollection> result, Exception exception)
        {


            if (exception != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", exception);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                this.ResultCollection = result;
                if (this.ResultCollection.Count == 0)
                {
                    //  Notify(ErrorNotice, new NotificationEventArgs<Exception>(SharedStrings.NO_RECORDS_SELECTED));
                    Notify(MapDataLoadedEvent, new NotificationEventArgs<Exception>());
                }
                else
                {
                    Notify(MapDataLoadedEvent, new NotificationEventArgs<Exception>());
                //     MapDataLoadedEvent(result, new NotificationEventArgs<Exception>());
                }
            }
        }

   
   
        #endregion

        #region Properties

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

        #endregion
    }
}