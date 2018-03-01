/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       GetSnapshot.aspx.cs
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
using System.IO;

namespace Ewav.Web
{
    public partial class GetSnapshot : System.Web.UI.Page
    {

        private string snapshotGuid;    

        protected void Page_Load(object sender, EventArgs e)
        {

            snapshotGuid = Request.QueryString["snapshotGuid"];

    
            EntityManager em = new EntityManager();

            string CanvasAs64 = em.GetCanvasSnapshot(snapshotGuid);


            // to jpeg         
            byte[] out_Bytes = Convert.FromBase64String(CanvasAs64);


            MemoryStream outStreamJPEG = new MemoryStream(out_Bytes);


            Response.ContentType = "image/jpeg";

            Response.BinaryWrite(out_Bytes);    




        }
    }
}