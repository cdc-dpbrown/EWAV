/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       VariablesControl.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Ewav.ViewModels;
using Ewav.Web.Services;
using System.Collections.Generic;
using Ewav.ContextMenu;

namespace Ewav
{
    public partial class VariablesControl : UserControl
    {
        private Storyboard SlideOut = new Storyboard();
        private Storyboard SlideIn = new Storyboard();
        private bool variableSliderCheck = false;
        RecodedVariable rv;
        FormattedValue fv;
        SimpleAssignment sa;
        AssignedExpression ae;
        public ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;
        public VariablesControl()
        {
            // Required to initialize variables
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(VariablesControl_Loaded);
            this.btnEditRule.IsEnabled = false;
            this.btnRemoveRule.IsEnabled = false;
            applicationViewModel.RulesAddedEvent += new Client.Application.RulesAddedEventHandler(applicationViewModel_RulesAddedEvent);
            applicationViewModel.UnloadedEvent += new Client.Application.UnloadedEventHandler(applicationViewModel_UnloadedEvent);
            applicationViewModel.ClearDefinedVariablesEvent += new Client.Application.ClearDefinedVariablesEventHandler(applicationViewModel_ClearDefinedVariablesEvent);
        }

        void applicationViewModel_ClearDefinedVariablesEvent(object o)
        {
            applicationViewModel.EwavDefinedVariables = new System.Collections.Generic.List<EwavRule_Base>();
            applicationViewModel.EwavDatasourceSelectedIndex = applicationViewModel.EwavDatasourceSelectedIndex;
            lbxRules.ItemsSource = null;
        }

        void applicationViewModel_UnloadedEvent(object o)
        {
            lbxRules.ItemsSource = null;
            //applicationViewModel.RulesAddedEvent -= new Client.Application.RulesAddedEventHandler(applicationViewModel_RulesAddedEvent);
        }

        void applicationViewModel_RulesAddedEvent(object o)
        {
            BindListBox();
        }

        void VariablesControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeControl();

        }

        private void InitializeControl()
        {


            DashboardMainPage.DragCanvasRightMouseDownEvent += new Client.Application.DragCanvasRightMouseDownEventHandler(DashboardMainPage_DragCanvasRightMouseDownEvent);
            SlideOut.Children.Add(CreateSlidingAnim("VSlider", 624));
            LayoutRoot.Resources.Add("SlideOut", SlideOut);

            SlideIn.Children.Add(CreateSlidingAnim("VSlider", 0));
            LayoutRoot.Resources.Add("SlideIn", SlideIn);
            applicationViewModel.ListOfRules = new System.Collections.Generic.List<ListBoxItemSource>();
            //ND:Commented VSlider.MouseEnter Line so that the tab only slides on MouseLeftButtonDown.Added the VSlider.MouseLeftButtonDown
			//VSlider.MouseEnter += new MouseEventHandler(Slider_MouseEnter);
            VSlider.MouseLeftButtonDown += new MouseButtonEventHandler(Slider_MouseEnter);
			VSlider.MouseLeave += new MouseEventHandler(Slider_MouseLeave);
            //     applicationViewModel.EwavDefinedVariables = new System.Collections.Generic.List<EwavRule_Recode>();

            //if (applicationViewModel.SelectedEwavColumnToAdd == null)
            //{
            //    applicationViewModel.SelectedEwavColumnToAdd = new List<BAL.EwavColumn>();
            //}
            //else
            //{
            //    applicationViewModel.SelectedEwavColumnToAdd.Clear();
            //}
            applicationViewModel.EwavDefinedVariables = new System.Collections.Generic.List<EwavRule_Base>();
        }



        void DashboardMainPage_DragCanvasRightMouseDownEvent(DashboardMainPage d)
        {


            SlideIn.Begin();
            rotatearrow1.Begin();
            variableSliderCheck = false;
        }





