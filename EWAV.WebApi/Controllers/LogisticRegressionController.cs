using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Ewav.Web.Services;
using Ewav.Web.EpiDashboard;
using Ewav.DTO;
using Google.DataTable.Net.Wrapper;
using System.Net.Http.Headers;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace EWAV.WebApi.Controllers
{
    public class LogisticRegressionController : ApiController
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
            LogisticRegressionDomainService LogisticRegressionDomainService = new LogisticRegressionDomainService();
            GadgetParameters GadgetParameters;
            ControllerCommon CommonClass = new ControllerCommon();
            List<EwavRule_Base> Rules = new List<EwavRule_Base>();

            JObject gadgetJSON = (JObject)value["gadget"];

            List<EwavDataFilterCondition> dashboardFilters = new List<EwavDataFilterCondition>();

            GadgetParameters = new GadgetParameters();

            GadgetParameters.DatasourceName = gadgetJSON["@DatasourceName"].ToString();
            GadgetParameters.InputVariableList = new Dictionary<string, string>();
            GadgetParameters.TableName = CommonClass.GetDatabaseObject(GadgetParameters.DatasourceName);

            Mapper Mapper = new Controllers.Mapper();


            Rules = CommonClass.ReadRules(value);

            dashboardFilters = CommonClass.GetFilters(value);


            GadgetParameters.GadgetFilters = CommonClass.GetFilters(gadgetJSON, true);


            List<string> columnNames = new List<string>();
            Dictionary<string, string> inputVariableList = Mapper.MapJSONToRegressionInputVariableList(gadgetJSON);

            foreach (KeyValuePair<string, string> kvp in inputVariableList)
            {
                if (kvp.Value.ToLower().Equals("unsorted") || kvp.Value.ToLower().Equals("dependvar") || kvp.Value.ToLower().Equals("weightvar") || kvp.Value.ToLower().Equals("matchvar"))
                {
                    columnNames.Add(kvp.Key);
                }
                else if (kvp.Value.ToLower().Equals("discrete"))
                {
                    columnNames.Add(kvp.Key);
                }
            }
            List<DictionaryDTO> inputDtoList = new List<DictionaryDTO>();


            inputDtoList = Mapper.MapDictToList(inputVariableList);

            LogRegressionResults LogRegressionResults = LogisticRegressionDomainService.GetRegressionResult(
                GadgetParameters,
            columnNames,
            inputDtoList,
                dashboardFilters,
                Rules,
                CommonClass.AdvancedDataFilterString);

            LogRegressionResults = Mapper.FormatLogRegressionResults(LogRegressionResults);





            var obj = new HttpResponseMessage()
            {
                Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(LogRegressionResults))//dt.GetJson()) 
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