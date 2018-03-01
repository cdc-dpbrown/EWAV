/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       Rule_SimpleAssign.cs
 *  Namespace:  EpiDashboard.Rules    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using Epi;
using Epi.Core;
using Epi.Fields;

namespace Ewav.Web.EpiDashboard.Rules
{   

    /// <summary>
    /// A class designed to assign data to another variable using a simple dashboard function
    /// </summary>
    public class Rule_SimpleAssign : DataAssignmentRule
    {
        #region Private Members
        private SimpleAssignType assignmentType;
        private List<string> assignmentParameters;
        #endregion // Private Members

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public Rule_SimpleAssign()
        {
            this.destinationColumnType = "System.Decimal";
            this.assignmentParameters = new List<string>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Rule_SimpleAssign(DashboardHelper dashboardHelper)
        {
            this.dashboardHelper = dashboardHelper;
            this.destinationColumnType = "System.Decimal";
            this.assignmentParameters = new List<string>();            
        }

        /// <summary>
        /// Constructor for simple assignment
        /// </summary>
        public     Rule_SimpleAssign(DashboardHelper dashboardHelper, string friendlyRule, string destinationColumnName, SimpleAssignType assignmentType, List<string> assignmentParameters)
        {
            this.friendlyRule = friendlyRule;
            this.destinationColumnName = destinationColumnName;            
            this.DashboardHelper = dashboardHelper;
            this.variableType = DashboardVariableType.Numeric;
            this.assignmentType = assignmentType;
            this.destinationColumnType = GetDestinationColumnType(this.assignmentType);
            this.assignmentParameters = assignmentParameters;
        }
        #endregion // Constructors

        #region Public Methods

        public override void SetupRule(DataTable table)
        {
            string destinationColumnType = this.DestinationColumnType;
            DataColumn dc = new DataColumn(destinationColumnName);

            switch (destinationColumnType)
            {
                case "System.Byte":
                    dc = new DataColumn(this.DestinationColumnName, typeof(byte));
                    break;
                case "System.Single":
                case "System.Double":
                    dc = new DataColumn(this.DestinationColumnName, typeof(double));
                    break;
                case "System.Decimal":
                    dc = new DataColumn(this.DestinationColumnName, typeof(decimal));
                    break;
                case "System.DateTime":
                    dc = new DataColumn(this.DestinationColumnName, typeof(DateTime));
                    break;
                case "System.String":
                default:
                    dc = new DataColumn(this.DestinationColumnName, typeof(string));
                    break;
            }
            
            //dc.Expression = this.expression;

            //if (table.Columns.Contains(dc.ColumnName))
            //{
            //    table.Columns.Remove(dc.ColumnName);
            //}

            if (!table.Columns.Contains(dc.ColumnName))
            {
                try
                {
                    table.Columns.Add(dc);
                }
                catch (ArgumentException)
                {
                    dc = new DataColumn(DestinationColumnName);
                    table.Columns.Add(dc);
                }
            }
            else
            {
                foreach (DataRow row in table.Rows)
                {
                    row[dc.ColumnName] = DBNull.Value;
                }
            }
        }
        #endregion //Public Methods

        #region Public Properties
        /// <summary>
        /// Gets the rule's expression
        /// </summary>
        public SimpleAssignType AssignmentType
        {
            get
            {
                return this.assignmentType;
            }
        }

        /// <summary>
        /// Gets the rule's assignment parameters
        /// </summary>
        public List<string> AssignmentParameters
        {
            get
            {
                return this.assignmentParameters;
            }
        }
        #endregion // Public Properties

        #region Public Methods
        /// <summary>
        /// Converts the value of the current EpiDashboard.Rule_ExpressionAssign object to its equivalent string representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.FriendlyRule;
        }
        #endregion // Public Methods

        #region Protected Methods
        /// <summary>
        /// Gets the appropriate column type to create given the type of assignment being carried out
        /// </summary>
        /// <param name="sAssignmentType">The simple assign type</param>
        /// <returns>A string representing the type of a .NET DataColumn</returns>
        protected string GetDestinationColumnType(SimpleAssignType sAssignmentType)
        {
            switch (sAssignmentType)
            {
                case SimpleAssignType.DaysElapsed:
                case SimpleAssignType.HoursElapsed:
                case SimpleAssignType.MinutesElapsed:
                case SimpleAssignType.MonthsElapsed:
                case SimpleAssignType.YearsElapsed:
                case SimpleAssignType.TextToNumber:
                case SimpleAssignType.StringLength:
                case SimpleAssignType.FindText:
                case SimpleAssignType.Round:
                    return "System.Decimal";
                case SimpleAssignType.Substring:
                    return "System.String";
                case SimpleAssignType.TextToDate:
                    return "System.DateTime";
            }

            return "System.Decimal";
        }

