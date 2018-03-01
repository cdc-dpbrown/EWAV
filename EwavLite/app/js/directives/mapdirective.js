directiveModule.directive('mapdirective', function () {
    return {
        restrict: 'A',

        link: function ($scope, elm, attrs) {

            function CreateColorQueue(queue) {
                var color = [223, 25, 30, 0.7]; // "#df191e";
                queue.push(color);

                color = [133, 188, 0, 0.7]; //"#85bc00";
                queue.push(color);

                color = [0, 188, 227, 0.7]; //"#00bce3";
                queue.push(color);

                color = [162, 0, 227, 0.7]; //"#a200e3";
                queue.push(color);

                color = [245, 111, 6, 0.7]; //"#f56f06";
                queue.push(color);

                color = [255, 204, 0, 0.7]; //"#ffcc00";
                queue.push(color);

                color = [68, 0, 227, 0.7]; //"#4400e3";
                queue.push(color);
            }

            function CreateColorQueue1(queue1) {
                var colorhex = "#df191e";
                queue1.push(colorhex);

                colorhex = "#85bc00";
                queue1.push(colorhex);

                colorhex = "#00bce3";
                queue1.push(colorhex);

                colorhex = "#a200e3";
                queue1.push(colorhex);

                colorhex = "#f56f06";
                queue1.push(colorhex);

                colorhex = "#ffcc00";
                queue1.push(colorhex);

                colorhex = "#4400e3";
                queue1.push(colorhex);
            }

            var init = function () {

                require([
                    "esri/map", "esri/geometry/Extent", "dojo/_base/array", "esri/layers/FeatureLayer", "esri/layers/OpenStreetMapLayer", "esri/InfoTemplate", "esri/symbols/SimpleFillSymbol",
                    "esri/renderers/SimpleRenderer", "esri/symbols/SimpleMarkerSymbol", "esri/renderers/ScaleDependentRenderer",
                    "esri/symbols/PictureMarkerSymbol",
                    "esri/geometry/Point", "esri/graphic", "dojo/_base/Color", "esri/geometry/webMercatorUtils", "extras/ClusterLayer", "esri/dijit/Legend", "esri/renderers/ClassBreaksRenderer", "dojo/dom", "dojo/domReady!"
                ], function (Map, Extent, arrayUtils, FeatureLayer, OpenStreetMapLayer, InfoTemplate, SimpleFillSymbol, SimpleRenderer, SimpleMarkerSymbol, ScaleDependentRenderer, PictureMarkerSymbol, Point, Graphic, Color, webMercatorUtils, ClusterLayer, Legend, ClassBreaksRenderer, dom) {

                    if ($scope.map) {
                        $scope.map.destroy();
                    }
                    $scope.map = new Map(elm[0], {
                        zoom: 4,
                        basemap: 'streets'
                    });

                    var clusterLayer;
                    var dataInfo = {};
                    var queue = [];
                    var queue1 = [];
                    CreateColorQueue(queue);
                    CreateColorQueue1(queue1);

                    // var legendLayers = [];
                    var ptData = [];
                    elm[0].children[0].innerHTML = "";
                    for (var j = 0; j < $scope.gadget.mapdirective.length; j++) {

                        var graphicsLayer = new OpenStreetMapLayer();


                        var color = queue.shift();
                        var colorhex = queue1.shift();

                        if (color == undefined) {
                            CreateColorQueue(queue);
                            color = queue.shift();


                        }

                        if (colorhex == undefined) {
                            CreateColorQueue1(queue1);
                            colorhex = queue1.shift();
                        }

                        var markerSym = new SimpleMarkerSymbol();
                        markerSym.setColor(new Color(color));


                        for (var i = 0; i < $scope.gadget.mapdirective[j].Collection.length; i++) {

                            var pt = $scope.gadget.mapdirective[j].Collection[i];
                            graphicsLayer.id = pt.StrataValue;

                            var point = Point(pt.LonX, pt.LatY);

                            var geometry = webMercatorUtils
                                .geographicToWebMercator(point);

                            var graphic = new Graphic(point, markerSym);
                            graphic.geometry = geometry;
                            graphicsLayer.add(graphic);


                        }

                        ptData = arrayUtils.map($scope.gadget.mapdirective[j].Collection, function (p) {
                            var latlng = new Point(parseFloat(p.LonX), parseFloat(p.LatY));
                            var webMercator = webMercatorUtils.geographicToWebMercator(latlng);

                            return {
                                "x": webMercator.x,
                                "y": webMercator.y//,
                                //"attributes": attributes
                            };
                        });


                        // }


                        var ext = new esri.graphicsExtent(graphicsLayer.graphics);
                        $scope.map.setExtent(ext);
                        elm[0].children[0].innerHTML += "<div style='display:block; margin: 5px 0 5px 0;' ><div style='width: 16px; height:16px; vertical-align: middle; display:inline-block; background-color:" + colorhex + "'></div> " + graphicsLayer.id + " </div>";


                        // cluster layer that uses OpenLayers style clustering
                        var radiusSize = $scope.Radius * 1.5;
                        clusterLayer = new ClusterLayer({
                            "data": ptData,
                            "distance": radiusSize,
                            "id": "clusters" + graphicsLayer.id,
                            "labelColor": "#fff",
                            //"labelOffset": 10,
                            "resolution": $scope.map.extent.getWidth() / $scope.map.width,
                            "singleColor": "#888"//,
                            //"singleTemplate": null
                        });


                        var defaultSym = new SimpleMarkerSymbol(); //.setSize(4);
                        defaultSym.setColor(new Color(color));
                        //defaultSym.setOffset(0, 2);
                        defaultSym.setSize(5);

                        var defaultSym1 = new SimpleMarkerSymbol(); //.setSize(4);
                        defaultSym1.setColor(new Color(color));
                        //defaultSym1.setOffset(0, 5);
                        defaultSym1.setSize(15);

                        var defaultSym2 = new SimpleMarkerSymbol(); //.setSize(4);
                        defaultSym2.setColor(new Color(color));
                        defaultSym2.setOffset(0, 0);
                        defaultSym2.setSize(25);

                        var defaultSym3 = new SimpleMarkerSymbol(); //.setSize(4);
                        defaultSym3.setColor(new Color(color));
                        defaultSym3.setOffset(0, 0);
                        defaultSym3.setSize(50);

                        var defaultSym4 = new SimpleMarkerSymbol(); //.setSize(4);
                        defaultSym4.setColor(new Color(color));
                        defaultSym4.setOffset(0, 0);
                        defaultSym4.setSize(100);
                        var renderer = new ClassBreaksRenderer(defaultSym, "clusterCount");

                        var clusterSize = $scope.ClusterSize;

                        renderer.addBreak(0, clusterSize, defaultSym1); //blue);
                        renderer.addBreak(clusterSize, 50, defaultSym2); //green);
                        renderer.addBreak(50, 1001, defaultSym3); //red);
                        renderer.addBreak(1001, 100000000000000, defaultSym4);

                        clusterLayer.setRenderer(renderer);

                        $scope.map.addLayer(clusterLayer);
                    }

                });

            };

            $scope.$watch('gadget.mapdirective', function () {
                var map = $scope.gadget.mapdirective;
                dojo.addOnLoad(init);


            }, true);
        }
    }
});

