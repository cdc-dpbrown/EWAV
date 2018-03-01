/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       IMapControlControlServiceAgent.cs
 *  Namespace:  Ewav.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;

using Ewav.ViewModels;
using Ewav.Web.Services;
using Ewav.DTO;

namespace Ewav.Services
{
    public interface IMapControlControlServiceAgent
    {


        void LoadMapData(Ewav.Web.EpiDashboard.GadgetParameters gp, Action<List<PointDTOCollection >, Exception> completed);

     

    }
}