        /// <summary>
        /// Gets a column type appropriate for a .NET data table based off of the dashboard variable type selected by the user
        /// </summary>
        /// <param name="dashboardVariableType">The type of variable that is storing the recoded values</param>
        /// <returns>A string representing the type of a .NET DataColumn</returns>
        protected string GetDestinationColumnType(DashboardVariableType dashboardVariableType)
        {
            switch (dashboardVariableType)
            {
                case DashboardVariableType.Numeric:
                    return "System.Single";
                case DashboardVariableType.Text:
                    return "System.String";
                case DashboardVariableType.YesNo:
                    return "System.Byte";
                case DashboardVariableType.Date:
                    return "System.DateTime";
                case DashboardVariableType.None:
                    throw new ApplicationException(SharedStrings.DASHBOARD_ERROR_INVALID_COLUMN_TYPE);
                default:
                    return "System.String";
            }
        }
        #endregion // Protected Methods

        #region IDashboardRule Members
        /// <summary>
        /// Generates an Xml element for this rule
        /// </summary>
        /// <param name="doc">The parent Xml document</param>
        /// <returns>XmlNode representing this rule</returns>
        public override System.Xml.XmlNode Serialize(System.Xml.XmlDocument doc)
        {
            string xmlString =
            "<friendlyRule>" + friendlyRule + "</friendlyRule>" +
            "<assignmentType>" + ((int)assignmentType).ToString() + "</assignmentType>" +
            "<destinationColumnName>" + destinationColumnName + "</destinationColumnName>" +
            "<destinationColumnType>" + destinationColumnType + "</destinationColumnType>";

            xmlString = xmlString + "<parameterList>";
            foreach (string parameter in this.AssignmentParameters)
            {
                xmlString = xmlString + "<parameter>" + parameter + "</parameter>";
            }
            xmlString = xmlString + "</parameterList>";

            System.Xml.XmlElement element = doc.CreateElement("rule");
            element.InnerXml = xmlString;

            System.Xml.XmlAttribute order = doc.CreateAttribute("order");
            System.Xml.XmlAttribute type = doc.CreateAttribute("ruleType");
            
            type.Value = "EpiDashboard.Rules.Rule_SimpleAssign";
            
            element.Attributes.Append(type);

            return element;
        }

        /// <summary>
        /// Creates the rule from an Xml element
        /// </summary>
        /// <param name="element">The XmlElement from which to create the rule</param>
        public override void CreateFromXml(System.Xml.XmlElement element)
        {
            foreach (XmlElement child in element.ChildNodes)
            {
                if (child.Name.Equals("friendlyRule"))
                {
                    this.friendlyRule = child.InnerText;
                }
                else if (child.Name.Equals("assignmentType"))
                {                    
                    this.assignmentType = ((SimpleAssignType)Int32.Parse(child.InnerText));
                }
                else if (child.Name.Equals("destinationColumnName"))
                {
                    this.destinationColumnName = child.InnerText;
                }
                else if (child.Name.Equals("destinationColumnType"))
                {
                    this.destinationColumnType = child.InnerText;
                }
                else if (child.Name.Equals("parameterList"))
                {
                    foreach (XmlElement parameter in child.ChildNodes)
                    {
                        if (parameter.Name.ToLower().Equals("parameter"))
                        {
                            AssignmentParameters.Add(parameter.InnerText);                            
                        }
                    }
                }
            }

            if (destinationColumnType.Equals("System.String"))
            {
                this.variableType = DashboardVariableType.Text;
            }
            else if(destinationColumnType.Equals("System.Single") || destinationColumnType.Equals("System.Double") || destinationColumnType.Equals("System.Decimal") || destinationColumnType.Equals("System.Int16") || destinationColumnType.Equals("System.Int32"))
            {
                this.variableType = DashboardVariableType.Numeric;
            }
        }

