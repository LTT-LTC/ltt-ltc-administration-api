using System;

namespace LTC.AdministrationService.Entities.CacheKeys
{
    public class PasswordResetTokenCacheItem
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime ExpirationTime { get; set; }
        public bool IsUsed { get; set; }
    }
}

