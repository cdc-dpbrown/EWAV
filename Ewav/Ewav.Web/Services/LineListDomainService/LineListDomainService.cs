/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       LineListDomainService.cs
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
    using Ewav.DTO;

    using System.Data;


    // TODO: Create methods containing your application logic.
    [EnableClientAccess()]
    public class LineListDomainService : DomainService
    {
        [Query(IsComposable = false)]
        public EwavRule_Base Getrule(int id)
        {
            return new EwavRule_Base();

        }

        [Invoke]
        public List<DatatableBag> GetLineList(GadgetParameters gadgetParameters,
               IEnumerable<EwavDataFilterCondition> ewavDataFilters, List<EwavRule_Base> rules, string filterString = "")
        {



            LineListManager llm = new LineListManager();

            List<DatatableBag> datatableBagList = llm.GetLineList(gadgetParameters, ewavDataFilters, rules, filterString);


            return datatableBagList;

        }


    }
}