/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       FormattedValue.xaml.cs
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
using Ewav.Web.EpiDashboard;
using Ewav.BAL;
using Ewav.Web.EpiDashboard.Rules;
using Ewav.Web.Services;
using System.Globalization;
//Serialize Method is written in Extensions.cs
namespace Ewav
{
    public partial class FormattedValue : ChildWindow, IEwavDashboardRule
    {
        bool editMode = false;
        public ApplicationViewModel applicationViewModel = ApplicationViewModel.Instance;
        EwavRule_Format formatRule = null;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FormattedValue()
        {
            InitializeComponent();
            FillSelectionComboboxes();
        }

        /// <summary>
        /// Parametrized Constructor for Edit functionality.
        /// </summary>
        /// <param name="editMode"></param>
        /// <param name="item"></param>
        public FormattedValue(bool editMode, ListBoxItemSource item)
        {
            this.editMode = editMode;
            InitializeComponent();
            FillSelectionComboboxes();
            IntializeFormattedValue(item);
        }

        /// <summary>
        /// Intializes the control for edit functionality.
        /// </summary>
        /// <param name="item"></param>
        private void IntializeFormattedValue(ListBoxItemSource item)
        {
            for (int i = 0; i < applicationViewModel.EwavDefinedVariables.Count; i++)
            {
                if (item.DestinationColumn == applicationViewModel.EwavDefinedVariables[i].VaraiableName)
                {
                    formatRule = applicationViewModel.EwavDefinedVariables[i] as EwavRule_Format;
                    break;
                }
            }
            txtDestinationField.Text = formatRule.TxtDestinationField;
            txtDestinationField.IsEnabled = false;
            cbxFieldName.SelectedIndex = GetSelectedIndex(cbxFieldName.Items, formatRule.CbxFieldName);
            cbxFieldName.IsEnabled = false;
            cbxFormatOptions.SelectedIndex = GetSelectedIndex(cbxFormatOptions.Items, formatRule.CbxFormatOptions);
        }

        /// <summary>
        /// Retuns the selected index of an item.
        /// </summary>
        /// <param name="itemCollection"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private int GetSelectedIndex(ItemCollection itemCollection, string p)
        {
            int index = -1;
            for (int i = 0; i < itemCollection.Count; i++)
            {
                if (itemCollection[i] is EwavColumn)
                {
                    if (p == ((EwavColumn)itemCollection[i]).Name.ToString())
                    {
                        index = i;
                        break;
                    }

                }
                else
                {
                    if (p == itemCollection[i].ToString())
                    {
                        index = i;
                        break;
                    }
                }
            }
            return index;
        }

        /// <summary>
        /// Fills in the Field Names combo box
        /// </summary>
        private void FillSelectionComboboxes()
        {
            cbxFieldName.Items.Clear();

            List<string> fieldNames = new List<string>();

            // ColumnDataType columnDataType = ColumnDataType.DateTime;

            List<EwavColumn> SourceColumns = this.applicationViewModel.EwavSelectedDatasource.AllColumns;

            List<ColumnDataType> columnDataType = new List<ColumnDataType>();
            columnDataType.Add(ColumnDataType.DateTime);

            IEnumerable<EwavColumn> CBXFieldCols = from cols in SourceColumns
                                                   where columnDataType.Contains(cols.SqlDataTypeAsString)
                                                   orderby cols.Name
                                                   select cols;

            List<EwavColumn> colsList = CBXFieldCols.ToList();

            //colsList.Insert(0, new EwavColumn() { Name = " ", Index = -1 });
            cbxFieldName.ItemsSource = colsList;
            cbxFieldName.SelectedValue = "Index";
            cbxFieldName.DisplayMemberPath = "Name";
            cbxFieldName.SelectedIndex = 0;

            if (editMode)
            {
                cbxFieldName.SelectedItem = null;
            }
        }

