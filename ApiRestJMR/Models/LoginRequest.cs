namespace ApiRestJMR.Models
{
    /// <summary>
    /// User data to authenticate against LDAP
    /// </summary>
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
