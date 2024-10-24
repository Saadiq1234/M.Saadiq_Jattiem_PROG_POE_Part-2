using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace M.Saadiq_Jattiem_PROG_POE_Part_2
{
    /// <summary>
    /// This window allows for users to create their account and have it stored in the database.
    /// </summary>
    public partial class CreateAccountWindow : Window
    {
        public CreateAccountWindow()
        {
            InitializeComponent();
        }

        // This method is called when the Create Account button is clicked and will save the user's information (name, surname, email, etc.) into the database.
        public void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the values from the form
            string role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string firstName = FirstNameTextBox.Text.Trim();
            string lastName = LastNameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();  // This should be hashed in a real system!
            string phoneNumber = PhoneNumberTextBox.Text.Trim();

            // Validate form input
            if (string.IsNullOrWhiteSpace(role) || string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(phoneNumber))
            {
                MessageBox.Show("Please fill out all fields.");
                return;
            }

            try
            {
                // Hash the password before saving it
                string passwordHash = HashPassword(password);

                // Address of SQL server and database
                string connectionString = "Data Source=labG9AEB3\\sqlexpress01;Initial Catalog=POE;Integrated Security=True;Encrypt=False;";


                // Establish connection
                using (var con = new SqlConnection(connectionString))
                {
                    // Open Connection
                    con.Open();

                    // SQL Query with all required fields
                    string query = "INSERT INTO AccountUser (FirstName, LastName, Email, PhoneNumber, PasswordHash, AccountType) " +
                                   "VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @PasswordHash, @AccountType)";

                    // Execute query with parameters
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                        cmd.Parameters.AddWithValue("@AccountType", role);

                        cmd.ExecuteNonQuery();
                    }

                    // Close Connection
                    con.Close();
                }

                MessageBox.Show("Account successfully created.");
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Database error: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        // Hash the password using SHA256
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
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
