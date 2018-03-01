/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       EwavRule_Format.cs
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
using Ewav.Web.EpiDashboard.Rules;

namespace Ewav.Web.Services
{
    [DataContract]
    public class EwavRule_Format : EwavRule_Base
    {
        string friendlyLabel;
        [DataMember]
        public string FriendlyLabel
        {
            get { return friendlyLabel; }
            set { friendlyLabel = value; }
        }

        string cbxFieldName;
        [DataMember]
        public string CbxFieldName
        {
            get
            {
                return cbxFieldName;
            }
            set
            {
                cbxFieldName = value;
            }
        }

        string txtDestinationField;

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

        string cbxFormatOptions;
        [DataMember]
        public string CbxFormatOptions
        {
            get
            {
                return cbxFormatOptions;
            }
            set
            {
                cbxFormatOptions = value;
            }
        }

        FormatTypes formatTypes;
        [DataMember]
        public FormatTypes FormatTypes
        {
            get
            {
                return formatTypes;
            }
            set
            {
                formatTypes = value;
            }
        }
    }
}