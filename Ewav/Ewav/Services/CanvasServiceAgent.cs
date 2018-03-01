/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       CanvasServiceAgent.cs
 *  Namespace:  Ewav.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using Ewav.Web.Services;
using System.ServiceModel.DomainServices.Client;
using Ewav.Web.EpiDashboard;
using Ewav.BAL;
using Ewav.ViewModels;
using Ewav.Web.Services.CanvasDomainService;
using Ewav.DTO;


namespace Ewav.Services
{
    /// <summary>
    /// Service agent for Ewav external data sources.  
    /// </summary>
    public class CanvasServiceAgent : ICanvasServiceAgent
    {
        #region Variables
        CanvasDomainContext canvasCtx;
        private Action<int, Exception> _completed;
        private Action<string, Exception> _completed2;
        private Action<DatatableBag, Exception> _userListLoadCompleted;
        private Action<CanvasDto, Exception> _loadCompleted;
        private Action<List<CanvasShareStatusDto>, Exception> _cssLoadCompleted;
        private Action<bool, Exception> _shareCompleted;
        private Action<bool, Exception> _sendEmailCompleted;
        private Action<DatatableBag, Exception> _readUsersCompleted;
        private Action<bool, Exception> _deleteCompleted;
        #endregion

        #region Constructor
        public CanvasServiceAgent()
        {
        }
        #endregion

        #region Helper Method

        #endregion

        #region Completion Callbacks

        #endregion


        public string snapshotGuid { get; set; }

        /// <summary>
        /// Loads the canvas.
        /// </summary>
        /// <param name="canvasGUID">The canvas GUID.</param>
        /// <param name="completed">The completed.</param>
        public void LoadCanvas(Guid canvasGUID, Action<CanvasDto, Exception> completed)
        {

            _loadCompleted = completed;
            canvasCtx = new CanvasDomainContext();

            InvokeOperation<CanvasDto> loadResults = canvasCtx.LoadCanvasGUID(canvasGUID);
            loadResults.Completed += new EventHandler(loadResultsGUID_Completed);



        }

        public void GetCanvasSharedStatusWithGuid(string canvasGUID, Action<List<CanvasShareStatusDto>, Exception> completed)
        {

            _cssLoadCompleted = completed;
            canvasCtx = new CanvasDomainContext();

            InvokeOperation<List<CanvasShareStatusDto>> cssLoadResults = canvasCtx.GetCanvasShareStatusGuid(canvasGUID);
            cssLoadResults.Completed += new EventHandler(cssLoadResults_Completed);



        }

        public void GetCanvasSharedStatus(int canvasId, int organizationId,
            Action<List<CanvasShareStatusDto>, Exception> completed)
        {

            _cssLoadCompleted = completed;
            canvasCtx = new CanvasDomainContext();

            InvokeOperation<List<CanvasShareStatusDto>> cssLoadResults = canvasCtx.GetCanvasShareStatus(canvasId, organizationId);
            cssLoadResults.Completed += new EventHandler(cssLoadResults_Completed);



        }

        void cssLoadResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<List<CanvasShareStatusDto>> result = (InvokeOperation<List<CanvasShareStatusDto>>)sender;
            Exception ex = null;

            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                ex = result.Error;
            }