        /// <summary>
        /// Gets the appropriate FormatType based on the selected format option
        /// </summary>
        /// <returns>FormatType</returns>
        private FormatTypes GetFormatType()
        {
            #region Input Validation
            if (cbxFormatOptions.SelectedItem == null)
            {
                //throw new ApplicationException("No value selected for the format option.");
            }
            #endregion // Input Validation

            FormatTypes formatType = FormatTypes.Day;

            string option = cbxFormatOptions.SelectedItem.ToString();

            switch (option)
            {
                case "an integer":
                    formatType = FormatTypes.NumericInteger;
                    break;
                case "a decimal with one digit":
                    formatType = FormatTypes.NumericDecimal1;
                    break;
                case "a decimal with two digits":
                    formatType = FormatTypes.NumericDecimal2;
                    break;
                case "a decimal with three digits":
                    formatType = FormatTypes.NumericDecimal3;
                    break;
                case "a decimal with four digits":
                    formatType = FormatTypes.NumericDecimal4;
                    break;
                case "a decimal with five digits":
                    formatType = FormatTypes.NumericDecimal5;
                    break;
                case "the day":
                    formatType = FormatTypes.Day;
                    break;
                case "the day name":
                    formatType = FormatTypes.FullDayName;
                    break;
                case "the abbreviated day name":
                    formatType = FormatTypes.ShortDayName;
                    break;
                case "the month":
                    formatType = FormatTypes.Month;
                    break;
                case "the month and four-digit year":
                    formatType = FormatTypes.MonthAndFourDigitYear;
                    break;
                case "the month name":
                    formatType = FormatTypes.FullMonthName;
                    break;
                case "the abbreviated month name":
                    formatType = FormatTypes.ShortMonthName;
                    break;
                case "the standard date":
                    formatType = FormatTypes.RegularDate;
                    break;
                case "the long date":
                    formatType = FormatTypes.LongDate;
                    break;
                case "the epi week":
                    formatType = FormatTypes.EpiWeek;
                    break;
                case "the four-digit year":
                    formatType = FormatTypes.FourDigitYear;
                    break;
                case "the two-digit year":
                    formatType = FormatTypes.TwoDigitYear;
                    break;
                case "the RFC 1123 date":
                    formatType = FormatTypes.RFC1123;
                    break;
                case "the sortable date":
                    formatType = FormatTypes.SortableDateTime;
                    break;
                case "the hour":
                    formatType = FormatTypes.Hours;
                    break;
            }

            return formatType;
        }

        /// <summary>
        /// Taken from EpiInfo. Fills cbxFormatOptions combobox.
        /// </summary>
        private void FillDateFormatOptions()
        {
            cbxFormatOptions.Items.Clear();
            cbxFormatOptions.Items.Add("the day");
            cbxFormatOptions.Items.Add("the day name");
            cbxFormatOptions.Items.Add("the abbreviated day name");
            cbxFormatOptions.Items.Add("the month");
            cbxFormatOptions.Items.Add("the month and four-digit year");
            cbxFormatOptions.Items.Add("the month name");
            cbxFormatOptions.Items.Add("the abbreviated month name");
            cbxFormatOptions.Items.Add("the standard date");
            cbxFormatOptions.Items.Add("the long date");
            cbxFormatOptions.Items.Add("the epi week");
            cbxFormatOptions.Items.Add("the four-digit year");
            cbxFormatOptions.Items.Add("the two-digit year");
            cbxFormatOptions.Items.Add("the hour");
            cbxFormatOptions.Items.Add("the RFC 1123 date");
            cbxFormatOptions.Items.Add("the sortable date");
        }

        /// <summary>
        /// Taken from EpiInfo. Fills cbxFormatOptions combobox.
        /// </summary>
        private void FillNumberFormatOptions()
        {
            cbxFormatOptions.Items.Clear();
            cbxFormatOptions.Items.Add("an integer");
            cbxFormatOptions.Items.Add("a decimal with one digit");
            cbxFormatOptions.Items.Add("a decimal with two digits");
            cbxFormatOptions.Items.Add("a decimal with three digits");
            cbxFormatOptions.Items.Add("a decimal with four digits");
            cbxFormatOptions.Items.Add("a decimal with five digits");
        }

