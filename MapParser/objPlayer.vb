Imports System.Windows.Forms.DataVisualization.Charting

Public Class objPlayer 'using "_" underscore as a property marker
    Private _sCharKey As String
    'Private _sCharName As String
    'Private _server As String
    Private _sZone As String
    Private _mkrPlayer As objLocMarker
    Private _bFirstPlot As Boolean
    Private _bJustZoned As Boolean
    Private _bConnected As Boolean
    'Private _bEdge As Boolean = 0 'do not store in DB this is calculated in calcMarker on the fly
    Private _dblp3dCoords As objDblPoint3D 'unscaled EQ /locs
    Private _dblp3dScaledCoords As objDblPoint3D 'scaled map coordinates
    'need scaled and EQ unscaled coords stored here to calc marker.
    Private _penPlot As Pen = New Pen(Color.Red, 2) 'Players plotter/pen
    Private _penDir As Pen = New Pen(Color.Black, 2) 'Players plotter/pen direction (arrow head in black)

    Public Sub New()

    End Sub

    Public Sub New(ByVal sCharKey As String, ByVal sZone As String, ByVal dblp3dCoords As objDblPoint3D, sglScaleFactor As Single, point3dOffset As Point3D, ByVal bFirstPlot As Boolean, ByVal bJustZoned As Boolean, ByVal bConnected As Boolean)
        _sCharKey = sCharKey 'name and server (split in here?)
        _sZone = sZone
        _dblp3dCoords = dblp3dCoords
        _bFirstPlot = bFirstPlot
        _bJustZoned = bJustZoned
        _bConnected = bConnected
        'calculate map marker here
        calcScaled(_dblp3dCoords.p.X, _dblp3dCoords.p.Y, _dblp3dCoords.q.X, _dblp3dCoords.q.Y, sglScaleFactor, point3dOffset, _bFirstPlot, _bJustZoned, _bConnected)
    End Sub

    Public Sub updateMarker(sglPX As Single, sglPY As Single, sglQX As Single, sglQY As Single, sglScaleFactor As Single, point3dOffset As Point3D, bFirstPlot As Boolean, bJustZoned As Boolean, bConnected As Boolean)
        '_dblp3dCoords = dblp3dCoords
        With _dblp3dCoords
            .p.X = sglPX
            .p.Y = sglPY
            .q.X = sglQX
            .q.Y = sglQY
        End With
        _bFirstPlot = bFirstPlot
        _bJustZoned = bJustZoned
        _bConnected = bConnected
        'calculate map marker here
        calcScaled(_dblp3dCoords.p.X, _dblp3dCoords.p.Y, _dblp3dCoords.q.X, _dblp3dCoords.q.Y, sglScaleFactor, point3dOffset, _bFirstPlot, _bJustZoned, _bConnected)
    End Sub

    'may need another contructor which does not calc marker, for updates that do not include locs.

    Public Property Plot() As Pen
        Get
            Return _penPlot
        End Get
        Set(penPlot As Pen)
            _penPlot = penPlot
        End Set
    End Property


    Public Property Dir() As Pen
        Get
            Return _penDir
        End Get
        Set(penDir As Pen)
            _penDir = penDir
        End Set
    End Property

    Public Property coords() As objDblPoint3D
        Get
            Return _dblp3dCoords
        End Get
        Set(dblp3dCoords As objDblPoint3D)
            _dblp3dCoords = dblp3dCoords
        End Set
    End Property

    Public Property scaledCoords As objDblPoint3D
        Get
            Return _dblp3dScaledCoords
        End Get
        Set(dblp3dScaledCoords As objDblPoint3D)
            _dblp3dScaledCoords = dblp3dScaledCoords
        End Set
    End Property

    Public Property mkrPlayer() As objLocMarker
        Get
            Return _mkrPlayer
        End Get
        Set(mkrPlayer As objLocMarker)
            _mkrPlayer = mkrPlayer
        End Set
    End Property

    'Public Property lastLoc() As Point3D
    '    Get
    '        Return _LastLoc
    '    End Get
    '    Set(loc As Point3D)
    '        _LastLoc = loc
    '    End Set
    'End Property

    Public ReadOnly Property charKey() As String
        Get
            Return _sCharKey
        End Get
    End Property

    'Public ReadOnly Property charName() As String
    '    Get
    '        Return _sCharName
    '    End Get
    'End Property

    'Public ReadOnly Property server() As String
    '    Get
    '        Return _server
    '    End Get
    'End Property

    Public Property currentZone() As String 'tells server which map clients should draw player on
        Get
            Return _sZone
        End Get
        Set(sZone As String)
            _sZone = sZone
        End Set
    End Property

    Public Property firstPlot() As Boolean
        Get
            Return _bFirstPlot
        End Get
        Set(bFirstPlot As Boolean)
            _bFirstPlot = bFirstPlot 'made writeable for change map, will be made the same by monitor thread at same time as player obj
        End Set
    End Property

    Public ReadOnly Property justZoned() As Boolean
        Get
            Return _bJustZoned
        End Get
    End Property

    Public ReadOnly Property connected() As Boolean
        Get
            Return _bConnected
        End Get
    End Property

    'implement code for decideing which marker to use for the playere here, move player marker code code in from objMap, 
    'use bConnected to decide if player should be drawn as an offline marker or as one of the connected markers
    'use bFirst plot if bConnected is true to decide if player should be drawn as a circle or a pointer

    'Scaled coordinates for map calculation
    Public Sub calcScaled(sglPX As Single, sglPY As Single, sglQX As Single, sglQY As Single, sglScaleFactor As Single, point3dOffSet As Point3D, bFirstPlot As Boolean, bJustZoned As Boolean, ByVal bConnected As Boolean)
        'if just zoned dont draw anything as coordinates will be for old zone
        Dim dblp3dScaledCoords As New objDblPoint3D 'scaled location
        'p (X,Y,Z)1 is origin, q (X,Y,Z)2 is end point '(in vector maths)
        dblp3dScaledCoords.p.X = -sglPX / sglScaleFactor + point3dOffSet.X 'scaled and offset x (Scaled origins - current loc)
        dblp3dScaledCoords.p.Y = -sglPY / sglScaleFactor + point3dOffSet.Y 'scaled and offset y

        If bFirstPlot = False Then 'only calc scaled end points if this is not the first plot in the zone
            dblp3dScaledCoords.q.X = -sglQX / sglScaleFactor + point3dOffSet.X '(last loc)
            dblp3dScaledCoords.q.Y = -sglQY / sglScaleFactor + point3dOffSet.Y
        End If
        _dblp3dScaledCoords = dblp3dScaledCoords
    End Sub

    'Marker calculation
    Public Sub calcMarker()
        'draw direction markers once here for both in map and edge
        Dim mkr As New objLocMarker 'players online direction marker 'change to objDirMarker
        'Dim ep As structVect 'scaled end point
        Dim point3dVec As New Point3D 'result of vector calculation
        Dim point3dUvec As New Point3D
        Dim sglScaler As Single = 9.0 'scaler changes the size of the cross and arrow head
        point3dVec.X = _dblp3dScaledCoords.q.X - _dblp3dScaledCoords.p.X 'vector x component
        point3dVec.Y = _dblp3dScaledCoords.q.Y - _dblp3dScaledCoords.p.Y 'vector y component
        'Need vector for sx,sy, ex,ey (a vector is a pair of numbers showing the difference between the 2 points)
        'Need Length of vector using pythag (for vectors this is also called the magnitude)
        'Calculate unit vector by dividing vector by length(magnitude)
        Dim mag As Single = Math.Sqrt((point3dVec.X ^ 2) + (point3dVec.Y ^ 2)) '(length)
        point3dUvec.X = point3dVec.X / mag 'unit vectors
        point3dUvec.Y = point3dVec.Y / mag
        'hba As structPoint 'Scaled vector start points
        'hbb As structPoint 'Scaled vector end points
        mkr.hb.p.X = _dblp3dScaledCoords.q.X - (point3dUvec.X * sglScaler)
        mkr.hb.p.Y = _dblp3dScaledCoords.q.Y - (point3dUvec.Y * sglScaler)
        mkr.hb.q.X = _dblp3dScaledCoords.q.X + (point3dUvec.X * sglScaler)
        mkr.hb.q.Y = _dblp3dScaledCoords.q.Y + (point3dUvec.Y * sglScaler)
        'crossbar
        mkr.cb.p = rotatePoint(mkr.hb.q, _dblp3dScaledCoords.q, 90)
        mkr.cb.q = rotatePoint(mkr.hb.q, _dblp3dScaledCoords.q, 270)
        'arrow head
        mkr.ar.p = rotatePoint(_dblp3dScaledCoords.q, mkr.hb.p, 45)
        mkr.ar.q = rotatePoint(_dblp3dScaledCoords.q, mkr.hb.p, -45)
        'centre of cross 'scaled loc
        mkr.ctr = _dblp3dScaledCoords.p

        _mkrPlayer = mkr
    End Sub 'used by refreshMap

    Private Function rotatePoint(ByRef point3dQ As Point3D, ByRef point3dP As Point3D, ByRef sglDegrees As Single) As Point3D
        rotatePoint = New Point3D
        rotatePoint.X = point3dP.X + (Math.Cos(d2R(sglDegrees)) * (point3dQ.X - point3dP.X) - Math.Sin(d2R(sglDegrees)) * (point3dQ.Y - point3dP.Y))
        rotatePoint.Y = point3dP.Y + (Math.Sin(d2R(sglDegrees)) * (point3dQ.X - point3dP.X) + Math.Cos(d2R(sglDegrees)) * (point3dQ.Y - point3dP.Y))
        Return rotatePoint
    End Function 'used by pbxMapArea_Paint

    Private Function d2R(ByVal sglAngle As Single) As Single 'degrees to radians
        d2R = sglAngle / 180 * Math.PI
    End Function 'used by rotate point

End Class
