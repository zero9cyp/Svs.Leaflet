@code
    ViewBag.Title = "Εικόνα επιτήρησης Κύπρου"

    Dim dataSources As String = ViewBag.dataSources
end code

<div class="row" id="data">
    <div class="container" id="izenda-root"></div>
</div>

<div id="map"></div>



<div class="container-fluid">

    <div class="row">
        <div id="map" class="col-sm-12"></div>
    </div>
</div>





<script type="text/javascript">

    $(document).ready(function () {
        var map = new L.map('map', {
            fullscreenControl: true,
            fullscreenControlOptions: {
                position: 'topleft',
                title: 'Fullscreen Mode'
            }
        });

        // create the tile layer
        var osmUrl = 'http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png';
        var osmAttrib = 'Map data © <a href="http://openstreetmap.org">OpenStreetMap</a> contributors';
        var osmLayer = new L.TileLayer(osmUrl, { minZoom: 5, maxZoom: 15, attribution: osmAttrib });

        var tiles = new L.tileLayer('/api/map/osm/{z}/{x}/{y}', { minZoom: 1, maxZoom: 13, opacity: 1 });
        var Alttiles = new L.tileLayer('/api/map/enc/{z}/{x}/{y}', { minZoom: 1, maxZoom: 12, opacity: 1 });

        // set the map start-up position
        map.setView(new L.LatLng(35.03, 33.17), 9);

        // set-up background layer
        map.addLayer(tiles);

        $(window).resize(function () {
            // remaining height - 22 (wrapper bottom padding + map border + 1) !
            $('#map').height($(this).innerHeight() - $('#navbar').outerHeight(true) - $('#footer').outerHeight(true));
        });
        $(window).resize();

        window.setInterval(function () { updateTracksLayer(map, tracksLayer, 'acss-tracks'); }, 10000);

        window.setInterval(function () { DrawACSSLabel(); }, 5000);

    });


</script>
