namespace JwtBearMemoryLeak.Authentications
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Threading.Tasks;

    public class CustomJwtBearerOptions : IPostConfigureOptions<JwtBearerOptions>
    {
        internal const string JwtTokenHeaderName = "x-jwt-token";
        private readonly ICredentialProvider _credentialProvider;

        public CustomJwtBearerOptions(ICredentialProvider credentialProvider)
        {
            _credentialProvider = credentialProvider;
        }

        void IPostConfigureOptions<JwtBearerOptions>.PostConfigure(string name, JwtBearerOptions options)
        {
            options.Events = new JwtBearerEvents()
            {
                OnMessageReceived = (MessageReceivedContext context) =>
                 {
                     if (context.Request.Headers.ContainsKey(JwtTokenHeaderName))
                     {
                         context.Token = context.Request.Headers[JwtTokenHeaderName];
                     }

                     return Task.CompletedTask;
                 },

                OnTokenValidated = (TokenValidatedContext context) =>
                {
                    var userInfo = context.HttpContext.RequestServices.GetRequiredService<IUserInfo>();

                    userInfo.Bind(context.HttpContext, context.Principal);

                    return Task.CompletedTask;
                },

                OnChallenge = (JwtBearerChallengeContext context) =>
                {
                    if (context.Request.Headers.ContainsKey(JwtTokenHeaderName))
                    {
                        var token = context.Request.Headers[JwtTokenHeaderName];
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<CustomJwtBearerOptions>>();

                        logger.LogWarning($"JWT Authentication failed with token: {token}. {context.Request.Method} {context.Request.Path}. {context.AuthenticateFailure.Message}");
                    }

                    return Task.CompletedTask;
                },
            };

            options.RequireHttpsMetadata = false;
            options.SaveToken = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                AuthenticationType = JwtBearerDefaults.AuthenticationScheme,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero,

                IssuerSigningKeyResolver = (string token, SecurityToken securityToken, string kid, TokenValidationParameters validationParameters) =>
                {
                    return new SecurityKey[] { _credentialProvider.GetSigningKey() };
                }
            };
        }

    }
}
