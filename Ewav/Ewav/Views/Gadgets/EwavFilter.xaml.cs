/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       EwavFilter.xaml.cs
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
using Ewav.Web.Services;
using System.Text;
using Ewav.Web.EpiDashboard;
using Ewav.ViewModels;
using Ewav.ExtensionMethods;
using CommonLibrary;
using System.Text.RegularExpressions;

namespace Ewav
{
    /// <summary>
    /// Used to define the object current state.
    /// </summary>
    public enum FilterControlType
    {
        DataFilter = 0,
        Conditional = 1
    }

    public partial class EwavFilter : UserControl
    {
        private FilterControlType filterType;

        public FilterControlType FilterType
        {
            get { return filterType; }
            set { filterType = value; }
        }

        private bool loadingOps;

        public bool LoadingOps
        {
            get { return loadingOps; }
            set { loadingOps = value; }
        }



        List<EwavColumn> colsList;
        List<string> extractedList, operatorsList, configList;
        List<int> rowRemoved = new List<int>();
        List<EwavColumn> SourceColumns;
        int globalPointer, globalRemovedPointer = 0;

        EwavDataFilterCondition currentCondition = null;
        public int GlobalRemovedPointer
        {
            get { return globalRemovedPointer; }
            set { globalRemovedPointer = value; }
        }

        public int GlobalPointer
        {
            get { return globalPointer; }
            set { globalPointer = value; }
        }
        StringBuilder conditionText = new StringBuilder();

        public StringBuilder ConditionText
        {
            get { return conditionText; }
            set { conditionText = value; }
        }


        bool loadingCombos = false;
        bool loadingOperators = false;
        int index = 0;

        public EwavFilter()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize the grid for Data Filter.
        /// </summary>
        public void InitializeDataFilter()
        {
            FillBindingLists();

            if (FindStackPanel(0, "sp") == null)
            {
                CreateFilterConditionRow();
            }
        }

        /// <summary>
        /// Method that checks if UserDefined Var is in Use and takes appropriate actions.
        /// </summary>
        /// <param name="o"></param>
        public void UserDefinedVarInUse(object o)
        {
            bool variableFound = false;

            globalPointer = ApplicationViewModel.Instance.EwavDatafilters.Count;
            List<EwavDataFilterCondition> filters = ApplicationViewModel.Instance.EwavDatafilters;
            List<EwavDataFilterCondition> filtersToDelete = new List<EwavDataFilterCondition>();
            int rowIndexDeleted = 0;
            for (int i = 0; i <= globalPointer; i++)
            {
                ComboBox cmb = ((ComboBox)FindControlInRow(i, "field"));
                if (cmb != null)
                {
                    if (((EwavColumn)cmb.SelectedItem).Name == ((EwavColumn)o).Name)
                    {
                        MainPanel.Children.Remove(FindStackPanel(i, "sp"));
                        variableFound = true;
                        //globalPointer--;
                        globalRemovedPointer++;
                        //filters.RemoveAt(i);
                        filtersToDelete.Add(filters[i - rowIndexDeleted]);
                        index = i;
                    }
                }
                else
                {
                    rowIndexDeleted++;
                }
            }

            for (int i = 0; i < filtersToDelete.Count; i++)
            {
                filters.Remove(filtersToDelete[i]);
            }

            ApplicationViewModel.Instance.EwavDatafilters = filters;

            if (variableFound)
            {
                RefreshDataFilter();
            }

            if (globalPointer == 1 && variableFound)
            {
                globalPointer = 0;
                globalRemovedPointer = 0;
                CreateFilterConditionRow();
                return;
            }

            int lastItemRow = 0;

            if (MainPanel.Children.Count > 0)
            {
                lastItemRow = Convert.ToInt32(((StackPanel)MainPanel.Children[MainPanel.Children.Count - 1]).Name.ToString().Split(new Char[] { ',' })[1]);
            }


            if (lastItemRow < index)
            {
                ReplaceRemoveButton(lastItemRow);
            }





        }

        /// <summary>
        /// Refreshes the Binding Lists, binds the combo boxes and sets the selectedIndex.
        /// </summary>
        public void RefreshDataFilter()
        {
            FillBindingLists();
            
            for (int i = 0; i <= this.globalPointer; i++)
            {
                ComboBox cmb = ((ComboBox)FindControlInRow(i, "field"));

                string selectedValue = "";

                if (cmb == null)
                {
                    //continue if the combo box doesnt exist.
                    continue;
                }

                if (cmb.SelectedItem != null)
                {
                    selectedValue = ((EwavColumn)cmb.SelectedItem).Name.ToString();

                    if (selectedValue == "")
                    {
                        return;
                    }

                    loadingCombos = true;
                    cmb.ItemsSource = null;
                    // cmb.ItemsSource = extractedList; // extractedList; 
                    cmb.ItemsSource = colsList;
                    cmb.SelectedValue = "Index";
                    cmb.DisplayMemberPath = "Name";
                    SearchAndSetIndex(cmb, selectedValue);
                    loadingCombos = false;
                }
                else
                {
                    cmb.ItemsSource = null;
                    //cmb.ItemsSource = extractedList;
                    cmb.ItemsSource = colsList;
                    cmb.SelectedValue = "Index";
                    cmb.DisplayMemberPath = "Name";
                }

            }

        }

