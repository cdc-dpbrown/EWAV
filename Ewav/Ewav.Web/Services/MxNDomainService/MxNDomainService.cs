/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MxNDomainService.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
namespace Ewav.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using EpiDashboard;
    using Ewav.BAL;

    // TODO: Create methods containing your application logic.
    [EnableClientAccess()]
    public class MxNDomainService : DomainService
    {
        private DashboardHelper dh;

        [Query(IsComposable = false)]
        public EwavRule_Base Getrule(int id)
        {
            return new EwavRule_Base();

        }


        private TwoxTwoAndMxNResultsSet resultSet;
        /// <summary>
        /// Get a list of columns    
        /// </summary>
        /// <param name="DataSourceName"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public List<EwavColumn> GetColumns(string DataSourceName, string TableName)
        {
            return BAL.Common.GetColumns(DataSourceName, TableName);
        }


            [Invoke]  
        public TwoxTwoAndMxNResultsSet SetupGadget(GadgetParameters clientGadgetOptions, List<EwavDataFilterCondition> ewavDataFilters, List<EwavRule_Base>   rules)
        {
            TwoxTwoManager twoxTwoManager = new TwoxTwoManager();
            TwoxTwoAndMxNResultsSet twoxTwoResultsSet = twoxTwoManager.SetupGadget(clientGadgetOptions, ewavDataFilters, rules);
            return twoxTwoResultsSet;
        
            
            
            
            
            
            
            
            
            }
    }
}