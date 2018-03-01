/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       LinRegressionResults.cs
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
using System.Text;

namespace Ewav.Web.Services
{
    /// <summary>
    /// Linear Regression results class. Local representation of what is presented in Statistics Repository.
    /// </summary>
    public class LinRegressionResults
    {

        private double correlationCoefficient;

        public double CorrelationCoefficient
        {
            get { return correlationCoefficient; }
            set { correlationCoefficient = value; }
        }
            private string errorMessage;

            public string ErrorMessage
            {
                get { return errorMessage; }
                set { errorMessage = value; }
            }
            private int regressionDf;

            public int RegressionDf
            {
                get { return regressionDf; }
                set { regressionDf = value; }
            }
            private double regressionF;

            public double RegressionF
            {
                get { return regressionF; }
                set { regressionF = value; }
            }
            private double regressionMeanSquare;

            public double RegressionMeanSquare
            {
                get { return regressionMeanSquare; }
                set { regressionMeanSquare = value; }
            }
            private double regressionSumOfSquares;

            public double RegressionSumOfSquares
            {
                get { return regressionSumOfSquares; }
                set { regressionSumOfSquares = value; }
            }
            private int residualsDf;

            public int ResidualsDf
            {
                get { return residualsDf; }
                set { residualsDf = value; }
            }
            private double residualsMeanSquare;

            public double ResidualsMeanSquare
            {
                get { return residualsMeanSquare; }
                set { residualsMeanSquare = value; }
            }
            private double residualsSumOfSquares;

            public double ResidualsSumOfSquares
            {
                get { return residualsSumOfSquares; }
                set { residualsSumOfSquares = value; }
            }
            private int totalDf;

            public int TotalDf
            {
                get { return totalDf; }
                set { totalDf = value; }
            }
            private double totalSumOfSquares;

            public double TotalSumOfSquares
            {
                get { return totalSumOfSquares; }
                set { totalSumOfSquares = value; }
            }
            private List<LinearRegVariableRow> variables;

            public List<LinearRegVariableRow> Variables
            {
                get { return variables; }
                set { variables = value; }
            }
        //Redundant unnecessary attribute. 
            //private LinRegressionResults regressionResults;

            //public LinRegressionResults RegressionResults
            //{
            //    get { return regressionResults; }
            //    set { regressionResults = value; }
            //}   

    }
}