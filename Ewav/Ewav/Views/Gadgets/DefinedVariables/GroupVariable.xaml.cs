/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       GroupVariable.xaml.cs
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
using System.Windows;
using System.Windows.Controls;
using Ewav.Web.EpiDashboard;
using Ewav.BAL;
using Ewav.ViewModels;
using Ewav.Web.Services;
using System.Text;

namespace Ewav
{
    public partial class GroupVariable : ChildWindow, IEwavDashboardRule
    {
        bool editMode = false;
        public ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;
        EwavRule_Format formatRule = null;
        public ListBoxItemSource SelectedItem { get; set; }
        /// <summary>
        /// Default Constructor
        /// </summary>
        public GroupVariable()
        {
            InitializeComponent();
            FillListBox();
        }

        public GroupVariable(bool editMode, ListBoxItemSource selectedItem = null)
        {
            SelectedItem = selectedItem;
            this.editMode = editMode;

           ClientCommon.Common cmnClass = new ClientCommon.Common();

            InitializeComponent();
            FillListBox();
            if (editMode)
            {
                EwavRule_GroupVariable rule = (EwavRule_GroupVariable)selectedItem.Rule;
                txtDestinationField.Text = rule.VaraiableName;
                txtDestinationField.IsEnabled = false;

                foreach (var item in rule.Items)
                {
                    //cbxField.SelectedIndex = cmnClass.FindIndexToSelect(cmnClass.GetItemsSource(GetFieldPrimaryDataType), child.Value.ToString().Replace("&lt;", "<"));
                    //int index = -1;
                    //lbxFieldName.ItemsSource
                    lbxFieldName.SelectedItems.Add( cmnClass.FindEwavColumn(item.VarName, ReadColumns()));
                }
            }
            

            

        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            EwavRule_GroupVariable rule = new EwavRule_GroupVariable();

            if (txtDestinationField.Text.Trim().Length == 0)
            {
                MessageBox.Show("Group Field Name must be entered.");
                return;
            }

            if (lbxFieldName.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select items to include in the group.");
                return;
            }

            List<MyString> lbxItems = new List<MyString>();

            StringBuilder listOfColumnNamesSelected = new StringBuilder();

            foreach (var item in lbxFieldName.SelectedItems)
            {
                MyString itemValue = new MyString();
                itemValue.VarName = ((EwavColumn)item).Name.ToString();
                lbxItems.Add(itemValue);
                listOfColumnNamesSelected.Append(itemValue.VarName + ",");
            }

            string friendlyLabel = "Create a variable group called " + txtDestinationField.Text + " and include: " + listOfColumnNamesSelected.ToString().Substring(0, listOfColumnNamesSelected.ToString().Length - 1);

            rule.Items = lbxItems;
            rule.VaraiableName = txtDestinationField.Text;
            rule.VaraiableDataType = ColumnDataType.GroupVariable.ToString();
            rule.FriendlyLabel = friendlyLabel;

      



            ListBoxItemSource listBoxItem = new ListBoxItemSource();
            listBoxItem.RuleString = friendlyLabel;
            //listBoxItem.SourceColumn = sourceColumn.Name;
            listBoxItem.DestinationColumn = txtDestinationField.Text;
            listBoxItem.NewColumn = txtDestinationField.Text;
            listBoxItem.RuleType = EwavRuleType.GroupVariable;
            listBoxItem.SourceColumn = null;
            listBoxItem.Rule = rule;

            EwavColumn newcolumn = new EwavColumn();
            newcolumn.Name = txtDestinationField.Text;
            newcolumn.SqlDataTypeAsString = ColumnDataType.GroupVariable;
            newcolumn.NoCamelName = txtDestinationField.Text;
            newcolumn.IsUserDefined = true;

            applicationViewModel.InvokePreColumnChangedEvent();

            List<EwavRule_Base> rules = new List<EwavRule_Base>();
            rules = applicationViewModel.EwavDefinedVariables;

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
                if (rule.VaraiableName == rules[i].VaraiableName)
                {
                    rules.RemoveAt(i);
                    applicationViewModel.ListOfRules.RemoveAt(i);
                    break;
                }
            }

            if (!editMode)
            {
                applicationViewModel.EwavSelectedDatasource.AllColumns.Add(newcolumn);
            }
            applicationViewModel.ListOfRules.Add(listBoxItem);
            List<EwavRule_Base> listOfRules = new List<EwavRule_Base>();
            listOfRules = applicationViewModel.EwavDefinedVariables;
            listOfRules.Add(rule);

            applicationViewModel.EwavDefinedVariables = listOfRules;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public void CreateFromXml(System.Xml.Linq.XElement element)
        {
            throw new NotImplementedException();
        }

        private void FillListBox()
        {
            lbxFieldName.Items.Clear();

            //List<string> fieldNames = new List<string>();

            // ColumnDataType columnDataType = ColumnDataType.DateTime;

            List<EwavColumn> colsList = ReadColumns();

            //colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            lbxFieldName.ItemsSource = colsList;
            lbxFieldName.SelectedValue = "Index";
            lbxFieldName.DisplayMemberPath = "Name";
            //lbxFieldName.SelectedIndex = 0;

            if (editMode)
            {
                lbxFieldName.SelectedItem = null;
            }
        }

        private List<EwavColumn> ReadColumns()
        {
            List<EwavColumn> SourceColumns = this.applicationViewModel.EwavSelectedDatasource.AllColumns;

            List<ColumnDataType> columnDataType = new List<ColumnDataType>();
            columnDataType.Add(ColumnDataType.DateTime);
            columnDataType.Add(ColumnDataType.Boolean);
            columnDataType.Add(ColumnDataType.Numeric);
            columnDataType.Add(ColumnDataType.Text);
            columnDataType.Add(ColumnDataType.UserDefined);

            IEnumerable<EwavColumn> CBXFieldCols = from cols in SourceColumns
                                                   where columnDataType.Contains(cols.SqlDataTypeAsString)
                                                   orderby cols.Name
                                                   select cols;

            List<EwavColumn> colsList = CBXFieldCols.ToList();
            return colsList;
        }
    }
}