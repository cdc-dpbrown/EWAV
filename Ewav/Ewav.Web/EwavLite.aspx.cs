/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       EwavLite.aspx.cs
 *  Namespace:  Ewav.Web    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ewav.BAL;

namespace Ewav.Web
{
    public partial class EwavLite : System.Web.UI.Page
    {

        private string thisUrl = "";

        private string snapshotGuid = "";


        protected void Page_Load(object sender, EventArgs e)
        {

            snapshotGuid = Request.QueryString["snapshot"];

            //pic.Src = "http://7777    ";

            //EntityManager em = new EntityManager();

            //string CanvasAs64 = em.GetCanvasSnapshot();        

           //     string snapshotGuid = Request.QueryString["snapshot"];
            //  src="getsnapshot.aspx?snapshotguid=83fcae26-61a7-4114-9e89-7fc64b302147"    


            snapshot.Src = "getsnapshot.aspx?snapshotGuid=" + snapshotGuid;    



        }
    }
}