using System;
using System.Data.SqlClient;
using System.Windows;

namespace M.Saadiq_Jattiem_PROG_POE_Part_2
{
    // Logic behind allowing users to update their claims information
    public partial class EditClaim : Window
    {
        // Connect to the database
        public string connectionString = "Data Source=labG9AEB3\\sqlexpress01;Initial Catalog=POE;Integrated Security=True;";
        public int claimsID;  // Changed from claimID to claimsID for consistency
        public decimal sessionCost = 105; // Example cost per session

        public EditClaim(int claimID)
        {
            InitializeComponent();
            this.claimsID = claimID;  // Correctly assign the parameter to claimsID
            LoadClaimDetails();
        }

        // Load claim details into text boxes
        public void LoadClaimDetails()
        {
            string query = "SELECT ClassTaught, NoOfSessions, ClaimTotalAmount FROM Claims WHERE ClaimsID = @ClaimsID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClaimsID", claimsID);  // Correct parameter name here

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        ClassTaughtTextBox.Text = reader["ClassTaught"].ToString();
                        NumberOfSessionsTextBox.Text = reader["NoOfSessions"].ToString();  // Ensure matching field names
                        TotalAmountTextBox.Text = reader["ClaimTotalAmount"].ToString();  // Ensure matching field names
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        // Update total amount based on number of sessions
        public void NumberOfSessionsTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (int.TryParse(NumberOfSessionsTextBox.Text, out int numberOfSessions))
            {
                decimal totalAmount = numberOfSessions * sessionCost;
                TotalAmountTextBox.Text = totalAmount.ToString("F2"); // Format to 2 decimal places
            }
            else
            {
                TotalAmountTextBox.Text = "0.00"; // Reset if input is invalid
            }
        }

        public void UpdateClaim(string classTaught, int numberOfSessions)
        {
            string query = "UPDATE Claims SET ClassTaught = @ClassTaught, NoOfSessions = @NoOfSessions WHERE ClaimsID = @ClaimsID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClassTaught", classTaught);
                command.Parameters.AddWithValue("@NoOfSessions", numberOfSessions);  // Ensure this matches your database
                command.Parameters.AddWithValue("@ClaimsID", claimsID);  // Ensure this matches your database

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        // Save updated claim details
        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string classTaught = ClassTaughtTextBox.Text;
            int numberOfSessions;

            if (int.TryParse(NumberOfSessionsTextBox.Text, out numberOfSessions))
            {
                // Only update the ClassTaught and NumberOfSessions
                UpdateClaim(classTaught, numberOfSessions);
                MessageBox.Show("Claim updated successfully!");
                Close();
            }
            else
            {
                MessageBox.Show("Invalid input for number of sessions.");
            }
        }

    }
}