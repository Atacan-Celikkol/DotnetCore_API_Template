using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Extensions
{
    public static class StringExtensions
    {
        public static bool ValidateIban(this string iban)
        {
            iban = iban.ToUpper().Replace(" ", string.Empty); //IN ORDER TO COPE WITH THE REGEX BELOW
            if (string.IsNullOrEmpty(iban))
                return false;
            if (Regex.IsMatch(iban, "^[A-Z0-9]"))
            {
                iban = iban.Replace(" ", String.Empty);
                var bank =
                    iban.Substring(4, iban.Length - 4) + iban.Substring(0, 4);
                var asciiShift = 55;
                StringBuilder sb = new StringBuilder();
                foreach (char c in bank)
                {
                    int v;
                    if (Char.IsLetter(c)) v = c - asciiShift;
                    else v = int.Parse(c.ToString());
                    sb.Append(v);
                }
                string checkSumString = sb.ToString();
                int checksum = int.Parse(checkSumString.Substring(0, 1));
                for (int i = 1; i < checkSumString.Length; i++)
                {
                    int v = int.Parse(checkSumString.Substring(i, 1));
                    checksum *= 10;
                    checksum += v;
                    checksum %= 97;
                }
                return checksum == 1;
            }
            return false;
        }

        public static string ToUrlFriendlyString(this string str)
        {
            str = str.ToNonTurkishString();
            //based on http://stackoverflow.com/a/780800/811405
            var normalizedString = str.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(str.Length);

            foreach (var c in normalizedString)
            {
                switch (CharUnicodeInfo.GetUnicodeCategory(c))
                {
                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.UppercaseLetter:
                    case UnicodeCategory.DecimalDigitNumber:
                        if (c < 128)
                            stringBuilder.Append(c);
                        else
                            stringBuilder.Append(c.RemapInternationalCharToAscii());
                        break;
                    case UnicodeCategory.SpaceSeparator:
                    case UnicodeCategory.ConnectorPunctuation:
                    case UnicodeCategory.DashPunctuation:
                        stringBuilder.Append('-');
                        break;
                }
            }
            var result = stringBuilder.ToString().ToLowerInvariant();
            result = string.Join("-", result.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries)); // remove duplicate underscores
            if (result.Length > 60)
            {
                result = result.Substring(0, 60);
            }
            return result;
        }

        public static string RemoveHtmlTags(this string str)
        {
            return HtmlRemoval.StripTagsRegex(str);
        }

        public static string Take(this string str, int length)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            length = str.Length > length ? length : str.Length;
            return str.Substring(0, length);
        }

        public static string ToNonTurkishString(this string str)
        {
            return string.Join("", str.Normalize(NormalizationForm.FormD)
                .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
        }

        public static string RemapInternationalCharToAscii(this char c)
        {
            string s = c.ToString().ToLowerInvariant();
            if ("àåáâäãåą".Contains(s))
            {
                return "a";
            }
            if ("èéêëę".Contains(s))
            {
                return "e";
            }
            if ("ìíîïı".Contains(s))
            {
                return "i";
            }
            if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }
            if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }
            if ("çćčĉ".Contains(s))
            {
                return "c";
            }
            if ("żźž".Contains(s))
            {
                return "z";
            }
            if ("śşšŝ".Contains(s))
            {
                return "s";
            }
            if ("ñń".Contains(s))
            {
                return "n";
            }
            if ("ýÿ".Contains(s))
            {
                return "y";
            }
            if ("ğĝ".Contains(s))
            {
                return "g";
            }
            if (c == 'ř')
            {
                return "r";
            }
            if (c == 'ł')
            {
                return "l";
            }
            if (c == 'đ')
            {
                return "d";
            }
            if (c == 'ß')
            {
                return "ss";
            }
            if (c == 'þ')
            {
                return "th";
            }
            if (c == 'ĥ')
            {
                return "h";
            }
            if (c == 'ĵ')
            {
                return "j";
            }
            return string.Empty;
        }

        #region Internal Classes
        internal static class HtmlRemoval
        {
            /// <summary>
            /// Remove HTML from string with Regex.
            /// </summary>
            public static string StripTagsRegex(string source)
            {
                return string.IsNullOrEmpty(source) ? source : Regex.Replace(source, "<.*?>", string.Empty);
            }

            /// <summary>
            /// Compiled regular expression for performance.
            /// </summary>
            private static readonly Regex HtmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

            /// <summary>
            /// Remove HTML from string with compiled Regex.
            /// </summary>
            public static string StripTagsRegexCompiled(string source)
            {
                return HtmlRegex.Replace(source, string.Empty);
            }

            /// <summary>
            /// Remove HTML tags from string using char array.
            /// </summary>
            public static string StripTagsCharArray(string source)
            {
                var array = new char[source.Length];
                var arrayIndex = 0;
                var inside = false;

                foreach (var @let in source)
                {
                    switch (@let)
                    {
                        case '<':
                            inside = true;
                            continue;
                        case '>':
                            inside = false;
                            continue;
                    }
                    if (!inside)
                    {
                        array[arrayIndex] = @let;
                        arrayIndex++;
                    }
                }
                return new string(array, 0, arrayIndex);
            }
        }
        #endregion

    }
}