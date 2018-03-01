/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       AdminDatasourcesDomainService.cs
 *  Namespace:  Ewav.Web.Services.AdminDatasourcesDomainService    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
namespace Ewav.Web.Services.AdminDatasourcesDomainService
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using Ewav.DTO;
    using Ewav.BAL;


    // TODO: Create methods containing your application logic.
    [EnableClientAccess()]
    public class AdminDatasourcesDomainService : DomainService
    {
        EntityManager em = new EntityManager();

        /// <summary>
        /// Tests the DB connection.
        /// </summary>
        /// <param name="connInfo">The conn info.</param>
        /// <returns></returns>
        public bool TestDBConnection(Connection connInfo)
        {
            return em.TestDBConnection(connInfo);
        }

        /// <summary>
        /// Tests the data.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <returns></returns>
        public bool TestData(Connection connectionInfo)
        {
            return em.TestData(connectionInfo);
        }

        /// <summary>
        /// Adds the datasource.
        /// </summary>
        /// <param name="ds">The ds.</param>
        /// <returns></returns>
        public bool AddDatasource(DatasourceDto ds)
        {
            return em.AddDatasouce(ds);
        }

        /// <summary>
        /// Updates the datasource.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns></returns>
        public bool UpdateDatasource(DatasourceDto dto)
        {
            return em.UpdateDatasource(dto);
        }

        /// <summary>
        /// Removes the datasource.
        /// </summary>
        /// <param name="dsId">The ds id.</param>
        /// <returns></returns>
        public bool RemoveDatasource(int dsId)
        {
            return em.RemoveDatasource(dsId);
        }

        /// <summary>
        /// Reads the datasource.
        /// </summary>
        /// <returns></returns>
        public List<DatasourceDto> ReadDatasource(int orgId)
        {

            try
            {
                return em.ReadDatasource(orgId);


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " -- " + ex.StackTrace);
            }

        }


        /// <summary>
        /// Reads the associated users.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public List<UserDTO> ReadAssociatedUsers(int dsId, int orgId)
        {
            try
            {
                return em.ReadAssociatedUsers(dsId, orgId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " -- " + ex.StackTrace);
            }

        }

        public IEnumerable<DatasourceUserDto> GetAllDatasourceUser()
        {
            try
            {

                EntityManager em = new EntityManager();
                List<DatasourceUserDto> datasourceUserDtoList = em.GetAllDatasourceUser();


                return datasourceUserDtoList as IEnumerable<DatasourceUserDto>;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " -- " + ex.StackTrace);
            }

        }

        /// <summary>
        /// Copies the saved dashboard associating it to new Datasource
        /// </summary>
        /// <param name="OldCanvasName"></param>
        /// <param name="NewCanvasName"></param>
        /// <param name="UserId"></param>
        /// <param name="NewDatasourceId"></param>
        /// <returns></returns>
        public string CopyDashboard(string OldCanvasName, string NewCanvasName,
            string OldDatasourceName, string NewDatasourceName)
        {
            try
            {
                return em.CopyDashboard(OldCanvasName, NewCanvasName, OldDatasourceName, NewDatasourceName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " -- " + ex.StackTrace);
            }
        }

        public string ReadEWEDatasourceFormId(EWEDatasourceDto EWEDsDto) 
        {
            try
            {
                return em.ReadEWEDatasourceFormId(EWEDsDto);
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        //public int ReadEwavDatasource(Guid DatasourceId)
        //{
        //    try
        //    {
        //        return em.ReadEwavDatasource(DatasourceId);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}
    }
}