namespace ApiRestJMR.Services
{
    public class EncryptionServices
    {
        /// <summary>
        /// Method that receives the secret key to decrypt it for validation
        /// </summary>
        /// <param name="base64EncodedData">
        /// The secret key encrypted in base64
        /// </param>
        /// <returns>
        /// Secret key decrypted
        /// </returns>
        public string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
