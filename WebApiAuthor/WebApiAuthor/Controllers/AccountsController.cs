using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApiAuthor.DTOs;

namespace WebApiAuthor.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
   private readonly UserManager<IdentityUser> _userManager;
   private readonly IConfiguration _configuration;
   private readonly SignInManager<IdentityUser> _signInManager;
   private readonly ApplicationDbContext _context;
   private readonly IMapper _mapper;

   public AccountsController(UserManager<IdentityUser> userManager, IConfiguration configuration, 
      SignInManager<IdentityUser> signInManager)
   {
      _userManager = userManager;
      _configuration = configuration;
      _signInManager = signInManager;
   }

   [HttpPost("register")] //api/accounts/register
   public async Task<ActionResult<AuthenticationResponse>> Register(UserCredentials userCredentials)
   {
       var user = new IdentityUser
       {
           UserName = userCredentials.Email,
           Email = userCredentials.Email
       };

    var result = await _userManager.CreateAsync(user, userCredentials.Password);

       if (result.Succeeded)
       {
          return BuildToken(userCredentials); //Se le brinda el token al usuario. Firmado con una llave secreta.
       }
       else
       {
          return BadRequest(result.Errors);
       }                                      
   }

   [HttpPost("login")]
   public async Task<ActionResult<AuthenticationResponse>> Login(UserCredentials userCredentials)
   {
      var result = await _signInManager.PasswordSignInAsync(userCredentials.Email, userCredentials.Password,
         isPersistent: false, lockoutOnFailure: false);

      if (result.Succeeded) return BuildToken(userCredentials);
      else return BadRequest("Incorrect Login");       
   }

   private AuthenticationResponse BuildToken(UserCredentials userCredentials)
   {
      var claims = new List<Claim>()
      {
         new Claim("email", userCredentials.Email), //Es importante nunca poner data sensitiva en un claim
         new Claim("Example", "Example Value")
      };

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var expiration = DateTime.UtcNow.AddYears(1);

      var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration,
         signingCredentials: credentials);

      return new AuthenticationResponse()
      {
         Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
         ExpirationTime = expiration
      };
   }
}