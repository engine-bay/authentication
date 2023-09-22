namespace EngineBay.Authentication
{
    internal static class LoggerExtensions
    {
        private static readonly Action<ILogger, Exception?> UserDoesNotExistValue = LoggerMessage.Define(
            logLevel: LogLevel.Warning,
            eventId: 1,
            formatString: "User does not exist.");

        private static readonly Action<ILogger, Exception?> UserDoesNotHaveBasicAuthCredentialsValue = LoggerMessage.Define(
            logLevel: LogLevel.Warning,
            eventId: 2,
            formatString: "Basic auth credentials do not exist for user.");

        private static readonly Action<ILogger, Exception?> RegisteredNewUserValue = LoggerMessage.Define(
            logLevel: LogLevel.Information,
            eventId: 3,
            formatString: "New user registered.");

        private static readonly Action<ILogger, string, Exception?> NamedUserDoesNotExistValue = LoggerMessage.Define<string>(
            logLevel: LogLevel.Warning,
            eventId: 4,
            formatString: "User with username '{Username}' does not exist.");

        public static void UserDoesNotExist(this ILogger logger)
        {
            UserDoesNotExistValue(logger, null);
        }

        public static void UserDoesNotExist(this ILogger logger, string username)
        {
            NamedUserDoesNotExistValue(logger, username, null);
        }

        public static void RegisteredNewUser(this ILogger logger)
        {
            RegisteredNewUserValue(logger, null);
        }

        public static void UserDoesNotHaveBasicAuthCredentials(this ILogger logger)
        {
            UserDoesNotHaveBasicAuthCredentialsValue(logger, null);
        }
    }
}