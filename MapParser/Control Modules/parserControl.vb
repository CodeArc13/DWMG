Imports System.ComponentModel
Imports System.IO
Imports System.Threading
Imports System.Text.RegularExpressions
Imports System.Windows.Forms.DataVisualization.Charting

Module parserControl

    'declare background worker thread and use from winthin this module
    Private WithEvents bwParser As BackgroundWorker
    'dispatch timer to wait until bwParser thread is no longer busy and restart it
    Private WithEvents disptimerRestartParser As New Windows.Threading.DispatcherTimer
    Private sLogPathForTimer As String 'place to parse in log path when timer is calling bwParser

    '[Thu Feb 14 12:36:32 2013] Your Location is 4027.73, -2795.26, -56.74
    Private regrLoc As Regex = New Regex("^(\D{4}\s\D{3}\s\d{2}\s\d{2}:\d{2}:\d{2}\s\d{4}] Your Location is)")
    Private regrZone As Regex = New Regex("^(\D{4}\s\D{3}\s\d{2}\s\d{2}:\d{2}:\d{2}\s\d{4}] You have entered)")
    Private regrWho As Regex = New Regex("^(\D{4}\s\D{3}\s\d{2}\s\d{2}:\d{2}:\d{2}\s\d{4}] There \b(are|is)\b [0-9]+ \b(player|players)\b in)")
    Private regrSlain As Regex = New Regex("^(\D{4}\s\D{3}\s\d{2}\s\d{2}:\d{2}:\d{2}\s\d{4}] You have been slain by)")

    Private bFirstPlot As Boolean = True 'when updateing firstplot to database the boolean value must be converted from True/False/-1/0 to 1/0 by subtracting it from 0 eg(0 - -1)
    Private bJustZoned As Boolean = False
    Private sZone As String = Nothing 'store current zone here, found by zoneing or /who (and reverse zone/who scan?))
    '^May need to clear these manually each time parser ends - needs to be in full scope as who zone name needs DB access to change this in progress changed

    Public Sub init() 'only call at DWMG loading
        bwParser = New BackgroundWorker
        With bwParser
            .WorkerReportsProgress = True
            .WorkerSupportsCancellation = True
        End With
        disptimerRestartParser.Interval = New TimeSpan(1) '100 nanoseconds
    End Sub

    Public Sub runWorkerAsync(ByVal sEQPath As String)
        If bwParser.IsBusy Then
            cancelAsync()
            sLogPathForTimer = sEQPath
            disptimerRestartParser.Start()
        Else 'if bwParser is not busy then
            bwParser.RunWorkerAsync(sEQPath)
        End If
    End Sub

    Public Sub cancelAsync()
        bwParser.CancelAsync()
    End Sub

    Private Sub restartParserTimer(ByVal sender As Object, ByVal e As EventArgs) Handles disptimerRestartParser.Tick
        If Not bwParser.IsBusy Then 'if bwParser is not busy then
            DirectCast(sender, Windows.Threading.DispatcherTimer).Stop()
            bwParser.RunWorkerAsync(sLogPathForTimer)
        End If
    End Sub

    Private Sub bwParser_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles bwParser.DoWork
        Dim dnfoLog As DirectoryInfo = New DirectoryInfo(e.Argument & "\Logs")
        Dim fnfoCurrLog As FileInfo = Nothing

        Dim sCharKey As String = Nothing 'store current Char key here after log validated so it does not need to be repeatedly chopped

        Dim objLastZoneFound As Object() 'last zone found in reverse search (0 is "zoned" or "who", 1 is zone/who name 
        Dim liLastReadLen As Long 'length of log on last read by parser
        Dim sLineIn As String 'line/String being read by parser

        Dim dblp3dCoords As New objDblPoint3D

        'Add /loc data, charname/server from log, firstPlot, 
        'Dim sCharName As String
        'Dim sServerName As String
        'Dim sCharKey As String
        'Dim sZoneName As String
        'Dim lastIndex = 0
        'Dim lastModlen As Long 'length of log last time EQ changed the file
        'Do all log location My.settings outside of here and take Dir as param!     
        'My.Settings.sLogLocation = Path.GetDirectoryName(sLogDir)
        'My.Settings.Save()
        'Extract charName and serverName from log in use and set properties -Will now be set by checking for last log used with time modified
        'sCharName = logChopper.chopChar(_logPath)
        '_serverName = logChopper.chopServer(_logPath)
        '_charKey = logChopper.chopKey(_logPath)
        '_zoneName = My.Settings.LastZone  'set by scanning backwards though log and finding last /who or zone, if none set to a default.
        'lastIndex = strmrLog.BaseStream.Length 'end up setting the last index to the last place in the file 'changed to length from position to avoid the need of looping to end of file (no need for first pass)

        If Directory.Exists(e.Argument & "\Logs") = False Then
            bwParser.ReportProgress(0) 'no log folder, check logs are enabled, stop parser in progresschanged.

        Else 'Log folder exists so check selected log file is valid before moving on to parseing stage

            'set latest log to use before running parser main loop
            fnfoCurrLog = lastLogAccessed(dnfoLog)

            'test log is valid
            If validateLog(fnfoCurrLog.FullName) = False Then
                bwParser.ReportProgress(1) 'invalid log, stop parser in progresschanged.
            Else 'valid log
                My.Settings.sEQLocation = e.Argument 'log is good save location in settings (Re-assign var to confirm)
                My.Settings.Save()
                sCharKey = chopKey(fnfoCurrLog.Name) 'set current charCkey here
                bwParser.ReportProgress(2, {sCharKey}) 'log loaded

                'start parseing valid log

                'Read log in reverse to get the last zone or who message to work out what the last zone the char was in for loading correct map
                'Dim sMsg As String = "Finding character's last zone...."
                bwParser.ReportProgress(3)
                objLastZoneFound = findLastZone(fnfoCurrLog, False) 'do not performa who search for intital log loading, ask user to perform who if no zone found
                Select Case objLastZoneFound(0)
                    Case "inUse"
                        bwParser.ReportProgress(12, {sCharKey}) 'EQ spamming log! please perform a /who to set map manually
                    Case "zoned"
                        bwParser.ReportProgress(4, {sCharKey, objLastZoneFound(1)})
                    Case "Unknown"
                        bwParser.ReportProgress(5, {sCharKey}) 'no zone found (Unknown is no map in log file)
                End Select

                'may need to set strmrLog's BaseStream.Position before commencing loop - will need to declear stream outside loop
                liLastReadLen = fnfoCurrLog.Length 'set last read length to be the very end of the log
                'Main Loop
                While bwParser.CancellationPending = False
                    'update log info to get latest size of log
                    fnfoCurrLog = New FileInfo(fnfoCurrLog.FullName)
                    'check if the last accessed log if different from the one currently being parsed
                    If lastLogAccessed(dnfoLog).Name <> fnfoCurrLog.Name Then
                        bwParser.ReportProgress(6, {sCharKey, dblp3dCoords}) 'log changed, send charkey of old log, stop parser, send last coords
                        Exit While
                    Else
                        'pause a tiny bit before scanning log
                        Thread.Sleep(100)

                        'Realtime Parse
                        Try
                            'May need to re-initilise fnfoCurrLog to get correct length.
                            While fnfoCurrLog.Length <> liLastReadLen And bwParser.CancellationPending = False 'compare current length of file to the length on last reading

                                Dim strmrLog As New StreamReader(fnfoCurrLog.FullName) 'reference to Log
                                'sets the position of the last place the file was read from
                                strmrLog.BaseStream.Seek(liLastReadLen, IO.SeekOrigin.Current)

                                'parse file to end or until parser canceled
                                While strmrLog.Peek <> -1 And bwParser.CancellationPending = False
                                    'read in next line of log
                                    sLineIn = strmrLog.ReadLine

                                    'detect loc
                                    If regrLoc.Match(sLineIn).Success Then
                                        'Me.BackgroundWorker1.ReportProgress(0, "Loc Match Found")
                                        '[Thu Feb 14 12:36:32 2013] Your Location is 4027.73, -2795.26, -56.74
                                        'String manipulation and casting to float to extract x,y locs
                                        Dim sCoords() = Split(Mid(sLineIn, 45), ",") 'split coords into string array
                                        'dblp3dCoords.copyPtoQ() 'update last location with current location.
                                        dblp3dCoords.q.X = dblp3dCoords.p.X
                                        dblp3dCoords.q.Y = dblp3dCoords.p.Y
                                        dblp3dCoords.q.Z = dblp3dCoords.p.Z
                                        dblp3dCoords.p.X = CSng(sCoords(1)) 'x update new current loc's
                                        dblp3dCoords.p.Y = CSng(sCoords(0)) 'y
                                        dblp3dCoords.p.Z = CSng(sCoords(2)) 'z

                                        'detect if plyaer has moved or not to prevent duplicate locs being sent
                                        Dim point3dMotion As New Point3D
                                        point3dMotion.X = dblp3dCoords.p.X - dblp3dCoords.q.X
                                        point3dMotion.Y = dblp3dCoords.p.Y - dblp3dCoords.q.Y
                                        'Z to be implemented here?

                                        Dim sglMotion As Single = point3dMotion.X + point3dMotion.Y 'result 0 if not moveing in any direction
                                        If sglMotion <> 0 Then 'if moveing then motion will be anything other than 0, only update when moveing
                                            bwParser.ReportProgress(7, {sCharKey, dblp3dCoords}) 'char moved
                                        End If

                                    ElseIf regrWho.Match(sLineIn).Success Then
                                        If splitWhoName(sLineIn) <> "EverQuest" Then 'prevents /who commands which have global results from updating DWMG
                                            bwParser.ReportProgress(8, {sCharKey, splitWhoName(sLineIn)}) 'player issued an in zone /who command
                                        End If

                                    ElseIf regrZone.Match(sLineIn).Success Then
                                        '[Fri May 03 20:45:56 2013] You have entered Qeynos Hills.
                                        bwParser.ReportProgress(9, {sCharKey, splitZoneName(sLineIn)}) 'char zoned

                                    ElseIf regrSlain.Match(sLineIn).Success Then
                                        bwParser.ReportProgress(3, sCharKey & " died! Finding character's last known zone and location....") 'should never produce log in use error as char is zoning? hopefully! Might only happen if character zones exceptionally fast (fast PC/Internet)
                                        objLastZoneFound = findLastZone(fnfoCurrLog, True)
                                        If objLastZoneFound(0) = "zoned" Then 'tell database what kind of zone name was found, who or zone
                                            bwParser.ReportProgress(10, {sCharKey, objLastZoneFound(1), findLastKnownLoc(fnfoCurrLog)}) 'char died, found zoned zone name
                                        ElseIf objLastZoneFound(0) = "who" Then
                                            bwParser.ReportProgress(11, {sCharKey, objLastZoneFound(1), findLastKnownLoc(fnfoCurrLog)}) 'char died, found who zone name
                                        End If
                                    End If

                                    liLastReadLen = strmrLog.BaseStream.Position 'update last read length

                                End While

                                strmrLog.Close()
                                strmrLog.Dispose()
                            End While 'End of file parse loop
                        Catch ex As IO.IOException
                            'IF AN IO EXCEPTION IS THROWN, WE COULD NOT GET EXCLUSIVE ACCESS TO THE FILE
                        End Try

                    End If

                End While
                'e.Result = _charKey 'for removeing the objPlayer from the dictionary (Leave to show e.Result is an outputable value, used in bwParser_RunWorkerCompleted)

            End If

        End If
        bFirstPlot = True 'reset variables for new log' probably not needed
        'sZone = Nothing
    End Sub

    Private Sub bwParser_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles bwParser.ProgressChanged
        'e.UserState - 0 = sCharKey

        Select Case e.ProgressPercentage

            Case 0 'no log folder
                frmMap.appendData("Log directory does not exist, check logs are enabled in EQ" & vbCrLf)
                cancelAsync()
                frmMap.selectDir()

            Case 1 'invalid log (no logs)
                frmMap.appendData("No logs in log directory or loaded log is invalid, check logs are enabled in EQ" & vbCrLf)
                cancelAsync()
                frmMap.selectDir()

            Case 2 'initial log loaded 'update/insert query (Player Database)
                bFirstPlot = True 'trying this
                'CHARONLINE SQL Query to update char on database to online or add empty record with char name and online status and first plot - (Player Database) (DO NOT set first plot to true)
                dBaseControl.charOnOffline(e.UserState(0), 1) 'bConnected = True for charOnline

                'NOT HERE > if a map for the zone the char is in exists switch to it if auto switch maps is set


                'update network - char is online

                frmMap.appendData("Log loaded " & e.UserState(0) & vbCrLf)

            Case 3 'informational message
                frmMap.appendData("Finding character's last known zone....") 'message string
                'Case 3 'initial who 'zone 'update/insert query (Player Database)
                '    'WHO SQL Query to get zone name from who name (Map Database)
                '    Dim sWhoZoneResult As String = dBaseControl.zoneNameQuery(e.UserState(1)) 'send in who name and get zone name from maps database
                '    If sWhoZoneResult = "noEntry" Then
                '        ''(Should be no longer needed->) FINDZONE SQL Query to find out current zone stored of char 'if zone matches then they have not zoned, no update of zone required (same behaviour as above with intra-zone port)
                '        If sWhoZoneResult <> sZone Then
                '            bFirstPlot = True
                '            'Select Case sWhoZoneResult
                '            '    Case Is = "noEntry" 'no entry on map database for requested map

                '            '    Case Else 'not "noEntry"
                '            sZone = sWhoZoneResult  'Need zone for below map change

                '            'ZONED SQL Query to SET firstPlot and new zone in local DB (zone name is the forign key used to link chars to maps) (Player Database) (DO SET first plot to true)
                '            dBaseControl.initialZone(e.UserState(0), sZone, bFirstPlot, True) 'temporarly set zone on db and network to Unknown so old locs wont be used to draw char on new map, set it when sending locs

                '            'update network - char has zoned (DO SET First plot on server db -by sending bFirstPlot from here) (DO NOT Set zone here must be unknown untill coords sent)


                '            'switch map gfx
                '            mapControl.switchMap(sZone)
                '            frmMap.appendData("Initial zone entered from who " & sZone & " fp " & bFirstPlot & " jz " & bJustZoned & vbCrLf) 'do not print this if zone matches local DB
                '            'End Select
                '        End If
                '    Else

                '    End If
               

            Case 4 'initial zone 'update/insert query (Player Database)
                'Applies to intra-zone port/death as both generate a zoneing
                bFirstPlot = True
                sZone = e.UserState(1) 'Need zone for below map change

                'attempt to remove possible old incorrect offline markers before updateing
                mapControl.remOldMarker(e.UserState(0), sZone)

                'ZONED SQL Query to SET firstPlot and new zone in local DB (zone name is the forign key used to link chars to maps) (Player Database) (DO SET first plot to true)
                dBaseControl.initialZone(e.UserState(0), sZone, bFirstPlot, True) 'bConnected = True

                'update network - char has zoned (DO SET First plot on server db -by sending bFirstPlot from here) (DO NOT Set zone here/or in network must be Unknown until coords sent)


                'switch map gfx
                frmMap.appendData("Initial zone entered " & sZone & " fp " & bFirstPlot & " jz " & bJustZoned & vbCrLf)
                mapControl.switchMap(sZone)

            Case 5 'no zone found, Unknown
                sZone = "Unknown"
                noZoneMsg(e.UserState(0))

            Case 6 'new log 'update/insert query (Player Database) - send out last location points known
                'CHAROFFLINE SQL Query to set last char as offline on local database -(Player Database) (dont set first plot, just pass it out incase char doesnt exist in database)
                dBaseControl.charOnOffline(e.UserState(0), False) 'bConnected = False'for going offline local DB is set to bConnected = False, do not need to send sZone here as that will only be for map gfx updates on client and network clients

                'update network - char went offline -(dont set first plot) -(this will also happen in netcom disconnected event on the server, NOT Client) - if local client disconects should set everyone else that was on the server to offline and local player online))
                '                               ^send sZone so it can be broadcast to other clients  for changeing their maps if selected
                'set chars marker to offline on which ever map they are on.
                mapControl.upsertPlayerLoc(e.UserState(0), e.UserState(1).p.X, e.UserState(1).p.Y, e.UserState(1).q.X, e.UserState(1).q.Y, sZone, bFirstPlot, bJustZoned, False) 'bConnected = True 'if locs are being sent the bConnected is 1

                cancelAsync() 'stop parser
                frmMap.appendData("New log loading..." & vbCrLf)
                runWorkerAsync(My.Settings.sEQLocation) 'restart parser

            Case 7 'loc 'update/insert query (Player Database)
                If sZone <> "Unknown" Then 'prevents plotting when no zone is found in log
                    bJustZoned = False 'set before updateing dicts/DB/network, so on first plot in the zone just zoned is no longer True
                    'LOCUPDATE SQL Query update the database with the loc (Player Database)

                    'draw local player on current map if locs zone matches, if first plot = True draw circle, then set first plot to 0


                    'update network - char moved
                    'set bFirstPlot to false after draw and database updates here so on next loc update it will produce a pointer marker on the map
                    mapControl.upsertPlayerLoc(e.UserState(0), e.UserState(1).p.X, e.UserState(1).p.Y, e.UserState(1).q.X, e.UserState(1).q.Y, sZone, bFirstPlot, bJustZoned, True) 'bConnected = True 'if locs are being sent the bConnected is 1
                    frmMap.appendData("P ch " & e.UserState(0) & " px " & e.UserState(1).p.X & " py " & e.UserState(1).p.Y & _
                                                " qx " & e.UserState(1).q.X & " qy " & e.UserState(1).q.Y & " zn " & sZone & " fp " & bFirstPlot & " jz " & bJustZoned & " cn True" & vbCrLf)  'bConnected = True 'just for the purpose of neat printing
                    dBaseControl.locUpdate(e.UserState(0), e.UserState(1).p.X, e.UserState(1).p.Y, e.UserState(1).q.X, e.UserState(1).q.Y, sZone, bFirstPlot, bJustZoned, True) 'bConnected = True

                    bFirstPlot = False

                Else 'is Unknown
                    noZoneMsg(e.UserState(0)) 'no zone in log for this character perform /who
                End If

            Case 8 'who zone 'update/insert query (Player Database)
                'WHO SQL Query to get zone name from who name (Map Database)
                'Dim sWhoZoneResult As String = dBaseControl.zoneNameQuery(e.UserState(1)) 'send in who name and get zone name from maps database
                Dim sWhoZoneResult = mapControl.getZoneFromWho(e.UserState(1))
                ''(Should be no longer needed->) FINDZONE SQL Query to find out current zone stored of char 'if zone matches then they have not zoned, no update of zone required (same behaviour as above with intra-zone port)

                'update database, map object dictionary and network
                If sWhoZoneResult <> sZone Then
                    bFirstPlot = True
                    bJustZoned = False
                    sZone = sWhoZoneResult 'Need zone for below map change
                    'ZONED SQL Query to SET firstPlot and new zone in local DB (zone name is the forign key used to link chars to maps) (Player Database) (DO SET first plot to true)
                    dBaseControl.zoned(e.UserState(0), sZone, bFirstPlot, bJustZoned, True) 'temporarly set zone on db and network to Unknown so old locs wont be used to draw char on new map, set it when sending locs

                    'update network - char has zoned (DO SET First plot on server db -by sending bFirstPlot from here) (DO NOT Set zone here/or in network must be Unknown until coords sent)

                    'remove character from last maps dictionary and redraw map
                    mapControl.remFromZone(e.UserState(0))

                    If sZone = "Unknown" Then
                        noZoneMsg(e.UserState(0))
                    End If
                    frmMap.appendData("Zone from who " & sZone & " fp " & bFirstPlot & " jz " & bJustZoned & vbCrLf) 'do not print this if zone matches local DB
                End If

                'switch map gfx
                If My.Settings.bAutoSwitchMaps Then
                    mapControl.switchMap(sWhoZoneResult) 'forces a map switch, switchMap sub works out if map is in the map dictionary or not.
                End If

            Case 9 'zone 'update/insert query (Player Database)
                'Applies to intra-zone port/death as both generate a zoneing
                bFirstPlot = True
                bJustZoned = True
                sZone = e.UserState(1) 'Need zone for below map change
                'ZONED SQL Query to SET firstPlot and new zone in local DB (zone name is the forign key used to link chars to maps) (Player Database) (DO SET first plot to true)
                dBaseControl.zoned(e.UserState(0), sZone, bFirstPlot, bJustZoned, True) 'temporarly set zone on db and network to Unknown so old locs wont be used to draw char on new map, set it when sending locs
                'update network - char has zoned (DO SET First plot on server db -by sending bFirstPlot from here) (DO NOT Set zone here/or in network must be Unknown until coords sent)

                'remove character from last maps dictionary and redraw map
                mapControl.remFromZone(e.UserState(0))

                'switch map gfx
                If My.Settings.bAutoSwitchMaps Then
                    mapControl.switchMap(sZone)
                End If
                frmMap.appendData("Zone entered " & sZone & " fp " & bFirstPlot & vbCrLf)

            Case 10 'death 'insert only query with or without /loc (Waypoint Database)
                'found zoned zone name 'update waypoint data base and use char_key as foreign key
                If e.UserState(1)(0) = "loc" Then
                    'CORPSEWP SQL Query set a corpse waypoint at last known loc on the current zone (Waypoint Database)

                    'DO NOT Set sZone here as this is just for waypoints not char location.
                    'set waypoint on map gfx if it is the currently selected
                    'update network - char died, send corpse waypoint and zone

                    frmMap.appendData(e.UserState(0) & " Died in " & e.UserState(1) & " last known location X " & e.UserState(2)(1).X & " Y " & e.UserState(2)(1).Y & vbCrLf)
                Else ' = "noLoc"
                    'update network - char died, send zone (No waypoint as no loc NO SQL)
                    frmMap.appendData(e.UserState(0) & " Died in " & e.UserState(1) & " location unknown!" & vbCrLf) 'print zone name only
                End If

            Case 11 'death 'insert only query with or without /loc (Waypoint Database)
                'found who zone name 'update waypoint database and use char_key as foreign key
                'WHO SQL Query to get zone name from who name (Map Database)
                'Dim sWhoZoneResult As String = dBaseControl.zoneNameQuery(e.UserState(1)) 'send in who name and get zone name from maps database
                Dim sWhoZoneResult = mapControl.getZoneFromWho(e.UserState(1))
                If e.UserState(1)(0) = "loc" Then
                    'CORPSEWP SQL Query set a corpse waypoint at last known loc on the current zone (Waypoint Database)

                    'DO NOT Set sZone here as this is just for waypoints not char location.
                    'set waypoint on map gfx if it is the currently selected
                    'update network - char died send corpse waypoint and zone (Waypoint Database) 

                    frmMap.appendData(e.UserState(0) & " Died in " & sWhoZoneResult & " last known location X " & e.UserState(2)(1).X & " Y " & e.UserState(2)(1).Y & vbCrLf)
                Else ' = "noLoc"
                    'update network - char died, send zone (No waypoint as no loc NO SQL)
                    frmMap.appendData(e.UserState(0) & " Died in " & sWhoZoneResult & " location unknown!" & vbCrLf) 'print zone name only
                End If

            Case 12 'log in use by EQ (lots of spam, battles, EC Auction spam)
                sZone = "Unknown"
                frmMap.appendData(e.UserState(0) & "'s log is in too much use by EQ. To get their last zone, please perform a /who to set the map manually." & vbCrLf)
        End Select



        'Client.SendData(Client.Convert2Ascii(1 & ", " & update(0) & ", " & update(3).p.X & ", " & update(3).p.Y & ", " & update(3).q.X & ", " & update(3).q.Y & ", " & firstPlotInt & ", " & plyrCurrZn & ", " & 1))

        'Case 2 'zone 'if lineIn is not a loc, then it is a zone (change map on zoneing here)
        '    updateFirstPlotOnNextCoords = True
        '    'update network with new zone here?********************
        '    'remove player from old map object and move to new one, check old map exists first
        '    If mapDict.ContainsKey(plyDict.Item(update(0)).currentZone) Then
        '        mapDict.Item(plyDict.Item(update(0)).currentZone).remPlayer(update(0)) 'remove from selected objMap
        '    End If
        '    plyDict.Item(update(0)).currentZone = update(2) 'update current plyObj with zoneName (proper name) of current zone
        '    If mapDict.ContainsKey(update(2)) Then 'check to see if new map exists before moveing to it!
        '        mapDict.Item(update(2)).movPlayer(plyDict.Item(update(0))) 'add player to newely selected objMap
        '    End If
        '    tbMapData.AppendText(update(0) & " zoned to " & update(2) & vbCrLf) 'display map name regardless of weather auto switching enabled
        '    'Tell server player has moved to new zone but do not update server DB just broadcast
        '    Client.SendData(Client.Convert2Ascii(2 & ", " & update(2))) 'send 2 for zoning tpye message, no ID required as will using NetComm ID

        '    'change map gfx to newely entered zone if auto switch maps is set
        '    If autoSwitchMaps = True Then 'actually change map if enabled else fresh current map to pbxMapArea
        '        'currentMapMenuIndex = mapIndex.lookUpIndex(parser.zoneName)
        '        If mapDict.ContainsKey(update(2)) = True Then 'check if newely entered map exists in mapDict, before attempting to display it
        '            cbxMapMenu.SelectedIndex = mapDict.Item(update(2)).menuIndex 'this will call cbxMapMenu_selectedindexchanged which will do the map refresh
        '        Else
        '            tbMapData.AppendText("There is currently no map for this zone." & vbCrLf)
        '        End If
        '        'refreshMap() 'delegate <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< REMEMBER FOR MAP GFX
        '    Else 'if auto switch map not enabled then refresh current map which will make player marker disappear
        '        pbxMapArea.Refresh()
        '    End If

        'Case 3 'player died
        '    updateFirstPlotOnNextCoords = True
        '    'Client.SendData(Client.Convert2Ascii(3 & ", " & update(3).p.X & ", " & update(3).p.Y))

        'Case 4 'New log in use (player camped to new character)
        '    tbMapData.AppendText("New log in use " & update(0) & vbCrLf)

    End Sub

    Private Sub bwParser_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles bwParser.RunWorkerCompleted
        'MsgBox("completed")
        'e.Result can be used here to pass values from DoWork
    End Sub

