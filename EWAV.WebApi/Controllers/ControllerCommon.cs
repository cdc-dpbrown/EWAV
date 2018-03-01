using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ewav.Web.Services;
using Newtonsoft.Json.Linq;
using Ewav.Clients.Common.DefinedVariables;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using Ewav.BAL;

namespace EWAV.WebApi.Controllers
{
    public class ControllerCommon
    {

        public string AdvancedDataFilterString = "";

        EntityManager eManager;

        public ControllerCommon()
        {
            eManager = new EntityManager();
        }

        public List<EwavRule_Base> ReadRules(JObject value)
        {
            if (value["Rules"].ToString().Trim().Length != 0)
            {
                value = (JObject)value["Rules"];
            }
            else
            {
                return null;
            }

            RulesConverter RulesConverter = new Ewav.Clients.Common.DefinedVariables.RulesConverter();

            var jsonString = value.ToString(Newtonsoft.Json.Formatting.None);

            XmlDocument xmlDoc = (XmlDocument)JsonConvert.DeserializeXmlNode(jsonString, "Rules");

            XDocument xdoc;

            var nodeReader = new XmlNodeReader(xmlDoc);
            nodeReader.MoveToContent();

            xdoc = XDocument.Load(nodeReader);

            return RulesConverter.ConvertXMLToDefinedVariables(xdoc);
        }

        /// <summary>
        ///  Get the value of colName at the supplied row     
        /// </summary>
        /// <param name="datatableBag"></param>
        /// <param name="colName">The name of the column </param>
        /// <param name="row">The FieldsList which is a List<> of record values  </param>
        /// <returns></returns>
        public string GetValueAtRow(DatatableBag datatableBag, string colName, FieldsList row)
        {
            string thisColumn;

            int columnNum = 0;
            int fcol = 0;

            for (columnNum = 0; columnNum < datatableBag.ColumnNameList.Count; columnNum++)
            {
                thisColumn = datatableBag.ColumnNameList[columnNum].VarName;

                if (thisColumn.ToLower() == colName.ToLower())
                {
                    fcol = columnNum;
                    break;
                }
            }

            return row.Fields[fcol].VarName;
        }

        public List<EwavDataFilterCondition> GetFilters(JObject json, bool gadgetLevelFilters = false)
        {

            bool UseAdvancedFilter = false;

            if (gadgetLevelFilters)
            {
                json = CreateFilterJSON(json);


                if (json == null)
                {
                    return null;
                }
            }
            else
            {
                if (json["DataFilters"].ToString().Trim().Length != 0)
                {
                    json = (JObject)json["DataFilters"];
                }
                else
                {
                    return null;
                }
            }




            var jsonString = json.ToString(Newtonsoft.Json.Formatting.None);

            XmlDocument xmlDoc = (XmlDocument)JsonConvert.DeserializeXmlNode(jsonString, "DataFilters");

            XmlNodeList xmlNodeList = xmlDoc.ChildNodes;

            XmlNode xmlNode = xmlNodeList.Item(0);

            List<EwavDataFilterCondition> filterList = new List<EwavDataFilterCondition>();

            foreach (XmlNode item in xmlNode.ChildNodes)
            {
                if (item.Name == "EwavDataFilterCondition")
                {

                    EwavDataFilterCondition condition = new EwavDataFilterCondition();
                    condition.FieldName = new MyString();
                    condition.FieldName.VarName = item.Attributes["fieldName"].Value.ToString();

                    condition.FriendlyOperand = new MyString();
                    condition.FriendlyOperand.VarName = item.Attributes["friendlyOperand"].Value.ToString();

                    if (item.Attributes["friendlyValue"].Value.ToString() != "null")
                    {
                        condition.FriendlyValue = new MyString();
                        condition.FriendlyValue.VarName = item.Attributes["friendlyValue"].Value.ToString();
                    }

                    if (item.Attributes["friendLowValue"].Value.ToString() != "null")
                    {
                        condition.FriendLowValue = new MyString();
                        condition.FriendLowValue.VarName = item.Attributes["friendLowValue"].Value.ToString();
                    }

                    if (item.Attributes["friendHighValue"].Value.ToString() != "null")
                    {
                        condition.FriendHighValue = new MyString();
                        condition.FriendHighValue.VarName = item.Attributes["friendHighValue"].Value.ToString();
                    }

                    condition.JoinType = new MyString();
                    condition.JoinType.VarName = item.Attributes["joinType"].Value.ToString();

                    filterList.Add(condition);
                    //this.EwavDatafilters.Add(
                }
                if (item.Name == "EwavAdvancedFilterString")
                {
                    UseAdvancedFilter = true;
                    AdvancedDataFilterString = item.Value;

                    break;
                }
            }

            return filterList;

        }



        /// <summary>
        /// This method calculates Mean value for Pareto Line 
        /// </summary>
        /// <param name="max"></param>
        /// <param name="runningPercent"></param>
        /// <returns></returns>
        internal double CalculateMean(string value, double max, double runningPercent)
        {
            //if (this.chartTypeEnum == XYControlChartTypes.Pareto)
            //{
            //    max = max + fcd.FrequencyColumn.SafeParsetoDou();
            //}
            //(((fcd.FrequencyColumn.SafeParsetoDou() / max) * 100) + runningPercent).ToString();
            //data.currentMeanValue = (((data.DependentValue.SafeParsetoDou() / max) * 100) + runningPercent).ToString();
            //runningPercent = data.currentMeanValue.SafeParsetoDou();
            return ((SafeParsetoDou(value) / max) * 100) + runningPercent;
            //return 0;
        }

        public double SafeParsetoDou(string s)
        {
            double d;

            if (double.TryParse(s, out d))
            {
                return d;
            }
            else
            {
                return 0;
            }
        }

        private JObject CreateFilterJSON(JObject gadgetJSON)
        {
            JObject gadgetFilterParentJSON = null;


            JObject gadgetFilterJSON = null;
            JArray gadgetFiltersJSONArray;

            var gadgetFiltersJSONTest = gadgetJSON["EwavDataFilterCondition"];

            if (gadgetFiltersJSONTest is JObject)
            {
                gadgetFilterJSON = gadgetFiltersJSONTest as JObject;

                // Just one filter    
                string parent = @"{  " +
                         "EwavDataFilterCondition: [ "
                              + gadgetFilterJSON.ToString() +
                          " ]   }";

                gadgetFilterParentJSON = JObject.Parse(parent);

            }
            else if (gadgetFiltersJSONTest is JArray)
            {
                // Just one filter    
                gadgetFiltersJSONArray = gadgetFiltersJSONTest as JArray;

                string parent = "{  " +
                         "EwavDataFilterCondition: [ ";

                for (int i = 0; i < gadgetFiltersJSONArray.Count; i++)
                {
                    parent += "  " +
                               gadgetFiltersJSONArray[i].ToString() +
                           " ,   ";

                }

                parent += " ] }  ";


                gadgetFilterParentJSON = JObject.Parse(parent);


            }
            else if (gadgetFiltersJSONTest == null)
            {

            }
            else
            {

                throw new ApplicationException("Problem with JSON response ");

            }

            return gadgetFilterParentJSON;
        }

        /// <summary>
        /// Gets name of database object
        /// </summary>
        /// <param name="datasourceName"></param>
        /// <returns></returns>
        public string GetDatabaseObject(string datasourceName)
        {
            return eManager.GetDatabaseObject(datasourceName);

        }

    }
}