/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ScatterDomainService.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
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
    [EnableClientAccess()]
    public class ScatterDomainService : DomainService
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
        [Invoke]
        public ScatterDataDTO GenerateTable(GadgetParameters gadgetOptions,
               IEnumerable<EwavDataFilterCondition> ewavDataFilters, List<EwavRule_Base> rules, string filterString = "")
        {
            List<ListOfStringClass> lls = new List<ListOfStringClass>();

            if (gadgetOptions .UseAdvancedDataFilter)
            {
                dh = new DashboardHelper(gadgetOptions, filterString, rules);
                gadgetOptions.UseAdvancedDataFilter = true;
                gadgetOptions.AdvancedDataFilterText = filterString;
            }
            else
            {
                dh = new DashboardHelper(gadgetOptions, ewavDataFilters, rules);
            }

            MyString ms = new MyString();
            string xAxisVar = gadgetOptions.MainVariableName;
            string yAxisVar = gadgetOptions.CrosstabVariableName;

            List<string> columns = new List<string>();
            columns.Add(xAxisVar);
            columns.Add(yAxisVar);


            DataTable dt = dh.GenerateTable(columns,gadgetOptions);



            List<NumericDataValue> dataValues = new List<NumericDataValue>();
            NumericDataValue minValue = null;
            NumericDataValue maxValue = null;

            //regressTable.FieldsList.Fields

            //foreach (DataRow row in regressTable.Rows)
            foreach (DataRow row in dt.Rows)
            {
                //if (regressTable.GetValueAtRow(yAxisVar, row).Equals(DBNull.Value) || regressTable.GetValueAtRow(xAxisVar, row).Equals(DBNull.Value))
                if (row[yAxisVar].Equals(DBNull.Value) || row[xAxisVar].Equals(DBNull.Value))
                {
                    continue;
                }
                 NumericDataValue currentValue = new NumericDataValue() { DependentValue = Convert.ToDecimal(row[yAxisVar]), IndependentValue = Convert.ToDecimal(row[xAxisVar]) };
                //NumericDataValue currentValue = new NumericDataValue() { DependentValue = Convert.ToDecimal(regressTable.GetValueAtRow(yAxisVar, row)), IndependentValue = Convert.ToDecimal(regressTable.GetValueAtRow(xAxisVar, row)) };
                dataValues.Add(currentValue);
                if (minValue == null)
                {
                    minValue = currentValue;
                }
                else
                {
                    if (currentValue.IndependentValue < minValue.IndependentValue)
                    {
                        minValue = currentValue;
                    }
                }
                if (maxValue == null)
                {
                    maxValue = currentValue;
                }
                else
                {
                    if (currentValue.IndependentValue > maxValue.IndependentValue)
                    {
                        maxValue = currentValue;
                    }
                }
            }

            StatisticsRepository.LinearRegression linearRegression = new StatisticsRepository.LinearRegression();
            Dictionary<string, string> inputVariableList = new Dictionary<string, string>();
            inputVariableList.Add(yAxisVar, "dependvar");
            inputVariableList.Add("intercept", "true");
            inputVariableList.Add("includemissing", "false");
            inputVariableList.Add("p", "0.95");
            inputVariableList.Add(xAxisVar, "unsorted");

            StatisticsRepository.LinearRegression.LinearRegressionResults regresResults = linearRegression.LinearRegression(inputVariableList, dt);
            LinRegressionResults results = new LinRegressionResults();

            results = RegressionManager.ConvertToLinRegResults(regresResults);

            //results.CorrelationCoefficient = results.RegressionResults.CorrelationCoefficient;
            //results.RegressionDf = results.RegressionResults.RegressionDf;
            //results.RegressionF = results.RegressionResults.RegressionF;
            //results.RegressionMeanSquare = results.RegressionResults.RegressionMeanSquare;
            //results.RegressionSumOfSquares = results.RegressionResults.RegressionSumOfSquares;
            //results.ResidualsDf = results.RegressionResults.ResidualsDf;
            //results.ResidualsMeanSquare = results.RegressionResults.ResidualsMeanSquare;
            //results.ResidualsSumOfSquares = results.RegressionResults.ResidualsSumOfSquares;
            //results.TotalDf = results.RegressionResults.TotalDf;
            //results.TotalSumOfSquares = results.RegressionResults.TotalSumOfSquares;
            //results.Variables = results.RegressionResults.Variables;
            if (results.ErrorMessage != null)
            {
                results.ErrorMessage = results.ErrorMessage.Replace("<tlt>", string.Empty).Replace("</tlt>", string.Empty);
            }


            ScatterDataDTO scatterDTO = new ScatterDataDTO();

            scatterDTO.DataValues = dataValues;
            scatterDTO.RegresResults = results;
            scatterDTO.MinValue = minValue;
            scatterDTO.MaxValue = maxValue;

            //RegressionManager lrm = new RegressionManager();
            //lls = lrm.ConvertDataTableToList(dt);
            //return new List<ListOfStringClass>();

            return scatterDTO;
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
        //public LinRegressionResults GetRegressionResult(string DataSourceName, string TableName, GadgetParameters gadgetOptions, List<string> columnNames, List<DictionaryDTO> inputDtoList)
        //{
        //    dh = new DashboardHelper();
        //    DataTable dt = dh.GenerateTable(columnNames, gadgetOptions);
        //    Dictionary<string, string> inputVariableList = RegressionManager.ConvertDtoToDic(inputDtoList);

        //    LinRegressionResults results = new LinRegressionResults();
        //    StatisticsRepository.LinearRegression linearRegression = new StatisticsRepository.LinearRegression();

        //    StatisticsRepository.LinearRegression.LinearRegressionResults regressionResults = linearRegression.LinearRegression(inputVariableList, dt);

        //    results.RegressionResults = RegressionManager.ConvertToLinRegResults(regressionResults);

        //    results.CorrelationCoefficient = results.RegressionResults.CorrelationCoefficient;
        //    results.RegressionDf = results.RegressionResults.RegressionDf;
        //    results.RegressionF = results.RegressionResults.RegressionF;
        //    results.RegressionMeanSquare = results.RegressionResults.RegressionMeanSquare;
        //    results.RegressionSumOfSquares = results.RegressionResults.RegressionSumOfSquares;
        //    results.ResidualsDf = results.RegressionResults.ResidualsDf;
        //    results.ResidualsMeanSquare = results.RegressionResults.ResidualsMeanSquare;
        //    results.ResidualsSumOfSquares = results.RegressionResults.ResidualsSumOfSquares;
        //    results.TotalDf = results.RegressionResults.TotalDf;
        //    results.TotalSumOfSquares = results.RegressionResults.TotalSumOfSquares;
        //    results.Variables = results.RegressionResults.Variables;
        //    if (results.RegressionResults.ErrorMessage != null)
        //    {
        //        results.ErrorMessage = results.RegressionResults.ErrorMessage.Replace("<tlt>", string.Empty).Replace("</tlt>", string.Empty);
        //    }

        //    return results;
        //}
    }
}