/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       LinearRegressionDomainService.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
namespace Ewav.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using EpiDashboard;
    using Ewav.BAL;
    using Ewav.DTO;
    using System.Data;


    // TODO: Create methods containing your application logic.
    /// <summary>
    /// Domain Service Class for Linear Regression Results
    /// </summary>
    [EnableClientAccess()]
    public class LinearRegressionDomainService : DomainService
    {

        DashboardHelper dh;

        /// <summary>
        /// Port this class to client generated file
        /// </summary>
        /// <param name="rr"></param>
        public void PortThisToClient(LinearRegressionResults rr) { }


        [Query(IsComposable = false)]
        public EwavRule_Base Getrule(int id)
        {
            return new EwavRule_Base();

        }



















        /// <summary>
        /// Calls dashboard helper to retrieve the table and coverts in Local representation of table
        /// </summary>
        /// <param name="DataSourceName"></param>
        /// <param name="TableName"></param>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        public List<ListOfStringClass> GenerateTable(string DataSourceName, string TableName, List<string> columnNames)
        {
            List<ListOfStringClass> lls = new List<ListOfStringClass>();
            dh = new DashboardHelper();
            DataTable dt = dh.GenerateTable(columnNames);
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
        public List<EwavColumn> GetColumns(string DataSourceName, string TableName)
        {
            return BAL.Common.GetColumns(DataSourceName, TableName);
        }

        /// <summary>
        /// Domain Service Table that interacts with dashboard helper methods. Recieves input data
        /// from Client, converts in domain service acceptable data structures, calls dashboard helper 
        /// method and wraps the data back in DTO.
        /// This method makes use of classes that are local representation of Regression results classes in EpiInfo
        /// or Statistics Repository.
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
        public LinRegressionResults GetRegressionResult(GadgetParameters gadgetOptions, List<string> columnNames, List<DictionaryDTO> inputDtoList, IEnumerable<EwavDataFilterCondition> ewavDataFilters, List<EwavRule_Base> rules, string filterString = "")
        {
            if (gadgetOptions.UseAdvancedDataFilter)
            {
                dh = new DashboardHelper(   gadgetOptions,    filterString, rules);
                gadgetOptions.UseAdvancedDataFilter = true;
                gadgetOptions.AdvancedDataFilterText = filterString;
            }
            else
            {
                dh = new DashboardHelper(   gadgetOptions,    ewavDataFilters, rules);
            }
            DataTable dt;
            LinRegressionResults results = new LinRegressionResults();
            try
            {
                dt = dh.GenerateTable(columnNames, gadgetOptions);
            }
            catch (Exception e)
            {

                results.ErrorMessage = e.Message;
                return results;
            }
            Dictionary<string, string> inputVariableList = RegressionManager.ConvertDtoToDic(inputDtoList);


            StatisticsRepository.LinearRegression linearRegression = new StatisticsRepository.LinearRegression();

            StatisticsRepository.LinearRegression.LinearRegressionResults regressionResults = linearRegression.LinearRegression(inputVariableList, dt);

            results.RegressionResults = RegressionManager.ConvertToLinRegResults(regressionResults);

            results.CorrelationCoefficient = results.RegressionResults.CorrelationCoefficient;
            results.RegressionDf = results.RegressionResults.RegressionDf;
            results.RegressionF = results.RegressionResults.RegressionF;
            results.RegressionMeanSquare = results.RegressionResults.RegressionMeanSquare;
            results.RegressionSumOfSquares = results.RegressionResults.RegressionSumOfSquares;
            results.ResidualsDf = results.RegressionResults.ResidualsDf;
            results.ResidualsMeanSquare = results.RegressionResults.ResidualsMeanSquare;
            results.ResidualsSumOfSquares = results.RegressionResults.ResidualsSumOfSquares;
            results.TotalDf = results.RegressionResults.TotalDf;
            results.TotalSumOfSquares = results.RegressionResults.TotalSumOfSquares;
            results.Variables = results.RegressionResults.Variables;
            if (results.RegressionResults.ErrorMessage != null)
            {
                results.ErrorMessage = results.RegressionResults.ErrorMessage.Replace("<tlt>", string.Empty).Replace("</tlt>", string.Empty);
            }

            return results;
        }

    }

}