/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       IDaoFactory.cs
 *  Namespace:  Ewav.DAL.Interfaces    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;

namespace Ewav.DAL.Interfaces
{
    public interface IDaoFactory
    {
        ICanvasDao CanvasDao { get; }
        IMetaDataDao MetaDataDao { get; }
        IMetaDataDao MetaDataDaoEmpty { get; }
        IRawDataDao RawDataDao { get; }
        /// <summary>
        /// Gets or sets the user DAO.
        /// </summary>
        /// <value>The user DAO.</value>
        IUserDao UserDao { get; }
        IOrganizationDao OrganizationDao { get; }
        IAdminDatasourceDao AdminDSDao { get; }
    }
}