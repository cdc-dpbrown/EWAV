/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       EwavColumn.cs
 *  Namespace:  Ewav.BAL    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Linq;
using Ewav.Web.EpiDashboard;

namespace Ewav.BAL
{
    public class EwavColumn        //: IEnumerable
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string NoCamelName { get; set; }
        //public string NoCamelName(  )  
        //{
        //    Regex r = new Regex("(?<=[a-z])(?<x>[A-Z])|(?<=.)(?<x>[A-Z])(?=[a-z]) ");
        //    return r.Replace( Name,  " ${x}");
        //}
        public ColumnDataType SqlDataTypeAsString { get; set; }
        public bool IsInUse { get; set; }
        public string ChildVariableName { get; set; }
        public bool IsUserDefined { get; set; }
    }
}