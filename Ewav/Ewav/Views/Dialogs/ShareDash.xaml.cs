/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ShareDash.xaml.cs
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
using System.Windows;
using System.Windows.Controls;
using Ewav.DTO;
using Ewav.ViewModels;
using Ewav.Web.Services;
using Ewav.ExtensionMethods;
using Ewav.Client.Application;
using Ewav.Views.Dialogs;

namespace Ewav
{
    public partial class ShareDash : ChildWindow
    {
        ApplicationViewModel applicationViewModel;
        //List<string> userFirstNames, userLastNames;
        List<UserListInfo> userList = null;
        List<UserListInfo> SharedCanvasUserList = null;
        public ShareDash()
        {
            InitializeComponent();
            applicationViewModel = ApplicationViewModel.Instance;
            applicationViewModel.ShareCanvasCompletedEvent -= new ShareCanvasCompletedEventHandler(applicationViewModel_ShareCanvasCompletedEvent);
            applicationViewModel.ShareCanvasCompletedEvent += new ShareCanvasCompletedEventHandler(applicationViewModel_ShareCanvasCompletedEvent);

            applicationViewModel.ShareCanvasLoadedEvent -= new ShareCanvasLoadedEventHandler(applicationViewModel_ShareCanvasLoadedEvent);
            applicationViewModel.ShareCanvasLoadedEvent += new ShareCanvasLoadedEventHandler(applicationViewModel_ShareCanvasLoadedEvent);

            applicationViewModel.EmailSentCompletedEvent -= new EmailSentCompletedEventHandler(applicationViewModel_EmailSentCompletedEvent);
            applicationViewModel.EmailSentCompletedEvent += new EmailSentCompletedEventHandler(applicationViewModel_EmailSentCompletedEvent);

            applicationViewModel.GetShareCanvasStatus();
            this.Unloaded += new RoutedEventHandler(ShareDash_Unloaded);
            waitCursor.Visibility = System.Windows.Visibility.Visible;
            //userFirstNames = new List<string>();
            //userLastNames = new List<string>();
        }

        void applicationViewModel_EmailSentCompletedEvent(object o)
        {
            waitCursor.Visibility = System.Windows.Visibility.Collapsed;
            tabShare.Visibility = System.Windows.Visibility.Collapsed;
            spMsg.Visibility = System.Windows.Visibility.Collapsed;
            spShare.Visibility = System.Windows.Visibility.Collapsed;
            spMsg_EmailSent.Visibility = System.Windows.Visibility.Visible;
        }

        void ShareDash_Unloaded(object sender, RoutedEventArgs e)
        {
            applicationViewModel.ShareCanvasCompletedEvent -= new ShareCanvasCompletedEventHandler(applicationViewModel_ShareCanvasCompletedEvent);
            applicationViewModel.ShareCanvasLoadedEvent -= new ShareCanvasLoadedEventHandler(applicationViewModel_ShareCanvasLoadedEvent);
        }

        void applicationViewModel_ShareCanvasLoadedEvent(object o)
        {
            ReadAllUsersForMyOrg();
            tbTitle.Text = applicationViewModel.SelectedCanvasName;
            tbDataSource.Text = applicationViewModel.SelectedCanvasName;
            txtEWAVUrl.Text = applicationViewModel.CurrentCanvas.EwavPermalink;
            txtEWAVLITEUrl.Text = applicationViewModel.CurrentCanvas.EwavLITEPermalink;

            //applicationViewModel.ShareCanvasLoadedEvent -= new ShareCanvasLoadedEventHandler(applicationViewModel_ShareCanvasLoadedEvent);
            waitCursor.Visibility = System.Windows.Visibility.Collapsed;

            ReadCanvasSharedUsers();
        }

