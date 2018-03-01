/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       AdminDatasourceViewModel.cs
 *  Namespace:  Ewav.ViewModels    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Windows;
using System.Threading;
using System.Collections.ObjectModel;

// Toolkit namespace
using SimpleMvvmToolkit;

// Toolkit extension methods
using SimpleMvvmToolkit.ModelExtensions;
using Ewav.Services;
using Ewav.DTO;
using System.Collections.Generic;
using Ewav.BAL;

namespace Ewav.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class AdminDatasourceViewModel : ViewModelBase<AdminDatasourceViewModel>
    {
        #region Initialization and Cleanup



        // TODO: Add a member for IXxxServiceAgent
        private IAdminDatasourcesServiceAgent serviceAgent;

        private IUserServiceAgent userServiceAgent;

        // Default ctor
        public AdminDatasourceViewModel()
        {
            serviceAgent = new AdminDatasourcesServiceAgent();
            userServiceAgent = new UserServiceAgent();
        }

        // TODO: ctor that accepts IXxxServiceAgent
        public AdminDatasourceViewModel(IAdminDatasourcesServiceAgent serviceAgent)
        {
            this.serviceAgent = serviceAgent;
        }

        #endregion

        #region Notifications


        // TODO: Add events to notify the view or obtain data from the view
        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;
        public event EventHandler<NotificationEventArgs<Exception>> AddCompletedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> UpdateCompletedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> DeleteCompletedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> ReadCompletedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> ReadUsersCompletedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> CopyDashboardCompletedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> ReadAssociatedUsersCompletedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> TestConnectionCompletedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> TestDataCompletedEvent;
        public event EventHandler<NotificationEventArgs<Exception>> ReadEweFormIdCompletedEvent;

        public event EventHandler GetAllDatasourceUserCompletedEvent;


        #endregion

        #region Properties


        #endregion

        #region Methods

        public void Add(DatasourceDto dsDto)
        {
            serviceAgent.Add(dsDto, AddCompleted);

        }

        public void Delete(int dsId)
        {
            serviceAgent.Delete(dsId, DeleteCompleted);
        }

        public void Update(DatasourceDto dsDto)
        {
            serviceAgent.Update(dsDto, UpdateCompleted);
        }

        public void Read(int orgId)
        {
            serviceAgent.Read(orgId, ReadCompleted);
        }

        public void TestConnection(Connection con)
        {
            serviceAgent.TestDBConnection(con, TestConnectionCompleted);
        }

        public void TestData(Connection connectionInfo)
        {
            serviceAgent.TestData(connectionInfo, TestDataCompleted);
        }

        public void ReadUsers(int organizationId)
        {
            userServiceAgent.ReadUser(-1, organizationId, ReadUsersCompleted); // sending -1 to read all users.
        }

        public void ReadAssociatedUsers(int dsId, int orgid)
        {
            serviceAgent.ReadAssociatedUsers(dsId, orgid, ReadAssociatedUsersCompleted);
        }

        public void GetAllDatasourceUser()
        {
            serviceAgent.GetAllDatasourceUser(GetAllDatasourceUserCompleted);
        }

        private void GetAllDatasourceUserCompleted(IEnumerable<DatasourceUserDto> dtoList, Exception e)
        {
            NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
            if (e == null)
            {
                //AddCompletedEvent(result, new NotificationEventArgs<Exception>());
                //this.Notify(ReadCompletedEvent, notification);
                GetAllDatasourceUserCompletedEvent(dtoList, notification);
            }
            else
            {
                this.Notify(ErrorNotice, notification);
            }
        }

        public void CopyDashboard(string OldCanvasName, string NewCanvasName, string OldDatasourceName, string NewDatasourceName)
        {
            serviceAgent.CopyDashboard(OldCanvasName, NewCanvasName, OldDatasourceName, NewDatasourceName, CopyDashboardCompleted);
        }

        public void ReadEWEDatasourceFormId(EWEDatasourceDto EweDSDto)
        {
            serviceAgent.ReadEWEDatasourceFormId(EweDSDto, ReadEweFormIdCompleted);
        }
        #endregion

        #region Completion Callbacks

        // TODO: Optionally add callback methods for async calls to the service agent

        private void AddCompleted(bool result, Exception e)
        {
            NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
            if (e == null)
            {
                //AddCompletedEvent(result, new NotificationEventArgs<Exception>());
                //this.Notify(AddCompletedEvent,notification);
                AddCompletedEvent(result, null);
            }
            else
            {
                this.Notify(ErrorNotice, notification);
            }

        }

        private void UpdateCompleted(bool result, Exception e)
        {
            NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
            if (e == null)
            {
                //AddCompletedEvent(result, new NotificationEventArgs<Exception>());
                //this.Notify(UpdateCompletedEvent, notification);
                UpdateCompletedEvent(result, notification);
            }
            else
            {
                this.Notify(ErrorNotice, notification);
            }
        }

        private void DeleteCompleted(bool result, Exception e)
        {
            NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
            if (e == null)
            {
                //AddCompletedEvent(result, new NotificationEventArgs<Exception>());
                //this.Notify(DeleteCompletedEvent, notification);
                DeleteCompletedEvent(result, notification);
            }
            else
            {
                this.Notify(ErrorNotice, notification);
            }
        }

        private void ReadCompleted(List<DatasourceDto> dtoList, Exception e)
        {
            NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
            if (e == null)
            {
                //AddCompletedEvent(result, new NotificationEventArgs<Exception>());
                //this.Notify(ReadCompletedEvent, notification);
                ReadCompletedEvent(dtoList, notification);
            }
            else
            {
                this.Notify(ErrorNotice, notification);
            }
        }

        private void TestConnectionCompleted(bool result, Exception e)
        {
            NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
            if (e == null)
            {
                //AddCompletedEvent(result, new NotificationEventArgs<Exception>());
                //this.Notify(TestConnectionCompletedEvent, notification);
                TestConnectionCompletedEvent(result, notification);
            }
            else
            {
                this.Notify(ErrorNotice, notification);
            }
        }

        private void TestDataCompleted(bool result, Exception e)
        {
            NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
            if (e == null)
            {
                //AddCompletedEvent(result, new NotificationEventArgs<Exception>());
                //this.Notify(TestDataCompletedEvent, notification);
                TestDataCompletedEvent(result, notification);
            }
            else
            {
                this.Notify(ErrorNotice, notification);
            }
        }

        private void ReadUsersCompleted(List<UserDTO> result, Exception e)
        {
            NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
            if (e == null)
            {
                //AddCompletedEvent(result, new NotificationEventArgs<Exception>());
                //this.Notify(ReadCompletedEvent, notification);
                ReadUsersCompletedEvent(result, notification);
            }
            else
            {
                this.Notify(ErrorNotice, notification);
            }
        }

        private void ReadAssociatedUsersCompleted(List<UserDTO> result, Exception e)
        {
            NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
            if (e == null)
            {
                //AddCompletedEvent(result, new NotificationEventArgs<Exception>());
                //this.Notify(ReadCompletedEvent, notification);
                ReadAssociatedUsersCompletedEvent(result, notification);
            }
            else
            {
                this.Notify(ErrorNotice, notification);
            }
        }

        private void CopyDashboardCompleted(string result, Exception e)
        {
            NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>(result, e);
            if (e == null)
            {
                CopyDashboardCompletedEvent(result, notification);
            }
            else
            {
                this.Notify(ErrorNotice, notification);
            }
        }

        private void ReadEweFormIdCompleted(string result, Exception e)
        {
            NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>(result, e);
            if (e == null)
            {
                ReadEweFormIdCompletedEvent(result, notification);
            }
            else
            {
                this.Notify(ErrorNotice, notification);
            }
        }
        #endregion

        #region Helpers

        // Helper method to notify View of an error
        private void NotifyError(string message, Exception error)
        {
            // Notify view of an error
            Notify(ErrorNotice, new NotificationEventArgs<Exception>(message, error));
        }

        #endregion
    }
}