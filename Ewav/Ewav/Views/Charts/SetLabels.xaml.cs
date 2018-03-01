/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       SetLabels.xaml.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Ewav.ViewModels;
using System.Windows.Documents;
using System.Collections.Generic;

namespace Ewav
{
    public partial class SetLabels : ChildWindow
    {
        SetLabelsViewModel viewModel = new SetLabelsViewModel();
        public string GadgetName { get; set; }
        public SetLabels()
        {
            InitializeComponent();
            FillColorPalletesDropDown();
        }


        public SetLabels(string gadgetName, SetLabelsViewModel viewModel)
        {
            InitializeComponent();
            GadgetName = gadgetName;
            FillColorPalletesDropDown();
            LoadDialog(gadgetName, viewModel);
        }

        public void FillColorPalletesDropDown()
        {
            List<ColorPalleteComboBox> Palletes = new List<ColorPalleteComboBox>();
            Palletes.Add(new ColorPalleteComboBox("../../Images/ColorPalette/Atlantic.png", "Atlantic") { });
            Palletes.Add(new ColorPalleteComboBox("../../Images/ColorPalette/Breeze.png", "Breeze") { });
            Palletes.Add(new ColorPalleteComboBox("../../Images/ColorPalette/Componenart.png", "ComponentArt") { });
            Palletes.Add(new ColorPalleteComboBox("../../Images/ColorPalette/Deep.png", "Deep") { });
            Palletes.Add(new ColorPalleteComboBox("../../Images/ColorPalette/Earth.png", "Earth") { });
            Palletes.Add(new ColorPalleteComboBox("../../Images/ColorPalette/Evergreen.png", "Evergreen") { });
            Palletes.Add(new ColorPalleteComboBox("../../Images/ColorPalette/Heatwave.png", "Heatwave") { });
            Palletes.Add(new ColorPalleteComboBox("../../Images/ColorPalette/Montreal.png", "Montreal") { });
            Palletes.Add(new ColorPalleteComboBox("../../Images/ColorPalette/Pastel.png", "Pastel") { });
            Palletes.Add(new ColorPalleteComboBox("../../Images/ColorPalette/Rennaisance.png", "Renaissance") { });
            Palletes.Add(new ColorPalleteComboBox("../../Images/ColorPalette/Sharepoint.png", "SharePoint") { });
            Palletes.Add(new ColorPalleteComboBox("../../Images/ColorPalette/Study.png", "Study") { });
            Palletes.Add(new ColorPalleteComboBox("../../Images/ColorPalette/VibrantA.png", "VibrantA") { });
            Palletes.Add(new ColorPalleteComboBox("../../Images/ColorPalette/VibrantB.png", "VibrantB") { });
            Palletes.Add(new ColorPalleteComboBox("../../Images/ColorPalette/VibrantC.png", "VibrantC") { });

            cbxColorPalette.ItemsSource = Palletes;
            cbxColorPalette.SelectedIndex = 12;
        }


        bool gadgetsOnly = false;

        public SetLabels(string gadgetName, SetLabelsViewModel viewModel, bool gadgetsOnly = true)
        {
            InitializeComponent();
            this.gadgetsOnly = gadgetsOnly;
            GadgetName = gadgetName;
            LoadGadgetDialog(gadgetName, viewModel);

            spChartProperties.Visibility = System.Windows.Visibility.Collapsed;

        }




        private void LoadGadgetDialog(string gadgetName, SetLabelsViewModel viewModel)
        {


            txtbxGadgetHeader.Text = viewModel.GadgetTitle;
            txtDescription.Text = viewModel.GadgetDescription;



        }


        private void LoadGadgetViewModel(SetLabelsViewModel viewModel)
        {


            viewModel.GadgetTitle = txtbxGadgetHeader.Text;
            viewModel.GadgetDescription = txtDescription.Text;


        }


