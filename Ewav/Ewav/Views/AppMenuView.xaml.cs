/*  ----------------------------------------------------------------------------
*  Emergint Technologies, Inc.
*  ----------------------------------------------------------------------------
*  Epi Info™ - Web Analysis & Visualization
*  ----------------------------------------------------------------------------
*  File:       AppMenuView.xaml.cs
*  Namespace:  Ewav    
*
*  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
*  Created:    27/05/2014    
*  Summary:	no summary     
*  ----------------------------------------------------------------------------
*/

using System;
using System.ServiceModel.DomainServices.Client.ApplicationServices;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Xml.Linq;
using Ewav.Client.Application;

using System.Linq;
using System.Collections.Generic;


using Ewav.Common;
using Ewav.DTO;
using Ewav.ViewModels;
using SimpleMvvmToolkit;
using Ewav.Membership;
using Ewav.Views.Dialogs;

namespace Ewav
{
    public partial class AppMenuView : UserControl
    {
        public ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;
        //OpenDash od = null;
        DeleteDash dd = null;
        SaveDash savedash = null;
        SetDatasource setDb = null;
        AppMenuViewModel appMenuViewModel = null;

        WebContext webCtx;

        public AppMenuView()
        {
            this.InitializeComponent();

            this.Loaded += new System.Windows.RoutedEventHandler(AppMenuView_Loaded);

            webCtx = new WebContext();
            //    Ewav.App.ApplicationLifetimeObjects.Add(webCtx);
        }

