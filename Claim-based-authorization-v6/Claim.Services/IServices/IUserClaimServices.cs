using Claim.Data.Entities;
using Claim.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Claim.Services.IServices
{
    public interface IUserClaimServices
    {
        Task<List<UserClaimDTO>> GetUserClaims(Expression<Func<AppUserClaim, bool>> expression);
    }
}
