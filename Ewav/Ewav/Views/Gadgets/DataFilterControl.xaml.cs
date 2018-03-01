/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       DataFilterControl.xaml.cs
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

using System.Windows.Media.Animation;
using Ewav.BAL;
using Ewav.ViewModels;
using Ewav.Web.Services;

using System.Windows.Input;

namespace Ewav
{
    /// <summary>
    /// JoinType enumeration.
    /// </summary>
    public enum JoinType
    {
        AND = 0,
        OR
    }

    /// <summary>
    /// Interaction logic for DataFilterControl.xaml
    /// </summary>
    public partial class DataFilterControl : UserControl
    {
        public ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;
        List<EwavColumn> colsList;
        List<string> extractedList, operatorsList, configList;
        int globalPointer = 0, globalRemovedPointer = 0;
        private bool dataFilterSlideCheck = false;
        List<int> rowRemoved = new List<int>();
        private Storyboard SlideIn = new Storyboard();
        private Storyboard SlideOut = new Storyboard();
        List<EwavColumn> SourceColumns;
        bool unsavedIndicator = false;
        //EwavFilterControl filterControl;
        /// <summary>
        /// Default Constructor
        /// </summary>
        public DataFilterControl()
        {
            InitializeComponent();
            SlideOut.Children.Add(CreateSlidingAnim("Slider", 644));
            LayoutRoot.Resources.Add("SlideOut", SlideOut);

            SlideIn.Children.Add(CreateSlidingAnim("Slider", 0));
            LayoutRoot.Resources.Add("SlideIn", SlideIn);
            //ND:Commented Slider.MouseEnter Line so that the tab only slides on MouseLeftButtonDown.Added the Slider.MouseLeftButtonDown
            //Slider.MouseEnter += new MouseEventHandler(Slider_MouseEnter);
            Slider.MouseLeftButtonDown += new MouseButtonEventHandler(Slider_MouseEnter);
            Slider.MouseLeave += new MouseEventHandler(Slider_MouseLeave);
            //btnClear.IsEnabled = false;

            this.Loaded += new RoutedEventHandler(DataFilterControl_Loaded);

            //applicationViewModel.UnloadedEvent += new Client.Application.UnloadedEventHandler(applicationViewModel_UnloadedEvent);

            //this.Unloaded += new RoutedEventHandler(DataFilterControl_Unloaded);

            UnWireEvents();

            WireEvents();

            FilterControl.FilterType = FilterControlType.DataFilter;
            FilterControl.InitializeDataFilter();



        }

