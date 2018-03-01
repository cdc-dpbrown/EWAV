/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       DatatableBag.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using System;
using System.Runtime.CompilerServices;

namespace Ewav.Web.Services
{
    public class DatatableBag : INotifyPropertyChanged
    {    
        private List<MyString> columnNameList = new List<MyString>();
        private List<EpiDashboard.DescriptiveStatistics> descriptiveStatisticsList;
        private FieldsList fieldsList;
        private List<FieldsList> recordList = new List<FieldsList>();
        private string _tableName;
        private string _exposure;
        private DataTable value;
        private GridCells _gridCellData = new GridCells();
        private List<DictionaryDTO> extraInfo;

        public List<DictionaryDTO> ExtraInfo
        {
            get { return extraInfo; }
            set { extraInfo = value; }
        }
   
        /// <summary>
        /// Required for WCF RIA         
        /// </summary>
        public DatatableBag()
        {
        }

        public DatatableBag(DataTable dt, string crosstab, List<EpiDashboard.DescriptiveStatistics> descStatsList = null, GridCells gridCells = null, int observations = 4)
        {
            _tableName = dt.TableName;
            _gridCellData = gridCells;
            _exposure = crosstab;

            this.descriptiveStatisticsList = descStatsList;

            foreach (DataColumn col in dt.Columns)
            {
                columnNameList.Add(new MyString(col.ColumnName));
            }
            foreach (DataRow row in dt.Rows)
            {
                fieldsList = new FieldsList();

                foreach (DataColumn col in dt.Columns)
                {
                    fieldsList.Fields.Add(new MyString(row[col.ColumnName].ToString()));
                }

                recordList.Add(fieldsList);
            }
        }

        public GridCells GridCellData
        {
            get
            {
                return _gridCellData;
            }
            set
            {
                _gridCellData = value;
                NotifyPropertyChanged("GridCellData");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public List<MyString> ColumnNameList
        {
            get
            {
                return this.columnNameList;
            }
            set
            {
                this.columnNameList = value;
                NotifyPropertyChanged("ColumnNameList");
            }
        }

        public List<EpiDashboard.DescriptiveStatistics> DescriptiveStatisticsList
        {
            get
            {
                return this.descriptiveStatisticsList;
            }
            set
            {
                this.descriptiveStatisticsList = value;
                NotifyPropertyChanged("DescriptiveStatisticsList");
            }
        }

        public FieldsList FieldsList
        {
            get
            {
                return this.fieldsList;
            }
            set
            {
                this.fieldsList = value;
            }
        }

        public List<FieldsList> RecordList
        {
            get
            {
                return this.recordList;
            }
            set
            {
                this.recordList = value;
            }
        }

        public string TableName
        {
            get
            {
                return _tableName;
            }
            set
            {
                _tableName = value;
                NotifyPropertyChanged("TableName");
            }
        }

        public string Exposure
        {
            get
            {
                return _exposure;
            }
            set
            {
                _exposure = value;
                NotifyPropertyChanged("Exposure");
            }
        }

        public string GetValue(string colName)
        {
            string thisColumn;
            int x = 0;

            for (x = 0; x < columnNameList.Count; x++)
            {
                thisColumn = columnNameList[x].VarName;
                if (thisColumn == colName) break;
            }

            return recordList[0].Fields[x].VarName;
        }

        public DatatableBag ConvertToDatatableBag(DataTable dt)
        {
            _tableName = dt.TableName;

            foreach (DataColumn col in dt.Columns)
            {
                columnNameList.Add(new MyString(col.ColumnName));
            }
            foreach (DataRow row in dt.Rows)
            {
                fieldsList = new FieldsList();

                foreach (DataColumn col in dt.Columns)
                {
                    fieldsList.Fields.Add(new MyString(row[col.ColumnName].ToString()));
                }

                recordList.Add(fieldsList);
            }

            return this;
        }
    }

    public class FieldsList
    {
        private List<MyString> fields;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldsList" /> class.
        /// </summary>
        public FieldsList()
        {
            fields = new List<MyString>();
        }

        /// <summary>
        /// Gets or sets the fields.
        /// </summary>
        /// <value>The fields.</value>
        public List<MyString> Fields
        {
            get
            {
                return this.fields;
            }
            set
            {
                this.fields = value;
            }
        }
    }
}