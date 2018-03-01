/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MxNGridCell.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
namespace Ewav.Web.Services
{
    public class MxNGridSetupParameter
    {
        private int count1;
        private string strataVar1;
        private string tableName1;
        public int count
        {
            get
            {
                return this.count1;
            }
            set
            {
                this.count1 = value;
            }
        }
        public string strataVar
        {
            get
            {
                return this.strataVar1;
            }
            set
            {
                this.strataVar1 = value;
            }
        }
        public string tableName
        {
            get
            {
                return this.tableName1;
            }
            set
            {
                this.tableName1 = value;
            }
        }
    }
}