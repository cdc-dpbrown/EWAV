/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       IRawDataDao.cs
 *  Namespace:  Ewav.DAL.Interfaces    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.Data;

namespace Ewav.DAL.Interfaces
{
    public interface IRawDataDao
    {
        DataTable GetColumnsForDatasource(DataRow[] dr);

        string GetRecordsCount(string datasourceName, string tableName, string FilterCriteria);

        DataTable GetTable(string Datasoucename, string connStr, string tableName);

        DataTable GetTopTwoTable(string Datasoucename, string connStr, string tableName);


        int CountRecords(string connStr, string sqlOrTableName);

        string GetTotalRecordsCount(string externalConnStr, string datasourceName);
    }
}

