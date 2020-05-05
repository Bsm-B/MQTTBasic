Imports uPLibrary.Networking.M2Mqtt
Imports uPLibrary.Networking.M2Mqtt.Messages
Imports System.Text
Imports System.Threading



Public Class Form1

    Dim client As MqttClient
    Dim Msg As StringBuilder = New StringBuilder(4096)

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If (TextBox1.Text.Length <> 0) Then


            Try
                client = New MqttClient(TextBox1.Text)

                Dim clientId As String = Guid.NewGuid().ToString()

                AddHandler client.MqttMsgPublishReceived, AddressOf Client_MqttMsgPublishReceived
                AddHandler client.ConnectionClosed, AddressOf Client_Disconnect

                client.Connect(clientId)

                If client.IsConnected Then
                    ComboBox1.SelectedIndex = 0
                    ComboBox2.SelectedIndex = 0

                    ToolStripStatusLabel1.Text = "Connected to " + "'" + TextBox1.Text + "'"
                Else
                    ToolStripStatusLabel1.Text = "Disconnected"
                End If

            Catch ex As Exception

                ToolStripStatusLabel1.Text = "Error"
                MsgBox(ex.Message(), MsgBoxStyle.Critical)
            End Try
        Else
            ToolStripStatusLabel1.Text = "Please enter a valid Broker address "

        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If (client IsNot Nothing AndAlso client.IsConnected()) Then
            client.Disconnect()
        Else
            ToolStripStatusLabel1.Text = "Error"
        End If
    End Sub
    Private Sub Client_Disconnect(sender As Object, e As EventArgs)
        ToolStripStatusLabel1.Text = "Connection Lost"
    End Sub


    Private Sub Client_MqttMsgPublishReceived(ByVal sender As Object, ByVal e As MqttMsgPublishEventArgs)
        Msg.AppendLine()
        Msg.Append("[" + TimeOfDay.ToString("h:mm:ss tt") + "] Topic: " + e.Topic.ToString() + ", Len: " + e.Message.Length.ToString() + ", Qos: " + e.QosLevel.ToString())
        Msg.AppendLine()
        Msg.Append("Msg : " + Encoding.Default.GetString(e.Message))
        Msg.AppendLine()

        SetText(Msg.ToString)

    End Sub

    Delegate Sub SetTextCallback(newString As String)
    Private Sub SetText(ByVal newString As String)
        ' Calling from another thread? -> Use delegate
        If Me.RichTextBox2.InvokeRequired Then
            Dim d As New SetTextCallback(AddressOf SetText)
            ' Execute delegate in the UI thread, pass args as an array
            Me.Invoke(d, New Object() {newString})
        Else ' Same thread, assign string to the textbox
            Me.RichTextBox2.Text = newString
        End If
    End Sub


    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If (client IsNot Nothing AndAlso client.IsConnected()) Then
            If (TextBox3.Text.Length <> 0) Then

                Try
                    Dim Topic() As String = {TextBox3.Text}
                    Dim Qos() As Byte = {ComboBox1.SelectedIndex}
                    client.Subscribe(Topic, Qos)
                    ToolStripStatusLabel1.Text = "Subscribe to " + "{" + TextBox3.Text + "}"

                Catch ex As Exception
                    ToolStripStatusLabel1.Text = "Error"
                    MsgBox(ex.Message, MsgBoxStyle.Critical)

                End Try


            Else
                ToolStripStatusLabel1.Text = "Please enter a valid topic "
            End If


        Else
            ToolStripStatusLabel1.Text = "Disconnected !! subscription procedure is not valid"
        End If


    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If (client IsNot Nothing AndAlso client.IsConnected()) Then
            If (TextBox4.Text.Length <> 0) Then
                Try
                    Dim Qos As Byte = ComboBox2.SelectedIndex
                    client.Publish(TextBox4.Text, Encoding.Default.GetBytes(RichTextBox1.Text), Qos, False)
                    ToolStripStatusLabel1.Text = "Publish to " + "{" + TextBox4.Text + "}"
                Catch ex As Exception
                    ToolStripStatusLabel1.Text = "Error"
                    MsgBox(ex.Message, MsgBoxStyle.Critical)
                End Try


            Else

                ToolStripStatusLabel1.Text = "Please enter a valid topic "
            End If
        Else
            ToolStripStatusLabel1.Text = "Disconnected !! Publish procedure is not valid"
        End If


    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Msg.Clear()
        RichTextBox2.Clear()
    End Sub
End Class
