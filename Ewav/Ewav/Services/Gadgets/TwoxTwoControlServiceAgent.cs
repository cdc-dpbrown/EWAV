/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       TwoxTwoControlServiceAgent.cs
 *  Namespace:  Ewav.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;
using Ewav.Web.EpiDashboard;
using Ewav.BAL;
using Ewav.Web.Services;
using Ewav.ViewModels;

namespace Ewav.Services
{
    public class TwoxTwoControlServiceAgent : ITwoxTwoControlServiceAgent
    {
        FrequencyDomainContext freqCtx;
        TwoByTwoDomainContext twoxTwoCtx;

        private Action<List<EwavColumn>, Exception> _completed;
        private Action<List<FrequencyResultData>, Exception> _frequencyResultsCompleted;
        private Action<TwoxTwoAndMxNResultsSet, Exception> _setupCompleted;
        Exception ex = null;
        public void GetFrequencyResults(string DataSourceName, string TableName,
            Ewav.Web.EpiDashboard.GadgetParameters gadgetParameters,
            IEnumerable<EwavDataFilterCondition> ewavDataFilters,
            Action<List<FrequencyResultData>, Exception> completed)
        {
            _frequencyResultsCompleted = completed;
            FrequencyDomainContext freqCtx = new FrequencyDomainContext();
            InvokeOperation<List<FrequencyResultData>> freqDataResults = freqCtx.GenerateFrequencyTable(gadgetParameters, ewavDataFilters,
                ApplicationViewModel.Instance.EwavDefinedVariables,             
                ApplicationViewModel.Instance.AdvancedDataFilterString);   ////  DataSourceName, TableName);

            freqDataResults.Completed += new EventHandler(freqDataResults_Completed);
        }

        void freqDataResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<List<FrequencyResultData>> result = (InvokeOperation<List<FrequencyResultData>>)sender;
            if (result.HasError)
            {
                ex = result.Error;
                result.MarkErrorAsHandled();
            }
            //else
            //{
                List<FrequencyResultData> returnedData = ((InvokeOperation<List<FrequencyResultData>>)sender).Value;
                _frequencyResultsCompleted(returnedData, null);
            //}
        }

        public string CheckColumnType(string p)
        {
            throw new NotImplementedException();
        }

        public void GetColumns(string DataSourceName, string TableName, Action<List<BAL.EwavColumn>, Exception> completed)
        {
            _completed = completed;
            FrequencyDomainContext freqCtx = new FrequencyDomainContext();
            InvokeOperation<List<EwavColumn>> freqColumnResults = freqCtx.GetColumns(DataSourceName, TableName);
            freqColumnResults.Completed += new EventHandler(freqColumnResults_Completed);
        }

        public void GetCrossTabResults(string DataSourceName, string TableName, Ewav.Web.EpiDashboard.GadgetParameters gadgetParameters, Action<System.Collections.Generic.List<Web.Services.CrossTabResponseObjectDto>, Exception> completed)
        {
        }

        void freqColumnResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<List<EwavColumn>> result = (InvokeOperation<List<EwavColumn>>)sender;
            if (result.HasError)
            {
                ex = result.Error;
                result.MarkErrorAsHandled();
            }
            //else
            //{
                List<EwavColumn> returnedData = ((InvokeOperation<List<EwavColumn>>)sender).Value;
                _completed(returnedData, null);
            //}
        }

        public void SetupGadget(GadgetParameters gadgetParameters, Action<TwoxTwoAndMxNResultsSet, Exception> completed)
        {
            _setupCompleted = completed;
            TwoByTwoDomainContext twoxTwoCtx = new TwoByTwoDomainContext();
            InvokeOperation<TwoxTwoAndMxNResultsSet> resultSet = twoxTwoCtx.SetupGadget(gadgetParameters,
                ViewModels.ApplicationViewModel.Instance.EwavDatafilters,
                ViewModels.ApplicationViewModel.Instance.EwavDefinedVariables);

            resultSet.Completed += new EventHandler(resultSet_Completed);
            //  AAAA   
        }

        void resultSet_Completed(object sender, EventArgs e)
        {
            InvokeOperation<TwoxTwoAndMxNResultsSet> result = (InvokeOperation<TwoxTwoAndMxNResultsSet>)sender;
            if (result.HasError)
            {
                ex = result.Error;
                result.MarkErrorAsHandled();
            }
            //else
            //{
                TwoxTwoAndMxNResultsSet returnedData = ((InvokeOperation<TwoxTwoAndMxNResultsSet>)sender).Value;
                _setupCompleted(returnedData, null);
            //}
        }
    }
}