#Region "Progress changed private functions"
    Private Sub noZoneMsg(sCharKey As String)
        frmMap.appendData("There was either no zone found in " & sCharKey & "'s log file, (please perform a /who or zone) or there is, as of yet, no map database entry for the zone. Plotting not possible." & vbCrLf)
    End Sub
#End Region

#Region "Do works's private functions"

    Private Function lastLogAccessed(ByRef dnfoLog As DirectoryInfo) As FileInfo
        Dim fnfoLastLog As FileInfo = Nothing
        Dim fnfoLogs() As FileInfo = dnfoLog.GetFiles("eqlog_*_*.txt")
        Dim dateLatestFound As New DateTime(0) 'place to store date last log scanned by this loop
        For i = 0 To fnfoLogs.Length - 1
            If fnfoLogs(i).LastWriteTime > dateLatestFound Then
                fnfoLastLog = fnfoLogs(i)
                dateLatestFound = fnfoLogs(i).LastWriteTime
            End If
        Next
        Return fnfoLastLog
    End Function

    Private Function validateLog(sLogPath As String) As Boolean
        Dim strmrFileCheck As New StreamReader(sLogPath)
        Dim regrValid As Regex = New Regex("^(\D{4}\s\D{3}\s\d{2}\s\d{2}:\d{2}:\d{2}\s\d{4}])")
        Dim regmFirstLine As Match = Nothing 'check if log is valid by reading the first line
        Try
            regmFirstLine = regrValid.Match(strmrFileCheck.ReadLine)
            strmrFileCheck.Close()
            strmrFileCheck.Dispose()
        Catch ex As IO.IOException
            MessageBox.Show("Log file in use")
        End Try
        Return regmFirstLine.Success
    End Function

    'need reverse log reading for instances where char dies with no loc and zone info.
    Private Function findLastKnownLoc(ByRef fnfoCurrLog As FileInfo) As Object()

        Dim point3dCoords As New Point3D

        Dim liBackReadStartPos As Long = fnfoCurrLog.Length
        frmMap.appendData("Finding characters last known location....")
        Do While liBackReadStartPos > 1 And bwParser.CancellationPending = False 'loop to start of file
            Dim objLineAndStart() = getNextLineBackwards(fnfoCurrLog.FullName, liBackReadStartPos)

            liBackReadStartPos = objLineAndStart(1)

            If regrLoc.Match(objLineAndStart(0)).Success Then
                Dim sCoords() = Split(Mid(objLineAndStart(0), 45), ",") 'split coords into string array
                point3dCoords.X = CSng(sCoords(1)) 'x update new current loc's
                point3dCoords.Y = CSng(sCoords(0)) 'y
                point3dCoords.Z = CSng(sCoords(2)) 'z
                Return {"loc", point3dCoords}
                Exit Do
            ElseIf regrZone.Match(objLineAndStart(0)).Success Then
                'set current zone var (this process will only happen when a new log is loaded or character is logged on)
                Return {"noloc"} 'returning noLoc found before last zone 
                Exit Do
            End If
        Loop
        Return {"noloc"} 'returning noLoc found before last zone (should never happen, but will generate same result as zone found)
    End Function

    Private Function findLastZone(ByRef fnfoCurrLog As FileInfo, bWhoSearch As Boolean) As Object()

        Dim liBackReadStartPos As Long = fnfoCurrLog.Length

        Do While liBackReadStartPos > 1 And bwParser.CancellationPending = False 'loop to start of file
            Dim objLineAndStart() = getNextLineBackwards(fnfoCurrLog.FullName, liBackReadStartPos)
            If objLineAndStart(0) <> "inUse" Then 'file not in use
                liBackReadStartPos = objLineAndStart(1)

                If bWhoSearch = True And regrWho.Match(objLineAndStart(0)).Success Then
                    'MsgBox("uyu")
                    Dim zone As String = Regex.Split(objLineAndStart(0), "\b(player|players)\b in ")(2)
                    zone = Split(zone, ".")(0)
                    'set current zone var
                    If zone <> "EverQuest" Then 'prevents /who commands which have global results from updating DWMG
                        Return {"who", zone} 'bwParser.ReportProgress(4, zone)
                        Exit Do
                    End If
                ElseIf regrZone.Match(objLineAndStart(0)).Success Then
                    'set current zone var (this process will only happen when a new log is loaded or character is logged on)
                    Return {"zoned", Split(Split(objLineAndStart(0), "have entered ")(1), ".")(0)} 'bwParser.ReportProgress(3, Split(Split(objLineAndStart(0), "have entered ")(1), ".")(0))
                    Exit Do
                End If
            Else
                Return {"inUse"} 'please perform a /who to switch zone manually
                Exit Do
            End If
        Loop
        Return {"Unknown"} 'no zone found in log 'at the moment this should cause an error but is unlikely to happen unless the log is in a strange state
    End Function

    Private Function getNextLineBackwards(sLogPath As String, liStartPos As Long) As Object() 'credit to 'Ronzan' for this http://www.xtremedotnettalk.com/showpost.php?p=354835&postcount=5
        Try
            Dim strmrBackLog As New StreamReader(sLogPath) ', System.Text.Encoding.Default)
            Dim sTemp As String = ""
            Dim sTempLine As String = ""
            Dim cTempBlock(0) As Char
            Dim byCharBuffer(0) As Byte
            Dim liLastLineStartPos As Long
            Dim liLastLineEndPos As Long

            'Goto lngStartPos, step back to previous line
            'Save     for end of line
            liLastLineEndPos = liStartPos - 2

            If liLastLineEndPos < 1 Then 'no can do, this would get us to     -1 in next statement
                Return {"eof"}
            End If

            'Loop to start of file (or where zone entered or who zone is found)
            'Nested Loop to find first vbLF reversed
            strmrBackLog.BaseStream.Position = liLastLineEndPos - 1
            strmrBackLog.BaseStream.Read(byCharBuffer, 0, 1)
            Do While Not ChrW(byCharBuffer(0)) = vbLf And strmrBackLog.BaseStream.Position > 1
                sTemp = sTemp & ChrW(byCharBuffer(0))
                strmrBackLog.BaseStream.Position = strmrBackLog.BaseStream.Position - 2
                strmrBackLog.BaseStream.Read(byCharBuffer, 0, 1)
            Loop
            'Save     for start of line
            liLastLineStartPos = strmrBackLog.BaseStream.Position

            'Clear variables
            cTempBlock = ""
            sTempLine = ""

            'Get the line from lngLastLineEndPos to lngLastLineStartPos
            ReDim cTempBlock(liLastLineEndPos - liLastLineStartPos)
            strmrBackLog.BaseStream.Position = liLastLineStartPos
            strmrBackLog.ReadBlock(cTempBlock, 0, liLastLineEndPos - liLastLineStartPos)
            sTempLine = cTempBlock

            strmrBackLog.Close()
            strmrBackLog.Dispose()

            Return {sTempLine, liLastLineStartPos}

        Catch ex As IOException 'file in use (lots of spam happening!!!) 'for death, this scan will occur during loading/zoneing so EQ SHOULD not be accessing the log.
            Return {"inUse"}
        End Try
    End Function

    Private Function splitZoneName(sLine As String) As String 'removes fullstop from end of zone name
        Dim sZone As String = Mid(sLine, 45) 'removes beginning of string
        Return Split(sZone, ".")(0)
    End Function 'used by parser (Do work)

    Private Function splitWhoName(sLine As String) As String 'removes fullstop from end of who name
        Dim sWho As String = Regex.Split(sLine, "\b(player|players)\b in ")(2)
        Return Split(sWho, ".")(0)
    End Function 'used by parser (Do work)

    Private Function logChop(ByVal log As String) As Array 'returns the string array of the path to the log
        logChop = Split(log, "_")
        Return logChop
    End Function

    Private Function chopChar(ByVal log) As String 'returns the character name from the log
        chopChar = logChop(log)(1)
        Return chopChar
    End Function

    Private Function chopServer(ByVal log) As String 'returns the character's server name from the log
        chopServer = Split(logChop(log)(2), ".")(0) 'remove .txt from server name
        Return chopServer
    End Function

    Private Function chopKey(ByVal log) As String
        chopKey = (Split(Split(log, "eqlog_")(1), ".")(0)) 'this will return "charName_serverName"
        Return chopKey
    End Function
#End Region

End Module
