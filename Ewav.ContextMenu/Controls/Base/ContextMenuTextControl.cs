/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ContextMenuTextControl.cs
 *  Namespace:  Ewav.ContextMenu    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    30/05/2012    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.Windows.Controls;

namespace Ewav.ContextMenu
{
    public abstract partial class ContextMenuTextControl : ContextMenuControl
    {
        #region Visual Elements

        /// <summary> 
        /// Text template.
        /// </summary>
        internal TextBlock ElementText { get; set; }
        internal const string ElementTextName = "ElementText";

        #endregion

        #region Event Handling

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ElementText = (TextBlock)GetTemplateChild(ElementTextName);
        }

        #endregion
    }
}