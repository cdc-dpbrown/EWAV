/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       Binomial.xaml.cs
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
using Ewav.ViewModels;
using Ewav.Web.Services;
using System.ComponentModel.Composition;
using System.ComponentModel;
using System.ServiceModel.DomainServices.Client;
using System.Text;

namespace Ewav
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(UserControl))]
    [ExportMetadata("type", "statcalc")]
    [ExportMetadata("tabindex", "22")]
    public partial class Binomial : UserControl, IEwavGadget, IChartControl, IGadget
    {
        //private bool isProcessing;
        //private StatisticsRepository.Strat2x2 strat2x2;
        BinomialStatCalcViewModel bViewModel;
        public Binomial()
        {
            // Required to initialize variables
            InitializeComponent();

            Construct();
        }
        private void Construct()
        {
            //GadgetContent.Visibility = System.Windows.Visibility.Visible;
            bViewModel = new BinomialStatCalcViewModel();
            bViewModel.StatCalcLoadedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(bViewModel_StatCalcLoadedEvent);
            bViewModel.ErrorNotice += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(bViewModel_ErrorNotice);
        }

        void bViewModel_ErrorNotice(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            if (e.Data.Message.Length > 0)
            {
                ChildWindow window = new ErrorWindow(e.Data);
                window.Show();
                //return;
            }
            
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

        #region IGadget Members

        /// <summary>
        /// Updates the variable names available in the gadget's properties
        /// </summary>
        public void UpdateVariableNames()
        {
            //FillComboboxes(true);
        }

        public StringBuilder HtmlBuilder { get; set; }

        public void RefreshResults()
        {

        }



        /// <summary>
        /// Converts the gadget's output to Html
        /// </summary>
        /// <returns></returns>
        public string ToHTML(bool ForDash = false, string htmlFileName = "", int count = 0)
        {
            return string.Empty;


        }

        public string CustomOutputHeading { get { return string.Empty; } set { } }
        public string CustomOutputDescription { get { return string.Empty; } set { } }
        public string CustomOutputCaption { get { return string.Empty; } set { } }
        #endregion


        private void btnGo_Click(object sender, RoutedEventArgs e)
        {

            int numerator = 0;
            int observed = 0;
            double expected = 0;
            bool parseResult1 = int.TryParse(txtNumerator.Text, out numerator);
            bool parseResult2 = int.TryParse(txtObserved.Text, out observed);
            bool parseResult3 = double.TryParse(txtExpected.Text, out expected);
            bViewModel = new BinomialStatCalcViewModel();
            if (parseResult1 && parseResult2 && parseResult3 && numerator < observed)
            {
                bViewModel.GetBinomialStatCalc(txtNumerator.Text, txtObserved.Text, txtExpected.Text);
                waitCursor.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                txtLessThan.Text = "";
                txtLessThanEqual.Text = "";
                txtEqual.Text = "";
                txtGreaterThanEqual.Text = "";
                txtGreaterThan.Text = "";
                txtPValue.Text = "";
                txt95Ci.Text = "";
            }
        }

        void bViewModel_StatCalcLoadedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            BinomialStatCalcDTO bDto = bViewModel.DataBag;
            int numerator = 0;
            int observed = 0;
            double expected = 0;
            bool parseResult1 = int.TryParse(txtNumerator.Text, out numerator);
            bool parseResult2 = int.TryParse(txtObserved.Text, out observed);
            bool parseResult3 = double.TryParse(txtExpected.Text, out expected);
            if (parseResult1 && parseResult2 && parseResult3 && numerator < observed)
            {
                lblLessThan.Text = "< " + numerator;
                lblLessThanEqual.Text = "<= " + numerator;
                lblEqual.Text = "= " + numerator;
                lblGreaterThanEqual.Text = ">= " + numerator;
                lblGreaterThan.Text = "> " + numerator;

                txtLessThan.Text = bDto.LessThanTxt;
                txtLessThanEqual.Text = bDto.LessThanEqualTxt;
                txtEqual.Text = bDto.EqualTxt;
                txtGreaterThanEqual.Text = bDto.GreaterThanEqualTxt;
                txtGreaterThan.Text = bDto.GreaterThanTxt;
                txtPValue.Text = bDto.PValueTxt;
                txt95Ci.Text = bDto.NinefiveCiTxt;
            }
            
            waitCursor.Visibility = System.Windows.Visibility.Collapsed;

        }

        public string MyControlName
        {
            get { return "Binomial";  }
        }

        public string MyUIName
        {
            get { return "Binomial (Proportion vs. std.)"; }
        }

        public ApplicationViewModel ApplicationViewModel
        {
            get { throw new NotImplementedException(); }
        }

        public void SetChartLabels()
        {
            throw new NotImplementedException();
        }

        public void SaveAsImage()
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

        public System.Xml.Linq.XNode Serialize(System.Xml.Linq.XDocument doc)
        {
            //throw new NotImplementedException();
            return null;
        }


        public void CreateFromXml(System.Xml.Linq.XElement element)
        {
            //throw new NotImplementedException();
        }


        public ClientCommon.XYControlChartTypes GetChartTypeEnum()
        {



            return ClientCommon.XYControlChartTypes.Ignore;  


        }


        public void Reload()
        {
            throw new NotImplementedException();
        }


        public List<EwavDataFilterCondition> GadgetFilters
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

namespace Ewav.Web.Services
{
    public partial class BinomialDomainContext
    {
        //This is the place to set RIA Services query timeout. 
        //TimeSpan(0, 5, 0) means five minutes vs the 
        //default 60 sec
        partial void OnCreated()
        {
            if (!DesignerProperties.GetIsInDesignMode(Application.
                Current.RootVisual))
                ((WebDomainClient<IBinomialDomainServiceContract>)
                    DomainClient).ChannelFactory.Endpoint.Binding.SendTimeout =
                    new TimeSpan(0, 5, 0);
        }
    }
}