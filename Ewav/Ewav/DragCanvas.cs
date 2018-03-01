/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       DragCanvas.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;
using Ewav.Web.EpiDashboard;
using Ewav.Common;
using Ewav.Web.Services;
using Ewav.ViewModels;
using Ewav.Client.Application;
using System.Windows.Media.Imaging;
using System.IO;
using WriteableBitmapScreenCapture;

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;



namespace Ewav
{
    /// <summary>
    /// A class used in replacement of Canvas. It provides the dragging functionality of the controls added to it.
    /// </summary>
    public partial class DragCanvas : Canvas
    {
        #region Data
        /// <summary>
        /// This section contains Variables that this class use to perform functionality.
        /// </summary>
        //Variable used for determining if the control is in draggable state.
        bool isMouseCaptured;

        //Variable keeps the vertical position of the control.
        double mouseVerticalPosition;

        //Variable keeps the horizontal position of the control.
        double mouseHorizontalPosition;

        //Variable contains the reference for the control that is being dragged.
        UIElement controlBeingDragged;

        //Variable that holds the reference for current Layout.
        Grid LayoutRoot;

        private UIElement elementBeingDragged;

        #endregion

        private GadgetParameters gadgetparameters;

        public GadgetParameters Gadgetparameters
        {
            get
            {
                return gadgetparameters;
            }
            set
            {
                gadgetparameters = value;
            }
        }

        private GridCells gridcells;

        public GridCells Gridcells
        {
            get
            {
                return gridcells;
            }
            set
            {
                gridcells = value;
            }
        }

        #region CanBeDragged

        public static readonly DependencyProperty CanBeDraggedProperty;

        public static bool GetCanBeDragged(UIElement uiElement)
        {
            if (uiElement == null)
                return false;

            return (bool)uiElement.GetValue(CanBeDraggedProperty);
        }

        public static void SetCanBeDragged(UIElement uiElement, bool value)
        {
            if (uiElement != null)
                uiElement.SetValue(CanBeDraggedProperty, value);
        }

        #endregion

        #region gadgetNameProperty
        string gadgetNameOnRightClick;
        public string GadgetNameOnRightClick
        {
            get
            {
                return gadgetNameOnRightClick;
            }
            set
            {
                gadgetNameOnRightClick = value;
            }
        }


        List<Grid> strataList;

        public List<Grid> StrataList
        {
            get
            {
                return strataList;
            }
            set
            {
                strataList = value;
            }
        }

        IGadget selectedGadget;

        public IGadget SelectedGadget
        {
            get
            {
                return selectedGadget;
            }
            set
            {
                selectedGadget = value;
            }
        }
        #endregion

        #region Dependency Properties
        public static readonly DependencyProperty AllowDraggingProperty;
        //public static readonly DependencyProperty AllowDragOutOfViewProperty;

        #endregion

        #region Static Constructor
        static DragCanvas()
        {
            CanBeDraggedProperty = DependencyProperty.RegisterAttached(
                "CanBeDragged",
                typeof(bool),
                typeof(DragCanvas),
                new PropertyMetadata(true));
            AllowDraggingProperty = DependencyProperty.Register(
                "AllowDragging",
                typeof(bool),
                typeof(DragCanvas),
                new PropertyMetadata(true));
            //AllowDragOutOfViewProperty = DependencyProperty.Register(
            //    "AllowDragOutOfView",
            //    typeof(bool),
            //    typeof(DragCanvas),
            //    new PropertyMetadata(false));
        }

        #endregion

        //EwavUserControlList eucl;

        #region Constructor

        public DragCanvas()
        {
            //if (gadgetNameOnRightClick.Equals(""))
            //{
            //}
            //    eucl = new EwavUserControlList();
            //     FrequencyControl freq = new FrequencyControl();     
            //  ObjectType instance = (ObjectType)Activator.CreateInstance("MyNamespace.ObjectType, MyAssembly");

            ApplicationViewModel.Instance.DatasourceChangedEvent += new DatasourceChangedEventHandler(DatasourceChangedEvent);
            this.Loaded += new RoutedEventHandler(DragCanvas_Loaded);

            this.MouseRightButtonDown += new MouseButtonEventHandler(DragCanvas_MouseRightButtonDown);
        }

