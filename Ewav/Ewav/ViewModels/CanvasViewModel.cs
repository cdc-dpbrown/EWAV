/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       CanvasViewModel.cs
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
using Ewav.DTO;
using SimpleMvvmToolkit;
// Toolkit extension methods
using SimpleMvvmToolkit.ModelExtensions;
using Ewav.Services;
using Ewav.Web.Services;
using System.Collections.Generic;

namespace Ewav.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class CanvasViewModel : ViewModelBase<CanvasViewModel>
    {
        #region Initialization and Cleanup

        // TODO: Add a member for IXxxServiceAgent
        private ICanvasServiceAgent serviceAgent;

        // Default ctor
        public CanvasViewModel()
        {
            this.serviceAgent = new CanvasServiceAgent();
        }

        // TODO: ctor that accepts IXxxServiceAgent
        public CanvasViewModel(ICanvasServiceAgent serviceAgent)
        {
            this.serviceAgent = serviceAgent;
        }

        private string loadedXMLString;



        /// <summary>
        /// Gets or sets the saved canvas snapshot GUID.
        /// </summary>
        /// <value>The saved canvas snapshot GUID.</value>
        public string SavedCanvasSnapshotGuid { get; set; }

        /// <summary>
        /// Gets or sets the loaded canvas.
        /// </summary>
        /// <value>The loaded canvas.</value>
        public CanvasDto LoadedCanvas { get; set; }

        /// <summary>
        /// Gets or sets the canvas share status dto list.
        /// </summary>
        /// <value>The canvas share status dto list.</value>
        public List<CanvasShareStatusDto> CanvasShareStatusDtoList { get; set; }

        /// <summary>
        /// Gets the canvas shared status.
        /// </summary>
        /// <param name="canvasId">The canvas id.</param>
        /// <param name="organizationId">The organization id.</param>
        public void GetCanvasSharedStatus(int canvasId, int organizationId)
        {

            this.serviceAgent.GetCanvasSharedStatus(canvasId, organizationId, GetCanvasSharedStatusCompleted);

        }




        public void GetCanvasSharedStatusWithGuid(string canvasGUID)
        {

            this.serviceAgent.GetCanvasSharedStatusWithGuid(canvasGUID, GetCanvasSharedStatusWithGuidCompleted);

        }


        /// <summary>
        /// Gets the canvas dto with GUID.
        /// </summary>
        /// <param name="canvasGUIDFromUrl">The canvas GUID from URL.</param>
        public void GetCanvasDtoWithGUID(Guid canvasGUIDFromUrl)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the canvas snapshot.
        /// </summary>
        /// <param name="canvasSnapshotAsBase64">The canvas snapshot as base64.</param>
        public void SaveCanvasSnapshot(CanvasDto cdto)
        {

            this.serviceAgent.SaveCanvasSnapshot(cdto, SaveCanvasSnapshotCompleted);


        }

        public string LoadedXMLString
        {
            get { return loadedXMLString; }
            set { loadedXMLString = value; }
        }

        private int userIdOfOpenedCanvas;

        public int UserIdOfOpenedCanvas
        {
            get { return userIdOfOpenedCanvas; }
            set { userIdOfOpenedCanvas = value; }
        }

        private int canvasIdOfOpenedCanvas;

        public int CanvasIdOfOpenedCanvas
        {
            get { return canvasIdOfOpenedCanvas; }
            set { canvasIdOfOpenedCanvas = value; }
        }

        private bool isCurrentCanvasShared;

        public bool IsCurrentCanvasShared
        {
            get { return isCurrentCanvasShared; }
            set { isCurrentCanvasShared = value; }
        }
        #endregion

        #region Notifications

        // TODO: Add events to notify the view or obtain data from the view
        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;
        public event EventHandler<NotificationEventArgs<Exception>> CanvasSaved;
        public event EventHandler<NotificationEventArgs<Exception>> UserListLoaded;
        public event EventHandler<NotificationEventArgs<Exception>> CanvasLoaded;
        public event EventHandler<NotificationEventArgs<Exception>> CanvasShared;
        public event EventHandler<NotificationEventArgs<Exception>> EmailResent;
        public event EventHandler<NotificationEventArgs<Exception>> CanvasDeleted;

        public event EventHandler<NotificationEventArgs<Exception>> CanvasShareStatusDtoLoaded;
        public event EventHandler<NotificationEventArgs<Exception>> AllUsersInMyOrgLoaded;
        public event EventHandler<NotificationEventArgs<Exception>> CanvasSnapshotCompleted;








        #endregion

        #region Properties

        // TODO: Add properties using the mvvmprop code snippet

        private DatatableBag results;

        public DatatableBag Results
        {
            get { return results; }
            set { results = value; }
        }


        private DatatableBag allUsersInMyOrg;

        public DatatableBag AllUsersInMyOrg
        {
            get { return allUsersInMyOrg; }
            set { allUsersInMyOrg = value; }
        }

        private int savedCanvasId;

        public int SavedCanvasId
        {
            get { return savedCanvasId; }
            set { savedCanvasId = value; }
        }

        #endregion

        #region Methods

        // TODO: Add methods that will be called by the view

        public void SaveCanvas(CanvasDto canvasDto)
        {
            this.serviceAgent.SaveCanvas(canvasDto, SaveCanvasCompleted);
        }




        public void LoadCanvas(Guid canvasGUID)
        {
            this.serviceAgent.LoadCanvas(canvasGUID, LoadCanvasGUIDCompleted);
        }

        /// <summary>
        /// Loads the canvas GUID completed.
        /// </summary>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        private void LoadCanvasGUIDCompleted(CanvasDto canvasDto, Exception ex)
        {

            if (ex != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", ex);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                LoadedCanvas = canvasDto;



                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", ex);

                this.Notify(CanvasLoaded, notification);
            }


        }

        public void LoadCanvas(int canvasId)
        {
            this.serviceAgent.LoadCanvas(canvasId, LoadCanvasCompleted);
        }

        public void LoadCanvasForUserList(int UserId)
        {
            this.serviceAgent.LoadCanvasForUserList(UserId, LoadUserListCompleted);
        }

        public void ReadAllUsersInMyOrg(int orgId)
        {
            this.serviceAgent.ReadAllUsersInMyOrg(orgId, ReadAllUsersCompleted);
        }

        public void ShareCanvas(int canvasId, List<int> sharedUserIds)
        {
            this.serviceAgent.ShareCanvas(canvasId, sharedUserIds, ShareCanvasCompleted);
        }

        public void DeleteCanvas(int canvasId)
        {
            this.serviceAgent.DeleteCanvas(canvasId, DeleteCanvasCompleted);
        }

        public void ResendEmail(int canvasId, List<int> sharedUserIds)
        {
            this.serviceAgent.ResendEmail(canvasId, sharedUserIds, ResendEmailCompleted);
        }

        #endregion

        #region Completion Callbacks

        private void DeleteCanvasCompleted(bool result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(CanvasDeleted, notification);
            }
        }



        private void ShareCanvasCompleted(bool result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(CanvasShared, notification);
            }
        }

        private void ResendEmailCompleted(bool result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(EmailResent, notification);
            }
        }


        private void GetCanvasSharedStatusWithGuidCompleted(List<CanvasShareStatusDto> result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);

                this.CanvasShareStatusDtoList = result;
                this.Notify(CanvasShareStatusDtoLoaded, notification);
            }
        }


        private void GetCanvasSharedStatusCompleted(List<CanvasShareStatusDto> result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);

                this.CanvasShareStatusDtoList = result;
                this.Notify(CanvasShareStatusDtoLoaded, notification);
            }
        }


        private void ReadAllUsersCompleted(DatatableBag result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                AllUsersInMyOrg = null;
                AllUsersInMyOrg = result;
                this.Notify(AllUsersInMyOrgLoaded, notification);
            }
        }

        private void LoadUserListCompleted(DatatableBag result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                Results = null;
                Results = result;
                this.Notify(UserListLoaded, notification);
            }
        }

        private void LoadCanvasCompleted(CanvasDto result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                LoadedCanvas = result;

                LoadedXMLString = result.XmlData.ToString();
                UserIdOfOpenedCanvas = result.UserId;
                CanvasIdOfOpenedCanvas = result.CanvasId;
                IsCurrentCanvasShared = result.IsShared;
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(CanvasLoaded, notification);
            }
        }


        // TODO: Optionally add callback methods for async calls to the service agent


        private void SaveCanvasSnapshotCompleted(string result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                if (result != "")
                {

                    this.SavedCanvasSnapshotGuid = result;
                    this.Notify(CanvasSnapshotCompleted, notification);


                }


            }
        }


        private void SaveCanvasCompleted(int result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                if (result > 0)
                {
                    SavedCanvasId = result;
                    this.Notify(CanvasSaved, notification);
                }
                else
                {
                    if (result == -1) //Insert failed. Item already exists.
                    {
                        Exception exp = new Exception("Canvas Name already exists. Select another name.");
                        this.Notify(ErrorNotice, new NotificationEventArgs<Exception>("", exp));
                    }

                }

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