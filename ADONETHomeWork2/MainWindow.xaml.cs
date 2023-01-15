using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Documents;

namespace ADONETHomeWork2;

public partial class MainWindow : Window
{
    SqlConnection? conn = null;

    SqlDataAdapter? adapter = null;
    SqlCommandBuilder? cmdBuilder = null;

    DataTable? table = null;
    DataSet? dataSet = null;

    public MainWindow()
    {
        InitializeComponent();
        conn = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]);
    }

    private void btn_Fill_Click(object sender, RoutedEventArgs e)
    {
        adapter = new SqlDataAdapter("SELECT * FROM Authors", conn);
        cmdBuilder = new SqlCommandBuilder(adapter);
        table = new DataTable();
        adapter.Fill(table);
        DataGrid.ItemsSource = table.AsDataView();
    }

    private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        if (adapter is null) return;
        table?.Clear();
        string txt = tBox.Text;
        adapter.SelectCommand.CommandText = $"SELECT * FROM Authors WHERE UPPER(FirstName) LIKE UPPER('%{txt}%') OR UPPER(LastName) LIKE UPPER('%{txt}%')";
        adapter.Fill(table);
        DataGrid.ItemsSource = table.AsDataView();
        adapter.SelectCommand.CommandText = "SELECT * FROM Authors";
    }

    private void btn_Update_Click(object sender, RoutedEventArgs e)
    {
        SqlCommand updateCommand = new()
        {
            CommandText = "usp_updateAuthors",
            CommandType = CommandType.StoredProcedure,
            Connection = conn
        };

        updateCommand.Parameters.Add("aId", SqlDbType.Int);
        updateCommand.Parameters["aId"].SourceVersion = DataRowVersion.Original;
        updateCommand.Parameters["aId"].SourceColumn = "Id";


        updateCommand.Parameters.Add("aFirstName", SqlDbType.NVarChar);
        updateCommand.Parameters["aFirstName"].SourceVersion = DataRowVersion.Current;
        updateCommand.Parameters["aFirstName"].SourceColumn = "FirstName";

        updateCommand.Parameters.Add("aLastName", SqlDbType.NVarChar);
        updateCommand.Parameters["aLastName"].SourceVersion = DataRowVersion.Current;
        updateCommand.Parameters["aLastName"].SourceColumn = "LastName";

        adapter.UpdateCommand = updateCommand;

        try
        {
            adapter.Update(table);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void btn_Delete_Click(object sender, RoutedEventArgs e)
    {
        DataRow? row = (DataGrid.SelectedItem as DataRowView)?.Row;
        if (row is null)
            return;
        row.Delete();
        adapter.Update(table);
    }
}
