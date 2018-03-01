/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       Extensions.cs
 *  Namespace:  Ewav.ExtensionMethods    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Linq;
using Ewav.BAL;
using Ewav.Web.Services;
using System.Text;

namespace Ewav.ExtensionMethods
{
    public static class Extensions
    {

        public static string To64(this string thisStr)
        {


            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.Unicode.GetBytes(thisStr);
            string string64 = Convert.ToBase64String(toEncodeAsBytes);

            return string64;

        }

        public static string From64(this string thisStr)
        {


            byte[] encodedBytes = Convert.FromBase64String(thisStr);    

            string  ascii   = ASCIIEncoding.Unicode.GetString(encodedBytes);

            return ascii;    

        }



        public static void AddField(this DatatableBag datatableBag, string fieldName)
        {
            // Add a new column         
            MyString newColName = new MyString();
            newColName.VarName = fieldName;
            //  datatableBag.ColumnNameList.Add(newColName);  
            datatableBag.ColumnNameList.Insert(int.Parse(fieldName), newColName);

            // Add a col value to each record   
            foreach (FieldsList row in datatableBag.RecordList)
            {
                //  FieldsList newCol = new FieldsList();
                MyString newColValue = new MyString();
                newColValue.VarName = "0";
                row.Fields.Add(newColValue);
                row.Fields.Insert(int.Parse(fieldName), newColValue);
            }
        }

        public static void ClearValidationError(this FrameworkElement frameworkElement)
        {
            BindingExpression b = frameworkElement.GetBindingExpression(Control.TagProperty);

            if (b != null)
            {
                ((CustomValidation)b.DataItem).ShowErrorMessage = false;
                b.UpdateSource();
            }
        }

        public static bool DatatableBagHasColumnName(this DatatableBag datatableBag, string columnName)
        {
            bool f;
            int columnNum = 0;
            for (columnNum = 0; columnNum < datatableBag.ColumnNameList.Count; columnNum++)
            {
                string thisColumn = datatableBag.ColumnNameList[columnNum].VarName;

                if (thisColumn == columnName)
                {
                    f = true;
                }
            }

            f = false;

            return f;
        }


        public static string FromCamelCase(this string pascalCaseString)
        {
            Regex r = new Regex("(?<=[a-z])(?<x>[A-Z])|(?<=.)(?<x>[A-Z])(?=[a-z]) ");
            return r.Replace(pascalCaseString, " ${x}");
        }

        public static List<string> GetColumnNamesAsList(this DatatableBag datatableBag)
        {
            List<string> list = new List<string>();
            foreach (MyString s in datatableBag.ColumnNameList)
            {
                list.Add(s.VarName);
            }
            return list;
        }

        //public static bool HasAllFlags(this   Ewav.Organization.UserActionEnum op, params  Ewav.Organization.UserActionEnum[] checkflags)
        //{
        //    foreach (Ewav.Organization.UserActionEnum checkflag in checkflags)
        //    {
        //        if ((op & checkflag) != checkflag)
        //            return false;
        //    }
        //    return true;
        //}
        public static string GetSafeVarName(this MyString myString)
        {
            if (myString == null)
            {
                return "null";
            }
            else
            {
                return myString.VarName;
            }
        }

        //   public static string GetValue(this      List<MyString> columnNameList, List<FieldsList> recordList)
        public static string GetValue(this DatatableBag datatableBag, string colName)
        {
            string thisColumn;

            int columnNum = 0;

            for (columnNum = 0; columnNum < datatableBag.ColumnNameList.Count; columnNum++)
            {
                thisColumn = datatableBag.ColumnNameList[columnNum].VarName;

                if (thisColumn == colName)
                {
                    break;
                }
            }

            return datatableBag.RecordList[0].Fields[columnNum].VarName;                  //     recordListArray[0 ].Field.    
        }

        /// <summary>
        ///  Get the value of colName at the supplied row     
        /// </summary>
        /// <param name="datatableBag"></param>
        /// <param name="colName">The name of the column </param>
        /// <param name="row">The FieldsList which is a List<> of record values  </param>
        /// <returns></returns>
        public static string GetValueAtRow(this DatatableBag datatableBag, string colName, FieldsList row)
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



