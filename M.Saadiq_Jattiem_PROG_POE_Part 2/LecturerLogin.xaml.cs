using System;
using System.Data.SqlClient;
using System.Text;
using System.Windows;

namespace M.Saadiq_Jattiem_PROG_POE_Part_2
{
    /// <summary>
    /// When a lecturer logs in, they will be directed to the Lecturer Dashboard window if their login is successful and they have an account.
    /// </summary>
    public partial class LecturerLogin : Window
    {
        public LecturerLogin()
        {
            InitializeComponent();
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the user's input from the form fields
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            // Validate that both fields are filled out
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both email and password.");
                return;
            }

            try
            {
                // Hash the entered password
                string passwordHash = HashPassword(password);

                // SQL connection string to the database
                string connectionString = "Data Source=labG9AEB3\\sqlexpress01;Initial Catalog=POE;Integrated Security=True;Encrypt=False;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL query to check if the email and hashed password match an entry in the AccountUser table
                    string query = "SELECT COUNT(*) FROM AccountUser WHERE Email = @Email AND PasswordHash = @PasswordHash AND AccountType = 'Lecturer'";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Use SQL parameters to prevent SQL injection attacks
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@PasswordHash", passwordHash);  // Use hashed password

                        // Execute the query and get the number of matching entries
                        int count = (int)command.ExecuteScalar();

                        // Check if any entries were found
                        if (count > 0)
                        {
                            MessageBox.Show("Lecturer logged in successfully.");

                            // Open the Lecturer Dashboard window and close the login window
                            LecturerDashboard lecturerDashboard = new LecturerDashboard();
                            lecturerDashboard.Show();
                            this.Close();  // Close the login window
                        }
                        else
                        {
                            // No matching entries were found
                            MessageBox.Show("Invalid email or password.");
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Handle SQL-related exceptions
                MessageBox.Show("Database error: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                // Handle any other exceptions that occur during the process
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        // Hash the password using SHA-256
        private string HashPassword(string password)
        {
            using (var sha256Hash = System.Security.Cryptography.SHA256.Create())
            {
                // Compute the hash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));  // Convert each byte to hex
                }
                return builder.ToString();
            }
        }
    }
}
