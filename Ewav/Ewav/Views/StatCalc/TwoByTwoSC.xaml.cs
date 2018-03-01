/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       TwoByTwoSC.xaml.cs
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
using System.ComponentModel.Composition;
using System.Text;
using Ewav.Web.Services;
using Ewav.ExtensionMethods;
using Ewav.ViewModels;
using ComponentArt.Silverlight.DataVisualization.Common;

namespace Ewav
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(UserControl))]
    [ExportMetadata("type", "statcalc")]
    [ExportMetadata("tabindex", "21")]
    public partial class TwoByTwoSC : UserControl, IGadget, IEwavGadget
    {
        private bool isProcessing;
        List<DictionaryDTO> strataValues;
        string txtYesYesVal;
        StatCalcViewModel statcalcViewModel;
        MySingleTableResults singleResults;
        List<DictionaryDTO> strataVals, strataActive;
        int strata = 0;

        List<int> TabWorkedOn = new List<int>();

        public TwoByTwoSC()
        {
            InitializeComponent();
            strataValues = new List<DictionaryDTO>();
            statcalcViewModel = new StatCalcViewModel();
            strataVals = new List<DictionaryDTO>();
            strataActive = new List<DictionaryDTO>();
            txtYesYesVal1.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtYesNoVal1.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtNoYesVal1.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtNoNoVal1.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);

            txtYesYesVal2.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtYesNoVal2.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtNoYesVal2.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtNoNoVal2.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);

            txtYesYesVal3.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtYesNoVal3.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtNoYesVal3.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtNoNoVal3.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);

            txtYesYesVal4.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtYesNoVal4.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtNoYesVal4.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtNoNoVal4.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);

            txtYesYesVal5.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtYesNoVal5.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtNoYesVal5.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtNoNoVal5.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);

            txtYesYesVal6.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtYesNoVal6.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtNoYesVal6.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtNoNoVal6.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);

            txtYesYesVal7.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtYesNoVal7.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtNoYesVal7.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtNoNoVal7.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);

            txtYesYesVal8.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtYesNoVal8.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtNoYesVal8.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtNoNoVal8.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);

            txtYesYesVal9.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtYesNoVal9.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtNoYesVal9.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
            txtNoNoVal9.TextChanged += new TextChangedEventHandler(txtInputs_TextChanged);
        }

        //private void DoFocus()
        //{
        //    txtYesYesVal1.Focus();
        //}

        private void Construct()
        {
            //GadgetContent.Visibility = System.Windows.Visibility.Visible;
            statcalcViewModel.SingleTableLoadedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(statcalcViewModel_SingleTableLoadedEvent);
            statcalcViewModel.ErrorNotice += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(statcalcViewModel_ErrorNotice);
        }
        /// <summary>
        /// Serializes the specified doc.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <returns></returns>
        public System.Xml.Linq.XNode Serialize(System.Xml.Linq.XDocument doc)
        {
            // TODO: Implement this method
            //throw new NotImplementedException();
            return null;
        }

        void statcalcViewModel_ErrorNotice(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            if (e.Data.Message.Length > 0)
            {
                ChildWindow window = new ErrorWindow(e.Data);
                window.Show();
                //return;
            }

            this.SetGadgetToFinishedState();
        }

        private childItem FindChild<childItem>(DependencyObject obj, string name)
    where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem && ((FrameworkElement)child).Name.Equals(name))
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindChild<childItem>(child, name);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        private void CalculateStrata(int strata)
        {

            try
            {
                int yyVal = 0;
                int ynVal = 0;
                int nyVal = 0;
                int nnVal = 0;
                this.strata = strata;

                Grid grd = FindChild<Grid>(stackPanel, "grdTables" + strata);

                TextBox txtYesYesVal = FindChild<TextBox>(grd, "txtYesYesVal" + strata);

                TextBox txtYesNoVal = FindChild<TextBox>(grd, "txtYesNoVal" + strata);
                TextBox txtNoYesVal = FindChild<TextBox>(grd, "txtNoYesVal" + strata);
                TextBox txtNoNoVal = FindChild<TextBox>(grd, "txtNoNoVal" + strata);

                TextBlock txtYesTotalVal = FindChild<TextBlock>(grd, "txtYesTotalVal" + strata);
                TextBlock txtNoTotalVal = FindChild<TextBlock>(grd, "txtNoTotalVal" + strata);
                TextBlock txtYesYesRow = FindChild<TextBlock>(grd, "txtYesYesRow" + strata);
                TextBlock txtYesNoRow = FindChild<TextBlock>(grd, "txtYesNoRow" + strata);
                TextBlock txtNoYesRow = FindChild<TextBlock>(grd, "txtNoYesRow" + strata);
                TextBlock txtNoNoRow = FindChild<TextBlock>(grd, "txtNoNoRow" + strata);
                TextBlock txtTotalTotalVal = FindChild<TextBlock>(grd, "txtTotalTotalVal" + strata);
                TextBlock txtTotalYesVal = FindChild<TextBlock>(grd, "txtTotalYesVal" + strata);
                TextBlock txtYesYesCol = FindChild<TextBlock>(grd, "txtYesYesCol" + strata);
                TextBlock txtNoYesCol = FindChild<TextBlock>(grd, "txtNoYesCol" + strata);
                TextBlock txtTotalNoVal = FindChild<TextBlock>(grd, "txtTotalNoVal" + strata);
                TextBlock txtYesNoCol = FindChild<TextBlock>(grd, "txtYesNoCol" + strata);
                TextBlock txtNoNoCol = FindChild<TextBlock>(grd, "txtNoNoCol" + strata);
                TextBlock txtTotalYesRow = FindChild<TextBlock>(grd, "txtTotalYesRow" + strata);
                TextBlock txtTotalNoRow = FindChild<TextBlock>(grd, "txtTotalNoRow" + strata);
                TextBlock txtYesTotalCol = FindChild<TextBlock>(grd, "txtYesTotalCol" + strata);
                TextBlock txtNoTotalCol = FindChild<TextBlock>(grd, "txtNoTotalCol" + strata);
                TextBlock txtTotalYesCol = FindChild<TextBlock>(grd, "txtTotalYesCol" + strata);
                TextBlock txtTotalNoCol = FindChild<TextBlock>(grd, "txtTotalNoCol" + strata);
                TextBlock txtYesTotalRow = FindChild<TextBlock>(grd, "txtYesTotalRow" + strata);
                TextBlock txtNoTotalRow = FindChild<TextBlock>(grd, "txtNoTotalRow" + strata);
                TextBlock txtTotalTotalCol = FindChild<TextBlock>(grd, "txtTotalTotalCol" + strata);
                TextBlock txtTotalTotalRow = FindChild<TextBlock>(grd, "txtTotalTotalRow" + strata);

                TextBlock txtChiSqCorP = FindChild<TextBlock>(stackPanel, "txtChiSqCorP" + strata);
                TextBlock txtChiSqCorVal = FindChild<TextBlock>(stackPanel, "txtChiSqCorVal" + strata);
                TextBlock txtChiSqManP = FindChild<TextBlock>(stackPanel, "txtChiSqManP" + strata);
                TextBlock txtChiSqManVal = FindChild<TextBlock>(stackPanel, "txtChiSqManVal" + strata);
                TextBlock txtChiSqUncP = FindChild<TextBlock>(stackPanel, "txtChiSqUncP" + strata);
                TextBlock txtChiSqUncVal = FindChild<TextBlock>(stackPanel, "txtChiSqUncVal" + strata);
                TextBlock txtOddsRatioEstimate = FindChild<TextBlock>(stackPanel, "txtOddsRatioEstimate" + strata);
                TextBlock txtOddsRatioLower = FindChild<TextBlock>(stackPanel, "txtOddsRatioLower" + strata);
                TextBlock txtOddsRatioUpper = FindChild<TextBlock>(stackPanel, "txtOddsRatioUpper" + strata);
                TextBlock txtMidPEstimate = FindChild<TextBlock>(stackPanel, "txtMidPEstimate" + strata);
                TextBlock txtMidPLower = FindChild<TextBlock>(stackPanel, "txtMidPLower" + strata);
                TextBlock txtMidPUpper = FindChild<TextBlock>(stackPanel, "txtMidPUpper" + strata);
                TextBlock txtFisherLower = FindChild<TextBlock>(stackPanel, "txtFisherLower" + strata);
                TextBlock txtFisherUpper = FindChild<TextBlock>(stackPanel, "txtFisherUpper" + strata);
                TextBlock txtRiskDifferenceEstimate = FindChild<TextBlock>(stackPanel, "txtRiskDifferenceEstimate" + strata);
                TextBlock txtRiskDifferenceLower = FindChild<TextBlock>(stackPanel, "txtRiskDifferenceLower" + strata);
                TextBlock txtRiskDifferenceUpper = FindChild<TextBlock>(stackPanel, "txtRiskDifferenceUpper" + strata);
                TextBlock txtRiskRatioEstimate = FindChild<TextBlock>(stackPanel, "txtRiskRatioEstimate" + strata);
                TextBlock txtRiskRatioLower = FindChild<TextBlock>(stackPanel, "txtRiskRatioLower" + strata);
                TextBlock txtRiskRatioUpper = FindChild<TextBlock>(stackPanel, "txtRiskRatioUpper" + strata);
                TextBlock txtFisherExact = FindChild<TextBlock>(stackPanel, "txtFisherExact" + strata);
                TextBlock txtMidPExact = FindChild<TextBlock>(stackPanel, "txtMidPExact" + strata);
                TextBlock txtFisherExact2P = FindChild<TextBlock>(stackPanel, "txtFisherExact2P" + strata);

                int.TryParse(txtYesYesVal.Text, out yyVal);
                int.TryParse(txtYesNoVal.Text, out ynVal);
                int.TryParse(txtNoYesVal.Text, out nyVal);
                int.TryParse(txtNoNoVal.Text, out nnVal);

                DictionaryDTO dto = new DictionaryDTO();
                dto.Key = (strata.ToString() + "0").ToMyString();
                dto.Value = yyVal.ToString().ToMyString();
                strataVals.Add(dto);
                dto = new DictionaryDTO();
                dto.Key = (strata.ToString() + "1").ToMyString();
                dto.Value = ynVal.ToString().ToMyString();
                strataVals.Add(dto);
                dto = new DictionaryDTO();
                dto.Key = (strata.ToString() + "2").ToMyString();
                dto.Value = nyVal.ToString().ToMyString();
                strataVals.Add(dto);
                dto = new DictionaryDTO();
                dto.Key = (strata.ToString() + "3").ToMyString();
                dto.Value = nnVal.ToString().ToMyString();
                strataVals.Add(dto);

                dto = new DictionaryDTO();
                dto.Key = (strata.ToString() + "0").ToMyString();
                dto.Value = (txtYesYesVal.Text.Length > 0).ToString().ToMyString();
                strataActive.Add(dto);
                dto = new DictionaryDTO();
                dto.Key = (strata.ToString() + "1").ToMyString();
                dto.Value = (txtYesNoVal.Text.Length > 0).ToString().ToMyString();
                strataActive.Add(dto);
                dto = new DictionaryDTO();
                dto.Key = (strata.ToString() + "2").ToMyString();
                dto.Value = (txtNoYesVal.Text.Length > 0).ToString().ToMyString();
                strataActive.Add(dto);
                dto = new DictionaryDTO();
                dto.Key = (strata.ToString() + "3").ToMyString();
                dto.Value = (txtNoNoVal.Text.Length > 0).ToString().ToMyString();
                strataActive.Add(dto);
                //strataVals[strata][0] = yyVal;
                //strataVals[strata][1] = ynVal;
                //strataVals[strata][2] = ;
                //strataVals[strata][3] = ;
                //strataActive[strata][0] = (txtYesYesVal.Text.Length > 0);
                //strataActive[strata][1] = (txtYesNoVal.Text.Length > 0);
                //strataActive[strata][2] = (txtNoYesVal.Text.Length > 0);
                //strataActive[strata][3] = (txtNoNoVal.Text.Length > 0);

                int ytVal = yyVal + ynVal;
                int ntVal = nyVal + nnVal;
                int ttVal = ytVal + ntVal;
                int tyVal = yyVal + nyVal;
                int tnVal = ynVal + nnVal;
                double yyRowPct = 0;
                double ynRowPct = 0;
                double nyRowPct = 0;
                double nnRowPct = 0;
                double yyColPct = 0;
                double nyColPct = 0;
                double ynColPct = 0;
                double nnColPct = 0;
                double tyRowPct = 0;
                double tnRowPct = 0;
                double ytColPct = 0;
                double ntColPct = 0;
                if (ytVal != 0)
                {
                    yyRowPct = 1.0 * yyVal / ytVal;
                    ynRowPct = 1.0 * ynVal / ytVal;
                }
                if (ntVal != 0)
                {
                    nyRowPct = 1.0 * nyVal / ntVal;
                    nnRowPct = 1.0 * nnVal / ntVal;
                }
                if (tyVal != 0)
                {
                    yyColPct = 1.0 * yyVal / tyVal;
                    nyColPct = 1.0 * nyVal / tyVal;
                }
                if (tnVal != 0)
                {
                    ynColPct = 1.0 * ynVal / tnVal;
                    nnColPct = 1.0 * nnVal / tnVal;
                }
                if (ttVal != 0)
                {
                    tyRowPct = 1.0 * tyVal / ttVal;
                    tnRowPct = 1.0 * tnVal / ttVal;
                    ytColPct = 1.0 * ytVal / ttVal;
                    ntColPct = 1.0 * ntVal / ttVal;
                }

                txtYesTotalVal.Text = ytVal.ToString();
                txtNoTotalVal.Text = ntVal.ToString();
                txtYesYesRow.Text = yyRowPct.ToString("P");
                txtYesNoRow.Text = ynRowPct.ToString("P");
                txtNoYesRow.Text = nyRowPct.ToString("P");
                txtNoNoRow.Text = nnRowPct.ToString("P");
                txtTotalTotalVal.Text = ttVal.ToString();
                txtTotalYesVal.Text = tyVal.ToString();
                txtYesYesCol.Text = yyColPct.ToString("P");
                txtNoYesCol.Text = nyColPct.ToString("P");
                txtTotalNoVal.Text = tnVal.ToString();
                txtYesNoCol.Text = ynColPct.ToString("P");
                txtNoNoCol.Text = nnColPct.ToString("P");
                txtTotalYesRow.Text = tyRowPct.ToString("P");
                txtTotalNoRow.Text = tnRowPct.ToString("P");
                txtYesTotalCol.Text = ytColPct.ToString("P");
                txtNoTotalCol.Text = ntColPct.ToString("P");
                txtTotalYesCol.Text = (1).ToString("P");
                txtTotalNoCol.Text = (1).ToString("P");
                txtYesTotalRow.Text = (1).ToString("P");
                txtNoTotalRow.Text = (1).ToString("P");
                txtTotalTotalCol.Text = (1).ToString("P");
                txtTotalTotalRow.Text = (1).ToString("P");

                statcalcViewModel.GetStatCalc2x2(ytVal, ntVal, tyVal, tnVal, yyVal, ynVal, nyVal, nnVal, strataActive, strataVals);

                statcalcViewModel.SingleTableLoadedEvent += new EventHandler<SimpleMvvmToolkit.NotificationEventArgs<Exception>>(statcalcViewModel_SingleTableLoadedEvent);


            }
            catch (Exception ex)
            {
                //
            }
        }

        void statcalcViewModel_SingleTableLoadedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {
            singleResults = statcalcViewModel.StatCalcdto.SingleResults;
            RenderFinish();
        }

        void txtInputs_TextChanged(object sender, TextChangedEventArgs e)
        {
            int TabCount = Convert.ToInt32(((TextBox)sender).Name.Substring(((TextBox)sender).Name.Length - 1));
            if (!TabWorkedOn.Contains(TabCount))
            {
                TabWorkedOn.Add(TabCount);
            }

            button1.IsEnabled = true;
        }

        void RenderFinish()
        {
            List<MyString> results = statcalcViewModel.StatCalcdto.Results;
            MySingleTableResults singleTableResults = statcalcViewModel.StatCalcdto.SingleResults;
            string oddsRatioEstimate = "";
            string oddsRatioLower = "";
            string oddsRatioUpper = "";
            string riskRatioLower = "";
            string riskRatioUpper = "";
            string fisherExact2P = "";
            Grid grd = FindChild<Grid>(stackPanel, "grdTables" + strata);
            TextBox txtYesYesVal = FindChild<TextBox>(grd, "txtYesYesVal" + strata);
            TextBox txtYesNoVal = FindChild<TextBox>(grd, "txtYesNoVal" + strata);
            TextBox txtNoYesVal = FindChild<TextBox>(grd, "txtNoYesVal" + strata);
            TextBox txtNoNoVal = FindChild<TextBox>(grd, "txtNoNoVal" + strata);

            TextBlock txtYesTotalVal = FindChild<TextBlock>(grd, "txtYesTotalVal" + strata);
            TextBlock txtNoTotalVal = FindChild<TextBlock>(grd, "txtNoTotalVal" + strata);
            TextBlock txtYesYesRow = FindChild<TextBlock>(grd, "txtYesYesRow" + strata);
            TextBlock txtYesNoRow = FindChild<TextBlock>(grd, "txtYesNoRow" + strata);
            TextBlock txtNoYesRow = FindChild<TextBlock>(grd, "txtNoYesRow" + strata);
            TextBlock txtNoNoRow = FindChild<TextBlock>(grd, "txtNoNoRow" + strata);
            TextBlock txtTotalTotalVal = FindChild<TextBlock>(grd, "txtTotalTotalVal" + strata);
            TextBlock txtTotalYesVal = FindChild<TextBlock>(grd, "txtTotalYesVal" + strata);
            TextBlock txtYesYesCol = FindChild<TextBlock>(grd, "txtYesYesCol" + strata);
            TextBlock txtNoYesCol = FindChild<TextBlock>(grd, "txtNoYesCol" + strata);
            TextBlock txtTotalNoVal = FindChild<TextBlock>(grd, "txtTotalNoVal" + strata);
            TextBlock txtYesNoCol = FindChild<TextBlock>(grd, "txtYesNoCol" + strata);
            TextBlock txtNoNoCol = FindChild<TextBlock>(grd, "txtNoNoCol" + strata);
            TextBlock txtTotalYesRow = FindChild<TextBlock>(grd, "txtTotalYesRow" + strata);
            TextBlock txtTotalNoRow = FindChild<TextBlock>(grd, "txtTotalNoRow" + strata);
            TextBlock txtYesTotalCol = FindChild<TextBlock>(grd, "txtYesTotalCol" + strata);
            TextBlock txtNoTotalCol = FindChild<TextBlock>(grd, "txtNoTotalCol" + strata);
            TextBlock txtTotalYesCol = FindChild<TextBlock>(grd, "txtTotalYesCol" + strata);
            TextBlock txtTotalNoCol = FindChild<TextBlock>(grd, "txtTotalNoCol" + strata);
            TextBlock txtYesTotalRow = FindChild<TextBlock>(grd, "txtYesTotalRow" + strata);
            TextBlock txtNoTotalRow = FindChild<TextBlock>(grd, "txtNoTotalRow" + strata);
            TextBlock txtTotalTotalCol = FindChild<TextBlock>(grd, "txtTotalTotalCol" + strata);
            TextBlock txtTotalTotalRow = FindChild<TextBlock>(grd, "txtTotalTotalRow" + strata);

            TextBlock txtChiSqCorP = FindChild<TextBlock>(stackPanel, "txtChiSqCorP" + strata);
            TextBlock txtChiSqCorVal = FindChild<TextBlock>(stackPanel, "txtChiSqCorVal" + strata);
            TextBlock txtChiSqManP = FindChild<TextBlock>(stackPanel, "txtChiSqManP" + strata);
            TextBlock txtChiSqManVal = FindChild<TextBlock>(stackPanel, "txtChiSqManVal" + strata);
            TextBlock txtChiSqUncP = FindChild<TextBlock>(stackPanel, "txtChiSqUncP" + strata);
            TextBlock txtChiSqUncVal = FindChild<TextBlock>(stackPanel, "txtChiSqUncVal" + strata);
            TextBlock txtOddsRatioEstimate = FindChild<TextBlock>(stackPanel, "txtOddsRatioEstimate" + strata);
            TextBlock txtOddsRatioLower = FindChild<TextBlock>(stackPanel, "txtOddsRatioLower" + strata);
            TextBlock txtOddsRatioUpper = FindChild<TextBlock>(stackPanel, "txtOddsRatioUpper" + strata);
            TextBlock txtMidPEstimate = FindChild<TextBlock>(stackPanel, "txtMidPEstimate" + strata);
            TextBlock txtMidPLower = FindChild<TextBlock>(stackPanel, "txtMidPLower" + strata);
            TextBlock txtMidPUpper = FindChild<TextBlock>(stackPanel, "txtMidPUpper" + strata);
            TextBlock txtFisherLower = FindChild<TextBlock>(stackPanel, "txtFisherLower" + strata);
            TextBlock txtFisherUpper = FindChild<TextBlock>(stackPanel, "txtFisherUpper" + strata);
            TextBlock txtRiskDifferenceEstimate = FindChild<TextBlock>(stackPanel, "txtRiskDifferenceEstimate" + strata);
            TextBlock txtRiskDifferenceLower = FindChild<TextBlock>(stackPanel, "txtRiskDifferenceLower" + strata);
            TextBlock txtRiskDifferenceUpper = FindChild<TextBlock>(stackPanel, "txtRiskDifferenceUpper" + strata);
            TextBlock txtRiskRatioEstimate = FindChild<TextBlock>(stackPanel, "txtRiskRatioEstimate" + strata);
            TextBlock txtRiskRatioLower = FindChild<TextBlock>(stackPanel, "txtRiskRatioLower" + strata);
            TextBlock txtRiskRatioUpper = FindChild<TextBlock>(stackPanel, "txtRiskRatioUpper" + strata);
            TextBlock txtFisherExact = FindChild<TextBlock>(stackPanel, "txtFisherExact" + strata);
            TextBlock txtMidPExact = FindChild<TextBlock>(stackPanel, "txtMidPExact" + strata);
            TextBlock txtFisherExact2P = FindChild<TextBlock>(stackPanel, "txtFisherExact2P" + strata);

            if (singleTableResults.FisherExact2P != -1)
            {
                fisherExact2P = ((double)singleTableResults.FisherExact2P).ToString("F10");
            }

            if (singleTableResults.OddsRatioEstimate != null)
            {
                oddsRatioEstimate = ((double)singleTableResults.OddsRatioEstimate).ToString("F4");
            }

            if (singleTableResults.OddsRatioLower != null)
            {
                oddsRatioLower = ((double)singleTableResults.OddsRatioLower).ToString("F4");
            }

            if (singleTableResults.OddsRatioUpper != null)
            {
                oddsRatioUpper = ((double)singleTableResults.OddsRatioUpper).ToString("F4");
            }

            if (singleTableResults.RiskRatioLower != null)
            {
                riskRatioLower = ((double)singleTableResults.RiskRatioLower).ToString("F4");
            }

            if (singleTableResults.RiskRatioUpper != null)
            {
                riskRatioUpper = ((double)singleTableResults.RiskRatioUpper).ToString("F4");
            }

            txtChiSqCorP.Text = ((double)singleTableResults.ChiSquareYates2P).ToString("F10");
            txtChiSqCorVal.Text = ((double)singleTableResults.ChiSquareYatesVal).ToString("F4");
            txtChiSqManP.Text = ((double)singleTableResults.ChiSquareMantel2P).ToString("F10");
            txtChiSqManVal.Text = ((double)singleTableResults.ChiSquareMantelVal).ToString("F4");
            txtChiSqUncP.Text = ((double)singleTableResults.ChiSquareUncorrected2P).ToString("F10");
            txtChiSqUncVal.Text = ((double)singleTableResults.ChiSquareUncorrectedVal).ToString("F4");
            txtOddsRatioEstimate.Text = oddsRatioEstimate; // singleTableResults.OddsRatioEstimate.ToString("F4");
            txtOddsRatioLower.Text = oddsRatioLower; //singleTableResults.OddsRatioLower.ToString("F4");
            txtOddsRatioUpper.Text = oddsRatioUpper; // singleTableResults.OddsRatioUpper.ToString("F4");
            txtMidPEstimate.Text = ((double)singleTableResults.OddsRatioMLEEstimate).ToString("F4");
            txtMidPLower.Text = ((double)singleTableResults.OddsRatioMLEMidPLower).ToString("F4");
            txtMidPUpper.Text = ((double)singleTableResults.OddsRatioMLEMidPUpper).ToString("F4");
            txtFisherLower.Text = ((double)singleTableResults.OddsRatioMLEFisherLower).ToString("F4");
            txtFisherUpper.Text = ((double)singleTableResults.OddsRatioMLEFisherUpper).ToString("F4");
            txtRiskDifferenceEstimate.Text = ((double)singleTableResults.RiskDifferenceEstimate).ToString("F4");
            txtRiskDifferenceLower.Text = ((double)singleTableResults.RiskDifferenceLower).ToString("F4");
            txtRiskDifferenceUpper.Text = ((double)singleTableResults.RiskDifferenceUpper).ToString("F4");
            txtRiskRatioEstimate.Text = ((double)singleTableResults.RiskRatioEstimate).ToString("F4");
            txtRiskRatioLower.Text = riskRatioLower; //singleTableResults.RiskRatioLower.ToString("F4");
            txtRiskRatioUpper.Text = riskRatioUpper; //singleTableResults.RiskRatioUpper.ToString("F4");
            txtFisherExact.Text = ((double)singleTableResults.FisherExactP).ToString("F10");
            txtMidPExact.Text = ((double)singleTableResults.MidP).ToString("F10");
            txtFisherExact2P.Text = fisherExact2P;


            if (results.Count > 0)
            {
                txtStratAdjustedMle.Text = results[0].VarName;
                txtStratAdjustedMleLower.Text = results[1].VarName;
                txtStratAdjustedMleUpper.Text = results[2].VarName;

                txtStratAdjustedRr.Text = results[3].VarName;
                txtStratAdjustedRrLower.Text = results[4].VarName;
                txtStratAdjustedRrUpper.Text = results[5].VarName;

                txtStratCrudeOr.Text = results[6].VarName;
                txtStratCrudeOrLower.Text = results[7].VarName;
                txtStratCrudeOrUpper.Text = results[8].VarName;

                txtStratCrudeMle.Text = results[9].VarName;
                txtStratCrudeMleLower.Text = results[10].VarName;
                txtStratCrudeMleUpper.Text = results[11].VarName;

                txtStratFisherLower.Text = results[12].VarName;
                txtStratFisherUpper.Text = results[13].VarName;

                txtStratCrudeRr.Text = results[14].VarName;
                txtStratCrudeRrLower.Text = results[15].VarName;
                txtStratCrudeRrUpper.Text = results[16].VarName;

                txtStratAdjustedOr.Text = results[17].VarName;
                txtStratAdjustedOrLower.Text = results[18].VarName;
                txtStratAdjustedOrUpper.Text = results[19].VarName;

                txtStratChiUnc.Text = results[20].VarName;
                txtStratChiUnc2Tail.Text = results[21].VarName;

                txtStratChiCor.Text = results[22].VarName;
                txtStratChiCor2Tail.Text = results[23].VarName;
            }
            if (TabWorkedOn.Count > 0)
            {
                CalculateStrata(TabWorkedOn[0]);
                TabWorkedOn.RemoveAt(0);
            }
            waitCursor.Visibility = System.Windows.Visibility.Collapsed;
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
            GadgetWindow.Visibility = System.Windows.Visibility.Collapsed;
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
            get { return "StatCalc2x2"; }
        }

        public string MyUIName
        {
            get { return "Tables (2x2, 2xn)"; }
        }

        public ViewModels.ApplicationViewModel ApplicationViewModel
        {
            get { throw new NotImplementedException(); }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //int strata = ((TabControl)grdTables).SelectedIndex + 1;
            //for (int i = 0; i < TabWorkedOn.Count; i++)
            //{
            CalculateStrata(TabWorkedOn[0]);
            TabWorkedOn.RemoveAt(0);
            //}

            //button1.Content = "GO ->";
            waitCursor.Visibility = System.Windows.Visibility.Visible;
            button1.IsEnabled = false;
        }


        public void CreateFromXml(System.Xml.Linq.XElement element)
        {
            //throw new NotImplementedException();
        }

        private void Strata_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            btn.Style = Application.Current.Resources["btnprimary"] as Style;
            string starta = btn.Name.Substring(btn.Name.Length - 1);

            for (int i = 1; i < 10; i++)
            {
                StackPanel sp = FindChild<StackPanel>(stackPanel, "spStrata" + i);
                Button bp = FindChild<Button>(stackPanel, "Strata" + i);
                if (i == Convert.ToInt16(starta))
                {
                    sp.Visibility = System.Windows.Visibility.Visible;
                    bp.Style = Application.Current.Resources["btnprimary"] as Style;
                }
                else
                {
                    sp.Visibility = System.Windows.Visibility.Collapsed;
                    bp.Style = Application.Current.Resources["ButtonStyle"] as Style;

                }
            }
            
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