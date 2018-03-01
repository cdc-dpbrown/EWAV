/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       EwavMap.cs
 *  Namespace:  Ewav.Mapping    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using Ewav.DTO;

namespace Ewav.Mapping
{
    public class EwavMap
    {
        private readonly Map map;
        private readonly GraphicsLayer graphicsLayer;
        private readonly int maximumFlareCount;

        private readonly int flareClusterRadius;
        private List<EwavLegendItemData> ewavLegendItemList = new List<EwavLegendItemData>();
        private readonly bool useRoads;

        /// <summary>
        /// Initializes a new instance of the <see cref="EwavMap" /> class.
        /// </summary>
        public EwavMap(Map map, int flareClusterRadius, int maximumFlareCount, bool useRoads)
        {
            this.map = map;
            this.maximumFlareCount = maximumFlareCount;
            this.flareClusterRadius = flareClusterRadius;
            this.useRoads = useRoads;
        }

        /// <summary>
        /// Gets or sets the ewav legend item list.kkk
        /// </summary>
        /// <value>The ewav legend item list.</value>
        public List<EwavLegendItemData> EwavLegendItemList
        {
            get
            {
                return this.ewavLegendItemList;
            }
            set
            {
                this.ewavLegendItemList = value;
            }
        }

        private void myOpenStreetMapLayer_Initialized(object sender, EventArgs e)
        {

            // Get the OpenStreetMapLayer.
            ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer myOpenStreetMapLayer = (ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer)sender;

            

        }

        public void AddPointsAsManyGraphicsLayers(List<PointDTOCollection> pointDTOList)
        {
            try
            {
                Queue<SimpleMarkerSymbol> mQueue = this.CreateSimpleMarkerSymbolQueue();
                Queue<SimpleMarkerSymbol> markerSymbolQueue = new Queue<SimpleMarkerSymbol>(mQueue.Reverse<SimpleMarkerSymbol>());

                this.map.Layers.Clear();

                ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer openStreetMapLayer = new ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer();

                if (this.useRoads)
                {
                    openStreetMapLayer.Initialized += myOpenStreetMapLayer_Initialized;
                    map.Layers.Add(openStreetMapLayer);
                }

                foreach (PointDTOCollection pointDTOClass in pointDTOList)
                {
                    SimpleMarkerSymbol simpleMarkerSymbol;

                    //  Pick a symbol for this class              
                    if (markerSymbolQueue.Count == 0)
                    {
                        mQueue = this.CreateSimpleMarkerSymbolQueue();
                        markerSymbolQueue = new Queue<SimpleMarkerSymbol>(mQueue.Reverse<SimpleMarkerSymbol>());
                    }

                    simpleMarkerSymbol = markerSymbolQueue.Dequeue();

                    // Create a graphics layer  
                    GraphicsLayer graphicsLayer = new GraphicsLayer();
                    
                    if (pointDTOClass.Collection.Count > 1)
                    {
                        if (pointDTOList.Count > 1 && pointDTOClass.Collection.Count > 1)
                        {
                            if (pointDTOClass.Collection[0].StrataValue.Contains("\'"))
                            { // [Age] = '25'
                                graphicsLayer.ID = pointDTOClass.Collection[0].StrataValue;
                                int startPos = graphicsLayer.ID.IndexOf("=") + 3;
                                int endPos = graphicsLayer.ID.LastIndexOf("\'");
                                graphicsLayer.ID = graphicsLayer.ID.Substring(startPos, endPos - startPos);
                            }
                            else // maybe the strata value is a # example  
                                 // [Age] = 25
                            {
                                graphicsLayer.ID = pointDTOClass.Collection[0].StrataValue;
                                int startPos = graphicsLayer.ID.IndexOf("=") + 1;
                                int endPos = graphicsLayer.ID.Length;
                                graphicsLayer.ID = graphicsLayer.ID.Substring(startPos, endPos - startPos);
                            }
                        }
                        else
                        {
                            graphicsLayer.ID = pointDTOClass.Collection[0].StrataValue;
                        }
                    }

                    EwavLegendItemData eli = new EwavLegendItemData();
                    eli.Color = simpleMarkerSymbol.Color;
                    eli.Description = graphicsLayer.ID;

                    this.ewavLegendItemList.Add(eli);

                    // Create a clusterer with a flare   
                    FlareClusterer fc = new FlareClusterer();
                    fc.FlareBackground = simpleMarkerSymbol.Color;
                    fc.MaximumFlareCount = this.maximumFlareCount;    // 20  
                    fc.Radius = this.flareClusterRadius;        //  += fc.Radius / 10;  

                    // Add clusterer to graphics layer    
                    graphicsLayer.Clusterer = fc;

                    // Add graphics to graphics layer with             
                    foreach (PointDTO pointDTO in pointDTOClass.Collection)
                    {
                        Graphic graphic = this.createGraphic(pointDTO, simpleMarkerSymbol);
                        Border brdmaptip = new Border();
                        brdmaptip.Background = new SolidColorBrush(Colors.White);
                        brdmaptip.BorderBrush = simpleMarkerSymbol.Color;
                        brdmaptip.BorderThickness = new Thickness(1, 1, 1, 1);

                        // Create a map tip  
                        TextBlock tb = new TextBlock();

                        // if multiple cols are required for map tips later, 
                        // use of pointDTO.MapTip will facilitate it.  
                        tb.Text = graphicsLayer.ID;         // pointDTO.MapTip;                             
                        tb.Margin = new Thickness(5, 5, 5, 5);
                        brdmaptip.Child = tb;

                        graphic.MapTip = brdmaptip;
                        graphic.MapTip.VerticalAlignment = VerticalAlignment.Top;
                        graphic.MapTip.HorizontalAlignment = HorizontalAlignment.Right;

                        graphicsLayer.Graphics.Add(graphic);
                    }



                    // Add graphics layer to map     
                    this.map.Layers.Add(graphicsLayer);
                    this.map.Extent = graphicsLayer.FullExtent;
                    this.map.Zoom(.9);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0} == {1}", ex.Message, ex.StackTrace));
            }
        }

