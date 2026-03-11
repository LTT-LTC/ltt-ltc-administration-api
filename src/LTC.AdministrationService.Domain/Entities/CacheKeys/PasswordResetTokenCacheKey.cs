namespace LTC.AdministrationService.Entities.CacheKeys
{
    public class PasswordResetTokenCacheKey
    {
        public string Token { get; set; }

        public override string ToString()
        {
            return $"PasswordReset:{Token}";
        }
    }
}

