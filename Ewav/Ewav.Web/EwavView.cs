/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       EwavView.cs
 *  Namespace:  Ewav.Web    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;


namespace Ewav.Web
{
    public class EwavView : Epi.IView
    {
        public string CheckCode
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public string CheckCodeAfter
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public string CheckCodeBefore
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public string CheckCodeVariableDefinitions
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public string CurrentGlobalRecordId
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public int CurrentRecordId
        {
            get { throw new NotImplementedException(); }
        }
        public int CurrentRecordStatus
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public string DisplayName
        {
            get { throw new NotImplementedException(); }
        }
        public object FieldLockToken
        {
            get { throw new NotImplementedException(); }
        }
        public Epi.Collections.FieldCollectionMaster Fields
        {
            get { throw new NotImplementedException(); }
        }
        public Epi.Fields.ForeignKeyField ForeignKeyField
        {
            get { throw new NotImplementedException(); }
        }
        public bool ForeignKeyFieldExists
        {
            get { throw new NotImplementedException(); }
        }
        public string FromViewSQL
        {
            get { throw new NotImplementedException(); }
        }
        public string FullName
        {
            get { throw new NotImplementedException(); }
        }
        public Epi.Fields.GlobalRecordIdField GlobalRecordIdField
        {
            get { throw new NotImplementedException(); }
        }
        public int Id
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public bool IsDirty
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public bool IsRelatedView
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public bool MustRefreshFieldCollection
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public int PageHeight
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public string PageLabelAlign
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public string PageOrientation
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public List<Epi.Page> Pages
        {
            get { throw new NotImplementedException(); }
        }
        public int PageWidth
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public Epi.View ParentView
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public Epi.Project Project
        {
            get { throw new NotImplementedException(); }
        }
        public string RecordCheckCodeAfter
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public string RecordCheckCodeBefore
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public Epi.Fields.RecStatusField RecStatusField
        {
            get { throw new NotImplementedException(); }
        }
        public bool ReturnToParent
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public List<string> TableColumnNames
        {
            get { throw new NotImplementedException(); }
        }
        public string TableName
        {
            get
            {
                //  throw new NotImplementedException();
                return "FoodHistory1";

            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public Epi.Fields.UniqueIdentifierField UniqueIdentifierField
        {
            get { throw new NotImplementedException(); }
        }
        public Epi.Fields.UniqueKeyField UniqueKeyField
        {
            get { throw new NotImplementedException(); }
        }
        public System.Xml.XmlElement ViewElement
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public string ComposeFieldNameFromPromptText(string promptText)
        {
            throw new NotImplementedException();
        }
        public void CopyFrom(Epi.View other)
        {
            throw new NotImplementedException();
        }
        public Epi.Page CreatePage(string name, int position)
        {
            throw new NotImplementedException();
        }
        public void DeleteDataTables()
        {
            throw new NotImplementedException();
        }
        public void DeletePage(Epi.Page page)
        {
            throw new NotImplementedException();
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
        public List<Epi.View> GetDescendantViews()
        {
            throw new NotImplementedException();
        }
        public Epi.Fields.Field GetFieldById(int fieldId)
        {
            throw new NotImplementedException();
        }
        public int GetFieldId(System.Xml.XmlElement viewElement)
        {
            throw new NotImplementedException();
        }
        public Epi.Collections.NamedObjectCollection<Epi.Fields.Field> GetFieldsOnPage(Epi.Page page)
        {
            throw new NotImplementedException();
        }
        public int GetFirstRecordId()
        {
            throw new NotImplementedException();
        }
        public int GetLastRecordId()
        {
            throw new NotImplementedException();
        }
        public Epi.Data.Services.IMetadataProvider GetMetadata()
        {
            throw new NotImplementedException();
        }
        public System.Data.DataTable GetMirrorableFieldsAsDataTable()
        {
            throw new NotImplementedException();
        }
        public int GetNextRecordId(int currentRecordId)
        {
            throw new NotImplementedException();
        }
        public Epi.Page GetPageById(int pageId)
        {
            throw new NotImplementedException();
        }
        public Epi.Page GetPageByPosition(int position)
        {
            throw new NotImplementedException();
        }
        public int GetPageId(System.Xml.XmlNode pagesNode)
        {
            throw new NotImplementedException();
        }
        public void GetParent(int relatedViewId)
        {
            throw new NotImplementedException();
        }
        public int GetPreviousRecordId(int currentRecordId)
        {
            throw new NotImplementedException();
        }
        public Epi.Project GetProject()
        {
            throw new NotImplementedException();
        }
        public int GetRecordCount()
        {
            throw new NotImplementedException();
        }
        public bool IsViewRecordEmpty()
        {
            throw new NotImplementedException();
        }
        public void LoadFirstRecord()
        {
            throw new NotImplementedException();
        }
        public void LoadLastRecord()
        {
            throw new NotImplementedException();
        }
        public void LoadNextRecord(int currentRecordId)
        {
            throw new NotImplementedException();
        }
        public void LoadPreviousRecord(int currentRecordId)
        {
            throw new NotImplementedException();
        }
        public void LoadRecord(int recordID)
        {
            throw new NotImplementedException();
        }
        public void RemoveFromCollection(Epi.Fields.Field field)
        {
            throw new NotImplementedException();
        }
        public void RunCheckCode(Epi.IModule module, string checkCode)
        {
            throw new NotImplementedException();
        }
        public int SaveRecord(int recordId)
        {
            throw new NotImplementedException();
        }
        public int SaveRecord()
        {
            throw new NotImplementedException();
        }
        public void SaveToCollection(Epi.Fields.Field field)
        {
            throw new NotImplementedException();
        }
        public void SaveToDb()
        {
            throw new NotImplementedException();
        }
        public void SetTableName(string tableName)
        {
            throw new NotImplementedException();
        }
        public Epi.Data.IDbDriver Database
        {
            get { throw new NotImplementedException(); }
        }
    }
}