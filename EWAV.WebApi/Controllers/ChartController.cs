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
    [AllowAnonymous]
    public class ChartController : ApiController
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
            XYChartDomainService XYChartDomainService = new XYChartDomainService();
            Google.DataTable.Net.Wrapper.DataTable dt = null;
            HttpResponseMessage ReturnedObj = null;
            GadgetParameters GadgetParameters = new GadgetParameters();
            List<EwavRule_Base> Rules = new List<EwavRule_Base>();
            ControllerCommon CommonClass = new ControllerCommon();

            JObject chartJSON = (JObject)value["chart"];

            List<EwavDataFilterCondition> dashboardFilters = new List<EwavDataFilterCondition>();

            dt = new Google.DataTable.Net.Wrapper.DataTable();

            Rules = CommonClass.ReadRules(value);

            dashboardFilters = CommonClass.GetFilters(value);

            GadgetParameters.GadgetFilters = CommonClass.GetFilters(chartJSON, true);
            

            GadgetParameters.DatasourceName = chartJSON["@DatasourceName"].ToString();

            string strataVariable = string.Empty;

            if (chartJSON["chartType"].ToString().ToLower() == "stackedcolumn")
            {
                GadgetParameters.MainVariableName = chartJSON["xAxisVariable"].ToString();
                strataVariable = chartJSON["yAxisVariable"].ToString();
            }
            else if (chartJSON["chartType"].ToString().ToLower() == "pareto")
            {
                GadgetParameters.ShouldSortHighToLow = true;
                GadgetParameters.MainVariableName = chartJSON["singleVariable"].ToString();
                strataVariable = chartJSON["strataVariable"].ToString();
            }
            else
            {
                GadgetParameters.MainVariableName = chartJSON["singleVariable"].ToString();
                strataVariable = chartJSON["strataVariable"].ToString();
            }

            GadgetParameters.InputVariableList = new Dictionary<string, string>();
            GadgetParameters.WeightVariableName = chartJSON["weightVariable"].ToString();

            GadgetParameters.TableName = CommonClass.GetDatabaseObject(GadgetParameters.DatasourceName);


            if (strataVariable.Trim().Length > 0)
            {
                List<MyString> StrataList = new List<MyString>();
                MyString strata = new MyString();
                strata.VarName = strataVariable.ToString(); // chartJSON["strataVariable"].ToString();
                StrataList.Add(strata);
                GadgetParameters.StrataVariableList = StrataList;
            }

            
            List<FrequencyResultData> FrequencyResultDataList = XYChartDomainService.GenerateFrequencyTable(GadgetParameters, dashboardFilters, Rules, CommonClass.AdvancedDataFilterString);

            double runningPercent = 0, max = 0;

            foreach (var item in FrequencyResultDataList)
            {
                foreach (var row in item.FrequencyControlDtoList)
                {
                    if ((row.FreqVariable.Split('/').Length > 2 && row.FreqVariable.Split('/').Length < 4) // Valid Combos:- enumerator/enumerator/enumerator, enumerator-enumerator-enumerator ,
                                         || (row.FreqVariable.Split('-').Length > 2 && row.FreqVariable.Split('-').Length < 4)) //Invalid Combos:- enumerator/enumerator, enumerator/enumerator/enumerator/enumerator, enumerator-enumerator, enumerator-enumerator-enumerator-enumerator
                    {
                        row.FreqVariable = DateTime.Parse(row.FreqVariable, CultureInfo.CurrentUICulture).ToShortDateString();
                    }
                    else
                    {
                        row.FreqVariable = row.FreqVariable;
                    }

                    if (chartJSON["chartType"].ToString().ToLower() == "pareto") //this.chartTypeEnum == XYControlChartTypes.Pareto)
                    {
                        max = max + CommonClass.SafeParsetoDou(row.FrequencyColumn); //.SafeParsetoDou();
                    }

                }
            }



            if (FrequencyResultDataList.Count > 1)
            {
                dt.AddColumn(new Column(ColumnType.String, "FreqVariable", "FreqVariable"));

                var DtoCount = FrequencyResultDataList.Count;

                for (int i = 0; i < DtoCount; i++)
                {
                    var columnName = FrequencyResultDataList[i].FrequencyControlDtoList[0].NameOfDtoList;

                    columnName = columnName.Split('=')[1].ToString();

                   
                    dt.AddColumn(new Column(ColumnType.Number, columnName, columnName));
                }

                for (int i = 0; i < FrequencyResultDataList[0].FrequencyControlDtoList.Count; i++)
                {
                    Row r = dt.NewRow();
                    r.AddCell(
                   new Cell(FrequencyResultDataList[0].FrequencyControlDtoList[i].FreqVariable)
                       );
                    for (int j = 0; j < DtoCount; j++)
                    {
                        r.AddCell(
                new Cell(FrequencyResultDataList[j].FrequencyControlDtoList[i].FrequencyColumn)
                    );

                    }



                    dt.AddRow(r);
                }


            }
            else
            {
                dt.AddColumn(new Column(ColumnType.String, "FreqVariable", "FreqVariable"));
                dt.AddColumn(new Column(ColumnType.Number, "FrequencyColumn", FrequencyResultDataList[0].FrequencyControlDtoList[0].NameOfDtoList));

                if (chartJSON["chartType"].ToString().ToLower() == "pareto") // if chart is Pareto
                {
                    dt.AddColumn(new Column(ColumnType.Number, "mean", "mean"));
                }


                foreach (EwavFrequencyControlDto Frequencydto in FrequencyResultDataList[0].FrequencyControlDtoList)
                {
                    Row r = dt.NewRow();
                    r.AddCellRange(new Cell[]
                        {
                            new Cell(Frequencydto.FreqVariable),
                            new Cell(Frequencydto.FrequencyColumn)//,
           
                        }
                    );

                    if (chartJSON["chartType"].ToString().ToLower() == "pareto")// if chart is Pareto
                    {
                        double meanValue = CommonClass.CalculateMean(Frequencydto.FrequencyColumn, max, runningPercent);
                        meanValue = Math.Round(meanValue, 4);
                        r.AddCell(
                  new Cell(meanValue)
                      );
                        runningPercent = CommonClass.SafeParsetoDou(meanValue.ToString());
                    }
                    dt.AddRow(r);
                }
            }


            ReturnedObj = new HttpResponseMessage()
            {
                Content = new StringContent(dt.GetJson()) //Newtonsoft.Json.JsonConvert.SerializeObject(clientDto))//FrequencyResultData.FrequencyControlDtoList))
            };


            //  return resultAsStr;


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