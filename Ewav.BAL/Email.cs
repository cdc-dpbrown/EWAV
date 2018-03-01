/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       Email.cs
 *  Namespace:  Ewav.BAL    
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
using System.Configuration;

namespace Ewav.BAL
{
    public static class Email
    {



        public static bool SendMessage(string emailAddress, string subject, string body,
             bool debugEnabled = false)
        {
            try
            {
                bool isAuthenticated = false;
                bool isUsingSSL = false;
                int SMTPPort = 25;

                string AbsoluteUriEndPt = ConfigurationManager.AppSettings["EwavAbsoluteUriEndPt"].ToString();


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
                //  emailAddress = ConfigurationManager.AppSettings["EMAIL_TO"].ToString();
#endif
                //if (debugEnabled)
                //{
                //    emailAddress = ConfigurationManager.AppSettings["EMAIL_TO"].ToString();
                //}



                message.To.Add(emailAddress);
                message.Subject = subject;
                message.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["EMAIL_FROM"].ToString());
                int IndexOf = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.ToString().ToLower().IndexOf("services");
                body += string.Format("\n\n Link to Visualization Dashboard Home - \n " + System.Web.HttpContext.Current.Request.Url.AbsoluteUri.ToString().Substring(0, IndexOf) + AbsoluteUriEndPt);

                message.Body = body; // Add navigate to URL
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