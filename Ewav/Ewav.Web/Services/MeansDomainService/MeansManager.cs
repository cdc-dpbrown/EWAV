/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MeansManager.cs
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
using Ewav.Web.Services;

namespace Ewav.BAL
{
    public class MeansManager
    {
        public static List<CrossTabResponseObjectDto>
        ConvertCrossTabDictToDto(Dictionary<DataTable, List<DescriptiveStatistics>> dataTableDictionary, GadgetParameters gadgetParameters)
        {
            //bool columnNamesRead = false;
           
            List<CrossTabResponseObjectDto> crossTabDtoList = new List<CrossTabResponseObjectDto>();
            //KeyValuePair<DataTable, List<DescriptiveStatistics>> kvp = dataTableDictionary;

            foreach (KeyValuePair<DataTable, List<DescriptiveStatistics>> khp in dataTableDictionary)
            {
                CrossTabResponseObjectDto CrossTabDto = new CrossTabResponseObjectDto();
                DataTable dt = khp.Key;
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    CrossTabDto.ColumnNames.Add(dt.Columns[i].ColumnName);
                    
                }
                CrossTabDto.DsList= khp.Value;
               // columnNamesRead = true;
                
                crossTabDtoList.Add(CrossTabDto);
            }

            
            return crossTabDtoList;
        }
    }
}