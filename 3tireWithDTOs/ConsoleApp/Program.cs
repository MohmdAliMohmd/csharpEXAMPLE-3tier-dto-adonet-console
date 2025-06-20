using System;
using System.Configuration;
using UserBLL;
using UserDTO;

namespace ConsoleApp
{
    class Program
    {
        static void Main()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["UserDB"].ConnectionString;
            var userService = new UserService(connectionString);

            while (true)
            {
                Console.WriteLine("\nUser Management System");
                Console.WriteLine("1. Register User");
                Console.WriteLine("2. List Users");
                Console.WriteLine("3. Exit");
                Console.Write("Choose option: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        RegisterUser(userService);
                        break;
                    case "2":
                        ListUsers(userService);
                        break;
                    case "3":
                        return;
                }
            }
        }

        static void RegisterUser(UserService service)
        {
            try
            {
                Console.Write("First Name: ");
                var firstName = Console.ReadLine();

                Console.Write("Last Name: ");
                var lastName = Console.ReadLine();

                Console.Write("Email: ");
                var email = Console.ReadLine();

                service.RegisterNewUser(firstName, lastName, email);
                Console.WriteLine("User registered successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void ListUsers(UserService service)
        {
            var users = service.GetRegisteredUsers();

            Console.WriteLine("\nRegistered Users:");
            Console.WriteLine("ID\tName\t\tEmail\t\tRegistration Date");
            foreach (var user in users)
            {
                Console.WriteLine($"{user.Id}\t{user.FirstName} {user.LastName}\t{user.Email}\t{user.RegistrationDate:yyyy-MM-dd}");
            }
        }
    }
}