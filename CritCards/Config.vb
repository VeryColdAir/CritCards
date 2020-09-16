Public Class Config
    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        gridGames.Rows.Add()
        gridGames.Rows.Add()
        Dim gameNumber As Integer = gridGames.Rows.Count / 2
        gridGames.Rows.Item(gridGames.Rows.Count - 2).Cells(0).Value = gameNumber
        gridGames.Rows.Item(gridGames.Rows.Count - 1).Cells(0).Value = gameNumber
        gridGames.Rows.Item(gridGames.Rows.Count - 1).Cells(1).ReadOnly = True
        gridGames.Rows.Item(gridGames.Rows.Count - 2).Cells(2).Value = "Hit"
        gridGames.Rows.Item(gridGames.Rows.Count - 1).Cells(2).Value = "Fumble"
    End Sub

    Private Sub gridGames_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles gridGames.CellClick
        If e.ColumnIndex = 3 Then

            Dim dialog As New FolderBrowserDialog()
            dialog.RootFolder = Environment.SpecialFolder.Desktop
            dialog.SelectedPath = "C:\"
            dialog.Description = "Select a deck for the game."
            If dialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
                gridGames.Rows.Item(e.RowIndex).Cells(3).Value = dialog.SelectedPath
            End If
        End If
    End Sub

    Private Sub btnRemove_Click(sender As Object, e As EventArgs) Handles btnRemove.Click
        If gridGames.Rows.Count <= 0 Then
            MsgBox("There must be at least one game to remove!")
            Exit Sub
        ElseIf gridGames.SelectedRows.Count <= 0 Then
            MsgBox("There must be at least one *row* (not cell) selected.")
            Exit Sub
        End If

        Dim i As New List(Of Integer)
        Dim j As Integer
        For Each row As DataGridViewRow In gridGames.SelectedRows
            i.Add(gridGames.Rows(row.Index).Cells(0).Value)
        Next
        'Delete the game(s)
        For j = gridGames.Rows.Count - 1 To 0 Step -1
            If i.Contains(gridGames.Rows(j).Cells(0).Value) Then
                gridGames.Rows.Remove(gridGames.Rows(j))
            End If
        Next
        'Renumbers the game indexes
        For Each row As DataGridViewRow In gridGames.Rows
            row.Cells(0).Value = (row.Index + 1) \ 2 + (row.Index + 1) Mod 2
        Next
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If gridGames.Rows.Count <= 0 Then
            MsgBox("There must be at least one game to continue.")
            Exit Sub
        End If
        'Checks to see if game names were entered
        For Each row As DataGridViewRow In gridGames.Rows
            If row.Cells(1).Value = "" Then
                MsgBox("All games must have a name entered for them.")
                Exit Sub
            End If
        Next
        Me.Close()
    End Sub

    Private Sub gridGames_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles gridGames.CellEndEdit
        If e.ColumnIndex = 1 Then
            gridGames.Rows(e.RowIndex + 1).Cells(1).Value = gridGames.Rows(e.RowIndex).Cells(1).Value
        End If
    End Sub
End Class