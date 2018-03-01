/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ContextMenuControl.cs
 *  Namespace:  Ewav.ContextMenu    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    30/05/2012    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Ewav.ContextMenu
{
    #region Delegates

    public delegate void ContextMenuEventHandler(object sender, ContextMenuEventArgs e);

    #endregion

    public abstract partial class ContextMenuControl : ContentControl
    {
        #region Visual Elements

        /// <summary> 
        /// Root element template.
        /// </summary>
        public FrameworkElement ElementRoot { get; set; }
        internal const string ElementRootName = "RootElement";

        /// <summary>
        /// Gets the root Panel object for this control
        /// </summary>
        public Panel VisualRoot
        {
            get { return (Panel)ElementRoot; }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the opacity for certain control elements when disabled
        /// </summary>
        public static double DisabledOpacity { get; set; }

        /// <summary> 
        /// Gets or sets the ID
        /// </summary> 
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the ContextMenuTag
        /// </summary>
        public virtual object ContextMenuTag { get; set; }

        #endregion

        #region Constructor

        static ContextMenuControl()
        {
            DisabledOpacity = 0.7;
        }

        public ContextMenuControl()
        {
            IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnEnabledChanged);
            IsEnabled = true;
            DefaultStyleKey = this.GetType();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Finds the control for a provided name
        /// </summary>
        /// <param name="name">Control name</param>
        /// <returns>Control instance or null if not found</returns>
        public new object FindName(string name)
        {
            return (ElementRoot != null ? ElementRoot.FindName(name) : null);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Is called when the visuals need to be updated
        /// </summary>
        protected virtual void UpdateVisualState()
        {
            if (ElementRoot != null)
            {
                ElementRoot.Width = Width;
                ElementRoot.Height = Height;
            }
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// This is called when the template has been bound to the control
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ElementRoot = (FrameworkElement)GetTemplateChild(ElementRootName);
        }

        /// <summary>
        /// This event is called when the control enabled state changes
        /// </summary>
        /// <param name="sender">Event source object</param>
        /// <param name="e">Event arguments</param>
        private void OnEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            IsTabStop = (bool)e.NewValue;
            UpdateVisualState();
        }

        #endregion
    }

    #region ContextMenuEventArgs
    /// <summary>
    /// Event arguments for use with ContextMenu related events
    /// </summary>
    public partial class ContextMenuEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets whether to cancel an operation
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets or sets a generic tag for the event
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Gets or sets a generic parameter
        /// </summary>
        public object Parameter { get; set; }

        #endregion

        #region Constructor

        public ContextMenuEventArgs()
        {
            Cancel = false;
            Tag = null;
            Parameter = null;
        }

        #endregion
    }

    #endregion
}