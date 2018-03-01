/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       IMeansControlServiceAgent.cs
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
    public interface IMeansControlServiceAgent
    {
        void GetFrequencyResults(GadgetParameters gadgetParameters, Action<List<FrequencyResultData>, Exception> completed);

      //    void GetColumns(Action<List<EwavColumn>, Exception> completed);

        void GetCrossTabResults(GadgetParameters gadgetParameters, Action<List<CrossTabResponseObjectDto>, Exception> completed);

        void GetCrossTableAndFreq(GadgetParameters gadgetParameters, Action<FrequencyAndCrossTable, Exception> completed);
    }
}