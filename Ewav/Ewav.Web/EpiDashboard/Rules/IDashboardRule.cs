/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       IDashboardRule.cs
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
using System.Text;
using System.Xml;

namespace Ewav.Web.EpiDashboard.Rules
{
    /// <summary>
    /// An interface used for implementing dashboard rules
    /// </summary>
    public interface IDashboardRule
    {
        /// <summary>
        /// Gets the human-readable (display) version of the rule
        /// </summary>
        string FriendlyRule { get; }

        /// <summary>
        /// Sets up the rule
        /// </summary>
        void SetupRule(DataTable table);

        /// <summary>
        /// Applies the rule
        /// </summary>
        void ApplyRule(DataRow row);

        /// <summary>
        /// Generates an Xml element for this rule
        /// </summary>
        /// <param name="doc">The parent Xml document</param>
        /// <returns>XmlNode representing this rule</returns>
        XmlNode Serialize(XmlDocument doc);

        /// <summary>
        /// Creates the rule from an Xml element
        /// </summary>
        /// <param name="element">The XmlElement from which to create the rule</param>
        void CreateFromXml(XmlElement element);
    }
}