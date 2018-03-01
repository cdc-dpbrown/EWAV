/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MxNGridRow.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
namespace Ewav.Web.Services
{
    public class MxNGridRow
    {
        private string strataValue1;
        private int width1;
        public string strataValue
        {
            get
            {
                return this.strataValue1;
            }
            set
            {
                this.strataValue1 = value;
            }
        }
        public int width
        {
            get
            {
                return this.width1;
            }
            set
            {
                this.width1 = value;
            }
        }
    }
}