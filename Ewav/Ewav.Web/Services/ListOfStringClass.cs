/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ListOfStringClass.cs
 *  Namespace:  Ewav.Web.Services    
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

namespace Ewav.Web.Services
{
    /// <summary>
    /// A class created to help mimic the dataTable used in LogisticRegression Control.
    /// </summary>
    public class ListOfStringClass
    {
        private List<string> ls;

        public List<string> Ls
        {
            get { return ls; }
            set { ls = value; }
        }

    }
}