/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       TextControl.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Printing;
using System.Windows.Resources;
using System.Windows.Shapes;

using System.Xml.Linq;
using CommonLibrary;
using Ewav.Web.EpiDashboard;
using Ewav.BAL;
using Ewav.ViewModels;
using Ewav.Web.Services;
using Ewav.Services;
using System.Globalization;

using Ewav.ExtensionMethods;        

namespace Ewav
{
    public partial class TextControl : UserControl, IGadget, IEwavGadget
    {
        public TextControl()
        {
            InitializeComponent();


        }

        #region Bold, Italics & Underline



        bool ToolbarToggle  = false;
        bool ToolbarCurrentState = false;    


        /// <summary>
        /// Toggles the read only.
        /// </summary>
        public void ToggleReadOnly()
        {
            if (ToolbarToggle == true)
            {
                this.rct42.Visibility = System.Windows.Visibility.Visible;
                ToolBarGrid.Visibility = System.Windows.Visibility.Visible;
                GWindow.Visibility = System.Windows.Visibility.Visible;
                ApplicationBorder.BorderThickness = new Thickness(1);

                ToolbarCurrentState = true;     


            }
            else
            {
                this.rct42.Visibility = System.Windows.Visibility.Collapsed;
                ToolBarGrid.Visibility = System.Windows.Visibility.Collapsed;
                GWindow.Visibility = System.Windows.Visibility.Collapsed;
                ApplicationBorder.BorderThickness = new Thickness(0);

                ToolbarCurrentState = false;    


            }

            ToolbarToggle = !ToolbarToggle;
        }

        //Set Bold formatting to the selected content 
        private void btnBold_Click(object sender, RoutedEventArgs e)
        {
            if (rtb != null && rtb.Selection.Text.Length > 0)
            {
                if (rtb.Selection.GetPropertyValue(Run.FontWeightProperty) is FontWeight && ((FontWeight)rtb.Selection.GetPropertyValue(Run.FontWeightProperty)) == FontWeights.Normal)
                    rtb.Selection.ApplyPropertyValue(Run.FontWeightProperty, FontWeights.Bold);
                else
                    rtb.Selection.ApplyPropertyValue(Run.FontWeightProperty, FontWeights.Normal);
            }
            ReturnFocus();
        }

        //Set Italic formatting to the selected content 
        private void btnItalic_Click(object sender, RoutedEventArgs e)
        {
            if (rtb != null && rtb.Selection.Text.Length > 0)
            {
                if (rtb.Selection.GetPropertyValue(Run.FontStyleProperty) is FontStyle && ((FontStyle)rtb.Selection.GetPropertyValue(Run.FontStyleProperty)) == FontStyles.Normal)
                    rtb.Selection.ApplyPropertyValue(Run.FontStyleProperty, FontStyles.Italic);
                else
                    rtb.Selection.ApplyPropertyValue(Run.FontStyleProperty, FontStyles.Normal);
            }
            ReturnFocus();
        }

        //Set Underline formatting to the selected content 
        private void btnUnderline_Click(object sender, RoutedEventArgs e)
        {
            if (rtb != null && rtb.Selection.Text.Length > 0)
            {
                if (rtb.Selection.GetPropertyValue(Run.TextDecorationsProperty) == null)
                    rtb.Selection.ApplyPropertyValue(Run.TextDecorationsProperty, TextDecorations.Underline);
                else
                    rtb.Selection.ApplyPropertyValue(Run.TextDecorationsProperty, null);
            }
            ReturnFocus();

        }
        #endregion

        #region Font Type, Color & size

        //Set font type to selected content
        private void cmbFonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (rtb != null && rtb.Selection.Text.Length > 0)
            {
                rtb.Selection.ApplyPropertyValue(Run.FontFamilyProperty, new FontFamily((cmbFonts.SelectedItem as ComboBoxItem).Tag.ToString()));
            }
            ReturnFocus();
        }

