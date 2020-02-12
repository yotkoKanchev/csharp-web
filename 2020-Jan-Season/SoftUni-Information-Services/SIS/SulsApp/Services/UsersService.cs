namespace SulsApp.Services
{
    using SIS.MvcFramework;
    using SulsApp.Models;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public class UsersService : IUsersService
    {
        private readonly ApplicationDbContext db;

        public UsersService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public void ChangePassword(string username, string newPassword)
        {
            var user = this.db.Users.FirstOrDefault(x => x.Username == username);

            if (user == null)
            {
                return;
            }

            user.Password = this.Hash(newPassword);
            this.db.SaveChanges();
        }

        public int CountUsers()
        {
            return this.db.Users.Count();
        }

        public void CreateUser(string username, string email, string password)
        {
            var user = new User
            {
                Email = email,
                Username = username,
                Password = this.Hash(password),
                Role = IdentityRole.User,
            };

            this.db.Users.Add(user);
            this.db.SaveChanges();
        }

        public string GetUserId(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrEmpty(password))
            {
                return "invalidPassword";
            }

            var passwordHash = this.Hash(password);

            var user = this.db.Users
                .Where(x => x.Username == username)
                .FirstOrDefault();

            if (user == null)
            {
                return null;
            }

            if (user.Password != passwordHash)
            {
                return "invalidPassword";
            }

            return user.Id;
        }

        public bool IsEmailUsed(string email)
        {
            return this.db.Users.Any(x => x.Email == email);
        }

        public bool IsUsernameUsed(string username)
        {
            return this.db.Users.Any(x => x.Username == username);
        }

        private string Hash(string input)
        {
            var crypt = new SHA256Managed();
            var hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(input));

            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }

            return hash.ToString();
        }
    }
}
