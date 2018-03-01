/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       EwavDataFilterCondition.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.Collections.Generic;
using Ewav.Web.Services;

namespace Ewav.Web.Services
{
  public class EwavDataFilterCondition
  {
    //    dashboardHelper.AddDataFilterCondition(friendlyOperand, friendlyValue, fieldName, joinType);    
      MyString friendlyOperand;
      MyString friendlyValue;
      MyString fieldName;
      MyString joinType;
      MyString friendLowValue;
      MyString friendHighValue;     

    /// <summary>
    /// Gets of sets friendHighValue
    /// </summary>
    public MyString FriendHighValue
    {
      get { return friendHighValue; }
      set { friendHighValue = value; }
    }

    /// <summary>
    /// Gets or sets friendLowValue.
    /// </summary>
    public MyString FriendLowValue
    {
      get { return friendLowValue; }
      set { friendLowValue = value; }
    }

    /// <summary>
    /// Gets or sets the friendly operand.
    /// </summary>
    /// <value>The friendly operand.</value>
    public MyString FriendlyOperand
    {
      get
      {
        return this.friendlyOperand;
      }
      set
      {
        this.friendlyOperand = value;
      }
    }

    /// <summary>
    /// Gets or sets the friendly value.
    /// </summary>
    /// <value>The friendly value.</value>
    public MyString FriendlyValue
    {
      get
      {
        return this.friendlyValue;
      }
      set
      {
        this.friendlyValue = value;
      }
    }

    /// <summary>
    /// Gets or sets the name of the field.
    /// </summary>
    /// <value>The name of the field.</value>
    public MyString FieldName
    {
      get
      {
        return this.fieldName;
      }
      set
      {
        this.fieldName = value;
      }
    }

    /// <summary>
    /// Gets or sets the type of the join.
    /// </summary>
    /// <value>The type of the join.</value>
    public MyString JoinType
    {
      get
      {
        return this.joinType;
      }
      set
      {
        this.joinType = value;
      }
    }
  }
}