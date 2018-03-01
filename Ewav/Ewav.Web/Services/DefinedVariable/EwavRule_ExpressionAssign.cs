/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       EwavRule_ExpressionAssign.cs
 *  Namespace:  Ewav.Web.Services               
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Ewav.Web.Services           
{
    [DataContract]
    public class EwavRule_ExpressionAssign : EwavRule_Base
    {
        /// <summary>
        /// Gets or sets the type of the data.
        /// </summary>
        /// <value>The type of the data.</value>    
        /// 
                                                 [DataMember]     
        public string DataType { get; set; }
        /// <summary>
        /// Gets or sets the name of the destination column.
        /// </summary>
        /// <value>The name of the destination column.</value>
        /// 
        [DataMember]
        public string DestinationColumnName { get; set; }
        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        /// <value>The expression.</value>
        /// 
        [DataMember]
        public string Expression { get; set; }
        /// <summary>
        /// Gets or sets the friendly rule.
        /// </summary>
        /// <value>The friendly rule.</value>
        /// 
        [DataMember]
        public string FriendlyRule { get; set; }
    }
}