using System;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities
{
    public class RefreshToken
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string RemoteIpAddress { get; set; }
        public bool IsExpired { get; set; }
        public bool IsActive => DateTime.Now <= ExpiresAt;
    }
}
