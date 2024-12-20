using ApiRestJMR.Models;
using Novell.Directory.Ldap;

namespace ApiRestJMR.Services
{
    public class LdapService
    {
        private readonly IConfiguration _configuration;

        public LdapService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Method that verifies if the data provided by the user exists in the LDAP
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>
        /// The object with the data found in the LDAP
        /// </returns>
        /// <exception cref="Exception"></exception>
        public UserInfo Authenticate(string username, string password)
        {
            var ldapSettings = _configuration.GetSection("LdapSettings").Get<LdapSettings>();

            using (var connection = new LdapConnection())
            {
                connection.Connect(ldapSettings.Server, ldapSettings.Port);
                connection.Bind($"uid={username},{ldapSettings.organizationUnit},{ldapSettings.BaseDn}", password);

                if (connection.Bound)
                {
                    var searchFilter = $"(uid={username})";
                    var searchResults = connection.Search(
                        ldapSettings.BaseDn,
                        LdapConnection.ScopeSub,
                        searchFilter,
                        null,
                        false
                    );

                    if (searchResults.HasMore())
                    {
                        var entry = searchResults.Next();
                        return new UserInfo
                        {
                            Username = username,
                            Email = entry.GetAttribute("mail").StringValue,
                            DisplayName = entry.GetAttribute("cn").StringValue,
                            Department = entry.GetAttribute("departmentNumber").StringValue,
                            Title = entry.GetAttribute("title").StringValue
                        };
                    }
                }

                throw new Exception("Invalid credentials or user not found.");
            }
        }
    }

}
