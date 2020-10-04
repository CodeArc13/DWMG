Imports System.IO
Imports System.Windows.Forms.DataVisualization.Charting

Module mapControl
    'switching map causes init to run which loads correct map information from map database
    Private sCurrentMap As String 'sZone
    Private sMapInfo As String()
    'Private bitmap for map

    Private dictMaps As New Dictionary(Of String, objMap)
    Private dictWhoName As New Dictionary(Of String, String) 'sWhoZoneName as key, sZone as Value, for looking up the sZone from who name instead of querying SQLite Database
    'Private dictMapIndex As New Dictionary(Of Integer, String) 'stores index of maps for menu selection. initilised in dBaseControl.initMapDict
    Private dictPlayerZones As New Dictionary(Of String, String) 'sCharKey, sZone. For removing players from zones when a zone message is recieved from parser or network, initilised from dBaseControl.initPlyrDict
    'Private dictPlayers As New Dictionary(Of String, objPlayer)
    'Private dictWaypoints As New Dictionary(Of String, objWaypoint)

    Public Sub init() 'populates map dictionary with maps and data inc player dictionaries and wps
        'player and wP dictionaries set up inside each maps constructor
        dictMaps = dBaseControl.initMapDict
        For Each map As KeyValuePair(Of String, objMap) In dictMaps
            dictWhoName.Add(map.Value.whoName, map.Key)
        Next
        'set a default map here?
        switchMap("East Commonlands") 'the default map is east commonlands
    End Sub

    'Public Sub whoSwitchMap(sWhoName As String)
    '    switchMap(dictWhoName.Item(sWhoName))
    'End Sub

    Public Sub switchMap(sZone As String) '/load new map
        If sZone <> sCurrentMap Then 'only switchMap if not the same as old map, saves redrawing
            If dictMaps.ContainsKey(sZone) Then 'only refresh map if sZone is known by the map dict
                sCurrentMap = sZone
                'change cboMapMenu's index
                frmMap.setMapMenu(sZone)
                frmMap.refreshMap(sCurrentMap)
            Else 'no map in dict
                frmMap.appendData("There is, as of yet, no map database entry for " & sZone & ", plotting not possible." & vbCrLf)
            End If
        End If
    End Sub

    Public Sub addtoPlayerZones(sCharKey As String, sZone As String)
        dictPlayerZones.Add(sCharKey, sZone)
    End Sub

    Public Sub remFromZone(sCharKey As String)
        Try
            Dim sZone As String = dictPlayerZones.Item(sCharKey) 'the map the character is locally stored in
            dictMaps.Item(sZone).removePlayer(sCharKey)
            frmMap.refreshMap(sZone)
            dictPlayerZones.Remove(sCharKey)
        Catch ex As Exception
            'don't care if there is no key to remove, job done
        End Try
    End Sub

    Public Sub remOldMarker(sCharKey As String, sZoneFromLog As String) 'attempts to remove invalid offline markers from any zone that does not match the characters found zone, used on initial zone only
        Try
            Dim sZone As String = dictPlayerZones.Item(sCharKey) 'sZone here = the map the character is locally stored in the dictionaries
            If sZone <> sZoneFromLog Then 'if zone the char in reported by log different from zone char in recorded by dictionaries, remove incorrect entry
                dictMaps.Item(sZone).removePlayer(sCharKey)
                frmMap.refreshMap(sZone)
                dictPlayerZones.Remove(sCharKey)
            End If
        Catch ex As Exception
            'don't care if there is no key to remove, job done
        End Try
    End Sub

    Public Function getCurrentMap() As Bitmap
        Return dictMaps.Item(sCurrentMap).getMap
    End Function

    Public Function getZoneFromWho(sWhoName As String) As String
        Try
            Return dictWhoName.Item(sWhoName)
        Catch ex As KeyNotFoundException
            Return "Unknown"
        End Try
    End Function

    Public ReadOnly Property currentMap As String
        Get
            Return sCurrentMap
        End Get
    End Property

    Public Sub upsertPlayerLoc(sCharKey As String, sglPX As Single, sglPY As Single, sglQX As Single, sglQY As Single, sZone As String, bFirstPlot As Boolean, bJustZoned As Boolean, bConnected As Boolean)
        Try
            If dictPlayerZones.Item(sCharKey) = sZone Then 'if char exists in the correct zone then just update the marker, dont recreate the entire player object
                'dictMaps.Item(sZone).upsertPlayerMarker(New objPlayer(sCharKey, sZone, New objDblPoint3D(sglPX, sglPY, sglQX, sglQY), bFirstPlot)) 'update the marker
                dictMaps.Item(sZone).updatePlayerMarker(sCharKey, sglPX, sglPY, sglQX, sglQY, bFirstPlot, bJustZoned, bConnected)
            Else 'if the zone sent does not match the zone the player is reported to be in in PlayerZones dict then correct
                dictMaps.Item(dictPlayerZones.Item(sCharKey)).removePlayer(sCharKey) 'char in wrong zone remove from map dict
                dictPlayerZones.Item(sCharKey) = sZone 'update playerZones dict to correct zone
                dictMaps.Item(sZone).addPlayer(New objPlayer(sCharKey, sZone, New objDblPoint3D(sglPX, sglPY, sglQX, sglQY), dictMaps.Item(sZone).scaleFactor, dictMaps.Item(sZone).offSet, bFirstPlot, bJustZoned, bConnected)) 'add to correct map dict
            End If
        Catch exNoPlayerOrMap As KeyNotFoundException 'if there is no player key then add a new one to dictMaps and dictPlayerZones
            Try 'this will test to see if the map is missing in map dict
                dictMaps.Item(sZone).addPlayer(New objPlayer(sCharKey, sZone, New objDblPoint3D(sglPX, sglPY, sglQX, sglQY), dictMaps.Item(sZone).scaleFactor, dictMaps.Item(sZone).offSet, bFirstPlot, bJustZoned, bConnected)) 'add the player and marker
                dictPlayerZones.Add(sCharKey, sZone)
            Catch exNoMap As KeyNotFoundException 'missing map (this will either be a result of a missing database entry or a database entry with a missing map.jpg)
                frmMap.appendData("There is, as of yet, no map database entry for the zone " & sCharKey & " is in, plotting not possible." & vbCrLf)
            End Try
        End Try
        frmMap.refreshMap(sZone) 'will only refresh displayed map if the update was for it
    End Sub

    'Public Function 

    'Public Function init(sZone As String) As String 'make function 'Return "No Data" for no database entry and "No Map" for no Jpg
    '    'sFileName 0, sWhoName 1, sAlphaName 2, iEqGridSize 3, iMapGridSize 4, dblMapOffSetX 5, dblMapOffSetY 6
    '    sMapInfo = dBaseControl.initMapObjs(sZone) 'Get and Check MapData exists in DB, and Map.jpg Exits in maps folder
    '    If sMapInfo Is Nothing Then
    '        sCurrentMap = "No Data"
    '        Return sCurrentMap
    '    Else
    '        If Not File.Exists(Application.StartupPath & "/Maps/" & sMapInfo(0)) Then
    '            sCurrentMap = "No Map"
    '            Return sCurrentMap
    '        End If
    '    End If
    '    'Both data and jpg exist, do stuff to gfx here
    '    sCurrentMap = sZone

    '    'get players from players database for current map
    '    'get waypoints from waypoint database for current map
    '    'assign players and waypoints to array or dictionary list

    '    Return sCurrentMap
    'End Function

    Public Sub updatePlayer()
        'update the map dictionary for the specified zone
        'if update is for currently selected zone then redraw map.

    End Sub

    Public Sub updateWaypoint()

    End Sub


End Module
