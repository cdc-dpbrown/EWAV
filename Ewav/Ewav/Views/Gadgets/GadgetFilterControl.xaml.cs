/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       GadgetFilterControl.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Ewav.Web.Services;

namespace Ewav
{
    public partial class GadgetFilterControl : ChildWindow
    {
        private List<EwavDataFilterCondition> gadgetFilters;

        public List<EwavDataFilterCondition> GadgetFilters
        {
            get { return gadgetFilters; }
            set { gadgetFilters = value; }
        }


        public GadgetFilterControl()
        {
            InitializeComponent();
            FilterCtrl.FilterType = FilterControlType.DataFilter;
            FilterCtrl.InitializeDataFilter();
        }

        public GadgetFilterControl(List<EwavDataFilterCondition> Conditions) 
        {
            InitializeComponent();
            FilterCtrl.FilterType = FilterControlType.DataFilter;
            FilterCtrl.ConstructStackPanelFromDataFilters(Conditions);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            List<EwavDataFilterCondition> DFilters = FilterCtrl.CreateDataFilters();
            if (DFilters == null)
            {
                return;
            }
            GadgetFilters = DFilters;

            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            FilterCtrl.Clear();
            FilterCtrl.CreateFilterConditionRow();

            GadgetFilters = null;

            this.DialogResult = true;
        }
    }
}