using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;    

using  Ewav.Web.Services        ;    

namespace EIWSDataConverter
{
    class Program
    {
        static void Main(string[] args)
        {


            string surveyId = "";
            SqlConnection connection = new SqlConnection("aaaa ");

            SqlCommand metaDataReaderCommand = new SqlCommand(string.Format("exec usp_create_metadata '{0}' ", surveyId), connection);

            SqlDataAdapter sda = new SqlDataAdapter(metaDataReaderCommand);
            DataTable dtMetaData = new DataTable();
            sda.Fill(dtMetaData);




        }

        //public DatatableBag CreateDatatableBag()
        //{

        //}

    }
}    





