/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       TwoxTwoTableControl.xaml.cs
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
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using CommonLibrary;
using Ewav.Web.EpiDashboard;
using Ewav.BAL;
using Ewav.Client.Application;
using Ewav.Common;
using Ewav.ExtensionMethods;
using Ewav.ViewModels;
using Ewav.Views;
using Ewav.Web.Services;

namespace Ewav
{
    /// <summary>
    /// Interaction logic for TwoxTwoTableControl.xaml
    /// </summary>  
    /// 
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(UserControl))]
    [ExportMetadata("type", "gadget")]
    [ExportMetadata("tabindex", "3")]
    public partial class TwoxTwoTableControl : UserControl, IEwavGadget, IGadget, ICustomizableGadget
    {
        //  private  DashboardHelper dashboardHelper;
        int Index1 = -1, Index2 = -1;
        EwavColumn Col1, Col2;
        private bool triangleCollapsed;
        private Dictionary<string, GridCells> currentData;
        private GridCells currentSingleTableData;
        private bool advTriangleCollapsed;
        private GadgetParameters gadgetOptions;
        private BackgroundWorker baseWorker;
        private BackgroundWorker worker;
        private bool is2x2 = false;
        private bool isGrouped = false;
        private static object syncLock = new object();

        ClientCommon.Common CommonClass = new ClientCommon.Common();
        int rowCount = 1;
        int columnCount = 1;
        bool exceededMaxRows = false;
        bool exceededMaxColumns = false;
        bool includeMissing = false;

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

        /// <summary>
        /// Container that holds gadget level filters.
        /// </summary>
        public List<EwavDataFilterCondition> GadgetFilters { get; set; }

        /// <summary>
        /// Draws the borders.
        /// </summary>
        /// <param name="strataValue">The strata value.</param>
        private void drawBorders(object strataValue)
        {
            Grid grid = grdTable;

            waitCursor.Visibility = Visibility.Collapsed;
            int rdcount = 0;
            foreach (RowDefinition rd in grid.RowDefinitions)
            {
                int cdcount = 0;
                foreach (ColumnDefinition cd in grid.ColumnDefinitions)
                {
                    Rectangle rctBorder = new Rectangle();
                    //  rctBorder.Stroke = Brushes.Black;
                    //rctBorder.Stroke = new SolidColorBrush(Colors.Black);
                    rctBorder.Style = Application.Current.Resources["DataCell"] as Style;
                    Grid.SetRow(rctBorder, rdcount);
                    Grid.SetColumn(rctBorder, cdcount);
                    grid.Children.Add(rctBorder);
                    cdcount++;
                }
                rdcount++;
            }
        }

        public ApplicationViewModel ApplicationViewModel
        {
            get
            {
                return applicationViewModel;
            }
        }

        //public event GadgetClosingHandler GadgetClosing;
        //public event GadgetProcessingFinishedHandler GadgetProcessingFinished;
        //public event GadgetStatusUpdateHandler GadgetStatusUpdate;
        //public event GadgetCheckForCancellationHandler GadgetCheckForCancellation;  

        /// <summary>
        /// The value for the frameworkelement.Name property
        /// </summary>
        /// <value></value>
        public string MyControlName
        {
            get
            {
                return "TwoxTwoTableControl";
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
                return "2 x 2 Table";
            }
        }

        //private struct GridCells
        //{
        //    public int ytVal;
        //    public int quadValue;
        //    public int ynVal;
        //    public int ntVal;
        //    public int nyVal;
        //    public int nnVal;
        //    public int ttVal;
        //    public int tyVal;
        //    public int tnVal;
        //    public double yyRowPct;
        //    public double ynRowPct;
        //    public double nyRowPct;
        //    public double nnRowPct;
        //    public double tyRowPct;
        //    public double tnRowPct;
        //    public double yyColPct;
        //    public double nyColPct;
        //    public double ynColPct;
        //    public double nnColPct;
        //    public double ytColPct;
        //    public double ntColPct;
        //    //   public StatisticsRepository.cTable.SingleTableResults singleTableResults;
        //}

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






        #region Designer support
        private void CloseGadget_Click(object sender, RoutedEventArgs e)
        {
            CloseGadgetOnClick();
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
        public void CloseGadgetOnClick()
        {
            CloseGadget confirm = new CloseGadget(this);
            confirm.Show();
        }

        //private void Construct()
        //{
        //    //GadgetContent.Visibility = System.Windows.Visibility.Visible;
        //}

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //gadgetContextMenu.Hide();k
            //BusyIndicatorGrid.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void LayoutRoot_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClientCommon.Common cmnClass = new ClientCommon.Common();
            Point p = e.GetSafePosition(cmnClass.GetParentObject<Grid>(this, "LayoutRoot"));
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).GadgetNameOnRightClick = MyControlName;
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).StrataList = null;
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).SelectedGadget = this;
        }

        private void rectangle_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // TODO: Add event handler implementation here.
            Storyboard1.Stop();
            Rectangle myRect = (Rectangle)sender;
            anim1.SetValue(Storyboard.TargetNameProperty, myRect.Name);
            anim2.SetValue(Storyboard.TargetNameProperty, myRect.Name);
            Storyboard1.Begin();
            if (myRect.Name == "rctRed")
            {
                Storyboard2.Stop();
                anim3.SetValue(Storyboard.TargetNameProperty, "BspYY");
                Storyboard2.Begin();
            }
            else if (myRect.Name == "rctGreen")
            {
                Storyboard2.Stop();
                anim3.SetValue(Storyboard.TargetNameProperty, "BspNN");
                Storyboard2.Begin();
            }
            else if (myRect.Name == "rctYellow")
            {
                Storyboard2.Stop();
                anim3.SetValue(Storyboard.TargetNameProperty, "BspYN");
                Storyboard2.Begin();
            }
            else if (myRect.Name == "rctOrange")
            {
                Storyboard2.Stop();
                anim3.SetValue(Storyboard.TargetNameProperty, "BspNY");
                Storyboard2.Begin();
            }
        }

        private void rectangle_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // TODO: Add event handler implementation here.
            Storyboard2.Stop();
        }

        private void sp_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Border mysp = (Border)sender;
            Storyboard2.Stop();
            anim3.SetValue(Storyboard.TargetNameProperty, mysp.Name);
            Storyboard2.Begin();
            if (mysp.Name == "BspYY")
            {
                Storyboard1.Stop();
                anim1.SetValue(Storyboard.TargetNameProperty, "rctRed");
                anim2.SetValue(Storyboard.TargetNameProperty, "rctRed");
                Storyboard1.Begin();
            }
            else if (mysp.Name == "BspYN")
            {
                Storyboard1.Stop();
                anim1.SetValue(Storyboard.TargetNameProperty, "rctYellow");
                anim2.SetValue(Storyboard.TargetNameProperty, "rctYellow");
                Storyboard1.Begin();
            }
            else if (mysp.Name == "BspNY")
            {
                Storyboard1.Stop();
                anim1.SetValue(Storyboard.TargetNameProperty, "rctOrange");
                anim2.SetValue(Storyboard.TargetNameProperty, "rctOrange");
                Storyboard1.Begin();
            }
            else if (mysp.Name == "BspNN")
            {
                Storyboard1.Stop();
                anim1.SetValue(Storyboard.TargetNameProperty, "rctGreen");
                anim2.SetValue(Storyboard.TargetNameProperty, "rctGreen");
                Storyboard1.Begin();
            }
        }

        private void sp_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // TODO: Add event handler implementation here.
            Storyboard2.Stop();
        }

        #endregion

        private delegate void SetGridTextDelegate(string strataValue, TextBlockConfig textBlockConfig, FontWeight fontWeight);
        private delegate void AddFreqGridDelegate(string strataVar, string value, int columnCount);
        private delegate void RenderFrequencyHeaderDelegate(string strataValue, string freqVar, IEnumerable<string> allColumns);
        private delegate void SetGridBarDelegate(string strataValue, int rowNumber, double pct);
        private delegate void AddGridRowDelegate(string strataValue, int height);
        private delegate void AddGridFooterDelegate(string strataValue, int rowNumber, int[] totalRows);
        private delegate void DrawFrequencyBordersDelegate(string strataValue);

        //    private delegate void RenderGridDelegate(GridCells gridCells, DataTableToDTO twoByTwoTable);
        private delegate void RenderMultiGridDelegate(Dictionary<string, GridCells> gridCellCollection);
        private delegate void ShowErrorMessage(string errorMessage);
        private delegate void SimpleCallback();

        private delegate void SetStatusDelegate(string statusMessage);
        private delegate void RequestUpdateStatusDelegate(string statusMessage);
        private delegate bool CheckForCancellationDelegate();

        private delegate void RenderFinishWithErrorDelegate(string errorMessage);
        private delegate void RenderFinishWithWarningDelegate(string errorMessage);

        public event ColumnsLoadedEventEventHandler ColumnsLoaded;

        TwoxTwoViewModel twoxtwoViewModel;

        public TwoxTwoTableControl()
        {
            InitializeComponent();

            twoxtwoViewModel = (TwoxTwoViewModel)this.DataContext;

            this.Loaded += new RoutedEventHandler(TwoxTwoTableControl_Loaded);
            FillComboboxes();
        }

        void TwoxTwoTableControl_Loaded(object sender, RoutedEventArgs e)
        {
            RenderStart();

            try
            {
                twoxtwoViewModel = (TwoxTwoViewModel)this.DataContext;
                //twoxtwoViewModel.ColumnsLoadedEvent +=
                //    new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(twoxtwoViewModel_ColumnsLoadedEvent);
                twoxtwoViewModel.FrequencyTableLoadedEvent +=
                    new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(twoxtwoViewModel_FrequencyTableLoadedEvent);
                twoxtwoViewModel.SetupGadgetEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(twoxtwoViewModel_SetupGadgetEvent);
                twoxtwoViewModel.ErrorNotice += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(twoxtwoViewModel_ErrorNotice);
                applicationViewModel.ApplyDataFilterEvent += new ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);

                applicationViewModel.DefinedVariableAddedEvent += new Client.Application.DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);
                //    lrViewModel.ColumnsLoadedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(lrViewModel_ColumnsLoadedEvent);
                applicationViewModel.DefinedVariableInUseDeletedEvent += new Client.Application.DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableInUseDeletedEvent);
                applicationViewModel.DefinedVariableNotInUseDeletedEvent += new Client.Application.DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
                applicationViewModel.PreColumnChangedEvent += new Client.Application.PreColumnChangedEventHandler(applicationViewModel_PreColumnChangedEvent);
                applicationViewModel.UnloadedEvent += new UnloadedEventHandler(applicationViewModel_UnloadedEvent);

                //  FillComboboxes();

                Construct();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("TwoxTwoTableControl Error \n========================================={0}\n================================{1}", ex.Message, ex.StackTrace));
            }
            //this.gadgetExpander.IsExpanded = false;
        }

        void applicationViewModel_UnloadedEvent(object o)
        {
            UnloadGadget();
        }

        private void UnloadGadget()
        {
            applicationViewModel.ApplyDataFilterEvent -= new ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);

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
            RefreshResults();
        }

        void applicationViewModel_DefinedVariableInUseDeletedEvent(object o)
        {
            ResetGadget();
        }

        private void ResetGadget()
        {
            SearchIndex();
            if (IsDFUsedInThisGadget())
            {
                Index1 = Index2 = -1;
                pnlMainContent.Visibility = System.Windows.Visibility.Collapsed;
                pnlCrosstabContent.Visibility = System.Windows.Visibility.Collapsed;
                waitCursor.Visibility = System.Windows.Visibility.Collapsed;
                spContent.Visibility = System.Windows.Visibility.Collapsed;
            }
            LoadingDropDowns = true;
            FillComboboxes();
            LoadingDropDowns = false;
        }

        private bool IsDFUsedInThisGadget()
        {
            return Col1 != null && Col1.Name == applicationViewModel.ItemToBeRemoved.Name ||
                            Col2 != null && Col2.Name == applicationViewModel.ItemToBeRemoved.Name;
        }


        void applicationViewModel_DefinedVariableAddedEvent(object o)
        {
            SearchIndex();
            LoadingDropDowns = true;
            FillComboboxes();
            LoadingDropDowns = false;
            RefreshResults();
        }

        /// <summary>
        /// Saves the Values of Columns.
        /// </summary>
        private void SaveColumnValues()
        {
            Col1 = (EwavColumn)cbxExposureField.SelectedItem;
            Col2 = (EwavColumn)cbxOutcomeField.SelectedItem;
            //Col3 = (EwavColumn)cbxConf.SelectedItem;
            //Col4 = (EwavColumn)cbxFields.SelectedItem;
        }

        /// <summary>
        /// Searches current index of the columns.
        /// </summary>
        private void SearchIndex()
        {
            //List<ColumnDataType> columnDataType = new List<ColumnDataType>();
            //columnDataType.Add(ColumnDataType.Boolean);
            //columnDataType.Add(ColumnDataType.Numeric);
            //columnDataType.Add(ColumnDataType.Text);

            List<EwavColumn> colsList = CommonClass.GetItemsSource(GetFieldDataType, false);

            Index1 = CommonClass.SearchCurrentIndex(Col1, colsList);
            //    SearchCurrentIndex(Col1);
            Index2 = CommonClass.SearchCurrentIndex(Col2, colsList); //SearchCurrentIndex(Col2);
            //Index3 = cbxConf.SelectedIndex;
            //Index4 = SearchCurrentIndex(Col4);
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
            //DFInUse = null;
            if (Col1 != null && Col1.IsUserDefined == true)
            {
                Col1.IsInUse = true;
                //DFInUse = Col1;
            }
            if (Col2 != null && Col2.IsUserDefined == true)
            {
                Col2.IsInUse = true;
                //DFInUse = Col2;
            }
        }

        void twoxtwoViewModel_ErrorNotice(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
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

        void applicationViewModel_ApplyDataFilterEvent(object o)
        {
            if (applicationViewModel.RemoveIndicator &&
                IsDFUsedInThisGadget())
            {
                ResetGadget();
            }
            else
            {
                RefreshResults();
            }
        }

        void twoxtwoViewModel_SetupGadgetEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            gadgetOptions.UseAdvancedDataFilter = applicationViewModel.UseAdvancedFilter;
            twoxtwoViewModel = sender as TwoxTwoViewModel;

            TwoxTwoAndMxNResultsSet resultSet = twoxtwoViewModel.TwoxTwoAndMxNResultsSet;

            //if (resultSet.FreqResultsDataTable.Rows.Count == 2 && table.Columns.Count == 3)
            {
                Is2x2 = twoxtwoViewModel.TwoxTwoAndMxNResultsSet.Is2x2;
            }


            Ewav.Web.Services.GridCells gc = resultSet.GridCells;

            //            ClearSingleResults();

            //  this.Dispatcher.BeginInvoke(new SimpleCallback(SetGadgetToProcessingState));
            SetGadgetToProcessingState();
            //  this.Dispatcher.BeginInvoke(new SimpleCallback(ClearSingleResults));
            ClearSingleResults();

            RequestUpdateStatusDelegate requestUpdateStatus = new RequestUpdateStatusDelegate(RequestUpdateStatusMessage);
            CheckForCancellationDelegate checkForCancellation = new CheckForCancellationDelegate(IsCancelled);
            //gadgetOptions.GadgetStatusUpdate += new GadgetStatusUpdateHandler(requestUpdateStatus);
            //gadgetOptions.GadgetCheckForCancellation += new GadgetCheckForCancellationHandler(checkForCancellation);    

            rowCount = 1;
            columnCount = 1;
            exceededMaxRows = false;
            exceededMaxColumns = false;
            includeMissing = false;

            if (Is2x2)
            {

                this.DataContext = twoxtwoViewModel;
                this.listResults.ItemsSource = twoxtwoViewModel.DatatableBagResultSet;

                ///Render2X2GridCells(gc, resultSet, resultSet.TwoxTwoTableDTO);
            }
            else
            {
                RenderMxNGridCells(resultSet);
            }

            if (exceededMaxRows && exceededMaxColumns)
            {
                //  this.Dispatcher.BeginInvoke(new RenderFinishWithWarningDelegate(RenderFinishWithWarning), "Warning: Some rows and allColumns were not displayed due to gadget settings. Showing top " + MaxRows.ToString() + " rows and top " + MaxColumns.ToString() + " allColumns only.");
                RenderFinishWithWarning(string.Format("Warning: Some rows and allColumns were not displayed due to gadget settings. Showing top {0} rows and top {1} allColumns only.", MaxRows.ToString(), MaxColumns.ToString()));
            }
            else if (exceededMaxColumns)
            {
                //  this.Dispatcher.BeginInvoke(new RenderFinishWithWarningDelegate(RenderFinishWithWarning), "Warning: Some allColumns were not displayed due to gadget settings. Showing top " + MaxColumns.ToString() + " allColumns only.");    
                RenderFinishWithWarning(string.Format("Warning: Some allColumns were not displayed due to gadget settings. Showing top {0} allColumns only.", MaxColumns.ToString()));
            }
            else if (exceededMaxRows)
            {
                //  this.Dispatcher.BeginInvoke(new RenderFinishWithWarningDelegate(RenderFinishWithWarning), string.Format(SharedStrings.DASHBOARD_GADGET_STATUS_ROW_LIMIT, MaxRows.ToString()));    
                RenderFinishWithWarning(string.Format(SharedStrings.DASHBOARD_GADGET_STATUS_ROW_LIMIT, MaxRows.ToString()));
            }

            if(resultSet.DatatableBag != null)
            { 
                if (resultSet.DatatableBag.RecordList.Count > 3)
                {
                    RenderFinishWithWarning("Warning: More than two values exist in the exposure field. Displaying an MxN table.");
                }
                else if (resultSet.DatatableBag.ColumnNameList.Count > 3)
                {
                    RenderFinishWithWarning("Warning: More than two values exist in the outcome field. Displaying an MxN table.");
                }
                else
                {
                    RenderFinish();
                }
            }

            //else if (rowCount > 2)
            //{
            //    //  this.Dispatcher.BeginInvoke(new RenderFinishWithWarningDelegate(RenderFinishWithWarning), "Warning: More than two values exist in the exposure field. Displaying an MxN table.");    
            //    RenderFinishWithWarning("Warning: More than two values exist in the exposure field. Displaying an MxN table.");
            //}
            //else if (columnCount > 3)
            //{
            //    //  this.Dispatcher.BeginInvoke(new RenderFinishWithWarningDelegate(RenderFinishWithWarning), "Warning: More than two values exist in the outcome field. Displaying an MxN table.");     
            //    RenderFinishWithWarning("Warning: More than two values exist in the outcome field. Displaying an MxN table.");
            //}
            //else if (rowCount < 2)
            //{
            //    //  this.Dispatcher.BeginInvoke(new RenderFinishWithWarningDelegate(RenderFinishWithWarning), "Warning: Less than two values exist in the exposure field. Displaying an MxN table.");        
            //    RenderFinishWithWarning("Warning: Less than two values exist in the exposure field. Displaying an MxN table.");
            //}
            //else if (columnCount < 3)
            //{
            //    //  this.Dispatcher.BeginInvoke(new RenderFinishWithWarningDelegate(RenderFinishWithWarning), "Warning: Less than two values exist in the outcome field. Displaying an MxN table.");    
            //    RenderFinishWithWarning("Warning: Less than two values exist in the outcome field. Displaying an MxN table.");
            //}
            //else
            //{
            //    // this.Dispatcher.BeginInvoke(new SimpleCallback(RenderFinish));         
            //    RenderFinish();
            //}
            ////  this.Dispatcher.BeginInvoke(new SimpleCallback(SetGadgetToFinishedState));       

            SetGadgetToFinishedState();

            gadgetExpander.IsExpanded = false;
        }

        #region   ViewModel Event hadlers

        //public void twoxtwoViewModel_ColumnsLoadedEvent()    //  object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        //{
        //    //   TwoxTwoViewModel twoxTwoViewModel = sender as TwoxTwoViewModel;

        //    FillComboboxes();

        //    //  twoxTwoViewModel);
        //}

        void twoxtwoViewModel_FrequencyTableLoadedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            List<FrequencyResultData> frequencyResultDTO = sender as List<FrequencyResultData>;
            //      EwavStep1(frequencyResultDTO);
        }

        #endregion

        private void Construct()
        {
            cbxExposureField.SelectionChanged += new SelectionChangedEventHandler(Field_SelectionChanged);
            cbxOutcomeField.SelectionChanged += new SelectionChangedEventHandler(Field_SelectionChanged);

            checkboxShowPercents.Checked += new RoutedEventHandler(checkboxShowPercents_CheckChanged);
            checkboxShowPercents.Unchecked += new RoutedEventHandler(checkboxShowPercents_CheckChanged);

            checkboxSmartTable.Checked += new RoutedEventHandler(checkboxCheckChanged);
            checkboxSmartTable.Unchecked += new RoutedEventHandler(checkboxCheckChanged);

            this.advTriangleCollapsed = true;

            this.IsProcessing = false;
            this.txtStatus.Text = string.Empty;

            //this.GadgetStatusUpdate += new GadgetStatusUpdateHandler(RequestUpdateStatusMessage);
            //this.GadgetCheckForCancellation += new GadgetCheckForCancellationHandler(IsCancelled);
            if (gadgetOptions == null)
            {
                gadgetOptions = new GadgetParameters();
                gadgetOptions.ShouldIncludeFullSummaryStatistics = false;
                gadgetOptions.ShouldIncludeMissing = false;
                gadgetOptions.ShouldSortHighToLow = false;
                gadgetOptions.ShouldUseAllPossibleValues = false;
                gadgetOptions.StrataVariableNames = new List<string>();
            }

        }

        private bool Is2x2
        {
            get
            {
                return is2x2;
            }
            set
            {
                is2x2 = value;
            }
        }

        /// <summary>
        /// Handles the check / unchecked events
        /// </summary>
        /// <param name="sender">Object that fired the event</param>
        /// <param name="e">.NET supplied event parameters</param>
        private void checkboxShowPercents_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (checkboxShowPercents.IsChecked == true)
            {
                TurnOnPercents();
            }
            else
            {
                TurnOffPercents();
            }
        }

        //void mnuPrint_Click(object sender, RoutedEventArgs e)
        //{
        //    Common.Print(pnlMainContent);
        //}

        //void imgClose_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    CloseGadget();
        //}

        //void imgClose_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    Uri uriSource = new Uri("Images/x.png", UriKind.Relative);
        //    imgClose.Source = new BitmapImage(uriSource);
        //}

        //void imgClose_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    Uri uriSource = new Uri("Images/x_over.png", UriKind.Relative);
        //    imgClose.Source = new BitmapImage(uriSource);
        //}

        //void mnuSave_Click(object sender, RoutedEventArgs e)
        //{
        //    Common.SaveAsImage(pnlMainContent);
        //}

        ///// <summary>
        ///// Handles the click event for the 'Copy data to clipboard' context menu option
        ///// </summary>
        ///// <param name="sender">Object that fired the event</param>
        ///// <param name="e">.NET supplied event parameters</param>
        //void mnuCopyData_Click(object sender, RoutedEventArgs e)
        //{
        //    CopyToClipboard();
        //}

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

        private void RenderFinish()
        {
            waitCursor.Visibility = Visibility.Collapsed;
            pnlMainContent.Visibility = System.Windows.Visibility.Visible;
            pnlStatus.Visibility = System.Windows.Visibility.Collapsed;
            txtStatus.Text = string.Empty;
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            FilterButton.IsEnabled = true;
            if (Is2x2)
            {
                pnlMainContent.Visibility = System.Windows.Visibility.Visible;
                pnlCrosstabContent.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                pnlMainContent.Visibility = System.Windows.Visibility.Collapsed;

                pnlCrosstabContent.Visibility = System.Windows.Visibility.Visible;
            }

            //if (isGrouped)
            //{

            //}

            CheckAndSetPosition();
        }

        private void RenderStart()
        {
            //       waitCursor.Visibility = Visibility.Visible;
            txtStatus.Text = string.Empty;

            pnlMainContent.Visibility = System.Windows.Visibility.Collapsed;
            pnlCrosstabContent.Visibility = System.Windows.Visibility.Collapsed;
            spContent.Visibility = System.Windows.Visibility.Collapsed;
        }

        //private void ShowGrid()
        //{
        //    dgResults.UnselectAll();
        //    dgResults.Visibility = System.Windows.Visibility.Visible;
        //    dgResults.IsEnabled = dgResults.IsEnabled;
        //    if (dgResults != null && dgResults.Items.Count > 0)
        //    {
        //        dgResults.SelectionMode = Microsoft.Windows.Controls.DataGridSelectionMode.Single;
        //        dgResults.SelectedIndex = 0;
        //        int x = dgResults.Columns.Count;
        //    }
        //}

        private void RenderFinishWithWarning(string errorMessage)
        {
            waitCursor.Visibility = Visibility.Collapsed;
            pnlMainContent.Visibility = System.Windows.Visibility.Visible;
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            if (Is2x2)
            {
                pnlContent.Visibility = System.Windows.Visibility.Visible;
                pnlCrosstabContent.Visibility = System.Windows.Visibility.Collapsed;
                pnlStatus.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                pnlContent.Visibility = System.Windows.Visibility.Collapsed;
                pnlCrosstabContent.Visibility = System.Windows.Visibility.Visible;
                pnlStatus.Visibility = System.Windows.Visibility.Visible;
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

        private void RenderFinishWithError(string errorMessage)
        {
            //waitCursor.Visibility = Visibility.Collapsed;
            //pnlStatus.Visibility = System.Windows.Visibility.Visible;
            //txtStatus.Text = errorMessage;
            //pnlStatus.Background = new SolidColorBrush(Color.FromArgb(255, 248, 215, 226)); //Brushes.Tomato;
            //pnlStatusTop.Background = new SolidColorBrush(Color.FromArgb(255, 228, 101, 142)); //Brushes.Red;    
            //txtStatus.Visibility = System.Windows.Visibility.Visible;
            //pnlMainContent.Visibility = System.Windows.Visibility.Collapsed;
            //pnlCrosstabContent.Visibility = System.Windows.Visibility.Collapsed;
            //// CollapseConfigPanel();
            //CheckAndSetPosition();

            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            pnlStatus.Visibility = System.Windows.Visibility.Visible;
            pnlMainContent.Visibility = System.Windows.Visibility.Collapsed;
            pnlCrosstabContent.Visibility = System.Windows.Visibility.Collapsed;
            grdTable.Visibility = System.Windows.Visibility.Collapsed;

            waitCursor.Visibility = Visibility.Collapsed;
            //pnlMainContent.Visibility = System.Windows.Visibility.Collapsed;
            pnlStatus.Background = new SolidColorBrush(Color.FromArgb(255, 248, 215, 226)); //Brushes.Tomato;
            pnlStatusTop.Background = new SolidColorBrush(Color.FromArgb(255, 228, 101, 142)); //Brushes.Red; 

            //pnlStatus.Visibility = System.Windows.Visibility.Visible;
            txtStatus.Text = errorMessage;
            //txtStatus.Visibility = System.Windows.Visibility.Visible;
            //pnlMainContent.Visibility = System.Windows.Visibility.Collapsed;
            //pnlCrosstabContent.Visibility = System.Windows.Visibility.Collapsed;
            //grdTable.Visibility = System.Windows.Visibility.Collapsed;
            //// CollapseConfigPanel();
            //CheckAndSetPosition();
            CollapseConfigPanel();
            CheckAndSetPosition();
        }

        private void RequestUpdateStatusMessage(string statusMessage)
        {
            //this.Dispatcher.BeginInvoke(new SetStatusDelegate(SetStatusMessage), statusMessage);
            SetStatusMessage(statusMessage);
        }

        private void SetStatusMessage(string statusMessage)
        {
            pnlStatus.Visibility = System.Windows.Visibility.Visible;
            txtStatus.Text = statusMessage;
        }

        private bool IsCancelled()
        {
            if (worker != null && worker.WorkerSupportsCancellation && worker.CancellationPending)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void Field_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!LoadingDropDowns)
            {
                RefreshResults();
                //Serialize(new XDocument());
            }
        }

        private void FillComboboxes()    //    TwoxTwoViewModel twoxTwoViewModel, bool update = false)
        {
            LoadingDropDowns = true;

            object prevExposureField = string.Empty;
            string prevOutcomeField = string.Empty;

            //if (update)
            //{
            //    if (cbxExposureField.SelectedIndex >= 0)
            //    {
            //        if (cbxExposureField.SelectedItem is GroupField)
            //        {
            //            prevExposureField = cbxExposureField.SelectedItem;
            //        }
            //        else
            //        {
            //            prevExposureField = cbxExposureField.SelectedItem.ToString();
            //        }
            //    }
            //    if (cbxOutcomeField.SelectedIndex >= 0)
            //    {
            //        prevOutcomeField = cbxOutcomeField.SelectedItem.ToString();
            //    }
            //}

            cbxExposureField.ItemsSource = null;
            cbxExposureField.Items.Clear();

            cbxOutcomeField.ItemsSource = null;
            cbxOutcomeField.Items.Clear();

            //       ColumnDataType columnDataType = ColumnDataType.Text | ColumnDataType.Numeric | ColumnDataType.Boolean | ColumnDataType.UserDefined;

            //if (dashboardHelper.IsUsingEpiProject)
            //{
            //    List<string> fieldNames = dashboardHelper.GetFieldsAsList(columnDataType);

            //    foreach (string s in fieldNames)
            //    {
            //        cbxExposureField.Items.Add(s);
            //        cbxOutcomeField.Items.Add(s);
            //    }

            //    foreach (Epi.Page p in View.Pages)
            //    {
            //        foreach (GroupField group in p.GroupFields)
            //        {
            //            cbxExposureField.Items.Add(group);
            //        }
            //    }
            //}
            //else
            // {

            // List<string> exposureFields = dashboardHelper.GetFieldsAsList(columnDataType);    

            //  TODO = filter for column data type    

            //List<ColumnDataType> columnDataType = new List<ColumnDataType>();
            //columnDataType.Add(ColumnDataType.Boolean);
            //columnDataType.Add(ColumnDataType.Numeric);
            //columnDataType.Add(ColumnDataType.Text);
            //columnDataType.Add(ColumnDataType.UserDefined);

            //IEnumerable<EwavColumn> filteredCols = from cols in applicationViewModel.EwavSelectedDatasource.AllColumns
            //                                       where columnDataType.Contains(cols.SqlDataTypeAsString)
            //                                       orderby cols.Name
            //                                       select cols;

            List<EwavColumn> colsList = CommonClass.GetItemsSource(GetFieldDataType, false);  //filteredCols.ToList();

            this.

            //this. GetGroupVariablesAsList();
            // dpb 
            
            
            
            // List<  > outcomeFields = dashboardHelper.GetFieldsAsList(columnDataType);
            // List<EwavColumn> outcomeFields = CommonClass.GetItemsSource(columnDataType, false);  //filteredCols.ToList();

            cbxExposureField.ItemsSource = colsList;

            cbxExposureField.SelectedValue = "Index";
            cbxExposureField.DisplayMemberPath = "Name";

            cbxOutcomeField.ItemsSource = colsList;

            cbxOutcomeField.SelectedValue = "Index";
            cbxOutcomeField.DisplayMemberPath = "Name";

            //  }

            if (cbxExposureField.Items.Count > 0)
            {
                cbxExposureField.SelectedIndex = Index1;
            }
            if (cbxOutcomeField.Items.Count > 0)
            {
                cbxOutcomeField.SelectedIndex = Index2;
            }

            //if (update)
            //{
            cbxExposureField.SelectedItem = prevExposureField;
            cbxOutcomeField.SelectedItem = prevOutcomeField;
            //    }
            // delete this 

            LoadingDropDowns = false;
        }

        public List<ColumnDataType> GetFieldDataType
        {
            get
            {
                List<ColumnDataType> fieldDataType = new List<ColumnDataType>();
                fieldDataType.Add(ColumnDataType.Boolean);
                fieldDataType.Add(ColumnDataType.Numeric);
                fieldDataType.Add(ColumnDataType.Text);
                fieldDataType.Add(ColumnDataType.UserDefined);
                fieldDataType.Add(ColumnDataType.GroupVariable);

                return fieldDataType;

            }
        }

        //void Triangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    FrameworkElement source = sender as FrameworkElement;
        //    CollapseExpandConfigPanel();
        //}

        /// <summary>
        /// Handles the collapsing and expanding of the gadget's configuration panel.
        /// </summary>
        //private void CollapseExpandConfigPanel()
        //{
        //    if (triangleCollapsed)
        //    {
        //        ExpandConfigPanel();
        //    }
        //    else
        //    {
        //        CollapseConfigPanel();
        //    }
        //}

        /// <summary>
        /// Forces a collapse of the config panel
        /// </summary>
        private void CollapseConfigPanel()
        {
            // ConfigCollapsedTriangle.Visibility = Visibility.Visible;
            //  ConfigExpandedTriangle.Visibility = Visibility.Collapsed;
            //ConfigCollapsedTitle.Visibility = Visibility.Visible;
            //ConfigGrid.Height = 50;
            triangleCollapsed = true;
        }

        /// <summary>
        /// Forces an expansion of the config panel
        /// </summary>
        //private void ExpandConfigPanel()
        //{
        //    //  ConfigExpandedTriangle.Visibility = Visibility.Visible;
        //    // ConfigCollapsedTriangle.Visibility = Visibility.Collapsed;
        //    ConfigCollapsedTitle.Visibility = Visibility.Collapsed;
        //    ConfigGrid.Height = Double.NaN;
        //    ConfigGrid.UpdateLayout();
        //    triangleCollapsed = false;
        //}

        /// <summary>
        /// Closes the gadget
        /// </summary>
        //private void CloseGadget()
        //{
        //    if (worker != null && worker.WorkerSupportsCancellation)
        //    {
        //        worker.CancelAsync();
        //    }

        //    if (worker != null)
        //    {
        //        worker.DoWork -= new System.ComponentModel.DoWorkEventHandler(worker_DoWork);
        //    }
        //    if (baseWorker != null)
        //    {
        //        baseWorker.DoWork -= new System.ComponentModel.DoWorkEventHandler(Execute);
        //    }

        //    this.GadgetStatusUpdate -= new GadgetStatusUpdateHandler(RequestUpdateStatusMessage);
        //    this.GadgetCheckForCancellation -= new GadgetCheckForCancellationHandler(IsCancelled);

        //    dgResults.ItemsSource = null;

        //    if (GadgetClosing != null)
        //        GadgetClosing(this);

        //    gadgetOptions = null;

        //    GadgetClosing = null;
        //    GadgetCheckForCancellation = null;
        //    GadgetProcessingFinished = null;
        //    GadgetStatusUpdate = null;
        //}

        public void ClearSingleResults()
        {
            pnlStatus.Visibility = Visibility.Collapsed;
            txtStatus.Text = string.Empty;

            grdGraph.Visibility = Visibility.Visible;
            grdTables.Visibility = Visibility.Visible;
            waitCursor.Visibility = Visibility.Visible;
            dgResults.Visibility = Visibility.Collapsed;
            pnlCrosstabContent.Visibility = Visibility.Collapsed;

            ResetGrid();

            txtExposure.Visibility = Visibility.Visible;
        }

        //public void Execute(string exposureVar, string outcomeVar)
        //private void Execute(object sender, System.ComponentModel.DoWorkEventArgs e)
        //{
        //    if (worker != null && worker.WorkerSupportsCancellation)
        //    {
        //        worker.CancelAsync();
        //    }

        //    lock (syncLock)
        //    {
        //        worker = new System.ComponentModel.BackgroundWorker();
        //        worker.WorkerSupportsCancellation = true;
        //        worker.DoWork += new System.ComponentModel.DoWorkEventHandler(worker_DoWork);
        //        //worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(worker_WorkerCompleted);
        //        worker.RunWorkerAsync(gadgetOptions);
        //    }

        //    //string[] workerParams = new string[2];
        //    //workerParams[0] = exposureVar;
        //    //workerParams[1] = outcomeVar;

        //    //txtOutcome.Text = outcomeVar;
        //    //txtExposure.Text = exposureVar;

        //    //worker = new System.ComponentModel.BackgroundWorker();
        //    //worker.WorkerSupportsCancellation = true;
        //    //worker.DoWork += new DoWorkEventHandler(worker_DoWork);
        //    ////worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(worker_WorkerCompleted);
        //    //worker.RunWorkerAsync(workerParams);
        //}

        /// <summary>
        /// Handles the check / unchecked events
        /// </summary>
        /// <param name="sender">Object that fired the event</param>
        /// <param name="e">.NET supplied event parameters</param>
        private void checkboxCheckChanged(object sender, RoutedEventArgs e)
        {
            RefreshResults();
        }

        private void Render2X2GridCells(GridCells gridCells, TwoxTwoAndMxNResultsSet resultSet,
            TwoxTwoTableDTO twoByTwoTable)
        {
            waitCursor.Visibility = Visibility.Collapsed;

            int rowCount = 1;
            int columnCount = 1;
            bool exceededMaxRows = false;
            bool exceededMaxColumns = false;
            bool includeMissing = false;

            gridCells = resultSet.DatatableBagArray[0].GridCellData;

            //if (twoByTwoTable.Rows.Length == 3)
            //{
            //    pnlStatus.Visibility = Visibility.Visible;
            //    //pnlErrorMessages.Margin = new Thickness(0, ConfigGrid.ActualHeight, 0, 0);
            //    txtStatus.Text = twoByTwoTable.Rows[2, 0].ToString();
            //    return;
            //    }
            //else
            //{
            //pnlErrorMessages.Visibility = System.Windows.Visibility.Collapsed;
            // TODO 2x2
            pnlStatus.Visibility = Visibility.Collapsed;
            txtStatus.Text = string.Empty;
            //     }

            if (twoByTwoTable != null)  //  && twoByTwoTable.Rows.Length > 0)
            {
                pnlContent.Visibility = System.Windows.Visibility.Visible;

                exposureYes.Text = resultSet.DatatableBag.RecordList[0].Fields[0].VarName;         //        twoByTwoTable.ColumnName1;
                exposureNo.Text = resultSet.DatatableBag.RecordList[1].Fields[0].VarName;       //     twoByTwoTable.ColumnName2;

                //List<MyString> ListOfVars = new List<MyString>();
                //ListOfVars.Add(resultSet.DatatableBag.RecordList[1].Fields[0]);
                //ListOfVars.Add(resultSet.DatatableBag.RecordList[0].Fields[0]);

                //if (checkboxSmartTable.IsChecked == true)
                //{
                //    exposureYes.Text = ReadYesValue(ListOfVars);
                //    exposureNo.Text = ReadNoValue(ListOfVars);
                //}
                //else
                //{
                //    exposureYes.Text = ReadNoValue(ListOfVars);
                //    exposureNo.Text = ReadYesValue(ListOfVars);
                //}

                outcomeYes.Text = resultSet.DatatableBag.ColumnNameList[1].VarName;     //    twoByTwoTable.ColumnName1;
                outcomeNo.Text = resultSet.DatatableBag.ColumnNameList[2].VarName;      //  twoByTwoTable.ColumnName2;

                //outcomeYes.Text = ReadYesValue(resultSet.DatatableBag.ColumnNameList);
                //outcomeNo.Text = ReadNoValue(resultSet.DatatableBag.ColumnNameList);

                List<RenderGridCellsDTO> dt = new List<RenderGridCellsDTO>();

                if (gridCells.TyVal > 0 && gridCells.TnVal > 0 && gridCells.YtVal > 0 && gridCells.NtVal > 0)
                {
                    string oddsRatioEstimate = SharedStrings.UNDEFINED;
                    if (gridCells.SingleTableResults.OddsRatioEstimate != null)
                    {
                        oddsRatioEstimate = ((double)gridCells.SingleTableResults.OddsRatioEstimate).ToString("F4");
                    }

                    dt.Add(new RenderGridCellsDTO
                    {
                        Exposure = "...",
                        OutcomeRate_Exposure = gridCells.YyRowPct.ToString("P"),
                        OutcomeRate_NoExposure = gridCells.NyRowPct.ToString("P"),
                        RiskRatio = gridCells.SingleTableResults.RiskRatioEstimate == null ? "" : ((double)gridCells.SingleTableResults.RiskRatioEstimate).ToString("F4"),
                        OddsRatio = oddsRatioEstimate
                    });
                }
                else
                {
                    dt.Add(new RenderGridCellsDTO
                    {
                        Exposure = "...",
                        OutcomeRate_Exposure = gridCells.YyRowPct.ToString("P"),
                        OutcomeRate_NoExposure = gridCells.NyRowPct.ToString("P"),
                        RiskRatio = "0",
                        OddsRatio = "0"
                    });
                }

                currentSingleTableData = gridCells;

                dgResults.ItemsSource = dt;

                dgResults.Visibility = System.Windows.Visibility.Collapsed; // Temp fix only, re-work later

                txtYesYesVal.Text = gridCells.YyVal.ToString();
                txtYesYesRow.Text = gridCells.YyRowPct.ToString("P");
                txtYesYesCol.Text = gridCells.YyColPct.ToString("P");

                txtNoYesVal.Text = gridCells.NyVal.ToString();
                txtNoYesRow.Text = gridCells.NyRowPct.ToString("P");
                txtNoYesCol.Text = gridCells.NyColPct.ToString("P");

                txtTotalYesVal.Text = gridCells.TyVal.ToString();
                txtTotalYesRow.Text = gridCells.TyRowPct.ToString("P");
                // 100%

                txtYesNoVal.Text = gridCells.YnVal.ToString();
                txtYesNoRow.Text = gridCells.YnRowPct.ToString("P");
                txtYesNoCol.Text = gridCells.YnColPct.ToString("P");
                
                txtNoNoVal.Text = gridCells.NnVal.ToString();
                txtNoNoCol.Text = gridCells.NnColPct.ToString("P");
                txtNoNoRow.Text = gridCells.NnRowPct.ToString("P");

                txtTotalNoVal.Text = gridCells.TnVal.ToString();
                txtTotalNoRow.Text = gridCells.TnRowPct.ToString("P");
                // 100%

                txtYesTotalVal.Text = gridCells.YtVal.ToString();
                // m1
                txtYesTotalCol.Text = gridCells.YtColPct.ToString("P");

                txtNoTotalVal.Text = gridCells.NtVal.ToString();
                // m2
                txtNoTotalCol.Text = gridCells.NtColPct.ToString("P");

                txtTotalTotalVal.Text = gridCells.TtVal.ToString();

                if (gridCells.TyVal > 0 && gridCells.TnVal > 0 && gridCells.YtVal > 0 && gridCells.NtVal > 0)
                {
                    pnlAdvanced.Visibility = Visibility.Visible;

                    string fisherExact = SharedStrings.UNDEFINED;
                    string fisherExact2P = SharedStrings.UNDEFINED;
                    string fisherLower = SharedStrings.UNDEFINED;
                    string fisherUpper = SharedStrings.UNDEFINED;

                    string oddsRatioEstimate = SharedStrings.UNDEFINED;
                    string oddsRatioLower = SharedStrings.UNDEFINED;
                    string oddsRatioUpper = SharedStrings.UNDEFINED;

                    string oddsRatioMLEEstimate = SharedStrings.UNDEFINED;
                    string oddsRatioMLEMidPLower = SharedStrings.UNDEFINED;
                    string oddsRatioMLEMidPUpper = SharedStrings.UNDEFINED;

                    string riskRatioLower = SharedStrings.UNDEFINED;
                    string riskRatioUpper = SharedStrings.UNDEFINED;

                    if (gridCells.SingleTableResults.FisherExactP != -1)
                    {
                        fisherExact = gridCells.SingleTableResults.FisherExactP == null ? "" : ((double)gridCells.SingleTableResults.FisherExactP).ToString("F10");
                    }

                    if (gridCells.SingleTableResults.FisherExact2P != -1)
                    {
                        fisherExact2P = gridCells.SingleTableResults.FisherExact2P == null ? "" : ((double)gridCells.SingleTableResults.FisherExact2P).ToString("F10");
                    }

                    if (gridCells.SingleTableResults.OddsRatioMLEFisherLower != -1)
                    {
                        fisherLower = gridCells.SingleTableResults.OddsRatioMLEFisherLower == null ? "" : ((double)gridCells.SingleTableResults.OddsRatioMLEFisherLower).ToString("F4");
                    }

                    if (gridCells.SingleTableResults.OddsRatioMLEFisherUpper != -1)
                    {
                        fisherUpper = gridCells.SingleTableResults.OddsRatioMLEFisherUpper == null ? "" : ((double)gridCells.SingleTableResults.OddsRatioMLEFisherUpper).ToString("F4");
                    }

                    if (gridCells.SingleTableResults.OddsRatioMLEEstimate != -1)
                    {
                        oddsRatioMLEEstimate = gridCells.SingleTableResults.OddsRatioMLEEstimate == null ? "" : ((double)gridCells.SingleTableResults.OddsRatioMLEEstimate).ToString("F4");
                    }

                    if (gridCells.SingleTableResults.OddsRatioMLEMidPLower != -1)
                    {
                        oddsRatioMLEMidPLower = gridCells.SingleTableResults.OddsRatioMLEMidPLower == null ? "" : ((double)gridCells.SingleTableResults.OddsRatioMLEMidPLower).ToString("F4");
                    }

                    if (gridCells.SingleTableResults.OddsRatioMLEMidPUpper != -1)
                    {
                        oddsRatioMLEMidPUpper = gridCells.SingleTableResults.OddsRatioMLEMidPUpper == null ? "" : ((double)gridCells.SingleTableResults.OddsRatioMLEMidPUpper).ToString("F4");
                    }

                    if (gridCells.SingleTableResults.OddsRatioEstimate != null)
                    {
                        oddsRatioEstimate = ((double)gridCells.SingleTableResults.OddsRatioEstimate).ToString("F4");
                    }

                    if (gridCells.SingleTableResults.OddsRatioLower != null)
                    {
                        oddsRatioLower = ((double)gridCells.SingleTableResults.OddsRatioLower).ToString("F4");
                    }

                    if (gridCells.SingleTableResults.OddsRatioUpper != null)
                    {
                        oddsRatioUpper = ((double)gridCells.SingleTableResults.OddsRatioUpper).ToString("F4");
                    }

                    if (gridCells.SingleTableResults.RiskRatioLower != null)
                    {
                        riskRatioLower = ((double)gridCells.SingleTableResults.RiskRatioLower).ToString("F4");
                    }

                    if (gridCells.SingleTableResults.RiskRatioUpper != null)
                    {
                        riskRatioUpper = ((double)gridCells.SingleTableResults.RiskRatioUpper).ToString("F4");
                    }

                    txtChiSqCorP.Text = gridCells.SingleTableResults.ChiSquareYates2P == null ? "" : ((double)gridCells.SingleTableResults.ChiSquareYates2P).ToString("F10");
                    txtChiSqCorVal.Text = gridCells.SingleTableResults.ChiSquareYatesVal == null ? "" : ((double)gridCells.SingleTableResults.ChiSquareYatesVal).ToString("F4");
                    txtChiSqManP.Text = gridCells.SingleTableResults.ChiSquareMantel2P == null ? "" : ((double)gridCells.SingleTableResults.ChiSquareMantel2P).ToString("F10");
                    txtChiSqManVal.Text = gridCells.SingleTableResults.ChiSquareMantelVal == null ? "" : ((double)gridCells.SingleTableResults.ChiSquareMantelVal).ToString("F4");
                    txtChiSqUncP.Text = gridCells.SingleTableResults.ChiSquareUncorrected2P == null ? "" : ((double)gridCells.SingleTableResults.ChiSquareUncorrected2P).ToString("F10");
                    txtChiSqUncVal.Text = gridCells.SingleTableResults.ChiSquareUncorrectedVal == null ? "" : ((double)gridCells.SingleTableResults.ChiSquareUncorrectedVal).ToString("F4");
                    txtOddsRatioEstimate.Text = oddsRatioEstimate;
                    txtOddsRatioLower.Text = oddsRatioLower;
                    txtOddsRatioUpper.Text = oddsRatioUpper;
                    txtMidPEstimate.Text = oddsRatioMLEEstimate;
                    txtMidPLower.Text = oddsRatioMLEMidPLower;
                    txtMidPUpper.Text = oddsRatioMLEMidPUpper;
                    txtFisherLower.Text = fisherLower;
                    txtFisherUpper.Text = fisherUpper;
                    txtRiskDifferenceEstimate.Text = gridCells.SingleTableResults.RiskDifferenceEstimate == null ? "" : ((double)gridCells.SingleTableResults.RiskDifferenceEstimate).ToString("F4");
                    txtRiskDifferenceLower.Text = gridCells.SingleTableResults.RiskDifferenceLower == null ? "" : ((double)gridCells.SingleTableResults.RiskDifferenceLower).ToString("F4");
                    txtRiskDifferenceUpper.Text = gridCells.SingleTableResults.RiskDifferenceUpper == null ? "" : ((double)gridCells.SingleTableResults.RiskDifferenceUpper).ToString("F4");
                    txtRiskRatioEstimate.Text = gridCells.SingleTableResults.RiskRatioEstimate == null ? "" : ((double)gridCells.SingleTableResults.RiskRatioEstimate).ToString("F4");
                    txtRiskRatioLower.Text = riskRatioLower;
                    txtRiskRatioUpper.Text = riskRatioUpper;
                    txtFisherExact.Text = fisherExact;
                    txtFisherExact2P.Text = fisherExact2P;
                    txtMidPExact.Text = gridCells.SingleTableResults.MidP == null ? "" : ((double)gridCells.SingleTableResults.MidP).ToString("F10");
                }
                else
                {
                    pnlAdvanced.Visibility = Visibility.Collapsed;
                }
                rctRed.Height = 1;
                rctRed.Width = 1;
                rctYellow.Height = 1;
                rctYellow.Width = 1;
                rctOrange.Height = 1;
                rctOrange.Width = 1;
                rctGreen.Height = 1;
                rctGreen.Width = 1;

                //DoubleAnimation daRedWidth = new DoubleAnimation();
                //DoubleAnimation daRedHeight = new DoubleAnimation();

                AnimateSquare(rctRed, gridCells.YyVal, gridCells);
                AnimateSquare(rctYellow, gridCells.NyVal, gridCells);
                AnimateSquare(rctOrange, gridCells.YnVal, gridCells);
                AnimateSquare(rctGreen, gridCells.NnVal, gridCells);
                ////==============================
                //Storyboard sb = new Storyboard();
                //sb.Children.Add(daBar);
                //Storyboard.SetTarget(daBar, rctBar);
                //Storyboard.SetTargetProperty(daBar, new PropertyPath("Width"));
                //if (!LayoutRoot.Resources.Contains("unique_id"))
                //{
                //    LayoutRoot.Resources.Add("unique_id", sb);
                //}
                //sb.Begin();
                ////==============================
                //DoubleAnimation daOrange = new DoubleAnimation();
                //daOrange.From = 1;
                //if (gridCells.TtVal != 0)
                //    daOrange.To = 149.0 * gridCells.YnVal / gridCells.TtVal;
                //else
                //    daOrange.To = 1;
                //daOrange.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                //rctOrange.BeginAnimation(Rectangle.HeightProperty, daOrange);
                //rctOrange.BeginAnimation(Rectangle.WidthProperty, daOrange);
                //DoubleAnimation daGreen = new DoubleAnimation();
                //daGreen.From = 1;
                //if (gridCells.TtVal != 0)
                //    daGreen.To = 149.0 * gridCells.NnVal / gridCells.TtVal;
                //else
                //    daGreen.To = 1;
                //daGreen.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                //rctGreen.BeginAnimation(Rectangle.HeightProperty, daGreen);
                //rctGreen.BeginAnimation(Rectangle.WidthProperty, daGreen);
            }
            else
            {
                ResetGrid();
            }
            //    RenderFinish();
        }

        private string ReadYesValue(List<MyString> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].VarName == "1" || list[i].VarName.ToLower() == "yes")
                {
                    return list[i].VarName;
                }
            }
            return "";
        }

        private string ReadNoValue(List<MyString> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].VarName == "0" || list[i].VarName.ToLower() == "no")
                {
                    return list[i].VarName;
                }
            }
            return "";
        }

        private void RenderMxNGridCells(TwoxTwoAndMxNResultsSet resultSet)
        {
            AddFreqGridDelegate addGrid = new AddFreqGridDelegate(AddEwavFreqGrid);
            SetGridTextDelegate setText = new SetGridTextDelegate(SetEwavGridText);
            AddGridRowDelegate addRow = new AddGridRowDelegate(AddEwavGridRow);
            //      RenderFrequencyHeaderDelegate renderHeader = new RenderFrequencyHeaderDelegate(RenderFrequencyHeader);
            //DrawFrequencyBordersDelegate drawBorders = new DrawFrequencyBordersDelegate(DrawFrequencyBorders);

            int rowCount = 1;
            int columnCount = 1;
            bool exceededMaxRows = false;
            bool exceededMaxColumns = false;
            bool includeMissing = false;

            StringLiterals sl = new StringLiterals();

            TwoxTwoTableDTO table = resultSet.TwoxTwoTableDTO;

            DatatableBag datatableBag = resultSet.DatatableBag;

            string strataValue = resultSet.TableName;
            string freqVar = gadgetOptions.MainVariableName;

            //addGrid(string.Empty, "Tablename here", datatableBag.ColumnNameList.Count);

            pnlContent.Visibility = System.Windows.Visibility.Visible;

            double count = 0;

            if(resultSet != null && resultSet.FreqResultsDescriptiveStatistics != null)
            { 
                foreach (DescriptiveStatistics ds in resultSet.FreqResultsDescriptiveStatistics)
                {
                    count = count + ds.Observations;
                }
            }

            if (count == 0)
            {
                //  this.Dispatcher.BeginInvoke(new RenderFinishWithErrorDelegate(RenderFinishWithError), SharedStrings.NO_RECORDS_SELECTED);
                RenderFinishWithError(SharedStrings.NO_RECORDS_SELECTED);

                //  this.Dispatcher.BeginInvoke(new SimpleCallback(SetGadgetToFinishedState));
                SetGadgetToFinishedState();

                return;
            }

            if (resultSet.MxNGridRows.Count == 0)
            {
                //  this.Dispatcher.BeginInvoke(new RenderFinishWithErrorDelegate(RenderFinishWithError), SharedStrings.NO_RECORDS_SELECTED);
                RenderFinishWithError(SharedStrings.NO_RECORDS_SELECTED);

                //  this.Dispatcher.BeginInvoke(new SimpleCallback(SetGadgetToFinishedState));
                SetGadgetToFinishedState();

                return;
            }

            string tableHeading = ((EwavColumn)cbxExposureField.SelectedItem).Name;//strataValue;

            //  this.Dispatcher.BeginInvoke(renderHeader, strataValue, tableHeading, table.Columns);
            RenderFrequencyHeader(strataValue, tableHeading, resultSet);

            //      int rowCount = 1;

            int[] totals = new int[resultSet.DatatableBag.ColumnNameList.Count - 1];
            //     int columnCount = 1;

            //     bool exceededMaxColumns = false;

            foreach (FieldsList row in datatableBag.RecordList)
            {
                //  !row[freqVar].Equals(DBNull.Value) || (row[freqVar].Equals(DBNull.Value) && includeMissing == true))  
                //if (7 == 7)
                //{
                #region   displayValue stuff
                //Field field = null;
                //foreach (MyString fieldRow in dashboardHelper.FieldTable.Rows)
                //{
                //    if (fieldRow["columnname"].Equals(freqVar))
                //    {
                //        if (fieldRow["epifieldtype"] is Field)
                //        {
                //            field = fieldRow["epifieldtype"] as Field;
                //        }
                //        break;
                //    }
                //}
                ////    //  this.Dispatcher.Invoke(addRow, strataValue, 30);    
                addRow(strataValue, 30);
                string displayValue = datatableBag.GetValueAtRow(freqVar, row);                    //            row.            row[freqVar].ToString();     

                ////var enumerator = "aaaa";

                ////enumerator = datatableBag.GetValue(freqVar);

                //if (dashboardHelper.IsUserDefinedColumn(freqVar))
                //{
                //    displayValue = dashboardHelper.GetFormattedOutput(freqVar, row[freqVar]);
                //}
                //else
                //{
                //    if (field != null && field is YesNoField)
                //    {
                //        if (row[freqVar].ToString().Equals("1"))
                //            displayValue = "Yes";
                //        else if (row[freqVar].ToString().Equals("0"))
                //            displayValue = "No";
                //    }
                //    else if (field != null && field is DateField)
                //    {
                //        displayValue = string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:d}", row[freqVar]);
                //    }
                //    else if (field != null && field is TimeField)
                //    {
                //        displayValue = string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:T}", row[freqVar]);
                //    }
                //    else
                //    {
                //        displayValue = dashboardHelper.GetFormattedOutput(freqVar, row[freqVar]);
                //    }
                //}

                //if (string.IsNullOrEmpty(displayValue))
                //{
                //    Configuration config = dashboardHelper.Config;
                //    displayValue = config.Settings.RepresentationOfMissing;
                //}

                #endregion

                setText(strataValue, new TextBlockConfig(string.Format("{0}{1}{2}", sl.SPACE, displayValue, sl.SPACE), new Thickness(2, 0, 2, 0),
                    VerticalAlignment.Center, HorizontalAlignment.Left, rowCount, 0),
                    FontWeights.Normal);

                int rowTotal = 0;
                columnCount = 1;

                foreach (MyString columnName in resultSet.DatatableBag.ColumnNameList)
                {
                    if (columnCount > MaxColumns)
                    {
                        //this.Dispatcher.BeginInvoke(new ShowWarningDelegate(ShowWarning), (frequencies.Columns.Count - maxColumns).ToString() + " additional allColumns were not displayed due to gadget settings.");
                        exceededMaxColumns = true;
                        break;
                    }

                    if (columnName.VarName.Equals(freqVar))    //  .ColumnName
                    {
                        continue;
                    }

                    //  this.Dispatcher.BeginInvoke(setText, strataValue, new TextBlockConfig(sl.SPACE + row[column.ColumnName].ToString() + sl.SPACE, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, columnCount), FontWeights.Normal);

                    setText(strataValue,
                        new TextBlockConfig(string.Format("{0}{1}{2}", sl.SPACE, datatableBag.GetValueAtRow(columnName.VarName, row), sl.SPACE),
                            new Thickness(2, 0, 2, 0),
                            VerticalAlignment.Center,
                            HorizontalAlignment.Right,
                            rowCount, columnCount),
                        FontWeights.Normal);

                    columnCount++;

                    int rowValue = 0;
                    bool success = int.TryParse(datatableBag.GetValueAtRow(columnName.VarName, row).ToString(), out rowValue);
                    if (success)
                    {
                        totals[columnCount - 2] = totals[columnCount - 2] + rowValue;
                        rowTotal = rowTotal + rowValue;
                    }
                }//      foreach (MyString columnName in resultSet.DatatableBag.ColumnNameList)               

                //  this.Dispatcher.BeginInvoke(setText, strataValue, new TextBlockConfig(sl.SPACE + rowTotal.ToString() + sl.SPACE, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, columnCount), FontWeights.Bold);                    
                setText(strataValue,
                    new TextBlockConfig(
                        string.Format("{0}{1}{2}", sl.SPACE, rowTotal.ToString(), sl.SPACE),
                        new Thickness(2, 0, 2, 0),
                        VerticalAlignment.Center,
                        HorizontalAlignment.Right,
                        rowCount, columnCount),
                    FontWeights.Bold);

                rowCount++;

                if (rowCount > MaxRows)
                {
                    foreach (MyString column in datatableBag.ColumnNameList)
                    {
                        if (columnCount > MaxColumns)
                        {
                            exceededMaxColumns = true;
                            break;
                        }

                        if (column.VarName.Equals(freqVar))
                        {
                            continue;
                        }

                        //  this.Dispatcher.BeginInvoke(setText, strataValue, new TextBlockConfig(sl.SPACE + sl.ELLIPSIS + sl.SPACE, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, columnCount), FontWeights.Normal);
                        setText(strataValue,
                            new TextBlockConfig(
                                string.Format("{0}{1}{2}", sl.SPACE, sl.ELLIPSIS, sl.SPACE),
                                new Thickness(2, 0, 2, 0),
                                VerticalAlignment.Center,
                                HorizontalAlignment.Right,
                                rowCount, columnCount),
                            FontWeights.Normal);

                        columnCount++;
                    }//     foreach (DataColumn column in table.Columns)

                    //  this.Dispatcher.BeginInvoke(setText, strataValue, new TextBlockConfig(sl.SPACE + sl.ELLIPSIS + sl.SPACE, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, columnCount), FontWeights.Bold);
                    setText(strataValue, new TextBlockConfig(string.Format("{0}{1}{2}", sl.SPACE, sl.ELLIPSIS, sl.SPACE),
                        new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right,
                        rowCount, columnCount), FontWeights.Bold);

                    rowCount++;
                    //     bool exceededMaxRows = true;
                    break;
                }//   if (rowCount > MaxRows)
            }//    foreach (FieldsList row in datatableBag.RecordList)      

            RenderFrequencyFooter(strataValue, rowCount, totals);

            drawBorders(strataValue);
        }

        //aaaa 
        private void AnimateSquare(Rectangle thisRect, int quadValue, GridCells gridCells)
        {
            DoubleAnimation thisDAWidth = new DoubleAnimation();
            DoubleAnimation thisDAHeight = new DoubleAnimation();

            thisDAWidth.From = 1;
            if (gridCells.TtVal != 0)
            {
                thisDAWidth.To = 149.0 * quadValue / gridCells.TtVal;
            }
            else
            {
                thisDAWidth.To = 1;
            }
            thisDAWidth.Duration = new Duration(TimeSpan.FromSeconds(0.5));

            thisDAHeight.From = 1;
            if (gridCells.TtVal != 0)
            {
                thisDAHeight.To = 149.0 * quadValue / gridCells.TtVal;
            }
            else
            {
                thisDAHeight.To = 1;
            }
            thisDAHeight.Duration = new Duration(TimeSpan.FromSeconds(0.5));

            Storyboard sb = new Storyboard();
            sb.Children.Add(thisDAWidth);
            sb.Children.Add(thisDAHeight);

            Storyboard.SetTarget(thisDAWidth, thisRect);
            Storyboard.SetTarget(thisDAHeight, thisRect);
            Storyboard.SetTargetProperty(thisDAWidth, new PropertyPath("Width"));
            Storyboard.SetTargetProperty(thisDAHeight, new PropertyPath("Height"));

            if (!LayoutRoot.Resources.Contains("unique_id"))
            {
                LayoutRoot.Resources.Add("unique_id", sb);
            }

            sb.Begin();
        }

        private void ResetGrid()
        {
            txtNoNoCol.Text = "0";
            txtNoNoRow.Text = "0";
            txtNoNoVal.Text = "0";

            txtNoTotalCol.Text = "0";
            txtNoTotalRow.Text = (1).ToString("P");
            txtNoTotalVal.Text = "0";

            txtNoYesCol.Text = "0";
            txtNoYesRow.Text = "0";
            txtNoYesVal.Text = "0";

            txtTotalNoCol.Text = (1).ToString("P");
            txtTotalNoRow.Text = "0";
            txtTotalNoVal.Text = "0";

            txtTotalTotalCol.Text = (1).ToString("P");
            txtTotalTotalRow.Text = (1).ToString("P");
            txtTotalTotalVal.Text = "0";

            txtTotalYesCol.Text = (1).ToString("P");
            txtTotalYesRow.Text = "0";
            txtTotalYesVal.Text = "0";

            txtYesNoCol.Text = "0";
            txtYesNoRow.Text = "0";
            txtYesNoVal.Text = "0";

            txtYesTotalCol.Text = "0";
            txtYesTotalRow.Text = (1).ToString("P");
            txtYesTotalVal.Text = "0";

            txtYesYesCol.Text = "0";
            txtYesYesRow.Text = "0";
            txtYesYesVal.Text = "0";

            txtChiSqUncVal.Text = string.Empty;
            txtChiSqUncP.Text = string.Empty;
            txtChiSqManVal.Text = string.Empty;
            txtChiSqManP.Text = string.Empty;
            txtChiSqCorVal.Text = string.Empty;
            txtChiSqCorP.Text = string.Empty;

            txtMidPExact.Text = string.Empty;
            txtFisherExact.Text = string.Empty;

            txtRiskRatioEstimate.Text = string.Empty;
            txtRiskRatioLower.Text = string.Empty;
            txtRiskRatioUpper.Text = string.Empty;

            txtRiskDifferenceEstimate.Text = string.Empty;
            txtRiskDifferenceLower.Text = string.Empty;
            txtRiskDifferenceUpper.Text = string.Empty;

            txtOddsRatioEstimate.Text = string.Empty;
            txtOddsRatioLower.Text = string.Empty;
            txtOddsRatioUpper.Text = string.Empty;

            txtMidPEstimate.Text = string.Empty;
            txtMidPLower.Text = string.Empty;
            txtMidPUpper.Text = string.Empty;
            txtFisherEstimate.Text = string.Empty;
            txtFisherLower.Text = string.Empty;
            txtFisherUpper.Text = string.Empty;

            pnlAdvanced.Visibility = Visibility.Collapsed;
            pnlContent.Visibility = System.Windows.Visibility.Collapsed;

            //   dgResults.IsEnabled = true;

            grdTable.Children.Clear();
            grdTable.ColumnDefinitions.Clear();
            grdTable.RowDefinitions.Clear();
        }

        private void AddEwavFreqGrid(string strataVar, string value, int columnCount)
        {
            Grid grid = grdTable;
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();
            grid.Tag = value;
            //grid.Width = grdFreq.Width;
            grid.HorizontalAlignment = HorizontalAlignment.Center;
            grid.Margin = new Thickness(10, 10, 10, 10);//new Thickness(2, 94, 2, 2);
            grid.Visibility = System.Windows.Visibility.Visible;

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
            //   gridLabelsList.Add(txtGridLabel);
            //       pnlMainContent.Children.Add(txtGridLabel);
            //      pnlMainContent.Children.Add(grid);
            //    strataGridList.Add(grid);
        }

        private void SetEwavGridText(string strataValue, TextBlockConfig textBlockConfig, FontWeight fontWeight)
        {
            Grid grid = new Grid();

            grid = grdTable;

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

        private void AddEwavGridRow(string strataValue, int height)
        {
            Grid grid = grdTable;

            waitCursor.Visibility = Visibility.Collapsed;
            grid.Visibility = Visibility.Visible;
            RowDefinition rowDef = new RowDefinition();
            rowDef.Height = new GridLength(height);
            grid.RowDefinitions.Add(rowDef);
        }

        private void RenderFrequencyHeader(string strataValue, string freqVar, TwoxTwoAndMxNResultsSet resultSet)
        {
            Grid grid = grdTable;

            StringLiterals sl = new StringLiterals();

            RowDefinition rowDefHeader = new RowDefinition();
            rowDefHeader.Height = new GridLength(38);
            grid.RowDefinitions.Add(rowDefHeader);

            for (int y = 0; y < grid.ColumnDefinitions.Count; y++)
            {
                Rectangle rctHeader = new Rectangle();
                //     rctHeader.Fill = SystemColors.HighlightColor; 
                rctHeader.Style = Application.Current.Resources["HeaderCell"] as Style;
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
            //txtValHeader.Foreground = new SolidColorBrush(Colors.White);
            txtValHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtValHeader, 0);
            Grid.SetColumn(txtValHeader, 0);
            grid.Children.Add(txtValHeader);

            int maxColumnLength = MaxColumnLength;

            List<MyString> allColumns = resultSet.DatatableBag.ColumnNameList;

            TextBlock txtColHeader = new TextBlock();

            for (int i = 1; i < allColumns.Count; i++)
            {
                if (i > MaxColumns)
                {
                    break;
                }
                txtColHeader = new TextBlock();
                string columnName = allColumns[i].VarName.Trim();

                if (columnName.Length > maxColumnLength)
                {
                    columnName = columnName.Substring(0, maxColumnLength);
                }

                txtColHeader.Text = string.Format("{0}{1}{2}", sl.SPACE, columnName, sl.SPACE);
                //txtColHeader.VerticalAlignment = VerticalAlignment.Center;
                //txtColHeader.HorizontalAlignment = HorizontalAlignment.Center;
                //txtColHeader.Margin = new Thickness(2, 0, 2, 0);
                //txtColHeader.FontWeight = FontWeights.Bold;
                //txtColHeader.Foreground = new SolidColorBrush(Colors.White);
                txtColHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
                Grid.SetRow(txtColHeader, 0);
                Grid.SetColumn(txtColHeader, i);
                grid.Children.Add(txtColHeader);
            }

            TextBlock txtRowTotalHeader = new TextBlock();
            txtRowTotalHeader.Text = string.Format("{0}{1}{2}", sl.SPACE, SharedStrings.TOTAL, sl.SPACE);
            //txtRowTotalHeader.VerticalAlignment = VerticalAlignment.Center;
            // txtRowTotalHeader.HorizontalAlignment = HorizontalAlignment.Center;
            //txtRowTotalHeader.Margin = new Thickness(2, 0, 2, 0);
            // txtRowTotalHeader.FontWeight = FontWeights.Bold;
            //     txtRowTotalHeader.Foreground = Brushes.White;  
            //txtColHeader.Foreground = new SolidColorBrush(Colors.Black);
            txtRowTotalHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtRowTotalHeader, 0);
            Grid.SetColumn(txtRowTotalHeader, MaxColumns + 1);
            grid.Children.Add(txtRowTotalHeader);
        }

        private void RenderFrequencyFooter(string strataValue, int footerRowIndex, int[] totalRows)
        {
            StringLiterals sl = new StringLiterals();

            Grid grid = grdTable;

            RowDefinition rowDefTotals = new RowDefinition();
            rowDefTotals.Height = new GridLength(30);
            grid.RowDefinitions.Add(rowDefTotals);

            TextBlock txtValTotals = new TextBlock();
            txtValTotals.Text = string.Format("{0}{1}{2}", sl.SPACE, SharedStrings.TOTAL, sl.SPACE);
            txtValTotals.Margin = new Thickness(2, 0, 2, 0);
            txtValTotals.VerticalAlignment = VerticalAlignment.Center;
            txtValTotals.FontWeight = FontWeights.Bold;
            //txtValTotals.Style = Application.Current.Resources["HeaderFont"] as Style;
            //txtValTotals.HorizontalAlignment = HorizontalAlignment.Right;
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
                //txtFreqTotals.Style = Application.Current.Resources["HeaderFont"] as Style;
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
            //txtOverallTotal.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtOverallTotal, footerRowIndex);
            Grid.SetColumn(txtOverallTotal, MaxColumns + 1);
            grid.Children.Add(txtOverallTotal);
        }

        //private void DrawFrequencyBorders(string strataValue)
        //{
        //    Grid grid = grdTable;

        //    waitCursor.Visibility = Visibility.Collapsed;
        //    int rdcount = 0;
        //    foreach (RowDefinition rd in grid.RowDefinitions)
        //    {
        //        int cdcount = 0;
        //        foreach (ColumnDefinition cd in grid.ColumnDefinitions)
        //        {
        //            Rectangle rctBorder = new Rectangle();
        //            rctBorder.Stroke = Brushes.Black;
        //            Grid.SetRow(rctBorder, rdcount);
        //            Grid.SetColumn(rctBorder, cdcount);
        //            grid.Children.Add(rctBorder);
        //            cdcount++;
        //        }
        //        rdcount++;
        //    }
        //}

        #region IGadget Members

        /// <summary>
        /// Sets the gadget to its 'processing' state
        /// </summary>
        public void SetGadgetToProcessingState()
        {
            this.IsProcessing = true;
            this.cbxExposureField.IsEnabled = false;
            this.cbxOutcomeField.IsEnabled = false;
            this.checkboxSmartTable.IsEnabled = false;
            //  this.dgResults.IsEnabled = false;
            //     this.dgResults.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Sets the gadget to its 'finished' state
        /// </summary>
        public void SetGadgetToFinishedState()
        {
            this.IsProcessing = false;
            this.cbxExposureField.IsEnabled = true;
            this.cbxOutcomeField.IsEnabled = true;
            this.checkboxSmartTable.IsEnabled = true;

            if (isGrouped)
            {
                //  this.dgResults.IsEnabled = true;
                //     this.dgResults.Visibility = System.Windows.Visibility.Visible;
            }
            //if (GadgetProcessingFinished != null)
            //    GadgetProcessingFinished(this);
        }

        /// <summary>
        /// Gets/sets whether the gadget is processing
        /// </summary>
        public bool IsProcessing { get; set; }

        private void CreateInputVariableList()
        {
            Dictionary<string, string> inputVariableList = new Dictionary<string, string>();

            gadgetOptions.MainVariableName = string.Empty;
            gadgetOptions.WeightVariableName = string.Empty;
            gadgetOptions.StrataVariableNames = new List<string>();
            gadgetOptions.CrosstabVariableName = string.Empty;
            gadgetOptions.InputVariableList = new Dictionary<string, string>();

            if (checkboxSmartTable.IsChecked == true)
            {
                inputVariableList.Add("smarttable", "true");
            }
            else
            {
                inputVariableList.Add("smarttable", "false");
            }

            if (cbxOutcomeField.SelectedIndex > -1 && !string.IsNullOrEmpty(cbxOutcomeField.SelectedItem.ToString()))
            {
                gadgetOptions.CrosstabVariableName = ((EwavColumn)cbxOutcomeField.SelectedItem).Name;
            }


            if (cbxExposureField.SelectedIndex > -1 && !string.IsNullOrEmpty(cbxExposureField.SelectedItem.ToString()))
            {
                bool isGroupVariable = false;
                string selectedExposureName = string.Empty;

                if (cbxExposureField.SelectedItem is Ewav.BAL.EwavColumn)
                {
                    isGroupVariable = ((Ewav.BAL.EwavColumn)cbxExposureField.SelectedItem).SqlDataTypeAsString == ColumnDataType.GroupVariable;
                    selectedExposureName = ((EwavColumn)cbxExposureField.SelectedItem).Name;
                }
                else
                {
                    return;
                }

                gadgetOptions.MainVariableName = selectedExposureName;

                if (isGroupVariable)
                {
                    List<string> groupFieldNames = GetVariablesInGroup(selectedExposureName);
                    gadgetOptions.MainVariableNames = groupFieldNames;
                }
                else
                {
                    List<string> groupFieldNames = new List<string>();
                    groupFieldNames.Add(selectedExposureName);
                    gadgetOptions.MainVariableNames = groupFieldNames;
                }
            }
            else
            {
                return;
            }


            //       }
            //else if ((cbxExposureField.SelectedItem is GroupField) && gadgetOptions != null)
            //{
            //    if (cbxExposureField.SelectedIndex > -1 && !string.IsNullOrEmpty(cbxExposureField.SelectedItem.ToString()))
            //    {
            //        string[] fields = ((GroupField)cbxExposureField.SelectedItem).ChildFieldNames.Split(',');
            //        foreach (string field in fields)
            //        {
            //            //  TODO    
            //            //    
            //            //if ((View.Fields[field] is YesNoField) || (View.Fields[field] is CheckBoxField))
            //            //{
            //            //    inputVariableList.Add(field, "groupfield");
            //            //}
            //        }
            //    }
            //    else
            //    {
            //               return;
            //}

            if (cbxOutcomeField.SelectedIndex > -1 && !string.IsNullOrEmpty(cbxOutcomeField.SelectedItem.ToString()))
            {
                gadgetOptions.CrosstabVariableName = ((EwavColumn)cbxOutcomeField.SelectedItem).Name;
            }
            //    }

            gadgetOptions.ShouldIncludeFullSummaryStatistics = false;
            gadgetOptions.InputVariableList = inputVariableList;
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

        /// <summary>
        /// Updates the variable names available in the gadget's properties
        /// </summary>
        public void UpdateVariableNames()
        {
            //        FillComboboxes(true);
        }

        public StringBuilder HtmlBuilder { get; set; }

        public void RefreshResults()
        {
            //    if (!loadingCombos && cbxExposureField.SelectedIndex > -1 && cbxOutcomeField.SelectedIndex > -1)
            if (!LoadingDropDowns &&
                cbxExposureField.SelectedIndex > -1 && cbxOutcomeField.SelectedIndex > -1)
            {
                pnlStatus.Background = new SolidColorBrush(Color.FromArgb(255, 235, 245, 214)); //Brushes.PaleGreen;
                pnlStatusTop.Background = new SolidColorBrush(Color.FromArgb(255, 162, 208, 64)); //Brushes.Green;     

                if (gadgetOptions != null)    //  !(cbxExposureField.SelectedItem is GroupField) && 
                {
                    IsUserDefindVariableInUse();
                    CreateInputVariableList();
                    isGrouped = false;

                    txtOutcome.Text = ((EwavColumn)cbxOutcomeField.SelectedItem).Name;
                    txtExposure.Text = ((EwavColumn)cbxExposureField.SelectedItem).Name;

                    waitCursor.Visibility = Visibility.Visible;

                    gadgetOptions.TableName = applicationViewModel.EwavSelectedDatasource.TableName;
                    gadgetOptions.DatasourceName = applicationViewModel.EwavSelectedDatasource.DatasourceName;
                    gadgetOptions.UseAdvancedDataFilter = applicationViewModel.UseAdvancedFilter;
                    this.gadgetOptions.GadgetFilters = GadgetFilters;


                    twoxtwoViewModel.SetupGadget(gadgetOptions);
                }
                else
                {
                    throw new Exception("This code should not be reached! ");
                    //ResetGrid();
                    //CreateInputVariableList();
                    //isGrouped = true;
                    //txtOutcome.Text = gadgetOptions.CrosstabVariableName;
                    //string[] fields = ((GroupField)cbxExposureField.SelectedItem).ChildFieldNames.Split(',');
                    //List<string> binaryFields = new List<string>();
                    //foreach (string field in fields)
                    //{
                    //    if ((View.Fields[field] is YesNoField) || (View.Fields[field] is CheckBoxField))
                    //    {
                    //        binaryFields.Add(field);
                    //    }
                    //}
                    //if (binaryFields.Count > 0)
                    //{
                    //    Execute(binaryFields, cbxOutcomeField.SelectedItem.ToString());
                    //}
                    //waitCursor.Visibility = Visibility.Visible;
                    //baseWorker = new BackgroundWorker();
                    //baseWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(ExecuteGroup);
                    //baseWorker.RunWorkerAsync();
                }
            }
        }

        /// <summary>
        /// Generates Xml representation of this gadget
        /// </summary>
        /// <param name="doc">The Xml docment</param>
        /// <returns>XmlNode</returns>
        public XNode Serialize(XDocument doc)
        {
            string exposure = string.Empty;
            string outcome = string.Empty;

            if (cbxExposureField.SelectedIndex > -1)
            {
                exposure = ((EwavColumn)cbxExposureField.SelectedItem).Name.ToString().Replace("<", "&lt;");
            }

            if (cbxOutcomeField.SelectedIndex > -1)
            {
                outcome = ((EwavColumn)cbxOutcomeField.SelectedItem).Name.ToString().Replace("<", "&lt;");
            }



            string mainVar = "", crosstab = "";

            if (cbxExposureField.SelectedItem != null)
            {
                mainVar = ((EwavColumn)cbxExposureField.SelectedItem).Name;
            }

            if (cbxOutcomeField.SelectedItem != null)
            {
                crosstab = ((EwavColumn)cbxOutcomeField.SelectedItem).Name;
            }

            XElement element = new XElement("gadget",
                new XAttribute("top", Canvas.GetTop(this).ToString("F0")),
                new XAttribute("left", Canvas.GetLeft(this).ToString("F0")),
                new XAttribute("collapsed", "false"),
                new XAttribute("gadgetType", "Ewav.TwoxTwoTableControl"),
                new XElement("MainVariableName", mainVar),
                new XElement("CrosstabVariableName", crosstab),
                new XElement("ShouldIncludeFullSummaryStatistics", gadgetOptions.ShouldIncludeFullSummaryStatistics),
                new XElement("exposurevariable", exposure),
                new XElement("outcomevariable", outcome),
                new XElement("showCumulativePercent", checkboxShowPercents.IsChecked),
                new XElement("smarttable", checkboxSmartTable.IsChecked),
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
        /// Creates the 2x2 table gadget from an Xml element
        /// </summary>
        /// <param name="element">The element from which to create the gadget</param>
        //public void CreateFromXml(XmlElement element)
        //{
        //    this.LoadingDropDowns = true;

        //    foreach (XmlElement child in element.ChildNodes)
        //    {
        //        switch (child.Name.ToLower())
        //        {
        //            case "exposurevariable":
        //                cbxExposureField.Text = child.InnerText.Replace("&lt;", "<");
        //                break;
        //            case "outcomevariable":
        //                cbxOutcomeField.Text = child.InnerText.Replace("&lt;", "<");
        //                break;
        //            case "customheading":
        //                this.CustomOutputHeading = child.InnerText;
        //                break;
        //            case "customdescription":
        //                this.CustomOutputDescription = child.InnerText;
        //                break;
        //            case "customcaption":
        //                this.CustomOutputCaption = child.InnerText;
        //                break;
        //            case "smarttable":
        //                if (child.InnerText.ToLower().Equals("true"))
        //                {
        //                    checkboxSmartTable.IsChecked = true;
        //                }
        //                else
        //                {
        //                    checkboxSmartTable.IsChecked = false;
        //                }
        //                break;
        //        }
        //    }

        //    foreach (XmlAttribute attribute in element.Attributes)
        //    {
        //        switch (attribute.Name.ToLower())
        //        {
        //            case "top":
        //                Canvas.SetTop(this, double.Parse(attribute.Value));
        //                break;
        //            case "left":
        //                Canvas.SetLeft(this, double.Parse(attribute.Value));
        //                break;
        //        }
        //    }

        //    this.loadingCombos = false;

        //    RefreshResults();
        //    CollapseExpandConfigPanel();
        //}

        public void TurnOffPercents()
        {
            txtYesYesRow.Visibility = System.Windows.Visibility.Collapsed;
            txtYesYesCol.Visibility = System.Windows.Visibility.Collapsed;

            txtYesNoRow.Visibility = System.Windows.Visibility.Collapsed;
            txtYesNoCol.Visibility = System.Windows.Visibility.Collapsed;

            txtNoYesRow.Visibility = System.Windows.Visibility.Collapsed;
            txtNoYesCol.Visibility = System.Windows.Visibility.Collapsed;

            txtNoNoRow.Visibility = System.Windows.Visibility.Collapsed;
            txtNoNoCol.Visibility = System.Windows.Visibility.Collapsed;

            txtTotalYesRow.Visibility = System.Windows.Visibility.Collapsed;
            txtTotalYesCol.Visibility = System.Windows.Visibility.Collapsed;

            txtTotalNoRow.Visibility = System.Windows.Visibility.Collapsed;
            txtTotalNoCol.Visibility = System.Windows.Visibility.Collapsed;

            txtTotalNoRow.Visibility = System.Windows.Visibility.Collapsed;
            txtTotalNoCol.Visibility = System.Windows.Visibility.Collapsed;

            txtNoTotalRow.Visibility = System.Windows.Visibility.Collapsed;
            txtNoTotalCol.Visibility = System.Windows.Visibility.Collapsed;

            txtYesTotalRow.Visibility = System.Windows.Visibility.Collapsed;
            txtYesTotalCol.Visibility = System.Windows.Visibility.Collapsed;

            txtTotalTotalRow.Visibility = System.Windows.Visibility.Collapsed;
            txtTotalTotalCol.Visibility = System.Windows.Visibility.Collapsed;

            tblockYesRowPercent.Visibility = System.Windows.Visibility.Collapsed;
            tblockYesColPercent.Visibility = System.Windows.Visibility.Collapsed;

            tblockNoRowPercent.Visibility = System.Windows.Visibility.Collapsed;
            tblockNoColPercent.Visibility = System.Windows.Visibility.Collapsed;

            tblockTotalRowPercent.Visibility = System.Windows.Visibility.Collapsed;
            tblockTotalColPercent.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void TurnOnPercents()
        {
            txtYesYesRow.Visibility = System.Windows.Visibility.Visible;
            txtYesYesCol.Visibility = System.Windows.Visibility.Visible;

            txtYesNoRow.Visibility = System.Windows.Visibility.Visible;
            txtYesNoCol.Visibility = System.Windows.Visibility.Visible;

            txtNoYesRow.Visibility = System.Windows.Visibility.Visible;
            txtNoYesCol.Visibility = System.Windows.Visibility.Visible;

            txtNoNoRow.Visibility = System.Windows.Visibility.Visible;
            txtNoNoCol.Visibility = System.Windows.Visibility.Visible;

            txtTotalYesRow.Visibility = System.Windows.Visibility.Visible;
            txtTotalYesCol.Visibility = System.Windows.Visibility.Visible;

            txtTotalNoRow.Visibility = System.Windows.Visibility.Visible;
            txtTotalNoCol.Visibility = System.Windows.Visibility.Visible;

            txtTotalNoRow.Visibility = System.Windows.Visibility.Visible;
            txtTotalNoCol.Visibility = System.Windows.Visibility.Visible;

            txtNoTotalRow.Visibility = System.Windows.Visibility.Visible;
            txtNoTotalCol.Visibility = System.Windows.Visibility.Visible;

            txtYesTotalRow.Visibility = System.Windows.Visibility.Visible;
            txtYesTotalCol.Visibility = System.Windows.Visibility.Visible;

            txtTotalTotalRow.Visibility = System.Windows.Visibility.Visible;
            txtTotalTotalCol.Visibility = System.Windows.Visibility.Visible;

            tblockYesRowPercent.Visibility = System.Windows.Visibility.Visible;
            tblockYesColPercent.Visibility = System.Windows.Visibility.Visible;

            tblockNoRowPercent.Visibility = System.Windows.Visibility.Visible;
            tblockNoColPercent.Visibility = System.Windows.Visibility.Visible;

            tblockTotalRowPercent.Visibility = System.Windows.Visibility.Visible;
            tblockTotalColPercent.Visibility = System.Windows.Visibility.Visible;
        }

        private ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;
        private bool LoadingCanvas;

        public string CustomOutputHeading { get; set; }

        public string CustomOutputDescription { get; set; }

        public string CustomOutputCaption { get; set; }
        #endregion

        private object View
        {
            get
            {
                return new object();          //    this.dashboardHelper.View;
            }
        }

        //private IDbDriver Database
        //{
        //    get
        //    {
        //        return this.dashboardHelper.Database;
        //    }
        //}

        private int MaxRows
        {
            get
            {
                return 200;
                //int maxRows = 200;
                //bool success = int.TryParse(txtMaxRows.Text, out maxRows);
                //if (!success)
                //{
                //    return 200;
                //}
                //else
                //{
                //    return maxRows;
                //}
            }
        }

        private int MaxColumns
        {
            get
            {
                return 100;
                //int maxColumns = 50;
                //bool success = int.TryParse(txtMaxColumns.Text, out maxColumns);
                //if (!success)
                //{
                //    return 50;
                //}
                //else
                //{
                //    return maxColumns;
                //}
            }
        }

        private int MaxColumnLength
        {
            get
            {
                return 32;
                //int maxColumnLength = 24;
                //bool success = int.TryParse(txtMaxColumnLength.Text, out maxColumnLength);
                //if (!success)
                //{
                //    return 24;
                //}
                //else
                //{
                //    return maxColumnLength;
                //}
            }
        }

        public string ToHTML(bool ForDash = false, string htmlFileName = "", int count = 0)
        {
            ClientCommon.Common cmnClass = new ClientCommon.Common();
            StringBuilder htmlBuilder = new StringBuilder();
            try
            {
                //string fileName = string.Format("{0}{1}.html", System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N"));//GetHTMLLineListing();

                //System.IO.FileStream stream = System.IO.File.OpenWrite(fileName);
                //System.IO.StreamWriter sw = new System.IO.StreamWriter(stream);

                htmlBuilder.AppendLine("<html><head><title>2x2 Table</title>");
                htmlBuilder.AppendLine("<meta http-equiv=\"content-type\" content=\"text/html;charset=UTF-8\" />");
                htmlBuilder.AppendLine("<meta name=\"author\" content=\"Epi Info 7\" />");
                htmlBuilder.AppendLine(cmnClass.GenerateStandardHTMLStyle());
                //sb.AppendLine(this.ToHTML());
                //sw.Close();
                //sw.Dispose();
                //if (!string.IsNullOrEmpty(fileName))
                //{
                //    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                //    proc.StartInfo.FileName = fileName;
                //    proc.StartInfo.UseShellExecute = true;
                //    proc.Start();
                //}

                if (Is2x2)
                {
                    if (string.IsNullOrEmpty(CustomOutputHeading))
                    {
                        htmlBuilder.AppendLine("<h2 class=\"gadgetHeading\">2x2 Table</h2>");
                    }
                    else if (CustomOutputHeading != "(none)")
                    {
                        htmlBuilder.AppendLine("<h2 class=\"gadgetHeading\">" + CustomOutputHeading + "</h2>");
                    }

                    htmlBuilder.AppendLine("<p class=\"gadgetOptions\"><small>");
                    htmlBuilder.AppendLine("<em>Exposure variable:</em> <strong>" + ((EwavColumn)cbxExposureField.SelectedItem).Name + "</strong>");
                    htmlBuilder.AppendLine("<br />");

                    if (cbxOutcomeField.SelectedIndex >= 0)
                    {
                        htmlBuilder.AppendLine("<em>Crosstab variable:</em> <strong>" + ((EwavColumn)cbxOutcomeField.SelectedItem).Name + "</strong>");
                        htmlBuilder.AppendLine("<br />");
                    }

                    htmlBuilder.AppendLine("<em>Show percents:</em> <strong>" + checkboxShowPercents.IsChecked.ToString() + "</strong>");
                    htmlBuilder.AppendLine("<br />");

                    htmlBuilder.AppendLine("</small></p>");

                    if (!string.IsNullOrEmpty(txtStatus.Text) && pnlStatus.Visibility == Visibility.Visible)
                    {
                        htmlBuilder.AppendLine("<p><small><strong>" + txtStatus.Text + "</strong></small></p>");
                    }

                    if (!string.IsNullOrEmpty(CustomOutputDescription))
                    {
                        htmlBuilder.AppendLine("<p class=\"gadgetsummary\">" + CustomOutputDescription + "</p>");
                    }

                    if (dgResults.Visibility == System.Windows.Visibility.Visible)
                    {
                        htmlBuilder.AppendLine("<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">");
                        // Show Relative Risk Chart

                        htmlBuilder.AppendLine(" <tr>");
                        //for (int i = 0; i < ((DataView)dgResults.ItemsSource).Table.Columns.Count; i++)
                        //{
                        //    htmlBuilder.AppendLine("  <th>" + ((DataView)dgResults.ItemsSource).Table.Columns[i].ColumnName + "</th>");
                        //}
                        //htmlBuilder.AppendLine(" </tr>");

                        //foreach (DataRow row in ((DataView)dgResults.ItemsSource).Table.Rows)
                        //{
                        //    htmlBuilder.AppendLine(" <tr>");
                        //    for (int i = 0; i < ((DataView)dgResults.ItemsSource).Table.Columns.Count; i++)
                        //    {
                        //        htmlBuilder.AppendLine("  <td>" + row[i].ToString() + "</td>");
                        //    }

                        //    htmlBuilder.AppendLine(" </tr>");
                        //}
                        htmlBuilder.AppendLine("</table>");
                        htmlBuilder.AppendLine("<br style=\"clear: both\">");
                    }
                    else
                    {
                        htmlBuilder.AppendLine("<div style=\"height: 7px;\"></div>");
                        htmlBuilder.AppendLine("<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">");
                        //htmlBuilder.AppendLine("<caption>" + gridName + "</caption>");

                        htmlBuilder.AppendLine(" <tr>");
                        htmlBuilder.AppendLine("  <td></td>");
                        htmlBuilder.AppendLine("  <th colspan=\"2\">" + ((EwavColumn)cbxOutcomeField.SelectedItem).Name + "</th>");
                        htmlBuilder.AppendLine("  <td></td>");
                        htmlBuilder.AppendLine(" </tr>");

                        htmlBuilder.AppendLine(" <tr>");
                        htmlBuilder.AppendLine("  <th>" + txtExposure.Text + "</th>");
                        htmlBuilder.AppendLine("  <th>" + outcomeYes.Text + "</th>");
                        htmlBuilder.AppendLine("  <th>" + outcomeNo.Text + "</th>");
                        htmlBuilder.AppendLine("  <th>Total</th>");
                        htmlBuilder.AppendLine(" </tr>");

                        if (checkboxShowPercents.IsChecked == false)
                        {
                            htmlBuilder.AppendLine(" <tr>");
                            htmlBuilder.AppendLine("  <th>" + exposureYes.Text + "</th>");
                            htmlBuilder.AppendLine("  <td>" + currentSingleTableData.YyVal + "<br/><br/></td>");
                            htmlBuilder.AppendLine("  <td>" + currentSingleTableData.YnVal + "<br/><br/></td>");
                            htmlBuilder.AppendLine("  <td>" + currentSingleTableData.YtVal + "<br/><br/></td>");
                            htmlBuilder.AppendLine(" </tr>");

                            htmlBuilder.AppendLine(" <tr>");
                            htmlBuilder.AppendLine("  <th>" + exposureNo.Text + "</th>");
                            htmlBuilder.AppendLine("  <td>" + currentSingleTableData.NyVal + "<br/><br/></td>");
                            htmlBuilder.AppendLine("  <td>" + currentSingleTableData.NnVal + "<br/><br/></td>");
                            htmlBuilder.AppendLine("  <td>" + currentSingleTableData.NtVal + "<br/><br/></td>");
                            htmlBuilder.AppendLine(" </tr>");

                            htmlBuilder.AppendLine(" <tr>");
                            htmlBuilder.AppendLine("  <th>Total</th>");
                            htmlBuilder.AppendLine("  <td>" + currentSingleTableData.TyVal + "<br/><br/></td>");
                            htmlBuilder.AppendLine("  <td>" + currentSingleTableData.TnVal + "<br/><br/></td>");
                            htmlBuilder.AppendLine("  <td>" + currentSingleTableData.TtVal + "<br/><br/></td>");
                            htmlBuilder.AppendLine(" </tr>");
                        }
                        else
                        {
                            htmlBuilder.AppendLine(" <tr>");
                            htmlBuilder.AppendLine("  <th>" + exposureYes.Text + "<br/><small>Row %<br/>Col %</small></th>");
                            htmlBuilder.AppendLine("  <td>" + currentSingleTableData.YyVal + "<br/><small>" + currentSingleTableData.YyRowPct.ToString("P") + "<br/>" + currentSingleTableData.YyColPct.ToString("P") + "</small></td>");
                            htmlBuilder.AppendLine("  <td>" + currentSingleTableData.YnVal + "<br/><small>" + currentSingleTableData.YnRowPct.ToString("P") + "<br/>" + currentSingleTableData.YnColPct.ToString("P") + "</small></td>");
                            htmlBuilder.AppendLine("  <td>" + currentSingleTableData.YtVal + "<br/><small>" + 1.ToString("P") + "<br/>" + currentSingleTableData.YtColPct.ToString("P") + "</small></td>");
                            htmlBuilder.AppendLine(" </tr>");

                            htmlBuilder.AppendLine(" <tr>");
                            htmlBuilder.AppendLine("  <th>" + exposureNo.Text + "<br/><small>Row %<br/>Col %</small></th>");
                            htmlBuilder.AppendLine("  <td>" + currentSingleTableData.NyVal + "<br/><small>" + currentSingleTableData.NyRowPct.ToString("P") + "<br/>" + currentSingleTableData.NyColPct.ToString("P") + "</small></td>");
                            htmlBuilder.AppendLine("  <td>" + currentSingleTableData.NnVal + "<br/><small>" + currentSingleTableData.NnRowPct.ToString("P") + "<br/>" + currentSingleTableData.NnColPct.ToString("P") + "</small></td>");
                            htmlBuilder.AppendLine("  <td>" + currentSingleTableData.NtVal + "<br/><small>" + 1.ToString("P") + "<br/>" + currentSingleTableData.NtColPct.ToString("P") + "</small></td>");
                            htmlBuilder.AppendLine(" </tr>");

                            htmlBuilder.AppendLine(" <tr>");
                            htmlBuilder.AppendLine("  <th>Total<br/><small>Row %<br/>Col %</small></th>");
                            htmlBuilder.AppendLine("  <td>" + currentSingleTableData.TyVal + "<br/><small>" + currentSingleTableData.TyRowPct.ToString("P") + "<br/>" + 1.ToString("P") + "</small></td>");
                            htmlBuilder.AppendLine("  <td>" + currentSingleTableData.TnVal + "<br/><small>" + currentSingleTableData.TnRowPct.ToString("P") + "<br/>" + 1.ToString("P") + "</small></td>");
                            htmlBuilder.AppendLine("  <td>" + currentSingleTableData.TtVal + "<br/><small>" + 1.ToString("P") + "<br/>" + 1.ToString("P") + "</small></td>");
                            htmlBuilder.AppendLine(" </tr>");
                        }

                        htmlBuilder.AppendLine("</table>");

                        int redValue = (int)(((double)currentSingleTableData.YyVal / (double)currentSingleTableData.TtVal) * 200);
                        int orangeValue = (int)(((double)currentSingleTableData.YnVal / (double)currentSingleTableData.TtVal) * 200);
                        int yellowValue = (int)(((double)currentSingleTableData.NyVal / (double)currentSingleTableData.TtVal) * 200);
                        int greenValue = (int)(((double)currentSingleTableData.NnVal / (double)currentSingleTableData.TtVal) * 200);

                        htmlBuilder.AppendLine("<table class=\"twoByTwoColoredSquares\" align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">");
                        htmlBuilder.AppendLine(" <tr>");
                        htmlBuilder.AppendLine("  <td class=\"twobyTwoColoredSquareCell\" style=\"vertical-align: bottom; text-align: right;\">");
                        htmlBuilder.AppendLine("   <div style=\"float: right; vertical-align: bottom; background-color: red; height: " + redValue.ToString() + "px; width: " + redValue.ToString() + "px;\"></div>");
                        htmlBuilder.AppendLine("  </td>");
                        htmlBuilder.AppendLine("  <td class=\"twobyTwoColoredSquareCell\" style=\"vertical-align: bottom; text-align: left;\">");
                        htmlBuilder.AppendLine("   <div style=\"vertical-align: bottom; background-color: orange; height: " + orangeValue.ToString() + "px; width: " + orangeValue.ToString() + "px;\"></div>");
                        htmlBuilder.AppendLine("  </td>");
                        htmlBuilder.AppendLine(" </tr>");
                        htmlBuilder.AppendLine(" <tr>");
                        htmlBuilder.AppendLine("  <td class=\"twobyTwoColoredSquareCell\" style=\"vertical-align: top; text-align: right;\">");
                        htmlBuilder.AppendLine("   <div style=\"float: right; vertical-align: top; background-color: yellow; height: " + yellowValue.ToString() + "px; width: " + yellowValue.ToString() + "px;\"></div>");
                        htmlBuilder.AppendLine("  </td>");
                        htmlBuilder.AppendLine("  <td class=\"twobyTwoColoredSquareCell\" style=\"vertical-align: top; text-align: left;\">");
                        htmlBuilder.AppendLine("   <div style=\"vertical-align: top; background-color: green; height: " + greenValue.ToString() + "px; width: " + greenValue.ToString() + "px;\"></div>");
                        htmlBuilder.AppendLine("  </td>");
                        htmlBuilder.AppendLine(" </tr>");
                        htmlBuilder.AppendLine("</table>");

                        htmlBuilder.AppendLine("<br style=\"clear: all\">");


                        htmlBuilder.AppendLine("<br clear=\"all\" />");

                        string fisherExact = SharedStrings.UNDEFINED;
                        string fisherExact2 = SharedStrings.UNDEFINED;
                        string fisherLower = SharedStrings.UNDEFINED;
                        string fisherUpper = SharedStrings.UNDEFINED;

                        string oddsRatioEstimate = SharedStrings.UNDEFINED;
                        string oddsRatioLower = SharedStrings.UNDEFINED;
                        string oddsRatioUpper = SharedStrings.UNDEFINED;

                        string oddsRatioMLEEstimate = SharedStrings.UNDEFINED;
                        string oddsRatioMLEMidPLower = SharedStrings.UNDEFINED;
                        string oddsRatioMLEMidPUpper = SharedStrings.UNDEFINED;

                        string riskRatioLower = SharedStrings.UNDEFINED;
                        string riskRatioUpper = SharedStrings.UNDEFINED;

                        if (currentSingleTableData.SingleTableResults.FisherExactP != -1)
                        {
                            fisherExact = ((double)currentSingleTableData.SingleTableResults.FisherExactP).ToString("F10");
                        }

                        if (currentSingleTableData.SingleTableResults.FisherExact2P != -1)
                        {
                            fisherExact2 = ((double)currentSingleTableData.SingleTableResults.FisherExact2P).ToString("F10");
                        }

                        if (currentSingleTableData.SingleTableResults.OddsRatioMLEFisherLower != -1)
                        {
                            fisherLower = ((double)currentSingleTableData.SingleTableResults.OddsRatioMLEFisherLower).ToString("F4");
                        }

                        if (currentSingleTableData.SingleTableResults.OddsRatioMLEFisherUpper != -1)
                        {
                            fisherUpper = ((double)currentSingleTableData.SingleTableResults.OddsRatioMLEFisherUpper).ToString("F4");
                        }

                        if (currentSingleTableData.SingleTableResults.OddsRatioMLEEstimate != -1)
                        {
                            oddsRatioMLEEstimate = ((double)currentSingleTableData.SingleTableResults.OddsRatioMLEEstimate).ToString("F4");
                        }

                        if (currentSingleTableData.SingleTableResults.OddsRatioMLEMidPLower != -1)
                        {
                            oddsRatioMLEMidPLower = ((double)currentSingleTableData.SingleTableResults.OddsRatioMLEMidPLower).ToString("F4");
                        }

                        if (currentSingleTableData.SingleTableResults.OddsRatioMLEMidPUpper != -1)
                        {
                            oddsRatioMLEMidPUpper = ((double)currentSingleTableData.SingleTableResults.OddsRatioMLEMidPUpper).ToString("F4");
                        }

                        if (currentSingleTableData.SingleTableResults.OddsRatioEstimate != null)
                        {
                            oddsRatioEstimate = ((double)currentSingleTableData.SingleTableResults.OddsRatioEstimate).ToString("F4");
                        }

                        if (currentSingleTableData.SingleTableResults.OddsRatioLower != null)
                        {
                            oddsRatioLower = ((double)currentSingleTableData.SingleTableResults.OddsRatioLower).ToString("F4");
                        }

                        if (currentSingleTableData.SingleTableResults.OddsRatioUpper != null)
                        {
                            oddsRatioUpper = ((double)currentSingleTableData.SingleTableResults.OddsRatioUpper).ToString("F4");
                        }

                        if (currentSingleTableData.SingleTableResults.RiskRatioLower != null)
                        {
                            riskRatioLower = ((double)currentSingleTableData.SingleTableResults.RiskRatioLower).ToString("F4");
                        }

                        if (currentSingleTableData.SingleTableResults.RiskRatioUpper != null)
                        {
                            riskRatioUpper = ((double)currentSingleTableData.SingleTableResults.RiskRatioUpper).ToString("F4");
                        }


                        string chiSqCorP = currentSingleTableData.SingleTableResults.ChiSquareYates2P == null ? "" : ((double)currentSingleTableData.SingleTableResults.ChiSquareYates2P).ToString();
                        string chiSqCorVal = currentSingleTableData.SingleTableResults.ChiSquareYatesVal == null ? "" : ((double)currentSingleTableData.SingleTableResults.ChiSquareYatesVal).ToString("F4");
                        string chiSqManP = currentSingleTableData.SingleTableResults.ChiSquareMantel2P == null ? "" : ((double)currentSingleTableData.SingleTableResults.ChiSquareMantel2P).ToString();
                        string chiSqManVal = currentSingleTableData.SingleTableResults.ChiSquareMantelVal == null ? "" : ((double)currentSingleTableData.SingleTableResults.ChiSquareUncorrected2P).ToString("F4");
                        string chiSqUncP = currentSingleTableData.SingleTableResults.ChiSquareUncorrected2P == null ? "" : ((double)currentSingleTableData.SingleTableResults.ChiSquareUncorrected2P).ToString();
                        string chiSqUncVal = currentSingleTableData.SingleTableResults.ChiSquareUncorrectedVal == null ? "" : ((double)currentSingleTableData.SingleTableResults.ChiSquareUncorrectedVal).ToString("F4");
                        string riskDifferenceEstimate = currentSingleTableData.SingleTableResults.RiskDifferenceEstimate == null ? "" : ((double)currentSingleTableData.SingleTableResults.RiskDifferenceEstimate).ToString("F4");
                        string riskDifferenceLower = currentSingleTableData.SingleTableResults.RiskDifferenceLower == null ? "" : ((double)currentSingleTableData.SingleTableResults.RiskDifferenceLower).ToString("F4");
                        string riskDifferenceUpper = currentSingleTableData.SingleTableResults.RiskDifferenceUpper == null ? "" : ((double)currentSingleTableData.SingleTableResults.RiskDifferenceUpper).ToString("F4");
                        string riskRatioEstimate = currentSingleTableData.SingleTableResults.RiskRatioEstimate == null ? "" : ((double)currentSingleTableData.SingleTableResults.RiskRatioEstimate).ToString("F4");
                        string midPExact = currentSingleTableData.SingleTableResults.MidP == null ? "" : ((double)currentSingleTableData.SingleTableResults.MidP).ToString("F10");

                        htmlBuilder.AppendLine("<h4 align=\"Left\"> Single Table Analysis </h4>");

                        htmlBuilder.AppendLine("<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">");
                        htmlBuilder.AppendLine(" <tr>");
                        htmlBuilder.AppendLine("  <th></th>");
                        htmlBuilder.AppendLine("  <th align=\"center\">Point</th>");
                        htmlBuilder.AppendLine("  <th colspan=\"2\" align=\"center\">95% Confidence Interval</th>");
                        htmlBuilder.AppendLine(" </tr>");
                        htmlBuilder.AppendLine(" <tr>");
                        htmlBuilder.AppendLine("  <th></th>");
                        htmlBuilder.AppendLine("  <th align=\"center\">Estimate</th>");
                        htmlBuilder.AppendLine("  <th align=\"right\">Lower</th>");
                        htmlBuilder.AppendLine("  <th align=\"right\">Upper</th>");
                        htmlBuilder.AppendLine(" </tr>");
                        htmlBuilder.AppendLine(" <tr>");
                        htmlBuilder.AppendLine("  <td>PARAMETERS: Odds-based</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\"></td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\"></td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\"></td>");
                        htmlBuilder.AppendLine(" </tr>");
                        htmlBuilder.AppendLine(" <tr>");
                        htmlBuilder.AppendLine("  <td class=\"stats\">Odds Ratio (cross product)</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\" align=\"right\">" + oddsRatioEstimate + "</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\" align=\"right\">" + oddsRatioLower + "</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\" align=\"right\">" + oddsRatioUpper + "<tt> (T)</tt></td>");
                        htmlBuilder.AppendLine(" </tr>");
                        htmlBuilder.AppendLine(" <tr>");
                        htmlBuilder.AppendLine("  <td class=\"stats\">Odds Ratio (MLE)</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\" align=\"right\">" + oddsRatioMLEEstimate + "</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\" align=\"right\">" + oddsRatioMLEMidPLower + "</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\" align=\"right\">" + oddsRatioMLEMidPUpper + "<tt> (M)</tt></td>");
                        htmlBuilder.AppendLine(" </tr>");
                        htmlBuilder.AppendLine(" <tr>");
                        htmlBuilder.AppendLine("  <td class=\"stats\"></td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\"></td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\" align=\"right\">" + fisherLower + "</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\" align=\"right\">" + fisherUpper + "<tt> (F)</tt></td>");
                        htmlBuilder.AppendLine(" </tr>");
                        htmlBuilder.AppendLine(" <tr>");
                        htmlBuilder.AppendLine("  <td class=\"stats\">PARAMETERS: Risk-based</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\"></td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\"></td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\"></td>");
                        htmlBuilder.AppendLine(" </tr>");
                        htmlBuilder.AppendLine(" <tr>");
                        htmlBuilder.AppendLine("  <td class=\"stats\">Risk Ratio (RR)</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\" align=\"right\">" + riskRatioEstimate + "</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\" align=\"right\">" + riskRatioLower + "</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\" align=\"right\">" + riskRatioUpper + "<tt> (T)</tt></td>");
                        htmlBuilder.AppendLine(" </tr>");
                        htmlBuilder.AppendLine(" <tr>");
                        htmlBuilder.AppendLine("  <td class=\"stats\">Risk Difference (RD%)</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\" align=\"right\">" + riskDifferenceEstimate + "</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\" align=\"right\">" + riskDifferenceLower + "</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\" align=\"right\">" + riskDifferenceUpper + "<tt> (T)</tt></td>");
                        htmlBuilder.AppendLine(" </tr>");
                        htmlBuilder.AppendLine(" <tr />");
                        htmlBuilder.AppendLine(" <tr />");
                        htmlBuilder.AppendLine(" <tr />");
                        htmlBuilder.AppendLine(" <tr />");
                        htmlBuilder.AppendLine(" <tr />");
                        htmlBuilder.AppendLine(" <tr> ");
                        htmlBuilder.AppendLine("  <td class=\"stats\" colspan=\"4\"><p align=\"center\"><tt> (T=Taylor series; C=Cornfield; M=Mid-P; F=Fisher Exact)</tt></p>");
                        htmlBuilder.AppendLine(" </tr>");
                        htmlBuilder.AppendLine(" <tr />");
                        htmlBuilder.AppendLine(" <tr />");
                        htmlBuilder.AppendLine(" <tr />");
                        htmlBuilder.AppendLine(" <tr />");
                        htmlBuilder.AppendLine(" <tr />");
                        htmlBuilder.AppendLine(" <tr>");
                        htmlBuilder.AppendLine("  <th>STATISTICAL TESTS</th>");
                        htmlBuilder.AppendLine("  <th>Chi-square</th>");
                        htmlBuilder.AppendLine("  <th>1-tailed p</th>");
                        htmlBuilder.AppendLine("  <th>2-tailed p</th>");
                        htmlBuilder.AppendLine(" </tr>");
                        htmlBuilder.AppendLine(" <tr>");
                        htmlBuilder.AppendLine("  <td class=\"stats\">Chi-square - uncorrected</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\" align=\"right\">" + chiSqUncVal + "</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\"></td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\" align=\"right\">" + chiSqManP + "</td>");
                        htmlBuilder.AppendLine(" </tr>");
                        htmlBuilder.AppendLine(" <tr>");
                        htmlBuilder.AppendLine("  <td class=\"stats\">Chi-square - Mantel-Haenszel</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\" align=\"right\">" + chiSqManVal + "</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\"></td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\" align=\"right\">" + chiSqManP + "</td>");
                        htmlBuilder.AppendLine(" </tr>");
                        htmlBuilder.AppendLine(" <tr>");
                        htmlBuilder.AppendLine("  <td class=\"stats\">Chi-square - corrected (Yates)</td> ");
                        htmlBuilder.AppendLine("  <td class=\"stats\" align=\"right\">" + chiSqCorVal + "</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\"></td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\" align=\"right\">" + chiSqCorP + "</td>");
                        htmlBuilder.AppendLine(" </tr>");
                        htmlBuilder.AppendLine(" <tr>");
                        htmlBuilder.AppendLine("  <td class=\"stats\">Mid-p exact</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\"></td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\" align=\"right\">" + midPExact + "</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\"></td>");
                        htmlBuilder.AppendLine(" </tr>");
                        htmlBuilder.AppendLine(" <tr>");
                        htmlBuilder.AppendLine("  <td class=\"stats\">Fisher exact 1-tailed</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\"></td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\" align=\"right\">" + fisherExact + "</td>");
                        htmlBuilder.AppendLine("  <td class=\"stats\" align=\"right\">" + fisherExact2 + "</td>");
                        htmlBuilder.AppendLine(" </tr>");
                        htmlBuilder.AppendLine("</table>");
                    }
                    htmlBuilder.AppendLine("<br clear=\"all\" /><br clear=\"all\" />");
                }
                else
                {
                    if (string.IsNullOrEmpty(CustomOutputHeading))
                    {
                        htmlBuilder.AppendLine("<h2 class=\"gadgetHeading\">2x2 Table</h2>");
                    }
                    else if (CustomOutputHeading != "(none)")
                    {
                        htmlBuilder.AppendLine("<h2 class=\"gadgetHeading\">" + CustomOutputHeading + "</h2>");
                    }

                    htmlBuilder.AppendLine("<p class=\"gadgetOptions\"><small>");
                    htmlBuilder.AppendLine("<em>Exposure variable:</em> <strong>" + ((EwavColumn)cbxExposureField.SelectedItem).Name + "</strong>");
                    htmlBuilder.AppendLine("<br />");

                    if (cbxOutcomeField.SelectedIndex >= 0)
                    {
                        htmlBuilder.AppendLine("<em>Crosstab variable:</em> <strong>" + ((EwavColumn)cbxOutcomeField.SelectedItem).Name + "</strong>");
                        htmlBuilder.AppendLine("<br />");
                    }

                    htmlBuilder.AppendLine("</small></p>");

                    if (!string.IsNullOrEmpty(txtStatus.Text) && pnlStatus.Visibility == Visibility.Visible)
                    {
                        htmlBuilder.AppendLine("<p><small><strong>" + txtStatus.Text + "</strong></small></p>");
                    }

                    if (!string.IsNullOrEmpty(CustomOutputDescription))
                    {
                        htmlBuilder.AppendLine("<p class=\"gadgetsummary\">" + CustomOutputDescription + "</p>");
                    }

                    htmlBuilder.AppendLine("<div style=\"height: 7px;\"></div>");
                    htmlBuilder.AppendLine("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">");
                    htmlBuilder.AppendLine("<caption>" + ((EwavColumn)cbxExposureField.SelectedItem).Name + " * " + ((EwavColumn)cbxOutcomeField.SelectedItem).Name + "</caption>");

                    foreach (UIElement control in grdTable.Children)
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

                            if ((rowNumber == grdTable.RowDefinitions.Count - 1) || (columnNumber == grdTable.ColumnDefinitions.Count - 1))
                            {
                                formattedValue = "<span class=\"total\">" + value + "</span>";
                            }

                            htmlBuilder.AppendLine(tableDataTagOpen + formattedValue + tableDataTagClose);

                            if (columnNumber >= grdTable.ColumnDefinitions.Count - 1)
                            {
                                htmlBuilder.AppendLine("</tr>");
                            }
                        }
                    }
                }

                //return htmlBuilder.ToString();

            }
            finally
            {
            }

            System.Windows.Browser.HtmlPage.Window.Invoke("DisplayFormattedText", htmlBuilder.ToString());

            HtmlBuilder = htmlBuilder;     

            return "";
        }

        List<ColumnDataType> fieldPrimaryDataTypesList;
        public List<ColumnDataType> FieldPrimaryDataTypesList
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

        public void CreateFromXml(XElement element)
        {
            // The order of these three calls is very importaht  
            LoadingCanvas = true;
            //  FillComboboxes();
            LoadingDropDowns = true;

            ClientCommon.Common cmnClass = new ClientCommon.Common();


            this.GadgetFilters = new List<EwavDataFilterCondition>();

            List<string> extraElements = new List<string>();

            foreach (XElement child in element.Descendants())
            {
                switch (child.Name.ToString().ToLower())
                {
                    case "showPercents":
                        checkboxShowPercents.IsChecked = bool.Parse(child.Value.ToString());
                        break;
                    case "smarttable":
                        checkboxSmartTable.IsChecked = bool.Parse(child.Value.ToString());
                        break;
                    case "exposurevariable":
                        cbxExposureField.SelectedIndex = cmnClass.FindIndexToSelect(cmnClass.GetItemsSource(FieldPrimaryDataTypesList, false), child.Value.ToString().Replace("&lt;", "<"));
                        break;
                    case "outcomevariable":
                        cbxOutcomeField.SelectedIndex = cmnClass.FindIndexToSelect(cmnClass.GetItemsSource(FieldPrimaryDataTypesList, false), child.Value.ToString().Replace("&lt;", "<"));
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

                    default:
                        extraElements.Add(child.Name.ToString().ToLower());
                        break;
                }
            }

            if (extraElements.Count > 0)
            {
                //throw new Exception(" There was a problem parsing the XML for this 2x2 table.  The eleement " +
                //  extraElements.ToArray().ToString()  + "was not handled. ");
            }

            LoadingCanvas = false;
            LoadingDropDowns = false;

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

            cmnClass.AddControlToCanvas(this, mouseVerticalPosition, mouseHorizontalPosition, applicationViewModel.LayoutRoot);

            Construct();
            RefreshResults();
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


            RefreshResults();




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

                RefreshResults();
            }
        }


    }
}

namespace Ewav.Web.Services
{
    public partial class TwoByTwoDomainContext
    {
        //This is the place to set RIA Services query timeout. 
        //TimeSpan(0, 5, 0) means five minutes vs the 
        //default 60 sec
        partial void OnCreated()
        {
            if (!DesignerProperties.GetIsInDesignMode(Application.Current.RootVisual))
            {
                ((WebDomainClient<ITwoByTwoDomainServiceContract>)DomainClient).ChannelFactory.Endpoint.Binding.SendTimeout =
                    new TimeSpan(0, 120, 0);
            }
        }
    }
}