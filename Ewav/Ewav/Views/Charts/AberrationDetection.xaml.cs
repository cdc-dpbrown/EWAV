/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       AberrationDetection.xaml.cs
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
using System.Windows.Shapes;
using System.Xml.Linq;
using CommonLibrary;
using ComponentArt.Silverlight.DataVisualization.Charting;
using ComponentArt.Silverlight.DataVisualization.Common;
using Ewav.Web.EpiDashboard;
using Ewav.BAL;
using Ewav.ViewModels;
using Ewav.ExtensionMethods;
using Ewav.Web.Services;
using ComponentArt.Silverlight.DataVisualization;
using Ewav.Common;

namespace Ewav
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>    
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(UserControl))]
    [ExportMetadata("type", "gadget")]
    [ExportMetadata("tabindex", "1")]
    public partial class AberrationControl : UserControl, IGadget, IEwavGadget
    {
        public class SimpleDataValue
        {
            public double DependentValue { get; set; }
            public DateTime IndependentValue { get; set; }
        }

        public class DataGridRow
        {
            public DateTime Date { get; set; }
            public double Frequency { get; set; }
            public double RunningAverage { get; set; }
            public double StandardDeviation { get; set; }
            public double Delta
            {
                get
                {
                    return (this.Frequency - this.RunningAverage) / this.StandardDeviation;
                }
            }
        }

        #region Private Variables

        private bool loadingCombos;
        private bool triangleCollapsed;
        private GadgetParameters gadgetOptions;
        private object syncLock = new object();
        private const int DEFAULT_DEVIATIONS = 3;
        private const int DEFAULT_LAG_TIME = 7;

        ClientCommon.Common cmnClass = new ClientCommon.Common();

        #endregion

        AberrationViewModel aberrationViewModel;
        int Index1 = -1, Index2 = -1, Index3 = -1;
        EwavColumn Col1, Col2;
        //List<EwavColumn> SelectedColsCollection;
        XYChart chart = null;
        public List<EwavDataFilterCondition> GadgetFilters { get; set; }
        #region Delegates

        private delegate void SetGraphDelegate(string strataValue, List<SimpleDataValue> actualValues, List<SimpleDataValue> trendValues, List<SimpleDataValue> aberrationValues, List<DataGridRow> aberrationDetails);
        private delegate void SimpleCallback();
        private delegate void SetStatusDelegate(string statusMessage);
        private delegate void RequestUpdateStatusDelegate(string statusMessage);
        private delegate bool CheckForCancellationDelegate();

        private delegate void RenderFinishWithErrorDelegate(string errorMessage);
        private delegate void RenderFinishWithWarningDelegate(string errorMessage);

        #endregion

        public event GadgetClosingHandler GadgetClosing;
        public event GadgetProcessingFinishedHandler GadgetProcessingFinished;
        public event GadgetStatusUpdateHandler GadgetStatusUpdate;
        public event GadgetCheckForCancellationHandler GadgetCheckForCancellation;
        ClientCommon.Common CommonClass = new ClientCommon.Common();
        ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;
        #region Constructors

        /// <summary>
        /// Loaded Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AberrationControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeControl();
        }

        private void InitializeControl()
        {
            try
            {
                List<DTO.EwavFrequencyControlDto> frequencyDTO = new List<DTO.EwavFrequencyControlDto>();

                applicationViewModel.ApplyDataFilterEvent += new Client.Application.ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);
                this.aberrationViewModel = (AberrationViewModel)this.DataContext;
                //     aberrationViewModel.ColumnsLoadedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(aberrationViewModel_ColumnsLoadedEvent);
                this.aberrationViewModel.FrequencyTableLoadedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(aberrationViewModel_FrequencyTableLoadedEvent);
                this.aberrationViewModel.ErrorNotice += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(aberrationViewModel_ErrorNotice);
                //    aberrationViewModel.GetColumns("NEDS", "vwExternalData");    
                applicationViewModel.DefinedVariableAddedEvent += new Client.Application.DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);
                //    lrViewModel.ColumnsLoadedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(lrViewModel_ColumnsLoadedEvent);
                applicationViewModel.DefinedVariableInUseDeletedEvent += new Client.Application.DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableInUseDeletedEvent);
                applicationViewModel.DefinedVariableNotInUseDeletedEvent += new Client.Application.DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
                applicationViewModel.PreColumnChangedEvent += new Client.Application.PreColumnChangedEventHandler(applicationViewModel_PreColumnChangedEvent);
                applicationViewModel.UnloadedEvent += new Client.Application.UnloadedEventHandler(applicationViewModel_UnloadedEvent);
            }
            catch (Exception)
            {
                throw;
            }
            this.Construct();
        }

        void applicationViewModel_UnloadedEvent(object o)
        {
            UnloadGadget();
        }

        private void UnloadGadget()
        {
            applicationViewModel.ApplyDataFilterEvent -= new Client.Application.ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);
            applicationViewModel.DefinedVariableAddedEvent -= new Client.Application.DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);
            //    lrViewModel.ColumnsLoadedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(lrViewModel_ColumnsLoadedEvent);
            applicationViewModel.DefinedVariableInUseDeletedEvent -= new Client.Application.DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableInUseDeletedEvent);
            applicationViewModel.DefinedVariableNotInUseDeletedEvent -= new Client.Application.DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
            applicationViewModel.PreColumnChangedEvent -= new Client.Application.PreColumnChangedEventHandler(applicationViewModel_PreColumnChangedEvent);
        }

        ///// <summary>
        ///// Method used to locate the current index for selected column.
        ///// </summary>
        ///// <param name="Column"></param>
        ///// <returns></returns>
        //private int SearchCurrentIndex(EwavColumn Column)
        //{
        //    if (Column != null)
        //    {
        //        List<EwavColumn> SourceColumns = this.applicationViewModel.EwavSelectedDatasource.AllColumns;

        //        IEnumerable<EwavColumn> CBXFieldCols = from cols in SourceColumns
        //                                               orderby cols.Name
        //                                               select cols;

        //        List<EwavColumn> colsList = CBXFieldCols.ToList();

        //        for (int rule = 0; rule < colsList.Count; rule++)
        //        {
        //            if (Column.Name == colsList[rule].Name)
        //            {
        //                return rule + 1;
        //            }
        //        }
        //    }

        //    return 0;
        //}

        ///// <summary>
        ///// Sets the IsInUse indicator for the Column.
        ///// </summary>
        private void IsUserDefindVariableInUse()
        {
            Col1 = (cbxSyndrome.SelectedIndex > -1) ? (EwavColumn)cbxSyndrome.SelectedItem : null;
            Col2 = (cbxFieldWeight.SelectedIndex > -1) ? (EwavColumn)cbxFieldWeight.SelectedItem : null;
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

        /// <summary>
        /// Close button click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.CloseGadgetOnClick();
        }

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void LayoutRoot_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClientCommon.Common cmnClass = new ClientCommon.Common();
            Point p = e.GetSafePosition(cmnClass.GetParentObject<Grid>(this, "LayoutRoot"));
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).GadgetNameOnRightClick = MyControlName;
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).StrataList = null;
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).SelectedGadget = this;
            cmnClass.UpdateZOrder(this, true, cmnClass.GetParentObject<Grid>(this, "LayoutRoot"));
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Base constructor
        /// </summary>
        public AberrationControl()
        {
            this.InitializeComponent();

            this.gadgetOptions = new GadgetParameters();
            this.gadgetOptions.ShouldIncludeFullSummaryStatistics = false;
            this.gadgetOptions.ShouldIncludeMissing = false;
            this.gadgetOptions.ShouldSortHighToLow = false;
            this.gadgetOptions.ShouldUseAllPossibleValues = false;
            this.gadgetOptions.StrataVariableNames = new List<string>();

            this.Loaded += new RoutedEventHandler(AberrationControl_Loaded);
            //InitializeControl();

            this.FillDropDowns();
        }

        /// <summary>
        /// Event hanlder that listens to Error on the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void aberrationViewModel_ErrorNotice(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            //if (e.Data == null || e.Data.Message.Length > 0)
            //{
            //    ChildWindow window = new ErrorWindow(e.Data);
            //    window.Show();
            //    //return;
            //}

            RenderFinishWithError(e.Data.Message);
            this.SetGadgetToFinishedState();
        }

        /// <summary>
        /// Event hanlder that listens to DataFilter when applied
        /// </summary>
        /// <param name="o"></param>
        void applicationViewModel_ApplyDataFilterEvent(object o)
        {
            //if ((applicationViewModel.ItemToBeRemoved != null) &&
            //    DFInUse != null &&
            //    (applicationViewModel.ItemToBeRemoved.Name == DFInUse.Name))
            if (applicationViewModel.RemoveIndicator && IsDFUsedInThisGadget())
            {
                ResetGadget();
            }
            else
            {
                DoAD();
            }
        }

        /// <summary>
        /// Event hanlder when Frequency table loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void aberrationViewModel_FrequencyTableLoadedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            //waitCursor.Visibility = System.Windows.Visibility.Collapsed;
            List<FrequencyResultData> notConvertedTable = this.aberrationViewModel.FrequencyTable;
            Dictionary<List<DTO.EwavFrequencyControlDto>, List<DescriptiveStatistics>> Data = new Dictionary<List<DTO.EwavFrequencyControlDto>, List<DescriptiveStatistics>>();
            for (int i = 0; i < notConvertedTable.Count; i++)
            {
                Data.Add(notConvertedTable[i].FrequencyControlDtoList, notConvertedTable[i].DescriptiveStatisticsList);
            }

            this.DoWork(Data, gadgetOptions);
            this.waitCursor.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Pre Column Changed Event.
        /// </summary>
        /// <param name="o"></param>
        void applicationViewModel_PreColumnChangedEvent(object o)
        {
            SaveColumnValues();
        }

        /// <summary>
        /// Defined variable not in Use Deleted Event hanlder.
        /// </summary>
        /// <param name="o"></param>
        void applicationViewModel_DefinedVariableNotInUseDeletedEvent(object o)
        {
            SearchIndex();
            LoadingDropDowns = true;
            FillDropDowns();
            LoadingDropDowns = false;
            DoAD();
        }

        /// <summary>
        /// Defined Variable in use deleted event hanlder.
        /// </summary>
        /// <param name="o"></param>
        void applicationViewModel_DefinedVariableInUseDeletedEvent(object o)
        {
            ResetGadget();
        }

        private void ResetGadget()
        {
            SearchIndex();
            if (IsDFUsedInThisGadget())
            {
                Index1 = Index2 = Index3 = -1;
                pnlCharts.Visibility = System.Windows.Visibility.Collapsed;
                waitCursor.Visibility = System.Windows.Visibility.Collapsed;
            }
            LoadingDropDowns = true;
            FillDropDowns();
            LoadingDropDowns = false;
        }

        private bool IsDFUsedInThisGadget()
        {
            return Col1 != null && Col1.Name == applicationViewModel.ItemToBeRemoved.Name ||
                            Col2 != null && Col2.Name == applicationViewModel.ItemToBeRemoved.Name;
        }

        /// <summary>
        /// Defined Variable Added event hanlder.
        /// </summary>
        /// <param name="o"></param>
        void applicationViewModel_DefinedVariableAddedEvent(object o)
        {
            SearchIndex();
            LoadingDropDowns = true;
            FillDropDowns();
            LoadingDropDowns = false;
            DoAD();
        }

        /// <summary>
        /// Run button event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            SaveColumnValues();
            DoAD();
        }

        private void txtLagTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.VerifyNumber(((TextBox)sender).Text))
            {
                this.txtLagTime.Text = "2";
                return;
            }
            if (!string.IsNullOrEmpty(this.txtLagTime.Text))
            {
                int lagTime = 7;

                int.TryParse(this.txtLagTime.Text, out lagTime);

                if (lagTime > 365)
                {
                    this.txtLagTime.Text = "365";
                }
                else if (lagTime <= 2)
                {
                    this.txtLagTime.Text = "2";
                }
            }
        }

        private void txtDeviations_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.VerifyNumber(((TextBox)sender).Text))
            {
                this.txtDeviations.Text = "1";
                return;
            }

            if (!string.IsNullOrEmpty(this.txtDeviations.Text))
            {
                int dev = 0;

                int.TryParse(this.txtDeviations.Text, out dev);

                if (dev > 7)
                {
                    this.txtDeviations.Text = "7";
                }
                else if (dev <= 1)
                {
                    this.txtDeviations.Text = "1";
                }
            }
        }

        void cbxField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //RefreshResults();
            //aberrationViewModel = (AberrationViewModel)this.DataContext;
            //aberrationViewModel.GetFrequencyData("NEDS", "vwExternalData", gadgetOptions);
            if (cbxDate.SelectedIndex > 0)
            {
                Index1 = cbxDate.SelectedIndex;
            }
            this.EnableDisableButton();
        }

        private void DoWork(Dictionary<List<DTO.EwavFrequencyControlDto>, List<DescriptiveStatistics>> Data, GadgetParameters gadgetOptions)
        {
            Dictionary<string, string> inputVariableList = gadgetOptions.InputVariableList;

            SetGraphDelegate setGraph = new SetGraphDelegate(SetGraph);

            string freqVar = string.Empty;
            string weightVar = string.Empty;
            string strataVar = string.Empty;
            bool includeMissing = false;
            int lagTime = 7;
            double deviations = 3;

            if (inputVariableList.ContainsKey("freqvar"))
            {
                freqVar = inputVariableList["freqvar"];
            }

            if (inputVariableList.ContainsKey("weightvar"))
            {
                weightVar = inputVariableList["weightvar"];
            }

            if (inputVariableList.ContainsKey("stratavar"))
            {
                strataVar = inputVariableList["stratavar"];
            }

            if (inputVariableList.ContainsKey("lagtime"))
            {
                int.TryParse(inputVariableList["lagtime"], out lagTime);
            }
            if (inputVariableList.ContainsKey("deviations"))
            {
                double.TryParse(inputVariableList["deviations"], out deviations);
            }

            if (inputVariableList.ContainsKey("includemissing"))
            {
                if (inputVariableList["includemissing"].Equals("true"))
                {
                    includeMissing = true;
                }
            }

            deviations = deviations - 0.001;

            List<string> stratas = new List<string>();
            if (!string.IsNullOrEmpty(strataVar))
            {
                stratas.Add(strataVar);
            }

            try
            {
                RequestUpdateStatusDelegate requestUpdateStatus = new RequestUpdateStatusDelegate(RequestUpdateStatusMessage);
                CheckForCancellationDelegate checkForCancellation = new CheckForCancellationDelegate(IsCancelled);

                Dictionary<List<DTO.EwavFrequencyControlDto>, List<DescriptiveStatistics>> stratifiedFrequencyTables = Data; //dashboardHelper.GenerateFrequencyTable(gadgetOptions/*, freqVar, weightVar, stratas, string.Empty, useAllPossibleValues, sortHighLow, includeMissing*/);
                if (stratifiedFrequencyTables == null || stratifiedFrequencyTables.Count == 0)
                {
                    this.ShowError();
                }
                else
                {
                    string formatString = string.Empty;

                    foreach (KeyValuePair<List<DTO.EwavFrequencyControlDto>, List<DescriptiveStatistics>> tableKvp in stratifiedFrequencyTables)
                    {
                        string strataValue = "";

                        double count = 0;
                        foreach (DescriptiveStatistics ds in tableKvp.Value)
                        {
                            count = count + ds.Observations;
                        }

                        if (count == 0)
                        {
                            //RenderFinishWithError(SharedStrings.NO_RECORDS_SELECTED);
                            //return;
                            continue;
                        }

                        strataValue = tableKvp.Key[0].NameOfDtoList;

                        List<DTO.EwavFrequencyControlDto> frequencies = tableKvp.Key;

                        if (frequencies == null)
                        {
                            continue;
                        }
                    }

                    if (this.GadgetStatusUpdate != null)
                    {
                        //this.Dispatcher.BeginInvoke(GadgetStatusUpdate, "Displaying output...");
                        this.GadgetStatusUpdate("Displaying output...");
                    }

                    foreach (KeyValuePair<List<DTO.EwavFrequencyControlDto>, List<DescriptiveStatistics>> tableKvp in stratifiedFrequencyTables)
                    {
                        string strataValue = "";

                        double count = 0;
                        foreach (DescriptiveStatistics ds in tableKvp.Value)
                        {
                            count = count + ds.Observations;
                        }

                        if (count == 0)
                        {
                            //RenderFinishWithError(SharedStrings.NO_RECORDS_SELECTED);
                            //return;\
                            continue;
                        }

                        strataValue = tableKvp.Key[0].NameOfDtoList;

                        List<DTO.EwavFrequencyControlDto> frequencies = tableKvp.Key;

                        if (frequencies == null)
                        {
                            continue;
                        }

                        string tableHeading = tableKvp.Key[0].NameOfDtoList;

                        if (stratifiedFrequencyTables.Count > 1)
                        {
                            tableHeading = freqVar;
                        }

                        double lastAvg = double.NegativeInfinity;
                        double lastStdDev = double.NegativeInfinity;
                        Queue<double> frame = new Queue<double>();
                        List<SimpleDataValue> actualValues = new List<SimpleDataValue>();
                        List<SimpleDataValue> trendValues = new List<SimpleDataValue>();
                        List<SimpleDataValue> aberrationValues = new List<SimpleDataValue>();
                        List<DataGridRow> aberrationDetails = new List<DataGridRow>();
                        int rowCount = 1;
                        foreach (DTO.EwavFrequencyControlDto row in frequencies)
                        {
                            if (!row.FreqVariable.Equals(DBNull.Value) || (row.FreqVariable.Equals(DBNull.Value) && includeMissing == true))
                            {
                                DateTime displayValue = DateTime.Parse(row.FreqVariable.ToString());

                                //frame.Enqueue((double)row["freq"]);
                                frame.Enqueue(Convert.ToDouble(row.FrequencyColumn));
                                SimpleDataValue actualValue = new SimpleDataValue();
                                actualValue.IndependentValue = displayValue;
                                //actualValue.DependentValue = (double)row["freq"]; Convert.ToDouble(row.FrequencyColumn)
                                actualValue.DependentValue = Convert.ToDouble(row.FrequencyColumn);
                                actualValues.Add(actualValue);
                                if (frame.Count > lagTime - 1 /*6*/)
                                {
                                    double[] frameArray = frame.ToArray();
                                    double frameAvg = frameArray.Average();
                                    frame.Dequeue();
                                    double stdDev = this.CalculateStdDev(frameArray);
                                    if (lastAvg != double.NegativeInfinity)
                                    {
                                        SimpleDataValue trendValue = new SimpleDataValue();
                                        trendValue.IndependentValue = displayValue;
                                        trendValue.DependentValue = lastAvg;
                                        trendValues.Add(trendValue);
                                        //if ((double)row["freq"] > lastAvg + (/*2.99*/deviations * lastStdDev))
                                        if (Convert.ToDouble(row.FrequencyColumn) > lastAvg + (/*2.99*/deviations * lastStdDev))
                                        {
                                            SimpleDataValue aberrationValue = new SimpleDataValue();
                                            aberrationValue.IndependentValue = displayValue;
                                            //aberrationValue.DependentValue = (double)row["freq"];
                                            aberrationValue.DependentValue = Convert.ToDouble(row.FrequencyColumn);
                                            aberrationValues.Add(aberrationValue);
                                            DataGridRow aberrationDetail = new DataGridRow()
                                            {
                                                Date = displayValue,
                                                //Frequency = (double)row["freq"],
                                                Frequency = Convert.ToDouble(row.FrequencyColumn),
                                                RunningAverage = lastAvg,
                                                StandardDeviation = lastStdDev
                                            };
                                            aberrationDetails.Add(aberrationDetail);
                                        }
                                    }
                                    lastAvg = frameAvg;
                                    lastStdDev = stdDev;
                                }

                                rowCount++;
                            }
                        }

                        //this.Dispatcher.BeginInvoke(setGraph, strataValue, actualValues, trendValues, aberrationValues, aberrationDetails);
                        this.SetGraph(strataValue, actualValues, trendValues, aberrationValues, aberrationDetails);
                        //System.Threading.Thread.Sleep(1000);
                    }
                }
                this.RenderFinish();
                //this.Dispatcher.BeginInvoke(new SimpleCallback(RenderFinish));
                this.SetGadgetToFinishedState();
                //this.Dispatcher.BeginInvoke(new SimpleCallback(SetGadgetToFinishedState));
            }
            catch (Exception ex)
            {
                this.RenderFinishWithError(ex.Message);
                //this.Dispatcher.BeginInvoke(new RenderFinishWithErrorDelegate(RenderFinishWithError), ex.Message);
                this.SetGadgetToFinishedState();
                //this.Dispatcher.BeginInvoke(new SimpleCallback(SetGadgetToFinishedState));
            }
        }

        #endregion

        #region Private Methods

        ///// <summary>
        ///// Hides Main Panel
        ///// </summary>
        //private void HideMainPanel()
        //{

        //}

        /// <summary>
        /// Saves the Values of Columns.
        /// </summary>
        private void SaveColumnValues()
        {
            List<ColumnDataType> columnDataType = new List<ColumnDataType>();

            Col1 = (EwavColumn)cbxSyndrome.SelectedItem;
            Col2 = (EwavColumn)cbxFieldWeight.SelectedItem;
            //Col1ItemSource = (List<EwavColumn>)cbxSyndrome.ItemsSource;
            //columnDataType.Clear();
            //columnDataType.Add(ColumnDataType.Boolean);
            //columnDataType.Add(ColumnDataType.Numeric);
            //columnDataType.Add(ColumnDataType.Text);
            //Col1ItemSource = GetItemsSource(columnDataType);
            //columnDataType.Clear();
            //columnDataType.Add(ColumnDataType.Numeric);
            //Col2ItemSource = GetItemsSource(columnDataType);
            //SelectedColsCollection = new List<EwavColumn>();
            //SelectedColsCollection.Add(Col1);
            //SelectedColsCollection.Add(Col2);
            //Col3 = (EwavColumn)cbxDate.SelectedItem;
            //Col4 = (EwavColumn)cbxFields.SelectedItem;
        }

        /// <summary>
        /// Searches current index of the columns.
        /// </summary>
        private void SearchIndex()
        {
            //List<ColumnDataType> columnDataType = new List<ColumnDataType>();
            //columnDataType.Clear();
            //columnDataType.Add(ColumnDataType.Boolean);
            //columnDataType.Add(ColumnDataType.Numeric);
            //columnDataType.Add(ColumnDataType.Text);

            //Col1ItemSource = ;

            Index1 = cbxDate.SelectedIndex;
            Index2 = CommonClass.SearchCurrentIndex(Col1, CommonClass.GetItemsSource(GetFieldSyndromeDataType));

            //columnDataType.Clear();
            //columnDataType.Add(ColumnDataType.Numeric);

            //Col2ItemSource = ;

            Index3 = CommonClass.SearchCurrentIndex(Col2, CommonClass.GetItemsSource(GetFieldWeightDataType));
            //Index4 = SearchCurrentIndex(Col4);
        }

        public List<ColumnDataType> GetFieldDateDataType
        {
            get
            {
                List<ColumnDataType> columnDataType = new List<ColumnDataType>();
                columnDataType.Add(ColumnDataType.DateTime);
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

        public List<ColumnDataType> GetFieldSyndromeDataType
        {
            get
            {
                List<ColumnDataType> columnDataType = new List<ColumnDataType>();
                columnDataType.Clear();
                columnDataType.Add(ColumnDataType.Boolean);
                columnDataType.Add(ColumnDataType.Numeric);
                columnDataType.Add(ColumnDataType.Text);
                return columnDataType;
            }
        }

        /// <summary>
        /// Fills Combo boxes
        /// </summary>
        private void FillDropDowns()     //   object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            //    AberrationViewModel aberrationViewModel = (AberrationViewModel)sender;
            //List<EwavColumn> SourceColumns = this.applicationViewModel.EwavSelectedDatasource.AllColumns;        //   aberrationViewModel.Columns;
            this.cbxDate.ItemsSource = null;
            this.cbxDate.Items.Clear();

            this.cbxFieldWeight.ItemsSource = null;
            this.cbxFieldWeight.Items.Clear();

            this.cbxSyndrome.ItemsSource = null;
            this.cbxSyndrome.Items.Clear();

            List<string> fieldNames = new List<string>();
            List<string> weightFieldNames = new List<string>();
            List<string> strataFieldNames = new List<string>();

            weightFieldNames.Add(string.Empty);
            strataFieldNames.Add(string.Empty);

            //ColumnDataType columnDataType = ColumnDataType.DateTime;
            //fieldNames = dashboardHelper.GetFieldsAsList(columnDataType);

            //IEnumerable<EwavColumn> CBXFieldCols = from cols in SourceColumns
            //                                       where columnDataType.Contains(cols.SqlDataTypeAsString)
            //                                       orderby cols.Name
            //                                       select cols;

            //List<EwavColumn> colsList = CBXFieldCols.ToList();

            //colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            this.cbxDate.ItemsSource = CommonClass.GetItemsSource(GetFieldDateDataType);
            this.cbxDate.SelectedValue = "Index";
            this.cbxDate.DisplayMemberPath = "Name";
            this.cbxDate.SelectedIndex = Index1;

            //CBXFieldCols = from cols in SourceColumns
            //               where columnDataType.Contains(cols.SqlDataTypeAsString)
            //               orderby cols.Name
            //               select cols;

            //colsList = CBXFieldCols.ToList();

            //colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            this.cbxFieldWeight.ItemsSource = CommonClass.GetItemsSource(GetFieldWeightDataType);
            this.cbxFieldWeight.SelectedValue = "Index";
            this.cbxFieldWeight.DisplayMemberPath = "Name";
            cbxFieldWeight.SelectedIndex = Index3;

            //columnDataType = ColumnDataType.Numeric;
            //weightFieldNames.AddRange(dashboardHelper.GetFieldsAsList(columnDataType));

            //columnDataType = ColumnDataType.Numeric | ColumnDataType.Boolean | ColumnDataType.Text;
            //strataFieldNames.AddRange(dashboardHelper.GetFieldsAsList(columnDataType));

            //this.cbxSyndrome.ItemsSource = colsList;
            this.cbxSyndrome.ItemsSource = CommonClass.GetItemsSource(GetFieldSyndromeDataType);
            this.cbxSyndrome.SelectedValue = "Index";
            this.cbxSyndrome.DisplayMemberPath = "Name";
            cbxSyndrome.SelectedIndex = Index2;
            //fieldNames.Sort();
            //weightFieldNames.Sort();
            //strataFieldNames.Sort();
            //cbxDate.ItemsSource = fieldNames;
            //cbxFieldWeight.ItemsSource = weightFieldNames;
            // cbxSyndrome.ItemsSource = strataFieldNames;
            //if (this.cbxDate.Items.Count > 0)
            //{
            //    this.cbxDate.SelectedIndex = -1;
            //}
            //if (this.cbxFieldWeight.Items.Count > 0)
            //{
            //    this.cbxFieldWeight.SelectedIndex = -1;
            //}
            //if (this.cbxSyndrome.Items.Count > 0)
            //{
            //    this.cbxSyndrome.SelectedIndex = -1;
            //}
        }

        private void Construct()
        {
            this.cbxDate.SelectionChanged += new SelectionChangedEventHandler(cbxField_SelectionChanged);
            this.cbxFieldWeight.SelectionChanged += new SelectionChangedEventHandler(cbxField_SelectionChanged);
            this.cbxSyndrome.SelectionChanged += new SelectionChangedEventHandler(cbxField_SelectionChanged);
            this.IsProcessing = false;

            this.GadgetStatusUpdate += new GadgetStatusUpdateHandler(RequestUpdateStatusMessage);
            this.GadgetCheckForCancellation += new GadgetCheckForCancellationHandler(IsCancelled);

            this.waitCursor.Visibility = System.Windows.Visibility.Collapsed;
            this.pnlStatus.Visibility = System.Windows.Visibility.Collapsed;

            this.EnableDisableButton();
        }

        /// <summary>
        /// Enables or disables button
        /// </summary>
        private void EnableDisableButton()
        {
            if (this.cbxDate.SelectedIndex > 0)
            {
                this.btnRun.IsEnabled = true;
            }
            else
            {
                this.btnRun.IsEnabled = false;
            }
        }

        /// <summary>
        /// Main function that does Aberration Detection.
        /// </summary>
        private void DoAD()
        {
            if (!LoadingDropDowns && !LoadingCanvas)
            {
                //CommonClass.IsUserDefindVariableInUse(SelectedColsCollection);
                IsUserDefindVariableInUse();
                this.RefreshResults();
                this.aberrationViewModel = (AberrationViewModel)this.DataContext;
                if (Index1 > 0)
                {
                    this.gadgetOptions.DatasourceName = this.applicationViewModel.EwavSelectedDatasource.DatasourceName;
                    this.gadgetOptions.UseAdvancedDataFilter = applicationViewModel.UseAdvancedFilter;
                    gadgetOptions.GadgetFilters = GadgetFilters;
                    this.aberrationViewModel.GetFrequencyData(this.gadgetOptions);
                }
                this.gadgetExpander.IsExpanded = false;
            }
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
        /// Method that verifies the number.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool VerifyNumber(string input)
        {
            if (!this.cmnClass.IsWholeNumber((input)))
            {
                return false;
            }
            return true;
        }

        private double CalculateStdDev(IEnumerable<double> values)
        {
            double ret = 0;
            if (values.Count() > 0)
            {
                double avg = values.Average();
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                ret = Math.Sqrt((sum) / (values.Count() - 1));
            }
            return ret;
        }

        //private void FillComboboxes(bool update = false)
        //{
        //    loadingCombos = true;

        //    string prevDateField = string.Empty;
        //    string prevWeightField = string.Empty;
        //    string prevSyndromeField = string.Empty;

        //    if (update)
        //    {
        //        if (cbxDate.SelectedIndex >= 0)
        //        {
        //            prevDateField = cbxDate.SelectedItem.ToString();
        //        }
        //        if (cbxFieldWeight.SelectedIndex >= 0)
        //        {
        //            prevWeightField = cbxFieldWeight.SelectedItem.ToString();
        //        }
        //        if (cbxSyndrome.SelectedIndex >= 0)
        //        {
        //            prevSyndromeField = cbxSyndrome.SelectedItem.ToString();
        //        }
        //    }

        //    cbxDate.ItemsSource = null;
        //    cbxDate.Items.Clear();

        //    cbxFieldWeight.ItemsSource = null;
        //    cbxFieldWeight.Items.Clear();

        //    cbxSyndrome.ItemsSource = null;
        //    cbxSyndrome.Items.Clear();

        //    List<string> fieldNames = new List<string>();
        //    List<string> weightFieldNames = new List<string>();
        //    List<string> strataFieldNames = new List<string>();

        //    weightFieldNames.Add(string.Empty);
        //    strataFieldNames.Add(string.Empty);

        //    if (dashboardHelper.IsUsingEpiProject)
        //    {
        //        foreach (Field f in View.Fields)
        //        {
        //            if (!(f is RecStatusField || f is UniqueKeyField || f is GridField || f is GlobalRecordIdField))
        //            {
        //                if (f is IDataField)
        //                {
        //                    if (f is DateField || f is DateTimeField)
        //                    {
        //                        //cbxField.Items.Add(f.Name);
        //                        fieldNames.Add(f.Name);
        //                    }

        //                    if (f is NumberField)
        //                    {
        //                        //cbxFieldWeight.Items.Add(f.Name);
        //                        weightFieldNames.Add(f.Name);
        //                    }

        //                    if (f is TextField || f is TableBasedDropDownField || f is YesNoField || f is CheckBoxField)
        //                    {
        //                        //cbxFieldStrata.Items.Add(f.Name);
        //                        strataFieldNames.Add(f.Name);
        //                    }
        //                }
        //            }
        //        }

        //        foreach (IDashboardRule rule in dashboardHelper.Rules)
        //        {
        //            if (rule is DataAssignmentRule)
        //            {
        //                DataAssignmentRule assignmentRule = rule as DataAssignmentRule;
        //                if (assignmentRule.VariableType.Equals(DashboardVariableType.Date))
        //                {
        //                    fieldNames.Add(assignmentRule.DestinationColumnName);
        //                }
        //                strataFieldNames.Add(assignmentRule.DestinationColumnName);
        //                if (assignmentRule.VariableType.Equals(DashboardVariableType.Numeric))
        //                {
        //                    weightFieldNames.Add(assignmentRule.DestinationColumnName);
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        ColumnDataType columnDataType = ColumnDataType.DateTime;
        //        fieldNames = dashboardHelper.GetFieldsAsList(columnDataType);

        //        columnDataType = ColumnDataType.Numeric;
        //        weightFieldNames.AddRange(dashboardHelper.GetFieldsAsList(columnDataType));

        //        columnDataType = ColumnDataType.Numeric | ColumnDataType.Boolean | ColumnDataType.Text;
        //        strataFieldNames.AddRange(dashboardHelper.GetFieldsAsList(columnDataType));
        //    }

        //    fieldNames.Sort();
        //    weightFieldNames.Sort();
        //    strataFieldNames.Sort();

        //    cbxDate.ItemsSource = fieldNames;
        //    cbxFieldWeight.ItemsSource = weightFieldNames;
        //    cbxSyndrome.ItemsSource = strataFieldNames;

        //    if (cbxDate.Items.Count > 0)
        //    {
        //        cbxDate.SelectedIndex = -1;
        //    }
        //    if (cbxFieldWeight.Items.Count > 0)
        //    {
        //        cbxFieldWeight.SelectedIndex = -1;
        //    }
        //    if (cbxSyndrome.Items.Count > 0)
        //    {
        //        cbxSyndrome.SelectedIndex = -1;
        //    }

        //    if (update)
        //    {
        //        cbxDate.SelectedItem = prevDateField;
        //        cbxFieldWeight.SelectedItem = prevWeightField;
        //        cbxSyndrome.SelectedItem = prevSyndromeField;
        //    }

        //    loadingCombos = false;
        //}

        private void NumberBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //e.ControlText
            //e.Handled = !Util.IsWholeNumber(e.Text);
            //base.OnPreviewTextInput(e);
        }

        //private void txtLagTime_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (!string.IsNullOrEmpty(txtLagTime.Text))
        //    {
        //        int lagTime = 7;

        //        int.TryParse(txtLagTime.Text, out lagTime);

        //        if (lagTime > 365)
        //        {
        //            txtLagTime.Text = "365";
        //        }
        //        else if (lagTime <= 2)
        //        {
        //            txtLagTime.Text = "2";
        //        }
        //    }
        //}

        //private void txtDeviations_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (!string.IsNullOrEmpty(txtDeviations.Text))
        //    {
        //        int dev = 0;

        //        int.TryParse(txtDeviations.Text, out dev);

        //        if (dev > 7)
        //        {
        //            txtDeviations.Text = "7";
        //        }
        //        else if (dev <= 1)
        //        {
        //            txtDeviations.Text = "1";
        //        }
        //    }
        //}

        private void SetGraph(string strataValue, List<SimpleDataValue> actualValues, List<SimpleDataValue> trendValues, List<SimpleDataValue> aberrationValues, List<DataGridRow> aberrationDetails)
        {
            chart = new XYChart();
            chart.Is3D = false;
            chart.XPath = "IndependentValue";
            chart.DefaultStripesVisible = true;
            chart.HighlightDataPointOnHover = true;
            chart.Theme = Defaults.THEME; // "ArcticWhite";
            chart.GlareCoverVisible = true;
            chart.SelectionVisualHint = SelectionVisualHint.InvertedColor;
            chart.Width = Defaults.CHART_WIDTH; // 800.0;
            chart.Height = Defaults.CHART_HEGHT; // 400.0;
            chart.EnableDataPointPopup = false;
            chart.EnableAnimation = true;
            chart.AnimationDuration = new TimeSpan(0, 0, 0, 4);
            chart.AnimationOnLoad = false;
            chart.Palette = Palette.GetPalette(Defaults.COLOR_PALETTE);
            chart.Legend = new ComponentArt.Silverlight.DataVisualization.Common.Legend();
            chart.LegendVisible = Defaults.SHOW_CHART_LEGEND;
            chart.Legend.Margin = new Thickness(30.0, 0.0, 10.0, 0.0);
            // chart.Legend.CornerRadius = new CornerRadius(10.0);
            chart.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 192, 207, 226)); //   "#FFc0cfe2", 

            var xAxisCoords = new AxisCoordinates();
            xAxisCoords.Angle = 70.0;
            xAxisCoords.LabelGap = 1.5;
            xAxisCoords.Margin = new Thickness(0, 5, 0, 0);
            xAxisCoords.MaximumAnnotationLevel = 1;
            xAxisCoords.LabelMargin = 5.0;

            //xAxisCoords.LabelValueConverter = ;

            xAxisCoords.FormattingString = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern; //"M/d/yyyy";

            //var datapoint = new DataPoint();

            //var xAxisLabel = new AxisLabels();

            ChartLabel chrtLabel = new ChartLabel();
            chrtLabel.Orientation = ChartLabelOrientation.Horizontal;

            TextBlock tb = new TextBlock();
            tb.Width = 402.0;
            tb.TextAlignment = TextAlignment.Center;
            tb.Padding = new Thickness(0.0, 10.0, 0.0, 10.0);
            tb.Margin = new Thickness(0.0, 40.0, 0.0, 0.0);
            chrtLabel.Child = tb;

            chart.XAxisArea.Add(xAxisCoords);
            chart.XAxisArea.Add(chrtLabel);

            chart.CoordinatesPaddingPercentage = new Thickness(0, 5, 0, 0);

            var yAxisCoords = new AxisCoordinates();

            chrtLabel = new ChartLabel();
            chrtLabel.Orientation = ChartLabelOrientation.Vertical;

            tb = new TextBlock();
            tb.Padding = new Thickness(40.0, 0.0, 0.0, 20.0);
            tb.TextAlignment = TextAlignment.Center;
            tb.Height = 36.0;
            tb.Margin = new Thickness(-40.0, 0.0, -32.0, -91.0);
            tb.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

            chrtLabel.Child = tb;

            chart.YAxisArea.Add(yAxisCoords);
            chart.YAxisArea.Add(chrtLabel);

            LineSeries actlineSeries = new LineSeries();
            actlineSeries.Label = "Actual";
            //actlineSeries.Animate = AnimationTiming(0, 0, 1);
            actlineSeries.Id = "S0";
            //actlineSeries.YPath = "Y";
            actlineSeries.LineKind = LineKind.Auto;
            actlineSeries.DashStyle = LineDashStyle.Solid;
            actlineSeries.MarkerSize = 10.0;
            actlineSeries.Marker = new Marker("Circle");
            actlineSeries.MarkerFill = new SolidColorBrush(Color.FromArgb(55, 39, 97, 143));
            actlineSeries.Thickness = 1.0;
            actlineSeries.DataSource = actualValues;
            actlineSeries.XPath = "IndependentValue";
            actlineSeries.YPath = "DependentValue";
            actlineSeries.DoubleLine = true;
            actlineSeries.Color = Color.FromArgb(255, 39, 97, 143);
            actlineSeries.Padding = new Thickness(20);
            actlineSeries.BarRelativeBegin = 0.15;//Dont know what this does
            actlineSeries.BarRelativeEnd = 0.55;//Dont know what this does
            actlineSeries.AnimationProgress = 0.1;//Dont know what this does

            LineSeries expLineSeries = new LineSeries();
            expLineSeries.Label = "Expected";
            expLineSeries.Id = "S1";
            expLineSeries.LineKind = LineKind.Auto;
            expLineSeries.DashStyle = LineDashStyle.Dash;
            expLineSeries.MarkerVisible = false;
            expLineSeries.Thickness = 3.0;
            expLineSeries.Color = Color.FromArgb(255, 254, 203, 0);
            expLineSeries.DoubleLine = true;
            expLineSeries.DataSource = trendValues;
            expLineSeries.XPath = "IndependentValue";
            expLineSeries.YPath = "DependentValue";
            expLineSeries.DoubleLine = true;
            expLineSeries.BarRelativeBegin = 0.15;//Dont know what this does
            expLineSeries.BarRelativeEnd = 0.55;//Dont know what this does
            expLineSeries.AnimationProgress = 0.9;//Dont know what this does

            MarkerSeries abrMrkSeries = new MarkerSeries();
            abrMrkSeries.Label = "Aberration";
            abrMrkSeries.Marker = new Marker("Diamond");
            abrMrkSeries.DataSource = aberrationValues;
            abrMrkSeries.MarkerSize = 13.0;
            abrMrkSeries.Color = Color.FromArgb(255, 255, 0, 0);
            abrMrkSeries.XPath = "IndependentValue";
            abrMrkSeries.YPath = "DependentValue";
            abrMrkSeries.BarRelativeBegin = 0.15;//Dont know what this does
            abrMrkSeries.BarRelativeEnd = 0.55;//Dont know what this does
            abrMrkSeries.AnimationProgress = 1.0;//Dont know what this does

            //LineSeries abrLineSeries = new LineSeries();
            //abrLineSeries.Label = "Aberration";
            //abrLineSeries.Marker = new Marker("Diamond");
            //abrLineSeries.DataSource = aberrationValues;
            //abrLineSeries.MarkerSize = 13.0;
            //abrLineSeries.XPath = "IndependentValue";
            //abrLineSeries.YPath = "DependentValue";
            //abrLineSeries.Thickness = 0;
            //abrLineSeries.DoubleLine = false;

            //abrMrkSeries.

            SeriesTracker st = new SeriesTracker();
            st.SeriesId = "S0,S1";

            SeriesAnnotationTracker sat = new SeriesAnnotationTracker();
            sat.SeriesIdsCSS = "S0,S1";

            chart.XYChartMainArea.Add(actlineSeries);
            chart.XYChartMainArea.Add(expLineSeries);
            chart.XYChartMainArea.Add(abrMrkSeries);
            chart.XYChartMainArea.Add(st);
            chart.XYChartMainArea.Add(sat);

            this.pnlCharts.Visibility = System.Windows.Visibility.Visible;

            //LinearAxis dependentAxis = new LinearAxis();
            //dependentAxis.Orientation = AxisOrientation.Y;
            //dependentAxis.Minimum = 0;
            //CategoryAxis independentAxis = new CategoryAxis();
            //independentAxis.Orientation = AxisOrientation.X;
            //independentAxis.SortOrder = CategorySortOrder.Ascending;
            //independentAxis.AxisLabelStyle = Resources["RotateAxisStyle"] as Style;
            //try
            //{
            //    independentAxis.AxisLabelStyle.Setters.Add(new Setter(AxisLabel.StringFormatProperty, "{0:d}"));
            //}
            //catch (Exception ex)
            //{
            //    //already added
            //}

            //LineSeries series1 = new LineSeries();
            ////series1.DataSource
            ////series1.IndependentValuePath = "IndependentValue";
            ////series1.DependentValuePath = "DependentValue";
            //series1.DataSource = actualValues;
            //series1.Title = "Actual";
            //series1.DependentRangeAxis = dependentAxis;
            //series1.IndependentAxis = independentAxis;

            //LineSeries series2 = new LineSeries();
            //series2.IndependentValuePath = "IndependentValue";
            //series2.DependentValuePath = "DependentValue";
            //series2.ItemsSource = trendValues;
            //series2.Title = "Expected";
            //series2.DependentRangeAxis = dependentAxis;
            //series2.IndependentAxis = independentAxis;
            //series2.PolylineStyle = Resources["GooglePolylineStyle"] as Style;
            //series2.DataPointStyle = Resources["GoogleDataPointStyle"] as Style;

            //ScatterSeries series3 = new ScatterSeries();
            //series3.IndependentValuePath = "IndependentValue";
            //series3.DependentValuePath = "DependentValue";
            //series3.ItemsSource = aberrationValues;
            //series3.Title = "Aberration";
            //series3.DependentRangeAxis = dependentAxis;
            //series3.IndependentAxis = independentAxis;
            //series3.DataPointStyle = Resources["DataPointStyle"] as Style;

            //chart.Series.Add(series1);
            //chart.Series.Add(series3);
            //chart.Series.Add(series2);
            //chart.Height = 400;
            if (actualValues.Count > 37)
            {
                chart.Width = (actualValues.Count * (871.0 / 37.0)) + 129;
            }
            else
            {
                chart.Width = 1000;
            }
            chart.BorderThickness = new Thickness(0);
            chart.Margin = new Thickness(-100, -40, 0, 0);

            Label title = new Label();
            title.Content = strataValue;
            title.Margin = new Thickness(0, -20, 0, 20);
            title.FontWeight = FontWeights.Bold;
            // Title.Text = strataValue;

            this.pnlCharts.Children.Add(title);
            this.pnlCharts.Children.Add(chart);
            //title.Visibility = System.Windows.Visibility.Visible;
            //chart.Visibility = System.Windows.Visibility.Visible;
            //DataContent.Children.Add(title);
            //DataContent.Children.Add(chart);

            if (aberrationDetails.Count == 0)
            {
                Label noAbberration = new Label();
                noAbberration.Content = "No aberrations found.";
                noAbberration.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                noAbberration.Margin = new Thickness(0, -20, 0, 50);
                noAbberration.FontWeight = FontWeights.Bold;
                noAbberration.Foreground = new SolidColorBrush(Color.FromArgb(255, 155, 0, 0)); //Brushes.Red;
                this.pnlCharts.Children.Add(noAbberration);
            }
            else
            {
                Label abberrationFound = new Label();
                abberrationFound.Content = "Aberrations found:";
                abberrationFound.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                abberrationFound.Margin = new Thickness(0, -20, 0, 5);
                abberrationFound.FontWeight = FontWeights.Bold;
                abberrationFound.Foreground = new SolidColorBrush(Color.FromArgb(255, 155, 0, 0)); //Brushes.Red;
                this.pnlCharts.Children.Add(abberrationFound);

                Grid grid = new Grid();
                grid.HorizontalAlignment = HorizontalAlignment.Center;
                grid.Margin = new Thickness(0, 0, 0, 50);

                ColumnDefinition column1 = new ColumnDefinition();
                ColumnDefinition column2 = new ColumnDefinition();
                ColumnDefinition column3 = new ColumnDefinition();
                ColumnDefinition column4 = new ColumnDefinition();

                column1.Width = GridLength.Auto;
                column2.Width = GridLength.Auto;
                column3.Width = GridLength.Auto;
                column4.Width = GridLength.Auto;

                grid.ColumnDefinitions.Add(column1);
                grid.ColumnDefinitions.Add(column2);
                grid.ColumnDefinitions.Add(column3);
                grid.ColumnDefinitions.Add(column4);

                RowDefinition rowDefHeader = new RowDefinition();
                rowDefHeader.Height = new GridLength(25);
                grid.RowDefinitions.Add(rowDefHeader); //grdFreq.RowDefinitions.Add(rowDefHeader);

                for (int y = 0; y < /*grdFreq*/grid.ColumnDefinitions.Count; y++)
                {
                    Rectangle rctHeader = new Rectangle();
                    //rctHeader.Fill = new SolidColorBrush(SystemColors.HighlightColor);  //SystemColors.MenuHighlightBrush;
                    rctHeader.Style = Application.Current.Resources["HeaderCell"] as Style;
                    Grid.SetRow(rctHeader, 0);
                    Grid.SetColumn(rctHeader, y);
                    grid.Children.Add(rctHeader); //grdFreq.Children.Add(rctHeader);
                }

                TextBlock txtValHeader = new TextBlock();
                txtValHeader.Text = " Date ";
                //txtValHeader.VerticalAlignment = VerticalAlignment.Center;
                //txtValHeader.HorizontalAlignment = HorizontalAlignment.Center;
                //txtValHeader.Margin = new Thickness(2, 0, 2, 0);
                //txtValHeader.FontWeight = FontWeights.Bold;
                //txtValHeader.Foreground = Brushes.White;
                txtValHeader.Style = Application.Current.Resources["HeaderFont"] as Style; //Brushes.White;
                Grid.SetRow(txtValHeader, 0);
                Grid.SetColumn(txtValHeader, 0);
                grid.Children.Add(txtValHeader); //grdFreq.Children.Add(txtValHeader);

                TextBlock txtFreqHeader = new TextBlock();
                txtFreqHeader.Text = " Count ";
                //txtFreqHeader.VerticalAlignment = VerticalAlignment.Center;
                //txtFreqHeader.HorizontalAlignment = HorizontalAlignment.Center;
                //txtFreqHeader.Margin = new Thickness(2, 0, 2, 0);
                //txtFreqHeader.FontWeight = FontWeights.Bold;
                //txtFreqHeader.Foreground = Brushes.White;
                txtFreqHeader.Style = Application.Current.Resources["HeaderFont"] as Style; //Brushes.White;
                Grid.SetRow(txtFreqHeader, 0);
                Grid.SetColumn(txtFreqHeader, 1);
                grid.Children.Add(txtFreqHeader); //grdFreq.Children.Add(txtFreqHeader);

                TextBlock txtPctHeader = new TextBlock();
                txtPctHeader.Text = " Expected ";
                //txtPctHeader.VerticalAlignment = VerticalAlignment.Center;
                //txtPctHeader.HorizontalAlignment = HorizontalAlignment.Center;
                //txtPctHeader.Margin = new Thickness(2, 0, 2, 0);
                //txtPctHeader.FontWeight = FontWeights.Bold;
                //txtPctHeader.Foreground = Brushes.White;
                txtPctHeader.Style = Application.Current.Resources["HeaderFont"] as Style; //Brushes.White;
                Grid.SetRow(txtPctHeader, 0);
                Grid.SetColumn(txtPctHeader, 2);
                grid.Children.Add(txtPctHeader); //grdFreq.Children.Add(txtPctHeader);

                TextBlock txtAccuHeader = new TextBlock();
                txtAccuHeader.Text = " Difference ";
                //txtAccuHeader.VerticalAlignment = VerticalAlignment.Center;
                //txtAccuHeader.HorizontalAlignment = HorizontalAlignment.Center;
                //txtAccuHeader.Margin = new Thickness(2, 0, 2, 0);
                //txtAccuHeader.FontWeight = FontWeights.Bold;
                //txtAccuHeader.Foreground = Brushes.White;
                txtAccuHeader.Style = Application.Current.Resources["HeaderFont"] as Style; //Brushes.White;
                Grid.SetRow(txtAccuHeader, 0);
                Grid.SetColumn(txtAccuHeader, 3);
                grid.Children.Add(txtAccuHeader);

                this.pnlCharts.Children.Add(grid);

                int rowcount = 1;
                foreach (DataGridRow aberrationDetail in aberrationDetails)
                {
                    RowDefinition rowDef = new RowDefinition();
                    grid.RowDefinitions.Add(rowDef);

                    TextBlock txtDate = new TextBlock();
                    txtDate.Text = aberrationDetail.Date.ToShortDateString();
                    txtDate.Margin = new Thickness(2);
                    txtDate.VerticalAlignment = VerticalAlignment.Center;
                    txtDate.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetRow(txtDate, rowcount);
                    Grid.SetColumn(txtDate, 0);
                    grid.Children.Add(txtDate);

                    TextBlock txtFreq = new TextBlock();
                    txtFreq.Text = aberrationDetail.Frequency.ToString();
                    txtFreq.Margin = new Thickness(2);
                    txtFreq.VerticalAlignment = VerticalAlignment.Center;
                    txtFreq.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetRow(txtFreq, rowcount);
                    Grid.SetColumn(txtFreq, 1);
                    grid.Children.Add(txtFreq);

                    TextBlock txtAvg = new TextBlock();
                    txtAvg.Text = aberrationDetail.RunningAverage.ToString("N2");
                    txtAvg.Margin = new Thickness(2);
                    txtAvg.VerticalAlignment = VerticalAlignment.Center;
                    txtAvg.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetRow(txtAvg, rowcount);
                    Grid.SetColumn(txtAvg, 2);
                    grid.Children.Add(txtAvg);

                    TextBlock txtDelta = new TextBlock();
                    txtDelta.Text = string.Format("  {0} standard deviations  ", aberrationDetail.Delta.ToString("N2"));

                    StackPanel pnl = new StackPanel();
                    pnl.Orientation = Orientation.Horizontal;
                    pnl.VerticalAlignment = VerticalAlignment.Center;
                    pnl.HorizontalAlignment = HorizontalAlignment.Center;
                    pnl.Children.Add(txtDelta);

                    Grid.SetRow(pnl, rowcount);
                    Grid.SetColumn(pnl, 3);
                    grid.Children.Add(pnl);

                    rowcount++;
                }

                int rdcount = 0;
                foreach (RowDefinition rd in grid.RowDefinitions)
                {
                    int cdcount = 0;
                    foreach (ColumnDefinition cd in grid.ColumnDefinitions)
                    {
                        Rectangle rctBorder = new Rectangle();
                        ////rctBorder.Stroke = Brushes.Black;
                        rctBorder.Style = Application.Current.Resources["DataCell"] as Style;
                        Grid.SetRow(rctBorder, rdcount);
                        Grid.SetColumn(rctBorder, cdcount);
                        grid.Children.Add(rctBorder);
                        cdcount++;
                    }
                    rdcount++;
                }
            }
        }

        private void RenderFinish()
        {
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            this.waitCursor.Visibility = Visibility.Collapsed;
            this.DataContent.Visibility = System.Windows.Visibility.Visible;
            this.pnlStatus.Visibility = System.Windows.Visibility.Collapsed;
            this.txtStatus.Text = string.Empty;
            this.FilterButton.IsEnabled = true;
            this.HeaderButton.IsEnabled = true;
            if (viewModel != null)
            {
                LoadChart(viewModel);//Loads the values read from CreateFromXML. 
            }

        }

        private void RenderFinishWithWarning(string errorMessage)
        {
            this.waitCursor.Visibility = Visibility.Collapsed;

            //pnlStatus.Background = Brushes.Gold;
            //pnlStatusTop.Background = Brushes.Goldenrod;
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            this.pnlStatus.Visibility = System.Windows.Visibility.Visible;
            this.txtStatus.Text = errorMessage;
        }

        private void RenderFinishWithError(string errorMessage)
        {
            this.waitCursor.Visibility = Visibility.Collapsed;
            this.DataContent.Visibility = System.Windows.Visibility.Collapsed;
            this.pnlStatus.Background = new SolidColorBrush(Color.FromArgb(255, 248, 215, 226)); //Brushes.Tomato;
            this.pnlStatusTop.Background = new SolidColorBrush(Color.FromArgb(255, 228, 101, 142)); //Brushes.Red;
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            this.pnlStatus.Visibility = System.Windows.Visibility.Visible;
            this.txtStatus.Text = errorMessage;
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

        private bool IsCancelled()
        {
            return false;
        }

        private void CreateInputVariableList()
        {
            Dictionary<string, string> inputVariableList = new Dictionary<string, string>();

            this.gadgetOptions.MainVariableName = string.Empty;
            this.gadgetOptions.WeightVariableName = string.Empty;
            this.gadgetOptions.StrataVariableNames = new List<string>();
            this.gadgetOptions.CrosstabVariableName = string.Empty;
            this.gadgetOptions.StrataVariableList = new List<MyString>();
            this.gadgetOptions.TableName = applicationViewModel.EwavSelectedDatasource.TableName;
            if (this.cbxDate.SelectedIndex > -1 && !string.IsNullOrEmpty(((EwavColumn)this.cbxDate.SelectedItem).Name.ToString().Trim()))
            {
                inputVariableList.Add("freqvar", ((EwavColumn)this.cbxDate.SelectedItem).Name.ToString()); //cbxDate.SelectedItem.ToString());
                this.gadgetOptions.MainVariableName = ((EwavColumn)this.cbxDate.SelectedItem).Name.ToString(); // cbxDate.SelectedItem.ToString();
            }
            else
            {
                return;
            }

            if (this.cbxFieldWeight.SelectedIndex > -1 && !string.IsNullOrEmpty(((EwavColumn)this.cbxFieldWeight.SelectedItem).Name.ToString().Trim()))
            {
                inputVariableList.Add("weightvar", ((EwavColumn)this.cbxFieldWeight.SelectedItem).Name.ToString());
                this.gadgetOptions.WeightVariableName = ((EwavColumn)this.cbxFieldWeight.SelectedItem).Name.ToString();// cbxFieldWeight.SelectedItem.ToString();
            }
            if (this.cbxSyndrome.SelectedIndex > -1 && !string.IsNullOrEmpty(((EwavColumn)this.cbxSyndrome.SelectedItem).Name.ToString().Trim()))// cbxSyndrome.SelectedItem.ToString()))
            {
                inputVariableList.Add("stratavar", ((EwavColumn)this.cbxSyndrome.SelectedItem).Name.ToString()); //cbxSyndrome.SelectedItem.ToString());
                this.gadgetOptions.StrataVariableNames = new List<string>();
                //gadgetOptions.StrataVariableNames.Add();

                this.gadgetOptions.StrataVariableNames = new List<string>();
                List<MyString> listMyString = new List<MyString>();
                MyString objMyString = new MyString();
                objMyString.VarName = ((EwavColumn)this.cbxSyndrome.SelectedItem).Name.ToString(); //cbxSyndrome.SelectedItem.ToString();
                listMyString.Add(objMyString);
                this.gadgetOptions.StrataVariableList = listMyString;
            }

            if (!string.IsNullOrEmpty(this.txtLagTime.Text))
            {
                inputVariableList.Add("lagtime", this.txtLagTime.Text);
            }
            else
            {
                inputVariableList.Add("lagtime", DEFAULT_LAG_TIME.ToString());
                this.txtLagTime.Text = DEFAULT_LAG_TIME.ToString();
            }

            if (!string.IsNullOrEmpty(this.txtDeviations.Text))
            {
                inputVariableList.Add("deviations", this.txtDeviations.Text);
            }
            else
            {
                inputVariableList.Add("deviations", DEFAULT_DEVIATIONS.ToString());
                this.txtDeviations.Text = DEFAULT_DEVIATIONS.ToString();
            }
            inputVariableList.Add("aberration", "true");
            this.gadgetOptions.ShouldIncludeFullSummaryStatistics = false;
            this.gadgetOptions.InputVariableList = inputVariableList;
        }

        private void ShowError()
        {
            this.pnlStatus.Visibility = Visibility.Visible;
            this.txtStatus.Text = SharedStrings.NO_RECORDS_SELECTED;
        }

        #endregion

        #region Properties

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

        public string MyControlName
        {
            get
            {
                return "AberrationControl";
            }
        }

        public string MyUIName
        {
            get
            {
                return "Aberration Detection";
            }
        }

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

        /// <summary>
        /// Creates The gadget from XML
        /// </summary>
        /// <param name="element"></param>
        public void CreateFromXml(XElement element)
        {
            LoadingCanvas = true;

            //InitializeControl();

            List<EwavColumn> dateColList = cmnClass.GetItemsSource(GetFieldDateDataType);
            List<EwavColumn> weightColList = cmnClass.GetItemsSource(GetFieldWeightDataType);
            List<EwavColumn> syndromeColList = cmnClass.GetItemsSource(GetFieldSyndromeDataType);

            viewModel = new SetLabelsViewModel();

            this.GadgetFilters = new List<EwavDataFilterCondition>();
            foreach (XElement child in element.Descendants())
            {
                switch (child.Name.ToString().ToLower())
                {
                    case "mainvariable":
                        cbxDate.SelectedIndex = cmnClass.FindIndexToSelect(dateColList, child.Value.ToString().Replace("&lt;", "<"));
                        Index1 = cbxDate.SelectedIndex;
                        break;
                    case "stratavariable":
                        cbxSyndrome.SelectedIndex = cmnClass.FindIndexToSelect(syndromeColList, child.Value.ToString().Replace("&lt;", "<"));
                        Index2 = cbxSyndrome.SelectedIndex;
                        break;
                    case "weightvariable":
                        cbxFieldWeight.SelectedIndex = cmnClass.FindIndexToSelect(weightColList, child.Value.ToString().Replace("&lt;", "<"));
                        Index3 = cbxFieldWeight.SelectedIndex;
                        break;
                    case "lagtime":
                        txtLagTime.Text = child.Value;
                        break;
                    case "deviations":
                        txtDeviations.Text = child.Value;
                        break;

                    case "gadgetname":

                        if (!string.IsNullOrEmpty(child.Value.ToString()))
                        {
                            viewModel.GadgetName = child.Value.ToString();
                        }
                        else
                        {
                            viewModel.GadgetName = this.MyUIName;
                        }
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
                        {
                            viewModel.CollorPallet = child.Value.ToString();
                        }
                        else
                        {
                            viewModel.CollorPallet = Defaults.COLOR_PALETTE;
                        }

                        break;
                    case "usediffcolors":
                        if (!string.IsNullOrEmpty(child.Value.ToString()))
                        {
                            viewModel.UseDifferentBarColors = Convert.ToBoolean(child.Value.ToString());
                        }
                        else
                        {
                            viewModel.UseDifferentBarColors = Defaults.USE_DIFFERENT_BAR_COLORS;
                        }

                        break;
                    //case "spacesbetweenbars":
                    //    if (!string.IsNullOrEmpty(child.Value.ToString()))
                    //    {
                    //        viewModel.SpacesBetweenBars = child.Value.ToString();
                    //    }
                    //    else
                    //    {
                    //        viewModel.SpacesBetweenBars = Defaults.SPACE_BETWEEN_BARS;
                    //    }

                    //    break;
                    case "showlegend":
                        if (!string.IsNullOrEmpty(child.Value.ToString()))
                        {
                            viewModel.ShowLegend = Convert.ToBoolean(child.Value.ToString());
                        }
                        else
                        {
                            viewModel.ShowLegend = Defaults.SHOW_CHART_LEGEND;
                        }

                        break;
                    //case "showvariablenames":
                    //    if (!string.IsNullOrEmpty(child.Value.ToString()))
                    //    {
                    //        viewModel.ShowVariableNames = Convert.ToBoolean(child.Value.ToString());
                    //    }
                    //    else
                    //    {
                    //        viewModel.ShowVariableNames = Defaults.SHOW_CHART_VAR_NAMES;
                    //    }

                    //    break;
                    case "legendposition":
                        if (!string.IsNullOrEmpty(child.Value.ToString()))
                        {
                            viewModel.LegendPostion = child.Value.ToString();
                        }
                        else
                        {
                            viewModel.LegendPostion = Defaults.LEGEND_POSITION;
                        }

                        break;
                    case "width":
                        if (!string.IsNullOrEmpty(child.Value.ToString()))
                        {
                            viewModel.Width = Convert.ToDouble(child.Value.ToString());
                        }
                        else
                        {
                            viewModel.Width = Defaults.CHART_WIDTH;
                        }

                        break;
                    case "height":
                        if (!string.IsNullOrEmpty(child.Value.ToString()))
                        {
                            viewModel.Height = Convert.ToDouble(child.Value.ToString());
                        }
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

            DoAD();

            cmnClass.AddControlToCanvas(this, mouseVerticalPosition, mouseHorizontalPosition, applicationViewModel.LayoutRoot);
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

            if (this.GadgetProcessingFinished != null)
            {
                this.GadgetProcessingFinished(this);
            }
        }

        /// <summary>
        /// Gets/sets whether the gadget is processing
        /// </summary>
        public bool IsProcessing { get; set; }

        public StringBuilder HtmlBuilder { get; set; }

        public void RefreshResults()
        {
            if (!this.loadingCombos && this.gadgetOptions != null && this.cbxDate.SelectedIndex > -1)
            {
                this.CreateInputVariableList();

                this.pnlStatus.Background = new SolidColorBrush(Color.FromArgb(255, 235, 245, 214)); //Brushes.PaleGreen;
                this.pnlStatusTop.Background = new SolidColorBrush(Color.FromArgb(255, 162, 208, 64)); //Brushes.Green;

                this.pnlCharts.Children.Clear();
                // pnlStatus.Visibility = Visibility.Collapsed;
                this.waitCursor.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Updates the variable names available in the gadget's properties
        /// </summary>
        public void UpdateVariableNames()
        {
            //FillComboboxes(true);
        }

        /// <summary>
        /// Generates Xml representation of this gadget
        /// </summary>
        /// <param name="doc">The Xml docment</param>
        /// <returns>XmlNode</returns>
        public XNode Serialize(XDocument doc)
        {
            CreateInputVariableList();

            string freqVar = string.Empty;
            string strataVar = string.Empty;
            string weightVar = string.Empty;
            string sort = string.Empty;
            bool allValues = false;
            bool showConfLimits = true;
            bool showCumulativePercent = true;
            bool includeMissing = false;
            int lagTime = 7;
            int deviations = 3;

            if (gadgetOptions.InputVariableList != null)
            {
                if (gadgetOptions.InputVariableList.ContainsKey("freqvar"))
                {
                    freqVar = gadgetOptions.InputVariableList["freqvar"];
                }
                if (gadgetOptions.InputVariableList.ContainsKey("stratavar"))
                {
                    strataVar = gadgetOptions.InputVariableList["stratavar"];
                }
                if (gadgetOptions.InputVariableList.ContainsKey("weightvar"))
                {
                    weightVar = gadgetOptions.InputVariableList["weightvar"];
                }
                if (gadgetOptions.InputVariableList.ContainsKey("sort"))
                {
                    sort = gadgetOptions.InputVariableList["sort"];
                }
                if (gadgetOptions.InputVariableList.ContainsKey("lagtime"))
                {
                    lagTime = int.Parse(gadgetOptions.InputVariableList["lagtime"]);
                }
                if (gadgetOptions.InputVariableList.ContainsKey("deviations"))
                {
                    deviations = int.Parse(gadgetOptions.InputVariableList["deviations"]);
                }

            }

            //string xmlString =
            //"<mainVariable>" + freqVar + "</mainVariable>" +
            //"<strataVariable>" + strataVar + "</strataVariable>" +
            //"<weightVariable>" + weightVar + "</weightVariable>" +
            //"<lagTime>" + lagTime + "</lagTime>" +
            //"<deviations>" + deviations + "</deviations>" +
            //"<sort>" + sort + "</sort>";

            LoadViewModel();

            XElement element = new XElement("gadget",
                new XAttribute("top", Canvas.GetTop(this).ToString("F0")),
                new XAttribute("left", Canvas.GetLeft(this).ToString("F0")),
                new XAttribute("collapsed", "false"),
                new XAttribute("gadgetType", "Ewav.AberrationControl"),
                new XElement("mainVariable", freqVar),
                new XElement("strataVariable", strataVar),
                new XElement("weightVariable", weightVar),
                new XElement("lagTime", lagTime),
                new XElement("deviations", deviations),
                new XElement("sort", sort),
                
                new XElement("gadgetName", viewModel.GadgetName),
                new XElement("gadgetDescription", Convert.ToBase64String(System.Text.ASCIIEncoding.Unicode.GetBytes(viewModel.GadgetDescription))),
                new XElement("colorPalette", viewModel.CollorPallet),
                new XElement("useDiffColors", viewModel.UseDifferentBarColors),
                new XElement("showLegend", viewModel.ShowLegend),
                new XElement("legendPosition", viewModel.LegendPostion),
                new XElement("width", viewModel.Width),
                new XElement("height", viewModel.Height)
                );

            if (this.GadgetFilters != null)
            {
                this.GadgetFilters.Serialize(element);
            }
            return element;
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

        /// <summary>
        /// Converts the gadget's output to Html
        /// </summary>
        /// <returns></returns>
        public string ToHTML(bool ForDash = false, string htmlFileName = "", int count = 0)
        {
            //StringBuilder htmlBuilder = new StringBuilder();
            //htmlBuilder.AppendLine("<h2 class=\"gadgetHeading\">EARS Aberration</h2>");
            //int aberrationChartCount = 0;
            //string imageFileName = string.Empty;
            //if (htmlFileName.EndsWith(".html"))
            //{
            //    imageFileName = htmlFileName.Remove(htmlFileName.Length - 5, 5);
            //}
            //else if (htmlFileName.EndsWith(".htm"))
            //{
            //    imageFileName = htmlFileName.Remove(htmlFileName.Length - 4, 4);
            //}
            //foreach (UIElement control in pnlCharts.Children)
            //{
            //    if (control is Label)
            //    {
            //        Label label = control as Label;
            //        if (label.Content.ToString().ToLower().Contains("aberrations found"))
            //        {
            //            htmlBuilder.AppendLine("<p><span class=\"warning\">" + label.Content + "</span></p>");
            //        }
            //        else
            //        {
            //            htmlBuilder.AppendLine("<p><span class=\"bold\">" + label.Content + "</span></p>");
            //        }
            //    }
            //    else if (control is Chart)
            //    {
            //        imageFileName = imageFileName + "_" + count.ToString() + "_" + aberrationChartCount.ToString() + ".png";
            //        Chart chart = control as Chart;
            //        RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
            //        (int)chart.ActualWidth,
            //        (int)chart.ActualHeight,
            //        96d,
            //        96d,
            //        PixelFormats.Pbgra32);
            //        Size size = new Size(chart.ActualWidth, chart.ActualHeight);
            //        Rectangle rect = new Rectangle() { Width = chart.ActualWidth, Height = chart.ActualHeight, Fill = new VisualBrush(chart) };
            //        rect.Measure(size);
            //        rect.Arrange(new Rect(size));
            //        rect.UpdateLayout();
            //        renderBitmap.Render(rect);
            //        using (FileStream outStream = new FileStream(imageFileName, FileMode.Create))
            //        {
            //            // Use png encoder for our data
            //            PngBitmapEncoder encoder = new PngBitmapEncoder();
            //            // push the rendered bitmap to it
            //            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            //            // save the data to the stream
            //            encoder.Save(outStream);
            //        }
            //        //BitmapSource img = (BitmapSource)ToImageSource(chart);
            //        //FileStream stream = new FileStream(imageFileName, FileMode.Create);
            //        //JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            //        //encoder.Frames.Add(BitmapFrame.Create(img));
            //        //encoder.Save(stream);
            //        //stream.Close();
            //        htmlBuilder.AppendLine("<img src=\"" + imageFileName + "\" alt=\"aberration graph\" />");
            //        aberrationChartCount++;
            //    }
            //    else if (control is Grid)
            //    {
            //        Grid grid = control as Grid;
            //        htmlBuilder.AppendLine("<div style=\"height: 7px;\"></div>");
            //        htmlBuilder.AppendLine("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">");
            //        foreach (UIElement element in grid.Children)
            //        {
            //            if (element is TextBlock)
            //            {
            //                int rowNumber = Grid.GetRow(element);
            //                int columnNumber = Grid.GetColumn(element);
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
            //                string value = ((TextBlock)element).Text;
            //                string formattedValue = value;
            //                if ((rowNumber == grid.RowDefinitions.Count - 1))
            //                {
            //                    formattedValue = "<span class=\"total\">" + value + "</span>";
            //                }
            //                htmlBuilder.AppendLine(tableDataTagOpen + formattedValue + tableDataTagClose);
            //                if (columnNumber >= grid.ColumnDefinitions.Count - 1)
            //                {
            //                    htmlBuilder.AppendLine("</tr>");
            //                }
            //            }
            //            else if (element is StackPanel)
            //            {
            //                int rowNumber = Grid.GetRow(element);
            //                int columnNumber = Grid.GetColumn(element);
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
            //                StackPanel panel = element as StackPanel;
            //                string value = string.Empty;
            //                foreach (UIElement panelElement in panel.Children)
            //                {
            //                    if (panelElement is TextBlock)
            //                    {
            //                        value = value + ((TextBlock)panelElement).Text;
            //                    }
            //                }
            //                string formattedValue = value;
            //                if ((rowNumber == grid.RowDefinitions.Count - 1))
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
            //        htmlBuilder.AppendLine("<div style=\"height: 17px;\"></div>");
            //    }
            //}
            //return htmlBuilder.ToString();

            HtmlBuilder = new StringBuilder("<h2>Aberration Detection HTML not Implemented </h2>");

            return "";
        }

        public string CustomOutputHeading { get; set; }

        public string CustomOutputDescription { get; set; }

        public string CustomOutputCaption { get; set; }

        #endregion

        //public ImageSource ToImageSource(FrameworkElement obj)
        //{
        //    // Save current canvas transform
        //    Transform transform = obj.LayoutTransform;

        //    // fix margin offset as well
        //    Thickness margin = obj.Margin;
        //    obj.Margin = new Thickness(0, 0,
        //         margin.Right - margin.Left, margin.Bottom - margin.Top);

        //    // Get the size of canvas
        //    Size size = new Size(obj.ActualWidth, obj.ActualHeight);

        //    // force control to Update
        //    obj.Measure(size);
        //    obj.Arrange(new Rect(size));

        //    RenderTargetBitmap bmp = new RenderTargetBitmap(
        //        (int)obj.ActualWidth, (int)obj.ActualHeight, 96, 96, PixelFormats.Pbgra32);

        //    bmp.Render(obj);

        //    // return values as they were before
        //    obj.LayoutTransform = transform;
        //    obj.Margin = margin;
        //    return bmp;
        //}

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

        public ApplicationViewModel ApplicationViewModel
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        public void Reload()
        {




            SaveColumnValues();
            DoAD();




        }
        SetLabels window;
        SetLabelsViewModel viewModel = null;
        public void SetChartLabels()
        {
            LoadViewModel();
            window = new SetLabels("aberrationdetection", viewModel, true);// { DataContext = this.DataContext };

            window.Unloaded -= new RoutedEventHandler(window_Closed);
            window.Closed += new EventHandler(window_Closed);
            window.Show();
        }

        void window_Closed(object sender, EventArgs e)
        {
            if (window.DialogResult.Value)
            {
                SetValuesForAxis();
            }

        }
        private void SetValuesForAxis()
        {
            viewModel = (SetLabelsViewModel)this.window.DataContext;


            LoadChart(viewModel);
        }
        private void LoadChart(SetLabelsViewModel viewModel)
        {
            //this.tbChartName.Text = viewModel.GadgetName;
            //this.tbGadgetDescription.Text = viewModel.GadgetDescription;

            //this.chart.Palette = Palette.GetPalette(viewModel.CollorPallet);
            //this.chart.Width = viewModel.Width;
            //this.chart.Height = viewModel.Height;

            //switch (viewModel.LegendPostion.ToLower())
            //{
            //    case "left":
            //        this.chart.LegendDock = ComponentArt.Silverlight.DataVisualization.Charting.Dock.Left;
            //        break;
            //    case "right":
            //        this.chart.LegendDock = ComponentArt.Silverlight.DataVisualization.Charting.Dock.Right;
            //        break;
            //    case "bottom":
            //        this.chart.LegendDock = ComponentArt.Silverlight.DataVisualization.Charting.Dock.Bottom;
            //        break;
            //    default:
            //        this.chart.LegendDock = ComponentArt.Silverlight.DataVisualization.Charting.Dock.Top;
            //        break;
            //}

            //this.chart.LegendVisible = viewModel.ShowLegend;
            //this.chart.Legend.Orientation = ComponentArt.Silverlight.DataVisualization.Common.LegendItemOrientation.Vertical;
            //this.chart.UseDifferentBarColors = viewModel.UseDifferentBarColors;
            // set header / footer / title    
            tbGadgetDescription.Text = viewModel.GadgetDescription;

            tbChartName.Text = viewModel.GadgetTitle;

        }

        private void LoadViewModel()
        {
            //if (viewModel == null)
            //{
            //viewModel = new SetLabelsViewModel();
            //viewModel.GadgetName = tbChartName.Text;
            //viewModel.GadgetDescription = tbGadgetDescription.Text;
            //viewModel.ShowLegend = this.chart.LegendVisible;
            //viewModel.Width = this.chart.Width;
            //viewModel.Height = this.chart.Height;
            //viewModel.CollorPallet = this.chart.Palette.PaletteName.ToString();
            ////viewModel.ChartTitle = tbChartName.Text;
            //switch (this.chart.LegendDock)
            //{
            //    case ComponentArt.Silverlight.DataVisualization.Charting.Dock.Bottom:
            //        viewModel.LegendPostion = "Bottom";
            //        break;
            //    case ComponentArt.Silverlight.DataVisualization.Charting.Dock.Left:
            //        viewModel.LegendPostion = "Left";
            //        break;
            //    case ComponentArt.Silverlight.DataVisualization.Charting.Dock.Right:
            //        viewModel.LegendPostion = "Right";
            //        break;

            //    case ComponentArt.Silverlight.DataVisualization.Charting.Dock.Top:
            //        viewModel.LegendPostion = "Top";
            //        break;
            //    default:
            //        viewModel.LegendPostion = "Top";
            //        break;
            //}

            //viewModel.UseDifferentBarColors = this.chart.UseDifferentBarColors;
            viewModel = new SetLabelsViewModel();
            viewModel.GadgetTitle = tbChartName.Text;
            viewModel.GadgetDescription = tbGadgetDescription.Text;
            // }
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
            gadgetFiltersWindow.Closed += new EventHandler(gadgetFiltersWindow_Closed);
        }

        void gadgetFiltersWindow_Closed(object sender, EventArgs e)
        {
            GadgetFilterControl GadgetFilter = ((GadgetFilterControl)sender);
            if (GadgetFilter.DialogResult == true)
            {
                GadgetFilters = GadgetFilter.GadgetFilters;
                DoAD();
            }
        }
    }
}

namespace Ewav.Web.Services
{
    public partial class AberrationDomainContext
    {
        //This is the place to set RIA Services query timeout. 
        //TimeSpan(0, 5, 0) means five minutes vs the 
        //default 60 sec
        partial void OnCreated()
        {
            if (!DesignerProperties.GetIsInDesignMode(Application.Current.RootVisual))
            {
                ((WebDomainClient<IAberrationDomainServiceContract>)DomainClient).ChannelFactory.Endpoint.Binding.SendTimeout =
                    new TimeSpan(0, 120, 0);
            }
        }
    }
}