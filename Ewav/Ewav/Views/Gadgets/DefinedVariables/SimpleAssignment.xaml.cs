/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       SimpleAssignment.xaml.cs
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
using Ewav.BAL;
using Ewav.ViewModels;
using Ewav.Web.EpiDashboard;
using Ewav.Web.EpiDashboard.Rules;
using Ewav.Views.Gadgets.DefinedVariables;
using Ewav.Web.Services;
//Serialize Method is written in Extensions.cs
namespace Ewav
{
    public partial class SimpleAssignment : ChildWindow, IEwavDashboardRule
    {
        public ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;
        bool editMode = false;
        public ListBoxItemSource SelectedItem { get; set; }
        ColumnDataType RuleType;
        /// <summary>
        /// Default Constuctor
        /// </summary>
        public SimpleAssignment()
        {
            InitializeComponent();
            FillSelectionComboBoxes();
            InitializeSimpleAssignment();
        }
        /// <summary>
        /// Parametrized Constructor for Edit functionality.
        /// </summary>
        /// <param name="editMode"></param>
        /// <param name="item"></param>
        public SimpleAssignment(bool editMode, ListBoxItemSource item)
        {
            this.editMode = editMode;
            SelectedItem = item;
            InitializeComponent();
            FillSelectionComboBoxes();
            for (int i = 0; i < applicationViewModel.EwavDefinedVariables.Count; i++)
            {
                EwavRule_SimpleAssignment rule = null;

                if (applicationViewModel.EwavDefinedVariables[i] is EwavRule_SimpleAssignment)
                {
                    rule = applicationViewModel.EwavDefinedVariables[i] as EwavRule_SimpleAssignment;
                }
                else
                {
                    continue;
                }
                if (SelectedItem.DestinationColumn == rule.TxtDestinationField)
                {
                    txtDestinationField.Text = rule.TxtDestinationField;
                    MyString assignType = new MyString();
                    assignType.VarName = GetString(rule.AssignmentType);
                    cbxAssignmentType.SelectedIndex = -1;
                    cbxAssignmentType.SelectedIndex = SearchColumnIndex(cbxAssignmentType.Items, assignType);

                    switch (rule.AssignmentType)
                    {
                        case SimpleAssignType.Substring:
                            txtParam2.Text = rule.Parameters[1].VarName;
                            txtParam3.Text = rule.Parameters[2].VarName;
                            break;
                        case SimpleAssignType.StringLength:
                            break;
                        case SimpleAssignType.FindText:
                            txtParam2.Text = rule.Parameters[1].VarName;
                            break;
                        case SimpleAssignType.Round:
                            txtParam2.Text = rule.Parameters[1].VarName;
                            break;
                        case SimpleAssignType.AddDays:
                            txtParam2.Text = rule.Parameters[1].VarName;
                            break;
                        default:
                            break;
                    }


                    cbxParam1.SelectedIndex = SearchColumnIndex(cbxParam1.Items, rule.Parameters[0]);

                    if (rule.Parameters.Count > 1)
                    {
                        cbxParam2.SelectedIndex = SearchColumnIndex(cbxParam2.Items, rule.Parameters[1]);
                    }
                    

                }
            }

        }
        /// <summary>
        /// Initializes Simple Assingment
        /// </summary>
        private void InitializeSimpleAssignment()
        {
            cbxParam1.Visibility = System.Windows.Visibility.Visible;
            cbxParam2.Visibility = System.Windows.Visibility.Visible;
            cbxParam3.Visibility = System.Windows.Visibility.Collapsed;

            lblParam1.Visibility = System.Windows.Visibility.Visible;
            lblParam2.Visibility = System.Windows.Visibility.Visible;
            lblParam3.Visibility = System.Windows.Visibility.Collapsed;

            //cbxParam3.Items.Clear();
            lblParam1.Content = SimpleAssignmentStrings.PARAM_START_DATE;
            lblParam2.Content = SimpleAssignmentStrings.PARAM_END_DATE;
            cbxParam1.SelectedIndex = 0;
            AddDescription();
        }


        /// <summary>
        /// Returns the String Value for type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetString(SimpleAssignType type)
        {
            switch (type)
            {
                case SimpleAssignType.CheckboxesMarkedYes:
                    break;
                case SimpleAssignType.YesNoMarkedYes:
                    break;
                case SimpleAssignType.AllBooleanMarkedYes:
                    break;
                case SimpleAssignType.YearsElapsed:
                    return "Difference in years";
                case SimpleAssignType.MonthsElapsed:
                    return "Difference in months";
                case SimpleAssignType.DaysElapsed:
                    return "Difference in days";
                case SimpleAssignType.HoursElapsed:
                    return "Difference in hours";
                case SimpleAssignType.MinutesElapsed:
                    return "Difference in minutes";
                case SimpleAssignType.TextToNumber:
                    return "Convert text data to numeric data";
                case SimpleAssignType.TextToDate:
                    return "Convert text data to date data";
                case SimpleAssignType.Substring:
                    return "Substring";
                case SimpleAssignType.StringLength:
                    return "Find the length of text data";
                case SimpleAssignType.FindText:
                    return "Find the location of text data";
                case SimpleAssignType.Round:
                    return "Round a number";
                case SimpleAssignType.Uppercase:
                    return "Convert text characters to uppercase";
                case SimpleAssignType.Lowercase:
                    return "Convert text characters to lowercase";
                case SimpleAssignType.AddDays:
                    return "Add days to a date field";
                case SimpleAssignType.DetermineNonExistantListValues:
                    return "Determine if a drop-down list field contains a value not present in its code table";
                case SimpleAssignType.CountCheckedCheckboxesInGroup:
                    break;
                case SimpleAssignType.CountYesMarkedYesNoFieldsInGroup:
                    break;
                case SimpleAssignType.DetermineCheckboxesCheckedInGroup:
                    break;
                case SimpleAssignType.DetermineYesMarkedYesNoFieldsInGroup:
                    break;
                case SimpleAssignType.CountNumericFieldsBetweenValuesInGroup:
                    break;
                case SimpleAssignType.CountNumericFieldsOutsideValuesInGroup:
                    break;
                case SimpleAssignType.FindSumNumericFieldsInGroup:
                    break;
                case SimpleAssignType.FindMeanNumericFieldsInGroup:
                    break;
                case SimpleAssignType.FindMaxNumericFieldsInGroup:
                    break;
                case SimpleAssignType.FindMinNumericFieldsInGroup:
                    break;
                case SimpleAssignType.CountFieldsWithMissingInGroup:
                    break;
                case SimpleAssignType.CountFieldsWithoutMissingInGroup:
                    break;
                case SimpleAssignType.DetermineFieldsWithMissingInGroup:
                    break;
                default:
                    break;
            }
            return string.Empty;
        }

