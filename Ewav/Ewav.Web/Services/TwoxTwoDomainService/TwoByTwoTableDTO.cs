/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       TwoByTwoTableDTO.cs
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
    public class TwoxTwoTableDTO
    {
        private string columnName1 = Ewav.Web.Config.ConfigDataSet.RepresentationOfYes;//"Yes";
        private string columnName2 = Ewav.Web.Config.ConfigDataSet.RepresentationOfNo;//"No";

        private string tableName;  


        private int nn;
        private int ny;
        private int yn;
        private int yy;
        
        public TwoxTwoTableDTO()
        {
        }

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName
        {
            get
            {
                return this.tableName;
            }
            set
            {
                this.tableName = value;
            }
        }

        public string ColumnName1
        {
            get
            {
                return this.columnName1;
            }
            set
            {
                this.columnName1 = value;
            }
        }
        public string ColumnName2
        {
            get
            {
                return this.columnName2;
            }
            set
            {
                this.columnName2 = value;
            }
        }
        public int Nn
        {
            get
            {
                return this.nn;
            }
            set
            {
                this.nn = value;
            }
        }
        public int Ny
        {
            get
            {
                return this.ny;
            }
            set
            {
                this.ny = value;
            }
        }
        public int Yn
        {
            get
            {
                return this.yn;
            }
            set
            {
                this.yn = value;
            }
        }
        public int Yy
        {
            get
            {
                return this.yy;
            }
            set
            {
                this.yy = value;
            }
        }


    }
}