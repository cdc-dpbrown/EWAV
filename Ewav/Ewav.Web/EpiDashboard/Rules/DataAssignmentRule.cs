/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       DataAssignmentRule.cs
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

namespace Ewav.Web.EpiDashboard.Rules
{
    /// <summary>
    /// A class for assigning data to a column in the database
    /// </summary>
    public abstract class DataAssignmentRule : IDashboardRule
    {
        #region Protected Members
        protected string friendlyRule;
        protected string destinationColumnName;
        protected string destinationColumnType;
        protected DashboardHelper dashboardHelper;
        protected DashboardVariableType variableType; 
        #endregion // Protected Members

        #region Public Properties
        /// <summary>
        /// Gets the name of the column where the assigned values will reside
        /// </summary>
        public string DestinationColumnName
        {
            get { return this.destinationColumnName; }
        }

        public string DestinationColumnType 
        { 
            get { return this.destinationColumnType; } 
        }

        /// <summary>
        /// Gets the type of the variable storing the recoded values
        /// </summary>
        public DashboardVariableType VariableType
        {
            get { return this.variableType; }
        }
        #endregion // Public Properties

        #region Protected Properties
        /// <summary>
        /// Gets the dashboard helper attached to this rule
        /// </summary>
        protected DashboardHelper DashboardHelper
        {
            get
            {
                return this.dashboardHelper;
            }
            set
            {
                this.dashboardHelper = value;
            }
        }
        #endregion // Protected Properties

        #region IDashboardRule Members
        /// <summary>
        /// Gets the human-readable (display) version of the rule
        /// </summary>
        public string FriendlyRule
        {
            get { return this.friendlyRule; }
        }

        public abstract void ApplyRule(DataRow row);
        public abstract void SetupRule(DataTable table);
        public abstract System.Xml.XmlNode Serialize(System.Xml.XmlDocument doc);
        public abstract void CreateFromXml(System.Xml.XmlElement element);
        #endregion // IDashboardRule Members
    }
}