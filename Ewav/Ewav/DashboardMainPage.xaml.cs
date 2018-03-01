/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       DashboardMainPage.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Ewav.ContextMenu;
using Ewav.ViewModels;
using Ewav.Client.Application;

using Ewav.DTO;
using Ewav.Common;


namespace Ewav
{
    public delegate void GadgetClosingHandler(UserControl gadget);
    public delegate void GadgetProcessingFinishedHandler(UserControl gadget);

    public delegate void HTMLGeneratedHandler();
    public delegate void RecordCountChangedHandler(int recordCount, string dataSourceName);
    public delegate void GadgetStatusUpdateHandler(string statusMessage);
    public delegate void GadgetProgressUpdateHandler(string statusMessage, double progress);
    public delegate bool GadgetCheckForCancellationHandler();



    public partial class DashboardMainPage : UserControl
    {



        private Menu popupMenu;
        private ContextMenuCreator ctxMenuCreator = new ContextMenuCreator();

        public ApplicationViewModel applicationViewModel;
        SetDatasource setDS = null;


        AppMenuViewModel appMenuViewModel = new AppMenuViewModel();

        /// <summary>
        /// Gets or sets the P full screen button.
        /// </summary>
        /// <value>The P full screen button.</value>
        public Button FullScreenButton
        {
            get
            {
                return btnFullScreen;
            }



        }

        public bool storyboardFlag = false;

        public static event DragCanvasRightMouseDownEventHandler DragCanvasRightMouseDownEvent;

        DataFilterControl dtCtrl = null;
        VariablesControl vCtrl = null;

        //  MEF integration  
        //[ImportMany]
        //public IEnumerable<UserControl> Gadgets { get; set; }

        public DashboardMainPage()
        {


            InitializeComponent();
            //    CompositionInitializer.SatisfyImports(this);

            this.Loaded += new RoutedEventHandler(DashboardMainPage_Loaded);
            ContextMenuCreator.MenuPopup += new EventHandler(ContextMenuCreator_MenuPopup);
            AppMenuView.DashhoardReset += new EventHandler(AppMenuView_DashhoardReset);
            //adminMenuView.UsersClickEvent += new UsersClickCompletedEventHandler(adminMenuView_UsersClickEvent);
            adminMenuView.AdminViewClickEvent += new EventHandler(adminMenuView_AdminViewClickEvent);
            appMenuViewModel.RecordcountRecievedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(appMenuViewModel_RecordcountRecievedEvent);    



        }

        void applicationViewModel_ApplyDataFilterEvent(object o)
        {
            spButtons.Visibility = System.Windows.Visibility.Collapsed;
        }

        void applicationViewModel_RulesAddedEvent(object o)
        {
            spButtons.Visibility = System.Windows.Visibility.Collapsed;
        }


        void applicationViewModel_ScrollUpEvent(object sender, EventArgs e)
        {
            scrollViewer.ScrollToTop();
        }

        void adminMenuView_AdminViewClickEvent(object sender, EventArgs e)
        {
            grdWelcomeAdmin.Visibility = System.Windows.Visibility.Collapsed;
            spAdminBox.Children.Clear();
            switch (((Button)sender).Name.ToLower())
            {
                case "manageorg":
                    Organizations org = new Organizations();
                    org.Margin = new Thickness(0, 40, 0, 0);
                    spAdminBox.Children.Add(org);
                    break;
                case "manageusers":
                    Users users = new Users();
                    users.Margin = new Thickness(0, 40, 0, 0);
                    spAdminBox.Children.Add(users);
                    break;
                case "managedatasources":
                    DataSources dataSources = new DataSources();
                    dataSources.Margin = new Thickness(0, 40, 0, 0);
                    spAdminBox.Children.Add(dataSources);
                    break;
                case "copydashboard":
                    CopyDash copydashboard = new CopyDash();
                    copydashboard.Margin = new Thickness(0, 40, 0, 0);
                    spAdminBox.Children.Add(copydashboard);
                    break;         

            }
        }

        //void adminMenuView_OrganizationClickEvent(object sender, EventArgs e)
        //{
        //    // Optimize:  check for memeory leaks  
        //    grdWelcomeAdmin.Visibility = System.Windows.Visibility.Collapsed;

        //}

        //void adminMenuView_UsersClickEvent(object o)
        //{
        //    // Optimize:  check for memeory leaks    
        //    grdWelcomeAdmin.Visibility = System.Windows.Visibility.Collapsed;

        //}

