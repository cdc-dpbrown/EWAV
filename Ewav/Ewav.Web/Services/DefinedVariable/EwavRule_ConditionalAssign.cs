/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       EwavRule_ConditionalAssign.cs
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
using System.Web;
using System.Runtime.Serialization;
using System.Data.Objects.DataClasses;

namespace Ewav.Web.Services
{
    [DataContract]
    public class EwavRule_ConditionalAssign : EwavRule_Base
    {
        private string txtDestination;
        [DataMember]
        public string TxtDestination
        {
            get { return txtDestination; }
            set { txtDestination = value; }
        }

        private string destinationColumnType;
        [DataMember]
        public string DestinationColumnType
        {
            get { return destinationColumnType; }
            set { destinationColumnType = value; }
        }

        private string assignValue;
        [DataMember]
        public string AssignValue
        {
            get { return assignValue; }
            set { assignValue = value; }
        }

        private string elseValue;
        [DataMember]
        public string ElseValue
        {
            get {
                //if (destinationColumnType == "System.Boolean"   && elseValue.Length == 0)
                //{
                //    return null;
                //}
                //else
                //{
                    return elseValue;
                //}
            }
            set { elseValue = value; }
        }

        private List<EwavDataFilterCondition> conditionsList;
        [DataMember]
        public List<EwavDataFilterCondition> ConditionsList
        {
            get { return conditionsList; }
            set { conditionsList = value; }
        }

        private cbxFieldTypeEnum cbxFieldType;
        [DataMember]
        public cbxFieldTypeEnum CbxFieldType
        {
            get { return cbxFieldType; }
            set { cbxFieldType = value; }
        }
        private MyString friendlyRule;
        [DataMember]
        public MyString FriendlyRule
        {
            get { return friendlyRule; }
            set { friendlyRule = value; }
        }

    }

    public enum cbxFieldTypeEnum
    {
        YesNo = 0,
        Text,
        Numeric,
        None
    }
}