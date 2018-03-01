/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       Common.cs
 *  Namespace:  Ewav.ClientCommon    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Ewav.BAL;
using Ewav.Web.EpiDashboard;
using Ewav.ViewModels;
using System.Linq;
using Ewav;
using System.Text.RegularExpressions;

namespace Ewav.ClientCommon
{
    /// <summary>
    ///  Enum for the types of charts that can be created with the XYChart control    
    /// </summary>
    public enum XYControlChartTypes
    {
        StackedColumn = 1,
        Line,
        Bar,
        Column,
        Pie,
        Ignore,
        Pareto
    }

    public class Common
    {
        public static event EventHandler GadgetAddedEvent = delegate { };

        public List<string> GroupNames()
        {
            List<string> list = new List<string>();

            
            return list;
        }

        /// <summary>
        /// Generates the standard HTML style.
        /// </summary>
        /// <returns></returns>
        public string GenerateStandardHTMLStyle()
        {
            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.AppendLine("  <style type=\"text/css\">");
            htmlBuilder.AppendLine("            ");
            htmlBuilder.AppendLine("body ");
            htmlBuilder.AppendLine("{");     
            htmlBuilder.AppendLine("	background-color: white;");
            htmlBuilder.AppendLine("	font-family: Calibri, Arial, sans-serif;");
            htmlBuilder.AppendLine("	font-size: 11pt;");
            htmlBuilder.AppendLine("}");
            htmlBuilder.AppendLine("    ");
            htmlBuilder.AppendLine("td ");
            htmlBuilder.AppendLine("{");
            //TBD Starts
            //Need to revisit.
            //if (string.IsNullOrEmpty(CustomOutputTableFontFamily))
            //{
            //    //htmlBuilder.AppendLine("	font-family: Consolas, 'Courier New', monospace;");
            //    htmlBuilder.AppendLine("	font-family: Calibri, Arial, sans-serif;");
            //}
            //else
            //{
            //    htmlBuilder.AppendLine("	font-family: " + CustomOutputTableFontFamily + ", 'Courier New', monospace;");
            //}
            //htmlBuilder.AppendLine("	font-size: " + TableFontSize.ToString() + "px;");
            //TBD Ends
            //htmlBuilder.AppendLine("	font-family: Arial, sans-serif;");
            htmlBuilder.AppendLine("	border-right: 1px solid rgb(23, 54, 93);");
            htmlBuilder.AppendLine("	border-bottom: 1px solid rgb(23, 54, 93);");
            htmlBuilder.AppendLine("	text-align: right;");
            htmlBuilder.AppendLine("	padding-left: 3px;");
            htmlBuilder.AppendLine("	padding-right: 3px;");
            htmlBuilder.AppendLine("	padding-top: 2px;");
            htmlBuilder.AppendLine("	padding-bottom: 2px;");
            htmlBuilder.AppendLine("}");
            htmlBuilder.AppendLine("    ");
            htmlBuilder.AppendLine("td.blank ");
            htmlBuilder.AppendLine("{");
            htmlBuilder.AppendLine("	border-right: 0px solid rgb(23, 54, 93);");
            htmlBuilder.AppendLine("	border-bottom: 0px solid rgb(23, 54, 93);");
            htmlBuilder.AppendLine("}");
            htmlBuilder.AppendLine("    ");
            htmlBuilder.AppendLine("td.noborder ");
            htmlBuilder.AppendLine("{");
            htmlBuilder.AppendLine("	border-right: 0px solid rgb(23, 54, 93);");
            htmlBuilder.AppendLine("	border-bottom: 0px solid rgb(23, 54, 93);");
            htmlBuilder.AppendLine("}");
            htmlBuilder.AppendLine("    ");
            htmlBuilder.AppendLine("td.twobyTwoColoredSquareCell ");
            htmlBuilder.AppendLine("{");
            htmlBuilder.AppendLine("	border-right: 0px solid rgb(23, 54, 93);");
            htmlBuilder.AppendLine("	border-bottom: 0px solid rgb(23, 54, 93);");
            htmlBuilder.AppendLine("	padding-left: 0px;");
            htmlBuilder.AppendLine("	padding-right: 0px;");
            htmlBuilder.AppendLine("	padding-top: 0px;");
            htmlBuilder.AppendLine("	padding-bottom: 0px;");
            htmlBuilder.AppendLine("}");
            htmlBuilder.AppendLine("    ");
            htmlBuilder.AppendLine("td.value ");
            htmlBuilder.AppendLine("{");
            htmlBuilder.AppendLine("	text-align: left;");
            htmlBuilder.AppendLine("}");
            htmlBuilder.AppendLine("    ");

            htmlBuilder.AppendLine("td.highlightedRow ");
            htmlBuilder.AppendLine("{");
            htmlBuilder.AppendLine("	font-weight: bold;");
            htmlBuilder.AppendLine("	background-color: rgb(217, 150, 148);");
            htmlBuilder.AppendLine("}");
            htmlBuilder.AppendLine("    ");

            htmlBuilder.AppendLine("tr.altcolor"); // if alt color is other than white, rows will alternate between white and this color; off by default
            htmlBuilder.AppendLine("{");
            //TBD Starts
            //Need to revisit.
            // if (UseAlternatingColorsInOutput)
            //{
            htmlBuilder.AppendLine("	background-color: #EEEEEE;");
            //}
            // else
            //{
            htmlBuilder.AppendLine("	background-color: rgb(255, 255, 255);");
            //}
            //TBD Ends
            htmlBuilder.AppendLine("}");
            htmlBuilder.AppendLine("    ");
            htmlBuilder.AppendLine("th");
            htmlBuilder.AppendLine("{");
            htmlBuilder.AppendLine("	font-family: Calibri, sans-serif;");
            htmlBuilder.AppendLine("	background-color: #4a7ac9;");
            htmlBuilder.AppendLine("	font-weight: bold;");
            htmlBuilder.AppendLine("	color: white;");
            htmlBuilder.AppendLine("	padding-left: 5px;");
            htmlBuilder.AppendLine("	padding-right: 5px;");
            htmlBuilder.AppendLine("	padding-top: 3px;");
            htmlBuilder.AppendLine("	padding-bottom: 3px;");
            htmlBuilder.AppendLine("	border-right: 1px solid rgb(23, 54, 93);");
            htmlBuilder.AppendLine("	border-bottom: 1px solid rgb(23, 54, 93);");
            htmlBuilder.AppendLine("	min-width: 50px;");
            htmlBuilder.AppendLine("}");
            htmlBuilder.AppendLine("    ");
            htmlBuilder.AppendLine("h1");
            htmlBuilder.AppendLine("{");
            htmlBuilder.AppendLine("	color: #4a7ac9;");
            htmlBuilder.AppendLine("	font-family: Cambria, 'Times New Roman', serif;");
            htmlBuilder.AppendLine("}");
            htmlBuilder.AppendLine("    ");
            htmlBuilder.AppendLine("h2");
            htmlBuilder.AppendLine("{");
            htmlBuilder.AppendLine("	font-family: Cambria, 'Times New Roman', serif;");
            htmlBuilder.AppendLine("}");
            htmlBuilder.AppendLine("h2.gadgetHeading");
            htmlBuilder.AppendLine("{");
            //TBD Starts
            //Need to revisit.
            //if (!ShowGadgetHeadingsInOutput)
            //{
            //    htmlBuilder.AppendLine("	visibility: hidden;");
            //    htmlBuilder.AppendLine("	height: 1px;");
            //}
            //TBD Ends
            htmlBuilder.AppendLine("}");

            htmlBuilder.AppendLine("p.gadgetOptions");
            htmlBuilder.AppendLine("{");
            //TBD Starts
            //Need to revisit.
            //if (!ShowGadgetSettingsInOutput)
            //{
            //    htmlBuilder.AppendLine("	visibility: hidden;");
            //    htmlBuilder.AppendLine("	height: 1px;");
            //}
            //TBD Ends
            htmlBuilder.AppendLine("}");
            htmlBuilder.AppendLine("p.summary");
            htmlBuilder.AppendLine("{");
            htmlBuilder.AppendLine("	margin-left: 20px;");
            htmlBuilder.AppendLine("	margin-right: 20px;");
            htmlBuilder.AppendLine("}");
            htmlBuilder.AppendLine("    ");
            htmlBuilder.AppendLine("p.gadgetSummary");
            htmlBuilder.AppendLine("{");
            htmlBuilder.AppendLine("	font-size: 11pt;");
            htmlBuilder.AppendLine("	font-family: Calibri, Arial, Verdana, sans-serif;");
            htmlBuilder.AppendLine("}");
            htmlBuilder.AppendLine("    ");
            htmlBuilder.AppendLine("table ");
            htmlBuilder.AppendLine("{");
            htmlBuilder.AppendLine("	border-left: 1px solid rgb(23, 54, 93);");
            htmlBuilder.AppendLine("	border-top: 1px solid rgb(23, 54, 93);");
            htmlBuilder.AppendLine("}");
            htmlBuilder.AppendLine("    ");
            htmlBuilder.AppendLine("table.noborder ");
            htmlBuilder.AppendLine("{");
            htmlBuilder.AppendLine("	border-left: 0px solid rgb(23, 54, 93);");
            htmlBuilder.AppendLine("	border-top: 0px solid rgb(23, 54, 93);");
            htmlBuilder.AppendLine("}");
            htmlBuilder.AppendLine("    ");
            htmlBuilder.AppendLine("table.twoByTwoColoredSquares ");
            htmlBuilder.AppendLine("{");
            htmlBuilder.AppendLine("	border-left: 0px solid rgb(23, 54, 93);");
            htmlBuilder.AppendLine("	border-top: 0px solid rgb(23, 54, 93);");
            htmlBuilder.AppendLine("	width: 200px;");
            htmlBuilder.AppendLine("	height: 200px;");
            htmlBuilder.AppendLine("}");
            htmlBuilder.AppendLine("    ");
            htmlBuilder.AppendLine("div.percentBar");
            htmlBuilder.AppendLine("{");
            htmlBuilder.AppendLine("	height: 8px;");
            htmlBuilder.AppendLine("	background-color: rgb(34, 177, 76);");
            htmlBuilder.AppendLine("}");
            htmlBuilder.AppendLine("    ");
            htmlBuilder.AppendLine(".total");
            htmlBuilder.AppendLine("{");
            htmlBuilder.AppendLine("	font-weight: bold;");
            htmlBuilder.AppendLine("}");
            htmlBuilder.AppendLine("    ");
            htmlBuilder.AppendLine(".warning");
            htmlBuilder.AppendLine("{");
            htmlBuilder.AppendLine("	font-weight: bold;");
            htmlBuilder.AppendLine("	color: red;");
            htmlBuilder.AppendLine("}");
            htmlBuilder.AppendLine("    ");
            htmlBuilder.AppendLine(".bold");
            htmlBuilder.AppendLine("{");
            htmlBuilder.AppendLine("	font-weight: bold;");
            htmlBuilder.AppendLine("}");
            htmlBuilder.AppendLine("    ");
            htmlBuilder.AppendLine("caption");
            htmlBuilder.AppendLine("{");
            htmlBuilder.AppendLine("	font-weight: bold;");
            htmlBuilder.AppendLine("}");
            htmlBuilder.AppendLine("    ");
            htmlBuilder.AppendLine("  </style>");

            return htmlBuilder.ToString();
        }

