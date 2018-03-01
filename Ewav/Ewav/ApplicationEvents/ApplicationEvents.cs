/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ApplicationEvents.cs
 *  Namespace:  Ewav.Client.Application    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
namespace Ewav.Client.Application
{
    /// <summary>
    /// When a connetion is made to the datasouce the allColumns are available to the application  
    /// </summary>
    public delegate void ColumnsLoadedEventEventHandler(object o, ColumnsLoadedEventEventArgs e);
    /// <summary>
    /// After the user selects a datasource the Connection string is avail to the application   
    /// </summary>   
    public delegate void ConnectionStringReadyEventHandler(object o, ConnectionStringEventArgs e);
    /// <summary>
    /// When the filter super gadget applies a new filter to the selected data source the filtered 
    /// record couht is avial to the application      
    /// </summary>
    public delegate void FilteredRecordcountUpdatedEventHandler(object o, FilteredRecordcountUpdatedEventArgs e);
    /// <summary>
    /// When a conection is made to the data source total record count is avail to the application     
    /// </summary>
    public delegate void TotalRecordcountLoadedEventHandler(object o, TotalRecordcountLoadedEventArgs e);
    /// <summary>
    /// When the data source name is changed the name is made avail to the application  
    /// </summary>
    public delegate void DatasourceChangedEventHandler(object o, DatasourceChangedEventArgs e);


    /// <summary>
    /// When new positions for a(n) element is calculated a Dictionary of new positions is avail to 
    /// all elements     
    ///      
    /// </summary>
    /// <param name="o"></param>
    /// <param name="e"></param>
    public delegate void ElementMoveEventHandler(object o, ElementMoveEventArgs e);

    public delegate void MefAvailableGadgetListReadyEventHandler(object o, MefAvailableGadgetListReadyEventArgs e);

    //    public delegate void DatasourceListReadyEventHandler(object o, DatasourceListReadyEventArgs e);

    public delegate void ApplyFilterEventHandler(object o);

    public delegate void ReadFilterStringEventHandler(object o);

    public delegate void FilterStringUpdatedEventHandler(object o);

    public delegate void PreColumnChangedEventHandler(object o);

    public delegate void DefinedVariableAddedEventHandler(object o);

    public delegate void DefinedVariableInUseDeletedEventHandler(object o);

    public delegate void DefinedVariableNotInUseDeletedEventHandler(object o);

    public delegate void DragCanvasRightMouseDownEventHandler(DashboardMainPage d);

    public delegate void SaveCanvasCompletedEventHandler(object o);

    public delegate void RulesAddedEventHandler(object o);

    public delegate void FiltersDeserializedEventHandler(object o);

    public delegate void ErrorNoticeEventHandler(SimpleMvvmToolkit.NotificationEventArgs<Exception> o);

    public delegate void UnloadedEventHandler(object o);

    public delegate void CanvasListLoadedEventHandler(object o);

    public delegate void AllUsersReadEventHandler(object o);

    public delegate void ShareCanvasCompletedEventHandler(object o);

    public delegate void EmailSentCompletedEventHandler(object o);

    public delegate void ShareCanvasLoadedEventHandler(object o);

    public delegate void DeleteCanvasCompletedEventHandler(object o);

    public delegate void CUDUserCompletedEventHandler(object o);

    public delegate void CUDDatasourceCompletedEventHandler(object o);

    public delegate void ClearDataFiltersEventHandler(object o);

    public delegate void ClearDefinedVariablesEventHandler(object o);

    public delegate void UpdateDataFilterCountEventHandler(object o);


    public delegate void CanvasHeightChangedHandler(CanvasChangedEventArgs e);


    public delegate void CanvasWidthChangedHandler(CanvasChangedEventArgs e);

 

    public delegate void CanvasSizeChangedHandler(CanvasChangedEventArgs e);    

    public delegate void CanvasSnapshotCompleteHandler(string CanvasSnapshotAsBase64);

















    public static class ApplicationEvents
    {

        /// <summary>
        /// Occurs when [datasource watcher reset notification event].
        /// </summary>
        public static event EventHandler DatasourceWatcherEvent;




    }
}