using System;

namespace Dmarc.Common.Util
{
    public interface IClock
    {
        DateTime GetDateTimeUtc();
    }

    public class Clock : IClock
    {
        public DateTime GetDateTimeUtc()
        {
            return DateTime.UtcNow;
        }
    }
}
