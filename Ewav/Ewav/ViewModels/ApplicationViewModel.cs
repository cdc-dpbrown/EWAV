/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ApplicationViewModel.cs
 *  Namespace:  Ewav.ViewModels    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Windows.Controls;
using System.Xml.Linq;
using Ewav.Web.EpiDashboard;
using Ewav.Web.EpiDashboard.Rules;
using Ewav.BAL;
using Ewav.Client.Application;
using Ewav.ClientCommon;
using Ewav.Common;
using Ewav.DTO;
using Ewav.ExtensionMethods;
using Ewav.Membership;
using Ewav.ViewModels.Membership;
using Ewav.Web.Services;
using System.Globalization;
using System.Reflection;
using System.Diagnostics;


namespace Ewav.ViewModels
{
    /// <summary>
    ///  ** Singleton  **  
    /// This class is resp for global changes in the client application state      
    /// </summary>
    public sealed class ApplicationViewModel
    {
        #region   ** Singleton  **

        static readonly ApplicationViewModel _instance = new ApplicationViewModel();

        //  private ObservableCollection<EwavDatasourceDto> datasources;

        private bool demoMode;



        #endregion

        #region Constructor

        private string keyForBingMaps;

        private bool urlHasCanvasGuid;





        /// <summary>
        /// WARNING – This constructor cannot be public.  
        /// Making the constructor of this singleton class public will rip a hole in space-time, 
        /// surely destroying us all. -DS                               
        /// </summary>
        private ApplicationViewModel()
        {




            LoggingOut = false;



            //    demoMode =  System.conf

            DragCanvas.CanvasSnapshotComplete += new CanvasSnapshotCompleteHandler(DragCanvas_CanvasSnapshotComplete);


            this.ColumnsLoadedEvent += new ColumnsLoadedEventEventHandler(ApplicationViewModel_ColumnsLoadedEvent);
            this.TotalRecordcountLoadedEvent += new TotalRecordcountLoadedEventHandler(ApplicationViewModel_TotalRecordcountLoadedEvent);
            this.ApplyDataFilterEvent += new ApplyFilterEventHandler(ApplicationViewModel_ApplyDataFilterEvent);
            this.ReadFilterStringEvent += new ReadFilterStringEventHandler(ApplicationViewModel_ReadFilterStringEvent);
            this.LoadedGadgets = new ObservableCollection<string>();
            this.LoadedGadgets.CollectionChanged += new NotifyCollectionChangedEventHandler(loadedGadgets_CollectionChanged);
            //DatasourceChangedEvent += new DatasourceChangedEventHandler(ApplicationViewModel_DatasourceChangedEvent);
            this.mefAvailableGadgets = new List<string>();
            this.mefAvailableCharts = new List<string>();

            this.ewavDefinedVariables = new List<EwavRule_Base>();

            this.LoggedInUser = User.Instance;
            this.LoggedInUser.UserChanged += new EventHandler(LoggedInUser_UserChanged);

            DashboardMainPage.CanvasSizeChangedEvent += new CanvasSizeChangedHandler(DashboardMainPage_CanvasSizeChangedEvent);

            ClientCommon.Common.GadgetAddedEvent += Common_GadgetAddedEvent;

        }

        void Common_GadgetAddedEvent(object sender, EventArgs e)
        {

            GadgetsOnCanvas = true;
            this.AppMenuView.SetDatasourceDisplayText();

        }

        //void ApplicationViewModel_GadgetAddedEvent(object sender, EventArgs e)
        //{

        //    GadgetsOnCanvas = true;
        //    this.AppMenuView.SetDatasourceDisplayText();

        //}

        public AppMenuView AppMenuView { get; set; }

        /// <summary>
        /// Gets or sets the current canvas share status dto.
        /// </summary>
        /// <value>The current canvas share status dto.</value>
        public List<CanvasShareStatusDto> CurrentCanvasShareStatusDto { get; set; }

        /// <summary>
        /// Gets or sets the URL has canvas GUID.
        /// </summary>
        /// <value>The URL has canvas GUID.</value>
        public bool UrlHasCanvasGUID
        {
            get
            {

                if (this.urlHasCanvasGuid)
                {
                    this.urlHasCanvasGuid = false;
                    return true;
                }
                else
                {
                    return false;
                }


            }

            set
            {
                this.urlHasCanvasGuid = value;
            }
        }

        /// <summary>
        /// Gets or sets the canvas GUID from URL.
        /// </summary>
        /// <value>The canvas GUID from URL.</value>
        public Guid CanvasGUIDFromUrl { get; set; }

        void DashboardMainPage_CanvasSizeChangedEvent(CanvasChangedEventArgs e)
        {
            CanvasHeight = e.Height;
            CanvasWidth = e.Width;
        }

        void DragCanvas_CanvasSnapshotComplete(string CanvasSnapshotAsBase64)
        {

            CanvasViewModel canvasViewModel = new CanvasViewModel();
            CanvasDto cdto = new CanvasDto();
            cdto.CanvasId = CurrentCanvasId;
            cdto.CanvasSnapshotAsBase64 = CanvasSnapshotAsBase64;

            //   canvasViewModel

            canvasViewModel.CanvasSnapshotCompleted += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(canvasViewModel_CanvasSnapshotCompleted);

            canvasViewModel.SaveCanvasSnapshot(cdto);


        }

