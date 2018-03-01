/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       DatasourceDomainService.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using Ewav.BAL;
using Ewav.DTO;
using Ewav.Web.EpiDashboard;
using System;
using System.Reflection;
using System.Data;

namespace Ewav.Web.Services
{
    [EnableClientAccess()]
    public class DatasourceDomainService : DomainService
    {

        [Query(IsComposable = false)]
        public EwavRule_Base Getrule(int id)
        {
            return new EwavRule_Base();

        }


        public void PortToClient(EwavRuleType d) { }



        public List<EwavColumn> GetColumnsForDatasource(string datasourceName)
        {
            try
            {

                EntityManager em = new EntityManager();
                List<EwavColumn> allCols = em.GetColumnsForDatasource(datasourceName);

                return allCols;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " -- " + ex.StackTrace);
            }
        }

        /// <summary>
        /// Gets the datasources as I enumerble2.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EwavDatasourceDto> GetDatasourcesAsIEnumerble2(string userName)
        {
            try
            {
                EwavDatasourceListManager datasourceList = new EwavDatasourceListManager();

                IEnumerable<EwavDatasourceDto> result = datasourceList.GetDatasourcesAsIEnumerable2(userName);

                return result;
            }
            catch (Exception ex)
            {



                throw new Exception("Error with GetDatasourcesAsIEnumerble2 (top level ) ==== " + ex.Message + ex.StackTrace);


            }
        }





        //public IEnumerable<DatasourceUserDto> GetAllDatasourceUser()
        //{
        //    try
        //    {

        //        EntityManager em = new EntityManager();
        //        List<DatasourceUserDto> datasourceUserDtoList = em.GetAllDatasourceUser();


        //        return datasourceUserDtoList as IEnumerable<DatasourceUserDto>;


        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message + " -- " + ex.StackTrace);
        //    }

        //}

        /// <summary>
        /// Gets the Record Count.
        /// </summary>
        /// <returns></returns>
        [Invoke]
        public string GetRecordCount(List<EwavDataFilterCondition> filterList, List<EwavRule_Base> rules, string tableName, string dsName)
        {

            Assembly assembly = Assembly.GetExecutingAssembly();
            String version = assembly.FullName.Split(',')[1];
            String fullversion = version.Split('=')[1];

            try
            {
                string result;

                if (filterList.Count == 0 && rules.Count == 0)
                {
                    EntityManager em = new EntityManager();
                    int recordCount = em.GetRawDataTableRecordCount(dsName, tableName);

                    result = recordCount.ToString() + "," + recordCount.ToString();
                }
                else
                {
                    EpiDashboard.GadgetParameters inputs = new EpiDashboard.GadgetParameters();
                    inputs.TableName = tableName;
                    inputs.DatasourceName = dsName;
                    EpiDashboard.DashboardHelper dashboardHelper = new EpiDashboard.DashboardHelper(inputs, filterList, rules);
                    dashboardHelper.EwavConstructTableColumnNames(inputs);
                    dashboardHelper.CreateDataFilters(filterList);
                    dashboardHelper.GenerateView(inputs);


                    string totalRecords = dashboardHelper.DataSet.Tables[0].Rows.Count.ToString();
                    string filteredRecords = dashboardHelper.DataSet.Tables[0].DefaultView.Count.ToString();

                    result = totalRecords + "," + filteredRecords;
                }

                return result;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " -- " + ex.StackTrace);
            }


        }

        [Invoke]
        public string GetRecordCountByString(List<EwavRule_Base> rules, string s, string tableName, string dsName)
        {
            try
            {
                EpiDashboard.GadgetParameters inputs = new EpiDashboard.GadgetParameters();
                inputs.TableName = tableName;
                inputs.DatasourceName = dsName;
                EpiDashboard.DashboardHelper dashboardHelper = new EpiDashboard.DashboardHelper(inputs, s, rules);
                dashboardHelper.UseAdvancedUserDataFilter = true;
                dashboardHelper.AdvancedUserDataFilter = s;

                //EntityManager em = new EntityManager();

                //return em.GetRecordsCount(dsName, tableName, s);
                dashboardHelper.EwavConstructTableColumnNames(inputs);
                dashboardHelper.GenerateView(inputs);

                string totalRecords = dashboardHelper.DataSet.Tables[0].Rows.Count.ToString();
                string filteredRecords = dashboardHelper.DataSet.Tables[0].DefaultView.Count.ToString();

                string result = totalRecords + "," + filteredRecords;

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " -- " + ex.StackTrace);
            }


        }

        /// <summary>
        /// Reads the filter string
        /// </summary>
        /// <param name="filterList"></param>
        /// <param name="dsName"></param>
        /// <returns></returns>
        [Invoke]
        public string ReadFilterString(List<EwavDataFilterCondition> filterList, List<EwavRule_Base> rules, string tableName, string dsName)
        {
            try
            {
                EpiDashboard.GadgetParameters inputs = new EpiDashboard.GadgetParameters();
                inputs.TableName = tableName;
                inputs.DatasourceName = dsName;
                EpiDashboard.DashboardHelper dashboardHelper = new EpiDashboard.DashboardHelper(inputs, filterList, rules);
                dashboardHelper.EwavConstructTableColumnNames(inputs);
                dashboardHelper.CreateDataFilters(filterList);

                return dashboardHelper.GenerateDataFilterString();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " -- " + ex.StackTrace);
            }

        }


    }
}