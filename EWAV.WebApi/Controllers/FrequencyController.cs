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
    //[EnableCors()]
    public class FrequencyController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id) { return "value"; }


        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]      JObject value)
        {
            FrequencyDomainService FrequencyDomainService = new FrequencyDomainService();
            GadgetParameters GadgetParameters;
            ControllerCommon CommonClass = new ControllerCommon();
            List<EwavRule_Base> Rules = new List<EwavRule_Base>();

            JObject gadgetJSON = (JObject)value["gadget"];


            List<EwavDataFilterCondition> dashboardFilters = new List<EwavDataFilterCondition>();





            GadgetParameters = new GadgetParameters();

            GadgetParameters.DatasourceName = gadgetJSON["@DatasourceName"].ToString();
            GadgetParameters.MainVariableName = gadgetJSON["mainVariable"].ToString();
            GadgetParameters.InputVariableList = new Dictionary<string, string>();
            if (gadgetJSON["sort"] != null && gadgetJSON["sort"].ToString() == "highlow")
            {
                GadgetParameters.InputVariableList.Add("sort", "highlow");
                GadgetParameters.ShouldSortHighToLow = true;
            }
            else
            {
                GadgetParameters.ShouldSortHighToLow = false;
            }
            GadgetParameters.InputVariableList.Add("tableName", gadgetJSON["@DatasourceName"].ToString());
            GadgetParameters.InputVariableList.Add("freqvar", gadgetJSON["mainVariable"].ToString());
            GadgetParameters.InputVariableList.Add("allvalues", gadgetJSON["allValues"].ToString());
            GadgetParameters.InputVariableList.Add("freqVar", gadgetJSON["mainVariable"].ToString());
            GadgetParameters.InputVariableList.Add("showconflimits", gadgetJSON["showConfLimits"].ToString());
            GadgetParameters.InputVariableList.Add("showcumulativepercent", gadgetJSON["showCumulativePercent"].ToString());
            GadgetParameters.InputVariableList.Add("includemissing", gadgetJSON["includeMissing"].ToString());
            GadgetParameters.InputVariableList.Add("maxrows", "200");
            GadgetParameters.TableName = CommonClass.GetDatabaseObject(GadgetParameters.DatasourceName);




            if (gadgetJSON["strataVariable"].ToString().Trim().Length > 0)
            {
                List<MyString> StrataList = new List<MyString>();
                MyString strata = new MyString();
                strata.VarName = gadgetJSON["strataVariable"].ToString();
                StrataList.Add(strata);
                GadgetParameters.StrataVariableList = StrataList;
            }


            Rules = CommonClass.ReadRules(value);

            dashboardFilters = CommonClass.GetFilters(value);


            GadgetParameters.GadgetFilters = CommonClass.GetFilters(gadgetJSON, true);




            List<FrequencyResultData> FrequencyResultData = FrequencyDomainService.GenerateFrequencyTable(GadgetParameters, dashboardFilters, Rules, CommonClass.AdvancedDataFilterString);

            foreach (var item in FrequencyResultData)
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


                }
            }

            FrequencyInfoAndMetaInfo AllInfo = new FrequencyInfoAndMetaInfo();
            AllInfo.FreqResult = FrequencyResultData;
            AllInfo.GadgetParameters = GadgetParameters;



            var obj = new HttpResponseMessage()
            {
                Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(AllInfo))//dt.GetJson()) 
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

    public class FrequencyInfoAndMetaInfo
    {
        public object FreqResult { get; set; }
        public GadgetParameters GadgetParameters { get; set; }
    }


}