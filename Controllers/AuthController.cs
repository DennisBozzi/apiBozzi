using apiBozzi.Models;
using apiBozzi.Services.Firebase;
using Microsoft.AspNetCore.Mvc;

namespace apiBozzi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly FirebaseService _firebase;

    public AuthController(FirebaseService firebase)
    {
        _firebase = firebase;
    }

    [HttpPost("Login")]
    public async Task<FirebaseLoginResponse?> Login(string email, string password)
    {
        return await _firebase.LoginAsync(email, password);
    }
}