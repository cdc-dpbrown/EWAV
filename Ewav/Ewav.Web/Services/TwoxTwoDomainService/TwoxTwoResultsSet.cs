/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       TwoxTwoResultsSet.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Ewav.Web.EpiDashboard;

namespace Ewav.Web.Services
{
  
    public class TwoxTwoAndMxNResultsSet
    {
        private List<string> allColumns;
        private string[] columnNamesAsArray;
        private IEnumerable<string> columnNamesAsList;
        private DatatableBag datatableBag;
        private DatatableBag[] datatableBagArray ;// = new DatatableBag[];
        private List<EpiDashboard.DescriptiveStatistics> descriptiveStatisticsList;
   
        private List<MyString> errors = new List<MyString>();
        private List<MyString> errorTypes = new List<MyString>();
        private bool exceededMaxColumns1;
        private bool exceededMaxRows1;
        private DataTable freqResultsDataTable;

        private List<DataTable> freqResultsDataTableList = new List<DataTable>();
        private List<EpiDashboard.DescriptiveStatistics> freqResultsDescriptiveStatistics;
        private GridCells gridCells;

        private int gridRowHeight;
        private bool is2X2;
        private List<ListofDescriptiveStatistics> listofDescriptiveStatistics;
        private List<MxNGridSetupParameter> mxNGridCells;
        private List<MxNGridRow> mxNGridRows;
        private List<MxNSetTextParameter> mxNSetTextParameters;
        private int rowCount;
        private string strataValue1;
        private string tableHeading1;
        private string tableName;
        private int[] totals;
        private TwoxTwoTableDTO twoxTwoTableDto;


        public TwoxTwoAndMxNResultsSet()
        {
            datatableBagArray = new DatatableBag[0];  
        }
   
        public List<string> AllColumns
        {
            get
            {
                return allColumns;
            }
        }
    
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
        /// Gets or sets the datatable bag.
        /// </summary>
        /// <value>The datatable bag.</value>
        public DatatableBag DatatableBag
        {
            get
            {
                return this.datatableBag;
            }
        }
        /// <summary>
        /// Gets or sets the datatable bag array.
        /// </summary>
        /// <value>The datatable bag array.</value>
        public DatatableBag[] DatatableBagArray
        {
            get
            {
                return this.datatableBagArray;
            }
            set
            {
                this.datatableBagArray = value;
            }
        }
        public List<MyString> Errors
        {
            get
            {
                return this.errors;
            }
            set
            {
                this.errors = value;
            }
        }
        public List<MyString> ErrorTypes
        {
            get
            {
                return this.errorTypes;
            }
            set
            {
                this.errorTypes = value;
            }
        }
        public bool exceededMaxColumns
        {
            get
            {
                return this.exceededMaxColumns1;
            }
            set
            {
                this.exceededMaxColumns1 = value;
            }
        }
        public bool exceededMaxRows
        {
            get
            {
                return this.exceededMaxRows1;
            }
            set
            {
                this.exceededMaxRows1 = value;
            }
        }
        public DataTable FreqResultsDataTable
        {
            get
            {
                return this.freqResultsDataTable;
            }
            set
            {
                this.freqResultsDataTable = value;

                columnNamesAsList = value.Columns
                                         .OfType<DataColumn>()
                                         .Select(c => c.ColumnName);

                columnNamesAsArray = columnNamesAsList.ToArray<string>();
                this.datatableBag = new DatatableBag(value, "", new List<DescriptiveStatistics>(), new GridCells());
            }
        }
        
    
        public List<EpiDashboard.DescriptiveStatistics> FreqResultsDescriptiveStatistics
        {
            get
            {
                return this.freqResultsDescriptiveStatistics;
            }
            set
            {
                this.freqResultsDescriptiveStatistics = value;
            }
        }

        public GridCells GridCells
        {
            get
            {
                return this.gridCells;
            }
            set
            {
                this.gridCells = value;
            }
        }
        public int GridRowHeight
        {
            get
            {
                return this.gridRowHeight;
            }
            set
            {
                this.gridRowHeight = value;
            }
        }
        public bool Is2x2
        {
            get
            {
                return this.is2X2;
            }
            set
            {
                this.is2X2 = value;
            }
        }
        public List<MxNGridSetupParameter> MxNGridCells
        {
            get
            {
                return this.mxNGridCells;
            }
            set
            {
                this.mxNGridCells = value;
            }
        }
        public List<MxNGridRow> MxNGridRows
        {
            get
            {
                return this.mxNGridRows;
            }
            set
            {
                this.mxNGridRows = value;
            }
        }
        public List<MxNSetTextParameter> MxNSetTextParameters
        {
            get
            {
                return this.mxNSetTextParameters;
            }
            set
            {
                this.mxNSetTextParameters = value;
            }
        }
        public int RowCount
        {
            get
            {
                return this.rowCount;
            }
            set
            {
                this.rowCount = value;
            }
        }
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
        public string tableHeading
        {
            get
            {
                return this.tableHeading1;
            }
            set
            {
                this.tableHeading1 = value;
            }
        }
    

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName
        {
            get
            {
                return this.tableName;
            }
            set
            {
                this.tableName = value;
            }
        }
        public int[] Totals
        {
            get
            {
                return this.totals;
            }
            set
            {
                this.totals = value;
            }
        }
        public TwoxTwoTableDTO TwoxTwoTableDTO
        {
            get
            {
                return this.twoxTwoTableDto;
            }
            set
            {
                this.twoxTwoTableDto = value;
            }
        }
        public void AddResult(DataTable table, string crosstab, List<DescriptiveStatistics> statsList, GridCells gridCells)
        {
            freqResultsDataTableList.Add(table);
            this.FreqResultsDescriptiveStatistics = statsList;
            Array.Resize(ref datatableBagArray, datatableBagArray.Length + 1);  
            DatatableBag bag = new DatatableBag(table, crosstab, statsList, gridCells);
            datatableBagArray[datatableBagArray.Length - 1] = bag;       
        }
    }
}