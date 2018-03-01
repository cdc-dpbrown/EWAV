/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       SetLabelsViewModel.cs
 *  Namespace:  Ewav.ViewModels    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Ewav.Common;

namespace Ewav.ViewModels
{
    public class SetLabelsViewModel
    {
        private string gadgetName = string.Empty;

        /// <summary>
        /// Gets or sets the gadget title.
        /// </summary>
        /// <value>The gadget title.</value>
        public string GadgetTitle { get; set; }

        /// <summary>
        /// Gets or sets the footnote.
        /// </summary>
        /// <value>The footnote.</value>
        public string Footnote { get; set; }

        public string GadgetName
        {
            get { return gadgetName; }
            set { gadgetName = value; }
        }

        private string gadgetDescription = string.Empty;

        public string GadgetDescription
        {
            get { return gadgetDescription; }
            set { gadgetDescription = value; }
        }

        private string colorPallet = Defaults.COLOR_PALETTE;

        public string CollorPallet
        {
            get { return colorPallet; }
            set { colorPallet = value; }
        }

        private bool useDifferentBarColors = Defaults.USE_DIFFERENT_BAR_COLORS;

        public bool UseDifferentBarColors
        {
            get { return useDifferentBarColors; }
            set { useDifferentBarColors = value; }
        }

        private string spacesBetweenBars = Defaults.SPACE_BETWEEN_BARS;

        public string SpacesBetweenBars
        {
            get { return spacesBetweenBars; }
            set { spacesBetweenBars = value; }
        }

        private double width = Defaults.CHART_WIDTH;

        public double Width
        {
            get { return width; }
            set { width = value; }
        }

        private double height = Defaults.CHART_HEGHT;

        public double Height
        {
            get { return height; }
            set { height = value; }
        }

        private bool showLegend = Defaults.SHOW_CHART_LEGEND;

        public bool ShowLegend
        {
            get { return showLegend; }
            set { showLegend = value; }
        }

        private bool showVariableNames = Defaults.SHOW_CHART_VAR_NAMES;

        public bool ShowVariableNames
        {
            get { return showVariableNames; }
            set { showVariableNames = value; }
        }

        private string legendPosition = Defaults.LEGEND_POSITION;

        public string LegendPostion
        {
            get { return legendPosition; }
            set { legendPosition = value; }
        }

        private string chartTitle = string.Empty;

        public string ChartTitle
        {
            get { return chartTitle; }
            set { chartTitle = value; }
        }

        private string xaxisLabel = string.Empty;

        public string XaxisLabel
        {
            get { return xaxisLabel; }
            set { xaxisLabel = value; }
        }

        private string yaxisLabel = string.Empty;

        public string YaxisLabel
        {
            get { return yaxisLabel; }
            set { yaxisLabel = value; }
        }


    }
}