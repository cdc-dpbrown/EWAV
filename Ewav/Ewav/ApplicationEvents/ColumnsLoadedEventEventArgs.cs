/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ColumnsLoadedEventEventArgs.cs
 *  Namespace:  Ewav.Client.Application    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using Ewav.BAL;

namespace Ewav.Client.Application
{
    public class ColumnsLoadedEventEventArgs : EventArgs
    {
        private List<EwavColumn> columnList;
  
        public ColumnsLoadedEventEventArgs(List<EwavColumn> cols)
        {
            ColumnList = cols;
        }

        public List<EwavColumn> ColumnList
        {
            get
            {
                return columnList;
            }
            set
            {
                columnList = value;
            }
        }
    }
}