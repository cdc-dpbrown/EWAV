/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       IItemListServiceAgent.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Linq;
using System.Collections.Generic;

namespace Ewav
{
    // Represents a typical service agent interface
    public interface IItemListServiceAgent
    {
        // Retrieve entities from the Service
        void GetItems(Action<List<Item>, Exception> completed);

        // Insert an entity
        void AddItem(Item item);

        // Remove an entity
        void RemoveItem(Item item);

        // Save changes to entities
        void SaveChanges(Action<Exception> completed);

        // Reject changes to entities
        void RejectChanges();
    }
}