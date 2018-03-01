/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       LinearRegressionServiceAgent.cs
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
    public class LinearRegressionServiceAgent : ILinearRegressionServiceAgent
    {
            private Action<List<EwavColumn>, Exception> _completed;
            private Action<LinRegressionResults, Exception> _rresultsCompleted;

            #region completion

            void linrCtxRegResults_Completed(object sender, EventArgs e)
            {
                InvokeOperation<LinRegressionResults> result = (InvokeOperation<LinRegressionResults>)sender;
                Exception ex = null;
                if (result.HasError)
                {
                    ex = result.Error;
                    result.MarkErrorAsHandled();
                }
                //else
                //{
                    LinRegressionResults returnedData = ((InvokeOperation<LinRegressionResults>)sender).Value;
                    _rresultsCompleted(returnedData, ex);
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
            //public void GetColumns(string DataSourceName, string TableName, Action<List<EwavColumn>, Exception> completed)
            //{
            //    _completed = completed;
            //    LinearRegressionDomainContext linrCtx = new LinearRegressionDomainContext();
            //    InvokeOperation<List<EwavColumn>> linrCtxColumnResults = linrCtx.GetColumns(DataSourceName, TableName);
            //    linrCtxColumnResults.Completed += new EventHandler(linrCtxColumnResults_Completed);
            //}

            /// <summary>
            /// Gets the regression table results from Domain service
            /// </summary>
            /// <param name="DatasourceName"></param>
            /// <param name="TableName"></param>
            /// <param name="gadgetOptions"></param>
            /// <param name="columnNames"></param>
            /// <param name="inputDtoList"></param>
            /// <param name="completed"></param>
            public void GetRegressionResults(  GadgetParameters gadgetOptions, List<string> columnNames, List<DictionaryDTO> inputDtoList, Action<LinRegressionResults, Exception> completed)
            {
                _rresultsCompleted = completed;
                LinearRegressionDomainContext linrCtx = new LinearRegressionDomainContext();
                InvokeOperation<LinRegressionResults> linrCtxRegResults = linrCtx.GetRegressionResult(gadgetOptions, columnNames, inputDtoList, ApplicationViewModel.Instance.EwavDatafilters, ApplicationViewModel.Instance.EwavDefinedVariables, ApplicationViewModel.Instance.AdvancedDataFilterString);
                linrCtxRegResults.Completed += new EventHandler(linrCtxRegResults_Completed);
            }
            #endregion

    }
}