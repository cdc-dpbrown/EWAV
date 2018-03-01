/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       SimpleAssignDialog.cs
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
using Ewav.Web.EpiDashboard.Rules;

namespace Ewav.Web.EpiDashboard.Dialogs
{
    public partial class SimpleAssignDialog : DialogBase
    {
        #region Private Members
        private DashboardHelper dashboardHelper;
        private Rule_SimpleAssign assignRule;
        private bool editMode;
        #endregion // Private Members

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dashboardHelper">The dashboard helper to attach</param>
        public SimpleAssignDialog(DashboardHelper dashboardHelper)
        {
            InEditMode = false;
            this.dashboardHelper = dashboardHelper;
            InitializeComponent();

            cbxAssignmentType.SelectedIndex = 0;

            FillSelectionComboBoxes();

            this.txtDestinationField.Text = string.Empty;
        }

        /// <summary>
        /// Constructor used for editing an existing format rule
        /// </summary>
        /// <param name="dashboardHelper">The dashboard helper to attach</param>
        public SimpleAssignDialog(DashboardHelper dashboardHelper, Rule_SimpleAssign assignRule)
        {
            InEditMode = true;
            this.dashboardHelper = dashboardHelper;
            this.AssignRule = assignRule;
            InitializeComponent();

            SimpleAssignType assignType = assignRule.AssignmentType;

            switch (assignType)
            {
                case SimpleAssignType.YearsElapsed:
                    cbxAssignmentType.SelectedIndex = 0;
                    break;
                case SimpleAssignType.MonthsElapsed:
                    cbxAssignmentType.SelectedIndex = 1;
                    break;
                case SimpleAssignType.DaysElapsed:
                    cbxAssignmentType.SelectedIndex = 2;
                    break;
                case SimpleAssignType.HoursElapsed:
                    cbxAssignmentType.SelectedIndex = 3;
                    break;
                case SimpleAssignType.MinutesElapsed:
                    cbxAssignmentType.SelectedIndex = 4;
                    break;
                case SimpleAssignType.Round:
                    cbxAssignmentType.SelectedIndex = 5;
                    break;
                case SimpleAssignType.TextToNumber:
                    cbxAssignmentType.SelectedIndex = 6;
                    break;
                case SimpleAssignType.StringLength:
                    cbxAssignmentType.SelectedIndex = 7;
                    break;
                case SimpleAssignType.FindText:
                    cbxAssignmentType.SelectedIndex = 8;
                    break;
                case SimpleAssignType.Substring:
                    cbxAssignmentType.SelectedIndex = 9;
                    break;
                case SimpleAssignType.TextToDate:
                    cbxAssignmentType.SelectedIndex = 10;
                    break;
            }

            FillSelectionComboBoxes();

            this.txtDestinationField.Text = AssignRule.DestinationColumnName;            
            this.txtDestinationField.Enabled = false;
        }
        #endregion Constructors

        #region Public Properties
        /// <summary>
        /// Gets the format rule
        /// </summary>
        public Rule_SimpleAssign AssignRule
        {
            get
            {
                return this.assignRule;
            }
            private set
            {
                this.assignRule = value;
            }
        }
        #endregion Public Properties

        #region Private Properties
        /// <summary>
        /// Gets the EwavView associated with the attached dashboard helper
        /// </summary>
        private    Ewav.Web.EwavView   View
        {
            get
            {
                return this.dashboardHelper.View;
            }
        }

        /// <summary>
        /// Gets whether or not the rule is being edited
        /// </summary>
        private bool InEditMode
        {
            get
            {
                return this.editMode;
            }
            set
            {
                this.editMode = value;
            }
        }
        #endregion // Private Properties

