using ApiRestJMR.Models;
using ApiRestJMR.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiRestJMR.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly LdapService _ldapService;
        private readonly EncryptionServices _encryptionService;

        public IConfiguration _configuration;

        public AuthController(LdapService ldapService, EncryptionServices encryptionService, IConfiguration configuration)
        {
            _ldapService = ldapService;
            _encryptionService = encryptionService;
            _configuration = configuration;
        }

        /// <summary>
        /// Method that generates the token
        /// </summary>
        /// <param name="request">
        /// request.Subject
        /// request.SecretKey
        /// </param>
        /// <returns>
        /// Returns the satisfactory state and the token if the subject and the secret key are correct
        /// </returns>
        [HttpGet("token")]
        public IActionResult GetToken([FromBody] AuthRequest request)
        {
            try
            {
                var jwt = _configuration.GetSection("Jwt").Get<JwtSettings>();

                if (jwt.Subject == request.Subject && jwt.Key == _encryptionService.Base64Decode(request.SecretKey))
                {
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("subject", request.Subject),
                        new Claim("key", request.SecretKey),
                        new Claim("expire", DateTime.Now.AddMinutes(60).ToString())
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));

                    var singIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                            jwt.Issuer,
                            jwt.Audience,
                            claims: claims,
                            expires: DateTime.Now.AddMinutes(60),
                            signingCredentials: singIn
                        );

                    return Ok(new { status = "success", message = "Token successfully generated", result = new { token = new JwtSecurityTokenHandler().WriteToken(token), expires = DateTime.Now.AddMinutes(60).ToString() } });
                }
                else 
                {
                    return BadRequest(new { status = "error", message = "Invalid subject or secret key" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });
            }
        }

        /// <summary>
        /// Method that receives the token for validation and authenticates the user before the LDAP
        /// </summary>
        /// <param name="request"></param>
        /// <returns>
        /// Return user data from LDAP</returns>
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] LoginRequest request)
        {
            try
            {
                var jwt = _configuration.GetSection("Jwt").Get<JwtSettings>();

                var identity = HttpContext.User.Identity as ClaimsIdentity;

                var responseToken = ValidateToken(identity!);

                if (!responseToken?.status) return BadRequest(new { status = "error", message = responseToken });

                var userInfo = _ldapService.Authenticate(request.Username, request.Password);
                return Ok(new { status = "success", user = userInfo });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = "Invalid credentials or user not found." });
            }
        }

        /// <summary>
        /// Method that validates the integrity and validity of the token
        /// </summary>
        /// <param name="identity">
        /// Object of type ClaimsIdentity
        /// </param>
        /// <returns>
        /// True when being a valid token, false otherwise
        /// </returns>
        public dynamic ValidateToken(ClaimsIdentity identity)
        {
            try
            {
                var jwt = _configuration.GetSection("Jwt").Get<JwtSettings>();
                if (identity.Claims.Count() == 0)
                {
                    return Ok(new { status = false, message = "Invalid token", result = "" });
                }

                var id = identity.Claims.FirstOrDefault(x => x.Type == "subject")?.Value;
                var key = identity.Claims.FirstOrDefault(x => x.Type == "key")?.Value;
                var expire = identity.Claims.FirstOrDefault(x => x.Type == "expire")?.Value!;

                if (id != jwt.Subject || _encryptionService.Base64Decode(key!) != jwt.Key) 
                    return new { status = false, message = "Invalid token", result = "" };

                if(DateTime.Now > DateTime.Parse(expire))
                    return new { status = false, message = "Expired token", result = "" };

                return new { status = true, message = "success"};
            }
            catch (Exception ex)
            {
                return new { status = false, message = ex.Message, result = "" };
            }
        }
    }

}