        void canvasViewModel_CanvasSnapshotCompleted(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {

        }


        private bool enableExceptionDetail = false;
        /// <summary>
        /// This field is used only for showing the exception box with stack trace info.
        /// </summary>
        public bool EnableExceptionDetail
        {
            get { return enableExceptionDetail; }
            set { enableExceptionDetail = value; }
        }

        double CanvasHeight;
        double CanvasWidth;


        private bool sendEmailOnException;
        /// <summary>
        /// This field is used only to send email, if exception occurs based upon Web.Config switch.
        /// </summary>
        public bool SendEmailOnException
        {
            get { return sendEmailOnException; }
            set { sendEmailOnException = value; }
        }



        public const double CANVAS_DEFAULT_WIDTH = 4000;
        public const double CANVAS_DEFAULT_HEIGHT = 8000;





        /// <summary>
        /// Gets or sets the canvas loaded.
        /// </summary>
        /// <value>The canvas loaded.</value>
        public bool GadgetsOnCanvas
        {
            get
            {
                return this._gadgetsOnCanvas;
            }
            set
            {
                this._gadgetsOnCanvas = value;
            }
        }



        public event EventHandler GadgetsReloaded;

        public void ReloadGadgets()
        {



            foreach (IEwavGadget uc in this.Gadgets)
            {


                uc.Reload();






            }


            GadgetsReloaded(this, new EventArgs());



        }

        //void ApplicationViewModel_DatasourceChangedEvent(object o, DatasourceChangedEventArgs e)
        //{

        //}

        /// <summary>
        /// Gets or sets the key for bing maps.
        /// </summary>
        /// <value>The key for bing maps.</value>
        public string KeyForBingMaps
        {
            get
            {
                return this.keyForBingMaps;
            }
            set
            {
                this.keyForBingMaps = value;
            }
        }

        void LoggedInUser_UserChanged(object sender, EventArgs e)
        {
            cvm = new CanvasViewModel();
        }

        #endregion

        #region Events

        /// <summary>
        /// Gets or sets the logging out.
        /// </summary>
        /// <value>The logging out.</value>
        public bool LoggingOut { get; set; }

        /// <summary>
        /// After the user selects a datasource the Connection string is avail to the application   
        /// </summary>   
        public event ConnectionStringReadyEventHandler ConnectionStringReadyEvent;
        /// <summary>
        /// When a connetion is made to the datasouce the allColumns are available to the application  
        /// </summary>
        public event ColumnsLoadedEventEventHandler ColumnsLoadedEvent;
        /// <summary>
        /// When a conection is made to the data source total record count is avail to the application     
        /// </summary>        
        public event TotalRecordcountLoadedEventHandler TotalRecordcountLoadedEvent;
        /// <summary>
        /// When the filter super gadget applies a new filter to the selected data source the filtered 
        /// record couht is avial to the application      
        /// </summary>
        public event FilteredRecordcountUpdatedEventHandler FilteredRecordCountUpdatedEvent;
        /// <summary>
        /// When the data source name is chahged the name is made avail to the application  
        /// </summary>
        public event DatasourceChangedEventHandler DatasourceChangedEvent;
        /// <summary>
        /// When new positions for a(n) element is calculated a Dictionary of new positions is avail to 
        /// all elements   
        /// </summary>
        public event ElementMoveEventHandler ElementMoveEvent;

        // public event DatasourceListReadyEventHandler DatasourceListReadyEvent;

        public event ApplyFilterEventHandler ApplyDataFilterEvent;

        public event ReadFilterStringEventHandler ReadFilterStringEvent;

        public event FilterStringUpdatedEventHandler FilterStringUpdatedEvent;

        public event DefinedVariableAddedEventHandler DefinedVariableAddedEvent;

        public event DefinedVariableInUseDeletedEventHandler DefinedVariableInUseDeletedEvent;

        public event DefinedVariableNotInUseDeletedEventHandler DefinedVariableNotInUseDeletedEvent;

        public event PreColumnChangedEventHandler PreColumnChangedEvent;

        public event SaveCanvasCompletedEventHandler SaveCanvasCompletedEvent;

        public event RulesAddedEventHandler RulesAddedEvent;

        public event FiltersDeserializedEventHandler FiltersDeserializedEvent;

        public event ErrorNoticeEventHandler ErrorNoticeEvent;

        public event UnloadedEventHandler UnloadedEvent;

        public event CanvasListLoadedEventHandler CanvasListLoadedEvent;

        public event ShareCanvasCompletedEventHandler ShareCanvasCompletedEvent;

        public event EmailSentCompletedEventHandler EmailSentCompletedEvent;

        public event ShareCanvasLoadedEventHandler ShareCanvasLoadedEvent;

        public event DeleteCanvasCompletedEventHandler DeleteCanvasCompletedEvent;

        public event ClearDataFiltersEventHandler ClearDataFiltersEvent;

        public event ClearDefinedVariablesEventHandler ClearDefinedVariablesEvent;

        public event UpdateDataFilterCountEventHandler UpdateDataFilterCount;

        public event EventHandler ScrollUpEvent;

        //public event Clear

        //public event AdvDataFilterStringEventHandler AdvDataFilterStringEvent;

        #endregion // Eventsqlo

        #region Fields

        private EwavDatasourceDto ewavSelectedDatasource;

        /// <summary>
        /// Gadgets available through MEF    
        /// </summary>
        private readonly List<string> mefAvailableGadgets;
        private readonly List<string> mefAvailableCharts;
        private List<EwavDataFilterCondition> ewavDatafilters = new List<EwavDataFilterCondition>();

        // XElements for canvas file   
        private XElement allGadgetXmlElements = null;
        private XElement allDataFilterXmlElements = null;
        private XElement allRuleXmlElements = null;
        CanvasViewModel cvm = null;


        public CanvasDto LoadedCanvasDto = null;


        bool _gadgetsOnCanvas = false;






        #endregion

        #region  Properties

        private bool _isEpiWebIntegrationEnabled;
        /// <summary>
        /// Web.Config driven property.
        /// </summary>
        public bool IsEpiWebIntegrationEnabled
        {
            get { return _isEpiWebIntegrationEnabled; }
            set { _isEpiWebIntegrationEnabled = value; }
        }


        public List<string> UserNames { get; set; }

        /// <summary>
        /// Gets or sets the key for user password salt.
        /// </summary>
        /// <value>The key for user password salt.</value>
        public string KeyForUserPasswordSalt { get; set; }

        /// <summary>
        /// If true unloads all the previous gadgets including rules and df.
        /// </summary>
        private bool isReset = false;

        public bool IsReset
        {
            get
            {
                return isReset;
            }
            set
            {
                isReset = value;
            }
        }

        public int UserIdForOpenedCanvas { get; set; }




        public string AssemblyVersion
        {
            get
            {

                try
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    String version = assembly.FullName.Split(',')[1];
                    String fullversion = version.Split('=')[1];

                    return fullversion;
                }
                catch (Exception ex)
                {
                    return " ";
                }


            }
        }
        public DatatableBag AllUsersInMyOrg { get; set; }

        public string EwvSelectedDatasourceNameCandidate { get; set; }

        public string SelectedCanvasName { get; set; }

        public List<CanvasDto> CanvasDtoListForLoggedUser { get; set; }

        private bool isDefVarInUseByDF = false;

        public bool IsDefVarInUseByDF
        {
            get
            {
                return isDefVarInUseByDF;
            }
            set
            {
                isDefVarInUseByDF = value;
            }
        }

        public bool IsMouseDownOnMap { get; set; }

        private List<EwavColumn> listOfDefinedVarsInUseByAnotherVar = new List<EwavColumn>();

        public List<EwavColumn> ListOfDefinedVarsInUseByAnotherVar
        {
            get
            {
                return listOfDefinedVarsInUseByAnotherVar;
            }
            set
            {
                listOfDefinedVarsInUseByAnotherVar = value;
            }
        }

        public DatatableBag CanvasListForUser { get; set; }

        public int DatasourceId { get; set; }

        public User LoggedInUser { get; set; }

        public int CurrentCanvasId { get; set; }

        public bool IsCurrentCanvasShared { get; set; }

        public Canvas MainCanvas { get; set; }

        public Grid LayoutRoot { get; set; }

        public static ApplicationViewModel Instance
        {
            get
            {
                return _instance;
            }
        }

        //private Dictionary<string, Object> tempDataStorage = new Dictionary<string, object>();

        //public Dictionary<string, Object> TempDataStorage
        //{
        //    get
        //    {
        //        return tempDataStorage;
        //    }
        //    set
        //    {
        //        tempDataStorage = value;
        //    }
        //}

        private int ewavDatasourceSelectedIndex = -1; //Default value 

        /// <summary>
        /// Gets or sets the selected ewav datasource.
        /// </summary>
        /// <value>The selected ewav datasource.</value>
        public int EwavDatasourceSelectedIndex
        {
            get
            {
                return this.ewavDatasourceSelectedIndex;
            }
            set
            {
                try
                {
                    if (value != -1)//ewavDatasourceSelectedIndex != value &&
                    {
                        // set field      
                        this.ewavDatasourceSelectedIndex = value;

                        // set field 
                        this.ewavSelectedDatasource = this.EwavDatasources[value];

                        // reset filters 
                        this.ewavDatafilters = new List<EwavDataFilterCondition>();

                        // reset rules 
                        this.ewavDefinedVariables.Clear();
                        //  Datasources  
                        DatasourceViewModel d = new DatasourceViewModel();
                        d.GetColumnsForDatasource(this.ewavSelectedDatasource.DatasourceName);

                        AppMenuViewModel a = new AppMenuViewModel();

                        // GetColumnsForDatasource callback  
                        EventHandler ColumnsForDatasourceLoadedComplete = (s, e) =>
                        {
                            this.ewavSelectedDatasource.AllColumns = (List<EwavColumn>)s;


                            if (UseAdvancedFilter)
                            {
                                a.GetRecordCount(EwavDefinedVariables,
                                    AdvancedDataFilterString,
                                    EwavSelectedDatasource.TableName,
                                    EwavSelectedDatasource.DatasourceName);
                            }
                            else
                            {
                                a.GetRecordCount(EwavDatafilters,
                                    EwavDefinedVariables,
                                    EwavSelectedDatasource.TableName,
                                    EwavSelectedDatasource.DatasourceName);

                            }


                        };

                        //  GetTotalRecordCount count callback    
                        EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>> RecordsCountedComplete = (s, e) =>
                        {
                            string countStr = ((AppMenuViewModel)s).RecordCountString;
                            this.ewavSelectedDatasource.TotalRecords = Convert.ToInt64(countStr.Substring(0, countStr.IndexOf(",")));

                            this.DatasourceChangedEvent(this, new DatasourceChangedEventArgs(ewavDatasourceSelectedIndex));


                        };

                        //  wire up    
                        d.ColumnsForDatasourceLoaded += ColumnsForDatasourceLoadedComplete;
                        a.RecordcountRecievedEvent += RecordsCountedComplete;


                    }
                }
                catch (Exception ex)
                {

                    this.ewavDatasourceSelectedIndex = -1;
                    this.ewavSelectedDatasource = null;

                    throw new Exception(string.Format("Could not set selected datasource {0}{1}", this.ewavSelectedDatasource.DatasourceName, ex.Message));
                }
            }
        }

