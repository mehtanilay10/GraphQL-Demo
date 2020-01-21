using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PizzaOrder.API.Models;
using PizzaOrder.Business.Helpers;

namespace PizzaOrder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IConfiguration configuration, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody]LoginDetails model)
        {
            // Check user exist in system or not
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return NotFound();
            }

            // Perform login operation
            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
            if (signInResult.Succeeded)
            {
                // Obtain token
                TokenDetails token = await GetJwtSecurityTokenAsync(user);
                return Ok(token);
            }
            else
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> CreateDefaultUsers()
        {
            #region Roles

            var rolesDetails = new List<string>
            {
                    Constants.Roles.Customer,
                    Constants.Roles.Restaurant,
                    Constants.Roles.Admin
            };

            foreach (string roleName in rolesDetails)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            #endregion

            #region Users

            var userDetails = new Dictionary<string, IdentityUser>{
                {
                    Constants.Roles.Customer,
                    new IdentityUser { Email = "customer@demo.com", UserName = "CustomerUser", EmailConfirmed = true }
                },
                {
                    Constants.Roles.Restaurant,
                    new IdentityUser { Email = "restaurant@demo.com", UserName = "RestaurantUser", EmailConfirmed = true }
                },
                {
                    Constants.Roles.Admin,
                    new IdentityUser { Email = "admin@demo.com", UserName = "AdminUser", EmailConfirmed = true }
                }
            };

            foreach (var details in userDetails)
            {
                var existingUserDetails = await _userManager.FindByEmailAsync(details.Value.Email);
                if (existingUserDetails == null)
                {
                    await _userManager.CreateAsync(details.Value);
                    await _userManager.AddPasswordAsync(details.Value, "Password");
                    await _userManager.AddToRoleAsync(details.Value, details.Key);
                }
            }

            #endregion

            return Ok("Default User has been created");
        }

        public async Task<IActionResult> ProtectedPage()
        {
            // Obtain MailId from token
            ClaimsIdentity identity = _httpContextAccessor?.HttpContext?.User?.Identity as ClaimsIdentity;
            var userName = identity?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var user = await _userManager.FindByNameAsync(userName);
            return Ok(user);
        }

        private async Task<TokenDetails> GetJwtSecurityTokenAsync(IdentityUser user)
        {
            var keyInBytes = System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("JwtIssuerOptions:SecretKey").Value);
            SigningCredentials credentials = new SigningCredentials(new SymmetricSecurityKey(keyInBytes), SecurityAlgorithms.HmacSha256);
            DateTime tokenExpireOn = DateTime.Now.AddDays(3);

            // Obtain Role of User
            IList<string> rolesOfUser = await _userManager.GetRolesAsync(user);

            // Add new claims
            List<Claim> tokenClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, rolesOfUser.FirstOrDefault()),
            };

            // Make JWT token
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration.GetSection("JwtIssuerOptions:Issuer").Value,
                audience: _configuration.GetSection("JwtIssuerOptions:Audience").Value,
                claims: tokenClaims,
                expires: tokenExpireOn,
                signingCredentials: credentials
            );

            // Return it
            TokenDetails TokenDetails = new TokenDetails
            {
                UserId = user.Id,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpireOn = tokenExpireOn,
            };

            // Set current user details for busines & common library
            var currentUser = await _userManager.FindByEmailAsync(user.Email);

            // Add new claim details
            var existingClaims = await _userManager.GetClaimsAsync(currentUser);
            await _userManager.RemoveClaimsAsync(currentUser, existingClaims);
            await _userManager.AddClaimsAsync(currentUser, tokenClaims);

            return TokenDetails;
        }
    }
}
