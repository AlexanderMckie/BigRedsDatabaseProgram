using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Week_12_WPF_Database_01
{
 
    public partial class AddEmployee : Window
    {
    
        private string dbName = "AKM_ictprg432";
        private string dbUser = "root";
        private string dbPassword = " ";
        private int dbPort = 3306;
        private string dbServer = "localhost";
        private string dbConnectionString = "";
        private MySqlConnection conn;
        public AddEmployee()
        {
            InitializeComponent();
            dbConnectionString = $"server={dbServer}; user={dbUser}; database={dbName}; port={dbPort}; password={dbPassword}";
            conn = new MySqlConnection(dbConnectionString);
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            string branchQuery = "SELECT branch_id FROM employees";
            MySqlCommand branchCmd = new MySqlCommand(branchQuery, conn);
            MySqlDataReader branchReader = branchCmd.ExecuteReader();
            BranchComboBox.Items.Clear();

            while (branchReader.Read())
            {
                string branchId = branchReader["branch_id"].ToString();
                BranchComboBox.Items.Add(branchId);
            }

            branchReader.Close();

            
            string supervisorQuery = "SELECT supervisor_id FROM employees";
            MySqlCommand supervisorCmd = new MySqlCommand(supervisorQuery, conn);
            MySqlDataReader supervisorReader = supervisorCmd.ExecuteReader();
            SupervisorComboBox.Items.Clear();

            while (supervisorReader.Read())
            {
                string supervisorId = supervisorReader["supervisor_id"].ToString();
                SupervisorComboBox.Items.Add(supervisorId);
            }

            supervisorReader.Close();

           
            string genderQuery = "SELECT gender_identity FROM employees";
            MySqlCommand genderCmd = new MySqlCommand(genderQuery, conn);
            MySqlDataReader genderReader = genderCmd.ExecuteReader();
            GenderCombobox.Items.Clear();

            while (genderReader.Read())
            {
                string genderIdentity = genderReader["gender_identity"].ToString();
                GenderCombobox.Items.Add(genderIdentity);
            }

            genderReader.Close();
        }

        private void Clear_Button_Click(object sender, RoutedEventArgs e)
        {
            FirstNameTextbox.Text = string.Empty;
            LastNameTextbox.Text = string.Empty;
            DOB_DayTextbox.Text = string.Empty;
            DOB_MonthTextbox.Text = string.Empty;
            DOB_YearTextbox.Text = string.Empty;
            BranchComboBox.SelectedItem = null;
            GenderCombobox.SelectedItem = null;
            SalaryCombobox.Text = string.Empty;
            SupervisorComboBox.SelectedItem = null;
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            if(conn.State == ConnectionState.Closed)
                conn.Open();
            string selectedTable = "employees";
            string dateOfBirth;
            string familyName = LastNameTextbox.Text;
            string branchId = BranchComboBox.SelectedItem.ToString();
            string supervisorId = SupervisorComboBox.SelectedItem.ToString();
            string givenName = FirstNameTextbox.Text;
            string grossSalary = SalaryCombobox.Text;
            string genderIdentity = GenderCombobox.SelectedItem.ToString();

             if (int.TryParse(DOB_YearTextbox.Text, out int year) &&
                int.TryParse(DOB_MonthTextbox.Text, out int month) &&
                int.TryParse(DOB_DayTextbox.Text, out int day))
            {
                dateOfBirth = $"{year}-{month}-{day}";
            }
            else
            {
                MessageBox.Show("Please enter valid number for date of birth.");
                return;
            }

            if (!int.TryParse(grossSalary, out int salary))
            {
                MessageBox.Show("Please enter a valid number for gross salary.");
                return;
            }

            int id;
            string getMaxIdQuery = $"SELECT MAX(id) FROM {selectedTable}";
            MySqlCommand getMaxIdCmd = new MySqlCommand(getMaxIdQuery, conn);
            object maxIdResult = getMaxIdCmd.ExecuteScalar();
            if (maxIdResult != null && int.TryParse(maxIdResult.ToString(), out int maxId))
            {
                id = maxId + 1;
            }
            else
            {
                // default id value if the id column is empty
                id = 1111;
            }


            string addRowQuery = $"INSERT INTO {selectedTable} (date_of_birth, family_name, branch_id, supervisor_id, given_name, gross_salary, gender_identity, id) " +
                $"VALUES ('{dateOfBirth}', '{familyName}', '{branchId}', '{supervisorId}', '{givenName}', {salary}, '{genderIdentity}', {id})";

            MySqlCommand addRowCmd = new MySqlCommand(addRowQuery, conn);
            int rowsAffected = addRowCmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                MessageBox.Show("New Employee added successfully.");
            }
            else
            {
                MessageBox.Show("Failed to add new Employee.");
            }
        }
    }
}
