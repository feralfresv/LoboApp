using WolfApp.Models.Response;
using WolfApp.Models.Request;

namespace WolfApp.Services
{
    public interface IUserService
    {
        UserResponse Auth(AuthRequest model);
        string NewUser(NewUserRequest model);
    }
}