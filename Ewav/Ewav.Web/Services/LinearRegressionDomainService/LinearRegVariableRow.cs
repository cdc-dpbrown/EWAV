/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       LinearRegVariableRow.cs
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
    /// Linear Regression variable Row. It was struct in EpiInfo
    /// </summary>
    public class LinearRegVariableRow
    {
        private double coefficient;

        public double Coefficient
        {
            get { return coefficient; }
            set { coefficient = value; }
        }
        private double ftest;

        public double Ftest
        {
            get { return ftest; }
            set { ftest = value; }
        }
        private double p;

        public double P
        {
            get { return p; }
            set { p = value; }
        }
        private double stdError;

        public double StdError
        {
            get { return stdError; }
            set { stdError = value; }
        }
        private string variableName;

        public string VariableName
        {
            get { return variableName; }
            set { variableName = value; }
        }


    }
}