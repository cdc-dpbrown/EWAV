/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       StatCalcServiceAgent.cs
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
using System.Collections.Generic;


namespace Ewav.Services
{
    public class StatCalcServiceAgent : IStatCalcServiceAgent   
    {
        StatCalcDomainContext scdc;

        private Action<StatCalcDTO, Exception> _completed;
        Exception ex = null;
        void statcalcresults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<StatCalcDTO> result =
                 (InvokeOperation<StatCalcDTO>)sender;

             if (result.HasError)
            {
                ex = result.Error;
                result.MarkErrorAsHandled();
            }
            //else
            //{
                StatCalcDTO returnedData = ((InvokeOperation<StatCalcDTO>)sender).Value;
                _completed(returnedData, null);
            //}
        }

        /// <summary>
        /// Implements the method in IStatCalcServiceAgent
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
        /// <param name="completed"></param>
        public void GetStatCalc(int ytVal, int ntVal, int tyVal, int tnVal, int yyVal, int ynVal, int nyVal, int nnVal, 
            List<DictionaryDTO> strataActive, List<DictionaryDTO> strataVals, Action<StatCalcDTO, Exception> completed)
        {
            _completed = completed;
            scdc = new StatCalcDomainContext();
           InvokeOperation<StatCalcDTO> statcalcresults = scdc.GenerateSigTable(ytVal, ntVal, tyVal, tnVal, yyVal, ynVal, nyVal, nnVal, strataActive, strataVals);
            statcalcresults.Completed += new EventHandler(statcalcresults_Completed);
        }
    }
}