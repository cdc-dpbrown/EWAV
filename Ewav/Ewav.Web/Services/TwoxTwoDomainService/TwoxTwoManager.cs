/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       TwoxTwoManager.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Epi.Fields;
using Ewav.Web.EpiDashboard;

namespace Ewav.Web.Services
{
    /// <summary>
    /// TwoxTwoManager  
    /// </summary>
    public class TwoxTwoManager
    {
        #region  Fields
        public StringLiterals thisStringLiterals = new StringLiterals();
        private List<MxNGridSetupParameter> mxNGridSetupParameters = new List<MxNGridSetupParameter>();
        private int MaxColumns;
        private List<MxNSetTextParameter> mxNSetTextParameters = new List<MxNSetTextParameter>();
        private List<MxNGridRow> mxNGridRows = new List<MxNGridRow>();
        private bool smartTable;
        #endregion

        public TwoxTwoAndMxNResultsSet SetupGadget(GadgetParameters clientGadgetOptions, List<EwavDataFilterCondition> ewavDataFilters, List<EwavRule_Base> rules)
        {
            TwoxTwoAndMxNResultsSet resultSet = this.EvaluateData(clientGadgetOptions, ewavDataFilters, rules);
            if (resultSet.Errors.Count > 0 || resultSet.FreqResultsDataTable == null)
            {
                return resultSet;
            }

            if (resultSet.FreqResultsDataTable.Rows.Count == 2 && resultSet.FreqResultsDataTable.Columns.Count == 3)
            {
                return resultSet;
            }
            else
            {
                resultSet = this.SetupMxN(clientGadgetOptions, resultSet);
                return resultSet;
            }
        }

