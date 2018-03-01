/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       FrequencyAndCrossTable.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ewav.Web.Services
{
    public class FrequencyAndCrossTable
    {
        private List<CrossTabResponseObjectDto> crossTable;

        public List<CrossTabResponseObjectDto> CrossTable
        {
            get { return crossTable; }
            set { crossTable = value; }
        }

        private List<FrequencyResultData> frequencyTable;

        public List<FrequencyResultData> FrequencyTable
        {
            get { return frequencyTable; }
            set { frequencyTable = value; }
        }
    }
}