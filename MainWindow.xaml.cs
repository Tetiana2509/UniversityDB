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

            // Настройка DbContext
            var optionsBuilder = new DbContextOptionsBuilder<UniversityDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=UniversityDB;Trusted_Connection=True;");
            _dbContext = new UniversityDbContext(optionsBuilder.Options);

            SeedDatabase();

            LoadStudents();
        }

        private void LoadStudents()
        {
            var students = _dbContext.Students
                .Include(s => s.Specialization) // Подгрузка специальности
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
                    MessageBox.Show("Имя студента не может быть пустым.");
                    return;
                }

                if (!int.TryParse(SpecializationIdInput.Text, out int specializationId))
                {
                    MessageBox.Show("ID специальности должно быть числом.");
                    return;
                }

                var specialization = _dbContext.Specializations.FirstOrDefault(s => s.SpecializationId == specializationId);
                if (specialization == null)
                {
                    MessageBox.Show("Специальности с указанным ID не существует.");
                    return;
                }

                var newStudent = new Student
                {
                    Name = studentName,
                    SpecializationId = specializationId
                };

                _dbContext.Students.Add(newStudent);
                _dbContext.SaveChanges();

                MessageBox.Show("Студент успешно добавлен!");
                LoadStudents(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
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

                    MessageBox.Show("Студент обновлен!");
                    LoadStudents();
                }
                else
                {
                    MessageBox.Show("ID специальности должен быть числом.");
                }
            }
            else
            {
                MessageBox.Show("Выберите студента для обновления.");
            }
        }

        private void DeleteStudent_Click(object sender, RoutedEventArgs e)
        {
            if (StudentsGrid.SelectedItem is Student selectedStudent)
            {
                _dbContext.Students.Remove(selectedStudent);
                _dbContext.SaveChanges();

                MessageBox.Show("Студент удален!");
                LoadStudents();
            }
            else
            {
                MessageBox.Show("Выберите студента для удаления.");
            }
        }

        private void ClearTextBox(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && (textBox.Text == "Імя студента" || textBox.Text == "ID спеціальності"))
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
                    textBox.Text = "Імя студента";
                }
                else if (textBox.Name == "SpecializationIdInput")
                {
                    textBox.Text = "ID спеціальності";
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

                // Проверяем, есть ли у студента связанная специальность
                if (selectedStudent.Specialization != null)
                {
                    DetailSpecializationName.Text = selectedStudent.Specialization.Name;
                }
                else
                {
                    DetailSpecializationName.Text = "Нет данных";
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
                MessageBox.Show("Студенты не найдены.");
            }
        }

        private void SortStudents_Click(object sender, RoutedEventArgs e)
        {
            var students = _dbContext.Students
                .Include(s => s.Specialization)
                .OrderBy(s => s.Name)
                .ToList();
            StudentsGrid.ItemsSource = students;

            MessageBox.Show("Список студентов отсортирован по алфавиту.");
        }




    }
}
