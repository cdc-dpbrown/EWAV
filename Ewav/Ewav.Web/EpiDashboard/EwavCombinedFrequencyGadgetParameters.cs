/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       EwavCombinedFrequencyGadgetParameters.cs
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
using System.Web;

namespace Ewav.Web.EpiDashboard
{
    public class EwavCombinedFrequencyGadgetParameters
    {
        public CombineModeTypes CombineMode { get; set; }
        public bool SortHighToLow { get; set; }
        public bool ShowDenominator { get; set; }
        public string TrueValue { get; set; } 
    }
}