        /// <summary>
        /// Applies the rule
        /// </summary>
        public override void ApplyRule(DataRow row)
        {
            if (AssignmentType.Equals(SimpleAssignType.YearsElapsed))
            {
                string minDateColumnName = AssignmentParameters[0];
                string maxDateColumnName = AssignmentParameters[1];

                if (row[minDateColumnName] == null || row[maxDateColumnName] == null || string.IsNullOrEmpty(row[minDateColumnName].ToString()) || string.IsNullOrEmpty(row[maxDateColumnName].ToString()))
                {
                    row[this.DestinationColumnName] = DBNull.Value;
                }
                else
                {
                    DateTime minDate = (DateTime)row[minDateColumnName];
                    DateTime maxDate = (DateTime)row[maxDateColumnName];

                    int years = maxDate.Year - minDate.Year;
                    if
                    (
                        maxDate.Month < minDate.Month ||
                        (maxDate.Month == minDate.Month && maxDate.Day < minDate.Day)
                    )
                    {
                        years--;
                    }

                    row[this.DestinationColumnName] = years;
                }
            }
            else if (AssignmentType.Equals(SimpleAssignType.MonthsElapsed))
            {
                string minDateColumnName = AssignmentParameters[0];
                string maxDateColumnName = AssignmentParameters[1];

                if (row[minDateColumnName] == null || row[maxDateColumnName] == null || string.IsNullOrEmpty(row[minDateColumnName].ToString()) || string.IsNullOrEmpty(row[maxDateColumnName].ToString()))
                {
                    row[this.DestinationColumnName] = DBNull.Value;
                }
                else
                {
                    DateTime minDate = Convert.ToDateTime(row[minDateColumnName]);
                    DateTime maxDate = Convert.ToDateTime(row[maxDateColumnName]);

                    int monthsApart = 12 * (maxDate.Year - minDate.Year) + maxDate.Month - minDate.Month;

                    if (maxDate.Day < minDate.Day)
                    {
                        monthsApart--;
                    }

                    row[this.DestinationColumnName] = monthsApart;
                }
            }
            else if (AssignmentType.Equals(SimpleAssignType.DaysElapsed))
            {
                string minDateColumnName = AssignmentParameters[0];
                string maxDateColumnName = AssignmentParameters[1];

                if (row[minDateColumnName] == null || row[maxDateColumnName] == null || string.IsNullOrEmpty(row[minDateColumnName].ToString()) || string.IsNullOrEmpty(row[maxDateColumnName].ToString()))
                {
                    row[this.DestinationColumnName] = DBNull.Value;
                }
                else
                {
                    DateTime minDate = Convert.ToDateTime(row[minDateColumnName]);
                    DateTime maxDate = Convert.ToDateTime(row[maxDateColumnName]);
                    TimeSpan timeSpan = maxDate.Subtract(minDate);
                    double days = Math.Round(timeSpan.TotalDays);
                    row[this.DestinationColumnName] = days;
                }
            }
            else if (AssignmentType.Equals(SimpleAssignType.HoursElapsed))
            {
                string minDateColumnName = AssignmentParameters[0];
                string maxDateColumnName = AssignmentParameters[1];

                if (row[minDateColumnName] == null || row[maxDateColumnName] == null || string.IsNullOrEmpty(row[minDateColumnName].ToString()) || string.IsNullOrEmpty(row[maxDateColumnName].ToString()))
                {
                    row[this.DestinationColumnName] = DBNull.Value;
                }
                else
                {
                    DateTime minDate = Convert.ToDateTime(row[minDateColumnName]);
                    DateTime maxDate = Convert.ToDateTime(row[maxDateColumnName]);
                    TimeSpan timeSpan = maxDate.Subtract(minDate);
                    double hours = Math.Round(timeSpan.TotalHours);
                    row[this.DestinationColumnName] = hours;
                }
            }
            else if (AssignmentType.Equals(SimpleAssignType.MinutesElapsed))
            {
                string minDateColumnName = AssignmentParameters[0];
                string maxDateColumnName = AssignmentParameters[1];

                if (row[minDateColumnName] == null || row[maxDateColumnName] == null || string.IsNullOrEmpty(row[minDateColumnName].ToString()) || string.IsNullOrEmpty(row[maxDateColumnName].ToString()))
                {
                    row[this.DestinationColumnName] = DBNull.Value;
                }
                else
                {
                    DateTime minDate = Convert.ToDateTime(row[minDateColumnName]);
                    DateTime maxDate = Convert.ToDateTime(row[maxDateColumnName]);
                    TimeSpan timeSpan = maxDate.Subtract(minDate);
                    double hours = Math.Round(timeSpan.TotalMinutes);
                    row[this.DestinationColumnName] = hours;
                }
            }
            else if (AssignmentType.Equals(SimpleAssignType.TextToNumber))
            {
                string textColumnName = AssignmentParameters[0];
                string value = row[textColumnName].ToString().Trim();

                if (row[textColumnName] == null || string.IsNullOrEmpty(value))
                {
                    row[this.DestinationColumnName] = DBNull.Value;
                }
                else
                {
                    double result;
                    bool success = double.TryParse(value, out result);

                    if (success)
                    {
                        row[this.DestinationColumnName] = result;
                    }
                    else
                    {
                        row[this.DestinationColumnName] = DBNull.Value;
                    }
                }
            }
            else if (AssignmentType.Equals(SimpleAssignType.TextToDate))
            {
                string textColumnName = AssignmentParameters[0];
                string value = row[textColumnName].ToString().Trim();

                if (row[textColumnName] == null || string.IsNullOrEmpty(value))
                {
                    row[this.DestinationColumnName] = DBNull.Value;
                }
                else
                {
                    try
                    {
                        DateTime? dateField = DateTime.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
                        row[this.DestinationColumnName] = dateField;
                    }
                    catch (Exception)
                    {

                        row[this.DestinationColumnName] = DBNull.Value;
                    }
                }
            }
            else if (AssignmentType.Equals(SimpleAssignType.FindText))
            {
                string textColumnName = AssignmentParameters[0];
                string searchString = AssignmentParameters[1];

                string value = row[textColumnName].ToString();
                int indexOf = value.IndexOf(searchString);

                if (indexOf == -1)
                {
                    row[this.DestinationColumnName] = DBNull.Value;
                }
                else
                {
                    row[this.DestinationColumnName] = indexOf;
                }
            }
            else if (AssignmentType.Equals(SimpleAssignType.StringLength))
            {
                string textColumnName = AssignmentParameters[0];

                string value = row[textColumnName].ToString();
                row[this.DestinationColumnName] = value.Length;
            }
            else if (AssignmentType.Equals(SimpleAssignType.Round))
            {
                string numericColumnName = AssignmentParameters[0];
                int decimals = 0;
                int.TryParse(AssignmentParameters[1], out decimals);

                string value = row[numericColumnName].ToString().Trim();

                if (row[numericColumnName] == null || string.IsNullOrEmpty(value))
                {
                    row[this.DestinationColumnName] = DBNull.Value;
                }
                else
                {
                    decimal result;
                    bool success = decimal.TryParse(value, out result);
                    result = Math.Round(result, decimals);

                    if (success)
                    {
                        row[this.DestinationColumnName] = result;
                    }
                    else
                    {
                        row[this.DestinationColumnName] = DBNull.Value;
                    }
                }
            }
            else if (AssignmentType.Equals(SimpleAssignType.Substring))
            {
                string textColumnName = AssignmentParameters[0];
                int start = 0;
                int length = 0;

                if (dashboardHelper.TableColumnNames.ContainsKey(AssignmentParameters[1]))
                {
                    start = int.Parse(row[AssignmentParameters[1]].ToString());
                }
                else
                {
                    start = int.Parse(AssignmentParameters[1]);
                }

                if (dashboardHelper.TableColumnNames.ContainsKey(AssignmentParameters[2]))
                {
                    length = int.Parse(row[AssignmentParameters[2]].ToString());
                }
                else
                {
                    length = int.Parse(AssignmentParameters[2]);
                }

                --start;

                string value = row[textColumnName].ToString();

                if (start > row[textColumnName].ToString().Length)
                {
                    row[this.DestinationColumnName] = DBNull.Value;
                }
                else if (start + length > row[textColumnName].ToString().Length)
                {
                    int fullLength = row[textColumnName].ToString().Length;
                    int newLength = fullLength - start;
                    value = row[textColumnName].ToString().Substring(start, newLength);
                }
                else
                {
                    value = row[textColumnName].ToString().Substring(start, length);
                }

                row[this.DestinationColumnName] = value;          
            }
        }
        #endregion // IDashboardRule Members
    }
}