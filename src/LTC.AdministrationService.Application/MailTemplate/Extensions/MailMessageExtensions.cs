using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace LTC.AdministrationService.MailTemplate.Extensions
{
    public static class MailMessageExtensions
    {
        public static void AddRange(
            this MailAddressCollection collection,
            IEnumerable<string>? emails)
        {
            if (emails == null) return;

            foreach (var email in emails.Distinct())
            {
                collection.Add(new MailAddress(email));
            }
        }
    }
}
