using System.Collections;
using System.Data.SqlTypes;
using FluentValidation;
using FluentValidation.Validators;

namespace apiBozzi.Utils;

public static class Util
{
    public static bool IsEmpty(this object source)
    {
        var result = (source == null ||
                      source.Equals(0) ||
                      source.ToString().Equals("0") ||
                      (source is string && source.ToString().Trim().Equals(string.Empty)) ||
                      source.Equals(string.Empty) ||
                      (source is decimal && ((decimal)source).Equals(0)) ||
                      SqlDateTime.MinValue.Equals(source) ||
                      DateTime.MinValue.Equals(source)) ||
                     (source is ICollection && ((ICollection)source).Count == 0) ||
                     (source is IQueryable && !((IQueryable<object>)source).Any()) ||
                     TimeSpan.Zero.Equals(source);

        if (!result)
        {
            var collectionType =
                source.GetType()
                    .GetInterfaces()
                    .SingleOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));

            result = collectionType != null &&
                     ((int)collectionType.GetProperty("Count").GetValue(source, null)) == 0;
        }

        return result;
    }

    public static bool HasValue(this object source)
    {
        return !IsEmpty(source);
    }

    public static bool IsValidCpf(this string? cpf)
    {
        // Remove formatação
        cpf = cpf.Replace(".", "").Replace("-", "");

        // Verifica se tem 11 dígitos
        if (cpf.Length != 11 || !cpf.All(char.IsDigit))
            return false;

        // Verifica se todos os dígitos são iguais
        if (cpf.All(c => c == cpf[0]))
            return false;

        // Validação dos dígitos verificadores
        var digits = cpf.Select(c => int.Parse(c.ToString())).ToArray();

        // Primeiro dígito verificador
        var sum = 0;
        for (int i = 0; i < 9; i++)
            sum += digits[i] * (10 - i);

        var firstDigit = (sum * 10) % 11;
        if (firstDigit >= 10) firstDigit = 0;

        if (digits[9] != firstDigit)
            return false;

        // Segundo dígito verificador
        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += digits[i] * (11 - i);

        var secondDigit = (sum * 10) % 11;
        if (secondDigit >= 10) secondDigit = 0;

        return digits[10] == secondDigit;
    }

    public static bool IsValidEmail(this string? email)
    {
        var validator = new InlineValidator<string>();
        validator.RuleFor(x => x).EmailAddress();

        var result = validator.Validate(email);
        return result.IsValid;
    }
}