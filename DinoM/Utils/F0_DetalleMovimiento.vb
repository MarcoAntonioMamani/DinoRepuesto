Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX

Public Class F0_DetalleMovimiento
    Public dtProductoAll As DataTable
    Public dtDetalle As DataTable
    Public dtname As DataTable

    Public CategoriaPrecio As Integer

    Public Sub IniciarTodod()
        CargarProductos()
        CargarProductosVentas()

        tbProducto.Focus()
    End Sub

    Public Sub New(dtp As DataTable, dtv As DataTable, dtn As DataTable)
        InitializeComponent()
        dtProductoAll = dtp
        dtDetalle = dtv
        dtname = dtn
    End Sub

    Public Sub CargarProductosVentas()
        Dim bandera As Boolean = False
        Dim dt As DataTable
        If (IsNothing(dtDetalle)) Then

            bandera = True
            grProductoSeleccionado.DataSource = dtProductoAll

        Else
            grProductoSeleccionado.DataSource = dtDetalle

        End If



        grProductoSeleccionado.RetrieveStructure()
        grProductoSeleccionado.AlternatingColors = True


        With grProductoSeleccionado.RootTable.Columns("Item")
            .Width = 100
            .Caption = "CODIGO"
            .Visible = False

        End With
        With grProductoSeleccionado.RootTable.Columns("CodigoFabrica")
            .Width = 100
            .Caption = "CodigoFabrica"
            .MaxLines = 100
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = True
        End With
        With grProductoSeleccionado.RootTable.Columns("Medida")
            .Width = 90
            .Caption = "Medida"
            .MaxLines = 100
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = gb_CodigoBarra
        End With
        With grProductoSeleccionado.RootTable.Columns("Marca")
            .Width = 90
            .Caption = "Cod.Marca"
            .MaxLines = 100
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = True
        End With
        With grProductoSeleccionado.RootTable.Columns("Categoria")
            .Width = 100
            .Caption = "Categoria"
            .MaxLines = 100
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = True
        End With
        With grProductoSeleccionado.RootTable.Columns("yfcdprod1")
            .Width = 350
            .Caption = "PRODUCTOS"
            .Visible = True

        End With
        With grProductoSeleccionado.RootTable.Columns("yfgr1")
            .Width = 160
            .Visible = False
        End With

        If (dtname.Rows.Count > 0) Then

            With grProductoSeleccionado.RootTable.Columns("grupo1")
                .Width = 120
                .Caption = dtname.Rows(0).Item("Grupo 1").ToString
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = True
            End With
            With grProductoSeleccionado.RootTable.Columns("grupo2")
                .Width = 120
                .Caption = dtname.Rows(0).Item("Grupo 2").ToString
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = True
            End With

            With grProductoSeleccionado.RootTable.Columns("Laboratorio")
                .Width = 120
                .Caption = dtname.Rows(0).Item("Grupo 3").ToString
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = False
            End With
            With grProductoSeleccionado.RootTable.Columns("Presentacion")
                .Width = 120
                .Caption = dtname.Rows(0).Item("Grupo 4").ToString
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = False
            End With
        Else
            With grProductoSeleccionado.RootTable.Columns("grupo1")
                .Width = 120
                .Caption = "Grupo 1"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = True
            End With
            With grProductoSeleccionado.RootTable.Columns("grupo2")
                .Width = 120
                .Caption = "Grupo 2"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = False
            End With
            With grProductoSeleccionado.RootTable.Columns("Laboratorio")
                .Width = 120
                .Caption = "Grupo 3"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = False
            End With
            With grProductoSeleccionado.RootTable.Columns("Presentacion")
                .Width = 120
                .Caption = "Grupo 4"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = False
            End With
        End If


        With grProductoSeleccionado.RootTable.Columns("yfgr2")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With grProductoSeleccionado.RootTable.Columns("yfgr3")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With grProductoSeleccionado.RootTable.Columns("yfgr4")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grProductoSeleccionado.RootTable.Columns("yfumin")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grProductoSeleccionado.RootTable.Columns("UnidMin")
            .Width = 120
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
            .Caption = "Unidad Min."
        End With
        With grProductoSeleccionado.RootTable.Columns("stock")
            .Width = 150
            .Visible = True
            .FormatString = "0.00"
            .Caption = "STOCK"
        End With
        With grProductoSeleccionado.RootTable.Columns("Cantidad")
            .Width = 150
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Cantidad"
        End With

        With grProductoSeleccionado
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            ' diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
        End With
        If (bandera = True) Then
            CType(grProductoSeleccionado.DataSource, DataTable).Rows.Clear()

        End If
    End Sub
    Public Sub CargarProductos()
        grProductos.DataSource = dtProductoAll.Copy
        grProductos.RetrieveStructure()
        grProductos.AlternatingColors = True


        With grProductos.RootTable.Columns("item")
            .Width = 50
            .Caption = "codigo".ToUpper
            .Visible = True

        End With
        With grProductos.RootTable.Columns("categoria")
            .Width = 100
            .Caption = "categoria".ToUpper
            .MaxLines = 100
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = True
        End With
        With grProductos.RootTable.Columns("codigofabrica")
            .Width = 100
            .Caption = "codigo DE fabrica".ToUpper
            .MaxLines = 100
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = True
        End With
        With grProductos.RootTable.Columns("medida")
            .Width = 90
            .Caption = "medida".ToUpper
            .MaxLines = 100
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = gb_CodigoBarra
        End With
        With grProductos.RootTable.Columns("marca")
            .Width = 90
            .Caption = "cod.marca".ToUpper
            .MaxLines = 100
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = True
        End With

        With grProductos.RootTable.Columns("yfcdprod1")
            .Width = 350
            .Caption = "productos".ToUpper
            .Visible = True

        End With
        With grProductos.RootTable.Columns("yfgr1")
            .Width = 160
            .Visible = False
        End With

        If (dtname.Rows.Count > 0) Then

            With grProductos.RootTable.Columns("grupo1")
                .Width = 120
                .Caption = dtname.Rows(0).Item("grupo 1").ToString.ToUpper
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = True
            End With
            With grProductos.RootTable.Columns("grupo2")
                .Width = 120
                .Caption = dtname.Rows(0).Item("grupo 2").ToString.ToUpper
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = True
            End With

            With grProductos.RootTable.Columns("laboratorio")
                .Width = 120
                .Caption = dtname.Rows(0).Item("grupo 3").ToString.ToUpper
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = False
            End With
            With grProductos.RootTable.Columns("presentacion")
                .Width = 120
                .Caption = dtname.Rows(0).Item("grupo 4").ToString
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = False
            End With
        Else
            With grProductos.RootTable.Columns("grupo1")
                .Width = 120
                .Caption = "grupo 1".ToUpper
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = True
            End With
            With grProductos.RootTable.Columns("grupo2")
                .Width = 120
                .Caption = "grupo 2".ToUpper
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = False
            End With
            With grProductos.RootTable.Columns("laboratorio")
                .Width = 120
                .Caption = "grupo 3".ToUpper
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = False
            End With
            With grProductos.RootTable.Columns("presentacion")
                .Width = 120
                .Caption = "grupo 4".ToUpper
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = False
            End With
        End If


        With grProductos.RootTable.Columns("yfgr2")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With grProductos.RootTable.Columns("yfgr3")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With grProductos.RootTable.Columns("yfgr4")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grProductos.RootTable.Columns("yfumin")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grProductos.RootTable.Columns("unidmin")
            .Width = 120
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
            .Caption = "unidad min."
        End With
        With grProductos.RootTable.Columns("stock")
            .Width = 150
            .Visible = False
            .FormatString = "0.00"
            .Caption = "stock".ToUpper
        End With
        With grProductos.RootTable.Columns("cantidad")
            .Width = 150
            .Visible = False
            .FormatString = "0.00"
            .Caption = "cantidad"
        End With
        With grProductos.RootTable.Columns("ListaAlmacen")
            .Width = 200
            .Caption = "Stock".ToUpper
            .MaxLines = 100
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = True
        End With
        With grProductos
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
        End With

        _prAplicarCondiccionJanusSinLote()
    End Sub
    Public Sub _prAplicarCondiccionJanusSinLote()
        Dim fc As GridEXFormatCondition
        fc = New GridEXFormatCondition(grProductos.RootTable.Columns("stock"), ConditionOperator.Between, -9998 And 0)
        'fc.FormatStyle.FontBold = TriState.True
        fc.FormatStyle.ForeColor = Color.Red    'Color.Tan
        grProductos.RootTable.FormatConditions.Add(fc)
        Dim fr As GridEXFormatCondition
        fr = New GridEXFormatCondition(grProductos.RootTable.Columns("stock"), ConditionOperator.Equal, -9999)
        fr.FormatStyle.ForeColor = Color.BlueViolet
        grProductos.RootTable.FormatConditions.Add(fr)
    End Sub
    Private Sub F0_DetalleVenta_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        IniciarTodod()
        tbProducto.Focus()
    End Sub

    Public Function ExisteProducto(Id As Integer) As Boolean
        Dim dt As DataTable = CType(grProductoSeleccionado.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            If (dt.Rows(i).Item("Item") = Id) Then
                Return True
            End If



        Next
        Return False
    End Function

    Private Sub grProductos_KeyDown(sender As Object, e As KeyEventArgs) Handles grProductos.KeyDown

        If (e.KeyData = Keys.Enter) Then
            Dim f, c As Integer
            c = grProductos.Col
            f = grProductos.Row
            If (f >= 0) Then

                If (Not ExisteProducto(grProductos.GetValue("Item"))) Then
                    If (grProductos.GetValue("Stock") <= 0) Then

                        ToastNotification.Show(Me, "El Producto no Cuenta con Stock Disponible", My.Resources.WARNING, 4000, eToastGlowColor.Red, eToastPosition.TopCenter)
                        Return

                    End If
                    Dim cantidad As Double
                    'cantidad = InputBox("Seleccione Cantidad del Producto " + grProductos.GetValue("yfcdprod1") + "  " + "   Stock = " + Str(grProductos.GetValue("Stock")))

                    'If (cantidad <= grProductos.GetValue("Stock")) Then

                    '    Dim row As DataRow = CType(grProductos.DataSource, DataTable).Rows(f)

                    '    row.Item("Cantidad") = cantidad
                    '    CType(grProductoSeleccionado.DataSource, DataTable).ImportRow(row)
                    '    tbProducto.Focus()
                    'Else
                    '    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                    '    ToastNotification.Show(Me, "La cantidad es mayor al Stock".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                    '    tbProducto.Focus()

                    'End If

                    Dim ef = New Efecto
                    ef.tipo = 7
                    ef.Stock = grProductos.GetValue("Stock")
                    ef.Cantidad = 0
                    ef.CategoriaPrecio = CategoriaPrecio
                    ef.IdProducto = grProductos.GetValue("Item")
                    ef.NameProducto = grProductos.GetValue("yfcdprod1")

                    ef.ShowDialog()
                    Dim bandera As Boolean = False
                    bandera = ef.band
                    If (bandera = True) Then

                        cantidad = ef.Cantidad
                        If (cantidad > 0) Then
                            Dim row As DataRow = CType(grProductos.DataSource, DataTable).Rows(f)

                            row.Item("Cantidad") = cantidad
                            'If (CategoriaPrecio = 50) Then
                            '    row.Item("yhprecio") = ef.Precio
                            'End If

                            CType(grProductoSeleccionado.DataSource, DataTable).ImportRow(row)
                            tbProducto.Focus()

                        End If

                    Else
                        ToastNotification.Show(Me, "Debe ingresar una cantidad Valida para insertar al detalle", My.Resources.WARNING, 4000, eToastGlowColor.Red, eToastPosition.TopCenter)



                    End If
                Else
                    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                    ToastNotification.Show(Me, "El producto ya existe en el detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)

                    tbProducto.Focus()
                End If

            End If
        End If
        If e.KeyData = Keys.Escape Then
            Me.Close()

        End If
    End Sub

    Private Sub tbProducto_TextChanged(sender As Object, e As EventArgs) Handles tbProducto.TextChanged
        Dim dtProductoCopy As DataTable
        dtProductoCopy = dtProductoAll.Copy
        dtProductoCopy.Rows.Clear()
        Dim dt As DataTable = dtProductoAll.Copy

        Dim charSequence As String
        charSequence = tbProducto.Text.ToUpper
        If (charSequence.Trim <> String.Empty) Then
            Dim cantidad As Integer = 12
            Dim cont As Integer = 12

            'Split con array de delimitadores
            Dim delimitadores() As String = {" ", ".", ",", ";", "-"}
            Dim vectoraux() As String
            vectoraux = charSequence.Split(delimitadores, StringSplitOptions.None)

            'mostrar resultado
            'For Each item As String In vectoraux


            '    Console.WriteLine("'{0}'", item)
            'Next
            Dim cant As Integer = vectoraux.Length

            For i As Integer = 0 To dt.Rows.Count - 1 Step 1
                Dim nombre As String = dt.Rows(i).Item("yfcdprod1").ToString.ToUpper +
                    " " + dt.Rows(i).Item("CodigoFabrica").ToString.ToUpper +
                    " " + dt.Rows(i).Item("Marca").ToString.ToUpper +
                    " " + dt.Rows(i).Item("grupo1").ToString.ToUpper +
                    " " + dt.Rows(i).Item("grupo2").ToString.ToUpper +
                    " " + dt.Rows(i).Item("Medida").ToString.ToUpper
                Select Case cant
                    Case 1

                        If (nombre.Trim.Contains(vectoraux(0))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If

                    Case 2
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 3
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 4
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 5
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 6
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If

                    Case 7

                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 8
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 9
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 10
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If

                    Case 11
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If

                    Case 12
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If


                    Case 13
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 14
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 15
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13)) And nombre.Trim.Contains(vectoraux(14))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 16
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13)) And nombre.Trim.Contains(vectoraux(14)) And nombre.Trim.Contains(vectoraux(15))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 17
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13)) And nombre.Trim.Contains(vectoraux(14)) And nombre.Trim.Contains(vectoraux(15)) And nombre.Trim.Contains(vectoraux(16))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 18
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13)) And nombre.Trim.Contains(vectoraux(14)) And nombre.Trim.Contains(vectoraux(15)) And nombre.Trim.Contains(vectoraux(16)) And nombre.Trim.Contains(vectoraux(17))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If



                End Select

            Next
            grProductos.DataSource = dtProductoCopy.Copy
        Else
            grProductos.DataSource = dtProductoAll.Copy
        End If



    End Sub
    Private Sub tbProducto_KeyDown(sender As Object, e As KeyEventArgs) Handles tbProducto.KeyDown
        If e.KeyData = Keys.Escape Then
            Me.Close()

        End If
        If e.KeyData = Keys.Down Then
            grProductos.Focus()
        End If

    End Sub

    Private Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click
        Me.Close()

    End Sub

    Private Sub grProductos_DoubleClick(sender As Object, e As EventArgs) Handles grProductos.DoubleClick

        Dim f, c As Integer
        c = grProductos.Col
        f = grProductos.Row
        If (f >= 0) Then

            If (Not ExisteProducto(grProductos.GetValue("Item"))) Then

                If (grProductos.GetValue("Stock") <= 0) Then

                    ToastNotification.Show(Me, "El Producto no Cuenta con Stock Disponible", My.Resources.WARNING, 4000, eToastGlowColor.Red, eToastPosition.TopCenter)
                    Return

                End If
                Dim cantidad As Double


                Dim ef = New Efecto
                ef.tipo = 7
                ef.Stock = grProductos.GetValue("Stock")
                ef.Cantidad = 0
                ef.CategoriaPrecio = CategoriaPrecio
                ef.IdProducto = grProductos.GetValue("Item")
                ef.NameProducto = grProductos.GetValue("yfcdprod1")


                ef.ShowDialog()
                Dim bandera As Boolean = False
                bandera = ef.band
                If (bandera = True) Then
                    cantidad = ef.Cantidad
                    If (cantidad > 0) Then

                        Dim dt As DataTable = CType(grProductos.DataSource, DataTable).Copy
                        Dim row As DataRow = dt.Rows(f)

                        'row.Item("Cantidad") = cantidad
                        'If (CategoriaPrecio = 50) Then
                        '    row.Item("yhprecio") = ef.Precio
                        'End If

                        CType(grProductoSeleccionado.DataSource, DataTable).ImportRow(row)
                        tbProducto.Focus()

                    End If

                Else
                    ToastNotification.Show(Me, "Debe ingresar una cantidad Valida para insertar al detalle", My.Resources.WARNING, 4000, eToastGlowColor.Red, eToastPosition.TopCenter)



                End If



            Else
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "El producto ya existe en el detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)

                tbProducto.Focus()
            End If

        End If
    End Sub

End Class