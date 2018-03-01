/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       Poisson.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Ewav.ViewModels;

namespace Ewav
{

    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(UserControl))]
    [ExportMetadata("type", "statcalc")]
    [ExportMetadata("tabindex", "22")]
    public partial class Poisson : UserControl, IEwavGadget, IGadget
    {


        public Poisson()
        {
            InitializeComponent();

            txtObserved.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtExpected.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            //imgClose.MouseEnter += new MouseEventHandler(imgClose_MouseEnter);
            //imgClose.MouseLeave += new MouseEventHandler(imgClose_MouseLeave);
            //imgClose.MouseDown += new MouseButtonEventHandler(imgClose_MouseDown);
            //    mnuPrint.Click += new RoutedEventHandler(mnuPrint_Click);


            txtObserved.Text = "6";
            txtExpected.Text = "1.16";
        }
        private void Construct()
        {
            //GadgetContent.Visibility = System.Windows.Visibility.Visible;
        }

        void txtInputs_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtObserved.Text) && !string.IsNullOrEmpty(txtExpected.Text))
            {
                int observed = 0;
                double expected = 0;
                double lessThan = 0;
                double lessThanEqual = 0;
                double equal = 0;
                double greaterThanEqual = 0;
                double greaterThan = 0;

                bool parseResult1 = int.TryParse(txtObserved.Text, out observed);
                bool parseResult2 = double.TryParse(txtExpected.Text, out expected);

                if (parseResult1 && parseResult2)
                {
                    lblLessThan.Text = "< " + observed;
                    lblLessThanEqual.Text = "<= " + observed;
                    lblEqual.Text = "= " + observed;
                    lblGreaterThanEqual.Text = ">= " + observed;
                    lblGreaterThan.Text = "> " + observed;

                    poipdf(observed, expected, ref lessThan, ref lessThanEqual, ref equal, ref greaterThanEqual, ref greaterThan);

                    txtLessThan.Text = lessThan.ToString("F7");
                    txtLessThanEqual.Text = lessThanEqual.ToString("F7");
                    txtEqual.Text = equal.ToString("F7");
                    txtGreaterThanEqual.Text = greaterThanEqual.ToString("F7");
                    txtGreaterThan.Text = greaterThan.ToString("F7");
                }
            }
        }

        #region IGadget Members

        public StringBuilder HtmlBuilder { get; set; }

        public void RefreshResults()
        {

        }

        /// <summary>
        /// Updates the variable names available in the gadget's properties
        /// </summary>
        public void UpdateVariableNames()
        {
            //FillComboboxes(true);
        }

        //public XmlNode Serialize(XmlDocument doc)
        //{
        //    throw new NotImplementedException();
        //}

        //public void CreateFromXml(XmlElement element)
        //{

        //}




        public string CustomOutputHeading { get { return string.Empty; } set { } }
        public string CustomOutputDescription { get { return string.Empty; } set { } }
        public string CustomOutputCaption { get { return string.Empty; } set { } }
        #endregion

        private void LayoutRoot_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClientCommon.Common cmnClass = new ClientCommon.Common();
            Point p = e.GetSafePosition(cmnClass.GetParentObject<Grid>(this, "LayoutRoot"));
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).GadgetNameOnRightClick = this.MyControlName;
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).StrataList = null;
            ((Ewav.DragCanvas)VisualTreeHelper.GetParent(this)).SelectedGadget = this;
            cmnClass.UpdateZOrder(this, true, cmnClass.GetParentObject<Grid>(this, "LayoutRoot"));
        }

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //gadgetContextMenu.Hide();
            //BusyIndicatorGrid.Visibility = System.Windows.Visibility.Collapsed;

        }





        public void poipdf(int x, double lambda, ref double ltp, ref double lep,
            ref double eqp, ref double gep, ref double gtp)
        {
            double denominator = 0;
            ltp = 0.0;
            lep = 0.0;
            eqp = 0.0;
            gep = 0.0;
            gtp = 0.0;

            if (x > 0)
            {
                ltp = Math.Exp(-lambda);
                for (int j = 1; j <= x - 1; j++)
                {
                    denominator = 1;

                    for (int i = 0; i <= j - 1; i++)
                    {
                        denominator = denominator * (j - i);
                    }
                    ltp = ltp + (Math.Pow(lambda, j) * Math.Exp(-lambda)) / denominator;
                }
            }
            lep = Math.Exp(-lambda);
            for (int j = 1; j <= x; j++)
            {
                denominator = 1;
                for (int i = 0; i <= j - 1; i++)
                {
                    denominator = denominator * (j - i);
                }
                lep = lep + (Math.Pow(lambda, j) * Math.Exp(-lambda)) / denominator;
            }
            denominator = 1;
            for (int i = 0; i <= x - 1; i++)
            {
                denominator = denominator * (x - i);
            }
            eqp = (Math.Pow(lambda, x) * Math.Exp(-lambda)) / denominator;
            gep = 1 - ltp;
            gtp = 1 - lep;
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

        public string MyControlName
        {
            get
            {
                return "Poisson";
            }
        }

        public string MyUIName
        {

            get
            {
                return "Poisson";
            }
        }

            public       ViewModels.ApplicationViewModel ApplicationViewModel
        {
            get { throw new NotImplementedException(); }
        }


        public string ToHTML(bool ForDash = false, string htmlFileName = "", int count = 0)
        {
            throw new NotImplementedException();
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

        public int[] CalculateSampleSizes(int pop, double freq, double worst)
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

        //////////
        public int PreferredUIHeight
        {
            get { return 350; }
        }

        public int PreferredUIWidth
        {
            get { return 525; }
        }


        /// <summary>
        /// Sets the gadget to its 'processing' state
        /// </summary>
        public void SetGadgetToProcessingState()
        {
        }

        /// <summary>
        /// Sets the gadget to its 'finished' state
        /// </summary>
        public void SetGadgetToFinishedState()
        {


        }

        public bool DrawBorders { get; set; }


        ////////


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