/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       FillRanges.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Ewav
{
    public partial class FillRanges : ChildWindow
    {
        private int startValue;
        private int endValue;
        private int rangeValue;

        public int StartValue
        {
            get
            {
                return startValue;
            }
            private set
            {
                startValue = value;
            }
        }

        public int EndValue
        {
            get
            {
                return endValue;
            }
            private set
            {
                endValue = value;
            }
        }

        public int RangeValue
        {
            get
            {
                return rangeValue;
            }
            private set
            {
                rangeValue = value;
            }
        }

        public FillRanges()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
          // TODO: Clean this up later
            bool success = true;
            
            success = Int32.TryParse(txtStartValue.Text, out startValue);

            if (!success)
            {
                //MsgBox.ShowError("Invalid number.");
                //this.DialogResult = DialogResult.None;
                return;
            }

            success = Int32.TryParse(txtEndValue.Text, out endValue);

            if (!success)
            {
                //MsgBox.ShowError("Invalid number.");
                //this.DialogResult = DialogResult.None;
                return;
            }

            success = Int32.TryParse(txtRangeValue.Text, out rangeValue);

            if (!success)
            {
                //MsgBox.ShowError("Invalid number.");
                //this.DialogResult = DialogResult.None;
                return;
            }

            //this.DialogResult = DialogResult.OK;
            //this.Hide();
        
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}