        public TwoxTwoAndMxNResultsSet EvaluateData(GadgetParameters clientGadgetOptions, List<EwavDataFilterCondition> ewavDataFilters, List<EwavRule_Base> rules)
        {
            TwoxTwoAndMxNResultsSet twoxTwoResultsSet = new TwoxTwoAndMxNResultsSet();
            bool exceededMaxColumns = false;
            twoxTwoResultsSet.exceededMaxColumns = exceededMaxColumns;
            
            try
            {
                GadgetParameters gadgetOptions = new GadgetParameters();
                gadgetOptions = Utilities.Clone<GadgetParameters>(clientGadgetOptions);
                gadgetOptions.ShouldIncludeFullSummaryStatistics = false;
                gadgetOptions.ShouldSortHighToLow = false;
                gadgetOptions.ShouldUseAllPossibleValues = false;
                gadgetOptions.StrataVariableNames = new List<string>();

                if (gadgetOptions.InputVariableList.ContainsKey("smarttable"))
                {
                    smartTable = Convert.ToBoolean(gadgetOptions.InputVariableList["smarttable"].ToString());
                }

                DashboardHelper dashboardHelper = new DashboardHelper(gadgetOptions, ewavDataFilters, rules);

                var candidateRules = rules.Where(rule => rule.VaraiableName == gadgetOptions.MainVariableName); // before push > deprecate the use of MainVariableName

                List<string> groupVariables = new List<string>();

                if(candidateRules.Count() > 0)
                {
                    var groupVariableCandidate = candidateRules.ToList()[0]; // before push > interate through list

                    if (groupVariableCandidate.VaraiableDataType == "GroupVariable")
                    {
                        groupVariables = ((EwavRule_GroupVariable)groupVariableCandidate).Items.Select(i => i.VarName).ToList();
                    }
                }

                List<string> allGroupFields = dashboardHelper.GetAllGroupsAsList();

                string firstColumnName = string.Empty;
                int columnCount = gadgetOptions.ColumnNames.Count;
                if(gadgetOptions.ColumnNames.Count() > 0)
                {
                    firstColumnName = gadgetOptions.ColumnNames[0].VarName;
                }

                Dictionary<DataTable, List<DescriptiveStatistics>> stratifiedFrequencyTables = new Dictionary<DataTable, List<DescriptiveStatistics>>();


                if(groupVariables.Count == 0)
                {
                    groupVariables.Add(gadgetOptions.MainVariableName);
                }

                gadgetOptions.MainVariableNames = new List<string>();

                Dictionary<DataTable, List<DescriptiveStatistics>> tableDictionary = new Dictionary<DataTable, List<DescriptiveStatistics>>();
                foreach (string groupVariableName in groupVariables)
                {
                    gadgetOptions.MainVariableNames.Add(groupVariableName);
                    gadgetOptions.MainVariableName = groupVariableName;

                    tableDictionary = dashboardHelper.GenerateFrequencyTable(gadgetOptions);

                    if(tableDictionary.Count > 0)
                    {
                        foreach (KeyValuePair<DataTable, List<DescriptiveStatistics>> kvp in tableDictionary)
                        {
                            stratifiedFrequencyTables.Add(kvp.Key, kvp.Value);
                        }

                        if (tableDictionary.Count > 0)
                        {
                            var table = tableDictionary.First().Key;

                            if (table.Rows.Count == 2 && table.Columns.Count == 3)
                            {
                                twoxTwoResultsSet.Is2x2 = true;
                            }
                            else
                            {
                                twoxTwoResultsSet.Is2x2 = false;
                            }
                        }
                    }
                }

                if (stratifiedFrequencyTables == null || stratifiedFrequencyTables.Count == 0)
                {
                    return twoxTwoResultsSet;
                }
                else
                {
                    List<DescriptiveStatistics> statsList = new List<DescriptiveStatistics>();
                    DataTable table;

                    foreach (KeyValuePair<DataTable, List<DescriptiveStatistics>> tableKvp in stratifiedFrequencyTables)
                    {
                        statsList = tableKvp.Value;
                        table = tableKvp.Key;

                        CreateSmartTable(table);

                        twoxTwoResultsSet.FreqResultsDataTable = table;

                        TwoxTwoTableDTO twoxTwoTableDTO = this.CreateDTO(gadgetOptions, dashboardHelper, table);
                        twoxTwoResultsSet.TwoxTwoTableDTO = twoxTwoTableDTO;
                        twoxTwoResultsSet = this.Setup2x2(clientGadgetOptions, twoxTwoResultsSet);

                        twoxTwoResultsSet.AddResult(table, gadgetOptions.CrosstabVariableName, statsList, twoxTwoResultsSet.GridCells);

                        double count = 0;
                        foreach (DescriptiveStatistics ds in tableKvp.Value)
                        {
                            count = count + ds.Observations;
                        }

                        if (count == 0 && stratifiedFrequencyTables.Count == 1)
                        {
                            return twoxTwoResultsSet;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                twoxTwoResultsSet.Errors.Add(new MyString(ex.Message));
            }
            return twoxTwoResultsSet;
        }


        private void CreateSmartTable(DataTable table)
        {
            if (smartTable && table.Columns.Count == 3 && table.Rows.Count == 2)
            {
                Dictionary<string, string> booleanValues = new Dictionary<string, string>();
                booleanValues.Add("0", "1");
                booleanValues.Add("false", "true");
                booleanValues.Add("f", "t");
                booleanValues.Add("n", "y");

                if (!booleanValues.ContainsKey(Ewav.Web.Config.ConfigDataSet.RepresentationOfNo.ToLower()))
                {
                    booleanValues.Add(Ewav.Web.Config.ConfigDataSet.RepresentationOfNo.ToLower(), Ewav.Web.Config.ConfigDataSet.RepresentationOfYes.ToLower());
                }

                string firstColumnName = table.Columns[1].ColumnName.ToLower();
                string secondColumnName = table.Columns[2].ColumnName.ToLower();

                if (booleanValues.ContainsKey(firstColumnName) && secondColumnName.Equals(booleanValues[firstColumnName]))
                {
                    Swap2x2ColValues(table);
                }

                string firstRowName = table.Rows[0][0].ToString();
                string secondRowName = table.Rows[1][0].ToString();

                if (booleanValues.ContainsKey(firstRowName) && secondRowName.Equals(booleanValues[firstRowName]))
                {
                    Swap2x2RowValues(table);
                }
            }
        }

        //void Swap2x2RowValues(DataTable table)
        //{
        //    if (table.Rows.Count > 2 || table.Columns.Count > 3)
        //    {
        //        return; // cannot do an invalid 2x2 table
        //    }

        //    object row1Col1 = table.Rows[0][1];
        //    object row1Col2 = table.Rows[0][2];

        //    object row2Col1 = table.Rows[1][1];
        //    object row2Col2 = table.Rows[1][2];

        //    table.Rows[0][1] = row2Col1;
        //    table.Rows[0][2] = row2Col2;

        //    table.Rows[1][1] = row1Col1;
        //    table.Rows[1][2] = row1Col2;

        //    object firstRowName = table.Rows[0][0];
        //    table.Rows[0][0] = table.Rows[1][0];
        //    table.Rows[1][0] = firstRowName;
        //}

        //void Swap2x2ColValues(DataTable table)
        //{
        //    if (table.Rows.Count > 2 || table.Columns.Count > 3)
        //    {
        //        return; // cannot do an invalid 2x2 table
        //    }

        //    object row1Col1 = table.Rows[0][1];
        //    object row1Col2 = table.Rows[0][2];

        //    object row2Col1 = table.Rows[1][1];
        //    object row2Col2 = table.Rows[1][2];

        //    table.Rows[0][1] = row1Col2;
        //    table.Rows[0][2] = row1Col1;

        //    table.Rows[1][1] = row2Col2;
        //    table.Rows[1][2] = row2Col1;

        //    string firstColumnName = table.Columns[1].ColumnName;
        //    string secondColumnName = table.Columns[2].ColumnName;

        //    table.Columns[1].ColumnName = "_COL1_";
        //    table.Columns[2].ColumnName = "_COL2_";

        //    table.Columns[1].ColumnName = secondColumnName;
        //    table.Columns[2].ColumnName = firstColumnName;
        //}

        private TwoxTwoAndMxNResultsSet Setup2x2(GadgetParameters gadgetOptions, TwoxTwoAndMxNResultsSet resultSet)
        {
            GridCells gridCells = new GridCells();

            TwoxTwoTableDTO table = resultSet.TwoxTwoTableDTO;

            DataTable dt = resultSet.FreqResultsDataTable;

            if (table != null)  //      && table.Rows.Length != 0)
            {
                //if (gadgetOptions.InputVariableList.ContainsKey("smarttable") && gadgetOptions.InputVariableList["smarttable"].Equals("true"))
                //{
                //    CheckLabels(table);
                //}


                gridCells.yyVal = Convert.ToInt32(dt.Rows[0][1].ToString());//table.Yy;
                gridCells.ynVal = Convert.ToInt32(dt.Rows[0][2].ToString());//table.Yn;
                gridCells.nyVal = Convert.ToInt32(dt.Rows[1][1].ToString());//table.Ny;
                gridCells.nnVal = Convert.ToInt32(dt.Rows[1][2].ToString()); //table.Nn;

                gridCells.ntVal = gridCells.nnVal + gridCells.nyVal;
                gridCells.ytVal = gridCells.yyVal + gridCells.ynVal;

                gridCells.ttVal = gridCells.ytVal + gridCells.ntVal;
                gridCells.tyVal = gridCells.yyVal + gridCells.nyVal;
                gridCells.tnVal = gridCells.ynVal + gridCells.nnVal;
                if (gridCells.ytVal != 0)
                {
                    gridCells.yyRowPct = 1.0 * gridCells.yyVal / gridCells.ytVal;
                    gridCells.ynRowPct = 1.0 * gridCells.ynVal / gridCells.ytVal;
                }
                if (gridCells.ntVal != 0)
                {
                    gridCells.nyRowPct = 1.0 * gridCells.nyVal / gridCells.ntVal;
                    gridCells.nnRowPct = 1.0 * gridCells.nnVal / gridCells.ntVal;
                }
                if (gridCells.tyVal != 0)
                {
                    gridCells.yyColPct = 1.0 * gridCells.yyVal / gridCells.tyVal;
                    gridCells.nyColPct = 1.0 * gridCells.nyVal / gridCells.tyVal;
                }
                if (gridCells.tnVal != 0)
                {
                    gridCells.ynColPct = 1.0 * gridCells.ynVal / gridCells.tnVal;
                    gridCells.nnColPct = 1.0 * gridCells.nnVal / gridCells.tnVal;
                }
                if (gridCells.ttVal != 0)
                {
                    gridCells.tyRowPct = 1.0 * gridCells.tyVal / gridCells.ttVal;
                    gridCells.tnRowPct = 1.0 * gridCells.tnVal / gridCells.ttVal;
                    gridCells.ytColPct = 1.0 * gridCells.ytVal / gridCells.ttVal;
                    gridCells.ntColPct = 1.0 * gridCells.ntVal / gridCells.ttVal;
                }
                if (gridCells.tyVal > 0 && gridCells.tnVal > 0 && gridCells.ytVal > 0 && gridCells.ntVal > 0)
                {
                    //gridCells.singleTableResults = new StatisticsRepository.cTable().SigTable(
                    //    (double)gridCells.yyVal, (double)gridCells.ynVal,
                    //    (double)gridCells.nyVal, (double)gridCells.nnVal, 0.95);
                    StatisticsRepository.cTable.SingleTableResults stab = new StatisticsRepository.cTable().SigTable(
                        (double)gridCells.yyVal, (double)gridCells.ynVal,
                        (double)gridCells.nyVal, (double)gridCells.nnVal, 0.95);

                    //gridCells.singleTableResults = new MySingleTableResults()
                    //{
                    //     ChiSquareMantel2P =  stab.ChiSquareMantel2P, 
                    //      ChiSquareUncorrected2P = stab.ChiSquareUncorrected2P, 
                    //       ChiSquareMantelVal = stab.ChiSquareMantel2P     

                    //}  

                    gridCells.singleTableResults = new MySingleTableResults()
                    {
                        ChiSquareMantel2P = stab.ChiSquareMantel2P,
                        ChiSquareMantelVal = stab.ChiSquareMantelVal,
                        ChiSquareUncorrected2P = stab.ChiSquareUncorrected2P,
                        ChiSquareUncorrectedVal = stab.ChiSquareUncorrectedVal,
                        ChiSquareYates2P = stab.ChiSquareYates2P,
                        ChiSquareYatesVal = stab.ChiSquareYatesVal,
                        ErrorMessage = stab.ErrorMessage,
                        FisherExact2P = stab.FisherExact2P,
                        FisherExactP = stab.FisherExactP,
                        MidP = stab.MidP,
                        OddsRatioEstimate = stab.OddsRatioEstimate,
                        OddsRatioLower = stab.OddsRatioLower,
                        OddsRatioMLEEstimate = stab.OddsRatioMLEEstimate,
                        OddsRatioMLEFisherLower = stab.OddsRatioMLEFisherLower,
                        OddsRatioMLEFisherUpper = stab.OddsRatioMLEFisherUpper,
                        OddsRatioMLEMidPLower = stab.OddsRatioMLEMidPLower,
                        OddsRatioMLEMidPUpper = stab.OddsRatioMLEMidPUpper,
                        OddsRatioUpper = stab.OddsRatioUpper,
                        RiskDifferenceEstimate = stab.RiskDifferenceEstimate,
                        RiskDifferenceLower = stab.RiskDifferenceLower,
                        RiskDifferenceUpper = stab.RiskDifferenceUpper,
                        RiskRatioEstimate = stab.RiskRatioEstimate,
                        RiskRatioLower = stab.RiskRatioLower,
                        RiskRatioUpper = stab.RiskRatioUpper
                    };
                }
            }

            if (gridCells.singleTableResults != null && !string.IsNullOrEmpty(gridCells.singleTableResults.ErrorMessage))
            {
                //this.Dispatcher.BeginInvoke(new SimpleCallback(ClearSingleResults));
                //this.Dispatcher.BeginInvoke(new ShowErrorMessage(ShowError), gridCells.singleTableResults.ErrorMessage);
                //  this.Dispatcher.BeginInvoke(new RenderFinishWithErrorDelegate(RenderFinishWithError), gridCells.singleTableResults.ErrorMessage);
                //      RenderFinishWithError(gridCells.singleTableResults.ErrorMessage);
                //   resultSet.AddError(thisStringLiterals.RenderFinishWithError, gridCells.singleTableResults.ErrorMessage);
                //stopwatch.Stop();
                //Debug.Print("2x2 table gadget took " + stopwatch.Elapsed.ToString() + " seconds to complete with " + dashboardHelper.RecordCount.ToString() + " records and the following filters:");
                //Debug.Print(dashboardHelper.DataFilters.GenerateDataFilterString());
            }
            else
            {
                //    RenderGridCells(gridCells, table);            
                //    RenderFinish();
                resultSet.GridCells = gridCells;
            }

            return resultSet;
        }



        private TwoxTwoAndMxNResultsSet SetupMxN(GadgetParameters gadgetOptions, TwoxTwoAndMxNResultsSet resultSet)
        {
            DashboardHelper dashboardHelper = new DashboardHelper();
            DataTable table = resultSet.FreqResultsDataTable;

            StringLiterals thisStringLiterals = new StringLiterals();

            int MaxRows = 100;

            string strataValue = gadgetOptions.TableName;
            string freqVar = gadgetOptions.MainVariableName;

            //  this.Dispatcher.BeginInvoke(addGrid, string.Empty, table.TableName, table.Columns.Count);
            this.addGrid(string.Empty, table.TableName, table.Columns.Count);

            int rowCount;
            //double count = 0;
            //foreach (DescriptiveStatistics ds in resultSet.FreqResultsDescriptiveStatistics)
            //{
            //    count = count + ds.Observations;
            //}

            //if (count == 0)
            //{
            //    //  this.Dispatcher.BeginInvoke(new RenderFinishWithErrorDelegate(RenderFinishWithError), SharedStrings.NO_RECORDS_SELECTED);

            //    //   RenderFinishWithError(SharedStrings.NO_RECORDS_SELECTED);

            //    //  this.Dispatcher.BeginInvoke(new SimpleCallback(SetGadgetToFinishedState));

            //    // SetGadgetToFinishedState();

            //    return resultSet;    
            //}

            //if (table.Rows.Count == 0)
            //{
            //    //  this.Dispatcher.BeginInvoke(new RenderFinishWithErrorDelegate(RenderFinishWithError), SharedStrings.NO_RECORDS_SELECTED);
            //    RenderFinishWithError(SharedStrings.NO_RECORDS_SELECTED);

            //    //  this.Dispatcher.BeginInvoke(new SimpleCallback(SetGadgetToFinishedState));
            //    SetGadgetToFinishedState();

            //    return;
            //}

            string tableHeading = strataValue;

            //  this.Dispatcher.BeginInvoke(renderHeader, strataValue, tableHeading, table.Columns);
            //      renderHeader(strataValue, tableHeading, table.Columns);

            rowCount = 1;

            int[] totals = new int[table.Columns.Count - 1];
            int columnCount = 1;

            foreach (System.Data.DataRow row in table.Rows)
            {
                if (!row[freqVar].Equals(DBNull.Value) || (row[freqVar].Equals(DBNull.Value) && gadgetOptions.ShouldIncludeMissing == true))
                {
                    Field field = null;
                    foreach (DataRow fieldRow in dashboardHelper.FieldTable.Rows)
                    {
                        if (fieldRow["columnname"].Equals(freqVar))
                        {
                            if (fieldRow["epifieldtype"] is Field)
                            {
                                field = fieldRow["epifieldtype"] as Field;
                            }
                            break;
                        }
                    }

                    //  this.Dispatcher.Invoke(addRow, strataValue, 30);
                    this.addRow(strataValue, 30);

                    string displayValue = row[freqVar].ToString();

                    if (dashboardHelper.IsUserDefinedColumn(freqVar))
                    {
                        displayValue = dashboardHelper.GetFormattedOutput(freqVar, row[freqVar]);
                    }
                    else
                    {
                        if (field != null && field is YesNoField)
                        {
                            if (row[freqVar].ToString().Equals("1"))
                            {
                                displayValue = Ewav.Web.Config.ConfigDataSet.RepresentationOfYes;//"Yes";
                            }
                            else if (row[freqVar].ToString().Equals("0"))
                            {
                                displayValue = Ewav.Web.Config.ConfigDataSet.RepresentationOfNo;// "No";
                            }
                        }
                        else if (field != null && field is DateField)
                        {
                            displayValue = string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:d}", row[freqVar]);
                        }
                        else if (field != null && field is TimeField)
                        {
                            displayValue = string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:T}", row[freqVar]);
                        }
                        else
                        {
                            displayValue = dashboardHelper.GetFormattedOutput(freqVar, row[freqVar]);
                        }
                    }

                    if (string.IsNullOrEmpty(displayValue))
                    {
                        //Configuration config = dashboardHelper.Config;
                        displayValue = "Missing";//config.Settings.RepresentationOfMissing;
                    }

                    //  this.Dispatcher.BeginInvoke(setText, strataValue/new TextBlockConfig(thisStringLiterals.SPACE + displayValue + thisStringLiterals.SPACE, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Left, rowCount, 0), FontWeights.Normal);
                    this.setText(strataValue, new Ewav.Web.Services.TwoByTwoDomainService.TextBlockConfig(string.Format("{0}{1}{2}", thisStringLiterals.SPACE, displayValue, thisStringLiterals.SPACE),
                        rowCount, 0), "Normal");

                    int rowTotal = 0;
                    columnCount = 1;

                    foreach (DataColumn column in table.Columns)
                    {
                        if (columnCount > this.MaxColumns)
                        {
                            //this.Dispatcher.BeginInvoke(new ShowWarningDelegate(ShowWarning), (frequencies.Columns.Count - maxColumns).ToString() + " additional allColumns were not displayed due to gadget settings.");
                            resultSet.exceededMaxColumns = true;
                            break;
                        }

                        if (column.ColumnName.Equals(freqVar))
                        {
                            continue;
                        }

                        //  this.Dispatcher.BeginInvoke(setText, strataValue, new TextBlockConfig(thisStringLiterals.SPACE + row[column.ColumnName].ToString() + thisStringLiterals.SPACE, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, columnCount), FontWeights.Normal);
                        this.setText(strataValue, new Ewav.Web.Services.TwoByTwoDomainService.TextBlockConfig(string.Format("{0}{1}{2}", thisStringLiterals.SPACE, row[column.ColumnName].ToString(), thisStringLiterals.SPACE), rowCount, columnCount), "normal");

                        columnCount++;

                        int rowValue = 0;
                        bool success = int.TryParse(row[column.ColumnName].ToString(), out rowValue);
                        if (success)
                        {
                            totals[columnCount - 2] = totals[columnCount - 2] + rowValue;
                            rowTotal = rowTotal + rowValue;
                        }
                    }

                    //  this.Dispatcher.BeginInvoke(setText, strataValue, new TextBlockConfig(thisStringLiterals.SPACE + rowTotal.ToString() + thisStringLiterals.SPACE, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, columnCount), FontWeights.Bold);
                    this.setText(strataValue, new Ewav.Web.Services.TwoByTwoDomainService.TextBlockConfig(string.Format("{0}{1}{2}", thisStringLiterals.SPACE, rowTotal.ToString(), thisStringLiterals.SPACE),
                        rowCount, columnCount), "bo");

                    rowCount++;
                }

                if (rowCount > MaxRows)
                {
                    foreach (DataColumn column in table.Columns)
                    {
                        if (columnCount > this.MaxColumns)
                        {
                            resultSet.exceededMaxColumns = true;
                            break;
                        }

                        if (column.ColumnName.Equals(freqVar))
                        {
                            continue;
                        }

                        //  this.Dispatcher.BeginInvoke(setText, strataValue, new TextBlockConfig(thisStringLiterals.SPACE + thisStringLiterals.ELLIPSIS + thisStringLiterals.SPACE, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, columnCount), FontWeights.Normal);
                        this.setText(strataValue, new Ewav.Web.Services.TwoByTwoDomainService.TextBlockConfig(string.Format("{0}{1}{2}", thisStringLiterals.SPACE, thisStringLiterals.ELLIPSIS, thisStringLiterals.SPACE),
                            rowCount, columnCount), "normal");
                        columnCount++;
                    }

                    //  this.Dispatcher.BeginInvoke(setText, strataValue, new TextBlockConfig(thisStringLiterals.SPACE + thisStringLiterals.ELLIPSIS + thisStringLiterals.SPACE, new Thickness(2, 0, 2, 0), VerticalAlignment.Center, HorizontalAlignment.Right, rowCount, columnCount), FontWeights.Bold);
                    this.setText(strataValue, new Ewav.Web.Services.TwoByTwoDomainService.TextBlockConfig(string.Format("{0}{1}{2}", thisStringLiterals.SPACE, thisStringLiterals.ELLIPSIS, thisStringLiterals.SPACE),
                        rowCount, columnCount), "bo");

                    rowCount++;
                    resultSet.exceededMaxRows = true;

                    break;
                }
            }

            //  this.Dispatcher.BeginInvoke(new AddGridFooterDelegate(RenderFrequencyFooter), strataValue, rowCount, totals);
            this.RenderFrequencyFooter(strataValue, rowCount, totals);

            //  this.Dispatcher.BeginInvoke(drawBorders, strataValue);
            //      drawBorders(strataValue);

            //if (exceededMaxRows && exceededMaxColumns)
            //{
            //    //  this.Dispatcher.BeginInvoke(new RenderFinishWithWarningDelegate(RenderFinishWithWarning), "Warning: Some rows and allColumns were not displayed due to gadget settings. Showing top " + MaxRows.ToString() + " rows and top " + MaxColumns.ToString() + " allColumns only.");
            //    RenderFinishWithWarning("Warning: Some rows and allColumns were not displayed due to gadget settings. Showing top " + MaxRows.ToString() + " rows and top " + MaxColumns.ToString() + " allColumns only.");

            //}
            //else if (exceededMaxColumns)
            //{
            //    //  this.Dispatcher.BeginInvoke(new RenderFinishWithWarningDelegate(RenderFinishWithWarning), "Warning: Some allColumns were not displayed due to gadget settings. Showing top " + MaxColumns.ToString() + " allColumns only.");    
            //    RenderFinishWithWarning("Warning: Some allColumns were not displayed due to gadget settings. Showing top " + MaxColumns.ToString() + " allColumns only.");
            //}
            //else if (exceededMaxRows)
            //{
            //    //  this.Dispatcher.BeginInvoke(new RenderFinishWithWarningDelegate(RenderFinishWithWarning), string.Format(SharedStrings.DASHBOARD_GADGET_STATUS_ROW_LIMIT, MaxRows.ToString()));    
            //    RenderFinishWithWarning(string.Format(SharedStrings.DASHBOARD_GADGET_STATUS_ROW_LIMIT, MaxRows.ToString()));
            //}
            //else if (rowCount > 2)
            //{
            //    //  this.Dispatcher.BeginInvoke(new RenderFinishWithWarningDelegate(RenderFinishWithWarning), "Warning: More than two values exist in the exposure fields. Displaying an MxN table.");    
            //    RenderFinishWithWarning("Warning: More than two values exist in the exposure fields. Displaying an MxN table.");
            //}
            //else if (columnCount > 3)
            //{
            //    //  this.Dispatcher.BeginInvoke(new RenderFinishWithWarningDelegate(RenderFinishWithWarning), "Warning: More than two values exist in the outcome fields. Displaying an MxN table.");     
            //    RenderFinishWithWarning("Warning: More than two values exist in the outcome fields. Displaying an MxN table.");
            //}
            //else if (rowCount < 2)
            //{
            //    //  this.Dispatcher.BeginInvoke(new RenderFinishWithWarningDelegate(RenderFinishWithWarning), "Warning: Less than two values exist in the exposure fields. Displaying an MxN table.");        
            //    RenderFinishWithWarning("Warning: Less than two values exist in the exposure fields. Displaying an MxN table.");
            //}
            //else if (columnCount < 3)
            //{
            //    //  this.Dispatcher.BeginInvoke(new RenderFinishWithWarningDelegate(RenderFinishWithWarning), "Warning: Less than two values exist in the outcome fields. Displaying an MxN table.");    
            //    RenderFinishWithWarning("Warning: Less than two values exist in the outcome fields. Displaying an MxN table.");
            //}
            //else
            //{
            //    // this.Dispatcher.BeginInvoke(new SimpleCallback(RenderFinish));         
            //    RenderFinish();
            //}
            ////  this.Dispatcher.BeginInvoke(new SimpleCallback(SetGadgetToFinishedState));    
            //SetGadgetToFinishedState();

            resultSet.MxNGridRows = this.mxNGridRows;
            resultSet.MxNGridCells = this.mxNGridSetupParameters;
            resultSet.MxNSetTextParameters = this.mxNSetTextParameters;

            return resultSet;
        }

        /// <summary>
        /// Converts the data table.
        /// </summary>
        /// <param name="datatableBag">The datatable bag.</param>
        private void ConvertDataTable(DatatableBag datatableBag, DataTable dt)
        {
            //foreach (DataRow row in dtDataSources.Rows)
            //{
            //    foreach (DataColumn col in dtDataSources.Columns)
            //    {
            //        datatableBag.FieldsList = new List<MyString>();
            //          datatableBag.FieldsList.Add(new MyString(row[col.ColumnName].ToString()));
            //    }
            //      datatableBag.RecordList.Add(fieldsList);
            //}     
        }

        #region  Utilities

        private TwoxTwoTableDTO CreateDTO(GadgetParameters gp, DashboardHelper dh, DataTable table)
        {


            int YesRowIndexExp = 1;
            int NoRowIndexExp = 0;
            int YesRowIndexOut = 1;
            int NoRowIndexOut = 2;

            if (dh.GetColumnType(gp.MainVariableName.ToString()).ToLower() != "system.string")
            {
                YesRowIndexExp = FindYesRowIndex(table);

                NoRowIndexExp = FindNoRowIndex(table);
            }


            if (dh.GetColumnType(gp.CrosstabVariableName.ToString()).ToLower() != "system.string")
            {
                YesRowIndexOut = FindYesColIndex(table);

                NoRowIndexOut = FindNoColIndex(table);
            }

            TwoxTwoTableDTO ttDTO = new TwoxTwoTableDTO
                        {
                            Yy = int.Parse(table.Rows[YesRowIndexExp][YesRowIndexOut].ToString()),
                            Yn = int.Parse(table.Rows[YesRowIndexExp][NoRowIndexOut].ToString()),
                            Ny = int.Parse(table.Rows[NoRowIndexExp][YesRowIndexOut].ToString()),
                            Nn = int.Parse(table.Rows[NoRowIndexExp][NoRowIndexOut].ToString())
                        };

            //Yy = int.Parse(table.Rows[1][1].ToString()),
            //               Yn = int.Parse(table.Rows[1][2].ToString()),
            //               Ny = int.Parse(table.Rows[0][1].ToString()),
            //               Nn = int.Parse(table.Rows[0][2].ToString())
            //ttDTO.Rows[0, 1] = int.Parse(table.Rows[0][1].ToString());
            //ttDTO.Rows[0, 2] = int.Parse(table.Rows[0][2].ToString());
            //ttDTO.Rows[1, 1] = int.Parse(table.Rows[1][1].ToString());
            //ttDTO.Rows[1, 2] = int.Parse(table.Rows[1][2].ToString());  




            ttDTO.ColumnName1 = table.Columns[1].ColumnName;
            ttDTO.ColumnName2 = table.Columns[2].ColumnName;

            return ttDTO;
        }

        private int FindYesColIndex(DataTable table)
        {
            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (table.Columns[i].ColumnName.ToLower() == "1" ||
                    table.Columns[i].ColumnName.ToLower() == "yes" ||
                    table.Columns[i].ColumnName.ToLower() == "true")
                {
                    return i;
                }
            }

            return 0;
        }

