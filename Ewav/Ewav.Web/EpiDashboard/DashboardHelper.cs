/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       DashboardHelper.cs
 *  Namespace:  EpiDashboard    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using Epi;
using Epi.Data;
using Epi.Fields;
using Ewav.Web.EpiDashboard.Rules;
using Ewav.BAL;
using Ewav.Web;
using Ewav.Web.Services;

namespace Ewav.Web.EpiDashboard
{
    /// <summary>
    /// A class used for dashboard-based data management
    /// </summary>
    public class DashboardHelper
    {
        #region Private Members
        //   View view    
        //  private DashboardControl dashboardControl;
        private readonly CaseInsensitiveEqualityComparer caseInsensitiveEqualityComparer = new CaseInsensitiveEqualityComparer();
        private DataTable mainTable;
        private readonly object syncLock = new object();
        private readonly object syncLockView = new object();
        private bool userVarsNeedUpdating = false;
        private DateTime lastCacheTime = DateTime.Now;
        private string timeToCache = string.Empty;
        private string customQuery = string.Empty;
        readonly StringLiterals stringLiterals;
        readonly List<EwavRule_Base> _dashboardRulesTable = null;

        private readonly IEnumerable<Ewav.Web.Services.EwavDataFilterCondition> ewavDataFilters;

        #endregion // Private Members

        #region Constructors


