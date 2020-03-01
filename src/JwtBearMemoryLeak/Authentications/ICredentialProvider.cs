namespace JwtBearMemoryLeak.Authentications
{
    using Microsoft.IdentityModel.Tokens;

    public interface ICredentialProvider
    {
        SecurityKey GetSigningKey();
    }
}
