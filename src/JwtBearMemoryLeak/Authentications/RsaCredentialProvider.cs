namespace JwtBearMemoryLeak.Authentications
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Tokens;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.OpenSsl;
    using Org.BouncyCastle.Security;
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Security.Cryptography;

    internal class RsaCredentialProvider : ICredentialProvider
    {
        private const string PublicPemHeader = "-----BEGIN PUBLIC KEY-----";
        private const string PublicPemFooter = "-----END PUBLIC KEY-----";
        private const string PublicKeyPath = "keys/app1/public.pem";

        private readonly IHostEnvironment _hostingEnvironment;
        private ConcurrentDictionary<string, RSAParameters> _rsaParametersCache = new ConcurrentDictionary<string, RSAParameters>();

        public RsaCredentialProvider(IHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        SecurityKey ICredentialProvider.GetSigningKey()
        {
            return new RsaSecurityKey(GetPublicRSAParameters());
        }

        private RSAParameters GetPublicRSAParameters()
        {
            RSAParameters rsaParameters;

            var publicKeyFile = Path.Combine(_hostingEnvironment.ContentRootPath, PublicKeyPath).ToLowerInvariant();

            if (!_rsaParametersCache.TryGetValue(publicKeyFile, out rsaParameters))
            {
                if (!File.Exists(publicKeyFile))
                {
                    throw new ArgumentException("Invalid public key path:" + publicKeyFile);
                }

                var keyContent = File.ReadAllText(publicKeyFile).TrimEnd(Environment.NewLine.ToCharArray());

                if (!IsValidPublicKey(keyContent))
                {
                    throw new ArgumentException("Invalid public pem format:" + publicKeyFile);
                }

                using (var sr = new StringReader(keyContent))
                {
                    var keyPair = (RsaKeyParameters)new PemReader(sr).ReadObject();

                    rsaParameters = DotNetUtilities.ToRSAParameters(keyPair);

                    _rsaParametersCache.TryAdd(publicKeyFile, rsaParameters);
                }
            }

            return rsaParameters;
        }

        private bool IsValidPublicKey(string pemContent)
        {
            return pemContent.StartsWith(PublicPemHeader) && pemContent.EndsWith(PublicPemFooter);
        }
    }
}
