namespace EngineBay.Authentication
{
    using System.Collections.ObjectModel;

    public abstract class AuthenticationConfiguration
    {
        public static Collection<string> GetAudiences()
        {
            var audienceEnvironmentVariable = Environment.GetEnvironmentVariable(EnvironmentVariableConstants.AUTHENTICATIONAUDIENCE);
            var audiences = new Collection<string>();
            if (string.IsNullOrEmpty(audienceEnvironmentVariable))
            {
                Console.WriteLine($"Warning: {EnvironmentVariableConstants.AUTHENTICATIONAUDIENCE} not configured, using default '{DefaultAuthenticationConfigurationConstants.DefaultAudience}'.");
                audiences.Add(DefaultAuthenticationConfigurationConstants.DefaultAudience);
            }
            else
            {
                audiences.Add(audienceEnvironmentVariable);
            }

            return audiences;
        }

        public static string GetAudience()
        {
            var audienceEnvironmentVariable = Environment.GetEnvironmentVariable(EnvironmentVariableConstants.AUTHENTICATIONAUDIENCE);
            if (string.IsNullOrEmpty(audienceEnvironmentVariable))
            {
                Console.WriteLine($"Warning: {EnvironmentVariableConstants.AUTHENTICATIONAUDIENCE} not configured, using default '{DefaultAuthenticationConfigurationConstants.DefaultAudience}'.");
                return DefaultAuthenticationConfigurationConstants.DefaultAudience;
            }

            return audienceEnvironmentVariable;
        }

        public static SigningAlgorithmsTypes GetAlgorithm()
        {
            var algorithmEnvironmentVariable = Environment.GetEnvironmentVariable(EnvironmentVariableConstants.AUTHENTICATIONALGORITHM);

            if (string.IsNullOrEmpty(algorithmEnvironmentVariable))
            {
                Console.WriteLine($"Warning: {EnvironmentVariableConstants.AUTHENTICATIONALGORITHM} not configured, using default '{DefaultAuthenticationConfigurationConstants.DefaultAlgorithm}'.");
                return DefaultAuthenticationConfigurationConstants.DefaultAlgorithm;
            }

            var algorithm = (SigningAlgorithmsTypes)Enum.Parse(typeof(SigningAlgorithmsTypes), algorithmEnvironmentVariable);

            if (!Enum.IsDefined(typeof(SigningAlgorithmsTypes), algorithm) | algorithm.ToString().Contains(',', StringComparison.InvariantCulture))
            {
                Console.WriteLine($"Warning: '{algorithmEnvironmentVariable}' is not a valid {EnvironmentVariableConstants.AUTHENTICATIONALGORITHM} configuration option. Valid options are: ");
                foreach (string name in Enum.GetNames(typeof(SigningAlgorithmsTypes)))
                {
                    Console.Write(name);
                    Console.Write(", ");
                }

                throw new ArgumentException($"Invalid {EnvironmentVariableConstants.AUTHENTICATIONALGORITHM} configuration.");
            }

            return algorithm;
        }

        public static AuthenticationTypes GetAuthenticationMethod()
        {
            var authenticationMethodEnvironmentVariable = Environment.GetEnvironmentVariable(EnvironmentVariableConstants.AUTHENTICATIONMETHOD);

            if (string.IsNullOrEmpty(authenticationMethodEnvironmentVariable))
            {
                Console.WriteLine($"Warning: {EnvironmentVariableConstants.AUTHENTICATIONMETHOD} not configured, using default '{DefaultAuthenticationConfigurationConstants.DefaultAuthentication}'.");
                return DefaultAuthenticationConfigurationConstants.DefaultAuthentication;
            }

            var authenticationType = (AuthenticationTypes)Enum.Parse(typeof(AuthenticationTypes), authenticationMethodEnvironmentVariable);

            if (!Enum.IsDefined(typeof(AuthenticationTypes), authenticationType) | authenticationType.ToString().Contains(',', StringComparison.InvariantCulture))
            {
                Console.WriteLine($"Warning: '{authenticationMethodEnvironmentVariable}' is not a valid {EnvironmentVariableConstants.AUTHENTICATIONMETHOD} configuration option. Valid options are: ");
                foreach (string name in Enum.GetNames(typeof(AuthenticationTypes)))
                {
                    Console.Write(name);
                    Console.Write(", ");
                }

                throw new ArgumentException($"Invalid {EnvironmentVariableConstants.AUTHENTICATIONMETHOD} configuration.");
            }

            return authenticationType;
        }

