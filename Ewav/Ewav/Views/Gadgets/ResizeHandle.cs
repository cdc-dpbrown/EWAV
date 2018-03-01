/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ResizeHandle.cs
 *  Namespace:   Ewav    
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
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Collections.Generic;

namespace  Ewav
{
  public class ResizeHandle
  {
    public static bool GetAttach(DependencyObject obj)
    {
      return (bool)obj.GetValue(AttachProperty);
    }

    public static void SetAttach(DependencyObject obj, bool value)
    {
      obj.SetValue(AttachProperty, value);
    }

    public static readonly DependencyProperty AttachProperty =
        DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(ResizeHandle),
        new PropertyMetadata(false, OnAttachedPropertyChanged));

    private static Dictionary<FrameworkElement, Popup> _popups = new Dictionary<FrameworkElement, Popup>();

    private static void OnAttachedPropertyChanged(DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      FrameworkElement element = d as FrameworkElement;

      // Create a popup.
      var popup = new Popup();

      // and associate it with the element that can be re-sized
      _popups[element] = popup;

      // Add a thumb to the pop-up
      Thumb thumb = new Thumb()
      {
        Style = Application.Current.Resources["MyThumbStyle"] as Style
      };
      popup.Child = thumb;

      // add a relationship from the thumb to the target
      thumb.Tag = element;

      popup.IsOpen = true;

      thumb.DragDelta += new DragDeltaEventHandler(Thumb_DragDelta);
      element.SizeChanged += new SizeChangedEventHandler(Element_SizeChanged);  
        
    }

    /// <summary>
    /// Handles drag events from the thumb, resizing the associated element
    /// </summary>
    private static void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
      Thumb thumb = sender as Thumb;
      var fe = thumb.Tag as FrameworkElement;

      // set the Width / Height of the element
      if (fe.Width == 0.0 || double.IsNaN(fe.Width))
      {
        fe.Width = fe.ActualWidth;
      }
      if (fe.Height == 0.0 || double.IsNaN(fe.Height))
      {
        fe.Height = fe.ActualHeight;
      }

      // apply the drag deltas
      double newWidth = fe.Width + e.HorizontalChange;
      fe.Width = Math.Max(0, newWidth);

      double newHeight = fe.Height + e.VerticalChange;
      fe.Height = Math.Max(0, newHeight);
    }

    /// <summary>
    /// Handle size changes repositioning the thumb / popup
    /// </summary>
    private static void Element_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      FrameworkElement element = sender as FrameworkElement;

      // find the popup that hosts the thumb
      var popup = _popups[element];

      // reposition the popup
      FrameworkElement root = Application.Current.RootVisual as FrameworkElement;
      Point elementLocation = GetRelativePosition(element, root);

      var childElement = popup.Child as FrameworkElement;

      // Set where the popup will show up on the screen.
      popup.VerticalOffset = elementLocation.Y + element.ActualHeight - childElement.ActualHeight;
      popup.HorizontalOffset = elementLocation.X + element.ActualWidth - childElement.ActualWidth;

    }

    public static Point GetRelativePosition(UIElement element, UIElement otherElement)
    {
      return element.TransformToVisual(otherElement)
                   .Transform(new Point(0, 0));
    }

  }
}