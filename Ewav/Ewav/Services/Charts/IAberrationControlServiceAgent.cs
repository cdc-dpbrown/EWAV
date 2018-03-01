/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       IAberrationControlServiceAgent.cs
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
    public interface IAberrationControlServiceAgent
    {
        /// <summary>
        /// GetFrequencyResults method that will be implemented in the ServiceAgentClass
        /// </summary>
        /// <param name="DataSourceName"></param>
        /// <param name="TableName"></param>
        /// <param name="gadgetParameters"></param>
        /// <param name="completed"></param>
        void GetFrequencyResults(  GadgetParameters gadgetParameters, Action<List<FrequencyResultData>, Exception> completed);

        /// <summary>
        /// CheckColumnType that needs to be implemented in ServiceAgentClass
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        string CheckColumnType(string p);

        /// <summary>
        /// GetColumnsMethod that needs to be implemented in the ServiceAgentClass
        /// </summary>
        /// <param name="DataSourceName"></param>
        /// <param name="TableName"></param>
        /// <param name="completed"></param>
    //     void GetColumns(string DataSourceName, String TableName, Action<List<EwavColumn>, Exception> completed);

    }
}