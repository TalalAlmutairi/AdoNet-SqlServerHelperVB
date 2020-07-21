# AdoNet-SqlServerHelperVB
A class for applying database operations to SQL Server such as query, executing statements, transactions,and returning multi tables.

    Public Class SqlHelper
    'You need to change connection string from Web.config
    Private Shared ReadOnly connectionString As String = ConfigurationManager.ConnectionStrings("DbConnecion").ConnectionString
    ''' <summary>
    ''' Execute Select query and return results as a DataTable
    ''' </summary>
    ''' <param name="cmdText"></param>
    ''' <param name="cmdType"></param>
    ''' <param name="parameters"></param>
    ''' <returns>DataTable</returns>
    Public Shared Function ExecuteQuery(ByVal cmdText As String, ByVal cmdType As CommandType, ByVal parameters As SqlParameter()) As DataTable
        Dim table As DataTable = New DataTable()

        Try

            Using con As SqlConnection = New SqlConnection(connectionString)

                Using cmd As SqlCommand = New SqlCommand(cmdText, con)
                    con.Open()
                    cmd.CommandType = cmdType
                    If parameters IsNot Nothing Then cmd.Parameters.AddRange(parameters)
                    Dim adapter As SqlDataAdapter = New SqlDataAdapter(cmd)
                    adapter.Fill(table)
                End Using
            End Using

        Catch ex As Exception
            'Save ex to logger
            Dim [error] As String = ex.Message
            Return Nothing
        End Try

        Return table
    End Function

    ''' <summary>
    ''' Executes a SQL statement and returns the number of rows affected. NonQuery (Insert, update, and delete)
    ''' </summary>
    ''' <param name="cmdText"></param>
    ''' <param name="cmdType"></param>
    ''' <param name="parameters"></param>
    ''' <returns>Boolean</returns>
    Public Shared Function ExecuteNonQuery(ByVal cmdText As String, ByVal cmdType As CommandType, ByVal parameters As SqlParameter()) As Boolean
        Dim value = 0

        Try

            Using con As SqlConnection = New SqlConnection(connectionString)

                Using cmd As SqlCommand = New SqlCommand(cmdText, con)
                    con.Open()
                    cmd.CommandType = cmdType
                    If parameters IsNot Nothing Then cmd.Parameters.AddRange(parameters)
                    value = cmd.ExecuteNonQuery()
                End Using
            End Using

        Catch ex As Exception
            'Save ex to logger
            Dim [error] As String = ex.Message
            Return False
        End Try

        If value < 0 Then
            Return False
        Else
            Return True
        End If
    End Function

    ''' <summary>
    ''' Executes the query, and returns the first column of the first row in the result set returned by the query.
    ''' </summary>
    ''' <param name="cmdText"></param>
    ''' <param name="cmdType"></param>
    ''' <param name="parameters"></param>
    ''' <returns></returns>
    Public Shared Function ExecuteScalar(ByVal cmdText As String, ByVal cmdType As CommandType, ByVal parameters As SqlParameter()) As String
        Try

            Using con As SqlConnection = New SqlConnection(connectionString)

                Using cmd As SqlCommand = New SqlCommand(cmdText, con)
                    con.Open()
                    cmd.CommandType = cmdType
                    If parameters IsNot Nothing Then cmd.Parameters.AddRange(parameters)
                    Dim value As String = cmd.ExecuteScalar().ToString()
                    Return value
                End Using
            End Using

        Catch ex As Exception
            'Save ex to logger
            Dim [error] As String = ex.Message
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Executes the query, and returns multiple tables
    ''' </summary>
    ''' <param name="cmdText"></param>
    ''' <param name="cmdType"></param>
    ''' <param name="parameters"></param>
    ''' <returns>DataSet</returns>
    Public Shared Function ExecuteQueryDS(ByVal cmdText As String, ByVal cmdType As CommandType, ByVal parameters As SqlParameter()) As DataSet
        Dim tables As DataSet = New DataSet()

        Try

            Using con As SqlConnection = New SqlConnection(connectionString)

                Using cmd As SqlCommand = New SqlCommand(cmdText, con)
                    con.Open()
                    cmd.CommandType = cmdType
                    If parameters IsNot Nothing Then cmd.Parameters.AddRange(parameters)
                    Dim adapter As SqlDataAdapter = New SqlDataAdapter(cmd)
                    adapter.Fill(tables)
                End Using
            End Using

        Catch ex As Exception
            'Save ex to logger
            Dim [error] As String = ex.Message
            Return Nothing
        End Try

        Return tables
    End Function

    ''' <summary>
    ''' Execute multiple SQL statements such as Insert, update, and delete, which all SQL statements in a single transaction, rolling back if an error has occurred
    ''' </summary>
    ''' <param name="cmdText"></param>
    ''' <param name="paramerters"></param>
    ''' <returns>Boolean</returns>
    Public Shared Function ExecuteTransaction(ByVal cmdText As ArrayList, ByVal paramerters As List(Of SqlParameter())) As Boolean
        Using con As SqlConnection = New SqlConnection(connectionString)
            Dim cmd As SqlCommand = New SqlCommand()
            Dim transaction As SqlTransaction = Nothing
            cmd.Connection = con

            Try
                con.Open()
                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted)
                cmd.Connection = con
                cmd.Transaction = transaction

                For i As Integer = 0 To cmdText.Count - 1
                    cmd.CommandText = cmdText(i).ToString()
                    If paramerters(i).Length > 0 Then cmd.Parameters.AddRange(paramerters(i))
                    Dim flag As Integer = cmd.ExecuteNonQuery()
                    cmd.Parameters.Clear()
                    If flag < 1 Then Return False
                Next

                transaction.Commit()
                Return True
            Catch ex As Exception
                transaction.Rollback()
                'Save ex to logger
                Dim [error] As String = ex.Message
                Return False
            End Try
        End Using
    End Function
End Class


Example of using SqlHelper.vb

        Dim sql As String = "SELECT * FROM Employees"
        GridView1.DataSource = SqlHelper.ExecuteQuery(sql, CommandType.Text, Nothing)
        GridView1.DataBind()
        
        
        Dim sql As String = "SELECT * FROM Employees WHERE EmpID=@ID"
        'SqlParameter uses for security to prevent SQL injection
        '@ID in the SQL above must match the parameter in SqlParameter
        Dim parametersList As SqlParameter() = New SqlParameter() {
            New SqlParameter("@ID", "1")
        }
        GridView1.DataSource = SqlHelper.ExecuteQuery(sql, CommandType.Text, parametersList)
        GridView1.DataBind()
