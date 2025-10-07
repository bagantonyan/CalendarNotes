using Duende.IdentityServer.Models;
using Duende.IdentityServer;

namespace CalendarNotes.IdentityServer.Configuration
{
    /// <summary>
    /// Конфигурация IdentityServer
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Identity ресурсы
        /// </summary>
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };

        /// <summary>
        /// API области (Scopes)
        /// </summary>
        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("calendarnotes.api", "CalendarNotes API")
            };

        /// <summary>
        /// API ресурсы
        /// </summary>
        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("calendarnotes.api", "CalendarNotes API")
                {
                    Scopes = { "calendarnotes.api" }
                }
            };

        /// <summary>
        /// Клиенты
        /// </summary>
        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // Клиент для веб-приложения
                new Client
                {
                    ClientId = "calendarnotes.web",
                    ClientName = "CalendarNotes Web Client",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "calendarnotes.api"
                    },
                    
                    AccessTokenLifetime = 3600, // 1 час
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    AllowOfflineAccess = true
                },
                
                // Клиент для Swagger
                new Client
                {
                    ClientId = "calendarnotes.swagger",
                    ClientName = "CalendarNotes Swagger",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "calendarnotes.api"
                    }
                }
            };
    }
}
