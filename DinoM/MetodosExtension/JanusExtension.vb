Imports System.Drawing
Imports Janus.Windows
Imports Janus.Windows.GridEX
Imports Janus.Windows.GridEX.EditControls

Public Class JanusExtension

#Region "Griex"
    Public Shared Sub ConfigInicialVinculado(grid As GridEX, tabla As DataTable, nombre As String)
        grid.BoundMode = BoundMode.Bound
        grid.SetDataBinding(tabla, nombre)
        grid.RetrieveStructure()

    End Sub
#Region "Columnas"
    Public Shared Sub ColNoVisible(grid As GridEX, key As String)
        With grid.RootTable.Columns(key)
            .Visible = False
        End With
    End Sub

    Public Shared Sub ColAL(grid As GridEX, key As String, nombre As String, ancho As Integer, Optional tamFuente As Integer = 8)
        With grid.RootTable.Columns(key)
            .CellStyle.TextAlignment = TextAlignment.Near
            .WordWrap = True
            .MaxLines = 20
            .Visible = True
            ColPropiedadesComun(grid, key, nombre, ancho)
        End With
    End Sub
    Public Shared Sub ColCombo(grid As GridEX, key As String, nombre As String, ancho As Integer, combo As MultiColumnCombo, visible As Boolean, Optional tamFuente As Integer = 8)
        With grid.RootTable.Columns(key)
            .CellStyle.TextAlignment = TextAlignment.Near
            .Visible = visible
            .EditType = EditType.MultiColumnCombo
            .DropDown = combo.DropDownList
            ColPropiedadesComun(grid, key, nombre, ancho)
        End With
    End Sub
    Public Shared Sub ColALFecha(grid As GridEX, key As String, nombre As String, ancho As Integer, Optional tamFuente As Integer = 8)
        With grid.RootTable.Columns(key)
            .CellStyle.TextAlignment = TextAlignment.Near
            .FormatString = "yyyy/MM/dd"
            ColPropiedadesComun(grid, key, nombre, ancho)
        End With
    End Sub
    Public Shared Sub ColAC(grid As GridEX, key As String, nombre As String, ancho As Integer, Optional tamFuente As Integer = 8)
        With grid.RootTable.Columns(key)
            .CellStyle.TextAlignment = TextAlignment.Center
            .Visible = True
            ColPropiedadesComun(grid, key, nombre, ancho)
        End With
    End Sub
    Public Shared Sub ColArNro(grid As GridEX, key As String, nombre As String, ancho As Integer, formato As String, Optional tamFuente As Integer = 8)
        With grid.RootTable.Columns(key)
            .CellStyle.TextAlignment = TextAlignment.Far
            .FormatString = formato
            .Visible = True
            ColPropiedadesComun(grid, key, nombre, ancho)
        End With
    End Sub
    Public Shared Sub ColIcon(grid As GridEX, key As String, posicion As Integer, nombre As String, imagen As Image, Optional ancho As Integer = 40)
        With grid.RootTable.Columns(key)
            .Position = posicion
            ColPropiedadesComun(grid, key, nombre, ancho)
        End With
    End Sub
    Public Shared Sub ColIcon(grid As GridEX, key As String, nombre As String, imagen As Image, Optional ancho As Integer = 40)
        With grid.RootTable.Columns(key)
            ColPropiedadesComun(grid, key, nombre, ancho)
        End With
    End Sub
#End Region
#End Region
#Region "Configuracion Final"
    Public Shared Sub ConfigFinalCompleto(grid As GridEX, Optional fijarColumnas As Integer = 0)
        With grid
            .GroupByBoxVisible = True
            .FilterRowButtonStyle = FilterRowButtonStyle.ConditionOperatorDropDown
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .VisualStyle = VisualStyle.Office2007
            .SelectionMode = Janus.Windows.GridEX.SelectionMode.MultipleSelection
            .AlternatingColors = True
            .RecordNavigator = True
            .AllowEdit = InheritableBoolean.False
            .AllowColumnDrag = True
            .AutomaticSort = True
            .RootTable.HeaderLines = 2
            .RootTable.RowHeight = 20
            .CellSelectionMode = CellSelectionMode.SingleCell
            .RowHeaders = InheritableBoolean.True
            .RowHeaderContent = RowHeaderContent.RowIndex
            .GroupByBoxInfoText = "Arrastre un encabezado de columna aquí para agrupar por esa columna"
            If fijarColumnas > 0 Then
                grid.FrozenColumns = fijarColumnas
            End If
        End With
    End Sub
    Public Shared Sub ConfigFinalBasica(grid As GridEX, Optional fijarColumnas As Integer = 0)
        With grid
            .GroupByBoxVisible = False
            .AlternatingColors = True
            .FilterRowButtonStyle = FilterRowButtonStyle.ConditionOperatorDropDown
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .VisualStyle = VisualStyle.Office2007
            .AllowEdit = InheritableBoolean.False
        End With
    End Sub
    Public Shared Sub ConfigFinalDetalle(grid As GridEX)
        With grid
            .GroupByBoxVisible = False
            .AlternatingColors = True
            .FilterMode = FilterMode.None
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .VisualStyle = VisualStyle.Office2007
        End With
    End Sub
#End Region
#Region "Ayuda"
    Public Shared Function CelValor(Of T)(grid As GridEX, key As String) As T
        Return grid.CurrentRow.Cells(key).Value
    End Function
    Public Shared Sub SetCelValor(grid As GridEX, posicion As Integer, key As String, valor As String)
        CType(grid.DataSource, DataTable).Rows(posicion).Item(key) = valor
    End Sub
    Public Shared Function GetCelValor(Of T)(grid As GridEX, posicion As Integer, key As String) As T
        Return CType(grid.DataSource, DataTable).Rows(posicion).Item(key)
    End Function
    Public Shared Function CelIndex(grid As GridEX, key As String) As Integer
        Return grid.RootTable.Columns(key).Index
    End Function

#End Region

#Region "Metodos Privados"
    Public Shared Sub ColPropiedadesComun(grid As GridEX, key As String, nombre As String, ancho As Integer)
        With grid.RootTable.Columns(key)
            .Width = ancho
            .Caption = nombre
            .HeaderAlignment = TextAlignment.Center
        End With
    End Sub

    Public Shared Sub ColIconPropiedadesComun(grid As GridEX, key As String, nombre As String, imagen As Image, ancho As Integer)
        With grid.RootTable.Columns.Add(key, ColumnType.Image)
            .Width = ancho
            .Caption = nombre
            .HeaderAlignment = TextAlignment.Center
            .Image = imagen
            .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
        End With
    End Sub
#End Region
#Region "MultiColumnCombo"
    Public Shared Sub seleccionarPrimero(combo As MultiColumnCombo)
        Try
            combo.SelectedIndex = 0
        Catch ex As Exception

        End Try
    End Sub
#End Region

End Class
