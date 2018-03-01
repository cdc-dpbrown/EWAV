/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       SqlServerCanvasDao.cs
 *  Namespace:  Ewav.DAL.SqlServer    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
namespace Ewav.DAL.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SqlServerCanvasDao : ICanvasDao
    {
        public string ConnectionString { get; set; }
        public string TableName { get; set; }
        /// <summary>
        /// Shares the canvas.
        /// </summary>
        /// <param name="CanvasId">The canvas id.</param>
        /// <param name="SharedUserIdList">The shared user id list.</param>
        public void ShareCanvas(int CanvasId, List<int> SharedUserIdList)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the snapshot.
        /// </summary>
        /// <param name="snapshotGuid">The snapshot GUID.</param>
        /// <returns></returns>
        public string GetSnapshot(string snapshotGuid)
        {


            SqlDatabase db = new SqlDatabase(ConnectionString);
            string query = string.Empty;
            string snapshotAs64 = "";


            try
            {

                snapshotAs64 = Convert.ToString(db.ExecuteScalar("usp_get_canvas_snapshot", snapshotGuid));



            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }



            return snapshotAs64;







        }

        /// <summary>
        /// Saves the snapshot.
        /// </summary>
        /// <param name="cdto">The cdto.</param>
        /// <returns></returns>
        public string SaveSnapshot(CanvasDto cdto)
        {
            SqlDatabase db = new SqlDatabase(ConnectionString);
            string query = string.Empty;
            CanvasDto canvasDtoToReturn = cdto;
            string snapshotGuid = "";


            try
            {
                snapshotGuid = Convert.ToString(db.ExecuteScalar("usp_save_canvas_snapshot", cdto.CanvasId, cdto.CanvasSnapshotAsBase64));

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }



            return snapshotGuid;
        }

        public void DeleteCanvas(int canvasId)
        {
            SqlDatabase db = new SqlDatabase(this.ConnectionString);
            try
            {
                // db.ExecuteNonQuery(System.Data.CommandType.Text, query);
                db.ExecuteNonQuery("usp_delete_canvas", canvasId);
            }
            catch (SqlException sqlEx)
            {
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Loads the canvas.
        /// </summary>
        public DataSet LoadCanvas(int CanvasId)
        {
            SqlDatabase db = new SqlDatabase(this.ConnectionString);
            DataSet ds = null;
            //string readQuery = "Select [UserID],[CanvasID], [CanvasContent] From " + this.TableName + " WHERE [CanvasID] ='" + CanvasId + "'";

            try
            {
                ds = db.ExecuteDataSet("usp_read_canvasinfo", CanvasId);
            }
            catch (Exception)
            {
                throw;
            }

            return ds;
        }

        public DataSet LoadCanvas(Guid canvasGUID)
        {
            SqlDatabase db = new SqlDatabase(this.ConnectionString);
            DataSet ds = null;
            //string readQuery = "Select [UserID],[CanvasID], [CanvasContent] From " + this.TableName + " WHERE [CanvasID] ='" + CanvasId + "'";

            try
            {
                ds = db.ExecuteDataSet("usp_read_canvasinfoGUID", canvasGUID);
            }
            catch (Exception)
            {
                throw;
            }

            return ds;
        }


        /// <summary>
        /// Loads the canvas list for user.
        /// No longer using datasournceId parameter
        /// </summary>
        public DataSet LoadCanvasListForUser(int UserId)
        {
            SqlDatabase db = new SqlDatabase(this.ConnectionString);
            DataSet ds = null;
            //int userId = 0;
            //string readQuery = "Select * From " + this.TableName + " WHERE USERID=" + UserId + " Order By CreatedDate DESC ";

            //string readyQuery = string.Format("EXEC	@return_value = [dbo].[up_read_all_canvases] @userID = {0}", UserId);

            try
            {
                ds = db.ExecuteDataSet("usp_read_all_canvases", UserId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ds;
        }

        public DataSet ReadAllUsersInMyOrg(int orgId)
        {
            SqlDatabase db = new SqlDatabase(this.ConnectionString);
            DataSet ds = null;
            //int userId = 0;
            //string readQuery = "Select UserID, FirstName, LastName From [User] WHERE OrganizationID=" + orgId;//+ this.TableName ;

            try
            {
                //ds = db.ExecuteDataSet(System.Data.CommandType.Text, readQuery);
                //reads all the users.
                ds = db.ExecuteDataSet("usp_read_user", orgId, -1, " ", -1);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ds;
        }



        public DataTable GetCanvasShareStatusGuid(string canvasGuid)
        {
            DataSet ds = new DataSet();

            SqlDatabase db = new SqlDatabase(ConnectionString);
            string query = string.Empty;

            try
            {
                ds = db.ExecuteDataSet("usp_get_shared_canvases_guid", new Guid(canvasGuid));
            }
            catch (SqlException sqlEx)
            {
                Exception duplicateException = new Exception(string.Format("SQL Exception - {0}", sqlEx.Message));
                throw duplicateException;
            }

            return ds.Tables[0];
        }


        public DataTable GetCanvasShareStatus(int canvasID, int organizationID)
        {
            DataSet ds = new DataSet();

            SqlDatabase db = new SqlDatabase(ConnectionString);
            string query = string.Empty;

            try
            {
                ds = db.ExecuteDataSet("usp_get_shared_canvases", canvasID, organizationID);
            }
            catch (SqlException sqlEx)
            {
                Exception duplicateException = new Exception(string.Format("SQL Exception - {0}", sqlEx.Message));
                throw duplicateException;
            }

            return ds.Tables[0];
        }





        /// <summary>
        /// Saves the specified canvas dto.
        /// </summary>
        /// <param name="canvasDto">The canvas dto.</param>
        public int Save(CanvasDto canvasDto)
        {
            SqlDatabase db = new SqlDatabase(ConnectionString);
            string query = string.Empty;
            CanvasDto canvasDtoToReturn = canvasDto;
            int canvasId = -1;

        // //     SqlConnection sqlConn = new SqlConnection(this.ConnectionString);
            //  sqlConn.Open();

          // //   SqlCommand addCanvasCommand = sqlConn.CreateCommand();

            //               [CanvasID]
            //        INT IDENTITY(1, 1) NOT NULL,

            //[CanvasGUID]        UNIQUEIDENTIFIER CONSTRAINT[DF_Canvas_CanvasGUID] DEFAULT(newid()) NOT NULL,

            //[CanvasName]        VARCHAR(50)     NOT NULL,

            //[UserID]            INT NOT NULL,
            //    [CanvasDescription] VARCHAR(MAX)    NULL,
            //    [CreatedDate]
            //        DATETIME NOT NULL,
            //    [ModifiedDate]
            //        DATETIME NOT NULL,
            //    [DatasourceID]
            //        INT NOT NULL,
            //    [CanvasContent]     NVARCHAR(MAX)   NOT NULL,    


            //  SqlParameter param = new SqlParameter("CanvasName", SqlDbType.NVarChar);

           //   addCanvasCommand.Connection = new SqlConnection(ConnectionString);    

            ////addCanvasCommand.CommandType = CommandType.StoredProcedure;
            ////addCanvasCommand.CommandText = "usp_save_canvas";    

            ////addCanvasCommand.Parameters.Add("CanvasName", SqlDbType.NVarChar).Value = canvasDto.CanvasName;
            ////addCanvasCommand.Parameters.Add("UserID", SqlDbType.Int  ).Value = canvasDto.UserId;
            ////addCanvasCommand.Parameters.Add("CANVASDESC", SqlDbType.NVarChar ).Value = canvasDto.CanvasDescription;
            ////addCanvasCommand.Parameters.Add("CreatedDate", SqlDbType.Date    ).Value = canvasDto.CreatedDate;
            ////addCanvasCommand.Parameters.Add("ModifiedDate", SqlDbType.Date).Value = canvasDto.ModifiedDate;
            ////addCanvasCommand.Parameters.Add("DatasourceID", SqlDbType.Int  ).Value = canvasDto.DatasourceID;
            ////addCanvasCommand.Parameters.Add("IsNewCanvas", SqlDbType.Bit      ).Value = canvasDto.IsNewCanvas;
            ////addCanvasCommand.Parameters.Add("CanvasID", SqlDbType.Int      ).Value = canvasDto.CanvasId;
            ////addCanvasCommand.Parameters.Add("XMLCONTENT", SqlDbType.NVarChar).Value = canvasDto.XmlData.ToString();



     
            try
            {
                //sqlConn.Open();

                //canvasId = Convert.ToInt32(addCanvasCommand.ExecuteScalar());

                //  canvasId = Convert.ToInt32(db.ExecuteScalar("usp_save_canvas", addCanvasCommand.Parameters  ));

                canvasId = Convert.ToInt32(db.ExecuteScalar("usp_save_canvas", canvasDto.CanvasName, canvasDto.UserId, canvasDto.CanvasDescription, canvasDto.CreatedDate,
                    canvasDto.ModifiedDate, canvasDto.DatasourceID, canvasDto.IsNewCanvas, canvasDto.CanvasId, canvasDto.XmlData.ToString()));

            }
            catch (SqlException sqlEx)
            {
                Exception duplicateException = new Exception(string.Format("SQL Exception - {0}", sqlEx.Message));
                throw duplicateException;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            canvasDtoToReturn.CanvasId = canvasId;


            return canvasId;
        }


        public DataSet ShareCanvas(int CanvasId, int orgId, List<int> SharedUserIdList)
        {
            DataSet ds = new DataSet();

            string userIds = string.Join<int>(",", SharedUserIdList);

            SqlDatabase db = new SqlDatabase(ConnectionString);

            try
            {
                ds = db.ExecuteDataSet("usp_update_sharedcanvases", orgId, CanvasId, userIds);
            }
            catch (SqlException ex)
            {


                throw new Exception(ex.Message);



            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ds;
        }

        public void ShareCanvas1(int CanvasId, List<int> SharedUserIdList)
        {
            SqlDatabase db = new SqlDatabase(ConnectionString);
            for (int i = 0; i < SharedUserIdList.Count; i++)
            {
                //  string query = string.Format(
                // "INSERT INTO {0}    ([CanvasID] " +
                //    ",[UserID])" +
                //" VALUES ('{1}','{2}') ", "SharedCanvases", CanvasId, SharedUserIdList[i]);
                try
                {
                    // db.ExecuteNonQuery(System.Data.CommandType.Text, query);
                    db.ExecuteNonQuery("usp_sharecanvas", CanvasId, SharedUserIdList[i]);
                }
                catch (SqlException ex)
                {
                    //     query = string.Format(
                    //"UPDATE {0}    " +
                    //   "SET CanvasID = {1}, UserID = {2}", "SharedCanvases", CanvasId, SharedUserIdList[i]);
                    //     Exception duplicateException = new Exception("SQL Exception - " + sqlEx.Message);
                    //     throw duplicateException;
                    throw new Exception(ex.Message);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        /// <summary>
        /// Updates the specified canvas dto.
        /// </summary>
        /// <param name="canvasDto">The canvas dto.</param>
        public void Update(CanvasDto canvasDto)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }


        public DataSet ExportData(string dsName, int lowerThreshold, int upperThreshold)
        {
            throw new NotImplementedException();
        }


        public DataSet ReadCanvasListForLite(string FormId, int UserId)
        {
            SqlDatabase db = new SqlDatabase(this.ConnectionString);
            DataSet ds = null;
            try
            {
                ds = db.ExecuteDataSet("usp_read_canvases_for_lite", FormId, UserId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ds;
        }
    }
}