        void applicationViewModel_FiltersDeserializedEvent(object o)
        {
            if (dtCtrl != null)
            {
                grdDF.Children.Remove(dtCtrl);
            }

            dtCtrl = (DataFilterControl)o;
            AddDFToGrid();
        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            // Just saved the app from 2 1/2 tons of high-drama diva code        
            HtmlPage.Document.Submit();

        }


        void AppMenuView_DashhoardReset(object sender, EventArgs e)
        {
            WelcomeStackPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        void ContextMenuCreator_MenuPopup(object sender, EventArgs e)
        {
            WelcomeStackPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        void DashboardMainPage_Loaded(object sender, RoutedEventArgs e)
        {


            Construct();

            //thumb.Width = 400;
            //thumb.Height = 500;
            //double top = 110;
            //double left = 1000;
            //thumb.SetValue(Canvas.TopProperty, top);

            //thumb.SetValue(Canvas.LeftProperty, left);
            //Canvas.SetLeft(thumb, 1000d);    




            //ScaleTransform scaleTransform = new ScaleTransform();
            //scaleTransform.ScaleX = .50;
            //scaleTransform.ScaleY = .50;
            ////  transformGroup.Children.Add(scaleTransform);

            //DGBorder.RenderTransform = scaleTransform;


                             

            scrollViewer.AddHandler(ScrollViewer.MouseLeftButtonDownEvent, new MouseButtonEventHandler(LayoutRoot_MouseLeftButtonDown), true);

            scrollViewer.SetIsMouseWheelScrollingEnabled(true);



            applicationViewModel.LoggedInUser.UserChanged += new EventHandler(LoggedInUser_UserChanged);
            applicationViewModel.UnloadedEvent += new UnloadedEventHandler(applicationViewModel_UnloadedEvent);
            //#if DEBUG

            //#endif

            applicationViewModel.LoggedInUser.UserChanged += new EventHandler(LoggedInUser_UserChanged);

            applicationViewModel.ScrollUpEvent += new EventHandler(applicationViewModel_ScrollUpEvent);


            applicationViewModel.CanvasHeightWidthLoaded += new EventHandler(applicationViewModel_CanvasHeightLoaded);
            applicationViewModel.RulesAddedEvent += new RulesAddedEventHandler(applicationViewModel_RulesAddedEvent);
            applicationViewModel.ApplyDataFilterEvent += new ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);

             //     MouseWheel += new MouseWheelEventHandler(DashboardMainPage_MouseWheel);
            zoom_slider.Visibility = System.Windows.Visibility.Visible;
        }

        //void DashboardMainPage_MouseWheel(object sender, MouseWheelEventArgs e)
        //{


            
        //    scrollViewer.SetIsMouseWheelScrollingEnabled(true);    

              


        //}

        void applicationViewModel_CanvasHeightLoaded(object sender, EventArgs e)
        {



            dgCanvas.Width = applicationViewModel.LoadedCanvasDto.Width;        
            dgCanvas.Height = applicationViewModel.LoadedCanvasDto.Height;

            scrollViewer.ScrollToTop();
            scrollViewer.ScrollToLeft();        




        }


        void applicationViewModel_UnloadedEvent(object o)
        {
            dgCanvas.Children.Clear();
        }

        void LoggedInUser_UserChanged(object sender, EventArgs e)
        {
#if DEBUG
            //    userStats.Visibility = System.Windows.Visibility.Visible;
            List<UserDTO> u = new List<UserDTO>();
            u.Add(applicationViewModel.LoggedInUser.UserDto);

            //   userStats.ItemsSource = u;

#endif

            WelcomeStackPanel.Visibility = System.Windows.Visibility.Visible;
            brDashboardTitle.Visibility = System.Windows.Visibility.Visible;
            tbUser.Text = "Welcome " + applicationViewModel.LoggedInUser.UserDto.FirstName;
            tbUserAdmin.Text = "Welcome " + applicationViewModel.LoggedInUser.UserDto.FirstName;
            //    Org.LoadOrganizations();

            if (applicationViewModel.AuthenticationMode.ToString().ToLower() == "windows")
            {
                linkLogout.Visibility = System.Windows.Visibility.Collapsed;
                linkLogout1.Visibility = System.Windows.Visibility.Collapsed;
                linkLogout_sep.Visibility = System.Windows.Visibility.Collapsed;
                linkLogout1_sep.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (applicationViewModel.LoggedInUser.HighestRolesEnum == Membership.RolesEnum.Administrator ||
                applicationViewModel.LoggedInUser.HighestRolesEnum == Membership.RolesEnum.SuperAdministrator)
            {
                linkAdmin.Visibility = System.Windows.Visibility.Visible;
                linkAdmin_sep.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {


                linkAdmin.Visibility = System.Windows.Visibility.Collapsed;
                linkAdmin_sep.Visibility = System.Windows.Visibility.Collapsed;

            }


        }

        public void Construct()
        {
            applicationViewModel = ApplicationViewModel.Instance;

                if ( applicationViewModel.DemoMode  )
                 {
                     tbWelcomeAdmin.Text = "Welcome to Epi Info Visualization Administration! (Demo Mode)";
                 }
                 else
                 {
                     tbWelcomeAdmin.Text = "Welcome to Epi Info Visualization Administration! ";
                 }


            applicationViewModel.DatasourceChangedEvent += new Client.Application.DatasourceChangedEventHandler(applicationViewModel_DatasourceChangedEvent);
            applicationViewModel.FiltersDeserializedEvent += new FiltersDeserializedEventHandler(applicationViewModel_FiltersDeserializedEvent);
            ctxMenuCreator.ControlName = "GadgetSelector";
            applicationViewModel.MainCanvas = this.dgCanvas;
            applicationViewModel.LayoutRoot = this.LayoutRoot;
            applicationViewModel.GetControlNames();

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) == true)
            {
                //    MeansControl means = new MeansControl();
                //    means.Name = "MeansControl";
                //    dgCanvas.AddChild(means, this.LayoutRoot);

                //    FrequencyControl frequency = new FrequencyControl();
                //    frequency.Name = "FrequencyControl";
                //    dgCanvas.AddChild(frequency, this.LayoutRoot);

                //    //  TwoxTwoTableControl.xaml

                //    LogisticRegression logReg = new LogisticRegression();
                //    logReg.Name = "LogisticRegression";
                //    dgCanvas.AddChild(logReg, this.LayoutRoot);
            }
            else
            {
                // MEF Integration -  
                // Requires that gadgets implement the IEwavGadget interface    
                //ObservableCollection<string> gadgetControlNames = new ObservableCollection<string>();
                //GadgetManager gm = new GadgetManager();    
                //foreach (var gadget in Gadgets)
                //{
                //    IEwavGadget uc = gadget as IEwavGadget;
                //    //  gadgetControlNames.Add(uc.MyControlName);
                //    applicationViewModel.MefAvailableGadgets.Add(uc.MyControlName);
                //    //  All gadgets will not he loaded at startup     
                //    //gadget.Name = uc.MyControlName;        
                //    //dgCanvas.AddChild(gadget, this.LayoutRoot);
                //    //applicationViewModel.LoadedGadgets.Add(uc.MyControlName);
                //    //gm.LoadGadget(uc.MyControlName);    
                //}
                // simulate a context menu click to add a gadget    
                // string gadgetName = "aa";
                //  addGadget(gadgetName);
            }
        }

        void applicationViewModel_DatasourceChangedEvent(object o, Client.Application.DatasourceChangedEventArgs e)
        {
            WelcomeStackPanel.Visibility = System.Windows.Visibility.Visible;

            tbWelcome.Text = "You are now connected to Data Source " + applicationViewModel.EwavSelectedDatasource.DatasourceNoCamelName + ".";
            tbMsg.Text = "Right click on the canvas to start adding statistical gadgets and charts!";

            btnGadgetMenu.Visibility = System.Windows.Visibility.Visible;

            // BorderBrush="{StaticResource ConnectedBrush}" Background="{StaticResource Connected}"

            border.Background = Application.Current.Resources["Connected"] as LinearGradientBrush;
            border.BorderBrush = Application.Current.Resources["ConnectedBrush"] as SolidColorBrush;
            //applicationViewModel.MainCanvas = this.dgCanvas;
            //applicationViewModel.LayoutRoot = this.LayoutRoot;
            applicationViewModel.CleanupDashboard();
            RemovePopup();
            grdDF.Children.Clear();
            AddDataFilter();
            AddVariables();
            spButtons.Visibility = System.Windows.Visibility.Visible;
        }

        public void AddDataFilter()
        {
            dtCtrl = new DataFilterControl();
            AddDFToGrid();
        }

        private void AddDFToGrid()
        {
            dtCtrl.Name = "dfCtrl";
            dtCtrl.Margin = new Thickness(-644, 0, 0, 0);
            dtCtrl.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            //grdDF.Children.Clear();
            grdDF.Children.Add(dtCtrl);
            Canvas.SetZIndex(dtCtrl, 100);

        }

        public void AddVariables()
        {
            vCtrl = new VariablesControl();
            vCtrl.Name = "vCtrl";
            vCtrl.Margin = new Thickness(-624, 0, 0, 300);
            vCtrl.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            //grdDF.Children.Clear();
            grdDF.Children.Add(vCtrl);
            Canvas.SetZIndex(vCtrl, 200);
        }

        public void RemovePopup()
        {
            if (ctxMenuCreator.PopupMenu != null)
            {
                //popupMenu = ctxMenuCreator.PopupMenu;
                //popupMenu.Hide();
                ctxMenuCreator.PopupDisplayed = false;
                ctxMenuCreator.HidePopuMenu();
            }

        }

        private void LayoutRoot_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (applicationViewModel.EwavDatasourceSelectedIndex < 0) //replaced 1 with 0
            {
                ctxMenuCreator.DisplayWithOutDSSelected = true;
            }
            else
            {
                ctxMenuCreator.DisplayWithOutDSSelected = false;
            }

            Point p = e.GetSafePosition(dgCanvas);

            RemovePopup();

            string tempGadgetName = dgCanvas.GadgetNameOnRightClick;

            Point contextMeneRightClickCoord = e.GetSafePosition(dgCanvas);

            if (contextMeneRightClickCoord.Y < 0) return;

            if (string.IsNullOrEmpty(tempGadgetName))
            {
                popupMenu = ctxMenuCreator.CreateContextMenu("", contextMeneRightClickCoord, dgCanvas.StrataList,
                    dgCanvas.SelectedGadget, dgCanvas, LayoutRoot,
                    dgCanvas.Gridcells, dgCanvas.Gadgetparameters);
            }
            else
            {
                popupMenu = ctxMenuCreator.CreateContextMenu(tempGadgetName, contextMeneRightClickCoord, dgCanvas.StrataList, dgCanvas.SelectedGadget, dgCanvas, LayoutRoot, dgCanvas.Gridcells, dgCanvas.Gadgetparameters);
            }

            dgCanvas.AddChild(popupMenu, LayoutRoot);
            if (!ctxMenuCreator.PopupDisplayed)
            {
                ctxMenuCreator.PopupDisplayed = true;
            }

            DragCanvas.SetLeft(popupMenu, p.X);
            DragCanvas.SetTop(popupMenu, p.Y);

            popupMenu.Show();

            //ScaleTransform st = new ScaleTransform();
            //st.ScaleX = 2;
            //st.ScaleY = 2;
            //popupMenu.RenderTransform = st;

            if (dgCanvas.SelectedGadget != null)
            {
                UpdateZOrder((UIElement)dgCanvas.SelectedGadget, true);
            }

            UpdateZOrder(popupMenu, true);
            e.Handled = true;
            dgCanvas.GadgetNameOnRightClick = ""; // resetting the value to bring canvas context menu.            

            if (DragCanvasRightMouseDownEvent != null)
                DragCanvasRightMouseDownEvent(this);

            if (spButtons.Visibility == System.Windows.Visibility.Visible && storyboardFlag == false)
            {
                //spButtons.Visibility = System.Windows.Visibility.Collapsed;
                Shrink.Begin();
                storyboardFlag = true;
            }
        }

