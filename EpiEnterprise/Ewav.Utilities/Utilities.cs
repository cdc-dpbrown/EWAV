using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data;

namespace Ewav.Utilities
{
    public static class Utilities
    {

        public static long GetSize(object o)
        {



            long size = 0;
            //  object o = new object();  
            using (Stream s = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(s, o);
                size = s.Length;

                return size;
            }


        }


        public static DataTable CreateDT(int maxCols, int maxRows)
        {

            try
            {
                //Create instance of data table
                DataTable testTable = new DataTable();

                Random cols = new Random();
                Random row = new Random();


                int colCount = cols.Next(1, maxCols);
                int rowCount = cols.Next(1, maxRows);

                int dataType;

                //Add columns to data table    
                for (int c = 0; c < colCount  - 1; c++)
                {

                    //  later use datatypes to create etter  row data     
                    dataType = cols.Next(1, 3);

                    switch (dataType)
                    {

                        case 1:
                            testTable.Columns.Add("Column" + c.ToString(), typeof(int));
                            break;
                        case 2:
                            testTable.Columns.Add("Column" + c.ToString(), typeof(bool));
                            break;
                        case 3:

                            testTable.Columns.Add("Column" + c.ToString(), typeof(string));
                            break;

                    }




                }


                DataRow dr = testTable.NewRow();

                for (int r = 0; r < rowCount - 1; r++)
                {

                    //Create a row in data table
                    dr = testTable.NewRow();

                    for (int c = 0; c < colCount  - 1; c++)
                    {


                        //Fill all columns with value
                        dr["Column" + c.ToString()] = 1;


                    }

                    //Add the row into data table
                    testTable.Rows.Add(dr);


                }

                return testTable;



            }

            catch (Exception ex)
            {

                throw new Exception(ex.Message);    

                  
            }


        }
    }


}
