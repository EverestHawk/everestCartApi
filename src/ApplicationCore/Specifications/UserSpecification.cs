using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Specifications
{
    public class UserSpecification : BaseSpecification<User>
    {
        public UserSpecification(string identityId) : base(u => u.IdentityId == identityId)
        {
            AddInclude(u => u.RefreshTokens);
        }
    }
}
