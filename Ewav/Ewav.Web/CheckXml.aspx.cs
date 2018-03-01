/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       CheckXml.aspx.cs
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

using Ewav;
using Ewav.BAL;

namespace Ewav.Web
{
    public partial class CheckXml : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {

            EntityManager em = new EntityManager();
            CanvasDto dto = em.LoadCanvas(Convert.ToInt32(TextBox1.Text));
             
                
            TextArea1.InnerText = "";
            TextArea1.InnerText = dto.XmlData.ToString();

             //     Response.Write(  dto.XmlData.ToString()      )    ;


        }
    }
}