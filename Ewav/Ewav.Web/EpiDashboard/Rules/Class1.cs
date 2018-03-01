/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       Class1.cs
 *  Namespace:  Ewav.Web              
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
using Ewav.Web.Services;

namespace Ewav.Web          
{
    public class Class1
    {
        List<EwavDataFilterCondition> dfc = new List<EwavDataFilterCondition>();    

        /// <summary>
        /// Gets or sets the DFC.
        /// </summary>
        /// <value>The DFC.</value>
        public List<EwavDataFilterCondition> Dfc
        {
            get
            {
                return this.dfc;
            }
            set
            {
                this.dfc = value;
            }
        }
    }
}