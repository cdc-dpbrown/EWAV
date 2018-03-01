/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       CombinedFrequency.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using CommonLibrary;
using Ewav.Web.EpiDashboard;
using Ewav.BAL;
using Ewav.ViewModels;
using Ewav.Web.Services;
using Ewav.Services;
using System.ComponentModel;
using System.ServiceModel.DomainServices.Client;
using System.Xml.Linq;
using Ewav.ViewModels.Gadgets;
using Ewav.Common;
using System.Globalization;
using Ewav.ExtensionMethods;

namespace Ewav
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(UserControl))]
    [ExportMetadata("type", "gadget")]
    [ExportMetadata("tabindex", "100")]
    public partial class CombinedFrequency : UserControl, IGadget, IEwavGadget, ICustomizableGadget
    {

        /// <summary>
        /// Common Class reference
        /// </summary>
        ClientCommon.Common cmnClass;

        CombinedFrequencyViewModel combinedFreqVM;

        int Index1 = 0;

        EwavColumn Col1;

        TextBlock txtDenom;


        ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;

        EwavCombinedFrequencyGadgetParameters combinedParameters;

        public List<EwavDataFilterCondition> GadgetFilters { get; set; }

        private bool loadingDropDowns = false;



        /// <summary>
        /// Gets or sets the set labels popup.
        /// </summary>
        /// <value>The set labels popup.</value>
        public SetLabels setLabelsPopup { get; set; }


        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        /// <value>The view model.</value>
        public SetLabelsViewModel viewModel { get; set; }


        /// <summary>
        /// Handles the Click event of the HeaderButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        public void HeaderButton_Click(object sender, RoutedEventArgs e)
        {
            SetChartLabels();

        }

        /// <summary>
        /// Handles the Unloaded event of the window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        public void window_Unloaded(object sender, RoutedEventArgs e)
        {
            if (setLabelsPopup.DialogResult.Value)
            {
                SetHeaderAndFooter();
            }
        }

        /// <summary>
        /// Sets the chart labels.
        /// </summary>
        public void SetChartLabels()
        {
            LoadViewModel();
            setLabelsPopup = new SetLabels(MyControlName, viewModel, true);

            setLabelsPopup.Unloaded -= new RoutedEventHandler(window_Unloaded);
            setLabelsPopup.Unloaded += new RoutedEventHandler(window_Unloaded);



            setLabelsPopup.Show();


        }

        /// <summary>
        /// Sets the header and footer.
        /// </summary>
        public void SetHeaderAndFooter()
        {
            // set header / footer / title    
            viewModel = (SetLabelsViewModel)setLabelsPopup.DataContext;
            txtGadgetDescription.Text = viewModel.GadgetDescription;

            GName.Text = viewModel.GadgetTitle;
        }

        /// <summary>
        /// Loads the view model.
        /// </summary>
        public void LoadViewModel()
        {
            viewModel = new SetLabelsViewModel();
            viewModel.GadgetTitle = GName.Text;
            viewModel.GadgetDescription = txtGadgetDescription.Text;
            viewModel.Footnote = "Footnote";
        }

        public bool LoadingDropDowns
        {
            get { return loadingDropDowns; }
            set { loadingDropDowns = value; }
        }

        private bool loadingCanvas = false;

        public bool LoadingCanvas
        {
            get { return loadingCanvas; }
            set { loadingCanvas = value; }
        }


        List<ColumnDataType> fieldPrimaryDataTypesList;
        public List<ColumnDataType> GetFieldPrimaryDataType
        {
            get
            {
                fieldPrimaryDataTypesList = new List<ColumnDataType>();
                fieldPrimaryDataTypesList.Add(ColumnDataType.Boolean);
                fieldPrimaryDataTypesList.Add(ColumnDataType.Numeric);
                fieldPrimaryDataTypesList.Add(ColumnDataType.Text);
                fieldPrimaryDataTypesList.Add(ColumnDataType.UserDefined);
                fieldPrimaryDataTypesList.Add(ColumnDataType.GroupVariable);
                return fieldPrimaryDataTypesList;
            }
        }


        private GadgetParameters gadgetOptions;
        /// <summary>
        /// Gets/sets the gadget options
        /// </summary>
        public GadgetParameters GadgetOptions
        {
            get
            {
                return this.gadgetOptions;
            }
            set
            {
                this.gadgetOptions = value;
            }
        }


        public CombinedFrequency()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(CombinedFrequency_Loaded);
            FillComboboxes();
            cmbCombineMode.SelectionChanged += new SelectionChangedEventHandler(cmbCombineMode_SelectionChanged);
        }

        void CombinedFrequency_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeControl();
        }

        public void InitializeControl()
        {
            try
            {
                combinedFreqVM = (CombinedFrequencyViewModel)this.DataContext;

                combinedFreqVM.FrequencyTableLoadedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(cfViewModel_FrequencyTableLoadedEvent);

                combinedFreqVM.ErrorNotice += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(frequencyViewModel_ErrorNotice);
                applicationViewModel.ApplyDataFilterEvent += new Client.Application.ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);

                applicationViewModel.DefinedVariableAddedEvent += new Client.Application.DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);
                applicationViewModel.DefinedVariableInUseDeletedEvent += new Client.Application.DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableInUseDeletedEvent);
                applicationViewModel.DefinedVariableNotInUseDeletedEvent += new Client.Application.DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
                applicationViewModel.PreColumnChangedEvent += new Client.Application.PreColumnChangedEventHandler(applicationViewModel_PreColumnChangedEvent);
                applicationViewModel.UnloadedEvent += new Client.Application.UnloadedEventHandler(applicationViewModel_UnloadedEvent);
                //  frequencyViewModel.GetColumns("NEDS", "vwExternalData");    
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Frequency Control. Try again.");
                return;
            }

            this.cmnClass = new ClientCommon.Common();
            this.Construct();
            //this.RenderFinish();
        }

        void applicationViewModel_UnloadedEvent(object o)
        {
            UnloadGadget();
        }

        private void UnloadGadget()
        {
            applicationViewModel.ApplyDataFilterEvent -= new Client.Application.ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);

            applicationViewModel.DefinedVariableAddedEvent -= new Client.Application.DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);
            applicationViewModel.DefinedVariableInUseDeletedEvent -= new Client.Application.DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableInUseDeletedEvent);
            applicationViewModel.DefinedVariableNotInUseDeletedEvent -= new Client.Application.DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
            applicationViewModel.PreColumnChangedEvent -= new Client.Application.PreColumnChangedEventHandler(applicationViewModel_PreColumnChangedEvent);
        }
        void applicationViewModel_PreColumnChangedEvent(object o)
        {
            SaveColumnValues();
        }

        void applicationViewModel_DefinedVariableNotInUseDeletedEvent(object o)
        {
            SearchIndex();
            LoadingDropDowns = true;
            FillComboboxes();
            LoadingDropDowns = false;
            //RefreshResults();
            DoCombinedFrequency();
        }

        void applicationViewModel_DefinedVariableInUseDeletedEvent(object o)
        {
            ResetGadget();
        }

        void applicationViewModel_DefinedVariableAddedEvent(object o)
        {
            SearchIndex();
            LoadingDropDowns = true;
            FillComboboxes();
            LoadingDropDowns = false;
            //RefreshResults();
            DoCombinedFrequency();
        }

        void applicationViewModel_ApplyDataFilterEvent(object o)
        {
            if (applicationViewModel.RemoveIndicator
            &&
               IsDFUsedInThisGadget())
            {
                ResetGadget();
            }
            else
            {
                DoCombinedFrequency();
            }

        }
        /// <summary>
        /// Saves the Values of Columns.
        /// </summary>
        private void SaveColumnValues()
        {
            Col1 = (EwavColumn)cbxField.SelectedItem;
        }


        private void ResetGadget()
        {

            SearchIndex();
            if (IsDFUsedInThisGadget())
            {
                Index1 = -1;
                waitCursor.Visibility = System.Windows.Visibility.Collapsed;
                panelMain.Visibility = Visibility.Collapsed;
                pnlStatus.Visibility = System.Windows.Visibility.Collapsed;
            }
            LoadingDropDowns = true;
            FillComboboxes();
            LoadingDropDowns = false;
        }


        private void SearchIndex()
        {
            ClientCommon.Common CommonClass = new ClientCommon.Common();

            Index1 = CommonClass.SearchCurrentIndex(Col1, CommonClass.GetItemsSource(GetFieldPrimaryDataType));


        }


        void frequencyViewModel_ErrorNotice(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            string errorMessage = "";
            if (e.Data == null)
            {
                errorMessage = SharedStrings.NO_RECORDS_SELECTED;
            }
            else if (e.Data.Message.Length > 0)
            {
                //ChildWindow window = new ErrorWindow(e.Data);
                //window.Show();
                //return;
                errorMessage = e.Data.Message;
            }
            RenderFinishWithError(errorMessage);
            this.SetGadgetToFinishedState();
        }

        List<Grid> strataGridList;
        List<TextBlock> gridLabelsList;

        public void Construct()
        {
            strataGridList = new List<Grid>();
            gridLabelsList = new List<TextBlock>();
            cmnClass = new ClientCommon.Common();

        }

        void cfViewModel_FrequencyTableLoadedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            RefreshResults();
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.CloseGadgetOnClick();
        }

        /// <summary>
        /// Handles the click event for the Run button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            DoCombinedFrequency();
        }

        private void DoCombinedFrequency()
        {
            if (cbxField.SelectedIndex < 1)
            {
                return;
            }

            if (!LoadingDropDowns && !LoadingCanvas)
            {
                IsUserDefindVariableInUse();
                CreateInputVariableList();
                combinedFreqVM = (CombinedFrequencyViewModel)this.DataContext;

                if (cbxField.SelectedItem != null)
                {
                    this.waitCursor.Visibility = System.Windows.Visibility.Visible;
                    combinedFreqVM.GetCombinedFrequency(combinedParameters, ((EwavColumn)cbxField.SelectedItem).Name, GadgetOptions);
                }

                //waitCursor.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private bool IsDFUsedInThisGadget()
        {
            return Col1 != null && Col1.Name == applicationViewModel.ItemToBeRemoved.Name;
        }
        /// <summary>
        /// Determines whether is user defind variable in use].
        /// </summary>
        private void IsUserDefindVariableInUse()
        {
            Col1 = (cbxField.SelectedIndex > -1) ? (EwavColumn)cbxField.SelectedItem : null;
            if (Col1 != null && Col1.IsUserDefined == true)
            {
                Col1.IsInUse = true;
                //DFInUse = Col1;
            }
        }

        public List<string> GetVariablesInGroup(string groupVar)
        {
            EwavRule_Base rule = null;
            if (ApplicationViewModel.Instance.EwavDefinedVariables.Any(r => r.VaraiableName == groupVar))
            {
                rule = ApplicationViewModel.Instance.EwavDefinedVariables.Single(r => r.VaraiableName == groupVar);
            }

            List<string> fieldsList = new List<string>();
            if (rule != null && rule is EwavRule_GroupVariable)
            {
                EwavRule_GroupVariable gRule = (EwavRule_GroupVariable)rule;
                foreach (var item in gRule.Items)
                {
                    fieldsList.Add(item.VarName);
                }
            }
            return fieldsList;
        }

        public void DoWork()
        {
            Dictionary<string, string> inputVariableList = GadgetOptions.InputVariableList;
            this.ClearResults();
            System.Collections.Generic.Dictionary<string, DatatableBag> Freq_ListSet = new Dictionary<string, DatatableBag>();

            string freqVar = GadgetOptions.MainVariableName;
            string weightVar = GadgetOptions.WeightVariableName;
            string strataVar = string.Empty;

            if (GadgetOptions.StrataVariableNames != null && GadgetOptions.StrataVariableList.Count > 0)
            {
                strataVar = GadgetOptions.StrataVariableList[0].VarName;
            }

            try
            {
                List<string> stratas = new List<string>();
                if (!string.IsNullOrEmpty(strataVar))
                {
                    stratas.Add(strataVar);
                }

                DatatableBag dt = combinedFreqVM.FrequencyTable;
                bool booleanResults = true;
                int fields = -1;

                fields = GetVariablesInGroup(freqVar).Count;

                if (dt != null && dt.RecordList.Count > 0)
                {
                    string formatString = string.Empty;
                    double count = 0.0;

                    for (int i = 0; i < dt.RecordList.Count; i++)
                    {
                        count += Convert.ToDouble(dt.RecordList[i].Fields[1].VarName);
                    }


                    AddFreqGrid(strataVar, string.Empty);
                    string strataValue = dt.TableName;
                    RenderFrequencyHeader(strataValue, freqVar);
                    //double AccumulatedTotal = 0;
                    int rowCount = 1;
                    int denominator = 0;
                    //denominator = Convert.ToInt16(ApplicationViewModel.Instance.EwavSelectedDatasource.FilteredRecords);

                    foreach (var extraInfo in dt.ExtraInfo)
                    {
                        switch (extraInfo.Key.VarName.ToLower())
                        {
                            case "booleanresults":
                                booleanResults = Convert.ToBoolean(extraInfo.Value.VarName);
                                break;
                            case "fields":
                                fields = Convert.ToInt16(extraInfo.Value.VarName);
                                break;
                            case "denominator":
                                denominator = Convert.ToInt16(extraInfo.Value.VarName);
                                break;
                            default:
                                break;
                        }

                    }

                    if (!booleanResults)
                    {
                        denominator = denominator * fields;
                    }

                    for (int i = 0; i < dt.RecordList.Count; i++)
                    {

                        AddGridRow(strataValue, 26);
                        string displayValue = dt.RecordList[i].Fields[0].VarName.ToString();// row["value"].ToString();
                        SetGridText(strataValue, new TextBlockConfig(displayValue, new Thickness(6, 0, 6, 0), VerticalAlignment.Center, HorizontalAlignment.Left, TextAlignment.Left, rowCount, 0, Visibility.Visible));

                        double countValue = Convert.ToDouble(dt.RecordList[i].Fields[1].VarName.ToString());
                        SetGridText(strataValue, new TextBlockConfig(countValue.ToString(), new Thickness(6, 0, 6, 0), VerticalAlignment.Center, HorizontalAlignment.Right, TextAlignment.Right, rowCount, 1, Visibility.Visible));

                        double pct = 0;
                        if (count > 0)
                        {
                            pct = countValue / (double)denominator;
                        }
                        SetGridText(strataValue, new TextBlockConfig(pct.ToString("P"), new Thickness(6, 0, 6, 0), VerticalAlignment.Center, HorizontalAlignment.Right, TextAlignment.Right, rowCount, 2, Visibility.Visible));
                        rowCount++;
                    }

                    RenderFrequencyFooter(strataValue, denominator, fields, booleanResults);
                    this.DrawFrequencyBorders(strataValue);
                    RenderFinish();
                }
            }
            catch (Exception ex)
            {
                RenderFinishWithError(ex.Message);
                SetGadgetToFinishedState();
            }
        }

        /// <summary>
        /// Clears the gadget's outputs
        /// </summary>
        private void ClearResults()
        {
            this.txtStatus.Text = string.Empty;
            this.pnlStatus.Visibility = Visibility.Collapsed;
            this.waitCursor.Visibility = Visibility.Visible;

            foreach (Grid grid in this.strataGridList)
            {
                grid.Children.Clear();
                grid.RowDefinitions.Clear();
                this.panelMain.Children.Remove(grid);
            }

            this.panelMain.Children.Clear();

            this.strataGridList.Clear();
        }

        private void FillComboboxes(bool update = false)
        {
            LoadingDropDowns = true;
            List<EwavColumn> SourceColumns = ApplicationViewModel.Instance.EwavSelectedDatasource.AllColumns;

            IEnumerable<EwavColumn> CBXFieldCols = from cols in SourceColumns
                                                   where GetFieldPrimaryDataType.Contains(cols.SqlDataTypeAsString)
                                                   orderby cols.Name
                                                   select cols;

            List<EwavColumn> colsList = CBXFieldCols.ToList();

            colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            this.cbxField.ItemsSource = colsList;
            this.cbxField.SelectedValue = "Index";
            this.cbxField.DisplayMemberPath = "Name";
            cbxField.SelectedIndex = Index1;

            LoadingDropDowns = false;
        }

        private void AddFreqGrid(string strataVar, string value)
        {
            Grid grid = new Grid();
            grid.Tag = value;
            //grid.Style = this.Resources["genericOutputGrid"] as Style;
            grid.Visibility = System.Windows.Visibility.Collapsed;

            ColumnDefinition column1 = new ColumnDefinition();
            ColumnDefinition column2 = new ColumnDefinition();
            ColumnDefinition column3 = new ColumnDefinition();

            column1.Width = GridLength.Auto;
            column2.Width = GridLength.Auto;
            column3.Width = GridLength.Auto;

            grid.ColumnDefinitions.Add(column1);
            grid.ColumnDefinitions.Add(column2);
            grid.ColumnDefinitions.Add(column3);

            TextBlock txtGridLabel = new TextBlock();
            txtGridLabel.Text = value;
            txtGridLabel.HorizontalAlignment = HorizontalAlignment.Left;
            txtGridLabel.VerticalAlignment = VerticalAlignment.Bottom;
            txtGridLabel.Margin = new Thickness(2, 42, 2, 2);
            txtGridLabel.FontWeight = FontWeights.Bold;
            if (string.IsNullOrEmpty(strataVar))
            {
                txtGridLabel.Margin = new Thickness(2, 36, 2, 2);
                txtGridLabel.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                txtGridLabel.Margin = new Thickness(2, 42, 2, 2);
            }
            gridLabelsList.Add(txtGridLabel);
            panelMain.Children.Add(txtGridLabel);

            Border border = new Border();
            //border.Style = Application.Current.Resources["borderStyle1"] as Style;

            border.Child = grid;
            panelMain.Children.Add(border);
            strataGridList.Add(grid);
        }

        protected virtual Grid GetStrataGrid(string strataValue)
        {
            Grid grid = new Grid();

            foreach (Grid g in strataGridList)
            {
                if (g.Tag.Equals(strataValue))
                {
                    grid = g;
                    break;
                }
            }

            return grid;
        }

        private void SetGridText(string strataValue, TextBlockConfig textBlockConfig)
        {
            Grid grid = new Grid();

            grid = GetStrataGrid(strataValue);

            TextBlock txt = new TextBlock();
            txt.Text = textBlockConfig.Text;
            txt.Margin = textBlockConfig.Margin;
            txt.VerticalAlignment = textBlockConfig.VerticalAlignment;
            txt.HorizontalAlignment = textBlockConfig.HorizontalAlignment;
            Grid.SetRow(txt, textBlockConfig.RowNumber);
            Grid.SetColumn(txt, textBlockConfig.ColumnNumber);
            grid.Children.Add(txt);
        }

        public void AddGridRow(string strataValue, int height)
        {
            Grid grid = GetStrataGrid(strataValue);
            //waitPanel.Visibility = System.Windows.Visibility.Collapsed;
            //waitCursor.Visibility = Visibility.Collapsed;
            grid.Visibility = Visibility.Visible;
            RowDefinition rowDef = new RowDefinition();
            rowDef.Height = new GridLength(height);
            grid.RowDefinitions.Add(rowDef);
        }


        private void RenderFrequencyHeader(string strataValue, string freqVar)
        {
            Grid grid = GetStrataGrid(strataValue);

            RowDefinition rowDefHeader = new RowDefinition();
            rowDefHeader.Height = new GridLength(30);
            grid.RowDefinitions.Add(rowDefHeader);

            for (int y = 0; y < grid.ColumnDefinitions.Count; y++)
            {
                Rectangle rctHeader = new Rectangle();
                rctHeader.Style = Application.Current.Resources["HeaderCell"] as Style;
                Grid.SetRow(rctHeader, 0);
                Grid.SetColumn(rctHeader, y);
                grid.Children.Add(rctHeader);
            }

            TextBlock txtValHeader = new TextBlock();
            txtValHeader.Text = "Value";
            txtValHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtValHeader, 0);
            Grid.SetColumn(txtValHeader, 0);
            grid.Children.Add(txtValHeader);

            TextBlock txtFreqHeader = new TextBlock();
            txtFreqHeader.Text = "Frequency";// DashboardSharedStrings.COL_HEADER_FREQUENCY;
            txtFreqHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtFreqHeader, 0);
            Grid.SetColumn(txtFreqHeader, 1);
            grid.Children.Add(txtFreqHeader);

            TextBlock txtPctHeader = new TextBlock();
            txtPctHeader.Text = "Percent";// DashboardSharedStrings.COL_HEADER_PERCENT;
            txtPctHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtPctHeader, 0);
            Grid.SetColumn(txtPctHeader, 2);
            grid.Children.Add(txtPctHeader);
        }

        /// <summary>
        /// Draws the borders around a given output grid
        /// </summary>
        /// <param name="strataValue">
        /// The strata value to which this grid cell belongs; used to search the list of grids and 
        /// return the proper System.Windows.Controls.Grid for text insertion.
        /// </param>
        private void DrawFrequencyBorders(string strataValue)
        {
            Grid grid = this.GetStrataGrid(strataValue);

            //this.waitCursor.Visibility = Visibility.Collapsed;
            int rdcount = 0;
            Brush brush = new SolidColorBrush(Colors.DarkGray);

            foreach (RowDefinition rd in grid.RowDefinitions)
            {
                if (rdcount > 0)
                {
                    brush = new SolidColorBrush(Colors.DarkGray);
                }

                int cdcount = 0;
                foreach (ColumnDefinition cd in grid.ColumnDefinitions)
                {
                    Rectangle rctBorder = new Rectangle();
                    rctBorder.Style = Application.Current.Resources["DataCell"] as Style;
                    Grid.SetRow(rctBorder, rdcount);
                    Grid.SetColumn(rctBorder, cdcount);
                    grid.Children.Add(rctBorder);
                    cdcount++;
                }
                rdcount++;
            }
        }

        private void RenderFrequencyFooter(string strataValue, int denominator, int fields = -1, bool isBoolean = false)
        {
            Grid grid = GetStrataGrid(strataValue);
            if (checkboxShowDenominator.IsChecked == false)
            {
                return;
            }

            txtDenom = new TextBlock();
            txtDenom.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            txtDenom.FontWeight = FontWeights.Bold;
            txtDenom.Margin = new Thickness(0, 15, 0, 15);
            //txtDenom.Margin = (Thickness)this.Resources["genericTextMargin"];

            if (isBoolean == true)
            {
                txtDenom.Text = string.Format("Fields are boolean. \nDenominator = {0}", denominator);// TBD DashboardSharedStrings.DENOMINATOR_DESCRIPTION_BOOLEAN, denominator);
            }
            else
            {
                txtDenom.Text = string.Format("Fields are not boolean. \nDenominator = {0}", denominator);//DashboardSharedStrings.DENOMINATOR_DESCRIPTION_NON_BOOLEAN, denominator);
            }

            panelMain.Children.Add(txtDenom);
        }

        private void RenderFinish()
        {
            waitCursor.Visibility = System.Windows.Visibility.Collapsed;

            foreach (Grid freqGrid in strataGridList)
            {
                freqGrid.Visibility = Visibility.Visible;
            }
            pnlStatus.Visibility = System.Windows.Visibility.Collapsed;
            panelMain.Visibility = System.Windows.Visibility.Visible;
            gadgetExpander.IsExpanded = false;
        }

        private void RenderFinishWithWarning(string errorMessage)
        {
            waitCursor.Visibility = System.Windows.Visibility.Collapsed;

            foreach (Grid freqGrid in strataGridList)
            {
                freqGrid.Visibility = Visibility.Visible;
            }
        }

        private void RenderFinishWithError(string errorMessage)
        {
            waitCursor.Visibility = System.Windows.Visibility.Collapsed;

            //this.waitCursor.Visibility = Visibility.Collapsed;

            this.pnlStatus.Background = new SolidColorBrush(Color.FromArgb(255, 248, 215, 226)); //Brushes.Tomato;
            this.pnlStatusTop.Background = new SolidColorBrush(Color.FromArgb(255, 228, 101, 142)); //Brushes.Red;
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            this.pnlStatus.Visibility = System.Windows.Visibility.Visible;
            this.txtStatus.Text = errorMessage;
            this.panelMain.Visibility = System.Windows.Visibility.Collapsed;

        }

        private void CreateInputVariableList()
        {
            Dictionary<string, string> inputVariableList = new Dictionary<string, string>();
            this.combinedParameters = new EwavCombinedFrequencyGadgetParameters();
            this.gadgetOptions = new GadgetParameters();
            this.gadgetOptions.GadgetFilters = GadgetFilters;
            this.gadgetOptions.MainVariableName = string.Empty;
            this.gadgetOptions.WeightVariableName = string.Empty;
            this.gadgetOptions.StrataVariableNames = new List<string>();
            this.gadgetOptions.CrosstabVariableName = string.Empty;
            this.gadgetOptions.ColumnNames = new List<MyString>();
            this.gadgetOptions.StrataVariableList = new List<MyString>();

            this.gadgetOptions.TableName = ApplicationViewModel.Instance.EwavSelectedDatasource.TableName;
            inputVariableList.Add("tableName", this.gadgetOptions.TableName);

            this.gadgetOptions.DatasourceName = ApplicationViewModel.Instance.EwavSelectedDatasource.DatasourceName;

            if (cbxField.SelectedIndex > -1 && cbxField.SelectedItem != null)
            {
                GadgetOptions.MainVariableName = ((EwavColumn)cbxField.SelectedItem).Name.ToString();
            }
            else
            {
                return;
            }


            if (checkboxSortHighLow.IsChecked == true)
            {
                combinedParameters.SortHighToLow = true;
            }
            else
            {
                combinedParameters.SortHighToLow = false;
            }

            if (checkboxShowDenominator.IsChecked == true)
            {
                combinedParameters.ShowDenominator = true;
            }
            else
            {
                combinedParameters.ShowDenominator = false;
            }

            CombineModeTypes combineMode = (CombineModeTypes)(Enum.Parse(typeof(CombineModeTypes),
                ((ComboBoxItem)cmbCombineMode.SelectedItem).Content.ToString(), true));

            if (combineMode == CombineModeTypes.Boolean)
            {
                combinedParameters.TrueValue = txtTrueValue.Text;
            }

            combinedParameters.CombineMode = combineMode;

            GadgetOptions.ShouldIncludeMissing = false;


            if (cmbCombineMode.SelectedIndex >= 0)
            {
                inputVariableList.Add("combinemode", cmbCombineMode.SelectedIndex.ToString());
            }

            inputVariableList.Add("truevalue", txtTrueValue.Text);



            GadgetOptions.InputVariableList = inputVariableList;
        }

        private void ResizeButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (spContent.Visibility == System.Windows.Visibility.Visible)
            {
                spContent.Visibility = System.Windows.Visibility.Collapsed;
                ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn2"];
            }
            else
            {
                spContent.Visibility = System.Windows.Visibility.Visible;
                ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            }
        }

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //gadgetContextMenu.Hide();
        }

        private void EditProperties_Click(object sender, RoutedEventArgs e)
        {
        }

        private void LayoutRoot_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetSafePosition(this.cmnClass.GetParentObject<Grid>(this, "LayoutRoot"));
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).GadgetNameOnRightClick = this.MyControlName; //"FrequencyControl";
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).StrataList = this.strataGridList;
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).SelectedGadget = this;
            cmnClass.UpdateZOrder(this, true, cmnClass.GetParentObject<Grid>(this, "LayoutRoot"));
        }

        public StringBuilder HtmlBuilder { get; set; }

        /// <summary>
        /// Forces a refresh
        /// </summary>
        public void RefreshResults()
        {
            Dictionary<string, string> inputVariableList = new Dictionary<string, string>();

            if (cbxField.SelectedIndex > 0)
            {
                DoWork();
                //RefreshResults();
            }
        }

        /// <summary>
        /// Generates Xml representation of this gadget
        /// </summary>
        /// <param name="doc">The Xml docment</param>
        /// <returns>XmlNode</returns>
        XNode IGadget.Serialize(XDocument doc)
        {
            CreateInputVariableList();
            XElement element;
            Dictionary<string, string> inputVariableList = GadgetOptions.InputVariableList;

            string freqVar = GadgetOptions.MainVariableName;
            string strataVar = string.Empty;
            string crosstabVar = GadgetOptions.CrosstabVariableName;
            bool sortHighToLow = true;
            bool showDenominator = true;
            if (checkboxShowDenominator.IsChecked == false)
            {
                showDenominator = false;
            }

            if (checkboxSortHighLow.IsChecked == false)
            {
                sortHighToLow = false;
            }

            element = new XElement("gadget",
                new XAttribute("top", Canvas.GetTop(this).ToString("F0")),
                new XAttribute("left", Canvas.GetLeft(this).ToString("F0")),
                new XAttribute("collapsed", "false"),
                new XAttribute("gadgetType", "Ewav.CombinedFrequency"),
            new XElement("mainVariable", freqVar),
            new XElement("strataVariable", strataVar),
            new XElement("sort", sortHighToLow),
            new XElement("combinedmode", ((ComboBoxItem)cmbCombineMode.SelectedItem).Content.ToString()),
            new XElement("truevalue", txtTrueValue.Text),
            new XElement("showdenominator", showDenominator),
            new XElement("gadgetTitle", GName.Text),
            new XElement("gadgetDescription", txtGadgetDescription.Text)
);
            if (this.GadgetFilters != null)
            {
                this.GadgetFilters.Serialize(element);
            }
            

            return element;
        }

        /// <summary>
        /// Creates the frequency gadget from an Xml element
        /// </summary>
        /// <param name="element">The element from which to create the gadget</param>
        public void CreateFromXml(XElement element)
        {
            LoadingDropDowns = true;
            LoadingCanvas = true;
            ClientCommon.Common cmnClass = new ClientCommon.Common();
            this.GadgetFilters = new List<EwavDataFilterCondition>();
            //List<EwavDataFilterCondition> filterList = new List<EwavDataFilterCondition>();
            foreach (XElement child in element.Descendants())
            {
                switch (child.Name.ToString().ToLower())
                {
                    case "mainvariable":
                        cbxField.SelectedIndex = cmnClass.FindIndexToSelect(cmnClass.GetItemsSource(GetFieldPrimaryDataType, true), child.Value.ToString().Replace("&lt;", "<"));
                        break;
                    case "sort":
                        if (child.Value.ToString().ToLower().Equals("highlow"))
                        {
                            checkboxSortHighLow.IsChecked = true;
                        }
                        break;
                    case "combinedmode":
                        switch (child.Value.ToString().ToLower())
                        {
                            case "boolean":
                            case "bool":
                                cmbCombineMode.SelectedIndex = 1;
                                break;
                            case "categorical":
                                cmbCombineMode.SelectedIndex = 2;
                                break;
                            case "automatic":
                            case "auto":
                            default:
                                cmbCombineMode.SelectedIndex = 0;
                                break;
                        }
                        break;
                    case "truevalue":
                        txtTrueValue.Text = child.Value.ToString();
                        break;
                    case "showdenominator":
                        checkboxShowDenominator.IsChecked = Convert.ToBoolean(child.Value.ToString());
                        break;


                    case "gadgettitle":

                        GName.Text = child.Value.ToString();

                        break;
                    case "gadgetdescription":

                        txtGadgetDescription.Text = child.Value.ToString();


                        break;

                    case "ewavdatafiltercondition":
                         EwavDataFilterCondition condition = new EwavDataFilterCondition();
                        condition.FieldName = new MyString();
                        condition.FieldName.VarName = child.Attribute("fieldName").Value.ToString();

                        condition.FriendlyOperand = new MyString();
                        condition.FriendlyOperand.VarName = child.Attribute("friendlyOperand").Value.ToString();

                        if (child.Attribute("friendlyValue").Value.ToString() != "null")
                        {
                            condition.FriendlyValue = new MyString();
                            condition.FriendlyValue.VarName = child.Attribute("friendlyValue").Value.ToString();
                        }

                        if (child.Attribute("friendLowValue").Value.ToString() != "null")
                        {
                            condition.FriendLowValue = new MyString();
                            condition.FriendLowValue.VarName = child.Attribute("friendLowValue").Value.ToString();
                        }

                        if (child.Attribute("friendHighValue").Value.ToString() != "null")
                        {
                            condition.FriendHighValue = new MyString();
                            condition.FriendHighValue.VarName = child.Attribute("friendHighValue").Value.ToString();
                        }

                        condition.JoinType = new MyString();
                        condition.JoinType.VarName = child.Attribute("joinType").Value.ToString();

                        this.GadgetFilters.Add(condition);
                        break;



                }
                
                    
                

            }


            LoadingDropDowns = false;
            LoadingCanvas = false;

            double mouseVerticalPosition = 0.0, mouseHorizontalPosition = 0.0;

            foreach (XAttribute attribute in element.Attributes())
            {
                switch (attribute.Name.ToString().ToLower())
                {
                    case "top":
                        mouseVerticalPosition = double.Parse(element.Attribute("top").Value.ToString(),
                            new CultureInfo(applicationViewModel.LoadedCanvasDto.Culture));

                        break;
                    case "left":
                        mouseHorizontalPosition = double.Parse(element.Attribute("left").Value.ToString(),
                             new CultureInfo(applicationViewModel.LoadedCanvasDto.Culture));
                        break;


                }
            }

            DoCombinedFrequency();
            cmnClass.AddControlToCanvas(this, mouseVerticalPosition, mouseHorizontalPosition, applicationViewModel.LayoutRoot);
        }

        /// <summary>
        /// Converts the gadget's output to Html
        /// </summary>
        /// <returns></returns>
        public string ToHTML(bool ForDash = false, string htmlFileName = "", int count = 0)
        {
            StringBuilder htmlBuilder = new StringBuilder();

            htmlBuilder.AppendLine("<html><head>");
            htmlBuilder.AppendLine("<meta http-equiv=\"content-type\" content=\"text/html;charset=UTF-8\" />");
            htmlBuilder.AppendLine("<meta name=\"author\" content=\"Epi Info 7\" />");
            htmlBuilder.AppendLine(this.cmnClass.GenerateStandardHTMLStyle());

            if (string.IsNullOrEmpty(this.CustomOutputHeading))
            {
                htmlBuilder.AppendLine("<h2 class=\"gadgetHeading\">Combined Frequency</h2>");
            }
            else if (this.CustomOutputHeading != "(none)")
            {
                htmlBuilder.AppendLine(string.Format("<h2 class=\"gadgetHeading\">{0}</h2>", this.CustomOutputHeading));
            }

            if (!string.IsNullOrEmpty(this.CustomOutputDescription))
            {
                htmlBuilder.AppendLine(string.Format("<p class=\"gadgetsummary\">{0}</p>", this.CustomOutputDescription));
            }

            if (!string.IsNullOrEmpty(this.txtStatus.Text) && this.pnlStatus.Visibility == Visibility.Visible)
            {
                htmlBuilder.AppendLine(string.Format("<p><small><strong>{0}</strong></small></p>", this.txtStatus.Text));
            }

            foreach (Grid grid in this.strataGridList)
            {
                string gridName = grid.Tag.ToString();

                string summaryText = string.Format("This tables represents the frequency of the field {0}. ", ((EwavColumn)this.cbxField.SelectedItem).Name);

                summaryText += "Each non-heading row in the table represents one of the distinct frequency values. The last row contains the total.";

                htmlBuilder.AppendLine("<div style=\"height: 7px;\"></div>");
                htmlBuilder.AppendLine(string.Format("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" summary=\"{0}\">", summaryText));

                if (string.IsNullOrEmpty(this.CustomOutputCaption) == false)
                {
                    htmlBuilder.AppendLine(string.Format("<caption>{0}</caption>", this.CustomOutputCaption));
                }

                double barWidth = 0.0;

                foreach (UIElement control in grid.Children)
                {
                    if (control is TextBlock)
                    {
                        int rowNumber = Grid.GetRow((FrameworkElement)control);
                        int columnNumber = Grid.GetColumn((FrameworkElement)control);

                        string tableDataTagOpen = "<td>";
                        string tableDataTagClose = "</td>";

                        if (rowNumber == 0)
                        {
                            tableDataTagOpen = "<th>";
                            tableDataTagClose = "</th>";
                        }

                        if (columnNumber == 0)
                        {
                            if (((double)rowNumber) % 2.0 == 1)
                            {
                                htmlBuilder.AppendLine("<tr class=\"altcolor\">");
                            }
                            else
                            {
                                htmlBuilder.AppendLine("<tr>");
                            }
                        }
                        if (columnNumber == 0 && rowNumber > 0)
                        {
                            tableDataTagOpen = "<td class=\"value\">";
                        }

                        string value = ((TextBlock)control).Text;

                        if (string.IsNullOrEmpty(value))
                        {
                            value = "&nbsp;";
                        }

                        htmlBuilder.AppendLine(string.Format("{0}{1}{2}", tableDataTagOpen, value, tableDataTagClose));

                        if (columnNumber == 2 && rowNumber > 0)
                        {
                            barWidth = 0;
                            double.TryParse(value.Trim().TrimEnd('%').Trim(), out barWidth);
                        }

                        if (columnNumber >= grid.ColumnDefinitions.Count - 1)
                        {
                            htmlBuilder.AppendLine("</tr>");
                        }
                    }
                }

                htmlBuilder.AppendLine("</table>");
            }

            htmlBuilder.AppendLine("<p>" + txtDenom.Text + "</p>");

            if (ForDash == false)
            {
                System.Windows.Browser.HtmlPage.Window.Invoke("DisplayFormattedText", htmlBuilder.ToString());
            }

            HtmlBuilder = htmlBuilder;

            return "";
        }


        public bool IsProcessing
        {
            get;
            set;
        }

        public void SetGadgetToProcessingState()
        {
            this.IsProcessing = true;
            this.checkboxSortHighLow.IsEnabled = false;

        }

        public void SetGadgetToFinishedState()
        {
            this.IsProcessing = false;
            this.cbxField.IsEnabled = true;
            this.checkboxSortHighLow.IsEnabled = true;

            //waitCursor.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void UpdateVariableNames()
        {
            //throw new NotImplementedException();
        }

        public string CustomOutputHeading
        {
            get;
            set;
        }

        public string CustomOutputDescription
        {
            get;
            set;
        }

        public string CustomOutputCaption
        {
            get;
            set;
        }

        void IGadget.CloseGadget()
        {
            this.CloseGadget();
        }
        /// <summary>
        /// Closes the gadget on click;
        /// </summary>
        void IGadget.CloseGadgetOnClick()
        {
            this.CloseGadgetOnClick();
        }
        public string MyControlName
        {
            get { return "CombinedFrequency"; }
        }

        public string MyUIName
        {
            get { return "Combined Frequency"; }
        }

        public void Reload()
        {
            DoCombinedFrequency();
        }

        void CloseGadgetOnClick()
        {
            CloseGadget confirm = new CloseGadget(this);
            confirm.Show();
        }

        void CloseGadget()
        {
            applicationViewModel.CloseGadget(this);

        }

        private void cbxField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxField.SelectedIndex < 1)
            {
                btnRun.IsEnabled = false;
            }
            else
            {
                btnRun.IsEnabled = true;
            }
        }

        private void cmbCombineMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (((ComboBox)sender).SelectedIndex > -1)
            {
                CombineModeTypes combineMode = (CombineModeTypes)(Enum.Parse(typeof(CombineModeTypes),
                ((ComboBoxItem)((ComboBox)sender).SelectedItem).Content.ToString(), true));

                if (combineMode == CombineModeTypes.Boolean)
                {
                    txtTrueValue.Visibility = System.Windows.Visibility.Visible;
                    tblockTrueValue.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    tblockTrueValue.Visibility = System.Windows.Visibility.Collapsed;
                    txtTrueValue.Visibility = System.Windows.Visibility.Collapsed;
                    txtTrueValue.Text = "";
                }
            }


        }

        private void checkboxShowDenominator_Checked(object sender, RoutedEventArgs e)
        {
            if (txtDenom != null)
            {
                txtDenom.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void checkboxShowDenominator_Unchecked(object sender, RoutedEventArgs e)
        {
            if (txtDenom != null)
            {
                txtDenom.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            GadgetFilterControl gadgetFiltersWindow = null;

            if (GadgetFilters != null)
            {
                gadgetFiltersWindow = new GadgetFilterControl(GadgetFilters);
            }
            else
            {
                gadgetFiltersWindow = new GadgetFilterControl();
            }
             
            gadgetFiltersWindow.Show();
            gadgetFiltersWindow.Closed += new EventHandler(window_Closed);
        }

        void window_Closed(object sender, EventArgs e)
        {
            GadgetFilterControl GadgetFilter = ((GadgetFilterControl)sender);
            if (GadgetFilter.DialogResult == true)
            {
                GadgetFilters = GadgetFilter.GadgetFilters;
                DoCombinedFrequency();
            }
        }

    }
}

namespace Ewav.Web.Services.CombinedFrequencyDomainService
{
    public partial class CombinedFrequencyDomainContext
    {
        //This is the place to set RIA Services query timeout. 
        //TimeSpan(0, 5, 0) means five minutes vs the 
        //default 60 sec
        partial void OnCreated()
        {
            if (!DesignerProperties.GetIsInDesignMode(Application.
                Current.RootVisual))
                ((WebDomainClient<Ewav.Web.Services.CombinedFrequencyDomainService.CombinedFrequencyDomainContext.ICombinedFrequencyDomainServiceContract>)
                    DomainClient).ChannelFactory.Endpoint.Binding.SendTimeout =
                    new TimeSpan(0, 120, 0);
        }
    }
}

