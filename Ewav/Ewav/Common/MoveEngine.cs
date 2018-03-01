/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MoveEngine.cs
 *  Namespace:  Ewav.Common    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.Windows;
using System.Windows.Controls;

namespace Ewav.Common
{
    public class MoveEngine
    {

        private Ewav.ViewModels.ApplicationViewModel applicationViewModel = Ewav.ViewModels.ApplicationViewModel.Instance;
        private DragCanvas parentCanvas;


        public void MoveElementRight(UIElement element)
        {

        }

        public void MoveAllElements()
        {
            foreach (UIElement thisElement in parentCanvas.Children    )  
            {
                if (thisElement is UserControl)
                {
                    UserControl uc = thisElement as UserControl;

                    if (uc is IEwavGadget)
                    {
                        if (applicationViewModel.LoadedGadgets.Contains(uc.Name))
                        {
                            uc.SetValue(Canvas.LeftProperty, 44);
                            uc.SetValue(Canvas.TopProperty, 77);


                        }
                    }
                }
            }
        }

    }


}