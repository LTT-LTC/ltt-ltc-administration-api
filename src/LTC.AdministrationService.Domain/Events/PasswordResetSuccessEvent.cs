namespace LTC.AdministrationService.Events
{
    /// <summary>
    /// Event raised when password is successfully reset
    /// </summary>
    public class PasswordResetSuccessEvent
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Time { get; set; }
        public string Date { get; set; }
    }
}


