/*  ----------------------------------------------------------------------------
*  Emergint Technologies, Inc.
*  ----------------------------------------------------------------------------
*  Epi Info™ - Web Analysis & Visualization
*  ----------------------------------------------------------------------------
*  File:       CanvasDomainService.cs
*  Namespace:  Ewav.Web.Services.CanvasDomainService    
*
*  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
*  Created:    27/05/2014    
*  Summary:	no summary     
*  ----------------------------------------------------------------------------
*/

namespace Ewav.Web.Services.CanvasDomainService
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using Ewav.BAL;
    using Ewav.DTO;

    // TODO: Create methods containing your application logic.
    [EnableClientAccess()]
    public class CanvasDomainService : DomainService
    {
        readonly CanvasManager cm = new CanvasManager();

        /// <summary>
        /// Saves Canvas
        /// </summary>
        /// <param name="canvasDto"></param>
        /// <returns></returns>
        public int SaveCanvas(CanvasDto canvasDto)
        {
            return cm.SaveCanvas(canvasDto);
            // return false;
        }

        /// <summary>
        /// Loads Canvas List for User
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public DatatableBag LoadCanvasListForUser(int UserId)
        {
            DataSet ds = cm.LoadCanvasListForUser(UserId);
            DatatableBag dtb = new DatatableBag();
            return dtb.ConvertToDatatableBag(ds.Tables[0]);
        }

        public DataSet LoadCanvasListForUser(string UserId)
        {
            DataSet ds = cm.LoadCanvasListForUser(Int32.Parse(UserId));

            return ds;
        }


        public DatatableBag ReadCanvasListForLite(string FormId, int UserId)
        {
            DataSet ds = cm.ReadCanvasListForLite(FormId, UserId);
            DatatableBag dtb = new DatatableBag();
            return dtb.ConvertToDatatableBag(ds.Tables[0]);
        }

        /// <summary>
        /// Loads selected Canvas
        /// </summary>
        /// <param name="canvasId"></param>
        /// <returns></returns>
        public CanvasDto LoadCanvas(int canvasId)
        {
            return cm.LoadCanvas(canvasId);
        }

        public CanvasDto LoadCanvasGUID(Guid canvasGUID)
        {
            return cm.LoadCanvas(canvasGUID);
        }

        public List<CanvasShareStatusDto> GetCanvasShareStatus(int canvasID, int organizationID)
        {
            EntityManager em = new EntityManager();

            return em.GetCanvasShareStatus(canvasID, organizationID);
        }


        public List<CanvasShareStatusDto> GetCanvasShareStatusGuid(string canvasGUID)
        {


            EntityManager em = new EntityManager();

            return em.GetCanvasShareStatus(canvasGUID);


        }


        public string SaveCanvasSnapshot(CanvasDto cdto)
        {
            return cm.SaveCanvasSnapshot(cdto);
        }

        /// <summary>
        /// Shares canvas
        /// </summary>
        /// <param name="canvasId"></param>
        /// <param name="SharedUserIdList"></param>
        public bool ShareCanvas(int canvasId, List<int> sharedUserIds)
        {
            cm.ShareCanvas(canvasId, sharedUserIds);
            return true;
        }

        /// <summary>
        /// Deletes canvas
        /// </summary>
        /// <param name="canvasId"></param>
        /// <returns></returns>
        public bool DeleteCanvas(int canvasId)
        {
            cm.DeleteCanvas(canvasId);
            return false;
        }

        /// <summary>
        /// Reads All the users in my org
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public DatatableBag ReadAllUsersInMyOrg(int orgId)
        {
            DataSet ds = cm.ReadAllUsersInMyOrg(orgId);
            DatatableBag dtb = new DatatableBag();
            return dtb.ConvertToDatatableBag(ds.Tables[0]);
        }

        /// <summary>
        /// Exports the Data in to DatatableBag
        /// </summary>
        /// <param name="dsName"></param>
        /// <param name="tableName"></param>
        /// <param name="ewavDataFilters"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        public DatatableBag ExportData(string dsName, string tableName,
            IEnumerable<EwavDataFilterCondition> ewavDataFilters, List<EwavRule_Base> rules)
        {
            DatatableBag dtb = new DatatableBag();
            EpiDashboard.GadgetParameters gp = new EpiDashboard.GadgetParameters();
            gp.DatasourceName = dsName;
            gp.TableName = tableName;
            EpiDashboard.DashboardHelper dbHelper = new EpiDashboard.DashboardHelper(gp, ewavDataFilters, rules);
            dbHelper.PopulateDataSet();
            DataView dv = dbHelper.DataSet.Tables[0].DefaultView;
            ///Write code here
            return dtb;
        }

        /// <summary>
        /// Resends the email to selected shared users.
        /// </summary>
        /// <param name="canvasId"></param>
        /// <param name="SharedUserIds"></param>
        /// <returns></returns>
        public bool ResendEmail(int canvasId, List<int> SharedUserIds)
        {
            return cm.ResendEmail(canvasId, SharedUserIds);
        }
    }
}