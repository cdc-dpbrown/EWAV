/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       EwavDatasourceDto.cs
 *  Namespace:  Ewav.BAL    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
namespace Ewav.BAL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class EwavDatasourceDto 
    {
        private List<EwavColumn> allColumns;
        /// <summary>
        /// Gets or sets all columns.
        /// </summary>
        /// <value>All columns.</value>
        public List<EwavColumn> AllColumns
        {
            get
            {
                return this.allColumns;
            }
            set
            {
                this.allColumns = value;

                createNoCamelName();
            }
        }
        /// <summary>
        /// Gets or sets the type of the data base.
        /// </summary>
        /// <value>The type of the data base.</value>
        public string DataBaseType { get; set; }
        /// <summary>
        /// Gets or sets the name of the datasource.
        /// </summary>
        /// <value>The name of the datasource.</value>
        public string DatasourceName { get; set; }


        /// <summary>
        /// Gets or sets the organization id.
        /// </summary>
        /// <value>
        /// The organization id.
        /// </value>
        public int OrganizationId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public   int     DatasourceID  { get; set; }                  
        /// <summary>
        /// Gets or sets the name of the datasource no camel.
        /// </summary>
        /// <value>The name of the datasource no camel.</value>
        public string DatasourceNoCamelName { get; set; }
        /// <summary>
        /// Gets or sets the filtered records.
        /// </summary>
        /// <value>The filtered records.</value>
        public long FilteredRecords { get; set; }
        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName { get; set; }
        /// <summary>
        /// Gets or sets the total records.
        /// </summary>
        /// <value>The total records.</value>
        public long TotalRecords { get; set; }
        /// <summary>
        /// Creates the name of the no camel.
        /// </summary>
        private void createNoCamelName()
        {
            foreach (EwavColumn ewc in allColumns)
            {
                ewc.NoCamelName = FromCamelCase(ewc.Name);
            }
        }

        private string FromCamelCase(string pascalCaseString)
        {
            Regex r = new Regex("(?<=[a-z])(?<x>[A-Z])|(?<=.)(?<x>[A-Z])(?=[a-z]) ");
            return r.Replace(pascalCaseString, " ${x}");
        }
    }
}