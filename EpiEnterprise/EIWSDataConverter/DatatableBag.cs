/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       DatatableBag.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.Collections.Generic;
using System.Data;

namespace Ewav.Web.Services
{
    public class DatatableBag
    {    
        private List<MyString> columnNameList = new List<MyString>();
        private FieldsList fieldsList;
        private List<FieldsList> recordList = new List<FieldsList>();
        private string tableName;
        private DataTable value;
   
        /// <summary>
        /// Required for WCF RIA         
        /// </summary>
        public DatatableBag()
        {
        }


        public DatatableBag(DataTable dt  )    
        {
            this.tableName = dt.TableName;     
      
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

            ////  TEMP --   Only reverse or 2x2 data   
            //if (observations != 2)
            //{
            //    recordList.Reverse();
            //}
        }

        /// <summary>
        /// Gets or sets the column name list.
        /// </summary>
        /// <value>The column name list.</value>
        public List<MyString> ColumnNameList
        {
            get
            {
                return this.columnNameList;
            }
            set
            {
                this.columnNameList = value;
            }
        }
  
  
        /// <summary>
        /// Gets or sets the fields list.
        /// </summary>
        /// <value>The fields list.</value>
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

        /// <summary>
        /// Gets or sets the record list.
        /// </summary>
        /// <value>The record list.</value>
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

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName
        {
            get
            {
                return tableName;
            }
            set
            {
                this.tableName = value;
            }
        }

        public string GetValue(string colName)
        {
            string thisColumn;

            int x = 0;

            for (x = 0; x < columnNameList.Count; x++)
            {
                thisColumn = columnNameList[x].VarName;

                if (thisColumn == colName)
                    break;
            }

            //FieldsList[]      recordListArray = 

            //                   recordList[0].Fields[x]  

            return recordList[0].Fields[x].VarName;                  //     recordListArray[0 ].Fields.
        }

   

        public DatatableBag ConvertToDatatableBag(DataTable dt)
        {
            this.tableName = dt.TableName;

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