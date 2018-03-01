/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       LineListManager.cs
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
using System.Web;
using Ewav.BAL;
using Ewav.Web.EpiDashboard;
using Ewav.DTO;
using System.Data;


namespace Ewav.Web.Services
{
    /// <summary>
    ///   LineListManager     
    /// </summary>
    public class LineListManager
    {
        /// <summary>
        /// Gets the line list.
        /// </summary>
        /// <param name="filterList">The filter list.</param>
        /// <param name="gp">The gp.</param>
        /// <returns></returns>
        public List<DatatableBag> GetLineList(GadgetParameters gadgetParameters,
               IEnumerable<EwavDataFilterCondition> ewavDataFilters, List<EwavRule_Base> rules, string filterString = "")
        {

            DashboardHelper dh;

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

            List<DatatableBag> datatableBagList = new List<DatatableBag>();

            List<DataTable> dtList = dh.GenerateLineList(gadgetParameters, rules);

            foreach (DataTable dt in dtList)
            {
                if (dt != null && dt.Rows.Count > 0)
                {


                    int numberOfRows = Convert.ToInt32(gadgetParameters.InputVariableList["maxrows"].ToString());
                    DataTable newDt = dt.AsEnumerable().Take(numberOfRows).ToList().CopyToDataTable();

                    newDt.TableName = dt.TableName;
                    DatatableBag d = new DatatableBag(newDt, "");


                    datatableBagList.Add(d);
                }
            }


            return datatableBagList;
        }
    }
}