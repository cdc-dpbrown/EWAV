/*  ----------------------------------------------------------------------------
 *  CDC
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       CloseGadget.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  David Brown
 *  Created:    2016/08/25    
 *  Summary:	     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Ewav.ViewModels;

namespace Ewav
{
    public partial class CloseGadget : ChildWindow
    {
        ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;
        UserControl _userControl;
        public CloseGadget(UserControl userControl)
        {
            InitializeComponent();
            _userControl = userControl;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            applicationViewModel.CloseGadget(_userControl);
            this.DialogResult = true;
        }
    }
}