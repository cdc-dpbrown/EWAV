/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ListBoxItemSource.cs
 *  Namespace:  Ewav.ViewModels    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.Windows.Controls;
using Ewav.Web.Services;

namespace Ewav.ViewModels
{
    public class ListBoxItemSource
    {
        /// <summary>
        /// Gets or sets the new column.
        /// </summary>
        /// <value>The new column.</value>
        public string NewColumn { get; set; }

        /// <summary>
        /// Gets or sets the assign expression.
        /// </summary>
        /// <value>The assign expression.</value>
        public string AssignExpression { get; set; }

        public string RuleString { get; set; }

        public string SourceColumn { get; set; }

        public string DestinationColumn { get; set; }

        public string DataType;

        public EwavRuleType RuleType { get; set; }

        public EwavRule_Base Rule { get; set; }

        public Panel FilterConditionsPanel { get; set; }
    }
}