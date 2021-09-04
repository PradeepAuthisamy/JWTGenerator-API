using Authenticator_API.Model;
using Authenticator_API.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Authenticate([FromBody] UserCredential userCredential)
        {
            if (string.IsNullOrEmpty(userCredential.UserName))
            {
                return BadRequest("Please provide username");
            };

            var token = await _authenticationService.
                            GenerateTokenAsync(userCredential.UserName);
            return Ok(token);
        }
    }
}