/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       IDatasourceServiceAgent.cs
 *  Namespace:  Ewav.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using Ewav.BAL;
using Ewav.DTO;
using System.Collections.Generic;
using Ewav.Web.Services;

namespace Ewav.Services
{
    public interface IDatasourceServiceAgent
    {
        /// <summary>
        /// Gets the datasources2.
        /// </summary>
        /// <param name="datasourcesCompleted2">The datasources completed2.</param>
        void GetDatasources2(Action<IEnumerable<EwavDatasourceDto>, Exception> completed);

    


        void DatasetRecordCount(string DatasetName, Action<long, Exception> completed);

        void DatasetFilteredRecordCount(string DatasetName, object Filter, Action<long, Exception> completed);

        void GetRecordCount(List<EwavDataFilterCondition> filterList, List<EwavRule_Base> rules, string tableName, string dsName, Action<string, Exception> completed);

        void GetRecordCount(List<EwavRule_Base> rules,string s, string tableName, string dsName, Action<string, Exception> completed);

        void ReadFilterString(List<EwavDataFilterCondition> filterList, List<EwavRule_Base> rules, string tableName, string dsName, Action<string, Exception> completed);


        void ReadAllDatasourcesInMyOrg(int orgId, Action<List<DTO.DatasourceDto>, Exception> completed);
    }
}