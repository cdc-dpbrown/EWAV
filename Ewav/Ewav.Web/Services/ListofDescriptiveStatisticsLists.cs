/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ListofDescriptiveStatisticsLists.cs
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
using Ewav.Web.EpiDashboard;

namespace Ewav.Web.Services  
{

    public class ListofDescriptiveStatistics
    {
        private List<DescriptiveStatistics> descriptiveStatisticsList;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListofDescriptiveStatistics" /> class.
        /// </summary>
        public ListofDescriptiveStatistics(List<DescriptiveStatistics> descStatsList)
        {
        }

        /// <summary>
        /// Gets or sets the descriptive statistics list.
        /// </summary>
        /// <value>The descriptive statistics list.</value>
        public List<DescriptiveStatistics> DescriptiveStatisticsList
        {
            get
            {
                return this.descriptiveStatisticsList;
            }
            set
            {
                this.descriptiveStatisticsList = value;
            }
        }
    }
}