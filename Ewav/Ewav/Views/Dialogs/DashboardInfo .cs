/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       DashboardInfo .cs
 *  Namespace:  nothing    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.ComponentModel;

public class DashboardInfo : INotifyPropertyChanged
{
    private string datasource;
    private string dateSaved;
    private string description;
    private string status;
    private string title;
    public DashboardInfo()
    {
    }

    public DashboardInfo(string title, string description, string dateSaved, string datasource, string status)
    {
        this.title = title;
        this.description = description;
        this.dateSaved = dateSaved;
        this.datasource = datasource;
        this.Status = status;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public string DataSource
    {
        get
        {
            return datasource;
        }
        set
        {
            datasource = value;
            OnPropertyChanged("DataSource");
        }
    }
    public string DateSaved
    {
        get
        {
            return dateSaved;
        }
        set
        {
            dateSaved = value;
            OnPropertyChanged("DateSaved");
        }
    }
    public string Description
    {
        get
        {
            return description;
        }
        set
        {
            description = value;
            OnPropertyChanged("Description");
        }
    }
    public string Status
    {
        get
        {
            return status;
        }
        set
        {
            status = value;
            OnPropertyChanged("IsShared");
        }
    }
    public string Title
    {
        get
        {
            return title;
        }
        set
        {
            title = value;
            OnPropertyChanged("Title");
        }
    }
    public override string ToString()
    {
        return title.ToString();
    }

    protected void OnPropertyChanged(string info)
    {
        //PropertyChangedEventHandler handler = PropertyChanged;
        //if (handler != null)
        //{
        //    handler(this, new PropertyChangedEventArgs(info));
        //}
    }
}