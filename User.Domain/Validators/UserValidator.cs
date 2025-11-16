using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace User.Domain.Validators
{
    public class UserValidator : IValidator<User>
    {
        private static readonly Regex AlphaSpaceRegex =
            new Regex(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ ]+$", RegexOptions.Compiled);

        private static readonly Regex EmailRegex =
            new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        private static readonly Regex DigitsRegex =
            new Regex(@"^\d+$", RegexOptions.Compiled);

        public Result Validate(User user)
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

            // CI (obligatorio, solo dígitos, 5-12)
            if (string.IsNullOrWhiteSpace(user.ci))
                result = result.WithFieldError("ci", "El CI es obligatorio.");
            else
            {
                if (!DigitsRegex.IsMatch(user.ci))
                    result = result.WithFieldError("ci", "El CI solo debe contener números.");
                if (user.ci.Length < 5 || user.ci.Length > 12)
                    result = result.WithFieldError("ci", "El CI debe tener entre 5 y 12 dígitos.");
            }

            // TELÉFONO (int): obligatorio, positivo, 6–10 dígitos
            // *int* no guarda ceros a la izquierda, así que la longitud se evalúa sobre el valor entero.

            var digits = user.phone.ToString(CultureInfo.InvariantCulture).Length;

            if (digits < 6 || digits > 10)
                result = result.WithFieldError("phone", "El teléfono debe tener entre 6 y 10 dígitos.");


            return result;
        }
    }
}
