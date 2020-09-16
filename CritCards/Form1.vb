Imports System.Environment
Imports System.Xml
Public Class Form1
    Private myPaths As String(,)
    Private xmlPath As String = GetFolderPath(SpecialFolder.ApplicationData) & "\CritCards"
    Private myConfig As New Config
    Private numHits As Integer
    Private numFumbles As Integer
    Private versionNumber As String = "0.1.1"
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Check to see if the CritCards folder already exists in the appData
        If (Not System.IO.Directory.Exists(xmlPath)) Then
            System.IO.Directory.CreateDirectory(xmlPath)
        End If

        'Display prompt to fill out games w/ filepaths.
        If My.Computer.FileSystem.FileExists(xmlPath & "\paths.xml") Then
            'Read the file
            Dim doc As New XmlDocument()
            doc.Load(xmlPath & "\paths.xml")
            Dim nodelist = doc.GetElementsByTagName("game")
            For Each node As XmlElement In nodelist
                myConfig.gridGames.Rows.Add()
                myConfig.gridGames.Rows.Add()
                myConfig.gridGames.Rows.Item(myConfig.gridGames.Rows.Count - 2).Cells(0).Value = node("index").InnerText
                myConfig.gridGames.Rows.Item(myConfig.gridGames.Rows.Count - 1).Cells(0).Value = node("index").InnerText
                myConfig.gridGames.Rows.Item(myConfig.gridGames.Rows.Count - 2).Cells(1).Value = node("name").InnerText
                myConfig.gridGames.Rows.Item(myConfig.gridGames.Rows.Count - 1).Cells(1).Value = node("name").InnerText
                myConfig.gridGames.Rows.Item(myConfig.gridGames.Rows.Count - 1).Cells(1).ReadOnly = True
                myConfig.gridGames.Rows.Item(myConfig.gridGames.Rows.Count - 2).Cells(2).Value = "Hit"
                myConfig.gridGames.Rows.Item(myConfig.gridGames.Rows.Count - 1).Cells(2).Value = "Fumble"
                myConfig.gridGames.Rows.Item(myConfig.gridGames.Rows.Count - 2).Cells(3).Value = node("hit_deck").InnerText
                myConfig.gridGames.Rows.Item(myConfig.gridGames.Rows.Count - 1).Cells(3).Value = node("fumble_deck").InnerText
            Next
        Else
            'Dim myConfig As New Config 'remove after testing that class propertiy works correctly
            myConfig.ShowDialog()
            'Create the file
            CreateXML(myConfig)
        End If
        ReDim myPaths(myConfig.gridGames.Rows.Count / 2 - 1, 1)
        For i As Integer = 0 To myConfig.gridGames.Rows.Count - 1 Step 2
            cboGames.Items.Add(myConfig.gridGames.Rows(i).Cells(1).Value)
            myPaths(myConfig.gridGames.Rows(i).Cells(0).Value - 1, 0) = myConfig.gridGames.Rows(i).Cells(3).Value
            myPaths(myConfig.gridGames.Rows(i).Cells(0).Value - 1, 1) = myConfig.gridGames.Rows(i + 1).Cells(3).Value
        Next
    End Sub
    Private Sub CreateXML(myConfig As Config)
        Dim dom As MSXML2.DOMDocument60
        Dim root As MSXML2.IXMLDOMNode
        Dim child As MSXML2.IXMLDOMNode
        Dim innerchild As MSXML2.IXMLDOMNode

        '//Create an XML Document
        dom = New MSXML2.DOMDocument60
        dom.setProperty("SelectionLanguage", "XPath")
        dom.setProperty("SelectionNamespaces", "xmlns:xsl='http://www.w3.org/1999/XSL/Transform'")

        '//Append an xml processing instruction
        dom.appendChild(dom.createProcessingInstruction("xml", "version='1.0' encoding='UTF-8'"))
        '//Create a root node
        root = dom.createNode(1, "games", "")

        'For each game in the datagridview, add a row here
        For i As Integer = 0 To myConfig.gridGames.Rows.Count - 1 Step 2
            ' Add parent
            child = dom.createNode(1, "game", "")
            root.appendChild(child)
            innerchild = dom.createNode(1, "index", "")
            innerchild.text = myConfig.gridGames.Rows(i).Cells(0).Value
            child.appendChild(innerchild)
            innerchild = dom.createNode(1, "name", "")
            innerchild.text = myConfig.gridGames.Rows(i).Cells(1).Value
            child.appendChild(innerchild)
            innerchild = dom.createNode(1, "hit_deck", "")
            innerchild.text = myConfig.gridGames.Rows(i).Cells(3).Value
            child.appendChild(innerchild)
            innerchild = dom.createNode(1, "fumble_deck", "")
            innerchild.text = myConfig.gridGames.Rows(i + 1).Cells(3).Value
            child.appendChild(innerchild)

            root.appendChild(child)
        Next

        '//Append root node to the document
        dom.appendChild(root)

        '//View the document
        '//Note: The UTF-8 encoding will not show here
        '//        The dom doc will use it's own encoding internally until the document is saved
        Debug.Print(dom.xml)
        '//Save the document
        dom.save(xmlPath & "\paths.xml")
    End Sub
    Private Sub btnHit_Click(sender As Object, e As EventArgs) Handles btnHit.Click
        If cboGames.SelectedIndex = -1 Then
            MsgBox("You must select a game!")
            Exit Sub
        End If
        PictureBox4.Image = PictureBox3.Image
        PictureBox3.Image = PictureBox2.Image
        PictureBox2.Image = PictureBox1.Image

        'TODO make this faster?
        Dim cardNumber As String = Format(CInt(Math.Ceiling(Rnd() * numHits - 1) + 1), "00")
        Dim card As String

        If My.Computer.FileSystem.FileExists(myPaths(cboGames.SelectedIndex, 0) & "\" & cardNumber & ".png") Then
            card = myPaths(cboGames.SelectedIndex, 0) & "\" & cardNumber & ".png"
        ElseIf My.Computer.FileSystem.FileExists(myPaths(cboGames.SelectedIndex, 0) & "\" & cardNumber & ".jpeg") Then
            card = myPaths(cboGames.SelectedIndex, 0) & "\" & cardNumber & ".jpeg"
        ElseIf My.Computer.FileSystem.FileExists(myPaths(cboGames.SelectedIndex, 0) & "\" & cardNumber & ".jpg") Then
            card = myPaths(cboGames.SelectedIndex, 0) & "\" & cardNumber & ".jpg"
        Else
            MsgBox("Failed to find card " & cardNumber & " as a '.png', '.jpeg', or '.jpg' image file in " & myPaths(cboGames.SelectedIndex, 0))
            Exit Sub
        End If

        PictureBox1.Image = Image.FromFile(card)
        If Not PictureBox1.Image Is Nothing Then
            PictureBox1.Visible = True
        End If
        If Not PictureBox2.Image Is Nothing Then
            PictureBox2.Visible = True
        End If
        If Not PictureBox3.Image Is Nothing Then
            PictureBox3.Visible = True
        End If
        If Not PictureBox4.Image Is Nothing Then
            PictureBox4.Visible = True
        End If
    End Sub

    Private Sub btnFumble_Click(sender As Object, e As EventArgs) Handles btnFumble.Click
        If cboGames.SelectedIndex = -1 Then
            MsgBox("You must select a game!")
            Exit Sub
        End If
        PictureBox4.Image = PictureBox3.Image
        PictureBox3.Image = PictureBox2.Image
        PictureBox2.Image = PictureBox1.Image

        Dim cardNumber As String = Format(CInt(Math.Ceiling(Rnd() * numFumbles - 1) + 1), "00")
        Dim card As String

        If My.Computer.FileSystem.FileExists(myPaths(cboGames.SelectedIndex, 1) & "\" & cardNumber & ".png") Then
            card = myPaths(cboGames.SelectedIndex, 1) & "\" & cardNumber & ".png"
        ElseIf My.Computer.FileSystem.FileExists(myPaths(cboGames.SelectedIndex, 1) & "\" & cardNumber & ".jpeg") Then
            card = myPaths(cboGames.SelectedIndex, 1) & "\" & cardNumber & ".jpeg"
        ElseIf My.Computer.FileSystem.FileExists(myPaths(cboGames.SelectedIndex, 1) & "\" & cardNumber & ".jpg") Then
            card = myPaths(cboGames.SelectedIndex, 1) & "\" & cardNumber & ".jpg"
        Else
            MsgBox("Failed to find card " & cardNumber & " as a '.png', '.jpeg', or '.jpg' image file in " & myPaths(cboGames.SelectedIndex, 1))
            Exit Sub
        End If

        PictureBox1.Image = Image.FromFile(card)

        If Not PictureBox1.Image Is Nothing Then
            PictureBox1.Visible = True
        End If
        If Not PictureBox2.Image Is Nothing Then
            PictureBox2.Visible = True
        End If
        If Not PictureBox3.Image Is Nothing Then
            PictureBox3.Visible = True
        End If
        If Not PictureBox4.Image Is Nothing Then
            PictureBox4.Visible = True
        End If
    End Sub
    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        If PictureBox1.Image Is Nothing Then
            Exit Sub
        End If
        Dim myRelay As Replay = New Replay
        myRelay.picReplay.Image = PictureBox1.Image
        myRelay.Show()
    End Sub
    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        If PictureBox2.Image Is Nothing Then
            Exit Sub
        End If
        Dim myRelay As Replay = New Replay
        myRelay.picReplay.Image = PictureBox2.Image
        myRelay.Show()
    End Sub
    Private Sub PictureBox3_Click(sender As Object, e As EventArgs) Handles PictureBox3.Click
        If PictureBox3.Image Is Nothing Then
            Exit Sub
        End If
        Dim myRelay As Replay = New Replay
        myRelay.picReplay.Image = PictureBox3.Image
        myRelay.Show()
    End Sub
    Private Sub PictureBox4_Click(sender As Object, e As EventArgs) Handles PictureBox4.Click
        If PictureBox4.Image Is Nothing Then
            Exit Sub
        End If
        Dim myRelay As Replay = New Replay
        myRelay.picReplay.Image = PictureBox4.Image
        myRelay.Show()
    End Sub

    Private Sub btnConfig_Click(sender As Object, e As EventArgs) Handles btnConfig.Click
        myConfig.ShowDialog()
        'Rereate the file
        CreateXML(myConfig)
        ReDim myPaths(myConfig.gridGames.Rows.Count / 2 - 1, 1)
        cboGames.Items.Clear()
        For i As Integer = 0 To myConfig.gridGames.Rows.Count - 1 Step 2
            cboGames.Items.Add(myConfig.gridGames.Rows(i).Cells(1).Value)
            myPaths(myConfig.gridGames.Rows(i).Cells(0).Value - 1, 0) = myConfig.gridGames.Rows(i).Cells(3).Value
            myPaths(myConfig.gridGames.Rows(i).Cells(0).Value - 1, 1) = myConfig.gridGames.Rows(i + 1).Cells(3).Value
        Next
        cboGames.SelectedIndex = -1
        cboGames.ResetText()
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        If MessageBox.Show("Are you sure you want to exit?", "Confirm Exit", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Me.Close()
        Else
            Exit Sub
        End If
    End Sub

    Private Sub cboGames_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboGames.SelectedIndexChanged
        'These take a hot second to calculate, so instead of doing it every time a card is drawn, do it when the game is selected.
        numHits = (IO.Directory.GetFiles(myPaths(cboGames.SelectedIndex, 0), "*.png").Count) + IO.Directory.GetFiles(myPaths(cboGames.SelectedIndex, 0), "*.jpg").Count + IO.Directory.GetFiles(myPaths(cboGames.SelectedIndex, 0), "*.jpeg").Count
        numFumbles = (IO.Directory.GetFiles(myPaths(cboGames.SelectedIndex, 1), "*.png").Count) + IO.Directory.GetFiles(myPaths(cboGames.SelectedIndex, 1), "*.jpg").Count + IO.Directory.GetFiles(myPaths(cboGames.SelectedIndex, 1), "*.jpeg").Count
    End Sub

    Private Sub btnAbout_Click(sender As Object, e As EventArgs) Handles btnAbout.Click
        Dim myAbout As New About
        About.ShowDialog()
    End Sub
    Friend Function getVersion() As String
        Return versionNumber
    End Function
End Class
