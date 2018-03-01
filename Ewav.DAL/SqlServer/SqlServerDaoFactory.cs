/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       SqlServerDaoFactory.cs
 *  Namespace:  Ewav.DAL.SqlServer    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Linq;
using Ewav.DAL.Interfaces;



namespace Ewav.DAL.SqlServer
{
    public class SqlServerDaoFactory : IDaoFactory
    {

        private readonly string MetaDataConnectionString;
        private readonly string MetaDataViewName;
        
        
        public SqlServerDaoFactory(string MetaDataConnectionString, string MetaDataViewName)
        {
            this.MetaDataConnectionString = MetaDataConnectionString;
            this.MetaDataViewName = MetaDataViewName;
        }

        public SqlServerDaoFactory(string MetaDataConnectionString)
        {
            this.MetaDataConnectionString = MetaDataConnectionString;
        }

        public SqlServerDaoFactory()
        {
            // TODO: Complete member initialization
        }

        public ICanvasDao CanvasDao
        {
            get
            {
                return new SqlServer.SqlServerCanvasDao();
            }
        }
        /// <summary>
        /// Gets or sets the meta data DAO.
        /// </summary>
        /// <value>The meta data DAO.</value>
        public IMetaDataDao MetaDataDao
        {
            get
            {
                if (this.MetaDataConnectionString != null && this.MetaDataViewName != null)
                {
                    return new SqlServerMetaDataDao(this.MetaDataConnectionString, this.MetaDataViewName);       //     return new SqlMetaDataDao();
                }
                else
                {
                    throw new ApplicationException();
                }
            }
        }
        public IMetaDataDao MetaDataDaoEmpty
        {
            get
            {
                return new SqlServerMetaDataDao();
            }
        }
        /// <summary>
        /// Gets or sets the meta data DAO.
        /// </summary>
        public IRawDataDao RawDataDao
        {
            get
            {
                return new SqlServerRawDataDao(this.MetaDataConnectionString, this.MetaDataViewName);
            }
        }
        /// <summary>
        /// Gets or sets the user DAO.
        /// </summary>
        /// <value>The user DAO.</value>
        public IUserDao UserDao
        {
            get
            {
                return new SqlServer.SqlServerUserDao();    
            }
        }


        public IOrganizationDao OrganizationDao
        {
            get { return new SqlServerOrganizationDao(); }
        }


        public IAdminDatasourceDao AdminDSDao
        {
            get { return new SqlServerDatasourceDao(); }
        }
    }
}