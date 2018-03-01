/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       FrequencyDomainService.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
namespace Ewav.Web.Services
{
    using System.Collections.Generic;
    using System.Data;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using EpiDashboard;
    using Ewav.BAL;
    using Ewav.DTO;

    /// <summary>
    /// Frequency Domain Service functions  
    /// </summary>
    [EnableClientAccess()]
    public class FrequencyDomainService : DomainService
    {
        #region  "These are here to force the proxy classes to be created.  "
        //public void PortClassToClient(ColumnDataType d)
        //{
        //}
        public void PortClassToClient777(DescriptiveStatistics ds)
        {
        }

        public void PortClassToClient49491(EwavRule_Base gs)
        {


        }


        public void PortClassToClient494914(EwavRule_ExpressionAssign xx)
        {


        }


        public void PortClassToClient494941(Class1 xx)
        {


        }


        public void PortClassToClient494911(EwavRule_Recode gs)
        {


        }





























        public void PortClassToClient49491111(EwavRule_SimpleAssignment gs)
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

        public List<int> GenerateTestResults(int intValue)
        {
            List<int> collectionOfInt = new List<int>();

            for (int i = 0; i < 1000; i++)
            {
                collectionOfInt.Add(intValue + i);
            }

            return collectionOfInt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gadgetParameters"></param>
        /// <param name="DatasourceName"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>

        [Invoke]
        public List<FrequencyResultData> GenerateFrequencyTable(GadgetParameters gadgetParameters,
               IEnumerable<EwavDataFilterCondition> ewavDataFilters, List<EwavRule_Base> rules, string filterString = "")
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