/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MapClusterDomainService.cs
 *  Namespace:  Ewav.Web.Services.MapCluster    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
namespace Ewav.Web.Services.MapCluster
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using Ewav.DTO;
    using EpiDashboard;
    using Ewav.BAL;


    // TODO: Create methods containing your application logic.
    [EnableClientAccess()]
    public class MapClusterDomainService : DomainService
    {
        [Query(IsComposable = false)]
        public EwavRule_Base Getrule(int id)
        {
            return new EwavRule_Base();

        }

        [Invoke]
        public List<PointDTOCollection> GetMapValues(GadgetParameters gadgetParameters,
               IEnumerable<EwavDataFilterCondition> ewavDataFilters, List<EwavRule_Base> rules, string filterString = "")
        {



            try
            {
                MapManager mmanager = new MapManager();

                List<PointDTOCollection> MapList = mmanager.GetMapValues(gadgetParameters, ewavDataFilters, rules, filterString);

                return MapList;
            }
            catch (Exception e   )
            {
                throw new Exception(e.Message + "\n" + e.StackTrace);

            }

        }
    }
}