/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       Word.cs
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
    internal partial class Word : IComparable
    {
        #region Public Properties

        public string AffixKeys { get; set; }

        public int Index { get; set; }

        public string PhoneticCode { get; set; }

        public string Text { get; set; }

        internal int EditDistance { get; set; }

        internal int Height { get; set; }

        #endregion

        #region Constructor

        public Word()
        {
            AffixKeys = string.Empty;
            EditDistance = 0;
            Height = 0;
            Index = 0;
            PhoneticCode = string.Empty;
        }

        #endregion

        #region Public Methods

        public int CompareTo(object obj)
        {
            return EditDistance.CompareTo(((Word)obj).EditDistance);
        }

        #endregion
    }
}