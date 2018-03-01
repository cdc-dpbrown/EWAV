/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       XYChartControl.xaml.cs
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using CommonLibrary;
using ComponentArt.Silverlight.DataVisualization;
using ComponentArt.Silverlight.DataVisualization.Charting;
using ComponentArt.Silverlight.DataVisualization.Common;
using ComponentArt.Silverlight.Export.PDF;
using Ewav.Web.EpiDashboard;
using Ewav.BAL;
using Ewav.ClientCommon;
using Ewav.Common;
using Ewav.DTO;
using Ewav.ExtensionMethods;
using Ewav.ViewModels;
using Ewav.Web.Services;

using System.Globalization;
using System.Text;


namespace Ewav
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(UserControl))]
    [ExportMetadata("type", "chart")]
    [ExportMetadata("tabindex", "13")]
    public partial class XYChartControl : UserControl, IGadget, IEwavGadget, IChartControl
    {
        private readonly char[] SplitTokens = " \t;".ToCharArray();
        private ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;
        private bool loadingCombos;
        private TextBlock textBlockX, textBlockY;
        private bool isBooleanWithNoStratas = false;
        private GadgetParameters gadgetOptions;
        private object syncLock = new object();
        private object objXAxisStart;
        private object objXAxisEnd;
        DatatableBag databag;
        DashboardPanel dbp;
        XYChartViewModel xyChartViewModel;
        List<string> dateColumnNames = new List<string>();
        List<string> numericColumnNames = new List<string>();
        List<List<StringDataValue>> dataValues = new List<List<StringDataValue>>();
        ClientCommon.Common cmnClass;
        List<EwavColumn> primaryListColumns = null, stratListColumns = null;
        List<ColumnDataType> columnDataType_primary, columnDataType_numeric;
        XYChart XYChartControlChart;
        PieChart PieChart;
        string ErrMessage = "";
        int Index1 = -1, Index2 = -1, Index3 = -1, Index4 = -1, Index5 = -1;
        EwavColumn Col1, Col2, Col3, Col4, Col5;
        private bool chartDataLoaded = false;
        SetLabelsViewModel viewModel = null;
        BarSeries XYChartControlSeries = null;
        private bool loadingDropDowns = false;

        public XYControlChartTypes chartTypeEnum;
        CompositeSeries compositeSeries = null;
        //double recordCount = 0.0;

        private long recordCount;

        public long RecordCount
        {
            get { return recordCount; }
            set { recordCount = value; }
        }

        /// <summary>
        /// Container that holds gadget level filters.
        /// </summary>
        public List<EwavDataFilterCondition> GadgetFilters { get; set; }

        public bool LoadingDropDowns
        {
            get
            {
                return loadingDropDowns;
            }
            set
            {
                loadingDropDowns = value;
            }
        }

        private bool loadingCanvas = false;

        public bool LoadingCanvas
        {
            get
            {
                return loadingCanvas;
            }
            set
            {
                loadingCanvas = value;
            }
        }

        private List<string> comboBoxItems = new List<string>();

        public List<string> ComboBoxItems
        {
            get
            {
                return comboBoxItems;
            }
            set
            {
                comboBoxItems = value;
            }
        }

        /// <summary>
        /// Toes the HTML.
        /// </summary>
        /// <param name="ForDash"></param>
        /// <param name="htmlFileName">Name of the HTML file.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public string ToHTML(bool ForDash = false, string htmlFileName = "", int count = 0)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

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

        public List<ColumnDataType> GetPrimaryFieldColumnDataType
        {
            get
            {
                columnDataType_primary = new List<ColumnDataType>();

                columnDataType_primary.Add(ColumnDataType.Boolean);
                columnDataType_primary.Add(ColumnDataType.Numeric);
                columnDataType_primary.Add(ColumnDataType.Text);
                columnDataType_primary.Add(ColumnDataType.DateTime);
                columnDataType_primary.Add(ColumnDataType.UserDefined);

                return columnDataType_primary;
            }
        }

        public List<ColumnDataType> GetNumericFieldColumnDataType
        {
            get
            {
                columnDataType_numeric = new List<ColumnDataType>();
                columnDataType_numeric.Add(ColumnDataType.Numeric);

                return columnDataType_numeric;
            }
        }

        XNode IGadget.Serialize(XDocument doc)
        {
            string caseStatusField = string.Empty;
            string dateField = string.Empty;
            string singleField = string.Empty;
            string weightField = string.Empty;
            string strataField = string.Empty;
            string columnAggregateFunction = string.Empty;
            string xAxisField = string.Empty;
            string yAxisField = string.Empty;
            string xAxisFieldScatter = string.Empty;
            string yAxisFieldScatter = string.Empty;
            string chartType = string.Empty;
            string chartSize = "Medium";
            //string xAxisRotation = "90";



            if (cbxChartSize.SelectedIndex >= 0)
            {

                chartSize = cbxChartSize.SelectedValue.ToString();
            }

            chartType = this.chartTypeEnum.ToString();

            if (cbxColumnXAxisField.SelectedIndex > -1)
            {
                xAxisField = ((EwavColumn)cbxColumnXAxisField.SelectedItem).Name.ToString().Replace("<", "&lt;");
            }
            if (cbxColumnYAxisField.SelectedIndex > -1)
            {
                yAxisField = ((EwavColumn)cbxColumnYAxisField.SelectedItem).Name.ToString().Replace("<", "&lt;");
            }
            if (cbxSingleField.SelectedIndex > -1)
            {
                singleField = ((EwavColumn)cbxSingleField.SelectedItem).Name.ToString().Replace("<", "&lt;");
            }

            if (cbxWeightField.SelectedIndex > -1)
            {
                weightField = ((EwavColumn)cbxWeightField.SelectedItem).Name.ToString().Replace("<", "&lt;");
            }
            if (cbxStrataField.SelectedIndex > -1)
            {
                strataField = ((EwavColumn)cbxStrataField.SelectedItem).Name.ToString().Replace("<", "&lt;");
            }
            if (cbxColumnAggregateFunc.SelectedIndex > -1)
            {
                columnAggregateFunction = ((ComboBoxItem)cbxColumnAggregateFunc.SelectedItem).Content.ToString();
            }

            LoadViewModel();

            XElement element = new XElement("chart",
                new XAttribute("top", Canvas.GetTop(this).ToString("F0")),
                new XAttribute("left", Canvas.GetLeft(this).ToString("F0")),
                new XAttribute("collapsed", "false"),
                new XAttribute("gadgetType", "Ewav.XYChartControl"),
                new XElement("chartType", chartType),
                new XElement("chartSize", chartSize),
                   new XElement("chartTitle", dbp == null ? "" : dbp.Title.ToString()),
                new XElement("chartLegendTitle", LegendTitle),
                new XElement("columnAggregateFunction", columnAggregateFunction),
                new XElement("caseStatusVariable", caseStatusField),
                new XElement("dateVariable", dateField),
                new XElement("singleVariable", singleField),
                new XElement("weightVariable", weightField),
                new XElement("strataVariable", strataField),
                new XElement("yAxisVariable", yAxisField),
                new XElement("xAxisVariable", xAxisField),
                new XElement("yAxisLabel", textBlockY == null ? "" : textBlockY.Text),
                new XElement("xAxisLabel", textBlockX == null ? "" : textBlockX.Text),

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



        public class StringDataValue
        {
            public double DependentValue { get; set; }
            public string IndependentValue { get; set; }
            public string StratificationValue { get; set; }
            public double CurrentMeanValue { get; set; }
        }

        public class NumericDataValue
        {
            public decimal DependentValue { get; set; }
            public decimal IndependentValue { get; set; }
        }

        public class TypeStringTuple
        {
            public Type Item1 { get; set; }
            public string Item2 { get; set; }
            public TypeStringTuple(Type item1, string item2)
            {
                this.Item1 = item1;
                this.Item2 = item2;
            }
        }

        private string chartName;

        public string ChartName
        {
            get
            {
                if (this.chartName == null)
                {
                    return "XYChart";
                }
                else
                {
                    return this.chartName;
                }
            }
            set
            {
                this.chartName = value;
                this.chartTypeEnum = (XYControlChartTypes)Utils.ParseEnum<XYControlChartTypes>(this.chartName.Replace(" ", ""));
            }
        }

        public XYChartControl()
        {
            cmnClass = new ClientCommon.Common();
            this.InitializeComponent();

            this.gadgetOptions = new GadgetParameters();
            this.gadgetOptions.ShouldIncludeFullSummaryStatistics = false;
            this.gadgetOptions.ShouldIncludeMissing = false;
            this.gadgetOptions.ShouldSortHighToLow = false;
            this.gadgetOptions.ShouldUseAllPossibleValues = false;
            this.gadgetOptions.StrataVariableList = new List<MyString>();

            this.FillDropDowns();

            this.Loaded += new RoutedEventHandler(XYChartControl_Loaded);
            ComboBoxItems.Clear();
            ComboBoxItems.Add("Small");
            ComboBoxItems.Add("Medium");
            ComboBoxItems.Add("Large");
            cbxChartSize.ItemsSource = ComboBoxItems;
            cbxChartSize.SelectedIndex = 0;
            //this.tbChartName.Text = this.ChartName.FromCamelCase();
            //this.InitializeXYControl();
        }

        ScaleTransform scaleTransform;

        void XYChartControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeXYControl();
            //DoChart();
            this.EnableDisableButton();
            ShowAppropriateChart();
        }

        public void InitializeXYControl()
        {
            scaleTransform = new ScaleTransform();
            applicationViewModel.ApplyDataFilterEvent += new Client.Application.ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);
            applicationViewModel.DefinedVariableAddedEvent += new Client.Application.DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);
            applicationViewModel.DefinedVariableInUseDeletedEvent += new Client.Application.DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableInUseDeletedEvent);
            applicationViewModel.DefinedVariableNotInUseDeletedEvent += new Client.Application.DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
            applicationViewModel.PreColumnChangedEvent += new Client.Application.PreColumnChangedEventHandler(applicationViewModel_PreColumnChangedEvent);

            applicationViewModel.UnloadedEvent += new Client.Application.UnloadedEventHandler(applicationViewModel_UnloadedEvent);

            try
            {
                DatatableBag eCrvData = new DatatableBag();
                this.xyChartViewModel = (XYChartViewModel)this.DataContext;
                this.xyChartViewModel.FrequencyTableLoadedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(XYChartControlViewModel_FrequencyTableLoadedEvent);
                this.xyChartViewModel.ErrorNotice += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(xyChartViewModel_ErrorNotice);
            }
            catch (Exception)
            {
                throw;
            }
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

        private void ShowAppropriateChart()
        {
            SetChartTitle(this.ChartName);

            if (!LoadingCanvas)
            {
                if (this.chartTypeEnum == XYControlChartTypes.StackedColumn)
                {
                    this.pnlStackedColumnConfig.Visibility = System.Windows.Visibility.Visible;
                    this.pnlSingleConfig.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    this.pnlStackedColumnConfig.Visibility = System.Windows.Visibility.Collapsed;
                    this.pnlSingleConfig.Visibility = System.Windows.Visibility.Visible;
                }

                if (this.chartTypeEnum == XYControlChartTypes.Pareto || this.chartTypeEnum == XYControlChartTypes.Pie)
                {
                    this.cbxStrataField.Visibility = System.Windows.Visibility.Collapsed;
                    this.tblockStrataField.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        void applicationViewModel_PreColumnChangedEvent(object o)
        {
            //throw new NotImplementedException();
            SaveColumnValues();
        }

        void applicationViewModel_DefinedVariableNotInUseDeletedEvent(object o)
        {
            SearchIndex();
            LoadingDropDowns = true;
            FillDropDowns();
            LoadingDropDowns = false;
            DoChart();
        }

        void applicationViewModel_DefinedVariableInUseDeletedEvent(object o)
        {
            //throw new NotImplementedException();
            //if ((FieldSelectedCol1 != null || Col2 != null ||  CrossTabSelectedCol3 != null) 
            //    || (StrataSelectedCol4 != null || Col5 != null))
            //{
            if (IsDFUsedInThisGadget())
            {
                ResetGadget();
            }
            else
            {
                SearchIndex();
            }
            LoadingDropDowns = true;
            FillDropDowns();
            LoadingDropDowns = false;
        }

        private bool IsDFUsedInThisGadget()
        {
            return (Col1 != null && Col1.Name == applicationViewModel.ItemToBeRemoved.Name) ||
                            (Col2 != null && Col2.Name == applicationViewModel.ItemToBeRemoved.Name) ||
                            (Col3 != null && Col3.Name == applicationViewModel.ItemToBeRemoved.Name) ||
                            (Col4 != null && Col4.Name == applicationViewModel.ItemToBeRemoved.Name) ||
                            (Col5 != null && Col5.Name == applicationViewModel.ItemToBeRemoved.Name);
        }

        private void ResetGadget()
        {
            SearchIndex();
            if (IsDFUsedInThisGadget())
            {
                Index1 = Index2 = Index3 = Index4 = Index5 = -1;
                pnlChartContainer.Visibility = System.Windows.Visibility.Collapsed;
            }
            LoadingDropDowns = true;
            FillDropDowns();
            LoadingDropDowns = false;

        }


        void applicationViewModel_DefinedVariableAddedEvent(object o)
        {
            //throw new NotImplementedException();
            SearchIndex();
            LoadingDropDowns = true;
            FillDropDowns();
            LoadingDropDowns = false;
            DoChart();
        }

        /// <summary>
        /// Searches for Selected index of all the columns
        /// </summary>
        private void SearchIndex()
        {
            if (Col1 != null)
            {
                Index1 = cmnClass.SearchCurrentIndex(Col1, cmnClass.GetItemsSource(GetPrimaryFieldColumnDataType)); // SearchCurrentIndex(Col1);
            }
            if (Col2 != null)
            {
                Index2 = cmnClass.SearchCurrentIndex(Col2, cmnClass.GetItemsSource(GetNumericFieldColumnDataType));  //SearchCurrentIndex(Col2);
            }
            if (Col3 != null)
            {
                Index3 = cmnClass.SearchCurrentIndex(Col3, cmnClass.GetItemsSource(GetPrimaryFieldColumnDataType, false));  //SearchCurrentIndex(Col3);
            }
            if (Col4 != null)
            {
                Index4 = cmnClass.SearchCurrentIndex(Col4, cmnClass.GetItemsSource(GetPrimaryFieldColumnDataType));  //SearchCurrentIndex(Col4);
            }
            if (Col5 != null)
            {
                Index5 = cmnClass.SearchCurrentIndex(Col5, cmnClass.GetItemsSource(GetPrimaryFieldColumnDataType, false));  //SearchCurrentIndex(Col5);
            }
        }

        /// <summary>
        /// Saves columns values
        /// </summary>
        private void SaveColumnValues()
        {
            Col1 = (EwavColumn)cbxSingleField.SelectedItem;
            Col2 = (EwavColumn)cbxWeightField.SelectedItem;
            Col3 = (EwavColumn)cbxStrataField.SelectedItem;
            Col4 = (EwavColumn)cbxColumnXAxisField.SelectedItem;
            Col5 = (EwavColumn)cbxColumnYAxisField.SelectedItem;
        }


        void xyChartViewModel_ErrorNotice(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            RenderFinishWithError(e.Data.Message);
            this.SetGadgetToFinishedState();
        }

        void applicationViewModel_ApplyDataFilterEvent(object o)
        {
            if (applicationViewModel.RemoveIndicator &&
                IsDFUsedInThisGadget())
            {
                ResetGadget();
            }
            else
            {
                DoChart();
            }
        }



        void XYChartControlViewModel_FrequencyTableLoadedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            chartDataLoaded = true;
            RecordCount = 0;
            this.GenerateXYChartControl();
        }

        void FillDropDowns()
        {
            //IEnumerable<EwavColumn> CBXFieldCols = from cols in this.applicationViewModel.EwavSelectedDatasource.AllColumns    //  eCrvViewModel.Columns
            //                                       where columnDataType.Contains(cols.SqlDataTypeAsString)
            //                                       orderby cols.Name
            //                                       select cols;
            //List<EwavColumn> colsList = CBXFieldCols.ToList();
            //colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1, NoCamelName = " " });
            primaryListColumns = cmnClass.GetItemsSource(GetPrimaryFieldColumnDataType);
            this.cbxSingleField.ItemsSource = primaryListColumns;
            this.cbxSingleField.SelectedValue = "Index";
            //this.cbxSingleField.DisplayMemberPath = "NoCamelName";
            this.cbxSingleField.DisplayMemberPath = "Name";
            this.cbxSingleField.SelectedIndex = Index1;

            this.cbxColumnXAxisField.ItemsSource = primaryListColumns;
            this.cbxColumnXAxisField.SelectedValue = "Index";
            //this.cbxColumnXAxisField.DisplayMemberPath = "NoCamelName";
            this.cbxColumnXAxisField.DisplayMemberPath = "Name";
            this.cbxColumnXAxisField.SelectedIndex = Index4;

            //foreach (var item in primaryListColumns)
            //{
            //    this.dateColumnNames.Add(item.Name);
            //}

            stratListColumns = cmnClass.GetItemsSource(GetNumericFieldColumnDataType);

            //colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            //stratListColumns.Add(ColumnDataType.Numeric);
            //columnDataType.Add(ColumnDataType.Boolean);
            //columnDataType.Add(ColumnDataType.Text);

            //CBXFieldCols = from cols in this.applicationViewModel.EwavSelectedDatasource.AllColumns
            //               where columnDataType.Contains(cols.SqlDataTypeAsString)
            //               orderby cols.Name
            //               select cols;
            //List<EwavColumn> CaseStatusField = CBXFieldCols.ToList();

            //CaseStatusField.Insert(0, new EwavColumn() { Name = " ", Index = -1, NoCamelName = " " });

            this.cbxWeightField.ItemsSource = stratListColumns;
            this.cbxWeightField.SelectedValue = "Index";
            //this.cbxWeightField.DisplayMemberPath = "NoCamelName";
            this.cbxWeightField.DisplayMemberPath = "Name";
            this.cbxWeightField.SelectedIndex = Index2;

            primaryListColumns = cmnClass.GetItemsSource(GetPrimaryFieldColumnDataType);

            this.cbxStrataField.ItemsSource = primaryListColumns;
            this.cbxStrataField.SelectedValue = "Index";
            //this.cbxStrataField.DisplayMemberPath = "NoCamelName";
            this.cbxStrataField.DisplayMemberPath = "Name";
            this.cbxStrataField.SelectedIndex = Index3;

            primaryListColumns = cmnClass.GetItemsSource(GetPrimaryFieldColumnDataType, false);

            this.cbxColumnYAxisField.ItemsSource = primaryListColumns;
            this.cbxColumnYAxisField.SelectedValue = "Index";
            //this.cbxColumnYAxisField.DisplayMemberPath = "NoCamelName";
            this.cbxColumnYAxisField.DisplayMemberPath = "Name";
            this.cbxColumnYAxisField.SelectedIndex = Index5;
            //ewc = new EwavColumn();
            //ewc.Name = " ";
            //dateFields.Insert(0, ewc);
            //CaseStatusField.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            //cbxCaseStatusField.ItemsSource = CaseStatusField;
            //cbxCaseStatusField.SelectedValue = "Index";
            //cbxCaseStatusField.DisplayMemberPath = "Name";
            //foreach (var item in stratListColumns)
            //{
            //    this.numericColumnNames.Add(item.Name);
            //}
        }

        bool ValidateComboBoxes()
        {
            StackPanel stackpnl = null;
            bool indicator = true;
            if (pnlSingleConfig.Visibility == System.Windows.Visibility.Visible)
            {
                stackpnl = pnlSingleConfig;
            }
            else
            {
                stackpnl = pnlStackedColumnConfig;
            }
            Dictionary<string, string> testDict = new Dictionary<string, string>();
            foreach (var item in stackpnl.Children)
            {
                if (item is ComboBox && ((ComboBox)item).SelectedIndex > 0 &&
                    !((ComboBox)item).Name.ToString().Contains("ChartSize") &&
                    !((ComboBox)item).Name.ToString().Contains("ColumnAggregateFunc"))
                {
                    try
                    {
                        testDict.Add(((EwavColumn)((ComboBox)item).SelectedItem).Name, ((EwavColumn)((ComboBox)item).SelectedItem).Name);
                    }
                    catch (Exception ex)
                    {
                        RenderFinishWithError(ex.Message);
                        return false;
                    }
                }
                if (item is ComboBox &&
                    (((ComboBox)item).Name == "cbxSingleField" || ((ComboBox)item).Name == "cbxColumnXAxisField") &&
                    ((ComboBox)item).SelectedIndex < 0)
                {
                    indicator = false;
                }
            }
            return indicator;
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

        SetLabels xwindow;

        public void SetChartLabels()
        {

            LoadViewModel();
            this.xwindow = new SetLabels(ChartName, viewModel);// { DataContext = this.DataContext };

            this.xwindow.Unloaded -= new RoutedEventHandler(window_Unloaded);
            this.xwindow.Unloaded += new RoutedEventHandler(window_Unloaded);
            //if (this.textBlockX != null && this.textBlockY != null)
            //{
            //    this.xwindow.txtboxXaxis.Text = this.textBlockX.Text;
            //    this.xwindow.txtboxYaxis.Text = this.textBlockY.Text;
            //}

            this.xwindow.txtbxChrtTitle.Text = dbp.Title == null ? "" : dbp.Title.ToString();


            this.xwindow.Show();



        }

        private void LoadViewModel()
        {
            //if (viewModel == null)
            //{


            viewModel = new SetLabelsViewModel();
            viewModel.GadgetName = tbChartName.Text;
            viewModel.GadgetDescription = tbGadgetDescription.Text;

            if (XYChartControlChart != null)
            {
                viewModel.ShowLegend = this.XYChartControlChart.LegendVisible;
                viewModel.Width = this.XYChartControlChart.Width;
                viewModel.Height = this.XYChartControlChart.Height;
                viewModel.CollorPallet = this.XYChartControlChart.Palette.PaletteName.ToString();
                viewModel.UseDifferentBarColors = this.XYChartControlChart.UseDifferentBarColors;
                viewModel.XaxisLabel = this.textBlockX.Text;
                viewModel.YaxisLabel = this.textBlockY.Text;
                if (XYChartControlSeries != null)
                {
                    switch (XYChartControlSeries.RelativePointSpace.ToString().Substring(0, 3).Replace(',','.'))
                    {
                        case "0.1":
                            viewModel.SpacesBetweenBars = "Small";
                            break;
                        case "0.4":
                            viewModel.SpacesBetweenBars = "Medium";
                            break;
                        case "0.8":
                            viewModel.SpacesBetweenBars = "Large";
                            break;
                        default:
                            viewModel.SpacesBetweenBars = "None";
                            break;
                    }
                }

                switch (this.XYChartControlChart.LegendDock)
                {
                    case ComponentArt.Silverlight.DataVisualization.Charting.Dock.Bottom:
                        viewModel.LegendPostion = "Bottom";
                        break;
                    case ComponentArt.Silverlight.DataVisualization.Charting.Dock.Left:
                        viewModel.LegendPostion = "Left";
                        break;
                    case ComponentArt.Silverlight.DataVisualization.Charting.Dock.Right:
                        viewModel.LegendPostion = "Right";
                        break;

                    case ComponentArt.Silverlight.DataVisualization.Charting.Dock.Top:
                        viewModel.LegendPostion = "Top";
                        break;
                    default:
                        viewModel.LegendPostion = "Top";
                        break;
                }
            }
            else
            {
                viewModel.ShowLegend = this.PieChart.LegendVisible;
                viewModel.Width = this.PieChart.Width;
                viewModel.Height = this.PieChart.Height;
                viewModel.CollorPallet = this.PieChart.Palette.PaletteName.ToString();


                switch (this.PieChart.LegendDock)
                {
                    case ComponentArt.Silverlight.DataVisualization.Charting.Dock.Bottom:
                        viewModel.LegendPostion = "Bottom";
                        break;
                    case ComponentArt.Silverlight.DataVisualization.Charting.Dock.Left:
                        viewModel.LegendPostion = "Left";
                        break;
                    case ComponentArt.Silverlight.DataVisualization.Charting.Dock.Right:
                        viewModel.LegendPostion = "Right";
                        break;

                    case ComponentArt.Silverlight.DataVisualization.Charting.Dock.Top:
                        viewModel.LegendPostion = "Top";
                        break;
                    default:
                        viewModel.LegendPostion = "Top";
                        break;
                }
            }





            //switch (ChartName.ToLower())
            //{
            //    case "column":
            //    case "bar":
            //    case "pareto":
            //        viewModel.UseDifferentBarColors = this.XYChartControlChart.UseDifferentBarColors;
            //        break;
            //    default:
            //        viewModel.UseDifferentBarColors = false;
            //        break;
            //}
            // }
        }

        public void SetChartTitle(string chartTitle)
        {
            this.ChartName = chartTitle;
            this.tbChartName.Text = this.ChartName;
        }


        void window_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.xwindow.DialogResult.Value)
            {
                this.SetValuesForAxis();
            }
        }

        private void SetValuesForAxis()
        {
            viewModel = (SetLabelsViewModel)this.xwindow.DataContext;

            if (textBlockX != null && textBlockY != null)
            {

                XAxisLabel = viewModel.XaxisLabel;
                YAxisLabel = viewModel.YaxisLabel;

                this.textBlockX.Text = viewModel.XaxisLabel;
                this.textBlockY.Text = viewModel.YaxisLabel;
            }

            this.dbp.Title = viewModel.ChartTitle;
            LoadChart(viewModel);
        }

        private void LoadChart(SetLabelsViewModel viewModel)
        {
            this.tbChartName.Text = viewModel.GadgetName;
            this.tbGadgetDescription.Text = viewModel.GadgetDescription;

            if (XYChartControlChart != null)
            {
                //this.XYChartControlChart.LegendVisible = false;
                this.XYChartControlChart.Width = viewModel.Width;
                this.XYChartControlChart.Height = viewModel.Height;
                
                foreach (var item in compositeSeries.SubSeries)
                {
                    if (item != null && viewModel.SpacesBetweenBars != null)
                    {
                        switch (viewModel.SpacesBetweenBars.ToLower())
                        {
                            case "small":
                                item.RelativePointSpace = 0.1;
                                break;
                            case "medium":
                                item.RelativePointSpace = 0.4;
                                break;
                            case "large":
                                item.RelativePointSpace = 0.8;
                                break;
                            default:
                                item.RelativePointSpace = -0.1;
                                break;
                        }
                    }
                }
                

                switch (ChartName.ToLower())
                {
                    case "column":
                    case "bar":
                    case "pareto":
                        if (compositeSeries.SubSeries.Count <= 1)
                        {
                            this.XYChartControlChart.UseDifferentBarColors = viewModel.UseDifferentBarColors;
                        }

                        break;
                    default:
                        this.XYChartControlChart.UseDifferentBarColors = false;
                        break;
                }

                //this.XYChartControlChart.Legend.Visibility = System.Windows.Visibility.Collapsed;
                this.XYChartControlChart.LegendVisible = viewModel.ShowLegend;
                
                switch (viewModel.LegendPostion.ToLower())
                {
                    case "left":
                        this.XYChartControlChart.LegendDock = ComponentArt.Silverlight.DataVisualization.Charting.Dock.Left;
                        break;
                    case "right":
                        this.XYChartControlChart.LegendDock = ComponentArt.Silverlight.DataVisualization.Charting.Dock.Right;
                        break;
                    case "bottom":
                        this.XYChartControlChart.LegendDock = ComponentArt.Silverlight.DataVisualization.Charting.Dock.Bottom;
                        break;
                    default:
                        this.XYChartControlChart.LegendDock = ComponentArt.Silverlight.DataVisualization.Charting.Dock.Top;
                        break;
                }
                this.XYChartControlChart.Legend.Orientation = ComponentArt.Silverlight.DataVisualization.Common.LegendItemOrientation.Vertical;
                this.XYChartControlChart.Palette = Palette.GetPalette(viewModel.CollorPallet);
            }
            else
            {
                //this.PieChart.LegendVisible = false;
                this.PieChart.Width = viewModel.Width;
                this.PieChart.Height = viewModel.Height;

                switch (viewModel.LegendPostion.ToLower())
                {
                    case "left":
                        this.PieChart.LegendDock = ComponentArt.Silverlight.DataVisualization.Charting.Dock.Left;
                        break;
                    case "right":
                        this.PieChart.LegendDock = ComponentArt.Silverlight.DataVisualization.Charting.Dock.Right;
                        break;
                    case "bottom":
                        this.PieChart.LegendDock = ComponentArt.Silverlight.DataVisualization.Charting.Dock.Bottom;
                        break;
                    default:
                        this.PieChart.LegendDock = ComponentArt.Silverlight.DataVisualization.Charting.Dock.Top;
                        break;
                }

                //this.PieChart.Legend.Visibility = System.Windows.Visibility.Collapsed;
                this.PieChart.LegendVisible = viewModel.ShowLegend;

                this.PieChart.Legend.Orientation = ComponentArt.Silverlight.DataVisualization.Common.LegendItemOrientation.Vertical;
                //this.PieChart.Legend.Visibility = System.Windows.Visibility.Visible;
                this.PieChart.Palette = Palette.GetPalette(viewModel.CollorPallet);
            }
        }

        public void SaveAsImage()
        {
            ExportToPDF etp = new ExportToPDF();
            // XYChart currentChart = (XYChart)dp.Content;// (XYChart)pnlChartContainer.Children[0];//
            DashboardPanel currentChart = this.dbp;
            etp.SavePNG(currentChart, 100);
        }

        private void ResetComboboxes()
        {
            this.loadingCombos = true;

            //if (cbxCaseStatusField.Items.Count > 0)
            //    cbxCaseStatusField.SelectedIndex = -1;
            //if (cbxDateField.Items.Count > 0)
            //    cbxDateField.SelectedIndex = -1;
            //if (cbxSingleField.Items.Count > 0)
            //    cbxSingleField.SelectedIndex = -1;

            this.loadingCombos = false;
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

        private void RenderFinish()
        {

            this.waitCursor.Visibility = System.Windows.Visibility.Collapsed;
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            this.pnlChartContainer.Visibility = System.Windows.Visibility.Visible;

            this.pnlStatus.Visibility = System.Windows.Visibility.Collapsed;
            this.txtStatus.Text = string.Empty;

            lblNumberOfRecords.Visibility = System.Windows.Visibility.Visible;
            tbNumberOfRecords.Visibility = System.Windows.Visibility.Visible;
            tbNumberOfRecords.Text = Convert.ToString(RecordCount);

            HeaderButton.IsEnabled = true;
            FilterButton.IsEnabled = true;

            this.CheckAndSetPosition();

            string chartSize = "Large";

            if (this.gadgetOptions.InputVariableList != null && this.gadgetOptions.InputVariableList.ContainsKey("chartsize"))
            {
                chartSize = this.gadgetOptions.InputVariableList["chartsize"];
            }

            ScaleTransform st = this.RenderTransform as ScaleTransform;

            switch (this.chartTypeEnum)
            {
                case XYControlChartTypes.StackedColumn:
                case XYControlChartTypes.Line:
                case XYControlChartTypes.Bar:
                case XYControlChartTypes.Column:
                case XYControlChartTypes.Pareto:
                    this.XYChartControlChart.Width = Defaults.CHART_WIDTH; // 800.0;
                    this.XYChartControlChart.Height = Defaults.CHART_HEGHT; // 500.0;
                    break;
                case XYControlChartTypes.Pie:
                    this.PieChart.Width = Defaults.CHART_WIDTH; //800.0;
                    this.PieChart.Height = Defaults.CHART_HEGHT; //500.0;
                    break;
                default:
                    break;
            }


            //switch (this.chartTypeEnum)
            //{
            //    case XYControlChartTypes.StackedColumn:
            //    case XYControlChartTypes.Line:
            //    case XYControlChartTypes.Column:
            //    case XYControlChartTypes.Bar:
            //    case XYControlChartTypes.Pareto:
            //        switch (chartSize)
            //        {
            //            case "Medium":
            //                this.XYChartControlChart.Height = this.XYChartControlChart.Height * 1.15 + 100;
            //                this.XYChartControlChart.Width = this.XYChartControlChart.Width * 1.15 + 100;
            //                break;
            //            case "Large":
            //                this.XYChartControlChart.Height = this.XYChartControlChart.Height * 1.3 + 100;
            //                this.XYChartControlChart.Width = this.XYChartControlChart.Width * 1.3 + 100;

            //                break;
            //        }
            //        break;
            //    case XYControlChartTypes.Pie:
            //        switch (chartSize)
            //        {
            //            case "Medium":

            //                this.PieChart.Height = this.PieChart.Height * 1.15 + 100;
            //                this.PieChart.Width = this.PieChart.Width * 1.15 + 100;
            //                break;
            //            case "Large":
            //                this.PieChart.Height = this.PieChart.Height * 1.3 + 100;
            //                this.PieChart.Width = this.PieChart.Width * 1.3 + 100;
            //                break;
            //        }
            //        break;
            //    default:
            //        throw new Exception(string.Format("Unhandled chart type {0}", chartTypeEnum.ToString()));
            //}

            if (viewModel != null)
            {
                LoadChart(viewModel);//Loads the values read from CreateFromXML. 
            }
        }

        private void RenderFinishWithWarning(string errorMessage)
        {
            this.waitCursor.Visibility = System.Windows.Visibility.Collapsed;
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            this.pnlChartContainer.Visibility = System.Windows.Visibility.Visible;

            //pnlStatus.Background = Brushes.Gold;
            //pnlStatusTop.Background = Brushes.Goldenrod;

            this.pnlStatus.Visibility = System.Windows.Visibility.Visible;
            this.txtStatus.Text = errorMessage;



            this.CheckAndSetPosition();
        }

        private void RenderFinishWithError(string errorMessage)
        {
            this.waitCursor.Visibility = System.Windows.Visibility.Collapsed;
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            this.pnlChartContainer.Visibility = System.Windows.Visibility.Collapsed;

            //pnlStatus.Background = Brushes.Tomato;
            //pnlStatusTop.Background = Brushes.Red;

            this.pnlStatus.Visibility = System.Windows.Visibility.Visible;

            this.txtStatus.Text = errorMessage;

            this.CheckAndSetPosition();
        }

        private void RequestUpdateStatusMessage(string statusMessage)
        {
            //this.Dispatcher.BeginInvoke(new SetStatusDelegate(SetStatusMessage), statusMessage);
            this.SetStatusMessage(statusMessage);
        }

        private void SetStatusMessage(string statusMessage)
        {
            this.pnlStatus.Visibility = System.Windows.Visibility.Visible;
            this.txtStatus.Text = statusMessage;
        }



        private void GenerateXYChartControl()
        {
            this.dbp = new DashboardPanel();
            //dp.Title = "My Chart Title";
            this.dbp.Theme = Defaults.THEME;// "ArcticWhite";
            this.dbp.Style = this.Resources["DashboardPanelStyle1"] as Style;

            List<FrequencyResultData> frd = ((XYChartViewModel)this.DataContext).FrequencyTable;
            this.PieChart = new PieChart();
            if (this.chartTypeEnum == XYControlChartTypes.Pie)
            {
                this.PieChart.Name = "PieChart";
                this.PieChart.XPath = "IndependantValue";
                this.PieChart.ChartKind = PieChartKind.Pie2D;
                this.PieChart.Palette = Palette.GetPalette(Defaults.COLOR_PALETTE);
                this.PieChart.GlareCoverVisible = false;
                this.PieChart.MinWidth = Defaults.MIN_CHART_WIDTH; //700;
                this.PieChart.MaxWidth = Defaults.MAX_CHART_WIDTH; // 2000;
                //this.PieChart.Height = 500;
                //this.PieChart.Width = 1000;
                this.PieChart.LegendVisible = Defaults.SHOW_CHART_LEGEND; // false;
            }
            else
            {
                this.XYChartControlChart = new XYChart
                {
                    RenderTransform = scaleTransform,
                    MinWidth = Defaults.MIN_CHART_WIDTH, //800,
                    MaxWidth = Defaults.MAX_CHART_WIDTH, //2000,
                    //Height = 400,
                    //Width = 1000,
                    HighlightDataPointOnHover = true,
                    XPath = "IndependentValue",
                    SelectionVisualHint = ComponentArt.Silverlight.DataVisualization.Charting.SelectionVisualHint.InvertedColor,
                    Theme = Defaults.THEME, //"ArcticWhite",
                    Palette = Palette.GetPalette(Defaults.COLOR_PALETTE), // "VibrantC")     
                    EnableAnimation = true,
                    AnimationDuration = new TimeSpan(0, 0, 0, 4),
                    EnableDataPointPopup = true,
                    LegendVisible = Defaults.SHOW_CHART_LEGEND,
                    //LegendDock = ComponentArt.Silverlight.DataVisualization.Charting.Dock.Right,
                    CoordinatesPaddingPercentage = new Thickness(5, 5, 0, 0),
                    DefaultAxisAnnotationsVisible = true,
                    DefaultGridLinesVisible = true,
                    GlareCoverVisible = false,
                    AnimationOnLoad = false,
                    DefaultStripesVisible = true,
                    //UseDifferentBarColors = true
                    //Orientation = Orientation.Horizontal
                    //ThemeVariant = ThemeVariant.Standard// new ThemeVariant()    // Standard  
                };
            }

            //Legend ChartLegend = new Legend()
            //{
            //    // CornerRadius = new CornerRadius(10),
            //    BorderBrush = new SolidColorBrush(Color.FromArgb(255, 192, 207, 226)), //   "#FFc0cfe2", 
            //    //Margin = new Thickness(30, 0, 10, 0),
            //    FontFamily = new FontFamily("Segoe UI")

            //};

            AxisCoordinates XaxisCoordinates = new AxisCoordinates()
            {

                //  AxisId = "XAxis",
                //  Angle = 70,
                Margin = new Thickness(0, 5, 0, 0),
                //FontSize = 8.5,
                LabelGap = 1.0//,
                //  Coordinates = new NumericCoordinates()     //      DateTimeCoordinates()

            };

            ChartLabel chartLabel = new ChartLabel()
            {
                //Orientation = ChartLabelOrientation.Vertical
            };

            compositeSeries = new CompositeSeries()
            {
                // CompositionKind = ComponentArt.Silverlight.DataVisualization.Charting.CompositionKind.SideBySide
            };

            PieSeries pieSeries = new PieSeries();

            ChartLabel ychartlabel = new ChartLabel();

            switch (this.chartTypeEnum)
            {
                case XYControlChartTypes.StackedColumn:
                    this.XYChartControlChart.Orientation = Orientation.Vertical;
                    chartLabel.Orientation = ChartLabelOrientation.Horizontal;
                    this.ChartName = "StackedColumn";
                    this.textBlockX = new TextBlock()
                    {
                        Text = ((EwavColumn)this.cbxColumnXAxisField.SelectedItem).Name,
                        //Width = 402,
                        TextAlignment = System.Windows.TextAlignment.Center,
                        Padding = new Thickness(0, 10, 0, 10),
                        Margin = new Thickness(20, 0, 0, 0)
                    };

                    XaxisCoordinates.Angle = 70.0;
                    ychartlabel.Orientation = ComponentArt.Silverlight.DataVisualization.Charting.ChartLabelOrientation.Vertical;
                    if (((ComboBoxItem)this.cbxColumnAggregateFunc.SelectedItem).Content.ToString() == "Count")
                    {
                        compositeSeries.CompositionKind = ComponentArt.Silverlight.DataVisualization.Charting.CompositionKind.Stacked;
                    }
                    else
                    {
                        compositeSeries.CompositionKind = ComponentArt.Silverlight.DataVisualization.Charting.CompositionKind.Stacked100;
                    }

                    break;
                case XYControlChartTypes.Column:
                    this.ChartName = "Column";
                    this.XYChartControlChart.Orientation = Orientation.Vertical;
                    chartLabel.Orientation = ChartLabelOrientation.Horizontal;
                    this.textBlockX = new TextBlock()
                    {
                        Text = ((EwavColumn)this.cbxSingleField.SelectedItem).Name,
                        //Width = 402,
                        TextAlignment = System.Windows.TextAlignment.Center,
                        Padding = new Thickness(0, 10, 0, 10),
                        Margin = new Thickness(20, 0, 0, 0)
                    };

                    ychartlabel.Orientation = ComponentArt.Silverlight.DataVisualization.Charting.ChartLabelOrientation.Vertical;
                    XaxisCoordinates.Angle = 70.0;

                    compositeSeries.CompositionKind = ComponentArt.Silverlight.DataVisualization.Charting.CompositionKind.SideBySide;

                    break;
                case XYControlChartTypes.Line:
                    this.XYChartControlChart.Orientation = Orientation.Vertical;
                    this.ChartName = "Line";
                    chartLabel.Orientation = ChartLabelOrientation.Horizontal;
                    this.textBlockX = new TextBlock()
                    {
                        Text = ((EwavColumn)this.cbxSingleField.SelectedItem).Name,
                        //Width = 402,
                        TextAlignment = System.Windows.TextAlignment.Center,
                        Padding = new Thickness(0, 10, 0, 10),
                        Margin = new Thickness(20, 0, 0, 0)
                    };
                    ychartlabel.Orientation = ComponentArt.Silverlight.DataVisualization.Charting.ChartLabelOrientation.Vertical;
                    XaxisCoordinates.Angle = 70.0;
                    break;
                case XYControlChartTypes.Bar:
                    this.ChartName = "Bar";
                    this.XYChartControlChart.Orientation = Orientation.Horizontal;
                    chartLabel.Orientation = ChartLabelOrientation.Vertical;
                    this.textBlockX = new TextBlock()
                    {
                        Text = ((EwavColumn)this.cbxSingleField.SelectedItem).Name,
                        //Width = 402,
                        TextAlignment = System.Windows.TextAlignment.Center,
                        //  Padding = new Thickness(0, 10, 0, 10),
                        // Margin = new Thickness(20, 0, 0, 0)
                    };
                    ychartlabel.Orientation = ComponentArt.Silverlight.DataVisualization.Charting.ChartLabelOrientation.Horizontal;
                    compositeSeries.CompositionKind = ComponentArt.Silverlight.DataVisualization.Charting.CompositionKind.SideBySide;
                    break;
                case XYControlChartTypes.Pareto:
                    this.ChartName = "Pareto";
                    this.XYChartControlChart.Orientation = Orientation.Vertical;
                    chartLabel.Orientation = ChartLabelOrientation.Horizontal;
                    this.textBlockX = new TextBlock()
                    {
                        Text = ((EwavColumn)this.cbxSingleField.SelectedItem).Name,
                        //Width = 402,
                        TextAlignment = System.Windows.TextAlignment.Center,
                        Padding = new Thickness(0, 10, 0, 10),
                        Margin = new Thickness(20, 0, 0, 0)
                    };

                    ychartlabel.Orientation = ComponentArt.Silverlight.DataVisualization.Charting.ChartLabelOrientation.Vertical;

                    XaxisCoordinates.Angle = 70.0;

                    compositeSeries.CompositionKind = ComponentArt.Silverlight.DataVisualization.Charting.CompositionKind.SideBySide;

                    break;
                default:
                    break;
            }

            chartLabel.Child = this.textBlockX;

            this.textBlockY = new TextBlock()
            {
                Text = "Count",
                //Width = 402,
                VerticalAlignment = System.Windows.VerticalAlignment.Bottom,
                TextAlignment = System.Windows.TextAlignment.Center,

            };


            if (XAxisLabel != "")
                textBlockX.Text = XAxisLabel;
            if (YAxisLabel != "")
                textBlockY.Text = YAxisLabel;
            if (dbp != null)
                dbp.Title = ChartTitle;



            if (this.chartTypeEnum == XYControlChartTypes.Pie)
            {
            }
            else
            {
                this.XYChartControlChart.XAxisArea.Add(XaxisCoordinates);  //   axisCoordinates);
                this.XYChartControlChart.XAxisArea.Add(chartLabel);

                this.XYChartControlChart.YAxisArea.Add(new AxisCoordinates());

                ychartlabel.Child = this.textBlockY;

                this.XYChartControlChart.YAxisArea.Add(ychartlabel);
            }
            //  AxisCoordinates

            //string seriesTrackerText = "";
            //int seriesCounter = 0;

            //List<MyString> chartColsList = dtb.ColumnNameList.GetRange(1, dtb.ColumnNameList.Count - 1);

            string XYChartControlSeriesId, pieSeriesID;
            int sId = 0;
            double max = 0;

            foreach (FrequencyResultData frData in frd)
            {
                List<ChartDataValue> chartDataValuesList = new List<ChartDataValue>();

                List<EwavFrequencyControlDto> fcDtoList = frData.FrequencyControlDtoList;

                if (fcDtoList.Count < 1)
                {
                    this.RenderFinishWithError(SharedStrings.NO_RECORDS_SELECTED);

                    return;
                }

                if (fcDtoList[0].NameOfDtoList.Contains("Missing"))
                {
                    continue;
                }

                XYChartControlSeriesId = fcDtoList[0].NameOfDtoList;
                pieSeriesID = string.Format("S{0}", sId);
                sId++;
                double total = 0;
                double runningPercent = 0;


                foreach (EwavFrequencyControlDto fcd in fcDtoList)
                {
                    if (this.chartTypeEnum == XYControlChartTypes.Pareto)
                    {
                        max = max + fcd.FrequencyColumn.SafeParsetoDou();
                    }
                    total += Convert.ToDouble(fcd.FrequencyColumn);
                }


                foreach (EwavFrequencyControlDto fcd in fcDtoList)
                {
                    ChartDataValue data;
                    if (this.chartTypeEnum == XYControlChartTypes.Pie)
                    {
                        data = new ChartDataValue()
                        {
                            Format = "",
                            IndependentValue = string.Format("{0} - {1}", XYChartControlSeriesId, fcd.FreqVariable),
                            DependentValue = fcd.FrequencyColumn
                        };
                        //pieSeries.Label = data.IndependentValue;
                    }
                    else
                    {
                        data = new ChartDataValue()
                        {
                            Format = "",
                            IndependentValue = fcd.FreqVariable.ToString(),
                            DependentValue = fcd.FrequencyColumn
                        };
                    }

                    if (this.chartTypeEnum == XYControlChartTypes.Pareto)
                    {
                        // double 
                        data.currentMeanValue = (((data.DependentValue.SafeParsetoDou() / max) * 100) + runningPercent).ToString();
                        runningPercent = data.currentMeanValue.SafeParsetoDou();
                    }

                    if (data.DependentValue.Length == 0)
                    {
                        data.DependentValue = "0";
                    }
                    chartDataValuesList.Add(data);
                }

                switch (this.chartTypeEnum)
                {
                    case XYControlChartTypes.Pie:
                        pieSeries = new PieSeries();
                        //pieSeries.Id = pieSeriesID; //XYChartControlSeriesId;
                        pieSeries.Id = "S0";
                        pieSeries.YPath = "DependentValue";
                        pieSeries.XPath = "IndependentValue";
                        pieSeries.Texture = TextureKind.BrushedMetal;
                        pieSeries.DataSource = chartDataValuesList;
                        pieSeries.EnableDataPointPopup = true;
                        pieSeries.DataPointPopup = this.Resources["CustomDataPointPopup"] as DataPointPopup;
                        pieSeries.ShowPointAnnotations = true;
                        DataPointAnnotation dpa = new DataPointAnnotation();
                        dpa.RelativeX = 0.8;
                        dpa.RelativeY = 0.5;
                        dpa.RelativeRadialOffset = 0.3;
                        dpa.HorizontalOffset = 5;
                        dpa.Template = this.Resources["cdpa"] as DataTemplate;

                        dpa.LineStroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

                       // pieSeries.DataPointAnnotations.Add(dpa);

                        this.PieChart.PieChartMainArea.Add(pieSeries);

                        pieSeries.ShowPointAnnotations = true;
                        //this.PieChart.LegendVisible = true;

                        break;
                    case XYControlChartTypes.Line:
                        LineSeries XYChartControlLineSeries = new LineSeries();
                        //  LineSeries XYChartControlSeries = new LineSeries();

                        XYChartControlLineSeries.Id = XYChartControlSeriesId;
                        XYChartControlLineSeries.YPath = "DependentValue";
                        XYChartControlLineSeries.XPath = "IndependentValue";
                        XYChartControlLineSeries.Texture = TextureKind.BrushedMetal;
                        XYChartControlLineSeries.DataSource = chartDataValuesList;
                        //  XYChartControlChart.XYChartMainArea.Add(XYChartControlSeries);

                        //this.XYChartControlChart.Width = (chartDataValuesList.Count * 30) + 250;
                        compositeSeries.SubSeries.Add(XYChartControlLineSeries);

                        break;
                    case XYControlChartTypes.Pareto:
                        XYChartControlSeries = new BarSeries();
                        //  LineSeries XYChartControlSeries = new LineSeries();

                        XYChartControlSeries.Id = XYChartControlSeriesId;
                        XYChartControlSeries.YPath = "DependentValue";
                        XYChartControlSeries.XPath = "IndependentValue";
                        XYChartControlSeries.Texture = TextureKind.BrushedMetal;

                        XYChartControlSeries.DataSource = chartDataValuesList;

                        AxisCoordinates Y2AxisCoordinates = new AxisCoordinates()
                        {
                            AxisId = "PercentAxis",
                            Coordinates = new NumericCoordinates()
                            {
                                From = 0,
                                To = 100,
                                Step = 10
                            },
                        };

                        AxisCoordinates YAxisCoordinates = new AxisCoordinates();

                        LineSeries lineSeries = new LineSeries();

                        if (this.chartTypeEnum == XYControlChartTypes.Pareto)
                        {
                            lineSeries.Id = "pLineSeries";
                            lineSeries.YAxisId = "PercentAxis";
                            lineSeries.Label = "Accumulated %";
                            lineSeries.YPath = "CurrentMeanValue";
                            lineSeries.XPath = "IndependentValue";
                            lineSeries.Texture = TextureKind.BrushedMetal;
                            lineSeries.MarkerVisible = false;
                            lineSeries.DashStyle = LineDashStyle.Dash;

                            lineSeries.DataSource = chartDataValuesList;
                            // lineSeries.DataPointPopup = Resources["CustomDataPointPopupPareto"] as DataPointPopup;
                            this.XYChartControlChart.Y2AxisArea.Add(Y2AxisCoordinates);
                        }

                        //this.XYChartControlChart.Width = (chartDataValuesList.Count * 30) + 250;  // 25;
                        //this.XYChartControlChart.MaxHeight = 800;
                        //this.XYChartControlChart.MinWidth = 600;
                        compositeSeries.SubSeries.Add(XYChartControlSeries);
                        compositeSeries.SubSeries.Add(lineSeries);

                        break;
                    default:
                        XYChartControlSeries = new BarSeries();
                        //  LineSeries XYChartControlSeries = new LineSeries();

                        XYChartControlSeries.Id = XYChartControlSeriesId;
                        XYChartControlSeries.YPath = "DependentValue";
                        XYChartControlSeries.XPath = "IndependentValue";
                        XYChartControlSeries.Texture = TextureKind.BrushedMetal;
                        //  XYChartControlSeries.YAxisId = 
                        XYChartControlSeries.DataSource = chartDataValuesList;
                        //  XYChartControlChart.XYChartMainArea.Add(XYChartControlSeries);    

                        //if (this.chartTypeEnum == XYControlChartTypes.Bar)
                        //{
                        //    //this.XYChartControlChart.Width = 1000;
                        //    this.XYChartControlChart.MaxWidth = 800;
                        //    this.XYChartControlChart.Height = (chartDataValuesList.Count * 30) + 150;
                        //    this.XYChartControlChart.MinHeight = 400;
                        //}
                        //else
                        //{
                        //    this.XYChartControlChart.Width = (chartDataValuesList.Count * 30) + 150;
                        //}

                        compositeSeries.SubSeries.Add(XYChartControlSeries);

                        break;
                }
                //string datatemplate = "<DataTemplate> <TextBlock TextAlignment=\"Center\" Text=\"YES\" /> </DataTemplate>";
                //DataTemplate template = (DataTemplate)XamlReader.Load(datatemplate);
                //DataPointPopup dpp = new DataPointPopup();
                //dpp.IndexTemplate = template;
                //PieChart.DataPointPopup = dpp;
                RecordCount += Convert.ToInt64(total);
            }


            if (this.chartTypeEnum == XYControlChartTypes.Pie)
            {
                this.PieChart.EnableDataPointPopup = true;
                this.PieChart.DataPointPopup = this.Resources["CustomDataPointPopup"] as DataPointPopup;
                this.pnlChartContainer.Children.Clear();
                this.dbp.Content = this.PieChart;
            }
            else
            {
                this.XYChartControlChart.XYChartMainArea.Add(compositeSeries);

                this.pnlChartContainer.Children.Clear();
                //if (frd.Count > 1)
                //{
                //    XYChartControlChart.Legend = ChartLegend;
                //}

                this.dbp.Content = this.XYChartControlChart;
                //pnlChartContainer.Children.Add(dbp);
            }
            this.pnlChartContainer.Children.Add(this.dbp);
            this.tbChartName.Text = this.ChartName.FromCamelCase();
            this.RenderFinish();
        }


        private List<StringDataValue> ConvertToPct(List<StringDataValue> list)
        {
            List<StringDataValue> values = new List<StringDataValue>();
            foreach (StringDataValue value in list)
            {
                values.Add(new StringDataValue() { IndependentValue = value.IndependentValue, DependentValue = value.DependentValue / 31 * 100 });
            }
            return values;
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
            this.txtStatus.Text = string.Empty;
            this.pnlStatus.Visibility = Visibility.Collapsed;
            this.waitCursor.Visibility = Visibility.Visible;

            this.IsBooleanWithNoStratas = false;

            this.pnlChartContainer.Children.Clear();
            this.pnlChartContainer.Visibility = System.Windows.Visibility.Collapsed;
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

        }

        public StringBuilder HtmlBuilder { get; set; }

        public void RefreshResults()
        {
            try
            {
                this.waitCursor.Visibility = Visibility.Visible;
                this.pnlStatus.Visibility = Visibility.Collapsed;

                this.gadgetOptions = new GadgetParameters();

                this.gadgetOptions.MainVariableName = string.Empty;
                this.gadgetOptions.WeightVariableName = string.Empty;
                this.gadgetOptions.StrataVariableList = new List<MyString>();

                this.gadgetOptions.TableName = this.applicationViewModel.EwavSelectedDatasource.TableName;
                this.gadgetOptions.DatasourceName = this.applicationViewModel.EwavSelectedDatasource.DatasourceName;
                this.gadgetOptions.GadgetFilters = GadgetFilters;

                this.gadgetOptions.InputVariableList = new Dictionary<string, string>();
                this.gadgetOptions.ShouldUseAllPossibleValues = false;

                switch (this.chartTypeEnum)
                {
                    case XYControlChartTypes.StackedColumn:

                        if (this.cbxColumnXAxisField.SelectedIndex > -1)
                        {
                            this.gadgetOptions.MainVariableName = ((EwavColumn)this.cbxColumnXAxisField.SelectedItem).Name;
                        }
                        else
                        {
                            this.gadgetOptions.MainVariableName = null;
                        }

                        //      gadgetOptions.CrosstabVariableName = cbxStrataField;    
                        if (this.cbxColumnYAxisField.SelectedIndex > 0)
                        {
                            MyString mys = new MyString();
                            mys.VarName = ((EwavColumn)this.cbxColumnYAxisField.SelectedItem).Name;
                            this.gadgetOptions.StrataVariableList.Add(mys);
                        }
                        else
                        {
                            this.gadgetOptions.StrataVariableList.Clear();
                        }
                        break;
                    case XYControlChartTypes.Bar:
                    case XYControlChartTypes.Column:
                    case XYControlChartTypes.Line:
                    case XYControlChartTypes.Pie:
                        if (this.cbxChartSize.SelectedIndex > -1)
                        {
                            this.gadgetOptions.InputVariableList.Add("chartsize", this.cbxChartSize.SelectedValue.ToString());
                        }
                        // gadgetOptions.MainVariableName = ((EwavColumn)cbxSingleField.SelectedItem).Name;
                        //      gadgetOptions.CrosstabVariableName = cbxStrataField;    

                        if (this.cbxWeightField.SelectedItem != null)
                        {
                            this.gadgetOptions.WeightVariableName = ((EwavColumn)this.cbxWeightField.SelectedItem).Name;
                        }
                        else
                        {
                            this.gadgetOptions.WeightVariableName = null;
                        }

                        if (this.cbxSingleField.SelectedItem != null)
                        {
                            this.gadgetOptions.MainVariableName = ((EwavColumn)this.cbxSingleField.SelectedItem).Name.ToString();
                        }
                        else
                        {
                            this.gadgetOptions.MainVariableName = null;
                        }
                        if (this.cbxStrataField.SelectedIndex > 0)
                        {
                            MyString mys = new MyString();
                            mys.VarName = ((EwavColumn)this.cbxStrataField.SelectedItem).Name;
                            this.gadgetOptions.StrataVariableList.Add(mys);
                        }
                        else
                        {
                            this.gadgetOptions.StrataVariableList.Clear();
                        }
                        break;
                    case XYControlChartTypes.Pareto:

                        if (this.cbxChartSize.SelectedIndex > -1)
                        {
                            this.gadgetOptions.InputVariableList.Add("chartsize", this.cbxChartSize.SelectedValue.ToString());
                        }

                        if (this.cbxWeightField.SelectedItem != null)
                        {
                            this.gadgetOptions.WeightVariableName = ((EwavColumn)this.cbxWeightField.SelectedItem).Name;
                        }
                        else
                        {
                            this.gadgetOptions.WeightVariableName = null;
                        }

                        this.gadgetOptions.ShouldSortHighToLow = true;

                        if (this.cbxSingleField.SelectedItem != null)
                        {
                            this.gadgetOptions.MainVariableName = ((EwavColumn)this.cbxSingleField.SelectedItem).Name.ToString();
                        }
                        else
                        {
                            this.gadgetOptions.MainVariableName = null;
                        }
                        if (this.cbxStrataField.SelectedIndex > 0)
                        {
                            MyString mys = new MyString();
                            mys.VarName = ((EwavColumn)this.cbxStrataField.SelectedItem).Name;
                            this.gadgetOptions.StrataVariableList.Add(mys);
                        }
                        else
                        {
                            this.gadgetOptions.StrataVariableList.Clear();
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        /// <summary>
        /// Sets the gadget to its 'processing' state
        /// </summary>
        public void SetGadgetToProcessingState()
        {

            this.IsProcessing = true;
        }

        /// <summary>
        /// Sets the gadget to its 'finished' state
        /// </summary>
        public void SetGadgetToFinishedState()
        {
        }

        public string CustomOutputHeading { get; set; }

        public string CustomOutputDescription { get; set; }

        public string CustomOutputCaption { get; set; }

        public string ChartTitle { get; set; }

        public string LegendTitle { get; set; }

        public string XAxisLabel { get; set; }

        public string YAxisLabel { get; set; }

        #endregion

        private void DoChart()
        {
            if (!LoadingDropDowns && !LoadingCanvas)
            {
                // MEF patch 
                if (this.DataContext == null)
                {
                    // Only reply if chart data is loaded  
                    if (this.chartDataLoaded)
                    {
                        return;
                    }
                }

                if (ValidateComboBoxes())
                {
                    IsUserDefindVariableInUse();
                    this.RefreshResults();
                    xyChartViewModel = (XYChartViewModel)this.DataContext;
                    //RecordCount = 0;
                    this.gadgetOptions.TableName = this.applicationViewModel.EwavSelectedDatasource.TableName;
                    this.gadgetOptions.DatasourceName = this.applicationViewModel.EwavSelectedDatasource.DatasourceName;
                    this.gadgetOptions.UseAdvancedDataFilter = applicationViewModel.UseAdvancedFilter;
                    this.xyChartViewModel.GetFrequencyData(this.gadgetOptions);
                }
                this.gadgetExpander.IsExpanded = false;
            }
        }

        /// <summary>
        /// Use to verify if Defined Variables are in Use
        /// </summary>
        private void IsUserDefindVariableInUse()
        {
            Col1 = (cbxSingleField.SelectedIndex > -1) ? (EwavColumn)cbxSingleField.SelectedItem : null;
            Col2 = (cbxWeightField.SelectedIndex > -1) ? (EwavColumn)cbxWeightField.SelectedItem : null;
            Col3 = (cbxStrataField.SelectedIndex > -1) ? (EwavColumn)cbxStrataField.SelectedItem : null;
            Col4 = (cbxColumnXAxisField.SelectedIndex > -1) ? (EwavColumn)cbxColumnXAxisField.SelectedItem : null;
            Col5 = (cbxColumnYAxisField.SelectedIndex > -1) ? (EwavColumn)cbxColumnYAxisField.SelectedItem : null;
            if (Col1 != null && Col1.IsUserDefined == true)
            {
                Col1.IsInUse = true;
            }
            if (Col2 != null && Col2.IsUserDefined == true)
            {
                Col2.IsInUse = true;
            }
            if (Col3 != null && Col3.IsUserDefined == true)
            {
                Col3.IsInUse = true;
            }
            if (Col4 != null && Col4.IsUserDefined == true)
            {
                Col4.IsInUse = true;
            }
            if (Col5 != null && Col5.IsUserDefined == true)
            {
                Col5.IsInUse = true;
            }
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {

                

            XAxisLabel = YAxisLabel = "";
            ChartTitle = "";
            RecordCount = 0;
            DoChart();


        }

        private void EnableDisableButton()
        {
            if ((this.cbxColumnXAxisField.SelectedIndex > 0 && this.cbxColumnYAxisField.SelectedIndex > 0) ||
                this.cbxSingleField.SelectedIndex > 0)
            {
                this.btnRun.IsEnabled = true;
            }
            else
            {
                this.btnRun.IsEnabled = false;
            }
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
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.CloseGadgetOnClick();
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

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

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
                return "XYChartControl";
            }
        }

        public string MyUIName
        {
            get
            {
                return "XYChartControl Chart";
            }
        }

        public ApplicationViewModel ApplicationViewModel
        {
            get
            {
                return this.applicationViewModel;
            }
        }

        private void cbxField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.EnableDisableButton();
        }

        //XNode IGadget.Serialize(XDocument doc)
        //{
        //    throw new NotImplementedException();
        //}

        void IGadget.CreateFromXml(XElement element)
        {
            this.LoadingDropDowns = true;
            this.LoadingCanvas = true;
            viewModel = new SetLabelsViewModel();
            this.GadgetFilters = new List<EwavDataFilterCondition>();
            foreach (XElement child in element.Descendants())
            {
                switch (child.Name.ToString().ToLower())
                {

                    case "yaxisvariable":
                        //cbxColumnYAxisField.Text = child.InnerText.Replace("&lt;", "<");
                        cbxColumnYAxisField.SelectedIndex = cmnClass.FindIndexToSelect(cmnClass.GetItemsSource(GetPrimaryFieldColumnDataType, false), child.Value.Replace("&lt;", "<"));
                        break;
                    case "xaxisvariable":
                        cbxColumnXAxisField.SelectedIndex = cmnClass.FindIndexToSelect(cmnClass.GetItemsSource(GetPrimaryFieldColumnDataType), child.Value.Replace("&lt;", "<"));
                        break;

                    case "singlevariable":
                        cbxSingleField.SelectedIndex = cmnClass.FindIndexToSelect(cmnClass.GetItemsSource(GetPrimaryFieldColumnDataType), child.Value.Replace("&lt;", "<"));
                        break;
                    case "weightvariable":
                        cbxWeightField.SelectedIndex = cmnClass.FindIndexToSelect(cmnClass.GetItemsSource(GetNumericFieldColumnDataType), child.Value.Replace("&lt;", "<"));

                        break;
                    case "stratavariable":
                        cbxStrataField.SelectedIndex = cmnClass.FindIndexToSelect(cmnClass.GetItemsSource(GetPrimaryFieldColumnDataType), child.Value.Replace("&lt;", "<"));
                        break;

                    case "charttype":
                        this.ChartName = child.Value.ToString();
                        viewModel.GadgetName = this.ChartName; //charttype will always appear before chartName. Chartname will over write this value.
                        break;
                    case "customheading":

                    case "chartsize":
                        cbxChartSize.SelectedItem = child.Value.ToString();
                        break;
                    case "charttitle":
                        ChartTitle = child.Value.ToString();
                        break;
                    case "chartlegendtitle":
                        LegendTitle = child.Value.ToString();
                        break;
                    case "xaxislabel":
                        XAxisLabel = child.Value.ToString();
                        break;
                    case "yaxislabel":
                        YAxisLabel = child.Value.ToString();
                        break;
                    case "gadgetname":
                        viewModel.GadgetName = child.Value.ToString();
                        break;
                    case "gadgetdescription":
                        if (!string.IsNullOrEmpty(child.Value.ToString()))
                        {
                            byte[] encodedDataAsBytes = System.Convert.FromBase64String(child.Value.ToString());
                            viewModel.GadgetDescription =
                               System.Text.ASCIIEncoding.Unicode.GetString(encodedDataAsBytes);
                        }
                        else
                        {
                            viewModel.GadgetDescription = "";
                        }

                        break;
                    case "colorpalette":
                        if (!string.IsNullOrEmpty(child.Value.ToString()))
                        { viewModel.CollorPallet = child.Value.ToString(); }
                        else
                        {
                            viewModel.CollorPallet = Defaults.COLOR_PALETTE;
                        }

                        break;
                    case "usediffcolors":
                        if (!string.IsNullOrEmpty(child.Value.ToString()))
                        { viewModel.UseDifferentBarColors = Convert.ToBoolean(child.Value.ToString()); }
                        else
                        {
                            viewModel.UseDifferentBarColors = Defaults.USE_DIFFERENT_BAR_COLORS;
                        }

                        break;
                    case "spacesbetweenbars":
                        if (!string.IsNullOrEmpty(child.Value.ToString()))
                        { viewModel.SpacesBetweenBars = child.Value.ToString(); }
                        else
                        {
                            viewModel.SpacesBetweenBars = Defaults.SPACE_BETWEEN_BARS;
                        }

                        break;
                    case "showlegend":
                        if (!string.IsNullOrEmpty(child.Value.ToString()))
                        { viewModel.ShowLegend = Convert.ToBoolean(child.Value.ToString()); }
                        else
                        {
                            viewModel.ShowLegend = Defaults.SHOW_CHART_LEGEND;
                        }

                        break;
                    case "showvariablenames":
                        if (!string.IsNullOrEmpty(child.Value.ToString()))
                        { viewModel.ShowVariableNames = Convert.ToBoolean(child.Value.ToString()); }
                        else
                        {
                            viewModel.ShowVariableNames = Defaults.SHOW_CHART_VAR_NAMES;
                        }

                        break;
                    case "legendposition":
                        if (!string.IsNullOrEmpty(child.Value.ToString()))
                        { viewModel.LegendPostion = child.Value.ToString(); }
                        else
                        {
                            viewModel.LegendPostion = Defaults.LEGEND_POSITION;
                        }

                        break;
                    case "width":
                        if (!string.IsNullOrEmpty(child.Value.ToString()))
                        { viewModel.Width = Convert.ToDouble(child.Value.ToString()); }
                        else
                        {
                            viewModel.Width = Defaults.CHART_WIDTH;
                        }

                        break;
                    case "height":
                        if (!string.IsNullOrEmpty(child.Value.ToString()))
                        { viewModel.Height = Convert.ToDouble(child.Value.ToString()); }
                        else
                        {
                            viewModel.Height = Defaults.CHART_HEGHT;
                        }

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
            this.LoadingDropDowns = false;
            this.LoadingCanvas = false;
            double mouseVerticalPosition = 0.0, mouseHorizontalPosition = 0.0;
            foreach (XAttribute attribute in element.Attributes())
            {
                switch (attribute.Name.ToString().ToLower())
                {
                    case "top":
                        mouseVerticalPosition = double.Parse(attribute.Value, new CultureInfo(applicationViewModel.LoadedCanvasDto.Culture));
                        break;
                    case "left":
                        mouseHorizontalPosition = double.Parse(attribute.Value, new CultureInfo(applicationViewModel.LoadedCanvasDto.Culture));
                        break;
                }
            }

            ShowAppropriateChart();
            DoChart();
            cmnClass.AddControlToCanvas(this, mouseVerticalPosition, mouseHorizontalPosition, applicationViewModel.LayoutRoot);
            //CollapseExpandConfigPanel();
        }



        public XYControlChartTypes GetChartTypeEnum()
        {

            return chartTypeEnum;
        }


        void IEwavGadget.Reload()
        {


            DoChart();



        }

        private void HeaderButton_Click(object sender, RoutedEventArgs e)
        {
            //SetLabels sl = new SetLabels();
            //sl.GadgetName = this.ChartName;
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
                DoChart();
            }
        }


        
    }
}

namespace Ewav.Web.Services
{
    public partial class XYChartDomainContext
    {
        //This is the place to set RIA Services query timeout. 
        //TimeSpan(0, 5, 0) means five minutes vs the 
        //default 60 sec
        partial void OnCreated()
        {
            if (!DesignerProperties.GetIsInDesignMode(Application.Current.RootVisual))
            {
                ((WebDomainClient<IXYChartDomainServiceContract>)DomainClient).ChannelFactory.Endpoint.Binding.SendTimeout =
                    new TimeSpan(0, 120, 0);
            }
        }
    }
}