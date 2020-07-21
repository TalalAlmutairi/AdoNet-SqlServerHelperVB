Imports System.Data.SqlClient

Public Class Examples
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    ' Getting all employees data
    Protected Sub btnLoadData_Click(sender As Object, e As EventArgs) Handles btnLoadData.Click
        Dim sql As String = "SELECT * FROM Employees"
        GridView1.DataSource = SqlHelper.ExecuteQuery(sql, CommandType.Text, Nothing)
        GridView1.DataBind()
    End Sub

    ' Selecting only the employee with id=2
    Protected Sub btnSqlWhere_Click(sender As Object, e As EventArgs) Handles btnSqlWhere.Click
        Dim sql As String = "SELECT * FROM Employees WHERE EmpID=@ID"

        'SqlParameter uses for security to prevent SQL injection
        '@ID in the SQL above must match the parameter in SqlParameter
        Dim parametersList As SqlParameter() = New SqlParameter() {
            New SqlParameter("@ID", "1")
        }
        GridView1.DataSource = SqlHelper.ExecuteQuery(sql, CommandType.Text, parametersList)
        GridView1.DataBind()
    End Sub

    ' Getting the maximum salary of all employees as only one value
    Protected Sub btnExecuteScalar_Click(sender As Object, e As EventArgs) Handles btnExecuteScalar.Click
        Dim sql As String = "SELECT MAX(Age) FROM Employees"
        lbMsg.Text = SqlHelper.ExecuteScalar(sql, CommandType.Text, Nothing)
    End Sub

    ' Execute stored procedure
    Protected Sub btnSP_Click(sender As Object, e As EventArgs) Handles btnSP.Click
        GridView1.DataSource = SqlHelper.ExecuteQuery("GetAllEmployees", CommandType.StoredProcedure, Nothing)
        GridView1.DataBind()
    End Sub

    Protected Sub btnInsert_Click(sender As Object, e As EventArgs) Handles btnInsert.Click
        Dim sql As String = "INSERT INTO Employees VALUES(@FName,@LName,@Age,@CountryID)"
        Dim parametersList As SqlParameter() = New SqlParameter() {
            New SqlParameter("@FName", txtFName.Value),
            New SqlParameter("@LName", txtLName.Value),
            New SqlParameter("@Age", txtAge.Value),
            New SqlParameter("@CountryID", ddlCouontries.Value)
        }

        If SqlHelper.ExecuteNonQuery(sql, CommandType.Text, parametersList) Then
            lbMsg.Text = "Inserted successfully"
        Else
            lbMsg.Text = "Error"
        End If
    End Sub

    Protected Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Dim sql As String = "UPDATE Employees SET FirstName=@FName,LastName=@LName,Age=@Age,CountryID=@CountryID WHERE EmpID =@ID"
        Dim parametersList As SqlParameter() = New SqlParameter() {
            New SqlParameter("@ID", txtEmpID.Value),
            New SqlParameter("@FName", txtFName.Value),
            New SqlParameter("@LName", txtLName.Value),
            New SqlParameter("@Age", txtAge.Value),
            New SqlParameter("@CountryID", ddlCouontries.Value)
        }

        If SqlHelper.ExecuteNonQuery(sql, CommandType.Text, parametersList) Then
            lbMsg.Text = "Updated successfully"
        Else
            lbMsg.Text = "Error"
        End If
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim sql As String = "DELETE FROM Employees WHERE EmpID =@ID"
        Dim parametersList As SqlParameter() = New SqlParameter() {
            New SqlParameter("@ID", txtEmpID.Value)
        }

        If SqlHelper.ExecuteNonQuery(sql, CommandType.Text, parametersList) Then
            lbMsg.Text = "Deleted successfully"
        Else
            lbMsg.Text = "Error"
        End If
    End Sub

    'Execute two SQL statements Insert and update, which all SQL statements in a single transaction, rolling back if an error has occurred
    Protected Sub btnExecuteTransaction_Click(sender As Object, e As EventArgs) Handles btnExecuteTransaction.Click
        Dim listOfSQLs As ArrayList = New ArrayList()
        Dim listOfParamerters As List(Of SqlParameter()) = New List(Of SqlParameter())()

        Dim sql1 As String = "INSERT INTO Employees VALUES(@FName,@LName,@Age,@CountryID)"
        Dim parameters1 As SqlParameter() = New SqlParameter() {
            New SqlParameter("@FName", "Test F Name"),
            New SqlParameter("@LName", "Test L Name"),
            New SqlParameter("@Age", 25),
            New SqlParameter("@CountryID", 1)
            }
        listOfSQLs.Add(sql1)
        listOfParamerters.Add(parameters1)

        Dim sql2 As String = "UPDATE Employees SET FirstName=@FName,LastName=@LName,Age=@Age,CountryID=@CountryID WHERE EmpID =@ID"
        Dim parameters2 As SqlParameter() = New SqlParameter() {
            New SqlParameter("@ID", 4),
            New SqlParameter("@FName", "New F Name"),
            New SqlParameter("@LName", "New L Name"),
            New SqlParameter("@Age", 30),
            New SqlParameter("@CountryID", 2)
        }
        listOfSQLs.Add(sql2)
        listOfParamerters.Add(parameters2)

        If SqlHelper.ExecuteTransaction(listOfSQLs, listOfParamerters) Then
            lbMsg.Text = "All SQL statements executed successfully"
        Else
            lbMsg.Text = "Error"
        End If
    End Sub

    'Execute two select queries, and returns employees and country tables
    Protected Sub btnReturnDS_Click(sender As Object, e As EventArgs) Handles btnReturnDS.Click
        ' You can also use a stored procedure instead  of two queries in one string variable
        Dim sql As String = "SELECT * FROM Employees; SELECT * FROM Country"
        Dim ds As DataSet = SqlHelper.ExecuteQueryDS(sql, CommandType.Text, Nothing)

        GridViewEmp.DataSource = ds.Tables(0)
        GridViewEmp.DataBind()

        GridViewCountry.DataSource = ds.Tables(1)
        GridViewCountry.DataBind()
    End Sub
End Class