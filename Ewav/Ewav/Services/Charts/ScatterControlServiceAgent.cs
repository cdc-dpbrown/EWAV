/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ScatterControlServiceAgent.cs
 *  Namespace:  Ewav.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Ewav.Web.Services;
using Ewav.BAL;
using System.ServiceModel.DomainServices.Client;
using Ewav.Web.EpiDashboard;
using Ewav.ViewModels;

namespace Ewav.Services
{
    public class ScatterControlServiceAgent : IScatterControlServiceAgent
    {
        private Action<List<EwavColumn>, Exception> _completed;
        private Action<LinRegressionResults, Exception> _rresultsCompleted;
        private Action<ScatterDataDTO, Exception> _tableCompleted;
        Exception ex = null;
        #region completion

        void sctrCtxColumnResults_Completed(object sender, EventArgs e)
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

        void sctrCtxRegResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<LinRegressionResults> result = (InvokeOperation<LinRegressionResults>)sender;
            if (result.HasError)
            {
                ex = result.Error;
                result.MarkErrorAsHandled();
            }
            //else
            //{
                LinRegressionResults returnedData = ((InvokeOperation<LinRegressionResults>)sender).Value;
                _rresultsCompleted(returnedData, null);
            //}
        }

        #endregion

        #region Methods


        /// <summary>
        /// Gets columns from Domain Service
        /// </summary>
        /// <param name="DataSourceName"></param>
        /// <param name="TableName"></param>
        /// <param name="completed"></param>
        public void GetColumns(string DataSourceName, string TableName, Action<List<EwavColumn>, Exception> completed)
        {
            _completed = completed;

            ScatterDomainContext sctrCtx = new ScatterDomainContext();
            //LinearRegressionDomainContext linrCtx = new LinearRegressionDomainContext();

            InvokeOperation<List<EwavColumn>> sctrCtxColumnResults = sctrCtx.GetColumns(DataSourceName, TableName);
            sctrCtxColumnResults.Completed += new EventHandler(sctrCtxColumnResults_Completed);
        }

        /// <summary>
        /// Gets the regression table results from Domain service
        /// </summary>
        /// <param name="DatasourceName"></param>
        /// <param name="TableName"></param>
        /// <param name="gadgetOptions"></param>
        /// <param name="columnNames"></param>
        /// <param name="inputDtoList"></param>
        /// <param name="completed"></param>
        //public void GetRegressionResults(string DatasourceName, string TableName, GadgetParameters gadgetOptions, List<string> columnNames, List<DictionaryDTO> inputDtoList, Action<LinRegressionResults, Exception> completed)
        //{
        //    _rresultsCompleted = completed;
        //    ScatterDomainContext sctrCtx = new ScatterDomainContext();
        //    InvokeOperation<LinRegressionResults> sctrCtxRegResults = sctrCtx.GetRegressionResult(DatasourceName, TableName, gadgetOptions, columnNames, inputDtoList);
        //    sctrCtxRegResults.Completed += new EventHandler(sctrCtxRegResults_Completed);
        //}

        
        #endregion


        public void GenerateTable(GadgetParameters gadgetOptions, IEnumerable<EwavDataFilterCondition> ewavDataFilters,
            string filterString, Action<ScatterDataDTO, Exception> completed)
        {
            _tableCompleted = completed;
            ScatterDomainContext sctrCtx = new ScatterDomainContext();
            InvokeOperation<ScatterDataDTO> sctrCtxtableResults = sctrCtx.GenerateTable(gadgetOptions,ewavDataFilters, ApplicationViewModel.Instance.EwavDefinedVariables, ApplicationViewModel.Instance.AdvancedDataFilterString);
            sctrCtxtableResults.Completed += new EventHandler(sctrCtxtableResults_Completed);
        }

        void sctrCtxtableResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<ScatterDataDTO> result = (InvokeOperation<ScatterDataDTO>)sender;
            if (result.HasError)
            {
                ex = result.Error;
                result.MarkErrorAsHandled();
            }
            //else
            //{
                ScatterDataDTO returnedData = ((InvokeOperation<ScatterDataDTO>)sender).Value;
                _tableCompleted(returnedData, null);
            //}
        }

    }
}