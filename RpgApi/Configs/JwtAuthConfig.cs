namespace Rpg.Configs
{
    public class JwtAuthConfig
    {
        public string Issuer { get; private set; } = string.Empty;
        public string Audience { get; private set; } = string.Empty;
        public string Key { get; private set; } = string.Empty;
        public int DurDays { get; private set; } = 0;
        public int DurHours { get; private set; } = 0;
        public int DurMins { get; private set; } = 0;
        public int DurSecs { get; private set; } = 0;
        public string AesKey { get; private set; } = string.Empty;
        public string AesIv { get; private set; } = string.Empty;

        public TimeSpan Duration
        {
            get
            {
                var dur = new TimeSpan(days: DurDays, hours: DurHours, minutes: DurMins, seconds: DurSecs);
                if (dur.TotalSeconds == 0)
                    return TimeSpan.MaxValue;
                else
                    return dur;
            }
        }

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
