using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace User.Domain.Validators
{
    public static class ErrorMapping
    {
        public static Dictionary<string, string> ToDictionary(this IReadOnlyList<IError> errors)
        {
            return errors
                .GroupBy(e => e.Metadata.TryGetValue("field", out var f) ? f?.ToString() ?? string.Empty : string.Empty)
                .ToDictionary(
                    g => g.Key,
                    g => string.Join("; ", g.Select(e => e.Message))
                );
        }
    }
}
