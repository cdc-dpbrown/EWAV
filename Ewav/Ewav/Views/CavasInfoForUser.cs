/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       CavasInfoForUser.cs
 *  Namespace:  Ewav.Views    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
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
using System.ComponentModel;

namespace Ewav.Views
{
    /// <summary>
    /// 
    /// </summary>
    public class CavasInfoForUser : INotifyPropertyChanged
    {

        /// <summary>
        /// The canvas name
        /// </summary>
        private string canvasName;
        /// <summary>
        /// Represents a Canvas Name
        /// </summary>
        /// <value>
        /// The name of the canvas.
        /// </value>
        public string CanvasName
        {
            get { return canvasName; }
            set { canvasName = value; }
        }

        /// <summary>
        /// The date saved
        /// </summary>
        private string dateSaved;

        /// <summary>
        /// Gets or sets the date saved.
        /// </summary>
        /// <value>
        /// The date saved.
        /// </value>
        public string DateSaved
        {
            get { return dateSaved; }
            set { dateSaved = value; }
        }

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}