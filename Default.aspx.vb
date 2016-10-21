Imports System.Data.SqlClient
Imports System.Data
Imports System.Configuration

Partial Class _Default
    Inherits Page

    Const DASH As String = "-"
    Const ZERO As String = "0"
    Const COMMA As String = ","

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ddlDOW.Text = 1
        End If
    End Sub

    Protected Sub lbGet_Click(sender As Object, e As EventArgs) Handles lbGet.Click

        Dim sbGet As New StringBuilder
        Dim strDOW As String = ddlDOW.SelectedValue.ToString
        Dim strTime As String = ddlTime.SelectedValue.ToString
        Dim strTown As String = ddlTown.SelectedValue.ToString

        ' ? SqlDsList.EnableCaching = "True"

        ' Get Day of week
        If strDOW <> DASH Then
            If strDOW <> ZERO Then
                sbGet.Append(String.Format(" DOW = {0}", strDOW))
            Else
                sbGet.Append(String.Format(" DOW > {0}", 0))
            End If
        End If

        ' Get time 
        If strTime <> ZERO Then
            If sbGet.Length > 0 Then
                sbGet.Append(String.Format(" AND TimeID = {0}", strTime))
            End If
        End If

        ' Get town
        If strTown <> DASH Then
            If sbGet.Length > 0 Then
                sbGet.Append(String.Format(" AND Town = '{0}'", strTown))
            End If
        Else
            sbGet.Append(String.Format(" AND Town like '{0}'", "%"))
        End If


        ' Set the filter
        If sbGet.Length > 0 Then

            SqlDsList.FilterExpression = sbGet.ToString()
            ' lblFilter.Text = "Filter: " & SqlDsList.FilterExpression.ToString()

        Else

            SqlDsList.FilterExpression = vbNull
            ' lblFilter.Text = "Filter: " & SqlDsList.FilterExpression.ToString()

        End If

    End Sub

    
    Protected Sub lnkExport_Click(sender As Object, e As EventArgs) Handles lnkExport.Click

        ' Stored procedure: GetMeetingList

        ' Modified from
        ' http: //www.aspsnippets.com/Articles/Export-data-from-SQL-Server-to-Text-file-in-C-and-VBNet.aspx

        Dim rc As Integer = -1
        Dim cnstr As String = ConfigurationManager.ConnectionStrings("cnDEIG").ConnectionString
        Dim cn As New SqlConnection(cnstr)
        Dim cmd As New SqlCommand("GetMeetingList")
        cmd.CommandType = CommandType.StoredProcedure
        Dim da As New SqlDataAdapter()

        ' Get the connection into the SqlCommand object
        cmd.Connection = cn

        da.SelectCommand = cmd

        Dim dt As New DataTable()

        Try
            rc = da.Fill(dt)
        Catch ex As Exception
            ' lblInfo.Text = ex.ToString()
        End Try

        ' Read the table to create a file for export
        Dim sb As New StringBuilder()

        ' Get the column name
        For Each column As DataColumn In dt.Columns
            'Add the Header row for Text file.
            sb.Append(column.ColumnName & COMMA)
        Next

        'Add new line.
        sb.Append(vbCr & vbLf)

        ' Get the data
        For Each row As DataRow In dt.Rows
            For Each column As DataColumn In dt.Columns
                'Add the Data rows.
                ' DataTable.rows(i)(j)
                ' (row(column).tostring() works as well.
                sb.Append(row(column.ColumnName).ToString() & COMMA)
            Next

            'Add new line.
            sb.Append(vbCr & vbLf)
        Next

        'Download the Text file.
        Response.Clear()
        Response.Buffer = True
        Response.AddHeader("content-disposition", "attachment;filename=MeetingList.csv")
        Response.Charset = ""
        Response.ContentType = "application/text"
        Response.Output.Write(sb.ToString())
        Response.Flush()
        Response.End()


    End Sub

   
End Class