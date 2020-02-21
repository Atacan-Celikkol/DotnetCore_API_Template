using System;

namespace Data.Models.Mapping.Resolvers
{
    public static class QRCodeResolver
    {
        public static string ToQRCodeUrl(this string data)
        {
            return $"https://chart.googleapis.com/chart?cht=qr&chs=500x500&chl={Uri.EscapeUriString(data)}";
        }
    }
}