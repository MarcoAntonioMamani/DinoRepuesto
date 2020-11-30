Imports DevComponents.DotNetBar.Controls
Imports DevComponents.DotNetBar
Public Class F1_Cantidad
    Public Stock As Decimal
    Public Cantidad As Decimal
    Public Producto As String
    Public bandera As Boolean


    Private Sub F_Cantidad_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        tbCantidad.Value = 0

        lbStock.Text = "Stock Disponible = " + Str(Stock)
        lbProducto.Text = Producto
        tbCantidad.Focus()
    End Sub

    Private Sub tbCantidad_Enter(sender As Object, e As EventArgs) Handles tbCantidad.Enter

    End Sub

    Private Sub tbCantidad_KeyDown(sender As Object, e As KeyEventArgs) Handles tbCantidad.KeyDown
        If (e.KeyData = Keys.Escape) Then
            bandera = False
            Me.Close()
        End If
        If (e.KeyData = Keys.Enter) Then

            If (tbCantidad.Value > Stock) Then
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "La cantidad Ingresada " + Str(tbCantidad.Value) + " Es superior a la cantidad Disponible del Producto : " + Str(Stock), img, 5000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                Return

            Else
                If (tbCantidad.Value <= 0) Then
                    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                    ToastNotification.Show(Me, "La cantidad debe ser mayor a 0", img, 5000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                    Return

                Else
                    Cantidad = tbCantidad.Value
                    bandera = True
                    Me.Close()
                End If


            End If
        End If

    End Sub
End Class