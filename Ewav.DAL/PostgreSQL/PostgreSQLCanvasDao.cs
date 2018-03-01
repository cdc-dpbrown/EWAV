/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       PostgreSQLCanvasDao.cs
 *  Namespace:  Ewav.DAL.PostgreSQL    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using System.Data;

namespace Ewav.DAL.PostgreSQL
{
    class PostgreSQLCanvasDao : ICanvasDao
    {

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string ConnectionString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        public string TableName
        {
            get;
            set;
        }

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
        /// <exception cref="System.Exception">Error deleting the canvas.  + ex.Message</exception>
        public void DeleteCanvas(int canvasId)
        {
            PostgreSQLDB db = new PostgreSQLDB(this.ConnectionString);

            NpgsqlCommand Command = new NpgsqlCommand();

            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "delete_canvas";

            NpgsqlParameter parameter = new NpgsqlParameter("cid", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = canvasId;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);


            try
            {
                db.ExecuteNonQuery(Command);
            }
            catch (Exception ex)
            {

                throw new Exception("Error deleting the canvas. " + ex.Message);
            }
        }


        /// <summary>
        /// Loads the canvas.
        /// </summary>
        /// <param name="canvasId">The canvas id.</param>
        /// <returns></returns>
        public DataSet LoadCanvas(int canvasId)
        {
            PostgreSQLDB db = new PostgreSQLDB(this.ConnectionString);
            NpgsqlCommand Command = new NpgsqlCommand();
            DataSet ds;

            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "read_canvasinfo";

            NpgsqlParameter parameter = new NpgsqlParameter("cid", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = canvasId;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            try
            {
                ds = db.ExecuteDataSet(Command);
            }
            catch (Exception Ex)
            {

                throw;
            }
                        
            return ds;
        }

        /// <summary>
        /// Loads the canvas list for user.
        /// </summary>
        /// <param name="UserId">The user id.</param>
        /// <returns></returns>
        public DataSet LoadCanvasListForUser(int UserId)
        {
            PostgreSQLDB db = new PostgreSQLDB(this.ConnectionString);
            NpgsqlCommand Command = new NpgsqlCommand();
            DataSet ds;

            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "read_all_canvases";

            NpgsqlParameter parameter = new NpgsqlParameter("UserId", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = UserId;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            try
            {
                ds = db.ExecuteDataSet(Command);
            }
            catch (Exception Ex)
            {

                throw;
            }

            return ds;
        }

        /// <summary>
        /// Reads all users in my org.
        /// </summary>
        /// <param name="orgId">The org id.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public DataSet ReadAllUsersInMyOrg(int orgId)
        {
            PostgreSQLDB db = new PostgreSQLDB(this.ConnectionString);
            NpgsqlCommand Command = new NpgsqlCommand();
            DataSet ds;

            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "read_user";

            NpgsqlParameter parameter = new NpgsqlParameter("orgid", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = orgId;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("uid", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = -1;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("email", NpgsqlTypes.NpgsqlDbType.Varchar);
            parameter.Value = "";
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("rid", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = -1;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);

            try
            {
                ds = db.ExecuteDataSet(Command);
            }
            catch (Exception Ex)
            {

                throw new Exception(Ex.Message);
            }

            return ds;
        }

        /// <summary>
        /// Saves the specified canvas dto.
        /// </summary>
        /// <param name="canvasDto">The canvas dto.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public int Save(CanvasDto canvasDto)
        {
            PostgreSQLDB db = new PostgreSQLDB(ConnectionString);
            string query = string.Empty;
            CanvasDto canvasDtoToReturn = canvasDto;
            int canvasId = -1;


            NpgsqlCommand Command = new NpgsqlCommand();
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "save_canvas";

            NpgsqlParameter parameter = new NpgsqlParameter("cname", NpgsqlTypes.NpgsqlDbType.Text);
            parameter.Value = canvasDto.CanvasName;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);
            parameter = new NpgsqlParameter("uid", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = canvasDto.UserId;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);
            parameter = new NpgsqlParameter("cdesc", NpgsqlTypes.NpgsqlDbType.Text);
            parameter.Value = canvasDto.CanvasDescription;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);
            parameter = new NpgsqlParameter("cdate", NpgsqlTypes.NpgsqlDbType.Date);
            parameter.Value = canvasDto.CreatedDate;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);
            parameter = new NpgsqlParameter("mdate", NpgsqlTypes.NpgsqlDbType.Date);
            parameter.Value = canvasDto.ModifiedDate;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);
            parameter = new NpgsqlParameter("did", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = canvasDto.DatasourceID;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);
            parameter = new NpgsqlParameter("isnewcanvas", NpgsqlTypes.NpgsqlDbType.Boolean);
            parameter.Value = canvasDto.IsNewCanvas;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);
            parameter = new NpgsqlParameter("cid", NpgsqlTypes.NpgsqlDbType.Integer);
            parameter.Value = canvasDto.CanvasId;
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);
            parameter = new NpgsqlParameter("xmlcontent", NpgsqlTypes.NpgsqlDbType.Text);
            parameter.Value = canvasDto.XmlData.ToString();
            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);
            

            try
            {
                canvasId = Convert.ToInt32(db.ExecuteScalar(Command));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            canvasDtoToReturn.CanvasId = canvasId;
            return canvasId;
        }

        /// <summary>
        /// Shares the canvas.
        /// </summary>
        /// <param name="CanvasId">The canvas id.</param>
        /// <param name="SharedUserIdList">The shared user id list.</param>
        /// <exception cref="System.Exception"></exception>
        public void ShareCanvas(int CanvasId, List<int> SharedUserIdList)
        {

            PostgreSQLDB db = new PostgreSQLDB(this.ConnectionString);




            for (int i = 0; i < SharedUserIdList.Count; i++)
            {
                NpgsqlCommand Command = new NpgsqlCommand();

                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "sharecanvas";
                
                NpgsqlParameter parameter = new NpgsqlParameter("cid", NpgsqlTypes.NpgsqlDbType.Integer);
                parameter.Value = CanvasId;
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                parameter = new NpgsqlParameter("uid", NpgsqlTypes.NpgsqlDbType.Integer);
                parameter.Value = SharedUserIdList[i];
                parameter.Direction = ParameterDirection.Input;
                Command.Parameters.Add(parameter);

                try
                {
                    db.ExecuteNonQuery(Command);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public void Update(CanvasDto canvasDto)
        {
            throw new NotImplementedException();
        }

        public System.Data.DataSet ExportData(string dsName, int lowerThreshold, int upperThreshold)
        {
            throw new NotImplementedException();
        }


        public DataSet ReadCanvasListForLite(string FormId, int UserId)
        {
            throw new NotImplementedException("Functionality is not supported for underlying database type.");
        }
    }
}