        /// <summary>
        /// Searches and sets the Index. Also checks if the varaible is User Defined.
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="selectedValue"></param>
        private void SearchAndSetIndex(ComboBox cmb, string selectedValue)
        {

            for (int i = 0; i < cmb.Items.Count; i++)
            {
                if (((EwavColumn)cmb.Items[i]).Name == selectedValue)
                {
                    LoadingOps = true;
                    cmb.SelectedIndex = i;
                    if (((EwavColumn)cmb.Items[i]).IsUserDefined)
                    {
                        ((EwavColumn)cmb.Items[i]).IsInUse = true;
                    }
                    LoadingOps = false;
                    break;
                }
            }

        }


        /// <summary>
        /// helper method that create and adds the row to dataFilter Grid
        /// </summary>
        public void CreateFilterConditionRow()
        {
            //btnClear.IsEnabled = true;
            loadingCombos = true;
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            sp.Name = "sp" + "," + globalPointer;


            ComboBox cmb = new ComboBox();
            cmb.ItemsSource = colsList;
            cmb.SelectedValue = "Index";
            cmb.DisplayMemberPath = "Name";
            cmb.Name = "field" + "," + globalPointer;
            cmb.Width = 150;
            cmb.Margin = new Thickness(4, 2, 2, 2);
            cmb.SelectionChanged += new SelectionChangedEventHandler(cmb_SelectionChanged);
            cmb.SelectedIndex = -1;
            sp.Children.Add(cmb);

            ComboBox cmb1 = new ComboBox();
            cmb1.ItemsSource = operatorsList;
            cmb1.Name = "operator" + "," + globalPointer;
            cmb1.SelectionChanged += new SelectionChangedEventHandler(cmbOp_SelectionChanged);
            cmb1.Width = 120;
            cmb1.Margin = new Thickness(4, 2, 2, 2);
            sp.Children.Add(cmb1);

            StackPanel subSp = new StackPanel();
            subSp.Orientation = Orientation.Horizontal;
            subSp.VerticalAlignment = System.Windows.VerticalAlignment.Center;


            TextBox tb = new TextBox();
            tb.Name = "tb" + "," + globalPointer;
            tb.Width = 200;
            tb.Margin = new Thickness(4, 2, 2, 2);
            tb.TextChanged += new TextChangedEventHandler(tb_TextChanged);
            subSp.Name = "subSp" + "," + globalPointer;
            subSp.Children.Add(tb);
            sp.Children.Add(subSp);

            Button but = new Button();
            but.Click += new RoutedEventHandler(btnAdd_Click);
            but.Content = "ADD";
            but.Style = Application.Current.Resources["Add"] as Style;
            but.Cursor = Cursors.Hand;
            but.Name = "but" + "," + globalPointer;
            //but.Width = 70;
            but.Margin = new Thickness(4, 2, 2, 2);
            sp.Children.Add(but);
            MainPanel.Children.Add(sp);
            //filterCount.Text = string.Format("Data Filter ({0})", globalPointer - globalRemovedPointer);
            loadingCombos = false;
            if (currentCondition != null)
            {
                LoadThisStackPanel(globalPointer);
            }
        }

        /// <summary>
        /// Recieves the integer value as an Index and Loads the StackPanel.
        /// </summary>
        /// <param name="index"></param>
        public void LoadThisStackPanel(int index)
        {

            ComboBox cmb = (ComboBox)FindControlInRow(index, "field");
            //cmb.SelectedItem = currentCondition.FieldName.VarName;

            SearchAndSetIndex(cmb, currentCondition.FieldName.VarName);

            cmb = (ComboBox)FindControlInRow(index, "operator");
            cmb.SelectedItem = currentCondition.FriendlyOperand.VarName;

            //cmb = (ComboBox)FindControlInRow(index, "cmbJoinType");
            //if (cmb != null)
            //{
            //    cmb.SelectedItem = currentCondition.JoinType.VarName;
            //}

        }

