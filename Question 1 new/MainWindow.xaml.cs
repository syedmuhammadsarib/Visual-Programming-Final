using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Question_1_new
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            grade_cmd.Items.Add("A");
            grade_cmd.Items.Add("B");
            grade_cmd.Items.Add("C");
            grade_cmd.Items.Add("D");
            grade_cmd.Items.Add("E");
            grade_cmd.Items.Add("F");

            subject_cmd.Items.Add("Mathematics");
            subject_cmd.Items.Add("English");
            subject_cmd.Items.Add("Physics");

            LoadDataGrid();
            LoadData("All", "", "");
        }

        private void LoadDataGrid()
        {
            var students = new List<Student>
            {
                new Student { Name = "Sarib", Grade = "A", Subject = "Mathematics", Marks = 95, Attendance = 90 },
                new Student { Name = "Maaz", Grade = "B", Subject = "English", Marks = 85, Attendance = 88 },
                new Student { Name = "Ahmad", Grade = "C", Subject = "Physics", Marks = 75, Attendance = 80 }
            };

            dataGrid.ItemsSource = students;
        }

        public void LoadData(string pass, string grade = null, string subject = null)
        {
            SqlConnection newcon = new SqlConnection(@"Data Source=SYED-BROS-PC;Initial Catalog=StudentRecordDB;Integrated Security=True;TrustServerCertificate=True;");
            string query;

            if (pass == "All")
            {
                query = "SELECT * FROM StudentRecords";
            }
            else if (pass == "Grade" && !string.IsNullOrEmpty(grade))
            {
                query = "SELECT * FROM StudentRecords WHERE Grade = @Grade";
            }
            else if (pass == "Subject" && !string.IsNullOrEmpty(subject))
            {
                query = "SELECT * FROM StudentRecords WHERE Subject = @Subject";
            }
            else if (pass == "GradeAndSubject" && !string.IsNullOrEmpty(grade) && !string.IsNullOrEmpty(subject))
            {
                query = "SELECT * FROM StudentRecords WHERE Grade = @Grade AND Subject = @Subject";
            }
            else
            {
                return;
            }

            newcon.Open();
            SqlCommand sqlCommand = new SqlCommand(query, newcon);

            if (pass == "Grade" && !string.IsNullOrEmpty(grade))
            {
                sqlCommand.Parameters.AddWithValue("@Grade", grade);
            }
            if (pass == "Subject" && !string.IsNullOrEmpty(subject))
            {
                sqlCommand.Parameters.AddWithValue("@Subject", subject);
            }
            if (pass == "GradeAndSubject" && !string.IsNullOrEmpty(grade) && !string.IsNullOrEmpty(subject))
            {
                sqlCommand.Parameters.AddWithValue("@Grade", grade);
                sqlCommand.Parameters.AddWithValue("@Subject", subject);
            }

            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            adapter.Fill(dt);
            dataGrid.ItemsSource = dt.DefaultView;

            newcon.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(name_txt.Text) || string.IsNullOrEmpty(grade_txt.Text) || string.IsNullOrEmpty(subject_txt.Text) || string.IsNullOrEmpty(marks_txt.Text) || string.IsNullOrEmpty(attend_txt.Text))
            {
                MessageBox.Show("All fields must be filled.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string name = name_txt.Text;
            string grade = grade_txt.Text;
            string subject = subject_txt.Text;
            int marks = int.Parse(marks_txt.Text);
            int attendance = int.Parse(attend_txt.Text);

            using (SqlConnection conn = new SqlConnection(@"Data Source=SYED-BROS-PC;Initial Catalog=StudentRecordDB;Integrated Security=True;TrustServerCertificate=True;"))
            {
                conn.Open();
                string query = "INSERT INTO StudentRecords (Name, Grade, Subject, Marks, Attendance) VALUES (@Name, @Grade, @Subject, @Marks, @Attendance)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Grade", grade);
                cmd.Parameters.AddWithValue("@Subject", subject);
                cmd.Parameters.AddWithValue("@Marks", marks);
                cmd.Parameters.AddWithValue("@Attendance", attendance);
                cmd.ExecuteNonQuery();
            }

            LoadData("All");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            recordIdLabel.Visibility = Visibility.Visible;
            recordIdTextBox.Visibility = Visibility.Visible;

            if (string.IsNullOrEmpty(recordIdTextBox.Text) || string.IsNullOrEmpty(name_txt.Text) || string.IsNullOrEmpty(grade_txt.Text) || string.IsNullOrEmpty(subject_txt.Text) || string.IsNullOrEmpty(marks_txt.Text) || string.IsNullOrEmpty(attend_txt.Text))
            {
                MessageBox.Show("All fields must be filled.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string recordId = recordIdTextBox.Text;

            using (SqlConnection conn = new SqlConnection(@"Data Source=SYED-BROS-PC;Initial Catalog=StudentRecordDB;Integrated Security=True;TrustServerCertificate=True;"))
            {
                conn.Open();
                string checkQuery = "SELECT COUNT(*) FROM StudentRecords WHERE Id = @Id";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@Id", recordId);
                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    string name = name_txt.Text;
                    string grade = grade_txt.Text;
                    string subject = subject_txt.Text;
                    int marks = int.Parse(marks_txt.Text);
                    int attendance = int.Parse(attend_txt.Text);

                    string updateQuery = "UPDATE StudentRecords SET Name = @Name, Grade = @Grade, Subject = @Subject, Marks = @Marks, Attendance = @Attendance WHERE Id = @Id";
                    SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@Name", name);
                    updateCmd.Parameters.AddWithValue("@Grade", grade);
                    updateCmd.Parameters.AddWithValue("@Subject", subject);
                    updateCmd.Parameters.AddWithValue("@Marks", marks);
                    updateCmd.Parameters.AddWithValue("@Attendance", attendance);
                    updateCmd.Parameters.AddWithValue("@Id", recordId);
                    updateCmd.ExecuteNonQuery();

                    LoadData("All");
                }
                else
                {
                    MessageBox.Show("Record not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // Hide the TextBox and Label after updating
            recordIdLabel.Visibility = Visibility.Collapsed;
            recordIdTextBox.Visibility = Visibility.Collapsed;
            recordIdTextBox.Text = string.Empty;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            recordIdLabel.Visibility = Visibility.Visible;
            recordIdTextBox.Visibility = Visibility.Visible;

            if (string.IsNullOrEmpty(recordIdTextBox.Text))
            {
                MessageBox.Show("Record ID must be filled.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string recordId = recordIdTextBox.Text;

            using (SqlConnection conn = new SqlConnection(@"Data Source=SYED-BROS-PC;Initial Catalog=StudentRecordDB;Integrated Security=True;TrustServerCertificate=True;"))
            {
                conn.Open();
                string checkQuery = "SELECT COUNT(*) FROM StudentRecords WHERE Id = @Id";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@Id", recordId);
                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    string deleteQuery = "DELETE FROM StudentRecords WHERE Id = @Id";
                    SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn);
                    deleteCmd.Parameters.AddWithValue("@Id", recordId);
                    deleteCmd.ExecuteNonQuery();

                    LoadData("All");
                }
                else
                {
                    MessageBox.Show("Record not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // Hide the TextBox and Label after deleting
            recordIdLabel.Visibility = Visibility.Collapsed;
            recordIdTextBox.Visibility = Visibility.Collapsed;
            recordIdTextBox.Text = string.Empty;
        }

        private void grade_cmd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (grade_cmd.SelectedItem != null && subject_cmd.SelectedItem != null)
            {
                string selectedGrade = grade_cmd.SelectedItem.ToString();
                string selectedSubject = subject_cmd.SelectedItem.ToString();
                LoadData("GradeAndSubject", selectedGrade, selectedSubject);
            }
            else if (grade_cmd.SelectedItem != null)
            {
                string selectedGrade = grade_cmd.SelectedItem.ToString();
                LoadData("Grade", selectedGrade);
            }
        }

        private void subject_cmd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (grade_cmd.SelectedItem != null && subject_cmd.SelectedItem != null)
            {
                string selectedGrade = grade_cmd.SelectedItem.ToString();
                string selectedSubject = subject_cmd.SelectedItem.ToString();
                LoadData("GradeAndSubject", selectedGrade, selectedSubject);
            }
            else if (subject_cmd.SelectedItem != null)
            {
                string selectedSubject = subject_cmd.SelectedItem.ToString();
                LoadData("Subject", null, selectedSubject);
            }
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            grade_cmd.SelectedIndex = -1;
            subject_cmd.SelectedIndex = -1;
            LoadData("All");
        }
    }

    public class Student
    {
        public string Name { get; set; }
        public string Grade { get; set; }
        public string Subject { get; set; }
        public int Marks { get; set; }
        public int Attendance { get; set; }
    }
}
