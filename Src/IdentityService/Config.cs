using Duende.IdentityServer.Models;

namespace IdentityService
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            [new IdentityResources.OpenId(), new IdentityResources.Profile(),];

        public static IEnumerable<ApiScope> ApiScopes =>
            [new ApiScope("auctionApp", "Auction app full access")];

        public static IEnumerable<Client> GetClients(IWebHostEnvironment environment)
        {
            var clients = new List<Client>
            {
                // Postman client
                new() {
                    ClientId = "postman",
                    ClientName = "Postman",
                    AllowedScopes = { "openid", "profile", "auctionApp" },
                    RedirectUris = { "https://www.getpostman.com/oauth2/callback" },
                    ClientSecrets = [new Secret("postman".Sha256())],
                    AllowedGrantTypes = { GrantType.ResourceOwnerPassword }
                },
                // NextApp client
                new() {
                    ClientId = "nextApp",
                    ClientName = "NextApp",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    RequirePkce = true,
                    RedirectUris = {
                        "http://localhost:3000/api/auth/callback/id-server"
                    },
                    PostLogoutRedirectUris = {
                        "http://localhost:3000"
                    },
                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "auctionApp" },
                    AccessTokenLifetime = 3600 * 24 * 30,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RequireClientSecret = true,
                    AllowedCorsOrigins = { "http://localhost:3000" },
                    RequireConsent = false,
                    AllowPlainTextPkce = false
                }
            };

            return clients;
        }
        public static IEnumerable<Client> Clients => new List<Client>();
    }
}