        #region Private Methods
        /// <summary>
        /// Fills the combo boxes on this dialog
        /// </summary>
        private void FillSelectionComboBoxes()
        {          
            cbxParam1.DataSource = null;
            cbxParam2.DataSource = null;
            cbxParam3.DataSource = null;

            if (cbxAssignmentType.SelectedIndex >= 0 && cbxAssignmentType.SelectedIndex <= 4)
            {
                ColumnDataType columnDataType = ColumnDataType.DateTime | ColumnDataType.UserDefined;                
                List<string> fieldNames1 = dashboardHelper.GetFieldsAsList(columnDataType);
                List<string> fieldNames2 = dashboardHelper.GetFieldsAsList(columnDataType);
                cbxParam1.DataSource = fieldNames1;
                cbxParam2.DataSource = fieldNames2;
            }
            else if (cbxAssignmentType.SelectedIndex == 5 || cbxAssignmentType.SelectedIndex == 7)
            {
                ColumnDataType columnDataType = ColumnDataType.Numeric | ColumnDataType.UserDefined;                
                List<string> fieldNames = dashboardHelper.GetFieldsAsList(columnDataType);
                cbxParam1.DataSource = fieldNames;
            }
            else if (cbxAssignmentType.SelectedIndex == 6 || cbxAssignmentType.SelectedIndex == 8 || cbxAssignmentType.SelectedIndex == 9 || cbxAssignmentType.SelectedIndex == 10)
            {
                ColumnDataType columnDataType = ColumnDataType.Text | ColumnDataType.UserDefined;
                List<string> fieldNames = dashboardHelper.GetFieldsAsList(columnDataType);
                cbxParam1.DataSource = fieldNames;
            }

            if (editMode)
            {
                txtDestinationField.Enabled = false;
            }
            else
            {
                txtDestinationField.Enabled = true;
            }

            cbxParam1.SelectedIndex = -1;
            cbxParam2.SelectedIndex = -1;
            cbxParam3.SelectedIndex = -1;

            if (AssignRule != null && AssignRule.AssignmentParameters != null)
            {
                if (AssignRule.AssignmentParameters.Count > 0)
                {
                    cbxParam1.Text = AssignRule.AssignmentParameters[0];
                }
                if (AssignRule.AssignmentParameters.Count > 1)
                {
                    cbxParam2.Text = AssignRule.AssignmentParameters[1];
                }
                if (AssignRule.AssignmentParameters.Count > 2)
                {
                    cbxParam3.Text = AssignRule.AssignmentParameters[2];
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDestinationField.Text))
            {
                MsgBox.ShowError("Destination fields is blank.");
                this.DialogResult = DialogResult.None;
                return;
            }

            if (cbxAssignmentType.SelectedIndex < 0)
            {
                MsgBox.ShowError("Assignment type is blank.");
                this.DialogResult = DialogResult.None;
                return;
            }

            if (
                (cbxParam1.Visible == true && string.IsNullOrEmpty(cbxParam1.Text))
                || (cbxParam2.Visible == true && string.IsNullOrEmpty(cbxParam2.Text))
                || (cbxParam3.Visible == true && string.IsNullOrEmpty(cbxParam3.Text))
                )
            {
                MsgBox.ShowError("One or more required parameters are blank.");
                this.DialogResult = DialogResult.None;
                return;
            }

            if (!editMode)
            {
                ColumnDataType columnDataType = ColumnDataType.Boolean | ColumnDataType.Numeric | ColumnDataType.Text;
                foreach (string s in dashboardHelper.GetFieldsAsList(columnDataType))
                {
                    if (txtDestinationField.Text.ToLower().Equals(s.ToLower()))
                    {
                        MsgBox.ShowError("Destination fields name already exists as a column in this data set. Please use another name.");
                        this.DialogResult = DialogResult.None;
                        return;
                    }
                }

                foreach (IDashboardRule rule in dashboardHelper.Rules)
                {
                    if (rule is DataAssignmentRule)
                    {
                        DataAssignmentRule assignmentRule = rule as DataAssignmentRule;
                        if (txtDestinationField.Text.ToLower().Equals(assignmentRule.DestinationColumnName.ToLower()))
                        {
                            MsgBox.ShowError("Destination fields name already exists as a defined fields with recoded values. Please use another fields name.");
                            this.DialogResult = DialogResult.None;
                            return;
                        }
                    }
                }
            }

            string friendlyLabel = "Assign to " + txtDestinationField.Text;

            string param1 = cbxParam1.Text.ToString();
            string param2 = cbxParam2.Text.ToString();
            string param3 = cbxParam3.Text.ToString();

            List<string> parameters = new List<string>();
            parameters.Add(param1);
            if (!string.IsNullOrEmpty(param2)) { parameters.Add(param2); }
            if (!string.IsNullOrEmpty(param3)) { parameters.Add(param3); }
            SimpleAssignType assignmentType = SimpleAssignType.YearsElapsed;

            switch (cbxAssignmentType.SelectedItem.ToString())
            {
                case "Difference in years":
                    friendlyLabel = friendlyLabel + " the difference in years between " + param1 + " and " + param2;
                    assignmentType = SimpleAssignType.YearsElapsed;
                    break;
                case "Difference in months":
                    friendlyLabel = friendlyLabel + " the difference in months between " + param1 + " and " + param2;
                    assignmentType = SimpleAssignType.MonthsElapsed;
                    break;
                case "Difference in days":
                    friendlyLabel = friendlyLabel + " the difference in days between " + param1 + " and " + param2;
                    assignmentType = SimpleAssignType.DaysElapsed;
                    break;
                case "Difference in hours":
                    friendlyLabel = friendlyLabel + " the difference in hours between " + param1 + " and " + param2;
                    assignmentType = SimpleAssignType.HoursElapsed;
                    break;
                case "Difference in minutes":
                    friendlyLabel = friendlyLabel + " the difference in minutes between " + param1 + " and " + param2;
                    assignmentType = SimpleAssignType.MinutesElapsed;
                    break;
                case "Round a number":
                    friendlyLabel = friendlyLabel + " the rounded value of " + param1;
                    if (!string.IsNullOrEmpty(param2))
                    {
                        friendlyLabel = friendlyLabel + " to " + param2 + " decimal place(s)";
                    }
                    assignmentType = SimpleAssignType.Round;
                    break;
                case "Convert text data to numeric data":
                    friendlyLabel = friendlyLabel + " the numeric representation of " + param1;
                    assignmentType = SimpleAssignType.TextToNumber;
                    break;
                case "Convert text data to date data":
                    friendlyLabel = friendlyLabel + " the numeric representation of " + param1;
                    assignmentType = SimpleAssignType.TextToDate;
                    break;
                case "Find the length of text data":
                    friendlyLabel = friendlyLabel + " the length of the text contained in " + param1;
                    assignmentType = SimpleAssignType.StringLength;
                    break;
                case "Find the location of text data":
                    friendlyLabel = friendlyLabel + " the starting location of the text " + param2 + " contained in " + param1;
                    assignmentType = SimpleAssignType.FindText;
                    break;
                case "Substring":                    
                    friendlyLabel = friendlyLabel + " the portion of the text contained in " + param1 + " starting at position " + param2 + " and continuing for " + param3 + " characters";
                    assignmentType = SimpleAssignType.Substring;
                    break;
            }
            
            AssignRule = new Rule_SimpleAssign(this.dashboardHelper, friendlyLabel, txtDestinationField.Text, assignmentType, parameters);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion // Private Methods

        #region Event Handlers
        /// <summary>
        /// Handles the SelectedIndexChanged event for the assign type drop-down list
        /// </summary>
        /// <param name="sender">Object that fired the event</param>
        /// <param name="e">.NET supplied event parameters</param>
        private void cbxAssignmentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbxAssignmentType.SelectedIndex == -1) 
            {
                return;
            }

