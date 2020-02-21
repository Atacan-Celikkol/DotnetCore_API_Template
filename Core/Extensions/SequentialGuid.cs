using System;

namespace Core.Extensions
{
    public static class SequentialGuid
    {
        private static int _sequenced = (int)DateTime.UtcNow.Ticks;

        private static readonly System.Security.Cryptography.RNGCryptoServiceProvider Random =
        new System.Security.Cryptography.RNGCryptoServiceProvider();

        private static readonly byte[] Buffer = new byte[6];

        public static Guid NewGuid()
        {
            var ticks = DateTime.UtcNow.Ticks;
            var sequenceNum = System.Threading.Interlocked.Increment(ref _sequenced);
            lock (Buffer)
            {
                Random.GetBytes(Buffer);
                return new Guid(
                (int)(ticks >> 32), (short)(ticks >> 16), (short)ticks,
                (byte)(sequenceNum >> 8), (byte)sequenceNum,
                Buffer[0], Buffer[1], Buffer[2], Buffer[3], Buffer[4], Buffer[5]
                );
            }
        }
    }
}