        /// <summary>
        /// Handles the addition of rule in EwavDefinedVariables 
        /// addition of rule in ListOfRules used to bind ListBox
        /// addition of newly created column.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            List<ListBoxItemSource> TempList = applicationViewModel.ListOfRules.Where(t => t.DestinationColumn.ToLower() == txtDestinationField.Text.ToLower()).ToList();

            Boolean columnNameExists = false;
            columnNameExists = applicationViewModel.EwavSelectedDatasource.AllColumns.Any(c => c.Name.ToLower() == txtDestinationField.Text.ToLower());

            if ((TempList.Count > 0 && !editMode) || columnNameExists)
            {
                MessageBox.Show("Variable name already exists.");
                return;
            }


            if (string.IsNullOrEmpty(txtDestinationField.Text))
            {
                MessageBox.Show("Destination field is blank.");
                //this.DialogResult = DialogResult.None;
                return;
            }
            if (cbxFormatOptions.SelectedIndex < 0)
            {
                MessageBox.Show("No format options selected.");
                //this.DialogResult = DialogResult.None;
                return;
            }

            string friendlyLabel = "Format the display of " + ((EwavColumn)cbxFieldName.SelectedItem).Name.ToString() + " to show " + cbxFormatOptions.SelectedItem.ToString() + " and place the formatted values in " + txtDestinationField.Text;
            //, cbxFieldName.SelectedItem.ToString(), txtDestinationField.Text, cbxFormatOptions.SelectedItem.ToString(), GetFormatType());


            EwavRule_Format rule = new EwavRule_Format();

            rule.FriendlyLabel = friendlyLabel;
            rule.CbxFieldName = ((EwavColumn)cbxFieldName.SelectedItem).Name;
            rule.TxtDestinationField = txtDestinationField.Text;
            rule.FormatTypes = GetFormatType();
            rule.CbxFormatOptions = cbxFormatOptions.SelectedItem.ToString();
            rule.VaraiableName = txtDestinationField.Text;
            rule.VaraiableDataType = ColumnDataType.Text.ToString();

            ListBoxItemSource listBoxItem = new ListBoxItemSource();
            listBoxItem.RuleString = friendlyLabel;
            //listBoxItem.SourceColumn = sourceColumn.Name;
            listBoxItem.DestinationColumn = txtDestinationField.Text;
            listBoxItem.RuleType = EwavRuleType.Formatted;
            listBoxItem.Rule = rule;
            listBoxItem.SourceColumn = null;

            EwavColumn newcolumn = new EwavColumn();
            newcolumn.Name = txtDestinationField.Text;
            newcolumn.SqlDataTypeAsString = ColumnDataType.Text;
            newcolumn.NoCamelName = txtDestinationField.Text;
            newcolumn.IsUserDefined = true;

            applicationViewModel.InvokePreColumnChangedEvent();

            List<EwavRule_Base> rules = new List<EwavRule_Base>();
            rules = applicationViewModel.EwavDefinedVariables;

            //Shows the error message if name already exists.
            if (!editMode)
            {
                for (int i = 0; i < applicationViewModel.EwavDefinedVariables.Count; i++)
                {
                    if (applicationViewModel.EwavDefinedVariables[i].VaraiableName == rule.VaraiableName)
                    {
                        MessageBox.Show("Rule Name already exists. Select another name.");
                        return;
                    }
                }
            }


            for (int i = 0; i < rules.Count; i++)
            {
                if (rule.TxtDestinationField == rules[i].VaraiableName)
                {
                    rules.RemoveAt(i);
                    applicationViewModel.ListOfRules.RemoveAt(i);
                    break;
                }
            }

