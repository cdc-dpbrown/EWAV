/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       IView.cs
 *  Namespace:  Epi    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.Collections.Generic;
using System.Data;
using System.Xml;
using Epi.Collections;
using Epi.Data;
using Epi.Data.Services;
using Epi.Fields;

namespace Epi
{
    public interface IView : ITable, INamedObject
    {
        /// <summary>
        /// </summary>
        string CheckCode { get; set; }
        /// <summary>
        /// Gets/sets the "Check Code After" flag.
        /// </summary>
        string CheckCodeAfter { get; set; }
        /// <summary>
        /// Gets/sets the "Check Code Before" flag.
        /// </summary>
        string CheckCodeBefore { get; set; }
        /// <summary>
        /// Gets/sets the "Check Code Variable Definitions" flag.
        /// </summary>
        string CheckCodeVariableDefinitions { get; set; }
        /// <summary>
        /// Returns the curent record status
        /// </summary>
        string CurrentGlobalRecordId { get; set; }
        /// <summary>
        /// Returns the Current record Id
        /// </summary>
        int CurrentRecordId { get; }
        /// <summary>
        /// Returns the curent record status
        /// </summary>
        int CurrentRecordStatus { get; set; }
        /// <summary>
        /// Returns the display name of the <see cref="Epi.EwavView"/>.
        /// </summary>
        string DisplayName { get; }
        object FieldLockToken { get; }
        /// <summary>
        /// Master collection of all fields in the view.
        /// </summary>
        FieldCollectionMaster Fields { get; }
        /// <summary>
        /// Returns the foreign key fields of the unique key.
        /// </summary>
        ForeignKeyField ForeignKeyField { get; }
        bool ForeignKeyFieldExists { get; }
        string FromViewSQL { get; }
        /// <summary>
        /// Gets the fully-qualified project:view name.
        /// </summary>
        string FullName { get; }
        /// <summary>
        /// Returns the unique key fields.
        /// </summary>
        GlobalRecordIdField GlobalRecordIdField { get; }
        /// <summary>
        /// Gets/sets the <see cref="Epi.EwavView"/> Id.
        /// </summary>
        int Id { get; set; }
        /// <summary>
        /// Gets/sets whether the view is 'dirty' and needs to be saved.
        /// </summary>
        bool IsDirty { get; set; }
        /// <summary>
        /// Gets/sets the "Is Related EwavView" flag.
        /// </summary>
        bool IsRelatedView { get; set; }
        /// <summary>
        /// Gets/sets the "Must Refresh Fields Collection" flag.
        /// </summary>
        bool MustRefreshFieldCollection { get; set; }
        /// <summary>
        /// Returns the name of the <see cref="Epi.EwavView"/>.
        /// </summary>
        string Name { get; set; }
        int PageHeight { get; set; }
        string PageLabelAlign { get; set; }
        string PageOrientation { get; set; }
        /// <summary>
        /// Returns a collection of all pages of the <see cref="Epi.EwavView"/>.
        /// </summary>
        List<Page> Pages { get; }
        int PageWidth { get; set; }
        /// <summary>
        /// EwavView that this view is related to if IsRelatedView = true.
        /// </summary>
        View ParentView { get; set; }
        /// <summary>
        /// Gets the view's <see cref="Epi.Project"/>.
        /// </summary>
        Project Project { get; }
        /// <summary>
        /// Gets/sets the "Record Check Code After" flag.
        /// </summary>
        string RecordCheckCodeAfter { get; set; }
        /// <summary>
        /// Gets/sets the "Record Check Code Before" flag.
        /// </summary>
        string RecordCheckCodeBefore { get; set; }
        /// <summary>
        /// Returns the record status fields.
        /// </summary>
        RecStatusField RecStatusField { get; }
        /// <summary>
        /// Gets/sets the "Should return to parent after one record" flag (specifies 1-to-1 relationship
        /// if true, specifies 1-to-many if false; only applies to related views).
        /// </summary>
        bool ReturnToParent { get; set; }
        /// <summary>
        /// TODO: need to implement this method
        /// </summary>
        List<string> TableColumnNames { get; }
        /// <summary>
        /// The name of the view's collected data table
        /// </summary>
        string TableName { get; set; }
        /// <summary>
        /// Returns the Unique Identifier Fields.
        /// </summary>
        UniqueIdentifierField UniqueIdentifierField { get; }
        /// <summary>
        /// Returns the unique key fields.
        /// </summary>
        UniqueKeyField UniqueKeyField { get; }
        /// <summary>
        /// The view element of the view 
        /// </summary>
        XmlElement ViewElement { get; set; }
        /// <summary>
        /// Composes a fields name from a prompt name.
        /// </summary>
        /// <param name="promptText">String entered as prompt</param>
        /// <returns>Fields name</returns>
        string ComposeFieldNameFromPromptText(string promptText);

        /// <summary>
        /// Copies a view object into this
        /// </summary>
        void CopyFrom(View other);

        /// <summary>
        /// Creates a new page and adds it to the page collection
        /// </summary>
        /// <param name="name">Name of the page</param>
        /// <param name="position">Position of the page</param>
        /// <returns>The new page object</returns>
        Page CreatePage(string name, int position);

        /// <summary>
        /// Delete a view's data table(s)
        /// </summary>
        void DeleteDataTables();

