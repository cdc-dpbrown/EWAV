/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       FrequencyControl.xaml.cs
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

using System.Globalization;

using Ewav.ExtensionMethods;

namespace Ewav
{
    /// <summary>
    /// Interaction logic for FrequencyControl.xaml
    /// </summary>
    /// <remarks>
    /// This gadget is used to generate a basic frequency for most types of data in the database. It will return the count, percent of the total, 
    /// cumulative percent, and upper/lower 95% confidence intervals. It also displays a 'percent bar' as the final column in the output table
    /// that visually represents the percent of the total. It is more or less a mirror of the FREQ command in the 'Classic' Analysis module.
    /// </remarks>    
    /// 
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(UserControl))]
    [ExportMetadata("type", "gadget")]
    [ExportMetadata("tabindex", "1")]
    public partial class FrequencyControl : UserControl, IGadget, IEwavGadget, ICustomizableGadget
    {
        /// <summary>
        /// The list of grids for each strata value. E.g. if stratifying by SEX, there would be (likely)
        /// two grids in this list: One showing the summary statistics for Male and another for Female.
        /// </summary>
        private List<Grid> strataGridList;

        /// <summary>
        /// The list of labels for each strata value. E.g. if stratifying by SEX, there would be (likely)
        /// two values in this list: One for Male and one for Female. These show up as the table
        /// headings when displaying output.
        /// </summary>
        private List<TextBlock> gridLabelsList;

        /// <summary>
        ///  to satisfy interface    
        /// </summary>
        public ApplicationViewModel ApplicationViewModel
        {
            get
            {
                return this.applicationViewModel;
            }
        }

        int Index1 = 0, Index2 = 0, Index3 = 0;

        EwavColumn Col1, Col2, Col3;

        /// <summary>
        /// Common Class reference
        /// </summary>
        ClientCommon.Common cmnClass;

        private string myUIName = "Frequency";

        /// <summary>
        /// The value for the UI menus
        /// </summary>
        /// <value></value>
        public string MyUIName
        {
            set
            {
                myUIName = value;
            }
            get
            {
                return myUIName;
            }
        }

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

        /// <summary>
        /// The value for the frameworkelement.Name property
        /// </summary>
        /// <value></value>
        private string myControlName = "FrequencyControl";

        public string MyControlName
        {
            set
            {
                myControlName = value;
            }
            get
            {
                return this.myControlName;
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

        #region Delegates

        /// <summary>
        /// Container that holds gadget level filters.
        /// </summary>
        public List<EwavDataFilterCondition> GadgetFilters { get; set; }

        private delegate void RequestUpdateStatusDelegate(string statusMessage);
        private delegate bool CheckForCancellationDelegate();

        #endregion // Delegates
        #region Events
        public event GadgetClosingHandler GadgetClosing;
        public event GadgetProcessingFinishedHandler GadgetProcessingFinished;
        public event GadgetStatusUpdateHandler GadgetStatusUpdate;
        public event GadgetCheckForCancellationHandler GadgetCheckForCancellation;
        #endregion // Events

        #region Constructors

        public ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dashboardHelper">The dashboard helper object to attach</param>
        public FrequencyControl()
        {
            this.InitializeComponent();

            this.Loaded += new RoutedEventHandler(FrequencyControl_Loaded);
            //initializeControl();
            this.FillDropDowns();
        }

        void FrequencyControl_Loaded(object sender, RoutedEventArgs e)
        {
            #region WordCloud
            // ObservableCollection<WordCloudWord> cloudWords = new ObservableCollection<WordCloudWord>();
            // //for (int x  = 0; x  <   5      ; x ++)
            // //{
            // WordCloudWord ct = new WordCloudWord
            // {
            //     TagLink = "",
            //     TagName = "AAAA",
            //     TagOccurrences = 7
            // };
            // //     }
            // cloudWords.Add(ct);
            // ct = new WordCloudWord
            //{
            //    TagLink = "",
            //    TagName = "DDDD",
            //    TagOccurrences = 17
            //};
            // cloudWords.Add(ct);
            // ct = new WordCloudWord
            //{
            //    TagLink = "",
            //    TagName = "QQQQ",
            //    TagOccurrences = 4
            //};
            // cloudWords.Add(ct);
            // ct = new WordCloudWord
            //{
            //    TagLink = "",
            //    TagName = "FFFF",
            //    TagOccurrences = 2
            //};
            // cloudWords.Add(ct);
            // ct = new WordCloudWord
            // {
            //     TagLink = "",
            //     TagName = "DDDD",
            //     TagOccurrences = 17
            // };
            // cloudWords.Add(ct);
            // ct = new WordCloudWord
            // {
            //     TagLink = "",
            //     TagName = "QQQQ",
            //     TagOccurrences = 4
            // };
            // cloudWords.Add(ct);
            // ct = new WordCloudWord
            // {
            //     TagLink = "",
            //     TagName = "FFFF",
            //     TagOccurrences = 2
            // };
            // cloudWords.Add(ct);
            // ct = new WordCloudWord
            // {
            //     TagLink = "",
            //     TagName = "DDDD",
            //     TagOccurrences = 17
            // };
            // cloudWords.Add(ct);
            // ct = new WordCloudWord
            // {
            //     TagLink = "",
            //     TagName = "QQQQ",
            //     TagOccurrences = 4
            // };
            // cloudWords.Add(ct);
            // ct = new WordCloudWord
            // {
            //     TagLink = "",
            //     TagName = "FFFF",
            //     TagOccurrences = 2
            // };
            // cloudWords.Add(ct);
            // ct = new WordCloudWord
            //{
            //    TagLink = "",
            //    TagName = "XXXX",
            //    TagOccurrences = 7
            //};
            // cloudWords.Add(ct);
            // WordCloud wc = new WordCloud(this.wrapPanel, cloudWords);
            #endregion

            InitializeControl();
        }

        public void InitializeControl()
        {
            try
            {
                List<DTO.EwavFrequencyControlDto> frequencyDTO = new List<DTO.EwavFrequencyControlDto>();
                FrequencyViewModel frequencyViewModel = (FrequencyViewModel)this.DataContext;

                //   frequencyViewModel.ColumnsLoadedEvent +=
                //        new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(frequencyViewModel_ColumnsLoadedEvent);
                frequencyViewModel.FrequencyTableLoadedEvent +=
                    new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(frequencyViewModel_FrequencyTableLoadedEvent);
                frequencyViewModel.ErrorNotice += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(frequencyViewModel_ErrorNotice);
                applicationViewModel.ApplyDataFilterEvent += new Client.Application.ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);

                applicationViewModel.DefinedVariableAddedEvent += new Client.Application.DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);
                applicationViewModel.DefinedVariableInUseDeletedEvent += new Client.Application.DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableDeletedEvent);
                applicationViewModel.DefinedVariableNotInUseDeletedEvent += new Client.Application.DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
                applicationViewModel.PreColumnChangedEvent += new Client.Application.PreColumnChangedEventHandler(applicationViewModel_PreColumnChangedEvent);
                applicationViewModel.UnloadedEvent += new Client.Application.UnloadedEventHandler(applicationViewModel_UnloadedEvent);
                //  frequencyViewModel.GetColumns("NEDS", "vwExternalData");    
            }
            catch (Exception)
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
            applicationViewModel.DefinedVariableInUseDeletedEvent -= new Client.Application.DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableDeletedEvent);
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
            FillDropDowns();
            LoadingDropDowns = false;
            DoFrequency();
        }

        void applicationViewModel_DefinedVariableDeletedEvent(object o)
        {
            ResetGadget();
        }

        void applicationViewModel_DefinedVariableAddedEvent(object o)
        {
            SearchIndex();
            LoadingDropDowns = true;
            FillDropDowns();
            LoadingDropDowns = false;
            DoFrequency();
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
                ChildWindow window = new ErrorWindow(e.Data);
                window.Show();
                //return;
                errorMessage = e.Data.Message;
            }
            RenderFinishWithError(errorMessage);
            this.SetGadgetToFinishedState();
        }

        void applicationViewModel_ApplyDataFilterEvent(object o)
        {
            //if ((applicationViewModel.ItemToBeRemoved != null) &&
            //DFInUse != null &&
            //(applicationViewModel.ItemToBeRemoved.Name == DFInUse.Name))
            if (applicationViewModel.RemoveIndicator &&
                IsDFUsedInThisGadget())
            {
                ResetGadget();
            }
            else
            {
                DoFrequency();
            }
        }

        private void ResetGadget()
        {

            SearchIndex();
            if (IsDFUsedInThisGadget())
            {
                Index1 = Index2 = Index3 = -1;
                waitCursor.Visibility = System.Windows.Visibility.Collapsed;
                panelMain.Visibility = Visibility.Collapsed;
                pnlStatus.Visibility = System.Windows.Visibility.Collapsed;
            }
            LoadingDropDowns = true;
            FillDropDowns();
            LoadingDropDowns = false;
        }

        private bool IsDFUsedInThisGadget()
        {
            return Col1 != null && Col1.Name == applicationViewModel.ItemToBeRemoved.Name ||
                            Col2 != null && Col2.Name == applicationViewModel.ItemToBeRemoved.Name ||
                            Col3 != null && Col3.Name == applicationViewModel.ItemToBeRemoved.Name;
        }

        #endregion // Constructors
        List<ColumnDataType> fieldWeightDataType;
        public List<ColumnDataType> GetFieldWeightDataType
        {
            get
            {
                fieldWeightDataType = new List<ColumnDataType>();
                fieldWeightDataType.Add(ColumnDataType.Numeric);
                return fieldWeightDataType;
            }

        }

        List<ColumnDataType> fieldStrataDataType;
        public List<ColumnDataType> GetFieldStrataDataType
        {
            get
            {
                fieldStrataDataType = new List<ColumnDataType>();
                fieldStrataDataType.Add(ColumnDataType.Boolean);
                fieldStrataDataType.Add(ColumnDataType.Numeric);
                fieldStrataDataType.Add(ColumnDataType.Text);
                fieldStrataDataType.Add(ColumnDataType.UserDefined);
                return fieldStrataDataType;
            }
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
                fieldPrimaryDataTypesList.Add(ColumnDataType.DateTime);
                return fieldPrimaryDataTypesList;
            }
        }

        void FillDropDowns()    //    object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            //    FrequencyViewModel frequencyViewModel = (FrequencyViewModel)sender;
            List<EwavColumn> SourceColumns = this.applicationViewModel.EwavSelectedDatasource.AllColumns;

            //this.cbxField.SelectedIndex = 0;

            //List<ColumnDataType> columnDataType = new List<ColumnDataType>();
            //columnDataType.Add(ColumnDataType.Boolean);
            //columnDataType.Add(ColumnDataType.Numeric);
            //columnDataType.Add(ColumnDataType.UserDefined);

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
            //============================================

            //columnDataType.Clear();
            //fieldWeightColumnDataType.Add(ColumnDataType.Numeric);

            CBXFieldCols = from cols in SourceColumns
                           where GetFieldWeightDataType.Contains(cols.SqlDataTypeAsString)
                           orderby cols.Name
                           select cols;

            colsList = CBXFieldCols.ToList();

            colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            this.cbxFieldWeight.ItemsSource = colsList;
            this.cbxFieldWeight.SelectedValue = "Index";
            this.cbxFieldWeight.DisplayMemberPath = "Name";
            cbxFieldWeight.SelectedIndex = Index3;
            //=============================================

            //columnDataType.Clear();
            //columnDataType.Add(ColumnDataType.Boolean);
            //columnDataType.Add(ColumnDataType.Numeric);
            //columnDataType.Add(ColumnDataType.Text);
            //columnDataType.Add(ColumnDataType.UserDefined);
            CBXFieldCols = from cols in SourceColumns
                           where GetFieldStrataDataType.Contains(cols.SqlDataTypeAsString)
                           orderby cols.Name
                           select cols;

            colsList = CBXFieldCols.ToList();

            colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            this.cbxFieldStrata.ItemsSource = colsList;
            this.cbxFieldStrata.SelectedValue = "Index";
            this.cbxFieldStrata.DisplayMemberPath = "Name";
            cbxFieldStrata.SelectedIndex = Index2;
        }

        // #region Private Methods
        /// <summary>
        /// Copies a grid's output to the clipboard
        /// </summary>
        private void CopyToClipboard()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Grid grid in this.strataGridList)
            {
                string gridName = grid.Tag.ToString();

                if (this.strataGridList.Count > 1)
                {
                    sb.AppendLine(grid.Tag.ToString());
                }

                foreach (UIElement control in grid.Children)
                {
                    if (control is TextBlock)
                    {
                        int columnNumber = Grid.GetColumn((FrameworkElement)control);
                        string value = ((TextBlock)control).Text;

                        sb.AppendFormat("{0}\t", value);

                        if (columnNumber >= grid.ColumnDefinitions.Count - 2)
                        {
                            sb.AppendLine();
                        }
                    }
                }

                sb.AppendLine();
            }

            try
            {
                Clipboard.SetText(sb.ToString());
            }
            catch (System.Security.SecurityException)
            {
                // this exception is thrown when the user declines to give
                // permission for this web app to write to the clipboard
            }
            catch { }
        }

        /// <summary>
        /// Handles the filling of the gadget's combo boxesF
        /// </summary>
        //private void FillComboboxes(bool update = false)
        //{
        //    List<List<string>> ComboList = new List<List<string>>();
        //    this.loadingCombos = true;

        //    string prevField = string.Empty;
        //    string prevWeightField = string.Empty;
        //    string prevStrataField = string.Empty;

        //    if (update)
        //    {
        //        if (this.cbxField.SelectedIndex >= 0)
        //        {
        //            prevField = this.cbxField.SelectedItem.ToString();
        //        }
        //        if (this.cbxFieldWeight.SelectedIndex >= 0)
        //        {
        //            prevWeightField = this.cbxFieldWeight.SelectedItem.ToString();
        //        }
        //        if (this.cbxFieldStrata.SelectedIndex >= 0)
        //        {
        //            prevStrataField = this.cbxFieldStrata.SelectedItem.ToString();
        //        }
        //    }

        //    this.cbxField.ItemsSource = null;
        //    this.cbxField.Items.Clear();

        //    this.cbxFieldWeight.ItemsSource = null;
        //    this.cbxFieldWeight.Items.Clear();

        //    this.cbxFieldStrata.ItemsSource = null;
        //    this.cbxFieldStrata.Items.Clear();

        //    this.cbxField.ItemsSource = ComboList[0];
        //    this.cbxFieldWeight.ItemsSource = ComboList[1];
        //    this.cbxFieldStrata.ItemsSource = ComboList[2];

        //    if (this.cbxField.Items.Count > 0)
        //    {
        //        this.cbxField.SelectedIndex = -1;
        //    }
        //    if (this.cbxFieldWeight.Items.Count > 0)
        //    {
        //        this.cbxFieldWeight.SelectedIndex = -1;
        //    }
        //    if (this.cbxFieldStrata.Items.Count > 0)
        //    {
        //        this.cbxFieldStrata.SelectedIndex = -1;
        //    }

        //    if (update)
        //    {
        //        this.cbxField.SelectedItem = prevField;
        //        this.cbxFieldWeight.SelectedItem = prevWeightField;
        //        this.cbxFieldStrata.SelectedItem = prevStrataField;
        //    }

        //    this.loadingCombos = false;
        //}

        /// <summary>
        /// Used to add a new FREQ grid to the gadget's output
        /// </summary>
        /// <param name="strataVar">The name of the stratification variable selected, if any</param>
        /// <param name="value">The value by which this grid has been stratified by</param>
        private void AddFreqGrid(string strataVar, string value)
        {
            Grid grid = new Grid();
            grid.Tag = value;
            grid.HorizontalAlignment = HorizontalAlignment.Center;
            grid.Margin = new Thickness(0, 0, 0, 0);
            grid.Visibility = System.Windows.Visibility.Collapsed;

            ColumnDefinition column1 = new ColumnDefinition();
            ColumnDefinition column2 = new ColumnDefinition();
            ColumnDefinition column3 = new ColumnDefinition();
            ColumnDefinition column4 = new ColumnDefinition();
            ColumnDefinition column5 = new ColumnDefinition();
            ColumnDefinition column6 = new ColumnDefinition();
            ColumnDefinition column7 = new ColumnDefinition();

            column1.Width = GridLength.Auto;
            column2.Width = GridLength.Auto;
            column3.Width = GridLength.Auto;
            column4.Width = GridLength.Auto;
            column5.Width = GridLength.Auto;
            column6.Width = GridLength.Auto;
            column7.Width = new GridLength(100);

            grid.ColumnDefinitions.Add(column1);
            grid.ColumnDefinitions.Add(column2);
            grid.ColumnDefinitions.Add(column3);
            grid.ColumnDefinitions.Add(column4);
            grid.ColumnDefinitions.Add(column5);
            grid.ColumnDefinitions.Add(column6);
            grid.ColumnDefinitions.Add(column7);

            TextBlock txtGridLabel = new TextBlock();
            txtGridLabel.Text = value;
            txtGridLabel.HorizontalAlignment = HorizontalAlignment.Left;
            txtGridLabel.VerticalAlignment = VerticalAlignment.Bottom;
            txtGridLabel.Margin = new Thickness(2, 54, 2, 2);
            txtGridLabel.FontWeight = FontWeights.Bold;
            if (string.IsNullOrEmpty(strataVar))
            {
                txtGridLabel.Margin = new Thickness(2, 46, 2, 2);
                txtGridLabel.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                if (this.strataGridList.Count < 1)
                {
                    txtGridLabel.Margin = new Thickness(2, 0, 2, 2);
                }
                else
                {
                    txtGridLabel.Margin = new Thickness(2, 54, 2, 2);
                }
            }
            this.gridLabelsList.Add(txtGridLabel);
            this.panelMain.Children.Add(txtGridLabel);

            this.panelMain.Children.Add(grid);
            this.strataGridList.Add(grid);
        }

        /// <summary>
        /// Gets a Grid from the strataGridList based on a given stratification value
        /// </summary>
        /// <param name="strataValue">The strata value to use in searching the grid list</param>
        /// <returns>The System.Windows.Control.Grid associated with the given strata value</returns>
        private Grid GetStrataGrid(string strataValue)
        {
            Grid grid = new Grid();

            foreach (Grid g in this.strataGridList)
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

            grid = this.GetStrataGrid(strataValue);

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
        /// Sets the grid's percentage bar
        /// </summary>
        /// <param name="strataValue">
        /// The strata value to which this grid cell belongs; used to search the list of grids and 
        /// return the proper System.Windows.Controls.Grid for text insertion.
        /// </param>
        /// <param name="rowNumber">The row number of the grid to add the bar to</param>
        /// <param name="pct">The percentage width of the bar</param>
        private void SetGridBar(string strataValue, int rowNumber, double pct)
        {
            Grid grid = this.GetStrataGrid(strataValue);

            Rectangle rctBar = new Rectangle();
            rctBar.Width = 0.1;
            rctBar.Style = Application.Current.Resources["RectBar"] as Style;
            rctBar.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(rctBar, rowNumber);
            Grid.SetColumn(rctBar, /*4*/6);
            grid.Children.Add(rctBar);

            DoubleAnimation daBar = new DoubleAnimation();
            daBar.From = 1;
            daBar.To = pct * 100.0;
            daBar.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            Storyboard sb = new Storyboard();
            sb.Children.Add(daBar);
            Storyboard.SetTarget(daBar, rctBar);
            Storyboard.SetTargetProperty(daBar, new PropertyPath("Width"));
            if (!this.LayoutRoot.Resources.Contains("unique_id"))
            {
                this.LayoutRoot.Resources.Add("unique_id", sb);
            }

            sb.Begin();
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
            Grid grid = this.GetStrataGrid(strataValue);

            //this.waitCursor.Visibility = Visibility.Collapsed;
            grid.Visibility = Visibility.Visible;
            RowDefinition rowDef = new RowDefinition();
            rowDef.Height = new GridLength(height);
            grid.RowDefinitions.Add(rowDef);
        }

        /// <summary>
        /// Hides the cumulative percentage column
        /// </summary>
        private void HideCumulativePercent()
        {
            foreach (Grid grid in this.strataGridList)
            {
                grid.ColumnDefinitions[3].Width = new GridLength(0);
            }
        }

        /// <summary>
        /// Hides the 95% CI column
        /// </summary>
        private void HideConfidenceIntervals()
        {
            foreach (Grid grid in this.strataGridList)
            {
                grid.ColumnDefinitions[4].Width = new GridLength(0);
                grid.ColumnDefinitions[5].Width = new GridLength(0);
            }
        }

        /// <summary>
        /// Used to render the header (first row) of a given frequency output grid
        /// </summary>
        /// <param name="strataValue">
        /// The strata value to which this grid cell belongs; used to search the list of grids and 
        /// return the proper System.Windows.Controls.Grid for text insertion.
        /// </param>
        /// <param name="freqVar">The variable that the statistics were run on</param>        
        private void RenderFrequencyHeader(string strataValue, string freqVar)
        {
            Grid grid = this.GetStrataGrid(strataValue);

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
            txtValHeader.Text = freqVar;
            txtValHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtValHeader, 0);
            Grid.SetColumn(txtValHeader, 0);
            grid.Children.Add(txtValHeader);

            TextBlock txtFreqHeader = new TextBlock();
            txtFreqHeader.Text = SharedStrings.DASHBOARD_FREQUENCY_FREQUENCY;
            txtFreqHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtFreqHeader, 0);
            Grid.SetColumn(txtFreqHeader, 1);
            grid.Children.Add(txtFreqHeader);

            TextBlock txtPctHeader = new TextBlock();
            txtPctHeader.Text = SharedStrings.DASHBOARD_FREQUENCY_PERCENT;
            txtPctHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtPctHeader, 0);
            Grid.SetColumn(txtPctHeader, 2);
            grid.Children.Add(txtPctHeader);

            TextBlock txtAccuHeader = new TextBlock();
            txtAccuHeader.Text = SharedStrings.DASHBOARD_FREQUENCY_CUMULATIVE_PERCENT;
            txtAccuHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtAccuHeader, 0);
            Grid.SetColumn(txtAccuHeader, 3);
            grid.Children.Add(txtAccuHeader);

            TextBlock txtCILowHeader = new TextBlock();
            txtCILowHeader.Text = string.Format(" {0} ", SharedStrings.DASHBOARD_FREQUENCY_CI_LOWER);
            txtCILowHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtCILowHeader, 0);
            Grid.SetColumn(txtCILowHeader, 4);
            grid.Children.Add(txtCILowHeader);

            TextBlock txtCIUpperHeader = new TextBlock();
            txtCIUpperHeader.Text = string.Format(" {0} ", SharedStrings.DASHBOARD_FREQUENCY_CI_UPPER);
            txtCIUpperHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtCIUpperHeader, 0);
            Grid.SetColumn(txtCIUpperHeader, 5);
            grid.Children.Add(txtCIUpperHeader);
        }

        /// <summary>
        /// Used to render the footer (last row) of a given frequency output grid
        /// </summary>
        /// <param name="strataValue">
        /// The strata value to which this grid cell belongs; used to search the list of grids and 
        /// return the proper System.Windows.Controls.Grid for text insertion.
        /// </param>
        /// <param name="footerRowIndex">The row index of the footer</param>
        /// <param name="totalRows">The total number of rows in this grid</param>
        private void RenderFrequencyFooter(string strataValue, int footerRowIndex, int totalRows)
        {
            Grid grid = this.GetStrataGrid(strataValue);

            RowDefinition rowDefTotals = new RowDefinition();
            rowDefTotals.Height = new GridLength(30);
            grid.RowDefinitions.Add(rowDefTotals); //grdFreq.RowDefinitions.Add(rowDefTotals);

            TextBlock txtValTotals = new TextBlock();
            txtValTotals.Text = SharedStrings.TOTAL;
            txtValTotals.Margin = new Thickness(4, 0, 4, 0);
            txtValTotals.VerticalAlignment = VerticalAlignment.Center;
            txtValTotals.FontWeight = FontWeights.Bold;
            Grid.SetRow(txtValTotals, footerRowIndex);
            Grid.SetColumn(txtValTotals, 0);
            grid.Children.Add(txtValTotals); //grdFreq.Children.Add(txtValTotals);

            TextBlock txtFreqTotals = new TextBlock();
            txtFreqTotals.Text = totalRows.ToString();
            txtFreqTotals.Margin = new Thickness(4, 0, 4, 0);
            txtFreqTotals.VerticalAlignment = VerticalAlignment.Center;
            txtFreqTotals.HorizontalAlignment = HorizontalAlignment.Right;
            txtFreqTotals.FontWeight = FontWeights.Bold;
            Grid.SetRow(txtFreqTotals, footerRowIndex);
            Grid.SetColumn(txtFreqTotals, 1);
            grid.Children.Add(txtFreqTotals);

            TextBlock txtPctTotals = new TextBlock();
            txtPctTotals.Text = SharedStrings.DASHBOARD_100_PERCENT_LABEL;
            txtPctTotals.Margin = new Thickness(4, 0, 4, 0);
            txtPctTotals.VerticalAlignment = VerticalAlignment.Center;
            txtPctTotals.HorizontalAlignment = HorizontalAlignment.Right;
            txtPctTotals.FontWeight = FontWeights.Bold;
            Grid.SetRow(txtPctTotals, footerRowIndex);
            Grid.SetColumn(txtPctTotals, 2);
            grid.Children.Add(txtPctTotals);

            TextBlock txtAccuTotals = new TextBlock();
            txtAccuTotals.Text = SharedStrings.DASHBOARD_100_PERCENT_LABEL;
            txtAccuTotals.Margin = new Thickness(4, 0, 4, 0);
            txtAccuTotals.VerticalAlignment = VerticalAlignment.Center;
            txtAccuTotals.HorizontalAlignment = HorizontalAlignment.Right;
            txtAccuTotals.FontWeight = FontWeights.Bold;
            Grid.SetRow(txtAccuTotals, footerRowIndex);
            Grid.SetColumn(txtAccuTotals, 3);
            grid.Children.Add(txtAccuTotals);

            TextBlock txtCILowerTotals = new TextBlock();
            txtCILowerTotals.Text = "   ";
            txtCILowerTotals.Margin = new Thickness(4, 0, 4, 0);
            txtCILowerTotals.VerticalAlignment = VerticalAlignment.Center;
            txtCILowerTotals.HorizontalAlignment = HorizontalAlignment.Right;
            txtCILowerTotals.FontWeight = FontWeights.Bold;
            Grid.SetRow(txtCILowerTotals, footerRowIndex);
            Grid.SetColumn(txtCILowerTotals, 4);
            grid.Children.Add(txtCILowerTotals);

            TextBlock txtUpperTotals = new TextBlock();
            txtUpperTotals.Text = "   ";//"   ";StringLiterals.SPACE + StringLiterals.SPACE + StringLiterals.SPACE;
            txtUpperTotals.Margin = new Thickness(4, 0, 4, 0);
            txtUpperTotals.VerticalAlignment = VerticalAlignment.Center;
            txtUpperTotals.HorizontalAlignment = HorizontalAlignment.Right;
            txtUpperTotals.FontWeight = FontWeights.Bold;
            Grid.SetRow(txtUpperTotals, footerRowIndex);
            Grid.SetColumn(txtUpperTotals, 5);
            grid.Children.Add(txtUpperTotals);

            Rectangle rctTotalsBar = new Rectangle();
            rctTotalsBar.Width = 0.1;// 100;
            rctTotalsBar.Style = Application.Current.Resources["RectBar"] as Style;
            //rctTotalsBar.Fill = GetGradientBrush();

            rctTotalsBar.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(rctTotalsBar, footerRowIndex);
            Grid.SetColumn(rctTotalsBar, 6);
            grid.Children.Add(rctTotalsBar);

            DoubleAnimation daBar = new DoubleAnimation();
            daBar.From = 1;
            daBar.To = 100;
            daBar.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            //rctTotalsBar.BeginAnimation(Rectangle.WidthProperty, daBar);
            Storyboard sb = new Storyboard();
            sb.Children.Add(daBar);
            Storyboard.SetTarget(daBar, rctTotalsBar);
            Storyboard.SetTargetProperty(daBar, new PropertyPath("Width"));
            if (!this.LayoutRoot.Resources.Contains("unique_id"))
            {
                this.LayoutRoot.Resources.Add("unique_id", sb);
            }

            sb.Begin();
            //rctTotalsBar.beg
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
            this.waitCursor.Visibility = Visibility.Collapsed;
            if (this.strataGridList != null)
            {
                foreach (Grid freqGrid in this.strataGridList)
                {
                    freqGrid.Visibility = Visibility.Visible;
                }
            }
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            this.pnlStatus.Visibility = System.Windows.Visibility.Collapsed;
            this.txtStatus.Text = string.Empty;
            this.panelMain.Visibility = System.Windows.Visibility.Visible;
            this.CheckAndSetPosition();
            //Serialize();
        }

        /// <summary>
        /// Sets the gadget's state to 'finished with warning' mode
        /// </summary>
        /// <remarks>
        /// Common scenario for the usage of this method is when the distinct list of frequency values
        /// exceeds some built-in row limit. The output is limited to prevent the UI from locking up,
        /// and we want to let the user know that the output is limited while still showing them something.
        /// Thus we finish the rendering, but still show a message.
        /// </remarks>
        /// <param name="errorMessage">The warning message to display</param>
        private void RenderFinishWithWarning(string errorMessage)
        {
            this.waitCursor.Visibility = Visibility.Collapsed;

            foreach (Grid freqGrid in this.strataGridList)
            {
                freqGrid.Visibility = Visibility.Visible;
            }
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            this.pnlStatus.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 221)); //Light yellow;
            this.pnlStatusTop.Background = new SolidColorBrush(Color.FromArgb(255, 250, 194, 112));//Light Orange;

            this.pnlStatus.Visibility = System.Windows.Visibility.Visible;
            this.txtStatus.Text = errorMessage;
            this.CheckAndSetPosition();
        }

        /// <summary>
        /// Sets the gadget's state to 'finished with error' mode
        /// </summary>
        /// <param name="errorMessage">The error message to display</param>
        private void RenderFinishWithError(string errorMessage)
        {
            this.waitCursor.Visibility = Visibility.Collapsed;

            this.pnlStatus.Background = new SolidColorBrush(Color.FromArgb(255, 248, 215, 226)); //Brushes.Tomato;
            this.pnlStatusTop.Background = new SolidColorBrush(Color.FromArgb(255, 228, 101, 142)); //Brushes.Red;
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            this.pnlStatus.Visibility = System.Windows.Visibility.Visible;
            this.txtStatus.Text = errorMessage;
            this.panelMain.Visibility = System.Windows.Visibility.Collapsed;
            this.CheckAndSetPosition();
        }

        /// <summary>
        /// Used to push a status message to the gadget's status panel
        /// </summary>
        /// <param name="statusMessage">The status message to display</param>
        private void RequestUpdateStatusMessage(string statusMessage)
        {
            this.SetStatusMessage(statusMessage);
        }

        /// <summary>
        /// Used to sets the gadget's current status, e.g. "Processing results..." or "Displaying output..."
        /// </summary>
        /// <param name="statusMessage">The status message to display</param>
        private void SetStatusMessage(string statusMessage)
        {
            this.pnlStatus.Visibility = System.Windows.Visibility.Visible;
            this.txtStatus.Text = statusMessage;
        }

        /// <summary>
        /// Used to check if the user took an action that effectively cancels the currently-running worker
        /// thread. This could include: Closing the gadget, refreshing the dashboard, or selecting another
        /// means variable from the list of variables.
        /// </summary>
        /// <returns>Bool indicating whether or not the gadget's processing thread has been cancelled by the user</returns>
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

        /// <summary>
        /// Gets the gradient brush to use in drawing the output grid
        /// </summary>
        /// <returns>Brush</returns>
        private Brush GetGradientBrush()
        {
            LinearGradientBrush gradient = new LinearGradientBrush();
            gradient.StartPoint = new Point(0.5, 0);
            gradient.EndPoint = new Point(0.5, 1);

            GradientStop color1 = new GradientStop();
            color1.Color = SystemColors.ControlLightLightColor;
            color1.Color = SystemColors.ActiveCaptionColor;//GradientActiveCaptionColor;
            color1.Offset = 0;
            gradient.GradientStops.Add(color1);

            GradientStop color4 = new GradientStop();
            color4.Color = SystemColors.ControlLightColor;
            color4.Color = SystemColors.HighlightColor;
            color4.Offset = 0.1;
            gradient.GradientStops.Add(color4);

            GradientStop color2 = new GradientStop();
            color2.Color = SystemColors.ControlDarkDarkColor;
            color2.Color = SystemColors.WindowColor;
            color2.Offset = 0.5;
            gradient.GradientStops.Add(color2);

            GradientStop color3 = new GradientStop();
            color3.Color = SystemColors.ControlDarkColor;
            color3.Color = SystemColors.InactiveCaptionColor;//GradientInactiveCaptionColor;
            color3.Offset = 0.75;
            gradient.GradientStops.Add(color3);

            return gradient;
        }

        /// <summary>
        /// Used to generate the list of variables and options for the GadgetParameters object
        /// </summary> 
        private void CreateInputVariableList()
        {
            Dictionary<string, string> inputVariableList = new Dictionary<string, string>();
            this.gadgetOptions = new GadgetParameters();
            this.gadgetOptions.MainVariableName = string.Empty;
            this.gadgetOptions.WeightVariableName = string.Empty;
            this.gadgetOptions.StrataVariableNames = new List<string>();
            gadgetOptions.GadgetFilters = GadgetFilters;
            this.gadgetOptions.CrosstabVariableName = string.Empty;
            this.gadgetOptions.ColumnNames = new List<MyString>();
            this.gadgetOptions.StrataVariableList = new List<MyString>();

            this.gadgetOptions.TableName = this.applicationViewModel.EwavSelectedDatasource.TableName;
            inputVariableList.Add("tableName", this.gadgetOptions.TableName);

            this.gadgetOptions.DatasourceName = this.applicationViewModel.EwavSelectedDatasource.DatasourceName;

            if (this.cbxField.SelectedIndex > -1 && !string.IsNullOrWhiteSpace(((EwavColumn)(this.cbxField.SelectedValue)).Name))
            {
                inputVariableList.Add("freqvar", ((EwavColumn)(this.cbxField.SelectedValue)).Name);
                this.gadgetOptions.MainVariableName = ((EwavColumn)(this.cbxField.SelectedValue)).Name;
            }
            else
            {
                return;
            }

            if (this.cbxFieldWeight.SelectedIndex > -1 && !string.IsNullOrWhiteSpace(((EwavColumn)(this.cbxFieldWeight.SelectedValue)).Name))
            {
                inputVariableList.Add("weightvar", ((EwavColumn)(this.cbxFieldWeight.SelectedValue)).Name);
                this.gadgetOptions.WeightVariableName = ((EwavColumn)(this.cbxFieldWeight.SelectedValue)).Name;
            }
            if (this.cbxFieldStrata.SelectedIndex > -1 && !string.IsNullOrWhiteSpace(((EwavColumn)(this.cbxFieldStrata.SelectedValue)).Name))
            {
                string strataSelectedItem = ((EwavColumn)(this.cbxFieldStrata.SelectedValue)).Name;
                inputVariableList.Add("stratavar", ((EwavColumn)(this.cbxFieldStrata.SelectedValue)).Name);
                this.gadgetOptions.StrataVariableNames = new List<string>();
                //gadgetOptions.StrataVariableNames.ToList().Add(((EwavColumn)(cbxFieldStrata.SelectedValue)).Name);
                List<MyString> listMyString = new List<MyString>();
                MyString objMyString = new MyString();
                objMyString.VarName = strataSelectedItem;
                listMyString.Add(objMyString);
                this.gadgetOptions.StrataVariableList = listMyString;
            }

            inputVariableList.Add("allvalues", "false");
            this.gadgetOptions.ShouldUseAllPossibleValues = false;

            this.gadgetOptions.ShouldShowCommentLegalLabels = false;

            if (this.checkboxSortHighLow.IsChecked == true)
            {
                inputVariableList.Add("sort", "highlow");
                this.gadgetOptions.ShouldSortHighToLow = true;
            }
            else
            {
                this.gadgetOptions.ShouldSortHighToLow = false;
            }

            if (this.checkboxConfLimits.IsChecked == true)
            {
                inputVariableList.Add("showconflimits", "true");
            }
            else
            {
                inputVariableList.Add("showconflimits", "false");
            }

            if (this.checkboxCumPercent.IsChecked == true)
            {
                inputVariableList.Add("showcumulativepercent", "true");
            }
            else
            {
                inputVariableList.Add("showcumulativepercent", "false");
            }

            if (this.checkboxIncludeMissing.IsChecked == true)
            {
                inputVariableList.Add("includemissing", "true");
                this.gadgetOptions.ShouldIncludeMissing = true;
            }
            else
            {
                inputVariableList.Add("includemissing", "false");
                this.gadgetOptions.ShouldIncludeMissing = false;
            }

            inputVariableList.Add("maxrows", this.MaxRows.ToString());

            this.gadgetOptions.ShouldIncludeFullSummaryStatistics = false;
            this.gadgetOptions.InputVariableList = inputVariableList;
        }

        private void Construct()
        {
            this.strataGridList = new List<Grid>();
            this.gridLabelsList = new List<TextBlock>();
            this.cbxField.SelectionChanged += new SelectionChangedEventHandler(cbxField_SelectionChanged);
            this.cbxFieldWeight.SelectionChanged += new SelectionChangedEventHandler(cbxField_SelectionChanged);
            this.cbxFieldStrata.SelectionChanged += new SelectionChangedEventHandler(cbxField_SelectionChanged);

            this.checkboxConfLimits.Checked += new RoutedEventHandler(checkboxCheckChanged);
            this.checkboxConfLimits.Unchecked += new RoutedEventHandler(checkboxCheckChanged);

            this.checkboxCumPercent.Checked += new RoutedEventHandler(checkboxCheckChanged);
            this.checkboxCumPercent.Unchecked += new RoutedEventHandler(checkboxCheckChanged);

            this.checkboxIncludeMissing.Checked += new RoutedEventHandler(checkboxCheckChanged);
            this.checkboxIncludeMissing.Unchecked += new RoutedEventHandler(checkboxCheckChanged);

            this.checkboxSortHighLow.Checked += new RoutedEventHandler(checkboxCheckChanged);
            this.checkboxSortHighLow.Unchecked += new RoutedEventHandler(checkboxCheckChanged);

            //advTriangleCollapsed = true;

            this.IsProcessing = false;

            this.GadgetStatusUpdate += new GadgetStatusUpdateHandler(RequestUpdateStatusMessage);
            this.GadgetCheckForCancellation += new GadgetCheckForCancellationHandler(IsCancelled);

            this.gadgetOptions = new GadgetParameters();
            this.gadgetOptions.ShouldIncludeFullSummaryStatistics = false;
            this.gadgetOptions.ShouldIncludeMissing = false;
            this.gadgetOptions.ShouldSortHighToLow = false;
            this.gadgetOptions.ShouldUseAllPossibleValues = false;
            this.gadgetOptions.StrataVariableNames = new List<string>();
        }

        /// <summary>
        /// Checks to see whether a valid numeric digit was pressed for numeric-only conditions
        /// </summary>
        /// <param name="keyChar">The key that was pressed</param>
        /// <returns>Whether the input was a valid number character</returns>
        private bool ValidNumberChar(string keyChar)
        {
            System.Globalization.NumberFormatInfo numberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;

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
        /// Closes the gadget
        /// </summary>
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

        void CloseGadgetOnClick()
        {
            CloseGadget confirm = new CloseGadget(this);
            confirm.Show();
        }

        void CloseGadget()
        {
            applicationViewModel.CloseGadget(this);
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

            foreach (TextBlock textBlock in this.gridLabelsList)
            {
                this.panelMain.Children.Remove(textBlock);
            }

            this.strataGridList.Clear();

            this.grdFreq.Children.Clear();
            this.grdFreq.RowDefinitions.Clear();
        }

        void mnuSendDataToHTML_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// Fired when the user changes a field selection
        /// </summary>
        /// <param name="sender">Object that fired the event</param>
        /// <param name="e">.NET supplied event parameters</param>
        private void cbxField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DoFrequency();
        }

        private void DoFrequency()
        {
            if (!LoadingDropDowns && !LoadingCanvas)
            {
                IsUserDefindVariableInUse();
                if (this.cbxField.SelectedIndex < 1)
                {
                    return;
                }

                if (this.DataContext == null)
                {
                    return;
                }

                this.RefreshResults();

                ///----------------------

                this.CreateInputVariableList();
                FrequencyViewModel frequencyViewModel = (FrequencyViewModel)this.DataContext;

                this.gadgetOptions.DatasourceName = this.applicationViewModel.EwavSelectedDatasource.DatasourceName;
                //    frequencyViewModel.GetFrequencyData("NEDS", "vwExternalData", gadgetOptions);
                this.gadgetOptions.UseAdvancedDataFilter = applicationViewModel.UseAdvancedFilter;
                try
                {
                    frequencyViewModel.GetFrequencyData(this.gadgetOptions);
                }
                catch (GadgetException e)
                {
                    MessageBox.Show(e.Message);
                }

                gadgetExpander.IsExpanded = false;
            }

        }

        /// <summary>
        /// Determines whether is user defind variable in use].
        /// </summary>
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
            Col1 = (cbxField.SelectedIndex > -1) ? (EwavColumn)cbxField.SelectedItem : null;
            Col2 = (cbxFieldStrata.SelectedIndex > -1) ? (EwavColumn)cbxFieldStrata.SelectedItem : null;
            Col3 = (cbxFieldWeight.SelectedIndex > -1) ? (EwavColumn)cbxFieldWeight.SelectedItem : null;
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
            if (Col3 != null && Col3.IsUserDefined == true)
            {
                Col3.IsInUse = true;
                //DFInUse = Col3;
            }
        }

        void frequencyViewModel_FrequencyTableLoadedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            FrequencyViewModel frequencyViewModel = (FrequencyViewModel)sender;
            List<FrequencyResultData> notConvertedTable = frequencyViewModel.FrequencyTable;
            Dictionary<List<DTO.EwavFrequencyControlDto>, List<DescriptiveStatistics>> Data = new Dictionary<List<DTO.EwavFrequencyControlDto>, List<DescriptiveStatistics>>();
            this.RefreshResults();
            for (int i = 0; i < notConvertedTable.Count; i++)
            {
                Data.Add(notConvertedTable[i].FrequencyControlDtoList, notConvertedTable[i].DescriptiveStatisticsList);
            }

            this.Dowork(Data, gadgetOptions);
        }

        /// <summary>
        /// Handles the check / unchecked events
        /// </summary>
        /// <param name="sender">Object that fired the event</param>
        /// <param name="e">.NET supplied event parameters</param>
        private void checkboxCheckChanged(object sender, RoutedEventArgs e)
        {
            this.RefreshResults();

            ///----------------------

            this.CreateInputVariableList();
            FrequencyViewModel frequencyViewModel = (FrequencyViewModel)this.DataContext;
            //  frequencyViewModel.GetFrequencyData("NEDS", "vwExternalData", gadgetOptions);
            frequencyViewModel.GetFrequencyData(this.gadgetOptions);
        }

        #region Private Properties
        /// <summary>
        /// Gets the maximum number of rows this gadget will display
        /// </summary>
        private int MaxRows
        {
            get
            {
                int maxRows = 200;
                bool success = int.TryParse(this.txtMaxRows.Text, out maxRows);
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
        #endregion // Private Properties

        private void LayoutRoot_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetSafePosition(this.cmnClass.GetParentObject<Grid>(this, "LayoutRoot"));
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).GadgetNameOnRightClick = this.MyControlName; //"FrequencyControl";
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).StrataList = this.strataGridList;
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).SelectedGadget = this;
            cmnClass.UpdateZOrder(this, true, cmnClass.GetParentObject<Grid>(this, "LayoutRoot"));
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
                htmlBuilder.AppendLine("<h2 class=\"gadgetHeading\">Frequency</h2>");
            }
            else if (this.CustomOutputHeading != "(none)")
            {
                htmlBuilder.AppendLine(string.Format("<h2 class=\"gadgetHeading\">{0}</h2>", this.CustomOutputHeading));
            }

            htmlBuilder.AppendLine("<p class=\"gadgetOptions\"><small>");
            htmlBuilder.AppendLine(string.Format("<em>Frequency variable:</em> <strong>{0}</strong>", ((EwavColumn)this.cbxField.SelectedItem).Name));
            htmlBuilder.AppendLine("<br />");

            if (this.cbxFieldWeight.SelectedIndex >= 0)
            {
                htmlBuilder.AppendLine(string.Format("<em>Weight variable:</em> <strong>{0}</strong>", ((EwavColumn)this.cbxFieldWeight.SelectedItem).Name));
                htmlBuilder.AppendLine("<br />");
            }
            if (this.cbxFieldStrata.SelectedIndex >= 0)
            {
                htmlBuilder.AppendLine(string.Format("<em>Strata variable:</em> <strong>{0}</strong>", ((EwavColumn)this.cbxFieldStrata.SelectedItem).Name));
                htmlBuilder.AppendLine("<br />");
            }
            htmlBuilder.AppendLine(string.Format("<em>Include missing:</em> <strong>{0}</strong>", this.checkboxIncludeMissing.IsChecked.ToString()));
            htmlBuilder.AppendLine("<br />");
            htmlBuilder.AppendLine("</small></p>");

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
                if (this.cbxFieldWeight.SelectedIndex > -1 && !string.IsNullOrEmpty(((EwavColumn)this.cbxFieldWeight.SelectedValue).Name.ToString()))
                {
                    summaryText += string.Format("The field {0} has been specified as a weight. ", ((EwavColumn)this.cbxFieldWeight.SelectedItem).Name);
                }
                if (this.cbxFieldStrata.SelectedIndex > -1 && !string.IsNullOrEmpty(((EwavColumn)this.cbxFieldStrata.SelectedValue).Name.ToString()))
                {
                    summaryText += string.Format("The frequency data has been stratified. The data in this table is for the strata value {0}. ", grid.Tag.ToString());
                }
                summaryText += "Each non-heading row in the table represents one of the distinct frequency values. The last row contains the total.";

                htmlBuilder.AppendLine("<div style=\"height: 7px;\"></div>");
                htmlBuilder.AppendLine(string.Format("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" summary=\"{0}\">", summaryText));

                if (string.IsNullOrEmpty(this.CustomOutputCaption))
                {
                    if (this.cbxFieldStrata.SelectedIndex > -1 && !string.IsNullOrEmpty(((EwavColumn)this.cbxFieldStrata.SelectedValue).Name.ToString()))
                    {
                        htmlBuilder.AppendLine(string.Format("<caption>{0}</caption>", grid.Tag));
                    }
                }
                else
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
                        if (columnNumber == 3 && this.checkboxCumPercent.IsChecked == false)
                        {
                            continue;
                        }
                        if ((columnNumber == 4 || columnNumber == 5) && this.checkboxConfLimits.IsChecked == false)
                        {
                            continue;
                        }

                        string value = ((TextBlock)control).Text;

                        if (string.IsNullOrEmpty(value))
                        {
                            value = "&nbsp;";
                        }

                        string formattedValue = value;

                        if (rowNumber == grid.RowDefinitions.Count - 1)
                        {
                            formattedValue = string.Format("<span class=\"total\">{0}</span>", value);
                        }

                        htmlBuilder.AppendLine(string.Format("{0}{1}{2}", tableDataTagOpen, formattedValue, tableDataTagClose));

                        if (columnNumber == 2 && rowNumber > 0)
                        {
                            barWidth = 0;
                            double.TryParse(value.Trim().TrimEnd('%').Trim(), out barWidth);
                        }

                        if (columnNumber >= grid.ColumnDefinitions.Count - 2)
                        {
                            if (rowNumber > 0)
                            {
                                htmlBuilder.AppendLine(string.Format("<td class=\"value\"><div class=\"percentBar\" style=\"width: {0}px;\"></td>", ((int)barWidth * 2).ToString()));
                            }
                            else
                            {
                                htmlBuilder.AppendLine(string.Format("{0}{1}", tableDataTagOpen, tableDataTagClose));
                            }

                            htmlBuilder.AppendLine("</tr>");
                        }
                    }
                }

                htmlBuilder.AppendLine("</table>");
            }

            //return htmlBuilder.ToString();

            if (ForDash == false)
                System.Windows.Browser.HtmlPage.Window.Invoke("DisplayFormattedText", htmlBuilder.ToString());

            HtmlBuilder = htmlBuilder;

            return "";
        }

        void mnuClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.CloseGadgetOnClick();
        }

        void mnuCopy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.CopyToClipboard();
        }

        /// <summary>
        /// Handles the 'preview text input' method of the MaxRows text box
        /// </summary>
        /// <param name="sender">Object that fired the event</param>
        /// <param name="e">.NET supplied event parameters</param>
        private void txtMaxRows_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !this.ValidNumberChar(e.Text);
        }

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //gadgetContextMenu.Hide();
        }

        private void EditProperties_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Dowork(Dictionary<List<DTO.EwavFrequencyControlDto>, List<DescriptiveStatistics>> listDto, GadgetParameters gadgetParameters)
        {
            Dictionary<string, string> inputVariableList = gadgetParameters.InputVariableList;

            this.SetGadgetToProcessingState();
            this.ClearResults();

            string freqVar = string.Empty;
            string weightVar = string.Empty;
            string strataVar = string.Empty;
            bool showConfLimits = false;
            bool showCumulativePercent = false;
            bool includeMissing = false;
            int maxRows = 200;
            bool exceededMaxRows = false;

            if (inputVariableList == null)
            {
                this.SetGadgetToFinishedState();
                return;
            }

            if (inputVariableList.ContainsKey("freqvar"))
            {
                freqVar = inputVariableList["freqvar"];//FrequencyVariable
            }
            if (inputVariableList.ContainsKey("weightvar"))
            {
                weightVar = inputVariableList["weightvar"];
            }
            if (inputVariableList.ContainsKey("stratavar"))
            {
                strataVar = inputVariableList["stratavar"];
            }
            if (inputVariableList.ContainsKey("showconflimits"))
            {
                if (inputVariableList["showconflimits"].Equals("true"))
                {
                    showConfLimits = true;
                }
            }
            if (inputVariableList.ContainsKey("showcumulativepercent"))
            {
                if (inputVariableList["showcumulativepercent"].Equals("true"))
                {
                    showCumulativePercent = true;
                }
            }
            if (inputVariableList.ContainsKey("includemissing"))
            {
                if (inputVariableList["includemissing"].Equals("true"))
                {
                    includeMissing = true;
                }
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

            try
            {
                //string yesValue = "Yes";// config.Settings.RepresentationOfYes;
                //string noValue = "No";// config.Settings.RepresentationOfNo;

                RequestUpdateStatusDelegate requestUpdateStatus = new RequestUpdateStatusDelegate(RequestUpdateStatusMessage);
                CheckForCancellationDelegate checkForCancellation = new CheckForCancellationDelegate(IsCancelled);

                Dictionary<List<DTO.EwavFrequencyControlDto>, List<DescriptiveStatistics>> stratifiedFrequencyTables = listDto;
                if (stratifiedFrequencyTables == null || stratifiedFrequencyTables.Count == 0)
                {
                    this.RenderFinishWithError(SharedStrings.NO_RECORDS_SELECTED);
                    this.SetGadgetToFinishedState();
                    return;
                }
                else
                {
                    string formatString = string.Empty;

                    foreach (KeyValuePair<List<DTO.EwavFrequencyControlDto>, List<DescriptiveStatistics>> tableKvp in stratifiedFrequencyTables)
                    {
                        string strataValue = "";
                        try
                        {
                            strataValue = tableKvp.Key[0].NameOfDtoList;
                        }
                        catch (Exception)
                        {
                            this.RenderFinishWithError(SharedStrings.NO_RECORDS_SELECTED);
                            this.SetGadgetToFinishedState();
                            return;
                        }


                        double count = 0;
                        foreach (DescriptiveStatistics ds in tableKvp.Value)
                        {
                            count = count + ds.Observations;
                        }

                        if (count == 0 && stratifiedFrequencyTables.Count == 1)
                        {
                            // this is the only table and there are no records, so let the user know
                            this.RenderFinishWithError(SharedStrings.NO_RECORDS_SELECTED);
                            this.SetGadgetToFinishedState();
                            return;
                        }
                        else if (count == 0)
                        {
                            continue;
                        }
                        List<DTO.EwavFrequencyControlDto> frequencies = tableKvp.Key;

                        if (frequencies == null)
                        {
                            continue;
                        }

                        this.AddFreqGrid(strataVar, frequencies[0].NameOfDtoList);
                    }

                    if (this.GadgetStatusUpdate != null)
                    {
                        this.GadgetStatusUpdate(SharedStrings.DASHBOARD_GADGET_STATUS_DISPLAYING_OUTPUT);
                    }

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
                        List<DTO.EwavFrequencyControlDto> frequencies = tableKvp.Key;

                        if (frequencies.Count == 0)
                        {
                            continue;
                        }

                        string tableHeading = tableKvp.Key[0].NameOfDtoList;

                        if (stratifiedFrequencyTables.Count > 1)
                        {
                            tableHeading = freqVar;
                        }

                        if (!showCumulativePercent)
                        {
                            this.HideCumulativePercent();
                        }

                        if (!showConfLimits)
                        {
                            this.HideConfidenceIntervals();
                        }

                        string field = string.Empty;
                        string columnType = string.Empty;

                        this.RenderFrequencyHeader(strataValue, tableHeading);

                        double AccumulatedTotal = 0;
                        int rowCount = 1;
                        foreach (DTO.EwavFrequencyControlDto row in frequencies)
                        {
                            if (!row.FreqVariable.Equals(DBNull.Value) || (row.FreqVariable.Equals(DBNull.Value) && includeMissing == true))
                            {
                                this.AddGridRow(strataValue, 30);
                                string displayValue = row.FreqVariable.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                                //following logic converts 1 to Yes and 0 to NO. Commented the logic as we are considering 1 as an int and not as boolean sd per requirments                                //if (row.FreqVariable.ToString().Equals("1"))
                                //{
                                //    displayValue = yesValue;
                                //}
                                //else if (row.FreqVariable.ToString().Equals("0"))
                                //{
                                //    displayValue = noValue;
                                //}
                                //else
                                //{
                                //    displayValue = row.FreqVariable;
                                //}
                                //}
                                try
                                {
                                    if ((row.FreqVariable.Split('/').Length > 2 && row.FreqVariable.Split('/').Length < 4) // Valid Combos:- enumerator/enumerator/enumerator, enumerator-enumerator-enumerator ,
                                         || (row.FreqVariable.Split('-').Length > 2 && row.FreqVariable.Split('-').Length < 4)) //Invalid Combos:- enumerator/enumerator, enumerator/enumerator/enumerator/enumerator, enumerator-enumerator, enumerator-enumerator-enumerator-enumerator
                                    {
                                        displayValue = DateTime.Parse(row.FreqVariable).ToShortDateString();
                                    }
                                    else
                                    {
                                        displayValue = row.FreqVariable;
                                    }

                                }
                                catch (Exception)
                                {

                                    displayValue = row.FreqVariable.Replace("&lt;", "<").Replace("&gt;", ">");
                                }


                                if (string.IsNullOrEmpty(displayValue))
                                {
                                    displayValue = "Missing";//config.Settings.RepresentationOfMissing;
                                }

                                double pct = 0;
                                if (count > 0)
                                {
                                    pct = Convert.ToDouble(row.FrequencyColumn) / (count * 1.0);
                                }
                                AccumulatedTotal += pct;

                                this.SetGridText(strataValue, new TextBlockConfig(displayValue, new Thickness(4, 0, 4, 0), VerticalAlignment.Center, HorizontalAlignment.Left, rowCount, 0));
                                this.SetGridText(strataValue, new TextBlockConfig(row.FrequencyColumn.ToString(), new Thickness(4, 0, 4, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 1));
                                this.SetGridText(strataValue, new TextBlockConfig(pct.ToString("P"), new Thickness(4, 0, 4, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 2));

                                this.SetGridText(strataValue, new TextBlockConfig(AccumulatedTotal.ToString("P"), new Thickness(4, 0, 4, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 3));
                                this.SetGridText(strataValue, new TextBlockConfig(double.Parse(row.Perc95ClLowerColumn).ToString("P"), new Thickness(4, 0, 4, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 4));
                                this.SetGridText(strataValue, new TextBlockConfig(double.Parse(row.Perc95ClUpperColumn).ToString("P"), new Thickness(4, 0, 4, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 5));
                                this.SetGridBar(strataValue, rowCount, pct);

                                rowCount++;
                            }

                            if (rowCount > maxRows)
                            {
                                this.AddGridRow(strataValue, 30);
                                this.SetGridText(strataValue, new TextBlockConfig(" ... ", new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Left, rowCount, 0));
                                this.SetGridText(strataValue, new TextBlockConfig(" ... ", new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 1));
                                this.SetGridText(strataValue, new TextBlockConfig(" ... ", new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 2));
                                this.SetGridText(strataValue, new TextBlockConfig(" ... ", new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 3));
                                this.SetGridText(strataValue, new TextBlockConfig(" ... ", new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 4));
                                this.SetGridText(strataValue, new TextBlockConfig(" ... ", new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 5));
                                rowCount++;
                                exceededMaxRows = true;
                                break;
                            }
                        }

                        this.RenderFrequencyFooter(strataValue, rowCount, (int)count);
                        this.DrawFrequencyBorders(strataValue);
                    }

                    stratifiedFrequencyTables.Clear();
                }

                if (exceededMaxRows)
                {
                    string statusMessage = string.Format(SharedStrings.DASHBOARD_GADGET_STATUS_ROW_LIMIT, maxRows.ToString());
                    this.RenderFinishWithWarning(statusMessage);
                    this.SetGadgetToFinishedState();
                }
                else
                {
                    this.RenderFinish();
                    this.SetGadgetToFinishedState();
                }
            }
            catch (Exception ex)
            {
                this.RenderFinishWithError(ex.Message);
                this.SetGadgetToFinishedState();
            }
            finally
            {
            }
        }

        #region IGadget Members
        /// <summary>
        /// The object that is passed into the Dashboard Helper and contains the options the user selected.
        /// For example, this will contain the allColumns for the main variable, weight variable, strata variable(s),
        /// and any other parameters set in the gadget UI.
        /// </summary>
        private GadgetParameters gadgetOptions;

        private bool loadingCombos;

        public StringBuilder HtmlBuilder { get; set; }

        public void RefreshResults()
        {
            if (!this.loadingCombos && this.cbxField.SelectedIndex > -1 && this.gadgetOptions != null)
            {
                this.CreateInputVariableList();
                this.waitCursor.Visibility = Visibility.Visible;
            }
            else if (!this.loadingCombos && this.cbxField.SelectedIndex == -1)
            {
                this.ClearResults();
                this.waitCursor.Visibility = Visibility.Collapsed;
            }
        }

        public bool IsProcessing { get; set; }

        public void SetGadgetToProcessingState()
        {
            this.IsProcessing = true;
            this.cbxField.IsEnabled = false;
            this.cbxFieldStrata.IsEnabled = false;
            this.cbxFieldWeight.IsEnabled = false;
            this.checkboxIncludeMissing.IsEnabled = false;
            this.checkboxSortHighLow.IsEnabled = false;
            this.checkboxCumPercent.IsEnabled = false;
            this.checkboxConfLimits.IsEnabled = false;
            this.txtMaxRows.IsEnabled = false;
        }

        public void SetGadgetToFinishedState()
        {
            this.IsProcessing = false;
            this.cbxField.IsEnabled = true;
            this.cbxFieldStrata.IsEnabled = true;
            this.cbxFieldWeight.IsEnabled = true;
            this.checkboxIncludeMissing.IsEnabled = true;
            this.checkboxSortHighLow.IsEnabled = true;
            this.checkboxCumPercent.IsEnabled = true;
            this.checkboxConfLimits.IsEnabled = true;
            this.txtMaxRows.IsEnabled = true;

            if (this.GadgetProcessingFinished != null)
            {
                this.GadgetProcessingFinished(this);
            }

            waitCursor.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void UpdateVariableNames()
        {
            //this.FillComboboxes(true);
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseGadgetOnClick();
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
                this.txtMaxRows.Text = "1";
                return;
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ValidNumberChar(txtMaxRows.Text))
            {
                this.RefreshResults();
                this.CreateInputVariableList();
                FrequencyViewModel frequencyViewModel = (FrequencyViewModel)this.DataContext;
                //  frequencyViewModel.GetFrequencyData("NEDS", "vwExternalData", gadgetOptions);
                frequencyViewModel.GetFrequencyData(this.gadgetOptions);
            }
        }

        /// <summary>
        /// Saves the Values of Columns.
        /// </summary>
        private void SaveColumnValues()
        {
            Col1 = (EwavColumn)cbxField.SelectedItem;
            Col2 = (EwavColumn)cbxFieldStrata.SelectedItem;
            Col3 = (EwavColumn)cbxFieldWeight.SelectedItem;
        }

        /// <summary>
        /// Searches current index of the columns.
        /// </summary>
        private void SearchIndex()
        {
            //if (Col1 != null)
            //{
            //    Index1 = SearchCurrentIndex(Col1);
            //}
            //if (Col2 != null)
            //{
            //    Index2 = SearchCurrentIndex(Col2);
            //}
            //if (Col3 != null)
            //{
            //    Index3 = SearchCurrentIndex(Col3);
            //}

            ClientCommon.Common CommonClass = new ClientCommon.Common();

            Index1 = CommonClass.SearchCurrentIndex(Col1, CommonClass.GetItemsSource(GetFieldPrimaryDataType));

            Index2 = CommonClass.SearchCurrentIndex(Col2, CommonClass.GetItemsSource(GetFieldStrataDataType));

            Index3 = CommonClass.SearchCurrentIndex(Col3, CommonClass.GetItemsSource(GetFieldWeightDataType));

        }

        /// <summary>
        /// Method used to locate the current index for selected column.
        /// </summary>
        /// <param name="Column"></param>
        /// <returns></returns>
        //private int SearchCurrentIndex(EwavColumn Column)
        //{
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
        //    return 0;
        //}
        XElement element;

        /// <summary>
        /// Generates Xml representation of this gadget
        /// </summary>
        /// <param name="doc">The Xml docment</param>
        /// <returns>XmlNode</returns>
        XNode IGadget.Serialize(XDocument doc)
        {
            CreateInputVariableList();

            Dictionary<string, string> inputVariableList = this.gadgetOptions.InputVariableList;

            string freqVar = string.Empty;
            string strataVar = string.Empty;
            string weightVar = string.Empty;
            string sort = string.Empty;
            bool allValues = false;
            bool showConfLimits = true;
            bool showCumulativePercent = true;
            bool includeMissing = false;


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
            }



            int precision = 4;

            element = new XElement("gadget",
                new XAttribute("top", Canvas.GetTop(this).ToString("F0")),
                new XAttribute("left", Canvas.GetLeft(this).ToString("F0")),
                new XAttribute("collapsed", "false"),
                new XAttribute("gadgetType", "Ewav.FrequencyControl"),
            new XElement("mainVariable", freqVar),
            new XElement("strataVariable", strataVar),
            new XElement("weightVariable", weightVar),
            new XElement("sort", sort),
            new XElement("allValues", allValues),
            new XElement("precision", precision.ToString()),
            new XElement("includeMissing", includeMissing),
            new XElement("showConfLimits", checkboxConfLimits.IsChecked),
            new XElement("showCumulativePercent", checkboxCumPercent.IsChecked),
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
            //this.LoadingCombos = true;
            //HideConfigPanel();
            //infoPanel.Visibility = System.Windows.Visibility.Collapsed;
            //messagePanel.Visibility = System.Windows.Visibility.Collapsed;

            //EwavColumn col = new EwavColumn();// element.Element("mainvariable");
            //col.Name = "Black";//element.Element("mainVariable").Value;
            //cbxField.SelectedItem = col;
            waitCursor.Visibility = System.Windows.Visibility.Visible;
            LoadingCanvas = true;
            //this.FillDropDowns();

            //initializeControl();

            this.GadgetFilters = new List<EwavDataFilterCondition>();
            ClientCommon.Common cmnClass = new ClientCommon.Common();

            foreach (XElement child in element.Descendants())
            {
                switch (child.Name.ToString().ToLower())
                {
                    case "mainvariable":
                        //cbxField.Text = child.InnerText.Replace("&lt;", "<");
                        cbxField.SelectedIndex = cmnClass.FindIndexToSelect(cmnClass.GetItemsSource(GetFieldPrimaryDataType), child.Value.ToString().Replace("&lt;", "<"));
                        break;
                    //case "stratavariable":
                    //    lbxFieldStrata.SelectedItems.Add(child.InnerText.Replace("&lt;", "<"));
                    //    break;
                    case "stratavariable":
                        //foreach (XElement field in child.Descendants)
                        //{
                        //    List<string> fields = new List<string>();
                        //    if (field.Name.ToLower().Equals("stratavariable"))
                        //    {
                        //        lbxFieldStrata.SelectedItems.Add(field.InnerText.Replace("&lt;", "<"));
                        //    }
                        //}
                        cbxFieldStrata.SelectedIndex = cmnClass.FindIndexToSelect(cmnClass.GetItemsSource(GetFieldStrataDataType), child.Value.ToString().Replace("&lt;", "<"));
                        break;
                    case "weightvariable":
                        //cbxFieldWeight.Text = child.InnerText.Replace("&lt;", "<");
                        cbxFieldWeight.SelectedIndex = cmnClass.FindIndexToSelect(cmnClass.GetItemsSource(GetFieldWeightDataType), child.Value.ToString().Replace("&lt;", "<"));
                        break;

                    case "sort":
                        if (child.Value.ToString().ToLower().Equals("highlow"))
                        {
                            checkboxSortHighLow.IsChecked = true;
                        }
                        break;
                    case "includemissing":
                        if (child.Value.ToString().ToLower().Equals("true")) { checkboxIncludeMissing.IsChecked = true; }
                        else { checkboxIncludeMissing.IsChecked = false; }
                        break;
                    case "showconflimits":
                        if (child.Value.ToString().ToLower().Equals("true")) { checkboxConfLimits.IsChecked = true; }
                        else { checkboxConfLimits.IsChecked = false; }
                        break;
                    case "showcumulativepercent":
                        if (child.Value.ToString().ToLower().Equals("true")) { checkboxCumPercent.IsChecked = true; }
                        else { checkboxCumPercent.IsChecked = false; }
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
            //SetPositionFromXml(element);
            //this.LoadingCombos = false;
            //CheckVariables();
            //HideConfigPanel();
            //DragCanvas dgCanvas = 

            double mouseVerticalPosition = 0.0, mouseHorizontalPosition = 0.0;
            // mouseVerticalPosition = double.Parse(element.Attribute("top").Value.ToString());
            //mouseHorizontalPosition = double.Parse(element.Attribute("left").Value.ToString());

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

            DoFrequency();
            cmnClass.AddControlToCanvas(this, mouseVerticalPosition, mouseHorizontalPosition, applicationViewModel.LayoutRoot);
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            //CreateFromXml();
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

        //private void FilterButton_Click(object sender, RoutedEventArgs e)
        //{
        //    GadgetLevelFilter gadgetFiltersWindow = null;

        //    if (DataFilters != null)
        //    {
        //        gadgetFiltersWindow = new GadgetLevelFilter(DataFilters);
        //    }
        //    else
        //    {
        //        gadgetFiltersWindow = new GadgetLevelFilter();
        //    }

        //    gadgetFiltersWindow.Show();
        //    gadgetFiltersWindow.Closed += new EventHandler(window_Closed);
        //}

        public void window_Closed(object sender, EventArgs e)
        {
            GadgetFilterControl GadgetFilter = ((GadgetFilterControl)sender);
            if (GadgetFilter.DialogResult == true)
            {
                GadgetFilters = GadgetFilter.GadgetFilters;
            }

            DoFrequency();
        }

        public void Reload()
        {

            DoFrequency();




        }

        public void HeaderButton_Click(object sender, RoutedEventArgs e)
        {
            SetChartLabels();
        }

        public SetLabels setLabelsPopup { get; set; }

        public void SetChartLabels()
        {
            LoadViewModel();
            setLabelsPopup = new SetLabels(MyControlName, viewModel, true);

            setLabelsPopup.Unloaded -= new RoutedEventHandler(window_Unloaded);
            setLabelsPopup.Unloaded += new RoutedEventHandler(window_Unloaded);

            setLabelsPopup.Show();
        }

        public SetLabelsViewModel viewModel { get; set; }

        public void window_Unloaded(object sender, RoutedEventArgs e)
        {
            if (setLabelsPopup.DialogResult.Value)
            {
                SetHeaderAndFooter();
            }
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

        public void LoadViewModel()
        {
            viewModel = new SetLabelsViewModel();
            viewModel.GadgetTitle = GName.Text;
            viewModel.GadgetDescription = txtGadgetDescription.Text;
            viewModel.Footnote = "Footnote";
        }
    }






}

namespace Ewav.Web.Services
{
    public partial class FrequencyDomainContext
    {
        //This is the place to set RIA Services query timeout. 
        //TimeSpan(0, 5, 0) means five minutes vs the 
        //default 60 sec
        partial void OnCreated()
        {
            if (!DesignerProperties.GetIsInDesignMode(Application.
                Current.RootVisual))
                ((WebDomainClient<IFrequencyDomainServiceContract>)
                    DomainClient).ChannelFactory.Endpoint.Binding.SendTimeout =
                    new TimeSpan(0, 120, 0);
        }
    }
}