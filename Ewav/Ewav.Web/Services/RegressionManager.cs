/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       RegressionManager.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace Ewav.Web.Services
{
    /// <summary>
    /// LogisticRegression Manager Class. It has all the helper methods needed in LogisticRegression 
    /// Domain Service class.
    /// </summary>
    public class RegressionManager
    {
        /// <summary>
        /// Methods that accepts DataTable as Parameter and converts that table into
        /// List<ListOfStringClass>.
        /// </summary>
        /// <param name="dtDataSources"></param>
        /// <returns></returns>
        public List<ListOfStringClass> ConvertDataTableToList(DataTable dt)
        {
            List<ListOfStringClass> lls = new List<ListOfStringClass>();

            ListOfStringClass colLs = new ListOfStringClass();
            ListOfStringClass ls1;
            
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                colLs.Ls.Add(dt.Columns[i].ColumnName.ToString());
            }

            lls.Add(colLs);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ls1 = new ListOfStringClass();
                for (int j = i; j < dt.Columns.Count; j++)
                {
                    ls1.Ls.Add(dt.Rows[i][j].ToString());

                }
                lls.Add(ls1);
            }
            return lls;
        }

        /// <summary>
        /// GetFactory that takes List of DTO Objects and converts them into List<string,string>
        /// </summary>
        /// <param name="inputDtoList"></param>
        /// <returns></returns>
        internal static Dictionary<string, string> ConvertDtoToDic(List<DictionaryDTO> inputDtoList)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            for (int i = 0; i < inputDtoList.Count; i++)
            {
                dict.Add(inputDtoList[i].Key.VarName.ToString(), inputDtoList[i].Value.VarName.ToString());
            }
            return dict;
        }

        /// <summary>
        /// GetFactory that converts Local representation of table in to DataTable
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        internal static DataTable ConvertListToDataTable(List<ListOfStringClass> dataTable)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// GetFactory that converts LogisticRegressionResults values to LogRessionResults(custom) class.
        /// </summary>
        /// <param name="logisticRegressionResults"></param>
        /// <returns></returns>
        internal static LogRegressionResults ConvertToLogRegResults(StatisticsRepository.LogisticRegression.LogisticRegressionResults logisticRegressionResults)
        {
            LogRegressionResults logRegResults = new LogRegressionResults();

            logRegResults.CasesIncluded = logisticRegressionResults.casesIncluded;
            logRegResults.Convergence = logisticRegressionResults.convergence;
            logRegResults.FinalLikelihood = logisticRegressionResults.finalLikelihood;
            logRegResults.Iterations = logisticRegressionResults.iterations;
            logRegResults.LRDF = logisticRegressionResults.LRDF;
            logRegResults.LRP = logisticRegressionResults.LRP;
            logRegResults.LRStatistic = logisticRegressionResults.LRStatistic;
            logRegResults.ScoreDF = logisticRegressionResults.scoreDF;
            logRegResults.ScoreP = logisticRegressionResults.scoreP;
            logRegResults.ScoreStatistic = logisticRegressionResults.scoreStatistic;
            logRegResults.Variables = ConvertToVariableClass(logisticRegressionResults.variables);
            logRegResults.ErrorMessage = logisticRegressionResults.errorMessage;
            return logRegResults;
        }

        /// <summary>
        /// GetFactory that converts LogisticRegressionResults values to LogRessionResults(custom) class.
        /// </summary>
        /// <param name="logisticRegressionResults"></param>
        /// <returns></returns>
        internal static LinRegressionResults ConvertToLinRegResults(StatisticsRepository.LinearRegression.LinearRegressionResults linearRegressionResults)
        {
            LinRegressionResults linRegResults = new LinRegressionResults();

            linRegResults.CorrelationCoefficient = linearRegressionResults.correlationCoefficient;
            linRegResults.RegressionDf = linearRegressionResults.regressionDf;
            linRegResults.RegressionF = linearRegressionResults.regressionF;
            linRegResults.RegressionMeanSquare = linearRegressionResults.regressionMeanSquare;
            linRegResults.RegressionSumOfSquares = linearRegressionResults.regressionSumOfSquares;
            linRegResults.ResidualsDf = linearRegressionResults.residualsDf;
            linRegResults.ResidualsMeanSquare = linearRegressionResults.residualsMeanSquare;
            linRegResults.ResidualsSumOfSquares = linearRegressionResults.residualsSumOfSquares;
            linRegResults.TotalDf = linearRegressionResults.totalDf;
            linRegResults.TotalSumOfSquares = linearRegressionResults.totalSumOfSquares;
            linRegResults.Variables = ConvertToVariableClass(linearRegressionResults.variables);
            linRegResults.ErrorMessage = linearRegressionResults.errorMessage;
            return linRegResults;
        }

        /// <summary>
        /// GetFactory that converts List<StatisticsRepository.LogisticRegression.VariableRow>
        /// into List<VariableRow> (custom) class.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static List<VariableRow> ConvertToVariableClass(List<StatisticsRepository.LogisticRegression.VariableRow> list)
        {
            List<VariableRow> listVarRow = new List<VariableRow>();
            if (list != null)
            {
                
            
            foreach (var item in list)
            {
                VariableRow varRow = new VariableRow();
                varRow.Ci = item.ci;
                varRow.Coefficient = item.coefficient;
                varRow.NinetyFivePercent = item.ninetyFivePercent;
                varRow.OddsRatio = item.oddsRatio;
                varRow.P = item.P;
                varRow.Se = item.se;
                varRow.VariableName = item.variableName;
                varRow.Z = item.Z;
                listVarRow.Add(varRow);
            }
            }
            return listVarRow;
            
        }

        /// <summary>
        /// GetFactory that converts List<StatisticsRepository.LogisticRegression.VariableRow>
        /// into List<VariableRow> (custom) class.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static List<LinearRegVariableRow> ConvertToVariableClass(List<StatisticsRepository.LinearRegression.VariableRow> list)
        {
            List<LinearRegVariableRow> listVarRow = new List<LinearRegVariableRow>();
            if (list != null)
            {
                foreach (var item in list)
                {
                    LinearRegVariableRow varRow = new LinearRegVariableRow();
                    varRow.Coefficient = item.coefficient;
                    varRow.Ftest = item.Ftest;
                    varRow.StdError = item.stdError;
                    varRow.P = item.P;
                    varRow.VariableName = item.variableName;
                    listVarRow.Add(varRow);
                }
            }
            return listVarRow;

        }

        /// <summary>
        /// GetFactory that converts List of MyString to List of simple strings.
        /// </summary>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        internal static List<string> ConvertMyStringToString(List<MyString> columnNames)
        {
            List<string> ls = new List<string>();
            foreach (var item in columnNames)
            {
                ls.Add(item.VarName);
            }
            return ls;
        }
    }
    
}