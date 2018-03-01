/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ColorItem.cs
 *  Namespace:  Ewav.ContextMenu    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    30/05/2012    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Ewav.ContextMenu
{
    public partial class ColorItem : ItemViewerItem
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the color
        /// </summary>
        public Color Color
        {
            get { return ((SolidColorBrush)Background).Color; }
            set { ((SolidColorBrush)Background).Color = value; }
        }

        #endregion
    }
}