/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       SaveDash.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Ewav.ViewModels;
using Ewav.Membership;
using Ewav.Views.Dialogs;

namespace Ewav
{
    public partial class SaveDash : ChildWindow
    {
        ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;
        public SaveDash(string message = "")
        {
            InitializeComponent();
            applicationViewModel.ErrorNoticeEvent += new Client.Application.ErrorNoticeEventHandler(applicationViewModel_ErrorNoticeEvent);
            applicationViewModel.SaveCanvasCompletedEvent += new Client.Application.SaveCanvasCompletedEventHandler(applicationViewModel_SaveCanvasCompletedEvent);
            
            if (message.Length > 0)
            {
                spMsg.Visibility = System.Windows.Visibility.Collapsed;
                spSave.Visibility = System.Windows.Visibility.Collapsed;
                spMsg_Success.Visibility = System.Windows.Visibility.Visible; 
            }
            else
            {
                //spMsg.Visibility = System.Windows.Visibility.Visible;
                //spSave.Visibility = System.Windows.Visibility.Visible;
                //spMsg_Success.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        //public SaveDash(string message) 
        //{
        //    InitializeComponent();
        //    applicationViewModel.SaveCanvasCompletedEvent += new Client.Application.SaveCanvasCompletedEventHandler(applicationViewModel_SaveCanvasCompletedEvent);
        void applicationViewModel_ErrorNoticeEvent(SimpleMvvmToolkit.NotificationEventArgs<Exception> o)
        {
            spMsg.Visibility = System.Windows.Visibility.Visible;

            if (o.Data != null)
            {
                tbSaveError.Text = o.Data.Message;
            }
        }

        //}
        void applicationViewModel_SaveCanvasCompletedEvent(object o)
        {
            spMsg.Visibility = System.Windows.Visibility.Collapsed;
            spSave.Visibility = System.Windows.Visibility.Collapsed;
            spMsg_Success.Visibility = System.Windows.Visibility.Visible;
            canvasName.Text = applicationViewModel.SelectedCanvasName;
        }

        private void btnBegin_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            this.DialogResult = true;
        }

        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            if (ApplicationViewModel.Instance.DemoMode)
            {
                DemoMode dm = new DemoMode();
                dm.Show();
                return;
            }


            // TODO: Add event handler implementation here.
            //spMsg_Success.Visibility = System.Windows.Visibility.Visible;
            //spSave.Visibility = System.Windows.Visibility.Collapsed;
            //if (spMsg.Visibility != System.Windows.Visibility.Visible)
            //{
            //}
            //else
            //{
            //    //spMsg.Visibility = System.Windows.Visibility.Collapsed;
            //    //this.DialogResult = true;
            //}
            XElement element = applicationViewModel.SerializeCanvas();

            CanvasDto dto = new CanvasDto();

            dto.CanvasName = txtSaveTitle.Text;
            dto.CanvasDescription = txtSaveDesc.Text;
            dto.CreatedDate = DateTime.Now;
            dto.ModifiedDate = DateTime.Now;
            // dto.DatasourceID = applicationViewModel.LoggedInUser.UserDto.DatasourceID;
            dto.DatasourceID = applicationViewModel.EwavSelectedDatasource.DatasourceID;
            dto.XmlData = element;
            dto.UserId = Convert.ToInt32(applicationViewModel.LoggedInUser.UserDto.UserID);
            dto.IsNewCanvas = true;
            applicationViewModel.SelectedCanvasName = txtSaveTitle.Text;
            applicationViewModel.SaveCanvas(element, dto);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}