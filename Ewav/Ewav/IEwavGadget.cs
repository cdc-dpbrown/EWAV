/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       IEwavGadget.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using Ewav.ViewModels;
using System.Collections.Generic;
using Ewav.Web.Services;

namespace Ewav
{
    public interface IEwavGadget
    {
        /// <summary>
        /// The value for the frameworkelement.Name property 
        /// </summary>  
        string MyControlName { get;   }
        /// <summary>
        /// The value for the UI menus    
        /// </summary>
        string MyUIName { get; }

        /// <summary>
        /// Container that holds gadget level filters.
        /// </summary>
        List<EwavDataFilterCondition> GadgetFilters { get; set; }

        void Reload();


    }
}