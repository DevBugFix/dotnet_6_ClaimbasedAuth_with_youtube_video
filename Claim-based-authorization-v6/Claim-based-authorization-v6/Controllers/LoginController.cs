using Claim.Common.BindingModels;
using Claim.Data.Entities;
using Claim.DTO;
using Claim.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Claim_based_authorization_v6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        public LoginController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;

        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> Login([FromBody] LoginBindingModel model)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                var userDTO = new UserDTO();
                if(result != null && result.Succeeded)
                {
                    var tempUser=await _userManager.FindByEmailAsync(model.Email);
                    var tempUserClaims = _userManager.GetClaimsAsync(tempUser).Result.ToList();
                    userDTO.Claims = new List<UserClaimDTO>();
                    foreach(var claim in tempUserClaims)
                    {
                        userDTO.Claims.Add(new UserClaimDTO() { ClaimType = claim.Type, ClaimValue = claim.Value });
                    }

                    string role = _userManager.GetRolesAsync(tempUser).Result.FirstOrDefault() ?? "";
                    if(role==Constants.ROLE_ADMIN)
                    {
                        userDTO.Claims.Add(new UserClaimDTO() { ClaimType = Constants.ROLE_ADMIN, ClaimValue = role });
                    }
                    if (role == Constants.ROLE_MANAGER)
                    {
                        userDTO.Claims.Add(new UserClaimDTO() { ClaimType = Constants.ROLE_MANAGER, ClaimValue = role });
                    }

                    userDTO.FullName = tempUser.FullName;
                    userDTO.Role = role;
                    userDTO.Email=tempUser.Email;
                    userDTO.Token = GenerateToken(tempUser, userDTO.Claims);

                    return userDTO;
                }
                else
                {
                    return BadRequest("Email or password is incorrect");
                }

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    
        private string GenerateToken(AppUser appuser, List<UserClaimDTO> userClaims)
        {
            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub,appuser.Id.ToString()),
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Name,appuser.UserName),
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            };

            var roleClaims = userClaims.Select(c => new System.Security.Claims.Claim(c.ClaimType, c.ClaimValue));
            claims.AddRange(roleClaims);
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Constants.SECRET_KEY));
            var cred =new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer:Constants.Issuer,
                audience:Constants.Audience,
                claims:claims,
                expires:DateTime.Now.AddDays(30),
                signingCredentials:cred
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
     
    }
}
