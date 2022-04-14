using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace UserApi.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher _passwordHasher;
        private readonly DataContext _dataContext;

        public UserService(IConfiguration configuration, IPasswordHasher passwordHasher, DataContext dataContext)
        {
            _configuration = configuration;
            _passwordHasher = passwordHasher;
            _dataContext = dataContext;
        }

        public async Task<AuthenticationResponse> Authenticate(AuthenticationRequest request)
        {
            var user = await _dataContext.Users.SingleOrDefaultAsync(u => u.Name == request.Username);

            if (user is not null)
            {
                var passwordVerified = await Task.Run(() => _passwordHasher.VerifyPassword(password: request.Password, hash: user.PasswordHash, salt: user.PasswordSalt));
                if (passwordVerified)
                {
                    return new AuthenticationResponse { Token = CreateToken(user) };
                }
            }

            return new AuthenticationResponse { Error = "Invalid username or password." };
        }

        public async Task<RegistrationResponse> Register(RegistrationRequest request)
        {
            // TODO:
            // validate username
            // validate password
            // handle db exceptions

            if (!request.Username.All(c => char.IsLower(c) || char.IsDigit(c)))
            {
                return new RegistrationResponse { Token = null, Error = "Username may contain only digits and lowercase letters." };
            }

            if (_dataContext.Users.Any(u => u.Name == request.Username))
            {
                return new RegistrationResponse { Error = "Username already exists." };
            }

            var (hash, salt) = await Task.Run(() => _passwordHasher.CreatePassword(request.Password));

            var user = new User
            {
                Name = request.Username,
                PasswordHash = hash,
                PasswordSalt = salt,
            };

            _dataContext.Users.Add(user);
            var success = _dataContext.SaveChanges() > 0;

            return success ? new RegistrationResponse { Token = CreateToken(user) } : new RegistrationResponse { Error = "Failed to register user." };
        }

        public async Task<GetDataResponse> GetData(GetDataRequest request)
        {
            var user = await _dataContext.Users.FirstAsync(u => u.Name == request.Username);
            return new GetDataResponse { Data = user.Data };
        }

        public async Task<SetDataResponse> SetData(SetDataRequest request)
        {
            var user = await _dataContext.Users.FirstAsync(u => u.Name == request.Username);
            user.Data = request.Data;
            var success = _dataContext.SaveChanges() > 0;

            return success ? new SetDataResponse() : new SetDataResponse { Error = "Failed to update user." };
        }

        private string CreateToken(User user)
        {
            var key = new SymmetricSecurityKey(Convert.FromBase64String(_configuration.GetSection("AppSettings:Jwt").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Name)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
