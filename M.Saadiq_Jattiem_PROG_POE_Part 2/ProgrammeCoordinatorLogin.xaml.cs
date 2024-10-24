using System;
using System.Security.Cryptography; // Required for SHA256
using System.Text;
using System.Windows;
using System.Data.SqlClient;

namespace M.Saadiq_Jattiem_PROG_POE_Part_2
{
    /// <summary>
    /// When coordinators are approved for login they will be taken to the coordinator dashboard
    /// </summary>
    public partial class ProgrammeCoordinatorLogin : Window
    {
        public ProgrammeCoordinatorLogin()
        {
            InitializeComponent();
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            // Get email and password from the form
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            // Validate input
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both email and password.");
                return;
            }

            try
            {
                // Hash the entered password
                string passwordHash = HashPassword(password);

                // Connection string to the database
                string connectionString = "Data Source=labG9AEB3\\sqlexpress01;Initial Catalog=POE;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Open connection
                    connection.Open();

                    // SQL query to check if the user exists and is a Programme Coordinator
                    string query = "SELECT COUNT(*) FROM AccountUser WHERE Email = @Email AND PasswordHash = @PasswordHash AND AccountType = 'Programme Coordinator/Academic Manager'";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters for the email and password
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@PasswordHash", passwordHash);  // Use hashed password

                        // Execute the query
                        int count = (int)command.ExecuteScalar();

                        // If the credentials match a Programme Coordinator, redirect to the dashboard
                        if (count > 0)
                        {
                            MessageBox.Show("Programme Coordinator logged in successfully.");

                            // Redirect to ProgrammeCoordinatorDashboard window and close the login window
                            ProgrammeCoordinatorDashboard coordinatorDashboard = new ProgrammeCoordinatorDashboard();
                            coordinatorDashboard.Show();
                            this.Close();  // Close the login window
                        }
                        else
                        {
                            MessageBox.Show("Invalid email or password.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // Convert to hex string
                }
                return builder.ToString();
            }
        }
    }
}
