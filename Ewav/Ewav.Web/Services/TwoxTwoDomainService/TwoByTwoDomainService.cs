/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       TwoByTwoDomainService.cs
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
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using EpiDashboard;
    using Ewav.BAL;

    // TODO: Create methods containing your application logic.
    [EnableClientAccess()]
    public partial class TwoByTwoDomainService : DomainService
    {

        [Query(IsComposable = false)]
        public EwavRule_Base Getrule(int id)
        {
            return new EwavRule_Base();

        }

        private DashboardHelper dh;
        private TwoxTwoAndMxNResultsSet resultSet;
        private TwoxTwoTableDTO twoxTwoTableDTO;

        public TwoxTwoTableDTO TwoxTwoTableDto
        {
            get
            {
                return this.twoxTwoTableDTO;
            }
            set
            {
                this.twoxTwoTableDTO = value;
            }
        }

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

        //      public void PortThistoClient(StringLiterals sl) { }
        public void PortClassToClient4(TwoxTwoTableDTO gp)
        {
        }

        public void PortClassToClient77(TextBlockConfig gp)
        {
        }

        [Invoke]
        public TwoxTwoAndMxNResultsSet SetupGadget(GadgetParameters clientGadgetOptions,
            List<EwavDataFilterCondition> ewavDataFilters,
            List<EwavRule_Base >   rules                            ) 
        {
            TwoxTwoManager twoxTwoManager = new TwoxTwoManager();
            TwoxTwoAndMxNResultsSet twoxTwoResultsSet = twoxTwoManager.SetupGadget(clientGadgetOptions, ewavDataFilters, rules);
            return twoxTwoResultsSet;
        }
        public void PortClassToClient441(DatatableBag dtc)
        {

        }

    }
}