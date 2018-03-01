/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       FillRangesDialog.cs
 *  Namespace:  EpiDashboard.Dialogs    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ewav.Web.EpiDashboard;
using Epi;
using Epi.Core;
using Epi.Fields;
using Epi.Windows;
using Epi.Windows.Dialogs;

namespace Ewav.Web.EpiDashboard.Dialogs
{
    public partial class FillRangesDialog : DialogBase
    {
        private int startValue;
        private int endValue;
        private int rangeValue;

        public FillRangesDialog()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // TODO: Clean this up later
            bool success = true;
            
            success = Int32.TryParse(txtStartValue.Text, out startValue);

            if (!success)
            {
                MsgBox.ShowError("Invalid number.");
                this.DialogResult = DialogResult.None;
                return;
            }

            success = Int32.TryParse(txtEndValue.Text, out endValue);

            if (!success)
            {
                MsgBox.ShowError("Invalid number.");
                this.DialogResult = DialogResult.None;
                return;
            }

            success = Int32.TryParse(txtRangeValue.Text, out rangeValue);

            if (!success)
            {
                MsgBox.ShowError("Invalid number.");
                this.DialogResult = DialogResult.None;
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

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
    }
}