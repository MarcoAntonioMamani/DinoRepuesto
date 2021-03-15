
Imports Logica.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports System.Drawing
Imports DevComponents.DotNetBar.Controls
Imports DinoM.JanusExtension
Imports System.Transactions

Public Class F0_MovimientoNuevo
    Dim _Inter As Integer = 0
#Region "Variables Globales"
    Public _nameButton As String
    Public _tab As SuperTabItem
    Dim Lote As Boolean = False
    Public _modulo As SideNavItem
    Dim Table_producto As DataTable
    Dim FilaSelectLote As DataRow = Nothing
    Dim dtProductoGoblal As DataTable
    Dim tMovimiento As DataTable
#End Region
#Region "Metodos Privados"
    Private Sub _IniciarTodo()
        MSuperTabControl.SelectedTabIndex = 0
        _prValidarLote()
        _prCargarComboLibreriaConcepto(cbConcepto)
        _prCargarComboLibreriaDeposito(cbAlmacenOrigen)
        _prCargarComboLibreriaDeposito(cbDepositoDestino)
        _prCargarMovimiento()
        _prInhabiliitar()
        _prAsignarPermisos()
        Me.Text = "MOVIMIENTO PRODUCTOS"
        tbObservacion.MaxLength = 100
    End Sub
    Public Sub _prValidarLote()
        Dim dt As DataTable = L_fnPorcUtilidad()
        If (dt.Rows.Count > 0) Then
            Dim lot As Integer = dt.Rows(0).Item("VerLote")
            If (lot = 1) Then
                Lote = True
            Else
                Lote = False
            End If

        End If
    End Sub
    Private Sub _prCargarComboLibreriaDeposito(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_fnListarDepositos()
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("aanumi").Width = 60
            .DropDownList.Columns("aanumi").Caption = "COD"
            .DropDownList.Columns.Add("aabdes").Width = 500
            .DropDownList.Columns("aabdes").Caption = "SUCURSAL"
            .ValueMember = "aanumi"
            .DisplayMember = "aabdes"
            .DataSource = dt
            .Refresh()
        End With
    End Sub
    Private Sub _prCargarComboLibreriaConcepto(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_prMovimientoConcepto()
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("cpnumi").Width = 60
            .DropDownList.Columns("cpnumi").Caption = "COD"
            .DropDownList.Columns.Add("cpdesc").Width = 250
            .DropDownList.Columns("cpdesc").Caption = "CONCEPTO"
            .ValueMember = "cpnumi"
            .DisplayMember = "cpdesc"
            .DataSource = dt
            .Refresh()
        End With
    End Sub

    Private Sub _prAsignarPermisos()
        Dim dtRolUsu As DataTable = L_prRolDetalleGeneral(gi_userRol, _nameButton)
        Dim show As Boolean = dtRolUsu.Rows(0).Item("ycshow")
        Dim add As Boolean = dtRolUsu.Rows(0).Item("ycadd")
        Dim modif As Boolean = dtRolUsu.Rows(0).Item("ycmod")
        Dim del As Boolean = dtRolUsu.Rows(0).Item("ycdel")
        If add = False Then
            btnNuevo.Visible = False
        End If
        If modif = False Then
            btnModificar.Visible = False
        End If
        If del = False Then
            btnEliminar.Visible = False
        End If
    End Sub
    Private Sub _prInhabiliitar()

        cbConcepto.ReadOnly = True
        tbObservacion.ReadOnly = True
        tbFecha.IsInputReadOnly = True
        cbAlmacenOrigen.ReadOnly = True
        ''''''''''
        btnModificar.Enabled = True
        btnGrabar.Enabled = False
        btnNuevo.Enabled = True
        btnEliminar.Enabled = True
        grmovimiento.Enabled = True

        grdetalle.RootTable.Columns("stock").Visible = False
        grdetalle.RootTable.Columns("img").Visible = False
        grdetalle.RootTable.Columns("imgAdd").Visible = False

        PanelInferior.Enabled = True
        FilaSelectLote = Nothing
    End Sub
    Private Sub _prhabilitar()
        tbObservacion.ReadOnly = False
        tbFecha.IsInputReadOnly = False
        cbAlmacenOrigen.ReadOnly = False
        grmovimiento.Enabled = False
        If (tbCodigo.Text.Length > 0) Then
            cbAlmacenOrigen.ReadOnly = True
            cbConcepto.ReadOnly = True
        Else
            cbAlmacenOrigen.ReadOnly = False
            cbConcepto.ReadOnly = False
        End If

        ColArNro(grdetalle, "stock", "Stock", 80, "0.00")
        btnGrabar.Enabled = True
    End Sub
    Public Sub _prFiltrar()
        Dim _Mpos As Integer
        _prCargarMovimiento()
        If grmovimiento.RowCount > 0 Then
            _Mpos = 0
            grmovimiento.Row = _Mpos
        Else
            _Limpiar()
            LblPaginacion.Text = "0/0"
        End If
    End Sub
    Private Sub _Limpiar()
        tbCodigo.Clear()
        tbObservacion.Clear()
        tbFecha.Value = Now.Date
        cargarDetalle(-1)

        With grdetalle.RootTable.Columns("img")
            .Width = 70
            .Caption = "Eliminar"
            .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
            .Visible = True
        End With
        With grdetalle.RootTable.Columns("imgAdd")
            .Width = 70
            .Caption = "Nuevo"
            .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
            .Visible = True
        End With
        ColArNro(grdetalle, "stock", "Stock", 80, "0.00")

        If (CType(cbAlmacenOrigen.DataSource, DataTable).Rows.Count > 0) Then
            cbAlmacenOrigen.SelectedIndex = 0
        End If
        _prAddDetalleVenta()
        If (CType(cbConcepto.DataSource, DataTable).Rows.Count > 0) Then
            cbConcepto.SelectedIndex = 0
        End If

        cbConcepto.Focus()
        FilaSelectLote = Nothing
    End Sub
    Public Sub _prMostrarRegistro(_N As Integer)
        With grmovimiento
            tbCodigo.Text = .GetValue("Id")
            tbFecha.Value = .GetValue("Fecha")
            cbConcepto.Value = .GetValue("ConceptoId")
            tbObservacion.Text = .GetValue("Observacion")
            lbFecha.Text = CType(.GetValue("FechaReg"), Date).ToString("dd/MM/yyyy")
            lbHora.Text = .GetValue("HoraReg").ToString
            lbUsuario.Text = .GetValue("UsuarioReg").ToString
        End With
        cargarDetalle(tbCodigo.Text)
        LblPaginacion.Text = Str(grmovimiento.Row + 1) + "/" + grmovimiento.RowCount.ToString
    End Sub

    Private Sub _prCargarMovimiento()
        tMovimiento = l_obtenerMovimeintos()
        ConfigInicialVinculado(grmovimiento, tMovimiento, "Movimiento")
        ColAL(grmovimiento, "Id", "ITEM", 80)
        ColALFecha(grmovimiento, "Fecha", "FECHA", 90)
        ColNoVisible(grmovimiento, "ConceptoId")
        ColAL(grmovimiento, "Concepto", "Concepto".ToUpper, 200)
        ColAL(grmovimiento, "observacion", "observacion".ToUpper, 250)
        ColNoVisible(grmovimiento, "Estado")
        ColNoVisible(grmovimiento, "FechaReg")
        ColNoVisible(grmovimiento, "HoraReg")
        ColNoVisible(grmovimiento, "UsuarioReg")
        ConfigFinalBasica(grmovimiento)
        If (tMovimiento.Rows.Count <= 0) Then
            cargarDetalle(-1)
        End If
    End Sub
    Private Sub cargarDetalle(movimientoId As String)
        Try
            Dim tdetalle As New DataTable
            tdetalle = l_obtenerDetalleMovimiento(movimientoId)
            ConfigInicialVinculado(grdetalle, tdetalle, "Movimiento")
            ColNoVisible(grdetalle, "Id")
            ColNoVisible(grdetalle, "movimientoId")
            ColAL(grdetalle, "ProductoId", "Item", 60)
            ColAL(grdetalle, "CategoriaProducto", "Cat.Producto ", 120)
            ColAL(grdetalle, "CodigoFabrica", "Cod. Fabrica", 100)
            ColAL(grdetalle, "CodigoMarca", "Cod. Marca", 100)
            ColAL(grdetalle, "Medida", "Medida", 90)
            ColAL(grdetalle, "producto", "Producto", 250)
            ColArNro(grdetalle, "Cantidad", "Cantidad", 80, "0")
            ColCombo(grdetalle, "AlmOrigenId", "Alm. Origen", 150, cbAlmacenOrigen, True)
            ColCombo(grdetalle, "AlmDestinoId", "Alm. Destino", 150, cbDepositoDestino, IIf(cbConcepto.Value = 6, True, False))
            ColNoVisible(grdetalle, "img")
            ColNoVisible(grdetalle, "imgAdd")
            ColNoVisible(grdetalle, "estado")
            ColNoVisible(grdetalle, "stock")
            ConfigFinalDetalle(grdetalle)
        Catch ex As Exception
            MostrarMensajeError(ex.Message)
        End Try

    End Sub
    Public Sub actualizarSaldoSinLote(ByRef dt As DataTable)
        Dim _dtDetalle As DataTable = CType(grdetalle.DataSource, DataTable)
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim sum As Integer = 0
            Dim codProducto As Integer = dt.Rows(i).Item("Item")
            For j As Integer = 0 To _dtDetalle.Rows.Count - 1 Step 1

                Dim estado As Integer = _dtDetalle.Rows(j).Item("estado")
                If (estado = 0) Then
                    If (codProducto = _dtDetalle.Rows(j).Item("ProductoId")) Then
                        sum = sum + _dtDetalle.Rows(j).Item("Cantidad")
                    End If
                End If
            Next
            dt.Rows(i).Item("stock") = dt.Rows(i).Item("stock") - sum
        Next
    End Sub
    Private Sub _prCargarProductos()
        Dim dtname As DataTable = L_fnNameLabel()
        ' Using a As New Object]
        If dtProductoGoblal Is Nothing Then
            If (Lote = True And cbConcepto.Value <> 1) Then
                dtProductoGoblal = L_prMovimientoListarProductosConLote(cbAlmacenOrigen.Value)  ''1=Almacen
                actualizarSaldoSinLote(dtProductoGoblal)
                'dtProductoGoblal = dt
            Else
                dtProductoGoblal = L_prMovimientoListarProductos(cbAlmacenOrigen.Value)  ''1=Almacen
                'dtProductoGoblal = dt
            End If
        End If




        Dim dtMovimiento As DataTable = dtProductoGoblal.Copy
        dtMovimiento.Rows.Clear()
        Dim detalle As DataTable = CType(grdetalle.DataSource, DataTable)
        For i As Integer = 0 To detalle.Rows.Count - 1

            If (detalle.Rows(i).Item("estado") >= 0) Then
                Dim codigoProducto As Integer = detalle.Rows(i).Item("ProductoId")

                For j As Integer = 0 To dtProductoGoblal.Rows.Count - 1 Step 1

                    If (dtProductoGoblal.Rows(j).Item("Item") = codigoProducto) Then
                        dtProductoGoblal.Rows(j).Item("Cantidad") = detalle.Rows(i).Item("Cantidad")
                        'dt.Rows(j).Item("yhprecio") = detalle.Rows(i).Item("tbpbas")
                        dtMovimiento.ImportRow(dtProductoGoblal.Rows(j))
                    End If

                Next


            End If


        Next

        Dim frm As F0_DetalleMovimiento

        frm = New F0_DetalleMovimiento(dtProductoGoblal, dtMovimiento, dtname)
        frm.lbConcepto.Text = cbConcepto.Text
        frm.tbAlmacenNombre.Text = cbAlmacenOrigen.Text
        frm.ShowDialog()
        Dim dtProd As DataTable = frm.dtDetalle

        For i As Integer = 0 To dtProd.Rows.Count - 1 Step 1

            InsertarProductosSinLote(dtProd, i)
        Next

        dtMovimiento.Rows.Clear()
    End Sub
    Public Sub actualizarSaldo(ByRef dt As DataTable, CodProducto As Integer)

        Dim _detalle As DataTable = CType(grdetalle.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim lote As String = dt.Rows(i).Item("iclot")
            Dim FechaVenc As Date = dt.Rows(i).Item("icfven")
            Dim sum As Integer = 0
            For j As Integer = 0 To _detalle.Rows.Count - 1
                Dim estado As Integer = _detalle.Rows(j).Item("estado")
                If (estado = 0) Then
                    If (lote = _detalle.Rows(j).Item("iclot") And
                        FechaVenc = _detalle.Rows(j).Item("icfvenc") And CodProducto = _detalle.Rows(j).Item("ProductoId")) Then
                        sum = sum + _detalle.Rows(j).Item("Cantidad")
                    End If
                End If
            Next
            dt.Rows(i).Item("stock") = dt.Rows(i).Item("stock") - sum
        Next

    End Sub

    Private Sub _prAddDetalleVenta()
        Try
            Dim Bin As New MemoryStream
            Dim Bin02 As New MemoryStream
            Dim img As New Bitmap(My.Resources.delete, 28, 28)
            Dim img02 As New Bitmap(My.Resources.add, 28, 28)
            img.Save(Bin, Imaging.ImageFormat.Png)
            img02.Save(Bin02, Imaging.ImageFormat.Png)

            'CType(grdetalle.DataSource, DataTable).Columns.Add("eliminar", Type.GetType("System.Image"))
            'CType(grdetalle.DataSource, DataTable).Columns.Add("nuevo", Type.GetType("System.Byte[]"))
            'CType(grdetalle.DataSource, DataTable).Columns("Eliminar").AllowDBNull = True
            'CType(grdetalle.DataSource, DataTable).Columns("Nuevo").AllowDBNull = True
            'Dim data As Byte() = Bin.ToArray()
            'Dim ms = New MemoryStream(data)
            'Dim imagen = Image.FromStream(ms)

            'CType(grdetalle.DataSource, DataTable).Columns("img").DataType = System.Type.GetType(“System.Image”)
            'CType(grdetalle.DataSource, DataTable).Columns("imgAdd").DataType = System.Type.GetType(“System.Image”)
            'Dim imagena As Byte() = Bin.ToArray()
            ''Dim imagenn As Image = Image.FromStream(Bin)
            ''Dim a As ImageConverter = New ImageConverter()
            'Bin.Write(imagena, 0, imagena.Length)

            CType(grdetalle.DataSource, DataTable).Rows.Add(_fnSiguienteNumi() + 1, 0, 0, "", "", "", "", "", 0, 1, 1, Bin.GetBuffer(), Bin02.GetBuffer(), 0, 0)
            'CType(grdetalle.DataSource, DataTable).Rows("Eliminar")
            'CType(grdetalle.DataSource, DataTable).Rows(0).Add("Eliminar", Type.GetType("System.Iamge"))
            'CType(grdetalle.DataSource, DataTable).Columns.Add("Nuevo", Type.GetType("System.Iamge"))
            'CType(grdetalle.DataSource, DataTable).Columns("img") = a.ConvertTo(My.Resources.delete, System.Type.GetType("System.Byte[]"))

            ' ImageConverter.ConvertTo(Properties.Resources._1, System.Type.GetType("System.Byte[]"));
            '      var ImageConverter = New ImageConverter();
            ' row["Column1"] = imageConverter.ConvertTo(Properties.Resources._1, System.Type.GetType("System.Byte[]")); 

            '' Dim bs As System.IO.Stream
            ' Dim b(8000) As Byte

            ' bs = New System.IO.MemoryStream()
            ' b = dsFotos.Tables("Fotos").Rows(0)("Thumb")
            ' bs.Write(b, 0, b.Length)
            ' picImagen.Image = Image.FromStream(bs)



        Catch ex As Exception


            MostrarMensajeError(ex.Message)
        End Try


    End Sub
    Private Sub MostrarMensajeError(mensaje As String)
        ToastNotification.Show(Me,
                               mensaje.ToUpper,
                               My.Resources.WARNING,
                               5000,
                               eToastGlowColor.Red,
                               eToastPosition.TopCenter)

    End Sub
    Public Function _fnSiguienteNumi()
        Dim dt As DataTable = CType(grdetalle.DataSource, DataTable)
        Dim mayor As Integer = 0
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim data As Integer = IIf(IsDBNull(CType(grdetalle.DataSource, DataTable).Rows(i).Item("Id")), 0, CType(grdetalle.DataSource, DataTable).Rows(i).Item("Id"))
            If (data > mayor) Then
                mayor = data

            End If
        Next
        Return mayor
    End Function
    Public Function _fnAccesible()
        Return tbFecha.IsInputReadOnly = False
    End Function
    Private Sub _HabilitarProductos()
        _prCargarProductos()
    End Sub

    Public Sub _fnObtenerFilaDetalle(ByRef pos As Integer, numi As Integer)
        For i As Integer = 0 To CType(grdetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _numi As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("Id")
            If (_numi = numi) Then
                pos = i
                Return
            End If
        Next
    End Sub

    Public Function _fnExisteProducto(idprod As Integer) As Boolean
        For i As Integer = 0 To CType(grdetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _idprod As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("ProductoId")
            Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("estado")
            If (_idprod = idprod And estado >= 0) Then

                Return True
            End If
        Next
        Return False
    End Function
    Public Sub _prEliminarFila()
        If (grdetalle.Row >= 0) Then
            If (grdetalle.RowCount >= 2) Then
                Dim estado As Integer = grdetalle.GetValue("estado")
                Dim pos As Integer = -1
                Dim lin As Integer = grdetalle.GetValue("Id")
                _fnObtenerFilaDetalle(pos, lin)
                If (estado = 0) Then
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado") = -2

                End If
                If (estado = 1) Then
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado") = -1
                End If
                grdetalle.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grdetalle.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.GreaterThanOrEqualTo, 0))

                grdetalle.Select()
                grdetalle.Col = 4
                grdetalle.Row = grdetalle.RowCount - 1
            End If
        End If


    End Sub
    Public Function _ValidarCampos() As Boolean
        If (cbConcepto.SelectedIndex < 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Por Favor Seleccione un Concepto".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            cbConcepto.Focus()
            Return False

        End If
        If (cbAlmacenOrigen.SelectedIndex < 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Por Favor Seleccione un Deposito".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            cbAlmacenOrigen.Focus()
            Return False
        End If
        If (cbConcepto.SelectedIndex >= 0) Then
            If (cbConcepto.Value = 6) Then
                If (cbDepositoDestino.SelectedIndex < 0) Then
                    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                    ToastNotification.Show(Me, "Por Favor Seleccione un Deposito Desitno".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                    cbDepositoDestino.Focus()
                    Return False
                End If
            End If
        End If
        If (grdetalle.RowCount <= 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Por Favor Inserte un Detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            grdetalle.Focus()

            Return False
        End If
        If (grdetalle.RowCount = 1) Then
            If (CType(grdetalle.DataSource, DataTable).Rows(0).Item("ProductoId") = 0) Then
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "Por Favor Inserte un Detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                grdetalle.Focus()

                Return False
            End If
        End If

        Return True
    End Function
    Private Sub MostrarMensajeOk(mensaje As String)
        ToastNotification.Show(Me,
                               mensaje.ToUpper,
                               My.Resources.OK,
                               5000,
                               eToastGlowColor.Green,
                               eToastPosition.TopCenter)
    End Sub
    Sub _prGuardarTraspaso()
        Dim Id As String = ""
        Dim res As Boolean = L_prMovimientoChoferGrabar(Id, tbFecha.Value.ToString("yyyy/MM/dd"), cbConcepto.Value, tbObservacion.Text, cbAlmacenOrigen.Value, cbDepositoDestino.Value, 0, CType(grdetalle.DataSource, DataTable))
        If res Then

            Dim resDestino As Boolean = l_MovimientoGuardar(Id, tbFecha.Value.ToString("yyyy/MM/dd"), cbConcepto.Value, tbObservacion.Text, CType(grdetalle.DataSource, DataTable))
            If resDestino Then
                _prCargarMovimiento()
                _Limpiar()
                Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
                ToastNotification.Show(Me, "Código de Movimiento ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper,
                                          img, 2000,
                                          eToastGlowColor.Green,
                                          eToastPosition.TopCenter
                                          )
            End If

        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "El Movimiento no pudo ser insertado".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
        End If
    End Sub
    Public Sub _GuardarNuevo()
        Try
            Dim Id As String = ""
            Dim resultado As Boolean = False
            Using sb As New TransactionScope
                resultado = l_MovimientoGuardar(Id, tbFecha.Value.ToString("yyyy/MM/dd"), cbConcepto.Value,
                                                            tbObservacion.Text, CType(grdetalle.DataSource, DataTable))
                If resultado Then
                    _prCargarMovimiento()
                    _Limpiar()
                    MostrarMensajeOk("Código de Movimiento ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper)
                    sb.Complete()
                Else
                    MostrarMensajeError("El Movimiento no pudo ser insertado".ToUpper)
                End If
            End Using
        Catch ex As Exception
            MostrarMensajeError(ex.Message)
        End Try
    End Sub
    Private Sub _prGuardarModificado()
        Dim Id As String = ""
        Dim resultado As Boolean = False
        Using sb As New TransactionScope
            resultado = l_MovimientoModificar(tbCodigo.Text, tbFecha.Value.ToString("yyyy/MM/dd"), cbConcepto.Value,
                                                           tbObservacion.Text, CType(grdetalle.DataSource, DataTable))
            If resultado Then
                _prCargarMovimiento()
                _prSalir()
                MostrarMensajeOk("Código de Movimiento ".ToUpper + tbCodigo.Text + " Modificado con Exito.".ToUpper)
                sb.Complete()
            Else
                MostrarMensajeError("El Movimiento no pudo ser modificado".ToUpper)
            End If
        End Using
    End Sub
    Private Sub _prSalir()
        If btnGrabar.Enabled = True Then
            _prInhabiliitar()
            If grmovimiento.RowCount > 0 Then

                _prMostrarRegistro(0)

            End If
        Else

            _modulo.Select()
            Me.Close()

        End If
    End Sub
    Public Sub _prCargarIconELiminar()
        If (cbConcepto.Value <> 3) Then
            For i As Integer = 0 To CType(grdetalle.DataSource, DataTable).Rows.Count - 1 Step 1
                Dim Bin As New MemoryStream
                Dim img As New Bitmap(My.Resources.delete, 28, 28)
                img.Save(Bin, Imaging.ImageFormat.Png)
                CType(grdetalle.DataSource, DataTable).Rows(i).Item("img") = Bin.GetBuffer
                grdetalle.RootTable.Columns("img").Visible = True

                Dim Bin2 As New MemoryStream
                Dim img2 As New Bitmap(My.Resources.add, 28, 28)
                img2.Save(Bin2, Imaging.ImageFormat.Png)
                CType(grdetalle.DataSource, DataTable).Rows(i).Item("imgAdd") = Bin2.GetBuffer
                grdetalle.RootTable.Columns("imgAdd").Visible = True

            Next
        End If
    End Sub
    Public Sub _PrimerRegistro()
        Dim _MPos As Integer
        If grmovimiento.RowCount > 0 Then
            _MPos = 0
            ''   _prMostrarRegistro(_MPos)
            grmovimiento.Row = _MPos
        End If
    End Sub

    Public Sub InsertarProductosSinLote(dt As DataTable, fila As Integer)
        Dim pos As Integer = -1
        grdetalle.Row = grdetalle.RowCount - 1
        If (grdetalle.GetValue("ProductoId") <> 0) Then
            _prAddDetalleVenta()
            grdetalle.Row = grdetalle.RowCount - 1
        End If
        _fnObtenerFilaDetalle(pos, grdetalle.GetValue("Id"))

        Dim existe As Boolean = _fnExisteProducto(dt.Rows(fila).Item("Item"))
        If ((pos >= 0) And (Not existe)) Then
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("ProductoId") = dt.Rows(fila).Item("Item")

            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("CodigoFabrica") = dt.Rows(fila).Item("CodigoFabrica")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("CodigoMarca") = dt.Rows(fila).Item("Marca")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Medida") = dt.Rows(fila).Item("Medida")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("CategoriaProducto") = dt.Rows(fila).Item("Categoria")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("producto") = dt.Rows(fila).Item("yfcdprod1")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = dt.Rows(fila).Item("Cantidad")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("stock") = dt.Rows(fila).Item("stock")
        Else
            If (existe) Then
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "El producto ya existe en el detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            End If
        End If
    End Sub

    Public Function _fnExisteProductoConLote(idprod As Integer, lote As String, fechaVenci As Date) As Boolean
        For i As Integer = 0 To CType(grdetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _idprod As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("ProductoId")
            Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("estado")
            '          a.tbnumi ,a.tbtv1numi ,a.tbty5prod ,b.yfcdprod1 as producto,a.tbest ,a.tbcmin ,a.tbumin ,Umin .ycdes3 as unidad,a.tbpbas ,a.tbptot ,a.tbobs ,
            'a.tbpcos,a.tblote ,a.tbfechaVenc , a.tbptot2, a.tbfact ,a.tbhact ,a.tbuact,1 as estado,Cast(null as Image) as img,
            'Cast (0 as decimal (18,2)) as stock
            Dim _LoteDetalle As String = CType(grdetalle.DataSource, DataTable).Rows(i).Item("iclot")
            Dim _FechaVencDetalle As Date = CType(grdetalle.DataSource, DataTable).Rows(i).Item("icfvenc")
            If (_idprod = idprod And estado >= 0 And lote = _LoteDetalle And fechaVenci = _FechaVencDetalle) Then

                Return True
            End If
        Next
        Return False
    End Function



#End Region

#Region "Eventos Formulario"



    Private Sub F0_Movimiento_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _IniciarTodo()
        btnNuevo.PerformClick()
    End Sub

    Private Sub btnNuevo_Click(sender As Object, e As EventArgs) Handles btnNuevo.Click
        _Limpiar()
        _prhabilitar()

        btnNuevo.Enabled = False
        btnModificar.Enabled = False
        btnEliminar.Enabled = False
        btnGrabar.Enabled = True
        PanelInferior.Enabled = False
    End Sub

    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        _prSalir()
    End Sub

    Private Sub grdetalle_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grdetalle.EditingCell
        If (_fnAccesible()) Then

            'Habilitar solo las columnas de Precio, %, Monto y Observación
            If (e.Column.Index = grdetalle.RootTable.Columns("Cantidad").Index Or e.Column.Index = grdetalle.RootTable.Columns("AlmOrigenId").Index Or
                e.Column.Index = grdetalle.RootTable.Columns("AlmDestinoId").Index) Then
                e.Cancel = False
            Else
                e.Cancel = True
            End If
        Else
            e.Cancel = True
        End If
    End Sub
    Public Function SeleccionarCategoria(newItem As Boolean) As Integer
        Dim dt As DataTable
        Dim idCategoria As Integer = 0
        Dim nombreCategoria As String
        dt = L_fnListarCategoriaVentas()
        '   yccod3,ycdes3 

        Dim listEstCeldas As New List(Of Modelo.Celda)
        listEstCeldas.Add(New Modelo.Celda("yccod3,", True, "Codigo", 100))
        listEstCeldas.Add(New Modelo.Celda("ycdes3", True, "Nombre Categoria", 500))

        Dim ef = New Efecto
        ef.tipo = 3
        ef.dt = dt
        ef.SeleclCol = 2
        ef.listEstCeldas = listEstCeldas
        ef.alto = 50
        ef.ancho = 500
        ef.Context = "Seleccione Categoria".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row
            ''yccod3,ycdes3 
            idCategoria = Row.Cells("yccod3").Value
            nombreCategoria = Row.Cells("ycdes3").Value
            If (idCategoria > 0) Then
                'If (newItem = True) Then
                '    _prAddDetalleVenta()
                'End If
                _HabilitarProductos()
            End If
        End If
        Return idCategoria
    End Function
    Private Sub grdetalle_KeyDown(sender As Object, e As KeyEventArgs) Handles grdetalle.KeyDown
        If (Not _fnAccesible()) Then
            Return
        End If

        If (e.KeyData = Keys.Enter) Then
            Dim f, c As Integer
            c = grdetalle.Col
            f = grdetalle.Row

            If (grdetalle.Col = grdetalle.RootTable.Columns("Cantidad").Index) Then
                If (grdetalle.GetValue("producto") <> String.Empty) Then
                    '_prAddDetalleVenta()
                    _HabilitarProductos()
                    ' SeleccionarCategoria(True)
                Else
                    ToastNotification.Show(Me, "Seleccione un Producto Por Favor", My.Resources.WARNING, 3000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If

            End If
            If (grdetalle.Col = grdetalle.RootTable.Columns("producto").Index) Then
                If (grdetalle.GetValue("producto") <> String.Empty) Then
                    '_prAddDetalleVenta()
                    _HabilitarProductos()
                    'SeleccionarCategoria(True)
                Else
                    ToastNotification.Show(Me, "Seleccione un Producto Por Favor", My.Resources.WARNING, 3000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If

            End If
salirIf:
        End If

        If (e.KeyData = Keys.Control + Keys.Enter And grdetalle.Row >= 0 And
            grdetalle.Col = grdetalle.RootTable.Columns("producto").Index) Then
            Dim indexfil As Integer = grdetalle.Row
            Dim indexcol As Integer = grdetalle.Col
            _HabilitarProductos()
            'SeleccionarCategoria(True)
        End If
        If (e.KeyData = Keys.Escape And grdetalle.Row >= 0) Then

            _prEliminarFila()


        End If



    End Sub
    Private Sub grdetalle_CellValueChanged(sender As Object, e As ColumnActionEventArgs) Handles grdetalle.CellValueChanged
        Try
            'Celda Cantidad
            If (e.Column.Index = CelIndex(grdetalle, "Cantidad")) Then
                Dim cantidad As Integer = grdetalle.GetValue("Cantidad")
                Dim stock As Integer = grdetalle.GetValue("Stock")
                Dim posicion = obtenerPosicion()
                Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(posicion).Item("estado")
                If cbConcepto.Value = 2 And cbConcepto.Value = 6 Then
                    If cantidad > stock Then
                        CType(grdetalle.DataSource, DataTable).Rows(posicion).Item("Cantidad") = 1
                        grdetalle.SetValue("Cantidad", 1)
                        Throw New Exception("La cantidad que se quiere sacar es mayor a la que existe 
                                         en el stock solo puede Sacar : ".ToUpper + Str(stock).Trim)
                    End If
                End If

                If (Not IsNumeric(cantidad) Or
                    cantidad.ToString = String.Empty) Then

                    CType(grdetalle.DataSource, DataTable).Rows(posicion).Item("Cantidad") = 1
                    cambiarEstado(posicion, estado)
                Else
                    cambiarEstado(posicion, estado)
                    If (cantidad = 0) Then
                        CType(grdetalle.DataSource, DataTable).Rows(posicion).Item("Cantidad") = 1
                    End If
                End If
            End If

            'Celda Almacen de origen
            If (e.Column.Index = CelIndex(grdetalle, "AlmOrigenId")) Then
                Dim posicion = obtenerPosicion()
                Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(posicion).Item("estado")

                Dim productoId As Integer = grdetalle.GetValue("ProductoId")
                Dim almacenId As Integer = grdetalle.GetValue("AlmOrigenId")

                If productoId <> 0 And almacenId <> 0 Then
                    Dim stock = l_obtenerStockXAlmacenYProducto(almacenId, productoId)
                    SetCelValor(grdetalle, posicion, "Stock", stock)
                    seleccionarCeldaCantidad(posicion)
                    cambiarEstado(posicion, estado)
                End If
            End If
        Catch ex As Exception
            MostrarMensajeError(ex.Message)
        End Try
    End Sub
    Public Sub seleccionarCeldaCantidad(posicion As Integer)
        Dim celdadCantidad As Integer = 8
        grdetalle.Select()
        grdetalle.Col = celdadCantidad
        grdetalle.Row = posicion
    End Sub
    Private Sub cambiarEstado(posicion As Integer, estado As Integer)
        If (estado = 1) Then
            CType(grdetalle.DataSource, DataTable).Rows(posicion).Item("estado") = 2
        End If
    End Sub

    Public Function obtenerPosicion() As Integer
        Dim lin As Integer = grdetalle.GetValue("Id")
        Dim pos As Integer = -1
        _fnObtenerFilaDetalle(pos, lin)
        Return pos
    End Function
    Private Sub grdetalle_CellEdited(sender As Object, e As ColumnActionEventArgs) Handles grdetalle.CellEdited
        If (e.Column.Index = grdetalle.RootTable.Columns("Cantidad").Index) Then
            If (Not IsNumeric(grdetalle.GetValue("Cantidad")) Or grdetalle.GetValue("Cantidad").ToString = String.Empty) Then

                grdetalle.SetValue("Cantidad", 1)
            Else
                If (grdetalle.GetValue("Cantidad") > 0) Then
                    Dim stock As Double = grdetalle.GetValue("stock")
                    If (grdetalle.GetValue("Cantidad") > stock And cbConcepto.Value <> 1) Then
                        Dim lin As Integer = grdetalle.GetValue("Id")
                        Dim pos As Integer = -1
                        _fnObtenerFilaDetalle(pos, lin)
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = stock
                        grdetalle.SetValue("Cantidad", stock)
                        Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                        ToastNotification.Show(Me, "La cantidad que se quiere sacar es mayor a la que existe en el stock solo puede Sacar : ".ToUpper + Str(stock).Trim,
                          img,
                          5000,
                          eToastGlowColor.Blue,
                          eToastPosition.BottomLeft)
                    End If
                Else

                    grdetalle.SetValue("Cantidad", 1)

                End If
            End If
        End If
    End Sub

    Private Sub grdetalle_MouseClick(sender As Object, e As MouseEventArgs) Handles grdetalle.MouseClick
        If (Not _fnAccesible()) Then
            Return
        End If

        If (grdetalle.RowCount >= 2) Then
            If (grdetalle.CurrentColumn.Index = grdetalle.RootTable.Columns("img").Index) Then
                _prEliminarFila()
            End If
        End If
        Try
            If (grdetalle.CurrentColumn.Index = grdetalle.RootTable.Columns("imgAdd").Index) Then
                _HabilitarProductos()
                ' SeleccionarCategoria(True)
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub btnGrabar_Click(sender As Object, e As EventArgs) Handles btnGrabar.Click
        If _ValidarCampos() = False Then
            Exit Sub
        End If

        If (tbCodigo.Text = String.Empty) Then
            _GuardarNuevo()
        Else
            If (tbCodigo.Text <> String.Empty) Then
                _prGuardarModificado()
                ''    _prInhabiliitar()

            End If
        End If
    End Sub

    Private Sub btnModificar_Click(sender As Object, e As EventArgs) Handles btnModificar.Click
        If (grmovimiento.RowCount > 0) Then
            _prhabilitar()
            btnNuevo.Enabled = False
            btnModificar.Enabled = False
            btnEliminar.Enabled = False
            btnGrabar.Enabled = True

            PanelInferior.Enabled = False
            _prCargarIconELiminar()
        End If
    End Sub

    Private Sub btnEliminar_Click(sender As Object, e As EventArgs) Handles btnEliminar.Click
        Dim ef = New Efecto


        ef.tipo = 2
        ef.Context = "¿esta seguro de eliminar el registro?".ToUpper
        ef.Header = "mensaje principal".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean = L_prMovimientoEliminar(tbCodigo.Text)
            If res Then


                Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)

                ToastNotification.Show(Me, "Código de Movimiento ".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper,
                                          img, 2000,
                                          eToastGlowColor.Green,
                                          eToastPosition.TopCenter)

                _prFiltrar()

            Else
                Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
                ToastNotification.Show(Me, mensajeError, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            End If
        End If
    End Sub

    Private Sub btnSiguiente_Click(sender As Object, e As EventArgs) Handles btnSiguiente.Click
        Dim _pos As Integer = grmovimiento.Row
        If _pos < grmovimiento.RowCount - 1 Then
            _pos = grmovimiento.Row + 1
            '' _prMostrarRegistro(_pos)
            grmovimiento.Row = _pos
        End If
    End Sub

    Private Sub btnPrimero_Click(sender As Object, e As EventArgs) Handles btnPrimero.Click
        _PrimerRegistro()
    End Sub

    Private Sub btnUltimo_Click(sender As Object, e As EventArgs) Handles btnUltimo.Click
        Dim _pos As Integer = grmovimiento.Row
        If grmovimiento.RowCount > 0 Then
            _pos = grmovimiento.RowCount - 1
            ''  _prMostrarRegistro(_pos)
            grmovimiento.Row = _pos
        End If
    End Sub

    Private Sub btnAnterior_Click(sender As Object, e As EventArgs) Handles btnAnterior.Click
        Dim _MPos As Integer = grmovimiento.Row
        If _MPos > 0 And grmovimiento.RowCount > 0 Then
            _MPos = _MPos - 1
            ''  _prMostrarRegistro(_MPos)
            grmovimiento.Row = _MPos
        End If
    End Sub

    Private Sub grdetalle_Enter(sender As Object, e As EventArgs) Handles grdetalle.Enter
        If (grdetalle.RowCount > 0) Then
            grdetalle.Select()
            grdetalle.Col = 3
            grdetalle.Row = 0
        End If
    End Sub

    Private Sub cbAlmacen_KeyDown(sender As Object, e As KeyEventArgs) Handles cbAlmacenOrigen.KeyDown
        If (_fnAccesible()) Then
            If e.KeyData = Keys.Enter Then
                grdetalle.Focus()
                'grdetalle.Select()
                grdetalle.Col = 2
                grdetalle.Row = 0
            End If
        End If
    End Sub

    Private Sub cbConcepto_ValueChanged(sender As Object, e As EventArgs) Handles cbConcepto.ValueChanged


        If (cbConcepto.SelectedIndex >= 0) Then
            If (cbConcepto.Value = 6) Then ''''Movimiento 6=Traspaso Salida
                If (CType(cbAlmacenOrigen.DataSource, DataTable).Rows.Count > 1) Then
                    lbDepositoDestino.Visible = False
                    cbDepositoDestino.Visible = False
                    lbDepositoOrigen.Text = "Deposito Origen"
                    lbDepositoDestino.Text = "Deposito Destino"
                    cbDepositoDestino.SelectedIndex = 1
                    If grdetalle.RowCount > 0 Then
                        ColCombo(grdetalle, "AlmDestinoId", "Alm. Destino", 150, cbDepositoDestino, True)
                    End If

                    If (Not _fnAccesible()) Then
                        btnModificar.Enabled = False
                    End If
                Else
                    lbDepositoDestino.Visible = False
                    cbDepositoDestino.Visible = False
                    lbDepositoOrigen.Text = "Deposito:"
                    If (Not _fnAccesible()) Then
                        btnModificar.Enabled = True
                    End If
                End If
            Else
                btnModificar.Enabled = True
                lbDepositoDestino.Visible = False
                cbDepositoDestino.Visible = False
                lbDepositoOrigen.Text = "Deposito:"
                If grdetalle.RowCount > 0 Then
                    ColCombo(grdetalle, "AlmDestinoId", "Alm. Destino", 150, cbDepositoDestino, False)
                End If
            End If
            If (_fnAccesible() And tbCodigo.Text = String.Empty) Then
                CType(grdetalle.DataSource, DataTable).Rows.Clear()
                _prAddDetalleVenta()
            End If
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        _Inter = _Inter + 1
        If _Inter = 1 Then
            Me.WindowState = FormWindowState.Normal
        Else
            Me.Opacity = 100
            Timer1.Enabled = False
        End If
    End Sub

    Private Sub grProductos_DoubleClick(sender As Object, e As EventArgs)

    End Sub

    Private Sub grmovimiento_SelectionChanged(sender As Object, e As EventArgs) Handles grmovimiento.SelectionChanged
        If (grmovimiento.RowCount >= 0 And grmovimiento.Row >= 0) Then
            _prMostrarRegistro(grmovimiento.Row)
        End If
    End Sub





#End Region
End Class