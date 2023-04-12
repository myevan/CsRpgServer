namespace Rpg.Configs
{
    public class JwtConfig
    {
        public string Issuer { get; private set; } = string.Empty;
        public string Audience { get; private set; } = string.Empty;
        public string Key { get; private set; } = string.Empty;
    }
}
