/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       IEwavGadget2.cs
 *  Namespace:  Ewav        
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Ewav.Web.EpiDashboard;
using Ewav.BAL;
using Ewav.ExtensionMethods;
using Ewav.ViewModels;
using Ewav.ViewModels.Gadgets;
using Ewav.Web.Services;

namespace Ewav    

{


    public interface IEwavGadget2 : Ewav.IGadget, Ewav.IEwavGadget
    {
        /// <summary>
        /// Associates the event handlers with Events. 
        /// </summary>
        void Construct();

        void UnloadGadget();

        void applicationViewModel_UnloadedEvent(object o);

        void applicationViewModel_PreColumnChangedEvent(object o);

        void applicationViewModel_DefinedVariableNotInUseDeletedEvent(object o);

        void applicationViewModel_DefinedVariableInUseDeletedEvent(object o);

        /// <summary>
        /// Resets the gadget to initial state.
        /// </summary>
        void ResetGadget();

        bool IsDFUsedInThisGadget();

        void applicationViewModel_DefinedVariableAddedEvent(object o);

        void SaveColumnValues();

        /// <summary>
        /// Searches current index of the columns.
        /// </summary>
        void SearchIndex();

        void applicationViewModel_ApplyDataFilterEvent(object o);

        void SelectDropDownValues();

        void RefreshResults();

        void CreateFromXml(System.Xml.Linq.XElement element);

  

        void CloseGadget();

        /// <summary>
        /// Clears the results.
        /// </summary>
        void ClearResults();

        /// <summary>
        /// Sets the gadget's state to 'finished with error' mode
        /// </summary>
        /// <param name="errorMessage">The error message to display</param>
        void RenderFinishWithError(string errorMessage);

        /// <summary>
        /// Used to generate the list of variables and options for the GadgetParameters object
        /// </summary> 
        void CreateInputVariableList();
    }
}