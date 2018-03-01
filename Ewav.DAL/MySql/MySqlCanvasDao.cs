/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MySqlCanvasDao.cs
 *  Namespace:  Ewav.DAL.MySqlLayer    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

/// <summary>
/// TODO: Update summary.
/// </summary>
namespace Ewav.DAL.MySqlLayer
{
    public class MySqlCanvasDao : ICanvasDao
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }

        public string TableName { get; set; }

   

        /// <summary>
        /// Shares the canvas.
        /// </summary>
        /// <param name="CanvasId">The canvas id.</param>
        /// <param name="orgId">The org id.</param>
        /// <param name="SharedUserIdList">The shared user id list.</param>
        /// <returns></returns>
        DataSet ICanvasDao.ShareCanvas(int CanvasId, int orgId, List<int> SharedUserIdList)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        /// <summary>
        /// Shares the canvas.
        /// </summary>
        /// <param name="CanvasId">The canvas id.</param>
        /// <param name="orgId">The org id.</param>
        /// <param name="SharedUserIdList">The shared user id list.</param>
        public void ShareCanvas(int CanvasId, int orgId, List<int> SharedUserIdList)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the shared canvases.
        /// </summary>
        /// <param name="canvasID">The canvas ID.</param>
        /// <param name="organizationID">The organization ID.</param>
        /// <returns></returns>
        public DataTable GetCanvasShareStatus(int canvasID, int organizationID)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        public DataTable GetCanvasShareStatusGuid(string canvasGuid)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Loads the canvas.
        /// </summary>
        /// <param name="canvasGUID">The canvas GUID.</param>
        /// <returns></returns>
        public DataSet LoadCanvas(Guid canvasGUID)
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
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the snapshot.
        /// </summary>
        /// <param name="cdto">The cdto.</param>
        /// <returns></returns>
        public string SaveSnapshot(CanvasDto cdto)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the canvas.
        /// </summary>
        /// <param name="canvasId">The canvas id.</param>
        public void DeleteCanvas(int canvasId)
        {
            try
            {
                // Query string    
                string query = string.Empty;

                //  Connection      
                MySqlConnection conn = new MySqlConnection(ConnectionString);
                conn.Open();

                MySqlCommand myCommand = new MySqlCommand();
                myCommand.Connection = conn;
                myCommand.CommandText = "usp_delete_canvas";
                myCommand.CommandType = CommandType.StoredProcedure;

                myCommand.Parameters.Add(new MySqlParameter("canvasId", canvasId));

                myCommand.ExecuteNonQuery();
            }
            catch (MySqlException)
            {
                throw new ApplicationException("Error with MySQL -  delete  canvas");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DataSet ExportData(string dsName, int lowerThreshold, int upperThreshold)
        {
            throw new NotImplementedException();
        }

        public DataSet LoadCanvas(int canvasId)
        {
            //  DataSet ds = null;
            DataSet ds = MySqlUtilities.ExecuteSimpleDatsetFromStoredProc(ConnectionString, "usp_read_canvasInfo",
                new string[] { canvasId.ToString() });

            ////string readQuery = string.Format("Select UserID ,CanvasID, CanvasContent,  IsShared "  + 
            ////        " From {0} WHERE  CanvasID  ='{1}'", this.TableName, canvasId);

            //MySqlConnection conn = new MySqlConnection(this.ConnectionString);

            //try
            //{
            //    ds = MySqlHelper.ExecuteDataset(conn, string.Format("call usp_read_canvasInfo ({0} )  ", canvasId));
            //}
            ////catch (MySqlException)
            ////{
            ////    throw new Exception("MySql Exception");
            ////}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}

            return ds;
        }

        public DataSet LoadCanvasListForUser(int UserId)
        {
            DataSet ds = null;

            try
            {
                MySqlParameter pa = new MySqlParameter("UserId", UserId);
                ds = MySqlHelper.ExecuteDataset(ConnectionString, string.Format("call usp_read_all_canvases ({0} )  ", UserId));
            }
            //catch (MySqlException)
            //{
            //    throw new Exception("MySql Exception");
            //}
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ds;
        }

        public DataSet ReadAllUsersInMyOrg(int orgId)
        {
            DataSet ds = null;

            try
            {
                //  ds = MySqlHelper.ExecuteDataset(ConnectionString, string.Format(" call  up_read_all_users_in_my_org ( {0}) ", orgId));
                ds = MySqlUtilities.ExecuteSimpleDatsetFromStoredProc(ConnectionString, "usp_read_all_users_in_my_org",
                    new string[] { orgId.ToString() });
                //reads all the users.
                //  ds = db.ExecuteDataSet("usp_read_user", orgId, -1, " ", -1, -1);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ds;
        }

        public int Save(CanvasDto canvasDto)
        {
            if (canvasDto.IsNewCanvas)
            {
                try
                {
                    MySqlConnection conn = new MySqlConnection(ConnectionString);
                    conn.Open();

                    MySqlCommand myCommand = new MySqlCommand();
                    myCommand.Connection = conn;
                    myCommand.CommandText = "usp_save_canvas";
                    myCommand.CommandType = CommandType.StoredProcedure;

                    myCommand.Parameters.Add(new MySqlParameter("CanvasName", canvasDto.CanvasName));
                    myCommand.Parameters.Add(new MySqlParameter("CanvasContent", canvasDto.XmlData));
                    myCommand.Parameters.Add(new MySqlParameter("CanvasDescription", canvasDto.CanvasDescription));
                    myCommand.Parameters.Add(new MySqlParameter("CreatedDate", canvasDto.CreatedDate));
                    myCommand.Parameters.Add(new MySqlParameter("DatasourceID", canvasDto.DatasourceID));
                    myCommand.Parameters.Add(new MySqlParameter("ModifiedDate", canvasDto.ModifiedDate));
                    myCommand.Parameters.Add(new MySqlParameter("UserID", canvasDto.UserId));

                    MySqlParameter p1 = new MySqlParameter();
                    p1.ParameterName = "CanvasID";
                    p1.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(p1);

                    myCommand.ExecuteNonQuery();

                    int retval = Int32.Parse(p1.Value.ToString());
                    if (retval == -1)
                    {
                        throw new Exception("MySQL Exception -  canvas already exists");
                    }

                    return Int32.Parse(p1.Value.ToString());
                }
                catch (MySqlException mex)
                {
                    //  throw new   Exception("MySQL Exception - {0}", mex.Message);
                    throw new Exception(string.Format("MySQL Exception -  {0}", mex.Message));
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("{0}Stack trace -- {1}", ex.Message, ex.StackTrace));
                }
            }
            else
            {
                try
                {
                    MySqlConnection conn = new MySqlConnection(ConnectionString);
                    conn.Open();

                    MySqlCommand myCommand = new MySqlCommand();
                    myCommand.Connection = conn;
                    myCommand.CommandText = "usp_update_canvas";
                    myCommand.CommandType = CommandType.StoredProcedure;

                    myCommand.Parameters.Add(new MySqlParameter("CanvasContent", canvasDto.XmlData));
                    // myCommand.Parameters.Add(new MySqlParameter("CanvasID", canvasDto.CanvasId));
                    myCommand.Parameters.Add(new MySqlParameter("ModifiedDate", canvasDto.ModifiedDate));

                    myCommand.ExecuteNonQuery();

                    return canvasDto.CanvasId;
                }
                catch (MySqlException mex)
                {
                    throw new Exception(string.Format("MySQL Exception -  {0}", mex.Message));
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("{0}Stack trace -- {1}", ex.Message, ex.StackTrace));
                }
            }
        }

        public void ShareCanvas(int CanvasId, List<int> SharedUserIdList)
        {
            try
            {
                //  Connection      
                MySqlConnection conn = new MySqlConnection(ConnectionString);
                conn.Open();

                MySqlCommand myCommand = new MySqlCommand();
                myCommand.Connection = conn;
                myCommand.CommandText = "usp_share_canvas";
                myCommand.CommandType = CommandType.StoredProcedure;

                foreach (int userId in SharedUserIdList)
                {
                    myCommand.Parameters.Clear();
                    myCommand.Parameters.Add(new MySqlParameter("canvasId", CanvasId));
                    myCommand.Parameters.Add(new MySqlParameter("userId", userId));

                    myCommand.ExecuteNonQuery();
                }
            }
            catch (MySqlException)
            {
                throw new ApplicationException("Error with MySQL -   share  canvas");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(CanvasDto canvasDto)
        {
            throw new NotImplementedException();
        }


        public DataSet ReadCanvasListForLite(string FormId, int UserId)
        {
            throw new NotImplementedException("Functionality is not supported for underlying database type.");
        }
    }
}