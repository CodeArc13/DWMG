Imports System.IO
Imports System.Data.SQLite
Imports System.Text
Imports System.Windows.Forms.DataVisualization.Charting

'seperate BW thread for database access, with reportprogress retreiving the information?
Module dBaseControl
    'Create/Initilise New or existing database
    'Update Entries from parser and network
    'Read from map DB and Player DB (On map switching(zoning and user))
    'Delete all (user initiated) and re-create databases incase of errors

    Public Sub init()
        initPlayers()
        initMaps()
    End Sub

    'upsert player data querys
    'CHARONLINE/OFFLINE
    Public Sub charOnOffline(sCharKey As String, bConnected As Boolean)
        Dim strbUpdate As New StringBuilder
        Dim strbInsert As New StringBuilder
        strbUpdate.AppendFormat("UPDATE Players SET bConnected = '{0}' WHERE sCharKey = '{1}'", 0 - bConnected, sCharKey)
        strbInsert.AppendFormat("INSERT INTO Players(sCharKey, bConnected) VALUES ('{0}', '{1}')", sCharKey, 0 - bConnected)
        upSert("Data Source=dbPlayers.db3", strbUpdate.ToString, strbInsert.ToString)
    End Sub

    'CHAROFFLINE
    'Public Sub charOnOffline(sCharKey As String, bConnected As Boolean)
    '    Dim strbUpdate As New StringBuilder
    '    Dim strbInsert As New StringBuilder
    '    strbUpdate.AppendFormat("UPDATE Players SET bConnected = '{0}' WHERE sCharKey = '{1}'", 0 - bConnected, sCharKey)
    '    strbInsert.AppendFormat("INSERT INTO Players(sCharKey, bConnected) VALUES ('{0}', '{1}')", sCharKey, 0 - bConnected)
    '    upSert("Data Source=dbPlayers.db3", strbUpdate.ToString, strbInsert.ToString)
    'End Sub

    'INITIAL ZONE
    Public Sub initialZone(sCharKey As String, sZone As String, bFirstPlot As Boolean, bConnected As Boolean)
        sZone = escapeQuote(sZone)
        Dim strbUpdate As New StringBuilder
        Dim strbInsert As New StringBuilder
        strbUpdate.AppendFormat("UPDATE Players SET sZone = '{0}', bFirstPlot = '{1}', bConnected = '{2}' WHERE sCharKey = '{3}'", sZone, 0 - bFirstPlot, 0 - bConnected, sCharKey)
        strbInsert.AppendFormat("INSERT INTO Players(sCharKey, sZone, bFirstPlot, bConnected) VALUES ('{0}', '{1}', '{2}', '{3}')", sCharKey, sZone, 0 - bFirstPlot, 0 - bConnected)
        upSert("Data Source=dbPlayers.db3", strbUpdate.ToString, strbInsert.ToString)
    End Sub

    'ZONED
    Public Sub zoned(sCharKey As String, sZone As String, bFirstPlot As Boolean, bJustZoned As Boolean, bConnected As Boolean)
        sZone = escapeQuote(sZone)
        Dim strbUpdate As New StringBuilder
        Dim strbInsert As New StringBuilder
        strbUpdate.AppendFormat("UPDATE Players SET sZone = '{0}', bFirstPlot = '{1}', bJustZoned = '{2}', bConnected = '{3}' WHERE sCharKey = '{4}'", sZone, 0 - bFirstPlot, 0 - bJustZoned, 0 - bConnected, sCharKey)
        strbInsert.AppendFormat("INSERT INTO Players(sCharKey, sZone, bFirstPlot, bJustZoned, bConnected) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')", sCharKey, sZone, 0 - bFirstPlot, 0 - bJustZoned, 0 - bConnected)
        upSert("Data Source=dbPlayers.db3", strbUpdate.ToString, strbInsert.ToString)
    End Sub


    ''LOCUPDATE 'any locs sent means tht char is online so set bConnected = '1', no need to send in through parameters.
    'Public Sub locUpdate(sCharKey As String, dblCurLocX As Double, dblCurLocY As Double, dblLastLocX As Double, dblLastLocY As Double, bFirstPlot As Boolean, sZone As String)
    '    Dim strbUpdate As New StringBuilder
    '    Dim strbInsert As New StringBuilder
    '    strbUpdate.AppendFormat("UPDATE Players SET " & _
    '                            "dblCurLocX = '{0}', dblCurLocY = '{1}', dblLastLocX = '{2}', dblLastLocY = '{3}', bFirstPlot = '{4}', sZone = '{5}', bConnected = True WHERE sCharKey = '{6}'", _
    '                            dblCurLocX, dblCurLocY, dblLastLocX, dblLastLocY, 0 - bFirstPlot, sZone, sCharKey)
    '    strbInsert.AppendFormat("INSERT INTO Players(sCharKey, dblCurLocX, dblCurLocY, dblLastLocX, dblLastLocY, bFirstPlot, sZone, bConnected) " & _
    '                            "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '1')", _
    '                            sCharKey, dblCurLocX, dblCurLocY, dblLastLocX, dblLastLocY, 0 - bFirstPlot, sZone)
    '    upSert("Data Source=dbPlayers.db3", strbUpdate.ToString, strbInsert.ToString)

    '    plyrLineRead(sCharKey)
    'End Sub

    'LOCUPDATE 'any locs sent means tht char is online so set bConnected = '1', no need to send in through parameters.
    Public Sub locUpdate(sCharKey As String, sglCurLocX As Single, sglCurLocY As Single, sglLastLocX As Single, sglLastLocY As Single, sZone As String, bFirstPlot As Boolean, bJustZoned As Boolean, bConnected As Boolean)
        sZone = escapeQuote(sZone)
        'sZone = removeQuote(sZone)
        Dim strbUpdate As New StringBuilder
        strbUpdate.AppendFormat("UPDATE Players SET " & _
                                "sglCurLocX = '{0}', sglCurLocY = '{1}', sglLastLocX = '{2}', sglLastLocY = '{3}', bFirstPlot = '{4}', sZone = '{5}', bJustzoned = '{6}', bConnected = {7} WHERE sCharKey = '{8}'", _
                                sglCurLocX, sglCurLocY, sglLastLocX, sglLastLocY, 0 - bFirstPlot, sZone, 0 - bJustZoned, 0 - bConnected, sCharKey)
        update("Data Source=dbPlayers.db3", strbUpdate.ToString)

        plyrLineRead(sCharKey)
    End Sub

    'select player data querys
    'FINDZONE - currently unused
    'Public Function findZone() As String
    '    Return 0
    'End Function

    'select map data querys
    'WHOZONE
    'Public Function zoneNameQuery(sWhoName As String) As String
    '    sWhoName = escapeQuote(sWhoName)
    '    'sWhoName = removeQuote(sWhoName)
    '    zoneNameQuery = Nothing
    '    Dim SQLconnect As New SQLite.SQLiteConnection()
    '    Dim SQLcommand As SQLiteCommand
    '    SQLconnect.ConnectionString = "Data Source=dbMaps.db3"
    '    SQLconnect.Open()
    '    SQLcommand = SQLconnect.CreateCommand
    '    SQLcommand.CommandText = "SELECT sZone FROM Maps WHERE sWhoName = '" & sWhoName & "'"
    '    Dim SQLreader As SQLiteDataReader = SQLcommand.ExecuteReader()
    '    While SQLreader.Read()
    '        frmMap.appendData("Zone found: " & SQLreader(0) & vbCrLf)
    '        zoneNameQuery = SQLreader(0)
    '    End While
    '    SQLcommand.Dispose()
    '    SQLconnect.Close()
    '    If zoneNameQuery = Nothing Then
    '        Return "Unknown" 'no entry for map found on map database
    '    Else
    '        Return zoneNameQuery
    '    End If
    'End Function

    Public Function mapDataQuery(sZone As String) As String()
        sZone = escapeQuote(sZone)
        mapDataQuery = Nothing
        Dim SQLconnect As New SQLite.SQLiteConnection()
        Dim SQLcommand As SQLiteCommand
        SQLconnect.ConnectionString = "Data Source=dbMaps.db3"
        SQLconnect.Open()
        SQLcommand = SQLconnect.CreateCommand
        SQLcommand.CommandText = "SELECT * FROM Maps WHERE sZone = '" & sZone & "'"
        Dim SQLreader As SQLiteDataReader = SQLcommand.ExecuteReader()
        While SQLreader.Read()
            '               sFileName, sWhoName, sAlphaName, iEqGridSize, iMapGridSize, dblMapOffSetX, dblMapOffSetY
            mapDataQuery = {SQLreader(1), SQLreader(2), SQLreader(3), SQLreader(4), SQLreader(5), SQLreader(6), SQLreader(7)}
        End While
        SQLcommand.Dispose()
        SQLconnect.Close()
        Return mapDataQuery
    End Function

    Public Function initMapDict() As Dictionary(Of String, objMap)
        initMapDict = New Dictionary(Of String, objMap)
        Dim SQLconnect As New SQLite.SQLiteConnection()
        Dim SQLcommand As SQLiteCommand
        SQLconnect.ConnectionString = "Data Source=dbMaps.db3"
        SQLconnect.Open()
        SQLcommand = SQLconnect.CreateCommand
        SQLcommand.CommandText = "SELECT * FROM Maps"
        Dim SQLreader As SQLiteDataReader = SQLcommand.ExecuteReader()
        Dim iMenuIndex As Integer = 0 'for map menu index
        While SQLreader.Read()
            'may need to initilise map menu here (create accessor function in frm map or interface control.
            'check if map img exits if not skip, and report to consol on missing map
            'Dim sPath As String = Application.StartupPath & "\Maps\" & SQLreader(1)
            If File.Exists(Application.StartupPath & "\Maps\" & SQLreader(1)) Then
                '              sZone0, sFileName1, sWhoName2 (i) for menuindex, sAlphaName3, iEqGridSize4, iMapGridSize5, dblMapOffSetX6, dblMapOffSetY7
                initMapDict.Add(SQLreader(0), New objMap(SQLreader(0), SQLreader(1), SQLreader(2), SQLreader(3), SQLreader(4), SQLreader(5), New Point3D(SQLreader(6), SQLreader(7), 0)))
                frmMap.addtoMapMenu(SQLreader(3), iMenuIndex, SQLreader(0))
                iMenuIndex += 1
            Else 'if map image missing
                frmMap.appendData("Map image " & SQLreader(1) & " missing for " & SQLreader(0) & ". Check map is in Maps folder and file spelling." & vbCrLf)
            End If
        End While
        SQLcommand.Dispose()
        SQLconnect.Close()
        Return initMapDict
    End Function

    'insert waypoint query (use for corpses and generic wp or make one for each)
    'CORPSEWP
    Public Sub corpseWP()

    End Sub

    Private Sub plyrLineRead(sCharKey As String)
        Dim SQLconnect As New SQLite.SQLiteConnection()
        Dim SQLcommand As SQLiteCommand
        SQLconnect.ConnectionString = "Data Source=dbPlayers.db3"
        SQLconnect.Open()
        SQLcommand = SQLconnect.CreateCommand
        SQLcommand.CommandText = "SELECT * FROM Players WHERE sCharKey = '" & sCharKey & "'"
        Dim SQLreader As SQLiteDataReader = SQLcommand.ExecuteReader()
        While SQLreader.Read()
            frmMap.appendData("D ch " & SQLreader(0) & " px " & SQLreader(1) & " py " & SQLreader(2) & " qx " & SQLreader(3) & " qy " & SQLreader(4) & " zn " & SQLreader(5) & " fp " & SQLreader(6) & " jz " & SQLreader(7) & " cn " & SQLreader(8) & vbCrLf)
        End While
        SQLcommand.Dispose()
        SQLconnect.Close()
    End Sub

    Public Function initPlyrDict(sZone As String, sglScaleFactor As Single, point3dOffset As Point3D) As Dictionary(Of String, objPlayer)
        sZone = escapeQuote(sZone) ' Single quote is a special character in SQLite which must be escaped by its self using 2 single quotes
        initPlyrDict = New Dictionary(Of String, objPlayer)
        Dim SQLconnect As New SQLite.SQLiteConnection()
        Dim SQLcommand As SQLiteCommand
        SQLconnect.ConnectionString = "Data Source=dbPlayers.db3"
        SQLconnect.Open()
        SQLcommand = SQLconnect.CreateCommand
        SQLcommand.CommandText = "SELECT * FROM Players WHERE sZone = '" & sZone & "'"
        Dim SQLreader As SQLiteDataReader = SQLcommand.ExecuteReader()
        While SQLreader.Read()
            '0sCharKey TEXT PRIMARY KEY, 1sglCurLocX FLOAT DEFAULT 0, 2sglCurLocY FLOAT DEFAULT 0, " & _
            '"3sglLastLocX FLOAT DEFAULT 0, 4sglLastLocY FLOAT DEFAULT 0, 5sZone TEXT DEFAULT 'Unknown', 6bFirstPlot BOOLEAN DEFAULT 1, 7bJustZoned BOOLEAN DEFAULT 1 ,8bConnected BOOLEAN DEFAULT 1)"
            '"Unknown" zone should never be sent in as the sZone param, so no Unknown zone dictionary entries should ever be created
            '(ByVal 0sCharKey As String, ByVal 5sZone As String, ByVal 1234dblp3dCoords As objDblPoint3D, sglScaleFactor As Single, point3dOffset As Point3D, ByVal 6bFirstPlot As Boolean, 7bJustZoned As Boolean, 8bConnected As Boolean)
            initPlyrDict.Add(SQLreader(0), New objPlayer(SQLreader(0), SQLreader(5), New objDblPoint3D(SQLreader(1), SQLreader(2), SQLreader(3), SQLreader(4)), sglScaleFactor, point3dOffset, 0 - SQLreader(6), 0 - SQLreader(7), 0 - SQLreader(8))) 'setBconnected to offline for every player when initialising playerDict so updates will cause connected to go to 1
            mapControl.addtoPlayerZones(SQLreader(0), SQLreader(5)) 'sCharkey, sZone
        End While
        SQLcommand.Dispose()
        SQLconnect.Close()
        Return initPlyrDict
    End Function

    Private Sub upSert(sConnection As String, sUpdate As String, sInsert As String)
        Dim SQLconnect As New SQLite.SQLiteConnection()
        Dim SQLcommand As SQLiteCommand
        SQLconnect.ConnectionString = sConnection
        SQLconnect.Open()
        SQLcommand = SQLconnect.CreateCommand
        SQLcommand.CommandText = sUpdate
        If SQLcommand.ExecuteNonQuery() = 0 Then 'execute above query and test if changes made if not insert a new record.
            SQLcommand.CommandText = sInsert
            SQLcommand.ExecuteNonQuery()
        End If
        SQLcommand.Dispose()
        SQLconnect.Close()
    End Sub

    Private Sub update(sConnection As String, sUpdate As String)
        Dim SQLconnect As New SQLite.SQLiteConnection()
        Dim SQLcommand As SQLiteCommand
        SQLconnect.ConnectionString = sConnection
        SQLconnect.Open()
        SQLcommand = SQLconnect.CreateCommand
        SQLcommand.CommandText = sUpdate
        SQLcommand.ExecuteNonQuery()
        SQLcommand.Dispose()
        SQLconnect.Close()
    End Sub

    Public Sub deleteAllPlayers()
        Dim SQLconnect As New SQLite.SQLiteConnection()
        Dim SQLcommand As SQLiteCommand
        SQLconnect.ConnectionString = "Data Source=dbPlayers.db3"
        SQLconnect.Open()
        SQLcommand = SQLconnect.CreateCommand
        SQLcommand.CommandText = "DROP TABLE IF EXISTS Players"
        SQLcommand.ExecuteNonQuery()
        SQLcommand.Dispose()
        SQLconnect.Close()
    End Sub

    Public Sub deleteAllMaps()
        Dim SQLconnect As New SQLite.SQLiteConnection()
        Dim SQLcommand As SQLiteCommand
        SQLconnect.ConnectionString = "Data Source=dbPlayers.db3"
        SQLconnect.Open()
        SQLcommand = SQLconnect.CreateCommand
        SQLcommand.CommandText = "DROP TABLE IF EXISTS Maps"
        SQLcommand.ExecuteNonQuery()
        SQLcommand.Dispose()
        SQLconnect.Close()
    End Sub

    Private Sub initPlayers()
        Dim SQLconnect As New SQLite.SQLiteConnection()
        Dim SQLcommand As SQLiteCommand
        SQLconnect.ConnectionString = "Data Source=dbPlayers.db3"
        SQLconnect.Open()
        SQLcommand = SQLconnect.CreateCommand
        SQLcommand.CommandText = "CREATE TABLE IF NOT EXISTS Players(sCharKey TEXT PRIMARY KEY, sglCurLocX DOUBLE DEFAULT 0, sglCurLocY DOUBLE DEFAULT 0, " & _
            "sglLastLocX DOUBLE DEFAULT 0, sglLastLocY DOUBLE DEFAULT 0, sZone TEXT DEFAULT 'Unknown', bFirstPlot BOOLEAN DEFAULT 1, bJustZoned BOOLEAN DEFAULT 1, bConnected BOOLEAN DEFAULT 0)"
        SQLcommand.ExecuteNonQuery()
        'SQL query to SET everyone to disconnected true on server/client loading - incase of crash)
        SQLcommand.CommandText = "UPDATE Players SET bConnected = 0" 'only set firstPlot when char moveing, zoning and dieing (dieing causes zone even if bound in same zone)
        SQLcommand.ExecuteNonQuery()

        'read back records - for testing (Select Query)
        SQLcommand.CommandText = "SELECT * FROM Players"
        Dim SQLreader As SQLiteDataReader = SQLcommand.ExecuteReader()
        frmMap.appendData("Players:" & vbCrLf)
        While SQLreader.Read()
            frmMap.appendData("D ch " & SQLreader(0) & " px " & SQLreader(1) & " py " & SQLreader(2) & " qx " & SQLreader(3) & " qy " & SQLreader(4) & " zn " & SQLreader(5) & " fp " & SQLreader(6) & " jz " & SQLreader(7) & " cn " & SQLreader(8) & vbCrLf)
        End While

        SQLcommand.Dispose()
        SQLconnect.Close()

    End Sub

    Private Sub initMaps()
        Dim SQLconnect As New SQLite.SQLiteConnection()
        Dim SQLcommand As SQLiteCommand
        SQLconnect.ConnectionString = "Data Source=dbMaps.db3"
        SQLconnect.Open()
        SQLcommand = SQLconnect.CreateCommand
        SQLcommand.CommandText = "CREATE TABLE IF NOT EXISTS Maps(sZone TEXT PRIMARY KEY, sFileName TEXT, sWhoName TEXT, sAlphaName TEXT, iEqGridSize INTEGER, iMapGridSize INTEGER, sglMapOffSetX FLOAT, sglMapOffSetY FLOAT)"
        SQLcommand.ExecuteNonQuery()
        SQLcommand.CommandText = "SELECT Count(*) FROM Maps" 'determine if map table is empty
        If SQLcommand.ExecuteScalar() = 0 Then 'Map Table empty - populate
            Dim strbUpdate As New StringBuilder

            SQLcommand.CommandText = "INSERT INTO Maps(sZone, sFileName, sWhoName, sAlphaName, iEqGridSize, iMapGridSize, sglMapOffSetX, sglMapOffSetY) VALUES " &
                "('The Burning Wood', 'Map_burningwood.jpg', 'Burning Woods', 'Burning Woods', '1000', '47', '179.0', '352.0'), " &
                "('Butcherblock Mountains', 'Butcherblock.jpg', 'Butcherblock Mountains', 'Butcherblock', '1000', '73', '257.0', '250.0'), " &
                "('East Commonlands', 'Map_eastcommons.jpg', 'East Commonlands', 'Commons East', '1000', '89', '475.0', '143.0'), " &
                "('West Commonlands', 'Zone_westcommons.jpg', 'West Commonlands', 'Commons West', '1000', '96', '441.0', '136.0'), " &
                "('Cobaltscar', 'Map_cobalt_scar.jpg', 'Cobalt Scar', 'Cobalt Scar', '500', '60', '263.0', '224.0'), " &
                "('Crushbone', 'Crushbone.jpg', 'Clan Crushbone', 'Crushbone', '200', '66', '310.0', '156.0'), " &
                "('Dreadlands', 'Map_dreadlands.jpg', 'Dreadlands', 'Dreadlands', '1000', '47', '504.0', '188.0'), " &
                "('The Emerald Jungle', 'Map_emeraldjungle.jpg', 'The Emerald Jungle', 'Emerald Jungle', '1000', '57', '324.0', '282.0'), " &
                "('Everfrost', 'Map_everfrost.jpg', 'Everfrost Peaks', 'Everfrost', '1000', '56', '107.0', '240.0'), " &
                "('Greater Faydark', 'Greaterfaydark.jpg', 'Greater Faydark', 'Faydark Greater', '1000', '89', '275.0', '265.0'), " &
                "('Lesser Faydark', 'Lesserfaydark.jpg', 'Lesser Faydark', 'Faydark Lesser', '1000', '87', '364.0', '201.0'), " &
                "('The Feerrott', 'Map_feerrott.jpg', 'The Feerrott', 'Feerrott', '500', '40', '279.0', '153.0'), " &
                "('Field of Bone', 'Map_fieldofbone.jpg', 'The Field of Bone', 'Field of Bone', '1000', '77', '359.0', '291.0'), " &
                "('Firiona Vie', 'Map_firionavie.jpg', 'Firiona Vie', 'Firiona Vie', '1000', '52', '329.0', '292.0'), " &
                "('Frontier Mountains', 'Map_frontiermtns.jpg', 'Frontier Mountains', 'Frontier Mountains', '1000', '58', '277.0', '298.0'), " &
                "('The Great Divide', 'Map_great_divide.jpg', 'Great Divide', 'Great Divide', '1000', '54', '266.0', '51.0'), " &
                "('Guk', 'Upperguk.jpg', 'Upper Guk', 'Guk Upper', '250', '80', '242.0', '574.0'), " &
                "('The Iceclad Ocean', 'Iceclad.jpg', 'Iceclad Ocean', 'Iceclad Ocean', '2000', '44', '250.0', '228.0'), " &
                "('Innothule Swamp', 'Map_innothule.jpg', 'Innothule Swamp', 'Innothule', '1000', '117', '256.0', '307.0'), " &
                "('Eastern Plains of Karana', 'Map_ekarana.jpg', 'East Karana', 'Karana East', '1000', '79', '125.0', '203.0'), " &
                "('Northern Plains of Karana', 'Map_nkarana.jpg', 'North Karana', 'Karana North', '1000', '70', '269.0', '145.0'), " &
                "('Southern Plains of Karana', 'Zone_southkarana.jpg', 'South Karana', 'Karana South', '1000', '47', '182.0', '142.0'), " &
                "('Western Plains of Karana', 'Zone_westkarana.jpg', 'West Karana', 'Karana West', '1000', '64', '0', '117.0'), " &
                "('Kithicor Woods', 'Map_kithicor.jpg', 'Kithicor Forest', 'Kithicor', '1000', '88', '456.0', '207.0'), " &
                "('Lake of Ill Omen', 'Map_lakeofillomen.jpg', 'Lake of Ill Omen', 'Lake Of Ill Omen', '2000', '87', '197.0', '338.0'), " &
                "('Lavastorm Mountains', 'Map_lavastorm.jpg', 'Lavastorm Mountains', 'Lavastorm Mountains', '500', '66', '228.0', '224.0'), " &
                "('Castle Mistmoore', 'Mistmoore.jpg', 'Castle Mistmoore', 'Mistmoore', '100', '62', '396.0', '203.0'), " &
                "('Misty Thicket', 'Map_mistythicket.jpg', 'Misty Thicket', 'Misty Thicket', '500', '60', '203.0', '174.0'), " &
                "('Najena', 'Najena.jpg', 'Najena', 'Najena', '250', '94', '373.0', '233.0'), " &
                "('The Nektulos Forest', 'Map_nektulos.jpg', 'Nektulos Forest', 'Nektulos Forest', '1000', '91', '143.0', '275.0'), " &
                "('Oasis of Marr', 'Map_oasisofmarr.jpg', 'Oasis of Marr', 'Oasis Of Marr', '1000', '102', '178.0', '281.0'), " &
                "('The Overthere', 'Map_overthere.jpg', 'The Overthere', 'Overthere', '1000', '57', '266.0', '300.0'), " &
                "('Permafrost', 'Zone_permafrost.jpg', 'Permafrost Keep', 'Permafrost Caverns', '100', '37', '187.0', '487.0'), " &
                "('Qeynos Hills', 'Qeynoshills.jpg', 'Qeynos Hills', 'Qeynos Hills', '1000', '94', '141.0', '526.0'), " &
                "('Rathe Mountains', 'Zone_rathemtns.jpg', 'Mountains of Rathe', 'Rathe Mountains', '1500', '106', '226.0', '464.0'), " &
                "('Rivervale', 'Rivervale.jpg', 'Rivervale', 'Rivervale', '200', '104', '92.0', '297.0'), " &
                "('Northern Desert of Ro', 'Map_northro.jpg', 'North Ro', 'Ro North', '1000', '87', '175.0', '389.0'), " &
                "('Southern Desert of Ro', 'Zone_southro.jpg', 'South Ro', 'Ro South', '1000', '99', '181.0', '182.0'), " &
                "('Skyfire Mountains', 'Map_skyfiremtns.jpg', 'Skyfire Mountains', 'Skyfire Mountains', '1000', '52', '294.0', '319.0'), " &
                "('Steamfont Mountains', 'Steamfont.jpg', 'Steamfont Mountains', 'Steamfont Mountains', '1000', '106', '251.0', '233.0'), " &
                "('Swamp Of No Hope', 'Map_swampofnohope.jpg', 'Swamp of No Hope', 'Swamp Of No Hope', '1000', '58', '251.0', '347.0'), " &
                "('Timorous Deep', 'Map_timdeep.jpg', 'Timorous Deep', 'Timorous Deep', '1000', '31', '158.0', '327.0'), " &
                "('Toxxulia Forest', 'Toxxulia.jpg', 'Toxxulia Forest', 'Toxxulia Forest', '1000', '91', '268.0', '271.0'), " &
                "('Trakanon''s Teeth', 'Map_trakanonsteeth.jpg', 'Trakanon''s Teeth', 'Trakanon''s Teeth', '1000', '47', '270.0', '230.0'), " &
                "('The Wakening Land', 'Map_wakening_lands.jpg', 'The Wakening Land', 'Wakening Land', '1000', '49', '262.0', '186.0'), " &
                "('Warsliks Woods', 'Map_warslikswood.jpg', 'Warsliks Wood', 'Warsliks Woods', '1000', '59', '292.0', '266.0'), " &
                "('Eastern Wastes', 'Map_eastern_wastes.jpg', 'Eastern Wastes', 'Wastes Eastern', '1000', '38', '289.0', '71.0'), " &
                "('The Western Wastes', 'Westernwastes.jpg', 'Western Wastes', 'Wastes Western', '1000', '53', '295.0', '292.0')"
            'fill in other maps here
            '(sZone, sFileName, sWhoName, sAlphaName, iEqGridSize, iMapGridSize, sglMapOffSetX, sglMapOffSetY)
            SQLcommand.ExecuteNonQuery()
        End If

        'read back records - for testing
        SQLcommand.CommandText = "SELECT * FROM Maps"
        Dim SQLreader As SQLiteDataReader = SQLcommand.ExecuteReader()
        frmMap.appendData("Maps:" & vbCrLf)
        While SQLreader.Read()
            frmMap.appendData(SQLreader(0) & ", " & SQLreader(1) & ", " & SQLreader(2) & ", " & SQLreader(3) & ", " & SQLreader(4) & ", " & SQLreader(5) & ", " & SQLreader(6) & ", " & SQLreader(7) & vbCrLf)
        End While

        SQLcommand.Dispose()
        SQLconnect.Close()
    End Sub

    Private Function escapeQuote(sStr As String) As String
        Return sStr.Replace("'", "''") 'replace single quote with escaped single quote to avoid sqlite special char use errors
    End Function

    'Private Function removeQuote(sZone As String) As String
    '    Return sZone.Replace("'", "&#39;")
    'End Function

    'Private Function replaceQuote(sZone As String) As String
    '    Return sZone.Replace("&#39;", "'")
    'End Function

End Module
