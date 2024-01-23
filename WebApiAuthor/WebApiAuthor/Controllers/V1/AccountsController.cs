using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApiAuthor.DTOs;
using WebApiAuthor.Services;

namespace WebApiAuthor.Controllers.V1;

[ApiController]
[Route("api/v1/accounts")]
public class AccountsController : ControllerBase
{
   private readonly UserManager<IdentityUser> _userManager;
   private readonly IConfiguration _configuration;
   private readonly SignInManager<IdentityUser> _signInManager;
   private readonly HashService _hashService;
   private readonly ApplicationDbContext _context;
   private readonly IMapper _mapper;
   private readonly IDataProtector _dataProtector;         

   public AccountsController(UserManager<IdentityUser> userManager, IConfiguration configuration, 
      SignInManager<IdentityUser> signInManager, IDataProtectionProvider dataProtectionProvider, HashService hashService)
   {
      _userManager = userManager;
      _configuration = configuration;
      _signInManager = signInManager;
      _hashService = hashService;
      _dataProtector = dataProtectionProvider.CreateProtector("unique_value");
   }

   [HttpGet("hash/{planeText}")]
   public ActionResult PerformHash(string planeText)
   {
      //Hago un hash 2 veces para ver que hay 2 sales distintas
      var result1 = _hashService.Hash(planeText);
      var result2 = _hashService.Hash(planeText);

      return Ok(new
      {
         planeText = planeText,
         Hash1 = result1,
         Hash2 = result2
      });
   }

   [HttpGet("encrypt")]
   public ActionResult Encrypt()
   {
      var planeText = "Roman Vitolo";
      var encryptionText = _dataProtector.Protect(planeText);
      var decryptionText = _dataProtector.Unprotect(encryptionText);

      return Ok(new
      {
         planeText = planeText,
         encryptionText = encryptionText,
         decryptionText = decryptionText
      });
   }
   
   [HttpGet("encryptByTime")]
   public ActionResult EncryptByTime()
   {
      //var timeProtected = _dataProtector.ToTimeLimitedDataProtector();
      
      var planeText = "Roman Vitolo";
      //var encryptionText = timeProtected.Protect(planeText, lifetime: TimeSpan.FromSeconds(5));
      var encryptionText = _dataProtector.Protect(planeText);
      Thread.Sleep(6000);
      //var decryptionText = timeProtected.Unprotect(encryptionText);
      var decryptionText = _dataProtector.Unprotect(encryptionText);

      return Ok(new
      {
         planeText = planeText,
         encryptionText = encryptionText,
         decryptionText = decryptionText
      });
   }

   [HttpPost("register", Name = "userRegister")] //api/accounts/register
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
          return await BuildToken(userCredentials); //Se le brinda el token al usuario. Firmado con una llave secreta.
       }
       else
       {
          return BadRequest(result.Errors);
       }                                      
   }

   [HttpPost("login", Name = "userLogin")]
   public async Task<ActionResult<AuthenticationResponse>> Login(UserCredentials userCredentials)
   {
      var result = await _signInManager.PasswordSignInAsync(userCredentials.Email, userCredentials.Password,
         isPersistent: false, lockoutOnFailure: false);

      if (result.Succeeded) return await BuildToken(userCredentials);
      else return BadRequest("Incorrect Login");       
   }

   [HttpGet("RenovateToken", Name = "renovateToken")]
   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
   public async Task<ActionResult<AuthenticationResponse>> Renovate()
   {
      var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();  
      var email = emailClaim.Value;
      var userCredentials = new UserCredentials
      {
         Email = email
      };
      
      return await BuildToken(userCredentials);
   }

   private async Task<AuthenticationResponse> BuildToken(UserCredentials userCredentials)
   {
      var claims = new List<Claim>()
      {
         new Claim("email", userCredentials.Email), //Es importante nunca poner data sensitiva en un claim
         new Claim("Example", "Example Value")
      };

      var user = await _userManager.FindByEmailAsync(userCredentials.Email);
      var claimsDB = await _userManager.GetClaimsAsync(user);
      
      claims.AddRange(claimsDB);

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var expiration = DateTime.UtcNow.AddYears(1); //La duracion del token es para pruebas. Deberia ser mucho menor el tiempo

      var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration,
         signingCredentials: credentials);

      return new AuthenticationResponse()
      {
         Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
         ExpirationTime = expiration
      };
   }

   [HttpPost("CreateAdmin", Name = "createAdmin")]
   public async Task<ActionResult> CreateAdmin(EditAdminDTO editAdminDto)
   {
      var user = await _userManager.FindByEmailAsync(editAdminDto.Email);
      await _userManager.AddClaimAsync(user, new Claim("IsAdmin", "1"));
      return NoContent();
   }
   
   [HttpPost("RemoveAdmin", Name = "removeAdmin")]
   public async Task<ActionResult> RemoveAdmin(EditAdminDTO editAdminDto)
   {
      var user = await _userManager.FindByEmailAsync(editAdminDto.Email);
      await _userManager.RemoveClaimAsync(user, new Claim("IsAdmin", "1"));
      return NoContent();
   }
}