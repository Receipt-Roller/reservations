using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using wppReservations.Areas.Api1.Models;
using wppReservations.Models;

namespace wppReservations.Areas.Api1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly IConfiguration _configuration;

        public UsersController(UserManager<UserModel> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Authenticates a user and generates a JWT token.
        /// </summary>
        /// <param name="model">The login model containing the username and password.</param>
        /// <returns>A JWT token if authentication is successful; otherwise, an Unauthorized status.</returns>
        /// <response code="200">Returns the JWT token if the username and password are correct.</response>
        /// <response code="401">Returned if the user cannot be authenticated.</response>
        [HttpPost("/login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateToken([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds);

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            return Unauthorized();
        }

        /// <summary>
        /// Registers a new user with the provided details and confirms their email automatically.
        /// </summary>
        /// <param name="model">The registration details including username, email, and password.</param>
        /// <returns>A response indicating the success or failure of the registration process.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /user/register
        ///     {
        ///         "username": "johndoe",
        ///         "email": "johndoe@example.com",
        ///         "password": "StrongPassword123!"
        ///     }
        ///
        /// This endpoint handles user registration and simulates email confirmation internally. 
        /// No email will be sent to the user, but the email will be marked as confirmed in the database.
        /// </remarks>
        /// <response code="200">Returns a success message if the user is registered and email is confirmed</response>
        /// <response code="400">If the input model is invalid or if an error occurs during the user creation process</response>
        [HttpPost("/register")]
        [SwaggerOperation(Summary = "Register User", Description = "Registers a new user and confirms their email automatically.")]
        [SwaggerResponse(statusCode: 200, description: "User registered and email confirmed")]
        [SwaggerResponse(statusCode: 400, description: "Bad request if the input is invalid or the registration fails")]
        public async Task<ActionResult> RegisterUser([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new UserModel
            {
                UserName = model.Username,
                Email = model.Email,
                Name = model.Name
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Simulating email confirmation
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _userManager.ConfirmEmailAsync(user, token);

                return Ok("User registered and email confirmed");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return BadRequest(ModelState);
        }
    }
}
