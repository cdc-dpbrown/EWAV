/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       EpiCurveDomainService.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
namespace Ewav.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using EpiDashboard;
    using Ewav.DTO;
    using Ewav.BAL;
    using System.Data;


    // TODO: Create methods containing your application logic.
    [EnableClientAccess()]
    public class EpiCurveDomainService : DomainService
    {
        #region  "These are here to force the proxy classes to be created.  "
        //public void PortClassToClient(ColumnDataType d)
        //{
        //}
        public void PortClassToClient777(DescriptiveStatistics ds)
        {
        }

        public void PortClassToClient4(GadgetParameters gp)
        {
        }

        public void PortClassToClient7(EwavFrequencyControlDto gp)
        {
        }

        public void PortClassToClient77(EwavColumn gp)
        {
        }

        //public void PortClassToClient(EwavColumnsMetaData wcmd) { }
        public void PortClassToClient79(EwavConnectionString aa)
        {

        }
        #endregion


        [Query(IsComposable = false)]
        public EwavRule_Base Getrule(int id)
        {
            return new EwavRule_Base();

        }




        /// <summary>
        /// DashboardHelper    
        /// </summary>
        private DashboardHelper dh;


        /// <summary>
        /// Get a list of columns    
        /// </summary>
        /// <param name="DataSourceName"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public List<EwavColumn> GetColumns(string DataSourceName, string TableName)
        {
            return BAL.Common.GetColumns(DataSourceName, TableName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gadgetParameters"></param>
        /// <param name="DatasourceName"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>

        //public DatatableBag GenerateEpiCurveTable(GadgetParameters gadgetParameters,
        //    decimal minWeek, decimal maxWeek)
        //{
        //    dh = new DashboardHelper();
        //    DataTable data;
        //    data = dh.GenerateEpiCurveTable(gadgetParameters, minWeek, maxWeek);

        //    //DatatableBag dtBag = new DatatableBag();
        //    //dtBag.

        //    Dictionary<List<EwavFrequencyControlDto>, List<DescriptiveStatistics>> dtoDictionary;
        //    dtoDictionary = FrequencyManager.ConvertDatatableToList(dataTableDictionary, gadgetParameters);

        //    List<FrequencyResultData> outputList = new List<FrequencyResultData>();

        //    foreach (KeyValuePair<List<EwavFrequencyControlDto>, List<DescriptiveStatistics>> khp in dtoDictionary)
        //    {
        //        List<EwavFrequencyControlDto> outFreqList = new List<EwavFrequencyControlDto>();
        //        List<DescriptiveStatistics> outDscStat = new List<DescriptiveStatistics>();

        //        foreach (EwavFrequencyControlDto freqDto in khp.Key)
        //        {
        //            outFreqList.Add(freqDto);
        //        }
        //        foreach (DescriptiveStatistics descStat in khp.Value)
        //        {
        //            outDscStat.Add(descStat);
        //        }

        //        var frequencyResultData = new FrequencyResultData()
        //        {
        //            FrequencyControlDtoList = outFreqList,
        //            DescriptiveStatisticsList = outDscStat
        //        };
        //        outputList.Add(frequencyResultData);
        //    }

        //    return outputList;
        //}

        [Invoke]
        public DatatableBag GetEpiCurveData(GadgetParameters gadgetParameters,
               List<EwavDataFilterCondition> ewavDataFilters, List<EwavRule_Base> rules,string filterString,  bool byEpiWeek, string dateVar, string caseStatusVar)
        {
            DataTable data = new DataTable();
            DatatableBag dtb = new DatatableBag();

            int recordCount = 0;

            if (gadgetParameters.UseAdvancedDataFilter)
            {
                dh = new DashboardHelper(gadgetParameters, filterString, rules);
                gadgetParameters.UseAdvancedDataFilter = true;
                gadgetParameters.AdvancedDataFilterText = filterString;
            }
            else
            {
                if(gadgetParameters.GadgetFilters != null)
                { 
                    ewavDataFilters.AddRange(gadgetParameters.GadgetFilters);
                }

                dh = new DashboardHelper(gadgetParameters, ewavDataFilters, rules);
            }

            if (byEpiWeek)
            {
                decimal? minWeek = null;
                decimal? maxWeek = null;

                dh.FindUpperLowerNumericValues(dateVar, ref minWeek, ref maxWeek);
                if (minWeek.HasValue && maxWeek.HasValue)
                {
                    // override min and max values if the user set custom start and end points for the x-axis
                    if (gadgetParameters.InputVariableList.ContainsKey("xaxisstart"))
                    {
                        minWeek = decimal.Parse(gadgetParameters.InputVariableList["xaxisstart"]);
                    }
                    if (gadgetParameters.InputVariableList.ContainsKey("xaxisend"))
                    {
                        maxWeek = decimal.Parse(gadgetParameters.InputVariableList["xaxisend"]);
                    }

                    if (string.IsNullOrEmpty(caseStatusVar))
                    {
                        gadgetParameters.CrosstabVariableName = "_default_";
                        data = dh.GenerateEpiCurveTable(gadgetParameters, minWeek.Value, maxWeek.Value);
                    }
                    else
                    {
                        data = dh.GenerateEpiCurveTable(gadgetParameters, minWeek.Value, maxWeek.Value);
                    }

                    recordCount = Convert.ToInt16( data.Compute("Sum(totals)", ""));

                    if (data != null && data.Rows.Count > 0)
                    {
                        if (string.IsNullOrEmpty(caseStatusVar))
                        {
                            data = Pivot(data, dateVar, "_default_", "totals");
                        }
                        else
                        {
                            data = Pivot(data, dateVar, caseStatusVar, "totals");
                        }
                    }
                    decimal currentWeek = minWeek.Value;
                    while (currentWeek <= maxWeek.Value)
                    {
                        if (data.Select("[" + dateVar + "] = '" + currentWeek.ToString() + "'").Count() == 0)
                        {
                            DataRow row = data.NewRow();
                            row[0] = currentWeek;
                            data.Rows.Add(row);
                        }
                        currentWeek = currentWeek + 1;
                    }
                }
            }
            else
            {
                string dateInterval = gadgetParameters.InputVariableList["dateinterval"];

                DateTime? minDate = null;
                DateTime? maxDate = null;

                dh.FindUpperLowerDateValues(dateVar, ref minDate, ref maxDate, gadgetParameters);
                if (minDate.HasValue && maxDate.HasValue)
                {
                    if (gadgetParameters.InputVariableList.ContainsKey("xaxisstart"))
                    {
                        DateTime dt;
                        bool success = DateTime.TryParse(gadgetParameters.InputVariableList["xaxisstart"], out dt);
                        if (success)
                        {
                            minDate = dt;
                        }
                    }
                    if (gadgetParameters.InputVariableList.ContainsKey("xaxisend"))
                    {
                        DateTime dt;
                        bool success = DateTime.TryParse(gadgetParameters.InputVariableList["xaxisend"], out dt);
                        if (success)
                        {
                            maxDate = dt;
                        }
                    }

                    if (string.IsNullOrEmpty(caseStatusVar))
                    {
                        gadgetParameters.CrosstabVariableName = "_default_";
                    }

                    data = dh.GenerateEpiCurveTable(gadgetParameters, (DateTime)minDate, (DateTime)maxDate);

                    recordCount = Convert.ToInt16(data.Compute("Sum(totals)", ""));

                    if (data != null && data.Rows.Count > 0)
                    {
                        if (string.IsNullOrEmpty(caseStatusVar))
                        {
                            data = Pivot(data, dateVar, "_default_", "totals");
                        }
                        else
                        {
                            data = Pivot(data, dateVar, caseStatusVar, "totals");
                        }
                    }
                    else
                    {
                        return new DatatableBag();
                    }
                    DateTime currentDate = minDate.Value;
                    while (currentDate <= maxDate.Value)
                    {
                        //if (IsCancelled())
                        //{
                        //    data = null;
                        //    return data;
                        //}

                        if (data.Select("[" + dateVar + "] = '" + currentDate.ToString() + "'").Length == 0)
                        {
                            DataRow row = data.NewRow();
                            row[0] = currentDate;
                            data.Rows.Add(row);
                        }

                        //if (dashboardHelper.IsUsingEpiProject && View.Fields.Contains(dateVar) && View.Fields[dateVar] is DateTimeField && !(View.Fields[dateVar] is DateField) && !(View.Fields[dateVar] is TimeField))
                        if (dateInterval.ToLower().Equals("hours"))
                        {
                            currentDate = currentDate.AddHours(1);
                        }
                        else if (dateInterval.ToLower().Equals("days"))
                        {
                            currentDate = currentDate.AddDays(1);
                        }
                        else if (dateInterval.ToLower().Equals("months"))
                        {
                            currentDate = currentDate.AddMonths(1);
                        }
                        else if (dateInterval.ToLower().Equals("years"))
                        {
                            currentDate = currentDate.AddMonths(1);
                        }
                    }
                }
            }

            if (data == null || data.Columns.Count == 0)
            {
                return null;
            }

            

            data.DefaultView.Sort = "[" + data.Columns[0].ColumnName + "]";
            DataView dv = data.DefaultView;
            DataTable newDT = dv.ToTable();

            DatatableBag returnedTable = new DatatableBag(newDT, "");
            returnedTable.ExtraInfo = new List<DictionaryDTO>();
            returnedTable.ExtraInfo.Add(new DictionaryDTO() { Key = new MyString("RecordCount"), Value = new MyString(recordCount.ToString()) });

            return returnedTable;

            //  return dtb.ToDatatableBag(newDT);      //  .Select("",
            //    "[" + data.Columns[0].ColumnName + "]"));

        }

        public static DataTable Pivot(DataTable dataValues, string keyColumn, string pivotNameColumn, string pivotValueColumn)
        {
            DataTable tmp = new DataTable();
            DataRow r;
            string LastKey = "//dummy//";
            int i, pValIndex, pNameIndex;
            string s;
            bool FirstRow = true;

            pValIndex = dataValues.Columns[pivotValueColumn].Ordinal;
            pNameIndex = dataValues.Columns[pivotNameColumn].Ordinal;

            for (i = 0; i <= dataValues.Columns.Count - 1; i++)
            {
                if (i != pValIndex && i != pNameIndex)
                    tmp.Columns.Add(dataValues.Columns[i].ColumnName, dataValues.Columns[i].DataType);
            }

            r = tmp.NewRow();

            foreach (DataRow row1 in dataValues.Rows)
            {
                if (row1[keyColumn] != DBNull.Value)
                {
                    if (row1[keyColumn].ToString() != LastKey)
                    {
                        if (!FirstRow)
                            tmp.Rows.Add(r);

                        r = tmp.NewRow();
                        FirstRow = false;

                        //loop thru fields of row1 and populate tmp table
                        for (i = 0; i <= row1.ItemArray.Length - 3; i++)
                            r[i] = row1[tmp.Columns[i].ToString()];

                        LastKey = row1[keyColumn].ToString();
                    }

                    s = row1[pNameIndex].ToString();

                    if (!string.IsNullOrEmpty(s))
                    {
                        if (!tmp.Columns.Contains(s))
                            tmp.Columns.Add(s, typeof(int));// dataValues.Columns[pNameIndex].DataType);
                        r[s] = row1[pValIndex];
                    }
                }
            }

            //add that final row to the datatable:
            tmp.Rows.Add(r);
            return tmp;
        }

        //public List<DateTime?> FindUpperLowerDateValues(string columnName, DateTime? minDate, DateTime? maxDate) 
        //{
        //    List<DateTime?> listDateTime = new List<DateTime?>();
        //    dh.FindUpperLowerDateValues(columnName, ref minDate, ref maxDate);
        //    listDateTime.Add(minDate);
        //    listDateTime.Add(maxDate);

        //    return listDateTime;
        //}
    }
}