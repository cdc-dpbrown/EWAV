using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace SilverlightApplication1.Web
{
    public class Global : System.Web.HttpApplication
    {


        static Ewav.Cache.EwavCache ewavCache;


        /// <summary>
        /// Gets or sets the P ewav cache.
        /// </summary>
        /// <value>The P ewav cache.</value>
        public static Ewav.Cache.EwavCache EwavCache
        {
            get
            {
                return ewavCache;
            }
            set
            {
                ewavCache = value;
            }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            //  create the cache     
      //       EwavCache = new Ewav.Cache.EwavCache("CM1");

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}