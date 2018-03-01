/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       EwavRule_GroupVariable.cs
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

namespace Ewav.Web.Services
{
    [DataContract]
    public class EwavRule_GroupVariable : EwavRule_Base
    {
        string friendlyLabel;
        [DataMember]
        public string FriendlyLabel
        {
            get { return friendlyLabel; }
            set { friendlyLabel = value; }
        }

        private string groupName;
        [DataMember]
        public string GroupName
        {
            get { return groupName; }
            set { groupName = value; }
        }

        private List<MyString> items;
        [DataMember]
        public List<MyString> Items
        {
            get { return items; }
            set { items = value; }
        }

    }
}