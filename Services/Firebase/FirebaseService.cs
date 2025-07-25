using apiBozzi.Models;

namespace apiBozzi.Services;

public class FirebaseService(HttpClient httpClient)
{
    public async Task<FirebaseLoginResponse> LoginAsync(string email, string password)
    {
        var url = Environment.GetEnvironmentVariable("FIREBASE_TOKEN_URI");
        var payload = new
        {
            email,
            password,
            returnSecureToken = true
        };

        var content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(url, content);
        var responseBody = await response.Content.ReadFromJsonAsync<FirebaseLoginResponse>();

        return responseBody;
    }
}