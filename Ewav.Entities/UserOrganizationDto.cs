/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       UserOrganizationDto.cs
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
    /// TODO: Update summary.
    /// </summary>
    public class UserOrganizationDto
    {

        public OrganizationDto Organization { get; set; }
        public UserDTO User { get; set; }
        public bool Active { get; set; }
        public int RoleId { get; set; }
        public string RoleText {
            get {
                return Convert.ToString(Enum.Parse(typeof(RolesEnum), this.RoleId.ToString(), false));
            }
        }

    }
}