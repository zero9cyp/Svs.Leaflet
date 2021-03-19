<head>
    <script src="http://www.openlayers.org/dev/OpenLayers.js"></script>
    <script type="text/javascript">

        var map;
        OpenLayers.IMAGE_RELOAD_ATTEMPTS = 3;
        OpenLayers.ImgPath = "http://js.mapbox.com/theme/dark/";
        function init() {

            // Customize the values below to change the tileset.
            // This information is available on each tileset page.
            var layername = 'world-light';
            var file_extension = 'png';

            // Build the map
            var options = {
                projection: new OpenLayers.Projection("EPSG:900913"),
                displayProjection: new OpenLayers.Projection("EPSG:4326"),
                units: "m",
                numZoomLevels: 12,
                maxResolution: 156543.0339,
                maxExtent: new OpenLayers.Bounds(
                    -20037500,
                    -20037500,
                    20037500,
                    20037500
                )
            };
            map = new OpenLayers.Map('map', options);

            // Layer definitions
            var layer = new OpenLayers.Layer.TMS(
                "MapBox Layer",
                ["http://a.tile.mapbox.com/", "http://b.tile.mapbox.com/",
                    "http://c.tile.mapbox.com/", "http://d.tile.mapbox.com/"],
                { 'layername': layername, 'type': file_extension }
            );

            // Add layers to the map
            map.addLayers([layer]);

            // Set the map's initial center point
            map.setCenter(new OpenLayers.LonLat(0, 0), 1);
        }

    </script>
</head>
<body onload="init()">
    <div id="map" style="width: 500px; height: 300px"></div>
</body>