/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MefAvailableGadgetListReadyEventArgs.cs
 *  Namespace:  Ewav.Client.Application    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.Windows;

using System.Collections.Generic;
using System;

namespace Ewav.Client.Application
{
    public class MefAvailableGadgetListReadyEventArgs : EventArgs
    {
        private List<string> availableGadgets;

        /// <summary>
        ///  Constructor  
        /// </summary>
        /// <param name="MoveDictionry"> A dictionary of element names and a new location for their 
        /// upper left corner</param>
        public MefAvailableGadgetListReadyEventArgs(List<string> gadgets)
        {
            availableGadgets = gadgets;
        }

        /// <summary>
        /// Gets or sets the available gadgets.
        /// </summary>
        /// <value>The available gadgets.</value>
        public List<string> AvailableGadgets
        {
            get
            {
                return this.availableGadgets;
            }
            set
            {
                this.availableGadgets = value;
            }
        }
    }

}