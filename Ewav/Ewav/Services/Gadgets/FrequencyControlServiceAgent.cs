/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       FrequencyControlServiceAgent.cs
 *  Namespace:  Ewav.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using Ewav.Web.Services;
using System.ServiceModel.DomainServices.Client;
using Ewav.Web.EpiDashboard;
using Ewav.BAL;
using Ewav.ViewModels;


namespace Ewav.Services
{
    /// <summary>
    /// Service agent for Ewav external data sources.  
    /// </summary>
    public class FrequencyControlServiceAgent : IFrequencyControlServiceAgent
    {
        #region Variables
        FrequencyDomainContext freqCtx;
        private Action<List<EwavColumn>, Exception> _completed;
        private Action<List<FrequencyResultData>, Exception> _freqCompleted;

        #endregion

        #region Constructor
        public FrequencyControlServiceAgent()
        {
        }
        #endregion

        #region Helper Method

        public string CheckColumnType(string p)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// GetColumns Method that calls the method in Domain Service
        /// </summary>
        /// <param name="DataSourceName"></param>
        /// <param name="TableName"></param>
        /// <param name="completed"></param>
        //public void GetColumns(string DataSourceName, string TableName, Action<List<EwavColumn>, Exception> completed)
        //{
        //    _completed = completed;
        //    FrequencyDomainContext freqCtx = new FrequencyDomainContext();
        //    InvokeOperation<List<EwavColumn>> freqColumnResults = freqCtx.GetColumns(DataSourceName, TableName);
        //    freqColumnResults.Completed += new EventHandler(freqColumnResults_Completed);
        //}

        /// <summary>
        /// GetFrequencyResults method that calls Domain Service Method.
        /// </summary>
        /// <param name="DataSourceName"></param>
        /// <param name="TableName"></param>
        /// <param name="gadgetParameters"></param>
        /// <param name="completed"></param>
        //void IFrequencyControlServiceAgent.GetFrequencyResults(GadgetParameters gadgetParameters,
        //          IEnumerable<EwavDataFilterCondition> ewavDataFilters, string filterString,
        //    Action<List<FrequencyResultData>, Exception> completed)
        //{
           
        //}
        #endregion

        #region Completion Callbacks
        void freqColumnResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<List<EwavColumn>> result = (InvokeOperation<List<EwavColumn>>)sender;
            if (result.HasError)
            {

            }
            else
            {
                List<EwavColumn> returnedData = ((InvokeOperation<List<EwavColumn>>)sender).Value;
                _completed(returnedData, null);
            }
        }

        void freqTableResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<List<FrequencyResultData>> result =
                (InvokeOperation<List<FrequencyResultData>>)sender;
            Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                //throw new GadgetException(result.Error.Message);
                ex = result.Error;
            }
            //else
            //{
                List<FrequencyResultData> returnedData = ((InvokeOperation<List<FrequencyResultData>>)sender).Value;
                _freqCompleted(returnedData, ex);
            //}
        }
        #endregion

        public void GetFrequencyResults(GadgetParameters gadgetParameters, IEnumerable<EwavDataFilterCondition> ewavDataFilters,
            string filterString, Action<List<FrequencyResultData>, Exception> completed)
        {
            _freqCompleted = completed;
            freqCtx = new FrequencyDomainContext();
            InvokeOperation<List<FrequencyResultData>> freqResults = 
                freqCtx.GenerateFrequencyTable(gadgetParameters, ewavDataFilters, 
                ApplicationViewModel.Instance.EwavDefinedVariables,   filterString);
            // Not used a
            //    Dictionary<List<DTO.EwavFrequencyControlDto>, List<DescriptiveStatistics>> freqTableResults = new Dictionary<List<DTO.EwavFrequencyControlDto>, List<DescriptiveStatistics>>();
            freqResults.Completed += new EventHandler(freqTableResults_Completed);

        }
    }
}