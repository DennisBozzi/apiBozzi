using FluentValidation;

namespace apiBozzi.Utils;

public static class Util
{
    public static bool IsValidCpf(this string? cpf)
    {
        cpf = cpf.Replace(".", "").Replace("-", "");
        
        if (cpf.Length != 11 || !cpf.All(char.IsDigit))
            return false;
        
        if (cpf.All(c => c == cpf[0]))
            return false;

        var digits = cpf.Select(c => int.Parse(c.ToString())).ToArray();

        var sum = 0;
        for (int i = 0; i < 9; i++)
            sum += digits[i] * (10 - i);

        var firstDigit = (sum * 10) % 11;
        if (firstDigit >= 10) firstDigit = 0;

        if (digits[9] != firstDigit)
            return false;

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

    public static DateTime? ToUtcDateTime(this DateTime? dateTime)
    {
        if (!dateTime.HasValue)
            return null;

        return dateTime.Value.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc)
            : dateTime.Value;
    }
    
    public static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLower();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".txt" => "text/plain",
            ".zip" => "application/zip",
            _ => "application/octet-stream"
        };
    }
}