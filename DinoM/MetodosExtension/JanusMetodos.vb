Imports System.Drawing
Imports System.IO
Imports DevComponents.DotNetBar
Imports Janus.Windows
Imports Janus.Windows.GridEX
Imports Janus.Windows.GridEX.EditControls
Public Class JanusMetodos
    Public Shared Function jMetodo_SumarFila(grid As GridEX, key As String) As Double
        Dim stock As Double = 0
        For Each fila As DataRow In CType(grid.DataSource, DataTable).Rows
            stock += fila(key)
        Next
        Return stock
    End Function
End Class
