/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       EwavRule_Recode.cs
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
using Ewav.Web.EpiDashboard;
using Ewav.Web.EpiDashboard.Rules;
using System.Runtime.Serialization;

namespace Ewav.Web.Services
{
    [DataContract]
    public class EwavRule_Recode : EwavRule_Base
    {
        private string friendlyrule;

        [DataMember]
        public string Friendlyrule
        {
            get
            {
                return friendlyrule;
            }
            set
            {
                friendlyrule = value;
            }
        }

        private string sourceColumnName;

        [DataMember]
        public string SourceColumnName
        {
            get
            {
                return sourceColumnName;
            }
            set
            {
                sourceColumnName = value;
            }
        }

        private string sourceColumnType;

        [DataMember]
        public string SourceColumnType
        {
            get
            {
                return sourceColumnType;
            }
            set
            {
                sourceColumnType = value;
            }
        }

        private string txtDestinationField;

        [DataMember]
        public string TxtDestinationField
        {
            get
            {
                return txtDestinationField;
            }
            set
            {
                txtDestinationField = value;
            }
        }

        private DashboardVariableType destinationFieldType;

        [DataMember]
        public DashboardVariableType DestinationFieldType
        {
            get
            {
                return destinationFieldType;
            }
            set
            {
                destinationFieldType = value;
            }
        }

        private List<EwavRuleRecodeDataRow> recodeTable;

        [DataMember]
        public List<EwavRuleRecodeDataRow> RecodeTable
        {
            get
            {
                return recodeTable;
            }
            set
            {
                recodeTable = value;
            }
        }

        private string txtElseValue;

        [DataMember]
        public string TxtElseValue
        {
            get
            {
                return txtElseValue;
            }
            set
            {
                txtElseValue = value;
            }
        }

        private bool checkboxMaintainSortOrderIndicator;

        [DataMember]
        public bool CheckboxMaintainSortOrderIndicator
        {
            get
            {
                return checkboxMaintainSortOrderIndicator;
            }
            set
            {
                checkboxMaintainSortOrderIndicator = value;
            }
        }

        private bool checkboxUseWildcardsIndicator;

        [DataMember]
        public bool CheckboxUseWildcardsIndicator
        {
            get
            {
                return checkboxUseWildcardsIndicator;
            }
            set
            {
                checkboxUseWildcardsIndicator = value;
            }
        }

        public EwavRule_Recode()
        {
        }
    }

}