/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       EwavRule_SimpleAssignment.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Ewav.Web.EpiDashboard.Rules;

namespace Ewav.Web.Services
{
    [DataContract]
    public class EwavRule_SimpleAssignment : EwavRule_Base
    {
        private string friendlyLabel;

        [DataMember]
        public string FriendlyLabel
        {
            get { return friendlyLabel; }
            set { friendlyLabel = value; }
        }

        private string txtDestinationField;
        [DataMember]
        public string TxtDestinationField
        {
            get { return txtDestinationField; }
            set { txtDestinationField = value; }
        }

        private SimpleAssignType assignmentType;
        [DataMember]
        public SimpleAssignType AssignmentType
        {
            get { return assignmentType; }
            set { assignmentType = value; }
        }

        private List<MyString> parameters;
        [DataMember]
        public List<MyString> Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }
    }
}