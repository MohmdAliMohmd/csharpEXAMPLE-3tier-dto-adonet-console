Here's a complete example of a 3-tier architecture with DTOs in C# using ADO.NET and Console Application (.NET Framework):

### 1. Database Setup (SQL Server)
```sql
CREATE DATABASE UserDB;
GO

USE UserDB;
GO

CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    RegistrationDate DATETIME DEFAULT GETDATE()
);
```
###  Sample Data
```sql
INSERT INTO Users (FirstName, LastName, Email, RegistrationDate) VALUES
('John', 'Doe', 'john.doe@example.com', '2023-01-15'),
('Jane', 'Smith', 'jane.smith@example.com', '2023-02-20'),
('Michael', 'Johnson', 'michael.j@example.com', DEFAULT),  -- Uses default
('Emily', 'Williams', 'emily.w@example.com', DEFAULT),      -- Uses default
('David', 'Brown', 'david.brown@example.com', '2023-05-10');
```

### 2. Solution Structure
```
- UserDTO (Data Transfer Object)
- UserDAL (Data Access Layer)
- UserBLL (Business Logic Layer)
- ConsoleApp (Presentation Layer)
```

### 3. Implementation

**1. UserDTO.cs (DTO Layer)**
```csharp
namespace UserDTO
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
```

**2. UserDAL.cs (Data Access Layer)**
```csharp
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UserDTO;

namespace UserDAL
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();

            using (var connection = new SqlConnection(_connectionString))
            {
                const string query = "SELECT Id, FirstName, LastName, Email, RegistrationDate FROM Users";
                var command = new SqlCommand(query, connection);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = (int)reader["Id"],
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Email = reader["Email"].ToString(),
                            RegistrationDate = (DateTime)reader["RegistrationDate"]
                        });
                    }
                }
            }
            return users;
        }

        public void AddUser(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                const string query = @"INSERT INTO Users (FirstName, LastName, Email) 
                                     VALUES (@FirstName, @LastName, @Email)";
                
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FirstName", user.FirstName);
                command.Parameters.AddWithValue("@LastName", user.LastName);
                command.Parameters.AddWithValue("@Email", user.Email);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
```

**3. UserBLL.cs (Business Logic Layer)**
```csharp
using System.Collections.Generic;
using UserDTO;
using UserDAL;

namespace UserBLL
{
    public class UserService
    {
        private readonly UserRepository _repository;

        public UserService(string connectionString)
        {
            _repository = new UserRepository(connectionString);
        }

        public List<User> GetRegisteredUsers()
        {
            return _repository.GetAllUsers();
        }

        public void RegisterNewUser(string firstName, string lastName, string email)
        {
            // Business validation
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                throw new System.Exception("Name fields are required");
            
            if (!IsValidEmail(email))
                throw new System.Exception("Invalid email format");

            // Create DTO
            var newUser = new User
            {
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Email = email.Trim()
            };

            _repository.AddUser(newUser);
        }

        private bool IsValidEmail(string email)
        {
            return email.Contains("@") && email.Contains(".");
        }
    }
}
```

**4. Program.cs (Presentation Layer)**
```csharp
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
```

**5. App.config**
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.8"/>
    </startup>
    
    <connectionStrings>
        <add name="UserDB" 
             connectionString="Server=.;Database=UserDB;Integrated Security=True;"
             providerName="System.Data.SqlClient"/>
    </connectionStrings>
</configuration>
```

### 4. How DTOs Work in This Architecture:
1. **Presentation Layer**:
   - Receives user input
   - Calls BLL methods with primitive types/DTOs
   - Displays DTOs received from BLL

2. **Business Logic Layer**:
   - Receives requests from presentation layer
   - Performs validation and business rules
   - Uses DTOs to communicate with DAL
   - Returns processed DTOs to presentation layer

3. **Data Access Layer**:
   - Receives DTOs from BLL
   - Maps DTOs to database operations
   - Returns database results as DTOs to BLL

4. **DTO Layer**:
   - Simple data containers without behavior
   - Passed between all layers
   - Isolated from business logic and data access

### 5. Key Benefits:
1. **Separation of Concerns**: Each layer has distinct responsibilities
2. **Decoupling**: Changes in one layer don't affect others
3. **Maintainability**: Easier to modify and test components
4. **Security**: Prevents direct database exposure to presentation layer
5. **Reusability**: Business logic can be reused with different UIs

To run this example:
1. Create the database using the provided SQL script
2. Create a C# Console Application (.NET Framework)
3. Add the connection string in App.config
4. Implement the 4 code files in separate class files
5. Add references between projects/layers as needed

This implementation shows a complete flow from user input through business validation to database storage, using DTOs to transfer data between layers while maintaining separation of concerns.
