/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       Utils.cs
 *  Namespace:  Ewav.ClientCommon        
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

namespace Ewav.ClientCommon    
{
    public class Utils
    {

        public static Object ParseEnum<T>(string s)
        {
            try
            {
                var o = Enum.Parse(typeof(T), s, true);
                return (T)o;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }


    }
}