        /// <summary>
        /// Delete a page from a view
        /// </summary>
        void DeletePage(Page page);

        /// <summary>
        /// Implements IDisposable.Dispose() method
        /// </summary>
        void Dispose();

        /// <summary>
        /// Gets a list of all descendant views.
        /// </summary>
        /// <returns>List of Views</returns>
        List<View> GetDescendantViews();

        /// <summary>
        /// Fetches a fields from it's collection by it's Id.
        /// </summary>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        Field GetFieldById(int fieldId);

        /// <summary>
        /// Returns the next available fields Id on a <see cref="Epi.EwavView"/>.
        /// </summary>
        /// <param name="viewElement">An <see cref="Epi.EwavView"/> element.</param>
        /// <returns></returns>
        int GetFieldId(XmlElement viewElement);

        /// <summary>
        /// Gets a collection of renderable <see cref="Epi.Fields"/> on an <see cref="Epi.Page"/>
        /// </summary>
        /// <param name="page"><see cref="Epi.Page"/></param>
        /// <returns>A named object collection of renderable <see cref="Epi.Fields"/>.</returns>
        NamedObjectCollection<Field> GetFieldsOnPage(Page page);

        /// <summary>
        /// Gets the record Id for the first record
        /// </summary>
        /// <returns>Id of the first record</returns>
        int GetFirstRecordId();

        /// <summary>
        /// Gets the record Id for the last record
        /// </summary>
        /// <returns>Id of the last record</returns>
        int GetLastRecordId();

        /// <summary>
        /// Shortcut for getting Metadata
        /// </summary>
        /// <returns>Metadata Provider.</returns>
        IMetadataProvider GetMetadata();

        /// <summary>
        /// Returns  a table of mirrorable fields
        /// </summary>
        DataTable GetMirrorableFieldsAsDataTable();

        /// <summary>
        /// Returns the next record Id
        /// </summary>
        /// <param name="currentRecordId">Current record Id.</param>
        /// <returns>Next record Id.</returns>
        int GetNextRecordId(int currentRecordId);

        /// <summary>
        /// Returns an <see cref="Epi.Page"/> object by its id.
        /// </summary>
        /// <param name="pageId">The Id of the page.</param>
        /// <returns><see cref="Epi.Page"/></returns>
        Page GetPageById(int pageId);

        /// <summary>
        /// Returns an <see cref="Epi.Page"/> object by its position.
        /// </summary>
        /// <param name="position">The index of the page</param>
        /// <returns><see cref="Epi.Page"/></returns>
        Page GetPageByPosition(int position);

        /// <summary>
        /// Retrieves the page Id
        /// </summary>
        /// <param name="pagesNode">The XML pages node</param>
        /// <returns>The page id</returns>
        int GetPageId(XmlNode pagesNode);

        void GetParent(int relatedViewId);

        /// <summary>
        /// Returns the previous record Id
        /// </summary>
        /// <param name="currentRecordId">Current record Id.</param>
        /// <returns>Previous record Id.</returns>
        int GetPreviousRecordId(int currentRecordId);

        /// <summary>
        /// Gets a reference to the Epi7 project object.
        /// </summary>
        /// <returns></returns>
        Project GetProject();

        /// <summary>
        /// Gets the record count for the current view
        /// </summary>
        /// <returns>Record count</returns>
        int GetRecordCount();

        /// <summary>
        /// Checks if all input fields are empty
        /// </summary>
        /// <returns>true if all input fields are empty; otherwise false</returns>
        bool IsViewRecordEmpty();

        /// <summary>
        /// Loads the first record into view
        /// </summary>
        void LoadFirstRecord();

        /// <summary>
        /// Loads the last record into this <see cref="Epi.EwavView"/>.
        /// </summary>
        void LoadLastRecord();

        /// <summary>
        /// Loads the next to the current record into this <see cref="Epi.EwavView"/> by Id.
        /// </summary>
        /// <param name="currentRecordId">Current record Id.</param>
        void LoadNextRecord(int currentRecordId);

        /// <summary>
        /// Loads the prior to the current record into this <see cref="Epi.EwavView"/> by Id.
        /// </summary>
        /// <param name="currentRecordId">Current record Id.</param>
        void LoadPreviousRecord(int currentRecordId);

        /// <summary>
        /// Gets the data for the current record in the current view 
        /// </summary>
        /// <param name="recordID">Record Id</param>
        void LoadRecord(int recordID);

        void RemoveFromCollection(Field field);

        /// <summary>
        /// Runs the specified check code
        /// </summary>
        /// <param name="module">current module</param>
        /// <param name="checkCode">Check code block to run</param>
        void RunCheckCode(IModule module, string checkCode);

        /// <summary>
        /// Saves the current record
        /// </summary>		
        /// <param name="recordId">The unique record</param>
        /// <returns>the record Id</returns>
        int SaveRecord(int recordId);

        /// <summary>
        /// Inserts the current record
        /// </summary>
        /// <returns>Record Id</returns>
        int SaveRecord();

        void SaveToCollection(Field field);

        /// <summary>
        /// Save to database
        /// </summary>
        void SaveToDb();

        /// <summary>
        /// Overrides TableName property value
        /// </summary>
        /// <param name="tableName"></param>
        void SetTableName(string tableName);
    }
 }