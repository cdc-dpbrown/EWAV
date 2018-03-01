/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       StatCalc2x2.cs
 *  Namespace:  Ewav.DTO    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Linq;

namespace Ewav.DTO
{
    /// <summary>
    /// This Class is used as a DTO for Statcalc2x2.
    /// </summary>
    public class StatCalc2x2
    {
        public string ChiSqCorP { get; set; }
        public string ChiSqCorVal { get; set; }
        public string ChiSqManP { get; set; }
        public string ChiSqManVal { get; set; }
        public string ChiSqUncP { get; set; }
        public string ChiSqUncVal { get; set; }
        public string FisherExact { get; set; }
        public string FisherExact2P { get; set; }
        public string FisherLower { get; set; }
        public string FisherUpper { get; set; }
        public string MidPEstimate { get; set; }
        public string MidPExact { get; set; }
        public string MidPLower { get; set; }
        public string MidPUpper { get; set; }
        public string OddsRatioEstimate { get; set; }
        public string OddsRatioLower { get; set; }
        public string OddsRatioUpper { get; set; }
        public string RiskDifferenceEstimate { get; set; }
        public string RiskDifferenceLower { get; set; }
        public string RiskDifferenceUpper { get; set; }
        public string RiskRatioEstimate { get; set; }
        public string RiskRatioLower { get; set; }
        public string RiskRatioUpper { get; set; }
    }
}