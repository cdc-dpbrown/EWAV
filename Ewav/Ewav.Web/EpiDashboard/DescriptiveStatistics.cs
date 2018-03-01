/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       DescriptiveStatistics.cs
 *  Namespace:  EpiDashboard    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ewav.Web.EpiDashboard
{
    /// <summary>
    /// A structure used to store the mean, median, mode, min, and max of a given numeric column. Also
    /// used to store ANOVA, Kruskal-Wallis and other advanced statistics when appropriate.
    /// </summary>
    public class DescriptiveStatistics
    {
        private double? mean;

        /// <summary>
        /// Gets or sets the filtered observations.
        /// </summary>
        /// <value>The filtered observations.</value>
        public long FilteredObservations
        {
            get
            {
                return this.filteredObservations;
            }
            set
            {
                this.filteredObservations = value;
            }
        }

        public double? Mean
        {
            get { return mean; }
            set { mean = value; }
        }
        private double? grandMean;

        public double? GrandMean
        {
            get { return grandMean; }
            set { grandMean = value; }
        }
        private double? median;

        public double? Median
        {
            get { return median; }
            set { median = value; }
        }
        private double? mode;

        public double? Mode
        {
            get { return mode; }
            set { mode = value; }
        }
        private double? min;

        public double? Min
        {
            get { return min; }
            set { min = value; }
        }
        private double? max;

        public double? Max
        {
            get { return max; }
            set { max = value; }
        }
        private double? q1;

        public double? Q1
        {
            get { return q1; }
            set { q1 = value; }
        }
        private double? q3;

        public double? Q3
        {
            get { return q3; }
            set { q3 = value; }
        }
        private double? sum;

        public double? Sum
        {
            get { return sum; }
            set { sum = value; }
        }

        private long filteredObservations;


        private double observations;

        public double Observations
        {
            get { return observations; }
            set { observations = value; }
        }
        private double? stdDev;

        public double? StdDev
        {
            get { return stdDev; }
            set { stdDev = value; }
        }
        private double? variance;

        public double? Variance
        {
            get { return variance; }
            set { variance = value; }
        }
        private string mainVariable;

        public string MainVariable
        {
            get { return mainVariable; }
            set { mainVariable = value; }
        }
        private string crosstabValue;

        public string CrosstabValue
        {
            get { return crosstabValue; }
            set { crosstabValue = value; }
        }
        private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }

        private double? ssBetween;

        public double? SsBetween
        {
            get { return ssBetween; }
            set { ssBetween = value; }
        }
        private double? dfBetween;

        public double? DfBetween
        {
            get { return dfBetween; }
            set { dfBetween = value; }
        }
        private double? msBetween;

        public double? MsBetween
        {
            get { return msBetween; }
            set { msBetween = value; }
        }
        private double? ssWithin;

        public double? SsWithin
        {
            get { return ssWithin; }
            set { ssWithin = value; }
        }
        private double? dfWithin;

        public double? DfWithin
        {
            get { return dfWithin; }
            set { dfWithin = value; }
        }
        private double? msWithin;

        public double? MsWithin
        {
            get { return msWithin; }
            set { msWithin = value; }
        }
        private double? fStatistic;

        public double? FStatistic
        {
            get { return fStatistic; }
            set { fStatistic = value; }
        }
        private double? anovaPValue;

        public double? AnovaPValue
        {
            get { return anovaPValue; }
            set { anovaPValue = value; }
        }
        private double? chiSquare;

        public double? ChiSquare
        {
            get { return chiSquare; }
            set { chiSquare = value; }
        }
        private double? bartlettPValue;

        public double? BartlettPValue
        {
            get { return bartlettPValue; }
            set { bartlettPValue = value; }
        }
        private double? kruskalWallisH;

        public double? KruskalWallisH
        {
            get { return kruskalWallisH; }
            set { kruskalWallisH = value; }
        }

        private double? kruskalPValue;

        public double? KruskalPValue
        {
            get { return kruskalPValue; }
            set { kruskalPValue = value; }
        }
    }

}