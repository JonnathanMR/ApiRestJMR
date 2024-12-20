namespace ApiRestJMR.Models
{
    /// <summary>
    /// Parameters required for token creation
    /// </summary>
    public class AuthRequest
    {
        
        public string Subject { get; set; }
        public string SecretKey { get; set; }
    }
}
