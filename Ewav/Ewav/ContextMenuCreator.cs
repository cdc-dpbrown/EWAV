/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ContextMenuCreator.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ewav.Common;
using Ewav.ContextMenu;
using System.Text;
using System.Collections.Generic;
using System;
using System.Windows.Browser;
using System.Windows.Media;
using Ewav.Web.Services;
using Ewav.Web.EpiDashboard;
using CommonLibrary;
using System.ComponentModel.Composition;
using System.Linq;
using Ewav.ViewModels;

namespace Ewav
{
    public class ContextMenuCreator
    {
        Menu popupMenu;

        ViewModels.ApplicationViewModel applicationViewModel = ViewModels.ApplicationViewModel.Instance;

        public static event EventHandler MenuPopup;

        public Menu PopupMenu
        {
            get
            {
                return popupMenu;
            }
            set
            {
                popupMenu = value;
            }
        }


        #region Properties

        ClientCommon.Common cmnClass;

        /// <summary>
        /// The list of grids for each strata value. E.g. if stratifying by SEX, there would be (likely)
        /// two grids in this list: One showing the summary statistics for Male and another for Female.
        /// </summary>

        private List<Grid> strataGridList;

        public List<Grid> StrataGridList
        {
            get
            {
                return strataGridList;
            }
            set
            {
                strataGridList = value;
            }
        }

        /// <summary>
        /// Contains the reference for the Selected Gadget.
        /// </summary>
        private IGadget selectedGadget;

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

        /// <summary>
        /// Contains the reference for The Root Canvas.
        /// </summary>
        private Canvas root;

        public Canvas Root
        {
            get
            {
                return root;
            }
            set
            {
                root = value;
            }
        }

        private DragCanvas dgRoot;

        public DragCanvas DgRoot
        {
            get
            {
                return dgRoot;
            }
            set
            {
                dgRoot = value;
            }
        }
        /// <summary>
        /// Intializes the name of Control where the right click was clicked.
        /// </summary>
        private string controlName;

        public string ControlName
        {
            get
            {
                return controlName;
            }
            set
            {
                controlName = value;
            }
        }

        private bool popupDisplayed;

        public bool PopupDisplayed
        {
            get
            {
                return popupDisplayed;
            }
            set
            {
                popupDisplayed = value;
            }
        }

        private Grid layoutRoot;
        
        private GridCells gridCells;

        public GridCells GridCells
        {
            get
            {
                return gridCells;
            }
            set
            {
                gridCells = value;
            }
        }

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

        private bool displayWithOutDSSelected;

        public bool DisplayWithOutDSSelected
        {
            get { return displayWithOutDSSelected; }
            set { displayWithOutDSSelected = value; }
        }

        #endregion

        #region Constructors

        public ContextMenuCreator()
        {
        }

        private Point contextMenuRightClickCoords;
        
