/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       OrganizationDto.cs
 *  Namespace:  Ewav    
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
using Ewav.DTO;

namespace Ewav
{
    //This Data transfer object carries meta information about a given organization
    public class OrganizationDto
    {
        //private List<UserDTO> adminList = new List<UserDTO>();

        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public int TotalUserCount { get; set; }
        public int DatasourceCount { get; set; }
        public int AdminCount { get; set; }
        public int AnalystCount { get; set; }
        public int SuperAdminCount { get; set; }

        public string Description { get; set; }
        public int DatasourcesCount { get; set; }


        //public List<UserDTO> AdminList
        //{
        //    get
        //    {
        //        return adminList;
        //    }
        //    set
        //    {
        //        adminList = value;
        //    }
        //}
    }
}