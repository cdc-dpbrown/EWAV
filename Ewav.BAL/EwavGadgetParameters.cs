/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       EwavGadgetParameters.cs
 *  Namespace:  Ewav.BAL     
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ewav.BAL 
{
    public class EwavGadgetParameters
    {
        #region Private Members
        //private List<MyString>
        #endregion // Private Members
        #region Events
        #endregion // Events
        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        public EwavGadgetParameters()
        {
            ColumnNames = new List<string>();
            //   GadgetStatusUpdate = null;
            InputVariableList = new Dictionary<string, string>();
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
            NameOfDtoList = string.Empty;
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        public EwavGadgetParameters(EwavGadgetParameters parameters)
        {
            MainVariableNames = parameters.MainVariableNames;
            MainVariableName = parameters.MainVariableName;
            ColumnNames = parameters.ColumnNames;
            //  GadgetStatusUpdate = parameters.GadgetStatusUpdate;
            InputVariableList = parameters.InputVariableList;
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
        }

        #endregion // Constructors

        #region Public Properties
     
        public string NameOfDtoList { get; set; }

        /// <summary>
        /// Gets/sets the input variable list
        /// </summary>
        public Dictionary<string, string> InputVariableList { get; set; }

        /// <summary>
        /// Gets/sets the names of the main variables
        /// </summary>
        public List<string> MainVariableNames { get; set; }

        /// <summary>
        /// Gets/sets the name of the main variable
        /// </summary>
        public string MainVariableName { get; set; }

        /// <summary>
        /// Gets/sets the names of the column names to use in processing this gadget
        /// </summary>
        public List<string> ColumnNames { get; set; }

        /// <summary>
        /// Gets/sets the name of the crosstab variable
        /// </summary>
        public string CrosstabVariableName { get; set; }

        /// <summary>
        /// Gets/sets the name of the crosstab variable
        /// </summary>
        public string WeightVariableName { get; set; }

        /// <summary>
        /// Gets/sets the list of strata variable names
        /// </summary>
        public List<string> StrataVariableNames { get; set; }

        /// <summary>
        /// Gets/sets whether or not to include missing values in the results and calculations
        /// </summary>
        public bool ShouldIncludeMissing { get; set; }

        /// <summary>
        /// Gets/sets whether or not to sort results in high-to-low fashion
        /// </summary>
        public bool ShouldSortHighToLow { get; set; }

        /// <summary>
        /// Gets/sets whether the results should include all possible list values; applicable only for drop-down list fields in Epi Info 7 projects
        /// </summary>
        public bool ShouldUseAllPossibleValues { get; set; }

        /// <summary>
        /// Gets/sets whether the results should include the labels from comment legal fields
        /// </summary>
        public bool ShouldShowCommentLegalLabels { get; set; }

        /// <summary>
        /// Gets/sets whether the results should include the full set of summary statistics (median, mode, mean, variance, etc); if not, only the total number of observations will be calculated
        /// </summary>
        public bool ShouldIncludeFullSummaryStatistics { get; set; }

        /// <summary>
        /// Gets/sets a custom filter to use for this gadget (warning: only applicable to certain methods)
        /// </summary>
        /// <remarks>Only applicable to certain methods</remarks>
        public string CustomFilter { get; set; }

        /// <summary>
        /// Gets/sets a custom sort order to use for this gadget (warning: only applicable to certain methods)
        /// </summary>
        /// <remarks>Only applicable to certain methods</remarks>
        public string CustomSortColumnName { get; set; }


        /// <summary>
        /// Gets or sets the name of the lat X column.
        /// </summary>
        /// <value>The name of the lat X column.</value>
        public string LatXColumnName { get; set; }

        /// <summary>
        /// Gets or sets the name of the lon Y column.
        /// </summary>
        /// <value>The name of the lon Y column.</value>
        public string LonYColumnName { get; set; }


        /// <summary>
        /// Gets or sets the name of the map tip column.
        /// </summary>
        /// <value>The name of the map tip column.</value>
        public string MapTipColumnName { get; set; }

        #endregion // Public Properties
    }
}