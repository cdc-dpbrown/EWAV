/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       LogisticRegressionDomainService.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
namespace Ewav.Web.Services
{
    using System.Collections.Generic;
    using System.Data;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using EpiDashboard;
    using Ewav.BAL;
    using Ewav.DTO;
    using System;


    // TODO: Create methods containing your application logic.
    [EnableClientAccess()]
    public class LogisticRegressionDomainService : DomainService
    {
        DashboardHelper dh;

        [Query(IsComposable = false)]
        public EwavRule_Base Getrule(int id)
        {
            return new EwavRule_Base();

        }



        //public void PortThisToClient(RegressionResults rr) { }

        public List<ListOfStringClass> GenerateTable(string DataSourceName, string TableName, List<string> columnNames, string customFilter = "")
        {
            List<ListOfStringClass> lls = new List<ListOfStringClass>();
            dh = new DashboardHelper();
            DataTable dt = dh.GenerateTable(columnNames, customFilter);
            RegressionManager lrm = new RegressionManager();
            lls = lrm.ConvertDataTableToList(dt);
            return new List<ListOfStringClass>();
        }

        /// <summary>
        /// Get a list of columns    
        /// </summary>
        /// <param name="DataSourceName"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        //public List<EwavColumn> GetColumns(string DataSourceName, string TableName)
        //{
        //    return BAL.Common.GetColumns(DataSourceName, TableName);
        //}

        /// <summary>
        /// Domain Service Table that interacts with dashboard helper methods. Recieves input data
        /// from Client, converts in domain service acceptable data structures, calls dashboard helper 
        /// method and wraps the data back in DTO.
        /// </summary>
        /// <param name="DataSourceName"></param>
        /// <param name="TableName"></param>
        /// <param name="gadgetOptions"></param>
        /// <param name="columnNames"></param>
        /// <param name="inputDtoList"></param>
        /// <param name="customFilter"></param>
        /// <returns></returns>  
        /// 

            [Invoke]
        public LogRegressionResults GetRegressionResult(GadgetParameters gadgetOptions, List<string> columnNames, List<DictionaryDTO> inputDtoList, IEnumerable<EwavDataFilterCondition> ewavDataFilters, List<EwavRule_Base> rules, string filterString = "", string customFilter = "")
        {
            if (gadgetOptions.UseAdvancedDataFilter)
            {
                dh = new DashboardHelper(  gadgetOptions,     filterString, rules);
                gadgetOptions.UseAdvancedDataFilter = true;
                gadgetOptions.AdvancedDataFilterText = filterString;
            }
            else
            {
                dh = new DashboardHelper(    gadgetOptions,    ewavDataFilters, rules);
            }
            DataTable dt;  
            Dictionary<string, string> inputVariableList = RegressionManager.ConvertDtoToDic(inputDtoList);

            //LogRegressionResults results = new LogRegressionResults();
            StatisticsRepository.LogisticRegression logisticRegression = new StatisticsRepository.LogisticRegression();
            try
            {
                dt = dh.GenerateTable(columnNames, gadgetOptions, customFilter);
            }
            catch (System.Exception e)
            {
                throw new Exception("Error retrieving data from Dashboard Helper");
                //results.ErrorMessage = e.Message;
                //return results;
            }


  

            StatisticsRepository.LogisticRegression.LogisticRegressionResults regressionResults = logisticRegression.LogisticRegression(inputVariableList, dt);

            LogRegressionResults results = RegressionManager.ConvertToLogRegResults(regressionResults);

            //results.CasesIncluded = results.RegressionResults1.CasesIncluded;
            //results.Convergence = results.RegressionResults1.Convergence;
            //results.FinalLikelihood = results.RegressionResults1.FinalLikelihood;
            //results.Iterations = results.RegressionResults1.Iterations;
            //results.LRDF = results.RegressionResults1.LRDF;
            //results.LRP = results.RegressionResults1.LRP;
            //results.LRStatistic = results.RegressionResults1.LRStatistic;
            //results.ScoreDF = results.RegressionResults1.ScoreDF;
            //results.ScoreP = results.RegressionResults1.ScoreP;
            //results.ScoreStatistic = results.RegressionResults1.ScoreStatistic;
            //if (results.RegressionResults1.ErrorMessage != null)
            //{
            //    results.ErrorMessage = results.RegressionResults1.ErrorMessage.Replace("<tlt>", string.Empty).Replace("</tlt>", string.Empty);
            //}

            //results.Variables = results.RegressionResults1.Variables;


            return results;
        }

    }
}