        public static Collection<string> GetAlgorithms()
        {
            var algorithms = new Collection<string>();

            foreach (string name in Enum.GetNames(typeof(SigningAlgorithmsTypes)))
            {
                algorithms.Add(name);
            }

            return algorithms;
        }

        public static Collection<string> GetIssuers()
        {
            var issuerEnvironmentVariable = Environment.GetEnvironmentVariable(EnvironmentVariableConstants.AUTHENTICATIONISSUER);
            var issuers = new Collection<string>();
            if (string.IsNullOrEmpty(issuerEnvironmentVariable))
            {
                Console.WriteLine($"Warning: {EnvironmentVariableConstants.AUTHENTICATIONISSUER} not configured, using default '{DefaultAuthenticationConfigurationConstants.DefaultIssuer}'.");
                issuers.Add(DefaultAuthenticationConfigurationConstants.DefaultIssuer);
            }
            else
            {
                issuers.Add(issuerEnvironmentVariable);
            }

            return issuers;
        }

        public static string GetIssuer()
        {
            var issuerEnvironmentVariable = Environment.GetEnvironmentVariable(EnvironmentVariableConstants.AUTHENTICATIONISSUER);
            if (string.IsNullOrEmpty(issuerEnvironmentVariable))
            {
                Console.WriteLine($"Warning: {EnvironmentVariableConstants.AUTHENTICATIONISSUER} not configured, using default '{DefaultAuthenticationConfigurationConstants.DefaultIssuer}'.");
                return DefaultAuthenticationConfigurationConstants.DefaultIssuer;
            }

            return issuerEnvironmentVariable;
        }

        public static string? GetAuthority()
        {
            var authorityEnvironmentVariable = Environment.GetEnvironmentVariable(EnvironmentVariableConstants.AUTHENTICATIONAUTHORITY);

            return authorityEnvironmentVariable;
        }

        public static string GetSigningKey()
        {
            var signingKeyEnvironmentVariable = Environment.GetEnvironmentVariable(EnvironmentVariableConstants.AUTHENTICATIONSECRET);

            if (string.IsNullOrEmpty(signingKeyEnvironmentVariable))
            {
                Console.WriteLine($"Error: {EnvironmentVariableConstants.AUTHENTICATIONSECRET} not configured..");
                throw new ArgumentException(EnvironmentVariableConstants.AUTHENTICATIONSECRET);
            }

            return signingKeyEnvironmentVariable;
        }

        public static bool ShouldValidateAudience()
        {
            var audienceValidationEnvironmentVariable = Environment.GetEnvironmentVariable(EnvironmentVariableConstants.AUTHENTICATIONVALIDATEAUDIENCE);

            if (string.IsNullOrEmpty(audienceValidationEnvironmentVariable))
            {
                return true;
            }

            bool audienceValidationEnabled;
            if (bool.TryParse(audienceValidationEnvironmentVariable, out audienceValidationEnabled))
            {
                if (!audienceValidationEnabled)
                {
                    Console.WriteLine($"Warning: Audience has been disabled by {EnvironmentVariableConstants.AUTHENTICATIONVALIDATEAUDIENCE} configuration.");
                }

                return audienceValidationEnabled;
            }

            throw new ArgumentException($"Invalid {EnvironmentVariableConstants.AUTHENTICATIONVALIDATEAUDIENCE} configuration.");
        }

