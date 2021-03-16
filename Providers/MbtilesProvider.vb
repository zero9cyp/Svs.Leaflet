Imports System
Imports System.Collections.Generic
Imports System.Data.SQLite
Imports System.IO
Imports System.Linq
Imports System.Web
Imports System.Runtime.InteropServices

Public Class MbtilesProvider
    Private ReadOnly _connectionString As String

    Public Sub New(ByVal connectionString As String)
        _connectionString = connectionString
    End Sub

    Public Function GetTile(ByVal level As Integer, ByVal col As Integer, ByVal row As Integer) As MemoryStream
        Dim stream As MemoryStream = New MemoryStream()

        Using connection As SQLiteConnection = New SQLiteConnection(_connectionString)

            Using command = New SQLiteCommand(connection)
                Dim tmsCol As Integer = 0, tmsRow As Integer = 0
                ConvertGoogleTileToTMSTile(level, row, col, tmsRow, tmsCol)
                command.CommandText = "SELECT [tile_data] FROM [tiles] WHERE zoom_level = @zoom AND tile_column = @col AND tile_row = @row"
                command.Parameters.Add(New SQLiteParameter("zoom", level))
                command.Parameters.Add(New SQLiteParameter("col", tmsCol))
                command.Parameters.Add(New SQLiteParameter("row", tmsRow))
                connection.Open()
                Dim tileObj = command.ExecuteScalar()

                If tileObj IsNot Nothing Then
                    stream = New MemoryStream(CType(tileObj, Byte()))
                End If

                connection.Close()
            End Using
        End Using

        Return stream
    End Function

    Public Shared Sub ConvertGoogleTileToTMSTile(ByVal level As Integer, ByVal row As Integer, ByVal col As Integer, <Out> ByRef outRow As Integer, <Out> ByRef outCol As Integer)
        outCol = col
        outRow = (CInt((Math.Pow(2.0, level) - 1.0))) - row
    End Sub
End Class
