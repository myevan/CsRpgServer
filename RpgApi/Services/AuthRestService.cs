namespace Rpg.Services
{
    public class AuthRestService
    {
        public AuthRestService(AuthService authSvc, JwtService jwtSvc)
        {
            _authSvc = authSvc;
            _jwtSvc= jwtSvc;
        }

        public async Task<IResult> PostAuthAsync(string name)
        {
            var ses = await _authSvc.SignUpAsync(name);
            var tkn = _jwtSvc.DumpSession(ses);
            return Results.Text(tkn);
        }

        public IResult GetAuth(HttpContext httpCtx)
        {
            var ses = _jwtSvc.LoadSession(httpCtx);
            if (ses == null) return Results.NotFound();
            return Results.Ok(ses.ToDict());
        }

        private readonly AuthService _authSvc;
        private readonly JwtService _jwtSvc;
    }
}
