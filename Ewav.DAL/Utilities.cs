/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       Utilities.cs
 *  Namespace:  Ewav.DAL    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
namespace Ewav.DAL
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using Ewav.DTO;
    using Ewav.Security;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class Utilities
    {
        public static string CreateConnectionString(DataBaseTypeEnum externalDataBaseType, DataView dv)
        {
            string extConnectionString;

            try
            {
                switch (externalDataBaseType)
                {
                    case DataBaseTypeEnum.MySQL:

                        extConnectionString =
                            string.Format("Server={0};Database={1};User Id={2};Pwd={3};Port={4}",
                                decrypt(dv[0]["DatasourceServerName"].ToString()),
                                decrypt(dv[0]["InitialCatalog"].ToString()),
                                decrypt(dv[0]["DatabaseUserID"].ToString()),
                                decrypt(dv[0]["Password"].ToString()),
                                decrypt(dv[0]["PortNumber"].ToString()));                        //     shorterd01"     

                        break;
                    case DataBaseTypeEnum.SQLServer:
                        
                        if (decrypt(dv[0]["PortNumber"].ToString()).ToString().Trim().Length > 0)
                        {
                            extConnectionString =
                            string.Format("Data Source={0},{5};Initial Catalog={1};Persist Security Info={2};User ID={3};Password={4};",
                                decrypt(dv[0]["DatasourceServerName"].ToString()),
                                decrypt(dv[0]["InitialCatalog"].ToString()),
                                dv[0]["PersistSecurityInfo"].ToString(),
                                decrypt(dv[0]["DatabaseUserID"].ToString()),
                                decrypt(dv[0]["Password"].ToString()),
                                decrypt(dv[0]["PortNumber"].ToString()));
                        }
                        else
                        {
                            extConnectionString =
                            string.Format("Data Source={0};Initial Catalog={1};Persist Security Info={2};User ID={3};Password={4};",
                                decrypt(dv[0]["DatasourceServerName"].ToString()),
                                decrypt(dv[0]["InitialCatalog"].ToString()),
                                dv[0]["PersistSecurityInfo"].ToString(),
                                decrypt(dv[0]["DatabaseUserID"].ToString()),
                                decrypt(dv[0]["Password"].ToString()));
                        }

                        break;

                    case DataBaseTypeEnum.PostgreSQL:


                        extConnectionString =
                            string.Format("Server={0};Database={1};User Id={2};Password={3}; Port = {4};",
                                decrypt(dv[0]["DatasourceServerName"].ToString()),
                                decrypt(dv[0]["InitialCatalog"].ToString()),
                                decrypt(dv[0]["DatabaseUserID"].ToString()),
                                decrypt(dv[0]["Password"].ToString()),
                                decrypt(dv[0]["PortNumber"].ToString())
                                );

                        break;
                    default:
                        extConnectionString = "";

                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return extConnectionString;
        }

        public static string CreateConnectionString(DataBaseTypeEnum externalDataBaseType, DataRow[] dr)
        {
            string extConnectionString;

            try
            {
                switch (externalDataBaseType)
                {
                    case DataBaseTypeEnum.MySQL:

                        extConnectionString =
                            string.Format("Server={0};Database={1};User Id={2};Pwd={3};Port={4}",
                                decrypt(dr[0]["DatasourceServerName"].ToString()),
                                decrypt(dr[0]["InitialCatalog"].ToString()),
                                decrypt(dr[0]["DatabaseUserID"].ToString()),
                                decrypt(dr[0]["Password"].ToString()),
                                decrypt(dr[0]["PortNumber"].ToString()));                        //     shorterd01"     

                        break;

                    case DataBaseTypeEnum.SQLServer:


                        

                        if (decrypt(dr[0]["PortNumber"].ToString()).ToString().Trim().Length > 0)
                        {
                            extConnectionString =
                            string.Format("Data Source={0},{5};Initial Catalog={1};Persist Security Info={2};User ID={3};Password={4};",
                                decrypt(dr[0]["DatasourceServerName"].ToString()),
                                decrypt(dr[0]["InitialCatalog"].ToString()),
                                dr[0]["PersistSecurityInfo"].ToString(),
                                decrypt(dr[0]["DatabaseUserID"].ToString()),
                                decrypt(dr[0]["Password"].ToString()),
                                decrypt(dr[0]["PortNumber"].ToString()));
                        }
                        else
                        {
                            extConnectionString =
                            string.Format("Data Source={0};Initial Catalog={1};Persist Security Info={2};User ID={3};Password={4};",
                                decrypt(dr[0]["DatasourceServerName"].ToString()),
                                decrypt(dr[0]["InitialCatalog"].ToString()),
                                dr[0]["PersistSecurityInfo"].ToString(),
                                decrypt(dr[0]["DatabaseUserID"].ToString()),
                                decrypt(dr[0]["Password"].ToString()));
                        }

                        break;

                    case DataBaseTypeEnum.PostgreSQL:


                        extConnectionString =
                            string.Format("Server={0};Database={1};User Id={2};Password={3}; Port = {4};",
                                decrypt(dr[0]["DatasourceServerName"].ToString()),
                                decrypt(dr[0]["InitialCatalog"].ToString()),
                                decrypt(dr[0]["DatabaseUserID"].ToString()),
                                decrypt(dr[0]["Password"].ToString()),
                                decrypt(dr[0]["PortNumber"].ToString())
                                );                        //     shorterd01"     

                        break;
                    default:
                        extConnectionString = "";

                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return extConnectionString;
        }

        /// <summary>
        /// Decrypts the specified dr.
        /// </summary>
        /// <param name="dr">The dr.</param>
        private static string decrypt(string dr)
        {
            Cryptography cy = new Cryptography();
            string s = cy.Decrypt(dr);
            return s;
        }
    }
}