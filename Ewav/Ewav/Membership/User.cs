/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       User.cs
 *  Namespace:  Ewav.Membership    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using Ewav.DTO;          

namespace Ewav.Membership
{
    /// <summary>
    ///  A singleton class that manages the user logged ihto the application     
    /// </summary>
    public class User
    {
        private static readonly User _instance = new User();
        /// <summary>
        /// the userDTO is a property of the user class to eliminate the need for extension methods 
        /// extension methods can be used to attach functionality that would not have been brought over through 
        /// RIA services, but this approach eliminates the need for extension methods and creates a more 
        /// manageable code hase  
        /// </summary>
        private UserDTO userDto;
        /// <summary>
        /// WARNING – This constructor cannot be public.  
        /// Making the constructor of this singleton class public will rip a hole in space-time, 
        /// surely destroying us all. -DS                               
        /// </summary>
        private User()
        {
            this.userDto = new UserDTO();
            userDto.PasswordHash = false.ToString();
        }

        /// <summary>
        /// Gets or sets the highest roles enum.
        /// </summary>
        /// <value>The highest roles enum.</value>
        public RolesEnum HighestRolesEnum { get; set; }

        /// <summary>
        ///  This is fired when the user changes to another one of the one or many orgs they may belong  to    
        /// </summary>
        public event EventHandler SelectedOrganizationChanged = delegate { };
        // This is necessary to ensure that the event is never null when  
        // it is raised elsewhere in the application   
        // http://www.dailycoding.com/Posts/avoiding_event__null_check.aspx    
        //
        public event EventHandler UserChanged = delegate { };
        /// <summary>
        /// 
        /// </summary>
        public static User Instance
        {
            get
            {
                return _instance;
            }
        }
        /// <summary>
        /// Gets or sets the roles enum.
        /// </summary>
        /// <value>The roles enum.</value>
        public RolesEnum RolesEnum { get; set; }
        /// <summary>
        /// Gets or sets the user dto.
        /// </summary>
        /// <value>The user dto.</value>
        public UserDTO UserDto
        {
            get
            {
                return this.userDto;
            }

            set
            {
                try
                {
                    if (value.UserID != this.userDto.UserID)
                    {
                        this.userDto = value;
                        // this.RolesEnum =  (RolesEnum)(Enum.Parse(typeof(RolesEnum), value.RoleValue.ToString(), true));
                        this.HighestRolesEnum = (RolesEnum)(Enum.Parse(typeof(RolesEnum), value.HighestRole.ToString(), true));
                        UserChanged(this, new EventArgs());
                    }
                }
                catch (NullReferenceException ex)
                {
                    throw new NullReferenceException("UserDTO is null", ex);    
                }
         
            }
        }
        /// <summary>
        /// Keeping the actual user dto "dumb" we set the value in the User class here and 
        /// raise the SelectedOrganizationChanged event  
        /// </summary>
        /// <param name="organizationId"></param>
        public void SetOrganizationId(int organizationId)
        {
            //this.userDto.OrganizationID = organizationId;
            SelectedOrganizationChanged(organizationId, null);
        }
    }
}