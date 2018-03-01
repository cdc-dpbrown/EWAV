using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ewav.DTO
{
    public class CanvasShareStatusDto     //    SharedCanvasesDto
    {


        public int CanvasID { get; set; }
        public int OrganizationID { get; set; }
        public string OrganizationName { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UserID { get; set; }

        public bool Shared { get; set; }




    }
}
