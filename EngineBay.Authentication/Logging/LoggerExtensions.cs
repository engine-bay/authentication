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

        public static void UserDoesNotExist(this ILogger logger)
        {
            UserDoesNotExistValue(logger, null);
        }

        public static void UserDoesNotHaveBasicAuthCredentials(this ILogger logger)
        {
            UserDoesNotHaveBasicAuthCredentialsValue(logger, null);
        }
    }
}