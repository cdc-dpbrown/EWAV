/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ContextMenuTimerControl.cs
 *  Namespace:  Ewav.ContextMenu      
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    30/05/2012    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Windows.Media.Animation;

namespace Ewav.ContextMenu  
{
    public abstract partial class ContextMenuTimerControl : ContextMenuControl
    {
        #region Private Properties

        private Storyboard _timer = new Storyboard();

        #endregion

        #region Event Handling

        /// <summary>
        /// This is called when the template has been bound to the control
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _timer.Duration = TimeSpan.FromMilliseconds(20);
            _timer.Completed += new EventHandler(Tick);
            _timer.Begin();
        }

        /// <summary>
        /// This event is called periodically
        /// </summary>
        /// <param name="sender">Event source object</param>
        /// <param name="e">Event arguments</param>
        protected virtual void Tick(object sender, EventArgs e)
        {
            _timer.Begin();
        }

        #endregion
    }
}