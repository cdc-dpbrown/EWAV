/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       Defaults.cs
 *  Namespace:  Ewav.Common    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;

namespace Ewav.Common
{
    public class Defaults
    {
        public const double CANVAS_HEIGHT = 8000;
        public const double CANVAS_WIDTH = 4000;

        public const double CHART_HEGHT = 400;
        public const double CHART_WIDTH = 800;

        public const double MIN_CHART_WIDTH = 400;
        public const double MAX_CHART_WIDTH = 2000;

        public const string THEME = "ArcticWhite";
        public const string COLOR_PALETTE = "VibrantA";
        public const string LEGEND_POSITION = "right";

        public const bool SHOW_CHART_LEGEND = false;
        public const bool SHOW_CHART_VAR_NAMES = false;
        public const string SPACE_BETWEEN_BARS = "0.1";
        public const bool USE_DIFFERENT_BAR_COLORS = false;
    }
}