        /// <summary>
        /// Helper method Finds the Control in Row
        /// </summary>
        /// <param name="rowNumber"></param>
        /// <param name="controlName"></param>
        /// <param name="deep"></param>
        /// <returns></returns>
        private Control FindControlInRow(int rowNumber, string controlName, bool deep = false)
        {
            StackPanel sp;
            if (deep)
            {
                sp = ((StackPanel)FindStackPanel(rowNumber, "subSp", true));
            }
            else
            {
                sp = ((StackPanel)FindStackPanel(rowNumber, "sp"));
            }

            if (sp == null)
            {
                return null;
            }

            int count = VisualTreeHelper.GetChildrenCount(sp);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(sp, i);
                if (child is Control)
                {
                    if (((Control)child).Name.Contains(controlName))
                    {
                        return (Control)child;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Finds the stackpanel
        /// </summary>
        /// <param name="rowNumber"></param>
        /// <param name="controlName"></param>
        /// <param name="deep"></param>
        /// <returns></returns>
        private StackPanel FindStackPanel(int rowNumber, string controlName, bool deep = false)
        {
            StackPanel sp;
            if (deep)
            {
                sp = FindStackPanel(rowNumber, "sp");
            }
            else
            {
                sp = MainPanel;

            }
            int count = VisualTreeHelper.GetChildrenCount(sp);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(sp, i);
                if (child is StackPanel)
                {
                    if (((StackPanel)child).Name.Contains(controlName + "," + rowNumber))
                    {
                        return (StackPanel)child;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Fills the comboboxes for dataFilter Grid
        /// </summary>
        public void FillBindingLists()
        {
            SourceColumns = ApplicationViewModel.Instance.EwavSelectedDatasource.AllColumns;
            extractedList = new List<string>();
            operatorsList = new List<string>();
            configList = new List<string>();

            List<ColumnDataType> columnDataType = new List<ColumnDataType>();
            columnDataType.Add(ColumnDataType.Boolean);
            columnDataType.Add(ColumnDataType.Numeric);
            columnDataType.Add(ColumnDataType.DateTime);
            columnDataType.Add(ColumnDataType.Text);
            columnDataType.Add(ColumnDataType.UserDefined);

            IEnumerable<EwavColumn> CBXFieldCols = from cols in SourceColumns
                                                   where columnDataType.Contains(cols.SqlDataTypeAsString)
                                                   orderby cols.Name
                                                   select cols;

            colsList = CBXFieldCols.ToList();

            //colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });


        }

        /// <summary>
        /// Converts the Conditions to StackPanels
        /// </summary>
        /// <param name="Conditions"></param>
        public void ConstructStackPanelFromDataFilters(List<EwavDataFilterCondition> Conditions)
        {
            if (MainPanel != null)
            {
                MainPanel.Children.Clear();
            }
            else
            {
                MainPanel = new StackPanel();
            }


            FillBindingLists();

            if (Conditions.Count == 0)
            {
                CreateFilterConditionRow();
                return;
            }

            for (int i = 0; i < Conditions.Count; i++)
            {
                this.GlobalPointer = i;
                currentCondition = new EwavDataFilterCondition();
                currentCondition.FieldName = new MyString();
                currentCondition.FieldName.VarName = Conditions[i].FieldName.VarName;

                currentCondition.FriendlyOperand = new MyString();
                currentCondition.FriendlyOperand.VarName = Conditions[i].FriendlyOperand.VarName;

                if (Conditions[i].FriendlyValue != null && Conditions[i].FriendlyValue.VarName != null)
                {
                    currentCondition.FriendlyValue = new MyString();
                    currentCondition.FriendlyValue.VarName = Conditions[i].FriendlyValue.VarName;
                }

                if (Conditions[i].FriendLowValue != null && Conditions[i].FriendLowValue.VarName != null)
                {
                    currentCondition.FriendLowValue = new MyString();
                    currentCondition.FriendLowValue.VarName = Conditions[i].FriendLowValue.VarName;
                }

                if (Conditions[i].FriendHighValue != null && Conditions[i].FriendHighValue.VarName != null)
                {
                    currentCondition.FriendHighValue = new MyString();
                    currentCondition.FriendHighValue.VarName = Conditions[i].FriendHighValue.VarName;
                }


                currentCondition.JoinType = new MyString();
                currentCondition.JoinType.VarName = Conditions[i].JoinType.VarName;

                CreateFilterConditionRow();

                if (i != Conditions.Count - 1)
                {
                    ReplaceAddButton(this.GlobalPointer,
                        (JoinType)Enum.Parse(typeof(JoinType), currentCondition.JoinType.VarName, true)
                        );

                }

            }

            this.GlobalPointer = Conditions.Count;


            currentCondition = null;

        }

        /// <summary>
        /// Method that creates dataFilter
        /// </summary>
        /// <returns></returns>
        public List<EwavDataFilterCondition> CreateDataFilters()
        {

            ApplicationViewModel.Instance.ListOfDefinedVarsInUseByAnotherVar.Clear();
            List<EwavDataFilterCondition> filtercondList = new List<EwavDataFilterCondition>();

            if (ValidateControl())
            {
                int childCount = MainPanel.Children.Count;
                FillBindingLists();
                for (int i = 0; i < childCount; i++)
                {
                    UIElementCollection listOfCtrl = ((StackPanel)MainPanel.Children[i]).Children;



                    string colVal = ((EwavColumn)((ComboBox)listOfCtrl[0]).SelectedItem).Name.ToString();

                    if (colVal.Trim().Length == 0)
                    {
                        return new List<EwavDataFilterCondition>();
                    }

                    ColumnDataType dataType = LookUpDataType(colVal);

                    if (((EwavColumn)((ComboBox)listOfCtrl[0]).SelectedItem).IsUserDefined && this.FilterType == FilterControlType.DataFilter)
                    {
                        ((EwavColumn)((ComboBox)listOfCtrl[0]).SelectedItem).IsInUse = true;
                        EwavColumn col = new EwavColumn();
                        col.Name = ((EwavColumn)((ComboBox)listOfCtrl[0]).SelectedItem).Name;
                        col.IsUserDefined = true;
                        col.IsInUse = true;
                        ApplicationViewModel.Instance.ListOfDefinedVarsInUseByAnotherVar.Add(col);
                        ApplicationViewModel.Instance.IsDefVarInUseByDF = true; // TBD When NO DF then applyDFEVent gets raised for gadgets. shouldnt be c
                    }

                    string operatorSelected = ((ComboBox)listOfCtrl[1]).SelectedItem.ToString();
                    switch (dataType)
                    {

                        case ColumnDataType.Text:


                            EwavDataFilterCondition condition = new EwavDataFilterCondition()
                            {
                                FieldName = ((EwavColumn)((ComboBox)listOfCtrl[0]).SelectedItem).Name.ToString().ToMyString(),
                                FriendlyOperand = ((ComboBox)listOfCtrl[1]).SelectedItem.ToString().ToMyString(),
                                FriendlyValue = ((TextBox)((StackPanel)listOfCtrl[2]).Children[0]).Text.ToString().ToMyString(),
                                JoinType = (listOfCtrl[3] is ComboBox) ? ((ComboBox)listOfCtrl[3]).SelectedItem.ToString().ToMyString() : "AND".ToMyString()
                            };
                            filtercondList.Add(condition);

                            conditionText.Append("The Value of " + condition.FieldName.VarName + " " +
                                condition.FriendlyOperand.VarName + " " +
                                condition.FriendlyValue.VarName + " " +
                               condition.JoinType.VarName);
                            break;
                        case ColumnDataType.Numeric:
                            if (SharedStrings.FRIENDLY_OPERATOR_BETWEEN == operatorSelected)
                            {
                                condition =
                                 new EwavDataFilterCondition()
                                 {
                                     FieldName = ((EwavColumn)((ComboBox)listOfCtrl[0]).SelectedItem).Name.ToString().ToMyString(),
                                     FriendlyOperand = ((ComboBox)listOfCtrl[1]).SelectedItem.ToString().ToMyString(),
                                     FriendLowValue = ((TextBox)((StackPanel)listOfCtrl[2]).Children[0]).Text.Replace(',', '.').ToString().ToMyString(),
                                     FriendHighValue = ((TextBox)((StackPanel)listOfCtrl[2]).Children[2]).Text.Replace(',', '.').ToString().ToMyString(),
                                     JoinType = (listOfCtrl[3] is ComboBox) ? ((ComboBox)listOfCtrl[3]).SelectedItem.ToString().ToMyString() : "AND".ToMyString()
                                 };
                                filtercondList.Add(condition);
                                conditionText.Append("The Value of " + condition.FieldName.VarName + " " +
                                condition.FriendlyOperand.VarName + " " +
                                condition.FriendLowValue.VarName + " " +
                                condition.FriendHighValue.VarName + " " +
                               condition.JoinType.VarName);
                            }
                            else
                            {

                                condition =
                                   new EwavDataFilterCondition()
                                   {
                                       FieldName = ((EwavColumn)((ComboBox)listOfCtrl[0]).SelectedItem).Name.ToString().ToMyString(),
                                       FriendlyOperand = ((ComboBox)listOfCtrl[1]).SelectedItem.ToString().ToMyString(),
                                       FriendlyValue = ((TextBox)((StackPanel)listOfCtrl[2]).Children[0]).Text.Replace(',', '.').ToString().ToMyString(),
                                       JoinType = (listOfCtrl[3] is ComboBox) ? ((ComboBox)listOfCtrl[3]).SelectedItem.ToString().ToMyString() : "AND".ToMyString()
                                   };
                                filtercondList.Add(condition);
                                conditionText.Append("The Value of " + condition.FieldName.VarName + " " +
                              condition.FriendlyOperand.VarName + " " +
                              condition.FriendlyValue.VarName + " " +
                             condition.JoinType.VarName);

                            }
                            break;
                        case ColumnDataType.Boolean:
                            MyString fValue;
                            if (((ComboBox)((StackPanel)listOfCtrl[2]).Children[0]).IsEnabled)
                            {
                                if (((ComboBox)((StackPanel)listOfCtrl[2]).Children[0]).SelectedItem.ToString().ToUpper() == "MISSING")
                                {
                                    fValue = "missing".ToMyString();
                                }
                                else
                                {
                                    fValue = ((ComboBox)((StackPanel)listOfCtrl[2]).Children[0]).SelectedItem.ToString().ToMyString();
                                }
                            }
                            else
                            {
                                fValue = new MyString();
                            }

                            condition =
                               new EwavDataFilterCondition()
                               {
                                   FieldName = ((EwavColumn)((ComboBox)listOfCtrl[0]).SelectedItem).Name.ToString().ToMyString(),
                                   FriendlyOperand = ((ComboBox)listOfCtrl[1]).SelectedItem.ToString().ToMyString(),
                                   FriendlyValue = fValue,
                                   JoinType = (listOfCtrl[3] is ComboBox) ? ((ComboBox)listOfCtrl[3]).SelectedItem.ToString().ToMyString() : "AND".ToMyString()
                               };
                            filtercondList.Add(condition);
                            conditionText.Append("The Value of " + condition.FieldName.VarName + " " +
                          condition.FriendlyOperand.VarName + " " +
                          condition.FriendlyValue.VarName + " " +
                         condition.JoinType.VarName);

                            break;
                        case ColumnDataType.DateTime:
                            if (SharedStrings.FRIENDLY_OPERATOR_BETWEEN == operatorSelected)
                            {

                                condition =
                                  new EwavDataFilterCondition()
                                  {
                                      FieldName = ((EwavColumn)((ComboBox)listOfCtrl[0]).SelectedItem).Name.ToString().ToMyString(),
                                      FriendlyOperand = ((ComboBox)listOfCtrl[1]).SelectedItem.ToString().ToMyString(),
                                      FriendLowValue = ((DatePicker)((StackPanel)listOfCtrl[2]).Children[0]).Text.ToString().ToMyString(),
                                      FriendHighValue = ((DatePicker)((StackPanel)listOfCtrl[2]).Children[2]).Text.ToString().ToMyString(),
                                      JoinType = (listOfCtrl[3] is ComboBox) ? ((ComboBox)listOfCtrl[3]).SelectedItem.ToString().ToMyString() : "AND".ToMyString()
                                  };
                                filtercondList.Add(condition);
                                conditionText.Append("The Value of " + condition.FieldName.VarName + " " +
                                condition.FriendlyOperand.VarName + " " +
                                condition.FriendLowValue.VarName + " " +
                                condition.FriendHighValue.VarName + " " +
                               condition.JoinType.VarName);
                            }
                            else
                            {

                                condition =
                                   new EwavDataFilterCondition()
                                   {
                                       FieldName = ((EwavColumn)((ComboBox)listOfCtrl[0]).SelectedItem).Name.ToString().ToMyString(),
                                       FriendlyOperand = ((ComboBox)listOfCtrl[1]).SelectedItem.ToString().ToMyString(),
                                       FriendlyValue = ((DatePicker)((StackPanel)listOfCtrl[2]).Children[0]).Text.ToString().ToMyString(),
                                       JoinType = (listOfCtrl[3] is ComboBox) ? ((ComboBox)listOfCtrl[3]).SelectedItem.ToString().ToMyString() : "AND".ToMyString()
                                   };
                                filtercondList.Add(condition);
                                conditionText.Append("The Value of " + condition.FieldName.VarName + " " +
                             condition.FriendlyOperand.VarName + " " +
                             condition.FriendlyValue.VarName + " " +
                            condition.JoinType.VarName);
                            }
                            break;
                        case ColumnDataType.UserDefined:
                            break;
                        default:
                            break;
                    }
                }

                return filtercondList;
            }
            return null;
        }


        /// <summary>
        /// Validates the controls.
        /// </summary>
        /// <param name="stackPanel"></param>
        /// <returns></returns>
        public bool ValidateControl()
        {
            for (int j = 0; j < MainPanel.Children.Count; j++)
            {
                StackPanel child = (StackPanel)MainPanel.Children[j];

                StackPanel stackPanel = child;
                string spName = child.Name;
                //int j = Convert.ToInt32(spName.Substring(spName.IndexOf(",") + 1, spName.Length - spName.IndexOf(",") - 1));

                String s = string.Empty;

                for (int i = 0; i < stackPanel.Children.Count; i++)
                {
                    if (stackPanel.Children[i] is TextBox && ((TextBox)stackPanel.Children[i]).Text == ""
                        && ((TextBox)stackPanel.Children[i]).IsEnabled)
                    {
                        s += "No data entered in the Value.\n";
                    }
                    else if (stackPanel.Children[i] is ComboBox && ((ComboBox)stackPanel.Children[i]).SelectedIndex < 0
                        && ((ComboBox)stackPanel.Children[i]).IsEnabled)
                    {
                        s += "No selection has been made for either Field or Operator dropdownlist.\n";
                    }
                    else if (stackPanel.Children[i] is DatePicker && ((DatePicker)stackPanel.Children[i]).Text == ""
                         && ((DatePicker)stackPanel.Children[i]).IsEnabled)
                    {
                        s += "No date selected.\n";
                    }
                    else if (stackPanel.Children[i] is StackPanel)
                    {
                        s += ValidateStackPanel((StackPanel)stackPanel.Children[i]);
                    }
                }
                if (s.Length > 0)
                {
                    MessageBox.Show(s);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Validates the stack panel.
        /// </summary>
        /// <param name="sp">The sp.</param>
        /// <returns></returns>
        public string ValidateStackPanel(StackPanel sp)
        {
            string s = "";

            for (int i = 0; i < sp.Children.Count; i++)
            {
                if (sp.Children[i] is TextBox && ((TextBox)sp.Children[i]).Text == ""
                     && ((TextBox)sp.Children[i]).IsEnabled)
                {
                    s += "No data entered in the Value.\n";
                }
                else if (sp.Children[i] is ComboBox && ((ComboBox)sp.Children[i]).SelectedIndex < 0
                     && ((ComboBox)sp.Children[i]).IsEnabled)
                {
                    s += "No Selection has been made for Value dropdownlist.\n";
                }
                else if (sp.Children[i] is DatePicker && ((DatePicker)sp.Children[i]).Text == ""
                     && ((DatePicker)sp.Children[i]).IsEnabled)
                {
                    s += "No date selected.\n";
                }
            }
            return s;
        }

        /// <summary>
        /// Fills the comboBoex for Operator ComboBoxex
        /// </summary>
        /// <param name="field"></param>
        private void FillOperatorValues(ColumnDataType field)
        {


            operatorsList.Clear();

            // Set operator drop-down values
            if (field == ColumnDataType.Numeric || field == ColumnDataType.DateTime)
            {
                operatorsList.Add(SharedStrings.FRIENDLY_OPERATOR_EQUAL_TO);
                operatorsList.Add(SharedStrings.FRIENDLY_OPERATOR_BETWEEN);
                operatorsList.Add(SharedStrings.FRIENDLY_OPERATOR_LESS_THAN);
                operatorsList.Add(SharedStrings.FRIENDLY_OPERATOR_GREATER_THAN);
                operatorsList.Add(SharedStrings.FRIENDLY_OPERATOR_LESS_THAN_OR_EQUAL);
                operatorsList.Add(SharedStrings.FRIENDLY_OPERATOR_GREATER_THAN_OR_EQUAL);
                operatorsList.Add(SharedStrings.FRIENDLY_OPERATOR_NOT_MISSING);
                operatorsList.Add(SharedStrings.FRIENDLY_OPERATOR_NOT_EQUAL_TO);
            }
            else if (field == ColumnDataType.Boolean)
            {
                operatorsList.Add(SharedStrings.FRIENDLY_OPERATOR_EQUAL_TO);
                operatorsList.Add(SharedStrings.FRIENDLY_OPERATOR_NOT_MISSING);

                configList.Add("Yes");
                configList.Add("No");
                //configList.Add("Missing");
            }
            else if (field == ColumnDataType.Text)
            {
                operatorsList.Add(SharedStrings.FRIENDLY_OPERATOR_EQUAL_TO);
                //operatorsList.Add(SharedStrings.FRIENDLY_OPERATOR_LIKE);
                operatorsList.Add(SharedStrings.FRIENDLY_OPERATOR_BEGINSWITH);
                operatorsList.Add(SharedStrings.FRIENDLY_OPERATOR_ENDSWITH);
                operatorsList.Add(SharedStrings.FRIENDLY_OPERATOR_CONTAINS);
                operatorsList.Add(SharedStrings.FRIENDLY_OPERATOR_NOT_MISSING);
                operatorsList.Add(SharedStrings.FRIENDLY_OPERATOR_NOT_EQUAL_TO);
            }

        }

        /// <summary>
        /// Looks up the dataType for selected column
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private ColumnDataType LookUpDataType(string selectedColumn)
        {
            IEnumerable<ColumnDataType> CBXFieldCols = from cols in SourceColumns
                                                       orderby cols.Name
                                                       where cols.Name == selectedColumn
                                                       select cols.SqlDataTypeAsString;

            List<ColumnDataType> listDT = CBXFieldCols.ToList<ColumnDataType>();
            return listDT[0];
        }

        /// <summary>
        /// Handler responds to textChanged event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Text != "")
            {
                //btnApply.IsEnabled = true;
                //btnClear.IsEnabled = true;
                //Raise an event that control is in edit mode. TBD
            }

        }

        /// <summary>
        /// Handler responds to Add Click event. 
        /// Adds a row in Data Filter Control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (MainPanel.Children.Count == 0)
            {
                MainPanel = (StackPanel)((StackPanel)((Button)sender).Parent).Parent;
            }

            string buttonName = ((Button)sender).Name;
            int rowNumber = Convert.ToInt32(buttonName.Substring(buttonName.IndexOf(",") + 1, buttonName.Length - buttonName.IndexOf(",") - 1));



            ReplaceAddButton(rowNumber);

            //filterCount.Text = filterCount.Text + " * ";
            globalPointer++;
            CreateFilterConditionRow();
            //scroll_content.UpdateLayout();
            //scroll_content.ScrollToVerticalOffset(double.MaxValue);
            //Raise event to update the scroll bar.

        }

        /// <summary>
        /// Replaces Remove buttons with appropriate buttons
        /// </summary>
        /// <param name="rowNumber"></param>
        private void ReplaceRemoveButton(int rowNumber)
        {

            ((StackPanel)MainPanel.Children[rowNumber]).Children.Remove(FindControlInRow(rowNumber, "cmbJoinType"));

            ((StackPanel)MainPanel.Children[rowNumber]).Children.Remove(FindControlInRow(rowNumber, "btnRemove"));

            Button but = new Button();
            but.Click += new RoutedEventHandler(btnAdd_Click);
            but.Content = "ADD";
            but.Style = Application.Current.Resources["Add"] as Style;
            but.Cursor = Cursors.Hand;
            but.Name = "but" + "," + rowNumber;
            //but.Width = 70;
            but.Margin = new Thickness(4, 2, 2, 2);
            ((StackPanel)MainPanel.Children[rowNumber]).Children.Add(but);
        }

        /// <summary>
        /// Replaces Add button with appropriate buttons.
        /// </summary>
        /// <param name="rowNumber"></param>
        private void ReplaceAddButton(int rowNumber, JoinType joinType = JoinType.AND)
        {
            int lineNumber = MainPanel.Children.Count - 1;

            ((StackPanel)MainPanel.Children[lineNumber]).Children.Remove(FindControlInRow(rowNumber, "but"));

            ComboBox cmbJoinType = new ComboBox();
            cmbJoinType.Name = "cmbJoinType, " + rowNumber;

            cmbJoinType.Items.Add(JoinType.AND);
            cmbJoinType.Items.Add(JoinType.OR);
            cmbJoinType.Width = 65;
            cmbJoinType.FontSize = 11;
            cmbJoinType.Margin = new Thickness(4, 2, 2, 2);
            //cmbJoinType.SelectedIndex = 0;
            cmbJoinType.SelectedItem = joinType;
            cmbJoinType.SelectionChanged += new SelectionChangedEventHandler(cmbJoinType_SelectionChanged);
            ((StackPanel)MainPanel.Children[lineNumber]).Children.Add(cmbJoinType);


            Button butRemove = new Button();
            butRemove.Name = "btnRemove," + rowNumber;
            butRemove.Content = "X";
            butRemove.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            //butRemove.Width = 14;
            //butRemove.Height = 14;
            butRemove.Click += new RoutedEventHandler(butRemove_Click);
            butRemove.Style = Application.Current.Resources["Remove2"] as Style;
            butRemove.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            butRemove.Cursor = Cursors.Hand;
            //butRemove.Margin = new Thickness(0, 5, 47, -2);
            butRemove.Margin = new Thickness(4, 2, 2, 2);

            ((StackPanel)MainPanel.Children[lineNumber]).Children.Add(butRemove);
        }

        /// <summary>
        /// Responds to selectionChanged Event of Join Type ComboBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cmbJoinType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //btnApply.IsEnabled = true;
            //raise an event that control is in Edit mode: TBD
        }

        /// <summary>
        /// Responds to Remove Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void butRemove_Click(object sender, RoutedEventArgs e)
        {
            string buttonName = ((Button)sender).Name;
            int rowNumber = Convert.ToInt32(buttonName.Substring(buttonName.IndexOf(",") + 1, buttonName.Length - buttonName.IndexOf(",") - 1));
            if (MainPanel.Children.Count == 0)
            {
                //StackPanel pnlC = (StackPanel)ApplicationViewModel.Instance.TempDataStorage["FilterConditionalPanel"];
                //pnlC.Children.Remove(FindStackPanel(rowNumber, "sp"));
            }
            else
            {
                MainPanel.Children.Remove(FindStackPanel(rowNumber, "sp"));
            }

            rowRemoved.Add(rowNumber);
            //btnApply.IsEnabled = true;
            //raise an event that control is in Edit mode: TBD
            globalRemovedPointer++;
        }

        /// <summary>
        /// responds to Field SelectionChanged Event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (loadingCombos)
            {
                return;
            }

            if (((ComboBox)sender).SelectedItem != null && ((EwavColumn)((ComboBox)sender).SelectedItem).Name.ToString().Trim() != "")
            {
                string selectedString = "";
                selectedString = ((EwavColumn)((ComboBox)sender).SelectedItem).Name.ToString();

                ColumnDataType dataType = LookUpDataType(selectedString);

                int rowNumber = Convert.ToInt32(((ComboBox)sender).Name.ToString().Split(new Char[] { ',' })[1]);

                FillOperatorValues(dataType);
                string controlName = "operator";
                Control elemnt = FindControlInRow(rowNumber, controlName);
                if (elemnt != null)
                {
                    ((ComboBox)elemnt).ItemsSource = null;
                    ((ComboBox)elemnt).ItemsSource = operatorsList;
                    ((ComboBox)elemnt).SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// Responds to Operator Selection Chnaged Event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cmbOp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if ((((ComboBox)sender).SelectedItem != null))
            {
                int rowNumber = Convert.ToInt32(((ComboBox)sender).Name.ToString().Split(new Char[] { ',' })[1]);
                string selectedString = "";
                selectedString = ((EwavColumn)((ComboBox)FindControlInRow(rowNumber, "field")).SelectedItem).Name.ToString();

                ColumnDataType dataType = LookUpDataType(selectedString);

                StackPanel subSp = new StackPanel();
                subSp.Name = "subSp" + "," + globalPointer;
                string operatorSelected = ((ComboBox)sender).SelectedItem.ToString();

                StackPanel ctrlContainer = new StackPanel();
                ctrlContainer = FindStackPanel(rowNumber, "subSp", true);
                ctrlContainer.Children.Clear();
                ctrlContainer.Orientation = Orientation.Horizontal;

                switch (dataType)
                {
                    case ColumnDataType.Text:

                        TextBox tb = new TextBox();
                        tb.Width = 200;
                        tb.Margin = new Thickness(4, 2, 2, 2);
                        tb.Name = "tb" + "," + rowNumber;
                        tb.TextChanged += new TextChangedEventHandler(tb_TextChanged);
                        tb.IsEnabled = SharedStrings.FRIENDLY_OPERATOR_NOT_MISSING == operatorSelected ? false : true;

                        if (currentCondition != null && currentCondition.FriendlyValue != null)
                        {
                            tb.Text = currentCondition.FriendlyValue.VarName;
                        }
                        ctrlContainer.Name = "subSp" + "," + rowNumber;
                        ctrlContainer.Children.Add(tb);
                        break;
                    case ColumnDataType.Numeric:
                        if (SharedStrings.FRIENDLY_OPERATOR_BETWEEN == operatorSelected)
                        {
                            tb = new TextBox();
                            tb.Name = "tb" + rowNumber + "1";
                            tb.Width = 83;
                            tb.Margin = new Thickness(4, 2, 2, 2);
                            tb.LostFocus += new RoutedEventHandler(tb_LostFocus);
                            if (currentCondition != null && currentCondition.FriendLowValue != null)
                            {
                                tb.Text = currentCondition.FriendLowValue.VarName;
                            }

                            tb.TextChanged += new TextChangedEventHandler(tb_TextChanged);

                            ctrlContainer.Name = "subSp" + "," + rowNumber;
                            ctrlContainer.Children.Add(tb);
                            TextBlock tblk = new TextBlock();
                            tblk.Name = "between" + rowNumber;
                            //tblk.Width = 40;
                            tblk.Text = " and ";
                            tblk.Margin = new Thickness(0, 0, 1, 0);
                            tblk.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                            ctrlContainer.Children.Add(tblk);
                            tb = new TextBox();
                            tb.Name = "tb" + rowNumber + "2";
                            tb.Width = 83;
                            tb.Margin = new Thickness(4, 2, 2, 2);
                            tb.LostFocus += new RoutedEventHandler(tb_LostFocus);
                            if (currentCondition != null && currentCondition.FriendHighValue != null)
                            {
                                tb.Text = currentCondition.FriendHighValue.VarName;
                            }
                            tb.TextChanged += new TextChangedEventHandler(tb_TextChanged);
                            ctrlContainer.Children.Add(tb);
                        }
                        else
                        {
                            tb = new TextBox();
                            tb.Width = 200;
                            tb.Margin = new Thickness(4, 2, 2, 2);
                            tb.Name = "tb" + "," + rowNumber;
                            tb.LostFocus += new RoutedEventHandler(tb_LostFocus);
                            if (currentCondition != null && currentCondition.FriendlyValue != null)
                            {
                                tb.Text = currentCondition.FriendlyValue.VarName;
                            }

                            tb.TextChanged += new TextChangedEventHandler(tb_TextChanged);
                            tb.IsEnabled = SharedStrings.FRIENDLY_OPERATOR_NOT_MISSING == operatorSelected ? false : true;
                            ctrlContainer.Name = "subSp" + "," + rowNumber;
                            ctrlContainer.Children.Add(tb);
                        }
                        break;
                    case ColumnDataType.Boolean:
                        ComboBox cmbBool = new ComboBox();
                        cmbBool.Name = "value" + globalPointer;
                        cmbBool.Width = 200;
                        cmbBool.Margin = new Thickness(4, 2, 2, 2);
                        cmbBool.Items.Add("YES");
                        cmbBool.Items.Add("NO");
                        //cmbBool.Items.Add("Missing");
                        cmbBool.SelectionChanged += new SelectionChangedEventHandler(cmbBool_SelectionChanged);

                        if (currentCondition != null && currentCondition.FriendlyValue != null)
                        {
                            cmbBool.SelectedItem = currentCondition.FriendlyValue.VarName;
                        }

                        if (((ComboBox)sender).SelectedItem.ToString() == SharedStrings.FRIENDLY_OPERATOR_NOT_MISSING)
                        {
                            cmbBool.IsEnabled = false;
                        }
                        else
                        {
                            cmbBool.IsEnabled = true;
                        }
                        ctrlContainer.Name = "subSp" + "," + rowNumber;
                        ctrlContainer.Children.Add(cmbBool);
                        break;
                    case ColumnDataType.DateTime:
                        if (SharedStrings.FRIENDLY_OPERATOR_BETWEEN == operatorSelected)
                        {
                            DatePicker dp = new DatePicker();
                            dp.Name = "dp" + rowNumber + "1";
                            //dp.Width = 95;
                            dp.Style = Application.Current.Resources["dpbetween"] as Style;
                            dp.Margin = new Thickness(4, 2, 14, 2);

                            if (currentCondition != null && currentCondition.FriendLowValue != null)
                            {
                                dp.Text = currentCondition.FriendLowValue.VarName;
                            }

                            dp.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(dp_SelectedDateChanged);
                            ctrlContainer.Name = "subSp" + "," + rowNumber;
                            ctrlContainer.Children.Add(dp);
                            TextBlock tblk = new TextBlock();
                            tblk.Name = "between" + rowNumber;
                            //tblk.Width = 20;
                            tblk.Text = " - ";
                            tblk.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                            ctrlContainer.Children.Add(tblk);
                            dp = new DatePicker();
                            dp.Name = "dp" + rowNumber + "2";
                            //dp.Width = 95;
                            dp.Style = Application.Current.Resources["dpbetween"] as Style;
                            dp.Margin = new Thickness(4, 2, 13, 2);

                            if (currentCondition != null && currentCondition.FriendHighValue != null)
                            {
                                dp.Text = currentCondition.FriendHighValue.VarName;
                            }

                            dp.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(dp_SelectedDateChanged);

                            ctrlContainer.Children.Add(dp);
                        }
                        else
                        {
                            DatePicker dp = new DatePicker();
                            dp.Name = "datep," + rowNumber;
                            dp.Width = 190;
                            dp.Style = Application.Current.Resources["dpbetween"] as Style;
                            dp.Margin = new Thickness(4, 2, 12, 2);

                            if (currentCondition != null && currentCondition.FriendlyValue != null)
                            {
                                dp.Text = currentCondition.FriendlyValue.VarName;
                            }

                            dp.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(dp_SelectedDateChanged);
                            dp.IsEnabled = SharedStrings.FRIENDLY_OPERATOR_NOT_MISSING == operatorSelected ? false : true;
                            ctrlContainer.Name = "subSp" + "," + rowNumber;
                            ctrlContainer.Children.Add(dp);
                        }
                        break;
                    case ColumnDataType.UserDefined:
                        break;
                    default:
                        break;
                }

            }

        }

        /// <summary>
        /// Event raised when focus is lost from the field to check for numeric values.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tb_LostFocus(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            string txtValue = ((TextBox)sender).Text;
            //if (!Regex.IsMatch(txtValue, @"^[0-9]*$"))
            foreach (var item in txtValue.ToCharArray())
            {
                if (!ValidNumberChar(item.ToString()))
                {
                    ((TextBox)sender).Text = "";
                    return;
                }
            }

        }

        /// <summary>
        /// Checks to see whether a valid numeric digit was pressed for numeric-only conditions
        /// </summary>
        /// <param name="keyChar">The key that was pressed</param>
        /// <returns>Whether the input was a valid number character</returns>
        private bool ValidNumberChar(string keyChar)
        {
            System.Globalization.NumberFormatInfo numberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;

            if (keyChar == numberFormatInfo.NegativeSign | keyChar == numberFormatInfo.NumberDecimalSeparator | keyChar == numberFormatInfo.PercentDecimalSeparator)
            {
                return true;
            }

            for (int i = 0; i < keyChar.Length; i++)
            {
                char ch = keyChar[i];
                if (!Char.IsDigit(ch))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Responds to selection Changed Event of Bool, YES/NO field.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cmbBool_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //btnApply.IsEnabled = true;
            //raise an event that control is in Edit mode: TBD
        }

        /// <summary>
        /// DatePicker Selection Changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dp_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //btnApply.IsEnabled = true;
            //raise an event that control is in Edit mode: TBD
        }

        public List<EwavDataFilterCondition> Conditions { get; set; }

        internal void Clear()
        {
            MainPanel.Children.Clear();
        }
    }
}