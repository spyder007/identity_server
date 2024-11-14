using System.Text;
using System.Security.Cryptography;

namespace Spydersoft.Identity.Extensions
{
    /// <summary>
    /// Class StringExtensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts to md5.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>System.String.</returns>
        public static string ToMd5(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return string.Empty;
            }

            var loweredBytes = Encoding.Default.GetBytes(s.ToLower());
            var buffer = MD5.HashData(loweredBytes);
            var sb = new StringBuilder(buffer.Length * 2);
            for (var i = 0; i < buffer.Length; i++)
            {
                _ = sb.Append(buffer[i].ToString("X2"));
            }

            return sb.ToString().ToLower();
        }

        /// <summary>
        /// Converts to md5.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>System.String.</returns>
        public static string ToGravatarUrl(this string s)
        {
            return $"https://www.gravatar.com/avatar/{s.ToMd5()}";
        }
    }
}
