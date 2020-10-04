#Region "Imports"
Imports System.IO
Imports System.Threading
Imports System.Text.RegularExpressions
'Imports System.Drawing.Drawing2D
Imports System.Math
Imports System.Net.Sockets
Imports System.Text
Imports System.ComponentModel
Imports System.Text.Encoding
'Imports NetComm

#End Region
'LOG SELECTING NOW DONE BY PARSER DETECTING SIZE CHANGE IN LOG FILES. Open log button will now only select directory of eq, then size change will select log.
Public Class frmMap
#Region "Vars"
    Private dictMapIndex As New Dictionary(Of Integer, String) 'iMenuIndex, sZone 'for manual map selecting
    Private dictMapIndexLookUp As New Dictionary(Of String, Integer) 'the above dictionary reversed to enable indexes to be lookedup from zone names for automap changes
#End Region

    Private Sub frmMap_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        dirAutoDetect()
        dBaseControl.init()
        interfaceControl.init()
        mapControl.init()
        parserControl.init()
        'start parser with dir stored in My.Settings.sEQLocation
        parserControl.runWorkerAsync(My.Settings.sEQLocation)
    End Sub

    Private Sub dirAutoDetect() 'Attempts to find EverQuest in a default location
        If My.Settings.sEQLocation = Nothing Then
            If Directory.Exists("C:\Program Files (x86)\Sony\EverQuest") Then
                My.Settings.sEQLocation = "C:\Program Files (x86)\Sony\EverQuest"
                appendData("Found EverQuest in C:\Program Files (x86)\Sony\EverQuest" & vbCrLf)
            ElseIf Directory.Exists("C:\Program Files\Sony\EverQuest") Then
                My.Settings.sEQLocation = "C:\Program Files\Sony\EverQuest"
                appendData("Found EverQuest in C:\Program Files\Sony\EverQuest" & vbCrLf)
            ElseIf Directory.Exists("C:\Program Files (x86)\EverQuest") Then
                My.Settings.sEQLocation = "C:\Program Files (x86)\EverQuest"
                appendData("Found EverQuest in C:\Program Files (x86)\EverQuest" & vbCrLf)
            ElseIf Directory.Exists("C:\Program Files\EverQuest") Then
                My.Settings.sEQLocation = "C:\Program Files\EverQuest"
                appendData("Found EverQuest in C:\Program Files\EverQuest" & vbCrLf)
            End If
        End If
    End Sub

    Public Sub selectDir()
        appendData("Please select your EverQuest folder" & vbCrLf)
        ofodOpenLogDir.SelectedPath = My.Settings.sEQLocation
        If (ofodOpenLogDir.ShowDialog() = DialogResult.OK) Then
            If ofodOpenLogDir.SelectedPath <> My.Settings.sEQLocation Then 'only set path if folder is actually different, prevents issues with the same log being loaded twice.
                parserControl.runWorkerAsync(ofodOpenLogDir.SelectedPath)
            End If
        End If
    End Sub

    Public Sub appendData(sData As String)
        'sData = sData.Replace("&#39;", "'")
        txtMapData.AppendText(sData)
    End Sub

    Public Sub addtoMapMenu(sAlphaName As String, iMenuIndex As Integer, sZone As String)
        cboMapMenu.Items.Add(sAlphaName)
        dictMapIndex.Add(iMenuIndex, sZone)
        dictMapIndexLookUp.Add(sZone, iMenuIndex)
    End Sub

    Public Sub refreshMap(sZone As String)
        If sZone = mapControl.currentMap Then 'check update is for currently displayed map.
            picMapArea.Refresh() 'should call picMapArea's paint event below
        End If
    End Sub

    Private Sub resizeMap() 'for resizing the map, when user changes the size of the map window or splitContainer
        picMapArea.Refresh() 'should call picMapArea's paint event below
    End Sub

#Region "Interface Events"
    Private Sub picMapArea_Paint(sender As Object, e As PaintEventArgs) Handles picMapArea.Paint
        Try
            Dim bmpMap As Bitmap = mapControl.getCurrentMap()
            Dim iScaleH As Integer
            Dim iScaleW As Integer
            'mapHeight = bmpMap.Height
            'mapWidth = bmpMap.Width

            If bmpMap.Height > picMapArea.Height Or bmpMap.Width > picMapArea.Width Then
                Dim sglRatioW As Single = bmpMap.Width / picMapArea.Width
                Dim sglRatioH As Single = bmpMap.Height / picMapArea.Height
                Dim sglRatio As Single = Max(sglRatioW, sglRatioH)
                iScaleW = bmpMap.Width / sglRatio
                iScaleH = bmpMap.Height / sglRatio
            Else 'No scale as mapSize is smaller than picture box
                iScaleW = bmpMap.Width
                iScaleH = bmpMap.Height
            End If

            e.Graphics.DrawImage(bmpMap, 0, 0, iScaleW, iScaleH) 'draw currently selected bmpMap
        Catch ex As ArgumentNullException

        End Try
    End Sub

    Public Sub setMapMenu(sZone As String)
        cboMapMenu.SelectedIndex = dictMapIndexLookUp.Item(sZone)
    End Sub

    Private Sub cboMapMenu_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboMapMenu.SelectedIndexChanged
        mapControl.switchMap(dictMapIndex.Item(cboMapMenu.SelectedIndex()))
    End Sub

    'Map resizing
    Private Sub frmMap_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        resizeMap()
    End Sub

    Private Sub Sc1Panel1_Resize(sender As Object, e As EventArgs) Handles splitContainer1.SplitterMoved
        resizeMap()
    End Sub

    Private Sub cmdAutoSwitchMap_Click(sender As Object, e As EventArgs) Handles cmdAutoSwitchMap.Click
        interfaceControl.autoSwitchMaps()
    End Sub

    Private Sub cmdMapData_Click(sender As Object, e As EventArgs) Handles cmdMapData.Click
        interfaceControl.mapDataVisible()
    End Sub

    Private Sub cmdOnTop_Click(sender As Object, e As EventArgs) Handles cmdOnTop.Click
        interfaceControl.alwaysOnTop()
    End Sub

    Private Sub cboOpacity_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboOpacity.SelectedIndexChanged
        interfaceControl.opacity()
    End Sub

    Private Sub cmdSelectDir_Click(sender As Object, e As EventArgs) Handles cmdSelectDir.Click
        'selecting a new log must stop the parser
        selectDir()
    End Sub

    Private Sub frmMap_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        'parserControl.cancelAsync()
        interfaceControl.wndLoc()
        'tell server going offline so it can set the status of the char
        'set char in local DB to offline as well (do not set first plot)
    End Sub
#End Region


End Class
