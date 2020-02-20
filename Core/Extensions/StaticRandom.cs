using System;

namespace Core.Extensions
{
    public static class SuperRandom
    {
        private static readonly Random Global = new Random();
        [ThreadStatic]
        private static Random _local;
        public static int Next()
        {
            var inst = _local;
            if (inst != null)
            {
                return inst.Next();
            }
            int seed;
            lock (Global) seed = Global.Next();
            _local = inst = new Random(seed);
            return inst.Next();
        }
    }
}
