/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       TextBlockConfig.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.ServiceModel.DomainServices.Server;

namespace Ewav.Web.Services
{
  
    public partial class TwoByTwoDomainService : DomainService
    {
        public class TextBlockConfig
        {
            public int ColumnNumber;
            public string DisplayValue;
            public int RowNumber;
            public TextBlockConfig(string displayValue, int rowNumber, int columnNumber)
            {
                this.DisplayValue = displayValue;
                this.RowNumber = rowNumber;
                this.ColumnNumber = columnNumber;
            }
        }
    }
}