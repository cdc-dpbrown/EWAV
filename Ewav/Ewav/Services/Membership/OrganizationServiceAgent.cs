/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       OrganizationServiceAgent.cs
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
using System.Linq;
using Ewav.DTO;

namespace Ewav.Services
{
    /// <summary>
    /// Service agent for Ewav external data sources.  
    /// </summary>
    public class OrganizationServiceAgent : IOrganizationServiceAgent
    {
        #region Variables
        OrganizationDomainContext organizationCtx;
        private Action<int, Exception> _addCompleted;
        private Action<bool, Exception> _delCompleted;
        private Action<OrganizationDto, Exception> _readCompleted;
        private Action<bool, Exception> _updCompleted;
        private Action<List<OrganizationDto>, Exception> _readAllCompleted;


        #endregion

        #region Constructor


        private List<OrganizationDto> allOrganizations;

        public OrganizationServiceAgent()
        {
        }

        #endregion

        #region Helper Method

        #endregion

        #region Completion Callbacks

        #endregion

        /// <summary>
        /// Gets or sets all organizations.
        /// </summary>
        /// <value>All organizations.</value>
        public List<OrganizationDto> AllOrganizations
        {
            get
            {
                return this.allOrganizations;
            }
            set
            {
                this.allOrganizations = value;
            }
        }

        /// <summary>
        /// Deletes the specified organization id.
        /// </summary>
        /// <param name="organizationId">The organization id.</param>
        /// <param name="user">The user.</param>
        /// <param name="completed">The completed.</param>
        public void Delete(int organizationId, UserDTO user, Action<bool, Exception> completed)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads all.
        /// </summary>
        /// <param name="completed">The completed.</param>
        public void ReadAll(Action<List<OrganizationDto>, Exception> completed)
        {

            this._readAllCompleted = completed;

            organizationCtx = new OrganizationDomainContext();

            InvokeOperation<List<OrganizationDto>> readAllResults = organizationCtx.ReadAllOrganizations();

            readAllResults.Completed += new EventHandler(readAllResults_Completed);


        }

        void readAllResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<List<OrganizationDto>> result = (InvokeOperation<List<OrganizationDto>>)sender;
            Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                ex = result.Error;
            }

            this.AllOrganizations = result.Value;
            _readAllCompleted(AllOrganizations, ex);
        }

        /// <summary>
        /// Reads all.
        /// </summary>
        /// <param name="readAllCompleted">The read all completed.</param>
        public void ReadAll()
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        public void Add(UserOrganizationDto dto, Action<int, Exception> completed)
        {
            _addCompleted = completed;

            organizationCtx = new OrganizationDomainContext();

            InvokeOperation<int> addResults = organizationCtx.AddOrganization(dto);

            addResults.Completed += new EventHandler(addResults_Completed);
        }

        void addResults_Completed(object sender, EventArgs e)
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
            _addCompleted(returnedData, ex);
            //}
        }

        public void Update(OrganizationDto dto, Action<bool, Exception> completed)
        {
            _updCompleted = completed;

            organizationCtx = new OrganizationDomainContext();

            InvokeOperation<bool> updResults = organizationCtx.UpdateOrganization(dto);

            updResults.Completed += new EventHandler(updResults_Completed);
        }

        void updResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<bool> result =
                (InvokeOperation<bool>)sender;
            Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                //throw new GadgetException(result.Error.Message);
                ex = result.Error;
            }
            //else
            //{
            bool returnedData = ((InvokeOperation<bool>)sender).Value;
            _updCompleted(returnedData, ex);
        }

        public void Delete(int organizationId, Action<bool, Exception> completed)
        {
            _delCompleted = completed;

            organizationCtx = new OrganizationDomainContext();

            InvokeOperation<bool> delResults = organizationCtx.RemoveOrganization(organizationId);

            delResults.Completed += new EventHandler(delResults_Completed);
        }

        void delResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<bool> result =
               (InvokeOperation<bool>)sender;
            Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                //throw new GadgetException(result.Error.Message);
                ex = result.Error;
            }
            //else
            //{
            bool returnedData = ((InvokeOperation<bool>)sender).Value;
            _delCompleted(returnedData, ex);
        }

        public void Read(int organizationId, Action<OrganizationDto, Exception> completed)
        {
            //_readCompleted = completed;

            //organizationCtx = new OrganizationDomainContext();

            //InvokeOperation<List<OrganizationDto>> readResults = organizationCtx.ReadOrganization(null, organizationId);

            //readResults.Completed += new EventHandler(readResults_Completed);
        }

        void readResults_Completed(object sender, EventArgs e)
        {
            InvokeOperation<List<OrganizationDto>> result =
               (InvokeOperation<List<OrganizationDto>>)sender;
            Exception ex = null;
            if (result.HasError)
            {
                result.MarkErrorAsHandled();
                //throw new GadgetException(result.Error.Message);
                ex = result.Error;
            }
            //else
            //{
            OrganizationDto returnedData = ((InvokeOperation<OrganizationDto>)sender).Value;
            _readCompleted(returnedData, ex);
        }
    }
}