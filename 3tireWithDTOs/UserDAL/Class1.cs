using System;
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