            switch (cbxAssignmentType.SelectedItem.ToString())
            {
                case "Difference in years":
                case "Difference in months":
                case "Difference in days":
                case "Difference in hours":
                case "Difference in minutes":                
                    cbxParam1.Visible = true;
                    cbxParam2.Visible = true;
                    cbxParam3.Visible = false;

                    lblParam1.Visible = true;
                    lblParam2.Visible = true;
                    lblParam3.Visible = false;

                    cbxParam3.Text = string.Empty;
                    lblParam1.Text = "Start date:";
                    lblParam2.Text = "End date:";
                    break;
                case "Find the location of text data":                
                    cbxParam1.Visible = true;
                    cbxParam2.Visible = true;
                    cbxParam3.Visible = false;

                    lblParam1.Visible = true;
                    lblParam2.Visible = true;
                    lblParam3.Visible = false;

                    cbxParam3.Text = string.Empty;
                    lblParam1.Text = "Text fields to search:";
                    lblParam2.Text = "The text string to search for:";
                    break;
                case "Round a number":
                    cbxParam1.Visible = true;
                    cbxParam2.Visible = true;
                    cbxParam3.Visible = false;

                    lblParam1.Visible = true;
                    lblParam2.Visible = true;
                    lblParam3.Visible = false;

                    cbxParam3.Text = string.Empty;
                    lblParam1.Text = "The numeric fields to round:";
                    lblParam2.Text = "To number of decimal places to round to:";
                    break;
                case "Substring":
                    cbxParam1.Visible = true;
                    cbxParam2.Visible = true;
                    cbxParam3.Visible = true;

                    lblParam1.Visible = true;
                    lblParam2.Visible = true;
                    lblParam3.Visible = true;
                    
                    lblParam1.Text = "Text fields:";
                    lblParam2.Text = "Position of the first character to extract:";
                    lblParam3.Text = "Number of characters to extract:";
                    break;
                case "Convert text data to numeric data":
                case "Find the length of text data":
                    cbxParam1.Visible = true;
                    cbxParam2.Visible = false;
                    cbxParam3.Visible = false;

                    lblParam1.Visible = true;
                    lblParam2.Visible = false;
                    lblParam3.Visible = false;

                    cbxParam2.Text = string.Empty;
                    cbxParam3.Text = string.Empty;
                    lblParam1.Text = "Text fields:";                    
                    break;                
            }

            FillSelectionComboBoxes();
        }
        #endregion // Event Handlers
    }
}