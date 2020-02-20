using System;
using System.Linq;

namespace Core.Utils
{
    public static class VoucherCodeGenerator
    {
        public static string GenerateVoucher(string alphabet, int lengthOfVoucher)
        {
            var keys = alphabet.ToCharArray();
            var random = new Random();
            return Enumerable
                .Range(1, lengthOfVoucher) // for(i.. ) 
                .Select(k => keys[random.Next(0, keys.Length - 1)])  // generate a new random char 
                .Aggregate("", (e, c) => e + c).ToUpperInvariant(); // join into a string
        }
        /// <summary>
        /// Creates voucher code with the specified length.
        /// Default alphabet: "ABCDEFGHIJKLMNOPRSTUVYZ1234567890"
        /// </summary>
        /// <param name="lengthOfVoucher"></param>
        /// <returns></returns>
        public static string GenerateVoucher(int lengthOfVoucher)
        {
            return GenerateVoucher("ABCDEFGHIJKLMNOPRSTUVYZ1234567890", lengthOfVoucher);
        }
    }
}
