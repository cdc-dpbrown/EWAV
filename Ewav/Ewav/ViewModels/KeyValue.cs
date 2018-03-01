/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       KeyValue.cs
 *  Namespace:  Ewav.ViewModels    
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
using Ewav.Common;

namespace Ewav.ViewModels
{
    public class KeyValue
    {
        public int Key { get; set; }
        public EwavContextMenuItem Value { get; set; }

        public KeyValue()
        {

        }

        public KeyValue(int index, EwavContextMenuItem item)
        {
            Key = index;
            Value = item;
        }
    }

    public class ControlMetaInfo
    {
        private string controlName;

        public string ControlName
        {
            get { return controlName; }
            set { controlName = value; }
        }

        private string controlUIName;

        public string ControlUIName
        {
            get { return controlUIName; }
            set { controlUIName = value; }
        }

        private string type;

        public string Type
        {   
            get { return type; }
            set { type = value; }
        }

        private int contextMenuIndex;

        public int ContextMenuIndex
        {
            get { return contextMenuIndex; }
            set { contextMenuIndex = value; }
        } 
    }
}