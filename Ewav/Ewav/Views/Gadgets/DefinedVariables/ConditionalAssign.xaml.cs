/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ConditionalAssign.xaml.cs
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
using Ewav.Web.EpiDashboard;
using Ewav.BAL;
using Ewav.ViewModels;
using Ewav.Web.Services;
using Ewav.ExtensionMethods;
using CommonLibrary;
using System.Windows.Media.Imaging;
using System.Text;
using Ewav.Views.Gadgets;
//Serialize Method is written in Extensions.cs
//CreateFromXml is written in ApplicationViewModel.cs
namespace Ewav
{
    public partial class ConditionalAssign : ChildWindow, IEwavDashboardRule
    {
        bool editMode = false;
        List<EwavColumn> colsList;
        List<string> extractedList, operatorsList, configList;
        List<int> rowRemoved = new List<int>();
        List<EwavColumn> SourceColumns;
        int globalPointer, globalRemovedPointer = 0;
        public ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;
        StringBuilder conditionText = new StringBuilder();
        ListBoxItemSource SelectedItem;
        //EwavFilterControl ctrl;
        //EwavFilter UserControl;
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ConditionalAssign()
        {
            InitializeComponent();
            if (this.applicationViewModel.EwavSelectedDatasource.AllColumns != null)
            {
                FilterCtrl.FilterType = FilterControlType.Conditional;
                FilterCtrl.InitializeDataFilter();
            }
            FillConditionalComboboxes();
            txtElseValue.IsEnabled = false;
            cmbElseValue.IsEnabled = false;
        }

        /// <summary>
        /// Parametrized Constructor for Edit functionality.
        /// </summary>
        /// <param name="editMode"></param>
        /// <param name="item"></param>
        public ConditionalAssign(bool editMode, ListBoxItemSource item)
        {
            InitializeComponent();
            this.editMode = editMode;
            SelectedItem = item;
            FillConditionalComboboxes();
            EwavRule_ConditionalAssign ca = (EwavRule_ConditionalAssign)SelectedItem.Rule;

            txtDestinationField.IsEnabled = false;
            cbxFieldType.SelectedIndex = SearchColumnIndex(cbxFieldType.Items, ca.CbxFieldType.ToString());
            cbxFieldType.IsEnabled = false;

            switch (cbxFieldType.SelectedValue.ToString().ToUpper().Replace("/", ""))
            {
                case "YESNO":
                    cmbAssignValue.SelectedIndex = SearchColumnIndex(cmbAssignValue.Items, (ca.AssignValue == "True") ? "Yes" : "No");
                    if (ca.ElseValue.Length > 0)
                    {
                        cmbElseValue.SelectedIndex = SearchColumnIndex(cmbElseValue.Items, (ca.ElseValue == "True") ? "Yes" : "No");
                    }

                    if (cmbElseValue.SelectedIndex > 0)
                    {
                        checkboxUseElse.IsChecked = true;
                    }
                    break;
                default:
                    txtAssignValue.Text = ca.AssignValue;
                    txtElseValue.Text = ca.ElseValue;
                    if (ca.ElseValue.Length > 0)
                    {
                        checkboxUseElse.IsChecked = true;
                    }
                    break;
            }

            if (checkboxUseElse.IsChecked == true)
            {
                txtElseValue.IsEnabled = true;
            }
            else
            {
                txtElseValue.IsEnabled = false;
            }

            txtDestinationField.Text = ca.TxtDestination;

            //FillSelectionComboboxes();

            pnlGuidedMode.Visibility = System.Windows.Visibility.Visible;
            //pnlBtns.Visibility = System.Windows.Visibility.Visible;
            pnlAdvancedMode.Visibility = System.Windows.Visibility.Collapsed;
            pnlContainer.Visibility = System.Windows.Visibility.Visible;

            //ctrl = new EwavFilterControl();
            FilterCtrl.FilterType = FilterControlType.Conditional;
            FilterCtrl.ConstructStackPanelFromDataFilters(((EwavRule_ConditionalAssign)SelectedItem.Rule).ConditionsList);

            
        }

        /// <summary>
        /// Validates the controls.
        /// </summary>
        /// <param name="stackPanel"></param>
        /// <returns></returns>
        public Boolean ValidateControl()
        {
            if (txtDestinationField.Text == "")
            {
                MessageBox.Show("Source Column is empty.");
                return false;
            }

            return FilterCtrl.ValidateControl();
            
        }

