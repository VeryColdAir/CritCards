Public Class About
    Private Sub About_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lblVersion.Text = Form1.getVersion()
    End Sub
End Class