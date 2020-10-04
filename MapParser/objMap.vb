Imports System.Windows.Forms.DataVisualization.Charting

Public Class objMap
    'use each objMap object as a selectable back buffer to draw in entirety to the mapArea if any update needed to gfx
    'load objMap objects up with their own offsets and any other specific data

    'to remove something from a map backbuffer object <Remove from list?> then redraw bb object

    Private _sZone As String
    Private _sAlphaName As String
    Private _sWhoName As String
    Private _sMapPath As String
    'Private _iMenuIndex As Integer
    Private _sglScaleFactor As Single = 0.0
    Private _point3dOffSet As Point3D

    Private _dictPlayers As New Dictionary(Of String, objPlayer) 'dictionary of players currently on the map
    'Private dictWaypoints As New Dictionary(Of Integer, objWaypoint) 'maychange local key to a unique server key for deleteing waypoints in multi more easily
    Private _iWpIndex As Integer = 0
    Private _bmpMapImg As Bitmap 'the original map image from file
    Private _bmpMap As Bitmap 'this is the "back buffer" for each map, everything that is seen is drawn here first
    Private _gfxDrawingArea As Graphics

    Public Sub New() 'empty default constructor

    End Sub

    'constructor sets up a fresh empty map with all associated settings and data
    'sZone0, sFileName1, sWhoName2, sAlphaName3, iEqGridSize4, iMapGridSize5, dblMapOffSetX6, dblMapOffSetY7
    Public Sub New(ByVal sZone As String, ByVal sFileName As String, ByVal sWhoName As String, ByVal sAlphaName As String, ByVal iEqGridSize As Integer, ByVal iMapGridSize As Integer, ByVal point3dOffSet As Point3D) 'custom constructor
        _sZone = sZone
        '_iMenuIndex = iMenuIndex
        _sWhoName = sWhoName
        _sAlphaName = sAlphaName
        _sglScaleFactor = iEqGridSize / iMapGridSize 'calculate scale factor on the fly, should be more accurate.
        _point3dOffSet = point3dOffSet
        _dictPlayers = dBaseControl.initPlyrDict(sZone, _sglScaleFactor, _point3dOffSet)
        _sMapPath = Application.StartupPath & "\Maps\" & sFileName
        _bmpMapImg = New Bitmap(_sMapPath) 'possibly merge these 2 bmp's into one
        _bmpMap = New Bitmap(_bmpMapImg, _bmpMapImg.Width, _bmpMapImg.Height)
        _gfxDrawingArea = Graphics.FromImage(_bmpMap)
        drawMap()
    End Sub

    Public ReadOnly Property getMap() As Bitmap
        Get
            Return _bmpMap
        End Get
    End Property

    Public ReadOnly Property whoName() As String
        Get
            Return _sWhoName
        End Get
    End Property

    'Public ReadOnly Property getMenuIndex() As Integer
    '    Get
    '        Return _iMenuIndex
    '    End Get
    'End Property

    Public ReadOnly Property scaleFactor() As Single
        Get
            Return _sglScaleFactor
        End Get
    End Property

    Public ReadOnly Property offSet() As Point3D
        Get
            Return _point3dOffSet
        End Get
    End Property

    Public Sub addPlayer(objPlyr As objPlayer)
        _dictPlayers.Add(objPlyr.charKey, objPlyr)
        drawMap()
    End Sub

    Public Sub updatePlayerMarker(sCharKey As String, sglPX As Single, sglPY As Single, sglQX As Single, sglQY As Single, bFirstPlot As Boolean, bJustZoned As Boolean, bConnected As Boolean)
        _dictPlayers.Item(sCharKey).updateMarker(sglPX, sglPY, sglQX, sglQY, _sglScaleFactor, _point3dOffSet, bFirstPlot, bJustZoned, bConnected)
        drawMap()
    End Sub

    'Public WriteOnly Property movPlayer() As objPlayer 'for manual map change will not draw player to map, only add to maps Dictionary
    '    Set(objPlyr As objPlayer)
    '        If Not _dictPlayers.ContainsKey(objPlyr.charKey) Then
    '            _dictPlayers.Add(objPlyr.charKey, objPlyr)
    '        Else
    '            _dictPlayers.Item(objPlyr.charKey).mkrPlayer = objPlyr.mkrPlayer
    '        End If
    '    End Set
    'End Property

    Public Sub removePlayer(sCharKey As String)
        _dictPlayers.Remove(sCharKey)
        drawMap()
    End Sub

    Public ReadOnly Property chkPlayer(sCharKey As String) As Boolean
        Get
            Return _dictPlayers.ContainsKey(sCharKey)
        End Get
    End Property