        void DragCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {





        }

        void DragCanvas_Loaded(object sender, RoutedEventArgs e)
        {


            ApplicationViewModel.Instance.SaveCanvasCompletedEvent += new SaveCanvasCompletedEventHandler(ApplicationViewModel_SaveCanvasCompletedEvent);
        }

        void ApplicationViewModel_SaveCanvasCompletedEvent(object o)
        {


            try
            {
                //string snapshot = CaptureSnapshot2();
                //CanvasSnapshotComplete(snapshot);
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating canvas snapshot -- " + ex.Message + ex.StackTrace);
            }

            //if (snapshot.Length > 0)
            //    CanvasSnapshotComplete(snapshot);    
            //else 
            //      throw new Exception ("Error 





        }

        void DatasourceChangedEvent(object o, Client.Application.DatasourceChangedEventArgs e)
        {
            for (int i = this.Children.Count - 1; i >= 0; i--)
            //        foreach (UIElement ue in this.Children)
            {
                UserControl uc = this.Children[i] as UserControl;

                if (uc != null)
                {
                    IGadget gadget = uc as IGadget;

                    if (gadget != null)
                    {
                        gadget.CloseGadget();
                    }
                }
            }
        }

        #endregion

        #region AllowDragging

        public bool AllowDragging
        {
            get
            {
                return (bool)base.GetValue(AllowDraggingProperty);
            }
            set
            {
                base.SetValue(AllowDraggingProperty, value);
            }
        }

        #endregion

        //AllowDragOutOfView property is not very useful in the web scenario. So keeping the code but commenting out.

        //#region AllowDragOutOfView

        //public bool AllowDragOutOfView
        //{
        //    get { return (bool)GetValue(AllowDragOutOfViewProperty); }
        //    set { SetValue(AllowDragOutOfViewProperty, value); }
        //}

        //#endregion

        #region BringToFront / SendToBack

        public void BringToFront(UIElement element)
        {
            this.UpdateZOrder(element, true);
        }

        public void SendToBack(UIElement element)
        {
            this.UpdateZOrder(element, false);
        }

        #endregion

        #region ElementBeingDragged

        public UIElement ElementBeingDragged
        {
            get
            {
                if (!this.AllowDragging)
                    return null;
                else
                    return this.elementBeingDragged;
            }
            protected set
            {
                if (this.elementBeingDragged != null)
                    this.elementBeingDragged.ReleaseMouseCapture();

                if (!this.AllowDragging)
                    this.elementBeingDragged = null;
                else
                {
                    if (DragCanvas.GetCanBeDragged(value))
                    {
                        this.elementBeingDragged = value;
                        this.elementBeingDragged.CaptureMouse();
                    }
                    else
                        this.elementBeingDragged = null;
                }
            }
        }

        #endregion

        #region AddChild
        /// <summary>
        /// AddChild is a method that takes UIElement and Main Grid Layout as a parameter. This method wiresup the events for the child added to canvas.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="layoutRoot"></param>
        public void AddChild(UIElement element, Grid layoutRoot)
        {
            element.MouseLeftButtonDown += new MouseButtonEventHandler(element_MouseLeftButtonDown);
            element.MouseMove += new MouseEventHandler(element_MouseMove);
            element.MouseLeftButtonUp += new MouseButtonEventHandler(element_MouseLeftButtonUp);
            //element.MouseRightButtonDown += new MouseButtonEventHandler(element_MouseRightButtonDown);



            //if (element is Ewav.ContextMenu.Menu)
            //{
            //    ((ContextMenu.Menu)element).Name = ClientCommon.Common.ControlNameGenerator("contextmenu");
            //}

            this.LayoutRoot = layoutRoot;

            //if (this.Children.Contains(element) == false)
            //{
            this.Children.Add(element);
            //   }

        }

        #endregion

