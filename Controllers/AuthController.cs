using Microsoft.AspNetCore.Mvc;
using CharityHubOnionArchitecture.Application;
 

namespace CharityHubOnionArchitecture.Controllers
{

    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthenticateHandler _authenticateHandler;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthenticateHandler authenticateHandler, ILogger<AuthController> logger)
        {
            _authenticateHandler = authenticateHandler;
            _logger = logger;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Login([FromBody] Authenticate authenticate)
        {
            _logger.LogInformation("Processing authentication request");

            try
            {
                var response = await _authenticateHandler.HandleAsync(authenticate);
                return Ok(response);  
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing authentication");
                return StatusCode(500, "Internal Server Error"); // Handle any errors
            }
        }
    }
}

 
