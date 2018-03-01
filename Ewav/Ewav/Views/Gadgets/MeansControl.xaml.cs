/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MeansControl.xaml.cs
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
using System.Windows.Shapes;
using CommonLibrary;
using Ewav.Web.EpiDashboard;
using Ewav.BAL;
using Ewav.ViewModels;
using Ewav.Web.Services;
using System.ComponentModel;
using System.ServiceModel.DomainServices.Client;
using System.Xml.Linq;
using Ewav.ExtensionMethods;

namespace Ewav
{
    /// <summary>
    /// Interaction logic for MeansControl.xaml
    /// </summary>
    /// <remarks>
    /// This gadget is used to generate descriptive statistics for a numeric variable in the database. It will return the mean, median, mode, standard
    /// deviation, variance, 25% value, 75% value, min value and max value. It also supports weights and can be both stratified and cross-tabulated.
    /// It is essentially a mirror of the MEANS command in the 'Classic' Analysis module.
    /// </remarks>          
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(UserControl))]
    [ExportMetadata("type", "gadget")]
    [ExportMetadata("tabindex", "5")]
    public partial class MeansControl : UserControl, IGadget, IEwavGadget, ICustomizableGadget
    {



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

        public List<EwavDataFilterCondition> GadgetFilters { get; set; }

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


        #region Private Members
        /// <summary>
        /// The list of labels for each strata value. E.g. if stratifying by SEX, there would be (likely)
        /// two values in this list: One for Male and one for Female. These show up as the table
        /// headings when displaying output.
        /// </summary>
        private List<TextBlock> gridLabelsList;

        /// <summary>
        /// The list of grids for each strata value. E.g. if stratifying by SEX, there would be (likely)
        /// two grids in this list: One showing the summary statistics for Male and another for Female.
        /// </summary>
        private List<Grid> strataGridList;

        /// <summary>
        /// A list of panels, one for each of the stratifcation values, and containing the ANOVA statistics
        /// </summary>
        private List<StackPanel> anovaBlocks;

        /// <summary>
        /// Bool used to determine whether or not to run events on the combo boxes
        /// </summary>
        private bool loadingCombos;

        /// <summary>
        /// The object that is passed into the Dashboard Helper and contains the options the user selected.
        /// For example, this will contain the allColumns for the main variable, weight variable, strata variable(s),
        /// and any other parameters set in the gadget UI.
        /// </summary>
        private GadgetParameters gadgetOptions;

        /// <summary>
        /// Required to 'lock' the worker thread so that you can't spawn more than one worker thread
        /// in the gadget at a single time.
        /// </summary>
        private object syncLock = new object();

        public ApplicationViewModel ApplicationViewModel
        {
            get
            {
                return applicationViewModel;
            }
        }

        public string MyUIName
        {
            get
            {
                return "Means";
            }
        }

        /// <summary>
        /// A data structure used for managing the configuration of text blocks within the output grid
        /// </summary>
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

        #endregion // Private Members

        #region Delegates

        private delegate void SetGridTextDelegate(string strataValue, TextBlockConfig textBlockConfig);
        private delegate void AddMeansGridDelegate(string strataVar, string value);
        private delegate void AddAnovaDelegate(string strataValue, DescriptiveStatistics stats);
        private delegate void RenderMeansHeaderDelegate(string strataValue, string meansVar, string crosstabVar);
        private delegate void SetGridBarDelegate(string strataValue, int rowNumber, double pct);
        private delegate void AddGridRowDelegate(string strataValue, int height);
        private delegate void AddGridFooterDelegate(string strataValue, int rowNumber, int totalRows);
        private delegate void DrawMeansBordersDelegate(string strataValue);

        private delegate void SetStatusDelegate(string statusMessage);
        private delegate void RequestUpdateStatusDelegate(string statusMessage);
        private delegate bool CheckForCancellationDelegate();
        private delegate void RenderFinishWithErrorDelegate(string errorMessage);
        private delegate void SimpleCallback();

        #endregion // Delegates

        #region Events

        public event GadgetClosingHandler GadgetClosing;
        public event GadgetProcessingFinishedHandler GadgetProcessingFinished;
        public event GadgetStatusUpdateHandler GadgetStatusUpdate;
        //     public event GadgetCheckForCancellationHandler GadgetCheckForCancellation;

        #endregion // Events

        List<CrossTabResponseObjectDto> crossTable;

        ClientCommon.Common cmnClass = new ClientCommon.Common();

        int FieldSelectedIndex1 = -1, WeightSelectedIndex2 = -1, CrossTabSelectedIndex3 = -1, StrataSelectedIndex4 = -1;

        // EwavColumns to store state of dropdowns for user def vars     
        //   
        EwavColumn FieldSelectedCol1, WeightSelectedCol2, CrossTabSelectedCol3, StrataSelectedCol4;

        private bool loadingDropDowns = false;

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

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public MeansControl()
        {
            InitializeComponent();

            // Init gadget parameters with default values
            gadgetOptions = new GadgetParameters();
            gadgetOptions.ShouldIncludeFullSummaryStatistics = true;
            gadgetOptions.ShouldIncludeMissing = false;
            gadgetOptions.ShouldSortHighToLow = false;
            gadgetOptions.ShouldUseAllPossibleValues = false;
            gadgetOptions.StrataVariableNames = new List<string>();
            this.Loaded -= new RoutedEventHandler(MeansControl_Loaded);
            this.Loaded += new RoutedEventHandler(MeansControl_Loaded);

            FillDropDowns();
        }
        void MeansControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeControl();
        }

        private void InitializeControl()
        {
            List<DTO.EwavFrequencyControlDto> listDto1 = new List<DTO.EwavFrequencyControlDto>();
            try
            {
                MeansViewModel meansViewModel = (MeansViewModel)this.DataContext;
                //   meansViewModel.ColumnsLoadedEvent +=
                //       new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(LoadComboBoxes);
                meansViewModel.FrequencyTableLoadedEvent +=
                    new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(meansViewModel_FrequencyTableLoadedEvent);
                //meansViewModel.MeansCrossTabDataLoadedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(meansViewModel_MeansCrossTabDataLoadedEvent);
                meansViewModel.FreqAndCrossTableLoadedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(meansViewModel_FreqAndCrossTableLoadedEvent);
                meansViewModel.ErrorNotice += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(meansViewModel_ErrorNotice);
                applicationViewModel.ApplyDataFilterEvent += new Client.Application.ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);

                //////////////////////////
                // Events for user-def field suport 
                //
                // Fired when a new user-def fields is added to the application.  Gadget response it to refresh all dropdowns.    
                applicationViewModel.DefinedVariableAddedEvent += new Client.Application.DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);
                applicationViewModel.DefinedVariableInUseDeletedEvent += new Client.Application.DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableDeletedEvent);
                applicationViewModel.DefinedVariableNotInUseDeletedEvent += new Client.Application.DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
                applicationViewModel.PreColumnChangedEvent += new Client.Application.PreColumnChangedEventHandler(applicationViewModel_PreColumnChangedEvent);
                applicationViewModel.UnloadedEvent += new Client.Application.UnloadedEventHandler(applicationViewModel_UnloadedEvent);

                //  meansViewModel.GetColumns( "NEDS", "FoodHistory1");    
            }
            catch (Exception)
            {
                MessageBox.Show("Error loading Means Control. Try again.");
                return;
            }

            Construct();
            //RenderFinish();
        }

        void applicationViewModel_UnloadedEvent(object o)
        {
            UnloadGadget();
        }

        private void UnloadGadget()
        {
            applicationViewModel.ApplyDataFilterEvent -= new Client.Application.ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);
            applicationViewModel.DefinedVariableAddedEvent -= new Client.Application.DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);
            applicationViewModel.DefinedVariableInUseDeletedEvent -= new Client.Application.DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableDeletedEvent);
            applicationViewModel.DefinedVariableNotInUseDeletedEvent -= new Client.Application.DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
            applicationViewModel.PreColumnChangedEvent -= new Client.Application.PreColumnChangedEventHandler(applicationViewModel_PreColumnChangedEvent);
        }

        /// <summary>
        /// Applications the view model_ defined variable added event.
        /// </summary>
        /// <param name="o">The o.</param>
        private void applicationViewModel_DefinedVariableAddedEvent(object o)
        {
            SearchIndex();
            LoadingDropDowns = true;
            FillDropDowns();
            LoadingDropDowns = false;
            DoMeans();
        }

        /// <summary>
        /// Applications the view model_ pre column changed event.
        /// </summary>
        /// <param name="o">The o.</param>
        private void applicationViewModel_PreColumnChangedEvent(object o)
        {
            SaveColumnValues();
        }

        /// <summary>
        /// Applications the view model_ defined variable deleted event.
        /// </summary>
        /// <param name="o">The o.</param>
        private void applicationViewModel_DefinedVariableDeletedEvent(object o)
        {
            //SearchIndex();
            //if (IsDFUsedInThisGadget())
            //{
            //    FieldSelectedIndex1 = WeightSelectedIndex2 = CrossTabSelectedIndex3 = StrataSelectedIndex4 = -1;

            //  HideMainPanel();  
            //panelMain.Visibility = System.Windows.Visibility.Collapsed;
            //waitCursor.Visibility = System.Windows.Visibility.Collapsed;
            //return;
            ResetGadget();
            //}
            //LoadingDropDowns = true;
            //FillDropDowns();
            //LoadingDropDowns = false;
        }

        private bool IsDFUsedInThisGadget()
        {
            return FieldSelectedCol1 != null && FieldSelectedCol1.Name == applicationViewModel.ItemToBeRemoved.Name ||
                            WeightSelectedCol2 != null && WeightSelectedCol2.Name == applicationViewModel.ItemToBeRemoved.Name ||
                            CrossTabSelectedCol3 != null && CrossTabSelectedCol3.Name == applicationViewModel.ItemToBeRemoved.Name ||
                            StrataSelectedCol4 != null && StrataSelectedCol4.Name == applicationViewModel.ItemToBeRemoved.Name;
        }

        /// <summary>
        /// Saves the Values of Columns.
        /// </summary>
        private void SaveColumnValues()
        {
            FieldSelectedCol1 = (EwavColumn)cbxField.SelectedItem;
            WeightSelectedCol2 = (EwavColumn)cbxFieldWeight.SelectedItem;

            CrossTabSelectedCol3 = (EwavColumn)cbxFieldCrosstab.SelectedItem;
            StrataSelectedCol4 = (EwavColumn)cbxFieldStrata.SelectedItem;
        }

        /// <summary>
        /// Applications the view model_ defined variable not in use deleted event.
        /// </summary>
        /// <param name="o">The o.</param>
        private void applicationViewModel_DefinedVariableNotInUseDeletedEvent(object o)
        {
            SearchIndex();
            LoadingDropDowns = true;
            FillDropDowns();
            LoadingDropDowns = false;
            DoMeans();
        }

        void meansViewModel_ErrorNotice(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            if (e.Data.Message.Length > 0)
            {
                ChildWindow window = new ErrorWindow(e.Data);
                window.Show();
                //return;
            }
            RenderFinishWithError(e.Data.Message);
        }

        void applicationViewModel_ApplyDataFilterEvent(object o)
        {
            //if ((applicationViewModel.ItemToBeRemoved != null) &&
            //    DFInUse != null &&
            //    (applicationViewModel.ItemToBeRemoved.Name == DFInUse.Name))
            if (applicationViewModel.RemoveIndicator &&
                IsDFUsedInThisGadget())
            {
                ResetGadget();
            }
            else
            {
                DoMeans();
            }


        }

        private void ResetGadget()
        {
            SearchIndex();
            if (IsDFUsedInThisGadget())
            {
                FieldSelectedIndex1 = WeightSelectedIndex2 = CrossTabSelectedIndex3 = StrataSelectedIndex4 = -1;
                pnlStatus.Visibility = System.Windows.Visibility.Collapsed;
                panelMain.Visibility = System.Windows.Visibility.Collapsed;
                waitCursor.Visibility = System.Windows.Visibility.Collapsed;
                //return;
            }
            LoadingDropDowns = true;
            FillDropDowns();
            LoadingDropDowns = false;
        }

        /// <summary>
        /// Searches current index of the columns.  
        /// Gets the current index of the selected item 
        /// </summary>
        private void SearchIndex()
        {
            //if (FieldSelectedCol1 != null)
            //{
            //    FieldSelectedIndex1 = SearchCurrentIndex(FieldSelectedCol1);
            //}

            //if (WeightSelectedCol2 != null)
            //{
            //    WeightSelectedIndex2 = SearchCurrentIndex(WeightSelectedCol2);
            //}

            //if (CrossTabSelectedCol3 != null)
            //{
            //    CrossTabSelectedIndex3 = SearchCurrentIndex(CrossTabSelectedCol3);
            //}
            //if (StrataSelectedCol4 != null)
            //{
            //    StrataSelectedIndex4 = SearchCurrentIndex(StrataSelectedCol4);
            //}

            //List<ColumnDataType> columnDataType = new List<ColumnDataType>();
            //columnDataType.Clear();
            //columnDataType.Add(ColumnDataType.Boolean);
            //columnDataType.Add(ColumnDataType.Numeric);
            //columnDataType.Add(ColumnDataType.Text);

            //Col1ItemSource = ;

            ClientCommon.Common CommonClass = new ClientCommon.Common();

            FieldSelectedIndex1 = CommonClass.SearchCurrentIndex(FieldSelectedCol1, CommonClass.GetItemsSource(GetFieldDataType));

            WeightSelectedIndex2 = CommonClass.SearchCurrentIndex(WeightSelectedCol2, CommonClass.GetItemsSource(GetWeightDataType));

            CrossTabSelectedIndex3 = CommonClass.SearchCurrentIndex(CrossTabSelectedCol3, CommonClass.GetItemsSource(GetPrimaryFieldDataType));

            StrataSelectedIndex4 = CommonClass.SearchCurrentIndex(StrataSelectedCol4, CommonClass.GetItemsSource(GetPrimaryFieldDataType));

        }

        /// <summary>
        /// Method used to locate the current index for selected column.
        /// </summary>
        /// <param name="Column"></param>
        /// <returns></returns>
        //private int SearchCurrentIndex(EwavColumn Column)
        //{
        //    //if (Column != null)
        //    //{
        //    List<EwavColumn> SourceColumns = this.applicationViewModel.EwavSelectedDatasource.AllColumns;

        //    IEnumerable<EwavColumn> CBXFieldCols = from cols in SourceColumns
        //                                           orderby cols.Name
        //                                           select cols;

        //    List<EwavColumn> colsList = CBXFieldCols.ToList();

        //    for (int i = 0; i < colsList.Count; i++)
        //    {
        //        if (Column.Name == colsList[i].Name)
        //        {
        //            return i + 1;
        //        }
        //    }
        //    //}
        //    return 0;
        //}

        private void IsUserDefindVariableInUse()
        {
            /// Following loop defaults all the userDefined variable to not in use.
            //for (int rule = 0; rule < applicationViewModel.EwavSelectedDatasource.AllColumns.Count; rule++)
            //{
            //    if (applicationViewModel.EwavSelectedDatasource.AllColumns[rule].SqlDataTypeAsString == ColumnDataType.UserDefined)
            //    {
            //        applicationViewModel.EwavSelectedDatasource.AllColumns[rule].IsInUse = false;
            //    }
            //}
            ///following logic turns only those flags on which are in use.
            FieldSelectedCol1 = (cbxField.SelectedIndex > -1) ? (EwavColumn)cbxField.SelectedItem : null;
            StrataSelectedCol4 = (cbxFieldStrata.SelectedIndex > -1) ? (EwavColumn)cbxFieldStrata.SelectedItem : null;
            WeightSelectedCol2 = (cbxFieldWeight.SelectedIndex > -1) ? (EwavColumn)cbxFieldWeight.SelectedItem : null;
            CrossTabSelectedCol3 = (cbxFieldCrosstab.SelectedIndex > -1) ? (EwavColumn)cbxFieldCrosstab.SelectedItem : null;
            //DFInUse = null;
            if (FieldSelectedCol1 != null && FieldSelectedCol1.IsUserDefined)
            {
                FieldSelectedCol1.IsInUse = true;
                //DFInUse = FieldSelectedCol1;
            }
            if (StrataSelectedCol4 != null && StrataSelectedCol4.IsUserDefined)
            {
                StrataSelectedCol4.IsInUse = true;
                //DFInUse = StrataSelectedCol4;
            }
            if (WeightSelectedCol2 != null && WeightSelectedCol2.IsUserDefined)
            {
                WeightSelectedCol2.IsInUse = true;
                //DFInUse = WeightSelectedCol2;
            }

            if (CrossTabSelectedCol3 != null && CrossTabSelectedCol3.IsUserDefined)
            {
                CrossTabSelectedCol3.IsInUse = true;
                //DFInUse = CrossTabSelectedCol3;
            }
        }

        private void DoMeans()
        {
            waitCursor.Visibility = Visibility.Visible;
            if (!LoadingDropDowns && !LoadingCanvas)
            {
                IsUserDefindVariableInUse();

                try
                {
                    ValidateInput();
                }
                catch (Exception)
                {
                    RenderFinishWithError("ValidateInput -- Item already exists.");
                    waitCursor.Visibility = Visibility.Collapsed;
                    return;
                    //throw;
                }

                try
                {
                    RefreshResults();
                }
                catch (Exception)
                {
                    RenderFinishWithError("Item already exists.");
                    waitCursor.Visibility = Visibility.Collapsed;
                    //throw;
                }
                this.gadgetExpander.IsExpanded = false;
            }
            else
            {
                waitCursor.Visibility = Visibility.Collapsed;
            }
            //
        }

        void meansViewModel_FreqAndCrossTableLoadedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            MeansViewModel meansViewModel = (MeansViewModel)sender;

            crossTable = meansViewModel.FreqAndCrossTable.CrossTable;

            List<FrequencyResultData> notConvertedTable = meansViewModel.FreqAndCrossTable.FrequencyTable;
            Dictionary<List<DTO.EwavFrequencyControlDto>, List<DescriptiveStatistics>> Data = new Dictionary<List<DTO.EwavFrequencyControlDto>, List<DescriptiveStatistics>>();
            for (int i = 0; i < notConvertedTable.Count; i++)
            {
                Data.Add(notConvertedTable[i].FrequencyControlDtoList, crossTable[i].DsList);
            }

            DoWork(Data, gadgetOptions);
        }

        void meansViewModel_MeansCrossTabDataLoadedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            MeansViewModel meansViewModel = (MeansViewModel)sender;

            crossTable = meansViewModel.CrossTabTable;
            List<DescriptiveStatistics> dsList = crossTable[0].DsList;

            List<FrequencyResultData> notConvertedTable = meansViewModel.FrequencyTable;
            Dictionary<List<DTO.EwavFrequencyControlDto>, List<DescriptiveStatistics>> Data = new Dictionary<List<DTO.EwavFrequencyControlDto>, List<DescriptiveStatistics>>();
            for (int i = 0; i < notConvertedTable.Count; i++)
            {
                Data.Add(notConvertedTable[i].FrequencyControlDtoList, crossTable[i].DsList);
            }

            DoWork(Data, gadgetOptions);
        }

        public List<ColumnDataType> GetFieldDataType
        {
            get
            {
                List<ColumnDataType> fieldDataType = new List<ColumnDataType>();
                fieldDataType.Add(ColumnDataType.Boolean);
                fieldDataType.Add(ColumnDataType.Numeric);
                //fieldDataType.Add(ColumnDataType.Text);
                fieldDataType.Add(ColumnDataType.UserDefined);

                return fieldDataType;

            }
        }

        public List<ColumnDataType> GetPrimaryFieldDataType
        {
            get
            {
                List<ColumnDataType> fieldDataType = new List<ColumnDataType>();
                fieldDataType.Add(ColumnDataType.Boolean);
                fieldDataType.Add(ColumnDataType.Numeric);
                fieldDataType.Add(ColumnDataType.Text);
                fieldDataType.Add(ColumnDataType.UserDefined);

                return fieldDataType;
            }

        }

        public List<ColumnDataType> GetWeightDataType
        {
            get
            {
                List<ColumnDataType> weightDataType = new List<ColumnDataType>();
                weightDataType.Add(ColumnDataType.Numeric);
                return weightDataType;
            }
        }

        void FillDropDowns()       //      object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            //  MeansViewModel meansViewModel = (MeansViewModel)sender;
            List<EwavColumn> SourceColumns =
                applicationViewModel.EwavSelectedDatasource.AllColumns;

            cbxField.SelectedIndex = 0;

            //List<ColumnDataType> columnDataType = new List<ColumnDataType>();
            //columnDataType.Add(ColumnDataType.Boolean);
            //columnDataType.Add(ColumnDataType.Numeric);
            //columnDataType.Add(ColumnDataType.UserDefined);

            //ColumnDataType test = ColumnDataType.Boolean | ColumnDataType.Numeric | ColumnDataType.DateTime;


            //IEnumerable<EwavColumn> CBXFieldCols = from cols in SourceColumns
            //                                       where test.HasFlag(cols.SqlDataTypeAsString)        //    columnDataType.Contains(cols.SqlDataTypeAsString)
            //                                       orderby cols.Name
            //                                       select cols;




            IEnumerable<EwavColumn> CBXFieldCols = from cols in SourceColumns
                                                   where GetFieldDataType.Contains(cols.SqlDataTypeAsString)
                                                   orderby cols.Name
                                                   select cols;


            List<EwavColumn> colsList = CBXFieldCols.ToList();

            colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            cbxField.ItemsSource = colsList;
            cbxField.SelectedValue = "Index";
            cbxField.DisplayMemberPath = "Name";
            cbxField.SelectedIndex = FieldSelectedIndex1;

            //============================================

            //columnDataType.Clear();
            //columnDataType.Add(ColumnDataType.Numeric);

            CBXFieldCols = from cols in SourceColumns
                           where GetWeightDataType.Contains(cols.SqlDataTypeAsString)
                           orderby cols.Name
                           select cols;
            colsList = CBXFieldCols.ToList();

            colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            cbxFieldWeight.ItemsSource = colsList;
            cbxFieldWeight.SelectedValue = "Index";
            cbxFieldWeight.DisplayMemberPath = "Name";
            cbxFieldWeight.SelectedIndex = WeightSelectedIndex2;
            //=============================================

            //columnDataType.Clear();
            //columnDataType.Add(ColumnDataType.Boolean);
            //columnDataType.Add(ColumnDataType.Numeric);
            //columnDataType.Add(ColumnDataType.Text);
            //columnDataType.Add(ColumnDataType.UserDefined);

            CBXFieldCols = from cols in SourceColumns
                           where GetPrimaryFieldDataType.Contains(cols.SqlDataTypeAsString)
                           orderby cols.Name
                           select cols;
            colsList = CBXFieldCols.ToList();

            colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            cbxFieldStrata.ItemsSource = colsList;
            cbxFieldStrata.SelectedValue = "Index";
            cbxFieldStrata.DisplayMemberPath = "Name";
            cbxFieldStrata.SelectedIndex = StrataSelectedIndex4;

            //columnDataType.Clear();
            //columnDataType.Add(ColumnDataType.Boolean);
            //columnDataType.Add(ColumnDataType.Numeric);
            //columnDataType.Add(ColumnDataType.Text);
            //columnDataType.Add(ColumnDataType.UserDefined);

            CBXFieldCols = from cols in SourceColumns
                           where GetPrimaryFieldDataType.Contains(cols.SqlDataTypeAsString)
                           orderby cols.Name
                           select cols;
            colsList = CBXFieldCols.ToList();

            colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            cbxFieldCrosstab.ItemsSource = colsList;
            cbxFieldCrosstab.SelectedValue = "Index";
            cbxFieldCrosstab.DisplayMemberPath = "Name";
            cbxFieldCrosstab.SelectedIndex = CrossTabSelectedIndex3;
        }

        void meansViewModel_FrequencyTableLoadedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            MeansViewModel meansViewModel = (MeansViewModel)sender;

            List<FrequencyResultData> notConvertedTable = meansViewModel.FrequencyTable;
            Dictionary<List<DTO.EwavFrequencyControlDto>, List<DescriptiveStatistics>> Data = new Dictionary<List<DTO.EwavFrequencyControlDto>, List<DescriptiveStatistics>>();
            for (int i = 0; i < notConvertedTable.Count; i++)
            {
                Data.Add(notConvertedTable[i].FrequencyControlDtoList, notConvertedTable[i].DescriptiveStatisticsList);
            }
            DoWork(Data, gadgetOptions);
        }

        #endregion // Constructors

        #region Event Handlers

        /// <summary>
        /// Handles the check / unchecked events
        /// </summary>
        /// <param name="sender">Object that fired the event</param>
        /// <param name="e">.NET supplied event parameters</param>
        private void checkboxCheckChanged(object sender, RoutedEventArgs e)
        {
            RefreshResults();
        }

        /// <summary>
        /// Fired when the user changes a field selection
        /// </summary>
        /// <param name="sender">Object that fired the event</param>
        /// <param name="e">.NET supplied event parameters</param>
        void cbxField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DoMeans();
        }

        /// <summary>
        /// Validates the inputs in combo boxes.  
        ///  
        /// the test for > 0 is for user def filed support     
        /// 
        /// </summary>
        private void ValidateInput()
        {
            Dictionary<string, string> dictString = new Dictionary<string, string>();

            if (((EwavColumn)cbxField.SelectedItem) != null &&
                cbxField.SelectedIndex > 0)
            {
                dictString.Add(((EwavColumn)cbxField.SelectedItem).Name.ToString(), "cbxField");
            }
            if (((EwavColumn)cbxFieldWeight.SelectedItem) != null &&
                cbxFieldWeight.SelectedIndex > 0)
            {
                dictString.Add(((EwavColumn)cbxFieldWeight.SelectedItem).Name.ToString(), "cbxFieldWeight");
            }
            if (((EwavColumn)cbxFieldCrosstab.SelectedItem) != null &&
                cbxFieldCrosstab.SelectedIndex > 0)
            {
                dictString.Add(((EwavColumn)cbxFieldCrosstab.SelectedItem).Name.ToString(), "cbxFieldCrosstab");
            }
            if (((EwavColumn)cbxFieldStrata.SelectedItem) != null &&
                cbxFieldStrata.SelectedIndex > 0)
            {
                dictString.Add(((EwavColumn)cbxFieldStrata.SelectedItem).Name.ToString(), "cbxFieldStrata");
            }
        }

        /// <summary>
        /// Handles the DoWorker event for the worker
        /// </summary>
        /// <param name="sender">Object that fired the event</param>
        /// <param name="e">.NET supplied event parameters</param>
        private void DoWork(Dictionary<List<DTO.EwavFrequencyControlDto>, List<DescriptiveStatistics>> listDto, GadgetParameters gadgetParameters)
        {
            SetGadgetToProcessingState();
            ClearResults();

            string meansVar = gadgetOptions.MainVariableName;
            string weightVar = gadgetOptions.WeightVariableName;
            string strataVar = string.Empty;
            bool hasData = false;
            if (gadgetOptions.StrataVariableList != null && gadgetOptions.StrataVariableList.Count > 0)
            {
                strataVar = gadgetOptions.StrataVariableList[0].VarName;
            }

            bool showAnova = true;
            if (gadgetOptions.InputVariableList.ContainsKey("showanova"))
            {
                bool.TryParse(gadgetOptions.InputVariableList["showanova"], out showAnova);
            }

            string crosstabVar = gadgetOptions.CrosstabVariableName;

            List<string> stratas = new List<string>();
            if (!string.IsNullOrEmpty(strataVar))
            {
                stratas.Add(strataVar);
            }
            try
            {
                RequestUpdateStatusDelegate requestUpdateStatus = new RequestUpdateStatusDelegate(RequestUpdateStatusMessage);

                Dictionary<List<DTO.EwavFrequencyControlDto>, List<DescriptiveStatistics>> stratifiedFrequencyTables = listDto;

                if (stratifiedFrequencyTables == null || stratifiedFrequencyTables.Count == 0)
                {
                    RenderFinishWithError(SharedStrings.NO_RECORDS_SELECTED);
                }
                else
                {
                    string formatString = string.Empty;

                    foreach (KeyValuePair<List<DTO.EwavFrequencyControlDto>, List<DescriptiveStatistics>> tableKvp in stratifiedFrequencyTables)
                    {
                        string strataValue = tableKvp.Key[0].NameOfDtoList;

                        double count = 0;
                        foreach (DescriptiveStatistics ds in tableKvp.Value)
                        {
                            count = count + ds.Observations;
                        }

                        if (count == 0)
                        {
                            continue;
                        }
                        List<DTO.EwavFrequencyControlDto> frequencies = tableKvp.Key; //dashboardHelper.GenerateFrequencyTable(freqVar, weightVar, out count);

                        if (frequencies == null)
                        {
                            continue;
                        }

                        AddMeansGrid(strataVar, frequencies[0].NameOfDtoList);

                        if (showAnova && tableKvp.Value[0].AnovaPValue.HasValue)
                        {
                            AddAnova(strataValue, tableKvp.Value[0]);
                        }
                    }

                    if (GadgetStatusUpdate != null)
                    {
                        GadgetStatusUpdate(SharedStrings.DASHBOARD_GADGET_STATUS_DISPLAYING_OUTPUT);
                    }

                    int dtoCount = 0;
                    foreach (KeyValuePair<List<DTO.EwavFrequencyControlDto>, List<DescriptiveStatistics>> tableKvp in stratifiedFrequencyTables)
                    {
                        string strataValue = tableKvp.Key[0].NameOfDtoList;

                        double count = 0;
                        foreach (DescriptiveStatistics ds in tableKvp.Value)
                        {
                            count = count + ds.Observations;
                        }

                        if (count == 0)
                        {
                            continue;
                        }
                        else
                        {
                            hasData = true;
                        }

                        List<DTO.EwavFrequencyControlDto> frequencies = tableKvp.Key; //dashboardHelper.GenerateFrequencyTable(freqVar, weightVar, out count);                    

                        if (frequencies.Count == 0)
                        {
                            continue;
                        }

                        string tableHeading = tableKvp.Key[0].NameOfDtoList;

                        if (!string.IsNullOrEmpty(crosstabVar))
                        {
                            tableHeading = string.Format("{0} * {1}", meansVar, crosstabVar);// +": " + frequencies.TableName;
                        }

                        RenderMeansHeader(strataValue, tableHeading, crosstabVar);
                        int rowCount = 1;

                        #region "Option1 "
                        int crossTabCount = 1;
                        if (crossTable != null && !string.IsNullOrEmpty(gadgetOptions.CrosstabVariableName))
                        {
                            crossTabCount = crossTable[dtoCount].ColumnNames.ToList<string>().Count;
                        }
                        else
                        {
                            crossTable = null;
                        }
                        #endregion

                        for (int i = 0; i < crossTabCount; i++)
                        {
                            string columnName = "";
                            if (crossTable != null)
                            {
                                columnName = crossTable[dtoCount].ColumnNames.ToList<string>()[i];
                            }
                            else
                            {
                                columnName = "freq";
                            }

                            if (columnName.Equals("___sortvalue___"))
                            {
                                continue;
                            }

                            // 
                            DescriptiveStatistics means;
                            if (crossTable != null)
                            {
                                means = crossTable[dtoCount].DsList[rowCount - 1];
                            }
                            else
                            {
                                means = tableKvp.Value[i];
                            }
                            if (!columnName.ToLower().Equals(meansVar.ToLower()))
                            {
                                AddGridRow(strataValue, 30);
                                string displayValue = meansVar;

                                if (crossTable != null && crossTable[dtoCount].DsList.Count > 1)
                                {
                                    displayValue = columnName;
                                }

                                string statsObs = string.Format(" {0} ", means.Observations.ToString("F4"));
                                string statsSum = SharedStrings.UNDEFINED;
                                string statsMean = SharedStrings.UNDEFINED;
                                string statsVar = SharedStrings.UNDEFINED;
                                string statsStdDev = SharedStrings.UNDEFINED;
                                string statsMin = SharedStrings.UNDEFINED;
                                string statsMax = SharedStrings.UNDEFINED;
                                string statsMedian = SharedStrings.UNDEFINED;
                                string statsMode = SharedStrings.UNDEFINED;
                                string statsQ1 = SharedStrings.UNDEFINED;
                                string statsQ3 = SharedStrings.UNDEFINED;

                                if (means.Sum != null)
                                {
                                    statsSum = string.Format(" {0} ", ((double)means.Sum).ToString("F4"));
                                }
                                if (means.Mean != null)
                                {
                                    statsMean = string.Format(" {0} ", ((double)means.Mean).ToString("F4"));
                                }
                                if (means.Variance != null)
                                {
                                    statsVar = string.Format(" {0} ", ((double)means.Variance).ToString("F4"));
                                }
                                if (means.StdDev != null)
                                {
                                    statsStdDev = string.Format(" {0} ", ((double)means.StdDev).ToString("F4"));
                                }
                                if (means.Min != null)
                                {
                                    statsMin = string.Format(" {0} ", ((double)means.Min).ToString("F4"));
                                }
                                if (means.Q1 != null)
                                {
                                    statsQ1 = string.Format(" {0} ", ((double)means.Q1).ToString("F4"));
                                }
                                if (means.Median != null)
                                {
                                    statsMedian = string.Format(" {0} ", ((double)means.Median).ToString("F4"));
                                }
                                if (means.Q3 != null)
                                {
                                    statsQ3 = string.Format(" {0} ", ((double)means.Q3).ToString("F4"));
                                }
                                if (means.Max != null)
                                {
                                    statsMax = string.Format(" {0} ", ((double)means.Max).ToString("F4"));
                                }
                                if (means.Mode != null)
                                {
                                    statsMode = string.Format(" {0} ", ((double)means.Mode).ToString("F4"));
                                }

                                if (statsObs.EndsWith("0000 "))
                                {
                                    statsObs = string.Format(" {0} ", means.Observations.ToString());
                                }

                                if (statsSum.EndsWith("0000 "))
                                {
                                    statsSum = string.Format(" {0} ", ((double)means.Sum).ToString());
                                }

                                SetGridText(strataValue, new TextBlockConfig(displayValue, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Left, rowCount, 0));
                                SetGridText(strataValue, new TextBlockConfig(statsObs, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 1));

                                SetGridText(strataValue, new TextBlockConfig(statsSum, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 2));
                                SetGridText(strataValue, new TextBlockConfig(statsMean, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 3));

                                SetGridText(strataValue, new TextBlockConfig(statsVar, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 4));
                                SetGridText(strataValue, new TextBlockConfig(statsStdDev, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 5));
                                SetGridText(strataValue, new TextBlockConfig(statsMin, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 6));
                                SetGridText(strataValue, new TextBlockConfig(statsQ1, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 7));
                                SetGridText(strataValue, new TextBlockConfig(statsMedian, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 8));
                                SetGridText(strataValue, new TextBlockConfig(statsQ3, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 9));
                                SetGridText(strataValue, new TextBlockConfig(statsMax, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 10));
                                SetGridText(strataValue, new TextBlockConfig(statsMode, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 11));

                                rowCount++;
                            }
                        }
                        DrawMeansBorders(strataValue);
                        dtoCount++;
                    }
                }

                if (!hasData)
                {
                    RenderFinishWithError(SharedStrings.NO_RECORDS_SELECTED);
                }
                else
                {
                    RenderFinish();
                }
                SetGadgetToFinishedState();
            }
            catch (Exception ex)
            {
                RenderFinishWithError(ex.Message);
                SetGadgetToFinishedState();
            }
        }

        #endregion

        #region Public Methods
        #endregion

        #region Private Methods

        /// <summary>
        /// Clears the gadget's output
        /// </summary>
        private void ClearResults()
        {
            txtStatus.Text = string.Empty;
            pnlStatus.Visibility = Visibility.Collapsed;
            //waitCursor.Visibility = Visibility.Visible;

            foreach (Grid grid in strataGridList)
            {
                grid.Children.Clear();
                grid.RowDefinitions.Clear();
                panelMain.Children.Remove(grid);
            }

            foreach (TextBlock textBlock in gridLabelsList)
            {
                panelMain.Children.Remove(textBlock);
            }

            foreach (StackPanel anovaBlock in anovaBlocks)
            {
                panelMain.Children.Remove(anovaBlock);
            }

            strataGridList.Clear();
            anovaBlocks.Clear();
            grdMeans.Visibility = System.Windows.Visibility.Collapsed;
            grdMeans.Children.Clear();
            grdMeans.RowDefinitions.Clear();
        }

        /// <summary>
        /// Handles the filling of the gadget's combo boxes
        /// </summary>
        //private void FillComboboxes(bool update = false)
        //{
        //    loadingCombos = true;

        //    string prevField = string.Empty;
        //    string prevWeightField = string.Empty;
        //    string prevStrataField = string.Empty;
        //    string prevCrosstabField = string.Empty;

        //    if (update)
        //    {
        //        if (cbxField.SelectedIndex >= 0)
        //        {
        //            prevField = ((EwavColumn)cbxField.SelectedItem).Name.ToString();
        //        }
        //        if (cbxFieldWeight.SelectedIndex >= 0)
        //        {
        //            prevWeightField = ((EwavColumn)cbxFieldWeight.SelectedItem).Name.ToString();
        //        }
        //        if (cbxFieldStrata.SelectedIndex >= 0)
        //        {
        //            prevStrataField = ((EwavColumn)cbxFieldStrata.SelectedItem).Name.ToString();
        //        }
        //        if (cbxFieldCrosstab.SelectedIndex >= 0)
        //        {
        //            prevCrosstabField = cbxFieldCrosstab.SelectedItem.ToString();
        //        }
        //    }

        //    cbxField.ItemsSource = null;
        //    cbxField.Items.Clear();

        //    cbxFieldWeight.ItemsSource = null;
        //    cbxFieldWeight.Items.Clear();

        //    cbxFieldStrata.ItemsSource = null;
        //    cbxFieldStrata.Items.Clear();

        //    cbxFieldCrosstab.ItemsSource = null;
        //    cbxFieldCrosstab.Items.Clear();

        //    List<string> fieldNames = new List<string>();
        //    List<string> weightFieldNames = new List<string>();
        //    List<string> strataFieldNames = new List<string>();
        //    List<string> crosstabFieldNames = new List<string>();

        //    fieldNames.Add(string.Empty);
        //    weightFieldNames.Add(string.Empty);
        //    strataFieldNames.Add(string.Empty);
        //    crosstabFieldNames.Add(string.Empty);

        //    ColumnDataType columnDataType = ColumnDataType.Boolean | ColumnDataType.Numeric | ColumnDataType.UserDefined;
        //    //fieldNames = dashboardHelper.GetFieldsAsList(columnDataType);

        //    columnDataType = ColumnDataType.Numeric | ColumnDataType.UserDefined;
        //    //weightFieldNames.AddRange(dashboardHelper.GetFieldsAsList(columnDataType));

        //    columnDataType = ColumnDataType.Numeric | ColumnDataType.Boolean | ColumnDataType.Text | ColumnDataType.UserDefined;
        //    //strataFieldNames.AddRange(dashboardHelper.GetFieldsAsList(columnDataType));

        //    columnDataType = ColumnDataType.Numeric | ColumnDataType.Boolean | ColumnDataType.Text | ColumnDataType.UserDefined;
        //    //crosstabFieldNames.AddRange(dashboardHelper.GetFieldsAsList(columnDataType));

        //    fieldNames.Sort();
        //    weightFieldNames.Sort();
        //    strataFieldNames.Sort();
        //    crosstabFieldNames.Sort();

        //    cbxField.ItemsSource = fieldNames;
        //    //    cbxField.SelectedIndex = Index1;

        //    cbxFieldWeight.ItemsSource = weightFieldNames;

        //    cbxFieldStrata.ItemsSource = strataFieldNames;

        //    cbxFieldCrosstab.ItemsSource = crosstabFieldNames;

        //    if (cbxField.Items.Count > 0)
        //    {
        //        cbxField.SelectedIndex = -1;
        //    }
        //    if (cbxFieldWeight.Items.Count > 0)
        //    {
        //        cbxFieldWeight.SelectedIndex = -1;
        //    }
        //    if (cbxFieldStrata.Items.Count > 0)
        //    {
        //        cbxFieldStrata.SelectedIndex = -1;
        //    }
        //    if (cbxFieldCrosstab.Items.Count > 0)
        //    {
        //        cbxFieldCrosstab.SelectedIndex = -1;
        //    }

        //    if (update)
        //    {
        //        ((EwavColumn)cbxField.SelectedItem).Name = prevField;
        //        ((EwavColumn)cbxFieldWeight.SelectedItem).Name = prevWeightField;
        //        ((EwavColumn)cbxFieldStrata.SelectedItem).Name = prevStrataField;
        //        cbxFieldCrosstab.SelectedItem = prevCrosstabField;
        //    }

        //    loadingCombos = false;
        //}

        /// <summary>
        /// Adds anova statistics to the gadget
        /// </summary>
        /// <param name="strataValue">The strata value associated with the results</param>
        /// <param name="stats">The descriptive statistics to process</param>
        private void AddAnova(string strataValue, DescriptiveStatistics stats)
        {
            StackPanel pnl = new StackPanel();
            KeyValuePair<string, DescriptiveStatistics> ANOVAStatistics = new KeyValuePair<string, DescriptiveStatistics>(strataValue, stats);
            pnl.Tag = ANOVAStatistics;
            pnl.HorizontalAlignment = HorizontalAlignment.Center;
            pnl.VerticalAlignment = VerticalAlignment.Center;
            pnl.Margin = new Thickness(5);
            anovaBlocks.Add(pnl);

            TextBlock txt1 = new TextBlock();
            txt1.Text = "ANOVA, a Parametric Test for Inequality of Population Means";
            txt1.HorizontalAlignment = HorizontalAlignment.Center;
            txt1.Margin = new Thickness(5);
            txt1.FontWeight = FontWeights.Bold;
            pnl.Children.Add(txt1);

            TextBlock txt2 = new TextBlock();
            txt2.Text = "(For normally distributed data only)";
            txt2.HorizontalAlignment = HorizontalAlignment.Center;
            pnl.Children.Add(txt2);

            Grid grid1 = new Grid();
            grid1.HorizontalAlignment = HorizontalAlignment.Center;
            grid1.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            grid1.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            grid1.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            grid1.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            grid1.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid1.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid1.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid1.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid1.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            pnl.Children.Add(grid1);

            for (int y = 0; y < grid1.ColumnDefinitions.Count; y++)
            {
                Rectangle rctHeader = new Rectangle();
                //rctHeader.Fill = new SolidColorBrush(SystemColors.HighlightColor);  //SystemColors.MenuHighlightBrush;
                rctHeader.Style = Application.Current.Resources["HeaderCell"] as Style;
                Grid.SetRow(rctHeader, 0);
                Grid.SetColumn(rctHeader, y);
                grid1.Children.Add(rctHeader);
            }

            for (int y = 1; y < grid1.RowDefinitions.Count; y++)
            {
                Rectangle rctHeader = new Rectangle();
                //rctHeader.Fill = new SolidColorBrush(SystemColors.HighlightColor);  //SystemColors.MenuHighlightBrush;
                rctHeader.Style = Application.Current.Resources["HeaderCell"] as Style;
                Grid.SetRow(rctHeader, y);
                Grid.SetColumn(rctHeader, 0);
                grid1.Children.Add(rctHeader);
            }

            TextBlock lblVariation = new TextBlock();
            lblVariation.Text = "Variation";
            lblVariation.Style = Application.Current.Resources["HeaderFont"] as Style;
            grid1.Children.Add(lblVariation);
            Grid.SetRow(lblVariation, 0);
            Grid.SetColumn(lblVariation, 0);

            TextBlock lblBetween = new TextBlock();
            lblBetween.Text = "Between";
            lblBetween.Style = Application.Current.Resources["HeaderFont"] as Style;
            grid1.Children.Add(lblBetween);
            Grid.SetRow(lblBetween, 1);
            Grid.SetColumn(lblBetween, 0);

            TextBlock lblWithin = new TextBlock();
            lblWithin.Text = "Within";
            lblWithin.Style = Application.Current.Resources["HeaderFont"] as Style;
            grid1.Children.Add(lblWithin);
            Grid.SetRow(lblWithin, 2);
            Grid.SetColumn(lblWithin, 0);

            TextBlock lblTotal = new TextBlock();
            lblTotal.Text = "Total";
            lblTotal.Style = Application.Current.Resources["HeaderFont"] as Style;
            grid1.Children.Add(lblTotal);
            Grid.SetRow(lblTotal, 3);
            Grid.SetColumn(lblTotal, 0);

            TextBlock lblSS = new TextBlock();
            lblSS.Text = "SS";
            lblSS.Style = Application.Current.Resources["HeaderFont"] as Style;
            grid1.Children.Add(lblSS);
            Grid.SetRow(lblSS, 0);
            Grid.SetColumn(lblSS, 1);

            TextBlock lblDf = new TextBlock();
            lblDf.Text = "dF";
            lblDf.Style = Application.Current.Resources["HeaderFont"] as Style;
            grid1.Children.Add(lblDf);
            Grid.SetRow(lblDf, 0);
            Grid.SetColumn(lblDf, 2);

            TextBlock lblMS = new TextBlock();
            lblMS.Text = "MS";
            lblMS.Style = Application.Current.Resources["HeaderFont"] as Style;
            grid1.Children.Add(lblMS);
            Grid.SetRow(lblMS, 0);
            Grid.SetColumn(lblMS, 3);

            TextBlock lblF = new TextBlock();
            lblF.Text = "F-Statistic";
            lblF.Style = Application.Current.Resources["HeaderFont"] as Style;
            grid1.Children.Add(lblF);
            Grid.SetRow(lblF, 0);
            Grid.SetColumn(lblF, 4);

            TextBlock txtSSBetween = new TextBlock();
            txtSSBetween.Text = stats.SsBetween.Value.ToString("N4");
            txtSSBetween.Margin = new Thickness(4, 0, 4, 0);
            txtSSBetween.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            txtSSBetween.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            grid1.Children.Add(txtSSBetween);
            Grid.SetRow(txtSSBetween, 1);
            Grid.SetColumn(txtSSBetween, 1);

            TextBlock txtSSWithin = new TextBlock();
            txtSSWithin.Text = stats.SsWithin.Value.ToString("N4");
            txtSSWithin.Margin = new Thickness(4, 0, 4, 0);
            txtSSWithin.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            txtSSWithin.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            grid1.Children.Add(txtSSWithin);
            Grid.SetRow(txtSSWithin, 2);
            Grid.SetColumn(txtSSWithin, 1);

            TextBlock txtSSTotal = new TextBlock();
            txtSSTotal.Text = (stats.SsWithin.Value + stats.SsBetween.Value).ToString("N4");
            txtSSTotal.Margin = new Thickness(4, 0, 4, 0);
            txtSSTotal.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            txtSSTotal.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            grid1.Children.Add(txtSSTotal);
            Grid.SetRow(txtSSTotal, 3);
            Grid.SetColumn(txtSSTotal, 1);

            TextBlock txtDFBetween = new TextBlock();
            txtDFBetween.Text = stats.DfBetween.Value.ToString();
            txtDFBetween.Margin = new Thickness(4, 0, 4, 0);
            txtDFBetween.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            txtDFBetween.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            grid1.Children.Add(txtDFBetween);
            Grid.SetRow(txtDFBetween, 1);
            Grid.SetColumn(txtDFBetween, 2);

            TextBlock txtDFWithin = new TextBlock();
            txtDFWithin.Text = stats.DfWithin.Value.ToString();
            txtDFWithin.Margin = new Thickness(4, 0, 4, 0);
            txtDFWithin.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            txtDFWithin.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            grid1.Children.Add(txtDFWithin);
            Grid.SetRow(txtDFWithin, 2);
            Grid.SetColumn(txtDFWithin, 2);

            TextBlock txtDFTotal = new TextBlock();
            txtDFTotal.Text = ((stats.DfWithin.Value + stats.DfBetween.Value).ToString());
            txtDFTotal.Margin = new Thickness(4, 0, 4, 0);
            txtDFTotal.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            txtDFTotal.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            grid1.Children.Add(txtDFTotal);
            Grid.SetRow(txtDFTotal, 3);
            Grid.SetColumn(txtDFTotal, 2);

            TextBlock txtMSBetween = new TextBlock();
            txtMSBetween.Text = stats.MsBetween.Value.ToString("N4");
            txtMSBetween.Margin = new Thickness(4, 0, 4, 0);
            txtMSBetween.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            txtMSBetween.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            grid1.Children.Add(txtMSBetween);
            Grid.SetRow(txtMSBetween, 1);
            Grid.SetColumn(txtMSBetween, 3);

            TextBlock txtMSWithin = new TextBlock();
            txtMSWithin.Text = stats.MsWithin.Value.ToString("N4");
            txtMSWithin.Margin = new Thickness(4, 0, 4, 0);
            txtMSWithin.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            txtMSWithin.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            grid1.Children.Add(txtMSWithin);
            Grid.SetRow(txtMSWithin, 2);
            Grid.SetColumn(txtMSWithin, 3);

            TextBlock txtFStat = new TextBlock();
            txtFStat.Text = stats.FStatistic.Value.ToString("N4");
            txtFStat.Margin = new Thickness(4, 0, 4, 0);
            txtFStat.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            txtFStat.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            grid1.Children.Add(txtFStat);
            Grid.SetRow(txtFStat, 1);
            Grid.SetColumn(txtFStat, 4);

            int rdcount = 0;
            foreach (RowDefinition rd in grid1.RowDefinitions)
            {
                int cdcount = 0;
                foreach (ColumnDefinition cd in grid1.ColumnDefinitions)
                {
                    Rectangle rctBorder = new Rectangle();
                    rctBorder.Style = Application.Current.Resources["DataCell"] as Style;
                    Grid.SetRow(rctBorder, rdcount);
                    Grid.SetColumn(rctBorder, cdcount);
                    grid1.Children.Add(rctBorder);
                    cdcount++;
                }
                rdcount++;
            }

            Grid grid2 = new Grid();
            grid2.Margin = new Thickness(5);
            grid2.HorizontalAlignment = HorizontalAlignment.Center;
            grid2.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            grid2.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(130) });
            grid2.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(95) });
            pnl.Children.Add(grid2);

            for (int y = 0; y < grid2.RowDefinitions.Count; y++)
            {
                Rectangle rctHeader = new Rectangle();
                //rctHeader.Fill = new SolidColorBrush(SystemColors.HighlightColor);  //SystemColors.MenuHighlightBrush;
                rctHeader.Style = Application.Current.Resources["HeaderCell"] as Style;
                Grid.SetRow(rctHeader, y);
                Grid.SetColumn(rctHeader, 0);
                grid2.Children.Add(rctHeader);
            }

            TextBlock lblPValue = new TextBlock();
            lblPValue.Text = "P-Value";
            lblPValue.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(lblPValue, 0);
            Grid.SetColumn(lblPValue, 0);
            grid2.Children.Add(lblPValue);

            TextBlock txtPValue = new TextBlock();
            txtPValue.Text = stats.AnovaPValue.Value.ToString("N4");
            txtPValue.HorizontalAlignment = HorizontalAlignment.Center;
            txtPValue.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(txtPValue, 0);
            Grid.SetColumn(txtPValue, 1);
            grid2.Children.Add(txtPValue);

            foreach (RowDefinition rd in grid2.RowDefinitions)
            {
                int cdcount = 0;
                foreach (ColumnDefinition cd in grid2.ColumnDefinitions)
                {
                    Rectangle rctBorder = new Rectangle();
                    rctBorder.Style = Application.Current.Resources["DataCell"] as Style;
                    Grid.SetRow(rctBorder, rdcount);
                    Grid.SetColumn(rctBorder, cdcount);
                    grid2.Children.Add(rctBorder);
                    cdcount++;
                }
                rdcount++;
            }

            TextBlock txt3 = new TextBlock();
            txt3.Text = "Bartlett's Test for Inequality of Population Variances";
            txt3.HorizontalAlignment = HorizontalAlignment.Center;
            txt3.Margin = new Thickness(5);
            txt3.FontWeight = FontWeights.Bold;
            pnl.Children.Add(txt3);

            Grid grid3 = new Grid();
            grid3.Margin = new Thickness(5);
            grid3.HorizontalAlignment = HorizontalAlignment.Center;
            grid3.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            grid3.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            grid3.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            grid3.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(130) });
            grid3.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(95) });
            pnl.Children.Add(grid3);

            for (int y = 0; y < grid3.RowDefinitions.Count; y++)
            {
                Rectangle rctHeader = new Rectangle();
                //rctHeader.Fill = new SolidColorBrush(SystemColors.HighlightColor);  //SystemColors.MenuHighlightBrush;
                rctHeader.Style = Application.Current.Resources["HeaderCell"] as Style;
                Grid.SetRow(rctHeader, y);
                Grid.SetColumn(rctHeader, 0);
                grid3.Children.Add(rctHeader);
            }

            TextBlock lblBartlettChi = new TextBlock();
            lblBartlettChi.Text = "Chi Square";
            lblBartlettChi.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(lblBartlettChi, 0);
            Grid.SetColumn(lblBartlettChi, 0);
            grid3.Children.Add(lblBartlettChi);

            TextBlock lblBartlettDf = new TextBlock();
            lblBartlettDf.Text = "Degrees of freedom";
            lblBartlettDf.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(lblBartlettDf, 1);
            Grid.SetColumn(lblBartlettDf, 0);
            grid3.Children.Add(lblBartlettDf);

            TextBlock lblBartlettP = new TextBlock();
            lblBartlettP.Text = "P-Value";
            lblBartlettP.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(lblBartlettP, 2);
            Grid.SetColumn(lblBartlettP, 0);
            grid3.Children.Add(lblBartlettP);

            TextBlock txtBartlettChi = new TextBlock();
            txtBartlettChi.Text = stats.ChiSquare.Value.ToString("N4");
            txtBartlettChi.HorizontalAlignment = HorizontalAlignment.Center;
            txtBartlettChi.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(txtBartlettChi, 0);
            Grid.SetColumn(txtBartlettChi, 1);
            grid3.Children.Add(txtBartlettChi);

            TextBlock txtBartlettDf = new TextBlock();
            txtBartlettDf.Text = stats.DfBetween.Value.ToString();
            txtBartlettDf.HorizontalAlignment = HorizontalAlignment.Center;
            txtBartlettDf.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(txtBartlettDf, 1);
            Grid.SetColumn(txtBartlettDf, 1);
            grid3.Children.Add(txtBartlettDf);

            TextBlock txtBartlettP = new TextBlock();
            txtBartlettP.Text = stats.BartlettPValue.Value.ToString("N4");
            txtBartlettP.HorizontalAlignment = HorizontalAlignment.Center;
            txtBartlettP.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(txtBartlettP, 2);
            Grid.SetColumn(txtBartlettP, 1);
            grid3.Children.Add(txtBartlettP);

            rdcount = 0;
            foreach (RowDefinition rd in grid3.RowDefinitions)
            {
                int cdcount = 0;
                foreach (ColumnDefinition cd in grid3.ColumnDefinitions)
                {
                    Rectangle rctBorder = new Rectangle();
                    rctBorder.Style = Application.Current.Resources["DataCell"] as Style;
                    Grid.SetRow(rctBorder, rdcount);
                    Grid.SetColumn(rctBorder, cdcount);
                    grid3.Children.Add(rctBorder);
                    cdcount++;
                }
                rdcount++;
            }

            TextBlock txt4 = new TextBlock();
            txt4.Text = "A small p-value (e.g., less than 0.05) suggests that the variances are not homogeneous and that the ANOVA may not be appropriate.";
            txt4.HorizontalAlignment = HorizontalAlignment.Center;
            txt4.Margin = new Thickness(5);
            pnl.Children.Add(txt4);

            TextBlock txt5 = new TextBlock();
            txt5.Text = "Mann-Whitney/Wilcoxon Two-Sample Test (Kruskal-Wallis test for two groups)";
            txt5.HorizontalAlignment = HorizontalAlignment.Center;
            txt5.Margin = new Thickness(5);
            txt5.FontWeight = FontWeights.Bold;
            pnl.Children.Add(txt5);

            Grid grid4 = new Grid();
            grid4.Margin = new Thickness(5);
            grid4.HorizontalAlignment = HorizontalAlignment.Center;
            grid4.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            grid4.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            grid4.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            grid4.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(130) });
            grid4.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(95) });
            pnl.Children.Add(grid4);

            for (int y = 0; y < grid4.RowDefinitions.Count; y++)
            {
                Rectangle rctHeader = new Rectangle();
                //rctHeader.Fill = new SolidColorBrush(SystemColors.HighlightColor);  //SystemColors.MenuHighlightBrush;
                rctHeader.Style = Application.Current.Resources["HeaderCell"] as Style;
                Grid.SetRow(rctHeader, y);
                Grid.SetColumn(rctHeader, 0);
                grid4.Children.Add(rctHeader);
            }

            TextBlock lblKWChi = new TextBlock();
            lblKWChi.Text = "Kruskal-Wallis H";
            lblKWChi.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(lblKWChi, 0);
            Grid.SetColumn(lblKWChi, 0);
            grid4.Children.Add(lblKWChi);

            TextBlock lblKWDf = new TextBlock();
            lblKWDf.Text = "Degrees of freedom";
            lblKWDf.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(lblKWDf, 1);
            Grid.SetColumn(lblKWDf, 0);
            grid4.Children.Add(lblKWDf);

            TextBlock lblKWP = new TextBlock();
            lblKWP.Text = "P-Value";
            lblKWP.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(lblKWP, 2);
            Grid.SetColumn(lblKWP, 0);
            grid4.Children.Add(lblKWP);

            TextBlock txtKWChi = new TextBlock();
            txtKWChi.Text = stats.KruskalWallisH.Value.ToString("N4");
            txtKWChi.HorizontalAlignment = HorizontalAlignment.Center;
            txtKWChi.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(txtKWChi, 0);
            Grid.SetColumn(txtKWChi, 1);
            grid4.Children.Add(txtKWChi);

            TextBlock txtWKDf = new TextBlock();
            txtWKDf.Text = stats.DfBetween.Value.ToString();
            txtWKDf.HorizontalAlignment = HorizontalAlignment.Center;
            txtWKDf.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(txtWKDf, 1);
            Grid.SetColumn(txtWKDf, 1);
            grid4.Children.Add(txtWKDf);

            TextBlock txtKWP = new TextBlock();
            txtKWP.Text = stats.KruskalPValue.Value.ToString("N4");
            txtKWP.HorizontalAlignment = HorizontalAlignment.Center;
            txtKWP.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(txtKWP, 2);
            Grid.SetColumn(txtKWP, 1);
            grid4.Children.Add(txtKWP);

            rdcount = 0;
            foreach (RowDefinition rd in grid4.RowDefinitions)
            {
                int cdcount = 0;
                foreach (ColumnDefinition cd in grid4.ColumnDefinitions)
                {
                    Rectangle rctBorder = new Rectangle();
                    rctBorder.Style = Application.Current.Resources["DataCell"] as Style;
                    Grid.SetRow(rctBorder, rdcount);
                    Grid.SetColumn(rctBorder, cdcount);
                    grid4.Children.Add(rctBorder);
                    cdcount++;
                }
                rdcount++;
            }

            panelMain.Children.Add(pnl);
        }

        /// <summary>
        /// Used to add a new MEANS grid to the gadget's output
        /// </summary>
        /// <param name="strataVar">The name of the stratification variable selected, if any</param>
        /// <param name="value">The value by which this grid has been stratified by</param>
        private void AddMeansGrid(string strataVar, string value)
        {
            Grid grid = new Grid();
            grid.Tag = value;
            grid.HorizontalAlignment = HorizontalAlignment.Left;
            grid.Margin = new Thickness(0, 0, 0, 0);
            grid.Visibility = System.Windows.Visibility.Collapsed;

            ColumnDefinition column1 = new ColumnDefinition();
            ColumnDefinition column2 = new ColumnDefinition();
            ColumnDefinition column3 = new ColumnDefinition();
            ColumnDefinition column4 = new ColumnDefinition();
            ColumnDefinition column5 = new ColumnDefinition();
            ColumnDefinition column6 = new ColumnDefinition();
            ColumnDefinition column7 = new ColumnDefinition();
            ColumnDefinition column8 = new ColumnDefinition();
            ColumnDefinition column9 = new ColumnDefinition();
            ColumnDefinition column10 = new ColumnDefinition();
            ColumnDefinition column11 = new ColumnDefinition();
            ColumnDefinition column12 = new ColumnDefinition();

            column1.Width = GridLength.Auto; //new GridLength(150);
            column1.MinWidth = 150;

            column2.Width = GridLength.Auto;
            column3.Width = GridLength.Auto;
            column4.Width = GridLength.Auto;
            column5.Width = GridLength.Auto;
            column6.Width = GridLength.Auto;
            column7.Width = GridLength.Auto;
            column8.Width = GridLength.Auto;
            column9.Width = GridLength.Auto;
            column10.Width = GridLength.Auto;
            column11.Width = GridLength.Auto;
            column12.Width = GridLength.Auto;

            grid.ColumnDefinitions.Add(column1);
            grid.ColumnDefinitions.Add(column2);
            grid.ColumnDefinitions.Add(column3);
            grid.ColumnDefinitions.Add(column4);
            grid.ColumnDefinitions.Add(column5);
            grid.ColumnDefinitions.Add(column6);
            grid.ColumnDefinitions.Add(column7);
            grid.ColumnDefinitions.Add(column8);
            grid.ColumnDefinitions.Add(column9);
            grid.ColumnDefinitions.Add(column10);
            grid.ColumnDefinitions.Add(column11);
            grid.ColumnDefinitions.Add(column12);

            TextBlock txtGridLabel = new TextBlock();
            txtGridLabel.Text = value;
            txtGridLabel.HorizontalAlignment = HorizontalAlignment.Left;
            txtGridLabel.VerticalAlignment = VerticalAlignment.Bottom;
            txtGridLabel.FontWeight = FontWeights.Bold;
            if (string.IsNullOrEmpty(strataVar))
            {
                txtGridLabel.Margin = new Thickness(2, 46, 2, 2);
                txtGridLabel.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                if (strataGridList.Count < 1)
                {
                    txtGridLabel.Margin = new Thickness(2, 0, 2, 2);
                }
                else
                {
                    txtGridLabel.Margin = new Thickness(2, 54, 2, 2);
                }
            }
            gridLabelsList.Add(txtGridLabel);
            panelMain.Children.Add(txtGridLabel);

            panelMain.Children.Add(grid);
            strataGridList.Add(grid);
        }

        /// <summary>
        /// Gets a Grid from the strataGridList based on a given stratification value
        /// </summary>
        /// <param name="strataValue">The strata value to use in searching the grid list</param>
        /// <returns>The System.Windows.Control.Grid associated with the given strata value</returns>
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

        /// <summary>
        /// Sets the text value of a grid cell in a given output grid
        /// </summary>
        /// <param name="strataValue">
        /// The strata value to which this grid cell belongs; used to search the list of grids and 
        /// return the proper System.Windows.Controls.Grid for text insertion.
        /// </param>
        /// <param name="textBlockConfig">The configuration options for this block of text</param>
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

        /// <summary>
        /// Adds a new row to a given output grid
        /// </summary>
        /// <param name="strataValue">
        /// The strata value to which this grid cell belongs; used to search the list of grids and 
        /// return the proper System.Windows.Controls.Grid for text insertion.
        /// </param>
        /// <param name="height">The desired height of the grid row</param>
        private void AddGridRow(string strataValue, int height)
        {
            Grid grid = GetStrataGrid(strataValue);

            //waitCursor.Visibility = Visibility.Collapsed;//Hidden;
            grid.Visibility = Visibility.Visible;
            RowDefinition rowDef = new RowDefinition();
            rowDef.Height = new GridLength(height);
            grid.RowDefinitions.Add(rowDef);
        }

        /// <summary>
        /// Used to render the header (first row) of a given MEANS output grid
        /// </summary>
        /// <param name="strataValue">
        /// The strata value to which this grid cell belongs; used to search the list of grids and 
        /// return the proper System.Windows.Controls.Grid for text insertion.
        /// </param>
        /// <param name="meansVar">The variable that the statistics were run on</param>
        /// <param name="crosstabVar">The variable used to cross-tabulate by, if any</param>
        private void RenderMeansHeader(string strataValue, string meansVar, string crosstabVar)
        {
            Grid grid = GetStrataGrid(strataValue);

            RowDefinition rowDefHeader = new RowDefinition();
            rowDefHeader.Height = new GridLength(30);
            grid.RowDefinitions.Add(rowDefHeader);

            for (int y = 0; y < grid.ColumnDefinitions.Count; y++)
            {
                Rectangle rctHeader = new Rectangle();
                //rctHeader.Fill = new SolidColorBrush(SystemColors.HighlightColor);  //SystemColors.MenuHighlightBrush;
                rctHeader.Style = Application.Current.Resources["HeaderCell"] as Style;
                Grid.SetRow(rctHeader, 0);
                Grid.SetColumn(rctHeader, y);
                grid.Children.Add(rctHeader);
            }

            TextBlock txtValHeader = new TextBlock();
            if (string.IsNullOrEmpty(crosstabVar))
            {
                txtValHeader.Text = string.Empty;
            }
            else
            {
                txtValHeader.Text = meansVar;
            }
            txtValHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtValHeader, 0);
            Grid.SetColumn(txtValHeader, 0);
            grid.Children.Add(txtValHeader);

            TextBlock txtObservationsHeader = new TextBlock();
            txtObservationsHeader.Text = string.Format(" {0} ", SharedStrings.DASHBOARD_MEANS_OBSERVATIONS);
            txtObservationsHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtObservationsHeader, 0);
            Grid.SetColumn(txtObservationsHeader, 1);
            grid.Children.Add(txtObservationsHeader);

            TextBlock txtTotalHeader = new TextBlock();
            txtTotalHeader.Text = SharedStrings.DASHBOARD_MEANS_TOTAL;
            txtTotalHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtTotalHeader, 0);
            Grid.SetColumn(txtTotalHeader, 2);
            grid.Children.Add(txtTotalHeader);

            TextBlock txtMeanHeader = new TextBlock();
            txtMeanHeader.Text = SharedStrings.DASHBOARD_MEANS_MEAN;
            txtMeanHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtMeanHeader, 0);
            Grid.SetColumn(txtMeanHeader, 3);
            grid.Children.Add(txtMeanHeader);

            TextBlock txtVarianceHeader = new TextBlock();
            txtVarianceHeader.Text = SharedStrings.DASHBOARD_MEANS_VARIANCE;
            txtVarianceHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtVarianceHeader, 0);
            Grid.SetColumn(txtVarianceHeader, 4);
            grid.Children.Add(txtVarianceHeader);

            TextBlock txtStdDevHeader = new TextBlock();
            txtStdDevHeader.Text = SharedStrings.DASHBOARD_MEANS_STANDARD_DEVIATION;
            txtStdDevHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtStdDevHeader, 0);
            Grid.SetColumn(txtStdDevHeader, 5);
            grid.Children.Add(txtStdDevHeader);

            TextBlock txtMinHeader = new TextBlock();
            txtMinHeader.Text = SharedStrings.DASHBOARD_MEANS_MIN;
            txtMinHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtMinHeader, 0);
            Grid.SetColumn(txtMinHeader, 6);
            grid.Children.Add(txtMinHeader);

            TextBlock txt25Header = new TextBlock();
            txt25Header.Text = SharedStrings.DASHBOARD_MEANS_25;
            txt25Header.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txt25Header, 0);
            Grid.SetColumn(txt25Header, 7);
            grid.Children.Add(txt25Header);

            TextBlock txtMedianHeader = new TextBlock();
            txtMedianHeader.Text = SharedStrings.DASHBOARD_MEANS_MEDIAN;
            txtMedianHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtMedianHeader, 0);
            Grid.SetColumn(txtMedianHeader, 8);
            grid.Children.Add(txtMedianHeader);

            TextBlock txt75Header = new TextBlock();
            txt75Header.Text = SharedStrings.DASHBOARD_MEANS_75;
            txt75Header.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txt75Header, 0);
            Grid.SetColumn(txt75Header, 9);
            grid.Children.Add(txt75Header);

            TextBlock txtMaxHeader = new TextBlock();
            txtMaxHeader.Text = SharedStrings.DASHBOARD_MEANS_MAX;
            txtMaxHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtMaxHeader, 0);
            Grid.SetColumn(txtMaxHeader, 10);
            grid.Children.Add(txtMaxHeader);

            TextBlock txtModeHeader = new TextBlock();
            txtModeHeader.Text = SharedStrings.DASHBOARD_MEANS_MODE;
            txtModeHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtModeHeader, 0);
            Grid.SetColumn(txtModeHeader, 11);
            grid.Children.Add(txtModeHeader);
        }

        /// <summary>
        /// Draws the borders around a given output grid
        /// </summary>
        /// <param name="strataValue">
        /// The strata value to which this grid cell belongs; used to search the list of grids and 
        /// return the proper System.Windows.Controls.Grid for text insertion.
        /// </param>
        private void DrawMeansBorders(string strataValue)
        {
            Grid grid = GetStrataGrid(strataValue);

            //waitCursor.Visibility = Visibility.Collapsed;
            int rdcount = 0;
            foreach (RowDefinition rd in grid.RowDefinitions)
            {
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

        /// <summary>
        /// Checks the gadget's position on the screen and, if necessary, re-sets it so that it is visible. This is used in 
        /// scenarios where the user has moved the gadget to a specific position where its top and/or left positions are outside 
        /// of the canvas boundaries, adds a filter, and the subsequent gadget output is too short to roll onto the visible 
        /// canvas. The gadget is then hidden entirely from the user's view. This will re-set the gadget so that it is visible.
        /// This should only be called from the RenderFinish series of methods.
        /// </summary>
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
        private void SendToBack()
        {
            Canvas.SetZIndex(this, -1);
        }

        /// <summary>
        /// Sets the gadget's state to 'finished' mode
        /// </summary>
        private void RenderFinish()
        {
            waitCursor.Visibility = Visibility.Collapsed;
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            foreach (Grid freqGrid in strataGridList)
            {
                freqGrid.Visibility = Visibility.Visible;
            }

            pnlStatus.Visibility = System.Windows.Visibility.Collapsed;
            panelMain.Visibility = Visibility.Visible;
            txtStatus.Text = string.Empty;
            FilterButton.IsEnabled = true;
            CheckAndSetPosition();
        }

        /// <summary>
        /// Sets the gadget's state to 'finished with error' mode
        /// </summary>
        /// <param name="errorMessage">The error message to display</param>
        private void RenderFinishWithError(string errorMessage)
        {
            waitCursor.Visibility = Visibility.Collapsed;
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            pnlStatus.Visibility = System.Windows.Visibility.Visible;

            panelMain.Visibility = Visibility.Collapsed;

            pnlStatus.Background = new SolidColorBrush(Color.FromArgb(255, 248, 215, 226)); //Brushes.Tomato;
            pnlStatusTop.Background = new SolidColorBrush(Color.FromArgb(255, 228, 101, 142)); //Brushes.Red;

            txtStatus.Text = errorMessage;

            CheckAndSetPosition();
        }

        /// <summary>
        /// Used to push a status message to the gadget's status panel
        /// </summary>
        /// <param name="statusMessage">The status message to display</param>
        private void RequestUpdateStatusMessage(string statusMessage)
        {
            SetStatusMessage(statusMessage);
        }

        /// <summary>
        /// Used to set the gadget's current status, e.g. "Processing results..." or "Displaying output..."
        /// </summary>
        /// <param name="statusMessage">The status message to display</param>
        private void SetStatusMessage(string statusMessage)
        {
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            pnlStatus.Visibility = System.Windows.Visibility.Visible;
            txtStatus.Text = statusMessage;
        }

        /// <summary>
        /// Used to generate the list of variables and options for the GadgetParameters object
        /// </summary>        
        private void CreateInputVariableList()
        {
            Dictionary<string, string> inputVariableList = new Dictionary<string, string>();

            if (gadgetOptions == null)
            {
                gadgetOptions = new GadgetParameters();
            }

            gadgetOptions.MainVariableName = string.Empty;
            gadgetOptions.WeightVariableName = string.Empty;
            gadgetOptions.StrataVariableNames = new List<string>();
            gadgetOptions.CrosstabVariableName = string.Empty;
            gadgetOptions.ColumnNames = new List<MyString>();
            gadgetOptions.StrataVariableList = new List<MyString>();

            //   gadgetOptions.TableName = "FoodHistory1";
            gadgetOptions.TableName =
                applicationViewModel.EwavSelectedDatasource.TableName;
            gadgetOptions.DatasourceName =
                applicationViewModel.EwavSelectedDatasource.DatasourceName;
            gadgetOptions.GadgetFilters = GadgetFilters;
            inputVariableList.Add("tableName", gadgetOptions.TableName);

            if (cbxField.SelectedIndex > -1 && !string.IsNullOrWhiteSpace(((EwavColumn)(cbxField.SelectedValue)).Name))
            {
                inputVariableList.Add("meansvar", ((EwavColumn)cbxField.SelectedItem).Name.ToString());
                gadgetOptions.MainVariableName = ((EwavColumn)cbxField.SelectedItem).Name.ToString();
            }
            else
            {
                return;
            }

            if (cbxFieldWeight.SelectedIndex > -1 && !string.IsNullOrWhiteSpace(((EwavColumn)(cbxFieldWeight.SelectedValue)).Name))
            {
                inputVariableList.Add("weightvar", ((EwavColumn)cbxFieldWeight.SelectedItem).Name.ToString());
                gadgetOptions.WeightVariableName = ((EwavColumn)cbxFieldWeight.SelectedItem).Name.ToString();
            }
            if (cbxFieldStrata.SelectedIndex > -1 && !string.IsNullOrWhiteSpace(((EwavColumn)(cbxFieldStrata.SelectedValue)).Name))
            {
                string strataSelectedItem = ((EwavColumn)(cbxFieldStrata.SelectedValue)).Name;
                inputVariableList.Add("stratavar", ((EwavColumn)(cbxFieldStrata.SelectedValue)).Name);
                gadgetOptions.StrataVariableNames = new List<string>();
                List<MyString> listMyString = new List<MyString>();
                MyString objMyString = new MyString();
                objMyString.VarName = strataSelectedItem;
                listMyString.Add(objMyString);
                gadgetOptions.StrataVariableList = listMyString;
            }
            if (cbxFieldCrosstab.SelectedIndex > -1 && !string.IsNullOrWhiteSpace(((EwavColumn)(cbxFieldCrosstab.SelectedValue)).Name))
            {
                inputVariableList.Add("crosstabvar", ((EwavColumn)(cbxFieldCrosstab.SelectedValue)).Name);
                gadgetOptions.CrosstabVariableName = ((EwavColumn)(cbxFieldCrosstab.SelectedValue)).Name;
            }

            if (checkboxShowANOVA.IsChecked == true)
            {
                inputVariableList.Add("showanova", "true");
            }
            else
            {
                inputVariableList.Add("showanova", "false");
            }

            gadgetOptions.InputVariableList = inputVariableList;
        }

        /// <summary>
        /// Used to construct the gadget and assign events
        /// </summary>
        /// <param name="dashboardHelper">The dashboard helper to attach to this gadget</param>
        private void Construct()
        {
            strataGridList = new List<Grid>();
            anovaBlocks = new List<StackPanel>();
            gridLabelsList = new List<TextBlock>();
            cbxField.SelectionChanged += new SelectionChangedEventHandler(cbxField_SelectionChanged);
            cbxFieldWeight.SelectionChanged += new SelectionChangedEventHandler(cbxField_SelectionChanged);
            cbxFieldStrata.SelectionChanged += new SelectionChangedEventHandler(cbxField_SelectionChanged);
            cbxFieldCrosstab.SelectionChanged += new SelectionChangedEventHandler(cbxField_SelectionChanged);

            checkboxShowANOVA.Checked += new RoutedEventHandler(checkboxCheckChanged);
            checkboxShowANOVA.Unchecked += new RoutedEventHandler(checkboxCheckChanged);

            this.IsProcessing = false;

            this.GadgetStatusUpdate += new GadgetStatusUpdateHandler(RequestUpdateStatusMessage);

            tblockCrosstabVariable.Text = SharedStrings.DASHBOARD_CROSSTAB_FIELD_LABEL;
            tblockMainVariable.Text = SharedStrings.DASHBOARD_FIELD_LABEL;
            tblockStrataVariable.Text = SharedStrings.DASHBOARD_STRATA_FIELD_SINGLE_LABEL;
            tblockWeightVariable.Text = SharedStrings.DASHBOARD_WEIGHT_FIELD_LABEL;
        }

        #endregion // Private Methods

        #region IGadget Members

        /// <summary>
        /// Sets the gadget to its 'processing' state
        /// </summary>
        public void SetGadgetToProcessingState()
        {
            this.IsProcessing = true;
            this.cbxField.IsEnabled = false;
            this.cbxFieldCrosstab.IsEnabled = false;
            this.cbxFieldStrata.IsEnabled = false;
            this.cbxFieldWeight.IsEnabled = false;
            waitCursor.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Sets the gadget to its 'finished' state
        /// </summary>
        public void SetGadgetToFinishedState()
        {
            this.IsProcessing = false;
            this.cbxField.IsEnabled = true;
            this.cbxFieldCrosstab.IsEnabled = true;
            this.cbxFieldStrata.IsEnabled = true;
            this.cbxFieldWeight.IsEnabled = true;

            if (GadgetProcessingFinished != null)
            {
                GadgetProcessingFinished(this);
            }
            waitCursor.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Gets/sets whether the gadget is processing
        /// </summary>
        public bool IsProcessing { get; set; }

        /// <summary>
        /// Updates the variable names available in the gadget's properties
        /// </summary>
        public void UpdateVariableNames()
        {
            //  FillComboboxes(true);
        }

        public StringBuilder HtmlBuilder { get; set; }

        /// <summary>
        /// Initiates a refresh of the gadget's output
        /// </summary>
        public void RefreshResults()
        {
            if (cbxField.SelectedIndex < 1)
            {
                return;
            }

            //if (!loadingCombos && gadgetOptions != null && cbxField.SelectedIndex > -1)
            //{
            //    CreateInputVariableList();
            //    waitCursor.Visibility = Visibility.Visible;
            //    pnlStatus.Background = new SolidColorBrush(Color.FromArgb(255, 235, 245, 214)); //Brushes.PaleGreen;
            //    pnlStatusTop.Background = new SolidColorBrush(Color.FromArgb(255, 162, 208, 64)); //Brushes.Green;
            //}
            //else 
            if (!loadingCombos && cbxField.SelectedIndex == -1)
            {
                waitCursor.Visibility = System.Windows.Visibility.Visible;
                ClearResults();
                waitCursor.Visibility = System.Windows.Visibility.Collapsed;
            }

            CreateInputVariableList();
            MeansViewModel meansViewModel = (MeansViewModel)this.DataContext;
            gadgetOptions.UseAdvancedDataFilter = applicationViewModel.UseAdvancedFilter;

            if (gadgetOptions.CrosstabVariableName.ToString().Length > 0)
            {
                //meansViewModel.GetCrossTabResults("NEDS", "FoodHistory1", gadgetOptions);
                meansViewModel.GetCrossTabAndFreqResults(gadgetOptions);
            }
            else
            {
                //  meansViewModel.GetFrequencyData("NEDS", "FoodHistory1", gadgetOptions);
                meansViewModel.GetFrequencyData(gadgetOptions);
            }
        }

        /// <summary>
        /// Generates Xml representation of this gadget
        /// </summary>
        /// <param name="doc">The Xml docment</param>
        /// <returns>XmlNode</returns>
        public XNode Serialize(XDocument doc)
        {
            CreateInputVariableList();

            Dictionary<string, string> inputVariableList = gadgetOptions.InputVariableList;

            string meansVar = string.Empty;
            string strataVar = string.Empty;
            string weightVar = string.Empty;
            string crosstabVar = string.Empty;

            if (inputVariableList != null)
            {
                if (inputVariableList.ContainsKey("meansvar"))
                {
                    meansVar = inputVariableList["meansvar"].Replace("<", "&lt;");
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
            }




            XElement element = new XElement("gadget",
                new XAttribute("top", Canvas.GetTop(this).ToString("F0")),
                new XAttribute("left", Canvas.GetLeft(this).ToString("F0")),
                new XAttribute("collapsed", "false"),
                new XAttribute("gadgetType", "Ewav.MeansControl"),
                new XElement("mainVariable", meansVar),
                new XElement("strataVariable", strataVar),
                new XElement("weightVariable", weightVar),
                new XElement("crosstabVariable", crosstabVar),
                new XElement("showANOVA", (bool)checkboxShowANOVA.IsChecked),
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
        /// <param name="ForDash"></param>
        /// <param name="htmlFileName"></param>
        /// <param name="count"></param>
        /// <param name="element">The element from which to create the gadget</param>
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
            htmlBuilder.AppendLine(cmnClass.GenerateStandardHTMLStyle());
            if (string.IsNullOrEmpty(CustomOutputHeading))
            {
                htmlBuilder.AppendLine("<h2 class=\"gadgetHeading\">Means</h2>");
            }
            else if (CustomOutputHeading != "(none)")
            {
                htmlBuilder.AppendLine(string.Format("<h2 class=\"gadgetHeading\">{0}</h2>", CustomOutputHeading));
            }

            htmlBuilder.AppendLine("<p class=\"gadgetOptions\"><small>");
            htmlBuilder.AppendLine(string.Format("<em>Main variable:</em> <strong>{0}</strong>", ((EwavColumn)cbxField.SelectedItem).Name));
            htmlBuilder.AppendLine("<br />");

            if (cbxFieldCrosstab.SelectedIndex >= 0)
            {
                htmlBuilder.AppendLine(string.Format("<em>Crosstab variable:</em> <strong>{0}</strong>", ((EwavColumn)cbxFieldCrosstab.SelectedItem).Name));
                htmlBuilder.AppendLine("<br />");
            }

            if (cbxFieldWeight.SelectedIndex >= 0)
            {
                htmlBuilder.AppendLine(string.Format("<em>Weight variable:</em> <strong>{0}</strong>", ((EwavColumn)cbxFieldWeight.SelectedItem).Name));
                htmlBuilder.AppendLine("<br />");
            }
            if (cbxFieldStrata.SelectedIndex >= 0)
            {
                htmlBuilder.AppendLine(string.Format("<em>Strata variable:</em> <strong>{0}</strong>", ((EwavColumn)cbxFieldStrata.SelectedItem).Name));
                htmlBuilder.AppendLine("<br />");
            }

            htmlBuilder.AppendLine("</small></p>");

            if (!string.IsNullOrEmpty(CustomOutputDescription))
            {
                htmlBuilder.AppendLine(string.Format("<p class=\"gadgetsummary\">{0}</p>", CustomOutputDescription));
            }

            // Each grid in the 'strataGridList' has a tag associated with it which tells us which strata 
            // that grid is for. For example, if we stratify by 'Sex' in the Oswego table (in Sample.prj)
            // we will have two grids in strataGridList, one for males and one for females. The tags will
            // be 'Sex = Male' and 'Sex = Female', respectively. 
            foreach (Grid grid in this.strataGridList)
            {
                string gridName = grid.Tag.ToString();

                string summaryText = string.Format("This tables contains several descriptive statistics for the field {0}. ", ((EwavColumn)cbxField.SelectedItem).Name);
                if (cbxFieldWeight.SelectedIndex > -1 && !string.IsNullOrEmpty(((EwavColumn)cbxFieldWeight.SelectedItem).Name))
                {
                    summaryText += string.Format("The field {0} has been specified as a weight. ", ((EwavColumn)cbxFieldWeight.SelectedItem).Name);
                }
                if (cbxFieldStrata.SelectedIndex > -1 && !string.IsNullOrEmpty(((EwavColumn)cbxFieldStrata.SelectedItem).Name))
                {
                    summaryText += string.Format("The data has been stratified. The data in this table is for the strata value {0}. ", grid.Tag.ToString());
                }
                if (cbxFieldCrosstab.SelectedIndex > -1 && !string.IsNullOrEmpty(((EwavColumn)cbxFieldCrosstab.SelectedItem).Name))
                {
                    summaryText += string.Format("The data has been cross-tabulated; there will be one data row for each value of {0}. ", ((EwavColumn)cbxFieldCrosstab.SelectedItem).Name);
                }
                summaryText += "The column headings are: The description of the data, the total number of observations, the sum of the observations, the mean, the variance, the standard deviation, the minimum observed value, the 25% value, the median value, the 75% value, the maximum, and the mode.";

                htmlBuilder.AppendLine("<div style=\"height: 7px;\"></div>");
                htmlBuilder.AppendLine(string.Format("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" summary=\"{0}\">", summaryText));
                htmlBuilder.AppendLine(string.Format("<caption>{0}</caption>", gridName));

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
                            htmlBuilder.AppendLine("<tr>");
                        }
                        if (columnNumber == 0 && rowNumber > 0)
                        {
                            tableDataTagOpen = "<td class=\"value\">";
                        }

                        string value = ((TextBlock)control).Text;
                        string formattedValue = value;

                        htmlBuilder.AppendLine(string.Format("{0}{1}{2}", tableDataTagOpen, formattedValue, tableDataTagClose));

                        if (columnNumber >= grid.ColumnDefinitions.Count - 1)
                        {
                            htmlBuilder.AppendLine("</tr>");
                        }
                    }
                }

                htmlBuilder.AppendLine("</table>");

                // We must find the specific stack panel that corresponds to this strata value from the list of 
                // ANOVA panels in the gadget. The 'tag' property of the panel is a key value pair that contains
                // the strata value we need, plus the set of descriptive statistics that we must display in the
                // output.
                StackPanel anovaPanel = null;
                DescriptiveStatistics statistics = new DescriptiveStatistics();
                foreach (StackPanel panel in anovaBlocks)
                {
                    if (panel.Tag is KeyValuePair<string, DescriptiveStatistics>)
                    {
                        KeyValuePair<string, DescriptiveStatistics> kvp = ((KeyValuePair<string, DescriptiveStatistics>)panel.Tag);
                        if (kvp.Key.Equals(gridName))
                        {
                            anovaPanel = panel;
                            statistics = kvp.Value;
                            break; // no sense in continuning
                        }
                    }
                }

                // check to make sure we actually found one
                if (!(anovaPanel == null))
                {
                    string strssBetweenValue = SharedStrings.UNDEFINED;
                    string strdfBetweenValue = SharedStrings.UNDEFINED;
                    string strmsBetweenValue = SharedStrings.UNDEFINED;
                    string strssWithinValue = SharedStrings.UNDEFINED;
                    string strdfWithinValue = SharedStrings.UNDEFINED;
                    string strmsWithinValue = SharedStrings.UNDEFINED;
                    string strfStatisticValue = SharedStrings.UNDEFINED;
                    string stranovaPValueValue = SharedStrings.UNDEFINED;
                    string stranovaTValueValue = SharedStrings.UNDEFINED;
                    string strchiSquareValue = SharedStrings.UNDEFINED;
                    string strbartlettPValue = SharedStrings.UNDEFINED;
                    string strTotalSSValue = SharedStrings.UNDEFINED;
                    string strTotalDFValue = SharedStrings.UNDEFINED;
                    string strKruskalWallisH = SharedStrings.UNDEFINED;
                    string strKruskalPValue = SharedStrings.UNDEFINED;

                    if (statistics.SsBetween.HasValue)
                    {
                        strssBetweenValue = statistics.SsBetween.Value.ToString("F4");
                    }
                    if (statistics.DfBetween.HasValue)
                    {
                        strdfBetweenValue = statistics.DfBetween.Value.ToString("F0");
                    }
                    if (statistics.MsBetween.HasValue)
                    {
                        strmsBetweenValue = statistics.MsBetween.Value.ToString("F4");
                    }
                    if (statistics.SsWithin.HasValue)
                    {
                        strssWithinValue = statistics.SsWithin.Value.ToString("F4");
                    }
                    if (statistics.DfBetween.HasValue)
                    {
                        strdfWithinValue = statistics.DfBetween.Value.ToString("F0");
                    }
                    if (statistics.MsWithin.HasValue)
                    {
                        strmsWithinValue = statistics.MsWithin.Value.ToString("F4");
                    }
                    if (statistics.FStatistic.HasValue)
                    {
                        strfStatisticValue = statistics.FStatistic.Value.ToString("F4");
                    }
                    if (statistics.AnovaPValue.HasValue)
                    {
                        stranovaPValueValue = statistics.AnovaPValue.Value.ToString("F4");
                    }
                    if (statistics.ChiSquare.HasValue)
                    {
                        strchiSquareValue = statistics.ChiSquare.Value.ToString("F4");
                    }
                    if (statistics.BartlettPValue.HasValue)
                    {
                        strbartlettPValue = statistics.BartlettPValue.Value.ToString("F4");
                    }

                    if (statistics.SsBetween.HasValue && statistics.SsWithin.HasValue)
                    {
                        strTotalSSValue = (statistics.SsBetween.Value + statistics.SsWithin.Value).ToString("F4");
                    }
                    if (statistics.DfBetween.HasValue && statistics.DfBetween.HasValue)
                    {
                        strTotalDFValue = (statistics.DfBetween.Value + statistics.DfBetween.Value).ToString("F0");
                    }
                    if (statistics.KruskalWallisH.HasValue)
                    {
                        strKruskalWallisH = statistics.KruskalWallisH.Value.ToString("F4");
                    }
                    if (statistics.KruskalPValue.HasValue)
                    {
                        strKruskalPValue = statistics.KruskalPValue.Value.ToString("F4");
                    }

                    summaryText = string.Format("This table contains analysis of variance (ANOVA) statistics for the field {0}, cross-tabulated by {1}. ", ((EwavColumn)cbxField.SelectedItem).Name, ((EwavColumn)cbxFieldCrosstab.SelectedItem).Name);
                    summaryText += "The column headings for this table are: The variation, the SS value, the degrees of freedom, the MS value, and the F-statistic. There are three rows: The between, the within, and the total.";

                    // ANOVA
                    htmlBuilder.AppendLine("<div style=\"height: 7px;\"></div>");
                    htmlBuilder.AppendLine("<p><strong>ANOVA, a parametric test for inequality of population means</strong><br />");
                    htmlBuilder.AppendLine("<small>(For normally distributed data only)</small></p>");
                    htmlBuilder.AppendLine(string.Format("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" summary=\"{0}\">", summaryText));
                    htmlBuilder.AppendLine(" <tr>");
                    htmlBuilder.AppendLine("   <th>Variation</th>");
                    htmlBuilder.AppendLine("   <th>SS</th>");
                    htmlBuilder.AppendLine("   <th>df</th>");
                    htmlBuilder.AppendLine("   <th>MS</th>");
                    htmlBuilder.AppendLine("   <th>F statistic</th>");
                    htmlBuilder.AppendLine(" </tr>");

                    htmlBuilder.AppendLine(" <tr>");
                    htmlBuilder.AppendLine("   <th>Between</th>");
                    htmlBuilder.AppendLine(string.Format("   <td>{0}</td>", strssBetweenValue));
                    htmlBuilder.AppendLine(string.Format("   <td>{0}</td>", strdfBetweenValue));
                    htmlBuilder.AppendLine(string.Format("   <td>{0}</td>", strmsBetweenValue));
                    htmlBuilder.AppendLine(string.Format("   <td>{0}</td>", strfStatisticValue));
                    htmlBuilder.AppendLine(" </tr>");

                    htmlBuilder.AppendLine(" <tr>");
                    htmlBuilder.AppendLine("   <th>Within</th>");
                    htmlBuilder.AppendLine(string.Format("   <td>{0}</td>", strssWithinValue));
                    htmlBuilder.AppendLine(string.Format("   <td>{0}</td>", strdfWithinValue));
                    htmlBuilder.AppendLine(string.Format("   <td>{0}</td>", strmsWithinValue));
                    htmlBuilder.AppendLine("   <td>&nbsp;</td>");
                    htmlBuilder.AppendLine(" </tr>");

                    htmlBuilder.AppendLine(" <tr>");
                    htmlBuilder.AppendLine("   <th>Total</th>");
                    htmlBuilder.AppendLine(string.Format("   <td>{0}</td>", strTotalSSValue));
                    htmlBuilder.AppendLine(string.Format("   <td>{0}</td>", strTotalDFValue));
                    htmlBuilder.AppendLine("   <td>&nbsp;</td>");
                    htmlBuilder.AppendLine("   <td>&nbsp;</td>");
                    htmlBuilder.AppendLine(" </tr>");
                    htmlBuilder.AppendLine("</table>");

                    htmlBuilder.AppendLine("<div style=\"height: 7px;\"></div>");

                    htmlBuilder.AppendLine("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" summary=\"This table contains the ANOVA p-value.\">");
                    htmlBuilder.AppendLine(" <tr>");
                    htmlBuilder.AppendLine("   <th>P Value</th>");
                    htmlBuilder.AppendLine(string.Format("   <td>{0}</td>", stranovaPValueValue));
                    htmlBuilder.AppendLine(" </tr>");
                    htmlBuilder.AppendLine("</table>");

                    // Bartlett
                    htmlBuilder.AppendLine("<div style=\"height: 7px;\"></div>");
                    htmlBuilder.AppendLine("<p><strong>Bartlett's Test for Inequality of Population Variances</strong></p>");
                    htmlBuilder.AppendLine("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">");
                    htmlBuilder.AppendLine(" <tr>");
                    htmlBuilder.AppendLine("   <th>Chi Square</th>");
                    htmlBuilder.AppendLine(string.Format("   <td>{0}</td>", strchiSquareValue));
                    htmlBuilder.AppendLine(" </tr>");

                    htmlBuilder.AppendLine(" <tr>");
                    htmlBuilder.AppendLine("   <th>Degrees of freedom</th>");
                    htmlBuilder.AppendLine(string.Format("   <td>{0}</td>", strdfBetweenValue));
                    htmlBuilder.AppendLine(" </tr>");

                    htmlBuilder.AppendLine(" <tr>");
                    htmlBuilder.AppendLine("   <th>P Value</th>");
                    htmlBuilder.AppendLine(string.Format("   <td>{0}</td>", strbartlettPValue));
                    htmlBuilder.AppendLine("</table>");

                    htmlBuilder.AppendLine("<p><small>A small p-value (e.g., less than 0.05) suggests that the variances are not homogeneous and that the ANOVA may not be appropriate.</small></p>");

                    // Kruskal-Wallis
                    htmlBuilder.AppendLine("<div style=\"height: 7px;\"></div>");
                    htmlBuilder.AppendLine("<p><strong>Mann-Whitney/Wilcoxon Two-Sample Test (Kruskal-Wallis test for two groups)</strong></p>");
                    htmlBuilder.AppendLine("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" >");
                    htmlBuilder.AppendLine(" <tr>");
                    htmlBuilder.AppendLine("   <th>Kruskal-Wallis H</th>");
                    htmlBuilder.AppendLine(string.Format("   <td>{0}</td>", strKruskalWallisH));
                    htmlBuilder.AppendLine(" </tr>");

                    htmlBuilder.AppendLine(" <tr>");
                    htmlBuilder.AppendLine("   <th>Degrees of freedom</th>");
                    htmlBuilder.AppendLine(string.Format("   <td>{0}</td>", strdfBetweenValue));
                    htmlBuilder.AppendLine(" </tr>");

                    htmlBuilder.AppendLine(" <tr>");
                    htmlBuilder.AppendLine("   <th>P Value</th>");
                    htmlBuilder.AppendLine(string.Format("   <td>{0}</td>", strKruskalPValue));
                    htmlBuilder.AppendLine("</table>");
                }
            }

            HtmlBuilder = htmlBuilder;

            if (ForDash == false)
                System.Windows.Browser.HtmlPage.Window.Invoke("DisplayFormattedText", htmlBuilder.ToString());

            return "";

        }

        /// <summary>
        /// Gets/sets the gadget's custom output heading
        /// </summary>
        public string CustomOutputHeading { get; set; }

        /// <summary>
        /// Gets/sets the gadget's custom output description
        /// </summary>
        public string CustomOutputDescription { get; set; }

        /// <summary>
        /// Gets/sets the gadget's custom output caption for its table or image output components, if applicable
        /// </summary>
        public string CustomOutputCaption { get; set; }

        #endregion

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void LayoutRoot_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetSafePosition(GetParentObject<Grid>(this, "LayoutRoot"));
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).GadgetNameOnRightClick = MyControlName;
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).StrataList = this.strataGridList;
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).SelectedGadget = this;
        }

        /// <summary>
        /// GetChildObjects is a method that returns all the Objects on the Canvas.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<T> GetChildObjects<T>(DependencyObject obj, string name = "") where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (((T)child).Name == name | string.IsNullOrEmpty(name)))
                {
                    childList.Add((T)child);
                }

                childList.AddRange(GetChildObjects<T>(child));
            }

            return childList;
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseGadgetOnClick();
        }

        private string myControlName = "MeansControl";
        public ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;

        public string MyControlName
        {
            get
            {
                return myControlName;
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
        /// <summary>
        /// Closes the gadget after confirmation.
        /// </summary>
        void CloseGadgetOnClick()
        {
            CloseGadget confirm = new CloseGadget(this);
            confirm.Show();
        }

        /// <summary>
        /// Closes the gadget.
        /// </summary>
        void CloseGadget()
        {
            applicationViewModel.CloseGadget(this);

        }

        public void CreateFromXml(XElement element)
        {
            LoadingCanvas = true;

            List<EwavColumn> fieldColList = cmnClass.GetItemsSource(GetFieldDataType);
            List<EwavColumn> weightColList = cmnClass.GetItemsSource(GetWeightDataType);
            List<EwavColumn> primaryColList = cmnClass.GetItemsSource(GetPrimaryFieldDataType);
            this.GadgetFilters = new List<EwavDataFilterCondition>();
            foreach (XElement child in element.Descendants())
            {
                switch (child.Name.ToString().ToLower())
                {
                    case "mainvariable":
                        cbxField.SelectedIndex = cmnClass.FindIndexToSelect(fieldColList, child.Value.ToString().Replace("&lt;", "<"));
                        break;
                    case "stratavariable":
                        cbxFieldStrata.SelectedIndex = cmnClass.FindIndexToSelect(primaryColList, child.Value.ToString().Replace("&lt;", "<"));
                        break;
                    case "weightvariable":
                        cbxFieldWeight.SelectedIndex = cmnClass.FindIndexToSelect(weightColList, child.Value.ToString().Replace("&lt;", "<"));
                        break;
                    case "crosstabvariable":
                        cbxFieldCrosstab.SelectedIndex = cmnClass.FindIndexToSelect(primaryColList, child.Value.ToString().Replace("&lt;", "<"));
                        break;
                    case "showANOVA":
                        bool anova = false;
                        bool.TryParse(child.Value, out anova);
                        checkboxShowANOVA.IsChecked = anova;
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

            LoadingCanvas = false;

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

            DoMeans();

            cmnClass.AddControlToCanvas(this, mouseVerticalPosition, mouseHorizontalPosition, applicationViewModel.LayoutRoot);

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

            DoMeans();


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
                DoMeans();
            }
        }



    }
}
namespace Ewav.Web.Services
{
    public partial class MeansDomainContext
    {
        //This is the place to set RIA Services query timeout. 
        //TimeSpan(0, 5, 0) means five minutes vs the 
        //default 60 sec
        partial void OnCreated()
        {
            if (!DesignerProperties.GetIsInDesignMode(Application.
                Current.RootVisual))
                ((WebDomainClient<IMeansDomainServiceContract>)
                    DomainClient).ChannelFactory.Endpoint.Binding.SendTimeout =
                    new TimeSpan(0, 120, 0);
        }
    }
}