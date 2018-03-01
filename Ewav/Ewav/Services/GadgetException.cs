/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       GadgetException.cs
 *  Namespace:  Ewav.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ewav.Services
{
    class GadgetException : Exception
    {
        private string p;

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public GadgetException(string p)
        {
            // TODO: Complete member initialization
            this.p = p;
            Message = p;
        }
    }
}