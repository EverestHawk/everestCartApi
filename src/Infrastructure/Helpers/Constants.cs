using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Helpers
{
    public static class Constants
    {
        public static class JwtClaimIdentifiers
        {
            public const string Role = "role", Id = "id";
        }

        public static class JwtClaims
        {
            public const string ApiAccess = "everestCart_api_access";
        }
    }
}
