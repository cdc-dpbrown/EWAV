/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       LogisticRegressionManager.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
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
    public class LogisticRegressionManager
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
            //List<string> ls2;

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
        /// Helper method that Mapps LogisticRegressionResults to RegressionResults(custom) class.
        /// </summary>
        /// <param name="lrr"></param>
        /// <returns></returns>
        //public RegressionResults MapLogRegResultsToDto(LogisticRegressionResults lrr) 
        //{
        //    RegressionResults rr = new RegressionResults();
        //    rr.CasesIncluded = lrr.casesIncluded;
        //    rr.Convergence = lrr.convergence;
        //    rr.FinalLikelihood = lrr.finalLikelihood;
        //    rr.Iterations = lrr.iterations;
        //    rr.LRDF= lrr.LRDF;
        //    rr.LRP = lrr.LRP;
        //    rr.LRStatistic = lrr.LRStatistic;
        //    rr.ScoreDF = lrr.scoreDF;
        //    rr.ScoreP = lrr.scoreP;
        //    rr.ScoreStatistic = lrr.scoreStatistic;
        //    rr.Variables = lrr.variables;
        //    return rr;
        //}

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
    }

    /// <summary>
    /// custome LogisticRegressionResults. It was struct in EpiInfo statistical repository.. 
    /// </summary>
    public class LogisticRegressionResults
    {
        public int casesIncluded;
        public string convergence;
        public string errorMessage;
        public double finalLikelihood;
        public int iterations;
        public double LRDF;
        public double LRP;
        public double LRStatistic;
        public double scoreDF;
        public double scoreP;
        public double scoreStatistic;
        public List<VariableRow> variables;
    }


    /// <summary>
    /// Custome VariableRow class. It was struct in EpiInfo statistical repository.
    /// </summary>
    public class VariableRow
    {
        private string variableName;

        public string VariableName
        {
            get { return variableName; }
            set { variableName = value; }
        }
        private double oddsRatio;

        public double OddsRatio
        {
            get { return oddsRatio; }
            set { oddsRatio = value; }
        }

        private double ninetyFivePercent;

        public double NinetyFivePercent
        {
            get { return ninetyFivePercent; }
            set { ninetyFivePercent = value; }
        }
        private double ci;

        public double Ci
        {
            get { return ci; }
            set { ci = value; }
        }
        private double coefficient;

        public double Coefficient
        {
            get { return coefficient; }
            set { coefficient = value; }
        }
        private double se;

        public double Se
        {
            get { return se; }
            set { se = value; }
        }
        private double z;

        public double Z
        {
            get { return z; }
            set { z = value; }
        }
        private double p;

        public double P
        {
            get { return p; }
            set { p = value; }
        }
    }
}