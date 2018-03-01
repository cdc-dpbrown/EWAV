/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       CombinedFrequencyServiceAgent.cs
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
using Ewav.Web.Services.CombinedFrequencyDomainService;

namespace Ewav.Services
{
    public class CombinedFrequencyServiceAgent : ICombinedFrequencyServiceAgent
    {
        #region Variables
        CombinedFrequencyDomainContext freqCtx;
        private Action<DatatableBag, Exception> _freqCompleted;

        #endregion

        #region Constructor
        public CombinedFrequencyServiceAgent()
        {
            
        }
        #endregion


        #region Completion Callbacks

        void freqTableResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<DatatableBag> result =
                (InvokeOperation<DatatableBag>)sender;
            Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                ex = result.Error;
            }
            DatatableBag returnedData = ((InvokeOperation<DatatableBag>)sender).Value;
            _freqCompleted(returnedData, ex);
        }
        #endregion


        public void GetCombinedFrequencyResults(EwavCombinedFrequencyGadgetParameters combinedParameters, string groupVar, GadgetParameters gadgetParameters, IEnumerable<EwavDataFilterCondition> ewavDataFilters, List<EwavRule_Base> rules, string filterString, Action<DatatableBag, Exception> completed)
        {
            _freqCompleted = completed;
            freqCtx = new CombinedFrequencyDomainContext();
            InvokeOperation<DatatableBag> freqResults = freqCtx.GenerateCombinedFrequency(combinedParameters, groupVar, gadgetParameters, ewavDataFilters, rules, filterString);

            freqResults.Completed += new EventHandler(freqTableResults_Completed);
        }
    }
}