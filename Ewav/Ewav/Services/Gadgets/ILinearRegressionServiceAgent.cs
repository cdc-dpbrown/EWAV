/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ILinearRegressionServiceAgent.cs
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
using Ewav.BAL;
using System.Collections.Generic;
using Ewav.Web.Services;
using Ewav.Web.EpiDashboard;

namespace Ewav.Services
{
    public interface ILinearRegressionServiceAgent
    {

      //  
        
        
        
        
        
        
        //  void GetColumns(string DataSourceName, String TableName, Action<List<EwavColumn>, Exception> completed);

        void GetRegressionResults(    GadgetParameters gadgetOptions, List<string> columnNames, List<DictionaryDTO> inputDtoList, Action<LinRegressionResults, Exception> completed);

    }
}