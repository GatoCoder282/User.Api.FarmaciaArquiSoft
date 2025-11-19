using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using User.Domain.Ports;

namespace User.Application.Services
{
    public class UsernameGenerator : IUsernameGenerator
    {
        public string GenerateBase(string first, string firstLast, string? secondLast)
        {
            static string Initial(string? s) => string.IsNullOrWhiteSpace(s) ? "" : s.Trim()[0].ToString();
            var raw = (Initial(first) + firstLast + Initial(secondLast)).ToLowerInvariant();

            var norm = raw.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(norm.Length);
            foreach (var ch in norm)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark) sb.Append(ch);
            }
            var s = sb.ToString().Normalize(NormalizationForm.FormC);
            s = Regex.Replace(s, "[^a-z0-9]", "");
            if (s.Length > 20) s = s[..20];
            if (string.IsNullOrEmpty(s)) s = "user";
            return s;
        }

        public string EnsureUnique(string baseUsername, IEnumerable<string> existing)
        {
            var set = new HashSet<string>(existing, StringComparer.OrdinalIgnoreCase);
            var candidate = baseUsername;
            var i = 1;
            while (set.Contains(candidate))
            {
                var suffix = i.ToString();
                var head = baseUsername;
                if (head.Length + suffix.Length > 20)
                    head = head[..Math.Max(1, 20 - suffix.Length)];
                candidate = head + suffix;
                i++;
            }
            return candidate;
        }
    }
}