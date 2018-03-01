/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       EwavRule_Base.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
 using System.Data.Objects.DataClasses;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Ewav.Web.Services
{
    [DataContract]
    [KnownType(typeof(EwavRule_Recode)), 
    KnownType(typeof(EwavRule_SimpleAssignment)),
    KnownType(typeof(EwavRule_Format)),
    KnownType(typeof(EwavRule_GroupVariable)),
    KnownType(typeof(EwavRule_ExpressionAssign)),
    KnownType(typeof(EwavRule_ConditionalAssign))]
    public class EwavRule_Base : EntityObject
    {
        int id;

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>    
        /// 

        [Key]
        [DataMember]
        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        [Key]
        [DataMember]
        public string VaraiableName
        { get; set; }

        [Key]
        [DataMember]
        public string VaraiableDataType
        { get; set; }

        
    }


    public enum EwavRuleType
    {
        Recode = 0,
        Assign,
        Formatted,
        Simple,
        conditional,
        GroupVariable
    }
}