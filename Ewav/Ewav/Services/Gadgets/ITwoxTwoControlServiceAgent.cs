/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ITwoxTwoControlServiceAgent.cs
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
    public interface ITwoxTwoControlServiceAgent
    {
        void GetFrequencyResults(string DataSourceName, string TableName, GadgetParameters gadgetParameters,
                IEnumerable<EwavDataFilterCondition> ewavDataFilters, 
            Action<List<FrequencyResultData>, Exception> completed);

        string CheckColumnType(string p);

        void GetColumns(string DataSourceName, String TableName, Action<List<EwavColumn>, Exception> completed);

        void GetCrossTabResults(string DataSourceName, string TableName, GadgetParameters gadgetParameters, Action<List<CrossTabResponseObjectDto>, Exception> completed);
    }
}