namespace Rpg.Configs
{
    public class JwtAuthConfig
    {
        public string Issuer { get; private set; } = string.Empty;
        public string Audience { get; private set; } = string.Empty;
        public string Key { get; private set; } = string.Empty;
        public string AesKey { get; private set; } = string.Empty;
        public string AesIv { get; private set; } = string.Empty;

        public byte[] AesKeyBytes
        {
            get
            {
                if (_aesKeyBytes == null)
                {
                    _aesKeyBytes = Convert.FromBase64String(AesKey);
                }
                return _aesKeyBytes;
            }
        }

        public byte[] AesIvBytes
        {
            get
            {
                if (_aesIvBytes == null)
                {
                    _aesIvBytes = Convert.FromBase64String(AesIv);
                }
                return _aesIvBytes;
            }
        }

        private byte[]? _aesKeyBytes;
        private byte[]? _aesIvBytes;
    }
}
