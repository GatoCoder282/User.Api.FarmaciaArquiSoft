using FluentResults;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using User.Domain.Entities;
using User.Domain.Ports;

namespace User.Domain.Validators
{
    public class UserValidator : IUserValidator
    {
        private static readonly Regex AlphaSpaceRegex =
            new Regex(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ ]+$", RegexOptions.Compiled);

        private static readonly Regex EmailRegex =
            new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        private static readonly Regex DigitsRegex =
            new Regex(@"^\d+$", RegexOptions.Compiled);

        // CI: 5–12 dígitos, con letra opcional al inicio y/o al final
        // Ejemplos válidos: 1234567, E1234567, 1234567A, E1234567A
        private static readonly Regex CiRegex =
            new Regex(@"^[A-Z]?\d{5,12}[A-Z]?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public Result Validate(UserEntity user)
        {
            var result = Result.Ok();

            // NOMBRE (obligatorio, 2-50, letras/espacios)
            if (string.IsNullOrWhiteSpace(user.first_name))
                result = result.WithFieldError("first_name", "El nombre es obligatorio.");
            else
            {
                if (user.first_name.Length < 2 || user.first_name.Length > 50)
                    result = result.WithFieldError("first_name", "El nombre debe tener entre 2 y 50 caracteres.");
                if (!AlphaSpaceRegex.IsMatch(user.first_name))
                    result = result.WithFieldError("first_name", "El nombre solo debe contener letras y espacios.");
            }

            // Segundo apellido (opcional)
            if (!string.IsNullOrWhiteSpace(user.last_second_name))
            {
                if (user.last_second_name.Length < 2 || user.last_second_name.Length > 50)
                    result = result.WithFieldError("last_second_name", "El Segundo apellido debe tener entre 2 y 50 caracteres.");
                if (!AlphaSpaceRegex.IsMatch(user.last_second_name))
                    result = result.WithFieldError("last_second_name", "El Segundo apellido solo debe contener letras y espacios.");
            }

            // APELLIDO (obligatorio)
            if (string.IsNullOrWhiteSpace(user.last_first_name))
                result = result.WithFieldError("last_first_name", "El apellido es obligatorio.");
            else
            {
                if (user.last_first_name.Length < 2 || user.last_first_name.Length > 50)
                    result = result.WithFieldError("last_first_name", "El apellido debe tener entre 2 y 50 caracteres.");
                if (!AlphaSpaceRegex.IsMatch(user.last_first_name))
                    result = result.WithFieldError("last_first_name", "El apellido solo debe contener letras y espacios.");
            }

            // CORREO (obligatorio)
            if (string.IsNullOrWhiteSpace(user.mail))
                result = result.WithFieldError("mail", "El correo es obligatorio.");
            else
            {
                if (user.mail.Length > 100)
                    result = result.WithFieldError("mail", "El correo no debe exceder 100 caracteres.");
                if (!EmailRegex.IsMatch(user.mail))
                    result = result.WithFieldError("mail", "El correo no tiene un formato válido.");
            }

            // CI (obligatorio, 5–12 dígitos, letra opcional al inicio o al final)
            if (string.IsNullOrWhiteSpace(user.ci))
            {
                result = result.WithFieldError("ci", "El CI es obligatorio.");
            }
            else
            {
                if (!CiRegex.IsMatch(user.ci))
                {
                    result = result.WithFieldError(
                        "ci",
                        "El CI debe tener entre 5 y 12 dígitos, con una letra opcional al inicio o al final. Ejemplos: 1234567, E1234567, 1234567A, E1234567A."
                    );
                }
            }

            // TELÉFONO (string): obligatorio, solo dígitos, 6–10 caracteres
            var digits = user.phone.ToString(CultureInfo.InvariantCulture).Length;

            if (string.IsNullOrWhiteSpace(user.phone))
            {
                result = result.WithFieldError("phone", "El teléfono es obligatorio.");
            }
            else
            {
                if (!DigitsRegex.IsMatch(user.phone))
                    result = result.WithFieldError("phone", "El teléfono solo debe contener números.");

                if (user.phone.Length < 6 || user.phone.Length > 10)
                    result = result.WithFieldError("phone", "El teléfono debe tener entre 6 y 10 dígitos.");
            }

            return result;
        }
    }
}
