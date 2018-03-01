/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       LineList.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
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
using Ewav.ViewModels.Gadgets;
using Ewav.Web.Services;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Ewav.ViewModels;
using Ewav.Web.EpiDashboard;
using Ewav.BAL;
using System.ComponentModel;
using Ewav.ExtensionMethods;
using System.ServiceModel.DomainServices.Client;
using System.Text;
using System.Xml.Linq;
using Ewav.ExtensionMethods;
using Ewav.Views.Dialogs;

namespace Ewav
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(UserControl))]
    [ExportMetadata("type", "gadget")]
    [ExportMetadata("tabindex", "0")]
    public partial class LineList : UserControl, IGadget, IEwavGadget,  ICustomizableGadget     
    {
        LineListViewModel llviewModel;
        private GadgetParameters gadgetOptions;
        private List<EwavColumn> selectedItems = new List<EwavColumn>(), sortingItems = new List<EwavColumn>();
        int Index1 = -1, Index2 = -1, Index3 = -1;
        EwavColumn Col1, Col2, Col3, DFInUse = null;        

        ClientCommon.Common cmnClass = new ClientCommon.Common();
        private bool loadingDropDowns = false;

       /// <summary>
        /// Gets or sets the set labels popup.
        /// </summary>
        /// <value>The set labels popup.</value>
        public SetLabels setLabelsPopup { get; set; }
        /// <summary>
        /// Container that holds gadget level filters.
        /// </summary>
        public List<EwavDataFilterCondition> GadgetFilters { get; set; }

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
                fieldPrimaryDataTypesList.Add(ColumnDataType.GroupVariable);
                return fieldPrimaryDataTypesList;
            }
        }
        public LineList()
        {
            InitializeComponent();

            // Init gadget parameters with default values
            gadgetOptions = new GadgetParameters();
            gadgetOptions.ShouldIncludeFullSummaryStatistics = true;
            gadgetOptions.ShouldIncludeMissing = false;
            gadgetOptions.ShouldSortHighToLow = false;
            gadgetOptions.ShouldUseAllPossibleValues = false;
            gadgetOptions.StrataVariableNames = new List<string>();

            this.Loaded += new RoutedEventHandler(LineList_Loaded);
            //initializeControl();
            PopulateControlsWithData();


        }

        void LineList_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeControl();
        }

        private void InitializeControl()
        {
            llviewModel = (LineListViewModel)this.DataContext;
            llviewModel.TableLoadedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(llviewModel_TableLoadedEvent);
            llviewModel.ErrorNotice += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(llviewModel_ErrorNotice);


            applicationViewModel.ApplyDataFilterEvent += new Client.Application.ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);
            applicationViewModel.DefinedVariableAddedEvent += new Client.Application.DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);

            applicationViewModel.DefinedVariableInUseDeletedEvent += new Client.Application.DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableInUseDeletedEvent);
            applicationViewModel.DefinedVariableNotInUseDeletedEvent += new Client.Application.DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
            applicationViewModel.PreColumnChangedEvent += new Client.Application.PreColumnChangedEventHandler(applicationViewModel_PreColumnChangedEvent);

            applicationViewModel.UnloadedEvent += new Client.Application.UnloadedEventHandler(applicationViewModel_UnloadedEvent);
            //Construct();
            //  lrViewModel.GetColumns("NEDS", "vwExternalData");    

            // FillDropDowns();
            //PopulateControlsWithData();
        }

        private void PopulateControlsWithData()
        {
            lbxFields.ItemsSource = null;
            lbxFields.Items.Clear();
            lbxSortFields.Items.Clear();
            List<EwavColumn> SourceColumns = this.applicationViewModel.EwavSelectedDatasource.AllColumns;
            IEnumerable<EwavColumn> CBXFieldCols = from cols in SourceColumns
                                                   where GetFieldPrimaryDataType.Contains(cols.SqlDataTypeAsString)
                                                   orderby cols.Name
                                                   select cols;

            List<EwavColumn> colsList = CBXFieldCols.ToList();

            //colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            this.lbxFields.ItemsSource = colsList;
            this.lbxFields.SelectedValue = "Index";
            this.lbxFields.DisplayMemberPath = "Name";
            this.lbxFields.SelectedIndex = -1;
            //lbxFields.SelectedIndex = Index1;

            for (int i = 0; i < selectedItems.Count; i++)
            {
                lbxFields.SelectedItems.Add(selectedItems[i]);
            }

             CBXFieldCols = from cols in SourceColumns
                                                   where (GetFieldPrimaryDataType.Contains(cols.SqlDataTypeAsString) &&
                                                   cols.SqlDataTypeAsString != ColumnDataType.GroupVariable)
                                                   orderby cols.Name
                                                   select cols;

             colsList = CBXFieldCols.ToList();

            List<EwavColumn> cbxsortList = CBXFieldCols.ToList();
            cbxsortList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            cbxSortField.ItemsSource = cbxsortList;
            this.cbxSortField.SelectedValue = "Index";
            this.cbxSortField.DisplayMemberPath = "Name";
            cbxSortField.SelectedIndex = Index2;

            List<EwavColumn> cbxGrpList = CBXFieldCols.ToList();
            cbxGrpList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            cbxGroupField.ItemsSource = cbxGrpList;
            this.cbxGroupField.SelectedValue = "Index";
            this.cbxGroupField.DisplayMemberPath = "Name";
            cbxGroupField.SelectedIndex = Index3;


            this.lbxSortFields.SelectedValue = "Index";
            this.lbxSortFields.DisplayMemberPath = "Name";
            for (int i = 0; i < sortingItems.Count; i++)
            {
                lbxSortFields.Items.Add(sortingItems[i]);
            }

            selectedItems.Clear();
            sortingItems.Clear();

        }


        /// <summary>
        /// Associates the event handlers with Events. 
        /// </summary>
        private void Construct()
        {

        }

        private void UnloadGadget()
        {
            applicationViewModel.ApplyDataFilterEvent -= new Client.Application.ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);

            applicationViewModel.DefinedVariableAddedEvent -= new Client.Application.DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);
            applicationViewModel.DefinedVariableInUseDeletedEvent -= new Client.Application.DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableInUseDeletedEvent);
            applicationViewModel.DefinedVariableNotInUseDeletedEvent -= new Client.Application.DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
            applicationViewModel.PreColumnChangedEvent -= new Client.Application.PreColumnChangedEventHandler(applicationViewModel_PreColumnChangedEvent);
        }

        void applicationViewModel_UnloadedEvent(object o)
        {
            UnloadGadget();
        }

        void applicationViewModel_PreColumnChangedEvent(object o)
        {
            SaveColumnValues();
        }

        void applicationViewModel_DefinedVariableNotInUseDeletedEvent(object o)
        {
            SearchIndex();
            LoadingDropDowns = true;
            PopulateControlsWithData();
            LoadingDropDowns = false;
            DoLineList();
        }

        private void DoLineList()
        {
            if (this.lbxFields.SelectedItems.Count < 1)
            {
                return;
            }

            this.gadgetOptions.DatasourceName = this.applicationViewModel.EwavSelectedDatasource.DatasourceName;
            IsUserDefindVariableInUse();
            CreateInputVariableList();

            if (llviewModel == null)
            {
                llviewModel = (LineListViewModel)this.DataContext;
            }

            llviewModel.GetLineList(gadgetOptions);
        }

        /// <summary>
        /// Determines whether is user defind variable in use].
        /// </summary>
        private void IsUserDefindVariableInUse()
        {
            for (int i = 0; i < lbxFields.SelectedItems.Count; i++)
            {
                if (((EwavColumn)lbxFields.SelectedItems[i]).IsUserDefined)
                {
                    ((EwavColumn)lbxFields.SelectedItems[i]).IsInUse = true;
                }
            }

            for (int i = 0; i < lbxSortFields.Items.Count; i++)
            {
                if (((EwavColumn)lbxSortFields.Items[i]).IsUserDefined)
                {
                    ((EwavColumn)lbxSortFields.Items[i]).IsInUse = true;
                }
            }

            if (cbxGroupField.SelectedIndex > -1 && ((EwavColumn)cbxGroupField.SelectedItem).IsUserDefined)
            {
                ((EwavColumn)cbxGroupField.SelectedItem).IsInUse = true;
            }
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
            Index1 = Index2 = Index3 = -1;
            panelMain.Children.Clear();
            panelMain.Visibility = System.Windows.Visibility.Collapsed;
            pnlStatus.Visibility = System.Windows.Visibility.Collapsed;
            this.lbxFields.ItemsSource = null;
            //this.lbxSortFields.Items.Clear();
            this.cbxGroupField.ItemsSource = null;
            this.sortingItems.Clear();
            }
            LoadingDropDowns = true;
            PopulateControlsWithData();
            LoadingDropDowns = false;

        }

        private bool IsDFUsedInThisGadget()
        {
            //bool dfInUse = false;

            //for (int i = 0; i < lbxFields.SelectedItems.Count; i++)
            //{
            //    if (((EwavColumn)lbxFields.SelectedItems[i]).IsUserDefined)
            //    {
            //        //ret((EwavColumn)lbxFields.SelectedItems[i]);
            //        return true;
            //    }
            //}

            //for (int i = 0; i < lbxSortFields.Items.Count; i++)
            //{
            //    if (((EwavColumn)lbxSortFields.SelectedItems[i]).IsUserDefined)
            //    {
            //        //DFInUse = ((EwavColumn)lbxSortFields.SelectedItems[i]);
            //        return true;
            //    }
            //}

            //if (cbxGroupField.SelectedIndex > 0 && ((EwavColumn)cbxGroupField.SelectedItem).IsUserDefined)
            //{
            //    //DFInUse = ((EwavColumn)cbxGroupField.SelectedItem);
            //    return true;
            //}
            //return false;

            return Col1 != null && Col1.Name == applicationViewModel.ItemToBeRemoved.Name ||
                           Col2 != null && Col2.Name == applicationViewModel.ItemToBeRemoved.Name ||
                           Col3 != null && Col3.Name == applicationViewModel.ItemToBeRemoved.Name;
                           
        }

        void applicationViewModel_DefinedVariableAddedEvent(object o)
        {
            SearchIndex();
            LoadingDropDowns = true;
            PopulateControlsWithData();
            LoadingDropDowns = false;
            DoLineList();  
        }

        private void SaveColumnValues()
        {
            Col1 = (EwavColumn)lbxFields.SelectedItem;
            Col2 = (EwavColumn)cbxSortField.SelectedItem;

            for (int i = 0; i < lbxFields.SelectedItems.Count; i++)
            {
                selectedItems.Add(((EwavColumn)lbxFields.SelectedItems[i]));
            }

            for (int i = 0; i < lbxSortFields.Items.Count; i++)
            {
                sortingItems.Add(((EwavColumn)lbxSortFields.Items[i]));
            }

            Col3 = (EwavColumn)cbxGroupField.SelectedItem;
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

            Index1 = CommonClass.SearchCurrentIndex(Col1, CommonClass.GetItemsSource(GetFieldPrimaryDataType));



            Index2 = CommonClass.SearchCurrentIndex(Col2, CommonClass.GetItemsSource(GetFieldPrimaryDataType, true));

            Index3 = CommonClass.SearchCurrentIndex(Col3, CommonClass.GetItemsSource(GetFieldPrimaryDataType, true));

        }
        void applicationViewModel_ApplyDataFilterEvent(object o)
        {
            //bool dfColFound = false;

            //if (applicationViewModel.RemoveIndicator &&
            //    DFInUse != null)
            //{
            //    foreach (var item in DFInUse.Name.Split('*'))
            //    {
            //        if (applicationViewModel.ItemToBeRemoved.Name == item)
            //        {
            //            dfColFound = true;
            //            //return;
            //        }
            //    }
            //}


            //if ((applicationViewModel.ItemToBeRemoved != null) &&
            //   DFInUse != null &&
            //   dfColFound)
            if (applicationViewModel.RemoveIndicator &&
                (IsDFUsedInThisGadget()))
            {
                ResetGadget();
            }
            else
            {
                waitCursor.Visibility = Visibility.Visible;
                DoLineList();
                gadgetExpander.IsExpanded = false;
            }
        }

        void llviewModel_ErrorNotice(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            //if (e.Data.Message.Length > 0)
            //{
            //    ChildWindow window = new ErrorWindow(e.Data);
            //    window.Show();
            //    //return;
            //}
            RenderFinishWithError(e.Data.Message);
        }

        private void SelectDropDownValues()
        {
            if (Index2 > -1)
            {
                cbxGroupField.SelectedIndex = Index2;
            }

            if (Index3 > -1)
            {
                cbxSortField.SelectedIndex = Index3;
            }
        }


        XElement element = null;

        public StringBuilder HtmlBuilder { get; set; }

        public void RefreshResults()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Generates Xml representation of this gadget
        /// </summary>
        /// <param name="doc">The Xml docment</param>
        /// <returns>XmlNode</returns>
        public System.Xml.Linq.XNode Serialize(System.Xml.Linq.XDocument doc)
        {
            Dictionary<string, string> inputVariableList = gadgetOptions.InputVariableList;
            //string customusercolumnsort;
            string groupVar = string.Empty;

            if (cbxGroupField.SelectedItem != null)
            {
                groupVar = ((EwavColumn)cbxGroupField.SelectedItem).Name.ToString().Replace("<", "&lt;");
            }

            //CustomOutputHeading = headerPanel.Text;
            ////CustomOutputDescription = txtOutputDescription.Text.Replace("<", "&lt;");
            //CustomOutputDescription = descriptionPanel.Text; //txtOutputDescription.Text.Replace("<", "&lt;");

            //string xmlString =
            //"<groupVariable>" + groupVar + "</groupVariable>" +
            //"<maxRows>" + MaxRows.ToString() + "</maxRows>" +
            //"<maxColumnNameLength>" + MaxColumnLength.ToString() + "</maxColumnNameLength>" +
            //"<sortColumnsByTabOrder>" + checkboxTabOrder.IsChecked.ToString() + "</sortColumnsByTabOrder>" +
            //"<useFieldPrompts>" + checkboxUsePrompts.IsChecked.ToString() + "</useFieldPrompts>" +
            //"<showListLabels>" + checkboxListLabels.IsChecked + "</showListLabels>" +
            //"<showLineColumn>" + checkboxLineColumn.IsChecked.ToString() + "</showLineColumn>" +
            //"<showColumnHeadings>" + checkboxColumnHeaders.IsChecked.ToString() + "</showColumnHeadings>" +
            //"<showNullLabels>" + checkboxShowNulls.IsChecked.ToString() + "</showNullLabels>" +
            //    //"<alternatingRowColors>" + checkboxAltRowColors.IsChecked.ToString() + "</alternatingRowColors>" +
            //    //"<allowUpdates>" + checkboxAllowUpdates.IsChecked.ToString() + "</allowUpdates>" +
            //"<customHeading>" + CustomOutputHeading.Replace("<", "&lt;") + "</customHeading>" +
            //"<customDescription>" + CustomOutputDescription.Replace("<", "&lt;") + "</customDescription>" +
            //"<customCaption>" + CustomOutputCaption + "</customCaption>";

            //xmlString = xmlString + SerializeAnchors();

            element = new XElement("gadget",
                new XAttribute("top", Canvas.GetTop(this).ToString("F0")),
                new XAttribute("left", Canvas.GetLeft(this).ToString("F0")),
                new XAttribute("collapsed", "false"),
                new XAttribute("gadgetType", "Ewav.LineList"),
            new XElement("groupVariable", groupVar),
            new XElement("maxRows", ((ComboBoxItem)cbxMaxRows.SelectedItem).Content),
            new XElement("maxColumnNameLength", txtMaxColumnLength.Text),
            new XElement("sortColumnsByTabOrder", checkboxTabOrder.IsChecked.ToString()),
            new XElement("showcolumnheadings", checkboxColumnHeaders.IsChecked.ToString()),
            new XElement("showlinecolumn", checkboxLineColumn.IsChecked.ToString()),
            new XElement("showNullLabels", checkboxShowNulls.IsChecked.ToString()),
             new XElement("gadgetTitle", GName.Text),
            new XElement("gadgetDescription", txtGadgetDescription.Text)

);


            //if (inputVariableList.ContainsKey("customusercolumnsort"))
            //{
            //    string columns = inputVariableList["customusercolumnsort"];
            //    xmlString = xmlString + "<customusercolumnsort>" + columns + "</customusercolumnsort>";
            //}
            //else if (columnOrder != null && columnOrder.Count > 0) // when user has re-ordered columns but not refreshed
            //{
            //    WordBuilder wb = new WordBuilder("^");
            //    for (int i = 0; i < columnOrder.Count; i++)
            //    {
            //        wb.Add(columnOrder[i]);
            //    }
            //    xmlString = xmlString + "<customusercolumnsort>" + wb.ToString() + "</customusercolumnsort>";
            //}

            //System.Xml.XmlElement element = doc.CreateElement("lineListGadget");
            //element.InnerXml = xmlString;
            //element.AppendChild(SerializeFilters(doc));

            //System.Xml.XmlAttribute id = doc.CreateAttribute("id");
            //System.Xml.XmlAttribute locationY = doc.CreateAttribute("top");
            //System.Xml.XmlAttribute locationX = doc.CreateAttribute("left");
            //System.Xml.XmlAttribute collapsed = doc.CreateAttribute("collapsed");
            //System.Xml.XmlAttribute type = doc.CreateAttribute("gadgetType");

            //id.Value = this.UniqueIdentifier.ToString();
            //locationY.Value = Canvas.GetTop(this).ToString("F0");
            //locationX.Value = Canvas.GetLeft(this).ToString("F0");
            //collapsed.Value = IsCollapsed.ToString();
            //type.Value = "EpiDashboard.LineListControl";

            //element.Attributes.Append(locationY);
            //element.Attributes.Append(locationX);
            //element.Attributes.Append(collapsed);
            //element.Attributes.Append(type);
            //element.Attributes.Append(id);

            if (lbxFields.Items.Count > 0 && lbxFields.SelectedItems.Count > 0)
            {
                string xmlListItemString = string.Empty;
                XElement listItemElement = new XElement("listFields");


                foreach (EwavColumn s in lbxFields.SelectedItems)
                {
                    XElement subElement = new XElement("listField");
                    subElement.Value = s.Name;
                    listItemElement.Add(subElement);
                    //xmlListItemString = xmlListItemString + "<listField>" + s.Name + "</listField>";
                }

                //listItemElement.Value = xmlListItemString;
                element.Add(listItemElement);
            }

            if (lbxSortFields.Items.Count > 0)
            {
                string xmlSortString = string.Empty;
                XElement sortElement = new XElement("sortFields");

                foreach (EwavColumn s in lbxSortFields.Items)
                {
                    XElement subElement = new XElement("sortField");
                    subElement.Value = s.Name;
                    sortElement.Add(subElement);
                    //xmlSortString = xmlSortString + "<sortField>" + s.Name + "</sortField>";
                }

                //sortElement.Value = xmlSortString;
                element.Add(sortElement);
            }

            if (this.GadgetFilters != null)
            {
                this.GadgetFilters.Serialize(element);
            }

            return element;

        }

        public void CreateFromXml(System.Xml.Linq.XElement element)
        {
            //this.LoadingCombos = true;
            //this.ColumnWarningShown = true;

            //HideConfigPanel();

            //infoPanel.Visibility = System.Windows.Visibility.Collapsed;
            //messagePanel.Visibility = System.Windows.Visibility.Collapsed;

            LoadingCanvas = true;


            this.GadgetFilters = new List<EwavDataFilterCondition>();

            PopulateControlsWithData();

            foreach (XElement child in element.Descendants())
            {
                switch (child.Name.ToString().ToLower())
                {
                    case "groupvariable":
                        //cbxGroupField.Text = child.InnerText.Replace("&lt;", "<");
                        cbxGroupField.SelectedIndex = cmnClass.FindIndexToSelect(cmnClass.GetItemsSource(GetFieldPrimaryDataType), child.Value.ToString().Replace("&lt;", "<"));
                        break;
                    case "maxcolumnnamelength":
                        int maxColumnLength = 24;
                        int.TryParse(child.Value, out maxColumnLength);
                        txtMaxColumnLength.Text = maxColumnLength.ToString();
                        break;
                    case "maxrows":
                        int maxRows = 50;
                        int.TryParse(child.Value, out maxRows);
                        //txtMaxRows.Text = maxRows.ToString();
                        for (int i = 0; i < cbxMaxRows.Items.Count; i++)
                        {
                            if (((ComboBoxItem)cbxMaxRows.Items[i]).Content.ToString() == maxRows.ToString())
                            {
                                cbxMaxRows.SelectedIndex = i;
                            }
                        }
                        cbxMaxRows.SelectedItem = maxRows.ToString();
                        break;
                    case "sortcolumnsbytaborder":
                        bool sortByTabs = false;
                        bool.TryParse(child.Value, out sortByTabs);
                        checkboxTabOrder.IsChecked = sortByTabs;
                        break;
                    case "usefieldprompts":
                        bool usePrompts = false;
                        bool.TryParse(child.Value, out usePrompts);
                        checkboxUsePrompts.IsChecked = usePrompts;
                        break;
                    case "allowupdates":
                        bool allowUpdates = false;
                        bool.TryParse(child.Value, out allowUpdates);
                        checkboxAllowUpdates.IsChecked = allowUpdates;
                        break;
                    case "showlinecolumn":
                        bool showLineColumn = true;
                        bool.TryParse(child.Value, out showLineColumn);
                        checkboxLineColumn.IsChecked = showLineColumn;
                        break;
                    case "showcolumnheadings":
                        bool showColumnHeadings = true;
                        bool.TryParse(child.Value, out showColumnHeadings);
                        checkboxColumnHeaders.IsChecked = showColumnHeadings;
                        break;
                    case "showlistlabels":
                        bool showLabels = false;
                        bool.TryParse(child.Value, out showLabels);
                        checkboxListLabels.IsChecked = showLabels;
                        break;
                    case "shownulllabels":
                        bool showNullLabels = true;
                        bool.TryParse(child.Value, out showNullLabels);
                        checkboxShowNulls.IsChecked = showNullLabels;
                        break;
     
                    case "listfields":
                        foreach (XElement field in child.Elements())
                        {
                            List<string> fields = new List<string>();
                            if (field.Name.ToString().ToLower().Equals("listfield"))
                            {
                                //   fields.Add(field.InnerText);

                                //lbxFields.SelectedItems.Add(field.Value.Replace("&lt;", "<"));
                                Object col = applicationViewModel.EwavSelectedDatasource.AllColumns.Where(t => t.Name == field.Value.ToString()).First();

                                lbxFields.SelectedItems.Add((EwavColumn)col);
                            }
                        }
                        break;
                    case "sortfields":
                        foreach (XElement field in child.Elements())
                        {
                            List<string> fields = new List<string>();
                            if (field.Name.ToString().ToLower().Equals("sortfield"))
                            {
                                Object col = applicationViewModel.EwavSelectedDatasource.AllColumns.Where(t => field.Value.ToString().Contains(t.Name)).First();
                                string s = AddColumnToSortList(((EwavColumn)(col)).Name);
                                EwavColumn colToAdd = ((EwavColumn)col).Copy();
                                colToAdd.Name = s;
                                lbxSortFields.Items.Add(colToAdd);
                            }
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

            DoLineList();
            cmnClass.AddControlToCanvas(this, mouseVerticalPosition, mouseHorizontalPosition, applicationViewModel.LayoutRoot);
        }

        public string ToHTML(bool ForDash = false, string htmlFileName = "", int count = 0)
        {

            HtmlBuilder = new StringBuilder();
            //  htmlBuilder.AppendLine("<h2 class=\"gadgetHeading\">Frequency</h2>");
            HtmlBuilder.Append("<h2 >LineList HTML Not Implemented</h2>");

            return "LineList HTML Not Implemented";
        }

        public bool IsProcessing
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void SetGadgetToProcessingState()
        {
            throw new NotImplementedException();
        }

        public void SetGadgetToFinishedState()
        {
            throw new NotImplementedException();
        }

        public void UpdateVariableNames()
        {
            throw new NotImplementedException();
        }

        public string CustomOutputHeading
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string CustomOutputDescription
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string CustomOutputCaption
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void CloseGadgetOnClick()
        {
            CloseGadget confirm = new CloseGadget(this);
            confirm.Show();
        }

        public void CloseGadget()
        {
            applicationViewModel.CloseGadget(this);

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseGadgetOnClick();
        }

        public string MyControlName
        {
            get { return "LineList"; }
        }

        public string MyUIName
        {
            get { return "Line List"; }
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
        ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;

        public ApplicationViewModel ApplicationViewModel
        {
            get
            {
                return applicationViewModel;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (this.lbxFields.SelectedItems.Count < 1)
            {
                DemoMode dm = new DemoMode("Pick at least one column to list. ");
                dm.Show();

                return;
            }
            waitCursor.Visibility = Visibility.Visible;
            DoLineList();
            gadgetExpander.IsExpanded = false;
        }

        void llviewModel_TableLoadedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {


            RenderLineList();
        }

        private void RenderLineList()
        {
            panelMain.Children.Clear();

            waitCursor.Visibility = Visibility.Collapsed;

            gadgetExpander.IsExpanded = false;

            pnlStatus.Visibility = System.Windows.Visibility.Collapsed;

            for (int n = 0; n < llviewModel.ListOfTables.Count; n++)
            {
                DatatableBag dtb = this.llviewModel.ListOfTables[n];

                List<List<string>> listOfList = new List<List<string>>();

                if (checkboxShowNulls.IsChecked == true)
                {
                    for (int p = 0; p < dtb.RecordList.Count; p++)
                    {
                        if (((MyString)dtb.RecordList[p].Fields[0]).VarName.ToString().Length == 0)
                        {
                            ((MyString)dtb.RecordList[p].Fields[0]).VarName = "Missing";
                        }
                    }
                }

                for (int i = 0; i < dtb.RecordList.Count; i++)
                {
                    List<string> list = new List<string>();
                    for (int j = 0; j < dtb.RecordList[i].Fields.Count; j++)
                    {
                        list.Add(dtb.RecordList[i].Fields[j].VarName);
                    }
                    listOfList.Add(list);
                }

                DataGrid lineListGrid = new DataGrid();
                //lineListGrid.LoadingRow +=new EventHandler<DataGridRowEventArgs>(lineListGrid_LoadingRow);
                //lineListGrid.ro
                //lineListGrid.Height = 195.0;
                //lineListGrid.Margin = 12,12,0,0 ;
                lineListGrid.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                //lineListGrid.Width = 352.0;
                lineListGrid.MaxHeight = 500;
                lineListGrid.MaxWidth = 1000;

                lineListGrid.Columns.Clear();
                lineListGrid.AutoGenerateColumns = false;
                lineListGrid.LoadingRow += new EventHandler<DataGridRowEventArgs>(lineListGrid_LoadingRow);
                lineListGrid.MouseLeftButtonDown += new MouseButtonEventHandler(lineListGrid_MouseLeftButtonDown);

                lineListGrid.HeadersVisibility = DataGridHeadersVisibility.All;
                lineListGrid.RowHeight = 30;
                lineListGrid.IsReadOnly = true;
                lineListGrid.CanUserReorderColumns = false;
                lineListGrid.CanUserResizeColumns = true;
                lineListGrid.CanUserSortColumns = true;
                lineListGrid.Style = App.Current.Resources["DataGridStyle1"] as Style;
                lineListGrid.ColumnHeaderStyle = App.Current.Resources["DataGridColumnHeaderStyle1"] as Style;
                lineListGrid.CellStyle = App.Current.Resources["DataGridCellStyle1"] as Style;
                //lineListGrid.RowStyle = App.Current.Resources["DataGridRowStyle1"] as Style;
                //lineListGrid.RowHeaderStyle = App.Current.Resources["DataGridRowHeaderStyle1"] as Style;

                lineListGrid.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 115, 149, 191));
                lineListGrid.VerticalGridLinesBrush = new SolidColorBrush(Color.FromArgb(255, 115, 149, 191));
                lineListGrid.HorizontalGridLinesBrush = new SolidColorBrush(Color.FromArgb(255, 115, 149, 191));
                lineListGrid.BorderThickness = new Thickness(1, 1, 1, 1);
                lineListGrid.AutoGenerateColumns = false;
                lineListGrid.AlternatingRowBackground = null;


                //ObservableCollection<ObservableCollection<string>> dataSource = new ObservableCollection<ObservableCollection<string>>();
               


                for (int k = 0; k < dtb.RecordList[0].Fields.Count; k++)
                {
                    DataGridTextColumn dataColumn = new DataGridTextColumn();
                    if (checkboxColumnHeaders.IsChecked == true)
                    {
                        if (dtb.ColumnNameList[k].VarName.Length > Convert.ToInt32(txtMaxColumnLength.Text))
                        {
                            dataColumn.Header = dtb.ColumnNameList[k].VarName.Substring(0, Convert.ToInt32(txtMaxColumnLength.Text)) + "...";
                        }
                        else
                        {
                            dataColumn.Header = dtb.ColumnNameList[k].VarName;
                        }
                    }
                    
                    dataColumn.Binding = new System.Windows.Data.Binding("[" + k.ToString() + "]");
                    lineListGrid.Columns.Add(dataColumn);
                }

                TextBlock tb = new TextBlock();
                //tb.Height = 50.0;
                tb.Margin = new Thickness(0, 22, 0, 5);
                tb.FontWeight = FontWeights.Bold;
                tb.HorizontalAlignment = HorizontalAlignment.Left;
                tb.VerticalAlignment = VerticalAlignment.Top;
                //tb.Width = 400.0;

                tb.Text = dtb.TableName.ToString().Replace("[", "").Replace("]", "");

                lineListGrid.ItemsSource = listOfList;
                if (llviewModel.ListOfTables.Count > 1)
                {
                    panelMain.Children.Add(tb);
                }
                panelMain.Children.Add(lineListGrid);
            }
            panelMain.Visibility = System.Windows.Visibility.Visible;
            spContent.Visibility = System.Windows.Visibility.Visible;
            ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            FilterButton.IsEnabled = true;
        }

        void lineListGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void LayoutRoot_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //ClientCommon.Common cmnClass = new ClientCommon.Common();
            Point p = e.GetSafePosition(this.cmnClass.GetParentObject<Grid>(this, "LayoutRoot"));
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).GadgetNameOnRightClick = this.MyControlName; //"FrequencyControl";
            //((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).StrataList = this.strataGridList;
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).SelectedGadget = this;
            cmnClass.UpdateZOrder(this, true, cmnClass.GetParentObject<Grid>(this, "LayoutRoot"));
        }

        void lineListGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (checkboxLineColumn.IsChecked == true)
            {
                e.Row.Header = (e.Row.GetIndex() + 1).ToString();
                e.Row.Style = App.Current.Resources["DataGridRowStyle1"] as Style;
                // e.RowHeaderStyle = App.Current.Resources["DataGridRowHeaderStyle1"] as Style;
                e.Row.HeaderStyle = App.Current.Resources["DataGridRowHeaderStyle1"] as Style;
            }
            else
            {
                ((DataGrid)sender).HeadersVisibility = DataGridHeadersVisibility.Column;
            }


        }



        /// <summary>
        /// Clears the results.
        /// </summary>
        private void ClearResults()
        {
            panelMain.Children.Clear();

            grdLineListOptions.Visibility = Visibility.Collapsed;

            txtStatus.Visibility = Visibility.Collapsed;
            txtStatus.Text = string.Empty;

            waitCursor.Visibility = Visibility.Visible;

            //btnRun.IsEnabled = false;
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
            panelMain.Visibility = System.Windows.Visibility.Collapsed;
            //CollapseConfigPanel();
        }

        /// <summary>
        /// Used to generate the list of variables and options for the GadgetParameters object
        /// </summary> 
        private void CreateInputVariableList()
        {
            Dictionary<string, string> inputVariableList = new Dictionary<string, string>();

            gadgetOptions.MainVariableName = string.Empty;
            gadgetOptions.WeightVariableName = string.Empty;
            gadgetOptions.StrataVariableNames = new List<string>();
            gadgetOptions.CrosstabVariableName = string.Empty;
            gadgetOptions.UseAdvancedDataFilter = applicationViewModel.UseAdvancedFilter;
            this.gadgetOptions.TableName = this.applicationViewModel.EwavSelectedDatasource.TableName;
            inputVariableList.Add("tableName", this.gadgetOptions.TableName);

            gadgetOptions.GadgetFilters = GadgetFilters;
            List<string> listFields = new List<string>();

            if (lbxFields.SelectedItems.Count > 0)
            {
                foreach (EwavColumn item in lbxFields.SelectedItems)
                {
                    if (!string.IsNullOrEmpty(item.Name))
                    {
                        listFields.Add(item.Name);
                    }
                }
            }

            listFields.Sort();


            foreach (string field in listFields)
            {
                inputVariableList.Add(field, "listfield");
            }

            if (!inputVariableList.ContainsKey("sortcolumnsbytaborder"))
            {
                if (checkboxTabOrder.IsChecked == true)
                {
                    inputVariableList.Add("sortcolumnsbytaborder", "true");
                }
                else
                {
                    inputVariableList.Add("sortcolumnsbytaborder", "false");
                }
            }

            if (!inputVariableList.ContainsKey("usepromptsforcolumnnames"))
            {
                if (checkboxUsePrompts.IsChecked == true)
                {
                    inputVariableList.Add("usepromptsforcolumnnames", "true");
                }
                else
                {
                    inputVariableList.Add("usepromptsforcolumnnames", "false");
                }
            }

            if (!inputVariableList.ContainsKey("showcolumnheadings"))
            {
                if (checkboxColumnHeaders.IsChecked == true)
                {
                    inputVariableList.Add("showcolumnheadings", "true");
                }
                else
                {
                    inputVariableList.Add("showcolumnheadings", "false");
                }
            }

            if (!inputVariableList.ContainsKey("showlinecolumn"))
            {
                if (checkboxLineColumn.IsChecked == true)
                {
                    inputVariableList.Add("showlinecolumn", "true");
                }
                else
                {
                    inputVariableList.Add("showlinecolumn", "false");
                }
            }

            if (!inputVariableList.ContainsKey("shownulllabels"))
            {
                if (checkboxShowNulls.IsChecked == true)
                {
                    inputVariableList.Add("shownulllabels", "true");
                }
                else
                {
                    inputVariableList.Add("shownulllabels", "false");
                }
            }

            if (!inputVariableList.ContainsKey("maxColumnNameLength"))
            {
                inputVariableList.Add("maxColumnNameLength", txtMaxColumnLength.Text);
            }

            if (checkboxListLabels.IsChecked == true)
            {
                gadgetOptions.ShouldShowCommentLegalLabels = true;
            }
            else
            {
                gadgetOptions.ShouldShowCommentLegalLabels = false;
            }

            if (lbxSortFields.Items.Count > 0)
            {
                foreach (EwavColumn item in lbxSortFields.Items)
                {
                    if (!string.IsNullOrEmpty(item.Name))
                    {
                        string baseStr = item.Name;

                        if (baseStr.EndsWith("(ascending)"))
                        {
                            baseStr = "[" + baseStr.Remove(baseStr.Length - 12) + "] ASC";
                        }
                        if (baseStr.EndsWith("(descending)"))
                        {
                            baseStr = "[" + baseStr.Remove(baseStr.Length - 13) + "] DESC";
                        }
                        inputVariableList.Add(baseStr, "sortfield");
                    }
                }
            }
            this.gadgetOptions.StrataVariableList = new List<MyString>();

            if (cbxGroupField.SelectedIndex > 0)
            {
                if (!string.IsNullOrEmpty(((EwavColumn)cbxGroupField.SelectedItem).Name.ToString().Trim()))
                {
                    //gadgetOptions.StrataVariabl.Add(cbxGroupField.SelectedItem.ToString());
                    List<MyString> listMyString = new List<MyString>();
                    MyString objMyString = new MyString();
                    objMyString.VarName = ((EwavColumn)cbxGroupField.SelectedItem).Name;
                    listMyString.Add(objMyString);
                    this.gadgetOptions.StrataVariableList = listMyString;
                }
            }

            //if (StrataGridList.Count >= 1)
            //{
            //    Grid grid = StrataGridList[0];
            //    SortedDictionary<int, string> sortColumnDictionary = new SortedDictionary<int, string>();

            //    foreach (UIElement element in grid.Children)
            //    {
            //        if (Grid.GetRow(element) == 0 && element is TextBlock)
            //        {
            //            TextBlock txtColumnName = element as TextBlock;
            //            //columnOrder.Add(txtColumnName.Text);
            //            sortColumnDictionary.Add(Grid.GetColumn(element), txtColumnName.Text);
            //        }
            //    }

            //    columnOrder = new List<string>();
            //    foreach (KeyValuePair<int, string> kvp in sortColumnDictionary)
            //    {
            //        columnOrder.Add(kvp.Value);
            //    }

            //    if (columnOrder.Count == listFields.Count || columnOrder.Count == (listFields.Count + 1))
            //    {
            //        bool same = true;
            //        foreach (string s in listFields)
            //        {
            //            if (!columnOrder.Contains(s))
            //            {
            //                same = false;
            //            }
            //        }

            //        if (same)
            //        {
            //            WordBuilder wb = new WordBuilder("^");
            //            foreach (string s in columnOrder)
            //            {
            //                wb.Add(s);
            //            }

            //            inputVariableList.Add("customusercolumnsort", wb.ToString());
            //        }
            //        else
            //        {
            //            columnOrder = new List<string>();
            //        }
            //    }
            //    else
            //    {
            //        columnOrder = new List<string>();
            //    }
            //}

            //inputVariableList.Add("maxcolumns",  MaxColumns.ToString());
            inputVariableList.Add("maxrows", ((ComboBoxItem)cbxMaxRows.SelectedItem).Content.ToString());

            gadgetOptions.ShouldIncludeFullSummaryStatistics = false;
            gadgetOptions.InputVariableList = inputVariableList;
        }

        private string AddColumnToSortList(string columnName)
        {
            if (!lbxSortFields.Items.Contains(columnName) && !lbxSortFields.Items.Contains(columnName + " (ascending)"))
            {
                return columnName + " (ascending)";
            }
            return "";
        }

        private void cbxSortField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (((ComboBox)sender).SelectedItem != null)
            //{
            //    lbxSortFields.Items.Add(((EwavColumn)((ComboBox)sender).SelectedItem).Name);     
            //}

            if (!LoadingDropDowns &&
                cbxSortField.SelectedIndex > 0) // && !lbxSortFields.Items.Contains(((EwavColumn)cbxSortField.SelectedItem).Name.ToString()))
            {
                string s = AddColumnToSortList(((EwavColumn)((ComboBox)sender).SelectedItem).Name);
                if (s.Length > 0)
                {
                    for (int i = 0; i < lbxSortFields.Items.Count; i++)
                    {
                        if (((EwavColumn)lbxSortFields.Items[i]).Name == s)
                        {
                            return;
                        }

                    }

                    EwavColumn tempCol = ((EwavColumn)((ComboBox)sender).SelectedItem).Copy();

                    tempCol.Name = s;

                    lbxSortFields.Items.Add(tempCol);

                }

            }

        }

        private void btnRemoveVariables_Click(object sender, RoutedEventArgs e)
        {
            lbxSortFields.Items.Remove(lbxSortFields.SelectedItem);
        }


        public void Reload()
        {


            waitCursor.Visibility = Visibility.Visible;
            DoLineList();
            gadgetExpander.IsExpanded = false;





        }

        private void lbxFields_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbxFields.SelectedItems.Count < 1)
            {
                btnRun.IsEnabled = false;
            }
            else
            {
                btnRun.IsEnabled = true;
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

        public void window_Closed(object sender, EventArgs e)
        {
            GadgetFilterControl GadgetFilter = ((GadgetFilterControl)sender);
            if (GadgetFilter.DialogResult == true)
            {
                GadgetFilters = GadgetFilter.GadgetFilters;
            }

            DoLineList();
        }


      
    }
}

namespace Ewav.Web.Services
{
    public partial class LineListDomainContext
    {
        //This is the place to set RIA Services query timeout. 
        //TimeSpan(0, 5, 0) means five minutes vs the 
        //default 60 sec
        partial void OnCreated()
        {
            if (!DesignerProperties.GetIsInDesignMode(Application.
                Current.RootVisual))
                ((WebDomainClient<ILineListDomainServiceContract>)
                    DomainClient).ChannelFactory.Endpoint.Binding.SendTimeout =
                    new TimeSpan(0, 120, 0);
        }
    }
}