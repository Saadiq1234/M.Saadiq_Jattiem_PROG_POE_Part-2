using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
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
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        // Check for null values before accessing
                        if (reader.IsDBNull(0) || reader.IsDBNull(1) || reader.IsDBNull(2) || reader.IsDBNull(3))
                        {
                            continue; // Skip the entry if any required field is null
                        }

                        claims.Add(new Claim
                        {
                            ClaimID = reader.GetInt32(0),
                            ClassTaught = reader.GetString(1),
                            TotalAmount = reader.GetDecimal(2),
                            ClaimStatus = reader.GetString(3)
                        });
                    }

                    // Debugging information
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
            string query = "UPDATE Claims SET ClaimStatus = @ClaimStatus WHERE ClaimsID = @ClaimID"; // Fixed the parameter name

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClaimStatus", newStatus);
                command.Parameters.AddWithValue("@ClaimID", claimID);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Claim status updated successfully!");
                    LoadClaims(); // Reload claims after updating
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

        // Rejection button will change status to rejected
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
}
while (reader.Read())
{
    int claimID = reader.GetInt32(0);
    string classTaught = reader.GetString(1);
    decimal totalAmount = reader.GetDecimal(2);
    string claimStatus = reader.GetString(3);

    MessageBox.Show($"ClaimID: {claimID}, ClassTaught: {classTaught}, TotalAmount: {totalAmount}, Status: {claimStatus}");

    claims.Add(new Claim
    {
        ClaimID = claimID,
        ClassTaught = classTaught,
        TotalAmount = totalAmount,
        ClaimStatus = claimStatus
    });
}
try
{
    connection.Open();
    MessageBox.Show("Connection successful");
    SqlDataReader reader = command.ExecuteReader();
    // existing logic
}
catch (Exception ex)
{
    MessageBox.Show("Connection failed: " + ex.Message);
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

    public class Claim
    {
        public int ClaimID { get; set; }
        public string ClassTaught { get; set; }
        public decimal TotalAmount { get; set; }
        public string ClaimStatus { get; set; }
    }
}