        /// <summary>
        /// Validates stackpanel
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public bool ValidateStackPanel(StackPanel sp)
        {
            StackPanel stackPanel = sp;
            string spName = sp.Name;
            int j = Convert.ToInt32(spName.Substring(spName.IndexOf(",") + 1, spName.Length - spName.IndexOf(",") - 1));

            String s = string.Empty;

            for (int i = 0; i < stackPanel.Children.Count; i++)
            {
                if (stackPanel.Children[i] is TextBox && ((TextBox)stackPanel.Children[i]).Text == ""
                    && ((TextBox)stackPanel.Children[i]).IsEnabled)
                {
                    s += "No Data Entered in the Text Box" + "\n";
                }
                else if (stackPanel.Children[i] is ComboBox && ((ComboBox)stackPanel.Children[i]).SelectedIndex < 0
                    && ((ComboBox)stackPanel.Children[i]).IsEnabled)
                {
                    s += "No Selection has been made for combo Box" + "\n";
                }
                else if (stackPanel.Children[i] is DatePicker && ((DatePicker)stackPanel.Children[i]).Text == ""
                    && ((DatePicker)stackPanel.Children[i]).IsEnabled)
                {
                    s += "No Date selected." + "\n";
                }
            }
            if (s.Length > 0)
            {
                MessageBox.Show(s);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Searches for the index for the column.
        /// </summary>
        /// <param name="itemCollection"></param>
        /// <param name="myString"></param>
        /// <returns></returns>
        private int SearchColumnIndex(ItemCollection itemCollection, string myString)
        {
            int index = -1;

            for (int i = 0; i < itemCollection.Count; i++)
            {
                string columnNm = itemCollection[i].ToString().Replace("/", "");

                if (myString == columnNm)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        /// <summary>
        /// Private Helper method that Fills the ComboBoxes on Conditional Assignment
        /// </summary>
        private void FillConditionalComboboxes()
        {
            txtDestinationField.Text = string.Empty;
            cbxFieldType.Items.Clear();

            List<string> fieldNames = new List<string>();

            //ColumnDataType columnDataType = ColumnDataType.Boolean | ColumnDataType.Numeric | ColumnDataType.Text | ColumnDataType.DateTime | ColumnDataType.UserDefined;
            //fieldNames = dashboardHelper.GetFieldsAsList(columnDataType);

            //fieldNames.Sort();

            cbxFieldType.Items.Add("Text");
            cbxFieldType.Items.Add("Numeric");
            cbxFieldType.Items.Add("Yes/No");
            cbxFieldType.SelectedIndex = 0;

            cmbAssignValue.Items.Add("Yes"); //(dashboardHelper.Config.Settings.RepresentationOfYes);
            cmbAssignValue.Items.Add("No"); //(dashboardHelper.Config.Settings.RepresentationOfNo);

            cmbElseValue.Items.Add("Yes"); //(dashboardHelper.Config.Settings.RepresentationOfYes);
            cmbElseValue.Items.Add("No"); //(dashboardHelper.Config.Settings.RepresentationOfNo);
        }

        /// <summary>
        /// Event Handler for FieldType Selection Changed. 
        /// Sets the visibility of Fields based upon selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxFieldType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cbxFieldType.SelectedIndex)
            {
                case -1:
                    return;
                case 0:
                    txtAssignValue.Visibility = System.Windows.Visibility.Visible;
                    txtElseValue.Visibility = System.Windows.Visibility.Visible;
                    cmbAssignValue.Visibility = System.Windows.Visibility.Collapsed;
                    cmbElseValue.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case 1:
                    txtAssignValue.Visibility = System.Windows.Visibility.Visible;
                    txtElseValue.Visibility = System.Windows.Visibility.Visible;
                    cmbAssignValue.Visibility = System.Windows.Visibility.Collapsed;
                    cmbElseValue.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case 2:
                    txtAssignValue.Visibility = System.Windows.Visibility.Collapsed;
                    txtElseValue.Visibility = System.Windows.Visibility.Collapsed;
                    cmbAssignValue.Visibility = System.Windows.Visibility.Visible;
                    cmbElseValue.Visibility = System.Windows.Visibility.Visible;
                    break;
            }

            //cbxFieldType.Items.Add("Text");     0
            //cbxFieldType.Items.Add("Numeric");  1
            //cbxFieldType.Items.Add("Yes/No");   2
        }

        /// <summary>
        /// 
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


            if (ValidateControl())
            {
                List<EwavDataFilterCondition> listOfFilters = FilterCtrl.CreateDataFilters();

                string destinationColumnType = "System.String";

                ColumnDataType colType = ColumnDataType.Text;

                object elseValue = this.txtElseValue.Text;
                object assignValue = this.txtAssignValue.Text;
                cbxFieldTypeEnum fieldEnum = cbxFieldTypeEnum.None;
                switch (cbxFieldType.SelectedItem.ToString())
                {
                    case "Yes/No":
                        destinationColumnType = "System.Boolean";
                        if (cmbAssignValue.SelectedIndex == 0)
                        {
                            assignValue = true;
                        }
                        else if (cmbAssignValue.SelectedIndex == 1)
                        {
                            assignValue = false;
                        }

                        if (checkboxUseElse.IsChecked == false)
                        {
                            elseValue = "";
                        }
                        else if (cmbElseValue.SelectedIndex == 0)
                        {
                            elseValue = true;
                        }
                        else if (cmbElseValue.SelectedIndex == 1)
                        {
                            elseValue = false;
                        }
                        fieldEnum = cbxFieldTypeEnum.YesNo;
                        colType = ColumnDataType.Boolean;
                        break;
                    case "Text":
                        destinationColumnType = "System.String";
                        if (checkboxUseElse.IsChecked == false)
                        {
                            elseValue = "";
                        }
                        else
                        {
                            elseValue = this.txtElseValue.Text;
                        }

                        assignValue = this.txtAssignValue.Text;
                        fieldEnum = cbxFieldTypeEnum.Text;
                        colType = ColumnDataType.Text;
                        break;
                    case "Numeric":
                        destinationColumnType = "System.Decimal";
                        decimal decElse;
                        decimal decAssign;
                        if (checkboxUseElse.IsChecked == false)
                        {
                            elseValue = "";
                        }
                        else
                        {
                            bool success1 = Decimal.TryParse(this.txtElseValue.Text, out decElse);
                            if (success1) elseValue = decElse;
                        }

                        bool success2 = Decimal.TryParse(this.txtAssignValue.Text, out decAssign);
                        if (success2) assignValue = decAssign;

                        //if ((!success1 && checkboxUseElse.IsChecked) || !success2)
                        //{
                        //    Epi.Windows.MsgBox.ShowError("Invalid input detected.");
                        //    this.DialogResult = DialogResult.None;
                        //    return;
                        //}
                        fieldEnum = cbxFieldTypeEnum.Numeric;
                        colType = ColumnDataType.Numeric;
                        break;
                }

                string assignText = FilterCtrl.ConditionText.ToString().Substring(0, FilterCtrl.ConditionText.Length - 3);

                if (checkboxUseElse.IsChecked == true)
                {
                    assignText += ". Otherwise, assign " + txtDestinationField.Text + " the value " + elseValue + ".";
                }
                //conditionText = ( 

                ListBoxItemSource listBoxItem = new ListBoxItemSource();
                listBoxItem.RuleString = "Assign " + txtDestinationField.Text + " the value " + assignValue + " when " + assignText;
                listBoxItem.SourceColumn = null;
                listBoxItem.DestinationColumn = txtDestinationField.Text;
                listBoxItem.RuleType = EwavRuleType.conditional;

                EwavRule_ConditionalAssign rule = new EwavRule_ConditionalAssign();
                rule.TxtDestination = txtDestinationField.Text;
                rule.DestinationColumnType = destinationColumnType;
                rule.AssignValue = assignValue.ToString();
                rule.ElseValue = elseValue.ToString();
                rule.ConditionsList = listOfFilters;
                rule.CbxFieldType = fieldEnum;
                rule.VaraiableName = txtDestinationField.Text;
                rule.VaraiableDataType = colType.ToString();

                MyString tempStr = new MyString();
                tempStr.VarName = "Assign " + txtDestinationField.Text + " the value " + assignValue + " when " + assignText;
                rule.FriendlyRule = tempStr;

                EwavColumn newColumn = new EwavColumn();
                newColumn.Name = txtDestinationField.Text;
                newColumn.SqlDataTypeAsString = colType;// ColumnDataType.Text;
                newColumn.NoCamelName = txtDestinationField.Text;
                newColumn.IsUserDefined = true;

                applicationViewModel.InvokePreColumnChangedEvent();



                List<EwavRule_Base> rules = new List<EwavRule_Base>();
                rules = applicationViewModel.EwavDefinedVariables;
                listBoxItem.Rule = rule;
                //listBoxItem.FilterConditionsPanel = pnlContainer;

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

                for (int i = 0; i < rules.Count; i++)
                {
                    if (rule.TxtDestination == rules[i].VaraiableName)
                    {
                        rules[i] = rule;
                        //applicationViewModel.ListOfRules.RemoveAt(i);
                        applicationViewModel.ListOfRules[i] = listBoxItem;
                        break;
                    }
                }

                if (!editMode)
                {
                    applicationViewModel.EwavSelectedDatasource.AllColumns.Add(newColumn);
                    applicationViewModel.ListOfRules.Add(listBoxItem);
                    rules.Add(rule);
                }

                applicationViewModel.EwavDefinedVariables = rules;

                this.DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnIfCondition_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            pnlGuidedMode.Visibility = System.Windows.Visibility.Visible;
            //pnlBtns.Visibility = System.Windows.Visibility.Visible;
            pnlAdvancedMode.Visibility = System.Windows.Visibility.Collapsed;
            pnlContainer.Visibility = System.Windows.Visibility.Visible;

        }

        private void btnApply_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            pnlGuidedMode.Visibility = System.Windows.Visibility.Collapsed;
            //pnlBtns.Visibility = System.Windows.Visibility.Collapsed;
            pnlAdvancedMode.Visibility = System.Windows.Visibility.Visible;
        }

        private void checkboxUseElse_Checked(object sender, RoutedEventArgs e)
        {
            txtElseValue.IsEnabled = true;
            cmbElseValue.IsEnabled = true;
        }

        private void checkboxUseElse_Unchecked(object sender, RoutedEventArgs e)
        {
            txtElseValue.IsEnabled = false;
            cmbElseValue.IsEnabled = false;
        }

        void IEwavDashboardRule.CreateFromXml(System.Xml.Linq.XElement element)
        {
            throw new NotImplementedException();
        }
    }
}