' XRDB4_Plugins.vb - a set of pre-written parser plugins for XRDB4
' Programmer: William "Xyphos" Scott <TheGreatXyphos@gmail.com>
' Date: Aug 21, 2009
'
'This file is part of XRDB4.
'
'    XRDB4 is free software: you can redistribute it and/or modify
'    it under the terms of the GNU General Public License as published by
'    the Free Software Foundation, either version 3 of the License, or
'    (at your option) any later version.
'
'    XRDB4 is distributed in the hope that it will be useful,
'    but WITHOUT ANY WARRANTY; without even the implied warranty of
'    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    GNU General Public License for more details.
'
'    You should have received a copy of the GNU General Public License
'    along with XRDB4.  If not, see <http://www.gnu.org/licenses/>.

Imports System
Imports System.IO
Imports System.Text


Public Class AOCellItemOutput
    Implements XRDB4_Extras.Plugin

    Private outputfile As StreamWriter
    Private outputfile2 As StreamWriter
    Private Lookup As XRDB4_Extras.Lookups
    Private SkipCompare As Boolean
    Private ItAreNano As Boolean
    Private ElementsOpen As Integer

    Public attackval As String
    Public defval As String
    Public attrval As String
    Public aoidnum As String
    Public isnanoitem As String
    Public itemql As String
    Public args As String
    Public funcs As String
    Public actions As String
    Public reqs As String
    Public countactions As Integer
    Public countevents As Integer
    Public sinbytes() As Byte
    Private xname As String


    Public actionid As Integer = 0
    Public eventid As Integer = 0
    Public functionid As Integer = 0
    Public reqid As Integer = 0
    Public actionreqid As Integer = 0
    Public attackattrid As Integer = 0
    Public defenseattrid As Integer = 0
    Public attributeid As Integer = 0
    Public functionargid As Integer = 0

    Public aoidholder As Integer = 0



    Public Event Abort(ReasonMsg As String) Implements XRDB4_Extras.Plugin.Abort
    Public Event ChangePriority(ByVal Priority As System.Threading.ThreadPriority) Implements XRDB4_Extras.Plugin.ChangePriority

    Public Function revit(ByVal torev As String) As String
        Dim c As Integer = torev.Length
        Dim outt As String = ""
        Do While c > 0
            c -= 2
            outt += torev.Substring(c, 2)
        Loop
        Return outt
    End Function

    Public Function ExtractInfo() _
        As XRDB4_Extras.ExtractRecordDictionary.ExtractRecord() _
        Implements XRDB4_Extras.Plugin.ExtractInfo

        Dim ERD As New XRDB4_Extras.ExtractRecordDictionary

        Return New XRDB4_Extras.ExtractRecordDictionary.ExtractRecord() _
            {ERD.Items, ERD.NanoPrograms}

    End Function

    Public Sub Parse_Begin( _
            ByVal OutputPath As String, _
            ByVal AOVersion As String, _
            ByVal SkippedCompare As Boolean, _
            ByVal CommandLine As String) _
        Implements XRDB4_Extras.Plugin.Parse_Begin

        SkipCompare = SkippedCompare

        Lookup = New XRDB4_Extras.Lookups

        Dim File As String = String.Format("{0}\items.sql", OutputPath)

        If My.Computer.FileSystem.FileExists(File) Then
            If MsgBox(String.Format("File {0}{1}{0}{2}already exists. Overwrite?", _
                                    Chr(34), File, vbNewLine), _
                        MsgBoxStyle.YesNo _
                        Or MsgBoxStyle.Question) = MsgBoxResult.No Then
                RaiseEvent Abort("File was not overwritten")
                Exit Sub
            End If
        End If

        Dim File2 As String = String.Format("{0}\itemnames.sql", OutputPath)
        If My.Computer.FileSystem.FileExists(File2) Then
            If MsgBox(String.Format("File {0}{1}{0}{2}already exists. Overwrite?", _
                                    Chr(34), File2, vbNewLine), _
                        MsgBoxStyle.YesNo _
                        Or MsgBoxStyle.Question) = MsgBoxResult.No Then
                RaiseEvent Abort("File was not overwritten")
                Exit Sub
            End If
        End If

        outputfile = New StreamWriter(File)
        outputfile2 = New StreamWriter(File2)

        outputfile.WriteLine("CREATE TABLE  `items` (")
        outputfile.WriteLine("  `AOID` int(10) NOT NULL,")
        outputfile.WriteLine("  `IsNano` int(10) NOT NULL DEFAULT '0',")
        outputfile.WriteLine("  `QL` int(10) NOT NULL,")
        outputfile.WriteLine("  `ItemType` int(10) NOT NULL,")
        outputfile.WriteLine("  PRIMARY KEY (`AOID`)")
        outputfile.WriteLine(") ENGINE=MyIsam DEFAULT CHARSET=latin1;")
        outputfile.WriteLine("")

        outputfile.WriteLine("CREATE TABLE `item_events` (")
        outputfile.WriteLine("`eventid` int(10) NOT NULL,")
        outputfile.WriteLine("`itemid` int(10) NOT NULL,")
        outputfile.WriteLine("`eventnum` int(10) NOT NULL,")
        outputfile.WriteLine("PRIMARY KEY (`itemid`,`eventid`) USING BTREE")
        outputfile.WriteLine(") ENGINE=MyISAM DEFAULT CHARSET=latin1;")
        outputfile.WriteLine("")

        outputfile.WriteLine("CREATE TABLE `item_functions` (")
        outputfile.WriteLine("`functionid` int(10) NOT NULL,")
        outputfile.WriteLine("`eventid` int(10) NOT NULL,")
        outputfile.WriteLine("`itemid` int(10) NOT NULL,")
        outputfile.WriteLine("`functionnum` int(10) NOT NULL,")
        outputfile.WriteLine("`target` int(10) NOT NULL,")
        outputfile.WriteLine("`tickcount` int(10) NOT NULL,")
        outputfile.WriteLine("`tickinterval` int(10) NOT NULL,")
        outputfile.WriteLine("PRIMARY KEY (`itemid`,`eventid`,`functionid`) USING BTREE")
        outputfile.WriteLine(") ENGINE=MyISAM DEFAULT CHARSET=latin1;")
        outputfile.WriteLine("")

        outputfile.WriteLine("CREATE TABLE `item_function_reqs` (")
        outputfile.WriteLine("`reqid` int(10) NOT NULL,")
        outputfile.WriteLine("`functionid` int(10) NOT NULL,")
        outputfile.WriteLine("`eventid` int(10) NOT NULL,")
        outputfile.WriteLine("`itemid` int(10) NOT NULL,")
        outputfile.WriteLine("`attrnum` int(10) NOT NULL,")
        outputfile.WriteLine("`attrval` int(10) NOT NULL,")
        outputfile.WriteLine("`operator` int(10) NOT NULL,")
        outputfile.WriteLine("`child_op` int(10) NOT NULL,")
        outputfile.WriteLine("`target` int(10) NOT NULL,")
        outputfile.WriteLine("PRIMARY KEY (`itemid`,`eventid`,`functionid`,`reqid`) USING BTREE")
        outputfile.WriteLine(") ENGINE=MyISAM DEFAULT CHARSET=latin1;")
        outputfile.WriteLine("")

        outputfile.WriteLine("CREATE TABLE `item_function_arguments` (")
        outputfile.WriteLine("`attrid` int(10) NOT NULL,")
        outputfile.WriteLine("`functionid` int(10) NOT NULL,")
        outputfile.WriteLine("`eventid` int(10) NOT NULL,")
        outputfile.WriteLine("`itemid` int(10) NOT NULL,")
        outputfile.WriteLine("`argvalint` int(10),")
        outputfile.WriteLine("`argvalsingle` Float,")
        outputfile.WriteLine("`argvalstring` TEXT,")
        outputfile.WriteLine("PRIMARY KEY (`itemid`,`eventid`,`functionid`,`attrid`) USING BTREE")
        outputfile.WriteLine(") ENGINE=MyISAM DEFAULT CHARSET=latin1;")
        outputfile.WriteLine("")

        outputfile.WriteLine("CREATE TABLE `item_actions` (")
        outputfile.WriteLine("`actionid` int(10) NOT NULL,")
        outputfile.WriteLine("`itemid` int(10) NOT NULL,")
        outputfile.WriteLine("`actionnum` int(10) NOT NULL,")
        outputfile.WriteLine("PRIMARY KEY (`itemid`,`actionid`) USING BTREE")
        outputfile.WriteLine(") ENGINE=MyISAM DEFAULT CHARSET=latin1;")
        outputfile.WriteLine("")

        outputfile.WriteLine("CREATE TABLE `item_action_reqs` (")
        outputfile.WriteLine("`reqid` int(10) NOT NULL,")
        outputfile.WriteLine("`actionid` int(10) NOT NULL,")
        outputfile.WriteLine("`itemid` int(10) NOT NULL,")
        outputfile.WriteLine("`attrnum` int(10) NOT NULL,")
        outputfile.WriteLine("`attrval` int(10) NOT NULL,")
        outputfile.WriteLine("`operator` int(10) NOT NULL,")
        outputfile.WriteLine("`child_op` int(10) NOT NULL,")
        outputfile.WriteLine("`target` int(10) NOT NULL,")
        outputfile.WriteLine("PRIMARY KEY (`itemid`,`actionid`,`reqid`) USING BTREE")
        outputfile.WriteLine(") ENGINE=MyISAM DEFAULT CHARSET=latin1;")
        outputfile.WriteLine("")

        outputfile.WriteLine("CREATE TABLE `item_defense_attributes` (")
        outputfile.WriteLine("`defenseid` int(10) NOT NULL,")
        outputfile.WriteLine("`itemid` int(10) NOT NULL,")
        outputfile.WriteLine("`num` int(10) NOT NULL,")
        outputfile.WriteLine("`value` int(10) NOT NULL,")
        outputfile.WriteLine("PRIMARY KEY (`itemid`,`defenseid`,`num`) USING BTREE")
        outputfile.WriteLine(") ENGINE=MyISAM DEFAULT CHARSET=latin1;")
        outputfile.WriteLine("")

        outputfile.WriteLine("CREATE TABLE `item_attack_attributes` (")
        outputfile.WriteLine("`attackid` int(10) NOT NULL,")
        outputfile.WriteLine("`itemid` int(10) NOT NULL,")
        outputfile.WriteLine("`num` int(10) NOT NULL,")
        outputfile.WriteLine("`value` int(10) NOT NULL,")
        outputfile.WriteLine("PRIMARY KEY (`itemid`,`attackid`,`num`) USING BTREE")
        outputfile.WriteLine(") ENGINE=MyISAM DEFAULT CHARSET=latin1;")
        outputfile.WriteLine("")

        outputfile.WriteLine("CREATE TABLE `item_attributes` (")
        outputfile.WriteLine("`attributeid` int(10) NOT NULL,")
        outputfile.WriteLine("`itemid` int(10) NOT NULL,")
        outputfile.WriteLine("`num` int(10) NOT NULL,")
        outputfile.WriteLine("`value` int(10) NOT NULL,")
        outputfile.WriteLine("PRIMARY KEY (`itemid`,`attributeid`,`num`) USING BTREE")
        outputfile.WriteLine(") ENGINE=MyISAM DEFAULT CHARSET=latin1;")
        outputfile.WriteLine("")




        outputfile2.WriteLine("CREATE TABLE `itemnames` (")
        outputfile2.WriteLine("  `AOID` int(10) NOT NULL,")
        outputfile2.WriteLine("  `Name` varchar(250) NOT NULL,")
        outputfile2.WriteLine("  PRIMARY KEY (`AOID`)")
        outputfile2.WriteLine(") ENGINE=MyIsam DEFAULT CHARSET=latin1;")
        outputfile2.WriteLine("")


    End Sub

    Public Sub Parse_End(ByVal Aborted As Boolean) _
        Implements XRDB4_Extras.Plugin.Parse_End

        outputfile.Close()
        outputfile2.Close()
    End Sub

    Public Function ItemNano_Begin( _
            ByVal AOID As Integer, _
            ByVal IsNano As Boolean, _
            ByVal ChangeState As XRDB4_Extras.Plugin.ChangeStates) _
        As Boolean _
        Implements XRDB4_Extras.Plugin.ItemNano_Begin

        attributeid = 0
        attackattrid = 0
        defenseattrid = 0
        eventid = 0
        actionid = 0


        countactions = 0
        countevents = 0

        aoidholder = AOID

        ItAreNano = IsNano
        If Not IsNano Then
            outputfile.Write("INSERT INTO items VALUES (" + AOID.ToString() + ",0,")
            outputfile2.Write("INSERT INTO itemnames VALUES(" + AOID.ToString() + ",")
        End If
        Return Not IsNano 'Yes, we want this record parsed
    End Function

    Public Sub ItemNano( _
            ByVal Info As XRDB4_Extras.Plugin.ItemNanoInfo, _
            ByVal Attributes() As XRDB4_Extras.Plugin.ItemNanoKeyVal) _
            Implements XRDB4_Extras.Plugin.ItemNano

        xname = Info.Name
        xname = xname.Replace("'", "''")


        outputfile2.WriteLine("'" + xname + "');")
        outputfile.WriteLine(Info.QL.ToString() + "," + Info.Type.ToString() + ");")

        attributeid = 0
        actionid = 0

        For Each Item As XRDB4_Extras.Plugin.ItemNanoKeyVal In Attributes

            outputfile.WriteLine("INSERT INTO item_attributes VALUES (" + attributeid.ToString() + "," + aoidholder.ToString() + "," + Item.AttrKey.ToString() + "," + Item.AttrVal.ToString() + ");")
            ' attrval += revit(Item.AttrKey.ToString("X8"))
            ' attrval += revit(Item.AttrVal.ToString("X8"))
            attributeid += 1
        Next

    End Sub

    Public Sub ItemNano_End() Implements XRDB4_Extras.Plugin.ItemNano_End
        If attackval = "" Then attackval = "00000000"
        If defval = "" Then defval = "00000000"
        If attrval = "" Then attrval = "00000000"
        If funcs = "" Then funcs = "00000000"
        If actions = "" Then actions = "00000000"
        'outputfile.WriteLine(attackval + defval + attrval + revit(countevents.ToString("X8")) + funcs + revit(countactions.ToString("X8")) + actions + "');")
        attackval = ""
        defval = ""
        attrval = ""
        args = ""
        funcs = ""
        actions = ""
        reqs = ""
        countactions = 0
        countevents = 0

    End Sub

    Public Sub ItemNanoAction( _
            ByVal ActionNum As Integer, _
            ByVal Requirements() As XRDB4_Extras.Plugin.ItemNanoRequirement) _
            Implements XRDB4_Extras.Plugin.ItemNanoAction


        outputfile.WriteLine("INSERT INTO item_actions VALUES (" + actionid.ToString() + "," + aoidholder.ToString() + "," + ActionNum.ToString() + ");")



        actions += revit(ActionNum.ToString("X8"))
        actionreqid = 0
        For Each R As XRDB4_Extras.Plugin.ItemNanoRequirement In Requirements
            outputfile.WriteLine("INSERT INTO item_action_reqs VALUES (" + actionreqid.ToString() + "," + actionid.ToString() + "," + aoidholder.ToString() + "," + R.AttrNum.ToString() + "," + R.AttrValue.ToString() + "," + R.MainOp.ToString() + "," + R.ChildOp.ToString() + "," + R.Target.ToString() + ");")
            actionreqid += 1
        Next
        actionid += 1
    End Sub


    Public Sub ItemNanoAttackAndDefense( _
            ByVal Attack() As XRDB4_Extras.Plugin.ItemNanoKeyVal, _
            ByVal Defense() As XRDB4_Extras.Plugin.ItemNanoKeyVal) _
            Implements XRDB4_Extras.Plugin.ItemNanoAttackAndDefense

        attackattrid = 0
        For Each DE As XRDB4_Extras.Plugin.ItemNanoKeyVal In Attack
            outputfile.WriteLine("INSERT INTO item_attack_attributes VALUES (" + attackattrid.ToString() + "," + aoidholder.ToString() + "," + DE.AttrKey.ToString() + "," + DE.AttrVal.ToString() + ");")
            attackattrid += 1
        Next

        defenseattrid = 0
        For Each DE As XRDB4_Extras.Plugin.ItemNanoKeyVal In Defense
            outputfile.WriteLine("INSERT INTO item_defense_attributes VALUES (" + defenseattrid.ToString() + "," + aoidholder.ToString() + "," + DE.AttrKey.ToString() + "," + DE.AttrVal.ToString() + ");")
            defenseattrid += 1
        Next

    End Sub

    Public Sub ItemNanoEventAndFunctions( _
            ByVal EventNum As Integer, _
            ByVal Functions() As XRDB4_Extras.Plugin.ItemNanoFunction) _
            Implements XRDB4_Extras.Plugin.ItemNanoEventAndFunctions

        Dim tempint As Int32
        Dim tempsingle As Single
        countevents += 1

        outputfile.WriteLine("INSERT INTO item_events VALUES (" + eventid.ToString() + "," + aoidholder.ToString() + "," + EventNum.ToString() + ");")
        functionid = 0
        For Each F As XRDB4_Extras.Plugin.ItemNanoFunction In Functions

            outputfile.WriteLine("INSERT INTO item_functions VALUES (" + functionid.ToString() + "," + eventid.ToString() + "," + aoidholder.ToString() + "," + F.FunctionNum.ToString() + "," + F.Target.ToString() + "," + F.TickCount.ToString() + "," + F.TickInterval.ToString() + ");")

            functionargid = 0

            For Each A As String In F.FunctionArgs

                If Int32.TryParse(A, tempint) Then
                    outputfile.WriteLine("INSERT INTO item_function_arguments VALUES (" + functionargid.ToString() + "," + functionid.ToString() + "," + eventid.ToString() + "," + aoidholder.ToString() + "," + tempint.ToString() + ",NULL,NULL);")
                Else
                    If Single.TryParse(A, tempsingle) Then
                        outputfile.WriteLine("INSERT INTO item_function_arguments VALUES (" + functionargid.ToString() + "," + functionid.ToString() + "," + eventid.ToString() + "," + aoidholder.ToString() + ",NULL," + tempsingle.ToString().Replace(",", ".") + ",NULL);")
                    Else
                        outputfile.WriteLine("INSERT INTO item_function_arguments VALUES (" + functionargid.ToString() + "," + functionid.ToString() + "," + eventid.ToString() + "," + aoidholder.ToString() + ",NULL,NULL,'" + A.Replace("'", "''").TrimEnd(Convert.ToChar(0)) + "');")
                    End If
                End If
                functionargid += 1
            Next

            reqid = 0
            For Each R As XRDB4_Extras.Plugin.ItemNanoRequirement In F.FunctionReqs
                outputfile.WriteLine("INSERT INTO item_function_reqs VALUES (" + reqid.ToString() + "," + functionid.ToString() + "," + eventid.ToString() + "," + aoidholder.ToString() + "," + R.AttrNum.ToString() + "," + R.AttrValue.ToString() + "," + R.MainOp.ToString() + "," + R.ChildOp.ToString() + "," + R.Target.ToString() + ");")
                reqid += 1
            Next
            functionid += 1
        Next
        eventid += 1
    End Sub

    Public Sub ItemNanoAnimSets(ByVal ActionNum As Integer, ByVal AnimData() As Integer) Implements XRDB4_Extras.Plugin.ItemNanoAnimSets
    End Sub

    Public Sub ItemNanoSoundSets(ByVal ActionNum As Integer, ByVal AnimData() As Integer) Implements XRDB4_Extras.Plugin.ItemNanoSoundSets
    End Sub

    Public Function OtherData_Begin( _
            ByVal AOID As Integer, _
            ByVal RecordType As Integer, _
            ByVal ChangeState As XRDB4_Extras.Plugin.ChangeStates) _
        As Boolean _
            Implements XRDB4_Extras.Plugin.OtherData_Begin

        Return False 'Not interested in other data
    End Function

    Public Sub OtherData(ByVal BinaryData() As Byte) Implements XRDB4_Extras.Plugin.OtherData
    End Sub
    Public Sub OtherData_End() Implements XRDB4_Extras.Plugin.OtherData_End
    End Sub

