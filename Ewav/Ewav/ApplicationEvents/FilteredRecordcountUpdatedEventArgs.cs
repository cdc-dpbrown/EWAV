﻿/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       FilteredRecordcountUpdatedEventArgs.cs
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
    public class FilteredRecordcountUpdatedEventArgs : EventArgs
    {
        private long filteredRecordcount;
    
        public FilteredRecordcountUpdatedEventArgs(long filteredRecordCount)
        {
            FilteredRecordcount = filteredRecordCount;
        }

        public long FilteredRecordcount
        {
            get
            {
                return filteredRecordcount;
            }
            set
            {
                filteredRecordcount = value;
            }
        }
    }
}