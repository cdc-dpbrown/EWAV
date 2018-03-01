/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MySingleTableResults.cs
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
using System.ComponentModel;

namespace Ewav.Web.Services
{
    public class MySingleTableResults
    {
        private double? chiSquareMantel2P;

        public double? ChiSquareMantel2P
        {
            get { return chiSquareMantel2P; }
            set { chiSquareMantel2P = value; } // NotifyPropertyChanged("ChiSquareMantel2P");
        }
        private double? chiSquareMantelVal;

        public double? ChiSquareMantelVal
        {
            get { return chiSquareMantelVal; }
            set { chiSquareMantelVal = value; }
        }
        private double? chiSquareUncorrected2P;

        public double? ChiSquareUncorrected2P
        {
            get { return chiSquareUncorrected2P; }
            set { chiSquareUncorrected2P = value; }
        }

        private double? chiSquareUncorrectedVal;

        public double? ChiSquareUncorrectedVal
        {
            get { return chiSquareUncorrectedVal; }
            set { chiSquareUncorrectedVal = value; }
        }
        private double? chiSquareYates2P;

        public double? ChiSquareYates2P
        {
            get { return chiSquareYates2P; }
            set { chiSquareYates2P = value; }
        }
        private double? chiSquareYatesVal;

        public double? ChiSquareYatesVal
        {
            get { return chiSquareYatesVal; }
            set { chiSquareYatesVal = value; }
        }
        private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }
        private double? fisherExact2P;

        public double? FisherExact2P
        {
            get { return fisherExact2P; }
            set { fisherExact2P = value; }
        }
        private double? fisherExactP;

        public double? FisherExactP
        {
            get { return fisherExactP; }
            set { fisherExactP = value; }
        }
        private double? midP;

        public double? MidP
        {
            get { return midP; }
            set { midP = value; }
        }
        private double? oddsRatioEstimate;

        public double? OddsRatioEstimate
        {
            get { return oddsRatioEstimate; }
            set { oddsRatioEstimate = value; }
        }
        private double? oddsRatioLower;

        public double? OddsRatioLower
        {
            get { return oddsRatioLower; }
            set { oddsRatioLower = value; }
        }
        private double? oddsRatioMLEEstimate;

        public double? OddsRatioMLEEstimate
        {
            get { return oddsRatioMLEEstimate; }
            set { oddsRatioMLEEstimate = value; }
        }
        private double? oddsRatioMLEFisherLower;

        public double? OddsRatioMLEFisherLower
        {
            get { return oddsRatioMLEFisherLower; }
            set { oddsRatioMLEFisherLower = value; }
        }
        private double? oddsRatioMLEFisherUpper;

        public double? OddsRatioMLEFisherUpper
        {
            get { return oddsRatioMLEFisherUpper; }
            set { oddsRatioMLEFisherUpper = value; }
        }
        private double? oddsRatioMLEMidPLower;

        public double? OddsRatioMLEMidPLower
        {
            get { return oddsRatioMLEMidPLower; }
            set { oddsRatioMLEMidPLower = value; }
        }
        private double? oddsRatioMLEMidPUpper;

        public double? OddsRatioMLEMidPUpper
        {
            get { return oddsRatioMLEMidPUpper; }
            set { oddsRatioMLEMidPUpper = value; }
        }
        private double? oddsRatioUpper;

        public double? OddsRatioUpper
        {
            get { return oddsRatioUpper; }
            set { oddsRatioUpper = value; }
        }
        private double? riskDifferenceEstimate;

        public double? RiskDifferenceEstimate
        {
            get { return riskDifferenceEstimate; }
            set { riskDifferenceEstimate = value; }
        }
        private double? riskDifferenceLower;

        public double? RiskDifferenceLower
        {
            get { return riskDifferenceLower; }
            set { riskDifferenceLower = value; }
        }
        private double? riskDifferenceUpper;

        public double? RiskDifferenceUpper
        {
            get { return riskDifferenceUpper; }
            set { riskDifferenceUpper = value; }
        }
        private double? riskRatioEstimate;

        public double? RiskRatioEstimate
        {
            get { return riskRatioEstimate; }
            set { riskRatioEstimate = value; }
        }
        private double? riskRatioLower;

        public double? RiskRatioLower
        {
            get { return riskRatioLower; }
            set { riskRatioLower = value; }
        }
        private double? riskRatioUpper;

        public double? RiskRatioUpper
        {
            get { return riskRatioUpper; }
            set { riskRatioUpper = value; }
        }
    }
}