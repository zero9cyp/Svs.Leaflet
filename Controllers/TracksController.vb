Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net
Imports System.Net.Http
Imports System.Threading.Tasks
Imports System.Web.Http
Imports System.Web.Http.Description
Imports GeoJSON.Net.Feature
Imports GeoJSON.Net.Geometry
Imports Newtonsoft.Json
Imports Nss.Models
Imports System.Runtime.CompilerServices

Namespace Controllers
    Public Class TracksController
        Inherits ApiController

        Private Shared ReadOnly _log As log4net.ILog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

        '  <HttpGet>
        '  <Route("api/tracks/currentsituation/{id}")>
        '  <ResponseType(GetType(List(Of Track)))>
        Public Async Function GetRealTimeTracks(ByVal id As String) As Task(Of IHttpActionResult)
            Dim otherSources As String() = {"ACSS-AIS", "ACSS-TRACKS", "ACSS-TRACKS-BIG", "NSS"}

            If String.IsNullOrEmpty(id) Then
                id = "ACSS-AIS"
            Else
                id = id.ToUpper()
            End If

            Dim features As List(Of Feature) = New List(Of Feature)()

            Using db As NssDbEntities = New NssDbEntities()
                Dim listOfTracks As List(Of vwCurrentSituation) = Nothing
                Dim queryOfTracks = db.vwCurrentSituations.AsNoTracking()

                Select Case id
                    Case "ACSS-AIS"
                        listOfTracks = queryOfTracks.Where(Function(m) m.DataSource.Equals("ACSS-AIS")).ToList()
                    Case "NSS"
                        listOfTracks = queryOfTracks.Where(Function(m) m.DataSource.Equals("NSS")).ToList()
                    Case "ACSS-TRACKS"
                        listOfTracks = queryOfTracks.Where(Function(m) m.DataSource.Equals("ACSS-TRACKS")).ToList()
                    Case "ACSS-TRACKS-BIG"
                        listOfTracks = queryOfTracks.Where(Function(m) m.DataSource.Equals("ACSS-TRACKS") AndAlso m.Category <> "Small").ToList()
                    Case Else
                        listOfTracks = queryOfTracks.Where(Function(m) Not otherSources.Contains(m.DataSource)).ToList()
                End Select

                For Each track As vwCurrentSituation In listOfTracks
                    Dim geometry As Point = New Point(New Position(track.Latitude, track.Longitude))
                    Dim properties = New Dictionary(Of String, Object)()
                    properties.Add("trackno", track.TrackNo)
                    properties.Add("vesselname", track.VesselName)
                    properties.Add("imonumber", track.IMONumber)
                    properties.Add("callsign", track.CallSign)
                    properties.Add("countryaiscode", track.CountryAISCode)
                    properties.Add("country", track.Country)
                    properties.Add("countryisocode", track.CountryISOCode)
                    properties.Add("isocode3", track.ISOCode3)
                    properties.Add("shiptype", track.ShipType)
                    properties.Add("shiptypeclass", track.ShipTypeClass)
                    properties.Add("type", track.ShipTypeName)
                    properties.Add("navigationstatus", track.ToNavigationStatus())
                    properties.Add("course", track.COG)
                    properties.Add("speed", track.SOG)
                    properties.Add("source", track.DataSource)
                    properties.Add("dateinitiated", track.DateInitiated.ToLocalTime())
                    properties.Add("lastupdate", track.LastUpdate.ToLocalTime())
                    properties.Add("iconurl", track.ToIcon())
                    properties.Add("sidc", track.ToSIDC())
                    properties.Add("mmsi", track.MMSI)
                    properties.Add("noofdetectors", track.NoOfDetectors)
                    properties.Add("idsofdetectors", track.IDsOfDetectors)
                    properties.Add("amplification", track.Amplification)
                    properties.Add("category", track.Category)
                    properties.Add("identity", track.Identity)
                    properties.Add("vesseldescription", track.VesselDescription)
                    features.Add(New Feature(geometry, properties))
                Next

                If Not otherSources.Contains(id) Then
                    Dim sensors As List(Of Sensor) = db.Sensors.AsNoTracking().ToList()

                    For Each s As Sensor In sensors
                        Dim geometry As Point = New Point(New Position(s.Latitude, s.Longitude))
                        Dim properties = New Dictionary(Of String, Object)()
                        properties.Add("trackno", s.SensorName)
                        properties.Add("vesselname", s.SensorName)
                        properties.Add("source", "Radar")
                        properties.Add("shiptype", 0)
                        properties.Add("shiptypeclass", s.SensorName)
                        properties.Add("iconurl", "/content/markers/square0.svg")
                        features.Add(New Feature(geometry, properties))
                    Next
                End If
            End Using

            Dim model As FeatureCollection = New FeatureCollection(features)
            Return Ok(model)
        End Function

        <HttpPost>
        <Route("api/tracks/currentsituation/{id}")>
        <ResponseType(GetType(List(Of Track)))>
        Public Async Function GetRealTimeTracks(
        <FromUri> ByVal id As String,
        <FromBody> ByVal filter As filterObject) As Task(Of IHttpActionResult)
            Dim dataSource As String = id.ToUpper()
            Dim features As List(Of Feature) = New List(Of Feature)()

            Using db As NssDbEntities = New NssDbEntities()

                Try
                    Dim listOfTracks As List(Of vwCurrentSituation) = Nothing
                    Dim queryOfTracks As IQueryable(Of vwCurrentSituation) = db.vwCurrentSituations.AsNoTracking()

                    Select Case dataSource
                        Case "ACSS-TRACKS"
                            listOfTracks = queryOfTracks.Where(Function(m) m.DataSource.Equals(dataSource) AndAlso filter.identities.Contains(m.Amplification) AndAlso Not filter.excludeCategories.Contains(m.Category)).ToList()
                        Case Else
                            listOfTracks = queryOfTracks.Where(Function(m) m.DataSource.Equals(dataSource)).ToList()
                    End Select

                    For Each track As vwCurrentSituation In listOfTracks
                        Dim geometry As Point = New Point(New Position(track.Latitude, track.Longitude))
                        Dim properties = New Dictionary(Of String, Object)()
                        properties.Add("trackno", track.TrackNo)
                        properties.Add("vesselname", track.VesselName)
                        properties.Add("imonumber", track.IMONumber)
                        properties.Add("callsign", track.CallSign)
                        properties.Add("countryaiscode", track.CountryAISCode)
                        properties.Add("country", track.Country)
                        properties.Add("countryisocode", track.CountryISOCode)
                        properties.Add("isocode3", track.ISOCode3)
                        properties.Add("shiptype", track.ShipType)
                        properties.Add("shiptypeclass", track.ShipTypeClass)
                        properties.Add("type", track.ShipTypeName)
                        properties.Add("navigationstatus", track.ToNavigationStatus())
                        properties.Add("course", track.COG)
                        properties.Add("speed", track.SOG)
                        properties.Add("source", track.DataSource)
                        properties.Add("dateinitiated", track.DateInitiated.ToLocalTime())
                        properties.Add("lastupdate", track.LastUpdate.ToLocalTime())
                        properties.Add("iconurl", track.ToIcon())
                        properties.Add("sidc", track.ToSIDC())
                        properties.Add("mmsi", track.MMSI)
                        properties.Add("noofdetectors", track.NoOfDetectors)
                        properties.Add("idsofdetectors", track.IDsOfDetectors)
                        properties.Add("amplification", track.Amplification)
                        properties.Add("category", track.Category)
                        properties.Add("identity", track.Identity)
                        properties.Add("vesseldescription", track.VesselDescription)
                        features.Add(New Feature(geometry, properties))
                    Next

                Catch ex As Exception
                    _log.Fatal("GetRealTimeTracks()", ex)
                End Try
            End Using

            Dim model As FeatureCollection = New FeatureCollection(features)
            Return Ok(model)
        End Function

        <HttpGet>
        <Route("api/tracks/filtered/{id}")>
        <ResponseType(GetType(List(Of Track)))>
        Public Async Function GetFilteredTracks(ByVal id As String) As Task(Of IHttpActionResult)
            Dim features As List(Of Feature) = New List(Of Feature)()

            Using db As NssDbEntities = New NssDbEntities()
                Dim listOfTracks As List(Of vwCurrentSituation) = Nothing
                Dim queryOfTracks = db.vwCurrentSituations.AsNoTracking()

                Select Case id
                    Case "All"
                        listOfTracks = queryOfTracks.Where(Function(m) m.DataSource.Equals("ACSS-TRACKS")).ToList()
                    Case "Hostile"
                        listOfTracks = queryOfTracks.Where(Function(m) m.DataSource.Equals("ACSS-TRACKS") AndAlso m.Amplification = "HOS").ToList()
                    Case "Friend"
                        listOfTracks = queryOfTracks.Where(Function(m) m.DataSource.Equals("ACSS-TRACKS") AndAlso m.Amplification = "FRD").ToList()
                    Case "Pending"
                        listOfTracks = queryOfTracks.Where(Function(m) m.DataSource.Equals("ACSS-TRACKS") AndAlso m.Amplification = "PND").ToList()
                    Case "Unknown"
                        listOfTracks = queryOfTracks.Where(Function(m) m.DataSource.Equals("ACSS-TRACKS") AndAlso m.Amplification = "UNK").ToList()
                    Case "Assumed Friend"
                        listOfTracks = queryOfTracks.Where(Function(m) m.DataSource.Equals("ACSS-TRACKS") AndAlso m.Amplification = "AFD").ToList()
                    Case "Neutral"
                        listOfTracks = queryOfTracks.Where(Function(m) m.DataSource.Equals("ACSS-TRACKS") AndAlso m.Amplification = "NEU").ToList()
                    Case "Suspect"
                        listOfTracks = queryOfTracks.Where(Function(m) m.DataSource.Equals("ACSS-TRACKS") AndAlso m.Amplification = "SUS").ToList()
                    Case Else
                End Select
            End Using

            Return Nothing
        End Function

        <HttpGet>
        <Route("api/tracks/backtracks/{id}")>
        <ResponseType(GetType(List(Of Point)))>
        Public Async Function GetBackTracks(ByVal id As String) As Task(Of IHttpActionResult)
            Dim features As List(Of Point) = New List(Of Point)()

            Using db As NssDbEntities = New NssDbEntities()
                Dim FirstTrack As vwCurrentSituation = Nothing
                Dim queryOfTracks = db.vwCurrentSituations.AsNoTracking()
                FirstTrack = queryOfTracks.Where(Function(m) m.DataSource.Equals("ACSS-AIS")).FirstOrDefault()
                Dim q1 = db.Tracks.AsNoTracking()
                Dim k2 = q1.Where(Function(t) t.TrackNo = FirstTrack.TrackNo And t.DateRecieved < FirstTrack.LastUpdate.AddHours(-1)).[Select](Function(t) New Track With {
                    .Longitude = t.Longitude,
                    .Latitude = t.Latitude
                }).ToList()

                For Each _track As Track In k2
                    Dim geometry As Point = New Point(New Position(_track.Latitude, _track.Longitude))
                    features.Add(geometry)
                Next
            End Using

            Return Ok(features)
        End Function

        <HttpGet>
        <Route("api/tracks/lastupdate/{id}")>
        <ResponseType(GetType(Integer))>
        Public Async Function GetLastUpdate(ByVal id As String) As Task(Of IHttpActionResult)
            Dim minutesToLastUpdate As Integer = 999
            Dim featuresLastUpate As DateTime = DateTime.MinValue
            Dim currentDatetime As DateTime = DateTime.UtcNow

            If String.IsNullOrEmpty(id) Then
                id = "ACSS-TRACKS"
            Else
                id = id.ToUpper()
            End If

            Using db As NssDbEntities = New NssDbEntities()

                Try
                    Dim queryOfTracks = db.vwCurrentSituations.AsNoTracking()

                    Select Case id
                        Case "ACSS-AIS"
                            featuresLastUpate = queryOfTracks.Where(Function(m) m.DataSource.Equals("ACSS-AIS")).OrderByDescending(Function(m) m.LastUpdate).[Select](Function(m) m.LastUpdate).FirstOrDefault()
                            minutesToLastUpdate = CInt(((currentDatetime - featuresLastUpate).TotalMinutes))
                        Case "ACSS-TRACKS"
                            featuresLastUpate = queryOfTracks.Where(Function(m) m.DataSource.Equals("ACSS-TRACKS")).OrderByDescending(Function(m) m.LastUpdate).[Select](Function(m) m.LastUpdate).FirstOrDefault()
                            minutesToLastUpdate = CInt(((currentDatetime - featuresLastUpate).TotalMinutes))
                    End Select

                Catch ex As Exception
                    _log.Fatal("GetLastUpdate()", ex)
                End Try
            End Using

            Return Ok(minutesToLastUpdate)
        End Function
    End Class

    Partial Module Extensions
        'have to change this from share to public
        Public NavigationStatuses As String() = {"Under way using engine", "At anchor", "Not under command", "Restricted manoeuverability", "Constrained by her draught", "Moored", "Aground", "Engaged in Fishing", "Under way sailing", "Reserved for future amendment of Navigational Status for HSC", "Reserved for future amendment of Navigational Status for WIG", "Reserved for future use", "Reserved for future use", "Reserved for future use", "AIS-SART is active"}

        <Extension()>
        Function ToIcon(ByVal model As vwCurrentSituation) As String
            If Not model.ShipType.HasValue Then
                model.ShipType = 0
            End If

            Dim iconUrl As String = "/content/markers/"
            Dim iconMask As String = "radar{0}.svg"

            If model.DataSource.Equals("ACSS-AIS") Then
                iconMask = "moving{0}.svg"

                If model.NavigationStatus = 1 OrElse model.NavigationStatus = 5 Then
                    iconMask = "anchored{0}.svg"
                End If
            End If

            Select Case model.ShipType.Value
                Case 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19
                    iconUrl += String.Format(iconMask, 0)
                Case 20, 21, 22, 23, 24, 25, 26, 27, 28
                    iconUrl += String.Format(iconMask, 4)
                Case 29
                    iconUrl += String.Format(iconMask, 3)
                Case 30
                    iconUrl += String.Format(iconMask, 2)
                Case 31, 32
                    iconUrl += String.Format(iconMask, 3)
                Case 33, 34, 35
                    iconUrl += String.Format(iconMask, 3)
                Case 36, 37
                    iconUrl += String.Format(iconMask, 9)
                Case 38, 39
                    iconUrl += String.Format(iconMask, 0)
                Case 40, 41, 42, 43, 44, 45, 46, 47, 48, 49
                    iconUrl += String.Format(iconMask, 3)
                Case 51
                    iconUrl += String.Format(iconMask, 3)
                Case 52
                    iconUrl += String.Format(iconMask, 3)
                Case 50, 53, 54, 55, 56, 57, 58, 59
                    iconUrl += String.Format(iconMask, 3)
                Case 60, 61, 62, 63, 64, 65, 66, 67, 68, 69
                    iconUrl += String.Format(iconMask, 6)
                Case 70, 71, 72, 73, 74, 75, 76, 77, 78, 79
                    iconUrl += String.Format(iconMask, 7)
                Case 80, 81, 82, 83, 84, 85, 86, 87, 88, 89
                    iconUrl += String.Format(iconMask, 8)
                Case Else
                    iconUrl += String.Format(iconMask, 0)
            End Select

            Return iconUrl
        End Function

        <Extension()>
        Function ToNavigationStatus(ByVal model As vwCurrentSituation) As String
            Dim result As String = "Not defined"

            If model.NavigationStatus >= 0 AndAlso model.NavigationStatus < 15 Then
                result = NavigationStatuses(model.NavigationStatus)
            End If

            Return result
        End Function

        <Extension()>
        Function ToSIDC(ByVal model As vwCurrentSituation) As String
            Dim result As String = "SOSP--------"

            If model IsNot Nothing Then
                Dim Affiliation As String = "O"

                Select Case model.Amplification
                    Case "PEN"
                        Affiliation = "P"
                    Case "UNK"
                        Affiliation = "U"
                    Case "ASS"
                        Affiliation = "A"
                    Case "FRI"
                        Affiliation = "F"
                    Case "NEU"
                        Affiliation = "N"
                    Case "SUS"
                        Affiliation = "S"
                    Case "HOS"
                        Affiliation = "H"
                    Case "JOK"
                        Affiliation = "J"
                    Case "FAK"
                        Affiliation = "K"
                    Case Else
                        Affiliation = "O"
                End Select

                Dim BattleDimension As String = "S"

                Select Case model.Identity
                    Case "SUR"
                        BattleDimension = "S"
                    Case "AIR"
                        BattleDimension = "A"
                    Case "SUB"
                        BattleDimension = "U"
                    Case Else
                        BattleDimension = "S"
                End Select

                result = $"S{Affiliation}{BattleDimension}P--------"
            End If

            If result.Equals("SNSP--------") Then

                If model.Category.Equals("Base Station") Then
                    result = "SNGPES------"
                Else

                    Select Case model.ShipTypeClass
                        Case "Cargo"
                            result = "SNSPXMC-----"
                        Case "Fishing"
                            result = "SNSPXF------"
                        Case "Passenger"
                            result = "SNSPXMP-----"
                        Case "Pleasure Craft", "Sailing Vessel"
                            result = "SNSPXR------"
                        Case "Search and Rescue"
                            result = "SNSPXL------"
                        Case "Tanker"
                            result = "SNSPXMO-----"
                        Case Else
                            result = "SNSPXM------"
                    End Select
                End If
            End If

            Return result
        End Function
    End Module

    Public Class filterObject
        Public Property identities As String()
        Public Property excludeCategories As String()


    End Class
End Namespace