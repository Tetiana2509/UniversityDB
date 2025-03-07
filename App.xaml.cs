using Microsoft.EntityFrameworkCore;
using MyWpfApp.Data;
using System.Configuration;
using System.Data;
using System.Windows;

namespace MyWpfApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var optionsBuilder = new DbContextOptionsBuilder<UniversityDbContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=UniversityDB;Trusted_Connection=True;");

        using (var dbContext = new UniversityDbContext(optionsBuilder.Options))
        {
            dbContext.Database.EnsureCreated();
        }
    }
}

