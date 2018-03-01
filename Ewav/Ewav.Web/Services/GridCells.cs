/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       GridCells.cs
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

namespace Ewav.Web.Services
{
    public class GridCells
    {
        public int ytVal;

        public int YtVal
        {
            get { return ytVal; }
            set { ytVal = value; }
        }
        public int yyVal;

        public int YyVal
        {
            get { return yyVal; }
            set { yyVal = value; }
        }
        public int ynVal;

        public int YnVal
        {
            get { return ynVal; }
            set { ynVal = value; }
        }
        public int ntVal;

        public int NtVal
        {
            get { return ntVal; }
            set { ntVal = value; }
        }
        public int nyVal;

        public int NyVal
        {
            get { return nyVal; }
            set { nyVal = value; }
        }
        public int nnVal;

        public int NnVal
        {
            get { return nnVal; }
            set { nnVal = value; }
        }
        public int ttVal;

        public int TtVal
        {
            get { return ttVal; }
            set { ttVal = value; }
        }
        public int tyVal;

        public int TyVal
        {
            get { return tyVal; }
            set { tyVal = value; }
        }
        public int tnVal;

        public int TnVal
        {
            get { return tnVal; }
            set { tnVal = value; }
        }
        public double yyRowPct;

        public double YyRowPct
        {
            get { return yyRowPct; }
            set { yyRowPct = value; }
        }
        public double ynRowPct;

        public double YnRowPct
        {
            get { return ynRowPct; }
            set { ynRowPct = value; }
        }
        public double nyRowPct;

        public double NyRowPct
        {
            get { return nyRowPct; }
            set { nyRowPct = value; }
        }
        public double nnRowPct;

        public double NnRowPct
        {
            get { return nnRowPct; }
            set { nnRowPct = value; }
        }
        public double tyRowPct;

        public double TyRowPct
        {
            get { return tyRowPct; }
            set { tyRowPct = value; }
        }
        public double tnRowPct;

        public double TnRowPct
        {
            get { return tnRowPct; }
            set { tnRowPct = value; }
        }
        public double yyColPct;

        public double YyColPct
        {
            get { return yyColPct; }
            set { yyColPct = value; }
        }
        public double nyColPct;

        public double NyColPct
        {
            get { return nyColPct; }
            set { nyColPct = value; }
        }
        public double ynColPct;

        public double YnColPct
        {
            get { return ynColPct; }
            set { ynColPct = value; }
        }
        public double nnColPct;

        public double NnColPct
        {
            get { return nnColPct; }
            set { nnColPct = value; }
        }
        public double ytColPct;

        public double YtColPct
        {
            get { return ytColPct; }
            set { ytColPct = value; }
        }
        public double ntColPct;

        public double NtColPct
        {
            get { return ntColPct; }
            set { ntColPct = value; }
        }
        public MySingleTableResults singleTableResults;

        //public StatisticsRepository.cTable.SingleTableResults SingleTableResults
        public MySingleTableResults SingleTableResults
        {
            get { return singleTableResults; }
            set { singleTableResults = value; }
        }

        public double _yySideLength;
        public double YySideLength
        {
            get {
                if (TtVal == 0) return 1;
                return 149.0 * YyVal / TtVal;
            }
        }

        public double _ynSideLength;
        public double YnSideLength
        {
            get {
                if (TtVal == 0) return 1;
                return 149.0 * YnVal / TtVal;
            }
        }

        public double _nySideLength;
        public double NySideLength
        {
            get {
                if (TtVal == 0) return 1;
                return 149.0 * NyVal / TtVal;
            }
        }

        public double _nnSideLength;
        public double NnSideLength
        {
            get {
                if (TtVal == 0) return 1;
                return 149.0 * NnVal / TtVal;
            }
        }

    }
}