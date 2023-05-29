using Claim.Helper;
using Claim.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Claim_based_authorization_v6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserClaimController : ControllerBase
    {
        private readonly IUserClaimServices _userClaimServices;
       public UserClaimController(IUserClaimServices userClaimServices)
        {
            _userClaimServices= userClaimServices;
        }


        [Authorize]
        [HttpGet("GetAdminClaims")] 
        public async Task<IActionResult> GetAdminClaims()
        {
            var result = await _userClaimServices.GetUserClaims(x => x.ClaimType == Constants.ROLE_ADMIN || x.ClaimType == Constants.ROLE_MANAGER);
            return Ok(result);
        }

        [HttpGet("GetManagerClaims")]
        public async Task<IActionResult> GetManagerClaims()
        {
            var result = await _userClaimServices.GetUserClaims(x => x.ClaimType == Constants.ROLE_MANAGER);
            return Ok(result);
        }
    }
}
