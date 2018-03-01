/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       VariableRow.cs
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
using System.Text;

namespace Ewav.Web.Services
{
    /// <summary>
    /// Custome VariableRow class. It was struct in EpiInfo statistical repository. combines variables from LinearRegression And LogisticRegression
    /// controls.
    /// </summary>
    //public class VariableRow
    //{
    //    private string variableName;

    //    public string VariableName
    //    {
    //        get { return variableName; }
    //        set { variableName = value; }
    //    }
    //    private double oddsRatio;

    //    public double OddsRatio
    //    {
    //        get { return oddsRatio; }
    //        set { oddsRatio = value; }
    //    }

    //    private double ninetyFivePercent;

    //    public double NinetyFivePercent
    //    {
    //        get { return ninetyFivePercent; }
    //        set { ninetyFivePercent = value; }
    //    }
    //    private double ci;

    //    public double Ci
    //    {
    //        get { return ci; }
    //        set { ci = value; }
    //    }
    //    private double coefficient;

    //    public double Coefficient
    //    {
    //        get { return coefficient; }
    //        set { coefficient = value; }
    //    }
    //    private double se;

    //    public double Se
    //    {
    //        get { return se; }
    //        set { se = value; }
    //    }
    //    private double z;

    //    public double Z
    //    {
    //        get { return z; }
    //        set { z = value; }
    //    }
    //    private double p;

    //    public double P
    //    {
    //        get { return p; }
    //        set { p = value; }
    //    }

    //    private double stdError;

    //    public double StdError
    //    {
    //        get { return stdError; }
    //        set { stdError = value; }
    //    }
    //    private double ftest;

    //    public double Ftest
    //    {
    //        get { return ftest; }
    //        set { ftest = value; }
    //    }

    //}
}