/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ResetPwd.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Ewav.Membership;
using Ewav.ViewModels.Membership;
using Ewav.ViewModels;
using Ewav.DTO;

namespace Ewav
{
    public partial class ResetPwd : ChildWindow
    {
        UserViewModel uvm = null;
        public ResetPwd()
        {
            InitializeComponent();
            uvm = new UserViewModel();

            uvm.ReadPasswordRules();
            uvm.PasswordRulesReadLoaded += new EventHandler(uvm_PasswordRulesReadLoaded);
        }

        private void btnBegin_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (tbNewPwd.Password == tbConfirmPwd.Password)
            {
                ResetPassword();

            }

            else
            {
                spMsg.Visibility = System.Windows.Visibility.Visible;
				 tbErrorMsg.Text = "The passwords do not match. Please try again.";
                 tbNewPwd.Password = "";
                 tbConfirmPwd.Password = "";
                //this.DialogResult = false;
            }

            //this.DialogResult = true;
            //if (spMsg.Visibility != System.Windows.Visibility.Visible)
            //{
            //    spMsg.Visibility = System.Windows.Visibility.Visible;
            //}
            //else
            //{
            //    //spMsg.Visibility = System.Windows.Visibility.Collapsed;
            //    //this.DialogResult = true;

            //}
        }

        void uvm_PasswordRulesReadLoaded(object sender, EventArgs e)
        {
            OKButton1.IsEnabled = true;
            passwordLengthMsg.Text = "Password must be at least " + uvm.PasswordRules.MinimumLength + " characters in length.";
            passwordTypeMsg.Text = "Password must be created using " + uvm.PasswordRules.NumberOfTypesRequiredInPassword + " following character types:";
            puncMsg.Text = "- characters are restricted to " + uvm.PasswordRules.Symbols ;
        }

        private void ResetPassword()
        {

            if (ValidatePassword(tbNewPwd.Password, uvm.PasswordRules))
            {
                UserOrganizationDto dto = new UserOrganizationDto();
                dto.User = User.Instance.UserDto;
                //dto.Organization = new OrganizationDto() { Id = ApplicationViewModel.Instance.EwavSelectedDatasource.OrganizationId };
                PasswordHasher ph = new PasswordHasher(ApplicationViewModel.Instance.KeyForUserPasswordSalt);

                string salt = ph.CreateSalt(User.Instance.UserDto.Email);
                User.Instance.UserDto.PasswordHash = ph.HashPassword(salt, tbNewPwd.Password);
                User.Instance.UserDto.UserEditType = UserEditType.EditingPassword;
                uvm.UpdateUser(dto);
                uvm.UserUpdated += new EventHandler(uvm_UserUpdated);
               // this.DialogResult = true;
                spMsg_Success.Visibility = System.Windows.Visibility.Visible;
                spFormatError.Visibility = System.Windows.Visibility.Collapsed;
                grdPwd.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                spFormatError.Visibility = System.Windows.Visibility.Visible;
                spMsg.Visibility = System.Windows.Visibility.Visible;

                tbErrorMsg.Text = "Password must match the Password Policy. Please try again.";
                tbNewPwd.Password = "";
                tbConfirmPwd.Password = "";
            }
        }

        public bool ValidatePassword(string password, PasswordRulesDTO dto)
        {
            int successCounter = 0;

            if (dto.UseSymbols && HasSymbol(password))
            {
                successCounter++;
            }

            if (dto.UseUpperCase && HasUpperCase(password))
            {
                successCounter++;
            }
            if (dto.UseLowerCase && HasLowerCase(password))
            {
                successCounter++;
            }
            if (dto.UseNumeric && HasNumber(password))
            {
                successCounter++;
            }

            if (dto.UseUserIdInPassword)
            {
                if (tbNewPwd.Password.ToString().Contains(User.Instance.UserDto.UserName.Split('@')[0].ToString()))
                {
                    successCounter = 0;
                }

            }

            if (dto.UseUserNameInPassword)
            {
                if (tbNewPwd.Password.ToString().Contains(User.Instance.UserDto.FirstName) || tbNewPwd.Password.ToString().Contains(User.Instance.UserDto.LastName))
                {
                    successCounter = 0;
                }
            }

            if (dto.NumberOfTypesRequiredInPassword <= successCounter && successCounter != 0)
            {
                return true;
            }

            return false;

        }



        public bool HasNumber(string password)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(password, @"\d");
        }

        public bool HasUpperCase(string password)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]");
        }

        public bool HasLowerCase(string password)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]");
        }

        public bool HasSymbol(string password)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(password, @"[" + uvm.PasswordRules.Symbols + "]");
        }



        void uvm_UserUpdated(object sender, EventArgs e)
        {
            if (sender == null)
            {
                spMsg.Visibility = System.Windows.Visibility.Collapsed;
                grdPwd.Visibility = System.Windows.Visibility.Collapsed;
                spMsg_Success.Visibility = System.Windows.Visibility.Visible;
                User.Instance.UserDto.ShouldResetPassword = false;
            }
            else
            {
                spMsg.Visibility = System.Windows.Visibility.Visible;
                tbErrorMsg.Text = "The passwords do not match. Please try again.";
            }
        }

        private void tbNewPwd_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (tbNewPwd.Password == tbConfirmPwd.Password)
            {

                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    ResetPassword();
                }

            }
        }

       
    }
}