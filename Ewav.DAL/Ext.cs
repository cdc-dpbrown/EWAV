/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       Ext.cs
 *  Namespace:  Ewav.DAL    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
namespace Ewav.DAL
{
    using System;
    using System.Linq;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class Ext
    {
        public static bool SQLTest(this string s)
        {
            if (s.ToLower().Contains("select"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}