        public static bool ShouldValidateIssuer()
        {
            var issuerValidationEnvironmentVariable = Environment.GetEnvironmentVariable(EnvironmentVariableConstants.AUTHENTICATIONVALIDATEISSUER);

            if (string.IsNullOrEmpty(issuerValidationEnvironmentVariable))
            {
                return true;
            }

            bool issuerValidationEnabled;
            if (bool.TryParse(issuerValidationEnvironmentVariable, out issuerValidationEnabled))
            {
                if (!issuerValidationEnabled)
                {
                    Console.WriteLine($"Warning: Issuer has been disabled by {EnvironmentVariableConstants.AUTHENTICATIONVALIDATEISSUER} configuration.");
                }

                return issuerValidationEnabled;
            }

            throw new ArgumentException($"Invalid {EnvironmentVariableConstants.AUTHENTICATIONVALIDATEISSUER} configuration.");
        }

        public static bool ShouldValidateExpiry()
        {
            var expiryValidationEnvironmentVariable = Environment.GetEnvironmentVariable(EnvironmentVariableConstants.AUTHENTICATIONVALIDATEEXPIRY);

            if (string.IsNullOrEmpty(expiryValidationEnvironmentVariable))
            {
                return true;
            }

            bool expiryValidationEnabled;
            if (bool.TryParse(expiryValidationEnvironmentVariable, out expiryValidationEnabled))
            {
                if (!expiryValidationEnabled)
                {
                    Console.WriteLine($"Warning: Expiry has been disabled by {EnvironmentVariableConstants.AUTHENTICATIONVALIDATEEXPIRY} configuration.");
                }

                return expiryValidationEnabled;
            }

            throw new ArgumentException($"Invalid {EnvironmentVariableConstants.AUTHENTICATIONVALIDATEEXPIRY} configuration.");
        }

        public static bool ShouldValidateIssuerSigningKey()
        {
            var issuerSigningKeyValidationEnvironmentVariable = Environment.GetEnvironmentVariable(EnvironmentVariableConstants.AUTHENTICATIONVALIDATEISSUERSIGNINGKEY);

            if (string.IsNullOrEmpty(issuerSigningKeyValidationEnvironmentVariable))
            {
                return true;
            }

            bool issuerSigningKeyValidationEnabled;
            if (bool.TryParse(issuerSigningKeyValidationEnvironmentVariable, out issuerSigningKeyValidationEnabled))
            {
                if (!issuerSigningKeyValidationEnabled)
                {
                    Console.WriteLine($"Warning: Issuer signing key has been disabled by {EnvironmentVariableConstants.AUTHENTICATIONVALIDATEISSUERSIGNINGKEY} configuration.");
                }

                return issuerSigningKeyValidationEnabled;
            }

            throw new ArgumentException($"Invalid {EnvironmentVariableConstants.AUTHENTICATIONVALIDATEISSUERSIGNINGKEY} configuration.");
        }

        public static bool ShouldValidateSignedTokens()
        {
            var signedTokensValidationEnvironmentVariable = Environment.GetEnvironmentVariable(EnvironmentVariableConstants.AUTHENTICATIONVALIDATESIGNEDTOKENS);

            if (string.IsNullOrEmpty(signedTokensValidationEnvironmentVariable))
            {
                return true;
            }

            bool signedTokensValidationEnabled;
            if (bool.TryParse(signedTokensValidationEnvironmentVariable, out signedTokensValidationEnabled))
            {
                if (!signedTokensValidationEnabled)
                {
                    Console.WriteLine($"Warning: Signed tokens has been disabled by {EnvironmentVariableConstants.AUTHENTICATIONVALIDATESIGNEDTOKENS} configuration.");
                }

                return signedTokensValidationEnabled;
            }

            throw new ArgumentException($"Invalid {EnvironmentVariableConstants.AUTHENTICATIONVALIDATESIGNEDTOKENS} configuration.");
        }
    }
}