using Microsoft.Win32; // Required for OpenFileDialog
using System;
using System.IO;
using System.Data.SqlClient;
using System.Windows;

namespace M.Saadiq_Jattiem_PROG_POE_Part_2
{
    public partial class SupportingDocs : Window
    {
        private string uploadedFilePath = null; // Store the uploaded file path

        public SupportingDocs()
        {
            InitializeComponent();
        }

        // Method to get the Claims ID based on the class name
        public int GetClaimsIDByClass(string classTaught)
        {
            string connectionString = "Data Source=labG9AEB3\\sqlexpress01;Initial Catalog=POE;Integrated Security=True;";
            string query = "SELECT ClaimsID FROM Claims WHERE ClassTaught = @ClassTaught";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClassTaught", classTaught);

                    try
                    {
                        connection.Open();
                        var result = command.ExecuteScalar();
                        if (result != null)
                        {
                            return Convert.ToInt32(result); // Return the ClaimsID
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
            return -1; // Return -1 if claim not found
        }

        // Method to save the supporting document to the database and update Claims table
        private void SaveSupportingDocument(int ClaimsID, string documentPath)
        {
            string connectionString = "Data Source=labG9AEB3\\sqlexpress01;Initial Catalog=POE;Integrated Security=True;";

            // Insert into SupportingDocuments table
            string insertQuery = @"INSERT INTO SupportingDocuments (ClaimsID, DocName, FilePath, SubmissionDate)
                                   VALUES (@ClaimsID, @DocName, @FilePath, @SubmissionDate)";

            // Update the Claims table to reflect the new document path
            string updateQuery = @"UPDATE Claims
                                   SET SupportingDocumentPath = @FilePath
                                   WHERE ClaimsID = @ClaimsID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // First, insert the document into SupportingDocuments
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@ClaimsID", ClaimsID);
                        insertCommand.Parameters.AddWithValue("@DocName", Path.GetFileName(documentPath));
                        insertCommand.Parameters.AddWithValue("@FilePath", documentPath);
                        insertCommand.Parameters.AddWithValue("@SubmissionDate", DateTime.Now);
                        insertCommand.ExecuteNonQuery();
                    }

                    // Then, update the Claims table to save the document path
                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@ClaimsID", ClaimsID);
                        updateCommand.Parameters.AddWithValue("@FilePath", documentPath);
                        updateCommand.ExecuteNonQuery();
                    }

                    MessageBox.Show("Supporting document submitted successfully!");
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

        // Event handler for the Upload Document button
        private void UploadDocument_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Documents (*.pdf;*.doc;*.docx)|*.pdf;*.doc;*.docx|All files (*.*)|*.*",
                Title = "Select a Document"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                uploadedFilePath = openFileDialog.FileName; // Store the file path
                MessageBox.Show("Document uploaded: " + uploadedFilePath);
            }
        }

        // Event handler for the Submit button
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string classTaught = ClassTaughtTextBox.Text;

            if (string.IsNullOrWhiteSpace(classTaught))
            {
                MessageBox.Show("Please enter a class taught.");
                return;
            }

            if (uploadedFilePath == null)
            {
                MessageBox.Show("Please upload a document before submitting.");
                return;
            }

            int ClaimsID = GetClaimsIDByClass(classTaught);
            if (ClaimsID == -1)
            {
                MessageBox.Show("No claim found for the specified class.");
                return;
            }

            SaveSupportingDocument(ClaimsID, uploadedFilePath);
            this.Close(); // Close the window after submission
        }

        // Event handler for the Cancel button
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Close the window without doing anything
        }
    }
}