        private void UpdateZOrder(UIElement element, bool bringToFront)
        {
            List<UserControl> feList = GetChildObjects<UserControl>(this.LayoutRoot); // Makes list of FrameWorkElement class. Which is inherited by Control class.

            List<Ewav.ContextMenu.Menu> feList2 = GetChildObjects<Ewav.ContextMenu.Menu>(this.LayoutRoot);

            List<FrameworkElement> feList3 = new List<FrameworkElement>();
            feList3.AddRange(feList2);
            feList3.AddRange(feList);

            #region Safety Check

            if (element == null)
                throw new ArgumentNullException("element");

            #endregion

            #region Calculate Z-Indici And Offset

            int elementNewZIndex = -1;
            if (bringToFront)
            {
                foreach (UIElement elem in feList3)
                    if (elem.Visibility != Visibility.Collapsed)
                        ++elementNewZIndex;
            }
            else
            {
                elementNewZIndex = 0;
            }

            int offset = (elementNewZIndex == 0) ? +1 : -1;

            int elementCurrentZIndex = Canvas.GetZIndex(element);

            #endregion

            #region Update Z-Indici
            int maxZindex = 0;

            foreach (UIElement childElement in feList)
            {
                if (Canvas.GetZIndex(childElement) > maxZindex)
                {
                    maxZindex = Canvas.GetZIndex(childElement);
                }
            }

            foreach (UIElement childElement in feList3)
            {
                if (childElement == element)
                    Canvas.SetZIndex(element, maxZindex + 10);
                else
                {
                    int zIndex = Canvas.GetZIndex(childElement);

                    if (bringToFront && elementCurrentZIndex < zIndex ||
                        !bringToFront && zIndex < elementCurrentZIndex)
                    {
                        Canvas.SetZIndex(childElement, zIndex + offset);
                        //Canvas.SetZIndex(childElement, elementNewZIndex - 1);
                    }
                }
            }
            #endregion
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

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //ChildWindow1 childWindow = new ChildWindow1();    smarttable
            //childWindow.Show();
            if (ctxMenuCreator.PopupDisplayed)
            {
                popupMenu.Hide();

                ctxMenuCreator.PopupDisplayed = false;
            }
            if (spButtons.Visibility == System.Windows.Visibility.Visible
                && storyboardFlag == false)
            {
                //spButtons.Visibility = System.Windows.Visibility.Collapsed;
                Shrink.Begin();
                storyboardFlag = true;

            }
            //FrameworkElement element = e.OriginalSource as FrameworkElement;    
            //scrollViewer.ScrollIntoView(element);
            //if (DGBorder.RenderTransform is ScaleTransform)
            //{
            //    if (((ScaleTransform)DGBorder.RenderTransform).ScaleX == ScaleTransforms.Half)
            //    {
            //        ScaleTransform st = new ScaleTransform();
            //        st.ScaleX = ScaleTransforms.Full;
            //        st.ScaleY = ScaleTransforms.Full;
            //        //st.CenterX = ((Point)e.GetPosition(null)).X;
            //        //st.CenterY = ((Point)e.GetPosition(null)).Y;
            //        DGBorder.RenderTransform = null;
            //        DGBorder.RenderTransform = st;
            //        FrameworkElement element = e.OriginalSource as FrameworkElement;
            //        scrollViewer.ScrollIntoView(element);
            //    }
            //}
        }

