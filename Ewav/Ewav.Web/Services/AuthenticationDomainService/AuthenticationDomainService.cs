/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       AuthenticationDomainService.cs
 *  Namespace:  Ewav.Web.Services.AuthenticationDomainService    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using System.ServiceModel.DomainServices.Server.ApplicationServices;

namespace Ewav.Web.Services.AuthenticationDomainService
{
    [EnableClientAccess]
    public class AuthenticationDomainService : AuthenticationBase<User>
    {
        // To enable Forms/Windows Authentication for the Web Application, edit the appropriate section of web.config file.
    }

    public class User : UserBase
    {
        // NOTE: Profile properties can be added here 
        // To enable profiles, edit the appropriate section of web.config file.

        // public string MyProfileProperty { get; set; }
    }
}