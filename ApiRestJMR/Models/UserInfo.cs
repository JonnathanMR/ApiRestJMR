namespace ApiRestJMR.Models
{
    /// <summary>
    /// User data obtained from LDAP
    /// </summary>
    public class UserInfo
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Department { get; set; }
        public string Title { get; set; }
    }
}