        private void WireEvents()
        {
            DashboardMainPage.DragCanvasRightMouseDownEvent += new Client.Application.DragCanvasRightMouseDownEventHandler(DashboardMainPage_DragCanvasRightMouseDownEvent);
            applicationViewModel.FilterStringUpdatedEvent += new Client.Application.FilterStringUpdatedEventHandler(applicationViewModel_FilterStringUpdatedEvent);

            applicationViewModel.ApplyDataFilterEvent += new Client.Application.ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);
            applicationViewModel.DefinedVariableAddedEvent += new Client.Application.DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);
            applicationViewModel.DefinedVariableInUseDeletedEvent += new Client.Application.DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableInUseDeletedEvent);
            applicationViewModel.DefinedVariableNotInUseDeletedEvent += new Client.Application.DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
            applicationViewModel.ClearDataFiltersEvent += new Client.Application.ClearDataFiltersEventHandler(applicationViewModel_ClearDataFiltersEvent);
            applicationViewModel.UpdateDataFilterCount += new Client.Application.UpdateDataFilterCountEventHandler(applicationViewModel_UpdateDataFilterCount);

            //  applicationViewModel.ColumnsLoadedEvent += new Client.Application.ColumnsLoadedEventEventHandler(applicationViewModel_ColumnsLoadedEvent);
          //      applicationViewModel.DatasourceChangedEvent += new Client.Application.DatasourceChangedEventHandler(applicationViewModel_DatasourceChangedEvent);

     

        }

        private void UnWireEvents()
        {
            DashboardMainPage.DragCanvasRightMouseDownEvent -= new Client.Application.DragCanvasRightMouseDownEventHandler(DashboardMainPage_DragCanvasRightMouseDownEvent);
            applicationViewModel.FilterStringUpdatedEvent -= new Client.Application.FilterStringUpdatedEventHandler(applicationViewModel_FilterStringUpdatedEvent);

            applicationViewModel.ApplyDataFilterEvent -= new Client.Application.ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);
            applicationViewModel.DefinedVariableAddedEvent -= new Client.Application.DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);
            applicationViewModel.DefinedVariableInUseDeletedEvent -= new Client.Application.DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableInUseDeletedEvent);
            applicationViewModel.DefinedVariableNotInUseDeletedEvent -= new Client.Application.DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
            applicationViewModel.UpdateDataFilterCount -= new Client.Application.UpdateDataFilterCountEventHandler(applicationViewModel_UpdateDataFilterCount);
        }

        void applicationViewModel_ColumnsLoadedEvent(object o, Client.Application.ColumnsLoadedEventEventArgs e)
        {


      
        }



        void applicationViewModel_UpdateDataFilterCount(object o)
        {
            filterCount.Text = string.Format("Data Filter ({0})", applicationViewModel.EwavDatafilters.Count);
            VerticalHeading.Text = string.Format("Data Filter ({0})", applicationViewModel.EwavDatafilters.Count);
        }

        void applicationViewModel_ClearDataFiltersEvent(object o)
        {
            Clear();
        }



        void DataFilterControl_Loaded(object sender, RoutedEventArgs e)
        {

            if (this.applicationViewModel.EwavSelectedDatasource.AllColumns != null)
            {

            }
        }


        void applicationViewModel_DefinedVariableNotInUseDeletedEvent(object o)
        {
            RefreshDataFilter();
        }

        void applicationViewModel_DefinedVariableInUseDeletedEvent(object o)
        {
            if (ApplicationViewModel.Instance.IsDefVarInUseByDF)
            {
                FilterControl.UserDefinedVarInUse(o);
            }
            else
            {
                RefreshDataFilter();
            }



        }

        void applicationViewModel_DefinedVariableAddedEvent(object o)
        {
            RefreshDataFilter();
        }
        bool loadingCombos = false;

        public void RefreshDataFilter()
        {
            //FillSelectionComboboxes();


            ////((ComboBox)FindControlInRow(rowNumber, "field"))


            //for (int i = 0; i <= this.globalPointer; i++)
            //{
            //    if (FindControlInRow(i, "field") != null)
            //    {
            //        string selectedValue = ((EwavColumn)((ComboBox)FindControlInRow(i, "field")).SelectedItem).Name;
            //        loadingCombos = true;
            //        ComboBox cmb = ((ComboBox)FindControlInRow(i, "field"));
            //        cmb.ItemsSource = null;
            //        cmb.ItemsSource = colsList; // extractedList; 
            //        SetSelectedIndex(cmb, selectedValue);
            //        loadingCombos = false;
            //    }

            //}
            FilterControl.RefreshDataFilter();

        }

        void DashboardMainPage_DragCanvasRightMouseDownEvent(DashboardMainPage d)
        {


            SlideIn.Begin();
            rotatearrow1.Begin();
            dataFilterSlideCheck = false;

        }

        void applicationViewModel_ApplyDataFilterEvent(object o)
        {
            filterCount.Text = string.Format("Data Filter ({0})", applicationViewModel.EwavDatafilters.Count());
        }

        void applicationViewModel_FilterStringUpdatedEvent(object o)
        {
            txtAdvancedFilter.Text = applicationViewModel.FilterString;
        }


        private void btnAdvMode_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            pnlAdvancedMode.Visibility = System.Windows.Visibility.Visible;
            pnlGuidedMode.Visibility = System.Windows.Visibility.Collapsed;
            btnAdvMode.Visibility = System.Windows.Visibility.Collapsed;
            btnGuidedMode.Visibility = System.Windows.Visibility.Visible;
            applicationViewModel.UseAdvancedFilter = true;
        }

        /// <summary>
        /// Button Apply Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            DoDataFilter();
        }

        public void DoDataFilter()
        {
            if (applicationViewModel.UseAdvancedFilter)
            {
                applicationViewModel.AdvancedDataFilterString = txtAdvancedFilter.Text;
                VerticalHeading.Text = "Data Filters";
            }
            else
            {
                List<EwavDataFilterCondition> DFilters = FilterControl.CreateDataFilters();
                if (DFilters == null)
                {
                    return;
                }
                applicationViewModel.EwavDatafilters = DFilters;
            }

            SlideIn.Begin();
            rotatearrow1.Begin();
            dataFilterSlideCheck = false;
        }

        /// <summary>
        /// Clear button event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            txtAdvancedFilter.Text = "";

            applicationViewModel.AdvancedDataFilterString = string.Empty;
            applicationViewModel.EwavDatafilters = new List<EwavDataFilterCondition>();
            FilterControl.GlobalPointer = 0;
            FilterControl.GlobalRemovedPointer = 0;
            filterCount.Text = "Data Filter (0)";
            VerticalHeading.Text = "Data Filter (0)";
            FilterControl.Clear();
            FilterControl.CreateFilterConditionRow();
            //SlideIn.Begin();
            rotatearrow1.Begin();
            dataFilterSlideCheck = false;
        }

        public void CreateFromXml(List<EwavDataFilterCondition> Conditions, string advancedFilterString)
        {
            if (advancedFilterString != null &&
                advancedFilterString.Length != 0)
            {
                pnlGuidedMode.Visibility = System.Windows.Visibility.Collapsed;
                pnlAdvancedMode.Visibility = System.Windows.Visibility.Visible;
                VerticalHeading.Text = "Data Filters";
                btnAdvMode.Visibility = System.Windows.Visibility.Collapsed;
                btnGuidedMode.Visibility = System.Windows.Visibility.Visible;
            }

            if (FilterControl == null)
            {
                FilterControl = (EwavFilter)Activator.CreateInstance(typeof(EwavFilter));
                FilterControl.FilterType = FilterControlType.DataFilter;
            }

            FilterControl.ConstructStackPanelFromDataFilters(Conditions);

        }

        private void btnGuidedMode_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            pnlAdvancedMode.Visibility = System.Windows.Visibility.Collapsed;
            pnlGuidedMode.Visibility = System.Windows.Visibility.Visible;
            btnAdvMode.Visibility = System.Windows.Visibility.Visible;
            btnGuidedMode.Visibility = System.Windows.Visibility.Collapsed;
            applicationViewModel.UseAdvancedFilter = false;
        }

        private void btnNewCondition_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SlideIn.Begin();
            rotatearrow1.Begin();
            dataFilterSlideCheck = false;
        }


        /// <summary>
        /// Handles the KeyDown event of the LayoutRoot control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs" /> instance containing the event data.</param>
        private void LayoutRoot_KeyDown(object sender, KeyEventArgs e)
        {




            int x;

            TextBox tb = (TextBox)sender;

            // Determine whether the keystroke is a number from the top of the keyboard.
            if (e.Key < Key.D0 || e.Key > Key.D9)
            {
                // Determine whether the keystroke is a number from the keypad.  
                if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
                {



                    e.Handled = true;


                }

            }


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

        /// <summary>
        /// Date picker date changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dp_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            btnApply.IsEnabled = true;
        }


        private void LayoutRoot_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // SlideIn.Begin();
        }

        private void LayoutRoot_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // TODO: Add event handler implementation here.
        }


        private void RectSlideIn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {


            if (dataFilterSlideCheck != false)
            {
                SlideIn.Begin();
                rotatearrow1.Begin();
                dataFilterSlideCheck = false;
            }
            else
            {
                SlideOut.Begin();
                rotatearrow.Begin();
                dataFilterSlideCheck = true;
            }

        }

        private void Slider_MouseEnter(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dataFilterSlideCheck == false)
            {
                SlideIn.Begin();
                rotatearrow1.Begin();
                dataFilterSlideCheck = false;
            }
            else
            {
                SlideOut.Begin();
                rotatearrow.Begin();
                dataFilterSlideCheck = true;
            }
        }

        private void Slider_MouseLeave(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// Text Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tb_TextChanged(object sender, TextChangedEventArgs e)
        {


            if (((TextBox)sender).Text != "")
            {
                btnApply.IsEnabled = true;
                //btnClear.IsEnabled = true;
            }






        }

        private void txtAdvancedFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (pnlAdvancedMode.Visibility == System.Windows.Visibility.Visible)
            {
                btnApply.IsEnabled = true;
            }
        }
    }
}