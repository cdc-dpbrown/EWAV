/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ChartSeriesDataValue.cs
 *  Namespace:  Ewav.Common    
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
using System.Collections.Generic;
using System.Globalization;

namespace Ewav.Common
{
    public class ChartDataValue
    {
        public string DependentValue { get; set; }
        public string independentValue;
        public string currentMeanValue;
        private string format;

        public string Format
        {
            get { return format; }
            set { format = value; }
        }

        /// <summary>
        /// Gets or sets the current mean value.
        /// </summary>
        /// <value>The current mean value.</value>
        public string CurrentMeanValue
        {
            get
            {
                return this.currentMeanValue;
            }
            set
            {
                this.currentMeanValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the independent value.
        /// </summary>
        /// <value>The independent value.</value>
        public string IndependentValue
        {
            get
            {
                //  If it is can converted to a date do it     
                string dateString = independentValue;
                var culture = CultureInfo.CurrentCulture;
                var styles = DateTimeStyles.None;
                DateTime dateResult;
                Double doubleresult;
                Boolean boolResult;
                // bool success = DateTime.TryParseExact(
                //    dateString,
                //    "MM/dd/yyyy HH:mm:ss tt",
                //    culture,
                //    styles,
                //    out dateResult
                //);
                if (Double.TryParse(independentValue, out doubleresult))
                {
                    return independentValue;
                }
                else if (Boolean.TryParse(independentValue, out boolResult))
                {
                    return independentValue;
                }
                else if (DateTime.TryParse(dateString, culture, styles, out dateResult))
                // if (success)
                //return dateResult.ToShortDateString();
                {
                    if (dateResult != null)
                    {
                        if (Format.ToLower().Equals("hours"))
                        {
                            return ((DateTime)dateResult).ToString("g");
                        }
                        else if (Format.ToLower().Equals("months"))
                        {
                            return ((DateTime)dateResult).ToString("MMM yyyy");
                        }
                        else if (Format.ToLower().Equals("years"))
                        {
                            return ((DateTime)dateResult).ToString("yyyy");
                        }
                        else
                        {
                            return ((DateTime)dateResult).ToShortDateString();
                        }
                    }
                }
                return independentValue;

            }
            set
            {
                this.independentValue = value;
            }
        }

    }

    public class ChartSeries
    {
        private string seriesName;
        private List<ChartDataValue> dataValues;

        /// <summary>
        /// Gets or sets the data values.
        /// </summary>
        /// <value>The data values.</value>
        public List<ChartDataValue> DataValues
        {
            get
            {
                return this.dataValues;
            }
            set
            {
                this.dataValues = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the series.
        /// </summary>
        /// <value>The name of the series.</value>
        public string SeriesName
        {
            get
            {
                return this.seriesName;
            }
            set
            {
                this.seriesName = value;
            }
        }
    }
}