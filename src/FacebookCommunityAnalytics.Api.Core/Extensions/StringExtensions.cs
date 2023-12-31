﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace FacebookCommunityAnalytics.Api.Core.Extensions
{
    public static class StringExtensions
    {
        public static readonly HashSet<char> Base64Characters = new HashSet<char>
        {
            'A',
            'B',
            'C',
            'D',
            'E',
            'F',
            'G',
            'H',
            'I',
            'J',
            'K',
            'L',
            'M',
            'N',
            'O',
            'P',
            'Q',
            'R',
            'S',
            'T',
            'U',
            'V',
            'W',
            'X',
            'Y',
            'Z',
            'a',
            'b',
            'c',
            'd',
            'e',
            'f',
            'g',
            'h',
            'i',
            'j',
            'k',
            'l',
            'm',
            'n',
            'o',
            'p',
            'q',
            'r',
            's',
            't',
            'u',
            'v',
            'w',
            'x',
            'y',
            'z',
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9',
            '+',
            '/',
            '='
        };

        public const string ReebonzEmailAddress = @"^([\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+\.)*[\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+@reebonz\.(([a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$";

        public const string NormalEmailAddress =
            @"^([\w\!\#$\%\&\'\*\-\/\=\?\^\`{\|\}\~]+\.)*[\w\!\#$\%\&\'\*\-\/\=\?\^\`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$";

        public static bool IsBase64String(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            if (value.Any(c => !Base64Characters.Contains(c)))
            {
                return false;
            }

            try
            {
                Convert.FromBase64String(value);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Computes the Damerau-Levenshtein Distance between two strings, represented as arrays of
        /// integers, where each integer represents the code point of a character in the source string.
        /// Includes an optional threshhold which can be used to indicate the maximum allowable distance.
        /// </summary>
        /// <param name="source">An array of the code points of the first string</param>
        /// <param name="target">An array of the code points of the second string</param>
        /// <param name="threshold">Maximum allowable distance</param>
        /// <returns>Int.MaxValue if threshhold exceeded; otherwise the Damerau-Leveshteim distance between the strings</returns>
        public static int LevenshteinDistance(this string source, string target, int threshold = int.MaxValue)
        {
            if (source == null || target == null)
                return int.MaxValue;

            var length1 = source.Length;
            var length2 = target.Length;

            // Return trivial case - difference in string lengths exceeds threshhold
            if (Math.Abs(length1 - length2) > threshold)
            {
                return int.MaxValue;
            }

            // Ensure arrays [i] / length1 use shorter length 
            if (length1 > length2)
            {
                Swap(ref target, ref source);
                Swap(ref length1, ref length2);
            }

            var maxi = length1;
            var maxj = length2;

            var dCurrent = new int[maxi + 1];
            var dMinus1 = new int[maxi + 1];
            var dMinus2 = new int[maxi + 1];
            int[] dSwap;

            for (int i = 0; i <= maxi; i++)
            {
                dCurrent[i] = i;
            }

            int jm1 = 0, im1 = 0, im2 = -1;

            for (int j = 1; j <= maxj; j++)
            {
                // Rotate
                dSwap = dMinus2;
                dMinus2 = dMinus1;
                dMinus1 = dCurrent;
                dCurrent = dSwap;

                // Initialize
                var minDistance = int.MaxValue;
                dCurrent[0] = j;
                im1 = 0;
                im2 = -1;

                for (int i = 1; i <= maxi; i++)
                {

                    var cost = source[im1] == target[jm1] ? 0 : 1;

                    var del = dCurrent[im1] + 1;
                    var ins = dMinus1[i] + 1;
                    var sub = dMinus1[im1] + cost;

                    //Fastest execution for min value of 3 integers
                    var min = (del > ins) ? (ins > sub ? sub : ins) : (del > sub ? sub : del);

                    if (i > 1 && j > 1 && source[im2] == target[jm1] && source[im1] == target[j - 2])
                        min = Math.Min(min, dMinus2[im2] + cost);

                    dCurrent[i] = min;
                    if (min < minDistance)
                    {
                        minDistance = min;
                    }

                    im1++;
                    im2++;
                }

                jm1++;
                if (minDistance > threshold)
                {
                    return int.MaxValue;
                }
            }

            var result = dCurrent[maxi];
            return (result > threshold) ? int.MaxValue : result;
        }

        private static void Swap<T>(ref T arg1, ref T arg2)
        {
            T temp = arg1;
            arg1 = arg2;
            arg2 = temp;
        }

        public static string[] SplitAtCharacters(this string source, int characters, char delimiter)
        {
            if (source == null || source.Length <= characters)
                return new string[] {source};

            var result = new List<string>();
            var chars = source.ToCharArray();
            var start = 0;
            var end = characters;

            while (true)
            {
                while (chars.Length >= start + end && chars[start + end] != delimiter)
                    end++;

                if (start + end >= source.Length)
                {
                    if (start < source.Length)
                        result.Add(source.Substring(start));

                    break;
                }

                result.Add(source.Substring(start, end));

                start += end;
                end = characters;
            }

            return result.ToArray();
        }

        public static string Replace(this string source, string oldValue, string newValue, StringComparison comparisonType)
        {
            if (source.Length == 0 || oldValue.Length == 0)
                return source;

            var result = new System.Text.StringBuilder();
            int startingPos = 0;
            int nextMatch;
            while ((nextMatch = source.IndexOf(oldValue, startingPos, comparisonType)) > -1)
            {
                result.Append(source, startingPos, nextMatch - startingPos);
                result.Append(newValue);
                startingPos = nextMatch + oldValue.Length;
            }

            result.Append(source, startingPos, source.Length - startingPos);

            return result.ToString();
        }

        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static T? EnumParse<T>(this string input) where T : struct
        {
            if (Enum.TryParse(input, true, out T result))
                return result;

            return null;
        }

        public static string SplitAndInsert(this string input, char separator, string textToInsert, int index)
        {
            var parts = input.Split(separator).ToList();
            parts.Insert(index + 1, textToInsert);
            return string.Join(separator.ToString(), parts);
        }

        //public static string HashHMACBase64(this string message, string keyHex)
        //{
        //    var hash = HexDecode(keyHex).HashHMAC(UTF8StringEncode(message));
        //    return Convert.ToBase64String(hash);
        //}

        //public static string HashHMACHex(this string message, string keyHex)
        //{
        //    var hash = HexDecode(keyHex).HashHMAC(UTF8StringEncode(message));
        //    return hash.HashEncode();
        //}

        public static string MaybeSubstring(this string message, int length, bool reverse = false)
        {
            if (message.Length <= length)
                return message;

            var m = reverse ? message.Reverse() : message;
            var result = m.Substring(0, length);

            return reverse ? result.Reverse() : result;
        }

        public static byte[] ASCIIStringEncode(this string text)
        {
            var encoding = new ASCIIEncoding();
            return encoding.GetBytes(text);
        }

        public static byte[] UTF8StringEncode(this string text)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetBytes(text);
        }

        public static byte[] HexDecode(this string hex)
        {
            var bytes = new byte[hex.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber);
            }

            return bytes;
        }

        //public static byte[] Compress(this string str)
        //{
        //    var bytes = Encoding.UTF8.GetBytes(str);

        //    using (var msi = new MemoryStream(bytes))
        //    using (var mso = new MemoryStream())
        //    {
        //        using (var gs = new GZipStream(mso, CompressionMode.Compress))
        //        {
        //            CopyTo(msi, gs);
        //        }

        //        return mso.ToArray();
        //    }
        //}

        //public static string Decompress(this byte[] bytes)
        //{
        //    using (var msi = new MemoryStream(bytes))
        //    using (var mso = new MemoryStream())
        //    {
        //        using (var gs = new GZipStream(msi, CompressionMode.Decompress))
        //        {
        //            CopyTo(gs, mso);
        //        }

        //        return Encoding.UTF8.GetString(mso.ToArray());
        //    }
        //}

        public static string LowercaseFirstChar(this string text)
        {
            if (!string.IsNullOrEmpty(text) && char.IsUpper(text[0]))
                return char.ToLower(text[0]) + text.Substring(1);

            return text;
        }

        public static string UppercaseFirstChar(this string text)
        {
            if (!string.IsNullOrEmpty(text) && char.IsLower(text[0]))
                return char.ToUpper(text[0]) + text.Substring(1);

            return text;
        }

        public static string Normalise(this string text)
        {
            return RemoveDiacritics(text).Trim().ToLowerInvariant();
        }

        public static int ToIntOrDefault(this string s, int defaultValue = 0)
        {
            if (!int.TryParse(s, out var result))
                return defaultValue;

            return result;
        }

        public static double ToDoubleOrDefault(this string s, double defaultValue = 0d)
        {
            if (!double.TryParse(s, out var result))
                return defaultValue;

            return result;
        }

        public static decimal ToDecimalOrDefault(this string s, decimal defaultValue = 0m)
        {
            if (!decimal.TryParse(s, out var result))
                return defaultValue;

            return result;
        }

        public static int? ToNullableInt(this string s)
        {
            if (!int.TryParse(s, out var result))
                return null;

            return result;
        }

        public static string ToLiteral(this string input)
        {
            var literal = new StringBuilder(input.Length + 2);
            literal.Append("\"");
            foreach (var c in input)
            {
                switch (c)
                {
                    case '\'':
                        literal.Append(@"\'");
                        break;
                    case '\"':
                        literal.Append("\\\"");
                        break;
                    case '\\':
                        literal.Append(@"\\");
                        break;
                    case '\0':
                        literal.Append(@"\0");
                        break;
                    case '\a':
                        literal.Append(@"\a");
                        break;
                    case '\b':
                        literal.Append(@"\b");
                        break;
                    case '\f':
                        literal.Append(@"\f");
                        break;
                    case '\n':
                        literal.Append(@"\n");
                        break;
                    case '\r':
                        literal.Append(@"\r");
                        break;
                    case '\t':
                        literal.Append(@"\t");
                        break;
                    case '\v':
                        literal.Append(@"\v");
                        break;
                    default:
                        if (Char.GetUnicodeCategory(c) != UnicodeCategory.Control)
                        {
                            literal.Append(c);
                        }
                        else
                        {
                            literal.Append(@"\u");
                            literal.Append(((ushort) c).ToString("x4"));
                        }

                        break;
                }
            }

            literal.Append("\"");
            return literal.ToString();
        }

        public static Dictionary<string, string> ParseQueryString(this string query, bool decode = true)
        {
            var result = new Dictionary<string, string>();

            foreach (string token in query.TrimStart(new char[] {'?'}).Split(new char[] {'&'}, StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = token.Split(new char[] {'='}, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2)
                    result[parts[0].Trim()] = decode ? HttpUtility.UrlDecode(parts[1]).Trim() : parts[1].Trim();
                else
                    result[parts[0].Trim()] = "";
            }

            return result;
        }

        /// <summary>Checks whether a string can be converted to an integer.</summary>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false.</returns>
        /// <param name="value">The string value to test.</param>
        public static bool IsInt(this string value)
        {
            int result;
            return int.TryParse(value, out result);
        }

        public static bool IsAsciiCharacters(this string value)
        {
            string sOut = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(value));
            return sOut == value;
        }

        public static bool StartsWithAny(this string value, params string[] tests)
        {
            return tests.Select(t => t.ToLowerInvariant()).Any(value.ToLowerInvariant().StartsWith);
        }

        public static string GetUrlPath(this string s)
        {
            s = s.Trim();

            if (s.StartsWith("http"))
            {
                return new Uri(s).AbsolutePath;
            }

            s = s.StartsWith("/") ? s : "/{0}".FormatWith(s);
            var index = s.IndexOf("?", StringComparison.InvariantCultureIgnoreCase);
            return s.Substring(0, index == -1 ? s.Length : index).ToLowerInvariant();
        }

        public static string TrimTo(this string s, int count, string ending = "...")
        {
            return s == null ? string.Empty : s.Length > count ? s.Substring(0, count) + ending : s;
        }

        public static string FirstXAsciiCharacters(this string s, int count)
        {
            return new string(s.ToLowerInvariant().Where(IsAsciiAz).Take(count).Select(s1 => s1).ToArray());
        }

        public static bool InvariantCultureIgnoreCaseCompare(this string obj, string compareTo)
        {
            return string.Equals(obj?.Trim(), compareTo?.Trim(), StringComparison.InvariantCultureIgnoreCase);
        }

        public static T ChangeType<T>(this string obj)
        {
            return (T) Convert.ChangeType(obj, typeof(T));
        }

        public static string StripLineBreaks(this string s, string replace = "")
        {
            return s?.Replace("\r\n", replace).Replace("\r", replace).Replace("\n", replace).Trim();
        }

        public static string GetQueueName(this string destination)
        {
            var parts = destination.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            return parts[parts.Length - 1];
        }

        public static Dictionary<string, T> ToDictionary<T>(this string s, Func<string, T> valueAction)
        {
            if (s.IsNullOrEmpty())
                return new Dictionary<string, T>();

            return s.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(entry => entry.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries))
                .ToDictionary(kvp => kvp[0], kvp => valueAction(kvp.Length > 1 ? kvp[1] : null));
        }

        public static string GetNumbers(this string s, int? count = null)
        {
            var result = s.AsEnumerable().Where(c => char.IsDigit(c) || c == ' ' || c == '-');
            return new String(count.HasValue && count.Value < result.Count() ? result.Take(count.Value).ToArray() : result.ToArray());
        }

        public static bool IsNotNullOrWhiteSpace(this string s)
        {
            return s != null && s.Trim() != string.Empty;
        }

        public static bool HasValue(this string s)
        {
            return s != null && s.Trim() != string.Empty;
        }

        public static bool HasNoValue(this string s)
        {
            return !HasValue(s);
        }

        public static bool IsSameAs(this string source, string dest)
        {
            return source.InvariantCultureIgnoreCaseCompare(dest) || (source.IsNullOrEmpty() && dest.IsNullOrEmpty());
        }

        public static string RavenEscape(this string s)
        {
            return s.Replace(":", "");
        }

        public static string FormatWith(this string s, params object[] args)
        {
            return string.Format(s, args);
        }

        public static string TryFormatWith(this string s, string failureValue, params object[] args)
        {
            try
            {
                return string.Format(s, args);
            }
            catch
            {
                return failureValue;
            }
        }

        public static string Reverse(this string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static string LastElement(this string input, char seperator)
        {
            if (input == null)
                return string.Empty;

            string[] elements = input.Split(new[] {seperator}, StringSplitOptions.RemoveEmptyEntries);
            return elements[elements.Length - 1];
        }

        public static string Base64Encode(this string input)
        {
            if (input.IsNullOrEmpty())
                return string.Empty;

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }

        public static string Base64Decode(this string input)
        {
            if (input.IsNullOrEmpty())
                return string.Empty;

            return Encoding.UTF8.GetString(Convert.FromBase64String(input));
        }

        //public static string MergeQueryStrings(this string query1, string query2)
        //{
        //    var nvc1 = HttpUtility.ParseQueryString(query1);
        //    var nvc2 = HttpUtility.ParseQueryString(query2);
        //    return nvc1.MergeWith(nvc2).ToString();
        //}

        public static string ReplaceFirstOccurrance(this string original, string oldValue, string newValue)
        {
            if (String.IsNullOrEmpty(original))
                return String.Empty;
            if (String.IsNullOrEmpty(oldValue))
                return original;
            if (String.IsNullOrEmpty(newValue))
                newValue = String.Empty;

            int loc = original.IndexOf(oldValue, StringComparison.InvariantCultureIgnoreCase);
            return loc >= 0 ? original.Remove(loc, oldValue.Length).Insert(loc, newValue) : original;
        }

        public static string ReplaceLastOccurrance(this string original, string oldValue, string newValue)
        {
            if (String.IsNullOrEmpty(original))
                return String.Empty;
            if (String.IsNullOrEmpty(oldValue))
                return original;
            if (String.IsNullOrEmpty(newValue))
                newValue = String.Empty;

            int loc = original.LastIndexOf(oldValue, StringComparison.InvariantCultureIgnoreCase);
            return loc >= 0 ? original.Remove(loc, oldValue.Length).Insert(loc, newValue) : original;
        }

        public static string Hash(this string input)
        {
            // step 1, calculate MD5 hash from input
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hash = md5.ComputeHash(inputBytes);

                // step 2, convert byte array to hex string
                var sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }

        public static string Quantity(this string singularForm, int qty, string irregularPluralForm = null)
        {
            return qty + " " + Pluralise(singularForm, qty, irregularPluralForm);
        }

        public static string Pluralise(this string singularForm, int qty, string irregularPluralForm = null)
        {
            if (qty == 1)
                return singularForm;

            return irregularPluralForm ?? (singularForm + "s");
        }

        /// <summary>
        /// Remove all characters except 0-9 and a-z, replace spaces with - and remove double spaces at the end
        /// </summary>
        /// <param name="value"></param>
        /// <param name="spaceReplacement"></param>
        /// <param name="allowPeriod"></param>
        /// <returns></returns>
        public static string SanitizeForUrl(this string value, string spaceReplacement = "-", bool allowPeriod = false)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            var builder = new StringBuilder();

            char[] originalChars = value.UnicodeToAscii().Trim().ToLowerInvariant().ToCharArray();

            foreach (char t in originalChars)
            {
                if (AcceptableUrlChar(t)) //only allow a-z and 0-9
                    builder.Append(t);
                else if (t == ' ') //replace spaces with hyphen
                    builder.Append(spaceReplacement);
                else if (t == '.' && allowPeriod)
                    builder.Append(".");
            }

            return builder.ToString().Replace("--", "-");
        }

        /// <summary>
        /// Remove all characters for language detection
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SanitizeForTranslation(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            var builder = new StringBuilder();

            char[] originalChars = value.Trim().ToLowerInvariant().ToCharArray();

            foreach (char t in originalChars)
            {
                if (t > 192)
                {
                    builder.Append(t);
                }
            }

            return builder.ToString();
        }

        public static string RemoveDiacritics(this string text)
        {
            //http://stackoverflow.com/a/249126/3856
            //note this only removes diacritics it doesn't ascii-ise
            //characters that are genuine single characters (e.g. ß, æ)
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    var ct = c;
                    if (c == 'Đ')
                    {
                        ct = 'D';
                    }

                    if (c == 'đ')
                    {
                        ct = 'd';
                    }

                    stringBuilder.Append(ct);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string UnicodeToAscii(this string text)
        {
            //http://stackoverflow.com/a/2086575/3856
            //note that ß -> ? with this
            byte[] tempBytes = Encoding.GetEncoding("ISO-8859-8").GetBytes(text);
            string asciiStr = Encoding.UTF8.GetString(tempBytes);
            return asciiStr;
        }

        /// <summary>
        /// Only returns true if character is a-z or 0-9
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        private static bool AcceptableUrlChar(char character)
        {
            return ((character > 96 && character < 123) || (character > 47 && character < 58) || character == '-' || (character > 191 && character < 383));
        }

        /// <summary>
        /// Only returns true if character is a-z
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        private static bool IsAsciiAz(char character)
        {
            return (character > 96 && character < 123);
        }

        /// <summary>
        /// Remove illegal XML characters from a string. For more info see this http://stackoverflow.com/a/12469826/52360
        /// </summary>
        public static string SanitizeForXml(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            var buffer = new StringBuilder(value.Length);

            foreach (char c in value)
            {
                if (IsLegalXmlChar(c))
                {
                    buffer.Append(c);
                }
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Whether a given character is allowed by XML 1.0.
        /// </summary>
        public static bool IsLegalXmlChar(int character)
        {
            return
            (
                character == 0x9 /* == '\t' == 9   */
                || character == 0xA /* == '\n' == 10  */
                || character == 0xD /* == '\r' == 13  */
                || (character >= 0x20 && character <= 0xD7FF)
                || (character >= 0xE000 && character <= 0xFFFD)
                || (character >= 0x10000 && character <= 0x10FFFF)
            );
        }

        public static string CleanPhoneNumber(this string s)
        {
            return s.IsNullOrEmpty() ? null : s.Trim().TrimStart('0');
        }

        public static string Limit(this string s, int max, string ending = "...")
        {
            return s.Length > max ? "{0}{1}".FormatWith(s.Substring(0, max), ending) : s;
        }

        public static string ToPascalCase(this string s, char delimiter = ' ', bool keepDelimiter = true)
        {
            if (s == null) return null;
            if (s.Length < 2) return s.ToUpper();

            var words = s.Split(delimiter);

            var result = string.Join(keepDelimiter ? delimiter.ToString() : string.Empty, words.Select(word => word.FirstLetterToUpper()).ToArray());
            return result;
        }

        public static string PutSpacesInPascalCase(this string s, bool firstLetterToUpper = false, bool includeNumbers = false)
        {
            var result = SpacesInPascalCaseRegex.Replace(s, m => string.Format("{0} {1}", m.Groups["notcaps"].Value, m.Groups["caps"].Value));

            if (includeNumbers)
            {
                result = SpacesInPascalCaseWithNumbersRegex.Replace(result, m => string.Format("{0} {1}", m.Groups["notcaps"].Value, m.Groups["caps"].Value));
            }

            return firstLetterToUpper ? result.FirstLetterToUpper() : result;
        }

        private static readonly Regex SpacesInPascalCaseRegex = new Regex("(?'notcaps'[^A-Z^ ])(?'caps'[A-Z])", RegexOptions.Compiled);
        private static readonly Regex SpacesInPascalCaseWithNumbersRegex = new Regex("(?'notcaps'[^0-9^ ])(?'caps'[0-9])", RegexOptions.Compiled);

        public static string WordsBeforeCharacterX(this string value, int index, string continuationString = null, int graceRegion = 0)
        {
            var split = value.SplitWordsAtCharacterX(index, graceRegion);

            return split.Item1 + (split.Item2.IsNullOrEmpty() ? "" : continuationString);
        }

        public static string FirstLetterToUpper(this string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        public static Tuple<string, string> SplitWordsAtCharacterX(this string value, int index, int graceRegion = 0)
        {
            if (value == null) return new Tuple<string, string>(null, null);

            if (value.Length - index < graceRegion)
                return Tuple.Create(value, (string) null);

            var words = value.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            int cumu = 0;
            var boundary = words.Where(w => (cumu += w.Length + 1) <= index + 1).Aggregate(0, (i, word) => i + 1);

            var before = words.Take(boundary).StringConcat(" ");
            var after = words.Skip(boundary).StringConcat(" ");

            return Tuple.Create(before.TrimEnd(), after.TrimEnd());
        }

        public static string SafeReplace(this string value, string oldValue, string newValue)
        {
            if (value == null)
                return null;

            return value.Replace(oldValue, newValue);
        }

        public static string SafeGetFileName(this string fileName)
        {
            int length;
            if ((length = fileName.LastIndexOf('.')) == -1)
                return fileName;
            return StripQuerystring(fileName.Substring(0, length));
        }

        public static string SafeGetFileExtension(this string fileName)
        {
            return GetFileExtension(fileName) ?? fileName;
        }

        public static string TrimSafe(this string value)
        {
            return value.IsNotNullOrEmpty() ? value.Trim() : string.Empty;
        }

        public static string GetFileExtension(this string fileName)
        {
            int length;
            if ((length = fileName.LastIndexOf('.')) == -1)
                return null;

            return StripQuerystring(fileName.Substring(length, fileName.Length - length));
        }

        public static string StripQuerystring(this string input)
        {
            return input.Substring(0, input.IndexOf('?') != -1 ? input.IndexOf('?') : input.Length);
        }

        public static List<string> Wrap(this string s, int maxLength)
        {
            if (s.Length == 0) return new List<string>();

            var words = s.Split(' ');
            var lines = new List<string>();
            var currentLine = "";

            foreach (var currentWord in words)
            {

                if ((currentLine.Length > maxLength) || ((currentLine.Length + currentWord.Length) > maxLength))
                {
                    lines.Add(currentLine);
                    currentLine = "";
                }

                if (currentLine.Length > 0)
                    currentLine += " " + currentWord;
                else
                    currentLine += currentWord;

            }

            if (currentLine.Length > 0)
                lines.Add(currentLine);


            return lines;
        }

        public static string EscapeForCsv(this string str)
        {
            bool mustQuote = (str.Contains(",") || str.Contains("\"") || str.Contains("\r") || str.Contains("\n"));
            if (mustQuote)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\"");
                foreach (char nextChar in str)
                {
                    sb.Append(nextChar);
                    if (nextChar == '"')
                        sb.Append("\"");
                }

                sb.Append("\"");
                return sb.ToString();
            }

            return str;
        }

        public static string GetAlphanumericsOnly(this string str)
        {
            if (str == null)
                return null;

            return (new Regex("[^a-zA-Z0-9]")).Replace(str, "");
        }

        public static bool IsValidEmailAddress(this string m)
        {
            if (m.IsNullOrEmpty())
                return false;

            var reebonzEmailRegex = new Regex(ReebonzEmailAddress);
            var match = reebonzEmailRegex.Match(m);

            if (!match.Success)
            {
                Regex normalEmailRegex = new Regex(NormalEmailAddress);
                Match normalMatch = normalEmailRegex.Match(m);
                return normalMatch.Success;
            }

            return true;
        }

        /// <summary>
        /// Checks to be sure a phone number contains 10 digits as per American phone numbers.  
        /// If 'IsRequired' is true, then an empty string will return False. 
        /// If 'IsRequired' is false, then an empty string will return True.
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="isRequired"></param>
        /// <returns></returns>
        public static bool IsValidPhoneNumber(this string phone, bool isRequired)
        {
            if (string.IsNullOrEmpty(phone) & !isRequired)
                return true;

            if (string.IsNullOrEmpty(phone) & isRequired)
                return false;

            var requiredLength = 10;
            var cleaned = phone.RemoveNonNumeric();
            if (isRequired)
            {
                if (cleaned.Length == requiredLength)
                    return true;
                else
                    return false;
            }
            else
            {
                if (cleaned.Length == 0)
                    return true;
                else if (cleaned.Length > 0 & cleaned.Length < requiredLength)
                    return false;
                else if (cleaned.Length == requiredLength)
                    return true;
                else
                    return false; // should never get here
            }
        }

        /// <summary>
        /// Removes all non numeric characters from a string
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static string RemoveNonNumeric(this string phone)
        {
            return Regex.Replace(phone, @"[^0-9]+", "");
        }

        public static string[] SplitPascalCase(this string source)
        {
            return Regex.Split(source, @"(?<!^)(?=[A-Z])");
        }

        public static string SplitPascalCase(this string source, string delimiter = " ")
        {
            return source.Any(char.IsUpper) ? string.Join(delimiter, Regex.Split(source, "(?<!^)(?=[A-Z])")) : source;
        }


        //public static T ToEnum<T>(this string value) where T : struct, IConvertible
        //{
        //    if (string.IsNullOrEmpty(value))
        //    {
        //        return default(T);
        //    }

        //    var @enum = (T)Enum.Parse(typeof(T), value, true);
        //    return @enum;
        //}

        public static int ToIntOrMinus1(this string s)
        {
            int result;
            return int.TryParse(s, out result) ? result : -1;
        }

        public static int ToIntODefault(this string s, int @default = 0)
        {
            int result;
            return int.TryParse(s, out result) ? result : @default;
        }

        public static long ToLongODefault(this string s, long @default = 0)
        {
            return long.TryParse(s, out var result) ? result : @default;
        }

        public static int ToPossitiveIntOrDefault(this string s, int @default = 0)
        {
            var result = s.ToIntODefault(@default);
            return result > 0 ? result : @default;
        }

        public static int? ToIntOrNull(this string s)
        {
            return int.TryParse(s, out var result) ? result : (int?) null;
        }

        public static string ToTitleCase(this string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }

        public static bool IsNumeric(this string s)
        {
            double d;
            return double.TryParse(s, out d);
        }

        public static string StripHtml(this string input)
        {
            if (input.IsNullOrEmpty())
                return input;
            input = Regex.Replace(input, "<.*?>", "\n");
            return Regex.Replace(input, "<.*?>", string.Empty).Trim();
        }

        public static string ReplaceRegex(this string input, string pattern, string replacement)
        {
            return Regex.Replace(input, pattern, replacement);
        }

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static bool ToBool(this string input)
        {
            if (input == null) return false;

            bool.TryParse(input, out var result);
            return result;
        }

        public static Guid? ToNullableGuid(this string str)
        {
            return Guid.TryParse(str, out var result) ? result : (Guid?) null;
        }

        public static DateTime? ToNullableDateTime(this string str)
        {
            return DateTime.TryParse(str, out var result) ? result : (DateTime?) null;
        }

        public static Guid ToGuidOrDefault(this string str)
        {
            return Guid.TryParse(str, out var result) ? result : Guid.Empty;
        }

        public static bool NotEqualsIgnoreCase(this string str1, string str2)
        {
            return !EqualsIgnoreCase(str1, str2);
        }

        public static bool EqualsIgnoreCase(this string str1, string str2)
        {
            if (str1.IsNullOrEmpty() || str2.IsNullOrEmpty()) return false;

            return str1.Trim().Equals(str2.Trim(), StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool ContainsIgnoreCase(this string str1, string str2)
        {
            return str1.ToLower().Trim().Contains(str2.ToLower().Trim());
        }

        public static bool IsExcelFile(this string filePath)
        {
            var excelExtensions = new List<string>
            {
                ".xls",
                ".xlsx",
                ".csv"
            };
            return filePath.IsNotNullOrWhiteSpace() && excelExtensions.Any(extension => filePath.EndsWith(extension, System.StringComparison.OrdinalIgnoreCase));
        }

        public static string RemoveAccent(this string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        public static string FormatHtml(this string input)
        {
            if (input.IsNotNullOrEmpty())
            {
                input = input.Replace("/r/n", "<br/>");
            }

            return input;
        }

        public static string ReplaceInvalidChars(this string input)
        {
            return string.Join("_", input.Split(Path.GetInvalidFileNameChars()));
        }

        public static string ToPostUrl(this string fid)
        {
            if (fid.IsNotNullOrEmpty()) { return fid.Length >= 5 ? $"Fid:{fid.Substring(fid.Length - 5)}" : fid; }

            return string.Empty;
        }
             
        public static List<string> SplitClean(this string value, params char[] splitter)
        {
            if (value.IsNullOrEmpty()) return new List<string>();
            var values = value.Split(splitter, StringSplitOptions.RemoveEmptyEntries).ToList();
            return values.Where(x => x.IsNotNullOrEmpty()).Distinct().Select(x=>x.Trim()).ToList();
        }
        
        public static List<string> SplitKeywords(this string value)
        {
            return value.SplitClean(',', ';', '|');
        }

        public static List<string> SplitHashtags(this string value)
        {
            return value.SplitClean(' ', ',', ';', '|').Select(x=>x.Replace("#", "")).ToList();
        }

        public static List<string> SplitEmails(this string value)
        {
            return value.SplitClean(' ', ',', ';', '|');
        }       
        
        public static bool IsInIgnoreCase(this string value, params string[] list)
        {
            if (value.IsNotNullOrWhiteSpace()) return false;
            if (list.IsNullOrEmpty()) return false;

            var newValue = value.Trim().ToLower();
            var newList = list.Select(_ => _.Trim().ToLower()).ToList();

            return newList.Contains(newValue);
        }

        public static bool IsInIgnoreCase(this string value, List<string> list)
        {
            if (list.IsNullOrEmpty()) return false;
            return IsInIgnoreCase(value, list.ToArray());
        }

    }
}