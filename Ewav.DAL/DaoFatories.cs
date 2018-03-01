/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       DaoFatories.cs
 *  Namespace:  Ewav.DAL    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Linq;
using Ewav.DAL.Interfaces;
using Ewav.DAL.MySqlLayer;
using Ewav.DAL.SqlServer;
using Ewav.DTO;
using Ewav.DAL.PostgreSQL;

// -----------------------------------------------------------------------
// <copyright file="$safeitemrootname$.cs" company="$registeredorganization$">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
namespace Ewav.DAL
{
    public static class DaoFatories
    {
        /// <summary>
        /// Gets the factory.
        /// </summary>
        /// <param name="dataBaseTypeEnum">The data base type enum.</param>
        /// <param name="MetaDataConnectionString">The meta data connection string.</param>
        /// <param name="MetaDataViewName">Name of the meta data view.</param>
        /// <returns></returns>
        public static IDaoFactory GetFactory(DataBaseTypeEnum dataBaseTypeEnum, 
                    string MetaDataConnectionString, string MetaDataViewName)
        {
            switch (dataBaseTypeEnum)
            {
                case DataBaseTypeEnum.MySQL:
                    return new MySqlDaoFactory(MetaDataConnectionString, MetaDataViewName);


                case DataBaseTypeEnum.SQLServer:
                    return new SqlServerDaoFactory(MetaDataConnectionString, MetaDataViewName);

                case DataBaseTypeEnum.PostgreSQL:
                    return new PostgreSQLDaoFactory(MetaDataConnectionString, MetaDataViewName);
                    
                    // default:
                default:
                    throw new ApplicationException(string.Format("Database type {0} is not supported ", dataBaseTypeEnum.ToString()));
            }
        }

        /// <summary>
        /// Gets the factory.
        /// </summary>
        /// <param name="dataBaseTypeEnum">The data base type enum.</param>
        /// <returns></returns>
        public static IDaoFactory GetFactory(DataBaseTypeEnum dataBaseTypeEnum)
        {
            switch (dataBaseTypeEnum)
            {
                case DataBaseTypeEnum.MySQL:
                    return new MySqlDaoFactory();
                  
                case DataBaseTypeEnum.SQLServer:
                    return new SqlServerDaoFactory();
                case DataBaseTypeEnum.PostgreSQL:
                    return new PostgreSQLDaoFactory();
                    // default:
                default:
                    throw new ApplicationException(string.Format("Database type {0} is not supported ", dataBaseTypeEnum.ToString()));
            }
        }
    }
}