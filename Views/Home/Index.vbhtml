@code
    ViewBag.Title = "Εικόνα επιτήρησης Κύπρου"

    Dim dataSources As String = ViewBag.dataSources
end code

<div Class="row">
    <div id = "map" Class="col-sm-12"></div>
</div>

@section scripts{
    <script type="text/javascript">
        var searchCtrl = null;
        var arrayIdentities = [];
        var excludedCats = [];
        var objFilter = {
            identities: ['PEN', 'UNK', 'ASS', 'FRI', 'NEU', 'SUS', 'HOS'],
            excludeCategories: ["Small"]
        };

        function DrawACSSLabel() {
            $.getJSON("/api/tracks/lastupdate/acss-tracks", function (data) {
                var lastUpdate = parseInt(data);
                if (lastUpdate < 5) {
                    $('#acssStatus').css('color', 'green').html('ACSS Status (<strong>OK</strong>)');
                }
                else if (lastUpdate < 10 && lastUpdate > 5) {
                    $('#acssStatus').css('color', 'orange').html('ACSS Status (<strong>Pending</strong>)');
                }
                else if (lastUpdate > 10) {
                    $('#acssStatus').css('color', 'red').html('ACSS Status (<strong>Critical</strong>)');
                }
            });
        }

        function updateTracksLayer(map, layer, label) {

            if (objFilter.excludeCategories.length === 0) {
                objFilter.excludeCategories.push('');
            }

            $.ajax({
                type: "POST",
                url: "/api/tracks/currentsituation/" + label,
                data: objFilter,
                dataType: 'json'
            }).done(function (data, status, jqXHR) {
                layer.clearLayers();
                layer.addData(data);

                if (label === 'acss-tracks') {
                    searchCtrl.indexFeatures(data, ['trackno', 'vesselname', 'imonumber', 'shiptypeclass', 'type', 'mmsi', 'vesseldescription']);
                    var searchValue = '';
                    if (searchCtrl._input.value !== null && searchCtrl._input.value !== '') {
                        searchValue = searchCtrl._input.value;
                    }
                    searchCtrl.searchFeatures(searchValue);
                }

            }).fail(function (jqXHR, status, error) {
                console.log(status + ' - ' + error);
            });

        }

        function drawCheckbox(cssStyle, label, value) {

            var id = '';
            var checkValue = '';
            var index = -1;
            if (cssStyle === 'check-identification') {
                id = 'ide-' + value;

                index = objFilter.identities.indexOf(value);
                if (index >= 0) {
                    checkValue = ' checked="checked"';
                    arrayIdentities.push(value);
                }
            } else {
                id = 'cat-' + value;

                checkValue = ' checked="checked"';
                index = objFilter.excludeCategories.indexOf(value);
                if (index >= 0) {
                    checkValue = '';
                    excludedCats.push(value);
                }
            }

            var content = ''
                + '<div class="form-check">'
                + '<input id="' + id + '" class="form-check-input ' + cssStyle + '" type="checkbox" value="' + value + '"' + checkValue + '>'
                + '<label class="form-check-label" for="' + id + '">' + label + '</label>'
                + '</div>';

            return content;
        }

        function createTooltip(f) {
            var tooltip = '<strong>(<b>' + f.properties.amplification + '</b>) ' + f.properties.trackno + '</strong><br/>';
            if (f.properties.vesselname) {
                tooltip = '<strong>(<b>' + f.properties.amplification + '</b>) ' + f.properties.trackno + ' / ' + f.properties.vesselname + '</strong><br/>';
            }
            if (f.properties.mmsi) {
                tooltip += '<div class="flag" id="' + f.properties.isocode3 + '"title="' + f.properties.country + '"></div> <i>' + f.properties.country + '</i></strong><br/>';
            }

            if (f.properties.type) {
                tooltip += '<i>' + f.properties.type + '</i><br/>';
            }
            else {
                tooltip += '<i>' + f.properties.category + '</i><br/>';
            }
            tooltip += parseFloat(f.properties.speed).toFixed(2) + ' knots / ' + parseFloat(f.properties.course).toFixed(1) + '&deg;<br/>';
            tooltip += 'updated: ' + moment.utc(f.properties.lastupdate).fromNow() + '<br/>';

            return tooltip;
        }

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

            // ACSS tracks layer
            var tracksLayer = new L.geoJson(null, {
                pointToLayer: function (f, latlng) {
                    var iconSize = [26, 26];
                    var iconAnchor = [13, 13];
                    var iconUrl = '/content/markers/' + f.properties.amplification + '-' + f.properties.identity + '.svg';

                    var vesselIcon = L.icon({
                        iconUrl: iconUrl,
                        iconSize: iconSize,
                        iconAnchor: iconAnchor
                    });

                    var vesselSymbol = new ms.Symbol(
                        f.properties.sidc, {
                            uniqueDesignation: f.properties.vesselname
                        });

                    // Now that we have a symbol we can ask for the echelon and set the symbol size
                    vesselSymbol = vesselSymbol.setOptions({
                        size: 22,
                        direction: f.properties.course
                    });

                    vesselIcon = L.icon({
                        iconUrl: vesselSymbol.toDataURL(),
                        iconAnchor: [vesselSymbol.getAnchor().x, vesselSymbol.getAnchor().y],
                    });

                    var tooltip = createTooltip(f);

                    return new L.marker(latlng, {
                        icon: vesselIcon, rotationAngle: 0, rotationOrigin: 'center center'
                    }).bindTooltip(tooltip, { permanent: false });
                },
                onEachFeature: function (f, l) {
                    f.layer = l;

                    l.bindPopup(function () {
                        return createTooltip(f);
                    });
                }
            });

            var gridLayer = L.latlngGraticule({
                showLabel: true,
                dashArray: [5, 5],
                font: '14px',
                fontColor: '#000',
                weight: '0.9'
                ////zoomInterval: [
                ////    { start: 2, end: 3, interval: 30 },
                ////    { start: 4, end: 4, interval: 10 },
                ////    { start: 5, end: 7, interval: 5 },
                ////    { start: 8, end: 10, interval: 1 }
                ////]
            });

            // set the map start-up position
            map.setView(new L.LatLng(35.03, 33.17), 9);

            // set-up background layer (πιθανώς να βάλουμε και τα παλιά να υπάρχουν)
            map.addLayer(tiles);

            // set-up tracks & grid layers
            //map.addLayer(aisLayer);
            map.addLayer(tracksLayer);
            //map.addLayer(gridLayer);


            // Layers Tree
            var baseTree = {
                label: 'Βασικοί Χάρτες',
                children: [
                    { label: 'Υπόβαθρο', layer: tiles },
                    { label: 'Ναυτιλιακός Χάρτης', layer: Alttiles }
                ]
            };
            var tracksTree = {
                label: 'Επίπεδα Δεδομένων',
                children: [
                    {
                        label: 'Ροές Δεδομένων',
                        children: [
                            //{ label: 'AIS Tracks', layer: aisLayer },
                            { label: 'ACCS Tracks', layer: tracksLayer },
                            { label: 'Grid', layer: gridLayer }
                        ]
                    }
                ]
            };
            var layersTree = { label: 'Εκτεταμένα Γεωγραφικά Επίπεδα', children: [] };
            var filesLayer = [];
            var urlSource = $.getJSON("/layers/index", function (dataList, status) {
                if (dataList !== undefined && dataList.length > 0) {
                    for (var i = 0; i < dataList.length; i++) {
                        var geoJsonLayer = '';
                        if (dataList[i].style != null) {
                            geoJsonLayer = L.geoJson(null, { style: JSON.parse(dataList[i].style) });
                        }
                        else {
                            if (dataList[i].filename == 'CyprusExclusiveZone') {

                                geoJsonLayer = L.geoJson(null, {
                                    style: function (feature) {
                                        return {
                                            color: 'red',
                                            weight: 1,
                                            opacity: 0.5,
                                            clickable: false
                                        };
                                    },
                                    onEachFeature: function (feature, layer) {
                                        layer.bindTooltip('' + feature.properties.Name, { permanent: true, opacity: 0.8, direction: 'center', className: 'layer-label' });
                                    }
                                });
                            }
                            else {
                                geoJsonLayer = L.geoJson(null, { style: JSON.parse(dataList[i].style) });
                            }
                        }
                        filesLayer.push(geoJsonLayer);

                        var geoJsonFile = { label: dataList[i].description, layer: filesLayer[i] };
                        layersTree.children.push(geoJsonFile);

                        $.ajax({
                            url: '/layers/fetch/' + dataList[i].filename,
                            method: 'GET',
                            datatype: "json",
                            context: filesLayer[i]
                        }).done(function (data, status, jqXHR) {
                            this.clearLayers();
                            this.addData(data);
                        }).fail(function (jqXHR, status, error) {

                        });
                    }

                }

                tracksTree.children.push(layersTree);
                L.control.layers.tree(baseTree, tracksTree).addTo(map);

            });

            // Coordinates Control
            L.control.coordinates({
                position: "bottomright", //optional default "bootomright"
                decimals: 2, //optional default 4
                decimalSeperator: ".", //optional default "."
                labelTemplateLat: "{y} N", //optional default "Lat: {y}"
                labelTemplateLng: "- {x} E", //optional default "Lng: {x}"
                enableUserInput: true, //optional default true
                useDMS: true, //optional default false
                useLatLngOrder: true, //ordering of labels, default false-> lng-lat
                markerType: L.marker, //optional default L.marker
                markerProps: {}, //optional default {},
            }).addTo(map);

            // Measure control.
            var options = {
                position: 'topleft',            // Position to show the control. Values: 'topright', 'topleft', 'bottomright', 'bottomleft'
                unit: 'nauticalmiles',                 // Show imperial or metric distances. Values: 'metres', 'landmiles', 'nauticalmiles'
                clearMeasurementsOnStop: true,  // Clear all the measurements when the control is unselected
                showBearings: false,            // Whether bearings are displayed within the tooltips
                bearingTextIn: 'In',            // language dependend label for inbound bearings
                bearingTextOut: 'Out',          // language dependend label for outbound bearings
                tooltipTextDraganddelete: 'Κάνε click και σύρετε το <b>σημείο</b><br>Πατήστε ALT-key και κάνετε click στο σημείο για να το <b>διαγράψετε</b>.',
                tooltipTextResume: '<br>Press CTRL-key and click to <b>resume line</b>',
                tooltipTextAdd: 'Press CTRL-key and click to <b>add point</b>', // language dependend labels for point's tooltips
                measureControlTitleOn: 'Εκκίνηση "Μέτρησης Αποστάσεων"',   // Title for the control going to be switched on
                measureControlTitleOff: 'Τερματισμός "Μέτρησης Αποστάσεων"', // Title for the control going to be switched off
                measureControlLabel: '&#8614;', // Label of the Measure control (maybe a unicode symbol)
                measureControlClasses: [],      // Classes to apply to the Measure control
                showClearControl: false,        // Show a control to clear all the measurements
                clearControlTitle: 'Clear Measurements', // Title text to show on the clear measurements control button
                clearControlLabel: '&times',    // Label of the Clear control (maybe a unicode symbol)
                clearControlClasses: [],        // Classes to apply to clear control button
                showUnitControl: false,         // Show a control to change the units of measurements
                unitControlTitle: {             // Title texts to show on the Unit Control button
                    text: 'Change Units',
                    metres: 'metres',
                    landmiles: 'land miles',
                    nauticalmiles: 'nautical miles'
                },
                unitControlLabel: {             // Label symbols to show in the Unit Control button
                    metres: 'm',
                    kilometres: 'km',
                    feet: 'ft',
                    landmiles: 'mi',
                    nauticalmiles: 'nm'
                },
                tempLine: {                     // Styling settings for the temporary dashed line
                    color: '#00f',              // Dashed line color
                    weight: 2                   // Dashed line weight
                },
                fixedLine: {                    // Styling for the solid line
                    color: '#006',              // Solid line color
                    weight: 2                   // Solid line weight
                },
                startCircle: {                  // Style settings for circle marker indicating the starting point of the polyline
                    color: '#000',              // Color of the border of the circle
                    weight: 1,                  // Weight of the circle
                    fillColor: '#0f0',          // Fill color of the circle
                    fillOpacity: 1,             // Fill opacity of the circle
                    radius: 3                   // Radius of the circle
                },
                intermedCircle: {               // Style settings for all circle markers between startCircle and endCircle
                    color: '#000',              // Color of the border of the circle
                    weight: 1,                  // Weight of the circle
                    fillColor: '#ff0',          // Fill color of the circle
                    fillOpacity: 1,             // Fill opacity of the circle
                    radius: 3                   // Radius of the circle
                },
                currentCircle: {                // Style settings for circle marker indicating the latest point of the polyline during drawing a line
                    color: '#000',              // Color of the border of the circle
                    weight: 1,                  // Weight of the circle
                    fillColor: '#f0f',          // Fill color of the circle
                    fillOpacity: 1,             // Fill opacity of the circle
                    radius: 3                   // Radius of the circle
                },
                endCircle: {                    // Style settings for circle marker indicating the last point of the polyline
                    color: '#000',              // Color of the border of the circle
                    weight: 1,                  // Weight of the circle
                    fillColor: '#f00',          // Fill color of the circle
                    fillOpacity: 1,             // Fill opacity of the circle
                    radius: 3                   // Radius of the circle
                },
            };
            L.control.polylineMeasure(options).addTo(map);

            // Print control
            //L.easyPrint({
            //    title: 'Αποθήκευση sreenshot',
            //    position: 'topleft',
            //    exportOnly: 'true',
            //    sizeModes: ['A4Portrait', 'A4Landscape']
            //}).addTo(map);

            // Search control
            searchCtrl = L.control.fuseSearch({
                title: 'Αναζήτηση',
                panelTitle: 'Αναζήτηση',
                placeholder: 'Αναζήτηση',
                showResultFct: function (f, container) {
                    var tooltip = createTooltip(f);
                    container.innerHTML = tooltip;
                }
            });
            searchCtrl.addTo(map);

            // Filter control
            var slideMenu = L.control.slideMenu('', {
                position: 'topright',
                menuposition: 'topright',
                width: '320px',
                //height: '470px',
                delay: '15',
                direction: 'horizontal',
                icon: 'fa-chevron-left'
            }).addTo(map);

            var contents = '<div class="panel-content"><h4>ACSS Tracks Filter</h4>';
            contents += '<hr>'
                + '<div id ="identificationPanel" class="form"><h5>Track Identification</h5>'
                + drawCheckbox('check-identification', 'Pending', 'PEN')
                + drawCheckbox('check-identification', 'Unknown', 'UNK')
                + drawCheckbox('check-identification', 'Assumed Friend', 'ASS')
                + drawCheckbox('check-identification', 'Friend', 'FRI')
                + drawCheckbox('check-identification', 'Neutral', 'NEU')
                + drawCheckbox('check-identification', 'Suspect', 'SUS')
                + drawCheckbox('check-identification', 'Hostile', 'HOS')
                + '</div>';
            contents += '<hr>';
            contents += '<div id ="categoriesPanel"><h5>Vessel Categories</h5>'
                           If (dataSources!= null && dataSources.Length > 0) Then
                                                 {
                    foreach(String category In dataSources)
                    {
:+ drawCheckbox('check-category', 'category', 'category')
                    }
                }
                + '</div>';
            contents += '<hr>';
            contents += '<div class="form-group text-right">';
            contents += '<button id="btnApply" class="btn btn-primary">Εφαρμογή</button>';
            contents += '</div></div>';

            slideMenu.setContents(contents);

            $(".check-identification").change(function () {
                var k = $(this).val();
                If (this.checked) Then {
                    arrayIdentities.push(k);
                }
                Else {
                    var t2 = arrayIdentities.indexOf(k);
                    arrayIdentities.splice(t2, 1);
                }
            });

            $(".check-category").change(function () {
                var k = $(this).val();
                If (this.checked) Then {
                    var t2 = excludedCats.indexOf(k);
                    excludedCats.splice(t2, 1);
                }
                Else {
                    excludedCats.push(k);
                }
            });

            // Apply button click
            $("#btnApply").click(function () {
                objFilter.identities = arrayIdentities;
                objFilter.excludeCategories = excludedCats;
                updateTracksLayer(map, tracksLayer, 'acss-tracks');
            });

            $(window).resize(function () {
                // remaining height - 22 (wrapper bottom padding + map border + 1) !
                $('#map').height($(this).innerHeight() - $('#navbar').outerHeight(true) - $('#footer').outerHeight(true));
            });
            $(window).resize();

            window.setInterval(Function() {updateTracksLayer(map, tracksLayer, 'acss-tracks'); }, 10000);

                    window.setInterval(Function() {DrawACSSLabel(); }, 5000);

        });

    </script>
    end  section

}