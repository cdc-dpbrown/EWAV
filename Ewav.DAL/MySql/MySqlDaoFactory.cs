/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MySqlDaoFactory.cs
 *  Namespace:  Ewav.DAL.MySqlLayer    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Linq;
using Ewav.DAL.Interfaces;

namespace Ewav.DAL.MySqlLayer
{
    public class MySqlDaoFactory : IDaoFactory    
    {
        private readonly string MetaDataConnectionString;
        private readonly string MetaDataViewName;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlDaoFactory" /> class.
        /// </summary>
        /// <param name="rawDataDao">The raw data DAO.</param>
        public MySqlDaoFactory(IRawDataDao rawDataDao)
        {
            this.RawDataDao = rawDataDao;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlDaoFactory" /> class.
        /// </summary>
        /// <param name="ConnectionString">The meta data connection string.</param>
        public MySqlDaoFactory(string MetaDataConnectionString)     
        {
            this.MetaDataConnectionString = MetaDataConnectionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlDaoFactory" /> class.
        /// </summary>
        /// <param name="ConnectionString">The meta data connection string.</param>
        /// <param name="MetaDataViewName">Name of the meta data view.</param>
        public MySqlDaoFactory(string MetaDataConnectionString, string MetaDataViewName)
        {
            // TODO: Complete member initialization
            this.MetaDataConnectionString = MetaDataConnectionString;
            this.MetaDataViewName = MetaDataViewName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlDaoFactory" /> class.
        /// </summary>
        public MySqlDaoFactory()
        {
            // TODO: Complete member initialization
        }

        /// <summary>
        /// Gets the admin DS DAO.
        /// </summary>
        /// <value>The admin DS DAO.</value>
        public IAdminDatasourceDao AdminDSDao
        {
            get
            {
                return new MySqlDatasourceDao();
            }
        }

        /// <summary>
        /// Gets the canvas DAO.
        /// </summary>
        /// <value>The canvas DAO.</value>
        public ICanvasDao CanvasDao
        {
            get
            {
                return new MySqlCanvasDao();
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
                return new MySqlMetaDataDao(this.MetaDataConnectionString, this.MetaDataViewName);                 
            }
            set
            {
                // TODO: Implement this property setter
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the meta data DAO empty.
        /// </summary>
        /// <value>The meta data DAO empty.</value>
        public IMetaDataDao MetaDataDaoEmpty
        {
            get
            {
                // TODO: Implement this property getter
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the organization DAO.
        /// </summary>
        /// <value>The organization DAO.</value>
        public IOrganizationDao OrganizationDao
        {
            get
            {
                return new MySqlOrganizationDao();
            }
        }

        /// <summary>
        /// Gets or sets the raw data DAO.
        /// </summary>
        /// <value>The raw data DAO.</value>
        public IRawDataDao RawDataDao
        {
            get
            {
                return new MySqlRawDataDao(this.MetaDataConnectionString, this.MetaDataViewName);  
            }
            set
            {
                // TODO: Implement this property setter
                throw new NotImplementedException();
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
                return new MySqlUserDao();    
            }
        }
    }
}