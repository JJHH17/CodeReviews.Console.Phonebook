using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Configuration;

// Entity Framework installed, both framework and SQL Server EF Packages
// SQL Server used
// app.config file used for SQL credentials

namespace Phonebook.JJHH17
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Howdy, all!");
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
            optionsBuilder.UseSqlServer("Connection string to be added");
        }
    }
}