/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       Unmatched.xaml.cs
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
    [ExportMetadata("tabindex", "25")]
    public partial class Unmatched : UserControl, IGadget, IEwavGadget
    {
        public Unmatched()
        {
            InitializeComponent();
            Construct();
            txtPowerU.TextChanged += new TextChangedEventHandler(txtInput_TextChanged);
            txtRatioControlsExposedU.TextChanged += new TextChangedEventHandler(txtInput_TextChanged);
            txtPctControlsExposedU.TextChanged += new TextChangedEventHandler(txtInput_TextChanged);
            txtOddsRatioU.TextChanged += new TextChangedEventHandler(txtInput_TextChanged);
            txtPctCasesWithExposureU.TextChanged += new TextChangedEventHandler(txtInput_TextChanged);
        }

        private void Construct()
        {
            unmatched.Visibility = System.Windows.Visibility.Visible;
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
            get { return "Unmatched"; }
        }

        public string MyUIName
        {
            get { return "Unmatched case-control"; }
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
        void txtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            double percentExposed = 0;
            double.TryParse(txtPctControlsExposedU.Text, out percentExposed);
            percentExposed = percentExposed / 100.0;

            if ((sender == txtOddsRatioU) || (sender == txtPctControlsExposedU))
            {
                bool b = FocusManager.GetFocusedElement() == txtPctCasesWithExposureU;
                if (!b)
                {
                    double oddsRatio = 0;
                    double.TryParse(txtOddsRatioU.Text, out oddsRatio);
                    txtPctCasesWithExposureU.Text = OddsToPercentCases(oddsRatio, percentExposed).ToString("N1");
                }
            }
            else if (sender == txtPctCasesWithExposureU)
            {
                bool b = FocusManager.GetFocusedElement() == txtOddsRatioU;
                if (!b)
                {
                    double percentCasesExposure = 0;
                    double.TryParse(txtPctCasesWithExposureU.Text, out percentCasesExposure);
                    percentCasesExposure = percentCasesExposure / 100.0;
                    txtOddsRatioU.Text = PercentCasesToOdds(percentCasesExposure, percentExposed).ToString();
                }
            }
            Calculate();
        }

        private void CalculatePopulation()
        {
            throw new NotImplementedException();
        }

        private void Calculate()
        {
            string confidenceRaw = ((ComboBoxItem)cbxConfidenceLevelU.SelectedItem).Content.ToString().Split('%')[0];
            double confidence = (100.0 - double.Parse(confidenceRaw, System.Globalization.CultureInfo.InvariantCulture)) / 100.0;
            double power = 0;
            double.TryParse(txtPowerU.Text, out power);

            double controlRatio = 0;
            if (txtRatioControlsExposedU.Text.Length > 0)
            {
                double.TryParse(txtRatioControlsExposedU.Text, out controlRatio);
            }
            double percentExposed = 0;
            double.TryParse(txtPctControlsExposedU.Text, out percentExposed);
            percentExposed = percentExposed / 100.0;

            double oddsRatio = 0;
            if (txtOddsRatioU.Text.Length > 0)
            {
                double.TryParse(txtOddsRatioU.Text, out oddsRatio);
            }
            double percentCasesExposure = 0;
            double.TryParse(txtPctCasesWithExposureU.Text, out percentCasesExposure);
            percentCasesExposure = percentCasesExposure / 100.0;
            Calculate(confidence, power, controlRatio, percentExposed, oddsRatio, percentCasesExposure);
        }

        private void Calculate(double a, double b, double vr, double v2, double vor, double v1)
        {
            //95,80,1.0,40,10,86.96
            //UnmatchedCaseControl(0.05, 80.0, 1.0, 0.4, 10.0, 0.8696);

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

            txtKelseyCasesU.Text = Math.Ceiling(vn).ToString();
            txtKelseyControlsU.Text = Math.Ceiling(vn * vr).ToString();
            txtKelseyTotalU.Text = (Math.Ceiling(vn) + Math.Ceiling(vn * vr)).ToString();

            txtFleissCasesU.Text = Math.Ceiling(vn1).ToString();
            txtFleissControlsU.Text = Math.Ceiling(vn1 * vr).ToString();
            txtFleissTotalU.Text = (Math.Ceiling(vn1) + Math.Ceiling(vn1 * vr)).ToString();

            txtFleissCCCasesU.Text = Math.Ceiling(vn2).ToString();
            txtFleissCCControlsU.Text = Math.Ceiling(vn2 * vr).ToString();
            txtFleissCCTotalU.Text = (Math.Ceiling(vn2) + Math.Ceiling(vn2 * vr)).ToString();
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

        /*static void Main(string[] args)
        {
            //95,80,1.0,40,10,86.96
            UnmatchedCaseControl(0.05, 80.0, 1.0, 0.4, 10.0, 0.8696);
            //95,80,1.0,5,24,55.81,11.16,50.81
            Cohort(0.05, 80.0, 1.0, 0.05, 24.0, 0.5581, 11.16, 0.5081);
            OddsToPercentCases(13, 0.37);
            PercentCasesToOdds(0.8842, 0.37);
            SampleSize(999999, 50, 11);
        }*/

        public double OddsToPercentCases(double oddsRatio, double percentControls)
        {
            double rawVal = 0;
            if (oddsRatio != 0)
            {
                rawVal = 100.0 * percentControls * oddsRatio / (1.0 + percentControls * (oddsRatio - 1.0));
            }
            return Math.Round(100000.0 * rawVal) / 100000.0;
        }

        public double PercentCasesToOdds(double percentCases, double percentControls)
        {
            return Math.Round(100000.0 * (percentCases * (1.0 - percentControls)) / (percentControls * (1.0 - percentCases))) / 100000.0;
        }

        private void cbxConfidenceLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxConfidenceLevelU != null && cbxConfidenceLevelU.SelectedIndex > 0)
            {
                Calculate();
            }

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