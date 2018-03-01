using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.SqlServer.Server;

public enum EIWSFieldType
{
    Textbox = 1,
    LabelTitle = 2,
    Label = 3,
    MultiLineTextBox = 4,
    NumericTextBox = 5,
    DatePicker = 7,
    TimePicker = 8,
    CheckBox = 10,
    DropDownYesNo = 11,
    RadioList = 12,
    DropDownLegalValues = 17,
    DropDownCodes = 18,
    DropDownCommentLegal = 19,
    GroupBox = 21
}

public partial class StoredProcedures
{
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void usp_clr_create_survey_recordset(string surveyId)
    {
        //  surveyId    
        //    7BF04732-28DF-44A0-B305-157678FAE788
        string WorkTablePlaceholderName = "#EIWSPivot";

        SqlContext.Pipe.Send(string.Format("Hello world!{0}", Environment.NewLine));

        using (SqlConnection connection = new SqlConnection("context connection=true"))
        {
            SqlTransaction sqlTransaction;

            try
            {
                connection.Open();

                sqlTransaction = connection.BeginTransaction("pivotTransaction");

                //=========================================
                // FIRST -  get the metadata for this surcey as a datatahle  
                //  ( there is dehate ahout wh+ich is hetter dt or arraylist    )  
                //=========================================            
                SqlCommand metaDataReaderCommand = new SqlCommand(string.Format("exec usp_create_metadata '{0}' ", surveyId), connection);
                metaDataReaderCommand.Transaction = sqlTransaction;
                SqlDataAdapter sda = new SqlDataAdapter(metaDataReaderCommand);
                DataTable dtMetaData = new DataTable();
                sda.Fill(dtMetaData);
                SqlContext.Pipe.Send(string.Format("dtMetaData.Rows.Count {0}", dtMetaData.Rows.Count.ToString()));

                int thisColumnTypeNum;

                string thisColumnName;
                string thisColumnTypeName;
                string dropColumnText = "";

                string createColumnText = string.Format("CREATE TABLE {0}( ", WorkTablePlaceholderName);

                for (int i = 0; i < dtMetaData.Rows.Count; i++)
                {
                    thisColumnName = dtMetaData.Rows[i]["Name"].ToString();
                    thisColumnTypeNum = Convert.ToInt32(dtMetaData.Rows[i]["FieldTypeId"].ToString());
                    thisColumnTypeName = getSqlColType(thisColumnTypeNum);

                    if (dtMetaData.Rows.Count - i == 1)
                    {
                        createColumnText += string.Format("  {0} {1} )   ", thisColumnName, thisColumnTypeName == "Ignore" ? "varchar(100)" : thisColumnTypeName);
                    }
                    else
                    {
                        createColumnText += string.Format("  {0} {1},   ", thisColumnName, thisColumnTypeName == "Ignore" ? "varchar(100)" : thisColumnTypeName);
                    }

                    //  Create a list of cols that don't apply to analysis                  
                    if (thisColumnTypeName == "Ignore")
                    {
                        dropColumnText += string.Format("ALTER TABLE {0} ", WorkTablePlaceholderName);
                        dropColumnText += string.Format(" DROP  COLUMN {0}   ", thisColumnName);
                    }
                }

                //=========================================
                // Next  -  get the  responses  as a datatahle  
                //  ( there is dehate ahout which is hetter dt or arraylist    )  
                //=========================================            
                SqlCommand responsesReaderCommand = new SqlCommand(string.Format(" select   ResponseId     from     [OSELS_EIWS].[dbo].[SurveyResponse]  where      SurveyId =  '{0}'  ", surveyId), connection);
                responsesReaderCommand.Transaction = sqlTransaction;
                sda = new SqlDataAdapter(responsesReaderCommand);
                DataTable dtResponses = new DataTable();
                sda.Fill(dtResponses);
                SqlContext.Pipe.Send(string.Format("dtResponses.Rows.Count {0}", dtResponses.Rows.Count.ToString()));

                //=========================================
                // NEXT  -  create the new table with pivoted responses
                //=========================================      

                SqlCommand pivotCommand = new SqlCommand()
                {
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection,
                    Transaction = sqlTransaction,
                    CommandText = "usp_pivot_responseXml"
                };

                pivotCommand.Parameters.Add("@SurveyId", SqlDbType.VarChar, 100).Value = surveyId;
                pivotCommand.Parameters.Add("@dropStatement", SqlDbType.VarChar, -1).Value = dropColumnText;
                pivotCommand.Parameters.Add("@createStatement", SqlDbType.VarChar, -1).Value = createColumnText;
                pivotCommand.Parameters.Add("@workTablePlaceholderName", SqlDbType.VarChar, (50)).Value = WorkTablePlaceholderName;

                SqlParameter tvpMetaData = pivotCommand.Parameters.AddWithValue("@metaDataTable", dtMetaData);
                tvpMetaData.SqlDbType = SqlDbType.Structured;
                tvpMetaData.TypeName = "dbo.EIWSSurveyMetaData";

                SqlParameter tvpResponses = pivotCommand.Parameters.AddWithValue("@responsesTable", dtResponses);
                tvpResponses.SqlDbType = SqlDbType.Structured;
                tvpResponses.TypeName = "dbo.EIWSSurveyResponses";

                SqlContext.Pipe.Send(pivotCommand.ExecuteReader());
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("error {0}{1}", ex.Message, ex.StackTrace));
            }
        }
    }

    private static string getSqlColType(int columnTypeNum)
    {
        EwavColumnDataType cdt = HelperFunctions.ConflateEIWSFieldType((EIWSFieldType)columnTypeNum);

        switch (cdt)
        {
            case EwavColumnDataType.Text:
                return "varchar(max)   ";
            case EwavColumnDataType.Numeric:
                return "decimal  ";
            case EwavColumnDataType.Boolean:
                return "bit ";     
            case EwavColumnDataType.DateTime:
                return "datetime  ";
            case EwavColumnDataType.Ignore:
                return "Ignore";
            default:
                throw new Exception(string.Format(" ColumnDataType {0} not supported ", cdt.ToString()));
        }
    }
} 