        void applicationViewModel_ShareCanvasCompletedEvent(object o)
        {
            tabShare.Visibility = System.Windows.Visibility.Collapsed;
            spMsg.Visibility = System.Windows.Visibility.Collapsed;
            spShare.Visibility = System.Windows.Visibility.Collapsed;
            spMsg_Success.Visibility = System.Windows.Visibility.Visible;
            applicationViewModel.IsCurrentCanvasShared = true;
            //applicationViewModel.ShareCanvasCompletedEvent -= new ShareCanvasCompletedEventHandler(applicationViewModel_ShareCanvasCompletedEvent);
            waitCursor.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void ReadCanvasSharedUsers()
        {
            List<CanvasShareStatusDto> cssDto = applicationViewModel.CurrentCanvasShareStatusDto;

            SharedCanvasUserList = new List<UserListInfo>();
            for (int i = 0; i < cssDto.Count; i++)
            {
                if (cssDto[i].Shared)
                {
                    SharedCanvasUserList.Add(new UserListInfo(false, cssDto[i].FirstName, cssDto[i].LastName, cssDto[i].UserID));   //using false so that users wont appear selected on the Grid.
                }
            }


            dgSharedUsers.ItemsSource = null;
            dgSharedUsers.ItemsSource = new UserListCollection(SharedCanvasUserList);
        }



        private void ReadAllUsersForMyOrg()
        {
            UpdateUserList();
        }

        private void UpdateUserList()
        {

            List<CanvasShareStatusDto> cssDto = applicationViewModel.CurrentCanvasShareStatusDto;

            //  List<DatasourceUserDto> dsuDto;

            AdminDatasourceViewModel adminDatasourceViewModel = new AdminDatasourceViewModel();

            EventHandler GetAllDatasourceUserComplete = (s, e) =>
            {
                var dsuDto = (IEnumerable<DatasourceUserDto>)s;
                userList = new List<UserListInfo>();

                foreach (CanvasShareStatusDto cssd in cssDto)
                {
                    var cssd1 = cssd;
                    var approved = dsuDto.Select(d =>
                        d.UserId == cssd1.UserID &&
                        d.DatasourceId == applicationViewModel.EwavSelectedDatasource.DatasourceID);

                    var approved2 = from d in dsuDto
                                    where d.UserId == cssd1.UserID &&
                                          d.DatasourceId == applicationViewModel.EwavSelectedDatasource.DatasourceID
                                    select d.UserId;



                    //d.UserId == cssd1.UserID &&
                    //d.DatasourceId == applicationViewModel.EwavSelectedDatasource.DatasourceID);

                    if (approved2.Count() == 1)
                    {
                        userList.Add(new UserListInfo(cssd.Shared, cssd.FirstName, cssd.LastName, cssd.UserID));
                    }
                }


                dgUsers.ItemsSource = null;
                dgUsers.ItemsSource = new UserListCollection(userList);



            };

            adminDatasourceViewModel.GetAllDatasourceUserCompletedEvent += GetAllDatasourceUserComplete;

            adminDatasourceViewModel.GetAllDatasourceUser();

        }




        //private void MakeUserNamesList(DatatableBag dtb)
        //{
        //    userList = new List<UserListInfo>();
        //    for (int i = 0; i < dtb.RecordList.Count; i++)
        //    {
        //        if (dtb.GetValueAtRow("UserID", dtb.RecordList[i]) != applicationViewModel.LoggedInUser.UserDto.UserID)
        //        {


        //            userList.Add(new UserListInfo
        //                (false,
        //                dtb.GetValueAtRow("FirstName", dtb.RecordList[i]),
        //                dtb.GetValueAtRow("LastName", dtb.RecordList[i]),
        //                Convert.ToInt32(dtb.GetValueAtRow("UserID", dtb.RecordList[i]))
        //                ));
        //        }
        //    }
        //}

        private void BindGrid()
        {
            dgUsers.ItemsSource = null;
            dgUsers.ItemsSource = new UserListCollection(userList);
        }


        private void OKButton_Click(object sender, RoutedEventArgs e)
        {

            if (ApplicationViewModel.Instance.DemoMode)
            {
                DemoMode dm = new DemoMode();
                dm.Show();
                return;
            }



            waitCursor.Visibility = System.Windows.Visibility.Visible;
            UserListCollection userList = (UserListCollection)dgUsers.ItemsSource;

            var filteredList = from user in userList
                               where user.IsSelected == true
                               select new { FirstName = user.FirstName, LastName = user.LastName, UserID = user.UserId };
            List<int> userIds = new List<int>();



            foreach (var item in filteredList)
            {
                userIds.Add(item.UserID);
            }
            if (applicationViewModel.CurrentCanvasId > 0)
            {
                applicationViewModel.ShareCanvas(applicationViewModel.CurrentCanvasId, userIds);

                string xx = string.Join<int>(",", userIds);

            }
            else
            {
                MessageBox.Show("Cannot share unsaved canvas.");
            }

            //this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void select_all_Click(object sender, RoutedEventArgs e)
        {
            UpdateUserList(true);
        }

        private void UpdateUserList(bool AllSelected)
        {
            DatatableBag dtb = applicationViewModel.AllUsersInMyOrg;
            userList = new List<UserListInfo>();
            for (int i = 0; i < dtb.RecordList.Count; i++)
            {
                if (dtb.GetValueAtRow("USERNAME", dtb.RecordList[i]) != applicationViewModel.LoggedInUser.UserDto.UserName)
                {


                    userList.Add(new UserListInfo
                        (AllSelected,
                        dtb.GetValueAtRow("FIRSTNAME", dtb.RecordList[i]),
                        dtb.GetValueAtRow("LASTNAME", dtb.RecordList[i]),
                        Convert.ToInt32(dtb.GetValueAtRow("USERID", dtb.RecordList[i]))
                        ));
                }
            }
            BindGrid();


        }

        private void UpdateSharedUserList(bool AllSelected)
        {
            UserListCollection SharedUserList = (UserListCollection)dgSharedUsers.ItemsSource;
            foreach (var item in SharedUserList)
            {
                item.IsSelected = AllSelected;
            }

            dgSharedUsers.ItemsSource = null;
            dgSharedUsers.ItemsSource = SharedUserList;
        }

        private void btnBegin_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void clear_all_Click(object sender, RoutedEventArgs e)
        {
            UpdateUserList(false);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnSendEmail_Click(object sender, RoutedEventArgs e)
        {

            if (ApplicationViewModel.Instance.DemoMode)
            {
                DemoMode dm = new DemoMode();
                dm.Show();
                return;
            }


            UserListCollection SharedUserList = (UserListCollection)dgSharedUsers.ItemsSource;



            var filteredList = from user in SharedUserList
                               where user.IsSelected == true
                               select new { FirstName = user.FirstName, LastName = user.LastName, UserID = user.UserId };
            List<int> userIds = new List<int>();

            if (filteredList.Count() == 0)
            {
                spMsg.Visibility = System.Windows.Visibility.Visible;
                tbSaveError.Visibility = System.Windows.Visibility.Visible;
                return;
            }

            foreach (var item in filteredList)
            {
                userIds.Add(item.UserID);
            }
            if (applicationViewModel.CurrentCanvasId > 0)
            {
                applicationViewModel.ResendEmail(applicationViewModel.CurrentCanvasId, userIds);
            }
            waitCursor.Visibility = System.Windows.Visibility.Visible;
            spMsg.Visibility = System.Windows.Visibility.Collapsed;
            btnSendEmail.IsEnabled = false;
        }

        private void select_shared_Click(object sender, RoutedEventArgs e)
        {
            UpdateSharedUserList(true);
        }

        private void clear_shared_Click(object sender, RoutedEventArgs e)
        {
            UpdateSharedUserList(false);
        }

    }

    //public class UserListInfo : INotifyPropertyChanged
    //{
    //    public event PropertyChangedEventHandler PropertyChanged;

    //    private bool isSelected;

    //    public bool IsSelected
    //    {
    //        get { return isSelected; }
    //        set
    //        {
    //            isSelected = value;
    //            OnPropertyChanged("IsSelected");
    //        }
    //    }

    //    private void OnPropertyChanged(string p)
    //    {
    //        PropertyChangedEventHandler handler = PropertyChanged;
    //        if (handler != null)
    //        {
    //            handler(this, new PropertyChangedEventArgs(p));
    //        }
    //    }

    //    private string firstName;

    //    public string FirstName
    //    {
    //        get { return firstName; }
    //        set
    //        {
    //            firstName = value;
    //            OnPropertyChanged("FirstName");
    //        }
    //    }

    //    private string lastName;

    //    public string LastName
    //    {
    //        get { return lastName; }
    //        set
    //        {
    //            lastName = value;
    //            OnPropertyChanged("LastName");
    //        }
    //    }

    //    private int userId;
    //    public int UserId
    //    {
    //        get { return userId; }
    //        set { userId = value; OnPropertyChanged("UserID"); }
    //    }

    //    public UserListInfo(bool isSelected, string firstName, string lastName, int userId)
    //    {
    //        this.IsSelected = isSelected;
    //        this.FirstName = firstName;
    //        this.LastName = lastName;
    //        this.UserId = userId;
    //    }

    //}

    //public class UserListCollection : ObservableCollection<UserListInfo>
    //{
    //    public UserListCollection(List<UserListInfo> userList)
    //        : base()
    //    {
    //        for (int i = 0; i < userList.Count; i++)
    //        {
    //            Add(new UserListInfo(userList[i].IsSelected,
    //                                userList[i].FirstName,
    //                                userList[i].LastName,
    //                                userList[i].UserId));
    //        }
    //    }
    //}
}