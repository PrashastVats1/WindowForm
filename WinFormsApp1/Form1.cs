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
            toolTip1 = new ToolTip();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                string connectionString = "Data Source=LAPTOP-H3OMMTNN\\SQLEXPRESS;Initial Catalog=WinFormDb;Integrated Security=True";

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
                        command.Parameters.AddWithValue("@JobTitle", textBox4.Text);
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

            // Clear previous tool tip text
            toolTip1.RemoveAll();

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                toolTip1.SetToolTip(textBox1, "First Name is required");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                toolTip1.SetToolTip(textBox2, "Last Name is required");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                toolTip1.SetToolTip(textBox3, "Mobile number is required");
                isValid = false;
            }
            else if (!Regex.IsMatch(textBox3.Text, @"^\d{10}$"))
            {
                toolTip1.SetToolTip(textBox3, "Mobile number should be 10 digits");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                toolTip1.SetToolTip(textBox4, "Job Title is required");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(textBox5.Text))
            {
                toolTip1.SetToolTip(textBox5, "Salary is required");
                isValid = false;
            }
            else if (!decimal.TryParse(textBox5.Text, out _))
            {
                toolTip1.SetToolTip(textBox5, "Salary must be a numeric value");
                isValid = false;
            }
            else if (decimal.Parse(textBox5.Text) < 0)
            {
                toolTip1.SetToolTip(textBox5, "Salary must be a non-negative value");
                isValid = false;
            }

            // Display the error messages in a visible tooltip
            if (!isValid)
            {
                toolTip1.Show("Please correct the validation errors.", this, 0, this.Height, 5000);
            }

            return isValid;
        }
    }
}
