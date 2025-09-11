using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

// Entity Framework installed, both framework and SQL Server EF Packages
// SQL Server used
// app.config file used for SQL credentials
// User will need to create the migrations folder and run the database update command 

namespace Phonebook.JJHH17
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using (var context = new PhoneBookContext())
            {
                context.Database.EnsureCreated();

                var newEntry = new PhoneBook
                {
                    Name = "Jane Doe",
                    Email = "Jane.Doe@email.org",
                    PhoneNumber = "123-456-7890",
                };
                context.PhoneBooks.Add(newEntry);
                context.SaveChanges();

            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

        }
    }

    public class PhoneBook
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class PhoneBookContext : DbContext
    {
        private static readonly string server = ConfigurationManager.AppSettings["Server"];
        private static readonly string databaseInstance = ConfigurationManager.AppSettings["DatabaseName"];
        public static string connectionString = $@"Server=({server})\{databaseInstance};Integrated Security=true;";

        public DbSet<PhoneBook> PhoneBooks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}