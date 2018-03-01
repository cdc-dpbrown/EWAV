/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       AssignedExpression.xaml.cs
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


//Serialize Method is written in Extensions.cs
namespace Ewav
{
    public partial class AssignedExpression : ChildWindow, IEwavDashboardRule
    {
        public ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;

        List<EwavColumn> CBXFieldCols = new List<EwavColumn>();
        //private string childColumn;
        private bool editMode;
        //int index = -1;
        //private EwavColumn sourceColumn;
        //List<EwavColumn> SourceColumns;

        public AssignedExpression(bool editMode, ListBoxItemSource selectedItem = null)
        {
            SelectedItem = selectedItem;
            this.editMode = editMode;

            InitializeComponent();

            if (editMode)
            {
                txtDestinationField.Text = selectedItem.NewColumn;
                txtExpression.Text = selectedItem.AssignExpression;
           
            }
            //else
            //{
            CBXFieldCols.Add(new EwavColumn { Name = "Text", SqlDataTypeAsString = ColumnDataType.Text });
            CBXFieldCols.Add(new EwavColumn { Name = "Numeric", SqlDataTypeAsString = ColumnDataType.Numeric });


            cbxDataType.ItemsSource = CBXFieldCols;
            cbxDataType.DisplayMemberPath = "Name";

            if (editMode)
            {
                if (selectedItem.DataType == "Text")
                {
                    cbxDataType.SelectedIndex = 0;
                }
                else
                {
                    cbxDataType.SelectedIndex = 1;
                }
            }
            else
            {
                cbxDataType.SelectedIndex = 0;
            }


            //}
        }

        public ListBoxItemSource SelectedItem { get; set; }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            // add data to new rule add to  list       
            // add new rule to listbox item source   kk      
            //  add column to Allcolumns  

            List<ListBoxItemSource> TempList = applicationViewModel.ListOfRules.Where(t => t.DestinationColumn.ToLower() == txtDestinationField.Text.ToLower()).ToList();
            if (TempList.Count > 0 && !editMode)
            {
                MessageBox.Show("Variable name already exists.");
                return;
            }


            EwavRule_ExpressionAssign ea = new EwavRule_ExpressionAssign();
            ea.Expression = txtExpression.Text;

            if (((EwavColumn)cbxDataType.SelectedItem).SqlDataTypeAsString.ToString() == "Text")
            {
                ea.DataType = "System.String";

            }
            else if (((EwavColumn)cbxDataType.SelectedItem).SqlDataTypeAsString.ToString() == "Numeric")
            {
                ea.DataType = "System.Decimal";
            }

            //   "System.String";
            ea.FriendlyRule = string.Format("Assign {0} the expression: {1}", txtDestinationField.Text, txtExpression.Text);
            ea.DestinationColumnName = txtDestinationField.Text;
            ea.VaraiableName = txtDestinationField.Text;

            List<EwavRule_Base> rules = new List<EwavRule_Base>();
            rules = applicationViewModel.EwavDefinedVariables;

            //Shows the error message if name already exists.
            if (!editMode)
            {
                for (int i = 0; i < applicationViewModel.EwavDefinedVariables.Count; i++)
                {
                    if (applicationViewModel.EwavDefinedVariables[i].VaraiableName == ea.VaraiableName)
                    {
                        MessageBox.Show("Rule Name already exists. Select another name.");
                        return;
                    }
                }
            }

            for (int i = 0; i < rules.Count; i++)
            {
                if (ea.DestinationColumnName == rules[i].VaraiableName)
                {
                    rules.RemoveAt(i);
                    applicationViewModel.ListOfRules.RemoveAt(i);
                    break;
                }
            }

            //EwavRule_ExpressionAssign ea = new EwavRule_ExpressionAssign();
            //ea.Expression = txtExpression.Text;

            //ea.DataType = "System.String";
            //ea.FriendlyRule = "Assign " + txtDestinationField.Text + " the expression: " + txtExpression.Text;
            //ea.DestinationColumnName = txtDestinationField.Text;

            applicationViewModel.InvokePreColumnChangedEvent();



          //   applicationViewModel.EwavDefinedVariables.Add(ea);

            ListBoxItemSource listBoxItem = new ListBoxItemSource();
            listBoxItem.RuleString = ea.FriendlyRule;
            listBoxItem.NewColumn = txtDestinationField.Text;
            listBoxItem.AssignExpression = txtExpression.Text;
            listBoxItem.DataType = ((EwavColumn)cbxDataType.SelectedItem).SqlDataTypeAsString.ToString();
            listBoxItem.DestinationColumn = txtDestinationField.Text;
            listBoxItem.SourceColumn = null;
            listBoxItem.RuleType = EwavRuleType.Assign;

            ea.VaraiableDataType = ((EwavColumn)cbxDataType.SelectedItem).SqlDataTypeAsString.ToString();

            listBoxItem.Rule = ea;
            EwavColumn newColumn = new EwavColumn();
            newColumn.Name = txtDestinationField.Text;
            newColumn.SqlDataTypeAsString = ((EwavColumn)cbxDataType.SelectedItem).SqlDataTypeAsString;
            newColumn.NoCamelName = txtDestinationField.Text;
            newColumn.IsUserDefined = true;

            if (editMode == false)
            {
                applicationViewModel.EwavSelectedDatasource.AllColumns.Add(newColumn);
            }

            applicationViewModel.ListOfRules.Add(listBoxItem);

            List<EwavRule_Base> tempList = applicationViewModel.EwavDefinedVariables;
            tempList.Add(ea);

            applicationViewModel.EwavDefinedVariables = tempList;

            this.DialogResult = true;
        }

        void IEwavDashboardRule.CreateFromXml(System.Xml.Linq.XElement element)
        {
            throw new NotImplementedException();
        }
    }
}