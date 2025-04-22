using System.Text;
using System.Text.Json;
using DotNetEnv;
using TelegramBot.Persistence.Requests;

namespace TelegramBot.Services;

public class HttpClientService
{
    private readonly HttpClient _httpClient;
    private readonly string? _baseUrl = Environment.GetEnvironmentVariable("TARGET_URL");
    
    public HttpClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<string> GetAsync()
    {
        var response = await _httpClient.GetAsync(_baseUrl);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
    
    public async Task<string> PostAsync(SendMessageRequest message)
    {
        var response = await _httpClient.PostAsync(_baseUrl, new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}