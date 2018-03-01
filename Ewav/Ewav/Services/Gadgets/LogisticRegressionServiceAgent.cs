/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       LogisticRegressionServiceAgent.cs
 *  Namespace:  Ewav.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
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
    public class LogisticRegressionServiceAgent : ILogisticRegressionServiceAgent
    {
        LogisticRegressionDomainContext logrCtx;
        private Action<List<EwavColumn>, Exception> _completed;
        private Action<List<ListOfStringClass>, Exception> _logrCompleted;
        private Action<LogRegressionResults,Exception> _rresultsCompleted;

        //public void GetColumns(string DataSourceName, string TableName, Action<List<EwavColumn>, Exception> completed)
        //{
        //    _completed = completed;
        //    LogisticRegressionDomainContext logrCtx = new LogisticRegressionDomainContext();
        //    InvokeOperation<List<EwavColumn>> logrCtxColumnResults = logrCtx.GetColumns(DataSourceName, TableName);
        //    logrCtxColumnResults.Completed += new EventHandler(logrCtxColumnResults_Completed);
        //}

        void logrCtxColumnResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<List<EwavColumn>> result = (InvokeOperation<List<EwavColumn>>)sender;
            Exception ex = null;
            if (result.HasError)
            {
                ex = result.Error;
                result.MarkErrorAsHandled();
            }
            //else
            //{
                List<EwavColumn> returnedData = ((InvokeOperation<List<EwavColumn>>)sender).Value;
                _completed(returnedData, ex);
            //}
        }

        

        void logrResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<List<ListOfStringClass>> result =
                (InvokeOperation<List<ListOfStringClass>>)sender;

            if (result.HasError)
            {

            }
            else
            {
                List<ListOfStringClass> returnedData = ((InvokeOperation<List<ListOfStringClass>>)sender).Value;
                _logrCompleted(returnedData, null);
            }
        }

        

        void logrCtxRegResults_Completed(object sender, EventArgs e) 
        {
            InvokeOperation<LogRegressionResults> result = (InvokeOperation<LogRegressionResults>)sender;
            if (result.HasError)
            {

            }
            else
            {
                LogRegressionResults returnedData = ((InvokeOperation<LogRegressionResults>)sender).Value;
                _rresultsCompleted(returnedData, null);
            }
        }




       // void GetRegressionResults(string DatasourceName, string TableName, GadgetParameters gadgetOptions, List<MyString> columnNames, string customFilter,   List<DictionaryDTO> inputDtoList, Action<RegressionResults, Exception> completed)
       //{
          
       //}
        //public void GetColumns(string DataSourceName, string TableName, Action<List<EwavColumn>, Exception> completed)
        //{
        //    _completed = completed;
        //    LogisticRegressionDomainContext logrCtx = new LogisticRegressionDomainContext();
        //    InvokeOperation<List<EwavColumn>> logrCtxColumnResults = logrCtx.GetColumns(DataSourceName, TableName);
        //    logrCtxColumnResults.Completed += new EventHandler(logrCtxColumnResults_Completed);
        //}

       public void GenerateTable(string DataSourceName, string TableName, List<string> columnNames, string customFilter, Action<List<ListOfStringClass>, Exception> completed)
        {
            _logrCompleted = completed;
            logrCtx = new LogisticRegressionDomainContext();
            InvokeOperation<List<ListOfStringClass>> logrResults = logrCtx.GenerateTable(DataSourceName, TableName, columnNames, customFilter);
            logrResults.Completed += new EventHandler(logrResults_Completed);
        }

       //void ILogisticRegressionServiceAgent.GetColumns(string DataSourceName, string TableName, Action<List<EwavColumn>, Exception> completed)
       //{
       //    throw new NotImplementedException();
       //}

       //void ILogisticRegressionServiceAgent.GenerateTable(string DataSourceName, string TableName, List<string> columnNames, string customFilter, Action<List<ListOfStringClass>, Exception> completed)
       //{
       //    throw new NotImplementedException();
       //}




       void ILogisticRegressionServiceAgent.GetRegressionResults(GadgetParameters gadgetOptions, List<string> columnNames, string customFilter, List<DictionaryDTO> inputDtoList, Action<LogRegressionResults , Exception> completed)
       {
           _rresultsCompleted = completed;
           LogisticRegressionDomainContext logrCtx = new LogisticRegressionDomainContext();
           InvokeOperation<LogRegressionResults> logrCtxRegResults = logrCtx.GetRegressionResult(gadgetOptions, columnNames, inputDtoList, ApplicationViewModel.Instance.EwavDatafilters, ApplicationViewModel.Instance.EwavDefinedVariables,ApplicationViewModel.Instance.AdvancedDataFilterString, customFilter);
           logrCtxRegResults.Completed += new EventHandler(logrCtxRegResults_Completed);
       }
    }
}