        private void LoadViewModel(string gadgetName, SetLabelsViewModel viewModel)
        {
            switch (gadgetName.ToLower())
            {
                case "line":
                    viewModel.GadgetName = txtbxGadgetHeader.Text;
                    viewModel.GadgetDescription = txtDescription.Text;

                    viewModel.LegendPostion = ((ComboBoxItem)cbxLegendPosition.SelectedItem).Content.ToString();
                    viewModel.ShowLegend = (bool)cbShowLegend.IsChecked;
                    viewModel.ShowVariableNames = (bool)cbShowLegendVariable.IsChecked;

                    viewModel.CollorPallet = ((Ewav.ColorPalleteComboBox)(cbxColorPalette.SelectedItem)).ImageText;
                    viewModel.Height = nudHeight.Value;
                    viewModel.Width = nudWidth.Value;

                    viewModel.XaxisLabel = txtboxXaxis.Text;
                    viewModel.YaxisLabel = txtboxYaxis.Text;
                    viewModel.ChartTitle = txtbxChrtTitle.Text;
                    break;
                case "stackedcolumn":
                case "column":
                case "bar":
                case "pareto":
                case "epicurve":
                case "pie":
                    viewModel.GadgetName = txtbxGadgetHeader.Text;
                    viewModel.GadgetDescription = txtDescription.Text;

                    viewModel.LegendPostion = ((ComboBoxItem)cbxLegendPosition.SelectedItem).Content.ToString();
                    viewModel.ShowLegend = (bool)cbShowLegend.IsChecked;
                    viewModel.ShowVariableNames = (bool)cbShowLegendVariable.IsChecked;

                    viewModel.SpacesBetweenBars = ((ComboBoxItem)cbxBarSpace.SelectedItem).Content.ToString();
                    viewModel.UseDifferentBarColors = (bool)cbDiffColor.IsChecked;
                    //viewModel.CollorPallet = ((ComboBoxItem)cbxColorPalette.SelectedItem).Content.ToString();
                    viewModel.CollorPallet = ((Ewav.ColorPalleteComboBox)(cbxColorPalette.SelectedItem)).ImageText;
                    viewModel.Height = nudHeight.Value;
                    viewModel.Width = nudWidth.Value;

                    viewModel.XaxisLabel = txtboxXaxis.Text;
                    viewModel.YaxisLabel = txtboxYaxis.Text;
                    viewModel.ChartTitle = txtbxChrtTitle.Text;

                    break;
                case "scatter":
                    viewModel.GadgetName = txtbxGadgetHeader.Text;
                    viewModel.GadgetDescription = txtDescription.Text;
                    viewModel.Height = nudHeight.Value;
                    viewModel.Width = nudWidth.Value;
                    viewModel.CollorPallet = ((Ewav.ColorPalleteComboBox)(cbxColorPalette.SelectedItem)).ImageText;
                    viewModel.XaxisLabel = txtboxXaxis.Text;
                    viewModel.YaxisLabel = txtboxYaxis.Text;
                    viewModel.ChartTitle = txtbxChrtTitle.Text;
                    break;
                //case "aberrationdetection":
                //     viewModel.GadgetName = txtbxGadgetHeader.Text;
                //    viewModel.GadgetDescription = txtDescription.Text;

                //    viewModel.LegendPostion = ((ComboBoxItem)cbxLegendPosition.SelectedItem).Content.ToString();
                //    viewModel.ShowLegend = (bool)cbShowLegend.IsChecked;
                //    viewModel.ShowVariableNames = (bool)cbShowLegendVariable.IsChecked;
                //    viewModel.UseDifferentBarColors = (bool)cbDiffColor.IsChecked;

                //    viewModel.Height = nudHeight.Value;
                //    viewModel.Width = nudWidth.Value;
                //    viewModel.CollorPallet = ((Ewav.ColorPalleteComboBox)(cbxColorPalette.SelectedItem)).ImageText;
                //    break;
                //case "pie":
                //    viewModel.GadgetName = txtbxGadgetHeader.Text;
                //    viewModel.GadgetDescription = txtDescription.Text;

                //    viewModel.ShowLegend = (bool)cbShowLegend.IsChecked;
                //    viewModel.ShowVariableNames = (bool)cbShowLegendVariable.IsChecked;
                //    viewModel.LegendPostion = ((ComboBoxItem)cbxLegendPosition.SelectedItem).Content.ToString();

                //    viewModel.CollorPallet = ((ComboBoxItem)cbxColorPalette.SelectedItem).Content.ToString();
                //    viewModel.UseDifferentBarColors = (bool)cbDiffColor.IsChecked;
                //    viewModel.Height = nudHeight.Value;
                //    viewModel.Width = nudWidth.Value;

                //    viewModel.XaxisLabel = txtboxXaxis.Text;
                //    viewModel.YaxisLabel = txtboxYaxis.Text;
                //    viewModel.ChartTitle = txtbxChrtTitle.Text;

                //    break;
                default:
                    cbDiffColor.Visibility = System.Windows.Visibility.Collapsed;
                    cbShowLegend.Visibility = System.Windows.Visibility.Visible;
                    viewModel.GadgetName = txtbxGadgetHeader.Text;
                    viewModel.LegendPostion = ((ComboBoxItem)cbxLegendPosition.SelectedItem).Content.ToString();
                    viewModel.GadgetName = txtbxGadgetHeader.Text;
                    viewModel.Height = nudHeight.Value;
                    viewModel.Width = nudWidth.Value;
                    viewModel.ShowLegend = (bool)cbShowLegend.IsChecked;
                    viewModel.ShowVariableNames = (bool)cbShowLegendVariable.IsChecked;
                    viewModel.SpacesBetweenBars = ((ComboBoxItem)cbxBarSpace.SelectedItem).Content.ToString();
                    viewModel.CollorPallet = ((Ewav.ColorPalleteComboBox)(cbxColorPalette.SelectedItem)).ImageText;
                    viewModel.XaxisLabel = txtboxXaxis.Text;
                    viewModel.YaxisLabel = txtboxYaxis.Text;
                    viewModel.ChartTitle = txtbxChrtTitle.Text;
                    break;
            }
        }

