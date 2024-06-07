using Duende.IdentityServer.Models;

namespace MSA.IdentityService.DummyData
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            [
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            ];

        public static IEnumerable<ApiScope> ApiScopes =>
            [
                new ApiScope("productapi.read"),
                new ApiScope("productapi.write"),
                new ApiScope("orderapi.read"),
                new ApiScope("orderapi.write"),
                new ApiScope("paymentapi.read"),
                new ApiScope("paymentapi.write"),
            ];

        public static IEnumerable<ApiResource> ApiResources =>
        [
            new ApiResource("productapi")
            {
                Scopes = ["productapi.read", "productapi.write"],
                ApiSecrets = [new Secret("Scopesecret".Sha256())],
                UserClaims = ["role"]
            },
            new ApiResource("orderapi")
            {
                Scopes = ["orderapi.read", "orderapi.write"],
                ApiSecrets = [new Secret("Scopesecret".Sha256())],
                UserClaims = ["role"]
            },
            new ApiResource("paymentapi")
            {
                Scopes = ["paymentapi.read", "paymentapi.write"],
                ApiSecrets = [new Secret("Scopesecret".Sha256())],
                UserClaims = ["role"]
            }
        ];

        public static IEnumerable<Client> Clients =>
            [
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "m2m.client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = {
                        "productapi.read",
                        "productapi.write",
                        "orderapi.read",
                        "orderapi.write",
                        "paymentapi.read",
                        "paymentapi.write"
                    }
                },

                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "interactive",
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { "https://localhost:5002/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:5002/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "productapi.read", "productapi.write" }
                },

                // product-swagger client using code flow + pkce
                new Client
                {
                    ClientId = "product-swagger",
                    RequireClientSecret = false,

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = false,

                    RedirectUris = { "https://localhost:5002/swagger/oauth2-redirect.html" },
                    AllowedCorsOrigins = { "https://localhost:5002" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "productapi.read", "productapi.write" },

                    AlwaysIncludeUserClaimsInIdToken = true
                },
                new Client
                {
                    ClientId = "payment-swagger",
                    RequireClientSecret = false,

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = false,

                    RedirectUris = { "https://localhost:5004/swagger/oauth2-redirect.html" },
                    AllowedCorsOrigins = { "https://localhost:5004" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "paymentapi.read", "paymentapi.write" }
                },
                new Client
                {
                    ClientId = "order-swagger",
                    RequireClientSecret = false,

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = false,

                    RedirectUris = { "https://localhost:5003/swagger/oauth2-redirect.html" },
                    AllowedCorsOrigins = { "https://localhost:5003" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "orderapi.read", "orderapi.write", "paymentapi.read", "paymentapi.write", "productapi.read" }
                }
            ];
    }
}