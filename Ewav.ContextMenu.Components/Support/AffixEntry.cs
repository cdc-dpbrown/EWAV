/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       AffixEntry.cs
 *  Namespace:  Ewav.ContextMenu.Components.Internal    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    30/05/2012    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Text.RegularExpressions;

namespace Ewav.ContextMenu.Components.Internal
{
    internal partial class AffixEntry
    {
        #region Public Properties

        public string AddCharacters { get; set; }

        public int[] Condition { get; set; }

        public string StripCharacters { get; set; }

        public int ConditionCount { get; set; }

        #endregion

        #region Constructor

        public AffixEntry()
        {
            AddCharacters = string.Empty;
            Condition = new int[256];
            StripCharacters = string.Empty;
        }

        #endregion
    }
}