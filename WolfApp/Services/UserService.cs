using WolfApp.Models;
using WolfApp.Models.Request;
using WolfApp.Models.Response;
using WolfApp.Tools;
using System.Linq;
using WolfApp.Models.Common;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System;

namespace WolfApp.Services
{
    public class UserService : IUserService
    {
        private readonly db_WolfContext _context;
        private readonly AppSettings _appSettings;
        public UserService(db_WolfContext context, IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _context = context;
        }
        public UserResponse Auth(AuthRequest model)
        {
            UserResponse userResponse = new UserResponse();


            string password = Encrypt.GetSHA256(model.Password);

            var user = _context.User.Where(d => d.Email == model.Email && d.Password == password).FirstOrDefault();

            if (user == null) return null;

            userResponse.Email = user.Email;
            userResponse.Token = GetToken(user);

            return userResponse;
        }

        public string NewUser(NewUserRequest model)
        {        
            var duplicate = _context.User.Any(i => i.Email == model.Email);
            if (!duplicate)
            {
                string password = Encrypt.GetSHA256(model.Password);

                if (model.Password == model.ConfirmPassword)
                {
                    User newUser = new User
                    {
                        Email = model.Email,
                        Password = password,
                        Name = model.Name
                    };
                    _context.User.Add(newUser);
                    _context.SaveChanges();

                    return newUser.ToString();
                }
                return "Incorrect Password";
            }
            return "Duplicate Email";
        }

        private string GetToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var llave = Encoding.ASCII.GetBytes(_appSettings.SecretKey);
            var tokenDesriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Email, user.Email)
                    }
                    ),
                Expires = DateTime.UtcNow.AddDays(60),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(llave), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDesriptor);
            return tokenHandler.WriteToken(token);
        }


    }
}