using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;

namespace M.Saadiq_Jattiem_PROG_POE_Part_2
{
    /// <summary>
    /// Coordinators will have submitted claims uploaded to this screen where they can either approve or reject the claims.
    /// </summary>
    public partial class ProgrammeCoordinatorDashboard : Window
    {
        // Constructor for the ProgrammeCoordinatorDashboard
        public ProgrammeCoordinatorDashboard()
        {
            InitializeComponent();
            LoadClaims();
        }

        // Method to load claims into the ListView
        private void LoadClaims()
        {
            List<Claim> claims = GetClaimsFromDatabase();

            if (claims.Count == 0)
            {
                MessageBox.Show("No claims available to display.");
            }
            else
            {
                MessageBox.Show($"{claims.Count} claims loaded successfully.");
            }

            ClaimsListView.ItemsSource = claims;
        }

        // Retrieves claims from the database
        private List<Claim> GetClaimsFromDatabase()
        {
            List<Claim> claims = new List<Claim>();
            string connectionString = "Data Source=labG9AEB3\\sqlexpress01;Initial Catalog=POE;Integrated Security=True;Encrypt=False;";
            string query = "SELECT ClaimsID, ClassTaught, ClaimTotalAmount, ClaimStatus FROM Claims";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    MessageBox.Show("Connection successful");

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int claimID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                        string classTaught = reader.IsDBNull(1) ? "Unknown" : reader.GetString(1);
                        decimal totalAmount = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2);
                        string claimStatus = reader.IsDBNull(3) ? "Pending" : reader.GetString(3);

                        claims.Add(new Claim
                        {
                            ClaimID = claimID,
                            ClassTaught = classTaught,
                            TotalAmount = totalAmount,
                            ClaimStatus = claimStatus
                        });

                        // Debugging message to verify claim retrieval
                        MessageBox.Show($"ClaimID: {claimID}, ClassTaught: {classTaught}, TotalAmount: {totalAmount}, Status: {claimStatus}");
                    }

                    MessageBox.Show($"Total claims retrieved: {claims.Count}");
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show("Database error: " + sqlEx.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            return claims;
        }

        // Updates the claims status based on the coordinator's decision
        private void UpdateClaimStatus(int claimID, string newStatus)
        {
            string connectionString = "Data Source=labG9AEB3\\sqlexpress01;Initial Catalog=POE;Integrated Security=True;Encrypt=False;";
            string query = "UPDATE Claims SET ClaimStatus = @ClaimStatus WHERE ClaimsID = @ClaimID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClaimStatus", newStatus);
                command.Parameters.AddWithValue("@ClaimID", claimID);

                try
                {
                    connection.Open();
                    // Execute the query and get the number of rows affected
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Claim status updated successfully!");
                        LoadClaims(); // Reload claims after updating
                    }
                    else
                    {
                        // If no rows were affected, something went wrong
                        MessageBox.Show("No rows were updated. Please check if the claim exists or if there are any constraints.");
                    }
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show("Database error: " + sqlEx.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }


        // Approve button changes status of claims to approved
        private void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClaimsListView.SelectedItem is Claim selectedClaim)
            {
                UpdateClaimStatus(selectedClaim.ClaimID, "Approved");
            }
            else
            {
                MessageBox.Show("Please select a claim to approve.");
            }
        }

        // Reject button will change status to rejected
        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClaimsListView.SelectedItem is Claim selectedClaim)
            {
                UpdateClaimStatus(selectedClaim.ClaimID, "Rejected");
            }
            else
            {
                MessageBox.Show("Please select a claim to reject.");
            }
        }

        // Pending button will change status to pending
        private void PendingButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClaimsListView.SelectedItem is Claim selectedClaim)
            {
                UpdateClaimStatus(selectedClaim.ClaimID, "Pending");
            }
            else
            {
                MessageBox.Show("Please select a claim to set as pending.");
            }
        }
    }

    // Claim class definition
    public class Claim
    {
        public int ClaimID { get; set; }
        public string ClassTaught { get; set; }
        public decimal TotalAmount { get; set; }
        public string ClaimStatus { get; set; }
    }
}
