using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Ewav.DTO;
using System.Net.Http.Headers;
using Google.DataTable.Net.Wrapper;
using Ewav.Web.Services;
using Ewav.Web.EpiDashboard;
using Newtonsoft.Json.Linq;
using Ewav.Web.Services.CombinedFrequencyDomainService;
using System.Globalization;


namespace EWAV.WebApi.Controllers
{
    public class EpiCurveController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]      JObject value)
        {
            EpiCurveDomainService EpiCurveDomainService = new EpiCurveDomainService();
            Google.DataTable.Net.Wrapper.DataTable dt = null;
            HttpResponseMessage ReturnedObj = null;
            GadgetParameters GadgetParameters = new GadgetParameters();
            List<EwavRule_Base> Rules = new List<EwavRule_Base>();
            ControllerCommon CommonClass = new ControllerCommon();

            JObject chartJSON = (JObject)value["chart"];

            List<EwavDataFilterCondition> dashboardFilters = new List<EwavDataFilterCondition>();
            bool byEpiWeek = false;

            GadgetParameters.MainVariableName = string.Empty;
            GadgetParameters.WeightVariableName = string.Empty;
            GadgetParameters.StrataVariableNames = new List<string>();
            GadgetParameters.CrosstabVariableName = string.Empty;
            GadgetParameters.InputVariableList = new Dictionary<string, string>();
            GadgetParameters.ShouldSortHighToLow = false;






            GadgetParameters.DatasourceName = chartJSON["@DatasourceName"].ToString();

            string strataVariable = string.Empty;

            GadgetParameters.MainVariableName = chartJSON["dateVariable"].ToString();


            GadgetParameters.InputVariableList = new Dictionary<string, string>();
            // tablename really menas DatasourName  
            GadgetParameters.InputVariableList.Add("tableName", chartJSON["@DatasourceName"].ToString());



            GadgetParameters.ShouldUseAllPossibleValues = false;



            GadgetParameters.InputVariableList.Add("xaxisstart", chartJSON["xAxisStartValue"].ToString());

            GadgetParameters.InputVariableList.Add("xaxisend", chartJSON["xAxisEndValue"].ToString());

            GadgetParameters.InputVariableList.Add("isdatecolumnnumeric", "false");
            GadgetParameters.InputVariableList.Add("dateinterval", chartJSON["dateInterval"].ToString());

            GadgetParameters.CrosstabVariableName = chartJSON["caseStatusVariable"].ToString();

            GadgetParameters.TableName = CommonClass.GetDatabaseObject(GadgetParameters.DatasourceName);

            dashboardFilters = CommonClass.GetFilters(value);

            GadgetParameters.GadgetFilters = CommonClass.GetFilters(chartJSON, true);

            Rules = CommonClass.ReadRules(value);

            if (GadgetParameters.InputVariableList.ContainsKey("isdatecolumnnumeric"))
            {
                byEpiWeek = bool.Parse(GadgetParameters.InputVariableList["isdatecolumnnumeric"]);
            }


            DatatableBag data = EpiCurveDomainService.GetEpiCurveData(GadgetParameters, dashboardFilters, Rules, CommonClass.AdvancedDataFilterString, byEpiWeek, chartJSON["dateVariable"].ToString(), chartJSON["caseStatusVariable"].ToString());



            dt = new Google.DataTable.Net.Wrapper.DataTable();



            dt.AddColumn(new Column(ColumnType.String, "FreqVariable", "FreqVariable"));

            for (int i = 1; i < data.ColumnNameList.Count; i++)
            {
                dt.AddColumn(new Column(ColumnType.Number, "FrequencyColumn" + i, data.ColumnNameList[i].VarName.ToString()));
            }


            foreach (var item in data.RecordList)
            {
                //if (item.Fields[1].VarName.ToString().Length > 0)
                //{
                    Row r = dt.NewRow();

                    for (int i = 0; i < data.ColumnNameList.Count; i++)
                    {
                        string dateValue = item.Fields[i].VarName.ToString();

                        DateTime result = DateTime.Now;

                        if (DateTime.TryParse(dateValue, out result))
                        {
                            switch (chartJSON["dateInterval"].ToString().ToLower())
                            {
                                case "hours":
                                    dateValue = result.ToString("g");
                                    break;
                                case "months":
                                    dateValue = result.ToString("MMM yyyy");
                                    break;
                                case "years":
                                    dateValue = result.ToString("yyyy");
                                    break;
                                default://"days"
                                    dateValue = result.ToShortDateString();
                                    break;
                            }
                        }


                        r.AddCell(new Cell(dateValue));
                    }
                    //);

                    dt.AddRow(r);
                //}

            }

            ReturnedObj = new HttpResponseMessage()
            {
                Content = new StringContent(dt.GetJson())
            };




            ReturnedObj.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");


            return ReturnedObj;


        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}