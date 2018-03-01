/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ICombinedFrequencyServiceAgent.cs
 *  Namespace:  Ewav.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using Ewav.DTO;
using System.Collections.Generic;
using Ewav.Web.EpiDashboard;
using Ewav.Web.Services;
using Ewav.BAL;

namespace Ewav.Services
{
    public interface ICombinedFrequencyServiceAgent
    {
        /// <summary>
        /// GetCombinedFrequencyResults method that will be implemented in the ServiceAgentClass
        /// </summary>
        /// <param name="DataSourceName"></param>
        /// <param name="TableName"></param>
        /// <param name="gadgetParameters"></param>
        /// <param name="completed"></param>
        void GetCombinedFrequencyResults(EwavCombinedFrequencyGadgetParameters combinedParameters, string groupVar, GadgetParameters gadgetParameters,
            IEnumerable<EwavDataFilterCondition> ewavDataFilters, List<EwavRule_Base> rules, string filterString,
             Action<DatatableBag, Exception> completed);
    }
}