        /*********************** Validation Code starts***********************************/

        public static void SetValidation(this FrameworkElement frameworkElement, string message)
        {
            CustomValidation customValidation = new CustomValidation(message);

            Binding binding = new Binding("ValidationError")
            {
                Mode = System.Windows.Data.BindingMode.TwoWay,
                NotifyOnValidationError = true,
                ValidatesOnExceptions = true,
                Source = customValidation
            };
            frameworkElement.SetBinding(Control.TagProperty, binding);
        }

        public static bool IsEmailValid(this string inputEmail)
        {
            bool isEmailValid = true;
            string emailExpression = @"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$";
            Regex re = new Regex(emailExpression);
            if (!re.IsMatch(inputEmail))
            {
                isEmailValid = false;
            }
            return isEmailValid;
        }

        public static bool IsPhoneNumberValid(this string inputNumber)
        {
            bool isNumberValid = true;
            string phoneExpression = @"^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$";
            Regex re = new Regex(phoneExpression);
            if (!re.IsMatch(inputNumber))
            {
                isNumberValid = false;
            }
            return isNumberValid;
        }

        public static bool IsAlphaNumericValid(this string inputText)
        {
            bool isTextValid = true;

            foreach (char character in inputText)
            {
                if (char.IsWhiteSpace(character) == false)
                {
                    if (char.IsLetterOrDigit(character) == false)
                    {
                        if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                        {
                            isTextValid = false;
                            break;
                        }
                    }
                }
            }
            return isTextValid;
        }

        public static bool IsTextValid(this string inputText)
        {
            bool isTextValid = true;

            foreach (char character in inputText)
            {
                if (char.IsWhiteSpace(character) == false)
                {
                    if (char.IsLetter(character) == false)
                    {
                        if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                        {
                            isTextValid = false;
                            break;
                        }
                    }
                }
            }
            return isTextValid;
        }


        /*********************** Validtion Code Ends  ************************************/


        public static void MakeNoCamelColumn(this EwavColumn ewc)
        {
            ewc.NoCamelName = ewc.Name.FromCamelCase();
        }

        public static void RaiseValidationError(this FrameworkElement frameworkElement)
        {
            BindingExpression b = frameworkElement.GetBindingExpression(Control.TagProperty);

            if (b != null)
            {
                ((CustomValidation)b.DataItem).ShowErrorMessage = true;
                b.UpdateSource();
            }
        }

        /// <summary>
        ///  "Refreshes"  a DataGreid    
        /// </summary>
        /// <param name="dg"></param>
        /// <param name="itemSource"></param>
        // public static void Refresh(this DataGrid dg, IEnumerable<object> itemSource)
        public static void Refresh(this DataGrid dg, object itemSource)
        {
            if (itemSource is IEnumerable<object>)
            {
                dg.ItemsSource = null;
                dg.ItemsSource = itemSource as IEnumerable<object>;
            }
            else
            {
                // this will cause the normal exception that happens when you try to assign something other than 
                // IEnumerable<> to the itemsource property of a datagrid    
                throw new Exception("Must assign a type that implements IEnumerable<> to a datagrid itemsource ");

            }
        }



        public static double SafeParsetoDou(this string s)
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

