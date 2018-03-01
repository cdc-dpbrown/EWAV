/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       CanvasManager.cs
 *  Namespace:  Ewav.Web.Services.CanvasDomainService    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ewav.BAL;
using System.Data;
using Ewav.DTO;

namespace Ewav.Web.Services.CanvasDomainService
{
    public class CanvasManager
    {
        EntityManager em = null;
        public CanvasManager()
        {
            em = new EntityManager();
        }


        /// <summary>
        /// Saves the canvas snapshot.
        /// </summary>
        /// <param name="cdto">The cdto.</param>
        public string SaveCanvasSnapshot(CanvasDto cdto)
        {

            return em.SaveCanvasSnapshot(cdto);

            // TODO: Implement this method
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the canvas.
        /// </summary>
        /// <param name="canvasDto">The canvas dto.</param>
        public int SaveCanvas(CanvasDto canvasDto)
        {
            return em.SaveCanvas(canvasDto);
        }
        /// <summary>
        /// Loads Canvas List for User
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public DataSet LoadCanvasListForUser(int UserId)
        {
            return em.LoadCanvasListForUser(UserId);
        }

        public DataSet ReadCanvasListForLite(string FormId, int UserId)
        {
            return em.ReadCanvasListForLite(FormId, UserId);
        }



        /// <summary>
        /// Loads Canvas
        /// </summary>
        /// <param name="canvasId"></param>
        /// <returns></returns>
        public CanvasDto LoadCanvas(int canvasId)
        {
            return em.LoadCanvas(canvasId);
        }

        /// <summary>
        /// Load canvas with GUID canvasid
        /// </summary>
        /// <param name="canvasGUID"></param>
        /// <returns></returns>
        public CanvasDto LoadCanvas(Guid canvasGUID)    
        {
            return em.LoadCanvas(canvasGUID);
        }



        /// <summary>
        /// Shares the canvas
        /// </summary>
        /// <param name="canvasId"></param>
        /// <param name="SharedUserIdList"></param>
        public void ShareCanvas(int canvasId, List<int> sharedUserIds)
        {
            em.ShareCanvas(canvasId, sharedUserIds);
        }

        /// <summary>
        /// Deletes canvas
        /// </summary>
        /// <param name="canvasId"></param>
        public void DeleteCanvas(int canvasId)
        {
            em.DeleteCanvas(canvasId);
        }
        /// <summary>
        /// Reads all the users in the current org
        /// </summary>
        /// <returns></returns>
        public DataSet ReadAllUsersInMyOrg(int orgId)
        {
            return em.ReadAllUsersInMyOrg(orgId);
        }

        public bool ResendEmail(int canvasId, List<int> SharedUserIds)
        {
            return em.ResendEmail(canvasId, SharedUserIds);
        }

    }
}