        private void testMenu_ItemSelected(object sender, MenuEventArgs e)
        {
            switch (e.Tag.ToString())
            {
                case "item11":
                    // TODO: New functionality
                    break;
                case "item12":
                    // TODO: Save functionality
                    break;
            }
            //    testMenu.CloseChildren();
        }

        private void dgCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this);

            DragCanvas.SetLeft(popupMenu, p.X);
            DragCanvas.SetTop(popupMenu, p.Y);
            popupMenu.Show();

            e.Handled = true;
        }

        ScaleTransform scaleTransform = new ScaleTransform();

        private void ZoomToggle_Click(object sender, RoutedEventArgs e)
        {
            //    if (scaleTransform.ScaleX == .50)
            //    {
            //        scaleTransform.ScaleX = 1;
            //        scaleTransform.ScaleY = 1;
            //    }
            //    else
            //    {
            //        scaleTransform.ScaleX = .50;
            //        scaleTransform.ScaleY = .50;
            //    }
            //    //  transformGroup.Children.Add(scaleTransform);
            //    DGBorder.RenderTransform = scaleTransform;
        }

        private void zoom_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            scaleTransform.ScaleX = 1 + (e.NewValue / 10);
            scaleTransform.ScaleY = 1 + (e.NewValue / 10);

            DGBorder.RenderTransform = scaleTransform;
        }

        private void zoom_slider_MouseEnter(object sender, MouseEventArgs e)
        {
            zoom_slider.MouseWheel += new MouseWheelEventHandler(zoom_slider_MouseWheel);
        }

        void zoom_slider_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var slider = e.Delta / 100;

            //  Console.WriteLine(slider.ToString());
            System.Diagnostics.Debug.WriteLine(e.Delta / 100);

            if (slider > zoom_slider.Maximum)
            {
                zoom_slider.Value = zoom_slider.Maximum;
            }
            else
            {
                zoom_slider.Value = e.Delta / 100;
            }
        }

        private void zoom_slider_MouseLeave(object sender, MouseEventArgs e)
        {
            zoom_slider.MouseWheel -= zoom_slider_MouseWheel;
        }

        private void btnZoomReset_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            zoom_slider.Value = 0;
        }

        private void btnZoomOut_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            zoom_slider.Value = zoom_slider.Value - 1;
        }

        private void btnZoomIn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            zoom_slider.Value = zoom_slider.Value + 1;
        }



        private void admin_click(object sender, System.Windows.RoutedEventArgs e)
        {
            Storyboard1.Begin();
            //grdDash.Visibility = System.Windows.Visibility.Collapsed;
            // grdAdmin.Visibility = System.Windows.Visibility.Visible;

        }

        private void dash_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Storyboard2.Begin();
            //grdDash.Visibility = System.Windows.Visibility.Visible;
            // grdAdmin.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void spButtons_MouseEnter(object sender, System.Windows.RoutedEventArgs e)
        {
            if (spButtons.Visibility == System.Windows.Visibility.Collapsed)
            {
                spButtons.Visibility = System.Windows.Visibility.Visible;
                Grow.Begin();
                storyboardFlag = false;
            }
            else
            {
                Shrink.Begin();
                storyboardFlag = true;
            }

        }


        private void btnDS_Click(object sender, RoutedEventArgs e)
        {

            setDS = new SetDatasource();
            setDS.Show();
            setDS.Closed += new EventHandler(setDb_Closed);
        }

        void setDb_Closed(object sender, EventArgs e)
        {

            if (setDS.DialogResult == true && appMenuView.DisplayControlButtons())
            {
                //    applicationViewModel.CleanupDashboard();

                applicationViewModel.ReadAllUsersInMyOrg();

                appMenuView.SetDatasourceDisplayText();

                appMenuViewModel.GetRecordCount(applicationViewModel.EwavDatafilters,
                    this.applicationViewModel.EwavDefinedVariables,
                    this.applicationViewModel.EwavSelectedDatasource.TableName,
                    this.applicationViewModel.EwavSelectedDatasource.DatasourceName);
                spButtons.Visibility = System.Windows.Visibility.Visible;
            }





        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenDash od = new OpenDash();
            od.Show();
            od.Closed += new EventHandler(od_Closed);
            //int i = 0;
            ////UnloadedEvent();
            //applicationViewModel.LoadCanvas(i);
        }

        void od_Closed(object sender, EventArgs e)
        {
            OpenDash od = sender as OpenDash;

            try
            {
                if (od.DialogResult == true)
                {
                    applicationViewModel.GadgetsOnCanvas = true;

                    applicationViewModel.ReadAllUsersInMyOrg();
                    //Share.Visibility = System.Windows.Visibility.Visible;
                    //Delete.Visibility = System.Windows.Visibility.Visible;
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                od.Closed -= new EventHandler(od_Closed);
                od = null;

                sender = null;
            }
        }
        void appMenuViewModel_RecordcountRecievedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            appMenuView.SetRecordCountText(this.appMenuViewModel);
        }

        private void GadgetSelectedEventHandler(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            ClientCommon.Common cmnClass = new ClientCommon.Common();

            //const string gNamespace = "Ewav";

            GadgetManager gm = new GadgetManager();

            string btnName;//= btn.Name.ToString();

            if (btn.Tag != null)
            {
                btnName = btn.Tag.ToString();
            }
            else
            {
                btnName = btn.Name.ToString();
            }


            UserControl uc = gm.LoadGadget(btnName);

            uc.Name = cmnClass.GenerateControlName(uc, dgCanvas);


            if (uc is XYChartControl && btn.Tag != null)
            {
                // ((XYChartControl)uc).ChartName = ((Ewav.ContextMenu.MenuItem)(sender)).Text.Replace(" ","");
                ((XYChartControl)uc).SetChartTitle(btn.Name.ToString().Replace(" ", ""));
            }

            cmnClass.AddControlToCanvas(uc, 10, 10, applicationViewModel.LayoutRoot);

            spButtons.Visibility = System.Windows.Visibility.Collapsed;
        }


        public double DragCanvasHeight
        {
            get
            {
                return dgCanvas.Height;
            }
        }


        public static event CanvasSizeChangedHandler CanvasSizeChangedEvent;


        private void btnCanvasHeight_Click(object sender, RoutedEventArgs e)
        {


            dgCanvas.Height = dgCanvas.Height + 5000;
            CanvasSizeChangedEvent(new CanvasChangedEventArgs(dgCanvas.Height, dgCanvas.Width));




        }

        private void btnCanvasWidth_Click(object sender, RoutedEventArgs e)
        {


            dgCanvas.Width = dgCanvas.Width + 2000;
            CanvasSizeChangedEvent(new CanvasChangedEventArgs(dgCanvas.Height, dgCanvas.Width));



        }



    }
}