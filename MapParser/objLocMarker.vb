Imports System.Windows.Forms.DataVisualization.Charting

Public Class objLocMarker
    Private _hb As New objDblPoint3D 'heading bar
    Private _cb As New objDblPoint3D 'crossbar
    Private _ar As New objDblPoint3D 'arrowhead
    Private _ctr As New Point3D 'centre of cross (actual scaled location)

    Public Sub New() 'empty default constructor

    End Sub

    Public Sub shiftY(sglYShift As Single)
        _hb.p.Y -= sglYShift
        _hb.q.Y -= sglYShift
        _cb.p.Y -= sglYShift
        _cb.q.Y -= sglYShift
        _ar.p.Y -= sglYShift
        _ar.q.Y -= sglYShift
        '_ctr.Y -= sglYShift
    End Sub

    Public Sub shiftX(sglXShift As Single)
        _hb.p.X -= sglXShift
        _hb.q.X -= sglXShift
        _cb.p.X -= sglXShift
        _cb.q.X -= sglXShift
        _ar.p.X -= sglXShift
        _ar.q.X -= sglXShift
        '_ctr.X -= sglXShift
    End Sub

    Public Property hb() As objDblPoint3D
        Get
            Return _hb
        End Get
        Set(hb As objDblPoint3D)
            _hb = hb
        End Set
    End Property

    Public Property cb() As objDblPoint3D
        Get
            Return _cb
        End Get
        Set(cb As objDblPoint3D)
            _cb = cb
        End Set
    End Property

    Public Property ar() As objDblPoint3D
        Get
            Return _ar
        End Get
        Set(ar As objDblPoint3D)
            _ar = ar
        End Set
    End Property

    Public Property ctr() As Point3D
        Get
            Return _ctr
        End Get
        Set(ctr As Point3D)
            _ctr = ctr
        End Set
    End Property

End Class
