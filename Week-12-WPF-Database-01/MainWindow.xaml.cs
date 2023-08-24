using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;



namespace Week_12_WPF_Database_01
{

    public partial class MainWindow : Window
    {
        private string dbName = "AKM_ictprg432";
        private string dbUser = "root";
        private string dbPassword = " ";
        private int dbPort = 3306;
        private string dbServer = "localhost";
        private string dbConnectionString = "";
        private MySqlConnection conn;

        public MainWindow()
        {
            InitializeComponent();
            dbConnectionString = $"server={dbServer}; user={dbUser}; database={dbName}; port={dbPort}; password={dbPassword}";
            conn = new MySqlConnection(dbConnectionString);
            conn.Open();

            // This code I got help from a online fourm to populate the combo box with data from my SQL database
            // Then I further use this code to populate my colunm combo boxes 

            DataTable tablesSchema = conn.GetSchema("Tables");

            foreach (DataRow row in tablesSchema.Rows)
            {
                string tableName = row["TABLE_NAME"].ToString();
                TableComboBox.Items.Add(tableName);
            }

            DataTable tablesSchema2 = conn.GetSchema("Tables");

            foreach (DataRow row in tablesSchema2.Rows)
            {
                string tableName = row["TABLE_NAME"].ToString();
                TableComboBox2.Items.Add(tableName);
            }
        }

        private void TableComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            string selectedTable = TableComboBox.SelectedItem.ToString();
            {
                DataTable columnsSchema = conn.GetSchema("Columns", new string[] { null, null, selectedTable });
                ColumnComboBox.Items.Clear();
                foreach (DataRow row in columnsSchema.Rows)
                {
                    string columnName = row["COLUMN_NAME"].ToString();
                    ColumnComboBox.Items.Add(columnName);
                }
                ColumnComboBox2.Items.Clear();
                foreach (DataRow row in columnsSchema.Rows)
                {
                    string columnName = row["COLUMN_NAME"].ToString();
                    ColumnComboBox2.Items.Add(columnName);
                }
            }
        }


