/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       GadgetParameters.cs
 *  Namespace:  EpiDashboard    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epi;
using Ewav.Web.Services;
using Ewav.BAL;

namespace Ewav.Web.EpiDashboard
{
    /// <summary>
    /// A class used to encompass input parameters, delegates, and events
    /// </summary>
    public class GadgetParameters
    {
        #region Private Members

        /// <summary>
        /// The Table name of the Table
        /// </summary>
        private string tableName;




        private string datasourceName;






        /// <summary>
        /// The list of input variables for this operation
        /// </summary>
        private Dictionary<string, string> inputVariableList;

        /// <summary>
        /// The main (or exposure) variable names
        /// </summary>
        private List<string> mainVariableNames;

        /// <summary>
        /// The main (or exposure) variable name
        /// </summary>
        private string mainVariableName;

        /// <summary>
        /// The column names to select
        /// </summary>
        private List<MyString> columnNames;

        /// <summary>
        /// The cross-tab (or outcome) variable name
        /// </summary>
        private string crosstabVariableName;

        /// <summary>
        /// The list of strata variable names
        /// </summary>
        private List<string> strataVariableNames;

        private List<MyString> strataVariableList;

        /// <summary>
        /// The weight variable name
        /// </summary>
        private string weightVariableName;

        /// <summary>
        /// Whether results should include missing values
        /// </summary>
        private bool shouldIncludeMissing;

        /// <summary>
        /// Whether results should be sorted high-to-low; if not, they will be sorted alphabetically
        /// </summary>
        private bool shouldSortHighToLow;

        /// <summary>
        /// Whether the results should include all possible list values; applicable only for drop-down list fields in Epi Info 7 projects
        /// </summary>
        private bool shouldUseAllPossibleValues;

        /// <summary>
        /// Whether the results should include label values from comment legal fields
        /// </summary>
        private bool shouldShowCommentLegalLabels;

        /// <summary>
        /// Whether the results should include the full set of summary statistics (median, mode, mean, variance, etc); if not, only the total number of observations will be calculated
        /// </summary>
        private bool shouldIncludeFullSummaryStatistics;

        /// <summary>
        /// A custom data filter to use
        /// </summary>
        private string customFilter;

        /// <summary>
        /// A custom sort order to use
        /// </summary>
        private string customSortColumnName;
        #endregion // Private Members

        //public delegate void GadgetStatusUpdateHandler(string statusMessage);
        //public delegate void GadgetProgressUpdateHandler(string statusMessage, double progress);
        //public delegate bool GadgetCheckForCancellationHandler();

        #region Events
        //public event GadgetStatusUpdateHandler GadgetStatusUpdate;
        //public event GadgetCheckForCancellationHandler GadgetCheckForCancellation;
        //public event SetGadgetStatusHandler GadgetCheckForProgress;
        #endregion // Events

        #region Constructors
        private bool useAdvancedDataFilter;

        private string advancedDataFilterText;

        /// <summary>
        /// Constructor
        /// </summary>
        public GadgetParameters()
        {
            columnNames = new List<MyString>();
            //   GadgetStatusUpdate = null;
            inputVariableList = new Dictionary<string, string>();
            CustomFilter = string.Empty;
            CustomSortColumnName = string.Empty;
            ShouldIncludeFullSummaryStatistics = false;
            ShouldUseAllPossibleValues = false;
            ShouldShowCommentLegalLabels = false;
            ShouldSortHighToLow = false;
            ShouldIncludeMissing = false;
            CrosstabVariableName = string.Empty;
            WeightVariableName = string.Empty;
            StrataVariableNames = new List<string>();
            StrataVariableList = new List<MyString>();
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        public GadgetParameters(GadgetParameters parameters)
        {
            MainVariableNames = parameters.MainVariableNames;
            MainVariableName = parameters.MainVariableName;
            columnNames = parameters.columnNames;
            //  GadgetStatusUpdate = parameters.GadgetStatusUpdate;
            inputVariableList = parameters.inputVariableList;
            CustomFilter = parameters.CustomFilter;
            CustomSortColumnName = parameters.CustomSortColumnName;
            ShouldIncludeFullSummaryStatistics = parameters.ShouldIncludeFullSummaryStatistics;
            ShouldUseAllPossibleValues = parameters.ShouldUseAllPossibleValues;
            ShouldShowCommentLegalLabels = parameters.ShouldShowCommentLegalLabels;
            ShouldSortHighToLow = parameters.ShouldSortHighToLow;
            ShouldIncludeMissing = parameters.ShouldIncludeMissing;
            CrosstabVariableName = parameters.CrosstabVariableName;
            WeightVariableName = parameters.WeightVariableName;
            StrataVariableNames = parameters.StrataVariableNames;
            StrataVariableList = parameters.StrataVariableList;
        }
        #endregion // Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the advanced data filter text.
        /// </summary>
        /// <value>The advanced data filter text.</value>
        public string AdvancedDataFilterText
        {
            get
            {
                return this.advancedDataFilterText;
            }
            set
            {
                this.advancedDataFilterText = value;
            }
        }

        /// <summary>
        /// Gets or sets the use advanced data filter.
        /// </summary>
        /// <value>The use advanced data filter.</value>
        public bool UseAdvancedDataFilter
        {
            get
            {
                return this.useAdvancedDataFilter;
            }
            set
            {
                this.useAdvancedDataFilter = value;
            }
        }


        /// <summary>
        /// Gets or sets the map tips column list.
        /// </summary>
        /// <value>The map tips column list.</value>
        public List<EwavColumn> MapTipsColumnList { get; set; }

        /// <summary>
        /// Gets or sets the name of the datasource.
        /// </summary>
        /// <value>The name of the datasource.</value>
        public string DatasourceName
        {
            get
            {
                return this.datasourceName;
            }
            set
            {
                this.datasourceName = value;
            }
        }

        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }
        /// <summary>
        /// Gets/sets the input variable list
        /// </summary>
        public Dictionary<string, string> InputVariableList
        {
            get
            {
                return this.inputVariableList;
            }
            set
            {
                this.inputVariableList = value;
            }
        }

