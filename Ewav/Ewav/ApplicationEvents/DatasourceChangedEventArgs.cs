/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       DatasourceChangedEventArgs.cs
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
    public class DatasourceChangedEventArgs : EventArgs
    {
        private int dataSource;

        public DatasourceChangedEventArgs(int dataSource)
        {
            DataSource = dataSource;
        }  

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public int DataSource
        {
            get
            {
                return this.dataSource;
            }
            set
            {
                this.dataSource = value;
            }
        }

    }
}