        /// <summary>
        /// Searches for the index for the column.
        /// </summary>
        /// <param name="itemCollection"></param>
        /// <param name="myString"></param>
        /// <returns></returns>
        private int SearchColumnIndex(ItemCollection itemCollection, MyString myString)
        {
            int index = -1;
            for (int i = 0; i < itemCollection.Count; i++)
            {
                string columnNm = string.Empty;
                if (itemCollection[i] is EwavColumn)
                {
                    columnNm = ((EwavColumn)itemCollection[i]).Name.ToString();
                }
                else
                {
                    columnNm = ((ComboBoxItem)itemCollection[i]).Content.ToString();
                }


                if (myString.VarName == columnNm)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        /// <summary>
        /// Handles the addition of rule in EwavDefinedVariables 
        /// addition of rule in ListOfRules used to bind ListBox
        /// addition of newly created column.
        /// code for Group variables is commented as being out of scope.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            List<ListBoxItemSource> TempList = applicationViewModel.ListOfRules.Where(t => t.DestinationColumn.ToLower() == txtDestinationField.Text.ToLower()).ToList();

            Boolean columnNameExists = false;
            columnNameExists = applicationViewModel.EwavSelectedDatasource.AllColumns.Any(c => c.Name.ToLower() == txtDestinationField.Text.ToLower());

            //if ((TempList.Count > 0 && !editMode) || columnNameExists)
            //{
            //    MessageBox.Show("Variable name already exists.");
            //    return;
            //}


            if (string.IsNullOrEmpty(txtDestinationField.Text))
            {
                MessageBox.Show(SimpleAssignmentStrings.ERROR_DESTINATION_FIELD_MISSING);
                //this.DialogResult = false;
                return;
            }

            if (cbxAssignmentType.SelectedIndex < 0)
            {
                MessageBox.Show(SimpleAssignmentStrings.ERROR_TYPE_MISSING);
                //this.DialogResult = DialogResult.None;
                return;
            }

            //if (
            //    (cbxParam1.Visibility == System.Windows.Visibility.Visible && cbxParam1 != null && string.isnullorempty(((ewavcolumn)cbxparam1.selecteditem).name))
            //    || (cbxParam2.Visibility == System.Windows.Visibility.Visible && cbxParam1 != null && string.isnullorempty(((ewavcolumn)cbxparam2.selecteditem).name.tostring()))
            //    || (cbxParam3.Visibility == System.Windows.Visibility.Visible && cbxParam1 != null && string.isnullorempty(((ewavcolumn)cbxparam3.selecteditem).name.tostring()))
            //    )
            //{
            //    messagebox.show(simpleassignmentstrings.error_params_blank);
            //    //this.dialogresult = dialogresult.none;
            //    return;
            //}

            if (cbxParam1.Visibility == System.Windows.Visibility.Visible && cbxParam1.SelectedIndex < 1)
            {
                MessageBox.Show(SimpleAssignmentStrings.ERROR_PARAMS_BLANK);
                return;
            }
            if (cbxParam2.Visibility == System.Windows.Visibility.Visible && cbxParam2.SelectedIndex < 1)
            {
                MessageBox.Show(SimpleAssignmentStrings.ERROR_PARAMS_BLANK);
                return;
            }
            if (cbxParam3.Visibility == System.Windows.Visibility.Visible && cbxParam3.SelectedIndex < 1)
            {
                MessageBox.Show(SimpleAssignmentStrings.ERROR_PARAMS_BLANK);
                return;
            }

            if (txtParam2.Visibility == System.Windows.Visibility.Visible && txtParam2.Text == "")
            {
                MessageBox.Show(SimpleAssignmentStrings.ERROR_PARAMS_BLANK);
                return;
            }

            if (txtParam3.Visibility == System.Windows.Visibility.Visible && txtParam3.Text == "")
            {
                MessageBox.Show(SimpleAssignmentStrings.ERROR_PARAMS_BLANK);
                return;
            }

            if (!editMode)
            {
                //ColumnDataType columnDataType = ColumnDataType.Boolean | ColumnDataType.Numeric | ColumnDataType.Text;
                //foreach (string s in dashboardHelper.GetFieldsAsList(columnDataType))
                //{
                //    if (txtDestinationField.Text.ToLower().Equals(s.ToLower()))
                //    {
                //        MsgBox.ShowError(SimpleAssignmentStrings.ERROR_FIELD_ALREADY_EXISTS);
                //        this.DialogResult = DialogResult.None;
                //        return;
                //    }
                //}

                //foreach (IDashboardRule rule in dashboardHelper.Rules)
                //{
                //    if (rule is DataAssignmentRule)
                //    {
                //        DataAssignmentRule assignmentRule = rule as DataAssignmentRule;
                //        if (txtDestinationField.Text.ToLower().Equals(assignmentRule.DestinationColumnName.ToLower()))
                //        {
                //            MsgBox.ShowError(SimpleAssignmentStrings.ERROR_FIELD_ALREADY_EXISTS_WITH_RECODED_DATA);
                //            this.DialogResult = DialogResult.None;
                //            return;
                //        }
                //    }
                //}
            }

            string friendlyLabel = "Assign " + txtDestinationField.Text;

            string param1 = (cbxParam1.SelectedItem != null) ? ((EwavColumn)cbxParam1.SelectedItem).Name.ToString() : string.Empty;
            string param2 = (cbxParam2.SelectedItem != null) ? ((EwavColumn)cbxParam2.SelectedItem).Name.ToString() : txtParam2.Text;
            string param3 = (cbxParam3.SelectedItem != null) ? ((EwavColumn)cbxParam3.SelectedItem).Name.ToString() : txtParam3.Text;

            List<string> parameters = new List<string>();
            parameters.Add(param1);
            if (!string.IsNullOrEmpty(param2)) { parameters.Add(param2); }
            if (!string.IsNullOrEmpty(param3)) { parameters.Add(param3); }
            SimpleAssignType assignmentType = SimpleAssignType.YearsElapsed;

            //switch (cbxAssignmentType.SelectedItem.ToString())
            switch (cbxAssignmentType.SelectedIndex)
            {
                case 0: //"Difference in years":
                    friendlyLabel = friendlyLabel + " the difference in years between " + param1 + " and " + param2;
                    assignmentType = SimpleAssignType.YearsElapsed;
                    RuleType = ColumnDataType.Numeric;
                    break;
                case 1: //"Difference in months":
                    friendlyLabel = friendlyLabel + " the difference in months between " + param1 + " and " + param2;
                    assignmentType = SimpleAssignType.MonthsElapsed;
                    RuleType = ColumnDataType.Numeric;
                    break;
                case 2: //"Difference in days":
                    friendlyLabel = friendlyLabel + " the difference in days between " + param1 + " and " + param2;
                    assignmentType = SimpleAssignType.DaysElapsed;
                    RuleType = ColumnDataType.Numeric;
                    break;
                case 3: //"Difference in hours":
                    friendlyLabel = friendlyLabel + " the difference in hours between " + param1 + " and " + param2;
                    assignmentType = SimpleAssignType.HoursElapsed;
                    RuleType = ColumnDataType.Numeric;
                    break;
                case 4: //"Difference in minutes":
                    friendlyLabel = friendlyLabel + " the difference in minutes between " + param1 + " and " + param2;
                    assignmentType = SimpleAssignType.MinutesElapsed;
                    RuleType = ColumnDataType.Numeric;
                    break;
                case 5: //"Round a number":
                    friendlyLabel = friendlyLabel + " the rounded value of " + param1;
                    if (!string.IsNullOrEmpty(param2))
                    {
                        friendlyLabel = friendlyLabel + " to " + param2 + " decimal place(s)";
                    }
                    assignmentType = SimpleAssignType.Round;
                    RuleType = ColumnDataType.Numeric;
                    break;
                case 6: //"Convert text data to numeric data":
                    friendlyLabel = friendlyLabel + " the numeric representation of " + param1;
                    assignmentType = SimpleAssignType.TextToNumber;
                    RuleType = ColumnDataType.Numeric;
                    break;
                case 7: //"Find the length of text data":
                    friendlyLabel = friendlyLabel + " the length of the text contained in " + param1;
                    assignmentType = SimpleAssignType.StringLength;
                    RuleType = ColumnDataType.Numeric;
                    break;
                case 8: //"Find the location of text data":
                    friendlyLabel = friendlyLabel + " the starting location of the text " + param2 + " contained in " + param1;
                    assignmentType = SimpleAssignType.FindText;
                    RuleType = ColumnDataType.Numeric;
                    break;
                case 9: //"Substring":                    
                    friendlyLabel = friendlyLabel + " the portion of the text contained in " + param1 + " starting at position " + param2 + " and continuing for " + param3 + " characters";
                    assignmentType = SimpleAssignType.Substring;
                    RuleType = ColumnDataType.Text;
                    break;

                // New ones added after 7.0.9.48
                case 10: //"Convert text characters to uppercase":
                    friendlyLabel = friendlyLabel + " the upper case equivalent of " + param1;
                    assignmentType = SimpleAssignType.Uppercase;
                    RuleType = ColumnDataType.Text;
                    break;
                case 11: //"Convert text characters to lower":
                    friendlyLabel = friendlyLabel + " the lower case equivalent of " + param1;
                    assignmentType = SimpleAssignType.Lowercase;
                    RuleType = ColumnDataType.Text;
                    break;

                // New ones added after 7.0.9.51
                case 12: //"Add days to a date field":
                    friendlyLabel = friendlyLabel + " the date value in " + param1 + " and add " + param2 + " days";
                    assignmentType = SimpleAssignType.AddDays;
                    RuleType = ColumnDataType.DateTime;
                    break;
                case 13: //"Determine if a drop-down list field contains a value not present in its code table":
                    friendlyLabel = friendlyLabel + " a Yes if the value in " + param1 + " appears in its corresponding code table";
                    assignmentType = SimpleAssignType.DetermineNonExistantListValues;
                    RuleType = ColumnDataType.Boolean;
                    break;
                case 14: //"Convert text data to date type data":
                    friendlyLabel = friendlyLabel + " the date representation of " + param1;
                    assignmentType = SimpleAssignType.TextToDate;
                    RuleType = ColumnDataType.DateTime;
                    break;
                //case 14: //"Convert text data to numeric data":
                //    friendlyLabel = friendlyLabel + " the numeric representation of " + param1;
                //    assignmentType = SimpleAssignType.TextToDate;
                //    RuleType = ColumnDataType.DateTime;
                //    break;
                //case 14: //"Count the number of checked checkboxes in a group":
                //    friendlyLabel = friendlyLabel + " the number of checked checkboxes in " + param1 + " (group field)";
                //    assignmentType = SimpleAssignType.CountCheckedCheckboxesInGroup;
                //    break;
                case 15: //"Count the number of Yes-marked Yes/No fields in a group":
                    friendlyLabel = friendlyLabel + " the number of Yes-marked Yes/No fields in " + param1 + " (group field)";
                    assignmentType = SimpleAssignType.CountYesMarkedYesNoFieldsInGroup;
                    break;
                case 16: //"Determine if more than N checkboxes are checked in a group":
                    friendlyLabel = friendlyLabel + " a Yes if more than " + param2 + " checkboxes are checked in " + param1 + " (group field)";
                    assignmentType = SimpleAssignType.DetermineCheckboxesCheckedInGroup;
                    break;
                case 17: //"Determine if more than N Yes/No fields are marked Yes in a group":
                    friendlyLabel = friendlyLabel + " a Yes if more than " + param2 + " Yes/No fields are marked Yes in " + param1 + " (group field)";
                    assignmentType = SimpleAssignType.DetermineYesMarkedYesNoFieldsInGroup;
                    break;
                case 18: //"Count the number of numeric fields with values between X and Y in a group":
                    friendlyLabel = friendlyLabel + " the number of numeric fields with values between (inclusive) " + param2 + " and " + param3 + " in " + param1 + " (group field)";
                    assignmentType = SimpleAssignType.CountNumericFieldsBetweenValuesInGroup;
                    break;
                case 19: //"Count the number of numeric fields with values outside X and Y in a group":
                    friendlyLabel = friendlyLabel + " the number of numeric fields with values outside " + param2 + " and " + param3 + " in " + param1 + " (group field)";
                    assignmentType = SimpleAssignType.CountNumericFieldsOutsideValuesInGroup;
                    break;
                //case 20: //"Find the sum of all numeric fields in a group":
                //    friendlyLabel = friendlyLabel + " the sum of all numeric fields in " + param1 + " (group field).";
                //    if (param2 == dashboardHelper.Config.Settings.RepresentationOfYes) friendlyLabel = friendlyLabel + " Include Yes/No fields.";
                //    else friendlyLabel = friendlyLabel + " Do not include Yes/No fields.";
                //    if (param3 == dashboardHelper.Config.Settings.RepresentationOfYes) friendlyLabel = friendlyLabel + " Include Comment Legal fields.";
                //    else friendlyLabel = friendlyLabel + " Do not include Comment Legal fields.";
                //    assignmentType = SimpleAssignType.FindSumNumericFieldsInGroup;
                //    break;
                //case 21: //"Find the mean of all numeric fields in a group":
                //    friendlyLabel = friendlyLabel + " the mean of all numeric fields in " + param1 + " (group field).";
                //    if (param2 == dashboardHelper.Config.Settings.RepresentationOfYes) friendlyLabel = friendlyLabel + " Include Yes/No fields.";
                //    else friendlyLabel = friendlyLabel + " Do not include Yes/No fields.";
                //    if (param3 == dashboardHelper.Config.Settings.RepresentationOfYes) friendlyLabel = friendlyLabel + " Include Comment Legal fields.";
                //    else friendlyLabel = friendlyLabel + " Do not include Comment Legal fields.";
                //    assignmentType = SimpleAssignType.FindMeanNumericFieldsInGroup;
                //    break;
                case 22: //"Find the maximum value of all numeric fields in a group":
                    friendlyLabel = friendlyLabel + " the maximum numeric value in " + param1 + " (group field)";
                    assignmentType = SimpleAssignType.FindMaxNumericFieldsInGroup;
                    break;
                case 23: //"Find the minimum value of all numeric fields in a group":
                    friendlyLabel = friendlyLabel + " the minimum numeric value in " + param1 + " (group field)";
                    assignmentType = SimpleAssignType.FindMinNumericFieldsInGroup;
                    break;
                case 24: //"Count the number of fields with missing values in a group":
                    friendlyLabel = friendlyLabel + " the number of fields with missing values in " + param1 + " (group field)";
                    assignmentType = SimpleAssignType.CountFieldsWithMissingInGroup;
                    break;
                case 25: //"Count the number of fields without missing values in a group":
                    friendlyLabel = friendlyLabel + " the number of fields without missing values in " + param1 + " (group field)";
                    assignmentType = SimpleAssignType.CountFieldsWithoutMissingInGroup;
                    break;
                case 26: //"Determine if more than N fields have missing values in a group":
                    friendlyLabel = friendlyLabel + " a Yes if more than " + param2 + " fields are missing in " + param1 + " (group field)";
                    assignmentType = SimpleAssignType.DetermineFieldsWithMissingInGroup;
                    break;

                
            }

            //AssignRule = new Rule_SimpleAssign(this.dashboardHelper, friendlyLabel, txtDestinationField.Text, assignmentType, parameters);


            EwavRule_SimpleAssignment rule = new EwavRule_SimpleAssignment();

            rule.FriendlyLabel = friendlyLabel;
            rule.TxtDestinationField = txtDestinationField.Text;
            rule.AssignmentType = assignmentType;
            rule.VaraiableName = txtDestinationField.Text;
            var RuleDataType = RuleType; //ColumnDataType.Text;
            rule.VaraiableDataType = RuleDataType.ToString();;
 
            List<MyString> parameters_myString = new List<MyString>();
            for (int i = 0; i < parameters.Count; i++)
            {
                MyString mystr = new MyString();
                mystr.VarName = parameters[i];
                parameters_myString.Add(mystr);
            }
            rule.AssignmentType = assignmentType;
            rule.Parameters = parameters_myString;
            //rule.EwavRuleType = EwavRuleType.Simple;

            ListBoxItemSource listBoxItem = new ListBoxItemSource();
            listBoxItem.RuleString = friendlyLabel;
            listBoxItem.SourceColumn = null;
            listBoxItem.DestinationColumn = txtDestinationField.Text;
            listBoxItem.RuleType = EwavRuleType.Simple;
            listBoxItem.Rule = rule;

            EwavColumn newColumn = new EwavColumn();
            newColumn.Name = txtDestinationField.Text;
            newColumn.SqlDataTypeAsString = RuleDataType;//ColumnDataType.Text;
            newColumn.NoCamelName = txtDestinationField.Text;
            newColumn.IsUserDefined = true;

            //Shows the error message if name already exists.
            if (!editMode)
            {
                for (int i = 0; i < applicationViewModel.EwavDefinedVariables.Count; i++)
                {
                    if (applicationViewModel.EwavDefinedVariables[i].VaraiableName == rule.VaraiableName)
                    {
                        MessageBox.Show("Rule Name already exists. Select another name.");
                        return;
                    }
                }
            }

            applicationViewModel.InvokePreColumnChangedEvent();



            List<EwavRule_Base> rules = new List<EwavRule_Base>();
            rules = applicationViewModel.EwavDefinedVariables;

            for (int i = 0; i < rules.Count; i++)
            {
                if (rule.TxtDestinationField == rules[i].VaraiableName)
                {
                    rules.RemoveAt(i);
                    applicationViewModel.ListOfRules.RemoveAt(i);
                    break;
                }
            }

            if (!editMode)
            {
                applicationViewModel.EwavSelectedDatasource.AllColumns.Add(newColumn);
            }
            applicationViewModel.ListOfRules.Add(listBoxItem);
            rules.Add(rule);
            applicationViewModel.EwavDefinedVariables = rules;

            //this.DialogResult = DialogResult.OK;
            this.DialogResult = true;
        }

        /// <summary>
        /// Handles the Cancel Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        /// <summary>
        /// Fills the combo boxes on this dialog
        /// </summary>
        private void FillSelectionComboBoxes()
        {
            cbxParam1.ItemsSource = null;
            cbxParam2.ItemsSource = null;
            cbxParam3.ItemsSource = null;

            cbxParam2.Items.Clear();
            //cbxParam3.Items.Clear();


            List<EwavColumn> SourceColumns = this.applicationViewModel.EwavSelectedDatasource.AllColumns;

            List<ColumnDataType> columnDataType = new List<ColumnDataType>();
            columnDataType.Add(ColumnDataType.Numeric);

            IEnumerable<EwavColumn> CBXFieldCols = from cols in SourceColumns
                                                   where columnDataType.Contains(cols.SqlDataTypeAsString)
                                                   orderby cols.Name
                                                   select cols;

            List<EwavColumn> colsList = CBXFieldCols.ToList();

            colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            //this.cbxField.ItemsSource = colsList;
            //this.cbxField.SelectedValue = "Index";
            //this.cbxField.DisplayMemberPath = "Name";
            //cbxField.SelectedIndex = Index1;

            //List<string> fieldNames1 = dashboardHelper.GetFieldsAsList(columnDataType);
            //List<string> fieldNames2 = dashboardHelper.GetFieldsAsList(columnDataType);

            if (cbxAssignmentType.SelectedIndex >= 0)
            {
                switch (cbxAssignmentType.SelectedIndex)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        columnDataType.Clear();
                        columnDataType.Add(ColumnDataType.DateTime);

                        CBXFieldCols = from cols in SourceColumns
                                       where columnDataType.Contains(cols.SqlDataTypeAsString)
                                       orderby cols.Name
                                       select cols;

                        colsList = CBXFieldCols.ToList();

                        colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
                        cbxParam1.ItemsSource = colsList;
                        cbxParam1.SelectedValue = "Index";
                        cbxParam1.DisplayMemberPath = "Name";
                        cbxParam2.ItemsSource = colsList;
                        cbxParam2.SelectedValue = "Index";
                        cbxParam2.DisplayMemberPath = "Name";
                        break;
                    case 5:
                        columnDataType.Clear();
                        columnDataType.Add(ColumnDataType.Numeric);

                        CBXFieldCols = from cols in SourceColumns
                                       where columnDataType.Contains(cols.SqlDataTypeAsString)
                                       orderby cols.Name
                                       select cols;

                        colsList = CBXFieldCols.ToList();

                        colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });

                        //fieldNames1 = dashboardHelper.GetFieldsAsList(columnDataType);
                        cbxParam1.ItemsSource = colsList;
                        cbxParam1.SelectedValue = "Index";
                        cbxParam1.DisplayMemberPath = "Name";
                        break;
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 14:
                        //columnDataType = ColumnDataType.Text | ColumnDataType.UserDefined;
                        //fieldNames1 = dashboardHelper.GetFieldsAsList(columnDataType);
                        columnDataType.Clear();
                        columnDataType.Add(ColumnDataType.Text);

                        CBXFieldCols = from cols in SourceColumns
                                       where columnDataType.Contains(cols.SqlDataTypeAsString)
                                       orderby cols.Name
                                       select cols;

                        colsList = CBXFieldCols.ToList();

                        colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
                        cbxParam1.ItemsSource = colsList;
                        cbxParam1.SelectedValue = "Index";
                        cbxParam1.DisplayMemberPath = "Name";
                        break;
                    case 12:
                        //columnDataType = ColumnDataType.DateTime;
                        //fieldNames1 = dashboardHelper.GetFieldsAsList(columnDataType);
                        columnDataType.Clear();
                        columnDataType.Add(ColumnDataType.DateTime);

                        CBXFieldCols = from cols in SourceColumns
                                       where columnDataType.Contains(cols.SqlDataTypeAsString)
                                       orderby cols.Name
                                       select cols;

                        colsList = CBXFieldCols.ToList();

                        colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
                        cbxParam1.ItemsSource = colsList;
                        cbxParam1.SelectedValue = "Index";
                        cbxParam1.DisplayMemberPath = "Name";
                        break;
                    case 13:
                        //columnDataType = ColumnDataType.Text;
                        //fieldNames1 = dashboardHelper.GetFieldsAsList(columnDataType);
                        columnDataType.Clear();
                        columnDataType.Add(ColumnDataType.Text);

                        CBXFieldCols = from cols in SourceColumns
                                       where columnDataType.Contains(cols.SqlDataTypeAsString)
                                       orderby cols.Name
                                       select cols;

                        colsList = CBXFieldCols.ToList();

                        colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
                        cbxParam1.ItemsSource = colsList;
                        cbxParam1.SelectedValue = "Index";
                        cbxParam1.DisplayMemberPath = "Name";
                        break;
                    //case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                        //fieldNames1 = dashboardHelper.GetAllGroupsAsList();
                        //cbxParam1.DataSource = fieldNames1;
                        break;
                    case 20:
                    case 21:
                        //fieldNames1 = dashboardHelper.GetAllGroupsAsList();
                        //cbxParam1.DataSource = fieldNames1;
                        cbxParam2.Items.Add("Yes");//dashboardHelper.Config.Settings.RepresentationOfYes
                        cbxParam2.Items.Add("No");//dashboardHelper.Config.Settings.RepresentationOfNo);
                        cbxParam3.Items.Add("Yes");//dashboardHelper.Config.Settings.RepresentationOfYes);
                        cbxParam3.Items.Add("No");//dashboardHelper.Config.Settings.RepresentationOfNo);
                        break;
                }
            }
            //if (cbxAssignmentType.SelectedIndex >= 0 && cbxAssignmentType.SelectedIndex <= 4)
            //{
            //    ColumnDataType columnDataType = ColumnDataType.DateTime | ColumnDataType.UserDefined;                
            //    List<string> fieldNames1 = dashboardHelper.GetFieldsAsList(columnDataType);
            //    List<string> fieldNames2 = dashboardHelper.GetFieldsAsList(columnDataType);
            //    cbxParam1.DataSource = fieldNames1;
            //    cbxParam2.DataSource = fieldNames2;
            //}
            //else if (cbxAssignmentType.SelectedIndex == 5 || cbxAssignmentType.SelectedIndex == 7)
            //{
            //    ColumnDataType columnDataType = ColumnDataType.Numeric | ColumnDataType.UserDefined;                
            //    List<string> fieldNames = dashboardHelper.GetFieldsAsList(columnDataType);
            //    cbxParam1.DataSource = fieldNames;
            //}
            //else if (cbxAssignmentType.SelectedIndex == 6 || cbxAssignmentType.SelectedIndex == 8 || cbxAssignmentType.SelectedIndex == 9 || cbxAssignmentType.SelectedIndex == 10)
            //{
            //    ColumnDataType columnDataType = ColumnDataType.Text | ColumnDataType.UserDefined;
            //    List<string> fieldNames = dashboardHelper.GetFieldsAsList(columnDataType);
            //    cbxParam1.DataSource = fieldNames;
            //}