#Region "stuff"
    'Public Sub updPlayer(ByVal objPlyr As objPlayer) 'upSerts a player to the dictionary and sets its markers location
    '    If Not _dictPlayers.ContainsKey(objPlyr.charKey) Then
    '        _dictPlayers.Add(objPlyr.charKey, objPlyr)
    '    Else
    '        _dictPlayers.Item(objPlyr.charKey).mkrPlayer = objPlyr.mkrPlayer
    '    End If
    '    drawMap()
    'End Sub

    'Public Sub movPlayer(ByVal objPlyr As objPlayer)
    '    If Not _dictPlayers.ContainsKey(objPlyr.charKey) Then
    '        _dictPlayers.Add(objPlyr.charKey, objPlyr)
    '    Else
    '        _dictPlayers.Item(objPlyr.charKey).mkrPlayer = objPlyr.mkrPlayer
    '    End If
    'End Sub

    'Public Function chkPlayer(ByVal sCharKey As String) As Boolean 'checks if a player is on the map by player key
    '    chkPlayer = _dictPlayers.ContainsKey(sCharKey)
    'End Function

    'Public Sub remPlayer(sCharKey As String)
    '    _dictPlayers.Remove(sCharKey)
    '    drawMap()
    'End Sub

    'Public Sub addWaypoint(ByRef ply As objPlayer, wpStr As structWaypoint)
    '    _iWpIndex = _iWpIndex + 1 'replce with better index with multiplayer
    '    waypoints.Add(wpIndex, New objWaypoint(ply.charKey, wpStr.ctr, properName, wpStr, ply.Plot.Color))
    '    addWpToMap(waypoints.Last.Value)
    'End Sub

    'Public Sub remLastWaypoint()
    '    If waypoints.Count > 0 Then
    '        waypoints.Remove(waypoints.Last.Key)
    '        drawMap()
    '    End If
    'End Sub

    'Public Sub remAllWaypoints() ' rmeove all waypoints on map
    '    waypoints.Clear()
    '    drawMap()
    'End Sub

    'Public Sub remPlyrsWps(ByVal charKey As String)
    '    For Each wp As KeyValuePair(Of Integer, objWaypoint) In waypoints
    '        If wp.Value.charKey = charKey Then
    '            waypoints.Remove(wp.Key)
    '        End If
    '    Next
    '    drawMap()
    'End Sub

    'Private Sub addWpToMap(wp As objWaypoint) 'for adding static markers such as waypoints' saves the need of haveing to redraw the entire map
    '    _gfxDrawingArea.DrawLine(wp.wpPen, wp.wpStruct.cb1(0).X, wp.wpStruct.cb1(0).Y, wp.wpStruct.cb1.q.X, wp.wpStruct.cb1.q.Y)  'cb1
    '    _gfxDrawingArea.DrawLine(wp.wpPen, wp.wpStruct.cb2(0).X, wp.wpStruct.cb2(0).Y, wp.wpStruct.cb2.q.X, wp.wpStruct.cb2.q.Y)  'cb2
    'End Sub
