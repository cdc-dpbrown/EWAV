/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ICustomizableGadget.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.Windows;
using Ewav.ViewModels;

namespace Ewav
{
    public interface ICustomizableGadget
    {
  

    
        /// <summary>
        /// Gets or sets the set labels popup.
        /// </summary>
        /// <value>The set labels popup.</value>
        SetLabels setLabelsPopup { get; set; }
        

        
                /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        /// <value>The view model.</value>
        SetLabelsViewModel viewModel   { get; set; }

        
        void HeaderButton_Click(object sender, RoutedEventArgs e);

        void window_Unloaded(object sender, RoutedEventArgs e);

        void SetChartLabels();

        /// <summary>
        /// Sets the header and footer.
        /// </summary>
        void SetHeaderAndFooter();

        void LoadViewModel();
    }
}