/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       StringLiterals.Shared.cs
 *  Namespace:  Ewav.Web    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;

//namespace Ewav.Web
//{
    public partial class StringLiterals
    {

        public string RenderFinishWithError = "RenderFinishWithError";
        public string ApplicationException = "RenderFinishWithError";
        public string RenderFinishWithWarning = "RenderFinishWithError";
        public string RenderFinish = "RenderFinishWithError";

        /// <summary>
        /// String Literal ASCENDING (++)
        /// </summary>
        private  string aSCENDING = "(++)";

        public string ASCENDING
        {
            get { return aSCENDING; }
        } 


        /// <summary>
        /// String Literal AMPERSAND
        /// </summary>
        private  string aMPERSAND = "&";

        public string AMPERSAND
        {
            get { return aMPERSAND; }
        } 


        /// <summary>
        /// String literal back tick
        /// </summary>
        private  string bACK_TICK = @"`";

        public string BACK_TICK
        {
            get { return bACK_TICK; }
        } 


        /// <summary>
        /// String Literal BACKWARD SLASH
        /// </summary>
        private  string bACKWARD_SLASH = "\\";

        public string BACKWARD_SLASH
        {
            get { return bACKWARD_SLASH; }
        } 


        /// <summary>
        /// String Literal CARET
        /// </summary>
        private  string cARET = "^";

        public string CARET
        {
            get { return cARET; }
        } 


        /// <summary>
        /// String Literal COLON
        /// </summary>
        private  string cOLON = ":";

        public string COLON
        {
            get { return cOLON; }
        } 


        /// <summary>
        /// String Literal COMMA
        /// </summary>
        private  string cOMMA = ",";

        public string COMMA
        {
            get { return cOMMA; }
        } 


        /// <summary>
        /// String Literal COMMERCIAL AT
        /// </summary>
        private  string cOMMERCIAL_AT = "@";

        public string COMMERCIAL_AT
        {
            get { return cOMMERCIAL_AT; }
        } 


        /// <summary>
        /// String Literal Left Curly Brace
        /// </summary>
        private  string cURLY_BRACE_LEFT = "{";

        public string CURLY_BRACE_LEFT
        {
            get { return cURLY_BRACE_LEFT; }
        } 


        /// <summary>
        /// String Literal Right Curly Brace
        /// </summary>
        private  string cURLY_BRACE_RIGHT = "}";

        public string CURLY_BRACE_RIGHT
        {
            get { return cURLY_BRACE_RIGHT; }
        } 


        /// <summary>
        /// String Literal DESCENDING
        /// </summary>
        private  string dESCENDING = "(--)";

        public string DESCENDING
        {
            get { return dESCENDING; }
        } 


        /// <summary>
        /// String Literal DOUBLE QUOTES
        /// </summary>
        private  string dOUBLEQUOTES = "\"";

        public string DOUBLEQUOTES
        {
            get { return dOUBLEQUOTES; }
        } 


        /// <summary>
        /// Ellipsis
        /// </summary>
        private  string eLLIPSIS = "...";

        public string ELLIPSIS
        {
            get { return eLLIPSIS; }
        } 


        /// <summary>
        /// String Literal EPI REPRESENTATION OF FALSE
        /// </summary>
        private  string ePI_REPRESENTATION_OF_FALSE = "(-)";

        public string EPI_REPRESENTATION_OF_FALSE
        {
            get { return ePI_REPRESENTATION_OF_FALSE; }
        } 


        /// <summary>
        /// String Literal EPI REPRESENTATION OF MISSING
        /// </summary>
        private  string ePI_REPRESENTATION_OF_MISSING = "(.)";

        public string EPI_REPRESENTATION_OF_MISSING
        {
            get { return ePI_REPRESENTATION_OF_MISSING; }
        } 


        /// <summary>
        /// String Literal EPI REPRESENTATION OF TRUE
        /// </summary>
        private  string ePI_REPRESENTATION_OF_TRUE = "(+)";

        public string EPI_REPRESENTATION_OF_TRUE
        {
            get { return ePI_REPRESENTATION_OF_TRUE; }
        } 


        /// <summary>
        /// String Literal EPI TABLE
        /// </summary>
        private  string ePI_TABLE = "epitable";

        public string EPI_TABLE
        {
            get { return ePI_TABLE; }
        } 


        /// <summary>
        /// String Literal EQUAL
        /// </summary>
        private  string eQUAL = "=";

        public string EQUAL
        {
            get { return eQUAL; }
        } 


        /// <summary>
        /// String Literal FORWARD SLASH
        /// </summary>
        private  string fORWARD_SLASH = "/";

        public string FORWARD_SLASH
        {
            get { return fORWARD_SLASH; }
        } 


        /// <summary>
        /// String Literal GREATER_THAN
        /// </summary>
        private  string gREATER_THAN = ">";

        public string GREATER_THAN
        {
            get { return gREATER_THAN; }
        } 


        /// <summary>
        /// String Literal GREATER_THAN_OR_EQUAL
        /// </summary>
        private  string gREATER_THAN_OR_EQUAL = ">=";

        public string GREATER_THAN_OR_EQUAL
        {
            get { return gREATER_THAN_OR_EQUAL; }
        } 


        /// <summary>
        /// String Literal HASH CLOSE
        /// </summary>
        private  string hASH = "#";

        public string HASH
        {
            get { return hASH; }
        } 


        /// <summary>
        /// String Literal HYPHEN
        /// </summary>
        private  string hYPHEN = "-";

        public string HYPHEN
        {
            get { return hYPHEN; }
        } 


        /// <summary>
        /// String Literal LEFT SQUARE BRACKET
        /// </summary>
        private  string lEFT_SQUARE_BRACKET = "[";

        public string LEFT_SQUARE_BRACKET
        {
            get { return lEFT_SQUARE_BRACKET; }
        } 


        /// <summary>
        /// String Literal LESS THAN
        /// </summary>
        private  string lESS_THAN = "<";

        public string LESS_THAN
        {
            get { return lESS_THAN; }
        } 



        /// <summary>
        /// String Literal LESS THAN_OR_EQUAL
        /// </summary>
        private  string lESS_THAN_OR_EQUAL = "<=";

        public string LESS_THAN_OR_EQUAL
        {
            get { return lESS_THAN_OR_EQUAL; }
        } 


        /// <summary>
        /// String Literal LESS THAN_OR_GREATER THAN (NOT EQUAL)
        /// </summary>
        private  string lESS_THAN_OR_GREATER_THAN = "<>";

        public string LESS_THAN_OR_GREATER_THAN
        {
            get { return lESS_THAN_OR_GREATER_THAN; }
        } 


        /// <summary>
        /// String Literal NEW LINE
        /// </summary>
        private  string nEW_LINE = "\r\n";

        public string NEW_LINE
        {
            get { return nEW_LINE; }
        } 


        /// <summary>
        /// String Literal NewValue
        /// </summary>
        private  string nEW_VALUE = "NewValue";

        public string NEW_VALUE
        {
            get { return nEW_VALUE; }
        } 


        /// <summary>
        /// String Literal 255
        /// </summary>
        private  string nUMBER_255 = "255";

        public string NUMBER_255
        {
            get { return nUMBER_255; }
        } 


        /// <summary>
        /// String Literal for Old Guid Code
        /// </summary>
        private  string OLD_GUID_CODE = "ALWAYS\r\nIF CDCUNIQUEID=(.) OR CDCUNIQUEID=\"\"  THEN\r\nASSIGN CDCUNIQUEID = GLOBAL_ID!GetGlobalUniqueID()\r\nASSIGN CDC_UNIQUE_ID=CDCUNIQUEID\r\nEND\r\nEND";

        public string OLD_GUID_CODE1
        {
            get { return OLD_GUID_CODE; }
        } 


        /// <summary>
        /// String Literal OldValue
        /// </summary>
        private  string oLD_VALUE = "OldValue";

        public string OLD_VALUE
        {
            get { return oLD_VALUE; }
        } 


        /// <summary>
        /// String Literal PARENTHESES OPEN
        /// </summary>
        private  string pARANTHESES_OPEN = "(";

        public string PARANTHESES_OPEN
        {
            get { return pARANTHESES_OPEN; }
        } 


        /// <summary>
        /// String Literal PARENTHESES CLOSE
        /// </summary>
        private  string pARANTHESES_CLOSE = ")";

        public string PARANTHESES_CLOSE
        {
            get { return pARANTHESES_CLOSE; }
        } 


        /// <summary>
        /// String Literal PERCENT
        /// </summary>
        private  string pERCENT = "%";

        public string PERCENT
        {
            get { return pERCENT; }
        } 


        /// <summary>
        /// String Literal PERIOD
        /// </summary>
        private  string pERIOD = ".";

        public string PERIOD
        {
            get { return pERIOD; }
        } 


        /// <summary>
        /// String Literal PLUS
        /// </summary>
        private  string pLUS = "+";

        public string PLUS
        {
            get { return pLUS; }
        } 


        /// <summary>
        /// String Literal RIGHT SQUARE BRACKET
        /// </summary>
        private  string rIGHT_SQUARE_BRACKET = "]";

        public string RIGHT_SQUARE_BRACKET
        {
            get { return rIGHT_SQUARE_BRACKET; }
        } 


        /// <summary>
        /// Strlig literal semicolon
        /// </summary>
        private  string sEMI_COLON = @";";

        public string SEMI_COLON
        {
            get { return sEMI_COLON; }
        } 


        /// <summary>
        /// String Literal SINGLE QUOTES
        /// </summary>
        private  string sINGLEQUOTES = "'";

        public string SINGLEQUOTES
        {
            get { return sINGLEQUOTES; }
        } 


        /// <summary>
        /// String Literal SPACE
        /// </summary>
        private  string sPACE = " ";

        public string SPACE
        {
            get { return sPACE; }
        } 


        /// <summary>
        /// String Literal STAR
        /// </summary>
        private  string sTAR = "*";

        public string STAR
        {
            get { return sTAR; }
        } 


        /// <summary>
        /// String Literal TAB
        /// </summary>
        private  string tAB = "\t";

        public string TAB
        {
            get { return tAB; }
        } 


        /// <summary>
        /// String Literal for Table Import Status
        /// </summary>
        private  string tABLE_IMPORT_STATUS = "{0} ({1} {2}{3})";

        public string TABLE_IMPORT_STATUS
        {
            get { return tABLE_IMPORT_STATUS; }
        } 


        /// <summary>
        /// String literal UNDERSCORE
        /// </summary>
        private  string uNDER_SCORE = "_";

        public string UNDER_SCORE
        {
            get { return uNDER_SCORE; }
        } 


    } // class StringLiterals