/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ILogisticRegressionServiceAgent.cs
 *  Namespace:  Ewav.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
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
    public interface ILogisticRegressionServiceAgent
    {
        /// <summary>
        /// GetColumnsMethod that needs to be implemented in the ServiceAgentClass
        /// </summary>
        /// <param name="DataSourceName"></param>
        /// <param name="TableName"></param>
        /// <param name="completed"></param>
      //  void GetColumns(string DataSourceName, String TableName, Action<List<EwavColumn>, Exception> completed);


        void GenerateTable(string DataSourceName, String TableName, List<string> columnNames, string customFilter, Action<List<ListOfStringClass>, Exception> completed);

        void GetRegressionResults(GadgetParameters gadgetOptions, List<string> columnNames, string customFilter, List<DictionaryDTO> inputDtoList, Action<LogRegressionResults, Exception> completed);
    }
}