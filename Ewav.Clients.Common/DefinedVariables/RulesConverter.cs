using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Ewav.Web.Services;
using Ewav.BAL;
using Ewav.Web.EpiDashboard.Rules;

namespace Ewav.Clients.Common.DefinedVariables
{
    public class RulesConverter
    {

        public List<EwavRule_Base> ConvertXMLToDefinedVariables(XDocument doc)
        {
            List<EwavRule_Base> EwavDefinedVariables = new List<EwavRule_Base>();

            foreach (var item in doc.Descendants("Rules").Descendants())
            {
                if (item.Name.ToString().ToLower() == "rule")
                {
                    try
                    {
                        //Type gadgetType = Type.GetType(item.Attribute("type").Value); // item.Attributes["gadgetType"].Value);
                        //EwavRule_Base rule = null;
                        EwavRule_Base baseRule = null;


                        switch (item.Attribute("type").Value.ToLower())
                        {
                            case "rule_format":
                                EwavRule_Format rule = new EwavRule_Format();
                                rule.FriendlyLabel = item.Element("friendlyRule").Value.ToString();
                                rule.CbxFieldName = item.Element("sourceColumnName").Value.ToString();
                                rule.TxtDestinationField = item.Element("destinationColumnName").Value.ToString();
                                rule.CbxFormatOptions = item.Element("formatString").Value.ToString();
                                rule.FormatTypes = (Ewav.Web.EpiDashboard.Rules.FormatTypes)Enum.Parse(typeof(Ewav.Web.EpiDashboard.Rules.FormatTypes),
                                    item.Element("formatType").Value.ToString(), true);
                                rule.VaraiableDataType = item.Element("variableDataType").Value.ToString();

                                baseRule = rule;
                                baseRule.VaraiableName = rule.TxtDestinationField;

                                break;
                            case "rule_expressionassign":
                                EwavRule_ExpressionAssign ruleAssign = new EwavRule_ExpressionAssign();
                                ruleAssign.FriendlyRule = item.Element("friendlyRule").Value.ToString();
                                ruleAssign.Expression = item.Element("expression").Value.ToString();
                                ruleAssign.DestinationColumnName = item.Element("destinationColumnName").Value.ToString();
                                ruleAssign.DataType = item.Element("destinationColumnType").Value.ToString();
                                ruleAssign.VaraiableDataType = item.Element("variableDataType").Value.ToString();

                                baseRule = ruleAssign;
                                baseRule.VaraiableName = ruleAssign.DestinationColumnName;

                                break;
                            case "rule_groupvariable":
                                EwavRule_GroupVariable ruleGroupVar = new EwavRule_GroupVariable();
                                ruleGroupVar.FriendlyLabel = item.Element("friendlyLabel").Value.ToString();
                                ruleGroupVar.VaraiableName = item.Element("destinationColumnName").Value.ToString();
                                ruleGroupVar.VaraiableDataType = item.Element("variableDataType").Value.ToString();
                                List<MyString> columnList = new List<MyString>();
                                foreach (var column in item.Descendants("column"))
                                {
                                    MyString colVal = new MyString();
                                    colVal.VarName = column.Value.ToString();
                                    columnList.Add(colVal);
                                }
                                ruleGroupVar.Items = columnList;

                                baseRule = ruleGroupVar;
                                baseRule.VaraiableName = ruleGroupVar.VaraiableName;

                                break;
                            case "rule_conditionalassign":
                                EwavRule_ConditionalAssign ruleCondAssign = new EwavRule_ConditionalAssign();
                                MyString myString = new MyString();
                                myString.VarName = item.Element("friendlyRule").Value.ToString();
                                ruleCondAssign.FriendlyRule = myString;
                                ruleCondAssign.TxtDestination = item.Element("destinationColumnName").Value.ToString();
                                ruleCondAssign.DestinationColumnType = item.Element("destinationColumnType").Value.ToString();
                                ruleCondAssign.AssignValue = item.Element("assignValue").Value.ToString();
                                ruleCondAssign.ElseValue = item.Element("elseValue").Value.ToString();
                                ruleCondAssign.VaraiableDataType = item.Element("variableDataType").Value.ToString();
                                if (item.Element("cbxFieldType") != null)
                                {
                                    ruleCondAssign.CbxFieldType = (cbxFieldTypeEnum)Enum.Parse(typeof(cbxFieldTypeEnum), item.Element("cbxFieldType").Value.ToString(), false);
                                }

                                ruleCondAssign.ConditionsList = new List<EwavDataFilterCondition>();
                                //ruleCondAssign.ConditionsList = 

                                foreach (var condition in item.Descendants("EwavDataFilterCondition").OrderBy(x => (int)x.Attribute("order")))
                                {
                                    EwavDataFilterCondition df = new EwavDataFilterCondition();
                                    if (condition.Attribute("friendlyOperand") != null)
                                    {
                                        df.FriendlyOperand = ToMyString(condition.Attribute("friendlyOperand").Value);
                                    }

                                    if (condition.Attribute("friendlyValue") != null)
                                    {
                                        df.FriendlyValue = ToMyString(condition.Attribute("friendlyValue").Value);
                                    }

                                    if (condition.Attribute("fieldName") != null)
                                    {
                                        df.FieldName = ToMyString(condition.Attribute("fieldName").Value);
                                    }

                                    if (condition.Attribute("joinType") != null)
                                    {
                                        df.JoinType = ToMyString(condition.Attribute("joinType").Value);
                                    }

                                    if (condition.Attribute("friendLowValue") != null &&
                                        condition.Attribute("friendLowValue").Value != "null")
                                    {
                                        df.FriendLowValue = ToMyString(condition.Attribute("friendLowValue").Value);
                                    }

                                    if (condition.Attribute("friendHighValue") != null &&
                                        condition.Attribute("friendHighValue").Value != "null")
                                    {
                                        df.FriendHighValue = ToMyString(condition.Attribute("friendHighValue").Value);
                                    }

                                    ruleCondAssign.ConditionsList.Add(df);
                                }

                                baseRule = ruleCondAssign;
                                baseRule.VaraiableName = ruleCondAssign.TxtDestination;


                                break;
                            case "rule_simpleassign":
                                EwavRule_SimpleAssignment ruleSimple = new EwavRule_SimpleAssignment();
                                ruleSimple.FriendlyLabel = item.Element("friendlyRule").Value.ToString();
                                ruleSimple.AssignmentType = (Ewav.Web.EpiDashboard.Rules.SimpleAssignType)Enum.Parse(typeof(Ewav.Web.EpiDashboard.Rules.SimpleAssignType),
                                    item.Element("assignmentType").Value.ToString(),
                                    true);
                                ruleSimple.TxtDestinationField = item.Element("destinationColumnName").Value.ToString();
                                ruleSimple.Parameters = new List<MyString>();
                                ruleSimple.VaraiableDataType = item.Element("variableDataType").Value.ToString();

                                foreach (var item1 in item.Element("parametersList").Descendants())
                                {
                                    MyString mys = new MyString();
                                    mys.VarName = item1.Value;
                                    ruleSimple.Parameters.Add(mys);
                                }
                                baseRule = ruleSimple;
                                baseRule.VaraiableName = ruleSimple.TxtDestinationField;


                                break;
                            case "rule_recode":
                                EwavRule_Recode ruleRecode = new EwavRule_Recode();
                                ruleRecode.Friendlyrule = item.Element("friendlyRule").Value.ToString();
                                ruleRecode.SourceColumnName = item.Element("sourceColumnName").Value.ToString();
                                ruleRecode.SourceColumnType = item.Element("sourceColumnType").Value.ToString();
                                ruleRecode.TxtDestinationField = item.Element("destinationColumnName").Value.ToString();
                                ruleRecode.DestinationFieldType = (DashboardVariableType)Enum.Parse(typeof(DashboardVariableType), item.Element("destinationColumnType").Value.ToString(), true);
                                ruleRecode.TxtElseValue = item.Element("elseValue").Value.ToString();
                                ruleRecode.CheckboxUseWildcardsIndicator = bool.Parse(item.Element("shouldUseWildcards").Value.ToString());
                                ruleRecode.CheckboxMaintainSortOrderIndicator = bool.Parse(item.Element("shouldMaintainSortOrder").Value.ToString());
                                ruleRecode.VaraiableName = item.Element("destinationColumnName").Value.ToString();
                                ruleRecode.VaraiableDataType = item.Element("variableDataType").Value.ToString();
                                //ruleRecode.VaraiableDataType = item.Element("variableDataType").Value.ToString();
                                List<EwavRuleRecodeDataRow> rows = new List<EwavRuleRecodeDataRow>();

                                foreach (var item2 in item.Descendants("recodeTable"))
                                {
                                    var itemmm = item2;
                                    foreach (var item3 in item2.Elements("recodeTableRow"))
                                    {
                                        EwavRuleRecodeDataRow row = new EwavRuleRecodeDataRow();
                                        IEnumerable<XElement> enumerableList = item3.Elements("recodeTableData");

                                        List<XElement> list = enumerableList.ToList();

                                        if (list.Count == 2)
                                        {
                                            row.col1 = list[0].Value.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                                            row.col3 = list[1].Value.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                                        }
                                        else
                                        {
                                            row.col1 = list[0].Value.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                                            row.col2 = list[1].Value.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                                            row.col3 = list[2].Value.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                                        }
                                        rows.Add(row);
                                    }
                                }

                                ruleRecode.RecodeTable = rows;

                                baseRule = ruleRecode;
                                baseRule.VaraiableName = ruleRecode.TxtDestinationField;
                                break;
                            default:
                                throw new Exception("This Rule doesn't exists.");
                        }
                        //newColumn.SqlDataTypeAsString = (ColumnDataType)Enum.Parse(typeof(ColumnDataType), item.Element("variableDataType").Value.ToString(), false);
                        EwavDefinedVariables.Add(baseRule);
                        
                    }
                    catch (Exception ex)
                    {
                        //Epi.Windows.MsgBox.ShowError(DashboardSharedStrings.GADGET_LOAD_ERROR);
                        throw new Exception("Exception occured deserializing Rules." + ex.Message);
                        //return;
                    }

                }
            }
            return EwavDefinedVariables;
        }

        public MyString ToMyString(string str)
        {
            MyString mystr = new MyString();
            mystr.VarName = str;
            return mystr;
        }
    }
}
