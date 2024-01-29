namespace EngineBay.Authentication.Cookies
{
    public static class CookieAuthEndpoints
    {
        public static void MapEndpoints(RouteGroupBuilder endpoints)
        {
            endpoints.MapPost(
                    "/signIn",
                    async (
                        SignIn command,
                        SignInCredentials signInCredentials,
                        CancellationToken cancellation) =>
                    {
                        var result = await command.Handle(signInCredentials, cancellation);
                        return Results.Ok(result);
                    })
                .AllowAnonymous();
        }
    }
}