        //Set font size to selected content
        private void cmbFontSizes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (rtb != null && rtb.Selection.Text.Length > 0)
            {
                rtb.Selection.ApplyPropertyValue(Run.FontSizeProperty, double.Parse((cmbFontSizes.SelectedItem as ComboBoxItem).Tag.ToString()));
            }
            ReturnFocus();
        }

        //Set font color to selected content
        private void cmbFontColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (rtb != null && rtb.Selection.Text.Length > 0)
            {
                string color = (cmbFontColors.SelectedItem as ComboBoxItem).Tag.ToString();

                SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(
                    byte.Parse(color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(color.Substring(6, 2), System.Globalization.NumberStyles.HexNumber)));

                rtb.Selection.ApplyPropertyValue(Run.ForegroundProperty, brush);
            }
            ReturnFocus();
        }
        #endregion

        #region Insert UIElements

        //Insert an image into the RichTextBox
        private void btnImage_Click(object sender, RoutedEventArgs e)
        {
            InlineUIContainer container = new InlineUIContainer();

            container.Child = TextControl.createImageFromUri(new Uri("/SilverlightTextEditor;component/images/Desert.jpg", UriKind.RelativeOrAbsolute), 200, 150);

            rtb.Selection.Insert(container);
            ReturnFocus();
        }

        private static Image createImageFromUri(Uri URI, double width, double height)
        {
            Image img = new Image();
            img.Stretch = Stretch.Uniform;
            img.Width = width;
            img.Height = height;
            BitmapImage bi = new BitmapImage(URI);
            img.Source = bi;
            img.Tag = bi.UriSource.ToString();
            return img;
        }

        //Insert a Datagrid into the RichTextBox
        private void btnDatagrid_Click(object sender, RoutedEventArgs e)
        {
            InlineUIContainer container = new InlineUIContainer();

            container.Child = getDataGrid();

            rtb.Selection.Insert(container);
            ReturnFocus();
        }

        private DataGrid getDataGrid()
        {
            DataGrid dg = new DataGrid();
            dg.AutoGenerateColumns = true;
            dg.Width = 500;
            dg.Height = 150;
            //dg.ItemsSource = Customer.GetSampleCustomerList();
            dg.Style = (Style)this.Resources["DataGridStyle1"];

            return dg;
        }

        //Insert a Calendar into the RichTextBox
        private void btnCalendar_Click(object sender, RoutedEventArgs e)
        {
            //InlineUIContainer container = new InlineUIContainer();

            //container.Child = getCalendar();

            //rtb.Selection.Insert(container);
            //ReturnFocus();
        }

        //private Calendar getCalendar()
        //{
        //    Calendar cal = new Calendar();
        //    cal.Width = 179;
        //    cal.Height = 169;
        //    cal.FontFamily = new FontFamily("Portable User Interface");
        //    cal.Style = (Style)this.Resources["CalendarStyle1"];

        //    return cal;
        //}

        #endregion

        #region Insert Hyperlink

        //Insert a hyperlink
        private void btnHyperlink_Click(object sender, RoutedEventArgs e)
        {
            //InsertURL cw = new InsertURL(rtb.Selection.Text);
            //cw.HasCloseButton = false;

            ////Hook up an event handler to the Closed event on the ChildWindows cw. 
            //cw.Closed += (s, args) =>
            //{
            //    if (cw.DialogResult.Value)
            //    {
            //        Hyperlink hyperlink = new Hyperlink();
            //        hyperlink.TargetName = "_blank";
            //        hyperlink.NavigateUri = new Uri(cw.txtURL.Text);

            //        if (cw.txtURLDesc.Text.Length > 0)
            //            hyperlink.Inlines.Add(cw.txtURLDesc.Text);
            //        else
            //            hyperlink.Inlines.Add(cw.txtURL.Text);

            //        rtb.Selection.Insert(hyperlink);
            //        ReturnFocus();
            //    }
            //};
            //cw.Show();
        }
        #endregion

        #region Clipboard Operations

        //Cut the selected text
        private void btnCut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(rtb.Selection.Text);
            }
            catch (System.Security.SecurityException)
            {
                // this exception is thrown when the user declines to give
                // permission for this web app to write to the clipboard
            }
            catch { }

            rtb.Selection.Text = "";
            ReturnFocus();
        }

        //Copy the selected text
        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(rtb.Selection.Text);
            }
            catch (System.Security.SecurityException)
            {
                // this exception is thrown when the user declines to give
                // permission for this web app to write to the clipboard
            }
            catch { }

            ReturnFocus();
        }

        //paste the text
        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            rtb.Selection.Text = Clipboard.GetText();
            ReturnFocus();
        }
        #endregion

        #region Print

        //Print the document
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            //PrintPreview cw = new PrintPreview();
            //cw.ShowPreview(rtb);
            //cw.HasCloseButton = false;

            ////Hook up a handler to the Closed event before we display the PrintPreview window by calling the Show() method.
            //cw.Closed += (t, a) =>
            //{
            //    if (cw.DialogResult.Value)
            //    {
            //        PrintDocument theDoc = new PrintDocument();
            //        theDoc.PrintPage += (s, args) =>
            //        {
            //            args.PageVisual = rtb;
            //            args.HasMorePages = false;
            //        };

            //        theDoc.EndPrint += (s, args) =>
            //        {
            //            MessageBox.Show("The document printed successfully", "Text Editor", MessageBoxButton.OK);
            //        };

            //        theDoc.Print("Silverlight 4 Text Editor");
            //        ReturnFocus();
            //    }
            //};
            //cw.Show();
        }

        #endregion

        #region LeftToRight FlowDirection

        //Set the flow direction
        public void btnRTL_Checked(object sender, RoutedEventArgs e)
        {
            //Set the button image based on the state of the toggle button. 
            //if (btnRTL.IsChecked.Value)
            //    btnRTL.Content = TextControl.createImageFromUri(new Uri("/SilverlightTextEditor;component/images/rtl.png", UriKind.RelativeOrAbsolute), 30, 32);
            //else
            //    btnRTL.Content = TextControl.createImageFromUri(new Uri("/SilverlightTextEditor;component/images/ltr.png", UriKind.RelativeOrAbsolute), 30, 32);

            //ApplicationBorder.FlowDirection = (ApplicationBorder.FlowDirection == System.Windows.FlowDirection.LeftToRight) ? System.Windows.FlowDirection.RightToLeft : System.Windows.FlowDirection.LeftToRight;
            //ReturnFocus();

        }
        #endregion

        #region XAML Markup


        string string64;

        //Set the xamlTb TextBox with the current XAML of the RichTextBox and make it visible. Any changes to the XAML made 
        //in xamlTb is also reflected back on the RichTextBox. Note that the Xaml string returned by RichTextBox.Xaml will 
        //not include any UIElement contained in the current RichTextBox. Hence the UIElements will be lost when we 
        //set the Xaml back again from the xamlTb to the RichTextBox.
        public void btnMarkUp_Checked(object sender, RoutedEventArgs e)
        {
            if (btnMarkUp.IsChecked.Value)
            {
                xamlTb.Visibility = System.Windows.Visibility.Visible;
                xamlTb.IsTabStop = true;
                xamlTb.Text = rtb.Xaml;

                //byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.Unicode.GetBytes(rtb.Xaml);
                //string64 = Convert.ToBase64String(toEncodeAsBytes);



            }
            else
            {

                //byte[] encodedDataAsBytes = System.Convert.FromBase64String(string64);
                //string returnValue =
                //   System.Text.ASCIIEncoding.Unicode.GetString(encodedDataAsBytes);


                rtb.Xaml = xamlTb.Text;
                xamlTb.Visibility = System.Windows.Visibility.Collapsed;
                xamlTb.IsTabStop = false;
            }

        }

        #endregion

        #region Read only RichTextBox

        //Make the RichTextBox read-only
        public void btnRO_Checked(object sender, RoutedEventArgs e)
        {
            rtb.IsReadOnly = !rtb.IsReadOnly;

            //Set the button image based on the state of the toggle button.
            if (rtb.IsReadOnly)
                btnRO.Content = TextControl.createImageFromUri(new Uri("/SilverlightTextEditor;component/images/view.png", UriKind.RelativeOrAbsolute), 29, 32);
            else
                btnRO.Content = TextControl.createImageFromUri(new Uri("/SilverlightTextEditor;component/images/edit.png", UriKind.RelativeOrAbsolute), 29, 32);
            ReturnFocus();
        }

        #endregion

        #region Context Menu

        //Though we dont execute any logic on Right Mouse button down, we need to ensure the event is set to be handled to allow
        //the subsequent Right Mouse button up to be raised on the control. The context menu is displayed when the right mouse
        //button up event is raised. 
        private void rtb_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
              
            e.Handled = true;
        }

        private void rtb_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Construct and display the context menu

            //RTBContextMenu menu = new RTBContextMenu(rtb);
            //menu.Show(e.GetSafePosition(LayoutRoot));
        }
        #endregion

        #region Highlight

        private List<Rect> m_selectionRect = new List<Rect>();

        public void btnHighlight_Checked(object sender, RoutedEventArgs e)
        {
            if (btnHighlight.IsChecked.Value)
            {
                TextPointer tp = rtb.ContentStart;
                TextPointer nextTp = null;
                Rect nextRect = Rect.Empty;
                Rect tpRect = tp.GetCharacterRect(LogicalDirection.Forward);
                Rect lineRect = Rect.Empty;

                int lineCount = 1;

                while (tp != null)
                {
                    nextTp = tp.GetNextInsertionPosition(LogicalDirection.Forward);
                    if (nextTp != null && nextTp.IsAtInsertionPosition)
                    {
                        nextRect = nextTp.GetCharacterRect(LogicalDirection.Forward);
                        // this occurs for more than one line
                        if (nextRect.Top > tpRect.Top)
                        {
                            if (m_selectionRect.Count < lineCount)
                                m_selectionRect.Add(lineRect);
                            else
                                m_selectionRect[lineCount - 1] = lineRect;

                            lineCount++;

                            if (m_selectionRect.Count < lineCount)
                                m_selectionRect.Add(nextRect);

                            lineRect = nextRect;
                        }
                        else if (nextRect != Rect.Empty)
                        {
                            if (tpRect != Rect.Empty)
                                lineRect.Union(nextRect);
                            else
                                lineRect = nextRect;
                        }
                    }
                    tp = nextTp;
                    tpRect = nextRect;
                }
                if (lineRect != Rect.Empty)
                {
                    if (m_selectionRect.Count < lineCount)
                        m_selectionRect.Add(lineRect);
                    else
                        m_selectionRect[lineCount - 1] = lineRect;
                }
                while (m_selectionRect.Count > lineCount)
                {
                    m_selectionRect.RemoveAt(m_selectionRect.Count - 1);
                }
            }
            else
            {
                if (highlightRect != null)
                {
                    highlightRect.Visibility = System.Windows.Visibility.Collapsed;
                }
            }

        }

        Rectangle highlightRect;
        MouseEventArgs lastRTBMouseMove;
        private void rtb_MouseMove(object sender, MouseEventArgs e)
        {
            lastRTBMouseMove = e;
            if (btnHighlight.IsChecked.Value)
            {
                foreach (Rect r in m_selectionRect)
                {
                    if (r.Contains(e.GetSafePosition(highlightCanvas)))
                    {
                        if (highlightRect == null)
                        {
                            highlightRect = CreateHighlightRectangle(r);
                        }
                        else
                        {
                            highlightRect.Visibility = System.Windows.Visibility.Visible;
                            highlightRect.Width = r.Width;
                            highlightRect.Height = r.Height;
                            Canvas.SetLeft(highlightRect, r.Left);
                            Canvas.SetTop(highlightRect, r.Top);
                        }
                    }
                }
            }
        }

        private Rectangle CreateHighlightRectangle(Rect bounds)
        {
            Rectangle r = new Rectangle();
            r.Fill = new SolidColorBrush(Color.FromArgb(75, 0, 0, 200));
            r.Stroke = new SolidColorBrush(Color.FromArgb(230, 0, 0, 254));
            r.StrokeThickness = 1;
            r.Width = bounds.Width;
            r.Height = bounds.Height;
            Canvas.SetLeft(r, bounds.Left);
            Canvas.SetTop(r, bounds.Top);

            highlightCanvas.Children.Add(r);

            return r;

        }
        #endregion

        #region DragAndDrop

        private void rtb_Drop(object sender, System.Windows.DragEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);

            //the Drop event passes in an array of FileInfo objects for the list of files that were selected and drag-dropped onto the RichTextBox.
            if (e.Data == null)
            {
                ReturnFocus();
                return;
            }

            //This checks if the dropped objects are files and if not, return. 
            IDataObject f = e.Data as IDataObject;

            if (f == null)
            {
                ReturnFocus();
                return;
            }

            object data = f.GetData(DataFormats.FileDrop);
            FileInfo[] files = data as FileInfo[];

            if (files == null)
            {
                ReturnFocus();
                return;
            }

            //Walk through the list of FileInfo objects of the selected and drag-dropped files and parse the .txt and .docx files 
            //and insert their content in the RichTextBox.
            foreach (FileInfo file in files)
            {
                if (file == null)
                {
                    continue;
                }

                if (file.Extension.Equals(".txt"))
                {
                    ParseTextFile(file);
                }
                else if (file.Extension.Equals(".docx"))
                {
                    ParseDocxFile(file);
                }
            }
            ReturnFocus();
        }

        //Create a StreamReader on the text file and read as a string. 
        private void ParseTextFile(FileInfo file)
        {
            Stream sr = file.OpenRead();
            string contents;
            using (StreamReader reader = new StreamReader(sr))
            {
                contents = reader.ReadToEnd();
            }

            rtb.Selection.Text = contents;
            sr.Close();
        }

        private void ParseDocxFile(FileInfo file)
        {
            Stream sr = file.OpenRead();
            string contents;

            StreamResourceInfo zipInfo = new StreamResourceInfo(sr, null);
            StreamResourceInfo wordInfo = Application.GetResourceStream(zipInfo, new Uri("word/document.xml", UriKind.Relative));

            using (StreamReader reader = new StreamReader(wordInfo.Stream))
            {
                contents = reader.ReadToEnd();
            }

            XDocument xmlFile = XDocument.Parse(contents);
            XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

            var query = from xp in xmlFile.Descendants(w + "p")
                        select xp;
            Paragraph p = null;
            Run r = null;
            foreach (XElement xp in query)
            {
                p = new Paragraph();
                var query2 = from xr in xp.Descendants(w + "r")
                             select xr;
                foreach (XElement xr in query2)
                {
                    r = new Run();
                    var query3 = from xt in xr.Descendants()
                                 select xt;
                    foreach (XElement xt in query3)
                    {
                        if (xt.Name == (w + "t"))
                            r.Text = xt.Value.ToString();
                        else if (xt.Name == (w + "br"))
                            p.Inlines.Add(new LineBreak());
                    }
                    p.Inlines.Add(r);
                }
                p.Inlines.Add(new LineBreak());
                rtb.Blocks.Add(p);
            }
        }

        private void rtb_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            VisualStateManager.GoToState(this, "DragOver", true);
        }

        private void rtb_DragLeave(object sender, System.Windows.DragEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }

        #endregion

        #region FileOperations
        //Clears the contents of the existing file.
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            rtb.Blocks.Clear();
        }

        //Saves the existing file
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string ContentToSave = rtb.Xaml;

            //Check if the file contains any UIElements
            var res = from block in rtb.Blocks
                      from inline in (block as Paragraph).Inlines
                      where inline.GetType() == typeof(InlineUIContainer)
                      select inline;

            //If the file contains any UIElements, it will not be saved
            if (res.Count() != 0)
            {
                MessageBox.Show("Saving documents with UIElements is not supported");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = ".sav";
            sfd.Filter = "Saved Files|*.sav|All Files|*.*";

            if (sfd.ShowDialog().Value)
            {
                using (FileStream fs = (FileStream)sfd.OpenFile())
                {
                    System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
                    byte[] buffer = enc.GetBytes(ContentToSave);
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Close();
                }
            }
        }

        //Opens an existing file
        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Saved Files|*.sav|All Files|*.*";

            if (ofd.ShowDialog().Value)
            {
                FileInfo fi = ofd.File;
                StreamReader r = fi.OpenText();
                rtb.Xaml = r.ReadToEnd();
                r.Close();
            }
        }
        #endregion

        #region helper functions

        private void ReturnFocus()
        {
            if (rtb != null)
                rtb.Focus();
        }
        #endregion
        /// <summary>
        /// The value for the frameworkelement.Name property
        /// </summary>
        /// <value></value>
        public string MyControlName
        {
            get
            {
                return "TextControl";
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
                return "Text Tool";
            }
        }

        /// <summary>
        /// Reloads this instance.
        /// </summary>
        public void Reload()
        {
    

        }

        public StringBuilder HtmlBuilder { get; set; }

        /// <summary>
        /// Refreshes the results.
        /// </summary>
        public void RefreshResults()
        {
   

        }



        XNode IGadget.Serialize(XDocument doc)
        {



            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.Unicode.GetBytes(rtb.Xaml);
            string string64 = Convert.ToBase64String(toEncodeAsBytes);

            string aa = rtb.Xaml.To64();        


            XElement element = new XElement("gadget",
                new XAttribute("top", Canvas.GetTop(this).ToString("F0")),
                new XAttribute("left", Canvas.GetLeft(this).ToString("F0")),
                new XAttribute("collapsed", "false"),
                new XAttribute("xaml", string64),
                new XAttribute("gadgetType", "Ewav.TextControl"),
                new XAttribute("toolbar_open", ToolbarCurrentState));    





            //      new XElement("text", text1.Text));




            return element;




        }


        ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;



        /// <summary>
        /// Creates from XML.
        /// </summary>
        /// <param name="element">The element.</param>
        public void CreateFromXml(XElement element)
        {


            ClientCommon.Common cmnClass = new ClientCommon.Common();

            foreach (XElement child in element.Descendants())
            {
                switch (child.Name.ToString().ToLower())
                {
                }
            }


            double mouseVerticalPosition = 0.0, mouseHorizontalPosition = 0.0;


            string thisName = "";

            foreach (XAttribute attribute in element.Attributes())
            {

                thisName = attribute.Name.ToString().ToLower();

                switch (thisName)
                {
                    case "top":
                        mouseVerticalPosition = double.Parse(element.Attribute("top").Value.ToString(),
                            new CultureInfo(applicationViewModel.LoadedCanvasDto.Culture));

                        break;
                    case "left":
                        mouseHorizontalPosition = double.Parse(element.Attribute("left").Value.ToString(),
                             new CultureInfo(applicationViewModel.LoadedCanvasDto.Culture));
                        break;

                    case "xaml":

                        byte[] encodedXamlBytes = Convert.FromBase64String(element.Attribute("xaml").Value.ToString());
                        
                        string xaml = ASCIIEncoding.Unicode.GetString(encodedXamlBytes);
                        
                        if(!string.IsNullOrEmpty(xaml))
                        { 
                            rtb.Xaml = xaml;
                        }

                        break;    

                    case "toolbar_open"    :


                        ToolbarCurrentState  = Convert.ToBoolean(element.Attribute("toolbar_open").Value);
                        ToolbarToggle = ToolbarCurrentState;    



                        ToggleReadOnly();    



                        break;    



                }



            }

            cmnClass.AddControlToCanvas(this, mouseVerticalPosition, mouseHorizontalPosition, applicationViewModel.LayoutRoot);



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

            HtmlBuilder.Append("Text control HTML not implemented ");
            return "Text control HTML not implemented ";

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

        /// <summary>
        /// Sets the state of the gadget to processing.
        /// </summary>
        public void SetGadgetToProcessingState()
        {
            // TODO: Implement this method
            throw new NotImplementedException();
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
        /// Updates the variable names.
        /// </summary>
        public void UpdateVariableNames()
        {
            // TODO: Implement this method
            throw new NotImplementedException();
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


        ClientCommon.Common common = new ClientCommon.Common();

        private void LayoutRoot_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetSafePosition(this.common.GetParentObject<Grid>(this, "LayoutRoot"));
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).GadgetNameOnRightClick = this.MyControlName; //"FrequencyControl";
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).SelectedGadget = this;
            common.UpdateZOrder(this, true, common.GetParentObject<Grid>(this, "LayoutRoot"));

            //  e.Handled = true;  


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
        /// Closes the gadget after confirmation.
        /// </summary>
        public void CloseGadgetOnClick()
        {
            CloseGadget confirm = new CloseGadget(this);
            confirm.Show();
        }

        /// <summary>
        /// Closes the gadget.
        /// </summary>
        public void CloseGadget()
        {
            DragCanvas dc = this.Parent as DragCanvas;
            Canvas parentCanvas = (Canvas)this.Parent;
            parentCanvas.Children.Remove((UIElement)this);
            dc.Cleanup(this as UserControl);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        public List<EwavDataFilterCondition> GadgetFilters
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
    }
}