/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       RecodedVariable.xaml.cs
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
using Ewav.BAL;
using Ewav.ViewModels;
using Ewav.Web.EpiDashboard;
using Ewav.Web.Services;
using Ewav.Web.EpiDashboard.Rules;
//Serialize Method is written in Extensions.cs
namespace Ewav
{
    public partial class RecodedVariable : ChildWindow, IEwavDashboardRule
    {
        public ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;

        private string sourceColumnName = string.Empty, destinationColumnName = string.Empty, elseValue = string.Empty, selectedDType = string.Empty;
        private EwavColumn sourceColumn;


        IEnumerable<EwavColumn> CBXFieldCols;
        List<EwavColumn> SourceColumns;

        bool twoColsEnabled = false;

        private bool editMode;
        int index = -1;

        ListBoxItemSource selectedItem;
        private bool loadingFieldType;

        public ListBoxItemSource SelectedItem
        {
            get { return selectedItem; }
            set { selectedItem = value; }
        }

        /// <summary>
        /// Property that returns the selected column's DataType 
        /// </summary>
        public string ColumnSourceDataType
        {
            get
            {
                if (sourceColumn == null)
                {
                    return null;
                }
                switch (sourceColumn.SqlDataTypeAsString)
                {
                    case ColumnDataType.Text:
                        return "System.String";
                    case ColumnDataType.Numeric:
                        return "System.Single";
                    case ColumnDataType.Boolean:
                        return "System.Boolean";
                    case ColumnDataType.DateTime:
                        return "System.DateTime";
                    case ColumnDataType.UserDefined:
                        return "UserDefined";
                    default:
                        break;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns selected VariableType in CbxFieldType Combo Box
        /// </summary>
        public DashboardVariableType DestinationFieldType
        {
            get
            {
                if (cbxFieldType.SelectedIndex >= 0)
                {
                    switch (cbxFieldType.SelectedItem.ToString())
                    {
                        case "Yes/No":
                            return DashboardVariableType.YesNo;
                        case "Numeric":
                            return DashboardVariableType.Numeric;
                        case "Text":
                            return DashboardVariableType.Text;
                        default:
                            return DashboardVariableType.None;
                    }
                }
                else
                {
                    return DashboardVariableType.None;
                }
            }
        }

        //Serialize Method is written in Extensions.cs

        /// <summary>
        /// Default constructor
        /// </summary>
        public RecodedVariable()
        {
            InitializeComponent();
            selectedDType = "Text";
            FillComboBox();
            checkboxMaintainSortOrder.IsChecked = true;
        }

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="editMode"></param>
        /// <param name="selectedItem"></param>
        public RecodedVariable(bool editMode, ListBoxItemSource selectedItem = null)
        {
            InitializeComponent();
            SelectedItem = selectedItem;
            this.editMode = editMode;
            FillComboBox();
        }

        /// <summary>
        /// Method that is used to Fill ComboBoxes.
        /// </summary>
        private void FillComboBox()
        {

            SourceColumns =
               applicationViewModel.EwavSelectedDatasource.AllColumns;

            List<ColumnDataType> columnDataType = new List<ColumnDataType>();
            columnDataType.Add(ColumnDataType.Boolean);
            columnDataType.Add(ColumnDataType.Numeric);
            columnDataType.Add(ColumnDataType.DateTime);
            columnDataType.Add(ColumnDataType.Text);
            columnDataType.Add(ColumnDataType.UserDefined);

            CBXFieldCols = from cols in SourceColumns
                           where columnDataType.Contains(cols.SqlDataTypeAsString)
                           orderby cols.Name
                           select cols;
            List<EwavColumn> colsList = CBXFieldCols.ToList();

            //Following loop to get the index of selected Item.
            for (int i = 0; i < colsList.Count; i++)
            {
                if (SelectedItem != null && SelectedItem.SourceColumn == colsList[i].Name)
                {
                    index = i;
                    break;
                }
            }
            cbxSourceField.ItemsSource = colsList;
            cbxSourceField.SelectedValue = "Index";
            cbxSourceField.DisplayMemberPath = "Name";
            cbxSourceField.SelectedIndex = index;

            loadingFieldType = true;
            cbxFieldType.Items.Add("Text");
            cbxFieldType.Items.Add("Numeric");
            cbxFieldType.Items.Add("Yes/No");
            //cbxFieldType.SelectedIndex = fieldTypeIndex;
            loadingFieldType = false;
            cbxFieldType.SelectedValue = selectedDType;
            if (editMode)
            {
                txtDestinationField.IsEnabled = false;
            }
            else
            {
                txtDestinationField.IsEnabled = true;
                cbxFieldType.SelectedIndex = cbxSourceField.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Method that is used to Fill the DataGrid
        /// </summary>
        private void FillDataGrid()
        {
            bool addMoreRows = true;
            dataGridViewRecode.IsReadOnly = false;
            List<EwavRuleRecodeDataRow> ListNumObject = new List<EwavRuleRecodeDataRow>();
            dataGridViewRecode.Columns.Clear();

            List<ColumnDataType> columnDataType = new List<ColumnDataType>();
            columnDataType.Add(ColumnDataType.DateTime);
            columnDataType.Add(ColumnDataType.Numeric);

            CBXFieldCols = from cols in SourceColumns
                           where columnDataType.Contains(cols.SqlDataTypeAsString)
                           orderby cols.Name
                           select cols;
            List<EwavColumn> colsList = CBXFieldCols.ToList();

            List<string> ColumnsListForDateAndNumeric = new List<string>();
            List<string> ColumnsListBool = new List<string>();
            List<string> ColumnsListNumeric = new List<string>();

            for (int i = 0; i < colsList.Count; i++)
            {
                ColumnsListForDateAndNumeric.Add(colsList[i].Name);
            }

            columnDataType = new List<ColumnDataType>();
            columnDataType.Add(ColumnDataType.Boolean);
            CBXFieldCols = from cols in SourceColumns
                           where columnDataType.Contains(cols.SqlDataTypeAsString)
                           orderby cols.Name
                           select cols;
            colsList = CBXFieldCols.ToList();

            for (int i = 0; i < colsList.Count; i++)
            {
                ColumnsListBool.Add(colsList[i].Name);
            }


            columnDataType = new List<ColumnDataType>();
            columnDataType.Add(ColumnDataType.Numeric);
            CBXFieldCols = from cols in SourceColumns
                           where columnDataType.Contains(cols.SqlDataTypeAsString)
                           orderby cols.Name
                           select cols;
            colsList = CBXFieldCols.ToList();

            for (int i = 0; i < colsList.Count; i++)
            {
                ColumnsListNumeric.Add(colsList[i].Name);
            }


            if (ColumnsListForDateAndNumeric.Contains(sourceColumnName))
            {
                dataGridViewRecode.Columns.Add(CreateTextColumn("col1", "From"));
                dataGridViewRecode.Columns.Add(CreateTextColumn("col2", "To"));
                twoColsEnabled = false;
            }
            else
            {
                dataGridViewRecode.Columns.Add(CreateTextColumn("col1", "From"));
                twoColsEnabled = true;
            }

            if (ColumnsListNumeric.Contains(sourceColumnName))
            {
                dataGridViewRecode.Columns.Add(CreateTextColumn("col3", "Representation"));

            }
            else if (ColumnsListBool.Contains(sourceColumnName))
            {
                DataGridTextColumn dgTextCol = CreateTextColumn("col3", "Representation");
                dataGridViewRecode.Columns.Add(dgTextCol);
            }
            else
            {
                dataGridViewRecode.Columns.Add(CreateTextColumn("col3", "Representation"));
            }

            if (cbxFieldType.SelectedItem != null &&
                cbxFieldType.SelectedItem.ToString() == "Yes/No")
            {
                ListNumObject = new List<EwavRuleRecodeDataRow>();
                EwavRuleRecodeDataRow Numobj1 = new EwavRuleRecodeDataRow();

                Numobj1.col1 = "";
                Numobj1.col2 = "";

                Numobj1.col3 = "Yes";
                ListNumObject.Add(Numobj1);

                Numobj1 = new EwavRuleRecodeDataRow();

                Numobj1.col1 = "";
                Numobj1.col2 = "";

                Numobj1.col3 = "No";
                ListNumObject.Add(Numobj1);
                addMoreRows = false;
            }


            if (addMoreRows)
            {
                for (int i = 0; i < 10; i++)
                {
                    ListNumObject.Add(new EwavRuleRecodeDataRow()
                    {
                        col1 = "",
                        col2 = "",
                        col3 = ""
                    }
                    );
                }
            }

            string selectedValue = (DestinationFieldType.ToString() == "YesNo") ? "Yes/No" : DestinationFieldType.ToString();


            if (SelectedItem != null &&
                // ((EwavRule_Recode)SelectedItem.Rule).DestinationFieldType.ToString() == DestinationFieldType.ToString()
                selectedDType.ToString() == selectedValue
                && index > -1)
            {
                dataGridViewRecode.ItemsSource = null;
                dataGridViewRecode.ItemsSource = (List<EwavRuleRecodeDataRow>)FindItemSource();
            }
            else
            {
                dataGridViewRecode.ItemsSource = null;
                dataGridViewRecode.ItemsSource = ListNumObject;
            }


        }



        /// <summary>
        /// Method that is used to Find the ItemSource of Selected Item.
        /// </summary>
        /// <returns></returns>
        private System.Collections.IEnumerable FindItemSource()
        {
            List<EwavRuleRecodeDataRow> obj = new List<EwavRuleRecodeDataRow>();
            foreach (EwavRule_Base i in applicationViewModel.EwavDefinedVariables)
            {
                if (i is EwavRule_Recode)
                {
                    EwavRule_Recode rule = i as EwavRule_Recode;
                    if (SelectedItem != null && SelectedItem.DestinationColumn == rule.TxtDestinationField)
                    {
                        for (int j = 0; j < rule.RecodeTable.Count; j++)
                        {
                            obj.Add(new EwavRuleRecodeDataRow
                            {
                                col1 = rule.RecodeTable[j].col1,
                                col2 = rule.RecodeTable[j].col2,
                                col3 = rule.RecodeTable[j].col3
                            });
                        }
                        //obj = new List<EwavRuleRecodeDataRow>(rule.RecodeTable);
                    }
                }
                //else
                //{


                //    throw new Exception(" rule eror for " + i.GetType().ToString());    







                //}
            }
            return obj;
        }

        /// <summary>
        /// Method that is used to Create Column of Text Type.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private static DataGridTextColumn CreateTextColumn(string fieldName, string title)
        {

            DataGridTextColumn column = new DataGridTextColumn();

            column.Header = title;
            column.MinWidth = 122;
            column.Binding = new System.Windows.Data.Binding(fieldName);
            column.Binding.Mode = System.Windows.Data.BindingMode.TwoWay;
            return column;

        }

        /// <summary>
        /// Event Hanlder that adds the selected rule.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDestinationField.Text))
            {
                MessageBox.Show("Destination field is blank.");
                this.DialogResult = true;
                return;
            }
            List<ListBoxItemSource> TempList = applicationViewModel.ListOfRules.Where(t => t.DestinationColumn.ToLower() == txtDestinationField.Text.ToLower()).ToList();
            if (TempList.Count > 0 && !editMode)
            {
                MessageBox.Show("Variable name already exists.");
                return;
            }

            if (!ValidateDataGrid((List<EwavRuleRecodeDataRow>)dataGridViewRecode.ItemsSource))
            {
                MessageBox.Show("Grid has some empty or invalid data.");
                return;
            }


            //following will not run while editing rule.
            if (!editMode)
            {
                List<ColumnDataType> columnDataType = new List<ColumnDataType>();
                columnDataType.Add(ColumnDataType.Boolean);
                columnDataType.Add(ColumnDataType.Numeric);
                columnDataType.Add(ColumnDataType.Text);

                CBXFieldCols = from cols in SourceColumns
                               where columnDataType.Contains(cols.SqlDataTypeAsString)
                               orderby cols.Name
                               select cols;
                List<EwavColumn> colsList = CBXFieldCols.ToList();
                List<string> ColumnsList = new List<string>();

                for (int i = 0; i < colsList.Count; i++)
                {
                    ColumnsList.Add(colsList[i].Name);
                }
            }

            string friendlyRule = "Recode the values in " + sourceColumn.Name + " to " + txtDestinationField.Text + "";


            EwavRule_Recode rule = new EwavRule_Recode();
            rule.Friendlyrule = friendlyRule;
            rule.SourceColumnName = sourceColumnName;
            rule.SourceColumnType = ColumnSourceDataType;
            rule.TxtDestinationField = txtDestinationField.Text;
            rule.DestinationFieldType = DestinationFieldType;
            rule.RecodeTable = (List<EwavRuleRecodeDataRow>)dataGridViewRecode.ItemsSource;
            rule.TxtElseValue = txtElseValue.Text;
            rule.CheckboxMaintainSortOrderIndicator = (bool)checkboxMaintainSortOrder.IsChecked;
            rule.CheckboxUseWildcardsIndicator = (bool)checkboxUseWildcards.IsChecked;
            //   rule.EwavRuleType = EwavRuleType.Recode;
            rule.VaraiableName = txtDestinationField.Text;
            rule.VaraiableDataType = GetDestinationColumnType(DestinationFieldType).ToString();


            ListBoxItemSource listBoxItem = new ListBoxItemSource();
            listBoxItem.RuleString = friendlyRule;
            listBoxItem.SourceColumn = sourceColumn.Name;
            listBoxItem.DestinationColumn = txtDestinationField.Text;
            listBoxItem.Rule = rule;


            EwavColumn newColumn = new EwavColumn();
            newColumn.Name = txtDestinationField.Text;
            newColumn.SqlDataTypeAsString = GetDestinationColumnType(DestinationFieldType);
            newColumn.NoCamelName = txtDestinationField.Text;
            newColumn.IsUserDefined = true;
            //following logic is used to create a relationship between UserDefined variable using another userDefined variable.

            sourceColumn.ChildVariableName = newColumn.Name;

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


            //determins if item already exists in collections. if it does, it is removed to get added again as a new item.
            bool itemExists = false;
            for (int i = 0; i < applicationViewModel.EwavDefinedVariables.Count; i++)
            {
                if (applicationViewModel.EwavDefinedVariables[i] is EwavRule_Recode)
                {
                    EwavRule_Recode ruleRec = applicationViewModel.EwavDefinedVariables[i] as EwavRule_Recode;


                    if (txtDestinationField.Text == ruleRec.TxtDestinationField)
                    {
                        applicationViewModel.EwavDefinedVariables.RemoveAt(i);
                        itemExists = true;
                    }
                }

            }
            applicationViewModel.InvokePreColumnChangedEvent();

            if (!itemExists)
            {
                applicationViewModel.EwavSelectedDatasource.AllColumns.Add(newColumn);
            }
            else
            {
                ListBoxItemSource item = (ListBoxItemSource)applicationViewModel.ListOfRules.Single(r => r.DestinationColumn == listBoxItem.DestinationColumn);
                applicationViewModel.ListOfRules.Remove(item);
            }
            applicationViewModel.ListOfRules.Add(listBoxItem);
            List<EwavRule_Base> listOfRules = new List<EwavRule_Base>();
            listOfRules = applicationViewModel.EwavDefinedVariables;
            listOfRules.Add(rule);

            applicationViewModel.EwavDefinedVariables = listOfRules;
            this.DialogResult = true;

        }

        private bool ValidateDataGrid(List<EwavRuleRecodeDataRow> Rows)
        {
            //  Col1 is From 
            //  Col2  is To  
            // Col 3 is  Representation         

            if (twoColsEnabled)
            {
                // this case the cols are always "From" and "Representation"  
                return Rows[0].col1.Length > 0 && Rows[0].col3.Length > 0;
            }
            else
            {
                // this case the cols are always  "From" To" and "Representation"    
                return Rows[0].col1.Length > 0 && Rows[0].col2.Length > 0 && Rows[0].col3.Length > 0;
            }


            //return true;
        }

        /// <summary>
        /// Gets a column type appropriate for a .NET data table based off of the dashboard variable type selected by the user
        /// </summary>
        /// <param name="dashboardVariableType">The type of variable that is storing the recoded values</param>
        /// <returns>A string representing the type of a .NET DataColumn</returns>
        private ColumnDataType GetDestinationColumnType(DashboardVariableType dashboardVariableType)
        {
            switch (dashboardVariableType)
            {
                case DashboardVariableType.Numeric:
                    return ColumnDataType.Numeric;
                case DashboardVariableType.Date:
                    return ColumnDataType.DateTime;
                case DashboardVariableType.Text:
                    return ColumnDataType.Text;
                case DashboardVariableType.YesNo:
                    return ColumnDataType.Boolean;
                case DashboardVariableType.None:
                    throw new Exception();
                default:
                    return ColumnDataType.Text;
            }
        }


        /// <summary>
        /// Event Handler that handles Cancel Button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// Event Handler that handles Selected Change Event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxSourceField_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxSourceField.SelectedIndex > -1)
            {
                List<ColumnDataType> columnDataType = new List<ColumnDataType>();
                columnDataType.Add(ColumnDataType.Boolean);
                columnDataType.Add(ColumnDataType.Numeric);
                columnDataType.Add(ColumnDataType.Text);
                columnDataType.Add(ColumnDataType.DateTime);
                columnDataType.Add(ColumnDataType.UserDefined);

                CBXFieldCols = from cols in SourceColumns
                               where columnDataType.Contains(cols.SqlDataTypeAsString)
                               orderby cols.Name
                               select cols;
                //following code reads from the list and reads the Column as EwavColumn and also reads the name of Selected EwavColumn.
                foreach (EwavColumn col in CBXFieldCols.ToList())
                {
                    if (col.Name.ToLower().Equals(((EwavColumn)cbxSourceField.SelectedItem).Name.ToString().ToLower().Trim()))
                    {
                        sourceColumnName = col.Name;
                        sourceColumn = col;
                        break;
                    }
                }

                //following code reads destinationColumnName and elseValue for selected EwavColumn
                foreach (EwavRule_Base rule in applicationViewModel.EwavDefinedVariables)
                {
                    if (rule is EwavRule_Recode)
                    {
                        EwavRule_Recode rule1 = rule as EwavRule_Recode;
                        if (SelectedItem != null &&
                            SelectedItem.DestinationColumn == rule1.TxtDestinationField.ToString())
                        {
                            destinationColumnName = rule1.TxtDestinationField;
                            elseValue = rule1.TxtElseValue;
                            selectedDType = (rule1.DestinationFieldType.ToString() == "YesNo") ? "Yes/No" : rule1.DestinationFieldType.ToString();
                            checkboxMaintainSortOrder.IsChecked = rule1.CheckboxMaintainSortOrderIndicator;
                            checkboxUseWildcards.IsChecked = rule1.CheckboxUseWildcardsIndicator;
                            break;
                        }
                    }
                }


                if (!editMode)
                {
                    txtDestinationField.Text = ((EwavColumn)cbxSourceField.SelectedItem).Name.ToString() + "_RECODED";
                }
                else
                {
                    txtDestinationField.Text = destinationColumnName;
                    txtElseValue.Text = elseValue;
                }
                FillDataGrid();
            }
            else
            {
                dataGridViewRecode.ItemsSource = null;
            }

            EnableDisableFillRanges();
        }

        public void SetSelectedItem(string SelectedValue, ComboBox CmbBox)
        {
            for (int i = 0; i < CmbBox.Items.Count; i++)
            {
                if (CmbBox.Items[i] == SelectedValue)
                {
                    CmbBox.SelectedIndex = i;
                    break;
                }
            }
        }

        private void cbxFieldType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!loadingFieldType)
            {
                FillDataGrid();
                EnableDisableFillRanges();
            }

        }

        /// <summary>
        /// Method that turns FillRangeButton On/off.
        /// </summary>
        private void EnableDisableFillRanges()
        {
            if (ColumnSourceDataType == null)
            {
                return;
            }
            if (ColumnSourceDataType == "System.Single" && DestinationFieldType.Equals(DashboardVariableType.Text))
            {
                FillRangesButton.IsEnabled = true;
            }
            else
            {
                FillRangesButton.IsEnabled = false;
            }
        }


        /// <summary>
        /// Event that handles the click event of FillRangeButton.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FillRangesButton_Click(object sender, RoutedEventArgs e)
        {
            FillRanges fillRange = new FillRanges();
            fillRange.Show();
            fillRange.Closed += new EventHandler(fillRange_Closed);
        }

        void fillRange_Closed(object sender, EventArgs e)
        {
            FillRanges fillRange = sender as FillRanges;
            if (fillRange.DialogResult == true)
            {
                int startValue = fillRange.StartValue;
                int endValue = fillRange.EndValue;
                int rangeValue = fillRange.RangeValue;

                List<EwavRuleRecodeDataRow> rangeTable = new List<EwavRuleRecodeDataRow>();

                EwavRuleRecodeDataRow row = new EwavRuleRecodeDataRow();
                row.col1 = "LOVALUE";
                row.col2 = startValue.ToString();
                row.col3 = "LOVALUE - <" + startValue.ToString();

                rangeTable.Add(row);

                for (int i = startValue; i < endValue; i = i + rangeValue)
                {
                    string lowerBound = i.ToString();
                    string upperBound = (i + rangeValue).ToString();

                    if ((i + rangeValue) > endValue)
                    {
                        upperBound = endValue.ToString();
                    }
                    row = new EwavRuleRecodeDataRow();
                    row.col1 = lowerBound;
                    row.col2 = upperBound;
                    row.col3 = lowerBound + " - <" + upperBound;
                    rangeTable.Add(row);
                }
                row = new EwavRuleRecodeDataRow();
                row.col1 = endValue.ToString();
                row.col2 = "HIVALUE";
                row.col3 = endValue.ToString() + " - < HIVALUE";
                rangeTable.Add(row);

                dataGridViewRecode.ItemsSource = rangeTable;
            }
        }

        void IEwavDashboardRule.CreateFromXml(System.Xml.Linq.XElement element)
        {
            throw new NotImplementedException();
        }

        private void dataGridViewRecode_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //if (e.Key == System.Windows.Input.Key.Tab)
            //{
            DataGrid DGrid = ((DataGrid)sender);
            int DCount = DGrid.ItemsSource.OfType<object>().Count();

            if (DCount - 1 == DGrid.SelectedIndex) //- coz of zeor based index of DGrid rows
            {
                List<EwavRuleRecodeDataRow> ListNumObject = new List<EwavRuleRecodeDataRow>();
                ListNumObject = (List<EwavRuleRecodeDataRow>)DGrid.ItemsSource;
                ListNumObject.Add(
                    new EwavRuleRecodeDataRow()
                    {
                        col1 = "",
                        col2 = "",
                        col3 = ""
                    });
                DGrid.ItemsSource = null;
                DGrid.ItemsSource = ListNumObject;
            }
            //}
        }
    }


}