/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       EwavFrequencyControlInputTableDto.cs
 *  Namespace:  Ewav.DTO    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.Collections.Generic;


namespace Ewav.DTO
{
    public class EwavFrequencyControlInputTableDto
    {
        public int RowsCount { get; set; }

        public string TableName { get; set; }
          
        public Dictionary<string, string> InputTable { get; set; }
    }
}