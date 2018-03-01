/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       AffixRule.cs
 *  Namespace:  Ewav.ContextMenu.Components.Internal    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    30/05/2012    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Ewav.ContextMenu.Components.Internal
{
    internal partial class AffixRule
    {
        #region Public Properties

        public bool AllowCombine { get; set; }

        public List<AffixEntry> AffixEntries { get; set; }

        public string Name { get; set; }

        #endregion

        #region Constructor

        public AffixRule()
        {
            AllowCombine = false;
            AffixEntries = new List<AffixEntry>();
            Name = string.Empty;
        }

        #endregion
    }
}