/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       OrganizationViewModel.cs
 *  Namespace:  Ewav.ViewModels.Membership    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using Ewav.Services;
using SimpleMvvmToolkit;
using System.Collections.Generic;
using System.Linq;
using Ewav.DTO;
  
namespace Ewav.ViewModels.Membership
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class OrganizationViewModel : ViewModelBase<OrganizationViewModel>
    {
        #region Initialization and Cleanup

        // TODO: Add a member for IXxxServiceAgent
        private IOrganizationServiceAgent serviceAgent;

        private List<OrganizationDto> organizationDtoList;

        private int addedOrganizationId;

        // Default ctor
        public OrganizationViewModel()
        {
        }

        // TODO: ctor that accepts IXxxServiceAgent
        public OrganizationViewModel(IOrganizationServiceAgent serviceAgent)
        {
            this.serviceAgent = serviceAgent;
        }

        #endregion


        #region Notifications

        // TODO: Add events to notify the view or obtain data from the view
        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;
        public event EventHandler<NotificationEventArgs<Exception>> DtoRead;
        public event EventHandler<NotificationEventArgs<Exception>> DtoAdded;
        public event EventHandler<NotificationEventArgs<Exception>> DtoUpdated;        


        #endregion

        /// <summary>
        /// Gets or sets the added organization id.
        /// </summary>
        /// <value>The added organization id.</value>
        public int AddedOrganizationId
        {
            get
            {
                return this.addedOrganizationId;
            }
            set
            {
                this.addedOrganizationId = value;
            }
        }

        /// <summary>
        /// Gets or sets the organization dto list.
        /// </summary>
        /// <value>The organization dto list.</value>
        public List<OrganizationDto> OrganizationDtoList
        {
            get
            {
                return this.organizationDtoList;
            }
            set
            {
                this.organizationDtoList = value;
            }
        }

   
        #region Properties

        // TODO: Add properties using the mvvmprop code snippet

        #endregion

        #region Methods

        public void Add(UserOrganizationDto dto    )                      
        {
            OrganizationServiceAgent osa = new OrganizationServiceAgent();
            osa.Add(dto, AddCompleted);
        }


        public void Delete(OrganizationDto dto)
        {
            OrganizationServiceAgent osa = new OrganizationServiceAgent();
            osa.Delete(dto.Id, DeleteCompleted);
        }


        //public void Update(OrganizationDto dto)  
        //{
        //    //OrganizationDto dto = organizationDtoList.Single(x => x.Id == organizationID);    
        //    OrganizationServiceAgent osa = new OrganizationServiceAgent();
        //    osa.Update(dto , UpdateCompleted);
        //}


        public void Update(OrganizationDto dto)
        {
            OrganizationServiceAgent osa = new OrganizationServiceAgent();
            osa.Update(dto, UpdateCompleted);
        }

        /// <summary>
        /// Updates the completed.
        /// </summary>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        private void UpdateCompleted(bool result, Exception e)
        {

            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {


                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(DtoUpdated, notification);

            }
                
              
        }

        public void Read(OrganizationDto dto)
        {

            OrganizationServiceAgent osa = new OrganizationServiceAgent();
            osa.Read(dto.Id, ReadCompleted);


        }


        public void ReadAll()
        {

            OrganizationServiceAgent osa = new OrganizationServiceAgent();
            osa.ReadAll(ReadAllCompleted);

            //   777777777
        }




        #endregion

        #region Completion Callbacks



        private void AddCompleted(int result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {


                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.AddedOrganizationId  = result;
                this.Notify(DtoAdded,    notification);  

            }
                
              

        }



        private void ReadCompleted(OrganizationDto result, Exception e)
        {

        }

        private void ReadAllCompleted(List<OrganizationDto> result, Exception e)
        {
            if (e != null)
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.Notify(ErrorNotice, notification);
            }
            else
            {
                NotificationEventArgs<Exception> notification = new NotificationEventArgs<Exception>("", e);
                this.OrganizationDtoList = result;
                this.Notify(DtoRead, notification);
            }
                
              

        }

        private void DeleteCompleted(bool result, Exception e)
        {


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