        /// <summary>
        /// Constructor that instantiats the contextMenu.
        /// </summary>
        /// <param name="controlName"></param>
        /// <param name="StrataList"></param>
        /// <param name="selectedGadget"></param>
        /// <param name="canvas"></param>
        /// <returns></returns>
        public Menu CreateContextMenu(string controlName, Point mouseButtonCoords,
            List<Grid> StrataList, IGadget selectedGadget, Canvas canvas,
            Grid LayoutRoot, GridCells gc, GadgetParameters gp)
        {
            contextMenuRightClickCoords = mouseButtonCoords;

            cmnClass = new ClientCommon.Common();
            popupMenu = new Menu();
            this.StrataGridList = StrataList;
            this.SelectedGadget = selectedGadget;
            this.Root = canvas;
            this.DgRoot = (DragCanvas)canvas;
            this.ControlName = controlName;
            this.GridCells = gc;
            this.Gadgetparameters = gp;

            Ewav.ContextMenu.MenuItem mnuCopy;
            Ewav.ContextMenu.MenuItem mnuSendDataToHTML;
            Ewav.ContextMenu.MenuItem mnuSendToBack;
            Ewav.ContextMenu.MenuItem mnuClose;

            switch (controlName.ToUpper())
            {
                case "FREQUENCYCONTROL":
                case "MEANSCONTROL":
                case "MXNTABLECONTROL":
                case "TWOXTWOTABLECONTROL":
                    mnuCopy = new Ewav.ContextMenu.MenuItem("Copy data to clipboard", "Images/clipboard.png");
                    mnuCopy.MouseLeftButtonDown += new MouseButtonEventHandler(mnuCopy_MouseLeftButtonDown);
                    mnuSendDataToHTML = new Ewav.ContextMenu.MenuItem("Send data to web browser", "Images/webexport.png");
                    mnuSendDataToHTML.MouseLeftButtonDown += new MouseButtonEventHandler(mnuSendDataToHTML_MouseLeftButtonDown);
                    mnuSendToBack = new Ewav.ContextMenu.MenuItem("Send gadget to back", "Images/sendback.png");
                    mnuSendToBack.MouseLeftButtonDown += new MouseButtonEventHandler(mnuSendToBack_MouseLeftButtonDown);
                    mnuClose = new Ewav.ContextMenu.MenuItem("Close this gadget", "Images/closegadget.png");
                    mnuClose.MouseLeftButtonDown += new MouseButtonEventHandler(mnuClose_MouseLeftButtonDown);
                    popupMenu.Items.Add(mnuCopy);
                    popupMenu.Items.Add(mnuSendDataToHTML);
                    popupMenu.Items.Add(mnuSendToBack);
                    popupMenu.Items.Add(mnuClose);
                    break;
                case "COMBINEDFREQUENCY":
                    mnuSendDataToHTML = new Ewav.ContextMenu.MenuItem("Send data to web browser", "Images/webexport.png");
                    mnuSendDataToHTML.MouseLeftButtonDown += new MouseButtonEventHandler(mnuSendDataToHTML_MouseLeftButtonDown);
                    mnuSendToBack = new Ewav.ContextMenu.MenuItem("Send gadget to back", "Images/sendback.png");
                    mnuSendToBack.MouseLeftButtonDown += new MouseButtonEventHandler(mnuSendToBack_MouseLeftButtonDown);
                    mnuClose = new Ewav.ContextMenu.MenuItem("Close this gadget", "Images/closegadget.png");
                    mnuClose.MouseLeftButtonDown += new MouseButtonEventHandler(mnuClose_MouseLeftButtonDown);
                    popupMenu.Items.Add(mnuSendDataToHTML);
                    popupMenu.Items.Add(mnuSendToBack);
                    popupMenu.Items.Add(mnuClose);
                    break;
                case "ABERRATIONCONTROL":
                case "STATCALC2X2":
                case "BINOMIAL":
                case "COHORT":
                case "POPULATION":
                case "UNMATCHED":
                case "CHISQUARE":
                case "POISSON":
                case "LINELIST":
                case "MAPCONTROL":
                case "LOGISTICREGRESSION":
                case "LINEARREGRESSION":
                    mnuSendToBack = new Ewav.ContextMenu.MenuItem("Send gadget to back", "Images/sendback.png");
                    mnuSendToBack.MouseLeftButtonDown += new MouseButtonEventHandler(mnuSendToBack_MouseLeftButtonDown);
                    mnuClose = new Ewav.ContextMenu.MenuItem("Close this gadget", "Images/closegadget.png");
                    mnuClose.MouseLeftButtonDown += new MouseButtonEventHandler(mnuClose_MouseLeftButtonDown);
                    popupMenu.Items.Add(mnuSendToBack);
                    popupMenu.Items.Add(mnuClose);
                    break;
                case "TEXTCONTROL":
                    Ewav.ContextMenu.MenuItem mnuCloseTextControl = new Ewav.ContextMenu.MenuItem("Close this gadget", "Images/closegadget.png");
                    mnuCloseTextControl.MouseLeftButtonDown += new MouseButtonEventHandler(mnuClose_MouseLeftButtonDown);
                    Ewav.ContextMenu.MenuItem mnuStopEdit = new Ewav.ContextMenu.MenuItem("Show/Hide Tool Bar", "");
                    mnuStopEdit.MouseLeftButtonDown += new MouseButtonEventHandler(mnuStopEdit_MouseLeftButtonDown);
                    popupMenu.Items.Add(mnuStopEdit);
                    popupMenu.Items.Add(mnuCloseTextControl);
                    break;
                case "EPICURVE":
                case "SCATTER":
                case "XYCHARTCONTROL":
                    Ewav.ContextMenu.MenuItem mnuSetChartTitle = new ContextMenu.MenuItem("Set Diplay options", "Images/title.png");
                    mnuSetChartTitle.MouseLeftButtonDown += new MouseButtonEventHandler(mnuSetChartTitle_MouseLeftButtonDown);
                    Ewav.ContextMenu.MenuItem munSaveAsImage = new ContextMenu.MenuItem("Save as Image", "Images/save1.png");
                    munSaveAsImage.MouseLeftButtonDown += new MouseButtonEventHandler(munSaveAsImage_MouseLeftButtonDown);
                    mnuSendToBack = new Ewav.ContextMenu.MenuItem("Send gadget toback", "Images/sendback.png");
                    mnuSendToBack.MouseLeftButtonDown += new MouseButtonEventHandler(mnuSendToBack_MouseLeftButtonDown);
                    mnuClose = new Ewav.ContextMenu.MenuItem("Close this gadget", "Images/closegadget.png");
                    mnuClose.MouseLeftButtonDown += new MouseButtonEventHandler(mnuClose_MouseLeftButtonDown);
                    if (SelectedGadget is IChartControl)
                    {
                        if (((IChartControl)SelectedGadget).GetChartTypeEnum() != ClientCommon.XYControlChartTypes.Pie)
                        {
                            popupMenu.Items.Add(mnuSetChartTitle);
                        }
                    }
                    popupMenu.Items.Add(munSaveAsImage);
                    popupMenu.Items.Add(mnuSendToBack);
                    popupMenu.Items.Add(mnuClose);
                    break;
                default:
 
                    Ewav.ContextMenu.MenuItem GadgetSel = new Ewav.ContextMenu.MenuItem("", ""); 
                    GadgetSel.IsEnabled = false;
                    GadgetSel.FontSize = 0.2;
                    GadgetSel.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                    Ewav.ContextMenu.MenuItem Gadget = new Ewav.ContextMenu.MenuItem("Add Gadgets", "Images/gadget.png");
                    Gadget.IsEnabled = !DisplayWithOutDSSelected;
                    Gadget.MouseLeftButtonDown += new MouseButtonEventHandler(MouseLeftButtonDown);
                    Ewav.ContextMenu.MenuItem Chart = new Ewav.ContextMenu.MenuItem("Add Charts", "Images/chart.png");
                    Chart.IsEnabled = !DisplayWithOutDSSelected; ;
                    Chart.MouseLeftButtonDown += new MouseButtonEventHandler(MouseLeftButtonDown);

                    Ewav.ContextMenu.MenuDivider MD = new MenuDivider();
                    Ewav.ContextMenu.MenuItem StatCalc = new Ewav.ContextMenu.MenuItem("Add StatCalc Calculator", "Images/calculator.png");
                    StatCalc.MouseLeftButtonDown += new MouseButtonEventHandler(MouseLeftButtonDown);

                    Ewav.ContextMenu.Menu subMenuGadget = new Ewav.ContextMenu.Menu();
                    Ewav.ContextMenu.Menu subMenuChart = new Ewav.ContextMenu.Menu();
                    Ewav.ContextMenu.Menu subMenuStatCalc = new Ewav.ContextMenu.Menu();

                    List<EwavContextMenuItem> listOfItems = new List<EwavContextMenuItem>();
                    listOfItems = applicationViewModel.MefEwavContextMenuItems;
                    if (applicationViewModel.MefOrderDictionary != null)
                    {
                        listOfItems.Clear();
                        var sortedList = from q in applicationViewModel.MefOrderDictionary orderby q.Key ascending select q;
                        foreach (var item in sortedList)
                        {
                            listOfItems.Add(((EwavContextMenuItem)item.Value));
                        }
                    }

                    foreach (EwavContextMenuItem ewavMenuItem in listOfItems)
                    {
                        Ewav.ContextMenu.MenuItem gadgetItem = new ContextMenu.MenuItem(ewavMenuItem); 
                        gadgetItem.MouseLeftButtonDown += new MouseButtonEventHandler(AddGadget);

                        switch (ewavMenuItem.Type)
                        {
                            case "chart":
                                subMenuChart.Items.Add(gadgetItem);
                                break;
                            case "gadget":
                                subMenuGadget.Items.Add(gadgetItem);
                                break;
                            case "statcalc":
                                subMenuStatCalc.Items.Add(gadgetItem);
                                break;
                            default:
                                break;
                        }
                    }

                    Gadget.Content = subMenuGadget;
                    Chart.Content = subMenuChart;
                    StatCalc.Content = subMenuStatCalc;

                    foreach (Ewav.ContextMenu.MenuItem item in popupMenu.Items)
                    {
                        if (item.Text == "Add Gadgets")
                        {
                            popupMenu.Items.Remove(item);
                        }
                    }

                    popupMenu.Items.Add(GadgetSel);
                    popupMenu.Items.Add(Gadget);
                    popupMenu.Items.Add(Chart);
                    popupMenu.Items.Add(MD);
                    popupMenu.Items.Add(StatCalc);
                    break;
            }

            PopupMenu = popupMenu;

            return popupMenu;
        }

