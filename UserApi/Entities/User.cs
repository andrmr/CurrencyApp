namespace UserApi.Entities
{
    public class User
    {
        public int Id { get; set; } = 0;

        public string Name { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;
        
        public string PasswordSalt { get; set; } = string.Empty;

        public UserData? Data { get; set; }
    }

    public class UserData
    {
        public IEnumerable<string>? Favorites { get; set; }
        
        public IDictionary<string, IEnumerable<decimal>>? Alerts { get; set; }
    }
}
