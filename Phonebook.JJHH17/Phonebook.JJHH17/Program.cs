using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Spectre.Console.Cli;
using Spectre.Console;

// Entity Framework installed, both framework and SQL Server EF Packages
// SQL Server used
// app.config file used for SQL credentials
// Spectre console is used for the menu and console
// User will need to create the migrations folder and run the database update command 

namespace Phonebook.JJHH17
{
    public class Program
    {

        enum MenuOptions
        {
            AddEntry,
            Read,
            Delete,
            Update,
            Exit,
        }

        public static async Task Main(string[] args)
        {
            bool running = true;
            while (running)
            {
                AnsiConsole.MarkupLine("[bold blue]Welcome to the Phonebook Application![/]");
                Console.Clear();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<MenuOptions>()
                        .Title("Select an option:")
                        .AddChoices(Enum.GetValues<MenuOptions>()));

                switch (choice)
                {
                    case MenuOptions.AddEntry:
                        Console.Clear();
                        AddEntry();
                        break;

                    case MenuOptions.Read:
                        Console.Clear();
                        ReadEntries();
                        AnsiConsole.MarkupLine("[bold green]Press any key to continue...[/]");
                        Console.ReadKey();
                        break;

                    case MenuOptions.Delete:
                        Console.Clear();
                        DeleteEntry();
                        break;

                    case MenuOptions.Update:
                        Console.Clear();
                        UpdateEntry();
                        break;

                    case MenuOptions.Exit:
                        Console.Clear();
                        running = false;
                        AnsiConsole.MarkupLine("[bold green]Thank you for using the Phonebook Application![/]");
                        Console.ReadKey();
                        break;
                }
            }
        }

        public static void AddEntry()
        {
            Console.WriteLine("Enter Name:");
            string name = Console.ReadLine();
            string email = EnterEmail();
            string phoneNumber = EnterPhoneNumber();

            using (var context = new PhoneBookContext())
            {
                var newEntry = new PhoneBook
                {
                    Name = name,
                    Email = email,
                    PhoneNumber = phoneNumber,
                };
                context.PhoneBooks.Add(newEntry);
                context.SaveChanges();
            }

            Console.WriteLine("Entry added successfully. Enter any key to continue...");
            Console.ReadKey();
        }

        public static void ReadEntries()
        {
            using (var context = new PhoneBookContext())
            {
                var query = context.PhoneBooks.ToList();

                if (query.Count == 0)
                {
                    Console.WriteLine("No entries found.");
                }
                else
                {
                    Console.WriteLine("Phonebook Entries:");
                    var table = new Table();
                    table.AddColumn("ID");
                    table.AddColumn("Name");
                    table.AddColumn("Email");
                    table.AddColumn("Phone Number");

                    foreach (var entry in query)
                    {
                        table.AddRow(entry.Id.ToString(), entry.Name, entry.Email, entry.PhoneNumber);
                    }

                    AnsiConsole.Write(table);
                }
            }
        }

        public static void DeleteEntry()
        {
            using (var context = new PhoneBookContext())
            {
                AnsiConsole.MarkupLine("[blue]Delete an entry[/]");
                ReadEntries();
                Console.WriteLine("Enter the ID of the entry to delete:");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    var entry = context.PhoneBooks.Find(id);
                    if (entry != null)
                    {
                        context.PhoneBooks.Remove(entry);
                        context.SaveChanges();
                        Console.WriteLine("Entry deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Entry not found.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid ID format.");
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        public static void UpdateEntry()
        {
            using (var context = new PhoneBookContext())
            {
                AnsiConsole.MarkupLine("[blue]Update an entry[/]");
                ReadEntries();

                Console.WriteLine("Enter the ID of the entry to update:");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    var entry = context.PhoneBooks.Find(id);
                    if (entry != null)
                    {
                        Console.WriteLine("Enter new Name (leave blank to keep current):");
                        string name = Console.ReadLine();
                        string email = EnterEmail();
                        string phoneNumber = EnterPhoneNumber(); 
                        if (!string.IsNullOrWhiteSpace(name)) entry.Name = name;
                        if (!string.IsNullOrWhiteSpace(email)) entry.Email = email;
                        if (!string.IsNullOrWhiteSpace(phoneNumber)) entry.PhoneNumber = phoneNumber;
                        context.SaveChanges();
                        Console.WriteLine("Entry updated successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Entry not found.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid ID format.");
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private static bool IsDigit(string input)
        {
            foreach (char c in input)
            {
                if (c < '0' || c > '9')
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsPhoneNumber(string number)
        {
            // Please note that this specifically checks for UK phone numbers (they start with 07)
            return number[0] == '0' && number[1] == '7' && number.Length == 11 && IsDigit(number);
        }

        private static string EnterPhoneNumber()
        {
            Console.WriteLine("Please enter a UK phone number");
            Console.WriteLine("UK Phone numbers have an 07 prefix and are 11 digits long");
            string phoneNumber = Console.ReadLine();
            while (!IsPhoneNumber(phoneNumber))
            {
                Console.WriteLine("Invalid phone number format. Please enter a valid UK phone number:");
                phoneNumber = Console.ReadLine();
            }

            return phoneNumber;
        }

        private static bool IsEmail(string email)
        {
            // Some email domains don't contain a dot (.), so I've left this out for now
            return email.Length >= 4 && email.Contains("@");
        }

        private static string EnterEmail()
        {
            Console.WriteLine("Please enter an email address");
            Console.WriteLine("A valid email address must contain atleast 4 characters and an @ symbol");
            string email = Console.ReadLine();
            while (!IsEmail(email))
            {
                Console.WriteLine("Invalid email format. Please enter a valid email address:");
                email = Console.ReadLine();
            }

            return email;
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