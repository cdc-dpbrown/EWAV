/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ISettings.cs
 *  Namespace:  Epi    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;

namespace Epi1
{
	/// <summary>
	/// Defines all the settings to be implemented at three levels: Default, Config, Temp
	/// </summary>
	public interface ISettings
	{
		/// <summary>
		/// MetaDataDao for Background Image (settings)
		/// </summary>
		string BackgroundImage
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for Default database format (settings)
		/// </summary>
		int DefaultDatabaseFormat
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for Default data format (settings)
		/// </summary>
		int DefaultDataFormat
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for Fields prompt font (settings)
		/// </summary>
		string EditorFontName
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for Fields prompt font size (settings)
		/// </summary>
		System.Decimal EditorFontSize
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for Fields control font (settings)
		/// </summary>
		string ControlFontName
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for Fields control font size (settings)
		/// </summary>
		System.Decimal ControlFontSize
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for Include missing values (settings)
		/// </summary>
		bool? IncludeMissingValues
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for Language (settings)
		/// </summary>
		string Language
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for MRU Projects count (settings)
		/// </summary>
		int MRUProjectsCount
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for MRU Views Count(settings)
		/// </summary>
		int MRUViewsCount
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for Project Directory (settings)
		/// </summary>
		string ProjectDirectory
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for Record Processing Scope (settings)
		/// </summary>
		int RecordProcessingScope
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for Representation of Yes (settings)
		/// </summary>
		string RepresentationOfYes
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for Representation of No(settings)
		/// </summary>
		string RepresentationOfNo
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for Representation of missing (settings)
		/// </summary>
		string RepresentationOfMissing
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for Show Complete (settings)
		/// </summary>
		bool? ShowCompletePrompt
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for Show Graphics(settings)
		/// </summary>
		bool? ShowGraphics
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for Show HyperLinks (settings)
		/// </summary>
		bool? ShowHyperlinks
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for Show Percents(settings)
		/// </summary>
		bool? ShowPercents
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for Show Selection(settings)
		/// </summary>
		bool? ShowSelection
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for Show Tables(settings)
		/// </summary>
		bool? ShowTables
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for Snap to Grid(settings)
		/// </summary>
		bool? SnapToGrid
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for Statisatics Level(settings)
		/// </summary>
		int StatisticsLevel
		{
			get;
			set;
		}

		/// <summary>
		/// MetaDataDao for Working Directory (settings)
		/// </summary>
		string WorkingDirectory
		{
			get;
			set;
		}
	}
}