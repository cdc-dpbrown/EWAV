/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       AdminDatasourcesServiceAgent.cs
 *  Namespace:  Ewav.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using Ewav.Web.Services.AdminDatasourcesDomainService;
using Ewav.DTO;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;

namespace Ewav.Services
{
    public class AdminDatasourcesServiceAgent : IAdminDatasourcesServiceAgent
    {
        AdminDatasourcesDomainContext dsCtx;

        private Action<bool, Exception> _addCompleted;
        private Action<bool, Exception> _delCompleted;
        private Action<List<DatasourceDto>, Exception> _readCompleted;
        private Action<bool, Exception> _updCompleted;
        private Action<bool, Exception> _testDBCompleted;
        private Action<bool, Exception> _testDataCompleted;
        private Action<List<UserDTO>, Exception> _readAssociatedListCompleted;
        private Action<string, Exception> _copyDashboardCompleted;
        private Action<string, Exception> _eweDsNameReadCompleted;

        private Action<List<DatasourceUserDto>, Exception> _getDatasourceUserCompleted;

        /// <summary>
        /// Tests the DB connection.
        /// </summary>
        /// <param name="con">The con.</param>
        /// <param name="completed">The completed.</param>
        public void TestDBConnection(Connection con, Action<bool, Exception> completed)
        {
            _testDBCompleted = completed;
            dsCtx = new AdminDatasourcesDomainContext();
            InvokeOperation<bool> testDBResult = dsCtx.TestDBConnection(con);
            testDBResult.Completed += new EventHandler(testDBResult_Completed);
        }

        /// <summary>
        /// Handles the Completed event of the testDBResult.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        void testDBResult_Completed(object sender, EventArgs e)
        {
            InvokeOperation<bool> result = (InvokeOperation<bool>)sender;
            Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                ex = result.Error;
            }

            _testDBCompleted(result.Value, ex);
        }

        /// <summary>
        /// Tests the data.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="completed">The completed.</param>
        public void TestData(Connection connectionInfo, Action<bool, Exception> completed)
        {
            _testDataCompleted = completed;
            dsCtx = new AdminDatasourcesDomainContext();
            InvokeOperation<bool> testData = dsCtx.TestData(connectionInfo);
            testData.Completed += new EventHandler(testData_Completed);
        }

        void testData_Completed(object sender, EventArgs e)
        {
            InvokeOperation<bool> result = (InvokeOperation<bool>)sender;
            Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                ex = result.Error;
            }

