/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       IOrganizationServiceAgent.cs
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
using Ewav.Web.Services;
using Ewav.DTO;

namespace Ewav.Services
{
    public interface IOrganizationServiceAgent    
    {
        /// <summary>
        /// Reads all.
        /// </summary>
        /// <param name="readAllCompleted">The read all completed.</param>
        void ReadAll();

        void Add(UserOrganizationDto dto, Action<int, Exception> completed);

        void Update(OrganizationDto dto, Action<bool, Exception> completed);

        void Delete(int organizationId, Action<bool, Exception> completed);

        void Read(int organizationId, Action<OrganizationDto, Exception> completed);
    }
}