            if (!editMode)
            {
                applicationViewModel.EwavSelectedDatasource.AllColumns.Add(newcolumn);
            }
            applicationViewModel.ListOfRules.Add(listBoxItem);
            List<EwavRule_Base> listOfRules = new List<EwavRule_Base>();
            listOfRules = applicationViewModel.EwavDefinedVariables;
            listOfRules.Add(rule);

            applicationViewModel.EwavDefinedVariables = listOfRules;
            this.DialogResult = true;
        }

        /// <summary>
        /// Handles the Click Event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// Handles Selection Changed event for cbxFieldName.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxFieldName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            cbxFormatOptions.Items.Clear();

            //Configuration config = dashboardHelper.Config;

            if (cbxFieldName.SelectedIndex == -1)
            {
                return;
            }

            if (((EwavColumn)cbxFieldName.SelectedItem).SqlDataTypeAsString == ColumnDataType.Numeric)
            {
                FillNumberFormatOptions();
            }
            else if (((EwavColumn)cbxFieldName.SelectedItem).SqlDataTypeAsString == ColumnDataType.DateTime)
            {
                FillDateFormatOptions();
            }

            if (editMode)
            {
                //cbxFormatOptions.SelectedItem = FormatRule.FormatString;
            }
            else
            {
                txtDestinationField.Text = ((EwavColumn)cbxFieldName.SelectedItem).Name.ToString() + "_FORMATTED";
            }

        }

        /// <summary>
        /// Handles the SelectionChanged Event for cbxFormatOptions.
        /// Code for EpiWeek is commented as being out of scope.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxFormatOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxFormatOptions.SelectedIndex >= 0)
            {
                FormatTypes type = GetFormatType();

                if (type != FormatTypes.EpiWeek)
                {
                    //EwavRule_Format tempFormatRule = new EwavRule_Format();//dashboardHelper, "temp", "temp", "temp", "temp", type);
                    string formatString = GetFormatString(type);
                    txtPreview.Text = string.Format(System.Globalization.CultureInfo.CurrentCulture, formatString, DateTime.Now);
                }
                else
                {
                    //StatisticsRepository.EpiWeek epiWeek = new StatisticsRepository.EpiWeek();
                    txtPreview.Text = GetEpiWeek(DateTime.Now).ToString().Trim();
                }
            }

        }

        /*
         * Following two methods are taken from Statistics Repository.
         */
        #region StatisticsRepo Methods
        //private object GetMMWRStart(System.DateTime dteDateIn)
        //{
        //    object functionReturnValue = null;

        //    //   GetMMWRStart returns the date of the start of the MMWR year closest to Jan 01
        //    //   of the year passed in.  It finds 01/01/yyyy first then moves forward or back
        //    //   the correct number of days to be the start of the MMWR year.  MMWR Week #1 is 
        //    //   always the first week of the year that has a minimum of 4 days in the new year.
        //    //   If Jan. first falls on a Thurs, Fri, or Sat, the MMWRStart date returned could be
        //    //   greater than the date passed in so this must be checked for by the calling Sub.

        //    //   If Jan. first is a Mon, Tues, or Wed, the MMWRStart goes back to the last
        //    //   Sunday in Dec of the previous year which is the start of MMWR Week 1 for the
        //    //   current year.

        //    //   If the first of January is a Thurs, Fri, or Sat, the MMWRStart goes forward to 
        //    //   the first Sunday in Jan of the current year which is the start of
        //    //   MMWR Week 1 for the current year.  For example, if the year passed
        //    //   in was 01/02/1998, a Friday, the MMWRStart that is returned is 01/04/1998, a Sunday.  
        //    //   Since 01/04/1998 > 01/02/1998, we must subract a year and pass Jan 1 of the new
        //    //   year into this function as in GetMMWRStart("01/01/1997").
        //    //   The MMWRStart date would then be returned as the date of the first
        //    //   MMWR Week of the previous year.    

        //    System.DateTime dteYrBegin = default(System.DateTime);
        //    DayOfWeek dblDayOfWeek = 0;
        //    dteYrBegin = Convert.ToDateTime("01/01/" + Convert.ToString(dteDateIn.Year));
        //    dblDayOfWeek = dteYrBegin.DayOfWeek; //DateAndTime.Weekday(dteYrBegin);

        //    if (dblDayOfWeek <= DayOfWeek.Wednesday) //Wednesday
        //    {
        //        //functionReturnValue = DateAndTime.DateAdd("d", -(dblDayOfWeek - 1), dteYrBegin);
        //        functionReturnValue = dteYrBegin.AddDays(-((int)dblDayOfWeek - 1));

        //    }
        //    else
        //    {
        //        //functionReturnValue = DateAndTime.DateAdd("d", ((7 - dblDayOfWeek) + 1), dteYrBegin);
        //        functionReturnValue = dteYrBegin.AddDays(((7 - (int)dblDayOfWeek) + 1));
        //    }
        //    return functionReturnValue;
        //}


        //public int GetEpiWeek(System.DateTime InputDate)
        //{
        //    int functionReturnValue = 0;
        //    string strAnswer = null;
        //    System.DateTime dteStart = default(System.DateTime);
        //    long lngYear = 0;
        //    string strYear = null;
        //    System.DateTime dteQDate = default(System.DateTime);
        //    System.DateTime dteQAccept = default(System.DateTime);
        //    //System.DateTime dteWkStart = default(System.DateTime);
        //    //System.DateTime dteWkEnd = default(System.DateTime);
        //    System.DateTime dteEndOfQYr = default(System.DateTime);
        //    int intMmwrWk = 0;
        //    //int intMmwrNow = 0;
        //    //int intMmwrMax = 0;
        //    DayOfWeek intEndOfYrDay = 0;

        //    dteQDate = InputDate;

        //    // The following lines of code make sure that if a NULL (blank) date is passed into this function
        //    // from Epi Info, that we don't cause an error to appear in Epi Info. Instead, we return a null
        //    // value and exit the function.
        //    if (InputDate == null)
        //    {
        //        functionReturnValue = 1;
        //        return functionReturnValue;
        //    }

        //    dteQAccept = dteQDate;

        //    // get the year
        //    lngYear = dteQAccept.Year; //DateAndTime.Year(dteQAccept);

        //    // convert the year to a string
        //    strYear = Convert.ToString(lngYear);

        //    string sdp = null;
        //    sdp = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;

        //    if (sdp.ToLower().StartsWith("d"))
        //    {
        //        dteEndOfQYr = Convert.ToDateTime("31/12/" + strYear);
        //    }
        //    else
        //    {
        //        dteEndOfQYr = Convert.ToDateTime("12/31/" + strYear);
        //    }


        //    intEndOfYrDay = dteEndOfQYr.DayOfWeek;//DateAndTime.Weekday(dteEndOfQYr);

        //    if (intEndOfYrDay < DayOfWeek.Wednesday) //Wednesday
        //    {
        //        //if ((DateAndTime.DateDiff("d", dteQAccept, dteEndOfQYr) < intEndOfYrDay))
        //        if ((Convert.ToInt16((dteEndOfQYr.Subtract(dteQAccept).ToString("dd"))) < (int)intEndOfYrDay))
        //        {
        //            dteQAccept = Convert.ToDateTime("01/01/" + Convert.ToString(lngYear + 1));
        //        }
        //    }

        //    dteStart = (DateTime)GetMMWRStart(dteQAccept);
        //    if (dteStart > dteQAccept)
        //    {
        //        dteStart = (DateTime)GetMMWRStart(Convert.ToDateTime("01/01/" + Convert.ToString(lngYear - 1)));
        //    }
        //    //intMmwrWk = 1 + DateAndTime.DateDiff("w", dteStart, dteQAccept);
        //    intMmwrWk = 1 + (dteQAccept - dteStart).Days / 7;
        //    strAnswer = Convert.ToString(intMmwrWk);
        //    if (strAnswer.Length < 2)
        //        strAnswer = "0" + strAnswer;

        //    functionReturnValue = Convert.ToInt32(strAnswer);
        //    return functionReturnValue;
        //}

        public int GetEpiWeek(DateTime currentDate)
        {
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            //Calendar calendar = cultureInfo.Calendar;
            CalendarWeekRule calenderWeekRule = CalendarWeekRule.FirstFourDayWeek;
            DayOfWeek firstDayOfWeek = DayOfWeek.Sunday;
            int weekNumber =
              cultureInfo.Calendar.GetWeekOfYear(currentDate,
                calenderWeekRule, firstDayOfWeek);
            if (weekNumber.Equals(53))
            {
                // The week number appears to be 53. 
                // If so, the week number of the next week must be 1.
                // Obtain the week number of the next week and check if this is 1.
                int weekNumberNext =
                  cultureInfo.Calendar.GetWeekOfYear(currentDate.AddDays(7),
                    calenderWeekRule, firstDayOfWeek);
                if (!weekNumberNext.Equals(1))
                {
                    // The next week has week number 2.
                    // Thus, the current week number is not 53 but 1.
                    weekNumber = 1;
                }
            }
            return weekNumber;
        }


        #endregion

        /// <summary>
        /// Gets the format string for this rule
        /// </summary>
        public string GetFormatString(FormatTypes type)
        {
            string formatString = string.Empty;
            switch (type)
            {
                case FormatTypes.EpiWeek:
                    formatString = "epiweek"; // note: Special case scenario
                    break;
                case FormatTypes.Day:
                    formatString = "{0:dd}";
                    break;
                case FormatTypes.ShortDayName:
                    formatString = "{0:ddd}";
                    break;
                case FormatTypes.FullDayName:
                    formatString = "{0:dddd}";
                    break;
                case FormatTypes.FourDigitYear:
                    formatString = "{0:yyyy}";
                    break;
                case FormatTypes.TwoDigitYear:
                    formatString = "{0:yy}";
                    break;
                case FormatTypes.DayMonth:
                    formatString = "{0:M}";
                    break;
                case FormatTypes.Month:
                    formatString = "{0:MM}";
                    break;
                case FormatTypes.ShortMonthName:
                    formatString = "{0:MMM}";
                    break;
                case FormatTypes.FullMonthName:
                    formatString = "{0:MMMM}";
                    break;
                case FormatTypes.RFC1123:
                    formatString = "{0:r}";
                    break;
                case FormatTypes.RegularDate:
                    formatString = "{0:d}";
                    break;
                case FormatTypes.LongDate:
                    formatString = "{0:D}";
                    break;
                case FormatTypes.SortableDateTime:
                    formatString = "{0:s}";
                    break;
                case FormatTypes.MonthYear:
                    formatString = "{0:y}";
                    break;
                case FormatTypes.Hours:
                    formatString = "{0:HH}";
                    break;
                case FormatTypes.HoursMinutes:
                    formatString = "{0:t}";
                    break;
                case FormatTypes.HoursMinutesSeconds:
                    formatString = "{0:T}";
                    break;
                case FormatTypes.NumericInteger:
                    formatString = "{0:0}";
                    break;
                case FormatTypes.NumericDecimal1:
                    formatString = "{0:0.0}";
                    break;
                case FormatTypes.NumericDecimal2:
                    formatString = "{0:0.00}";
                    break;
                case FormatTypes.NumericDecimal3:
                    formatString = "{0:0.000}";
                    break;
                case FormatTypes.NumericDecimal4:
                    formatString = "{0:0.0000}";
                    break;
                case FormatTypes.NumericDecimal5:
                    formatString = "{0:0.00000}";
                    break;
                case FormatTypes.MonthAndFourDigitYear:
                    formatString = "{0:y}";
                    break;
            }

            return formatString;
        }

        void IEwavDashboardRule.CreateFromXml(System.Xml.Linq.XElement element)
        {
        }
    }
}