        private int FREQUENCY_CROSSTAB_LIMIT = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["FREQUENCY_CROSSTAB_LIMIT"]); //100;
        private int ABERRATION_ROW_LIMIT = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["ABERRATION_ROW_LIMIT"]); //366;
        private int FREQUENCY_ROW_LIMIT = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["FREQUENCY_ROW_LIMIT"]); //200
        private int Combined_Frequency_Row_Limit = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["FREQUENCY_ROW_LIMIT"]); //200
        private DataFilters dataFiltersForUserDef;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ewavDataFilters"></param>
        public DashboardHelper(GadgetParameters gp, IEnumerable<Ewav.Web.Services.EwavDataFilterCondition> ewavDataFilters,
            List<EwavRule_Base> rulesTable = null)
            : this()
        {
            DataFilters = new DataFilters(this);
            DataFiltersForUserDef = new DataFilters(this);

            this.ewavDataFilters = ewavDataFilters;
            _dashboardRulesTable = rulesTable;

            //bool conditionalRuleExsists;
            //foreach (EwavRule_Base rule in rulesTable)
            //{
            //    if (rule is EwavRule_ConditionalAssign)
            //    {
            //        conditionalRuleExsists = true;
            //    }
            //}

            // if conditional rule exists we can't process here 
            ConstructWithRules(gp);


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ewavDataFilters"></param>
        public DashboardHelper(GadgetParameters gp, string filterString, List<EwavRule_Base> rulesTable = null)
            : this()
        {
            DataFilters = new DataFilters(this);
            DataFiltersForUserDef = new DataFilters(this);

            AdvancedUserDataFilter = filterString;
            UseAdvancedUserDataFilter = true;
            _dashboardRulesTable = rulesTable;
            ConstructWithRules(gp);
        }


        /// <summary>
        /// Default constructor
        /// </summary>
        public DashboardHelper()
        {

            DataFilters = new DataFilters(this);
            DataFiltersForUserDef = new DataFilters(this);

            this.TableName = string.Empty;
            this.View = new EwavView();
            stringLiterals = new StringLiterals();
            Construct();
        }

        /// <summary>
        /// Gets or sets the data filters for user def.
        /// </summary>
        /// <value>The data filters for user def.</value>
        public DataFilters DataFiltersForUserDef
        {
            get
            {
                return this.dataFiltersForUserDef;
            }
            set
            {
                this.dataFiltersForUserDef = value;
            }
        }

        private void ConstructWithRules(GadgetParameters gp)
        {
            if (_dashboardRulesTable != null)
            {
                for (int i = 0; i < _dashboardRulesTable.Count; i++)
                {
                    IDashboardRule ruleObj = RetreiveRuleFromClient(_dashboardRulesTable[i], gp);
                    this.AddRule(ruleObj);
                }
            }
        }

        private IDashboardRule RetreiveRuleFromClient(EwavRule_Base rule, GadgetParameters gp)
        {
            if (rule is EwavRule_Recode)
            {
                EwavRule_Recode ewavRule_Recode = rule as EwavRule_Recode;

                DataTable dt = ConvertListToDataTable(ewavRule_Recode.RecodeTable);

                return new Rule_Recode(this, ewavRule_Recode.Friendlyrule,
                    ewavRule_Recode.SourceColumnName,
                    ewavRule_Recode.SourceColumnType, ewavRule_Recode.TxtDestinationField,
                    ewavRule_Recode.DestinationFieldType, dt, ewavRule_Recode.TxtElseValue,
                    true, ewavRule_Recode.CheckboxUseWildcardsIndicator);
            }
            else if (rule is EwavRule_SimpleAssignment)
            {
                EwavRule_SimpleAssignment sa = rule as EwavRule_SimpleAssignment;

                List<string> ss = sa.Parameters.Select(f => f.VarName).ToList();
                return new Rule_SimpleAssign(this, sa.FriendlyLabel,
                    sa.TxtDestinationField, sa.AssignmentType, ss);
            }
            else if (rule is EwavRule_ExpressionAssign)
            {
                EwavRule_ExpressionAssign ea = rule as EwavRule_ExpressionAssign;

                return new Rule_ExpressionAssign(this, ea.FriendlyRule,
                    ea.DestinationColumnName, ea.DataType, ea.Expression);
            }
            else if (rule is EwavRule_ConditionalAssign)
            {

                EwavConstructTableColumnNames(gp);

                EwavRule_ConditionalAssign ac = rule as EwavRule_ConditionalAssign;

                DataFilters df = CreateDataFiltersForUserDefVars(ac.ConditionsList);

                Rule_ConditionalAssign rule_ConditionalAssign = new Rule_ConditionalAssign(this, ac.FriendlyRule.VarName, ac.TxtDestination,
                  ac.DestinationColumnType, ac.AssignValue, ac.ElseValue, df.GenerateDataFilterString());

                rule_ConditionalAssign.DataFilters = df;

                return rule_ConditionalAssign;
            }
            else if (rule is EwavRule_Format)
            {
                EwavRule_Format ruleFormat = rule as EwavRule_Format;
                return new Rule_Format(this,
                    ruleFormat.FriendlyLabel,
                    ruleFormat.CbxFieldName,
                    ruleFormat.TxtDestinationField,
                    ruleFormat.CbxFormatOptions,
                    ruleFormat.FormatTypes);
            }
            else if (rule is EwavRule_GroupVariable)
            {
                EwavRule_GroupVariable ruleGroup = rule as EwavRule_GroupVariable;

                MyString ms = new MyString();

                return new Rule_VariableGroup(this, ruleGroup.VaraiableName, ms.ToListOfString(ruleGroup.Items));
            }
            else
            {
                throw new Exception();
            }
        }

        private DataTable ConvertListToDataTable(List<EwavRuleRecodeDataRow> list)
        {
            DataTable dt = new DataTable();
            string tempStr = string.Empty;
            dt.Columns.Add("From");
            dt.Columns.Add("To");
            dt.Columns.Add("Representation");
            for (int i = 0; i < list.Count; i++)
            {
                //dt.Rows.Add()
                DataRow dr = dt.NewRow();
                dr[0] = list[i].col1;
                dr[1] = list[i].col2;
                //tempStr += list[i].col3;
                dr[2] = list[i].col3;
                dt.Rows.Add(dr);
            }

            dt = RemoveEmptyColumn(dt);

            dt.Columns[dt.Columns.Count - 1].ColumnName = "Representation";

            return dt;
        }

        private DataTable RemoveEmptyColumn(DataTable dt)
        {
            DataTable datTab = dt;
            string col2 = string.Empty, col3 = string.Empty;

            for (int i = 0; i < datTab.Rows.Count; i++)
            {
                col2 += datTab.Rows[i][1];
                col3 += datTab.Rows[i][2];
            }

            if (col2.Length == 0)
            {
                datTab.Columns.RemoveAt(1);
            }
            if (col3.Length == 0)
            {
                datTab.Columns.RemoveAt(2);
            }

            return datTab;
        }

        /// <summary>
        /// Constructor for creating the dashboard helper using an Epi Info 7 project
        /// </summary>
        /// <param name="db">The database to attach</param>
        /// <param name="view">The view to attach</param>
        //public DashboardHelper(EwavView view, IDbDriver db)    //   View        
        //{
        //    #region Input Validation
        //    if (view == null)
        //    {
        //        throw new ApplicationException("EwavView can not be null."); // TODO: Update text and move string to shared strings
        //    }

        //    if (db == null)
        //    {
        //        throw new ApplicationException("Data driver can not be null."); // TODO: Update text and move string to shared strings
        //    }
        //    #endregion // Input Validation

        //    //     dataFilters = new DataFilters(this);
        //    this.view = view;
        //    this.tableName = string.Empty;
        //    this.db = db;
        //    Construct();
        //}

        /// <summary>
        /// Constructor for creating the dashboard helper using a standard Epi 7 data sourceObject other than an Epi 7 project
        /// </summary>
        /// <param name="db">The database to attach</param>
        /// <param name="tableName">The name of the table from which to request data</param>
        //public DashboardHelper(string tableName, IDbDriver db)
        //{
        //    #region Input Validation
        //    if (string.IsNullOrEmpty(tableName.Trim()))
        //    {
        //        throw new ApplicationException("Table name cannot be null or empty."); // TODO: Update text and move string to shared strings
        //    }

        //    if (db == null)
        //    {
        //        throw new ApplicationException("Data driver can not be null."); // TODO: Update text and move string to shared strings
        //    }
        //    #endregion // Input Validation

        //    //     dataFilters = new DataFilters(this);
        //    this.view = null;
        //    this.tableName = tableName;
        //    this.db = db;
        //    Construct();
        //}

        /// <summary>
        /// Constructor for creating the dashboard helper using a standard Epi 7 data sourceObject other than an Epi 7 project,
        /// and where the user has specified a custom SQL query.
        /// </summary>
        /// <param name="db">The database to attach</param>
        /// <param name="tableName">The name of the table from which to request data</param>
        //public DashboardHelper(string tableName, string customQuery, IDbDriver db)
        //{
        //    #region Input Validation
        //    if (string.IsNullOrEmpty(tableName.Trim()))
        //    {
        //        throw new ApplicationException("Table name cannot be null or empty."); // TODO: Update text and move string to shared strings
        //    }

        //    if (string.IsNullOrEmpty(customQuery.Trim()))
        //    {
        //        throw new ApplicationException("Query name cannot be null or empty."); // TODO: Update text and move string to shared strings
        //    }

        //    if (db == null)
        //    {
        //        throw new ApplicationException("Data driver can not be null."); // TODO: Update text and move string to shared strings
        //    }
        //    #endregion // Input Validation
        //    //
        //    //      dataFilters = new DataFilters(this);
        //    this.view = null;
        //    this.tableName = tableName;
        //    this.db = db;
        //    this.customQuery = customQuery;
        //    Construct();
        //}

        //public DashboardHelper(string TableName_2)
        //{
        //    // TODO: Complete member initialization
        //    this.TableName_2 = TableName_2;
        //}
        #endregion // Constructors

        #region Public Properties

        /// <summary>
        /// Gets/sets the custom SQL query used for this record set.
        /// </summary>
        public string CustomQuery
        {
            get
            {
                return this.customQuery;
            }
            set
            {
                this.customQuery = value;
            }
        }

        /// <summary>
        /// Gets/sets the record count for the current data sourceObject
        /// </summary>
        public int RecordCount { get; private set; }

        /// <summary>
        /// Gets/sets whether the dashboard helper should use an advanced, user-specified custom data filter
        /// </summary>
        public bool UseAdvancedUserDataFilter { get; set; }

        /// <summary>
        /// Gets/sets a user-specified custom data filter
        /// </summary>
        public string AdvancedUserDataFilter { get; set; }

        /// <summary>
        /// Gets the currently-attached view
        /// </summary>
        /// <remarks>This property will be null if the dashboard was created using a non-Epi 7 project</remarks>
        public EwavView View { get; private set; }

        /// <summary>
        /// Gets/sets the current record processing scope
        /// </summary>
        /// <remarks>Only valid for Epi Info 7 projects, as non-Epi projects generally do not implement a RECSTATUS column</remarks>
        public RecordProcessingScope RecordProcessScope
        {
            get
            {
                return this.DataFilters.RecordProcessScope;
            }
            set
            {
                this.DataFilters.RecordProcessScope = value;
            }
        }

        /// <summary>
        /// Gets/sets whether the user variables need updating
        /// </summary>
        public bool UserVarsNeedUpdating
        {
            get
            {
                return this.userVarsNeedUpdating;
            }
            set
            {
                userVarsNeedUpdating = value;
            }
        }

        /// <summary>
        /// Gets the date/time the data was last cached
        /// </summary>
        public DateTime LastCacheTime
        {
            get
            {
                return this.lastCacheTime;
            }
        }

        /// <summary>
        /// Gets how long it took to last cache the data set locally
        /// </summary>
        public string TimeToCache
        {
            get
            {
                return this.timeToCache;
            }
        }

        /// <summary>
        /// Gets the current set of table column names
        /// </summary>        
        public Dictionary<string, string> TableColumnNames { get; private set; }

        /// <summary>
        /// Gets the current set of fields
        /// </summary>        
        public DataTable FieldTable { get; private set; }

        /// <summary>
        /// Gets the list of related connections
        /// </summary>
        public List<RelatedConnection> ConnectionsForRelate { get; private set; }

        /// <summary>
        /// Gets the list of connections for merging
        /// </summary>
        public List<MergeConnection> ConnectionsForMerge { get; private set; }

        /// <summary>
        /// Gets the currently-attached database
        /// </summary>
        public IDbDriver Database { get; private set; }

        /// <summary>
        /// Gets the current dataset
        /// </summary>
        public DataSet DataSet { get; private set; }

        /// <summary>
        /// Gets the name of the table from which to request data
        /// </summary>
        /// <remarks>This property will be empty if the dashboard is using an Epi 7 project</remarks>
        public string TableName { get; private set; }

        /// <summary>
        /// Gets the 'safe' table name that can be used in SQL queries
        /// </summary>
        /// <remarks>This property will be empty if the dashboard is using an Epi 7 project</remarks>
        public string SafeTableName
        {
            get
            {
                return FormatTableName(this.TableName);
            }
        }

        /// <summary>
        /// Gets whether or not the dashboard is using an Epi Info 7 project
        /// </summary>
        public bool IsUsingEpiProject
        {
            get
            {
                return false;

                if (this.View == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Gets the current data filter for the dashboard
        /// </summary>
        public DataFilters DataFilters { get; set; }

        /// <summary>
        /// Gets the current set of rules for the dashboard
        /// </summary>
        public DashboardRules Rules { get; private set; }

        /// <summary>
        /// Gets the Epi Info configuration associated with this instance of the dashboard
        /// </summary>
        public Configuration Config { get; private set; }
        #endregion // Public Properties

        #region Events
        #endregion // Events

        #region Public Methods
        /// <summary>
        /// Sets the dashboard control that owns the dashboard helper
        /// </summary>
        /// <param name="control">The control to set</param>
        //public void SetDashboardControl(DashboardControl control)
        //{
        //    this.dashboardControl = control;
        //}

        /// <summary>
        /// Converts the relevant portions of the dashboard helper into HTML
        /// </summary>
        /// <returns>string</returns>
        //public string ToHTML(string htmlFileName = "")
        //{
        //    StringBuilder htmlOutput = new StringBuilder();

        //    htmlOutput.AppendLine("<p>");
        //    if (IsUsingEpiProject)
        //    {
        //        htmlOutput.AppendLine("<em>Current form:</em> ");
        //        htmlOutput.AppendLine("<strong>" + this.View.FullName + "</strong>");
        //    }
        //    else
        //    {
        //        htmlOutput.AppendLine("<em>Current data sourceObject:</em> ");
        //        htmlOutput.AppendLine("<strong>" + this.Database.ConnectionDescription + "</strong>");
        //    }

        //    htmlOutput.AppendLine("<br/>");

        //    if (DataFilters.Count > 0 || (UseAdvancedUserDataFilter && !string.IsNullOrEmpty(AdvancedUserDataFilter)))
        //    {
        //        htmlOutput.AppendLine("<em>Active data filters:</em> ");
        //        htmlOutput.AppendLine("<strong>" + GenerateFilterCriteria() + "</strong>");
        //        htmlOutput.AppendLine("<br />");
        //    }

        //    htmlOutput.AppendLine("<em>Record count:</em> ");
        //    htmlOutput.AppendLine("<strong>" + this.RecordCount.ToString() + "</strong>");

        //    //switch (this.RecordProcessScope)
        //    //{
        //    //    case RecordProcessingScope.Undeleted:
        //    //        htmlOutput.AppendLine(" <em>(Deleted records excluded)</em> ");
        //    //        break;
        //    //    case RecordProcessingScope.Deleted:
        //    //        htmlOutput.AppendLine(" <em>(Deleted records only)</em> ");
        //    //        break;
        //    //    case RecordProcessingScope.Both:
        //    //        htmlOutput.AppendLine(" <em>(Both deleted and undeleted records)</em> ");
        //    //        break;
        //    //}

        //    htmlOutput.AppendLine(" <em>Date:</em> ");
        //    htmlOutput.AppendLine("<strong>" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "</strong>");

        //    htmlOutput.AppendLine("</p>");

        //    return htmlOutput.ToString();
        //}

        /// <summary>
        /// Converts the dashboard into XML
        /// </summary>
        /// <returns>XmlElement</returns>
        public XmlElement Serialize(XmlDocument doc)
        {
            System.Xml.XmlElement root = doc.CreateElement("dashboardHelper");

            string xmlString = string.Empty;

            if (IsUsingEpiProject)
            {
                xmlString =
                    string.Format("<projectPath>{0}</projectPath><viewName>{1}</viewName><connectionString>{2}</connectionString><tableName>{3}</tableName>", this.View.Project.FilePath, this.View.Name, string.Empty, string.Empty);
            }
            else
            {
                xmlString =
                    string.Format("<projectPath>{0}</projectPath><viewName>{1}</viewName><connectionString>{2}</connectionString><tableName>{3}</tableName>", string.Empty, string.Empty, Configuration.Encrypt(this.Database.ConnectionString), this.TableName);

                if (!string.IsNullOrEmpty(this.CustomQuery))
                {
                    xmlString = string.Format("{0}<customSQLQuery>{1}</customSQLQuery>", xmlString, this.CustomQuery);
                }
            }

            xmlString +=
                string.Format("<advancedDataFilterCondition>{0}</advancedDataFilterCondition><useAdvancedDataFilterCondition>{1}</useAdvancedDataFilterCondition>", this.AdvancedUserDataFilter, this.UseAdvancedUserDataFilter.ToString());

            root.InnerXml = xmlString;
            root.AppendChild(SerializeRelatedDataConnections(doc));
            root.AppendChild(SerializeFilters(doc));
            root.AppendChild(SerializeRules(doc));

            return root;
        }

        /// <summary>
        /// Creates a dashboard helper from an XML element
        /// </summary>
        /// <param name="element">The element from which to create the dashboard helper</param>
        //        public void CreateFromXml(XmlElement element)
        //        {
        //            string projectPath = string.Empty;
        //            string viewName = string.Empty;
        //            string connectionString = string.Empty;
        //            string table = string.Empty;

        //            TableColumnNames.Clear();
        ////
        //        //    this.dataFilters = new DataFilters(this);

        //            foreach (System.Xml.XmlElement child in element.ChildNodes)
        //            {
        //                string name = child.Name.ToLower();

        //                if (name.Equals("projectpath"))
        //                {
        //                    projectPath = child.InnerText;
        //                }
        //                else if (name.Equals("viewname"))
        //                {
        //                    viewName = child.InnerText;
        //                }
        //                else if (name.Equals("connectionstring") && !string.IsNullOrEmpty(child.InnerText))
        //                {
        //                    connectionString = Configuration.Decrypt(child.InnerText);
        //                }
        //                else if (name.Equals("tablename"))
        //                {
        //                    table = child.InnerText;
        //                }
        //                else if (name.Equals("datafilters"))
        //                {
        //                    this.DataFilters.CreateFromXml(child);
        //                }
        //                else if (name.Equals("dashboardrules"))
        //                {
        //                    this.Rules.CreateFromXml(child);
        //                }
        //                else if (name.Equals("advanceddatafiltercondition"))
        //                {
        //                    this.AdvancedUserDataFilter = child.InnerText;
        //                }
        //                else if (name.Equals("useadvanceddatafiltercondition"))
        //                {
        //                    this.UseAdvancedUserDataFilter = bool.Parse(child.InnerText);
        //                }
        //                else if (name.Equals("customsqlquery"))
        //                {
        //                    this.CustomQuery = child.InnerText.Trim();
        //                }

        //                else if (name.Equals("relateddataconnections"))
        //                {
        //                    foreach (System.Xml.XmlElement grandChild in child.ChildNodes)
        //                    {
        //                        if (grandChild.Name.ToLower().Equals("relateddataconnection"))
        //                        {
        //                            string rel_projectPath = string.Empty;
        //                            string rel_viewName = string.Empty;
        //                            string rel_connectionString = string.Empty;
        //                            string rel_tableName = string.Empty;
        //                            string rel_parentKey = string.Empty;
        //                            string rel_childKey = string.Empty;
        //                            bool rel_useUnmatched = true;
        //                            bool rel_sameDataSource = true;

        //                            foreach (XmlElement rChild in grandChild.ChildNodes)
        //                            {
        //                                if (rChild.Name.ToLower().Equals("projectpath"))
        //                                {
        //                                    rel_projectPath = rChild.InnerText;
        //                                }
        //                                else if (rChild.Name.ToLower().Equals("viewname"))
        //                                {
        //                                    rel_viewName = rChild.InnerText;
        //                                }
        //                                else if (rChild.Name.ToLower().Equals("connectionstring") && !string.IsNullOrEmpty(rChild.InnerText))
        //                                {
        //                                    rel_connectionString = Configuration.Decrypt(rChild.InnerText);
        //                                }
        //                                else if (rChild.Name.ToLower().Equals("tablename"))
        //                                {
        //                                    rel_tableName = rChild.InnerText;
        //                                }
        //                                else if (rChild.Name.ToLower().Equals("parentkeyfield"))
        //                                {
        //                                    rel_parentKey = rChild.InnerText;
        //                                }
        //                                else if (rChild.Name.ToLower().Equals("childkeyfield"))
        //                                {
        //                                    rel_childKey = rChild.InnerText;
        //                                }
        //                                else if (rChild.Name.ToLower().Equals("useunmatched"))
        //                                {
        //                                    bool.TryParse(rChild.InnerText, out rel_useUnmatched);
        //                                }
        //                                else if (rChild.Name.ToLower().Equals("samedatasource"))
        //                                {
        //                                    bool.TryParse(rChild.InnerText, out rel_sameDataSource);
        //                                }
        //                            }

        //                            if (string.IsNullOrEmpty(rel_viewName))
        //                            {
        //                                #region Validate and Update Project Path

        //                                if (!string.IsNullOrEmpty(projectPath))
        //                                {
        //                                    if (rel_sameDataSource)
        //                                    {
        //                                        FileInfo fiMainProject = new FileInfo(projectPath);
        //                                        Project mainProject = new Project(projectPath);

        //                                        if (rel_connectionString.ToLower().StartsWith("provider=microsoft.ace") || rel_connectionString.ToLower().StartsWith("provider=microsoft.jet.oledb"))
        //                                        {
        //                                            string filePath = string.Empty;

        //                                            int indexOf = rel_connectionString.IndexOf("Data Source=");

        //                                            if (indexOf >= 0)
        //                                            {
        //                                                indexOf += 12;
        //                                                filePath = rel_connectionString.Substring(indexOf, rel_connectionString.Length - indexOf);

        //                                                indexOf = filePath.IndexOf(';');

        //                                                if (indexOf >= 0)
        //                                                {
        //                                                    filePath = filePath.Substring(0, indexOf);
        //                                                    filePath = filePath.TrimStart('\"');
        //                                                    filePath = filePath.TrimEnd('\"');

        //                                                    FileInfo fiDataSource = new FileInfo(filePath);

        //                                                    string dsPath = filePath;

        //                                                    if (File.Exists(fiMainProject.Directory + "\\" + fiDataSource.Name))
        //                                                    {
        //                                                        dsPath = fiMainProject.Directory + "\\" + fiDataSource.Name;
        //                                                        rel_connectionString = rel_connectionString.Replace(filePath, dsPath);
        //                                                        //rel_connectionString = dsPath;
        //                                                    }
        //                                                    else if (!System.IO.File.Exists(dsPath))
        //                                                    {
        //                                                        string message = string.Format("The data sourceObject for this canvas cannot be found: " + dsPath);
        //                                                        Epi.Windows.MsgBox.ShowError(message);
        //                                                        return;
        //                                                    }
        //                                                }
        //                                            }
        //                                        }

        //                                    }
        //                                    else
        //                                    {
        //                                        // TODO: Notify user the connection was skipped
        //                                        continue;
        //                                    }
        //                                }

        //                                #endregion // Validate and Update Project Path

        //                                IDbDriver database = DBReadExecute.GetDataDriver(rel_connectionString);
        //                                if (database.CheckDatabaseTableExistance(rel_connectionString, rel_tableName, true))
        //                                {
        //                                    RelatedConnection rConn = new RelatedConnection(rel_tableName, database, rel_parentKey, rel_childKey, rel_useUnmatched, rel_sameDataSource);
        //                                    AddRelatedDataSource(rConn);
        //                                }
        //                                else
        //                                {
        //                                    // TODO: Notify user the connection was skipped
        //                                    continue;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                #region Validate and Update Project Path

        //                                if (!File.Exists(rel_projectPath) && !string.IsNullOrEmpty(projectPath))
        //                                {
        //                                    if (rel_sameDataSource)
        //                                    {
        //                                        FileInfo fiRelatedProject = new FileInfo(rel_projectPath);
        //                                        FileInfo fiMainProject = new FileInfo(projectPath);

        //                                        Project mainProject = new Project(projectPath);

        //                                        if (File.Exists(projectPath))
        //                                        {
        //                                            rel_projectPath = fiMainProject.Directory + "\\" + mainProject.Name + ".prj";
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        // TODO: Notify user the connection was skipped
        //                                        continue;
        //                                    }
        //                                }

        //                                #endregion // Validate and Update Project Path

        //                                Project newProject = new Project(rel_projectPath);
        //                                if (newProject.Views.Contains(rel_viewName))
        //                                {
        //                                    EwavView newView = newProject.GetViewByName(rel_viewName);
        //                                    IDbDriver database = DBReadExecute.GetDataDriver(rel_projectPath);
        //                                    RelatedConnection rConn = new RelatedConnection(newView, database, rel_parentKey, rel_childKey, rel_useUnmatched, rel_sameDataSource);
        //                                    AddRelatedDataSource(rConn);
        //                                }
        //                                else
        //                                {
        //                                    throw new ViewNotFoundException("The form named '" + viewName + "' was not found in this project. Please make sure the project you are selecting to use with this canvas file contains the appropriate form.");
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            if (!string.IsNullOrEmpty(projectPath) && !string.IsNullOrEmpty(viewName))
        //            {
        //                if (System.IO.File.Exists(projectPath))
        //                {
        //                    Project newProject = new Project(projectPath);
        //                    if (newProject.Views.Contains(viewName))
        //                    {
        //                        EwavView newView = newProject.GetViewByName(viewName);
        //                        IDbDriver database = DBReadExecute.GetDataDriver(projectPath);

        //                        this.view = newView;
        //                        this.db = database;
        //                    }
        //                    else
        //                    {
        //                        throw new ViewNotFoundException("The form named '" + viewName + "' was not found in this project. Please make sure the project you are selecting to use with this canvas file contains the appropriate form.");
        //                    }
        //                }
        //                else
        //                {
        //                    throw new System.IO.FileNotFoundException("The specified project file (" + projectPath + ") that is associated with this canvas was not found. Please modify the canvas file to point to a valid Epi Info 7 project or select a new data sourceObject.");
        //                }
        //            }
        //            else if (!string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(table))
        //            {
        //                IDbDriver database = DBReadExecute.GetDataDriver(connectionString);

        //                this.db = database;
        //                this.tableName = table;
        //            }
        //            else
        //            {
        //                throw new ApplicationException("Invalid xml input");
        //            }
        //            ConstructTableColumnNames();
        //        }

        /// <summary>
        /// Gets the formatted output for a given column
        /// </summary>
        /// <param name="columnName">The column name</param>
        /// <param name="data">The data to format</param>
        /// <remarks>Should only be run after all statistics processing has completed</remarks>
        /// <returns>string representing the formatted data</returns>
        public string GetFormattedOutput(string columnName, object data)
        {
            bool useSpecialFormatting = false;
            string formatString = string.Empty;
            string displayValue = data.ToString();

            if (IsColumnBoolean(columnName))
            {
                if (data.ToString().ToLower().Equals("true"))
                {
                    displayValue = this.Config.Settings.RepresentationOfYes;
                }
                else if (data.ToString().ToLower().Equals("false"))
                {
                    displayValue = this.Config.Settings.RepresentationOfNo;
                }
            }

            // Get any special formatting rules
            foreach (IDashboardRule rule in this.Rules)
            {
                if (rule is Rule_Format)
                {
                    Rule_Format formatRule = rule as Rule_Format;

                    if (formatRule.ExecutionLocation == RuleExecutionLocation.ExecuteAfter)
                    {
                        if (formatRule.SourceColumnName.Equals(columnName))
                        {
                            formatString = formatRule.GetFormatString();
                            useSpecialFormatting = true;
                        }
                    }
                }
            }

            if (useSpecialFormatting)
            {
                displayValue = string.Format(System.Globalization.CultureInfo.CurrentCulture, formatString, data);
            }

            return displayValue;
        }

        /// <summary>
        /// Adds a new data filtering condition
        /// </summary>
        /// <param name="friendlyCondition">The human-readable condition</param>
        /// <param name="friendlyOperand">The human-readable operand</param>
        /// <param name="friendlyValue">The human-readable value</param>
        /// <param name="columnName">The name of the column</param>
        /// <param name="joinType">The type of join to use in including this condition in the list of conditions</param>
        /// <returns>Whether the addition was successful</returns>
        public bool AddDataFilterCondition(DataFilters datafilterGroup, string friendlyOperand, string friendlyValue, string columnName, ConditionJoinType joinType)
        {
            #region Input Validation
            if (string.IsNullOrEmpty(friendlyOperand.Trim()))
            {
                throw new ArgumentNullException("friendlyOperand");
            }
            else if (string.IsNullOrEmpty(columnName.Trim()))
            {
                throw new ArgumentNullException("columnName");
            }
            #endregion // Input Validation

            bool added = false;
            string columnType = GetColumnType(columnName);
            string rawColumnName = columnName;

            columnName = AddBracketsToString(columnName);

            if ((columnType.Equals("System.DateTime") || columnType.Equals("System.DateTimeOffset")) && friendlyOperand.Equals(SharedStrings.FRIENDLY_OPERATOR_EQUAL_TO))
            {
                DateTime lowVal = DateTime.Parse(friendlyValue, System.Globalization.CultureInfo.CurrentCulture);
                string friendlyLowValue = lowVal.ToString("M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                DateTime highVal = DateTime.Parse(friendlyValue, System.Globalization.CultureInfo.CurrentCulture);
                string friendlyHighValue = highVal.ToString("M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                FilterCondition newCondition = datafilterGroup.GenerateFilterCondition(columnName, rawColumnName, columnType, friendlyOperand, friendlyLowValue, friendlyHighValue);
                datafilterGroup.AddFilterCondition(newCondition, joinType);
            }
            else if ((columnType.Equals("System.DateTime") || columnType.Equals("System.DateTimeOffset")) && friendlyOperand.Equals(SharedStrings.FRIENDLY_OPERATOR_LESS_THAN_OR_EQUAL))
            {
                DateTime dateVal = DateTime.Parse(friendlyValue, System.Globalization.CultureInfo.CurrentCulture);

                if (dateVal.Hour == 0)
                {
                    dateVal = dateVal.AddHours(23);
                }
                if (dateVal.Minute == 0)
                {
                    dateVal = dateVal.AddMinutes(59);
                }
                if (dateVal.Second == 0)
                {
                    dateVal = dateVal.AddSeconds(59);
                }
                if (dateVal.Millisecond == 0)
                {
                    dateVal = dateVal.AddMilliseconds(999);
                }

                friendlyValue = dateVal.ToString("M/d/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                FilterCondition newCondition = datafilterGroup.GenerateFilterCondition(columnName, rawColumnName, columnType, friendlyOperand, friendlyValue);
                datafilterGroup.AddFilterCondition(newCondition, joinType);
            }
            else if ((columnType.Equals("System.DateTime") || columnType.Equals("System.DateTimeOffset")) && friendlyOperand.Equals(SharedStrings.FRIENDLY_OPERATOR_GREATER_THAN))
            {
                DateTime dateVal = DateTime.Parse(friendlyValue, System.Globalization.CultureInfo.CurrentCulture);

                if (dateVal.Hour == 0)
                {
                    dateVal = dateVal.AddHours(23);
                }
                if (dateVal.Minute == 0)
                {
                    dateVal = dateVal.AddMinutes(59);
                }
                if (dateVal.Second == 0)
                {
                    dateVal = dateVal.AddSeconds(59);
                }
                if (dateVal.Millisecond == 0)
                {
                    dateVal = dateVal.AddMilliseconds(999);
                }

                friendlyValue = dateVal.ToString("M/d/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                FilterCondition newCondition = datafilterGroup.GenerateFilterCondition(columnName, rawColumnName, columnType, friendlyOperand, friendlyValue);
                datafilterGroup.AddFilterCondition(newCondition, joinType);
            }
            else if ((columnType.Equals("System.DateTime") || columnType.Equals("System.DateTimeOffset")))
            {
                if (string.IsNullOrEmpty(friendlyValue))
                {
                    friendlyValue = EwavTempRepresentationOfMissing; // Config.Settings.RepresentationOfMissing;
                }
                else
                {
                    DateTime dateVal = DateTime.Parse(friendlyValue, System.Globalization.CultureInfo.CurrentCulture);
                    friendlyValue = dateVal.ToString("M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }

                FilterCondition newCondition = datafilterGroup.GenerateFilterCondition(columnName, rawColumnName, columnType, friendlyOperand, friendlyValue);
                datafilterGroup.AddFilterCondition(newCondition, joinType);
            }
            else
            {
                FilterCondition newCondition = datafilterGroup.GenerateFilterCondition(columnName, rawColumnName, columnType, friendlyOperand, friendlyValue);
                datafilterGroup.AddFilterCondition(newCondition, joinType);
            }

            return added;
        }

        /// <summary>
        /// Adds a new data filtering condition using the between operator
        /// </summary>
        /// <param name="friendlyCondition">The human-readable condition</param>
        /// <param name="condition">The condition to add</param>
        /// <returns>Whether the addition was successful</returns>
        public bool AddDataFilterCondition(DataFilters datafilterGroup, string friendlyOperand, string friendlyLowValue, string friendlyHighValue, string columnName, ConditionJoinType joinType)
        {
            #region Input Validation
            if (string.IsNullOrEmpty(friendlyOperand.Trim()))
            {
                throw new ArgumentNullException("friendlyOperand");
            }
            else if (string.IsNullOrEmpty(columnName.Trim()))
            {
                throw new ArgumentNullException("columnName");
            }
            else if (string.IsNullOrEmpty(friendlyLowValue.Trim()))
            {
                throw new ArgumentNullException("friendlyLowValue");
            }
            else if (string.IsNullOrEmpty(friendlyHighValue.Trim()))
            {
                throw new ArgumentNullException("friendlyHighValue");
            }
            #endregion // Input Validation

            string columnType = GetColumnType(columnName);
            string rawColumnName = columnName;

            if (!IsUsingEpiProject)
            {
                columnName = AddBracketsToString(columnName);
            }

            if (columnType.Equals("System.DateTime") || columnType.Equals("System.DateTimeOffset"))
            {
                DateTime lowVal = DateTime.Parse(friendlyLowValue, System.Globalization.CultureInfo.CurrentCulture);
                friendlyLowValue = lowVal.ToString("M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                DateTime highVal = DateTime.Parse(friendlyHighValue, System.Globalization.CultureInfo.CurrentCulture);
                friendlyHighValue = highVal.ToString("M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }

            bool added = false;
            FilterCondition newCondition = datafilterGroup.GenerateFilterCondition(columnName, rawColumnName, columnType, friendlyOperand, friendlyLowValue, friendlyHighValue);
            datafilterGroup.AddFilterCondition(newCondition, joinType);

            added = true;


            return added;
        }

        /// <summary>
        /// Gets the associated fields (if any) for a given column
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public Field GetAssociatedField(string columnName)
        {
            Field field = null;
            foreach (DataRow fieldRow in FieldTable.Rows)
            {
                if (fieldRow["columnname"].Equals(columnName))
                {
                    if (fieldRow["epifieldtype"] is Field)
                    {
                        field = fieldRow["epifieldtype"] as Field;
                    }
                    break;
                }
            }

            return field;
        }

        /// <summary>
        /// Gets a standard style HTML tag for use in gadgets, the same as used on the owning dashboard control object, if needed by a gadget - for example, if the gadget has a direct HTML export routine that bypasses the dashboard's own ToHTML routine.
        /// </summary>
        /// <returns></returns>
        public StringBuilder GenerateStandardHTMLStyle()
        {
            StringBuilder sb = new StringBuilder();
            //if (dashboardControl != null)
            //{
            //    dashboardControl.GenerateStandardHTMLStyle(sb);
            //}
            return sb;
        }

        /// <summary>
        /// Adds a new rule
        /// </summary>
        /// <param name="recodeRule">The rule to add</param>
        /// <returns>Whether the addition was successful</returns>
        public bool AddRule(IDashboardRule rule)
        {
            return Rules.AddRule(rule);
        }

        /// <summary>
        /// Removes a selection condition
        /// </summary>
        /// <param name="friendlyCondition">The human-readable condition to remove</param>
        //public void RemoveDataFilterCondition(string friendlyCondition)
        //{
        //    //         DataFilters.RemoveFilterCondition(friendlyCondition);
        //}

        /// <summary>
        /// Determines if a column is a fields on the data entry form or if it's a column holding re-formatted or re-coded data
        /// </summary>
        /// <param name="columnName">The name of the column to check</param>
        /// <returns>bool</returns>
        public bool IsUserDefinedColumn(string columnName)
        {
            #region Input Validation
            if (string.IsNullOrEmpty(columnName))
            {
                throw new ArgumentNullException("columnName");
            }
            #endregion // Input Validation

            if (Rules != null && Rules.Count > 0)
            {
                foreach (IDashboardRule rule in Rules)
                {
                    if (rule is DataAssignmentRule)
                    {
                        DataAssignmentRule assignmentRule = rule as DataAssignmentRule;

                        if (assignmentRule.DestinationColumnName.Equals(columnName))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a list of Epi.Data.TableColumns
        /// </summary>
        /// <returns>List of Epi.Data.TableColumns</returns>
        public List<Epi.Data.TableColumn> GetFieldsAsListOfEpiTableColumns(bool sortByTabOrder = false)
        {
            List<Epi.Data.TableColumn> tcList = new List<Epi.Data.TableColumn>();

            foreach (KeyValuePair<string, string> kvp in TableColumnNames)
            {
                string columnName = kvp.Key;
                string columnDataType = kvp.Value;

                if (!string.IsNullOrEmpty(columnDataType))
                {
                    GenericDbColumnType genericDbColumnType = ConvertToGenericDbColumnType(columnDataType);
                    if (!genericDbColumnType.Equals(GenericDbColumnType.Object))
                    {
                        Epi.Data.TableColumn tc = new Epi.Data.TableColumn(columnName, genericDbColumnType, true);
                        if (genericDbColumnType == GenericDbColumnType.String)
                        {
                            tc.Length = 255; // TODO: Look into this.
                        }
                        tcList.Add(tc);
                    }
                }
            }

            if (sortByTabOrder && IsUsingEpiProject)
            {
                Dictionary<string, int> columnNames = new Dictionary<string, int>();
                int runningTabIndex = 0;

                foreach (Page page in this.View.Pages)
                {
                    SortedDictionary<double, string> fieldsOnPage = new SortedDictionary<double, string>();

                    foreach (Field field in page.Fields)
                    {
                        if (field is RenderableField && field is IDataField)
                        {
                            RenderableField renderableField = field as RenderableField;
                            double tabIndex = renderableField.TabIndex;
                            while (fieldsOnPage.ContainsKey(tabIndex))
                            {
                                tabIndex = tabIndex + 0.1;
                            }

                            fieldsOnPage.Add(tabIndex, field.Name);
                        }
                    }

                    foreach (KeyValuePair<double, string> kvp in fieldsOnPage)
                    {
                        columnNames.Add(kvp.Value, runningTabIndex);
                        runningTabIndex++;
                    }
                }

                if (columnNames.ContainsKey("UniqueKey"))
                {
                    columnNames["UniqueKey"] = runningTabIndex + 1;
                }
                else if (columnNames.ContainsKey("UNIQUEKEY"))
                {
                    columnNames["UNIQUEKEY"] = runningTabIndex + 1;
                }
                else
                {
                    columnNames.Add("UNIQUEKEY", runningTabIndex + 1);
                }

                List<Epi.Data.TableColumn> tcTabOrderedList = new List<TableColumn>();
                foreach (KeyValuePair<string, int> kvp in columnNames)
                {
                    foreach (Epi.Data.TableColumn tc in tcList)
                    {
                        if (tc.Name.Equals(kvp.Key))
                        {
                            tcTabOrderedList.Add(tc);
                            break;
                        }
                    }
                }

                foreach (Epi.Data.TableColumn tc in tcList)
                {
                    if (!tcTabOrderedList.Contains(tc))
                    {
                        tcTabOrderedList.Add(tc);
                    }
                }

                tcList = tcTabOrderedList;
            }

            return tcList;
        }

        /// <summary>
        /// Updates an existing rule
        /// </summary>
        /// <param name="originalRule">The rule to be updated</param>
        /// <param name="updatedRule">The rule to update</param>
        public void UpdateRule(IDashboardRule originalRule, IDashboardRule updatedRule)
        {
            this.Rules.UpdateRule(originalRule, updatedRule);
        }

        /// <summary>
        /// Removes a rule
        /// </summary>
        /// <param name="friendlyRule">The human-readable rule to remove</param>        
        public void RemoveRule(string friendlyRule)
        {
            this.Rules.RemoveRule(friendlyRule);
        }

        /// <summary>
        /// Clears all data filter conditions
        /// </summary>        
        //public void ClearDataFilterConditions()
        //{
        //    //   DataFilters.ClearFilterConditions();
        //}

        /// <summary>
        /// Clears all formatting rules
        /// </summary>        
        public void ClearRules()
        {
            this.Rules.ClearRules();
        }

        /// <summary>
        /// Orders the columns in a data table
        /// </summary>        
        /// <param name="table">The table whose columns should be re-ordered</param>
        /// <param name="sortByTabOrder">Whether or not the columns should be sorted by tab order (the order of data entry in an Epi Info 7 form), as opposed to being sorted alphabetically.</param>
        internal void OrderColumns(DataTable table, bool sortByTabOrder = false)
        {
            if (!sortByTabOrder || !IsUsingEpiProject)
            {
                List<string> columnNames = new List<string>();

                foreach (DataColumn dc in table.Columns)
                {
                    columnNames.Add(dc.ColumnName);
                }

                //columnNames.Sort();

                if (columnNames.Contains("UniqueKey"))
                {
                    columnNames.Remove("UniqueKey");
                    columnNames.Add("UniqueKey");
                }
                else if (columnNames.Contains("UNIQUEKEY"))
                {
                    columnNames.Remove("UNIQUEKEY");
                    columnNames.Add("UNIQUEKEY");
                }

                for (int i = 0; i < columnNames.Count; i++)
                {
                    string columnName = columnNames[i];
                    table.Columns[columnName].SetOrdinal(i);
                }
            }
            else if (sortByTabOrder && IsUsingEpiProject)
            {
                Dictionary<string, int> columnNames = new Dictionary<string, int>();
                int runningTabIndex = 0;

                foreach (Page page in this.View.Pages)
                {
                    SortedDictionary<double, string> fieldsOnPage = new SortedDictionary<double, string>();

                    foreach (Field field in page.Fields)
                    {
                        if (field is RenderableField && field is IDataField)
                        {
                            RenderableField renderableField = field as RenderableField;
                            double tabIndex = renderableField.TabIndex;
                            while (fieldsOnPage.ContainsKey(tabIndex))
                            {
                                tabIndex = tabIndex + 0.1;
                            }

                            fieldsOnPage.Add(tabIndex, field.Name);
                        }
                    }

                    foreach (KeyValuePair<double, string> kvp in fieldsOnPage)
                    {
                        columnNames.Add(kvp.Value, runningTabIndex);
                        runningTabIndex++;
                    }
                }

                if (columnNames.ContainsKey("UniqueKey"))
                {
                    columnNames["UniqueKey"] = runningTabIndex + 1;
                }
                else if (columnNames.ContainsKey("UNIQUEKEY"))
                {
                    columnNames["UNIQUEKEY"] = runningTabIndex + 1;
                }
                else
                {
                    columnNames.Add("UNIQUEKEY", runningTabIndex + 1);
                }

                int newRunningTabIndex = 0; // to prevent arg exceptions; TODO: Fix this better
                foreach (KeyValuePair<string, int> kvp in columnNames)
                {
                    if (table.Columns.Contains(kvp.Key))
                    {
                        table.Columns[kvp.Key].SetOrdinal(newRunningTabIndex);
                        newRunningTabIndex++;
                    }
                }
            }
        }

        /// <summary>
        /// Resets the View object
        /// </summary>
        /// <param name="view">The new view object to use</param>
        public void ResetView(EwavView view)
        {
            if (IsUsingEpiProject && System.IO.File.Exists(view.Project.FilePath))
            {
                if (this.View.Name != view.Name || this.View.Project.FilePath != view.Project.FilePath)
                {
                    this.View = view;
                    Database = view.Project.CollectedData.GetDbDriver(); //DBReadExecute.GetDataDriver(view.Project.FilePath);
                    this.DataSet.Tables.Clear();
                    this.ConstructTableColumnNames();
                    //   this.dashboardControl.ReCacheDataSource();
                }
            }
        }

        /// <summary>
        /// Change SQL Query
        /// </summary>
        /// <param name="newQuery">The new query to use</param>
        public void SetCustomQuery(string newQuery)
        {
            if (newQuery == string.Empty)
            {
                return;
            }

            string oldQuery = this.CustomQuery;

            if (!IsUsingEpiProject)
            {
                this.customQuery = newQuery;
                this.DataSet.Tables.Clear();

                try
                {
                    ConstructTableColumnNames();
                    //    this.dashboardControl.ReCacheDataSource();
                }
                catch
                {
                    this.customQuery = oldQuery;
                    this.DataSet.Tables.Clear();
                    ConstructTableColumnNames();
                    //    this.dashboardControl.ReCacheDataSource();
                }
            }
        }

        /// <summary>
        /// Gets the column type of a given column
        /// </summary>
        /// <param name="columnName">The name of the column to check</param>
        /// <returns>String representing the column's type</returns>
        public string GetColumnType(string columnName)
        {
            #region Input Validation
            if (string.IsNullOrEmpty(columnName.Trim()))
            {
                throw new ArgumentNullException("columnName");
            }
            #endregion // Input Validation

            string columnType = string.Empty;

            // For a better matching method where the fields name doesn't match the column name in terms of case, e.g. TimeSupper and TIMESUPPER
            foreach (KeyValuePair<string, string> kvp in TableColumnNames)
            {
                string kvpColumnName = kvp.Key.ToLower();
                if (columnName.ToLower().Equals(kvpColumnName))
                {
                    columnType = kvp.Value;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(columnType))
            {
                return columnType;
            }
            else
            {
                throw new ApplicationException(string.Format("The column {0} does not exist in this data set.", columnName));
            }
        }

        /// <summary>
        /// Gets the column DbType of a given column
        /// </summary>
        /// <param name="columnName">The name of the column to check</param>
        /// <returns>DbType</returns>
        public DbType GetColumnDbType(string columnName)
        {
            #region Input Validation
            if (string.IsNullOrEmpty(columnName.Trim()))
            {
                throw new ArgumentNullException("columnName");
            }
            #endregion // Input Validation

            string type = GetColumnType(columnName);

            switch (type)
            {
                case "System.Decimal":
                    return DbType.Decimal;
                case "System.Double":
                    return DbType.Double;
                case "System.Single":
                    return DbType.Single;
                case "System.Int64":
                    return DbType.Int64;
                case "System.DateTime":
                    return DbType.DateTime;
                case "System.Int16":
                    return DbType.Int16;
                case "System.Int32":
                    return DbType.Int32;
                case "System.Boolean":
                    return DbType.Boolean;
                case "System.Byte":
                    return DbType.Byte;
                case "System.String":
                    return DbType.String;
                default:
                    throw new ApplicationException("Unknown column type.");
            }
        }

        /// <summary>
        /// Is the column numeric?
        /// </summary>
        /// <param name="columnName">The column name to check</param>
        /// <returns>bool</returns>
        public bool IsColumnNumeric(string columnName)
        {
            #region Input Validation
            if (string.IsNullOrEmpty(columnName.Trim()))
            {
                throw new ArgumentNullException("columnName");
            }
            #endregion // Input Validation

            string columnType = string.Empty;
            TableColumnNames.TryGetValue(columnName, out columnType);

            switch (columnType)
            {
                case "System.Byte":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.Single":
                case "System.Double":
                case "System.Decimal":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines if the frequency values in a frequency table should show the time component or not
        /// </summary>
        /// <param name="dtDataSources">The data table to process</param>
        /// <param name="columnName">The name of the column to check</param>
        public bool DateColumnRequiresTime(DataTable dt, string columnName)
        {
            #region Input Validation
            if (dt == null)
            {
                throw new ArgumentNullException("dtDataSources");
            }
            if (string.IsNullOrEmpty(columnName))
            {
                throw new ArgumentNullException("columnName");
            }
            #endregion // Input Validation

            int rowCount = dt.Rows.Count;

            if (rowCount == 0)
            {
                return true;
            }

            try
            {
                List<int> rowIndexList = new List<int>();
                rowIndexList.Add(0);

                if (rowCount > 1)
                {
                    rowIndexList.Add(rowCount - 1);
                }

                if (rowCount > 10)
                {
                    rowIndexList.Add(1);
                    rowIndexList.Add(2);
                    rowIndexList.Add(3);
                    rowIndexList.Add(8);
                    rowIndexList.Add(9);
                }

                if (rowCount > 20)
                {
                    rowIndexList.Add(13);
                    rowIndexList.Add(15);
                    rowIndexList.Add(17);
                }

                bool useTime = false;
                DateTime dateTime = new DateTime(2011, 10, 31, 0, 0, 0);
                string dateTimeString = dateTime.ToLongTimeString();

                foreach (int i in rowIndexList)
                {
                    if (!dt.Rows[i][columnName].ToString().EndsWith(dateTimeString))
                    {
                        useTime = true;
                        break;
                    }
                }

                return useTime;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Does the column exist?
        /// </summary>
        /// <param name="columnName">The column name to check</param>
        /// <returns>bool</returns>
        public bool DoesColumnExist(string columnName)
        {
            #region Input Validation
            if (string.IsNullOrEmpty(columnName.Trim()))
            {
                throw new ArgumentNullException("columnName");
            }
            #endregion // Input Validation

            ColumnDataType fieldFilterOptions = ColumnDataType.Boolean | ColumnDataType.DateTime | ColumnDataType.Numeric | ColumnDataType.Text;
            return (GetFieldsAsList(fieldFilterOptions).Contains(columnName, caseInsensitiveEqualityComparer));
        }

        /// <summary>
        /// Is the column boolean?
        /// </summary>
        /// <param name="columnName">The column name to check</param>
        /// <returns>bool</returns>
        public bool IsColumnBoolean(string columnName)
        {
            #region Input Validation
            if (string.IsNullOrEmpty(columnName.Trim()))
            {
                throw new ArgumentNullException("columnName");
            }
            #endregion // Input Validation

            string columnType = string.Empty;
            TableColumnNames.TryGetValue(columnName, out columnType);

            switch (columnType)
            {
                case "System.Boolean":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Is the column for a Yes/No fields?
        /// </summary>
        /// <param name="columnName">The column name to check</param>
        /// <returns>bool</returns>
        public bool IsColumnYesNo(string columnName)
        {
            #region Input Validation
            if (string.IsNullOrEmpty(columnName.Trim()))
            {
                throw new ArgumentNullException("columnName");
            }
            #endregion // Input Validation

            foreach (DataRow fieldRow in FieldTable.Rows)
            {
                if (fieldRow["columnname"].Equals(columnName))
                {
                    if (fieldRow["epifieldtype"] is YesNoField)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Is the column numeric?
        /// </summary>
        /// <param name="columnName">The column name to check</param>
        /// <returns>bool</returns>
        public bool IsColumnText(string columnName)
        {
            #region Input Validation
            if (string.IsNullOrEmpty(columnName.Trim()))
            {
                throw new ArgumentNullException("columnName");
            }
            #endregion // Input Validation

            string columnType = string.Empty;
            TableColumnNames.TryGetValue(columnName, out columnType);

            switch (columnType)
            {
                case "System.String":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Is the column date/time?
        /// </summary>
        /// <param name="columnName">The column name to check</param>
        /// <returns>bool</returns>
        public bool IsColumnDateTime(string columnName)
        {
            #region Input Validation
            if (string.IsNullOrEmpty(columnName.Trim()))
            {
                throw new ArgumentNullException("columnName");
            }
            #endregion // Input Validation

            string columnType = string.Empty;
            TableColumnNames.TryGetValue(columnName, out columnType);

            switch (columnType)
            {
                case "System.DateTime":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Formats a given value so it can be used in selections and filtering
        /// </summary>
        /// <param name="value">The value to format</param>
        /// <param name="columnType">The data type of the column the value belongs to</param>
        /// <returns>String representing the formatted value</returns>
        public string FormatValue(string value, string columnType)
        {
            #region Input Validation
            if (string.IsNullOrEmpty(columnType.Trim()))
            {
                throw new ArgumentException("columnType");
            }
            #endregion // Input Validation

            switch (columnType)
            {
                case "System.DateTime":
                    value = string.Format("#{0}#", value);
                    break;
                case "System.String":
                    value = string.Format("'{0}'", value.Replace("'", "''"));
                    break;
            }
            return value;
        }

        /// <summary>
        /// Adds a related data connection
        /// </summary>
        /// <param name="rConn">Related data connection</param>
        public void AddRelatedDataSource(RelatedConnection rConn)
        {
            if (rConn != null)
            {
                ConnectionsForRelate.Add(rConn);
                ConstructTableColumnNames();
                DataSet.Tables.Clear();
            }
        }

        /// <summary>
        /// Adds all system variables to the specified table
        /// </summary>
        /// <param name="dtDataSources">The table to modify</param>
        public void AddSystemVariablesToTable(DataTable dt)
        {
            if (!dt.Columns.Contains("SYSTEMDATE"))
            {
                DataColumn systemDateColumn = new DataColumn("SYSTEMDATE", typeof(DateTime));
                systemDateColumn.DefaultValue = DateTime.Now;
                dt.Columns.Add(systemDateColumn);

                if (!TableColumnNames.ContainsKey("SYSTEMDATE"))
                {
                    TableColumnNames.Add("SYSTEMDATE", "System.DateTime");
                }

                DataRow row = FieldTable.Rows.Find("SYSTEMDATE");
                if (row == null)
                {
                    FieldTable.Rows.Add("SYSTEMDATE", "System.DateTime", null, null, null);
                }
            }
        }

        /// <summary>
        /// Adds all permanent variables to the specified table
        /// </summary>
        /// <param name="dtDataSources">The table to modify</param>
        public void AddPermanentVariablesToTable(DataTable dt)
        {
            foreach (Epi.DataSets.Config.PermanentVariableRow row in Config.PermanentVariables)
            {
                string dataType = row.DataType.ToString();
                string dataValue = row.DataValue.ToString();
                string variableName = row.Name.ToString();

                if (!dataValue.ToLower().Equals("null") && !dt.Columns.Contains(variableName))
                {
                    switch (dataType)
                    {
                        case "1": // numeric
                            DataColumn numericPermanentVariableColumn = new DataColumn(variableName, typeof(decimal));
                            if (!string.IsNullOrEmpty(dataValue))
                            {
                                numericPermanentVariableColumn.DefaultValue = Convert.ToDecimal(dataValue);
                            }
                            dt.Columns.Add(numericPermanentVariableColumn);
                            break;
                        case "2": // text
                            DataColumn textPermanentVariableColumn = new DataColumn(variableName, typeof(string));
                            textPermanentVariableColumn.DefaultValue = dataValue;
                            dt.Columns.Add(textPermanentVariableColumn);
                            break;
                        case "3": // date/time
                            DataColumn datePermanentVariableColumn = new DataColumn(variableName, typeof(DateTime));
                            if (!string.IsNullOrEmpty(dataValue))
                            {
                                datePermanentVariableColumn.DefaultValue = Convert.ToDateTime(dataValue);
                            }
                            dt.Columns.Add(datePermanentVariableColumn);
                            break;
                        case "6": // boolean
                            DataColumn boolPermanentVariableColumn = new DataColumn(variableName, typeof(bool));
                            if (string.IsNullOrEmpty(dataValue))
                            {
                                boolPermanentVariableColumn.DefaultValue = null;
                            }
                            else
                            {
                                boolPermanentVariableColumn.DefaultValue = bool.Parse(dataValue);
                            }
                            dt.Columns.Add(boolPermanentVariableColumn);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a list of all the fields in the current data set.
        /// </summary>
        /// <returns>List of strings</returns>
        public List<string> GetFieldsAsList()
        {
            List<string> columnNames = new List<string>();

            foreach (KeyValuePair<string, string> kvp in TableColumnNames)
            {
                if (!string.IsNullOrEmpty(kvp.Value) && !kvp.Value.ToLower().StartsWith("epi"))
                {
                    columnNames.Add(kvp.Key);
                }
            }

            columnNames.Sort();
            return columnNames;
        }

        /// <summary>
        /// Gets a list of all the fields in the current data set, with any list filtering options
        /// </summary>
        /// <param name="listFilterOptions"></param>
        /// <returns></returns>
        public List<string> GetFieldsAsList(ColumnDataType fieldFilterOptions)
        {
            List<string> columnNames = new List<string>();

            foreach (KeyValuePair<string, string> kvp in TableColumnNames)
            {
                if (kvp.Value != null)
                {
                    switch (kvp.Value.ToLower())
                    {
                        case "system.string":
                            if ((fieldFilterOptions & ColumnDataType.Text) == ColumnDataType.Text)
                            {
                                columnNames.Add(kvp.Key);
                            }
                            break;
                        case "system.byte":
                        case "system.int16":
                        case "system.int32":
                        case "system.int64":
                        case "system.single":
                        case "system.double":
                        case "system.decimal":
                            if ((fieldFilterOptions & ColumnDataType.Numeric) == ColumnDataType.Numeric)
                            {
                                columnNames.Add(kvp.Key);
                            }
                            break;
                        case "system.datetime":
                            if ((fieldFilterOptions & ColumnDataType.DateTime) == ColumnDataType.DateTime)
                            {
                                columnNames.Add(kvp.Key);
                            }
                            break;
                        case "system.boolean":
                            if ((fieldFilterOptions & ColumnDataType.Boolean) == ColumnDataType.Boolean)
                            {
                                columnNames.Add(kvp.Key);
                            }
                            break;
                    }

                    if (IsUsingEpiProject && View.Fields.Contains(kvp.Key) &&
                        View.Fields[kvp.Key] is YesNoField &&
                        ((fieldFilterOptions & ColumnDataType.Boolean) == ColumnDataType.Boolean) &&
                        !columnNames.Contains(kvp.Key))
                    {
                        columnNames.Add(kvp.Key);
                    }
                }
            }

            // Remove user-defined fields if the options say to do so
            if ((fieldFilterOptions & ColumnDataType.UserDefined) != ColumnDataType.UserDefined)
            {
                foreach (IDashboardRule rule in Rules)
                {
                    if (rule is DataAssignmentRule)
                    {
                        DataAssignmentRule assignRule = rule as DataAssignmentRule;
                        if (columnNames.Contains(assignRule.DestinationColumnName))
                        {
                            columnNames.Remove(assignRule.DestinationColumnName);
                        }
                    }
                }
            }

            // Sort the resulting list
            columnNames.Sort();

            return columnNames;
        }

        /// <summary>
        /// Gets a list of all the user-defined fields created on the dashboard
        /// </summary>        
        /// <returns>List of strings</returns>
        public List<string> GetDefinedFieldsAsList()
        {
            List<string> dashboardFields = new List<string>();

            foreach (KeyValuePair<string, string> kvp in Rules.GetUserDefinedVariables())
            {
                dashboardFields.Add(kvp.Key);
            }

            dashboardFields.Sort();

            return dashboardFields;
        }

        /// <summary>
        /// Gets a list of all the group fields on the form as a list of strings
        /// </summary>        
        /// <returns>List of strings</returns>
        public List<string> GetGroupFieldsAsList(List<EwavRule_Base> Rules)
        {
            List<string> groupFields = new List<string>();
            foreach (var item in Rules)
            {
                if (item is EwavRule_GroupVariable)
                {
                    groupFields.Add(item.VaraiableName);
                }
            }






            //if (IsUsingEpiProject)
            //{
            //    foreach (Field field in this.View.Fields)
            //    {
            //        if (field is GroupField)
            //        {
            //            groupFields.Add(field.Name);
            //        }
            //    }
            groupFields.Sort();
            //}

            return groupFields;
        }

        /// <summary>
        /// Gets the distinct values for a given column as a list
        /// </summary>        
        /// <param name="columnName">The column name to use when selecting the distinct values</param>        
        /// <returns>List containing the distinct values</returns>
        public List<string> GetDistinctValuesAsList(string columnName)
        {
            #region Input Validation
            if (columnName == null || columnName.Length == 0)
            {
                throw new ArgumentNullException("columnName");
            }
            #endregion // Input Validation

            List<string> distinctValues = new List<string>();

            List<string> columnNames = new List<string>();
            columnNames.Add(columnName);

            DataTable distinctTable = GenerateTable(columnNames).DefaultView.ToTable("DistinctTable", true, columnName);

            foreach (DataRow row in distinctTable.Rows)
            {
                distinctValues.Add(row[0].ToString());
            }

            return distinctValues;
        }

        /// <summary>
        /// Gets the number of occurrences of a given value in a given column and table
        /// </summary>
        public int GetDistinctValueCount(DataTable table, string columnName, string value)
        {
            #region Input Validation
            if (columnName == null || columnName.Length == 0)
            {
                throw new ArgumentNullException("columnName");
            }
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            #endregion // Input Validation

            int count = 0;

            foreach (DataRow row in table.Rows)
            {
                if (row[columnName].ToString().Equals(value))
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Determines if a given value in the specified column occurs more than once
        /// </summary>                               
        public bool ValueOccursMoreThanOnce(DataTable table, string columnName, string value)
        {
            #region Input Validation
            if (columnName == null || columnName.Length == 0)
            {
                throw new ArgumentNullException("columnName");
            }
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            #endregion // Input Validation

            List<string> distinctValues = new List<string>();

            foreach (DataRow row in table.Rows)
            {
                if (row[columnName].ToString().Equals(value))
                {
                    distinctValues.Add(row[columnName].ToString());
                    if (distinctValues.Count > 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Merge placeholder
        /// </summary>
        internal void MergeDataSet()
        {
            //foreach(ConnectionsForMerge
        }

        private DataTable JoinPageTables(EwavView vw, bool isRelatedView = false, GadgetParameters inputs = null)
        {
            DataTable unfilteredTable = new DataTable();

            // Get the form's fields count, adding base table fields plus GUID fields for each page. If less than 255, use SQL relate; otherwise, >255 exceeds OLE fields capacity and we need to use a less efficient method
            if (vw.Fields.DataFields.Count + vw.Pages.Count + 3 < 255)
            {
                unfilteredTable = Database.Select(Database.CreateQuery(string.Format("SELECT * {0}", vw.FromViewSQL)));
                if (unfilteredTable.Columns.Contains("t.GlobalRecordId"))
                {
                    unfilteredTable.Columns["t.GlobalRecordId"].ColumnName = "GlobalRecordId";
                }

                foreach (Page page in vw.Pages)
                {
                    string pageGUIDName = string.Format("{0}.GlobalRecordId", page.TableName);
                    if (unfilteredTable.Columns.Contains(pageGUIDName))
                    {
                        unfilteredTable.Columns.Remove(pageGUIDName);
                    }
                }
            }
            else
            {
                DataTable viewTable = new DataTable();
                if (isRelatedView)
                {
                    viewTable = Database.GetTableData(vw.TableName, "GlobalRecordId");
                }
                else
                {
                    viewTable = Database.GetTableData(vw.TableName, "GlobalRecordId, UniqueKey, RECSTATUS");
                }
                viewTable.TableName = vw.TableName;

                DataTable relatedTable = new DataTable("relatedTable");

                int total = 0;
                // Get totals for percent completion calculation
                foreach (Page page in vw.Pages)
                {
                    string pageColumnsToSelect = string.Empty;
                    foreach (Field field in page.Fields)
                    {
                        if (field is RenderableField && field is IDataField)
                        {
                            total++;
                        }
                    }
                }

                total = total * RecordCount;

                int counter = 0;

                foreach (Page page in vw.Pages)
                {
                    string pageColumnsToSelect = string.Empty;
                    foreach (Field field in page.Fields)
                    {
                        if (field is RenderableField && field is IDataField)
                        {
                            pageColumnsToSelect = string.Format("{0}{1},", pageColumnsToSelect, AddBracketsToString(field.Name));
                        }
                    }
                    pageColumnsToSelect = string.Format("{0}{1}", pageColumnsToSelect, AddBracketsToString("GlobalRecordId"));

                    DataTable pageTable = Database.GetTableData(page.TableName, pageColumnsToSelect);
                    pageTable.TableName = page.TableName;

                    foreach (DataColumn dc in pageTable.Columns)
                    {
                        if (dc.ColumnName != "GlobalRecordId")
                        {
                            viewTable.Columns.Add(dc.ColumnName, dc.DataType);
                        }
                    }

                    try
                    {
                        // assume GlobalUniqueId column is unique and try to make it the primary key.
                        DataColumn[] parentPrimaryKeyColumns = new DataColumn[1];
                        parentPrimaryKeyColumns[0] = viewTable.Columns["GlobalRecordId"];
                        viewTable.PrimaryKey = parentPrimaryKeyColumns;
                    }
                    catch
                    {
                    }

                    foreach (DataRow row in pageTable.Rows)
                    {
                        DataRow viewRow = viewTable.Rows.Find(row["GlobalRecordId"].ToString());
                        if (viewRow["GlobalRecordId"].ToString().Equals(row["GlobalRecordId"].ToString()))
                        {
                            foreach (DataColumn dc in pageTable.Columns)
                            {
                                viewRow[dc.ColumnName] = row[dc.ColumnName];
                                counter++;

                                if (counter % 200 == 0 && inputs != null)
                                {
                                    if (counter > total)
                                    {
                                        counter = total;
                                    }
                                    inputs.UpdateGadgetProgress(((double)counter / (double)total) * 100);
                                    inputs.UpdateGadgetStatus(string.Format(SharedStrings.DASHBOARD_GADGET_STATUS_RELATING_PAGES, ((double)counter / (double)total).ToString("P")));
                                    if (inputs.IsRequestCancelled())
                                    {
                                        return null;
                                    }
                                }
                            }
                        }
                    }
                }
                unfilteredTable = viewTable;
            }

            return unfilteredTable;
        }

        /// <summary>
        /// Populates the DataSet object
        /// </summary>
        /// <returns>DataTable</returns>
        internal void PopulateDataSet(GadgetParameters inputs = null)
        {
            DataTable unfilteredTable = new DataTable("unfilteredTable");
            //ds = DatasourceDao
            if (inputs != null)
            {
                inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_CACHING_WAIT);
            }

            lock (syncLock)
            {
                Stopwatch stopwatch = new Stopwatch();

                if (DataSet == null || DataSet.Tables.Count == 0 || (DataSet.Tables[0].Rows.Count == 0 && DataSet.Tables[0].Columns.Count == 0))
                {
                    stopwatch.Start();

                    if (inputs != null)
                    {
                        inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_CACHING);
                    }

                    if (IsUsingEpiProject)
                    {
                        unfilteredTable = JoinPageTables(this.View, false, inputs);

                        if (unfilteredTable == null)
                        {
                            return; // cancelled
                        }
                    }// end if using Epi Info project
                    else
                    {
                        if (string.IsNullOrEmpty(this.CustomQuery))
                        {
                            //unfilteredTable = Database.GetTableData(inputs.TableName);    
                            Ewav.BAL.EwavDatasourceListManager edlm = new Ewav.BAL.EwavDatasourceListManager();

                            //      DataManager dm = new DataManager(edlm.ConnectionString, edlm.MetaDataViewName,
                            //                                edlm.MetaDatabaseTypeEnum);

                            EntityManager em = new EntityManager();

                            //    IRawDataDao rawDataDao = dm.DAOFactory.RawDataDao;

                            unfilteredTable = em.GetRawDataTable(inputs.DatasourceName, inputs.TableName);
                            //  rawDataDao.GetTable(  inputs.DatasourceName, inputs.TableName);       
                            //    CommonSQLDAO dObject = new Ewav.DAL.SqlServer.CommonSQLDAO();      
                            //    unfilteredTable = dObject.GetTable(inputs.DatasourceName);
                        }
                        else
                        {
                            Query selectQuery = Database.CreateQuery(this.CustomQuery);
                            unfilteredTable = Database.Select(selectQuery);
                        }
                    }

                    if (ConnectionsForRelate.Count > 0)
                    {
                        foreach (RelatedConnection conn in ConnectionsForRelate)
                        {
                            DataTable relatedTable = new DataTable();

                            if (conn.IsEpiInfoProject)
                            {
                                //relatedTable = conn.db.Select(conn.db.CreateQuery("SELECT * " + conn.view.FromViewSQL));
                                relatedTable = JoinPageTables(conn.view, true, inputs);

                                if (relatedTable == null)
                                {
                                    break; // skipped
                                }

                                relatedTable.TableName = conn.view.TableName;
                            }
                            else
                            {
                                relatedTable = conn.db.GetTableData(conn.TableName);
                                relatedTable.TableName = conn.TableName;
                            }

                            RelateInto(unfilteredTable, relatedTable, conn.ParentKeyField, conn.ChildKeyField, conn.UseUnmatched, inputs);
                        }
                    }

                    //RecordCount = unfilteredTable.Rows.Count;                    
                    AddSystemVariablesToTable(unfilteredTable);
                    // commented because epi code to support not implemented 

                    //    AddPermanentVariablesToTable(unfilteredTable);
                    DataSet = new DataSet();
                    DataSet.Tables.Add(unfilteredTable.Copy()); //ds.Tables.Add(ConvertTableColumns(unfilteredTable));
                    mainTable = DataSet.Tables[0];
                    stopwatch.Stop();
                    timeToCache = stopwatch.Elapsed.ToString();

                }
                else
                {
                    if (inputs != null)
                    {
                        inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_PROCESSING);
                    }
                }

                if (UserVarsNeedUpdating)
                {
                    ApplyDashboardRules(inputs);
                }
                if (DataSet == null || DataSet.Tables.Count == 0 || (DataSet.Tables[0].Rows.Count == 0 && DataSet.Tables[0].Columns.Count == 0))
                {
                    DataView tempView = new DataView(DataSet.Tables[0], GenerateFilterCriteria(), string.Empty, DataViewRowState.CurrentRows);
                    RecordCount = tempView.Count;
                }

                lastCacheTime = DateTime.Now;
            }

            if (inputs != null)
            {
                inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_INSTRUCTIONS_FINISHED_CACHING);
            }
        }

        /// <summary>
        /// Generates a table of just the top two records in the current data set
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable GenerateTopTwoTable()
        {
            DataTable dt = new DataTable("toptwo");

            if (IsUsingEpiProject)
            {
                // TODO: Use data drivers for the TOP 2 syntax for Epi Info 7 projects
                dt = Database.Select(Database.CreateQuery(string.Format("SELECT TOP 2 * {0}", View.FromViewSQL)));
            }
            else
            {
                dt = Database.GetTopTwoTable(TableName);
            }

            return dt;
        }

        /// <summary>
        /// Generates a sorted .NET DataView based off of the selection criteria and sort criteria
        /// </summary>
        /// <param name="sortOrder">The order by which to sort</param>
        /// <returns>DataTable</returns>
        public DataView GenerateView(GadgetParameters inputs = null)
        {
            string sortOrder = string.Empty;
            string filterCriteria = GenerateFilterCriteria();

            if (inputs != null)
            {
                sortOrder = inputs.CustomSortColumnName;

                if (!string.IsNullOrEmpty(inputs.CustomFilter))
                {
                    if (!string.IsNullOrEmpty(filterCriteria))
                    {
                        filterCriteria = string.Format("{0} AND {1}", filterCriteria, inputs.CustomFilter);
                    }
                    else
                    {
                        filterCriteria = inputs.CustomFilter;
                    }
                }
            }

            PopulateDataSet(inputs);

            lock (syncLockView)
            {
                if (DataSet == null || DataSet.Tables.Count == 0)
                {
                    return null;
                }

                DataSet.Tables[0].DefaultView.RowFilter = GenerateFilterCriteria();

                if(DataSet.Tables[0].Columns.Contains(sortOrder))
                {
                    DataSet.Tables[0].DefaultView.Sort = sortOrder;
                }
                
                DataView dv = DataSet.Tables[0].DefaultView;

                RecordCount = dv.Count;

                if (inputs != null && inputs.CustomFilter != null && inputs.CustomFilter != string.Empty)
                {
                    dv = new DataView(mainTable, filterCriteria, DataSet.Tables[0].DefaultView.Sort, DataViewRowState.CurrentRows);
                }
                else
                {
                    string filter = GenerateFilterCriteria();

                    if(mainTable.Columns.Contains(sortOrder) == false)
                    {
                        sortOrder = string.Empty;
                    }

                    dv = new DataView(mainTable, filter, sortOrder, DataViewRowState.CurrentRows);
                }

                return dv;
            }
        }

        /// <summary>
        /// Generates a sorted .NET DataTable based off of the selection criteria and sort criteria
        /// </summary>
        /// <param name="sortOrder">The order by which to sort</param>
        /// <returns>DataTable</returns>
        public DataTable GenerateTable(GadgetParameters inputs = null)
        {
            List<string> columnNames = new List<string>();

            if (inputs != null)
            {
                //columnNames.AddRange(inputs.ColumnNames.ToList<string>());
                foreach (var item in inputs.ColumnNames)
                {
                    columnNames.Add(item.VarName);
                }

                if (!columnNames.Contains(inputs.MainVariableName, caseInsensitiveEqualityComparer) && !string.IsNullOrEmpty(inputs.MainVariableName.Trim()))
                {
                    columnNames.Add(inputs.MainVariableName);
                }
                if (!columnNames.Contains(inputs.CrosstabVariableName, caseInsensitiveEqualityComparer) && !string.IsNullOrEmpty(inputs.CrosstabVariableName.Trim()))
                {
                    columnNames.Add(inputs.CrosstabVariableName);
                }
                if (!columnNames.Contains(inputs.WeightVariableName, caseInsensitiveEqualityComparer) && !string.IsNullOrEmpty(inputs.WeightVariableName.Trim()))
                {
                    columnNames.Add(inputs.WeightVariableName);
                }
                //foreach (string strataVariableName in inputs.StrataVariableNames)
                foreach (MyString stratVariableName in inputs.StrataVariableList)
                {
                    if (!columnNames.Contains(stratVariableName.VarName, caseInsensitiveEqualityComparer) && !string.IsNullOrEmpty(stratVariableName.VarName.Trim()))
                    {
                        columnNames.Add(stratVariableName.VarName);
                    }
                }
            }

            AddSupplementaryColumnNames(columnNames);
            return GenerateView(inputs).ToTable("output", false, columnNames.ToArray());
        }

        /// <summary>
        /// Generates a .NET DataTable based off of the selection criteria and with only the specified columns
        /// </summary>
        /// <param name="columnNames">The list of columns to select</param>
        /// <param name="customFilter">A custom filter parameter</param>
        /// <returns>DataTable</returns>
        public DataTable GenerateTable(List<string> columnNames, string customFilter = "")
        {
            #region Input Validation
            if (columnNames.Count == 0)
            {
                throw new ApplicationException(SharedStrings.NO_COLUMNS_FOR_SELECT_QUERY);
            }
            #endregion // Input Validation

            GadgetParameters inputs = new GadgetParameters();
            MyString myString = new MyString();
            inputs.MainVariableName = string.Empty;
            inputs.MainVariableNames = new List<string>();
            inputs.ColumnNames = myString.ToListOfMyString(columnNames);
            inputs.CustomFilter = customFilter;
            inputs.CustomSortColumnName = string.Empty;
            inputs.StrataVariableNames = new List<string>();
            inputs.StrataVariableList = new List<MyString>();
            inputs.InputVariableList = new Dictionary<string, string>();

            string columnsToSelect = string.Empty;

            AddSupplementaryColumnNames(columnNames);

            return GenerateView(inputs).ToTable("output", false, columnNames.ToArray());
        }

        public DataTable GenerateTable(List<string> columnNames, GadgetParameters inputs, string customFilter = "")
        {
            string columnsToSelect = string.Empty;
            inputs.CustomFilter = customFilter;
            AddSupplementaryColumnNames(columnNames);
            EwavConstructTableColumnNames(inputs);
            DataView dv = GenerateView(inputs);

            if (inputs.GadgetFilters != null && inputs.GadgetFilters.Count > 0)
            {
                dv.RowFilter = CombineFilters(GenerateFilterCriteria(inputs.GadgetFilters), dv);
            }

            return dv.ToTable("output", false, columnNames.ToArray());
        }

        /// <summary>
        /// Generates a combined frequency table of all the drop-down list values in a single group fields
        /// </summary>
        /// <param name="groupVar">The group variable name</param>
        /// <param name="allListValues">Whether to show comment legal list values</param>
        /// <param name="sortHighToLow">Determines whether or not the frequency output is sorted from the highest value to the lowest value</param>
        /// <returns>A data table with the combined frequency</returns>
        public DataTable GenerateCombinedListFrequencyTable(string groupVar, bool sortHighToLow)
        {
            #region Input Validation
            if (string.IsNullOrEmpty(groupVar))
            {
                throw new ArgumentNullException("groupVar");
            }
            #endregion // Input Validation

            List<string> columnNames = new List<string>();
            GroupField groupField = null;

            foreach (DataRow row in FieldTable.Rows)
            {
                if (row["columnname"].ToString().Equals(groupVar))
                {
                    groupField = row["epifieldtype"] as GroupField;

                    foreach (string s in groupField.ChildFieldNameArray)
                    {
                        columnNames.Add(s);
                    }
                }
            }

            if (groupField == null)
            {
                return null;
            }

            DataView dv = GenerateView();

            if (dv == null || dv.Count == 0 || mainTable == null || mainTable.Rows.Count == 0)
            {
                return null;
            }

            DataTable combinedFrequencyTable = new DataTable();

            DataColumn col1 = new DataColumn("value", typeof(string));
            DataColumn col2 = new DataColumn("count", typeof(int));

            combinedFrequencyTable.Columns.Add(col1);
            combinedFrequencyTable.Columns.Add(col2);

            List<DDLFieldOfLegalValues> legalValueFields = new List<DDLFieldOfLegalValues>();
            List<DDLFieldOfCommentLegal> commentLegalFields = new List<DDLFieldOfCommentLegal>();

            string[] fieldNames = groupField.ChildFieldNameArray;

            foreach (string s in fieldNames)
            {
                Field field = GetAssociatedField(s);
                if (field != null && field is DDLFieldOfLegalValues)
                {
                    legalValueFields.Add(field as DDLFieldOfLegalValues);
                }
                else if (field != null && field is DDLFieldOfCommentLegal)
                {
                    commentLegalFields.Add(field as DDLFieldOfCommentLegal);
                }
            }

            List<string> values = new List<string>();

            if (legalValueFields.Count >= commentLegalFields.Count)
            {
                foreach (DDLFieldOfLegalValues legalValuesField in legalValueFields)
                {
                    List<string> fValues = GetDistinctValuesAsList(legalValuesField.Name);
                    foreach (string s in fValues)
                    {
                        if (!values.Contains(s))
                        {
                            values.Add(s);
                        }
                    }
                }
            }
            else
            {
                foreach (DDLFieldOfCommentLegal commentLegalField in commentLegalFields)
                {
                    List<string> fValues = GetDistinctValuesAsList(commentLegalField.Name);
                    foreach (string s in fValues)
                    {
                        if (!values.Contains(s))
                        {
                            values.Add(s);
                        }
                    }
                }
            }

            for (int i = 0; i < values.Count; i++)
            {
                string value = values[i];
                combinedFrequencyTable.Rows.Add(value, 0);
            }

            if (legalValueFields.Count >= commentLegalFields.Count)
            {
                foreach (DDLFieldOfLegalValues legalValuesField in legalValueFields)
                {
                    for (int i = 0; i < values.Count; i++)
                    {
                        string value = values[i];
                        double sum = Convert.ToDouble(mainTable.Compute(string.Format("count({0})", AddBracketsToString(legalValuesField.Name)), CombineFilters(string.Format("{0} = '{1}'", AddBracketsToString(legalValuesField.Name), value.Replace("'", "''")))));
                        combinedFrequencyTable.Rows[i][1] = (int)combinedFrequencyTable.Rows[i][1] + (int)sum;
                    }
                }
            }
            else
            {
                foreach (DDLFieldOfCommentLegal commentLegalField in commentLegalFields)
                {
                    for (int i = 0; i < values.Count; i++)
                    {
                        string value = values[i];
                        double sum = Convert.ToDouble(mainTable.Compute(string.Format("count({0})", AddBracketsToString(commentLegalField.Name)), CombineFilters(string.Format("{0} = '{1}'", AddBracketsToString(commentLegalField.Name), value.Replace("'", "''")))));
                        combinedFrequencyTable.Rows[i][1] = (int)combinedFrequencyTable.Rows[i][1] + (int)sum;
                    }
                }
            }

            if (sortHighToLow)
            {
                combinedFrequencyTable = SortHighToLow(combinedFrequencyTable, combinedFrequencyTable.Columns["count"]);
            }

            return combinedFrequencyTable;
        }

        /// <summary>
        /// Generates a combined frequency table of all the checked checkbox fields or 'Yes'-marked Yes/No fields in a single group fields
        /// </summary>
        /// <param name="groupVar">The group variable name</param>
        /// <param name="sortHighToLow">Determines whether or not the frequency output is sorted from the highest value to the lowest value</param>
        /// <returns>A data table with the combined frequency</returns>
        public DataTable GenerateCombinedBooleanFrequencyTable(string groupVar, bool sortHighToLow, GadgetParameters gadgetParameters, List<EwavRule_Base> rules = null)
        {
            #region Input Validation
            if (string.IsNullOrEmpty(groupVar))
            {
                throw new ArgumentNullException("groupVar");
            }
            #endregion // Input Validation

            List<string> columnNames = new List<string>();
            //GroupField groupField = null;

            if (rules != null)
            {
                EwavRule_GroupVariable GroupRule = null;
                if (rules.Any(r => r.VaraiableName == groupVar))
                {
                    GroupRule = (EwavRule_GroupVariable)rules.Single(r => r.VaraiableName == groupVar);
                    foreach (var item in GroupRule.Items)
                    {
                        columnNames.Add(item.VarName);
                    }
                }

                else
                {
                    columnNames.Add(groupVar);
                }

            }

            EwavConstructTableColumnNames(gadgetParameters);

            //foreach (DataRow row in FieldTable.Rows)
            //{
            //    if (row["columnname"].ToString().Equals(groupVar))
            //    {
            //        groupField = row["epifieldtype"] as GroupField;

            //        foreach (string s in groupField.ChildFieldNameArray)
            //        {
            //            columnNames.Add(s);
            //        }
            //    }
            //}

            //if (groupField == null)
            //{
            //    return null;
            //}

            if (columnNames.Count == 0)
            {
                return null;
            }

            DataView dv = GenerateView(gadgetParameters);

            if (dv == null || dv.Count == 0 || mainTable == null || mainTable.Rows.Count == 0)
            {
                return null;
            }

            DataTable combinedFrequencyTable = new DataTable();

            DataColumn col1 = new DataColumn("value", typeof(string));
            DataColumn col2 = new DataColumn("count", typeof(int));

            combinedFrequencyTable.Columns.Add(col1);
            combinedFrequencyTable.Columns.Add(col2);

            //List<CheckBoxField> checkboxFields = new List<CheckBoxField>();
            //List<YesNoField> yesNoFields = new List<YesNoField>();

            //string[] fieldNames = groupField.ChildFieldNameArray;

            //foreach (string s in fieldNames)
            //{
            //    Field field = GetAssociatedField(s);
            //    if (field != null && field is CheckBoxField)
            //    {
            //        checkboxFields.Add(field as CheckBoxField);
            //    }
            //    else if (field != null && field is YesNoField)
            //    {
            //        yesNoFields.Add(field as YesNoField);
            //    }
            //}

            //if (checkboxFields.Count == 0 && yesNoFields.Count == 0)
            //{
            //    return GenerateCombinedListFrequencyTable(groupVar, sortHighToLow);
            //}

            foreach (var item in columnNames)
            {
                double sum = Convert.ToDouble(mainTable.Compute(string.Format("count({0})", AddBracketsToString(item.ToString())), CombineFilters(string.Format("{0} = true", AddBracketsToString(item.ToString())))));
                combinedFrequencyTable.Rows.Add(item.ToString(), (int)sum);
            }
            combinedFrequencyTable.Rows.Add("Denominator", (int)dv.Count);
            //if (checkboxFields.Count == 0)
            //{
            //    foreach (YesNoField yesNoField in yesNoFields)
            //    {
            //        double sum = Convert.ToDouble(mainTable.Compute(string.Format("count({0})", AddBracketsToString(yesNoField.Name)), CombineFilters(string.Format("{0} = 1", AddBracketsToString(yesNoField.Name)))));
            //        combinedFrequencyTable.Rows.Add(yesNoField.PromptText, (int)sum);
            //    }
            //}

            if (sortHighToLow)
            {
                combinedFrequencyTable = SortHighToLow(combinedFrequencyTable, combinedFrequencyTable.Columns["count"]);
            }

            return combinedFrequencyTable;
        }

        /// <summary>
        /// Generates a combined frequency table of all fields in the group
        /// </summary>
        /// <param name="inputs">The inputs for this method</param>
        /// <param name="booleanOutput">Whether the results are of boolean fields</param>
        /// <returns>DataTable</returns>
        public DataTable GenerateCombinedFrequencyTable(EwavCombinedFrequencyGadgetParameters combinedParameters, string groupVar, ref bool booleanOutput, ref int fields, ref int denominator, GadgetParameters gadgetParameters, List<EwavRule_Base> rules = null)
        {
            #region Input Validation
            //if (inputs == null)
            //{
            //    throw new ArgumentNullException("inputs");
            //}
            //if (inputs.ColumnNames.Count == 0)
            //{
            //    throw new ArgumentNullException("inputs.ColumnNames");
            //}
            #endregion // Input Validation

            //string groupVar = inputs.MainVariableName;
            //bool includeMissing = inputs.IncludeMissing;
            //bool sortHighToLow = sortHighToLow;

            List<string> columnNames = new List<string>();
            //GroupField groupField = null;

            if (rules != null)
            {
                EwavRule_GroupVariable GroupRule = null;
                if (rules.Any(r => r.VaraiableName == groupVar))
                {
                    GroupRule = (EwavRule_GroupVariable)rules.Single(r => r.VaraiableName == groupVar && r.VaraiableDataType == "GroupVariable");
                    foreach (var item in GroupRule.Items)
                    {
                        columnNames.Add(item.VarName);
                    }
                }

                else
                {
                    columnNames.Add(groupVar);
                }

            }

            EwavConstructTableColumnNames(gadgetParameters);


            booleanOutput = false;
            string trueValue = string.Empty;
            //int combineMode = 0; // automatic

            List<string> fieldNames = new List<string>();

            foreach (string listFieldName in columnNames)
            {
                if (!fieldNames.Contains(listFieldName))
                {
                    //if (IsUsingEpiProject && GetGroupFieldsAsList().Contains(listFieldName, caseInsensitiveEqualityComparer))
                    //{
                    //    // add fields in a group
                    //    foreach (Field field in this.View.Fields)
                    //    {
                    //        if (field is GroupField && field.Name.Equals(listFieldName))
                    //        {
                    //            GroupField groupField = field as GroupField;
                    //            foreach (string s in groupField.ChildFieldNameArray)
                    //            {
                    //                if (!fieldNames.Contains(listFieldName))
                    //                {
                    //                    fieldNames.Add(s);
                    //                }
                    //            }
                    //            break;
                    //        }
                    //    }
                    //}
                    //else 
                    //if (GetGroupVariablesAsList().Contains(listFieldName, caseInsensitiveEqualityComparer))
                    //{
                    EwavRule_GroupVariable GroupRule = null;
                    if (rules.Any(r => r.VaraiableName == groupVar && r.VaraiableDataType == "GroupVariable"))
                    {
                        GroupRule = (EwavRule_GroupVariable)rules.Single(r => r.VaraiableName == groupVar);

                        foreach (var item in GroupRule.Items)
                        {
                            if (!fieldNames.Contains(item.VarName))
                            {
                                fieldNames.Add(item.VarName);
                            }

                        }

                        // add fields in a group
                        //foreach (string columnName in GetGroupVariable(listFieldName).Variables)
                        //{
                        //    if (!fieldNames.Contains(listFieldName))
                        //    {
                        //        fieldNames.Add(columnName);
                        //    }
                        //}
                    }
                    //else if (IsUsingEpiProject && listFieldName.ToLower().StartsWith("page "))
                    //{
                    //    // add fields on a specific page
                    //    int pageNumber = -1;

                    //    string strPageNumber = listFieldName.Remove(0, 5);

                    //    int.TryParse(strPageNumber, out pageNumber);

                    //    if (pageNumber < 0)
                    //    {
                    //        continue;
                    //    }

                    //    pageNumber--;
                    //    Page page = this.View.Pages[pageNumber];

                    //    foreach (Field field in page.Fields)
                    //    {
                    //        if (field is IDataField && field is RenderableField && !(field is GroupField) && !(field is GridField) && !(field is MirrorField))
                    //        {
                    //            if (!fieldNames.Contains(listFieldName))
                    //            {
                    //                fieldNames.Add(field.Name);
                    //            }
                    //        }
                    //    }
                    //}
                    else
                    {
                        // add a single field
                        if (!fieldNames.Contains(listFieldName))
                        {
                            fieldNames.Add(listFieldName);
                        }
                    }
                }
            }

            fields = fieldNames.Count;

            //if (GetGroupVariablesAsList().Contains(groupVar))
            //{
            //    fieldNames = GetGroupVariable(groupVar).Variables;
            //}
            //else if (GetGroupFieldsAsList().Contains(groupVar))
            //{
            //    fieldNames = GetVariablesInGroup(groupVar);
            //}

            trueValue = combinedParameters.TrueValue;
            CombineModeTypes combineMode = combinedParameters.CombineMode;


            //if (inputs.InputVariableList.ContainsKey("combinemode"))
            //{
            //    int.TryParse(inputs.InputVariableList["combinemode"], out combineMode);
            //}
            //if (inputs.InputVariableList.ContainsKey("truevalue"))
            //{
            //    trueValue = inputs.InputVariableList["truevalue"];
            //}

            DataView dv = GenerateView(gadgetParameters);

            if (dv == null || dv.Count == 0 || mainTable == null || mainTable.Rows.Count == 0)
            {
                return null;
            }

            DataTable combinedFrequencyTable = new DataTable();

            DataColumn col1 = new DataColumn("value", typeof(string));
            DataColumn col2 = new DataColumn("count", typeof(int));

            combinedFrequencyTable.Columns.Add(col1);
            combinedFrequencyTable.Columns.Add(col2);

            List<string> booleanFields = new List<string>();
            List<string> numericFields = new List<string>();
            List<string> otherFields = new List<string>();

            foreach (string s in fieldNames)
            {
                if (IsColumnBoolean(s) || IsColumnYesNo(s))
                {
                    booleanFields.Add(s);
                }
                else if (IsColumnText(s))
                {
                    otherFields.Add(s);
                }
                else if (IsColumnNumeric(s))
                {
                    numericFields.Add(s);
                }
            }

            DataView cmbDv = new DataView(mainTable, "", "", DataViewRowState.CurrentRows);
            cmbDv.RowFilter = mainTable.DefaultView.RowFilter;
            //if (!string.IsNullOrEmpty(inputs.CustomFilter))
            //{
            //    cmbDv.RowFilter = CombineFilters(inputs.CustomFilter);
            //}

            denominator = cmbDv.Count; //denomintor sent back to the client for further processing.

            string originalFilter = cmbDv.RowFilter;

            if (originalFilter == "")
            {
                originalFilter = "true = true";
            }

            string gadgetLevelFilter = string.Empty;
            if (gadgetParameters.GadgetFilters != null && gadgetParameters.GadgetFilters.Count > 0)
            {
                gadgetLevelFilter = GenerateFilterCriteria(gadgetParameters.GadgetFilters);
            }
            else
            {
                gadgetLevelFilter = "true = true";
            }
            

            DataTable cmbDt = cmbDv.ToTable();
            cmbDv = new DataView(cmbDt,
                "(" + originalFilter + ") " + " and " + "(" + gadgetLevelFilter + ")",
                "", DataViewRowState.CurrentRows);

            originalFilter = "(" + originalFilter + ") " + " and " + "(" + gadgetLevelFilter + ")";

            if (booleanFields.Count == 0 && otherFields.Count > 0 && otherFields.Count > numericFields.Count)
            {
                if (combineMode == CombineModeTypes.Automatic || combineMode == CombineModeTypes.Categorical)
                {
                    List<string> values = new List<string>();

                    foreach (string otherField in otherFields)
                    {
                        List<string> fieldValues = GetDistinctValuesAsList(otherField);
                        foreach (string s in fieldValues)
                        {
                            if (!values.Contains(s))
                            {
                                values.Add(s);
                            }
                        }
                    }

                    if (values.Contains(""))
                    {
                        values.Remove("");
                        //if (includeMissing)
                        //{
                        //    values.Add("");
                        //}
                    }

                    if (values.Count == 0)
                    {
                        throw new ApplicationException(DashboardSharedStrings.ERROR_ZERO_VALUES_COMBINED_FREQ);
                    }
                    else if (values.Count > Combined_Frequency_Row_Limit)
                    {
                        string exMsg = string.Format(DashboardSharedStrings.ERROR_TOO_MANY_VALUES_COMBINED_FREQ_EXT, values.Count, Combined_Frequency_Row_Limit);
                        throw new ApplicationException(exMsg);
                    }

                    for (int i = 0; i < values.Count; i++)
                    {
                        combinedFrequencyTable.Rows.Add(values[i], 0);
                    }

                    foreach (string otherField in otherFields)
                    {
                        for (int i = 0; i < values.Count; i++)
                        {
                            cmbDv.RowFilter = originalFilter;
                            string value = values[i];
                            double sum = 0;
                            if (string.IsNullOrEmpty(value))
                            {
                                cmbDv.RowFilter = CombineFilters(AddBracketsToString(otherField) + " is null", cmbDv);
                                sum = cmbDv.Count;
                            }
                            else
                            {
                                cmbDv.RowFilter = CombineFilters(AddBracketsToString(otherField) + " = '" + value.Replace("'", "''") + "'", cmbDv);
                                sum = cmbDv.Count;
                            }
                            combinedFrequencyTable.Rows[i][1] = (int)combinedFrequencyTable.Rows[i][1] + (int)sum;
                        }
                    }
                }
                else if (combineMode == CombineModeTypes.Boolean)
                {
                    foreach (string otherField in otherFields)
                    {
                        cmbDv.RowFilter = originalFilter;

                        cmbDv.RowFilter = CombineFilters(AddBracketsToString(otherField) + " = '" + trueValue.Replace("'", "''") + "'", cmbDv);
                        double sum = cmbDv.Count;

                        Field field = GetAssociatedField(otherField);
                        if (field != null && field is CheckBoxField)
                        {
                            combinedFrequencyTable.Rows.Add((field as CheckBoxField).PromptText, (int)sum);
                        }
                        else
                        {
                            combinedFrequencyTable.Rows.Add(otherField, (int)sum);
                        }
                    }
                    booleanOutput = true;
                }
            }
            else if (booleanFields.Count > 0)
            {
                foreach (string booleanField in booleanFields)
                {
                    cmbDv.RowFilter = originalFilter;

                    cmbDv.RowFilter = CombineFilters(AddBracketsToString(booleanField) + " = true", cmbDv);
                    double sum = cmbDv.Count;

                    Field field = GetAssociatedField(booleanField);
                    if (field != null && field is CheckBoxField)
                    {
                        combinedFrequencyTable.Rows.Add((field as CheckBoxField).PromptText, (int)sum);
                    }
                    else
                    {
                        combinedFrequencyTable.Rows.Add(booleanField, (int)sum);
                    }
                }
                booleanOutput = true;
            }
            else if (numericFields.Count > 0)
            {
                if (combineMode == CombineModeTypes.Boolean)
                {
                    foreach (string numberField in numericFields)
                    {
                        cmbDv.RowFilter = originalFilter;

                        cmbDv.RowFilter = CombineFilters(AddBracketsToString(numberField) + " = " + trueValue, cmbDv);
                        double sum = cmbDv.Count;

                        Field field = GetAssociatedField(numberField);
                        if (field != null && field is NumberField)
                        {
                            combinedFrequencyTable.Rows.Add((field as NumberField).PromptText, (int)sum);
                        }
                        else
                        {
                            combinedFrequencyTable.Rows.Add(numberField, (int)sum);
                        }
                    }
                    booleanOutput = true;
                }
                else
                {
                    List<string> values = new List<string>();
                    foreach (string numberField in numericFields)
                    {
                        List<string> fieldValues = GetDistinctValuesAsList(numberField);
                        foreach (string s in fieldValues)
                        {
                            if (!values.Contains(s))
                            {
                                values.Add(s);
                            }
                        }
                    }

                    if (values.Contains(""))
                    {
                        values.Remove("");
                        //if (includeMissing)
                        //{
                        //    values.Add("");
                        //}
                    }

                    if (values.Count == 0)
                    {
                        throw new ApplicationException(DashboardSharedStrings.ERROR_ZERO_VALUES_COMBINED_FREQ);
                    }
                    else if (values.Count > Combined_Frequency_Row_Limit)
                    {
                        string exMsg = string.Format(DashboardSharedStrings.ERROR_TOO_MANY_VALUES_COMBINED_FREQ_EXT, values.Count, Combined_Frequency_Row_Limit);
                        throw new ApplicationException(exMsg);
                    }

                    for (int i = 0; i < values.Count; i++)
                    {
                        combinedFrequencyTable.Rows.Add(values[i], 0);
                    }

                    foreach (string numberField in numericFields)
                    {
                        for (int i = 0; i < values.Count; i++)
                        {
                            cmbDv.RowFilter = originalFilter;

                            string value = values[i];
                            double sum = 0;
                            if (string.IsNullOrEmpty(value))
                            {
                                cmbDv.RowFilter = CombineFilters(AddBracketsToString(numberField) + " is null", cmbDv);
                                sum = cmbDv.Count;
                            }
                            else
                            {
                                cmbDv.RowFilter = CombineFilters(AddBracketsToString(numberField) + " = " + value + "", cmbDv);
                                sum = cmbDv.Count;
                            }
                            combinedFrequencyTable.Rows[i][1] = (int)combinedFrequencyTable.Rows[i][1] + (int)sum;
                        }
                    }
                }
            }

            if (combinedFrequencyTable == null || combinedFrequencyTable.Rows.Count == 0)
            {
                throw new ApplicationException(DashboardSharedStrings.ERROR_ZERO_VALUES_COMBINED_FREQ_EXT);
            }

            if (combinedParameters.SortHighToLow)
            {
                combinedFrequencyTable = SortHighToLow(combinedFrequencyTable, combinedFrequencyTable.Columns["count"]);
            }

            foreach (DataRow row in combinedFrequencyTable.Rows)
            {
                if (string.IsNullOrEmpty(row[0].ToString()))
                {
                    row[0] = this.Config.Settings.RepresentationOfMissing;
                    break;
                }
            }

            return combinedFrequencyTable;
        }

        /// <summary>
        /// Generates a grouped line listing .NET data table based off of the given columns, sort order, and group fields
        /// </summary>
        /// <param name="inputs">The inputs needed to process the frequency</param>
        /// <returns>A dictionary of data tables, one for each value of the stratification variable, and that table's associated count</returns>
        public List<DataTable> GenerateLineList(GadgetParameters inputs, List<EwavRule_Base> rules = null)
        {
            try
            {
                inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_CREATING_VARIABLES);

                List<string> columnsToSelect = new List<string>();
                List<string> originalColumnsToSelect = new List<string>();

                string sortOrder = string.Empty;
                int maxRows = 200;

                //if (inputs.StrataVariableNames != null && inputs.StrataVariableNames.Count > 0)
                //{
                //    if (!columnsToSelect.Contains(inputs.StrataVariableNames[0]))
                //    {
                //        columnsToSelect.Add(inputs.StrataVariableNames[0]);
                //    }
                //}
                EwavConstructTableColumnNames(inputs);

                if (inputs.StrataVariableList != null && inputs.StrataVariableList.Count > 0)
                {
                    if (!columnsToSelect.Contains(inputs.StrataVariableList[0].VarName))
                    {
                        columnsToSelect.Add(inputs.StrataVariableList[0].VarName);
                    }
                }

                foreach (KeyValuePair<string, string> kvp in inputs.InputVariableList)
                {
                    if (kvp.Value.ToLower().Equals("listfield") && !columnsToSelect.Contains(kvp.Key))
                    {
                        if (rules != null && GetGroupFieldsAsList(rules).Contains(kvp.Key, caseInsensitiveEqualityComparer))
                        {
                            // add fields in a group
                            //foreach (Field field in this.View.Fields)
                            //{
                            //if (field is GroupField && field.Name.Equals(kvp.Key))
                            //{
                            //    GroupField groupField = field as GroupField;
                            EwavRule_GroupVariable GroupVar = (EwavRule_GroupVariable)rules.Single(r => r.VaraiableName == kvp.Key);
                            foreach (var item in GroupVar.Items)
                            {
                                if (!columnsToSelect.Contains(kvp.Key))
                                {
                                    columnsToSelect.Add(item.VarName);
                                    originalColumnsToSelect.Add(item.VarName);
                                }
                            }
                            //foreach (string s in groupField.ChildFieldNameArray)
                            //{

                            //}
                            //  break;
                            //}
                            //}
                        }
                        else if (IsUsingEpiProject && kvp.Key.ToLower().StartsWith("page "))
                        {
                            // add fields on a specific page
                            int pageNumber = -1;

                            string strPageNumber = kvp.Key.Remove(0, 5);

                            int.TryParse(strPageNumber, out pageNumber);

                            if (pageNumber < 0)
                            {
                                continue;
                            }

                            pageNumber--;
                            Page page = this.View.Pages[pageNumber];

                            foreach (Field field in page.Fields)
                            {
                                if (field is IDataField && field is RenderableField && !(field is GroupField) && !(field is GridField) && !(field is MirrorField))
                                {
                                    if (!columnsToSelect.Contains(kvp.Key))
                                    {
                                        columnsToSelect.Add(field.Name);
                                        originalColumnsToSelect.Add(field.Name);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // add a single fields
                            if (!columnsToSelect.Contains(kvp.Key))
                            {
                                columnsToSelect.Add(kvp.Key);
                                originalColumnsToSelect.Add(kvp.Key);
                            }
                        }
                    }
                }

                foreach (KeyValuePair<string, string> kvp in inputs.InputVariableList)
                {
                    if (kvp.Value.ToLower().Equals("sortfield"))
                    {
                        sortOrder = string.Format("{0}{1},", sortOrder, kvp.Key);

                        string sortFieldName = kvp.Key;

                        if (sortFieldName.ToLower().EndsWith("desc"))
                        {
                            sortFieldName = sortFieldName.Remove(sortFieldName.Length - 5);
                        }
                        else
                        {
                            sortFieldName = sortFieldName.Remove(sortFieldName.Length - 4);
                        }

                        sortFieldName = sortFieldName.TrimStart('[').TrimEnd(']');

                        if (!columnsToSelect.Contains(sortFieldName))
                        {
                            columnsToSelect.Add(sortFieldName);
                        }
                    }
                }

                //foreach (FilterCondition condition in this.DataFilters)
                //{
                //    if (!columnsToSelect.Contains(condition.RawColumnName))
                //    {
                //        columnsToSelect.Add(condition.RawColumnName);
                //    }
                //}

                if (inputs.InputVariableList.ContainsKey("maxrows"))
                {
                    maxRows = int.Parse(inputs.InputVariableList["maxrows"]);
                }

                sortOrder = sortOrder.TrimEnd(',');
                inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_GENERATING_TABLE);

                //PopulateDataSet(inputs);


                DataView dvAllRows = GenerateView(inputs); //new DataView(mainTable);
                //dvAllRows.RowFilter = mainTable.DefaultView.RowFilter;
                dvAllRows.Sort = sortOrder;

                RecordCount = dvAllRows.Count;// mainTable.DefaultView.Count;

                if (inputs.GadgetFilters != null && inputs.GadgetFilters.Count > 0)
                {
                    dvAllRows.RowFilter = CombineFilters(GenerateFilterCriteria(inputs.GadgetFilters), dvAllRows);
                }

                // Grouped output; use one table for each strata value
                //if (inputs.StrataVariableNames != null && inputs.StrataVariableNames.Count > 0)
                if (inputs.StrataVariableList != null && inputs.StrataVariableList.Count > 0)
                {
                    List<DataTable> tables = new List<DataTable>();
                    List<string> strataValues = new List<string>();
                    string strataVariableName = inputs.StrataVariableList[0].VarName;

                    inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_FINDING_STRATA_VALUES);
                    for (int i = 0; i < dvAllRows.Count; i++)
                    {
                        DataRow dataRow = dvAllRows[i].Row;
                        string strataValue = dataRow[strataVariableName].ToString().Trim();
                        if (!strataValues.Contains(strataValue, caseInsensitiveEqualityComparer))
                        {
                            strataValues.Add(strataValue);
                        }
                    }

                    foreach (string strataValue in strataValues)
                    {
                        DataView strataDv = new DataView(mainTable);
                        strataDv.Sort = dvAllRows.Sort;
                        strataDv.RowFilter = dvAllRows.RowFilter;

                        DataTable dt = new DataTable();//dtAllRows.Clone();

                        string strataFilter = string.Empty;
                        string fullFilter = string.Empty;

                        if (!string.IsNullOrEmpty(strataValue))
                        {
                            string columnType = GetColumnType(strataVariableName);

                            strataFilter = string.Format("{0}{1}{2}{3}{4}{5}{6}", stringLiterals.LEFT_SQUARE_BRACKET, strataVariableName, stringLiterals.RIGHT_SQUARE_BRACKET, stringLiterals.SPACE, stringLiterals.EQUAL, stringLiterals.SPACE, FormatValue(strataValue, columnType));
                        }
                        else
                        {
                            strataFilter = string.Format("{0}{1}{2}{3} is null", stringLiterals.LEFT_SQUARE_BRACKET, strataVariableName, stringLiterals.RIGHT_SQUARE_BRACKET, stringLiterals.SPACE);
                        }

                        if (!string.IsNullOrEmpty(dvAllRows.RowFilter))
                        {
                            fullFilter = string.Format("{0} and {1}", strataFilter, dvAllRows.RowFilter);
                        }
                        else
                        {
                            fullFilter = strataFilter;
                        }

                        strataDv.RowFilter = fullFilter;

                        try
                        {
                            dt = strataDv.ToTable(mainTable.TableName);
                        }
                        catch (InvalidOperationException)
                        {
                            continue;
                        }

                        List<DataColumn> columnsToRemove = new List<DataColumn>();

                        foreach (DataColumn column in dt.Columns)
                        {
                            bool found = false;

                            foreach (string s in originalColumnsToSelect)
                            {
                                if (column.ColumnName.ToLower().Equals(s.ToLower()))
                                {
                                    found = true;
                                }
                            }

                            if (!found)
                            {
                                columnsToRemove.Add(column);
                            }
                        }

                        foreach (DataColumn dc in columnsToRemove)
                        {
                            dt.Columns.Remove(dc);
                        }
                        dt.TableName = strataFilter;
                        tables.Add(dt);
                    }

                    return tables;
                }
                // Non-grouped output; only one line list table will be produced
                else
                {
                    DataTable dt = dvAllRows.ToTable();

                    List<DataColumn> columnsToRemove = new List<DataColumn>();

                    foreach (DataColumn column in dt.Columns)
                    {
                        bool found = false;

                        foreach (string s in originalColumnsToSelect)
                        {
                            if (column.ColumnName.ToLower().Equals(s.ToLower()))
                            {
                                found = true;
                            }
                        }

                        if (!found)
                        {
                            columnsToRemove.Add(column);
                        }
                    }

                    foreach (DataColumn dc in columnsToRemove)
                    {
                        dt.Columns.Remove(dc);
                    }

                    bool sortByTabOrder = false;
                    if (inputs.InputVariableList.ContainsKey("sortcolumnsbytaborder"))
                    {
                        sortByTabOrder = bool.Parse(inputs.InputVariableList["sortcolumnsbytaborder"]);
                        OrderColumns(dt, sortByTabOrder);
                    }

                    if (inputs.ShouldShowCommentLegalLabels)
                    {
                        AddOptionLabelsToListTable(dt);
                        AddCommentLegalLabelsToListTable(dt);
                    }

                    List<DataTable> tables = new List<DataTable>();
                    tables.Add(dt);

                    return tables;
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Combines a custom filter with the existing canvas-level data filter 
        /// </summary>        
        /// <param name="additionalFilter">The filter to add</param>
        /// <param name="dv">The data view whose row filter should be combined with</param>
        /// <returns>String represeting the current row filter and the additional filter</returns>
        private string CombineFilters(string additionalFilter, DataView dv = null)
        {
            string rowFilter = DataSet.Tables[0].DefaultView.RowFilter;

            if (dv != null)
            {
                rowFilter = dv.RowFilter;
            }

            if (string.IsNullOrEmpty(rowFilter))
            {
                return additionalFilter;
            }
            else
            {
                return string.Format("({0}) and ({1})", rowFilter, additionalFilter);
            }
        }

        public Dictionary<DataTable, List<DescriptiveStatistics>> GenerateFrequencyTable(GadgetParameters gadgetParameters, string DatasourceName, string TableName)
        {
            Dictionary<DataTable, List<DescriptiveStatistics>> dict = GenerateFrequencyTable(gadgetParameters);

            return dict;
        }

        /// <summary>
        /// Gets a list of all the group fields on the form as a list of strings
        /// </summary>        
        /// <returns>List of strings</returns>
        public List<string> GetGroupFieldsAsList()
        {
            List<string> groupFields = new List<string>();

            if (IsUsingEpiProject)
            {
                foreach (Field field in this.View.Fields)
                {
                    if (field is GroupField)
                    {
                        groupFields.Add(field.Name);
                    }
                }
                groupFields.Sort();
            }

            return groupFields;
        }

        /// <summary>
        /// Gets a list of all the user-defined group variables as a list of strings
        /// </summary>        
        /// <returns>List of strings</returns>
        public List<string> GetGroupVariablesAsList()
        {
            List<string> groupVariables = new List<string>();

            foreach (EwavRule_Base rule in _dashboardRulesTable)
            {
                if (rule is EwavRule_GroupVariable)
                {
                    groupVariables.Add((rule as EwavRule_GroupVariable).VaraiableName);
                }
            }

            return groupVariables;
        }

        /// <summary>
        /// Gets a list of both the user-defined group variables and Group Fields on the form as a list of strings
        /// </summary>        
        /// <returns>List of strings</returns>
        public List<string> GetAllGroupsAsList()
        {
            List<string> groupNames = GetGroupFieldsAsList();
            groupNames.AddRange(GetGroupVariablesAsList());

            return groupNames;
        }

        /// <summary>
        /// Generates a weighted and multiple-stratified .NET frequency table based off of the selection criteria
        /// </summary>
        /// <param name="inputs">The inputs needed to process the frequency</param>
        /// <returns>A dictionary of data tables, one for each value of the stratification variable, and that table's associated count</returns>
        public Dictionary<DataTable, List<DescriptiveStatistics>> GenerateFrequencyTable(GadgetParameters inputs)
        {
            inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_CHECKING_INPUTS);

            #region Input Validation
            if (inputs == null)
            {
                throw new ArgumentNullException("inputs");
            }
            if (string.IsNullOrEmpty(inputs.MainVariableName))
            {
                throw new ArgumentNullException("inputs.MainVariableName");
            }
            #endregion // Input Validation

            this.UseAdvancedUserDataFilter = inputs.UseAdvancedDataFilter;

            if (UseAdvancedUserDataFilter == true)
            {
                this.AdvancedUserDataFilter = inputs.AdvancedDataFilterText;
            }

            string freqVar = inputs.MainVariableName;
            string crosstabVar = inputs.CrosstabVariableName;
            string weightVar = inputs.WeightVariableName;
            bool includeMissing = inputs.ShouldIncludeMissing;
            bool sortHighToLow = inputs.ShouldSortHighToLow;
            bool useAllPossibleValues = inputs.ShouldUseAllPossibleValues;
            bool includeFullSummaryStatistics = inputs.ShouldIncludeFullSummaryStatistics;
            //List<string> strataVars = inputs.StrataVariableNames;
            List<MyString> strataVars = inputs.StrataVariableList;
            bool needsOutputTable = true;

            if (inputs.InputVariableList.ContainsKey("NeedsOutputGrid"))
            {
                if (inputs.InputVariableList["NeedsOutputGrid"] == "false") needsOutputTable = false;
            }
            //CollectionV
            inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_CREATING_VARIABLES);

            bool doStratification = true;
            bool doCrossTab = true;
            bool doMeans = false;
            bool valuesAreBool = false;

            EwavConstructTableColumnNames(inputs);

            List<string> columnNames = new List<string>();
            if (IsUserDefinedColumn(freqVar) == false)
            {
                columnNames.Add(freqVar);
            }

            if (strataVars == null || strataVars.Count == 0)
            {
                doStratification = false;
            }
            else
            {
                foreach (MyString str in strataVars)
                {
                    if (IsUserDefinedColumn(str.VarName) == false)
                    {
                        columnNames.Add(str.VarName);
                    }
                }
            }

            if (string.IsNullOrEmpty(crosstabVar.Trim()))
            {
                doCrossTab = false;
            }
            else if (IsUserDefinedColumn(crosstabVar) == false)
            {
                columnNames.Add(crosstabVar);
            }

            if (!string.IsNullOrEmpty(weightVar) && IsUserDefinedColumn(weightVar) == false)
            {
                columnNames.Add(weightVar);
            }

            if (inputs.IsRequestCancelled())
            {
                return null;
            }

            if (!string.IsNullOrEmpty(inputs.CustomFilter))
            {
                ColumnDataType columnDataType = ColumnDataType.Boolean 
                    | ColumnDataType.Numeric 
                    | ColumnDataType.Text 
                    | ColumnDataType.UserDefined;

                List<string> dataColumnNames = GetFieldsAsList(columnDataType);
                foreach (string name in dataColumnNames)
                {
                    if (inputs.CustomFilter.ToLower().Contains(name.ToLower()) && !columnNames.Contains(name, caseInsensitiveEqualityComparer))
                    {
                        columnNames.Add(name);
                    }
                }
            }

            Dictionary<DataTable, List<DescriptiveStatistics>> stratifiedFrequencyTables = new Dictionary<DataTable, List<DescriptiveStatistics>>();

            inputs.CustomSortColumnName = freqVar;
            // WPF_TO_SL  pass rules as a parameter   
            // also pass table name as param   
            foreach (IDashboardRule rule in this.Rules)
            {
                if (rule is Rule_Format && (((Rule_Format)rule).FormatType.Equals(FormatTypes.RegularDate) || ((Rule_Format)rule).FormatType.Equals(FormatTypes.LongDate) || ((Rule_Format)rule).FormatType.Equals(FormatTypes.MonthAndFourDigitYear)) && ((Rule_Format)rule).DestinationColumnName.Equals(freqVar))
                {
                    inputs.CustomSortColumnName = ((Rule_Format)rule).SourceColumnName;
                    break;
                }
            }

            foreach (var item in columnNames)
            {
                List<string> ls = new List<string>();
                MyString ms = new MyString();
                ls = ms.ToListOfString(inputs.ColumnNames);
                if (!ls.Contains(item))
                {
                    inputs.ColumnNames.Add(new MyString(item));
                }
            }

            //inputs.ColumnNames.AddRange(columnNames);

            inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_GENERATING_TABLE);

            DataView dv = GenerateView(inputs);

            if (inputs.GadgetFilters != null && inputs.GadgetFilters.Count > 0)
            {
                dv.RowFilter = CombineFilters(GenerateFilterCriteria(inputs.GadgetFilters), dv);
            }


            int overallRecordCount = 0;

            if (dv != null && dv.Count > 0)
            {
                overallRecordCount = dv.Count;
            }
            else
            {
                return stratifiedFrequencyTables;
            }

            string columnType = string.Empty;
            if (mainTable.Columns[freqVar] != null)
            {
                columnType = mainTable.Columns[freqVar].DataType.ToString();
            }
            else
            {
                return stratifiedFrequencyTables;
            }

            string crosstabColumnType = string.Empty;
            if (mainTable.Columns[crosstabVar] != null)
            {
                crosstabColumnType = mainTable.Columns[crosstabVar].DataType.ToString();
            }

            if (columnType.Equals("System.Boolean"))
            {
                valuesAreBool = true;
            }
            else if (columnType.Equals("System.Int16") ||
                     columnType.Equals("System.Int32") ||
                     columnType.Equals("System.Int64") ||
                     columnType.Equals("System.Double") ||
                     columnType.Equals("System.Single") ||
                     columnType.Equals("System.Decimal") ||
                     columnType.Equals("System.Byte"))
            {
                doMeans = true;
            }

            inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_FINDING_STRATA_VALUES);
            // Select distinct values for stratification            
            Dictionary<string, string> stratas = new Dictionary<string, string>();
            DataTable distinctTable = new DataTable();
            DataTable sortedDistinctTable = new DataTable();

            if (doStratification)
            {
                stratas = GetStrataValuesAsDictionary(strataVars, includeMissing);
            }
            else
            {
                stratas.Add(freqVar, freqVar);
            }

            inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_FINDING_FREQ_VALUES);

            // Select distinct values for the main variable            
            List<string> freqValues = new List<string>();

            if (!freqVar.Equals(inputs.CustomSortColumnName))
            {
                distinctTable = SelectDistinct(inputs.CustomSortColumnName, dv, freqVar); // this needs to be checked
            }
            else
            {
                distinctTable = SelectDistinct(dv, freqVar);
            }

            if (inputs.IsRequestCancelled())
            {
                return null;
            }

            inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_SORTING_FREQ_VALUES);
            sortedDistinctTable = SortBySingleColumn(distinctTable, distinctTable.Columns[freqVar]);
            inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_COMPARING_FREQ_VALUES);

            int count = 0;
            int totalCount = sortedDistinctTable.Rows.Count;
            foreach (DataRow row in sortedDistinctTable.Rows)
            {
                // WARNING: Values in sortedDistinctTable are case-sensitive, e.g. "EE" is treated as different than "ee".
                // However, the .NET DataTable SELECT and COMPUTE methods treat both values as the same. Hence we must
                // ignore case to ensure we don't end up double-counting some of the values.
                if (!freqValues.Contains(row[0].ToString().TrimEnd(), caseInsensitiveEqualityComparer))
                {
                    if (inputs.IsRequestCancelled())
                    {
                        return null;
                    }

                    freqValues.Add(row[0].ToString().TrimEnd());
                }

                count++;

                if (count % 500 == 0)
                {
                    string statusMessage = string.Format(SharedStrings.DASHBOARD_GADGET_STATUS_COMPARED_FREQ_VALUES, count.ToString("N0"), sortedDistinctTable.Rows.Count.ToString("N0"));
                    inputs.UpdateGadgetStatus(statusMessage);
                }
            }

            if (inputs.InputVariableList.ContainsKey("aberration") && inputs.InputVariableList["aberration"].Equals("true") && freqValues.Count > ABERRATION_ROW_LIMIT)//!inputs.ShouldIgnoreRowLimits && 
            {
                string exMessage = string.Format(DashboardSharedStrings.ERROR_TOO_MANY_VALUES, freqVar, freqValues.Count, ABERRATION_ROW_LIMIT);
                throw new ApplicationException(exMessage);
            }
            else if (!inputs.InputVariableList.ContainsKey("aberration") && freqValues.Count > FREQUENCY_ROW_LIMIT)//
            {
                string exMessage = string.Format(DashboardSharedStrings.ERROR_TOO_MANY_VALUES, freqVar, freqValues.Count, FREQUENCY_ROW_LIMIT);
                throw new ApplicationException(exMessage);
            }

            if (!needsOutputTable)
            {
                string temp = freqValues[0];
                freqValues.Clear();
                freqValues.Add(temp);
            }
            if (includeMissing)
            {
                if (freqValues.Contains(string.Empty))
                {
                    freqValues.Remove(string.Empty);
                    freqValues.Add(string.Empty);
                }
            }

            Field field = null;

            foreach (DataRow fieldRow in FieldTable.Rows)
            {
                if (fieldRow["columnname"].Equals(freqVar))
                {
                    if (fieldRow["epifieldtype"] is Field)
                    {
                        field = fieldRow["epifieldtype"] as Field;
                    }
                    break;
                }
            }

            if (!valuesAreBool && field != null && field is YesNoField)
            {
                // special case as Epi 7 projects may implement Yes/No fields in such a way that they get read in as Int16s and therefore aren't caught above
                valuesAreBool = true;
                doMeans = true;
            }

            if (useAllPossibleValues && field != null)
            {
                inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_FINDING_LIST_VALUES);
                //fields = EwavView.Fields[freqVar];

                if (field is DDLFieldOfLegalValues)
                {
                    DataTable dataTable = ((TableBasedDropDownField)field).GetSourceData();
                    Dictionary<string, string> fieldValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    foreach (System.Data.DataRow row in dataTable.Rows)
                    {
                        if (inputs.IsRequestCancelled())
                        {
                            return null;
                        }

                        if (!string.IsNullOrEmpty(((TableBasedDropDownField)field).CodeColumnName.Trim()))
                        {
                            string key = row[((TableBasedDropDownField)field).CodeColumnName.Trim()].ToString();
                            if (!freqValues.Contains(key))
                            {
                                freqValues.Add(key);
                            }
                        }
                    }
                }
                else if (field is DDLFieldOfCommentLegal)
                {
                    DataTable dataTable = ((TableBasedDropDownField)field).GetSourceData();
                    Dictionary<string, string> fieldValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    foreach (System.Data.DataRow row in dataTable.Rows)
                    {
                        if (inputs.IsRequestCancelled())
                        {
                            return null;
                        }

                        if (!string.IsNullOrEmpty(((TableBasedDropDownField)field).TextColumnName.Trim()))
                        {
                            string key = row[((TableBasedDropDownField)field).TextColumnName.Trim()].ToString();

                            int dash = key.IndexOf('-');
                            string newKey = key.Substring(0, dash);

                            if (!freqValues.Contains(newKey))
                            {
                                freqValues.Add(newKey);
                            }
                        }
                    }
                }
                else if (field is DDLFieldOfCodes)
                {
                    DataTable dataTable = ((TableBasedDropDownField)field).GetSourceData();
                    Dictionary<string, string> fieldValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    foreach (System.Data.DataRow row in dataTable.Rows)
                    {
                        if (inputs.IsRequestCancelled())
                        {
                            return null;
                        }

                        if (!string.IsNullOrEmpty(((TableBasedDropDownField)field).TextColumnName.Trim()))
                        {
                            string key = row[((TableBasedDropDownField)field).TextColumnName.Trim()].ToString();

                            if (!freqValues.Contains(key))
                            {
                                freqValues.Add(key);
                            }
                        }
                    }
                }
            }
            else if (useAllPossibleValues && IsUserDefinedColumn(freqVar))
            {
                List<IDashboardRule> rules = Rules.GetRules(freqVar);
                foreach (IDashboardRule rule in rules)
                {
                    if (rule is Rule_Recode)
                    {
                        Rule_Recode recodeRule = rule as Rule_Recode;
                        foreach (string s in recodeRule.GetToValues())
                        {
                            if (!freqValues.Contains(s))
                            {
                                freqValues.Add(s);
                            }
                        }
                    }
                }
            }

            Dictionary<string, string> crosstabValues = new Dictionary<string, string>();
            if (doCrossTab)
            {
                inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_FINDING_CROSSTAB_VALUES);
                crosstabValues = GetCrosstabValuesAsDictionary(crosstabVar, freqVar, includeMissing);

                if (crosstabValues.Count > FREQUENCY_CROSSTAB_LIMIT) // !inputs.ShouldIgnoreRowLimits &&
                {
                    string exMessage = string.Format(DashboardSharedStrings.ERROR_TOO_MANY_CROSSTAB_VALUES, crosstabVar, crosstabValues.Count, FREQUENCY_CROSSTAB_LIMIT);
                    throw new ApplicationException(exMessage);
                }
            }

            inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_CALCULATING_TOTALS);
            foreach (KeyValuePair<string, string> strataKvp in stratas)
            {
                if (inputs.IsRequestCancelled())
                {
                    return null;
                }

                double sum = -1;
                double totalSum = 0;
                DataTable freqTable = new DataTable(strataKvp.Value);

                if (valuesAreBool)
                {
                    freqTable.Columns.Add(freqVar, typeof(string));
                }
                else
                {
                    freqTable.Columns.Add(freqVar, mainTable.Columns[freqVar].DataType);
                }
                freqTable.Columns.Add("freq", typeof(double));

                string nullColumnName = string.Empty;

                foreach (string s in freqValues)
                {
                    if (inputs.IsRequestCancelled())
                    {
                        return null;
                    }

                    string value = s;
                    string crosstabValue = string.Empty;

                    string filter = string.Empty;
                    string operand = " = ";

                    if (doStratification)
                    {
                        filter = strataKvp.Key;
                    }

                    value = FormatValue(value, columnType);

                    if (string.IsNullOrEmpty(s) && (includeMissing))
                    {
                        value = "is null";
                        operand = string.Empty;
                    }
                    else if (string.IsNullOrEmpty(s))
                    {
                        continue;
                    }

                    if (columnType.Equals("System.DateTime") && !string.IsNullOrWhiteSpace(s))
                    {
                        //string dateString = value;
                        //var culture = CultureInfo.CurrentCulture;
                        //var styles = DateTimeStyles.None;
                        //DateTime dateResult;
                        //if (DateTime.TryParse(dateString, culture, styles, out dateResult))
                        //{
                        DateTime dateValue = DateTime.Parse(value.Trim('#'), System.Globalization.CultureInfo.CurrentCulture);
                        value = string.Format("#{0}#", dateValue.ToString("MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));
                        //}
                        //else
                        //{
                        //    value = "";
                        //}
                    }

                    if (doStratification)
                    {
                        if (columnType.Equals("System.String") && value.ToLower().Equals("is null"))
                        {
                            filter = string.Format("{0} and ({1}{2}= '' or {3}{4}{5}{6})", filter, AddBracketsToString(freqVar), stringLiterals.SPACE, AddBracketsToString(freqVar), stringLiterals.SPACE, operand, value);
                        }
                        else if (columnType.Equals("System.Single"))
                        {
                            filter = string.Format(CultureInfo.InvariantCulture, "{0} and {1}{2}{3}{4}", filter, AddBracketsToString(freqVar), stringLiterals.SPACE, operand, value.Replace(',', '.'));
                        }
                        else
                        {
                            filter = string.Format("{0} and {1}{2}{3}{4}", filter, AddBracketsToString(freqVar), stringLiterals.SPACE, operand, value);
                        }
                    }
                    else
                    {
                        if (columnType.Equals("System.String") && value.ToLower().Equals("is null"))
                        {
                            filter = string.Format("{0}{1}= '' or {2}{3}{4}{5}", AddBracketsToString(freqVar), stringLiterals.SPACE, AddBracketsToString(freqVar), stringLiterals.SPACE, operand, value);
                        }
                        else if (columnType.Equals("System.Single"))
                        {
                            filter = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}", AddBracketsToString(freqVar), stringLiterals.SPACE, operand, value.Replace(',', '.'));
                        }
                        else
                        {
                            filter = string.Format("{0}{1}{2}{3}", AddBracketsToString(freqVar), stringLiterals.SPACE, operand, value);
                        }
                    }

                    if (!doCrossTab)
                    {
                        if (string.IsNullOrEmpty(weightVar) || string.IsNullOrWhiteSpace(weightVar))
                        {
                            try
                            {
                                // special case because count(column) will never work with an 'is null' filter
                                if (filter.ToLower().EndsWith("is null"))
                                {
                                    // safe replacement in case column name includes "is null"
                                    string originalFilter = filter;
                                    string baseFilter = filter.Substring(0, filter.Length - 8);
                                    baseFilter = string.Format("{0}{1}is not null", baseFilter, stringLiterals.SPACE);

                                    string strataFilter = originalFilter;

                                    if (strataFilter.EndsWith("is null"))
                                    {
                                        //sum = mainTable.Select(CombineFilters(filter)).Length;
                                        //DataView sumView = new DataView(mainTable, CombineFilters(filter, dv), string.Empty, DataViewRowState.CurrentRows);
                                        //sum = sumView.Count;

                                        string combinedFilter = CombineFilters(filter, dv);
                                        sum = mainTable.Select(combinedFilter, "", DataViewRowState.CurrentRows).Length;
                                    }
                                    else
                                    {
                                        string strataFilter2 = string.Format("{0} AND {1}", baseFilter, strataFilter);

                                        double sumNotMissing = Convert.ToDouble(mainTable.Compute(string.Format("count({0})", AddBracketsToString(freqVar)), CombineFilters(strataFilter2, dv)));
                                        double sumTotal = Convert.ToDouble(mainTable.Compute(string.Format("count({0})", AddBracketsToString(strataKvp.Key)), CombineFilters(strataFilter, dv)));
                                        sum = sumTotal - sumNotMissing;
                                    }
                                }
                                else if (filter.ToLower().EndsWith("is null)"))
                                {
                                    // safe replacement in case column name includes "is null"
                                    string originalFilter = filter;
                                    string baseFilter = filter.Substring(0, filter.Length - 9);
                                    baseFilter = string.Format("{0}{1}is not null)", baseFilter, stringLiterals.SPACE);

                                    string strataFilter = originalFilter;

                                    if (strataFilter.EndsWith("is null)"))
                                    {
                                        //sum = mainTable.Select(CombineFilters(filter)).Length;
                                        //DataView sumView = new DataView(mainTable, CombineFilters(filter, dv), string.Empty, DataViewRowState.CurrentRows);
                                        //sum = sumView.Count;

                                        string combinedFilter = CombineFilters(filter, dv);
                                        sum = mainTable.Select(combinedFilter, "", DataViewRowState.CurrentRows).Length;
                                    }
                                    else
                                    {
                                        string strataFilter2 = string.Format("{0} AND {1}", baseFilter, strataFilter);

                                        double sumNotMissing = Convert.ToDouble(mainTable.Compute(string.Format("count({0})", AddBracketsToString(freqVar)), CombineFilters(strataFilter2, dv)));
                                        double sumTotal = Convert.ToDouble(mainTable.Compute(string.Format("count({0})", AddBracketsToString(strataKvp.Key)), CombineFilters(strataFilter, dv)));
                                        sum = sumTotal - sumNotMissing;
                                    }
                                }
                                else
                                {
                                    //DataView sumView = new DataView(mainTable, CombineFilters(filter, dv), "", DataViewRowState.CurrentRows);
                                    //sum = sumView.Count;
                                    //sum = Convert.ToDouble(mainTable.Compute("count(" + AddBracketsToString(freqVar) + ")", CombineFilters(filter, dv)));

                                    string combinedFilter = CombineFilters(filter, dv);
                                    sum = mainTable.Select(combinedFilter, "", DataViewRowState.CurrentRows).Length;
                                }
                            }
                            catch (InvalidCastException)
                            {
                                sum = 0;
                            }
                            totalSum = totalSum + sum;
                        }
                        else
                        {
                            double trySum;
                            bool success;
                            success = double.TryParse(mainTable.Compute(string.Format("sum({0})", AddBracketsToString(weightVar)), CombineFilters(filter, dv)).ToString(), out trySum);
                            if (success)
                            {
                                sum = trySum;
                            }
                            else
                            {
                                sum = 0;
                            }
                            totalSum = totalSum + sum;
                        }

                        value = s;

                        if (valuesAreBool)
                        {
                            value = RecodeBooleanToYesNo(s);
                        }

                        if (string.IsNullOrEmpty(value))
                        {
                            freqTable.Rows.Add(DBNull.Value, sum);
                        }
                        else
                        {
                            freqTable.Rows.Add(value, sum);
                        }
                    }
                    // end if for !doCrosstab
                    else
                    {
                        if (freqTable.Rows.Count == 0)
                        {
                            freqTable = new DataTable(strataKvp.Value);

                            if (valuesAreBool)
                            {
                                freqTable.Columns.Add(freqVar, typeof(string));
                            }
                            else
                            {
                                freqTable.Columns.Add(freqVar, mainTable.Columns[freqVar].DataType);
                            }

                            // do cross-tab
                            foreach (KeyValuePair<string, string> csKvp in crosstabValues)
                            {
                                if (inputs.IsRequestCancelled())
                                {
                                    return null;
                                }

                                freqTable.Columns.Add(csKvp.Value, typeof(double));
                                if (string.IsNullOrEmpty(csKvp.Value))
                                {
                                    nullColumnName = freqTable.Columns[freqTable.Columns.Count - 1].ColumnName;
                                }
                            }
                        }

                        value = s;

                        if (valuesAreBool)
                        {
                            value = RecodeBooleanToYesNo(s);
                        }

                        if (string.IsNullOrEmpty(value))
                        {
                            freqTable.Rows.Add(DBNull.Value);
                        }
                        else
                        {
                            freqTable.Rows.Add(value);
                        }

                        string originalFilter = filter;

                        foreach (KeyValuePair<string, string> csKvp in crosstabValues)
                        {
                            if (inputs.IsRequestCancelled())
                            {
                                return null;
                            }

                            crosstabValue = csKvp.Key;
                            crosstabValue = FormatValue(crosstabValue, crosstabColumnType);

                            if (string.IsNullOrEmpty(crosstabValue) || crosstabValue.Equals("''"))
                            {
                                if (crosstabColumnType.Equals("System.String"))
                                {
                                    filter = string.Format("{0} and ({1} = '' or {2} is null)", originalFilter, AddBracketsToString(crosstabVar), AddBracketsToString(crosstabVar));
                                }
                                else
                                {
                                    filter = string.Format("{0} and {1} is null", originalFilter, AddBracketsToString(crosstabVar));
                                }
                            }
                            else
                            {
                                filter = string.Format("{0} and {1} = {2}", originalFilter, AddBracketsToString(crosstabVar), crosstabValue);
                            }

                            if (filter.StartsWith(" and "))
                            {
                                filter = filter.Remove(0, 4);
                            }

                            if (string.IsNullOrEmpty(weightVar))
                            {
                                try
                                {
                                    // special case because count(column) will never work with an 'is null' filter
                                    //if (originalFilter.ToLower().EndsWith("is null"))
                                    //{
                                    //    string baseFilter = originalFilter.Substring(0, originalFilter.Length - 8);
                                    //    baseFilter = baseFilter + stringLiterals.SPACE + "is not null";
                                    //    if (baseFilter.Contains(AddBracketsToString(freqVar) + " = ''"))
                                    //    {
                                    //        baseFilter = baseFilter.Replace(AddBracketsToString(freqVar) + " = ''", "not " + AddBracketsToString(freqVar) + " = ''");
                                    //        if (baseFilter.Contains("'' or ["))
                                    //        {
                                    //            baseFilter = baseFilter.Replace("'' or [", "'' and [");
                                    //        }
                                    //    }
                                    //    string crosstabFilter = filter.Substring(originalFilter.Length + 5, filter.Length - originalFilter.Length - 5);
                                    //    if (crosstabFilter.EndsWith("is null"))
                                    //    {
                                    //        sum = filteredTable.Select(filter).Length;
                                    //    }
                                    //    else
                                    //    {
                                    //        string crosstabFilter2 = "(" + baseFilter + ") AND " + crosstabFilter;
                                    //        double sumNotMissing = Convert.ToDouble(filteredTable.Compute("count(" + AddBracketsToString(freqVar) + ")", crosstabFilter2));
                                    //        double sumTotal = Convert.ToDouble(filteredTable.Compute("count(" + AddBracketsToString(crosstabVar) + ")", crosstabFilter));
                                    //        sum = sumTotal - sumNotMissing;
                                    //    }                                        
                                    //}
                                    //else if (originalFilter.ToLower().EndsWith("is null)"))
                                    //{
                                    //    string baseFilter = originalFilter.Substring(0, originalFilter.Length - 9);
                                    //    baseFilter = baseFilter + stringLiterals.SPACE + "is not null)";
                                    //    if (baseFilter.Contains(AddBracketsToString(freqVar) + " = ''"))
                                    //    {
                                    //        baseFilter = baseFilter.Replace(AddBracketsToString(freqVar) + " = ''", "not " + AddBracketsToString(freqVar) + " = ''");
                                    //        if (baseFilter.Contains("'' or ["))
                                    //        {
                                    //            baseFilter = baseFilter.Replace("'' or [", "'' and [");
                                    //        }
                                    //    }
                                    //    string crosstabFilter = filter.Substring(originalFilter.Length + 5, filter.Length - originalFilter.Length - 5);
                                    //    if (crosstabFilter.EndsWith("is null)"))
                                    //    {
                                    //        sum = filteredTable.Select(filter).Length;
                                    //    }
                                    //    else
                                    //    {
                                    //        string crosstabFilter2 = baseFilter + " AND " + crosstabFilter;
                                    //        double sumNotMissing = Convert.ToDouble(filteredTable.Compute("count(" + AddBracketsToString(freqVar) + ")", crosstabFilter2));
                                    //        double sumTotal = Convert.ToDouble(filteredTable.Compute("count(" + AddBracketsToString(crosstabVar) + ")", crosstabFilter));
                                    //        sum = sumTotal - sumNotMissing;
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //DataView sumView = new DataView(mainTable, CombineFilters(filter, dv), string.Empty, DataViewRowState.CurrentRows);
                                    //sum = sumView.Count;
                                    string combinedFilter = CombineFilters(filter, dv);
                                    sum = mainTable.Select(combinedFilter, "", DataViewRowState.CurrentRows).Length;
                                    //sum = mainTable.Select(CombineFilters(filter), string.Empty).Length;
                                    //sum = Convert.ToDouble(filteredTable.Compute("count(" + AddBracketsToString(freqVar) + ")", filter));
                                    //}
                                }
                                catch (InvalidCastException)
                                {
                                    sum = 0;
                                }
                                totalSum = totalSum + sum;
                            }
                            else
                            {
                                double trySum;
                                bool success = double.TryParse(mainTable.Compute(string.Format("sum({0})", AddBracketsToString(weightVar)), CombineFilters(filter, dv)).ToString(), out trySum);
                                if (success)
                                {
                                    sum = trySum;
                                }
                                else
                                {
                                    sum = 0;
                                }
                                totalSum = totalSum + sum;
                            }

                            int lastRowAdded = freqTable.Rows.Count - 1;
                            if (!string.IsNullOrEmpty(csKvp.Value))
                            {
                                freqTable.Rows[lastRowAdded][csKvp.Value] = sum;
                            }
                            else
                            {
                                freqTable.Rows[lastRowAdded][nullColumnName] = sum;
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(nullColumnName))
                {
                    freqTable.Columns[nullColumnName].ColumnName = "Missing"; // config.Settings.RepresentationOfMissing;
                }

                DataTable sortedFreqTable = new DataTable();

                if (sortHighToLow && string.IsNullOrEmpty(crosstabVar))
                {
                    sortedFreqTable = SortHighToLow(freqTable, freqTable.Columns["freq"]);
                }
                else
                {
                    if (valuesAreBool)
                    {
                        sortedFreqTable = SortBySingleColumn(freqTable, freqTable.Columns[freqVar], SortOrder.Descending);
                    }
                    else
                    {
                        sortedFreqTable = SortBySingleColumn(freqTable, freqTable.Columns[freqVar]);
                    }

                }

                if (inputs.IsRequestCancelled())
                {
                    return null;
                }

                List<DescriptiveStatistics> descriptiveStatistics = new List<DescriptiveStatistics>();
                try
                {
                    // do means
                    if (doMeans && includeFullSummaryStatistics)
                    {
                        string outerFilter = string.Empty;
                        if (doStratification)
                        {
                            outerFilter = strataKvp.Key;
                        }
                        if (doCrossTab)
                        {
                            foreach (KeyValuePair<string, string> csKvp in crosstabValues)
                            {
                                if (inputs.IsRequestCancelled())
                                {
                                    return null;
                                }

                                string crosstabValue = csKvp.Key;
                                crosstabValue = FormatValue(crosstabValue, crosstabColumnType);

                                string filter = string.Format("{0} and {1} = {2}", outerFilter, AddBracketsToString(crosstabVar), crosstabValue);
                                if (filter.StartsWith(" and "))
                                {
                                    filter = filter.Remove(0, 4);
                                }
                                inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_CALCULATING_DESCRIPTIVE_STATISTICS);
                                DataTable filteredTable = GenerateTable(inputs);
                                DescriptiveStatistics means = DoMeans(inputs, filteredTable, freqTable, filter, outerFilter);
                                //if (means.observations > -1)
                                //{
                                descriptiveStatistics.Add(means);
                                //}
                            }

                            bool showAnova = true;
                            if (inputs.InputVariableList.ContainsKey("showanova"))
                            {
                                bool.TryParse(inputs.InputVariableList["showanova"], out showAnova);
                            }

                            if (crosstabValues.Count >= 2 && crosstabValues.Count <= 127 && showAnova)
                            {
                                ////////////////////////////////////////////
                                inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_CALCULATING_DESCRIPTIVE_STATISTICS_ADVANCED);

                                List<double> observations = new List<double>();
                                List<double> avgs = new List<double>();
                                List<double> vars = new List<double>();
                                double grandMean = 0;
                                double grandTotal = 0;
                                int crosstabs = 0;
                                foreach (DescriptiveStatistics stats in descriptiveStatistics)
                                {
                                    if (stats.Mean > -1)
                                    {
                                        observations.Add(stats.Observations);
                                        grandTotal += stats.Observations;
                                        avgs.Add(stats.Mean.Value);
                                        vars.Add(stats.Variance.Value);
                                        grandMean = stats.GrandMean.Value;

                                        crosstabs++;
                                    }
                                }
                                DescriptiveStatistics ds = descriptiveStatistics[0];
                                ds.SsBetween = CalculateSSBetween(grandMean, observations, avgs);
                                ds.DfBetween = crosstabs - 1;
                                ds.MsBetween = ds.SsBetween / ds.DfBetween;
                                ds.SsWithin = CalculateSSWithin(observations, vars);
                                ds.DfWithin = grandTotal - crosstabs;
                                ds.MsWithin = ds.SsWithin / ds.DfWithin;
                                ds.FStatistic = ds.MsBetween / ds.MsWithin;
                                ds.AnovaPValue = new StatisticsRepository.statlib().PfromF(ds.FStatistic.Value, ds.DfBetween.Value, ds.DfWithin.Value);
                                ds.ChiSquare = CalculateChiSquare(ds.DfWithin.Value, ds.MsWithin.Value, observations, vars);
                                ds.BartlettPValue = new StatisticsRepository.statlib().PfromX2(ds.ChiSquare.Value, ds.DfBetween.Value);

                                string nullFilter = string.Format("{0} is not null", AddBracketsToString(freqVar));
                                string weightFilter = string.Format("{0} is not null", AddBracketsToString(weightVar));
                                string crosstabFilter = string.Format("{0} is not null", AddBracketsToString(crosstabVar));
                                if (GetColumnType(crosstabVar).Equals("System.String"))
                                {
                                    crosstabFilter = string.Format("{0} and not {1} = ''", crosstabFilter, AddBracketsToString(crosstabVar));
                                }
                                string fullFilter = string.Empty;
                                if (string.IsNullOrEmpty(outerFilter))
                                {
                                    fullFilter = nullFilter;
                                }
                                else
                                {
                                    fullFilter = string.Format("{0} and {1}", outerFilter, nullFilter);
                                }

                                if (!string.IsNullOrEmpty(weightVar))
                                {
                                    if (string.IsNullOrEmpty(fullFilter))
                                    {
                                        fullFilter = weightFilter;
                                    }
                                    else
                                    {
                                        fullFilter = string.Format("{0} and {1}", fullFilter, weightFilter);
                                    }
                                }

                                if (!string.IsNullOrEmpty(crosstabVar))
                                {
                                    if (string.IsNullOrEmpty(fullFilter))
                                    {
                                        fullFilter = crosstabVar;
                                    }
                                    else
                                    {
                                        fullFilter = string.Format("{0} and {1}", fullFilter, crosstabFilter);
                                    }
                                }

                                GadgetParameters newInputs = new GadgetParameters();
                                newInputs.ShouldIncludeFullSummaryStatistics = false;
                                newInputs.ShouldIncludeMissing = false;
                                newInputs.MainVariableName = freqVar;
                                newInputs.CustomFilter = fullFilter;
                                newInputs.TableName = inputs.TableName;
                                newInputs.DatasourceName = inputs.DatasourceName;
                                if (!string.IsNullOrEmpty(weightVar))
                                {
                                    newInputs.WeightVariableName = weightVar;
                                }

                                DataTable freqHorizontal = ExtractFirstFrequencyTable(this.GenerateFrequencyTable(newInputs));

                                newInputs = new GadgetParameters();
                                newInputs.ShouldIncludeFullSummaryStatistics = false;
                                newInputs.ShouldIncludeMissing = false;
                                newInputs.MainVariableName = crosstabVar;
                                newInputs.CustomFilter = fullFilter;
                                newInputs.TableName = inputs.TableName;
                                newInputs.DatasourceName = inputs.DatasourceName;
                                if (!string.IsNullOrEmpty(weightVar))
                                {
                                    newInputs.WeightVariableName = weightVar;
                                }

                                //Debug.Print("Horizontal");
                                //foreach (DataRow r in freqHorizontal.Rows)
                                //{
                                //    string line = string.Empty;
                                //    foreach (object dstr in r.ItemArray)
                                //    {
                                //        line = line + dstr.ToString() + "  ";
                                //    }
                                //    Debug.Print(line);
                                //}

                                DataTable freqVertical = ExtractFirstFrequencyTable(this.GenerateFrequencyTable(newInputs));

                                Debug.Print("---");

                                Debug.Print("Vertical");
                                foreach (DataRow r in freqVertical.Rows)
                                {
                                    string line = string.Empty;
                                    foreach (object dstr in r.ItemArray)
                                    {
                                        line = string.Format("{0}{1}  ", line, dstr.ToString());
                                    }
                                    Debug.Print(line);
                                }

                                List<List<object>> allLocalFreqs = ConvertTableToListOfLists(freqTable);
                                //freqVertical = SortHighToLow(freqVertical, freqVertical.Columns[0]);
                                ds.KruskalWallisH = CalculateKruskalWallisH(freqHorizontal, SortHighToLow(freqVertical, freqVertical.Columns[0]), allLocalFreqs, grandTotal);
                                ds.KruskalPValue = new StatisticsRepository.statlib().PfromX2(ds.KruskalWallisH.Value, ds.DfBetween.Value);
                                descriptiveStatistics[0] = ds;
                                ////////////////////////////////////////////
                            }
                        }
                        else
                        {
                            inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_CALCULATING_DESCRIPTIVE_STATISTICS);
                            DataTable filteredTable = GenerateTable(inputs);

                            DescriptiveStatistics means = DoMeans(inputs, filteredTable, freqTable, outerFilter, string.Empty);
                            if (means.Observations == -1)
                            {
                                return null;
                            }
                            descriptiveStatistics.Add(means);
                        }
                    }// end if doMeans && includeFullSummaryStats
                    else
                    {
                        DescriptiveStatistics means = new DescriptiveStatistics();
                        means.Observations = totalSum;
                        descriptiveStatistics.Add(means);
                    }

                    // Add filtered record count for UI  
                    foreach (DescriptiveStatistics desc in descriptiveStatistics)
                    {
                        desc.FilteredObservations = RecordCount;
                    }
                }
                catch (Exception ex)
                {
                    inputs.ErrorMessage = ex.Message;
                }

                #region Debug
                //System.Diagnostics.Debug.Print(sortedFreqTable.TableName);
                //for (int i = 0; i < sortedFreqTable.Rows.Count; i++)
                //{
                //    string line = string.Empty;
                //    for (int j = 0; j < sortedFreqTable.Columns.Count; j++)
                //    {
                //        line = line + sortedFreqTable.Rows[i][j].ToString() + " ";
                //    }
                //System.Diagnostics.Debug.Print(line);
                //}
                //System.Diagnostics.Debug.Print(string.Empty);
                #endregion // Debug

                if (inputs != null && inputs.ShouldShowCommentLegalLabels && field != null && field is DDLFieldOfCommentLegal)
                {
                    AddCommentLegalLabelsToFreqTable(sortedFreqTable);
                }
                else if (inputs != null && inputs.ShouldShowCommentLegalLabels && field != null && field is OptionField)
                {
                    AddOptionLabelsToFreqTable(sortedFreqTable);
                }

                stratifiedFrequencyTables.Add(sortedFreqTable, descriptiveStatistics);
                string processedStatusMessage = string.Format(SharedStrings.DASHBOARD_GADGET_STATUS_PROCESSED_TABLES, stratifiedFrequencyTables.Count.ToString());
                inputs.UpdateGadgetStatus(processedStatusMessage);
            }// end foreach strata

            return stratifiedFrequencyTables;
        }

        public void EwavConstructTableColumnNames(GadgetParameters inputs)
        {
            ////   Ewav.DAL.SqlServer.CommonSQLDAO ewDAL = new Ewav.DAL.SqlServer.CommonSQLDAO();
            //Ewav.BAL.EwavDatasourceListManager edlm = new Ewav.BAL.EwavDatasourceListManager();
            //DataManager dm = new DataManager(edlm.ConnectionString, edlm.MetaDataViewName,
            //    edlm.MetaDatabaseTypeEnum);
            //IRawDataDao ewDAL = dm.DAOFactory.RawDataDao;
            EntityManager ewDAL = new EntityManager();

            if (this.View != null || !string.IsNullOrEmpty(TableName))
            {
                TableColumnNames = new Dictionary<string, string>();
                FieldTable.Rows.Clear();
                FieldTable.Rows.Clear();

                DataTable parentTable = new DataTable();
                if (IsUsingEpiProject)
                {
                    ////  Use EWAV library  
                    ////  DataTable pageTable = Database.GetTopTwoTable(page.TableName);
                    ////                          
                    //parentTable = ewDAL.GetTopTwoTable(inputs.TableName);
                    ////  parentTable = Database.GetTopTwoTable(view.TableName);
                    //AddPermanentVariablesToTable(parentTable);
                    //AddSystemVariablesToTable(parentTable);
                    //if (!tableColumnNames.ContainsKey("UniqueKey"))
                    //{
                    //    tableColumnNames.Add("UniqueKey", "System.Int32");
                    //    fieldTable.Rows.Add("UniqueKey", "System.Int32", view.TableName, view.Name, null);
                    //}
                    //if (!tableColumnNames.ContainsKey("RECSTATUS") && !tableColumnNames.ContainsKey("RecStatus"))
                    //{
                    //    tableColumnNames.Add("RecStatus", "System.Int32");
                    //    fieldTable.Rows.Add("RecStatus", "System.Int32", view.TableName, view.Name, null);
                    //}
                    //foreach (Page page in View.Pages)
                    //{
                    //    //  Use EWAV library  
                    //    //  DataTable pageTable = Database.GetTopTwoTable(page.TableName);
                    //    //      
                    //    DataTable pageTable = ewDAL.GetTopTwoTable(inputs.TableName);
                    //    foreach (DataColumn dc in pageTable.Columns)
                    //    {
                    //        if (!tableColumnNames.ContainsKey(dc.ColumnName))
                    //        {
                    //            tableColumnNames.Add(dc.ColumnName, dc.DataType.ToString());
                    //            if (view.Fields.Contains(dc.ColumnName))
                    //            {
                    //                fieldTable.Rows.Add(dc.ColumnName, dc.DataType.ToString(), page.TableName, view.Name, view.Fields[dc.ColumnName]);
                    //            }
                    //        }
                    //        //fieldTable.Rows.Add(dc.ColumnName, dc.DataType.ToString(), page.TableName, false);
                    //    }
                    //    RelateInto(parentTable, pageTable, "GlobalRecordId", "GlobalRecordId", false);
                    //}
                    //// Get group fields too
                    //foreach (Field field in View.Fields)
                    //{
                    //    if (field is GroupField && !TableColumnNames.ContainsKey(field.Name))
                    //    {
                    //        FieldTable.Rows.Add(field.Name, string.Empty, string.Empty, view.Name, field as GroupField);
                    //        TableColumnNames.Add(field.Name, field.GetType().ToString());
                    //    }
                    //}
                }
                else
                {
                    if (string.IsNullOrEmpty(this.CustomQuery))
                    {
                        // Read the top-two table once and place the fields names and their types into the column name dictionary.
                        // This way, we avoid having to query the DB over and over when we want to get a list of fields.
                        //    parentTable = GenerateTopTwoTable();
                        //  Use EWAV library  
                        //  DataTable pageTable = Database.GetTopTwoTable(page.TableName);
                        //      
                        parentTable = ewDAL.GetTopTwoTable(inputs.DatasourceName, inputs.TableName);
                    }
                    else
                    {
                        // can't do a top two table if the user has custom SQL, so do the whole thing. 
                        // This has the benefit of populating the data set while it's doing this, but the downside
                        // is that you'll wait on it until it's done to do anything with the UI since you can't work 
                        // with gadgets if you don't have column names.
                        parentTable = GenerateTable();
                    }
                    foreach (DataColumn dc in parentTable.Columns)
                    {
                        if (!TableColumnNames.ContainsKey(dc.ColumnName))
                        {
                            TableColumnNames.Add(dc.ColumnName, dc.DataType.ToString());
                            FieldTable.Rows.Add(dc.ColumnName, dc.DataType.ToString(), TableName, string.Empty, null);
                        }
                        //fieldTable.Rows.Add(dc.ColumnName, dc.DataType.ToString(), TableName, false);                        
                    }
                }

                if (ConnectionsForRelate.Count > 0)
                {
                    foreach (RelatedConnection conn in ConnectionsForRelate)
                    {
                        DataTable relatedTable = new DataTable();

                        if (conn.IsEpiInfoProject)
                        {
                            relatedTable = conn.db.Select(conn.db.CreateQuery(string.Format("SELECT TOP 2 * {0}", conn.view.FromViewSQL)));

                            foreach (Page page in conn.view.Pages)
                            {
                                string pageGUIDName = string.Format("{0}.GlobalRecordId", page.TableName);
                                if (relatedTable.Columns.Contains(pageGUIDName))
                                {
                                    relatedTable.Columns.Remove(pageGUIDName);
                                }
                            }
                        }
                        else
                        {
                            relatedTable = conn.db.GetTopTwoTable(conn.TableName);
                        }

                        RelateInto(parentTable, relatedTable, conn.ParentKeyField, conn.ChildKeyField, conn.UseUnmatched);

                        if (parentTable.Columns.Contains("RECSTATUS"))
                        {
                            parentTable.Columns["RECSTATUS"].ColumnName = "RecStatus";
                        }

                        foreach (DataColumn dc in parentTable.Columns)
                        {
                            if (!TableColumnNames.ContainsKey(dc.ColumnName))
                            {
                                TableColumnNames.Add(dc.ColumnName, dc.DataType.ToString());
                                if (conn.IsEpiInfoProject)
                                {
                                    if (conn.view.Fields.Contains(dc.ColumnName))
                                    {
                                        FieldTable.Rows.Add(dc.ColumnName, dc.DataType.ToString(), string.Empty, conn.view.Name, conn.view.Fields[dc.ColumnName]);
                                    }
                                }
                                else
                                {
                                    FieldTable.Rows.Add(dc.ColumnName, dc.DataType.ToString(), conn.TableName, string.Empty, null);
                                }
                            }
                        }

                        if (conn.IsEpiInfoProject)
                        {
                            // get group fields in related connections too
                            foreach (Field field in conn.view.Fields)
                            {
                                if (field is GroupField)
                                {
                                    if (!TableColumnNames.ContainsKey(field.Name))
                                    {
                                        FieldTable.Rows.Add(field.Name, string.Empty, string.Empty, conn.view.Name, field as GroupField);
                                        TableColumnNames.Add(field.Name, field.GetType().ToString());
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (IDashboardRule rule in this.Rules)
                {
                    if (rule is DataAssignmentRule)
                    {
                        DataAssignmentRule assignRule = rule as DataAssignmentRule;

                        if (!TableColumnNames.ContainsKey(assignRule.DestinationColumnName))
                        {
                            FieldTable.Rows.Add(assignRule.DestinationColumnName, assignRule.DestinationColumnType);
                            TableColumnNames.Add(assignRule.DestinationColumnName, assignRule.DestinationColumnType);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Extracts the first frequency table from a dictionary of table-list of statistics pairs
        /// </summary>
        /// <param name="frequencyTables">The Dictionary to process</param>
        /// <returns>The first data table in the dictionary</returns>
        public DataTable ExtractFirstFrequencyTable(Dictionary<DataTable, List<DescriptiveStatistics>> frequencyTables)
        {
            if (frequencyTables.Count >= 1)
            {
                foreach (KeyValuePair<DataTable, List<DescriptiveStatistics>> tableKvp in frequencyTables)
                {
                    if (tableKvp.Key != null)
                    {
                        return tableKvp.Key;
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Generates a .NET 2x2 table based off of the selection criteria.
        /// </summary>
        /// <param name="exposure">The exposure variable</param>
        /// <param name="outcome">The outcome variable</param>        
        /// <returns>DataTable</returns>
        public DataTable Generate2x2Table(string exposure, string outcome, bool includeMissing = false)
        {
            #region Input Validation
            if (string.IsNullOrEmpty(exposure) || string.IsNullOrEmpty(outcome))
            {
                throw new ApplicationException("No columns specified for SELECT.");
            }
            #endregion // Input Validation

            List<string> columnNames = new List<string>();

            if (exposure.Equals(outcome))
            {
                return null;
            }

            columnNames.Add(exposure);
            columnNames.Add(outcome);

            DataTable filteredTable = GenerateTable(columnNames);

            // Section: Check for a scenario where no records may exist due to the user's choice of selection conditions
            if (filteredTable == null || filteredTable.Rows.Count <= 0)
            {
                return null;
            }

            // Section: Get distinct values for both exposure and outcome
            List<string> exposureValues = new List<string>();
            List<string> outcomeValues = new List<string>();

            Field exposureField = null;
            Field outcomeField = null;

            foreach (DataRow fieldRow in FieldTable.Rows)
            {
                if (fieldRow["columnname"].Equals(exposure))
                {
                    if (fieldRow["epifieldtype"] is Field)
                    {
                        exposureField = fieldRow["epifieldtype"] as Field;
                    }
                    break;
                }
            }

            foreach (DataRow fieldRow in FieldTable.Rows)
            {
                if (fieldRow["columnname"].Equals(outcome))
                {
                    if (fieldRow["epifieldtype"] is Field)
                    {
                        outcomeField = fieldRow["epifieldtype"] as Field;
                    }
                    break;
                }
            }

            foreach (DataRow row in filteredTable.Rows)
            {
                string key = string.Empty;

                key = row[exposure].ToString();
                if (!exposureValues.Contains(key))
                {
                    if (string.IsNullOrEmpty(key) && (includeMissing == false))
                    {
                        // don't add missing values unless the record processing scope specifically includes them
                    }
                    else
                    {
                        exposureValues.Add(key);
                    }
                }

                key = row[outcome].ToString();
                if (!outcomeValues.Contains(key))
                {
                    if (string.IsNullOrEmpty(key) && (includeMissing == false))
                    {
                        // don't add missing values unless the record processing scope specifically includes them
                    }
                    else
                    {
                        outcomeValues.Add(key);
                    }
                }

                // Don't loop through everything if we already have two distinct values. We actually let it add up to
                // three so we can do proper error checking on the values in the section below and display the appropriate
                // message.
                if ((outcomeValues.Count > 2 && exposureValues.Count >= 2) || (exposureValues.Count > 2 && outcomeValues.Count >= 2))
                {
                    break;
                }
            }

            DataTable twoByTwoTable = new DataTable("2x2");

            // Section: Sort the values and set the representations of Yes and No
            string exposureValue1 = string.Empty;
            string exposureValue2 = string.Empty;

            if (exposureValues.Count < 2)
            {
                exposureValue1 = exposureValues[0];
            }
            else
            {
                exposureValue1 = exposureValues[0];
                exposureValue2 = exposureValues[1];
            }

            string outcomeValue1 = string.Empty;
            string outcomeValue2 = string.Empty;

            if (outcomeValues.Count < 2)
            {
                outcomeValue1 = outcomeValues[0];
            }
            else
            {
                outcomeValue1 = outcomeValues[0];
                outcomeValue2 = outcomeValues[1];
            }

            string columnType = string.Empty;

            // sort the exposure values and check the exposure column to see if it's boolean
            columnType = filteredTable.Columns[exposure].DataType.ToString();
            if (columnType.Equals("System.Boolean") || (exposureField != null && exposureField is YesNoField))
            {
                if (exposureValue1.Equals("0") || exposureValue1.ToLower().Equals("false"))
                {
                    exposureValue1 = Config.Settings.RepresentationOfNo;
                    exposureValue2 = Config.Settings.RepresentationOfYes;
                }
                else
                {
                    exposureValue1 = Config.Settings.RepresentationOfYes;
                    exposureValue2 = Config.Settings.RepresentationOfNo;
                }

                // make sure 'yes' is always first
                if ((exposureValues[0].Equals("0") || exposureValues[0].ToLower().Equals("false")) && exposureValues.Count >= 2)
                {
                    string temp = exposureValues[0];
                    exposureValues[0] = exposureValues[1];
                    exposureValues[1] = temp;

                    exposureValue1 = Config.Settings.RepresentationOfYes;
                    exposureValue2 = Config.Settings.RepresentationOfNo;
                }
            }
            else
            {
                exposureValues.Sort();
                exposureValue1 = exposureValues[0];
                exposureValue2 = exposureValues[1];
            }

            // sort the outcome values and check the outcome column to see if it's boolean
            columnType = filteredTable.Columns[outcome].DataType.ToString();
            if (columnType.Equals("System.Boolean") || (outcomeField != null && outcomeField is YesNoField))
            {
                if (outcomeValue1.Equals("0") || outcomeValue1.ToLower().Equals("false"))
                {
                    outcomeValue1 = Config.Settings.RepresentationOfNo;
                    outcomeValue2 = Config.Settings.RepresentationOfYes;
                }
                else
                {
                    outcomeValue1 = Config.Settings.RepresentationOfYes;
                    outcomeValue2 = Config.Settings.RepresentationOfNo;
                }

                // make sure 'yes' is always first
                if ((outcomeValues[0].Equals("0") || outcomeValues[0].ToLower().Equals("false")) && outcomeValues.Count >= 2)
                {
                    string temp = outcomeValues[0];
                    outcomeValues[0] = outcomeValues[1];
                    outcomeValues[1] = temp;

                    outcomeValue1 = Config.Settings.RepresentationOfYes;
                    outcomeValue2 = Config.Settings.RepresentationOfNo;
                }
            }
            else
            {
                // if it's not boolean, do a sort so that results look consistent
                outcomeValues.Sort();
                outcomeValue1 = outcomeValues[0];
                outcomeValue2 = outcomeValues[1];
            }

            // Section: Set up the 2x2 table as a .NET DataTable, add the appropriate columns and Rows for the cross-tab
            twoByTwoTable.Columns.Add("exposure", typeof(string));

            // make sure the column names contain our two distinct values
            twoByTwoTable.Columns.Add(outcomeValue1, typeof(int));
            twoByTwoTable.Columns.Add(outcomeValue2, typeof(int));

            // the first cell in each row should contain the two distinct exposure values
            twoByTwoTable.Rows.Add(exposureValue1, 0, 0);
            twoByTwoTable.Rows.Add(exposureValue2, 0, 0);

            try
            {
                // iterate over each row in the filtered table and increment the appropriate cell in the 2x2 table
                foreach (DataRow row in filteredTable.Rows)
                {
                    if (row[exposure].ToString() == exposureValues[0])
                    {
                        if (row[outcome].ToString() == outcomeValues[0])
                        {
                            twoByTwoTable.Rows[0][1] = Convert.ToInt32(twoByTwoTable.Rows[0][1]) + 1;
                        }
                        else if (row[outcome].ToString() == outcomeValues[1])
                        {
                            twoByTwoTable.Rows[0][2] = Convert.ToInt32(twoByTwoTable.Rows[0][2]) + 1;
                        }
                    }
                    else if (row[exposure].ToString() == exposureValues[1])
                    {
                        if (row[outcome].ToString() == outcomeValues[0])
                        {
                            twoByTwoTable.Rows[1][1] = Convert.ToInt32(twoByTwoTable.Rows[1][1]) + 1;
                        }
                        else if (row[outcome].ToString() == outcomeValues[1])
                        {
                            twoByTwoTable.Rows[1][2] = Convert.ToInt32(twoByTwoTable.Rows[1][2]) + 1;
                        }
                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                twoByTwoTable.Rows.Add(SharedStrings.TWO_BY_TWO_VALUES_TOO_FEW, -1, -1);
                return twoByTwoTable;
            }

            // Section: Check for more or fewer than two distinct values
            if (exposureValues.Count > 2)
            {
                twoByTwoTable.Rows.Add(SharedStrings.EXPOSURE_VALUES_TOO_MANY, -1, -1);
                return twoByTwoTable;
            }
            else if (outcomeValues.Count > 2)
            {
                twoByTwoTable.Rows.Add(SharedStrings.OUTCOME_VALUES_TOO_MANY, -1, -1);
                return twoByTwoTable;
            }

            // the resulting 2x2 table contains the totals for each cell that we need to run all of the table statistics
            return twoByTwoTable;
        }

        public DataTable GenerateEpiCurveTable(GadgetParameters inputs, decimal minWeek, decimal maxWeek, bool includeMissing = false)
        {
            #region Input Validation
            if (string.IsNullOrEmpty(inputs.MainVariableName))
            {
                throw new ArgumentNullException("inputs.MainVariableName");
            }
            #endregion // Input Validation

            inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_CREATING_VARIABLES);

            string epiWeekColumn = inputs.MainVariableName;
            string caseStatusColumn = inputs.CrosstabVariableName;

            inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_GENERATING_TABLE);

            DataView filteredView = GenerateView(inputs);

            if (inputs.GadgetFilters != null && inputs.GadgetFilters.Count > 0)
            {
                filteredView.RowFilter = CombineFilters(GenerateFilterCriteria(inputs.GadgetFilters), filteredView);
            }

            if (inputs.IsRequestCancelled())
            {
                return null;
            }

            // Section: Check for a scenario where no records may exist due to the user's choice of selection conditions
            if (filteredView == null || filteredView.Count <= 0)
            {
                inputs.UpdateGadgetStatus(SharedStrings.NO_RECORDS_SELECTED);
                return null;
            }

            List<decimal?> dateColumnValues = new List<decimal?>();
            List<string> statusColumnValues = new List<string>();

            inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_FINDING_DATE_VALUES);
            // find all distinct date values and case status values
            foreach (DataRowView rowView in filteredView)
            {
                DataRow row = rowView.Row;

                if (inputs.IsRequestCancelled())
                {
                    return null;
                }

                decimal? dateKey = null;

                if (!string.IsNullOrEmpty(row[epiWeekColumn].ToString()))
                {
                    decimal result;
                    Decimal.TryParse(row[epiWeekColumn].ToString(), out result);

                    dateKey = result;

                    if (dateKey.HasValue && !dateColumnValues.Contains(dateKey.Value))
                    {
                        if (!(dateKey.HasValue) && includeMissing == false)
                        {
                            // don't add missing values
                        }
                        else
                        {
                            if (dateKey.Value >= minWeek && dateKey.Value <= maxWeek)
                            {
                                if (!dateColumnValues.Contains(Math.Truncate(dateKey.Value)))
                                {
                                    dateColumnValues.Add(Math.Truncate(dateKey.Value));
                                }
                            }
                        }
                    }
                }

                string key = null;
                if (caseStatusColumn.Equals("_default_"))
                {
                    if (!statusColumnValues.Contains("true"))
                    {
                        statusColumnValues.Add("true");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(row[caseStatusColumn].ToString()))
                    {
                        key = row[caseStatusColumn].ToString();
                        if (!string.IsNullOrEmpty(key) && !statusColumnValues.Contains(key))
                        {
                            statusColumnValues.Add(key);
                        }
                    }
                }
            }

            inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_GENERATING_EPI_CURVE_TABLE);
            // create and set up the epi curve table; add the approrpiate columns
            DataTable epiCurveTable = new DataTable("EpiCurveTable");
            epiCurveTable.Columns.Add("totals", typeof(int));
            epiCurveTable.Columns.Add(epiWeekColumn, typeof(decimal));
            epiCurveTable.Columns.Add(caseStatusColumn, typeof(string));

            // fill in the epi curve table with one row for each value of case status, and set the "total" to zero
            foreach (decimal? dt in dateColumnValues)
            {
                if (inputs.IsRequestCancelled())
                {
                    return null;
                }

                if (dt.HasValue)
                {
                    foreach (string s in statusColumnValues)
                    {
                        epiCurveTable.Rows.Add(0, (decimal)dt, s);
                    }
                }
            }

            inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_TOTALS_EPI_CURVE_TABLE);
            // iterate over all Rows in the filtered table and increment the appropriate "totals" cell in the epi curve table when matches are found
            foreach (DataRowView rowView in filteredView)
            {
                if (inputs.IsRequestCancelled())
                {
                    return null;
                }

                DataRow row = rowView.Row;

                // TODO: There must be a more efficient way of doing this?
                foreach (decimal? dt in dateColumnValues)
                {
                    if (inputs.IsRequestCancelled())
                    {
                        return null;
                    }

                    decimal result;
                    Decimal.TryParse(row[epiWeekColumn].ToString(), out result);

                    if (dt.HasValue && row[epiWeekColumn] != DBNull.Value && Math.Truncate(result) == dt)
                    {
                        foreach (string s in statusColumnValues)
                        {
                            if (inputs.IsRequestCancelled())
                            {
                                return null;
                            }

                            if (!caseStatusColumn.Equals("_default_") && row[caseStatusColumn].ToString().Equals(s))
                            {
                                foreach (DataRow iRow in epiCurveTable.Rows)
                                {
                                    if (inputs.IsRequestCancelled())
                                    {
                                        return null;
                                    }

                                    if (iRow[epiWeekColumn].ToString().Equals(dt.ToString()) && iRow[caseStatusColumn].Equals(s))
                                    {
                                        iRow["totals"] = int.Parse(iRow["totals"].ToString()) + 1;
                                        break;
                                    }
                                }
                            }
                            else if (caseStatusColumn.Equals("_default_"))
                            {
                                foreach (DataRow iRow in epiCurveTable.Rows)
                                {
                                    if (inputs.IsRequestCancelled())
                                    {
                                        return null;
                                    }

                                    if (iRow[epiWeekColumn].ToString().Equals(dt.ToString()))
                                    {
                                        iRow["totals"] = int.Parse(iRow["totals"].ToString()) + 1;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return epiCurveTable;
        }

        /// <summary>
        /// Generates a data table for use in creating epi curve charts
        /// </summary>
        /// <param name="inputs">The parameters for this method</param>
        /// <param name="maxDate">The maximum date value to use when selecting dates</param>
        /// <param name="minDate">The minimum date value to use when selecting dates</param>
        /// <returns>DataTable</returns>
        public DataTable GenerateEpiCurveTable(GadgetParameters inputs, DateTime minDate, DateTime maxDate, bool includeMissing = false)
        {
            #region Input Validation
            if (string.IsNullOrEmpty(inputs.MainVariableName))
            {
                throw new ArgumentNullException("inputs.MainVariableName");
            }
            #endregion // Input Validation

            inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_CREATING_VARIABLES);

            string dateColumn = inputs.MainVariableName;
            string caseStatusColumn = inputs.CrosstabVariableName;

            inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_GENERATING_TABLE);

            DataView filteredView = GenerateView(inputs);

            if (inputs.GadgetFilters != null && inputs.GadgetFilters.Count > 0)
            {
                filteredView.RowFilter = CombineFilters(GenerateFilterCriteria(inputs.GadgetFilters), filteredView);
            }

            if (inputs.IsRequestCancelled())
            {
                return null;
            }

            // Section: Check for a scenario where no records may exist due to the user's choice of selection conditions
            if (filteredView == null || filteredView.Count <= 0)
            {
                inputs.UpdateGadgetStatus(SharedStrings.NO_RECORDS_SELECTED);
                return null;
            }
            string dateInterval = inputs.InputVariableList["dateinterval"];
            List<DateTime?> dateColumnValues = new List<DateTime?>();
            List<string> dateTimeColumnValues = new List<string>();
            List<string> statusColumnValues = new List<string>();

            inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_FINDING_DATE_VALUES);
            // find all distinct date values and case status values
            foreach (DataRowView rowView in filteredView)
            {
                if (inputs.IsRequestCancelled())
                {
                    return null;
                }

                DataRow row = rowView.Row;
                DateTime? dateKey = null;

                if (!string.IsNullOrEmpty(row[dateColumn].ToString()))
                {
                    dateKey = (DateTime?)row[dateColumn];
                    if (dateKey.HasValue)
                    {
                        int year = 0;
                        int month = 0;
                        int day = 0;
                        int hour = 0;
                        DateTime newDate = DateTime.Now;

                        switch (dateInterval.ToLower())
                        {
                            case "years":
                                year = ((DateTime)dateKey).Year;

                                newDate = new DateTime(year, 1, 1, 0, 0, 0);

                                if (!dateColumnValues.Contains(newDate) && (dateKey >= minDate && dateKey <= maxDate))
                                {
                                    dateColumnValues.Add(newDate);
                                }
                                break;
                            case "months":
                                year = ((DateTime)dateKey).Year;
                                month = ((DateTime)dateKey).Month;

                                newDate = new DateTime(year, month, 1, 0, 0, 0);

                                if (!dateColumnValues.Contains(newDate) && (dateKey >= minDate && dateKey <= maxDate))
                                {
                                    dateColumnValues.Add(newDate);
                                }
                                break;
                            case "hours":
                                year = ((DateTime)dateKey).Year;
                                month = ((DateTime)dateKey).Month;
                                day = ((DateTime)dateKey).Day;
                                hour = ((DateTime)dateKey).Hour;

                                newDate = new DateTime(year, month, day, hour, 0, 0);

                                if (!dateColumnValues.Contains(newDate) && (dateKey >= minDate && dateKey <= maxDate))
                                {
                                    dateColumnValues.Add(newDate);
                                }
                                break;
                            case "days":
                                year = ((DateTime)dateKey).Year;
                                month = ((DateTime)dateKey).Month;
                                day = ((DateTime)dateKey).Day;

                                newDate = new DateTime(year, month, day, 0, 0, 0);

                                if (!dateColumnValues.Contains(newDate) && (dateKey >= minDate && dateKey <= maxDate))
                                {
                                    dateColumnValues.Add(newDate);
                                }
                                break;
                            default:
                                if (!(dateKey.HasValue) && includeMissing == false) // Note: There is no 'include missing' option in epi curve, perhaps this code should be re-visited
                                {
                                    // don't add missing values
                                }
                                else
                                {
                                    if (dateKey.Value >= minDate && dateKey.Value <= maxDate)
                                    {
                                        dateColumnValues.Add(dateKey.Value.Date);
                                    }
                                }
                                break;
                        }
                    }
                }

                string key = null;
                if (caseStatusColumn.Equals("_default_"))
                {
                    if (!statusColumnValues.Contains("true"))
                    {
                        statusColumnValues.Add("true");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(row[caseStatusColumn].ToString()))
                    {
                        key = row[caseStatusColumn].ToString();
                        if (!string.IsNullOrEmpty(key) && !statusColumnValues.Contains(key))
                        {
                            statusColumnValues.Add(key);
                        }
                    }
                }
            }

            inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_GENERATING_EPI_CURVE_TABLE);
            // create and set up the epi curve table; add the approrpiate columns
            DataTable epiCurveTable = new DataTable("EpiCurveTable");
            epiCurveTable.Columns.Add("totals", typeof(int));
            epiCurveTable.Columns.Add(dateColumn, typeof(DateTime));
            epiCurveTable.Columns.Add(caseStatusColumn, typeof(string));

            // fill in the epi curve table with one row for each value of case status, and set the "total" to zero
            foreach (DateTime? dt in dateColumnValues)
            {
                if (inputs.IsRequestCancelled())
                {
                    return null;
                }

                if (dt.HasValue)
                {
                    foreach (string s in statusColumnValues)
                    {
                        epiCurveTable.Rows.Add(0, dt.Value, s);
                    }
                }
            }

            inputs.UpdateGadgetStatus(SharedStrings.DASHBOARD_GADGET_STATUS_TOTALS_EPI_CURVE_TABLE);
            // iterate over all Rows in the filtered table and increment the appropriate "totals" cell in the epi curve table when matches are found
            foreach (DataRowView rowView in filteredView)
            {
                if (inputs.IsRequestCancelled())
                {
                    return null;
                }

                DataRow row = rowView.Row;

                // TODO: There must be a more efficient way of doing this?
                foreach (DateTime? dt in dateColumnValues)
                {
                    if (inputs.IsRequestCancelled())
                    {
                        return null;
                    }

                    DateTime? rowDateTime = null;
                    DateTime? rowOriginalDateTime = null;

                    if (row[dateColumn] != DBNull.Value)
                    {
                        rowDateTime = ((DateTime)row[dateColumn]);
                        rowOriginalDateTime = rowDateTime;
                    }
                    else
                    {
                        continue;
                    }

                    if (dt.HasValue && rowDateTime.HasValue)
                    {
                        int year = rowDateTime.Value.Year;
                        int month = rowDateTime.Value.Month;
                        int day = rowDateTime.Value.Day;
                        int hour = rowDateTime.Value.Hour;

                        switch (dateInterval.ToLower())
                        {
                            case "hours":
                                year = rowDateTime.Value.Year;
                                month = rowDateTime.Value.Month;
                                day = rowDateTime.Value.Day;
                                hour = rowDateTime.Value.Hour;
                                rowDateTime = new DateTime(year, month, day, hour, 0, 0);
                                break;
                            case "days":
                                year = rowDateTime.Value.Year;
                                month = rowDateTime.Value.Month;
                                day = rowDateTime.Value.Day;
                                rowDateTime = new DateTime(year, month, day, 0, 0, 0);
                                break;
                            case "months":
                                year = rowDateTime.Value.Year;
                                month = rowDateTime.Value.Month;
                                rowDateTime = new DateTime(year, month, 1, 0, 0, 0);
                                break;
                            case "years":
                                year = rowDateTime.Value.Year;
                                rowDateTime = new DateTime(year, 1, 1, 0, 0, 0);
                                break;
                        }
                    }

                    if (rowDateTime.HasValue && (rowOriginalDateTime.Value < minDate || rowOriginalDateTime.Value > maxDate))
                    {
                        continue;
                    }

                    if (rowDateTime.HasValue && ((DateTime)dt).Equals(((DateTime)rowDateTime)))
                    {
                        foreach (string s in statusColumnValues)
                        {
                            if (inputs.IsRequestCancelled())
                            {
                                return null;
                            }

                            if (!caseStatusColumn.Equals("_default_") && row[caseStatusColumn].ToString().Equals(s))
                            {
                                foreach (DataRow iRow in epiCurveTable.Rows)
                                {
                                    if (inputs.IsRequestCancelled())
                                    {
                                        return null;
                                    }
                                    if (iRow[dateColumn].ToString().Equals(dt.ToString()) && iRow[caseStatusColumn].Equals(s))
                                    {
                                        iRow["totals"] = int.Parse(iRow["totals"].ToString()) + 1;
                                        break;
                                    }
                                }
                            }
                            else if (caseStatusColumn.Equals("_default_"))
                            {
                                foreach (DataRow iRow in epiCurveTable.Rows)
                                {
                                    if (inputs.IsRequestCancelled())
                                    {
                                        return null;
                                    }

                                    if (iRow[dateColumn].ToString().Equals(dt.ToString()))
                                    {
                                        iRow["totals"] = int.Parse(iRow["totals"].ToString()) + 1;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return epiCurveTable;
        }

        /// <summary>
        /// Generates a data table for use in creating stacked bar charts
        /// </summary>
        /// <param name="xAxisColumn">The name of the case status column to process</param>
        /// <param name="yAxisColumn">The name of the data column to process</param>
        /// <returns>DataTable</returns>
        public DataTable GenerateStackedBarTable(string xAxisColumn, string yAxisColumn, GadgetParameters gadgetOptions)
        {
            #region Input Validation
            if (string.IsNullOrEmpty(xAxisColumn) || string.IsNullOrEmpty(yAxisColumn))
            {
                throw new ApplicationException("No columns specified for SELECT.");
            }
            #endregion // Input Validation

            //DataTable filteredTable = GenerateTable();
            //DataView dv = GenerateView(null);
            DataView dv = GenerateView(gadgetOptions);

            // Section: Check for a scenario where no records may exist due to the user's choice of selection conditions
            if (dv == null || dv.Count <= 0)
            {
                Epi.Windows.MsgBox.ShowInformation(SharedStrings.NO_RECORDS_SELECTED);
                return null;
            }

            List<string> xAxisValues = new List<string>();
            List<string> yAxisValues = new List<string>();

            // find all distinct date values and case status values
            foreach (DataRowView rowView in dv)
            {
                DataRow row = rowView.Row;

                string key = null;

                if (!string.IsNullOrEmpty(row[xAxisColumn].ToString()))
                {
                    key = row[xAxisColumn].ToString();
                    if (!string.IsNullOrEmpty(key) && !xAxisValues.Contains(key))
                    {
                        xAxisValues.Add(key);
                    }
                }

                key = null;

                if (!string.IsNullOrEmpty(row[yAxisColumn].ToString()))
                {
                    key = row[yAxisColumn].ToString();

                    if (!string.IsNullOrEmpty(key) && !yAxisValues.Contains(key))
                    {
                        yAxisValues.Add(key);
                    }
                }
            }

            if (IsUsingEpiProject)
            {
                if (yAxisValues.Count == 2)
                {
                    if (
                        (yAxisValues[0] == "False" && yAxisValues[1] == "True") ||
                        (IsColumnYesNo(yAxisColumn) && yAxisValues[0] == "0" && yAxisValues[1] == "1")
                    )
                    {
                        string temp = yAxisValues[0];
                        yAxisValues[0] = yAxisValues[1];
                        yAxisValues[1] = temp;
                    }
                }
                if (xAxisValues.Count == 2)
                {
                    if (
                        (xAxisValues[0] == "False" && xAxisValues[1] == "True") ||
                        (IsColumnYesNo(xAxisColumn) && xAxisValues[0] == "0" && xAxisValues[1] == "1")
                    )
                    {
                        string temp = xAxisValues[0];
                        xAxisValues[0] = xAxisValues[1];
                        xAxisValues[1] = temp;
                    }
                }
            }

            // create and set up the epi curve table; add the approrpiate columns
            DataTable stackedBarTable = new DataTable("StackedBarTable");
            stackedBarTable.Columns.Add("totals", typeof(int));
            stackedBarTable.Columns.Add(xAxisColumn, typeof(string));
            stackedBarTable.Columns.Add(yAxisColumn, typeof(string));

            // fill in the epi curve table with one row for each value of case status, and set the "total" to zero
            foreach (string xs in xAxisValues)
            {
                foreach (string ys in yAxisValues)
                {
                    stackedBarTable.Rows.Add(0, xs, ys);
                }
            }

            // iterate over all Rows in the filtered table and increment the appropriate "totals" cell in the epi curve table when matches are found
            foreach (DataRowView rowView in dv)
            {
                DataRow row = rowView.Row;
                // TODO: There must be a more efficient way of doing this?
                foreach (string xs in xAxisValues)
                {
                    if (row[xAxisColumn].ToString() == xs.ToString())
                    {
                        foreach (string ys in yAxisValues)
                        {
                            if (row[yAxisColumn].ToString().Equals(ys))
                            {
                                foreach (DataRow iRow in stackedBarTable.Rows)
                                {
                                    if (iRow[xAxisColumn].ToString().Equals(xs.ToString()) && iRow[yAxisColumn].Equals(ys))
                                    {
                                        iRow["totals"] = int.Parse(iRow["totals"].ToString()) + 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (IsUsingEpiProject)
            {
                foreach (DataRow row in stackedBarTable.Rows)
                {
                    if (row[2].ToString().ToLower().Equals("true") || (IsColumnYesNo(yAxisColumn) && row[2].ToString().Equals("1")))
                    {
                        row[2] = Config.Settings.RepresentationOfYes;
                    }
                    else if (row[2].ToString().ToLower().Equals("false") || (IsColumnYesNo(yAxisColumn) && row[2].ToString().Equals("0")))
                    {
                        row[2] = Config.Settings.RepresentationOfNo;
                    }

                    if (row[1].ToString().ToLower().Equals("true") || (IsColumnYesNo(xAxisColumn) && row[1].ToString().Equals("1")))
                    {
                        row[1] = Config.Settings.RepresentationOfYes;
                    }
                    else if (row[1].ToString().ToLower().Equals("false") || (IsColumnYesNo(xAxisColumn) && row[1].ToString().Equals("0")))
                    {
                        row[1] = Config.Settings.RepresentationOfNo;
                    }
                }
            }

            return stackedBarTable;
        }

        /// <summary>
        /// Generates a record count
        /// </summary>
        /// <param name="useFilters">Forces the use of data filtering. Note that for large data sets, the filtering may cost more in terms of computation time.</param>
        //public void GenerateRecordCount(bool useFilters)
        //{
        //    lock (syncLock)
        //    {
        //        if (ds != null && ds.Tables.Count >= 1 && ds.Tables[0].DefaultView != null)
        //        {
        //            ds.Tables[0].DefaultView.RowFilter = GenerateFilterCriteria();
        //            RecordCount = ds.Tables[0].DefaultView.Count;
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrEmpty(this.CustomQuery))
        //            {
        //                GenerateView();
        //                return;
        //            }

        //            if (useFilters)
        //            {
        //                // Get the first column in the data set and use that, that way we're not potentially pulling in millions 
        //                // of records and thousands of columns just to get the filtered count
        //                ColumnDataType columnDataType = ColumnDataType.Boolean | ColumnDataType.Numeric | ColumnDataType.Text | ColumnDataType.DateTime;
        //                List<string> allFieldNames = GetFieldsAsList(columnDataType);
        //                List<string> columnNames = new List<string>();

        //                string filterCriteria = GenerateFilterCriteria();

        //                // DataFilters.Count == 0 &&  
        //                if (allFieldNames.Count > 0 && !UseAdvancedUserDataFilter)
        //                {
        //                    columnNames.Add(allFieldNames[0]);
        //                }
        //                else
        //                {
        //                    foreach (string columnName in allFieldNames)
        //                    {
        //                        if (filterCriteria.ToLower().Contains(columnName.ToLower()))
        //                        {
        //                            columnNames.Add(columnName);
        //                        }
        //                    }
        //                }

        //                // TODO: Make this better
        //                DataTable unfilteredTable = GenerateRecordCountTable(columnNames);
        //                RecordCount = unfilteredTable.Select(filterCriteria).Length;
        //            }
        //            else
        //            {
        //                if (IsUsingEpiProject)
        //                {
        //                    switch (RecordProcessScope)
        //                    {
        //                        case RecordProcessingScope.Both:
        //                            RecordCount = ((int)Database.ExecuteScalar(Database.CreateQuery("SELECT Count(*) " + view.FromViewSQL + "")));
        //                            break;
        //                        case RecordProcessingScope.Deleted:
        //                            RecordCount = ((int)Database.ExecuteScalar(Database.CreateQuery("SELECT Count(*) " + view.FromViewSQL + " WHERE RECSTATUS = 0")));
        //                            break;
        //                        case RecordProcessingScope.Undeleted:
        //                        default:
        //                            RecordCount = ((int)Database.ExecuteScalar(Database.CreateQuery("SELECT Count(*) FROM " + AddBracketsToString(view.TableName) + " WHERE RECSTATUS > 0")));
        //                            break;
        //                    }

        //                }
        //                else
        //                {
        //                    RecordCount = ((int)Database.ExecuteScalar(Database.CreateQuery("Select Count(*) from " + FormatTableName(TableName) + "")));
        //                }
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Finds the upper and lower numeric values for a given date column
        /// </summary>
        /// <param name="numberColumn">The column name to process</param>
        /// <param name="minNum">The minNum to be passed by reference back to the calling method</param>
        /// <param name="maxNum">The maxNum to be passed by reference back to the calling method</param>
        public void FindUpperLowerNumericValues(string numberColumn, ref decimal? minNum, ref decimal? maxNum)
        {
            #region Input Validation
            if (string.IsNullOrEmpty(numberColumn))
            {
                Epi.Windows.MsgBox.ShowError("No column selected.");
                return;
            }
            #endregion // Input Validation

            List<string> columnNames = new List<string>();
            columnNames.Add(numberColumn);

            DataTable filteredTable = GenerateTable(columnNames);
            if (filteredTable == null)
            {
                return;
            }

            string filterCriteria = GenerateFilterCriteria();
            string columnType = GetColumnType(numberColumn);
            try
            {
                if (columnType.Equals("System.Int32") || columnType.Equals("System.Int16") || columnType.Equals("System.Int64"))
                {
                    minNum = (int)filteredTable.Compute(string.Format("min({0})", AddBracketsToString(numberColumn)), filterCriteria);
                    maxNum = (int)filteredTable.Compute(string.Format("max({0})", AddBracketsToString(numberColumn)), filterCriteria);
                }
                else if (columnType.Equals("System.Decimal"))
                {
                    minNum = (decimal)filteredTable.Compute(string.Format("min({0})", AddBracketsToString(numberColumn)), filterCriteria);
                    maxNum = (decimal)filteredTable.Compute(string.Format("max({0})", AddBracketsToString(numberColumn)), filterCriteria);
                }
                else
                {
                    minNum = Convert.ToDecimal(filteredTable.Compute(string.Format("min({0})", AddBracketsToString(numberColumn)), filterCriteria));
                    maxNum = Convert.ToDecimal(filteredTable.Compute(string.Format("max({0})", AddBracketsToString(numberColumn)), filterCriteria));
                }
            }
            catch (InvalidCastException)
            {
                minNum = null;
                maxNum = null;
            }
        }

        /// <summary>
        /// Finds the upper and lower date values for a given date column
        /// </summary>
        /// <param name="dateColumn">The column name to process</param>
        /// <param name="minDate">The minDate to be passed by reference back to the calling method</param>
        /// <param name="maxDate">The maxDate to be passed by reference back to the calling method</param>
        public void FindUpperLowerDateValues(string dateColumn, ref DateTime? minDate, ref DateTime? maxDate, GadgetParameters gadgetOptions = null)
        {
            #region Input Validation
            if (string.IsNullOrEmpty(dateColumn))
            {
                Epi.Windows.MsgBox.ShowError("No column selected.");
                return;
            }
            #endregion // Input Validation

            List<string> columnNames = new List<string>();
            columnNames.Add(dateColumn);

            if (gadgetOptions != null)
            {
                EwavConstructTableColumnNames(gadgetOptions);
            }

            DataView dv = GenerateView(gadgetOptions);

            if (dv.ToTable().Rows.Count == 0)
            {
                return;
            }

            try
            {
                //dv.RowFilter = string.Format("{0} is not null", AddBracketsToString(dateColumn));
                dv.RowFilter = CombineFilters(AddBracketsToString(dateColumn) + " is not null", dv);
                dv.Sort = AddBracketsToString(dateColumn);
                if (dv.ToTable().Rows.Count == 0)
                {
                    return;
                }
                minDate = Convert.ToDateTime(dv[0].Row[dateColumn]);

                dv.Sort = string.Format("{0} DESC", AddBracketsToString(dateColumn));
                maxDate = Convert.ToDateTime(dv[0].Row[dateColumn]);

                if (minDate.HasValue)
                {
                    int year = minDate.Value.Year;
                    int month = minDate.Value.Month;
                    int day = minDate.Value.Day;
                    int hour = minDate.Value.Hour;
                    minDate = new DateTime(year, month, day, hour, 0, 0);
                }

                if (maxDate.HasValue)
                {
                    int year = maxDate.Value.Year;
                    int month = maxDate.Value.Month;
                    int day = maxDate.Value.Day;
                    int hour = maxDate.Value.Hour;
                    int minute = 0;

                    if (maxDate.Value.Minute > 0 || maxDate.Value.Second > 0 || maxDate.Value.Millisecond > 0)
                    {
                        minute = 59;
                    }

                    maxDate = new DateTime(year, month, day, hour, minute, 0);
                }
            }
            catch (InvalidCastException)
            {
                minDate = null;
                maxDate = null;
            }
            //string filterCriteria = GenerateFilterCriteria();
            //try
            //{
            //    minDate = Convert.ToDateTime(unfilteredTable.Compute("min(" + AddBracketsToString(dateColumn) + ")", filterCriteria));
            //    maxDate = Convert.ToDateTime(unfilteredTable.Compute("max(" + AddBracketsToString(dateColumn) + ")", filterCriteria));
            //    if (minDate.HasValue)
            //    {
            //        int year = minDate.Value.Year;
            //        int month = minDate.Value.Month;
            //        int day = minDate.Value.Day;
            //        int hour = minDate.Value.Hour;
            //        minDate = new DateTime(year, month, day, hour, 0, 0);
            //    }
            //    if (maxDate.HasValue)
            //    {
            //        int year = maxDate.Value.Year;
            //        int month = maxDate.Value.Month;
            //        int day = maxDate.Value.Day;
            //        int hour = maxDate.Value.Hour;
            //        int minute = 0;
            //        if (maxDate.Value.Minute > 0 || maxDate.Value.Second > 0 || maxDate.Value.Millisecond > 0)
            //        {
            //            minute = 59;
            //        }
            //        maxDate = new DateTime(year, month, day, hour, minute, 0);
            //    }
            //}
            //catch (InvalidCastException)
            //{
            //    minDate = null;
            //    maxDate = null;
            //}
        }

        /// <summary>
        /// Determines whether or not a column name is valid
        /// </summary>
        /// <param name="columnName">The name of the column</param>
        /// <returns>Whether the column name is valid</returns>
        public bool ValidateColumnName(string columnName)
        {
            bool isValid = true;

            for (int i = 0; i < columnName.Length; i++)
            {
                string columnNameChar = columnName.Substring(i, 1);
                System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(columnNameChar, "[A-Za-z0-9_]");
                if (!m.Success)
                {
                    return false;
                }
            }

            return isValid;
        }

        #endregion // Public Methods

        #region Private Methods

        /// <summary>
        /// Converts the related data connections into XML
        /// </summary>
        /// <param name="doc">The Xml document to modify</param>
        /// <returns>XmlElement</returns>
        private XmlElement SerializeRelatedDataConnections(XmlDocument doc)
        {
            System.Xml.XmlElement relatedDataConnections = doc.CreateElement("relatedDataConnections");

            foreach (RelatedConnection rConn in this.ConnectionsForRelate)
            {
                System.Xml.XmlElement relatedDataConnection = doc.CreateElement("relatedDataConnection");
                string xmlString = string.Empty;
                if (rConn.IsEpiInfoProject)
                {
                    xmlString =
                        string.Format("<projectPath>{0}</projectPath><viewName>{1}</viewName><connectionString>{2}</connectionString><tableName>{3}</tableName>", rConn.view.Project.FilePath, rConn.view.Name, string.Empty, string.Empty);
                }
                else
                {
                    xmlString =
                        string.Format("<projectPath>{0}</projectPath><viewName>{1}</viewName><connectionString>{2}</connectionString><tableName>{3}</tableName>", string.Empty, string.Empty, Configuration.Encrypt(rConn.db.ConnectionString), rConn.TableName);
                }
                xmlString = string.Format("{0}<parentKeyField>{1}</parentKeyField>", xmlString, rConn.ParentKeyField);
                xmlString = string.Format("{0}<childKeyField>{1}</childKeyField>", xmlString, rConn.ChildKeyField);
                xmlString = string.Format("{0}<useUnmatched>{1}</useUnmatched>", xmlString, rConn.UseUnmatched);
                xmlString = string.Format("{0}<sameDataSource>{1}</sameDataSource>", xmlString, rConn.SameDataSource);
                relatedDataConnection.InnerXml = xmlString;
                relatedDataConnections.AppendChild(relatedDataConnection);
            }

            return relatedDataConnections;
        }

        /// <summary>
        /// Converts the data filter conditions into XML
        /// </summary>
        /// <param name="doc">The Xml document to modify</param>
        /// <returns>XmlElement</returns>
        private XmlElement SerializeFilters(XmlDocument doc)
        {
            //        return DataFilters.Serialize(doc);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the rules into XML
        /// </summary>
        /// <param name="doc">The Xml document to modify</param>
        /// <returns>XmlElement</returns>
        private XmlElement SerializeRules(XmlDocument doc)
        {
            return Rules.Serialize(doc);
        }

        /// <summary>
        /// Construct
        /// </summary>
        private void Construct()
        {
            this.Rules = new DashboardRules(this);
            this.RecordCount = 0;
            //    this.RecordProcessScope = RecordProcessingScope.Undeleted;
            this.ConnectionsForRelate = new List<RelatedConnection>();
            this.ConnectionsForMerge = new List<MergeConnection>();
            this.userVarsNeedUpdating = true;

            this.Config = new Configuration();  //  EwavTest.Web.Configuration.GetNewInstance();
            this.DataSet = new DataSet();

            FieldTable = new DataTable("fieldTable");
            FieldTable.Columns.Add(new DataColumn("columnname", typeof(string)));
            FieldTable.Columns.Add(new DataColumn("datatype", typeof(string)));
            FieldTable.Columns.Add(new DataColumn("tablename", typeof(string)));
            FieldTable.Columns.Add(new DataColumn("formname", typeof(string)));
            FieldTable.Columns.Add(new DataColumn("epifieldtype", typeof(Field)));
            DataColumn[] keyColumns = new DataColumn[1];
            keyColumns[0] = FieldTable.Columns[0];
            FieldTable.PrimaryKey = keyColumns;

            this.TableColumnNames = new Dictionary<string, string>(); // This dictionary stores the table column names and their associated data type. 
            //    ConstructTableColumnNames();
        }

        /// <summary>
        /// Construct table column name data structure
        /// </summary>
        private void ConstructTableColumnNames()
        {
            if (this.View != null || !string.IsNullOrEmpty(TableName))
            {
                TableColumnNames = new Dictionary<string, string>();
                FieldTable.Rows.Clear();
                FieldTable.Rows.Clear();

                DataTable parentTable = new DataTable();
                if (IsUsingEpiProject)
                {
                    parentTable = Database.GetTopTwoTable(View.TableName);
                    AddPermanentVariablesToTable(parentTable);
                    AddSystemVariablesToTable(parentTable);

                    if (!TableColumnNames.ContainsKey("UniqueKey"))
                    {
                        TableColumnNames.Add("UniqueKey", "System.Int32");
                        FieldTable.Rows.Add("UniqueKey", "System.Int32", View.TableName, View.Name, null);
                    }

                    if (!TableColumnNames.ContainsKey("RECSTATUS") && !TableColumnNames.ContainsKey("RecStatus"))
                    {
                        TableColumnNames.Add("RecStatus", "System.Int32");
                        FieldTable.Rows.Add("RecStatus", "System.Int32", View.TableName, View.Name, null);
                    }

                    foreach (Page page in View.Pages)
                    {
                        DataTable pageTable = Database.GetTopTwoTable(page.TableName);

                        foreach (DataColumn dc in pageTable.Columns)
                        {
                            if (!TableColumnNames.ContainsKey(dc.ColumnName))
                            {
                                TableColumnNames.Add(dc.ColumnName, dc.DataType.ToString());
                                if (View.Fields.Contains(dc.ColumnName))
                                {
                                    FieldTable.Rows.Add(dc.ColumnName, dc.DataType.ToString(), page.TableName, View.Name, View.Fields[dc.ColumnName]);
                                }
                            }
                            //fieldTable.Rows.Add(dc.ColumnName, dc.DataType.ToString(), page.TableName, false);
                        }
                        RelateInto(parentTable, pageTable, "GlobalRecordId", "GlobalRecordId", false);
                    }

                    // Get group fields too
                    foreach (Field field in View.Fields)
                    {
                        if (field is GroupField && !TableColumnNames.ContainsKey(field.Name))
                        {
                            FieldTable.Rows.Add(field.Name, string.Empty, string.Empty, View.Name, field as GroupField);
                            TableColumnNames.Add(field.Name, field.GetType().ToString());
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(this.CustomQuery))
                    {
                        // Read the top-two table once and place the fields names and their types into the column name dictionary.
                        // This way, we avoid having to query the DB over and over when we want to get a list of fields.
                        parentTable = GenerateTopTwoTable();
                    }
                    else
                    {
                        // can't do a top two table if the user has custom SQL, so do the whole thing. 
                        // This has the benefit of populating the data set while it's doing this, but the downside
                        // is that you'll wait on it until it's done to do anything with the UI since you can't work 
                        // with gadgets if you don't have column names.
                        parentTable = GenerateTable();
                    }
                    foreach (DataColumn dc in parentTable.Columns)
                    {
                        if (!TableColumnNames.ContainsKey(dc.ColumnName))
                        {
                            TableColumnNames.Add(dc.ColumnName, dc.DataType.ToString());
                            FieldTable.Rows.Add(dc.ColumnName, dc.DataType.ToString(), TableName, string.Empty, null);
                        }
                        //fieldTable.Rows.Add(dc.ColumnName, dc.DataType.ToString(), TableName, false);                        
                    }
                }

                if (ConnectionsForRelate.Count > 0)
                {
                    foreach (RelatedConnection conn in ConnectionsForRelate)
                    {
                        DataTable relatedTable = new DataTable();

                        if (conn.IsEpiInfoProject)
                        {
                            relatedTable = conn.db.Select(conn.db.CreateQuery(string.Format("SELECT TOP 2 * {0}", conn.view.FromViewSQL)));

                            foreach (Page page in conn.view.Pages)
                            {
                                string pageGUIDName = string.Format("{0}.GlobalRecordId", page.TableName);
                                if (relatedTable.Columns.Contains(pageGUIDName))
                                {
                                    relatedTable.Columns.Remove(pageGUIDName);
                                }
                            }
                        }
                        else
                        {
                            relatedTable = conn.db.GetTopTwoTable(conn.TableName);
                        }

                        RelateInto(parentTable, relatedTable, conn.ParentKeyField, conn.ChildKeyField, conn.UseUnmatched);

                        if (parentTable.Columns.Contains("RECSTATUS"))
                        {
                            parentTable.Columns["RECSTATUS"].ColumnName = "RecStatus";
                        }

                        foreach (DataColumn dc in parentTable.Columns)
                        {
                            if (!TableColumnNames.ContainsKey(dc.ColumnName))
                            {
                                TableColumnNames.Add(dc.ColumnName, dc.DataType.ToString());
                                if (conn.IsEpiInfoProject)
                                {
                                    if (conn.view.Fields.Contains(dc.ColumnName))
                                    {
                                        FieldTable.Rows.Add(dc.ColumnName, dc.DataType.ToString(), string.Empty, conn.view.Name, conn.view.Fields[dc.ColumnName]);
                                    }
                                }
                                else
                                {
                                    FieldTable.Rows.Add(dc.ColumnName, dc.DataType.ToString(), conn.TableName, string.Empty, null);
                                }
                            }
                        }

                        if (conn.IsEpiInfoProject)
                        {
                            // get group fields in related connections too
                            foreach (Field field in conn.view.Fields)
                            {
                                if (field is GroupField)
                                {
                                    if (!TableColumnNames.ContainsKey(field.Name))
                                    {
                                        FieldTable.Rows.Add(field.Name, string.Empty, string.Empty, conn.view.Name, field as GroupField);
                                        TableColumnNames.Add(field.Name, field.GetType().ToString());
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (IDashboardRule rule in this.Rules)
                {
                    if (rule is DataAssignmentRule)
                    {
                        DataAssignmentRule assignRule = rule as DataAssignmentRule;

                        if (!TableColumnNames.ContainsKey(assignRule.DestinationColumnName))
                        {
                            FieldTable.Rows.Add(assignRule.DestinationColumnName, assignRule.DestinationColumnType);
                            TableColumnNames.Add(assignRule.DestinationColumnName, assignRule.DestinationColumnType);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Applies all dashboard rules to the data set, e.g. recoding, formatting, assigning, etc.
        /// </summary>
        /// <param name="view">The view on which to apply the rules</param>
        internal void ApplyDashboardRules(GadgetParameters inputs = null)
        {
            foreach (IDashboardRule rule in Rules)
            {
                rule.SetupRule(mainTable);
            }

            int count = 0;
            DataView view = new DataView(mainTable, "", "", DataViewRowState.CurrentRows);
            foreach (DataRowView rowView in view/*mainTable.DefaultView*/)
            {
                DataRow row = rowView.Row;
                foreach (IDashboardRule rule in Rules)
                {
                    rule.ApplyRule(row);
                }

                if (inputs != null)
                {
                    count++;
                    if (Rules.Count > 0 && count % 100 == 0)
                    {
                        double percent = (double)count / (double)rowView.DataView.Count;
                        inputs.UpdateGadgetStatus(string.Format(SharedStrings.DASHBOARD_GADGET_STATUS_ASSIGNED_VALUES_TO, percent.ToString("P")));
                        inputs.UpdateGadgetProgress(((double)count / (double)rowView.DataView.Count) * 100);
                    }
                }
            }

            UserVarsNeedUpdating = false;
        }

        /// <summary>
        /// Generates the appropriate Recstatus condition based on the record process scope for use inside of an SQL query
        /// </summary>
        /// <returns>String</returns>
        private string GenerateRecstatusConditionForSelect()
        {
            throw new NotImplementedException();
            //if (this.IsUsingEpiProject)
            //{
            //    if (this.RecordProcessScope == RecordProcessingScope.Undeleted)
            //    {
            //        return " where recstatus > 0";
            //    }
            //    else if (this.RecordProcessScope == RecordProcessingScope.Deleted)
            //    {
            //        return " where recstatus = 0";
            //    }
            //    else if (this.RecordProcessScope == RecordProcessingScope.Both)
            //    {
            //        return " where recstatus >= 0";
            //    }
            //}
            //return string.Empty;
        }

        /// <summary>
        /// Generates the filter criteria string based of off the user's selection conditions
        /// </summary>
        /// <returns>String representing the filter criteria</returns>
        public string GenerateFilterCriteria()
        {
            if (UseAdvancedUserDataFilter)
            {
                return AdvancedUserDataFilter;
            }
            else
            {
                //  DataFilters df = new DataFilters(this);
                if (ewavDataFilters != null)
                {
                    CreateDataFilters(this.ewavDataFilters);
                }

                if (DataFilters.Count > 0)
                {
                    return GenerateDataFilterString();
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Generates the filter criteria string based of off the user's selection conditions
        /// </summary>
        /// <returns>String representing the filter criteria</returns>
        public string GenerateFilterCriteria(IEnumerable<EwavDataFilterCondition> FilterConditions)
        {

            //  DataFilters df = new DataFilters(this);
            if (FilterConditions != null)
            {
                DataFilters.ClearFilterConditions();

                CreateDataFilters(FilterConditions);

                return GenerateDataFilterString();
            }


            return string.Empty;
        }

        public string GenerateDataFilterString()
        {
            return DataFilters.GenerateDataFilterString();
        }

        /// <summary>
        /// Creates the data filters.
        /// </summary>
        /// <param name="ewavDataFilters">The ewav data filters.</param>
        /// <returns></returns>
        public void CreateDataFilters(IEnumerable<EwavDataFilterCondition> ewavDataFilters)
        {
            bool added;
            foreach (EwavDataFilterCondition dfc in ewavDataFilters)
            {
                if (dfc.FriendLowValue != null || dfc.FriendHighValue != null)
                {
                    added = AddDataFilterCondition(this.DataFilters,
                                                dfc.FriendlyOperand.VarName,
                                                dfc.FriendLowValue.VarName,
                                                dfc.FriendHighValue.VarName,
                                                dfc.FieldName.VarName,
                                                (ConditionJoinType)Ewav.Web.Utilities.ParseEnum<ConditionJoinType>(dfc.JoinType.VarName));
                }
                else
                {
                    if (dfc.FriendlyOperand.VarName.ToUpper() == "BEGINS WITH")
                    {
                        added = AddDataFilterCondition(this.DataFilters,
                                                "is like",
                                                dfc.FriendlyValue.VarName + "%",
                                                dfc.FieldName.VarName,
                                                (ConditionJoinType)Ewav.Web.Utilities.ParseEnum<ConditionJoinType>(dfc.JoinType.VarName));
                    }
                    else if (dfc.FriendlyOperand.VarName.ToUpper() == "ENDS WITH")
                    {
                        added = AddDataFilterCondition(this.DataFilters,
                                                "is like",
                                                 "%" + dfc.FriendlyValue.VarName,
                                                dfc.FieldName.VarName,
                                                (ConditionJoinType)Ewav.Web.Utilities.ParseEnum<ConditionJoinType>(dfc.JoinType.VarName));
                    }
                    else if (dfc.FriendlyOperand.VarName.ToUpper() == "CONTAINS")
                    {
                        added = AddDataFilterCondition(this.DataFilters,
                                                "is like",
                                                 "%" + dfc.FriendlyValue.VarName + "%",
                                                dfc.FieldName.VarName,
                                                (ConditionJoinType)Ewav.Web.Utilities.ParseEnum<ConditionJoinType>(dfc.JoinType.VarName));
                    }
                    else
                    {
                        added = AddDataFilterCondition(this.DataFilters,
                            dfc.FriendlyOperand.VarName,
                            dfc.FriendlyValue.VarName,
                            dfc.FieldName.VarName,
                            (ConditionJoinType)Ewav.Web.Utilities.ParseEnum<ConditionJoinType>(dfc.JoinType.VarName));

                    }
                }
            }
        }

        public DataFilters CreateDataFiltersForUserDefVars(IEnumerable<EwavDataFilterCondition> ewavDataFilters)
        {
            bool added = false;
            DataFilters dataFiltersForUserDef = new DataFilters(this);
            foreach (EwavDataFilterCondition dfc in ewavDataFilters)
            {
                if (dfc.FriendLowValue != null || dfc.FriendHighValue != null)
                {
                    added = AddDataFilterCondition(dataFiltersForUserDef,
                        dfc.FriendlyOperand.VarName,
                        dfc.FriendLowValue.VarName,
                        dfc.FriendHighValue.VarName,
                        dfc.FieldName.VarName,
                        (ConditionJoinType)Ewav.Web.Utilities.ParseEnum<ConditionJoinType>(dfc.JoinType.VarName));
                }
                else
                {
                    //added = AddDataFilterCondition(dataFiltersForUserDef,
                    //    dfc.FriendlyOperand.VarName,
                    //    dfc.FriendlyValue.VarName,
                    //    dfc.FieldName.VarName,
                    //    (ConditionJoinType)Ewav.Web.Utilities.ParseEnum<ConditionJoinType>(dfc.JoinType.VarName));

                    if (dfc.FriendlyOperand.VarName.ToUpper() == "BEGINS WITH")
                    {
                        added = AddDataFilterCondition(dataFiltersForUserDef,
                                                "is like",
                                                dfc.FriendlyValue.VarName + "%",
                                                dfc.FieldName.VarName,
                                                (ConditionJoinType)Ewav.Web.Utilities.ParseEnum<ConditionJoinType>(dfc.JoinType.VarName));
                    }
                    else if (dfc.FriendlyOperand.VarName.ToUpper() == "ENDS WITH")
                    {
                        added = AddDataFilterCondition(dataFiltersForUserDef,
                                                "is like",
                                                 "%" + dfc.FriendlyValue.VarName,
                                                dfc.FieldName.VarName,
                                                (ConditionJoinType)Ewav.Web.Utilities.ParseEnum<ConditionJoinType>(dfc.JoinType.VarName));
                    }
                    else if (dfc.FriendlyOperand.VarName.ToUpper() == "CONTAINS")
                    {
                        added = AddDataFilterCondition(dataFiltersForUserDef,
                                                "is like",
                                                 "%" + dfc.FriendlyValue.VarName + "%",
                                                dfc.FieldName.VarName,
                                                (ConditionJoinType)Ewav.Web.Utilities.ParseEnum<ConditionJoinType>(dfc.JoinType.VarName));
                    }
                    else
                    {
                        added = AddDataFilterCondition(dataFiltersForUserDef,
                            dfc.FriendlyOperand.VarName,
                            dfc.FriendlyValue.VarName,
                            dfc.FieldName.VarName,
                            (ConditionJoinType)Ewav.Web.Utilities.ParseEnum<ConditionJoinType>(dfc.JoinType.VarName));

                    }
                }
            }

            //if (added)
            //{
            return dataFiltersForUserDef;
            //}
            //else
            //{
            //    throw new Exception("Data filters exception");
            //}

        }

        #region ANOVA
        /// <summary>
        /// Converts a data table to a list of lists for use in Kruskal-Wallis statistics
        /// </summary>
        /// <param name="table">The table to convert</param>
        /// <returns>The data contained within the table represented as a list of lists</returns>
        private List<List<object>> ConvertTableToListOfLists(DataTable table)
        {
            List<List<object>> columns = new List<List<object>>();

            //table.Columns.Remove(table.Columns[0].ColumnName);

            DataTable dt = table.Clone();

            foreach (DataRow row in table.Rows)
            {
                dt.Rows.Add(row.ItemArray);
            }

            dt.Columns.Remove(dt.Columns[0]);

            int i = 0;

            //Debug.Print("------------");
            //Debug.Print("TEST");
            //Debug.Print("------------");

            foreach (DataRow row in dt.Rows)
            {
                bool success = false;
                string line = string.Empty;
                double value = 0;
                foreach (object dstr in row.ItemArray)
                {
                    success = double.TryParse(dstr.ToString(), out value);

                    if (!success)
                    {
                        break;
                    }
                    line = string.Format("{0}{1}", line, dstr.ToString());
                }
                //Debug.Print(line);

                bool zeroSuccess = double.TryParse(line, out value);

                if (zeroSuccess && value == 0)
                {
                    success = false;
                }

                if (success)
                {
                    columns.Add(row.ItemArray.ToList());
                }
                else
                {
                }

                i++;
            }

            return columns;
        }

        /// <summary>
        /// CalculateSSBetween
        /// </summary>
        /// <param name="grandMean">The grand mean</param>
        /// <param name="observations">The list of observations</param>
        /// <param name="avgs">The list of averages</param>
        /// <returns>The SS between value</returns>
        private double CalculateSSBetween(double grandMean, List<double> observations, List<double> avgs)
        {
            double retval = 0;
            for (int x = 0; x < observations.Count; x++)
            {
                retval += observations[x] * (Math.Pow(avgs[x] - grandMean, 2));
            }
            return retval;
        }

        /// <summary>
        /// CalculateSSWithin
        /// </summary>
        /// <param name="observations">The list of observations</param>
        /// <param name="vars">The list of variances</param>
        /// <returns>The SS within value</returns>
        private double CalculateSSWithin(List<double> observations, List<double> vars)
        {
            double retval = 0;
            for (int x = 0; x < observations.Count; x++)
            {
                retval += (observations[x] - 1) * vars[x];
            }
            return retval;
        }

        /// <summary>
        /// CalculateChiSquare
        /// </summary>
        /// <param name="dfWithin">The degrees of freedom within</param>
        /// <param name="pooledVariance">The pooled variance</param>
        /// <param name="observations">The list of observations</param>
        /// <param name="vars">The list of variances</param>
        /// <returns>Chi square value</returns>
        private double CalculateChiSquare(double dfWithin, double pooledVariance, List<double> observations, List<double> vars)
        {
            double denominator = 0;
            double result = 0;

            for (int j = 0; j < observations.Count; j++)
            {
                if ((observations[j] - 1 != 0) && (vars[j] != 0))
                {
                    denominator += 1.0 / (observations[j] - 1);
                    result += (observations[j] - 1) * Math.Log(vars[j]);
                }
                else
                {
                    denominator = 0;
                    result = 0;
                }
            }

            denominator = 1.0 + (1.0 / (3.0 * (observations.Count - 1))) * (denominator - 1.0 / dfWithin);

            if ((denominator != 0) && (pooledVariance != 0))
            {
                result = (1.0 / denominator) * (dfWithin * Math.Log(pooledVariance) - result);
            }

            return result;
        }

        /// <summary>
        /// Calculates a Kruskal-Wallis H value when doing a cross-tabulated MEANS.
        /// </summary>
        /// <param name="freqHorizontal">The frequencies of just the horizontal variable</param>
        /// <param name="freqVertical">The frequencies of just the vertical variable</param>
        /// <param name="allLocalFreqs">A list of lists of all the inner table cells of the cross-tabulation</param>
        /// <param name="recordCount">The record count</param>
        /// <returns>Double representation of the KW H-value</returns>
        private double CalculateKruskalWallisH(DataTable freqHorizontal, DataTable freqVertical, List<List<object>> allLocalFreqs, double recordCount)
        {
            double cf = 0;
            double avgr = 0;
            int greaterSize;
            if (freqHorizontal.Rows.Count > freqVertical.Rows.Count)
            {
                greaterSize = freqHorizontal.Rows.Count;
            }
            else
            {
                greaterSize = freqVertical.Rows.Count;
            }
            double[] sr = new double[greaterSize];
            double adj = 0;
            double H = 0;

            for (int i = 0; i < allLocalFreqs.Count; i++)
            {
                double totalHFreq = (double)freqHorizontal.Rows[i]["freq"];
                cf += totalHFreq;
                avgr = cf - (totalHFreq - 1) / 2.0;
                for (int l = 0; l < allLocalFreqs[0].Count; l++)
                {
                    sr[l] += (double)allLocalFreqs[i][l] * avgr;
                }
                adj += totalHFreq * (Math.Pow(totalHFreq, 2) - 1);
            }

            for (int i = 0; i < freqVertical.Rows.Count; i++)
            {
                double totalVFreq = (double)freqVertical.Rows[i]["freq"];
                if (totalVFreq != 0)
                {
                    H += sr[i] * sr[i] / totalVFreq;
                }
            }

            H = H * 12 / (recordCount * (recordCount + 1)) - 3 * (recordCount + 1);
            H = H / (1 - adj / (Math.Pow(recordCount, 3) - recordCount));

            return H;
        }

        #endregion // ANOVA

        /// <summary>
        /// Sorts a data table from highest value to lowest value
        /// </summary>
        /// <param name="table">The table to sort</param>
        /// <param name="column">The column by which to sort</param>
        /// <param name="placeMissingAtBottom">Whether or not to place missing values at the bottom of the table regardless of the count</param>
        private DataTable SortHighToLow(DataTable table, DataColumn column)
        {
            DataView sortedView = new DataView(table, "", string.Format("{0} DESC", AddBracketsToString(column.ColumnName)), DataViewRowState.CurrentRows);

            if (sortedView == null || sortedView.Count <= 0)
            {
                return null;
            }

            return sortedView.ToTable();
        }

        /// <summary>
        /// Sorts a data table by a single column
        /// </summary>
        /// <param name="table">The table to sort</param>
        /// <param name="column">The column by which to sort</param>
        private DataTable SortBySingleColumn(DataTable table, DataColumn column, SortOrder sort = SortOrder.Ascending)
        {
            #region Input Validation
            if (table.Rows.Count == 0)
            {
                return table;
            }
            #endregion // Input Validation

            if (table.Rows.Count == 1)
            {
                return table; // nothing to sort!
            }

            string[] abbrMonthNames = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames;
            string[] abbrDayNames = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames;
            string[] monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            string[] dayNames = CultureInfo.CurrentCulture.DateTimeFormat.DayNames;
            string[] yesNoValues = new string[3];
            // WPF_TO_SL  These must ultimaely be converted back to readihg the
            // Configuration object, whatever FormatException that object takes 
            yesNoValues[0] = Ewav.Web.Config.ConfigDataSet.RepresentationOfYes.ToLower();//"yes"; //config.Settings.RepresentationOfYes;
            yesNoValues[1] = Ewav.Web.Config.ConfigDataSet.RepresentationOfNo.ToLower();// "no"; //config.Settings.RepresentationOfNo;
            yesNoValues[2] = Ewav.Web.Config.ConfigDataSet.RepresentationOfMissing.ToLower();// "missing";  // config.Settings.RepresentationOfMissing;
            string[] values = null;

            DataRow[] rowArray = null;
            DataTable sortedTable = new DataTable(table.TableName);
            DataTable unsortedTable = new DataTable(table.TableName);
            unsortedTable = table.Copy();

            string sortColumnName = column.ColumnName;

            string compareValue = table.Rows[0][column.ColumnName].ToString().Trim();
            if (string.IsNullOrEmpty(compareValue))
            {
                compareValue = table.Rows[1][column.ColumnName].ToString().Trim();
            }

            if (values == null)
            {
                foreach (string s in abbrMonthNames)
                {
                    if (s.Equals(compareValue))
                    {
                        values = abbrMonthNames;
                        break;
                    }
                }
            }

            if (values == null)
            {
                foreach (string s in monthNames)
                {
                    if (s.Equals(compareValue))
                    {
                        values = monthNames;
                        break;
                    }
                }
            }

            if (values == null)
            {
                foreach (string s in abbrDayNames)
                {
                    if (s.Equals(compareValue))
                    {
                        values = abbrDayNames;
                        break;
                    }
                }
            }

            if (values == null)
            {
                foreach (string s in dayNames)
                {
                    if (s.Equals(compareValue))
                    {
                        values = dayNames;
                        break;
                    }
                }
            }

            if (values == null)
            {
                // WPF_TO_SL  
                if (Ewav.Web.Config.ConfigDataSet.RepresentationOfYes.Equals(compareValue) ||
                    Ewav.Web.Config.ConfigDataSet.RepresentationOfNo.Equals(compareValue) ||
                    Ewav.Web.Config.ConfigDataSet.RepresentationOfMissing.Equals(compareValue))
                {
                    values = yesNoValues;
                }
            }

            if (values == null && IsUserDefinedColumn(column.ColumnName))
            {
                foreach (IDashboardRule rule in Rules)
                {
                    if (rule is Rule_Recode)
                    {
                        Rule_Recode recodeRule = rule as Rule_Recode;

                        if (recodeRule.DestinationColumnName.ToLower().Equals(column.ColumnName.ToLower()) && !recodeRule.DestinationColumnName.Equals("Yes/No") && recodeRule.ShouldMaintainSortOrder)
                        {
                            if (string.IsNullOrEmpty(recodeRule.ElseValue))
                            {
                                values = new string[recodeRule.RecodeInputTable.Rows.Count];
                            }
                            else
                            {
                                values = new string[recodeRule.RecodeInputTable.Rows.Count + 1];
                            }

                            for (int i = 0; i < recodeRule.RecodeInputTable.Rows.Count; i++)
                            {
                                DataRow row = recodeRule.RecodeInputTable.Rows[i];
                                values[i] = row[recodeRule.RecodeInputTable.Columns.Count - 1].ToString();
                            }

                            if (!string.IsNullOrEmpty(recodeRule.ElseValue))
                            {
                                values[recodeRule.RecodeInputTable.Rows.Count] = recodeRule.ElseValue;
                            }

                            break;
                        }
                    }
                    else if (rule is Rule_Format && (((Rule_Format)rule).FormatType.Equals(FormatTypes.RegularDate) || ((Rule_Format)rule).FormatType.Equals(FormatTypes.LongDate) || ((Rule_Format)rule).FormatType.Equals(FormatTypes.MonthAndFourDigitYear)))
                    {
                        return unsortedTable;
                    }
                }
            }

            string sortClause = " ASC";
            if (sort.Equals(SortOrder.Descending))
            {
                sortClause = " DESC";
            }

            if (values != null)
            {
                DataColumn dc = new DataColumn("___sortvalue___", typeof(int));
                unsortedTable.Columns.Add(dc);

                foreach (DataRow row in unsortedTable.Rows)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        string s = values[i];

                        if (s.Equals(row[column.ColumnName].ToString().Trim()))
                        {
                            row["___sortvalue___"] = i;
                        }
                    }
                }
                rowArray = unsortedTable.Select(string.Format("{0} is not null", AddBracketsToString(column.ColumnName)), string.Format("___sortvalue___ {0}, {1} {2}", sortClause, AddBracketsToString(sortColumnName), sortClause));
            }
            else
            {
                rowArray = unsortedTable.Select(string.Format("{0} is not null", AddBracketsToString(column.ColumnName)), string.Format("{0}{1}", AddBracketsToString(sortColumnName), sortClause));
            }

            if (sortedTable.Columns.Contains("___sortvalue___"))
            {
                sortedTable.Columns.Remove("___sortvalue___");
            }

            if (unsortedTable.Columns.Contains("___sortvalue___"))
            {
                unsortedTable.Columns.Remove("___sortvalue___");
            }

            DataRow[] missingRowArray = new DataRow[1];
            missingRowArray = unsortedTable.Select(string.Format("{0} is null", AddBracketsToString(column.ColumnName)), string.Empty);

            sortedTable = rowArray.CopyToDataTable().DefaultView.ToTable(table.TableName);

            if (missingRowArray.Length == 1)
            {
                sortedTable.Rows.Add(missingRowArray[0].ItemArray);
            }

            return sortedTable;
        }

        /// <summary>
        /// Adds leading and trailing brackets to a given string
        /// </summary>
        /// <param name="s">The string to add brackets to</param>        
        /// <returns>The original string with leading and trailing brackets</returns>
        private string AddBracketsToString(string s)
        {
            #region GetFactory Description
            /* Used to standardize the adding of brackets to a string. Necessary to work with non-Epi 7 data
            * sources like Excel documents where column names may have spaces or non-standard characters.
            */
            #endregion // GetFactory Description
            return string.Format("{0}{1}{2}", stringLiterals.LEFT_SQUARE_BRACKET, s, stringLiterals.RIGHT_SQUARE_BRACKET);
        }

        /// <summary>
        /// Converts a column from its current type to the new desired type
        /// </summary>
        /// <param name="dtDataSources">The table to use</param>
        /// <param name="dc">The column to convert</param>
        /// <param name="genericColumnType">The destination column type</param>
        private void ConvertColumnToType(DataTable dt, DataColumn dc, GenericDbColumnType genericColumnType)
        {
            DataColumn convertedColumn = new DataColumn(string.Format("_____{0}_____5334", dc.ColumnName));

            switch (genericColumnType)
            {
                case GenericDbColumnType.String:
                    convertedColumn.DataType = typeof(string);
                    break;
                case GenericDbColumnType.Int16:
                    convertedColumn.DataType = typeof(Int16);
                    break;
                case GenericDbColumnType.Int32:
                    convertedColumn.DataType = typeof(int);
                    break;
                case GenericDbColumnType.Double:
                    convertedColumn.DataType = typeof(double);
                    break;
                case GenericDbColumnType.Single:
                    convertedColumn.DataType = typeof(Single);
                    break;
                case GenericDbColumnType.Decimal:
                    convertedColumn.DataType = typeof(decimal);
                    break;
                case GenericDbColumnType.DateTime:
                case GenericDbColumnType.Date:
                case GenericDbColumnType.Time:
                    convertedColumn.DataType = typeof(DateTime);
                    break;
                default:
                    throw new ApplicationException("Cannot convert column type.");
            }

            dt.Columns.Add(convertedColumn);

            foreach (DataRow row in dt.Rows)
            {
                row[convertedColumn.ColumnName] = row[dc.ColumnName];
            }

            convertedColumn.SetOrdinal(dc.Ordinal);
            dt.Columns.Remove(dc);
            convertedColumn.ColumnName = dc.ColumnName;
        }

        /// <summary>
        /// Adds option fields labels to a frequency table
        /// </summary>
        /// <param name="dtDataSources">The data table to process</param>
        private void AddOptionLabelsToFreqTable(DataTable dt)
        {
            #region Input Validation
            if (dt == null)
            {
                throw new ArgumentNullException("dtDataSources");
            }
            #endregion // Input Validation

            if (dt.Rows.Count == 0)
            {
                return;
            }

            // This code is messy and inefficient, perhaps we should just convert the option values to their
            // respective labels during the initial read? Does anyone ever want to see the numeric values?

            Field field = GetAssociatedField(dt.Columns[0].ColumnName);

            if (field == null)
            {
                return;
            }

            int numberOfRowsProcessed = 0;
            int totalRows = dt.Rows.Count;

            if (field is OptionField && (field as OptionField).Options.Count > 0)
            {
                OptionField optionField = field as OptionField;
                List<string> options = new List<string>();

                options.AddRange(optionField.Options);
                Dictionary<string, string> fieldValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                if (dt.Columns[0].DataType.ToString().Equals("System.Int16"))
                {
                    ConvertColumnToType(dt, dt.Columns[0], GenericDbColumnType.String);
                }

                for (int i = 0; i < options.Count; i++)
                {
                    string option = options[i];
                    foreach (DataRow fRow in dt.Rows)
                    {
                        string fValue = fRow[0].ToString();
                        if (fValue.Equals(i.ToString()))
                        {
                            fRow[0] = option;
                            numberOfRowsProcessed++;
                        }

                        if (numberOfRowsProcessed >= totalRows)
                        {
                            break;
                        }
                    }

                    if (numberOfRowsProcessed >= totalRows)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Adds option fields labels to a line list table
        /// </summary>
        /// <param name="dtDataSources">The data table to process</param>
        private void AddOptionLabelsToListTable(DataTable dt)
        {
            #region Input Validation
            if (dt == null)
            {
                throw new ArgumentNullException("dtDataSources");
            }
            #endregion // Input Validation

            if (dt.Rows.Count == 0)
            {
                return;
            }

            // This code is messy and inefficient, perhaps we should just convert the option values to their
            // respective labels during the initial read? Does anyone ever want to see the numeric values?

            Dictionary<OptionField, int> optionFields = new Dictionary<OptionField, int>();
            List<DataColumn> columnsToConvert = new List<DataColumn>();

            foreach (DataColumn dc in dt.Columns)
            {
                Field field = GetAssociatedField(dc.ColumnName);
                if (field is OptionField)
                {
                    optionFields.Add(field as OptionField, dc.Ordinal);

                    if (dc.DataType.ToString().Equals("System.Int16"))
                    {
                        columnsToConvert.Add(dc);
                    }
                }
            }

            foreach (DataColumn dc in columnsToConvert)
            {
                ConvertColumnToType(dt, dc, GenericDbColumnType.String);
            }

            if (optionFields.Count > 0)
            {
                foreach (DataRow fRow in dt.Rows)
                {
                    foreach (KeyValuePair<OptionField, int> kvp in optionFields)
                    {
                        string fValue = fRow[kvp.Value].ToString();
                        OptionField optionField = kvp.Key;
                        List<string> options = new List<string>();

                        options.AddRange(optionField.Options);

                        for (int i = 0; i < options.Count; i++)
                        {
                            string option = options[i];

                            if (fValue.Equals(i.ToString()))
                            {
                                fRow[kvp.Value] = option;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds comment legal labels to a frequency table
        /// </summary>
        /// <param name="dtDataSources">The data table to process</param>
        private void AddCommentLegalLabelsToFreqTable(DataTable dt)
        {
            #region Input Validation
            if (dt == null)
            {
                throw new ArgumentNullException("dtDataSources");
            }
            #endregion // Input Validation

            if (dt.Rows.Count == 0)
            {
                return;
            }

            Field field = GetAssociatedField(dt.Columns[0].ColumnName);

            if (field == null)
            {
                return;
            }

            int numberOfRowsProcessed = 0;
            int totalRows = dt.Rows.Count;

            if (field is DDLFieldOfCommentLegal && !string.IsNullOrEmpty(((TableBasedDropDownField)field).TextColumnName.Trim()))
            {
                DataTable codeDataTable = ((TableBasedDropDownField)field).GetSourceData();
                Dictionary<string, string> fieldValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                foreach (System.Data.DataRow row in codeDataTable.Rows)
                {
                    string key = row[((TableBasedDropDownField)field).TextColumnName.Trim()].ToString();
                    int dash = key.IndexOf('-');
                    string newKey = key.Substring(0, dash);

                    foreach (DataRow fRow in dt.Rows)
                    {
                        string fValue = fRow[0].ToString();
                        if (fValue.Equals(newKey))
                        {
                            fRow[0] = key;
                            numberOfRowsProcessed++;
                        }

                        if (numberOfRowsProcessed >= totalRows)
                        {
                            break;
                        }
                    }

                    if (numberOfRowsProcessed >= totalRows)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Adds comment legal labels to a line list table
        /// </summary>
        /// <param name="dtDataSources">The data table to process</param>
        private void AddCommentLegalLabelsToListTable(DataTable dt)
        {
            #region Input Validation
            if (dt == null)
            {
                throw new ArgumentNullException("dtDataSources");
            }
            #endregion // Input Validation

            if (dt.Rows.Count == 0)
            {
                return;
            }

            Dictionary<DDLFieldOfCommentLegal, int> optionFields = new Dictionary<DDLFieldOfCommentLegal, int>();

            foreach (DataColumn dc in dt.Columns)
            {
                Field field = GetAssociatedField(dc.ColumnName);
                if (field is DDLFieldOfCommentLegal)
                {
                    optionFields.Add(field as DDLFieldOfCommentLegal, dc.Ordinal);
                }
            }

            if (optionFields.Count > 0)
            {
                foreach (DataRow fRow in dt.Rows)
                {
                    foreach (KeyValuePair<DDLFieldOfCommentLegal, int> kvp in optionFields)
                    {
                        string fValue = fRow[kvp.Value].ToString();
                        DataTable codeDataTable = kvp.Key.GetSourceData();
                        Dictionary<string, string> fieldValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                        foreach (System.Data.DataRow row in codeDataTable.Rows)
                        {
                            string key = row[kvp.Key.TextColumnName.Trim()].ToString();
                            int dash = key.IndexOf('-');
                            string newKey = key.Substring(0, dash);

                            if (fValue.Equals(newKey))
                            {
                                fRow[kvp.Value] = key;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds supplementary column names to a list of column names
        /// </summary>
        /// <param name="columnNames">The list of column names to supplement</param>
        private void AddSupplementaryColumnNames(List<string> columnNames)
        {
            if (UseAdvancedUserDataFilter)
            {
                if (!string.IsNullOrEmpty(AdvancedUserDataFilter))
                {
                    foreach (KeyValuePair<string, string> kvp in TableColumnNames)
                    {
                        if (AdvancedUserDataFilter.ToLower().Contains(kvp.Key.ToLower()) && !columnNames.Contains(kvp.Key, caseInsensitiveEqualityComparer))
                        {
                            columnNames.Add(kvp.Key);
                        }
                    }
                }
            }
            else
            {
                //foreach (FilterCondition condition in DataFilters)
                //{
                //    if (!columnNames.Contains(condition.RawColumnName, caseInsensitiveEqualityComparer))
                //    {
                //        columnNames.Add(condition.RawColumnName);
                //    }
                //}
            }

            if (IsUsingEpiProject)
            {
                columnNames.Add("RecStatus");
            }

            foreach (IDashboardRule rule in Rules)
            {
                if (rule is Rule_Recode)
                {
                    Rule_Recode recodeRule = rule as Rule_Recode;
                    if (columnNames.Contains(recodeRule.DestinationColumnName, caseInsensitiveEqualityComparer) && !columnNames.Contains(recodeRule.SourceColumnName, caseInsensitiveEqualityComparer))
                    {
                        columnNames.Add(recodeRule.SourceColumnName);
                    }
                }
                else if (rule is Rule_Format)
                {
                    Rule_Format formatRule = rule as Rule_Format;
                    if (columnNames.Contains(formatRule.DestinationColumnName, caseInsensitiveEqualityComparer) && !columnNames.Contains(formatRule.SourceColumnName, caseInsensitiveEqualityComparer))
                    {
                        columnNames.Add(formatRule.SourceColumnName);
                    }
                }
                else if (rule is Rule_SimpleAssign)
                {
                    Rule_SimpleAssign simpleAssignRule = rule as Rule_SimpleAssign;
                    ColumnDataType columnDataType = ColumnDataType.Boolean | ColumnDataType.DateTime | ColumnDataType.Numeric | ColumnDataType.Text;
                    List<string> fieldNames = this.GetFieldsAsList(columnDataType);
                    foreach (string parameter in simpleAssignRule.AssignmentParameters)
                    {
                        if (!columnNames.Contains(parameter, caseInsensitiveEqualityComparer) && fieldNames.Contains(parameter, caseInsensitiveEqualityComparer))
                        {
                            columnNames.Add(parameter);
                        }
                    }
                }
                else if (rule is Rule_ExpressionAssign)
                {
                    Rule_ExpressionAssign expressionAssignRule = rule as Rule_ExpressionAssign;
                    ColumnDataType columnDataType = ColumnDataType.Boolean | ColumnDataType.Numeric | ColumnDataType.Text;
                    List<string> fieldNames = this.GetFieldsAsList(columnDataType);
                    foreach (string fieldName in fieldNames)
                    {
                        if (expressionAssignRule.Expression.ToLower().Contains(fieldName.ToLower()) && !columnNames.Contains(fieldName, caseInsensitiveEqualityComparer))
                        {
                            columnNames.Add(fieldName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Selects distinct values
        /// </summary>
        /// <param name="sourceTable">The table to execute the selection on</param>
        /// <param name="columnNames">The column names to use when selecting distinct values</param>        
        /// <returns>DataTable</returns>
        private DataTable SelectDistinct(DataTable sourceTable, params string[] columnNames)
        {
            #region Input Validation
            if (columnNames == null || columnNames.Length == 0)
            {
                throw new ArgumentNullException("columnNames");
            }
            #endregion // Input Validation

            object[] lastValues;
            DataTable distinctTable;

            lastValues = new object[columnNames.Length];
            distinctTable = new DataTable();

            foreach (string columnName in columnNames)
            {
                Type type = sourceTable.Columns[columnName].DataType;
                distinctTable.Columns.Add(columnName, type);
                if (type.ToString().Equals("System.Single") || type.ToString().Equals("System.Double"))
                {
                    distinctTable.Columns[columnName].DataType = typeof(decimal);
                }
            }

            DataTable dt = new DataTable();

            if (columnNames.Length != 0 && columnNames.Length == 1 && sourceTable.Columns.Count == 1 && columnNames[0].Equals(sourceTable.Columns[0].ColumnName))
            {
                dt = sourceTable;

                foreach (DataRow row in dt.Rows)
                {
                    if (!ColumnValuesAreEqual(lastValues, row, columnNames))
                    {
                        distinctTable.Rows.Add(CreateRowClone(row, distinctTable.NewRow(), columnNames));
                        SetLastValues(lastValues, row, columnNames);
                    }
                }
            }
            else
            {
                List<DataColumn> columnsToRemove = new List<DataColumn>();
                foreach (DataColumn column in sourceTable.Columns)
                {
                    bool found = false;
                    foreach (string s in columnNames)
                    {
                        if (column.ColumnName.ToLower().Equals(s.ToLower()))
                        {
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        columnsToRemove.Add(column);
                    }
                }

                dt = sourceTable.DefaultView.ToTable(false, columnNames);

                foreach (DataColumn dc in columnsToRemove)
                {
                    try
                    {
                        bool canRemove = dt.Columns.CanRemove(dc);

                        if (canRemove)
                        {
                            dt.Columns.Remove(dc.ColumnName);
                        }
                        else
                        {
                            dt.PrimaryKey = null;
                            DataColumn dc1 = dt.Columns[dc.ColumnName];
                            dt.Columns.Remove(dc1);
                        }
                    }
                    catch (ArgumentException)
                    {
                        // cannot remove column, so just continue
                    }
                }

                string[] safeColumnNames = new string[columnNames.Length];

                for (int i = 0; i < columnNames.Length; i++)
                {
                    safeColumnNames[i] = AddBracketsToString(columnNames[i]);
                }

                //orderedRows = dtDataSources.Select("", string.Join(", ", safeColumnNames));                
                DataView orderedView = new DataView(dt, "", string.Join(", ", safeColumnNames), DataViewRowState.CurrentRows);

                foreach (DataRowView rowView in orderedView)
                {
                    DataRow row = rowView.Row;
                    //Debug.Print(row.ItemArray[0].ToString());// + "  " + row.ItemArray[1].ToString());
                    if (!ColumnValuesAreEqual(lastValues, row, columnNames))
                    {
                        distinctTable.Rows.Add(CreateRowClone(row, distinctTable.NewRow(), columnNames));
                        SetLastValues(lastValues, row, columnNames);
                    }
                }
            }

            return distinctTable;
        }

        /// <summary>
        /// Selects distinct values
        /// </summary>
        /// <param name="sortColumn">The column by which to sort on</param>
        /// <param name="sourceTable">The table to execute the selection on</param>
        /// <param name="columnNames">The column names to use when selecting distinct values</param>        
        /// <remarks>
        /// The cases that are returned from this method are case-sensitive, such that "EE" 
        /// will be treated as differently than "ee" and both will be included in the list.
        /// The .NET Select and Compute methods, which can take filter parameters, will treat
        /// "ee" and "EE" as the same value; this may cause problems when calling this method
        /// with filters applied as some values may be double-counted as a result.
        /// </remarks>
        /// <returns>DataTable</returns>
        //private DataTable SelectDistinct(string sortColumn, DataTable sourceTable, params string[] columnNames)
        //{
        //    #region Input Validation
        //    if (columnNames == null || columnNames.Length == 0)
        //    {
        //        throw new ArgumentNullException("columnNames");
        //    }
        //    #endregion // Input Validation

        //    object[] lastValues;
        //    DataTable distinctTable;
        //    DataRow[] orderedRows;

        //    lastValues = new object[columnNames.Length];
        //    distinctTable = new DataTable();

        //    foreach (string columnName in columnNames)
        //    {
        //        distinctTable.Columns.Add(columnName, sourceTable.Columns[columnName].DataType);
        //    }

        //    string names = sortColumn + "," + string.Join(",", columnNames);

        //    orderedRows = sourceTable.Select("", names);

        //    foreach (DataRow row in orderedRows)
        //    {
        //        //Debug.Print(row.ItemArray[0].ToString() + "  " + row.ItemArray[1].ToString());

        //        if (!ColumnValuesAreEqual(lastValues, row, columnNames))
        //        {
        //            distinctTable.Rows.Add(CreateRowClone(row, distinctTable.NewRow(), columnNames));
        //            SetLastValues(lastValues, row, columnNames);
        //        }
        //    }
        //    return distinctTable;
        //}

        /// <summary>
        /// Selects distinct values
        /// </summary>        
        /// <param name="columnNames">The column names to use when selecting distinct values</param>        
        /// <returns>DataTable</returns>
        private DataTable SelectDistinct(DataView sourceView, params string[] columnNames)
        {
            #region Input Validation
            if (columnNames == null || columnNames.Length == 0)
            {
                throw new ArgumentNullException("columnNames");
            }
            #endregion // Input Validation

            object[] lastValues;
            DataTable distinctTable;

            lastValues = new object[columnNames.Length];
            distinctTable = new DataTable();

            foreach (string columnName in columnNames)
            {
                Type type = mainTable.Columns[columnName].DataType;
                distinctTable.Columns.Add(columnName, type);
                if (type.ToString().Equals("System.Single") || type.ToString().Equals("System.Double"))
                {
                    distinctTable.Columns[columnName].DataType = typeof(decimal);
                }
            }

            //if (columnNames.Length != 0 && columnNames.Length == 1 && sourceTable.Columns.Count == 1 && columnNames[0].Equals(sourceTable.Columns[0].ColumnName))
            //{

            string[] safeColumnNames = new string[columnNames.Length];

            for (int i = 0; i < columnNames.Length; i++)
            {
                safeColumnNames[i] = AddBracketsToString(columnNames[i]);
            }

            DataView sortedSourceView = new DataView(DataSet.Tables[0], sourceView.RowFilter, string.Join(", ", safeColumnNames), DataViewRowState.CurrentRows);
            foreach (DataRowView rowView in sortedSourceView)
            {
                DataRow row = rowView.Row;
                if (!ColumnValuesAreEqual(lastValues, row, columnNames))
                {
                    distinctTable.Rows.Add(CreateRowClone(row, distinctTable.NewRow(), columnNames));
                    SetLastValues(lastValues, row, columnNames);
                }
            }
            //}
            //else
            //{
            //    List<DataColumn> columnsToRemove = new List<DataColumn>();
            //    foreach (DataColumn column in sourceTable.Columns)
            //    {
            //        bool found = false;
            //        foreach (string s in columnNames)
            //        {
            //            if (column.ColumnName.ToLower().Equals(s.ToLower()))
            //            {
            //                found = true;
            //            }
            //        }

            //        if (!found)
            //        {
            //            columnsToRemove.Add(column);
            //        }
            //    }

            //    dtDataSources = sourceTable.DefaultView.ToTable(false, columnNames);

            //    foreach (DataColumn dc in columnsToRemove)
            //    {
            //        try
            //        {
            //            bool canRemove = dtDataSources.Columns.CanRemove(dc);

            //            if (canRemove)
            //            {
            //                dtDataSources.Columns.Remove(dc.ColumnName);
            //            }
            //            else
            //            {
            //                dtDataSources.PrimaryKey = null;
            //                DataColumn dc1 = dtDataSources.Columns[dc.ColumnName];
            //                dtDataSources.Columns.Remove(dc1);
            //            }
            //        }
            //        catch (ArgumentException)
            //        {
            //            // cannot remove column, so just continue
            //        }
            //    }

            //    string[] safeColumnNames = new string[columnNames.Length];

            //    for (int i = 0; i < columnNames.Length; i++)
            //    {
            //        safeColumnNames[i] = AddBracketsToString(columnNames[i]);
            //    }

            //    orderedRows = dtDataSources.Select("", string.Join(", ", safeColumnNames));

            //    foreach (DataRow row in orderedRows)
            //    {
            //        //Debug.Print(row.ItemArray[0].ToString());// + "  " + row.ItemArray[1].ToString());
            //        if (!ColumnValuesAreEqual(lastValues, row, columnNames))
            //        {
            //            distinctTable.Rows.Add(CreateRowClone(row, distinctTable.NewRow(), columnNames));
            //            SetLastValues(lastValues, row, columnNames);
            //        }
            //    }
            //}

            return distinctTable;
        }

        /// <summary>
        /// Selects distinct values
        /// </summary>
        /// <param name="sortColumn">The column by which to sort on</param>        
        /// <param name="columnNames">The column names to use when selecting distinct values</param>        
        /// <remarks>
        /// The cases that are returned from this method are case-sensitive, such that "EE" 
        /// will be treated as differently than "ee" and both will be included in the list.
        /// The .NET Select and Compute methods, which can take filter parameters, will treat
        /// "ee" and "EE" as the same value; this may cause problems when calling this method
        /// with filters applied as some values may be double-counted as a result.
        /// </remarks>
        /// <returns>DataTable</returns>
        private DataTable SelectDistinct(string sortColumn, DataView sourceView, params string[] columnNames)
        {
            #region Input Validation
            if (columnNames == null || columnNames.Length == 0)
            {
                throw new ArgumentNullException("columnNames");
            }
            #endregion // Input Validation

            object[] lastValues;
            DataTable distinctTable;
            //DataRow[] orderedRows;

            lastValues = new object[columnNames.Length];
            distinctTable = new DataTable();

            foreach (string columnName in columnNames)
            {
                distinctTable.Columns.Add(columnName, mainTable.Columns[columnName].DataType);
            }

            string names = string.Format("{0},{1}", sortColumn, string.Join(",", columnNames));

            DataView sortedView = new DataView(mainTable, sourceView.RowFilter, names, DataViewRowState.CurrentRows);

            //orderedRows = sourceTable.Select("", names);

            foreach (DataRowView rowView in sortedView)
            {
                DataRow row = rowView.Row;
                //Debug.Print(row.ItemArray[0].ToString() + "  " + row.ItemArray[1].ToString());

                if (!ColumnValuesAreEqual(lastValues, row, columnNames))
                {
                    distinctTable.Rows.Add(CreateRowClone(row, distinctTable.NewRow(), columnNames));
                    SetLastValues(lastValues, row, columnNames);
                }
            }
            return distinctTable;
        }

        /// <summary>
        /// Determines if the column values in a given row are equal to the values in the previous row for a given set of columns
        /// </summary>
        /// <param name="lastValues">The values to check against the previous row</param>
        /// <param name="currentRow">The current data row to check against</param>
        /// <param name="columnNames">The column names to use</param>
        /// <returns>bool</returns>
        private bool ColumnValuesAreEqual(object[] lastValues, DataRow currentRow, string[] columnNames)
        {
            bool equal = true;

            for (int i = 0; i < columnNames.Length; i++)
            {
                if (lastValues[i] == null || !lastValues[i].Equals(currentRow[columnNames[i]]))
                {
                    equal = false;
                    break;
                }
            }
            return equal;
        }

        /// <summary>
        /// Creates a cloned data row
        /// </summary>
        /// <param name="sourceRow"></param>
        /// <param name="newRow"></param>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        private DataRow CreateRowClone(DataRow sourceRow, DataRow newRow, string[] columnNames)
        {
            foreach (string field in columnNames)
            {
                newRow[field] = sourceRow[field];
            }
            return newRow;
        }

        /// <summary>
        /// Sets the last values for a given set of columns
        /// </summary>
        /// <param name="lastValues"></param>
        /// <param name="sourceRow"></param>
        /// <param name="columnNames"></param>
        private void SetLastValues(object[] lastValues, DataRow sourceRow, string[] columnNames)
        {
            for (int i = 0; i < columnNames.Length; i++)
            {
                lastValues[i] = sourceRow[columnNames[i]];
            }
        }

        /// <summary>
        /// Calculates various numeric statistics for a given frequency variable and filter criteria
        /// </summary>
        /// <param name="table">The overall set of data to be processed</param>
        /// <param name="freqTable">The frequencies of the data values for the frequency column</param>
        /// <param name="weightTotal"></param>
        /// <param name="freqVar">The frequency column name</param>
        /// <param name="weightVar">The weight column name</param>
        /// <param name="filter">Any filter parameters to apply</param>
        /// <returns>DescriptiveStatistics</returns>
        private DescriptiveStatistics DoMeans(GadgetParameters inputs, DataTable table, DataTable freqTable, string filter, string outerFilter)
        {
            DescriptiveStatistics means = new DescriptiveStatistics();

            string freqVar = inputs.MainVariableName;
            string weightVar = inputs.WeightVariableName;
            string crosstabVar = inputs.CrosstabVariableName;
            bool includeFullSummaryStatistics = inputs.ShouldIncludeFullSummaryStatistics;

            string safeFreqVar = AddBracketsToString(freqVar);
            string safeWeightVar = AddBracketsToString(weightVar);
            string safeCrosstabVar = AddBracketsToString(crosstabVar);

            if (string.IsNullOrEmpty(filter))
            {
                filter = string.Format("{0} is not null", safeFreqVar);
            }
            else
            {
                filter = string.Format("{0} and {1} is not null", filter, safeFreqVar);
            }

            if (!string.IsNullOrEmpty(outerFilter))
            {
                outerFilter = string.Format("{0} and {1} is not null", outerFilter, safeFreqVar);
            }
            else
            {
                outerFilter = string.Format("{0} is not null", safeFreqVar);
            }

            if (!string.IsNullOrEmpty(weightVar))
            {
                DataTable sortedFilteredTable = SortBySingleColumn(table, table.Columns[weightVar]);

                if (inputs.IsRequestCancelled())
                {
                    means.Observations = -1;
                    return means;
                }

                try
                {
                    string sumVar = sortedFilteredTable.Compute(string.Format("sum({0})", safeWeightVar), filter).ToString();
                    if (sumVar.Length > 0)
                    {
                        means.Observations = Convert.ToDouble(sumVar);
                    }
                }
                catch (InvalidCastException)
                {
                    means.Observations = 0;
                    return means;
                }

                if (includeFullSummaryStatistics)
                {
                    DataColumn squaresColumn = new DataColumn();
                    squaresColumn.DataType = typeof(double);
                    squaresColumn.ColumnName = "___squares___";
                    squaresColumn.Expression = string.Format("{0}*{1}", safeWeightVar, safeFreqVar);
                    sortedFilteredTable.Columns.Add(squaresColumn);

                    means.Sum = ConvertToDouble(string.Format("sum({0})", AddBracketsToString(squaresColumn.ColumnName)), filter, sortedFilteredTable);
                    //means.Min = Convert.ToDouble(sortedFilteredTable.Compute("min(" + safeFreqVar + ")", filter));
                    means.Min = ConvertToDouble(string.Format("min({0})", safeFreqVar), filter, sortedFilteredTable);
                    //means.Max = Convert.ToDouble(sortedFilteredTable.Compute("max(" + safeFreqVar + ")", filter));
                    means.Max = ConvertToDouble(string.Format("max({0})", safeFreqVar), filter, sortedFilteredTable);
                    means.Mean = means.Sum / means.Observations;

                    double grandObservations = ConvertToDouble(string.Format("sum({0})", safeWeightVar), outerFilter, sortedFilteredTable);
                    double grandSum = ConvertToDouble(string.Format("sum({0})", AddBracketsToString(squaresColumn.ColumnName)), outerFilter, sortedFilteredTable);

                    //means.grandMean = Convert.ToDouble(sortedFilteredTable.Compute("avg(" + AddBracketsToString(squaresColumn.ColumnName) + ")", outerFilter));

                    means.GrandMean = grandSum / grandObservations; //Convert.ToDouble(sortedFilteredTable.Select(outerFilter, string.Empty).CopyToDataTable().DefaultView.ToTable(sortedFilteredTable.TableName).Compute("avg(" + AddBracketsToString(freqVar) + ")", string.Empty));

                    DataColumn varCalcColumn = new DataColumn();
                    varCalcColumn.DataType = typeof(double);
                    varCalcColumn.ColumnName = "___varcalc___";
                    varCalcColumn.Expression = string.Format("(({0}*{1}) * {2})", safeFreqVar, safeFreqVar, safeWeightVar);
                    sortedFilteredTable.Columns.Add(varCalcColumn);

                    double? sumOfSquares = ConvertToDouble(string.Format("sum({0})", varCalcColumn.ColumnName), filter, sortedFilteredTable);
                    means.Variance = (sumOfSquares - means.Sum * means.Mean) / (means.Observations - 1);

                    if (means.Variance != null)
                    {
                        means.StdDev = Math.Sqrt((double)means.Variance);
                    }

                    //DataRow[] Rows = freqTable.Select("freq > 0", freqVar + " ASC");
                    //DataRow[] Rows = table.Select(filter, freqVar + " ASC");
                    DataRow[] rows = sortedFilteredTable.Select(filter, string.Format("{0} ASC", safeFreqVar)  /*,  freqVar + " ASC"*/);

                    DataTable weightTable = new DataTable();
                    DataColumn countColumn = new DataColumn();
                    countColumn.DataType = typeof(double);
                    countColumn.ColumnName = "___count___";
                    weightTable.Columns.Add(countColumn);

                    int rowCounter = 0;
                    foreach (System.Data.DataRow row in rows)
                    {
                        if (!string.IsNullOrEmpty(row[weightVar].ToString()))
                        {
                            double frequencyValue;

                            bool successValue = double.TryParse(row[freqVar].ToString(), out frequencyValue);

                            double weightValue = 1;
                            bool successWeight = double.TryParse(row[weightVar].ToString(), out weightValue);

                            if (successWeight && successValue)
                            {
                                rowCounter++;
                                for (double i = 0; i < weightValue; i++)
                                {
                                    weightTable.Rows.Add(frequencyValue);
                                }
                            }
                        }
                    }

                    rows = weightTable.Select(string.Empty, string.Format("{0} ASC", countColumn.ColumnName));

                    double quartileResult = 0;

                    // odd number of Rows
                    if (((double)rows.Length / 2.0) % 1 > 0)
                    {
                        int medianPosition = (rows.Length / 2);
                        means.Median = Convert.ToDouble(rows[medianPosition][countColumn.ColumnName]);

                        quartileResult = (double)rows.Length / 4.0;
                    }
                    else if (rows.Length > 1)
                    {
                        int lowerMedianPosition = (rows.Length / 2) - 1;
                        int upperMedianPosition = (rows.Length / 2);
                        means.Median = (Convert.ToDouble(rows[lowerMedianPosition][countColumn.ColumnName]) + Convert.ToDouble(rows[upperMedianPosition][countColumn.ColumnName])) / 2.0;

                        quartileResult = (double)rows.Length / 4.0;
                    }

                    // odd number of Rows in halves
                    if ((quartileResult / 2.0) % 1 > 0)
                    {
                        int q1Position = (int)quartileResult;
                        means.Q1 = Convert.ToDouble(rows[q1Position][countColumn.ColumnName]);
                        means.Q3 = Convert.ToDouble(rows[rows.Length - 1 - q1Position][countColumn.ColumnName]);
                    }
                    else
                    {
                        int lowerQ1Position = ((int)quartileResult) - 1;
                        int upperQ1Position = ((int)quartileResult);
                        if (lowerQ1Position + upperQ1Position > 0)
                        {
                            means.Q1 = (Convert.ToDouble(rows[lowerQ1Position][countColumn.ColumnName]) + Convert.ToDouble(rows[upperQ1Position][countColumn.ColumnName])) / 2.0;
                            means.Q3 = (Convert.ToDouble(rows[rows.Length - 1 - lowerQ1Position][countColumn.ColumnName]) + Convert.ToDouble(rows[rows.Length - 1 - upperQ1Position][countColumn.ColumnName])) / 2.0;
                        }
                    }

                    DataTable modeTable = weightTable.Clone();

                    foreach (DataRow row in weightTable.Rows)
                    {
                        modeTable.Rows.Add(row.ItemArray);
                    }

                    means.Mode = GetMode(modeTable, countColumn.ColumnName, string.Empty);

                    sortedFilteredTable.Columns.Remove(squaresColumn.ColumnName);
                    sortedFilteredTable.Columns.Remove(varCalcColumn.ColumnName);
                }
            }
            else
            {
                DataRow[] rowArray = table.Select(filter, string.Empty);
                if (rowArray.Length == 0)
                {
                    return means;
                }

                if (inputs.IsRequestCancelled())
                {
                    means.Observations = -1;
                    return means;
                }

                DataTable filteredTable = rowArray.CopyToDataTable().DefaultView.ToTable(table.TableName);

                if (inputs.IsRequestCancelled())
                {
                    means.Observations = -1;
                    return means;
                }

                DataTable sortedFilteredTable = SortBySingleColumn(filteredTable, filteredTable.Columns[freqVar]);

                if (inputs.IsRequestCancelled())
                {
                    means.Observations = -1;
                    return means;
                }

                means.Observations = Convert.ToDouble(sortedFilteredTable.Compute(string.Format("count({0})", safeFreqVar), string.Empty));
                means.Mode = 0;

                if (includeFullSummaryStatistics)
                {
                    try
                    {
                        means.Sum = ConvertToDouble(string.Format("sum({0})", safeFreqVar), string.Empty, sortedFilteredTable);
                        means.Min = ConvertToDouble(string.Format("min({0})", safeFreqVar), string.Empty, sortedFilteredTable);
                        means.Max = ConvertToDouble(string.Format("max({0})", safeFreqVar), string.Empty, sortedFilteredTable);
                        means.Mean = means.Sum / means.Observations;

                        string grandMeanFilter = outerFilter;

                        if (!string.IsNullOrEmpty(crosstabVar))
                        {
                            grandMeanFilter = string.Format("{0} and {1} is not null", grandMeanFilter, safeCrosstabVar);
                        }

                        if (GetColumnType(freqVar).Equals("System.Byte") || GetColumnType(freqVar).Equals("System.Int16") || GetColumnType(freqVar).Equals("System.Int32"))
                        {
                            DataTable copyOfTable = table.Clone();
                            DataColumn dc = copyOfTable.Columns[freqVar];

                            if (dc.DataType.ToString().Equals("System.Byte") || dc.DataType.ToString().Equals("System.Int16") || GetColumnType(freqVar).Equals("System.Int32"))
                            {
                                dc.DataType = typeof(decimal);
                            }

                            foreach (DataRow row in table.Rows)
                            {
                                copyOfTable.ImportRow(row);
                            }

                            DataTable tempTable = copyOfTable.Select(grandMeanFilter, string.Empty).CopyToDataTable().DefaultView.ToTable(copyOfTable.TableName);
                            means.GrandMean = ConvertToDouble(string.Format("avg({0})", safeFreqVar), string.Empty, tempTable);
                        }
                        else
                        {
                            DataTable tempTable = table.Select(grandMeanFilter, string.Empty).CopyToDataTable().DefaultView.ToTable(table.TableName);
                            means.GrandMean = ConvertToDouble(string.Format("avg({0})", safeFreqVar), string.Empty, tempTable);
                        }
                    }
                    catch (InvalidCastException)
                    {
                        // do nothing
                    }
                    try
                    {
                        DataColumn varCalcColumn = new DataColumn();
                        varCalcColumn.DataType = typeof(double);
                        varCalcColumn.ColumnName = "___varcalc___";

                        varCalcColumn.Expression = string.Format("({0}*{1})", safeFreqVar, safeFreqVar);
                        sortedFilteredTable.Columns.Add(varCalcColumn);

                        double? sumOfSquares = ConvertToDouble(string.Format("sum({0})", varCalcColumn.ColumnName), filter, sortedFilteredTable);
                        means.Variance = (sumOfSquares - means.Sum * means.Mean) / (means.Observations - 1);

                        sortedFilteredTable.Columns.Remove(varCalcColumn.ColumnName);

                        if (means.Variance != null)
                        {
                            means.StdDev = Math.Sqrt((double)means.Variance);
                        }
                    }
                    catch (InvalidCastException)
                    {
                        // do nothing
                    }

                    if (sortedFilteredTable.Rows.Count >= 1)
                    {
                        double quartileResult = 0;

                        // odd number of Rows
                        if (((double)sortedFilteredTable.Rows.Count / 2.0) % 1 > 0)
                        {
                            int medianPosition = (sortedFilteredTable.Rows.Count / 2);
                            means.Median = Convert.ToDouble(sortedFilteredTable.Rows[medianPosition][freqVar]);

                            quartileResult = (double)sortedFilteredTable.Rows.Count / 4.0;
                        }
                        else if (sortedFilteredTable.Rows.Count > 1)
                        {
                            int lowerMedianPosition = (sortedFilteredTable.Rows.Count / 2) - 1;
                            int upperMedianPosition = (sortedFilteredTable.Rows.Count / 2);
                            means.Median = (Convert.ToDouble(sortedFilteredTable.Rows[lowerMedianPosition][freqVar]) + Convert.ToDouble(sortedFilteredTable.Rows[upperMedianPosition][freqVar])) / 2.0;

                            quartileResult = (double)sortedFilteredTable.Rows.Count / 4.0;
                        }

                        if (((double)sortedFilteredTable.Rows.Count / 4.0) % 1 > 0)
                        {
                            int q1Position = (int)quartileResult;
                            means.Q1 = Convert.ToDouble(sortedFilteredTable.Rows[q1Position][freqVar]);
                            means.Q3 = Convert.ToDouble(sortedFilteredTable.Rows[sortedFilteredTable.Rows.Count - 1 - q1Position][freqVar]);
                        }
                        else
                        {
                            int lowerQ1Position = ((int)quartileResult) - 1;
                            int upperQ1Position = ((int)quartileResult);
                            means.Q1 = (Convert.ToDouble(sortedFilteredTable.Rows[lowerQ1Position][freqVar]) + Convert.ToDouble(sortedFilteredTable.Rows[upperQ1Position][freqVar])) / 2.0;
                            means.Q3 = (Convert.ToDouble(sortedFilteredTable.Rows[sortedFilteredTable.Rows.Count - 1 - lowerQ1Position][freqVar]) + Convert.ToDouble(sortedFilteredTable.Rows[sortedFilteredTable.Rows.Count - 1 - upperQ1Position][freqVar])) / 2.0;
                        }

                        means.Mode = GetMode(sortedFilteredTable, freqVar, string.Empty);
                    }
                    //else
                    //{
                    //    means.median = (Convert.ToDouble(sortedFilteredTable.Rows[0][freqVar]) + Convert.ToDouble(sortedFilteredTable.Rows[0][freqVar])) / 2;
                    //    means.q1 = means.median;
                    //    means.q3 = means.median;
                    //    means.mode = means.median;
                    //}
                }
            }
            return means;
        }

        private double ConvertToDouble(string computeExp, string filter, DataTable sortedFilteredTable)
        {
            return Convert.ToDouble(sortedFilteredTable.Compute(computeExp, filter));
            //if (result.Length> 0)
            //{
            //    return Convert.ToDouble(result);
            //}
            //else
            //{
            //    return 0.0;
            //}
        }

        /// <summary>
        /// Gets the mode (the value with the highest frequency) for a given column
        /// </summary>
        /// <param name="table">The data table to use</param>
        /// <param name="columnName">The name of the column in the data table</param>
        /// <param name="filter">Any filters to apply</param>
        /// <returns>double</returns>
        private double GetMode(DataTable table, string columnName, string filter)
        {
            DataView modeView = new DataView(table, filter, string.Format("{0} ASC", AddBracketsToString(columnName)), DataViewRowState.CurrentRows);

            List<double> frequencyValues = new List<double>();

            foreach (DataRowView rowView in modeView)
            {
                DataRow row = rowView.Row;

                double rowValue = 0;
                bool success = Double.TryParse(row[columnName].ToString(), out rowValue);

                if (success)
                {
                    frequencyValues.Add(rowValue);
                }
            }
            var mode = frequencyValues.GroupBy(n => n).OrderByDescending(g => g.Count()).Select(g => g.Key).FirstOrDefault();
            return mode;
        }

        /// <summary>
        /// Gets the appropriate GenericDbColumnType for the given standard type.
        /// </summary>
        /// <param name="columnType">The column type</param>
        /// <returns>GenericDbColumnType</returns>
        private GenericDbColumnType ConvertToGenericDbColumnType(string columnType)
        {
            switch (columnType)
            {
                case "System.Int16":
                    return GenericDbColumnType.Int16;
                case "System.Int32":
                    return GenericDbColumnType.Int32;
                case "System.Int64":
                    return GenericDbColumnType.Int64;
                case "System.String":
                    return GenericDbColumnType.String;
                case "System.Byte":
                    return GenericDbColumnType.Byte;
                case "System.Boolean":
                    return GenericDbColumnType.Boolean;
                case "System.Decimal":
                    return GenericDbColumnType.Decimal;
                case "System.Double":
                    return GenericDbColumnType.Double;
                case "System.DateTime":
                    return GenericDbColumnType.DateTime;
                case "System.Guid":
                    return GenericDbColumnType.Guid;
                case "System.UInt16":
                    return GenericDbColumnType.UInt16;
                case "System.UInt32":
                    return GenericDbColumnType.UInt32;
                case "System.UInt64":
                    return GenericDbColumnType.UInt64;
                case "System.Single":
                    return GenericDbColumnType.Single;
                case "System.SByte":
                    return GenericDbColumnType.SByte;
                default:
                    return GenericDbColumnType.Object;
            }
        }

        /// <summary>
        /// Relate child table data into a parent table
        /// </summary>
        /// <param name="parentTable">The parent table to be modified</param>
        /// <param name="childTable">The child table</param>
        /// <param name="parentKey">The key column to be used in the parent</param>
        /// <param name="childKey">The key column to be used in the child</param>
        private void RelateInto(DataTable parentTable, DataTable childTable, string parentKey, string childKey, bool useUnmatched, GadgetParameters inputs = null)
        {
            Dictionary<string, string> columnNameMapping = new Dictionary<string, string>();

            if (inputs != null)
            {
                inputs.UpdateGadgetStatus(string.Format(SharedStrings.DASHBOARD_GADGET_STATUS_PROCESSING_RELATED_TABLE, childTable.TableName));
            }

            foreach (DataColumn dc in childTable.Columns)
            {
                string columnName = dc.ColumnName;
                int i = 1;

                while (parentTable.Columns.Contains(columnName))
                {
                    columnName = string.Format("{0}{1}", dc.ColumnName, i.ToString());
                    i++;
                }

                columnNameMapping.Add(dc.ColumnName, columnName); // map the actual name used to the one in the child table
                parentTable.Columns.Add(columnName, dc.DataType);
            }

            bool toMany = false;
            foreach (DataRow childRow in childTable.Rows)
            {
                if (ValueOccursMoreThanOnce(childTable, childKey, childRow[childKey].ToString()))
                {
                    toMany = true;
                    break;
                }
            }

            try
            {
                // assume parent key is unique and try to make it the primary key. If successful, this will speed up the relate
                // procedure by a very large amount.
                DataColumn[] parentPrimaryKeyColumns = new DataColumn[1];
                parentPrimaryKeyColumns[0] = parentTable.Columns[parentKey];
                parentTable.PrimaryKey = parentPrimaryKeyColumns;
            }
            catch
            {
            }

            string parentKeyDataType = parentTable.Columns[parentKey].DataType.ToString();
            string childKeyDataType = childTable.Columns[childKey].DataType.ToString();

            if (toMany == false && parentTable.PrimaryKey.Contains(parentTable.Columns[parentKey]) && parentTable.PrimaryKey.Length == 1 && parentKeyDataType == childKeyDataType)
            {
                int counter = 0;
                int total = childTable.Rows.Count;

                foreach (DataRow childRow in childTable.Rows)
                {
                    DataRow parentRow = parentTable.Rows.Find(childRow[childKey]);
                    if (parentRow != null)
                    {
                        if (parentRow[parentKey].ToString().Equals(childRow[childKey].ToString()))
                        {
                            foreach (DataColumn dc in childTable.Columns)
                            {
                                string mappedColumnName = columnNameMapping[dc.ColumnName];
                                parentRow[mappedColumnName] = childRow[dc.ColumnName];
                            }
                        }
                    }

                    counter++;

                    if (counter % 200 == 0 && inputs != null)
                    {
                        if (counter > total)
                        {
                            counter = total;
                        }

                        inputs.UpdateGadgetProgress(((double)counter / (double)total) * 100);
                        inputs.UpdateGadgetStatus(string.Format(SharedStrings.DASHBOARD_GADGET_STATUS_RELATING_TABLE, childTable.TableName, ((double)counter / (double)total).ToString("P")));
                        if (inputs.IsRequestCancelled())
                        {
                            return;
                        }
                    }
                }
            }
            else
            {
                int counter = 0;
                int total = childTable.Rows.Count;

                if (toMany)
                {
                    parentTable.PrimaryKey = null;
                    Dictionary<string, object[]> objects = new Dictionary<string, object[]>();
                    List<DataRow> rowsToRemove = new List<DataRow>();

                    parentTable.Columns.Add("__INT__TRK__ID__", typeof(int));
                    childTable.Columns.Add("__INT__TRK__ID__", typeof(int));

                    for (int r = 0; r < childTable.Rows.Count; r++)
                    {
                        childTable.Rows[r]["__INT__TRK__ID__"] = r + 1;
                    }

                    counter = 0;
                    foreach (DataRow childRow in childTable.Rows)
                    {
                        int count = 0;

                        if (!string.IsNullOrEmpty(childRow[childKey].ToString().Trim()))
                        {
                            count = GetDistinctValueCount(childTable, childKey, childRow[childKey].ToString());
                            foreach (DataRow parentRow in parentTable.Rows)
                            {
                                if (parentRow[parentKey].ToString().Equals(childRow[childKey].ToString()))
                                {
                                    //for (int i = 1; i < count; i++)
                                    //{
                                    if (!objects.ContainsKey(childRow["__INT__TRK__ID__"].ToString()) && count >= 2)
                                    {
                                        objects.Add(childRow["__INT__TRK__ID__"].ToString(), parentRow.ItemArray);
                                        if (!rowsToRemove.Contains(parentRow))
                                        {
                                            rowsToRemove.Add(parentRow);
                                        }
                                    }
                                    //}
                                }
                            }
                        }

                        counter++;

                        if (counter % 200 == 0 && inputs != null)
                        {
                            if (counter > total)
                            {
                                counter = total;
                            }
                            inputs.UpdateGadgetProgress(((double)counter / (double)total) * 100);
                            inputs.UpdateGadgetStatus(string.Format(SharedStrings.DASHBOARD_GADGET_STATUS_PREPROCESSING_RELATED_TABLE, childTable.TableName, ((double)counter / (double)total).ToString("P")));
                            if (inputs.IsRequestCancelled())
                            {
                                return;
                            }
                        }
                    }

                    counter = 0;

                    if (!useUnmatched)
                    {
                        parentTable.Rows.Clear();
                    }
                    else
                    {
                        foreach (DataRow row in rowsToRemove)
                        {
                            parentTable.Rows.Remove(row);
                        }
                    }

                    foreach (KeyValuePair<string, object[]> kvp in objects)
                    {
                        parentTable.Rows.Add(kvp.Value);
                        parentTable.Rows[parentTable.Rows.Count - 1]["__INT__TRK__ID__"] = kvp.Key;
                    }

                    foreach (DataRow childRow in childTable.Rows)
                    {
                        foreach (DataRow parentRow in parentTable.Rows)
                        {
                            if (parentRow["__INT__TRK__ID__"].ToString().Equals(childRow["__INT__TRK__ID__"].ToString()))
                            {
                                foreach (DataColumn dc in childTable.Columns)
                                {
                                    if (dc.ColumnName != "__INT__TRK__ID__")
                                    {
                                        string mappedColumnName = columnNameMapping[dc.ColumnName];
                                        parentRow[mappedColumnName] = childRow[dc.ColumnName];
                                    }
                                }
                                break;
                            }
                        }

                        counter++;

                        if (counter % 200 == 0 && inputs != null)
                        {
                            if (counter > total)
                            {
                                counter = total;
                            }
                            inputs.UpdateGadgetProgress(((double)counter / (double)total) * 100);
                            inputs.UpdateGadgetStatus(string.Format(SharedStrings.DASHBOARD_GADGET_STATUS_RELATING_TABLE, childTable.TableName, ((double)counter / (double)total).ToString("P")));
                            if (inputs.IsRequestCancelled())
                            {
                                return;
                            }
                        }
                    }

                    childTable.Columns.Remove("__INT__TRK__ID__");
                    parentTable.Columns.Remove("__INT__TRK__ID__");
                }
                else
                {
                    foreach (DataRow childRow in childTable.Rows)
                    {
                        foreach (DataRow parentRow in parentTable.Rows)
                        {
                            if (parentRow[parentKey].ToString().Equals(childRow[childKey].ToString()))
                            {
                                foreach (DataColumn dc in childTable.Columns)
                                {
                                    string mappedColumnName = columnNameMapping[dc.ColumnName];
                                    parentRow[mappedColumnName] = childRow[dc.ColumnName];
                                }
                                break;
                            }
                        }

                        counter++;

                        if (counter % 200 == 0 && inputs != null)
                        {
                            if (counter > total)
                            {
                                counter = total;
                            }

                            inputs.UpdateGadgetStatus(string.Format(SharedStrings.DASHBOARD_GADGET_STATUS_RELATING_TABLE, childTable.TableName, ((double)counter / (double)total).ToString("P")));
                            if (inputs.IsRequestCancelled())
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets a list of all distinct stratafication values
        /// </summary>
        /// <param name="filteredTable">The table from which to pull the distinct values</param>
        /// <param name="strataVars">The list of variables to statify</param>
        /// <returns>List of strings</returns>
        private Dictionary<string, string> GetStrataValuesAsDictionary(List<MyString> strataList, bool includeMissing = false)
        {
            DataView filteredView = DataSet.Tables[0].DefaultView;
            List<string> strataVars = new List<string>();
            for (int i = 0; i < strataList.Count; i++)
            {
                strataVars.Add(strataList[i].VarName);
            }

            Dictionary<string, string> stratas = new Dictionary<string, string>();

            #region Input Validation
            if (strataVars == null || strataVars.Count == 0)
            {
                throw new ApplicationException("No strata variables.");
            }
            #endregion //Input Validation

            DataTable distinctTable = SelectDistinct(filteredView, strataVars.ToArray());
            string sortClause = string.Empty;
            List<string> booleanColumnNames = new List<string>();

            foreach (string strataVar in strataVars)
            {
                string columnType = GetColumnType(strataVar);
                string sort = " ASC,";

                if (columnType.Equals("System.Boolean") || (IsUsingEpiProject && View.Fields.Contains(strataVar) && View.Fields[strataVar] is YesNoField))
                {
                    sort = " DESC,";
                    booleanColumnNames.Add(strataVar);
                }

                sortClause = string.Format("{0}{1}{2}", sortClause, strataVar, sort);
            }
            sortClause = sortClause.TrimEnd(',');

            DataRow[] rowArray = distinctTable.Select("", sortClause);
            DataTable sortedDistinctTable = rowArray.CopyToDataTable().DefaultView.ToTable(distinctTable.TableName);
            DataTable sortedDistinctTableTranslated = new DataTable();

            sortedDistinctTableTranslated = sortedDistinctTable.Copy(); // make a copy with values translated into Epi 7 yes/no values if necessary

            foreach (string booleanColumnName in booleanColumnNames)
            {
                DataColumn column = new DataColumn(string.Format("{0}_TEMP_DB_X", booleanColumnName), typeof(string));
                sortedDistinctTableTranslated.Columns.Add(column);

                foreach (DataRow row in sortedDistinctTableTranslated.Rows)
                {
                    row[column] = RecodeBooleanToYesNo(row[booleanColumnName].ToString());
                }

                sortedDistinctTableTranslated.Columns.Remove(booleanColumnName);
                column.ColumnName = booleanColumnName;
            }

            for (int j = 0; j < sortedDistinctTable.Rows.Count; j++)
            {
                DataRow row = sortedDistinctTable.Rows[j];
                DataRow translatedRow = sortedDistinctTableTranslated.Rows[j];

                string s = string.Empty;
                string epiValue = string.Empty;

                for (int i = 0; i < sortedDistinctTable.Columns.Count; i++)
                {
                    string strataColumnType = GetColumnType(sortedDistinctTable.Columns[i].ColumnName);
                    string strataValue = row[i].ToString();
                    strataValue = FormatValue(strataValue, strataColumnType);

                    if (string.IsNullOrEmpty(strataValue))
                    {
                        if (includeMissing)
                        {
                            s = string.Format("{0}{1} is null ", s, AddBracketsToString(sortedDistinctTable.Columns[i].ColumnName));
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        s = string.Format("{0}{1} = {2}", s, AddBracketsToString(sortedDistinctTable.Columns[i].ColumnName), strataValue);
                    }

                    string translatedStrataValue = string.Empty;
                    translatedStrataValue = translatedRow[i].ToString();
                    epiValue = string.Format("{0}{1} = {2}", epiValue, sortedDistinctTable.Columns[i].ColumnName, translatedStrataValue);

                    if (i != sortedDistinctTable.Columns.Count - 1)
                    {
                        s = string.Format("{0} and ", s);
                        epiValue = string.Format("{0} and ", epiValue);
                    }
                }

                if (s.StartsWith(" and") && epiValue.StartsWith(" and"))
                {
                    s = s.Remove(0, 4);
                    epiValue = epiValue.Remove(0, 4);
                }

                if (string.IsNullOrEmpty(s))
                {
                    if (includeMissing)
                    {
                        stratas.Add(s, epiValue);
                    }
                }
                else
                {
                    stratas.Add(s, epiValue);
                }
            }
            return stratas;
        }

        /// <summary>
        /// Gets a dictionary of all distinct cross-tabluation values and their corresponding 'Epi 7 friendly' display values
        /// </summary>        
        /// <param name="crosstabVar">The variable to use for the cross-tabulation</param>
        /// <returns>List of strings</returns>
        private Dictionary<string, string> GetCrosstabValuesAsDictionary(string crosstabVar, string freqVar, bool includeMissing = false)
        {
            Dictionary<string, string> crosstabValues = new Dictionary<string, string>();

            #region Input Validation
            if (string.IsNullOrEmpty(crosstabVar))
            {
                throw new ArgumentNullException("crosstabVar");
            }
            #endregion //Input Validation

            SortOrder sort = SortOrder.Ascending;
            DataView sourceView = DataSet.Tables[0].DefaultView;

            string columnType = GetColumnType(crosstabVar);
            bool isBoolean = false;
            //string yesValue = "Yes";//; config.Settings.RepresentationOfYes;
            //string noValue = "No";//config.Settings.RepresentationOfNo;
            //string missingValue = "Missing";//config.Settings.RepresentationOfMissing;

            if (columnType.Equals("System.Boolean"))// || (IsUsingEpiProject && View.Fields.Contains(crosstabVar) && View.Fields[crosstabVar] is YesNoField))
            {
                isBoolean = true;
                sort = SortOrder.Descending;
            }

            //if (columnType.Equals("System.Decimal") || columnType.Equals())
            //{
            //    sort = SortOrder.Descending;
            //}

            DataTable distinctTable = SelectDistinct(sourceView, crosstabVar);
            DataTable sortedDistinctTable = SortBySingleColumn(distinctTable, distinctTable.Columns[crosstabVar], sort);

            foreach (DataRow row in sortedDistinctTable.Rows)
            {
                if (!includeMissing && row[0].ToString().ToLower().Trim().Equals(string.Empty))
                {
                    continue;
                }

                if (isBoolean)
                {
                    string epiValue = RecodeBooleanToYesNo(row[0].ToString());
                    crosstabValues.Add(row[0].ToString(), epiValue);
                }
                else
                {
                    crosstabValues.Add(row[0].ToString(), row[0].ToString());
                }
            }
            return crosstabValues;
        }

        readonly string EwavTempRepresentationOfYes = Ewav.Web.Config.ConfigDataSet.RepresentationOfYes;//"Yes";
        readonly string EwavTempRepresentationOfNo = Ewav.Web.Config.ConfigDataSet.RepresentationOfNo;//"No";
        readonly string EwavTempRepresentationOfMissing = Ewav.Web.Config.ConfigDataSet.RepresentationOfMissing;//"Missing";

        /// <summary>
        /// Recodes a boolean "true" or "false" (or 1 and 0) into Epi Info 7 representations of yes and no
        /// </summary>
        /// <param name="value">The value to recode</param>
        /// <returns>string</returns>
        private string RecodeBooleanToYesNo(string value)
        {
            string recodedValue = string.Empty;

            if (value.ToLower().Equals("true") || value.ToLower().Equals("1"))
            {
                recodedValue = EwavTempRepresentationOfYes;            //       config.Settings.RepresentationOfYes;
            }
            else if (value.ToLower().Equals("false") || value.ToLower().Equals("0"))
            {
                recodedValue = EwavTempRepresentationOfNo;     //         config.Settings.RepresentationOfNo;
            }
            else if (value.ToLower().Trim().Equals(string.Empty))
            {
                recodedValue = EwavTempRepresentationOfMissing;     //      config.Settings.RepresentationOfMissing;
            }
            return recodedValue;
        }

        /// <summary>
        /// Formats the table name property of the connection
        /// </summary>
        /// <param name="tableName">The table name to format</param>
        /// <returns>String representing the formatted table name</returns>
        private string FormatTableName(string tableName)
        {
            if (tableName.Contains('.'))
            {
                return tableName;
            }
            else
            {
                return string.Format("{0}{1}{2}", stringLiterals.LEFT_SQUARE_BRACKET, TableName, stringLiterals.RIGHT_SQUARE_BRACKET);
            }
        }

        #endregion // Private Methods

        #region Classes
        private class CaseInsensitiveEqualityComparer : IEqualityComparer<string>
        {
            // Implement the IComparable interface.
            public bool Equals(string obj1, string obj2)
            {
                if (string.Compare(obj1, obj2, true) == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }
        }
        #endregion // Classes
    }
}