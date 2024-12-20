using System.Security.Claims;

namespace ApiRestJMR.Models
{
    /// <summary>
    /// Subject of the JWT settings recorded in the appsettings.json file
    /// </summary>
    public class JwtSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Subject { get; set; }
    }
}
