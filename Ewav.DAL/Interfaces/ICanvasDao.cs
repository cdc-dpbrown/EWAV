/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ICanvasDao.cs
 *  Namespace:  Ewav.DAL    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
// using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Data;

// -----------------------------------------------------------------------
// <copyright file="$safeitemrootname$.cs" company="$registeredorganization$">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
namespace Ewav.DAL
{
    public interface ICanvasDao
    {
        DataSet ShareCanvas(int CanvasId, int orgId, List<int> SharedUserIdList);
          

        DataTable GetCanvasShareStatus(int canvasID, int organizationID);

        DataTable GetCanvasShareStatusGuid(string canvasGuid);    


        /// <summary>
        /// Gets the snapshot.
        /// </summary>
        /// <param name="snapshotGuid">The snapshot GUID.</param>
        /// <returns></returns>
        string GetSnapshot(string snapshotGuid);

        /// <summary>
        /// Saves the snapshot.
        /// </summary>
        /// <param name="cdto">The cdto.</param>
        /// <returns></returns>
        string SaveSnapshot(CanvasDto cdto);

        string ConnectionString { get; set; }

        string TableName { get; set; }

        void DeleteCanvas(int canvasId);

        DataSet LoadCanvas(int canvasId);

        DataSet LoadCanvas(Guid canvasGUID);


        DataSet LoadCanvasListForUser(int UserId);

        DataSet ReadAllUsersInMyOrg(int orgId);

        int Save(CanvasDto canvasDto);

        void ShareCanvas(int CanvasId, List<int> SharedUserIdList);

        void Update(CanvasDto canvasDto);

        DataSet ExportData(string dsName, int lowerThreshold, int upperThreshold);

        DataSet ReadCanvasListForLite(string FormId, int UserId);

    }
}