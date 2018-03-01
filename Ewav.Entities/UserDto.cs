/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       UserDto.cs
 *  Namespace:  Ewav.DTO    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
namespace Ewav.DTO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Ewav.Membership;

    /// <summary>
    /// 
    /// </summary>
    public class UserDTO   
    {


        
        /// <summary>
        /// The password hash
        /// </summary>
        private string passwordHash = false.ToString();

        /// <summary>
        /// The organization ID
        /// </summary>
        private int organizationID = -1;

        /// <summary>
        /// The update
        /// </summary>
        private bool Update;


        /// <summary>
        /// The highest role
        /// </summary>
        private int highestRole;

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        /// <value>
        /// The user ID.
        /// </value>
        public int UserID { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user role in organization.
        /// </summary>
        /// <value>
        /// The user role in organization.
        /// </value>
        public string UserRoleInOrganization { get; set; }

        /// <summary>
        /// Gets or sets the highest role across the organizations.
        /// </summary>
        /// <value>
        /// The highest role.
        /// </value>
        public int HighestRole
        {
            get
            {
                return this.highestRole;
            }
            set
            {
                this.highestRole = value;
            }
        }



        /// <summary>
        /// Gets or sets the organization ID.
        /// </summary>
        /// <value>
        /// The organization ID.
        /// </value>
        //public int OrganizationID
        //{
        //    get
        //    {
        //        return organizationID;
        //    }
        //    set
        //    {
        //        organizationID = value;
        //    }
        //}
        //public int SecurityLevelID { get; set; }
        //public int RoleValue { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get; set; }
        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName { get; set; }
        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        /// <value>
        /// The phone.
        /// </value>
        public string Phone { get; set; }
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets the temp id.
        /// </summary>
        /// <value>
        /// The temp id.
        /// </value>
        public Guid TempId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }
        //public int OriginalUserID { get; set; }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        public string FullName 
        { 
            get 
            {
                return this.FirstName + " " + this.LastName;
            } 
        }


        /// <summary>
        /// Gets or sets the password hash.
        /// </summary>
        /// <value>
        /// The password hash.
        /// </value>
        public string PasswordHash
        {
            get
            {
                return passwordHash;
            }
            set
            {
                passwordHash = value;
            }
        }



        /// <summary>
        /// Gets or sets a value indicating whether [should reset password].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [should reset password]; otherwise, <c>false</c>.
        /// </value>
        public bool ShouldResetPassword { get; set; }


        /// <summary>
        /// Gets or sets the datasource list.
        /// </summary>
        /// <value>
        /// The datasource list.
        /// </value>
        public List<DTO.DatasourceDto> DatasourceList { get; set; }

        //public string RoleText
        //{
        //    get
        //    {
        //        return Convert.ToString(Enum.Parse(typeof(RolesEnum), this.RoleValue.ToString(), false));
        //    }
        //}

        /// <summary>
        /// Gets or sets the datasource count.
        /// </summary>
        /// <value>
        /// The datasource count.
        /// </value>
        public int DatasourceCount { get; set; }

        /// <summary>
        /// Gets or sets the type of the user edit.
        /// </summary>
        /// <value>
        /// The type of the user edit.
        /// </value>
        public Ewav.DTO.UserEditType UserEditType { get;set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is existing user.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is existing user; otherwise, <c>false</c>.
        /// </value>
        public bool IsExistingUser { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum UserEditType
    {
        /// <summary>
        /// The editing password
        /// </summary>
        EditingPassword = 0,
        /// <summary>
        /// The editing user info
        /// </summary>
        EditingUserInfo
    }
}