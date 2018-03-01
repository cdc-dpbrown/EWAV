/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       DashboardInfoCollection.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.ObjectModel;


namespace Ewav
{
    public class DashboardInfoCollection : ObservableCollection<DashboardInfo>
    {
        public DashboardInfoCollection(System.Collections.Generic.List<CanvasDto> results) : base()
        {
            for (int i = 0; i < results.Count; i++)
            {
                Add(
                    new DashboardInfo(results[i].CanvasName,
                        results[i].CanvasDescription,
                        Convert.ToString(results[i].CreatedDate),
                        results[i].Datasource, results[i].Status));
            }
            //Add(new DashboardInfo(results))
            //
        }
    }
}