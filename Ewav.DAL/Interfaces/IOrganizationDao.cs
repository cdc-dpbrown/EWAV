/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       IOrganizationDao.cs
 *  Namespace:  Ewav.DAL.Interfaces    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ewav;
using System.Data;
using Ewav.DTO;

namespace Ewav.DAL.Interfaces
{
    public interface IOrganizationDao
    {
        string ConnectionString { get; set; }

        string TableName { get; set; }

        int AddOrganization(UserOrganizationDto organizationDto);

        bool UpdateOrganization(OrganizationDto dto);

        bool RemoveOrganization(int organzationId);

        DataSet ReadOrganization(int organizationID);          

        DataSet ReadAllOrganizations();  

    }
}