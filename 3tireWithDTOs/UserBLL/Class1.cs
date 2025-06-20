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