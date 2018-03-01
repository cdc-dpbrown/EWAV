/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ScatterDTO.cs
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

namespace Ewav.Web.Services
{
    public class ScatterDataDTO
    {
        private List<NumericDataValue> dataValues;

        public List<NumericDataValue> DataValues
        {
            get { return dataValues; }
            set { dataValues = value; }
        }

        private LinRegressionResults regresResults;

        public LinRegressionResults RegresResults
        {
            get { return regresResults; }
            set { regresResults = value; }
        }

        private NumericDataValue maxValue;

        public NumericDataValue MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }

        private NumericDataValue minValue;

        public NumericDataValue MinValue
        {
            get { return minValue; }
            set { minValue = value; }
        }
    }

    public class NumericDataValue
    {
        public decimal DependentValue { get; set; }
        public decimal IndependentValue { get; set; }
    }
}