        void AppMenuView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {

                applicationViewModel.AppMenuView = this;

                DatasourceWatcher.Instance.DatasourceWatcherEvent += new EventHandler(datasourceWatcherEvent_DatasourceWatcherEvent);

                DatasourceWatcher.Instance.RefreshEvent += new EventHandler(datasourceWatcher_RefreshEvent);

                if (applicationViewModel.DemoMode)
                {
                    AppNameText.Text = "Epi Info Cloud Data Analytics (Demo Mode)";
                }
                else
                {
                    AppNameText.Text = "Epi Info Cloud Data Analytics ";
                }


                AppMenuViewModel appMenuViewModel = (AppMenuViewModel)this.DataContext;
                //appMenuViewModel.DatasourcesLoadedEvent +=
                //    new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(appMenuViewModel_DatasourcesLoadedEvent);    

                applicationViewModel.GadgetsReloaded += new EventHandler(applicationViewModel_GadgetsReloaded);

                //applicationViewModel.FilteredRecordCountUpdatedEvent -= new Client.Application.FilteredRecordcountUpdatedEventHandler(applicationViewModel_FilteredRecordCountUpdatedEvent);
                applicationViewModel.ApplyDataFilterEvent -= new Client.Application.ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);
                applicationViewModel.ReadFilterStringEvent -= new Client.Application.ReadFilterStringEventHandler(applicationViewModel_ReadFilterStringEvent);
                applicationViewModel.DefinedVariableAddedEvent -= new Client.Application.DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);
                applicationViewModel.PreColumnChangedEvent -= new Client.Application.PreColumnChangedEventHandler(applicationViewModel_PreColumnChangedEvent);
                applicationViewModel.DefinedVariableNotInUseDeletedEvent -= new Client.Application.DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
                applicationViewModel.DefinedVariableInUseDeletedEvent -= new Client.Application.DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableInUseDeletedEvent);

                appMenuViewModel.ErrorNotice -= new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(appMenuViewModel_ErrorNotice);

                //applicationViewModel.FilteredRecordCountUpdatedEvent += new Client.Application.FilteredRecordcountUpdatedEventHandler(applicationViewModel_FilteredRecordCountUpdatedEvent);
                applicationViewModel.ApplyDataFilterEvent += new Client.Application.ApplyFilterEventHandler(applicationViewModel_ApplyDataFilterEvent);
                applicationViewModel.ReadFilterStringEvent += new Client.Application.ReadFilterStringEventHandler(applicationViewModel_ReadFilterStringEvent);
                applicationViewModel.DefinedVariableAddedEvent += new Client.Application.DefinedVariableAddedEventHandler(applicationViewModel_DefinedVariableAddedEvent);
                applicationViewModel.PreColumnChangedEvent += new Client.Application.PreColumnChangedEventHandler(applicationViewModel_PreColumnChangedEvent);
                applicationViewModel.DefinedVariableNotInUseDeletedEvent += new Client.Application.DefinedVariableNotInUseDeletedEventHandler(applicationViewModel_DefinedVariableNotInUseDeletedEvent);
                applicationViewModel.DefinedVariableInUseDeletedEvent += new Client.Application.DefinedVariableInUseDeletedEventHandler(applicationViewModel_DefinedVariableInUseDeletedEvent);

                applicationViewModel.DatasourceChangedEvent += ApplicationViewModel_DatasourceChangedEvent;

                appMenuViewModel.ErrorNotice += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(appMenuViewModel_ErrorNotice);

                //cboDatasoures.SelectionChanged += new SelectionChangedEventHandler(cboDatasoures_SelectionChanged);

                SetDatasourceDisplayText();

                AuthenticateUser();
            }
            catch (System.Exception)
            {
                throw new Exception("aa");
            }
        }

        private void ApplicationViewModel_DatasourceChangedEvent(object o, DatasourceChangedEventArgs e)
        {

            if (applicationViewModel.EwavDatafilters.Count > 0)
            {
                this.tbRecordCount.Text = "Calculatihg filtered record count...";
            }

        
            if (ApplicationViewModel.Instance.EwavSelectedDatasource != null &&
                ApplicationViewModel.Instance.EwavSelectedDatasource.DatasourceName != "Set Data Source")
            {
                DisplayControlButtons();
                this.tbDatasourceName.Text = ApplicationViewModel.Instance.EwavSelectedDatasource.DatasourceName;
                if (ApplicationViewModel.Instance.EwavSelectedDatasource.FilteredRecords > 0)
                {
                    this.tbRecordCount.Text = "Records: " + ApplicationViewModel.Instance.EwavSelectedDatasource.FilteredRecords.ToString() + " of " + ApplicationViewModel.Instance.EwavSelectedDatasource.TotalRecords.ToString();
                }
                else
                {
                    this.tbRecordCount.Text = "Records: " + ApplicationViewModel.Instance.EwavSelectedDatasource.TotalRecords.ToString() + " of " + ApplicationViewModel.Instance.EwavSelectedDatasource.TotalRecords.ToString();
                }
            }
            else
            {
                this.tbDatasourceName.Text = "";
                this.tbRecordCount.Text = "";
            }
        }

        void datasourceWatcher_RefreshEvent(object sender, EventArgs e)
        {
            AppMenuViewModel datasourceWatcher_AppMenuViewModel = ((DatasourceWatcher)sender).appMenuViewModel;

            //    AppMenuViewModel datasourceWatcher_AppMenuViewModel = (AppMenuViewModel)sender;

            if (datasourceWatcher_AppMenuViewModel.RecordCountString != null &&
                datasourceWatcher_AppMenuViewModel.RecordCountString.Length > 0)
            {
                applicationViewModel.EwavSelectedDatasource.FilteredRecords =
                    Convert.ToInt64(datasourceWatcher_AppMenuViewModel.RecordCountString.Substring(
                        datasourceWatcher_AppMenuViewModel.RecordCountString.IndexOf(",") + 1,
                                                                                              datasourceWatcher_AppMenuViewModel.RecordCountString.Length - 1 -
                                                                                              datasourceWatcher_AppMenuViewModel.RecordCountString.IndexOf(",")));

                string totalRecords = datasourceWatcher_AppMenuViewModel.RecordCountString.Substring(
                    0, datasourceWatcher_AppMenuViewModel.RecordCountString.IndexOf(","));

                this.tbRecordCount.Text = string.Format("Records: {0} of {1}", applicationViewModel.EwavSelectedDatasource.FilteredRecords.ToString(), totalRecords);
            }
        }

        /// <summary>
        /// Handles the RecordcountRecievedEvent event of the datasourceWatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SimpleMvvmToolkit.NotificationEventArgs&lt;System.Exception&gt;" /> instance containing the event data.</param>
        private void datasourceWatcher_RecordcountRecievedEvent(object sender, NotificationEventArgs<Exception> e)
        {


        }

        void applicationViewModel_GadgetsReloaded(object sender, EventArgs e)
        {
            //       this.tbRecordCount.Text = "Records: " + DatasourceWatcher.Instance.CurrentApplicationRecordCount + " of " + DatasourceWatcher.Instance.CurrentDatasourceRecordCount;
        }

        void datasourceWatcherEvent_DatasourceWatcherEvent(object sender, EventArgs e)
        {
            UpdateRefreshCountStatus();
        }

        void UpdateRefreshCountStatus()
        {
            if (DatasourceWatcher.Instance.RecordCountDifference > 0)
            {
                brdrRefresh.Visibility = System.Windows.Visibility.Visible;
                Fader.Begin();
                //        this.tbRecordCount.Text = "Records: " + DatasourceWatcher.Instance.CurrentApplicationRecordCount + " of " + DatasourceWatcher.Instance.CurrentDatasourceRecordCount;
            }
            else
            {
                brdrRefresh.Visibility = System.Windows.Visibility.Collapsed;
            }

            tbRefreshCount.Text = DatasourceWatcher.Instance.RecordCountDifference.ToString();
        }

        void RefreshCountStatusOff()
        {
            brdrRefresh.Visibility = System.Windows.Visibility.Collapsed;
        }

        private LogIn logIn;

        private void OnLoadUser_Completed(LoadUserOperation operation)
        {
            if (ApplicationViewModel.Instance.AuthenticationMode.ToString().ToLower() == "windows")
            {
                Membership.MembershipManager mm = new Membership.MembershipManager();
                // Authenticate this user aganist the EWAV database.  If authenticated,  load the user
                ApplicationViewModel.Instance.CurrentUserDomain =
                    WebContext.Current.Authentication.User.Identity.Name.Split('\\')[0].ToString();
                //ApplicationViewModel.Instance.LoggedInUser.UserDto.UserName =
                //    WebContext.Current.Authentication.User.Identity.Name.Split('\\')[1].ToString();

                //mm.AuthenticateAndLoadUser(WebContext.Current.Authentication.User.Identity.Name.Split('\\')[1].ToString());
                mm.AuthenticateAndLoadUser(WebContext.Current.Authentication.User.Identity.Name);
                mm.UserAuthenticatedFromEwav += new EventHandler(mm_UserAuthenticated);
                mm.UserNotAuthenticatedFromEwav += new EventHandler(mm_UserNotAuthenticatedFromEwav);
                mm.UserLoadedWithNoDatasource += new EventHandler(mm_UserLoadedWithNoDatasource);
            }
        }

        void mm_UserAuthenticated(object sender, EventArgs e)
        {
            ReadDatasources();
        }

        void mm_UserLoadedWithNoDatasource(object sender, EventArgs e)
        {
            NoDSAssignedErrorMsg();
            //ErrorWindow Err = new ErrorWindow("You do not have access to any datasource.", "");
            //Err.Show();
        }

        private static void NoDSAssignedErrorMsg()
        {
            string warningMessage = "";
            if (ApplicationViewModel.Instance.LoggedInUser.UserDto.HighestRole == (int)RolesEnum.SuperAdministrator)
            {
                warningMessage = "No datasource has been created yet!";
            }
            else
            {
                warningMessage = "You do not have access to any datasource. Please contact the administrator for your organization to request access.";
            }
            WarningWindow error = new WarningWindow(warningMessage, "");
            error.Show();
        }

        void mm_UserNotAuthenticatedFromEwav(object sender, EventArgs e)
        {
            //string s = sender.ToString();
            HtmlPage.Window.Navigate(new Uri(string.Format("Error.aspx?AdminEmail={0}", sender.ToString()), UriKind.Relative));
        }

        private void AuthenticateUser()
        {
            if (applicationViewModel.AuthenticationMode.ToString().ToLower() != "windows")
            {
                logIn = new LogIn();
                logIn.Show();
                logIn.Closed += new EventHandler(logIn_Closed);
            }
            else
            {
                if (ApplicationViewModel.Instance.AuthenticationMode.ToString().ToLower() == "windows")
                {
                    webCtx.Authentication = new System.ServiceModel.DomainServices.Client.ApplicationServices.WindowsAuthentication();
                    WebContext.Current.Authentication.LoadUser(OnLoadUser_Completed, null);
                }
                else
                {
                    //  webCtx.Authentication = new System.ServiceModel.DomainServices.Client.ApplicationServices.FormsAuthentication();
                }
                //             ReadDatasources();
            }
        }

        void logIn_Closed(object sender, EventArgs e)
        {
            if (logIn.DialogResult == true)
            {
                if (applicationViewModel.LoggedInUser.UserDto.DatasourceCount == 0)
                {
                    NoDSAssignedErrorMsg();
                }

                ReadDatasources();
            }
            else
            {
                return;
            }
        }

        CanvasViewModel canvasViewModel = new CanvasViewModel();

        private void ReadDatasources()
        {
            //  if  quesrys then change datasource amd load cancas     
            ///////////////////////////////////////
            AppMenuViewModel appMenuViewModel = (AppMenuViewModel)this.DataContext;

            appMenuViewModel.DatasourcesLoadedEvent +=
                new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(appMenuViewModel_DatasourcesLoadedEvent);

            appMenuViewModel.RecordcountRecievedEvent -= new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(appMenuViewModel_RecordcountRecievedEvent);
            appMenuViewModel.FilterStringRecieved -= new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(appMenuViewModel_FilterStringRecieved);

            appMenuViewModel.RecordcountRecievedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(appMenuViewModel_RecordcountRecievedEvent);
            appMenuViewModel.FilterStringRecieved += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(appMenuViewModel_FilterStringRecieved);

            appMenuViewModel.GetDatasourcesAsIEnumerable2();
        }

        void appMenuViewModel_ErrorNotice(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            ShowError(e);
        }

        void ShowError(SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            if (e.Data.Message.Length > 0)
            {
                ChildWindow window = new ErrorWindow(e.Data);
                window.Show();
                //return;
            }
        }

        void applicationViewModel_DefinedVariableInUseDeletedEvent(object o)
        {
            //throw new NotImplementedException();
        }

        void applicationViewModel_DefinedVariableNotInUseDeletedEvent(object o)
        {
            //throw new NotImplementedException();
        }

        void applicationViewModel_PreColumnChangedEvent(object o)
        {
            //throw new NotImplementedException();
        }

        void applicationViewModel_DefinedVariableAddedEvent(object o)
        {
            //applicationViewModel.EwavSelectedDatasource.AllColumns.Add(new EwavColumn());
        }

        /// <summary>
        /// Filter String Recieved Event. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void appMenuViewModel_FilterStringRecieved(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            AppMenuViewModel appMenuViewModel = (AppMenuViewModel)this.DataContext;
            if (appMenuViewModel.FilterString.Length > 0)
            {
                applicationViewModel.FilterString = appMenuViewModel.FilterString;
                //applicationViewModel.AdvancedDataFilterString = appMenuViewModel.FilterString;
            }
        }

        /// <summary>
        /// Filter String Read Event
        /// </summary>
        /// <param name="o"></param>
        void applicationViewModel_ReadFilterStringEvent(object o)
        {
            AppMenuViewModel appMenuViewModel = (AppMenuViewModel)this.DataContext;
            appMenuViewModel.GetFilterString(applicationViewModel.EwavDatafilters,
                this.applicationViewModel.EwavDefinedVariables,
                this.applicationViewModel.EwavSelectedDatasource.TableName,
                this.applicationViewModel.EwavSelectedDatasource.DatasourceName);
        }

        void applicationViewModel_ApplyDataFilterEvent(object o)
        {

            AppMenuViewModel appMenuViewModel = (AppMenuViewModel)this.DataContext;

            if (applicationViewModel.EwavDatafilters.Count > 0)
            {
                this.tbRecordCount.Text = "Calculatihg filtered record count...";
            }


            if (applicationViewModel.UseAdvancedFilter)
            {
                appMenuViewModel.GetRecordCount(this.applicationViewModel.EwavDefinedVariables,
                    applicationViewModel.AdvancedDataFilterString,
                    this.applicationViewModel.EwavSelectedDatasource.TableName,
                    this.applicationViewModel.EwavSelectedDatasource.DatasourceName);
            }
            else
            {
                appMenuViewModel.GetRecordCount(applicationViewModel.EwavDatafilters,
                    this.applicationViewModel.EwavDefinedVariables,
                    this.applicationViewModel.EwavSelectedDatasource.TableName,
                    this.applicationViewModel.EwavSelectedDatasource.DatasourceName);
                // appMenuViewModel.RecordcountRecievedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(appMenuViewModel_RecordcountRecievedEvent);
            }

            RefreshCountStatusOff();
        }

        void appMenuViewModel_RecordcountRecievedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            //    AppMenuViewModel appMenuViewModel = (AppMenuViewModel)this.DataContext;
            AppMenuViewModel appMenuViewModel = (AppMenuViewModel)sender;

            SetRecordCountText(appMenuViewModel);
        }

        public void SetRecordCountText(AppMenuViewModel appMenuViewModel)
        {

            if (appMenuViewModel.RecordCountString != null && appMenuViewModel.RecordCountString.Length > 0)
            {
                applicationViewModel.EwavSelectedDatasource.FilteredRecords = Convert.ToInt64(appMenuViewModel.RecordCountString.Substring(appMenuViewModel.RecordCountString.IndexOf(",") + 1, appMenuViewModel.RecordCountString.Length - 1 - appMenuViewModel.RecordCountString.IndexOf(",")));

                string totalRecords = appMenuViewModel.RecordCountString.Substring(0, appMenuViewModel.RecordCountString.IndexOf(","));

                this.tbRecordCount.Text = string.Format("Records: {0} of {1}", applicationViewModel.EwavSelectedDatasource.FilteredRecords.ToString(), totalRecords);
                SetDatasourceDisplayText();
            }
        }

        private void appMenuViewModel_DatasourcesLoadedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            appMenuViewModel = (AppMenuViewModel)sender;

            applicationViewModel.EwavDatasources = appMenuViewModel.Datasources2;


            if (applicationViewModel.CanvasGUIDFromUrl.ToString() != "00000000-0000-0000-0000-000000000000")
            {
                canvasViewModel.GetCanvasSharedStatusWithGuid(applicationViewModel.CanvasGUIDFromUrl.ToString());

                canvasViewModel.CanvasShareStatusDtoLoaded +=
                    new EventHandler<NotificationEventArgs<Exception>>(canvasViewModel_CanvasShareStatusDtoLoaded);
            }

        }

        void canvasViewModel_CanvasShareStatusDtoLoaded(object sender, NotificationEventArgs<Exception> e)
        {

            CanvasViewModel cvm = (CanvasViewModel)sender;


            var cssDto = from user in cvm.CanvasShareStatusDtoList
                         where user.UserID == applicationViewModel.LoggedInUser.UserDto.UserID
                          && user.Shared == true
                         select user;


            List<CanvasShareStatusDto> cssDtoList = new List<CanvasShareStatusDto>();

            cssDtoList = cssDto.ToList();



            if (applicationViewModel.UrlHasCanvasGUID && cssDtoList.Count > 0)
            {
                CanvasDto canvasDto;



                canvasViewModel.LoadCanvas(applicationViewModel.CanvasGUIDFromUrl);

                System.EventHandler<SimpleMvvmToolkit.NotificationEventArgs<System.Exception>> CanvasLoadedE = null;
                DatasourceChangedEventHandler datasourceChangedEvent = null;

                CanvasLoadedE = (s, e4) =>
                {
                    canvasDto = ((CanvasViewModel)s).LoadedCanvas;

                    canvasViewModel.CanvasLoaded -= CanvasLoadedE;

                    for (int i = 0; i < applicationViewModel.EwavDatasources.Count; i++)
                    {
                        int datasourceId = canvasDto.DatasourceID;

                        if (datasourceId == applicationViewModel.EwavDatasources[i].DatasourceID)
                        {
                            applicationViewModel.EwavDatasourceSelectedIndex = i;

                            datasourceChangedEvent = (o, args) =>
                            {

                                applicationViewModel.DatasourceChangedEvent -= datasourceChangedEvent;

                                //  load the dash    
                                applicationViewModel.LoadCanvas(canvasDto.CanvasId);
                                applicationViewModel.ReadAllUsersInMyOrg();



                            };

                            //                   new Client.Application.DatasourceChangedEventHandler(applicationViewModel_DatasourceChangedEvent);

                            applicationViewModel.DatasourceChangedEvent += datasourceChangedEvent;

                            break;
                        }
                    }
                };

                canvasViewModel.CanvasLoaded += CanvasLoadedE;


            }


        }

        private void cboDatasoures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AppMenuViewModel appMenuViewModel = (AppMenuViewModel)this.DataContext;

            applicationViewModel.ReadAllUsersInMyOrg();
        }

        private void testMenu_ItemSelected(object sender, ContextMenu.MenuEventArgs e)
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
            //   mainMenu.CloseChildren();
        }

        public static EventHandler DashhoardReset;

        private void Reset_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ResetDash rd = new ResetDash();
            rd.Show();
        }

        private void Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            if (ApplicationViewModel.Instance.DemoMode)
            {
                DemoMode dm = new DemoMode();
                dm.Show();
                return;
            }


            //SaveFile(element);
            //applicationViewModel.LoadCanvasUserList(user);
            //UserDTO user = new UserDTO();
            //user.UserID = applicationViewModel.LoggedInUser.UserDto.UserID;
            //User ewavUser = applicationViewModel.LoggedInUser;
            //ewavUser.UserDto = user;         
            //applicationViewModel.LoggedInUser = ewavUser;                //    new UserDTO() { UserID = "1" };
            //applicationViewModel.DatasourceId = 1;
            //if (applicationViewModel.UserIdForOpenedCanvas == Convert.ToInt32(applicationViewModel.LoggedInUser.UserDto.UserID))
            //{
            //    applicationViewModel.CurrentCanvasId = applicationViewModel.CanvasIdForOpenedCanvas;
            //}
            if (applicationViewModel.CurrentCanvasId > 0)
            {
                XElement element = applicationViewModel.SerializeCanvas();

                CanvasDto dto = new CanvasDto();

                //dto.CanvasName = txtSaveTitle.Text;
                //dto.CanvasDescription = txtSaveDesc.Text;
                //dto.CreatedDate = DateTime.Now;
                dto.ModifiedDate = DateTime.Now;
                dto.CreatedDate = DateTime.Now;
                //dto.DatasourceID = applicationViewModel.LoggedInUser.UserDto.DatasourceID;
                dto.XmlData = element;
                //dto.UserId = Convert.ToInt32(applicationViewModel.LoggedInUser.UserDto.UserID);
                //dto.UserId.UserId1 = "1";
                dto.IsNewCanvas = false;
                dto.CanvasId = applicationViewModel.CurrentCanvasId;
                applicationViewModel.SaveCanvas(element, dto);
                savedash = new SaveDash("success");
                savedash.Closed += new EventHandler(savedash_Closed);
                savedash.Show();
            }
            else
            {
                SaveAsCall();
            }
        }

        private void SaveAsCall()
        {
            savedash = new SaveDash();
            savedash.Closed += new EventHandler(savedash_Closed);
            savedash.Show();
        }

        void savedash_Closed(object sender, EventArgs e)
        {
            if (savedash.DialogResult == true)
            {
                applicationViewModel.GadgetsOnCanvas = true;
                Delete.Visibility = System.Windows.Visibility.Visible;
                Share.Visibility = System.Windows.Visibility.Visible;
            }
        }

        /// <summary>
        /// Saves the file on local drive. Leaving the code if situation arises in future.
        /// </summary>
        /// <param name="element"></param>
        //private void SaveFile(XElement element)
        //{
        //    SaveFileDialog saveFileDialog = new SaveFileDialog();
        //    saveFileDialog.DefaultExt = ".cvs7";
        //    saveFileDialog.Filter = "Epi Info 7 Dashboard Canvas File|*.cvs7";

        //    saveFileDialog.FilterIndex = 1;

        //    if (saveFileDialog.ShowDialog() == true)
        //    {
        //        using (Stream stream = saveFileDialog.OpenFile())
        //        {
        //            StreamWriter sw = new StreamWriter(stream, System.Text.Encoding.UTF8);
        //            sw.Write(element.ToString());
        //            sw.Close();

        //            stream.Close();
        //        }
        //    }
        //}

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
                    Share.Visibility = System.Windows.Visibility.Visible;
                    Delete.Visibility = System.Windows.Visibility.Visible;
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

        private void Share_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            //if (ApplicationViewModel.Instance.DemoMode)
            //{
            //    DemoMode dm = new DemoMode();
            //    dm.Show();
            //    return;
            //}


            if (applicationViewModel.CurrentCanvasId != -1) // if its equal to -1 means it belongs to different user or hasn't been saved.
            {


                ShareDash sharedash = new ShareDash();
                sharedash.Show();
            }
            else
            {
                WarningWindow error = new WarningWindow("Please save the dashboard before sharing.", "");
                error.Show();
                //MessageBox.Show("This canvas is either shared by someone or not saved. Please save the canvas before performing this operation.");
            }
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationViewModel.Instance.DemoMode)
            {
                DemoMode dm = new DemoMode();
                dm.Show();
                return;
            }


            SaveAsCall();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationViewModel.Instance.DemoMode)
            {
                DemoMode dm = new DemoMode();
                dm.Show();
                return;
            }


            if (applicationViewModel.CurrentCanvasId != -1) // if its equal to -1 means it belongs to different user or hasn't been saved.
            {
                dd = new DeleteDash();
                dd.Closed += new EventHandler(dd_Closed);
                dd.Show();
            }
            else
            {
                WarningWindow error = new WarningWindow("This is a shared dashboard and cannot be deleted, or dashboard does not exist. ", "");
                error.Show();
                //MessageBox.Show("This canvas is either shared by someone or not saved. Please save the canvas before performing this operation.");
            }
        }

        void dd_Closed(object sender, EventArgs e)
        {
            if (dd.DialogResult == true)
            {
                applicationViewModel.GadgetsOnCanvas = false;
                Delete.Visibility = System.Windows.Visibility.Collapsed;
                Share.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            ExportDash ed = new ExportDash();
            ed.Show();
        }

        RefreshDash refd = new RefreshDash();

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            // RefreshDash refd = new RefreshDash();
            refd.tabReferesh.IsSelected = true;
            refd.Show();
            //   brdrRefresh.Visibility = System.Windows.Visibility.Visible;
        }

        private void SetDataSource_Click(object sender, RoutedEventArgs e)
        {
            setDb = new SetDatasource();
            setDb.Show();
            setDb.Closed += new EventHandler(setDb_Closed);
        }

        void setDb_Closed(object sender, EventArgs e)
        {
            if (setDb.DialogResult == true && DisplayControlButtons())
            {


                applicationViewModel.GadgetsOnCanvas = false;

                applicationViewModel.ReadAllUsersInMyOrg();

                SetDatasourceDisplayText();

                appMenuViewModel.GetRecordCount(applicationViewModel.EwavDatafilters,
                    this.applicationViewModel.EwavDefinedVariables,
                    this.applicationViewModel.EwavSelectedDatasource.TableName,
                    this.applicationViewModel.EwavSelectedDatasource.DatasourceName);
            }
        }

        public bool DisplayControlButtons()
        {
            //  if (ApplicationViewModel.Instance.EwavSelectedDatasource == null)
            if (ApplicationViewModel.Instance.GadgetsOnCanvas == false)
            {
                Save.Visibility = System.Windows.Visibility.Collapsed;
                SaveAs.Visibility = System.Windows.Visibility.Collapsed;
                Share.Visibility = System.Windows.Visibility.Collapsed;
                Delete.Visibility = System.Windows.Visibility.Collapsed;
                Reset.Visibility = System.Windows.Visibility.Collapsed;
                Export.Visibility = System.Windows.Visibility.Collapsed;
                spRefreshbtn.Visibility = System.Windows.Visibility.Collapsed;
                return false;
            }
            else
            {
                Save.Visibility = System.Windows.Visibility.Visible;
                SaveAs.Visibility = System.Windows.Visibility.Visible;
                Export.Visibility = System.Windows.Visibility.Collapsed;


                if (applicationViewModel.CurrentCanvasId > 0)
                {
                    Share.Visibility = System.Windows.Visibility.Visible;
                    Delete.Visibility = System.Windows.Visibility.Visible;
                }



                Reset.Visibility = System.Windows.Visibility.Visible;
                // Refresh.Visibility = System.Windows.Visibility.Collapsed;
                spRefreshbtn.Visibility = System.Windows.Visibility.Visible;
            }
            return true;
        }

        public void SetDatasourceDisplayText()
        {
            AppMenuViewModel appMenuViewModel = (AppMenuViewModel)this.DataContext;

            if (applicationViewModel.EwavDatafilters.Count > 0)
            {
                this.tbRecordCount.Text = "Calculatihg filtered record count...";
            }

            if (ApplicationViewModel.Instance.EwavSelectedDatasource != null &&
                ApplicationViewModel.Instance.EwavSelectedDatasource.DatasourceName != "Set Data Source")
            {
                DisplayControlButtons();
                this.tbDatasourceName.Text = ApplicationViewModel.Instance.EwavSelectedDatasource.DatasourceName;
                if (ApplicationViewModel.Instance.EwavSelectedDatasource.FilteredRecords    >  0  )    
                {
                    this.tbRecordCount.Text = "Records: " + ApplicationViewModel.Instance.EwavSelectedDatasource.FilteredRecords.ToString() + " of " + ApplicationViewModel.Instance.EwavSelectedDatasource.TotalRecords.ToString();
                }
                else
                {
                    this.tbRecordCount.Text = "Records: " + ApplicationViewModel.Instance.EwavSelectedDatasource.TotalRecords.ToString() + " of " + ApplicationViewModel.Instance.EwavSelectedDatasource.TotalRecords.ToString();
                }
            }
            else
            {
                this.tbDatasourceName.Text = "";
                this.tbRecordCount.Text = "";
            }
        }

        private void HTML_Click(object sender, RoutedEventArgs e)
        {
            applicationViewModel.DashToHtml();
        }
    }

    /// <summary>
    /// {6CBDD833-7AC5-4ED2-BC4D-FC83C448DFEC}
    /// </summary>
    public class Logon : ChildWindow
    {
        public string userName;
        public string password;
    }
}