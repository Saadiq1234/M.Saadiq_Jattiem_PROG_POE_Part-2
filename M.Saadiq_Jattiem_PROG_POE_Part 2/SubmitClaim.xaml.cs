using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Data.SqlClient;

namespace M.Saadiq_Jattiem_PROG_POE_Part_2
{
    /// <summary>
    /// Lecturers claims will be uploaded based on information they input
    /// </summary>
    public partial class SubmitClaim : Window
    {
        public SubmitClaim()
        {
            InitializeComponent();
        }
        // For File Operations

        private string uploadedFilePath = null; // Store the uploaded file path
        private double totalAmount = 0; // Store calculated total amount

        // Event for uploading documents
        private void UploadDocument_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Supported Documents|*.docx;*.xlsx;*.pdf"; // Only allow docx, xlsx, pdf
            if (openFileDialog.ShowDialog() == true)
            {
                uploadedFilePath = openFileDialog.FileName; // Store the path
                MessageBox.Show("Document uploaded successfully.");
            }
        }

        // Event for calculating the total amount
        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            int numberOfSessions;
            if (!int.TryParse(SessionsTextBox.Text, out numberOfSessions))
            {
                MessageBox.Show("Please enter a valid number of sessions.");
                return;
            }

            double hourlyRate = 105; // Fixed hourly rate
            totalAmount = numberOfSessions * hourlyRate;
            TotalAmountTextBlock.Text = totalAmount.ToString("C"); // Display total as currency
        }

        // Event for submitting the claim
        // Event for submitting the claim
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string classTaught = ClassTaughtTextBox.Text;
            int numberOfSessions;
            if (!int.TryParse(SessionsTextBox.Text, out numberOfSessions))
            {
                MessageBox.Show("Please enter a valid number of sessions.");
                return;
            }

            // Check if the total amount is calculated
            if (totalAmount == 0)
            {
                MessageBox.Show("Please calculate the total claim amount before submitting.");
                return;
            }

            // Ensure a document is uploaded
            if (string.IsNullOrEmpty(uploadedFilePath))
            {
                MessageBox.Show("Please upload a supporting document.");
                return;
            }

            // Save the claim to the database
            double hourlyRate = 105; // Fixed hourly rate
            SaveClaimToDatabase(classTaught, numberOfSessions, totalAmount, uploadedFilePath, hourlyRate);

            // Clear the form after successful submission
            ClearForm();
        }

        // Method to save claim details to the database
        private void SaveClaimToDatabase(string classTaught, int sessions, double totalAmount, string documentPath, double hourlyRate)
        {
            string connectionString = "Data Source=labG9AEB3\\sqlexpress01;Initial Catalog=POE;Integrated Security=True;TrustServerCertificate=True";

            string query = @"INSERT INTO Claims (ClassTaught, NoOfSessions, SupportingDocumentPath, HourlyRatePerSession)
                     VALUES (@ClassTaught, @NumberOfSessions, @DocumentPath, @HourlyRate)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClassTaught", classTaught);
                command.Parameters.AddWithValue("@NumberOfSessions", sessions);
                command.Parameters.AddWithValue("@DocumentPath", documentPath); // Store file path
                command.Parameters.AddWithValue("@HourlyRate", hourlyRate); // Include the hourly rate

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Claim submitted successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }



        // Event for clearing the form (Cancel)
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        // Helper method to clear the form
        private void ClearForm()
        {
            ClassTaughtTextBox.Clear();
            SessionsTextBox.Clear();
            TotalAmountTextBlock.Text = string.Empty;
            uploadedFilePath = null;
            totalAmount = 0;
            MessageBox.Show("Form cleared.");
        }

    }
}

