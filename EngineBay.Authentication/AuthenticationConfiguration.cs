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
    }
}