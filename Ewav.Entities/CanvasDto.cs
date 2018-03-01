/*  ----------------------------------------------------------------------------
*  Emergint Technologies, Inc.
*  ----------------------------------------------------------------------------
*  Epi Info™ - Web Analysis & Visualization
*  ----------------------------------------------------------------------------
*  File:       CanvasDto.cs
*  Namespace:  Ewav    
*
*  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
*  Created:    26/08/2013    
*  Summary:	no summary     
*  ----------------------------------------------------------------------------
*/

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Ewav
{
    /// <summary>
    /// This Class will act as a communicating object between client and Server.
    /// </summary>
    public class CanvasDto
    {
        /// <summary>
        /// Gets or Sets Canvas Description
        /// </summary>
        public string CanvasDescription { get; set; }

        /// <summary>
        /// Gets or sets the canvas id.
        /// </summary>
        /// <value>The canvas id.</value>
        public int CanvasId { get; set; }


        private string _ewavPermalink = "";
        private string _ewavLITEPermalink = "";

        public string EwavPermalink
        {
            get
            {
                return _ewavPermalink;
            }
            set
            {
                _ewavPermalink = value;
            }
        }

        public string EwavLITEPermalink
        {
            get
            {
                return _ewavLITEPermalink;
            }
            set
            {
                _ewavLITEPermalink = value;
            }
        }


        /// <summary>
        /// Gets or sets CanvasName
        /// </summary>
        public string CanvasName { get; set; }


        public Guid CanvasGUID { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>The created date.</value>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        /// <value>The culture.</value>
        public string Culture { set; get; }

        /// <summary>
        /// Sets and gets datasource Name
        /// </summary>
        public string Datasource { get; set; }

        /// <summary>
        /// Gets or sets the meta datasource ID.
        /// </summary>
        /// <value>The meta datasource ID.</value>
        public int DatasourceID { get; set; }


        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public double Height { get; set; }

        /// <summary>
        /// Gets or sets the is new canvas.
        /// </summary>
        /// <value>The is new canvas.</value>
        public bool IsNewCanvas { get; set; }

        /// <summary>
        /// Gets or sets the is shared.
        /// </summary>
        /// <value>The is shared.</value>
        public bool IsShared { get; set; }

        /// <summary>
        /// Gets or sets the modified date.
        /// </summary>
        /// <value>The modified date.</value>
        public DateTime ModifiedDate { get; set; }

        public string Status { get; set; }

        /// <summary>
        /// Gets or sets User ID
        /// </summary>
        public int UserId { get; set; }

        //  a jpeg as a  Base64  string
        public string CanvasSnapshotAsBase64 { get; set; }





        /// <summary>
        /// Gets or sets XML Data
        /// </summary>
        public XElement XmlData { get; set; }

        /// <summary>
        /// Gets or sets database object name.
        /// </summary>
        public string DatabaseObjectName { get; set; }
    }
}