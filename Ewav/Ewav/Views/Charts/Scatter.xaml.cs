/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       Scatter.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using ComponentArt.Silverlight.DataVisualization.Charting;
using ComponentArt.Silverlight.DataVisualization.Common;
using ComponentArt.Silverlight.Export.PDF;
using Ewav.Web.EpiDashboard;
using Ewav.BAL;
using Ewav.Client.Application;
using Ewav.ViewModels;
using Ewav.Web.Services;
using Ewav.ExtensionMethods;
using CommonLibrary;
using ComponentArt.Silverlight.DataVisualization;

namespace Ewav
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(UserControl))]
    [ExportMetadata("type", "chart")]
    [ExportMetadata("tabindex", "12")]
    public partial class Scatter : UserControl, IGadget, IEwavGadget, IChartControl
    {
        private readonly char[] SplitTokens = " \t;".ToCharArray();
        private ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;
        private TextBlock textBlockX = new TextBlock();
        private TextBlock textBlockY = new TextBlock();

        private bool isBooleanWithNoStratas = false;
        private GadgetParameters gadgetOptions;
        private object syncLock = new object();
        private object objXAxisStart;
        private object objXAxisEnd;
        private int Index1 = -1, Index2 = -1;
        EwavColumn Col1, Col2;
        DatatableBag databag;
        DashboardPanel dp = new DashboardPanel();
        SetLabels setLabels;
        SetLabelsViewModel viewModel = null;
        ClientCommon.Common CommonClass = new ClientCommon.Common();
        string xAxisVarName, yAxisVarName;
        //EpiCurveViewModel eCrvViewModel;
        ScatterViewModel sctrViewModel;
        List<string> dateColumnNames = new List<string>();
        List<string> numericColumnNames = new List<string>();
        List<List<StringDataValue>> dataValues = new List<List<StringDataValue>>();


        XYChart chart = null;
        MarkerSeries mseries = null;
        LineSeries regression = null;

        public event GadgetClosingHandler GadgetClosing;
        public event GadgetProcessingFinishedHandler GadgetProcessingFinished;
        public event GadgetStatusUpdateHandler GadgetStatusUpdate;
        public event GadgetCheckForCancellationHandler GadgetCheckForCancellation;

        private delegate void SetStatusDelegate(string statusMessage);
        private delegate void RequestUpdateStatusDelegate(string statusMessage);
        private delegate bool CheckForCancellationDelegate();
        private delegate void RenderFinishWithErrorDelegate(string errorMessage);
        private delegate void RenderFinishWithWarningDelegate(string errorMessage);
        // private delegate void RenderFinishEpiCurveDelegate(DataTable data, List<List<StringDataValue>> dataValues);
        private delegate void RenderFinishSingleChartDelegate(List<List<StringDataValue>> stratifiedValues);
        //private delegate void RenderFinishScatterChartDelegate(List<NumericDataValue> dataValues, StatisticsRepository.LinearRegression.LinearRegressionResults results, NumericDataValue maxValue, NumericDataValue minValue);
        //private delegate void RenderFinishStackedChartDelegate(List<List<StringDataValue>> dataValues, DataTable data);
        private delegate void SimpleCallback();

        private long recordCount;

        public long RecordCount
        {
            get { return recordCount; }
            set { recordCount = value; }
        }

        public List<EwavDataFilterCondition> GadgetFilters { get; set; }

        public class StringDataValue
        {
            public double DependentValue { get; set; }
            public string IndependentValue { get; set; }
            public string StratificationValue { get; set; }
            public double CurrentMeanValue { get; set; }
        }

        //public class NumericDataValue
        //{
        //    public decimal DependentValue { get; set; }
        //    public decimal IndependentValue { get; set; }
        //}

        public class TypeStringTuple
        {
            public Type Item1 { get; set; }
            public string Item2 { get; set; }
            public TypeStringTuple(Type item1, string item2)
            {
                Item1 = item1;
                Item2 = item2;
            }
        }

        ClientCommon.Common cmnClass = new ClientCommon.Common();

        public List<ColumnDataType> GetFieldNumericDataType
        {
            get
            {
                List<ColumnDataType> columnDataType = new List<ColumnDataType>();
                //columnDataType.Add(ColumnDataType.DateTime);
                columnDataType.Add(ColumnDataType.Numeric);

                return columnDataType;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Scatter()
        {
            InitializeComponent();

            gadgetOptions = new GadgetParameters();
            gadgetOptions.ShouldIncludeFullSummaryStatistics = false;
            gadgetOptions.ShouldIncludeMissing = false;
            gadgetOptions.ShouldSortHighToLow = false;
            gadgetOptions.ShouldUseAllPossibleValues = false;
            gadgetOptions.StrataVariableNames = new List<string>();

            this.Loaded += new RoutedEventHandler(Scatter_Loaded);

            FillDropDowns();
            //InitializeControl();
        }

        void Scatter_Loaded(object sender, RoutedEventArgs e)
        {


            InitializeControl();
            //this.gadgetExpander.IsExpanded = false;     

            //if (YAxisLabel != null)
            //    textBlockY.Text = YAxisLabel;
            //if (XAxisLabel != null)
            //    textBlockX.Text = XAxisLabel;
            //if (dp != null && ChartTitle != null)
            //    dp.Title = ChartTitle;

        }

        private void InitializeControl()
        {
            cbxScatterXAxisField.SelectionChanged += new SelectionChangedEventHandler(ConfigField_SelectionChanged);
            cbxScatterYAxisField.SelectionChanged += new SelectionChangedEventHandler(ConfigField_SelectionChanged);
            try
            {
                DatatableBag eCrvData = new DatatableBag();
                sctrViewModel = (ScatterViewModel)this.DataContext;
                //sctrViewModel.ColumnsLoadedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(sctrViewModel_ColumnsLoadedEvent);
                sctrViewModel.RegressTableLoadedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(sctrViewModel_RegressResultsLoadedEvent);
                applicationViewModel.ConnectionStringReadyEvent += new ConnectionStringReadyEventHandler(applicationViewModel_ConnectionStringLoadedEvent);
                //sctrViewModel.GetColumns("NEDS", "vwExternalData");
                applicationViewModel.ApplyDataFilterEvent += new ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);
                applicationViewModel.DefinedVariableAddedEvent += new DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);
                applicationViewModel.DefinedVariableInUseDeletedEvent += new DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableInUseDeletedEvent);
                applicationViewModel.DefinedVariableNotInUseDeletedEvent += new DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
                applicationViewModel.PreColumnChangedEvent += new PreColumnChangedEventHandler(applicationViewModel_PreColumnChangedEvent);
                applicationViewModel.UnloadedEvent += new UnloadedEventHandler(applicationViewModel_UnloadedEvent);
                sctrViewModel.ErrorNotice += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(sctrViewModel_ErrorNotice);
                EnableDisableButton();
            }
            catch (Exception)
            {
                throw;
            }

            this.IsProcessing = false;

            this.GadgetStatusUpdate += new GadgetStatusUpdateHandler(RequestUpdateStatusMessage);
            this.GadgetCheckForCancellation += new GadgetCheckForCancellationHandler(IsCancelled);
        }

        void applicationViewModel_UnloadedEvent(object o)
        {
            UnloadGadget();
        }

        private void UnloadGadget()
        {
            applicationViewModel.ConnectionStringReadyEvent -= new ConnectionStringReadyEventHandler(applicationViewModel_ConnectionStringLoadedEvent);
            //sctrViewModel.GetColumns("NEDS", "vwExternalData");
            applicationViewModel.ApplyDataFilterEvent -= new ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);
            applicationViewModel.DefinedVariableAddedEvent -= new DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);
            applicationViewModel.DefinedVariableInUseDeletedEvent -= new DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableInUseDeletedEvent);
            applicationViewModel.DefinedVariableNotInUseDeletedEvent -= new DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
            applicationViewModel.PreColumnChangedEvent -= new PreColumnChangedEventHandler(applicationViewModel_PreColumnChangedEvent);
        }

        void applicationViewModel_PreColumnChangedEvent(object o)
        {
            SaveColumnValues();
        }

        void applicationViewModel_DefinedVariableNotInUseDeletedEvent(object o)
        {
            SearchIndex();
            LoadingDropDowns = true;
            FillDropDowns();
            LoadingDropDowns = false;
            DoScatter();
        }

        void applicationViewModel_DefinedVariableInUseDeletedEvent(object o)
        {
            ResetGadget();
        }

        private void ResetGadget()
        {
            SearchIndex();
            if (Col1 != null && Col1.Name == applicationViewModel.ItemToBeRemoved.Name ||
                Col2 != null && Col2.Name == applicationViewModel.ItemToBeRemoved.Name)
            {
                Index1 = Index2 = -1;
                pnlChartContainer.Visibility = System.Windows.Visibility.Collapsed;
                pnlStatus.Visibility = System.Windows.Visibility.Collapsed;
                this.cbxScatterXAxisField.ItemsSource = null;
                this.cbxScatterYAxisField.ItemsSource = null;
            }
            LoadingDropDowns = true;
            FillDropDowns();
            LoadingDropDowns = false;
        }

        void applicationViewModel_DefinedVariableAddedEvent(object o)
        {
            SearchIndex();
            LoadingDropDowns = true;
            FillDropDowns();
            LoadingDropDowns = false;
            DoScatter();
        }

        private void SearchIndex()
        {
            Index1 = CommonClass.SearchCurrentIndex(Col1, CommonClass.GetItemsSource(GetFieldNumericDataType));
            Index2 = CommonClass.SearchCurrentIndex(Col2, CommonClass.GetItemsSource(GetFieldNumericDataType));
        }

        /// <summary>
        /// Saves the Values of Columns.
        /// </summary>
        private void SaveColumnValues()
        {
            Col1 = (EwavColumn)cbxScatterXAxisField.SelectedItem;
            Col2 = (EwavColumn)cbxScatterYAxisField.SelectedItem;
        }



        void sctrViewModel_ErrorNotice(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            if (e.Data.Message.Length > 0)
            {
                ChildWindow window = new ErrorWindow(e.Data);
                window.Show();
                //return;
            }
            RenderFinishWithError(e.Data.Message);
            this.SetGadgetToFinishedState();
        }

        private void DoScatter()
        {

            RefreshResults();
            bool byEpiWeek = false;

            if (gadgetOptions.InputVariableList == null)
            {
                return;
            }

            this.gadgetOptions.GadgetFilters = GadgetFilters;

            if (gadgetOptions.InputVariableList.ContainsKey("isdatecolumnnumeric"))
            {
                byEpiWeek = bool.Parse(gadgetOptions.InputVariableList["isdatecolumnnumeric"]);
            }
            sctrViewModel = (ScatterViewModel)this.DataContext;
            //eCrvViewModel.GetEpiCurve(gadgetOptions, byEpiWeek, gadgetOptions.MainVariableName, gadgetOptions.CrosstabVariableName);
            //eCrvViewModel.
            //epiCurveWorker_DoWork(dtb, gadgetOptions);
            //if (Index1 > 0 && Index2 > 0)
            //{



            xAxisVarName = gadgetOptions.MainVariableName;
            yAxisVarName = gadgetOptions.CrosstabVariableName;

            //List<MyString> columnNames = new List<MyString>();
            //columnNames.Add(new MyString().VarNamexAxisVar);
            //columnNames.Add(yAxisVar);
            gadgetOptions.UseAdvancedDataFilter = applicationViewModel.UseAdvancedFilter;
            sctrViewModel.GenerateTable(gadgetOptions);
            //}

        }

        void applicationViewModel_ApplyDataFilterEvent(object o)
        {
            if (this.DataContext != null)
            {
                DoScatter();
            }
        }

        /// <summary>
        /// method used to enabled disable Generate Button
        /// </summary>
        private void EnableDisableButton()
        {
            if (cbxScatterXAxisField.SelectedIndex > 0 && cbxScatterYAxisField.SelectedIndex > 0)
            {
                btnRun.IsEnabled = true;
            }
            else
            {
                btnRun.IsEnabled = false;
            }
        }

        /// <summary>
        /// application connectionstring loaded event.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        void applicationViewModel_ConnectionStringLoadedEvent(object o, ConnectionStringEventArgs e)
        {
            // reset tool                   
            gadgetOptions.TableName = applicationViewModel.EwavSelectedDatasource.TableName;
            gadgetOptions.DatasourceName = applicationViewModel.EwavSelectedDatasource.DatasourceName;
        }

        /// <summary>
        /// scatter view model table loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void sctrViewModel_RegressResultsLoadedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            scatterChartWorker_DoWork();
            this.gadgetExpander.IsExpanded = false;
        }

        /// <summary>
        /// Loads Columns
        /// </summary>
        void FillDropDowns()//object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            //List<EwavColumn> SourceColumns =
            //    applicationViewModel.EwavSelectedDatasource.AllColumns;

            //IEnumerable<EwavColumn> CBXFieldCols = from cols in SourceColumns
            //                                       where GetFieldNumericDataType.Contains(cols.SqlDataTypeAsString)
            //                                       orderby cols.Name
            //                                       select cols;

            List<EwavColumn> colsList = CommonClass.GetItemsSource(GetFieldNumericDataType); ;  // CBXFieldCols.ToList();

            //colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            cbxScatterXAxisField.ItemsSource = colsList;
            cbxScatterXAxisField.SelectedValue = "Index";
            cbxScatterXAxisField.DisplayMemberPath = "Name";
            cbxScatterXAxisField.SelectedIndex = Index1;

            foreach (var item in colsList)
            {
                dateColumnNames.Add(item.Name);
            }

            //columnDataType.Clear();

            //columnDataType.Add(ColumnDataType.Numeric);
            //columnDataType.Add(ColumnDataType.Boolean);
            //columnDataType.Add(ColumnDataType.Text);

            //CBXFieldCols = from cols in SourceColumns
            //               where GetFieldNumericDataType.Contains(cols.SqlDataTypeAsString)
            //               orderby cols.Name
            //               select cols;
            //List<EwavColumn> CaseStatusField = CBXFieldCols.ToList();
            //ewc = new EwavColumn();
            //ewc.Name = " ";
            //dateFields.Insert(0, ewc);
            //CaseStatusField.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            cbxScatterYAxisField.ItemsSource = colsList; // CaseStatusField;
            cbxScatterYAxisField.SelectedValue = "Index";
            cbxScatterYAxisField.DisplayMemberPath = "Name";
            cbxScatterYAxisField.SelectedIndex = Index2;

            foreach (var item in colsList)
            {
                numericColumnNames.Add(item.Name);
            }
        }


        /// <summary>
        /// Wires the loaded and unloaded event for the popup window.
        /// </summary>
        public void SetChartLabels()
        {


            LoadViewModel();
            setLabels = new SetLabels(MyControlName, viewModel);// { DataContext = this.DataContext };

            setLabels.Loaded -= new RoutedEventHandler(window_Loaded);
            setLabels.Loaded += new RoutedEventHandler(window_Loaded);
            setLabels.Closed -= new EventHandler(setLabels_Closed);
            setLabels.Closed += new EventHandler(setLabels_Closed);



            setLabels.txtboxXaxis.Text = this.textBlockX.Text;
            setLabels.txtboxYaxis.Text = textBlockY.Text;
            setLabels.txtbxChrtTitle.Text = dp.Title == null ? "" : dp.Title.ToString();


            setLabels.Show();



        }

        private void LoadViewModel()
        {
            viewModel = new SetLabelsViewModel();
            viewModel.GadgetName = tbChartName.Text;
            viewModel.GadgetDescription = tbGadgetDescription.Text;
            viewModel.Width = chart.Width;
            viewModel.Height = chart.Height;
            viewModel.CollorPallet = this.chart.Palette.PaletteName.ToString();

        }

        void setLabels_Closed(object sender, EventArgs e)
        {

            if (setLabels.DialogResult.Value)
            {
                SetValuesForAxis();
            }
        }

        /// <summary>
        /// Loaded event for window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void window_Loaded(object sender, RoutedEventArgs e)
        {
            //window.txtboxXaxis.Text = textBlockX.Text;
            //window.txtboxYaxis.Text = textBlockY.Text;
            //if (dp.Title != null)
            //{
            //    window.txtbxChrtTitle.Text = (string)dp.Title;
            //}
        }

        /// <summary>
        /// Sets Chart Labels
        /// </summary>
        /// <param name="chartTitle"></param>
        /// <param name="legendTitle"></param>
        /// <param name="xAxisLabel"></param>
        /// <param name="yAxisLabel"></param>
        public void SetChartLabels(string chartTitle, string legendTitle, string xAxisLabel, string yAxisLabel)
        {

        }



        /// <summary>
        /// Sets the value for labels
        /// </summary>
        private void SetValuesForAxis()
        {
            viewModel = (SetLabelsViewModel)this.setLabels.DataContext;

            if (textBlockX != null && textBlockY != null && dp != null)
            {
                XAxisLabel = textBlockX.Text = setLabels.txtboxXaxis.Text;
                YAxisLabel = textBlockY.Text = setLabels.txtboxYaxis.Text;
                dp.Title = setLabels.txtbxChrtTitle.Text;
                LoadChart(viewModel);
            }
        }

        private void LoadChart(SetLabelsViewModel viewModel)
        {
            this.tbChartName.Text = viewModel.GadgetName;
            this.tbGadgetDescription.Text = viewModel.GadgetDescription;
            this.chart.Width = viewModel.Width;
            this.chart.Height = viewModel.Height;
            this.chart.Palette = Palette.GetPalette(viewModel.CollorPallet);

        }

        /// <summary>
        /// Saves chart as an image
        /// </summary>
        public void SaveAsImage()
        {
            ExportToPDF etp = new ExportToPDF();
            // XYChart currentChart = (XYChart)dp.Content;// (XYChart)pnlChartContainer.Children[0];//
            DashboardPanel currentChart = dp;
            etp.SavePNG(currentChart, 200);
        }


        void cbxChartType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string chartType = ((ComboBoxItem)e.AddedItems[0]).Content.ToString();
                switch (chartType)
                {

                    case "Scatter":
                        pnlScatterConfig.Visibility = Visibility.Visible;
                        //pnlStackedColumnConfig.Visibility = Visibility.Collapsed;
                        //pnlEpiCurveConfig.Visibility = Visibility.Collapsed;
                        pnlSingleConfig.Visibility = Visibility.Collapsed;
                        break;
                    default:

                        break;
                }
                ResetComboboxes();
            }
        }

        /// <summary>
        /// resets selection in Combo Boxes
        /// </summary>
        private void ResetComboboxes()
        {
            LoadingDropDowns = true;


            if (cbxScatterXAxisField.Items.Count > 0)
            {
                cbxScatterXAxisField.SelectedIndex = -1;
            }
            if (cbxScatterYAxisField.Items.Count > 0)
            {
                cbxScatterYAxisField.SelectedIndex = -1;
            }

            LoadingDropDowns = false;
        }

        /// <summary>
        /// Config field selection changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ConfigField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!LoadingDropDowns)
            {

            }
            EnableDisableButton();
        }


        private void CheckAndSetPosition()
        {
            double top = Canvas.GetTop(this);
            double left = Canvas.GetLeft(this);

            if (top < 0)
            {
                Canvas.SetTop(this, 0);
            }
            if (left < 0)
            {
                Canvas.SetLeft(this, 0);
            }
        }

        /// <summary>
        /// Sends the gadget to the back of the canvas
        /// </summary>
        //private void SendToBack()
        //{
        //    Canvas.SetZIndex(this, -1);
        //}

        /// <summary>
        /// Render Finish
        /// </summary>
        private void RenderFinish()
        {
            waitCursor.Visibility = System.Windows.Visibility.Collapsed;
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            pnlChartContainer.Visibility = System.Windows.Visibility.Visible;
            tbNumberOfRecords.Text = Convert.ToString(RecordCount);
            pnlStatus.Visibility = System.Windows.Visibility.Collapsed;
            txtStatus.Text = string.Empty;
            HeaderButton.IsEnabled = true;
            SetChartLabels(ChartTitle, LegendTitle, XAxisLabel, YAxisLabel);
            FilterButton.IsEnabled = true;
            CheckAndSetPosition();

            if (viewModel != null)
            {
                LoadChart(viewModel);//Loads the values read from CreateFromXML. 
            }
        }

        /// <summary>
        /// Render finish with Warning
        /// </summary>
        /// <param name="errorMessage"></param>
        private void RenderFinishWithWarning(string errorMessage)
        {
            waitCursor.Visibility = System.Windows.Visibility.Collapsed;
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            pnlChartContainer.Visibility = System.Windows.Visibility.Visible;

            //pnlStatus.Background = Brushes.Gold;
            //pnlStatusTop.Background = Brushes.Goldenrod;

            pnlStatus.Visibility = System.Windows.Visibility.Visible;
            txtStatus.Text = errorMessage;

            SetChartLabels(ChartTitle, LegendTitle, XAxisLabel, YAxisLabel);

            CheckAndSetPosition();
        }

        /// <summary>
        /// Render Finish with Error.
        /// </summary>
        /// <param name="errorMessage"></param>
        private void RenderFinishWithError(string errorMessage)
        {
            waitCursor.Visibility = System.Windows.Visibility.Collapsed;
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            pnlChartContainer.Visibility = System.Windows.Visibility.Collapsed;

            //pnlStatus.Background = Brushes.Tomato;
            //pnlStatusTop.Background = Brushes.Red;

            pnlStatus.Visibility = System.Windows.Visibility.Visible;

            txtStatus.Text = errorMessage;

            CheckAndSetPosition();
        }

        /// <summary>
        /// updates status
        /// </summary>
        /// <param name="statusMessage"></param>
        private void RequestUpdateStatusMessage(string statusMessage)
        {
            //this.Dispatcher.BeginInvoke(new SetStatusDelegate(SetStatusMessage), statusMessage);
            SetStatusMessage(statusMessage);
        }

        /// <summary>
        /// updates status
        /// </summary>
        /// <param name="statusMessage"></param>
        private void SetStatusMessage(string statusMessage)
        {
            pnlStatus.Visibility = System.Windows.Visibility.Visible;
            txtStatus.Text = statusMessage;
        }

        private bool IsCancelled()
        {
            //if (worker != null && worker.WorkerSupportsCancellation && worker.CancellationPending)
            //{
            //    return true;
            //}
            //else
            //{
            return false;
            //}
        }


        private bool IsBooleanWithNoStratas
        {
            get
            {
                return this.isBooleanWithNoStratas;
            }
            set
            {
                this.isBooleanWithNoStratas = value;
            }
        }

        public void ClearResults()
        {
            txtStatus.Text = string.Empty;
            pnlStatus.Visibility = Visibility.Collapsed;
            waitCursor.Visibility = Visibility.Visible;

            IsBooleanWithNoStratas = false;

            pnlChartContainer.Children.Clear();
            pnlChartContainer.Visibility = System.Windows.Visibility.Collapsed;
        }

        #region Private Properties
        //private View View
        //{
        //    get
        //    {
        //        return this.dashboardHelper.View;
        //    }
        //}

        //private IDbDriver Database
        //{
        //    get
        //    {
        //        return this.dashboardHelper.Database;
        //    }
        //}
        #endregion // Private Properties

        #region IGadget Members

        public bool IsProcessing { get; set; }

        /// <summary>
        /// Updates the variable names available in the gadget's properties
        /// </summary>
        public void UpdateVariableNames()
        {
            //FillComboboxes(true);
        }

        public StringBuilder HtmlBuilder { get; set; }

        public void RefreshResults()
        {
            if (!LoadingCanvas && !LoadingDropDowns &&
                gadgetOptions != null &&
                (cbxScatterXAxisField.SelectedIndex > -1 && cbxScatterYAxisField.SelectedIndex > -1))//((cbxDateField.SelectedIndex > -1)))|| (cbxColumnXAxisField.SelectedIndex > -1 && cbxColumnYAxisField.SelectedIndex > -1) || (cbxScatterXAxisField.SelectedIndex > -1 && cbxScatterYAxisField.SelectedIndex > -1) || (cbxSingleField.SelectedIndex > -1)))
            {
                waitCursor.Visibility = Visibility.Visible;
                //pnlStatus.Visibility = Visibility.Visible;

                //pnlStatus.Background = Brushes.PaleGreen;
                //pnlStatusTop.Background = Brushes.Green;

                //this.Dispatcher.BeginInvoke(GadgetStatusUpdate, SharedStrings.DASHBOARD_GADGET_STATUS_INITIALIZING);

                gadgetOptions.MainVariableName = string.Empty;
                gadgetOptions.WeightVariableName = string.Empty;
                gadgetOptions.StrataVariableNames = new List<string>();
                gadgetOptions.CrosstabVariableName = string.Empty;
                gadgetOptions.InputVariableList = new Dictionary<string, string>();
                gadgetOptions.ShouldSortHighToLow = false;
                gadgetOptions.TableName = applicationViewModel.EwavSelectedDatasource.TableName;

                gadgetOptions.ShouldUseAllPossibleValues = false;

                if (cbxScatterXAxisField.SelectedIndex >= 0 && cbxScatterYAxisField.SelectedIndex >= 0)
                {
                    gadgetOptions.MainVariableName = ((EwavColumn)cbxScatterXAxisField.SelectedItem).Name.ToString();
                    gadgetOptions.CrosstabVariableName = ((EwavColumn)cbxScatterYAxisField.SelectedItem).Name.ToString();
                    //gadgetOptions.DatasourceName = "NNDSS-HIV";
                    //gadgetOptions.TableName = "vwExternalData";
                    //GenerateScatterChart();
                    gadgetOptions.TableName =
                        applicationViewModel.EwavSelectedDatasource.TableName;
                    gadgetOptions.DatasourceName =
                        applicationViewModel.EwavSelectedDatasource.DatasourceName;
                }
                //break;
            }
        }

        void chart_Loaded(object sender, RoutedEventArgs e)
        {
            SetChartLabels(ChartTitle, LegendTitle, XAxisLabel, YAxisLabel);
        }

        void scatterChartWorker_DoWork()//(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                //this.Dispatcher.BeginInvoke(new SimpleCallback(SetGadgetToProcessingState));
                SetGadgetToProcessingState();
                //this.Dispatcher.BeginInvoke(new SimpleCallback(ClearResults));
                ClearResults();


                ScatterDataDTO regressTable = sctrViewModel.RegressionResults;
                RenderFinishScatterChart(regressTable.DataValues, regressTable.RegresResults, regressTable.MaxValue, regressTable.MinValue);
                SetGadgetToFinishedState();
            }
            catch (Exception ex)
            {
                //this.Dispatcher.BeginInvoke(new RenderFinishWithErrorDelegate(RenderFinishWithError), ex.Message);
                RenderFinishWithError(ex.Message);
                //this.Dispatcher.BeginInvoke(new SimpleCallback(SetGadgetToFinishedState));
                SetGadgetToFinishedState();
            }
        }


        private void RenderFinishScatterChart(List<NumericDataValue> dataValues, LinRegressionResults results, NumericDataValue maxValue, NumericDataValue minValue)
        {
            dp = new DashboardPanel();
            //dp.Title = "My Chart Title";    

            if (ChartTitle != null)
                dp.Title = ChartTitle;


            dp.Theme = "ArcticWhite";
            dp.Style = Resources["DashboardPanelStyle1"] as Style;

            chart = new XYChart();
            chart.XPath = "IndependentValue";
            chart.DefaultStripesVisible = true;
            chart.HighlightDataPointOnHover = true;
            chart.Theme = "ArcticWhite";
            chart.GlareCoverVisible = true;
            chart.SelectionVisualHint = SelectionVisualHint.InvertedColor;
            chart.Width = 800.0;//dataValues.Count * 3;
            chart.MinWidth = 800.0;
            chart.Height = 500.0;
            chart.EnableDataPointPopup = true;
            chart.EnableAnimation = true;
            chart.AnimationDuration = new TimeSpan(0, 0, 0, 5, 0);
            chart.AnimationOnLoad = false;
            chart.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            chart.CoordinatesPaddingPercentage = new Thickness(0, 5, 0, 0);
            chart.Palette = Palette.GetPalette("VibrantA");

            RecordCount = Convert.ToInt64(dataValues.Count);

            //chart.Loaded += new RoutedEventHandler(chart_Loaded);
            //chart.BorderThickness = new Thickness(0);
            //ScatterSeries series = new ScatterSeries();

            //MarkerSeries mseries = new MarkerSeries();
            //mseries.YPath = "IndependentValue";
            //mseries.Marker = MarkerSeries.tri

            var xAxisCoords = new AxisCoordinates();
            //xAxisCoords.Angle = 70.0;
            xAxisCoords.LabelGap = 1.0;
            xAxisCoords.Margin = new Thickness(0, 5, 0, 0);
            //xAxisCoords.MaximumAnnotationLevel = 1;
            xAxisCoords.LabelMargin = 5.0;

            ChartLabel chrtLabel = new ChartLabel();
            chrtLabel.Orientation = ChartLabelOrientation.Horizontal;

            textBlockX = new TextBlock();
            textBlockX.Width = 402.0;

            if (XAxisLabel == "")
                textBlockX.Text = xAxisVarName;
            else
                textBlockX.Text = XAxisLabel;


            textBlockX.TextAlignment = TextAlignment.Center;
            textBlockX.Padding = new Thickness(0.0, 10.0, 0.0, 10.0);
            textBlockX.Margin = new Thickness(0.0, 20.0, 0.0, 0.0);
            chrtLabel.Child = textBlockX;

            chart.XAxisArea.Add(xAxisCoords);
            chart.XAxisArea.Add(chrtLabel);

            var yAxisCoords = new AxisCoordinates();

            chrtLabel = new ChartLabel();
            chrtLabel.Orientation = ChartLabelOrientation.Vertical;

            textBlockY = new TextBlock();
            //tb.Padding = new Thickness(20, 10, 0, 0);

            if (YAxisLabel == "")
                textBlockY.Text = yAxisVarName;   //"Y-Axis";    
            else
                textBlockY.Text = YAxisLabel;

            textBlockY.TextAlignment = TextAlignment.Center;
            textBlockY.Height = 36.0;
            textBlockY.Margin = new Thickness(0, 20, 0, 0);
            textBlockY.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

            chrtLabel.Child = textBlockY;

            chart.YAxisArea.Add(yAxisCoords);
            chart.YAxisArea.Add(chrtLabel);

            MarkerSeriesAnimator msa = MarkerSeriesAnimator.Create("Dropping");

            mseries = new MarkerSeries();
            mseries.YPath = "DependentValue";
            mseries.XPath = "IndependentValue";
            mseries.Id = "S0";
            mseries.Animator = msa;
            mseries.DataSource = dataValues;

            //LinearAxis xaxis = new LinearAxis();
            //xaxis.Orientation = AxisOrientation.X;
            //xaxis.Title = cbxScatterXAxisField.SelectedItem.ToString();
            //series.IndependentAxis = xaxis;

            //LinearAxis yaxis = new LinearAxis();
            //yaxis.Orientation = AxisOrientation.Y;
            //yaxis.Title = cbxScatterYAxisField.SelectedItem.ToString();
            //series.DependentRangeAxis = yaxis;
            //yaxis.ShowGridLines = true;

            //series.IndependentValuePath = "IndependentValue";
            //series.DependentValuePath = "DependentValue";
            //series.ItemsSource = dataValues;
            //CompositeSeries cseries = new CompositeSeries();
            //cseries.SubSeries.Add(mseries);
            //cseries.SubSeries.Add(lseries);
            //chart.DataSeries.Add(cseries);
            chart.XYChartMainArea.Add(mseries);

            if (results.Variables.Count == 0)
            {
                RenderFinishWithError(SharedStrings.NO_RECORDS_SELECTED);
                return;
            }

            if (results.Variables != null)
            {
                decimal coefficient = Convert.ToDecimal(results.Variables[0].Coefficient);
                decimal constant = Convert.ToDecimal(results.Variables[1].Coefficient);

                NumericDataValue newMaxValue = new NumericDataValue();
                newMaxValue.IndependentValue = maxValue.IndependentValue + 1;
                newMaxValue.DependentValue = (coefficient * maxValue.IndependentValue) + constant;
                NumericDataValue newMinValue = new NumericDataValue();
                newMinValue.IndependentValue = minValue.IndependentValue - 1;
                newMinValue.DependentValue = (coefficient * minValue.IndependentValue) + constant;

                List<NumericDataValue> regresValues = new List<NumericDataValue>();
                regresValues.Add(newMinValue);
                regresValues.Add(newMaxValue);

                LineSeriesAnimator lsa = LineSeriesAnimator.Create("Morph");

                regression = new LineSeries();
                //regression.DependentRangeAxis = yaxis;
                //regression.IndependentAxis = xaxis;
                regression.YPath = "DependentValue";
                regression.XPath = "IndependentValue";
                regression.Id = "S1";
                regression.Thickness = 2;
                regression.Animator = lsa;
                regression.DataSource = regresValues;
                chart.XYChartMainArea.Add(regression);

                dp.Content = chart;

                pnlChartContainer.Children.Add(dp);

            }
            else
            {
                pnlChartContainer.Children.Clear();
                RenderFinishWithWarning("Insufficient data to produce this chart.");
            }
            RenderFinish();
        }

        /// <summary>
        /// Sets the gadget to its 'processing' state
        /// </summary>
        public void SetGadgetToProcessingState()
        {
            cbxScatterXAxisField.IsEnabled = false;
            cbxScatterYAxisField.IsEnabled = false;

            this.IsProcessing = true;
        }

        /// <summary>
        /// Sets the gadget to its 'finished' state
        /// </summary>
        public void SetGadgetToFinishedState()
        {
            cbxScatterXAxisField.IsEnabled = true;
            cbxScatterYAxisField.IsEnabled = true;

            this.IsProcessing = false;

            if (GadgetProcessingFinished != null)
            {
                GadgetProcessingFinished(this);
            }
        }

        /// <summary>
        /// Serializes the gadget into Xml
        /// </summary>
        /// <param name="doc">The Xml docment</param>
        XNode IGadget.Serialize(XDocument doc)
        {
            try
            {


                string caseStatusField = string.Empty;
                string dateField = string.Empty;
                string singleField = string.Empty;
                string weightField = string.Empty;
                string strataField = string.Empty;
                string columnAggregateFunction = string.Empty;
                string xAxisField = textBlockX.Text;
                string yAxisField = textBlockY.Text;
                string xAxisFieldScatter = string.Empty;
                string yAxisFieldScatter = string.Empty;
                string chartType = string.Empty;
                string chartSize = "Medium";
                string xAxisRotation = "90";

                if (cbxScatterXAxisField.SelectedIndex > -1)
                {
                    xAxisFieldScatter = ((EwavColumn)cbxScatterXAxisField.SelectedItem).Name.ToString().Replace("<", "&lt;");
                }
                if (cbxScatterYAxisField.SelectedIndex > -1)
                {
                    yAxisFieldScatter = ((EwavColumn)cbxScatterYAxisField.SelectedItem).Name.ToString().Replace("<", "&lt;");
                }

                chartType = "Scatter";

                LoadViewModel();

                XElement element = new XElement("chart",
                    new XAttribute("top", Canvas.GetTop(this).ToString("F0")),
                    new XAttribute("left", Canvas.GetLeft(this).ToString("F0")),
                    new XAttribute("collapsed", "false"),
                    new XAttribute("gadgetType", "Ewav.Scatter"),
                    new XElement("chartType", chartType),
                    new XElement("chartSize", chartSize),
                    new XElement("chartLegendTitle", " "),     //   LegendTitle),
                    new XElement("columnAggregateFunction", columnAggregateFunction),
                    new XElement("caseStatusVariable", caseStatusField),
                    new XElement("dateVariable", dateField),
                    new XElement("singleVariable", singleField),
                    new XElement("weightVariable", weightField),
                    new XElement("strataVariable", strataField),
                    new XElement("yAxisVariable", textBlockY.Text),
                    new XElement("xAxisVariable", textBlockX.Text),
                      new XElement("charttitle", dp.Title == null ? "" : dp.Title.ToString()),
                    new XElement("yAxisScatterVariable", yAxisFieldScatter),
                    new XElement("xAxisScatterVariable", xAxisFieldScatter),
                    new XElement("yAxisLabel", YAxisLabel),
                    new XElement("xAxisLabel", XAxisLabel),

                new XElement("gadgetName", viewModel.GadgetName),
                new XElement("gadgetDescription", Convert.ToBase64String(System.Text.ASCIIEncoding.Unicode.GetBytes(viewModel.GadgetDescription))),
                new XElement("colorPalette", viewModel.CollorPallet),
                new XElement("useDiffColors", viewModel.UseDifferentBarColors),
                new XElement("spacesBetweenBars", viewModel.SpacesBetweenBars),
                new XElement("showLegend", viewModel.ShowLegend),
                new XElement("showVariableNames", viewModel.ShowVariableNames),
                new XElement("legendPosition", viewModel.LegendPostion),
                new XElement("width", viewModel.Width),
                new XElement("height", viewModel.Height)
                );
                if (this.GadgetFilters != null)
                {
                    this.GadgetFilters.Serialize(element);
                }
                //element.Attributes.Append(locationY);
                //element.Attributes.Append(locationX);
                //element.Attributes.Append(collapsed);
                //element.Attributes.Append(type);

                //return element;
                return element;
            }
            catch (Exception ex)
            {


                throw new Exception(ex.Message);

            }

        }


        /// <summary>
        /// Converts the gadget's output to Html
        /// </summary>
        /// <returns></returns>
        public string ToHTML(bool ForDash = false, string htmlFileName = "", int count = 0)
        {
            // Check to see if a chart has been created.
            if (pnlChartContainer.ActualHeight == 0 || pnlChartContainer.ActualWidth == 0)
            {
                return string.Empty;
            }

            StringBuilder htmlBuilder = new StringBuilder();

            if (string.IsNullOrEmpty(CustomOutputHeading))
            {
                htmlBuilder.AppendLine("<h2 class=\"gadgetHeading\">Chart</h2>");
            }
            else if (CustomOutputHeading != "(none)")
            {
                htmlBuilder.AppendLine(string.Format("<h2 class=\"gadgetHeading\">{0}</h2>", CustomOutputHeading));
            }

            if (!string.IsNullOrEmpty(CustomOutputDescription))
            {
                htmlBuilder.AppendLine(string.Format("<p class=\"gadgetsummary\">{0}</p>", CustomOutputDescription));
            }

            string imageFileName = string.Empty;

            if (htmlFileName.EndsWith(".html"))
            {
                imageFileName = htmlFileName.Remove(htmlFileName.Length - 5, 5);
            }
            else if (htmlFileName.EndsWith(".htm"))
            {
                imageFileName = htmlFileName.Remove(htmlFileName.Length - 4, 4);
            }

            imageFileName = string.Format("{0}_{1}.png", imageFileName, count.ToString());

            //BitmapSource img = (BitmapSource)Common.ToImageSource(pnlChartContainer);
            //FileStream stream = new FileStream(imageFileName, FileMode.Create);
            ////JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            //PngBitmapEncoder encoder = new PngBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(img));
            //encoder.Save(stream);
            //stream.Close();

            htmlBuilder.AppendLine(string.Format("<img src=\"{0}\" />", imageFileName));

            HtmlBuilder = htmlBuilder;    

            return htmlBuilder.ToString();
        }

        public string CustomOutputHeading { get; set; }

        public string CustomOutputDescription { get; set; }

        public string CustomOutputCaption { get; set; }

        public string ChartTitle { get; set; }

        public string LegendTitle { get; set; }

        public string XAxisLabel { get; set; }

        public string YAxisLabel { get; set; }

        #endregion

        /// <summary>
        /// Generate Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            //Serialize(new XDocument());

            XAxisLabel = "";
            YAxisLabel = "";
            ChartTitle = "";
            RecordCount = 0;
            //  if (dp != null)
            dp.Title = "";
            DoScatter();
            //sctrViewModel.GenerateTable(
        }

        /// <summary>
        /// Closes the gadget.
        /// </summary>
        public void CloseGadget()
        {
            applicationViewModel.CloseGadget(this);
        }

        /// <summary>
        /// Closes the gadget after confirmation.
        /// </summary>
        void CloseGadgetOnClick()
        {
            CloseGadget confirm = new CloseGadget(this);
            confirm.Show();
        }

        /// <summary>
        /// Close button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseGadgetOnClick();
        }

        private void ResizeButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
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
        /// <summary>
        /// Mouse left button down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// Mouse right button down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayoutRoot_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetSafePosition(cmnClass.GetParentObject<Grid>(this, "LayoutRoot"));
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).GadgetNameOnRightClick = MyControlName;
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).StrataList = null;
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).SelectedGadget = this;
            cmnClass.UpdateZOrder(this, true, cmnClass.GetParentObject<Grid>(this, "LayoutRoot"));
        }

        public string MyControlName
        {
            get
            {
                return "Scatter";
            }
        }

        public string MyUIName
        {
            get
            {
                return "Scatter";
            }
        }

        public ApplicationViewModel ApplicationViewModel
        {
            get
            {
                return this.applicationViewModel;
            }
        }

        void IGadget.CloseGadget()
        {
            CloseGadget();
        }
        /// <summary>
        /// Closes the gadget on click;
        /// </summary>
        void IGadget.CloseGadgetOnClick()
        {
            this.CloseGadgetOnClick();
        }

        public void CreateFromXml(XElement element)
        {



            try
            {

                LoadingCanvas = true;

                //InitializeControl();

                viewModel = new SetLabelsViewModel();
                viewModel.GadgetName = MyUIName.ToString();
                List<EwavColumn> weightColList = cmnClass.GetItemsSource(GetFieldNumericDataType);

                this.GadgetFilters = new List<EwavDataFilterCondition>();

                foreach (XElement child in element.Descendants())
                {
                    switch (child.Name.ToString().ToLower())
                    {

                        case "yaxisscattervariable":
                            cbxScatterYAxisField.SelectedIndex = cmnClass.FindIndexToSelect(weightColList, child.Value.ToString().Replace("&lt;", "<"));
                            Index2 = cbxScatterYAxisField.SelectedIndex;
                            break;
                        case "xaxisscattervariable":
                            cbxScatterXAxisField.SelectedIndex = cmnClass.FindIndexToSelect(weightColList, child.Value.ToString().Replace("&lt;", "<"));
                            Index1 = cbxScatterXAxisField.SelectedIndex;
                            break;

                        case "customheading":
                            this.CustomOutputHeading = child.Value;
                            break;
                        case "customdescription":
                            this.CustomOutputDescription = child.Value;
                            break;
                        case "customcaption":
                            this.CustomOutputCaption = child.Value;
                            break;
                        case "chartsize":
                            cbxChartSize.SelectedItem = child.Value;
                            break;
                        case "charttitle":
                            ChartTitle = child.Value;
                            //          dp.Title = child.Value;
                            break;
                        case "chartlegendtitle":
                            LegendTitle = child.Value;
                            break;
                        case "xaxislabel":
                            textBlockX.Text = XAxisLabel = child.Value;
                            //       textBlockX.Text = child.Value;  
                            break;
                        case "yaxislabel":
                            textBlockY.Text = YAxisLabel = child.Value;
                            //      textBlockY.Text = child.Value;
                            break;
                        //case "xaxisstartvalue":
                        //    txtXAxisStartValue.Text = child.InnerText;
                        //    break;
                        //case "xaxisendvalue":
                        //    txtXAxisEndValue.Text = child.InnerText;
                        //    break;
                        case "gadgetname":
                            viewModel.GadgetName = child.Value.ToString();
                            break;
                        case "gadgetdescription":
                            byte[] encodedDataAsBytes = System.Convert.FromBase64String(child.Value.ToString());
                            viewModel.GadgetDescription =
                               System.Text.ASCIIEncoding.Unicode.GetString(encodedDataAsBytes);
                            break;
                        case "colorpalette":
                            viewModel.CollorPallet = child.Value.ToString();
                            break;
                        case "usediffcolors":
                            viewModel.UseDifferentBarColors = Convert.ToBoolean(child.Value.ToString());
                            break;
                        case "spacesbetweenbars":
                            viewModel.SpacesBetweenBars = child.Value.ToString();
                            break;
                        case "showlegend":
                            viewModel.ShowLegend = Convert.ToBoolean(child.Value.ToString());
                            break;
                        case "showvariablenames":
                            viewModel.ShowVariableNames = Convert.ToBoolean(child.Value.ToString());
                            break;
                        case "legendposition":
                            viewModel.LegendPostion = child.Value.ToString();
                            break;
                        case "width":
                            viewModel.Width = Convert.ToDouble(child.Value.ToString());
                            break;
                        case "height":
                            viewModel.Height = Convert.ToDouble(child.Value.ToString());
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

                double mouseVerticalPosition = 0.0, mouseHorizontalPosition = 0.0;

                foreach (XAttribute attribute in element.Attributes())
                {
                    switch (attribute.Name.ToString().ToLower())
                    {
                        case "top":
                            //mouseVerticalPosition = double.Parse(element.Attribute("top").Value.ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            double.TryParse(element.Attribute("top").Value.ToString(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out mouseVerticalPosition);
                            break;
                        case "left":
                            //mouseHorizontalPosition = double.Parse(element.Attribute("left").Value.ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            double.TryParse(element.Attribute("left").Value.ToString(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out mouseHorizontalPosition);
                            break;
                    }
                }
                LoadingCanvas = false;

                DoScatter();

                cmnClass.AddControlToCanvas(this, mouseVerticalPosition, mouseHorizontalPosition, applicationViewModel.LayoutRoot);

            }
            catch (Exception ex)
            {



                throw new Exception(ex.Message);



            }

        }

        public bool LoadingCanvas { get; set; }

        public bool LoadingDropDowns { get; set; }


        public ClientCommon.XYControlChartTypes GetChartTypeEnum()
        {


            return ClientCommon.XYControlChartTypes.Ignore;


        }


        public void Reload()
        {




            XAxisLabel = "";
            YAxisLabel = "";
            ChartTitle = "";


            //  if (dp != null)
            dp.Title = "";
            DoScatter();


        }

        private void HeaderButton_Click(object sender, RoutedEventArgs e)
        {
            //SetLabels sl = new SetLabels();
            //sl.Show();
            SetChartLabels();
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
                DoScatter();
            }
        }


    }
}

namespace Ewav.Web.Services
{
    public partial class ScatterDomainContext
    {
        //This is the place to set RIA Services query timeout. 
        //TimeSpan(0, 5, 0) means five minutes vs the 
        //default 60 sec
        partial void OnCreated()
        {
            if (!DesignerProperties.GetIsInDesignMode(Application.Current.RootVisual))
            {
                ((WebDomainClient<IScatterDomainServiceContract>)DomainClient).ChannelFactory.Endpoint.Binding.SendTimeout =
                    new TimeSpan(0, 120, 0);
            }
        }
    }
}