            List<CanvasShareStatusDto> returnedData = ((InvokeOperation<List<CanvasShareStatusDto>>)sender).Value;
            _cssLoadCompleted(returnedData, ex);
        }


        /// <summary>
        /// Saves the canvas snapshot.
        /// </summary>
        /// <param name="canvasSnapshotAsBase64">The canvas snapshot as base64.</param>
        /// <param name="completed">The completed.</param>
        public void SaveCanvasSnapshot(CanvasDto cdto, Action<string, Exception> completed)
        {

            _completed2 = completed;

            canvasCtx = new CanvasDomainContext();

            InvokeOperation<string> canvasSnapshotResults = canvasCtx.SaveCanvasSnapshot(cdto);

            canvasSnapshotResults.Completed += new EventHandler(canvasSnapshotResults_Completed);

        }

        void canvasSnapshotResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<string> result = (InvokeOperation<string>)sender;

            Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                ex = result.Error;
            }
            string snapshotGuid = ((InvokeOperation<string>)sender).Value;
            _completed2(snapshotGuid, ex);


        }

        public void SaveCanvas(CanvasDto canvasDto, Action<int, Exception> completed)
        {
            _completed = completed;
            canvasCtx = new CanvasDomainContext();

            InvokeOperation<int> canvasResults = canvasCtx.SaveCanvas(canvasDto);

            canvasResults.Completed += new EventHandler(canvasResults_Completed);
        }

        public void LoadCanvasForUserList(int UserId, Action<DatatableBag, Exception> completed)
        {
            _userListLoadCompleted = completed;
            canvasCtx = new CanvasDomainContext();
            InvokeOperation<DatatableBag> results = canvasCtx.LoadCanvasListForUser(UserId);

            results.Completed += new EventHandler(results_Completed);
        }

        public void ReadAllUsersInMyOrg(int orgId, Action<DatatableBag, Exception> completed)
        {
            _readUsersCompleted = completed;
            canvasCtx = new CanvasDomainContext();
            InvokeOperation<DatatableBag> readResults = canvasCtx.ReadAllUsersInMyOrg(orgId);

            readResults.Completed += new EventHandler(readResults_Completed);
        }

        void readResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<DatatableBag> result =
                (InvokeOperation<DatatableBag>)sender;
            Exception ex = null;

            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                ex = result.Error;
            }

            DatatableBag returnedData = ((InvokeOperation<DatatableBag>)sender).Value;
            _readUsersCompleted(returnedData, ex);
        }

        public void LoadCanvas(int canvasId, Action<CanvasDto, Exception> completed)
        {
            _loadCompleted = completed;
            canvasCtx = new CanvasDomainContext();

            InvokeOperation<CanvasDto> loadResults = canvasCtx.LoadCanvas(canvasId);
            loadResults.Completed += new EventHandler(loadResults_Completed);
        }

        public void ShareCanvas(int canvasId, List<int> sharedUserIds, Action<bool, Exception> completed)
        {
            _shareCompleted = completed;
            canvasCtx = new CanvasDomainContext();

            InvokeOperation<bool> shareResults = canvasCtx.ShareCanvas(canvasId, sharedUserIds);
            shareResults.Completed += new EventHandler(shareResults_Completed);
        }

        public void ResendEmail(int canvasId, List<int> sharedUserIds, Action<bool, Exception> completed)
        {
            _sendEmailCompleted = completed;
            canvasCtx = new CanvasDomainContext();

            InvokeOperation<bool> sendEmailResults = canvasCtx.ResendEmail(canvasId, sharedUserIds);
            sendEmailResults.Completed += new EventHandler(sendEmailResults_Completed);
        }




        public void DeleteCanvas(int canvasId, Action<bool, Exception> completed)
        {
            _deleteCompleted = completed;
            canvasCtx = new CanvasDomainContext();

            InvokeOperation<bool> deleteResults = canvasCtx.DeleteCanvas(canvasId);
            deleteResults.Completed += new EventHandler(deleteResults_Completed);
        }

        void deleteResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<bool> result =
                 (InvokeOperation<bool>)sender;
            Exception ex = null;

            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                ex = result.Error;
            }

            bool returnedData = ((InvokeOperation<bool>)sender).Value;
            _deleteCompleted(returnedData, ex);
        }

        void shareResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<bool> result =
                (InvokeOperation<bool>)sender;
            Exception ex = null;

            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                ex = result.Error;
            }

            bool returnedData = ((InvokeOperation<bool>)sender).Value;
            _shareCompleted(returnedData, ex);
        }

        void sendEmailResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<bool> result =
                (InvokeOperation<bool>)sender;
            Exception ex = null;

            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                ex = result.Error;
            }

            bool returnedData = ((InvokeOperation<bool>)sender).Value;
            _sendEmailCompleted(returnedData, ex);
        }

        void loadResultsGUID_Completed(object sender, EventArgs e)
        {
            InvokeOperation<CanvasDto> result =
                (InvokeOperation<CanvasDto>)sender;
            Exception ex = null;

            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                ex = result.Error;
            }

            CanvasDto returnedData = result.Value;
            _loadCompleted(returnedData, ex);
        }


        void loadResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<CanvasDto> result =
                (InvokeOperation<CanvasDto>)sender;
            Exception ex = null;

            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                ex = result.Error;
            }

            CanvasDto returnedData = ((InvokeOperation<CanvasDto>)sender).Value;
            _loadCompleted(returnedData, ex);
        }

        void results_Completed(object sender, EventArgs e)
        {
            InvokeOperation<DatatableBag> result =
                (InvokeOperation<DatatableBag>)sender;
            Exception ex = null;

            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                ex = result.Error;
            }

            DatatableBag returnedData = ((InvokeOperation<DatatableBag>)sender).Value;
            _userListLoadCompleted(returnedData, ex);
        }

        void canvasResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<int> result =
               (InvokeOperation<int>)sender;

            Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                //throw new GadgetException(result.Error.Message);
                ex = result.Error;
            }
            //else
            //{
            int returnedData = ((InvokeOperation<int>)sender).Value;
            _completed(returnedData, ex);
            //}
        }







    }
}