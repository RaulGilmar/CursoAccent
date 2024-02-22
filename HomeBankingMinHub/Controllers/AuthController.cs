using Microsoft.AspNetCore.Mvc;
using HomeBankingMindHub.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Text;
using System.Security.Cryptography;
using HomeBankingMindHub.Repositories.Interfaces;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IClientRepository _clientRepository;
        public AuthController(IClientRepository clientRepository) 
        {
            _clientRepository = clientRepository;
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Client LoginRequestDTO) 
        {
            try 
            {
                Client user = _clientRepository.FindByEmail(LoginRequestDTO.Email);
                if (user != null) 
                {
                    if (!VerifyPassword(LoginRequestDTO.Password, user.Password))
                        return StatusCode(400, "Credenciales incorrectas");
                }
                else 
                {
                    return StatusCode(404, "Usuario no encontrado");
                }
                var claims = new List<Claim>
                {
                    new Claim ("Client", user.Email)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                              new ClaimsPrincipal(claimsIdentity));

                return StatusCode(200,"Inicio de sesión exitoso");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        private bool VerifyPassword(string enteredPassword, string savedPasswordHash)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedPasswordBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));
                string enteredPasswordHash = Convert.ToBase64String(hashedPasswordBytes);
                return savedPasswordHash == enteredPasswordHash;
            }
        }
        [HttpPost("Logout")]
        public async Task <IActionResult> Logout()
        {
            try 
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