#End Region

    'Clear the entire map gfx and reload a clean map
    Private Sub clearMap()
        _bmpMapImg = New Bitmap(_sMapPath)
        _bmpMap = New Bitmap(_bmpMapImg, _bmpMapImg.Width, _bmpMapImg.Height)
        _gfxDrawingArea = Graphics.FromImage(_bmpMap)
    End Sub

    Private Sub drawMap() 'all co'ordinates here are scaled vbPictureBox co-ords NOT EQ /loc's
        clearMap() 'clear map to a blank map image for this zone, ready for re-drawing
        For Each kvpPlyr As KeyValuePair(Of String, objPlayer) In _dictPlayers
            Dim ply As objPlayer = kvpPlyr.Value 'copy player into temp object 'shouldnt need to copy back unless alter edge coords are needed.

            If ply.justZoned = False Then 'do not draw on map if player has just zoned in, so to not use old zones coords in error
                Dim bEdge As Boolean = False 'calculated here store and use no where else.
                'players name drawn by marker with font here?
                'determine if player is off a map edge 'will happen for any marker so it can be drawn on the edge of the map if needed
                Select Case ply.scaledCoords.p.Y
                    Case Is < 0 'Top 'Zone edge detection for drawing at edge of map when player is off map
                        Dim sglYShift As Single = ply.scaledCoords.p.Y
                        ply.scaledCoords.q.Y -= sglYShift
                        ply.scaledCoords.p.Y = 0
                        'ply.mkrPlayer.shiftY(sglYShift)
                        bEdge = True
                    Case Is > _bmpMapImg.Height 'Bottom
                        Dim sglYShift As Single = (ply.scaledCoords.p.Y - _bmpMapImg.Height)
                        ply.scaledCoords.q.Y -= sglYShift
                        ply.scaledCoords.p.Y = _bmpMapImg.Height
                        'ply.mkrPlayer.shiftY(sglYShift)
                        bEdge = True                '^ gets the difference, how much to move cursor
                End Select
                '^if player is within the map bounderies do not alter the locs
                Select Case ply.scaledCoords.p.X
                    Case Is < 0 'Top 'Zone edge detection for drawing at edge of map when player is off map
                        Dim sglXShift As Single = ply.scaledCoords.p.X
                        ply.scaledCoords.q.X -= sglXShift
                        ply.scaledCoords.p.X = 0
                        'ply.mkrPlayer.shiftX(sglXShift)
                        bEdge = True
                    Case Is > _bmpMapImg.Width 'Bottom
                        Dim sglXShift As Single = (ply.scaledCoords.p.X - _bmpMapImg.Width)
                        ply.scaledCoords.q.X -= sglXShift
                        ply.scaledCoords.p.X = _bmpMapImg.Width
                        'ply.mkrPlayer.shiftX(sglXShift)
                        bEdge = True
                End Select
                '^if player is within the map bounderies do not alter the locs

                ply.calcMarker()

                If ply.connected = True Then 'draw other markers
                    If ply.firstPlot = False Then 'draw direction markers
                        If bEdge = False Then 'draw direction marker on map (coloured cross)
                            _gfxDrawingArea.DrawLine(ply.Plot, ply.mkrPlayer.hb.p.X, ply.mkrPlayer.hb.p.Y, ply.mkrPlayer.hb.q.X, ply.mkrPlayer.hb.q.Y)  'heading bar
                            _gfxDrawingArea.DrawLine(ply.Plot, ply.mkrPlayer.cb.p.X, ply.mkrPlayer.cb.p.Y, ply.mkrPlayer.cb.q.X, ply.mkrPlayer.cb.q.Y) 'crossbar
                        Else '1 circle on map edge (with the direction marker)
                            Dim iRadius As Integer = 11
                            Dim rect As New Rectangle(ply.mkrPlayer.ctr.X - 5.5, ply.mkrPlayer.ctr.Y - 5.5, iRadius, iRadius)
                            _gfxDrawingArea.DrawEllipse(ply.Plot, rect)
                        End If
                        '(black arrow heads) (over the top of edge and cross markers)
                        _gfxDrawingArea.DrawLine(ply.Dir, ply.mkrPlayer.ar.p.X, ply.mkrPlayer.ar.p.Y, ply.mkrPlayer.hb.p.X, ply.mkrPlayer.hb.p.Y) 'arrowhead
                        _gfxDrawingArea.DrawLine(ply.Dir, ply.mkrPlayer.ar.q.X, ply.mkrPlayer.ar.q.Y, ply.mkrPlayer.hb.p.X, ply.mkrPlayer.hb.p.Y)
                        _gfxDrawingArea.DrawLine(ply.Dir, ply.mkrPlayer.ar.p.X, ply.mkrPlayer.ar.p.Y, ply.mkrPlayer.ar.q.X, ply.mkrPlayer.ar.q.Y)
                    Else 'first plot 1 
                        'no direction plotting possible
                        'circle in or on edge of map depending on if the coords have been shifted by edge detection
                        Dim iRadius As Integer = 11
                        Dim rect As New Rectangle(ply.mkrPlayer.ctr.X - 5.5, ply.mkrPlayer.ctr.Y - 5.5, iRadius, iRadius)
                        _gfxDrawingArea.DrawEllipse(ply.Plot, rect)
                    End If
                Else '0 if not connected draw squares, including for first plot
                    'draw square
                    Dim iEdgeLen As Integer = 11
                    Dim rect As New Rectangle(ply.mkrPlayer.ctr.X - 5.5, ply.mkrPlayer.ctr.Y - 5.5, iEdgeLen, iEdgeLen)
                    _gfxDrawingArea.DrawRectangle(ply.Plot, rect)
                End If
            End If

        Next

        'draw waypoints that war already on map
        'For Each wp As KeyValuePair(Of Integer, objWaypoint) In waypoints
        '    drawingArea.DrawLine(wp.Value.wpPen, wp.Value.wpStruct.cb1(0).X, wp.Value.wpStruct.cb1(0).Y, wp.Value.wpStruct.cb1.q.X, wp.Value.wpStruct.cb1.q.Y)  'cb1
        '    drawingArea.DrawLine(wp.Value.wpPen, wp.Value.wpStruct.cb2(0).X, wp.Value.wpStruct.cb2(0).Y, wp.Value.wpStruct.cb2.q.X, wp.Value.wpStruct.cb2.q.Y)  'cb2
        'Next
    End Sub

    'Public Function calcWaypoint(ByRef wp As structPoint, ByRef map As objMap) As structWaypoint
    '    'no need for vector for simple waypoint (.ctr is Scaled, not EQ /loc)
    '    Dim size As Integer = 9 'size of cross

    '    calcWaypoint.ctr.X = -wp.X / map.scaleFactor + map.offSet.X 'scaled loc
    '    calcWaypoint.ctr.Y = -wp.Y / map.scaleFactor + map.offSet.Y

    '    'calculate cross locs
    '    calcWaypoint.cb1.p.X = calcWaypoint.ctr.X + size
    '    calcWaypoint.cb1.p.Y = calcWaypoint.ctr.Y - size
    '    calcWaypoint.cb1.q.X = calcWaypoint.ctr.X - size
    '    calcWaypoint.cb1.q.Y = calcWaypoint.ctr.Y + size

    '    calcWaypoint.cb2.p.X = calcWaypoint.ctr.X + size
    '    calcWaypoint.cb2.p.Y = calcWaypoint.ctr.Y + size
    '    calcWaypoint.cb2.q.X = calcWaypoint.ctr.X - size
    '    calcWaypoint.cb2.q.Y = calcWaypoint.ctr.Y - size
    'End Function

End Class
