/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       UniqueValueInfoDTO.cs
 *  Namespace:  Ewav.Mapping    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
namespace Ewav.Mapping
{
    public struct UniqueValueInfoDTO
    {
        public string Description { get; set; }

        public string Label { get; set; }

        public string Symbol { get; set; }

        public string Value { get; set; }
    }
}