        void a_RecordcountRecievedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            //  throw new NotImplementedException();
        }

        public bool UseAdvancedFilter { get; set; }

        public List<EwavDatasourceDto> EwavDatasources { get; set; }

        private bool alreadyExists;

        public bool AlreadyExists
        {
            get
            {
                if (this.alreadyExists)
                {
                    return true;
                }
                else
                {
                    this.alreadyExists = true;
                    return false;
                }
            }
        }

        private string filterString;

        /// <summary>
        /// Gets or sets Filterstring.
        /// This Property is not same as Property AdvancedDataFilterString. 
        /// </summary>
        public string FilterString
        {
            get
            {
                return filterString;
            }
            set
            {
                filterString = value;
                FilterStringUpdatedEvent(this);
            }
        }

        /// <summary>
        /// Gets or sets the datasources.
        /// </summary>
        /// <value>The datasources.</value>
        //public ObservableCollection<EwavDatasourceDto> Datasources
        //{
        //    get
        //    {
        //        return this.datasources;
        //    }
        //    set
        //    {
        //        this.datasources = value;
        //        DatasourceListReadyEvent(this, new DatasourceListReadyEventArgs(datasources));
        //    }
        //}

        /// <summary>
        /// Gets and sets Filter String for advanced mode
        /// </summary>
        private string advancedDataFilterString;

        //This property doesn't replace Property FilterString. It uses FilterString internally.
        public string AdvancedDataFilterString
        {
            get
            {
                return advancedDataFilterString;
            }
            set
            {
                advancedDataFilterString = value;
                this.FilterString = value;
                if (!(value == string.Empty)) //Refrain from raising event here coz it ll be invoked in next steps when value is string.empty.
                {
                    ApplyDataFilterEvent(this);
                }
                //AdvDataFilterStringEvent(this);
            }
        }

        /// <summary>
        /// Gets or sets the ewav datafilters.
        /// </summary>
        /// <value>The ewav datafilters.</value>
        public List<EwavDataFilterCondition> EwavDatafilters
        {
            get
            {
                return this.ewavDatafilters;
            }
            set
            {
                this.ewavDatafilters = value;
                ApplyDataFilterEvent(this);
                ReadFilterStringEvent(this);
                UpdateDataFilterCount(this);
            }
        }

        public string CurrentUserDomain { get; set; }

        public string AuthenticationMode { get; set; }

        public void InvokePreColumnChangedEvent()
        {
            PreColumnChangedEvent(this);
        }

        // = new List<string>();

        public List<ListBoxItemSource> ListOfRules { get; set; }

        private bool removeIndicator = false;

        public bool RemoveIndicator
        {
            get
            {
                return removeIndicator;
            }
            set
            {
                removeIndicator = value;
            }
        }

        public EwavColumn ItemToBeRemoved { get; set; }

        private List<EwavRule_Base> ewavDefinedVariables;

        public List<EwavRule_Base> EwavDefinedVariables
        {
            get
            {
                return ewavDefinedVariables;
            }
            set
            {
                if (RemoveIndicator)
                {
                    if (DeletedVariableInUse)
                    {
                        DefinedVariableInUseDeletedEvent(ItemToBeRemoved);
                    }
                    else
                    {
                        DefinedVariableNotInUseDeletedEvent(ItemToBeRemoved);
                    }
                }
                else if (ewavDefinedVariables.Count == 0)
                {
                    //do nothing.
                }
                else
                {
                    // event is raised  
                    DefinedVariableAddedEvent(this);
                }
                ewavDefinedVariables = value;
            }
        }

        //private long selectedDatasourceFilteredRecords = -1;

        //public long SelectedDatasourceFilteredRecords
        //{
        //    set
        //    {
        //        if (ewavSelectedDatasource == null)
        //        {
        //            throw new Exception("There is no selected datasource ");
        //        }
        //        selectedDatasourceFilteredRecords = value;
        //        EwavSelectedDatasource.FilteredRecords = value;
        //        FilteredRecordCountUpdatedEvent(this, new FilteredRecordcountUpdatedEventArgs(value));
        //    }
        //    get
        //    {
        //        return selectedDatasourceFilteredRecords;
        //    }
        //}

        public bool DeletedVariableInUse { get; set; }

