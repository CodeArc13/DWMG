<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMap
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMap))
        Me.toolStripMenu1 = New System.Windows.Forms.ToolStrip()
        Me.cmdSelectDir = New System.Windows.Forms.ToolStripButton()
        Me.toolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.cboMapMenu = New System.Windows.Forms.ToolStripComboBox()
        Me.cmdAutoSwitchMap = New System.Windows.Forms.ToolStripButton()
        Me.toolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.cmdMapData = New System.Windows.Forms.ToolStripButton()
        Me.cmdOnTop = New System.Windows.Forms.ToolStripButton()
        Me.cboOpacity = New System.Windows.Forms.ToolStripComboBox()
        Me.toolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.txtWp = New System.Windows.Forms.ToolStripTextBox()
        Me.cmdEnterWp = New System.Windows.Forms.ToolStripButton()
        Me.cmdRemLastWp = New System.Windows.Forms.ToolStripButton()
        Me.cmdRemAllWp = New System.Windows.Forms.ToolStripButton()
        Me.toolStripContainer1 = New System.Windows.Forms.ToolStripContainer()
        Me.splitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.picMapArea = New System.Windows.Forms.PictureBox()
        Me.txtMapData = New System.Windows.Forms.TextBox()
        Me.ofodOpenLogDir = New System.Windows.Forms.FolderBrowserDialog()
        Me.toolStripMenu1.SuspendLayout()
        Me.toolStripContainer1.ContentPanel.SuspendLayout()
        Me.toolStripContainer1.TopToolStripPanel.SuspendLayout()
        Me.toolStripContainer1.SuspendLayout()
        CType(Me.splitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitContainer1.Panel1.SuspendLayout()
        Me.splitContainer1.Panel2.SuspendLayout()
        Me.splitContainer1.SuspendLayout()
        CType(Me.picMapArea, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'toolStripMenu1
        '
        Me.toolStripMenu1.Dock = System.Windows.Forms.DockStyle.None
        Me.toolStripMenu1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.toolStripMenu1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.cmdSelectDir, Me.toolStripSeparator1, Me.cboMapMenu, Me.cmdAutoSwitchMap, Me.toolStripSeparator2, Me.cmdMapData, Me.cmdOnTop, Me.cboOpacity, Me.toolStripSeparator3, Me.txtWp, Me.cmdEnterWp, Me.cmdRemLastWp, Me.cmdRemAllWp})
        Me.toolStripMenu1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow
        Me.toolStripMenu1.Location = New System.Drawing.Point(3, 0)
        Me.toolStripMenu1.Name = "toolStripMenu1"
        Me.toolStripMenu1.Size = New System.Drawing.Size(462, 25)
        Me.toolStripMenu1.TabIndex = 29
        Me.toolStripMenu1.Text = "ToolStrip1"
        '
        'cmdSelectDir
        '
        Me.cmdSelectDir.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.cmdSelectDir.Image = CType(resources.GetObject("cmdSelectDir.Image"), System.Drawing.Image)
        Me.cmdSelectDir.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.cmdSelectDir.Name = "cmdSelectDir"
        Me.cmdSelectDir.Size = New System.Drawing.Size(23, 22)
        Me.cmdSelectDir.Text = "&Open"
        Me.cmdSelectDir.ToolTipText = "Set EQ Directory"
        '
        'toolStripSeparator1
        '
        Me.toolStripSeparator1.Name = "toolStripSeparator1"
        Me.toolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'cboMapMenu
        '
        Me.cboMapMenu.Name = "cboMapMenu"
        Me.cboMapMenu.Size = New System.Drawing.Size(121, 25)
        Me.cboMapMenu.Text = "Select a Map"
        Me.cboMapMenu.ToolTipText = "Select a Map"
        '
        'cmdAutoSwitchMap
        '
        Me.cmdAutoSwitchMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.cmdAutoSwitchMap.Image = Global.DWMGbeta010.My.Resources.Resources.AutoMapSwitch
        Me.cmdAutoSwitchMap.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.cmdAutoSwitchMap.Name = "cmdAutoSwitchMap"
        Me.cmdAutoSwitchMap.Size = New System.Drawing.Size(23, 22)
        Me.cmdAutoSwitchMap.Text = "cmdAutoSwitchMap"
        Me.cmdAutoSwitchMap.ToolTipText = "Enable/Disable Automatic Map Switch on zone and /who"
        '
        'toolStripSeparator2
        '
        Me.toolStripSeparator2.Name = "toolStripSeparator2"
        Me.toolStripSeparator2.Size = New System.Drawing.Size(6, 25)
        '
        'cmdMapData
        '
        Me.cmdMapData.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.cmdMapData.Image = Global.DWMGbeta010.My.Resources.Resources.ShowMapData
        Me.cmdMapData.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.cmdMapData.Name = "cmdMapData"
        Me.cmdMapData.Size = New System.Drawing.Size(23, 22)
        Me.cmdMapData.Text = "btnMapData"
        Me.cmdMapData.ToolTipText = "Show/Hide Map/Debug Data"
        '
        'cmdOnTop
        '
        Me.cmdOnTop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.cmdOnTop.Image = Global.DWMGbeta010.My.Resources.Resources.AlwaysOnTop
        Me.cmdOnTop.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.cmdOnTop.Name = "cmdOnTop"
        Me.cmdOnTop.Size = New System.Drawing.Size(23, 22)
        Me.cmdOnTop.Text = "btnOnTop"
        Me.cmdOnTop.ToolTipText = "Enable/Disable Always On Top"
        '
        'cboOpacity
        '
        Me.cboOpacity.AutoSize = False
        Me.cboOpacity.DropDownWidth = 30
        Me.cboOpacity.Items.AddRange(New Object() {"10", "20", "30", "40", "50", "60", "70", "80", "90", "100"})
        Me.cboOpacity.Name = "cboOpacity"
        Me.cboOpacity.Size = New System.Drawing.Size(44, 23)
        Me.cboOpacity.ToolTipText = "Set Map Transparency"
        '
        'toolStripSeparator3
        '
        Me.toolStripSeparator3.Name = "toolStripSeparator3"
        Me.toolStripSeparator3.Size = New System.Drawing.Size(6, 25)
        '
        'txtWp
        '
        Me.txtWp.Enabled = False
        Me.txtWp.MaxLength = 19
        Me.txtWp.Name = "txtWp"
        Me.txtWp.Size = New System.Drawing.Size(109, 25)
        Me.txtWp.Text = "Not yet available"
        Me.txtWp.ToolTipText = "Enter Waypoint Y X as e.g ""-123.12 123.12"" (seperated by a space)"
        '
        'cmdEnterWp
        '
        Me.cmdEnterWp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.cmdEnterWp.Enabled = False
        Me.cmdEnterWp.Image = CType(resources.GetObject("cmdEnterWp.Image"), System.Drawing.Image)
        Me.cmdEnterWp.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.cmdEnterWp.Name = "cmdEnterWp"
        Me.cmdEnterWp.Size = New System.Drawing.Size(23, 22)
        Me.cmdEnterWp.Text = "ToolStripButton1"
        Me.cmdEnterWp.ToolTipText = "Type loc and click to enter waypoint"
        '
        'cmdRemLastWp
        '
        Me.cmdRemLastWp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.cmdRemLastWp.Enabled = False
        Me.cmdRemLastWp.Image = CType(resources.GetObject("cmdRemLastWp.Image"), System.Drawing.Image)
        Me.cmdRemLastWp.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.cmdRemLastWp.Name = "cmdRemLastWp"
        Me.cmdRemLastWp.Size = New System.Drawing.Size(23, 22)
        Me.cmdRemLastWp.Text = "ToolStripButton1"
        Me.cmdRemLastWp.ToolTipText = "Remove last waypoint added to this map"
        '
        'cmdRemAllWp
        '
        Me.cmdRemAllWp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.cmdRemAllWp.Enabled = False
        Me.cmdRemAllWp.Image = CType(resources.GetObject("cmdRemAllWp.Image"), System.Drawing.Image)
        Me.cmdRemAllWp.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.cmdRemAllWp.Name = "cmdRemAllWp"
        Me.cmdRemAllWp.Size = New System.Drawing.Size(23, 22)
        Me.cmdRemAllWp.Text = "ToolStripButton1"
        Me.cmdRemAllWp.ToolTipText = "Remove all waypoints from this map"
        '
        'toolStripContainer1
        '
        Me.toolStripContainer1.BottomToolStripPanelVisible = False
        '
        'toolStripContainer1.ContentPanel
        '
        Me.toolStripContainer1.ContentPanel.Controls.Add(Me.splitContainer1)
        Me.toolStripContainer1.ContentPanel.Size = New System.Drawing.Size(651, 585)
        Me.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.toolStripContainer1.LeftToolStripPanelVisible = False
        Me.toolStripContainer1.Location = New System.Drawing.Point(0, 0)
        Me.toolStripContainer1.Name = "toolStripContainer1"
        Me.toolStripContainer1.RightToolStripPanelVisible = False
        Me.toolStripContainer1.Size = New System.Drawing.Size(651, 610)
        Me.toolStripContainer1.TabIndex = 31
        Me.toolStripContainer1.Text = "ToolStripContainer1"
        '
        'toolStripContainer1.TopToolStripPanel
        '
        Me.toolStripContainer1.TopToolStripPanel.Controls.Add(Me.toolStripMenu1)
        '
        'splitContainer1
        '
        Me.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.splitContainer1.Name = "splitContainer1"
        Me.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'splitContainer1.Panel1
        '
        Me.splitContainer1.Panel1.Controls.Add(Me.picMapArea)
        '
        'splitContainer1.Panel2
        '
        Me.splitContainer1.Panel2.Controls.Add(Me.txtMapData)
        Me.splitContainer1.Size = New System.Drawing.Size(651, 585)
        Me.splitContainer1.SplitterDistance = 442
        Me.splitContainer1.TabIndex = 8
        Me.splitContainer1.TabStop = False
        '
        'picMapArea
        '
        Me.picMapArea.Dock = System.Windows.Forms.DockStyle.Fill
        Me.picMapArea.InitialImage = Nothing
        Me.picMapArea.Location = New System.Drawing.Point(0, 0)
        Me.picMapArea.Name = "picMapArea"
        Me.picMapArea.Size = New System.Drawing.Size(647, 438)
        Me.picMapArea.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.picMapArea.TabIndex = 11
        Me.picMapArea.TabStop = False
        '
        'txtMapData
        '
        Me.txtMapData.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtMapData.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtMapData.Location = New System.Drawing.Point(0, 0)
        Me.txtMapData.Multiline = True
        Me.txtMapData.Name = "txtMapData"
        Me.txtMapData.ReadOnly = True
        Me.txtMapData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtMapData.Size = New System.Drawing.Size(647, 135)
        Me.txtMapData.TabIndex = 0
        '
        'frmMap
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(11.0!, 22.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(651, 610)
        Me.Controls.Add(Me.toolStripContainer1)
        Me.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HelpButton = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(6, 5, 6, 5)
        Me.Name = "frmMap"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Dude, Where's My Guild? (Beta 0.010 Non-Networked) by Tobeunce"
        Me.toolStripMenu1.ResumeLayout(False)
        Me.toolStripMenu1.PerformLayout()
        Me.toolStripContainer1.ContentPanel.ResumeLayout(False)
        Me.toolStripContainer1.TopToolStripPanel.ResumeLayout(False)
        Me.toolStripContainer1.TopToolStripPanel.PerformLayout()
        Me.toolStripContainer1.ResumeLayout(False)
        Me.toolStripContainer1.PerformLayout()
        Me.splitContainer1.Panel1.ResumeLayout(False)
        Me.splitContainer1.Panel2.ResumeLayout(False)
        Me.splitContainer1.Panel2.PerformLayout()
        CType(Me.splitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitContainer1.ResumeLayout(False)
        CType(Me.picMapArea, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents splitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents toolStripContainer1 As System.Windows.Forms.ToolStripContainer
    Friend WithEvents toolStripMenu1 As System.Windows.Forms.ToolStrip
    Friend WithEvents toolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents toolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents toolStripSeparator3 As System.Windows.Forms.ToolStripSeparator

    Friend WithEvents cboMapMenu As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents cboOpacity As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents picMapArea As System.Windows.Forms.PictureBox
    Friend WithEvents txtWp As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents txtMapData As System.Windows.Forms.TextBox

    Friend WithEvents cmdSelectDir As System.Windows.Forms.ToolStripButton
    Friend WithEvents cmdAutoSwitchMap As System.Windows.Forms.ToolStripButton
    Friend WithEvents cmdMapData As System.Windows.Forms.ToolStripButton
    Friend WithEvents cmdOnTop As System.Windows.Forms.ToolStripButton
    Friend WithEvents cmdEnterWp As System.Windows.Forms.ToolStripButton
    Friend WithEvents cmdRemAllWp As System.Windows.Forms.ToolStripButton
    Friend WithEvents cmdRemLastWp As System.Windows.Forms.ToolStripButton
    Friend WithEvents ofodOpenLogDir As System.Windows.Forms.FolderBrowserDialog

End Class
