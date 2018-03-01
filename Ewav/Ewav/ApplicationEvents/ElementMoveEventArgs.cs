/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ElementMoveEventArgs.cs
 *  Namespace:  Ewav.Client.Application                             
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Ewav.Client.Application                         


{
    /// <summary>
    /// Arguments class for the ElementMoveEventHandler
    /// </summary>
    public class   ElementMoveEventArgs : EventArgs
    {
        /// <summary>
        /// A dictionary of element names and a new location for 
        /// their upper left corner 
        /// </summary>
        private Dictionary<string, Point> moveDictionary;

        /// <summary>
        ///  Constructor  
        /// </summary>
        /// <param name="MoveDictionry"> A dictionary of element names and a new location for their 
        /// upper left corner</param>
        public ElementMoveEventArgs( Dictionary<string, Point>   MoveDictionry  )
        {
            moveDictionary = MoveDictionary;
        }

        /// <summary>
        /// A dictionary of element names and a new location for 
        /// their upper left corner      
        /// </summary>
        /// <value>The move dictionary.</value>
        public Dictionary<string, Point> MoveDictionary
        {
            get
            {
                return this.moveDictionary;
            }
            set
            {
                this.moveDictionary = value;
            }
        }

   
    }

}