/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       TotalRecordcountLoadedEventArgs.cs
 *  Namespace:  Ewav.Client.Application    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;

namespace Ewav.Client.Application
{
    public class TotalRecordcountLoadedEventArgs : EventArgs
    {
        private long totalRecordcount;

        public long Recordcount
        {
            get { return totalRecordcount; }
            set { totalRecordcount = value; }
        }


        public TotalRecordcountLoadedEventArgs(long recordCount)
        {
            Recordcount = recordCount;
        }

    }
}