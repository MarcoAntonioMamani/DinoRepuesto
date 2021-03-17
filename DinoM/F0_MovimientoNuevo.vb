﻿Imports Logica.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports System.Drawing
Imports DevComponents.DotNetBar.Controls
Imports System.Transactions
Imports DinoM.JanusExtension
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
        If dtProductoGoblal Is Nothing Then
            If (Lote = True And cbConcepto.Value <> 1) Then
                dtProductoGoblal = L_prMovimientoListarProductosConLote(cbAlmacenOrigen.Value)
                actualizarSaldoSinLote(dtProductoGoblal)
            Else
                dtProductoGoblal = L_prMovimientoListarProductos(cbAlmacenOrigen.Value)
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
            ObtenerImagenAddDetalle(Bin, Bin02)
            'Obtiene el Id Mayor
            Dim idMayor As Integer = ObtenerIdMayor(grdetalle, "Id")
            CType(grdetalle.DataSource, DataTable).Rows.Add(idMayor + 1, 0, 0, "", "", "", "", "", 0, 1, 1, Bin.GetBuffer(), Bin02.GetBuffer(), 0, 0)
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
    Private Sub MostrarMensaje(mensaje As String)
        ToastNotification.Show(Me,
                               mensaje.ToUpper,
                               My.Resources.mensaje,
                               2000,
                               eToastGlowColor.Red,
                               eToastPosition.TopCenter)

    End Sub
    Public Function _fnAccesible()
        Return tbFecha.IsInputReadOnly = False
    End Function
    Private Sub _HabilitarProductos()
        _prCargarProductos()
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
    Public Function _ValidarCampos() As Boolean
        If (cbConcepto.SelectedIndex < 0) Then
            MostrarMensaje("Por Favor Seleccione un Concepto")
            Return False
        End If
        If (cbAlmacenOrigen.SelectedIndex < 0) Then
            MostrarMensaje("Por Favor Seleccione un Deposito")
            cbAlmacenOrigen.Focus()
            Return False
        End If
        If (cbConcepto.SelectedIndex >= 0) Then
            If (cbConcepto.Value = 6) Then
                If (cbDepositoDestino.SelectedIndex < 0) Then
                    MostrarMensaje("Por Favor Seleccione un Deposito Desitno")
                    cbDepositoDestino.Focus()
                    Return False
                End If
            End If
        End If
        If (grdetalle.RowCount <= 0) Then
            MostrarMensaje("Por Favor Inserte un Detalle")
            grdetalle.Focus()
            Return False
        End If
        If (grdetalle.RowCount = 1) Then
            If (CType(grdetalle.DataSource, DataTable).Rows(0).Item("ProductoId") = 0) Then
                MostrarMensaje("Por Favor Inserte un Detalle")
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
    Public Sub InsertarProductosSinLote(dt As DataTable, fila As Integer)
        grdetalle.Row = grdetalle.RowCount - 1
        If (grdetalle.GetValue("ProductoId") <> 0) Then
            _prAddDetalleVenta()
            grdetalle.Row = grdetalle.RowCount - 1
        End If
        Dim posicionFila As Integer = ObtenerPosicionFila(grdetalle, "Id")
        Dim existe As Boolean = _fnExisteProducto(dt.Rows(fila).Item("Item"))
        If ((posicionFila >= 0) And (Not existe)) Then
            SetCelValor(grdetalle, posicionFila, "ProductoId", dt.Rows(fila).Item("Item"))
            SetCelValor(grdetalle, posicionFila, "CodigoFabrica", dt.Rows(fila).Item("CodigoFabrica"))
            SetCelValor(grdetalle, posicionFila, "CodigoMarca", dt.Rows(fila).Item("Marca"))
            SetCelValor(grdetalle, posicionFila, "Medida", dt.Rows(fila).Item("Medida"))
            SetCelValor(grdetalle, posicionFila, "CategoriaProducto", dt.Rows(fila).Item("Categoria"))
            SetCelValor(grdetalle, posicionFila, "producto", dt.Rows(fila).Item("yfcdprod1"))
            SetCelValor(grdetalle, posicionFila, "Cantidad", dt.Rows(fila).Item("Cantidad"))
            SetCelValor(grdetalle, posicionFila, "stock", dt.Rows(fila).Item("stock"))
        Else
            If (existe) Then
                MostrarMensaje("El producto ya existe en el detalle")
            End If
        End If
    End Sub
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
    Private Sub grdetalle_CellValueChanged(sender As Object, e As ColumnActionEventArgs) Handles grdetalle.CellValueChanged
        Try
            'Celda Cantidad
            If (e.Column.Index = CelIndex(grdetalle, "Cantidad")) Then
                Dim cantidad As Integer = grdetalle.GetValue("Cantidad")
                Dim stock As Integer = grdetalle.GetValue("Stock")

                Dim posicion = ObtenerPosicionFila(grdetalle, "Id")

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

                Dim posicion = ObtenerPosicionFila(grdetalle, "Id")
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
    Private Sub grdetalle_CellEdited(sender As Object, e As ColumnActionEventArgs) Handles grdetalle.CellEdited
        If (e.Column.Index = CelIndex(grdetalle, "Cantidad")) Then
            If (Not IsNumeric(grdetalle.GetValue("Cantidad")) Or grdetalle.GetValue("Cantidad").ToString = String.Empty) Then
                grdetalle.SetValue("Cantidad", 1)
            Else
                If (grdetalle.GetValue("Cantidad") > 0) Then
                    Dim stock As Double = grdetalle.GetValue("stock")
                    If (grdetalle.GetValue("Cantidad") > stock And cbConcepto.Value <> 1) Then
                        Dim pos As Integer = ObtenerPosicionFila(grdetalle, "Id")
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
        Try
            If (Not _fnAccesible()) Then
                Return
            End If

            If (grdetalle.RowCount >= 2) Then
                If (grdetalle.CurrentColumn.Index = grdetalle.RootTable.Columns("img").Index) Then

                    EliminarFilaDetalle()
                End If
            End If

            If (grdetalle.CurrentColumn.Index = grdetalle.RootTable.Columns("imgAdd").Index) Then
                _HabilitarProductos()
            End If
        Catch ex As Exception
            MostrarMensajeError(ex.Message)
        End Try
    End Sub
    Private Sub EliminarFilaDetalle()
        Dim colProducto = 7
        EliminarFIla(grdetalle, "Id", "estado")
        grdetalle.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grdetalle.RootTable.Columns("estado"), janus.Windows.GridEX.ConditionOperator.GreaterThanOrEqualTo, 0))
        grdetalle.Select()
        grdetalle.Col = 7
        grdetalle.Row = grdetalle.RowCount - 1
    End Sub
    Private Sub btnGrabar_Click(sender As Object, e As EventArgs) Handles btnGrabar.Click
        If _ValidarCampos() = False Then
            Exit Sub
        End If
        If (tbCodigo.Text = String.Empty) Then
            _GuardarNuevo()
        Else
            _prGuardarModificado()
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
    Private Sub btnPrimero_Click(sender As Object, e As EventArgs) Handles btnPrimero.Click
        navegarPrimerRegistro(grmovimiento)
    End Sub
    Private Sub btnSiguiente_Click(sender As Object, e As EventArgs) Handles btnSiguiente.Click
        navegarSiguienteRegistro(grmovimiento)
    End Sub
    Private Sub btnUltimo_Click(sender As Object, e As EventArgs) Handles btnUltimo.Click
        navegarUltimoRegistro(grmovimiento)
    End Sub

    Private Sub btnAnterior_Click(sender As Object, e As EventArgs) Handles btnAnterior.Click
        navegarAnteriorRegistro(grmovimiento)
    End Sub

    Private Sub grdetalle_Enter(sender As Object, e As EventArgs) Handles grdetalle.Enter
        If (grdetalle.RowCount > 0) Then
            grdetalle.Select()
            SeleccionarColumnaFila(grdetalle, 3, 0)
        End If
    End Sub

    Private Sub cbAlmacen_KeyDown(sender As Object, e As KeyEventArgs) Handles cbAlmacenOrigen.KeyDown, grdetalle.KeyDown
        If (_fnAccesible()) Then
            If e.KeyData = Keys.Enter Then
                grdetalle.Focus()
                SeleccionarColumnaFila(grdetalle, 2, 0)
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

    Private Sub grmovimiento_SelectionChanged(sender As Object, e As EventArgs) Handles grmovimiento.SelectionChanged
        If (grmovimiento.RowCount >= 0 And grmovimiento.Row >= 0) Then
            _prMostrarRegistro(grmovimiento.Row)
        End If
    End Sub
#End Region
End Class