/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       IStatCalcServiceAgent.cs
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
using Ewav.Web.EpiDashboard;
using Ewav.Web.Services;
using System.Collections.Generic;

namespace Ewav.Services
{
    public interface IStatCalcServiceAgent
    {
        /// <summary>
        /// GetStatCalc method that will be implemented in the ServiceAgentClass
        /// </summary>
        /// <param name="DataSourceName"></param>
        /// <param name="TableName"></param>
        /// <param name="gadgetParameters"></param>
        /// <param name="completed"></param>
        void GetStatCalc(int ytVal, int ntVal, int tyVal, int tnVal, int yyVal, int ynVal, int nyVal, int nnVal,List<DictionaryDTO> strataActive,
            List<DictionaryDTO> strataVals, Action<StatCalcDTO, Exception> completed);
    }
}