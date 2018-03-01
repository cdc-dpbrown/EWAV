/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       XYChartDomainService.cs
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
    using System.Data;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using EpiDashboard;
    using Ewav.DTO;
    using Ewav.BAL;


    // TODO: Create methods containing your application logic.
    [EnableClientAccess()]
    public class XYChartDomainService : DomainService
    {


        [Query(IsComposable = false)]
        public EwavRule_Base Getrule(int id)
        {
            return new EwavRule_Base();

        }


        DashboardHelper dh;

        [Invoke]
        public List<FrequencyResultData> GenerateFrequencyTable(GadgetParameters gadgetParameters,
            IEnumerable<EwavDataFilterCondition> ewavDataFilters,
            List<EwavRule_Base> rules,
            string filterString = "")
        {
            if (gadgetParameters.UseAdvancedDataFilter)
            {
                dh = new DashboardHelper(gadgetParameters, filterString, rules);
                gadgetParameters.UseAdvancedDataFilter = true;
                gadgetParameters.AdvancedDataFilterText = filterString;
            }
            else
            {
                dh = new DashboardHelper(gadgetParameters, ewavDataFilters, rules);
            }
            Dictionary<DataTable, List<DescriptiveStatistics>> dataTableDictionary;
            dataTableDictionary = dh.GenerateFrequencyTable(gadgetParameters, gadgetParameters.DatasourceName, gadgetParameters.TableName);

            Dictionary<List<EwavFrequencyControlDto>, List<DescriptiveStatistics>> dtoDictionary;
            dtoDictionary = FrequencyManager.ConvertDatatableToList(dataTableDictionary, gadgetParameters);

            List<FrequencyResultData> outputList = new List<FrequencyResultData>();

            foreach (KeyValuePair<List<EwavFrequencyControlDto>, List<DescriptiveStatistics>> khp in dtoDictionary)
            {
                List<EwavFrequencyControlDto> outFreqList = new List<EwavFrequencyControlDto>();
                List<DescriptiveStatistics> outDscStat = new List<DescriptiveStatistics>();

                foreach (EwavFrequencyControlDto freqDto in khp.Key)
                {
                    outFreqList.Add(freqDto);
                }
                foreach (DescriptiveStatistics descStat in khp.Value)
                {
                    outDscStat.Add(descStat);
                }

                var frequencyResultData = new FrequencyResultData()
                {
                    FrequencyControlDtoList = outFreqList,
                    DescriptiveStatisticsList = outDscStat
                };
                outputList.Add(frequencyResultData);
            }

            return outputList;
        }
    }
}