            if (editMode)
            {
                txtDestinationField.IsEnabled = false;
            }
            else
            {
                txtDestinationField.IsEnabled = true;
            }

            cbxParam1.SelectedIndex = -1;
            cbxParam2.SelectedIndex = -1;
            cbxParam3.SelectedIndex = -1;

            //if (AssignRule != null && AssignRule.AssignmentParameters != null)
            //{
            //    if (AssignRule.AssignmentParameters.Count > 0)
            //    {
            //        cbxParam1.Text = AssignRule.AssignmentParameters[0];
            //    }
            //    if (AssignRule.AssignmentParameters.Count > 1)
            //    {
            //        cbxParam2.Text = AssignRule.AssignmentParameters[1];
            //    }
            //    if (AssignRule.AssignmentParameters.Count > 2)
            //    {
            //        cbxParam3.Text = AssignRule.AssignmentParameters[2];
            //    }
            //}
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event for the assign type drop-down list
        /// </summary>
        /// <param name="sender">Object that fired the event</param>
        /// <param name="e">.NET supplied event parameters</param>
        private void cbxAssignmentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxAssignmentType == null)
            {
                return;
            }
            if (cbxAssignmentType.SelectedIndex == -1)
            {
                return;
            }

            //switch (cbxAssignmentType.SelectedItem.ToString())
            switch (cbxAssignmentType.SelectedIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    cbxParam1.Visibility = System.Windows.Visibility.Visible;
                    cbxParam2.Visibility = System.Windows.Visibility.Visible;
                    cbxParam3.Visibility = System.Windows.Visibility.Collapsed;

                    txtParam2.Visibility = System.Windows.Visibility.Collapsed;
                    txtParam3.Visibility = System.Windows.Visibility.Collapsed;

                    lblParam1.Visibility = System.Windows.Visibility.Visible;
                    lblParam2.Visibility = System.Windows.Visibility.Visible;
                    lblParam3.Visibility = System.Windows.Visibility.Collapsed;

                    //cbxParam3.Items.Clear();
                    lblParam1.Content = SimpleAssignmentStrings.PARAM_START_DATE;
                    lblParam2.Content = SimpleAssignmentStrings.PARAM_END_DATE;
                    break;
                case 8: // "Find the location of text data":
                    cbxParam1.Visibility = System.Windows.Visibility.Visible;
                    //cbxParam2.Visibility = System.Windows.Visibility.Visible;
                    txtParam2.Visibility = System.Windows.Visibility.Visible;
                    cbxParam3.Visibility = System.Windows.Visibility.Collapsed;
                    txtParam3.Visibility = System.Windows.Visibility.Collapsed;
                    lblParam1.Visibility = System.Windows.Visibility.Visible;
                    lblParam2.Visibility = System.Windows.Visibility.Visible;
                    lblParam3.Visibility = System.Windows.Visibility.Collapsed;

                    //cbxParam3.Items.Clear();// = string.Empty;
                    lblParam1.Content = SimpleAssignmentStrings.PARAM_TEXT_FIELD_TO_SEARCH;
                    lblParam2.Content = SimpleAssignmentStrings.PARAM_TEXT_TO_SEARCH_FOR;
                    break;
                case 5: // "Round a number":
                    cbxParam1.Visibility = System.Windows.Visibility.Visible;
                    cbxParam2.Visibility = System.Windows.Visibility.Collapsed;
                    txtParam2.Visibility = System.Windows.Visibility.Visible;
                    cbxParam3.Visibility = System.Windows.Visibility.Collapsed;
                    txtParam3.Visibility = System.Windows.Visibility.Collapsed;
                    lblParam1.Visibility = System.Windows.Visibility.Visible;
                    lblParam2.Visibility = System.Windows.Visibility.Visible;
                    lblParam3.Visibility = System.Windows.Visibility.Collapsed;

                    //cbxParam3.Items.Clear();// = string.Empty;
                    lblParam1.Content = SimpleAssignmentStrings.PARAM_NUMERIC_FIELD_TO_ROUND;
                    lblParam2.Content = SimpleAssignmentStrings.PARAM_ROUND_TO;
                    break;
                case 9: // "Substring":
                    cbxParam1.Visibility = System.Windows.Visibility.Visible;
                    cbxParam2.Visibility = System.Windows.Visibility.Collapsed;
                    cbxParam3.Visibility = System.Windows.Visibility.Collapsed;
                    txtParam2.Visibility = System.Windows.Visibility.Visible;
                    txtParam3.Visibility = System.Windows.Visibility.Visible;

                    lblParam1.Visibility = System.Windows.Visibility.Visible;
                    lblParam2.Visibility = System.Windows.Visibility.Visible;
                    lblParam3.Visibility = System.Windows.Visibility.Visible;

                    lblParam1.Content = SimpleAssignmentStrings.PARAM_TEXT_FIELD;
                    lblParam2.Content = SimpleAssignmentStrings.PARAM_FIRST_CHARACTER;
                    lblParam3.Content = SimpleAssignmentStrings.PARAM_NUMBER_OF_CHARACTERS;
                    break;
                case 6: // "Convert text data to numeric data":
                case 7: // "Find the length of text data":
                case 14: // "convert to date":
                    cbxParam1.Visibility = System.Windows.Visibility.Visible;
                    cbxParam2.Visibility = System.Windows.Visibility.Collapsed;
                    cbxParam3.Visibility = System.Windows.Visibility.Collapsed;

                    txtParam2.Visibility = System.Windows.Visibility.Collapsed;
                    txtParam3.Visibility = System.Windows.Visibility.Collapsed;

                    lblParam1.Visibility = System.Windows.Visibility.Visible;
                    lblParam2.Visibility = System.Windows.Visibility.Collapsed;
                    lblParam3.Visibility = System.Windows.Visibility.Collapsed;

                    cbxParam2.ItemsSource = null;
                    //cbxParam3.Items.Clear();// = string.Empty;
                    lblParam1.Content = SimpleAssignmentStrings.PARAM_TEXT_FIELD;
                    break;
                case 10: // "Convert text characters to uppercase":
                case 11: // "Convert text characters to lowercase":
                    cbxParam1.Visibility = System.Windows.Visibility.Visible;
                    cbxParam2.Visibility = System.Windows.Visibility.Collapsed;
                    cbxParam3.Visibility = System.Windows.Visibility.Collapsed;

                    txtParam2.Visibility = System.Windows.Visibility.Collapsed;
                    txtParam3.Visibility = System.Windows.Visibility.Collapsed;

                    lblParam1.Visibility = System.Windows.Visibility.Visible;
                    lblParam2.Visibility = System.Windows.Visibility.Collapsed;
                    lblParam3.Visibility = System.Windows.Visibility.Collapsed;

                    lblParam1.Content = SimpleAssignmentStrings.PARAM_TEXT_FIELD;
                    break;

                // New ones added
                case 12:
                    cbxParam1.Visibility = System.Windows.Visibility.Visible;
                    cbxParam2.Visibility = System.Windows.Visibility.Collapsed;
                    txtParam2.Visibility = System.Windows.Visibility.Visible;
                    cbxParam3.Visibility = System.Windows.Visibility.Collapsed;

                    lblParam1.Visibility = System.Windows.Visibility.Visible;
                    lblParam2.Visibility = System.Windows.Visibility.Visible;
                    lblParam3.Visibility = System.Windows.Visibility.Collapsed;

                    lblParam1.Content = SimpleAssignmentStrings.PARAM_DATE_FIELD;
                    lblParam2.Content = SimpleAssignmentStrings.PARAM_DAYS_TO_ADD;
                    break;

                case 13:
                    cbxParam1.Visibility = System.Windows.Visibility.Visible;
                    cbxParam2.Visibility = System.Windows.Visibility.Collapsed;
                    cbxParam3.Visibility = System.Windows.Visibility.Collapsed;

                    txtParam2.Visibility = System.Windows.Visibility.Collapsed;
                    txtParam3.Visibility = System.Windows.Visibility.Collapsed;

                    lblParam1.Visibility = System.Windows.Visibility.Visible;
                    lblParam2.Visibility = System.Windows.Visibility.Collapsed;
                    lblParam3.Visibility = System.Windows.Visibility.Collapsed;

                    lblParam1.Content = SimpleAssignmentStrings.PARAM_DDL_FIELD;
                    break;

                //case 14:
                case 15:
                case 22:
                case 23:
                case 24:
                case 25:
                    cbxParam1.Visibility = System.Windows.Visibility.Visible;
                    cbxParam2.Visibility = System.Windows.Visibility.Collapsed;
                    cbxParam3.Visibility = System.Windows.Visibility.Collapsed;

                    lblParam1.Visibility = System.Windows.Visibility.Visible;
                    lblParam2.Visibility = System.Windows.Visibility.Collapsed;
                    lblParam3.Visibility = System.Windows.Visibility.Collapsed;

                    lblParam1.Content = SimpleAssignmentStrings.PARAM_GROUP_FIELD;
                    break;
                case 20:
                case 21:
                    cbxParam1.Visibility = System.Windows.Visibility.Visible;
                    cbxParam2.Visibility = System.Windows.Visibility.Visible;
                    cbxParam3.Visibility = System.Windows.Visibility.Visible;

                    lblParam1.Visibility = System.Windows.Visibility.Visible;
                    lblParam2.Visibility = System.Windows.Visibility.Visible;
                    lblParam3.Visibility = System.Windows.Visibility.Visible;

                    lblParam1.Content = SimpleAssignmentStrings.PARAM_GROUP_FIELD;
                    lblParam2.Content = SimpleAssignmentStrings.PARAM_INCLUDE_YESNO;
                    lblParam3.Content = SimpleAssignmentStrings.PARAM_INCLUDE_COMMENTLEGAL;
                    break;
                case 16:
                case 17:
                case 26:
                    cbxParam1.Visibility = System.Windows.Visibility.Visible;
                    cbxParam2.Visibility = System.Windows.Visibility.Visible;
                    cbxParam3.Visibility = System.Windows.Visibility.Collapsed;

                    lblParam1.Visibility = System.Windows.Visibility.Visible;
                    lblParam2.Visibility = System.Windows.Visibility.Visible;
                    lblParam3.Visibility = System.Windows.Visibility.Collapsed;

                    lblParam1.Content = SimpleAssignmentStrings.PARAM_GROUP_FIELD;
                    lblParam2.Content = SimpleAssignmentStrings.PARAM_COUNT;
                    break;

                case 18:
                case 19:
                    cbxParam1.Visibility = System.Windows.Visibility.Visible;
                    cbxParam2.Visibility = System.Windows.Visibility.Visible;
                    cbxParam3.Visibility = System.Windows.Visibility.Visible;

                    lblParam1.Visibility = System.Windows.Visibility.Visible;
                    lblParam2.Visibility = System.Windows.Visibility.Visible;
                    lblParam3.Visibility = System.Windows.Visibility.Visible;

                    lblParam1.Content = SimpleAssignmentStrings.PARAM_GROUP_FIELD;
                    lblParam2.Content = SimpleAssignmentStrings.PARAM_LOWER_BOUND;
                    lblParam3.Content = SimpleAssignmentStrings.PARAM_UPPER_BOUND;
                    break;

                //case "Difference in years":
                //case "Difference in months":
                //case "Difference in days":
                //case "Difference in hours":
                //case "Difference in minutes":                
                //    cbxParam1.Visibility = System.Windows.Visibility.Visible;
                //    cbxParam2.Visibility = System.Windows.Visibility.Visible;
                //    cbxParam3.Visibility = System.Windows.Visibility.Collapsed;

                //    lblParam1.Visibility = System.Windows.Visibility.Visible;
                //    lblParam2.Visibility = System.Windows.Visibility.Visible;
                //    lblParam3.Visibility = System.Windows.Visibility.Collapsed;

                //    cbxParam3.Text = string.Empty;
                //    lblParam1.Text = "Start date:";
                //    lblParam2.Text = "End date:";
                //    break;
                //case "Find the location of text data":                
                //    cbxParam1.Visibility = System.Windows.Visibility.Visible;
                //    cbxParam2.Visibility = System.Windows.Visibility.Visible;
                //    cbxParam3.Visibility = System.Windows.Visibility.Collapsed;

                //    lblParam1.Visibility = System.Windows.Visibility.Visible;
                //    lblParam2.Visibility = System.Windows.Visibility.Visible;
                //    lblParam3.Visibility = System.Windows.Visibility.Collapsed;

                //    cbxParam3.Text = string.Empty;
                //    lblParam1.Text = "Text field to search:";
                //    lblParam2.Text = "The text string to search for:";
                //    break;
                //case "Round a number":
                //    cbxParam1.Visibility = System.Windows.Visibility.Visible;
                //    cbxParam2.Visibility = System.Windows.Visibility.Visible;
                //    cbxParam3.Visibility = System.Windows.Visibility.Collapsed;

                //    lblParam1.Visibility = System.Windows.Visibility.Visible;
                //    lblParam2.Visibility = System.Windows.Visibility.Visible;
                //    lblParam3.Visibility = System.Windows.Visibility.Collapsed;

                //    cbxParam3.Text = string.Empty;
                //    lblParam1.Text = "The numeric field to round:";
                //    lblParam2.Text = "To number of decimal places to round to:";
                //    break;
                //case "Substring":
                //    cbxParam1.Visibility = System.Windows.Visibility.Visible;
                //    cbxParam2.Visibility = System.Windows.Visibility.Visible;
                //    cbxParam3.Visibility = System.Windows.Visibility.Visible;

                //    lblParam1.Visibility = System.Windows.Visibility.Visible;
                //    lblParam2.Visibility = System.Windows.Visibility.Visible;
                //    lblParam3.Visibility = System.Windows.Visibility.Visible;

                //    lblParam1.Text = "Text field:";
                //    lblParam2.Text = "Position of the first character to extract:";
                //    lblParam3.Text = "Number of characters to extract:";
                //    break;
                //case "Convert text data to numeric data":
                //case "Find the length of text data":
                //    cbxParam1.Visibility = System.Windows.Visibility.Visible;
                //    cbxParam2.Visibility = System.Windows.Visibility.Collapsed;
                //    cbxParam3.Visibility = System.Windows.Visibility.Collapsed;

                //    lblParam1.Visibility = System.Windows.Visibility.Visible;
                //    lblParam2.Visibility = System.Windows.Visibility.Collapsed;
                //    lblParam3.Visibility = System.Windows.Visibility.Collapsed;

                //    cbxParam2.Text = string.Empty;
                //    cbxParam3.Text = string.Empty;
                //    lblParam1.Text = "Text field:";                    
                //    break;
                //case "Convert text characters to uppercase":
                //case "Convert text characters to lowercase":
                //    cbxParam1.Visibility = System.Windows.Visibility.Visible;
                //    cbxParam2.Visibility = System.Windows.Visibility.Collapsed;
                //    cbxParam3.Visibility = System.Windows.Visibility.Collapsed;

                //    lblParam1.Visibility = System.Windows.Visibility.Visible;
                //    lblParam2.Visibility = System.Windows.Visibility.Collapsed;
                //    lblParam3.Visibility = System.Windows.Visibility.Collapsed;

                //    lblParam1.Text = "Text field:";
                //    break;
            }

            AddDescription();

            FillSelectionComboBoxes();
        }

        /// <summary>
        /// Adds descriptive text to the dialog box
        /// </summary>
        private void AddDescription()
        {
            switch (cbxAssignmentType.SelectedIndex)
            {
                case 0:
                    txtDescription.Text = SimpleAssignmentStrings.YEARS_DESCRIPTION;
                    break;
                case 1:
                    txtDescription.Text = SimpleAssignmentStrings.MONTHS_DESCRIPTION;
                    break;
                case 2:
                    txtDescription.Text = SimpleAssignmentStrings.DAYS_DESCRIPTION;
                    break;
                case 3:
                    txtDescription.Text = SimpleAssignmentStrings.HOURS_DESCRIPTION;
                    break;
                case 4:
                    txtDescription.Text = SimpleAssignmentStrings.MINUTES_DESCRIPTION;
                    break;
                case 5:
                    txtDescription.Text = SimpleAssignmentStrings.ROUND_DESCRIPTION;
                    break;
                case 6: // "Convert text data to numeric data":
                    txtDescription.Text = SimpleAssignmentStrings.TEXT_TO_NUMBER_DESCRIPTION;
                    break;
                case 7: // "Find the length of text data":
                    txtDescription.Text = SimpleAssignmentStrings.STRLEN_DESCRIPTION;
                    break;
                case 8:
                    txtDescription.Text = SimpleAssignmentStrings.FINDTEXT_DESCRIPTION;
                    break;
                case 9:
                    txtDescription.Text = SimpleAssignmentStrings.SUBSTRING_DESCRIPTION;
                    break;
                case 10: // "Convert text characters to uppercase":
                    txtDescription.Text = SimpleAssignmentStrings.UPPERCASE_DESCRIPTION;
                    break;
                case 11: // "Convert text characters to lowercase":
                    txtDescription.Text = SimpleAssignmentStrings.LOWERCASE_DESCRIPTION;
                    break;
                case 12:
                    txtDescription.Text = SimpleAssignmentStrings.ADD_DAYS_DESCRIPTION;
                    break;
                case 13:
                    txtDescription.Text = SimpleAssignmentStrings.DDL_CHECK_DESCRIPTION;
                    break;
                case 14:
                    //txtDescription.Text = SimpleAssignmentStrings.COUNT_CHECKED_CHECKBOXES_DESCRIPTION;
                    txtDescription.Text = SimpleAssignmentStrings.TEXT_TO_DATE_DESCRIPTION;
                    break;
                case 15:
                    txtDescription.Text = SimpleAssignmentStrings.COUNT_YES_YESNOFIELDS_DESCRIPTION;
                    break;
                case 16:
                    txtDescription.Text = SimpleAssignmentStrings.DETERMINE_IF_N_CHECKBOXES_CHECKED_DESCRIPTION;
                    break;
                case 17:
                    txtDescription.Text = SimpleAssignmentStrings.DETERMINE_IF_N_YESNOFIELDS_DESCRIPTION;
                    break;
                case 18:
                    txtDescription.Text = SimpleAssignmentStrings.COUNT_NUMERIC_BETWEEN_X_Y_DESCRIPTION;
                    break;
                case 19:
                    txtDescription.Text = SimpleAssignmentStrings.COUNT_NUMERIC_OUTSIDE_X_Y_DESCRIPTION;
                    break;
                case 20:
                    txtDescription.Text = SimpleAssignmentStrings.SUM_NUMERIC_FIELDS_DESCRIPTION;
                    break;
                case 21:
                    txtDescription.Text = SimpleAssignmentStrings.AVERAGE_NUMERIC_FIELDS_DESCRIPTION;
                    break;
                case 22:
                    txtDescription.Text = SimpleAssignmentStrings.MAX_NUMERIC_FIELDS_DESCRIPTION;
                    break;
                case 23:
                    txtDescription.Text = SimpleAssignmentStrings.MIN_NUMERIC_FIELDS_DESCRIPTION;
                    break;
                case 24:
                    txtDescription.Text = SimpleAssignmentStrings.COUNT_MISSING_DESCRIPTION;
                    break;
                case 25:
                    txtDescription.Text = SimpleAssignmentStrings.COUNT_NOT_MISSING_DESCRIPTION;
                    break;
                case 26:
                    txtDescription.Text = SimpleAssignmentStrings.DETERMINE_IF_N_MISSING_DESCRIPTION;
                    break;
            }
        }

        void IEwavDashboardRule.CreateFromXml(System.Xml.Linq.XElement element)
        {
            throw new NotImplementedException();
        }
    }
}