        /// <summary>
        /// Serializes the rules
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static XElement Serialize(this EwavRule_Base rule)
        {
            XElement element = null;
            if (rule is EwavRule_Format)
            {
                EwavRule_Format ruleFormat = (EwavRule_Format)rule;
                element = new XElement("rule", new XAttribute("type", "Rule_Format"),
                    new XElement("friendlyRule", ruleFormat.FriendlyLabel),
                    new XElement("sourceColumnName", ruleFormat.CbxFieldName),
                    new XElement("destinationColumnName", ruleFormat.TxtDestinationField),
                    new XElement("formatString", ruleFormat.CbxFormatOptions),
                    new XElement("formatType", ruleFormat.FormatTypes),
                    new XElement("variableDataType", ruleFormat.VaraiableDataType));
            }
            else if (rule is EwavRule_ExpressionAssign)
            {
                EwavRule_ExpressionAssign ruleAssign = (EwavRule_ExpressionAssign)rule;
                element = new XElement("rule", new XAttribute("type", "Rule_ExpressionAssign"),
                    new XElement("friendlyRule", ruleAssign.FriendlyRule),
                    new XElement("expression", ruleAssign.Expression),
                    new XElement("destinationColumnName", ruleAssign.DestinationColumnName),
                    new XElement("destinationColumnType", ruleAssign.DataType),
                    new XElement("variableDataType", ruleAssign.VaraiableDataType));
            }
            else if (rule is EwavRule_ConditionalAssign)
            {
                EwavRule_ConditionalAssign ruleCondAssign = (EwavRule_ConditionalAssign)rule;

                element = new XElement("rule", new XAttribute("type", "Rule_ConditionalAssign"),
                    new XElement("friendlyRule", ruleCondAssign.FriendlyRule.VarName),
                    new XElement("destinationColumnName", ruleCondAssign.TxtDestination),
                    new XElement("destinationColumnType", ruleCondAssign.DestinationColumnType),
                    new XElement("assignValue", ruleCondAssign.AssignValue),
                    new XElement("elseValue", ruleCondAssign.ElseValue),
                    new XElement("cbxFieldType", ruleCondAssign.CbxFieldType),
                    new XElement("variableDataType", ruleCondAssign.VaraiableDataType));

                ruleCondAssign.ConditionsList.Serialize(element);
            }
            else if (rule is EwavRule_GroupVariable)
            {
                EwavRule_GroupVariable ruleGroupVar = (EwavRule_GroupVariable)rule;

                element = new XElement("rule", new XAttribute("type", "Rule_GroupVariable"),
                    new XElement("friendlyLabel", ruleGroupVar.FriendlyLabel),
                    new XElement("destinationColumnName", ruleGroupVar.VaraiableName),
                    //new XElement("destinationColumnType", ruleGroupVar.VaraiableDataType),
                    new XElement("variableDataType", ruleGroupVar.VaraiableDataType));
                XElement items = new XElement("Columns");

                for (int i = 0; i < ruleGroupVar.Items.Count; i++)
                {
                    items.Add(new XElement("column", ruleGroupVar.Items[i].VarName));
                }
                element.Add(items);
            }
            else if (rule is EwavRule_Recode)
            {
                EwavRule_Recode ruleRecode = (EwavRule_Recode)rule;

                element = new XElement("rule", new XAttribute("type", "Rule_Recode"),
                    new XElement("friendlyRule", ruleRecode.Friendlyrule),
                    new XElement("sourceColumnName", ruleRecode.SourceColumnName),
                    new XElement("sourceColumnType", ruleRecode.SourceColumnType),
                    new XElement("destinationColumnName", ruleRecode.TxtDestinationField),
                    new XElement("destinationColumnType", ruleRecode.DestinationFieldType),
                    //new XElement("tableColumns",(List<EwavRuleRecodeDataRow>)dataGridViewRecode.ItemsSource.Count.ToString() ),
                    new XElement("elseValue", ruleRecode.TxtElseValue),
                    new XElement("shouldUseWildcards", ruleRecode.CheckboxUseWildcardsIndicator),
                    new XElement("shouldMaintainSortOrder", ruleRecode.CheckboxMaintainSortOrderIndicator),
                    new XElement("variableDataType", ruleRecode.VaraiableDataType));

                List<EwavRuleRecodeDataRow> dtb = ruleRecode.RecodeTable;

                XElement recodeTableElement = new XElement("recodeTable");
                XElement recodeTableRowElement = null;
                for (int i = 0; i < dtb.Count; i++)
                {
                    if (dtb[i].col1.Length > 0 && dtb[i].col1.Length > 0 && dtb[i].col1.Length > 0)
                    {
                        recodeTableRowElement = new XElement("recodeTableRow");
                        recodeTableRowElement.Add(new XElement("recodeTableData", dtb[i].col1.Replace("<", "&lt;").Replace(">", "&gt;")));
                        if (dtb[i].col2 == null)
                        {
                            recodeTableRowElement.Add(new XElement("recodeTableData", null));
                        }
                        else if (dtb[i].col2.Length > 0)
                        {
                            recodeTableRowElement.Add(new XElement("recodeTableData", dtb[i].col2.Replace("<", "&lt;").Replace(">", "&gt;")));
                        }
                        recodeTableRowElement.Add(new XElement("recodeTableData", dtb[i].col3.Replace("<", "&lt;").Replace(">", "&gt;")));
                        recodeTableElement.Add(recodeTableRowElement);
                    }
                }

                element.Add(recodeTableElement);
            }
            else if (rule is EwavRule_SimpleAssignment)
            {
                EwavRule_SimpleAssignment ruleSimple = (EwavRule_SimpleAssignment)rule;

                element = new XElement("rule", new XAttribute("type", "Rule_SimpleAssign"),
                    new XElement("friendlyRule", ruleSimple.FriendlyLabel),
                    new XElement("assignmentType", ruleSimple.AssignmentType),
                    new XElement("destinationColumnName", ruleSimple.TxtDestinationField),
                    new XElement("variableDataType", ruleSimple.VaraiableDataType));

                XElement paramsList = new XElement("parametersList");

                for (int i = 0; i < ruleSimple.Parameters.Count; i++)
                {
                    paramsList.Add(new XElement("parameter", ruleSimple.Parameters[i].VarName));
                }
                element.Add(paramsList);
            }
            return element;
        }