            _testDataCompleted(result.Value, ex);
        }

        /// <summary>
        /// Adds the specified dto.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <param name="completed">The completed.</param>
        public void Add(DTO.DatasourceDto dto, Action<bool, Exception> completed)
        {
            _addCompleted = completed;
            dsCtx = new AdminDatasourcesDomainContext();
            InvokeOperation<bool> addResult = dsCtx.AddDatasource(dto);
            addResult.Completed += new EventHandler(addResult_Completed);
        }

        public void GetAllDatasourceUser(Action<IEnumerable<DatasourceUserDto>, Exception> completed)
        {
            _getDatasourceUserCompleted = completed;
            dsCtx = new AdminDatasourcesDomainContext();
            InvokeOperation<IEnumerable<DatasourceUserDto>> AllDatasourceUsers = dsCtx.GetAllDatasourceUser();

            dsCtx.GetAllDatasourceUser(result =>
            {
                Exception ex = null;
                if (result.HasError)
                {
                    result.MarkErrorAsHandled();
                    ex = result.Error;
                    throw new GadgetException(result.Error.Message);

                }

                IEnumerable<DatasourceUserDto> datasourceUserDtoList = ((InvokeOperation<IEnumerable<DatasourceUserDto>>)result).Value;
                completed(datasourceUserDtoList, ex);
            }, null);

        }

        void AllDatasourceUsers_Completed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void addResult_Completed(object sender, EventArgs e)
        {
            InvokeOperation<bool> result = (InvokeOperation<bool>)sender;
            Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                ex = result.Error;
            }

            _addCompleted(result.Value, ex);
        }

        /// <summary>
        /// Deletes the specified ds id.
        /// </summary>
        /// <param name="dsId">The ds id.</param>
        /// <param name="completed">The completed.</param>
        public void Delete(int dsId, Action<bool, Exception> completed)
        {
            _delCompleted = completed;
            dsCtx = new AdminDatasourcesDomainContext();
            InvokeOperation<bool> delResult = dsCtx.RemoveDatasource(dsId);
            delResult.Completed += new EventHandler(delResult_Completed);
        }

        void delResult_Completed(object sender, EventArgs e)
        {
            InvokeOperation<bool> result = (InvokeOperation<bool>)sender;
            Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                ex = result.Error;
            }

            _delCompleted(result.Value, ex);
        }

        /// <summary>
        /// Updates the specified dto.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <param name="completed">The completed.</param>
        public void Update(DTO.DatasourceDto dto, Action<bool, Exception> completed)
        {
            _updCompleted = completed;
            dsCtx = new AdminDatasourcesDomainContext();
            InvokeOperation<bool> updResult = dsCtx.UpdateDatasource(dto);
            updResult.Completed += new EventHandler(updResult_Completed);
        }

        void updResult_Completed(object sender, EventArgs e)
        {
            InvokeOperation<bool> result = (InvokeOperation<bool>)sender;
            Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                ex = result.Error;
            }
            _updCompleted(result.Value, ex);
        }

        /// <summary>
        /// Reads the specified ds id.
        /// </summary>
        /// <param name="dsId">The ds id.</param>
        /// <param name="completed">The completed.</param>
        public void Read(int orgId, Action<System.Collections.Generic.List<DTO.DatasourceDto>, Exception> completed)
        {
            _readCompleted = completed;
            dsCtx = new AdminDatasourcesDomainContext();
            InvokeOperation<List<DatasourceDto>> readResult = dsCtx.ReadDatasource(orgId);
            readResult.Completed += new EventHandler(readResult_Completed);
        }

        void readResult_Completed(object sender, EventArgs e)
        {
            InvokeOperation<List<DatasourceDto>> result = (InvokeOperation<List<DatasourceDto>>)sender;
            Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                ex = result.Error;
            }
            _readCompleted(result.Value, ex);
        }


        public void ReadAssociatedUsers(int dsId, int orgid, Action<List<UserDTO>, Exception> completed)
        {
            _readAssociatedListCompleted = completed;
            dsCtx = new AdminDatasourcesDomainContext();

            InvokeOperation<List<UserDTO>> readUsersResult = dsCtx.ReadAssociatedUsers(dsId, orgid);
            readUsersResult.Completed += new EventHandler(readUsersResult_Completed);
        }

        void readUsersResult_Completed(object sender, EventArgs e)
        {
            InvokeOperation<List<UserDTO>> result = (InvokeOperation<List<UserDTO>>)sender;
            Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                ex = result.Error;
            }
            _readAssociatedListCompleted(result.Value, ex);
        }


        public void CopyDashboard(string OldCanvasName, string NewCanvasName, string OldDatasourceName, string NewDatasourceName, Action<string, Exception> completed)
        {
            _copyDashboardCompleted = completed;
            dsCtx = new AdminDatasourcesDomainContext();

            InvokeOperation<string> CopyDashboardResult = dsCtx.CopyDashboard(OldCanvasName, NewCanvasName, OldDatasourceName, NewDatasourceName);
            CopyDashboardResult.Completed += new EventHandler(CopyDashboardResult_Completed);
        }

        void CopyDashboardResult_Completed(object sender, EventArgs e)
        {
            InvokeOperation<string> result = (InvokeOperation<string>)sender;
            Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                ex = result.Error;
            }
            _copyDashboardCompleted(result.Value, ex);
        }

        public void ReadEWEDatasourceFormId(EWEDatasourceDto EweDsDto, Action<string, Exception> completed)
        {
            _eweDsNameReadCompleted = completed;
            dsCtx = new AdminDatasourcesDomainContext();

            InvokeOperation<string> ReadEweResult = dsCtx.ReadEWEDatasourceFormId(EweDsDto);
            ReadEweResult.Completed += new EventHandler(ReadEweResult_Completed);
        }

        void ReadEweResult_Completed(object sender, EventArgs e)
        {
            InvokeOperation<string> result = (InvokeOperation<string>)sender;
            Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                ex = result.Error;
            }
            _eweDsNameReadCompleted(result.Value, ex);
        }

    }
}