/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       SingleTableResults.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
namespace Ewav.Web.Services
{
    public class SingleTableResults
    {
        public System.Nullable<double> OddsRatioEstimate;
        public System.Nullable<double> OddsRatioLower;
        public System.Nullable<double> OddsRatioUpper;
        public double OddsRatioMLEEstimate;
        public double OddsRatioMLEMidPLower;
        public double OddsRatioMLEMidPUpper;
        public double OddsRatioMLEFisherLower;
        public double OddsRatioMLEFisherUpper;
        public double RiskRatioEstimate;
        public System.Nullable<double> RiskRatioLower;
        public System.Nullable<double> RiskRatioUpper;
        public double RiskDifferenceEstimate;
        public double RiskDifferenceLower;
        public double RiskDifferenceUpper;
        public double ChiSquareUncorrectedVal;
        public double ChiSquareUncorrected2P;
        public double ChiSquareMantelVal;
        public double ChiSquareMantel2P;
        public double ChiSquareYatesVal;
        public double ChiSquareYates2P;
        public double MidP;
        public double FisherExactP;
        public double FisherExact2P;
        public string ErrorMessage;



    }
}