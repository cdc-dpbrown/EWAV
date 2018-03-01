/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ChiSquare.xaml.cs
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
using Ewav.Common;

namespace Ewav
{

    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(UserControl))]
    [ExportMetadata("type", "statcalc")]
    [ExportMetadata("tabindex", "22")]
    public partial class ChiSquare : UserControl, IEwavGadget, IGadget
    {
        /// <summary>
        /// The value for the frameworkelement.Name property
        /// </summary>
        /// <value></value>
        public string MyControlName
        {
            get
            {
                return "ChiSquare";
            }
        }

        /// <summary>
        /// The value for the UI menus
        /// </summary>
        /// <value></value>
        public string MyUIName
        {
            get
            {
                return "Chi Square for trend";
            }
        }

        /// <summary>
        /// Gets the application view model.
        /// </summary>
        /// <value>The application view model.</value>
        public ApplicationViewModel ApplicationViewModel
        {
            get
            {
                // TODO: Implement this property getter
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Closes the gadget.
        /// </summary>
        public void CloseGadget()
        {

            Canvas parentCanvas = (Canvas)this.Parent;

            DragCanvas dc = this.Parent as DragCanvas;
            dc.Cleanup(this as UserControl);

            parentCanvas.Children.Remove((UIElement)this);

            cEventHelper eh = new cEventHelper();
            eh.RemoveAllEventHandlers(this);


        }

        /// <summary>
        /// Closes the gadget after confirmation.
        /// </summary>
        public void CloseGadgetOnClick()
        {
            CloseGadget confirm = new CloseGadget(this);
            confirm.Show();
        }

        private struct Row
        {
            public TextBox Score;
            public TextBox Cases;
            public TextBox Controls;
            public TextBlock OddsRatio;
        }

        private List<Row> rows;
        public ChiSquare()
        {
            InitializeComponent();
            Construct();
            rows = new List<Row>();
            for (int x = 0; x < 5; x++)
            {
                CreateRow();
            }
            btnAddRow.Click += new RoutedEventHandler(btnAddRow_Click);
            // scrlViewer.ScrollChanged += new ScrollChangedEventHandler(scrlViewer_ScrollChanged);
        }


        private void Construct()
        {
            //GadgetContent.Visibility = System.Windows.Visibility.Visible;
        }

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

        //void scrlViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        //{
        //    if (e.ExtentHeight > 160)
        //    {
        //        grdMain.Margin = new Thickness(17, 0, 0, 0);
        //    }
        //}
        void btnAddRow_Click(object sender, RoutedEventArgs e)
        {
            CreateRow();
            scrlViewer.ScrollToBottom();
        }


        void txtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            int validRows = 0;

            foreach (Row row in rows)
            {
                row.OddsRatio.Text = string.Empty;
            }
            foreach (Row row in rows)
            {
                double scoreTest;
                double casesTest;
                double controlsTest;
                if (string.IsNullOrEmpty(row.Score.Text) || string.IsNullOrEmpty(row.Cases.Text) || string.IsNullOrEmpty(row.Controls.Text) || !double.TryParse(row.Score.Text, out scoreTest) || !double.TryParse(row.Cases.Text, out casesTest) || !double.TryParse(row.Controls.Text, out controlsTest))
                    break;
                validRows++;
            }
            if (validRows > 0)
            {
                double[] scores = new double[validRows];
                double[] cases = new double[validRows];
                double[] controls = new double[validRows];
                for (int x = 0; x < validRows; x++)
                {
                    scores[x] = double.Parse(rows[x].Score.Text);
                    cases[x] = double.Parse(rows[x].Cases.Text);
                    controls[x] = double.Parse(rows[x].Controls.Text);
                }
                ChiSquareResult result = GetChiSquareForTrend(scores, cases, controls);
                for (int x = 0; x < result.GetOddsRatios().Length; x++)
                {
                    rows[x].OddsRatio.Text = result.GetOddsRatios()[x].ToString("N3");
                }
                txtChiSquare.Text = result.GetChi() == 0 ? "0" : result.GetChi().ToString("N5");
                txtPValue.Text = result.GetPValue() == 0 ? "0" : result.GetPValue().ToString("N5");
            }
            else
            {
                txtChiSquare.Text = "...";
                txtPValue.Text = "...";
            }
        }

        private void CreateRow()
        {
            Rectangle rct1 = new Rectangle();
            rct1.Style = Application.Current.Resources["DataCell"] as Style;
            Grid.SetColumn(rct1, 0);
            Grid.SetRow(rct1, grdMain.RowDefinitions.Count);

            Rectangle rct2 = new Rectangle();
            rct2.Style = Application.Current.Resources["DataCell"] as Style;
            Grid.SetColumn(rct2, 1);
            Grid.SetRow(rct2, grdMain.RowDefinitions.Count);

            Rectangle rct3 = new Rectangle();
            rct3.Style = Application.Current.Resources["DataCell"] as Style;
            Grid.SetColumn(rct3, 2);
            Grid.SetRow(rct3, grdMain.RowDefinitions.Count);

            Rectangle rct4 = new Rectangle();
            rct4.Style = Application.Current.Resources["DataCell"] as Style;
            Grid.SetColumn(rct4, 3);
            Grid.SetRow(rct4, grdMain.RowDefinitions.Count);

            TextBox txtScore = new TextBox();
            txtScore.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            txtScore.Margin = new Thickness(1);
            txtScore.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            txtScore.TextAlignment = TextAlignment.Right;
            txtScore.Height = 30;
            txtScore.FontFamily = new FontFamily("Microsoft Sans Serif");
            txtScore.FontSize = 20;
            txtScore.TextChanged += new TextChangedEventHandler(txtInput_TextChanged);
            Grid.SetColumn(txtScore, 0);
            Grid.SetRow(txtScore, grdMain.RowDefinitions.Count);

            TextBox txtCases = new TextBox();
            txtCases.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            txtCases.Margin = new Thickness(1);
            txtCases.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            txtCases.TextAlignment = TextAlignment.Right;
            txtCases.Height = 30;
            txtCases.FontFamily = new FontFamily("Microsoft Sans Serif");
            txtCases.FontSize = 20;
            txtCases.TextChanged += new TextChangedEventHandler(txtInput_TextChanged);
            Grid.SetColumn(txtCases, 1);
            Grid.SetRow(txtCases, grdMain.RowDefinitions.Count);

            TextBox txtControls = new TextBox();
            txtControls.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            txtControls.Margin = new Thickness(1);
            txtControls.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            txtControls.TextAlignment = TextAlignment.Right;
            txtControls.Height = 30;
            txtControls.FontFamily = new FontFamily("Microsoft Sans Serif");
            txtControls.FontSize = 20;
            txtControls.TextChanged += new TextChangedEventHandler(txtInput_TextChanged);
            Grid.SetColumn(txtControls, 2);
            Grid.SetRow(txtControls, grdMain.RowDefinitions.Count);

            TextBlock txtOddsRatio = new TextBlock();
            txtOddsRatio.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            txtOddsRatio.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            txtOddsRatio.FontFamily = new FontFamily("Microsoft Sans Serif");
            txtOddsRatio.FontSize = 20;
            Grid.SetColumn(txtOddsRatio, 3);
            Grid.SetRow(txtOddsRatio, grdMain.RowDefinitions.Count);

            grdMain.RowDefinitions.Add(new RowDefinition());
            grdMain.Children.Add(rct1);
            grdMain.Children.Add(rct2);
            grdMain.Children.Add(rct3);
            grdMain.Children.Add(rct4);
            grdMain.Children.Add(txtScore);
            grdMain.Children.Add(txtCases);
            grdMain.Children.Add(txtControls);
            grdMain.Children.Add(txtOddsRatio);

            rows.Add(new Row() { Score = txtScore, Cases = txtCases, Controls = txtControls, OddsRatio = txtOddsRatio });
        }


        public double calcP(double q, double df)
        {
            double tk = 0;
            double CFL = 0;
            double CFU = 0;
            double prob = 0;
            double df2 = df / 2.0;
            double q2 = q / 2.0;
            int nn = 5;
            if (q <= 0 || df <= 0) return -1;
            if (q < df)
            {
                tk = q2 * (1 - nn - df2) / (df2 + 2 * nn - 1 + nn * q2 / (df2 + 2 * nn));
                for (int kk = nn - 1; kk > 1; kk--)
                    tk = q2 * (1 - kk - df2) / (df2 + 2 * kk - 1 + kk * q2 / (df2 + 2 * kk + tk));
                CFL = 1 - q2 / (df2 + 1 + q2 / (df2 + 2 + tk));
                prob = Math.Exp(df2 * Math.Log(q2) - q2 - lngamma(df2 + 1) - Math.Log(CFL));
            }
            else
            {
                tk = (nn - df2) / (q2 + nn);
                for (int kk = nn - 1; kk > 1; kk--)
                    tk = (kk - df2) / (q2 + kk / (1 + tk));
                CFU = 1 + (1 - df2) / (q2 + 1 / (1 + tk));
                prob = 1 - Math.Exp((df2 - 1) * Math.Log(q2) - q2 - lngamma(df2) - Math.Log(CFU));
            }
            prob = 1 - prob;
            return prob;
        }

        public double lngamma(double c)
        {
            double[] cof = new double[6];
            cof[0] = 76.18009172947146;
            cof[1] = -86.50532032941677;
            cof[2] = 24.01409824083091;
            cof[3] = -1.231739572450155;
            cof[4] = 0.1208650973866179e-2;
            cof[5] = -0.5395239384953e-5;
            double xx = c;
            double yy = c;
            double tmp = xx + 5.5 - (xx + 0.5) * Math.Log(xx + 5.5);
            double ser = 1.000000000190015;
            for (int j = 0; j <= 5; j++)
                ser += (cof[j] / ++yy);
            return (Math.Log(2.5066282746310005 * ser / xx) - tmp);
        }

        public ChiSquareResult GetChiSquareForTrend(double[] col1, double[] col2, double[] col3)
        {
            ChiSquareResult result = new ChiSquareResult();
            double[] acrude = new double[col1.Length];
            double[] bcrude = new double[col1.Length];
            double[] mhORad = new double[col1.Length];
            double[] mhORbc = new double[col1.Length];
            double[] oddsRatios = new double[col1.Length];

            int levels = col1.Length;
            double ccrude = 0;
            double dcrude = 0;
            double ttot;
            double Vsum = 0;
            double V1sum = 0;
            double XMHchisq = 0;

            double T1 = 0;
            double T2 = 0;
            double T3 = 0;
            double n1 = 0;
            double n2 = 0;
            double n = 0;
            double OR = 1.0;

            double x, a, b, m;

            double abase = col2[0];
            double bbase = col3[0];
            ccrude += abase;
            dcrude += bbase;

            for (int t = 0; t < levels; t++)
            {
                x = col1[t];

                a = col2[t];
                b = col3[t];
                acrude[t] += a;
                bcrude[t] += b;
                m = a + b;
                T1 += a * x;
                T2 += m * x;
                T3 += m * x * x;
                n1 += a;
                n2 += b;
                n += a + b;

                OR = (a * bbase) / (b * abase);
                if (t > 0)
                {
                    ttot = a + bbase + b + abase;
                    mhORad[t] += a * bbase / ttot;
                    mhORbc[t] += b * abase / ttot;
                }
                oddsRatios[t] = OR;
            }
            result.SetOddsRatios(oddsRatios);

            Vsum += (n1 * n2 * (n * T3 - (T2 * T2))) / (n * n * (n - 1));
            V1sum += T1 - ((n1 / n) * T2);
            XMHchisq += ((V1sum - 0.5) * (V1sum - 0.5)) / Vsum;
            result.SetChi(XMHchisq);
            result.SetPValue(calcP(XMHchisq, 1));
            return result;
        }

        public int PreferredUIHeight
        {
            get
            {
                return 425;
            }
        }

        public int PreferredUIWidth
        {
            get
            {
                return 515;
            }
        }
        public void UpdateVariableNames()
        {
            //FillComboboxes(true);
        }

        public StringBuilder HtmlBuilder { get; set; }

        public void RefreshResults()
        {

        }

        //public System.Xml.XmlNode Serialize(System.Xml.XmlDocument doc)
        //{
        //    return null;
        //}

        //public void CreateFromXml(System.Xml.XmlElement element)
        //{

        //}

        public string ToHTML(bool ForDash = false, string htmlFileName = "", int count = 0)
        {
            return string.Empty;
        }

        public string CustomOutputHeading { get { return string.Empty; } set { } }
        public string CustomOutputDescription { get { return string.Empty; } set { } }
        public string CustomOutputCaption { get { return string.Empty; } set { } }

        public bool IsProcessing
        {
            get
            {
                return false;
            }
            set
            {
                //isProcessing = value;
            }
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
            //if (GadgetProcessingFinished != null)
            //    GadgetProcessingFinished(this);
        }

        public bool DrawBorders { get; set; }


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

    public class ChiSquareResult
    {

        private double[] oddsRatios;
        private double chi;
        private double pValue;

        public double[] GetOddsRatios()
        {
            return oddsRatios;
        }

        public void SetOddsRatios(double[] oddsRatios)
        {
            this.oddsRatios = oddsRatios;
        }

        public double GetChi()
        {
            return chi;
        }

        public void SetChi(double chi)
        {
            this.chi = chi;
        }

        public double GetPValue()
        {
            return pValue;
        }

        public void SetPValue(double pValue)
        {
            this.pValue = pValue;
        }

    }
}