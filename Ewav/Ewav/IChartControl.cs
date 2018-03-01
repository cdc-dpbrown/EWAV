/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       IChartControl.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using Ewav.ClientCommon;
using Ewav.ViewModels;

namespace Ewav
{
    /// <summary>
    /// IChartControl
    /// </summary>
    public interface IChartControl
    {
        /// <summary>
        /// Method used to Set the Chart Label
        /// </summary>
        void SetChartLabels();

        /// <summary>
        /// Method that saves the image.
        /// </summary>
        void SaveAsImage();


        XYControlChartTypes GetChartTypeEnum();


    }
}