        void mnuStopEdit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectedGadget is TextControl)
            {
                ((TextControl)SelectedGadget).ToggleReadOnly();
            }
        }

        void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        void munSaveAsImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((IChartControl)SelectedGadget).SaveAsImage();
            root.Children.Remove((UIElement)((Ewav.ContextMenu.MenuItem)sender).ParentMenu);
            HidePopuMenu();
        }

        void mnuSetChartTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((IChartControl)SelectedGadget).SetChartLabels();
            root.Children.Remove((UIElement)((Ewav.ContextMenu.MenuItem)sender).ParentMenu);
            HidePopuMenu();
        }

        public void SetChartName()
        {
        }

        void Gadget_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HidePopuMenu();
        }

        #endregion

        void AddGadget(object sender, MouseButtonEventArgs e)
        {
            Ewav.ContextMenu.MenuItem thisMenuItem = sender as Ewav.ContextMenu.MenuItem;

            GadgetManager gm = new GadgetManager();
            UserControl uc = gm.LoadGadget(thisMenuItem.ControlID);

            double mouseHorizontalPosition, mouseVerticalPosition;

            uc.Name = ControlNameGenerator(uc);

            mouseVerticalPosition = contextMenuRightClickCoords.Y - 55;
            mouseHorizontalPosition = contextMenuRightClickCoords.X;

            if (uc is XYChartControl)
            {
                ((XYChartControl)uc).SetChartTitle(((Ewav.ContextMenu.MenuItem)(sender)).Text.Replace(" ", ""));
            }

            cmnClass.AddControlToCanvas(uc, mouseVerticalPosition, mouseHorizontalPosition, applicationViewModel.LayoutRoot);
            MenuPopup(this, new EventArgs());

            root.Children.Remove(popupMenu);
        }

        private string ControlNameGenerator(UIElement element)
        {
            return cmnClass.GenerateControlName(element, DgRoot);
        }

        #region Event Handler
        /// <summary>
        /// Event Handler for Close Gadget Menu Click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mnuClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CloseGadget();
            root.Children.Remove((UIElement)((Ewav.ContextMenu.MenuItem)sender).ParentMenu);
            HidePopuMenu();
        }

        /// <summary>
        /// Event Handler for Send To Back Menu Click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mnuSendToBack_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragCanvas.SetZIndex((UIElement)SelectedGadget, -1);
            root.Children.Remove((UIElement)((Ewav.ContextMenu.MenuItem)sender).ParentMenu);
            HidePopuMenu();
        }

        /// <summary>
        /// Event Handler for Send To HTML Menu Click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mnuSendDataToHTML_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectedGadget.ToHTML(false, "", 0);
            root.Children.Remove((UIElement)((Ewav.ContextMenu.MenuItem)sender).ParentMenu);
            HidePopuMenu();
        }

        /// <summary>
        /// Event Handler for Copy to Clipboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mnuCopy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CopyToClipboard();
            HidePopuMenu();
        }

        /// <summary>
        /// hides popup menu
        /// </summary>
        public void HidePopuMenu()
        {
            root.Children.Remove(popupMenu);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Copies a grid's output to the clipboard
        /// </summary>
        private void CopyToClipboard()
        {
            if (ControlName.ToString().ToLower().Contains("two"))
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(Gadgetparameters.MainVariableName + " by " + Gadgetparameters.CrosstabVariableName);
                sb.AppendLine();
                sb.AppendLine("\t" + "Yes" + "\t" + "No" + "\tTotal");
                sb.AppendLine("Yes" + "\t" + GridCells.YyVal.ToString() + "\t" + GridCells.YnVal.ToString() + "\t" + GridCells.YtVal.ToString());
                sb.AppendLine("No" + "\t" + GridCells.NyVal.ToString() + "\t" + GridCells.NnVal.ToString() + "\t" + GridCells.NtVal.ToString());
                sb.AppendLine("Total\t" + GridCells.YtVal.ToString() + "\t" + GridCells.TnVal.ToString() + "\t" + GridCells.TtVal.ToString());
                sb.AppendLine();

                sb.AppendLine("(Exposure = Rows; Outcome = Columns)");

                try
                {
                    Clipboard.SetText(sb.ToString());
                }
                catch (System.Security.SecurityException)
                {
                    // this exception is thrown when the user declines to give
                    // permission for this web app to write to the clipboard
                }
                catch { }
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (Grid grid in this.strataGridList)
                {
                    string gridName = grid.Tag.ToString();

                    if (strataGridList.Count > 1)
                    {
                        sb.AppendLine(grid.Tag.ToString());
                    }

                    foreach (UIElement control in grid.Children)
                    {
                        if (control is TextBlock)
                        {
                            int columnNumber = Grid.GetColumn((FrameworkElement)control);
                            string value = ((TextBlock)control).Text;

                            sb.Append(value + "\t");

                            if (columnNumber >= grid.ColumnDefinitions.Count - 2)
                            {
                                sb.AppendLine();
                            }
                        }
                    }

                    sb.AppendLine();
                }

                try
                {
                    Clipboard.SetText(sb.ToString());
                }
                catch (System.Security.SecurityException)
                {
                    // this exception is thrown when the user declines to give
                    // permission for this web app to write to the clipboard
                }
                catch { }

            }
        }

        /// <summary>
        /// Closes the gadget
        /// </summary>
        private void CloseGadget()
        {
            SelectedGadget.CloseGadgetOnClick();
            //root.Children.Remove((UIElement)(Root.FindName(ControlName)));
            HidePopuMenu();
        }

        #endregion

        /// <summary>
        /// Updates the Zorder
        /// </summary>
        /// <param name="element"></param>
        /// <param name="bringToFront"></param>
        private void UpdateZOrder(UIElement element, bool bringToFront)
        {
            cmnClass.UpdateZOrder(element, bringToFront, applicationViewModel.LayoutRoot);
        }
    }
}