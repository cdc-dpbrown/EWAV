/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       EwavFrequencyControlDto.cs
 *  Namespace:  Ewav.DTO    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;

using System.Linq;

namespace Ewav.DTO
{
    public class EwavFrequencyControlDto
    {
        public string FrequencyColumn { get; set; }
        public string FreqVariable { get; set; }
        public string NameOfDtoList { get; set; }
        //private string columnId;
        //private string columnName;
        //public string ColumnId {
        //    get 
        //    {
        //        return columnId;
        //    }
        //    set 
        //    {
        //        columnId = value;
        //    }
        //}
        //public string ColumnName {
        //    get 
        //    {
        //        return columnName;
        //    }
        //    set 
        //    {
        //        columnName = value;
        //    }
        //}
        public string Perc95ClLowerColumn { get; set; }
        public string Perc95ClUpperColumn { get; set; }
        public string PercentColumn { get; set; }
    }
    //    public string EpiFieldType
    //    {
    //        get { return epiFieldType; }
    //        set { epiFieldType = value; }
    //    }
    //}
    //    public string FormName
    //    {
    //      get { return formName; }
    //      set { formName = value; }
    //    }
    //    private string epiFieldType;
    //    public string TableName
    //    {
    //      get { return tableName; }
    //      set { tableName = value; }
    //    }
    //    private string formName;
    //    }
    //    private string tableName;
    //      set { dataType = value; }
    //      get { return dataType; }
    //    public string DataType
    //    {
    //    private string dataType;
    //    }
    //    {
    //      get { return columnName; }
    //      set { columnName = value; }
    //    public string ColumnName
    //public class EwavColumnsMetaData 
    //{
    //    private string columnName;
}