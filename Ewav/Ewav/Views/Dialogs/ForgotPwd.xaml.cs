/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ForgotPwd.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.Windows;
using System.Windows.Controls;
using Ewav.ViewModels.Membership;
using Ewav.ExtensionMethods;
namespace Ewav
{
    public partial class ForgotPwd : ChildWindow
    {
        UserViewModel uvm = null;
        public ForgotPwd()
        {
            InitializeComponent();
            uvm = new UserViewModel();
        }

        /// <summary>
        /// Handles the Click event of the btnLoginReturn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void btnLoginReturn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            this.DialogResult = false;
            LogIn log = new LogIn();
            log.Show();
        }

        /// <summary>
        /// Handles the Click event of the CancelButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            LogIn lo = new LogIn();
            lo.Show();
        }

        /// <summary>
        /// Handles the Click event of the OKButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (tbEmail.Text == string.Empty)
            {
                spMsg.Visibility = System.Windows.Visibility.Visible;
            }

            else
            {
                //
                if (ValidateForm())
                {
                    OKButton.IsEnabled = false;
                    CancelButton.IsEnabled = false;
                    bsyInd.Visibility = System.Windows.Visibility.Visible;
                    uvm.ForgotPassword(tbEmail.Text.ToLower());
                    uvm.ForgotPasswordLoaded += new System.EventHandler(uvm_ForgotPasswordLoaded);
                    uvm.ForgotPasswordFailed += new System.EventHandler(uvm_ForgotPasswordFailed);

                }
                //else
                //{
                //    spMsg.Visibility = System.Windows.Visibility.Visible;
                //}
            }
        }

        /// <summary>
        /// Validates the form.
        /// </summary>
        /// <returns></returns>
        private bool ValidateForm() 
        {
            tbEmail.ClearValidationError();

            bool isFormValid = true;
            spMsg.Visibility = System.Windows.Visibility.Collapsed;

            if (!tbEmail.Text.IsEmailValid() || tbEmail.Text == "")
            {
                tbEmail.SetValidation("Please enter correct Email Address.");
                tbEmail.RaiseValidationError();
                isFormValid = false;
                spMsg.Visibility = System.Windows.Visibility.Visible;
            }


            return isFormValid;
        }

        /// <summary>
        /// Handles the ForgotPasswordFailed event of the uvm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        void uvm_ForgotPasswordFailed(object sender, System.EventArgs e)
        {
            //in case of error.
            bsyInd.Visibility = System.Windows.Visibility.Collapsed;
            spMsg.Visibility = System.Windows.Visibility.Collapsed;
            grdPwd.Visibility = System.Windows.Visibility.Collapsed;
            spMsg_Error.Visibility = System.Windows.Visibility.Visible;
            //spMsg.Visibility = System.Windows.Visibility.Visible;
            //tbErrMsg.Text = "Error updating information. Contact system administrator.";
            bsyInd.Visibility = System.Windows.Visibility.Collapsed;
            OKButton.IsEnabled = true;
            CancelButton.IsEnabled = true;
        }

        /// <summary>
        /// Handles the ForgotPasswordLoaded event of the uvm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        void uvm_ForgotPasswordLoaded(object sender, System.EventArgs e)
        {
            //this.DialogResult = true;

            //spMsg.Visibility = System.Windows.Visibility.Collapsed;
            //this.DialogResult = true;
            bsyInd.Visibility = System.Windows.Visibility.Collapsed;
            spMsg.Visibility = System.Windows.Visibility.Collapsed;
            grdPwd.Visibility = System.Windows.Visibility.Collapsed;
            spMsg_Success.Visibility = System.Windows.Visibility.Visible;



        }

        /// <summary>
        /// Handles the KeyDown event of the tbemail control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs" /> instance containing the event data.</param>
        private void tbemail_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
        	if (e.Key == System.Windows.Input.Key.Enter)
                {
                    OKButton_Click(sender, e);
                }
            

            if (e.Key == System.Windows.Input.Key.Escape)
            {
                btnLoginReturn_Click(sender, e);
            }

        }

        /// <summary>
        /// Handles the KeyDown event of the btnLoginReturn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs" /> instance containing the event data.</param>
        private void btnLoginReturn_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
           if (e.Key == System.Windows.Input.Key.Escape)
            {
                btnLoginReturn_Click(sender, e);
            }
        }


    }
}