        /// <summary>
        /// Gets or sets the ewav selected datasource.
        /// </summary>             
        /// <value>The ewav selected datasource.</value>
        public EwavDatasourceDto EwavSelectedDatasource
        {
            get
            {
                if (this.ewavSelectedDatasource != null)
                {
                    return EwavDatasources[ewavDatasourceSelectedIndex];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the loaded gadgets.
        /// </summary>
        /// <value>The loaded gadgets.</value>
        public ObservableCollection<string> LoadedGadgets { get; set; }

        private List<UserControl> gadgets = new List<UserControl>();

        public List<UserControl> Gadgets
        {
            get
            {
                return gadgets;
            }
            set
            {
                gadgets = value;
            }
        }

        #endregion

        #region Event Hadlers

        //void ApplicationViewModel_AdvDataFilterStringEvent(object o)
        //{

        //}

        void ApplicationViewModel_ReadFilterStringEvent(object o)
        {
            //throw new NotImplementedException();
        }

        void ApplicationViewModel_ApplyDataFilterEvent(object o)
        {
        }

        private void ApplicationViewModel_ColumnsLoadedEvent(object o, ColumnsLoadedEventEventArgs e)
        {
        }

        private void ApplicationViewModel_TotalRecordcountLoadedEvent(object o, TotalRecordcountLoadedEventArgs e)
        {
        }

        //private void ApplicationViewModel_DatasourceListReadyEvent(object o, DatasourceListReadyEventArgs e)
        //{
        //    //   throw new NotImplementedException();
        //}

        private void loadedGadgets_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)   //    e.Action = NotifyCollectionChangedAction.Add  
            {
                //case NotifyCollectionChangedAction.Add:
                //case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    throw new Exception("Cannot replace a gadget ");
                case NotifyCollectionChangedAction.Reset:
                    break;
            }
        }

        #endregion

        #region    Mef

        public IEnumerable<Lazy<UserControl, IMapMetaData>> mefAvailableGadgets1;

        //  MEF integration  
        [ImportMany]
        public IEnumerable<Lazy<UserControl, IMapMetaData>> MefAvailableGadgets
        {
            get
            {
                return mefAvailableGadgets1;
            }
            set
            {
                mefAvailableGadgets1 = value;
            }
        }

        //public IEnumerable<UserControl> MefAvailableGadgets { get; set; }

        /// <summary>
        /// Gets the gadgets.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Lazy<UserControl, IMapMetaData>> MefGetGadgets()
        {
            if (MefAvailableGadgets == null)
            {
                CompositionInitializer.SatisfyImports(this);
                //CompositionContainer container = new CompositionContainer();
                //container.ComposeParts(this);
                return MefAvailableGadgets;
            }
            else
            {
                return MefAvailableGadgets;
            }
        }

        public List<KeyValue> MefOrderDictionary;

        List<ControlMetaInfo> controlListWithMetaInfo = new List<ControlMetaInfo>();

        public List<ControlMetaInfo> ControlListWithMetaInfo
        {
            get
            {
                return controlListWithMetaInfo;
            }
            set
            {
                controlListWithMetaInfo = value;
            }
        }

        public void GetControlNames()
        {
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += HttpsCompleted;
            wc.DownloadStringAsync(new Uri(App.Current.Host.Source.AbsoluteUri.Replace("Ewav.xap", "Assets/Controls.xml")));
            //Uri filePath = new Uri("Controls.xml", UriKind.Relative);
            //Uri filePath = new Uri(App.Current.Host.Source, "Controls.xml");
            //client1.OpenReadAsync(filePath);
            //XDocument doc = XDocument.Load(filePath.ToString());
            //foreach (var item in doc.Descendants("Control"))
            //{
            //    if (item.Name.ToString() == "ControlName")
            //    {
            //        controlList.Add(item.Value);
            //    }
            //}
        }

        private void HttpsCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                XDocument xdoc = XDocument.Parse(e.Result, LoadOptions.None);

                //IEnumerable<Person> list = from p in doc.Descendants("Person")
                //                           select new Person
                //                           {
                //                               FirstName = (string)p.Attribute("FirstName"),
                //                               LastName = (string)p.Attribute("LastName")
                //                           };
                //DataGrid1.ItemsSource = list;
                foreach (var item in xdoc.Descendants("Controls"))
                {
                    //if (item.Name.ToString() == "ControlName")
                    //{
                    //    controlList.Add(item.Value);
                    //}
                    foreach (var subItem in item.Descendants("Control"))
                    {
                        ControlMetaInfo metaObj = new ControlMetaInfo();
                        foreach (var ctrlitem in subItem.Descendants())
                        {
                            switch (ctrlitem.Name.ToString())
                            {
                                case "ControlName":
                                    metaObj.ControlName = ctrlitem.Value;
                                    break;
                                case "UIName":
                                    metaObj.ControlUIName = ctrlitem.Value;
                                    break;
                                case "Type":
                                    metaObj.Type = ctrlitem.Value;
                                    break;
                                case "ContextMenuIndex":
                                    metaObj.ContextMenuIndex = Int32.Parse(ctrlitem.Value);
                                    break;
                                default:
                                    break;
                            }
                        }
                        ControlListWithMetaInfo.Add(metaObj);
                    }
                }
            }
        }

        private List<EwavContextMenuItem> mefEwavContextMenuItems;

        public List<EwavContextMenuItem> MefEwavContextMenuItems
        {
            get
            {
                if (mefEwavContextMenuItems == null)
                {
                    mefEwavContextMenuItems = new List<EwavContextMenuItem>();
                    MefOrderDictionary = new List<KeyValue>();

                    //GetControlNames();

                    //controlList

                    for (int i = 0; i < ControlListWithMetaInfo.Count; i++)
                    {
                        if (ControlListWithMetaInfo[i].ControlName == "XYChartControl")
                        {
                            foreach (XYControlChartTypes xyChartType in Enum.GetValues(typeof(XYControlChartTypes)))
                            {
                                if (xyChartType != XYControlChartTypes.Ignore)
                                {
                                    mefEwavContextMenuItems.Add(new EwavContextMenuItem()
                                    {
                                        Text = xyChartType.ToString(),
                                        UCName = ControlListWithMetaInfo[i].ControlName,
                                        Type = ControlListWithMetaInfo[i].Type
                                    });
                                    MefOrderDictionary.Add(new KeyValue(ControlListWithMetaInfo[i].ContextMenuIndex, new EwavContextMenuItem()
                                    {
                                        Text = xyChartType.ToString().FromCamelCase(),
                                        UCName = ControlListWithMetaInfo[i].ControlName,
                                        Type = ControlListWithMetaInfo[i].Type
                                    }));
                                }
                            }
                        }
                        else
                        {
                            mefEwavContextMenuItems.Add(new EwavContextMenuItem()
                            {
                                Text = ControlListWithMetaInfo[i].ControlUIName,
                                UCName = ControlListWithMetaInfo[i].ControlName,
                                Type = ControlListWithMetaInfo[i].Type
                            });
                            MefOrderDictionary.Add(new KeyValue(ControlListWithMetaInfo[i].ContextMenuIndex, new EwavContextMenuItem()
                            {
                                Text = ControlListWithMetaInfo[i].ControlUIName,
                                UCName = ControlListWithMetaInfo[i].ControlName,
                                Type = ControlListWithMetaInfo[i].Type
                            }));
                        }
                    }
                    //foreach (Lazy<UserControl, IMapMetaData> uc in MefGetGadgets())
                    //{
                    //    UserControl gadget = uc.Value;
                    //    //string thisUIName = ((IEwavGadget)gadget).MyUIName;
                    //    string thisControlName = ((IEwavGadget)gadget).MyControlName;
                    //    string thisGadgetType = uc.Metadata.type.ToString();
                    //    int tabindex = int.Parse(uc.Metadata.tabindex.ToString());
                    //    if (thisControlName == "XYChartControl")
                    //    {
                    //        //   foreach (string xyControlName in xYControlFamilyNames)
                    //        foreach (XYControlChartTypes xyChartType in Enum.GetValues(typeof(XYControlChartTypes)))
                    //        {
                    //            mefEwavContextMenuItems.Add(new EwavContextMenuItem()
                    //            {
                    //                Text = xyChartType.ToString(),
                    //                UCName = thisControlName,
                    //                Type = thisGadgetType
                    //            });
                    //            MefOrderDictionary.Add(new KeyValue(tabindex, new EwavContextMenuItem()
                    //            {
                    //                Text = xyChartType.ToString().FromCamelCase(),
                    //                UCName = thisControlName,
                    //                Type = thisGadgetType
                    //            }));
                    //            //string x = "aa";
                    //            //string a = "aa";
                    //            //string f = "aa";
                    //            //string g = "aa";
                    //            //string j = "aa";
                    //            //string w = "aa";
                    //            //string enumerator = "aa";
                    //        }
                    //    }
                    //    else
                    //    {
                    //        mefEwavContextMenuItems.Add(new EwavContextMenuItem()
                    //        {
                    //            Text = ((IEwavGadget)gadget).MyUIName,
                    //            UCName = ((IEwavGadget)gadget).MyControlName,
                    //            Type = thisGadgetType
                    //        });
                    //        MefOrderDictionary.Add(new KeyValue(tabindex, new EwavContextMenuItem()
                    //        {
                    //            Text = ((IEwavGadget)gadget).MyUIName,
                    //            UCName = ((IEwavGadget)gadget).MyControlName,
                    //            Type = thisGadgetType
                    //        }));
                    //    }
                    //}
                }

                return mefEwavContextMenuItems;
            }
        }

        public void MefCleanup()
        {
            //CompositionContainer container = new CompositionContainer();
            //container.Dispose();
            MefAvailableGadgets = null;
        }

        #endregion

        #region  Serialize Helpers

        XElement CanvasCurrentCulture;
        XElement CanvasHeightElement;
        XElement CanvasWidthElement;


        public XElement SerializeCanvas()
        {
            XElement element = new XElement("DashboardCanvas");


            CanvasCurrentCulture = new XElement("CanvasCurrentCulture");
            CanvasWidthElement = new XElement("CanvasWidth");
            CanvasHeightElement = new XElement("CanvasHeight");


            allGadgetXmlElements = new XElement("Gadgets");
            allDataFilterXmlElements = new XElement("DataFilters");
            allRuleXmlElements = new XElement("Rules");

            SerializeEwavDatafilters();
            SerializeRules();
            SerializeGadgets();

            CanvasCurrentCulture.Add(CultureInfo.CurrentCulture.Name);
            CanvasWidthElement.Add(CanvasWidth == 0 ? CANVAS_DEFAULT_WIDTH : CanvasWidth);
            CanvasHeightElement.Add(CanvasHeight == 0 ? CANVAS_DEFAULT_HEIGHT : CanvasHeight);



            element.Add(CanvasCurrentCulture);
            element.Add(CanvasWidthElement);
            element.Add(CanvasHeightElement);

            element.Add(allRuleXmlElements);
            element.Add(allDataFilterXmlElements);
            element.Add(allGadgetXmlElements);

            return element;
        }

        /// <summary>
        /// Collect XML from all datafilters ahd populate  EwavDatafiltersAsXML 
        /// </summary>
        private void SerializeEwavDatafilters()
        {
            EwavDatafilters.Serialize(allDataFilterXmlElements, ApplicationViewModel.Instance.AdvancedDataFilterString);
        }

        /// <summary>
        /// Serializes Rules and retuns XElement with all the data
        /// </summary>
        /// <returns></returns>
        private void SerializeRules()
        {
            for (int i = 0; i < this.ListOfRules.Count; i++)
            {
                EwavRule_Base rule = this.ListOfRules[i].Rule;
                XElement element = rule.Serialize();
                element.Add(new XAttribute("order", i));
                allRuleXmlElements.Add(element);
            }
            //return element;
        }

        /// <summary>
        /// Serialzes Gadgets and returns XElement with all the data.
        /// </summary>
        /// <returns></returns>
        private void SerializeGadgets()
        {
            List<UserControl> listOfGadget = this.Gadgets;
            for (int i = 0; i < listOfGadget.Count; i++)
            {
                if (listOfGadget[i] is IGadget)
                {
                    allGadgetXmlElements.Add(((IGadget)listOfGadget[i]).Serialize(new XDocument()));
                }
            }
        }

        #endregion

        #region    Canvas Helpers

        public void ShareCanvas(int canvasId, List<int> userIds)
        {
            // This may to be uncommented 
            //cvm = new CanvasViewModel();
            if (cvm == null)
            {
                cvm = new CanvasViewModel();
            }
            cvm.ShareCanvas(canvasId, userIds);
            cvm.CanvasShared -= new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(cvm_CanvasShared);
            cvm.CanvasShared += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(cvm_CanvasShared);
        }

        public void ResendEmail(int canvasId, List<int> userIds)
        {
            if (cvm == null)
            {
                cvm = new CanvasViewModel();
            }
            cvm.ResendEmail(canvasId, userIds);
            cvm.EmailResent -= new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(cvm_EmailResent);
            cvm.EmailResent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(cvm_EmailResent);
        }

        void cvm_EmailResent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            this.EmailSentCompletedEvent(sender);
        }

        void cvm_CanvasShared(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {

            // re-query shared status                 

            GetShareCanvasStatus();


            this.ShareCanvasCompletedEvent(sender);


        }

        public void GetShareCanvasStatus()
        {
            cvm.CanvasShareStatusDtoLoaded -= new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(cvm_CanvasShareStatusDtoLoaded);
            cvm.CanvasShareStatusDtoLoaded += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(cvm_CanvasShareStatusDtoLoaded);
            cvm.GetCanvasSharedStatus(CurrentCanvas.CanvasId, EwavSelectedDatasource.OrganizationId);

        }

        /// <summary>
        /// Saves Canvas
        /// </summary>
        /// <param name="element"></param>
        public void SaveCanvas(XElement element, CanvasDto canvasToSave)
        {
            // This may need to be uncommented 
            //cvm = new CanvasViewModel();
            if (cvm == null)
            {
                cvm = new CanvasViewModel();
            }
            cvm.CanvasSaved += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(cvm_CanvasSaved);
            cvm.ErrorNotice += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(cvm_ErrorNotice);


            cvm.SaveCanvas(canvasToSave);
            //LoadCanvasUserList(dto.UserId);
        }

        /// <summary>
        /// Deletes canvas
        /// </summary>
        /// <param name="canvasId"></param>
        public void DeleteCanvas(int canvasId)
        {
            //cvm = new CanvasViewModel();
            if (cvm == null)
            {
                cvm = new CanvasViewModel();
            }
            cvm.DeleteCanvas(canvasId);
            cvm.CanvasDeleted += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(cvm_CanvasDeleted);
        }

        void cvm_CanvasDeleted(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            this.DeleteCanvasCompletedEvent(this);
            this.ClearDataFiltersEvent(this);
            this.ClearDefinedVariablesEvent(this);
        }

        void cvm_ErrorNotice(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            this.ErrorNoticeEvent(e);
        }


        public event EventHandler SnapshotRequestEvent;

        void cvm_CanvasSaved(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            this.CurrentCanvasId = cvm.SavedCanvasId;//.CanvasId;        
            this.LoadJustSavedCanvas(this.CurrentCanvasId);




            if (this.SaveCanvasCompletedEvent != null)
            {

                this.SaveCanvasCompletedEvent(sender);


            }

        }

        public void LoadCanvasUserList(int UserId)
        {
            //cvm = new CanvasViewModel();
            if (cvm == null)
            {
                cvm = new CanvasViewModel();
            }
            cvm.LoadCanvasForUserList(UserId);
            cvm.UserListLoaded -= new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(cvm_UserListLoaded);
            cvm.UserListLoaded += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(cvm_UserListLoaded);
        }

        /// <summary>
        /// Fills the canvas DTO list.
        /// </summary>
        /// <param name="results">The results.</param>
        private void FillCanvasDTOList(DatatableBag results)
        {

            this.CanvasDtoListForLoggedUser.Clear();


            for (int i = 0; i < results.RecordList.Count; i++)
            {
                this.CanvasDtoListForLoggedUser.Add(
                    new CanvasDto()
                    {
                        CanvasId = Convert.ToInt32(results.GetValueAtRow("CanvasID", results.RecordList[i])),
                        CanvasName = results.GetValueAtRow("CanvasName", results.RecordList[i]),
                        UserId = Convert.ToInt32(results.GetValueAtRow("UserID", results.RecordList[i])),
                        CanvasDescription = results.GetValueAtRow("CanvasDescription", results.RecordList[i]),
                        CreatedDate = Convert.ToDateTime(results.GetValueAtRow("CreatedDate", results.RecordList[i])),
                        ModifiedDate = Convert.ToDateTime(results.GetValueAtRow("ModifiedDate", results.RecordList[i])),
                        DatasourceID = Convert.ToInt32(results.GetValueAtRow("DatasourceID", results.RecordList[i])),
                        Status = results.GetValueAtRow("Status", results.RecordList[i]),
                        Datasource = results.GetValueAtRow("DatasourceName", results.RecordList[i])
                    });
            }


            CanvasListLoadedEvent(CanvasListForUser);

        }

        void cvm_UserListLoaded(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            CanvasListForUser = cvm.Results;
            FillCanvasDTOList(CanvasListForUser);
        }


        CanvasViewModel cvm_justCreated;

        /// <summary>
        /// Loads the just saved canvas.
        /// </summary>
        /// <param name="canvasId">The canvas id.</param>
        private void LoadJustSavedCanvas(int canvasId)
        {
            //cvm = new CanvasViewModel();
            if (cvm_justCreated == null)
            {
                cvm_justCreated = new CanvasViewModel();
            }
            cvm_justCreated.LoadCanvas(canvasId);
            cvm_justCreated.CanvasLoaded -= new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(cvm_justCreated_CanvasLoaded);
            cvm_justCreated.CanvasLoaded += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(cvm_justCreated_CanvasLoaded);
        }

        void cvm_justCreated_CanvasLoaded(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            this.CurrentCanvas = cvm_justCreated.LoadedCanvas;

            // Only now raise  SaveCanvasCompletedEvent               
            if (this.SaveCanvasCompletedEvent != null)
            {



                this.SaveCanvasCompletedEvent(sender);


            }

        }

        public void LoadCanvas(int canvasId)
        {
            //cvm = new CanvasViewModel();
            if (cvm == null)
            {
                cvm = new CanvasViewModel();
            }
            cvm.LoadCanvas(canvasId);
            cvm.CanvasLoaded -= new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(cvm_CanvasLoaded);
            cvm.CanvasLoaded += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(cvm_CanvasLoaded);
        }

        public CanvasDto CurrentCanvas;

        void cvm_CanvasLoaded(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            CleanupDashboard();
            //Creates from XML    

            CreateControlsFromLoadedXML();

            this.UserIdForOpenedCanvas = cvm.UserIdOfOpenedCanvas;
            this.IsCurrentCanvasShared = cvm.IsCurrentCanvasShared;

            if (this.UserIdForOpenedCanvas == Convert.ToInt32(this.LoggedInUser.UserDto.UserID))
            {
                this.CurrentCanvasId = cvm.CanvasIdOfOpenedCanvas;
                this.CurrentCanvas = cvm.LoadedCanvas;



                //   GetShareCanvasStatus(); Do not load the share list while loading a canvas, it is loading when share Canvas button is clicked.

            }
            else
            {
                this.CurrentCanvasId = -1;
            }
        }





        #endregion

        #region  Other Helpers

        public void ReadUserNames()
        {
            UserViewModel uvm = new UserViewModel();
            uvm.ReadUserNamesFromEwav();
            uvm.ReadUserNamesLoaded -= new EventHandler(uvm_ReadUserNamesLoaded);
            uvm.ReadUserNamesLoaded += new EventHandler(uvm_ReadUserNamesLoaded);
        }

        void uvm_ReadUserNamesLoaded(object sender, EventArgs e)
        {
            this.UserNames = (List<string>)sender;
        }

        /// <summary>
        ///   Just cleans up gadgets.  Does not touch user, filters or vars       
        /// </summary>
        public void ResetDashBoard()
        {
            IsReset = true;
            CleanupDashboard();
            IsReset = false;
            CurrentCanvasId = -1;
        }

        /// <summary>
        ///         
        /// </summary>
        public void CloseGadget(UserControl userControl)
        {
            DragCanvas dragCanvas = MainCanvas as DragCanvas;
            MainCanvas.Children.Remove(userControl);
            dragCanvas.Cleanup(userControl);
            // UnloadGadget();
        }

        //  NOTE:  Check this file in TFS for logout code that is currently not needed    

        public void ReadAllUsersInMyOrg()
        {


            cvm.AllUsersInMyOrgLoaded -= new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(cvm_AllUsersInMyOrgLoaded);
            cvm.CanvasShareStatusDtoLoaded -= new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(cvm_CanvasShareStatusDtoLoaded);

            // NOTE:  the  methods that calls this asynchronous method DO NOT assume it is asynchronous!  
            // it is not causing problems now but it should be fixed ASAP    
            if (cvm == null)
            {
                cvm = new CanvasViewModel();
            }
            cvm.ReadAllUsersInMyOrg(EwavSelectedDatasource.OrganizationId);
            cvm.AllUsersInMyOrgLoaded += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(cvm_AllUsersInMyOrgLoaded);



        }

        void cvm_CanvasShareStatusDtoLoaded(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {

            var cssDto = from user in cvm.CanvasShareStatusDtoList
                         where user.UserID != LoggedInUser.UserDto.UserID
                         select user;


            CurrentCanvasShareStatusDto = cssDto.ToList();

            if (this.ShareCanvasLoadedEvent != null)
                this.ShareCanvasLoadedEvent(sender);

        }

        void cvm_AllUsersInMyOrgLoaded(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            AllUsersInMyOrg = cvm.AllUsersInMyOrg;
        }



        public void CleanupDashboard()
        {
            //EwavDatafilters = new System.Collections.Generic.List<EwavDataFilterCondition>();
            //Clears the defined Vars
            //EwavDefinedVariables = new System.Collections.Generic.List<EwavRule_Base>();
            //ListOfRules = new List<ListBoxItemSource>();   

            if (CurrentCanvasShareStatusDto != null)
                CurrentCanvasShareStatusDto.ForEach(d => d.Shared = false);


            this.UserIdForOpenedCanvas = -1;
            this.CurrentCanvasId = -1;
            //Clears the Gadget Collection
            Gadgets.Clear();

            //Unloads the objects from memory
            if (!IsReset)
            {
                UnloadedEvent(this);
                //removes the user defined columns.
                for (int i = 0; i < this.EwavSelectedDatasource.AllColumns.Count; i++)
                {
                    if (this.EwavSelectedDatasource.AllColumns[i].IsUserDefined)
                    {
                        this.EwavSelectedDatasource.AllColumns.RemoveAt(i);
                    }
                }
            }

            MainCanvas.Children.Clear();
        }

        public event EventHandler CanvasHeightWidthLoaded;
        public string DemoModeUser;
        public string DemoModePassword;
        internal string ServerAssemblyVersion;
        internal string ClientAssemblyVersion;

        private void CreateControlsFromLoadedXML()
        {

            try
            {
                XDocument doc = XDocument.Parse(cvm.LoadedXMLString);

                LoadedCanvasDto = new CanvasDto();

                IEnumerator<XElement> enumerator = doc.Descendants("CanvasCurrentCulture").GetEnumerator();


                //  Current culture 
                enumerator.MoveNext();
                if (enumerator.Current == null)
                    LoadedCanvasDto.Culture = CultureInfo.CurrentCulture.Name;
                else
                    LoadedCanvasDto.Culture = enumerator.Current.Value.ToString();


                // Canvas Width    
                enumerator = doc.Descendants("CanvasWidth").GetEnumerator();
                enumerator.MoveNext();
                if (enumerator.Current == null)
                {
                    LoadedCanvasDto.Width = Defaults.CANVAS_WIDTH;

                }
                else
                {
                    LoadedCanvasDto.Width = Convert.ToDouble(enumerator.Current.Value.ToString());

                }



                // Canvas Height
                enumerator = doc.Descendants("CanvasHeight").GetEnumerator();
                enumerator.MoveNext();
                if (enumerator.Current == null)
                {
                    LoadedCanvasDto.Height = Defaults.CANVAS_HEIGHT;

                }
                else
                {
                    LoadedCanvasDto.Height = Convert.ToDouble(enumerator.Current.Value.ToString());

                }

                CanvasHeightWidthLoaded(this, new EventArgs());



                this.EwavDefinedVariables = new List<EwavRule_Base>();
                ListBoxItemSource listBoxItem = null;
                this.ListOfRules = new List<ListBoxItemSource>();
                ScrollUpEvent(this, new EventArgs());
                foreach (var item in doc.Descendants("Rules").Descendants())
                {

                    Debug.WriteLine(item);
                    Debug.WriteLine("===============================");                           


                    if (item.Name.ToString().ToLower() == "rule")
                    {
                        try
                        {
                            //Type gadgetType = Type.GetType(item.Attribute("type").Value); // item.Attributes["gadgetType"].Value);
                            //EwavRule_Base rule = null;
                            EwavRule_Base baseRule = null;
                            EwavColumn newColumn = null;
                            listBoxItem = null;


                            switch (item.Attribute("type").Value.ToLower())
                            {
                                case "rule_format":
                                    EwavRule_Format rule = new EwavRule_Format();
                                    rule.FriendlyLabel = item.Element("friendlyRule").Value.ToString();
                                    rule.CbxFieldName = item.Element("sourceColumnName").Value.ToString();
                                    rule.TxtDestinationField = item.Element("destinationColumnName").Value.ToString();
                                    rule.CbxFormatOptions = item.Element("formatString").Value.ToString();
                                    rule.FormatTypes = (Ewav.Web.EpiDashboard.Rules.FormatTypes)Enum.Parse(typeof(Ewav.Web.EpiDashboard.Rules.FormatTypes),
                                        item.Element("formatType").Value.ToString(), true);
                                    rule.VaraiableDataType = item.Element("variableDataType").Value.ToString();

                                    listBoxItem = new ListBoxItemSource();
                                    listBoxItem.RuleString = rule.FriendlyLabel;
                                    listBoxItem.DestinationColumn = rule.TxtDestinationField;
                                    listBoxItem.RuleType = EwavRuleType.Formatted;
                                    listBoxItem.Rule = rule;
                                    baseRule = rule;
                                    baseRule.VaraiableName = rule.TxtDestinationField;

                                    newColumn = new EwavColumn();
                                    newColumn.Name = rule.TxtDestinationField;
                                    newColumn.SqlDataTypeAsString = Ewav.Web.EpiDashboard.ColumnDataType.Numeric;
                                    newColumn.NoCamelName = rule.TxtDestinationField;
                                    newColumn.IsUserDefined = true;

                                    break;
                                case "rule_expressionassign":
                                    EwavRule_ExpressionAssign ruleAssign = new EwavRule_ExpressionAssign();
                                    ruleAssign.FriendlyRule = item.Element("friendlyRule").Value.ToString();
                                    ruleAssign.Expression = item.Element("expression").Value.ToString();
                                    ruleAssign.DestinationColumnName = item.Element("destinationColumnName").Value.ToString();
                                    ruleAssign.DataType = item.Element("destinationColumnType").Value.ToString();
                                    ruleAssign.VaraiableDataType = item.Element("variableDataType").Value.ToString();

                                    listBoxItem = new ListBoxItemSource();
                                    listBoxItem.RuleString = ruleAssign.FriendlyRule;
                                    listBoxItem.DestinationColumn = ruleAssign.DestinationColumnName;
                                    listBoxItem.NewColumn = ruleAssign.DestinationColumnName;
                                    listBoxItem.AssignExpression = ruleAssign.Expression;
                                    listBoxItem.RuleType = EwavRuleType.Assign;
                                    listBoxItem.Rule = ruleAssign;
                                    baseRule = ruleAssign;
                                    baseRule.VaraiableName = ruleAssign.DestinationColumnName;

                                    newColumn = new EwavColumn();
                                    newColumn.Name = ruleAssign.DestinationColumnName;
                                    newColumn.SqlDataTypeAsString = Ewav.Web.EpiDashboard.ColumnDataType.Numeric;
                                    newColumn.NoCamelName = ruleAssign.DestinationColumnName;
                                    newColumn.IsUserDefined = true;

                                    break;
                                case "rule_groupvariable":
                                    EwavRule_GroupVariable ruleGroupVar = new EwavRule_GroupVariable();
                                    ruleGroupVar.FriendlyLabel = item.Element("friendlyLabel").Value.ToString();
                                    ruleGroupVar.VaraiableName = item.Element("destinationColumnName").Value.ToString();
                                    ruleGroupVar.VaraiableDataType = item.Element("variableDataType").Value.ToString();
                                    List<MyString> columnList = new List<MyString>();
                                    foreach (var column in item.Descendants("column"))
                                    {
                                        MyString colVal = new MyString();
                                        colVal.VarName = column.Value.ToString();
                                        columnList.Add(colVal);
                                    }
                                    ruleGroupVar.Items = columnList;

                                    listBoxItem = new ListBoxItemSource();
                                    listBoxItem.RuleString = ruleGroupVar.FriendlyLabel;
                                    listBoxItem.NewColumn = ruleGroupVar.VaraiableName;
                                    listBoxItem.DestinationColumn = ruleGroupVar.VaraiableName;
                                    listBoxItem.RuleType = EwavRuleType.GroupVariable;
                                    listBoxItem.Rule = ruleGroupVar;
                                    baseRule = ruleGroupVar;
                                    baseRule.VaraiableName = ruleGroupVar.VaraiableName;

                                    newColumn = new EwavColumn();
                                    newColumn.Name = ruleGroupVar.VaraiableName;
                                    newColumn.SqlDataTypeAsString = Ewav.Web.EpiDashboard.ColumnDataType.GroupVariable;
                                    newColumn.NoCamelName = ruleGroupVar.VaraiableName;
                                    newColumn.IsUserDefined = true;

                                    break;
                                case "rule_conditionalassign":
                                    EwavRule_ConditionalAssign ruleCondAssign = new EwavRule_ConditionalAssign();
                                    MyString myString = new MyString();
                                    myString.VarName = item.Element("friendlyRule").Value.ToString();
                                    ruleCondAssign.FriendlyRule = myString;
                                    ruleCondAssign.TxtDestination = item.Element("destinationColumnName").Value.ToString();
                                    ruleCondAssign.DestinationColumnType = item.Element("destinationColumnType").Value.ToString();
                                    ruleCondAssign.AssignValue = item.Element("assignValue").Value.ToString();
                                    ruleCondAssign.ElseValue = item.Element("elseValue").Value.ToString();
                                    ruleCondAssign.VaraiableDataType = item.Element("variableDataType").Value.ToString();
                                    if (item.Element("cbxFieldType") != null)
                                    {
                                        ruleCondAssign.CbxFieldType = (cbxFieldTypeEnum)Enum.Parse(typeof(cbxFieldTypeEnum), item.Element("cbxFieldType").Value.ToString(), false);
                                    }

                                    ruleCondAssign.ConditionsList = new List<EwavDataFilterCondition>();
                                    //ruleCondAssign.ConditionsList = 

                                    foreach (var condition in item.Descendants("EwavDataFilterCondition").OrderBy(x => (int)x.Attribute("order")))
                                    {
                                        EwavDataFilterCondition df = new EwavDataFilterCondition();
                                        if (condition.Attribute("friendlyOperand") != null)
                                        {
                                            df.FriendlyOperand = condition.Attribute("friendlyOperand").Value.ToMyString();
                                        }

                                        if (condition.Attribute("friendlyValue") != null)
                                        {
                                            df.FriendlyValue = condition.Attribute("friendlyValue").Value.ToMyString();
                                        }

                                        if (condition.Attribute("fieldName") != null)
                                        {
                                            df.FieldName = condition.Attribute("fieldName").Value.ToMyString();
                                        }

                                        if (condition.Attribute("joinType") != null)
                                        {
                                            df.JoinType = condition.Attribute("joinType").Value.ToMyString();
                                        }

                                        if (condition.Attribute("friendLowValue") != null &&
                                            condition.Attribute("friendLowValue").Value != "null")
                                        {
                                            df.FriendLowValue = condition.Attribute("friendLowValue").Value.ToMyString();
                                        }

                                        if (condition.Attribute("friendHighValue") != null &&
                                            condition.Attribute("friendHighValue").Value != "null")
                                        {
                                            df.FriendHighValue = condition.Attribute("friendHighValue").Value.ToMyString();
                                        }

                                        ruleCondAssign.ConditionsList.Add(df);
                                    }

                                    listBoxItem = new ListBoxItemSource();
                                    listBoxItem.RuleString = ruleCondAssign.FriendlyRule.VarName;
                                    listBoxItem.DestinationColumn = ruleCondAssign.TxtDestination;
                                    listBoxItem.RuleType = EwavRuleType.conditional;
                                    listBoxItem.Rule = ruleCondAssign;
                                    //listBoxItem.FilterConditionsPanel = pnlContainer;
                                    baseRule = ruleCondAssign;
                                    baseRule.VaraiableName = ruleCondAssign.TxtDestination;

                                    newColumn = new EwavColumn();
                                    newColumn.Name = ruleCondAssign.TxtDestination;
                                    newColumn.SqlDataTypeAsString = Ewav.Web.EpiDashboard.ColumnDataType.Text;
                                    newColumn.NoCamelName = ruleCondAssign.TxtDestination;
                                    newColumn.IsUserDefined = true;

                                    break;
                                case "rule_simpleassign":
                                    EwavRule_SimpleAssignment ruleSimple = new EwavRule_SimpleAssignment();
                                    ruleSimple.FriendlyLabel = item.Element("friendlyRule").Value.ToString();
                                    ruleSimple.AssignmentType = (Ewav.Web.EpiDashboard.Rules.SimpleAssignType)Enum.Parse(typeof(Ewav.Web.EpiDashboard.Rules.SimpleAssignType),
                                        item.Element("assignmentType").Value.ToString(),
                                        true);
                                    ruleSimple.TxtDestinationField = item.Element("destinationColumnName").Value.ToString();
                                    ruleSimple.Parameters = new List<MyString>();
                                    ruleSimple.VaraiableDataType = item.Element("variableDataType").Value.ToString();

                                    foreach (var item1 in item.Element("parametersList").Descendants())
                                    {
                                        MyString mys = new MyString();
                                        mys.VarName = item1.Value;
                                        ruleSimple.Parameters.Add(mys);
                                    }

                                    listBoxItem = new ListBoxItemSource();
                                    listBoxItem.RuleString = ruleSimple.FriendlyLabel;
                                    listBoxItem.DestinationColumn = ruleSimple.TxtDestinationField;
                                    listBoxItem.RuleType = EwavRuleType.Simple;
                                    listBoxItem.Rule = ruleSimple;
                                    baseRule = ruleSimple;
                                    baseRule.VaraiableName = ruleSimple.TxtDestinationField;

                                    newColumn = new EwavColumn();
                                    newColumn.Name = ruleSimple.TxtDestinationField;
                                    newColumn.SqlDataTypeAsString = Ewav.Web.EpiDashboard.ColumnDataType.Numeric;
                                    newColumn.NoCamelName = ruleSimple.TxtDestinationField;
                                    newColumn.IsUserDefined = true;

                                    break;
                                case "rule_recode":
                                    EwavRule_Recode ruleRecode = new EwavRule_Recode();
                                    ruleRecode.Friendlyrule = item.Element("friendlyRule").Value.ToString();
                                    ruleRecode.SourceColumnName = item.Element("sourceColumnName").Value.ToString();
                                    ruleRecode.SourceColumnType = item.Element("sourceColumnType").Value.ToString();
                                    ruleRecode.TxtDestinationField = item.Element("destinationColumnName").Value.ToString();
                                    ruleRecode.DestinationFieldType = (DashboardVariableType)Enum.Parse(typeof(DashboardVariableType), item.Element("destinationColumnType").Value.ToString(), true);
                                    ruleRecode.TxtElseValue = item.Element("elseValue").Value.ToString();
                                    ruleRecode.CheckboxUseWildcardsIndicator = bool.Parse(item.Element("shouldUseWildcards").Value.ToString());
                                    ruleRecode.CheckboxMaintainSortOrderIndicator = bool.Parse(item.Element("shouldMaintainSortOrder").Value.ToString());
                                    ruleRecode.VaraiableName = item.Element("destinationColumnName").Value.ToString();
                                    ruleRecode.VaraiableDataType = item.Element("variableDataType").Value.ToString();
                                    //ruleRecode.VaraiableDataType = item.Element("variableDataType").Value.ToString();
                                    List<EwavRuleRecodeDataRow> rows = new List<EwavRuleRecodeDataRow>();

                                    foreach (var item2 in item.Descendants("recodeTable"))
                                    {
                                        var itemmm = item2;
                                        foreach (var item3 in item2.Elements("recodeTableRow"))
                                        {
                                            EwavRuleRecodeDataRow row = new EwavRuleRecodeDataRow();
                                            IEnumerable<XElement> enumerableList = item3.Elements("recodeTableData");

                                            List<XElement> list = enumerableList.ToList();

                                            if (list.Count == 2)
                                            {
                                                row.col1 = list[0].Value.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                                                row.col3 = list[1].Value.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                                            }
                                            else
                                            {
                                                row.col1 = list[0].Value.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                                                row.col2 = list[1].Value.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                                                row.col3 = list[2].Value.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                                            }
                                            rows.Add(row);
                                        }
                                    }

                                    ruleRecode.RecodeTable = rows;

                                    //}

                                    newColumn = new EwavColumn();
                                    newColumn.Name = ruleRecode.TxtDestinationField;

                                    newColumn.NoCamelName = ruleRecode.TxtDestinationField;
                                    newColumn.IsUserDefined = true;

                                    listBoxItem = new ListBoxItemSource();
                                    listBoxItem.RuleString = ruleRecode.Friendlyrule;
                                    listBoxItem.SourceColumn = ruleRecode.SourceColumnName;
                                    listBoxItem.DestinationColumn = ruleRecode.TxtDestinationField;
                                    listBoxItem.Rule = ruleRecode;
                                    baseRule = ruleRecode;
                                    baseRule.VaraiableName = ruleRecode.TxtDestinationField;
                                    break;
                                default:
                                    throw new Exception("This Rule doesn't exists.");
                            }
                            //item.Element("variableDataType").Value.ToString();
                            newColumn.SqlDataTypeAsString = (ColumnDataType)Enum.Parse(typeof(ColumnDataType), item.Element("variableDataType").Value.ToString(), false);
                            this.EwavDefinedVariables.Add(baseRule);
                            this.ListOfRules.Add(listBoxItem);
                            this.EwavSelectedDatasource.AllColumns.Add(newColumn);
                        }
                        catch (Exception ex)
                        {
                            //Epi.Windows.MsgBox.ShowError(DashboardSharedStrings.GADGET_LOAD_ERROR);
                            throw new Exception("Exception occured deserializing Rules." + ex.Message);
                            //return;
                        }

                    }
                }
                Ewav.VariablesControl ctrl = (Ewav.VariablesControl)Activator.CreateInstance(typeof(Ewav.VariablesControl));
                RulesAddedEvent(ctrl);

                //this.EwavDatafilters = new List<EwavDataFilterCondition>();

                try
                {
                    List<EwavDataFilterCondition> filterList = new List<EwavDataFilterCondition>();

                    Ewav.DataFilterControl ctrl2 = (Ewav.DataFilterControl)Activator.CreateInstance(typeof(Ewav.DataFilterControl));

                    foreach (var item in doc.Descendants("DataFilters").Descendants())
                    {
                        if (item.Name == "EwavDataFilterCondition")
                        {
                            EwavDataFilterCondition condition = new EwavDataFilterCondition();
                            condition.FieldName = new MyString();
                            condition.FieldName.VarName = item.Attribute("fieldName").Value.ToString();

                            condition.FriendlyOperand = new MyString();
                            condition.FriendlyOperand.VarName = item.Attribute("friendlyOperand").Value.ToString();

                            if (item.Attribute("friendlyValue").Value.ToString() != "null")
                            {
                                condition.FriendlyValue = new MyString();
                                condition.FriendlyValue.VarName = item.Attribute("friendlyValue").Value.ToString();
                            }

                            if (item.Attribute("friendLowValue").Value.ToString() != "null")
                            {
                                condition.FriendLowValue = new MyString();
                                condition.FriendLowValue.VarName = item.Attribute("friendLowValue").Value.ToString();
                            }

                            if (item.Attribute("friendHighValue").Value.ToString() != "null")
                            {
                                condition.FriendHighValue = new MyString();
                                condition.FriendHighValue.VarName = item.Attribute("friendHighValue").Value.ToString();
                            }

                            condition.JoinType = new MyString();
                            condition.JoinType.VarName = item.Attribute("joinType").Value.ToString();

                            filterList.Add(condition);
                            //this.EwavDatafilters.Add(
                        }
                        if (item.Name == "EwavAdvancedFilterString")
                        {
                            this.UseAdvancedFilter = true;
                            this.AdvancedDataFilterString = item.Value;
                            break;
                        }
                    }





                    this.EwavDatafilters = filterList;

                    ctrl2.CreateFromXml(EwavDatafilters, this.AdvancedDataFilterString);
                    FiltersDeserializedEvent(ctrl2);



                }
                catch (Exception ex)
                {


                    throw new Exception("Error loading rules " + ex.Message);




                }




                foreach (var item in doc.Descendants("Gadgets").Descendants())
                {
                    if (item.Name == "chart" || item.Name == "gadget")
                    {
                        try
                        {
                            //Type gadgetType = Type.GetType(item.Attribute("gadgetType").Value); // item.Attributes["gadgetType"].Value);
                            //// The 2x2 gadget was removed and its functionality absorbed into the MxN (crosstab) gadget.
                            //// This code, along with updates to the CreateFromXml routine in the Crosstab gadget, ensures
                            //// that 2x2 gadgets created in canvas files from versions 7.0.9.61 and prior will still be
                            //// loaded without issue. They will now be loaded as Crosstab gadgets instead.
                            //if (item.Attribute("gadgetType").Value.Equals("EpiDashboard.TwoByTwoTableControl"))
                            //{
                            //    gadgetType = Type.GetType("EpiDashboard.CrosstabControl");
                            //}
                            //IGadget gadget = (IGadget)Activator.CreateInstance(gadgetType);
                            GadgetManager gm = new GadgetManager();
                            string gName = item.Attribute("gadgetType").Value;
                            IGadget gadget = (IGadget)gm.LoadGadget(gName);
                            gadget.CreateFromXml(item);
                            //AddGadgetToCanvasFromFile(gadget);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error loading gadgets. " + ex.Message);
                        }
                    }
                }

            }
            catch (Exception ex)
            {


                throw new Exception("Error loading the  Canvas   " + ex.Message);



            }

        }


        #endregion

        public string DatasourceDisplayText { get; set; }

        public bool DemoMode
        {
            get { return demoMode; }
            set { demoMode = value; }
        }

        public void DashToHtml()
        {

            System.Text.StringBuilder dashHTML = new System.Text.StringBuilder();

            foreach (var userControl1 in gadgets)
            {
                var userControl = (IGadget)userControl1;

                userControl.ToHTML(true);

                dashHTML.Append(userControl.HtmlBuilder.ToString());

                System.Windows.Browser.HtmlPage.Window.Invoke("DisplayFormattedText", dashHTML.ToString());

                //  userControl.ToHTML(    )



            }
        }
    }
}