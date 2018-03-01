/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       OrganizationDomainService.cs
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
    using Ewav.BAL;
    using Ewav.DTO;

    // This domain service is created for CRUD Operations on Organization Enity.
    [EnableClientAccess()]
    public class OrganizationDomainService : DomainService
    {
        EntityManager em = new EntityManager();

        /// <summary>
        /// Generates/Adds the organization
        /// </summary>
        /// <param name="dto"></param>
        public int AddOrganization(UserOrganizationDto dto)
        {
            //return em.AddOrganization(organizationDto, userDTO);
            return em.AddOrganization(dto);
        }

        /// <summary>
        /// Removes/Deletes the organization
        /// </summary>
        /// <param name="organzationId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool RemoveOrganization(int organzationId)
        {
            //Call BAL and Then DAL to soft delete the organzation from db.
            //Logged in user cannot delete himself.


            return em.RemoveOrganization(organzationId);
        }

        /// <summary>
        /// Updates/Modify organization
        /// </summary>
        /// <param name="dto"></param>
        public bool UpdateOrganization(OrganizationDto dto)
        {
            //Call BAL and Then DAL to update organization name to DB.
            em.UpdateOrganization(dto);
            return true;
        }

        /// <summary>
        /// Reads The organization(s)
        /// </summary>
        /// <param name="user"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public OrganizationDto ReadOrganization()
        {
            ////retrieve all the organizations
            //OrganizationDto organizationDto = em.ReadAllOrganizations();
            //return organizationDto;

            throw new Exception();    
        }

        public List<OrganizationDto> ReadAllOrganizations()
        {
            //retrieve all the organizations
            List<OrganizationDto> listOfOrganizations = em.ReadAllOrganizations();
            return listOfOrganizations;
        }
    }
}