        private Graphic createGraphic(PointDTO pointDTO, SimpleMarkerSymbol simpleMarkerSymbol)
        {
            // Create a MapPoint object and set its SpatialReference and coordinate (X,Y) information. Note: Point 
            // Graphics are known as MapPoint objects.
            MapPoint mapPoint = new MapPoint()
            {
                Y = pointDTO.LatY,
                X = pointDTO.LonX,
                SpatialReference = new SpatialReference(4326)
            };

            Graphic graphic = new Graphic();

            // Apply the Graphic's Geometry and Symbol Properties.  
            graphic.Geometry = ESRI.ArcGIS.Client.Bing.Transform.GeographicToWebMercator(mapPoint);

            // [MapTip]            
            graphic.Attributes.Add("MapTip", pointDTO.MapTip);
            graphic.Attributes.Add("StrataValue", pointDTO.StrataValue);

            graphic.Symbol = simpleMarkerSymbol;         //     as ESRI.ArcGIS.Client.Symbols.Symbol;

            return graphic;
        }

        public void AddUniqueValues(List<PointDTO> pointDTOList, List<UniqueValueInfoDTO> uniqueValueInfoDTOList)
        {
            Queue<SimpleMarkerSymbol> markerSymbolQueue = this.CreateSimpleMarkerSymbolQueue();
            markerSymbolQueue.Reverse<SimpleMarkerSymbol>();

            try
            {
                List<Graphic> graphicsList = new List<Graphic>();

                // First create a list of graphics from the DTO  
                foreach (PointDTO pointDTO in pointDTOList)
                {
                    Graphic graphic = this.createStratifiedGraphic(pointDTO);

                    this.graphicsLayer.Graphics.Add(graphic);
                }

                UniqueValueRenderer r = new UniqueValueRenderer();
                r.Field = "StrataValue";

                foreach (UniqueValueInfoDTO dto in uniqueValueInfoDTOList)
                {
                    UniqueValueInfo x = new UniqueValueInfo();

                    SimpleMarkerSymbol simpleMarkerySymbol = markerSymbolQueue.Dequeue();

                    //  x.Symbol = 7777         

                    x.Symbol = simpleMarkerySymbol;
                    x.Value = dto.Value;

                    r.Infos.Add(x);
                }

                this.graphicsLayer.Renderer = r;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Problem with EwavMap.AddPoints \n{0}\n{1}", ex.Message, ex.StackTrace));
            }
        }

