Module interfaceControl

    Public Sub init()

        'Load the form in the last location it was placed
        frmMap.Size = My.Settings.drawSWndSize
        frmMap.Location = My.Settings.drawPWndLoc

        If My.Settings.bOnTop = True Then
            frmMap.TopMost = True
            frmMap.appendData("Always On Top, On" & vbCrLf)
            frmMap.cmdOnTop.Image = My.Resources.AlwaysOnTop
        Else
            frmMap.TopMost = False
            frmMap.appendData("Always On Top, Off" & vbCrLf)
            frmMap.cmdOnTop.Image = My.Resources.NotAlwaysOnTop
        End If

        If My.Settings.bAutoSwitchMaps = True Then
            frmMap.appendData("Auto Switch Maps, On" & vbCrLf)
            frmMap.cmdAutoSwitchMap.Image = My.Resources.AutoMapSwitch
        Else
            frmMap.appendData("Auto Switch Maps, Off" & vbCrLf)
            frmMap.cmdAutoSwitchMap.Image = My.Resources.ManualMapSwitch
        End If

        If My.Settings.bMapDataVisible = True Then
            frmMap.splitContainer1.Panel2Collapsed = False 'show
            frmMap.cmdMapData.Image = My.Resources.ShowMapData
        Else
            frmMap.splitContainer1.Panel2Collapsed = True 'hide
            frmMap.cmdMapData.Image = My.Resources.HideMapData
        End If

        frmMap.Opacity = My.Settings.dLastOpacity 'set the map windows opacity
        frmMap.cboOpacity.SelectedIndex = ((My.Settings.dLastOpacity * 10) - 1) 'set combo box to last opacity used

        frmMap.cboMapMenu.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        frmMap.cboMapMenu.DropDownStyle = ComboBoxStyle.DropDown
        frmMap.cboMapMenu.AutoCompleteSource = AutoCompleteSource.ListItems

        frmMap.cboOpacity.AutoCompleteMode = AutoCompleteMode.Suggest
        frmMap.cboOpacity.DropDownStyle = ComboBoxStyle.DropDownList
        frmMap.cboOpacity.AutoCompleteSource = AutoCompleteSource.ListItems

    End Sub

#Region "UI Control Methods"
    Public Sub mapDataVisible()
        If My.Settings.bMapDataVisible = False Then
            My.Settings.bMapDataVisible = True 'show
            frmMap.splitContainer1.Panel2Collapsed = False 'show
            frmMap.cmdMapData.Image = My.Resources.ShowMapData 'show
        Else
            My.Settings.bMapDataVisible = False 'hide
            frmMap.splitContainer1.Panel2Collapsed = True 'hide
            frmMap.cmdMapData.Image = My.Resources.HideMapData 'hide
        End If
        My.Settings.Save()
    End Sub

    Public Sub alwaysOnTop()
        If My.Settings.bOnTop = False Then
            frmMap.TopMost = True
            My.Settings.bOnTop = True
            frmMap.appendData("Always On Top, On" & vbCrLf)
            frmMap.cmdOnTop.Image = My.Resources.AlwaysOnTop
        Else 'if it was true make it false
            frmMap.TopMost = False
            My.Settings.bOnTop = False
            frmMap.appendData("Always On Top, Off" & vbCrLf)
            frmMap.cmdOnTop.Image = My.Resources.NotAlwaysOnTop
        End If
        My.Settings.Save()
    End Sub

    Public Sub autoSwitchMaps()
        If My.Settings.bAutoSwitchMaps = False Then
            My.Settings.bAutoSwitchMaps = True
            frmMap.appendData("Auto Switch Maps, On" & vbCrLf)
            frmMap.cmdAutoSwitchMap.Image = My.Resources.AutoMapSwitch
        Else 'if it was true make it false
            My.Settings.bAutoSwitchMaps = False
            frmMap.appendData("Auto Switch Maps, Off" & vbCrLf)
            frmMap.cmdAutoSwitchMap.Image = My.Resources.ManualMapSwitch
        End If
        My.Settings.Save()
    End Sub

    Public Sub opacity()
        frmMap.Opacity = CInt(frmMap.cboOpacity.Text) / 100
        My.Settings.dLastOpacity = frmMap.Opacity
        My.Settings.Save()
    End Sub

    Public Sub wndLoc() 'saves the position and size of the window
        If frmMap.WindowState = FormWindowState.Normal Then
            My.Settings.drawPWndLoc = frmMap.Location
            My.Settings.drawSWndSize = frmMap.Size
            My.Settings.Save()
        End If
    End Sub
#End Region
End Module
