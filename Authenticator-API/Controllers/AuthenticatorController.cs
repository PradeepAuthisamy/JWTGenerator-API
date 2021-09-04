using Authenticator_API.Model;
using Authenticator_API.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authenticator_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticatorController : ControllerBase
    {
        private readonly IAuthenticatorService _authenticationService;

        public AuthenticatorController(IAuthenticatorService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("Authenticate")]
        [AllowAnonymous]
        public IActionResult Authenticate([FromBody] UserCredential userCredential)
        {
            if (string.IsNullOrEmpty(userCredential.UserName) &&
               string.IsNullOrEmpty(userCredential.Password))
            {
                return BadRequest("Please provide username and password");
            };

            var token = _authenticationService.
                            GenerateToken(userCredential.UserName, userCredential.Password);
            return Ok(token);
        }
    }
}