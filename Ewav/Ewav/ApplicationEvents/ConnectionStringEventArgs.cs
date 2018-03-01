/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ConnectionStringEventArgs.cs
 *  Namespace:  Ewav.Client.Application    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using Ewav.Web.Services;
using Ewav.BAL;

namespace Ewav.Client.Application
{
    public class ConnectionStringEventArgs : EventArgs
    {
        private EwavDatasourceDto ewavConnectionInfo;

        private string tableName;

        private string datasourceName;

        public ConnectionStringEventArgs(EwavDatasourceDto  ewc)
        {
            ewavConnectionInfo = ewc;
        }

        /// <summary>
        /// Gets or sets the ewav connection info.
        /// </summary>
        /// <value>The ewav connection info.</value>

        public EwavDatasourceDto EwavConnectionInfo
        {
            get
            {
                return this.ewavConnectionInfo;
            }

        }
    }
}