/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       PopulationSurvey.xaml.cs
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
using Ewav.ContextMenu;
using System.ComponentModel.Composition;
using System.Text;
using Ewav.ExtensionMethods;
namespace Ewav
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(UserControl))]
    [ExportMetadata("type", "statcalc")]
    [ExportMetadata("tabindex", "27")]
    public partial class PopulationSurvey : UserControl, IGadget, IEwavGadget
    {
        public PopulationSurvey()
        {
            InitializeComponent();
            Construct();
            // gadgetMenu.Click += new MouseButtonEventHandler(expandEffect_MouseLeftButtonDown);
            txtPopulationSize.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtExpectedFreq.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtConfidenceLimits.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtDesignEffect.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtNumberOfClusters.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);

            txtPopulationSize.Text = "999999";
            txtExpectedFreq.Text = "50";
            txtConfidenceLimits.Text = "5";
            txtDesignEffect.Text = "1.0";
            txtNumberOfClusters.Text = "1";
        }

        private void Construct()
        {
            popSurvey.Visibility = System.Windows.Visibility.Visible;
        }

        private void LayoutRoot_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClientCommon.Common cmnClass = new ClientCommon.Common();
            Point p = e.GetSafePosition(cmnClass.GetParentObject<Grid>(this, "LayoutRoot"));
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).GadgetNameOnRightClick = MyControlName;
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).StrataList = null;
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).SelectedGadget = this;
            cmnClass.UpdateZOrder(this, true, cmnClass.GetParentObject<Grid>(this, "LayoutRoot"));
        }

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //gadgetContextMenu.Hide();
            //BusyIndicatorGrid.Visibility = System.Windows.Visibility.Collapsed;

        }

        private void EditProperties_Click(object sender, RoutedEventArgs e)
        {
            //GadgetContent.Visibility = System.Windows.Visibility.Visible;
        }

        private void CloseGadget_Click(object sender, RoutedEventArgs e)
        {
            CloseGadgetOnClick();
        }

        private void ResizeButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            if (GadgetContentGrid.Visibility == System.Windows.Visibility.Visible)
            {
                GadgetContentGrid.Visibility = System.Windows.Visibility.Collapsed;
                ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn2"];
            }
            else
            {
                GadgetContentGrid.Visibility = System.Windows.Visibility.Visible;
                ResizeButton.Template = (ControlTemplate)Application.Current.Resources["resizebtn"];
            }
        }

        public StringBuilder HtmlBuilder { get; set; }

        public void RefreshResults()
        {
            throw new NotImplementedException();
        }

        public string ToHTML(bool ForDash = false, string htmlFileName = "", int count = 0)
        {
            throw new NotImplementedException();
        }

        public bool IsProcessing
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void SetGadgetToProcessingState()
        {
            throw new NotImplementedException();
        }

        public void SetGadgetToFinishedState()
        {
            throw new NotImplementedException();
        }

        public void UpdateVariableNames()
        {
            throw new NotImplementedException();
        }

        public string CustomOutputHeading
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string CustomOutputDescription
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string CustomOutputCaption
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void CloseGadget()
        {
            DragCanvas dc = this.Parent as DragCanvas;

            Canvas parentCanvas = (Canvas)this.Parent;
            parentCanvas.Children.Remove((UIElement)this);

            dc.Cleanup(this as UserControl);
        }

        /// <summary>
        /// Closes the gadget after confirmation.
        /// </summary>
        public void CloseGadgetOnClick()
        {
            CloseGadget confirm = new CloseGadget(this);
            confirm.Show();
        }

        public string MyControlName
        {
            get { return "population"; }
        }

        public string MyUIName
        {
            get { return "Population survey"; }
        }

        public ViewModels.ApplicationViewModel ApplicationViewModel
        {
            get { throw new NotImplementedException(); }
        }

        //private void btnCalculate_Click(object sender, RoutedEventArgs e)
        //{
        //    string calculatorToShow = "";
        //    if (popSurvey.Visibility == System.Windows.Visibility.Visible)
        //    {
        //        CalculatePopulation();
        //    }
        //    else
        //    if(cohort.Visibility == System.Windows.Visibility.Visible){
        //        //CalculateCohort();
        //    }
        //    else
        //    {
        //        CalculateUnmatched();
        //    }

        //}

        private void CalculateUnmatched()
        {
            throw new NotImplementedException();
        }

        //private void CalculateCohort()
        void txtInputs_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPopulationSize.Text) && !string.IsNullOrEmpty(txtExpectedFreq.Text) && !string.IsNullOrEmpty(txtConfidenceLimits.Text) && !string.IsNullOrEmpty(txtDesignEffect.Text) && !string.IsNullOrEmpty(txtNumberOfClusters.Text))
            {
                int populationSize = 0;
                double expectedFreq = 0;
                double confidenceLimits = 0;
                double de = 1.0;
                int clusters = 1;
                bool parseResult1 = int.TryParse(txtPopulationSize.Text, out populationSize);
                bool parseResult2 = double.TryParse(txtExpectedFreq.Text, out expectedFreq);
                bool parseResult3 = double.TryParse(txtConfidenceLimits.Text, out confidenceLimits);
                bool parseResult4 = double.TryParse(txtDesignEffect.Text, out de);
                bool parseResult5 = int.TryParse(txtNumberOfClusters.Text, out clusters);

                if (parseResult1 && parseResult2 && parseResult3 && parseResult4 && parseResult5)
                {
                    int[] res = CalculateSampleSizes(populationSize, expectedFreq, confidenceLimits, de, clusters);
                    txt80.Text = res[0].ToString();
                    txt90.Text = res[1].ToString();
                    txt95.Text = res[2].ToString();
                    txt97.Text = res[3].ToString();
                    txt99.Text = res[4].ToString();
                    txt999.Text = res[5].ToString();
                    txt9999.Text = res[6].ToString();
                    ttxt80.Text = (clusters * res[0]).ToString();
                    ttxt90.Text = (clusters * res[1]).ToString();
                    ttxt95.Text = (clusters * res[2]).ToString();
                    ttxt97.Text = (clusters * res[3]).ToString();
                    ttxt99.Text = (clusters * res[4]).ToString();
                    ttxt999.Text = (clusters * res[5]).ToString();
                    ttxt9999.Text = (clusters * res[6]).ToString();
                }
            }
        }

        private void CalculatePopulation()
        {
            throw new NotImplementedException();
        }

        public int[] CalculateSampleSizes(int pop, double freq, double worst, double de, int clusters)
        {
            double[] percentiles = new double[] { 0.80, 0.90, 0.95, 0.97, 0.99, 0.999, 0.9999 };
            int[] sizes = new int[7];
            //double d = Math.abs(freq - worst);
            double d = Math.Abs(worst);
            double factor = freq * (100 - freq) / (d * d);
            for (int i = 0; i < percentiles.Length; i++)
            {
                double twoTail = ANorm(1 - percentiles[i]);
                double n = twoTail * twoTail * factor;
                double sampleSize = n / (1 + (n / pop));
                sizes[i] = (int)Math.Round(sampleSize);
                sizes[i] = (int)Math.Ceiling(de * (double)sizes[i] / (double)clusters);
            }
            return sizes;
        }

        public void UnmatchedCaseControl(double a, double b, double vr, double v2, double vor, double v1)
        {
            double Za = ANorm(a);
            double Zb = 0;

            if (b >= 1)
            {
                b = b / 100.0;
            }
            if (b < 0.5)
            {
                Zb = -ANorm(2.0 * b);
            }
            else
            {
                Zb = ANorm(2.0 - 2.0 * b);
            }
            if (vor != 0)
            {
                v1 = v2 * vor / (1.0 + v2 * (vor - 1.0));
            }
            double pbar = (v1 + vr * v2) / (1.0 + vr);
            double qbar = 1.0 - pbar;
            double vn = ((Math.Pow((Za + Zb), 2.0)) * pbar * qbar * (vr + 1.0)) / ((Math.Pow((v1 - v2), 2.0)) * vr);
            double vn1 = Math.Pow(((Za * Math.Sqrt((vr + 1.0) * pbar * qbar)) + (Zb * Math.Sqrt((vr * v1 * (1.0 - v1)) + (v2 * (1.0 - v2))))), 2.0) / (vr * Math.Pow((v2 - v1), 2.0));
            double vn2 = Math.Pow(Za * Math.Sqrt((vr + 1.0) * pbar * qbar) + Zb * Math.Sqrt(vr * v1 * (1.0 - v1) + v2 * (1.0 - v2)), 2.0) / (vr * Math.Pow(Math.Abs(v1 - v2), 2.0));
            vn2 = vn2 * Math.Pow((1.0 + Math.Sqrt(1.0 + 2.0 * (vr + 1.0) / (vn2 * vr * Math.Abs(v2 - v1)))), 2.0) / 4.0;
        }

        public void Cohort(double a, double b, double vr, double v2, double vor, double v1, double rr, double dd)
        {
            double Za = ANorm(a);
            if (b >= 1)
            {
                b = b / 100.0;
            }
            double Zb;
            if (b < 0.5)
            {
                Zb = -ANorm(2.0 * b);
            }
            else
            {
                Zb = ANorm(2.0 - (2.0 * b));
            }
            if (vor != 0)
            {
                v1 = v2 * vor / (1.0 + v2 * (vor - 1.0));
            }
            double pbar = (v1 + vr * v2) / (1.0 + vr);
            double qbar = 1.0 - pbar;
            double vn = ((Math.Pow((Za + Zb), 2.0)) * pbar * qbar * (vr + 1.0)) / ((Math.Pow((v1 - v2), 2.0)) * vr);
            double vn1 = Math.Pow(((Za * Math.Sqrt((vr + 1.0) * pbar * qbar)) + (Zb * Math.Sqrt((vr * v1 * (1.0 - v1)) + (v2 * (1.0 - v2))))), 2.0) / (vr * Math.Pow((v2 - v1), 2.0));
            double vn2 = Math.Pow(Za * Math.Sqrt((vr + 1.0) * pbar * qbar) + Zb * Math.Sqrt(vr * v1 * (1.0 - v1) + v2 * (1.0 - v2)), 2.0) / (vr * Math.Pow(Math.Abs(v1 - v2), 2.0));
            vn2 = vn2 * Math.Pow((1.0 + Math.Sqrt(1.0 + 2.0 * (vr + 1.0) / (vn2 * vr * Math.Abs(v2 - v1)))), 2.0) / 4.0;
        }

        private double ANorm(double p)
        {
            double v = 0.5;
            double dv = 0.5;
            double z = 0;

            while (dv > 1e-6)
            {
                z = 1.0 / v - 1.0;
                dv = dv / 2.0;
                if (Norm(z) > p)
                {
                    v = v - dv;
                }
                else
                {
                    v = v + dv;
                }
            }

            return z;
        }

        private double Norm(double z)
        {
            z = Math.Sqrt(z * z);
            double p = 1.0 + z * (0.04986735 + z * (0.02114101 + z * (0.00327763 + z * (0.0000380036 + z * (0.0000488906 + z * 0.000005383)))));
            p = p * p; p = p * p; p = p * p;
            return 1.0 / (p * p);
        }

        private double OddsToPercentCases(double oddsRatio, double percentControls)
        {
            double rawVal = 0;
            if (oddsRatio != 0)
            {
                rawVal = 100 * percentControls * oddsRatio / (1 + percentControls * (oddsRatio - 1));
            }
            return Math.Round(100000 * rawVal) / 100000;
        }

        private double PercentCasesToOdds(double percentCases, double percentControls)
        {
            return Math.Round(100000 * (percentCases * (1 - percentControls)) / (percentControls * (1 - percentCases))) / 100000;
        }



        public System.Xml.Linq.XNode Serialize(System.Xml.Linq.XDocument doc)
        {
            //throw new NotImplementedException();
            return null;
        }


        public void CreateFromXml(System.Xml.Linq.XElement element)
        {
            //throw new NotImplementedException();
        }


        public void Reload()
        {
            throw new NotImplementedException();
        }


        public List<Web.Services.EwavDataFilterCondition> GadgetFilters
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}