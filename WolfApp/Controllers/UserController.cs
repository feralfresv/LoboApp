using Microsoft.AspNetCore.Mvc;
using WolfApp.Models.Request;
using WolfApp.Models.Response;
using WolfApp.Services;

namespace WolfApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public IActionResult Authentication([FromBody] AuthRequest model)
        {
            Response response = new Response();
            var userResponse = _userService.Auth(model);

            if (userResponse == null)
            {
                response.Exito = 0;
                response.Mensaje = "Usuario o contraseña incorrecta";
                return BadRequest(response);
            }

            response.Exito = 1;
            response.Data = userResponse;
            return Ok(response);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] NewUserRequest model)
        {
            Response response = new Response();
            var newUserResponse = _userService.NewUser(model);
            
            if (newUserResponse == "Duplicate Email")
            {
                response.Exito = 0;
                response.Mensaje = "Duplicate Email";
                return BadRequest(response);
            }

            if (newUserResponse == "Incorrect Password")
            {
                response.Exito = 0;
                response.Mensaje = "La contraseña no son iguales";
                return BadRequest(response);
            }

            response.Exito = 1;
            response.Mensaje = "Created";
            return Ok(response);
        }
    }
}