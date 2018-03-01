/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       LogisticRegression.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
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
using System.Text;
using Ewav.ViewModels;
using Ewav.BAL;
using Ewav.Web.EpiDashboard;
using CommonLibrary;
using Ewav.Web.Services;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ServiceModel.DomainServices.Client;
using System.Xml.Linq;
using Ewav.ExtensionMethods;

namespace Ewav
{
    /// <summary>
    /// Interaction logic for LogisticRegressionControl.xaml
    /// </summary>

    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(UserControl))]
    [ExportMetadata("type", "gadget")]
    [ExportMetadata("tabindex", "7")]
    public partial class LogisticRegression : UserControl, IGadget, IEwavGadget, ICustomizableGadget
    {
        #region Private Variables

        int Index1 = -1, Index2 = -1, Index3 = -1, Index4 = -1, Index5 = -1;

        EwavColumn Col1, Col2, Col3, Col4, Col5, DFInUse = null;
        private bool isProcessing;
        /// <summary>
        /// The Shared class that has represenation of Literal Used.
        /// </summary>
        StringLiterals stringLiterals = new StringLiterals();

        /// <summary>
        /// variable to includemissing
        /// </summary>
        bool includeMissing = true;
        /// <summary>
        /// variable to hold customerFilter
        /// </summary>
        string customFilter = string.Empty;

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
        /// Lists of string used to assign the itemsSource property of listBoxes
        /// </summary>
        List<EwavColumn> otherVars = new List<EwavColumn>();
        List<EwavColumn> dummVars = new List<EwavColumn>();
        List<EwavColumn> interterms = new List<EwavColumn>();
        List<EwavColumn> otherVars_bk = new List<EwavColumn>();
        List<EwavColumn> dummVars_bk = new List<EwavColumn>();
        List<EwavColumn> interterms_bk = new List<EwavColumn>();
        ClientCommon.Common cmnClass = new ClientCommon.Common();

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

        #endregion // Private Variables



        public event GadgetClosingHandler GadgetClosing;
        public event GadgetProcessingFinishedHandler GadgetProcessingFinished;

        /// <summary>
        /// Variable to hold the table values returned.
        /// </summary>
        public List<ListOfStringClass> regressTable;

        private bool errorOnPage;

        public bool ErrorOnPage
        {
            get { return errorOnPage; }
            set { errorOnPage = value; }
        }
        private string customOutputHeading;
        private string customOutputDescription;
        private string customOutputCaption;

        public List<EwavDataFilterCondition> GadgetFilters { get; set; }
        public string CustomOutputHeading
        {
            get
            {
                return this.customOutputHeading;
            }
            set
            {
                this.customOutputHeading = value;
            }
        }

        public string CustomOutputDescription
        {
            get
            {
                return this.customOutputDescription;
            }
            set
            {
                this.customOutputDescription = value;
            }
        }

        public string CustomOutputCaption
        {
            get
            {
                return this.customOutputCaption;
            }
            set
            {
                this.customOutputCaption = value;
            }
        }




        private string myControlName = "LogisticRegression";

        public string MyControlName
        {
            get { return myControlName; }
        }

        public string MyUIName
        {
            get
            {
                return "Logistic Regression";

            }
        }


        #region Constructors


        private ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;

        /// <summary>
        /// Default constructor
        /// </summary>
        public LogisticRegression()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(LogisticRegression_Loaded);
            //InitializeControl();
            FillDropDowns();
        }

        void LogisticRegression_Loaded(object sender, RoutedEventArgs e)
        {


            InitializeControl();

        }

        private void InitializeControl()
        {
            LogisticRegressionViewModel lrViewModel = (LogisticRegressionViewModel)this.DataContext;
            lrViewModel.RegressResultsLoadedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(lrViewModel_RegressResultsLoadedEvent);
            lrViewModel.ErrorNotice += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(lrViewModel_ErrorNotice);
            applicationViewModel.ApplyDataFilterEvent += new Client.Application.ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);
            applicationViewModel.DefinedVariableAddedEvent += new Client.Application.DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);
            applicationViewModel.DefinedVariableInUseDeletedEvent += new Client.Application.DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableInUseDeletedEvent);
            applicationViewModel.DefinedVariableNotInUseDeletedEvent += new Client.Application.DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
            applicationViewModel.PreColumnChangedEvent += new Client.Application.PreColumnChangedEventHandler(applicationViewModel_PreColumnChangedEvent);
            Construct();
            applicationViewModel.UnloadedEvent += new Client.Application.UnloadedEventHandler(applicationViewModel_UnloadedEvent);
            //    lrViewModel.GetColumns("NEDS", "vwExternalData");

            SelectDropDownValues();
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
            FillDropDowns();
            LoadingDropDowns = false;
            DoLogisticRegression();
        }

        void applicationViewModel_DefinedVariableInUseDeletedEvent(object o)
        {
            ResetGadget();
        }

        public List<ColumnDataType> GetMatchDataType
        {
            get
            {
                List<ColumnDataType> matchDataType = new List<ColumnDataType>();
                matchDataType.Add(ColumnDataType.Numeric);
                matchDataType.Add(ColumnDataType.Boolean);
                return matchDataType;
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

        public List<ColumnDataType> GetFieldDataType
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

        /// <summary>
        /// Resets the gadget to initial state.
        /// </summary>
        void ResetGadget()
        {
            SearchIndex();

            if (IsDFUsedInThisGadget())
            {
                Index1 = Index2 = Index3 = Index4 = Index5 = -1;
                spContent.Visibility = System.Windows.Visibility.Visible;
                ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
                pnlContent.Visibility = System.Windows.Visibility.Collapsed;
                pnlStatus.Visibility = System.Windows.Visibility.Collapsed;
                this.lbxDummyTerms.ItemsSource = null;
                this.lbxInteractionTerms.ItemsSource = null;
                this.lbxOtherFields.ItemsSource = null;
                dummVars.Clear();
                otherVars.Clear();
                interterms.Clear();
                dummVars_bk.Clear();
                otherVars_bk.Clear();
                interterms_bk.Clear();
            }
            LoadingDropDowns = true;
            FillDropDowns();
            LoadingDropDowns = false;
        }

        private bool IsDFUsedInThisGadget()
        {
            IsUserDefindVariableInUse();
            return Col1 != null && Col1.Name == applicationViewModel.ItemToBeRemoved.Name ||
                        Col2 != null && Col2.Name == applicationViewModel.ItemToBeRemoved.Name ||
                        Col3 != null && Col3.Name == applicationViewModel.ItemToBeRemoved.Name ||
                        Col4 != null && Col4.Name == applicationViewModel.ItemToBeRemoved.Name ||
                        Col5 != null && Col5.Name == applicationViewModel.ItemToBeRemoved.Name;
        }

        void applicationViewModel_DefinedVariableAddedEvent(object o)
        {
            SearchIndex();
            LoadingDropDowns = true;
            FillDropDowns();
            LoadingDropDowns = false;
            DoLogisticRegression();
        }

        //private void SaveState()
        //{
        //    Index1 = cbxFieldOutcome.SelectedIndex;
        //    Index2 = cbxFieldWeight.SelectedIndex;
        //    Index3 = 
        //    Index4 = cbxConf.SelectedIndex;
        //    Index5 = cbxFields.SelectedIndex;
        //}

        /// <summary>
        /// Saves the Values of Columns.
        /// </summary>
        private void SaveColumnValues()
        {
            Col1 = (EwavColumn)cbxFieldOutcome.SelectedItem;
            Col2 = (EwavColumn)cbxFieldWeight.SelectedItem;
            //Col3 = (EwavColumn)cbxConf.SelectedItem;
            Col3 = (EwavColumn)cbxFieldMatch.SelectedItem;
            Col5 = (EwavColumn)cbxFields.SelectedItem;

            this.otherVars_bk = otherVars;
            this.interterms_bk = interterms;
            this.dummVars_bk = dummVars;
        }

        /// <summary>
        /// Searches current index of the columns.
        /// </summary>
        private void SearchIndex()
        {
            //Index1 = SearchCurrentIndex(Col1);
            //Index2 = SearchCurrentIndex(Col2);
            //Index3 = SearchCurrentIndex(Col3);
            //Index4 = cbxConf.SelectedIndex;
            //Index5 = SearchCurrentIndex(Col5);

            ClientCommon.Common CommonClass = new ClientCommon.Common();

            Index1 = CommonClass.SearchCurrentIndex(Col1, CommonClass.GetItemsSource(GetFieldDataType));

            Index2 = CommonClass.SearchCurrentIndex(Col2, CommonClass.GetItemsSource(GetWeightDataType));

            Index3 = CommonClass.SearchCurrentIndex(Col3, CommonClass.GetItemsSource(GetMatchDataType));

            Index4 = cbxConf.SelectedIndex;

            Index5 = CommonClass.SearchCurrentIndex(Col5, CommonClass.GetItemsSource(GetFieldDataType));

            this.otherVars = otherVars_bk;
            this.interterms = interterms_bk;
            this.dummVars = dummVars_bk;
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

        void applicationViewModel_ApplyDataFilterEvent(object o)
        {
            bool dfColFound = false;
            if (applicationViewModel.RemoveIndicator &&
                DFInUse != null)
            {
                foreach (var item in DFInUse.Name.Split('*'))
                {
                    if (applicationViewModel.ItemToBeRemoved.Name == item)
                    {
                        dfColFound = true;
                        //return;
                    }
                }
            }


            if (applicationViewModel.RemoveIndicator &&
                (IsDFUsedInThisGadget() || dfColFound))
            {
                ResetGadget();
            }
            else
            {
                DoLogisticRegression();
            }

        }

        void lrViewModel_ErrorNotice(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            //if (e.Data.Message.Length > 0)
            //{
            //    ChildWindow window = new ErrorWindow(e.Data);
            //    window.Show();
            //    //return;
            //}
            RenderFinishWithError(e.Data.Message);
        }

        #endregion




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


        #region Public Methods

        /// <summary>
        /// Clears results
        /// </summary>
        public void ClearResults()
        {
            grdRegress.Children.Clear();
            grdRegress.RowDefinitions.Clear();

            grdParameters.Visibility = Visibility.Collapsed;

            waitCursor.Visibility = Visibility.Visible;
            txtConvergenceLabel.Text = string.Empty;
            txtIterationsLabel.Text = string.Empty;
            txtFinalLogLabel.Text = string.Empty;
            txtCasesIncludedLabel.Text = string.Empty;

            //btnRun.IsEnabled = false;
        }


        #endregion

        #region Event Handlers



        void cbxField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// Event raised with selected Index is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbxFields_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (cbxFields.SelectedIndex > -1)
            {
                if (!lbxOtherFields.Items.Contains(((EwavColumn)cbxFields.SelectedItem).Name.ToString()))
                {
                    if (!(((EwavColumn)cbxFields.SelectedItem).Name == null) && (((EwavColumn)cbxFields.SelectedItem).Name.ToString().Trim().Length > 0))
                    {
                        this.otherVars.Add((EwavColumn)cbxFields.SelectedItem);
                        this.CreateListBoxWithCheckBox(this.lbxOtherFields, otherVars);

                    }

                }
            }

            // RefreshResults();



        }

        /// <summary>
        /// Method used to see if UserDefined Variables are in use if it is sets the indicator.
        /// </summary>
        private void IsUserDefindVariableInUse()
        {
            Col1 = (cbxFieldOutcome.SelectedIndex > -1) ? (EwavColumn)cbxFieldOutcome.SelectedItem : null;
            Col2 = (cbxFieldWeight.SelectedIndex > -1) ? (EwavColumn)cbxFieldWeight.SelectedItem : null;
            Col3 = (cbxFieldMatch.SelectedIndex > -1) ? (EwavColumn)cbxFieldMatch.SelectedItem : null;
            Col5 = (cbxFields.SelectedIndex > -1) ? (EwavColumn)cbxFields.SelectedItem : null;
            DFInUse = null;
            if (Col1 != null && Col1.IsUserDefined == true)
            {
                Col1.IsInUse = true;
                DFInUse = Col1;
            }
            if (Col2 != null && Col2.IsUserDefined == true)
            {
                Col2.IsInUse = true;
                DFInUse = Col2;
            }
            if (Col3 != null && Col3.IsUserDefined == true)
            {
                Col3.IsInUse = true;
                DFInUse = Col3;
            }
            if (Col5 != null && Col5.IsUserDefined == true)
            {
                Col5.IsInUse = true;
                DFInUse = Col5;
            }

            IsUserDefinedVariableInUseByListBox(lbxDummyTerms);
            IsUserDefinedVariableInUseByListBox(lbxOtherFields);
            IsUserDefinedVariableInUseByListBox(lbxInteractionTerms);


        }

        private void IsUserDefinedVariableInUseByListBox(ListBox listBox)
        {
            List<ListBoxItem> Items = null;
            if (listBox.ItemsSource != null && ((List<ListBoxItem>)listBox.ItemsSource).Count != 0)
            {
                Items = (List<ListBoxItem>)listBox.ItemsSource;
                for (int i = 0; i < Items.Count; i++)
                {
                    Col4 = (EwavColumn)((CheckBox)((ListBoxItem)listBox.Items[i]).Content).Tag;
                    if (Col4 != null && Col4.IsUserDefined == true)
                    {
                        Col4.IsInUse = true;
                        DFInUse = Col4;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < listBox.Items.Count; i++)
                {
                    Col4 = (EwavColumn)((CheckBox)((ListBoxItem)listBox.Items[i]).Content).Tag;
                    if (Col4 != null && Col4.IsUserDefined == true)
                    {
                        Col4.IsInUse = true;
                        DFInUse = Col4;
                        break;
                    }
                }
            }


        }

        /// <summary>
        /// Does the Main Logistics Regression
        /// </summary>
        private void DoLogisticRegression()
        {
            if (!LoadingDropDowns)
            {
                if (ValidateLists())
                {
                    IsUserDefindVariableInUse();
                    //RefreshResults();
                    LogisticRegressionViewModel logisticRegressionViewModel = (LogisticRegressionViewModel)this.DataContext;
                    Dictionary<string, string> inputVariableList = CreateInputVariableList();
                    if (!ErrorOnPage)
                    {
                        List<string> columnNames = new List<string>();
                        customFilter = null;
                        foreach (KeyValuePair<string, string> kvp in inputVariableList)
                        {
                            if (kvp.Key.ToLower().Equals("includemissing"))
                            {
                                includeMissing = bool.Parse(kvp.Value);
                            }
                        }

                        foreach (KeyValuePair<string, string> kvp in inputVariableList)
                        {
                            if (kvp.Value.ToLower().Equals("unsorted") || kvp.Value.ToLower().Equals("dependvar") || kvp.Value.ToLower().Equals("weightvar") || kvp.Value.ToLower().Equals("matchvar"))
                            {
                                columnNames.Add(kvp.Key);
                                if (!kvp.Value.ToLower().Equals("dependvar"))
                                {
                                    customFilter = customFilter + stringLiterals.PARANTHESES_OPEN + stringLiterals.LEFT_SQUARE_BRACKET + kvp.Key + stringLiterals.RIGHT_SQUARE_BRACKET + stringLiterals.SPACE + "is not null" + stringLiterals.PARANTHESES_CLOSE + " AND ";
                                }
                            }
                            else if (kvp.Value.ToLower().Equals("discrete"))
                            {
                                columnNames.Add(kvp.Key);
                                customFilter = customFilter + stringLiterals.PARANTHESES_OPEN + stringLiterals.LEFT_SQUARE_BRACKET + kvp.Key + stringLiterals.RIGHT_SQUARE_BRACKET + stringLiterals.SPACE + "is not null" + stringLiterals.PARANTHESES_CLOSE + " AND ";
                            }
                        }

                        if (includeMissing)
                        {
                            customFilter = string.Empty;
                        }
                        else
                        {
                            if (customFilter != null && customFilter.Length > 4)
                            {
                                customFilter = customFilter.Remove(customFilter.Length - 4, 4);
                            }

                        }

                        List<DictionaryDTO> inputDtoList;

                        inputDtoList = ConvertDictToList(CreateInputVariableList());

                        GadgetParameters gadgetOptions = new GadgetParameters();

                        gadgetOptions.TableName = applicationViewModel.EwavSelectedDatasource.TableName;

                        gadgetOptions.DatasourceName = applicationViewModel.EwavSelectedDatasource.DatasourceName;

                        gadgetOptions.UseAdvancedDataFilter = applicationViewModel.UseAdvancedFilter;

                        gadgetOptions.GadgetFilters = GadgetFilters;
                        logisticRegressionViewModel.GetRegressionResults(gadgetOptions, columnNames, inputDtoList, customFilter);

                        this.gadgetExpander.IsExpanded = false;
                        waitCursor.Visibility = System.Windows.Visibility.Visible;
                        ResetOtherVariableList();
                    }
                }
            }

        }

        /// <summary>
        /// Event is raised with run button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            DoLogisticRegression();
        }
        /// <summary>
        /// Verifies if Lists are not blank
        /// </summary>
        /// <returns></returns>
        private bool ValidateLists()
        {
            if ((this.otherVars.Count > 0 || this.dummVars.Count > 0 || this.interterms.Count > 0) &&
                this.cbxFieldOutcome.SelectedIndex > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Event is raised with OtherFields combo Boxes changes the index
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbxOtherFields_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbxOtherFields.SelectedItems.Count == 1 || lbxOtherFields.SelectedItems.Count == 2)
            {
                btnMakeDummy.IsEnabled = true;
            }
            else
            {
                btnMakeDummy.IsEnabled = false;
            }

            if (lbxOtherFields.SelectedItems.Count == 1 || lbxOtherFields.SelectedItems.Count > 2)
            {
                btnMakeDummy.Content = "Make Dummy";
            }
            else if (lbxOtherFields.SelectedItems.Count == 2)
            {
                btnMakeDummy.Content = "Make Interaction";
            }
        }

        /// <summary>
        /// Main Method that takes the server results and displays on client.
        /// </summary>
        /// <param name="regResults"></param>
        /// <param name="inputVariableList"></param>
        private void worker_DoWork(LogRegressionResults regResults, Dictionary<string, string> inputVariableList = null)
        {
            Dictionary<string, string> setProperties = new Dictionary<string, string>();

            setProperties.Add("BLabels", "Yes" + ";" + "No" + ";" + "Missing");
            SetGadgetToProcessingState();
            ClearResults();
            string customFilter = string.Empty;
            List<string> columnNames = new List<string>();
            LogRegressionResults results = new LogRegressionResults();

            if (regResults.ErrorMessage != "")
            {
                RenderFinishWithError(regResults.ErrorMessage);
                return;
            }

            //results.CasesIncluded = regResults.RegressionResults1.CasesIncluded;
            //results.Convergence = regResults.RegressionResults1.Convergence;
            //results.FinalLikelihood = regResults.RegressionResults1.FinalLikelihood;
            //results.Iterations = regResults.RegressionResults1.Iterations;
            //results.LRDF = regResults.RegressionResults1.LRDF;
            //results.LRP = regResults.RegressionResults1.LRP;
            //results.LRStatistic = regResults.RegressionResults1.LRStatistic;
            //results.ScoreDF = regResults.RegressionResults1.ScoreDF;
            //results.ScoreP = regResults.RegressionResults1.ScoreP;
            //results.ScoreStatistic = regResults.RegressionResults1.ScoreStatistic;
            //if (regResults.RegressionResults1.ErrorMessage != null)
            //{
            //    results.ErrorMessage = regResults.RegressionResults1.ErrorMessage.Replace("<tlt>", string.Empty).Replace("</tlt>", string.Empty);
            //}

            results.CasesIncluded = regResults.CasesIncluded;
            results.Convergence = regResults.Convergence;
            results.FinalLikelihood = regResults.FinalLikelihood;
            results.Iterations = regResults.Iterations;
            results.LRDF = regResults.LRDF;
            results.LRP = regResults.LRP;
            results.LRStatistic = regResults.LRStatistic;
            results.ScoreDF = regResults.ScoreDF;
            results.ScoreP = regResults.ScoreP;
            results.ScoreStatistic = regResults.ScoreStatistic;
            if (regResults.ErrorMessage != null)
            {
                results.ErrorMessage = regResults.ErrorMessage.Replace("<tlt>", string.Empty).Replace("</tlt>", string.Empty);
            }

            results.Variables = new List<VariableRow>();

            if (results.Variables.Count != null)
            {
                foreach (VariableRow vrow in regResults.Variables)
                {
                    VariableRow nrow = new VariableRow();
                    nrow.Coefficient = vrow.Coefficient;
                    nrow.Ci = vrow.Ci;
                    nrow.P = vrow.P;
                    nrow.NinetyFivePercent = vrow.NinetyFivePercent;
                    nrow.OddsRatio = vrow.OddsRatio;
                    nrow.Se = vrow.Se;
                    nrow.VariableName = vrow.VariableName;
                    nrow.Z = vrow.Z;
                    results.Variables.Add(nrow);
                }
                if (results.Variables.Count > 0)
                {
                    RenderRegressionHeader();
                }
                int rowCount = 1;
                foreach (VariableRow row in results.Variables)
                {
                    AddGridRow(grdRegress, 30);

                    string displayValue = row.VariableName;

                    SetGridText(grdRegress, new TextBlockConfig(stringLiterals.SPACE + displayValue + stringLiterals.SPACE, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Left, rowCount, 0));
                    if (row.OddsRatio <= -9999)
                    {
                        SetGridText(grdRegress, new TextBlockConfig("*", new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 1));

                        SetGridText(grdRegress, new TextBlockConfig("*", new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 2));


                        SetGridText(grdRegress, new TextBlockConfig("*", new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 3));
                    }
                    else
                    {
                        SetGridText(grdRegress, new TextBlockConfig(stringLiterals.SPACE + row.OddsRatio.ToString("F4") + stringLiterals.SPACE, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 1));

                        SetGridText(grdRegress, new TextBlockConfig(stringLiterals.SPACE + row.NinetyFivePercent.ToString("F4") + stringLiterals.SPACE, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 2));
                        if (row.Ci > 1.0E12)
                        {

                            SetGridText(grdRegress, new TextBlockConfig(">1.0E12", new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 3));
                        }
                        else
                        {
                            SetGridText(grdRegress, new TextBlockConfig(stringLiterals.SPACE + row.Ci.ToString("F4") + stringLiterals.SPACE, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 3));
                        }
                    }
                    SetGridText(grdRegress, new TextBlockConfig(stringLiterals.SPACE + row.Coefficient.ToString("F4") + stringLiterals.SPACE, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 4));
                    SetGridText(grdRegress, new TextBlockConfig(stringLiterals.SPACE + row.Se.ToString("F4") + stringLiterals.SPACE, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 5));
                    SetGridText(grdRegress, new TextBlockConfig(stringLiterals.SPACE + row.Z.ToString("F4") + stringLiterals.SPACE, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 6));
                    SetGridText(grdRegress, new TextBlockConfig(stringLiterals.SPACE + row.P.ToString("F4") + stringLiterals.SPACE, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 7));

                    rowCount++;
                }

                DrawRegressionBorders();
            }
            RenderRegressionResults(results);
            SetGadgetToFinishedState();

        }

        /// <summary>
        /// method used to Render results
        /// </summary>
        /// <param name="results"></param>
        private void RenderRegressionResults(LogRegressionResults results)
        {
            txtConvergence.Visibility = Visibility.Visible;
            txtIterations.Visibility = Visibility.Visible;
            txtFinalLog.Visibility = Visibility.Visible;
            txtCasesIncluded.Visibility = Visibility.Visible;

            txtConvergenceLabel.Visibility = Visibility.Visible;
            txtIterationsLabel.Visibility = Visibility.Visible;
            txtFinalLogLabel.Visibility = Visibility.Visible;
            txtCasesIncludedLabel.Visibility = Visibility.Visible;
            pnlContent.Visibility = System.Windows.Visibility.Visible;
            waitCursor.Visibility = Visibility.Collapsed;
            btnRun.IsEnabled = true;

            if (!string.IsNullOrEmpty(results.ErrorMessage) || results.Variables == null)
            {
                grdParameters.Visibility = System.Windows.Visibility.Collapsed;
                grdStats.Visibility = System.Windows.Visibility.Visible;

                txtConvergence.Text = string.Empty;
                txtIterations.Text = string.Empty;
                txtFinalLog.Text = string.Empty;
                txtCasesIncluded.Text = string.Empty;

                txtIterationsLabel.Text = string.Empty;
                txtFinalLogLabel.Text = string.Empty;
                txtCasesIncludedLabel.Text = string.Empty;
                txtStatus.Text = results.ErrorMessage;
                spContent.Visibility = System.Windows.Visibility.Visible;
                ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
                pnlStatus.Visibility = System.Windows.Visibility.Visible;
                txtStatus.Visibility = System.Windows.Visibility.Visible;
                txtConvergenceLabel.TextWrapping = TextWrapping.Wrap;
                txtConvergenceLabel.MaxWidth = 100.0;

            }
            else
            {
                grdParameters.Visibility = System.Windows.Visibility.Visible;
                grdStats.Visibility = System.Windows.Visibility.Visible;

                txtScoreStatistic.Text = stringLiterals.SPACE + results.ScoreStatistic.ToString("F4") + stringLiterals.SPACE;
                txtScoreDF.Text = stringLiterals.SPACE + results.ScoreDF.ToString() + stringLiterals.SPACE;
                txtScoreP.Text = stringLiterals.SPACE + results.ScoreP.ToString("F4") + stringLiterals.SPACE;

                txtLStatistic.Text = stringLiterals.SPACE + results.LRStatistic.ToString("F4") + stringLiterals.SPACE;
                txtLDF.Text = stringLiterals.SPACE + results.LRDF.ToString() + stringLiterals.SPACE;
                txtLP.Text = stringLiterals.SPACE + results.LRP.ToString("F4") + stringLiterals.SPACE;

                txtConvergence.Text = results.Convergence;
                txtIterations.Text = stringLiterals.SPACE + results.Iterations.ToString() + stringLiterals.SPACE;
                txtFinalLog.Text = stringLiterals.SPACE + results.FinalLikelihood.ToString("F4") + stringLiterals.SPACE;
                txtCasesIncluded.Text = stringLiterals.SPACE + results.CasesIncluded.ToString() + stringLiterals.SPACE;

                txtConvergenceLabel.Text = "Convergence:";
                txtIterationsLabel.Text = "Iterations:";
                txtFinalLogLabel.Text = "Final -2*Log-Likelihood:";
                txtCasesIncludedLabel.Text = "Cases Included:";
                pnlStatus.Visibility = System.Windows.Visibility.Collapsed;
                spContent.Visibility = System.Windows.Visibility.Visible;
                ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
                FilterButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// method that is used to Render Header
        /// </summary>
        private void RenderRegressionHeader()
        {
            RowDefinition rowDefHeader = new RowDefinition();
            rowDefHeader.Height = new GridLength(30);
            grdRegress.RowDefinitions.Add(rowDefHeader);

            for (int y = 0; y < grdRegress.ColumnDefinitions.Count; y++)
            {
                Rectangle rctHeader = new Rectangle();
                rctHeader.Style = Application.Current.Resources["HeaderCell"] as Style;
                Grid.SetRow(rctHeader, 0);
                Grid.SetColumn(rctHeader, y);
                grdRegress.Children.Add(rctHeader);
            }

            TextBlock txtVarHeader = new TextBlock();
            txtVarHeader.Style = Application.Current.Resources["HeaderFont"] as Style;
            Grid.SetRow(txtVarHeader, 0);
            Grid.SetColumn(txtVarHeader, 0);
            grdRegress.Children.Add(txtVarHeader);

            TextBlock txtOddsHeader = new TextBlock();
            txtOddsHeader.Text = "Odds Ratio";
            txtOddsHeader.Style = Application.Current.Resources["HeaderFont"] as Style; //Brushes.White;
            Grid.SetRow(txtOddsHeader, 0);
            Grid.SetColumn(txtOddsHeader, 1);
            grdRegress.Children.Add(txtOddsHeader);

            TextBlock txt95Header = new TextBlock();
            txt95Header.Text = "95%";
            txt95Header.Style = Application.Current.Resources["HeaderFont"] as Style; //Brushes.White;
            Grid.SetRow(txt95Header, 0);
            Grid.SetColumn(txt95Header, 2);
            grdRegress.Children.Add(txt95Header);

            TextBlock txtCIHeader = new TextBlock();
            txtCIHeader.Text = "C.I.";
            txtCIHeader.Style = Application.Current.Resources["HeaderFont"] as Style; //Brushes.White;
            Grid.SetRow(txtCIHeader, 0);
            Grid.SetColumn(txtCIHeader, 3);
            grdRegress.Children.Add(txtCIHeader);

            TextBlock txtCoefficientHeader = new TextBlock();
            txtCoefficientHeader.Text = "Coefficient";
            txtCoefficientHeader.Style = Application.Current.Resources["HeaderFont"] as Style; //Brushes.White;
            Grid.SetRow(txtCoefficientHeader, 0);
            Grid.SetColumn(txtCoefficientHeader, 4);
            grdRegress.Children.Add(txtCoefficientHeader);

            TextBlock txtSEHeader = new TextBlock();
            txtSEHeader.Text = "S.E.";
            txtSEHeader.Style = Application.Current.Resources["HeaderFont"] as Style; //Brushes.White;
            Grid.SetRow(txtSEHeader, 0);
            Grid.SetColumn(txtSEHeader, 5);
            grdRegress.Children.Add(txtSEHeader);

            TextBlock txtZHeader = new TextBlock();
            txtZHeader.Text = "Z-Statistic";
            txtZHeader.Style = Application.Current.Resources["HeaderFont"] as Style; //Brushes.White;
            Grid.SetRow(txtZHeader, 0);
            Grid.SetColumn(txtZHeader, 6);
            grdRegress.Children.Add(txtZHeader);

            TextBlock txtPHeader = new TextBlock();
            txtPHeader.Text = "P-Value";
            txtPHeader.Style = Application.Current.Resources["HeaderFont"] as Style; //Brushes.White;
            Grid.SetRow(txtPHeader, 0);
            Grid.SetColumn(txtPHeader, 7);
            grdRegress.Children.Add(txtPHeader);
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Method that is used to create ListItem with checkbox in it
        /// </summary>
        /// <param name="listBox"></param>
        /// <param name="list"></param>
        private void CreateListBoxWithCheckBox(ListBox listBox, List<EwavColumn> list)
        {
            List<ListBoxItem> listBoxItem = new List<ListBoxItem>();
            listBox.ItemsSource = null;

            foreach (var item in list)
            {
                ListBoxItem tempItem = new ListBoxItem();
                CheckBox chk = new CheckBox();
                chk.Content = item.Name;
                chk.Tag = item;
                tempItem.Content = chk;
                listBoxItem.Add(tempItem);
            }

            listBox.UpdateLayout();
            listBox.ItemsSource = listBoxItem;
        }
        /// <summary>
        /// this method is called to associate the event handlers with controls
        /// </summary>
        private void Construct()
        {
            cbxFieldOutcome.SelectionChanged += new SelectionChangedEventHandler(cbxField_SelectionChanged);
            cbxFields.SelectionChanged += new SelectionChangedEventHandler(cbxFields_SelectionChanged);
            // this.expander1.IsExpanded = true;
        }
        /// <summary>
        /// Method to convert Dictionary object into List of custom created dictionary object
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private List<DictionaryDTO> ConvertDictToList(Dictionary<string, string> dictionary)
        {
            List<DictionaryDTO> listDicDto = new List<DictionaryDTO>();
            foreach (var item in dictionary)
            {
                DictionaryDTO dtoDict = new DictionaryDTO();
                dtoDict.Key = new MyString();
                dtoDict.Key.VarName = item.Key;
                dtoDict.Value = new MyString();
                dtoDict.Value.VarName = item.Value;
                listDicDto.Add(dtoDict);
            }
            return listDicDto;
        }
        /// <summary>
        /// Method that draws the border of Regression
        /// </summary>
        private void DrawRegressionBorders()
        {
            waitCursor.Visibility = Visibility.Collapsed;
            int rdcount = 0;
            foreach (RowDefinition rd in grdRegress.RowDefinitions)
            {
                int cdcount = 0;
                foreach (ColumnDefinition cd in grdRegress.ColumnDefinitions)
                {
                    Rectangle rctBorder = new Rectangle();
                    rctBorder.Style = Application.Current.Resources["DataCell"] as Style;
                    Grid.SetRow(rctBorder, rdcount);
                    Grid.SetColumn(rctBorder, cdcount);
                    grdRegress.Children.Add(rctBorder);
                    cdcount++;
                }
                rdcount++;
            }
        }


        private void SetGridText(Grid grid, TextBlockConfig textBlockConfig)
        {
            TextBlock txt = new TextBlock();
            txt.Text = textBlockConfig.Text;
            txt.Margin = textBlockConfig.Margin;
            txt.VerticalAlignment = textBlockConfig.VerticalAlignment;
            txt.HorizontalAlignment = textBlockConfig.HorizontalAlignment;
            Grid.SetRow(txt, textBlockConfig.RowNumber);
            Grid.SetColumn(txt, textBlockConfig.ColumnNumber);
            grid.Children.Add(txt);
        }

        private void SetGridBar(int rowNumber, double pct)
        {
            Rectangle rctBar = new Rectangle();
            rctBar.Width = 0.1;// pct * 100.0;
            rctBar.Fill = GetGradientBrush();
            rctBar.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(rctBar, rowNumber);
            Grid.SetColumn(rctBar, 4);
            grdRegress.Children.Add(rctBar);

            DoubleAnimation daBar = new DoubleAnimation();
            daBar.From = 1;
            daBar.To = pct * 100.0;
            daBar.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            Storyboard sb = new Storyboard();
            sb.Children.Add(daBar);
            Storyboard.SetTarget(daBar, rctBar);
            Storyboard.SetTargetProperty(daBar, new PropertyPath("Width"));
            if (!LayoutRoot.Resources.Contains("unique_id"))
            {
                LayoutRoot.Resources.Add("unique_id", sb);
            }

            sb.Begin();
        }

        private void AddGridRow(Grid grid, int height)
        {
            waitCursor.Visibility = Visibility.Collapsed;
            grdRegress.Visibility = Visibility.Visible;
            RowDefinition rowDef = new RowDefinition();
            rowDef.Height = new GridLength(height);
            grid.RowDefinitions.Add(rowDef);
        }

        private Brush GetGradientBrush()
        {
            LinearGradientBrush gradient = new LinearGradientBrush();
            gradient.StartPoint = new Point(0.5, 0);
            gradient.EndPoint = new Point(0.5, 1);

            GradientStop color1 = new GradientStop();
            color1.Color = SystemColors.ActiveCaptionColor;//GradientActiveCaptionColor;
            color1.Offset = 0;
            gradient.GradientStops.Add(color1);

            GradientStop color4 = new GradientStop();
            color4.Color = SystemColors.HighlightColor;
            color4.Offset = 0.1;
            gradient.GradientStops.Add(color4);

            GradientStop color2 = new GradientStop();
            color2.Color = SystemColors.WindowColor;
            color2.Offset = 0.5;
            gradient.GradientStops.Add(color2);

            GradientStop color3 = new GradientStop();
            color3.Color = SystemColors.InactiveCaptionColor;//GradientInactiveCaptionColor;
            color3.Offset = 0.75;
            gradient.GradientStops.Add(color3);


            return gradient;
        }

        #endregion

        #region IGadget Members

        /// <summary>
        /// Converts the gadget's output to Html
        /// </summary>
        /// <returns></returns>
        public string ToHTML(bool ForDash = false, string htmlFileName = "", int count = 0)
        {

            HtmlBuilder = new StringBuilder();

            HtmlBuilder.Append("<h2>LogisticRegression HTML Not Implemented</h2> ");

            return "LogisticRegression HTML Not Implemented ";

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
            this.IsProcessing = false;

            if (GadgetProcessingFinished != null)
                GadgetProcessingFinished(this);
        }

        /// <summary>
        /// Gets/sets whether the gadget is processing
        /// </summary>
        public bool IsProcessing
        {
            get
            {
                return isProcessing;
            }
            set
            {
                isProcessing = value;
            }
        }

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
            if (cbxFieldOutcome.SelectedIndex > -1 && (lbxOtherFields.Items.Count > 0 || lbxDummyTerms.Items.Count > 0))
            {
                Dictionary<string, string> inputVariableList = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                inputVariableList = CreateInputVariableList();

            }
        }

        private Dictionary<string, string> CreateInputVariableList()
        {
            Dictionary<string, string> inputVariableList = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                if (!((EwavColumn)cbxFieldOutcome.SelectedItem == null) && !string.IsNullOrEmpty(((EwavColumn)cbxFieldOutcome.SelectedItem).Name.ToString().Trim()))
                {
                    inputVariableList.Add(((EwavColumn)cbxFieldOutcome.SelectedItem).Name.ToString(), "dependvar");
                }


                if (cbxFieldWeight.SelectedIndex >= 0 && !string.IsNullOrEmpty(((EwavColumn)cbxFieldWeight.SelectedItem).Name.ToString().Trim()))
                {
                    inputVariableList.Add(((EwavColumn)cbxFieldWeight.SelectedItem).Name.ToString(), "weightvar");
                }

                if (checkboxNoIntercept.IsChecked == true)
                {
                    inputVariableList.Add("intercept", "false");
                }
                else
                {
                    inputVariableList.Add("intercept", "true");
                }

                if (checkboxIncludeMissing.IsChecked == true)
                {
                    inputVariableList.Add("includemissing", "true");
                }
                else
                {
                    inputVariableList.Add("includemissing", "false");
                }

                if (cbxFieldMatch.SelectedIndex >= 0 && !string.IsNullOrEmpty(((EwavColumn)cbxFieldMatch.SelectedItem).Name.ToString().Trim()))
                {
                    inputVariableList.Add(((EwavColumn)cbxFieldMatch.SelectedItem).Name.ToString(), "MatchVar");
                }

                double p = 0.95;
                if (cbxConf.SelectedIndex > -1 && !string.IsNullOrEmpty(cbxConf.SelectedItem.ToString().Trim()))
                {
                    bool success = Double.TryParse(cbxConf.SelectedItem.ToString().Replace("%", string.Empty), out p);
                    if (!success)
                    {
                        p = 0.95;
                    }
                }
                inputVariableList.Add("P", p.ToString());

                foreach (var s in lbxOtherFields.Items)
                {
                    string t = ((CheckBox)((ListBoxItem)s).Content).Content.ToString();
                    if (!string.IsNullOrEmpty(t) && !inputVariableList.ContainsKey(t))
                    {
                        inputVariableList.Add(t, "unsorted");
                    }
                }

                foreach (var s in lbxInteractionTerms.Items)
                {

                    string t = ((CheckBox)((ListBoxItem)s).Content).Content.ToString();
                    if (!string.IsNullOrEmpty(t) && !inputVariableList.ContainsKey(t))
                    {
                        inputVariableList.Add(t, "term");
                    }
                }

                foreach (var s in lbxDummyTerms.Items)
                {

                    string t = ((CheckBox)((ListBoxItem)s).Content).Content.ToString();
                    if (!string.IsNullOrEmpty(t) && !inputVariableList.ContainsKey(t))
                    {
                        inputVariableList.Add(t, "discrete");
                    }
                }
                ErrorOnPage = false;
            }
            catch (ArgumentException)
            {
                RenderFinishWithError("The same variable cannot be used more than once.");
            }

            return inputVariableList;
        }

        private void RenderFinishWithError(string s)
        {
            ErrorOnPage = true;
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            pnlStatus.Visibility = System.Windows.Visibility.Visible;
            txtStatus.Text = s;
            txtStatus.Visibility = System.Windows.Visibility.Visible;
            pnlContent.Visibility = System.Windows.Visibility.Collapsed;
            waitCursor.Visibility = System.Windows.Visibility.Collapsed;
        }

        public XNode Serialize(XDocument doc)
        {
            string dependVar = string.Empty;
            string weightVar = string.Empty;
            string matchVar = string.Empty;
            string pvalue = string.Empty;
            bool intercept = true;
            bool includeMissing = false;

            if (cbxFieldOutcome.SelectedItem != null)
            {
                dependVar = ((EwavColumn)cbxFieldOutcome.SelectedItem).Name.ToString();
            }

            if (cbxFieldWeight.SelectedItem != null)
            {
                weightVar = ((EwavColumn)cbxFieldWeight.SelectedItem).Name.ToString();
            }

            if (cbxFieldMatch.SelectedItem != null)
            {
                matchVar = ((EwavColumn)cbxFieldMatch.SelectedItem).Name.ToString();
            }

            if (checkboxNoIntercept.IsChecked == true)
            {
                intercept = false;
            }
            else
            {
                intercept = true;
            }

            if (checkboxIncludeMissing.IsChecked == true)
            {
                includeMissing = true;
            }
            else
            {
                includeMissing = false;
            }

            double p = 0.95;
            if (cbxConf.SelectedItem != null && !string.IsNullOrEmpty(cbxConf.SelectedItem.ToString()))
            {
                bool success = Double.TryParse(cbxConf.SelectedItem.ToString().Replace("%", string.Empty), out p);
                if (!success)
                {
                    pvalue = 95.ToString();
                }
                else
                {
                    pvalue = p.ToString();
                }
            }



            XElement element = new XElement("gadget",
               new XAttribute("top", Canvas.GetTop(this).ToString("F0")),
               new XAttribute("left", Canvas.GetLeft(this).ToString("F0")),
               new XAttribute("collapsed", "false"),
               new XAttribute("gadgetType", "Ewav.LogisticRegression"),
               new XElement("mainVariable", dependVar),
               new XElement("weightVariable", weightVar),
               new XElement("matchVariable", matchVar),
               new XElement("pvalue", pvalue),
               new XElement("includeMissing", includeMissing),
               new XElement("intercept", intercept.ToString()),
                new XElement("gadgetTitle", GName.Text),
               new XElement("gadgetDescription", txtGadgetDescription.Text)
                   );

            //element.Attributes.Append(locationY);
            //element.Attributes.Append(locationX);
            //element.Attributes.Append(collapsed);
            //element.Attributes.Append(type);

            if ((lbxDummyTerms.Items.Count + lbxOtherFields.Items.Count) > 0)
            {
                string xmlCovariateString = string.Empty;
                XElement covariateElement = new XElement("covariates");

                for (int i = 0; i < lbxOtherFields.Items.Count; i++)
                {
                    covariateElement.Add(new XElement("covariate", ((CheckBox)((ListBoxItem)lbxOtherFields.Items[i]).Content).Content.ToString()));
                }

                for (int i = 0; i < lbxDummyTerms.Items.Count; i++)
                {
                    //xmlCovariateString = xmlCovariateString + "<dummy>" + s + "</dummy>";
                    covariateElement.Add(new XElement("dummy", ((CheckBox)((ListBoxItem)lbxDummyTerms.Items[i]).Content).Content.ToString()));
                }

                for (int i = 0; i < lbxInteractionTerms.Items.Count; i++)
                {
                    //xmlCovariateString = xmlCovariateString + "<interactionTerm>" + s + "</interactionTerm>";
                    covariateElement.Add(new XElement("interactionTerm", ((CheckBox)((ListBoxItem)lbxInteractionTerms.Items[i]).Content).Content.ToString()));
                }

                //covariateElement.InnerXml = xmlCovariateString;
                element.Add(covariateElement);
            }

            if (this.GadgetFilters != null)
            {
                this.GadgetFilters.Serialize(element);
            }
            return element;
        }

        //public void CreateFromXml(XmlElement element)
        //{
        //    this.loadingCombos = true;

        //    foreach (XmlElement child in element.ChildNodes)
        //    {
        //        switch (child.Name.ToLower())
        //        {
        //            case "mainvariable":
        //                cbxFieldOutcome.Text = child.InnerText;
        //                break;
        //            case "weightvariable":
        //                cbxFieldWeight.Text = child.InnerText;
        //                break;
        //            case "matchvariable":
        //                cbxFieldMatch.Text = child.InnerText;
        //                break;
        //            case "pvalue":
        //                if (child.InnerText.Equals("90")) { cbxConf.SelectedIndex = 1; }
        //                if (child.InnerText.Equals("95")) { cbxConf.SelectedIndex = 2; }
        //                if (child.InnerText.Equals("99")) { cbxConf.SelectedIndex = 3; }
        //                break;
        //            case "intercept":
        //                if (child.InnerText.ToLower().Equals("false")) { checkboxNoIntercept.IsChecked = true; }
        //                break;
        //            case "covariates":
        //                foreach (XmlElement covariate in child.ChildNodes)
        //                {
        //                    if (covariate.Name.ToLower().Equals("covariate"))
        //                    {
        //                        lbxOtherFields.Items.Add(covariate.InnerText);
        //                    }
        //                    if (covariate.Name.ToLower().Equals("dummy"))
        //                    {
        //                        lbxDummyTerms.Items.Add(covariate.InnerText);
        //                    }
        //                    if (covariate.Name.ToLower().Equals("interactionterm"))
        //                    {
        //                        lbxInteractionTerms.Items.Add(covariate.InnerText);
        //                    }
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
        #endregion


        #region Event handlers

        /// <summary>
        /// Event that is raised with RegressResult Table is Loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lrViewModel_RegressResultsLoadedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            LogisticRegressionViewModel logisticRegressionViewModel = (LogisticRegressionViewModel)this.DataContext;
            LogRegressionResults regResults = logisticRegressionViewModel.RegressionResults;
            Dictionary<string, string> inputVariableList = CreateInputVariableList();
            worker_DoWork(regResults, inputVariableList);

        }

        /// <summary>
        /// Event that is raised with Columns are loaded. This event sets the itemsSource Properties of ComboBoxes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FillDropDowns()    //     object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            //        LogisticRegressionViewModel logRegressionviewModel = (LogisticRegressionViewModel)sender;
            cbxConf.Items.Clear();
            cbxConf.ItemsSource = null;

            //if (cbxConf.Items.Count > 0)
            //{

            //}

            cbxConf.Items.Add(" ");
            cbxConf.Items.Add("90%");
            cbxConf.Items.Add("95%");
            cbxConf.Items.Add("99%");
            cbxConf.SelectedIndex = Index4;

            cbxFields.ItemsSource = null;
            cbxFields.Items.Clear();

            lbxOtherFields.ItemsSource = null;
            lbxOtherFields.Items.Clear();

            lbxInteractionTerms.ItemsSource = null;
            lbxInteractionTerms.Items.Clear();

            List<EwavColumn> SourceColumns = applicationViewModel.EwavSelectedDatasource.AllColumns;     //   logRegressionviewModel.Columns;

            //cbxFieldOutcome.SelectedIndex = 0;

            //List<ColumnDataType> columnDataType = new List<ColumnDataType>();
            //columnDataType.Add(ColumnDataType.Boolean);
            //columnDataType.Add(ColumnDataType.Numeric);
            //columnDataType.Add(ColumnDataType.Text);
            //columnDataType.Add(ColumnDataType.UserDefined);

            IEnumerable<EwavColumn> CBXFieldCols = from cols in SourceColumns
                                                   where GetFieldDataType.Contains(cols.SqlDataTypeAsString)
                                                   orderby cols.Name
                                                   select cols;


            List<EwavColumn> colsList = CBXFieldCols.ToList();

            colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            cbxFieldOutcome.ItemsSource = colsList;
            cbxFieldOutcome.SelectedValue = "Index";
            cbxFieldOutcome.DisplayMemberPath = "Name";
            cbxFieldOutcome.SelectedIndex = Index1;

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
            cbxFieldWeight.SelectedIndex = Index2;

            //============================================



            CBXFieldCols = from cols in SourceColumns
                           where GetMatchDataType.Contains(cols.SqlDataTypeAsString)
                           orderby cols.Name
                           select cols;

            colsList = CBXFieldCols.ToList();

            colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            cbxFieldMatch.ItemsSource = colsList;
            cbxFieldMatch.SelectedValue = "Index";
            cbxFieldMatch.DisplayMemberPath = "Name";
            cbxFieldMatch.SelectedIndex = Index3;
            //============================================

            //columnDataType.Clear();
            //columnDataType.Add(ColumnDataType.Numeric);
            //columnDataType.Add(ColumnDataType.Boolean);
            //columnDataType.Add(ColumnDataType.Text);
            //columnDataType.Add(ColumnDataType.UserDefined);

            CBXFieldCols = from cols in SourceColumns
                           where GetFieldDataType.Contains(cols.SqlDataTypeAsString)
                           orderby cols.Name
                           select cols;

            colsList = CBXFieldCols.ToList();

            colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            cbxFields.ItemsSource = colsList;
            cbxFields.SelectedValue = "Index";
            cbxFields.DisplayMemberPath = "Name";
            //cbxFields.SelectedIndex = Index5;

            //=============================================

            this.CreateListBoxWithCheckBox(this.lbxDummyTerms, this.dummVars);
            this.CreateListBoxWithCheckBox(this.lbxInteractionTerms, this.interterms);
            this.CreateListBoxWithCheckBox(this.lbxOtherFields, this.otherVars);

        }

        private void SelectDropDownValues()
        {
            if (Index1 > -1)
            {
                cbxFieldOutcome.SelectedIndex = Index1;
            }

            if (Index2 > -1)
            {
                cbxFieldWeight.SelectedIndex = Index2;
            }

            if (Index3 > -1)
            {
                cbxFieldMatch.SelectedIndex = Index3;
            }

            if (Index4 > -1)
            {
                cbxConf.SelectedIndex = Index4;
            }

            if (Index5 > -1)
            {
                cbxFields.SelectedIndex = Index5;
            }
        }
        private void lbxOtherFields_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbxOtherFields.SelectedItems.Count == 1)
            {
                lbxOtherFields.Items.Remove(((EwavColumn)lbxOtherFields.SelectedItem).Name);
            }
        }

        private void lbxDummyTerms_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbxDummyTerms.SelectedItems.Count == 1)
            {
                lbxDummyTerms.Items.Remove(((EwavColumn)lbxDummyTerms.SelectedItem).Name);
                lbxOtherFields.Items.Add(((EwavColumn)lbxDummyTerms.SelectedItem).Name);
            }
        }

        private void lbxInteractionTerms_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbxInteractionTerms.SelectedItems.Count == 1)
            {
                lbxInteractionTerms.Items.Remove(((EwavColumn)lbxInteractionTerms.SelectedItem).Name);
            }
        }



        private void btnMakeDummy_Click(object sender, RoutedEventArgs e)
        {
            List<ListBoxItem> listItems = this.lbxOtherFields.ItemsSource as List<ListBoxItem>;
            if (null != listItems)
            {
                foreach (var item in listItems)
                {
                    CheckBox chk = item.Content as CheckBox;
                    if (null != chk && chk.IsChecked.Value)
                    {
                        EwavColumn selectedItem = chk.Tag as EwavColumn;

                        if (null != selectedItem)
                        {
                            this.dummVars.Add(selectedItem);
                            this.otherVars.Remove(selectedItem);

                        }
                    }
                }
                this.CreateListBoxWithCheckBox(this.lbxDummyTerms, dummVars);
                this.CreateListBoxWithCheckBox(this.lbxOtherFields, otherVars);

            }
        }

        private void btnMakeInteractionTerms_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //List<ListBoxItem> listItems = this.lbxOtherFields.ItemsSource as List<ListBoxItem>;
            //if (null != listItems)
            //{
            //    int itemCount = 0;
            //    for (int i = 0; i < listItems.Count; i++)
            //    {
            //        CheckBox chk = listItems[i].Content as CheckBox;
            //        if (chk.IsChecked == true)
            //        {
            //            itemCount++;

            //        }
            //    }

            //    if (itemCount == 2)
            //    {

            //        string term = string.Empty; //lbxOtherFields.SelectedItems[0] + "*" + lbxOtherFields.SelectedItems[1];


            //        foreach (var item in listItems)
            //        {
            //            CheckBox chk = item.Content as CheckBox;
            //            if (null != chk && chk.IsChecked.Value)
            //            {
            //                string selectedItem = chk.Tag as string;

            //                if (null != selectedItem)
            //                {
            //                    if (string.IsNullOrEmpty(term))
            //                    {
            //                        term = selectedItem;
            //                    }
            //                    else
            //                    {
            //                        term = term + "*" + selectedItem;
            //                    }


            //                }
            //            }
            //        }
            //        if (!this.interterms.Contains(term))
            //        {
            //            this.interterms.Add(term);
            //        }

            //        this.CreateListBoxWithCheckBox(this.lbxInteractionTerms, interterms);
            //    }

            //}
            //ResetOtherVariableList();

            List<ListBoxItem> listItems = this.lbxOtherFields.ItemsSource as List<ListBoxItem>;
            bool IsUDVInUse = false;
            if (null != listItems)
            {
                int itemCount = 0;
                for (int i = 0; i < listItems.Count; i++)
                {
                    CheckBox chk = listItems[i].Content as CheckBox;
                    if (chk.IsChecked == true)
                    {
                        itemCount++;
                    }
                }

                if (itemCount == 2)
                {
                    string term = string.Empty; //lbxOtherFields.SelectedItems[0] + "*" + lbxOtherFields.SelectedItems[1];

                    foreach (var item in listItems)
                    {
                        CheckBox chk = item.Content as CheckBox;
                        if (null != chk && chk.IsChecked.Value)
                        {
                            EwavColumn selectedItem = chk.Tag as EwavColumn;

                            if (null != selectedItem)
                            {
                                if (string.IsNullOrEmpty(term))
                                {
                                    term = selectedItem.Name;
                                }
                                else
                                {
                                    term = term + "*" + selectedItem.Name;
                                }
                                if (selectedItem.IsUserDefined)
                                {
                                    IsUDVInUse = selectedItem.IsUserDefined;
                                }
                            }
                        }
                    }

                    if (interterms.Count > 0)
                    {
                        for (int i = 0; i < interterms.Count; i++)
                        {
                            if (!(interterms[i].Name == term))
                            {
                                interterms.Add(new EwavColumn { Name = term, IsUserDefined = IsUDVInUse });
                            }
                        }
                    }
                    else
                    {
                        interterms.Add(new EwavColumn { Name = term, IsUserDefined = IsUDVInUse });
                    }


                    //if (!this.interterms.Contains(term))
                    //{
                    //    this.interterms.Add(term);
                    //}

                    this.CreateListBoxWithCheckBox(this.lbxInteractionTerms, interterms);
                }
            }
            ResetOtherVariableList();
        }

        private void ResetOtherVariableList()
        {
            List<ListBoxItem> listItems = this.lbxOtherFields.ItemsSource as List<ListBoxItem>;
            if (listItems != null)
            {
                for (int i = 0; i < listItems.Count; i++)
                {
                    CheckBox chk = listItems[i].Content as CheckBox;
                    if (chk.IsChecked == true)
                    {
                        chk.IsChecked = false;

                    }
                }
            }
        }


        private void btnRemoveDummy_Click(object sender, RoutedEventArgs e)
        {
            //List<ListBoxItem> listItems = this.lbxDummyTerms.ItemsSource as List<ListBoxItem>;
            //if (null != listItems)
            //{
            //    foreach (var item in listItems)
            //    {
            //        CheckBox chk = item.Content as CheckBox;
            //        if (null != chk && chk.IsChecked.Value)
            //        {
            //            string selectedItem = chk.Tag as string;

            //            if (null != selectedItem)
            //            {
            //                this.dummVars.Remove(selectedItem);
            //            }
            //        }
            //    }

            //    this.CreateListBoxWithCheckBox(this.lbxDummyTerms, this.dummVars);
            //}

            List<ListBoxItem> listItems = this.lbxDummyTerms.ItemsSource as List<ListBoxItem>;
            if (null != listItems)
            {
                foreach (var item in listItems)
                {
                    CheckBox chk = item.Content as CheckBox;
                    if (null != chk && chk.IsChecked.Value)
                    {
                        EwavColumn selectedItem = chk.Tag as EwavColumn;

                        if (null != selectedItem)
                        {
                            this.dummVars.Remove(selectedItem);
                            //this.otherVars.Remove(selectedItem);

                        }
                    }
                }
                this.CreateListBoxWithCheckBox(this.lbxDummyTerms, dummVars);
                //this.CreateListBoxWithCheckBox(this.lbxOtherFields, otherVars);

            }
        }


        private void btnRemoveInteraction_Click(object sender, RoutedEventArgs e)
        {
            //List<ListBoxItem> listItems = this.lbxInteractionTerms.ItemsSource as List<ListBoxItem>;
            //if (null != listItems)
            //{
            //    foreach (var item in listItems)
            //    {
            //        CheckBox chk = item.Content as CheckBox;
            //        if (null != chk && chk.IsChecked.Value)
            //        {
            //            string selectedItem = chk.Tag as string;

            //            if (null != selectedItem)
            //            {
            //                this.interterms.Remove(selectedItem);

            //            }
            //        }
            //    }
            //    this.CreateListBoxWithCheckBox(this.lbxInteractionTerms, interterms);

            //}
            List<ListBoxItem> listItems = this.lbxInteractionTerms.ItemsSource as List<ListBoxItem>;

            if (null != listItems)
            {
                string term = string.Empty;

                foreach (var item in listItems)
                {
                    CheckBox chk = item.Content as CheckBox;
                    if (null != chk && chk.IsChecked.Value)
                    {
                        EwavColumn selectedItem = chk.Tag as EwavColumn;
                        interterms.Remove(selectedItem);
                    }
                }

                this.CreateListBoxWithCheckBox(this.lbxInteractionTerms, interterms);
            }
            ResetOtherVariableList();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseGadgetOnClick();
        }

        private void LayoutRoot_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetSafePosition(GetParentObject<Grid>(this, "LayoutRoot"));
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).GadgetNameOnRightClick = MyControlName;
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).StrataList = null;
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).SelectedGadget = this;
            cmnClass.UpdateZOrder(this, true, cmnClass.GetParentObject<Grid>(this, "LayoutRoot"));

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

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            this.cbxFieldOutcome.SelectedIndex = -1;
            this.cbxFieldWeight.SelectedIndex = -1;
            this.cbxFieldMatch.SelectedIndex = -1;
            this.cbxConf.SelectedIndex = -1;
            this.checkboxNoIntercept.IsChecked = false;
            this.checkboxIncludeMissing.IsChecked = false;
            this.cbxFields.SelectedIndex = -1;
            this.otherVars = new List<EwavColumn>();
            this.dummVars = new List<EwavColumn>();
            this.interterms = new List<EwavColumn>();
            this.lbxOtherFields.ItemsSource = null;
            this.lbxDummyTerms.ItemsSource = null;
            this.lbxInteractionTerms.ItemsSource = null;
            this.btnRun.IsEnabled = true;
        }

        private void btnRemoveVariables_Click(object sender, RoutedEventArgs e)
        {
            //List<ListBoxItem> listItems = this.lbxOtherFields.ItemsSource as List<ListBoxItem>;
            //if (null != listItems)
            //{
            //    foreach (var item in listItems)
            //    {
            //        CheckBox chk = item.Content as CheckBox;
            //        if (null != chk && chk.IsChecked.Value)
            //        {
            //            string selectedItem = chk.Tag as string;

            //            if (null != selectedItem)
            //            {
            //                this.otherVars.Remove(selectedItem);

            //            }
            //        }
            //    }

            //    this.CreateListBoxWithCheckBox(this.lbxOtherFields, this.otherVars);
            //}
            List<ListBoxItem> listItems = this.lbxOtherFields.ItemsSource as List<ListBoxItem>;
            if (null != listItems)
            {
                foreach (var item in listItems)
                {
                    CheckBox chk = item.Content as CheckBox;
                    if (null != chk && chk.IsChecked.Value)
                    {
                        EwavColumn selectedItem = chk.Tag as EwavColumn;

                        if (null != selectedItem)
                        {
                            this.otherVars.Remove(selectedItem);
                        }
                    }
                }

                this.CreateListBoxWithCheckBox(this.lbxOtherFields, this.otherVars);
            }
        }
        #endregion

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
        void CloseGadgetOnClick()
        {
            CloseGadget confirm = new CloseGadget(this);
            confirm.Show();
        }

        void CloseGadget()
        {
            applicationViewModel.CloseGadget(this);

        }

        public ApplicationViewModel ApplicationViewModel
        {
            get { throw new NotImplementedException(); }
        }

        public void SaveState()
        {
            Index1 = cbxFieldOutcome.SelectedIndex;
            Index2 = cbxFieldWeight.SelectedIndex;
            Index3 = cbxFieldMatch.SelectedIndex;
            Index4 = cbxConf.SelectedIndex;
            Index5 = cbxFields.SelectedIndex;
        }

        private void cbxFieldOutcome_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.btnRun.IsEnabled = true;
        }


        public void CreateFromXml(XElement element)
        {
            LoadingCanvas = true;

            //InitializeControl();

            List<EwavColumn> primaryColList = cmnClass.GetItemsSource(GetFieldDataType);
            List<EwavColumn> weightColList = cmnClass.GetItemsSource(GetWeightDataType);
            List<EwavColumn> matchColList = cmnClass.GetItemsSource(GetMatchDataType);

            this.GadgetFilters = new List<EwavDataFilterCondition>();
            foreach (XElement child in element.Descendants())
            {
                switch (child.Name.ToString().ToLower())
                {
                    case "mainvariable":
                        cbxFieldOutcome.SelectedIndex = cmnClass.FindIndexToSelect(primaryColList, child.Value.ToString().Replace("&lt;", "<"));
                        break;
                    case "weightvariable":
                        cbxFieldWeight.SelectedIndex = cmnClass.FindIndexToSelect(weightColList, child.Value.ToString().Replace("&lt;", "<"));
                        break;
                    case "matchvariable":
                        cbxFieldMatch.SelectedIndex = cmnClass.FindIndexToSelect(matchColList, child.Value.ToString().Replace("&lt;", "<"));
                        break;
                    case "pvalue":
                        if (child.Value.Equals("90")) { cbxConf.SelectedIndex = 1; }
                        if (child.Value.Equals("95")) { cbxConf.SelectedIndex = 2; }
                        if (child.Value.Equals("99")) { cbxConf.SelectedIndex = 3; }
                        break;
                    case "intercept":
                        if (child.Value.ToLower().Equals("false")) { checkboxNoIntercept.IsChecked = true; }
                        break;
                    case "covariates":
                        List<EwavColumn> listOfCovariate = new List<EwavColumn>();
                        List<EwavColumn> listOfDummy = new List<EwavColumn>();
                        List<EwavColumn> listOfInter = new List<EwavColumn>();
                        foreach (XElement covariate in child.Descendants())
                        {

                            if (covariate.Name.ToString().ToLower().Equals("covariate"))
                            {
                                EwavColumn col = cmnClass.FindEwavColumn(covariate.Value.ToString(), primaryColList);

                                if (col != null)
                                {
                                    //listOfCovariate.Add(col);
                                    otherVars.Add(col);
                                }

                            }
                            if (covariate.Name.ToString().ToLower().Equals("dummy"))
                            {
                                EwavColumn col = cmnClass.FindEwavColumn(covariate.Value.ToString(), primaryColList);
                                if (col != null)
                                {
                                    //listOfDummy.Add(col);
                                    dummVars.Add(col);
                                }

                            }
                            if (covariate.Name.ToString().ToLower().Equals("interactionterm"))
                            {
                                EwavColumn col = new EwavColumn();
                                col.Name = covariate.Value.ToString();
                                //lbxInteractionTerms.Items.Add(covariate.Value);
                                if (col.Name.Length > 0)
                                {
                                    //listOfInter.Add(col);
                                    interterms.Add(col);
                                }

                            }
                        }

                        if (otherVars.Count > 0)
                        {
                            CreateListBoxWithCheckBox(lbxOtherFields, otherVars);
                        }
                        if (dummVars.Count > 0)
                        {
                            CreateListBoxWithCheckBox(lbxDummyTerms, dummVars);
                        }
                        if (interterms.Count > 0)
                        {
                            CreateListBoxWithCheckBox(lbxInteractionTerms, interterms);
                        }
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

            DoLogisticRegression();

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

            DoLogisticRegression();


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
                DoLogisticRegression();
            }
        }


    }


}

namespace Ewav.Web.Services
{
    public partial class LogisticRegressionDomainContext
    {
        //This is the place to set RIA Services query timeout. 
        //TimeSpan(0, 5, 0) means five minutes vs the 
        //default 60 sec
        partial void OnCreated()
        {
            if (!DesignerProperties.GetIsInDesignMode(Application.
                Current.RootVisual))
                ((WebDomainClient<ILogisticRegressionDomainServiceContract>)
                    DomainClient).ChannelFactory.Endpoint.Binding.SendTimeout =
                    new TimeSpan(0, 120, 0);
        }
    }
}