        /// <summary>
        /// Gets/sets the names of the main variables
        /// </summary>
        public List<string> MainVariableNames
        {
            get
            {
                return this.mainVariableNames;
            }
            set
            {
                this.mainVariableNames = value;
            }
        }

        /// <summary>
        /// Gets/sets the name of the main variable
        /// </summary>
        public string MainVariableName
        {
            get
            {
                return this.mainVariableName;
            }
            set
            {
                this.mainVariableName = value;
            }
        }

        /// <summary>
        /// Gets/sets the names of the column names to use in processing this gadget
        /// </summary>
        public List<MyString> ColumnNames
        {
            get
            {
                return this.columnNames;
            }
            set
            {
                this.columnNames = value;
            }
        }

        /// <summary>
        /// Gets/sets the name of the crosstab variable
        /// </summary>
        public string CrosstabVariableName
        {
            get
            {
                return this.crosstabVariableName;
            }
            set
            {
                this.crosstabVariableName = value;
            }
        }

        /// <summary>
        /// Gets/sets the name of the crosstab variable
        /// </summary>
        public string WeightVariableName
        {
            get
            {
                return this.weightVariableName;
            }
            set
            {
                this.weightVariableName = value;
            }
        }

        /// <summary>
        /// Gets/sets the list of strata variable names
        /// </summary>
        public List<string> StrataVariableNames
        {
            get
            {
                return this.strataVariableNames;
            }
            set
            {
                this.strataVariableNames = value;
            }
        }


        public List<MyString> StrataVariableList
        {
            get { return strataVariableList; }
            set { strataVariableList = value; }
        }

        /// <summary>
        /// Gets/sets whether or not to include missing values in the results and calculations
        /// </summary>
        public bool ShouldIncludeMissing
        {
            get
            {
                return this.shouldIncludeMissing;
            }
            set
            {
                this.shouldIncludeMissing = value;
            }
        }

        /// <summary>
        /// Gets/sets whether or not to sort results in high-to-low fashion
        /// </summary>
        public bool ShouldSortHighToLow
        {
            get
            {
                return this.shouldSortHighToLow;
            }
            set
            {
                this.shouldSortHighToLow = value;
            }
        }

        /// <summary>
        /// Gets/sets whether the results should include all possible list values; applicable only for drop-down list fields in Epi Info 7 projects
        /// </summary>
        public bool ShouldUseAllPossibleValues
        {
            get
            {
                return this.shouldUseAllPossibleValues;
            }
            set
            {
                this.shouldUseAllPossibleValues = value;
            }
        }

        /// <summary>
        /// Gets/sets whether the results should include the labels from comment legal fields
        /// </summary>
        public bool ShouldShowCommentLegalLabels
        {
            get
            {
                return this.shouldShowCommentLegalLabels;
            }
            set
            {
                this.shouldShowCommentLegalLabels = value;
            }
        }

        /// <summary>
        /// Gets/sets whether the results should include the full set of summary statistics (median, mode, mean, variance, etc); if not, only the total number of observations will be calculated
        /// </summary>
        public bool ShouldIncludeFullSummaryStatistics
        {
            get
            {
                return this.shouldIncludeFullSummaryStatistics;
            }
            set
            {
                this.shouldIncludeFullSummaryStatistics = value;
            }
        }

        /// <summary>
        /// Gets/sets a custom filter to use for this gadget (warning: only applicable to certain methods)
        /// </summary>
        /// <remarks>Only applicable to certain methods</remarks>
        public string CustomFilter
        {
            get
            {
                return customFilter;
            }
            set
            {
                customFilter = value;
            }
        }

        /// <summary>
        /// Gets/sets a custom sort order to use for this gadget (warning: only applicable to certain methods)
        /// </summary>
        /// <remarks>Only applicable to certain methods</remarks>
        public string CustomSortColumnName
        {
            get
            {
                return customSortColumnName;
            }
            set
            {
                customSortColumnName = value;
            }
        }

        private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }


        private List<EwavDataFilterCondition> gadgetFilters;

        public List<EwavDataFilterCondition> GadgetFilters
        {
            get { return gadgetFilters; }
            set { gadgetFilters = value; }
        }


        #endregion // Public Properties

        #region Internal Methods
        /// <summary>
        /// Sends a status message back to the gadget
        /// </summary>
        /// <param name="statusMessage">The status message to display</param>
        internal void UpdateGadgetStatus(string statusMessage)
        {
            //if (GadgetStatusUpdate != null)
            //{
            //  //   GadgetStatusUpdate(statusMessage);
            //}
        }

        /// <summary>
        /// Sends a progress update back to the gadget
        /// </summary>
        /// <param name="progress">The amount of progress</param>
        internal void UpdateGadgetProgress(double progress)
        {
            //if (GadgetCheckForProgress != null)
            //{
            //    GadgetCheckForProgress("", progress);
            //}
        }

        /// <summary>
        /// Used to check whether or not the request to generate output has been cancelled
        /// </summary>
        /// <returns>bool</returns>
        internal bool IsRequestCancelled()
        {
            //if (GadgetCheckForCancellation != null)
            //{
            //    return GadgetCheckForCancellation();
            //}
            return false;
        }
        #endregion // Internal Methods
    }
}