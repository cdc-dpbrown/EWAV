/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       DiagnosticsDomainService.cs
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
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using Ewav.BAL;
    using Ewav.DTO;
    using System.Data;
    using Ewav.Security;


    // TODO: Create methods containing your application logic.
    [EnableClientAccess()]
    public class DiagnosticsDomainService : DomainService
    {

        //  service endpoint test (ping ) 
        //  user test 

        public bool UserCheck(string userName)
        {

            EntityManager em = new EntityManager();

            try
            {
                UserDTO uDto = em.LoadUser(userName);
                return true;
            }
            catch (Exception ex)
            {

                return false;


            }

        }


        public bool ServiceEndpointCheck()
        {

            return true;


        }


        public Dictionary<string, bool> CheckDatasource(string datasourceName)
        {

            DataTable dtRawData;

            EntityManager em = new EntityManager();

            DataTable dtAllDatasources = em.GetAllDatasources("*");
            DataRow[] dr = dtAllDatasources.Select("DataSourceName =  '" + datasourceName + "'");

            Dictionary<string, bool> resultDict = new Dictionary<string, bool>();

            string dataTableName = dr[0][10].ToString();


            Cryptography cy = new Cryptography();

            try
            {
                dtRawData = em.GetRawDataTable(datasourceName, cy.Decrypt(dataTableName));
                resultDict.Add(datasourceName, true);
            }
            catch (Exception ex)
            {
                resultDict.Add(datasourceName, false);
            }



            return resultDict;

        }


        [Invoke]
        public List<string> GetAllDatasourceNames()
        {

            EntityManager em = new EntityManager();

            DataTable dt = em.GetAllDatasources("*");

            DataTable dt2 = dt.DefaultView.ToTable(true, "DatasourceName");

            List<string> s = dt2.AsEnumerable().Select(x => x["DatasourceName"].ToString()).ToList();

            return s;
        }

        [Invoke]
        public string CheckAllDatasources()
        {
            string resultStr = "";
            DataTable dtRawData;

            try
            {
                EntityManager em = new EntityManager();
                DataTable dtAllDatasources = em.GetAllDatasources("*");

                // Go through each data source  
                foreach (DataRow dr in dtAllDatasources.Rows)
                {
                    string datasourceName = dr["DatasourceName"].ToString();
                    string dataTableName = dr["DatabaseObject"].ToString();
                    //  Test 3 - Get all cols for this data source   
                    List<EwavColumn> columnList = em.GetColumnsForDatasource(datasourceName);
                    if (columnList.Count == 0)
                        throw new Exception("Problem with datasource " + datasourceName);
                    // Test 4 -  Get raw data    
                    Cryptography cy = new Cryptography();

                    try
                    {
                        dtRawData = em.GetRawDataTable(datasourceName, cy.Decrypt(dataTableName));
                    }
                    catch (Exception ex)
                    {
                        resultStr += datasourceName + " ";
                    }
                }
            }
            catch (Exception ex)
            {



            }

            if (resultStr == "")
                resultStr = "No connection errors";

            return resultStr;
        }
    }
}