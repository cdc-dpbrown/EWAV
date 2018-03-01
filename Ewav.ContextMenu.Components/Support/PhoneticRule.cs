/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       PhoneticRule.cs
 *  Namespace:  Ewav.ContextMenu.Components.Internal    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    30/05/2012    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;

namespace Ewav.ContextMenu.Components.Internal
{
    internal partial class PhoneticRule
    {
        #region Public Properties

        public bool BeginningOnly { get; set; }

        public int[] Condition { get; set; }

        public int ConditionCount { get; set; }

        public int ConsumeCount { get; set; }

        public bool EndOnly { get; set; }

        public int Priority { get; set; }

        public bool ReplaceMode { get; set; }

        public string ReplaceString { get; set; }

        #endregion

        #region Constructor

        public PhoneticRule()
        {
            Condition = new int[256];
            ConditionCount = 0;
            ReplaceMode = false;
        }

        #endregion
    }
}