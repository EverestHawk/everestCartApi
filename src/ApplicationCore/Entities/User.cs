using System;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Entities
{
    public class User : BaseEntity
    {
        public string IdentityId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string UserName { get; private set; }
        public string Address { get; set; }
        public byte? Gender { get; set; }
        public DateTime DateOfBirth { get; set; }

        private List<RefreshToken> _refreshTokens = new List<RefreshToken>();
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
        private User() { }
        public User(string firstName, string lastName, string identityId, string userName)
        {
            FirstName = firstName;
            LastName = lastName;
            IdentityId = identityId;
            UserName = userName;
        }
        public bool HasValidRefreshToken(string refreshToken)
        {
            return _refreshTokens.Any(t => t.Token == refreshToken && t.IsActive);
        }

        public void AddRefreshToken(string token, int userId, string remoteIpAddress, double daysToExpire = 1)
        {
            _refreshTokens.Add(new RefreshToken
            {
                UserId = userId,
                ExpiresAt = DateTime.Now.AddDays(daysToExpire),
                RemoteIpAddress = remoteIpAddress,
                Token = token
            });
        }

        public void ExpireRefreshToken(string token)
        {
            var obj = _refreshTokens.FirstOrDefault(t => t.Token == token && !t.IsExpired && !t.IsActive);
            if (obj != null)
            {
                obj.IsExpired = true;
            }
        }

        public void ExpireAllActiveTokenForUser(int userId)
        {
            var tokenObjs = _refreshTokens.FindAll(t => t.UserId == userId && !t.IsExpired);

            foreach (var obj in tokenObjs)
            {
                obj.IsExpired = true;
            }
        }
    }
}