        private void LoadDialog(string gadgetName, SetLabelsViewModel viewModel)
        {
            switch (gadgetName.ToLower())
            {
                case "line":
                    txtbxGadgetHeader.Text = viewModel.GadgetName;
                    txtDescription.Text = viewModel.GadgetDescription;

                    cbDiffColor.Visibility = System.Windows.Visibility.Collapsed;
                    cbDiffColor.IsChecked = viewModel.UseDifferentBarColors;

                    cbShowLegend.Visibility = System.Windows.Visibility.Visible;
                    cbShowLegend.IsChecked = viewModel.ShowLegend;
                    cbShowLegendVariable.IsChecked = viewModel.ShowVariableNames;

                    nudHeight.Value = viewModel.Height;
                    nudWidth.Value = viewModel.Width;

                    cbxColorPalette.SelectedItem = cbxColorPalette.Items.FirstOrDefault(c => ((Ewav.ColorPalleteComboBox)(c)).ImageText.ToString().ToLower() == viewModel.CollorPallet.ToString().ToLower());
                    cbxLegendPosition.SelectedItem = cbxLegendPosition.Items.SingleOrDefault(c => (c as ComboBoxItem).Content.ToString().ToLower() == viewModel.LegendPostion.ToString().ToLower());
                    cbxBarSpace.Visibility = System.Windows.Visibility.Collapsed;
                    lblSpaceBetweenBars.Visibility = System.Windows.Visibility.Collapsed;

                    txtbxChrtTitle.Text = viewModel.ChartTitle;
                    txtboxXaxis.Text = viewModel.XaxisLabel;
                    txtboxYaxis.Text = viewModel.YaxisLabel;
                    break;
                case "stackedcolumn":
                case "column":
                case "bar":
                case "pareto":
                    txtbxGadgetHeader.Text = viewModel.GadgetName;
                    txtDescription.Text = viewModel.GadgetDescription;

                    cbDiffColor.Visibility = System.Windows.Visibility.Visible;
                    cbDiffColor.IsChecked = viewModel.UseDifferentBarColors;

                    cbShowLegend.Visibility = System.Windows.Visibility.Visible;
                    cbShowLegend.IsChecked = viewModel.ShowLegend;
                    cbxLegendPosition.SelectedItem = viewModel.LegendPostion;

                    nudHeight.Value = viewModel.Height;
                    nudWidth.Value = viewModel.Width;

                    cbShowLegendVariable.IsChecked = viewModel.ShowVariableNames;

                    cbxColorPalette.SelectedItem = cbxColorPalette.Items.FirstOrDefault(c => ((Ewav.ColorPalleteComboBox)(c)).ImageText.ToString().ToLower() == viewModel.CollorPallet.ToString().ToLower());
                    cbxLegendPosition.SelectedItem = cbxLegendPosition.Items.SingleOrDefault(c => (c as ComboBoxItem).Content.ToString().ToLower() == viewModel.LegendPostion.ToString().ToLower());
                    cbxBarSpace.SelectedItem = cbxBarSpace.Items.SingleOrDefault(c => (c as ComboBoxItem).Content.ToString().ToLower() == viewModel.SpacesBetweenBars.ToString().ToLower());
                    txtbxChrtTitle.Text = viewModel.ChartTitle;
                    txtboxXaxis.Text = viewModel.XaxisLabel;
                    txtboxYaxis.Text = viewModel.YaxisLabel;
                    break;

                //txtbxGadgetHeader.Text = viewModel.GadgetName;
                //txtDescription.Text = viewModel.GadgetDescription;

                //cbDiffColor.Visibility = System.Windows.Visibility.Visible;
                //cbDiffColor.IsChecked = viewModel.UseDifferentBarColors;

                //cbShowLegend.Visibility = System.Windows.Visibility.Visible;
                //cbShowLegend.IsChecked = viewModel.ShowLegend;
                //cbxLegendPosition.SelectedItem = cbxLegendPosition.Items.SingleOrDefault(c => (c as ComboBoxItem).Content.ToString().ToLower() == viewModel.LegendPostion.ToString().ToLower());

                //nudHeight.Value = viewModel.Height;
                //nudWidth.Value = viewModel.Width;

                //cbShowLegendVariable.IsChecked = viewModel.ShowVariableNames;

                //cbxBarSpace.SelectedItem = cbxBarSpace.Items.SingleOrDefault(c => (c as ComboBoxItem).Content.ToString().ToLower() == viewModel.SpacesBetweenBars.ToString().ToLower());
                //cbxColorPalette.SelectedItem = cbxColorPalette.Items.SingleOrDefault(c => (c as ComboBoxItem).Content.ToString().ToLower() == viewModel.CollorPallet.ToString().ToLower());
                //txtbxChrtTitle.Text = viewModel.ChartTitle;
                //txtboxXaxis.Text = viewModel.XaxisLabel;
                //txtboxYaxis.Text = viewModel.YaxisLabel;

                //break;
                //case "pareto":
                //txtbxGadgetHeader.Text = viewModel.GadgetName;
                //txtDescription.Text = viewModel.GadgetDescription;

                //cbDiffColor.Visibility = System.Windows.Visibility.Visible;
                //cbDiffColor.IsChecked = viewModel.UseDifferentBarColors;

                //cbShowLegend.Visibility = System.Windows.Visibility.Visible;
                //cbShowLegend.IsChecked = viewModel.ShowLegend;
                //cbxLegendPosition.SelectedItem = viewModel.LegendPostion;

                //nudHeight.Value = viewModel.Height;
                //nudWidth.Value = viewModel.Width;

                //cbShowLegendVariable.IsChecked = viewModel.ShowVariableNames;

                //cbxColorPalette.SelectedItem = cbxColorPalette.Items.SingleOrDefault(c => (c as ComboBoxItem).Content.ToString().ToLower() == viewModel.CollorPallet.ToString().ToLower());
                //cbxLegendPosition.SelectedItem = cbxLegendPosition.Items.SingleOrDefault(c => (c as ComboBoxItem).Content.ToString().ToLower() == viewModel.LegendPostion.ToString().ToLower());
                //cbxBarSpace.SelectedItem = cbxBarSpace.Items.SingleOrDefault(c => (c as ComboBoxItem).Content.ToString().ToLower() == viewModel.SpacesBetweenBars.ToString().ToLower());
                //txtbxChrtTitle.Text = viewModel.ChartTitle;
                //txtboxXaxis.Text = viewModel.XaxisLabel;
                //txtboxYaxis.Text = viewModel.YaxisLabel;
                //break;
                //case "aberrationdetection":
                //    txtbxGadgetHeader.Text = viewModel.GadgetName;
                //    txtDescription.Text = viewModel.GadgetDescription;

                //    cbShowLegend.Visibility = System.Windows.Visibility.Visible;
                //    cbShowLegend.IsChecked = viewModel.ShowLegend;

                //    cbDiffColor.Visibility = System.Windows.Visibility.Collapsed;
                //    cbDiffColor.IsChecked = viewModel.UseDifferentBarColors;

                //    nudHeight.Value = viewModel.Height;
                //    nudWidth.Value = viewModel.Width;
                //    cbxColorPalette.SelectedItem = cbxColorPalette.Items.FirstOrDefault(c => ((Ewav.ColorPalleteComboBox)(c)).ImageText.ToString().ToLower() == viewModel.CollorPallet.ToString().ToLower());
                //    cbxLegendPosition.SelectedItem = cbxLegendPosition.Items.SingleOrDefault(c => (c as ComboBoxItem).Content.ToString().ToLower() == viewModel.LegendPostion.ToString().ToLower());

                //    cbxBarSpace.Visibility = System.Windows.Visibility.Collapsed;
                //    brdChartTitle.Visibility = System.Windows.Visibility.Collapsed;
                //    lblSpaceBetweenBars.Visibility = System.Windows.Visibility.Collapsed;

                //    break;
                case "epicurve":
                    txtbxGadgetHeader.Text = viewModel.GadgetName;
                    txtDescription.Text = viewModel.GadgetDescription;

                    cbDiffColor.Visibility = System.Windows.Visibility.Collapsed;
                    cbDiffColor.IsChecked = viewModel.UseDifferentBarColors;

                    cbShowLegend.Visibility = System.Windows.Visibility.Visible;
                    cbShowLegend.IsChecked = viewModel.ShowLegend;

                    cbxBarSpace.Visibility = System.Windows.Visibility.Collapsed;
                    lblSpaceBetweenBars.Visibility = System.Windows.Visibility.Collapsed;

                    nudHeight.Value = viewModel.Height;
                    nudWidth.Value = viewModel.Width;
                    //cbShowLegend.IsChecked = viewModel.ShowLegend;
                    cbShowLegendVariable.IsChecked = viewModel.ShowVariableNames;
                    cbxColorPalette.SelectedItem = cbxColorPalette.Items.FirstOrDefault(c => ((Ewav.ColorPalleteComboBox)(c)).ImageText.ToString().ToLower() == viewModel.CollorPallet.ToString().ToLower());
                    cbxLegendPosition.SelectedItem = cbxLegendPosition.Items.SingleOrDefault(c => (c as ComboBoxItem).Content.ToString().ToLower() == viewModel.LegendPostion.ToString().ToLower());
                    //cbxBarSpace.SelectedItem = cbxBarSpace.Items.SingleOrDefault(c => (c as ComboBoxItem).Content.ToString().ToLower() == viewModel.SpacesBetweenBars.ToString().ToLower());
                    txtbxChrtTitle.Text = viewModel.ChartTitle;
                    txtboxXaxis.Text = viewModel.XaxisLabel;
                    txtboxYaxis.Text = viewModel.YaxisLabel;
                    break;
                case "scatter":
                    cbDiffColor.Visibility = System.Windows.Visibility.Collapsed;
                    cbDiffColor.IsChecked = viewModel.UseDifferentBarColors;

                    brdChartLegend.Visibility = System.Windows.Visibility.Collapsed;

                    txtbxGadgetHeader.Text = viewModel.GadgetName;

                    txtDescription.Text = viewModel.GadgetDescription;
                    nudHeight.Value = viewModel.Height;
                    nudWidth.Value = viewModel.Width;

                    lblSpaceBetweenBars.Visibility = System.Windows.Visibility.Collapsed;
                    cbxBarSpace.Visibility = System.Windows.Visibility.Collapsed;
                    cbxColorPalette.SelectedItem = cbxColorPalette.Items.FirstOrDefault(c => ((Ewav.ColorPalleteComboBox)(c)).ImageText.ToString().ToLower() == viewModel.CollorPallet.ToString().ToLower());

                    txtbxChrtTitle.Text = viewModel.ChartTitle;
                    txtboxXaxis.Text = viewModel.XaxisLabel;
                    txtboxYaxis.Text = viewModel.YaxisLabel;
                    break;
                //case "aberrationdetection":
                //    cbDiffColor.Visibility = System.Windows.Visibility.Collapsed;
                //    cbDiffColor.IsChecked = false;
                //    cbShowLegend.Visibility = System.Windows.Visibility.Visible;
                //    cbShowLegend.IsChecked = false;
                //    cbxLegendPosition.SelectedItem = viewModel.LegendPostion;
                //    txtbxGadgetHeader.Text = viewModel.GadgetName;
                //    cbxLegendPosition.SelectedItem = viewModel.LegendPostion;
                //    txtbxGadgetHeader.Text = viewModel.GadgetName;
                //    nudHeight.Value = viewModel.Height;
                //    nudWidth.Value = viewModel.Width;
                //    cbShowLegend.IsChecked = viewModel.ShowLegend;
                //    cbShowLegendVariable.IsChecked = viewModel.ShowVariableNames;
                //    cbxBarSpace.SelectedItem = viewModel.SpacesBetweenBars;
                //    break;
                case "pie":
                    cbDiffColor.Visibility = System.Windows.Visibility.Collapsed;
                    cbDiffColor.IsChecked = false;

                    cbShowLegend.Visibility = System.Windows.Visibility.Visible;
                    cbShowLegend.IsChecked = false;
                    cbxLegendPosition.SelectedItem = viewModel.LegendPostion;
                    txtbxGadgetHeader.Text = viewModel.GadgetName;
                    txtDescription.Text = viewModel.GadgetDescription;
                    nudHeight.Value = viewModel.Height;
                    nudWidth.Value = viewModel.Width;
                    cbShowLegend.IsChecked = viewModel.ShowLegend;
                    cbShowLegendVariable.IsChecked = viewModel.ShowVariableNames;
                    cbxColorPalette.SelectedItem = cbxColorPalette.Items.FirstOrDefault(c => ((Ewav.ColorPalleteComboBox)(c)).ImageText.ToString().ToLower() == viewModel.CollorPallet.ToString().ToLower());
                    cbxLegendPosition.SelectedItem = cbxLegendPosition.Items.SingleOrDefault(c => (c as ComboBoxItem).Content.ToString().ToLower() == viewModel.LegendPostion.ToString().ToLower());
                    txtboxXaxis.Visibility = System.Windows.Visibility.Collapsed;
                    txtboxYaxis.Visibility = System.Windows.Visibility.Collapsed;
                    lblxaxis.Visibility = System.Windows.Visibility.Collapsed;
                    lblyaxis.Visibility = System.Windows.Visibility.Collapsed;
                    lblSpaceBetweenBars.Visibility = System.Windows.Visibility.Collapsed;
                    cbxBarSpace.Visibility = System.Windows.Visibility.Collapsed;
                    txtbxChrtTitle.Text = viewModel.ChartTitle;
                    txtboxXaxis.Text = viewModel.XaxisLabel;
                    txtboxYaxis.Text = viewModel.YaxisLabel;
                    break;
                default:
                    cbDiffColor.Visibility = System.Windows.Visibility.Collapsed;
                    cbDiffColor.IsChecked = false;
                    cbShowLegend.Visibility = System.Windows.Visibility.Visible;
                    cbShowLegend.IsChecked = false;
                    cbxLegendPosition.SelectedItem = viewModel.LegendPostion;
                    txtbxGadgetHeader.Text = viewModel.GadgetName;
                    cbxLegendPosition.SelectedItem = viewModel.LegendPostion;
                    txtbxGadgetHeader.Text = viewModel.GadgetName;
                    nudHeight.Value = viewModel.Height;
                    nudWidth.Value = viewModel.Width;
                    cbShowLegend.IsChecked = viewModel.ShowLegend;
                    cbShowLegendVariable.IsChecked = viewModel.ShowVariableNames;
                    cbxColorPalette.SelectedItem = cbxColorPalette.Items.FirstOrDefault(c => ((Ewav.ColorPalleteComboBox)(c)).ImageText.ToString().ToLower() == viewModel.CollorPallet.ToString().ToLower());
                    cbxLegendPosition.SelectedItem = cbxLegendPosition.Items.SingleOrDefault(c => (c as ComboBoxItem).Content.ToString().ToLower() == viewModel.LegendPostion.ToString().ToLower());
                    cbxBarSpace.SelectedItem = cbxBarSpace.Items.SingleOrDefault(c => (c as ComboBoxItem).Content.ToString().ToLower() == viewModel.SpacesBetweenBars.ToString().ToLower());
                    txtbxChrtTitle.Text = viewModel.ChartTitle;
                    txtboxXaxis.Text = viewModel.XaxisLabel;
                    txtboxYaxis.Text = viewModel.YaxisLabel;
                    break;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (gadgetsOnly)
                LoadGadgetViewModel(viewModel);
            else
                LoadViewModel(GadgetName, viewModel);
            this.DataContext = viewModel;
            this.DialogResult = true;
        }

        private void cbxColorPalette_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

    public class ColorPalleteComboBox
    {
        public string ImageURL { get; set; }
        public string ImageText { get; set; }

        public ColorPalleteComboBox(string imageUrl, string imageText, bool isSelected = false)
        {
            this.ImageText = imageText;
            this.ImageURL = imageUrl;
        }
    }
}