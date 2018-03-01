/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       DataTableConverter.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ewav.Web.Services
{
    /// <summary>
    /// Converts a DataTable to other formats    
    /// </summary>
    public class DataTableConverter
    {
        private string[] columnNamesAsArray;
        private IEnumerable<string> columnNamesAsList;
        private string[][] dataTableAsArray;
        private IEnumerable<IEnumerable<string>> dataTableAsIEnumerable;    


        /// <summary>
        /// Initializes a new instance of the <see cref="DataTableConverter" /> class.
        /// </summary>
        public DataTableConverter(DataTable dt)
        {
            columnNamesAsList = dt.Columns
                                  .OfType<DataColumn>()
                                  .Select(c => c.ColumnName);

            columnNamesAsArray = columnNamesAsList.ToArray<string>();

            dataTableAsArray = dt.Rows
                .OfType<DataRow>()
                .Select(r => dt.Columns
                        .OfType<DataColumn>()
                        .Select(c => r[c.ColumnName].ToString())
                        .ToArray())
                .ToArray();

            dataTableAsIEnumerable = dt.Rows
                .OfType<DataRow>()
                .Select(r => dt.Columns
                        .OfType<DataColumn>()
                        .Select(c => r[c.ColumnName].ToString())
                        .ToList<string>()
                        .AsEnumerable<string>());
        }

        /// <summary>
        /// Gets or sets the data table as I enumerable.
        /// </summary>
        /// <value>The data table as I enumerable.</value>
        public IEnumerable<IEnumerable<string>> DataTableAsIEnumerable
        {
            get
            {
                return this.dataTableAsIEnumerable;
            }
            set
            {
                this.dataTableAsIEnumerable = value;
            }
        }

        /// <summary>
        /// Gets or sets the column names as array.
        /// </summary>
        /// <value>The column names as array.</value>
        public string[] ColumnNamesAsArray
        {
            get
            {
                return this.columnNamesAsArray;
            }
            set
            {
                this.columnNamesAsArray = value;
            }
        }
        /// <summary>
        /// Gets or sets the column names as list.
        /// </summary>
        /// <value>The column names as list.</value>
        public IEnumerable<string> ColumnNamesAsList
        {
            get
            {
                return this.columnNamesAsList;
            }
            set
            {
                this.columnNamesAsList = value;
            }
        }
        /// <summary>
        /// Gets or sets the data table as array.
        /// </summary>
        /// <value>The data table as array.</value>
        public string[][] DataTableAsArray
        {
            get
            {
                return this.dataTableAsArray;
            }
            set
            {
                this.dataTableAsArray = value;
            }
        }
    }
}