End Class

Public Class AOCellNanoOutput
    Implements XRDB4_Extras.Plugin

    Private outputfile As StreamWriter
    Private Lookup As XRDB4_Extras.Lookups
    Private SkipCompare As Boolean
    Private ItAreNano As Boolean
    Private ElementsOpen As Integer

    Public attackval As String
    Public defval As String
    Public attrval As String
    Public aoidnum As String
    Public isnanoitem As String
    Public itemql As String
    Public args As String
    Public funcs As String
    Public actions As String
    Public reqs As String
    Public countactions As Integer
    Public countevents As Integer
    Public sinbytes() As Byte


    Public actionid As Integer = 0
    Public eventid As Integer = 0
    Public functionid As Integer = 0
    Public reqid As Integer = 0
    Public actionreqid As Integer = 0
    Public attackattrid As Integer = 0
    Public defenseattrid As Integer = 0
    Public attributeid As Integer = 0
    Public functionargid As Integer = 0

    Public aoidholder As Integer = 0

    Public Event Abort(ByVal ReasonMsg As String) Implements XRDB4_Extras.Plugin.Abort
    Public Event ChangePriority(ByVal Priority As System.Threading.ThreadPriority) Implements XRDB4_Extras.Plugin.ChangePriority

    Public Function revit(ByVal torev As String) As String
        Dim c As Integer = torev.Length
        Dim outt As String = ""
        Do While c > 0
            c -= 2
            outt += torev.Substring(c, 2)
        Loop
        Return outt
    End Function

    Public Function ExtractInfo() _
        As XRDB4_Extras.ExtractRecordDictionary.ExtractRecord() _
        Implements XRDB4_Extras.Plugin.ExtractInfo

        Dim ERD As New XRDB4_Extras.ExtractRecordDictionary

        Return New XRDB4_Extras.ExtractRecordDictionary.ExtractRecord() _
            {ERD.Items, ERD.NanoPrograms}

    End Function

    Public Sub Parse_Begin( _
            ByVal OutputPath As String, _
            ByVal AOVersion As String, _
            ByVal SkippedCompare As Boolean, _
            ByVal CommandLine As String) _
        Implements XRDB4_Extras.Plugin.Parse_Begin

        SkipCompare = SkippedCompare

        Lookup = New XRDB4_Extras.Lookups

        Dim File As String = String.Format("{0}\nanos.sql", OutputPath)

        If My.Computer.FileSystem.FileExists(File) Then
            If MsgBox(String.Format("File {0}{1}{0}{2}already exists. Overwrite?", _
                                    Chr(34), File, vbNewLine), _
                        MsgBoxStyle.YesNo _
                        Or MsgBoxStyle.Question) = MsgBoxResult.No Then
                RaiseEvent Abort("File was not overwritten")
                Exit Sub
            End If
        End If

        outputfile = New StreamWriter(File)

        outputfile.WriteLine("CREATE TABLE  `nanos` (")
        outputfile.WriteLine("  `AOID` int(10) NOT NULL,")
        outputfile.WriteLine("  `IsNano` int(10) NOT NULL DEFAULT '0',")
        outputfile.WriteLine("  `QL` int(10) NOT NULL,")
        outputfile.WriteLine("  `ItemType` int(10) NOT NULL,")
        outputfile.WriteLine("  PRIMARY KEY (`AOID`)")
        outputfile.WriteLine(") ENGINE=MyIsam DEFAULT CHARSET=latin1;")
        outputfile.WriteLine("")

        outputfile.WriteLine("CREATE TABLE `nano_events` (")
        outputfile.WriteLine("`eventid` int(10) NOT NULL,")
        outputfile.WriteLine("`nanoid` int(10) NOT NULL,")
        outputfile.WriteLine("`eventnum` int(10) NOT NULL,")
        outputfile.WriteLine("PRIMARY KEY (`nanoid`,`eventid`) USING BTREE")
        outputfile.WriteLine(") ENGINE=MyISAM DEFAULT CHARSET=latin1;")
        outputfile.WriteLine("")

        outputfile.WriteLine("CREATE TABLE `nano_functions` (")
        outputfile.WriteLine("`functionid` int(10) NOT NULL,")
        outputfile.WriteLine("`eventid` int(10) NOT NULL,")
        outputfile.WriteLine("`nanoid` int(10) NOT NULL,")
        outputfile.WriteLine("`functionnum` int(10) NOT NULL,")
        outputfile.WriteLine("`target` int(10) NOT NULL,")
        outputfile.WriteLine("`tickcount` int(10) NOT NULL,")
        outputfile.WriteLine("`tickinterval` int(10) NOT NULL,")
        outputfile.WriteLine("PRIMARY KEY (`nanoid`,`eventid`,`functionid`) USING BTREE")
        outputfile.WriteLine(") ENGINE=MyISAM DEFAULT CHARSET=latin1;")
        outputfile.WriteLine("")

        outputfile.WriteLine("CREATE TABLE `nano_function_reqs` (")
        outputfile.WriteLine("`reqid` int(10) NOT NULL,")
        outputfile.WriteLine("`functionid` int(10) NOT NULL,")
        outputfile.WriteLine("`eventid` int(10) NOT NULL,")
        outputfile.WriteLine("`nanoid` int(10) NOT NULL,")
        outputfile.WriteLine("`attrnum` int(10) NOT NULL,")
        outputfile.WriteLine("`attrval` int(10) NOT NULL,")
        outputfile.WriteLine("`operator` int(10) NOT NULL,")
        outputfile.WriteLine("`child_op` int(10) NOT NULL,")
        outputfile.WriteLine("`target` int(10) NOT NULL,")
        outputfile.WriteLine("PRIMARY KEY (`nanoid`,`eventid`,`functionid`,`reqid`) USING BTREE")
        outputfile.WriteLine(") ENGINE=MyISAM DEFAULT CHARSET=latin1;")
        outputfile.WriteLine("")

        outputfile.WriteLine("CREATE TABLE `nano_function_arguments` (")
        outputfile.WriteLine("`attrid` int(10) NOT NULL,")
        outputfile.WriteLine("`functionid` int(10) NOT NULL,")
        outputfile.WriteLine("`eventid` int(10) NOT NULL,")
        outputfile.WriteLine("`nanoid` int(10) NOT NULL,")
        outputfile.WriteLine("`argvalint` int(10),")
        outputfile.WriteLine("`argvalsingle` Float,")
        outputfile.WriteLine("`argvalstring` TEXT,")
        outputfile.WriteLine("PRIMARY KEY (`nanoid`,`eventid`,`functionid`,`attrid`) USING BTREE")
        outputfile.WriteLine(") ENGINE=MyISAM DEFAULT CHARSET=latin1;")
        outputfile.WriteLine("")

        outputfile.WriteLine("CREATE TABLE `nano_actions` (")
        outputfile.WriteLine("`actionid` int(10) NOT NULL,")
        outputfile.WriteLine("`nanoid` int(10) NOT NULL,")
        outputfile.WriteLine("`actionnum` int(10) NOT NULL,")
        outputfile.WriteLine("PRIMARY KEY (`nanoid`,`actionid`) USING BTREE")
        outputfile.WriteLine(") ENGINE=MyISAM DEFAULT CHARSET=latin1;")
        outputfile.WriteLine("")

        outputfile.WriteLine("CREATE TABLE `nano_action_reqs` (")
        outputfile.WriteLine("`reqid` int(10) NOT NULL,")
        outputfile.WriteLine("`actionid` int(10) NOT NULL,")
        outputfile.WriteLine("`nanoid` int(10) NOT NULL,")
        outputfile.WriteLine("`attrnum` int(10) NOT NULL,")
        outputfile.WriteLine("`attrval` int(10) NOT NULL,")
        outputfile.WriteLine("`operator` int(10) NOT NULL,")
        outputfile.WriteLine("`child_op` int(10) NOT NULL,")
        outputfile.WriteLine("`target` int(10) NOT NULL,")
        outputfile.WriteLine("PRIMARY KEY (`nanoid`,`actionid`,`reqid`) USING BTREE")
        outputfile.WriteLine(") ENGINE=MyISAM DEFAULT CHARSET=latin1;")
        outputfile.WriteLine("")

        outputfile.WriteLine("CREATE TABLE `nano_defense_attributes` (")
        outputfile.WriteLine("`defenseid` int(10) NOT NULL,")
        outputfile.WriteLine("`nanoid` int(10) NOT NULL,")
        outputfile.WriteLine("`num` int(10) NOT NULL,")
        outputfile.WriteLine("`value` int(10) NOT NULL,")
        outputfile.WriteLine("PRIMARY KEY (`nanoid`,`defenseid`,`num`) USING BTREE")
        outputfile.WriteLine(") ENGINE=MyISAM DEFAULT CHARSET=latin1;")
        outputfile.WriteLine("")

        outputfile.WriteLine("CREATE TABLE `nano_attack_attributes` (")
        outputfile.WriteLine("`attackid` int(10) NOT NULL,")
        outputfile.WriteLine("`nanoid` int(10) NOT NULL,")
        outputfile.WriteLine("`num` int(10) NOT NULL,")
        outputfile.WriteLine("`value` int(10) NOT NULL,")
        outputfile.WriteLine("PRIMARY KEY (`nanoid`,`attackid`,`num`) USING BTREE")
        outputfile.WriteLine(") ENGINE=MyISAM DEFAULT CHARSET=latin1;")
        outputfile.WriteLine("")

        outputfile.WriteLine("CREATE TABLE `nano_attributes` (")
        outputfile.WriteLine("`attributeid` int(10) NOT NULL,")
        outputfile.WriteLine("`nanoid` int(10) NOT NULL,")
        outputfile.WriteLine("`num` int(10) NOT NULL,")
        outputfile.WriteLine("`value` int(10) NOT NULL,")
        outputfile.WriteLine("PRIMARY KEY (`nanoid`,`attributeid`,`num`) USING BTREE")
        outputfile.WriteLine(") ENGINE=MyISAM DEFAULT CHARSET=latin1;")
        outputfile.WriteLine("")



    End Sub

    Public Sub Parse_End(ByVal Aborted As Boolean) _
        Implements XRDB4_Extras.Plugin.Parse_End

        outputfile.Close()
    End Sub

    Public Function ItemNano_Begin( _
            ByVal AOID As Integer, _
            ByVal IsNano As Boolean, _
            ByVal ChangeState As XRDB4_Extras.Plugin.ChangeStates) _
        As Boolean _
        Implements XRDB4_Extras.Plugin.ItemNano_Begin


        attributeid = 0
        attackattrid = 0
        defenseattrid = 0
        eventid = 0
        actionid = 0


        countactions = 0
        countevents = 0

        aoidholder = AOID

        ItAreNano = IsNano
        If IsNano Then
            outputfile.Write("INSERT INTO nanos VALUES (" + AOID.ToString() + ",1,")
        End If
        Return IsNano 'Yes, we want this record parsed
    End Function

    Public Sub ItemNano( _
            ByVal Info As XRDB4_Extras.Plugin.ItemNanoInfo, _
            ByVal Attributes() As XRDB4_Extras.Plugin.ItemNanoKeyVal) _
            Implements XRDB4_Extras.Plugin.ItemNano


        outputfile.WriteLine(Info.QL.ToString() + "," + Info.Type.ToString() + ");")

        attributeid = 0
        actionid = 0

        For Each Item As XRDB4_Extras.Plugin.ItemNanoKeyVal In Attributes

            outputfile.WriteLine("INSERT INTO nano_attributes VALUES (" + attributeid.ToString() + "," + aoidholder.ToString() + "," + Item.AttrKey.ToString() + "," + Item.AttrVal.ToString() + ");")
            attributeid += 1
        Next


    End Sub

    Public Sub ItemNano_End() Implements XRDB4_Extras.Plugin.ItemNano_End
        If attackval = "" Then attackval = "00000000"
        If defval = "" Then defval = "00000000"
        If attrval = "" Then attrval = "00000000"
        If funcs = "" Then funcs = "00000000"
        If actions = "" Then actions = "00000000"
        attackval = ""
        defval = ""
        attrval = ""
        args = ""
        funcs = ""
        actions = ""
        reqs = ""
        countactions = 0
        countevents = 0

    End Sub

    Public Sub ItemNanoAction( _
            ByVal ActionNum As Integer, _
            ByVal Requirements() As XRDB4_Extras.Plugin.ItemNanoRequirement) _
            Implements XRDB4_Extras.Plugin.ItemNanoAction


        outputfile.WriteLine("INSERT INTO nano_actions VALUES (" + actionid.ToString() + "," + aoidholder.ToString() + "," + ActionNum.ToString() + ");")



        actions += revit(ActionNum.ToString("X8"))
        actionreqid = 0
        For Each R As XRDB4_Extras.Plugin.ItemNanoRequirement In Requirements
            outputfile.WriteLine("INSERT INTO nano_action_reqs VALUES (" + actionreqid.ToString() + "," + actionid.ToString() + "," + aoidholder.ToString() + "," + R.AttrNum.ToString() + "," + R.AttrValue.ToString() + "," + R.MainOp.ToString() + "," + R.ChildOp.ToString() + "," + R.Target.ToString() + ");")
            actionreqid += 1
        Next
        actionid += 1
    End Sub


    Public Sub ItemNanoAttackAndDefense( _
            ByVal Attack() As XRDB4_Extras.Plugin.ItemNanoKeyVal, _
            ByVal Defense() As XRDB4_Extras.Plugin.ItemNanoKeyVal) _
            Implements XRDB4_Extras.Plugin.ItemNanoAttackAndDefense

        attackattrid = 0
        For Each DE As XRDB4_Extras.Plugin.ItemNanoKeyVal In Attack
            outputfile.WriteLine("INSERT INTO nano_attack_attributes VALUES (" + attackattrid.ToString() + "," + aoidholder.ToString() + "," + DE.AttrKey.ToString() + "," + DE.AttrVal.ToString() + ");")
            attackattrid += 1
        Next

        defenseattrid = 0
        For Each DE As XRDB4_Extras.Plugin.ItemNanoKeyVal In Defense
            outputfile.WriteLine("INSERT INTO nano_defense_attributes VALUES (" + defenseattrid.ToString() + "," + aoidholder.ToString() + "," + DE.AttrKey.ToString() + "," + DE.AttrVal.ToString() + ");")
            defenseattrid += 1
        Next

    End Sub

    Public Sub ItemNanoEventAndFunctions( _
            ByVal EventNum As Integer, _
            ByVal Functions() As XRDB4_Extras.Plugin.ItemNanoFunction) _
            Implements XRDB4_Extras.Plugin.ItemNanoEventAndFunctions


        Dim tempint As Int32
        Dim tempsingle As Single
        countevents += 1

        outputfile.WriteLine("INSERT INTO nano_events VALUES (" + eventid.ToString() + "," + aoidholder.ToString() + "," + EventNum.ToString() + ");")
        functionid = 0
        For Each F As XRDB4_Extras.Plugin.ItemNanoFunction In Functions

            outputfile.WriteLine("INSERT INTO nano_functions VALUES (" + functionid.ToString() + "," + eventid.ToString() + "," + aoidholder.ToString() + "," + F.FunctionNum.ToString() + "," + F.Target.ToString() + "," + F.TickCount.ToString() + "," + F.TickInterval.ToString() + ");")

            functionargid = 0

            For Each A As String In F.FunctionArgs

                If Int32.TryParse(A, tempint) Then
                    outputfile.WriteLine("INSERT INTO nano_function_arguments VALUES (" + functionargid.ToString() + "," + functionid.ToString() + "," + eventid.ToString() + "," + aoidholder.ToString() + "," + tempint.ToString() + ",NULL,NULL);")
                Else
                    If Single.TryParse(A, tempsingle) Then
                        outputfile.WriteLine("INSERT INTO nano_function_arguments VALUES (" + functionargid.ToString() + "," + functionid.ToString() + "," + eventid.ToString() + "," + aoidholder.ToString() + ",NULL," + tempsingle.ToString().Replace(",", ".") + ",NULL);")
                    Else
                        outputfile.WriteLine("INSERT INTO nano_function_arguments VALUES (" + functionargid.ToString() + "," + functionid.ToString() + "," + eventid.ToString() + "," + aoidholder.ToString() + ",NULL,NULL,'" + A.Replace("'", "''").TrimEnd(Convert.ToChar(0)) + "');")
                    End If
                End If
                functionargid += 1
            Next

            reqid = 0
            For Each R As XRDB4_Extras.Plugin.ItemNanoRequirement In F.FunctionReqs
                outputfile.WriteLine("INSERT INTO nano_function_reqs VALUES (" + reqid.ToString() + "," + functionid.ToString() + "," + eventid.ToString() + "," + aoidholder.ToString() + "," + R.AttrNum.ToString() + "," + R.AttrValue.ToString() + "," + R.MainOp.ToString() + "," + R.ChildOp.ToString() + "," + R.Target.ToString() + ");")
                reqid += 1
            Next
            functionid += 1
        Next
        eventid += 1
    End Sub

    Public Sub ItemNanoAnimSets(ByVal ActionNum As Integer, ByVal AnimData() As Integer) Implements XRDB4_Extras.Plugin.ItemNanoAnimSets
    End Sub

    Public Sub ItemNanoSoundSets(ByVal ActionNum As Integer, ByVal AnimData() As Integer) Implements XRDB4_Extras.Plugin.ItemNanoSoundSets
    End Sub

    Public Function OtherData_Begin( _
            ByVal AOID As Integer, _
            ByVal RecordType As Integer, _
            ByVal ChangeState As XRDB4_Extras.Plugin.ChangeStates) _
        As Boolean _
            Implements XRDB4_Extras.Plugin.OtherData_Begin

        Return False 'Not interested in other data
    End Function

    Public Sub OtherData(ByVal BinaryData() As Byte) Implements XRDB4_Extras.Plugin.OtherData
    End Sub
    Public Sub OtherData_End() Implements XRDB4_Extras.Plugin.OtherData_End
    End Sub

End Class




