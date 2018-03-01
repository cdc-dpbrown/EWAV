/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       Diagnostics.aspx.cs
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
using Ewav.DTO;
using Ewav.DAL.SqlServer;
using System.Data.SqlClient;
using System.Configuration;


namespace Ewav.Web
{
    public partial class Diagnostics : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

   

        }

        protected void Button1_Click(object sender, EventArgs e)
        {


            SqlServerUserDao dao = new SqlServerUserDao();
            dao.ConnectionString = txtConnStr.Text;


            System.Data.DataTable dt = dao.LoadUser(txtUser.Text);


            Response.Write(dt.Rows.Count.ToString() + "  record(s) retrieved!");



        }

        protected void Button2_Click(object sender, EventArgs e)
        {


            Email.SendMessage(txtEmail.Text, "Test message from " +
                HttpContext.Current.Request.Url.ToString() + "on " +
                HttpContext.Current.Request.ServerVariables["SERVER_NAME"].ToString(), " ", true, "Diagnostics.aspx");



        }

    }

    public static class Email
    {



        public static bool SendMessage(string emailAddress, string subject, string body,
             bool debugEnabled = false, string AbsoluteUriEndPt = "ewav.aspx")
        {
            try
            {
                bool isAuthenticated = false;
                bool isUsingSSL = false;
                int SMTPPort = 25;

                // App Config Settings:
                // EMAIL_USE_AUTHENTICATION [ True | False ] default is False
                // EMAIL_USE_SSL [ True | False] default is False
                // SMTP_HOST [ url or ip address of smtp server ]
                // SMTP_PORT [ port number to use ] default is 25
                // EMAIL_FROM [ email address of sender and authenticator ]
                // EMAIL_PASSWORD [ password of sender and authenticator ]


                string s = ConfigurationManager.AppSettings["EMAIL_USE_AUTHENTICATION"];
                if (!String.IsNullOrEmpty(s))
                {
                    if (s.ToUpper() == "TRUE")
                    {
                        isAuthenticated = true;
                    }
                }

                s = ConfigurationManager.AppSettings["EMAIL_USE_SSL"];
                if (!String.IsNullOrEmpty(s))
                {
                    if (s.ToUpper() == "TRUE")
                    {
                        isUsingSSL = true;
                    }
                }

                s = ConfigurationManager.AppSettings["SMTP_PORT"];
                if (!int.TryParse(s, out SMTPPort))
                {
                    SMTPPort = 25;
                }

                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                //message.To.Add(emailAddress);
#if (DEBUG)
                emailAddress = ConfigurationManager.AppSettings["EMAIL_TO"].ToString();
#endif
                if (debugEnabled)
                {
                    emailAddress = ConfigurationManager.AppSettings["EMAIL_TO"].ToString();
                }

                message.To.Add(emailAddress);
                message.Subject = subject;
                message.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["EMAIL_FROM"].ToString());
                int IndexOf = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.ToString().ToLower().IndexOf("services");
                body += string.Format("\n" + System.Web.HttpContext.Current.Request.Url.AbsoluteUri.ToString().Substring(0, IndexOf) + "ewav.aspx");

                message.Body = "  ";    //   body; // Add navigate to URL
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["SMTP_HOST"].ToString());

                smtp.Port = SMTPPort;

                if (isAuthenticated)
                {
                    smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["EMAIL_FROM"].ToString(), ConfigurationManager.AppSettings["EMAIL_PASSWORD"].ToString());
                }


                smtp.EnableSsl = isUsingSSL;


                smtp.Send(message);

                return true;

            }
            catch (Exception ex)
            {
                //return false;
                throw new Exception(ex.Message);
            }
        }


    }
}