        private DoubleAnimation CreateSlidingAnim(string elementName, double toValue)
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.SetValue(Storyboard.TargetNameProperty, elementName);
            animation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)"));
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            animation.To = toValue;
            return animation;
        }
        private void Slider_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void Slider_MouseEnter(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            //SlideOut.Begin();
            //rotatearrow.Begin();
            //variableSliderCheck = true;
            if (variableSliderCheck == false)
            {
                SlideIn.Begin();
                rotatearrow1.Begin();
                variableSliderCheck = false;
            }
            else
            {
                SlideOut.Begin();
                rotatearrow.Begin();
                variableSliderCheck = true;
            }
		}
		private void MenuList_ItemSelected(object sender, MenuEventArgs e)
		{
			switch (e.Tag.ToString())
			{
				case "varRecode":
					//rv = new RecodedVariable(false);
					//rv.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
					//rv.VerticalAlignment = System.Windows.VerticalAlignment.Center;
					//rv.Show();
					//rv.Closed += new EventHandler(rv_Closed);
					break;
				case "varFormat":
					//FormattedValue fv = new FormattedValue(); 
					// fv.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
					// fv.VerticalAlignment = System.Windows.VerticalAlignment.Center;
					// fv.Show();
					// //fv.Closed += new EventHandler(fv_Closed);
					break;
				case "varSimple":
					//SimpleAssignment sa = new SimpleAssignment();
					// sa.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
					// sa.VerticalAlignment = System.Windows.VerticalAlignment.Center;
					// sa.Show();
					//sa.Closed += new EventHandler(sa_Closed);
					break;
				case "varAssign":
					//ae = new AssignedExpression(false);
					// ae.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
					//ae.VerticalAlignment = System.Windows.VerticalAlignment.Center;
					//ae.Show();
					break;


                default:
                    //ConditionalAssign  co = new ConditionalAssign();
                    //co.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    //co.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    //co.Show();
                    break;
            }
           // menuVar.Hide();
            menuVar.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void btnNewRule_Click(object sender, RoutedEventArgs e)
        {
            //menuVar.Show();
            menuVar.Visibility = System.Windows.Visibility.Visible;
            //ae = new AssignedExpression(false);
            //ae.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            //ae.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            //ae.Show();

            //sa = new SimpleAssignment();
            //sa.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            //sa.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            //sa.Show();

            //fv = new FormattedValue();
            //fv.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            //fv.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            //fv.Show();

            //ConditionalAssign ca = new ConditionalAssign();
            //ca.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            //ca.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            //ca.Show();

            //rv = new RecodedVariable(false);
            //rv.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            //rv.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            //rv.Show();
            //rv.Closed += new EventHandler(rv_Closed);
            //ContextMenu.Menu menu = new ContextMenu.Menu();
            //ContextMenu.MenuItem item = new ContextMenu.MenuItem("Recoded Variable");
            //menu.Items.a


            //rv = new RecodedVariable(false);
            //rv.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            //rv.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            //rv.Show();
            //rv.Closed += new EventHandler(rv_Closed);

            //if (rv)
            //{

            //}
        }

        void rv_Closed(object sender, EventArgs e)
        {
            BindListBox();
        }

        /// <summary>
        /// Binds ListBox Control
        /// </summary>
        private void BindListBox()
        {

            lbxRules.ItemsSource = null;
            lbxRules.ItemsSource = applicationViewModel.ListOfRules;
            lbxRules.DisplayMemberPath = "RuleString";
            lbxRules.SelectedValuePath = "SourceColumn";
            filterCount.Text = string.Format("Create User Defined Variables ({0})", applicationViewModel.ListOfRules.Count);
            VerticalHeading.Text = string.Format("Defined Variables ({0})", applicationViewModel.ListOfRules.Count);
            EnableDisableEditRemove();
        }
        private void btnEditRule_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //SlideIn.Begin();
            if (lbxRules.SelectedIndex > -1)
            {
                switch (((ListBoxItemSource)lbxRules.SelectedItem).RuleType)
                {
                    case EwavRuleType.Recode:
                        rv = new RecodedVariable(true, (ListBoxItemSource)lbxRules.SelectedItem);
                        rv.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        rv.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        //rv.SelectedItem = ;
                        rv.Show();
                        rv.Closed += new EventHandler(rv_Closed);
                        break;
                    case EwavRuleType.Assign:

                        ae = new AssignedExpression(true, (ListBoxItemSource)lbxRules.SelectedItem);
                        ae.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        ae.VerticalAlignment = System.Windows.VerticalAlignment.Center;

                        ae.Show();
                        ae.Closed += new EventHandler(rv_Closed);
                         

                        break;
                    case EwavRuleType.Formatted:
                        fv = new FormattedValue(true, (ListBoxItemSource)lbxRules.SelectedItem);
                        fv.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        fv.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        fv.Show();
                        fv.Closed += new EventHandler(rv_Closed);
                        break;
                    case EwavRuleType.Simple:
                        sa = new SimpleAssignment(true, (ListBoxItemSource)lbxRules.SelectedItem);
                        sa.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        sa.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        sa.Show();
                        sa.Closed += new EventHandler(rv_Closed);
                        break;
                    case EwavRuleType.conditional:
                        ConditionalAssign ca = new ConditionalAssign(true, (ListBoxItemSource)lbxRules.SelectedItem);
                        ca.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        ca.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        ca.Show();
                        ca.Closed += new EventHandler(rv_Closed);
                        break;
                    case EwavRuleType.GroupVariable:
                        GroupVariable gv = new GroupVariable(true, (ListBoxItemSource)lbxRules.SelectedItem);
                        gv.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        gv.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        gv.Show();
                        gv.Closed += new EventHandler(rv_Closed);
                        break;
                    default:
                        break;
                }
                //if (((ListBoxItemSource)lbxRules.SelectedItem).RuleType  == EwavRuleType.Recode)
                //{

                //}

            }


        }
        private void btnRemoveRule_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (lbxRules.SelectedIndex < 0)
            {
                return;
            }
            int colindex = -1;
            string colName = string.Empty;
            //bool varUsedByAnotherVar = false;
            applicationViewModel.DeletedVariableInUse = false;
            for (int i = 0; i < applicationViewModel.EwavSelectedDatasource.AllColumns.Count; i++)
            {
                if (((ListBoxItemSource)lbxRules.SelectedItems[0]).DestinationColumn.Equals(applicationViewModel.EwavSelectedDatasource.AllColumns[i].Name))
                {
                    colindex = i;
                    colName = applicationViewModel.EwavSelectedDatasource.AllColumns[i].Name;
                    break;
                }
            }

            //for (int i = 0; i < applicationViewModel.ListOfDefinedVarsInUseByAnotherVar.Count; i++)
            //{
            //    if (applicationViewModel.ListOfDefinedVarsInUseByAnotherVar[i].Name.ToLower() == colName.ToLower())
            //    {
            //        varUsedByAnotherVar = true;
            //    }
            //}varUsedByAnotherVar ||

            //Handles if it is being used by another variable.
            if (applicationViewModel.EwavSelectedDatasource.AllColumns[colindex].ChildVariableName != null && applicationViewModel.EwavSelectedDatasource.AllColumns[colindex].ChildVariableName.Length > 0)
            {
                MessageBox.Show("This variable cannot be deleted since it is being used by another defined variable. Please delete the dependent variables before deleting this variable.",
   "Warning", MessageBoxButton.OK);
                return;
            }

            //Handles if it being used by another gadget.

            if (applicationViewModel.EwavSelectedDatasource.AllColumns[colindex].IsInUse)
            {
                applicationViewModel.DeletedVariableInUse = true;
                MessageBoxResult result = MessageBox.Show("This variable is being used by one or more gadgets. The gadgets will have their properties reset if the variable is deleted. Would you like to delete this variable?",
    "Warning", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            int index = -1;
            string selectedItem = ((ListBoxItemSource)lbxRules.SelectedItems[0]).DestinationColumn;
            List<ListBoxItemSource> source = applicationViewModel.ListOfRules;

            List<EwavRule_Base> listOfRules = new List<EwavRule_Base>();
            for (int i = 0; i < applicationViewModel.EwavDefinedVariables.Count; i++)
            {
                listOfRules.Add(applicationViewModel.EwavDefinedVariables[i]);
            }

            //for (int i = 0; i < lbxRules.Items.Count; i++)
            //{
            //    if (listOfRules[i] is EwavRule_Recode)
            //    {
            //        EwavRule_Recode rule = listOfRules[i] as EwavRule_Recode;


            //        if (selectedItem == rule.TxtDestinationField)
            //        {
            //            index = i;
            //        }
            //    }
            //    else if (listOfRules[i] is EwavRule_SimpleAssignment)
            //    {
            //        EwavRule_SimpleAssignment rule = listOfRules[i] as EwavRule_SimpleAssignment;
            //        if (selectedItem == rule.TxtDestinationField)
            //        {
            //            index = i;
            //        }

            //    }
            //    else
            //    {
            //        throw new Exception(" rule error for " + listOfRules[i].GetType().ToString());
            //    }
            //}

            for (int i = 0; i < lbxRules.Items.Count; i++)
            {
                if (selectedItem == listOfRules[i].VaraiableName)
                {
                    index = i;
                    break;
                }
            }
            if (index > -1)
            {
                source.RemoveAt(index);
                listOfRules.RemoveAt(index);
            }

            lbxRules.ItemsSource = null;
            lbxRules.ItemsSource = source;
            filterCount.Text = string.Format("Defined Variables ({0})", applicationViewModel.ListOfRules.Count);
            VerticalHeading.Text = string.Format("Defined Variables ({0})", applicationViewModel.ListOfRules.Count);
            index = -1;

            EwavRule_Base Rule = new EwavRule_Base();

            //for (int i = 0; i < listOfRules.Count; i++)
            //{
            //    if (listOfRules[i] is EwavRule_Recode)
            //    {
            //        EwavRule_Recode rule = listOfRules[i] as EwavRule_Recode;

            //        if (selectedItem == rule.TxtDestinationField)
            //        {
            //            index = i;
            //        }
            //    }
            //    else
            //    {


            //        throw new Exception(" rule error for " + listOfRules[i].GetType().ToString());

            //    }
            //}

            //for (int i = 0; i < listOfRules.Count; i++)
            //{
            //    if (selectedItem == listOfRules[i].VaraiableName)
            //    {
            //        index = i;
            //        break;
            //    }
            //}


            applicationViewModel.InvokePreColumnChangedEvent();
            colName = applicationViewModel.EwavSelectedDatasource.AllColumns[colindex].Name;

            applicationViewModel.RemoveIndicator = true;
            applicationViewModel.ItemToBeRemoved = applicationViewModel.EwavSelectedDatasource.AllColumns[colindex];
            applicationViewModel.EwavSelectedDatasource.AllColumns.RemoveAt(colindex);
            applicationViewModel.EwavDefinedVariables = listOfRules;
            applicationViewModel.RemoveIndicator = false;

            //searches for any reference of deleted column. and removes the dependencies.
            for (int i = 0; i < applicationViewModel.EwavSelectedDatasource.AllColumns.Count; i++)
            {
                if (applicationViewModel.EwavSelectedDatasource.AllColumns[i].ChildVariableName != null &&
                    applicationViewModel.EwavSelectedDatasource.AllColumns[i].ChildVariableName == colName)
                {
                    applicationViewModel.EwavSelectedDatasource.AllColumns[i].ChildVariableName = string.Empty;
                }
            }

            EnableDisableEditRemove();
        }

        /// <summary>
        /// Enables/Disables the Edit/Remove button
        /// </summary>
        private void EnableDisableEditRemove()
        {
            if (applicationViewModel.ListOfRules.Count == 0)
            {
                this.btnEditRule.IsEnabled = false;
                this.btnRemoveRule.IsEnabled = false;
            }
            else
            {
                this.btnEditRule.IsEnabled = true;
                this.btnRemoveRule.IsEnabled = true;
            }
        }

        private void LayoutRoot_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // SlideIn.Begin();
           // menuVar.Hide();
            menuVar.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void RectSlideIn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //SlideIn.Begin();
            //rotatearrow1.Begin();
            //menuVar.Hide();
            menuVar.Visibility = System.Windows.Visibility.Collapsed;

            if (variableSliderCheck != false)
            {
                SlideIn.Begin();
                rotatearrow1.Begin();
                variableSliderCheck = false;
            }
            else
            {
                SlideOut.Begin();
                rotatearrow.Begin();
                variableSliderCheck = true;
            }
        }

        private void LayoutRoot_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // TODO: Add event handler implementation here.
            //menuVar.Hide();
            menuVar.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void varRecode_MouseLeftButtonDown(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            rv = new RecodedVariable();
            rv.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            rv.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            rv.Show();
            rv.Closed += new EventHandler(rv_Closed);
            //menuVar.Hide();
            menuVar.Visibility = System.Windows.Visibility.Collapsed;
            //menuVar.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void varFormat_MouseLeftButtonDown(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            fv = new FormattedValue();
            fv.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            fv.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            fv.Show();
            fv.Closed += new EventHandler(rv_Closed);
            //menuVar.Hide();
            menuVar.Visibility = System.Windows.Visibility.Collapsed;
            //menuVar.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void varSimple_MouseLeftButtonDown(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            sa = new SimpleAssignment();
            sa.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            sa.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            sa.Show();
            sa.Closed += new EventHandler(rv_Closed);
            //menuVar.Hide();
            menuVar.Visibility = System.Windows.Visibility.Collapsed;
            //menuVar.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void varAssign_MouseLeftButtonDown(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            ae = new AssignedExpression(false);
            ae.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            ae.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            ae.Show();
            //menuVar.Hide();
            menuVar.Visibility = System.Windows.Visibility.Collapsed;
            ae.Closed += new EventHandler(rv_Closed);

            //menuVar.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void varCondition_MouseLeftButtonDown(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            ConditionalAssign ca = new ConditionalAssign();
            ca.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            ca.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            ca.Show();
            //menuVar.Hide();
            menuVar.Visibility = System.Windows.Visibility.Collapsed;
            ca.Closed += new EventHandler(rv_Closed);
            //menuVar.Visibility = System.Windows.Visibility.Collapsed;
        }
		private void varGroup_MouseLeftButtonDown(object sender, System.Windows.RoutedEventArgs e)
		{
			// TODO: Add event handler implementation here.
			GroupVariable gv = new GroupVariable();
			gv.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
			gv.VerticalAlignment = System.Windows.VerticalAlignment.Center;
			gv.Show();
			//menuVar.Hide();
			menuVar.Visibility = System.Windows.Visibility.Collapsed;
			gv.Closed += new EventHandler(rv_Closed);
			//menuVar.Visibility = System.Windows.Visibility.Collapsed;
		}



        private void menuVar_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // TODO: Add event handler implementation here.
            //menuVar.Hide();
            menuVar.Visibility = System.Windows.Visibility.Collapsed;
        }

       




    }
}