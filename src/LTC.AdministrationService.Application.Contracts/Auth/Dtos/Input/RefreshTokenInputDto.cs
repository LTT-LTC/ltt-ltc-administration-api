using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTC.AdministrationService.Auth.Dtos.Input
{
    public class UserRefreshTokenDto
    {
        public string UserId { get; set; }
        public string SessionId { get; set; }
        public override string ToString()
        {
            return $"{SessionId}:{UserId}";
        }
    }

    public class RefreshLoginInputDto
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
    }
}
