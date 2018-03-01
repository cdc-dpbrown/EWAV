/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       FrequencyManager.cs
 *  Namespace:  Ewav.BAL    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.Collections.Generic;
using System.Data;
using Ewav.Web.EpiDashboard;
using Ewav.DTO;

namespace Ewav.BAL
{
    public class FrequencyManager
    {
        public static Dictionary<List<EwavFrequencyControlDto>, List<DescriptiveStatistics>>
        ConvertDatatableToList(Dictionary<DataTable, List<DescriptiveStatistics>> dataTableDictionary, 
            GadgetParameters gadgetParameters)

        {
            Dictionary<List<EwavFrequencyControlDto>, List<DescriptiveStatistics>> DtoDictionary =
                new Dictionary<List<EwavFrequencyControlDto>, List<DescriptiveStatistics>>();

            EwavGadgetParameters g = new EwavGadgetParameters()
            {
                CrosstabVariableName = gadgetParameters.CrosstabVariableName,
                CustomFilter = gadgetParameters.CustomFilter,
                CustomSortColumnName = gadgetParameters.CustomSortColumnName,
                InputVariableList = gadgetParameters.InputVariableList,
                MainVariableName = gadgetParameters.MainVariableName,
                MainVariableNames = gadgetParameters.MainVariableNames,
                ShouldIncludeFullSummaryStatistics = gadgetParameters.ShouldIncludeFullSummaryStatistics,
                ShouldIncludeMissing = gadgetParameters.ShouldIncludeMissing,
                ShouldShowCommentLegalLabels = gadgetParameters.ShouldSortHighToLow,
                ShouldUseAllPossibleValues = gadgetParameters.ShouldUseAllPossibleValues,
                StrataVariableNames = gadgetParameters.StrataVariableNames,
                WeightVariableName = gadgetParameters.WeightVariableName,
                NameOfDtoList = gadgetParameters.MainVariableName
            };
            
            foreach (KeyValuePair<DataTable, List<DescriptiveStatistics>> khp in dataTableDictionary)
            {
                DataTable dt = khp.Key;    
                List<EwavFrequencyControlDto> ewavFrequencyControlDto = Mapper.FrequencyOutputList(dt, g);
                DtoDictionary.Add(ewavFrequencyControlDto, khp.Value);
            }

            return DtoDictionary;
        }

        public static Dictionary<List<EwavFrequencyControlDto>, List<DescriptiveStatistics>>
            ConvertPivotDatatableToList(Dictionary<DataTable, List<DescriptiveStatistics>> dataTableDictionary,
                GadgetParameters gadgetParameters)    

        {
            Dictionary<List<EwavFrequencyControlDto>, List<DescriptiveStatistics>> DtoDictionary =
                new Dictionary<List<EwavFrequencyControlDto>, List<DescriptiveStatistics>>();

            EwavGadgetParameters g = new EwavGadgetParameters()
            {
                CrosstabVariableName = gadgetParameters.CrosstabVariableName,
                CustomFilter = gadgetParameters.CustomFilter,
                CustomSortColumnName = gadgetParameters.CustomSortColumnName,
                InputVariableList = gadgetParameters.InputVariableList,
                MainVariableName = gadgetParameters.MainVariableName,
                MainVariableNames = gadgetParameters.MainVariableNames,
                ShouldIncludeFullSummaryStatistics = gadgetParameters.ShouldIncludeFullSummaryStatistics,
                ShouldIncludeMissing = gadgetParameters.ShouldIncludeMissing,
                ShouldShowCommentLegalLabels = gadgetParameters.ShouldSortHighToLow,
                ShouldUseAllPossibleValues = gadgetParameters.ShouldUseAllPossibleValues,
                StrataVariableNames = gadgetParameters.StrataVariableNames,
                WeightVariableName = gadgetParameters.WeightVariableName,
                NameOfDtoList = gadgetParameters.MainVariableName
            };

            foreach (KeyValuePair<DataTable, List<DescriptiveStatistics>> khp in dataTableDictionary)
            {
                DataTable dt = Ewav.Web.Utilities.Pivot(khp.Key, "Frequency", "MainVarname", "totals");
                List<EwavFrequencyControlDto> ewavFrequencyControlDto = Mapper.FrequencyOutputList(dt, g);
                DtoDictionary.Add(ewavFrequencyControlDto, khp.Value);
            }

            return DtoDictionary;
        }
    }
}