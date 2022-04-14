namespace UserApi.Helpers
{
    public interface IPasswordHasher
    {
        (string hash, string salt) CreatePassword(string password);
        bool VerifyPassword(string password, string hash, string salt);
    }

    public class BCryptPasswordHasher : IPasswordHasher
    {
        public (string hash, string salt) CreatePassword(string password)
        {
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var hash = BCrypt.Net.BCrypt.HashPassword(password, salt);

            return (hash, salt);
        }

        public bool VerifyPassword(string password, string hash, string salt) =>
            BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