        private int FindNoColIndex(DataTable table)
        {
            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (table.Columns[i].ColumnName.ToLower() == "0" ||
                    table.Columns[i].ColumnName.ToLower() == "no" ||
                    table.Columns[i].ColumnName.ToLower() == "false")
                {
                    return i;
                }
            }

            return 0;
        }

        private int FindYesRowIndex(DataTable table)
        {
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (table.Rows[i][0].ToString() == "1" ||
                    table.Rows[i][0].ToString().ToLower() == "yes" ||
                    table.Rows[i][0].ToString().ToLower() == "true")
                {
                    return i;
                }
            }

            return 0;
        }

        private int FindNoRowIndex(DataTable table)
        {
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (table.Rows[i][0].ToString() == "0" ||
                    table.Rows[i][0].ToString().ToLower() == "no" ||
                    table.Rows[i][0].ToString().ToLower() == "false")
                {
                    return i;
                }
            }

            return 0;
        }






        void Swap2x2RowValues(DataTable table)
        {
            if (table.Rows.Count > 2 || table.Columns.Count > 3)
            {
                return; // cannot do an invalid 2x2 table
            }

            object row1Col1 = table.Rows[0][1];
            object row1Col2 = table.Rows[0][2];

            object row2Col1 = table.Rows[1][1];
            object row2Col2 = table.Rows[1][2];

            table.Rows[0][1] = row2Col1;
            table.Rows[0][2] = row2Col2;

            table.Rows[1][1] = row1Col1;
            table.Rows[1][2] = row1Col2;

            object firstRowName = table.Rows[0][0];
            table.Rows[0][0] = table.Rows[1][0];
            table.Rows[1][0] = firstRowName;
        }

        void Swap2x2ColValues(DataTable table)
        {
            if (table.Rows.Count > 2 || table.Columns.Count > 3)
            {
                return; // cannot do an invalid 2x2 table
            }

            object row1Col1 = table.Rows[0][1];
            object row1Col2 = table.Rows[0][2];

            object row2Col1 = table.Rows[1][1];
            object row2Col2 = table.Rows[1][2];

            table.Rows[0][1] = row1Col2;
            table.Rows[0][2] = row1Col1;

            table.Rows[1][1] = row2Col2;
            table.Rows[1][2] = row2Col1;

            string firstColumnName = table.Columns[1].ColumnName;
            string secondColumnName = table.Columns[2].ColumnName;

            table.Columns[1].ColumnName = "_COL1_";
            table.Columns[2].ColumnName = "_COL2_";

            table.Columns[1].ColumnName = secondColumnName;
            table.Columns[2].ColumnName = firstColumnName;
        }

        private void addRow(string strataValue, int width)
        {
            //MxNGridRow mxNGridRow = new MxNGridRow();
            //mxNGridRow.strataValue = "aa";
            //mxNGridRow.width = 77;
            this.mxNGridRows.Add(new MxNGridRow
            {
                strataValue = strataValue,
                width = width
            });
        }

        private void addGrid(string strataVar, string tableName, int count)
        {
            //mxNGridCell.strataVar = strataVar;
            //mxNGridCell.tableName = tableName;
            //mxNGridCell.count = count;    
            this.mxNGridSetupParameters.Add(new MxNGridSetupParameter
            {
                strataVar = strataVar,
                tableName = tableName,
                count = count
            });
        }

        private MySingleTableResults ConvertSingleTableResultToLocal(StatisticsRepository.cTable.SingleTableResults stResults)
        {
            MySingleTableResults mystResults = new MySingleTableResults();

            mystResults.ChiSquareMantel2P = stResults.ChiSquareMantel2P;
            mystResults.ChiSquareMantel2P = stResults.ChiSquareMantel2P;
            mystResults.ChiSquareMantelVal = stResults.ChiSquareMantelVal;
            mystResults.ChiSquareUncorrected2P = stResults.ChiSquareUncorrected2P;
            mystResults.ChiSquareUncorrectedVal = stResults.ChiSquareUncorrectedVal;
            mystResults.ChiSquareYates2P = stResults.ChiSquareYates2P;
            mystResults.ChiSquareYatesVal = stResults.ChiSquareYatesVal;
            mystResults.ErrorMessage = stResults.ErrorMessage;
            mystResults.FisherExact2P = stResults.FisherExact2P;
            mystResults.FisherExactP = stResults.FisherExactP;
            mystResults.MidP = stResults.MidP;
            mystResults.OddsRatioEstimate = stResults.OddsRatioEstimate;
            mystResults.OddsRatioLower = stResults.OddsRatioLower;
            mystResults.OddsRatioMLEEstimate = stResults.OddsRatioMLEEstimate;
            mystResults.OddsRatioMLEFisherLower = stResults.OddsRatioMLEFisherLower;
            mystResults.OddsRatioMLEFisherUpper = stResults.OddsRatioMLEFisherUpper;
            mystResults.OddsRatioMLEMidPLower = stResults.OddsRatioMLEMidPLower;
            mystResults.OddsRatioMLEMidPUpper = stResults.OddsRatioMLEMidPUpper;
            mystResults.OddsRatioUpper = stResults.OddsRatioUpper;
            mystResults.RiskDifferenceEstimate = stResults.RiskDifferenceEstimate;
            mystResults.RiskDifferenceLower = stResults.RiskDifferenceLower;
            mystResults.RiskDifferenceUpper = stResults.RiskDifferenceUpper;
            mystResults.RiskRatioEstimate = stResults.RiskRatioEstimate;
            mystResults.RiskRatioLower = stResults.RiskRatioLower;
            mystResults.RiskRatioUpper = stResults.RiskRatioUpper;

            return mystResults;
        }

        private void setText(string strataValue, Ewav.Web.Services.TwoByTwoDomainService.TextBlockConfig textBlockConfig, string weight)
        {
            this.mxNSetTextParameters.Add(new MxNSetTextParameter
            {
                strataValue = strataValue,
                textBlockConfig = textBlockConfig
            });
            //MxNSetTextParameter xx = new MxNSetTextParameter();
            //xx.strataValue = strataValue;
            //xx.textBlockConfig = textBlockConfig;
            //xx.fontWeight = normal;    
        }

        private void RenderFrequencyFooter(string strataValue, object rowCount, int[] totals)
        {
            // TODO: Implement this method
            //       throw new NotImplementedException();
        }

        private void CheckDataTable(DataTable table)                //       TwoxTwoTableDTO table)
        {
            Dictionary<string, string> booleanValues = new Dictionary<string, string>();
            booleanValues.Add("0", "1");
            booleanValues.Add("false", "true");
            booleanValues.Add("f", "t");
            booleanValues.Add("no", "yes");
            booleanValues.Add("n", "y");

            //if (!booleanValues.ContainsKey(dashboardHelper.Config.Settings.RepresentationOfNo.ToLower()))
            //{
            //    booleanValues.Add(dashboardHelper.Config.Settings.RepresentationOfNo.ToLower(),
            //        dashboardHelper.Config.Settings.RepresentationOfYes.ToLower());
            //}

            string firstColumnName = table.Columns[1].ColumnName.ToLower();
            string secondColumnName = table.Columns[2].ColumnName.ToLower();

            if (booleanValues.ContainsKey(firstColumnName) && secondColumnName.Equals(booleanValues[firstColumnName]))
            {
                this.Swap2x2ColValues(table);
            }

            string firstRowName = table.Rows[0][0].ToString();
            string secondRowName = table.Rows[1][0].ToString();

            if (booleanValues.ContainsKey(firstRowName) && secondRowName.Equals(booleanValues[firstRowName]))
            {
                this.Swap2x2RowValues(table);
            }
        }
        #endregion
    }
}