/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       CrossTabResponseObject.cs
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
using System.Text;
using Ewav.Web.EpiDashboard;

namespace Ewav.Web.Services
{
    public class CrossTabResponseObjectDto
    {
        public CrossTabResponseObjectDto()
        {
            columnNames = new List<string>();
            dsList = new List<DescriptiveStatistics>();
        }

        private List<string> columnNames;

        public List<string> ColumnNames
        {
            get { return columnNames; }
            set { columnNames = value; }
        }

        private List<DescriptiveStatistics> dsList;

        public List<DescriptiveStatistics> DsList
        {
            get { return dsList; }
            set { dsList = value; }
        }
    }
}