        public static void Serialize(this List<EwavDataFilterCondition> ewavDataFilterConditions, XElement element, string advancedFilterString = "")
        {
            int orderCount = 0;

            XElement elementFilter = null;

            if (advancedFilterString != null && advancedFilterString.Length != 0)
            {
                elementFilter = new XElement("EwavAdvancedFilterString", advancedFilterString);
                element.Add(elementFilter);
            }

            foreach (EwavDataFilterCondition ewavDataFilterCondition in ewavDataFilterConditions)
            {
                elementFilter = new XElement("EwavDataFilterCondition",
                    new XAttribute("friendlyOperand", ewavDataFilterCondition.FriendlyOperand.GetSafeVarName()),
                    new XAttribute("friendlyValue", ewavDataFilterCondition.FriendlyValue.GetSafeVarName()),
                    new XAttribute("fieldName", ewavDataFilterCondition.FieldName.GetSafeVarName()),
                    new XAttribute("joinType", ewavDataFilterCondition.JoinType.GetSafeVarName()),
                    new XAttribute("friendLowValue", ewavDataFilterCondition.FriendLowValue.GetSafeVarName()),
                    new XAttribute("friendHighValue", ewavDataFilterCondition.FriendHighValue.GetSafeVarName()),
                    new XAttribute("order", orderCount));

                //thisElement.SetAttributeValue("order", orderCount);
                orderCount++;
                //allDataFilterXmlElements.Add(thisElement);
                element.Add(elementFilter);
            }
            //return element;
        }


        public static MyString ToMyString(this string str)
        {
            MyString mystr = new MyString();
            mystr.VarName = str;
            return mystr;
        }

        public static int WordCount(this String str)
        {
            return str.Split(new char[] { ' ', '.', '?' },
                StringSplitOptions.RemoveEmptyEntries).Length;
        }


        public static EwavColumn Copy(this EwavColumn col)
        {
            return new EwavColumn()
            {
                ChildVariableName = col.ChildVariableName,
                Index = col.Index,
                Name = col.Name,
                NoCamelName = col.NoCamelName,
                SqlDataTypeAsString = col.SqlDataTypeAsString,
                IsInUse = col.IsInUse,
                IsUserDefined = col.IsUserDefined
            };
        }
        //  public static  string (this )     
    }
}