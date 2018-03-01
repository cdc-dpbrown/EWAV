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
    public class ScatterController : ApiController
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
            ScatterDomainService ScatterDomainService = new Ewav.Web.Services.ScatterDomainService();
            GadgetParameters GadgetParameters;
            ControllerCommon CommonClass = new ControllerCommon();
            List<EwavRule_Base> Rules = new List<EwavRule_Base>();
            Google.DataTable.Net.Wrapper.DataTable dt = null;
            JObject chartJSON = (JObject)value["chart"];

            List<EwavDataFilterCondition> dashboardFilters = new List<EwavDataFilterCondition>();


            GadgetParameters = new GadgetParameters();

            GadgetParameters.DatasourceName = chartJSON["@DatasourceName"].ToString();
            GadgetParameters.InputVariableList = new Dictionary<string, string>();

            GadgetParameters.TableName = CommonClass.GetDatabaseObject(GadgetParameters.DatasourceName);

            Mapper Mapper = new Controllers.Mapper();

            GadgetParameters.MainVariableName = chartJSON["xAxisVariable"].ToString();
            GadgetParameters.CrosstabVariableName = chartJSON["yAxisVariable"].ToString();

            Rules = CommonClass.ReadRules(value);

            dashboardFilters = CommonClass.GetFilters(value);

            GadgetParameters.GadgetFilters = CommonClass.GetFilters(chartJSON, true);

            dt = new Google.DataTable.Net.Wrapper.DataTable();


            ScatterDataDTO ScatterDTO = ScatterDomainService.GenerateTable(
                GadgetParameters,
                dashboardFilters,
                Rules,
                CommonClass.AdvancedDataFilterString);

            //if (ScatterDTO.RegresResults.Variables != null)
            //{
            decimal coefficient = Convert.ToDecimal(ScatterDTO.RegresResults.Variables[0].Coefficient);
            decimal constant = Convert.ToDecimal(ScatterDTO.RegresResults.Variables[1].Coefficient);

            NumericDataValue newMaxValue = new NumericDataValue();
            newMaxValue.IndependentValue = ScatterDTO.MaxValue.IndependentValue + 1;
            newMaxValue.DependentValue = (coefficient * ScatterDTO.MaxValue.IndependentValue) + constant;
            NumericDataValue newMinValue = new NumericDataValue();
            newMinValue.IndependentValue = ScatterDTO.MinValue.IndependentValue - 1;
            newMinValue.DependentValue = (coefficient * ScatterDTO.MinValue.IndependentValue) + constant;


            //}

            dt.AddColumn(new Column(ColumnType.Number, "X", "X"));
            dt.AddColumn(new Column(ColumnType.Number, GadgetParameters.MainVariableName, GadgetParameters.MainVariableName));
            dt.AddColumn(new Column(ColumnType.Number, GadgetParameters.CrosstabVariableName, GadgetParameters.CrosstabVariableName));

            Row r = dt.NewRow();
            r.AddCellRange(new Cell[]
                        {
                            new Cell(newMinValue.IndependentValue),
                            new Cell(newMinValue.DependentValue),
                            new Cell(null)
                        }
            );

            dt.AddRow(r);
            r = dt.NewRow();
            r.AddCellRange(new Cell[]
                        {
                            new Cell(newMaxValue.IndependentValue),
                            new Cell(newMaxValue.DependentValue),
                            new Cell(null)
                        }
            );

            dt.AddRow(r);
            foreach (var item in ScatterDTO.DataValues)
            {
                r = dt.NewRow();
                r.AddCellRange(new Cell[]
                        {
                            new Cell(item.IndependentValue),
                            new Cell(null),
                            new Cell(item.DependentValue)
                        }
                );
                dt.AddRow(r);
            }



            var obj = new HttpResponseMessage()
            {
                Content = new StringContent(dt.GetJson())
            };


            obj.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return obj;

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