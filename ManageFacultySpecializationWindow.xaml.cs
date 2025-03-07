using System;
using System.Windows;
using MyWpfApp.Data;
using MyWpfApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MyWpfApp
{
    public partial class ManageFacultySpecializationWindow : Window
    {
        private readonly UniversityDbContext _dbContext;

        public ManageFacultySpecializationWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<UniversityDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=UniversityDB;Trusted_Connection=True;");
            _dbContext = new UniversityDbContext(optionsBuilder.Options);

            LoadData();
            LoadFilters();
        }

        private void LoadData()
        {
            FacultyListGrid.ItemsSource = _dbContext.Faculties.ToList();
            SpecializationListGrid.ItemsSource = _dbContext.Specializations.ToList();
        }

        private void LoadFilters()
        {
            FacultyFilterComboBox.ItemsSource = _dbContext.Faculties.ToList();
            SpecializationFilterComboBox.ItemsSource = _dbContext.Specializations.ToList();
        }

        private void FilterByFaculty_Click(object sender, RoutedEventArgs e)
        {
            if (FacultyFilterComboBox.SelectedItem is Faculty selectedFaculty)
            {
                FacultyListGrid.ItemsSource = _dbContext.Faculties
                    .Where(f => f.FacultyId == selectedFaculty.FacultyId)
                    .ToList();
            }
        }

        private void FilterBySpecialization_Click(object sender, RoutedEventArgs e)
        {
            if (SpecializationFilterComboBox.SelectedItem is Specialization selectedSpecialization)
            {
                SpecializationListGrid.ItemsSource = _dbContext.Specializations
                    .Where(s => s.SpecializationId == selectedSpecialization.SpecializationId)
                    .ToList();
            }
        }

        private void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            FacultyListGrid.ItemsSource = _dbContext.Faculties.ToList();
            SpecializationListGrid.ItemsSource = _dbContext.Specializations.ToList();
        }

        private void AddFaculty_Click(object sender, RoutedEventArgs e)
        {
            var facultyName = NewFacultyNameInput.Text;
            if (string.IsNullOrWhiteSpace(facultyName))
            {
                MessageBox.Show("Faculty name cannot be empty.");
                return;
            }

            var faculty = new Faculty { Name = facultyName };
            _dbContext.Faculties.Add(faculty);
            _dbContext.SaveChanges();

            MessageBox.Show("Faculty added!");
            LoadData();
            LoadFilters();
        }

        private void AddSpecialization_Click(object sender, RoutedEventArgs e)
        {
            var specializationName = NewSpecializationNameInput.Text;
            if (!int.TryParse(NewFacultyIdInput.Text, out int facultyId))
            {
                MessageBox.Show("Faculty ID must be a number.");
                return;
            }

            var faculty = _dbContext.Faculties.Find(facultyId);
            if (faculty == null)
            {
                MessageBox.Show("No faculty exists with this ID.");
                return;
            }

            var specialization = new Specialization
            {
                Name = specializationName,
                FacultyId = facultyId
            };

            _dbContext.Specializations.Add(specialization);
            _dbContext.SaveChanges();

            MessageBox.Show("Specialization added!");
            LoadData();
            LoadFilters();
        }
    }
}
