/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       FrequencyResultData.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.Collections.Generic;

namespace Ewav.Web.Services
{
    public class FrequencyResultData
    {
        private List<Ewav.DTO.EwavFrequencyControlDto> frequencyControlDtoList;
        private List<EpiDashboard.DescriptiveStatistics> descriptiveStatisticsList;

        public FrequencyResultData()
        {
        }

        public List<Ewav.DTO.EwavFrequencyControlDto> FrequencyControlDtoList
        {
            get { return frequencyControlDtoList; }
            set { frequencyControlDtoList = value; }
        }    
   
        public List<EpiDashboard.DescriptiveStatistics> DescriptiveStatisticsList
        {
            get { return descriptiveStatisticsList; }
            set { descriptiveStatisticsList = value; }
        }
   
    }
}