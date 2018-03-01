/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       PostgreSQLDaoFactory.cs
 *  Namespace:  Ewav.DAL.PostgreSQL    
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
using Ewav.DAL.Interfaces;

namespace Ewav.DAL.PostgreSQL
{
    public class PostgreSQLDaoFactory : IDaoFactory
    {
        private readonly string MetaDataConnectionString;
        private readonly string MetaDataViewName;

        public PostgreSQLDaoFactory(string MetaDataConnectionString, string MetaDataViewName)
        {
            this.MetaDataConnectionString = MetaDataConnectionString;
            this.MetaDataViewName = MetaDataViewName;
        }

        public PostgreSQLDaoFactory(string MetaDataConnectionString)
        {
            this.MetaDataConnectionString = MetaDataConnectionString;
        }

        public PostgreSQLDaoFactory()
        {

        }


        public ICanvasDao CanvasDao
        {
            get { return new PostgreSQLCanvasDao(); }
        }

        public IMetaDataDao MetaDataDao
        {
            get
            {
                if (this.MetaDataConnectionString != null && this.MetaDataViewName != null)
                {
                    return new PostgreSQLMetaDataDao(this.MetaDataConnectionString, this.MetaDataViewName);       //     return new SqlMetaDataDao();
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

                return new PostgreSQLMetaDataDao();       //     return new SqlMetaDataDao();

            }
        }

        public IRawDataDao RawDataDao
        {
            get
            {
                return new PostgreSQLRawDataDao(this.MetaDataConnectionString, this.MetaDataViewName);
            }
        }

        public IUserDao UserDao
        {
            get { return new PostgreSQLUserDao(); }
        }

        public IOrganizationDao OrganizationDao
        {
            get { return new PostgreSQLOrganizationDao(); }
        }

        public IAdminDatasourceDao AdminDSDao
        {
            get { return new PostgreSQLDatasourceDao(); }
        }
    }
}