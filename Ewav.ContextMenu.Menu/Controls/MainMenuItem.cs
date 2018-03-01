/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MainMenuItem.cs
 *  Namespace:  Ewav.ContextMenu    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    30/05/2012    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ewav.ContextMenu
{
    /// <summary>
    /// A Main Menu Item control for use with the MainMenu control
    /// </summary>
    public partial class MainMenuItem : MenuItem
    {
        #region Dependency Properties

        /// <summary>
        /// Gets or sets whether child menus show on mouse hover
        /// </summary>
        public static readonly DependencyProperty ShowChildMenuOnHoverProperty = DependencyProperty.Register("ShowChildMenuOnHover", typeof(bool), typeof(MainMenuItem), null);
        public bool ShowChildMenuOnHover
        {
            get { return (bool)this.GetValue(ShowChildMenuOnHoverProperty); }
            set { base.SetValue(ShowChildMenuOnHoverProperty, value); }
        }

        #endregion

        #region Constructor

        public MainMenuItem()
        {
        }

        #endregion

        #region Event Handling

        protected override void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (IsEnabled)
            {
                ParentMenu.SetHilightChild(this);

                if (ParentMenu.ShowChild)
                {
                    ParentMenu.OpenChild(this, true);
                }
                else if (ShowChildMenuOnHover)
                {
                    ParentMenu.Focus();
                    ParentMenu.OpenChild(this, true);
                }
            }
        }

        protected override void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsEnabled)
            {
                ParentMenu.Focus();
                ParentMenu.ShowChild ^= true;
                if (ParentMenu.ShowChild)
                {
                    ParentMenu.OpenChild(this, true);
                }
                else
                {
                    ParentMenu.CloseChild();
                }
            }
        }

        #endregion
    }
}