        private void ShowTableButton_Click(object sender, RoutedEventArgs e)
        {
            string sqlQuery = $"Select  *  from {TableComboBox.SelectedItem} ;";
            if (conn.State == ConnectionState.Closed) 
                conn.Open();
            try
            {
                TableListbox.Items.Clear();
                MySqlCommand cmd = new MySqlCommand(sqlQuery, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                StringBuilder rowData = new StringBuilder();
                while (rdr.Read())
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        string columnValue = rdr.IsDBNull(i) ? "NULL" : rdr.GetValue(i).ToString();
                        rowData.Append(columnValue);
                        if (i < rdr.FieldCount - 1)
                            rowData.Append("\t");
                        if ((i + 1) % 10 == 0 || i == rdr.FieldCount - 1)
                        {
                            TableListbox.Items.Add(rowData.ToString());
                            rowData.Clear();
                        }
                    }
                rdr.Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            conn.Close();
        }

        private void ColumnSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (conn.State == ConnectionState.Closed) 
                conn.Open();
            TableListbox.Items.Clear();

            string selectedTable = TableComboBox.SelectedItem?.ToString() ?? string.Empty;
            string selectedColumn = ColumnComboBox.SelectedItem?.ToString() ?? string.Empty;
            string selectedColumn2 = ColumnComboBox2.SelectedItem?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(selectedTable) ||
               string.IsNullOrEmpty(selectedColumn) ||
                string.IsNullOrEmpty(selectedColumn2))
            {     
                MessageBox.Show("Please enter a selection in TABLE:, Column: and Column 2:", "ERROR");
                return;
            }
            StringBuilder sqlQuery = new StringBuilder($"SELECT * FROM {selectedTable}");
            MySqlCommand cmd = new MySqlCommand(sqlQuery.ToString(), conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                string column1Value = rdr.GetString(1);
                string column2Value = rdr.GetString(0);
                TableListbox.Items.Add($"{column1Value} : {column2Value}");
            }
            rdr.Close();
        }

        private void ShowColumnbutton_Click(object sender, RoutedEventArgs e)
        {
            if (conn.State == ConnectionState.Closed) 
                conn.Open();

            TableListbox.Items.Clear();
            string selectedTable = TableComboBox.SelectedItem?.ToString() ?? string.Empty;
            string selectedColumn = ColumnComboBox.SelectedItem?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(selectedTable) ||
              string.IsNullOrEmpty(selectedColumn))

            {
                MessageBox.Show("Please enter a selection in TABLE:, Column: and Column 2:", "ERROR");
                return;
            }

            StringBuilder sqlQuery = new StringBuilder($"SELECT {selectedColumn} FROM {selectedTable}");
            MySqlCommand cmd = new MySqlCommand(sqlQuery.ToString(), conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                string columnValue = rdr.GetString(0);
                TableListbox.Items.Add($"{columnValue} ");
            }

            rdr.Close();

        }

        private void ShowColumnbutton2_Click(object sender, RoutedEventArgs e)
        {
            if (conn.State == ConnectionState.Closed) 
                conn.Open();
            TableListbox.Items.Clear();
            string selectedTable = TableComboBox.SelectedItem?.ToString() ?? string.Empty;
            string selectedColumn2 = ColumnComboBox2.SelectedItem?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(selectedTable) ||
              string.IsNullOrEmpty(selectedColumn2))
            {
                MessageBox.Show("Please enter a selection in TABLE:, Column: and Column 2:", "ERROR");
                return;
            }

            StringBuilder sqlQuery = new StringBuilder($"SELECT {selectedColumn2} FROM {selectedTable}");
            MySqlCommand cmd = new MySqlCommand(sqlQuery.ToString(), conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                string columnValue = rdr.GetString(0);
                TableListbox.Items.Add($"{columnValue} ");
            }

            rdr.Close();
        }

        private void ColumnSearchButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedTable = TableComboBox.SelectedItem?.ToString() ?? string.Empty;
            string selectedColumn = ColumnComboBox.SelectedItem?.ToString() ?? string.Empty;
            string searchText = SearchTextbox.Text;

            if (string.IsNullOrEmpty(selectedTable) ||
              string.IsNullOrEmpty(selectedColumn))
            {
                MessageBox.Show("Please enter a selection in TABLE:, Column: and Column 2:", "ERROR");
                return;
            }
            if (string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Please enter a valid search value.");
                return;
            }

            StringBuilder sqlQuery = new StringBuilder($"SELECT * FROM {selectedTable} WHERE {selectedColumn} = '{searchText}'");
            MySqlCommand cmd = new MySqlCommand(sqlQuery.ToString(), conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            TableListbox.Items.Clear();

            if (rdr.HasRows)
            {
                rdr.Read();
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    TableListbox.Items.Add(rdr[i]);
                }
            }
            else
            {
                MessageBox.Show("No matching data found.");
            }
        }

        private void ColumnSearchButton2_Click(object sender, RoutedEventArgs e)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            string selectedTable = TableComboBox.SelectedItem?.ToString() ?? string.Empty;
            string selectedColumn = ColumnComboBox.SelectedItem?.ToString() ?? string.Empty;
            string selectedColumn2 = ColumnComboBox2.SelectedItem?.ToString() ?? string.Empty;
            string searchText = SearchTextbox.Text;

            if (string.IsNullOrEmpty(selectedTable) ||
               string.IsNullOrEmpty(selectedColumn) ||
                string.IsNullOrEmpty(selectedColumn2))
            {
                MessageBox.Show("Please enter a selection in TABLE:, Column: and Column 2:", "ERROR");
                return;
            }
            if (string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Please enter a valid search value.");
                return;
            }

            StringBuilder sqlQuery = new StringBuilder($"SELECT {selectedColumn}, {selectedColumn2} FROM {selectedTable} WHERE {selectedColumn} = '{searchText}'");
            MySqlCommand cmd = new MySqlCommand(sqlQuery.ToString(), conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            TableListbox.Items.Clear();

            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    string columnValue1 = rdr[selectedColumn].ToString();
                    string columnValue2 = rdr[selectedColumn2].ToString();
                    string ListBoxData = columnValue1 + ", " + columnValue2;
                    TableListbox.Items.Add(ListBoxData);
                }
            }
            else
            {
                MessageBox.Show("No matching data found.");
            }
            rdr.Close();
        }

        private void RowSearchButton2_Click(object sender, RoutedEventArgs e)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            string selectedTable = TableComboBox.SelectedItem?.ToString() ?? string.Empty;
            string selectedColumn = ColumnComboBox.SelectedItem?.ToString() ?? string.Empty;
            string searchText = SearchTextbox.Text;

            if (string.IsNullOrEmpty(selectedTable) ||
                string.IsNullOrEmpty(selectedColumn) ||
                string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Please enter a selection in TABLE:, Column: and have a value in SEARCH ", "ERROR");
                return;
            }

            if (string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Please enter a valid search value.");
                return;
            }

            StringBuilder sqlQuery = new StringBuilder($"SELECT * FROM {selectedTable} WHERE {selectedColumn} = '{searchText}'");
            MySqlCommand cmd = new MySqlCommand(sqlQuery.ToString(), conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            TableListbox.Items.Clear();

            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    StringBuilder rowData = new StringBuilder();

                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        string columnValue = rdr[i].ToString();
                        rowData.Append(columnValue);
                        rowData.Append(", ");
                    }

                    rowData.Length -= 2;
                    TableListbox.Items.Add(rowData.ToString());
                }
            }
            else
            {
                MessageBox.Show("No matching data found.");
            }

            rdr.Close();
        }

        private void RowSearchButtonByValue_Click(object sender, RoutedEventArgs e)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            string selectedTable = TableComboBox.SelectedItem?.ToString() ?? string.Empty;
            string selectedColumn = ColumnComboBox.SelectedItem?.ToString() ?? string.Empty;
            string searchText = SearchTextbox.Text;

            if (string.IsNullOrEmpty(selectedTable) ||
                string.IsNullOrEmpty(selectedColumn) ||
                string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Please enter a selection in TABLE:, Column: and have a value in SEARCH ", "ERROR");
                return;
            }

            if (!int.TryParse(searchText, out int searchInterger))
            {
                MessageBox.Show("Please enter a valid integer value.", "ERROR");
                return;
            }

            StringBuilder sqlQuery = new StringBuilder($"SELECT * FROM {selectedTable} WHERE {selectedColumn} > {searchInterger}");
            MySqlCommand cmd = new MySqlCommand(sqlQuery.ToString(), conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            TableListbox.Items.Clear();

            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    StringBuilder rowData = new StringBuilder();

                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        string columnValue = rdr[i].ToString();
                        rowData.Append(columnValue);
                        rowData.Append(", ");
                    }

                    rowData.Length -= 2;
                    TableListbox.Items.Add(rowData.ToString());
                }
            }
            else
            {
                MessageBox.Show("No matching data found.");
            }
            rdr.Close();
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            AddEmployee op1 = new AddEmployee();
            op1.ShowDialog();
        }

        private void DeleteEmployeeButton_Click(object sender, RoutedEventArgs e)

        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            string selectedTable = TableComboBox.SelectedItem?.ToString() ?? string.Empty;
            string selectedColumn = ColumnComboBox.SelectedItem?.ToString() ?? string.Empty;
            string searchText = SearchTextbox.Text;

            if (string.IsNullOrEmpty(selectedTable) ||
                string.IsNullOrEmpty(selectedColumn) ||
                string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Please enter a selection in TABLE:, Column: and have a value in SEARCH ", "ERROR");
                return;
            }

            StringBuilder sqlQuery = new StringBuilder($"SELECT * FROM {selectedTable} WHERE {selectedColumn} = '{searchText}'");
            MySqlCommand cmd = new MySqlCommand(sqlQuery.ToString(), conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            TableListbox.Items.Clear();
            StringBuilder SelectedRow = new StringBuilder();

            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    StringBuilder rowData = new StringBuilder();

                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        string columnValue = rdr[i].ToString();
                        rowData.Append(columnValue);
                        rowData.Append(", ");
                    }

                    rowData.Length -= 2;
                    TableListbox.Items.Add(rowData.ToString());
                }
            }
            else
            {
                MessageBox.Show("No matching data found.");
            }

            MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {SelectedRow.ToString()} ?", "Delete Employee", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                rdr.Close();

                sqlQuery = new StringBuilder($"DELETE FROM {selectedTable} WHERE {selectedColumn} = '{searchText}'");
                cmd = new MySqlCommand(sqlQuery.ToString(), conn);
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Row deleted successfully.");
                    TableListbox.Items.Clear();
                }
                else
                {
                    MessageBox.Show("Error occurred while deleting the row.");
                }
                TableListbox.Items.Clear();
            }
        }

        private void JoinQuerySearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            string selectedTable = TableComboBox.SelectedItem?.ToString() ?? string.Empty;
            string selectedColumn = ColumnComboBox.SelectedItem?.ToString() ?? string.Empty;
            string selectedColumn2 = ColumnComboBox2.SelectedItem?.ToString() ?? string.Empty;
            string searchText = SearchTextbox.Text;
            string selectedTable2 = TableComboBox2.SelectedItem?.ToString() ?? string.Empty;
            string selectedColumn3 = ColumnComboBox3.SelectedItem?.ToString() ?? string.Empty;
            string selectedColumn4 = ColumnComboBox4.SelectedItem?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(selectedTable) ||
                string.IsNullOrEmpty(selectedColumn) ||
                string.IsNullOrEmpty(selectedColumn2) ||
                string.IsNullOrEmpty(searchText) ||
                string.IsNullOrEmpty(selectedTable2) ||
                string.IsNullOrEmpty(selectedColumn3) ||
                string.IsNullOrEmpty(selectedColumn4))
            {
                MessageBox.Show("Please enter a selection in each Table and Column \nPlease enter a search in the SEARCH bar to Join the 2 Tables.", "ERROR");
                return;
            }

            StringBuilder sqlQuery = new StringBuilder($"SELECT {selectedTable}.{selectedColumn}, {selectedTable}.{selectedColumn2}," +
                $" SUM({selectedTable2}.{selectedColumn3}) AS {selectedColumn3}" +
                $" FROM {selectedTable}" +
                $" JOIN {selectedTable2} ON {selectedTable}.{selectedColumn} = {selectedTable2}.{selectedColumn4} " +
                $" WHERE {selectedTable}.{selectedColumn} = {searchText} " +
                $" GROUP BY  {selectedTable}.{selectedColumn}, {selectedTable}.{selectedColumn2}");

           

             MySqlCommand cmd = new MySqlCommand(sqlQuery.ToString(), conn);
             MySqlDataReader rdr = cmd.ExecuteReader();
             TableListbox.Items.Clear();
          
             if (rdr.HasRows)
             {
                 while (rdr.Read())
                 {
                     StringBuilder rowData = new StringBuilder();
          
                     for (int i = 0; i < rdr.FieldCount; i++)
                     {
                         string columnValue = rdr[i].ToString();
                         rowData.Append(columnValue);
                         rowData.Append(", ");
                     }
          
                     rowData.Length -= 2;
          
                     TableListbox.Items.Add(rowData.ToString());
                 }
             }
             else
             {
                 MessageBox.Show("No matching data found.");
             }
          
          
             rdr.Close();
           
          
        }

        private void TableComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            string selectedTable = TableComboBox2.SelectedItem.ToString();
            {
                DataTable columnsSchema = conn.GetSchema("Columns", new string[] { null, null, selectedTable });
                ColumnComboBox3.Items.Clear();
                foreach (DataRow row in columnsSchema.Rows)
                {
                    string columnName = row["COLUMN_NAME"].ToString();
                    ColumnComboBox3.Items.Add(columnName);
                }
                ColumnComboBox4.Items.Clear();
                foreach (DataRow row in columnsSchema.Rows)
                {
                    string columnName = row["COLUMN_NAME"].ToString();
                    ColumnComboBox4.Items.Add(columnName);
                }
            }
        }

        private void ChangeColumnValueButton_Click(object sender, RoutedEventArgs e)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            string selectedTable = TableComboBox.SelectedItem?.ToString() ?? string.Empty;
            string selectedColumn = ColumnComboBox.SelectedItem?.ToString() ?? string.Empty;
            string selectedColumn2 = ColumnComboBox2.SelectedItem?.ToString() ?? string.Empty;
            string searchText = SearchTextbox.Text;
            string valueToBeChanged = AddValueTextBox.Text;

            if (string.IsNullOrEmpty(selectedTable) ||
                string.IsNullOrEmpty(selectedColumn) ||
                string.IsNullOrEmpty(selectedColumn2) ||
                string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Please enter a selection in each Table, Column and Column 2: \nPlease enter values in the SEARCH and the Text box next to this button.", "ERROR");
                return;
            }

            if (string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Please enter a valid search value.");
                return;
            }

            if (string.IsNullOrEmpty(valueToBeChanged))
            {
                MessageBox.Show("Please enter a value to be changed");
                return;
            }

            StringBuilder sqlQuery = new StringBuilder($"SELECT * FROM {selectedTable} WHERE {selectedColumn} = '{searchText}'");
            MySqlCommand cmd = new MySqlCommand(sqlQuery.ToString(), conn);
            MySqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    StringBuilder rowData = new StringBuilder();

                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        string columnValue = rdr[i].ToString();
                        rowData.Append(columnValue);
                        rowData.Append(", ");
                    }

                    rowData.Length -= 2;
                    TableListbox.Items.Add(rowData.ToString());
                }
            }
            else
            {
                MessageBox.Show("No matching data found.");
            }

            if (rdr.HasRows)
            {
                rdr.Read();

                MessageBoxResult result = MessageBox.Show($"Are you sure you want to change the value of {selectedColumn2} of the selected row to {valueToBeChanged} ?", "Confirm Value Change", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    rdr.Close();

                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    sqlQuery = new StringBuilder($"UPDATE {selectedTable} SET {selectedColumn2} = '{valueToBeChanged}' WHERE {selectedColumn} = '{searchText}'");
                    cmd = new MySqlCommand(sqlQuery.ToString(), conn);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Value changed successfully.");
                        TableListbox.Items.Clear();
                    }
                    else
                    {
                        MessageBox.Show("Error occurred while changing the value.");
                    }
                }
            }
            else
            {
                MessageBox.Show("No matching data found.");
            }
            rdr.Close();
        }
  
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            string QueryData = SearchTextbox.Text;
            if (string.IsNullOrEmpty(QueryData))
            {
                MessageBox.Show("Please enter a valid search value.");
                return;
            }

            StringBuilder sqlQuery = new StringBuilder($"{QueryData}");

            try
            {
                MySqlCommand cmd = new MySqlCommand(sqlQuery.ToString(), conn);
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    TableListbox.Items.Clear(); 

                    while (rdr.Read())
                    {
                        StringBuilder rowData = new StringBuilder();

                        for (int i = 0; i < rdr.FieldCount; i++)
                        {
                            string columnValue = rdr[i].ToString();
                            rowData.Append(columnValue);
                            rowData.Append(", ");
                        }

                        rowData.Length -= 2;
                        TableListbox.Items.Add(rowData.ToString());
                    }
                }
            }
            catch (MySqlException InvalidSqlQuery)
            {
                MessageBox.Show("An error occurred while executing the SQL query: " + InvalidSqlQuery.Message);
            }
        }

        private void JoinQuerySearchSeperateSalesButton_Click(object sender, RoutedEventArgs e)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            string selectedTable = TableComboBox.SelectedItem?.ToString() ?? string.Empty;
            string selectedColumn = ColumnComboBox.SelectedItem?.ToString() ?? string.Empty;
            string selectedColumn2 = ColumnComboBox2.SelectedItem?.ToString() ?? string.Empty;
            string searchText = SearchTextbox.Text;
            string selectedTable2 = TableComboBox2.SelectedItem?.ToString() ?? string.Empty;
            string selectedColumn3 = ColumnComboBox3.SelectedItem?.ToString() ?? string.Empty;
            string selectedColumn4 = ColumnComboBox4.SelectedItem?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(selectedTable) ||
                string.IsNullOrEmpty(selectedColumn) ||
                string.IsNullOrEmpty(selectedColumn2) ||
                string.IsNullOrEmpty(searchText) ||
                string.IsNullOrEmpty(selectedTable2) ||
                string.IsNullOrEmpty(selectedColumn3) ||
                string.IsNullOrEmpty(selectedColumn4))
            {
                MessageBox.Show("Please enter a selection in each Table and Column \nPlease enter a search in the SEARCH bar to Join the 2 Tables.", "ERROR");
                return;
            }

            StringBuilder sqlQuery = new StringBuilder($"SELECT {selectedTable}.{selectedColumn}, {selectedTable}.{selectedColumn2}," +
                $" {selectedTable2}.{selectedColumn3},{selectedTable2}.{selectedColumn4}, working_with.client_id" +
                $" FROM {selectedTable}" +
                $" JOIN {selectedTable2} ON {selectedTable}.{selectedColumn} = {selectedTable2}.{selectedColumn4} " +
                $" WHERE {selectedTable}.{selectedColumn} = {searchText} ");
                                
             MySqlCommand cmd = new MySqlCommand(sqlQuery.ToString(), conn);
             MySqlDataReader rdr = cmd.ExecuteReader();
             TableListbox.Items.Clear();
            
             if (rdr.HasRows)
             {
                 while (rdr.Read())
                 {
                     StringBuilder rowData = new StringBuilder();
            
                     for (int i = 0; i < rdr.FieldCount; i++)
                     {
                         string columnValue = rdr[i].ToString();
                         rowData.Append(columnValue);
                         rowData.Append(", ");
                     }
            
                     rowData.Length -= 2;
            
                     TableListbox.Items.Add(rowData.ToString());
                 }
             }
             else
             {
                 MessageBox.Show("No matching data found.");
             }        
           
             rdr.Close();
            
        }
    }
}
        
        
        
        
        
        
        
        
       