        /// <summary>
        ///  Memory leak POLICE     
        /// </summary>
        /// <param name="uc"></param>
        public void Cleanup(UserControl uc)
        {
            //  Unwire events  
            uc.MouseLeftButtonDown -= element_MouseLeftButtonDown;
            uc.MouseMove -= element_MouseMove;
            uc.MouseLeftButtonUp -= element_MouseLeftButtonUp;
            uc.MouseRightButtonDown -= element_MouseRightButtonDown;

            //         cEventHelper.RemoveAllEventHandlers(uc);    

            // Un-wire data context    
            uc.DataContext = null;

            this.Children.Remove(uc);

            ApplicationViewModel.Instance.Gadgets.Remove(uc);
            // This *should* be bye-bye    
            uc = null;
        }

        #region Events

        /// <summary>  
        /// This section contains all the events that are attached to the child, when child is added to canvas. And fire up when associated event is raised.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void element_MouseMove(object sender, MouseEventArgs e)
        {
            Handle_MouseMove(sender, e);
        }

        void element_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //PopupDisplayed = true;
        }

        void element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Handle_MouseUp(sender, e);
        }

        void element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {


            //    Handle_MouseDown(sender, e);

            UIElement item = sender as UIElement;



            if (item.GetType().BaseType.ToString() == "System.Windows.Controls.UserControl" &&
                !ApplicationViewModel.Instance.IsMouseDownOnMap)
            {
                this.ElementBeingDragged = item;
                mouseVerticalPosition = e.GetPosition(null).Y;
                mouseHorizontalPosition = e.GetPosition(null).X;
                BringToFront(sender as UIElement);
                isMouseCaptured = true;
                this.Cursor = Cursors.Hand;
                //item.CaptureMouse();
            }
            else
            {
                ApplicationViewModel.Instance.IsMouseDownOnMap = false;
            }







            //////////////////////////////////////////////////////////////////////////////////////////////

            //// create a WriteableBitmap
            //WriteableBitmap bitmap = new WriteableBitmap(
            //    (int)this.ActualWidth,
            //    (int)this.ActualHeight);

            //// render the visual element to the WriteableBitmap
            //bitmap.Render(this,  new ScaleTransform());

            //// request an redraw of the bitmap
            //bitmap.Invalidate();

            //try
            //{
            //    // locate the WriteableBitmap source for the clicked image          
            //    if (null == bitmap)
            //    {
            //        MessageBox.Show("Nothing to save");
            //        return;
            //    }

            //    // Create an instance of the open file dialog box.
            //    SaveFileDialog dialog = new SaveFileDialog();

            //    // prompt for a location to save it
            //    if (dialog.ShowDialog() == true)
            //    {
            //        // the "using" block ensures the stream is cleaned up when we are finished
            //        using (Stream stream = dialog.OpenFile())
            //        {
            //            // encode the stream
            //            JPGUtil.EncodeJpg(bitmap, stream);
            //        }
            //    }

            //}


            //catch (Exception ex) 
            //{



            //}


        }

        public void Handle_MouseDown(object sender, MouseEventArgs args)
        {
            //UIElement item = sender as UIElement;



            //if (item.GetType().BaseType.ToString() == "System.Windows.Controls.UserControl" &&  
            //    !ApplicationViewModel.Instance.IsMouseDownOnMap)
            //{
            //    this.ElementBeingDragged = item;
            //    mouseVerticalPosition = args.GetPosition(null).Y;
            //    mouseHorizontalPosition = args.GetPosition(null).X;
            //    BringToFront(sender as UIElement);
            //    isMouseCaptured = true;
            //    this.Cursor = Cursors.Hand;
            //    //item.CaptureMouse();
            //}
            //else
            //{
            //    ApplicationViewModel.Instance.IsMouseDownOnMap = false;
            //}



            //// create a WriteableBitmap
            //WriteableBitmap bitmap = new WriteableBitmap(
            //    (int)this.LayoutRoot.ActualWidth,
            //    (int)this.LayoutRoot.ActualHeight);

            //// render the visual element to the WriteableBitmap
            //bitmap.Render(this.LayoutRoot, new ScaleTransform());

            //// request an redraw of the bitmap
            //bitmap.Invalidate();

            //try
            //{
            //    // locate the WriteableBitmap source for the clicked image          
            //    if (null == bitmap)
            //    {
            //        MessageBox.Show("Nothing to save");
            //        return;
            //    }

            //    // Create an instance of the open file dialog box.
            //    SaveFileDialog dialog = new SaveFileDialog();

            //    // prompt for a location to save it
            //    if (dialog.ShowDialog() == true)
            //    {
            //        // the "using" block ensures the stream is cleaned up when we are finished
            //        using (Stream stream = dialog.OpenFile())
            //        {
            //            // encode the stream
            //            JPGUtil.EncodeJpg(bitmap, stream);
            //        }
            //    }

            //}


            //catch (Exception ex)
            //{



            //}


        }

        public void Handle_MouseMove(object sender, MouseEventArgs args)
        {
            controlBeingDragged = sender as UIElement;
            if (isMouseCaptured)
            {
                double screenWidth = Application.Current.Host.Content.ActualWidth;
                double screenHeight = Application.Current.Host.Content.ActualHeight;

                // Calculate the current position of the object.
                double deltaV = 0.0;
                double deltaH = 0.0;

                if (args.GetPosition(LayoutRoot).X < screenWidth * 0.93)
                {
                    if (args.GetPosition(LayoutRoot).X > 10)
                    {
                        deltaH = args.GetPosition(null).X - mouseHorizontalPosition;
                    }
                    else
                    {
                        deltaH = 0;
                    }
                }

                if (args.GetPosition(LayoutRoot).Y < 8000 * 0.99)
                {
                    if (args.GetPosition(LayoutRoot).Y > 59)
                    {
                        deltaV = args.GetPosition(null).Y - mouseVerticalPosition;
                    }
                    else
                    {
                        deltaV = 0;
                    }
                }

                double newTop = deltaV + (double)controlBeingDragged.GetValue(Canvas.TopProperty);
                double newLeft = deltaH + (double)controlBeingDragged.GetValue(Canvas.LeftProperty);

                // Set new position of object.
                controlBeingDragged.SetValue(Canvas.TopProperty, newTop);
                controlBeingDragged.SetValue(Canvas.LeftProperty, newLeft);

                // Update position global variables.
                mouseVerticalPosition = args.GetPosition(null).Y;
                mouseHorizontalPosition = args.GetPosition(null).X;
            }
        }

        public void Handle_MouseUp(object sender, MouseEventArgs args)
        {
            UIElement item = sender as UIElement;
            isMouseCaptured = false;
            item.ReleaseMouseCapture();
            mouseVerticalPosition = -1;
            mouseHorizontalPosition = -1;
            this.Cursor = Cursors.Arrow;
        }

        #endregion


        public static event CanvasSnapshotCompleteHandler CanvasSnapshotComplete;


        public string CaptureSnapshot()
        {

            try
            {


                ScaleTransform scaleTransform = new ScaleTransform();


         
                // create a WriteableBitmap
                WriteableBitmap bitmap = new WriteableBitmap(
                    (int)this.ActualWidth ,
                    (int)this.ActualHeight );    


         


                this.RenderTransform = scaleTransform;

                // render the visual element to the WriteableBitmap
                bitmap.Render(this, new ScaleTransform());

                // request an redraw of the bitmap
                bitmap.Invalidate();

                     
                Stream streamJPEG = new MemoryStream();
                JPGUtil.EncodeJpg(bitmap, streamJPEG);

            

                // concert stream1 of the new jpeg     
                byte[] m_Bytes = StreamHelper.GetAllBytes(streamJPEG);

                // to str        
                string string64 = Convert.ToBase64String(m_Bytes);


                // to jpeg         
                byte[] out_Bytes = Convert.FromBase64String(string64);


                MemoryStream outStreamJPEG = new MemoryStream(out_Bytes);

                //        outStreamJPEG.Write(out_Bytes, 0, out_Bytes.Length);    

                // Create an instance of the open file dialog box.
                //SaveFileDialog dialog = new SaveFileDialog();


                //// prompt for a location to save it
                //if (dialog.ShowDialog() == true)
                //{ 
                //    // the "using" block ensures the stream is cleaned up when we are finished
                //    using (Stream stream = dialog.OpenFile())
                //    {
                //        // encode the stream
                //        //    JPGUtil.EncodeJpg(bitmap, stream);    


                //        outStreamJPEG.Write(out_Bytes, 0, out_Bytes.Length);    



                //    }
                //}




                //// convert stream to string
                //StreamReader reader = new StreamReader(streamJPEG);
                //string text = reader.ReadToEnd();



                return string64;




            }
            catch (Exception ex)
            {


                throw new Exception("Error on capture " + ex.Message);

            }



        }

        //public string CaptureSnapshot2()
        //{

        //    try
        //    {


        //        // create a WriteableBitmap
        //        WriteableBitmap bitmap = new WriteableBitmap(
        //            (int)this.ActualWidth,
        //            (int)this.ActualHeight);



        //        int width = 500;  // (int)this.ActualWidth;
        //        int height = 500;  //  (int)this.ActualHeight;


        //        var pixels = new TrackingEnumerable<int>(width * height, (int)0x00ff00 | (0xff << 24));



        //        // render the visual element to the WriteableBitmap
        //        bitmap.Render(this, new ScaleTransform());

        //        // request an redraw of the bitmap
        //        bitmap.Invalidate();


        //        Stream streamPng = new MemoryStream();

        //        MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);


      


        //        var contents = PngEncoder.Encode(width, height, pixels);
        //        foreach (var b in contents)
        //        {
        //            streamPng.WriteByte(b);
        //        }


        //        // concert stream1 of the new jpeg     
        //        byte[] m_Bytes = StreamHelper.GetAllBytes(streamPng);

        //        // to str        
        //        string string64 = Convert.ToBase64String(m_Bytes);


        //        // to jpeg         
        //        byte[] out_Bytes = Convert.FromBase64String(string64);


        //        MemoryStream outStreamJPEG = new MemoryStream(out_Bytes);

        //        //        outStreamJPEG.Write(out_Bytes, 0, out_Bytes.Length);    

        //        // Create an instance of the open file dialog box.
        //        //SaveFileDialog dialog = new SaveFileDialog();


        //        //// prompt for a location to save it
        //        //if (dialog.ShowDialog() == true)
        //        //{ 
        //        //    // the "using" block ensures the stream is cleaned up when we are finished
        //        //    using (Stream stream = dialog.OpenFile())
        //        //    {
        //        //        // encode the stream
        //        //        //    JPGUtil.EncodeJpg(bitmap, stream);    


        //        //        outStreamJPEG.Write(out_Bytes, 0, out_Bytes.Length);    



        //        //    }
        //        //}




        //        //// convert stream to string
        //        //StreamReader reader = new StreamReader(streamJPEG);
        //        //string text = reader.ReadToEnd();



        //        return string64;




        //    }
        //    catch (Exception ex)
        //    {


        //        throw new Exception("Error on capture " + ex.Message);

        //    }



        //}
        #region Helper Methods

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

        /// <summary>
        /// This is helper method that sets the Z-index and brings the control on the front/back when it dragging.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="bringToFront"></param>
        private void UpdateZOrder(UIElement element, bool bringToFront)
        {
            List<UserControl> feList = GetChildObjects<UserControl>(this.LayoutRoot); // Makes list of FrameWorkElement class. Which is inherited by Control class.


            #region Safety Check

            if (element == null)
                throw new ArgumentNullException("element");

            #endregion

            #region Calculate Z-Indici And Offset

            int elementNewZIndex = -1;
            if (bringToFront)
            {
                foreach (UIElement elem in feList)
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

            foreach (UIElement childElement in feList)
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
                    }
                }
            }
            #endregion
        }


        public struct ControlFootprint
        {
            public double Bottom;

            public double Left;
            public double Right;
            public double Top;
        }



        public List<ControlFootprint> cfpList = new List<ControlFootprint>();

        private void Refresh()
        {
            this.UpdateLayout();

            this.cfpList.Clear();

            foreach (var c in GetChildObjects<UserControl>(this))
            // foreach (var c in this.CanvasToMap.AllChildren<Control>())
            {
                if (c is IGadget)
                {
                    UserControl uc = c as UserControl;


                    ControlFootprint cfp = new ControlFootprint();
                    cfp.Top = Canvas.GetTop(uc);
                    cfp.Left = Canvas.GetLeft(uc);
                    cfp.Bottom = cfp.Top + uc.ActualHeight;
                    cfp.Right = cfp.Left + uc.ActualWidth;


                    this.cfpList.Add(cfp);


                }
            }
        }


        #endregion
    }
}