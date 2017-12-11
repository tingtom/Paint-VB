Public Class PaintForm
    Dim g As Graphics

    'saved point values
    Dim startLocation As Point
    Dim endLocation As Point
    Dim TempLocation As New Point(-1, -1)
    Dim TempLocation2 As New Point(-1, -1)

    'pens, color ect.
    Dim CurrentColor As Color = Color.Blue
    Dim CurrentColor2 As Color = Color.LightSkyBlue
    Dim PenWidth As Single = 1.0F
    Dim PenPoint As Pen
    Dim CurrentFont As Font

    'canvas
    Dim LastImage As New Bitmap(880, 665)
    Dim M1 As New Bitmap(880, 665)

    'saving stuff
    Dim SavedFileAddress As String = ""
    Dim image As System.Drawing.Image

    'saved values
    Dim edit As Boolean
    Dim paste As Boolean
    Dim open As Boolean
    Dim grid As Boolean
    Dim drawing As Boolean = False
    Dim NumberOfAngle As Integer = 0

    Function FillRegion(ByVal X As Integer, ByVal Y As Integer, ByVal FillCol As Color) As Boolean
        If X < 0 Or X > LastImage.Width Or Y < 0 Or Y > LastImage.Height Then
            Return False

        End If

        Application.DoEvents()

        Dim points As Stack = New Stack
        points.Push(New Point(X, Y))

        Dim Pointcolor As Color = LastImage.GetPixel(X, Y)


        Do
            Dim p As Point = CType(points.Pop(), Point)

            LastImage.SetPixel(p.X, p.Y, FillCol)

            If UpPixelHaseSameColor(p.X, p.Y, Pointcolor) Then
                points.Push(New Point(p.X, p.Y - 1))
            End If

            If DownPixelHaseSameColor(p.X, p.Y, Pointcolor) Then
                points.Push(New Point(p.X, p.Y + 1))
            End If


            If RightPixelHaseSameColor(p.X, p.Y, Pointcolor) Then
                points.Push(New Point(p.X + 1, p.Y))
            End If

            If LeftPixelHaseSameColor(p.X, p.Y, Pointcolor) Then
                points.Push(New Point(p.X - 1, p.Y))
            End If

        Loop While points.Count > 0

        Return True
    End Function
    Function UpPixelHaseSameColor(ByVal X As Integer, ByVal Y As Integer, ByVal Col As Color) As Boolean
        Dim result As Boolean = False
        If (Y > 0) Then
            If (LastImage.GetPixel(X, Y - 1) = Col) Then
                result = True
            End If

        End If
        Return result

    End Function
    Function DownPixelHaseSameColor(ByVal X As Integer, ByVal Y As Integer, ByVal Col As Color) As Boolean
        Dim result As Boolean = False
        If (Y < LastImage.Height - 1) Then
            If (LastImage.GetPixel(X, Y + 1) = Col) Then
                result = True
            End If

        End If
        Return result

    End Function
    Function RightPixelHaseSameColor(ByVal X As Integer, ByVal Y As Integer, ByVal Col As Color) As Boolean
        Dim result As Boolean = False
        If (X < LastImage.Width - 1) Then
            If (LastImage.GetPixel(X + 1, Y) = Col) Then
                result = True
            End If

        End If

        Return result

    End Function
    Function LeftPixelHaseSameColor(ByVal X As Integer, ByVal Y As Integer, ByVal Col As Color) As Boolean

        Dim result As Boolean = False
        If (X > 0) Then
            If (LastImage.GetPixel(X - 1, Y) = Col) Then
                result = True
            End If
        End If


        Return result

    End Function

    Sub UpdateImage()
        g = Graphics.FromImage(LastImage)
        Me.PictureBox1.Image = LastImage
    End Sub

    Sub ReloadPen(ByVal PenWd As Single, ByVal CurColor As Color)
        PenPoint = New Pen(CurColor, PenWd)
    End Sub

    Private Sub PictureBox1_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseDoubleClick
        Dim ss As ListViewItem
        For Each ss In ListView1.SelectedItems
            If ss.Text = "MultiLine" Then
                If TempLocation.X <> -1 Then
                    endLocation = e.Location
                    g.DrawLine(PenPoint, TempLocation, TempLocation2)
                    TempLocation = New Point(-1, -1)
                End If
            End If
        Next
    End Sub

    Private Sub PictureBox1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseDown
        Dim ss As ListViewItem
        For Each ss In ListView1.SelectedItems
            If e.Button = Windows.Forms.MouseButtons.Left Then
                If drawing = False Then

                    startLocation = e.Location
                    drawing = True
                    edit = True

                    If ss.Text = "MultiLine" Then

                        If TempLocation.X = -1 Then
                            TempLocation = startLocation
                            TempLocation2 = startLocation
                        End If

                        endLocation = e.Location
                        g.DrawLine(PenPoint, TempLocation, endLocation)
                        TempLocation = endLocation

                    ElseIf ss.Text = "Triangle" Then
                        If TempLocation.X = -1 Then
                            TempLocation = startLocation
                            TempLocation2 = startLocation
                        End If

                        If NumberOfAngle <= 2 Then

                            endLocation = e.Location
                            g.DrawLine(PenPoint, TempLocation, endLocation)
                            TempLocation = endLocation

                            If NumberOfAngle = 2 Then
                                g.DrawLine(PenPoint, TempLocation, TempLocation2)
                                TempLocation = New Point(-1, -1)
                                NumberOfAngle = 0
                            Else
                                NumberOfAngle += 1
                            End If


                        End If

                    ElseIf ss.Text = "Line" Then

                        If TempLocation.X = -1 Then
                            TempLocation = startLocation
                        End If

                        If NumberOfAngle < 2 Then
                            endLocation = e.Location
                            g.DrawLine(PenPoint, TempLocation, endLocation)
                            If NumberOfAngle = 1 Then
                                TempLocation = New Point(-1, -1)
                                NumberOfAngle = 0
                            Else
                                NumberOfAngle += 1
                            End If


                        End If

                    End If

                End If

            End If
        Next
    End Sub

    Private Sub PictureBox1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseMove
        If Not grid Then
            M1 = LastImage.Clone()
        End If
        If drawing = True Then
            Dim ss As ListViewItem
            For Each ss In ListView1.SelectedItems
                If ss.Text = "Pen" Then
                    g.DrawLine(PenPoint, startLocation.X, startLocation.Y, e.X, e.Y)
                    startLocation = e.Location
                    UpdateImage()

                ElseIf ss.Text = "Eraser" Then

                    Dim p As New Pen(Color.White, PenWidth)

                    g.DrawLine(p, startLocation.X, startLocation.Y, e.X, e.Y)
                    startLocation = e.Location
                    UpdateImage()
                End If
            Next
        End If

        ToolStripStatusLabel1.Text = "X: " & e.X
        ToolStripStatusLabel2.Text = "Y: " & e.Y
    End Sub

    Private Sub PictureBox1_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseUp
        If Not grid Then
            M1 = LastImage.Clone()
        Else
            GridToolStripMenuItem.Checked = Not GridToolStripMenuItem.Checked
            Dim i As Integer = 0
            While i < 880
                i = i + 20
                g.DrawLine(Pens.White, i, 0, i, 665)
            End While

            Dim i1 As Integer = 0
            While i1 < 665
                i1 = i1 + 20
                g.DrawLine(Pens.White, 0, i1, 880, i1)
            End While
            LastImage = M1.Clone
            grid = False
            UpdateImage()
        End If
        If drawing Then
            Dim ss As ListViewItem
            For Each ss In ListView1.SelectedItems
                If ss.Text = "Rectangle" Then

                    endLocation = e.Location

                    Dim s As Point

                    s.X = endLocation.X - startLocation.X
                    If s.X < 0 Then
                        startLocation.X = endLocation.X

                    End If

                    s.Y = endLocation.Y - startLocation.Y
                    If s.Y < 0 Then
                        startLocation.Y = endLocation.Y

                    End If

                    s.X = Math.Abs(s.X)
                    s.Y = Math.Abs(s.Y)
                    g.DrawRectangle(PenPoint, New Rectangle(startLocation, s))

                ElseIf ss.Text = "Rectangle(G)" Then

                    endLocation = e.Location

                    Dim s As Point

                    s.X = endLocation.X - startLocation.X
                    If s.X < 0 Then
                        startLocation.X = endLocation.X

                    End If

                    s.Y = endLocation.Y - startLocation.Y
                    If s.Y < 0 Then
                        startLocation.Y = endLocation.Y

                    End If

                    s.X = Math.Abs(s.X)
                    s.Y = Math.Abs(s.Y)
                    If s.Y = 0 Then
                        Return
                    End If
                    Dim b As Brush
                    b = New Drawing2D.LinearGradientBrush(New Rectangle(startLocation, s), CurrentColor, CurrentColor2, Drawing2D.LinearGradientMode.BackwardDiagonal)
                    g.FillRectangle(b, New Rectangle(startLocation, s))

                ElseIf ss.Text = "Circle" Then

                    endLocation = e.Location
                    Dim s As Point

                    s.X = endLocation.X - startLocation.X

                    If s.X < 0 Then
                        startLocation.X = endLocation.X

                    End If

                    s.Y = endLocation.Y - startLocation.Y

                    If s.Y < 0 Then
                        startLocation.Y = endLocation.Y

                    End If

                    s.X = Math.Abs(s.X)
                    s.Y = Math.Abs(s.Y)

                    If s.X > s.Y Then
                        s.Y = s.X
                    Else
                        s.X = s.Y
                    End If

                    g.DrawEllipse(PenPoint, New Rectangle(startLocation, s))

                ElseIf ss.Text = "Parallelepiped" Then

                    endLocation = e.Location

                    Dim s As Point

                    s.X = endLocation.X - startLocation.X

                    If s.X < 0 Then
                        Dim tmp As Integer = startLocation.X
                        startLocation.X = endLocation.X
                        endLocation.X = tmp
                    End If

                    s.Y = endLocation.Y - startLocation.Y

                    If s.Y < 0 Then
                        Dim tmp As Integer = startLocation.Y
                        startLocation.Y = endLocation.Y
                        endLocation.Y = tmp
                    End If

                    s.X = Math.Abs(s.X)
                    s.Y = Math.Abs(s.Y)



                    Dim p(3) As Point

                    p(0) = New Point(startLocation.X + s.X / 5, startLocation.Y)
                    p(1) = New Point(startLocation.X + s.X, startLocation.Y)

                    p(2) = New Point(endLocation.X - s.X / 5, endLocation.Y)
                    p(3) = New Point(endLocation.X - s.X, endLocation.Y)

                    g.DrawPolygon(PenPoint, p)

                ElseIf ss.Text = "Fill" Then
                    FillRegion(e.X, e.Y, CurrentColor)

                ElseIf ss.Text = "Text" Then
                    Dim txt As String = ToolStripTextBox1.Text

                    Dim b As Brush
                    b = New Drawing2D.LinearGradientBrush(New Rectangle(1, 1, 1, 1), CurrentColor, CurrentColor, Drawing2D.LinearGradientMode.Horizontal)
                    g.DrawString(txt, CurrentFont, b, e.X, e.Y)

                ElseIf ss.Text = "Circle(G)" Then

                    endLocation = e.Location
                    Dim s As Point

                    s.X = endLocation.X - startLocation.X

                    If s.X < 0 Then
                        startLocation.X = endLocation.X

                    End If

                    s.Y = endLocation.Y - startLocation.Y

                    If s.Y < 0 Then
                        startLocation.Y = endLocation.Y

                    End If

                    s.X = Math.Abs(s.X)
                    s.Y = Math.Abs(s.Y)

                    If s.X > s.Y Then
                        s.Y = s.X
                    Else
                        s.X = s.Y
                    End If
                    If s.Y = 0 Then
                        Return
                    End If

                    Dim b As Brush
                    b = New Drawing2D.LinearGradientBrush(New Rectangle(startLocation, s), CurrentColor, CurrentColor2, Drawing2D.LinearGradientMode.BackwardDiagonal)
                    g.FillEllipse(b, New Rectangle(startLocation, s))

                ElseIf ss.Text = "Color Picker" Then
                    Dim Pointcolor As Color = LastImage.GetPixel(e.X, e.Y)

                    CurrentColor = Pointcolor
                    ToolStripButton1.BackColor = Pointcolor

                    ReloadPen(PenWidth, CurrentColor)

                    If CurrentColor = Color.Black Then
                        ToolStripButton1.ForeColor = Color.White
                    ElseIf CurrentColor = Color.White Then
                        ToolStripButton1.ForeColor = Color.Black
                    End If

                ElseIf paste = True Then
                    g.DrawImage(image, e.Location)
                    paste = False
                    UpdateImage()

                ElseIf ss.Text = "Parallelepiped(G)" Then

                    endLocation = e.Location

                    Dim s As Point

                    s.X = endLocation.X - startLocation.X

                    If s.X < 0 Then
                        Dim tmp As Integer = startLocation.X
                        startLocation.X = endLocation.X
                        endLocation.X = tmp
                    End If

                    s.Y = endLocation.Y - startLocation.Y

                    If s.Y < 0 Then
                        Dim tmp As Integer = startLocation.Y
                        startLocation.Y = endLocation.Y
                        endLocation.Y = tmp
                    End If

                    s.X = Math.Abs(s.X)
                    s.Y = Math.Abs(s.Y)



                    Dim p(3) As Point

                    p(0) = New Point(startLocation.X + s.X / 5, startLocation.Y)
                    p(1) = New Point(startLocation.X + s.X, startLocation.Y)

                    p(2) = New Point(endLocation.X - s.X / 5, endLocation.Y)
                    p(3) = New Point(endLocation.X - s.X, endLocation.Y)

                    Dim b As Brush
                    b = New Drawing2D.LinearGradientBrush(New Rectangle(startLocation, s), CurrentColor, CurrentColor2, Drawing2D.LinearGradientMode.BackwardDiagonal)
                    g.FillPolygon(b, p)

                End If
            Next
        End If

        drawing = False

        UpdateImage()
       
    End Sub

    Private Sub PaintForm_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        If edit = True Then
            Dim result As Integer = MsgBox("Save the image before exiting?", MsgBoxStyle.YesNo, "Information")
            If result = MsgBoxResult.Yes Then
                SaveFileDialog1.Filter = "Bitmap File (*.bmp)|*.bmp"
                If SaveFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
                    SavedFileAddress = SaveFileDialog1.FileName
                    LastImage.Save(SavedFileAddress)
                End If

            ElseIf result = MsgBoxResult.No Then
                End
            End If
        End If

    End Sub
    Private Sub PaintForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        CurrentFont = Me.Font
        ToolStripButton3.Text = "Font - " & CurrentFont.Name
        ToolStripButton1.BackColor = CurrentColor
        ToolStripButton2.BackColor = CurrentColor2
        penlabel1.Text = "Pen Size: 1"
        ToolStripStatusLabel4.Text = "Current Tool: Pen"
        ToolStripButton4.Image = My.Resources.Brush_icon
        g = Graphics.FromImage(LastImage)
        ReloadPen(PenWidth, CurrentColor)

        Text = "Paint Pro - Untitled Image"
        UpdateImage()
        g.Clear(Color.White)

    End Sub

    Private Sub ClearToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ClearToolStripMenuItem.Click
        ClearScreen()
    End Sub

    Sub ClearScreen()
        g = Graphics.FromImage(LastImage)
        g.Clear(Color.White)
        UpdateImage()
        edit = False
        Text = "Paint Pro - Untitled Image"
    End Sub

    Private Sub SaveToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveToolStripMenuItem.Click
        If open = False Then
            If edit = True Then
                SaveFileDialog1.Filter = "Bitmap File (*.bmp)|*.bmp"
                If SaveFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
                    SavedFileAddress = SaveFileDialog1.FileName
                    LastImage.Save(SavedFileAddress)
                    Me.Text = "Paint Pro - " & SaveFileDialog1.FileName
                End If
            End If
        ElseIf edit = False Then
            MsgBox("Blank page, No need to save", MsgBoxStyle.Critical, "Error")
            Return
        Else
            LastImage.Save(SavedFileAddress)
        End If
    End Sub

    Private Sub SaveAsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveAsToolStripMenuItem.Click
        If edit = True Then
            SaveFileDialog1.Filter = "Bitmap File (*.bmp)|*.bmp"
            If SaveFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
                SavedFileAddress = SaveFileDialog1.FileName
                LastImage.Save(SavedFileAddress)
                Me.Text = "Paint Pro - " & SaveFileDialog1.FileName
            End If
        ElseIf edit = False Then
            MsgBox("Blank page, No need to save", MsgBoxStyle.Critical, "Error")
            Return
        ElseIf open = True Then
            LastImage.Save(SavedFileAddress)
        End If
        
    End Sub

    Private Sub P1RadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles P1RadioButton.CheckedChanged
        PenWidth = 1.0F
        ReloadPen(PenWidth, CurrentColor)
        penlabel1.Text = "Pen Size: 1"
    End Sub

    Private Sub P2RadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles P2RadioButton.CheckedChanged
        PenWidth = 2.0F
        ReloadPen(PenWidth, CurrentColor)
        penlabel1.Text = "Pen Size: 2"
    End Sub

    Private Sub P3RadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles P3RadioButton.CheckedChanged
        PenWidth = 3.0F
        ReloadPen(PenWidth, CurrentColor)
        penlabel1.Text = "Pen Size: 3"
    End Sub

    Private Sub RadioButton1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton1.CheckedChanged
        PenWidth = 4.0F
        ReloadPen(PenWidth, CurrentColor)
        penlabel1.Text = "Pen Size: 4"
    End Sub

    Private Sub RadioButton2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton2.CheckedChanged
        PenWidth = 5.0F
        ReloadPen(PenWidth, CurrentColor)
        penlabel1.Text = "Pen Size: 5"
    End Sub

    Private Sub M1ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles M1ToolStripMenuItem.Click
        M1 = LastImage.Clone()
    End Sub

    Private Sub S1ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles S1ToolStripMenuItem.Click
        LastImage = M1.Clone
        UpdateImage()
    End Sub

    Private Sub NewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewToolStripMenuItem.Click
        If edit = True Then
            Dim result As Integer = MsgBox("Save the image before clearing?", MsgBoxStyle.YesNo, "Information")
            If result = MsgBoxResult.Yes Then
                SaveFileDialog1.Filter = "Bitmap File (*.bmp)|*.bmp"
                If SaveFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
                    SavedFileAddress = SaveFileDialog1.FileName
                    LastImage.Save(SavedFileAddress)
                    Me.Text = "Paint Pro - " & SaveFileDialog1.FileName
                    edit = False
                End If
            ElseIf result = MsgBoxResult.No Then
                g.Clear(Color.White)
                UpdateImage()
                SavedFileAddress = ""
                Text = "Paint Pro - Untitled Image"
                edit = False
            End If
        Else
            g.Clear(Color.White)
            UpdateImage()
            SavedFileAddress = ""
            Text = "Paint Pro - Untitled Image"
            edit = False
        End If
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        If edit = True Then
            Dim result As Integer = MsgBox("Save the image before exiting?", MsgBoxStyle.YesNo, "Information")
            If result = MsgBoxResult.Yes Then
                SaveFileDialog1.Filter = "Bitmap File (*.bmp)|*.bmp"
                If SaveFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
                    SavedFileAddress = SaveFileDialog1.FileName
                    LastImage.Save(SavedFileAddress)
                    ' Me.Text = "Paint Pro - " & SaveFileDialog1.FileName
                    End
                End If

            ElseIf result = MsgBoxResult.No Then
                End
            End If
        End If
    End Sub

    Private Sub ToolStripButton3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton3.Click
        If FontDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            CurrentFont = FontDialog1.Font
            ToolStripButton3.Text = "Font - " & CurrentFont.Name
        End If
    End Sub

    Private Sub ToolStripButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton1.Click
        If ColorDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            CurrentColor = ColorDialog1.Color
            ToolStripButton1.BackColor = CurrentColor
            ReloadPen(PenWidth, CurrentColor)
            If CurrentColor = Color.Black Then
                ToolStripButton1.ForeColor = Color.White

            ElseIf CurrentColor = Color.White Then
                ToolStripButton1.ForeColor = Color.Black

            End If
        End If
    End Sub

    Private Sub ToolStripButton2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton2.Click
        If ColorDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            CurrentColor2 = ColorDialog1.Color
            ToolStripButton2.BackColor = CurrentColor2

            If CurrentColor2 = Color.Black Then
                ToolStripButton2.ForeColor = Color.White
            ElseIf CurrentColor2 = Color.White Then
                ToolStripButton2.ForeColor = Color.Black
            End If
        End If
    End Sub

    Private Sub OpenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenToolStripMenuItem.Click
        Dim i As Integer
        Dim image As System.Drawing.Image
        i = OpenFileDialog1.ShowDialog()
        If i = 1 Then
            image = image.FromFile(OpenFileDialog1.FileName)
            g.DrawImage(image, New Point(0, 0))
            UpdateImage()
            SavedFileAddress = OpenFileDialog1.FileName
            Me.Text = "Paint Pro - " & OpenFileDialog1.FileName
            open = True
        End If
    End Sub

    Private Sub ImageToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ImageToolStripMenuItem1.Click
        Dim i As Integer
        i = OpenFileDialog1.ShowDialog()
        If i = 1 Then
            image = image.FromFile(OpenFileDialog1.FileName)
            paste = True
        End If
    End Sub

    Private Sub ClipboardToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ClipboardToolStripMenuItem.Click
        If Clipboard.ContainsImage Then
            paste = True
            image = Clipboard.GetImage
        Else
            MsgBox("Clipboard Empty", MsgBoxStyle.Critical, "Error")
            paste = False

        End If
    End Sub

    Private Sub SelectAllToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SelectAllToolStripMenuItem.Click
        Clipboard.SetImage(PictureBox1.Image)
    End Sub

    Private Sub CutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CutToolStripMenuItem.Click
        Clipboard.SetImage(PictureBox1.Image)
        g.Clear(Color.White)
        UpdateImage()
    End Sub

    Private Sub PrintToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PrintToolStripMenuItem.Click
        If edit = False Then
            MsgBox("Nothing drawn", MsgBoxStyle.Information, "Error")
        Else
            PrintForm1.PrintAction = Printing.PrintAction.PrintToPrinter
            Dim f As New Form
            f.Visible = False
            f.WindowState = FormWindowState.Maximized
            f.BackgroundImage = PictureBox1.Image
            f.BackgroundImageLayout = ImageLayout.Stretch
            f.FormBorderStyle = Windows.Forms.FormBorderStyle.None
            PrintForm1.DocumentName = SavedFileAddress
            PrintForm1.Print(f, PowerPacks.Printing.PrintForm.PrintOption.FullWindow)
        End If
    End Sub

    Private Sub PrintPreviewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PrintPreviewToolStripMenuItem.Click
        If edit = False Then
            MsgBox("Nothing drawn", MsgBoxStyle.Information, "Error")
        Else
            PrintForm1.PrintAction = Printing.PrintAction.PrintToPreview
            Dim f As New Form
            f.Visible = False
            f.WindowState = FormWindowState.Maximized
            f.BackgroundImage = PictureBox1.Image
            f.BackgroundImageLayout = ImageLayout.Stretch
            f.FormBorderStyle = Windows.Forms.FormBorderStyle.None
            PrintForm1.DocumentName = SavedFileAddress
            PrintForm1.Print(f, PowerPacks.Printing.PrintForm.PrintOption.FullWindow)
        End If
    End Sub

    Private Sub ShowA4LineToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowA4LineToolStripMenuItem.Click
        ShowA4LineToolStripMenuItem.Checked = Not ShowA4LineToolStripMenuItem.Checked
        If ShowA4LineToolStripMenuItem.Checked = True Then
            g.DrawLine(Pens.Black, 537, 0, 537, 665)
            UpdateImage()
        Else
            g.DrawLine(Pens.White, 537, 0, 537, 665)
            UpdateImage()
        End If
    End Sub

    Private Sub ListView1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListView1.SelectedIndexChanged
        Dim ss As ListViewItem
        For Each ss In ListView1.SelectedItems

            'special fix::multi select
            If ListView1.SelectedItems.Count > 1 Then
                ss.Selected = False
            End If

            'text 
            If ss.Text = "Text" Then
                ToolStripButton4.Image = My.Resources.Text_icon
                ToolStripTextBox1.Visible = True
                ToolStripButton3.Visible = True
                ToolStripStatusLabel4.Text = "Current Tool: " & ss.Text
            Else
                ToolStripTextBox1.Visible = False
                ToolStripButton3.Visible = False
            End If

            'general
            If ss.Text = "Pen" Then
                Me.Cursor = Cursors.Cross
                ToolStripButton4.Image = My.Resources.Brush_icon
                ToolStripStatusLabel4.Text = "Current Tool: " & ss.Text

            ElseIf ss.Text = "Color Picker" Then
                Me.Cursor = Cursors.Cross
                ToolStripButton4.Image = My.Resources.Eyedropper_icon
                ToolStripStatusLabel4.Text = "Current Tool: " & ss.Text

            ElseIf ss.Text = "MultiLine" Then
                Me.Cursor = Cursors.Cross
                ToolStripStatusLabel4.Text = "Current Tool: " & ss.Text
                ToolStripButton4.Image = Nothing

            ElseIf ss.Text = "Fill" Then
                Me.Cursor = Cursors.Cross
                ToolStripStatusLabel4.Text = "Current Tool: " & ss.Text
                ToolStripButton4.Image = My.Resources.Paintbucket_icon

            ElseIf ss.Text = "Eraser" Then
                Me.Cursor = Cursors.Default
                ToolStripButton4.Image = My.Resources.Rubber_icon
                ToolStripStatusLabel4.Text = "Current Tool: " & ss.Text

            ElseIf ss.Text = "Text" Then
                Me.Cursor = Cursors.IBeam
            Else
                Me.Cursor = Cursors.Default
            End If

            'shapes
            If ss.Text = "Rectangle" Then
                ToolStripStatusLabel4.Text = "Current Tool: " & ss.Text
                Me.Cursor = Cursors.SizeAll
                ToolStripButton4.Image = Nothing

            ElseIf ss.Text = "Rectangle(G)" Then
                Me.Cursor = Cursors.SizeAll
                ToolStripStatusLabel4.Text = "Current Tool: Rectangle(Gradient)"
                ToolStripButton4.Image = Nothing

            ElseIf ss.Text = "Triangle" Then
                ToolStripStatusLabel4.Text = "Current Tool: " & ss.Text
                TempLocation = New Point(-1, -1)
                ToolStripButton4.Image = Nothing

            ElseIf ss.Text = "Circle" Then
                Me.Cursor = Cursors.SizeAll
                ToolStripStatusLabel4.Text = "Current Tool: " & ss.Text
                ToolStripButton4.Image = Nothing

            ElseIf ss.Text = "Circle(G)" Then
                Me.Cursor = Cursors.SizeAll
                ToolStripStatusLabel4.Text = "Current Tool: Circle(Gradient)"
                ToolStripButton4.Image = Nothing

            ElseIf ss.Text = "MultiLine" Then
                Me.Cursor = Cursors.Cross
                ToolStripStatusLabel4.Text = "Current Tool: " & ss.Text
                ToolStripButton4.Image = Nothing

            ElseIf ss.Text = "Parallelepiped" Then
                ToolStripStatusLabel4.Text = "Current Tool: " & ss.Text
                ToolStripButton4.Image = Nothing

            ElseIf ss.Text = "Line" Then
                Me.Cursor = Cursors.Cross
                ToolStripStatusLabel4.Text = "Current Tool: " & ss.Text
                ToolStripButton4.Image = Nothing

            ElseIf ss.Text = "Parallelepiped(G)" Then
                ToolStripStatusLabel4.Text = "Current Tool: Parallelepiped(Gradient)"
                ToolStripButton4.Image = Nothing

            End If
        Next

    End Sub

    Private Sub GridToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GridToolStripMenuItem.Click
        GridToolStripMenuItem.Checked = Not GridToolStripMenuItem.Checked

        If GridToolStripMenuItem.Checked = True Then
            Dim i As Integer = 0
            While i < 880
                i = i + 20
                g.DrawLine(Pens.Black, i, 0, i, 665)
            End While


            Dim i1 As Integer = 0
            While i1 < 665
                i1 = i1 + 20
                g.DrawLine(Pens.Black, 0, i1, 880, i1)
            End While
            grid = True
            UpdateImage()
        Else
            Dim i As Integer = 0
            While i < 880
                i = i + 20
                g.DrawLine(Pens.White, i, 0, i, 665)
            End While

            Dim i1 As Integer = 0
            While i1 < 665
                i1 = i1 + 20
                g.DrawLine(Pens.White, 0, i1, 880, i1)
            End While
            LastImage = M1.Clone
            grid = False
            UpdateImage()
        End If
    End Sub
End Class