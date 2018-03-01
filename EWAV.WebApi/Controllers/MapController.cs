using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Ewav.Web.Services.MapCluster;
using Ewav.Web.EpiDashboard;
using Ewav.Web.Services;
using Newtonsoft.Json.Linq;
using Ewav.DTO;
using System.Net.Http.Headers;

namespace EWAV.WebApi.Controllers
{
    public class MapController : ApiController
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
            MapClusterDomainService MapClusterDomainService = new MapClusterDomainService();
            GadgetParameters GadgetParameters;
            ControllerCommon CommonClass = new ControllerCommon();
            List<EwavRule_Base> Rules = new List<EwavRule_Base>();

            JObject gadgetJSON = (JObject)value["gadget"];


            List<EwavDataFilterCondition> dashboardFilters = new List<EwavDataFilterCondition>();





            GadgetParameters = new GadgetParameters();

            GadgetParameters.ShouldIncludeFullSummaryStatistics = true;
            GadgetParameters.ShouldIncludeMissing = false;
            GadgetParameters.ShouldSortHighToLow = false;
            GadgetParameters.ShouldUseAllPossibleValues = false;

            GadgetParameters.DatasourceName = gadgetJSON["@DatasourceName"].ToString();
            //GadgetParameters.MainVariableName = gadgetJSON["mainVariable"].ToString();
            GadgetParameters.StrataVariableNames = new List<string>();
            //GadgetParameters.UseAdvancedDataFilter = CommonClass.AdvancedDataFilterString;

            GadgetParameters.TableName = CommonClass.GetDatabaseObject(GadgetParameters.DatasourceName);

            GadgetParameters.InputVariableList = new Dictionary<string, string>();
            GadgetParameters.InputVariableList.Add("lonx", gadgetJSON["longitude"].ToString());
            GadgetParameters.InputVariableList.Add(gadgetJSON["longitude"].ToString(), "listfield");
            GadgetParameters.InputVariableList.Add("laty", gadgetJSON["latitude"].ToString());
            GadgetParameters.InputVariableList.Add(gadgetJSON["latitude"].ToString(), "listfield");
            GadgetParameters.InputVariableList.Add(gadgetJSON["stratifyby"].ToString(), "listfield");


            if (gadgetJSON["stratifyby"].ToString().Trim().Length > 0)
            {
                List<MyString> StrataList = new List<MyString>();
                MyString strata = new MyString();
                strata.VarName = gadgetJSON["stratifyby"].ToString();
                StrataList.Add(strata);
                GadgetParameters.StrataVariableList = StrataList;
            }


            Rules = CommonClass.ReadRules(value);

            dashboardFilters = CommonClass.GetFilters(value);


            GadgetParameters.GadgetFilters = CommonClass.GetFilters(gadgetJSON, true);




            List<PointDTOCollection> PDTOList = MapClusterDomainService.GetMapValues(GadgetParameters, dashboardFilters, Rules, CommonClass.AdvancedDataFilterString);




            var obj = new HttpResponseMessage()
            {
                Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(PDTOList))
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