Imports System.Windows.Forms.DataVisualization.Charting

Public Class objDblPoint3D
    Private _p As New Point3D
    Private _q As New Point3D

    Public Sub New()
        _p.X = 0
        _p.Y = 0
        _p.Z = 0
        _q.X = 0
        _q.Y = 0
        _q.Z = 0
    End Sub

    Public Sub New(ByVal pX As Single, ByVal pY As Single, ByVal qX As Single, ByVal qY As Single)
        _p.X = pX
        _p.Y = pY
        _p.Z = 0
        _q.X = qX
        _q.Y = qY
        _q.Z = 0
    End Sub

    Public Property p() As Point3D
        Get
            Return _p
        End Get
        Set(p As Point3D)
            _p = p
        End Set
    End Property

    Public Property q() As Point3D
        Get
            Return _q
        End Get
        Set(q As Point3D)
            _q = q
        End Set
    End Property

End Class
