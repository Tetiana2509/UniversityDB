using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using MyWpfApp.Data;
using MyWpfApp.Models;
using System.Linq;

namespace MyWpfApp
{
    public partial class MainWindow : Window
    {
        private UniversityDbContext _dbContext;

        public MainWindow()
        {
            InitializeComponent();

            // Configure DbContext
            var optionsBuilder = new DbContextOptionsBuilder<UniversityDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=UniversityDB;Trusted_Connection=True;");
            _dbContext = new UniversityDbContext(optionsBuilder.Options);

            SeedDatabase();

            LoadStudents();
        }

        private void LoadStudents()
        {
            var students = _dbContext.Students
                .Include(s => s.Specialization) // Load specialization
                .ToList();
            StudentsGrid.ItemsSource = students;
        }

        private void AddStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var studentName = StudentNameInput.Text;
                if (string.IsNullOrWhiteSpace(studentName))
                {
                    MessageBox.Show("Student name cannot be empty.");
                    return;
                }

                if (!int.TryParse(SpecializationIdInput.Text, out int specializationId))
                {
                    MessageBox.Show("Specialization ID must be a number.");
                    return;
                }

                var specialization = _dbContext.Specializations.FirstOrDefault(s => s.SpecializationId == specializationId);
                if (specialization == null)
                {
                    MessageBox.Show("No specialization exists with the given ID.");
                    return;
                }

                var newStudent = new Student
                {
                    Name = studentName,
                    SpecializationId = specializationId
                };

                _dbContext.Students.Add(newStudent);
                _dbContext.SaveChanges();

                MessageBox.Show("Student successfully added!");
                LoadStudents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }


        private void UpdateStudent_Click(object sender, RoutedEventArgs e)
        {
            if (StudentsGrid.SelectedItem is Student selectedStudent)
            {
                selectedStudent.Name = StudentNameInput.Text;
                if (int.TryParse(SpecializationIdInput.Text, out int specializationId))
                {
                    selectedStudent.SpecializationId = specializationId;
                    _dbContext.Students.Update(selectedStudent);
                    _dbContext.SaveChanges();

                    MessageBox.Show("Student updated!");
                    LoadStudents();
                }
                else
                {
                    MessageBox.Show("Specialization ID must be a number.");
                }
            }
            else
            {
                MessageBox.Show("Select a student to update.");
            }
        }

        private void DeleteStudent_Click(object sender, RoutedEventArgs e)
        {
            if (StudentsGrid.SelectedItem is Student selectedStudent)
            {
                _dbContext.Students.Remove(selectedStudent);
                _dbContext.SaveChanges();

                MessageBox.Show("Student deleted!");
                LoadStudents();
            }
            else
            {
                MessageBox.Show("Select a student to delete.");
            }
        }

        private void ClearTextBox(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && (textBox.Text == "Student Name" || textBox.Text == "Specialization ID"))
            {
                textBox.Text = "";
            }
        }

        private void ResetTextBox(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox.Name == "StudentNameInput")
                {
                    textBox.Text = "Student Name";
                }
                else if (textBox.Name == "SpecializationIdInput")
                {
                    textBox.Text = "Specialization ID";
                }
            }
        }

        private void SeedDatabase()
        {

            if (!_dbContext.Faculties.Any())
            {
                var faculty = new Faculty { Name = "Engineering" };
                _dbContext.Faculties.Add(faculty);
                _dbContext.SaveChanges();


                var specializations = new List<Specialization>
        {
            new Specialization { Name = "Computer Science", FacultyId = faculty.FacultyId },
            new Specialization { Name = "Electrical Engineering", FacultyId = faculty.FacultyId }
        };
                _dbContext.Specializations.AddRange(specializations);
                _dbContext.SaveChanges();
            }


            if (!_dbContext.Students.Any())
            {
                var student = new Student
                {
                    Name = "John Doe",
                    SpecializationId = _dbContext.Specializations.First().SpecializationId
                };
                _dbContext.Students.Add(student);
                _dbContext.SaveChanges();
            }
        }

        private void StudentsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StudentsGrid.SelectedItem is Student selectedStudent)
            {

                DetailStudentName.Text = selectedStudent.Name;
                DetailSpecializationId.Text = selectedStudent.SpecializationId.ToString();

                // Check if the student has an associated specialization
                if (selectedStudent.Specialization != null)
                {
                    DetailSpecializationName.Text = selectedStudent.Specialization.Name;
                }
                else
                {
                    DetailSpecializationName.Text = "No data available";
                }
            }
            else
            {
                DetailStudentName.Text = "";
                DetailSpecializationId.Text = "";
                DetailSpecializationName.Text = "";
            }
        }

        private void OpenManageFacultySpecializationWindow(object sender, RoutedEventArgs e)
        {
            var manageWindow = new ManageFacultySpecializationWindow();
            manageWindow.Show();
        }


        private void SearchStudent_Click(object sender, RoutedEventArgs e)
        {
            var searchText = SearchStudentInput.Text.ToLower();
            var students = _dbContext.Students
                .Include(s => s.Specialization)
                .Where(s => s.Name.ToLower().Contains(searchText))
                .ToList();
            StudentsGrid.ItemsSource = students;

            if (!students.Any())
            {
                MessageBox.Show("No students found.");
            }
        }

        private void SortStudents_Click(object sender, RoutedEventArgs e)
        {
            var students = _dbContext.Students
                .Include(s => s.Specialization)
                .OrderBy(s => s.Name)
                .ToList();
            StudentsGrid.ItemsSource = students;

            MessageBox.Show("Student list sorted alphabetically.");
        }
    }
}
