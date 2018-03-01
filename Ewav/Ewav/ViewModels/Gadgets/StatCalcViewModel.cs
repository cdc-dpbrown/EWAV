/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       StatCalcViewModel.cs
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
using Ewav.Web.Services;
using System.Collections.Generic;

namespace Ewav.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class StatCalcViewModel : ViewModelBase<StatCalcViewModel>
    {
        #region Initialization and Cleanup

        // TODO: Add a member for IXxxServiceAgent
        private IStatCalcServiceAgent serviceAgent;

        // Default ctor
        public StatCalcViewModel()
        {
            serviceAgent = new StatCalcServiceAgent();
        }

        // TODO: ctor that accepts IXxxServiceAgent
        public StatCalcViewModel(IStatCalcServiceAgent serviceAgent)
        {
            this.serviceAgent = serviceAgent;
        }

        #endregion

        #region Notifications

        // TODO: Add events to notify the view or obtain data from the view
        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;
        public event EventHandler<NotificationEventArgs<Exception>> SingleTableLoadedEvent;

        #endregion

        #region Properties

        // TODO: Add properties using the mvvmprop code snippet

        private StatCalcDTO statCaldto;

        public StatCalcDTO StatCalcdto
        {
            get { return statCaldto; }
            set { statCaldto = value; }
        }

        #endregion

        #region Methods

        // TODO: Add methods that will be called by the view

        #endregion

        #region Completion Callbacks

        // TODO: Optionally add callback methods for async calls to the service agent

        /// <summary>
        /// Completion for frequency Table
        /// </summary>
        /// <param name="result"></param>
        /// <param name="e"></param>
        private void GetStatCalcTableCompleted(StatCalcDTO result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                this.statCaldto = result;
                Notify(SingleTableLoadedEvent, new NotificationEventArgs<Exception>());
            }
        }

        #endregion

        #region Helpers

        // Helper method to notify View of an error
        private void NotifyError(string message, Exception error)
        {
            // Notify view of an error
            Notify(ErrorNotice, new NotificationEventArgs<Exception>(message, error));
        }

        /// <summary>
        /// Method which is called by view and which calls service agent, passing all the needed parameters.
        /// </summary>
        /// <param name="ytVal"></param>
        /// <param name="ntVal"></param>
        /// <param name="tyVal"></param>
        /// <param name="tnVal"></param>
        /// <param name="yyVal"></param>
        /// <param name="ynVal"></param>
        /// <param name="nyVal"></param>
        /// <param name="nnVal"></param>
        /// <param name="strataActive"></param>
        /// <param name="strataVals"></param>
        public void GetStatCalc2x2(int ytVal, int ntVal, int tyVal, int tnVal, int yyVal, int ynVal, int nyVal, int nnVal, List<DictionaryDTO> strataActive,
            List<DictionaryDTO> strataVals)
        {
            serviceAgent.GetStatCalc(ytVal, ntVal, tyVal, tnVal, yyVal, ynVal, nyVal, nnVal, strataActive, strataVals, GetStatCalcTableCompleted);
        }


        #endregion
    }
}