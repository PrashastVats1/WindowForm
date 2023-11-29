using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private ErrorProvider errorProvider;
        //private ToolTip toolTip1;

        public Form1()
        {
            InitializeComponent();
            errorProvider = new ErrorProvider();

            // Populate Job Title dropdown list
            comboBox1.Items.AddRange(new string[] { "Manager", "Developer", "Analyst", "UI/UX Designer", "HR Manager", "Finance Manager", "Tester" });
            // Set the DropDownStyle property to DropDownList
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                string connectionString = "Data Source=DESKTOP-NGKK7DI;Initial Catalog=WinFormDb;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string insertQuery = "INSERT INTO Employee (FirstName, LastName, MobileNo, JobTitle, Salary) " +
                                         "VALUES (@FirstName, @LastName, @MobileNo, @JobTitle, @Salary)";

                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", textBox1.Text);
                        command.Parameters.AddWithValue("@LastName", textBox2.Text);
                        command.Parameters.AddWithValue("@MobileNo", textBox3.Text);
                        command.Parameters.AddWithValue("@JobTitle", comboBox1.SelectedItem.ToString());
                        command.Parameters.AddWithValue("@Salary", decimal.Parse(textBox5.Text));

                        command.ExecuteNonQuery();
                    }

                    // Retrieve all data and display it
                    string selectQuery = "SELECT * FROM Employee";
                    using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
                    {
                        using (SqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            string allData = "Employee Data:\n";
                            while (reader.Read())
                            {
                                allData += $"ID: {reader["EmployeeID"]}, FirstName: {reader["FirstName"]}, LastName: {reader["LastName"]}, MobileNo: {reader["MobileNo"]}, JobTitle: {reader["JobTitle"]}, Salary: {reader["Salary"]}\n";
                            }

                            // Show the data in a message box
                            MessageBox.Show(allData, "Employee Data");
                        }
                    }

                    connection.Close();
                }

                MessageBox.Show("Data inserted successfully!");
                this.Close();
            }
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            // Clear previous error provider messages
            errorProvider.Clear();

            // First Name required
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                errorProvider.SetError(textBox1, "First Name is required");
                isValid = false;
            }

            // Last Name required
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                errorProvider.SetError(textBox2, "Last Name is required");
                isValid = false;
            }

            // Mobile No required
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                errorProvider.SetError(textBox3, "Mobile number is required");
                isValid = false;
            }
            else if (!Regex.IsMatch(textBox3.Text, @"^\d+$"))
            {
                // Mobile No should have only digits
                errorProvider.SetError(textBox3, "Input field should have only digits");
                isValid = false;
            }
            else if (textBox3.Text.Length != 10)
            {
                // Mobile No should have exactly 10 digits
                errorProvider.SetError(textBox3, "Input field must have exactly 10 digits");
                isValid = false;
            }

            // Job Title required
            if (comboBox1.SelectedItem == null)
            {
                errorProvider.SetError(comboBox1, "Job Title is required");
                isValid = false;
            }

            // Salary required
            if (string.IsNullOrWhiteSpace(textBox5.Text))
            {
                errorProvider.SetError(textBox5, "Salary is required");
                isValid = false;
            }
            else if (!decimal.TryParse(textBox5.Text, out _))
            {
                // Salary must be a numeric value
                errorProvider.SetError(textBox5, "Salary must be a numeric value");
                isValid = false;
            }
            else if (decimal.Parse(textBox5.Text) < 0)
            {
                // Salary must be a non-negative value
                errorProvider.SetError(textBox5, "Salary must be a non-negative value");
                isValid = false;
            }

            // Display the error messages in a visible tooltip
            if (!isValid)
            {
                MessageBox.Show("Please correct the validation errors.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return isValid;
        }
    }
}
