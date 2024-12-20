namespace ApiRestJMR.Models
{
    /// <summary>
    /// Object of the LDAP settings recorded in the appsettings.json file
    /// </summary>
    public class LdapSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string BaseDn { get; set; }
        public string UserDn { get; set; }
        public string organizationUnit { get; set; }
        public string Password { get; set; }
    }
}
