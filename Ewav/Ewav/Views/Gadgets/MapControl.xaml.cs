/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MapControl.xaml.cs
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
using System.Xml.Linq;
using Ewav.Web.EpiDashboard;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using Ewav.BAL;
using Ewav.DTO;
using Ewav.Mapping;
using Ewav.ViewModels;
using Ewav.ViewModels.Gadgets;
using Ewav.Web.Services;
using System.Windows.Media;
using System.Windows.Input;
using ESRI.ArcGIS.Client.Toolkit;
using System.Windows.Shapes;
using Ewav.ExtensionMethods;
using System.ComponentModel;

namespace Ewav
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(UserControl))]
    [ExportMetadata("type", "gadget")]
    [ExportMetadata("tabindex", "0")]
    public partial class MapControl : UserControl, IEwavGadget2, ICustomizableGadget
    {
        public const string mapKey = "Akvtgf3NbIDAoTRCfRuIV8zh6xxpf_-3hOZ2zmf_7oowderuJ7kml2-qMOwqrJ5u";

        //   MapControlViewModel mmviewModel;
        ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;

        public List<EwavDataFilterCondition> GadgetFilters
        {
            get;
            set;
        }

        EwavColumn Col1, Col2, Col3, DFInUse = null;

        Envelope curentExtent = new Envelope();

        ClientCommon.Common cmnClass = new ClientCommon.Common();

        XElement element;

        List<ColumnDataType> fieldPrimaryDataTypesList;

        private GadgetParameters gadgetOptions;

        int Index1 = -1, Index2 = -1, Index3 = -1;
        List<ColumnDataType> latLonDataTypesList;

        private bool LoadingDropDowns;

        MapControlViewModel mcviewModel;

        private bool fromCanvas;

        public MapControl()
        {


            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MapControl_Loaded);

            // Init gadget parameters with default values
            gadgetOptions = new GadgetParameters();
            gadgetOptions.ShouldIncludeFullSummaryStatistics = true;
            gadgetOptions.ShouldIncludeMissing = false;
            gadgetOptions.ShouldSortHighToLow = false;
            gadgetOptions.ShouldUseAllPossibleValues = false;
            gadgetOptions.StrataVariableNames = new List<string>();

            PopulateControlsWithData();

            udWidth.Value = map1.Width;
            udHeight.Value = map1.Height;



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


        /// <summary>
        /// Applications the view model_ apply data filter event.
        /// </summary>
        /// <param name="o">The o.</param>
        public void applicationViewModel_ApplyDataFilterEvent(object o)
        {
            if (applicationViewModel.RemoveIndicator &&
                    (IsDFUsedInThisGadget()))
            {
                ResetGadget();
            }
            else
            {
                DoMap();
            }
        }

        /// <summary>
        /// Applications the view model_ defined variable added event.
        /// </summary>
        /// <param name="o">The o.</param>
        public void applicationViewModel_DefinedVariableAddedEvent(object o)
        {
            SearchIndex();
            LoadingDropDowns = true;
            PopulateControlsWithData();
            LoadingDropDowns = false;
            DoMap();
        }

        /// <summary>
        /// Applications the view model_ defined variable in use deleted event.
        /// </summary>
        /// <param name="o">The o.</param>
        public void applicationViewModel_DefinedVariableInUseDeletedEvent(object o)
        {
            ResetGadget();
        }

        /// <summary>
        /// Applications the view model_ defined variable not in use deleted event.
        /// </summary>
        /// <param name="o">The o.</param>
        public void applicationViewModel_DefinedVariableNotInUseDeletedEvent(object o)
        {
            SearchIndex();
            LoadingDropDowns = true;
            PopulateControlsWithData();
            LoadingDropDowns = false;
            DoMap();
        }

        /// <summary>
        /// Applications the view model_ pre column changed event.
        /// </summary>
        /// <param name="o">The o.</param>
        public void applicationViewModel_PreColumnChangedEvent(object o)
        {
            SaveColumnValues();
        }

        /// <summary>
        /// Applications the view model_ unloaded event.
        /// </summary>
        /// <param name="o">The o.</param>
        public void applicationViewModel_UnloadedEvent(object o)
        {
            UnloadGadget();
        }

        /// <summary>
        /// Clears the results.
        /// </summary>
        public void ClearResults()
        {
            // TODO: Implement this method
            throw new NotImplementedException();
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
        public void CloseGadget()
        {
            applicationViewModel.CloseGadget(this);

        }

        /// <summary>
        /// Associates the event handlers with Events.
        /// </summary>
        public void Construct()
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        private void LayoutRoot_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //ClientCommon.Common cmnClass = new ClientCommon.Common();
            Point p = e.GetSafePosition(cmnClass.GetParentObject<Grid>(this, "LayoutRoot"));
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).GadgetNameOnRightClick = this.MyControlName; //"FrequencyControl";
            //((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).StrataList = this.strataGridList;
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).SelectedGadget = this;
            cmnClass.UpdateZOrder(this, true, cmnClass.GetParentObject<Grid>(this, "LayoutRoot"));
        }
        /// <summary>
        /// Creates from XML.
        /// </summary>
        /// <param name="element">The element.</param>
        public void CreateFromXml(XElement element)
        {
            try
            {
                ClientCommon.Common cmnClass = new ClientCommon.Common();
                LoadingCanvas = true;
                this.GadgetFilters = new List<EwavDataFilterCondition>();
                PopulateControlsWithData();
                //PopulateControlsWithData();
                //FillRanges();

                // map1.Extent = new Envelope();    
                curentExtent = new Envelope();

                foreach (XElement child in element.Descendants())
                {
                    switch (child.Name.ToString().ToLower())
                    {
                        case "latitude":
                            cbxLatitude.SelectedIndex = cmnClass.FindIndexToSelect(cmnClass.GetItemsSource(GetLatLonPrimaryDataType, false), child.Value.ToString().Replace("&lt;", "<"));
                            break;
                        case "longitude":
                            cbxLongitude.SelectedIndex = cmnClass.FindIndexToSelect(cmnClass.GetItemsSource(GetLatLonPrimaryDataType, false), child.Value.ToString().Replace("&lt;", "<"));
                            break;
                        case "stratifyby":
                            cbxStratify.SelectedIndex = cmnClass.FindIndexToSelect(cmnClass.GetItemsSource(GetFieldPrimaryDataType, true), child.Value.ToString().Replace("&lt;", "<"));
                            break;
                        case "radius":
                            udRadius.Value = Convert.ToDouble(child.Value.ToString());
                            break;
                        case "clustersize":
                            udClusterSize.Value = Convert.ToDouble(child.Value.ToString());
                            break;
                        case "mapwidth":
                            udWidth.Value = Convert.ToDouble(child.Value.ToString());
                            break;
                        case "mapheight":
                            udHeight.Value = Convert.ToDouble(child.Value.ToString());
                            break;
                        case "roadsselected":
                            rbRoads.IsChecked = Convert.ToBoolean(child.Value.ToString());
                            break;

                        case "satelliteselected":
                            rbSatellite.IsChecked = Convert.ToBoolean(child.Value.ToString());
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
                            throw new Exception("element " + child.Name + " not selected ");
                    }
                }

                LoadingCanvas = false;
                double mouseVerticalPosition = 0.0, mouseHorizontalPosition = 0.0;
                //mouseVerticalPosition = double.Parse(element.Attribute("top").Value.ToString());
                //mouseHorizontalPosition = double.Parse(element.Attribute("left").Value.ToString());
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

                this.fromCanvas = true;

                DoMap();
                cmnClass.AddControlToCanvas(this, mouseVerticalPosition, mouseHorizontalPosition, applicationViewModel.LayoutRoot);
            }
            catch (Exception ex)
            {


                throw new Exception();



            }
        }

        /// <summary>
        /// Used to generate the list of variables and options for the GadgetParameters object
        /// </summary>
        public void CreateInputVariableList()
        {
            //   gadgetOptions = new GadgetParameters();
            gadgetOptions.DatasourceName = applicationViewModel.EwavSelectedDatasource.DatasourceName;
            gadgetOptions.StrataVariableNames = new List<string>();
            gadgetOptions.UseAdvancedDataFilter = applicationViewModel.UseAdvancedFilter;
            gadgetOptions.GadgetFilters = GadgetFilters;
            MyString s = new MyString();

            if (cbxStratify.SelectedIndex > 0)
            {
                s.VarName = ((EwavColumn)cbxStratify.SelectedItem).Name;
                gadgetOptions.StrataVariableList = new List<MyString>() { s };
            }
            else
            {
                gadgetOptions.StrataVariableList = null;
            }
            //else
            //{
            //    s = new MyString() { VarName = "" };
            //    gadgetOptions.StrataVariableList = new List<MyString>();    

            //}


            //  for each item ih the selected fields for map tips    
            //       gadgetOptions.MapTipsColumnList.Add 




            gadgetOptions.TableName = applicationViewModel.EwavSelectedDatasource.TableName;
            gadgetOptions.InputVariableList = new Dictionary<string, string>();


            if ((EwavColumn)cbxLongitude.SelectedItem != null)
            {
                gadgetOptions.InputVariableList.Add("lonx", ((EwavColumn)cbxLongitude.SelectedItem).Name);
                gadgetOptions.InputVariableList.Add(((EwavColumn)cbxLongitude.SelectedItem).Name, "listfield");
            }

            if ((EwavColumn)cbxLatitude.SelectedItem != null)
            {
                gadgetOptions.InputVariableList.Add("laty", ((EwavColumn)cbxLatitude.SelectedItem).Name);
                gadgetOptions.InputVariableList.Add(((EwavColumn)cbxLatitude.SelectedItem).Name, "listfield");
            }


            if ((EwavColumn)cbxStratify.SelectedItem != null)
            {
                gadgetOptions.InputVariableList.Add(((EwavColumn)cbxStratify.SelectedItem).Name, "listfield");
            }





        }

        /// <summary>
        /// Determines whether {6CBDD833-7AC5-4ED2-BC4D-FC83C448DFEC}[is DF used in this gadget].
        /// </summary>
        /// <returns></returns>
        public bool IsDFUsedInThisGadget()
        {
            if (cbxLatitude.SelectedIndex > 0 && ((EwavColumn)cbxLatitude.SelectedItem).IsUserDefined)
            {
                //DFInUse = ((EwavColumn)cbxGroupField.SelectedItem);
                return true;
            }
            if (cbxLongitude.SelectedIndex > 0 && ((EwavColumn)cbxLongitude.SelectedItem).IsUserDefined)
            {
                //DFInUse = ((EwavColumn)cbxGroupField.SelectedItem);
                return true;
            }
            if (cbxStratify.SelectedIndex > 0 && ((EwavColumn)cbxStratify.SelectedItem).IsUserDefined)
            {
                //DFInUse = ((EwavColumn)cbxGroupField.SelectedItem);
                return true;
            }
            return false;
        }

        public StringBuilder HtmlBuilder { get; set; }

        /// <summary>
        /// Refreshes the results.
        /// </summary>
        public void RefreshResults()
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the gadget's state to 'finished with error' mode
        /// </summary>
        /// <param name="errorMessage">The error message to display</param>
        public void RenderFinishWithError(string errorMessage)
        {

            waitCursor.Visibility = Visibility.Collapsed;
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            txtStatus.Visibility = System.Windows.Visibility.Visible;
            txtStatus.Text = errorMessage;
            txtStatus.Visibility = System.Windows.Visibility.Visible;
            pnlStatus.Visibility = System.Windows.Visibility.Visible;
            panelMain.Visibility = System.Windows.Visibility.Collapsed;
        }


        public void RenderFinish()
        {

            gadgetExpander.IsExpanded = false;

            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            txtStatus.Visibility = System.Windows.Visibility.Collapsed;

            pnlStatus.Visibility = System.Windows.Visibility.Collapsed;
            panelMain.Visibility = System.Windows.Visibility.Visible;
            FilterButton.IsEnabled = true;


        }

        /// <summary>
        /// Resets the gadget to initial state.
        /// </summary>
        public void ResetGadget()
        {
            SearchIndex();

            //if (IsDFUsedInThisGadget())
            //{
            Index1 = Index2 = Index3 = -1;
            panelMain.Children.Clear();
            panelMain.Visibility = System.Windows.Visibility.Collapsed;
            pnlStatus.Visibility = System.Windows.Visibility.Collapsed;
            this.cbxLatitude.ItemsSource = null;
            this.cbxLongitude.ItemsSource = null;
            this.cbxStratify.ItemsSource = null;

            LoadingDropDowns = true;
            PopulateControlsWithData();
            LoadingDropDowns = false;
        }

        /// <summary>
        /// Saves the column values.
        /// </summary>
        public void SaveColumnValues()
        {
            Col1 = (EwavColumn)cbxLatitude.SelectedItem;
            Col2 = (EwavColumn)cbxLongitude.SelectedItem;
            Col3 = (EwavColumn)cbxStratify.SelectedItem;
        }

        /// <summary>
        /// Searches current index of the columns.
        /// </summary>
        public void SearchIndex()
        {
            ClientCommon.Common CommonClass = new ClientCommon.Common();

            Index1 = CommonClass.SearchCurrentIndex(Col1, CommonClass.GetItemsSource(GetLatLonPrimaryDataType, false));



            Index2 = CommonClass.SearchCurrentIndex(Col2, CommonClass.GetItemsSource(GetLatLonPrimaryDataType, false));

            Index3 = CommonClass.SearchCurrentIndex(Col3, CommonClass.GetItemsSource(GetFieldPrimaryDataType, true));
        }

        /// <summary>
        /// Selects the drop down values.
        /// </summary>
        public void SelectDropDownValues()
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serializes the specified doc.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <returns></returns>
        public XNode Serialize(XDocument doc)
        {
            try
            {
                map1.ZoomTo(map1.Extent);

                element = new XElement("gadget",
                    new XAttribute("top", Canvas.GetTop(this).ToString("F0")),
                    new XAttribute("left", Canvas.GetLeft(this).ToString("F0")),
                    new XAttribute("collapsed", "false"),
                    new XAttribute("gadgetType", "Ewav.MapControl"),
                    new XElement("latitude", ((EwavColumn)cbxLatitude.SelectedItem).Name),
                    new XElement("longitude", ((EwavColumn)cbxLongitude.SelectedItem).Name),
                    new XElement("stratifyby", cbxStratify.SelectedItem == null ? "" : ((EwavColumn)cbxStratify.SelectedItem).Name),
                    new XElement("Radius", udRadius.Value),
                    new XElement("ClusterSize", udClusterSize.Value),
                    new XElement("MapWidth", udWidth.Value),
                    new XElement("MapHeight", udHeight.Value),
                    new XElement("RoadsSelected", rbRoads.IsChecked),
                    new XElement("SatelliteSelected", rbSatellite.IsChecked),
                         new XElement("gadgetTitle", GName.Text),
            new XElement("gadgetDescription", txtGadgetDescription.Text)

                    );

                if (this.GadgetFilters != null)
                {
                    this.GadgetFilters.Serialize(element);
                }
                return element;

            }
            catch (Exception e)
            {


                throw new Exception();



            }
        }

        /// <summary>
        /// Sets the state of the gadget to finished.
        /// </summary>
        public void SetGadgetToFinishedState()
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the state of the gadget to processing.
        /// </summary>
        public void SetGadgetToProcessingState()
        {
            // TODO: Implement this method
            throw new NotImplementedException();
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

            HtmlBuilder = new StringBuilder();


            HtmlBuilder.Append("<hr>Map HTML Not Implemented </h2> ");

            return "Map HTML Not Implemented ";

        }

        /// <summary>
        /// Unloads the gadget.
        /// </summary>
        public void UnloadGadget()
        {
            applicationViewModel.ApplyDataFilterEvent -= new Client.Application.ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);

            applicationViewModel.DefinedVariableAddedEvent -= new Client.Application.DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);
            applicationViewModel.DefinedVariableInUseDeletedEvent -= new Client.Application.DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableInUseDeletedEvent);
            applicationViewModel.DefinedVariableNotInUseDeletedEvent -= new Client.Application.DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
            applicationViewModel.PreColumnChangedEvent -= new Client.Application.PreColumnChangedEventHandler(applicationViewModel_PreColumnChangedEvent);
            mcviewModel.MapDataLoadedEvent -= new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(mcviewModel_MapDataLoadedEvent);
        }

        /// <summary>
        /// Updates the variable names.
        /// </summary>
        public void UpdateVariableNames()
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        private void btnLegend_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            if (Legend1.Visibility == System.Windows.Visibility.Collapsed)
            {
                // Legend1.Visibility = System.Windows.Visibility.Visible;
                Grow.Begin();
                //storyboardFlag = false;
            }
            else
            {
                Shrink.Begin();
                //storyboardFlag = true;
            }
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            waitCursor.Visibility = Visibility.Visible;
            DoMap();
            gadgetExpander.IsExpanded = false;
        }



        private void btnToggleView_ViewChanged(object sender, RoutedEventArgs e)
        {
            this.ViewChanged();
        }

        private void button1_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        private void CloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CloseGadgetOnClick();
        }

        /// <summary>
        /// Does the map.
        /// </summary>
        private void DoMap()
        {
            //   this.gp.DatasourceName = this.applicationViewModel.EwavSelectedDatasource.DatasourceName;
            IsUserDefindVariableInUse();
            CreateInputVariableList();


            mcviewModel = DataContext as MapControlViewModel;

            mcviewModel.LoadMapData(gadgetOptions);
        }

        /// <summary>
        /// Determines whether is user defind variable in use].
        /// </summary>
        private void IsUserDefindVariableInUse()
        {

            if (cbxLatitude.SelectedIndex > -1 && ((EwavColumn)cbxLatitude.SelectedItem).IsUserDefined)
            {
                ((EwavColumn)cbxLatitude.SelectedItem).IsInUse = true;
            }

            if (cbxLongitude.SelectedIndex > -1 && ((EwavColumn)cbxLongitude.SelectedItem).IsUserDefined)
            {
                ((EwavColumn)cbxLatitude.SelectedItem).IsInUse = true;
            }

            if (cbxStratify.SelectedIndex > 0 && ((EwavColumn)cbxStratify.SelectedItem).IsUserDefined)
            {
                ((EwavColumn)cbxStratify.SelectedItem).IsInUse = true;
            }
        }


        /// <summary>
        /// Closes the gadget.
        /// </summary>
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
        /// Refreshes the results.
        /// </summary>
        void IGadget.RefreshResults()
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }


        void map1_ExtentChanged(object sender, ExtentEventArgs e)
        {

        }

        //private void (object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        private bool isMouseLeftButtonDown;
        private double originalMouseLeftButtonDownXCoord;
        private double originalMouseLeftButtonDownYCoord;
        //Ref Members
        private MapPoint originalMapPoint;

        private void map1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isMouseLeftButtonDown = true;

            originalMapPoint = map1.Extent.GetCenter();
            MapPoint mp = map1.ScreenToMap(new Point(e.GetSafePosition(map1).X, e.GetSafePosition(map1).Y));

            originalMouseLeftButtonDownXCoord = mp.X;
            originalMouseLeftButtonDownYCoord = mp.Y;
        }
        private void map1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMouseLeftButtonDown = false;
        }
        private void map1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseLeftButtonDown)
            {
                Point p = new Point(e.GetSafePosition(map1).X, e.GetSafePosition(map1).Y);
                MapPoint mp = map1.ScreenToMap(p);

                double xDiff = originalMouseLeftButtonDownXCoord - mp.X;
                double yDiff = originalMouseLeftButtonDownYCoord - mp.Y;

                MapPoint newPoint = new MapPoint(originalMapPoint.X + xDiff, originalMapPoint.Y + yDiff);

                map1.PanDuration = new TimeSpan(0, 0, 0, 0, 500);
                map1.PanTo(newPoint);
            }
        }

        void MapControl_Loaded(object sender, RoutedEventArgs e)
        {
            mcviewModel = (MapControlViewModel)this.DataContext;
            mcviewModel.ErrorNotice += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(mmviewModel_ErrorNotice);

            applicationViewModel.ApplyDataFilterEvent += new Client.Application.ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);
            applicationViewModel.DefinedVariableAddedEvent += new Client.Application.DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);

            applicationViewModel.DefinedVariableInUseDeletedEvent += new Client.Application.DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableInUseDeletedEvent);
            applicationViewModel.DefinedVariableNotInUseDeletedEvent += new Client.Application.DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
            applicationViewModel.PreColumnChangedEvent += new Client.Application.PreColumnChangedEventHandler(applicationViewModel_PreColumnChangedEvent);

            applicationViewModel.UnloadedEvent += new Client.Application.UnloadedEventHandler(applicationViewModel_UnloadedEvent);

            mcviewModel.MapDataLoadedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(mcviewModel_MapDataLoadedEvent);

            map1.ExtentChanged += new EventHandler<ExtentEventArgs>(map1_ExtentChanged);

            map1.Layers.LayersInitialized += new LayerCollection.LayersInitializedHandler(Layers_LayersInitialized);

        }

        void mcviewModel_MapDataLoadedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            if (e != null)
            {
                List<PointDTOCollection> pdc = ((MapControlViewModel)sender).ResultCollection;             //    (List<PointDTOCollection>)sender;


                if (pdc.Count == 0)
                {
                    RenderFinishWithError("No records match the selection criteria. ");
                    return;
                }

                if (pdc.Count == 1 && pdc[0].Collection.Count == 0)
                {
                    RenderFinishWithError("No records match the selection criteria. ");
                    return;
                }


                if (pdc.Count == 1)
                {
                    btnLegend.Visibility = System.Windows.Visibility.Collapsed;
                    Legend1.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    btnLegend.Visibility = System.Windows.Visibility.Visible;
                }


                map1.Width = udWidth.Value;
                map1.Height = udHeight.Value;

                bool useRoads = Convert.ToBoolean(rbRoads.IsChecked);

                EwavMap ewavMap = new EwavMap(this.map1, Convert.ToInt32(udRadius.Value),
                    Convert.ToInt32(udClusterSize.Value), useRoads);


                ewavMap.AddPointsAsManyGraphicsLayers(pdc);

                makeEwavLegend(ewavMap.EwavLegendItemList);


                RenderFinish();

            }
        }

        void Layers_LayersInitialized(object sender, EventArgs args)
        {

            waitCursor.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Makes the ewav legend.
        /// </summary>
        /// <param name="ewavLegendItemList">The ewav legend item list.</param>
        private void makeEwavLegend(List<EwavLegendItemData> ewavLegendItemList)
        {
            try
            {
                Legend2.Children.Clear();

                StackPanel stackPanel;
                StackPanel stackPanelTitle;

                stackPanelTitle = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left
                };


                if (cbxStratify.SelectedItem != null)
                {
                    TextBlock textBlockTitle1 = new TextBlock()
                    {
                        Text = "Stratify by:  ",
                        FontWeight = FontWeights.SemiBold
                    };

                    TextBlock textBlockTitle2 = new TextBlock()
                    {
                        Text = ((EwavColumn)cbxStratify.SelectedItem).Name,
                        FontWeight = FontWeights.SemiBold
                    };

                    stackPanelTitle.Children.Add(textBlockTitle1);
                    stackPanelTitle.Children.Add(textBlockTitle2);

                    Legend2.Children.Add(stackPanelTitle);
                }


                foreach (EwavLegendItemData eli in ewavLegendItemList)
                {
                    Ellipse ellipse = new Ellipse()
                    {
                        Width = 15,
                        Height = 15,
                        Stroke = eli.Color,
                        Fill = eli.Color,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    ellipse.SetValue(Grid.RowProperty, 0);
                    ellipse.SetValue(Grid.ColumnProperty, 0);

                    stackPanel = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                        Height = 20,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center
                    };

                    TextBlock tb = new TextBlock()
                    {
                        Text = eli.Description,
                        Margin = new Thickness(5, 0, 0, 0)
                    };

                    stackPanel.Children.Add(ellipse);
                    stackPanel.Children.Add(tb);

                    Legend2.Children.Add(stackPanel);
                    //  Legend1.Child = Legend2;    

                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error with makeLegend");
            }

        }

        void mmviewModel_ErrorNotice(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            RenderFinishWithError(e.Data.Message);
        }

        private void PopulateControlsWithData()
        {
            cbxLatitude.ItemsSource = null;
            cbxLatitude.Items.Clear();
            List<EwavColumn> SourceColumns = this.applicationViewModel.EwavSelectedDatasource.AllColumns;
            IEnumerable<EwavColumn> CBXFieldCols = from cols in SourceColumns
                                                   where GetLatLonPrimaryDataType.Contains(cols.SqlDataTypeAsString)
                                                   orderby cols.Name
                                                   select cols;

            List<EwavColumn> colsList = CBXFieldCols.ToList();

            this.cbxLatitude.ItemsSource = colsList;
            this.cbxLatitude.SelectedValue = "Index";
            this.cbxLatitude.DisplayMemberPath = "Name";
            this.cbxLatitude.SelectedIndex = Index1;

            cbxLongitude.ItemsSource = null;
            cbxLongitude.Items.Clear();
            //SourceColumns = this.applicationViewModel.EwavSelectedDatasource.AllColumns;
            //CBXFieldCols = from cols in SourceColumns
            //               where GetLatLonPrimaryDataType.Contains(cols.SqlDataTypeAsString)
            //               orderby cols.Name
            //               select cols;

            //colsList = CBXFieldCols.ToList();

            this.cbxLongitude.ItemsSource = colsList;
            this.cbxLongitude.SelectedValue = "Index";
            this.cbxLongitude.DisplayMemberPath = "Name";
            this.cbxLongitude.SelectedIndex = Index2;

            GuessCols();

            cbxStratify.ItemsSource = null;
            cbxStratify.Items.Clear();
            SourceColumns = this.applicationViewModel.EwavSelectedDatasource.AllColumns;
            CBXFieldCols = from cols in SourceColumns
                           where GetFieldPrimaryDataType.Contains(cols.SqlDataTypeAsString)
                           orderby cols.Name
                           select cols;

            colsList = CBXFieldCols.ToList();
            colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            this.cbxStratify.ItemsSource = colsList;
            this.cbxStratify.SelectedValue = "Index";
            this.cbxStratify.DisplayMemberPath = "Name";
            this.cbxStratify.SelectedIndex = Index3;
        }

        private void GuessCols()
        {
            List<string> LongitudeCandidates = new List<string>();
            List<string> LatitudeCandidates = new List<string>();

            int x;

            LongitudeCandidates.Add("lng");
            LongitudeCandidates.Add("longitude");
            LongitudeCandidates.Add("longx");
            LongitudeCandidates.Add("lonX");
            LongitudeCandidates.Add("X");

            LatitudeCandidates.Add("lat");
            LatitudeCandidates.Add("latitude");
            LatitudeCandidates.Add("laty");
            LatitudeCandidates.Add("y");

            for (int i = 0; i < cbxLongitude.Items.Count; i++)
            {
                string colName = ((EwavColumn)cbxLongitude.Items[i]).Name;

                //x = LongitudeCandidates.IndexOf(colName.ToLower());
                //if (x > -1)
                //    cbxLongitude.SelectedIndex = i;

                if (LongitudeCandidates.Contains(colName.ToLower()))
                {
                    cbxLongitude.SelectedIndex = i;
                }
            }

            for (int i = 0; i < cbxLatitude.Items.Count; i++)
            {
                string colName = ((EwavColumn)cbxLatitude.Items[i]).Name;


                if (LatitudeCandidates.Contains(colName.ToLower()))
                {
                    cbxLatitude.SelectedIndex = i;
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

        void TestDataMap()
        {
            List<PointDTO> pointDTOList1 = new List<PointDTO>();
            List<PointDTO> pointDTOList2 = new List<PointDTO>();
            List<PointDTO> pointDTOList3 = new List<PointDTO>();

            List<List<PointDTO>> pointDTOList = new List<List<PointDTO>>();

            List<UniqueValueInfoDTO> uniqueValueInfoDTOList = new List<UniqueValueInfoDTO>();

            pointDTOList2.Add(new PointDTO() { LatY = 11.151, LonX = 104.40, MapTip = "aaaa", StrataValue = "doctor" });
            pointDTOList2.Add(new PointDTO() { LatY = 10.982, LonX = 106.31, MapTip = "dddd", StrataValue = "doctor" });
            pointDTOList2.Add(new PointDTO() { LatY = 11.030, LonX = 106.75, MapTip = "ffff", StrataValue = "doctor" });
            pointDTOList2.Add(new PointDTO() { LatY = 10.847, LonX = 106.75, MapTip = "hhhh", StrataValue = "doctor" });
            pointDTOList2.Add(new PointDTO() { LatY = 10.982, LonX = 106.31, MapTip = "dddd", StrataValue = "doctor" });
            pointDTOList2.Add(new PointDTO() { LatY = 11.030, LonX = 106.75, MapTip = "ffff", StrataValue = "doctor" });
            pointDTOList2.Add(new PointDTO() { LatY = 10.847, LonX = 106.75, MapTip = "hhhh", StrataValue = "doctor" });

            pointDTOList.Add(pointDTOList2);

            pointDTOList1.Add(new PointDTO() { LatY = 11.251, LonX = 104.70, MapTip = "aaaa", StrataValue = "nurse" });
            pointDTOList1.Add(new PointDTO() { LatY = 10.782, LonX = 106.91, MapTip = "dddd", StrataValue = "nurse" });
            pointDTOList1.Add(new PointDTO() { LatY = 11.130, LonX = 106.45, MapTip = "ffff", StrataValue = "nurse" });
            pointDTOList1.Add(new PointDTO() { LatY = 10.130, LonX = 107.45, MapTip = "ffff", StrataValue = "nurse" });
            pointDTOList1.Add(new PointDTO() { LatY = 10.947, LonX = 106.15, MapTip = "hhhh", StrataValue = "nurse" });

            pointDTOList.Add(pointDTOList1);

            pointDTOList3.Add(new PointDTO() { LatY = 11.251, LonX = 104.70, MapTip = "aaaa", StrataValue = "Patient" });
            pointDTOList3.Add(new PointDTO() { LatY = 10.782, LonX = 106.91, MapTip = "dddd", StrataValue = "Patient" });
            pointDTOList3.Add(new PointDTO() { LatY = 11.130, LonX = 106.45, MapTip = "ffff", StrataValue = "Patient" });
            pointDTOList3.Add(new PointDTO() { LatY = 10.130, LonX = 107.45, MapTip = "ffff", StrataValue = "Patient" });
            pointDTOList3.Add(new PointDTO() { LatY = 10.947, LonX = 106.15, MapTip = "hhhh", StrataValue = "Patient" });
            pointDTOList3.Add(new PointDTO() { LatY = 10.782, LonX = 106.91, MapTip = "dddd", StrataValue = "Patient" });
            pointDTOList3.Add(new PointDTO() { LatY = 11.130, LonX = 106.45, MapTip = "ffff", StrataValue = "Patient" });
            pointDTOList3.Add(new PointDTO() { LatY = 10.130, LonX = 107.45, MapTip = "ffff", StrataValue = "Patient" });
            pointDTOList3.Add(new PointDTO() { LatY = 10.947, LonX = 106.15, MapTip = "hhhh", StrataValue = "Patient" });

            pointDTOList.Add(pointDTOList3);

            uniqueValueInfoDTOList.Add(new UniqueValueInfoDTO()
            {
                Description = "doctor",
                Label = "doctor",
                Symbol = "doctor",
                Value = "doctor"
            });

            uniqueValueInfoDTOList.Add(new UniqueValueInfoDTO()
            {
                Description = "nurse",
                Label = "nurse",
                Symbol = "nurse",
                Value = "nurse"
            });

            uniqueValueInfoDTOList.Add(new UniqueValueInfoDTO()
            {
                Description = "Patient",
                Label = "Patient",
                Symbol = "Patient",
                Value = "Patient"
            });

            GraphicsLayer graphicsLayer = map1.Layers["graphicsLayer"] as GraphicsLayer;
            //   EwavMap ewavMap = new EwavMap(map1, graphicsLayer,  "1101");
            //     ewavMap.AddPointsAsManyGraphicsLayers(pointDTOList);
            //   Legend1.Map = map1;
            //    ewavMap.AddPointsAsManyGraphicsLayers(pointDTOList);     
        }

        private void ViewChanged()
        {
            //switch (this.btnToggleView.IsChecked)
            //{
            //    case true:
            //        this.btnToggleView.Content = "Road View";
            //        break;
            //    case false:
            //        this.btnToggleView.Content = "Satellite View";
            //        break;
            //}
        }

        /// <summary>
        /// Gets or sets the custom output caption.
        /// </summary>
        /// <value>The custom output caption.</value>
        public string CustomOutputCaption
        {
            get
            {
                // TODO: Implement this property getter
                throw new NotImplementedException();
            }
            set
            {
                // TODO: Implement this property setter
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets or sets the custom output description.
        /// </summary>
        /// <value>The custom output description.</value>
        public string CustomOutputDescription
        {
            get
            {
                // TODO: Implement this property getter
                throw new NotImplementedException();
            }
            set
            {
                // TODO: Implement this property setter
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets or sets the custom output heading.
        /// </summary>
        /// <value>The custom output heading.</value>
        public string CustomOutputHeading
        {
            get
            {
                // TODO: Implement this property getter
                throw new NotImplementedException();
            }
            set
            {
                // TODO: Implement this property setter
                throw new NotImplementedException();
            }
        }

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

        public List<ColumnDataType> GetLatLonPrimaryDataType
        {
            get
            {
                latLonDataTypesList = new List<ColumnDataType>();

                latLonDataTypesList.Add(ColumnDataType.Numeric);

                return latLonDataTypesList;
            }
        }





        /// <summary>
        /// Gets or sets the is processing.
        /// </summary>
        /// <value>The is processing.</value>
        public bool IsProcessing
        {
            get
            {
                // TODO: Implement this property getter
                throw new NotImplementedException();
            }
            set
            {
                // TODO: Implement this property setter
                throw new NotImplementedException();
            }
        }

        public bool LoadingCanvas { get; set; }

        /// <summary>
        /// The value for the frameworkelement.Name property
        /// </summary>
        /// <value></value>
        public string MyControlName
        {
            get
            {
                return "MapControl";
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
                return "Case Cluster Map";
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            applicationViewModel.IsMouseDownOnMap = true;
        }

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //e.Handled = true;
        }

        private void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //e.Handled = true;
        }

        private void LayoutRoot_MouseMove(object sender, MouseEventArgs e)
        {
            // e.Handled = true;
        }


        public void Reload()
        {





            waitCursor.Visibility = Visibility.Visible;
            DoMap();
            gadgetExpander.IsExpanded = false;




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
                DoMap();
            }
        }






    }
}

namespace Ewav.Web.Services.MapCluster
{
    public partial class MapClusterDomainContext
    {
        //This is the place to set RIA Services query timeout. 
        //TimeSpan(0, 5, 0) means five minutes vs the 
        //default 60 sec
        partial void OnCreated()
        {
            if (!DesignerProperties.GetIsInDesignMode(Application.
                Current.RootVisual))
                ((System.ServiceModel.DomainServices.Client.WebDomainClient<IMapClusterDomainServiceContract>)
                    DomainClient).ChannelFactory.Endpoint.Binding.SendTimeout =
                    new TimeSpan(0, 120, 0);
        }
    }
}
