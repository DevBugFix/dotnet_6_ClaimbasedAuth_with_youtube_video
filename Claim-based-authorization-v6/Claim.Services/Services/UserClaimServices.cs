using Claim.Data;
using Claim.Data.Entities;
using Claim.DTO;
using Claim.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Claim.Services.Services
{
    public class UserClaimServices : IUserClaimServices
    {
        private readonly AppDbContext _apDbContext;

        public UserClaimServices(AppDbContext apDbContext)
        {
            _apDbContext = apDbContext;
        }

        public async Task<List<UserClaimDTO>> GetUserClaims(Expression<Func<AppUserClaim, bool>> expression)
        {
          return  await (from claim in _apDbContext.AppUserClaim.Where(expression)
                   select new UserClaimDTO()
                   {
                       ClaimType = claim.ClaimType,
                       ClaimValue = claim.ClaimValue,
                   }).ToListAsync();
        }
    }
}
