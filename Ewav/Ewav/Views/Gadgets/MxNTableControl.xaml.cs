/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MxNTableControl.xaml.cs
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
using System.Windows.Shapes;
using System.Xml.Linq;
using CommonLibrary;
using Ewav.Web.EpiDashboard;
using Ewav.BAL;
using Ewav.Client.Application;
using Ewav.ExtensionMethods;
using Ewav.ViewModels;
using Ewav.Web.Services;
using Ewav.Common;
using System.Text;

namespace Ewav
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(UserControl))]
    [ExportMetadata("type", "gadget")]
    [ExportMetadata("tabindex", "4")]
    public partial class MxNTableControl : UserControl, IEwavGadget, IGadget, ICustomizableGadget    
    {
        int Index1 = -1, Index2 = -1, Index3 = -1, Index4 = -1;
        EwavColumn Col1, Col2, Col3, Col4;
        private TwoxTwoAndMxNResultsSet resultsSet;

        private ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;

        private bool MxNDataLoaded = false;

        private bool loadingDropDowns = false;

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

        /// <summary>
        /// Container that holds gadget level filters.
        /// </summary>
        public List<EwavDataFilterCondition> GadgetFilters { get; set; }

        /// <summary>
        /// Gets or sets the is processing.
        /// </summary>
        /// <value>The is processing.</value>
        public bool IsProcessing { get; set; }

        public event ColumnsLoadedEventEventHandler ColumnsLoaded;

        #region Delegates

        private delegate void SetGridTextDelegate(string strataValue, TextBlockConfig textBlockConfig, FontWeight fontWeight);
        //    private delegate void AddFreqGridDelegate(string strataVar, string value, string crosstabVar, int columnCount);
        private delegate void RenderFrequencyHeaderDelegate(string strataValue, string freqVar, List<MyString> columns);
        private delegate void SetGridBarDelegate(string strataValue, int rowNumber, double pct);
        private delegate void AddGridRowDelegate(string strataValue, int height);
        private delegate void AddGridFooterDelegate(string strataValue, int rowNumber, int[] totalRows);
        private delegate void DrawFrequencyBordersDelegate(string strataValue);
        private delegate void SetStatusDelegate(string statusMessage);
        private delegate void RequestUpdateStatusDelegate(string statusMessage);
        private delegate bool CheckForCancellationDelegate();

        private delegate void RenderFinishWithErrorDelegate(string errorMessage);
        private delegate void RenderFinishWithWarningDelegate(string errorMessage);
        private delegate void SimpleCallback();

        #endregion

        /// <summary>
        /// Common Class reference
        /// </summary>
        ClientCommon.Common cmnClass;

        private RequestUpdateStatusDelegate requestUpdateStatus;
        private CheckForCancellationDelegate checkForCancellation;
        private List<TextBlock> gridLabelsList;
        private List<Grid> strataGridList;
        private List<Grid> strataConfGridList;
        private List<TextBlock> confHeadings;

        private bool triangleCollapsed;
        private bool advTriangleCollapsed;
        private bool columnWarningShown;
        private int rowCount = 1;
        private int columnCount = 1;
        private object syncLock = new object();
        private Dictionary<string, string> inputVariableList;

        private bool exposureIsDropDownList = false;
        private bool exposureIsCommentLegal = false;
        private bool exposureIsOptionField = false;
        private bool outcomeIsNumeric = false;

        //        private TwoxTwoAndMxNResultsSet resultsSet;     

        private struct TextBlockConfig
        {
            public string Text;
            public Thickness Margin;
            public VerticalAlignment VerticalAlignment;
            public HorizontalAlignment HorizontalAlignment;
            public int ColumnNumber;
            public int RowNumber;

            public TextBlockConfig(string text, Thickness margin, VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment, int rowNumber, int columnNumber)
            {
                this.Text = text;
                this.Margin = margin;
                this.VerticalAlignment = verticalAlignment;
                this.HorizontalAlignment = horizontalAlignment;
                this.RowNumber = rowNumber;
                this.ColumnNumber = columnNumber;
            }
        }

        StringLiterals sl = new StringLiterals();

        string freqVar;
        string weightVar;
        string strataVar;
        string crosstabVar;
        bool includeMissing;
        bool outcomeContinuous;
        int maxColumns;
        int maxRows;
        bool exceededMaxRows;
        bool exceededMaxColumns;




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


        public MxNTableControl()
        {
            // Required to initialize variables
            InitializeComponent();

            //this.Loaded += new RoutedEventHandler(MxNTableControl_Loaded);
            InitializeControl();
        }

        //void MxNTableControl_Loaded(object sender, RoutedEventArgs e)
        //{
        //    InitializeControl();
        //}

        private void InitializeControl()
        {
            cmnClass = new ClientCommon.Common();
            cbxExposureField.SelectionChanged += new SelectionChangedEventHandler(Field_SelectionChanged);
            cbxOutcomeField.SelectionChanged += new SelectionChangedEventHandler(Field_SelectionChanged);
            cbxFieldWeight.SelectionChanged += new SelectionChangedEventHandler(Field_SelectionChanged);
            cbxFieldStrata.SelectionChanged += new SelectionChangedEventHandler(Field_SelectionChanged);

            mxNViewModel = (MxNViewModel)this.DataContext;
            //mxNViewModel.ColumnsLoadedEvent +=
            //    new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(mxNViewModel_ColumnsLoadedEvent);
            mxNViewModel.FrequencyTableLoadedEvent +=
                new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(mxNViewModel_FrequencyTableLoadedEvent);

            mxNViewModel.SetupGadgetEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(mxNViewModel_SetupGadgetEvent);

            mxNViewModel.ErrorNotice += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(mxNViewModel_ErrorNotice);
            //  Query allColumns 
            //mxNViewModel.GetColumns(applicationViewModel.EwavDatasources[
            //    applicationViewModel.EwavDatasourceSelectedIndex].DatasourceName,
            //                         applicationViewModel.EwavDatasources[
            //    applicationViewModel.EwavDatasourceSelectedIndex].TableName);  

            applicationViewModel.ApplyDataFilterEvent += new ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);

            applicationViewModel.DefinedVariableAddedEvent += new Client.Application.DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);
            //    lrViewModel.ColumnsLoadedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(lrViewModel_ColumnsLoadedEvent);
            applicationViewModel.DefinedVariableInUseDeletedEvent += new Client.Application.DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableInUseDeletedEvent);
            applicationViewModel.DefinedVariableNotInUseDeletedEvent += new Client.Application.DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
            applicationViewModel.PreColumnChangedEvent += new Client.Application.PreColumnChangedEventHandler(applicationViewModel_PreColumnChangedEvent);
            applicationViewModel.UnloadedEvent += new UnloadedEventHandler(applicationViewModel_UnloadedEvent);
            //mxNViewModel_ColumnsLoadedEvent();
            FillComboboxes();
            Construct();
            //RenderFinish();
        }

        void applicationViewModel_UnloadedEvent(object o)
        {
            applicationViewModel.ApplyDataFilterEvent -= new ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);

            applicationViewModel.DefinedVariableAddedEvent -= new Client.Application.DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);
            applicationViewModel.DefinedVariableInUseDeletedEvent -= new Client.Application.DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableInUseDeletedEvent);
            applicationViewModel.DefinedVariableNotInUseDeletedEvent -= new Client.Application.DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
            applicationViewModel.PreColumnChangedEvent -= new Client.Application.PreColumnChangedEventHandler(applicationViewModel_PreColumnChangedEvent);
        }

        void mxNViewModel_ErrorNotice(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            if (this.DataContext != null)
            {
                //if (e.Data.Message.Length > 0)
                //{
                //    ChildWindow window = new ErrorWindow(e.Data);
                //    window.Show();
                //    //return;
                //}
                RenderFinishWithError(e.Data.Message);
                SetGadgetToFinishedState();
            }
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
            mxNViewModel = o as MxNViewModel;

            if (mxNViewModel != null && mxNViewModel.TwoxTwoAndMxNResultsSet != null)
            {
                resultsSet = mxNViewModel.TwoxTwoAndMxNResultsSet;
                DoMxN();
            }


        }

        void applicationViewModel_DefinedVariableInUseDeletedEvent(object o)
        {
            SearchIndex();
            if (IsDFUsedInThisGadget())
            {
                Index1 = Index2 = Index3 = Index4 = -1;
                HideMainPanel();
            }
            LoadingDropDowns = true;
            FillComboboxes();
            LoadingDropDowns = false;
        }

        private bool IsDFUsedInThisGadget()
        {
            return Col1 != null && Col1.Name == applicationViewModel.ItemToBeRemoved.Name ||
                            Col2 != null && Col2.Name == applicationViewModel.ItemToBeRemoved.Name ||
                            Col3 != null && Col3.Name == applicationViewModel.ItemToBeRemoved.Name ||
                            Col4 != null && Col4.Name == applicationViewModel.ItemToBeRemoved.Name;
        }

        private void HideMainPanel()
        {
            panelMain.Visibility = System.Windows.Visibility.Collapsed;
        }

        void applicationViewModel_DefinedVariableAddedEvent(object o)
        {
            SearchIndex();
            LoadingDropDowns = true;
            FillComboboxes();
            LoadingDropDowns = false;
            //mxNViewModel = o as MxNViewModel;
            if (mxNViewModel != null)
            {
                resultsSet = mxNViewModel.TwoxTwoAndMxNResultsSet;
                if (cbxExposureField.SelectedIndex > -1 && cbxOutcomeField.SelectedIndex > -1)
                {
                    RequestMxN();
                }
                //DoMxN();
            }
        }

        /// <summary>
        /// Saves the Values of Columns.
        /// </summary>
        private void SaveColumnValues()
        {
            Col1 = (EwavColumn)cbxExposureField.SelectedItem;
            Col2 = (EwavColumn)cbxOutcomeField.SelectedItem;
            Col3 = (EwavColumn)cbxFieldWeight.SelectedItem;
            Col4 = (EwavColumn)cbxFieldStrata.SelectedItem;
        }

        /// <summary>
        /// Searches current index of the columns.
        /// </summary>
        private void SearchIndex()
        {
            ClientCommon.Common CommonClass = new ClientCommon.Common();

            List<EwavColumn> ColsList = CommonClass.GetItemsSource(GetFieldPrimaryDataType);

            Index1 = CommonClass.SearchCurrentIndex(Col1, ColsList);  //SearchCurrentIndex(Col1);
            Index2 = CommonClass.SearchCurrentIndex(Col2, ColsList);
            Index4 = CommonClass.SearchCurrentIndex(Col4, ColsList);

            ColsList = CommonClass.GetItemsSource(GetFieldWeightDataType);


            Index3 = CommonClass.SearchCurrentIndex(Col3, ColsList);
        }

        /// <summary>
        /// Method used to locate the current index for selected column.
        /// </summary>
        /// <param name="Column"></param>
        /// <returns></returns>
        //private int SearchCurrentIndex(EwavColumn Column)
        //{
        //    if (Column != null)
        //    {
        //        List<EwavColumn> SourceColumns = this.applicationViewModel.EwavSelectedDatasource.AllColumns;

        //        IEnumerable<EwavColumn> CBXFieldCols = from cols in SourceColumns
        //                                               orderby cols.Name
        //                                               select cols;

        //        List<EwavColumn> colsList = CBXFieldCols.ToList();

        //        for (int i = 0; i < colsList.Count; i++)
        //        {
        //            if (Column.Name == colsList[i].Name)
        //            {
        //                return i + 1;
        //            }
        //        }
        //    }

        //    return 0;
        //}

        private void IsUserDefindVariableInUse()
        {
            EwavColumn Col1 = (cbxExposureField.SelectedIndex > -1) ? (EwavColumn)cbxExposureField.SelectedItem : null;
            EwavColumn Col2 = (cbxOutcomeField.SelectedIndex > -1) ? (EwavColumn)cbxOutcomeField.SelectedItem : null;
            EwavColumn Col4 = (cbxFieldStrata.SelectedIndex > -1) ? (EwavColumn)cbxFieldStrata.SelectedItem : null;
            if (Col1 != null && Col1.IsUserDefined == true)
            {
                Col1.IsInUse = true;
            }
            if (Col2 != null && Col2.IsUserDefined == true)
            {
                Col2.IsInUse = true;
            }
            if (Col4 != null && Col4.IsUserDefined == true)
            {
                Col4.IsInUse = true;
            }
        }

        void applicationViewModel_ApplyDataFilterEvent(object o)
        {
            //  Patch for MEF  
            //if (this.DataContext != null)
            //{
            //if (this.MxNDataLoaded)
            //{
            //    domxncount = 0;
            //    RequestMxN();
            //}
            if (applicationViewModel.RemoveIndicator &&
                           IsDFUsedInThisGadget())
            {
                ResetGadget();
            }
            else
            {
                domxncount = 0;
                RequestMxN();
            }
            //}
        }

        private void ResetGadget()
        {
            SearchIndex();
            if (IsDFUsedInThisGadget())
            {
                Index1 = Index2 = Index3 = Index4 = -1;
                panelMain.Visibility = System.Windows.Visibility.Collapsed;
                //pnlCrosstabContent.Visibility = System.Windows.Visibility.Collapsed;
                waitCursor.Visibility = System.Windows.Visibility.Collapsed;
            }
            LoadingDropDowns = true;
            FillComboboxes();
            LoadingDropDowns = false;
        }

        private void Construct()
        {
            inputVariableList = new Dictionary<string, string>();
            strataGridList = new List<Grid>();
            strataConfGridList = new List<Grid>();
            confHeadings = new List<TextBlock>();
            gridLabelsList = new List<TextBlock>();

            cbxExposureField.SelectionChanged += new SelectionChangedEventHandler(cbxField_SelectionChanged);
            cbxOutcomeField.SelectionChanged += new SelectionChangedEventHandler(cbxField_SelectionChanged);
            cbxFieldWeight.SelectionChanged += new SelectionChangedEventHandler(cbxField_SelectionChanged);
            cbxFieldStrata.SelectionChanged += new SelectionChangedEventHandler(cbxField_SelectionChanged);

            //AdvCollapsedTriangle.MouseLeftButtonUp += new MouseButtonEventHandler(AdvTriangle_MouseLeftButtonUp);
            //AdvExpandedTriangle.MouseLeftButtonUp += new MouseButtonEventHandler(AdvTriangle_MouseLeftButtonUp);
            //  grdAdvancedOptionsHeading.MouseLeftButtonUp += new MouseButtonEventHandler(AdvTriangle_MouseLeftButtonUp);

            //checkboxAllValues.Checked += new RoutedEventHandler(checkboxCheckChanged);
            //checkboxAllValues.Unchecked += new RoutedEventHandler(checkboxCheckChanged);

            //checkboxIncludeMissing.Checked += new RoutedEventHandler(checkboxCheckChanged);
            //checkboxIncludeMissing.Unchecked += new RoutedEventHandler(checkboxCheckChanged);

            //checkboxOutcomeContinuous.Checked += new RoutedEventHandler(checkboxCheckChanged);
            //checkboxOutcomeContinuous.Unchecked += new RoutedEventHandler(checkboxCheckChanged);

            //  columnWarningShown = false;

            gadgetOptions = new GadgetParameters();
            gadgetOptions.TableName = applicationViewModel.EwavDatasources[
                applicationViewModel.EwavDatasourceSelectedIndex].TableName;

            gadgetOptions.DatasourceName = applicationViewModel.EwavDatasources[
                applicationViewModel.EwavDatasourceSelectedIndex].DatasourceName;

            gadgetOptions.DatasourceName = applicationViewModel.EwavSelectedDatasource.DatasourceName;
            gadgetOptions.ShouldIncludeFullSummaryStatistics = false;
            gadgetOptions.ShouldIncludeMissing = false;
            gadgetOptions.ShouldSortHighToLow = false;
            gadgetOptions.ShouldUseAllPossibleValues = false;
            gadgetOptions.StrataVariableNames = new List<string>();
        }

        /// <summary>
        /// Handles the check / unchecked events
        /// </summary>
        /// <param name="sender">Object that fired the event</param>
        /// <param name="e">.NET supplied event parameters</param>
        //private void checkboxCheckChanged(object sender, RoutedEventArgs e)
        //{
        //    if (sender == checkboxOutcomeContinuous && checkboxOutcomeContinuous.IsChecked == true)
        //    {
        //        loadingCombos = true;
        //        checkboxIncludeMissing.IsChecked = false;
        //        loadingCombos = false;
        //    }
        //    else if (sender == checkboxIncludeMissing && checkboxIncludeMissing.IsChecked == true)
        //    {
        //        loadingCombos = true;
        //        checkboxOutcomeContinuous.IsChecked = false;
        //        loadingCombos = false;
        //    }

        //    if (cbxExposureField.SelectedIndex > -1 && cbxOutcomeField.SelectedIndex > -1)
        //    {
        //        domxncount = 0;
        //        RequestMxN();
        //    }
        //}

        /// <summary>
        /// Handles the SelectionChanged event of the cbxField control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void cbxField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: Implement this method
            //    throw new NotImplementedException();
        }

        #region  MVVM Events

        // TODO find a fix eventually for the second firing of the viewmodel event 
        long domxncount = 0;

        void mxNViewModel_SetupGadgetEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            mxNViewModel = sender as MxNViewModel;

            resultsSet = mxNViewModel.TwoxTwoAndMxNResultsSet;

            // TODO find a fix eventually for the second firing of the viewmodel event  
            //if (domxncount == 0)
            //{
            //    domxncount++;
                DoMxN();
            //}
            //Serialize(new XDocument());
        }

        public void mxNViewModel_ColumnsLoadedEvent()    //  object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            //    mxNViewModel = sender as MxNViewModel;
            FillComboboxes();

            RenderFinish();
        }

        void mxNViewModel_FrequencyTableLoadedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            //mxNViewModel = sender as MxNViewModel;
            //resultsSet = mxNViewModel.TwoxTwoAndMxNResultsSet;
            //DoMxN();
        }

        public List<ColumnDataType> GetFieldPrimaryDataType
        {
            get
            {
                List<ColumnDataType> columnDataType = new List<ColumnDataType>();
                columnDataType.Add(ColumnDataType.Boolean);
                columnDataType.Add(ColumnDataType.Numeric);
                columnDataType.Add(ColumnDataType.Text);
                columnDataType.Add(ColumnDataType.UserDefined);

                return columnDataType;
            }
        }

        public List<ColumnDataType> GetFieldWeightDataType
        {
            get
            {
                List<ColumnDataType> columnDataType = new List<ColumnDataType>();
                columnDataType.Add(ColumnDataType.Numeric);

                return columnDataType;
            }
        }

        private void FillComboboxes(bool update = false)
        {
            loadingCombos = true;

            object prevExposureField = string.Empty;
            string prevOutcomeField = string.Empty;

            cbxExposureField.ItemsSource = null;
            cbxExposureField.Items.Clear();

            cbxOutcomeField.ItemsSource = null;
            cbxOutcomeField.Items.Clear();

            cbxFieldStrata.ItemsSource = null;
            cbxFieldStrata.Items.Clear();

            cbxFieldWeight.ItemsSource = null;
            cbxFieldWeight.Items.Clear();

            //  TODO = filter for column data type    

            IEnumerable<EwavColumn> filteredCols = from cols in applicationViewModel.EwavSelectedDatasource.AllColumns
                                                   where GetFieldPrimaryDataType.Contains(cols.SqlDataTypeAsString)
                                                   orderby cols.Name
                                                   select cols;

            List<EwavColumn> exposureFields = filteredCols.ToList();
            EwavColumn ewc = new EwavColumn();
            ewc.Name = " ";
            exposureFields.Insert(0, ewc);

            // List<  > outcomeFields = dashboardHelper.GetFieldsAsList(columnDataType);
            //List<EwavColumn> outcomeFields = filteredCols.ToList();
            //outcomeFields.Insert(0, ewc);

            cbxExposureField.ItemsSource = exposureFields;
            cbxExposureField.SelectedValue = "Index";
            cbxExposureField.DisplayMemberPath = "Name";

            cbxOutcomeField.ItemsSource = exposureFields;
            cbxOutcomeField.SelectedValue = "Index";
            cbxOutcomeField.DisplayMemberPath = "Name";

            cbxFieldStrata.ItemsSource = exposureFields;
            cbxFieldStrata.SelectedValue = "Index";
            cbxFieldStrata.DisplayMemberPath = "Name";

            filteredCols = from cols in applicationViewModel.EwavSelectedDatasource.AllColumns
                           where GetFieldWeightDataType.Contains(cols.SqlDataTypeAsString)
                           orderby cols.Name
                           select cols;

            exposureFields = filteredCols.ToList();
            ewc = new EwavColumn();
            ewc.Name = " ";
            exposureFields.Insert(0, ewc);

            // List<  > outcomeFields = dashboardHelper.GetFieldsAsList(columnDataType);
            //outcomeFields = filteredCols.ToList();
            //outcomeFields.Insert(0, ewc);

            cbxFieldWeight.ItemsSource = exposureFields;
            cbxFieldWeight.SelectedValue = "Index";
            cbxFieldWeight.DisplayMemberPath = "Name";
            cbxFieldWeight.SelectedIndex = Index3;

            if (cbxExposureField.Items.Count > 0)
            {
                cbxExposureField.SelectedIndex = Index1;
            }
            if (cbxOutcomeField.Items.Count > 0)
            {
                cbxOutcomeField.SelectedIndex = Index2;
            }
            if (cbxFieldStrata.Items.Count > 0)
            {
                cbxFieldStrata.SelectedIndex = Index4;
            }

            if (update)
            {
                cbxExposureField.SelectedItem = prevExposureField;
                cbxOutcomeField.SelectedItem = prevOutcomeField;
            }

            loadingCombos = false;
        }

        #endregion

        #region       IEwavGadget

        /// <summary>
        /// The value for the frameworkelement.Name property
        /// </summary>
        /// <value></value>
        public string MyControlName
        {
            get
            {
                return "MxNTableControl";
            }
        }
        /// <summary>
        /// The value for the UI menus
        /// </summary>
        /// <value></value>
        public string MyUIName
        {
            get
            {
                return "M x N Table ";
            }
        }

        /// <summary>
        /// Generates Xml representation of this gadget
        /// </summary>
        /// <param name="doc">The Xml docment</param>
        /// <returns>XmlNode</returns>
        public XNode Serialize(XDocument doc)
        {
            Dictionary<string, string> inputVariableList = this.gadgetOptions.InputVariableList;

            string freqVar = string.Empty;
            string crosstabVar = string.Empty;
            string strataVar = string.Empty;
            string weightVar = string.Empty;
            string sort = string.Empty;
            string maxColumns = string.Empty;
            string maxRows = string.Empty;
            bool allValues = false;
            bool showConfLimits = true;
            bool showCumulativePercent = true;
            bool includeMissing = false;
            bool outcomeContinuous = false;
            //bool rowColPercents = (bool)checkboxRowColPercents.IsChecked;
            string layoutMode = "vertical";
            bool showStrataSummaryOnly = false;

            //if (checkboxHorizontal.IsChecked == true)
            //{
            //    layoutMode = "horizontal";
            //}

            if (inputVariableList != null)
            {
                if (inputVariableList.ContainsKey("freqvar"))
                {
                    freqVar = inputVariableList["freqvar"].Replace("<", "&lt;");
                }
                if (inputVariableList.ContainsKey("stratavar"))
                {
                    strataVar = inputVariableList["stratavar"].Replace("<", "&lt;");
                }
                if (inputVariableList.ContainsKey("weightvar"))
                {
                    weightVar = inputVariableList["weightvar"].Replace("<", "&lt;");
                }
                if (inputVariableList.ContainsKey("crosstabvar"))
                {
                    crosstabVar = inputVariableList["crosstabvar"].Replace("<", "&lt;");
                }
                if (inputVariableList.ContainsKey("sort"))
                {
                    sort = inputVariableList["sort"];
                }
                if (inputVariableList.ContainsKey("allvalues"))
                {
                    allValues = bool.Parse(inputVariableList["allvalues"]);
                }
                if (inputVariableList.ContainsKey("showconflimits"))
                {
                    showConfLimits = bool.Parse(inputVariableList["showconflimits"]);
                }
                if (inputVariableList.ContainsKey("showcumulativepercent"))
                {
                    showCumulativePercent = bool.Parse(inputVariableList["showcumulativepercent"]);
                }
                if (inputVariableList.ContainsKey("includemissing"))
                {
                    includeMissing = bool.Parse(inputVariableList["includemissing"]);
                }
                if (inputVariableList.ContainsKey("treatoutcomeascontinuous"))
                {
                    outcomeContinuous = bool.Parse(inputVariableList["treatoutcomeascontinuous"]);
                }
                if (inputVariableList.ContainsKey("stratasummaryonly"))
                {
                    showStrataSummaryOnly = bool.Parse(inputVariableList["stratasummaryonly"]);
                }
                if (inputVariableList.ContainsKey("maxcolumns"))
                {
                    maxColumns = inputVariableList["maxcolumns"];
                }
                if (inputVariableList.ContainsKey("maxrows"))
                {
                    maxRows = inputVariableList["maxrows"];
                }
            }


     

            XElement element = new XElement("gadget",
                new XAttribute("top", Canvas.GetTop(this).ToString("F0")),
                new XAttribute("left", Canvas.GetLeft(this).ToString("F0")),
                new XAttribute("collapsed", "false"),
                new XAttribute("gadgetType", "Ewav.MxNTableControl"),
                new XElement("mainVariable", freqVar),
                new XElement("crosstabVariable", crosstabVar),
                new XElement("strataVariable", strataVar),
                new XElement("weightVariable", weightVar),
                new XElement("maxColumnNameLength", MaxColumnLength.ToString()),
                new XElement("allValues", allValues),
                new XElement("showConfLimits", showConfLimits),
                new XElement("showCumulativePercent", showCumulativePercent),
                new XElement("includeMissing", includeMissing),
                new XElement("showStrataSummaryOnly", showStrataSummaryOnly),
                new XElement("treatOutcomeAsContinuous", outcomeContinuous),
                new XElement("maxColumns", maxColumns),
                new XElement("maxRows", maxRows),
                new XElement("gadgetTitle", GName.Text),
            new XElement("gadgetDescription", txtGadgetDescription.Text)
                
                );
            if (this.GadgetFilters != null)
            {
                this.GadgetFilters.Serialize(element);
            }
            
            return element;
        }

        #endregion

        #region  Simple MVVM

        public ApplicationViewModel ApplicationViewModel
        {
            get
            {
                return this.applicationViewModel;
            }
        }

        private MxNViewModel mxNViewModel;
        private bool loadingCombos;

        #endregion

        #region UI Events
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseGadgetOnClick();
        }

        private void LayoutRoot_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(cmnClass.GetParentObject<Grid>(this, "LayoutRoot"));
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).GadgetNameOnRightClick = MyControlName; //"FrequencyControl";
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).StrataList = this.strataGridList;
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).SelectedGadget = this;
            cmnClass.UpdateZOrder(this, true, cmnClass.GetParentObject<Grid>(this, "LayoutRoot"));
        }

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            domxncount = 0;

            if (cbxExposureField.SelectedIndex > -1 && cbxOutcomeField.SelectedIndex > -1)
            {
                domxncount = 0;
                RequestMxN();
            }
        }

        #endregion

        private int MaxColumns
        {
            get
            {
                int maxColumns = 50;
                bool success = int.TryParse(txtMaxColumns.Text, out maxColumns);
                if (!success)
                {
                    return 50;
                }
                else
                {
                    return maxColumns;
                }
            }
        }

        private int MaxColumnLength
        {
            get
            {
                int maxColumnLength = 24;
                bool success = int.TryParse(txtMaxColumnLength.Text, out maxColumnLength);
                if (!success)
                {
                    return 24;
                }
                else
                {
                    return maxColumnLength;
                }
            }
        }

        public T GetParentObject<T>(DependencyObject obj, string name = "") where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);

            while (parent != null)
            {
                if (parent is T && (((T)parent).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)parent;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        //private void DoMxN()
        //{

        //}

        private void Field_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void RequestMxN()
        {
            CreateInputVariableList();

            #region  Set up  GadgetParameters

            //  Dictionary<string, string> inputVariableList = ((GadgetParameters)e.Argument).InputVariableList;

            inputVariableList = gadgetOptions.InputVariableList;

            SetGadgetToProcessingState();
            ClearResults();

            freqVar = string.Empty;
            weightVar = string.Empty;
            strataVar = string.Empty;
            crosstabVar = string.Empty;
            includeMissing = false;
            outcomeContinuous = false;
            maxColumns = 20;
            maxRows = 200;
            exceededMaxRows = false;
            exceededMaxColumns = false;

            if (inputVariableList == null)
            {
                return;
            }

            if (inputVariableList.ContainsKey("freqvar"))
            {
                freqVar = inputVariableList["freqvar"];
            }

            if (inputVariableList.ContainsKey("crosstabvar"))
            {
                crosstabVar = inputVariableList["crosstabvar"];
            }

            if (inputVariableList.ContainsKey("weightvar"))
            {
                weightVar = inputVariableList["weightvar"];
            }

            if (inputVariableList.ContainsKey("stratavar"))
            {
                strataVar = inputVariableList["stratavar"];
            }

            if (inputVariableList.ContainsKey("includemissing"))
            {
                if (inputVariableList["includemissing"].Equals("true"))
                {
                    includeMissing = true;
                }
            }

            if (inputVariableList.ContainsKey("treatoutcomeascontinuous"))
            {
                if (inputVariableList["treatoutcomeascontinuous"].Equals("true"))
                {
                    outcomeContinuous = true;
                }
            }

            if (inputVariableList.ContainsKey("maxcolumns"))
            {
                maxColumns = int.Parse(inputVariableList["maxcolumns"]);
            }

            if (inputVariableList.ContainsKey("maxrows"))
            {
                maxRows = int.Parse(inputVariableList["maxrows"]);
            }

            List<string> stratas = new List<string>();
            if (!string.IsNullOrEmpty(strataVar))
            {
                stratas.Add(strataVar);
            }

            #endregion

            try
            {
                if (mxNViewModel == null && this.DataContext != null)
                {
                    mxNViewModel = (MxNViewModel)this.DataContext;
                }
                //  may have to change scope of the vars above this    
                //mxNViewModel.GetFrequencyData(ApplicationViewModel.Instance.SelectedDatasourceName,
                //                                    ApplicationViewModel.Instance.SelectedDatasourceTableName,
                //                                    gadgetOptions);    
                gadgetOptions.DatasourceName = applicationViewModel.EwavSelectedDatasource.DatasourceName;

                gadgetOptions.TableName = applicationViewModel.EwavSelectedDatasource.TableName;

                gadgetOptions.UseAdvancedDataFilter = applicationViewModel.UseAdvancedFilter;


                gadgetOptions.GadgetFilters = GadgetFilters;

                mxNViewModel.SetupGadget(gadgetOptions);
                //Dictionary<DataTable, List<DescriptiveStatistics>> stratifiedFrequencyTables =
                //    dashboardHelper.GenerateFrequencyTable(gadgetOptions/*, freqVar, weightVar, stratas, crosstabVar, useAllPossibleValues, sortHighLow, includeMissing, false*/);
            }
            catch (Exception)
            {
            }
        }

        GadgetParameters gadgetOptions;

        private void CreateInputVariableList()
        {
            Dictionary<string, string> inputVariableList = new Dictionary<string, string>();

            gadgetOptions = new GadgetParameters();

            gadgetOptions.TableName = applicationViewModel.EwavSelectedDatasource.TableName;
            gadgetOptions.DatasourceName = applicationViewModel.EwavSelectedDatasource.DatasourceName;

            gadgetOptions.MainVariableName = string.Empty;
            gadgetOptions.WeightVariableName = string.Empty;
            gadgetOptions.StrataVariableNames = new List<string>();
            gadgetOptions.CrosstabVariableName = string.Empty;

            if (cbxExposureField.SelectedIndex > -1 && cbxExposureField.SelectedItem != null)
            {
                inputVariableList.Add("freqvar", ((EwavColumn)cbxExposureField.SelectedItem).Name);
                gadgetOptions.MainVariableName = ((EwavColumn)cbxExposureField.SelectedItem).Name;
            }
            else
            {
                return;
            }

            EwavColumn ecolTest = (EwavColumn)cbxOutcomeField.SelectedItem;
            if (cbxOutcomeField.SelectedIndex > -1 && ecolTest != null)
            {
                inputVariableList.Add("crosstabvar", ((EwavColumn)cbxOutcomeField.SelectedItem).Name);
                gadgetOptions.CrosstabVariableName = ((EwavColumn)cbxOutcomeField.SelectedItem).Name;
            }

            ecolTest = (EwavColumn)cbxFieldWeight.SelectedItem;
            if (cbxFieldWeight.SelectedIndex > -1 && ecolTest != null && ecolTest.Name != sl.SPACE)
            {
                inputVariableList.Add("weightvar", ((EwavColumn)cbxFieldWeight.SelectedItem).Name);
                gadgetOptions.WeightVariableName = ((EwavColumn)cbxFieldWeight.SelectedItem).Name;
            }

            ecolTest = (EwavColumn)cbxFieldStrata.SelectedItem;
            MyString s = new MyString();
            if (cbxFieldStrata.SelectedIndex > -1 && ecolTest != null && ecolTest.Name != sl.SPACE)
            {
                s.VarName = ecolTest.Name;
                inputVariableList.Add("stratavar", ((EwavColumn)cbxFieldStrata.SelectedItem).Name);
                gadgetOptions.StrataVariableList = new List<MyString>();
                gadgetOptions.StrataVariableList.Add(s);
            }

            if (checkboxIncludeMissing.IsChecked == true)
            {
                inputVariableList.Add("includemissing", "true");
                gadgetOptions.ShouldIncludeMissing = true;
            }
            else
            {
                inputVariableList.Add("includemissing", "false");
                gadgetOptions.ShouldIncludeMissing = false;
            }

            if (checkboxOutcomeContinuous.IsChecked == true)
            {
                inputVariableList.Add("treatoutcomeascontinuous", "true");
            }
            else
            {
                inputVariableList.Add("treatoutcomeascontinuous", "false");
            }

            inputVariableList.Add("maxcolumns", MaxColumns.ToString());
            inputVariableList.Add("maxrows", MaxRows.ToString());

            gadgetOptions.ShouldIncludeFullSummaryStatistics = false;
            gadgetOptions.InputVariableList = inputVariableList;
        }


        private void DoMxN()
        {
            if (!LoadingDropDowns && !LoadingCanvas)
            {
                IsUserDefindVariableInUse();
                //   AddFreqGridDelegate addGrid = new AddFreqGridDelegate(AddFreqGrid);
                SetGridTextDelegate setText = new SetGridTextDelegate(SetGridText);
                AddGridRowDelegate addRow = new AddGridRowDelegate(AddGridRow);
                //   RenderFrequencyHeaderDelegate renderHeader = new RenderFrequencyHeaderDelegate(RenderFrequencyHeader);
                DrawFrequencyBordersDelegate drawBorders = new DrawFrequencyBordersDelegate(DrawFrequencyBorders);

                try
                {
                    List<DatatableBag> datatableBagList = null;
                    if (resultsSet != null)
                    {
                        datatableBagList = resultsSet.DatatableBagArray.ToList<DatatableBag>();
                    }

                    if (resultsSet == null || datatableBagList.Count == 0)
                    {
                        RenderFinishWithError(SharedStrings.NO_RECORDS_SELECTED);
                        SetGadgetToFinishedState();
                        return;
                    }
                    //else if (worker.CancellationPending)
                    //{
                    //    this.Dispatcher.BeginInvoke(new RenderFinishWithErrorDelegate(RenderFinishWithError), SharedStrings.DASHBOARD_GADGET_STATUS_OPERATION_CANCELLED);
                    //    this.Dispatcher.BeginInvoke(new SimpleCallback(SetGadgetToFinishedState));
                    //    Debug.Print("Thread cancelled");
                    //    return;
                    //    return;
                    //  }
                    else
                    {
                        string formatString = string.Empty;

                        foreach (DatatableBag datatableBag in datatableBagList)
                        {
                            string strataValue = datatableBag.TableName;

                            List<DescriptiveStatistics> descStats = datatableBag.DescriptiveStatisticsList;    //  resultsSet FreqResultsDescriptiveStatistics;

                            double count = 0;
                            foreach (DescriptiveStatistics ds in descStats)
                            {
                                count = count + ds.Observations;
                            }

                            if (count == 0 && datatableBagList.Count == 1)
                            {
                                // this is the only table and there are no records, so let the user know
                                RenderFinishWithError(SharedStrings.NO_RECORDS_SELECTED);
                                SetGadgetToFinishedState();
                                return;
                            }
                            else if (count == 0)
                            {
                                continue;
                            }

                            DatatableBag frequencies = datatableBag;    //        Datatable      

                            if (frequencies.RecordList.Count == 0)
                            {
                                continue;
                            }

                            //    outcomeContinuous = true;

                            if (outcomeContinuous)
                            {
                                if (frequencies.ColumnNameList.Count <= 128)
                                {
                                    int min;
                                    int max;
                                    if (int.TryParse(frequencies.GetValueAtRow(
                                        frequencies.ColumnNameList[1].VarName,
                                        frequencies.RecordList[1]),
                                        out min) &&
                                        int.TryParse(frequencies.ColumnNameList[frequencies.ColumnNameList.Count - 1].VarName,
                                            out max))
                                    {
                                        bool addedColumns = false;
                                        for (int i = min; i <= max; i++)
                                        {
                                            //  if (!frequencies.Columns.Contains(rule.ToString()))       
                                            if (!frequencies.GetColumnNamesAsList().Contains(i.ToString()))
                                            {
                                                frequencies.AddField(i.ToString());
                                                //EwavColumn newColumn = new EwavColumn();
                                                ////  new DataColumn(rule.ToString(), typeof(double));
                                                //newColumn.Name = rule.ToString();
                                                ////  newColumn.DefaultValue = 0;        
                                                //MyString colName = new MyString();
                                                //colName.VarName = newColumn.Name;
                                                //frequencies.ColumnNameList.Add(colName);    
                                                //frequencies.RecordList.
                                                //addedColumns = true;
                                            }
                                        }
                                        //if (addedColumns)
                                        //{
                                        //    int ordinal = 1;
                                        //    for (int rule = min; rule <= max; rule++)
                                        //    {
                                        //        if (frequencies.Columns.Contains(rule.ToString()))
                                        //        {
                                        //            frequencies.Columns[rule.ToString()].SetOrdinal(ordinal);
                                        //            ordinal++;
                                        //        }
                                        //    }
                                        //}
                                    }
                                }
                            }

                            AddFreqGrid(strataVar, frequencies.TableName, crosstabVar, frequencies.FieldsList.Fields.Count);
                        }

                        //if (GadgetStatusUpdate != null)
                        //{
                        //    this.Dispatcher.BeginInvoke(GadgetStatusUpdate, "Displaying output...");
                        //}

                        foreach (DatatableBag datatableBag in datatableBagList)
                        {
                            string strataValue = datatableBag.TableName;    //       tableKvp.Key.TableName;

                            double count = 0;
                            foreach (DescriptiveStatistics ds in datatableBag.DescriptiveStatisticsList)
                            {
                                count = count + ds.Observations;
                            }

                            if (count == 0)
                            {
                                continue;
                            }
                            DatatableBag frequencies = datatableBag;

                            if (frequencies.RecordList.Count == 0)
                            {
                                continue;
                            }

                            string tableHeading = frequencies.TableName;

                            if (datatableBagList.Count > 1)
                            {
                                tableHeading = freqVar;
                            }

                            RenderFrequencyHeader(strataValue, tableHeading, frequencies.FieldsList.Fields, frequencies.ColumnNameList);

                            rowCount = 1;

                            int[] totals = new int[frequencies.ColumnNameList.Count - 1];
                            columnCount = 1;

                            foreach (FieldsList row in frequencies.RecordList)
                            {
                                //if (7 == 7)  //    (!row[freqVar].Equals(DBNull.Value) || (row[freqVar].Equals(DBNull.Value) && includeMissing == true))
                                //{
                                addRow(strataValue, 30);
                                string displayValue = frequencies.GetValueAtRow(freqVar, row);  //     row[freqVar].ToString();

                                //if (dashboardHelper.IsUserDefinedColumn(freqVar))
                                //{
                                //    //    displayValue = dashboardHelper.GetFormattedOutput(freqVar, row[freqVar]);
                                //}
                                //else
                                //{
                                //    if (dashboardHelper.IsUsingEpiProject && View.Fields[freqVar] is YesNoField)
                                //    {
                                //        //if (row[freqVar].ToString().Equals("1"))
                                //        //    displayValue = "Yes";
                                //        //else if (row[freqVar].ToString().Equals("0"))
                                //        //    displayValue = "No";
                                //    }
                                //    else if (dashboardHelper.IsUsingEpiProject && View.Fields[freqVar] is DateField)

                                //    {
                                //        //   displayValue = string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:d}", row[freqVar]);
                                //    }
                                //    else if (dashboardHelper.IsUsingEpiProject && View.Fields[freqVar] is TimeField)
                                //    {
                                //        //    displayValue = string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:T}", row[freqVar]);
                                //    }
                                //    else
                                //    {
                                //        //    displayValue = dashboardHelper.GetFormattedOutput(freqVar, row[freqVar]);
                                //    }
                                //}

                                if (string.IsNullOrEmpty(displayValue))
                                {
                                    //Configuration config = dashboardHelper.Config;
                                    displayValue = "Missing";//config.Settings.RepresentationOfMissing;
                                }

                                setText(strataValue,
                                    new TextBlockConfig(
                                        string.Format("{0}{1}{2}", sl.SPACE, displayValue, sl.SPACE),
                                        new Thickness(2, 0, 2, 0),
                                        VerticalAlignment.Center,
                                        HorizontalAlignment.Left,
                                        rowCount, 0),
                                    FontWeights.Normal);

                                int rowTotal = 0;
                                columnCount = 1;

                                foreach (MyString column in frequencies.ColumnNameList)
                                {
                                    if (columnCount > maxColumns)
                                    {
                                        //this.Dispatcher.BeginInvoke(new ShowWarningDelegate(ShowWarning), (frequencies.Columns.Count - maxColumns).ToString() + " additional columns were not displayed due to gadget settings.");
                                        exceededMaxColumns = true;
                                        break;
                                    }

                                    if (column.VarName.Equals(freqVar))
                                    {
                                        continue;
                                    }

                                    string zz = datatableBag.GetValueAtRow(column.VarName, row);

                                    setText(strataValue,
                                        new TextBlockConfig(
                                            string.Format("{0}{1}{2}", sl.SPACE, zz, sl.SPACE),    //    row.Fields[column.VarName].ToString()     
                                            new Thickness(2, 0, 2, 0),
                                            VerticalAlignment.Center,
                                            HorizontalAlignment.Right,
                                            rowCount, columnCount),
                                        FontWeights.Normal);

                                    columnCount++;

                                    int rowValue = 0;
                                    bool success = int.TryParse(frequencies.GetValueAtRow(column.VarName, row), out rowValue);
                                    if (success)
                                    {
                                        totals[columnCount - 2] = totals[columnCount - 2] + rowValue;
                                        rowTotal = rowTotal + rowValue;
                                    }
                                }

                                setText(strataValue,
                                    new TextBlockConfig(
                                        string.Format("{0}{1}{2}", sl.SPACE, rowTotal.ToString(), sl.SPACE),
                                        new Thickness(2, 0, 2, 0),
                                        VerticalAlignment.Center,
                                        HorizontalAlignment.Right,
                                        rowCount, columnCount),
                                    FontWeights.Bold);

                                rowCount++;
                                //}

                                if (rowCount > maxRows)
                                {
                                    foreach (MyString column in frequencies.ColumnNameList)
                                    {
                                        if (columnCount > maxColumns)
                                        {
                                            exceededMaxColumns = true;
                                            break;
                                        }

                                        if (column.VarName.Equals(freqVar))
                                        {
                                            continue;
                                        }

                                        setText(strataValue,
                                            new TextBlockConfig(
                                                string.Format("{0}{1}{2}", sl.SPACE, sl.ELLIPSIS, sl.SPACE),
                                                new Thickness(2, 0, 2, 0),
                                                VerticalAlignment.Center,
                                                HorizontalAlignment.Right,
                                                rowCount, columnCount),
                                            FontWeights.Normal);

                                        columnCount++;
                                    }

                                    setText(strataValue,
                                        new TextBlockConfig(
                                            string.Format("{0}{1}{2}", sl.SPACE, sl.ELLIPSIS, sl.SPACE),
                                            new Thickness(2, 0, 2, 0),
                                            VerticalAlignment.Center, HorizontalAlignment.Right,
                                            rowCount, columnCount),
                                        FontWeights.Bold);

                                    rowCount++;
                                    exceededMaxRows = true;
                                    break;
                                }
                            }

                            RenderFrequencyFooter(strataValue, rowCount, totals);
                            drawBorders(strataValue);
                        }
                    }

                    if (exceededMaxRows && exceededMaxColumns)
                    {
                        RenderFinishWithWarning(string.Format("Warning: Some rows and columns were not displayed due to gadget settings. Showing top {0} rows and top {1} columns only.", maxRows.ToString(), maxColumns.ToString()));
                    }
                    else if (exceededMaxColumns)
                    {
                        RenderFinishWithWarning(string.Format("Warning: Some columns were not displayed due to gadget settings. Showing top {0} columns only.", maxColumns.ToString()));
                    }
                    else if (exceededMaxRows)
                    {
                        RenderFinishWithWarning(string.Format(SharedStrings.DASHBOARD_GADGET_STATUS_ROW_LIMIT, maxRows.ToString()));
                    }
                    else
                    {
                        RenderFinish();
                    }
                    SetGadgetToFinishedState();

                    // check for existence of 2x2 table...
                    if (rowCount == 3 && columnCount == 3)
                    {
                    }
                    //    stratifiedFrequencyTables.Clear();
                }
                catch (Exception ex)
                {
                    RenderFinishWithError(ex.Message);
                    SetGadgetToFinishedState();
                }
                finally
                {
                    // stopwatch.Stop();
                    //   Debug.Print("Crosstab gadget took " + stopwatch.Elapsed.ToString() + " seconds to complete with " + dashboardHelper.RecordCount.ToString() + " records and the following filters:");
                    //    Debug.Print(dashboardHelper.DataFilters.GenerateDataFilterString());
                }
            }
            this.gadgetExpander.IsExpanded = false;
        }

        /// <summary>
        /// Sends the gadget to the back of the canvas
        /// </summary>
        private void SendToBack()
        {
            Canvas.SetZIndex(this, -1);
        }

        private void RenderFinishWithWarning(string errorMessage)
        {
            waitCursor.Visibility = Visibility.Collapsed;

            foreach (Grid freqGrid in strataGridList)
            {
                freqGrid.Visibility = Visibility.Visible;
            }

            //pnlStatus.Background =       Brushes.Gold;
            pnlStatus.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 221)); //Light yellow;
            //pnlStatusTop.Background = Brushes.Goldenrod;
            pnlStatusTop.Background = new SolidColorBrush(Color.FromArgb(255, 250, 194, 112));//Light Orange;

            pnlStatus.Visibility = System.Windows.Visibility.Visible;
            txtStatus.Text = errorMessage;
            CollapseConfigPanel();
            CheckAndSetPosition();
        }

        /// <summary>
        /// Forces a collapse of the config panel
        /// </summary>
        private void CollapseConfigPanel()
        {
            //    ConfigCollapsedTriangle.Visibility = Visibility.Visible;
            //    ConfigExpandedTriangle.Visibility = Visibility.Collapsed;
            //    ConfigCollapsedTitle.Visibility = Visibility.Visible;
            //    ConfigGrid.Height = 50;
            triangleCollapsed = true;
        }

        private void RenderFinish()
        {
            waitCursor.Visibility = Visibility.Collapsed;
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            if (strataGridList != null)
            {
                foreach (Grid freqGrid in strataGridList)
                {
                    freqGrid.Visibility = Visibility.Visible;
                }

                foreach (Grid confGrid in strataConfGridList)
                {
                    confGrid.Visibility = Visibility.Visible;
                }

                foreach (TextBlock textBlock in confHeadings)
                {
                    textBlock.Visibility = Visibility.Collapsed;
                }
            }

            pnlStatus.Visibility = System.Windows.Visibility.Collapsed;
            txtStatus.Text = string.Empty;
            panelMain.Visibility = System.Windows.Visibility.Visible;
            FilterButton.IsEnabled = true;
            CheckAndSetPosition();
        }

        private void RenderFinishWithError(string errorMessage)
        {
            waitCursor.Visibility = Visibility.Collapsed;

            //pnlStatus.Background = Brushes.Tomato;
            //pnlStatusTop.Background = Brushes.Red;

            pnlStatus.Background = new SolidColorBrush(Color.FromArgb(255, 248, 215, 226)); //Brushes.Tomato;
            pnlStatusTop.Background = new SolidColorBrush(Color.FromArgb(255, 228, 101, 142)); //Brushes.Red;    
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            pnlStatus.Visibility = System.Windows.Visibility.Visible;
            txtStatus.Text = errorMessage;
            CollapseConfigPanel();
            CheckAndSetPosition();
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
        /// Sets the gadget to its 'processing' state
        /// </summary>
        public void SetGadgetToProcessingState()
        {
            this.IsProcessing = true;
            this.cbxExposureField.IsEnabled = false;
            this.cbxOutcomeField.IsEnabled = false;
            this.cbxFieldStrata.IsEnabled = false;
            this.cbxFieldWeight.IsEnabled = false;
            this.txtMaxColumnLength.IsEnabled = false;
            this.txtMaxColumns.IsEnabled = false;
            this.txtMaxRows.IsEnabled = false;
            //    this.checkboxAllValues.IsEnabled = false;
            this.checkboxIncludeMissing.IsEnabled = false;
            //     this.checkboxCommentLegalLabels.IsEnabled = false;
            this.checkboxOutcomeContinuous.IsEnabled = false;
        }

        /// <summary>
        /// Sets the gadget to its 'finished' state
        /// </summary>
        public void SetGadgetToFinishedState()
        {
            this.IsProcessing = false;
            this.cbxExposureField.IsEnabled = true;
            this.cbxOutcomeField.IsEnabled = true;
            this.cbxFieldStrata.IsEnabled = true;
            this.cbxFieldWeight.IsEnabled = true;
            this.txtMaxColumnLength.IsEnabled = true;
            this.txtMaxColumns.IsEnabled = true;
            this.txtMaxRows.IsEnabled = true;
            this.checkboxIncludeMissing.IsEnabled = true;
        }

        public void ClearResults()
        {
            txtStatus.Text = string.Empty;
            pnlStatus.Visibility = Visibility.Collapsed;
            waitCursor.Visibility = Visibility.Visible;
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            {
                foreach (Grid grid in strataGridList)
                {
                    grid.Children.Clear();
                    grid.RowDefinitions.Clear();
                    panelMain.Children.Remove(grid);
                }
            }

            foreach (Grid grid in strataConfGridList)
            {
                grid.Children.Clear();
                grid.RowDefinitions.Clear();
                panelMain.Children.Remove(grid);
            }

            foreach (TextBlock textBlock in confHeadings)
            {
                panelMain.Children.Remove(textBlock);
            }

            foreach (TextBlock textBlock in gridLabelsList)
            {
                panelMain.Children.Remove(textBlock);
            }

            strataGridList.Clear();
            strataConfGridList.Clear();
            confHeadings.Clear();

            grdFreq.Children.Clear();
            grdFreq.RowDefinitions.Clear();
            grdConf.Children.Clear();
            grdConf.RowDefinitions.Clear();
        }

        /// <summary>
        /// Converts the gadget's output to Html
        /// </summary>
        /// <returns></returns>
        //public string ToHTML(string htmlFileName = "", int count = 0)
        //{
        //    StringBuilder htmlBuilder = new StringBuilder();

        //    htmlBuilder.AppendLine("<h2 class=\"gadgetHeading\">Crosstabulation</h2>");

        //    htmlBuilder.AppendLine("<p class=\"gadgetOptions\"><small>");
        //    htmlBuilder.AppendLine("<em>Main variable:</em> <strong>" + cbxExposureField.Text + "</strong>");
        //    htmlBuilder.AppendLine("<br />");

        //    htmlBuilder.AppendLine("<em>Crosstab variable:</em> <strong>" + cbxOutcomeField.Text + "</strong>");
        //    htmlBuilder.AppendLine("<br />");

        //    if (cbxFieldWeight.SelectedIndex >= 0)
        //    {
        //        htmlBuilder.AppendLine("<em>Weight variable:</em> <strong>" + cbxFieldWeight.Text + "</strong>");
        //        htmlBuilder.AppendLine("<br />");
        //    }
        //    if (cbxFieldStrata.SelectedIndex >= 0)
        //    {
        //        htmlBuilder.AppendLine("<em>Strata variable:</em> <strong>" + cbxFieldStrata.Text + "</strong>");
        //        htmlBuilder.AppendLine("<br />");
        //    }

        //    htmlBuilder.AppendLine("<em>Include missing:</em> <strong>" + checkboxIncludeMissing.IsChecked.ToString() + "</strong>");
        //    htmlBuilder.AppendLine("<br />");
        //    htmlBuilder.AppendLine("</small></p>");

        //    if (!string.IsNullOrEmpty(txtStatus.Text) && pnlStatus.Visibility == Visibility.Visible)
        //    {
        //        htmlBuilder.AppendLine("<p><small><strong>" + txtStatus.Text + "</strong></small></p>");
        //    }

        //    foreach (Grid grid in this.strataGridList)
        //    {
        //        string gridName = grid.Tag.ToString();

        //        htmlBuilder.AppendLine("<div style=\"height: 7px;\"></div>");
        //        htmlBuilder.AppendLine("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">");
        //        htmlBuilder.AppendLine("<caption>" + cbxExposureField.Text + " * " + cbxOutcomeField.Text + "</caption>");

        //        foreach (UIElement control in grid.Children)
        //        {
        //            if (control is TextBlock)
        //            {
        //                int rowNumber = Grid.GetRow(control);
        //                int columnNumber = Grid.GetColumn(control);

        //                string tableDataTagOpen = "<td>";
        //                string tableDataTagClose = "</td>";

        //                if (rowNumber == 0)
        //                {
        //                    tableDataTagOpen = "<th>";
        //                    tableDataTagClose = "</th>";
        //                }

        //                if (columnNumber == 0)
        //                {
        //                    htmlBuilder.AppendLine("<tr>");
        //                }
        //                if (columnNumber == 0 && rowNumber > 0)
        //                {
        //                    tableDataTagOpen = "<td class=\"value\">";
        //                }

        //                string value = ((TextBlock)control).Text;
        //                string formattedValue = value;

        //                if ((rowNumber == grid.RowDefinitions.Count - 1) || (columnNumber == grid.ColumnDefinitions.Count - 1))
        //                {
        //                    formattedValue = "<span class=\"total\">" + value + "</span>";
        //                }

        //                htmlBuilder.AppendLine(tableDataTagOpen + formattedValue + tableDataTagClose);

        //                if (columnNumber >= grid.ColumnDefinitions.Count - 1)
        //                {
        //                    htmlBuilder.AppendLine("</tr>");
        //                }
        //            }
        //        }

        //        htmlBuilder.AppendLine("</table>");
        //    }

        //    return htmlBuilder.ToString();
        //}


        //public string ToHTML(string htmlFileName = "", int count = 0)
        //{
        //    ClientCommon.Common cmnClass = new ClientCommon.Common();
        //    StringBuilder htmlBuilder = new StringBuilder();
        //    Grid grdTable = null;
        //    foreach (var item in panelMain.Children)
        //    {
        //        if (item is Grid && ((Grid)item).Name == "grdContent")
        //        {
        //            grdTable = (Grid)item;
        //        }
        //    }

        //    try
        //    {
        //        //string fileName = string.Format("{0}{1}.html", System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N"));//GetHTMLLineListing();

        //        //System.IO.FileStream stream = System.IO.File.OpenWrite(fileName);
        //        //System.IO.StreamWriter sw = new System.IO.StreamWriter(stream);

        //        htmlBuilder.AppendLine("<html><head><title>MxN Table</title>");
        //        htmlBuilder.AppendLine("<meta http-equiv=\"content-type\" content=\"text/html;charset=UTF-8\" />");
        //        htmlBuilder.AppendLine("<meta name=\"author\" content=\"Epi Info 7\" />");
        //        htmlBuilder.AppendLine(cmnClass.GenerateStandardHTMLStyle());
        //        //sb.AppendLine(this.ToHTML());
        //        //sw.Close();
        //        //sw.Dispose();
        //        //if (!string.IsNullOrEmpty(fileName))
        //        //{
        //        //    System.Diagnostics.Process proc = new System.Diagnostics.Process();
        //        //    proc.StartInfo.FileName = fileName;
        //        //    proc.StartInfo.UseShellExecute = true;
        //        //    proc.Start();
        //        //}


        //        if (string.IsNullOrEmpty(CustomOutputHeading))
        //        {
        //            htmlBuilder.AppendLine("<h2 class=\"gadgetHeading\">MxN Table</h2>");
        //        }
        //        else if (CustomOutputHeading != "(none)")
        //        {
        //            htmlBuilder.AppendLine("<h2 class=\"gadgetHeading\">" + CustomOutputHeading + "</h2>");
        //        }

        //        htmlBuilder.AppendLine("<p class=\"gadgetOptions\"><small>");
        //        htmlBuilder.AppendLine("<em>Exposure variable:</em> <strong>" + ((EwavColumn)cbxExposureField.SelectedItem).Name + "</strong>");
        //        htmlBuilder.AppendLine("<br />");

        //        if (cbxOutcomeField.SelectedIndex >= 0)
        //        {
        //            htmlBuilder.AppendLine("<em>Crosstab variable:</em> <strong>" + ((EwavColumn)cbxOutcomeField.SelectedItem).Name + "</strong>");
        //            htmlBuilder.AppendLine("<br />");
        //        }

        //        htmlBuilder.AppendLine("</small></p>");

        //        if (!string.IsNullOrEmpty(txtStatus.Text) && pnlStatus.Visibility == Visibility.Visible)
        //        {
        //            htmlBuilder.AppendLine("<p><small><strong>" + txtStatus.Text + "</strong></small></p>");
        //        }

        //        if (!string.IsNullOrEmpty(CustomOutputDescription))
        //        {
        //            htmlBuilder.AppendLine("<p class=\"gadgetsummary\">" + CustomOutputDescription + "</p>");
        //        }

        //        htmlBuilder.AppendLine("<div style=\"height: 7px;\"></div>");
        //        htmlBuilder.AppendLine("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">");
        //        htmlBuilder.AppendLine("<caption>" + ((EwavColumn)cbxExposureField.SelectedItem).Name + " * " + ((EwavColumn)cbxOutcomeField.SelectedItem).Name + "</caption>");

        //        foreach (UIElement control in grdTable.Children)
        //        {
        //            if (control is TextBlock)
        //            {
        //                int rowNumber = Grid.GetRow((FrameworkElement)control);
        //                int columnNumber = Grid.GetColumn((FrameworkElement)control);

        //                string tableDataTagOpen = "<td>";
        //                string tableDataTagClose = "</td>";

        //                if (rowNumber == 0)
        //                {
        //                    tableDataTagOpen = "<th>";
        //                    tableDataTagClose = "</th>";
        //                }

        //                if (columnNumber == 0)
        //                {
        //                    htmlBuilder.AppendLine("<tr>");
        //                }
        //                if (columnNumber == 0 && rowNumber > 0)
        //                {
        //                    tableDataTagOpen = "<td class=\"value\">";
        //                }

        //                string value = ((TextBlock)control).Text;
        //                string formattedValue = value;

        //                if ((rowNumber == grdTable.RowDefinitions.Count - 1) || (columnNumber == grdTable.ColumnDefinitions.Count - 1))
        //                {
        //                    formattedValue = "<span class=\"total\">" + value + "</span>";
        //                }

        //                htmlBuilder.AppendLine(tableDataTagOpen + formattedValue + tableDataTagClose);

        //                if (columnNumber >= grdTable.ColumnDefinitions.Count - 1)
        //                {
        //                    htmlBuilder.AppendLine("</tr>");
        //                }
        //            }

        //        }

        //        //return htmlBuilder.ToString();

        //    }
        //    finally
        //    {
        //    }

        //    System.Windows.Browser.HtmlPage.Window.Invoke("DisplayFormattedText", htmlBuilder.ToString());
        //    return "";
        //}

        /// <summary>
        /// Converts the gadget's output to Html
        /// </summary>
        /// <returns></returns>
        public string ToHTML(bool ForDash = false, string htmlFileName = "", int count = 0)
        {
            StringBuilder htmlBuilder = new StringBuilder();

            htmlBuilder.AppendLine("<html><head><title>MxN Table</title>");
            htmlBuilder.AppendLine("<meta http-equiv=\"content-type\" content=\"text/html;charset=UTF-8\" />");
            htmlBuilder.AppendLine("<meta name=\"author\" content=\"Epi Info 7\" />");
            htmlBuilder.AppendLine(cmnClass.GenerateStandardHTMLStyle());

            //CustomOutputHeading = headerPanel.Text;
            //CustomOutputDescription = descriptionPanel.Text;

            if (CustomOutputHeading == null || (string.IsNullOrEmpty(CustomOutputHeading) && !CustomOutputHeading.Equals("(none)")))
            {
                htmlBuilder.AppendLine("<h2 class=\"gadgetHeading\">Crosstabulation</h2>");
            }
            else if (CustomOutputHeading != "(none)")
            {
                htmlBuilder.AppendLine("<h2 class=\"gadgetHeading\">" + CustomOutputHeading + "</h2>");
            }

            htmlBuilder.AppendLine("<p class=\"gadgetOptions\"><small>");
            htmlBuilder.AppendLine("<em>Main variable:</em> <strong>" + ((EwavColumn)cbxExposureField.SelectedItem).Name + "</strong>");
            htmlBuilder.AppendLine("<br />");

            htmlBuilder.AppendLine("<em>Crosstab variable:</em> <strong>" + ((EwavColumn)cbxOutcomeField.SelectedItem).Name + "</strong>");
            htmlBuilder.AppendLine("<br />");

            if (cbxFieldWeight.SelectedIndex >= 0)
            {
                htmlBuilder.AppendLine("<em>Weight variable:</em> <strong>" + ((EwavColumn)cbxFieldWeight.SelectedItem).Name + "</strong>");
                htmlBuilder.AppendLine("<br />");
            }
            //if (cbxFieldStrata.SelectedIndex >= 0)
            //{
            //    htmlBuilder.AppendLine("<em>Strata variable:</em> <strong>" + cbxFieldStrata.Text + "</strong>");
            //    htmlBuilder.AppendLine("<br />");
            //}
            //if (lbxFieldStrata.SelectedItems.Count > 0)
            //{
            //    WordBuilder wb = new WordBuilder(", ");
            //    foreach (string s in lbxFieldStrata.SelectedItems)
            //    {
            //        wb.Add(s);
            //    }
            //    htmlBuilder.AppendLine("<em>Strata variable:</em> <strong>" + wb.ToString() + "</strong>");
            //    htmlBuilder.AppendLine("<br />");
            //}

            htmlBuilder.AppendLine("<em>Include missing:</em> <strong>" + checkboxIncludeMissing.IsChecked.ToString() + "</strong>");
            htmlBuilder.AppendLine("<br />");
            htmlBuilder.AppendLine("</small></p>");

            if (!string.IsNullOrEmpty(CustomOutputDescription))
            {
                htmlBuilder.AppendLine("<p class=\"gadgetsummary\">" + CustomOutputDescription + "</p>");
            }

            //if (!string.IsNullOrEmpty(messagePanel.Text) && messagePanel.Visibility == Visibility.Visible)
            //{
            //    htmlBuilder.AppendLine("<p><small><strong>" + messagePanel.Text + "</strong></small></p>");
            //}

            //if (!string.IsNullOrEmpty(infoPanel.Text) && infoPanel.Visibility == Visibility.Visible)
            //{
            //    htmlBuilder.AppendLine("<p><small><strong>" + infoPanel.Text + "</strong></small></p>");
            //}

            foreach (Grid grid in this.strataGridList)
            {
                string gridName = grid.Tag.ToString();

                htmlBuilder.AppendLine("<div style=\"height: 7px;\"></div>");
                htmlBuilder.AppendLine("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">");
                htmlBuilder.AppendLine("<caption>" + //((EwavColumn)cbxExposureField.SelectedItem).Name.ToString()); //" * " + 
                    ((EwavColumn)cbxOutcomeField.SelectedItem).Name);

                if (cbxFieldStrata.SelectedIndex > 0)
                {
                    htmlBuilder.AppendLine(", " + gridName);
                }

                htmlBuilder.AppendLine("</caption>");

                foreach (UIElement control in grid.Children)
                {
                    string value = string.Empty;
                    int rowNumber = -1;
                    int columnNumber = -1;
                    if (control is TextBlock || control is StackPanel)
                    {
                        if (control is TextBlock)
                        {
                            rowNumber = Grid.GetRow((FrameworkElement)control);
                            columnNumber = Grid.GetColumn((FrameworkElement)control);
                            value = ((TextBlock)control).Text;
                        }
                        else if (control is StackPanel)
                        {
                            rowNumber = Grid.GetRow((FrameworkElement)control);
                            columnNumber = Grid.GetColumn((FrameworkElement)control);
                            value = (((control as StackPanel).Children[0]) as TextBlock).Text;
                        }

                        string tableDataTagOpen = "<td>";
                        string tableDataTagClose = "</td>";

                        if (rowNumber == 0)
                        {
                            tableDataTagOpen = "<th>";
                            tableDataTagClose = "</th>";
                        }

                        if (columnNumber == 0)
                        {
                            htmlBuilder.AppendLine("<tr>");
                        }
                        if (columnNumber == 0 && rowNumber > 0)
                        {
                            tableDataTagOpen = "<td class=\"value\">";
                        }

                        string formattedValue = value;

                        if ((rowNumber == grid.RowDefinitions.Count - 1) || (columnNumber == grid.ColumnDefinitions.Count - 1))
                        {
                            formattedValue = "<span class=\"total\">" + value + "</span>";
                        }

                        htmlBuilder.AppendLine(tableDataTagOpen + formattedValue + tableDataTagClose);

                        if (columnNumber >= grid.ColumnDefinitions.Count - 1)
                        {
                            htmlBuilder.AppendLine("</tr>");
                        }
                    }
                }

                htmlBuilder.AppendLine("</table>");

                // Chi Square

                //Grid chiSquareGrid = GetStrataChiSquareGrid(grid.Tag.ToString());
                //htmlBuilder.AppendLine("<p></p>");
                //htmlBuilder.AppendLine("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">");

                //foreach (UIElement control in chiSquareGrid.Children)
                //{
                //    if (control is TextBlock)
                //    {
                //        int rowNumber = Grid.GetRow((FrameworkElement)control);
                //        int columnNumber = Grid.GetColumn((FrameworkElement)control);

                //        string tableDataTagOpen = "<td>";
                //        string tableDataTagClose = "</td>";

                //        if (rowNumber == 0)
                //        {
                //            tableDataTagOpen = "<th>";
                //            tableDataTagClose = "</th>";
                //        }

                //        if (columnNumber == 0)
                //        {
                //            htmlBuilder.AppendLine("<tr>");
                //        }

                //        string value = ((TextBlock)control).Text;
                //        string formattedValue = value;

                //        htmlBuilder.AppendLine(tableDataTagOpen + formattedValue + tableDataTagClose);

                //        if (columnNumber >= grid.ColumnDefinitions.Count - 1)
                //        {
                //            htmlBuilder.AppendLine("</tr>");
                //        }
                //    }
                //}

                //htmlBuilder.AppendLine("</table>");

                //string disclaimer = GetStrataChiSquareDisclaimer(grid.Tag.ToString()).Text;
                //if (!string.IsNullOrEmpty(disclaimer))
                //{
                //    htmlBuilder.AppendLine("<p></p>");
                //    htmlBuilder.AppendLine("<p>" + disclaimer.Replace("<", "&lt;") + "</p>");
                //}

                // End Chi Square
            }

            //foreach (GadgetTwoByTwoPanel grid2x2 in this.strata2x2GridList)
            //{
            //    //string gridName = grid.Tag.ToString();
            //    string gridName = string.Empty;
            //    if (grid2x2.Tag != null)
            //    {
            //        gridName = grid2x2.Tag.ToString();
            //    }
            //    if (!string.IsNullOrEmpty(gridName))
            //    {
            //        htmlBuilder.AppendLine("<h3>" + gridName + "</h3>");
            //    }
            //    htmlBuilder.AppendLine("<div style=\"height: 7px;\"></div>");
            //    htmlBuilder.AppendLine(grid2x2.ToHTML());
            //    htmlBuilder.AppendLine("<p></p>");
            //}

            //foreach (UIElement element in panelMain.Children)
            //{
            //    if (element is StratifiedTableAnalysisPanel)
            //    {
            //        StratifiedTableAnalysisPanel stap = element as StratifiedTableAnalysisPanel;
            //        htmlBuilder.AppendLine("<div style=\"height: 7px;\"></div>");
            //        htmlBuilder.AppendLine(stap.ToHTML());
            //        htmlBuilder.AppendLine("<p></p>");
            //    }
            //}

            //return htmlBuilder.ToString();
            System.Windows.Browser.HtmlPage.Window.Invoke("DisplayFormattedText", htmlBuilder.ToString());

            HtmlBuilder = htmlBuilder;    

            return "";
        }

        //private Grid GetStrataChiSquareGrid(string strataValue)
        //{
        //    Grid grid = new Grid();

        //    foreach (Grid g in strataChiSquareGridList)
        //    {
        //        if (g.Tag.Equals(strataValue))
        //        {
        //            grid = g;
        //            break;
        //        }
        //    }

        //    return grid;
        //}
        private Grid GetStrataGrid(string strataValue)
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

        private void AddFreqGrid(string strataVar, string value, string crosstabVar, int columnCount)
        {
            Grid grid = new Grid();
            grid.Tag = value;
            //grid.Name = "grdContent";
            grid.Width = grdFreq.Width;
            grid.HorizontalAlignment = HorizontalAlignment.Center;
            grid.Margin = new Thickness(0, 0, 0, 0);
            grid.Visibility = System.Windows.Visibility.Collapsed;

            for (int i = 0; i < columnCount; i++)
            {
                if (i > MaxColumns)
                {
                    break;
                }

                ColumnDefinition column = new ColumnDefinition();
                column.Width = GridLength.Auto;
                grid.ColumnDefinitions.Add(column);
            }

            ColumnDefinition totalColumn = new ColumnDefinition();
            totalColumn.Width = GridLength.Auto;
            grid.ColumnDefinitions.Add(totalColumn);

            TextBlock txtGridLabel = new TextBlock();
            txtGridLabel.Text = value;
            txtGridLabel.VerticalAlignment = VerticalAlignment.Bottom;
            txtGridLabel.Margin = new Thickness(2, 0, 2, 2);
            txtGridLabel.FontWeight = FontWeights.Bold;
            if (string.IsNullOrEmpty(strataVar))
            {
                txtGridLabel.Text = crosstabVar;
                txtGridLabel.Margin = new Thickness(2, 0, 2, 2);
                txtGridLabel.HorizontalAlignment = HorizontalAlignment.Left;
                //txtGridLabel.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                if (value.EndsWith(" = "))
                {
                    //   value = value + dashboardHelper.Config.Settings.RepresentationOfMissing;
                }
                txtGridLabel.Text = string.Format("{0}{1}{2}{3}", crosstabVar, sl.COMMA, sl.SPACE, value);
                if (strataGridList.Count < 1)
                {
                    txtGridLabel.Margin = new Thickness(2, 0, 2, 2);
                }
                else
                {
                    txtGridLabel.Margin = new Thickness(2, 54, 2, 2);
                }
                txtGridLabel.HorizontalAlignment = HorizontalAlignment.Left;
            }
            gridLabelsList.Add(txtGridLabel);
            panelMain.Children.Add(txtGridLabel);

            panelMain.Children.Add(grid);
            strataGridList.Add(grid);
        }

        private void SetGridText(string strataValue, TextBlockConfig textBlockConfig, FontWeight fontWeight)
        {
            Grid grid = new Grid();

            grid = GetStrataGrid(strataValue);

            TextBlock txt = new TextBlock();
            txt.FontWeight = fontWeight;
            txt.Text = textBlockConfig.Text;
            txt.Margin = textBlockConfig.Margin;
            txt.VerticalAlignment = textBlockConfig.VerticalAlignment;
            txt.HorizontalAlignment = textBlockConfig.HorizontalAlignment;
            Grid.SetRow(txt, textBlockConfig.RowNumber);
            Grid.SetColumn(txt, textBlockConfig.ColumnNumber);
            grid.Children.Add(txt);
        }

        private void AddGridRow(string strataValue, int height)
        {
            Grid grid = GetStrataGrid(strataValue);

            waitCursor.Visibility = Visibility.Collapsed;
            grdConf.Visibility = Visibility.Visible;
            grid.Visibility = Visibility.Visible; //grdFreq.Visibility = Visibility.Visible;
            txtConfLimits.Visibility = Visibility.Visible;
            RowDefinition rowDef = new RowDefinition();
            rowDef.Height = new GridLength(height);
            grid.RowDefinitions.Add(rowDef);
        }

        private void RenderFrequencyHeader(string strataValue, string freqVar, List<MyString> columns,
            List<MyString> columnNames)
        {
            Grid grid = GetStrataGrid(strataValue);

            RowDefinition rowDefHeader = new RowDefinition();
            rowDefHeader.Height = new GridLength(38);
            grid.RowDefinitions.Add(rowDefHeader);

            for (int y = 0; y < grid.ColumnDefinitions.Count; y++)
            {
                Rectangle rctHeader = new Rectangle();
                rctHeader.Style = Application.Current.Resources["HeaderCell"] as Style;
                //     rctHeader.Fill = SystemColors.MenuHighlightBrush;
                Grid.SetRow(rctHeader, 0);
                Grid.SetColumn(rctHeader, y);
                grid.Children.Add(rctHeader);
            }

            TextBlock txtValHeader = new TextBlock();
            txtValHeader.Text = string.Format("{0}{1}{2}", sl.SPACE, freqVar, sl.SPACE);
            //txtValHeader.VerticalAlignment = VerticalAlignment.Center;
            //txtValHeader.HorizontalAlignment = HorizontalAlignment.Center;
            //txtValHeader.Margin = new Thickness(2, 0, 2, 0);
            //txtValHeader.FontWeight = FontWeights.Bold;
            //txtValHeader.Foreground = new SolidColorBrush(Colors.Black);
            txtValHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtValHeader, 0);
            Grid.SetColumn(txtValHeader, 0);
            grid.Children.Add(txtValHeader);

            int maxColumnLength = MaxColumnLength;

            for (int i = 1; i < columns.Count; i++)
            {
                if (i > MaxColumns)
                {
                    break;
                }
                TextBlock txtColHeader = new TextBlock();
                string columnName = columnNames[i].VarName.Trim();

                if (columnName.Length > maxColumnLength)
                {
                    columnName = columnName.Substring(0, maxColumnLength);
                }

                txtColHeader.Text = string.Format("{0}{1}{2}", sl.SPACE, columnName, sl.SPACE);
                //txtColHeader.VerticalAlignment = VerticalAlignment.Center;
                //txtColHeader.HorizontalAlignment = HorizontalAlignment.Center;
                //txtColHeader.Margin = new Thickness(2, 0, 2, 0);
                //txtColHeader.FontWeight = FontWeights.Bold;
                //txtColHeader.Foreground = new SolidColorBrush(Colors.Black);
                //  txtColHeader.Foreground = Brushes.White;
                txtColHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
                Grid.SetRow(txtColHeader, 0);
                Grid.SetColumn(txtColHeader, i);
                grid.Children.Add(txtColHeader);
            }

            TextBlock txtRowTotalHeader = new TextBlock();
            txtRowTotalHeader.Text = string.Format("{0}{1}{2}", sl.SPACE, SharedStrings.TOTAL, sl.SPACE);
            //txtRowTotalHeader.VerticalAlignment = VerticalAlignment.Center;
            //txtRowTotalHeader.HorizontalAlignment = HorizontalAlignment.Center;
            //txtRowTotalHeader.Margin = new Thickness(2, 0, 2, 0);
            //txtRowTotalHeader.FontWeight = FontWeights.Bold;
            //txtRowTotalHeader.Foreground = new SolidColorBrush(Colors.Black);  //  Brushes.White;
            txtRowTotalHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtRowTotalHeader, 0);
            Grid.SetColumn(txtRowTotalHeader, MaxColumns + 1);
            grid.Children.Add(txtRowTotalHeader);
        }

        private void RenderFrequencyFooter(string strataValue, int footerRowIndex, int[] totalRows)
        {
            Grid grid = GetStrataGrid(strataValue);

            RowDefinition rowDefTotals = new RowDefinition();
            rowDefTotals.Height = new GridLength(30);
            grid.RowDefinitions.Add(rowDefTotals);

            TextBlock txtValTotals = new TextBlock();
            txtValTotals.Text = string.Format("{0}{1}{2}", sl.SPACE, SharedStrings.TOTAL, sl.SPACE);
            txtValTotals.Margin = new Thickness(2, 0, 2, 0);
            txtValTotals.VerticalAlignment = VerticalAlignment.Center;
            txtValTotals.FontWeight = FontWeights.Bold;
            Grid.SetRow(txtValTotals, footerRowIndex);
            Grid.SetColumn(txtValTotals, 0);
            grid.Children.Add(txtValTotals);

            for (int i = 0; i < totalRows.Length; i++)
            {
                if (i >= MaxColumns)
                {
                    break;
                }
                TextBlock txtFreqTotals = new TextBlock();
                txtFreqTotals.Text = string.Format("{0}{1}{2}", sl.SPACE, totalRows[i].ToString(), sl.SPACE);
                txtFreqTotals.Margin = new Thickness(2, 0, 2, 0);
                txtFreqTotals.VerticalAlignment = VerticalAlignment.Center;
                txtFreqTotals.HorizontalAlignment = HorizontalAlignment.Right;
                txtFreqTotals.FontWeight = FontWeights.Bold;
                Grid.SetRow(txtFreqTotals, footerRowIndex);
                Grid.SetColumn(txtFreqTotals, i + 1);
                grid.Children.Add(txtFreqTotals);
            }

            int sumTotal = 0;
            foreach (int n in totalRows)
            {
                sumTotal = sumTotal + n;
            }

            TextBlock txtOverallTotal = new TextBlock();
            txtOverallTotal.Text = string.Format("{0}{1}{2}", sl.SPACE, sumTotal.ToString(), sl.SPACE);
            txtOverallTotal.Margin = new Thickness(2, 0, 2, 0);
            txtOverallTotal.VerticalAlignment = VerticalAlignment.Center;
            txtOverallTotal.HorizontalAlignment = HorizontalAlignment.Right;
            txtOverallTotal.FontWeight = FontWeights.Bold;
            Grid.SetRow(txtOverallTotal, footerRowIndex);
            Grid.SetColumn(txtOverallTotal, MaxColumns + 1);
            grid.Children.Add(txtOverallTotal);
        }

        private int MaxRows
        {
            get
            {
                //        return 200;
                int maxRows = 200;
                bool success = int.TryParse(txtMaxRows.Text, out maxRows);
                if (!success)
                {
                    return 200;
                }
                else
                {
                    return maxRows;
                }
            }
        }
        private void DrawFrequencyBorders(string strataValue)
        {
            Grid grid = GetStrataGrid(strataValue);

            waitCursor.Visibility = Visibility.Collapsed;
            int rdcount = 0;
            foreach (RowDefinition rd in grid.RowDefinitions)
            {
                int cdcount = 0;
                foreach (ColumnDefinition cd in grid.ColumnDefinitions)
                {
                    Rectangle rctBorder = new Rectangle();
                    //rctBorder.Stroke = new SolidColorBrush(Colors.Black);        //      Brushes.Black;
                    //rctBorder.StrokeThickness = 0.5;
                    rctBorder.Style = Application.Current.Resources["DataCell"] as Style;
                    Grid.SetRow(rctBorder, rdcount);
                    Grid.SetColumn(rctBorder, cdcount);
                    grid.Children.Add(rctBorder);
                    cdcount++;
                }
                rdcount++;
            }
        }

        public event GadgetClosingHandler GadgetClosing;

        public event GadgetProcessingFinishedHandler GadgetProcessingFinished;

        public StringBuilder HtmlBuilder { get; set; }

        public void RefreshResults()
        {
            //throw new NotImplementedException();
        }

        //public string ToHTML(string htmlFileName = "", int count = 0)
        //{
        //    throw new NotImplementedException();
        //}

        public void UpdateVariableNames()
        {
            //throw new NotImplementedException();
        }

        public string CustomOutputHeading { get; set; }

        public string CustomOutputDescription { get; set; }

        public string CustomOutputCaption { get; set; }

        /// <summary>
        /// Closes the gadget after confirmation.
        /// </summary>
        public void CloseGadgetOnClick()
        {
            CloseGadget confirm = new CloseGadget(this);
            confirm.Show();
        }

        /// <summary>
        /// Closes the gadget.
        /// </summary>
        public void CloseGadget()
        {
            applicationViewModel.CloseGadget(this);
        }

        private void UnloadGadget()
        {
            applicationViewModel.ApplyDataFilterEvent -= new ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);

            applicationViewModel.DefinedVariableAddedEvent -= new Client.Application.DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);
            applicationViewModel.DefinedVariableInUseDeletedEvent -= new Client.Application.DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableInUseDeletedEvent);
            applicationViewModel.DefinedVariableNotInUseDeletedEvent -= new Client.Application.DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
            applicationViewModel.PreColumnChangedEvent -= new Client.Application.PreColumnChangedEventHandler(applicationViewModel_PreColumnChangedEvent);
        }

        private bool VerifyNumber(string input)
        {
            if (!this.cmnClass.IsWholeNumber((input)))
            {
                return false;
            }
            return true;
        }

        private void txtMaxRows_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.VerifyNumber(((TextBox)sender).Text))
            {
                ((TextBox)sender).Text = "2";
                return;
            }
        }

        //private void txtMaxColumns_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (!this.VerifyNumber(((TextBox)sender).Text))
        //    {
        //        this.txtMaxColumns.Text = "2";
        //        return;
        //    }
        //}

        //private void txtMaxColumnLength_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (!this.VerifyNumber(((TextBox)sender).Text))
        //    {
        //        this.txtMaxColumnLength.Text = "2";
        //        return;
        //    }
        //}

        private void btn_Rows_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (cbxExposureField.SelectedIndex > -1 && cbxOutcomeField.SelectedIndex > -1)
                {
                    domxncount = 0;
                    RequestMxN();
                }
            }
        }

        public void CreateFromXml(XElement element)
        {
            //this.LoadingCombos = true;
            //this.ColumnWarningShown = true;
            //HideConfigPanel();
            //infoPanel.Visibility = System.Windows.Visibility.Collapsed;
            //messagePanel.Visibility = System.Windows.Visibility.Collapsed;
            LoadingCanvas = true;

            //InitializeControl();

            this.GadgetFilters = new List<EwavDataFilterCondition>();
            List<EwavColumn> fieldPrimaryColsList = new List<EwavColumn>();
            List<EwavColumn> fieldWeightColsList = new List<EwavColumn>();
            fieldPrimaryColsList = cmnClass.GetItemsSource(GetFieldPrimaryDataType);
            fieldWeightColsList = cmnClass.GetItemsSource(GetFieldWeightDataType);
            foreach (XElement child in element.Descendants())
            {
                switch (child.Name.ToString().ToLower())
                {
                    case "mainvariable":
                    case "exposurevariable": // added to work with the old 2x2 gadget
                        //cbxExposureField.Text = child.InnerText.Replace("&lt;", "<");
                        cbxExposureField.SelectedIndex = cmnClass.FindIndexToSelect(fieldPrimaryColsList, child.Value.ToString().Replace("&lt;", "<"));
                        break;
                    case "crosstabvariable":
                    case "outcomevariable": // added to work with the old 2x2 gadget
                        //cbxOutcomeField.Text = child.InnerText.Replace("&lt;", "<");
                        cbxOutcomeField.SelectedIndex = cmnClass.FindIndexToSelect(fieldPrimaryColsList, child.Value.ToString().Replace("&lt;", "<"));
                        break;
                    case "stratavariable":
                        //cbxFieldStrata.Text = child.InnerText.Replace("&lt;", "<");
                        //if (!string.IsNullOrEmpty(child.InnerText))
                        //{
                        //    lbxFieldStrata.SelectedItems.Add(child.InnerText.Replace("&lt;", "<"));
                        //}
                        cbxFieldStrata.SelectedIndex = cmnClass.FindIndexToSelect(fieldPrimaryColsList, child.Value.ToString().Replace("&lt;", "<"));
                        break;
                    case "weightvariable":
                        //cbxFieldWeight.Text = child.InnerText.Replace("&lt;", "<");
                        cbxFieldWeight.SelectedIndex = cmnClass.FindIndexToSelect(fieldWeightColsList, child.Value.ToString().Replace("&lt;", "<"));
                        break;
                    case "maxcolumnnamelength":
                        int maxColumnLength = 24;
                        int.TryParse(child.Value, out maxColumnLength);
                        txtMaxColumnLength.Text = maxColumnLength.ToString();
                        break;
    
                    case "includemissing":
                        if (child.Value.ToLower().Equals("true"))
                        {
                            checkboxIncludeMissing.IsChecked = true;
                        }
                        else
                        {
                            checkboxIncludeMissing.IsChecked = false;
                        }
                        break;
    
                    case "treatoutcomeascontinuous":
                        if (child.Value.ToLower().Equals("true"))
                        {
                            checkboxOutcomeContinuous.IsChecked = true;
                        }
                        else
                        {
                            checkboxOutcomeContinuous.IsChecked = false;
                        }
                        break;
                    case "maxcolumns":
                        txtMaxColumns.Text = child.Value;
                        break;
                    case "maxrows":
                        txtMaxRows.Text = child.Value;
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

            RequestMxN();

            cmnClass.AddControlToCanvas(this, mouseVerticalPosition, mouseHorizontalPosition, applicationViewModel.LayoutRoot);
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            if (cbxExposureField.SelectedIndex > -1 && cbxOutcomeField.SelectedIndex > -1)
            {
                if (!LoadingCanvas)
                {
                    domxncount = 0;
                    RequestMxN();
                }
            }
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


        public void Reload()
        {



            if (cbxExposureField.SelectedIndex > -1 && cbxOutcomeField.SelectedIndex > -1)
            {
                if (!LoadingCanvas)
                {
                    domxncount = 0;
                    RequestMxN();
                }
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

                if (cbxExposureField.SelectedIndex > -1 && cbxOutcomeField.SelectedIndex > -1)
                {
                    if (!LoadingCanvas)
                    {
                        domxncount = 0;
                        RequestMxN();
                    }
                }
            }
        }


    }
}

namespace Ewav.Web.Services
{
    public partial class MxNDomainContext
    {
        //This is the place to set RIA Services query timeout. 
        //TimeSpan(0, 5, 0) means five minutes vs the 
        //default 60 sec
        partial void OnCreated()
        {
            if (!DesignerProperties.GetIsInDesignMode(Application.Current.RootVisual))
            {
                ((WebDomainClient<IMxNDomainServiceContract>)DomainClient).ChannelFactory.Endpoint.Binding.SendTimeout =
                    new TimeSpan(0, 120, 0);
            }
        }
    }
}