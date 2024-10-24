using AuthenticationTask.DTO;
using AuthenticationTask.Identity;
using AuthenticationTask.ServiceContracts;
using AuthenticationTask.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationTask.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtService _jwtService;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }
        [HttpPost("register")]
        public async Task<ActionResult<ApplicationUser>> PostResister(RegisterDTO registerDTO)
        {
            if (ModelState.IsValid == false)
            {
                string errorMessage = string.Join(" | ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e =>
                    e.ErrorMessage));
                return Problem(errorMessage);
            }

            ApplicationUser user = new()
            {
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.PhoneNumber,
                UserName = registerDTO.Email,
                PersonName = registerDTO.PhoneNumber
            };
            IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);
           
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                var authenticationResponce = _jwtService.CreateTwtToken(user);
                
                
                return Ok(authenticationResponce);
            }
            else
            {
                string errorMessage = string.Join(" | ",
                    result.Errors.Select(e => e.Description));
                return Problem(errorMessage);
            }
        }
        [HttpGet]
        public async Task<IActionResult> IsEmailAlreadyRegister(string email)
        {
            ApplicationUser? user = await _userManager.FindByNameAsync(email);
            if (user == null)
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }




        [HttpPost("login")]
        public async Task<ActionResult<ApplicationUser>> PostLogin(LoginDTO loginDTO)
        {
            if (ModelState.IsValid == false)
            {
                string errorMessage = string.Join(" | ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e =>
                    e.ErrorMessage));
                return Problem(errorMessage);
            }
            var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(loginDTO.Email);
                if (user == null)
                {
                    return NoContent();
                }
                else
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    var authenticationResponce = _jwtService.CreateTwtToken(user);

                    return Ok(authenticationResponce);
                }

            }
            else
            {
                return Problem("Invalid email or password");
            }
        }

        [HttpGet("logout")]
        public async Task<ActionResult<IActionResult>> GetLogout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }
    }
}
