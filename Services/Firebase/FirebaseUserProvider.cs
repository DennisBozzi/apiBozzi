using System;
using apiBozzi.Models;
using Microsoft.AspNetCore.Http;

namespace apiBozzi.Services.Firebase;

public class FirebaseUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FirebaseUserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public FirebaseUser? Current => _httpContextAccessor.HttpContext?.User != null
        ? new FirebaseUser(_httpContextAccessor.HttpContext.User)
        : null;

    public bool IsAdmin => Current?.Admin ?? false;

    public string? AuthToken
    {
        get
        {
            var header = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(header))
            {
                return null;
            }

            const string bearerPrefix = "Bearer ";
            return header.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase)
                ? header[bearerPrefix.Length..].Trim()
                : header;
        }
    }
}