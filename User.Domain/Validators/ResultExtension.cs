using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace User.Domain.Validators
{
    public static class ResultExtensions
    {

        public static Result WithFieldError(this Result result, string field, string message)
        {
            var error = new Error(message).WithMetadata("field", field);
            return result.WithError(error);
        }
    }
}
