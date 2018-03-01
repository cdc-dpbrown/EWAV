/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       EpiCurveServiceAgent.cs
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
using Ewav.BAL;
using Ewav.ViewModels;
using Ewav.Web.Services;
using System.ServiceModel.DomainServices.Client;
using Ewav.Client.Application;



namespace Ewav.Services
{
    public class EpiCurveServiceAgent : IEpiCurveServiceAgent
    {
        private Action<List<EwavColumn>, Exception> _completed;
        private Action<Web.Services.DatatableBag, Exception> _epiCrvCompleted;

        public void GetEpiCurveResults(Ewav.Web.EpiDashboard.GadgetParameters gadgetParameters, bool byEpiWeek, string dateVar, string caseStatusVar, Action<Web.Services.DatatableBag, Exception> completed)
        {
            _epiCrvCompleted = completed;
            EpiCurveDomainContext eCrvCtx = new EpiCurveDomainContext();
            InvokeOperation<Web.Services.DatatableBag> eCrvResults = eCrvCtx.GetEpiCurveData(gadgetParameters,
                     ApplicationViewModel.Instance.EwavDatafilters, ApplicationViewModel.Instance.EwavDefinedVariables, ApplicationViewModel.Instance.AdvancedDataFilterString, byEpiWeek, dateVar, caseStatusVar);
            eCrvResults.Completed += new EventHandler(eCrvResults_Completed);
        }

        void eCrvResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<Web.Services.DatatableBag> result = (InvokeOperation<Web.Services.DatatableBag>)sender;
           Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                //throw new GadgetException(result.Error.Message);
                ex = result.Error;
            }
            //else
            //{
                Web.Services.DatatableBag returnedData = ((InvokeOperation<Web.Services.DatatableBag>)sender).Value;
                _epiCrvCompleted(returnedData, null);
            //}
        }

        public string CheckColumnType(string p)
        {
            throw new NotImplementedException();
        }

        public void GetColumns(string DataSourceName, string TableName, Action<System.Collections.Generic.List<BAL.EwavColumn>, Exception> completed)
        {
            _completed = completed;
            EpiCurveDomainContext eCrvCtx = new EpiCurveDomainContext();
            InvokeOperation<List<EwavColumn>> freqColumnResults = eCrvCtx.GetColumns(DataSourceName, TableName);
            freqColumnResults.Completed += new EventHandler(freqColumnResults_Completed);
        }

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


    }
}