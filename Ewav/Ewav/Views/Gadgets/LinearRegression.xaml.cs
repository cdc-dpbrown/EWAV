/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       LinearRegression.xaml.cs
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
using System.Text;
using System.Xml.Linq;
using Ewav.ExtensionMethods;

//using System.Data;
namespace Ewav
{
    /// <summary>
    /// Interaction logic for LinearRegressionControl.xaml
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(UserControl))]
    [ExportMetadata("type", "gadget")]
    [ExportMetadata("tabindex", "6")]
    public partial class LinearRegression : UserControl, IGadget, IEwavGadget, ICustomizableGadget
    {
        #region Private Variables

        int Index1 = -1, Index2 = -1, Index3 = -1, Index4 = -1;
        EwavColumn Col1, Col2, Col3, Col4, DFInUse = null;






        /// <summary>
        /// Variable that contains the Regression Table returned from server.
        /// </summary>
        public List<ListOfStringClass> regressTable;

        /// <summary>
        /// boolean indicator that shows if there is any error on page.
        /// </summary>
        private bool errorOnPage;

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


        public bool ErrorOnPage
        {
            get { return errorOnPage; }
            set { errorOnPage = value; }
        }

        private bool loadingCanvas = false;

        public bool LoadingCanvas
        {
            get { return loadingCanvas; }
            set { loadingCanvas = value; }
        }

        private bool isProcessing;
        private GadgetParameters gadgetOptions;
        /// <summary>
        /// Lists of string used to assign the itemsSource property of listBoxes
        /// </summary>
        //List<string> otherVars = new List<string>();
        List<EwavColumn> otherVars = new List<EwavColumn>();
        //List<string> dummVars = new List<string>();
        //List<string> interterms = new List<string>();

        List<EwavColumn> dummVars = new List<EwavColumn>();

        List<EwavColumn> interterms = new List<EwavColumn>();

        List<EwavColumn> otherVars_bk = new List<EwavColumn>();
        //List<string> dummVars = new List<string>();
        //List<string> interterms = new List<string>();

        List<EwavColumn> dummVars_bk = new List<EwavColumn>();

        List<EwavColumn> interterms_bk = new List<EwavColumn>();


        ClientCommon.Common cmnClass = new ClientCommon.Common();

        /// <summary>
        /// Instantiating the object for SharedLiterals class to access SharedLiterals
        /// </summary>
        StringLiterals stringLiterals = new StringLiterals();

        private bool loadingDropDowns = false;

        public bool LoadingDropDowns
        {
            get { return loadingDropDowns; }
            set { loadingDropDowns = value; }
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

        #region Delegates

        private delegate void RequestUpdateStatusDelegate(string statusMessage);

        #endregion // Delegates

        /// <summary>
        /// Following region contains the members for IEwavGadget interface.
        /// </summary>

        #region IEwavGadget Members

        private string myControlName = "LinearRegression";

        public string MyControlName
        {
            get
            {
                return myControlName;
            }
        }

        public string MyUIName
        {
            get
            {

                return "Linear Regression";

            }
        }



        ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;

        public ApplicationViewModel ApplicationViewModel
        {
            get
            {
                return applicationViewModel;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public LinearRegression()
        {
            InitializeComponent();


            // Init gadget parameters with default values
            gadgetOptions = new GadgetParameters();
            gadgetOptions.ShouldIncludeFullSummaryStatistics = true;
            gadgetOptions.ShouldIncludeMissing = false;
            gadgetOptions.ShouldSortHighToLow = false;
            gadgetOptions.ShouldUseAllPossibleValues = false;
            gadgetOptions.StrataVariableNames = new List<string>();

            this.Loaded += new RoutedEventHandler(LinearRegression_Loaded);
            //initializeControl();
            FillDropDowns();
        }

        void LinearRegression_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeControl();

        }

        private void InitializeControl()
        {
            LinearRegressionViewModel lrViewModel = (LinearRegressionViewModel)this.DataContext;
            lrViewModel.RegressResultsLoadedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(lrViewModel_RegressResultsLoadedEvent);
            lrViewModel.ErrorNotice += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(lrViewModel_ErrorNotice);
            applicationViewModel.ApplyDataFilterEvent += new Client.Application.ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);
            applicationViewModel.DefinedVariableAddedEvent += new Client.Application.DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);
            //    lrViewModel.ColumnsLoadedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(lrViewModel_ColumnsLoadedEvent);
            applicationViewModel.DefinedVariableInUseDeletedEvent += new Client.Application.DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableInUseDeletedEvent);
            applicationViewModel.DefinedVariableNotInUseDeletedEvent += new Client.Application.DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
            applicationViewModel.PreColumnChangedEvent += new Client.Application.PreColumnChangedEventHandler(applicationViewModel_PreColumnChangedEvent);

            applicationViewModel.UnloadedEvent += new Client.Application.UnloadedEventHandler(applicationViewModel_UnloadedEvent);
            Construct();
            //  lrViewModel.GetColumns("NEDS", "vwExternalData");    

            // FillDropDowns();
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
            DoLinearRegression();
        }

        void applicationViewModel_DefinedVariableInUseDeletedEvent(object o)
        {
            ResetGadget();
        }

        /// <summary>
        /// Resets the gadget to initial state.
        /// </summary>
        void ResetGadget()
        {
            SearchIndex();
            if (IsDFUsedInThisGadget())
            {
                Index1 = Index2 = Index3 = Index4 = -1;
                pnlContent.Visibility = System.Windows.Visibility.Collapsed;
                pnlStatus.Visibility = System.Windows.Visibility.Collapsed;
                this.lbxDummyTerms.ItemsSource = null;
                this.lbxInteractionTerms.ItemsSource = null;
                this.lbxOtherFields.ItemsSource = null;
                dummVars.Clear();
                otherVars.Clear();
                interterms.Clear();
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
                //Col3 != null && Col3.Name == applicationViewModel.ItemToBeRemoved.Name ||
                        Col4 != null && Col4.Name == applicationViewModel.ItemToBeRemoved.Name;
        }

        void applicationViewModel_DefinedVariableAddedEvent(object o)
        {
            SearchIndex();
            LoadingDropDowns = true;
            FillDropDowns();
            LoadingDropDowns = false;
            DoLinearRegression();
        }

        //private void SaveState()
        //{
        //    Index1 = cbxFieldOutcome.SelectedIndex;
        //    Index2 = cbxFieldWeight.SelectedIndex;
        //    Index3 = cbxConf.SelectedIndex;
        //    Index4 = cbxFields.SelectedIndex;
        //}

        /// <summary>
        /// Saves the Values of Columns.
        /// </summary>
        private void SaveColumnValues()
        {
            Col1 = (EwavColumn)cbxFieldOutcome.SelectedItem;
            Col2 = (EwavColumn)cbxFieldWeight.SelectedItem;
            //Col3 = (EwavColumn)cbxConf.SelectedItem;
            Col4 = (EwavColumn)cbxFields.SelectedItem;
            otherVars_bk = otherVars;
            interterms_bk = interterms;
            dummVars_bk = dummVars;
        }

        /// <summary>
        /// Searches current index of the columns.
        /// </summary>
        private void SearchIndex()
        {
            //Index1 = SearchCurrentIndex(Col1);
            //Index2 = SearchCurrentIndex(Col2);
            //Index3 = cbxConf.SelectedIndex;
            //Index4 = SearchCurrentIndex(Col4);

            ClientCommon.Common CommonClass = new ClientCommon.Common();

            Index1 = CommonClass.SearchCurrentIndex(Col1, CommonClass.GetItemsSource(GetFieldDataType));

            Index2 = CommonClass.SearchCurrentIndex(Col2, CommonClass.GetItemsSource(GetWeightDataType));

            Index3 = cbxConf.SelectedIndex;

            Index4 = CommonClass.SearchCurrentIndex(Col4, CommonClass.GetItemsSource(GetFieldDataType));

            otherVars = otherVars_bk;
            interterms = interterms_bk;
            dummVars = dummVars_bk;

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


            //if ((applicationViewModel.ItemToBeRemoved != null) &&
            //   DFInUse != null &&
            //   dfColFound)
            if (applicationViewModel.RemoveIndicator &&
                (IsDFUsedInThisGadget() || dfColFound))
            {
                ResetGadget();
            }
            else
            {
                DoLinearRegression();
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

        #region ViewModel Event Handlers

        void FillDropDowns()     //       object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            //  LinearRegressionViewModel logRegressionviewModel = (LinearRegressionViewModel)sender;
            cbxConf.Items.Clear();
            cbxConf.ItemsSource = null;

            cbxConf.Items.Add(" ");
            cbxConf.Items.Add("90%");
            cbxConf.Items.Add("95%");
            cbxConf.Items.Add("99%");
            cbxConf.SelectedIndex = Index3;
            //cbxFields.Items.Clear();
            if (cbxFields.Items.Count > 0) { cbxFields.ItemsSource = null; }

            //lbxOtherFields.Items.Clear();
            if (lbxOtherFields.Items.Count > 0) { lbxOtherFields.ItemsSource = null; }

            //lbxInteractionTerms.Items.Clear();
            if (lbxInteractionTerms.Items.Count > 0) { lbxInteractionTerms.ItemsSource = null; }

            //List<EwavColumn> SourceColumns = applicationViewModel.EwavSelectedDatasource.AllColumns;     // logRegressionviewModel.Columns;

            //cbxFieldOutcome.SelectedIndex = 0;



            //IEnumerable<EwavColumn> CBXFieldCols = from cols in SourceColumns
            //                                       where GetFieldDataType.Contains(cols.SqlDataTypeAsString)
            //                                       orderby cols.Name
            //                                       select cols;

            List<EwavColumn> colsList = cmnClass.GetItemsSource(GetFieldDataType);

            //colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            cbxFieldOutcome.ItemsSource = colsList;
            cbxFieldOutcome.SelectedValue = "Index";
            cbxFieldOutcome.DisplayMemberPath = "Name";
            cbxFieldOutcome.SelectedIndex = Index1;
            //============================================


            //CBXFieldCols = from cols in SourceColumns
            //               where GetWeightDataType.Contains(cols.SqlDataTypeAsString)
            //               orderby cols.Name
            //               select cols;

            colsList = cmnClass.GetItemsSource(GetWeightDataType);

            //colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            cbxFieldWeight.ItemsSource = colsList;
            cbxFieldWeight.SelectedValue = "Index";
            cbxFieldWeight.DisplayMemberPath = "Name";
            cbxFieldWeight.SelectedIndex = Index2;
            //============================================



            //CBXFieldCols = from cols in SourceColumns
            //               where GetFieldDataType.Contains(cols.SqlDataTypeAsString)
            //               orderby cols.Name
            //               select cols;

            colsList = cmnClass.GetItemsSource(GetFieldDataType);

            //colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            cbxFields.ItemsSource = colsList;
            cbxFields.SelectedValue = "Index";
            cbxFields.DisplayMemberPath = "Name";

            //LoadDropDownValues();
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
                cbxConf.SelectedIndex = Index3;
            }

            if (Index4 > -1)
            {
                cbxFields.SelectedIndex = Index4;
            }
        }

        void lrViewModel_RegressResultsLoadedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            LinearRegressionViewModel linearRegressionViewModel = (LinearRegressionViewModel)this.DataContext;
            LinRegressionResults regResults = linearRegressionViewModel.RegressionResults;
            Dictionary<string, string> inputVariableList = CreateInputVariableList();
            DoWork(regResults, inputVariableList);
        }

        #endregion

        public List<EwavDataFilterCondition> GadgetFilters { get; set; }

        #region Page / Controls Event Handlers

        void cbxField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        void cbxFields_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxFields.SelectedIndex > -1)
            {
                if (!lbxOtherFields.Items.Contains(((EwavColumn)cbxFields.SelectedItem).Name.ToString()))
                {
                    if (!(((EwavColumn)cbxFields.SelectedItem).Name == null) && (((EwavColumn)cbxFields.SelectedItem).Name.ToString().Trim().Length > 0))
                    {
                        this.otherVars.Add((EwavColumn)cbxFields.SelectedItem);
                        //this.otherVars.Add(((EwavColumn)cbxFields.SelectedItem).Name);
                        this.CreateListBoxWithCheckBox(this.lbxOtherFields, otherVars);
                    }
                }
            }
        }

        private void DoLinearRegression()
        {
            if (!LoadingDropDowns && !LoadingCanvas)
            {
                if (DataContext == null)
                {
                    return;
                }

                //SelectDropDownValues();

                List<string> columnNames = new List<string>();
                if (ValidateLists())
                {
                    IsUserDefindVariableInUse();
                    LinearRegressionViewModel linearRegressionViewModel = (LinearRegressionViewModel)this.DataContext;
                    Dictionary<string, string> inputVariableList = CreateInputVariableList();
                    if (!ErrorOnPage)
                    {
                        foreach (KeyValuePair<string, string> kvp in inputVariableList)
                        {
                            if (kvp.Value.ToLower().Equals("unsorted") || kvp.Value.ToLower().Equals("dependvar") || kvp.Value.ToLower().Equals("weightvar") || kvp.Value.ToLower().Equals("matchvar"))
                            {
                                columnNames.Add(kvp.Key);
                            }
                            else if (kvp.Value.ToLower().Equals("discrete"))
                            {
                                columnNames.Add(kvp.Key);
                            }
                        }
                        List<DictionaryDTO> inputDtoList;

                        inputDtoList = ConvertDictToList(CreateInputVariableList());

                        GadgetParameters gadgetOptions = new GadgetParameters();
                        //  gadgetOptions.TableName = "vwExternalData";
                        gadgetOptions.TableName = applicationViewModel.EwavSelectedDatasource.TableName;
                        gadgetOptions.DatasourceName = applicationViewModel.EwavSelectedDatasource.DatasourceName;
                        gadgetOptions.UseAdvancedDataFilter = applicationViewModel.UseAdvancedFilter;

                        gadgetOptions.GadgetFilters = GadgetFilters;
                        //  linearRegressionViewModel.GetRegressionResults("NEDS", "vwExternalData", gadgetOptions, columnNames, inputDtoList); 
                        linearRegressionViewModel.GetRegressionResults(gadgetOptions, columnNames, inputDtoList);

                        gadgetExpander.IsExpanded = false;
                        waitCursor.Visibility = System.Windows.Visibility.Visible;
                        ResetOtherVariableList();
                    }
                }
            }

        }

        private void IsUserDefindVariableInUse()
        {
            Col1 = (cbxFieldOutcome.SelectedIndex > -1) ? (EwavColumn)cbxFieldOutcome.SelectedItem : null;
            Col2 = (cbxFieldWeight.SelectedIndex > -1) ? (EwavColumn)cbxFieldWeight.SelectedItem : null;
            //EwavColumn Col3 = (EwavColumn)((CheckBox)((ListBoxItem)lbxDummyTerms.SelectedItems[0]).Content).Tag;// (cbxFields.SelectedIndex > -1) ? (EwavColumn)cbxFields.SelectedItem : null;
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

            IsUserDefinedVariableInUseByListBox(lbxDummyTerms);
            IsUserDefinedVariableInUseByListBox(lbxOtherFields);
            IsUserDefinedVariableInUseByListBox(lbxInteractionTerms);
        }

        private void IsUserDefinedVariableInUseByListBox(ListBox listBox)
        {
            List<ListBoxItem> Items = null;
            if (listBox.ItemsSource != null)
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

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                DoLinearRegression();
                //MessageBox.Show(Serialize(new XDocument()).ToString());
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

        private void btnRemoveDummy_Click(object sender, RoutedEventArgs e)
        {
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
                        }
                    }
                }

                this.CreateListBoxWithCheckBox(this.lbxDummyTerms, this.dummVars);
            }
        }

        private void btnMakeInteractionTerms_Click(object sender, System.Windows.RoutedEventArgs e)
        {
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

        private void btnRemoveInteraction_Click(object sender, RoutedEventArgs e)
        {
            List<ListBoxItem> listItems = this.lbxInteractionTerms.ItemsSource as List<ListBoxItem>;
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
                            this.interterms.Remove(selectedItem);
                        }
                    }
                }
                this.CreateListBoxWithCheckBox(this.lbxInteractionTerms, interterms);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            this.cbxFieldOutcome.SelectedIndex = -1;
            this.cbxFieldWeight.SelectedIndex = -1;
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
        }

        private void btnRemoveVariables_Click(object sender, RoutedEventArgs e)
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
                            this.otherVars.Remove(selectedItem);
                        }
                    }
                }

                this.CreateListBoxWithCheckBox(this.lbxOtherFields, this.otherVars);
            }
        }

        private void lbxOtherFields_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbxOtherFields.SelectedItems.Count == 1)
            {
                lbxOtherFields.Items.Remove(lbxOtherFields.SelectedItem);
            }
        }

        private void lbxDummyTerms_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbxDummyTerms.SelectedItems.Count == 1)
            {
                lbxDummyTerms.Items.Remove(lbxDummyTerms.SelectedItem);
                lbxOtherFields.Items.Add(lbxDummyTerms.SelectedItem);
            }
        }

        private void lbxInteractionTerms_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbxInteractionTerms.SelectedItems.Count == 1)
            {
                lbxInteractionTerms.Items.Remove(lbxInteractionTerms.SelectedItem);
            }
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

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        #endregion

        #region  Methods

        /// <summary>
        /// Associates the event handlers with Events. 
        /// </summary>
        private void Construct()
        {
            cbxFieldOutcome.SelectionChanged += new SelectionChangedEventHandler(cbxField_SelectionChanged);
            cbxFields.SelectionChanged += new SelectionChangedEventHandler(cbxFields_SelectionChanged);
            this.IsProcessing = false;

            pnlContent.Visibility = System.Windows.Visibility.Collapsed;
            this.gadgetExpander.IsExpanded = true;
        }

        /// <summary>
        /// Clears the results.
        /// </summary>
        private void ClearResults()
        {
            grdRegress.Children.Clear();
            grdRegress.RowDefinitions.Clear();

            grdParameters.Visibility = Visibility.Collapsed;
            txtCorrelation.Visibility = Visibility.Collapsed;

            txtStatus.Visibility = Visibility.Collapsed;
            txtStatus.Text = string.Empty;

            waitCursor.Visibility = Visibility.Visible;

            //btnRun.IsEnabled = false;
        }

        /// <summary>
        /// Main method that reads data from server and manipulates it on the client.
        /// </summary>
        /// <param name="regResults"></param>
        /// <param name="inputVariableList"></param>
        private void DoWork(LinRegressionResults regResults, Dictionary<string, string> inputVariableList = null)
        {
            //Configuration config = dashboardHelper.Config;
            Dictionary<string, string> setProperties = new Dictionary<string, string>();
            setProperties.Add("Intercept", "true");
            setProperties.Add("P", 0.95.ToString());
            //setProperties.Add("BLabels", config.Settings.RepresentationOfYes + ";" + config.Settings.RepresentationOfNo + ";" + config.Settings.RepresentationOfMissing); // TODO: Replace Yes, No, Missing with global vars
            setProperties.Add("BLabels", "YES" + ";" + "NO" + ";" + "Missing");

            SetGadgetToProcessingState();
            //this.Dispatcher.BeginInvoke(new SimpleCallback(ClearResults));
            ClearResults();

            try
            {
                LinRegressionResults results = new LinRegressionResults();
                results.CorrelationCoefficient = regResults.CorrelationCoefficient;
                results.RegressionDf = regResults.RegressionDf;
                results.RegressionF = regResults.RegressionF;
                results.RegressionMeanSquare = regResults.RegressionMeanSquare;
                results.RegressionSumOfSquares = regResults.RegressionSumOfSquares;
                results.ResidualsDf = regResults.ResidualsDf;
                results.ResidualsMeanSquare = regResults.ResidualsMeanSquare;
                results.ResidualsSumOfSquares = regResults.ResidualsSumOfSquares;
                results.TotalDf = regResults.TotalDf;
                results.TotalSumOfSquares = regResults.TotalSumOfSquares;
                results.ErrorMessage = regResults.ErrorMessage.Replace("<tlt>", string.Empty).Replace("</tlt>", string.Empty);
                results.Variables = new List<LinearRegVariableRow>();

                if (regResults.Variables != null)
                {
                    foreach (LinearRegVariableRow vrow in regResults.Variables)
                    {
                        LinearRegVariableRow nrow = new LinearRegVariableRow();
                        nrow.Coefficient = vrow.Coefficient;
                        nrow.Ftest = vrow.Ftest;
                        nrow.P = vrow.P;
                        nrow.StdError = vrow.StdError;
                        nrow.VariableName = vrow.VariableName;
                        results.Variables.Add(nrow);
                    }

                    if (results.Variables.Count > 0)
                    {
                        RenderRegressionHeader();
                    }
                    int rowCount = 1;
                    foreach (LinearRegVariableRow row in results.Variables)
                    {
                        AddGridRow(grdRegress, 30);
                        string displayValue = row.VariableName;

                        SetGridText(grdRegress, new TextBlockConfig(stringLiterals.SPACE + displayValue + stringLiterals.SPACE, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Left, rowCount, 0));
                        SetGridText(grdRegress, new TextBlockConfig(stringLiterals.SPACE + row.Coefficient.ToString("F3") + stringLiterals.SPACE, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 1));
                        SetGridText(grdRegress, new TextBlockConfig(stringLiterals.SPACE + row.StdError.ToString("F3") + stringLiterals.SPACE, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 2));
                        SetGridText(grdRegress, new TextBlockConfig(stringLiterals.SPACE + row.Ftest.ToString("F4") + stringLiterals.SPACE, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 3));
                        SetGridText(grdRegress, new TextBlockConfig(stringLiterals.SPACE + row.P.ToString("F6") + stringLiterals.SPACE, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, 4));

                        rowCount++;
                    }
                    DrawRegressionBorders();
                }
                RenderRegressionResults(results);
            }
            catch (Exception ex)
            {
                RenderFinishWithError(ex.Message);
            }
            finally
            {
            }
        }

        /// <summary>
        /// renders the table
        /// </summary>
        /// <param name="results"></param>
        private void RenderRegressionResults(LinRegressionResults results)
        {
            txtCorrelation.Visibility = Visibility.Visible;
            waitCursor.Visibility = Visibility.Collapsed;
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            grdParameters.Visibility = Visibility.Visible;
            //gadgetExpander.Visibility = System.Windows.Visibility.Collapsed;
            this.gadgetExpander.IsExpanded = false;
            btnRun.IsEnabled = true;

            if (!string.IsNullOrEmpty(results.ErrorMessage))
            {
                txtStatus.Text = results.ErrorMessage;
                //spContent.Visibility = System.Windows.Visibility.Visible;
                pnlStatus.Visibility = System.Windows.Visibility.Visible;
                txtStatus.Visibility = System.Windows.Visibility.Visible;
                grdParameters.Visibility = System.Windows.Visibility.Collapsed;
                pnlContent.Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (results.Variables == null)
            {
                txtCorrelation.Text = string.Empty;
                Thickness margin = txtCorrelation.Margin;
                margin.Top = 8;
                txtCorrelation.Margin = margin;
                //spContent.Visibility = System.Windows.Visibility.Visible;
                grdParameters.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                Thickness margin = txtCorrelation.Margin;
                margin.Top = 8;
                // spContent.Visibility = System.Windows.Visibility.Visible;
                pnlStatus.Visibility = System.Windows.Visibility.Collapsed;
                pnlContent.Visibility = System.Windows.Visibility.Visible;
                txtCorrelation.Margin = margin;
                grdParameters.Visibility = System.Windows.Visibility.Visible;

                txtRegressionDf.Text = stringLiterals.SPACE + results.RegressionDf.ToString() + stringLiterals.SPACE;
                txtRegressionSumOfSquares.Text = stringLiterals.SPACE + results.RegressionSumOfSquares.ToString("F4") + stringLiterals.SPACE;
                txtRegressionMeanSquare.Text = stringLiterals.SPACE + results.RegressionMeanSquare.ToString("F4") + stringLiterals.SPACE;
                txtRegressionFstatistic.Text = stringLiterals.SPACE + results.RegressionF.ToString("F4") + stringLiterals.SPACE;

                txtResidualsDf.Text = stringLiterals.SPACE + results.ResidualsDf.ToString() + stringLiterals.SPACE;
                txtResidualsSumOfSquares.Text = stringLiterals.SPACE + results.ResidualsSumOfSquares.ToString("F4") + stringLiterals.SPACE;
                txtResidualsMeanSquare.Text = stringLiterals.SPACE + results.ResidualsMeanSquare.ToString("F4") + stringLiterals.SPACE;

                txtTotalDf.Text = stringLiterals.SPACE + results.TotalDf.ToString() + stringLiterals.SPACE;
                txtTotalSumOfSquares.Text = stringLiterals.SPACE + results.TotalSumOfSquares.ToString("F4") + stringLiterals.SPACE;

                txtCorrelation.Text = "Correlation Coefficient: r^2 = " + results.CorrelationCoefficient.ToString("F2");
                 FilterButton.IsEnabled=true;
            }
        }

        /// <summary>
        /// renders the header
        /// </summary>
        private void RenderRegressionHeader()
        {
            RowDefinition rowDefHeader = new RowDefinition();
            rowDefHeader.Height = new GridLength(30);
            grdRegress.RowDefinitions.Add(rowDefHeader);
            grdRegress.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

            for (int y = 0; y < grdRegress.ColumnDefinitions.Count; y++)
            {
                Rectangle rctHeader = new Rectangle();
                rctHeader.Style = Application.Current.Resources["HeaderCell"] as Style;
                Grid.SetRow(rctHeader, 0);
                Grid.SetColumn(rctHeader, y);
                grdRegress.Children.Add(rctHeader);
            }

            TextBlock txtVarHeader = new TextBlock();
            txtVarHeader.Text = "Variable";
            txtVarHeader.Style = Application.Current.Resources["HeaderFont"] as Style; //Brushes.White;
            Grid.SetRow(txtVarHeader, 0);
            Grid.SetColumn(txtVarHeader, 0);
            grdRegress.Children.Add(txtVarHeader);

            TextBlock txtCoefHeader = new TextBlock();
            txtCoefHeader.Text = "Coefficient";
            txtCoefHeader.Style = Application.Current.Resources["HeaderFont"] as Style; //Brushes.White;
            Grid.SetRow(txtCoefHeader, 0);
            Grid.SetColumn(txtCoefHeader, 1);
            grdRegress.Children.Add(txtCoefHeader);

            TextBlock txtStdErrorHeader = new TextBlock();
            txtStdErrorHeader.Text = "Std Error";
            txtStdErrorHeader.Style = Application.Current.Resources["HeaderFont"] as Style; //Brushes.White;
            Grid.SetRow(txtStdErrorHeader, 0);
            Grid.SetColumn(txtStdErrorHeader, 2);
            grdRegress.Children.Add(txtStdErrorHeader);

            TextBlock txtFHeader = new TextBlock();
            txtFHeader.Text = "F-test";
            txtFHeader.Style = Application.Current.Resources["HeaderFont"] as Style; //Brushes.White;
            Grid.SetRow(txtFHeader, 0);
            Grid.SetColumn(txtFHeader, 3);
            grdRegress.Children.Add(txtFHeader);

            TextBlock txtPHeader = new TextBlock();
            txtPHeader.Text = "P-value";
            txtPHeader.Style = Application.Current.Resources["HeaderFont"] as Style; //Brushes.White;
            Grid.SetRow(txtPHeader, 0);
            Grid.SetColumn(txtPHeader, 4);
            grdRegress.Children.Add(txtPHeader);
        }

        /// <summary>
        /// Used to push a status message to the gadget's status panel
        /// </summary>
        /// <param name="statusMessage">The status message to display</param>
        private void RequestUpdateStatusMessage(string statusMessage)
        {
            //this.Dispatcher.BeginInvoke(new SetStatusDelegate(SetStatusMessage), statusMessage);
            SetStatusMessage(statusMessage);
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
            txtStatus.Visibility = System.Windows.Visibility.Visible;
            txtStatus.Text = errorMessage;
            txtStatus.Visibility = System.Windows.Visibility.Visible;
            pnlStatus.Visibility = System.Windows.Visibility.Visible;
            pnlContent.Visibility = System.Windows.Visibility.Collapsed;
            //CollapseConfigPanel();
        }

        /// <summary>
        /// Used to sets the gadget's current status, e.g. "Processing results..." or "Displaying output..."
        /// </summary>
        /// <param name="statusMessage">The status message to display</param>
        private void SetStatusMessage(string statusMessage)
        {
            txtStatus.Visibility = System.Windows.Visibility.Visible;
            txtStatus.Text = statusMessage;
        }

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
        /// method that resets the variables and controls on the screen.
        /// </summary>
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

        /// <summary>
        /// Method that draws the borders for regression table.
        /// </summary>
        private void DrawRegressionBorders()
        {
            waitCursor.Visibility = Visibility.Collapsed;
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
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

        /// <summary>
        /// Method that sets the Text in Grid
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="textBlockConfig"></param>
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

        /// <summary>
        /// Method that adds a row in Grid
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="height"></param>
        private void AddGridRow(Grid grid, int height)
        {
            waitCursor.Visibility = Visibility.Collapsed;
            //grdCoef.Visibility = Visibility.Visible;
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            grdRegress.Visibility = Visibility.Visible;
            //txtCoefficient.Visibility = Visibility.Visible;
            RowDefinition rowDef = new RowDefinition();
            rowDef.Height = new GridLength(height);
            grid.RowDefinitions.Add(rowDef);
        }

        /// <summary>
        /// Helper method that is used to get the Parent Object as a FrameElement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Method that intializes the InputVariable list.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> CreateInputVariableList()
        {
            Dictionary<string, string> inputVariableList = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            gadgetOptions.MainVariableName = string.Empty;
            gadgetOptions.WeightVariableName = string.Empty;
            gadgetOptions.StrataVariableNames = new List<string>();

            gadgetOptions.CrosstabVariableName = string.Empty;
            gadgetOptions.InputVariableList = new Dictionary<string, string>();

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

                double p = 0.95;
                if (cbxConf.SelectedIndex > -1 && !string.IsNullOrEmpty(cbxConf.SelectedItem.ToString()))
                {
                    bool success = Double.TryParse(cbxConf.SelectedItem.ToString().Replace("%", string.Empty), out p);
                    if (!success)
                    {
                        p = 0.95;
                    }
                }
                inputVariableList.Add("p", p.ToString());

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
                ErrorOnPage = true;
                spContent.Visibility = System.Windows.Visibility.Visible;
                ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
                pnlStatus.Visibility = System.Windows.Visibility.Visible;
                txtStatus.Text = "The same variable cannot be used more than once.";
                txtStatus.Visibility = System.Windows.Visibility.Visible;
            }

            gadgetOptions.InputVariableList = inputVariableList;
            return inputVariableList;
        }

        #endregion

        #region IGadget Members

        public event GadgetClosingHandler GadgetClosing;
        public event GadgetProcessingFinishedHandler GadgetProcessingFinished;

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
            {
                GadgetProcessingFinished(this);
            }
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

        public StringBuilder HtmlBuilder { get; set; }

        public void RefreshResults()
        {
        }

        public void UpdateVariableNames()
        {
        }

        private string customOutputHeading;
        private string customOutputDescription;
        private string customOutputCaption;

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
                htmlBuilder.AppendLine("<h2 class=\"gadgetHeading\">Linear  Regression</h2>");
            }
            else if (this.CustomOutputHeading != "(none)")
            {
                htmlBuilder.AppendLine(string.Format("<h2 class=\"gadgetHeading\">{0}</h2>", this.CustomOutputHeading));
            }

            htmlBuilder.AppendLine("<p class=\"gadgetOptions\"><small>");
            htmlBuilder.AppendLine(string.Format("<em>Frequency variable:</em> <strong>{0}</strong>",  "((EwavColumn)this.cbxField.SelectedItem).Name)"));
            htmlBuilder.AppendLine("<br />");

            htmlBuilder.AppendLine("<div style=\"height: 7px;\"></div>");
            htmlBuilder.AppendLine(string.Format("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" summary=\"{0}\">", "summaryText"));


            htmlBuilder.AppendLine(string.Format("<\table >"));



            HtmlBuilder = htmlBuilder;

            //HtmlBuilder.Append("<h2>LinearRegression HTML Not Implemented</h2>");
           
            return "";    


        }

        /// <summary>
        /// Generates Xml representation of this gadget
        /// </summary>
        /// <param name="doc">The Xml docment</param>
        /// <returns>XmlNode</returns>
        public XNode Serialize(XDocument doc)
        {
            string dependVar = string.Empty;
            string weightVar = string.Empty;
            string pvalue = string.Empty;
            bool intercept = true;

            if (cbxFieldOutcome.SelectedItem != null)
            {
                dependVar = ((EwavColumn)cbxFieldOutcome.SelectedItem).Name.ToString();
            }

            if (cbxFieldWeight.SelectedItem != null)
            {
                weightVar = ((EwavColumn)cbxFieldWeight.SelectedItem).Name.ToString();
            }

            if (checkboxNoIntercept.IsChecked == true)
            {
                intercept = false;
            }
            else
            {
                intercept = true;
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
                new XAttribute("gadgetType", "Ewav.LinearRegression"),
                new XElement("mainVariable", dependVar),
                new XElement("weightVariable", weightVar),
                new XElement("pvalue", pvalue),
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

                //foreach (string s in lbxOtherFields.Items)
                //{
                //    //xmlCovariateString = xmlCovariateString + "<covariate>" + s + "</covariate>";
                //    ;
                //}

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

        ///// <summary>
        ///// Creates the gadget from an Xml element
        ///// </summary>
        ///// <param name="element">The element from which to create the gadget</param>
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
        #endregion

        public void CreateFromXml(XElement element)
        {
            LoadingCanvas = true;
            //InitializeControl();

            List<EwavColumn> primaryColList = cmnClass.GetItemsSource(GetFieldDataType);
            List<EwavColumn> weightColList = cmnClass.GetItemsSource(GetWeightDataType);

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
            //this.loadingCombos = false;
            DoLinearRegression();
            //RefreshResults();
            //CollapseExpandConfigPanel();

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

            if (this.DataContext != null)
            {
                DoLinearRegression();
                //MessageBox.Show(Serialize(new XDocument()).ToString());
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
                DoLinearRegression();
            }
        }





    }

}
namespace Ewav.Web.Services
{
    public partial class LinearRegressionDomainContext
    {
        //This is the place to set RIA Services query timeout. 
        //TimeSpan(0, 5, 0) means five minutes vs the 
        //default 60 sec
        partial void OnCreated()
        {
            if (!DesignerProperties.GetIsInDesignMode(Application.
                Current.RootVisual))
                ((WebDomainClient<ILinearRegressionDomainServiceContract>)
                    DomainClient).ChannelFactory.Endpoint.Binding.SendTimeout =
                    new TimeSpan(0, 120, 0);
        }
    }
}