        private Queue<SimpleMarkerSymbol> CreateSimpleMarkerSymbolQueue()
        {
            Queue<SimpleMarkerSymbol> markerSymbolQueue = new Queue<SimpleMarkerSymbol>();

            SimpleMarkerSymbol simpleMarkerySymbol = new SimpleMarkerSymbol();

            simpleMarkerySymbol.Color = this.GetSolidColorBrush("#4400e3");
            simpleMarkerySymbol.Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle;
            simpleMarkerySymbol.Size = 10;

            markerSymbolQueue.Enqueue(simpleMarkerySymbol);

            simpleMarkerySymbol = new SimpleMarkerSymbol();
            simpleMarkerySymbol.Color = this.GetSolidColorBrush("#ffcc00");
            simpleMarkerySymbol.Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle;
            simpleMarkerySymbol.Size = 10;

            markerSymbolQueue.Enqueue(simpleMarkerySymbol);

            simpleMarkerySymbol = new SimpleMarkerSymbol();
            simpleMarkerySymbol.Color = this.GetSolidColorBrush("#f56f06");
            simpleMarkerySymbol.Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle;
            simpleMarkerySymbol.Size = 10;

            markerSymbolQueue.Enqueue(simpleMarkerySymbol);

            simpleMarkerySymbol = new SimpleMarkerSymbol();
            simpleMarkerySymbol.Color = this.GetSolidColorBrush("#a200e3");
            simpleMarkerySymbol.Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle;
            simpleMarkerySymbol.Size = 10;

            markerSymbolQueue.Enqueue(simpleMarkerySymbol);

            simpleMarkerySymbol = new SimpleMarkerSymbol();
            simpleMarkerySymbol.Color = this.GetSolidColorBrush("#00bce3");
            simpleMarkerySymbol.Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle;
            simpleMarkerySymbol.Size = 10;

            markerSymbolQueue.Enqueue(simpleMarkerySymbol);

            simpleMarkerySymbol = new SimpleMarkerSymbol();
            simpleMarkerySymbol.Color = this.GetSolidColorBrush("#85bc00");
            simpleMarkerySymbol.Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle;
            simpleMarkerySymbol.Size = 10;

            markerSymbolQueue.Enqueue(simpleMarkerySymbol);

            simpleMarkerySymbol = new SimpleMarkerSymbol();
            simpleMarkerySymbol.Color = this.GetSolidColorBrush("#df191e");
            simpleMarkerySymbol.Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle;
            simpleMarkerySymbol.Size = 10;

            markerSymbolQueue.Enqueue(simpleMarkerySymbol);

            return markerSymbolQueue;
        }

        private Graphic createStratifiedGraphic(PointDTO pointDTO)
        {
            // Create a MapPoint object and set its SpatialReference and coordinate (X,Y) information. Note: Point 
            // Graphics are known as MapPoint objects.
            MapPoint mapPoint = new MapPoint()
            {
                Y = pointDTO.LatY,
                X = pointDTO.LonX,
                SpatialReference = new SpatialReference(4326)
            };

            // Create a new instance of a SimpleMarkerSymbol and set its Color, Style, and Size Properties.
            SimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbol();
            simpleMarkerSymbol.Color = new System.Windows.Media.SolidColorBrush(Colors.Red);
            simpleMarkerSymbol.Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle;
            simpleMarkerSymbol.Size = 10;

            Graphic graphic = new Graphic();

            // Apply the Graphic's Geometry and Symbol Properties.  
            graphic.Geometry = ESRI.ArcGIS.Client.Bing.Transform.GeographicToWebMercator(mapPoint);

            //    [dto].MapTip     
            //  graphic.Attributes.Add("dto", pointDTO);

            // [MapTip]            
            graphic.Attributes.Add("MapTip", pointDTO.MapTip);
            graphic.Attributes.Add("StrataValue", pointDTO.StrataValue);

            // Manually create a map tip    
            // TextBlock t = new TextBlock();
            // t.Text = pointDTO.MapTip;
            // graphic.MapTip = t;    

            graphic.Symbol = simpleMarkerSymbol;         //     as ESRI.ArcGIS.Client.Symbols.Symbol;

            return graphic;
        }

        private SolidColorBrush GetSolidColorBrush(string hex)
        {
            byte r = (byte)(Convert.ToUInt32(hex.Substring(1, 2), 16));
            byte g = (byte)(Convert.ToUInt32(hex.Substring(3, 2), 16));
            byte b = (byte)(Convert.ToUInt32(hex.Substring(5, 2), 16));

            SolidColorBrush myBrush = new SolidColorBrush(Color.FromArgb(255, r, g, b));

            return myBrush;
        }
    }
}