        /// <summary>
        /// Gets the parent object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public T GetParentObject<T>(DependencyObject obj, string name = "") where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);

            while (parent != null)
            {
                if (parent is T && (((T)parent).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)parent;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        /// <summary>
        /// Updates the Z order.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="bringToFront">if set to <c>true</c> [bring to front].</param>
        /// <param name="LayoutRoot">The layout root.</param>
        /// <exception cref="System.ArgumentNullException">element</exception>
        public void UpdateZOrder(UIElement element, bool bringToFront, Grid LayoutRoot)
        {
            List<FrameworkElement> feList = this.GetChildObjects<FrameworkElement>(LayoutRoot); // Makes list of FrameWorkElement class. Which is inherited by Control class.

            #region Safety Check

            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            #endregion

            #region Calculate Z-Indici And Offset

            int elementNewZIndex = -1;
            if (bringToFront)
            {
                foreach (UIElement elem in feList)
                {
                    if (elem.Visibility != Visibility.Collapsed)
                        ++elementNewZIndex;
                }
            }
            else
            {
                elementNewZIndex = 0;
            }

            int offset = (elementNewZIndex == 0) ? +1 : -1;

            int elementCurrentZIndex = Canvas.GetZIndex(element);

            #endregion

            #region Update Z-Indici

            foreach (UIElement childElement in feList)
            {
                if (childElement == element)
                {
                    Canvas.SetZIndex(element, elementNewZIndex);
                }
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

        /// <summary>
        /// GetChildObjects is a method that returns all the Objects on the Canvas.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="name">The name.</param>
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

                childList.AddRange(this.GetChildObjects<T>(child));
            }

            return childList;
        }

        /// <summary>
        /// Generators a ControlName.
        /// </summary>
        /// <param name="ControlName">Name of the control.</param>
        /// <returns></returns>
        public static string ControlNameGenerator(string ControlName)
        {
            if (ControlName.Substring(5) != null)
            {
                for (int i = 0; i < 100; i++)
                {
                    if (ControlName.Substring(5) + i != null)
                    {
                        continue;
                    }
                    else
                    {
                        return ControlName.Substring(5) + i;
                    }
                }
            }
            else
            {
                return ControlName.Substring(5);
            }

            return "";
        }

        /// <summary>
        /// Indicates whether the string contains a whole number
        /// </summary>
        /// <param name="text">String to evaluate.</param>
        /// <returns>Results of test on numeric data.</returns>
        public bool IsWholeNumber(string text)
        {
            #region Input Validation
            if (text == null)
            {
                throw new ArgumentNullException("Text");
            }
            #endregion

            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];
                if (!Char.IsDigit(ch))
                {
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// Method used to locate the current index for selected column.
        /// </summary>
        /// <param name="Column"></param>
        /// <returns></returns>
        public int SearchCurrentIndex(EwavColumn Column, List<EwavColumn> Collection)
        {
            if (Column != null)
            {
                //List<EwavColumn> SourceColumns = this.applicationViewModel.EwavSelectedDatasource.AllColumns;

                //IEnumerable<EwavColumn> CBXFieldCols = from cols in SourceColumns
                //                                       orderby cols.Name
                //                                       select cols;

                List<EwavColumn> colsList = Collection;

                for (int i = 0; i < colsList.Count; i++)
                {
                    if (Column.Name == colsList[i].Name)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Sets the IsInUse indicator for the Column.
        /// </summary>
        //public void IsUserDefindVariableInUse(List<EwavColumn> ColumnsList)
        //{
        //    if (ColumnsList == null)
        //    {
        //        return;
        //    }
        //    for (int i = 0; i < ColumnsList.Count; i++)
        //    {
        //        if (ColumnsList[i] != null && ColumnsList[i].IsUserDefined == true)
        //        {
        //            ColumnsList[i].IsInUse = true;
        //        }

        //    }

        //    //EwavColumn Col1 = (cbxSyndrome.SelectedIndex > -1) ? (EwavColumn)cbxSyndrome.SelectedItem : null;
        //    //EwavColumn Col2 = (cbxFieldWeight.SelectedIndex > -1) ? (EwavColumn)cbxFieldWeight.SelectedItem : null;
        //    //if (Col1 != null && Col1.IsUserDefined == true)
        //    //{
        //    //    Col1.IsInUse = true;
        //    //}
        //    //if (Col2 != null && Col2.IsUserDefined == true)
        //    //{
        //    //    Col2.IsInUse = true;
        //    //}
        //}

        /// <summary>
        /// Gets the ItemsSource
        /// </summary>
        /// <param name="columnDataType"></param>
        /// <returns></returns>
        public List<EwavColumn> GetItemsSource(List<ColumnDataType> columnDataType, bool addBlankColumn = true)
        {
            //throw new NotImplementedException();
            List<EwavColumn> SourceColumns = ApplicationViewModel.Instance.EwavSelectedDatasource.AllColumns;

            IEnumerable<EwavColumn> CBXFieldCols;
            List<EwavColumn> colsList;
            CBXFieldCols = from cols in SourceColumns
                           where columnDataType.Contains(cols.SqlDataTypeAsString)
                           orderby cols.Name
                           select cols;

            colsList = CBXFieldCols.ToList();

            if (addBlankColumn)
            {
                colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1, NoCamelName = " " });
            }


            return colsList;
        }



        /// <summary>
        /// Adds the control to canvas.
        /// </summary>
        /// <param name="uc">The uc.</param>
        /// <param name="mouseVerticalPosition">The mouse vertical position.</param>
        /// <param name="mouseHorizontalPosition">The mouse horizontal position.</param>
        /// <param name="layoutRoot">The layout root.</param>
        internal void AddControlToCanvas(UserControl uc, double mouseVerticalPosition, double mouseHorizontalPosition, Grid layoutRoot)
        {


            ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;



            ((DragCanvas)applicationViewModel.MainCanvas).AddChild(uc, layoutRoot);
            applicationViewModel.Gadgets.Add(uc);
            uc.SetValue(Canvas.TopProperty, mouseVerticalPosition);
            uc.SetValue(Canvas.LeftProperty, mouseHorizontalPosition);
            UpdateZOrder(uc, true, layoutRoot);


            if (uc is IGadget)
            {
                applicationViewModel.GadgetsOnCanvas = true;

                GadgetAddedEvent(this, new EventArgs());
            }


        }

        /// <summary>
        /// Finds the Index to select
        /// </summary>
        /// <param name="itemCollection"></param>
        /// <param name="itemText"></param>
        /// <returns></returns>
        public int FindIndexToSelect(List<EwavColumn> itemCollection, string itemText)
        {
            List<EwavColumn> items = itemCollection.ToList<EwavColumn>();
            for (int i = 0; i < items.Count; i++)
            {
                if (((EwavColumn)items[i]).Name == itemText)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Finds the ewav column.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="columnList">The column list.</param>
        /// <returns></returns>
        internal EwavColumn FindEwavColumn(string p, List<EwavColumn> columnList)
        {

            for (int i = 0; i < columnList.Count; i++)
            {
                if (p == columnList[i].Name)
                {
                    return columnList[i];
                }
            }
            return null;
        }

        internal string GenerateControlName(UIElement element, DragCanvas DgRoot)
        {
            if (DgRoot.FindName(element.ToString().Substring(5)) != null)
            {
                for (int i = 0; i < 100; i++)
                {
                    if (DgRoot.FindName(element.ToString().Substring(5) + i) != null)
                    {
                        continue;
                    }
                    else
                    {
                        return element.ToString().Substring(5) + i;
                    }
                }
            }
            else
            {
                return element.ToString().Substring(5);
            }

            return "";
        }
    }
}