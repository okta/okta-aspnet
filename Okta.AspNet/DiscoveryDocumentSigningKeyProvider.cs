using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Okta.AspNet
{
    internal sealed class DiscoveryDocumentSigningKeyProvider
    {
        private readonly ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;

        public DiscoveryDocumentSigningKeyProvider(
            ConfigurationManager<OpenIdConnectConfiguration> configurationManager)
        {
            _configurationManager = configurationManager;
        }

        public async Task<ICollection<SecurityKey>> GetSigningKeysAsync()
        {
            var discoveryDocument = await _configurationManager.GetConfigurationAsync().ConfigureAwait(false);
            return discoveryDocument.SigningKeys;
        }
    }
}
