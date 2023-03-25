namespace EngineBay.Authentication
{
    internal static class LoggerExtensions
    {
        private static readonly Action<ILogger, Exception?> UserDoesNotExistValue = LoggerMessage.Define(
            logLevel: LogLevel.Warning,
            eventId: 1,
            formatString: "User does not exist.");

        public static void UserDoesNotExist(this ILogger logger)
        {
            UserDoesNotExistValue(logger, null);
        }
    }
}