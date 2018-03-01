/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MockItems-Temp.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Linq;
using System.Collections.Generic;

using SimpleMvvmToolkit;

namespace Ewav
{
    public static class MockItems
    {
        public static List<Item> GetItems()
        {
            return new List<Item>
            {
                new Item { Id = 1, Name = "Item 1" },
                new Item { Id = 2, Name = "Item 2" },
                new Item { Id = 3, Name = "Item 3" },
                new Item { Id = 4, Name = "Item 4" },
                new Item { Id = 5, Name = "Item 5" },
                new Item { Id = 6, Name = "Item 6" },
                new Item { Id = 7, Name = "Item 7" },
                new Item { Id = 8, Name = "Item 8" },
                new Item { Id = 9, Name = "Item 9" },
                new Item { Id = 10, Name = "Item 10" }
            };
        }
    }

    public class Item : ModelBase<Item>
    {
        private int id;
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged(m => m.Id);
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged(m => m.Name);
            }
        }
    }
}