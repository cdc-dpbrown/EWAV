/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       BinomialServiceAgent.cs
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
using Ewav.Web.Services;
using System.ServiceModel.DomainServices.Client;

namespace Ewav.Services
{
    public class BinomialServiceAgent : IBinomialServiceAgent
    {
        BinomialDomainContext dCtx;
        private Action<BinomialStatCalcDTO, Exception> completed;
        public void GetBinomialStatCalc(string txtNumerator, string txtObserved, string txtExpected, Action<Web.Services.BinomialStatCalcDTO, Exception> completed)
        {
            this.completed = completed;
            dCtx = new BinomialDomainContext();
            InvokeOperation<BinomialStatCalcDTO> dResults = dCtx.GenerateBinomial(txtNumerator, txtObserved, txtExpected);
            dResults.Completed += new EventHandler(dResults_Completed);
        }

        void dResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<BinomialStatCalcDTO> result =
                (InvokeOperation<BinomialStatCalcDTO>)sender;
            Exception ex = null;
            if (result.HasError)
            {
                ex = result.Error;
                result.MarkErrorAsHandled();
            }
            //else
            //{
                BinomialStatCalcDTO returnedData = ((InvokeOperation<BinomialStatCalcDTO>)sender).Value;
                completed(returnedData, null);
            //}
        }
    }
}